// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   60
// Annotated:        60/60
// Exempt:           120
// Human-reviewed:   0/60
// IP risk:          Low
// Security risk:    Critical
// Criteria:         38/38
// Resource impact:  3/10 max
// Unverified:       60
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>What the value form's emitted code does for one instruction itself, if anything.</summary>
/// <remarks>
/// <b>EVERY MEMBER BUT <see cref="None"/> IS AN INLINE TEMPLATE OF THE PURE SET (JSD-0035 section 5)</b>: an
/// instruction whose effect is on the slab alone and which cannot run guest code, guarded by a type test
/// where the machine instruction is exact on part of its domain only. <see cref="None"/> is a helper call,
/// which is what every other instruction is, and what a guarded one becomes when its guard fails.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=231C64
// Broiler-Falsified-If: a member other than None names an instruction outside the pure set, or an instruction the pure set holds has no member
// Broiler-Human:        PENDING
public enum JsValueInline : byte
{
    /// <summary>A helper call: the interpreter's arm runs the instruction.</summary>
    None = 0,

    /// <summary><c>Nop</c>, and a <c>CopyScope</c> of a resident environment.</summary>
    Nop,

    /// <summary>A word known when the unit is emitted: a special constant, or a Number constant.</summary>
    LoadWord,

    /// <summary><c>LoadArgument</c>, from the region's argument words.</summary>
    LoadArgument,

    /// <summary><c>LoadScoped</c> of a resident binding, guarded against its dead zone.</summary>
    LoadResident,

    /// <summary><c>StoreScoped</c> to a resident binding, guarded against its dead zone.</summary>
    StoreResident,

    /// <summary><c>InitialiseScoped</c> of a resident binding.</summary>
    InitialiseResident,

    /// <summary><c>Pop</c>.</summary>
    Pop,

    /// <summary><c>Duplicate</c>.</summary>
    Duplicate,

    /// <summary><c>DuplicateTwo</c>.</summary>
    DuplicateTwo,

    /// <summary><c>Swap</c>.</summary>
    Swap,

    /// <summary><c>Pick</c>.</summary>
    Pick,

    /// <summary><c>Void</c>.</summary>
    Void,

    /// <summary><c>Add</c>, <c>Subtract</c>, <c>Multiply</c> or <c>Divide</c> on two Numbers.</summary>
    Arithmetic,

    /// <summary><c>Negate</c> of a Number.</summary>
    Negate,

    /// <summary><c>Increment</c> or <c>Decrement</c> of a Number.</summary>
    Step,

    /// <summary><c>ToNumber</c> or <c>ToNumeric</c> of a Number, which leaves it as it is.</summary>
    ToNumber,

    /// <summary>A relational or equality comparison of two Numbers.</summary>
    Compare,

    /// <summary>A bitwise or shift operator on two Numbers inside the 32-bit range.</summary>
    Bitwise,

    /// <summary><c>BitwiseNot</c> of a Number inside the 32-bit range.</summary>
    BitwiseNot,

    /// <summary><c>Not</c> of a Boolean.</summary>
    Not,

    /// <summary><c>Jump</c>.</summary>
    Jump,

    /// <summary><c>JumpIfFalse</c> or <c>JumpIfTrue</c> on a Boolean, a Number, <c>undefined</c> or <c>null</c>.</summary>
    Branch,

    /// <summary>
    /// <c>Return</c> or <c>ReturnUndefined</c>: the value into the region's first word, the debt into the
    /// context, and out with <see cref="JsValueAbi.Returned"/> (stage JSV-3).
    /// </summary>
    Return,
}

/// <summary>The templates a value-form unit's body is made of, by what they do.</summary>
/// <remarks>
/// <b>Each member is one row of the value table, found there by <see cref="JsValueLayout.TemplateName"/></b>;
/// the prologue's and the epilogue's rows are fixed and a layout does not state them.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B0A4AE
// Broiler-Human:        PENDING
public enum JsValueTemplate : byte
{
    /// <summary><c>test eax, eax</c>.</summary>
    TestEax,

    /// <summary><c>js rel32</c>.</summary>
    JsLeave,

    /// <summary><c>cmp eax, pc</c>.</summary>
    CmpEaxPc,

    /// <summary><c>je rel32</c>.</summary>
    Je,

    /// <summary><c>jne rel32</c>.</summary>
    Jne,

    /// <summary><c>ja rel32</c>.</summary>
    Ja,

    /// <summary><c>jmp rel32</c>.</summary>
    Jmp,

    /// <summary><c>mov eax, status</c>.</summary>
    MovStatus,

    /// <summary><c>mov arg0, r14</c>.</summary>
    MovArg0R14,

    /// <summary><c>mov arg1d, pc</c>.</summary>
    MovArg1Pc,

    /// <summary><c>call [rbx+slot]</c>.</summary>
    CallSlot,

    /// <summary><c>call [rbx+settle]</c>.</summary>
    CallSettle,

    /// <summary><c>mov [r14+debt], r12</c>.</summary>
    SpillDebt,

    /// <summary><c>xor r12d, r12d</c>.</summary>
    ClearDebt,

    /// <summary><c>inc r12</c>.</summary>
    IncDebt,

    /// <summary><c>dec r12</c>.</summary>
    DecDebt,

    /// <summary><c>cmp r12, threshold</c>.</summary>
    CmpDebt,

    /// <summary><c>jae rel32</c>.</summary>
    Jae,

    /// <summary><c>jp rel32</c>.</summary>
    Jp,

    /// <summary><c>mov rax, [r15+slot]</c>.</summary>
    LoadRax,

    /// <summary><c>mov rdx, [r15+slot]</c>.</summary>
    LoadRdx,

    /// <summary><c>mov [r15+slot], rax</c>.</summary>
    StoreRax,

    /// <summary><c>mov [r15+slot], rdx</c>.</summary>
    StoreRdx,

    /// <summary><c>mov rax, word</c>.</summary>
    MovRaxWord,

    /// <summary><c>mov rcx, word</c>.</summary>
    MovRcxWord,

    /// <summary><c>cmp rax, rcx</c>.</summary>
    CmpRaxRcx,

    /// <summary><c>cmp rdx, rcx</c>.</summary>
    CmpRdxRcx,

    /// <summary><c>movq xmm0, rax</c>.</summary>
    MovqXmm0Rax,

    /// <summary><c>movq xmm1, rdx</c>.</summary>
    MovqXmm1Rdx,

    /// <summary><c>movq xmm1, rcx</c>.</summary>
    MovqXmm1Rcx,

    /// <summary><c>movq rax, xmm0</c>.</summary>
    MovqRaxXmm0,

    /// <summary><c>addsd xmm0, xmm1</c>.</summary>
    Addsd,

    /// <summary><c>subsd xmm0, xmm1</c>.</summary>
    Subsd,

    /// <summary><c>mulsd xmm0, xmm1</c>.</summary>
    Mulsd,

    /// <summary><c>divsd xmm0, xmm1</c>.</summary>
    Divsd,

    /// <summary><c>ucomisd xmm0, xmm1</c>.</summary>
    Ucomisd01,

    /// <summary><c>ucomisd xmm1, xmm0</c>.</summary>
    Ucomisd10,

    /// <summary><c>xorpd xmm1, xmm1</c>.</summary>
    XorpdXmm1,

    /// <summary><c>setcc al</c>.</summary>
    SetccAl,

    /// <summary><c>setcc dl</c>.</summary>
    SetccDl,

    /// <summary><c>and al, dl</c>.</summary>
    AndAlDl,

    /// <summary><c>or al, dl</c>.</summary>
    OrAlDl,

    /// <summary><c>movzx eax, al</c>.</summary>
    MovzxEaxAl,

    /// <summary><c>add rax, rcx</c>.</summary>
    AddRaxRcx,

    /// <summary><c>xor rax, rcx</c>.</summary>
    XorRaxRcx,

    /// <summary><c>xor rax, 1</c>.</summary>
    XorRax1,

    /// <summary><c>cvttsd2si eax, xmm0</c>.</summary>
    Cvttsd2siEax,

    /// <summary><c>cvttsd2si ecx, xmm1</c>.</summary>
    Cvttsd2siEcx,

    /// <summary><c>cmp eax, int32min</c>.</summary>
    CmpEaxMin,

    /// <summary><c>cmp ecx, int32min</c>.</summary>
    CmpEcxMin,

    /// <summary><c>or eax, ecx</c>.</summary>
    OrEaxEcx,

    /// <summary><c>and eax, ecx</c>.</summary>
    AndEaxEcx,

    /// <summary><c>xor eax, ecx</c>.</summary>
    XorEaxEcx,

    /// <summary><c>shl eax, cl</c>.</summary>
    ShlEaxCl,

    /// <summary><c>sar eax, cl</c>.</summary>
    SarEaxCl,

    /// <summary><c>shr eax, cl</c>.</summary>
    ShrEaxCl,

    /// <summary><c>not eax</c>.</summary>
    NotEax,

    /// <summary><c>movsxd rax, eax</c>.</summary>
    MovsxdRaxEax,

    /// <summary><c>cvtsi2sd xmm0, rax</c>.</summary>
    Cvtsi2sd,

    /// <summary><c>lea arg2, [rsp+callee]</c>: the callee context, as a helper's third argument.</summary>
    LeaArg2Callee,

    /// <summary><c>call [rbx+prepare]</c>: the call-prepare helper.</summary>
    CallPrepare,

    /// <summary><c>call [rbx+finish]</c>: the call-finish helper.</summary>
    CallFinish,

    /// <summary><c>cmp eax, direct</c>: whether the prepare helper asked for a direct call.</summary>
    CmpEaxDirect,

