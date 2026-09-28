using Broiler.VM;
using Broiler.VM.Ubc;
using F = Broiler.VM.Contract.Tests.UbcCorpusFamily;
using O = Broiler.VM.Ubc.UbcOpcode;
using Op = Broiler.VM.Contract.Tests.UbcCorpusFamily.Op;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// The walk over a call row of the signature effect form: the operand names a Types row, the row pops
/// that signature's parameters beneath its trailing pops and pushes its results, and what the walk
/// proved is what the verified program records for the interpreter.
/// </summary>
/// <remarks>
/// The corpus holds the control and the refusal the registry names; these hold the arithmetic the
/// control only implies - the counts per plane, each of the ways the stack beneath a signature row can
/// be wrong, and a named signature whose results rise above the declared height.
/// </remarks>
public sealed class UbcSignatureWalkTests
{
    private const UbcSlotType I32 = UbcSlotType.I32;
    private const UbcSlotType I64 = UbcSlotType.I64;
    private const UbcSlotType V = UbcSlotType.V;

    private static readonly UbcCorpusConfiguration Wide = new(descriptorManifest: F.WideManifestText);

    /// <summary>
    /// One family unit that runs <paramref name="prefix"/>, then the signature row naming the second
    /// Types row unless <paramref name="operand"/> says otherwise, then <paramref name="suffix"/>; the
    /// second Types row is <paramref name="parameters"/> to <paramref name="results"/>, and a second unit
    /// of the family has it.
    /// </summary>
    private static (byte[] Bytes, uint At) Program(
        UbcSlotType[] parameters,
        UbcSlotType[] results,
        UbcSlotType[] mainResults,
        Action<UbcCodeBuilder> prefix,
        Action<UbcCodeBuilder> suffix,
        uint maxWords = 4,
        uint maxValues = 4,
        ulong? operand = null)
    {
        var spec = new UbcCorpusSpec { Manifest = F.WideManifestText }.WithFamily(version: F.WideTableVersion);
        var main = spec.AddType([], mainResults);
        var called = spec.AddType(parameters, results);
        var at = 0U;

        var unit = spec.AddUnit(main, 1, maxWords, maxValues, UbcUnitFlags.Entry, b =>
        {
            prefix(b);
            at = b.Offset;
            b.F(Op.CallSignature, operand ?? (ulong)called);
            suffix(b);
            return [];
        });

        spec.AddUnit(called, 1, 4, 4, UbcUnitFlags.None, b =>
        {
            foreach (var result in results)
            {
                if (result == V)
                {
                    b.F(Op.PushSmall, 0, slot: 1);
                }
                else
                {
                    b.Emit(result == I64 ? O.ConstI64 : O.ConstI32, 0);
                }
            }

            b.Emit(O.Return);
            return [];
        });

        spec.AddEntry("main", unit);
        return (spec.Bytes(), at);
    }

    [Fact]
    public void The_Control_Records_The_Named_Signature_Beneath_The_Trailing_Pop_On_Each_Plane()
    {
        var observation = UbcCorpusRunner.Run(UbcCorpus.SignatureCall().Bytes(), Wide);
        var program = Assert.IsType<UbcVerifiedProgram>(observation.Outcome.State);
        var call = program.Units[0].Instructions.Single(static instruction => instruction.Row is { Effect.Form: UbcEffectForm.Signature });

        // Before the row: i64 and i32 on the word plane, one value. The row pops the named
        // signature's parameters - the i64 and the value - and the trailing i32, and pushes its
        // results - a value and an i32.
        Assert.Equal(2, call.WordHeight);
        Assert.Equal(1, call.ValueHeight);
        Assert.Equal(2, call.WordPops);
        Assert.Equal(1, call.ValuePops);
        Assert.Equal(1, call.WordPushes);
        Assert.Equal(1, call.ValuePushes);
        Assert.Equal(1UL, call.Operand);
        Assert.Equal(-1, call.Target);
    }

    [Fact]
    public void A_Signature_Row_Whose_Trailing_Slot_Is_Not_Its_Type_Is_Refused()
    {
        var (bytes, at) = Program(
            [I64], [I32], [I32],
            b => b.Emit(O.ConstI64, 1).Emit(O.ConstI64, 0),
            b => b.Emit(O.Return));

        Assert.Equal(
            new UbcCorpusAnswer(VmOutcome.InvalidArtifact, VmReason.InconsistentStructure, (int)UbcDiagnosticCode.StackTypeMismatch,
                VmBudgetDimension.Fuel, VmBudgetScope.Artifact, $"4:{at}:0:-1"),
            UbcCorpusRunner.Run(bytes, Wide).Answer);
    }

