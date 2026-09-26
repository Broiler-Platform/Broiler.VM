using System.Collections.Concurrent;
using System.Collections.Immutable;
using Broiler.VM;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using Broiler.VM.Ubc;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// The meter a corpus verification runs under: it enforces every allowance and ceiling it is given,
/// answers a poll false once the token is cancelled, and remembers the most verifier work it saw
/// between two polls.
/// </summary>
/// <remarks>
/// It holds no clock. A wall-clock ceiling compared against elapsed time would make an answer depend on
/// how busy the machine was, so a poll refuses for a cancelled token or a wall-clock ceiling of zero
/// and for nothing else.
/// </remarks>
internal sealed class UbcCorpusMeter : IVmMeter
{
    private readonly VmLimitVector ceilings;
    private readonly CancellationToken token;
    private readonly ulong[] charged = new ulong[VmBudgetDimensions.Count];
    private ulong workSinceLastPoll;

    internal UbcCorpusMeter(VmLimitVector ceilings, CancellationToken token)
    {
        this.ceilings = ceilings;
        this.token = token;
    }

    /// <summary>How much of <paramref name="dimension"/> was charged and admitted.</summary>
    internal ulong Charged(VmBudgetDimension dimension) => charged[(int)dimension];

    /// <summary>The dimension of the first refused charge, if one was refused.</summary>
    internal VmBudgetDimension? FirstRefusal { get; private set; }

    /// <summary>How many polls were made.</summary>
    internal int Polls { get; private set; }

    /// <summary>The most verifier work charged between two polls, or before the first.</summary>
    internal ulong MostWorkBetweenPolls { get; private set; }

    public bool TryCharge(VmBudgetDimension dimension, ulong amount)
    {
        var index = (int)dimension;
        var ceiling = ceilings[dimension];

        if (amount > ceiling || charged[index] > ceiling - amount)
        {
            FirstRefusal ??= dimension;
            return false;
        }

        charged[index] += amount;

        if (dimension is VmBudgetDimension.VerifierWork)
        {
            workSinceLastPoll += amount;
            MostWorkBetweenPolls = Math.Max(MostWorkBetweenPolls, workSinceLastPoll);
        }

        return true;
    }

    public bool Poll()
    {
        Polls++;
        workSinceLastPoll = 0;
        return !token.IsCancellationRequested && ceilings[VmBudgetDimension.WallClock] != 0;
    }

    public void ReportRetained(VmBudgetDimension dimension, ulong amount)
    {
    }

    public void ReportReleased(VmBudgetDimension dimension, ulong amount)
    {
    }
}

/// <summary>A verification context carrying the corpus meter and the ceilings an entry was pinned under.</summary>
internal sealed class UbcCorpusVerificationContext : IVmVerificationContext
{
    internal UbcCorpusVerificationContext(VmLimitVector verificationCeilings, UbcCorpusMeter meter)
    {
        Ceilings = new VmEffectiveCeilings(verificationCeilings, verificationCeilings);
        Recorder = meter;
    }

    internal UbcCorpusMeter Recorder { get; }

    public VmEffectiveCeilings Ceilings { get; }

    public IVmMeter Meter => Recorder;

    public ImmutableArray<VmHostCapabilityDescriptor> RegisteredCapabilities => ImmutableArray<VmHostCapabilityDescriptor>.Empty;

    public bool TryGetCapabilityDescriptor(VmCapabilityId capabilityId, int version, out VmHostCapabilityDescriptor descriptor)
    {
        descriptor = default;
        return false;
    }
}

/// <summary>What one corpus artifact answered, with the verified program when it verified.</summary>
internal sealed class UbcCorpusObservation
{
    internal UbcCorpusObservation(VmVerifierOutcome outcome, UbcCorpusMeter meter)
    {
        Outcome = outcome;
        Meter = meter;
    }

    internal VmVerifierOutcome Outcome { get; }

    internal UbcCorpusMeter Meter { get; }

    internal UbcVerifiedProgram? Program => Outcome.State as UbcVerifiedProgram;

    /// <summary>The answer in the manifest's shape.</summary>
    internal UbcCorpusAnswer Answer => new(
        Outcome.Category,
        Outcome.Reason,
        Outcome.ProfileDiagnosticCode,
        Outcome.ExhaustedDimension,
        Outcome.ExhaustedScope,
        UbcCorpusAnswer.Format(Outcome.Position));
}

/// <summary>
/// Replays one corpus artifact through the universal bytecode's verifier, called directly: no catalog,
/// no runtime, no registration. A fresh meter per artifact, because an allowance never refunds and an
/// entry's answer must not depend on the entries before it.
/// </summary>
internal static class UbcCorpusRunner
{
    private static readonly ConcurrentDictionary<UbcCorpusHookMode, VmProfileDescriptor> Descriptors = new();

