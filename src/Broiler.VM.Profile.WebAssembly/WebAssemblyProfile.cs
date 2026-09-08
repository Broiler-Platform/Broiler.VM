// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           3
// Human-reviewed:   0/13
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  3/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;
using System.Collections.Immutable;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The Broiler.VM WebAssembly language profile, exposed the way the contract requires: one static
/// accessor on the profile's own type, naming its own descriptor.
/// </summary>
/// <remarks>
/// <para>
/// <b>This build verifies and runs.</b> The descriptor below is a complete and valid entry in a
/// catalog; the verifier it names decodes a WebAssembly binary module and then validates it; and the
/// executor it names allocates a store, evaluates the module's global initialisers, applies its
/// element and data segments, runs its start function if it declares one, and interprets an exported
/// function. What it runs is the four numeric types, locals, globals, one linear memory with its
/// loads, stores, size and growth, structured control flow with all four branch forms, direct calls
/// and indirect calls through one table.
/// </para>
/// <para>
/// <b>WHAT IT DOES NOT DO IS RESTRICT THAT SURFACE PER FEATURE MANIFEST, AND THAT GAP IS STATED
/// RATHER THAN LEFT TO BE FOUND.</b> The roadmap's section 6 defines
/// <c>broiler.webassembly.slice</c> as one type, one function, one export, integer arithmetic, local
/// access and structured control flow - no memory, no table, no global and no float. This build's
/// decoder, validator and interpreter admit more than that under the same manifest identity, so a
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

    /// <summary>The one feature manifest this descriptor accepts, and which nothing implements.</summary>
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
    /// How much fuel and verifier work may be charged between two polls, which the descriptor
    /// declares and both the decoder and the interpreter pace themselves against.
    /// </summary>
    /// <remarks>
    /// It is the number the descriptor's uncharged-work row carries, named once so that a reader
    /// cannot find the interpreter pacing itself against a different one. Exceeding it is a profile
    /// fault and never a resource exhaustion, which is why the pacing polls before the charge that
    /// would cross it rather than after a fixed instruction count.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=D58353
    // Broiler-Falsified-If: the descriptor's uncharged-work row and this constant disagree
    // Broiler-Human:        PENDING
    public const uint MaxUnchargedWork = 65_536;

    /// <summary>This profile's descriptor: the one static accessor the contract asks for.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=FBD334
    // Broiler-Human:        PENDING
    public static VmProfileDescriptor Descriptor { get; } = Build();

    /// <summary>
    /// Builds the descriptor in one full-arity construction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Format version 1 means a bare WebAssembly binary module as the entire payload</b> - no
    /// Broiler magic, no framing, no envelope and no re-encoding - because the artifact descriptor
    /// already carries the profile identity, the format version, the feature manifest, the
    /// requested limits and the caller's identity beside the bytes. It says nothing about which
    /// specification version the module targets: the binary format's own version field has been 1
    /// across every published revision, and the language surface travels in the feature manifest.
    /// </para>
    /// <para>
    /// <b>What this build declares it does not do, it really does not do.</b> No host capability is
    /// imported, because nothing here calls one and the decoder refuses a module that declares an
    /// import. No guest-initiated load is declared, because WebAssembly has no dynamic-load
    /// instruction. Asynchronous instantiation and external suspension are both undeclared, because
    /// at every allocated manifest execution runs to completion or to a trap and there is no
    /// instruction of this format that parks a frame.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=FDF7A7
    // Broiler-Falsified-If: a row here states a capability, a guest load, a manifest or a format version this assembly does not implement without the surrounding text saying so
    // Broiler-Human:        PENDING
    private static VmProfileDescriptor Build()
    {
        VmDiagnosticsIdentity.TryCreate(Id, "broiler.webassembly.diagnostics", out var diagnostics);

        return new VmProfileDescriptor(
            profileId: Id,
            displayName: "Broiler WebAssembly",
            descriptorRevision: 1,
            supportedFormatVersions: new VmFormatVersionRange(1, 1),
            acceptedFeatureManifests: ImmutableArray.Create(SliceManifest),
            verifier: new WebAssemblyVerifier(Id, SliceManifest),
            executorFactory: static environment => new WebAssemblyExecutor(Id, environment),
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
    /// one until an interpreter exists whose native frame cost can be measured per claimed
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
    /// exists; and declared count, which this profile charges itself at one site.
    /// </para>
    /// <para>
    /// <b>The four that execution adds.</b> Fuel is charged once per instruction dispatched, plus a
    /// proportional charge for the two operations whose size a guest chooses - growing a memory, and
    /// initialising a segment at instantiation. Call depth is charged once per activation and
    /// released on return, and because frames are heap-allocated it is the only thing bounding
    /// recursion at all. Live bytes are reported retained for every byte a memory or a table holds
    /// and reported released when the store is dropped, because those are the dominant retained cost
    /// and a store that reported nothing would grow with no ceiling noticing.
    /// </para>
    /// <para>
    /// <b>Wall clock is charged in a sense worth spelling out, because this profile makes no
    /// wall-clock charge.</b> The core accrues it inside the poll rather than on a charge, so a
    /// profile that polls is a profile whose wall-clock ceiling bites - and this one polls, at the
    /// declared uncharged-work bound, in the decoder, in the validator and in the interpreter.
    /// Declaring the row inapplicable would say a wall-clock ceiling cannot stop an interpreter that
    /// has been running for an hour, which is false.
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
