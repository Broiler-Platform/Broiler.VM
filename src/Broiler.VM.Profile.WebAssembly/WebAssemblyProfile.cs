// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   14
// Annotated:        14/14
// Exempt:           4
// Human-reviewed:   0/14
// IP risk:          Low
// Security risk:    High
// Criteria:         6/6
// Resource impact:  2/10 max
// Unverified:       14
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;
using Broiler.VM.Ubc;
using System.Collections.Immutable;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The Broiler.VM WebAssembly language profile: its identity, the universal bytecode family it
/// declares, and the projections of its payloads.
/// </summary>
/// <remarks>
/// <para>
/// <b>This build translates, and the universal bytecode verifies and runs what it translated.</b> The
/// profile builds no descriptor of its own: it declares its family - <see cref="Registration"/> and
/// <see cref="Declaration"/> - and a composition root builds the descriptor from them over the
/// bytecode emitter it composes. <see cref="WasmTranslator"/> decodes a WebAssembly binary module with
/// this profile's own decoder, validates it, and lowers it into a universal bytecode artifact; the core
/// verifies the artifact through the family's hook, and the emitter instantiates it - allocating the
/// store, evaluating the module's global initialisers, applying its element and data segments and
/// running its start function if it declares one - and runs an exported function over the family's
/// handlers, <see cref="WasmFamily"/>. What it runs is the four numeric types, locals, globals, one
/// linear memory with its loads, stores, size and growth, structured control flow with all four branch
/// forms, direct calls and indirect calls through one table.
/// </para>
/// <para>
/// <b>Until milestone UBC-4 the profile carried a descriptor of its own</b>, whose verifier took a
/// bare WebAssembly module as the whole payload and whose executor ran it on an interpreter in this
/// assembly. UBC-4 retired that path - the descriptor, its verifier, its executor, the interpreter and
/// its value slot - once the composition roots translated first; the family's declaration keeps every
/// row the descriptor carried but its revision.
/// </para>
/// <para>
/// <b>WHAT IT DOES NOT DO IS RESTRICT THAT SURFACE PER FEATURE MANIFEST, AND THAT GAP IS STATED
/// RATHER THAN LEFT TO BE FOUND.</b> The roadmap's section 6 defines
/// <c>broiler.webassembly.slice</c> as one type, one function, one export, integer arithmetic, local
/// access and structured control flow - no memory, no table, no global and no float. This build's
/// decoder, validator and translator admit more than that under the same manifest identity, so a
/// module that declares the slice manifest and uses a float is accepted here and section 6 says it
/// should be refused at validation. The manifest identity is allocated; the surface behind it is
/// wider than its definition; and closing that is owned by the milestone that mints the second
/// manifest, not by a sentence here. No conformance is claimed for either.
/// </para>
/// <para>
/// There is deliberately <b>no aggregate type listing several profiles</b>. One would reference
/// every profile assembly and defeat the exact-closure reports a composition depends on, which is
/// why the core forbids such a type by name and why this component does not invent one either.
/// </para>
/// <para>
/// <b>One feature manifest is allocated and nothing has scored it.</b> Naming a manifest allocates
/// an identity; a manifest is earned by a retained run of the specification's own conformance suite
/// against it, and no such run exists - the suite is not pinned and no harness reads it. Modules
/// hand-encoded in this component's own harness are the only evidence there is, and a corpus this
/// component wrote cannot find a rejection this component never thought of.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D00910
// Broiler-Human:        PENDING
public static class WebAssemblyProfile
{
    /// <summary>This profile's identity.</summary>
    /// <remarks>
    /// The first label <c>broiler</c> is reserved and pairs with a <c>Broiler.*</c> package
    /// identity, which this profile takes on. The spelling is <c>broiler.webassembly</c> rather
    /// than <c>broiler.wasm</c>, and it is inherited by every manifest identity, every diagnostics
    /// namespace and every support-table row for the life of the component.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7A6C8B
    // Broiler-Human:        PENDING
    public static VmProfileId Id { get; } = VmProfileId.Parse("broiler.webassembly");

