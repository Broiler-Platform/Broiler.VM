using System.Collections.Immutable;
using Broiler.VM;
using Broiler.VM.Ubc;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// <see cref="UbcInstructionTable.TryCreate"/> refuses each violation of the family table schema with
/// a defect naming it, and admits the corpus family's two tables. A table is a family's input to a
/// composition root, so none of this is reachable from an artifact.
/// </summary>
public sealed class UbcInstructionTableTests
{
    private static readonly ImmutableArray<UbcSlotType> None = ImmutableArray<UbcSlotType>.Empty;
    private static readonly ImmutableArray<UbcSlotType> I32 = [UbcSlotType.I32];
    private static readonly ImmutableArray<UbcSlotType> I32I32 = [UbcSlotType.I32, UbcSlotType.I32];

    private static UbcInstructionRow Dynamic(byte opcode = 0x40, UbcOperandShape shape = UbcOperandShape.None) =>
        new(opcode, "test.dynamic", shape, UbcEffect.Listed(None, None), UbcTarget.None, UbcInstructionKind.Dynamic);

    private static UbcInstructionRow Primitive(
        UbcPrimitive primitive,
        UbcEffect effect,
        UbcOperandShape shape = UbcOperandShape.None,
        byte region = 0,
        ImmutableArray<UbcTrapMapping> traps = default,
        bool isTerminal = false) =>
        new(0x41, "test.primitive", shape, effect, UbcTarget.None, UbcInstructionKind.Primitive, isTerminal: isTerminal,
            primitive: primitive, region: region, traps: traps);

    public static TheoryData<string, string> Violations() => new()
    {
        { "an-identity-the-grammar-refuses", "family identity" },
        { "no-manifest", "feature manifest" },
        { "a-trap-code-declared-twice", "declared twice" },
        { "a-region-declared-twice", "declared twice" },
        { "a-region-kind-declared-twice", "declared twice" },
        { "a-landing-push-that-is-no-slot-type", "not a slot type" },
        { "a-landing-push-list-never-given", "not a slot type" },
        { "an-opcode-with-two-rows", "two rows" },
        { "an-empty-mnemonic", "mnemonic" },
        { "a-shape-outside-the-closed-set", "closed set" },
        { "no-effect", "must both be given" },
        { "no-target", "must both be given" },
        { "a-kind-outside-the-closed-set", "closed set" },
        { "a-cost-of-zero", "fuel unit" },
        { "a-counted-effect-on-a-u32-operand", "U8 or U16" },
        { "a-code-target-on-a-u16-operand", "U32 operand" },
        { "a-branch-with-no-code-target", "branch row names a code target" },
        { "a-code-target-on-a-dynamic-row", "only a branch row" },
        { "a-terminal-branch", "not terminal" },
        { "a-dynamic-row-naming-a-primitive", "only a primitive row" },
        { "a-dynamic-row-naming-a-trap-mapping", "only a primitive row" },
        { "a-dynamic-row-naming-a-region", "only a primitive row" },
        { "a-primitive-row-with-no-primitive", "entry of the primitive table" },
        { "a-primitive-row-naming-no-entry", "entry of the primitive table" },
        { "a-primitive-row-with-a-counted-effect", "fixed effect" },
        { "a-terminal-primitive-row", "fixed effect" },
        { "a-word-keep-that-changes-the-type", "word.keep" },
        { "a-word-keep-of-a-value", "word.keep" },
        { "an-effect-that-is-not-the-signature", "not the primitive's signature" },
        { "a-region-access-of-an-undeclared-region", "region the table declares" },
        { "a-region-load-with-no-offset-operand", "static offset" },
        { "a-region-size-with-a-u32-operand", "static offset" },
        { "a-region-on-a-primitive-that-reads-none", "only a region primitive" },
        { "a-mapping-for-a-trap-the-primitive-cannot-raise", "cannot raise" },
        { "a-mapping-of-two-traps-at-once", "cannot raise" },
        { "a-trap-mapped-twice", "mapped twice" },
        { "a-mapping-outside-the-trap-vocabulary", "not in the table's trap vocabulary" },
        { "a-trap-left-unmapped", "is not" },
        { "a-missing-row", "missing" },
        { "a-missing-region-kind", "missing" },
        { "a-missing-region", "missing" },
        { "a-missing-trap", "missing" },
    };

