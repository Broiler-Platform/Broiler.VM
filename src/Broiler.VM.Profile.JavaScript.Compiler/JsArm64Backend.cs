// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   37
// Annotated:        37/37
// Exempt:           5
// Human-reviewed:   0/37
// IP risk:          Low
// Security risk:    High
// Criteria:         14/14
// Resource impact:  5/10 max
// Unverified:       37
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The register and frame conventions the arm64 backend emits against, in one place.
/// </summary>
/// <remarks>
/// <para>
/// <b>AAPCS64 IS UNIFORM ACROSS LINUX, WINDOWS AND APPLE FOR THE ONE SIGNATURE THIS BACKEND EMITS,
/// AND THAT IS WHY THERE IS ONE arm64 BACKEND AND TWO x86-64 ONES.</b> The signature is fixed: one
/// pointer in, one integer out. That pointer arrives in x0 on every AAPCS64 variant, the answer
/// leaves in x0 on every one of them, x19 to x28 are callee-saved on every one of them, x29 is the
/// frame pointer and x30 the link register on every one of them, and the stack pointer is
/// sixteen-byte aligned at every instruction boundary on every one of them. The two x86-64
/// conventions disagree about the argument register and about which of RDI and RSI a callee must
/// preserve, which is why <see cref="JsNativeArchitecture"/> spells them apart; there is nothing
/// here to spell apart.
/// </para>
/// <para>
/// <b>The frame offsets below are <see cref="JsNativeFrame"/>'s declared layout, restated as
/// numbers because an encoder computes with numbers.</b> That structure's own contract states the
/// layout and states that it is sequential; this is the same fact in the form an emitter uses, and
/// a golden test asserts the two agree so a change to one cannot pass while the other stands still.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=79329A
// Broiler-Falsified-If: a field offset here differs from the offset the runtime gives that field of JsNativeFrame
// Broiler-Human:        PENDING
public static class JsArm64Frame
{
    /// <summary>Where the operand-slab pointer sits in the frame.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=67E192
    // Broiler-Human:        PENDING
    public const int OperandsOffset = 0;

    /// <summary>Where the operand-slab count sits in the frame.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1A5219
    // Broiler-Human:        PENDING
    public const int OperandCountOffset = 8;

    /// <summary>Where the local-slab pointer sits in the frame.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=86E9C2
    // Broiler-Human:        PENDING
    public const int LocalsOffset = 16;

    /// <summary>Where the local-slab count sits in the frame.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E3367D
    // Broiler-Human:        PENDING
    public const int LocalCountOffset = 24;

    /// <summary>Where the constant-slab pointer sits in the frame.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C182BB
    // Broiler-Human:        PENDING
    public const int ConstantsOffset = 32;

    /// <summary>Where the fuel-counter pointer sits in the frame.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=932A5F
    // Broiler-Human:        PENDING
    public const int FuelOffset = 40;

    /// <summary>Where the reached-bytecode-offset field sits in the frame.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=6209B3
    // Broiler-Human:        PENDING
    public const int BailoutPcOffset = 48;

    /// <summary>The callee-saved register the operand slab's base is held in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=A5D5B0
    // Broiler-Human:        PENDING
    public const int Operands = 19;

    /// <summary>The callee-saved register the local slab's base is held in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=9A3EFC
    // Broiler-Human:        PENDING
    public const int Locals = 20;

    /// <summary>The callee-saved register the constant slab's base is held in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7960D5
    // Broiler-Human:        PENDING
    public const int Constants = 21;

    /// <summary>The callee-saved register the frame pointer itself is held in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=EDD621
    // Broiler-Human:        PENDING
    public const int Frame = 22;

    /// <summary>The callee-saved register the fuel counter's address is held in.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=BF9398
    // Broiler-Human:        PENDING
    public const int Fuel = 23;

    /// <summary>A caller-saved integer scratch register, free because this backend calls nothing.</summary>
    /// <remarks>
    /// <b>x9 to x15 are corruptible across a call under AAPCS64, and an emitted unit here makes no
    /// call at all</b>, so nothing has to be preserved around anything and the whole
    /// spill-and-reload apparatus a register allocator exists for is absent by construction.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=EC73AA
    // Broiler-Human:        PENDING
    public const int Scratch = 9;

    /// <summary>A second caller-saved integer scratch register.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=42C846
    // Broiler-Human:        PENDING
    public const int SecondScratch = 10;

    /// <summary>The register the return code leaves in, which is also the argument register.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=491D6D
    // Broiler-Human:        PENDING
    public const int Result = 0;

    /// <summary>How many bytes of stack an emitted unit's prologue claims.</summary>
    /// <remarks>
    /// <b>Sixty-four, and it is a multiple of sixteen because AAPCS64 requires the stack pointer to
    /// be sixteen-byte aligned at every instruction and not merely at a call.</b> Five registers are
    /// saved and the pair instructions want them in pairs, so the frame carries one word of slack
    /// rather than an unaligned stack pointer.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=708AA7
    // Broiler-Falsified-If: the prologue leaves the stack pointer at an address that is not a multiple of sixteen
    // Broiler-Human:        PENDING
    public const int StackFrameBytes = 64;

