// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Broiler.VM.HyperV.Interop;

namespace Broiler.VM.HyperV;

/// <summary>
/// The architectural state of a pico VM's single virtual processor.
/// </summary>
/// <remarks>
/// Every property here is a round trip into the hypervisor, so batch reads with
/// <see cref="GetRegisters"/> or <see cref="Capture"/> when reading more than a couple at once.
/// Registers may only be touched while the processor is not running, which for a pico VM means
/// before a run starts or after one returns.
/// </remarks>
[SupportedOSPlatform("windows")]
public sealed unsafe class VirtualProcessor
{
    private readonly nint partition;

    internal VirtualProcessor(nint partition, uint index)
    {
        this.partition = partition;
        Index = index;
    }

    /// <summary>The processor's index within its partition.</summary>
    public uint Index { get; }

    /// <summary>Gets or sets RAX.</summary>
    public ulong Rax
    {
        get => GetRegister(GuestRegister.Rax);
        set => SetRegister(GuestRegister.Rax, value);
    }

    /// <summary>Gets or sets RBX.</summary>
    public ulong Rbx
    {
        get => GetRegister(GuestRegister.Rbx);
        set => SetRegister(GuestRegister.Rbx, value);
    }

    /// <summary>Gets or sets RCX.</summary>
    public ulong Rcx
    {
        get => GetRegister(GuestRegister.Rcx);
        set => SetRegister(GuestRegister.Rcx, value);
    }

    /// <summary>Gets or sets RDX.</summary>
    public ulong Rdx
    {
        get => GetRegister(GuestRegister.Rdx);
        set => SetRegister(GuestRegister.Rdx, value);
    }

    /// <summary>Gets or sets RSI.</summary>
    public ulong Rsi
    {
        get => GetRegister(GuestRegister.Rsi);
        set => SetRegister(GuestRegister.Rsi, value);
    }

    /// <summary>Gets or sets RDI.</summary>
    public ulong Rdi
    {
        get => GetRegister(GuestRegister.Rdi);
        set => SetRegister(GuestRegister.Rdi, value);
    }

    /// <summary>Gets or sets RSP.</summary>
    public ulong Rsp
    {
        get => GetRegister(GuestRegister.Rsp);
        set => SetRegister(GuestRegister.Rsp, value);
    }

    /// <summary>Gets or sets RBP.</summary>
    public ulong Rbp
    {
        get => GetRegister(GuestRegister.Rbp);
        set => SetRegister(GuestRegister.Rbp, value);
    }

    /// <summary>Gets or sets RIP.</summary>
    public ulong Rip
    {
        get => GetRegister(GuestRegister.Rip);
        set => SetRegister(GuestRegister.Rip, value);
    }

    /// <summary>Gets or sets RFLAGS.</summary>
    public ulong Rflags
    {
        get => GetRegister(GuestRegister.Rflags);
        set => SetRegister(GuestRegister.Rflags, value);
    }

    /// <summary>Reads one register.</summary>
    /// <param name="register">The register to read.</param>
    /// <returns>Its value, zero extended to 64 bits.</returns>
    /// <exception cref="HyperVException">The hypervisor refused the read.</exception>
    public ulong GetRegister(GuestRegister register)
    {
        WhvRegisterName name = (WhvRegisterName)register;
        WhvRegisterValue value = default;

        HyperVException.ThrowIfFailed(
            WhvNative.WHvGetVirtualProcessorRegisters(partition, Index, &name, 1, &value),
            "WHvGetVirtualProcessorRegisters");

        return value.Reg64;
    }

    /// <summary>Writes one register.</summary>
    /// <param name="register">The register to write.</param>
    /// <param name="value">The value to store.</param>
    /// <exception cref="HyperVException">The hypervisor refused the write.</exception>
    public void SetRegister(GuestRegister register, ulong value)
    {
        WhvRegisterName name = (WhvRegisterName)register;
        WhvRegisterValue raw = WhvRegisterValue.FromUInt64(value);

        HyperVException.ThrowIfFailed(
            WhvNative.WHvSetVirtualProcessorRegisters(partition, Index, &name, 1, &raw),
            "WHvSetVirtualProcessorRegisters");
    }

