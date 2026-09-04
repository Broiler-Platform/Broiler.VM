// Copyright (c) Broiler contributors. Licensed under the Apache-2.0 license.

using System;
using System.Runtime.InteropServices;

namespace Broiler.VM.HyperV.Interop;

/// <summary>Mirrors WHV_CAPABILITY_CODE from WinHvPlatformDefs.h.</summary>
internal enum WhvCapabilityCode : uint
{
    HypervisorPresent = 0x00000000,
    Features = 0x00000001,
    ExtendedVmExits = 0x00000002,
    ProcessorVendor = 0x00001000,
    ProcessorFeatures = 0x00001001,
    ProcessorClFlushSize = 0x00001002,
    ProcessorClockFrequency = 0x00001004,
    PhysicalAddressWidth = 0x0000100A,
}

/// <summary>Mirrors WHV_PARTITION_PROPERTY_CODE. Only the properties a pico VM sets are listed.</summary>
internal enum WhvPartitionPropertyCode : uint
{
    ExtendedVmExits = 0x00000001,
    ProcessorFeatures = 0x00001001,
    CpuidExitList = 0x00001003,
    LocalApicEmulationMode = 0x00001005,
    ProcessorCount = 0x00001fff,
}

/// <summary>Mirrors WHV_MAP_GPA_RANGE_FLAGS.</summary>
[Flags]
internal enum WhvMapGpaRangeFlags : uint
{
    None = 0x00000000,
    Read = 0x00000001,
    Write = 0x00000002,
    Execute = 0x00000004,
    TrackDirtyPages = 0x00000008,
}

/// <summary>Mirrors WHV_TRANSLATE_GVA_FLAGS.</summary>
[Flags]
internal enum WhvTranslateGvaFlags : uint
{
    None = 0x00000000,
    ValidateRead = 0x00000001,
    ValidateWrite = 0x00000002,
    ValidateExecute = 0x00000004,
    PrivilegeExempt = 0x00000008,
    SetPageTableBits = 0x00000010,
}

/// <summary>Mirrors WHV_TRANSLATE_GVA_RESULT_CODE.</summary>
internal enum WhvTranslateGvaResultCode : uint
{
    Success = 0,
    PageNotPresent = 1,
    PrivilegeViolation = 2,
    InvalidPageTableFlags = 3,
    GpaUnmapped = 4,
    GpaNoReadAccess = 5,
    GpaNoWriteAccess = 6,
    GpaIllegalOverlayAccess = 7,
    Intercept = 8,
}

/// <summary>Mirrors WHV_RUN_VP_EXIT_REASON for AMD64.</summary>
internal enum WhvRunVpExitReason : uint
{
    None = 0x00000000,
    MemoryAccess = 0x00000001,
    X64IoPortAccess = 0x00000002,
    UnrecoverableException = 0x00000004,
    InvalidVpRegisterValue = 0x00000005,
    UnsupportedFeature = 0x00000006,
    X64InterruptWindow = 0x00000007,
    X64Halt = 0x00000008,
    X64ApicEoi = 0x00000009,
    SynicSintDeliverable = 0x0000000A,
    X64MsrAccess = 0x00001000,
    X64Cpuid = 0x00001001,
    Exception = 0x00001002,
    X64Rdtsc = 0x00001003,
    X64ApicSmiTrap = 0x00001004,
    Hypercall = 0x00001005,
    X64ApicInitSipiTrap = 0x00001006,
    X64ApicWriteTrap = 0x00001007,
    Canceled = 0x00002001,
}

/// <summary>Mirrors WHV_PROCESSOR_VENDOR.</summary>
internal enum WhvProcessorVendor : ushort
{
    Amd = 0x0000,
    Intel = 0x0001,
    Hygon = 0x0002,
    Arm = 0x0010,
}