    /// <summary><c>lea arg0, [rsp+callee]</c>: the callee context, as the direct callee's frame.</summary>
    LeaArg0Callee,

    /// <summary><c>mov arg1d, [rsp+callee+entrypc]</c>: the offset the direct callee is entered at.</summary>
    MovArg1EntryPc,

    /// <summary><c>call [rsp+callee+entry]</c>: the direct call itself.</summary>
    CallEntry,

    /// <summary><c>mov arg3d, eax</c>: the direct callee's status, as the finish helper's fourth argument.</summary>
    MovArg3Status,

    /// <summary><c>mov eax, returned</c>: the status an inline return leaves with.</summary>
    MovReturned,
}

/// <summary>Which part of a value-form unit's body a layout entry belongs to, which names the clause it answers to.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B02C0E
// Broiler-Human:        PENDING
public enum JsValueRole : byte
{
    /// <summary>The dispatch, the landing tree or the defect block.</summary>
    Dispatch = 0,

    /// <summary>The two moves before a helper call.</summary>
    Head = 1,

    /// <summary>A helper call through the table.</summary>
    Slot = 2,

    /// <summary>What a helper call's answer is compared with and where it goes.</summary>
    Tail = 3,

    /// <summary>An inline template's own work.</summary>
    Inline = 4,

    /// <summary>A guard: a branch to the helper call of its own instruction (clause V6).</summary>
    Guard = 5,

    /// <summary>The debt: its count, its spill, its test and its settlement (clause V5).</summary>
    Debt = 6,

    /// <summary>A direct call: the prepare helper, the call of the callee's entry and the finish helper (clause V3).</summary>
    Call = 7,
}

/// <summary>One entry of a value-form unit's layout.</summary>
/// <param name="Template">Which template it is.</param>
/// <param name="Operand">
/// What its one field carries, when it has one: a program counter, eight times an opcode, the defect
/// status, a region displacement, a word, a condition code or the debt threshold; zero otherwise.
/// </param>
/// <param name="Target">
/// For a branch, the index in the layout it goes to, or <see cref="JsBaselineInstruction.Leave"/>;
/// <see cref="JsBaselineInstruction.None"/> otherwise.
/// </param>
/// <param name="Role">Which part of the body it belongs to.</param>
/// <param name="Pc">The instruction it belongs to, or -1 in the dispatch.</param>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=745761
// Broiler-Human:        PENDING
public readonly record struct JsValueInstruction(
    JsValueTemplate Template,
    long Operand,
    int Target,
    JsValueRole Role,
    int Pc);

/// <summary>
/// The plan of one value-form code unit: its partition, its operand heights and scope depths, its region,
/// and what each instruction's emitted code does - or nothing, when the unit has no plan.
/// </summary>
/// <remarks>
/// <b>ONLY <see cref="JsValueLayout.TryPlan"/> BUILDS ONE</b>, and every reader reads the same one: the
/// emitter lays it out, the template scan compares a payload with that layout, and the engine reads its
/// heights, its region and its residency, so the three cannot disagree about which word is which.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=DEAE80
// Broiler-Falsified-If: a plan answers a height, a depth, a residency or an inline decision other than the walk and the analysis JsValueLayout.TryPlan documents compute for the same image, unit, handler offsets and residency flag
// Broiler-Human:        PENDING
public sealed class JsValueUnitPlan
{
    private readonly int[] heights;
    private readonly int[] depths;
    private readonly JsValueInline[] inline;
    private readonly long[] operands;
    private readonly int[] residentBase;
    private readonly int[] residentSlots;

    internal JsValueUnitPlan(
        JsBaselineUnitPlan blocks,
        int[] heights,
        int[] depths,
        JsValueInline[] inline,
        long[] operands,
        int argumentWords,
        int residentWords,
        int[] residentBase,
        int[] residentSlots,
        bool wholeStack)
    {
        Blocks = blocks;
        this.heights = heights;
        this.depths = depths;
        this.inline = inline;
        this.operands = operands;
        ArgumentWords = argumentWords;
        ResidentWords = residentWords;
        this.residentBase = residentBase;
        this.residentSlots = residentSlots;
        WholeStack = wholeStack;
    }

    /// <summary>The partition in which every instruction is a block, with its landings.</summary>
    public JsBaselineUnitPlan Blocks { get; }

    /// <summary>Which code unit of the image this is the plan of.</summary>
    public int UnitIndex => Blocks.UnitIndex;

    /// <summary>The offset of the unit's first instruction.</summary>
    public int First => Blocks.First;

    /// <summary>The first offset past the unit.</summary>
    public int End => Blocks.End;

    /// <summary>How many words at the region's start hold the activation's arguments.</summary>
    public int ArgumentWords { get; }

    /// <summary>How many words after the arguments hold resident bindings.</summary>
    public int ResidentWords { get; }

    /// <summary>The region word the operand stack's first slot is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=0; Fingerprint=2875C9
    // Broiler-Falsified-If: it answers a word other than the first one after the arguments and the resident bindings
    // Broiler-Human:        PENDING
    public int StackBase => ArgumentWords + ResidentWords;

    /// <summary>
    /// Whether every helper of the unit reads the whole operand stack, which is what a unit that can
    /// suspend needs: its frame is the activation's own stack, so what it suspends with must be decoded.
    /// </summary>
    public bool WholeStack { get; }

    /// <summary>How many scope depths the plan describes: one past the deepest the unit reaches.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=6CEDCA
    // Broiler-Human:        PENDING
    public int Depths => residentBase.Length;

    /// <summary>The operand-stack height before the instruction at <paramref name="pc"/>, or -1 where no instruction starts or none is reached.</summary>
    /// <param name="pc">An offset of the code section.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=0; Fingerprint=782F5C
    // Broiler-Falsified-If: it answers a height for an offset outside the unit or not an instruction start, or a height other than the verifier's abstract height there
    // Broiler-Human:        PENDING
    public int HeightAt(int pc) =>
        (uint)(pc - First) < (uint)heights.Length ? heights[pc - First] : -1;

    /// <summary>The scope depth before the instruction at <paramref name="pc"/>, or -1 where none is reached.</summary>
    /// <param name="pc">An offset of the code section.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=74F33E
    // Broiler-Falsified-If: it answers a depth other than the verifier's scope depth at a reached instruction
    // Broiler-Human:        PENDING
    public int DepthAt(int pc) =>
        (uint)(pc - First) < (uint)depths.Length ? depths[pc - First] : -1;

    /// <summary>What the emitted code does for the instruction at <paramref name="pc"/> itself.</summary>
    /// <param name="pc">An instruction start of the unit.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=9D43C3
    // Broiler-Falsified-If: it answers an inline kind for an instruction outside the pure set, for one the walk did not reach, or for a scoped instruction whose binding is not resident
    // Broiler-Human:        PENDING
    public JsValueInline InlineAt(int pc) =>
        (uint)(pc - First) < (uint)inline.Length ? inline[pc - First] : JsValueInline.None;

    /// <summary>The region word of the first slot of the resident environment at <paramref name="depth"/>, or -1 when it is not resident.</summary>
    /// <param name="depth">A scope depth of the unit.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=0; Fingerprint=941DCD
    // Broiler-Falsified-If: two resident environments share a word, a resident word overlaps the arguments or the operand stack, or a depth a closure, an eval, a with, a name search or a mapped arguments object can reach answers a word
    // Broiler-Human:        PENDING
    public int ResidentBaseOf(int depth) =>
        (uint)depth < (uint)residentBase.Length ? residentBase[depth] : -1;

    /// <summary>How many region words the resident environment at <paramref name="depth"/> takes, or zero.</summary>
    /// <param name="depth">A scope depth of the unit.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=3D2019
    // Broiler-Falsified-If: it answers fewer words than an instance of the environment at that depth declares
    // Broiler-Human:        PENDING
    public int ResidentSlotsOf(int depth) =>
        ResidentBaseOf(depth) < 0 ? 0 : residentSlots[depth];

    /// <summary>The operand the instruction's inline template carries: a word, an argument, a region word or a pick depth.</summary>
    /// <param name="pc">An instruction start of the unit.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=594A0E
    // Broiler-Falsified-If: it answers an operand other than the word, argument, region word or pick depth the instruction's inline kind reads
    // Broiler-Human:        PENDING
    internal long OperandAt(int pc) => operands[pc - First];
}

