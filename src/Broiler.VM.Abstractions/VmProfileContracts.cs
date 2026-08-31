// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   39
// Annotated:        39/39
// Exempt:           33
// Human-reviewed:   0/39
// IP risk:          Low
// Security risk:    High
// Criteria:         9/8
// Resource impact:  0/10 max
// Unverified:       39
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM;

/// <summary>
/// The profile-facing metering surface. Exactly four members.
/// </summary>
/// <remarks>
/// <para>
/// There is no grant, refund, reset, extend or withdraw, and no member reads a remaining or
/// effective value. A profile learns a limit only by reaching it and being refused. That asymmetry
/// is the contract: a profile that could read its remaining allowance could branch on it, and a
/// profile that could branch on it could spend exactly up to a ceiling on every operation while
/// remaining formally compliant.
/// </para>
/// <para>
/// <c>amount</c> is unsigned, so a negative charge - a refund by another name - is not expressible.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7FA11B
// Broiler-Human:        PENDING
public interface IVmMeter
{
    /// <summary>
    /// Charges <paramref name="amount"/> against <paramref name="dimension"/>. Returns
    /// <see langword="false"/> when the allowance or ceiling will not admit it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=76973E
    // Broiler-Human:        PENDING
    bool TryCharge(VmBudgetDimension dimension, ulong amount);

    /// <summary>
    /// One combined budget and cancellation check. <see langword="false"/> means stop. A profile
    /// calls it at least as often as its declared uncharged-work bound requires.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=84FB1F
    // Broiler-Human:        PENDING
    bool Poll();

    /// <summary>Reports that the profile is now retaining <paramref name="amount"/> more.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=49758B
    // Broiler-Human:        PENDING
    void ReportRetained(VmBudgetDimension dimension, ulong amount);

    /// <summary>Reports that the profile has released <paramref name="amount"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F76A84
    // Broiler-Human:        PENDING
    void ReportReleased(VmBudgetDimension dimension, ulong amount);
}

/// <summary>
/// The immutable, profile-owned decoded form or snapshot a verified handle carries.
/// </summary>
/// <remarks>
/// A marker: the core stores it and calls nothing on it. Everything reachable from it must be
/// immutable once verification returns and safe for unsynchronised concurrent readers, because a
/// shareable handle is read by several runtimes at once with no lock between them.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=9B3EE1
// Broiler-Falsified-If: the core calls anything on a stored state, or a state reachable from a shared handle can be mutated
// Broiler-Human:        PENDING
public interface IVmVerifiedState
{
}

/// <summary>The profile-owned mutable state of one instance. The core never inspects it.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=FC8DAD
// Broiler-Falsified-If: the core reads an instance state, or one reaches an executor other than the profile that made it
// Broiler-Human:        PENDING
public interface IVmInstanceState
{
}

/// <summary>The profile-owned captured continuation of a suspended operation.</summary>
/// <remarks>
/// The core holds it, hands it back on resume, and passes it to the profile's terminal-unwind entry
/// point on abandonment. It never inspects it: what a paused profile exposes is the profile's own
/// surface.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=9DB83C
// Broiler-Falsified-If: the core inspects a continuation, or one is handed back to an operation it was not captured from
// Broiler-Human:        PENDING
public interface IVmProfileContinuation
{
}

/// <summary>What a caller asks an instance to do.</summary>
/// <remarks>
/// The entry point is UTF-8 text rather than a string or a symbol table index, because naming is a
/// language concept: the core carries the bytes and the profile decides what they mean.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=B2651B
// Broiler-Falsified-If: the core decodes, re-encodes or trims the entry-point bytes rather than carrying them verbatim
// Broiler-Human:        PENDING
public readonly ref struct VmInvocationRequest
{
    /// <summary>Creates a request naming an entry point.</summary>
    public VmInvocationRequest(VmUtf8Text entryPoint) => EntryPoint = entryPoint;

    /// <summary>The profile-interpreted entry point.</summary>
    public VmUtf8Text EntryPoint { get; }
}

