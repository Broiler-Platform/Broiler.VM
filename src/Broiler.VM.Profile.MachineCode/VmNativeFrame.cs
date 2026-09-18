// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   3
// Annotated:        3/3
// Exempt:           14
// Human-reviewed:   0/3
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       3
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.MachineCode;

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=0647B0
// Broiler-Human:        PENDING
public enum VmNativeArchitecture : uint
{
    /// <summary>No architecture.</summary>
    None = 0,

    /// <summary>x86-64 under the System V AMD64 convention: the frame pointer arrives in RDI.</summary>
    X64SystemV = 1,

    /// <summary>x86-64 under the Windows x64 convention: the frame pointer arrives in RCX.</summary>
    X64Windows = 2,

    /// <summary>arm64 under AAPCS64: the frame pointer arrives in X0.</summary>
    Arm64 = 3,
}

/// <summary>What an emitted code unit answers when it returns to its caller.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=8F527C
// Broiler-Human:        PENDING
public enum VmNativeReturn
{
    /// <summary>The unit returned normally. Its value is in the frame's operand slot zero.</summary>
    Returned = 0,

    /// <summary>The unit raised a fault.</summary>
    Threw = 2,

    /// <summary>The unit ran out of fuel.</summary>
    FuelExhausted = 3,
}

/// <summary>
/// The frame an emitted native code unit is called with.
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4998BA
// Broiler-Human:        PENDING
[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public unsafe struct VmNativeFrame
{
    /// <summary>The operand stack: unmanaged storage the emitted unit pushes and pops through.</summary>
    public double* Operands;

    /// <summary>How many operand slots <see cref="Operands"/> holds.</summary>
    public int OperandCount;

    /// <summary>The unit's environment/local slots.</summary>
    public double* Locals;

    /// <summary>How many slots <see cref="Locals"/> holds.</summary>
    public int LocalCount;

    /// <summary>The unit's numeric constants.</summary>
    public double* Constants;

    /// <summary>The remaining fuel counter pointer.</summary>
    public long* Fuel;

    /// <summary>The bytecode offset reached when stopped.</summary>
    public int BailoutPc;
}