/// <summary>
/// The value form's plan and layout, stated once: which words a unit's region holds, which bindings are
/// resident, which instructions the emitted code runs itself, and the templates between a unit's prologue
/// and its epilogue (JSD-0035 sections 3, 5, 7 and 9, stage JSV-2).
/// </summary>
/// <remarks>
/// <para>
/// <b>IT IS IN THE FORMAT ASSEMBLY FOR THE REASON <see cref="JsBaselineBlocks"/> IS</b>: the emitter in the
/// lowering, the template scan here and the engine in the profile all read it, and none of them may
/// reference another. It is a pure function of the image, the residency flag the artifact carries and the
/// unit's handler offsets.
/// </para>
/// <para>
/// <b>THE WALK IS THE VERIFIER'S.</b> Operand heights and scope depths come from the abstract pass the
/// verifier makes - the entry at height and depth zero, each handler at its region's height plus one and
/// its region's depth, the stepping opcodes one or two below on their taken branch - and a join whose two
/// states differ refuses the plan rather than approximating it, which a verified program never reaches.
/// </para>
/// <para>
/// <b>A BINDING IS RESIDENT ONLY WHERE NOTHING OUTSIDE ITS ACTIVATION CAN REACH IT.</b> The whole unit is
/// refused residency when it is a program body, eval code, a generator or an async body (whose frames
/// suspend, which is stage JSV-4's), or when it holds a direct eval, a <c>with</c>, a name search or an
/// eval name instruction. Otherwise an environment depth is resident when it is deeper than every depth
/// at which the unit creates a closure, which captures the whole chain below it, and deeper than the
/// function's own environment when the unit reads <c>arguments</c>, which a sloppy function maps onto it.
/// A resident environment takes the most slots any of its instances declares, and a unit whose resident
/// words would pass <see cref="ResidentCeiling"/> keeps them all in the managed chain.
/// </para>
/// <para>
/// <b>THE INLINE SET IS THE PURE SET, AND EVERY GUARD GOES TO ITS OWN INSTRUCTION'S HELPER.</b> A guarded
/// template counts its instruction into the debt register first, tests, and does its work only once every
/// test has passed; a failed test branches to a stub that takes the count back and calls the instruction's
/// own helper, so the instruction is charged once either way and the slab is untouched when the helper
/// reads it (clause V6). String constants, <c>TypeOf</c> and the truthiness of a handle are helpers at this
/// stage, because their answer is a handle only the instance knows or a property only a helper can read.
/// </para>
/// <para>
/// <b>EVERY CYCLE OF THE EMITTED CODE PASSES A SETTLEMENT (clause V5).</b> Every helper call stores the debt
/// and clears it before it calls, and the helper charges it; a backward inline branch tests the debt
/// against <see cref="DebtThreshold"/> and settles it through the settlement helper when it has reached
/// it; and a linear run of <see cref="StraightLineRun"/> inline instructions ends in the same test. A
/// backward branch that follows a helper's answer needs no test, because the helper left the debt at zero.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=758365
// Broiler-Falsified-If: an inline template's effect differs from its arm's for an operand its guards admit, a guard branches anywhere but its own instruction's helper stub, a backward inline branch carries no debt test, a binding is resident that a closure, an eval, a with, a name search or a mapped arguments object can reach, or TryPlan or Layout answers differently for the same image, unit, handler offsets and residency flag
// Broiler-Human:        PENDING
public static class JsValueLayout
{
    /// <summary>The debt at or past which a debt test settles.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=A922D6
    // Broiler-Falsified-If: an emitted debt test compares against any other value
    // Broiler-Human:        PENDING
    public const int DebtThreshold = 1024;

    /// <summary>How many inline instructions a linear run holds before it ends in a debt test.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=9E1B88
    // Broiler-Human:        PENDING
    public const int StraightLineRun = 256;

    /// <summary>The most argument words a region holds; a <c>LoadArgument</c> past them is a helper.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=88864F
    // Broiler-Human:        PENDING
    public const int ArgumentCeiling = 256;

    /// <summary>The most resident words a region holds; a unit that would need more has none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=BD1083
    // Broiler-Human:        PENDING
    public const int ResidentCeiling = 65536;

    /// <summary>The most words any region an inline template addresses can hold.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=289852
    // Broiler-Falsified-If: a region an emitted template addresses holds more words than this
    // Broiler-Human:        PENDING
    public const long CeilingRegionWords =
        (long)ArgumentCeiling + ResidentCeiling + JsFormat.CeilingOperandStack + 1;

    /// <summary>The name of the value-table row each <see cref="JsValueTemplate"/> member is, by the member's value.</summary>
    /// <remarks><b>Private, and read through <see cref="TemplateName"/></b>, so no reader can edit an entry.</remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=FF4199
    // Broiler-Falsified-If: an entry names a row other than the one the JsValueTemplate member of its index documents
    // Broiler-Human:        PENDING
    private static readonly string[] Names =
    [
        "test eax, eax", "js rel32", "cmp eax, pc", "je rel32", "jne rel32", "ja rel32", "jmp rel32",
        "mov eax, status", "mov arg0, r14", "mov arg1d, pc", "call [rbx+slot]", "call [rbx+settle]",
        "mov [r14+debt], r12", "xor r12d, r12d", "inc r12", "dec r12", "cmp r12, threshold", "jae rel32",
        "jp rel32", "mov rax, [r15+slot]", "mov rdx, [r15+slot]", "mov [r15+slot], rax",
        "mov [r15+slot], rdx", "mov rax, word", "mov rcx, word", "cmp rax, rcx", "cmp rdx, rcx",
        "movq xmm0, rax", "movq xmm1, rdx", "movq xmm1, rcx", "movq rax, xmm0", "addsd xmm0, xmm1",
        "subsd xmm0, xmm1", "mulsd xmm0, xmm1", "divsd xmm0, xmm1", "ucomisd xmm0, xmm1",
        "ucomisd xmm1, xmm0", "xorpd xmm1, xmm1", "setcc al", "setcc dl", "and al, dl", "or al, dl",
        "movzx eax, al", "add rax, rcx", "xor rax, rcx", "xor rax, 1", "cvttsd2si eax, xmm0",
        "cvttsd2si ecx, xmm1", "cmp eax, int32min", "cmp ecx, int32min", "or eax, ecx", "and eax, ecx",
        "xor eax, ecx", "shl eax, cl", "sar eax, cl", "shr eax, cl", "not eax", "movsxd rax, eax",
        "cvtsi2sd xmm0, rax", "lea arg2, [rsp+callee]", "call [rbx+prepare]", "call [rbx+finish]",
        "cmp eax, direct", "lea arg0, [rsp+callee]", "mov arg1d, [rsp+callee+entrypc]", "call [rsp+callee+entry]",
        "mov arg3d, eax", "mov eax, returned",
    ];

    /// <summary>How many templates a value layout names.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=4AD000
    // Broiler-Human:        PENDING
    public static int TemplateCount => Names.Length;

    /// <summary>The name of the value-table row <paramref name="template"/> is.</summary>
    /// <param name="template">A value template.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=E26DA3
    // Broiler-Falsified-If: it answers a name other than the one of the row the member documents
    // Broiler-Human:        PENDING
    public static string TemplateName(JsValueTemplate template) => Names[(int)template];

    /// <summary>The bits of the Number one, which an increment adds and a decrement subtracts.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A755C6
    // Broiler-Human:        PENDING
    private const long OneBits = 0x3FF0_0000_0000_0000L;

    /// <summary>The sign bit, which a negation flips.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1F7B56
    // Broiler-Human:        PENDING
    private const long SignBit = long.MinValue;

    /// <summary>
    /// The plan of one code unit in the value form, or a refusal that says why the unit has none.
    /// </summary>
    /// <remarks>
    /// <b>THE PARTITION IS <see cref="JsBaselineBlocks.TryPlanEachInstruction"/>'S</b>, so its refusals come
    /// first and in its order; then the walk, whose refusal names the offset whose two states differ.
    /// </remarks>
    /// <param name="image">The program the unit belongs to.</param>
    /// <param name="unitIndex">Which code unit of <paramref name="image"/> to plan.</param>
    /// <param name="handlerOffsets">The handler offsets of the unit's exception regions.</param>
    /// <param name="residentBindings">Whether any binding may be resident, as the artifact's form byte says.</param>
    /// <param name="plan">The plan, when the answer is <see langword="true"/>.</param>
    /// <param name="refusal">Why there is no plan, when the answer is <see langword="false"/>.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=836AAF
    // Broiler-Falsified-If: a plan it answers has a height or depth other than the verifier's at some instruction, a resident depth the remark refuses, or an inline decision for an instruction whose height is not known
    // Broiler-Human:        PENDING
    public static bool TryPlan(
        JsNativeProgramImage image,
        int unitIndex,
        System.ReadOnlySpan<uint> handlerOffsets,
        bool residentBindings,
        out JsValueUnitPlan plan,
        out string refusal)
    {
        plan = null!;

        if (!JsBaselineBlocks.TryPlanEachInstruction(image, unitIndex, handlerOffsets, out var blocks, out refusal))
        {
            return false;
        }

        var row = image.Functions[unitIndex];
        var code = image.Code;
        var first = blocks.First;
        var end = blocks.End;
        var span = end - first;
        var heights = new int[span];
        var depths = new int[span];
        System.Array.Fill(heights, -1);
        System.Array.Fill(depths, -1);

        if (!Walk(image, unitIndex, first, end, heights, depths, out refusal))
        {
            return false;
        }

        var flags = (JsFormat.FunctionFlags)row.Flags;
        var suspends = (flags & (JsFormat.FunctionFlags.Generator | JsFormat.FunctionFlags.Async)) != 0;

        // ---- residency ---------------------------------------------------------------------------
        var eligible = residentBindings &&
            (flags & (JsFormat.FunctionFlags.ProgramBody | JsFormat.FunctionFlags.EvalCode |
                JsFormat.FunctionFlags.Generator | JsFormat.FunctionFlags.Async)) == 0;
        var poisoned = (flags & JsFormat.FunctionFlags.UsesArguments) != 0 ? 0 : -1;
        var deepest = 0;
        var slots = new int[JsFormat.CeilingScopeDepth + 2];
        slots[0] = (int)row.ScopeSlots;
        var arguments = 0;

        for (var at = first; at < end; at += JsOpcodes.InstructionWidth((JsOpcode)code[at]))
        {
            var opcode = (JsOpcode)code[at];
            var depth = depths[at - first];

            switch (opcode)
            {
                case JsOpcode.CallEval or JsOpcode.CallEvalSpread or JsOpcode.PushObjectScope or
                    JsOpcode.ResolveName or JsOpcode.LoadEvalName or JsOpcode.LoadEvalNameOrUndefined or
                    JsOpcode.StoreEvalName or JsOpcode.LoadEvalNameWithBase or JsOpcode.DeleteEvalName or
                    JsOpcode.StoreEvalVariable or JsOpcode.WithBaseObject:
                    eligible = false;
                    break;

                case JsOpcode.NewArguments:
                    poisoned = System.Math.Max(poisoned, 0);
                    break;

                case JsOpcode.Closure when depth >= 0:
                    poisoned = System.Math.Max(poisoned, depth);
                    break;

                case JsOpcode.PushScope when depth >= 0 && depth + 1 < slots.Length:
                    slots[depth + 1] = System.Math.Max(slots[depth + 1], (int)Operand(code, at, opcode));
                    deepest = System.Math.Max(deepest, depth + 1);
                    break;

                case JsOpcode.CopyScope when depth >= 0 && depth < slots.Length:
                    slots[depth] = System.Math.Max(slots[depth], (int)Operand(code, at, opcode));
                    break;

                case JsOpcode.LoadArgument when heights[at - first] >= 0:
                    var argument = (int)Operand(code, at, opcode);

                    if (argument < ArgumentCeiling)
                    {
                        arguments = System.Math.Max(arguments, argument + 1);
                    }

                    break;
            }

            deepest = System.Math.Max(deepest, depth);
        }

        var residentBase = new int[deepest + 1];
        var residentSlots = new int[deepest + 1];
        var resident = 0L;

        for (var depth = 0; depth <= deepest; depth++)
        {
            residentBase[depth] = -1;
            residentSlots[depth] = slots[depth];

            if (eligible && depth > poisoned && slots[depth] > 0)
            {
                residentBase[depth] = arguments + (int)resident;
                resident += slots[depth];
            }
        }

        if (resident > ResidentCeiling)
        {
            System.Array.Fill(residentBase, -1);
            resident = 0;
        }

        // ---- the inline decision -----------------------------------------------------------------
        var inline = new JsValueInline[span];
        var operands = new long[span];

        for (var at = first; at < end; at += JsOpcodes.InstructionWidth((JsOpcode)code[at]))
        {
            var local = at - first;

            if (heights[local] < 0)
            {
                continue;
            }

            var opcode = (JsOpcode)code[at];
            var operand = Operand(code, at, opcode);
            var (kind, value) = Decide(image, opcode, operand, depths[local], residentBase, slots, arguments);
            inline[local] = kind;
            operands[local] = value;
        }

        plan = new JsValueUnitPlan(
            blocks, heights, depths, inline, operands, arguments, (int)resident, residentBase, residentSlots, suspends);

        refusal = string.Empty;
        return true;
    }