/// <summary>
/// Everything a verification is a total function of, and nothing else.
/// </summary>
/// <remarks>
/// No runtime, no instance, no executor state, no artifact provider, and no capability
/// <em>invoker</em>. A verifier may read capability descriptors and record them as an identity
/// component; it may never invoke one. That is what makes a composition verify successfully with
/// capability implementations that are absent or that throw, and it is what keeps verification
/// separable from execution.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A43F7C
// Broiler-Falsified-If: the context reaches anything a verifier could invoke rather than capability descriptors alone
// Broiler-Human:        PENDING
public interface IVmVerificationContext
{
    /// <summary>The materialized ceilings this verification runs under.</summary>
    VmEffectiveCeilings Ceilings { get; }

    /// <summary>The meter to charge verifier work and allocation against.</summary>
    IVmMeter Meter { get; }

    /// <summary>The capability shapes registered into the verifying runtime, in canonical order.</summary>
    System.Collections.Immutable.ImmutableArray<VmHostCapabilityDescriptor> RegisteredCapabilities { get; }

    /// <summary>Looks up one registered capability shape by identity and exact version.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0006 s6; IP=Low; Security=Medium; Resources=0; Fingerprint=7EDEF8
    // Broiler-Human:        PENDING
    bool TryGetCapabilityDescriptor(
        VmCapabilityId capabilityId,
        int version,
        out VmHostCapabilityDescriptor descriptor);
}

/// <summary>
/// What a profile verifier answers with: the four classes it may map its own failures onto, plus
/// success.
/// </summary>
/// <remarks>
/// This is deliberately narrower than the verification stage's own row. <c>Normal</c> as a category
/// and <c>InvalidState</c> are core-owned and unreachable from a verifier: verify called on a
/// disposed runtime is an invalid state like every other stage, and success is the core's to
/// declare. Keeping the two lists apart by <em>type</em> rather than by review is why a profile
/// author cannot end up believing the core will accept an outcome it cannot represent.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=8DC8E5
// Broiler-Human:        PENDING
public readonly struct VmVerifierOutcome
{
    // Broiler-AI:           Origin=AI; Spec=ADR-0006 s5; IP=Low; Security=Medium; Resources=0; Fingerprint=487D97
    // Broiler-Human:        PENDING
    private VmVerifierOutcome(
        VmOutcome category,
        VmReason reason,
        IVmVerifiedState? state,
        VmArtifactSharing sharing,
        int profileDiagnosticCode,
        VmSourcePosition position,
        VmBudgetDimension dimension,
        VmBudgetScope scope)
    {
        Category = category;
        Reason = reason;
        State = state;
        NarrowedSharing = sharing;
        ProfileDiagnosticCode = profileDiagnosticCode;
        Position = position;
        ExhaustedDimension = dimension;
        ExhaustedScope = scope;
    }

    /// <summary>
    /// The artifact verified. <paramref name="narrowedSharing"/> may narrow the descriptor's
    /// declared sharing and may never widen it, which the core checks rather than trusts.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0006 s5; IP=Low; Security=Medium; Resources=0; Fingerprint=D95005
    // Broiler-Human:        PENDING
    public static VmVerifierOutcome Verified(IVmVerifiedState state, VmArtifactSharing narrowedSharing) =>
        new(VmOutcome.Normal, VmReason.NormalCompleted, state, narrowedSharing, 0, default,
            VmBudgetDimension.Fuel, VmBudgetScope.Artifact);

    /// <summary>
    /// The verifier does not host the identity it was handed. It is a distinct answer from a
    /// malformed payload.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0006 s5; IP=Low; Security=Low; Resources=0; Fingerprint=11657D
    // Broiler-Human:        PENDING
    public static VmVerifierOutcome UnsupportedProfile() =>
        new(VmOutcome.UnsupportedProfile, VmReason.ProfileNotInCatalog, null,
            VmArtifactSharing.RuntimeScoped, 0, default, VmBudgetDimension.Fuel, VmBudgetScope.Artifact);

    /// <summary>The bytes are not a well-formed artifact of this profile and format version.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0006 s5; IP=Low; Security=Low; Resources=0; Fingerprint=C7F217
    // Broiler-Human:        PENDING
    public static VmVerifierOutcome InvalidArtifact(
        VmReason reason,
        int profileDiagnosticCode,
        VmSourcePosition position) =>
        new(VmOutcome.InvalidArtifact, reason, null, VmArtifactSharing.RuntimeScoped,
            profileDiagnosticCode, position, VmBudgetDimension.Fuel, VmBudgetScope.Artifact);

    /// <summary>Verification ran out of a named budget in a named scope.</summary>
    /// <remarks>
    /// The reason is derived from the dimension's class and is not a parameter: an allowance is
    /// spent and a ceiling is reached, the vocabulary distinguishes them, and which of the two a
    /// dimension is, is a fixed property of that dimension rather than a judgement the caller
    /// makes. Hardcoding either produced the defect this replaces - every profile verifier
    /// answered <see cref="VmReason.AllowanceExhausted"/> for a section-count ceiling while the
    /// core answered <see cref="VmReason.CeilingReached"/> for the identical breach of the
    /// artifact-bytes ceiling one call earlier.
    /// </remarks>
    // Broiler-AI:           Origin=AI; Spec=ADR-0006 s5; IP=Low; Security=Low; Resources=0; Fingerprint=9374B8
    // Broiler-Human:        PENDING
    public static VmVerifierOutcome ResourceExhaustion(VmBudgetDimension dimension, VmBudgetScope scope) =>
        new(VmOutcome.ResourceExhaustion,
            VmBudgetDimensions.ClassOf(dimension) is VmBudgetClass.Ceiling
                ? VmReason.CeilingReached
                : VmReason.AllowanceExhausted,
            null,
            VmArtifactSharing.RuntimeScoped, 0, default, dimension, scope);

    /// <summary>A cancellation request was observed at a polling point.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0006 s5; IP=Low; Security=Low; Resources=0; Fingerprint=F6446E
    // Broiler-Human:        PENDING
    public static VmVerifierOutcome Cancellation() =>
        new(VmOutcome.Cancellation, VmReason.Cancelled, null, VmArtifactSharing.RuntimeScoped,
            0, default, VmBudgetDimension.Fuel, VmBudgetScope.Artifact);

    /// <summary>Which of the five answers this is.</summary>
    public VmOutcome Category { get; }

    /// <summary>The reason accompanying the category.</summary>
    public VmReason Reason { get; }

    /// <summary>The verified state, present only on the success answer.</summary>
    public IVmVerifiedState? State { get; }

    /// <summary>The sharing the verifier narrowed to.</summary>
    public VmArtifactSharing NarrowedSharing { get; }

    /// <summary>The profile's own stable diagnostic code.</summary>
    public int ProfileDiagnosticCode { get; }

    /// <summary>Where in the artifact the failure was found.</summary>
    public VmSourcePosition Position { get; }

    /// <summary>Which dimension was exhausted.</summary>
    public VmBudgetDimension ExhaustedDimension { get; }

    /// <summary>At which scope.</summary>
    public VmBudgetScope ExhaustedScope { get; }
}

