// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   46
// Annotated:        46/46
// Exempt:           30
// Human-reviewed:   0/46
// IP risk:          Low
// Security risk:    High
// Criteria:         19/17
// Resource impact:  2/10 max
// Unverified:       46
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The baseline form's block partition and unit layout, stated once: which instructions run alone,
/// where a block step stops, the plan of a unit's heads and tails, and the templates between a unit's
/// prologue and its epilogue.
/// </summary>
/// <remarks>
/// <para>
/// <b>IT IS IN THE FORMAT ASSEMBLY BECAUSE THE EMITTER AND THE ENGINE BOTH REFERENCE THIS ASSEMBLY AND
/// NEITHER MAY REFERENCE THE OTHER.</b> The emitter is in the lowering and the step is in the profile;
/// the profile may not reference its lowering (rule N1) and the lowering does not reference the
/// profile, so a partition stated on either side could not be the one the other side reads, and one
/// restated on both would be two partitions that could drift. It is public because no product assembly
/// grants another its internals.
/// </para>
/// <para>
/// <b>A BLOCK IS THE MAXIMAL LINEAR RUN FROM A HEAD, AND TWO BLOCKS MAY SHARE A SUFFIX.</b> A head inside
/// another head's run - a loop head reached linearly from the code before it - has a block of its own,
/// and the run that passes through it does not stop there. That is what keeps <see cref="StopsAfter"/>
/// a pure function of the instruction just run, the next program counter, the byte there and the
/// unit's end, with no per-unit set of heads to consult while a step runs.
/// </para>
/// <para>
/// <b>WHAT IT ALLOCATES IS SIZED BY WHAT IT HOLDS, AND NOTHING GROWS.</b> The handler offsets of an image
/// are grouped by unit once, into arrays of exactly their count; a plan holds one mark per byte of the
/// unit's code while it is built, and answers one entry per landing and one block per head; a layout is
/// one array of exactly <see cref="LayoutLength"/> entries, whose branches to heads are found through the
/// plan's blocks. No list here grows by doubling, and planning a unit reads no exception region at all:
/// its handler offsets are handed to it. The layout's entries come out of one walk, in order and each
/// final when it is made; <see cref="Layout"/> writes them into its array, and the template scan takes
/// them one at a time and stops at the first that differs from the payload, so the scan never holds a
/// layout and makes no more of one than the payload's instructions can match.
/// </para>
/// <para>
/// <b>NOTHING HERE DECIDES WHAT AN INSTRUCTION DOES.</b> The partition says where a step would return to
/// emitted code and what that code compares next; every instruction is still charged, run, filtered and
/// landed by the interpreter's own arm, whichever step runs it.
/// </para>
/// <para>
/// <b>THE BASELINE EMITTER ENCODES THE LAYOUT, A BLOCK STEP STOPS WHERE <see cref="StopsAfter"/> HOLDS,
/// AND THE TEMPLATE SCAN HOLDS EVERY x86-64 BASELINE PAYLOAD TO THE LAYOUT IN EVERY IMAGE.</b> The emitter
/// writes one template per entry of <see cref="Layout"/> and keeps no landing walk, tree or leaf size of
/// its own, and the engine's block step asks the same stop rule at every boundary, so the block a handler
/// runs and the tail emitted after its call come from one function. The scan plans each unit of the
/// program a payload is verified with and requires the unit's body to be the layout's instructions, one
/// for one, so an image with no emitter refuses a baseline payload this build would not have emitted,
/// and an image with one also holds it to its re-emission.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=DD3F28
// Broiler-Falsified-If: for some defined opcode RunsAlone, EndsBlock and StopsAfter disagree, or TryPlan, LayoutLength or Layout answers differently for the same image, unit and handler offsets
// Broiler-Human:        PENDING
public static class JsBaselineBlocks
{
    /// <summary>How many compares a leaf of a unit's landing tree makes before it gives up.</summary>
    /// <remarks>
    /// <b>Four, because a chain of four costs about what one more level of the tree would</b>, and a unit
    /// with no suspension and no handler has a single landing, its entry. The baseline emitter has no tree
    /// of its own: it writes the one <see cref="Layout"/> answers.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=DDFFAD
    // Broiler-Human:        PENDING
    public const int LeafLandings = 4;

    /// <summary>The bits of a table entry that hold a defined opcode's instruction width.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=C130C9
    // Broiler-Human:        PENDING
    private const byte WidthMask = 0x0F;

    /// <summary>The table-entry bit that says a block ends after an instruction of this byte.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=33ADB4
    // Broiler-Human:        PENDING
    private const byte EndsAfterBit = 0x10;

    /// <summary>The table-entry bit that says an instruction of this byte runs alone.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=499AB7
    // Broiler-Human:        PENDING
    private const byte RunsAloneBit = 0x20;

    /// <summary>The entry of a byte no opcode takes: it ends a block, runs alone and has no width.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=401F38
    // Broiler-Human:        PENDING
    private const byte UndefinedEntry = EndsAfterBit | RunsAloneBit;

    /// <summary>The mark of a plan that says an instruction of the unit starts at that byte.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=C60558
    // Broiler-Human:        PENDING
    private const byte StartMark = 0x01;

    /// <summary>The mark of a plan that says the instruction at that byte is a landing.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=095C84
    // Broiler-Human:        PENDING
    private const byte LandingMark = 0x02;

    /// <summary>The mark of a plan that says the instruction at that byte is a head and not a landing.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=2F36A6
    // Broiler-Human:        PENDING
    private const byte HeadMark = 0x04;

    /// <summary>One entry per byte value, which is all <see cref="StopsAfter"/> reads.</summary>
    /// <remarks>
    /// <b>A defined byte's entry is the predicates' own answers and an undefined byte's is the fail-closed
    /// reading.</b> For a defined byte the entry carries <see cref="JsOpcodes.InstructionWidth"/>,
    /// <see cref="EndsBlock"/> and <see cref="RunsAlone"/>. For an undefined byte it ends the block, runs
    /// alone and has width zero, which deliberately differs from <see cref="RunsAlone"/> (false) and from
    /// <see cref="JsOpcodes.InstructionWidth"/> (one) at that byte; the width is never read, because the
    /// ends bit decides first.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=58E210
    // Broiler-Falsified-If: for a defined byte the entry's width or either bit differs from JsOpcodes.InstructionWidth, EndsBlock or RunsAlone, or an undefined byte's entry lacks either bit
    // Broiler-Human:        PENDING
    private static readonly byte[] Table = Build();