    /// <summary>The layout of a plan: every entry between the unit's prologue and its epilogue, in order, with its branches resolved.</summary>
    /// <remarks>
    /// <para>
    /// <b>THE DISPATCH, THEN EVERY INSTRUCTION IN BYTECODE ORDER, THEN THE STUBS.</b> The dispatch is the
    /// baseline form's: a negative answer leaves, a landing goes to its instruction through the compare
    /// tree, and anything else is the defect. An instruction the emitted code runs itself is its inline
    /// template and falls into the next instruction; any other is a spill of the debt, a helper call and
    /// the helper's tail, which compares the answer with the instruction's target and its successor. The
    /// stubs follow every instruction: a guarded template's helper call, a backward branch's debt test and
    /// a settlement.
    /// </para>
    /// <para>
    /// <b>A <c>Call</c> THE WALK REACHED IS A DIRECT CALL SITE (stage JSV-3).</b> After the spill it calls the
    /// prepare helper with the callee context as a third argument; an answer other than
    /// <see cref="JsValueAbi.DirectCall"/> is the instruction's own helper answer and goes to its tail. The
    /// direct call enters the callee's emitted entry with the context and the offset the helper wrote, then
    /// calls the finish helper with the callee's status as a fourth argument, whose answer goes to the same
    /// tail.
    /// </para>
    /// <para>
    /// <b>A TARGET IS AN INDEX INTO THE ANSWER</b>, or <see cref="JsBaselineInstruction.Leave"/> for the
    /// epilogue's first instruction.
    /// </para>
    /// </remarks>
    /// <param name="plan">A plan <see cref="TryPlan"/> answered.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=3; Fingerprint=8394EE
    // Broiler-Falsified-If: an entry of the answer differs from the template, operand and target the remark and the inline set document for its instruction, or a branch resolves to an index other than the entry it names
    // Broiler-Human:        PENDING
    public static JsValueInstruction[] Layout(JsValueUnitPlan plan)
    {
        var builder = new Builder(plan);
        builder.Build();
        return builder.Finish();
    }

    // ---- the walk ----------------------------------------------------------------------------------

    /// <summary>The verifier's abstract pass over one unit: the operand height and scope depth at every instruction.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=7D275C
    // Broiler-Falsified-If: it answers a height or depth at an instruction other than the verifier's, or accepts a join of two different states
    // Broiler-Human:        PENDING
    private static bool Walk(
        JsNativeProgramImage image, int unitIndex, int first, int end, int[] heights, int[] depths, out string refusal)
    {
        var code = image.Code;
        var pending = new System.Collections.Generic.Stack<int>();

        if (!Seed(first, 0, 0, first, heights, depths, pending, out refusal))
        {
            return false;
        }

        foreach (var region in image.Regions)
        {
            if (region.FunctionIndex == (uint)unitIndex &&
                !Seed((int)region.HandlerOffset, (int)region.StackHeight + 1, (int)region.ScopeDepth, first, heights, depths, pending, out refusal))
            {
                return false;
            }
        }

        while (pending.Count != 0)
        {
            var at = pending.Pop();

            while (true)
            {
                var opcode = (JsOpcode)code[at];
                var width = JsOpcodes.InstructionWidth(opcode);
                var operand = Operand(code, at, opcode);
                var height = heights[at - first];
                var depth = depths[at - first];

                if (!JsOpcodes.TryDescribe(opcode, operand, out var pops, out var pushes) || height < pops)
                {
                    refusal = "the `" + opcode + "` at " + at + " pops more than its height holds";
                    return false;
                }

                var after = height - pops + pushes;
                var afterDepth = opcode switch
                {
                    JsOpcode.PushScope or JsOpcode.PushObjectScope => depth + 1,
                    JsOpcode.PopScope => depth - 1,
                    _ => depth,
                };

                if (JsOpcodes.HasCodeTarget(opcode))
                {
                    var targetHeight = opcode switch
                    {
                        JsOpcode.ForInNext or JsOpcode.IterateNext or
                            JsOpcode.IterateCloseAsync or JsOpcode.DisposeStep => height - 1,
                        JsOpcode.IterateAwaitStep => height - 2,
                        _ => after,
                    };

                    if (!Seed((int)operand, targetHeight, afterDepth, first, heights, depths, pending, out refusal))
                    {
                        return false;
                    }
                }

                if (JsOpcodes.IsTerminal(opcode))
                {
                    break;
                }

                var next = at + width;

                if (next >= end)
                {
                    refusal = "the `" + opcode + "` at " + at + " falls off the end of its unit";
                    return false;
                }

                if (heights[next - first] < 0)
                {
                    heights[next - first] = after;
                    depths[next - first] = afterDepth;
                    at = next;
                    continue;
                }

                if (heights[next - first] != after || depths[next - first] != afterDepth)
                {
                    refusal = "two paths reach " + next + " with different operand heights or scope depths";
                    return false;
                }

                break;
            }
        }

        refusal = string.Empty;
        return true;
    }

    /// <summary>Records the state at an offset the walk has not reached, or checks it against the one it has.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=732B2F
    // Broiler-Falsified-If: it records a state at an offset outside the unit, or accepts a second state that differs from the first
    // Broiler-Human:        PENDING
    private static bool Seed(
        int at,
        int height,
        int depth,
        int first,
        int[] heights,
        int[] depths,
        System.Collections.Generic.Stack<int> pending,
        out string refusal)
    {
        var local = at - first;

        if ((uint)local >= (uint)heights.Length || height < 0 || depth < 0)
        {
            refusal = "the walk reaches " + at + " outside its unit or below an empty stack";
            return false;
        }

        if (heights[local] < 0)
        {
            heights[local] = height;
            depths[local] = depth;
            pending.Push(at);
            refusal = string.Empty;
            return true;
        }

        refusal = heights[local] == height && depths[local] == depth
            ? string.Empty
            : "two paths reach " + at + " with different operand heights or scope depths";

        return refusal.Length == 0;
    }

    // ---- the inline set ----------------------------------------------------------------------------

