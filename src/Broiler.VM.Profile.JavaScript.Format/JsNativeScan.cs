// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   16
// Annotated:        16/16
// Exempt:           15
// Human-reviewed:   0/16
// IP risk:          Low
// Security risk:    Critical
// Criteria:         5/5
// Resource impact:  3/10 max
// Unverified:       16
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>What a template-closure scan answered, as one word.</summary>
/// <remarks>
/// <b>IT IS A VOCABULARY AND NOT A BOOLEAN, BECAUSE THE ANSWER "NO" IS USELESS TO EVERY READER OF
/// IT.</b> A refusal has to name the byte and the reason: a producer needs to know which template
/// its emitter wrote that this build cannot decode, a reader of a refused artifact needs to know
/// whether the payload was truncated or forged, and a lane check needs to be able to say which
/// clause it exercised. Each member below is one clause of the scan.
/// </remarks>
/// <remarks>
/// <b>TWO OF THEM GUARD THE TABLE RATHER THAN THE PAYLOAD, and saying which is more useful than
/// implying that bytes reach every one.</b> <see cref="AmbiguousTemplate"/> answers when two
/// templates match one byte sequence, which is decidable from the table alone and which no payload
/// can provoke while the table's entries are mutually exclusive - as this build's are;
/// <see cref="UnknownArchitecture"/> is unreachable from the verifier, which has already refused
/// every architecture value this build does not name, and is reachable from a caller of this scan
/// that names one. Every other member is reached by bytes somebody could hand this build, and the
/// composition lane beside the backends has a row for each.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=294341
// Broiler-Human:        PENDING
public enum JsNativeScanOutcome
{
    /// <summary>Every byte belongs to exactly one template instantiation with in-range operands.</summary>
    Accepted = 0,

    /// <summary>The architecture the payload names has no template table in this build.</summary>
    UnknownArchitecture = 1,

    /// <summary>A byte belongs to no code unit, because the first symbol does not start the blob.</summary>
    UnclaimedBytes = 2,

    /// <summary>Two units overlap, or a later unit starts before an earlier one.</summary>
    OverlappingUnits = 3,

    /// <summary>No template of this architecture matches the bytes at this offset.</summary>
    NoTemplate = 4,

    /// <summary>
    /// Two templates match one byte sequence, so the byte belongs to two instantiations rather than
    /// to one.
    /// </summary>
    AmbiguousTemplate = 5,

    /// <summary>A template matches and one of its operands is not a value a backend could write.</summary>
    OperandOutOfRange = 6,

    /// <summary>An instantiation runs past the end of the unit it began in.</summary>
    InstructionCrossesUnitEnd = 7,

    /// <summary>A branch leaves the unit that contains it.</summary>
    BranchLeavesUnit = 8,

    /// <summary>A branch lands inside an instruction rather than on one.</summary>
    BranchIntoInstruction = 9,

    /// <summary>A call's target is not the entry point of any code unit.</summary>
    CallNotAUnitEntry = 10,

    /// <summary>A unit's last instruction is not a return.</summary>
    UnitDoesNotEndInAReturn = 11,

    /// <summary>The bytes between one unit's last instruction and the next entry are not padding.</summary>
    PaddingNotAlignment = 12,

    /// <summary>
    /// A baseline unit does not open with exactly the prologue and close with exactly the epilogue,
    /// or carries an instruction of either anywhere else.
    /// </summary>
    FrameSequenceMalformed = 13,

    /// <summary>A baseline branch lands inside the prologue, or on an epilogue instruction after its first.</summary>
    BranchIntoFrameSequence = 14,
}