    /// <summary>The bit pattern this backend represents <c>undefined</c> with.</summary>
    /// <remarks>
    /// <para>
    /// <b>IT IS A QUIET NaN, AND THE CONSEQUENCE IS THE ONE LIMIT OF THIS BACKEND A READER MUST NOT
    /// BE LEFT TO DISCOVER.</b> A slab of <c>double</c> has no encoding for <c>undefined</c>, and
    /// the numeric manifest writes <see cref="JsOpcode.LoadUndefined"/> in every program body it
    /// admits because the lowering gives a program a completion slot. Every arithmetic, unary and
    /// relational operator the manifest admits treats <c>undefined</c> and a NaN identically -
    /// <c>undefined + 1</c> and <c>NaN + 1</c> are both NaN, <c>undefined &lt; 1</c> and
    /// <c>NaN &lt; 1</c> are both false, <c>!undefined</c> and <c>!NaN</c> are both true - so within
    /// the emitted code the substitution is not observable.
    /// </para>
    /// <para>
    /// <b>The equality operators are where it WOULD be observable, and that is why this backend
    /// refuses all four of them.</b> <c>undefined === undefined</c> is true and
    /// <c>NaN === NaN</c> is false, so a template for <see cref="JsOpcode.StrictEquals"/> over this
    /// representation would be right for numbers and silently wrong for the one value that is not
    /// one. The refusal is by name and it says so; admitting them would need a static analysis
    /// proving no operand of an equality can be the completion slot, and this backend does not carry
    /// one.
    /// </para>
    /// <para>
    /// <b>And the value a unit RETURNS is still ambiguous, which no refusal fixes.</b> A caller
    /// reading operand slot zero cannot tell <c>undefined</c> from a NaN the program computed. That
    /// is a real difference between this form and the bytecode form, it is stated rather than
    /// hidden, and it is not made moot by the fact that nothing calls an emitted unit here.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=279E6F
    // Broiler-Falsified-If: an admitted operator distinguishes this bit pattern from a NaN the emitted code could itself produce
    // Broiler-Human:        PENDING
    public const ulong UndefinedBits = 0x7FF8000000000000UL;
}

/// <summary>
/// The arm64 backend: an emitter for AAPCS64 whose output is never executed.
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS BACKEND IS EMITTING-ONLY, AND THE REASON IS NOT A MISSING MACHINE.</b> A processor of
/// this architecture requires an instruction-cache maintenance sequence between the moment code is
/// written as data and the moment it is fetched as code - clean the data cache to the point of
/// unification, a data synchronisation barrier, invalidate the instruction cache to the point of
/// unification, a second barrier, an instruction synchronisation barrier - and <b>that sequence has
/// no managed expression and no dependable library export.</b> It is not an API; it is five
/// instructions the compiler that built this assembly will not emit and no runtime intrinsic
/// exposes, and the C entry point that usually stands in for it is a compiler builtin rather than a
/// symbol a platform invoke can bind to. Skipping it does not fail loudly: the processor fetches
/// whatever stale line the instruction cache already held, which is a defect that depends on cache
/// state and reproduces on one machine in ten.
/// </para>
/// <para>
/// <b>SO THERE IS NO ARMING PATH HERE, AND THIS PARAGRAPH STANDS WHERE ONE WOULD BE ADDED.</b>
/// Nothing in this file, and nothing anywhere in this component's arm64 half, maps memory, changes
/// a page protection, flushes a cache, takes the address of an emitted byte or reaches any
/// native-memory interface at all. A reader who arrives here intending to add the call that makes
/// these bytes runnable is the reader this paragraph is for: the missing piece is not the mapping
/// call, it is the maintenance sequence, and adding the mapping without it produces code that works
/// until it does not.
/// </para>
/// <para>
/// <b>WHAT PINS THIS BACKEND IS GOLDEN BYTES, AND A GOLDEN BYTE IS A CLAIM ABOUT WHAT THE ENCODER
/// WROTE AND NOT A CLAIM ABOUT WHAT A PROCESSOR WOULD DO.</b> That distinction is the whole
/// difference between this backend's evidence and an executed backend's, and it must not be
/// smudged: a golden test proves that the encoder is stable and that its words match encodings
/// derived by hand from the architecture's field layout, and it proves nothing whatever about
/// whether those words compute what this profile's interpreter computes. No figure, no support-table
/// row and no capability claim attaches to arm64 on the strength of a golden byte.
/// </para>
/// <para>
/// <b>What it compiles is a closed subset of the numeric manifest, and what it does not it refuses
/// by name.</b> The refusals are not gaps to be filled in later by whoever is passing: each one
/// names an instruction and the reason a template for it would be wrong rather than merely absent.
/// The three families are the realm instructions, whose operands are an object graph a frame of
/// doubles cannot hold; the equality instructions, for the reason
/// <see cref="JsArm64Frame.UndefinedBits"/> gives; and the instructions whose JavaScript semantics
/// no single A64 instruction has - <c>%</c> and <c>**</c>, which are library calls, and the bitwise
/// and shift operators, whose <c>ToInt32</c> is a truncation modulo two to the thirty-second that
/// <c>fcvtzs</c> does NOT perform for operands of magnitude two to the sixty-third or more, where it
/// saturates instead. A template that is right for small operands and silently wrong for large ones
/// is exactly the defect a golden byte cannot see.
/// </para>
/// <para>
/// <b>There is no register allocator and no instruction scheduler, and their absence is a design
/// decision rather than an omission.</b> The operand stack lives in the frame's operand slab at
/// compile-time-known offsets, two floating-point scratch registers and two integer ones carry every
/// intermediate, and instructions are emitted in bytecode order. That makes the output slow and
/// makes it DETERMINISTIC AND READABLE, which are the two properties an emitting-only backend can
/// actually be held to: one program compiles to one byte sequence, and a reader can decode the
/// sequence by hand.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=5C4732
// Broiler-Falsified-If: this component maps, protects or executes memory on behalf of an arm64 emission, or emits a template for an instruction whose JavaScript semantics that template does not implement
// Broiler-Human:        PENDING
public sealed class JsArm64Backend : IJsNativeBackend
{
    /// <summary>The most operand slots, local slots or constants an emitted unit can address.</summary>
    /// <remarks>
    /// <b>It is the reach of the unsigned twelve-bit scaled immediate a load or store carries, and
    /// not a policy.</b> A slot past it needs a second instruction to form the address, and this
    /// backend refuses instead: an emitter that quietly grew a two-instruction addressing form for
    /// large frames would have two templates per access and would have to be right about both.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B347C5
    // Broiler-Human:        PENDING
    public const int SlotReach = 4095;