    /// <summary>The descriptor of one hook mode, built once and never registered.</summary>
    internal static VmProfileDescriptor Descriptor(UbcCorpusHookMode mode) =>
        Descriptors.GetOrAdd(mode, static hook => UbcCorpusFamily.Descriptor(hook));

    /// <summary>The verification ceilings an entry runs under: the family's defaults, the entry's replacements, then its request.</summary>
    internal static VmLimitVector Ceilings(UbcCorpusConfiguration configuration)
    {
        var defaults = UbcCorpusFamily.Declaration().LimitDefaults;
        var values = new ulong[VmBudgetDimensions.Count];

        foreach (var dimension in VmBudgetDimensions.All)
        {
            values[(int)dimension] = configuration.Ceilings.TryGetValue(dimension, out var replaced) ? replaced : defaults[dimension];
        }

        if (configuration.ArtifactBytesRequest != 0)
        {
            var index = (int)VmBudgetDimension.ArtifactBytes;
            values[index] = Math.Min(values[index], configuration.ArtifactBytesRequest);
        }

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }

    internal static UbcCorpusObservation Run(byte[] payload, UbcCorpusConfiguration configuration) =>
        Run(payload, configuration, Descriptor(configuration.Hook));

    internal static UbcCorpusObservation Run(byte[] payload, UbcCorpusConfiguration configuration, VmProfileDescriptor profile)
    {
        var ceilings = Ceilings(configuration);

        using var cancellation = new CancellationTokenSource();

        if (configuration.Cancelled)
        {
            cancellation.Cancel();
        }

        var meter = new UbcCorpusMeter(ceilings, cancellation.Token);
        var context = new UbcCorpusVerificationContext(ceilings, meter);

        var descriptor = new VmArtifactDescriptor(
            UbcCorpusFamily.ProfileId,
            configuration.DescriptorFormatVersion,
            VmFeatureManifestId.Parse(configuration.DescriptorManifest),
            RequestedLimits(configuration.ArtifactBytesRequest),
            VmCallerIdentity.FromCanonicalIdentity("corpus://ubc-1"));

        var outcome = profile.Verifier.Verify(in descriptor, payload, context, cancellation.Token);
        return new UbcCorpusObservation(outcome, meter);
    }

    /// <summary>The limits an artifact descriptor requests: nothing, or an artifact-bytes tightening alone.</summary>
    internal static VmLimitVector RequestedLimits(ulong artifactBytesRequest)
    {
        if (artifactBytesRequest == 0)
        {
            return default;
        }

        var values = new ulong[VmBudgetDimensions.Count];
        Array.Fill(values, ulong.MaxValue);
        values[(int)VmBudgetDimension.ArtifactBytes] = artifactBytesRequest;

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }
}

/// <summary>
/// How one corpus entry is presented to the verifier: the descriptor's format version and manifest,
/// the ceilings of the verification context, the hook the descriptor is built with, and whether the
/// token is already cancelled.
/// </summary>
/// <remarks>
/// ADR 0011's schema carries two of these, <c>descriptorFormatVersion</c> and
/// <c>artifactBytesRequest</c>. The universal bytecode's walk refuses on four more things a
/// presentation decides - the descriptor's manifest, a ceiling other than artifact bytes, the hook, and
/// a cancellation - so the manifest carries them beside the two, and its comment says so.
/// </remarks>
internal sealed class UbcCorpusConfiguration
{
    internal UbcCorpusConfiguration(
        uint descriptorFormatVersion = UbcFormat.FormatVersion,
        ulong artifactBytesRequest = 0,
        string descriptorManifest = UbcCorpusFamily.BaseManifestText,
        UbcCorpusHookMode hook = UbcCorpusHookMode.Standard,
        bool cancelled = false,
        ImmutableSortedDictionary<VmBudgetDimension, ulong>? ceilings = null)
    {
        DescriptorFormatVersion = descriptorFormatVersion;
        ArtifactBytesRequest = artifactBytesRequest;
        DescriptorManifest = descriptorManifest;
        Hook = hook;
        Cancelled = cancelled;
        Ceilings = ceilings ?? ImmutableSortedDictionary<VmBudgetDimension, ulong>.Empty;
    }

    internal static UbcCorpusConfiguration Default { get; } = new();

    /// <summary>The format version the presenting artifact descriptor declares.</summary>
    internal uint DescriptorFormatVersion { get; }

    /// <summary>An artifact-bytes tightening the artifact descriptor requests, or zero for none.</summary>
    internal ulong ArtifactBytesRequest { get; }

    /// <summary>The feature manifest the presenting artifact descriptor names.</summary>
    internal string DescriptorManifest { get; }

    /// <summary>Which hook the descriptor is built with.</summary>
    internal UbcCorpusHookMode Hook { get; }

    /// <summary>Whether the cancellation token is cancelled before verification begins.</summary>
    internal bool Cancelled { get; }