/// <summary>What a scan answered, and about which byte.</summary>
/// <remarks>
/// <b>The offset is counted from the first byte of the EMITTED BLOB and not from the artifact's
/// first byte</b>, because the blob is the thing the scan was handed and a scan that reported
/// artifact offsets would be reporting a number it computed from a framing it was not given. A
/// caller that wants an artifact position adds the section's own base to it, which is the caller's
/// arithmetic and not this one's.
/// </remarks>
/// <param name="Outcome">The clause that answered.</param>
/// <param name="Offset">The byte the answer is about, from the start of the emitted blob.</param>
/// <param name="Unit">The code unit the byte belongs to, or minus one where none does.</param>
/// <param name="Reason">One sentence naming what was found and what was expected.</param>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8C38B9
// Broiler-Human:        PENDING
public readonly record struct JsNativeScanResult(
    JsNativeScanOutcome Outcome,
    uint Offset,
    int Unit,
    string Reason)
{
    /// <summary>Whether the payload is closed under the template table.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=445910
    // Broiler-Human:        PENDING
    public bool Accepted => Outcome == JsNativeScanOutcome.Accepted;
}

/// <summary>
/// The template-closure scan: it decodes an emitted payload against the closed template table and
/// requires every byte of it to belong to an instantiation a backend of this build could have
/// written.
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS IS THE LAYER AN EXECUTION-ONLY IMAGE HAS AND THE ONLY ONE IT HAS.</b> An image that
/// carries no lowering cannot re-emit an artifact's bytecode and compare, so before this existed
/// its trust in an emitted payload rested on provenance and on nothing the build could check - and
/// the architecture value, which asks which machine the bytes are for rather than whether they are
/// code, was the whole of what stood between a malformed payload and an armed page. On 2026-09-08 a
/// corpus entry carrying FOUR ZERO BYTES as an x86-64 payload verified, was armed, was jumped into
/// and killed the process with an access violation. Four zero bytes match no template of any table
/// here.
/// </para>
/// <para>
/// <b>IT RUNS ALWAYS AND NOT ONLY WHERE NO BACKEND IS IN THE IMAGE.</b> Re-emission equality is
/// strictly stronger where it is available - it reaches the GENERATOR, and this reaches only the
/// PAYLOAD - but a layer that ran only in the weaker configuration would be a layer nobody
/// exercises: every lane in this repository that compiles a native artifact carries a backend, so
/// a scan skipped in that case would be a scan with no test behind it, and the first time it ran
/// for real would be in the composition that has no other check.
/// </para>
/// <para>
/// <b>WHAT IT CANNOT SAY IS THAT THE CODE IS RIGHT, and that is a property of the design rather
/// than a gap in it.</b> A well-formed sequence of the WRONG templates is well formed: a backend
/// that emitted a multiply where the bytecode said add produces a payload every clause below
/// accepts. It also would not have caught the accident the core retains as a fixture - a wrong
/// calling convention that returned the correct answer every time and leaked stack until the
/// process died millions of calls later - because a well-formed artifact from a wrong generator is
/// well formed. Re-emission equality is the only layer that says otherwise, and what would pin the
/// generator instead is a differential oracle, which is somebody else's stage.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=31ACE8
// Broiler-Falsified-If: a payload this scan accepts carries a byte no backend of this build could have emitted
// Broiler-Human:        PENDING
public static class JsNativeScan
{
    /// <summary>Scans an emitted payload against the template table for its architecture.</summary>
    /// <remarks>
    /// <para>
    /// <b>THE UNITS TILE THE BLOB AND THE SCAN HOLDS THEM TO IT.</b> A symbol row carries no
    /// length, so a unit runs from its own offset to the next one's and the last runs to the end of
    /// the blob. Every byte therefore belongs to exactly one unit, or the scan refuses: bytes
    /// before the first entry point belong to nothing, and two rows out of order would leave the
    /// extent of a unit a question of which row a reader happened to look at next.
    /// </para>
    /// <para>
    /// <b><c>instantiated</c> is how a lane check asks which templates a corpus of programs
    /// actually reached, and it exists because a template nobody reaches is a template nobody
    /// checks.</b> The closure direction is only as strong as the programs it compiles: a table
    /// entry no program instantiates is an entry the encoders could have contradicted without any
    /// row going red. Passing a collection here costs a scan nothing and lets the check name the
    /// gap.
    /// </para>
    /// <para>
    /// <b>The branch check is a second pass, because a forward branch names an instruction the
    /// first pass has not reached yet.</b> The first pass records where each instantiation began;
    /// the second requires every branch to land on one of those, in the same unit, and every call
    /// to land on a code unit's entry point. A branch into the MIDDLE of an instruction is the
    /// defect this pair exists for: it is a legal displacement, it decodes as something the emitter
    /// never wrote, and no framing check has ever been able to see it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=885F63
    // Broiler-Falsified-If: a byte sequence no backend of this build can emit is accepted, or a sequence one of them emits is refused
    // Broiler-Human:        PENDING
    public static JsNativeScanResult Scan(
        JsNativeArchitecture architecture,
        byte[] code,
        JsNativeSymbolRow[] symbols,
        uint alignment,
        System.Collections.Generic.ICollection<string>? instantiated = null) =>
        Scan(architecture, JsNativeTier.Numeric, code, symbols, alignment, instantiated);

