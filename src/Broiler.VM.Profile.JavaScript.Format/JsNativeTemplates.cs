// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   32
// Annotated:        32/32
// Exempt:           19
// Human-reviewed:   0/32
// IP risk:          Low
// Security risk:    High
// Criteria:         2/2
// Resource impact:  3/10 max
// Unverified:       32
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>What one variable field of an instruction template is allowed to carry.</summary>
/// <remarks>
/// <para>
/// <b>A FIELD KIND IS A CLOSED SET OF VALUES AND NOT A WIDTH.</b> The width says how many bits an
/// operand occupies and says nothing whatever about which of them an emitter could have written;
/// the kind is the second half, and it is the half that makes a scan of emitted bytes worth
/// running. A displacement field that admitted every thirty-two-bit value would accept a load from
/// four gigabytes past the frame, which is a legal encoding, an impossible emission and exactly
/// the shape of byte a payload nobody generated would carry.
/// </para>
/// <para>
/// <b>Each kind names the emitter decision it corresponds to</b>, so that a reader who wants to
/// know why a value is admitted is directed at the backend that emits it rather than at a range.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B5D437
// Broiler-Human:        PENDING
public enum JsNativeFieldKind
{
    /// <summary>A displacement naming one declared field of <see cref="JsNativeFrame"/>.</summary>
    FrameField = 0,

    /// <summary>A byte displacement into a slab of <c>double</c>: non-negative and eight-aligned.</summary>
    SlabDisplacement = 1,

    /// <summary>A count of operand-slab slots, as a prologue subtracts and an epilogue adds back.</summary>
    SlotCount = 2,

    /// <summary>
    /// One of the three sixty-four-bit values an emitted unit materialises into its return
    /// register: the <c>undefined</c> pattern, a throw and a fuel exhaustion.
    /// </summary>
    MaterialisedValue = 3,

    /// <summary>
    /// One of the two sixty-four-bit bit patterns an emitted unit materialises into its scratch
    /// register to compare or exclusive-or against: the uninitialised pattern and the sign bit.
    /// </summary>
    MaterialisedPattern = 4,

    /// <summary>A value of <see cref="JsNativeReturn"/> an arm64 unit moves into its result register.</summary>
    ReturnCode = 5,

    /// <summary>An arm64 block's fuel charge, counted in bytecode instructions.</summary>
    BlockCost = 6,

    /// <summary>One halfword of a bytecode offset an arm64 unit records before it stops.</summary>
    OffsetHalfword = 7,

    /// <summary>An x86-64 condition code, as the low nibble of a conditional opcode.</summary>
    X64Condition = 8,

    /// <summary>An A64 condition code, as a <c>b.cond</c> carries it.</summary>
    Arm64Condition = 9,

    /// <summary>An A64 condition code inverted, as a <c>cset</c> carries it.</summary>
    Arm64InvertedCondition = 10,

    /// <summary>A branch displacement, which must land on an instruction of this same unit.</summary>
    UnitLocalBranch = 11,

    /// <summary>A call displacement, which must land on a code unit's entry point.</summary>
    UnitEntryBranch = 12,
}

/// <summary>One field of a template: where it sits, how wide it is, and what it may carry.</summary>
/// <remarks>
/// <b>The offset is counted in BITS from the first byte of the instantiation, and that is what lets
/// one record serve both architectures.</b> An x86-64 displacement occupies whole bytes and an A64
/// immediate occupies a bit range inside one little-endian word; a record that counted bytes would
/// need a second record for the second architecture, and two records of one idea is two places for
/// the extraction to be wrong.
/// </remarks>
/// <param name="Kind">The closed set of values this field may carry.</param>
/// <param name="BitOffset">Where the field starts, in bits from the instantiation's first byte.</param>
/// <param name="BitWidth">How many bits the field occupies.</param>
/// <param name="Signed">Whether the extracted bits are sign-extended before they are judged.</param>
/// <param name="Scale">
/// What the extracted value is multiplied by to become the value the emitter asked for. It is eight
/// for an A64 scaled load, four for a word branch displacement, and one everywhere else.
/// </param>
/// <param name="FromInstructionEnd">
/// Whether a branch displacement is measured from the end of the instruction rather than from its
/// first byte. It is true for every x86-64 <c>rel32</c> and false for every A64 branch, which is
/// the one difference between the two architectures' branch arithmetic and the classic place to
/// get it wrong by the length of the instruction.
/// </param>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=032419
// Broiler-Human:        PENDING
public readonly record struct JsNativeTemplateField(
    JsNativeFieldKind Kind,
    int BitOffset,
    int BitWidth,
    bool Signed,
    int Scale,
    bool FromInstructionEnd);

/// <summary>
/// One instruction template: the bytes that are fixed, the bits that are not, and what the ones
/// that are not may carry.
/// </summary>
/// <remarks>
/// <b>A TEMPLATE IS AN EMISSION AND NOT AN INSTRUCTION.</b> The architectures both encode far more
/// than this profile's backends write, and the table these belong to enumerates what the backends
/// write - one entry per byte sequence an encoder can produce, with the registers already resolved
/// to the ones the backend actually names. That is why <c>mov</c> appears here a dozen times and
/// why an encoding the manual admits but no backend emits appears not at all.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=340610
// Broiler-Human:        PENDING
public sealed class JsNativeTemplate
{
    /// <summary>Creates a template from a byte pattern, masking out every field's bits.</summary>
    /// <remarks>
    /// <b>The mask is DERIVED from the fields rather than written beside them</b>, so a template
    /// whose field moved cannot keep matching against bits that field now occupies. The pattern is
    /// masked as well, so a pattern byte carrying rubbish under a field is rubbish that can never
    /// be compared against anything.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=DF7BDF
    // Broiler-Human:        PENDING
    public JsNativeTemplate(
        string text, byte[] pattern, bool isReturn, params JsNativeTemplateField[] fields)
    {
        var mask = new byte[pattern.Length];
        var fixedBytes = new byte[pattern.Length];

        for (var index = 0; index < pattern.Length; index++)
        {
            mask[index] = 0xFF;
        }

        foreach (var field in fields)
        {
            for (var bit = field.BitOffset; bit < field.BitOffset + field.BitWidth; bit++)
            {
                mask[bit / 8] &= (byte)~(1 << (bit % 8));
            }
        }

        for (var index = 0; index < pattern.Length; index++)
        {
            fixedBytes[index] = (byte)(pattern[index] & mask[index]);
        }

        Text = text;
        Fixed = fixedBytes;
        Mask = mask;
        Fields = fields;
        IsReturn = isReturn;
    }

