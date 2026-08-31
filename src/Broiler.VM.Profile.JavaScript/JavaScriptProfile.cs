// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           3
// Human-reviewed:   0/9
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  3/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;
using Broiler.VM.Profile.JavaScript.Format;
using System.Collections.Immutable;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The Broiler.VM JavaScript language profile, exposed the way the contract requires: one static
/// accessor on the profile's own type, naming its own descriptor.
/// </summary>
/// <remarks>
/// <para>
/// There is deliberately <b>no aggregate type listing several profiles</b>. One would reference
/// every profile assembly and defeat the exact-closure reports a composition depends on, which is
/// why the core forbids such a type by name and why this component does not invent one either.
/// </para>
/// <para>
/// <b>What this profile supports at this milestone is one feature manifest and nothing else.</b>
/// A profile name is not a conformance claim and neither is a manifest name; the accepted manifest
/// is <c>broiler.javascript.slice</c>, which admits numbers, arithmetic, comparison, local
/// variables and structured control flow, and admits no object, no string, no function and no
/// property access. An artifact naming any other manifest is refused at verification.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=EA198F
// Broiler-Falsified-If: a second static accessor or an aggregate profile-listing type appears in this graph, or the descriptor accepts a manifest this build does not implement
// Broiler-Human:        PENDING
public static class JavaScriptProfile
{
    /// <summary>This profile's identity.</summary>
    /// <remarks>
    /// The first label <c>broiler</c> is reserved and pairs with a <c>Broiler.*</c> package
    /// identity, which decision JSD-0001 records this profile taking on.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=051CB2
    // Broiler-Human:        PENDING
    public static VmProfileId Id { get; } = VmProfileId.Parse("broiler.javascript");

    /// <summary>The one feature manifest this build accepts.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=2B766D
    // Broiler-Human:        PENDING
    public static VmFeatureManifestId SliceManifest { get; } =
        VmFeatureManifestId.Parse("broiler.javascript.slice");

    /// <summary>The kind ID stamped on a completion value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C9902C
    // Broiler-Human:        PENDING
    public const int CompletionKindId = 1001;

    /// <summary>The kind ID stamped on a language fault.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=68B648
    // Broiler-Human:        PENDING
    public const int FaultKindId = 1002;

    /// <summary>The descriptor a composition root names directly.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=FBD334
    // Broiler-Human:        PENDING
    public static VmProfileDescriptor Descriptor { get; } = Build();

    /// <summary>Projects a completion value out of an invocation result.</summary>
    /// <remarks>
    /// The profile-owned projection the contract specifies: the core hands back an opaque payload
    /// whose identity it has already checked, and this accessor is what turns it into a type the
    /// caller can read. A core-side generic projection would need the core to name a profile type.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=50E921
    // Broiler-Human:        PENDING
    public static bool TryGetCompletion(in VmInvocationResult result, out JavaScriptCompletion completion) =>
        result.TryGetPayload(out completion);

    /// <summary>Projects a language fault out of an invocation result.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=87273C
    // Broiler-Human:        PENDING
    public static bool TryGetFault(in VmInvocationResult result, out JavaScriptFault fault) =>
        result.TryGetPayload(out fault);

