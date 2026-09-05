// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   23
// Annotated:        23/23
// Exempt:           0
// Human-reviewed:   0/23
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       23
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>One code unit's row in the functions section.</summary>
/// <param name="NameConstant">
/// One more than the constant index of the unit's name, or zero when it is anonymous. The bias is
/// what lets zero mean "no name" without reserving a constant slot for it.
/// </param>
/// <param name="ParameterCount">How many declared parameters the unit has.</param>
/// <param name="ScopeSlots">How many slots the unit's own environment record holds.</param>
/// <param name="MaxOperandStack">The operand-stack height the unit declares it never exceeds.</param>
/// <param name="CodeOffset">Where the unit's code starts in the code section.</param>
/// <param name="CodeLength">How many bytes of code the unit has.</param>
/// <param name="Flags">The unit's flag bits.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=2B4ACB
// Broiler-Human:        PENDING
public readonly record struct JsFunctionRow(
    uint NameConstant,
    uint ParameterCount,
    uint ScopeSlots,
    uint MaxOperandStack,
    uint CodeOffset,
    uint CodeLength,
    uint Flags);

/// <summary>One exception region.</summary>
/// <param name="FunctionIndex">The code unit the region belongs to.</param>
/// <param name="TryStart">The first code offset the region covers.</param>
/// <param name="TryEnd">The first code offset after the region.</param>
/// <param name="HandlerOffset">Where control goes when the region catches.</param>
/// <param name="ScopeDepth">
/// How many environments deep the handler runs. Control unwinds to exactly this depth before the
/// handler is entered, so a <c>throw</c> from inside a block does not leave that block's scope on
/// the chain.
/// </param>
/// <param name="StackHeight">
/// The operand-stack height the handler is entered at, before the thrown value is pushed. The
/// executor truncates the stack to this and pushes one value, so a throw from the middle of an
/// expression cannot leave a partial expression behind.
/// </param>
/// <param name="Kind">Whether the handler catches or rethrows.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=9A947E
// Broiler-Human:        PENDING
public readonly record struct JsExceptionRegionRow(
    uint FunctionIndex,
    uint TryStart,
    uint TryEnd,
    uint HandlerOffset,
    uint ScopeDepth,
    uint StackHeight,
    JsFormat.HandlerKind Kind);

/// <summary>One import entry: what a module binds one of its own names to.</summary>
/// <param name="RequestIndex">Which of the module's requested modules the binding comes from.</param>
/// <param name="NameConstant">
/// The constant holding the EXPORTED name being imported. It is unread and must be zero when
/// <paramref name="Kind"/> is a namespace import, because a namespace import names no export.
/// </param>
/// <param name="Kind">Whether one export is bound, or the requested module's namespace object.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=TBF
// Broiler-Human:        PENDING
public readonly record struct JsImportEntryRow(
    uint RequestIndex, uint NameConstant, JsFormat.ImportKind Kind);

/// <summary>One local export entry: a name this module exports out of a slot of its own.</summary>
/// <param name="NameConstant">The constant holding the exported name.</param>
/// <param name="Slot">
/// The slot of the module's own environment the name reads through. It is a SLOT and not a value,
/// which is what makes an export live: a later write by this module is seen by every importer.
/// </param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=TBF
// Broiler-Human:        PENDING
public readonly record struct JsLocalExportRow(uint NameConstant, uint Slot);

/// <summary>
/// One indirect export entry: a name this module exports out of a module it requested.
/// </summary>
/// <param name="NameConstant">The constant holding the name this module exports it under.</param>
/// <param name="RequestIndex">Which requested module supplies it.</param>
/// <param name="ImportNameConstant">
/// The constant holding the name it is exported under THERE, which need not be the same name. Zero
/// and unread when <paramref name="Kind"/> is a namespace re-export.
/// </param>
/// <param name="Kind">Whether one export is re-exported, or the whole namespace object.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=TBF
// Broiler-Human:        PENDING
public readonly record struct JsIndirectExportRow(
    uint NameConstant, uint RequestIndex, uint ImportNameConstant, JsFormat.ImportKind Kind);