    /// <summary>The one feature manifest the family's table is keyed on, and which nothing implements.</summary>
    /// <remarks>
    /// A manifest identity is allocated by being named and is earned by a retained run scoring it.
    /// This one is allocated here and earned nowhere: an artifact naming it can be verified, and
    /// nothing has scored the surface against the specification's own suite.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7ED779
    // Broiler-Human:        PENDING
    public static VmFeatureManifestId SliceManifest { get; } =
        VmFeatureManifestId.Parse("broiler.webassembly.slice");

    /// <summary>The payload kind identifying the values an entry point returned.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6BBDCE
    // Broiler-Human:        PENDING
    public const int ResultsKindId = 2001;

    /// <summary>The payload kind identifying a trap.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CDB336
    // Broiler-Human:        PENDING
    public const int TrapKindId = 2002;

    /// <summary>The payload kind identifying an entry point that could not be resolved.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B32184
    // Broiler-Human:        PENDING
    public const int EntryPointFaultKindId = 2003;

    /// <summary>
    /// How much fuel and verifier work may be charged between two polls, which the family's
    /// declaration carries and the decoder, the validator, the translator and the family's
    /// instantiation pace themselves against.
    /// </summary>
    /// <remarks>
    /// It is the number the declaration's uncharged-work row carries, named once so that a reader
    /// cannot find one of them pacing itself against a different one. Exceeding it is a profile fault
    /// and never a resource exhaustion, which is why the pacing polls before the charge that would
    /// cross it rather than after a fixed count.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D58353
    // Broiler-Falsified-If: the declaration's uncharged-work row and this constant disagree
    // Broiler-Human:        PENDING
    public const uint MaxUnchargedWork = 65_536;

    /// <summary>
    /// The WebAssembly family's registration: its identity, its one instruction table under
    /// <see cref="SliceManifest"/>, its verifier hook, and the universal bytecode contract version it
    /// was written for and compiled against.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is what a composition root builds the profile's descriptor from</b>, with
    /// <see cref="Declaration"/> and the forms the root composes, through
    /// <c>UbcDescriptors.Build</c>: this profile references no emitter, so it cannot build that
    /// descriptor itself. The descriptor verifies universal bytecode an artifact names this profile
    /// in, and runs it over the family's handlers, <see cref="WasmFamily"/>.
    /// </para>
    /// <para>
    /// <b>It is the profile's only descriptor source since milestone UBC-4</b>, which retired the
    /// descriptor the profile built itself over bare modules once the roots translated first.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=9322B9
    // Broiler-Falsified-If: the registration carries a table of another identity or manifest, or a contract version other than the one the family was written for
    // Broiler-Human:        PENDING
    public static UbcFamilyRegistration<WasmFamily> Registration { get; } =
        new(WasmFamilyTable.Identity, [WasmFamilyTable.Table], new WasmFamilyVerifier(), authoredUbcContractVersion: 2);

    /// <summary>
    /// The descriptor rows the WebAssembly family declares itself - rows 1 to 3 and 8 to 30 - which a
    /// composition root hands <c>UbcDescriptors.Build</c> with <see cref="Registration"/>.
    /// </summary>
    /// <remarks>
    /// Every row is the one the profile's bare-module descriptor carried until milestone UBC-4 retired
    /// it, but the revision, which is 2: the descriptor built from these rows is a revision of the
    /// profile's descriptor whose format, manifests, verifier and executor are the universal bytecode's
    /// rather than the bare module's. Rows 4 to 7 are not here, because they are the universal
    /// bytecode's.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=B55BAA
    // Broiler-Falsified-If: a row here differs from the same row of the retired bare-module descriptor, other than the revision
    // Broiler-Human:        PENDING
    public static UbcFamilyDeclaration Declaration { get; } = Declare();