    /// <summary>The alignment every emitted unit's entry point is written at.</summary>
    /// <remarks>
    /// <b>Four, because A64 requires instruction words to be four-byte aligned and requires nothing
    /// more.</b> Every instruction is four bytes, so every unit that follows another is already
    /// aligned and no padding byte is ever written; an alignment chosen for a cache line would mean
    /// padding, and padding in an emitted section is bytes nothing accounts for.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=F3F7D6
    // Broiler-Human:        PENDING
    public const uint Alignment = 4;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=6119E3
    // Broiler-Human:        PENDING
    public string Name => JsNativeBackends.Arm64;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2A0308
    // Broiler-Human:        PENDING
    public JsNativeArchitecture Architecture => JsNativeArchitecture.Arm64;

    /// <summary>One: the version of the first backend of this component that encodes an instruction.</summary>
    /// <remarks>
    /// <b>Zero is taken and permanently means "emitted nothing".</b>
    /// <see cref="JsUnwrittenNativeBackend"/> answers zero so that no artifact produced before an
    /// encoder existed can be confused with one produced after; this is the next number, and it
    /// travels in the artifact so that an image can refuse a payload written against a contract it
    /// has since changed.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B49274
    // Broiler-Human:        PENDING
    public uint SemanticVersion => 1;

    /// <summary>Emits the whole program for AAPCS64, or refuses it and says why.</summary>
    /// <remarks>
    /// <b>THE WHOLE ARTIFACT OR NOTHING, because the profile's non-goals fix one form per
    /// handle.</b> A backend that emitted the units it liked would leave one artifact carrying two
    /// forms, which is the per-unit choice that rule refuses. So a single unit this backend cannot
    /// compile refuses the artifact, and the sentence names the unit, the bytecode offset and the
    /// instruction - because an author told only "arm64 declined" has no way to find out why.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=C3B8A0
    // Broiler-Falsified-If: this method answers true for a program containing a unit it emitted no entry point for, or answers true having emitted a template for an instruction it does not admit
    // Broiler-Human:        PENDING
    public bool TryEmit(JsAssembledProgram program, out JsNativeEmission emission, out string refusal)
    {
        emission = null!;

        if (program.ManifestId != JsNumericManifest.ManifestId)
        {
            refusal =
                "the arm64 backend emits only for the `" + JsNumericManifest.ManifestId +
                "` feature manifest, and this program names `" + program.ManifestId + "`";

            return false;
        }

        if (program.ExceptionRegions.Count != 0)
        {
            refusal =
                "the arm64 backend emits no exception region, and this program carries " +
                program.ExceptionRegions.Count;

            return false;
        }

        var assembler = new JsArm64Assembler();
        var symbols = new JsNativeSymbolRow[program.Functions.Count];

        for (var index = 0; index < program.Functions.Count; index++)
        {
            symbols[index] = new JsNativeSymbolRow((uint)index, (uint)assembler.ByteCount);

            if (!EmitUnit(assembler, program, index, out refusal))
            {
                return false;
            }
        }

        if (!assembler.TryFix(out refusal))
        {
            return false;
        }

        emission = new JsNativeEmission(
            JsNativeArchitecture.Arm64, SemanticVersion, Alignment, assembler.ToArray(), symbols);

        refusal = string.Empty;
        return true;
    }