    /// <summary>What the emitted code does for one reached instruction, and the operand its template carries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=DF103D
    // Broiler-Falsified-If: an instruction outside the pure set is given an inline kind, or a scoped instruction is inline for a binding that is not resident
    // Broiler-Human:        PENDING
    private static (JsValueInline Kind, long Operand) Decide(
        JsNativeProgramImage image,
        JsOpcode opcode,
        uint operand,
        int depth,
        int[] residentBase,
        int[] slots,
        int arguments)
    {
        switch (opcode)
        {
            case JsOpcode.Nop:
                return (JsValueInline.Nop, 0);

            case JsOpcode.LoadUndefined:
                return (JsValueInline.LoadWord, unchecked((long)JsWord.Undefined));

            case JsOpcode.LoadNull:
                return (JsValueInline.LoadWord, unchecked((long)JsWord.Null));

            case JsOpcode.LoadTrue:
                return (JsValueInline.LoadWord, unchecked((long)JsWord.True));

            case JsOpcode.LoadFalse:
                return (JsValueInline.LoadWord, unchecked((long)JsWord.False));

            case JsOpcode.LoadConstant
                when operand < (uint)image.ConstantIsNumber.Length && image.ConstantIsNumber[operand]:
                return (JsValueInline.LoadWord, unchecked((long)JsWord.FromNumber(image.ConstantValues[operand])));

            case JsOpcode.LoadArgument when operand < (uint)arguments:
                return (JsValueInline.LoadArgument, operand);

            case JsOpcode.LoadScoped or JsOpcode.StoreScoped or JsOpcode.InitialiseScoped:
            {
                var hops = (int)(operand >> 16);
                var slot = (int)(operand & 0xFFFF);
                var target = depth - hops;

                if (target < 0 || target >= residentBase.Length || residentBase[target] < 0 ||
                    slot >= slots[target])
                {
                    return (JsValueInline.None, 0);
                }

                var word = residentBase[target] + slot;

                return (opcode switch
                {
                    JsOpcode.LoadScoped => JsValueInline.LoadResident,
                    JsOpcode.StoreScoped => JsValueInline.StoreResident,
                    _ => JsValueInline.InitialiseResident,
                }, word);
            }

            // A PER-ITERATION COPY OF A RESIDENT ENVIRONMENT IS UNOBSERVABLE: the copy exists for a closure
            // to capture the turn's values, a resident environment has no closure, and its words carry
            // the values into the next turn exactly as the copy's slots would. `Copy` charges nothing of
            // its own, so the instruction's one unit is the whole charge.
            case JsOpcode.CopyScope when depth >= 0 && depth < residentBase.Length && residentBase[depth] >= 0:
                return (JsValueInline.Nop, 0);

            case JsOpcode.Pop:
                return (JsValueInline.Pop, 0);

            case JsOpcode.Duplicate:
                return (JsValueInline.Duplicate, 0);

            case JsOpcode.DuplicateTwo:
                return (JsValueInline.DuplicateTwo, 0);

            case JsOpcode.Swap:
                return (JsValueInline.Swap, 0);

            case JsOpcode.Pick:
                return (JsValueInline.Pick, operand);

            case JsOpcode.Void:
                return (JsValueInline.Void, 0);

            case JsOpcode.Add or JsOpcode.Subtract or JsOpcode.Multiply or JsOpcode.Divide:
                return (JsValueInline.Arithmetic, 0);

            case JsOpcode.Negate:
                return (JsValueInline.Negate, 0);

            case JsOpcode.Increment or JsOpcode.Decrement:
                return (JsValueInline.Step, 0);

            case JsOpcode.ToNumber or JsOpcode.ToNumeric:
                return (JsValueInline.ToNumber, 0);

            case JsOpcode.LessThan or JsOpcode.LessThanOrEqual or JsOpcode.GreaterThan or
                JsOpcode.GreaterThanOrEqual or JsOpcode.StrictEquals or JsOpcode.StrictNotEquals or
                JsOpcode.LooseEquals or JsOpcode.LooseNotEquals:
                return (JsValueInline.Compare, 0);

            case JsOpcode.BitwiseOr or JsOpcode.BitwiseAnd or JsOpcode.BitwiseXor or
                JsOpcode.ShiftLeft or JsOpcode.ShiftRight or JsOpcode.ShiftRightUnsigned:
                return (JsValueInline.Bitwise, 0);

            case JsOpcode.BitwiseNot:
                return (JsValueInline.BitwiseNot, 0);

            case JsOpcode.Not:
                return (JsValueInline.Not, 0);

            case JsOpcode.Jump:
                return (JsValueInline.Jump, 0);

            case JsOpcode.JumpIfFalse or JsOpcode.JumpIfTrue:
                return (JsValueInline.Branch, 0);

            // A RETURN ENDS THE ACTIVATION AND TOUCHES NOTHING ELSE: the interpreter's arm records the value
            // and leaves, so the emitted code leaves the value in the region's first word for whoever reads
            // the answer (JSD-0035 section 6, stage JSV-3).
            case JsOpcode.Return:
                return (JsValueInline.Return, 0);

            case JsOpcode.ReturnUndefined:
                return (JsValueInline.Return, unchecked((long)JsWord.Undefined));

            default:
                return (JsValueInline.None, 0);
        }
    }

    /// <summary>
    /// An instruction's operand, as the verifier decodes it: a <c>U8U16</c> operand is its depth above the
    /// sixteen bits of its index.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F7D4AE
    // Broiler-Human:        PENDING
    internal static uint Operand(byte[] code, int pc, JsOpcode opcode) =>
        JsOpcodes.Shape(opcode) switch
        {
            JsOperandShape.U8 => code[pc + 1],
            JsOperandShape.U16 => (uint)(code[pc + 1] | (code[pc + 2] << 8)),
            JsOperandShape.U32 => (uint)(code[pc + 1] | (code[pc + 2] << 8) | (code[pc + 3] << 16) | (code[pc + 4] << 24)),
            JsOperandShape.U8U16 => (uint)((code[pc + 1] << 16) | code[pc + 2] | (code[pc + 3] << 8)),
            _ => 0,
        };

    // ---- the layout ---------------------------------------------------------------------------------

    /// <summary>Makes one plan's layout: entries with symbolic labels, resolved once every label is bound.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=B5C9F0
    // Broiler-Falsified-If: the layout it answers is not the one the remark on JsValueLayout.Layout documents for the plan it was handed
    // Broiler-Human:        PENDING
    private sealed class Builder(JsValueUnitPlan plan)
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1E4402
        // Broiler-Human:        PENDING
        private const int Dispatch = -3;

        private readonly System.Collections.Generic.List<JsValueInstruction> entries = [];
        private readonly System.Collections.Generic.List<int> labels = [];
        private readonly System.Collections.Generic.List<System.Action> stubs = [];
        private readonly System.Collections.Generic.Dictionary<int, int> settlements = [];
        private int[] instructionLabels = [];
        private int defect;

        /// <summary>Builds the dispatch, the instructions in order, and then every stub they asked for.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=D62CB8
        // Broiler-Falsified-If: the entries it adds are not the dispatch, then one sequence per instruction in bytecode order, then the stubs those sequences asked for
        // Broiler-Human:        PENDING
        public void Build()
        {
            var blocks = plan.Blocks.Blocks;
            instructionLabels = new int[blocks.Length];

            for (var index = 0; index < blocks.Length; index++)
            {
                instructionLabels[index] = NewLabel();
            }

            defect = NewLabel();

            // ---- the dispatch, as the baseline form lays it --------------------------------------------
            Add(JsValueTemplate.TestEax, 0, JsBaselineInstruction.None, JsValueRole.Dispatch, -1);
            Add(JsValueTemplate.JsLeave, 0, JsBaselineInstruction.Leave, JsValueRole.Dispatch, -1);
            Tree(plan.Blocks.Landings, 0, plan.Blocks.Landings.Length - 1);
            Bind(defect);
            Add(JsValueTemplate.MovStatus, (long)JsBaselineStatus.Defect, JsBaselineInstruction.None, JsValueRole.Dispatch, -1);
            Add(JsValueTemplate.Jmp, 0, JsBaselineInstruction.Leave, JsValueRole.Dispatch, -1);

            // ---- the instructions -------------------------------------------------------------------
            var run = 0;

            for (var index = 0; index < blocks.Length; index++)
            {
                var block = blocks[index];
                Bind(instructionLabels[index]);

                if (plan.InlineAt(block.Head) == JsValueInline.None)
                {
                    if (block.HeadOpcode == JsOpcode.Call && plan.HeightAt(block.Head) >= 0)
                    {
                        DirectCall(block);
                    }
                    else
                    {
                        Helper(block, stub: false, taken: false);
                    }

                    run = 0;
                    continue;
                }

                Inline(block);

                if (plan.InlineAt(block.Head) is JsValueInline.Jump or JsValueInline.Branch or JsValueInline.Return)
                {
                    continue;
                }

                if (++run >= StraightLineRun)
                {
                    DebtTest(block.Following);
                    run = 0;
                }
            }

            // ---- the stubs, in the order they were asked for ------------------------------------------
            for (var index = 0; index < stubs.Count; index++)
            {
                stubs[index]();
            }
        }

        /// <summary>The entries with every label resolved to an index.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=5FA585
        // Broiler-Falsified-If: an entry is answered with a target other than the index its label was bound at, or an unbound label resolves to anything but a refusal
        // Broiler-Human:        PENDING
        public JsValueInstruction[] Finish()
        {
            var resolved = new JsValueInstruction[entries.Count];

            for (var index = 0; index < resolved.Length; index++)
            {
                var entry = entries[index];

                if (entry.Target >= 0 && labels[entry.Target] < 0)
                {
                    throw new System.InvalidOperationException("the value layout branches to a label it never bound");
                }

                resolved[index] = entry.Target >= 0
                    ? entry with { Target = labels[entry.Target] }
                    : entry.Target == Dispatch ? entry with { Target = 0 } : entry;
            }

            return resolved;
        }

        // ---- the helper call and its tail ------------------------------------------------------------

        /// <summary>
        /// A helper call for the block's instruction and its tail: in line, where the next instruction
        /// follows it, or as a guarded template's stub, which takes the instruction's count back first.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=C924A8
        // Broiler-Falsified-If: a helper call passes a program counter other than its instruction's or a slot other than eight times that opcode, is not preceded by a spill and a clear of the debt, or a stub's call does not first take its instruction's count back
        // Broiler-Human:        PENDING
        private void Helper(JsBaselineBlock block, bool stub, bool taken)
        {
            var pc = block.Head;

            if (taken)
            {
                Add(JsValueTemplate.DecDebt, 0, JsBaselineInstruction.None, JsValueRole.Debt, pc);
            }

            Add(JsValueTemplate.SpillDebt, 0, JsBaselineInstruction.None, JsValueRole.Debt, pc);
            Add(JsValueTemplate.ClearDebt, 0, JsBaselineInstruction.None, JsValueRole.Debt, pc);
            Add(JsValueTemplate.MovArg0R14, 0, JsBaselineInstruction.None, JsValueRole.Head, pc);
            Add(JsValueTemplate.MovArg1Pc, pc, JsBaselineInstruction.None, JsValueRole.Head, pc);
            Add(JsValueTemplate.CallSlot, (long)block.HeadOpcode * 8, JsBaselineInstruction.None, JsValueRole.Slot, pc);
            Tail(block, stub);
        }