    /// <summary>
    /// Scans an emitted payload against the template table for its architecture and native tier.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THE NUMERIC TIER IS THE SCAN ABOVE, CLAUSE FOR CLAUSE.</b> Every clause the overload
    /// without a tier states is applied here to whichever table the tier selects: the units tile the
    /// blob, every byte belongs to an admitted instantiation, a unit ends in a return, padding
    /// follows a return only, and every branch lands on an instruction start of its own unit.
    /// </para>
    /// <para>
    /// <b>THE x86-64 BASELINE TABLES ADD THREE FRAME-SHAPE CLAUSES, AND THEY ARE WHAT MAKE THE ONE
    /// INDIRECT CALL SAFE.</b> A baseline unit's first six instantiations are exactly the prologue
    /// in order and its last four exactly the epilogue in order, neither sequence's instructions
    /// occur anywhere else, and no branch lands inside the prologue or on an epilogue instruction
    /// after its first. RBX is then written once, at entry, by the load of the table base out of the
    /// frame and restored once, at the single return; nothing in the table writes memory; so every
    /// <c>call qword [rbx+disp32]</c> a scan-accepted unit makes reads the table the runtime stored
    /// in the frame, at a slot the field kind holds to a defined opcode.
    /// </para>
    /// <para>
    /// <b>WHAT THE CLAUSES CANNOT SAY IS WHICH HANDLER BELONGS AT WHICH PROGRAM COUNTER.</b> A unit
    /// that calls the handler for one opcode where the bytecode holds another is closed and
    /// well-shaped. The handler closes that gap at run time: it refuses a program counter the
    /// managed side did not compute and an opcode byte that is not its own, and the unit answers a
    /// defect rather than any JavaScript value.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=3; Fingerprint=DCCED6
    // Broiler-Falsified-If: a baseline x86-64 payload is accepted whose unit writes RBX or RSP outside one prologue and one epilogue, branches into either sequence, or makes an indirect call other than through the handler table at a defined opcode's slot
    // Broiler-Human:        PENDING
    public static JsNativeScanResult Scan(
        JsNativeArchitecture architecture,
        JsNativeTier tier,
        byte[] code,
        JsNativeSymbolRow[] symbols,
        uint alignment,
        System.Collections.Generic.ICollection<string>? instantiated = null)
    {
        var templates = JsNativeTemplates.For(architecture, tier);

        if (templates.Length == 0)
        {
            return new JsNativeScanResult(
                JsNativeScanOutcome.UnknownArchitecture,
                0,
                -1,
                "this build enumerates no instruction templates for the architecture the payload " +
                "names, so no byte of it can be shown to be code");
        }

        if (symbols.Length == 0 || symbols[0].Offset != 0)
        {
            return new JsNativeScanResult(
                JsNativeScanOutcome.UnclaimedBytes,
                0,
                -1,
                "the emitted blob does not begin at a code unit's entry point, so its first bytes " +
                "belong to no unit and nothing decodes them");
        }

        // The first pass decodes; the second judges the branches. Both need the same two arrays, so
        // they are built once here rather than by each.
        var starts = new System.Collections.Generic.HashSet<uint>();
        var branches = new System.Collections.Generic.List<Branch>();

        // THE FRAME-SHAPE CLAUSES NEED THE SEQUENCE OF TEMPLATES EACH UNIT INSTANTIATED, and the
        // offsets no branch may land on; neither is collected for a table the clauses do not apply to.
        var framed = JsNativeTemplates.IsX64Baseline(templates);
        var sequence = framed ? new System.Collections.Generic.List<(uint At, int Index)>() : null;
        var frames = framed ? new System.Collections.Generic.HashSet<uint>() : null;

        for (var unit = 0; unit < symbols.Length; unit++)
        {
            var start = symbols[unit].Offset;
            var limit = unit + 1 < symbols.Length ? symbols[unit + 1].Offset : (uint)code.Length;

            if (limit <= start || start > (uint)code.Length || limit > (uint)code.Length)
            {
                return new JsNativeScanResult(
                    JsNativeScanOutcome.OverlappingUnits,
                    start,
                    unit,
                    "code unit " + unit + " runs from " + start + " to " + limit +
                    ", which is not a range inside the emitted blob that later units do not overlap");
            }

            sequence?.Clear();

            var decoded = Decode(
                architecture, templates, code, start, limit, alignment, unit, starts, branches,
                instantiated, sequence);

            if (!decoded.Accepted)
            {
                return decoded;
            }

            if (sequence is not null)
            {
                var shaped = FrameShape(sequence, templates.Length, start, unit, frames!);

                if (!shaped.Accepted)
                {
                    return shaped;
                }
            }
        }

        var closed = Branches(code, symbols, starts, branches);

        return !closed.Accepted || frames is null ? closed : FrameTargets(branches, frames);
    }

