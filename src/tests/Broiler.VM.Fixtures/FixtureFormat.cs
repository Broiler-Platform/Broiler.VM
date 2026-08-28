namespace Broiler.VM.Fixtures;

/// <summary>
/// The fixture profile's bytecode format.
/// </summary>
/// <remarks>
/// <para>
/// The fixture is deliberately shaped after a non-trivial existing runtime rather than after the
/// contract it proves: a magic number, a format version, length-framed sections, a constant pool, a
/// code section, and a stack machine with a real dispatch loop. A contract-shaped toy - one method
/// that returns a value - would let every core obligation be satisfied by a fixture that could not
/// exercise any of them, which is exactly the risk the roadmap names as "a core designed with no
/// real profile fits no real profile".
/// </para>
/// <para>
/// Nothing here is a core concept. The opcodes, the value model, the fault model and the section
/// kinds are the fixture profile's own, and the core neither names nor interprets any of them.
/// </para>
/// </remarks>
public static class FixtureFormat
{
    /// <summary>The four magic bytes every fixture artifact starts with.</summary>
    public static System.ReadOnlySpan<byte> Magic => "BVMF"u8;

    /// <summary>The only profile-format version the fixture verifier accepts.</summary>
    public const uint FormatVersion = 1;

    /// <summary>The constant-pool section.</summary>
    public const byte SectionConstants = 1;

    /// <summary>The code section.</summary>
    public const byte SectionCode = 2;

    /// <summary>Do nothing. Charges the profile's granularity like every other instruction.</summary>
    public const byte OpNop = 0x00;

    /// <summary>Push constant <c>n</c> onto the stack.</summary>
    public const byte OpPushConst = 0x01;

    /// <summary>Pop two, push their sum.</summary>
    public const byte OpAdd = 0x02;

    /// <summary>Pop two, push their difference.</summary>
    public const byte OpSub = 0x03;

    /// <summary>Pop two, push their product.</summary>
    public const byte OpMul = 0x04;

    /// <summary>Pop one, invoke host capability binding <c>n</c>, push the result.</summary>
    public const byte OpHostCall = 0x05;

    /// <summary>Suspend, resumable at the following instruction.</summary>
    public const byte OpYield = 0x06;

    /// <summary>Produce a language-defined fault carrying constant <c>n</c> as its code.</summary>
    public const byte OpFault = 0x07;

    /// <summary>Request a guest-initiated load, using constant <c>n</c> as the opaque specifier.</summary>
    public const byte OpLoad = 0x08;

    /// <summary>Burn <c>n</c> work units without polling. Used by the poll-bound-breaking variant.</summary>
    public const byte OpSpin = 0x09;

    /// <summary>Complete, returning the top of the stack.</summary>
    public const byte OpReturn = 0x0A;

    /// <summary>Allocate a buffer of constant <c>n</c> elements through the bounded allocator.</summary>
    public const byte OpAllocate = 0x0B;

    /// <summary>Retain constant <c>n</c> bytes against the live-bytes ceiling.</summary>
    public const byte OpRetain = 0x0C;

    /// <summary>Release constant <c>n</c> bytes previously retained.</summary>
    public const byte OpRelease = 0x0D;
}