/// <summary>
/// Mirrors WHV_REGISTER_NAME for AMD64. Kept in step with GuestRegister, which is the
/// public spelling of the same values.
/// </summary>
internal enum WhvRegisterName : uint
{
    Rax = 0x00000000,
    Rcx = 0x00000001,
    Rdx = 0x00000002,
    Rbx = 0x00000003,
    Rsp = 0x00000004,
    Rbp = 0x00000005,
    Rsi = 0x00000006,
    Rdi = 0x00000007,
    R8 = 0x00000008,
    R9 = 0x00000009,
    R10 = 0x0000000A,
    R11 = 0x0000000B,
    R12 = 0x0000000C,
    R13 = 0x0000000D,
    R14 = 0x0000000E,
    R15 = 0x0000000F,
    Rip = 0x00000010,
    Rflags = 0x00000011,
    Es = 0x00000012,
    Cs = 0x00000013,
    Ss = 0x00000014,
    Ds = 0x00000015,
    Fs = 0x00000016,
    Gs = 0x00000017,
    Ldtr = 0x00000018,
    Tr = 0x00000019,
    Idtr = 0x0000001A,
    Gdtr = 0x0000001B,
    Cr0 = 0x0000001C,
    Cr2 = 0x0000001D,
    Cr3 = 0x0000001E,
    Cr4 = 0x0000001F,
    Cr8 = 0x00000020,
    XCr0 = 0x00000027,
    Tsc = 0x00002000,
    Efer = 0x00002001,
    ApicBase = 0x00002003,
    Pat = 0x00002004,
}

/// <summary>Mirrors WHV_X64_SEGMENT_REGISTER (16 bytes).</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct WhvSegmentRegister
{
    public ulong Base;
    public uint Limit;
    public ushort Selector;

    /// <summary>The packed attribute word: type, S, DPL, P, AVL, L, D/B and G.</summary>
    public ushort Attributes;
}

/// <summary>Mirrors WHV_X64_TABLE_REGISTER (16 bytes) - GDTR and IDTR.</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct WhvTableRegister
{
    public ushort Pad0;
    public ushort Pad1;
    public ushort Pad2;
    public ushort Limit;
    public ulong Base;
}

/// <summary>Mirrors the WHV_REGISTER_VALUE union (16 bytes).</summary>
[StructLayout(LayoutKind.Explicit, Size = 16)]
internal struct WhvRegisterValue
{
    [FieldOffset(0)]
    public ulong Reg64;

    [FieldOffset(8)]
    public ulong High64;

    [FieldOffset(0)]
    public WhvSegmentRegister Segment;

    [FieldOffset(0)]
    public WhvTableRegister Table;

    public static WhvRegisterValue FromUInt64(ulong value) => new WhvRegisterValue { Reg64 = value };
}

/// <summary>Mirrors WHV_X64_VP_EXIT_CONTEXT (40 bytes).</summary>
[StructLayout(LayoutKind.Explicit, Size = 40)]
internal struct WhvVpExitContext
{
    [FieldOffset(0)]
    public ushort ExecutionState;

    /// <summary>Low nibble is the length of the instruction that caused the exit, high nibble is CR8.</summary>
    [FieldOffset(2)]
    public byte InstructionLengthAndCr8;

    [FieldOffset(8)]
    public WhvSegmentRegister Cs;

    [FieldOffset(24)]
    public ulong Rip;

    [FieldOffset(32)]
    public ulong Rflags;

    public readonly int InstructionLength => InstructionLengthAndCr8 & 0x0F;

    /// <summary>CPL, bits 0-1 of the execution state word.</summary>
    public readonly int Cpl => ExecutionState & 0x3;

    /// <summary>EFER.LMA, bit 4 of the execution state word: the VP is in long mode.</summary>
    public readonly bool LongModeActive => (ExecutionState & 0x10) != 0;

    /// <summary>CR0.PE, bit 2 of the execution state word: the VP is in protected mode.</summary>
    public readonly bool ProtectedModeActive => (ExecutionState & 0x04) != 0;
}

/// <summary>Mirrors WHV_MEMORY_ACCESS_CONTEXT (40 bytes).</summary>
[StructLayout(LayoutKind.Explicit, Size = 40)]
internal unsafe struct WhvMemoryAccessContext
{
    [FieldOffset(0)]
    public byte InstructionByteCount;

    [FieldOffset(4)]
    public fixed byte InstructionBytes[16];

    /// <summary>Bits 0-1 access type, bit 2 GpaUnmapped, bit 3 GvaValid.</summary>
    [FieldOffset(20)]
    public uint AccessInfo;

    [FieldOffset(24)]
    public ulong Gpa;

    [FieldOffset(32)]
    public ulong Gva;