    /// <summary>
    /// Holds one baseline unit to one prologue at its start and one epilogue at its end, and to
    /// neither sequence's instructions anywhere else.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>CLAUSE S1: THE FIRST SIX INSTANTIATIONS ARE THE PROLOGUE, IN ORDER.</b> Two pushes, the
    /// reservation, the frame into R14, the table base into RBX and the entry program counter into
    /// EAX. Nothing runs before the base is loaded, so no call can be made through an RBX the unit
    /// did not set.
    /// </para>
    /// <para>
    /// <b>CLAUSE S2: THE LAST FOUR ARE THE EPILOGUE, IN ORDER, AND NEITHER SEQUENCE'S TEMPLATES OCCUR
    /// ANYWHERE ELSE.</b> So RBX, R14 and RSP are written in exactly two places, each once, and the
    /// unit has exactly one return - which restores all three to what the caller left.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=2; Fingerprint=50C955
    // Broiler-Falsified-If: a baseline unit is accepted whose instantiations do not open with exactly the prologue, close with exactly the epilogue, or carry a prologue or epilogue template elsewhere
    // Broiler-Human:        PENDING
    private static JsNativeScanResult FrameShape(
        System.Collections.Generic.List<(uint At, int Index)> sequence,
        int tableLength,
        uint start,
        int unit,
        System.Collections.Generic.HashSet<uint> frames)
    {
        const int Prologue = JsNativeTemplates.X64BaselinePrologue;
        const int Epilogue = JsNativeTemplates.X64BaselineEpilogue;

        var epilogue = tableLength - Epilogue;
        var count = sequence.Count;

        if (count < Prologue + Epilogue)
        {
            return new JsNativeScanResult(
                JsNativeScanOutcome.FrameSequenceMalformed,
                start,
                unit,
                "code unit " + unit + " has " + count + " instructions, which is fewer than one " +
                "prologue and one epilogue");
        }

        for (var position = 0; position < count; position++)
        {
            var (at, index) = sequence[position];
            var tail = position - (count - Epilogue);

            if (position < Prologue)
            {
                if (index != position)
                {
                    return new JsNativeScanResult(
                        JsNativeScanOutcome.FrameSequenceMalformed,
                        at,
                        unit,
                        "instruction " + position + " of code unit " + unit + " is not the " +
                        "prologue's instruction " + position + ", so the unit does not open with " +
                        "the one sequence that saves what it writes and loads the handler table");
                }

                frames.Add(at);
                continue;
            }

            if (tail >= 0)
            {
                if (index != epilogue + tail)
                {
                    return new JsNativeScanResult(
                        JsNativeScanOutcome.FrameSequenceMalformed,
                        at,
                        unit,
                        "instruction " + position + " of code unit " + unit + " is not the " +
                        "epilogue's instruction " + tail + ", so the unit does not close with the " +
                        "one sequence that restores what its prologue saved");
                }

                // THE EPILOGUE'S FIRST INSTRUCTION IS A LANDING AND THE REST ARE NOT. Every exit
                // branches to the stack adjustment; a branch past it would pop what was not pushed.
                if (tail > 0)
                {
                    frames.Add(at);
                }

                continue;
            }

            if (index < Prologue || index >= epilogue)
            {
                return new JsNativeScanResult(
                    JsNativeScanOutcome.FrameSequenceMalformed,
                    at,
                    unit,
                    "instruction " + position + " of code unit " + unit + " is a prologue or " +
                    "epilogue instruction outside the unit's one prologue and one epilogue, so the " +
                    "stack pointer or a saved register is written somewhere the scan cannot pair");
            }
        }

        return Ok;
    }

