// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   23
// Annotated:        23/23
// Exempt:           1
// Human-reviewed:   0/23
// IP risk:          Low
// Security risk:    High
// Criteria:         6/6
// Resource impact:  1/10 max
// Unverified:       23
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM;
using Broiler.VM.Ubc;
using System.Collections.Immutable;

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The WebAssembly family's one instruction table: every row a translation of a WebAssembly module
/// writes that is not a row of the common family, keyed by the family opcode byte.
/// </summary>
/// <remarks>
/// <para>
/// <b>One table, under the one manifest.</b> Decision WAD-0002 keeps the table under
/// <c>broiler.webassembly.slice</c>, the manifest identity this profile already allocates, and mints
/// no second one; the table's version moves whenever a row changes.
/// </para>
/// <para>
/// <b>The rows.</b> The hundred and twenty-three numeric, comparison and conversion instructions are
/// primitive rows at their own W3C bytes, 0x45 to 0xBF, each naming the primitive of the same
/// operation, with the primitive's signature as its effect and a mapping for every trap
/// <see cref="UbcPrimitives.TrapsOf"/> says the primitive can raise - read from that member rather
/// than written here, so a row can never map a trap its primitive does not raise or miss one it
/// does. The twenty-three loads and stores are region primitives over <c>memory0</c> at 0x28 to
/// 0x3E, their operand the alignment exponent and the static offset; <c>memory.size</c> is the
/// region-size primitive at 0x3F and <c>memory.grow</c> a dynamic row at 0x40, each with the
/// reserved byte as its operand. <c>call_indirect</c> is a call row at 0x11 whose effect is the
/// signature form: its operand names the module's type, and it pops that type's parameters under
/// the i32 table index. The globals are eight typed dynamic rows at 0xE0 to 0xE7, a get and a set for
/// each of the four word types, because the region rows Appendix C names for them cannot be written
/// against the primitive table. Everything else a module says lowers to the common family.
/// </para>
/// <para>
/// <b>The trap vocabulary is <see cref="WasmTrapKind"/>'s</b>, code for value, so a trap's family code
/// is the kind a payload carries; <see cref="WasmTrapKind.UndefinedElement"/> is declared and no row
/// raises it, as the enumeration itself records.
/// </para>
/// <para>
/// This type depends on no static state of <see cref="WebAssemblyProfile"/>: the profile's
/// registration is built from this table while the profile's own statics are being initialised, so
/// the identity and the manifest are spelled here rather than read from there.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=E00481
// Broiler-Falsified-If: a numeric row names a primitive of another operation than its W3C byte's, or a row maps a trap its primitive cannot raise
// Broiler-Human:        PENDING
internal static class WasmFamilyTable
{
    /// <summary>The family's identity: the profile's.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=9F3696
    // Broiler-Human:        PENDING
    internal const string Identity = "broiler.webassembly";

    /// <summary>The manifest that selects the table.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=AF1DAB
    // Broiler-Human:        PENDING
    internal const string ManifestIdentity = "broiler.webassembly.slice";

    /// <summary>The table's version: it moves whenever a row changes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=02279F
    // Broiler-Human:        PENDING
    internal const uint TableVersion = 1;

    /// <summary>The family slot a translation puts the family in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=296103
    // Broiler-Human:        PENDING
    internal const byte Slot = 1;

    /// <summary>The index of the one region, <c>memory0</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=363D8B
    // Broiler-Human:        PENDING
    internal const byte MemoryRegion = 0;

    /// <summary><c>call_indirect</c>: a call row of the signature form.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=BC866F
    // Broiler-Human:        PENDING
    internal const byte CallIndirect = 0x11;

    /// <summary>The first load, <c>i32.load</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=934FBE
    // Broiler-Human:        PENDING
    internal const byte FirstAccess = 0x28;

    /// <summary>The last store, <c>i64.store32</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=54FA57
    // Broiler-Human:        PENDING
    internal const byte LastAccess = 0x3E;

    /// <summary><c>memory.size</c>: the region-size primitive over <c>memory0</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=05BBB6
    // Broiler-Human:        PENDING
    internal const byte MemorySize = 0x3F;

    /// <summary><c>memory.grow</c>: a dynamic row.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=DC6A2D
    // Broiler-Human:        PENDING
    internal const byte MemoryGrow = 0x40;