    /// <summary>Reads several registers in one call.</summary>
    /// <param name="registers">The registers to read.</param>
    /// <param name="values">Receives the values, in the same order.</param>
    /// <exception cref="ArgumentException"><paramref name="values"/> is too short.</exception>
    /// <exception cref="HyperVException">The hypervisor refused the read.</exception>
    public void GetRegisters(ReadOnlySpan<GuestRegister> registers, Span<ulong> values)
    {
        if (values.Length < registers.Length)
        {
            throw new ArgumentException("The value buffer is shorter than the register list.", nameof(values));
        }

        if (registers.IsEmpty)
        {
            return;
        }

        ReadOnlySpan<WhvRegisterName> names = MemoryMarshal.Cast<GuestRegister, WhvRegisterName>(registers);
        WhvRegisterValue* raw = AllocateValues(registers.Length);

        try
        {
            fixed (WhvRegisterName* namePointer = names)
            {
                HyperVException.ThrowIfFailed(
                    WhvNative.WHvGetVirtualProcessorRegisters(partition, Index, namePointer, (uint)names.Length, raw),
                    "WHvGetVirtualProcessorRegisters");
            }

            for (int i = 0; i < registers.Length; i++)
            {
                values[i] = raw[i].Reg64;
            }
        }
        finally
        {
            NativeMemory.AlignedFree(raw);
        }
    }

    /// <summary>Writes several registers in one call.</summary>
    /// <param name="registers">The registers to write.</param>
    /// <param name="values">The values, in the same order.</param>
    /// <exception cref="ArgumentException"><paramref name="values"/> is too short.</exception>
    /// <exception cref="HyperVException">The hypervisor refused the write.</exception>
    public void SetRegisters(ReadOnlySpan<GuestRegister> registers, ReadOnlySpan<ulong> values)
    {
        if (values.Length < registers.Length)
        {
            throw new ArgumentException("The value list is shorter than the register list.", nameof(values));
        }

        if (registers.IsEmpty)
        {
            return;
        }

        ReadOnlySpan<WhvRegisterName> names = MemoryMarshal.Cast<GuestRegister, WhvRegisterName>(registers);
        WhvRegisterValue* raw = AllocateValues(registers.Length);

        try
        {
            for (int i = 0; i < registers.Length; i++)
            {
                raw[i] = WhvRegisterValue.FromUInt64(values[i]);
            }

            fixed (WhvRegisterName* namePointer = names)
            {
                HyperVException.ThrowIfFailed(
                    WhvNative.WHvSetVirtualProcessorRegisters(partition, Index, namePointer, (uint)names.Length, raw),
                    "WHvSetVirtualProcessorRegisters");
            }
        }
        finally
        {
            NativeMemory.AlignedFree(raw);
        }
    }

    /// <summary>
    /// Allocates a register value array the way the hypervisor expects to find one.
    /// </summary>
    /// <remarks>
    /// WHV_REGISTER_VALUE is declared DECLSPEC_ALIGN(16) - it holds 128-bit XMM values - and the
    /// API reads arrays of it with aligned SSE moves. A managed array only guarantees 8-byte
    /// alignment for it, which faults the process inside WinHvPlatform.dll about half the time,
    /// so the buffer is allocated natively with the alignment the contract asks for.
    /// </remarks>
    private static WhvRegisterValue* AllocateValues(int count) =>
        (WhvRegisterValue*)NativeMemory.AlignedAlloc((nuint)(count * sizeof(WhvRegisterValue)), 16);