    /// <summary>The instruction as the encoder's own method names it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=09D21C
    // Broiler-Human:        PENDING
    public string Text { get; }

    /// <summary>The bytes this template fixes, with every field's bits cleared.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=D1032F
    // Broiler-Human:        PENDING
    public byte[] Fixed { get; }

    /// <summary>Which bits are fixed: a set bit is compared, a clear bit belongs to a field.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=402373
    // Broiler-Human:        PENDING
    public byte[] Mask { get; }

    /// <summary>The variable fields, in no significant order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=3ABA9F
    // Broiler-Human:        PENDING
    public JsNativeTemplateField[] Fields { get; }

    /// <summary>Whether this template returns to the caller.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=BCC877
    // Broiler-Human:        PENDING
    public bool IsReturn { get; }

    /// <summary>How many bytes one instantiation occupies.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B6C868
    // Broiler-Human:        PENDING
    public int Length => Fixed.Length;
}

/// <summary>
/// The closed enumeration of instruction templates this profile's backends emit, per architecture.
/// </summary>
/// <remarks>
/// <para>
/// <b>THIS TABLE IS THE AUTHORITY BOTH SIDES ANSWER TO, AND IT LIVES IN THE FORMAT ASSEMBLY FOR
/// THE REASON THE FORMAT ASSEMBLY EXISTS.</b> A compiler and an executor must agree about what a
/// payload is and neither may depend on the other: the lowering emits from an encoder, the
/// verifier scans against this table, and the only thing that makes the scan mean anything is that
/// the two answer to one written-down enumeration rather than to each other. Put the table beside
/// the encoders and an execution-only image cannot reach it; put it in the profile and the
/// encoders cannot; the format is the pivot both already depend on.
/// </para>
/// <para>
/// <b>THE ENUMERATION IS CLOSED, AND THAT CLOSURE IS THE WHOLE ARGUMENT FOR THE SCAN.</b> The
/// scanner accepts a byte only where it belongs to an instantiation of one of these templates with
/// every field in range, so the set of payloads it accepts is exactly the set of byte sequences
/// these templates generate. The backends emit only through the encoders' named methods, one
/// method per template here, with the registers those methods are called with fixed by the
/// backend - so every sequence a backend can emit is a sequence of instantiations from this table,
/// which is the direction a lane check exercises, and no sequence outside the table is accepted,
/// which is the direction this closure argument is. NEITHER HALF SAYS THE INSTRUCTIONS ARE THE
/// RIGHT ONES: a well-formed sequence of the wrong templates is well formed, and re-emission
/// equality is the only layer that reaches the generator.
/// </para>
/// <para>
/// <b>WHAT IS DELIBERATELY ABSENT IS AS MUCH A PART OF THE TABLE AS WHAT IS PRESENT.</b> The
/// x86-64 encoder can write an indirect <c>call r64</c> and the backend emits none - the encoder
/// carries it for the trampoline that stands between a caller and an emitted entry point - so
/// there is no template for it here, and a payload carrying one is refused. The same holds for the
/// encoder's raw byte doors, which exist for tests that write a deliberately wrong word. A table
/// that admitted a form no backend emits would be a table that admits an indirect transfer of
/// control into a page this profile is about to make executable.
/// </para>
/// <para>
/// <b>A CHANGE TO A TEMPLATE IN AN ENCODER IS A CHANGE HERE AND A CHANGE TO THE BACKEND'S
/// VERSION.</b> The backends' semantic version travels in every artifact and a verifier refuses a
/// payload whose version it is not; this table is the third record of the same fact, and the lane
/// check that compiles real programs and scans their images is what stops the three parting
/// company quietly.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=14F65E
// Broiler-Falsified-If: a template here differs from the bytes the encoder method it names emits, or a byte sequence a backend emits matches no template here
// Broiler-Human:        PENDING
public static class JsNativeTemplates
{
    /// <summary>The byte an x86-64 emission pads with between one unit's end and the next's entry.</summary>
    /// <remarks>
    /// <b>It is a one-byte no-op and never a zero.</b> A run of zero bytes decodes as
    /// <c>add [rax], al</c>, which faults on a null base and stores through any other; the encoder
    /// pads with <c>0x90</c> for that reason, and a scan that accepted a zero as padding would
    /// accept the one byte sequence the padding was chosen to avoid.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=EB14F0
    // Broiler-Human:        PENDING
    public const byte X64PaddingByte = 0x90;

    /// <summary>The furthest into a slab of <c>double</c> any emitted instruction may reach.</summary>
    /// <remarks>
    /// <b>It is derived from the format's own ceilings and is not a policy of its own.</b> A unit's
    /// region is its flattened scope slots plus its declared operand stack, and a call stages its
    /// arguments immediately past that region, so the furthest displacement any emitted unit can
    /// carry is bounded by the sum of the three ceilings the format already publishes, in bytes.
    /// A tighter bound would have to be per-unit, and the two counts a per-unit bound needs - a
    /// flattened scope-slot count and a caller's region size - are not carried by the artifact.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=53D952
    // Broiler-Human:        PENDING
    public const long CeilingSlabDisplacementBytes =
        ((long)JsFormat.CeilingScopeSlots + JsFormat.CeilingOperandStack + JsFormat.CeilingCallArguments) * 8;