    /// <summary>Emits one code unit: a prologue, the reachable blocks in order, and two epilogues.</summary>
    /// <remarks>
    /// <b>UNREACHABLE BYTECODE IS EMITTED AS NOTHING AT ALL, and that is sound rather than
    /// convenient.</b> The lowering writes a trailing <see cref="JsOpcode.ReturnUndefined"/> after
    /// a body that always returns, so a unit's last instruction is routinely dead. A branch target
    /// is by definition reachable from the branch that names it, so no branch can reach a byte this
    /// pass skipped, and a fall-through cannot either.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=60ADDC
    // Broiler-Falsified-If: an offset this pass treats as unreachable is the target of a branch it emitted
    // Broiler-Human:        PENDING
    private static bool EmitUnit(
        JsArm64Assembler assembler, JsAssembledProgram program, int index, out string refusal)
    {
        var row = program.Functions[index];
        var start = (int)row.CodeOffset;
        var end = start + (int)row.CodeLength;

        if (!JsNumericManifest.AdmitsFlags((JsFormat.FunctionFlags)row.Flags))
        {
            refusal =
                "unit " + index + " carries flags the numeric manifest does not admit, so the " +
                "arm64 backend has no entry-point shape for it";

            return false;
        }

        var walk = new JsArm64Walk(program, row);

        if (!walk.TryTrace(out refusal))
        {
            refusal = "unit " + index + ": " + refusal;
            return false;
        }

        var done = assembler.DefineLabel();
        var exhausted = assembler.DefineLabel();
        var targets = new int[end - start];

        for (var offset = 0; offset < targets.Length; offset++)
        {
            targets[offset] = walk.IsReachable(start + offset) ? assembler.DefineLabel() : -1;
        }

        Prologue(assembler);

        for (var at = start; at < end;)
        {
            var opcode = (JsOpcode)program.Code[at];
            var width = JsOpcodes.InstructionWidth(opcode);

            if (!walk.IsReachable(at))
            {
                at += width;
                continue;
            }

            assembler.Bind(targets[at - start]);

            if (walk.IsLeader(at))
            {
                Charge(assembler, walk.BlockCost(at), at, exhausted);
            }

            if (!EmitInstruction(assembler, program, walk, at, targets, start, done, out refusal))
            {
                refusal = "unit " + index + " at bytecode offset " + at + ": " + refusal;
                return false;
            }

            at += width;
        }

        assembler.Bind(exhausted);
        assembler.MoveWide32(JsArm64Frame.Result, (ushort)JsNativeReturn.FuelExhausted);
        assembler.Bind(done);
        Epilogue(assembler);

        refusal = string.Empty;
        return true;
    }

    /// <summary>Emits the prologue: save the callee-saved registers, then read the frame apart.</summary>
    /// <remarks>
    /// <b>THE FIVE REGISTERS ARE SAVED BECAUSE AAPCS64 SAYS x19 TO x28 BELONG TO THE CALLER, and
    /// the frame pointer and link register are saved because a debugger walks them.</b> They are
    /// saved as pairs because A64's pair instructions move two registers in one word and because a
    /// pre-indexed pair store claims the stack in the same instruction that fills the first eight
    /// bytes of it - which is the shape a stack unwinder is written to recognise.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=9051BC
    // Broiler-Falsified-If: a register this prologue saves is not restored by the epilogue to the same stack slot
    // Broiler-Human:        PENDING
    private static void Prologue(JsArm64Assembler assembler)
    {
        assembler.StorePairPreIndex(
            JsArm64Assembler.FramePointer,
            JsArm64Assembler.Link,
            JsArm64Assembler.StackOrZero,
            -JsArm64Frame.StackFrameBytes);

        assembler.StorePair(
            JsArm64Frame.Operands, JsArm64Frame.Locals, JsArm64Assembler.StackOrZero, 16);

        assembler.StorePair(
            JsArm64Frame.Constants, JsArm64Frame.Frame, JsArm64Assembler.StackOrZero, 32);

        assembler.StoreRegister(JsArm64Frame.Fuel, JsArm64Assembler.StackOrZero, 48);
        assembler.MoveFromStackPointer(JsArm64Assembler.FramePointer);
        assembler.Move(JsArm64Frame.Frame, JsArm64Frame.Result);

        assembler.LoadRegister(
            JsArm64Frame.Operands, JsArm64Frame.Frame, JsArm64Frame.OperandsOffset);

        assembler.LoadRegister(JsArm64Frame.Locals, JsArm64Frame.Frame, JsArm64Frame.LocalsOffset);

        assembler.LoadRegister(
            JsArm64Frame.Constants, JsArm64Frame.Frame, JsArm64Frame.ConstantsOffset);

        assembler.LoadRegister(JsArm64Frame.Fuel, JsArm64Frame.Frame, JsArm64Frame.FuelOffset);
    }