    /// <summary>Refuses a baseline branch into a unit's prologue or past the start of its epilogue.</summary>
    /// <remarks>
    /// <b>CLAUSE S3.</b> The prologue runs once, at entry, and a branch back into it would load RBX
    /// and reserve stack a second time; an epilogue entered after its stack adjustment would pop
    /// registers the unit never pushed and return with the stack pointer where the caller did not
    /// leave it. The first instruction of the epilogue is where every exit is meant to land.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Critical; Resources=2; Fingerprint=6A1345
    // Broiler-Falsified-If: a baseline payload is accepted with a branch whose target is a prologue instruction, a pop or a return
    // Broiler-Human:        PENDING
    private static JsNativeScanResult FrameTargets(
        System.Collections.Generic.List<Branch> branches,
        System.Collections.Generic.HashSet<uint> frames)
    {
        foreach (var branch in branches)
        {
            if (branch.ToUnitEntry || !frames.Contains((uint)branch.Target))
            {
                continue;
            }

            return new JsNativeScanResult(
                JsNativeScanOutcome.BranchIntoFrameSequence,
                branch.Site,
                branch.Unit,
                "the `" + branch.Text + "` at " + branch.Site + " targets " + branch.Target +
                ", which is inside code unit " + branch.Unit + "'s prologue or past the first " +
                "instruction of its epilogue");
        }

        return Ok;
    }

    /// <summary>One branch site: where it is, where it goes, and which unit it belongs to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=635244
    // Broiler-Human:        PENDING
    private readonly record struct Branch(
        uint Site, long Target, int Unit, uint UnitStart, uint UnitLimit, bool ToUnitEntry, string Text);