    /// <summary>The most operand-slab slots one unit's region can take.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=12973D
    // Broiler-Human:        PENDING
    public const long CeilingRegionSlots =
        (long)JsFormat.CeilingScopeSlots + JsFormat.CeilingOperandStack;

    /// <summary>The most an arm64 block charges in one <c>subs</c>.</summary>
    /// <remarks>
    /// It is the reach of the twelve-bit immediate that instruction carries. A block costing more
    /// is charged by repeated subtraction rather than by a wider form, so this is the field's own
    /// range and not a bound on a block.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=14F2FD
    // Broiler-Human:        PENDING
    public const long CeilingBlockCharge = 4095;

    /// <summary>Where in its own stack frame an emitted x86-64 unit spills the frame pointer.</summary>
    /// <remarks>
    /// <b>Above the shadow space, because the shadow space belongs to the callee of the next
    /// call.</b> Windows x64 makes a caller reserve thirty-two bytes and System V none, so the
    /// slot is thirty-two on the one and zero on the other. The convention table in the lowering
    /// assembly states the same two numbers; the lane check beside the backends compares them,
    /// because two records of one fact can part company.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=80B443
    // Broiler-Human:        PENDING
    public static int X64FramePointerSlot(JsNativeArchitecture architecture) =>
        architecture == JsNativeArchitecture.X64Windows ? 32 : 0;

    /// <summary>How many bytes an emitted x86-64 unit's prologue reserves.</summary>
    /// <remarks>
    /// The shadow space this convention makes the caller reserve, plus the eight bytes the frame
    /// pointer is spilled into, rounded up to a multiple of sixteen: forty-eight under Windows x64
    /// and sixteen under System V. A <c>call</c> leaves the stack pointer eight past a sixteen-byte
    /// boundary and one <c>push</c> brings it back, so the reservation must itself be a multiple of
    /// sixteen for the next <c>call</c> to be made from a boundary.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=47E17A
    // Broiler-Human:        PENDING
    public static int X64FrameBytes(JsNativeArchitecture architecture) =>
        architecture == JsNativeArchitecture.X64Windows ? 48 : 16;

    /// <summary>The register the single frame-pointer argument arrives in, by its own number.</summary>
    /// <remarks>
    /// RCX is one and RDI is seven, and the disagreement between the two conventions about which
    /// of them carries the argument is why the architecture rather than the instruction set is what
    /// an artifact names.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A62340
    // Broiler-Human:        PENDING
    public static int X64FramePointerRegister(JsNativeArchitecture architecture) =>
        architecture == JsNativeArchitecture.X64Windows ? 1 : 7;

    /// <summary>The templates for <paramref name="architecture"/>, or an empty table for none.</summary>
    /// <remarks>
    /// <b>An empty table is not an accepting one.</b> A scan against no templates accepts no byte,
    /// so an architecture this build does not enumerate refuses every payload rather than admitting
    /// one - which is the answer a table-driven check owes for an instruction set it has never
    /// been written against.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=938543
    // Broiler-Human:        PENDING
    public static JsNativeTemplate[] For(JsNativeArchitecture architecture) => architecture switch
    {
        JsNativeArchitecture.X64Windows => windows,
        JsNativeArchitecture.X64SystemV => systemV,
        JsNativeArchitecture.Arm64 => arm64,
        _ => [],
    };

    /// <summary>Whether <paramref name="value"/> is one this kind of field may carry.</summary>
    /// <remarks>
    /// <b>EVERY ARM OF THIS METHOD IS A CLOSED SET AND NONE OF THEM IS A WIDTH.</b> Each names the
    /// emitter decision that fixes it: the frame's declared field offsets, the eight-byte grid a
    /// slab of doubles is addressed on, the five bit patterns an emitted unit materialises, the
    /// conditions the two encoders' condition enumerations spell. A value outside one of these
    /// sets is a value no backend of this build could have written, whatever the architecture would
    /// make of it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=CEE484
    // Broiler-Falsified-If: a value no backend of this build asks an encoder for is admitted by one of these arms
    // Broiler-Human:        PENDING
    public static bool Admits(JsNativeFieldKind kind, long value) => kind switch
    {
        // THE FRAME'S OWN DECLARED OFFSETS, and nothing between them. The structure is sequential
        // and its seven fields sit at these seven offsets on a 64-bit target; a displacement that
        // named a byte between two of them would be reading half of one field and half of another.
        JsNativeFieldKind.FrameField =>
            value is 0 or 8 or 16 or 24 or 32 or 40 or 48,

        JsNativeFieldKind.SlabDisplacement =>
            value >= 0 && (value % 8) == 0 && value <= CeilingSlabDisplacementBytes,

        JsNativeFieldKind.SlotCount =>
            value >= 0 && value <= CeilingRegionSlots,

        // THE THREE VALUES A UNIT MOVES INTO ITS RETURN REGISTER. `undefined` is a reserved quiet
        // NaN, a throw is two and a fuel exhaustion is three; one is deliberately absent, because
        // there is no bail-out to an interpreter for it to mean.
        JsNativeFieldKind.MaterialisedValue =>
            value == JsNativeValues.UndefinedBits ||
            value == (long)JsNativeReturn.Threw ||
            value == (long)JsNativeReturn.FuelExhausted,

        // THE TWO PATTERNS A UNIT MOVES INTO ITS SCRATCH REGISTER. The uninitialised pattern is
        // what a temporal-dead-zone check compares against; the sign bit is what a negation
        // exclusive-ors with.
        JsNativeFieldKind.MaterialisedPattern =>
            value == JsNativeValues.UninitialisedBits || value == long.MinValue,

        JsNativeFieldKind.ReturnCode =>
            value == (long)JsNativeReturn.Returned || value == (long)JsNativeReturn.FuelExhausted,

        JsNativeFieldKind.BlockCost =>
            value >= 1 && value <= CeilingBlockCharge,

        JsNativeFieldKind.OffsetHalfword =>
            value is >= 0 and <= 65535,

        // THE SEVEN THE x86-64 ENCODER'S CONDITION ENUMERATION SPELLS: equal, not equal, above,
        // above or equal, parity, no parity and signed less than. Every one of them is reached
        // through the same nibble by a jump and by a set, which is why one set serves both.
        JsNativeFieldKind.X64Condition =>
            value is 0x3 or 0x4 or 0x5 or 0x7 or 0xA or 0xB or 0xC,

        // THE SIX THE A64 ENCODER'S CONDITION ENUMERATION SPELLS: eq, ne, mi, vs, ge and gt. The
        // four that answer false for an unordered compare are the reason the obvious ones - lt and
        // le - are absent from the encoder and therefore from here.
        JsNativeFieldKind.Arm64Condition =>
            value is 0 or 1 or 4 or 6 or 10 or 12,

        // The same six, inverted, because `cset` increments when its condition FAILS and inverting
        // an A64 condition is exclusive-or with one.
        JsNativeFieldKind.Arm64InvertedCondition =>
            value is 1 or 0 or 5 or 7 or 11 or 13,

        // A BRANCH IS JUDGED BY THE SCANNER AND NOT HERE, because whether a displacement is
        // admissible is a question about the unit it sits in and this method has no unit. Admitting
        // it here and refusing it there is what keeps the two questions apart.
        JsNativeFieldKind.UnitLocalBranch or JsNativeFieldKind.UnitEntryBranch => true,

        _ => false,
    };