    /// <summary>The first numeric row, <c>i32.eqz</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7F920C
    // Broiler-Human:        PENDING
    internal const byte FirstNumeric = 0x45;

    /// <summary>The last numeric row, <c>f64.reinterpret_i64</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=411748
    // Broiler-Human:        PENDING
    internal const byte LastNumeric = 0xBF;

    /// <summary>
    /// The first global row, <c>global.get.i32</c>. The eight rows run get then set for i32, i64,
    /// f32 and f64 in that order, so a row's type is its distance from here halved, and a set is an odd
    /// distance.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A17E82
    // Broiler-Human:        PENDING
    internal const byte FirstGlobal = 0xE0;

    /// <summary>The last global row, <c>global.set.f64</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=33E227
    // Broiler-Human:        PENDING
    internal const byte LastGlobal = 0xE7;

    /// <summary>Whether <paramref name="opcode"/> is one of the eight global rows.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C013A0
    // Broiler-Human:        PENDING
    internal static bool IsGlobal(byte opcode) => opcode is >= FirstGlobal and <= LastGlobal;

    /// <summary>Whether a global row sets its global rather than reading it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=90D5B0
    // Broiler-Human:        PENDING
    internal static bool IsGlobalSet(byte opcode) => ((opcode - FirstGlobal) & 1) == 1;

    /// <summary>The type of the global a global row reads or writes, in the module definitions' own coding.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F86ECD
    // Broiler-Human:        PENDING
    internal static WasmGlobalKind GlobalTypeOf(byte opcode) => (WasmGlobalKind)((opcode - FirstGlobal) >> 1);

    /// <summary>The slot type of a global of <paramref name="type"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=DC510B
    // Broiler-Human:        PENDING
    internal static UbcSlotType SlotOf(WasmGlobalKind type) => type switch
    {
        WasmGlobalKind.I32 => UbcSlotType.I32,
        WasmGlobalKind.I64 => UbcSlotType.I64,
        WasmGlobalKind.F32 => UbcSlotType.F32,
        _ => UbcSlotType.F64,
    };

    /// <summary>The family code a universal trap maps to: the <see cref="WasmTrapKind"/> of the same meaning.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=70B886
    // Broiler-Falsified-If: a universal trap maps to a kind of another meaning, so a payload names the wrong trap
    // Broiler-Human:        PENDING
    internal static ushort FamilyCodeOf(UbcTrapCode universal) => universal switch
    {
        UbcTrapCode.DivideByZero => (ushort)WasmTrapKind.IntegerDivideByZero,
        UbcTrapCode.IntegerOverflow => (ushort)WasmTrapKind.IntegerOverflow,
        UbcTrapCode.InvalidConversion => (ushort)WasmTrapKind.InvalidConversionToInteger,
        UbcTrapCode.OutOfBounds => (ushort)WasmTrapKind.OutOfBoundsMemoryAccess,
        _ => 0,
    };