/// <summary>
/// A VM profile's verification entry point.
/// </summary>
/// <remarks>
/// <para>
/// One entry point, taking the descriptor, the complete byte range and a cancellation token, and
/// nothing else. <see cref="System.ReadOnlySpan{T}"/> is the only byte form, which is also why no
/// asynchronous verification member can exist: a span cannot cross an await, so streaming
/// verification cannot arrive as a quiet widening of this signature. Adding it is a numbered
/// amendment.
/// </para>
/// <para>
/// The verifier declares its own identity and both contract integers. They are compared at catalog
/// construction against the descriptor that names it, so a descriptor cannot advertise one profile
/// and hand the core another profile's verifier.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=8ED829
// Broiler-Falsified-If: a verifier whose declared identity differs from the descriptor naming it is admitted to a catalog
// Broiler-Human:        PENDING
public interface IVmProfileVerifier
{
    /// <summary>The profile this verifier belongs to.</summary>
    VmProfileId ProfileId { get; }

    /// <summary>The core contract version the verifier was compiled against.</summary>
    int BuiltAgainstCoreContractVersion { get; }

    /// <summary>The core contract version its author wrote it for.</summary>
    int AuthoredCoreContractVersion { get; }

    /// <summary>
    /// The verifier's own semantic version. It is part of a verified handle's identity, so a
    /// verifier change invalidates sharing with handles produced by the old one.
    /// </summary>
    int VerifierSemanticVersion { get; }

    /// <summary>Verifies <paramref name="payload"/> against <paramref name="descriptor"/>.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0006 s6; IP=Low; Security=High; Resources=0; Fingerprint=ED6BA8
    // Broiler-Falsified-If: the payload arrives as anything but a span, or a second member here can answer a verification
    // Broiler-Human:        PENDING
    VmVerifierOutcome Verify(
        in VmArtifactDescriptor descriptor,
        System.ReadOnlySpan<byte> payload,
        IVmVerificationContext context,
        System.Threading.CancellationToken cancellationToken);
}