    // ---- the x86-64 tables ---------------------------------------------------------------------

    /// <summary>The table for x86-64 under the Windows x64 convention.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=A7E1AF
    // Broiler-Human:        PENDING
    private static readonly JsNativeTemplate[] windows = X64(JsNativeArchitecture.X64Windows);

    /// <summary>The table for x86-64 under the System V AMD64 convention.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=363F85
    // Broiler-Human:        PENDING
    private static readonly JsNativeTemplate[] systemV = X64(JsNativeArchitecture.X64SystemV);

    /// <summary>
    /// Every byte sequence the x86-64 backend emits, for one of the two calling conventions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THE TWO CONVENTIONS DIFFER IN THREE CONSTANTS AND IN NOTHING ELSE, so they are one table
    /// with three parameters rather than two tables.</b> The frame pointer arrives in RCX or in
    /// RDI, the stack frame is forty-eight bytes or sixteen, and the spill slot is at thirty-two or
    /// at zero. Every other byte of every other template is the same, which is a fact worth being
    /// able to see: a second table would let the two drift apart in a template neither convention
    /// disagrees about.
    /// </para>
    /// <para>
    /// <b>Every displacement is thirty-two bits and every branch is a <c>rel32</c>, because the
    /// encoder writes no shorter form.</b> A one-byte displacement is correct until a later edit
    /// pushes its target past a hundred and twenty-seven bytes, and the failure is a branch into
    /// the middle of an instruction; the encoder refuses to be tempted and so does this table.
    /// </para>
    /// <para>
    /// <b>The derivation is beside each row and the hexadecimal is the summary.</b> A REX prefix is
    /// <c>0x40</c> plus eight for a sixty-four-bit operand, four for a register field reaching the
    /// upper eight, and one for a base register doing the same; a ModRM byte for a register pair is
    /// <c>0xC0</c> or the two three-bit numbers; a ModRM byte for a memory operand with a
    /// thirty-two-bit displacement is <c>0x80</c> or the same, and a base whose low three bits are
    /// <c>100</c> takes the SIB byte <c>0x24</c> after it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=DF508B
    // Broiler-Human:        PENDING
    private static JsNativeTemplate[] X64(JsNativeArchitecture architecture)
    {
        var frame = (byte)X64FramePointerRegister(architecture);
        var spill = X64FramePointerSlot(architecture);
        var frameBytes = X64FrameBytes(architecture);

        // ModRM for `[rsp+disp32]` with the frame-pointer register in the reg field: 0x80 or the
        // register's low three bits shifted three, or 100 for "a SIB byte follows".
        var spillModRm = (byte)(0x80 | ((frame & 7) << 3) | 4);

        // ModRM for `r11, frame` as a register pair: 0xC0 or the source shifted three, or R11's low
        // three bits, which are 011.
        var moveModRm = (byte)(0xC0 | ((frame & 7) << 3) | 3);

        return
        [
            // push rbx: 50+r with r = 3. The REX prefix would be a bare 0x40 and the encoder writes
            // none, because an unnecessary prefix is a byte two encoders can disagree about.
            Plain("push rbx", [0x53]),

            // pop rbx: 58+r with r = 3.
            Plain("pop rbx", [0x5B]),

            // ret: C3, with no immediate form anywhere in the encoder. Both x86-64 conventions are
            // caller-pops, so there is no count for a callee to get wrong.
            Terminal("ret", [0xC3]),

            // sub rsp, imm32: REX.W 81 /5 with the frame size fixed by the convention.
            Plain("sub rsp, " + frameBytes, [0x48, 0x81, 0xEC, .. Int32(frameBytes)]),

            // add rsp, imm32: REX.W 81 /0, the same amount every epilogue gives back.
            Plain("add rsp, " + frameBytes, [0x48, 0x81, 0xC4, .. Int32(frameBytes)]),

            // mov [rsp+spill], frame: REX.W 89 /r over a SIB-based memory operand.
            Plain("mov [rsp+" + spill + "], frame", [0x48, 0x89, spillModRm, 0x24, .. Int32(spill)]),

            // mov frame, [rsp+spill]: REX.W 8B /r, the reload every call site and every epilogue
            // does rather than trusting a register System V calls volatile.
            Plain("mov frame, [rsp+" + spill + "]", [0x48, 0x8B, spillModRm, 0x24, .. Int32(spill)]),

            // mov r11, [rsp+spill]: REX.WR 8B /r - 0x4C because R11 is in the reg field.
            Plain("mov r11, [rsp+" + spill + "]", [0x4C, 0x8B, 0x9C, 0x24, .. Int32(spill)]),

            // mov r11, frame: REX.WB 89 /r - 0x49 because R11 is in the r/m field.
            Plain("mov r11, frame", [0x49, 0x89, moveModRm]),

            // mov rax, [r11+field]: REX.WB 8B /r, ModRM 0x83 = 0x80 | (0 << 3) | 011.
            Field("mov rax, [r11+field]", [0x49, 0x8B, 0x83, 0, 0, 0, 0], Disp32(3, JsNativeFieldKind.FrameField)),

            // mov rbx, [r11+field]: ModRM 0x9B = 0x80 | (011 << 3) | 011.
            Field("mov rbx, [r11+field]", [0x49, 0x8B, 0x9B, 0, 0, 0, 0], Disp32(3, JsNativeFieldKind.FrameField)),

            // mov [r11+field], rax: REX.WB 89 /r.
            Field("mov [r11+field], rax", [0x49, 0x89, 0x83, 0, 0, 0, 0], Disp32(3, JsNativeFieldKind.FrameField)),

            // mov [r11+field], rbx.
            Field("mov [r11+field], rbx", [0x49, 0x89, 0x9B, 0, 0, 0, 0], Disp32(3, JsNativeFieldKind.FrameField)),

            // add dword [r11+field], imm32: 81 /0 with REX.B and no REX.W, because the field it
            // edits is a four-byte count and a sixty-four-bit add would write over the pointer that
            // follows it.
            Field(
                "add dword [r11+field], slots",
                [0x41, 0x81, 0x83, 0, 0, 0, 0, 0, 0, 0, 0],
                Disp32(3, JsNativeFieldKind.FrameField),
                Imm32(7, JsNativeFieldKind.SlotCount)),

            // sub dword [r11+field], imm32: 81 /5, the prologue's half of the same edit.
            Field(
                "sub dword [r11+field], slots",
                [0x41, 0x81, 0xAB, 0, 0, 0, 0, 0, 0, 0, 0],
                Disp32(3, JsNativeFieldKind.FrameField),
                Imm32(7, JsNativeFieldKind.SlotCount)),

            // mov rdx, [rbx+slab]: REX.W 8B /r, ModRM 0x93 = 0x80 | (010 << 3) | 011.
            Field("mov rdx, [rbx+slab]", [0x48, 0x8B, 0x93, 0, 0, 0, 0], Disp32(3, JsNativeFieldKind.SlabDisplacement)),

            // mov [rbx+slab], rdx.
            Field("mov [rbx+slab], rdx", [0x48, 0x89, 0x93, 0, 0, 0, 0], Disp32(3, JsNativeFieldKind.SlabDisplacement)),

            // mov [rbx+slab], rax.
            Field("mov [rbx+slab], rax", [0x48, 0x89, 0x83, 0, 0, 0, 0], Disp32(3, JsNativeFieldKind.SlabDisplacement)),

            // lea rax, [rbx+slab]: REX.W 8D /r, the one address arithmetic an emitted unit does -
            // the base of the region the callee will use.
            Field("lea rax, [rbx+slab]", [0x48, 0x8D, 0x83, 0, 0, 0, 0], Disp32(3, JsNativeFieldKind.SlabDisplacement)),

            // mov rdx, [rax+slab]: ModRM 0x90 = 0x80 | (010 << 3) | 000, over a slab pointer the
            // instruction before this one loaded out of the frame.
            Field("mov rdx, [rax+slab]", [0x48, 0x8B, 0x90, 0, 0, 0, 0], Disp32(3, JsNativeFieldKind.SlabDisplacement)),

            // mov [rax+slab], rdx.
            Field("mov [rax+slab], rdx", [0x48, 0x89, 0x90, 0, 0, 0, 0], Disp32(3, JsNativeFieldKind.SlabDisplacement)),

            // dec qword [rax+0]: REX.W FF /1 with a displacement of zero, over the fuel counter
            // itself. The one-byte dec forms do not exist in 64-bit mode.
            Plain("dec qword [rax+0]", [0x48, 0xFF, 0x88, 0x00, 0x00, 0x00, 0x00]),

            // mov rax, imm64: REX.W B8+r and eight bytes, one of the three values a unit answers.
            Field(
                "mov rax, imm64",
                [0x48, 0xB8, 0, 0, 0, 0, 0, 0, 0, 0],
                Imm64(2, JsNativeFieldKind.MaterialisedValue)),

            // mov r10, imm64: REX.WB BA+r, one of the two patterns a unit compares or flips against.
            Field(
                "mov r10, imm64",
                [0x49, 0xBA, 0, 0, 0, 0, 0, 0, 0, 0],
                Imm64(2, JsNativeFieldKind.MaterialisedPattern)),

            // xor rax, rax: REX.W 31 /r, the zero a normal return answers with.
            Plain("xor rax, rax", [0x48, 0x31, 0xC0]),

            // xor rdx, r10: REX.WR 31 /r - the sign flip a negation is.
            Plain("xor rdx, r10", [0x4C, 0x31, 0xD2]),

            // cmp rdx, r10: REX.WR 39 /r - the temporal-dead-zone comparison.
            Plain("cmp rdx, r10", [0x4C, 0x39, 0xD2]),

            // test eax, eax: 85 /r with no REX at all, over the answer a callee left.
            Plain("test eax, eax", [0x85, 0xC0]),

            // setcc al: 0F 90+cc /0. The nibble is the condition and the ModRM byte is 0xC0.
            Field("setcc al", [0x0F, 0x90, 0xC0], Nibble(8, JsNativeFieldKind.X64Condition)),

            // setcc dl: the same over 0xC2.
            Field("setcc dl", [0x0F, 0x90, 0xC2], Nibble(8, JsNativeFieldKind.X64Condition)),

            // and al, dl: 20 /r, ModRM 0xD0 = 0xC0 | (010 << 3) | 000.
            Plain("and al, dl", [0x20, 0xD0]),

            // or al, dl: 08 /r.
            Plain("or al, dl", [0x08, 0xD0]),

            // movzx eax, al: 0F B6 /r, which is what makes a condition byte a zero or a one.
            Plain("movzx eax, al", [0x0F, 0xB6, 0xC0]),

            // jmp rel32: E9 id. There is no two-byte EB form in this encoder.
            Field("jmp rel32", [0xE9, 0, 0, 0, 0], Rel32(1, JsNativeFieldKind.UnitLocalBranch)),

            // jcc rel32: 0F 80+cc id. There is no two-byte 7x form either.
            Field(
                "jcc rel32",
                [0x0F, 0x80, 0, 0, 0, 0],
                Nibble(8, JsNativeFieldKind.X64Condition),
                Rel32(2, JsNativeFieldKind.UnitLocalBranch)),

            // call rel32: E8 id, and never an indirect form. Every call an emitted artifact makes
            // is a direct branch to a code unit of the same artifact, at a displacement fixed when
            // the artifact was compiled.
            Field("call rel32", [0xE8, 0, 0, 0, 0], Rel32(1, JsNativeFieldKind.UnitEntryBranch)),

            // movsd xmm0, [rbx+slab]: F2 0F 10 /r. The F2 is a mandatory prefix and comes BEFORE
            // any REX; here there is no REX, because every register is in the lower eight.
            Field(
                "movsd xmm0, [rbx+slab]",
                [0xF2, 0x0F, 0x10, 0x83, 0, 0, 0, 0],
                Disp32(4, JsNativeFieldKind.SlabDisplacement)),

            // movsd xmm1, [rbx+slab]: ModRM 0x8B = 0x80 | (001 << 3) | 011.
            Field(
                "movsd xmm1, [rbx+slab]",
                [0xF2, 0x0F, 0x10, 0x8B, 0, 0, 0, 0],
                Disp32(4, JsNativeFieldKind.SlabDisplacement)),

            // movsd [rbx+slab], xmm0: F2 0F 11 /r.
            Field(
                "movsd [rbx+slab], xmm0",
                [0xF2, 0x0F, 0x11, 0x83, 0, 0, 0, 0],
                Disp32(4, JsNativeFieldKind.SlabDisplacement)),

            // addsd, subsd, mulsd, divsd xmm0, xmm1: F2 0F 58/5C/59/5E /r with ModRM 0xC1.
            Plain("addsd xmm0, xmm1", [0xF2, 0x0F, 0x58, 0xC1]),
            Plain("subsd xmm0, xmm1", [0xF2, 0x0F, 0x5C, 0xC1]),
            Plain("mulsd xmm0, xmm1", [0xF2, 0x0F, 0x59, 0xC1]),
            Plain("divsd xmm0, xmm1", [0xF2, 0x0F, 0x5E, 0xC1]),

            // ucomisd xmm0, xmm1 and its swapped form: 66 0F 2E /r. It sets the parity flag when
            // either operand is NaN, which every comparison this backend emits reads.
            Plain("ucomisd xmm0, xmm1", [0x66, 0x0F, 0x2E, 0xC1]),
            Plain("ucomisd xmm1, xmm0", [0x66, 0x0F, 0x2E, 0xC8]),

            // xorpd xmm1, xmm1: 66 0F 57 /r, the zero a truthiness test compares against.
            Plain("xorpd xmm1, xmm1", [0x66, 0x0F, 0x57, 0xC9]),

            // cvtsi2sd xmm0, rax: F2 REX.W 0F 2A /r - it CONVERTS, which turns the zero or one a
            // setcc produced into the 0.0 or 1.0 a Boolean is in a slab of doubles.
            Plain("cvtsi2sd xmm0, rax", [0xF2, 0x48, 0x0F, 0x2A, 0xC0]),

            // movq xmm0, rax: 66 REX.W 0F 6E /r - it moves BITS, which is how the undefined pattern
            // reaches the register a return leaves its value in.
            Plain("movq xmm0, rax", [0x66, 0x48, 0x0F, 0x6E, 0xC0]),
        ];
    }

