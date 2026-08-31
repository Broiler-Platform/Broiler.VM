// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   22
// Annotated:        22/22
// Exempt:           0
// Human-reviewed:   0/22
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       22
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The programs JS-1 lowers, each one a claim about the language rather than about arithmetic.
/// </summary>
/// <remarks>
/// <para>
/// Every program here is chosen because a plausible implementation gets it wrong. An interpreter
/// over bare doubles answers <c>1 &lt; 2</c> with <c>1</c>; one that reached for a C# cast answers
/// <c>2147483648 | 0</c> with <c>2147483647</c>; one that guarded division answers <c>1 / 0</c>
/// with a fault; one that used a floored modulo answers <c>-5 % 3</c> with <c>1</c>. Each of those
/// is a defect no corpus of addition would find, which is the point of choosing the smallest
/// JavaScript that is still JavaScript rather than the smallest thing that runs.
/// </para>
/// <para>
/// The structured lowerings - <see cref="CountingLoop"/> and <see cref="Conditional"/> - exist for
/// a different reason: they are the ones that emit backward and forward branches and therefore the
/// ones that prove the format's jumps, the builder's patching and the verifier's join checking
/// agree. A straight-line corpus would exercise none of that.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=2; Fingerprint=3DBF25
// Broiler-Human:        PENDING
public static class SliceLowering
{
    /// <summary>The manifest every program below is lowered for.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=1DB034
    // Broiler-Human:        PENDING
    public const string SliceManifestId = "broiler.javascript.slice";

    /// <summary>The entry-point name this milestone's lowering uses.</summary>
    /// <remarks>
    /// <para>
    /// <b>This is the entry-point answer roadmap section 10 asks JS-1 to record.</b> An invocation
    /// request carries one UTF-8 name and nothing else - no argument channel and no return channel
    /// except a typed payload - and the answer taken here is the first of the three the roadmap
    /// offers: the artifact declares named program entries and the caller names one. Arguments,
    /// where a program needs them, are encoded by the lowering into the artifact the host asked
    /// for, which is what a browser does anyway because it compiles a PROGRAM rather than a call.
    /// </para>
    /// <para>
    /// The cost of that answer is recorded rather than hidden: a host that wants to call
    /// <c>f(1, 2)</c> against an already-instantiated realm cannot, and must lower a new program.
    /// Decision JSD-0008 records it, and roadmap section 18's argument-channel row - re-graded
    /// strong at JS-0 - is the amendment that would remove the cost.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=A4BCCA
    // Broiler-Human:        PENDING
    public const string MainEntry = "main";

    /// <summary>A program whose completion value is <paramref name="value"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=57DBB2
    // Broiler-Human:        PENDING
    public static byte[] Constant(double value) => Program(builder => builder
        .Entry(MainEntry)
        .Position(1, 1)
        .LoadNumber(value)
        .Emit(JavaScriptOpcode.Return));

    /// <summary><c>20 + 22</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=C2C543
    // Broiler-Human:        PENDING
    public static byte[] Addition() => Binary(20, 22, JavaScriptOpcode.Add);

    /// <summary><c>1 / 0</c>, whose value is <c>Infinity</c> and not a fault.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=84A1FB
    // Broiler-Human:        PENDING
    public static byte[] DivisionByZero() => Binary(1, 0, JavaScriptOpcode.Divide);

    /// <summary><c>0 / 0</c>, whose value is <c>NaN</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=143261
    // Broiler-Human:        PENDING
    public static byte[] NotANumber() => Binary(0, 0, JavaScriptOpcode.Divide);

    /// <summary><c>1 / -0</c>, whose value is <c>-Infinity</c>.</summary>
    /// <remarks>
    /// The one program that depends on the constant pool distinguishing <c>-0</c> from <c>+0</c>.
    /// An interning scheme keyed on numeric equality would fold them together and this would
    /// answer <c>Infinity</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=A35A78
    // Broiler-Human:        PENDING
    public static byte[] NegativeZeroDivision() => Binary(1, -0.0, JavaScriptOpcode.Divide);