/// <summary>What the runtime hands a profile executor when it creates one.</summary>
/// <remarks>
/// <see cref="TryGetArtifactLoadMediator"/> yields nothing for a profile that does not declare
/// guest-initiated loads. The core hands a non-declaring profile nothing at all, so an undeclared
/// request is structurally unrepresentable rather than a runtime check that could be reported,
/// logged, or forgotten.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F33F5D
// Broiler-Human:        PENDING
public interface IVmExecutionEnvironment
{
    /// <summary>The profile this executor serves.</summary>
    VmProfileId ProfileId { get; }

    /// <summary>The meter every charge goes through.</summary>
    IVmMeter Meter { get; }

    /// <summary>The bound capability table, addressed by index.</summary>
    IVmHostCapabilityInvoker Capabilities { get; }

    /// <summary>
    /// The mediator, for a declaring profile in a composition that registered a provider. False
    /// means there is no path to further code from here, and that is the whole of the refusal.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=647389
    // Broiler-Human:        PENDING
    bool TryGetArtifactLoadMediator(out IVmArtifactLoadMediator mediator);
}

/// <summary>
/// The bound host-capability table, addressed by index.
/// </summary>
/// <remarks>
/// Invocation is by index into an immutable table fixed at binding. No member returns the
/// registered capability set, resolves a capability by name, or returns a CLR type or member: a
/// profile may ask only whether index <em>k</em> is bound. That is what stops a capability registry
/// becoming an ambient platform surface.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E5E867
// Broiler-Human:        PENDING
public interface IVmHostCapabilityInvoker
{
    /// <summary>How many binding slots the profile declared.</summary>
    int BindingCount { get; }

    /// <summary>Whether slot <paramref name="bindingIndex"/> is bound.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=3130A9
    // Broiler-Human:        PENDING
    bool IsBound(int bindingIndex);

    /// <summary>Invokes a value capability with integer arguments.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=9A9BBB
    // Broiler-Human:        PENDING
    VmHostCallOutcome Invoke(int bindingIndex, System.ReadOnlySpan<long> arguments, out long result);

    /// <summary>Invokes a value capability with a byte argument, receiving an opaque reference.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=CB9CEE
    // Broiler-Human:        PENDING
    VmHostCallOutcome InvokeBytes(int bindingIndex, VmBytes argument, out VmOpaqueRef result);
}

/// <summary>What a profile executor answers with for one lifecycle step.</summary>
/// <remarks>
/// Nothing here is a <see cref="VmOutcome"/>. The core maps a step onto the stage result, applies
/// the outcome-to-instance-state mapping, and enforces the latch for a nested terminal outcome the
/// profile failed to convert. Letting a profile name a core category would be letting it write the
/// core's result enum one language at a time.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=6F3649
// Broiler-Human:        PENDING
public readonly struct VmExecutionStep
{
    private VmExecutionStep(
        VmExecutionStepKind kind,
        IVmInstanceState? state,
        IVmProfileContinuation? continuation,
        IVmProfilePayload? payload,
        VmReason reason)
    {
        Kind = kind;
        State = state;
        Continuation = continuation;
        Payload = payload;
        Reason = reason;
    }

    /// <summary>The step completed, optionally producing a typed payload.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=6AA8B9
    // Broiler-Human:        PENDING
    public static VmExecutionStep Completed(IVmProfilePayload? payload) =>
        new(VmExecutionStepKind.Completed, null, null, payload, VmReason.None);

    /// <summary>Instantiation completed and produced the instance's profile-owned state.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=5EFEAD
    // Broiler-Human:        PENDING
    public static VmExecutionStep Instantiated(IVmInstanceState state, IVmProfilePayload? payload) =>
        new(VmExecutionStepKind.Instantiated, state, null, payload, VmReason.None);

    /// <summary>The step parked, handing the core a continuation to resume from.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1BD1D2
    // Broiler-Human:        PENDING
    public static VmExecutionStep Suspended(IVmProfileContinuation continuation, IVmProfilePayload? projection) =>
        new(VmExecutionStepKind.Suspended, null, continuation, projection, VmReason.None);

    /// <summary>The step ended in a language-defined fault, carried as a typed payload.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=89FCD9
    // Broiler-Human:        PENDING
    public static VmExecutionStep Faulted(IVmProfilePayload? payload) =>
        new(VmExecutionStepKind.Faulted, null, null, payload, VmReason.None);

    /// <summary>
    /// The profile violated its own declared contract. It is a separate answer from a language
    /// fault so that a defect in the profile is never reported as a defect in the guest program.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=CF0528
    // Broiler-Human:        PENDING
    public static VmExecutionStep ContractViolation(VmReason reason) =>
        new(VmExecutionStepKind.ContractViolation, null, null, null, reason);

    /// <summary>Which of the five answers this is.</summary>
    public VmExecutionStepKind Kind { get; }

    /// <summary>The instance state, present only on the instantiated answer.</summary>
    public IVmInstanceState? State { get; }

    /// <summary>The continuation, present only on the suspended answer.</summary>
    public IVmProfileContinuation? Continuation { get; }

    /// <summary>The typed payload, where the profile produced one.</summary>
    public IVmProfilePayload? Payload { get; }

    /// <summary>The reason, present only on the contract-violation answer.</summary>
    public VmReason Reason { get; }
}

