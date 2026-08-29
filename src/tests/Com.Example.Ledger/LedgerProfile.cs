using Broiler.VM;
using System.Collections.Immutable;

namespace Com.Example.Ledger;

/// <summary>
/// The application-local ledger profile, exposed the way the contract requires: one static accessor
/// on the profile's own type, naming its own descriptor.
/// </summary>
/// <remarks>
/// <para>
/// There is deliberately no aggregate type listing several profiles, here or in the calculator. One
/// would reference every profile assembly and defeat the exact-closure reports a composition depends
/// on, which is why the core forbids such a type by name and why a consumer must not invent one
/// either.
/// </para>
/// <para>
/// The identity is <c>com.example.ledger</c>: a reverse-domain ID under the documentation-reserved
/// domain, outside the reserved <c>broiler</c> first label. Its payload kind range is disjoint from
/// the calculator's, which is what lets one composition host both.
/// </para>
/// </remarks>
public static class LedgerProfile
{
    /// <summary>This profile's identity.</summary>
    public static VmProfileId Id { get; } = VmProfileId.Parse("com.example.ledger");

    /// <summary>Its one accepted feature manifest.</summary>
    public static VmFeatureManifestId Manifest { get; } =
        VmFeatureManifestId.Parse("com.example.ledger.base");

    /// <summary>The kind ID this profile stamps on a balance.</summary>
    public const int BalanceKindId = 601;

    /// <summary>The kind ID it stamps on a language fault.</summary>
    public const int FaultKindId = 602;

    /// <summary>The descriptor a composition root names directly.</summary>
    public static VmProfileDescriptor Descriptor { get; } = Build();

    /// <summary>
    /// Projects a balance out of an invocation result, or returns false when the result carries no
    /// payload of this profile's.
    /// </summary>
    public static bool TryGetBalance(in VmInvocationResult result, out LedgerBalance balance) =>
        result.TryGetPayload(out balance);

    /// <summary>Projects a ledger fault out of an invocation result.</summary>
    public static bool TryGetFault(in VmInvocationResult result, out LedgerFault fault) =>
        result.TryGetPayload(out fault);

    private static VmProfileDescriptor Build()
    {
        VmDiagnosticsIdentity.TryCreate(Id, "com.example.ledger.diagnostics", out var diagnostics);

        return new VmProfileDescriptor(
            profileId: Id,
            displayName: "Example Ledger",
            descriptorRevision: 1,
            supportedFormatVersions: new VmFormatVersionRange(1, 1),
            acceptedFeatureManifests: ImmutableArray.Create(Manifest),
            verifier: new LedgerVerifier(Id),
            executorFactory: environment => new LedgerExecutor(Id, environment),
            artifactRepresentationKind: VmArtifactRepresentationKind.Decoded,
            artifactLifetimeKind: VmArtifactLifetimeKind.Managed,
            supportsConcurrentVerification: true,
            threadAffinity: VmThreadAffinity.Agile,
            cancellationPollBound: 256,
            abandonBudget: 0,
            limitDefaults: Defaults(),
            profileHardMaxima: Maxima(),
            budgetDeclarationMatrix: Matrix(),

            // One optional import, and nothing else. A composition may bind it or not; both
            // branches are reachable, and the profile learns which by asking the one question the
            // capability table answers.
            hostCapabilityDescriptors: ImmutableArray.Create(
                new VmCapabilityImport(LedgerCapabilities.Stamp, VmCapabilityImportKind.Optional)),
            guestInitiatedLoads: VmGuestLoadDeclaration.NotDeclared,
            asynchronousInstantiation: VmDeclaration.NotDeclared,
            externalSuspension: VmDeclaration.NotDeclared,
            payloadKindIdRange: new VmPayloadKindIdRange(600, 699),
            authoredCoreContractVersion: 1,
            conformanceManifestId: VmConformanceManifestId.Create("com.example.ledger.conformance"),
            conformanceManifestVersion: 1,
            diagnosticsIdentity: diagnostics,

            // Not a Broiler package, and the ID says so. The reserved-namespace check compares a
            // reserved ID against a Broiler package prefix; this ID claims nothing reserved, so the
            // check has nothing to hold it to and the honest package identity is this one.
            packageIdentity: new VmPackageIdentity("Com.Example.Ledger", "1.0.0", "example-application"),
            faultRecovery: VmFaultRecovery.InstanceRecoverable,
            maxUnchargedWork: 256,
            chargingGranularity: 1,
            artifactSharing: VmArtifactSharing.Shareable);
    }