    [Fact]
    public void A_Signature_Row_Over_A_Parameter_Of_Another_Type_Is_Refused()
    {
        var (bytes, at) = Program(
            [I64, V], [I32], [I32],
            b => b.Emit(O.ConstI32, 1).F(Op.PushSmall, 1).Emit(O.ConstI32, 0),
            b => b.Emit(O.Return));

        Assert.Equal(
            new UbcCorpusAnswer(VmOutcome.InvalidArtifact, VmReason.InconsistentStructure, (int)UbcDiagnosticCode.StackTypeMismatch,
                VmBudgetDimension.Fuel, VmBudgetScope.Artifact, $"4:{at}:0:-1"),
            UbcCorpusRunner.Run(bytes, Wide).Answer);
    }

    [Fact]
    public void A_Signature_Row_Over_Too_Few_Parameters_Underflows()
    {
        var (bytes, at) = Program(
            [I64, I64], [I32], [I32],
            b => b.Emit(O.ConstI64, 1).Emit(O.ConstI32, 0),
            b => b.Emit(O.Return));

        Assert.Equal(
            new UbcCorpusAnswer(VmOutcome.InvalidArtifact, VmReason.InconsistentStructure, (int)UbcDiagnosticCode.StackUnderflow,
                VmBudgetDimension.Fuel, VmBudgetScope.Artifact, $"4:{at}:0:-1"),
            UbcCorpusRunner.Run(bytes, Wide).Answer);
    }

    [Fact]
    public void A_Named_Signature_Whose_Results_Rise_Above_The_Declared_Height_Is_Refused()
    {
        var (bytes, at) = Program(
            [], [I32, I32, I32], [I32],
            b => b.Emit(O.ConstI32, 0),
            b => b.Emit(O.Drop).Emit(O.Drop).Emit(O.Return),
            maxWords: 2);

        Assert.Equal(
            new UbcCorpusAnswer(VmOutcome.InvalidArtifact, VmReason.InconsistentStructure, (int)UbcDiagnosticCode.HeightAboveDeclared,
                VmBudgetDimension.Fuel, VmBudgetScope.Artifact, $"4:{at}:0:-1"),
            UbcCorpusRunner.Run(bytes, Wide).Answer);
    }

    [Fact]
    public void An_Operand_One_Past_The_Types_Section_Is_Refused_And_The_Last_Row_Is_Admitted()
    {
        // Two Types rows: operand 2 is the first that names none, operand 1 the last that names one.
        var (past, at) = Program([], [], [I32], b => b.Emit(O.ConstI32, 0), b => b.Emit(O.ConstI32, 5).Emit(O.Return), operand: 2);

        Assert.Equal(
            new UbcCorpusAnswer(VmOutcome.InvalidArtifact, VmReason.InconsistentStructure, (int)UbcDiagnosticCode.SignatureTypeOutOfRange,
                VmBudgetDimension.Fuel, VmBudgetScope.Artifact, $"4:{at}:0:-1"),
            UbcCorpusRunner.Run(past, Wide).Answer);

        var (last, _) = Program([], [], [I32], b => b.Emit(O.ConstI32, 0), b => b.Emit(O.ConstI32, 5).Emit(O.Return), operand: 1);
        Assert.Equal(VmOutcome.Normal, UbcCorpusRunner.Run(last, Wide).Outcome.Category);
    }

    [Fact]
    public void A_Signature_Row_Naming_The_Callers_Own_Signature_Pops_Nothing_Beneath_The_Trailing_Slot()
    {
        // Operand 0 is the entry unit's own signature, [] -> [i32]: the row pops the trailing i32
        // alone and pushes an i32, so the stack it leaves is the one it found.
        var (bytes, _) = Program([], [], [I32], b => b.Emit(O.ConstI32, 0), b => b.Emit(O.Return), operand: 0);
        var program = Assert.IsType<UbcVerifiedProgram>(UbcCorpusRunner.Run(bytes, Wide).Outcome.State);
        var call = program.Units[0].Instructions.Single(static instruction => instruction.Row is { Effect.Form: UbcEffectForm.Signature });

        Assert.Equal((1, 0, 1, 0), (call.WordPops, call.ValuePops, call.WordPushes, call.ValuePushes));
    }
}