    public readonly uint AccessType => AccessInfo & 0x3;

    public readonly bool GpaUnmapped => (AccessInfo & 0x4) != 0;

    public readonly bool GvaValid => (AccessInfo & 0x8) != 0;
}

/// <summary>Mirrors WHV_X64_IO_PORT_ACCESS_CONTEXT (96 bytes).</summary>
[StructLayout(LayoutKind.Explicit, Size = 96)]
internal unsafe struct WhvIoPortAccessContext
{
    [FieldOffset(0)]
    public byte InstructionByteCount;

    [FieldOffset(4)]
    public fixed byte InstructionBytes[16];

    /// <summary>Bit 0 IsWrite, bits 1-3 access size in bytes, bit 4 StringOp, bit 5 RepPrefix.</summary>
    [FieldOffset(20)]
    public uint AccessInfo;

    [FieldOffset(24)]
    public ushort PortNumber;

    [FieldOffset(32)]
    public ulong Rax;

    [FieldOffset(40)]
    public ulong Rcx;

    [FieldOffset(48)]
    public ulong Rsi;

    [FieldOffset(56)]
    public ulong Rdi;

    [FieldOffset(64)]
    public WhvSegmentRegister Ds;

    [FieldOffset(80)]
    public WhvSegmentRegister Es;

    public readonly bool IsWrite => (AccessInfo & 0x1) != 0;

    public readonly int AccessSize => (int)((AccessInfo >> 1) & 0x7);

    public readonly bool IsStringOp => (AccessInfo & 0x10) != 0;

    public readonly bool HasRepPrefix => (AccessInfo & 0x20) != 0;
}

/// <summary>
/// Mirrors WHV_RUN_VP_EXIT_CONTEXT (224 bytes on AMD64). The union at offset 48 is
/// projected as the two arms a pico VM services; the rest is left as padding.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 224)]
internal struct WhvRunVpExitContext
{
    [FieldOffset(0)]
    public WhvRunVpExitReason ExitReason;

    [FieldOffset(8)]
    public WhvVpExitContext VpContext;

    [FieldOffset(48)]
    public WhvMemoryAccessContext MemoryAccess;

    [FieldOffset(48)]
    public WhvIoPortAccessContext IoPortAccess;
}

/// <summary>Mirrors WHV_EMULATOR_MEMORY_ACCESS_INFO (24 bytes).</summary>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct WhvEmulatorMemoryAccessInfo
{
    public ulong GpaAddress;

    /// <summary>0 = the guest is reading (fill Data), 1 = the guest is writing (consume Data).</summary>
    public byte Direction;

    public byte AccessSize;

    public fixed byte Data[8];
}

/// <summary>Mirrors WHV_EMULATOR_IO_ACCESS_INFO (12 bytes).</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct WhvEmulatorIoAccessInfo
{
    /// <summary>0 = IN (fill Data), 1 = OUT (consume Data).</summary>
    public byte Direction;

    public ushort Port;

    public ushort AccessSize;

    public uint Data;
}

/// <summary>Mirrors WHV_EMULATOR_CALLBACKS (48 bytes).</summary>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct WhvEmulatorCallbacks
{
    public uint Size;
    public uint Reserved;
    public delegate* unmanaged[Stdcall]<void*, WhvEmulatorIoAccessInfo*, int> IoPort;
    public delegate* unmanaged[Stdcall]<void*, WhvEmulatorMemoryAccessInfo*, int> Memory;
    public delegate* unmanaged[Stdcall]<void*, WhvRegisterName*, uint, WhvRegisterValue*, int> GetRegisters;
    public delegate* unmanaged[Stdcall]<void*, WhvRegisterName*, uint, WhvRegisterValue*, int> SetRegisters;
    public delegate* unmanaged[Stdcall]<void*, ulong, WhvTranslateGvaFlags, WhvTranslateGvaResultCode*, ulong*, int> TranslateGvaPage;
}

/// <summary>Mirrors the WHV_EMULATOR_STATUS bit field.</summary>
[StructLayout(LayoutKind.Sequential)]
internal struct WhvEmulatorStatus
{
    public uint AsUInt32;

    public readonly bool EmulationSuccessful => (AsUInt32 & 0x1) != 0;
}
