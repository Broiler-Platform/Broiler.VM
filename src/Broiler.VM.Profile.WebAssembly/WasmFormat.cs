// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           14
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  2/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The section identifiers the binary format defines.
/// </summary>
/// <remarks>
/// These are identifiers and not positions. Two members below sit at a rank the numbers do not
/// predict, and <see cref="WasmFormat.OrderRankOf"/> rather than this enum is what a decoder
/// compares.
/// </remarks>
// Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=44892C
// Broiler-Human:        PENDING
internal enum WasmSectionId : byte
{
    /// <summary>A custom section: unordered, repeatable, and ignored by this decoder.</summary>
    Custom = 0,

    /// <summary>The function types the module declares.</summary>
    Type = 1,

    /// <summary>The imports the module requires of its environment.</summary>
    Import = 2,

    /// <summary>One type index per locally defined function.</summary>
    Function = 3,

    /// <summary>The tables the module defines.</summary>
    Table = 4,

    /// <summary>The linear memories the module defines.</summary>
    Memory = 5,

    /// <summary>The globals the module defines, each with an initializer.</summary>
    Global = 6,

    /// <summary>The names the module publishes to its environment.</summary>
    Export = 7,

    /// <summary>The function run at the end of instantiation.</summary>
    Start = 8,

    /// <summary>The element segments that initialise tables.</summary>
    Element = 9,

    /// <summary>One body per locally defined function.</summary>
    Code = 10,

    /// <summary>The data segments that initialise memories.</summary>
    Data = 11,

    /// <summary>The count the data section must agree with, declared before the code section.</summary>
    DataCount = 12,

    /// <summary>The tags an exception-handling module declares. No manifest here admits one.</summary>
    Tag = 13,
}

/// <summary>
/// The binary format's fixed vocabulary: its magic, its version, and the order its sections appear
/// in.
/// </summary>
/// <remarks>
/// <para>
/// <b>SECTION IDENTIFIERS DO NOT SORT INTO SECTION ORDER, AND A DECODER THAT COMPARES THEM IS
/// WRONG IN A WAY A TOOLCHAIN-PRODUCED SMOKE TEST WILL NOT CATCH.</b> The specification says so in
/// as many words, and there are two live disagreements in the table below. The tag section carries
/// identifier 13, higher than every other, and is ordered between the memory section and the global
/// section. The data count section carries identifier 12 and is ordered before the code section
/// whose data indices it bounds, which is the whole reason it exists. A module a compiler emits
/// today exercises neither, so a decoder built by comparing identifiers passes every module anybody
/// hands it and fails the specification's own suite.
/// </para>
/// <para>
/// The order is therefore a table, and <see cref="OrderRankOf"/> is the only thing this assembly
/// compares. A custom section has no rank because it has no position: it may appear anywhere, any
/// number of times, and the ordering rule does not reach it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=2; Fingerprint=ABAE0A
// Broiler-Falsified-If: section order is decided anywhere in this assembly by comparing identifier values rather than by this table
// Broiler-Human:        PENDING
internal static class WasmFormat
{
    /// <summary>The four bytes every module begins with.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=990972
    // Broiler-Human:        PENDING
    internal static System.ReadOnlySpan<byte> Magic => [0x00, 0x61, 0x73, 0x6D];

    /// <summary>
    /// The binary format version the four bytes after the magic must carry.
    /// </summary>
    /// <remarks>
    /// It has been 1 across every published revision of the specification and it is not a proxy for
    /// the language surface, which travels in the feature manifest instead.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=4B1A6F
    // Broiler-Human:        PENDING
    internal const uint BinaryVersion = 1;

    /// <summary>The lowest artifact format version this profile decodes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=9E0BD9
    // Broiler-Human:        PENDING
    internal const uint MinimumFormatVersion = 1;

    /// <summary>The highest artifact format version this profile decodes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B614A4
    // Broiler-Human:        PENDING
    internal const uint MaximumFormatVersion = 1;

    /// <summary>The highest section identifier the format defines.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=CF79F5
    // Broiler-Human:        PENDING
    internal const byte HighestSectionId = 13;

    /// <summary>The opcode that terminates an expression.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=E9B664
    // Broiler-Human:        PENDING
    internal const byte EndOpcode = 0x0B;

    /// <summary>
    /// Where a section sits in the canonical order, as a table rather than as a number derived from
    /// its identifier.
    /// </summary>
    /// <remarks>
    /// A custom section answers zero, which is below every ordered section and is deliberately not
    /// a position: the caller does not compare a custom section at all, and zero is what it reads
    /// if it ever mistakenly does.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=0C2C57
    // Broiler-Falsified-If: the tag section does not rank between memory and global, or the data count section does not rank before code
    // Broiler-Human:        PENDING
    internal static int OrderRankOf(WasmSectionId id) => id switch
    {
        WasmSectionId.Type => 1,
        WasmSectionId.Import => 2,
        WasmSectionId.Function => 3,
        WasmSectionId.Table => 4,
        WasmSectionId.Memory => 5,
        WasmSectionId.Tag => 6,
        WasmSectionId.Global => 7,
        WasmSectionId.Export => 8,
        WasmSectionId.Start => 9,
        WasmSectionId.Element => 10,
        WasmSectionId.DataCount => 11,
        WasmSectionId.Code => 12,
        WasmSectionId.Data => 13,
        _ => 0,
    };

    /// <summary>Whether a byte read where a section identifier is expected names a section.</summary>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=Medium; Resources=0; Fingerprint=3F8021
    // Broiler-Human:        PENDING
    internal static bool IsDefinedSectionId(byte id) => id <= HighestSectionId;
}
