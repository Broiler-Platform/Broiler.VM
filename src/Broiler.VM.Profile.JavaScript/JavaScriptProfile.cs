// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           5
// Human-reviewed:   0/14
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  3/10 max
// Unverified:       14
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

    /// <summary>
    /// The wider feature manifest: objects, functions, closures, exceptions and a standard library.
    /// </summary>
    /// <remarks>
    /// <b>It is a second manifest and not a wider first one.</b> The slice's artifacts, its
    /// retained corpus and its conformance fixtures all name <c>broiler.javascript.slice</c> and
    /// mean by it exactly what they meant on the day they were written; widening that identity in
    /// place would silently change what every one of them claims. This name is what format version
    /// 2 is defined against, and an artifact naming one manifest at the other's format version is
    /// refused.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AB89AD
    // Broiler-Human:        PENDING
    public static VmFeatureManifestId WideManifest { get; } =
        VmFeatureManifestId.Parse("broiler.javascript.wide");

    /// <summary>The module goal's feature manifest.</summary>
    /// <remarks>
    /// <para>
    /// <b>A third identity rather than a wider second one, and it is the one a composition can
    /// decline.</b> A module carries a question a script does not - what a specifier names - and
    /// the answer is the host's rather than this profile's. So the surface is a separate identity,
    /// exactly as roadmap section 6 makes <c>broiler.javascript.dynamic</c> one, and declining it
    /// is a thing a composition does by registering nothing rather than by passing a flag.
    /// </para>
    /// <para>
    /// <b>Declining is registering no resolver, and there is no second switch.</b> Two switches
    /// that must agree is a defect waiting to be written: a composition that declared the surface
    /// and registered no resolver, or the reverse, would be in a state nobody could act on. The
    /// registration is the admission, and <see cref="ResolveCapability"/> is what is registered.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: a module artifact verifies in a composition that registered no module resolver
    // Broiler-Human:        PENDING
    public static VmFeatureManifestId ModulesManifest { get; } =
        VmFeatureManifestId.Parse(Format.JsFormat.ModulesManifestId);

    /// <summary>
    /// The host capability a composition registers to say that module resolution is its own.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The profile reads no file and follows no specifier, and this is the seam that keeps it
    /// that way.</b> A module artifact arrives with its graph already whole: every module the root
    /// can reach is in the artifact under the key the composition resolved it to, and a request is
    /// matched against those keys by exact comparison. Turning <c>"./b.mjs"</c> into a key is the
    /// composition's act, performed before the artifact existed, in whatever way that composition's
    /// deployment calls for - a file path, a URL, a name in a bundle, a table.
    /// </para>
    /// <para>
    /// <b>So what is registered is a RULING, not a door the guest reaches through.</b> Linking hands
    /// the host one request - the referring module's key, the specifier as the source wrote it, and
    /// the key the artifact says it resolves to, separated by NULs - and the host answers
    /// <c>Completed</c> when that is how it resolves the specifier and <c>Refused</c> when it is
    /// not. The profile therefore never derives a key: it can only be told that one it was handed
    /// is right, so a graph resolved by somebody else's rules is refused by the composition that
    /// would have to run it.
    /// </para>
    /// <para>
    /// <b>Why a confirmation rather than an answer.</b> A capability answering with bytes would have
    /// to hand back a reference this profile could dereference, and the contract's opaque reference
    /// is deliberately not dereferenceable - which is the right decision and not an obstacle to work
    /// around. A ruling needs no such channel and gives the composition the same authority: nothing
    /// this profile does with a request survives the host saying no.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=TBF
    // Broiler-Falsified-If: this profile opens a file, follows a specifier, or resolves a module request without asking the host
    // Broiler-Human:        PENDING
    public static VmHostCapabilityDescriptor ResolveCapability { get; } =
        new(
            VmCapabilityId.Parse("broiler.javascript.resolve"),
            version: 1,
            VmCapabilitySignatureId.FromCanonicalDescription("(bytes)->unit"),
            VmCapabilityKind.Value,
            VmCapabilityReentrancy.NonReentrant,
            VmCapabilityThreadAffinity.CallerThread,
            VmExceptionTranslation.TerminateOperation);

    /// <summary>The kind ID stamped on a completion value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C9902C
    // Broiler-Human:        PENDING
    public const int CompletionKindId = 1001;

    /// <summary>The kind ID stamped on a language fault.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=68B648
    // Broiler-Human:        PENDING
    public const int FaultKindId = 1002;

    /// <summary>The kind ID stamped on a wide-surface completion value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F39355
    // Broiler-Human:        PENDING
    public const int WideCompletionKindId = 1003;

    /// <summary>The kind ID stamped on a wide-surface uncaught exception.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4D1110
    // Broiler-Human:        PENDING
    public const int WideFaultKindId = 1004;

    /// <summary>The binding index the wide surface's <c>print</c> reaches the host through.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A9FBB7
    // Broiler-Human:        PENDING
    public const int WriteBindingIndex = 0;

    /// <summary>The binding index a module request is put to the composition through.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    public const int ResolveBindingIndex = 1;

    /// <summary>
    /// The one host capability this profile imports: write one run of UTF-8 text.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is OPTIONAL, and that is the whole design.</b> A composition that registers nothing
    /// still creates a runtime and still runs programs; what it does not have is a <c>print</c>
    /// that reaches anywhere. The profile asks <c>IsBound</c> and answers <c>undefined</c> either
    /// way, so the difference between a host that shows output and one that does not is a
    /// registration and not a code path in the guest.
    /// </para>
    /// <para>
    /// <b>Why a capability rather than a property somebody sets.</b> A static sink on this type
    /// would be process-wide, would outlive a runtime, and would let one composition's output reach
    /// another's - which is exactly the ambient platform surface the capability table exists to
    /// prevent. Registration is the permission, and there is no other door.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=77AB79
    // Broiler-Human:        PENDING
    public static VmHostCapabilityDescriptor WriteCapability { get; } =
        new(
            VmCapabilityId.Parse("broiler.javascript.write"),
            version: 1,
            VmCapabilitySignatureId.FromCanonicalDescription("(bytes)->unit"),
            VmCapabilityKind.Value,
            VmCapabilityReentrancy.NonReentrant,
            VmCapabilityThreadAffinity.CallerThread,
            VmExceptionTranslation.TerminateOperation);

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

    /// <summary>Projects a wide-surface completion value out of an invocation result.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AC0F96
    // Broiler-Human:        PENDING
    public static bool TryGetWideCompletion(in VmInvocationResult result, out JsCompletion completion) =>
        result.TryGetPayload(out completion);

    /// <summary>Projects a wide-surface uncaught exception out of an invocation result.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=56397D
    // Broiler-Human:        PENDING
    public static bool TryGetUncaught(in VmInvocationResult result, out JsUncaught uncaught) =>
        result.TryGetPayload(out uncaught);

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=11B609
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
                JavaScriptFormat.MinimumFormatVersion, Format.JsFormat.FormatVersion),
            acceptedFeatureManifests: ImmutableArray.Create(
                SliceManifest, WideManifest, ModulesManifest),
            verifier: new JavaScriptVerifier(Id, SliceManifest),
            executorFactory: environment => new JavaScriptExecutor(Id, environment),
            artifactRepresentationKind: VmArtifactRepresentationKind.Decoded,
            artifactLifetimeKind: VmArtifactLifetimeKind.Managed,
            supportsConcurrentVerification: true,
            threadAffinity: VmThreadAffinity.Agile,

            // MEASURED BY CONSTRUCTION rather than chosen, and it is the largest single work
            // charge this profile makes: the bounded reader charges one unit per byte consumed and
            // polls on every charge, so a charge larger than this bound is reported as a poll-bound
            // violation - which is exactly what a 4 KB code-section read produced while this said
            // 256. The verifier reads every bulk run in windows no larger than this, so the bound
            // and the behaviour are two statements of one fact.
            cancellationPollBound: 65_536,
            abandonBudget: 0,
            limitDefaults: Defaults(),
            profileHardMaxima: Maxima(),
            budgetDeclarationMatrix: Matrix(),

            // ONE OPTIONAL IMPORT, and it arrived with the wide manifest's standard library.
            // The slice imports nothing and still does; what changed is that a surface with a
            // `print` exists, and a `print` that reached the console without the composition
            // registering anything would be the ambient surface the capability table forbids.
            // TWO OPTIONAL IMPORTS, and the second is what a composition declines the module
            // surface by leaving out. It is Optional for the same reason the first is: a
            // composition that registers neither still creates a runtime and still runs scripts.
            // What it does not get is a module artifact past verification.
            hostCapabilityDescriptors: ImmutableArray.Create(
                new VmCapabilityImport(WriteCapability, VmCapabilityImportKind.Optional),
                new VmCapabilityImport(ResolveCapability, VmCapabilityImportKind.Optional)),

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

            // The same figure and for the same reason; see the poll bound above.
            maxUnchargedWork: 65_536,
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
    /// Three rows are inapplicable here because this profile declares no guest load, which makes
    /// those dimensions unreachable rather than merely unused. The host-call row was inapplicable
    /// until the wide manifest imported a capability and is charged now, which is the flip JSD-0008
    /// said JS-6 would make.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=2B6844
    // Broiler-Falsified-If: a row says charged for a dimension no code path charges, or inapplicable for one that is reachable
    // Broiler-Human:        PENDING
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
