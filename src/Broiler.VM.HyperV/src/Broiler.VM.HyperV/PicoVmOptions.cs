// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using System.Globalization;

namespace Broiler.VM.HyperV;

/// <summary>
/// The shape of a pico VM: how much RAM it has, which processor mode it starts in, and where
/// its code, stack and page tables live in guest physical memory.
/// </summary>
/// <remarks>
/// Every address here is a guest physical address. The defaults describe a 4 MiB long mode
/// machine with code at 64 KiB, its stack at the top of RAM, and an identity map built from
/// three pages starting at 4 KiB.
/// </remarks>
public sealed class PicoVmOptions
{
    /// <summary>The size of a guest page, and the granularity every mapping is aligned to.</summary>
    public const ulong PageSize = 4096;

    /// <summary>The smallest amount of RAM a pico VM can be given.</summary>
    public const ulong MinimumMemorySize = 64 * 1024;

    private ulong memorySize = 4 * 1024 * 1024;
    private ulong pageTableAddress = 0x1000;
    private string name = "pico";

    /// <summary>
    /// Gets or sets a name used in diagnostics and exception messages. Defaults to "pico".
    /// </summary>
    public string Name
    {
        get => name;
        set => name = string.IsNullOrWhiteSpace(value) ? "pico" : value;
    }

    /// <summary>
    /// Gets or sets the amount of guest RAM, mapped as one region at guest physical address 0.
    /// Must be a multiple of <see cref="PageSize"/> and at least <see cref="MinimumMemorySize"/>.
    /// Defaults to 4 MiB.
    /// </summary>
    public ulong MemorySize
    {
        get => memorySize;
        set => memorySize = value;
    }

    /// <summary>Gets or sets the processor mode the guest starts in. Defaults to <see cref="GuestMode.Long64"/>.</summary>
    public GuestMode Mode { get; set; } = GuestMode.Long64;

    /// <summary>
    /// Gets or sets the guest physical address code is loaded at and execution starts from.
    /// Defaults to 0x1000 in real mode and 0x10000 in the other modes.
    /// </summary>
    public ulong? CodeAddress { get; set; }

    /// <summary>
    /// Gets or sets the initial stack pointer. Defaults to 0xFFF0 in real mode and to the top
    /// of RAM in the other modes.
    /// </summary>
    public ulong? StackPointer { get; set; }

    /// <summary>
    /// Gets or sets the guest physical address of the identity-mapping page table, which
    /// occupies three pages (PML4, PDPT, PD). Long mode only. Defaults to 0x1000.
    /// </summary>
    public ulong PageTableAddress
    {
        get => pageTableAddress;
        set => pageTableAddress = value;
    }

    /// <summary>
    /// Gets or sets the access the guest is granted to its RAM. Defaults to
    /// <see cref="GuestMemoryAccess.ReadWriteExecute"/>.
    /// </summary>
    public GuestMemoryAccess MemoryAccess { get; set; } = GuestMemoryAccess.ReadWriteExecute;

    /// <summary>
    /// Gets or sets the number of VM exits a single run may service before it gives up, or 0
    /// for no limit. Guards against a guest that spins on a device rather than halting.
    /// Defaults to 0.
    /// </summary>
    public int ExitLimit { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether an unhandled access to unmapped guest memory
    /// stops the run. When <see langword="false"/> such accesses read back zero and writes are
    /// discarded, which is how real hardware behaves for an absent device. Defaults to
    /// <see langword="true"/>.
    /// </summary>
    public bool StopOnUnmappedMemory { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether an access to an unclaimed I/O port stops the
    /// run. When <see langword="false"/> reads return all-ones and writes are discarded, which
    /// is how a PC behaves for an absent device. Defaults to <see langword="false"/>.
    /// </summary>
    public bool StopOnUnhandledPort { get; set; }

    /// <summary>The effective code address for this configuration.</summary>
    internal ulong EffectiveCodeAddress =>
        CodeAddress ?? (Mode == GuestMode.Real16 ? 0x1000UL : 0x10000UL);

    /// <summary>The effective initial stack pointer for this configuration.</summary>
    internal ulong EffectiveStackPointer =>
        StackPointer ?? (Mode == GuestMode.Real16 ? 0xFFF0UL : MemorySize);

    /// <summary>Validates the configuration, throwing when it cannot describe a runnable machine.</summary>
    /// <exception cref="ArgumentException">The configuration is inconsistent.</exception>
    internal void Validate()
    {
        if (memorySize < MinimumMemorySize)
        {
            throw Invalid(
                "MemorySize is {0} bytes; the minimum is {1} bytes.",
                memorySize,
                MinimumMemorySize);
        }

        if (memorySize % PageSize != 0)
        {
            throw Invalid("MemorySize ({0} bytes) must be a multiple of the {1} byte page size.", memorySize, PageSize);
        }

        if (!Enum.IsDefined(Mode))
        {
            throw Invalid("Mode {0} is not a known guest mode.", (int)Mode);
        }

        ulong code = EffectiveCodeAddress;
        if (code >= memorySize)
        {
            throw Invalid("CodeAddress 0x{0:X} lies outside the {1} bytes of guest RAM.", code, memorySize);
        }

        ulong stack = EffectiveStackPointer;
        if (stack > memorySize)
        {
            throw Invalid("StackPointer 0x{0:X} lies outside the {1} bytes of guest RAM.", stack, memorySize);
        }

        if (Mode == GuestMode.Real16)
        {
            if (code % 16 != 0)
            {
                throw Invalid("CodeAddress 0x{0:X} must be 16-byte aligned in real mode, where it becomes the CS base.", code);
            }

            if (code >= 0x100000)
            {
                throw Invalid("CodeAddress 0x{0:X} is above the 1 MiB real mode address space.", code);
            }

            if (stack > 0x10000)
            {
                throw Invalid("StackPointer 0x{0:X} does not fit in a 16-bit real mode stack segment.", stack);
            }
        }

        if (Mode == GuestMode.Long64)
        {
            if (pageTableAddress % PageSize != 0)
            {
                throw Invalid("PageTableAddress 0x{0:X} must be page aligned.", pageTableAddress);
            }

            ulong pageTableEnd = pageTableAddress + (3 * PageSize);
            if (pageTableEnd > memorySize)
            {
                throw Invalid(
                    "The page table at 0x{0:X} needs three pages and would run past the {1} bytes of guest RAM.",
                    pageTableAddress,
                    memorySize);
            }

            if (code < pageTableEnd && code + PageSize > pageTableAddress)
            {
                throw Invalid(
                    "CodeAddress 0x{0:X} overlaps the page table at 0x{1:X}.",
                    code,
                    pageTableAddress);
            }
        }

        if (ExitLimit < 0)
        {
            throw Invalid("ExitLimit {0} cannot be negative.", ExitLimit);
        }
    }

    /// <summary>Takes a snapshot so a running VM is unaffected by later edits to the options object.</summary>
    internal PicoVmOptions Clone() => new()
    {
        Name = name,
        MemorySize = memorySize,
        Mode = Mode,
        CodeAddress = CodeAddress,
        StackPointer = StackPointer,
        PageTableAddress = pageTableAddress,
        MemoryAccess = MemoryAccess,
        ExitLimit = ExitLimit,
        StopOnUnmappedMemory = StopOnUnmappedMemory,
        StopOnUnhandledPort = StopOnUnhandledPort,
    };

    private static ArgumentException Invalid(string format, params object[] args) =>
        new(string.Format(CultureInfo.InvariantCulture, format, args), nameof(PicoVmOptions));
}