    /// <summary>
    /// Whether an instruction of this opcode is always a block of one, so that a unit calls a handler for
    /// it and nothing else in that call.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THE SET IS THE CALL CLASS AND THE OPCODES WHOSE ARMS ENTER GUEST CODE ON THEIR COMMON PATH.</b> A
    /// call or a construction nests a whole activation under its arm. The iterator protocol, delegation,
    /// spreading an iterable and static class elements reach <c>JsEngine.Call</c> behind no test but the
    /// iterator record's done flag, a nullish <c>return</c> method, a missing <c>Symbol.iterator</c>
    /// (which throws) or a class with no static elements, so recursion through any of them is ordinary
    /// code. An instruction that runs alone can be given a step of its own opcode, whose frame is small,
    /// rather than the step that holds every arm.
    /// </para>
    /// <para>
    /// <b>EVERY OTHER OPCODE REACHES GUEST OR EMBEDDER CODE ONLY BEHIND A TEST ITS COMMON PATH FAILS</b>:
    /// the object test of <c>JsEngine.ToPrimitive</c> or <c>JsEngine.Render</c>, which a coerced or thrown
    /// value passes only when it is an object; the getter or setter test of <c>Lookup</c>,
    /// <c>GetSymbol</c>, <c>SetProperty</c>, <c>SetSymbol</c>, <c>SetWithReceiver</c>, <c>SetSuper</c>,
    /// <c>GetSymbolWithReceiver</c>, <c>SetSymbolWithReceiver</c>,
    /// <c>ReadPrivate</c> or <c>WritePrivate</c>; the dispatch to <c>JsProxy</c> or <c>JsHostObject</c>;
    /// or <c>InstanceOf</c>'s test for a <c>Symbol.hasInstance</c> method. A <c>valueOf</c> that is a
    /// plain method still runs guest code under <see cref="JsOpcode.Add"/>, behind the first of those
    /// tests. Those opcodes are not here: they are most instructions of most programs, and splitting a
    /// block at each of them would leave blocks of about one instruction.
    /// </para>
    /// <para>
    /// <b><see cref="JsOpcode.IterateCloseCheck"/> is not here</b>, because its arm calls nothing: it
    /// throws a <c>TypeError</c> built from a description of the value's type when the awaited answer is
    /// not an object. The <c>return</c> method whose answer it checks was called by
    /// <see cref="JsOpcode.IterateCloseAsync"/>, which is.
    /// </para>
    /// <para>
    /// An undefined byte answers <see langword="false"/> here; <see cref="StopsAfter"/> treats one as
    /// running alone.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=C25AED
    // Broiler-Falsified-If: an opcode of the call class is not named here, or an opcode not named here has an arm or an unconditionally called helper that reaches guest or embedder code other than behind the object test of ToPrimitive or Render, the getter or setter test of Lookup, GetSymbol, SetProperty, SetSymbol, SetWithReceiver, SetSuper, GetSymbolWithReceiver, SetSymbolWithReceiver, ReadPrivate or WritePrivate, the dispatch to JsProxy or JsHostObject, or InstanceOf's test for a Symbol.hasInstance method
    // Broiler-Human:        PENDING
    public static bool RunsAlone(JsOpcode opcode) => opcode switch
    {
        // THE CALL CLASS: a nested activation under every one of these.
        JsOpcode.Call or JsOpcode.Construct or JsOpcode.CallEval or JsOpcode.SuperCall or
        JsOpcode.SuperCallForwarded or JsOpcode.CallSpread or JsOpcode.ConstructSpread or
        JsOpcode.SuperCallSpread or JsOpcode.ImportCall or JsOpcode.CallEvalSpread or

        // THE EVAL NAME INSTRUCTIONS: a name resolved through an eval view may reach a `with`
        // object's `has`, `get`, `set` or `deleteProperty` trap or accessor at any record of the walk,
        // with no test in front of the walk that its common path fails. They are classified here
        // rather than argued into a block against this method's falsifier (JSD-0026 section 11).
        JsOpcode.LoadEvalName or JsOpcode.LoadEvalNameOrUndefined or JsOpcode.StoreEvalName or
        JsOpcode.LoadEvalNameWithBase or JsOpcode.DeleteEvalName or

        // AND THE VARIABLE-ENVIRONMENT WRITE OF A SLOPPY EVALUATION (JSeal V15): at the global
        // environment it is a `Set` on the global object, which reaches an accessor's setter or a
        // proxy's trap on its common path.
        JsOpcode.StoreEvalVariable or

        // THE EXPLICIT GUEST ENTRIES: the iterator protocol, delegation and static elements enter
        // guest code on their common path.
        JsOpcode.IterateStart or JsOpcode.IterateNext or JsOpcode.IterateRest or JsOpcode.IterateClose or
        JsOpcode.SpreadArray or JsOpcode.YieldDelegate or
        JsOpcode.IterateStartAsync or JsOpcode.IterateNextAsync or JsOpcode.IterateCloseAsync or

        // AND DISPOSAL: a step or an end calls every registered disposer on its common path.
        JsOpcode.DisposeStep or JsOpcode.DisposeEnd or
        JsOpcode.RunStaticElements => true,
        _ => false,
    };

    /// <summary>Whether a block ends after an instruction of this opcode.</summary>
    /// <remarks>
    /// <b>After a transfer, and after an instruction that runs alone.</b> After a transfer the next
    /// instruction is decided while the program runs, and the unit dispatches it with a direct branch or
    /// the tree, which is the only point where emitted control flow earns anything; an instruction that
    /// runs alone is a block of one by definition. Suspension does not end a block: a suspending
    /// instruction leaves the step before any boundary is consulted, and its successor is a landing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=4AA9FF
    // Broiler-Falsified-If: it answers false for an opcode that is terminal, has a code target or runs alone, or true for any other opcode
    // Broiler-Human:        PENDING
    public static bool EndsBlock(JsOpcode opcode) =>
        JsOpcodes.IsTerminal(opcode) || JsOpcodes.HasCodeTarget(opcode) || RunsAlone(opcode);

    /// <summary>
    /// Whether a block step that has just run the instruction at <paramref name="current"/>, and would
    /// run the one at <paramref name="pc"/> next, stops.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>IT STOPS AFTER AN INSTRUCTION THAT ENDS A BLOCK, ON A PROGRAM COUNTER THAT IS NOT THE NEXT
    /// INSTRUCTION, AT THE UNIT'S END, AND BEFORE AN INSTRUCTION THAT RUNS ALONE.</b> Those are tested in
    /// that order, and the unit's end is tested before the byte at <paramref name="pc"/> is read, so the
    /// test never reads past the unit. A program counter that is not the next instruction comes from a
    /// landing - a caught throw or a forced return - since a transfer ends its block first.
    /// </para>
    /// <para>
    /// <b>It reads one table and calls no predicate</b>, so that a step can ask it at every instruction.
    /// The table is built from <see cref="JsOpcodes.InstructionWidth"/>, <see cref="EndsBlock"/> and
    /// <see cref="RunsAlone"/>, and an undefined byte stops the step whichever side of the boundary it is
    /// on.
    /// </para>
    /// </remarks>
    /// <param name="code">The code section the unit's code is in.</param>
    /// <param name="current">The offset of the instruction the step has just run.</param>
    /// <param name="pc">The offset the step would run next.</param>
    /// <param name="unitEnd">The first offset past the unit.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=DD109B
    // Broiler-Falsified-If: for a defined byte at current it answers other than EndsBlock of that byte, or pc other than current plus that byte's width, or pc at or past unitEnd, or RunsAlone of the byte at pc - or it reads the byte at pc when pc is at or past unitEnd
    // Broiler-Human:        PENDING
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool StopsAfter(byte[] code, int current, int pc, int unitEnd)
    {
        var entry = Table[code[current]];

        return (entry & EndsAfterBit) != 0 ||
            pc != current + (entry & WidthMask) ||
            (uint)pc >= (uint)unitEnd ||
            (Table[code[pc]] & RunsAloneBit) != 0;
    }