    // ---- the arm64 table -----------------------------------------------------------------------

    /// <summary>
    /// Every instruction word the arm64 backend emits, one template each.
    /// </summary>
    /// <remarks>
    /// <b>EVERY A64 INSTRUCTION IS FOUR BYTES AND THE BACKEND PADS NOTHING, so this table has no
    /// padding row and the scan admits no byte between one unit and the next.</b> The alignment
    /// that backend declares is four, every instruction is four bytes, and a unit that follows
    /// another is therefore already aligned - which is why an emitted arm64 section is exactly the
    /// concatenation of its units with nothing whatever between them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=BD036C
    // Broiler-Human:        PENDING
    private static readonly JsNativeTemplate[] arm64 =
    [
        // stp x29, x30, [sp, #-64]!  -- 0xA9800000 | imm7 = -8 (0x3C0000) | Rt2 = 30 (0x7800) |
        // Rn = 31 (0x3E0) | Rt = 29.
        Word("stp x29, x30, [sp, #-64]!", 0xA9BC7BFDu),

        // stp x19, x20, [sp, #16]  -- 0xA9000000 | imm7 = 2 | Rt2 = 20 | Rn = 31 | Rt = 19.
        Word("stp x19, x20, [sp, #16]", 0xA90153F3u),

        // stp x21, x22, [sp, #32]  -- imm7 = 4 (0x20000) | Rt2 = 22 (0x5800) | Rt = 21.
        Word("stp x21, x22, [sp, #32]", 0xA9025BF5u),

        // str x23, [sp, #48]  -- 0xF9000000 | imm12 = 6 (0x1800) | Rn = 31 | Rt = 23.
        Word("str x23, [sp, #48]", 0xF9001BF7u),

        // mov x29, sp  -- ADD (immediate) with imm12 = 0, Rn = 31 the STACK POINTER.
        Word("mov x29, sp", 0x910003FDu),

        // mov x22, x0  -- ORR (shifted register) with Rn = 31 the ZERO register: the frame pointer
        // the caller passed, moved into the callee-saved register that holds it.
        Word("mov x22, x0", 0xAA0003F6u),

        // The prologue's four reads of the frame, at the frame's own declared offsets:
        // ldr x19, [x22, #0], ldr x20, [x22, #16], ldr x21, [x22, #32], ldr x23, [x22, #40].
        Word("ldr x19, [x22, #0]", 0xF94002D3u),
        Word("ldr x20, [x22, #16]", 0xF9400AD4u),
        Word("ldr x21, [x22, #32]", 0xF94012D5u),
        Word("ldr x23, [x22, #40]", 0xF94016D7u),

        // The epilogue restores what the prologue saved, and the stack pointer is restored by the
        // WRITEBACK of the last load pair and by nothing else - which is why the three loads before
        // it read positive offsets from the stack pointer the prologue left. It is also why A64 has
        // no spelling for the `ret 8` defect: the amount is part of a load and not of the return.
        // ldr x23, [sp, #48], ldp x21, x22, [sp, #32], ldp x19, x20, [sp, #16] and
        // ldp x29, x30, [sp], #64.
        Word("ldr x23, [sp, #48]", 0xF9401BF7u),
        Word("ldp x21, x22, [sp, #32]", 0xA9425BF5u),
        Word("ldp x19, x20, [sp, #16]", 0xA94153F3u),
        Word("ldp x29, x30, [sp], #64", 0xA8C47BFDu),

        // The fuel counter is read and written through the register that holds its address:
        // ldr x9, [x23, #0] and str x9, [x23, #0].
        Word("ldr x9, [x23, #0]", 0xF94002E9u),
        Word("str x9, [x23, #0]", 0xF90002E9u),

        // subs x9, x9, #imm12  -- 0xF1000000 | imm12 << 10 | Rn = 9 | Rd = 9. The immediate is the
        // block's cost in bytecode instructions; a block costing more than the field carries is
        // charged by repeated subtraction rather than by a wider form.
        Bits("subs x9, x9, #cost", 0xF1000129u, new JsNativeTemplateField(
            JsNativeFieldKind.BlockCost, 10, 12, false, 1, false)),

        // str w9, [x22, #48]  -- the bytecode offset a unit records before it stops. size = 10 and
        // the immediate scales by FOUR here rather than by eight.
        Word("str w9, [x22, #48]", 0xB90032C9u),

        // movz w9, #imm16  -- 0x52800000 | imm16 << 5 | Rd = 9: the low halfword of that offset.
        Bits("movz w9, #imm16", 0x52800009u, new JsNativeTemplateField(
            JsNativeFieldKind.OffsetHalfword, 5, 16, false, 1, false)),

        // movk w9, #imm16, lsl #16  -- 0x72A00000 | imm16 << 5 | Rd = 9: its high halfword, KEEPING
        // the bits it does not write.
        Bits("movk w9, #imm16, lsl #16", 0x72A00009u, new JsNativeTemplateField(
            JsNativeFieldKind.OffsetHalfword, 5, 16, false, 1, false)),

        // movz w0, #imm16  -- the return code a unit answers with, in the register AAPCS64 answers
        // in. Only two of the three are reachable here: this backend emits no throw.
        Bits("movz w0, #code", 0x52800000u, new JsNativeTemplateField(
            JsNativeFieldKind.ReturnCode, 5, 16, false, 1, false)),

        // movz x9, #0x7FF8, lsl #48  -- the high halfword of a quiet NaN, which is how this backend
        // spells `undefined` in a slab of doubles.
        Word("movz x9, #0x7FF8, lsl #48", 0xD2EFFF09u),

        // fmov d0, x9  -- it moves BITS and does not convert, which is what makes that NaN a NaN.
        Word("fmov d0, x9", 0x9E670120u),

        // scvtf d0, x9  -- it CONVERTS, which turns the zero or one a cset produced into 0.0 or 1.0.
        Word("scvtf d0, x9", 0x9E620120u),

        // cset x9, cond and cset x10, cond  -- CSINC Xd, xzr, xzr, invert(cond). THE CONDITION IN
        // THE WORD IS THE INVERSE of the one asked for, because the instruction increments when its
        // condition fails.
        Bits("cset x9, cond", 0x9A9F07E9u, new JsNativeTemplateField(
            JsNativeFieldKind.Arm64InvertedCondition, 12, 4, false, 1, false)),
        Bits("cset x10, cond", 0x9A9F07EAu, new JsNativeTemplateField(
            JsNativeFieldKind.Arm64InvertedCondition, 12, 4, false, 1, false)),

        // orr x9, x9, x10  -- the one integer combination this backend does, joining two conditions.
        Word("orr x9, x9, x10", 0xAA0A0129u),

        // The double loads and stores, over the three slab registers the prologue filled. imm12 is
        // an UNSIGNED multiple of eight, so this form reaches forward and nothing back.
        Bits("ldr d0, [x19, #slab]", 0xFD400260u, Scaled12()),
        Bits("ldr d1, [x19, #slab]", 0xFD400261u, Scaled12()),
        Bits("ldr d0, [x20, #slab]", 0xFD400280u, Scaled12()),
        Bits("ldr d0, [x21, #slab]", 0xFD4002A0u, Scaled12()),
        Bits("str d0, [x19, #slab]", 0xFD000260u, Scaled12()),
        Bits("str d0, [x20, #slab]", 0xFD000280u, Scaled12()),

        // fadd, fsub, fmul, fdiv d0, d0, d1 -- 0x1E600000 | Rm = 1 | opcode | Rn = 0 | Rd = 0.
        Word("fadd d0, d0, d1", 0x1E612800u),
        Word("fsub d0, d0, d1", 0x1E613800u),
        Word("fmul d0, d0, d1", 0x1E610800u),
        Word("fdiv d0, d0, d1", 0x1E611800u),

        // fneg d0, d0  -- it FLIPS THE SIGN BIT and does not subtract from zero.
        Word("fneg d0, d0", 0x1E614000u),

        // fcmp d0, d1 and fcmp d1, d0  -- the four-way answer every relational operator reads.
        Word("fcmp d0, d1", 0x1E612000u),
        Word("fcmp d1, d0", 0x1E602020u),

        // fcmp d0, #0.0  -- a literal zero rather than a register holding one.
        Word("fcmp d0, #0.0", 0x1E602008u),

        // b <imm26>  -- a SIGNED WORD displacement, not a byte one: a scan that read bytes would
        // judge every branch four times too far.
        Bits("b imm26", 0x14000000u, new JsNativeTemplateField(
            JsNativeFieldKind.UnitLocalBranch, 0, 26, true, 4, false)),

        // b.<cond> <imm19>  -- bits 31..24 fixed, bit 4 zero, the condition in the low nibble and
        // the signed word displacement above it.
        Bits(
            "b.cond imm19",
            0x54000000u,
            new JsNativeTemplateField(JsNativeFieldKind.Arm64Condition, 0, 4, false, 1, false),
            new JsNativeTemplateField(JsNativeFieldKind.UnitLocalBranch, 5, 19, true, 4, false)),

        // ret  -- through x30, and A64's return TAKES NO IMMEDIATE.
        TerminalWord("ret", 0xD65F03C0u),
    ];

