namespace Broiler.VM;

/// <summary>
/// The core-owned, profile-neutral reason code every operation result carries beside its outcome
/// category.
/// </summary>
/// <remarks>
/// <para>
/// Reason codes are what keeps the ten-member category set closed. Adding a member inside an
/// existing category is additive and bumps only <see cref="VmReasonRegistry.Revision"/>, never
/// <see cref="VmCoreContract.Version"/>; adding a category is a numbered amendment. A profile may
/// not define or contribute a reason - a language outcome rides in a typed payload, not in this
/// enum, which is the rule that stops the core learning one language at a time.
/// </para>
/// <para>
/// <strong>Numbering is structural.</strong> A reason's hundreds digit is its
/// <see cref="VmOutcome"/>, so the category mapping is derivable rather than restated, and a member
/// filed under the wrong category is a visible number rather than a table entry someone has to
/// notice. The 1000 block is control-only: those reasons are carried by
/// <see cref="VmControlResult"/> and belong to no stage outcome.
/// </para>
/// <para>
/// Catalog-construction failures are deliberately <em>not</em> here. They are
/// <see cref="VmCatalogValidationReason"/> on a thrown exception, because composition is the one
/// place the core throws rather than returning. One condition, one surface: no test may reach a
/// catalog reason from a load-path API or a load-path reason from a catalog API.
/// </para>
/// </remarks>
public enum VmReason
{
    /// <summary>Reserved. It exists so that <c>default</c> cannot be read as a real reason.</summary>
    None = 0,

    // ---- Normal (100) ------------------------------------------------------------------------

    /// <summary>The stage completed normally.</summary>
    NormalCompleted = 100,

    // ---- UnsupportedProfile (200) ------------------------------------------------------------

    /// <summary>Generic unsupported-profile reason.</summary>
    UnsupportedProfileUnspecified = 200,

    /// <summary>
    /// The requested profile identity is well formed and the catalog does not contain it. The
    /// result names the requested ID; the catalog's contents are reachable only from the catalog.
    /// </summary>
    ProfileNotInCatalog = 201,

    // ---- InvalidArtifact (300) ---------------------------------------------------------------

    /// <summary>Generic invalid-artifact reason.</summary>
    InvalidArtifactUnspecified = 300,

    /// <summary>The artifact descriptor itself is malformed - an empty identity, an absent manifest.</summary>
    MalformedArtifactDescriptor = 301,

    /// <summary>The declared profile-format version lies outside the profile's supported range.</summary>
    UnsupportedProfileFormatVersion = 302,

    /// <summary>The declared feature manifest is not in the profile's accepted set.</summary>
    UnsupportedFeatureManifest = 303,

    /// <summary>The payload uses a feature the accepted manifest does not cover.</summary>
    UnknownFeature = 304,

    /// <summary>The payload ended before a declared structure completed.</summary>
    Truncated = 305,

    /// <summary>A variable-length or framed encoding is not well formed.</summary>
    MalformedEncoding = 306,

    /// <summary>The payload declares a format version the profile does not know.</summary>
    UnknownFormatVersion = 307,

    /// <summary>The payload is internally inconsistent: an index, offset or length disagrees with its frame.</summary>
    InconsistentStructure = 308,

    /// <summary>The payload decoded but failed the profile's semantic validation.</summary>
    SemanticValidationFailed = 309,

    /// <summary>The payload contradicts the descriptor that introduced it.</summary>
    DescriptorMismatch = 310,

    /// <summary>The artifact assumes a host signature the composition does not provide.</summary>
    UnsatisfiedHostAssumption = 311,

    // ---- InvalidState (400) ------------------------------------------------------------------

    /// <summary>Generic invalid-state reason.</summary>
    InvalidStateUnspecified = 400,

    /// <summary>The target object has been disposed.</summary>
    ObjectDisposed = 401,

    /// <summary>The target object is disposing and accepts no new work.</summary>
    ObjectDisposing = 402,

