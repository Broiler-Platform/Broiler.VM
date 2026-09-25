using Broiler.VM;
using Broiler.VM.Ubc;
using System.Collections.Immutable;

namespace Com.Example.Tally;

/// <summary>
/// The fixture family's instruction table: one row per instruction kind and the primitive rows the
/// program list needs, one region kind, and a trap vocabulary of four codes.
/// </summary>
/// <remarks>
/// <para>
/// The concept's section 8.3 asks for three rows - one dynamic, one primitive, one throw. The program
/// corpus the roadmap asks for needs a suspension, a call request, a branch and a guest load as well, so
/// every kind has a row here; that is recorded as a deviation in the milestone's bundle.
/// </para>
/// <para>
/// <b>Rows</b>, all opcodes of slot <see cref="TallyProfile.Slot"/>:
/// <c>tally.const</c> (dynamic, <c>I32</c>, <c>[] -&gt; [v]</c>, the hook refuses an operand outside
/// plus or minus one million), <c>tally.add</c> (dynamic, <c>[v v] -&gt; [v]</c>), <c>tally.count</c>
/// (dynamic, <c>[v] -&gt; [i64]</c>), <c>tally.i64.add</c>, <c>tally.i64.mul</c>,
/// <c>tally.i64.div</c> and <c>tally.i64.eqz</c> (primitive), <c>tally.throw</c> (throw, terminal),
/// <c>tally.if_positive</c> (branch), <c>tally.call</c> (call request into this program),
/// <c>tally.yield</c> (suspend) and <c>tally.load</c> (call request into a guest-loaded program).
/// </para>
/// </remarks>
public static class TallyTable
{
    /// <summary>The family's identity, which is also its profile identity.</summary>
    public const string Identity = "com.example.tally";

    /// <summary>The identity of the one feature manifest that selects this table.</summary>
    public const string ManifestIdentity = "com.example.tally.base";

    /// <summary><c>tally.const</c>: a tally of the operand.</summary>
    public const byte Const = 0x01;

    /// <summary><c>tally.add</c>: the sum of two tallies.</summary>
    public const byte Add = 0x02;

    /// <summary><c>tally.count</c>: a tally's amount as a word.</summary>
    public const byte Count = 0x03;

    /// <summary><c>tally.i64.add</c>: the primitive <c>i64.add</c>.</summary>
    public const byte AddWords = 0x04;

    /// <summary><c>tally.i64.mul</c>: the primitive <c>i64.mul</c>.</summary>
    public const byte MulWords = 0x05;

    /// <summary><c>tally.i64.div</c>: the primitive <c>i64.div_s</c>, whose two traps are mapped.</summary>
    public const byte DivWords = 0x06;

    /// <summary><c>tally.i64.eqz</c>: the primitive <c>i64.eqz</c>.</summary>
    public const byte IsZero = 0x07;

    /// <summary><c>tally.throw</c>: throws a tally.</summary>
    public const byte Throw = 0x08;

    /// <summary><c>tally.if_positive</c>: branches when a tally is above zero.</summary>
    public const byte IfPositive = 0x09;

    /// <summary><c>tally.call</c>: asks the emitter to enter the unit the operand names.</summary>
    public const byte CallUnit = 0x0A;

    /// <summary><c>tally.yield</c>: suspends with a tally, which the resumption pushes back.</summary>
    public const byte Yield = 0x0B;

    /// <summary><c>tally.load</c>: loads the guest program the operand names and enters its <c>main</c>.</summary>
    public const byte Load = 0x0C;

    /// <summary>The one region kind: a catch, which lands with the thrown tally.</summary>
    public const byte Catch = 0x01;

    /// <summary>Trap code: an integer division by zero.</summary>
    public const ushort TrapDivideByZero = 1;

    /// <summary>Trap code: a result that does not fit.</summary>
    public const ushort TrapOverflow = 2;

    /// <summary>Trap code: a program's own <c>trap</c> instruction.</summary>
    public const ushort TrapExplicit = 3;

    /// <summary>Trap code: a guest load the composition did not answer with a program of this family.</summary>
    public const ushort TrapLoadRefused = 4;

    /// <summary>The largest magnitude <c>tally.const</c> admits; the hook refuses a larger one.</summary>
    public const int ConstRange = 1_000_000;

    /// <summary>The hook's one diagnostic code: a <c>tally.const</c> operand outside the declared range.</summary>
    public const int ConstOutOfRange = 7001;

    /// <summary>The table the family's one manifest selects.</summary>
    public static UbcInstructionTable Table { get; } = Build();