    /// <summary>Emits the epilogue: restore what the prologue saved, then return.</summary>
    /// <remarks>
    /// <b>The stack pointer is restored by the WRITEBACK of the last load pair and by nothing
    /// else</b>, which is why the three loads before it read positive offsets from the stack pointer
    /// the prologue left rather than from the frame pointer. It is also why the <c>ret 8</c> defect
    /// the core retains as a fixture has no spelling here: the amount is part of a load, not part of
    /// the return.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=A20958
    // Broiler-Falsified-If: this epilogue leaves the stack pointer at a value other than its value on entry to the unit
    // Broiler-Human:        PENDING
    private static void Epilogue(JsArm64Assembler assembler)
    {
        assembler.LoadRegister(JsArm64Frame.Fuel, JsArm64Assembler.StackOrZero, 48);

        assembler.LoadPair(
            JsArm64Frame.Constants, JsArm64Frame.Frame, JsArm64Assembler.StackOrZero, 32);

        assembler.LoadPair(
            JsArm64Frame.Operands, JsArm64Frame.Locals, JsArm64Assembler.StackOrZero, 16);

        assembler.LoadPairPostIndex(
            JsArm64Assembler.FramePointer,
            JsArm64Assembler.Link,
            JsArm64Assembler.StackOrZero,
            JsArm64Frame.StackFrameBytes);

        assembler.Return();
    }

    /// <summary>Emits one basic block's fuel charge and the branch out when the counter goes negative.</summary>
    /// <remarks>
    /// <para>
    /// <b>FUEL IS CHARGED PER BASIC BLOCK AND THE INTERPRETER CHARGES PER INSTRUCTION, SO THE TWO
    /// FORMS EXHAUST AT DIFFERENT POINTS.</b> Emitted code cannot call a managed meter once per
    /// instruction without being slower than the interpreter it replaces; what it can do is
    /// subtract a block's whole instruction count from an unmanaged counter on entry to the block.
    /// The totals agree for a program that runs to completion and the exhaustion POINT does not,
    /// which is a real difference between the forms and is recorded here rather than left for a
    /// corpus row to disagree about.
    /// </para>
    /// <para>
    /// <b>The subtract is repeated rather than widened when a block is enormous</b>, because the
    /// immediate is twelve bits and an emitter that reached for a wider form would be carrying two
    /// templates where one will do. An intermediate value going negative between two subtracts is
    /// harmless: only the last one's flags are read.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=92DD0E
    // Broiler-Falsified-If: a block charges a number of units other than the count of bytecode instructions it contains
    // Broiler-Human:        PENDING
    private static void Charge(JsArm64Assembler assembler, int cost, int at, int exhausted)
    {
        assembler.LoadRegister(JsArm64Frame.Scratch, JsArm64Frame.Fuel, 0);

        var remaining = cost;

        while (remaining > 4095)
        {
            assembler.SubtractSetting(JsArm64Frame.Scratch, JsArm64Frame.Scratch, 4095);
            remaining -= 4095;
        }

        assembler.SubtractSetting(JsArm64Frame.Scratch, JsArm64Frame.Scratch, remaining);
        assembler.StoreRegister(JsArm64Frame.Scratch, JsArm64Frame.Fuel, 0);

        var enough = assembler.DefineLabel();
        assembler.BranchIf(JsArm64Condition.GreaterOrEqual, enough);
        assembler.MoveWide32(JsArm64Frame.Scratch, (ushort)(at & 0xFFFF));
        assembler.MoveKeep32High(JsArm64Frame.Scratch, (ushort)((at >> 16) & 0xFFFF));

        assembler.StoreWord(
            JsArm64Frame.Scratch, JsArm64Frame.Frame, JsArm64Frame.BailoutPcOffset);

        assembler.Branch(exhausted);
        assembler.Bind(enough);
    }

