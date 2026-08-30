using Broiler.VM;
using System.Collections.Immutable;

namespace Com.Example.Calculator;

/// <summary>
/// The application-local calculator profile, exposed the way the contract requires: one static
/// accessor on the profile's own type, naming its own descriptor.
/// </summary>
/// <remarks>
/// <para>
/// There is deliberately no aggregate type listing several profiles. One would reference every
/// profile assembly and defeat the exact-closure reports a composition depends on, which is why the
/// core forbids such a type by name and why a consumer must not invent one either.
/// </para>
/// <para>
/// The identity is <c>com.example.calculator</c>: a reverse-domain ID under the
/// documentation-reserved domain, outside the reserved <c>broiler</c> first label. An application
/// shipping this for real would use a domain it controls; the point the ID makes here is that
/// nothing in the core knows or cares which.
/// </para>
/// </remarks>
public static class CalculatorProfile
{
    /// <summary>This profile's identity.</summary>
    public static VmProfileId Id { get; } = VmProfileId.Parse("com.example.calculator");

    /// <summary>Its one accepted feature manifest.</summary>
    public static VmFeatureManifestId Manifest { get; } =
        VmFeatureManifestId.Parse("com.example.calculator.base");

    /// <summary>The kind ID this profile stamps on an answer.</summary>
    public const int AnswerKindId = 501;

    /// <summary>The kind ID it stamps on a language fault.</summary>
    public const int FaultKindId = 502;

    /// <summary>The descriptor a composition root names directly.</summary>
    public static VmProfileDescriptor Descriptor { get; } = Build();

    /// <summary>
    /// Projects a calculator answer out of an invocation result, or returns false when the result
    /// carries no payload of this profile's.
    /// </summary>
    /// <remarks>
    /// This is the profile-owned projection the contract specifies: the core hands back an opaque
    /// payload it has already checked the identity of, and the profile's own accessor is what turns
    /// it into a type the caller can read. A core-side generic projection would need the core to
    /// name a profile type.
    /// </remarks>
    public static bool TryGetAnswer(in VmInvocationResult result, out CalculatorAnswer answer) =>
        result.TryGetPayload(out answer);

    /// <summary>Projects a calculator fault out of an invocation result.</summary>
    public static bool TryGetFault(in VmInvocationResult result, out CalculatorFault fault) =>
        result.TryGetPayload(out fault);

    private static VmProfileDescriptor Build()
    {
        VmDiagnosticsIdentity.TryCreate(Id, "com.example.calculator.diagnostics", out var diagnostics);

        return new VmProfileDescriptor(
            profileId: Id,
            displayName: "Example Calculator",
            descriptorRevision: 1,
            supportedFormatVersions: new VmFormatVersionRange(1, 1),
            acceptedFeatureManifests: ImmutableArray.Create(Manifest),
            verifier: new CalculatorVerifier(Id),
            executorFactory: environment => new CalculatorExecutor(Id, environment),
            artifactRepresentationKind: VmArtifactRepresentationKind.Decoded,
            artifactLifetimeKind: VmArtifactLifetimeKind.Managed,
            supportsConcurrentVerification: true,
            threadAffinity: VmThreadAffinity.Agile,
            cancellationPollBound: 256,
            abandonBudget: 0,
            limitDefaults: Defaults(),
            profileHardMaxima: Maxima(),
            budgetDeclarationMatrix: Matrix(),

            // No host capability at all. A profile that imports nothing is a first-class case, and
            // it is the one that makes "registering value capabilities never implies a provider"
            // easy to see: this composition can register whatever it likes and this profile still
            // reaches none of it.
            hostCapabilityDescriptors: ImmutableArray<VmCapabilityImport>.Empty,
            guestInitiatedLoads: VmGuestLoadDeclaration.NotDeclared,
            asynchronousInstantiation: VmDeclaration.NotDeclared,
            externalSuspension: VmDeclaration.NotDeclared,
            payloadKindIdRange: new VmPayloadKindIdRange(500, 599),
            authoredCoreContractVersion: 1,
            conformanceManifestId: VmConformanceManifestId.Create("com.example.calculator.conformance"),
            conformanceManifestVersion: 1,
            diagnosticsIdentity: diagnostics,

            // Not a Broiler package, and the ID says so. The reserved-namespace check compares a
            // reserved ID against a Broiler package prefix; this ID claims nothing reserved, so the
            // check has nothing to hold it to and the honest package identity is this one.
            packageIdentity: new VmPackageIdentity("Com.Example.Calculator", "1.0.0", "example-application"),
            faultRecovery: VmFaultRecovery.InstanceRecoverable,
            maxUnchargedWork: 256,
            chargingGranularity: 1,
            artifactSharing: VmArtifactSharing.Shareable);
    }