    /// <summary>The target reached a terminal faulted state and accepts only disposal.</summary>
    TerminalFault = 403,

    /// <summary>The call is not legal from the object's current state.</summary>
    WrongState = 404,

    /// <summary>The operation already completed.</summary>
    AlreadyCompleted = 405,

    /// <summary>A re-entrant public call was refused by a non-reentrant declaration.</summary>
    ReentrancyRefused = 406,

    /// <summary>The call arrived on a thread the declared affinity does not permit.</summary>
    ThreadAffinityViolation = 407,

    /// <summary>The suspension object has already been consumed; it is single-use.</summary>
    ResumeTokenConsumed = 408,

    /// <summary>The verified handle is draining and admits no new lease or use.</summary>
    HandleDraining = 409,

    /// <summary>The verified handle has been disposed.</summary>
    HandleDisposed = 410,

    /// <summary>The object belongs to a different runtime than the one it was presented to.</summary>
    ForeignHandle = 411,

    /// <summary>The payload identity names a profile other than the one that produced the result.</summary>
    ForeignPayload = 412,

    /// <summary>The mediator was used outside the dynamic extent of the invocation that supplied it.</summary>
    MediatorOutOfScope = 413,

    /// <summary>A runtime call was attempted from inside a host capability that may not re-enter it.</summary>
    ReentrantRuntimeCallFromCapability = 414,

    /// <summary>A guest-initiated handle was presented to a runtime other than the one that made it.</summary>
    NestedHandleNotShareable = 415,

    /// <summary>The runtime is already holding its maximum number of live suspended operations.</summary>
    SuspendedOperationLimitReached = 416,

    /// <summary>The profile suspended during instantiation without declaring asynchronous instantiation.</summary>
    UndeclaredAsynchronousInstantiation = 417,

    /// <summary>The aggregate budget has been disposed.</summary>
    AggregateBudgetDisposed = 418,

    /// <summary>The aggregate budget still has live child runtimes and cannot be disposed.</summary>
    AggregateBudgetHasLiveRuntimes = 419,

    /// <summary>Sharing clause 3: the handle and the receiving runtime disagree on the core contract version.</summary>
    SharedHandleCoreContractVersionMismatch = 420,

    /// <summary>Sharing clause 4: the handle and the receiving catalog entry disagree on descriptor revision.</summary>
    SharedHandleDescriptorRevisionMismatch = 421,

    /// <summary>Sharing clause 5: the handle and the receiving profile disagree on profile-format version.</summary>
    SharedHandleFormatVersionMismatch = 422,

    /// <summary>Sharing clause 6: the handle and the receiving profile disagree on feature manifest.</summary>
    SharedHandleFeatureManifestMismatch = 423,

    /// <summary>Sharing clause 7: the handle and the receiving profile disagree on verifier semantic version.</summary>
    SharedHandleVerifierVersionMismatch = 424,

    /// <summary>Sharing clause 8: the handle's effective ceilings are not exactly the receiving runtime's.</summary>
    SharedHandleCeilingMismatch = 425,

    /// <summary>Sharing clause 9: the handle's host-signature assumptions are not the receiving runtime's bindings.</summary>
    SharedHandleCapabilityAssumptionMismatch = 426,

    /// <summary>Sharing clause 10: the profile declared the representation runtime-scoped.</summary>
    SharedHandleNotShareable = 427,

    /// <summary>Sharing clause 11: the two runtimes do not share an aggregate budget parent.</summary>
    SharedHandleAggregateBudgetMismatch = 428,

    // ---- ProfileFault (500) ------------------------------------------------------------------

    /// <summary>Generic profile-fault reason. A language-defined fault rides here with a typed payload.</summary>
    ProfileFaultUnspecified = 500,

    /// <summary>The profile violated the contract it declared - an illegal step, an out-of-range payload kind.</summary>
    ProfileContractViolation = 501,