    /// <summary>Emits the template for one bytecode instruction, or refuses it by name.</summary>
    /// <remarks>
    /// <para>
    /// <b>EVERY ADMITTED OPCODE HAS EXACTLY ONE TEMPLATE AND THE TEMPLATE NEVER VARIES WITH ITS
    /// OPERANDS.</b> A store of the top operand is the same three words whether the height is one or
    /// nine, a <c>Return</c> emits its load and store even when the value is already in slot zero,
    /// and no case here looks at a constant's value. That costs instructions and buys the property
    /// this backend can actually be held to: one program compiles to one byte sequence, and a golden
    /// sequence can be decoded by hand.
    /// </para>
    /// <para>
    /// <b>AND THE DEFAULT ARM REFUSES BY NAME RATHER THAN EMITTING A NOP.</b> An opcode with no
    /// template is not a gap to be papered over: it is an instruction whose meaning this backend
    /// does not implement, and emitting anything at all for it would produce a well-framed sequence
    /// that computes the wrong thing - the one defect class a verifier of machine code cannot see.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=104389
    // Broiler-Falsified-If: a template here computes something other than what this profile's interpreter computes for the same instruction over Number operands
    // Broiler-Human:        PENDING
    private static bool EmitInstruction(
        JsArm64Assembler assembler,
        JsAssembledProgram program,
        JsArm64Walk walk,
        int at,
        int[] targets,
        int start,
        int done,
        out string refusal)
    {
        var opcode = (JsOpcode)program.Code[at];
        var height = walk.HeightAt(at);
        refusal = string.Empty;

        switch (opcode)
        {
            case JsOpcode.LoadUndefined:
                Undefined(assembler);
                assembler.StoreDouble(0, JsArm64Frame.Operands, height * 8);
                return true;

            case JsOpcode.LoadConstant:
                assembler.LoadDouble(0, JsArm64Frame.Constants, Operand(program.Code, at) * 8);
                assembler.StoreDouble(0, JsArm64Frame.Operands, height * 8);
                return true;

            case JsOpcode.LoadScoped:
                assembler.LoadDouble(0, JsArm64Frame.Locals, walk.FlatSlot(at) * 8);
                assembler.StoreDouble(0, JsArm64Frame.Operands, height * 8);
                return true;

            case JsOpcode.StoreScoped:
            case JsOpcode.InitialiseScoped:
                assembler.LoadDouble(0, JsArm64Frame.Operands, (height - 1) * 8);
                assembler.StoreDouble(0, JsArm64Frame.Locals, walk.FlatSlot(at) * 8);
                return true;

            case JsOpcode.Duplicate:
                assembler.LoadDouble(0, JsArm64Frame.Operands, (height - 1) * 8);
                assembler.StoreDouble(0, JsArm64Frame.Operands, height * 8);
                return true;

            // A POP, A SCOPE PUSH, A SCOPE POP, A SCOPE COPY AND A ToNumber ARE ALL ZERO
            // INSTRUCTIONS, and each is zero for its own reason. A pop is a decrement of a
            // compile-time height. A scope push and pop move a compile-time base, because this
            // backend flattens every scope of a unit into one slab. A scope copy would matter only
            // if something could capture the environment it replaces, and the numeric manifest
            // admits no function expression, no arrow and no nested declaration, so nothing can.
            // A ToNumber over a slab of doubles is the identity, including over the value this
            // backend represents `undefined` with, whose ToNumber is a NaN and which is one.
            case JsOpcode.Pop:
            case JsOpcode.PushScope:
            case JsOpcode.PopScope:
            case JsOpcode.CopyScope:
            case JsOpcode.ToNumber:
                return true;

            case JsOpcode.Add:
                Binary(assembler, height, JsArm64Operation.Add);
                return true;

            case JsOpcode.Subtract:
                Binary(assembler, height, JsArm64Operation.Subtract);
                return true;

            case JsOpcode.Multiply:
                Binary(assembler, height, JsArm64Operation.Multiply);
                return true;

            case JsOpcode.Divide:
                Binary(assembler, height, JsArm64Operation.Divide);
                return true;

            case JsOpcode.Negate:
                assembler.LoadDouble(0, JsArm64Frame.Operands, (height - 1) * 8);
                assembler.NegateDouble(0, 0);
                assembler.StoreDouble(0, JsArm64Frame.Operands, (height - 1) * 8);
                return true;

            case JsOpcode.Not:
                Falsy(assembler, height);
                return true;

            case JsOpcode.LessThan:
                Compare(assembler, height, swap: false, JsArm64Condition.Minus);
                return true;

            case JsOpcode.GreaterThan:
                Compare(assembler, height, swap: false, JsArm64Condition.Greater);
                return true;

            case JsOpcode.GreaterThanOrEqual:
                Compare(assembler, height, swap: false, JsArm64Condition.GreaterOrEqual);
                return true;

            // `a <= b` IS EMITTED AS `b >= a` AND NOT AS THE CONDITION WHOSE NAME MATCHES IT. A64's
            // LE is true when the compare was unordered, and `NaN <= 1` is false in this language,
            // so the operands are swapped and GE - which is false when unordered - answers instead.
            case JsOpcode.LessThanOrEqual:
                Compare(assembler, height, swap: true, JsArm64Condition.GreaterOrEqual);
                return true;

            case JsOpcode.Jump:
                assembler.Branch(targets[Operand(program.Code, at) - start]);
                return true;

            case JsOpcode.JumpIfFalse:
                assembler.LoadDouble(0, JsArm64Frame.Operands, (height - 1) * 8);
                assembler.CompareDoubleWithZero(0);
                assembler.BranchIf(
                    JsArm64Condition.Equal, targets[Operand(program.Code, at) - start]);
                assembler.BranchIf(
                    JsArm64Condition.Overflow, targets[Operand(program.Code, at) - start]);
                return true;

            case JsOpcode.JumpIfTrue:
                JumpIfTrue(assembler, height, targets[Operand(program.Code, at) - start]);
                return true;

            case JsOpcode.Return:
                assembler.LoadDouble(0, JsArm64Frame.Operands, (height - 1) * 8);
                assembler.StoreDouble(0, JsArm64Frame.Operands, 0);
                assembler.MoveWide32(JsArm64Frame.Result, (ushort)JsNativeReturn.Returned);
                assembler.Branch(done);
                return true;

            case JsOpcode.ReturnUndefined:
                Undefined(assembler);
                assembler.StoreDouble(0, JsArm64Frame.Operands, 0);
                assembler.MoveWide32(JsArm64Frame.Result, (ushort)JsNativeReturn.Returned);
                assembler.Branch(done);
                return true;

            default:
                refusal = Why(opcode);
                return false;
        }
    }

