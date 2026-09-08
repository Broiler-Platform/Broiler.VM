// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   20
// Annotated:        20/20
// Exempt:           25
// Human-reviewed:   0/20
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  1/10 max
// Unverified:       20
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The value types the binary format encodes as single negative bytes.
/// </summary>
/// <remarks>
/// The encoding is the signed 7-bit form of a negative number, which is why the byte values run
/// downwards from <c>0x7F</c>. Only the four numeric members are admitted by anything this build
/// decodes; the vector and reference members are named so that a byte carrying one is recognised
/// and refused as an unadmitted feature rather than as an unknown byte.
/// </remarks>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=64AF44
// Broiler-Human:        PENDING
internal enum WasmValueType : byte
{
    /// <summary>A 32-bit integer.</summary>
    I32 = 0x7F,

    /// <summary>A 64-bit integer.</summary>
    I64 = 0x7E,

    /// <summary>A 32-bit float.</summary>
    F32 = 0x7D,

    /// <summary>A 64-bit float.</summary>
    F64 = 0x7C,

    /// <summary>A 128-bit vector. Named, and admitted by no manifest here.</summary>
    V128 = 0x7B,

    /// <summary>A function reference. Named, and admitted by no manifest here.</summary>
    FuncRef = 0x70,

    /// <summary>An external reference. Named, and admitted by no manifest here.</summary>
    ExternRef = 0x6F,
}

/// <summary>The four kinds of thing an import or an export can name.</summary>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=6FEB0B
// Broiler-Human:        PENDING
internal enum WasmExternalKind : byte
{
    /// <summary>A function.</summary>
    Function = 0,

    /// <summary>A table.</summary>
    Table = 1,

    /// <summary>A linear memory.</summary>
    Memory = 2,

    /// <summary>A global.</summary>
    Global = 3,
}

/// <summary>
/// A resizable limit: a minimum, and a maximum that may be absent.
/// </summary>
/// <remarks>
/// The absent maximum is a distinct state rather than a sentinel number, because an imported
/// memory or table has to be compared for compatibility and a module declaring no maximum is not
/// the same as one declaring the largest maximum the format can hold.
/// </remarks>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=7B8F59
// Broiler-Human:        PENDING
internal readonly struct WasmLimits
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7285DE
    // Broiler-Human:        PENDING
    internal WasmLimits(uint minimum, uint maximum, bool hasMaximum)
    {
        Minimum = minimum;
        Maximum = maximum;
        HasMaximum = hasMaximum;
    }

    /// <summary>The declared minimum.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=FB7523
    // Broiler-Human:        PENDING
    internal uint Minimum { get; }

    /// <summary>The declared maximum, meaningful only when <see cref="HasMaximum"/> is set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=5CF045
    // Broiler-Human:        PENDING
    internal uint Maximum { get; }

    /// <summary>Whether a maximum was declared at all.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=29A400
    // Broiler-Human:        PENDING
    internal bool HasMaximum { get; }
}

/// <summary>A linear memory's declared shape, which is its limits in units of 64 KiB pages.</summary>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=85A311
// Broiler-Human:        PENDING
internal readonly struct WasmMemoryType
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=129563
    // Broiler-Human:        PENDING
    internal WasmMemoryType(WasmLimits limits) => Limits = limits;

    /// <summary>The page limits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=28A854
    // Broiler-Human:        PENDING
    internal WasmLimits Limits { get; }
}

/// <summary>A table's declared shape: its element type and its limits in elements.</summary>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=A49863
// Broiler-Human:        PENDING
internal readonly struct WasmTableType
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A56460
    // Broiler-Human:        PENDING
    internal WasmTableType(WasmValueType elementType, WasmLimits limits)
    {
        ElementType = elementType;
        Limits = limits;
    }

    /// <summary>The element type. At this format version it can only be a function reference.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C7FD01
    // Broiler-Human:        PENDING
    internal WasmValueType ElementType { get; }

    /// <summary>The element limits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=28A854
    // Broiler-Human:        PENDING
    internal WasmLimits Limits { get; }
}