    /// <summary>The profile performed more work between two polls than its declared bound permits.</summary>
    CancellationPollBoundExceeded = 502,

    /// <summary>The executor a factory produced reports a different profile identity than its descriptor.</summary>
    ExecutorIdentityMismatch = 503,

    /// <summary>The profile tried to swallow a terminal nested outcome instead of converting it.</summary>
    NestedFailureNotConverted = 504,

    // ---- Suspension (600) --------------------------------------------------------------------

    /// <summary>Generic suspension reason.</summary>
    SuspensionUnspecified = 600,

    /// <summary>The guest suspended on its own terms.</summary>
    GuestSuspended = 601,

    /// <summary>The host requested an external suspension and the operation parked at a polling point.</summary>
    ExternallySuspended = 602,

    /// <summary>Instantiation suspended, which requires a declaring descriptor.</summary>
    InstantiationSuspended = 603,

    // ---- Cancellation (700) ------------------------------------------------------------------

    /// <summary>Generic cancellation reason.</summary>
    CancellationUnspecified = 700,

    /// <summary>A cancellation request was observed at a declared polling point.</summary>
    Cancelled = 701,

    /// <summary>The terminal unwind did not complete inside its bounded allowance.</summary>
    UnwindTimedOut = 702,

    /// <summary>A suspended operation exceeded the runtime's maximum suspended residency.</summary>
    SuspendedResidencyExpired = 703,

    /// <summary>
    /// A control handle holding an untaken external suspension was disposed, so the operation is
    /// latched cancelled rather than left parked forever.
    /// </summary>
    ExternalSuspensionAbandoned = 704,

    // ---- ResourceExhaustion (800) ------------------------------------------------------------

    /// <summary>Generic resource-exhaustion reason. The dimension and scope are on the diagnostics.</summary>
    ResourceExhaustionUnspecified = 800,

    /// <summary>The shared aggregate parent had no remaining allowance.</summary>
    ParentExhausted = 801,

    /// <summary>The parent is already at its live-runtime ceiling.</summary>
    LiveRuntimeCeilingReached = 802,

    /// <summary>A requested runtime ceiling exceeds the parent's remaining allowance.</summary>
    ExceedsParentRemaining = 803,

    /// <summary>An allowance for one named dimension in one named scope was spent.</summary>
    AllowanceExhausted = 804,

    /// <summary>A ceiling for one named dimension in one named scope was reached.</summary>
    CeilingReached = 805,

    // ---- HostFailure (900) -------------------------------------------------------------------

    /// <summary>Generic host-failure reason.</summary>
    HostFailureUnspecified = 900,

    /// <summary>A host capability delegate threw, and its declared translation made that observable.</summary>
    HostCapabilityFaulted = 901,

    /// <summary>The profile imported a capability this runtime did not register. Registration is the permission.</summary>
    CapabilityNotRegistered = 902,

    /// <summary>The registered capability and the declared import disagree on signature identity.</summary>
    CapabilitySignatureMismatch = 903,

    /// <summary>An opaque reference minted by another runtime was presented to this one.</summary>
    ForeignOpaqueRef = 904,

    /// <summary>An opaque reference whose generation has been invalidated was presented.</summary>
    StaleOpaqueRef = 905,

    /// <summary>
    /// The composition registered no artifact provider, so every guest-initiated load is refused.
    /// Registering none is the content policy, expressed as a contract outcome.
    /// </summary>
    ProviderNotRegistered = 906,

    /// <summary>More than one artifact provider was registered into one runtime.</summary>
    DuplicateArtifactProvider = 907,

    /// <summary>The provider answered with a typed refusal.</summary>
    ProviderRefused = 908,

    /// <summary>The provider answered that it has no artifact for the request.</summary>
    ProviderArtifactNotFound = 909,

    /// <summary>The provider violated its own contract - an answer shape the kind does not permit.</summary>
    ProviderContractViolation = 910,