    /// <summary>
    /// The bounded defaults a host adopts when it does not state its own. No member is unbounded.
    /// </summary>
    /// <remarks>
    /// Deliberately tighter than the fixture profile's in several dimensions. A calculator needs no
    /// host calls, no nested loads and very little memory, and a profile that declared the fixture's
    /// numbers because they were to hand would be asking a host for headroom it will never use.
    /// </remarks>
    private static VmLimitVector Defaults()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        values[(int)VmBudgetDimension.Fuel] = 100_000;
        values[(int)VmBudgetDimension.WallClock] = 5_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 256 * 1024;
        values[(int)VmBudgetDimension.HostCalls] = 0;
        values[(int)VmBudgetDimension.NestedLoadFanOut] = 0;
        values[(int)VmBudgetDimension.NestedLoadBytes] = 0;
        values[(int)VmBudgetDimension.VerifierWork] = 100_000;
        values[(int)VmBudgetDimension.LiveBytes] = 256 * 1024;
        values[(int)VmBudgetDimension.CallDepth] = 1;
        values[(int)VmBudgetDimension.NestedLoadDepth] = 0;
        values[(int)VmBudgetDimension.ArtifactBytes] = 64 * 1024;
        values[(int)VmBudgetDimension.SectionCount] = 1;
        values[(int)VmBudgetDimension.DeclaredCount] = 4_096;
        values[(int)VmBudgetDimension.StructuralDepth] = 1;
        values[(int)VmBudgetDimension.LiveRuntimes] = 16;

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }

    /// <summary>
    /// The hard maxima a host may tighten and may never loosen.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A profile hard maximum is not a statement of what this profile uses - the defaults above are
    /// that. It is the most this profile would tolerate a host granting it, and the two are not the
    /// same number.
    /// </para>
    /// <para>
    /// The distinction is load-bearing in a composition, not merely tidy. A runtime ceiling is
    /// clamped to the <em>tightest</em> hard maximum in the catalog, across every profile in it, so
    /// a profile that declared its own usage as its maximum caps every profile composed beside it.
    /// This profile never enters a section, never nests and never calls a host, so on those
    /// dimensions it has no opinion and says so with a generous number rather than with zero. A zero
    /// here would mean "no profile sharing a runtime with me may frame an artifact", which is not a
    /// claim a calculator is entitled to make. The dimensions it does use - fuel, bytes, work,
    /// depth, artifact size - are capped at what it will actually tolerate.
    /// </para>
    /// </remarks>
    private static VmLimitVector Maxima()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        values[(int)VmBudgetDimension.Fuel] = 10_000_000;
        values[(int)VmBudgetDimension.WallClock] = 60_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 16L * 1024 * 1024;
        values[(int)VmBudgetDimension.HostCalls] = 1_000_000;
        values[(int)VmBudgetDimension.NestedLoadFanOut] = 64;
        values[(int)VmBudgetDimension.NestedLoadBytes] = 1024 * 1024;
        values[(int)VmBudgetDimension.VerifierWork] = 10_000_000;
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
    /// Which of the fifteen dimensions this profile charges. There is no default row: a dimension
    /// this profile does not charge says so, and the catalog checks the declaration against the
    /// structural consequences of the rest of the descriptor.
    /// </summary>
    private static VmBudgetDeclarationMatrix Matrix()
    {
        var rows = new VmBudgetApplicability[VmBudgetDimensions.Count];

        for (var index = 0; index < rows.Length; index++)
        {
            rows[index] = VmBudgetApplicability.Charged;
        }

        // No imports, so no host call is reachable; no guest-load declaration, so the four nested
        // rows must say so too. Declaring them charged while the descriptor makes them unreachable
        // would be a claim the rest of the descriptor contradicts.
        rows[(int)VmBudgetDimension.HostCalls] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadDepth] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadFanOut] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadBytes] = VmBudgetApplicability.NotApplicable;

        VmBudgetDeclarationMatrix.TryCreate(rows, out var matrix);
        return matrix;
    }
}
