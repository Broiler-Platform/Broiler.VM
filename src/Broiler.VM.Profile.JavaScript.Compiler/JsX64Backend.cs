// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   29
// Annotated:        29/29
// Exempt:           5
// Human-reviewed:   0/29
// IP risk:          Low
// Security risk:    High
// Criteria:         17/17
// Resource impact:  3/10 max
// Unverified:       29
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// <see cref="JsNativeFrame"/>'s declared layout, restated as the numbers an encoder computes with.
/// </summary>
/// <remarks>
/// <b>The structure's own contract states this layout in prose and this states it in bytes, and a
/// test asserts the two agree.</b> An encoder cannot read an XML comment, so the numbers have to
/// exist somewhere; what makes the duplication safe is that the test constructs the real structure
/// and measures where the runtime actually put each field, so a change to one of the two that the
/// other did not follow fails rather than miscompiles.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=5FA1CA
// Broiler-Falsified-If: a field offset here differs from the offset the runtime gives that field of JsNativeFrame
// Broiler-Human:        PENDING
public static class JsX64Frame
{
    /// <summary>Where the operand-slab pointer sits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=67E192
    // Broiler-Human:        PENDING
    public const int OperandsOffset = 0;

    /// <summary>Where the remaining-operand-slot count sits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1A5219
    // Broiler-Human:        PENDING
    public const int OperandCountOffset = 8;

    /// <summary>Where the realm-binding slab pointer sits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=86E9C2
    // Broiler-Human:        PENDING
    public const int LocalsOffset = 16;

    /// <summary>Where the realm-binding slab's slot count sits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E3367D
    // Broiler-Human:        PENDING
    public const int LocalCountOffset = 24;

    /// <summary>Where the constant slab pointer sits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C182BB
    // Broiler-Human:        PENDING
    public const int ConstantsOffset = 32;

    /// <summary>Where the fuel counter's address sits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=932A5F
    // Broiler-Human:        PENDING
    public const int FuelOffset = 40;
}

/// <summary>Everything one unit's emission needs that is the same at every instruction of it.</summary>
/// <param name="Abi">The calling convention this unit is being emitted for.</param>
/// <param name="ScopeSlots">How many slots of the unit's region the flattened scope chain takes.</param>
/// <param name="RegionSlots">How many slots of the operand slab the whole region takes.</param>
/// <param name="Entries">One label per code unit, so a call is a direct branch.</param>
/// <param name="ReturnLabel">The epilogue that answers a normal return with the value in XMM0.</param>
/// <param name="PropagateLabel">The epilogue that answers with whatever code is already in EAX.</param>
/// <param name="ThrowLabel">The epilogue that answers a guest throw.</param>
/// <param name="FuelLabel">The epilogue that answers an exhausted fuel counter.</param>
/// <param name="Offsets">One label per bytecode offset of this unit, bound where it is emitted.</param>
/// <param name="Functions">Which code unit each function-valued realm binding holds.</param>
/// <param name="ConstantBindings">The realm bindings a <c>const</c> declaration made.</param>
/// <param name="DeclaredBindings">Every realm binding this artifact declares.</param>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D30091
// Broiler-Human:        PENDING
internal sealed record JsX64UnitPlan(
    JsX64Abi Abi,
    int ScopeSlots,
    int RegionSlots,
    int[] Entries,
    int ReturnLabel,
    int PropagateLabel,
    int ThrowLabel,
    int FuelLabel,
    int[] Offsets,
    System.Collections.Generic.Dictionary<int, int> Functions,
    System.Collections.Generic.HashSet<int> ConstantBindings,
    System.Collections.Generic.HashSet<int> DeclaredBindings)
{
    /// <summary>How many bytes of the operand slab this unit's region takes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A07B1C
    // Broiler-Human:        PENDING
    internal int RegionBytes => RegionSlots * 8;

    /// <summary>The displacement from the region base of operand-stack slot <paramref name="height"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9C923E
    // Broiler-Human:        PENDING
    internal int Operand(int height) => (ScopeSlots + height) * 8;
}

/// <summary>
/// The x86-64 backend: it turns one whole numeric-manifest artifact into machine code for one of
/// the two x86-64 calling conventions.
/// </summary>
/// <remarks>
/// <para>
/// <b>THERE IS NO REGISTER ALLOCATOR, AND ITS ABSENCE IS THE LARGEST SINGLE CORRECTNESS DECISION
/// IN THIS FILE.</b> The bytecode is a stack machine and this backend compiles it as one: every
/// operand-stack slot is a fixed slot of the frame's operand slab, RAX, RDX, R10, R11, XMM0 and
/// XMM1 are scratch that never live across an instruction boundary, and exactly one callee-saved
/// register - RBX - holds the base of the current unit's region and is pushed and popped by every
/// prologue and epilogue. A register allocator is where a compiler of this shape acquires its worst
/// defects: a value live across a call in a register the convention lets the callee destroy, a
/// spill slot reused while its value is still live, a phi at a merge the allocator resolved one way
/// on one path. Every one of those produces a wrong number rather than a crash, and none of them is
/// visible to a verifier of machine code. Deleting the allocator deletes the whole class, and what
/// it costs is a load and a store per operand - which is still nothing beside an interpreter's
/// dispatch and operand decode, the two costs this form exists to remove.
/// </para>
/// <para>
/// <b>WHAT THIS BACKEND EMITS IS A CLOSED SUBSET OF THE NUMERIC MANIFEST AND WHAT IT DOES NOT IT
/// REFUSES BY NAME.</b> The manifest admits the remainder, exponentiation and bitwise operators;
/// this backend emits none of them, because each needs a conversion it cannot do exactly. A
/// remainder is a floating-point <c>fmod</c> and not a division; an exponentiation is a library
/// function; every bitwise operator needs <c>ToInt32</c>, which for an operand outside the signed
/// 64-bit range is a modular reduction the one available instruction gets wrong by answering the
/// integer-indefinite value. An approximation of any of the three would answer a wrong number for
/// an input nobody tested, which is strictly worse than a refusal a caller can read.
/// </para>
/// <para>
/// <b>THE REFUSALS ARE PART OF THE CONTRACT AND NOT A LIST OF THINGS TO FILL IN.</b> A backend that
/// grew a template for one of them without also growing the argument for why the template is exact
/// would be a backend whose output nothing can check - re-emission equality compares this emitter
/// against itself and says nothing whatever about whether the instructions are the right ones.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=35D721
// Broiler-Falsified-If: an emitted unit answers a value the interpreter does not answer for the same bytecode and the same inputs
// Broiler-Human:        PENDING
public sealed class JsX64Backend : IJsNativeBackend, IJsNativeEmitter
{
    /// <summary>The convention this instance emits for.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A02B7A
    // Broiler-Human:        PENDING
    private readonly JsX64Abi abi;