    /// <summary>The provider returned a descriptor naming a profile other than the requesting one.</summary>
    ProviderProfileMismatch = 911,

    /// <summary>A provider is registered but the runtime carries no guest-load bounds.</summary>
    GuestLoadBoundsNotConfigured = 912,

    /// <summary>A configured guest-load bound is looser than the profile's declared maximum.</summary>
    GuestLoadBoundExceedsProfileMaximum = 913,

    /// <summary>A runtime-creation ceiling entry could not be resolved for some dimension.</summary>
    BudgetDimensionUnresolved = 914,

    /// <summary>A runtime-creation ceiling would raise a bound rather than tighten it.</summary>
    BudgetRaiseRefused = 915,

    /// <summary>A dimension was given a runtime-scope entry its scope table does not permit.</summary>
    BudgetDimensionNotRuntimeScoped = 916,

    /// <summary>The runtime options carry no finite maximum suspended residency.</summary>
    SuspendedResidencyUnbounded = 917,

    /// <summary>The runtime options carry no finite maximum live-suspended-operation count.</summary>
    SuspendedOperationLimitUnbounded = 918,

    // ---- Control-only (1000) -----------------------------------------------------------------

    /// <summary>
    /// External suspension was requested of an operation whose profile descriptor does not declare
    /// it. A missing capability is not an illegal transition, so this is an unsupported control
    /// result rather than an invalid state.
    /// </summary>
    ExternalSuspensionNotDeclared = 1000,

    /// <summary>
    /// External suspension was requested of a runtime that did not enable it. The double gate is
    /// deliberate: the profile declares, and the composition enables.
    /// </summary>
    ExternalSuspensionNotEnabled = 1001,
}

/// <summary>
/// The closed per-category reason registry, and the monotonic revision published beside the core
/// contract version.
/// </summary>
/// <remarks>
/// The revision is deliberately its own number. Wiring it to <see cref="VmCoreContract.Version"/>
/// would make an additive reason look like a contract amendment and a contract amendment look like
/// a new reason, which defeats the purpose of having two numbers.
/// </remarks>
public static class VmReasonRegistry
{
    /// <summary>
    /// The reason-registry revision. It increases when a reason is added inside an existing
    /// category; it is not the core contract version and must never be wired to it.
    /// </summary>
    public const int Revision = 1;

    /// <summary>
    /// The outcome category <paramref name="reason"/> belongs to, or <see cref="VmOutcome.None"/>
    /// for <see cref="VmReason.None"/> and for the control-only block.
    /// </summary>
    public static VmOutcome CategoryOf(VmReason reason)
    {
        var block = (int)reason / 100;

        return block is >= 1 and <= 9 ? (VmOutcome)block : VmOutcome.None;
    }

    /// <summary>
    /// Whether <paramref name="reason"/> is carried by <see cref="VmControlResult"/> rather than by
    /// a stage outcome.
    /// </summary>
    public static bool IsControlOnly(VmReason reason) => (int)reason >= 1000;

    /// <summary>Whether <paramref name="reason"/> may accompany <paramref name="outcome"/>.</summary>
    public static bool IsLegal(VmOutcome outcome, VmReason reason) =>
        outcome is not VmOutcome.None && CategoryOf(reason) == outcome;

    /// <summary>The generic reason for <paramref name="outcome"/>. Every category has one.</summary>
    public static VmReason GenericFor(VmOutcome outcome) =>
        outcome is VmOutcome.None ? VmReason.None : (VmReason)((int)outcome * 100);

    /// <summary>
    /// Every registered reason, in ascending numeric order.
    /// </summary>
    /// <remarks>
    /// The generic overload is used deliberately: the non-generic
    /// <c>Enum.GetValues(Type)</c> is a reflection call that trimming and Native AOT cannot see
    /// through, and rule B5 exists to keep that shape out of the product graph.
    /// </remarks>
    public static VmReason[] All() => System.Enum.GetValues<VmReason>();
}