    /// <summary>Decodes one unit linearly and records what it found.</summary>
    /// <remarks>
    /// <b>PADDING IS RECOGNISED ONLY AFTER A RETURN AND ONLY AS A WHOLE TRAILING RUN.</b> The
    /// x86-64 encoder aligns each unit's entry point by writing one-byte no-ops after the previous
    /// unit's last instruction, so a run of them at the end of a unit is expected and a no-op
    /// anywhere else is not. Admitting the padding byte as an ordinary template would admit it in
    /// the middle of a unit, where the encoder never writes one and where a decoder that met one
    /// would be re-synchronising against bytes nobody emitted.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=ACBBC0
    // Broiler-Human:        PENDING
    private static JsNativeScanResult Decode(
        JsNativeArchitecture architecture,
        JsNativeTemplate[] templates,
        byte[] code,
        uint start,
        uint limit,
        uint alignment,
        int unit,
        System.Collections.Generic.HashSet<uint> starts,
        System.Collections.Generic.List<Branch> branches,
        System.Collections.Generic.ICollection<string>? instantiated,
        System.Collections.Generic.List<(uint At, int Index)>? sequence)
    {
        var at = start;
        var padding = architecture != JsNativeArchitecture.Arm64;
        var lastWasReturn = false;

        while (at < limit)
        {
            if (padding && lastWasReturn && code[at] == JsNativeTemplates.X64PaddingByte)
            {
                return Padding(code, at, limit, alignment, unit);
            }

            var matched = Match(templates, code, at, limit, unit);

            if (!matched.Result.Accepted)
            {
                return matched.Result;
            }

            var template = matched.Template!;
            instantiated?.Add(template.Text);
            sequence?.Add((at, matched.Index));
            starts.Add(at);
            lastWasReturn = template.IsReturn;

            foreach (var field in template.Fields)
            {
                if (field.Kind is not (JsNativeFieldKind.UnitLocalBranch or JsNativeFieldKind.UnitEntryBranch))
                {
                    continue;
                }

                var displacement = Extract(code, at, field);

                // A REL32 IS MEASURED FROM THE END OF THE BRANCH INSTRUCTION and an A64 branch from
                // its first byte, which is the one arithmetic difference between the two
                // architectures' branches and the classic place to be wrong by an instruction.
                var origin = field.FromInstructionEnd ? at + (uint)template.Length : at;

                branches.Add(new Branch(
                    at,
                    origin + displacement,
                    unit,
                    start,
                    limit,
                    field.Kind == JsNativeFieldKind.UnitEntryBranch,
                    template.Text));
            }

            at += (uint)template.Length;
        }

        // A UNIT WITH NO INSTRUCTION AT ALL NEEDS NO CLAUSE OF ITS OWN, and the reason is worth a
        // sentence rather than a member of the vocabulary nothing reaches: a unit's range is
        // refused above unless it is non-empty, so the loop above ran at least once, and its first
        // pass cannot have been padding - padding is recognised only after a return. So the flag
        // below is the answer for an empty unit as well as for one that ends in the wrong
        // instruction, and there is no outcome of its own that nothing can produce.
        if (!lastWasReturn)
        {
            return new JsNativeScanResult(
                JsNativeScanOutcome.UnitDoesNotEndInAReturn,
                limit - 1,
                unit,
                "code unit " + unit + " ends without a return, so control would run off its last " +
                "instruction into whatever follows it");
        }

        return Ok;
    }

    /// <summary>Judges the bytes between a unit's last return and the next unit's entry point.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=7FB1A6
    // Broiler-Human:        PENDING
    private static JsNativeScanResult Padding(
        byte[] code, uint at, uint limit, uint alignment, int unit)
    {
        for (var index = at; index < limit; index++)
        {
            if (code[index] != JsNativeTemplates.X64PaddingByte)
            {
                return new JsNativeScanResult(
                    JsNativeScanOutcome.PaddingNotAlignment,
                    index,
                    unit,
                    "code unit " + unit + " is followed by a byte that is neither an instruction " +
                    "of this unit nor the one-byte no-op an entry point is aligned with");
            }
        }

        // THE RUN IS ALIGNMENT AND NOT A GAP, so it is shorter than the alignment it serves. A
        // longer run would be bytes an emitter had no reason to write, which is what a payload
        // carrying something between two units would look like.
        return limit - at >= alignment
            ? new JsNativeScanResult(
                JsNativeScanOutcome.PaddingNotAlignment,
                at,
                unit,
                "code unit " + unit + " is followed by " + (limit - at) + " no-op bytes, " +
                "which is more than the alignment of " + alignment + " this payload declares")
            : Ok;
    }