    /// <summary>Creates the backend for one calling convention.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=08D146
    // Broiler-Human:        PENDING
    public JsX64Backend(JsX64Abi convention) => abi = convention;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D51B04
    // Broiler-Human:        PENDING
    public string Name => abi.Name;

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=524EC4
    // Broiler-Human:        PENDING
    public JsNativeArchitecture Architecture => abi.Architecture;

    /// <summary>One: the first version of a backend that has emitted anything.</summary>
    /// <remarks>
    /// <b>The version travels in every artifact this backend writes, and an image refuses a payload
    /// whose version it is not.</b> A backend that changed one template without changing this
    /// number would let an image run bytes written against a contract it no longer honours, and the
    /// failure would be a wrong answer rather than a refusal. So a change to any template here is a
    /// change to this number - which also invalidates every retained artifact, and that is the
    /// point.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=B49274
    // Broiler-Falsified-If: a template in this file changes without this number changing
    // Broiler-Human:        PENDING
    public uint SemanticVersion => 1;

    /// <summary>Sixteen: the alignment every unit's entry point is written at.</summary>
    /// <remarks>
    /// It is the smallest alignment the emitted-code section's own checks admit that is also the
    /// architecture's usual instruction-fetch granularity. Nothing depends on it for correctness -
    /// x86-64 has no alignment requirement for code at all - so it is stated as a fixed number
    /// rather than derived, and the verifier holds the artifact to it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=DF71F2
    // Broiler-Human:        PENDING
    public uint CodeAlignment => 16;