    // ---- the small constructors ----------------------------------------------------------------

    /// <summary>A template with no variable field at all.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=AB2C96
    // Broiler-Human:        PENDING
    private static JsNativeTemplate Plain(string text, byte[] pattern) =>
        new(text, pattern, isReturn: false);

    /// <summary>A template with no variable field that returns to the caller.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F6928F
    // Broiler-Human:        PENDING
    private static JsNativeTemplate Terminal(string text, byte[] pattern) =>
        new(text, pattern, isReturn: true);

    /// <summary>A template with one or more variable fields.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=551F2E
    // Broiler-Human:        PENDING
    private static JsNativeTemplate Field(
        string text, byte[] pattern, params JsNativeTemplateField[] fields) =>
        new(text, pattern, isReturn: false, fields);

    /// <summary>One A64 instruction word, fixed in every bit.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=8E7AAA
    // Broiler-Human:        PENDING
    private static JsNativeTemplate Word(string text, uint word) =>
        new(text, LittleEndian(word), isReturn: false);

    /// <summary>The A64 return, which is the one word a unit must end on.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=934A02
    // Broiler-Human:        PENDING
    private static JsNativeTemplate TerminalWord(string text, uint word) =>
        new(text, LittleEndian(word), isReturn: true);

    /// <summary>One A64 instruction word with a bit field or two inside it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=AC5672
    // Broiler-Human:        PENDING
    private static JsNativeTemplate Bits(
        string text, uint word, params JsNativeTemplateField[] fields) =>
        new(text, LittleEndian(word), isReturn: false, fields);