    /// <summary>Says, for an instruction this backend does not emit, why a template would be wrong.</summary>
    /// <remarks>
    /// <b>THE THREE FAMILIES ARE THREE DIFFERENT MISTAKES AND THE SENTENCES SAY SO.</b> A realm
    /// instruction is refused because its operand is an object graph and the emitted frame carries
    /// no reference by construction; an equality is refused because a slab of doubles cannot tell
    /// <c>undefined</c> from a NaN; a remainder, an exponentiation or a bitwise operator is refused
    /// because no single A64 instruction has its JavaScript semantics. A reader told only "not
    /// supported" would go looking for the encoder somebody forgot to write, and for two of these
    /// three families there is nothing to write.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=CE5265
    // Broiler-Human:        PENDING
    private static string Why(JsOpcode opcode) => opcode switch
    {
        JsOpcode.LoadGlobal or JsOpcode.StoreGlobal or JsOpcode.DeclareGlobal or
        JsOpcode.DeclareGlobalLet or JsOpcode.DeclareGlobalConst or
        JsOpcode.InitialiseGlobalLexical =>
            "the instruction `" + opcode + "` reaches the realm, which is an object graph, and the " +
            "frame an emitted unit is handed carries pointers to slabs of doubles and no managed " +
            "reference at all - which is how this profile answers the rooting question by " +
            "construction rather than by a scheme",

        JsOpcode.Closure or JsOpcode.Call =>
            "the instruction `" + opcode + "` calls another code unit, and the frame an emitted " +
            "unit is handed carries no table of entry points and no second operand slab to call " +
            "one with; a call is a contract this backend has not been given",

        JsOpcode.StrictEquals or JsOpcode.StrictNotEquals or
        JsOpcode.LooseEquals or JsOpcode.LooseNotEquals =>
            "the instruction `" + opcode + "` is the one family of operator that can tell " +
            "`undefined` from a NaN, and this backend represents `undefined` as a NaN because a " +
            "slab of doubles has no other encoding for it; a template here would be right for " +
            "numbers and silently wrong for the completion value every program body carries",

        JsOpcode.Remainder or JsOpcode.Exponent =>
            "the instruction `" + opcode + "` has no A64 instruction: its JavaScript semantics are " +
            "a library routine, and an emitted unit of this backend calls nothing",

        JsOpcode.BitwiseNot or JsOpcode.BitwiseOr or JsOpcode.BitwiseAnd or JsOpcode.BitwiseXor or
        JsOpcode.ShiftLeft or JsOpcode.ShiftRight or JsOpcode.ShiftRightUnsigned =>
            "the instruction `" + opcode + "` converts its operands with ToInt32, which truncates " +
            "modulo two to the thirty-second, and A64's `fcvtzs` SATURATES for operands of " +
            "magnitude two to the sixty-third or more instead; a template built on it would be " +
            "right for small operands and silently wrong for large ones, which is the defect class " +
            "a golden byte cannot see",

        _ =>
            "the instruction `" + opcode + "` is outside the closed set this backend emits, and a " +
            "backend that emitted something for an instruction it has no template for would be " +
            "manufacturing a well-framed sequence that computes the wrong thing",
    };

    /// <summary>Which floating-point instruction a binary arithmetic template writes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=9FBF03
    // Broiler-Human:        PENDING
    private enum JsArm64Operation
    {
        Add,
        Subtract,
        Multiply,
        Divide,
    }

    /// <summary>Emits the four words a binary arithmetic instruction is.</summary>
    /// <remarks>
    /// <b>JavaScript's <c>+</c>, <c>-</c>, <c>*</c> and <c>/</c> over two Numbers ARE the IEEE-754
    /// binary64 operations, so each is one instruction and no rounding or special-case code is
    /// written.</b> The specification says the result is computed with the rules of IEEE-754,
    /// including that a NaN operand gives a NaN, that division by zero gives an infinity of the
    /// right sign, and that the sign of a zero is preserved - which is exactly what these four
    /// instructions do and exactly why an emitter must NOT be tempted to help.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=1D962A
    // Broiler-Falsified-If: the operands are loaded in an order that makes a subtraction or a division compute the reverse
    // Broiler-Human:        PENDING
    private static void Binary(JsArm64Assembler assembler, int height, JsArm64Operation operation)
    {
        assembler.LoadDouble(0, JsArm64Frame.Operands, (height - 2) * 8);
        assembler.LoadDouble(1, JsArm64Frame.Operands, (height - 1) * 8);

        switch (operation)
        {
            case JsArm64Operation.Add:
                assembler.AddDouble(0, 0, 1);
                break;

            case JsArm64Operation.Subtract:
                assembler.SubtractDouble(0, 0, 1);
                break;

            case JsArm64Operation.Multiply:
                assembler.MultiplyDouble(0, 0, 1);
                break;

            default:
                assembler.DivideDouble(0, 0, 1);
                break;
        }

        assembler.StoreDouble(0, JsArm64Frame.Operands, (height - 2) * 8);
    }