    [Theory]
    [MemberData(nameof(Violations))]
    public void A_Schema_Violation_Is_Refused_With_A_Defect_Naming_It(string violation, string defectMentions)
    {
        var identity = UbcCorpusFamily.Identity;
        var manifest = UbcCorpusFamily.BaseManifest;
        var rows = new List<UbcInstructionRow>();
        var kinds = new List<UbcRegionKindRow> { new(1, "catch", [UbcSlotType.V]) };
        var regions = new List<UbcRegionDeclaration> { new(0, "memory0") };
        var traps = new List<UbcFamilyTrap> { new(1, "divide-by-zero"), new(2, "overflow"), new(3, "bounds") };
        var divTraps = ImmutableArray.Create(new UbcTrapMapping(UbcTrapCode.DivideByZero, 1), new UbcTrapMapping(UbcTrapCode.IntegerOverflow, 2));

        switch (violation)
        {
            case "an-identity-the-grammar-refuses": identity = "not an identity"; break;
            case "no-manifest": manifest = default; break;
            case "a-trap-code-declared-twice": traps.Add(new(1, "again")); break;
            case "a-region-declared-twice": regions.Add(new(0, "again")); break;
            case "a-region-kind-declared-twice": kinds.Add(new(1, "again", [])); break;
            case "a-landing-push-that-is-no-slot-type": kinds.Add(new(2, "odd", [(UbcSlotType)9])); break;
            case "a-landing-push-list-never-given": kinds.Add(new(2, "odd", default)); break;
            case "an-opcode-with-two-rows": rows.Add(Dynamic()); rows.Add(Dynamic()); break;
            case "an-empty-mnemonic":
                rows.Add(new(0x40, string.Empty, UbcOperandShape.None, UbcEffect.Listed(None, None), UbcTarget.None, UbcInstructionKind.Dynamic));
                break;
            case "a-shape-outside-the-closed-set": rows.Add(Dynamic(shape: (UbcOperandShape)12)); break;
            case "no-effect":
                rows.Add(new(0x40, "test.x", UbcOperandShape.None, null!, UbcTarget.None, UbcInstructionKind.Dynamic));
                break;
            case "no-target":
                rows.Add(new(0x40, "test.x", UbcOperandShape.None, UbcEffect.Listed(None, None), null!, UbcInstructionKind.Dynamic));
                break;
            case "a-kind-outside-the-closed-set":
                rows.Add(new(0x40, "test.x", UbcOperandShape.None, UbcEffect.Listed(None, None), UbcTarget.None, (UbcInstructionKind)6));
                break;
            case "a-cost-of-zero":
                rows.Add(new(0x40, "test.x", UbcOperandShape.None, UbcEffect.Listed(None, None), UbcTarget.None, UbcInstructionKind.Dynamic, cost: 0));
                break;
            case "a-counted-effect-on-a-u32-operand":
                rows.Add(new(0x40, "test.x", UbcOperandShape.U32, UbcEffect.Counted(None, UbcSlotType.V, 1, None), UbcTarget.None, UbcInstructionKind.Dynamic));
                break;
            case "a-code-target-on-a-u16-operand":
                rows.Add(new(0x40, "test.x", UbcOperandShape.U16, UbcEffect.Listed(None, None), UbcTarget.Code(), UbcInstructionKind.Branch));
                break;
            case "a-branch-with-no-code-target":
                rows.Add(new(0x40, "test.x", UbcOperandShape.U32, UbcEffect.Listed(None, None), UbcTarget.None, UbcInstructionKind.Branch));
                break;
            case "a-code-target-on-a-dynamic-row":
                rows.Add(new(0x40, "test.x", UbcOperandShape.U32, UbcEffect.Listed(None, None), UbcTarget.Code(), UbcInstructionKind.Dynamic));
                break;
            case "a-terminal-branch":
                rows.Add(new(0x40, "test.x", UbcOperandShape.U32, UbcEffect.Listed(None, None), UbcTarget.Code(), UbcInstructionKind.Branch, isTerminal: true));
                break;
            case "a-dynamic-row-naming-a-primitive":
                rows.Add(new(0x40, "test.x", UbcOperandShape.None, UbcEffect.Listed(I32I32, I32), UbcTarget.None, UbcInstructionKind.Dynamic,
                    primitive: UbcPrimitive.I32Add));
                break;
            case "a-dynamic-row-naming-a-trap-mapping":
                rows.Add(new(0x40, "test.x", UbcOperandShape.None, UbcEffect.Listed(None, None), UbcTarget.None, UbcInstructionKind.Dynamic,
                    traps: [new(UbcTrapCode.DivideByZero, 1)]));
                break;
            case "a-dynamic-row-naming-a-region":
                rows.Add(new(0x40, "test.x", UbcOperandShape.None, UbcEffect.Listed(None, None), UbcTarget.None, UbcInstructionKind.Dynamic, region: 1));
                break;
            case "a-primitive-row-with-no-primitive":
                rows.Add(new(0x41, "test.x", UbcOperandShape.None, UbcEffect.Listed(I32I32, I32), UbcTarget.None, UbcInstructionKind.Primitive));
                break;
            case "a-primitive-row-naming-no-entry": rows.Add(Primitive((UbcPrimitive)999, UbcEffect.Listed(I32I32, I32))); break;
            case "a-primitive-row-with-a-counted-effect":
                rows.Add(Primitive(UbcPrimitive.I32Add, UbcEffect.Counted(None, UbcSlotType.I32, 2, I32), UbcOperandShape.U8));
                break;
            case "a-terminal-primitive-row": rows.Add(Primitive(UbcPrimitive.I32Add, UbcEffect.Listed(I32I32, I32), isTerminal: true)); break;
            case "a-word-keep-that-changes-the-type":
                rows.Add(Primitive(UbcPrimitive.WordKeep, UbcEffect.Listed(I32, [UbcSlotType.I64])));
                break;
            case "a-word-keep-of-a-value": rows.Add(Primitive(UbcPrimitive.WordKeep, UbcEffect.Listed([UbcSlotType.V], [UbcSlotType.V]))); break;
            case "an-effect-that-is-not-the-signature": rows.Add(Primitive(UbcPrimitive.I32Add, UbcEffect.Listed(I32I32, [UbcSlotType.I64]))); break;
            case "a-region-access-of-an-undeclared-region":
                rows.Add(Primitive(UbcPrimitive.RegionLoadI32, UbcEffect.Listed(I32, I32), UbcOperandShape.U32, region: 7,
                    traps: [new(UbcTrapCode.OutOfBounds, 3)]));
                break;
            case "a-region-load-with-no-offset-operand":
                rows.Add(Primitive(UbcPrimitive.RegionLoadI32, UbcEffect.Listed(I32, I32), UbcOperandShape.U16,
                    traps: [new(UbcTrapCode.OutOfBounds, 3)]));
                break;
            case "a-region-size-with-a-u32-operand": rows.Add(Primitive(UbcPrimitive.RegionSize, UbcEffect.Listed(None, I32), UbcOperandShape.U32)); break;
            case "a-region-on-a-primitive-that-reads-none": rows.Add(Primitive(UbcPrimitive.I32Add, UbcEffect.Listed(I32I32, I32), region: 1)); break;
            case "a-mapping-for-a-trap-the-primitive-cannot-raise":
                rows.Add(Primitive(UbcPrimitive.I32Add, UbcEffect.Listed(I32I32, I32), traps: [new(UbcTrapCode.DivideByZero, 1)]));
                break;
            case "a-mapping-of-two-traps-at-once":
                rows.Add(Primitive(UbcPrimitive.I32DivS, UbcEffect.Listed(I32I32, I32),
                    traps: [new(UbcTrapCode.DivideByZero | UbcTrapCode.IntegerOverflow, 1)]));
                break;
            case "a-trap-mapped-twice":
                rows.Add(Primitive(UbcPrimitive.I32DivS, UbcEffect.Listed(I32I32, I32), traps: divTraps.Add(new(UbcTrapCode.DivideByZero, 3))));
                break;
            case "a-mapping-outside-the-trap-vocabulary":
                rows.Add(Primitive(UbcPrimitive.I32DivU, UbcEffect.Listed(I32I32, I32), traps: [new(UbcTrapCode.DivideByZero, 77)]));
                break;
            case "a-trap-left-unmapped":
                rows.Add(Primitive(UbcPrimitive.I32DivS, UbcEffect.Listed(I32I32, I32), traps: [divTraps[0]]));
                break;
            case "a-missing-row": rows.Add(null!); break;
            case "a-missing-region-kind": kinds.Add(null!); break;
            case "a-missing-region": regions.Add(null!); break;
            case "a-missing-trap": traps.Add(null!); break;
            default: throw new ArgumentOutOfRangeException(nameof(violation), violation, "no such case");
        }

        var created = UbcInstructionTable.TryCreate(
            identity, 1, manifest, rows, kinds, regions, traps, canonicaliseNaN: false, out var table, out var defect);

        Assert.False(created, $"{violation} was admitted");
        Assert.Null(table);
        Assert.NotNull(defect);
        Assert.Contains(defectMentions, defect, StringComparison.Ordinal);
    }