    /// <summary><c>-5 % 3</c>, whose value is <c>-2</c>: the remainder takes the dividend's sign.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=BA6A9D
    // Broiler-Human:        PENDING
    public static byte[] RemainderSign() => Binary(-5, 3, JavaScriptOpcode.Rem);

    /// <summary><c>2147483648 | 0</c>, whose value is <c>-2147483648</c> through <c>ToInt32</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=7BB75B
    // Broiler-Human:        PENDING
    public static byte[] ToInt32Wraps() => Binary(2147483648d, 0, JavaScriptOpcode.BitwiseOr);

    /// <summary><c>-1 &gt;&gt;&gt; 0</c>, whose value is <c>4294967295</c> through <c>ToUint32</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=AB6076
    // Broiler-Human:        PENDING
    public static byte[] UnsignedShiftIsUnsigned() =>
        Binary(-1, 0, JavaScriptOpcode.ShiftRightUnsigned);

    /// <summary><c>1 &lt; 2</c>, whose value is the Boolean <c>true</c> and not the Number <c>1</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=9A9737
    // Broiler-Human:        PENDING
    public static byte[] ComparisonProducesABoolean() => Binary(1, 2, JavaScriptOpcode.LessThan);

    /// <summary><c>(0 / 0) === (0 / 0)</c>, whose value is <c>false</c>.</summary>
    /// <remarks>
    /// NaN is the one value not strictly equal to itself, and it is computed twice here rather
    /// than duplicated from one slot, so the answer does not depend on identity.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=1DF64A
    // Broiler-Human:        PENDING
    public static byte[] NotANumberIsNotItself() => Program(builder => builder
        .Entry(MainEntry)
        .LoadNumber(0).LoadNumber(0).Emit(JavaScriptOpcode.Divide)
        .LoadNumber(0).LoadNumber(0).Emit(JavaScriptOpcode.Divide)
        .Emit(JavaScriptOpcode.StrictEquals)
        .Emit(JavaScriptOpcode.Return));

    /// <summary><c>true + 1</c>, whose value is <c>2</c> because <c>ToNumber(true)</c> is 1.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=D4069E
    // Broiler-Human:        PENDING
    public static byte[] BooleanAddsAsOne() => Program(builder => builder
        .Entry(MainEntry)
        .LoadBoolean(true)
        .LoadNumber(1)
        .Emit(JavaScriptOpcode.Add)
        .Emit(JavaScriptOpcode.Return));

    /// <summary><c>1 === true</c>, whose value is <c>false</c> because the kinds differ.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=80AC99
    // Broiler-Human:        PENDING
    public static byte[] StrictEqualityComparesKinds() => Program(builder => builder
        .Entry(MainEntry)
        .LoadNumber(1)
        .LoadBoolean(true)
        .Emit(JavaScriptOpcode.StrictEquals)
        .Emit(JavaScriptOpcode.Return));

    /// <summary>A local that was declared and never assigned, whose value is <c>undefined</c>.</summary>
    /// <remarks>
    /// Which is what <c>var</c> does in the language. The slice has no lexical declaration and
    /// therefore no temporal dead zone; JS-3b is where reading a <c>let</c> before its initialiser
    /// stops being the same question.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=C1E897
    // Broiler-Human:        PENDING
    public static byte[] UnassignedLocalIsUndefined() => Program(builder =>
    {
        var slot = builder.DeclareLocal();
        return builder
            .Entry(MainEntry)
            .LoadLocal(slot)
            .Emit(JavaScriptOpcode.Return);
    });