/// <summary>One module record, as the artifact declares it.</summary>
/// <param name="KeyConstant">
/// The constant holding this module's resolved key. It is the composition's canonical name for the
/// module and is what a request is matched against; this format has no opinion about its shape.
/// </param>
/// <param name="UnitIndex">The code unit that is this module's body.</param>
/// <param name="InitialiserUnitIndex">
/// The code unit that initialises this module's environment. It runs for EVERY module of a graph
/// before ANY module's body does, which is what makes a function of a cyclic dependency callable
/// from a module that runs first.
/// </param>
/// <param name="RequestSpecifierConstants">
/// The specifiers this module names, in source order, exactly as the source wrote them. They are
/// carried so a running host can be asked whether it resolves them the way this artifact says.
/// </param>
/// <param name="RequestKeyConstants">
/// The keys of the modules this one requests, parallel to the specifiers. A request is a KEY and
/// not an index, so a producer cannot make a request point at a module by accident of table order.
/// </param>
/// <param name="Imports">This module's import entries, in source order.</param>
/// <param name="LocalExports">This module's local export entries.</param>
/// <param name="IndirectExports">This module's indirect export entries.</param>
/// <param name="StarExportRequests">
/// The requests this module re-exports every name of: one entry per <c>export * from</c>.
/// </param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=TBF
// Broiler-Human:        PENDING
public sealed record JsModuleRow(
    uint KeyConstant,
    uint UnitIndex,
    uint InitialiserUnitIndex,
    uint[] RequestSpecifierConstants,
    uint[] RequestKeyConstants,
    JsImportEntryRow[] Imports,
    JsLocalExportRow[] LocalExports,
    JsIndirectExportRow[] IndirectExports,
    uint[] StarExportRequests);

/// <summary>
/// Writes the byte layout of a format-version-2 artifact.
/// </summary>
/// <remarks>
/// It is the only writer of these bytes and it is deliberately dumb: it declares what it is told
/// to declare, including a count that disagrees with the number of items that follow. That is what
/// makes it usable to write the malformed inputs the verifier is judged against - a writer that
/// could only produce valid artifacts could not produce a single negative control.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=5F1BA6
// Broiler-Human:        PENDING
public static class JsArtifactWriter
{
    /// <summary>Assembles a whole artifact from already-encoded section bodies.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=101A52
    // Broiler-Human:        PENDING
    public static byte[] Write(
        string manifestId,
        JavaScriptArtifactWriter.Section[] sections,
        uint formatVersion = JsFormat.FormatVersion,
        uint? declaredSectionCount = null) =>
        JavaScriptArtifactWriter.Write(
            manifestId, sections, formatVersion, magic: null, declaredSectionCount);

    /// <summary>Encodes the limits section body.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=1CEC64
    // Broiler-Human:        PENDING
    public static byte[] Limits(
        uint maxOperandStack, uint maxScopeSlots, uint maxFunctions, uint maxConstants)
    {
        var buffer = new System.Collections.Generic.List<byte>();
        JavaScriptArtifactWriter.WriteVarUInt(buffer, maxOperandStack);
        JavaScriptArtifactWriter.WriteVarUInt(buffer, maxScopeSlots);
        JavaScriptArtifactWriter.WriteVarUInt(buffer, maxFunctions);
        JavaScriptArtifactWriter.WriteVarUInt(buffer, maxConstants);
        return buffer.ToArray();
    }

    /// <summary>Encodes the constant-pool section body.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=D51064
    // Broiler-Human:        PENDING
    public static byte[] Constants(byte[][] entries, uint? declaredCount = null)
    {
        var buffer = new System.Collections.Generic.List<byte>();
        JavaScriptArtifactWriter.WriteVarUInt(buffer, declaredCount ?? (ulong)entries.Length);

        foreach (var entry in entries)
        {
            buffer.AddRange(entry);
        }

        return buffer.ToArray();
    }

    /// <summary>The <c>undefined</c> constant.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=A92EF7
    // Broiler-Human:        PENDING
    public static byte[] UndefinedConstant() => [(byte)JsFormat.ConstantTag.Undefined];

    /// <summary>The <c>null</c> constant.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=24BA3C
    // Broiler-Human:        PENDING
    public static byte[] NullConstant() => [(byte)JsFormat.ConstantTag.Null];

    /// <summary>A Boolean constant.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=C291A0
    // Broiler-Human:        PENDING
    public static byte[] BooleanConstant(bool value) =>
        [(byte)JsFormat.ConstantTag.Boolean, value ? (byte)1 : (byte)0];

    /// <summary>A Number constant.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=611868
    // Broiler-Human:        PENDING
    public static byte[] NumberConstant(double value)
    {
        var bytes = new byte[9];
        bytes[0] = (byte)JsFormat.ConstantTag.Number;
        System.Buffers.Binary.BinaryPrimitives.WriteDoubleLittleEndian(
            System.MemoryExtensions.AsSpan(bytes, 1), value);
        return bytes;
    }

