namespace Broiler.VM;

/// <summary>Where a runtime ceiling's value comes from.</summary>
public enum VmCeilingSource
{
    /// <summary>The host stated a number.</summary>
    Explicit = 0,

    /// <summary>
    /// The host explicitly adopts whatever the profile declared as its default. Adoption is
    /// explicit precisely so that it is a decision rather than a silence.
    /// </summary>
    AdoptProfileDefault = 1,

    /// <summary>The host adopts whatever the parent aggregate budget still has.</summary>
    AdoptParentRemaining = 2,
}

/// <summary>Whether an allowance is shared across a scope or refreshed per operation.</summary>
public enum VmAllowanceMode
{
    /// <summary>One monotonically decreasing counter shared by every operation in the scope.</summary>
    Pooled = 0,

    /// <summary>A fresh allowance per operation.</summary>
    Replenishing = 1,
}

/// <summary>Whether a runtime permits the host to suspend an operation from outside.</summary>
/// <remarks>
/// Disabled by default, and the second half of the double gate: the profile must declare external
/// suspension <em>and</em> the composition must enable it. Either alone answers unsupported, and the
/// reason says which, so a host can tell "this profile cannot" from "I did not turn it on".
/// </remarks>
public enum VmExternalSuspensionMode
{
    /// <summary>The host may not suspend an operation from outside.</summary>
    Disabled = 0,

    /// <summary>The host may, where the profile also declares it.</summary>
    Enabled = 1,
}

/// <summary>One runtime ceiling entry: a dimension and where its value comes from.</summary>
public readonly struct VmCeilingSpec : System.IEquatable<VmCeilingSpec>
{
    private VmCeilingSpec(
        VmBudgetDimension dimension,
        VmCeilingSource source,
        ulong explicitValue,
        VmAllowanceMode allowanceMode)
    {
        Dimension = dimension;
        Source = source;
        ExplicitValue = explicitValue;
        AllowanceMode = allowanceMode;
    }

    /// <summary>An explicit ceiling.</summary>
    public static VmCeilingSpec Value(
        VmBudgetDimension dimension,
        ulong value,
        VmAllowanceMode mode = VmAllowanceMode.Pooled) =>
        new(dimension, VmCeilingSource.Explicit, value, mode);

    /// <summary>Adopt the profile's declared default for this dimension.</summary>
    public static VmCeilingSpec AdoptProfileDefault(
        VmBudgetDimension dimension,
        VmAllowanceMode mode = VmAllowanceMode.Pooled) =>
        new(dimension, VmCeilingSource.AdoptProfileDefault, 0, mode);

    /// <summary>Adopt whatever the parent aggregate budget still has for this dimension.</summary>
    public static VmCeilingSpec AdoptParentRemaining(
        VmBudgetDimension dimension,
        VmAllowanceMode mode = VmAllowanceMode.Pooled) =>
        new(dimension, VmCeilingSource.AdoptParentRemaining, 0, mode);

    /// <summary>Which dimension this entry is for.</summary>
    public VmBudgetDimension Dimension { get; }

    /// <summary>Where its value comes from.</summary>
    public VmCeilingSource Source { get; }

    /// <summary>The stated value, meaningful only for an explicit entry.</summary>
    public ulong ExplicitValue { get; }

    /// <summary>Whether the allowance is pooled or replenishing.</summary>
    public VmAllowanceMode AllowanceMode { get; }

    /// <inheritdoc/>
    public bool Equals(VmCeilingSpec other) =>
        Dimension == other.Dimension && Source == other.Source &&
        ExplicitValue == other.ExplicitValue && AllowanceMode == other.AllowanceMode;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VmCeilingSpec other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        System.HashCode.Combine((int)Dimension, (int)Source, ExplicitValue, (int)AllowanceMode);

    /// <summary>Value equality.</summary>
    public static bool operator ==(VmCeilingSpec left, VmCeilingSpec right) => left.Equals(right);

    /// <summary>Value inequality.</summary>
    public static bool operator !=(VmCeilingSpec left, VmCeilingSpec right) => !left.Equals(right);
}

/// <summary>The runtime's guest-load bounds: all four adopted from the profile, or all four stated.</summary>
/// <remarks>
/// All-four-or-none by construction, so a half-configured quadruple is unrepresentable rather than
/// caught by a check that could be forgotten.
/// </remarks>
public readonly struct VmGuestLoadBoundsSpec
{
    private VmGuestLoadBoundsSpec(bool adopts, VmGuestLoadBounds bounds)
    {
        AdoptsProfileMaxima = adopts;
        Bounds = bounds;
    }

    /// <summary>Adopt the profile's declared maxima for all four bounds.</summary>
    public static VmGuestLoadBoundsSpec AdoptProfileMaxima { get; } = new(true, VmGuestLoadBounds.None);

    /// <summary>State all four bounds. They may only tighten the profile's maxima.</summary>
    public static VmGuestLoadBoundsSpec Explicit(VmGuestLoadBounds bounds) => new(false, bounds);

    /// <summary>Whether the profile's maxima are adopted.</summary>
    public bool AdoptsProfileMaxima { get; }

    /// <summary>The stated bounds, meaningful only when not adopting.</summary>
    public VmGuestLoadBounds Bounds { get; }
}

/// <summary>A host capability delegate that takes integers and returns one.</summary>
public delegate VmHostCallOutcome VmHostCapabilityHandler(System.ReadOnlySpan<long> arguments, out long result);

/// <summary>A host capability delegate that takes bytes and returns an opaque reference.</summary>
public delegate VmHostCallOutcome VmHostBytesCapabilityHandler(VmBytes argument, out VmOpaqueRef result);