    /// <summary>
    /// <c>var total = 0, index = 0; while (index &lt; n) { index = index + 1; total = total + index; }
    /// return total;</c>
    /// </summary>
    /// <remarks>
    /// The program that emits a forward branch out of the loop and a backward branch into it, so
    /// the verifier's join checking has two edges arriving at one offset with a height to agree
    /// about. Its answer is the triangular number of <paramref name="count"/>, which is a value a
    /// reader can check without running anything.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=FE38CF
    // Broiler-Human:        PENDING
    public static byte[] CountingLoop(double count) => Program(builder =>
    {
        var total = builder.DeclareLocal();
        var index = builder.DeclareLocal();
        var test = builder.DefineLabel();
        var done = builder.DefineLabel();

        builder.Entry(MainEntry)
            .Position(1, 1)
            .LoadNumber(0).StoreLocal(total)
            .LoadNumber(0).StoreLocal(index)
            .MarkLabel(test)
            .LoadLocal(index).LoadNumber(count).Emit(JavaScriptOpcode.LessThan)
            .Branch(JavaScriptOpcode.JumpIfFalse, done)
            .LoadLocal(index).LoadNumber(1).Emit(JavaScriptOpcode.Add).StoreLocal(index)
            .LoadLocal(total).LoadLocal(index).Emit(JavaScriptOpcode.Add).StoreLocal(total)
            .Branch(JavaScriptOpcode.Jump, test)
            .MarkLabel(done)
            .LoadLocal(total)
            .Emit(JavaScriptOpcode.Return);

        return builder;
    });

    /// <summary><c>return condition ? whenTrue : whenFalse;</c> lowered as a branch.</summary>
    /// <remarks>
    /// Two paths reach the return with the same operand-stack height and different values, which
    /// is the join the verifier admits. A lowering that left an extra value on one arm would
    /// produce two heights at one offset, and that is the case rule the verifier refuses.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=4148A6
    // Broiler-Human:        PENDING
    public static byte[] Conditional(bool condition, double whenTrue, double whenFalse) => Program(builder =>
    {
        var otherwise = builder.DefineLabel();
        var end = builder.DefineLabel();

        builder.Entry(MainEntry)
            .LoadBoolean(condition)
            .Branch(JavaScriptOpcode.JumpIfFalse, otherwise)
            .LoadNumber(whenTrue)
            .Branch(JavaScriptOpcode.Jump, end)
            .MarkLabel(otherwise)
            .LoadNumber(whenFalse)
            .MarkLabel(end)
            .Emit(JavaScriptOpcode.Return);

        return builder;
    });

    /// <summary>
    /// A program that runs forever, for a host that needs to prove a budget or a cancellation
    /// bites.
    /// </summary>
    /// <remarks>
    /// It is a verified artifact and an infinite one, which is the pair of properties that makes
    /// it useful: nothing about it is malformed, so a run that stops has stopped for a reason the
    /// budget or the token gave rather than because the verifier refused it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=9B2B2F
    // Broiler-Human:        PENDING
    public static byte[] Forever() => Program(builder =>
    {
        var top = builder.DefineLabel();

        builder.Entry(MainEntry)
            .MarkLabel(top)
            .Branch(JavaScriptOpcode.Jump, top);

        return builder;
    });

    /// <summary>A program declaring a second entry point, so a caller can choose between them.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=9C360B
    // Broiler-Human:        PENDING
    public static byte[] TwoEntryPoints() => Program(builder =>
    {
        builder.Entry(MainEntry)
            .LoadNumber(1)
            .Emit(JavaScriptOpcode.Return)
            .Entry("second")
            .LoadNumber(2)
            .Emit(JavaScriptOpcode.Return);

        return builder;
    });

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=BCAF03
    // Broiler-Human:        PENDING
    private static byte[] Binary(double left, double right, JavaScriptOpcode opcode) => Program(builder => builder
        .Entry(MainEntry)
        .Position(1, 1)
        .LoadNumber(left)
        .LoadNumber(right)
        .Emit(opcode)
        .Emit(JavaScriptOpcode.Return));

    /// <summary>
    /// Runs one lowering and frames it, declaring an operand-stack maximum this surface never
    /// exceeds.
    /// </summary>
    /// <remarks>
    /// The declared maximum is a fixed generous number rather than a computed one. The verifier
    /// computes the real high-water mark itself and stores it, so a declaration that agreed by
    /// construction would make the comparison between the two vacuous.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=AA60EE
    // Broiler-Human:        PENDING
    private static byte[] Program(System.Func<SliceProgramBuilder, SliceProgramBuilder> lower) =>
        lower(new SliceProgramBuilder()).ToArtifact(SliceManifestId, declaredOperandStack: 16);
}