    /// <summary>The handler offsets of every exception region of an image, grouped by code unit once.</summary>
    /// <remarks>
    /// <para>
    /// <b>ONE PASS COUNTS, ONE PASS PLACES, AND EACH UNIT'S OFFSETS ARE THEN A SLICE.</b> A caller that plans
    /// every unit of an image groups once and hands each unit its own slice, so planning an image reads
    /// each region once rather than once per unit: an image may carry as many units and regions as the
    /// format's ceilings admit, and a pass over every region for every unit is their product.
    /// </para>
    /// <para>
    /// Within a unit the offsets are in the order the image carries its regions. A region whose function
    /// index names no unit of the image is in no slice; the verifier refuses an artifact that carries one,
    /// and the baseline emitter reads none.
    /// </para>
    /// </remarks>
    /// <param name="image">The program whose regions are grouped.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=5CECB9
    // Broiler-Falsified-If: for some unit of the image the slice it answers differs from the handler offsets of the image's regions whose function index is that unit, in the order the image carries them
    // Broiler-Human:        PENDING
    public static JsBaselineHandlerOffsets GroupHandlerOffsets(JsNativeProgramImage image)
    {
        var units = image.Functions.Length;
        var bounds = new int[units + 1];

        foreach (var region in image.Regions)
        {
            if (region.FunctionIndex < (uint)units)
            {
                bounds[region.FunctionIndex + 1]++;
            }
        }

        for (var unit = 0; unit < units; unit++)
        {
            bounds[unit + 1] += bounds[unit];
        }

        // EACH REGION IS PLACED AT ITS UNIT'S CURSOR, which leaves every cursor at the next unit's first
        // slot; moving the bounds back one place restores each unit's first slot.
        var offsets = new uint[bounds[units]];

        foreach (var region in image.Regions)
        {
            if (region.FunctionIndex < (uint)units)
            {
                offsets[bounds[region.FunctionIndex]++] = region.HandlerOffset;
            }
        }

        for (var unit = units; unit > 0; unit--)
        {
            bounds[unit] = bounds[unit - 1];
        }

        bounds[0] = 0;
        return new JsBaselineHandlerOffsets(offsets, bounds);
    }

    /// <summary>
    /// The plan of one code unit: its landings, its block heads, and for each head its last instruction
    /// and the tail that follows its call - or a refusal that says why the unit has none.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>VERIFIED BYTECODE REACHES NONE OF THE REFUSALS.</b> An empty or overrunning code range, an
    /// undefined opcode, an instruction past its unit's end, a landing and a target that is not an
    /// instruction start are all things the verifier has already refused, and they answer the sentences
    /// the baseline emitter answered for them when it walked its units itself, which it now passes on;
    /// they are here so that a producer's defect answers a sentence rather than a wrong plan. Three
    /// refusals have sentences of their own: a unit index the image does not have, which the emitter
    /// never asks for; a unit longer than the format's code ceiling, which no verified code section
    /// holds and which is what bounds every length this class answers; and a branch to an offset that is
    /// not a head, which is a defect of this class. They are
    /// tested in this order: the unit index, the range, the ceiling, the walk, the landings (the smallest
    /// offending one is named), the targets in bytecode order, then the branches - all before anything is
    /// laid out, so a caller's own refusal, such as a ceiling on the code it writes, comes after every one
    /// of them.
    /// </para>
    /// <para>
    /// <b>THE LANDINGS ARE EVERY OFFSET A STEP CAN ANSWER THAT NO TAIL COMPARES.</b> The unit's entry;
    /// every handler offset it is given, which is where a caught throw or a forced return puts the program
    /// counter; the instruction after each <c>Yield</c>, <c>Await</c> and <c>EnterBody</c>, where a normal
    /// resume re-enters; and each <c>YieldDelegate</c> itself, which a delegating resume runs again. The
    /// handler offsets are the caller's to give: the unit's slice of <see cref="GroupHandlerOffsets"/> is
    /// the unit's, and a plan made with any other offsets is the plan of a unit with those landings.
    /// </para>
    /// <para>
    /// <b>THE HEADS ARE THE LANDINGS, EVERY CODE TARGET, EVERY INSTRUCTION THAT RUNS ALONE, AND EVERY
    /// SUCCESSOR OF AN INSTRUCTION THAT ENDS A BLOCK.</b> A head's last instruction is the first on the
    /// linear walk from it after which <see cref="StopsAfter"/> holds with the next instruction as the
    /// program counter, so the plan and a step use one function. Its tail leaves when that instruction is
    /// terminal or the unit's last, falls through when its successor is the next head, and branches to its
    /// successor's head otherwise; an instruction with a code target also compares that target first.
    /// </para>
    /// <para>
    /// <b>THREE WALKS, ONE MARK PER BYTE, AND ARRAYS OF EXACTLY WHAT THEY HOLD.</b> The first walk marks the
    /// instruction starts and the resume landings and counts them; the second fills the landings in
    /// ascending order, refuses a bad target and marks the other heads, counting each head once; the
    /// third lays the blocks out in order, and every head since the last instruction a step stops after
    /// takes the next one as its last, so a unit whose heads overlap costs one test per instruction rather
    /// than one walk per head. A branch the plan would name to an offset that is not a head is refused as a
    /// defect in the partition; by construction there is none.
    /// </para>
    /// </remarks>
    /// <param name="image">The program the unit belongs to.</param>
    /// <param name="unitIndex">Which code unit of <paramref name="image"/> to plan.</param>
    /// <param name="handlerOffsets">The handler offsets of the unit's exception regions.</param>
    /// <param name="plan">The plan, when the answer is <see langword="true"/>.</param>
    /// <param name="refusal">Why there is no plan, when the answer is <see langword="false"/>.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=D47050
    // Broiler-Falsified-If: a plan it answers omits the unit's entry, a handler offset it was given, the successor of a Yield, Await or EnterBody or a YieldDelegate from its landings, names a head that is not an instruction start of the unit, gives a head a last instruction other than the first after which StopsAfter holds on its linear walk, or it answers differently for the same image, unit and handler offsets
    // Broiler-Human:        PENDING
    public static bool TryPlan(
        JsNativeProgramImage image,
        int unitIndex,
        System.ReadOnlySpan<uint> handlerOffsets,
        out JsBaselineUnitPlan plan,
        out string refusal) =>
        Plan(image, unitIndex, handlerOffsets, eachInstruction: false, out plan, out refusal);