/// <summary>The five answers a profile executor may give for one step.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=15A717
// Broiler-Falsified-If: a member's numeric value changes, or a kind exists the core's step-to-stage mapping has no arm for
// Broiler-Human:        PENDING
public enum VmExecutionStepKind
{
    /// <summary>The step completed.</summary>
    Completed = 0,

    /// <summary>Instantiation produced an instance.</summary>
    Instantiated = 1,

    /// <summary>The step parked and is resumable.</summary>
    Suspended = 2,

    /// <summary>The step ended in a language-defined fault.</summary>
    Faulted = 3,

    /// <summary>The profile violated its declared contract.</summary>
    ContractViolation = 4,
}

/// <summary>
/// The per-runtime executor a profile factory produces.
/// </summary>
/// <remarks>
/// One executor per runtime, created lazily at first instantiation rather than at runtime creation,
/// so a composition that never runs a profile never pays for its executor. The executor's declared
/// identity is checked when it is created, and a mismatch is <em>returned</em> as a profile fault
/// rather than thrown: it is a defect in a profile, observed at run time, not a composition error.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4CE117
// Broiler-Human:        PENDING
public interface IVmProfileExecutor
{
    /// <summary>The profile this executor claims to be.</summary>
    VmProfileId ProfileId { get; }

    /// <summary>Instantiates profile-owned mutable state from a verified artifact.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B853C0
    // Broiler-Human:        PENDING
    VmExecutionStep Instantiate(
        VmVerifiedArtifact artifact,
        System.Threading.CancellationToken cancellationToken);

    /// <summary>Invokes an entry point against instance state.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=6A0CE6
    // Broiler-Human:        PENDING
    VmExecutionStep Invoke(
        IVmInstanceState state,
        in VmInvocationRequest request,
        System.Threading.CancellationToken cancellationToken);

    /// <summary>Resumes a parked step from its continuation.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F0E5BB
    // Broiler-Human:        PENDING
    VmExecutionStep Resume(
        IVmInstanceState state,
        IVmProfileContinuation continuation,
        System.Threading.CancellationToken cancellationToken);

    /// <summary>
    /// The terminal-unwind entry point, run on the disposing thread under the tighter of the
    /// profile's declared abandon budget and the runtime's unwind budget. A profile that needs no
    /// unwinding leaves it empty and its continuation is dropped deterministically.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F78E9B
    // Broiler-Human:        PENDING
    void Unwind(IVmProfileContinuation continuation, ulong effectiveUnwindAllowance);
}

/// <summary>
/// Produces a profile's per-runtime executor.
/// </summary>
/// <remarks>
/// A delegate named directly by a descriptor, so the executor type is rooted for trimming and
/// Native AOT by an ordinary reference rather than by a rooting descriptor or a linker annotation.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=CC8727
// Broiler-Falsified-If: an executor is created on a path that does not instantiate, or its type is rooted by reflection
// Broiler-Human:        PENDING
public delegate IVmProfileExecutor VmExecutorFactory(IVmExecutionEnvironment environment);
