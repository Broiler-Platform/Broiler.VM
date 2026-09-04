// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;

namespace Broiler.VM.HyperV;

/// <summary>
/// The processor mode a pico VM starts its virtual processor in.
/// </summary>
/// <remarks>
/// A fresh virtual processor comes out of the hypervisor in the x86 reset state - 16-bit real
/// mode at F000:FFF0 - so every mode here is reached by writing the architectural registers
/// directly rather than by executing bootstrap code in the guest. That is what makes the
/// machines pico: there is no firmware, no bootloader and no descriptor tables to walk, only
/// the register state the mode requires.
/// </remarks>
public enum GuestMode
{
    /// <summary>
    /// 16-bit real mode. CS is based at the code address and DS, ES and SS are based at zero,
    /// so data addresses in the guest are guest physical addresses.
    /// </summary>
    Real16,

    /// <summary>
    /// 32-bit protected mode with paging off and flat segments, so linear addresses are guest
    /// physical addresses.
    /// </summary>
    Protected32,

    /// <summary>
    /// 64-bit long mode with flat segments and an identity-mapped page table built in guest
    /// RAM, so virtual addresses are guest physical addresses.
    /// </summary>
    Long64,
}

/// <summary>The access a guest is granted to a mapped memory region.</summary>
[Flags]
public enum GuestMemoryAccess
{
    /// <summary>No access. Any touch faults out to the host.</summary>
    None = 0,

    /// <summary>The guest may read the region.</summary>
    Read = 1,

    /// <summary>The guest may write the region.</summary>
    Write = 2,

    /// <summary>The guest may execute from the region.</summary>
    Execute = 4,

    /// <summary>Read and write, but not execute.</summary>
    ReadWrite = Read | Write,

    /// <summary>Read, write and execute: the default for pico VM RAM.</summary>
    ReadWriteExecute = Read | Write | Execute,
}

/// <summary>
/// The x86-64 registers a guest can be inspected and steered through.
/// </summary>
/// <remarks>Values match WHV_REGISTER_NAME, so they cross into the native API unchanged.</remarks>
public enum GuestRegister : uint
{
    /// <summary>The RAX general purpose register.</summary>
    Rax = 0x00000000,

    /// <summary>The RCX general purpose register.</summary>
    Rcx = 0x00000001,

    /// <summary>The RDX general purpose register.</summary>
    Rdx = 0x00000002,

    /// <summary>The RBX general purpose register.</summary>
    Rbx = 0x00000003,

    /// <summary>The stack pointer.</summary>
    Rsp = 0x00000004,

    /// <summary>The frame pointer.</summary>
    Rbp = 0x00000005,

    /// <summary>The RSI general purpose register.</summary>
    Rsi = 0x00000006,

    /// <summary>The RDI general purpose register.</summary>
    Rdi = 0x00000007,

    /// <summary>The R8 general purpose register.</summary>
    R8 = 0x00000008,

    /// <summary>The R9 general purpose register.</summary>
    R9 = 0x00000009,

    /// <summary>The R10 general purpose register.</summary>
    R10 = 0x0000000A,

    /// <summary>The R11 general purpose register.</summary>
    R11 = 0x0000000B,

    /// <summary>The R12 general purpose register.</summary>
    R12 = 0x0000000C,

    /// <summary>The R13 general purpose register.</summary>
    R13 = 0x0000000D,

    /// <summary>The R14 general purpose register.</summary>
    R14 = 0x0000000E,

    /// <summary>The R15 general purpose register.</summary>
    R15 = 0x0000000F,

    /// <summary>The instruction pointer.</summary>
    Rip = 0x00000010,

    /// <summary>The flags register.</summary>
    Rflags = 0x00000011,

    /// <summary>The CR0 control register.</summary>
    Cr0 = 0x0000001C,

    /// <summary>The CR2 page fault address register.</summary>
    Cr2 = 0x0000001D,

    /// <summary>The CR3 page table base register.</summary>
    Cr3 = 0x0000001E,

    /// <summary>The CR4 control register.</summary>
    Cr4 = 0x0000001F,

    /// <summary>The extended feature enable register.</summary>
    Efer = 0x00002001,

    /// <summary>The time stamp counter.</summary>
    Tsc = 0x00002000,
}