    /// <summary>
    /// Evaluates one numeric row with this profile's own arms - the reference handler - over
    /// <paramref name="a"/>, the deeper operand, and <paramref name="b"/>, the top one, which a
    /// one-operand row does not read.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Answers true with the result's bits, a thirty-two-bit result's in the low half; or true with
    /// the trap the arm raised in <paramref name="trap"/> and zero bits. <paramref name="trap"/> is zero,
    /// which names no trap, when none was raised. Answers false when the arms have no answer for the
    /// row, and for a byte that is not a numeric row of the family's table.
    /// </para>
    /// <para>
    /// <b>It is the door the universal bytecode's obligation E2 reads the profile through.</b> The arms
    /// are the retired bare-module interpreter's own, including the routing defect that gives the twelve
    /// float comparisons, 0x5B to 0x66, no answer; they canonicalise no NaN, so a comparison with the
    /// primitive table under the family's NaN flag must read a NaN answer as a NaN.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=Medium; Resources=1; Fingerprint=B05DAD
    // Broiler-Human:        PENDING
    public static bool TryEvaluateReference(byte opcode, ulong a, ulong b, out ulong bits, out WasmTrapKind trap) =>
        WasmReferenceNumerics.TryEvaluate(opcode, a, b, out bits, out trap);

    /// <summary>
    /// The family's declaration: the descriptor's own rows, in one full-arity construction, at
    /// descriptor revision 2.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The rows are the ones the profile's bare-module descriptor carried at revision 1, spelled here
    /// rather than read off it so that the declaration stood when that descriptor was retired at
    /// milestone UBC-4; every row but the revision agrees with it.
    /// </para>
    /// <para>
    /// <b>What this build declares it does not do, it really does not do.</b> No host capability is
    /// imported, because nothing here calls one and the decoder refuses a module that declares an
    /// import. No guest-initiated load is declared, because WebAssembly has no dynamic-load
    /// instruction. Asynchronous instantiation and external suspension are both undeclared, because
    /// at every allocated manifest execution runs to completion or to a trap and no row of the family's
    /// table parks a frame.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=2; Fingerprint=1B63C5
    // Broiler-Falsified-If: a row here states a capability, a guest load, a manifest or a limit this assembly does not implement without the surrounding text saying so
    // Broiler-Human:        PENDING
    private static UbcFamilyDeclaration Declare()
    {
        VmDiagnosticsIdentity.TryCreate(Id, "broiler.webassembly.diagnostics", out var diagnostics);

        return new UbcFamilyDeclaration(
            profileId: Id,
            displayName: "Broiler WebAssembly",
            descriptorRevision: 2,
            artifactRepresentationKind: VmArtifactRepresentationKind.Decoded,
            artifactLifetimeKind: VmArtifactLifetimeKind.Managed,
            supportsConcurrentVerification: true,
            threadAffinity: VmThreadAffinity.Agile,

            // The bound a decoder would have to poll within. The core enforces the uncharged-work
            // row below rather than this one; the two are set to the same number so a later reader
            // cannot take a difference between them for a decision somebody made.
            cancellationPollBound: 65_536,

            // AN ABANDON BUDGET OF ZERO IS A STATEMENT ABOUT GUEST CODE AND NOT ABOUT MEMORY. A
            // store holds arrays and nothing that needs running to be released, so an unwind has no
            // work to do under an allowance - it drops what it holds and tells the meter.
            abandonBudget: 0,
            limitDefaults: Defaults(),
            profileHardMaxima: Maxima(),
            budgetDeclarationMatrix: Matrix(),

            // NO IMPORT, AND THAT IS A PROPERTY OF THE BUILD RATHER THAN OF THE SCOPE. The slice
            // manifest admits no import instruction at all, and this assembly has no linker to
            // resolve one with. Imports arrive with the linking milestone.
            hostCapabilityDescriptors: ImmutableArray<VmCapabilityImport>.Empty,

            // WebAssembly has no instruction that asks its embedder for another artifact, so there
            // is no guest-initiated load to declare and no mediator this profile would ever hold.
            guestInitiatedLoads: VmGuestLoadDeclaration.NotDeclared,
            asynchronousInstantiation: VmDeclaration.NotDeclared,
            externalSuspension: VmDeclaration.NotDeclared,

            // Disjoint from the JavaScript profile's 1000-1099, so two profiles composed together
            // cannot mint a payload identity the other's range would accept. Three of the hundred
            // are used: the values an entry point returned, a trap, and an entry point that could
            // not be resolved.
            payloadKindIdRange: new VmPayloadKindIdRange(2000, 2099),
            authoredCoreContractVersion: 1,
            conformanceManifestId: VmConformanceManifestId.Create("broiler.webassembly.conformance"),
            conformanceManifestVersion: 1,
            diagnosticsIdentity: diagnostics,
            packageIdentity: new VmPackageIdentity(
                "Broiler.VM.Profile.WebAssembly", "0.1.0-preview.1", "broiler.webassembly"),
            faultRecovery: VmFaultRecovery.InstanceRecoverable,
            maxUnchargedWork: MaxUnchargedWork,
            chargingGranularity: 1,
            artifactSharing: VmArtifactSharing.Shareable);
    }