    /// <summary>A thirty-two-bit signed displacement at a byte offset.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=3306D9
    // Broiler-Human:        PENDING
    private static JsNativeTemplateField Disp32(int byteOffset, JsNativeFieldKind kind) =>
        new(kind, byteOffset * 8, 32, Signed: true, Scale: 1, FromInstructionEnd: false);

    /// <summary>A thirty-two-bit immediate at a byte offset.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1CDA2B
    // Broiler-Human:        PENDING
    private static JsNativeTemplateField Imm32(int byteOffset, JsNativeFieldKind kind) =>
        new(kind, byteOffset * 8, 32, Signed: true, Scale: 1, FromInstructionEnd: false);

    /// <summary>A sixty-four-bit immediate at a byte offset.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=334A2A
    // Broiler-Human:        PENDING
    private static JsNativeTemplateField Imm64(int byteOffset, JsNativeFieldKind kind) =>
        new(kind, byteOffset * 8, 64, Signed: true, Scale: 1, FromInstructionEnd: false);

    /// <summary>
    /// A four-bit condition code at a bit offset, which for x86-64 is the low nibble of an opcode.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F99542
    // Broiler-Human:        PENDING
    private static JsNativeTemplateField Nibble(int bitOffset, JsNativeFieldKind kind) =>
        new(kind, bitOffset, 4, Signed: false, Scale: 1, FromInstructionEnd: false);

