using System.Collections.Concurrent;
using System.Collections.Immutable;
using Broiler.VM;
using Broiler.VM.Fixtures;
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
            CorpusRunner.RequestedLimits(configuration.ArtifactBytesRequest),
            VmCallerIdentity.FromCanonicalIdentity("corpus://ubc-1"));

        var outcome = profile.Verifier.Verify(in descriptor, payload, context, cancellation.Token);
        return new UbcCorpusObservation(outcome, meter);
    }
}