    /// <summary>Reads the values an entry point returned, if it returned normally.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=59538F
    // Broiler-Human:        PENDING
    public static bool TryGetResults(
        in VmInvocationResult result, out WebAssemblyResults results) =>
        result.TryGetPayload(out results);

    /// <summary>Reads the trap that ended an invocation, if one did.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=4F1514
    // Broiler-Human:        PENDING
    public static bool TryGetTrap(in VmInvocationResult result, out WebAssemblyTrap trap) =>
        result.TryGetPayload(out trap);

    /// <summary>Reads the trap that ended an instantiation, if one did.</summary>
    /// <remarks>
    /// A trapping start function and an out-of-range segment both end an instantiation this way, and
    /// both leave no instance behind - which is why the projection exists on this stage as well as on
    /// invocation rather than being read off a handle a caller does not have.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=60D2E4
    // Broiler-Human:        PENDING
    public static bool TryGetTrap(in VmInstantiationResult result, out WebAssemblyTrap trap) =>
        result.TryGetPayload(out trap);

    /// <summary>Reads why an entry point could not be resolved, if it could not.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D3E8CC
    // Broiler-Human:        PENDING
    public static bool TryGetEntryPointFault(
        in VmInvocationResult result, out WebAssemblyEntryPointFault fault) =>
        result.TryGetPayload(out fault);