    /// <summary>
    /// The one full-arity construction, with every row filled and the language-shaped ones marked.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Five rows below are provisional and each names the milestone that settles it.</b> The
    /// contract asks for a descriptor and not for a finished language, so a row whose honest value
    /// depends on a decision this milestone has not taken is filled with a value that is safe,
    /// marked here, and carried in the ledger - rather than filled with a number that looks
    /// settled. The five are the call-depth default and maximum, the uncharged-work bound, the
    /// charging granularity and the cancellation poll bound; JS-5 replaces all five with numbers
    /// derived from a retained measurement, because roadmap section 8 says each is measured rather
    /// than chosen.
    /// </para>
    /// <para>
    /// <b>Four matrix rows say <c>NotApplicable</c> and JSD-0004 intended them charged.</b> That
    /// decision described the profile this component is growing into; this descriptor describes
    /// what it is now. The slice imports no host capability and declares no guest-initiated load,
    /// so host calls and the three nested-load dimensions are structurally unreachable, and
    /// declaring them charged would be a claim the rest of this descriptor contradicts. JS-6 flips
    /// the host-call row when the standard library imports something and JS-8 flips the three
    /// nested rows when guest loads are declared. The correction is dated in decision JSD-0008
    /// rather than left as a drift between a record and a construction.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=BA5E70
    // Broiler-Falsified-If: a row here disagrees with decision JSD-0004 or JSD-0008 without a dated record of the correction
    // Broiler-Human:        PENDING
    private static VmProfileDescriptor Build()
    {
        VmDiagnosticsIdentity.TryCreate(Id, "broiler.javascript.diagnostics", out var diagnostics);

        return new VmProfileDescriptor(
            profileId: Id,
            displayName: "Broiler JavaScript",
            descriptorRevision: 1,
            supportedFormatVersions: new VmFormatVersionRange(
                JavaScriptFormat.MinimumFormatVersion, JavaScriptFormat.MaximumFormatVersion),
            acceptedFeatureManifests: ImmutableArray.Create(SliceManifest),
            verifier: new JavaScriptVerifier(Id, SliceManifest),
            executorFactory: environment => new JavaScriptExecutor(Id, environment),
            artifactRepresentationKind: VmArtifactRepresentationKind.Decoded,
            artifactLifetimeKind: VmArtifactLifetimeKind.Managed,
            supportsConcurrentVerification: true,
            threadAffinity: VmThreadAffinity.Agile,

            // Provisional; JS-5 measures it. One poll per instruction is far inside this, which is
            // what makes the number safe to carry rather than right.
            cancellationPollBound: 256,
            abandonBudget: 0,
            limitDefaults: Defaults(),
            profileHardMaxima: Maxima(),
            budgetDeclarationMatrix: Matrix(),

            // No host capability at all. The slice has no standard library and nothing to import,
            // and a profile that imports nothing is the case that makes "registering a capability
            // never implies a provider" easy to see.
            hostCapabilityDescriptors: ImmutableArray<VmCapabilityImport>.Empty,

            // No `eval`, no Function constructor, no dynamic import. The `broiler.javascript.dynamic`
            // manifest is a separate identity precisely so a composition can decline exactly this
            // and say so, and JS-8 is where it is declared.
            guestInitiatedLoads: VmGuestLoadDeclaration.NotDeclared,

            // No top-level await, because there are no modules. JS-7 declares it.
            asynchronousInstantiation: VmDeclaration.NotDeclared,

            // Not declared, so a composition enabling it gets the named refusal rather than a pause
            // this profile cannot honour. JS-7 decides it.
            externalSuspension: VmDeclaration.NotDeclared,
            payloadKindIdRange: new VmPayloadKindIdRange(1000, 1099),
            authoredCoreContractVersion: 1,
            conformanceManifestId: VmConformanceManifestId.Create("broiler.javascript.conformance"),
            conformanceManifestVersion: 1,
            diagnosticsIdentity: diagnostics,
            packageIdentity: new VmPackageIdentity(
                "Broiler.VM.Profile.JavaScript", "0.1.0-preview.1", "broiler.javascript"),
            faultRecovery: VmFaultRecovery.InstanceRecoverable,

            // Provisional; JS-5 measures both.
            maxUnchargedWork: 256,
            chargingGranularity: 1,
            artifactSharing: VmArtifactSharing.Shareable);
    }