    /// <summary>
    /// The bounded defaults a host adopts when it does not state its own. No member is unbounded.
    /// </summary>
    /// <remarks>
    /// Different from the calculator's in every dimension the two profiles use differently: this one
    /// frames, so it needs section count and structural depth above one; it imports a capability, so
    /// it needs a host-call allowance; and its artifacts are records rather than programs, so it
    /// wants a larger declared count and a smaller fuel allowance per unit of it.
    /// </remarks>
    private static VmLimitVector Defaults()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        values[(int)VmBudgetDimension.Fuel] = 500_000;
        values[(int)VmBudgetDimension.WallClock] = 5_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 2 * 1024 * 1024;
        values[(int)VmBudgetDimension.HostCalls] = 1_024;
        values[(int)VmBudgetDimension.NestedLoadFanOut] = 0;
        values[(int)VmBudgetDimension.NestedLoadBytes] = 0;
        values[(int)VmBudgetDimension.VerifierWork] = 500_000;
        values[(int)VmBudgetDimension.LiveBytes] = 2 * 1024 * 1024;
        values[(int)VmBudgetDimension.CallDepth] = 1;
        values[(int)VmBudgetDimension.NestedLoadDepth] = 0;
        values[(int)VmBudgetDimension.ArtifactBytes] = 512 * 1024;
        values[(int)VmBudgetDimension.SectionCount] = 8;
        values[(int)VmBudgetDimension.DeclaredCount] = 131_072;
        values[(int)VmBudgetDimension.StructuralDepth] = 4;
        values[(int)VmBudgetDimension.LiveRuntimes] = 16;

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }

    /// <summary>
    /// The hard maxima a host may tighten and may never loosen.
    /// </summary>
    /// <remarks>
    /// Generous on the three nested-load dimensions for the same reason the calculator is generous
    /// on the ones it does not use: a runtime ceiling is clamped to the tightest hard maximum in the
    /// catalog, so declaring zero for a dimension this profile merely does not reach would forbid it
    /// to every profile composed alongside it. This profile declares no guest loads and charges none
    /// - its matrix says so, and that is what makes them unreachable here - so what it declares as a
    /// maximum is about its neighbours rather than about itself.
    /// </remarks>
    private static VmLimitVector Maxima()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        values[(int)VmBudgetDimension.Fuel] = 50_000_000;
        values[(int)VmBudgetDimension.WallClock] = 60_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 16L * 1024 * 1024;
        values[(int)VmBudgetDimension.HostCalls] = 1_000_000;
        values[(int)VmBudgetDimension.NestedLoadFanOut] = 64;
        values[(int)VmBudgetDimension.NestedLoadBytes] = 1024 * 1024;
        values[(int)VmBudgetDimension.VerifierWork] = 50_000_000;
        values[(int)VmBudgetDimension.LiveBytes] = 16L * 1024 * 1024;
        values[(int)VmBudgetDimension.CallDepth] = 16;
        values[(int)VmBudgetDimension.NestedLoadDepth] = 16;
        values[(int)VmBudgetDimension.ArtifactBytes] = 8L * 1024 * 1024;
        values[(int)VmBudgetDimension.SectionCount] = 64;
        values[(int)VmBudgetDimension.DeclaredCount] = 1_048_576;
        values[(int)VmBudgetDimension.StructuralDepth] = 16;
        values[(int)VmBudgetDimension.LiveRuntimes] = 64;

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }

    /// <summary>
    /// Which of the fifteen dimensions this profile charges. Host calls are charged because it
    /// imports a capability, which the catalog checks rather than takes on trust; the three
    /// nested-load rows are not, because the descriptor declares no guest-initiated loads.
    /// </summary>
    private static VmBudgetDeclarationMatrix Matrix()
    {
        var rows = new VmBudgetApplicability[VmBudgetDimensions.Count];

        for (var index = 0; index < rows.Length; index++)
        {
            rows[index] = VmBudgetApplicability.Charged;
        }

        rows[(int)VmBudgetDimension.NestedLoadDepth] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadFanOut] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadBytes] = VmBudgetApplicability.NotApplicable;

        VmBudgetDeclarationMatrix.TryCreate(rows, out var matrix);
        return matrix;
    }
}
