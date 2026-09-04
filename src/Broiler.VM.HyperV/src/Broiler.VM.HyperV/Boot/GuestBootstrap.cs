// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using System.Runtime.Versioning;
using Broiler.VM.HyperV.Interop;

namespace Broiler.VM.HyperV.Boot;

/// <summary>
/// Puts a freshly created virtual processor into the mode a pico VM wants to run in.
/// </summary>
/// <remarks>
/// The hypervisor hands back a processor in the x86 reset state: 16-bit real mode with CS based
/// at 0xFFFF0000 and RIP at 0xFFF0, the address a physical machine finds its firmware at. A pico
/// VM has no firmware, so instead of executing a boot sequence the host writes the end state
/// directly - descriptors, control registers and, for long mode, a page table built in guest RAM.
/// The guest's first instruction is therefore the first instruction of the program under test.
/// </remarks>
[SupportedOSPlatform("windows")]
internal static class GuestBootstrap
{
    /// <summary>Present, S=1, type 0xB (code, execute/read, accessed), 16-bit, byte granular.</summary>
    private const ushort RealModeCode = 0x009B;

    /// <summary>Present, S=1, type 0x3 (data, read/write, accessed), 16-bit, byte granular.</summary>
    private const ushort RealModeData = 0x0093;

    /// <summary>Present, S=1, type 0xB, D/B=1 (32-bit), G=1 (4 KiB granular).</summary>
    private const ushort ProtectedModeCode = 0xC09B;

    /// <summary>Present, S=1, type 0x3, D/B=1, G=1. Also used for data in long mode.</summary>
    private const ushort FlatData = 0xC093;

    /// <summary>Present, S=1, type 0xB, L=1 (64-bit), G=1.</summary>
    private const ushort LongModeCode = 0xA09B;

    private const ulong Cr0ProtectionEnable = 1UL << 0;
    private const ulong Cr0ExtensionType = 1UL << 4;
    private const ulong Cr0NumericError = 1UL << 5;
    private const ulong Cr0Paging = 1UL << 31;

    private const ulong Cr4PhysicalAddressExtension = 1UL << 5;
    private const ulong Cr4OsFxsr = 1UL << 9;
    private const ulong Cr4OsXmmExcept = 1UL << 10;

    private const ulong EferLongModeEnable = 1UL << 8;
    private const ulong EferLongModeActive = 1UL << 10;

    private const ulong PagePresent = 1UL << 0;
    private const ulong PageWritable = 1UL << 1;
    private const ulong PageLarge = 1UL << 7;

    private const ulong LargePageSize = 2 * 1024 * 1024;

    /// <summary>A PD of 512 large pages covers the first gigabyte of the address space.</summary>
    private const int LargePagesPerDirectory = 512;

    private const ushort CodeSelector = 0x08;
    private const ushort DataSelector = 0x10;

