// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           7
// Human-reviewed:   0/6
// IP risk:          Low
// Security risk:    Low
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.MachineCode;

// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=0B18E4
// Broiler-Human:        PENDING
public static class MachineCodeFormat
{
    /// <summary>The magic bytes starting a machine code artifact: "BMC\0".</summary>
    public static System.ReadOnlySpan<byte> Magic => "BMC\0"u8;

    /// <summary>Format version 1.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=F181CE
    // Broiler-Human:        PENDING
    public const uint FormatVersion = 1;

    /// <summary>Section kinds in a machine code artifact.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=DB9D3C
    // Broiler-Human:        PENDING
    public enum SectionKind : uint
    {
        Header = 0,
        Limits = 1,
        Constants = 2,
        NativeCode = 3,
        NativeSymbols = 4,
        Entries = 5,
    }
}

/// <summary>Execution limits for a machine code artifact.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=7707D0
// Broiler-Human:        PENDING
public readonly record struct MachineCodeLimits(uint MaximumOperandStack, uint MaximumScopeSlots);

/// <summary>One native symbol mapping a code unit index to its byte offset in the native code section.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=67C464
// Broiler-Human:        PENDING
public readonly record struct MachineCodeSymbol(uint FunctionIndex, uint Offset, uint Length);

/// <summary>One named entry point into the machine code artifact.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=6DAAB4
// Broiler-Human:        PENDING
public readonly record struct MachineCodeEntry(string Name, uint FunctionIndex);
