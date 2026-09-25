using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using Broiler.VM;
using Broiler.VM.Fixtures;
using Broiler.VM.Ubc;
using F = Broiler.VM.Contract.Tests.UbcCorpusFamily;
using O = Broiler.VM.Ubc.UbcOpcode;
using Op = Broiler.VM.Contract.Tests.UbcCorpusFamily.Op;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// The seeded universal bytecode corpus: what every artifact is, how it is presented, and what it must
/// answer, declared beside the bytes and never derived from a run.
/// </summary>
/// <remarks>
/// <para>
/// <b>The expectation is written by hand</b>, the position included. Where a position is an offset into
/// the payload it is computed from the artifact's layout - where a section or an identity begins - and
/// never from the verifier: a position read back from the verifier would agree with any change.
/// </para>
/// <para>
/// <b>Controls come first.</b> A corpus in which everything fails cannot tell a verifier that
/// classifies correctly from one that refuses whatever it is handed, so the controls are many and
/// between them use every common opcode, every family row, every operand shape, both effect forms and
/// all three target forms, and the runner checks that they do.
/// </para>
/// <para>
/// A few configurations are measured rather than stated - the allowance that runs out just after the
/// reader - because the point of those entries is where the exhaustion happens, not the number; the
/// answer they pin is still written by hand.
/// </para>
/// </remarks>
internal static class UbcCorpus
{
    private const UbcSlotType I32 = UbcSlotType.I32;
    private const UbcSlotType I64 = UbcSlotType.I64;
    private const UbcSlotType F32 = UbcSlotType.F32;
    private const UbcSlotType F64 = UbcSlotType.F64;
    private const UbcSlotType V = UbcSlotType.V;
    private const UbcUnitFlags Entry = UbcUnitFlags.Entry;
    private const UbcUnitFlags Suspends = UbcUnitFlags.Suspendable;

    /// <summary>The canonical control the two sweeps are derived from.</summary>
    internal static byte[] Canonical() => Smallest().Bytes();

    /// <summary>Every seeded entry, in a stable order.</summary>
    internal static IReadOnlyList<UbcCorpusEntry> Entries()
    {
        var entries = new List<UbcCorpusEntry>();

        AddControls(entries);
        AddHeader(entries);
        AddFraming(entries);
        AddFamilies(entries);
        AddTypesAndUnits(entries);
        AddWalk(entries);
        AddRegions(entries);
        AddTablesEntriesAndPositions(entries);
        AddForm(entries);
        AddHook(entries);
        AddCeilings(entries);
        AddSweeps(entries);

        return entries;
    }

    /// <summary>The control descriptions, by name, for the tests that read a control's structure.</summary>
    internal static IReadOnlyList<(string Id, UbcCorpusSpec Spec, UbcCorpusConfiguration Configuration)> Controls() =>
    [
        ("control-the-smallest-artifact-that-verifies", Smallest(), UbcCorpusConfiguration.Default),
        ("control-every-common-opcode", EveryCommonOpcode(), UbcCorpusConfiguration.Default),
        ("control-every-family-row", EveryFamilyRow(), UbcCorpusConfiguration.Default),
        ("control-regions-with-landings", RegionsWithLandings(), UbcCorpusConfiguration.Default),
        ("control-a-suspendable-unit-resumes-at-its-landings", SuspendableUnit(), UbcCorpusConfiguration.Default),
        ("control-jump-tables", JumpTables(), UbcCorpusConfiguration.Default),
        ("control-calls-between-units", CallsBetweenUnits(), UbcCorpusConfiguration.Default),
        ("control-locals-of-both-planes", LocalsOfBothPlanes(), UbcCorpusConfiguration.Default),
        ("control-word-and-value-slots", WordAndValueSlots(), UbcCorpusConfiguration.Default),
        ("control-entries-and-positions", EntriesAndPositions(), UbcCorpusConfiguration.Default),
        ("control-the-wide-manifest-selects-its-own-table", WideManifest(), new UbcCorpusConfiguration(descriptorManifest: F.WideManifestText)),
        ("control-a-family-in-the-highest-slot", HighestSlot(), UbcCorpusConfiguration.Default),
        ("control-optional-sections-present-and-empty", EmptyOptionalSections(), UbcCorpusConfiguration.Default),
        ("control-an-entry-name-of-the-longest-length-the-format-admits", LongestEntryName(), UbcCorpusConfiguration.Default),
        ("control-a-trap-the-family-vocabulary-defines", FamilyTrap(), UbcCorpusConfiguration.Default),
    ];

    // ---- the controls ------------------------------------------------------------------------------

    private static void AddControls(List<UbcCorpusEntry> entries)
    {
        var notes = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["control-the-smallest-artifact-that-verifies"] =
                "One unit, no family: const.i32 then return. The canonical control both sweeps are cut from.",
            ["control-every-common-opcode"] =
                "Every row of Appendix A in two units of no family: every jump form, a jump table, a call, every shuffle, locals of three word types and a reachable trap.",
            ["control-every-family-row"] =
                "Every row of the base table in one suspendable unit: every operand shape, both effect forms with a multiplier above one, all three target forms, every instruction kind including three region primitives.",
            ["control-regions-with-landings"] =
                "Two nested regions of the two region kinds, inner listed first, each landing pushing its kind's slots onto a value prefix; and a region whose entry prefix is a word.",
            ["control-a-suspendable-unit-resumes-at-its-landings"] =
                "Two suspending rows whose resume points are landings, inside a region whose handler is the third landing.",
            ["control-jump-tables"] =
                "Three jump tables of two units; a target shared by two tables, a default row, and a table of one row.",
            ["control-calls-between-units"] =
                "Calls between four units whose signatures have parameters and results of both planes, a family unit calling a unit of no family.",
            ["control-locals-of-both-planes"] =
                "Parameters and local runs of both planes, a run of length zero between two others, and every local instruction on each plane.",
            ["control-word-and-value-slots"] =
                "dup2, swap, pick, select and squash over stacks mixing words and values.",
            ["control-entries-and-positions"] =
                "Three entries listed out of byte order, two naming one unit, one name not ASCII; positions at both ends of both units and a coordinate at the core's maximum.",
            ["control-the-wide-manifest-selects-its-own-table"] =
                "The wide manifest selects table version 2, whose extra row the base table lacks.",
            ["control-a-family-in-the-highest-slot"] =
                "The family in slot fourteen: prefix byte 0xFE and the last FamilyData section kind.",
            ["control-optional-sections-present-and-empty"] =
                "Families, JumpTables, Regions and Positions present with no rows. Absent and empty are different bytes and both verify.",
            ["control-an-entry-name-of-the-longest-length-the-format-admits"] =
                "An entry name of exactly 1024 bytes, the longest the format admits.",
            ["control-a-trap-the-family-vocabulary-defines"] =
                "A trap naming the family's slot and a code of its trap vocabulary, reached on one path.",
        };

        foreach (var (id, spec, configuration) in Controls())
        {
            entries.Add(Verifies(id, "control", spec.Bytes(), notes[id], configuration));
        }

        var canonical = Canonical();