    /// <summary>A String value constant.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=458BE1
    // Broiler-Human:        PENDING
    public static byte[] StringConstant(string value) =>
        Text(JsFormat.ConstantTag.String, value);

    /// <summary>An interned property-name constant.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=65C159
    // Broiler-Human:        PENDING
    public static byte[] InternedNameConstant(string name) =>
        Text(JsFormat.ConstantTag.InternedName, name);

    /// <summary>Encodes the entries section body: a name and the code unit it starts.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=805F19
    // Broiler-Human:        PENDING
    public static byte[] Entries((string Name, uint FunctionIndex)[] entries, uint? declaredCount = null)
    {
        var buffer = new System.Collections.Generic.List<byte>();
        JavaScriptArtifactWriter.WriteVarUInt(buffer, declaredCount ?? (ulong)entries.Length);

        foreach (var (name, function) in entries)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(name);
            JavaScriptArtifactWriter.WriteVarUInt(buffer, (ulong)bytes.Length);
            buffer.AddRange(bytes);
            JavaScriptArtifactWriter.WriteVarUInt(buffer, function);
        }

        return buffer.ToArray();
    }

    /// <summary>Encodes the functions section body.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=BB9D6A
    // Broiler-Human:        PENDING
    public static byte[] Functions(JsFunctionRow[] rows, uint? declaredCount = null)
    {
        var buffer = new System.Collections.Generic.List<byte>();
        JavaScriptArtifactWriter.WriteVarUInt(buffer, declaredCount ?? (ulong)rows.Length);

        foreach (var row in rows)
        {
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.NameConstant);
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.ParameterCount);
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.ScopeSlots);
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.MaxOperandStack);
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.CodeOffset);
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.CodeLength);
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.Flags);
        }

        return buffer.ToArray();
    }

    /// <summary>Encodes the exception-region section body.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=7BBEFE
    // Broiler-Human:        PENDING
    public static byte[] ExceptionRegions(JsExceptionRegionRow[] rows, uint? declaredCount = null)
    {
        var buffer = new System.Collections.Generic.List<byte>();
        JavaScriptArtifactWriter.WriteVarUInt(buffer, declaredCount ?? (ulong)rows.Length);

        foreach (var row in rows)
        {
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.FunctionIndex);
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.TryStart);
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.TryEnd);
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.HandlerOffset);
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.ScopeDepth);
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.StackHeight);
            buffer.Add((byte)row.Kind);
        }

        return buffer.ToArray();
    }

    /// <summary>Encodes the module-record section body.</summary>
    /// <remarks>
    /// <b>The section carries what the source DECLARED and not what it resolves to.</b> Which slot
    /// of which module an import ends up reading is the answer to a walk over star and indirect
    /// exports, and a producer that wrote its own answer here would be asking the verifier to trust
    /// a resolution it did not perform. So the entries stay in the shape the language states them
    /// in - a request is a key, an indirect export names a request and a name - and resolving them
    /// is the verifier's, executing nothing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=TBF
    // Broiler-Human:        PENDING
    public static byte[] Modules(JsModuleRow[] rows, uint? declaredCount = null)
    {
        var buffer = new System.Collections.Generic.List<byte>();
        JavaScriptArtifactWriter.WriteVarUInt(buffer, declaredCount ?? (ulong)rows.Length);

        foreach (var row in rows)
        {
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.KeyConstant);
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.UnitIndex);
            JavaScriptArtifactWriter.WriteVarUInt(buffer, row.InitialiserUnitIndex);

            JavaScriptArtifactWriter.WriteVarUInt(buffer, (ulong)row.RequestKeyConstants.Length);

            for (var request = 0; request < row.RequestKeyConstants.Length; request++)
            {
                JavaScriptArtifactWriter.WriteVarUInt(buffer, row.RequestSpecifierConstants[request]);
                JavaScriptArtifactWriter.WriteVarUInt(buffer, row.RequestKeyConstants[request]);
            }

            JavaScriptArtifactWriter.WriteVarUInt(buffer, (ulong)row.Imports.Length);

            foreach (var entry in row.Imports)
            {
                JavaScriptArtifactWriter.WriteVarUInt(buffer, entry.RequestIndex);
                JavaScriptArtifactWriter.WriteVarUInt(buffer, entry.NameConstant);
                buffer.Add((byte)entry.Kind);
            }

            JavaScriptArtifactWriter.WriteVarUInt(buffer, (ulong)row.LocalExports.Length);

            foreach (var entry in row.LocalExports)
            {
                JavaScriptArtifactWriter.WriteVarUInt(buffer, entry.NameConstant);
                JavaScriptArtifactWriter.WriteVarUInt(buffer, entry.Slot);
            }

            JavaScriptArtifactWriter.WriteVarUInt(buffer, (ulong)row.IndirectExports.Length);

            foreach (var entry in row.IndirectExports)
            {
                JavaScriptArtifactWriter.WriteVarUInt(buffer, entry.NameConstant);
                JavaScriptArtifactWriter.WriteVarUInt(buffer, entry.RequestIndex);
                JavaScriptArtifactWriter.WriteVarUInt(buffer, entry.ImportNameConstant);
                buffer.Add((byte)entry.Kind);
            }

            JavaScriptArtifactWriter.WriteVarUInt(buffer, (ulong)row.StarExportRequests.Length);

            foreach (var request in row.StarExportRequests)
            {
                JavaScriptArtifactWriter.WriteVarUInt(buffer, request);
            }
        }

        return buffer.ToArray();
    }

    /// <summary>Encodes the position-table section body.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=7B7840
    // Broiler-Human:        PENDING
    public static byte[] Positions((uint Offset, uint Line, uint Column)[] rows, uint? declaredCount = null) =>
        JavaScriptArtifactWriter.Positions(rows, declaredCount);

    /// <summary>Appends one instruction with no operand.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=CBED9A
    // Broiler-Human:        PENDING
    public static void Emit(System.Collections.Generic.List<byte> code, JsOpcode opcode) =>
        code.Add((byte)opcode);

    /// <summary>Appends one instruction carrying a <c>u8</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=897227
    // Broiler-Human:        PENDING
    public static void Emit(System.Collections.Generic.List<byte> code, JsOpcode opcode, byte operand)
    {
        code.Add((byte)opcode);
        code.Add(operand);
    }

    /// <summary>Appends one instruction carrying a <c>u16</c>, little-endian.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=5121CC
    // Broiler-Human:        PENDING
    public static void Emit(System.Collections.Generic.List<byte> code, JsOpcode opcode, ushort operand)
    {
        code.Add((byte)opcode);
        code.Add((byte)(operand & 0xFF));
        code.Add((byte)(operand >> 8));
    }

    /// <summary>Appends one instruction carrying a <c>u8</c> then a <c>u16</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=28A630
    // Broiler-Human:        PENDING
    public static void Emit(
        System.Collections.Generic.List<byte> code, JsOpcode opcode, byte first, ushort second)
    {
        code.Add((byte)opcode);
        code.Add(first);
        code.Add((byte)(second & 0xFF));
        code.Add((byte)(second >> 8));
    }

    /// <summary>
    /// Appends one branch carrying an absolute <c>u32</c> code offset, and answers where the
    /// operand was written so a forward branch can be patched once its target is known.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=CA9C07
    // Broiler-Human:        PENDING
    public static int EmitBranch(
        System.Collections.Generic.List<byte> code, JsOpcode opcode, uint target = 0)
    {
        code.Add((byte)opcode);
        var at = code.Count;

        for (var shift = 0; shift < 32; shift += 8)
        {
            code.Add((byte)((target >> shift) & 0xFF));
        }

        return at;
    }

    /// <summary>Overwrites a target written earlier by <see cref="EmitBranch"/>.</summary>
    /// <remarks>
    /// The operand is the ABSOLUTE offset of the target instruction within the code section, which
    /// is the convention the verifier and the executor both read it under. Version 1 wrote a
    /// displacement; sharing one code section between many code units makes an absolute offset the
    /// only form a verifier can check against the containing unit's declared range without first
    /// working out which unit the branch came from.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=8E9EF7
    // Broiler-Human:        PENDING
    public static void PatchBranch(System.Collections.Generic.List<byte> code, int at, uint target)
    {
        for (var shift = 0; shift < 32; shift += 8)
        {
            code[at + (shift / 8)] = (byte)((target >> shift) & 0xFF);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=4D25A8
    // Broiler-Human:        PENDING
    private static byte[] Text(JsFormat.ConstantTag tag, string value)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(value);
        var buffer = new System.Collections.Generic.List<byte>(bytes.Length + 5) { (byte)tag };
        JavaScriptArtifactWriter.WriteVarUInt(buffer, (ulong)bytes.Length);
        buffer.AddRange(bytes);
        return buffer.ToArray();
    }
}