    [Fact]
    public void A_Missing_List_Is_Refused_With_A_Defect_Rather_Than_Thrown()
    {
        var manifest = UbcCorpusFamily.BaseManifest;

        foreach (var missing in Enumerable.Range(0, 4))
        {
            var created = UbcInstructionTable.TryCreate(
                UbcCorpusFamily.Identity, 1, manifest,
                missing == 0 ? null! : [],
                missing == 1 ? null! : [],
                missing == 2 ? null! : [],
                missing == 3 ? null! : [],
                canonicaliseNaN: false, out var table, out var defect);

            Assert.False(created);
            Assert.Null(table);
            Assert.Contains("missing", defect, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void The_Corpus_Family_Tables_Are_Admitted_Ordered_And_Searchable()
    {
        var tables = UbcCorpusFamily.Tables();
        var baseTable = tables[0];
        var wide = tables[1];

        Assert.Equal(UbcCorpusFamily.BaseTableVersion, baseTable.TableVersion);
        Assert.Equal(UbcCorpusFamily.WideManifest, wide.Manifest);
        Assert.True(baseTable.CanonicaliseNaN);
        Assert.False(wide.CanonicaliseNaN);
        Assert.Equal(baseTable.Rows.Length + 1, wide.Rows.Length);
        Assert.Equal(baseTable.Rows.Select(static row => row.Opcode).Order(), baseTable.Rows.Select(static row => row.Opcode));

        Assert.True(wide.TryGetRow(UbcCorpusFamily.Op.WideOnly, out _));
        Assert.False(baseTable.TryGetRow(UbcCorpusFamily.Op.WideOnly, out _));
        Assert.False(baseTable.TryGetRow(0xFF, out _));
        Assert.True(baseTable.TryGetRegionKind(UbcCorpusFamily.FinallyKind, out var finallyKind));
        Assert.Equal(new[] { UbcSlotType.V, UbcSlotType.I32 }, finallyKind.LandingPushes);
        Assert.False(baseTable.TryGetRegionKind(0, out _));
        Assert.True(baseTable.DefinesTrap(UbcCorpusFamily.TrapUser));
        Assert.False(baseTable.DefinesTrap(0));
    }

    [Fact]
    public void Every_Row_Of_The_Common_Table_Is_Appendix_A()
    {
        // U4's one table, read back: every byte Appendix A defines and no other, with its shape.
        var expected = new Dictionary<UbcOpcode, UbcOperandShape>
        {
            [UbcOpcode.Nop] = UbcOperandShape.None, [UbcOpcode.Trap] = UbcOperandShape.U8U16, [UbcOpcode.Jump] = UbcOperandShape.U32,
            [UbcOpcode.JumpIfZero] = UbcOperandShape.U32, [UbcOpcode.JumpIfNonZero] = UbcOperandShape.U32,
            [UbcOpcode.JumpTable] = UbcOperandShape.U16, [UbcOpcode.Return] = UbcOperandShape.None, [UbcOpcode.Call] = UbcOperandShape.U32,
            [UbcOpcode.Drop] = UbcOperandShape.None, [UbcOpcode.Dup] = UbcOperandShape.None, [UbcOpcode.Dup2] = UbcOperandShape.None,
            [UbcOpcode.Swap] = UbcOperandShape.None, [UbcOpcode.Pick] = UbcOperandShape.U8, [UbcOpcode.Select] = UbcOperandShape.None,
            [UbcOpcode.Squash] = UbcOperandShape.U8U8, [UbcOpcode.LocalGet] = UbcOperandShape.U16, [UbcOpcode.LocalSet] = UbcOperandShape.U16,
            [UbcOpcode.LocalTee] = UbcOperandShape.U16, [UbcOpcode.ConstI32] = UbcOperandShape.I32, [UbcOpcode.ConstI64] = UbcOperandShape.I64,
            [UbcOpcode.ConstF32] = UbcOperandShape.F32, [UbcOpcode.ConstF64] = UbcOperandShape.F64,
        };

        Assert.Equal(expected.Count, UbcOpcodes.All.Length);

        for (var value = 0; value < 256; value++)
        {
            var defined = UbcOpcodes.TryDescribe((byte)value, out var row);
            Assert.Equal(expected.ContainsKey((UbcOpcode)value), defined);

            if (defined)
            {
                Assert.Equal(expected[(UbcOpcode)value], row.Shape);
                Assert.Equal(1u, row.Cost);
                Assert.Equal(row.IsTerminal, row.Opcode is UbcOpcode.Trap or UbcOpcode.Jump or UbcOpcode.JumpTable or UbcOpcode.Return);
            }
        }
    }
}