    /// <summary>
    /// The bounded defaults a host adopts when it does not state its own.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the vector that reaches other profiles.</b> A host adopting profile defaults
    /// rather than stating numbers gets the tightest default in the catalog, per dimension, because
    /// at runtime creation no profile has been selected. So a dimension this profile does not
    /// charge still carries a generous finite number here: a zero would hand a neighbour's guest a
    /// ceiling of zero on a dimension this profile never touches, and the failure would surface in
    /// somebody else's verifier.
    /// </para>
    /// <para>
    /// <c>Unconstrained</c> is not available either - the core refuses a descriptor whose defaults
    /// carry an unconstrained slot - so the three guest-load rows are written as large finite
    /// numbers rather than left open.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=00D0CA
    // Broiler-Falsified-If: a default here is zero on a dimension this profile declares inapplicable, or any default exceeds its maximum
    // Broiler-Human:        PENDING
    private static VmLimitVector Defaults()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        values[(int)VmBudgetDimension.Fuel] = 50_000_000;
        values[(int)VmBudgetDimension.WallClock] = 10_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 64L * 1024 * 1024;
        values[(int)VmBudgetDimension.HostCalls] = 1_000_000;
        values[(int)VmBudgetDimension.VerifierWork] = 100_000_000;
        values[(int)VmBudgetDimension.LiveBytes] = 64L * 1024 * 1024;
        values[(int)VmBudgetDimension.CallDepth] = 1_024;
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
    /// A maximum binds this profile's own artifacts and nobody else's: it is applied at
    /// verification against the profile the artifact names, so a tight one constrains what this
    /// profile accepts and reaches no profile composed beside it. It is not a statement of what
    /// this profile uses - the defaults are that - but of the most it would tolerate a host
    /// granting. The call-depth row is a placeholder rather than a measurement, and it must stay
    /// one until a form runs this family whose native frame cost can be measured per claimed
    /// architecture.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=162AEB
    // Broiler-Human:        PENDING
    private static VmLimitVector Maxima()
    {
        var values = new ulong[VmBudgetDimensions.Count];
        values[(int)VmBudgetDimension.Fuel] = 1_099_511_627_776;
        values[(int)VmBudgetDimension.WallClock] = 3_600_000;
        values[(int)VmBudgetDimension.AllocatedBytes] = 4_294_967_296;
        values[(int)VmBudgetDimension.HostCalls] = 4_294_967_295;
        values[(int)VmBudgetDimension.VerifierWork] = 1_099_511_627_776;
        values[(int)VmBudgetDimension.LiveBytes] = 4_294_967_296;
        values[(int)VmBudgetDimension.CallDepth] = 8_192;
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
    /// Which of the fifteen dimensions this build charges.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>TEN ROWS READ CHARGED BECAUSE SOME PATH IN THIS ASSEMBLY GENUINELY CHARGES THEM, AND NOT
    /// ONE MORE.</b> The matrix is a statement about what this assembly does, so it moves when the
    /// code moves and not before. Six of the ten were already true of verification alone: artifact
    /// bytes, because the bounded reader refuses a payload above the ceiling before it is
    /// constructed; section count, because every section - custom sections included - is entered
    /// through the framing member that spends it; structural depth, charged twice over and both
    /// times as a high-water mark with a release on the way out, by the reader for section nesting
    /// and by the validator for control nesting; verifier work, charged per byte consumed; allocated
    /// bytes, because every array a verified module keeps is reserved against the meter before it
    /// exists; and declared count, which this profile charges itself at one site. Since milestone UBC-4
    /// those charges of the decoder and the validator reach the translator's own meter, and the core's
    /// meter is charged in the same six dimensions by its verification of the translated artifact - the
    /// universal bytecode's reader and walk, and this family's hook over the module definitions.
    /// </para>
    /// <para>
    /// <b>The four that execution adds.</b> Fuel is charged once per row an emitter executes, plus a
    /// proportional charge for the two operations whose size a guest chooses - growing a memory, and
    /// initialising a segment at instantiation. Call depth is charged once per activation and
    /// released on return, and because frames are heap-allocated it is the only thing bounding
    /// recursion at all. Live bytes are charged retained for every byte a memory or a table holds,
    /// before the array exists, and reported released when the store is dropped, because those are the
    /// dominant retained cost and a store that retained nothing would grow with no ceiling noticing.
    /// </para>
    /// <para>
    /// <b>Wall clock is charged in a sense worth spelling out, because this profile makes no
    /// wall-clock charge.</b> The core accrues it inside the poll rather than on a charge, so a
    /// profile that polls is a profile whose wall-clock ceiling bites - and this family polls, at the
    /// declared uncharged-work bound, through the universal bytecode: in the core's walk over the
    /// translated artifact, in an emitter's loop and in the family's instantiation. (The decoder and the
    /// validator poll the translator's own meter, which accrues no wall clock.) Declaring the row
    /// inapplicable would say a wall-clock ceiling cannot stop a guest that has been running for an
    /// hour, which is false.
    /// </para>
    /// <para>
    /// <b>Five rows read inapplicable, and each is a fact rather than a placeholder.</b> Host calls
    /// belong to an import, and the decoder refuses a module that declares one. The three
    /// guest-load rows belong to an instruction WebAssembly does not have. Live runtimes belong to
    /// the host. The milestone that first makes one of these true is the milestone that rewrites its
    /// row.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=18E0B0
    // Broiler-Falsified-If: a row says charged for a dimension no code path in this assembly charges or polls against, or a dimension this assembly charges is declared inapplicable
    // Broiler-Human:        PENDING
    private static VmBudgetDeclarationMatrix Matrix()
    {
        var rows = new VmBudgetApplicability[VmBudgetDimensions.Count];

        for (var index = 0; index < rows.Length; index++)
        {
            rows[index] = VmBudgetApplicability.NotApplicable;
        }

        rows[(int)VmBudgetDimension.ArtifactBytes] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.SectionCount] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.DeclaredCount] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.StructuralDepth] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.AllocatedBytes] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.VerifierWork] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.Fuel] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.CallDepth] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.LiveBytes] = VmBudgetApplicability.Charged;
        rows[(int)VmBudgetDimension.WallClock] = VmBudgetApplicability.Charged;

        VmBudgetDeclarationMatrix.TryCreate(rows, out var matrix);
        return matrix;
    }
}