    /// <summary>Verification ceilings that replace the family's declared default for their dimension.</summary>
    internal ImmutableSortedDictionary<VmBudgetDimension, ulong> Ceilings { get; }

    internal static UbcCorpusConfiguration With(VmBudgetDimension dimension, ulong value) =>
        new(ceilings: ImmutableSortedDictionary<VmBudgetDimension, ulong>.Empty.Add(dimension, value));
}

/// <summary>An answer, as the manifest writes it: the outcome tuple of ADR 0011's schema and a position.</summary>
internal readonly record struct UbcCorpusAnswer(
    VmOutcome Outcome,
    VmReason Reason,
    int ProfileDiagnosticCode,
    VmBudgetDimension Dimension,
    VmBudgetScope Scope,
    string Position)
{
    /// <summary>The written form of a position: section index, byte offset and the two coordinates, colon-separated.</summary>
    internal static string Format(VmSourcePosition position) =>
        string.Create(
            CultureInfo.InvariantCulture,
            $"{position.SectionIndex}:{position.ByteOffset}:{position.ProfileCoordinate0}:{position.ProfileCoordinate1}");

    /// <summary>What a pinned row writes where it pins no position.</summary>
    internal const string Unpinned = "-";
}

/// <summary>
/// Replays the retained corpus from its files and prints its failure-class table: one line per entry,
/// its id and the answer it gave, with the entry's pinned answer compared. The contract suite asserts
/// the same answers; the universal bytecode's fixture composition compiles this file and runs it in
/// every publish mode, so the three tables can be compared byte for byte.
/// </summary>
internal static class UbcCorpusReplay
{
    /// <summary>Replays <paramref name="directory"/>'s manifest; answers the number of entries that disagree.</summary>
    internal static int Replay(string directory, TextWriter output)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(directory, "manifest.json")));
        var failures = 0;
        var entries = 0;

        foreach (var element in document.RootElement.GetProperty("entries").EnumerateArray())
        {
            entries++;
            var id = element.GetProperty("id").GetString()!;
            var bytes = File.ReadAllBytes(Path.Combine(directory, element.GetProperty("file").GetString()!));

            if (bytes.Length != element.GetProperty("bytes").GetInt32() ||
                !string.Equals(Convert.ToHexStringLower(SHA256.HashData(bytes)), element.GetProperty("sha256").GetString(), StringComparison.Ordinal))
            {
                failures++;
                output.WriteLine($"FAIL {id} MUTATED");
                continue;
            }

            var ceilings = ImmutableSortedDictionary.CreateBuilder<VmBudgetDimension, ulong>();

            foreach (var ceiling in element.GetProperty("ceilings").EnumerateObject())
            {
                ceilings.Add(Enum.Parse<VmBudgetDimension>(ceiling.Name), ceiling.Value.GetUInt64());
            }

            var configuration = new UbcCorpusConfiguration(
                element.GetProperty("descriptorFormatVersion").GetUInt32(),
                element.GetProperty("artifactBytesRequest").GetUInt64(),
                element.GetProperty("descriptorManifest").GetString()!,
                Enum.Parse<UbcCorpusHookMode>(element.GetProperty("hook").GetString()!),
                element.GetProperty("cancelled").GetBoolean(),
                ceilings.ToImmutable());

            var exact = string.Equals(element.GetProperty("pinning").GetString(), "Exact", StringComparison.Ordinal);
            var pinned = element.GetProperty(exact ? "expected" : "recorded");
            var answer = UbcCorpusRunner.Run(bytes, configuration).Answer;
            var pinnedPosition = pinned.GetProperty("position").GetString()!;

            var agrees =
                string.Equals(answer.Outcome.ToString(), pinned.GetProperty("outcome").GetString(), StringComparison.Ordinal) &&
                string.Equals(answer.Reason.ToString(), pinned.GetProperty("reason").GetString(), StringComparison.Ordinal) &&
                answer.ProfileDiagnosticCode == pinned.GetProperty("profileDiagnosticCode").GetInt32() &&
                string.Equals(answer.Dimension.ToString(), pinned.GetProperty("dimension").GetString(), StringComparison.Ordinal) &&
                string.Equals(answer.Scope.ToString(), pinned.GetProperty("scope").GetString(), StringComparison.Ordinal) &&
                (pinnedPosition == UbcCorpusAnswer.Unpinned || string.Equals(answer.Position, pinnedPosition, StringComparison.Ordinal));

            failures += agrees ? 0 : 1;
            output.WriteLine(string.Create(
                CultureInfo.InvariantCulture,
                $"{(agrees ? "ok  " : "FAIL")} {id} {answer.Outcome} {answer.Reason} {answer.ProfileDiagnosticCode} {answer.Dimension} {answer.Scope} {answer.Position}"));
        }

        output.WriteLine(string.Create(CultureInfo.InvariantCulture, $"ubc-1 corpus: {entries} entries, {failures} disagree"));
        return entries == 0 ? 1 : failures;
    }
}