    /// <summary>Finds the one template that matches at <paramref name="at"/>, or says why none does.</summary>
    /// <remarks>
    /// <b>THE SHAPE AND THE OPERANDS ARE JUDGED SEPARATELY, because they are two different findings
    /// and a reader needs to be told which one it is.</b> Bytes matching no template's fixed bits
    /// are not code this build emits at all; bytes matching a template's fixed bits with an operand
    /// outside the set that template's field admits are an instruction this build's encoder can
    /// spell and its backends never ask for - a load from four gigabytes past the frame, say. The
    /// first is a forgery or a truncation; the second is the one a wrong producer would write.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B692D2
    // Broiler-Human:        PENDING
    private static (JsNativeScanResult Result, JsNativeTemplate? Template, int Index) Match(
        JsNativeTemplate[] templates, byte[] code, uint at, uint limit, int unit)
    {
        JsNativeTemplate? shaped = null;
        JsNativeTemplate? accepted = null;
        var acceptedIndex = -1;
        var shapedCrossed = false;
        var offending = JsNativeFieldKind.FrameField;
        var offendingValue = 0L;
        var matches = 0;

        for (var candidate = 0; candidate < templates.Length; candidate++)
        {
            var template = templates[candidate];

            if (!Shaped(template, code, at, limit, out var crossed))
            {
                continue;
            }

            if (crossed)
            {
                shaped ??= template;
                shapedCrossed = true;
                continue;
            }

            var admitted = true;

            foreach (var field in template.Fields)
            {
                var value = Extract(code, at, field);

                if (JsNativeTemplates.Admits(field.Kind, value))
                {
                    continue;
                }

                admitted = false;

                // THE FIRST SHAPE-MATCHING TEMPLATE IS THE ONE REPORTED, AND ITS FIELD IS THE ONE
                // REPORTED WITH IT. Recording the field of a later template beside the name of the
                // first would name an operand of an instruction the message does not mention.
                if (shaped is null)
                {
                    shaped = template;
                    offending = field.Kind;
                    offendingValue = value;
                }

                break;
            }

            if (!admitted)
            {
                continue;
            }

            accepted = template;
            acceptedIndex = candidate;
            matches++;
        }

        if (matches > 1)
        {
            return (
                new JsNativeScanResult(
                    JsNativeScanOutcome.AmbiguousTemplate,
                    at,
                    unit,
                    "the bytes at " + at + " match more than one template, so the byte belongs to " +
                    "two instantiations rather than to exactly one"),
                null,
                -1);
        }

        if (accepted is not null)
        {
            return (Ok, accepted, acceptedIndex);
        }

        if (shapedCrossed && shaped is not null)
        {
            return (
                new JsNativeScanResult(
                    JsNativeScanOutcome.InstructionCrossesUnitEnd,
                    at,
                    unit,
                    "the `" + shaped.Text + "` at " + at + " runs past the end of code unit " +
                    unit + ", so its last bytes belong to the unit that follows it"),
                null,
                -1);
        }

        if (shaped is not null)
        {
            return (
                new JsNativeScanResult(
                    JsNativeScanOutcome.OperandOutOfRange,
                    at,
                    unit,
                    "the `" + shaped.Text + "` at " + at + " carries " + offendingValue +
                    " where its " + offending + " field admits no such value, so it is an " +
                    "instruction this build can spell and no backend of it asks for"),
                null,
                -1);
        }

        return (
            new JsNativeScanResult(
                JsNativeScanOutcome.NoTemplate,
                at,
                unit,
                "the bytes at " + at + " match no instruction template this build's backends emit " +
                "for this architecture"),
            null,
            -1);
    }

