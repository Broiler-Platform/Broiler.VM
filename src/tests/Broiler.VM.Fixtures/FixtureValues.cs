namespace Broiler.VM.Fixtures;

/// <summary>
/// Which payload kind IDs each fixture profile owns.
/// </summary>
/// <remarks>
/// The two fixture profiles carry disjoint ranges, and the core rejects a payload whose kind falls
/// outside the range its profile declared. That is what makes a foreign payload demonstrable: the
/// second profile cannot stamp a payload the first profile would be allowed to return.
/// </remarks>
public static class FixtureKinds
{
    /// <summary>The first kind ID the second fixture profile owns.</summary>
    public const int BetaBase = 100;

    /// <summary>The value kind for <paramref name="profileId"/>.</summary>
    public static int Value(VmProfileId profileId) => Base(profileId) + 1;

    /// <summary>The fault kind for <paramref name="profileId"/>.</summary>
    public static int Fault(VmProfileId profileId) => Base(profileId) + 2;

    /// <summary>The suspension-projection kind for <paramref name="profileId"/>.</summary>
    public static int SuspensionProjection(VmProfileId profileId) => Base(profileId) + 3;

    private static int Base(VmProfileId profileId) =>
        profileId.Equals(SecondFixtureVmProfile.Id) ? BetaBase - 1 : 0;
}

/// <summary>
/// The fixture profile's value: the typed payload a normal invocation returns.
/// </summary>
/// <remarks>
/// The core never names this type, never calls a member on it, and never inspects anything about it
/// except the identity every payload carries. A consumer gets the concrete type back through the
/// profile's own static accessor, which is the projection shape the contract specifies.
/// </remarks>
public sealed class FixtureValue : IVmProfilePayload
{
    /// <summary>Creates a value payload stamped with its profile's own value kind.</summary>
    public FixtureValue(VmProfileId profileId, long value)
    {
        Identity = new VmPayloadIdentity(profileId, FixtureKinds.Value(profileId), 1);
        Value = value;
    }

    /// <inheritdoc/>
    public VmPayloadIdentity Identity { get; }

    /// <summary>The number the fixture machine left on top of its stack.</summary>
    public long Value { get; }
}

/// <summary>The fixture profile's language-defined fault.</summary>
/// <remarks>
/// It rides behind the profile-neutral fault category as a typed payload, so a fixture fault reaches
/// a caller in full without the core acquiring a case for it.
/// </remarks>
public sealed class FixtureFault : IVmProfilePayload
{
    /// <summary>Creates a fault payload stamped with its profile's own fault kind.</summary>
    public FixtureFault(VmProfileId profileId, long code, string description)
    {
        Identity = new VmPayloadIdentity(profileId, FixtureKinds.Fault(profileId), 1);
        Code = code;
        Description = description;
    }

    /// <inheritdoc/>
    public VmPayloadIdentity Identity { get; }

    /// <summary>The fixture-defined fault code. The core attaches no meaning to it.</summary>
    public long Code { get; }

    /// <summary>What the fixture calls this fault.</summary>
    public string Description { get; }
}

/// <summary>What the fixture exposes about a suspended operation.</summary>
/// <remarks>
/// A projection, not the continuation. What a paused profile exposes is the profile's own surface,
/// and this is the fixture exercising that: the core carries it and never looks inside.
/// </remarks>
public sealed class FixtureSuspensionProjection : IVmProfilePayload
{
    /// <summary>Creates a projection stamped with its profile's own projection kind.</summary>
    public FixtureSuspensionProjection(VmProfileId profileId, int instructionPointer, int stackDepth)
    {
        Identity = new VmPayloadIdentity(profileId, FixtureKinds.SuspensionProjection(profileId), 1);
        InstructionPointer = instructionPointer;
        StackDepth = stackDepth;
    }

    /// <inheritdoc/>
    public VmPayloadIdentity Identity { get; }

    /// <summary>Where the machine parked.</summary>
    public int InstructionPointer { get; }

    /// <summary>How deep its stack was.</summary>
    public int StackDepth { get; }
}

/// <summary>The fixture profile's immutable decoded artifact.</summary>
/// <remarks>
/// Everything reachable from it is immutable once verification returns, which is what makes a
/// shareable handle safe for unsynchronised concurrent readers in two runtimes at once.
/// </remarks>
public sealed class FixtureVerifiedState : IVmVerifiedState
{
    internal FixtureVerifiedState(long[] constants, byte[] code)
    {
        Constants = constants;
        Code = code;
    }

    internal long[] Constants { get; }

    internal byte[] Code { get; }

    /// <summary>How many constants the artifact declared.</summary>
    public int ConstantCount => Constants.Length;

    /// <summary>How many bytes of code the artifact declared.</summary>
    public int CodeLength => Code.Length;
}

/// <summary>The fixture profile's mutable per-instance state. The core never inspects it.</summary>
public sealed class FixtureInstanceState : IVmInstanceState
{
    internal FixtureInstanceState(FixtureVerifiedState verified) => Verified = verified;

    internal FixtureVerifiedState Verified { get; }

    /// <summary>How many times this instance has been invoked.</summary>
    public int InvocationCount { get; internal set; }

    /// <summary>How many bytes the instance currently reports retained.</summary>
    public ulong RetainedBytes { get; internal set; }
}

/// <summary>The fixture profile's captured continuation.</summary>
/// <remarks>
/// Parking by capturing an instruction pointer and a stack is what makes suspension demonstrable
/// against a real machine rather than against a flag.
/// </remarks>
public sealed class FixtureContinuation : IVmProfileContinuation
{
    internal FixtureContinuation(int instructionPointer, long[] stack, int stackDepth)
    {
        InstructionPointer = instructionPointer;
        Stack = stack;
        StackDepth = stackDepth;
    }

    internal int InstructionPointer { get; }

    internal long[] Stack { get; }

    internal int StackDepth { get; }

    /// <summary>Whether the profile's terminal-unwind entry point has run for this continuation.</summary>
    public bool Unwound { get; internal set; }
}
