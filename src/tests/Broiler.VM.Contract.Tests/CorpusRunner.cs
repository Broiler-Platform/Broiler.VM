using Broiler.VM;
using Broiler.VM.Fixtures;

namespace Broiler.VM.Contract.Tests;

/// <summary>What one corpus artifact answered, and what was observed about how it got there.</summary>
internal sealed class CorpusObservation
{
    internal CorpusObservation(
        VmOutcome outcome,
        VmReason reason,
        int profileDiagnosticCode,
        VmBudgetDimension dimension,
        VmBudgetScope scope,
        bool producedHandle,
        FixtureReadOrderRecorder recorder)
    {
        Outcome = outcome;
        Reason = reason;
        ProfileDiagnosticCode = profileDiagnosticCode;
        Dimension = dimension;
        Scope = scope;
        ProducedHandle = producedHandle;
        Recorder = recorder;
    }

    internal VmOutcome Outcome { get; }

    internal VmReason Reason { get; }

    internal int ProfileDiagnosticCode { get; }

    internal VmBudgetDimension Dimension { get; }

    internal VmBudgetScope Scope { get; }

    internal bool ProducedHandle { get; }

    internal FixtureReadOrderRecorder Recorder { get; }
}

/// <summary>
/// Runs one corpus artifact through the one verification entry point, and records what the
/// bounded reader and the bounded allocator did in what order.
/// </summary>
/// <remarks>
/// <para>
/// A fresh runtime per artifact, deliberately. The corpus includes artifacts that exhaust an
/// allowance, and an allowance never refunds, so sharing one runtime would make an entry's answer
/// depend on which entries ran before it - the exact nondeterministic failure class the gate
/// forbids.
/// </para>
/// <para>
/// The recorder is attached to the descriptor rather than held statically, so nothing here is
/// shared between artifacts or between suites running side by side.
/// </para>
/// </remarks>
internal static class CorpusRunner
{
    /// <summary>The component root, found by walking up to the directory holding the solution.</summary>
    internal static string Root { get; } = FindRoot();

    internal static CorpusObservation Run(
        byte[] payload,
        uint descriptorFormatVersion,
        ulong artifactBytesRequest)
    {
        var recorder = new FixtureReadOrderRecorder();

        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.Conforming, recorder));

        using var runtime = FixtureComposition.Runtime(catalog);

        var descriptor = new VmArtifactDescriptor(
            FixtureVmProfile.Id,
            descriptorFormatVersion,
            FixtureVmProfile.Manifest,
            RequestedLimits(artifactBytesRequest),
            VmCallerIdentity.FromCanonicalIdentity("corpus://vm-2"));

        var result = runtime.Verify(in descriptor, payload, CancellationToken.None);
        var producedHandle = result.TryGetArtifact(out var artifact);

        artifact?.Dispose();

        return new CorpusObservation(
            result.Outcome,
            result.Reason,
            result.Diagnostics.ProfileDiagnosticCode,
            result.Diagnostics.ExhaustedDimension,
            result.Diagnostics.ExhaustedScope,
            producedHandle,
            recorder);
    }

    /// <summary>
    /// A request that tightens exactly one dimension and says nothing about the other fourteen.
    /// </summary>
    /// <remarks>
    /// Every other slot is TOP rather than zero. A vector of zeros would not be "a request for one
    /// dimension" - it would be a request to reduce every ceiling to nothing, and the artifact would
    /// then fail for a reason the case was not written to produce.
    /// </remarks>
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

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Broiler.VM.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            "No directory above the test binaries holds Broiler.VM.slnx.");
    }
}