    /// <summary>Whether a template's fixed bits match, and whether it would cross the unit's end.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=4446C3
    // Broiler-Human:        PENDING
    private static bool Shaped(
        JsNativeTemplate template, byte[] code, uint at, uint limit, out bool crossed)
    {
        crossed = at + (uint)template.Length > limit;
        var available = (int)System.Math.Min((uint)template.Length, limit - at);

        for (var index = 0; index < available; index++)
        {
            if ((byte)(code[at + index] & template.Mask[index]) != template.Fixed[index])
            {
                return false;
            }
        }

        // A template whose fixed bytes agree as far as the unit reaches is a match for the purpose
        // of REPORTING - it is what the producer meant to write - and never for the purpose of
        // accepting, which is what the flag above carries.
        return true;
    }

    /// <summary>Reads one field's value out of an instantiation.</summary>
    /// <remarks>
    /// <b>Bit offsets are counted from the first byte, little-endian within each byte, because both
    /// architectures store their instructions that way.</b> An A64 word is four little-endian bytes
    /// and an x86-64 displacement is four of them, so one extraction serves both and there is no
    /// second one to disagree with it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=2F54C0
    // Broiler-Human:        PENDING
    private static long Extract(byte[] code, uint at, JsNativeTemplateField field)
    {
        var value = 0UL;

        for (var bit = 0; bit < field.BitWidth; bit++)
        {
            var source = field.BitOffset + bit;
            var one = (code[at + (uint)(source / 8)] >> (source % 8)) & 1;
            value |= (ulong)one << bit;
        }

        if (field.Signed && field.BitWidth < 64 &&
            (value & (1UL << (field.BitWidth - 1))) != 0)
        {
            value |= ulong.MaxValue << field.BitWidth;
        }

        return (long)value * field.Scale;
    }

    /// <summary>Requires every branch to land on an instruction, and every call on an entry point.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=602614
    // Broiler-Human:        PENDING
    private static JsNativeScanResult Branches(
        byte[] code,
        JsNativeSymbolRow[] symbols,
        System.Collections.Generic.HashSet<uint> starts,
        System.Collections.Generic.List<Branch> branches)
    {
        foreach (var branch in branches)
        {
            if (branch.ToUnitEntry)
            {
                var found = false;

                foreach (var row in symbols)
                {
                    if (branch.Target == row.Offset)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    return new JsNativeScanResult(
                        JsNativeScanOutcome.CallNotAUnitEntry,
                        branch.Site,
                        branch.Unit,
                        "the `" + branch.Text + "` at " + branch.Site + " targets " + branch.Target +
                        ", which is not the entry point of any code unit this artifact declares");
                }

                continue;
            }

            if (branch.Target < branch.UnitStart || branch.Target >= branch.UnitLimit ||
                branch.Target >= code.Length)
            {
                return new JsNativeScanResult(
                    JsNativeScanOutcome.BranchLeavesUnit,
                    branch.Site,
                    branch.Unit,
                    "the `" + branch.Text + "` at " + branch.Site + " targets " + branch.Target +
                    ", which is outside the emitted range of code unit " + branch.Unit);
            }

            if (!starts.Contains((uint)branch.Target))
            {
                return new JsNativeScanResult(
                    JsNativeScanOutcome.BranchIntoInstruction,
                    branch.Site,
                    branch.Unit,
                    "the `" + branch.Text + "` at " + branch.Site + " targets " + branch.Target +
                    ", which is inside an instruction rather than the first byte of one");
            }
        }

        return Ok;
    }

    /// <summary>The answer every clause returns when it has nothing to say.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=398E63
    // Broiler-Human:        PENDING
    private static JsNativeScanResult Ok =>
        new(JsNativeScanOutcome.Accepted, 0, -1, "");
}