    private static UbcInstructionTable Build()
    {
        var none = ImmutableArray<UbcSlotType>.Empty;
        var v = ImmutableArray.Create(UbcSlotType.V);
        var vv = ImmutableArray.Create(UbcSlotType.V, UbcSlotType.V);
        var i64 = ImmutableArray.Create(UbcSlotType.I64);
        var i64i64 = ImmutableArray.Create(UbcSlotType.I64, UbcSlotType.I64);
        var i32 = ImmutableArray.Create(UbcSlotType.I32);

        UbcInstructionRow Primitive(byte opcode, string mnemonic, UbcPrimitive primitive, ImmutableArray<UbcSlotType> pops, ImmutableArray<UbcSlotType> pushes, ImmutableArray<UbcTrapMapping> traps = default) =>
            new(opcode, mnemonic, UbcOperandShape.None, UbcEffect.Listed(pops, pushes), UbcTarget.None, UbcInstructionKind.Primitive, primitive: primitive, traps: traps);

        var rows = new[]
        {
            new UbcInstructionRow(Const, "tally.const", UbcOperandShape.I32, UbcEffect.Listed(none, v), UbcTarget.None, UbcInstructionKind.Dynamic),
            new UbcInstructionRow(Add, "tally.add", UbcOperandShape.None, UbcEffect.Listed(vv, v), UbcTarget.None, UbcInstructionKind.Dynamic),
            new UbcInstructionRow(Count, "tally.count", UbcOperandShape.None, UbcEffect.Listed(v, i64), UbcTarget.None, UbcInstructionKind.Dynamic),
            Primitive(AddWords, "tally.i64.add", UbcPrimitive.I64Add, i64i64, i64),
            Primitive(MulWords, "tally.i64.mul", UbcPrimitive.I64Mul, i64i64, i64),
            Primitive(DivWords, "tally.i64.div", UbcPrimitive.I64DivS, i64i64, i64, ImmutableArray.Create(
                new UbcTrapMapping(UbcTrapCode.DivideByZero, TrapDivideByZero),
                new UbcTrapMapping(UbcTrapCode.IntegerOverflow, TrapOverflow))),
            Primitive(IsZero, "tally.i64.eqz", UbcPrimitive.I64Eqz, i64, i32),
            new UbcInstructionRow(Throw, "tally.throw", UbcOperandShape.None, UbcEffect.Listed(v, none), UbcTarget.None, UbcInstructionKind.Throw, isTerminal: true),
            new UbcInstructionRow(IfPositive, "tally.if_positive", UbcOperandShape.U32, UbcEffect.Listed(v, none), UbcTarget.Code(), UbcInstructionKind.Branch),
            new UbcInstructionRow(CallUnit, "tally.call", UbcOperandShape.U32, UbcEffect.Listed(v, v), UbcTarget.None, UbcInstructionKind.Call),
            new UbcInstructionRow(Yield, "tally.yield", UbcOperandShape.None, UbcEffect.Listed(v, v), UbcTarget.None, UbcInstructionKind.Suspend),
            new UbcInstructionRow(Load, "tally.load", UbcOperandShape.U16, UbcEffect.Listed(v, v), UbcTarget.None, UbcInstructionKind.Call),
        };

        var created = UbcInstructionTable.TryCreate(
            Identity,
            1,
            VmFeatureManifestId.Parse(ManifestIdentity),
            rows,
            [new UbcRegionKindRow(Catch, "catch", v)],
            [],
            [
                new UbcFamilyTrap(TrapDivideByZero, "divide-by-zero"),
                new UbcFamilyTrap(TrapOverflow, "overflow"),
                new UbcFamilyTrap(TrapExplicit, "explicit"),
                new UbcFamilyTrap(TrapLoadRefused, "load-refused"),
            ],
            canonicaliseNaN: false,
            out var table,
            out var defect);

        return created ? table! : throw new System.InvalidOperationException(defect);
    }
}

/// <summary>The fixture family's verifier hook: it refuses one thing, a <c>tally.const</c> operand out of range.</summary>
public sealed class TallyVerifier : IUbcFamilyVerifier
{
    /// <inheritdoc/>
    public UbcHookAnswer Begin(UbcHookArtifact artifact, out object? familyState)
    {
        familyState = null;
        return UbcHookAnswer.Admit;
    }

    /// <inheritdoc/>
    public UbcHookAnswer CheckInstruction(object? familyState, in UbcHookInstruction instruction)
    {
        if (instruction.Opcode == TallyTable.Const)
        {
            var amount = (int)(uint)instruction.Operand;

            if (amount is < -TallyTable.ConstRange or > TallyTable.ConstRange)
            {
                return UbcHookAnswer.Refuse(TallyTable.ConstOutOfRange, VmReason.SemanticValidationFailed);
            }
        }

        return UbcHookAnswer.Admit;
    }

    /// <inheritdoc/>
    public UbcHookAnswer End(object? familyState) => UbcHookAnswer.Admit;
}