    /// <summary>
    /// The primitive rows, in byte order: the byte, the primitive of the same operation, and the
    /// operation's W3C name, which the row's mnemonic prefixes with <c>wasm.</c>.
    /// </summary>
    /// <remarks>
    /// This list is the whole of what is stated by hand: which primitive a byte is. Each row's effect,
    /// operand shape, region and trap mappings are computed from the primitive table, and the table's
    /// constructor refuses a row whose effect is not its primitive's signature.
    /// </remarks>
    // Broiler-AI:           Origin=Specification; IP=Low; Security=High; Resources=1; Fingerprint=2C03D0
    // Broiler-Falsified-If: a byte is paired with a primitive whose operation is not the W3C instruction of that byte
    // Broiler-Human:        PENDING
    private static readonly (byte Opcode, UbcPrimitive Primitive, string Name)[] Primitives =
    [
        (0x28, UbcPrimitive.RegionLoadI32, "i32.load"),
        (0x29, UbcPrimitive.RegionLoadI64, "i64.load"),
        (0x2A, UbcPrimitive.RegionLoadF32, "f32.load"),
        (0x2B, UbcPrimitive.RegionLoadF64, "f64.load"),
        (0x2C, UbcPrimitive.RegionLoadI32From8S, "i32.load8_s"),
        (0x2D, UbcPrimitive.RegionLoadI32From8U, "i32.load8_u"),
        (0x2E, UbcPrimitive.RegionLoadI32From16S, "i32.load16_s"),
        (0x2F, UbcPrimitive.RegionLoadI32From16U, "i32.load16_u"),
        (0x30, UbcPrimitive.RegionLoadI64From8S, "i64.load8_s"),
        (0x31, UbcPrimitive.RegionLoadI64From8U, "i64.load8_u"),
        (0x32, UbcPrimitive.RegionLoadI64From16S, "i64.load16_s"),
        (0x33, UbcPrimitive.RegionLoadI64From16U, "i64.load16_u"),
        (0x34, UbcPrimitive.RegionLoadI64From32S, "i64.load32_s"),
        (0x35, UbcPrimitive.RegionLoadI64From32U, "i64.load32_u"),
        (0x36, UbcPrimitive.RegionStoreI32, "i32.store"),
        (0x37, UbcPrimitive.RegionStoreI64, "i64.store"),
        (0x38, UbcPrimitive.RegionStoreF32, "f32.store"),
        (0x39, UbcPrimitive.RegionStoreF64, "f64.store"),
        (0x3A, UbcPrimitive.RegionStoreI32To8, "i32.store8"),
        (0x3B, UbcPrimitive.RegionStoreI32To16, "i32.store16"),
        (0x3C, UbcPrimitive.RegionStoreI64To8, "i64.store8"),
        (0x3D, UbcPrimitive.RegionStoreI64To16, "i64.store16"),
        (0x3E, UbcPrimitive.RegionStoreI64To32, "i64.store32"),
        (0x3F, UbcPrimitive.RegionSize, "memory.size"),
        (0x45, UbcPrimitive.I32Eqz, "i32.eqz"),
        (0x46, UbcPrimitive.I32Eq, "i32.eq"),
        (0x47, UbcPrimitive.I32Ne, "i32.ne"),
        (0x48, UbcPrimitive.I32LtS, "i32.lt_s"),
        (0x49, UbcPrimitive.I32LtU, "i32.lt_u"),
        (0x4A, UbcPrimitive.I32GtS, "i32.gt_s"),
        (0x4B, UbcPrimitive.I32GtU, "i32.gt_u"),
        (0x4C, UbcPrimitive.I32LeS, "i32.le_s"),
        (0x4D, UbcPrimitive.I32LeU, "i32.le_u"),
        (0x4E, UbcPrimitive.I32GeS, "i32.ge_s"),
        (0x4F, UbcPrimitive.I32GeU, "i32.ge_u"),
        (0x50, UbcPrimitive.I64Eqz, "i64.eqz"),
        (0x51, UbcPrimitive.I64Eq, "i64.eq"),
        (0x52, UbcPrimitive.I64Ne, "i64.ne"),
        (0x53, UbcPrimitive.I64LtS, "i64.lt_s"),
        (0x54, UbcPrimitive.I64LtU, "i64.lt_u"),
        (0x55, UbcPrimitive.I64GtS, "i64.gt_s"),
        (0x56, UbcPrimitive.I64GtU, "i64.gt_u"),
        (0x57, UbcPrimitive.I64LeS, "i64.le_s"),
        (0x58, UbcPrimitive.I64LeU, "i64.le_u"),
        (0x59, UbcPrimitive.I64GeS, "i64.ge_s"),
        (0x5A, UbcPrimitive.I64GeU, "i64.ge_u"),
        (0x5B, UbcPrimitive.F32Eq, "f32.eq"),
        (0x5C, UbcPrimitive.F32Ne, "f32.ne"),
        (0x5D, UbcPrimitive.F32Lt, "f32.lt"),
        (0x5E, UbcPrimitive.F32Gt, "f32.gt"),
        (0x5F, UbcPrimitive.F32Le, "f32.le"),
        (0x60, UbcPrimitive.F32Ge, "f32.ge"),
        (0x61, UbcPrimitive.F64Eq, "f64.eq"),
        (0x62, UbcPrimitive.F64Ne, "f64.ne"),
        (0x63, UbcPrimitive.F64Lt, "f64.lt"),
        (0x64, UbcPrimitive.F64Gt, "f64.gt"),
        (0x65, UbcPrimitive.F64Le, "f64.le"),
        (0x66, UbcPrimitive.F64Ge, "f64.ge"),
        (0x67, UbcPrimitive.I32Clz, "i32.clz"),
        (0x68, UbcPrimitive.I32Ctz, "i32.ctz"),
        (0x69, UbcPrimitive.I32Popcnt, "i32.popcnt"),
        (0x6A, UbcPrimitive.I32Add, "i32.add"),
        (0x6B, UbcPrimitive.I32Sub, "i32.sub"),
        (0x6C, UbcPrimitive.I32Mul, "i32.mul"),
        (0x6D, UbcPrimitive.I32DivS, "i32.div_s"),
        (0x6E, UbcPrimitive.I32DivU, "i32.div_u"),
        (0x6F, UbcPrimitive.I32RemS, "i32.rem_s"),
        (0x70, UbcPrimitive.I32RemU, "i32.rem_u"),
        (0x71, UbcPrimitive.I32And, "i32.and"),
        (0x72, UbcPrimitive.I32Or, "i32.or"),
        (0x73, UbcPrimitive.I32Xor, "i32.xor"),
        (0x74, UbcPrimitive.I32Shl, "i32.shl"),
        (0x75, UbcPrimitive.I32ShrS, "i32.shr_s"),
        (0x76, UbcPrimitive.I32ShrU, "i32.shr_u"),
        (0x77, UbcPrimitive.I32Rotl, "i32.rotl"),
        (0x78, UbcPrimitive.I32Rotr, "i32.rotr"),
        (0x79, UbcPrimitive.I64Clz, "i64.clz"),
        (0x7A, UbcPrimitive.I64Ctz, "i64.ctz"),
        (0x7B, UbcPrimitive.I64Popcnt, "i64.popcnt"),
        (0x7C, UbcPrimitive.I64Add, "i64.add"),
        (0x7D, UbcPrimitive.I64Sub, "i64.sub"),
        (0x7E, UbcPrimitive.I64Mul, "i64.mul"),
        (0x7F, UbcPrimitive.I64DivS, "i64.div_s"),
        (0x80, UbcPrimitive.I64DivU, "i64.div_u"),
        (0x81, UbcPrimitive.I64RemS, "i64.rem_s"),
        (0x82, UbcPrimitive.I64RemU, "i64.rem_u"),
        (0x83, UbcPrimitive.I64And, "i64.and"),
        (0x84, UbcPrimitive.I64Or, "i64.or"),
        (0x85, UbcPrimitive.I64Xor, "i64.xor"),
        (0x86, UbcPrimitive.I64Shl, "i64.shl"),
        (0x87, UbcPrimitive.I64ShrS, "i64.shr_s"),
        (0x88, UbcPrimitive.I64ShrU, "i64.shr_u"),
        (0x89, UbcPrimitive.I64Rotl, "i64.rotl"),
        (0x8A, UbcPrimitive.I64Rotr, "i64.rotr"),
        (0x8B, UbcPrimitive.F32Abs, "f32.abs"),
        (0x8C, UbcPrimitive.F32Neg, "f32.neg"),
        (0x8D, UbcPrimitive.F32Ceil, "f32.ceil"),
        (0x8E, UbcPrimitive.F32Floor, "f32.floor"),
        (0x8F, UbcPrimitive.F32Trunc, "f32.trunc"),
        (0x90, UbcPrimitive.F32Nearest, "f32.nearest"),
        (0x91, UbcPrimitive.F32Sqrt, "f32.sqrt"),
        (0x92, UbcPrimitive.F32Add, "f32.add"),
        (0x93, UbcPrimitive.F32Sub, "f32.sub"),
        (0x94, UbcPrimitive.F32Mul, "f32.mul"),
        (0x95, UbcPrimitive.F32Div, "f32.div"),
        (0x96, UbcPrimitive.F32Min, "f32.min"),
        (0x97, UbcPrimitive.F32Max, "f32.max"),
        (0x98, UbcPrimitive.F32Copysign, "f32.copysign"),
        (0x99, UbcPrimitive.F64Abs, "f64.abs"),
        (0x9A, UbcPrimitive.F64Neg, "f64.neg"),
        (0x9B, UbcPrimitive.F64Ceil, "f64.ceil"),
        (0x9C, UbcPrimitive.F64Floor, "f64.floor"),
        (0x9D, UbcPrimitive.F64Trunc, "f64.trunc"),
        (0x9E, UbcPrimitive.F64Nearest, "f64.nearest"),
        (0x9F, UbcPrimitive.F64Sqrt, "f64.sqrt"),
        (0xA0, UbcPrimitive.F64Add, "f64.add"),
        (0xA1, UbcPrimitive.F64Sub, "f64.sub"),
        (0xA2, UbcPrimitive.F64Mul, "f64.mul"),
        (0xA3, UbcPrimitive.F64Div, "f64.div"),
        (0xA4, UbcPrimitive.F64Min, "f64.min"),
        (0xA5, UbcPrimitive.F64Max, "f64.max"),
        (0xA6, UbcPrimitive.F64Copysign, "f64.copysign"),
        (0xA7, UbcPrimitive.I32WrapI64, "i32.wrap_i64"),
        (0xA8, UbcPrimitive.I32TruncF32S, "i32.trunc_f32_s"),
        (0xA9, UbcPrimitive.I32TruncF32U, "i32.trunc_f32_u"),
        (0xAA, UbcPrimitive.I32TruncF64S, "i32.trunc_f64_s"),
        (0xAB, UbcPrimitive.I32TruncF64U, "i32.trunc_f64_u"),
        (0xAC, UbcPrimitive.I64ExtendI32S, "i64.extend_i32_s"),
        (0xAD, UbcPrimitive.I64ExtendI32U, "i64.extend_i32_u"),
        (0xAE, UbcPrimitive.I64TruncF32S, "i64.trunc_f32_s"),
        (0xAF, UbcPrimitive.I64TruncF32U, "i64.trunc_f32_u"),
        (0xB0, UbcPrimitive.I64TruncF64S, "i64.trunc_f64_s"),
        (0xB1, UbcPrimitive.I64TruncF64U, "i64.trunc_f64_u"),
        (0xB2, UbcPrimitive.F32ConvertI32S, "f32.convert_i32_s"),
        (0xB3, UbcPrimitive.F32ConvertI32U, "f32.convert_i32_u"),
        (0xB4, UbcPrimitive.F32ConvertI64S, "f32.convert_i64_s"),
        (0xB5, UbcPrimitive.F32ConvertI64U, "f32.convert_i64_u"),
        (0xB6, UbcPrimitive.F32DemoteF64, "f32.demote_f64"),
        (0xB7, UbcPrimitive.F64ConvertI32S, "f64.convert_i32_s"),
        (0xB8, UbcPrimitive.F64ConvertI32U, "f64.convert_i32_u"),
        (0xB9, UbcPrimitive.F64ConvertI64S, "f64.convert_i64_s"),
        (0xBA, UbcPrimitive.F64ConvertI64U, "f64.convert_i64_u"),
        (0xBB, UbcPrimitive.F64PromoteF32, "f64.promote_f32"),
        (0xBC, UbcPrimitive.I32ReinterpretF32, "i32.reinterpret_f32"),
        (0xBD, UbcPrimitive.I64ReinterpretF64, "i64.reinterpret_f64"),
        (0xBE, UbcPrimitive.F32ReinterpretI32, "f32.reinterpret_i32"),
        (0xBF, UbcPrimitive.F64ReinterpretI64, "f64.reinterpret_i64"),
    ];