    /// <summary>Emits the six words a relational comparison is.</summary>
    /// <remarks>
    /// <b>The result is 0.0 or 1.0 in a slab of doubles, which is how a Boolean is represented
    /// here.</b> The manifest admits no Boolean literal and no way for a source to name one, so the
    /// only Booleans in an emitted unit are the ones these six words make and the ones a branch
    /// immediately consumes; representing them as the two doubles a <c>scvtf</c> of a
    /// <c>cset</c> produces keeps every slot in the frame one kind of thing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=ACABB6
    // Broiler-Falsified-If: a comparison answers true for a pair in which either operand is a NaN
    // Broiler-Human:        PENDING
    private static void Compare(
        JsArm64Assembler assembler, int height, bool swap, JsArm64Condition condition)
    {
        assembler.LoadDouble(0, JsArm64Frame.Operands, (height - 2) * 8);
        assembler.LoadDouble(1, JsArm64Frame.Operands, (height - 1) * 8);
        assembler.CompareDouble(swap ? 1 : 0, swap ? 0 : 1);
        assembler.SetIfCondition(JsArm64Frame.Scratch, condition);
        assembler.ConvertToDouble(0, JsArm64Frame.Scratch);
        assembler.StoreDouble(0, JsArm64Frame.Operands, (height - 2) * 8);
    }

    /// <summary>Emits the seven words logical negation is.</summary>
    /// <remarks>
    /// <b>ToBoolean of a Number is false for a positive zero, a negative zero AND a NaN, which is
    /// two conditions and not one.</b> A compare against zero answers EQ for either zero and VS for
    /// a NaN, so the two are computed separately and or-ed; a template that tested only EQ would
    /// answer <c>!NaN</c> as false, which is the wrong answer to the one input a reader would never
    /// think to try.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=ED52A4
    // Broiler-Falsified-If: this template answers false for a NaN operand
    // Broiler-Human:        PENDING
    private static void Falsy(JsArm64Assembler assembler, int height)
    {
        assembler.LoadDouble(0, JsArm64Frame.Operands, (height - 1) * 8);
        assembler.CompareDoubleWithZero(0);
        assembler.SetIfCondition(JsArm64Frame.Scratch, JsArm64Condition.Equal);
        assembler.SetIfCondition(JsArm64Frame.SecondScratch, JsArm64Condition.Overflow);
        assembler.Or(JsArm64Frame.Scratch, JsArm64Frame.Scratch, JsArm64Frame.SecondScratch);
        assembler.ConvertToDouble(0, JsArm64Frame.Scratch);
        assembler.StoreDouble(0, JsArm64Frame.Operands, (height - 1) * 8);
    }

    /// <summary>Emits the five words a branch-if-truthy is.</summary>
    /// <remarks>
    /// <b>It is spelled as two branches AWAY and one branch TO, because the falsy test is a
    /// disjunction and A64 has no condition that is the negation of one.</b> The value is falsy
    /// when the compare answers EQ or VS, so both of those skip the branch and everything else takes
    /// it; a single conditional branch would have to name a condition meaning "neither EQ nor VS",
    /// and there is no such condition code.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=2772AF
    // Broiler-Falsified-If: this template branches for an operand that is a zero or a NaN
    // Broiler-Human:        PENDING
    private static void JumpIfTrue(JsArm64Assembler assembler, int height, int target)
    {
        var skip = assembler.DefineLabel();
        assembler.LoadDouble(0, JsArm64Frame.Operands, (height - 1) * 8);
        assembler.CompareDoubleWithZero(0);
        assembler.BranchIf(JsArm64Condition.Equal, skip);
        assembler.BranchIf(JsArm64Condition.Overflow, skip);
        assembler.Branch(target);
        assembler.Bind(skip);
    }

    /// <summary>Emits the two words that put this backend's <c>undefined</c> into d0.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F5D902
    // Broiler-Human:        PENDING
    private static void Undefined(JsArm64Assembler assembler)
    {
        assembler.MoveWide(
            JsArm64Frame.Scratch, (ushort)(JsArm64Frame.UndefinedBits >> 48), halfword: 3);

        assembler.MoveBitsToDouble(0, JsArm64Frame.Scratch);
    }

    /// <summary>Reads the unsigned operand of the instruction at <paramref name="at"/>.</summary>
    /// <remarks>
    /// <b>The width comes from the opcode table and not from a guess</b>, which is the whole reason
    /// a backend can attach to this bytecode at all: an instruction's length is a function of its
    /// first byte, so a walk over a code range needs no separate map.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=625123
    // Broiler-Human:        PENDING
    private static int Operand(byte[] code, int at) =>
        JsOpcodes.OperandWidth((JsOpcode)code[at]) switch
        {
            1 => code[at + 1],
            2 => code[at + 1] | (code[at + 2] << 8),
            4 => code[at + 1] | (code[at + 2] << 8) | (code[at + 3] << 16) | (code[at + 4] << 24),
            3 => code[at + 2] | (code[at + 3] << 8),
            _ => 0,
        };
}