    /// <summary>Reads the registers worth looking at after a run, in a single call.</summary>
    /// <returns>A snapshot of the general purpose and control registers.</returns>
    public GuestState Capture()
    {
        ReadOnlySpan<GuestRegister> names =
        [
            GuestRegister.Rax, GuestRegister.Rbx, GuestRegister.Rcx, GuestRegister.Rdx,
            GuestRegister.Rsi, GuestRegister.Rdi, GuestRegister.Rsp, GuestRegister.Rbp,
            GuestRegister.R8, GuestRegister.R9, GuestRegister.R10, GuestRegister.R11,
            GuestRegister.R12, GuestRegister.R13, GuestRegister.R14, GuestRegister.R15,
            GuestRegister.Rip, GuestRegister.Rflags,
            GuestRegister.Cr0, GuestRegister.Cr2, GuestRegister.Cr3, GuestRegister.Cr4,
            GuestRegister.Efer,
        ];

        Span<ulong> values = stackalloc ulong[names.Length];
        GetRegisters(names, values);

        return new GuestState(
            values[0], values[1], values[2], values[3],
            values[4], values[5], values[6], values[7],
            values[8], values[9], values[10], values[11],
            values[12], values[13], values[14], values[15],
            values[16], values[17],
            values[18], values[19], values[20], values[21],
            values[22]);
    }

    internal void SetSegment(WhvRegisterName name, WhvSegmentRegister segment)
    {
        WhvRegisterValue value = default;
        value.Segment = segment;

        HyperVException.ThrowIfFailed(
            WhvNative.WHvSetVirtualProcessorRegisters(partition, Index, &name, 1, &value),
            "WHvSetVirtualProcessorRegisters");
    }
}

/// <summary>A snapshot of the guest registers worth reporting after a run.</summary>
/// <param name="Rax">RAX.</param>
/// <param name="Rbx">RBX.</param>
/// <param name="Rcx">RCX.</param>
/// <param name="Rdx">RDX.</param>
/// <param name="Rsi">RSI.</param>
/// <param name="Rdi">RDI.</param>
/// <param name="Rsp">RSP.</param>
/// <param name="Rbp">RBP.</param>
/// <param name="R8">R8.</param>
/// <param name="R9">R9.</param>
/// <param name="R10">R10.</param>
/// <param name="R11">R11.</param>
/// <param name="R12">R12.</param>
/// <param name="R13">R13.</param>
/// <param name="R14">R14.</param>
/// <param name="R15">R15.</param>
/// <param name="Rip">RIP.</param>
/// <param name="Rflags">RFLAGS.</param>
/// <param name="Cr0">CR0.</param>
/// <param name="Cr2">CR2.</param>
/// <param name="Cr3">CR3.</param>
/// <param name="Cr4">CR4.</param>
/// <param name="Efer">EFER.</param>
public readonly record struct GuestState(
    ulong Rax,
    ulong Rbx,
    ulong Rcx,
    ulong Rdx,
    ulong Rsi,
    ulong Rdi,
    ulong Rsp,
    ulong Rbp,
    ulong R8,
    ulong R9,
    ulong R10,
    ulong R11,
    ulong R12,
    ulong R13,
    ulong R14,
    ulong R15,
    ulong Rip,
    ulong Rflags,
    ulong Cr0,
    ulong Cr2,
    ulong Cr3,
    ulong Cr4,
    ulong Efer)
{
    /// <summary>Renders the snapshot as the kind of register dump a debugger prints.</summary>
    /// <returns>A multi-line, fixed-width register dump.</returns>
    public string Format()
    {
        StringBuilder text = new();
        Line(text, "rax", Rax, "rbx", Rbx, "rcx", Rcx, "rdx", Rdx);
        Line(text, "rsi", Rsi, "rdi", Rdi, "rsp", Rsp, "rbp", Rbp);
        Line(text, "r8 ", R8, "r9 ", R9, "r10", R10, "r11", R11);
        Line(text, "r12", R12, "r13", R13, "r14", R14, "r15", R15);
        Line(text, "rip", Rip, "rfl", Rflags, "cr0", Cr0, "cr2", Cr2);
        Line(text, "cr3", Cr3, "cr4", Cr4, "efr", Efer, null, 0);
        return text.ToString();

        static void Line(StringBuilder text, string n0, ulong v0, string n1, ulong v1, string n2, ulong v2, string? n3, ulong v3)
        {
            _ = text.AppendFormat(CultureInfo.InvariantCulture, "{0}={1:x16}  {2}={3:x16}  {4}={5:x16}", n0, v0, n1, v1, n2, v2);

            if (n3 is not null)
            {
                _ = text.AppendFormat(CultureInfo.InvariantCulture, "  {0}={1:x16}", n3, v3);
            }

            _ = text.AppendLine();
        }
    }
}