    /// <summary>The table, built once, after the primitive list it reads, since static initialisers run in the order they are written; a row the schema refuses is a defect of this type and throws with the row named.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=76C1E4
    // Broiler-Falsified-If: a table the schema refused is answered rather than thrown
    // Broiler-Human:        PENDING
    internal static UbcInstructionTable Table { get; } = Build();

    /// <summary>Builds the rows and hands them to the table's constructor, which checks every one against the schema.</summary>
    // Broiler-AI:           Origin=AI; Spec=ADR-0013; IP=Low; Security=High; Resources=1; Fingerprint=0F6E49
    // Broiler-Falsified-If: a row reaches the table without passing the constructor's schema checks, or a numeric row's trap mappings are not the primitive's own trap set
    // Broiler-Human:        PENDING
    private static UbcInstructionTable Build()
    {
        var rows = new System.Collections.Generic.List<UbcInstructionRow>(Primitives.Length + 10);

        foreach (var (opcode, primitive, name) in Primitives)
        {
            UbcPrimitives.TryGetSignature(primitive, out var operands, out var results);

            var region = UbcPrimitives.IsRegionAccess(primitive);
            var shape = primitive == UbcPrimitive.RegionSize
                ? UbcOperandShape.U8
                : region ? UbcOperandShape.U8U32 : UbcOperandShape.None;

            rows.Add(new UbcInstructionRow(
                opcode,
                "wasm." + name,
                shape,
                UbcEffect.Listed(operands, results),
                UbcTarget.None,
                UbcInstructionKind.Primitive,
                primitive: primitive,
                region: MemoryRegion,
                traps: TrapsFor(primitive)));
        }

        var none = ImmutableArray<UbcSlotType>.Empty;
        var i32 = ImmutableArray.Create(UbcSlotType.I32);

        rows.Add(new UbcInstructionRow(
            MemoryGrow,
            "wasm.memory.grow",
            UbcOperandShape.U8,
            UbcEffect.Listed(i32, i32),
            UbcTarget.None,
            UbcInstructionKind.Dynamic));

        rows.Add(new UbcInstructionRow(
            CallIndirect,
            "wasm.call_indirect",
            UbcOperandShape.U32,
            UbcEffect.Signature(i32),
            UbcTarget.None,
            UbcInstructionKind.Call));

        foreach (var type in (System.ReadOnlySpan<WasmGlobalKind>)[WasmGlobalKind.I32, WasmGlobalKind.I64, WasmGlobalKind.F32, WasmGlobalKind.F64])
        {
            var slot = ImmutableArray.Create(SlotOf(type));
            var get = (byte)(FirstGlobal + (2 * (int)type));
            var suffix = UbcSlotTypes.Name(SlotOf(type));

            rows.Add(new UbcInstructionRow(get, "wasm.global.get." + suffix, UbcOperandShape.U32, UbcEffect.Listed(none, slot), UbcTarget.None, UbcInstructionKind.Dynamic));
            rows.Add(new UbcInstructionRow((byte)(get + 1), "wasm.global.set." + suffix, UbcOperandShape.U32, UbcEffect.Listed(slot, none), UbcTarget.None, UbcInstructionKind.Dynamic));
        }

        UbcFamilyTrap[] traps =
        [
            new((ushort)WasmTrapKind.Unreachable, "unreachable"),
            new((ushort)WasmTrapKind.IntegerDivideByZero, "integer divide by zero"),
            new((ushort)WasmTrapKind.IntegerOverflow, "integer overflow"),
            new((ushort)WasmTrapKind.InvalidConversionToInteger, "invalid conversion to integer"),
            new((ushort)WasmTrapKind.OutOfBoundsMemoryAccess, "out of bounds memory access"),
            new((ushort)WasmTrapKind.OutOfBoundsTableAccess, "out of bounds table access"),
            new((ushort)WasmTrapKind.UndefinedElement, "undefined element"),
            new((ushort)WasmTrapKind.IndirectCallTypeMismatch, "indirect call type mismatch"),
            new((ushort)WasmTrapKind.UninitializedElement, "uninitialized element"),
        ];

        if (!UbcInstructionTable.TryCreate(
                Identity,
                TableVersion,
                VmFeatureManifestId.Parse(ManifestIdentity),
                rows,
                [],
                [new UbcRegionDeclaration(MemoryRegion, "memory0")],
                traps,
                canonicaliseNaN: true,
                out var table,
                out var defect))
        {
            throw new System.InvalidOperationException("the WebAssembly family table is refused by the schema: " + defect);
        }

        return table!;
    }

    /// <summary>
    /// The mappings of every trap <paramref name="primitive"/> can raise, read from
    /// <see cref="UbcPrimitives.TrapsOf"/>, each to the family code of the same meaning.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=658D26
    // Broiler-Falsified-If: a trap the primitive can raise is left unmapped, or one it cannot raise is mapped
    // Broiler-Human:        PENDING
    private static ImmutableArray<UbcTrapMapping> TrapsFor(UbcPrimitive primitive)
    {
        var raised = UbcPrimitives.TrapsOf(primitive);
        var mappings = ImmutableArray.CreateBuilder<UbcTrapMapping>();

        foreach (var universal in (System.ReadOnlySpan<UbcTrapCode>)[UbcTrapCode.DivideByZero, UbcTrapCode.IntegerOverflow, UbcTrapCode.InvalidConversion, UbcTrapCode.OutOfBounds])
        {
            if ((raised & universal) != 0)
            {
                mappings.Add(new UbcTrapMapping(universal, FamilyCodeOf(universal)));
            }
        }

        return mappings.ToImmutable();
    }
}
