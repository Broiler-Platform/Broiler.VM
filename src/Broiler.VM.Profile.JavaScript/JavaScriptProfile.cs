// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           8
// Human-reviewed:   0/17
// IP risk:          Low
// Security risk:    High
// Criteria:         6/6
// Resource impact:  3/10 max
// Unverified:       17
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
    /// <summary>The entry-point name a host invokes to run the jobs a program left owed.</summary>
    /// <remarks>
    /// <b>The profile never chooses when to drain and a host always does</b>, so the name is part of
    /// the profile's surface rather than a convention each host restates. A host that invokes it
    /// runs every due job on the guest stack; one that never does runs none, and a program whose
    /// only remaining work was a promise reaction ends with that work undone - which is a decision
    /// an embedder makes, not one this profile makes for it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=AE7B50
    // Broiler-Human:        PENDING
    public const string DrainEntryPoint = "#drain-jobs";

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

    /// <summary>
    /// The binary surface: <c>ArrayBuffer</c>, <c>DataView</c> and the typed array constructors.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is an optional surface rather than a third manifest an artifact could name.</b> An
    /// artifact still names <see cref="WideManifest"/> in its header; what the binary identity does
    /// is let that artifact <b>declare</b> that it also reaches this surface, and let a composition
    /// decline exactly that. The identity exists for the reason
    /// <see cref="DynamicManifest"/>'s does — so a composition can answer one question separately —
    /// and the question here is whether a guest whose whole argument is a verified artifact under a
    /// metered budget may hold shared mutable memory addressed by index.
    /// </para>
    /// <para>
    /// <b><c>SharedArrayBuffer</c> and <c>Atomics</c> are deliberately not in it.</b> They are the
    /// multi-agent surface and they need the agent model; folding them in would let a composition
    /// that wanted an ordinary byte buffer admit cross-agent shared memory by accident.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=50C297
    // Broiler-Human:        PENDING
    public static VmFeatureManifestId BinaryManifest { get; } =
        VmFeatureManifestId.Parse(Format.JsSurfaces.Binary);

    /// <summary>
    /// The dynamic surface: <c>eval</c> and the <c>Function</c> constructor.
    /// </summary>
    /// <remarks>
    /// Separate because a composition that registers no artifact provider must be able to decline
    /// exactly this and say so, which is roadmap section 6's reason and not a new one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CDF67F
    // Broiler-Human:        PENDING
    public static VmFeatureManifestId DynamicManifest { get; } =
        VmFeatureManifestId.Parse(Format.JsSurfaces.Dynamic);

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

    /// <summary>
    /// The optional artifact-provider capability this profile imports: source in, artifact out.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is the only route by which a String becomes code, and it is optional.</b> A
    /// composition that registers nothing still creates a runtime and still runs programs; what it
    /// does not have is an <c>eval</c> that can answer. That refusal is a run-time error the guest
    /// may catch, and it is deliberately NOT the same event as a composition declining
    /// <c>broiler.javascript.binary</c>'s sibling identity <c>broiler.javascript.dynamic</c>, which
    /// refuses the artifact at verification before the guest exists. Roadmap section 6 names both
    /// and says they must stay distinguishable.
    /// </para>
    /// <para>
    /// <b>The request payload is the source text, UTF-8, and nothing else.</b> The core carries it
    /// without decoding it, because what a specifier means is a language concept; a provider that
    /// wants to know whether it was a direct <c>eval</c> cannot be told, because the answer would
    /// not change what it may compile.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=9A3E06
    // Broiler-Falsified-If: this profile obtains executable bytes by any route but a provider registered under this identity
    // Broiler-Human:        PENDING
    public static VmHostCapabilityDescriptor SourceProviderCapability { get; } =
        new(
            VmCapabilityId.Parse("broiler.javascript.source-provider"),
            version: 1,
            VmCapabilitySignatureId.FromCanonicalDescription("(source-utf8)->artifact"),
            VmCapabilityKind.ArtifactProvider,
            VmCapabilityReentrancy.NonReentrant,
            VmCapabilityThreadAffinity.CallerThread,
            VmExceptionTranslation.TerminateOperation);

    /// <summary>The descriptor a composition root names directly, admitting every surface.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4F7006
    // Broiler-Human:        PENDING
    public static VmProfileDescriptor Descriptor { get; } = Build(EverySurface);

    /// <summary>
    /// A descriptor admitting only the optional surfaces named, so a composition can decline one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Declining is a composition's act and this is the only door to it.</b> There is no
    /// property anybody sets and no ambient switch: a composition states which surfaces it admits
    /// when it builds the descriptor it registers, and every artifact declaring one it did not name
    /// is refused at verification with an invalid-artifact reason. That is the outcome roadmap
    /// section 6 distinguishes by name from the run-time refusal a composition that admits a
    /// surface and registers no provider produces.
    /// </para>
    /// <para>
    /// <b>It is a method rather than a second static property</b>, because the set of interesting
    /// combinations is the power set of the surfaces and a property per member is a list that goes
    /// stale the day a surface is added.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=420B8F
    // Broiler-Falsified-If: a descriptor built here accepts an optional surface its caller did not name
    // Broiler-Human:        PENDING
    public static VmProfileDescriptor DescriptorAdmitting(params VmFeatureManifestId[] surfaces)
    {
        var names = ImmutableArray.CreateBuilder<string>();

        foreach (var surface in surfaces)
        {
            names.Add(surface.ToString());
        }

        return Build(names.ToImmutable());
    }

    /// <summary>Every optional surface this build implements.</summary>
    /// <remarks>
    /// <b>Computed on each read rather than initialised once</b>, because a static field is
    /// initialised in declaration order and <see cref="Descriptor"/> — which is declared above it
    /// and reads it — would otherwise be handed a default array. That is a defect a build cannot
    /// see and a type initialiser reports as a null reference from somewhere else entirely.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6C43BD
    // Broiler-Human:        PENDING
    private static ImmutableArray<string> EverySurface => ImmutableArray.Create(Format.JsSurfaces.All);

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
    /// <b>Every matrix row says <c>Charged</c>, and it took two milestones to get there.</b>
    /// JSD-0004 intended them charged; the descriptor as first written marked four
    /// <c>NotApplicable</c>, because the slice imported no host capability and declared no
    /// guest-initiated load, so those dimensions were structurally unreachable and declaring them
    /// charged would have been a claim the rest of the descriptor contradicted. The host-call row
    /// flipped when the standard library imported <c>print</c>; the three nested-load rows flip
    /// here, with <c>eval</c>. The correction was dated in decision JSD-0008 rather than left as a
    /// drift between a record and a construction, and this paragraph is what closes it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=929887
    // Broiler-Falsified-If: a row here disagrees with decision JSD-0004 or JSD-0008 without a dated record of the correction
    // Broiler-Human:        PENDING
    private static VmProfileDescriptor Build(ImmutableArray<string> admittedSurfaces)
    {
        VmDiagnosticsIdentity.TryCreate(Id, "broiler.javascript.diagnostics", out var diagnostics);

        var accepted = ImmutableArray.Create(SliceManifest, WideManifest);

        foreach (var surface in admittedSurfaces)
        {
            accepted = accepted.Add(VmFeatureManifestId.Parse(surface));
        }

        return new VmProfileDescriptor(
            profileId: Id,
            displayName: "Broiler JavaScript",
            descriptorRevision: 1,
            supportedFormatVersions: new VmFormatVersionRange(
                JavaScriptFormat.MinimumFormatVersion, Format.JsFormat.FormatVersion),
            acceptedFeatureManifests: accepted,
            verifier: new JavaScriptVerifier(Id, SliceManifest, admittedSurfaces),
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
            hostCapabilityDescriptors: ImmutableArray.Create(
                new VmCapabilityImport(WriteCapability, VmCapabilityImportKind.Optional),
                new VmCapabilityImport(SourceProviderCapability, VmCapabilityImportKind.Optional)),

            // `eval` AND THE `Function` CONSTRUCTOR, THROUGH THE MEDIATOR AND NOWHERE ELSE. Both
            // turn a String into a request and run whatever verified handle the composition's
            // artifact provider answers with; neither compiles anything inside this profile, which
            // is what keeps a compiler in a composition's declared closure. Dynamic `import()` is
            // NOT declared here: it belongs to the module goal, which does not exist yet.
            //
            // THE BOUNDS ARE THIS PROFILE'S OWN HARD MAXIMA AND A COMPOSITION MAY ONLY TIGHTEN
            // THEM. A depth of four admits an `eval` that evaluates source that evaluates source,
            // twice more, and refuses the fourth - which is enough for every generated program this
            // profile is built to run and far short of a recursion. The fan-out and byte bounds are
            // generous because a code-loading benchmark evaluates thousands of small functions, and
            // the fuel rate is what charges nested verification to the operation that asked rather
            // than to a separate allowance nobody set.
            guestInitiatedLoads: VmGuestLoadDeclaration.Declared(
                minimumProviderCapabilityVersion: 1,
                profileHardMaxima: new VmGuestLoadBounds(
                    nestedLoadDepth: 4,
                    nestedLoadFanOut: 1_000_000,
                    nestedLoadBytes: 64L * 1024 * 1024,
                    verifierWork: 1_000_000_000),
                verifierWorkToFuelRate: 1),

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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=B1B19D
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
        // ABOVE THE ENGINE'S OWN BOUND, so the default answer to a runaway recursion is the
        // language's catchable `RangeError` and not an abort. A host that wants a program
        // refused at a hundred frames states that ceiling and gets it; a host that states
        // nothing gets what every engine gives, which is what a program's own recursion guard
        // is written against *(JSC-96)*.
        values[(int)VmBudgetDimension.CallDepth] = 6_144;
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D45C6A
    // Broiler-Human:        PENDING
    private static VmLimitVector Maxima()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        values[(int)VmBudgetDimension.Fuel] = 1_099_511_627_776;
        values[(int)VmBudgetDimension.WallClock] = 3_600_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 4_294_967_296;
        values[(int)VmBudgetDimension.LiveBytes] = 2_147_483_648;
        values[(int)VmBudgetDimension.HostCalls] = 4_294_967_295;
        // MEASURED, NOT CHOSEN, AND THE MEASUREMENT IS RECORDED WITH IT. `eng/measure-frame-cost.py`
        // bisects the published binary against a recursion with no base case and finds that this
        // interpreter survives 17,963 JavaScript calls on the sixty-four-megabyte stack
        // `JsExecution.GuestStackBytes` declares - one call costing 3,736 bytes of native stack,
        // and costing the same whether the JavaScript frame is narrow or wide, because the operand
        // stack and the environment are heap objects rather than stack ones. The maximum a host may
        // be granted is set at less than half of that, so the CALL-DEPTH CEILING always reaches its
        // limit before the native stack reaches its, and a recursing program is refused as a
        // resource exhaustion naming the dimension rather than by the process dying.
        //
        // This row read 16,384 until the measurement was taken, which is four times what the stack
        // then held *(corrected: JSC-85)*. The per-call cost is the executor's own frame and grew
        // with the instruction set, so the measurement is retaken whenever opcodes are added; the
        // figures above are 2026-09-04's, with spread, destructuring, `for … of`, classes, the
        // generator family, the async family and `with` all in the set. Admitting the generators
        // took the per-call cost from 3,158 bytes to 3,463 and the capacity from 21,246 calls to
        // 19,377; admitting `async`, `await` and `with` took it to 3,736 bytes and 17,963 calls,
        // which is still more than twice this row - so the ordering the row exists to guarantee
        // still holds. Those families were measured apart, at 18,277 and 19,288 calls, and
        // the figure here is the one measured on a build carrying all of them, because a per-frame
        // cost belongs to the whole dispatch loop rather than to a family. Admitting the CLASS BODY
        // - fields, static blocks, private names and a generator member - took it to 4,073 bytes
        // and 16,478 calls, which is 2.01 times this row: the ordering the row exists to guarantee
        // still holds, and the margin is now the narrowest it has been *(JSC-126)*.
        //
        // AND ADMITTING ASYNCHRONOUS ITERATION TOOK IT BELOW TWO, which is the first time this row
        // has been the thing that moved rather than the thing that survived. Five more dispatch
        // arms took the per-call cost to 4,551 bytes and the capacity on sixty-four megabytes to
        // 14,737 calls - 1.80 times this row, so a program granted this maximum could have reached
        // the stack before it reached the ceiling. The GUEST STACK was raised to ninety-six
        // megabytes and the capacity re-measured at 22,122 calls, which is 2.70 times this row
        // *(JSC-142)*. This row did not move: a ceiling a host may be granted is a policy figure,
        // and lowering it to fit a stack would be answering a question about the machine with a
        // change to what a program is allowed to do.
        values[(int)VmBudgetDimension.CallDepth] = 8_192;
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
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=AAA8EB
    // Broiler-Falsified-If: a row says charged for a dimension no code path charges, or inapplicable for one that is reachable
    // Broiler-Human:        PENDING
    private static VmBudgetDeclarationMatrix Matrix()
    {
        var rows = new VmBudgetApplicability[VmBudgetDimensions.Count];

        for (var index = 0; index < rows.Length; index++)
        {
            rows[index] = VmBudgetApplicability.Charged;
        }

        // THE THREE NESTED-LOAD ROWS ARE CHARGED FROM THE DAY `eval` EXISTS, and the descriptor's
        // own remark predicted exactly this: JS-8 flips them when guest loads are declared. Leaving
        // them inapplicable beside a declared guest-initiated load would be a descriptor
        // contradicting itself.

        VmBudgetDeclarationMatrix.TryCreate(rows, out var matrix);
        return matrix;
    }
}