        /// <summary>
        /// A direct call site for the block's <c>Call</c>: the prepare helper, and when it asks for one the call
        /// of the callee's entry and the finish helper, then the instruction's tail (JSD-0035 section 6).
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=AFB86F
        // Broiler-Falsified-If: a direct call site passes a program counter other than its instruction's, calls anything but the prepare slot first, calls the callee's entry without the prepare helper having answered the direct call, is not followed by the finish helper with the callee's status, or is not preceded by a spill and a clear of the debt
        // Broiler-Human:        PENDING
        private void DirectCall(JsBaselineBlock block)
        {
            var pc = block.Head;
            var tail = NewLabel();

            Add(JsValueTemplate.SpillDebt, 0, JsBaselineInstruction.None, JsValueRole.Debt, pc);
            Add(JsValueTemplate.ClearDebt, 0, JsBaselineInstruction.None, JsValueRole.Debt, pc);
            Add(JsValueTemplate.MovArg0R14, 0, JsBaselineInstruction.None, JsValueRole.Head, pc);
            Add(JsValueTemplate.MovArg1Pc, pc, JsBaselineInstruction.None, JsValueRole.Head, pc);
            Add(JsValueTemplate.LeaArg2Callee, 0, JsBaselineInstruction.None, JsValueRole.Call, pc);
            Add(JsValueTemplate.CallPrepare, 0, JsBaselineInstruction.None, JsValueRole.Call, pc);
            Add(JsValueTemplate.CmpEaxDirect, 0, JsBaselineInstruction.None, JsValueRole.Call, pc);
            Add(JsValueTemplate.Jne, 0, tail, JsValueRole.Call, pc);
            Add(JsValueTemplate.LeaArg0Callee, 0, JsBaselineInstruction.None, JsValueRole.Call, pc);
            Add(JsValueTemplate.MovArg1EntryPc, 0, JsBaselineInstruction.None, JsValueRole.Call, pc);
            Add(JsValueTemplate.CallEntry, 0, JsBaselineInstruction.None, JsValueRole.Call, pc);
            Add(JsValueTemplate.MovArg3Status, 0, JsBaselineInstruction.None, JsValueRole.Call, pc);
            Add(JsValueTemplate.MovArg0R14, 0, JsBaselineInstruction.None, JsValueRole.Call, pc);
            Add(JsValueTemplate.MovArg1Pc, pc, JsBaselineInstruction.None, JsValueRole.Call, pc);
            Add(JsValueTemplate.LeaArg2Callee, 0, JsBaselineInstruction.None, JsValueRole.Call, pc);
            Add(JsValueTemplate.CallFinish, 0, JsBaselineInstruction.None, JsValueRole.Call, pc);
            Bind(tail);
            Tail(block, stub: false);
        }

        /// <summary>What a helper's answer for the block's instruction is compared with, and where it goes.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=B410A8
        // Broiler-Falsified-If: an answer goes anywhere but the instruction's target, its successor or the dispatch
        // Broiler-Human:        PENDING
        private void Tail(JsBaselineBlock block, bool stub)
        {
            var pc = block.Head;

            if (block.HasTarget)
            {
                Add(JsValueTemplate.CmpEaxPc, block.Target, JsBaselineInstruction.None, JsValueRole.Tail, pc);
                Add(JsValueTemplate.Je, 0, LabelOf(block.Target), JsValueRole.Tail, pc);
            }

            if (block.Tail == JsBaselineTail.Leave)
            {
                Add(JsValueTemplate.Jmp, 0, Dispatch, JsValueRole.Tail, pc);
                return;
            }

            Add(JsValueTemplate.CmpEaxPc, block.Following, JsBaselineInstruction.None, JsValueRole.Tail, pc);

            // IN LINE, THE NEXT INSTRUCTION'S CODE FOLLOWS THIS ONE, so its successor falls through; a stub
            // is out of line and branches to it.
            if (stub || block.Tail != JsBaselineTail.FallThrough)
            {
                Add(JsValueTemplate.Je, 0, LabelOf(block.Following), JsValueRole.Tail, pc);
                Add(JsValueTemplate.Jmp, 0, Dispatch, JsValueRole.Tail, pc);
                return;
            }

            Add(JsValueTemplate.Jne, 0, Dispatch, JsValueRole.Tail, pc);
        }

        /// <summary>The stub a guard of the block's instruction branches to, asked for once.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=514138
        // Broiler-Falsified-If: a guard's stub calls any instruction's helper but its own, or one instruction is given two stubs
        // Broiler-Human:        PENDING
        private int Slow(JsBaselineBlock block, ref int slow)
        {
            if (slow < 0)
            {
                var label = NewLabel();
                slow = label;

                stubs.Add(() =>
                {
                    Bind(label);
                    Helper(block, stub: true, taken: true);
                });
            }

            return slow;
        }

        // ---- the debt ----------------------------------------------------------------------------

        /// <summary>A debt test that settles and resumes at <paramref name="resume"/> when the debt has reached the threshold.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=0; Fingerprint=EA8806
        // Broiler-Falsified-If: a debt test compares against any value but the threshold, or settles to resume anywhere but the offset it names
        // Broiler-Human:        PENDING
        private void DebtTest(int resume)
        {
            Add(JsValueTemplate.CmpDebt, DebtThreshold, JsBaselineInstruction.None, JsValueRole.Debt, resume);
            Add(JsValueTemplate.Jae, 0, Settlement(resume), JsValueRole.Debt, resume);
        }

        /// <summary>The settlement stub that resumes at <paramref name="resume"/>, one per resuming offset.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=45CA33
        // Broiler-Falsified-If: a settlement stub resumes anywhere but its offset, or calls anything but the settlement slot
        // Broiler-Human:        PENDING
        private int Settlement(int resume)
        {
            if (settlements.TryGetValue(resume, out var existing))
            {
                return existing;
            }

            var label = NewLabel();
            settlements.Add(resume, label);

            stubs.Add(() =>
            {
                Bind(label);
                Add(JsValueTemplate.SpillDebt, 0, JsBaselineInstruction.None, JsValueRole.Debt, resume);
                Add(JsValueTemplate.ClearDebt, 0, JsBaselineInstruction.None, JsValueRole.Debt, resume);
                Add(JsValueTemplate.MovArg0R14, 0, JsBaselineInstruction.None, JsValueRole.Debt, resume);
                Add(JsValueTemplate.MovArg1Pc, resume, JsBaselineInstruction.None, JsValueRole.Debt, resume);
                Add(JsValueTemplate.CallSettle, 0, JsBaselineInstruction.None, JsValueRole.Debt, resume);
                Add(JsValueTemplate.CmpEaxPc, resume, JsBaselineInstruction.None, JsValueRole.Debt, resume);
                Add(JsValueTemplate.Je, 0, LabelOf(resume), JsValueRole.Debt, resume);
                Add(JsValueTemplate.Jmp, 0, Dispatch, JsValueRole.Debt, resume);
            });

            return label;
        }

        /// <summary>
        /// Where an inline branch to <paramref name="target"/> from <paramref name="pc"/> goes: the target
        /// itself forward, and a debt test in front of it backward.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=416307
        // Broiler-Falsified-If: a backward inline branch reaches its target without passing a debt test
        // Broiler-Human:        PENDING
        private int Transfer(int pc, int target)
        {
            if (target > pc)
            {
                return LabelOf(target);
            }

            var label = NewLabel();

            stubs.Add(() =>
            {
                Bind(label);
                DebtTest(target);
                Add(JsValueTemplate.Jmp, 0, LabelOf(target), JsValueRole.Debt, target);
            });

            return label;
        }

        // ---- the inline templates ------------------------------------------------------------------