    /// <summary>
    /// The plan of one code unit in the value form's partition, in which every instruction is a block of
    /// its own - or a refusal that says why the unit has none.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>IT IS <see cref="TryPlan"/> WITH EVERY INSTRUCTION A HEAD AND EVERY INSTRUCTION A STOP</b>, so the
    /// landings, the refusals and their order, the tails and the layout grammar are that method's, and a
    /// value-form unit calls one helper per instruction and follows its answer exactly as a baseline unit
    /// follows a block's (JSD-0035 section 5, stage JSV-1). Every tail therefore compares the one
    /// instruction's own successors: its code target when it has one, then the instruction after it.
    /// </para>
    /// <para>
    /// <b>A helper runs one instruction, so no stop rule is asked while it runs</b>, and nothing in the
    /// engine reads this partition: the helper's step is a per-opcode instantiation, which stops after
    /// its one instruction by construction.
    /// </para>
    /// </remarks>
    /// <param name="image">The program the unit belongs to.</param>
    /// <param name="unitIndex">Which code unit of <paramref name="image"/> to plan.</param>
    /// <param name="handlerOffsets">The handler offsets of the unit's exception regions.</param>
    /// <param name="plan">The plan, when the answer is <see langword="true"/>.</param>
    /// <param name="refusal">Why there is no plan, when the answer is <see langword="false"/>.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=580104
    // Broiler-Falsified-If: a plan it answers has a block that is not exactly one instruction, omits an instruction of the unit from its heads, or has landings, tails or refusals other than TryPlan's for the same image, unit and handler offsets
    // Broiler-Human:        PENDING
    public static bool TryPlanEachInstruction(
        JsNativeProgramImage image,
        int unitIndex,
        System.ReadOnlySpan<uint> handlerOffsets,
        out JsBaselineUnitPlan plan,
        out string refusal) =>
        Plan(image, unitIndex, handlerOffsets, eachInstruction: true, out plan, out refusal);

    /// <summary>The one planning walk both partitions are made by.</summary>
    /// <param name="image">The program the unit belongs to.</param>
    /// <param name="unitIndex">Which code unit of <paramref name="image"/> to plan.</param>
    /// <param name="handlerOffsets">The handler offsets of the unit's exception regions.</param>
    /// <param name="eachInstruction">Whether every instruction is a head and ends its block.</param>
    /// <param name="plan">The plan, when the answer is <see langword="true"/>.</param>
    /// <param name="refusal">Why there is no plan, when the answer is <see langword="false"/>.</param>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=358E53
    // Broiler-Falsified-If: with eachInstruction false it answers differently from the baseline partition TryPlan documents, or with it true some block is longer than one instruction
    // Broiler-Human:        PENDING
    private static bool Plan(
        JsNativeProgramImage image,
        int unitIndex,
        System.ReadOnlySpan<uint> handlerOffsets,
        bool eachInstruction,
        out JsBaselineUnitPlan plan,
        out string refusal)
    {
        plan = null!;

        if ((uint)unitIndex >= (uint)image.Functions.Length)
        {
            refusal = "the image has no code unit " + unitIndex;
            return false;
        }

        var row = image.Functions[unitIndex];
        var code = image.Code;
        var first = (long)row.CodeOffset;
        var end = first + row.CodeLength;

        if (row.CodeLength == 0 || end > code.Length)
        {
            refusal = "the unit's code range is empty or runs past the artifact's code";
            return false;
        }

        if (row.CodeLength > JsFormat.CeilingCodeBytes)
        {
            refusal = "the unit's code is longer than the format's code ceiling";
            return false;
        }

        var unitFirst = (int)first;
        var unitEnd = (int)end;
        var marks = new byte[row.CodeLength];
        var landingCount = 1;
        var smallestBadLanding = long.MaxValue;
        marks[0] = LandingMark;

        // THE FIRST WALK MARKS THE INSTRUCTION STARTS AND THE RESUME LANDINGS, refusing an undefined
        // byte or an instruction past the unit's end where it meets one. A resume landing inside the unit
        // is an instruction start once the walk completes; the only one that is not is the unit's end.
        for (var at = unitFirst; at < unitEnd;)
        {
            if (!JsOpcodes.IsDefined(code[at]))
            {
                refusal = "the byte at " + at + " is not an instruction this format version defines";
                return false;
            }

            var opcode = (JsOpcode)code[at];
            var width = JsOpcodes.InstructionWidth(opcode);

            if (at + width > unitEnd)
            {
                refusal = "the `" + opcode + "` at " + at + " runs past the end of its unit";
                return false;
            }

            marks[at - unitFirst] |= StartMark;

            var resume = opcode switch
            {
                JsOpcode.Yield or JsOpcode.Await or JsOpcode.EnterBody => at + width,
                JsOpcode.YieldDelegate => at,
                _ => -1,
            };

            if (resume >= unitEnd)
            {
                smallestBadLanding = System.Math.Min(smallestBadLanding, resume);
            }
            else if (resume >= 0 && (marks[resume - unitFirst] & LandingMark) == 0)
            {
                marks[resume - unitFirst] |= LandingMark;
                landingCount++;
            }

            at += width;
        }

        foreach (var offset in handlerOffsets)
        {
            if (offset < first || offset >= end || (marks[offset - first] & StartMark) == 0)
            {
                smallestBadLanding = System.Math.Min(smallestBadLanding, offset);
            }
            else if ((marks[offset - first] & LandingMark) == 0)
            {
                marks[offset - first] |= LandingMark;
                landingCount++;
            }
        }

        // THE SMALLEST LANDING THAT IS NOT AN INSTRUCTION START IS THE ONE NAMED, which is the one a walk
        // over the sorted landings meets first.
        if (smallestBadLanding != long.MaxValue)
        {
            refusal =
                "the offset " + smallestBadLanding + " is a landing of the unit and not the start of one " +
                "of its instructions";

            return false;
        }

        var landings = new int[landingCount];
        var filled = 0;
        var headCount = landingCount;

        // THE SECOND WALK FILLS THE LANDINGS IN ASCENDING ORDER AND MARKS THE OTHER THREE KINDS OF HEAD,
        // refusing a target that is not an instruction start after every landing, in bytecode order.
        for (var at = unitFirst; at < unitEnd;)
        {
            var index = at - unitFirst;
            var opcode = (JsOpcode)code[at];
            var width = JsOpcodes.InstructionWidth(opcode);

            if ((marks[index] & LandingMark) != 0)
            {
                landings[filled++] = at;
            }

            if (JsOpcodes.HasCodeTarget(opcode))
            {
                var target = ReadTarget(code, at);

                if (target < first || target >= end || (marks[target - first] & StartMark) == 0)
                {
                    refusal =
                        "the `" + opcode + "` at " + at + " targets " + target + ", which is not " +
                        "the start of an instruction of its unit";

                    return false;
                }

                headCount += MarkHead(marks, (int)(target - first));
            }

            if (eachInstruction || RunsAlone(opcode))
            {
                headCount += MarkHead(marks, index);
            }

            if (EndsBlock(opcode) && at + width < unitEnd)
            {
                headCount += MarkHead(marks, index + width);
            }

            at += width;
        }

        // THE THIRD WALK LAYS THE BLOCKS OUT IN ORDER: every head met since the last instruction a step
        // stops after takes the next such instruction as its last. The unit's last instruction always
        // stops, so no head is left without one.
        var blocks = new JsBaselineBlock[headCount];
        var laid = 0;
        var pending = 0;

        for (var at = unitFirst; at < unitEnd;)
        {
            var opcode = (JsOpcode)code[at];
            var following = at + JsOpcodes.InstructionWidth(opcode);

            if ((marks[at - unitFirst] & (LandingMark | HeadMark)) != 0)
            {
                blocks[laid++] = new JsBaselineBlock(
                    at, opcode, at, false, JsBaselineBlock.NoTarget, following, JsBaselineTail.Leave);
            }

            if (eachInstruction || StopsAfter(code, at, following, unitEnd))
            {
                for (; pending < laid; pending++)
                {
                    blocks[pending] = blocks[pending] with { Last = at };
                }
            }

            at = following;
        }

        for (var index = 0; index < blocks.Length; index++)
        {
            var block = blocks[index];
            var ending = (JsOpcode)code[block.Last];
            var following = block.Last + JsOpcodes.InstructionWidth(ending);
            var hasTarget = JsOpcodes.HasCodeTarget(ending);
            var target = hasTarget ? (int)ReadTarget(code, block.Last) : JsBaselineBlock.NoTarget;
            var next = index + 1 < blocks.Length ? blocks[index + 1].Head : unitEnd;

            var tail = JsOpcodes.IsTerminal(ending) || following == unitEnd
                ? JsBaselineTail.Leave
                : following == next
                    ? JsBaselineTail.FallThrough
                    : JsBaselineTail.Branch;

            if ((hasTarget && !IsHead(marks, target - unitFirst)) ||
                (tail != JsBaselineTail.Leave && (following >= unitEnd || !IsHead(marks, following - unitFirst))))
            {
                refusal =
                    "the plan names a branch to an offset that is not a block head, which is a defect " +
                    "in the partition rather than in the program";

                return false;
            }

            blocks[index] = block with
            {
                HasTarget = hasTarget,
                Target = target,
                Following = following,
                Tail = tail,
            };
        }

        plan = new JsBaselineUnitPlan(unitIndex, unitFirst, unitEnd, landings, blocks);
        refusal = string.Empty;
        return true;
    }