    /// <summary>
    /// A thirty-two-bit branch displacement, measured from the END of the instruction as every
    /// x86-64 <c>rel32</c> is.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=9D731C
    // Broiler-Human:        PENDING
    private static JsNativeTemplateField Rel32(int byteOffset, JsNativeFieldKind kind) =>
        new(kind, byteOffset * 8, 32, Signed: true, Scale: 1, FromInstructionEnd: true);

    /// <summary>The unsigned scaled twelve-bit immediate an A64 double load or store carries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F91159
    // Broiler-Human:        PENDING
    private static JsNativeTemplateField Scaled12() =>
        new(JsNativeFieldKind.SlabDisplacement, 10, 12, Signed: false, Scale: 8, FromInstructionEnd: false);

    /// <summary>Four bytes, little-endian, as the architecture's own data order.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=27A584
    // Broiler-Human:        PENDING
    private static byte[] LittleEndian(uint word) =>
        [(byte)word, (byte)(word >> 8), (byte)(word >> 16), (byte)(word >> 24)];

    /// <summary>Four bytes, little-endian, of a thirty-two-bit constant this table fixes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=FD4FF2
    // Broiler-Human:        PENDING
    private static byte[] Int32(int value) =>
        [(byte)value, (byte)(value >> 8), (byte)(value >> 16), (byte)(value >> 24)];
}