        /// <summary>The inline template of the block's one instruction.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=2; Fingerprint=FEE111
        // Broiler-Falsified-If: an inline template writes a word before its last guard, writes a word other than its arm's outputs, or computes a value other than its arm's for an operand its guards admit
        // Broiler-Human:        PENDING
        private void Inline(JsBaselineBlock block)
        {
            var pc = block.Head;
            var opcode = block.HeadOpcode;
            var height = plan.HeightAt(pc);
            var operand = plan.OperandAt(pc);
            var slow = -1;

            Add(JsValueTemplate.IncDebt, 0, JsBaselineInstruction.None, JsValueRole.Debt, pc);

            switch (plan.InlineAt(pc))
            {
                case JsValueInline.Nop:
                case JsValueInline.Pop:
                    break;

                case JsValueInline.LoadWord:
                    Op(JsValueTemplate.MovRaxWord, operand, pc);
                    Op(JsValueTemplate.StoreRax, Slot(height), pc);
                    break;

                case JsValueInline.LoadArgument:
                    Op(JsValueTemplate.LoadRax, operand * 8, pc);
                    Op(JsValueTemplate.StoreRax, Slot(height), pc);
                    break;

                case JsValueInline.LoadResident:
                    Op(JsValueTemplate.LoadRax, operand * 8, pc);
                    Op(JsValueTemplate.MovRcxWord, unchecked((long)JsWord.Empty), pc);
                    Op(JsValueTemplate.CmpRaxRcx, 0, pc);
                    Guard(JsValueTemplate.Je, Slow(block, ref slow), pc);
                    Op(JsValueTemplate.StoreRax, Slot(height), pc);
                    break;

                case JsValueInline.StoreResident:
                    Op(JsValueTemplate.LoadRax, operand * 8, pc);
                    Op(JsValueTemplate.MovRcxWord, unchecked((long)JsWord.Empty), pc);
                    Op(JsValueTemplate.CmpRaxRcx, 0, pc);
                    Guard(JsValueTemplate.Je, Slow(block, ref slow), pc);
                    Op(JsValueTemplate.LoadRax, Slot(height - 1), pc);
                    Op(JsValueTemplate.StoreRax, operand * 8, pc);
                    break;

                case JsValueInline.InitialiseResident:
                    Op(JsValueTemplate.LoadRax, Slot(height - 1), pc);
                    Op(JsValueTemplate.StoreRax, operand * 8, pc);
                    break;

                case JsValueInline.Duplicate:
                    Op(JsValueTemplate.LoadRax, Slot(height - 1), pc);
                    Op(JsValueTemplate.StoreRax, Slot(height), pc);
                    break;

                case JsValueInline.DuplicateTwo:
                    Op(JsValueTemplate.LoadRax, Slot(height - 2), pc);
                    Op(JsValueTemplate.LoadRdx, Slot(height - 1), pc);
                    Op(JsValueTemplate.StoreRax, Slot(height), pc);
                    Op(JsValueTemplate.StoreRdx, Slot(height + 1), pc);
                    break;

                case JsValueInline.Swap:
                    Op(JsValueTemplate.LoadRax, Slot(height - 1), pc);
                    Op(JsValueTemplate.LoadRdx, Slot(height - 2), pc);
                    Op(JsValueTemplate.StoreRdx, Slot(height - 1), pc);
                    Op(JsValueTemplate.StoreRax, Slot(height - 2), pc);
                    break;

                case JsValueInline.Pick:
                    Op(JsValueTemplate.LoadRax, Slot(height - 1 - (int)operand), pc);
                    Op(JsValueTemplate.StoreRax, Slot(height), pc);
                    break;

                case JsValueInline.Void:
                    Op(JsValueTemplate.MovRaxWord, unchecked((long)JsWord.Undefined), pc);
                    Op(JsValueTemplate.StoreRax, Slot(height - 1), pc);
                    break;

                case JsValueInline.Arithmetic:
                    TwoNumbers(block, height, ref slow);
                    Op(opcode switch
                    {
                        JsOpcode.Add => JsValueTemplate.Addsd,
                        JsOpcode.Subtract => JsValueTemplate.Subsd,
                        JsOpcode.Multiply => JsValueTemplate.Mulsd,
                        _ => JsValueTemplate.Divsd,
                    }, 0, pc);
                    Op(JsValueTemplate.MovqRaxXmm0, 0, pc);
                    Op(JsValueTemplate.StoreRax, Slot(height - 2), pc);
                    break;

                case JsValueInline.Negate:
                    OneNumber(block, height, ref slow);
                    Op(JsValueTemplate.MovRcxWord, SignBit, pc);
                    Op(JsValueTemplate.XorRaxRcx, 0, pc);
                    Op(JsValueTemplate.StoreRax, Slot(height - 1), pc);
                    break;

                case JsValueInline.Step:
                    OneNumber(block, height, ref slow);
                    Op(JsValueTemplate.MovqXmm0Rax, 0, pc);
                    Op(JsValueTemplate.MovRcxWord, OneBits, pc);
                    Op(JsValueTemplate.MovqXmm1Rcx, 0, pc);
                    Op(opcode == JsOpcode.Increment ? JsValueTemplate.Addsd : JsValueTemplate.Subsd, 0, pc);
                    Op(JsValueTemplate.MovqRaxXmm0, 0, pc);
                    Op(JsValueTemplate.StoreRax, Slot(height - 1), pc);
                    break;

                case JsValueInline.ToNumber:
                    OneNumber(block, height, ref slow);
                    break;

                case JsValueInline.Compare:
                    TwoNumbers(block, height, ref slow);
                    Compare(opcode, pc);
                    Op(JsValueTemplate.MovzxEaxAl, 0, pc);
                    Op(JsValueTemplate.MovRcxWord, unchecked((long)JsWord.False), pc);
                    Op(JsValueTemplate.AddRaxRcx, 0, pc);
                    Op(JsValueTemplate.StoreRax, Slot(height - 2), pc);
                    break;

                case JsValueInline.Bitwise:
                    TwoNumbers(block, height, ref slow);
                    Op(JsValueTemplate.Cvttsd2siEax, 0, pc);
                    Op(JsValueTemplate.CmpEaxMin, 0, pc);
                    Guard(JsValueTemplate.Je, Slow(block, ref slow), pc);
                    Op(JsValueTemplate.Cvttsd2siEcx, 0, pc);
                    Op(JsValueTemplate.CmpEcxMin, 0, pc);
                    Guard(JsValueTemplate.Je, Slow(block, ref slow), pc);
                    Op(opcode switch
                    {
                        JsOpcode.BitwiseOr => JsValueTemplate.OrEaxEcx,
                        JsOpcode.BitwiseAnd => JsValueTemplate.AndEaxEcx,
                        JsOpcode.BitwiseXor => JsValueTemplate.XorEaxEcx,
                        JsOpcode.ShiftLeft => JsValueTemplate.ShlEaxCl,
                        JsOpcode.ShiftRight => JsValueTemplate.SarEaxCl,
                        _ => JsValueTemplate.ShrEaxCl,
                    }, 0, pc);

                    // `>>>` ANSWERS AN UNSIGNED THIRTY-TWO-BIT VALUE, which the thirty-two-bit operation left
                    // zero-extended in RAX; every other operator answers a signed one.
                    if (opcode != JsOpcode.ShiftRightUnsigned)
                    {
                        Op(JsValueTemplate.MovsxdRaxEax, 0, pc);
                    }

                    Op(JsValueTemplate.Cvtsi2sd, 0, pc);
                    Op(JsValueTemplate.MovqRaxXmm0, 0, pc);
                    Op(JsValueTemplate.StoreRax, Slot(height - 2), pc);
                    break;

                case JsValueInline.BitwiseNot:
                    OneNumber(block, height, ref slow);
                    Op(JsValueTemplate.MovqXmm0Rax, 0, pc);
                    Op(JsValueTemplate.Cvttsd2siEax, 0, pc);
                    Op(JsValueTemplate.CmpEaxMin, 0, pc);
                    Guard(JsValueTemplate.Je, Slow(block, ref slow), pc);
                    Op(JsValueTemplate.NotEax, 0, pc);
                    Op(JsValueTemplate.MovsxdRaxEax, 0, pc);
                    Op(JsValueTemplate.Cvtsi2sd, 0, pc);
                    Op(JsValueTemplate.MovqRaxXmm0, 0, pc);
                    Op(JsValueTemplate.StoreRax, Slot(height - 1), pc);
                    break;

                case JsValueInline.Not:
                {
                    var flip = NewLabel();
                    Op(JsValueTemplate.LoadRax, Slot(height - 1), pc);
                    Op(JsValueTemplate.MovRcxWord, unchecked((long)JsWord.False), pc);
                    Op(JsValueTemplate.CmpRaxRcx, 0, pc);
                    Add(JsValueTemplate.Je, 0, flip, JsValueRole.Inline, pc);
                    Op(JsValueTemplate.MovRcxWord, unchecked((long)JsWord.True), pc);
                    Op(JsValueTemplate.CmpRaxRcx, 0, pc);
                    Guard(JsValueTemplate.Jne, Slow(block, ref slow), pc);
                    Bind(flip);
                    Op(JsValueTemplate.XorRax1, 0, pc);
                    Op(JsValueTemplate.StoreRax, Slot(height - 1), pc);
                    break;
                }

                case JsValueInline.Jump:
                    if (block.Target <= pc)
                    {
                        DebtTest(block.Target);
                    }

                    Add(JsValueTemplate.Jmp, 0, LabelOf(block.Target), JsValueRole.Inline, pc);
                    break;

                case JsValueInline.Branch:
                    Branch(block, height, ref slow);
                    break;

                case JsValueInline.Return:
                    if (opcode == JsOpcode.Return)
                    {
                        Op(JsValueTemplate.LoadRax, Slot(height - 1), pc);
                    }
                    else
                    {
                        Op(JsValueTemplate.MovRaxWord, operand, pc);
                    }

                    Op(JsValueTemplate.StoreRax, 0, pc);
                    Add(JsValueTemplate.SpillDebt, 0, JsBaselineInstruction.None, JsValueRole.Debt, pc);
                    Op(JsValueTemplate.MovReturned, 0, pc);
                    Add(JsValueTemplate.Jmp, 0, JsBaselineInstruction.Leave, JsValueRole.Inline, pc);
                    break;
            }
        }