    /// <summary>How many entries <see cref="Layout"/> answers for a plan, computed from the plan alone.</summary>
    /// <remarks>
    /// <para>
    /// <b>THE LENGTH IS THE LAYOUT'S GRAMMAR COUNTED, NOT A LAYOUT MEASURED.</b> Two entries open the
    /// dispatch and two make the defect block. A range of at most <see cref="LeafLandings"/> landings is two
    /// entries per landing and one jump; a longer range is three entries and its two halves. A block is
    /// three entries for its call, two more when its last instruction has a code target, and one, two or
    /// three for a tail that leaves, falls through or branches. So a caller can size what it compares a
    /// layout with, or refuse, before any of the layout exists.
    /// </para>
    /// <para>
    /// <b>It is at most four plus eleven per instruction of the unit</b>: a tree over landings is at most
    /// three entries per landing, a block at most eight, and neither landings nor heads outnumber the
    /// instructions. <see cref="TryPlan"/> refuses a unit longer than the format's code ceiling, so the
    /// length of any plan's layout is an <see cref="int"/>.
    /// </para>
    /// </remarks>
    /// <param name="plan">A plan <see cref="TryPlan"/> answered.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=4C8676
    // Broiler-Falsified-If: for a plan TryPlan answered it differs from the length of the array Layout answers for the same plan
    // Broiler-Human:        PENDING
    public static int LayoutLength(JsBaselineUnitPlan plan)
    {
        // The dispatch's sign test and leave, the tree, then the defect's move and jump.
        var length = 2L + TreeLength(plan.Landings.Length) + 2L;

        foreach (var block in plan.Blocks)
        {
            length += BlockLength(block);
        }

        return checked((int)length);
    }

    /// <summary>
    /// The unit's instructions between the prologue and the epilogue, as templates with their operands and
    /// their branch targets resolved to indices of the answer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THIS IS THE ONLY STATEMENT OF A BASELINE UNIT'S BODY</b>, made by the one internal walk the
    /// template scan compares a payload through. The prologue and the epilogue are fixed and are not part
    /// of it. What it answers, in order: the dispatch, which leaves on a negative answer
    /// and otherwise looks the answer up in a compare tree over the landings; the defect block, which
    /// materialises <see cref="JsBaselineStatus.Defect"/> and leaves; then, for each head in ascending
    /// order, the handler call for that head and its tail.
    /// </para>
    /// <para>
    /// <b>THE TREE IS A SEARCH OVER THE SORTED LANDINGS WITH A CHAIN AT EACH LEAF.</b> A range of at most
    /// <see cref="LeafLandings"/> landings is a chain of compares, each branching equal to its landing's
    /// head, ending in a jump to the defect; a longer range compares its middle landing, branches equal to
    /// its head and above to the right-hand range, and lays out the left-hand range before the right.
    /// </para>
    /// <para>
    /// <b>THE ANSWER IS ONE ARRAY OF <see cref="LayoutLength"/> ENTRIES, EACH WRITTEN ONCE AND IN
    /// ORDER.</b> Each block's first index is counted from the grammar before anything is written, a branch
    /// to a head finds that index through the plan's blocks by the head's offset, and an above-branch's
    /// subtree index is counted from the grammar too, from the length of the range to its left. So every
    /// entry is final when it is made, the entries are made in the order they stand, and no label is left
    /// to resolve afterwards. Nothing is sized by the unit's code. The walk that makes them is the one the
    /// template scan compares a payload through, entry by entry, which is why the order matters.
    /// </para>
    /// <para>
    /// <b>A TARGET IS AN INDEX INTO THE ANSWER</b>, or <see cref="JsBaselineInstruction.Leave"/> for the
    /// epilogue's first instruction, or <see cref="JsBaselineInstruction.None"/> for an instruction that
    /// does not branch. A plan <see cref="TryPlan"/> answered names no branch to an offset that is not a
    /// head; a branch to one would resolve to <see cref="JsBaselineInstruction.None"/>, which is not an
    /// index. An encoder of the layout must refuse a branch whose target is negative and not
    /// <see cref="JsBaselineInstruction.Leave"/> as a branch to a label it never bound, and must not read
    /// that target as an index.
    /// </para>
    /// </remarks>
    /// <param name="plan">A plan <see cref="TryPlan"/> answered.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=CAE84F
    // Broiler-Falsified-If: for a plan TryPlan answered, the layout's dispatch sends a landing anywhere but its own head's call or another non-negative answer anywhere but the defect, a head's call names a pc other than the head or a slot other than eight times its opcode, a tail differs from the one its block's kind and target dictate, a branch resolves to an index other than the instruction it names, or the answer's length differs from LayoutLength
    // Broiler-Human:        PENDING
    public static JsBaselineInstruction[] Layout(JsBaselineUnitPlan plan)
    {
        var filling = new LayoutFilling(new JsBaselineInstruction[LayoutLength(plan)]);
        Lay(plan, ref filling);
        return filling.Layout;
    }