/// <summary>What a composition root registers into one runtime.</summary>
/// <remarks>
/// Binding happens once, at runtime creation, into an immutable table. There is no post-creation
/// registration, no unregistration, no by-name lookup at call time, no fallback and no default
/// resolution: a capability the composition did not register is not reachable from the runtime at
/// all, which is what makes registration the permission.
/// </remarks>
public readonly struct VmCapabilityRegistration
{
    private VmCapabilityRegistration(
        VmHostCapabilityDescriptor descriptor,
        VmHostCapabilityHandler? handler,
        VmHostBytesCapabilityHandler? bytesHandler,
        IVmArtifactProvider? provider)
    {
        Descriptor = descriptor;
        Handler = handler;
        BytesHandler = bytesHandler;
        Provider = provider;
    }

    /// <summary>Registers a value capability.</summary>
    public static VmCapabilityRegistration Value(
        VmHostCapabilityDescriptor descriptor,
        VmHostCapabilityHandler handler) =>
        new(descriptor, handler, null, null);

    /// <summary>Registers a value capability that answers with an opaque reference.</summary>
    public static VmCapabilityRegistration Value(
        VmHostCapabilityDescriptor descriptor,
        VmHostBytesCapabilityHandler handler) =>
        new(descriptor, null, handler, null);

    /// <summary>Registers the artifact provider. At most one per runtime.</summary>
    public static VmCapabilityRegistration ArtifactProvider(
        VmHostCapabilityDescriptor descriptor,
        IVmArtifactProvider provider) =>
        new(descriptor, null, null, provider);

    /// <summary>The capability shape being registered.</summary>
    public VmHostCapabilityDescriptor Descriptor { get; }

    /// <summary>Which kind of capability this is.</summary>
    public VmCapabilityKind Kind => Descriptor.Kind;

    internal VmHostCapabilityHandler? Handler { get; }

    internal VmHostBytesCapabilityHandler? BytesHandler { get; }

    internal IVmArtifactProvider? Provider { get; }
}

/// <summary>
/// Everything a runtime is created with. Exactly one options object.
/// </summary>
/// <remarks>
/// <para>
/// Every one of the fifteen dimensions must carry an explicit value or an adopt marker.
/// <strong>Omission is not a value and fails runtime creation</strong>: invariant 9 makes resource
/// authority trusted and monotonic, and a dimension a host forgot would otherwise be whatever the
/// core felt like.
/// </para>
/// <para>
/// The two suspended-operation bounds are mandatory and finite, with no representable infinite
/// value. A paused operation holds frames, host handles and an execution slot, so an unbounded
/// residency is an unbounded retention with a debugger attached to it.
/// </para>
/// </remarks>
public sealed class VmRuntimeCreationOptions
{
    /// <summary>Creates an options object.</summary>
    public VmRuntimeCreationOptions(
        VmAggregateBudget? aggregateBudget,
        System.Collections.Immutable.ImmutableArray<VmCeilingSpec> ceilings,
        System.TimeSpan maxSuspendedResidency,
        int maxLiveSuspendedOperations,
        VmGuestLoadBoundsSpec guestLoadBounds,
        VmExternalSuspensionMode externalSuspension,
        System.Collections.Immutable.ImmutableArray<VmCapabilityRegistration> capabilities,
        int maxConcurrentVerifications = 1,
        System.TimeSpan? disposeDrainBudget = null,
        ulong unwindBudget = DefaultUnwindBudget)
    {
        AggregateBudget = aggregateBudget;
        Ceilings = ceilings;
        MaxSuspendedResidency = maxSuspendedResidency;
        MaxLiveSuspendedOperations = maxLiveSuspendedOperations;
        GuestLoadBounds = guestLoadBounds;
        ExternalSuspension = externalSuspension;
        Capabilities = capabilities;
        MaxConcurrentVerifications = maxConcurrentVerifications;
        DisposeDrainBudget = disposeDrainBudget ?? System.TimeSpan.FromSeconds(5);
        UnwindBudget = unwindBudget;
    }

    /// <summary>The bounded default unwind allowance a host may tighten or raise but never unbound.</summary>
    public const ulong DefaultUnwindBudget = 1_000_000;

    /// <summary>The shared parent, or null for an unparented runtime.</summary>
    public VmAggregateBudget? AggregateBudget { get; }

    /// <summary>One entry per dimension. Fifteen entries, no fewer.</summary>
    public System.Collections.Immutable.ImmutableArray<VmCeilingSpec> Ceilings { get; }

    /// <summary>How long a suspended operation may remain parked. Mandatory and finite.</summary>
    public System.TimeSpan MaxSuspendedResidency { get; }

    /// <summary>How many operations may be parked at once. Mandatory and finite.</summary>
    public int MaxLiveSuspendedOperations { get; }

    /// <summary>How many verifications may run at once. Defaults to one.</summary>
    public int MaxConcurrentVerifications { get; }

    /// <summary>How long disposal waits for in-flight work to drain.</summary>
    public System.TimeSpan DisposeDrainBudget { get; }

    /// <summary>The allowance a terminal unwind runs under.</summary>
    public ulong UnwindBudget { get; }

    /// <summary>The nested-load bounds, all four adopted or all four stated.</summary>
    public VmGuestLoadBoundsSpec GuestLoadBounds { get; }

    /// <summary>Whether this runtime enables external suspension.</summary>
    public VmExternalSuspensionMode ExternalSuspension { get; }

    /// <summary>The capabilities this runtime binds.</summary>
    public System.Collections.Immutable.ImmutableArray<VmCapabilityRegistration> Capabilities { get; }
}