        /// <summary>
        /// A conditional jump on a Boolean, a Number, <c>undefined</c> or <c>null</c>: every other word goes
        /// to the helper, which reads the truthiness a handle has.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=4495BA
        // Broiler-Falsified-If: a Boolean, a Number, undefined or null goes the other way from the arm's ToBoolean, or any other word goes anywhere but the helper
        // Broiler-Human:        PENDING
        private void Branch(JsBaselineBlock block, int height, ref int slow)
        {
            var pc = block.Head;
            var whenFalse = block.HeadOpcode == JsOpcode.JumpIfFalse;
            var jump = Transfer(pc, block.Target);
            var next = LabelOf(block.Following);
            var falsy = whenFalse ? jump : next;
            var truthy = whenFalse ? next : jump;

            Op(JsValueTemplate.LoadRax, Slot(height - 1), pc);
            Op(JsValueTemplate.MovRcxWord, unchecked((long)JsWord.False), pc);
            Op(JsValueTemplate.CmpRaxRcx, 0, pc);
            Add(JsValueTemplate.Je, 0, falsy, JsValueRole.Inline, pc);
            Op(JsValueTemplate.MovRcxWord, unchecked((long)JsWord.True), pc);
            Op(JsValueTemplate.CmpRaxRcx, 0, pc);
            Add(JsValueTemplate.Je, 0, truthy, JsValueRole.Inline, pc);
            Op(JsValueTemplate.MovRcxWord, unchecked((long)JsWord.Undefined), pc);
            Op(JsValueTemplate.CmpRaxRcx, 0, pc);
            Add(JsValueTemplate.Je, 0, falsy, JsValueRole.Inline, pc);
            Op(JsValueTemplate.MovRcxWord, unchecked((long)JsWord.Null), pc);
            Op(JsValueTemplate.CmpRaxRcx, 0, pc);
            Add(JsValueTemplate.Je, 0, falsy, JsValueRole.Inline, pc);
            Op(JsValueTemplate.MovRcxWord, unchecked((long)JsWord.Undefined), pc);
            Op(JsValueTemplate.CmpRaxRcx, 0, pc);
            Guard(JsValueTemplate.Jae, Slow(block, ref slow), pc);
            Op(JsValueTemplate.MovqXmm0Rax, 0, pc);
            Op(JsValueTemplate.XorpdXmm1, 0, pc);
            Op(JsValueTemplate.Ucomisd01, 0, pc);
            Add(JsValueTemplate.Jp, 0, falsy, JsValueRole.Inline, pc);
            Add(JsValueTemplate.Je, 0, falsy, JsValueRole.Inline, pc);

            // A TRUTHY NUMBER FALLS INTO THE NEXT INSTRUCTION FOR `JumpIfFalse` AND JUMPS FOR `JumpIfTrue`.
            if (!whenFalse)
            {
                Add(JsValueTemplate.Jmp, 0, jump, JsValueRole.Inline, pc);
            }
        }

        /// <summary>Loads the one operand into RAX and refuses anything but a Number.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=0; Fingerprint=CBCEEF
        // Broiler-Falsified-If: a word that is not a Number passes the guard
        // Broiler-Human:        PENDING
        private void OneNumber(JsBaselineBlock block, int height, ref int slow)
        {
            var pc = block.Head;
            Op(JsValueTemplate.LoadRax, Slot(height - 1), pc);
            Op(JsValueTemplate.MovRcxWord, unchecked((long)JsWord.Undefined), pc);
            Op(JsValueTemplate.CmpRaxRcx, 0, pc);
            Guard(JsValueTemplate.Jae, Slow(block, ref slow), pc);
        }

        /// <summary>Loads the two operands into XMM0 and XMM1 and refuses anything but two Numbers.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=0; Fingerprint=1E6A99
        // Broiler-Falsified-If: a pair of which either word is not a Number passes the guards
        // Broiler-Human:        PENDING
        private void TwoNumbers(JsBaselineBlock block, int height, ref int slow)
        {
            var pc = block.Head;
            Op(JsValueTemplate.LoadRax, Slot(height - 2), pc);
            Op(JsValueTemplate.LoadRdx, Slot(height - 1), pc);
            Op(JsValueTemplate.MovRcxWord, unchecked((long)JsWord.Undefined), pc);
            Op(JsValueTemplate.CmpRaxRcx, 0, pc);
            Guard(JsValueTemplate.Jae, Slow(block, ref slow), pc);
            Op(JsValueTemplate.CmpRdxRcx, 0, pc);
            Guard(JsValueTemplate.Jae, Slow(block, ref slow), pc);
            Op(JsValueTemplate.MovqXmm0Rax, 0, pc);
            Op(JsValueTemplate.MovqXmm1Rdx, 0, pc);
        }

        /// <summary>
        /// The comparison of XMM0 with XMM1 into AL: every relational operator is false for an unordered
        /// pair, and equality is true for an ordered equal pair only.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=51A6EB
        // Broiler-Falsified-If: a comparison answers true for an unordered pair where the arm answers false, or differs from the arm for any ordered pair
        // Broiler-Human:        PENDING
        private void Compare(JsOpcode opcode, int pc)
        {
            switch (opcode)
            {
                case JsOpcode.LessThan:
                    Op(JsValueTemplate.Ucomisd10, 0, pc);
                    Op(JsValueTemplate.SetccAl, Above, pc);
                    break;

                case JsOpcode.LessThanOrEqual:
                    Op(JsValueTemplate.Ucomisd10, 0, pc);
                    Op(JsValueTemplate.SetccAl, AboveOrEqual, pc);
                    break;

                case JsOpcode.GreaterThan:
                    Op(JsValueTemplate.Ucomisd01, 0, pc);
                    Op(JsValueTemplate.SetccAl, Above, pc);
                    break;

                case JsOpcode.GreaterThanOrEqual:
                    Op(JsValueTemplate.Ucomisd01, 0, pc);
                    Op(JsValueTemplate.SetccAl, AboveOrEqual, pc);
                    break;

                case JsOpcode.StrictEquals or JsOpcode.LooseEquals:
                    Op(JsValueTemplate.Ucomisd01, 0, pc);
                    Op(JsValueTemplate.SetccAl, Equal, pc);
                    Op(JsValueTemplate.SetccDl, NoParity, pc);
                    Op(JsValueTemplate.AndAlDl, 0, pc);
                    break;

                default:
                    Op(JsValueTemplate.Ucomisd01, 0, pc);
                    Op(JsValueTemplate.SetccAl, NotEqual, pc);
                    Op(JsValueTemplate.SetccDl, Parity, pc);
                    Op(JsValueTemplate.OrAlDl, 0, pc);
                    break;
            }
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=81A7B4
        // Broiler-Human:        PENDING
        private const long Equal = 0x4;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=AD4BA8
        // Broiler-Human:        PENDING
        private const long NotEqual = 0x5;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=1F16C0
        // Broiler-Human:        PENDING
        private const long Above = 0x7;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=2C0CED
        // Broiler-Human:        PENDING
        private const long AboveOrEqual = 0x3;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=6EEC56
        // Broiler-Human:        PENDING
        private const long Parity = 0xA;
        // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=74C67E
        // Broiler-Human:        PENDING
        private const long NoParity = 0xB;

        /// <summary>The displacement of an operand-stack slot in the region.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=0; Fingerprint=987B00
        // Broiler-Falsified-If: a stack slot resolves to a word outside the region's operand stack
        // Broiler-Human:        PENDING
        private long Slot(int slot) => (long)(plan.StackBase + slot) * 8;

        /// <summary>The compare tree over a sorted range of landings, as <see cref="JsBaselineBlocks"/> lays it.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1B4948
        // Broiler-Falsified-If: a landing is compared anywhere but on a path to its own instruction, or an offset that is no landing reaches anything but the defect
        // Broiler-Human:        PENDING
        private void Tree(System.ReadOnlySpan<int> landings, int low, int high)
        {
            if (high - low + 1 <= JsBaselineBlocks.LeafLandings)
            {
                for (var position = low; position <= high; position++)
                {
                    Add(JsValueTemplate.CmpEaxPc, landings[position], JsBaselineInstruction.None, JsValueRole.Dispatch, -1);
                    Add(JsValueTemplate.Je, 0, LabelOf(landings[position]), JsValueRole.Dispatch, -1);
                }

                Add(JsValueTemplate.Jmp, 0, defect, JsValueRole.Dispatch, -1);
                return;
            }

            var middle = (low + high) / 2;
            var right = NewLabel();
            Add(JsValueTemplate.CmpEaxPc, landings[middle], JsBaselineInstruction.None, JsValueRole.Dispatch, -1);
            Add(JsValueTemplate.Je, 0, LabelOf(landings[middle]), JsValueRole.Dispatch, -1);
            Add(JsValueTemplate.Ja, 0, right, JsValueRole.Dispatch, -1);
            Tree(landings, low, middle - 1);
            Bind(right);
            Tree(landings, middle + 1, high);
        }

        // ---- the entries and the labels -------------------------------------------------------------

        /// <summary>An inline template's own work.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=639B6A
        // Broiler-Human:        PENDING
        private void Op(JsValueTemplate template, long operand, int pc) =>
            Add(template, operand, JsBaselineInstruction.None, JsValueRole.Inline, pc);

        /// <summary>A guard: a branch to the stub that calls the instruction's own helper.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=02B717
        // Broiler-Falsified-If: a guard is recorded with any role but Guard or any target but the stub it was handed
        // Broiler-Human:        PENDING
        private void Guard(JsValueTemplate template, int stub, int pc) =>
            Add(template, 0, stub, JsValueRole.Guard, pc);

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=72EC8C
        // Broiler-Human:        PENDING
        private void Add(JsValueTemplate template, long operand, int target, JsValueRole role, int pc) =>
            entries.Add(new JsValueInstruction(template, operand, target, role, pc));

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=50D7BF
        // Broiler-Human:        PENDING
        private int NewLabel()
        {
            labels.Add(-1);
            return labels.Count - 1;
        }

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=CB139E
        // Broiler-Human:        PENDING
        private void Bind(int label) => labels[label] = entries.Count;

        /// <summary>The label of the instruction at <paramref name="pc"/>, found by halving through the blocks.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=93754D
        // Broiler-Falsified-If: it answers the label of an instruction other than the one at the offset, or any label for an offset that is no instruction's
        // Broiler-Human:        PENDING
        private int LabelOf(int pc)
        {
            var blocks = plan.Blocks.Blocks;
            var low = 0;
            var high = blocks.Length - 1;

            while (low <= high)
            {
                var middle = low + ((high - low) / 2);

                if (blocks[middle].Head == pc)
                {
                    return instructionLabels[middle];
                }

                if (blocks[middle].Head < pc)
                {
                    low = middle + 1;
                }
                else
                {
                    high = middle - 1;
                }
            }

            throw new System.InvalidOperationException("the value layout names an offset that is no instruction's");
        }
    }
}