    /// <summary>
    /// Makes a plan's layout entry by entry, in order, handing each to a sink that may stop the walk.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THIS IS THE ONE WALK THE LAYOUT IS MADE BY.</b> <see cref="Layout"/> fills an array through it, and
    /// the template scan compares a payload through it; neither states the layout's grammar a second time.
    /// Each entry handed out is final and carries its own index, which is exactly the index
    /// <see cref="Layout"/> writes it at.
    /// </para>
    /// <para>
    /// <b>WHAT IT HOLDS IS THE LAYOUT INDEX OF EACH BLOCK'S CALL AND NOTHING ELSE</b>, one integer per head,
    /// so that a branch to a head names its index before the head is reached. A sink that stops the walk
    /// after the entries a payload has makes no more of the layout than that.
    /// </para>
    /// </remarks>
    /// <param name="plan">A plan <see cref="TryPlan"/> answered.</param>
    /// <param name="sink">What takes each entry; its answer false stops the walk.</param>
    /// <typeparam name="TSink">The sink's type, a structure so that each take is a direct call.</typeparam>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=A0E4CD
    // Broiler-Falsified-If: for a plan TryPlan answered, it hands out an entry other than the one Layout writes at the index it names, hands out indices other than zero upward one at a time, goes on after the sink answers false, or answers true when it stopped before the last entry
    // Broiler-Human:        PENDING
    internal static bool Lay<TSink>(JsBaselineUnitPlan plan, ref TSink sink)
        where TSink : struct, IJsBaselineLayoutSink
    {
        var landings = plan.Landings;
        var blocks = plan.Blocks;

        // ---- where each part begins, from the grammar alone ------------------------------------------
        var defect = 2 + TreeLength(landings.Length);
        var starts = new int[blocks.Length];
        var start = defect + 2;

        for (var index = 0; index < blocks.Length; index++)
        {
            starts[index] = start;
            start += BlockLength(blocks[index]);
        }

        // ---- the dispatch: a negative answer leaves, anything else is looked up ----------------------
        var next = 0;

        if (!sink.Take(next++, Dispatching(JsBaselineTemplate.TestEax, 0, JsBaselineInstruction.None)) ||
            !sink.Take(next++, Dispatching(JsBaselineTemplate.JsLeave, 0, JsBaselineInstruction.Leave)) ||
            !Tree(ref sink, ref next, landings, 0, landings.Length - 1, defect, blocks, starts))
        {
            return false;
        }

        // ---- the defect: an answer with no landing ---------------------------------------------------
        if (!sink.Take(next++, Dispatching(JsBaselineTemplate.MovStatus, (int)JsBaselineStatus.Defect, JsBaselineInstruction.None)) ||
            !sink.Take(next++, Dispatching(JsBaselineTemplate.Jmp, 0, JsBaselineInstruction.Leave)))
        {
            return false;
        }

        // ---- one call and one tail per head, in ascending order ---------------------------------------
        foreach (var block in blocks)
        {
            var head = block.Head;

            if (!sink.Take(next++, new JsBaselineInstruction(
                    JsBaselineTemplate.MovArg0R14, 0, JsBaselineInstruction.None, JsBaselineRole.Head, head)) ||
                !sink.Take(next++, new JsBaselineInstruction(
                    JsBaselineTemplate.MovArg1Pc, head, JsBaselineInstruction.None, JsBaselineRole.Head, head)) ||
                !sink.Take(next++, new JsBaselineInstruction(
                    JsBaselineTemplate.CallSlot, (int)block.HeadOpcode * 8, JsBaselineInstruction.None, JsBaselineRole.Slot, head)))
            {
                return false;
            }

            if (block.HasTarget &&
                (!sink.Take(next++, Tailing(JsBaselineTemplate.CmpEaxPc, block.Target, JsBaselineInstruction.None, head)) ||
                 !sink.Take(next++, Tailing(JsBaselineTemplate.Je, 0, HeadIndex(blocks, starts, block.Target), head))))
            {
                return false;
            }

            var taken = block.Tail switch
            {
                JsBaselineTail.Leave =>
                    sink.Take(next++, Tailing(JsBaselineTemplate.Jmp, 0, 0, head)),

                JsBaselineTail.FallThrough =>
                    sink.Take(next++, Tailing(JsBaselineTemplate.CmpEaxPc, block.Following, JsBaselineInstruction.None, head)) &&
                    sink.Take(next++, Tailing(JsBaselineTemplate.Jne, 0, 0, head)),

                _ =>
                    sink.Take(next++, Tailing(JsBaselineTemplate.CmpEaxPc, block.Following, JsBaselineInstruction.None, head)) &&
                    sink.Take(next++, Tailing(JsBaselineTemplate.Je, 0, HeadIndex(blocks, starts, block.Following), head)) &&
                    sink.Take(next++, Tailing(JsBaselineTemplate.Jmp, 0, 0, head)),
            };

            if (!taken)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>The compare tree over a sorted range of a unit's landings, handed to a sink in order.</summary>
    /// <remarks>
    /// <b>A LEAF IS A CHAIN AND AN INNER NODE SPLITS ON ITS MIDDLE, WITH AN UNSIGNED COMPARE.</b> The
    /// dispatch has already sent every negative answer out of the unit, so what reaches the tree is a
    /// non-negative offset and an above-branch orders it correctly. A chain ends in the defect, so an
    /// offset the unit has no landing for never falls into whatever block follows. An above-branch goes to
    /// the entry after the range to its left, whose length <see cref="TreeLength"/> counts, so it is made
    /// before that range is.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=0D7902
    // Broiler-Falsified-If: a landing in the range is compared anywhere but on a path that branches to its own head, an offset outside the range reaches anything but the defect, it hands out other than TreeLength entries for the range, or it goes on after the sink answers false
    // Broiler-Human:        PENDING
    private static bool Tree<TSink>(
        ref TSink sink,
        ref int next,
        System.ReadOnlySpan<int> landings,
        int low,
        int high,
        int defect,
        System.ReadOnlySpan<JsBaselineBlock> blocks,
        int[] starts)
        where TSink : struct, IJsBaselineLayoutSink
    {
        if (high - low + 1 <= LeafLandings)
        {
            for (var position = low; position <= high; position++)
            {
                if (!sink.Take(next++, Dispatching(JsBaselineTemplate.CmpEaxPc, landings[position], JsBaselineInstruction.None)) ||
                    !sink.Take(next++, Dispatching(JsBaselineTemplate.Je, 0, HeadIndex(blocks, starts, landings[position]))))
                {
                    return false;
                }
            }

            return sink.Take(next++, Dispatching(JsBaselineTemplate.Jmp, 0, defect));
        }

        var middle = (low + high) / 2;

        if (!sink.Take(next++, Dispatching(JsBaselineTemplate.CmpEaxPc, landings[middle], JsBaselineInstruction.None)) ||
            !sink.Take(next++, Dispatching(JsBaselineTemplate.Je, 0, HeadIndex(blocks, starts, landings[middle]))))
        {
            return false;
        }

        // THE RIGHT-HAND RANGE STARTS AFTER THIS BRANCH AND THE WHOLE LEFT-HAND RANGE, which the grammar
        // counts before either is made.
        var right = next + 1 + TreeLength(middle - low);

        return sink.Take(next++, Dispatching(JsBaselineTemplate.Ja, 0, right)) &&
            Tree(ref sink, ref next, landings, low, middle - 1, defect, blocks, starts) &&
            Tree(ref sink, ref next, landings, middle + 1, high, defect, blocks, starts);
    }

    /// <summary>How many entries the compare tree over a range of this many landings is.</summary>
    /// <remarks>
    /// <b>It splits a range exactly as <see cref="Tree"/> does</b>: the middle of a range of
    /// <paramref name="count"/> landings has <c>(count - 1) / 2</c> of them to its left, whatever the
    /// range's first position.
    /// </remarks>
    /// <param name="count">How many landings the range holds.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=A8605C
    // Broiler-Falsified-If: for some count it differs from the number of entries Tree writes for a range of that many landings
    // Broiler-Human:        PENDING
    private static int TreeLength(int count)
    {
        if (count <= LeafLandings)
        {
            return (2 * System.Math.Max(count, 0)) + 1;
        }

        var left = (count - 1) / 2;
        return 3 + TreeLength(left) + TreeLength(count - 1 - left);
    }

    /// <summary>How many entries a block's call and tail are.</summary>
    /// <param name="block">The block.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=AB6E97
    // Broiler-Falsified-If: for some block it differs from the number of entries Layout writes for that block's call and tail
    // Broiler-Human:        PENDING
    private static int BlockLength(JsBaselineBlock block) =>
        3 + (block.HasTarget ? 2 : 0) + block.Tail switch
        {
            JsBaselineTail.Leave => 1,
            JsBaselineTail.FallThrough => 2,
            _ => 3,
        };

    /// <summary>The layout index of the call of the block whose head is at an offset, found through the blocks.</summary>
    /// <remarks>
    /// <b>The blocks ascend by head, so a head is found by halving</b>, and nothing indexed by the unit's
    /// code is kept. An offset that is no block's head answers <see cref="JsBaselineInstruction.None"/>.
    /// </remarks>
    /// <param name="blocks">The plan's blocks.</param>
    /// <param name="starts">The layout index of each block's first entry.</param>
    /// <param name="offset">The head's offset.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=E31FC6
    // Broiler-Falsified-If: it answers an index other than the first entry of the block whose head is at the offset, or an index when no block's head is there
    // Broiler-Human:        PENDING
    private static int HeadIndex(System.ReadOnlySpan<JsBaselineBlock> blocks, int[] starts, int offset)
    {
        var low = 0;
        var high = blocks.Length - 1;

        while (low <= high)
        {
            var middle = low + ((high - low) / 2);
            var head = blocks[middle].Head;

            if (head == offset)
            {
                return starts[middle];
            }

            if (head < offset)
            {
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        return JsBaselineInstruction.None;
    }

    /// <summary>One entry of the dispatch region.</summary>
    /// <param name="template">The template.</param>
    /// <param name="operand">Its operand.</param>
    /// <param name="target">Its resolved target.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=4B1CE2
    // Broiler-Human:        PENDING
    private static JsBaselineInstruction Dispatching(JsBaselineTemplate template, int operand, int target) =>
        new(template, operand, target, JsBaselineRole.Dispatch, -1);

    /// <summary>One entry of a head's tail.</summary>
    /// <param name="template">The template.</param>
    /// <param name="operand">Its operand.</param>
    /// <param name="target">Its resolved target.</param>
    /// <param name="head">The head the tail belongs to.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=B8B8FB
    // Broiler-Human:        PENDING
    private static JsBaselineInstruction Tailing(JsBaselineTemplate template, int operand, int target, int head) =>
        new(template, operand, target, JsBaselineRole.Tail, head);

    /// <summary>Marks an instruction start as a head, and answers one when it was neither a head nor a landing.</summary>
    /// <param name="marks">The plan's marks.</param>
    /// <param name="index">The instruction's index in the unit.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=F241EB
    // Broiler-Falsified-If: it answers one for an index that was already a head or a landing, or zero for one that was neither
    // Broiler-Human:        PENDING
    private static int MarkHead(byte[] marks, int index)
    {
        if ((marks[index] & (LandingMark | HeadMark)) != 0)
        {
            return 0;
        }

        marks[index] |= HeadMark;
        return 1;
    }

    /// <summary>Whether the instruction at an index of the unit is a head, a landing being one.</summary>
    /// <param name="marks">The plan's marks.</param>
    /// <param name="index">The instruction's index in the unit.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=3D4D59
    // Broiler-Human:        PENDING
    private static bool IsHead(byte[] marks, int index) =>
        (uint)index < (uint)marks.Length && (marks[index] & (LandingMark | HeadMark)) != 0;

    /// <summary>Reads the <c>u32</c> code target that follows an opcode byte.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=D311EA
    // Broiler-Human:        PENDING
    private static uint ReadTarget(byte[] code, int at) =>
        System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(System.MemoryExtensions.AsSpan(code, at + 1));

    /// <summary>Builds <see cref="Table"/> from the predicates.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=107A7E
    // Broiler-Falsified-If: for some byte the entry it writes differs from the one the Table remark states
    // Broiler-Human:        PENDING
    private static byte[] Build()
    {
        var table = new byte[256];

        for (var value = 0; value < table.Length; value++)
        {
            if (!JsOpcodes.IsDefined((byte)value))
            {
                table[value] = UndefinedEntry;
                continue;
            }

            var opcode = (JsOpcode)value;
            var entry = (byte)JsOpcodes.InstructionWidth(opcode);

            if (EndsBlock(opcode))
            {
                entry |= EndsAfterBit;
            }

            if (RunsAlone(opcode))
            {
                entry |= RunsAloneBit;
            }

            table[value] = entry;
        }

        return table;
    }

    /// <summary>The sink <see cref="Layout"/> fills its array through.</summary>
    /// <param name="layout">The array, of exactly <see cref="LayoutLength"/> entries.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=1D704E
    // Broiler-Human:        PENDING
    private readonly struct LayoutFilling(JsBaselineInstruction[] layout) : IJsBaselineLayoutSink
    {
        /// <summary>The array being filled.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=86DDD9
        // Broiler-Human:        PENDING
        public JsBaselineInstruction[] Layout => layout;

        /// <summary>Writes the entry at its index and always goes on.</summary>
        /// <param name="index">The entry's index.</param>
        /// <param name="entry">The entry.</param>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=8D1364
        // Broiler-Human:        PENDING
        public bool Take(int index, JsBaselineInstruction entry)
        {
            layout[index] = entry;
            return true;
        }
    }
}

/// <summary>What takes a baseline layout's entries, one at a time and in order, as they are made.</summary>
/// <remarks>
/// <b>IT IS INTERNAL TO THE FORMAT ASSEMBLY</b>, where the layout is made and where the template scan that
/// compares a payload with it lives; a caller outside the assembly reads a whole layout through
/// <see cref="JsBaselineBlocks.Layout"/>.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=7760FA
// Broiler-Human:        PENDING
internal interface IJsBaselineLayoutSink
{
    /// <summary>Takes the entry at an index of the layout, and answers whether the walk goes on.</summary>
    /// <param name="index">The entry's index in the layout.</param>
    /// <param name="entry">The entry.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=105555
    // Broiler-Human:        PENDING
    bool Take(int index, JsBaselineInstruction entry);
}

/// <summary>The handler offsets of an image's exception regions, grouped by code unit.</summary>
/// <remarks>
/// <b>ONLY <see cref="JsBaselineBlocks.GroupHandlerOffsets"/> BUILDS ONE</b>, and a caller reads it one
/// unit at a time. It holds two arrays of exactly their count: every grouped offset, and where each unit's
/// offsets begin.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=CB4ECA
// Broiler-Human:        PENDING
public sealed class JsBaselineHandlerOffsets
{
    private readonly uint[] offsets;
    private readonly int[] bounds;

    internal JsBaselineHandlerOffsets(uint[] offsets, int[] bounds)
    {
        this.offsets = offsets;
        this.bounds = bounds;
    }

    /// <summary>How many code units the image had when its offsets were grouped.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=CD9D7E
    // Broiler-Human:        PENDING
    public int Units => bounds.Length - 1;

    /// <summary>The handler offsets of one unit's regions, or none for a unit the image did not have.</summary>
    /// <param name="unitIndex">Which code unit.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=1E2F6A
    // Broiler-Falsified-If: it answers offsets outside the unit's own group, or any offset for an index the image had no unit at
    // Broiler-Human:        PENDING
    public System.ReadOnlySpan<uint> Of(int unitIndex) =>
        (uint)unitIndex < (uint)Units
            ? System.MemoryExtensions.AsSpan(offsets, bounds[unitIndex], bounds[unitIndex + 1] - bounds[unitIndex])
            : default;
}

/// <summary>The plan of one baseline code unit: its landings and its blocks, in ascending order.</summary>
/// <remarks>
/// <b>ONLY <see cref="JsBaselineBlocks.TryPlan"/> BUILDS ONE, AND A CALLER CAN ONLY READ IT</b>, so every
/// plan a caller holds has passed the checks that method makes: every landing and every head is an
/// instruction start of the unit, and every branch a tail names goes to a head.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=E0B32A
// Broiler-Human:        PENDING
public sealed class JsBaselineUnitPlan
{
    private readonly int[] landings;
    private readonly JsBaselineBlock[] blocks;

    internal JsBaselineUnitPlan(int unitIndex, int first, int end, int[] landings, JsBaselineBlock[] blocks)
    {
        UnitIndex = unitIndex;
        First = first;
        End = end;
        this.landings = landings;
        this.blocks = blocks;
    }

    /// <summary>Which code unit of the image this is the plan of.</summary>
    public int UnitIndex { get; }

    /// <summary>The offset of the unit's first instruction.</summary>
    public int First { get; }

    /// <summary>The first offset past the unit.</summary>
    public int End { get; }

    /// <summary>The unit's landings, ascending and distinct, its entry first.</summary>
    public System.ReadOnlySpan<int> Landings => landings;

    /// <summary>One block per head, ascending by head.</summary>
    public System.ReadOnlySpan<JsBaselineBlock> Blocks => blocks;
}

/// <summary>One block of a baseline unit: a head, the last instruction a step from it runs, and its tail.</summary>
/// <param name="Head">The offset of the head, which is the program counter its call passes.</param>
/// <param name="HeadOpcode">The opcode at the head; the call's slot is eight times it.</param>
/// <param name="Last">The offset of the last instruction a step started at the head runs on a linear path.</param>
/// <param name="HasTarget">Whether that last instruction has a code target, which the tail compares first.</param>
/// <param name="Target">That code target, or <see cref="NoTarget"/> when there is none.</param>
/// <param name="Following">The offset after the last instruction.</param>
/// <param name="Tail">What the unit does with an answer the target compare did not take.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=E281FD
// Broiler-Human:        PENDING
public readonly record struct JsBaselineBlock(
    int Head,
    JsOpcode HeadOpcode,
    int Last,
    bool HasTarget,
    int Target,
    int Following,
    JsBaselineTail Tail)
{
    /// <summary>The <see cref="Target"/> of a block whose last instruction has no code target.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=EB5C86
    // Broiler-Human:        PENDING
    public const int NoTarget = -1;
}

/// <summary>What a baseline block's tail does with an answer its target compare did not take.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=062EE4
// Broiler-Human:        PENDING
public enum JsBaselineTail : byte
{
    /// <summary>Goes to the dispatch: the last instruction is terminal, or it is the unit's last.</summary>
    Leave = 0,

    /// <summary>Compares the successor and falls into its head, which is the next block, or goes to the dispatch.</summary>
    FallThrough = 1,

    /// <summary>Compares the successor and branches to its head, or goes to the dispatch.</summary>
    Branch = 2,
}

/// <summary>The templates a baseline unit's body is made of, by what they do.</summary>
/// <remarks>
/// The prologue's and the epilogue's templates are not here: they are fixed, and a layout does not
/// state them.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=71E148
// Broiler-Human:        PENDING
public enum JsBaselineTemplate : byte
{
    /// <summary><c>test eax, eax</c>: the dispatch's sign test.</summary>
    TestEax,

    /// <summary><c>js rel32</c>: a negative answer leaves the unit.</summary>
    JsLeave,

    /// <summary><c>cmp eax, pc</c>: compares an answer with a program counter.</summary>
    CmpEaxPc,

    /// <summary><c>je rel32</c>.</summary>
    Je,

    /// <summary><c>jne rel32</c>.</summary>
    Jne,

    /// <summary><c>ja rel32</c>.</summary>
    Ja,

    /// <summary><c>jmp rel32</c>.</summary>
    Jmp,

    /// <summary><c>mov eax, status</c>: materialises a status.</summary>
    MovStatus,

    /// <summary><c>mov arg0, r14</c>: passes the frame.</summary>
    MovArg0R14,

    /// <summary><c>mov arg1d, pc</c>: passes the head's program counter.</summary>
    MovArg1Pc,

    /// <summary><c>call [rbx+slot]</c>: calls the head's handler through the table.</summary>
    CallSlot,
}

/// <summary>Which part of a baseline unit's body a layout entry belongs to.</summary>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=CD64E4
// Broiler-Human:        PENDING
public enum JsBaselineRole : byte
{
    /// <summary>The dispatch, the landing tree or the defect block.</summary>
    Dispatch = 0,

    /// <summary>The two moves before a head's call.</summary>
    Head = 1,

    /// <summary>A head's call through the handler table.</summary>
    Slot = 2,

    /// <summary>A head's tail.</summary>
    Tail = 3,
}

/// <summary>One entry of a baseline unit's layout.</summary>
/// <param name="Template">Which template it is.</param>
/// <param name="Operand">
/// The program counter a compare or a move passes, eight times the opcode a call's slot is, the status a
/// move materialises, and zero otherwise.
/// </param>
/// <param name="Target">
/// For a branch, the index in the layout it goes to, or <see cref="Leave"/>; <see cref="None"/> otherwise.
/// </param>
/// <param name="Role">Which part of the body it belongs to.</param>
/// <param name="Head">The head it belongs to, or -1 in the dispatch.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=F9F4A0
// Broiler-Human:        PENDING
public readonly record struct JsBaselineInstruction(
    JsBaselineTemplate Template,
    int Operand,
    int Target,
    JsBaselineRole Role,
    int Head)
{
    /// <summary>The target of a branch to the epilogue's first instruction.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=5D559B
    // Broiler-Human:        PENDING
    public const int Leave = -1;

    /// <summary>The target of an entry that does not branch.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=340F55
    // Broiler-Human:        PENDING
    public const int None = -2;
}
