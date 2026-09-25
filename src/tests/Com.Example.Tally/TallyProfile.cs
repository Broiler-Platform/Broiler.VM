using Broiler.VM;
using Broiler.VM.Ubc;
using System.Collections.Immutable;

namespace Com.Example.Tally;

/// <summary>
/// The fixture family's registration and declaration: everything a composition root needs to build
/// its descriptor through <see cref="UbcDescriptors.Build"/> over the emitters it composes.
/// </summary>
/// <remarks>
/// There is deliberately no descriptor here. A family names no emitter, so the descriptor - whose rows
/// 6 and 7 are the universal bytecode's verifier and the composed form's executor - can only be built
/// where the forms are chosen, which is the composition root.
/// </remarks>
public static class TallyProfile
{
    /// <summary>The family's identity.</summary>
    public static VmProfileId Id { get; } = VmProfileId.Parse(TallyTable.Identity);

    /// <summary>Its one feature manifest.</summary>
    public static VmFeatureManifestId Manifest { get; } = TallyTable.Table.Manifest;

    /// <summary>The family slot the program list's artifacts put it in.</summary>
    public const byte Slot = 1;

    /// <summary>The kind ID of a completion.</summary>
    public const int ResultKindId = 701;

    /// <summary>The kind ID of a fault.</summary>
    public const int FaultKindId = 702;

    /// <summary>The kind ID of a suspension's projection.</summary>
    public const int SuspensionKindId = 703;

    /// <summary>
    /// The artifact provider a composition registers to answer the family's guest loads: optional, so a
    /// composition that registers none still composes the family, and its guest loads trap.
    /// </summary>
    public static VmHostCapabilityDescriptor ProviderCapability { get; } =
        new(
            VmCapabilityId.Parse("com.example.tally.program-provider"),
            version: 1,
            VmCapabilitySignatureId.FromCanonicalDescription("(program-name)->artifact"),
            VmCapabilityKind.ArtifactProvider,
            VmCapabilityReentrancy.NonReentrant,
            VmCapabilityThreadAffinity.CallerThread,
            VmExceptionTranslation.TerminateOperation);

    /// <summary>The family's registration: its table, its hook, and its contract integers.</summary>
    public static UbcFamilyRegistration<TallyFamily> Registration { get; } =
        new(TallyTable.Identity, [TallyTable.Table], new TallyVerifier(), authoredUbcContractVersion: 1);

    /// <summary>The family's descriptor rows.</summary>
    public static UbcFamilyDeclaration Declaration { get; } = Declare(Defaults(), Maxima());

    /// <summary>
    /// The same rows with other limit vectors: for a composition that means to show which of a family's
    /// declared vectors reach its neighbours and which do not.
    /// </summary>
    public static UbcFamilyDeclaration Declare(VmLimitVector defaults, VmLimitVector maxima)
    {
        VmDiagnosticsIdentity.TryCreate(Id, "com.example.tally.diagnostics", out var diagnostics);

        return new UbcFamilyDeclaration(
            profileId: Id,
            displayName: "Example Tally",
            descriptorRevision: 1,
            artifactRepresentationKind: VmArtifactRepresentationKind.Decoded,
            artifactLifetimeKind: VmArtifactLifetimeKind.Managed,
            supportsConcurrentVerification: true,
            threadAffinity: VmThreadAffinity.Agile,
            cancellationPollBound: 256,
            abandonBudget: 0,
            limitDefaults: defaults,
            profileHardMaxima: maxima,
            budgetDeclarationMatrix: Matrix(),
            hostCapabilityDescriptors: ImmutableArray.Create(
                new VmCapabilityImport(ProviderCapability, VmCapabilityImportKind.Optional)),

            // Guest loads are declared because the program list loads one: a program a provider the
            // composition registers answers with, verified by this descriptor's verifier, run in the
            // same emitter.
            guestInitiatedLoads: VmGuestLoadDeclaration.Declared(
                minimumProviderCapabilityVersion: 1,
                profileHardMaxima: new VmGuestLoadBounds(
                    nestedLoadDepth: 2,
                    nestedLoadFanOut: 64,
                    nestedLoadBytes: 1024 * 1024,
                    verifierWork: 10_000_000),
                verifierWorkToFuelRate: 1),
            asynchronousInstantiation: VmDeclaration.NotDeclared,
            externalSuspension: VmDeclaration.NotDeclared,
            payloadKindIdRange: new VmPayloadKindIdRange(700, 799),
            authoredCoreContractVersion: 1,
            conformanceManifestId: VmConformanceManifestId.Create("com.example.tally.conformance"),
            conformanceManifestVersion: 1,
            diagnosticsIdentity: diagnostics,
            packageIdentity: new VmPackageIdentity("Com.Example.Tally", "1.0.0", "example-application"),
            faultRecovery: VmFaultRecovery.InstanceRecoverable,
            maxUnchargedWork: 256,
            chargingGranularity: 1,
            artifactSharing: VmArtifactSharing.Shareable);
    }