        entries.Add(Verifies(
            "control-an-artifact-exactly-at-its-requested-ceiling", "control", canonical,
            "The canonical control presented under an artifact-bytes request equal to its own length. The ceiling is inclusive.",
            new UbcCorpusConfiguration(artifactBytesRequest: (ulong)canonical.Length)));
    }

    internal static UbcCorpusSpec Smallest()
    {
        var spec = new UbcCorpusSpec();
        var type = spec.AddType([], [I32]);
        var unit = spec.AddUnit(type, 0, 1, 0, Entry, b =>
        {
            b.Emit(O.ConstI32, 42).Emit(O.Return);
            return [];
        });

        spec.AddEntry("main", unit);
        return spec;
    }

    /// <summary>One family unit in <paramref name="slot"/>: push a value, return it.</summary>
    internal static UbcCorpusSpec SmallestWithFamily(byte slot = 1)
    {
        var spec = new UbcCorpusSpec().WithFamily(slot);
        var type = spec.AddType([], [V]);
        var unit = spec.AddUnit(type, slot, 0, 1, Entry, b =>
        {
            b.F(Op.PushSmall, 1, slot).Emit(O.Return);
            return [];
        });

        spec.AddEntry("main", unit);
        return spec;
    }

    /// <summary>One entry unit of the given signature and code, with the family declared when <paramref name="slot"/> is not zero.</summary>
    private static UbcCorpusSpec OneUnit(
        UbcSlotType[] parameters,
        UbcSlotType[] results,
        byte slot,
        uint maxWords,
        uint maxValues,
        Func<UbcCodeBuilder, IEnumerable<uint>> body,
        UbcUnitFlags flags = Entry,
        params UbcLocalRun[] locals)
    {
        var spec = new UbcCorpusSpec();

        if (slot != 0)
        {
            spec.WithFamily(slot);
        }

        var type = spec.AddType(parameters, results);
        var unit = spec.AddUnit(type, slot, maxWords, maxValues, flags, body, locals);
        spec.AddEntry("main", unit);
        return spec;
    }

    private static UbcCorpusSpec EveryCommonOpcode()
    {
        var spec = new UbcCorpusSpec();
        var type = spec.AddType([I32], [I32]);
        uint first = 0, second = 0;

        var main = spec.AddUnit(type, 0, 4, 0, Entry, b =>
        {
            int zero = b.NewLabel(), call = b.NewLabel(), rowOne = b.NewLabel(), rowTwo = b.NewLabel(), done = b.NewLabel();

            b.Emit(O.Nop).Emit(O.LocalGet, 0).EmitTo(O.JumpIfZero, zero)
                .Emit(O.ConstI32, 7).Emit(O.ConstI64, 9).Emit(O.LocalSet, 1)
                .Emit(O.Dup).Emit(O.Dup2).Emit(O.Swap).Emit(O.Drop).Emit(O.Pick, 2).Emit(O.Select)
                .Emit(O.Squash, UbcCorpusCode.Pair(UbcOperandShape.U8U8, 1, 1))
                .Emit(O.ConstF32, UbcCorpusCode.Bits(1.0f)).Emit(O.Drop)
                .Emit(O.ConstF64, UbcCorpusCode.Bits(2.0)).Emit(O.LocalTee, 2).Emit(O.Drop)
                .Emit(O.LocalTee, 0).Emit(O.JumpTable, 0);
            b.Mark(zero).Emit(O.ConstI32, 1).EmitTo(O.JumpIfNonZero, call).Emit(O.Trap, UbcCorpusCode.Pair(UbcOperandShape.U8U16, 0, 0));
            b.Mark(call).Emit(O.ConstI32, 5).Emit(O.Call, 1).Emit(O.Return);
            first = b.Here(rowOne);
            b.Emit(O.ConstI32, 0).EmitTo(O.Jump, done);
            second = b.Here(rowTwo);
            b.Emit(O.ConstI32, 1);
            b.Mark(done).Emit(O.Return);
            return [];
        }, new UbcLocalRun(1, I64), new UbcLocalRun(1, F64));

        spec.AddUnit(type, 0, 1, 0, UbcUnitFlags.None, b =>
        {
            b.Emit(O.LocalGet, 0).Emit(O.Return);
            return [];
        });

        spec.JumpTables = [new UbcJumpTable((uint)main, [first, second])];
        spec.AddEntry("main", main);
        return spec;
    }

    internal static UbcCorpusSpec EveryFamilyRow()
    {
        var spec = new UbcCorpusSpec().WithFamily();
        spec.FamilyData[1] = [0, 1, 2];
        var type = spec.AddType([], [V]);

        var unit = spec.AddUnit(type, 1, 3, 4, Suspends | Entry, b =>
        {
            int done = b.NewLabel(), end = b.NewLabel();

            b.F(Op.Nop).F(Op.PushSmall, 3).F(Op.LoadConstant, 2)
                .F(Op.ConstPair, UbcCorpusCode.Pair(UbcOperandShape.U16U16, 1, 2))
                .F(Op.ScopeGet, UbcCorpusCode.Pair(UbcOperandShape.U8U16, 0, 5))
                .F(Op.GetSlot, UbcCorpusCode.Pair(UbcOperandShape.U8U8, 1, 2))
                .F(Op.MakeTemplate, 1).F(Op.CallValue, 1).F(Op.Await);
            var resume = b.Offset;
            b.F(Op.PushI32, 5).F(Op.PushI32, 3).F(Op.I32Add).F(Op.PushI32, 2).F(Op.I32DivS)
                .F(Op.LoadI32, 4).F(Op.PushI64, 7).F(Op.KeepI64).F(Op.StoreI64, UbcCorpusCode.Pair(UbcOperandShape.U8U32, 0, 8))
                .F(Op.MemorySize).F(Op.PushF64, UbcCorpusCode.Bits(1.5)).F(Op.PushF64, UbcCorpusCode.Bits(2.5)).F(Op.F64Add)
                .F(Op.Trunc).F(Op.I32Add).F(Op.PushF32, UbcCorpusCode.Bits(1.0f)).Emit(O.Drop)
                .F(Op.FromI32).F(Op.ToI32).Emit(O.Drop)
                .FTo(Op.IfTrue, done).FTo(Op.Iterate, end).F(Op.Throw);
            b.Mark(done).Emit(O.Return);
            b.Mark(end).F(Op.PushSmall, 0).Emit(O.Return);
            return [resume];
        });

        spec.AddEntry("main", unit);
        return spec;
    }

    private static UbcCorpusSpec RegionsWithLandings()
    {
        var spec = new UbcCorpusSpec().WithFamily();
        var values = spec.AddType([], [V]);
        var words = spec.AddType([], [I32]);
        uint start = 0, innerEnd = 0, catchAt = 0, finallyAt = 0;

        var outer = spec.AddUnit(values, 1, 1, 2, Entry, b =>
        {
            int handlerOne = b.NewLabel(), handlerTwo = b.NewLabel(), exit = b.NewLabel();

            b.F(Op.PushSmall, 1);
            start = b.Offset;
            b.F(Op.PushSmall, 2).F(Op.CallValue, 0).Emit(O.Drop);
            innerEnd = b.Offset;
            b.EmitTo(O.Jump, exit);
            catchAt = b.Here(handlerOne);
            b.Emit(O.Drop).EmitTo(O.Jump, exit);
            finallyAt = b.Here(handlerTwo);
            b.Emit(O.Drop).Emit(O.Drop);
            b.Mark(exit).Emit(O.Return);
            return [catchAt, finallyAt];
        });

        uint covered = 0, handler = 0;

        var wordPrefix = spec.AddUnit(words, 1, 1, 1, UbcUnitFlags.None, b =>
        {
            var landing = b.NewLabel();

            b.Emit(O.ConstI32, 4);
            covered = b.Offset;
            b.F(Op.PushSmall, 1).F(Op.Throw);
            handler = b.Here(landing);
            b.Emit(O.Drop).Emit(O.Return);
            return [handler];
        });

        spec.Regions =
        [
            new UbcRegion((uint)outer, start, innerEnd, catchAt, 0, 1, F.CatchKind),
            new UbcRegion((uint)outer, start, catchAt, finallyAt, 0, 1, F.FinallyKind),
            new UbcRegion((uint)wordPrefix, covered, handler, handler, 1, 0, F.CatchKind),
        ];

        spec.AddEntry("main", outer);
        return spec;
    }

    internal static UbcCorpusSpec SuspendableUnit() => SuspendableUnit(out _, out _, out _);

    private static UbcCorpusSpec SuspendableUnit(out uint first, out uint second, out uint handler)
    {
        var spec = new UbcCorpusSpec().WithFamily();
        var type = spec.AddType([], [V]);
        uint start = 0, one = 0, two = 0, landing = 0;

        var unit = spec.AddUnit(type, 1, 0, 1, Suspends | Entry, b =>
        {
            var label = b.NewLabel();

            b.F(Op.PushSmall, 1);
            start = b.Offset;
            b.F(Op.Await);
            one = b.Offset;
            b.F(Op.Await);
            two = b.Offset;
            b.Emit(O.Return);
            landing = b.Here(label);
            b.Emit(O.Return);
            return [one, two, landing];
        });

        spec.Regions = [new UbcRegion((uint)unit, start, two, landing, 0, 0, F.CatchKind)];
        spec.AddEntry("main", unit);
        first = one;
        second = two;
        handler = landing;
        return spec;
    }

    private static UbcCorpusSpec JumpTables()
    {
        var spec = new UbcCorpusSpec();
        var chooser = spec.AddType([I32], [I32]);
        var constant = spec.AddType([], [I32]);
        uint a = 0, b1 = 0, c = 0, d = 0, x = 0;

        var main = spec.AddUnit(chooser, 0, 1, 0, Entry, b =>
        {
            int l0 = b.NewLabel(), l1 = b.NewLabel(), l2 = b.NewLabel(), l3 = b.NewLabel(), end = b.NewLabel();

            b.Emit(O.LocalGet, 0).Emit(O.JumpTable, 0);
            a = b.Here(l0);
            b.Emit(O.ConstI32, 10).EmitTo(O.Jump, end);
            b1 = b.Here(l1);
            b.Emit(O.ConstI32, 20).EmitTo(O.Jump, end);
            c = b.Here(l2);
            b.Emit(O.LocalGet, 0).Emit(O.JumpTable, 1);
            d = b.Here(l3);
            b.Emit(O.ConstI32, 30);
            b.Mark(end).Emit(O.Return);
            return [];
        });

        var single = spec.AddUnit(constant, 0, 1, 0, Entry, b =>
        {
            var only = b.NewLabel();

            b.Emit(O.ConstI32, 1).Emit(O.JumpTable, 2);
            x = b.Here(only);
            b.Emit(O.ConstI32, 2).Emit(O.Return);
            return [];
        });

        spec.JumpTables =
        [
            new UbcJumpTable((uint)main, [a, b1, c]),
            new UbcJumpTable((uint)main, [a, d]),
            new UbcJumpTable((uint)single, [x]),
        ];

        spec.AddEntry("main", main);
        spec.AddEntry("single", single);
        return spec;
    }

    private static UbcCorpusSpec CallsBetweenUnits()
    {
        var spec = new UbcCorpusSpec().WithFamily();
        var main = spec.AddType([], [I32]);
        var pair = spec.AddType([I32, V], [V, I32]);
        var one = spec.AddType([V], [V]);

        var entry = spec.AddUnit(main, 1, 2, 1, Entry, b =>
        {
            b.Emit(O.ConstI32, 3).F(Op.PushSmall, 1).Emit(O.Call, 1).Emit(O.Swap).Emit(O.Call, 2).Emit(O.Drop)
                .Emit(O.Call, 3).F(Op.I32Add).Emit(O.Return);
            return [];
        });

        spec.AddUnit(pair, 1, 1, 1, UbcUnitFlags.None, b =>
        {
            b.Emit(O.LocalGet, 1).Emit(O.LocalGet, 0).Emit(O.Return);
            return [];
        });

        spec.AddUnit(one, 1, 0, 1, UbcUnitFlags.None, b =>
        {
            b.Emit(O.LocalGet, 0).Emit(O.Return);
            return [];
        });

        spec.AddUnit(main, 0, 1, 0, UbcUnitFlags.None, b =>
        {
            b.Emit(O.ConstI32, 9).Emit(O.Return);
            return [];
        });

        spec.AddEntry("main", entry);
        return spec;
    }

    internal static UbcCorpusSpec LocalsOfBothPlanes()
    {
        var spec = new UbcCorpusSpec().WithFamily();
        var type = spec.AddType([V, I64], [V]);

        var unit = spec.AddUnit(type, 1, 1, 1, Entry, b =>
        {
            b.Emit(O.LocalGet, 0).Emit(O.LocalSet, 2).Emit(O.LocalGet, 2).Emit(O.LocalTee, 3)
                .Emit(O.LocalGet, 1).Emit(O.Drop)
                .Emit(O.ConstI32, 1).Emit(O.LocalSet, 4)
                .Emit(O.ConstF64, UbcCorpusCode.Bits(0.5)).Emit(O.LocalTee, 6).Emit(O.LocalSet, 5)
                .Emit(O.Return);
            return [];
        }, new UbcLocalRun(2, V), new UbcLocalRun(1, I32), new UbcLocalRun(0, F32), new UbcLocalRun(2, F64));

        spec.AddEntry("main", unit);
        return spec;
    }

    private static UbcCorpusSpec WordAndValueSlots() =>
        OneUnit([], [V], 1, 3, 4, b =>
        {
            b.F(Op.PushSmall, 1).Emit(O.ConstI32, 2).Emit(O.Dup2).Emit(O.Swap).Emit(O.Pick, 3)
                .F(Op.PushSmall, 2).Emit(O.ConstI32, 0).Emit(O.Select)
                .Emit(O.Squash, UbcCorpusCode.Pair(UbcOperandShape.U8U8, 3, 1))
                .Emit(O.Dup).Emit(O.Drop)
                .Emit(O.Squash, UbcCorpusCode.Pair(UbcOperandShape.U8U8, 1, 1))
                .Emit(O.Return);
            return [];
        });

    private static UbcCorpusSpec EntriesAndPositions()
    {
        var spec = new UbcCorpusSpec();
        var type = spec.AddType([], [I32]);
        uint a0 = 0, a1 = 0, b0 = 0, b1 = 0;

        var first = spec.AddUnit(type, 0, 1, 0, Entry, b =>
        {
            a0 = b.Offset;
            b.Emit(O.ConstI32, 1);
            a1 = b.Offset;
            b.Emit(O.Return);
            return [];
        });

        var second = spec.AddUnit(type, 0, 1, 0, Entry, b =>
        {
            b0 = b.Offset;
            b.Emit(O.ConstI32, 2);
            b1 = b.Offset;
            b.Emit(O.Return);
            return [];
        });

        spec.AddEntry("second", second);
        spec.AddEntry("main", first);
        spec.AddEntry("grüße", second);
        spec.Positions =
        [
            new UbcPosition((uint)first, a0, 1, 1),
            new UbcPosition((uint)first, a1, 1, 9),
            new UbcPosition((uint)second, b0, 2, 1),
            new UbcPosition((uint)second, b1, int.MaxValue, 0),
        ];

        return spec;
    }

    private static UbcCorpusSpec WideManifest()
    {
        var spec = new UbcCorpusSpec { Manifest = F.WideManifestText }.WithFamily(version: F.WideTableVersion);
        var type = spec.AddType([], [V]);
        var unit = spec.AddUnit(type, 1, 0, 1, Entry, b =>
        {
            b.F(Op.WideOnly).F(Op.PushSmall, 1).Emit(O.Return);
            return [];
        });

        spec.AddEntry("main", unit);
        return spec;
    }

    private static UbcCorpusSpec HighestSlot()
    {
        var spec = new UbcCorpusSpec().WithFamily(UbcFormat.MaxFamilySlot);
        spec.FamilyData[UbcFormat.MaxFamilySlot] = [3];
        var type = spec.AddType([], [V]);
        var unit = spec.AddUnit(type, UbcFormat.MaxFamilySlot, 0, 1, Entry, b =>
        {
            b.F(Op.LoadConstant, 0, UbcFormat.MaxFamilySlot).Emit(O.Return);
            return [];
        });

        spec.AddEntry("main", unit);
        return spec;
    }

    private static UbcCorpusSpec EmptyOptionalSections()
    {
        var spec = Smallest();
        spec.Families = [];
        spec.JumpTables = [];
        spec.Regions = [];
        spec.Positions = [];
        return spec;
    }

    private static UbcCorpusSpec LongestEntryName()
    {
        var spec = Smallest();
        spec.Entries!.Clear();
        spec.AddEntry(new string('n', UbcFormat.MaxEntryNameBytes), 0);
        return spec;
    }

    private static UbcCorpusSpec FamilyTrap() =>
        OneUnit([I32], [I32], 1, 1, 0, b =>
        {
            var skip = b.NewLabel();

            b.Emit(O.LocalGet, 0).EmitTo(O.JumpIfZero, skip).Emit(O.Trap, UbcCorpusCode.Pair(UbcOperandShape.U8U16, 1, F.TrapUser));
            b.Mark(skip).Emit(O.ConstI32, 0).Emit(O.Return);
            return [];
        });

    // ---- the header ----------------------------------------------------------------------------------

    private static void AddHeader(List<UbcCorpusEntry> entries)
    {
        entries.Add(Refused(
            "an-empty-payload", "prefix", [],
            UbcDiagnosticCode.Truncated, VmReason.Truncated, InHeader(0),
            "Zero bytes. The first read is bounded, so this is a truncation and never an index."));

        entries.Add(Refused(
            "the-magic-and-nothing-after-it", "prefix", [.. UbcFormat.Magic],
            UbcDiagnosticCode.Truncated, VmReason.Truncated, InHeader(4),
            "The four magic bytes alone: the format version read runs off the end."));

        var wrongMagic = Smallest();
        wrongMagic.Magic = "BUBD"u8.ToArray();
        entries.Add(Refused(
            "a-wrong-magic", "header", wrongMagic.Bytes(),
            UbcDiagnosticCode.WrongMagic, VmReason.MalformedEncoding, InHeader(0),
            "The last magic byte one higher. A wrong magic is the format's own refusal, not a truncation."));

        entries.Add(Refused(
            "a-payload-version-the-descriptor-does-not-name", "header", Canonical(),
            UbcDiagnosticCode.DescriptorFormatVersionMismatch, VmReason.DescriptorMismatch, InHeader(5),
            "Format version 1 presented under a descriptor naming version 2: the payload disagrees with the descriptor, whatever version it claims.",
            new UbcCorpusConfiguration(descriptorFormatVersion: 2)));

        var version2 = Smallest();
        version2.FormatVersion = 2;
        entries.Add(Refused(
            "a-format-version-this-build-does-not-define", "header", version2.Bytes(),
            UbcDiagnosticCode.UnsupportedFormatVersion, VmReason.UnsupportedProfileFormatVersion, InHeader(5),
            "Format version 2 under a descriptor that agrees: the two match and this build defines only version 1.",
            new UbcCorpusConfiguration(descriptorFormatVersion: 2)));

        entries.Add(Refused(
            "a-format-version-with-a-redundant-group", "header", [.. UbcFormat.Magic, 0x81, 0x00],
            UbcDiagnosticCode.MalformedEncoding, VmReason.MalformedEncoding, InHeader(6),
            "Version 1 written with a redundant continuation group. Two encodings of one value would make a byte-identical artifact check meaningless."));

        var emptyProfile = Smallest();
        emptyProfile.Profile = string.Empty;
        var emptyProfileBuilt = emptyProfile.Build();
        entries.Add(Refused(
            "an-empty-profile-identity", "header", emptyProfileBuilt.Bytes,
            UbcDiagnosticCode.MalformedIdentity, VmReason.MalformedEncoding, InHeader(emptyProfileBuilt.IdentityAt(0)),
            "A profile identity of length zero."));

        var longManifest = Smallest();
        longManifest.Manifest = "com.example.ubccorpus." + new string('x', UbcFormat.MaxIdentityBytes + 1 - 22);
        var longManifestBuilt = longManifest.Build();
        entries.Add(Refused(
            "a-manifest-identity-longer-than-the-format-admits", "header", longManifestBuilt.Bytes,
            UbcDiagnosticCode.MalformedIdentity, VmReason.MalformedEncoding, InHeader(longManifestBuilt.IdentityAt(1)),
            "A manifest identity of 129 bytes, one past the format's bound. Refused at its length, before its bytes are read."));

        var nonAsciiForm = Smallest();
        nonAsciiForm.Form = "bytecöde";
        var nonAsciiFormBuilt = nonAsciiForm.Build();
        entries.Add(Refused(
            "a-form-identity-that-is-not-ascii", "header", nonAsciiFormBuilt.Bytes,
            UbcDiagnosticCode.MalformedIdentity, VmReason.MalformedEncoding, InHeader(nonAsciiFormBuilt.IdentityAt(2)),
            "A form identity carrying a two-byte UTF-8 letter. Identities are ASCII letters, digits, dot, hyphen and underscore."));

        var spacedTranslator = Smallest();
        spacedTranslator.Translator = "com.example ubccorpus";
        var spacedTranslatorBuilt = spacedTranslator.Build();
        entries.Add(Refused(
            "a-translator-identity-with-a-space", "header", spacedTranslatorBuilt.Bytes,
            UbcDiagnosticCode.MalformedIdentity, VmReason.MalformedEncoding, InHeader(spacedTranslatorBuilt.IdentityAt(3)),
            "A translator identity with a space in it."));

        var otherProfile = Smallest();
        otherProfile.Profile = "com.example.other";
        entries.Add(Refused(
            "another-profiles-artifact", "header", otherProfile.Bytes(),
            UbcDiagnosticCode.DescriptorProfileMismatch, VmReason.DescriptorMismatch, InHeader(0),
            "A well-formed artifact of another profile, presented to this family's descriptor."));

        var wideHeader = Smallest();
        wideHeader.Manifest = F.WideManifestText;
        entries.Add(Refused(
            "a-manifest-the-descriptor-does-not-name", "header", wideHeader.Bytes(),
            UbcDiagnosticCode.DescriptorManifestMismatch, VmReason.DescriptorMismatch, InHeader(0),
            "The header names the wide manifest and the descriptor the base manifest; both are the family's, and they disagree."));

        var absent = Smallest();
        absent.Manifest = "com.example.ubccorpus.absent";
        entries.Add(Refused(
            "a-manifest-that-selects-no-table-of-the-family", "header", absent.Bytes(),
            UbcDiagnosticCode.ManifestNotComposed, VmReason.UnsupportedFeatureManifest, InHeader(0),
            "Header and descriptor agree on a manifest the family registers no table for.",
            new UbcCorpusConfiguration(descriptorManifest: "com.example.ubccorpus.absent")));

        var native = Smallest();
        native.Form = "x86-64";
        entries.Add(Refused(
            "a-native-form-this-image-does-not-compose", "header", native.Bytes(),
            UbcDiagnosticCode.FormNotComposed, VmReason.UnknownFeature, InHeader(0),
            "A native form identity. This image composes the bytecode form alone, so the artifact is refused before an executor could be asked to run it."));
    }

    // ---- section framing ------------------------------------------------------------------------------

    private static void AddFraming(List<UbcCorpusEntry> entries)
    {
        foreach (var (kind, where, name) in new (uint Kind, int At, string Name)[]
                 {
                     (0, 0, "a-section-of-kind-zero"),
                     (23, int.MaxValue, "a-section-kind-between-family-data-and-emission"),
                     (25, int.MaxValue, "a-section-kind-past-emission"),
                 })
        {
            var spec = Smallest();
            spec.ExtraSections.Add((where, kind, [0x00]));
            var built = spec.Build();
            var index = where == 0 ? 0 : spec.Sections().Count - 1;

            entries.Add(Refused(
                name, "framing", built.Bytes,
                UbcDiagnosticCode.UnknownSectionKind, VmReason.UnknownFeature, InHeader(built.SectionAt(index)),
                $"A section of kind {kind}, which format version 1 does not define. An unknown section is a refusal, not a skip: there are no custom sections."));
        }

        var outOfOrder = Smallest();
        outOfOrder.Rearrange = sections => [sections[1], sections[0], .. sections.Skip(2)];
        var outOfOrderBuilt = outOfOrder.Build();
        entries.Add(Refused(
            "sections-out-of-order", "framing", outOfOrderBuilt.Bytes,
            UbcDiagnosticCode.SectionOrder, VmReason.InconsistentStructure, InSection(UbcSectionKind.Types, outOfOrderBuilt.SectionAt(1)),
            "Units before Types. Kinds are strictly ascending."));

        var duplicated = Smallest();
        duplicated.ExtraSections.Add((1, (uint)UbcSectionKind.Types, UbcSectionEncoder.Types(duplicated.Types!)));
        var duplicatedBuilt = duplicated.Build();
        entries.Add(Refused(
            "a-duplicated-section", "framing", duplicatedBuilt.Bytes,
            UbcDiagnosticCode.SectionOrder, VmReason.InconsistentStructure, InSection(UbcSectionKind.Types, duplicatedBuilt.SectionAt(1)),
            "The Types section twice, identical both times. Kinds are unique."));

        var noEntries = Smallest();
        noEntries.Entries = null;
        var noEntriesBytes = noEntries.Bytes();
        entries.Add(Refused(
            "no-entries-section", "framing", noEntriesBytes,
            UbcDiagnosticCode.MissingSection, VmReason.InconsistentStructure, InSection(UbcSectionKind.Entries, (ulong)noEntriesBytes.Length),
            "No Entries section: required, even though its rows are optional."));

        var noCode = Smallest();
        noCode.OmitCode = true;
        var noCodeBytes = noCode.Bytes();
        entries.Add(Refused(
            "no-code-section", "framing", noCodeBytes,
            UbcDiagnosticCode.MissingSection, VmReason.InconsistentStructure, InSection(UbcSectionKind.Code, (ulong)noCodeBytes.Length),
            "Types, Units and Entries without the Code section the units name."));

        var noSections = Smallest();
        noSections.Types = null;
        noSections.Units = null;
        noSections.OmitCode = true;
        noSections.Entries = null;
        var noSectionsBytes = noSections.Bytes();
        entries.Add(Refused(
            "a-header-and-no-section", "framing", noSectionsBytes,
            UbcDiagnosticCode.MissingSection, VmReason.InconsistentStructure, InSection(UbcSectionKind.Types, (ulong)noSectionsBytes.Length),
            "A complete header declaring zero sections. The first required section missing is Types."));

        var trailing = Smallest();
        trailing.Trailing = [0x00];
        var trailingBytes = trailing.Bytes();
        entries.Add(Refused(
            "a-byte-after-the-last-section", "framing", trailingBytes,
            UbcDiagnosticCode.TrailingBytes, VmReason.InconsistentStructure, InHeader((ulong)trailingBytes.Length - 1),
            "One zero byte after the last declared section."));

        var longer = Smallest();
        var typesBody = UbcSectionEncoder.Types(longer.Types!);
        longer.Bodies[(uint)UbcSectionKind.Types] = [.. typesBody, 0x00];
        var longerBuilt = longer.Build();
        entries.Add(Refused(
            "a-section-longer-than-its-rows", "framing", longerBuilt.Bytes,
            UbcDiagnosticCode.SectionLengthMismatch, VmReason.InconsistentStructure,
            InSection(UbcSectionKind.Types, longerBuilt.BodyOf(UbcSectionKind.Types) + (ulong)typesBody.Length),
            "The Types section declares one byte more than its rows use. Consuming less than declared is as structural an error as consuming more."));

        var shorter = Smallest();
        shorter.StatedLengths[(uint)UbcSectionKind.Types] = (ulong)typesBody.Length - 1;
        var shorterBuilt = shorter.Build();
        entries.Add(Refused(
            "a-section-shorter-than-its-rows", "framing", shorterBuilt.Bytes,
            UbcDiagnosticCode.SectionLengthMismatch, VmReason.InconsistentStructure,
            InSection(UbcSectionKind.Types, shorterBuilt.BodyOf(UbcSectionKind.Types) + (ulong)typesBody.Length),
            "The Types section declares one byte less than its rows use, so its last row is read past the frame's end."));

        var overDeclared = Smallest();
        overDeclared.SectionCount = 5;
        var overDeclaredBytes = overDeclared.Bytes();
        entries.Add(Refused(
            "a-section-count-larger-than-the-sections", "framing", overDeclaredBytes,
            UbcDiagnosticCode.Truncated, VmReason.Truncated, InHeader((ulong)overDeclaredBytes.Length),
            "Five sections declared and four present: the fifth section's kind runs off the end."));

        var beyond = Smallest();
        beyond.StatedLengths[(uint)UbcSectionKind.Code] = 100;
        var beyondBuilt = beyond.Build();
        entries.Add(Refused(
            "a-section-length-beyond-the-payload", "framing", beyondBuilt.Bytes,
            UbcDiagnosticCode.Truncated, VmReason.Truncated, InSection(UbcSectionKind.Code, beyondBuilt.BodyOf(UbcSectionKind.Code)),
            "The Code section declares a hundred bytes inside a far shorter payload. The length is compared with what remains before a frame exists."));

        var canonicalBuilt = Smallest().Build();
        var typesAt = (int)canonicalBuilt.SectionOf(UbcSectionKind.Types);
        var bytes = canonicalBuilt.Bytes;
        byte[] nonCanonicalLength = [.. bytes[..(typesAt + 1)], (byte)(bytes[typesAt + 1] | 0x80), 0x00, .. bytes[(typesAt + 2)..]];
        entries.Add(Refused(
            "a-section-length-with-a-redundant-group", "framing", nonCanonicalLength,
            UbcDiagnosticCode.MalformedEncoding, VmReason.MalformedEncoding, InHeader((ulong)typesAt + 3),
            "The Types section's length written with a redundant continuation group."));

        var redundantCount = Smallest();
        redundantCount.Bodies[(uint)UbcSectionKind.Types] = [(byte)(typesBody[0] | 0x80), 0x00, .. typesBody[1..]];
        var redundantCountBuilt = redundantCount.Build();
        entries.Add(Refused(
            "a-count-with-a-redundant-group", "framing", redundantCountBuilt.Bytes,
            UbcDiagnosticCode.MalformedEncoding, VmReason.MalformedEncoding,
            InSection(UbcSectionKind.Types, redundantCountBuilt.BodyOf(UbcSectionKind.Types) + 2),
            "The Types count written with a redundant continuation group."));

        var wide = Smallest();
        wide.Bodies[(uint)UbcSectionKind.Types] = [0xFF, 0xFF, 0xFF, 0xFF, 0x1F];
        var wideBuilt = wide.Build();
        entries.Add(Refused(
            "a-count-wider-than-thirty-two-bits", "framing", wideBuilt.Bytes,
            UbcDiagnosticCode.MalformedEncoding, VmReason.MalformedEncoding,
            InSection(UbcSectionKind.Types, wideBuilt.BodyOf(UbcSectionKind.Types) + 5),
            "A count whose fifth group carries bits past the thirty-second."));
    }

    // ---- the Families section and FamilyData ------------------------------------------------------------

    private static void AddFamilies(List<UbcCorpusEntry> entries)
    {
        foreach (var (slot, name) in new (byte, string)[] { (0, "a-family-in-slot-zero"), (15, "a-family-in-slot-fifteen") })
        {
            var spec = SmallestWithFamily();
            spec.Families![0] = new UbcFamilyEntry(slot, F.Identity, F.BaseTableVersion, F.BaseManifestText);
            var built = spec.Build();

            entries.Add(Refused(
                name, "families", built.Bytes,
                UbcDiagnosticCode.FamilySlotInvalid, VmReason.InconsistentStructure,
                InSection(UbcSectionKind.Families, built.BodyOf(UbcSectionKind.Families) + 1),
                slot == 0
                    ? "Slot zero is the common family, which is never declared."
                    : "Slot fifteen: its prefix byte would be 0xFF, the reserved extended prefix, so format version 1 admits slots one to fourteen."));
        }

        var descending = SmallestWithFamily();
        var second = new UbcFamilyEntry(2, F.Identity, F.BaseTableVersion, F.BaseManifestText);
        descending.Families = [second, new UbcFamilyEntry(1, F.Identity, F.BaseTableVersion, F.BaseManifestText)];
        var descendingBuilt = descending.Build();
        entries.Add(Refused(
            "family-slots-that-descend", "families", descendingBuilt.Bytes,
            UbcDiagnosticCode.FamilySlotInvalid, VmReason.InconsistentStructure,
            InSection(UbcSectionKind.Families, descendingBuilt.BodyOf(UbcSectionKind.Families) + 1 + (ulong)(UbcSectionEncoder.Families([second]).Length - 1)),
            "Slot two, then slot one. Slots are strictly ascending."));

        var slashed = SmallestWithFamily();
        slashed.Families![0] = new UbcFamilyEntry(1, "com/example/ubccorpus", F.BaseTableVersion, F.BaseManifestText);
        var slashedBuilt = slashed.Build();
        entries.Add(Refused(
            "a-family-identity-with-a-slash", "families", slashedBuilt.Bytes,
            UbcDiagnosticCode.MalformedIdentity, VmReason.MalformedEncoding,
            InSection(UbcSectionKind.Families, slashedBuilt.BodyOf(UbcSectionKind.Families) + 2),
            "A Families row whose identity holds a byte its grammar refuses."));

        var stranger = SmallestWithFamily();
        stranger.Families![0] = new UbcFamilyEntry(1, "com.example.stranger", F.BaseTableVersion, F.BaseManifestText);
        entries.Add(Refused(
            "a-family-this-image-does-not-compose", "families", stranger.Bytes(),
            UbcDiagnosticCode.FamilyNotComposed, VmReason.UnknownFeature, InRow(UbcSectionKind.Families, 0),
            "A family the image does not compose is refused by name, before any instruction of it is reachable."));

        var version = SmallestWithFamily();
        version.Families![0] = new UbcFamilyEntry(1, F.Identity, F.WideTableVersion, F.BaseManifestText);
        entries.Add(Refused(
            "a-family-row-of-another-table-version", "families", version.Bytes(),
            UbcDiagnosticCode.FamilyTableVersionMismatch, VmReason.UnknownFeature, InRow(UbcSectionKind.Families, 0),
            "The right family and manifest with table version 2, which the base manifest does not select."));

        var manifest = SmallestWithFamily();
        manifest.Families![0] = new UbcFamilyEntry(1, F.Identity, F.BaseTableVersion, F.WideManifestText);
        entries.Add(Refused(
            "a-family-row-selected-by-another-manifest", "families", manifest.Bytes(),
            UbcDiagnosticCode.FamilyManifestMismatch, VmReason.InconsistentStructure, InRow(UbcSectionKind.Families, 0),
            "The Families row names the wide manifest under a header naming the base manifest."));

        var otherSlotData = SmallestWithFamily();
        otherSlotData.FamilyData[2] = [];
        entries.Add(Refused(
            "family-data-for-a-slot-no-family-occupies", "families", otherSlotData.Bytes(),
            UbcDiagnosticCode.FamilyDataForUndeclaredSlot, VmReason.InconsistentStructure, InSection((UbcSectionKind)UbcFormat.FamilyDataKind(2), 0),
            "The family occupies slot one and a FamilyData section belongs to slot two."));

        var orphanData = Smallest();
        orphanData.FamilyData[1] = [0];
        entries.Add(Refused(
            "family-data-without-a-family", "families", orphanData.Bytes(),
            UbcDiagnosticCode.FamilyDataForUndeclaredSlot, VmReason.InconsistentStructure, InSection(UbcSectionKind.FamilyDataFirst, 0),
            "A FamilyData section in an artifact whose Families section is absent."));

        var twice = SmallestWithFamily();
        twice.WithFamily(2);
        entries.Add(Refused(
            "the-family-declared-twice", "families", twice.Bytes(),
            UbcDiagnosticCode.DuplicateFamily, VmReason.InconsistentStructure, InRow(UbcSectionKind.Families, 1),
            "The one family declared in two slots. An artifact has at most one family, which also refuses two language families."));
    }

    // ---- Types and Units ---------------------------------------------------------------------------------

    private static void AddTypesAndUnits(List<UbcCorpusEntry> entries)
    {
        var parameter = Smallest();
        parameter.Bodies[(uint)UbcSectionKind.Types] = [1, 1, 0, 1, (byte)I32];
        var parameterBuilt = parameter.Build();
        entries.Add(Refused(
            "a-parameter-of-slot-type-zero", "units", parameterBuilt.Bytes,
            UbcDiagnosticCode.UnknownSlotType, VmReason.UnknownFeature, InSection(UbcSectionKind.Types, parameterBuilt.BodyOf(UbcSectionKind.Types) + 2),
            "A signature parameter of slot-type byte 0. The slot types are 1 to 5."));

        var result = Smallest();
        result.Bodies[(uint)UbcSectionKind.Types] = [1, 0, 1, 6];
        var resultBuilt = result.Build();
        entries.Add(Refused(
            "a-result-of-slot-type-six", "units", resultBuilt.Bytes,
            UbcDiagnosticCode.UnknownSlotType, VmReason.UnknownFeature, InSection(UbcSectionKind.Types, resultBuilt.BodyOf(UbcSectionKind.Types) + 3),
            "A signature result of slot-type byte 6, one past the language value."));

        var run = Smallest();
        run.ChangeUnit(0, unit => With(unit, locals: [new UbcLocalRun(1, (UbcSlotType)6)]));
        var runBuilt = run.Build();
        entries.Add(Refused(
            "a-local-run-of-slot-type-six", "units", runBuilt.Bytes,
            UbcDiagnosticCode.UnknownSlotType, VmReason.UnknownFeature, InSection(UbcSectionKind.Units, runBuilt.BodyOf(UbcSectionKind.Units) + 5),
            "A run of locals of slot-type byte 6."));

        var missingType = Smallest();
        missingType.ChangeUnit(0, unit => With(unit, typeIndex: 1));
        entries.Add(Refused(
            "a-unit-naming-a-missing-signature", "units", missingType.Bytes(),
            UbcDiagnosticCode.TypeIndexOutOfRange, VmReason.InconsistentStructure, InRow(UbcSectionKind.Units, 0),
            "A unit naming signature 1 of a Types section that has one."));

        var undeclared = SmallestWithFamily();
        undeclared.ChangeUnit(0, unit => With(unit, slot: 2));
        entries.Add(Refused(
            "a-unit-of-an-undeclared-family-slot", "units", undeclared.Bytes(),
            UbcDiagnosticCode.UnitFamilyUndeclared, VmReason.InconsistentStructure, InRow(UbcSectionKind.Units, 0),
            "A unit of slot two where the family occupies slot one."));

        var aboveSlots = Smallest();
        aboveSlots.ChangeUnit(0, unit => With(unit, slot: 15));
        var aboveSlotsBuilt = aboveSlots.Build();
        entries.Add(Refused(
            "a-unit-slot-above-the-highest-slot", "units", aboveSlotsBuilt.Bytes,
            UbcDiagnosticCode.UnitFamilyUndeclared, VmReason.InconsistentStructure, InSection(UbcSectionKind.Units, aboveSlotsBuilt.BodyOf(UbcSectionKind.Units) + 1),
            "A unit of slot fifteen, which no artifact can declare; refused as the row is read."));

        var reserved = Smallest();
        reserved.ChangeUnit(0, unit => With(unit, flags: Entry | (UbcUnitFlags)4));
        entries.Add(Refused(
            "a-unit-with-a-reserved-flag", "units", reserved.Bytes(),
            UbcDiagnosticCode.ReservedUnitFlag, VmReason.InconsistentStructure, InRow(UbcSectionKind.Units, 0),
            "Flag bit 2, which is reserved."));

        var familyBit = Smallest();
        familyBit.ChangeUnit(0, unit => With(unit, flags: Entry | (UbcUnitFlags)0x100));
        entries.Add(Refused(
            "a-family-flag-on-a-unit-without-a-family", "units", familyBit.Bytes(),
            UbcDiagnosticCode.ReservedUnitFlag, VmReason.InconsistentStructure, InRow(UbcSectionKind.Units, 0),
            "Flag bit 8, which belongs to the unit's family, on a unit of no family."));

        var wideFlags = Smallest();
        wideFlags.Bodies[(uint)UbcSectionKind.Units] = [1, 0, 0, 0, 1, 0, 0, 6, 0x82, 0x80, 0x04, 0];
        var wideFlagsBuilt = wideFlags.Build();
        entries.Add(Refused(
            "unit-flags-wider-than-sixteen-bits", "units", wideFlagsBuilt.Bytes,
            UbcDiagnosticCode.ReservedUnitFlag, VmReason.InconsistentStructure, InSection(UbcSectionKind.Units, wideFlagsBuilt.BodyOf(UbcSectionKind.Units) + 8),
            "Flags of 0x10002: a canonical integer the sixteen-bit flag field cannot hold."));

        var gap = new UbcCorpusSpec();
        var gapType = gap.AddType([], [I32]);
        gap.AddUnit(gapType, 0, 1, 0, Entry, b =>
        {
            b.Emit(O.ConstI32, 1).Emit(O.Return);
            return [];
        });
        gap.AppendCode((byte)O.Nop);
        gap.AddUnit(gapType, 0, 1, 0, UbcUnitFlags.None, b =>
        {
            b.Emit(O.ConstI32, 2).Emit(O.Return);
            return [];
        });
        gap.AddEntry("main", 0);
        entries.Add(Refused(
            "a-gap-between-two-units", "units", gap.Bytes(),
            UbcDiagnosticCode.CodeNotTiled, VmReason.InconsistentStructure, InRow(UbcSectionKind.Units, 1),
            "A byte between the first unit's end and the second unit's start. Units tile the Code section with no gap."));

        var empty = Smallest();
        empty.ChangeUnit(0, unit => With(unit, codeLength: 0));
        empty.CodeOverride = [];
        entries.Add(Refused(
            "a-unit-of-no-code", "units", empty.Bytes(),
            UbcDiagnosticCode.CodeNotTiled, VmReason.InconsistentStructure, InRow(UbcSectionKind.Units, 0),
            "A unit of length zero. Every unit holds at least one instruction."));

        var tail = Smallest();
        tail.AppendCode((byte)O.Nop);
        entries.Add(Refused(
            "code-past-the-last-unit", "units", tail.Bytes(),
            UbcDiagnosticCode.CodeNotTiled, VmReason.InconsistentStructure, InSection(UbcSectionKind.Code, 0),
            "A byte of code no unit owns."));

        var locals = Smallest();
        locals.ChangeUnit(0, unit => With(unit, locals: [new UbcLocalRun(UbcFormat.MaxLocals + 1, I32)]));
        entries.Add(Refused(
            "more-locals-than-an-operand-can-name", "units", locals.Bytes(),
            UbcDiagnosticCode.UnitDeclarationTooLarge, VmReason.InconsistentStructure, InRow(UbcSectionKind.Units, 0),
            "65537 locals: a sixteen-bit operand names 65536."));

        var height = Smallest();
        height.ChangeUnit(0, unit => With(unit, maxWords: UbcFormat.MaxHeight + 1));
        entries.Add(Refused(
            "a-declared-height-above-the-format", "units", height.Bytes(),
            UbcDiagnosticCode.UnitDeclarationTooLarge, VmReason.InconsistentStructure, InRow(UbcSectionKind.Units, 0),
            "A word-plane height of 65536, one above the format's bound."));

        var descending = Smallest();
        descending.ChangeUnit(0, unit => With(unit, landings: [5, 0]));
        entries.Add(Refused(
            "landings-that-descend", "units", descending.Bytes(),
            UbcDiagnosticCode.LandingsInvalid, VmReason.InconsistentStructure, InRow(UbcSectionKind.Units, 0),
            "Landings 5 then 0. Landings are strictly ascending."));

        var extra = Smallest();
        extra.ChangeUnit(0, unit => With(unit, landings: [5]));
        entries.Add(Refused(
            "a-landing-that-is-no-resume-point-or-handler", "units", extra.Bytes(),
            UbcDiagnosticCode.LandingsInvalid, VmReason.InconsistentStructure, InRow(UbcSectionKind.Units, 0),
            "A landing at the return of a unit that neither suspends nor has a region. Landings are exactly the resume points and the handlers."));

        var noResume = SuspendableUnit(out _, out var second, out var handler);
        noResume.ChangeUnit(0, unit => With(unit, landings: [second, handler]));
        entries.Add(Refused(
            "a-resume-point-missing-from-the-landings", "units", noResume.Bytes(),
            UbcDiagnosticCode.LandingsInvalid, VmReason.InconsistentStructure, InRow(UbcSectionKind.Units, 0),
            "The suspendable control with the first resume point left out of its landings."));

        var noHandler = SuspendableUnit(out var first, out var secondAgain, out _);
        noHandler.ChangeUnit(0, unit => With(unit, landings: [first, secondAgain]));
        entries.Add(Refused(
            "a-handler-missing-from-the-landings", "units", noHandler.Bytes(),
            UbcDiagnosticCode.LandingsInvalid, VmReason.InconsistentStructure, InRow(UbcSectionKind.Units, 0),
            "The suspendable control with its region's handler left out of its landings."));
    }

    // ---- the code walk --------------------------------------------------------------------------------------

    private static void AddWalk(List<UbcCorpusEntry> entries)
    {
        void Walk(string id, UbcCorpusSpec spec, UbcDiagnosticCode code, VmReason reason, int unit, uint pc, string note) =>
            entries.Add(Refused(id, "walk", spec.Bytes(), code, reason, InCode(unit, pc), note));

        Walk("an-undefined-common-opcode",
            OneUnit([], [I32], 0, 1, 0, b => { b.Raw(0x13).Emit(O.ConstI32, 1).Emit(O.Return); return []; }),
            UbcDiagnosticCode.UnknownOpcode, VmReason.UnknownFeature, 0, 0,
            "The unprefixed byte 0x13, which names no common instruction.");

        Walk("a-family-opcode-the-table-does-not-define",
            OneUnit([], [V], 1, 0, 1, b => { b.Raw(0xF1, 0x7F).F(Op.PushSmall, 1).Emit(O.Return); return []; }),
            UbcDiagnosticCode.UnknownOpcode, VmReason.UnknownFeature, 0, 0,
            "Family opcode 0x7F, which the base table does not define.");

        Walk("a-wide-row-under-the-base-manifest",
            OneUnit([], [V], 1, 0, 1, b => { b.F(Op.WideOnly).F(Op.PushSmall, 1).Emit(O.Return); return []; }),
            UbcDiagnosticCode.UnknownOpcode, VmReason.UnknownFeature, 0, 0,
            "The wide table's extra row under the base manifest, whose table is version 1 and lacks it: the manifest selects the table.");

        Walk("the-prefix-that-would-name-the-common-family",
            OneUnit([], [I32], 0, 1, 0, b => { b.Raw(0xF0, 0x00).Emit(O.ConstI32, 1).Emit(O.Return); return []; }),
            UbcDiagnosticCode.ReservedPrefix, VmReason.UnknownFeature, 0, 0,
            "The prefix byte 0xF0, which would name slot zero - the common family, which is never prefixed.");

        Walk("the-reserved-extended-prefix",
            OneUnit([], [I32], 0, 1, 0, b => { b.Raw(0xFF, 0x00).Emit(O.ConstI32, 1).Emit(O.Return); return []; }),
            UbcDiagnosticCode.ReservedPrefix, VmReason.UnknownFeature, 0, 0,
            "The extended prefix 0xFF, which format version 1 does not define.");

        Walk("another-slots-prefix",
            OneUnit([], [V], 1, 0, 1, b => { b.F(Op.PushSmall, 1, slot: 2).Emit(O.Return); return []; }),
            UbcDiagnosticCode.ForeignFamilyInstruction, VmReason.InconsistentStructure, 0, 0,
            "A slot-two prefix in a unit of slot one. A unit is single-family.");

        Walk("a-family-prefix-in-a-unit-without-a-family",
            OneUnit([], [I32], 0, 1, 1, b => { b.F(Op.PushSmall, 1).Emit(O.Drop).Emit(O.ConstI32, 1).Emit(O.Return); return []; }),
            UbcDiagnosticCode.ForeignFamilyInstruction, VmReason.InconsistentStructure, 0, 0,
            "A family instruction in a unit of no family, in an artifact that declares none.");

        Walk("an-operand-that-runs-past-the-unit",
            OneUnit([], [I32], 0, 1, 0, b => { b.Raw((byte)O.ConstI32, 0x01, 0x00); return []; }),
            UbcDiagnosticCode.TruncatedInstruction, VmReason.Truncated, 0, 0,
            "const.i32 with two of its four operand bytes: the operand runs past the unit's end.");

        Walk("a-family-prefix-at-the-last-byte",
            OneUnit([], [V], 1, 0, 1, b => { b.F(Op.PushSmall, 1).Emit(O.Return).Raw(0xF1); return []; }),
            UbcDiagnosticCode.TruncatedInstruction, VmReason.Truncated, 0, 4,
            "A family prefix as the unit's last byte, with no opcode after it.");

        Walk("a-jump-into-an-operand",
            OneUnit([], [I32], 0, 1, 0, b => { b.Emit(O.Jump, 1); return []; }),
            UbcDiagnosticCode.NotAnInstructionBoundary, VmReason.InconsistentStructure, 0, 0,
            "A jump to offset 1, inside its own operand.");

        var outOfUnit = new UbcCorpusSpec();
        var outType = outOfUnit.AddType([], [I32]);
        outOfUnit.AddUnit(outType, 0, 1, 0, Entry, b => { b.Emit(O.Jump, 5); return []; });
        outOfUnit.AddUnit(outType, 0, 1, 0, UbcUnitFlags.None, b => { b.Emit(O.ConstI32, 1).Emit(O.Return); return []; });
        outOfUnit.AddEntry("main", 0);
        Walk("a-jump-into-another-unit", outOfUnit,
            UbcDiagnosticCode.NotAnInstructionBoundary, VmReason.InconsistentStructure, 0, 0,
            "A jump to the first instruction of the next unit. A target is a boundary of the jump's own unit.");

        Walk("a-drop-on-an-empty-stack",
            OneUnit([], [I32], 0, 1, 0, b => { b.Emit(O.Drop).Emit(O.ConstI32, 1).Emit(O.Return); return []; }),
            UbcDiagnosticCode.StackUnderflow, VmReason.InconsistentStructure, 0, 0,
            "drop at the unit's entry, whose operand stack is empty.");

        Walk("a-pick-deeper-than-the-stack",
            OneUnit([], [I32], 0, 2, 0, b => { b.Emit(O.ConstI32, 1).Emit(O.Pick, 1).Emit(O.Return); return []; }),
            UbcDiagnosticCode.StackUnderflow, VmReason.InconsistentStructure, 0, 5,
            "pick 1 over a stack of one slot.");

        Walk("a-squash-deeper-than-the-stack",
            OneUnit([], [I32], 0, 2, 0, b =>
            {
                b.Emit(O.ConstI32, 1).Emit(O.Squash, UbcCorpusCode.Pair(UbcOperandShape.U8U8, 1, 1)).Emit(O.Return);
                return [];
            }),
            UbcDiagnosticCode.StackUnderflow, VmReason.InconsistentStructure, 0, 5,
            "squash 1 1 over a stack of one slot: the kept slot is there and the discarded one is not.");

        var callee = new UbcCorpusSpec();
        var calleeMain = callee.AddType([], [I32]);
        var calleeSignature = callee.AddType([I32], [I32]);
        callee.AddUnit(calleeMain, 0, 1, 0, Entry, b => { b.Emit(O.Call, 1).Emit(O.Return); return []; });
        callee.AddUnit(calleeSignature, 0, 1, 0, UbcUnitFlags.None, b => { b.Emit(O.LocalGet, 0).Emit(O.Return); return []; });
        callee.AddEntry("main", 0);
        Walk("a-call-without-its-parameters", callee,
            UbcDiagnosticCode.StackUnderflow, VmReason.InconsistentStructure, 0, 0,
            "A call to a unit taking an i32, from an empty stack.");

        Walk("a-counted-run-deeper-than-the-stack",
            OneUnit([], [V], 1, 0, 2, b => { b.F(Op.PushSmall, 1).F(Op.CallValue, 2).Emit(O.Return); return []; }),
            UbcDiagnosticCode.StackUnderflow, VmReason.InconsistentStructure, 0, 3,
            "call_value 2 pops a callee beneath two arguments, and the stack holds one value.");

        Walk("a-branch-on-a-sixty-four-bit-word",
            OneUnit([], [I32], 0, 1, 0, b =>
            {
                var next = b.NewLabel();
                b.Emit(O.ConstI64, 1).EmitTo(O.JumpIfZero, next);
                b.Mark(next).Emit(O.ConstI32, 0).Emit(O.Return);
                return [];
            }),
            UbcDiagnosticCode.StackTypeMismatch, VmReason.InconsistentStructure, 0, 9,
            "jump_if_zero over an i64: its pop is an i32.");

        Walk("a-select-between-two-types",
            OneUnit([], [I32], 0, 3, 0, b =>
            {
                b.Emit(O.ConstI32, 1).Emit(O.ConstI64, 2).Emit(O.ConstI32, 0).Emit(O.Select).Emit(O.Return);
                return [];
            }),
            UbcDiagnosticCode.StackTypeMismatch, VmReason.InconsistentStructure, 0, 19,
            "select between an i32 and an i64. Both candidates are one type.");

        Walk("a-return-of-the-wrong-result-type",
            OneUnit([], [I32], 0, 1, 0, b => { b.Emit(O.ConstI64, 1).Emit(O.Return); return []; }),
            UbcDiagnosticCode.StackTypeMismatch, VmReason.InconsistentStructure, 0, 9,
            "return of an i64 from a unit declaring an i32 result.");

        Walk("a-local-set-of-the-wrong-type",
            OneUnit([], [I32], 0, 1, 0, b =>
            {
                b.Emit(O.ConstI64, 1).Emit(O.LocalSet, 0).Emit(O.ConstI32, 0).Emit(O.Return);
                return [];
            }, Entry, new UbcLocalRun(1, I32)),
            UbcDiagnosticCode.StackTypeMismatch, VmReason.InconsistentStructure, 0, 9,
            "local.set of an i64 into an i32 local.");

        Walk("a-counted-run-of-the-wrong-type",
            OneUnit([], [V], 1, 1, 1, b =>
            {
                b.Emit(O.ConstI32, 1).F(Op.PushSmall, 1).F(Op.MakeTemplate, 1).Emit(O.Return);
                return [];
            }),
            UbcDiagnosticCode.StackTypeMismatch, VmReason.InconsistentStructure, 0, 8,
            "make_template 1 pops a run of two values, and the deeper slot is an i32.");

        Walk("two-paths-meeting-with-different-stacks",
            OneUnit([I32], [I32], 0, 2, 0, b =>
            {
                var join = b.NewLabel();
                b.Emit(O.LocalGet, 0).EmitTo(O.JumpIfZero, join).Emit(O.ConstI32, 1);
                b.Mark(join).Emit(O.ConstI32, 2).Emit(O.Return);
                return [];
            }),
            UbcDiagnosticCode.JoinMismatch, VmReason.InconsistentStructure, 0, 13,
            "The branch reaches the join with nothing on the stack and the fall-through with an i32.");

        Walk("a-taken-edge-that-disagrees-with-the-fall-through",
            OneUnit([], [V], 1, 0, 1, b =>
            {
                var next = b.NewLabel();
                b.F(Op.PushSmall, 1).FTo(Op.Iterate, next);
                b.Mark(next).Emit(O.Return);
                return [];
            }),
            UbcDiagnosticCode.JoinMismatch, VmReason.InconsistentStructure, 0, 9,
            "iterate targets its own fall-through: the taken edge pushes nothing and the fall-through a value, so the walk applied the taken-edge adjustment.");

        Walk("a-push-above-the-declared-height",
            OneUnit([], [I32], 0, 1, 0, b => { b.Emit(O.ConstI32, 1).Emit(O.ConstI32, 2).Emit(O.Drop).Emit(O.Return); return []; }),
            UbcDiagnosticCode.HeightAboveDeclared, VmReason.InconsistentStructure, 0, 5,
            "A second word on a unit declaring a word height of one.");

        Walk("a-value-above-the-declared-value-height",
            OneUnit([], [V], 1, 0, 1, b => { b.F(Op.PushSmall, 1).F(Op.PushSmall, 2).Emit(O.Drop).Emit(O.Return); return []; }),
            UbcDiagnosticCode.HeightAboveDeclared, VmReason.InconsistentStructure, 0, 3,
            "A second value on a unit declaring a value height of one. The planes have separate heights.");

        Walk("a-path-that-runs-off-the-end",
            OneUnit([], [I32], 0, 1, 0, b => { b.Emit(O.ConstI32, 1).Emit(O.Drop); return []; }),
            UbcDiagnosticCode.FallsOffTheEnd, VmReason.InconsistentStructure, 0, 5,
            "The last instruction is drop, which falls through past the unit's end.");

        Walk("an-instruction-no-path-reaches",
            OneUnit([], [I32], 0, 1, 0, b => { b.Emit(O.ConstI32, 1).Emit(O.Return).Emit(O.Nop).Emit(O.Return); return []; }),
            UbcDiagnosticCode.UnreachableInstruction, VmReason.InconsistentStructure, 0, 6,
            "A nop after the return, which no path reaches.");

        Walk("a-local-past-the-local-table",
            OneUnit([], [I32], 0, 1, 0, b => { b.Emit(O.LocalGet, 1).Emit(O.Return); return []; }, Entry, new UbcLocalRun(1, I32)),
            UbcDiagnosticCode.LocalIndexOutOfRange, VmReason.InconsistentStructure, 0, 0,
            "local.get 1 in a unit of one local.");

        Walk("a-call-to-a-unit-that-does-not-exist",
            OneUnit([], [I32], 0, 1, 0, b => { b.Emit(O.Call, 9).Emit(O.Return); return []; }),
            UbcDiagnosticCode.CallTargetOutOfRange, VmReason.InconsistentStructure, 0, 0,
            "call 9 in an artifact of one unit.");

        Walk("a-jump-table-that-does-not-exist",
            OneUnit([], [I32], 0, 1, 0, b => { b.Emit(O.ConstI32, 0).Emit(O.JumpTable, 3); return []; }),
            UbcDiagnosticCode.JumpTableInvalid, VmReason.InconsistentStructure, 0, 5,
            "jump_table 3 in an artifact with no JumpTables section.");

        var foreignTable = new UbcCorpusSpec();
        var foreignType = foreignTable.AddType([], [I32]);
        foreignTable.AddUnit(foreignType, 0, 1, 0, Entry, b => { b.Emit(O.ConstI32, 0).Emit(O.JumpTable, 0); return []; });
        uint otherTarget = 0;
        foreignTable.AddUnit(foreignType, 0, 1, 0, UbcUnitFlags.None, b =>
        {
            otherTarget = b.Offset;
            b.Emit(O.ConstI32, 1).Emit(O.Return);
            return [];
        });
        foreignTable.JumpTables = [new UbcJumpTable(1, [otherTarget])];
        foreignTable.AddEntry("main", 0);
        Walk("a-jump-table-of-another-unit", foreignTable,
            UbcDiagnosticCode.JumpTableInvalid, VmReason.InconsistentStructure, 0, 5,
            "jump_table 0 in unit 0, where table 0 belongs to unit 1.");

        Walk("a-suspension-in-a-unit-not-flagged-suspendable",
            OneUnit([], [V], 1, 0, 1, b => { b.F(Op.PushSmall, 1).F(Op.Await).Emit(O.Return); return []; }),
            UbcDiagnosticCode.SuspendOutsideSuspendableUnit, VmReason.InconsistentStructure, 0, 3,
            "A suspending row in a unit whose Suspendable flag is clear.");

        Walk("a-trap-of-the-common-slot-with-a-code",
            OneUnit([], [I32], 0, 1, 0, b => { b.Emit(O.Trap, UbcCorpusCode.Pair(UbcOperandShape.U8U16, 0, 1)); return []; }),
            UbcDiagnosticCode.TrapUndefined, VmReason.InconsistentStructure, 0, 0,
            "trap 0 1. Slot zero has one code, zero, the universal unreachable.");

        Walk("a-trap-code-outside-the-family-vocabulary",
            OneUnit([], [I32], 1, 1, 0, b => { b.Emit(O.Trap, UbcCorpusCode.Pair(UbcOperandShape.U8U16, 1, 999)); return []; }),
            UbcDiagnosticCode.TrapUndefined, VmReason.InconsistentStructure, 0, 0,
            "trap 1 999: the family occupies slot one and its vocabulary has no code 999.");

        Walk("a-trap-naming-a-slot-no-family-occupies",
            OneUnit([], [I32], 1, 1, 0, b => { b.Emit(O.Trap, UbcCorpusCode.Pair(UbcOperandShape.U8U16, 2, F.TrapUser)); return []; }),
            UbcDiagnosticCode.TrapUndefined, VmReason.InconsistentStructure, 0, 0,
            "trap 2 100: a code of the family's vocabulary, under a slot no family occupies.");

        Walk("a-counted-operand-no-stack-could-supply",
            OneUnit([], [V], 1, 0, 1, b => { b.F(Op.MakeTemplate, ushort.MaxValue).Emit(O.Return); return []; }),
            UbcDiagnosticCode.CountedOperandTooLarge, VmReason.InconsistentStructure, 0, 0,
            "make_template 65535 pops 131070 values, more than any declared height admits: refused before anything is popped.");
    }

    // ---- regions ------------------------------------------------------------------------------------------

    /// <summary>One family unit: push a value, throw it, and a catch handler that returns the exception.</summary>
    private static UbcCorpusSpec RegionUnit(out uint start, out uint handler)
    {
        var spec = new UbcCorpusSpec().WithFamily();
        var type = spec.AddType([], [V]);
        uint begin = 0, landing = 0;

        var unit = spec.AddUnit(type, 1, 0, 1, Entry, b =>
        {
            var label = b.NewLabel();
            begin = b.Offset;
            b.F(Op.PushSmall, 1).F(Op.Throw);
            landing = b.Here(label);
            b.Emit(O.Return);
            return [landing];
        });

        spec.Regions = [new UbcRegion((uint)unit, begin, landing, landing, 0, 0, F.CatchKind)];
        spec.AddEntry("main", unit);
        start = begin;
        handler = landing;
        return spec;
    }

    private static void AddRegions(List<UbcCorpusEntry> entries)
    {
        void Region(string id, Func<UbcRegion, UbcRegion> change, UbcDiagnosticCode code, string position, string note)
        {
            var spec = RegionUnit(out _, out _);
            spec.Regions![0] = change(spec.Regions[0]);
            entries.Add(Refused(id, "regions", spec.Bytes(), code, VmReason.InconsistentStructure, position, note));
        }

        Region("a-region-of-a-unit-that-does-not-exist",
            region => new UbcRegion(3, region.Start, region.End, region.Handler, 0, 0, region.Kind),
            UbcDiagnosticCode.RegionRangeInvalid, InRow(UbcSectionKind.Regions, 0),
            "A region of unit 3 in an artifact of one unit.");

        Region("a-region-that-ends-where-it-starts",
            region => new UbcRegion(region.Unit, region.Start, region.Start, region.Handler, 0, 0, region.Kind),
            UbcDiagnosticCode.RegionRangeInvalid, InRow(UbcSectionKind.Regions, 0),
            "A region whose end is its start: it covers nothing.");

        Region("a-region-reaching-past-its-unit",
            region => new UbcRegion(region.Unit, region.Start, region.Handler + 2, region.Handler, 0, 0, region.Kind),
            UbcDiagnosticCode.RegionRangeInvalid, InRow(UbcSectionKind.Regions, 0),
            "A region ending one byte past its unit's last byte.");

        Region("a-handler-outside-its-unit",
            region => new UbcRegion(region.Unit, region.Start, region.End, 99, 0, 0, region.Kind),
            UbcDiagnosticCode.RegionRangeInvalid, InRow(UbcSectionKind.Regions, 0),
            "A handler at offset 99, outside the unit.");

        Region("a-region-of-a-kind-the-table-does-not-define",
            region => new UbcRegion(region.Unit, region.Start, region.End, region.Handler, 0, 0, 9),
            UbcDiagnosticCode.RegionKindUndefined, InRow(UbcSectionKind.Regions, 0),
            "Region kind 9, which the family's table does not define.");

        Region("a-region-starting-inside-an-instruction",
            region => new UbcRegion(region.Unit, region.Start + 1, region.End, region.Handler, 0, 0, region.Kind),
            UbcDiagnosticCode.NotAnInstructionBoundary, InRow(UbcSectionKind.Regions, 0),
            "A region starting at offset 1, inside the first instruction.");

        Region("a-region-entry-height-above-the-format",
            region => new UbcRegion(region.Unit, region.Start, region.End, region.Handler, UbcFormat.MaxHeight + 1, 0, region.Kind),
            UbcDiagnosticCode.RegionEntryInconsistent, InRow(UbcSectionKind.Regions, 0),
            "A word entry height of 65536, which no unit can declare.");

        Region("a-region-entry-height-the-stack-does-not-hold",
            region => new UbcRegion(region.Unit, region.Start, region.End, region.Handler, 0, 2, region.Kind),
            UbcDiagnosticCode.RegionEntryInconsistent, InCode(0, 0),
            "A value entry height of two over a region whose first instruction has an empty stack.");

        var aboveHeight = RegionUnit(out _, out var aboveHeightHandler);
        aboveHeight.Regions![0] = new UbcRegion(0, aboveHeight.Regions[0].Start, aboveHeight.Regions[0].End, aboveHeightHandler, 0, 0, F.FinallyKind);
        entries.Add(Refused(
            "a-landing-above-the-declared-height", "regions", aboveHeight.Bytes(),
            UbcDiagnosticCode.HeightAboveDeclared, VmReason.InconsistentStructure, InCode(0, aboveHeightHandler),
            "A finally landing pushes a value and a word into a unit declaring a word height of zero: the landing's pushes are held to the declared heights like any push."));

        var noFamily = Smallest();
        noFamily.Regions = [new UbcRegion(0, 0, 5, 5, 0, 0, F.CatchKind)];
        entries.Add(Refused(
            "a-region-in-a-unit-without-a-family", "regions", noFamily.Bytes(),
            UbcDiagnosticCode.RegionKindUndefined, VmReason.InconsistentStructure, InRow(UbcSectionKind.Regions, 0),
            "A region in a unit of no family: region kinds are the family's, so there is none to name."));

        var overlap = new UbcCorpusSpec().WithFamily();
        var overlapType = overlap.AddType([], [V]);
        overlap.AddUnit(overlapType, 1, 0, 2, Entry, b =>
        {
            b.F(Op.PushSmall, 1).F(Op.PushSmall, 2).Emit(O.Drop).F(Op.Throw).Emit(O.Return);
            return [9];
        });
        overlap.Regions = [new UbcRegion(0, 0, 6, 9, 0, 0, F.CatchKind), new UbcRegion(0, 3, 9, 9, 0, 0, F.CatchKind)];
        overlap.AddEntry("main", 0);
        entries.Add(Refused(
            "regions-that-overlap-without-nesting", "regions", overlap.Bytes(),
            UbcDiagnosticCode.RegionNesting, VmReason.InconsistentStructure, InRow(UbcSectionKind.Regions, 1),
            "Two regions [0, 6) and [3, 9) of one unit: they overlap and neither holds the other."));

        var outerFirst = new UbcCorpusSpec().WithFamily();
        var outerFirstType = outerFirst.AddType([], [V]);
        outerFirst.AddUnit(outerFirstType, 1, 0, 2, Entry, b =>
        {
            b.F(Op.PushSmall, 1).F(Op.PushSmall, 2).Emit(O.Drop).F(Op.Throw).Emit(O.Return);
            return [9];
        });
        outerFirst.Regions = [new UbcRegion(0, 0, 9, 9, 0, 0, F.CatchKind), new UbcRegion(0, 3, 6, 9, 0, 0, F.CatchKind)];
        outerFirst.AddEntry("main", 0);
        entries.Add(Refused(
            "an-outer-region-listed-before-its-inner-one", "regions", outerFirst.Bytes(),
            UbcDiagnosticCode.RegionNesting, VmReason.InconsistentStructure, InRow(UbcSectionKind.Regions, 1),
            "Region [0, 9) listed before the region [3, 6) it holds. Regions are listed inner before outer."));

        var shifting = new UbcCorpusSpec().WithFamily();
        var shiftingType = shifting.AddType([], [V]);
        uint regionStart = 0, secondCovered = 0, regionEnd = 0, landing = 0;
        shifting.AddUnit(shiftingType, 1, 2, 1, Entry, b =>
        {
            var handler = b.NewLabel();
            b.Emit(O.ConstI32, 1).Emit(O.ConstI64, 2);
            regionStart = b.Offset;
            b.Emit(O.Swap);
            secondCovered = b.Offset;
            b.Emit(O.Drop);
            regionEnd = b.Offset;
            b.Emit(O.Drop).F(Op.PushSmall, 1).Emit(O.Return);
            landing = b.Here(handler);
            b.Emit(O.Drop).Emit(O.Drop).F(Op.PushSmall, 0).Emit(O.Return);
            return [landing];
        });
        shifting.Regions = [new UbcRegion(0, regionStart, regionEnd, landing, 1, 0, F.CatchKind)];
        shifting.AddEntry("main", 0);
        entries.Add(Refused(
            "a-region-whose-prefix-changes-inside-it", "regions", shifting.Bytes(),
            UbcDiagnosticCode.RegionEntryInconsistent, VmReason.InconsistentStructure, InCode(0, secondCovered),
            "A region of one-word entry height over a swap: the bottom word is an i32 at the first covered instruction and an i64 at the second."));

        var disagreeing = new UbcCorpusSpec().WithFamily();
        var disagreeingType = disagreeing.AddType([], [V]);
        uint dropAt = 0, handlerAt = 0;
        disagreeing.AddUnit(disagreeingType, 1, 0, 1, Entry, b =>
        {
            var handler = b.NewLabel();
            b.F(Op.PushSmall, 1);
            dropAt = b.Offset;
            b.Emit(O.Drop);
            handlerAt = b.Here(handler);
            b.F(Op.PushSmall, 0).Emit(O.Return);
            return [handlerAt];
        });
        disagreeing.Regions = [new UbcRegion(0, 0, dropAt, handlerAt, 0, 0, F.CatchKind)];
        disagreeing.AddEntry("main", 0);
        entries.Add(Refused(
            "a-landing-that-disagrees-with-a-fall-through", "regions", disagreeing.Bytes(),
            UbcDiagnosticCode.JoinMismatch, VmReason.InconsistentStructure, InCode(0, handlerAt),
            "The handler is reached by the landing with the caught value and by a fall-through with an empty stack."));
    }

    // ---- jump tables, entries and positions ----------------------------------------------------------------

    private static void AddTablesEntriesAndPositions(List<UbcCorpusEntry> entries)
    {
        const string group = "tables-entries-positions";

        var noUnit = Smallest();
        noUnit.JumpTables = [new UbcJumpTable(5, [0])];
        entries.Add(Refused(
            "a-jump-table-of-a-unit-that-does-not-exist", group, noUnit.Bytes(),
            UbcDiagnosticCode.JumpTableMalformed, VmReason.InconsistentStructure, InRow(UbcSectionKind.JumpTables, 0),
            "A jump table of unit 5 in an artifact of one unit."));

        var noTarget = Smallest();
        noTarget.JumpTables = [new UbcJumpTable(0, [])];
        entries.Add(Refused(
            "a-jump-table-with-no-target", group, noTarget.Bytes(),
            UbcDiagnosticCode.JumpTableMalformed, VmReason.InconsistentStructure, InRow(UbcSectionKind.JumpTables, 0),
            "A jump table of no rows. Its last row is the default, so it has at least one."));

        var inside = Smallest();
        inside.JumpTables = [new UbcJumpTable(0, [1])];
        entries.Add(Refused(
            "a-jump-table-target-inside-an-instruction", group, inside.Bytes(),
            UbcDiagnosticCode.NotAnInstructionBoundary, VmReason.InconsistentStructure, InRow(UbcSectionKind.JumpTables, 0),
            "A jump table target at offset 1, inside const.i32. Checked even for a table no instruction uses."));

        var otherUnit = new UbcCorpusSpec();
        var otherUnitType = otherUnit.AddType([], [I32]);
        otherUnit.AddUnit(otherUnitType, 0, 1, 0, Entry, b => { b.Emit(O.ConstI32, 1).Emit(O.Return); return []; });
        var foreignTarget = otherUnit.NextCodeOffset;
        otherUnit.AddUnit(otherUnitType, 0, 1, 0, UbcUnitFlags.None, b => { b.Emit(O.ConstI32, 2).Emit(O.Return); return []; });
        otherUnit.JumpTables = [new UbcJumpTable(0, [foreignTarget])];
        otherUnit.AddEntry("main", 0);
        entries.Add(Refused(
            "a-jump-table-target-in-another-unit", group, otherUnit.Bytes(),
            UbcDiagnosticCode.NotAnInstructionBoundary, VmReason.InconsistentStructure, InRow(UbcSectionKind.JumpTables, 0),
            "A jump table of unit 0 whose target is the first instruction of unit 1: a boundary, and not one of the table's unit."));

        foreach (var (id, name, note) in new (string, byte[], string)[]
                 {
                     ("an-empty-entry-name", [], "An entry name of no bytes."),
                     ("an-entry-name-longer-than-the-format-admits", Encoding.ASCII.GetBytes(new string('n', UbcFormat.MaxEntryNameBytes + 1)),
                         "An entry name of 1025 bytes, one past the format's bound."),
                     ("an-entry-name-that-is-not-utf-8", [0x6D, 0xC3, 0x28], "An entry name holding a lead byte followed by no continuation byte."),
                 })
        {
            var spec = Smallest();
            spec.Entries!.Clear();
            spec.AddEntry(name, 0);
            var built = spec.Build();
            entries.Add(Refused(
                id, group, built.Bytes,
                UbcDiagnosticCode.EntryNameInvalid, VmReason.InconsistentStructure, InSection(UbcSectionKind.Entries, built.BodyOf(UbcSectionKind.Entries) + 1),
                note));
        }

        var twoNames = Smallest();
        twoNames.AddEntry("main", 0);
        entries.Add(Refused(
            "two-entries-with-one-name", group, twoNames.Bytes(),
            UbcDiagnosticCode.EntryNameInvalid, VmReason.InconsistentStructure, InRow(UbcSectionKind.Entries, 1),
            "Two entries named main. The later row is the one refused."));

        var missingUnit = Smallest();
        missingUnit.Entries!.Clear();
        missingUnit.AddEntry("main", 3);
        entries.Add(Refused(
            "an-entry-of-a-unit-that-does-not-exist", group, missingUnit.Bytes(),
            UbcDiagnosticCode.EntryUnitInvalid, VmReason.InconsistentStructure, InRow(UbcSectionKind.Entries, 0),
            "An entry of unit 3 in an artifact of one unit."));

        var unflagged = Smallest();
        unflagged.ChangeUnit(0, unit => With(unit, flags: UbcUnitFlags.None));
        entries.Add(Refused(
            "an-entry-of-a-unit-not-flagged-as-an-entry", group, unflagged.Bytes(),
            UbcDiagnosticCode.EntryUnitInvalid, VmReason.InconsistentStructure, InRow(UbcSectionKind.Entries, 0),
            "An entry naming a unit whose Entry flag is clear."));

        var positionUnit = Smallest();
        positionUnit.Positions = [new UbcPosition(4, 0, 1, 1)];
        entries.Add(Refused(
            "a-position-of-a-unit-that-does-not-exist", group, positionUnit.Bytes(),
            UbcDiagnosticCode.PositionInvalid, VmReason.InconsistentStructure, InRow(UbcSectionKind.Positions, 0),
            "A position of unit 4 in an artifact of one unit."));

        var coordinate = Smallest();
        coordinate.Positions = [];
        coordinate.Bodies[(uint)UbcSectionKind.Positions] = [1, 0, 0, 0x80, 0x80, 0x80, 0x80, 0x08, 0];
        var coordinateBuilt = coordinate.Build();
        entries.Add(Refused(
            "a-position-coordinate-above-the-core-range", group, coordinateBuilt.Bytes,
            UbcDiagnosticCode.PositionInvalid, VmReason.InconsistentStructure,
            InSection(UbcSectionKind.Positions, coordinateBuilt.BodyOf(UbcSectionKind.Positions) + 1),
            "A first coordinate of 2^31, one past the core position record's signed range."));

        var between = Smallest();
        between.Positions = [new UbcPosition(0, 1, 1, 1)];
        entries.Add(Refused(
            "a-position-inside-an-instruction", group, between.Bytes(),
            UbcDiagnosticCode.NotAnInstructionBoundary, VmReason.InconsistentStructure, InRow(UbcSectionKind.Positions, 0),
            "A position at offset 1, inside const.i32."));
    }

    // ---- the form layer --------------------------------------------------------------------------------------

    private static void AddForm(List<UbcCorpusEntry> entries)
    {
        var emission = new UbcEmission(UbcFormat.BytecodeForm, 1, 16, 0x0123_4567_89AB_CDEF, [0x90, 0xC3], [new UbcSymbol(0, 0)]);

        var carried = Smallest();
        carried.Emission = emission;
        entries.Add(Refused(
            "an-emission-in-the-bytecode-form", "form", carried.Bytes(),
            UbcDiagnosticCode.EmissionUnexpected, VmReason.InconsistentStructure, InSection(UbcSectionKind.Emission, 0),
            "A well-formed Emission section in an artifact of the bytecode form, refused after the walk admitted everything else."));

        var overrun = Smallest();
        overrun.Emission = emission;
        var body = UbcSectionEncoder.Emission(emission);
        var lengthAt = 1 + UbcFormat.BytecodeForm.Length + 1 + 1 + 8;
        body[lengthAt] = 0xE8;
        body[lengthAt + 1] = 0x03;
        overrun.Bodies[(uint)UbcSectionKind.Emission] = body;
        var overrunBuilt = overrun.Build();
        entries.Add(Refused(
            "an-emission-whose-code-length-overruns-it", "form", overrunBuilt.Bytes,
            UbcDiagnosticCode.Truncated, VmReason.Truncated,
            InSection(UbcSectionKind.Emission, overrunBuilt.BodyOf(UbcSectionKind.Emission) + (ulong)lengthAt + 4),
            "An Emission section stating a thousand emitted bytes and carrying two. The reader refuses the run before allocating it."));
    }

    // ---- the family hook ---------------------------------------------------------------------------------------

    private static void AddHook(List<UbcCorpusEntry> entries)
    {
        var pastPool = OneUnit([], [V], 1, 0, 1, b => { b.F(Op.LoadConstant, 3).Emit(O.Return); return []; });
        pastPool.FamilyData[1] = [0, 1, 2];
        entries.Add(RefusedByFamily(
            "a-constant-index-past-the-family-pool", "hook", pastPool.Bytes(),
            F.ConstantPastThePool, VmReason.SemanticValidationFailed, InCode(0, 0),
            "load_constant 3 against a FamilyData pool of three constants: only the family knows the pool, so the hook refuses with the family's code."));

        var noPool = OneUnit([], [V], 1, 0, 1, b => { b.F(Op.LoadConstant, 0).Emit(O.Return); return []; });
        entries.Add(RefusedByFamily(
            "a-constant-load-with-no-family-data", "hook", noPool.Bytes(),
            F.ConstantPastThePool, VmReason.SemanticValidationFailed, InCode(0, 0),
            "load_constant 0 in an artifact carrying no FamilyData section: the pool is empty."));

        var tag = SmallestWithFamily();
        tag.FamilyData[1] = [0, F.MaxConstantTag + 1];
        entries.Add(RefusedByFamily(
            "a-family-data-tag-above-its-bound", "hook", tag.Bytes(),
            F.ConstantTagAboveItsBound, VmReason.InconsistentStructure, InSection(UbcSectionKind.FamilyDataFirst, 0),
            "A FamilyData pool whose second tag is 8, above the family's bound of 7: the hook reads the section before any instruction."));

        entries.Add(Refused(
            "a-hook-answering-a-universal-code", "hook", SmallestWithFamily().Bytes(),
            UbcDiagnosticCode.VerifierDefect, VmReason.InconsistentStructure, InCode(0, 0),
            "A hook that refuses with a code of the universal range. The walk does not pass the universal bytecode's own code off as the family's; it reports its hook as defective.",
            new UbcCorpusConfiguration(hook: UbcCorpusHookMode.UniversalCode)));

        entries.Add(Refused(
            "a-hook-answering-no-invalid-artifact-reason", "hook", SmallestWithFamily().Bytes(),
            UbcDiagnosticCode.VerifierDefect, VmReason.InconsistentStructure, InCode(0, 0),
            "A hook that refuses with the reason NormalCompleted, which is no invalid-artifact reason.",
            new UbcCorpusConfiguration(hook: UbcCorpusHookMode.NoInvalidReason)));

        entries.Add(Exhausted(
            "a-hook-answering-an-exhaustion", "hook", SmallestWithFamily().Bytes(),
            VmBudgetDimension.AllocatedBytes, VmReason.AllowanceExhausted,
            "A hook that answers an allocated-bytes exhaustion: an exhaustion at artifact scope, not an invalid artifact.",
            new UbcCorpusConfiguration(hook: UbcCorpusHookMode.Exhausting)));
    }

    // ---- the ceilings and allowances ------------------------------------------------------------------------------

    private static void AddCeilings(List<UbcCorpusEntry> entries)
    {
        var canonical = Canonical();
        var defaults = F.Declaration().LimitDefaults;

        var count = Smallest();
        count.Bodies[(uint)UbcSectionKind.Types] = VarUInt(defaults[VmBudgetDimension.DeclaredCount] + 1);
        entries.Add(Exhausted(
            "a-declared-count-above-its-ceiling", "ceiling", count.Bytes(),
            VmBudgetDimension.DeclaredCount, VmReason.CeilingReached,
            "A Types count one above the declared-count ceiling, refused before anything proportional to it is reserved."));

        var sections = Smallest();
        sections.SectionCount = (uint)defaults[VmBudgetDimension.DeclaredCount] + 1;
        entries.Add(Exhausted(
            "a-section-count-above-the-declared-count-ceiling", "ceiling", sections.Bytes(),
            VmBudgetDimension.DeclaredCount, VmReason.CeilingReached,
            "A header declaring more sections than the declared-count ceiling: the section count is a declared count."));

        entries.Add(Exhausted(
            "more-sections-than-the-section-ceiling", "ceiling", canonical,
            VmBudgetDimension.SectionCount, VmReason.CeilingReached,
            "The canonical control's four sections under a section-count ceiling of three: entering the fourth is refused, and the same bytes verify under the default.",
            UbcCorpusConfiguration.With(VmBudgetDimension.SectionCount, 3)));

        entries.Add(Exhausted(
            "a-section-deeper-than-the-structural-ceiling", "ceiling", canonical,
            VmBudgetDimension.StructuralDepth, VmReason.CeilingReached,
            "The canonical control under a structural-depth ceiling of zero: a section is one level deep.",
            UbcCorpusConfiguration.With(VmBudgetDimension.StructuralDepth, 0)));

        entries.Add(Exhausted(
            "an-artifact-longer-than-its-requested-ceiling", "ceiling", canonical,
            VmBudgetDimension.ArtifactBytes, VmReason.CeilingReached,
            "The canonical control under an artifact-bytes request one below its length. Refused before the first byte is read.",
            new UbcCorpusConfiguration(artifactBytesRequest: (ulong)canonical.Length - 1)));

        entries.Add(Exhausted(
            "an-allocation-past-the-allowance", "ceiling", canonical,
            VmBudgetDimension.AllocatedBytes, VmReason.AllowanceExhausted,
            "An allocated-bytes allowance of sixteen: the profile identity's twenty-one bytes are reserved before they are copied, and refused.",
            UbcCorpusConfiguration.With(VmBudgetDimension.AllocatedBytes, 16)));

        entries.Add(Exhausted(
            "verifier-work-past-the-allowance", "ceiling", canonical,
            VmBudgetDimension.VerifierWork, VmReason.AllowanceExhausted,
            "A verifier-work allowance of eight: the header's identities cost more than that to read.",
            UbcCorpusConfiguration.With(VmBudgetDimension.VerifierWork, 8)));

        entries.Add(Exhausted(
            "verifier-work-that-runs-out-after-the-reader", "ceiling", canonical,
            VmBudgetDimension.VerifierWork, VmReason.AllowanceExhausted,
            "A verifier-work allowance of the payload's length and one: the reader spends one unit per byte, and the walk and the hook the rest.",
            UbcCorpusConfiguration.With(VmBudgetDimension.VerifierWork, (ulong)canonical.Length + 1)));

        entries.Add(Exhausted(
            "an-allocation-that-runs-out-after-the-reader", "ceiling", canonical,
            VmBudgetDimension.AllocatedBytes, VmReason.AllowanceExhausted,
            "An allocated-bytes allowance of what the reader reserves for the canonical control and one byte more, measured: the walk's first reservation is refused.",
            UbcCorpusConfiguration.With(VmBudgetDimension.AllocatedBytes, ReaderReservation(canonical) + 1)));

        entries.Add(new UbcCorpusEntry(
            "a-cancelled-verification", "ceiling", canonical, FixtureCorpusPinning.Exact,
            new UbcCorpusAnswer(VmOutcome.Cancellation, VmReason.Cancelled, 0, VmBudgetDimension.Fuel, VmBudgetScope.Artifact, UbcCorpusAnswer.Unpinned),
            false,
            new UbcCorpusConfiguration(cancelled: true),
            "The canonical control with a token cancelled before verification: the first poll stops the read, and a stop while the token is cancelled is a cancellation, not an exhaustion."));

        var open = new UbcCorpusConfiguration(ceilings: ImmutableSortedDictionary<VmBudgetDimension, ulong>.Empty
            .Add(VmBudgetDimension.DeclaredCount, uint.MaxValue)
            .Add(VmBudgetDimension.ArtifactBytes, 1UL << 40)
            .Add(VmBudgetDimension.AllocatedBytes, 1UL << 40));

        foreach (var (id, body, note) in new (string, byte[], string)[]
                 {
                     ("a-row-count-far-beyond-the-payload-under-open-ceilings", VarUInt(0x8000_0000),
                         "A Types count of 2^31 under ceilings that admit it: the reservation passes, and the rows run out of payload. No row array is sized from the count."),
                     ("a-slot-type-count-far-beyond-the-payload-under-open-ceilings", [0x01, .. VarUInt(0x8000_0000)],
                         "A signature of 2^31 parameters under ceilings that admit it: the reservation passes, and the parameters run out of payload."),
                 })
        {
            var spec = Smallest();
            spec.Units = null;
            spec.OmitCode = true;
            spec.Entries = null;
            spec.Bodies[(uint)UbcSectionKind.Types] = body;
            var bytes = spec.Bytes();

            entries.Add(Refused(
                id, "ceiling", bytes,
                UbcDiagnosticCode.Truncated, VmReason.Truncated, InSection(UbcSectionKind.Types, (ulong)bytes.Length),
                note, open));
        }
    }

    // ---- the systematic sweeps ---------------------------------------------------------------------------------------

    private static void AddSweeps(List<UbcCorpusEntry> entries)
    {
        var canonical = Canonical();

        for (var length = 0; length < canonical.Length; length++)
        {
            entries.Add(Recorded(
                string.Create(CultureInfo.InvariantCulture, $"truncated-at-{length:D3}"), "truncation-sweep", canonical[..length],
                string.Create(CultureInfo.InvariantCulture, $"The canonical control cut to {length} bytes. No prefix of a valid artifact may verify.")));
        }

        for (var index = 0; index < canonical.Length; index++)
        {
            var damaged = (byte[])canonical.Clone();
            damaged[index] = (byte)~damaged[index];

            entries.Add(Recorded(
                string.Create(CultureInfo.InvariantCulture, $"inverted-byte-{index:D3}"), "corruption-sweep", damaged,
                string.Create(CultureInfo.InvariantCulture, $"Byte {index} of the canonical control inverted. The sweep asserts a closed set of answers that has not moved, not that everything fails.")));
        }
    }

    // ---- construction ----------------------------------------------------------------------------------------------------

    private static UbcUnit With(
        UbcUnit unit,
        uint? typeIndex = null,
        byte? slot = null,
        ImmutableArray<UbcLocalRun>? locals = null,
        uint? maxWords = null,
        uint? codeLength = null,
        UbcUnitFlags? flags = null,
        ImmutableArray<uint>? landings = null) =>
        new(
            typeIndex ?? unit.TypeIndex,
            slot ?? unit.FamilySlot,
            locals ?? unit.Locals,
            maxWords ?? unit.MaxWordHeight,
            unit.MaxValueHeight,
            unit.CodeOffset,
            codeLength ?? unit.CodeLength,
            flags ?? unit.Flags,
            landings ?? unit.Landings);

    private static byte[] VarUInt(ulong value)
    {
        var output = new List<byte>();
        UbcSectionEncoder.WriteVarUInt(output, value);
        return output.ToArray();
    }

    /// <summary>What the reader reserves for <paramref name="payload"/> under the family's defaults: a measured configuration.</summary>
    private static ulong ReaderReservation(byte[] payload)
    {
        var meter = new CountingAllocationMeter();
        var bounds = new VmReadBounds(1UL << 30, 1024, 1UL << 20, 64);

        UbcArtifactReader.TryRead(payload, in bounds, meter, F.MaxUnchargedWork, UbcFormat.FormatVersion, out _, out _);
        return meter.Reserved;
    }

    private sealed class CountingAllocationMeter : IVmBoundedAllocationMeter
    {
        internal ulong Reserved { get; private set; }

        public bool TryReserve(ulong byteCount)
        {
            Reserved += byteCount;
            return true;
        }

        public void Release(ulong byteCount)
        {
        }

        public bool TryChargeWork(ulong workUnits) => true;

        public bool Poll() => true;
    }

    private static string InHeader(ulong offset) => string.Create(CultureInfo.InvariantCulture, $"-1:{offset}:-1:-1");

    private static string InSection(UbcSectionKind kind, ulong offset) => string.Create(CultureInfo.InvariantCulture, $"{(int)kind}:{offset}:-1:-1");

    private static string InCode(int unit, uint pc) => string.Create(CultureInfo.InvariantCulture, $"4:{pc}:{unit}:-1");

    private static string InRow(UbcSectionKind kind, int row) => string.Create(CultureInfo.InvariantCulture, $"{(int)kind}:0:-1:{row}");

    private static UbcCorpusEntry Verifies(string id, string family, byte[] bytes, string note, UbcCorpusConfiguration configuration) =>
        new(id, family, bytes, FixtureCorpusPinning.Exact,
            new UbcCorpusAnswer(VmOutcome.Normal, VmReason.NormalCompleted, 0, VmBudgetDimension.Fuel, VmBudgetScope.Artifact, UbcCorpusAnswer.Unpinned),
            false, configuration, note);

    private static UbcCorpusEntry Refused(
        string id,
        string family,
        byte[] bytes,
        UbcDiagnosticCode code,
        VmReason reason,
        string position,
        string note,
        UbcCorpusConfiguration? configuration = null) =>
        new(id, family, bytes, FixtureCorpusPinning.Exact,
            new UbcCorpusAnswer(VmOutcome.InvalidArtifact, reason, (int)code, VmBudgetDimension.Fuel, VmBudgetScope.Artifact, position),
            false, configuration ?? UbcCorpusConfiguration.Default, note);

    private static UbcCorpusEntry RefusedByFamily(
        string id,
        string family,
        byte[] bytes,
        int familyCode,
        VmReason reason,
        string position,
        string note) =>
        new(id, family, bytes, FixtureCorpusPinning.Exact,
            new UbcCorpusAnswer(VmOutcome.InvalidArtifact, reason, familyCode, VmBudgetDimension.Fuel, VmBudgetScope.Artifact, position),
            false, UbcCorpusConfiguration.Default, note);

    private static UbcCorpusEntry Exhausted(
        string id,
        string family,
        byte[] bytes,
        VmBudgetDimension dimension,
        VmReason reason,
        string note,
        UbcCorpusConfiguration? configuration = null) =>
        new(id, family, bytes, FixtureCorpusPinning.Exact,
            new UbcCorpusAnswer(VmOutcome.ResourceExhaustion, reason, 0, dimension, VmBudgetScope.Artifact, UbcCorpusAnswer.Unpinned),
            true, configuration ?? UbcCorpusConfiguration.Default, note);

    private static UbcCorpusEntry Recorded(string id, string family, byte[] bytes, string note) =>
        new(id, family, bytes, FixtureCorpusPinning.Recorded,
            new UbcCorpusAnswer(VmOutcome.None, VmReason.None, 0, VmBudgetDimension.Fuel, VmBudgetScope.Artifact, UbcCorpusAnswer.Unpinned),
            false, UbcCorpusConfiguration.Default, note);
}
