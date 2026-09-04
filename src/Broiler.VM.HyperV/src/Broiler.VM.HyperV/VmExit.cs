// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System.Globalization;

namespace Broiler.VM.HyperV;

/// <summary>Why a run of a pico VM stopped.</summary>
public enum VmExitReason
{
    /// <summary>The guest executed HLT. This is how a well behaved pico program finishes.</summary>
    Halted,

    /// <summary>
    /// The guest touched a guest physical address with neither RAM nor a device behind it.
    /// </summary>
    MemoryFault,

    /// <summary>The guest touched an I/O port with no device behind it.</summary>
    UnhandledPort,

    /// <summary>
    /// The instruction emulator could not replay a device access, usually because the
    /// instruction is not one it models.
    /// </summary>
    EmulationFailed,

    /// <summary>
    /// The guest triple faulted or otherwise wedged itself. With no IDT loaded this is the
    /// usual outcome of a bad instruction or a wild jump.
    /// </summary>
    UnrecoverableException,

    /// <summary>The processor was left in a state the hardware refuses to enter.</summary>
    InvalidRegisterValue,

    /// <summary>The guest used a feature the partition does not provide.</summary>
    UnsupportedFeature,

    /// <summary>The run was cancelled by the host, by a cancellation token or a timeout.</summary>
    Canceled,

    /// <summary>The run serviced as many exits as the configured limit allowed.</summary>
    ExitLimitReached,

    /// <summary>The hypervisor reported an exit this component does not service.</summary>
    Unexpected,
}

/// <summary>How the guest was touching memory when it faulted.</summary>
public enum MemoryAccessKind
{
    /// <summary>The guest was loading from the address.</summary>
    Read,

    /// <summary>The guest was storing to the address.</summary>
    Write,

    /// <summary>The guest was fetching instructions from the address.</summary>
    Execute,
}

/// <summary>
/// The outcome of a run: why it stopped, and where the guest was when it did.
/// </summary>
public sealed class VmExit
{
    internal VmExit(
        VmExitReason reason,
        ulong rip,
        ulong rflags,
        int instructionLength,
        int exitCount,
        string message,
        ulong? faultAddress = null,
        MemoryAccessKind? faultAccess = null,
        ushort? port = null)
    {
        Reason = reason;
        Rip = rip;
        Rflags = rflags;
        InstructionLength = instructionLength;
        ExitCount = exitCount;
        Message = message;
        FaultAddress = faultAddress;
        FaultAccess = faultAccess;
        Port = port;
    }

    /// <summary>Why the run stopped.</summary>
    public VmExitReason Reason { get; }

    /// <summary>
    /// RIP at the exit. For a halt this is the address after the HLT; for a fault it is the
    /// address of the instruction that faulted.
    /// </summary>
    public ulong Rip { get; }

    /// <summary>RFLAGS at the exit.</summary>
    public ulong Rflags { get; }

    /// <summary>The length in bytes of the instruction that caused the exit, when the hypervisor reported one.</summary>
    public int InstructionLength { get; }

    /// <summary>How many VM exits the run serviced, including this one.</summary>
    public int ExitCount { get; }

    /// <summary>The guest physical address that faulted, for <see cref="VmExitReason.MemoryFault"/>.</summary>
    public ulong? FaultAddress { get; }

    /// <summary>How the guest was touching <see cref="FaultAddress"/>.</summary>
    public MemoryAccessKind? FaultAccess { get; }

    /// <summary>The port involved, for <see cref="VmExitReason.UnhandledPort"/>.</summary>
    public ushort? Port { get; }

    /// <summary>A one line explanation suitable for logs and exception messages.</summary>
    public string Message { get; }

    /// <summary>Whether the guest stopped the way a finished pico program is expected to.</summary>
    public bool IsHalt => Reason == VmExitReason.Halted;

    /// <summary>Returns <see cref="Message"/>.</summary>
    /// <returns>The exit message.</returns>
    public override string ToString() => string.Format(
        CultureInfo.InvariantCulture,
        "{0} at rip=0x{1:x} after {2} exit(s): {3}",
        Reason,
        Rip,
        ExitCount,
        Message);
}