/// <summary>A global's declared shape: its value type and whether it may be assigned.</summary>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=5BF120
// Broiler-Human:        PENDING
internal readonly struct WasmGlobalType
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=180A13
    // Broiler-Human:        PENDING
    internal WasmGlobalType(WasmValueType valueType, bool isMutable)
    {
        ValueType = valueType;
        IsMutable = isMutable;
    }

    /// <summary>The value type held.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=03EE7C
    // Broiler-Human:        PENDING
    internal WasmValueType ValueType { get; }

    /// <summary>Whether <c>global.set</c> may name it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=39F421
    // Broiler-Human:        PENDING
    internal bool IsMutable { get; }
}

/// <summary>
/// One function type: a vector of parameter types and a vector of result types.
/// </summary>
/// <remarks>
/// The arrays are held rather than handed out, because everything reachable from the verified state
/// has to be immutable once verification returns and an array a caller holds is not.
/// </remarks>
// Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=33E22E
// Broiler-Falsified-If: either array reaches a caller, so a state two runtimes share can be mutated through it
// Broiler-Human:        PENDING
internal sealed class WasmFuncType
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0AD2A4
    // Broiler-Human:        PENDING
    private readonly WasmValueType[] parameters;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=53E85D
    // Broiler-Human:        PENDING
    private readonly WasmValueType[] results;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8D3855
    // Broiler-Human:        PENDING
    internal WasmFuncType(WasmValueType[] parameterTypes, WasmValueType[] resultTypes)
    {
        parameters = parameterTypes;
        results = resultTypes;
    }

    /// <summary>How many parameters the type declares.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=66D5CB
    // Broiler-Human:        PENDING
    internal int ParameterCount => parameters.Length;

    /// <summary>How many results the type declares.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=997333
    // Broiler-Human:        PENDING
    internal int ResultCount => results.Length;

    /// <summary>The parameter types, as a read-only window over the array this type keeps.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=4DBBDB
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<WasmValueType> Parameters =>
        System.MemoryExtensions.AsSpan(parameters);

    /// <summary>The result types, as a read-only window over the array this type keeps.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F4CCE3
    // Broiler-Human:        PENDING
    internal System.ReadOnlySpan<WasmValueType> Results =>
        System.MemoryExtensions.AsSpan(results);
}

/// <summary>
/// The one-byte tags and fixed numbers the type grammar uses, and the predicates over them.
/// </summary>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=1; Fingerprint=4FF4CE
// Broiler-Human:        PENDING
internal static class WasmTypeGrammar
{
    /// <summary>The byte a function type begins with.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=970559
    // Broiler-Human:        PENDING
    internal const byte FunctionTypeTag = 0x60;

    /// <summary>The limits flag meaning a minimum and no maximum.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=D11A9B
    // Broiler-Human:        PENDING
    internal const byte LimitsMinimumOnly = 0x00;

    /// <summary>The limits flag meaning a minimum and a maximum.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=75D48E
    // Broiler-Human:        PENDING
    internal const byte LimitsMinimumAndMaximum = 0x01;

    /// <summary>The block type meaning no result.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=FCB443
    // Broiler-Human:        PENDING
    internal const byte EmptyBlockType = 0x40;

    /// <summary>
    /// The most pages a linear memory may declare, which is the whole of a 32-bit address space in
    /// 64 KiB pages.
    /// </summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=09CB0A
    // Broiler-Human:        PENDING
    internal const uint MaximumMemoryPages = 65_536;

    /// <summary>Whether a byte is one of the four numeric value types this build decodes.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=EBCED0
    // Broiler-Human:        PENDING
    internal static bool IsNumericValueType(byte encoded) =>
        encoded is 0x7F or 0x7E or 0x7D or 0x7C;

    /// <summary>Whether a byte is any value type the format defines, admitted here or not.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=75770D
    // Broiler-Human:        PENDING
    internal static bool IsAnyValueType(byte encoded) =>
        encoded is 0x7F or 0x7E or 0x7D or 0x7C or 0x7B or 0x70 or 0x6F;
}