    /// <summary>Programs the processor for the configured mode and entry point.</summary>
    /// <param name="cpu">The processor to program.</param>
    /// <param name="memory">Guest memory, into which long mode page tables are written.</param>
    /// <param name="options">The machine configuration.</param>
    internal static void Apply(VirtualProcessor cpu, GuestMemory memory, PicoVmOptions options)
    {
        switch (options.Mode)
        {
            case GuestMode.Real16:
                ApplyRealMode(cpu, options);
                break;

            case GuestMode.Protected32:
                ApplyProtectedMode(cpu, options);
                break;

            case GuestMode.Long64:
                ApplyLongMode(cpu, memory, options);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(options), options.Mode, "Unknown guest mode.");
        }
    }

    private static void ApplyRealMode(VirtualProcessor cpu, PicoVmOptions options)
    {
        ulong code = options.EffectiveCodeAddress;

        // CS is based at the code address so the program sees itself at offset zero, the way a
        // .COM image does. Everything else is based at zero, so a data address written by the
        // program is a guest physical address.
        cpu.SetSegment(WhvRegisterName.Cs, Segment(code, 0xFFFF, (ushort)(code >> 4), RealModeCode));

        foreach (WhvRegisterName name in DataSegments)
        {
            cpu.SetSegment(name, Segment(0, 0xFFFF, 0, RealModeData));
        }

        SetControlState(
            cpu,
            rip: 0,
            rsp: options.EffectiveStackPointer,
            cr0: Cr0ExtensionType | Cr0NumericError,
            cr3: 0,
            cr4: 0,
            efer: 0);
    }

    private static void ApplyProtectedMode(VirtualProcessor cpu, PicoVmOptions options)
    {
        cpu.SetSegment(WhvRegisterName.Cs, Segment(0, 0xFFFFFFFF, CodeSelector, ProtectedModeCode));

        foreach (WhvRegisterName name in DataSegments)
        {
            cpu.SetSegment(name, Segment(0, 0xFFFFFFFF, DataSelector, FlatData));
        }

        // Paging stays off, so a linear address is a guest physical address.
        SetControlState(
            cpu,
            rip: options.EffectiveCodeAddress,
            rsp: options.EffectiveStackPointer,
            cr0: Cr0ProtectionEnable | Cr0ExtensionType | Cr0NumericError,
            cr3: 0,
            cr4: 0,
            efer: 0);
    }

    private static void ApplyLongMode(VirtualProcessor cpu, GuestMemory memory, PicoVmOptions options)
    {
        WriteIdentityPageTable(memory, options.PageTableAddress);

        cpu.SetSegment(WhvRegisterName.Cs, Segment(0, 0xFFFFFFFF, CodeSelector, LongModeCode));

        foreach (WhvRegisterName name in DataSegments)
        {
            cpu.SetSegment(name, Segment(0, 0xFFFFFFFF, DataSelector, FlatData));
        }

        SetControlState(
            cpu,
            rip: options.EffectiveCodeAddress,
            rsp: options.EffectiveStackPointer,
            cr0: Cr0ProtectionEnable | Cr0ExtensionType | Cr0NumericError | Cr0Paging,
            cr3: options.PageTableAddress,
            cr4: Cr4PhysicalAddressExtension | Cr4OsFxsr | Cr4OsXmmExcept,
            efer: EferLongModeEnable | EferLongModeActive);
    }

    /// <summary>
    /// Builds a four level page table that maps the first gigabyte of virtual addresses onto the
    /// same guest physical addresses, using 2 MiB pages so the whole map is three pages of
    /// tables. Addresses above RAM stay mapped on purpose: that is what lets a memory mapped
    /// device sit at an address with no RAM behind it and still be reachable by a normal load.
    /// </summary>
    private static void WriteIdentityPageTable(GuestMemory memory, ulong pageTableAddress)
    {
        ulong pml4 = pageTableAddress;
        ulong pdpt = pml4 + PicoVmOptions.PageSize;
        ulong pageDirectory = pdpt + PicoVmOptions.PageSize;

        memory.Fill(pml4, (int)(3 * PicoVmOptions.PageSize), 0);

        memory.WriteUInt64(pml4, pdpt | PagePresent | PageWritable);
        memory.WriteUInt64(pdpt, pageDirectory | PagePresent | PageWritable);

        for (int i = 0; i < LargePagesPerDirectory; i++)
        {
            ulong frame = (ulong)i * LargePageSize;
            memory.WriteUInt64(
                pageDirectory + ((ulong)i * sizeof(ulong)),
                frame | PagePresent | PageWritable | PageLarge);
        }
    }

    private static void SetControlState(
        VirtualProcessor cpu,
        ulong rip,
        ulong rsp,
        ulong cr0,
        ulong cr3,
        ulong cr4,
        ulong efer)
    {
        // One call, so the processor never holds a half-applied mode: the hypervisor validates
        // the combination of CR0.PG, CR4.PAE and EFER.LME when the guest is entered, not as each
        // register lands.
        ReadOnlySpan<GuestRegister> registers =
        [
            GuestRegister.Rip,
            GuestRegister.Rsp,
            GuestRegister.Rflags,
            GuestRegister.Cr0,
            GuestRegister.Cr3,
            GuestRegister.Cr4,
            GuestRegister.Efer,
        ];

        // Bit 1 of RFLAGS reads as one on every x86 processor ever made; interrupts stay
        // disabled because a pico VM has no interrupt descriptor table to vector through.
        ReadOnlySpan<ulong> values = [rip, rsp, 0x2, cr0, cr3, cr4, efer];

        cpu.SetRegisters(registers, values);
    }

    private static WhvSegmentRegister Segment(ulong segmentBase, uint limit, ushort selector, ushort attributes) => new()
    {
        Base = segmentBase,
        Limit = limit,
        Selector = selector,
        Attributes = attributes,
    };

    private static ReadOnlySpan<WhvRegisterName> DataSegments =>
    [
        WhvRegisterName.Ds,
        WhvRegisterName.Es,
        WhvRegisterName.Ss,
        WhvRegisterName.Fs,
        WhvRegisterName.Gs,
    ];
}