    /// <summary>
    /// The bounded defaults a host adopts when it does not state its own.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the vector that reaches other profiles.</b> A host adopting profile defaults
    /// rather than stating numbers gets the tightest default in the catalog, per dimension,
    /// because at runtime creation no profile has been selected and there is no other safe answer.
    /// So every number here is chosen as what this profile actually needs - and on a dimension it
    /// does not use, as something generous rather than as a zero, because a zero here is a claim
    /// about every neighbour that adopts defaults.
    /// </para>
    /// <para>
    /// The four dimensions the matrix marks inapplicable still carry generous numbers for exactly
    /// that reason. They cost this profile nothing, and a <c>0</c> in a guest-load default would
    /// hand a host that adopts defaults a ceiling of zero, with the failure surfacing in somebody
    /// else's verifier as a refusal naming a dimension they never touched.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=8CA639
    // Broiler-Falsified-If: a default here is zero on a dimension this profile declares inapplicable, or any default exceeds its maximum
    // Broiler-Human:        PENDING
    private static VmLimitVector Defaults()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        values[(int)VmBudgetDimension.Fuel] = 50_000_000;
        values[(int)VmBudgetDimension.WallClock] = 10_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 64L * 1024 * 1024;
        values[(int)VmBudgetDimension.LiveBytes] = 32L * 1024 * 1024;
        values[(int)VmBudgetDimension.HostCalls] = 1_000_000;
        values[(int)VmBudgetDimension.CallDepth] = 1_024;
        values[(int)VmBudgetDimension.VerifierWork] = 100_000_000;
        values[(int)VmBudgetDimension.ArtifactBytes] = 32L * 1024 * 1024;
        values[(int)VmBudgetDimension.SectionCount] = 64;
        values[(int)VmBudgetDimension.DeclaredCount] = 4_194_304;
        values[(int)VmBudgetDimension.StructuralDepth] = 256;
        values[(int)VmBudgetDimension.NestedLoadDepth] = 4;
        values[(int)VmBudgetDimension.NestedLoadFanOut] = 4_096;
        values[(int)VmBudgetDimension.NestedLoadBytes] = 16L * 1024 * 1024;
        values[(int)VmBudgetDimension.LiveRuntimes] = 64;

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }

    /// <summary>
    /// The hard maxima a host may tighten and may never loosen.
    /// </summary>
    /// <remarks>
    /// <b>A maximum binds this profile's own artifacts and nobody else's.</b> It is applied at
    /// verification, against the profile the artifact names, so a tight one constrains only what
    /// this profile accepts and reaches no profile composed beside it. It is not a statement of
    /// what this profile uses - the defaults above are that - but of the most it would tolerate a
    /// host granting.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=986107
    // Broiler-Human:        PENDING
    private static VmLimitVector Maxima()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        values[(int)VmBudgetDimension.Fuel] = 1_099_511_627_776;
        values[(int)VmBudgetDimension.WallClock] = 3_600_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 4_294_967_296;
        values[(int)VmBudgetDimension.LiveBytes] = 2_147_483_648;
        values[(int)VmBudgetDimension.HostCalls] = 4_294_967_295;
        values[(int)VmBudgetDimension.CallDepth] = 16_384;
        values[(int)VmBudgetDimension.VerifierWork] = 1_099_511_627_776;
        values[(int)VmBudgetDimension.ArtifactBytes] = 536_870_912;
        values[(int)VmBudgetDimension.SectionCount] = 1_024;
        values[(int)VmBudgetDimension.DeclaredCount] = 4_294_967_295;
        values[(int)VmBudgetDimension.StructuralDepth] = 4_096;
        values[(int)VmBudgetDimension.NestedLoadDepth] = 64;
        values[(int)VmBudgetDimension.NestedLoadFanOut] = 16_777_216;
        values[(int)VmBudgetDimension.NestedLoadBytes] = 536_870_912;
        values[(int)VmBudgetDimension.LiveRuntimes] = 4_096;

        VmLimitVector.TryCreate(values, out var vector);
        return vector;
    }

    /// <summary>
    /// Which of the fifteen dimensions this profile charges.
    /// </summary>
    /// <remarks>
    /// There is no default row: a dimension this profile does not charge says so, and the catalog
    /// checks the declaration against the structural consequences of the rest of the descriptor.
    /// Four rows are inapplicable here because the slice imports no capability and declares no
    /// guest load, which makes those dimensions unreachable rather than merely unused - and
    /// declaring an unreachable dimension charged would be a claim this descriptor contradicts
    /// two rows further down.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6308B1
    // Broiler-Falsified-If: a row says charged for a dimension no code path charges, or inapplicable for one that is reachable
    // Broiler-Human:        PENDING
    private static VmBudgetDeclarationMatrix Matrix()
    {
        var rows = new VmBudgetApplicability[VmBudgetDimensions.Count];

        for (var index = 0; index < rows.Length; index++)
        {
            rows[index] = VmBudgetApplicability.Charged;
        }

        rows[(int)VmBudgetDimension.HostCalls] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadDepth] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadFanOut] = VmBudgetApplicability.NotApplicable;
        rows[(int)VmBudgetDimension.NestedLoadBytes] = VmBudgetApplicability.NotApplicable;

        VmBudgetDeclarationMatrix.TryCreate(rows, out var matrix);
        return matrix;
    }
}