    /// <summary>The bounded defaults a host adopts when it states none.</summary>
    public static VmLimitVector Defaults()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        values[(int)VmBudgetDimension.Fuel] = 1_000_000;
        values[(int)VmBudgetDimension.WallClock] = 10_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 16L * 1024 * 1024;
        values[(int)VmBudgetDimension.HostCalls] = 1_024;
        values[(int)VmBudgetDimension.NestedLoadFanOut] = 16;
        values[(int)VmBudgetDimension.NestedLoadBytes] = 256 * 1024;
        values[(int)VmBudgetDimension.VerifierWork] = 5_000_000;
        values[(int)VmBudgetDimension.LiveBytes] = 16L * 1024 * 1024;
        values[(int)VmBudgetDimension.CallDepth] = 64;
        values[(int)VmBudgetDimension.NestedLoadDepth] = 2;
        values[(int)VmBudgetDimension.ArtifactBytes] = 1024 * 1024;
        values[(int)VmBudgetDimension.SectionCount] = 32;
        values[(int)VmBudgetDimension.DeclaredCount] = 65_536;
        values[(int)VmBudgetDimension.StructuralDepth] = 4;
        values[(int)VmBudgetDimension.LiveRuntimes] = 16;

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }

    /// <summary>The hard maxima a host may tighten and never loosen; they bind this family's artifacts alone.</summary>
    public static VmLimitVector Maxima()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        values[(int)VmBudgetDimension.Fuel] = 100_000_000;
        values[(int)VmBudgetDimension.WallClock] = 60_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 256L * 1024 * 1024;
        values[(int)VmBudgetDimension.HostCalls] = 1_000_000;
        values[(int)VmBudgetDimension.NestedLoadFanOut] = 64;
        values[(int)VmBudgetDimension.NestedLoadBytes] = 1024 * 1024;
        values[(int)VmBudgetDimension.VerifierWork] = 100_000_000;
        values[(int)VmBudgetDimension.LiveBytes] = 256L * 1024 * 1024;
        values[(int)VmBudgetDimension.CallDepth] = 4_096;
        values[(int)VmBudgetDimension.NestedLoadDepth] = 2;
        values[(int)VmBudgetDimension.ArtifactBytes] = 16L * 1024 * 1024;
        values[(int)VmBudgetDimension.SectionCount] = 64;
        values[(int)VmBudgetDimension.DeclaredCount] = 1_048_576;
        values[(int)VmBudgetDimension.StructuralDepth] = 16;
        values[(int)VmBudgetDimension.LiveRuntimes] = 64;

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }

    /// <summary>Every dimension is charged: the family loads guest programs, so the nested-load rows apply too.</summary>
    private static VmBudgetDeclarationMatrix Matrix()
    {
        var rows = new VmBudgetApplicability[VmBudgetDimensions.Count];

        for (var index = 0; index < rows.Length; index++)
        {
            rows[index] = VmBudgetApplicability.Charged;
        }

        VmBudgetDeclarationMatrix.TryCreate(rows, out var matrix);
        return matrix;
    }
}

/// <summary>A completion: the entry unit's word results and tally results, in order.</summary>
public sealed class TallyResult : IVmProfilePayload
{
    internal TallyResult(ImmutableArray<long> words, ImmutableArray<long> tallies)
    {
        Identity = new VmPayloadIdentity(TallyProfile.Id, TallyProfile.ResultKindId, 1);
        Words = words;
        Tallies = tallies;
    }

    /// <inheritdoc/>
    public VmPayloadIdentity Identity { get; }

    /// <summary>The word results, each as its sixty-four bits.</summary>
    public ImmutableArray<long> Words { get; }

    /// <summary>The tally results' amounts.</summary>
    public ImmutableArray<long> Tallies { get; }

    /// <inheritdoc/>
    public override string ToString() => $"completed words=[{string.Join(",", Words)}] tallies=[{string.Join(",", Tallies)}]";
}

/// <summary>What a Tally fault is.</summary>
public enum TallyFaultKind
{
    /// <summary>A trap of the family's vocabulary.</summary>
    Trap = 1,

    /// <summary>The universal unreachable: <c>trap 0 0</c>.</summary>
    Unreachable = 2,

    /// <summary>A thrown tally no region caught.</summary>
    Uncaught = 3,

    /// <summary>An invocation naming no entry.</summary>
    NoSuchEntry = 4,
}

/// <summary>A language fault of the fixture family.</summary>
public sealed class TallyFault : IVmProfilePayload
{
    internal TallyFault(TallyFaultKind kind, ushort code, long amount, VmReason reason = VmReason.None)
    {
        Identity = new VmPayloadIdentity(TallyProfile.Id, TallyProfile.FaultKindId, 1);
        Kind = kind;
        Code = code;
        Amount = amount;
        Reason = reason;
    }

    /// <inheritdoc/>
    public VmPayloadIdentity Identity { get; }

    /// <summary>What kind of fault.</summary>
    public TallyFaultKind Kind { get; }

    /// <summary>For a trap, its code.</summary>
    public ushort Code { get; }

    /// <summary>For an uncaught throw, the thrown tally's amount.</summary>
    public long Amount { get; }

    /// <summary>For a load-refused trap, the reason the core answered the guest load with.</summary>
    public VmReason Reason { get; }

    /// <inheritdoc/>
    public override string ToString() => Reason == VmReason.None
        ? $"faulted {Kind} code={Code} amount={Amount}"
        : $"faulted {Kind} code={Code} amount={Amount} load={Reason}";
}

/// <summary>What a host sees of a suspension: the tally the program yielded.</summary>
public sealed class TallySuspension : IVmProfilePayload
{
    internal TallySuspension(long amount)
    {
        Identity = new VmPayloadIdentity(TallyProfile.Id, TallyProfile.SuspensionKindId, 1);
        Amount = amount;
    }

    /// <inheritdoc/>
    public VmPayloadIdentity Identity { get; }

    /// <summary>The yielded tally's amount.</summary>
    public long Amount { get; }

    /// <inheritdoc/>
    public override string ToString() => $"suspended amount={Amount}";
}