    /// <summary>Emits a whole assembled program, or refuses it and says why.</summary>
    /// <remarks>
    /// <b>The constant pool is decoded HERE and the emitter is handed values</b>, so that the
    /// emitter this method calls is the same emitter a verifier calls with a pool the verifier
    /// decoded. Two decoders would be two chances to disagree, and re-emission equality is exactly
    /// a test of whether the two projections agree.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F67D3E
    // Broiler-Human:        PENDING
    public bool TryEmit(JsAssembledProgram program, out JsNativeEmission emission, out string refusal)
    {
        emission = null!;

        var values = new double[program.Constants.Count];
        var numbers = new bool[program.Constants.Count];

        for (var index = 0; index < program.Constants.Count; index++)
        {
            var entry = program.Constants[index];

            if (entry.Length == 9 && entry[0] == (byte)JsFormat.ConstantTag.Number)
            {
                values[index] = System.Buffers.Binary.BinaryPrimitives.ReadDoubleLittleEndian(
                    System.MemoryExtensions.AsSpan(entry, 1));

                numbers[index] = true;
            }
        }

        var rows = new JsFunctionRow[program.Functions.Count];

        for (var index = 0; index < rows.Length; index++)
        {
            rows[index] = program.Functions[index];
        }

        if (program.ExceptionRegions.Count != 0)
        {
            refusal =
                "this backend emits no exception region, and the numeric feature manifest admits " +
                "none, so an artifact carrying one did not come from that manifest's front end";

            return false;
        }

        var image = new JsNativeProgramImage(
            program.Code, rows, values, numbers, program.MaximumOperandStack);

        if (!TryEmit(image, out var code, out var symbols, out refusal))
        {
            return false;
        }

        emission = new JsNativeEmission(Architecture, SemanticVersion, CodeAlignment, code, symbols);
        return true;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <b>ONE REFUSED UNIT REFUSES THE WHOLE ARTIFACT, and that is this profile's non-goal written
    /// as control flow.</b> The output form is fixed when an artifact is compiled and pinned when
    /// it is verified: one executor, one form per handle. An emitter that answered "these units yes
    /// and those no" would be making the per-unit choice that paragraph refuses, so the only two
    /// answers here are every unit and none.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=86EFA0
    // Broiler-Falsified-If: an emission is produced in which some code unit has no emitted entry point
    // Broiler-Human:        PENDING
    public bool TryEmit(
        JsNativeProgramImage image,
        out byte[] code,
        out JsNativeSymbolRow[] symbols,
        out string refusal)
    {
        code = [];
        symbols = [];

        if (!TryReadBindings(image, out var functions, out var constants, out var declared, out refusal))
        {
            return false;
        }

        var assembler = new JsX64Assembler();
        var entries = new int[image.Functions.Length];

        for (var index = 0; index < entries.Length; index++)
        {
            entries[index] = assembler.Label();
        }

        var table = new JsNativeSymbolRow[image.Functions.Length];

        for (var index = 0; index < image.Functions.Length; index++)
        {
            assembler.AlignTo((int)CodeAlignment);
            table[index] = new JsNativeSymbolRow((uint)index, (uint)assembler.Position);

            if (!assembler.TryBind(entries[index], out refusal))
            {
                return false;
            }

            if (!EmitUnit(assembler, image, index, entries, functions, constants, declared, out refusal))
            {
                refusal = "code unit " + index + ": " + refusal;
                return false;
            }
        }

        if (!assembler.TryFinish(out code, out refusal))
        {
            return false;
        }

        symbols = table;
        refusal = string.Empty;
        return true;
    }

    /// <summary>Reads what the artifact's realm bindings are, before any code is emitted.</summary>
    /// <remarks>
    /// <para>
    /// <b>A FUNCTION-VALUED BINDING IS RESOLVED AT COMPILE TIME AND NEVER OCCUPIES A SLOT.</b> The
    /// lowering writes a function declaration as <c>Closure</c> followed immediately by
    /// <c>StoreGlobal</c>, and that pair is the whole of how a program of this manifest can produce
    /// a function value at all: function expressions, arrow functions and nested declarations are
    /// all refused by the manifest's admission pass. So the pairs can be collected in one linear
    /// sweep, before anything is emitted, and every call in the artifact resolves to a code unit.
    /// </para>
    /// <para>
    /// <b>A TOP-LEVEL <c>var</c> BINDING OF A NUMBER IS REFUSED HERE, and the reason is
    /// <c>undefined</c>.</b> A <c>var</c> is created when the program body is entered and holds
    /// <c>undefined</c> until its assignment runs, so a function called in between reads
    /// <c>undefined</c> and every arithmetic operation on it answers NaN. A slab of doubles has one
    /// reserved pattern for <c>undefined</c> and the emitter refuses to let it reach arithmetic, so
    /// the honest answer for a binding that can legitimately hold it before an arithmetic
    /// instruction reads it is to refuse the program. A <c>let</c> or <c>const</c> has no such
    /// window: reading one early is a <c>ReferenceError</c>, which the emitted code answers as a
    /// throw.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=FD3618
    // Broiler-Falsified-If: a binding this sweep records as function-valued is written a Number by any instruction of the artifact
    // Broiler-Human:        PENDING
    private static bool TryReadBindings(
        JsNativeProgramImage image,
        out System.Collections.Generic.Dictionary<int, int> functions,
        out System.Collections.Generic.HashSet<int> constants,
        out System.Collections.Generic.HashSet<int> declared,
        out string refusal)
    {
        functions = [];
        constants = [];
        declared = [];
        var hoisted = new System.Collections.Generic.HashSet<int>();

        foreach (var row in image.Functions)
        {
            var at = (int)row.CodeOffset;
            var end = at + (int)row.CodeLength;

            while (at < end)
            {
                var opcode = (JsOpcode)image.Code[at];

                if (!JsOpcodes.IsDefined(image.Code[at]))
                {
                    refusal = "the artifact carries an instruction this format version does not define";
                    return false;
                }

                var width = JsOpcodes.InstructionWidth(opcode);

                // ONLY THE FIVE INSTRUCTIONS THIS SWEEP CARES ABOUT HAVE THEIR OPERAND READ, and
                // the guard is not fussiness: every one of them carries a sixteen-bit operand, and
                // reading two bytes after an opcode that carries none walks off the end of the last
                // unit's code.
                var operand = JsOpcodes.Shape(opcode) == JsOperandShape.U16
                    ? (uint)(image.Code[at + 1] | (image.Code[at + 2] << 8))
                    : 0u;

                switch (opcode)
                {
                    case JsOpcode.DeclareGlobal:
                        hoisted.Add((int)operand);
                        declared.Add((int)operand);
                        break;

                    case JsOpcode.DeclareGlobalLet:
                        declared.Add((int)operand);
                        break;

                    case JsOpcode.DeclareGlobalConst:
                        declared.Add((int)operand);
                        constants.Add((int)operand);
                        break;

                    case JsOpcode.Closure when at + width < end &&
                        (JsOpcode)image.Code[at + width] == JsOpcode.StoreGlobal:
                        functions[image.Code[at + width + 1] | (image.Code[at + width + 2] << 8)] =
                            (int)operand;

                        break;
                }

                at += width;
            }
        }

        foreach (var name in hoisted)
        {
            if (!functions.ContainsKey(name))
            {
                refusal =
                    "a top-level `var` binding of a Number is refused: it holds `undefined` " +
                    "between the program body's entry and its assignment, and a slab of doubles " +
                    "cannot carry that through an arithmetic instruction without answering a " +
                    "number the interpreter would not";

                return false;
            }
        }

        refusal = string.Empty;
        return true;
    }

    /// <summary>Emits one code unit: a prologue, every reachable instruction, and four epilogues.</summary>
    /// <remarks>
    /// <para>
    /// <b>THE PROLOGUE IS THE SAME FOR EVERY UNIT, WHICH IS WHY THE ENTRY POINT AND A CALLEE ARE
    /// THE SAME ADDRESS.</b> A unit does not receive its region base from its caller: it reads the
    /// frame's operand-slab pointer, which is a BUMP POINTER that every prologue advances past its
    /// own region and every epilogue restores. The outermost call finds it at the slab's base, a
    /// nested call finds it just past its caller's region, and the same three instructions do both
    /// - so there is no separate outer thunk, no second symbol per unit, and no way for a caller
    /// and a callee to disagree about where a frame starts.
    /// </para>
    /// <para>
    /// <b>THE SLAB IS ALSO THE RECURSION BOUND, and that is deliberate rather than incidental.</b>
    /// The prologue subtracts its region from the frame's remaining-slot count and answers an
    /// exhaustion when the count goes negative, so a program that recurses without end runs out of
    /// operand slab and stops. Bounding recursion by the machine stack instead would mean bounding
    /// it by a resource this component cannot measure, cannot charge for and cannot survive
    /// exhausting.
    /// </para>
    /// <para>
    /// <b>Fuel is decremented once per unit entry and once per back edge.</b> That is coarser than
    /// the interpreter's charge per instruction and it is the accounting an emitted form can
    /// actually do: a managed meter cannot be called per instruction from machine code. THE
    /// CONSEQUENCE IS THAT THE TWO FORMS EXHAUST AT DIFFERENT POINTS FOR THE SAME PROGRAM, which is
    /// a real difference between the forms and is stated wherever a corpus pins an exhaustion.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=9A32C1
    // Broiler-Falsified-If: a path out of an emitted unit leaves the frame's slab pointer or remaining-slot count other than it found them
    // Broiler-Human:        PENDING
    private bool EmitUnit(
        JsX64Assembler assembler,
        JsNativeProgramImage image,
        int index,
        int[] entries,
        System.Collections.Generic.Dictionary<int, int> functions,
        System.Collections.Generic.HashSet<int> constants,
        System.Collections.Generic.HashSet<int> declared,
        out string refusal)
    {
        var row = image.Functions[index];

        if ((row.Flags & (uint)(JsFormat.FunctionFlags.Generator |
            JsFormat.FunctionFlags.Async |
            JsFormat.FunctionFlags.ClassConstructor |
            JsFormat.FunctionFlags.DerivedConstructor |
            JsFormat.FunctionFlags.BindsParameters |
            JsFormat.FunctionFlags.UsesArguments)) != 0)
        {
            refusal = "the unit carries a flag this backend has no prologue for";
            return false;
        }

        var walk = new JsX64Walk(image, row) { Functions = functions };

        if (!walk.TryTrace(out refusal))
        {
            return false;
        }

        if (row.ParameterCount > row.ScopeSlots)
        {
            refusal = "the unit declares more parameters than its own environment has slots";
            return false;
        }

        var offsets = new int[row.CodeLength];

        for (var slot = 0; slot < offsets.Length; slot++)
        {
            offsets[slot] = assembler.Label();
        }

        var plan = new JsX64UnitPlan(
            abi,
            walk.ScopeSlots,
            walk.ScopeSlots + (int)row.MaxOperandStack,
            entries,
            assembler.Label(),
            assembler.Label(),
            assembler.Label(),
            assembler.Label(),
            offsets,
            functions,
            constants,
            declared);

        var overflow = assembler.Label();
        EmitPrologue(assembler, plan, overflow);

        var start = (int)row.CodeOffset;

        for (var at = start; at < start + (int)row.CodeLength;)
        {
            var opcode = (JsOpcode)image.Code[at];
            var width = JsOpcodes.InstructionWidth(opcode);

            if (!walk.IsReachable(at))
            {
                at += width;
                continue;
            }

            if (!assembler.TryBind(offsets[at - start], out refusal))
            {
                return false;
            }

            if (!EmitInstruction(assembler, image, walk, plan, row, at, out refusal))
            {
                refusal = "bytecode offset " + at + ": " + refusal;
                return false;
            }

            at += width;
        }

        return EmitEpilogues(assembler, plan, overflow, out refusal);
    }

    /// <summary>Emits the uniform prologue.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=DA1CCC
    // Broiler-Falsified-If: the prologue advances the slab pointer before it has checked that the region fits
    // Broiler-Human:        PENDING
    private static void EmitPrologue(JsX64Assembler assembler, JsX64UnitPlan plan, int overflow)
    {
        // ONE CALLEE-SAVED REGISTER, PUSHED HERE AND POPPED IN EVERY EPILOGUE. RBX is callee-saved
        // under both conventions, so the caller - which for the outermost call is the CLR - gets it
        // back unchanged. It is the only one this backend touches; RDI and RSI, which the two
        // conventions disagree about, are never written at all.
        assembler.Push(JsX64Register.Rbx);
        assembler.SubRegisterImmediate32(JsX64Register.Rsp, plan.Abi.FrameBytes);

        assembler.MovMemoryRegister(
            JsX64Register.Rsp, plan.Abi.FramePointerSlot, plan.Abi.FramePointerRegister);

        assembler.MovRegisterRegister(JsX64Register.R11, plan.Abi.FramePointerRegister);

        // THE CHECK COMES BEFORE THE BUMP so that the overflow epilogue has exactly one thing to
        // undo. A prologue that bumped first would have to unwind two edits on the path it is least
        // likely to be tested on.
        assembler.SubMemory32Immediate32(
            JsX64Register.R11, JsX64Frame.OperandCountOffset, plan.RegionSlots);

        assembler.JumpIf(JsX64Condition.Less, overflow);

        assembler.MovRegisterMemory(
            JsX64Register.Rbx, JsX64Register.R11, JsX64Frame.OperandsOffset);

        assembler.LeaRegisterMemory(JsX64Register.Rax, JsX64Register.Rbx, plan.RegionBytes);

        assembler.MovMemoryRegister(
            JsX64Register.R11, JsX64Frame.OperandsOffset, JsX64Register.Rax);

        EmitFuelCharge(assembler, plan);
    }

    /// <summary>Decrements the fuel counter and branches to the exhaustion epilogue.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=BF26B8
    // Broiler-Falsified-If: a unit entry or a back edge is emitted without this sequence
    // Broiler-Human:        PENDING
    private static void EmitFuelCharge(JsX64Assembler assembler, JsX64UnitPlan plan)
    {
        assembler.MovRegisterMemory(
            JsX64Register.R11, JsX64Register.Rsp, plan.Abi.FramePointerSlot);

        assembler.MovRegisterMemory(JsX64Register.Rax, JsX64Register.R11, JsX64Frame.FuelOffset);
        assembler.DecrementMemory64(JsX64Register.Rax, 0);
        assembler.JumpIf(JsX64Condition.Equal, plan.FuelLabel);
        assembler.JumpIf(JsX64Condition.Less, plan.FuelLabel);
    }

    /// <summary>Emits the four ways out of a unit.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=9BCE24
    // Broiler-Falsified-If: any exit restores the stack pointer or the callee-saved register differently from the others
    // Broiler-Human:        PENDING
    private static bool EmitEpilogues(
        JsX64Assembler assembler, JsX64UnitPlan plan, int overflow, out string refusal)
    {
        // THE NORMAL RETURN, WITH THE VALUE IN XMM0. It is also written to the region's first slot,
        // which for the OUTERMOST unit is the frame's operand slot zero - the slot the frame's
        // contract says a normal return leaves its value in. For a nested unit that slot is inside
        // the region the epilogue has just given back, so the store is to storage nothing reads,
        // and the caller takes the value out of XMM0 where it also is.
        if (!assembler.TryBind(plan.ReturnLabel, out refusal))
        {
            return false;
        }

        assembler.MovRegisterMemory(
            JsX64Register.R11, JsX64Register.Rsp, plan.Abi.FramePointerSlot);

        assembler.MovMemoryRegister(
            JsX64Register.R11, JsX64Frame.OperandsOffset, JsX64Register.Rbx);

        assembler.MovsdStore(JsX64Register.Rbx, 0, 0);

        assembler.AddMemory32Immediate32(
            JsX64Register.R11, JsX64Frame.OperandCountOffset, plan.RegionSlots);

        assembler.XorRegisterRegister(JsX64Register.Rax, JsX64Register.Rax);
        assembler.AddRegisterImmediate32(JsX64Register.Rsp, plan.Abi.FrameBytes);
        assembler.Pop(JsX64Register.Rbx);
        assembler.Ret();

        if (!assembler.TryBind(plan.ThrowLabel, out refusal))
        {
            return false;
        }

        assembler.MovRegisterImmediate64(JsX64Register.Rax, (int)JsNativeReturn.Threw);
        assembler.Jump(plan.PropagateLabel);

        if (!assembler.TryBind(plan.FuelLabel, out refusal))
        {
            return false;
        }

        assembler.MovRegisterImmediate64(JsX64Register.Rax, (int)JsNativeReturn.FuelExhausted);

        if (!assembler.TryBind(plan.PropagateLabel, out refusal))
        {
            return false;
        }

        assembler.MovRegisterMemory(
            JsX64Register.R11, JsX64Register.Rsp, plan.Abi.FramePointerSlot);

        assembler.MovMemoryRegister(
            JsX64Register.R11, JsX64Frame.OperandsOffset, JsX64Register.Rbx);

        assembler.AddMemory32Immediate32(
            JsX64Register.R11, JsX64Frame.OperandCountOffset, plan.RegionSlots);

        assembler.AddRegisterImmediate32(JsX64Register.Rsp, plan.Abi.FrameBytes);
        assembler.Pop(JsX64Register.Rbx);
        assembler.Ret();

        // THE OVERFLOW EXIT IS THE ONLY ONE THAT DOES NOT RESTORE THE SLAB POINTER, because it is
        // reached before the prologue advanced it. It gives back exactly the one thing the prologue
        // had already taken, which is the count.
        if (!assembler.TryBind(overflow, out refusal))
        {
            return false;
        }

        assembler.MovRegisterMemory(
            JsX64Register.R11, JsX64Register.Rsp, plan.Abi.FramePointerSlot);

        assembler.AddMemory32Immediate32(
            JsX64Register.R11, JsX64Frame.OperandCountOffset, plan.RegionSlots);

        assembler.MovRegisterImmediate64(JsX64Register.Rax, (int)JsNativeReturn.FuelExhausted);
        assembler.AddRegisterImmediate32(JsX64Register.Rsp, plan.Abi.FrameBytes);
        assembler.Pop(JsX64Register.Rbx);
        assembler.Ret();

        refusal = string.Empty;
        return true;
    }

    /// <summary>Emits the template for one bytecode instruction, or refuses it by name.</summary>
    /// <remarks>
    /// <b>EVERY TEMPLATE IS A FIXED SEQUENCE AND NOTHING HERE LOOKS AT WHAT CAME BEFORE.</b> One
    /// bytecode instruction becomes the same instructions every time it appears, differing only in
    /// the displacements the walk resolved. That is what makes the output a pure function of the
    /// input, which is what makes re-emission equality possible; and it is what makes a template
    /// readable against the architecture manual one line at a time rather than as the output of an
    /// optimiser.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=EB7312
    // Broiler-Falsified-If: a template here computes a different value from the interpreter's case for the same instruction
    // Broiler-Human:        PENDING
    private bool EmitInstruction(
        JsX64Assembler assembler,
        JsNativeProgramImage image,
        JsX64Walk walk,
        JsX64UnitPlan plan,
        JsFunctionRow row,
        int at,
        out string refusal)
    {
        refusal = string.Empty;
        var opcode = (JsOpcode)image.Code[at];
        var operand = (int)walk.Operand(at, opcode);
        var stack = walk.StackAt(at);
        var height = stack.Length;
        var start = (int)row.CodeOffset;

        switch (opcode)
        {
            case JsOpcode.LoadUndefined:
                assembler.MovRegisterImmediate64(JsX64Register.Rax, JsNativeValues.UndefinedBits);
                assembler.MovMemoryRegister(JsX64Register.Rbx, plan.Operand(height), JsX64Register.Rax);
                return true;

            case JsOpcode.LoadConstant:
                if (operand >= image.ConstantIsNumber.Length || !image.ConstantIsNumber[operand])
                {
                    refusal = "a constant this manifest's emitted form has no representation for is loaded";
                    return false;
                }

                assembler.MovRegisterMemory(
                    JsX64Register.R11, JsX64Register.Rsp, plan.Abi.FramePointerSlot);

                assembler.MovRegisterMemory(
                    JsX64Register.Rax, JsX64Register.R11, JsX64Frame.ConstantsOffset);

                assembler.MovRegisterMemory(JsX64Register.Rdx, JsX64Register.Rax, operand * 8);
                assembler.MovMemoryRegister(JsX64Register.Rbx, plan.Operand(height), JsX64Register.Rdx);
                return true;

            case JsOpcode.LoadScoped:
                if (!walk.TryResolveSlot(at, walk.Depth(at), operand, out var read, out refusal))
                {
                    return false;
                }

                assembler.MovRegisterMemory(JsX64Register.Rdx, JsX64Register.Rbx, read * 8);
                assembler.MovMemoryRegister(JsX64Register.Rbx, plan.Operand(height), JsX64Register.Rdx);
                return true;

            case JsOpcode.StoreScoped:
            case JsOpcode.InitialiseScoped:
                if (!walk.TryResolveSlot(at, walk.Depth(at), operand, out var written, out refusal))
                {
                    return false;
                }

                if (stack[^1].Kind == JsX64ValueKind.Boolean)
                {
                    refusal = BooleanRefusal;
                    return false;
                }

                assembler.MovRegisterMemory(
                    JsX64Register.Rdx, JsX64Register.Rbx, plan.Operand(height - 1));

                assembler.MovMemoryRegister(JsX64Register.Rbx, written * 8, JsX64Register.Rdx);
                return true;

            case JsOpcode.PushScope:
            case JsOpcode.PopScope:
            case JsOpcode.CopyScope:
            case JsOpcode.DeclareGlobal:
            case JsOpcode.DeclareGlobalLet:
            case JsOpcode.DeclareGlobalConst:
            case JsOpcode.Pop:
                // NOTHING IS EMITTED AND EACH OF THE SEVEN HAS ITS OWN REASON. A scope push or pop
                // is a change to a shape the walk resolved statically; a scope copy exists so a
                // closure can capture a per-iteration binding and this manifest admits no closure
                // that can; a declaration reserves a slab slot the executor already filled with the
                // uninitialised pattern; and a pop of an operand-stack slot is a slot nothing will
                // read again.
                return true;

            case JsOpcode.Closure:
                // A function value never occupies a slot: the walk carries which code unit it is,
                // and the call that consumes it becomes a direct branch to that unit.
                return true;

            case JsOpcode.Duplicate:
                assembler.MovRegisterMemory(
                    JsX64Register.Rdx, JsX64Register.Rbx, plan.Operand(height - 1));

                assembler.MovMemoryRegister(JsX64Register.Rbx, plan.Operand(height), JsX64Register.Rdx);
                return true;

            case JsOpcode.LoadGlobal:
                return EmitLoadGlobal(assembler, plan, stack, height, operand, out refusal);

            case JsOpcode.StoreGlobal:
            case JsOpcode.InitialiseGlobalLexical:
                return EmitStoreGlobal(assembler, plan, stack, height, operand, opcode, out refusal);

            case JsOpcode.Add:
            case JsOpcode.Subtract:
            case JsOpcode.Multiply:
            case JsOpcode.Divide:
                return EmitArithmetic(assembler, plan, stack, height, opcode, out refusal);

            case JsOpcode.Negate:
                if (stack[^1].Kind != JsX64ValueKind.Number)
                {
                    refusal = BooleanRefusal;
                    return false;
                }

                assembler.MovRegisterMemory(
                    JsX64Register.Rdx, JsX64Register.Rbx, plan.Operand(height - 1));

                assembler.MovRegisterImmediate64(JsX64Register.R10, long.MinValue);
                assembler.XorRegisterRegister(JsX64Register.Rdx, JsX64Register.R10);

                assembler.MovMemoryRegister(
                    JsX64Register.Rbx, plan.Operand(height - 1), JsX64Register.Rdx);

                return true;

            case JsOpcode.ToNumber:
                // ToNumber of a Number is the identity, and this manifest admits no operand that is
                // not one. A template that emitted a conversion would be emitting a no-op with a
                // rounding mode.
                if (stack[^1].Kind != JsX64ValueKind.Number)
                {
                    refusal = BooleanRefusal;
                    return false;
                }

                return true;

            case JsOpcode.Not:
                return EmitNot(assembler, plan, height);

            case JsOpcode.LessThan:
            case JsOpcode.LessThanOrEqual:
            case JsOpcode.GreaterThan:
            case JsOpcode.GreaterThanOrEqual:
            case JsOpcode.StrictEquals:
            case JsOpcode.StrictNotEquals:
            case JsOpcode.LooseEquals:
            case JsOpcode.LooseNotEquals:
                return EmitComparison(assembler, plan, stack, height, opcode, out refusal);

            case JsOpcode.Jump:
                if (operand <= at)
                {
                    EmitFuelCharge(assembler, plan);
                }

                assembler.Jump(plan.Offsets[operand - start]);
                return true;

            case JsOpcode.JumpIfFalse:
            case JsOpcode.JumpIfTrue:
                return EmitBranch(assembler, plan, height, at, start, operand, opcode);

            case JsOpcode.Call:
                return EmitCall(assembler, image, plan, stack, height, operand, out refusal);

            case JsOpcode.Return:
                if (stack[^1].Kind == JsX64ValueKind.Boolean)
                {
                    refusal = BooleanRefusal;
                    return false;
                }

                assembler.MovsdLoad(0, JsX64Register.Rbx, plan.Operand(height - 1));
                assembler.Jump(plan.ReturnLabel);
                return true;

            case JsOpcode.ReturnUndefined:
                assembler.MovRegisterImmediate64(JsX64Register.Rax, JsNativeValues.UndefinedBits);
                assembler.MovqToXmm(0, JsX64Register.Rax);
                assembler.Jump(plan.ReturnLabel);
                return true;

            default:
                refusal = Why(opcode);
                return false;
        }
    }

    /// <summary>The sentence a Boolean that tried to leave its branch is refused with.</summary>
    /// <remarks>
    /// <b>It is one sentence in one place because the refusal is one rule.</b> A comparison's
    /// result is a Boolean, a slab of doubles stores Booleans as one and zero, and a Boolean that
    /// reached a binding would come back indistinguishable from the Number - so
    /// <c>(1 &lt; 2) === 1</c> would answer true where the language says false. Consuming the
    /// result in the branch that follows it is the only shape this backend can emit exactly.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=571BD0
    // Broiler-Human:        PENDING
    private const string BooleanRefusal =
        "this backend consumes a comparison or a logical negation only in the branch that " +
        "immediately follows it: a Boolean stored in a slab of doubles is indistinguishable from " +
        "the Number one or zero, and the language distinguishes them";

    /// <summary>Emits a read of a realm binding, with its temporal-dead-zone check.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=64963B
    // Broiler-Falsified-If: a read of an uninitialised realm binding answers a value rather than a throw
    // Broiler-Human:        PENDING
    private static bool EmitLoadGlobal(
        JsX64Assembler assembler,
        JsX64UnitPlan plan,
        JsX64Value[] stack,
        int height,
        int name,
        out string refusal)
    {
        refusal = string.Empty;

        if (plan.Functions.ContainsKey(name))
        {
            // A function-valued binding is resolved by the walk and occupies no slot.
            return true;
        }

        if (!plan.DeclaredBindings.Contains(name))
        {
            refusal =
                "a realm binding this artifact does not declare is read, so its value would come " +
                "from the realm - which is an object graph an emitted frame carries no reference to";

            return false;
        }

        assembler.MovRegisterMemory(
            JsX64Register.R11, JsX64Register.Rsp, plan.Abi.FramePointerSlot);

        assembler.MovRegisterMemory(JsX64Register.Rax, JsX64Register.R11, JsX64Frame.LocalsOffset);
        assembler.MovRegisterMemory(JsX64Register.Rdx, JsX64Register.Rax, name * 8);

        assembler.MovRegisterImmediate64(JsX64Register.R10, JsNativeValues.UninitialisedBits);
        assembler.CmpRegisterRegister(JsX64Register.Rdx, JsX64Register.R10);
        assembler.JumpIf(JsX64Condition.Equal, plan.ThrowLabel);

        assembler.MovMemoryRegister(JsX64Register.Rbx, plan.Operand(height), JsX64Register.Rdx);
        return true;
    }

    /// <summary>Emits a write to a realm binding.</summary>
    /// <remarks>
    /// <b>An initialisation writes without a check and an assignment checks first</b>, because a
    /// <c>let</c> is initialised exactly once by the instruction that ends its dead zone and
    /// assigned any number of times after it. Checking on the initialisation would refuse every
    /// binding the moment it was created.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=BDACE8
    // Broiler-Falsified-If: an assignment to a `const` realm binding is emitted rather than refused
    // Broiler-Human:        PENDING
    private static bool EmitStoreGlobal(
        JsX64Assembler assembler,
        JsX64UnitPlan plan,
        JsX64Value[] stack,
        int height,
        int name,
        JsOpcode opcode,
        out string refusal)
    {
        refusal = string.Empty;

        if (stack[^1].Kind == JsX64ValueKind.Function)
        {
            // The pair `Closure` then `StoreGlobal` is a function declaration, and the walk has
            // already recorded which unit it names. Nothing is stored.
            return true;
        }

        if (stack[^1].Kind == JsX64ValueKind.Boolean)
        {
            refusal = BooleanRefusal;
            return false;
        }

        if (plan.Functions.ContainsKey(name))
        {
            refusal =
                "a realm binding this artifact declared as a function is assigned a Number, so " +
                "one name would hold two kinds of value and a call through it could not be a " +
                "direct branch";

            return false;
        }

        if (!plan.DeclaredBindings.Contains(name))
        {
            refusal = "a realm binding this artifact does not declare is assigned";
            return false;
        }

        if (opcode == JsOpcode.StoreGlobal && plan.ConstantBindings.Contains(name))
        {
            refusal =
                "an assignment to a `const` realm binding is a TypeError this backend emits no " +
                "template for";

            return false;
        }

        assembler.MovRegisterMemory(
            JsX64Register.R11, JsX64Register.Rsp, plan.Abi.FramePointerSlot);

        assembler.MovRegisterMemory(JsX64Register.Rax, JsX64Register.R11, JsX64Frame.LocalsOffset);

        if (opcode == JsOpcode.StoreGlobal)
        {
            assembler.MovRegisterMemory(JsX64Register.Rdx, JsX64Register.Rax, name * 8);
            assembler.MovRegisterImmediate64(JsX64Register.R10, JsNativeValues.UninitialisedBits);
            assembler.CmpRegisterRegister(JsX64Register.Rdx, JsX64Register.R10);
            assembler.JumpIf(JsX64Condition.Equal, plan.ThrowLabel);
        }

        assembler.MovRegisterMemory(
            JsX64Register.Rdx, JsX64Register.Rbx, plan.Operand(height - 1));

        assembler.MovMemoryRegister(JsX64Register.Rax, name * 8, JsX64Register.Rdx);
        return true;
    }

    /// <summary>Emits one binary arithmetic instruction.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=2D960A
    // Broiler-Falsified-If: an operand that is not a Number reaches one of these instructions
    // Broiler-Human:        PENDING
    private static bool EmitArithmetic(
        JsX64Assembler assembler,
        JsX64UnitPlan plan,
        JsX64Value[] stack,
        int height,
        JsOpcode opcode,
        out string refusal)
    {
        refusal = string.Empty;

        if (stack[^1].Kind != JsX64ValueKind.Number || stack[^2].Kind != JsX64ValueKind.Number)
        {
            refusal =
                "an operand of this arithmetic instruction is not a Number, and this backend " +
                "emits no conversion: " + BooleanRefusal;

            return false;
        }

        assembler.MovsdLoad(0, JsX64Register.Rbx, plan.Operand(height - 2));
        assembler.MovsdLoad(1, JsX64Register.Rbx, plan.Operand(height - 1));

        switch (opcode)
        {
            case JsOpcode.Add:
                assembler.Addsd(0, 1);
                break;

            case JsOpcode.Subtract:
                assembler.Subsd(0, 1);
                break;

            case JsOpcode.Multiply:
                assembler.Mulsd(0, 1);
                break;

            default:
                assembler.Divsd(0, 1);
                break;
        }

        assembler.MovsdStore(JsX64Register.Rbx, plan.Operand(height - 2), 0);
        return true;
    }

    /// <summary>Emits a logical negation, which is the truthiness test with its answer inverted.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=455D61
    // Broiler-Falsified-If: this answers false for NaN or for negative zero
    // Broiler-Human:        PENDING
    private static bool EmitNot(JsX64Assembler assembler, JsX64UnitPlan plan, int height)
    {
        // FALSY IS ZERO OR NaN, AND BOTH ARE READ OFF ONE COMPARISON. `ucomisd` against zero sets
        // the zero flag for either sign of zero and the parity flag for NaN, so the two conditions
        // are the two flags and their disjunction is the answer.
        assembler.MovsdLoad(0, JsX64Register.Rbx, plan.Operand(height - 1));
        assembler.Xorpd(1, 1);
        assembler.Ucomisd(0, 1);
        assembler.SetCondition(JsX64Condition.Equal, JsX64Register.Rax);
        assembler.SetCondition(JsX64Condition.Parity, JsX64Register.Rdx);
        assembler.Or8(JsX64Register.Rax, JsX64Register.Rdx);
        assembler.MovZeroExtend8To32(JsX64Register.Rax, JsX64Register.Rax);
        assembler.Cvtsi2sd(0, JsX64Register.Rax);
        assembler.MovsdStore(JsX64Register.Rbx, plan.Operand(height - 1), 0);
        return true;
    }

    /// <summary>Emits one comparison, whose result is a Boolean the next branch consumes.</summary>
    /// <remarks>
    /// <b>THE OPERANDS ARE COMPARED IN THE OTHER ORDER FOR THE TWO "LESS" FORMS, and that is not a
    /// transcription error.</b> After <c>ucomisd</c> an unordered comparison sets the carry flag,
    /// so the unsigned "above" conditions answer false for a NaN operand and the "below" ones
    /// answer true. JavaScript says every relational comparison with a NaN operand is false, so
    /// every one of the four is written as an "above" test with the operands arranged to suit -
    /// which is the standard way and is the only way that gets NaN right without a second branch.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=86F63E
    // Broiler-Falsified-If: a relational comparison with a NaN operand answers true
    // Broiler-Human:        PENDING
    private static bool EmitComparison(
        JsX64Assembler assembler,
        JsX64UnitPlan plan,
        JsX64Value[] stack,
        int height,
        JsOpcode opcode,
        out string refusal)
    {
        refusal = string.Empty;

        if (stack[^1].Kind != JsX64ValueKind.Number || stack[^2].Kind != JsX64ValueKind.Number)
        {
            refusal =
                "an operand of this comparison is not a Number, and this backend emits no " +
                "conversion: " + BooleanRefusal;

            return false;
        }

        assembler.MovsdLoad(0, JsX64Register.Rbx, plan.Operand(height - 2));
        assembler.MovsdLoad(1, JsX64Register.Rbx, plan.Operand(height - 1));

        switch (opcode)
        {
            case JsOpcode.LessThan:
                assembler.Ucomisd(1, 0);
                assembler.SetCondition(JsX64Condition.Above, JsX64Register.Rax);
                break;

            case JsOpcode.LessThanOrEqual:
                assembler.Ucomisd(1, 0);
                assembler.SetCondition(JsX64Condition.AboveOrEqual, JsX64Register.Rax);
                break;

            case JsOpcode.GreaterThan:
                assembler.Ucomisd(0, 1);
                assembler.SetCondition(JsX64Condition.Above, JsX64Register.Rax);
                break;

            case JsOpcode.GreaterThanOrEqual:
                assembler.Ucomisd(0, 1);
                assembler.SetCondition(JsX64Condition.AboveOrEqual, JsX64Register.Rax);
                break;

            case JsOpcode.StrictEquals:
            case JsOpcode.LooseEquals:
                // EQUALITY NEEDS BOTH FLAGS. An unordered comparison sets the zero flag as well as
                // the parity flag, so `sete` alone would answer that NaN equals NaN.
                assembler.Ucomisd(0, 1);
                assembler.SetCondition(JsX64Condition.Equal, JsX64Register.Rax);
                assembler.SetCondition(JsX64Condition.NoParity, JsX64Register.Rdx);
                assembler.And8(JsX64Register.Rax, JsX64Register.Rdx);
                break;

            default:
                assembler.Ucomisd(0, 1);
                assembler.SetCondition(JsX64Condition.NotEqual, JsX64Register.Rax);
                assembler.SetCondition(JsX64Condition.Parity, JsX64Register.Rdx);
                assembler.Or8(JsX64Register.Rax, JsX64Register.Rdx);
                break;
        }

        assembler.MovZeroExtend8To32(JsX64Register.Rax, JsX64Register.Rax);
        assembler.Cvtsi2sd(0, JsX64Register.Rax);
        assembler.MovsdStore(JsX64Register.Rbx, plan.Operand(height - 2), 0);
        return true;
    }

    /// <summary>Emits a conditional branch, charging fuel first when it is a back edge.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=483910
    // Broiler-Falsified-If: a branch to an offset at or before this one is emitted without a fuel charge
    // Broiler-Human:        PENDING
    private static bool EmitBranch(
        JsX64Assembler assembler,
        JsX64UnitPlan plan,
        int height,
        int at,
        int start,
        int target,
        JsOpcode opcode)
    {
        if (target <= at)
        {
            EmitFuelCharge(assembler, plan);
        }

        assembler.MovsdLoad(0, JsX64Register.Rbx, plan.Operand(height - 1));
        assembler.Xorpd(1, 1);
        assembler.Ucomisd(0, 1);

        if (opcode == JsOpcode.JumpIfFalse)
        {
            assembler.JumpIf(JsX64Condition.Parity, plan.Offsets[target - start]);
            assembler.JumpIf(JsX64Condition.Equal, plan.Offsets[target - start]);
            return true;
        }

        var skip = assembler.Label();
        assembler.JumpIf(JsX64Condition.Parity, skip);
        assembler.JumpIf(JsX64Condition.Equal, skip);
        assembler.Jump(plan.Offsets[target - start]);
        return assembler.TryBind(skip, out _);
    }

    /// <summary>Emits a call: the arguments into the callee's slots, then a direct branch.</summary>
    /// <remarks>
    /// <b>THE ARGUMENT COUNT MUST MATCH THE CALLEE'S PARAMETER COUNT EXACTLY, and a mismatch is
    /// refused rather than padded.</b> JavaScript pads a short call with <c>undefined</c> and drops
    /// a long one's extra arguments; padding with the reserved <c>undefined</c> pattern would put a
    /// value into a parameter slot that the arithmetic in the body is then refused from touching,
    /// which is a refusal arriving in the wrong place with the wrong sentence. Refusing the call is
    /// the same fact said where an author can act on it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=A494F3
    // Broiler-Falsified-If: a call is emitted to a unit whose parameter count differs from the argument count
    // Broiler-Human:        PENDING
    private static bool EmitCall(
        JsX64Assembler assembler,
        JsNativeProgramImage image,
        JsX64UnitPlan plan,
        JsX64Value[] stack,
        int height,
        int arguments,
        out string refusal)
    {
        refusal = string.Empty;
        var callee = stack[height - arguments - 2];

        if (callee.Kind != JsX64ValueKind.Function)
        {
            refusal =
                "the callee of this call is not a function this artifact declares, so the call " +
                "could not be a direct branch to a code unit";

            return false;
        }

        var row = image.Functions[callee.Unit];

        if (row.ParameterCount != (uint)arguments)
        {
            refusal =
                "a call passes " + arguments + " arguments to a code unit declaring " +
                row.ParameterCount + " parameters, and this backend pads neither";

            return false;
        }

        for (var index = 0; index < arguments; index++)
        {
            if (stack[height - arguments + index].Kind != JsX64ValueKind.Number)
            {
                refusal = "an argument of this call is not a Number: " + BooleanRefusal;
                return false;
            }

            assembler.MovRegisterMemory(
                JsX64Register.Rdx, JsX64Register.Rbx, plan.Operand(height - arguments + index));

            // THE CALLEE'S PARAMETERS ARE ITS OWN SCOPE SLOTS ZERO UPWARDS, and its region begins
            // exactly where this one ends - which is the same address the callee's prologue will
            // read out of the frame's slab pointer, because this unit's prologue put it there.
            assembler.MovMemoryRegister(
                JsX64Register.Rbx, plan.RegionBytes + (index * 8), JsX64Register.Rdx);
        }

        assembler.MovRegisterMemory(
            plan.Abi.FramePointerRegister, JsX64Register.Rsp, plan.Abi.FramePointerSlot);

        assembler.Call(plan.Entries[callee.Unit]);

        assembler.TestRegister32(JsX64Register.Rax, JsX64Register.Rax);
        assembler.JumpIf(JsX64Condition.NotEqual, plan.PropagateLabel);

        assembler.MovsdStore(JsX64Register.Rbx, plan.Operand(height - arguments - 2), 0);
        return true;
    }

    /// <summary>Why one instruction is refused, named rather than described as unsupported.</summary>
    /// <remarks>
    /// <b>EACH SENTENCE NAMES THE THING THAT WOULD HAVE TO BE TRUE FOR A TEMPLATE TO EXIST.</b> A
    /// message that said "unsupported" would tell an author nothing about whether to rewrite their
    /// program or to wait for a version, and would tell a later reader of this file nothing about
    /// what the missing template would have to do.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=448932
    // Broiler-Human:        PENDING
    private static string Why(JsOpcode opcode) => opcode switch
    {
        JsOpcode.Remainder =>
            "the remainder operator is a floating-point remainder and not a division, and this " +
            "backend emits no template for it: the x87 instruction that computes one is not " +
            "reachable from the SSE2 register file this backend uses, and a division followed by " +
            "a truncation is exact only while the quotient fits a signed 64-bit integer",

        JsOpcode.Exponent =>
            "the exponentiation operator is a library function and this backend calls nothing " +
            "outside its own emitted blob",

        JsOpcode.BitwiseNot or JsOpcode.BitwiseOr or JsOpcode.BitwiseAnd or JsOpcode.BitwiseXor or
        JsOpcode.ShiftLeft or JsOpcode.ShiftRight or JsOpcode.ShiftRightUnsigned =>
            "every bitwise operator converts its operands with ToInt32, which is a modular " +
            "reduction; the one instruction that converts a double to an integer answers the " +
            "integer-indefinite value for an operand outside the signed 64-bit range rather than " +
            "the reduction the language specifies, and a template that ignored that would answer " +
            "a wrong number for an input nobody tested",

        _ =>
            "this backend emits no template for the instruction `" + opcode + "`",
    };
}
