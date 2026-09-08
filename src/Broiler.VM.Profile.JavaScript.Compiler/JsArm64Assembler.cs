// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   45
// Annotated:        45/45
// Exempt:           9
// Human-reviewed:   0/45
// IP risk:          Low
// Security risk:    High
// Criteria:         14/14
// Resource impact:  3/10 max
// Unverified:       45
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>The condition field of an A64 conditional instruction, as the architecture spells it.</summary>
/// <remarks>
/// <para>
/// <b>THE FOUR CONDITIONS THIS BACKEND USES ARE THE FOUR THAT ANSWER FALSE FOR AN UNORDERED
/// COMPARE, AND THAT IS WHY THE OBVIOUS ONES ARE ABSENT.</b> A64's <c>fcmp</c> sets N, Z, C and V
/// to a four-way answer - less, equal, greater, UNORDERED - and JavaScript's relational operators
/// answer <c>false</c> when either operand is a NaN. <c>LT</c> is true when N differs from V, which
/// an unordered compare satisfies, so <c>LT</c> spells "less than OR unordered" and is the wrong
/// instruction for <c>&lt;</c>; <c>MI</c>, which reads N alone, is the right one. <c>LE</c> carries
/// the same defect, so <c>&lt;=</c> is emitted as a <c>GE</c> over swapped operands rather than as
/// the condition whose name matches the operator.
/// </para>
/// <para>
/// <b>The numeric values are the architecture's and are not this component's to choose</b>: they
/// are the four bits an instruction word carries, and the one arithmetic relation between them that
/// this component relies on is that inverting a condition is exclusive-or with one, which is what
/// <c>cset</c> is built from.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=E686FF
// Broiler-Human:        PENDING
public enum JsArm64Condition
{
    /// <summary>Z set. After <c>fcmp</c> this is equal OR unordered.</summary>
    Equal = 0,

    /// <summary>Z clear. After <c>fcmp</c> this is less, greater or unordered.</summary>
    NotEqual = 1,

    /// <summary>N set. After <c>fcmp</c> this is strictly less, and false when unordered.</summary>
    Minus = 4,

    /// <summary>V set. After <c>fcmp</c> this is exactly "unordered", which is "either was NaN".</summary>
    Overflow = 6,

    /// <summary>N equals V. After <c>fcmp</c> this is greater or equal, and false when unordered.</summary>
    GreaterOrEqual = 10,

    /// <summary>Z clear and N equals V. After <c>fcmp</c> this is strictly greater.</summary>
    Greater = 12,
}

/// <summary>
/// An encoder for the A64 instructions this profile's arm64 backend emits, and nothing else.
/// </summary>
/// <remarks>
/// <para>
/// <b>EVERY INSTRUCTION IS ONE FIXED 32-BIT LITTLE-ENDIAN WORD, AND THAT ONE FACT DELETES MOST OF
/// WHAT AN ASSEMBLER USUALLY IS.</b> There is no prefix to choose, no ModRM byte to build, no SIB
/// byte, no operand-size escape, and - the one that matters - <b>no relaxation problem</b>. On a
/// variable-width instruction set a branch whose displacement is not yet known is encoded short and
/// widened later, and the pass that widens it moves every later instruction, which moves every later
/// displacement; here a branch site occupies one word before its target is known and one word after,
/// so a patch changes bits and never lengths.
/// </para>
/// <para>
/// <b>A DISPLACEMENT THAT DOES NOT FIT IS REFUSED AND NEVER TRUNCATED.</b> <c>b.cond</c> carries a
/// signed 19-bit word displacement, which is one mebibyte either way, and <c>b</c> carries a signed
/// 26-bit one, which is a hundred and twenty-eight. A patcher that masked an overlong displacement
/// into the field would produce a well-formed instruction word that branches somewhere else, and
/// that is the exact defect class nothing downstream can catch: the artifact verifier would see a
/// legal encoding, a golden-byte test would see whatever the encoder wrote, and the failure would
/// arrive as a jump into the middle of an unrelated basic block. So <see cref="TryFix"/> answers
/// with a sentence instead.
/// </para>
/// <para>
/// <b>THE BYTES THIS TYPE PRODUCES ARE NEVER EXECUTED BY ANYTHING IN THIS REPOSITORY.</b> See
/// <see cref="JsArm64Backend"/> for the reason, which is stated there once, at the place a reader
/// would otherwise go looking for an arming call. Nothing here maps memory, changes a page
/// protection, or takes the address of an emitted byte.
/// </para>
/// <para>
/// <b>Every encoding below is written as a base word or-ed with shifted fields, and the field
/// decomposition is written beside it.</b> A reader checking an encoder against the architecture
/// manual needs the field boundaries, not the hexadecimal; a constant with no decomposition beside
/// it is a constant a reader can only trust.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=ACB9B6
// Broiler-Falsified-If: a word this type writes decodes as an instruction other than the one its method name states, or a branch it patched reaches an offset other than its bound label
// Broiler-Human:        PENDING
public sealed class JsArm64Assembler
{
    /// <summary>The register number the encoding uses for the stack pointer or the zero register.</summary>
    /// <remarks>
    /// <b>It is one number meaning two registers, and which one it means is decided by the
    /// instruction rather than by the operand.</b> In a load or a store's base field, and in the
    /// <c>Rn</c> field of an add or subtract with an immediate, 31 is the stack pointer; in the
    /// <c>Rn</c>, <c>Rm</c> or <c>Rd</c> field of a data-processing instruction it is the zero
    /// register. The architecture does not spell them apart and neither does this constant.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C39507
    // Broiler-Human:        PENDING
    public const int StackOrZero = 31;

    /// <summary>The link register, which <c>ret</c> returns through.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=5642FC
    // Broiler-Human:        PENDING
    public const int Link = 30;

    /// <summary>The frame pointer register AAPCS64 names.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=82DB52
    // Broiler-Human:        PENDING
    public const int FramePointer = 29;

    /// <summary>The furthest a <c>b.cond</c> can reach, in instruction words, in either direction.</summary>
    /// <remarks>
    /// The field is a signed 19-bit word displacement, so it spans -262144 to 262143 words, which is
    /// one mebibyte of code either way. This constant is the positive end and the negative end is
    /// one further, which is why the check below is written as two comparisons rather than one
    /// absolute value.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=EC7485
    // Broiler-Human:        PENDING
    public const int ConditionalBranchReach = (1 << 18) - 1;

    /// <summary>The furthest a <c>b</c> can reach, in instruction words, in either direction.</summary>
    /// <remarks>
    /// The field is a signed 26-bit word displacement: -33554432 to 33554431 words, a hundred and
    /// twenty-eight mebibytes of code either way.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=C70A16
    // Broiler-Human:        PENDING
    public const int BranchReach = (1 << 25) - 1;

    private readonly System.Collections.Generic.List<uint> words = [];

    private readonly System.Collections.Generic.List<int> labels = [];

    private readonly System.Collections.Generic.List<(int Site, int Label, bool Conditional)> pending = [];

    /// <summary>How many instruction words have been written.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=01E00B
    // Broiler-Human:        PENDING
    public int WordCount => words.Count;

    /// <summary>Where the next word will be written, counted in bytes from the first.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=200C79
    // Broiler-Human:        PENDING
    public int ByteCount => words.Count * 4;

    /// <summary>Writes one already-encoded instruction word.</summary>
    /// <remarks>
    /// <b>It is public so that a golden-byte test can assert against a word this encoder never
    /// writes</b> - a deliberately wrong encoding, written to prove the comparison would notice -
    /// and it is the one door into the buffer that does not know what it is writing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=75D47D
    // Broiler-Human:        PENDING
    public void Word(uint word) => words.Add(word);

    /// <summary>The emitted bytes, little-endian, four per instruction word.</summary>
    /// <remarks>
    /// <b>The byte order is the architecture's data order and not a choice.</b> A64 instructions are
    /// fetched as little-endian words on every configuration this backend could name, so a word
    /// whose bytes were written the other way would decode as a different instruction rather than as
    /// a malformed one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=911DEE
    // Broiler-Falsified-If: the four bytes this method writes for a word are not that word's little-endian representation
    // Broiler-Human:        PENDING
    public byte[] ToArray()
    {
        var bytes = new byte[words.Count * 4];

        for (var index = 0; index < words.Count; index++)
        {
            var word = words[index];
            bytes[(index * 4) + 0] = (byte)(word & 0xFF);
            bytes[(index * 4) + 1] = (byte)((word >> 8) & 0xFF);
            bytes[(index * 4) + 2] = (byte)((word >> 16) & 0xFF);
            bytes[(index * 4) + 3] = (byte)((word >> 24) & 0xFF);
        }

        return bytes;
    }

    /// <summary>Reserves a label, which is a place in the stream nothing has decided yet.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=026DE5
    // Broiler-Human:        PENDING
    public int DefineLabel()
    {
        labels.Add(-1);
        return labels.Count - 1;
    }

    /// <summary>Binds <paramref name="label"/> to the next word to be written.</summary>
    /// <remarks>
    /// <b>Binding twice is a programming mistake in the emitter and is refused rather than
    /// absorbed.</b> A label bound twice means one of the two bindings is silently ignored, and the
    /// branches that were going to the other one now go somewhere the emitter never intended.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AFA6AE
    // Broiler-Human:        PENDING
    public void Bind(int label)
    {
        if (labels[label] >= 0)
        {
            throw new System.InvalidOperationException(
                "label " + label + " is already bound to word " + labels[label]);
        }

        labels[label] = words.Count;
    }

    /// <summary>Emits <c>b label</c> as one word, patched when the label is bound.</summary>
    /// <remarks>
    /// The word written now is the base encoding with a zero displacement, which is
    /// <c>b .</c> - a branch to itself. That is deliberate: an unpatched site is an infinite loop
    /// rather than a fall-through, so a fixup this encoder forgot shows up as a hang in whatever
    /// eventually runs it rather than as code that quietly continues into the next block.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=56C339
    // Broiler-Falsified-If: a site this method records is left carrying its placeholder word after TryFix has answered true
    // Broiler-Human:        PENDING
    public void Branch(int label)
    {
        pending.Add((words.Count, label, false));

        // b <imm26>: bits 31..26 = 000101, bits 25..0 = imm26, a signed word displacement.
        // 0b000101 << 26 = 0x14000000.
        words.Add(0x14000000u);
    }

    /// <summary>Emits <c>b.cond label</c> as one word, patched when the label is bound.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=C4E165
    // Broiler-Falsified-If: the condition field of the word this method writes is not the condition it was given
    // Broiler-Human:        PENDING
    public void BranchIf(JsArm64Condition condition, int label)
    {
        pending.Add((words.Count, label, true));

        // b.<cond> <imm19>: bits 31..24 = 01010100, bits 23..5 = imm19, bit 4 = 0,
        // bits 3..0 = cond. 0b01010100 << 24 = 0x54000000.
        words.Add(0x54000000u | (uint)condition);
    }

    /// <summary>
    /// Patches every branch site, or refuses and names the first site whose target is out of reach.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A DISPLACEMENT THAT DOES NOT FIT IS A REFUSAL AND NEVER A MASK.</b> Truncating an
    /// out-of-range displacement into the field produces a legal instruction word that branches to
    /// an address the emitter never named, and every check downstream of here - the artifact
    /// verifier's framing check, a golden-byte comparison, a template-closure scan - would pass it.
    /// The only place that defect can be caught is here, and the only way to catch it is to refuse.
    /// </para>
    /// <para>
    /// <b>The displacement is counted in WORDS and not in bytes, because the field is.</b> A64
    /// branch offsets are word offsets from the branch instruction itself, so the two low bits an
    /// aligned byte displacement would carry are not encoded at all; a patcher that wrote bytes into
    /// the field would branch four times too far.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=CFE6F8
    // Broiler-Falsified-If: a displacement outside the encodable range is written into a branch word instead of being refused
    // Broiler-Human:        PENDING
    public bool TryFix(out string refusal)
    {
        foreach (var (site, label, conditional) in pending)
        {
            var target = labels[label];

            if (target < 0)
            {
                refusal =
                    "the arm64 encoder left label " + label + " unbound, so the branch at word " +
                    site + " has no target";

                return false;
            }

            var displacement = target - site;
            var reach = conditional ? ConditionalBranchReach : BranchReach;

            if (displacement > reach || displacement < -reach - 1)
            {
                refusal =
                    "the branch at word " + site + " is " + displacement +
                    " words from its target, which the " + (conditional ? "19" : "26") +
                    "-bit displacement field cannot carry; a truncated displacement is a branch to " +
                    "somewhere else and is refused rather than encoded";

                return false;
            }

            words[site] = conditional
                ? (words[site] & 0xFF00001Fu) | (((uint)displacement & 0x7FFFFu) << 5)
                : (words[site] & 0xFC000000u) | ((uint)displacement & 0x03FFFFFFu);
        }

        refusal = string.Empty;
        return true;
    }

    /// <summary>
    /// Scales a byte offset into the unsigned twelve-bit field a load or store carries, or refuses.
    /// </summary>
    /// <remarks>
    /// <b>AN OFFSET THAT DOES NOT FIT THROWS RATHER THAN SPILLING INTO THE NEIGHBOURING FIELD.</b>
    /// The immediate occupies bits 21 to 10, and the register number of the base sits at bits 9 to
    /// 5 directly below it; an offset one too large would or-into <c>Rn</c> and produce a load from
    /// a different register, which decodes as a legal instruction and is therefore invisible to
    /// every check after this one. The backend refuses such a unit before it reaches here with a
    /// sentence a caller can read, so this throw is the second line and not the first.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=1F7360
    // Broiler-Falsified-If: an offset outside the encodable range is written into an instruction word instead of raising
    // Broiler-Human:        PENDING
    private static uint Scaled(int offset, int scale)
    {
        if (offset < 0 || offset % scale != 0 || offset / scale > 4095)
        {
            throw new System.InvalidOperationException(
                "the byte offset " + offset + " is not an encodable multiple of " + scale +
                " within the twelve-bit immediate an A64 load or store carries");
        }

        return (uint)(offset / scale);
    }

    /// <summary>
    /// Scales a byte offset into the signed seven-bit field a load or store PAIR carries, or refuses.
    /// </summary>
    /// <remarks>
    /// <b>The field is signed, so the range is not symmetric and the check is two comparisons.</b>
    /// A pre-index store pair with a positive offset would grow the stack downward-by-negative,
    /// which is why the prologue's offset is negative and the epilogue's is positive; a truncated
    /// offset here would leave the stack pointer somewhere the epilogue does not undo.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=0B2388
    // Broiler-Falsified-If: an offset outside the signed seven-bit range is written into a pair instruction instead of raising
    // Broiler-Human:        PENDING
    private static uint PairOffset(int offset)
    {
        if (offset % 8 != 0 || offset / 8 > 63 || offset / 8 < -64)
        {
            throw new System.InvalidOperationException(
                "the byte offset " + offset + " is not an encodable multiple of 8 within the " +
                "signed seven-bit immediate an A64 load or store pair carries");
        }

        return (uint)(offset / 8) & 0x7Fu;
    }

    // ---- prologue and epilogue -------------------------------------------------------------

    /// <summary>Emits <c>stp xT1, xT2, [sp, #offset]!</c> - a store pair that writes sp back.</summary>
    /// <remarks>
    /// <b>Encoding.</b> Bits 31..30 = <c>opc</c> = 10 (64-bit), bits 29..27 = 101, bit 26 =
    /// <c>V</c> = 0 (general registers), bits 25..23 = 011 (pre-index), bit 22 = <c>L</c> = 0
    /// (store), bits 21..15 = <c>imm7</c>, bits 14..10 = <c>Rt2</c>, bits 9..5 = <c>Rn</c>, bits
    /// 4..0 = <c>Rt</c>. The base word is bits 31..22 = 1010100110, which is 0xA9800000.
    /// <c>imm7</c> is a SIGNED multiple of eight, so the offset is divided by eight before it is
    /// encoded - a patcher that wrote bytes would move the stack pointer eight times too far.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=A28B68
    // Broiler-Falsified-If: the word this method writes moves the stack pointer by an amount other than the offset it was given
    // Broiler-Human:        PENDING
    public void StorePairPreIndex(int first, int second, int baseRegister, int offset) =>
        words.Add(
            0xA9800000u |
            (PairOffset(offset) << 15) |
            ((uint)second << 10) |
            ((uint)baseRegister << 5) |
            (uint)first);

    /// <summary>Emits <c>stp xT1, xT2, [xN, #offset]</c> - a store pair that leaves the base alone.</summary>
    /// <remarks>
    /// <b>Encoding.</b> As <see cref="StorePairPreIndex"/> but bits 25..23 = 010, the signed-offset
    /// form: bits 31..22 = 1010100100, which is 0xA9000000.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=94F7FE
    // Broiler-Human:        PENDING
    public void StorePair(int first, int second, int baseRegister, int offset) =>
        words.Add(
            0xA9000000u |
            (PairOffset(offset) << 15) |
            ((uint)second << 10) |
            ((uint)baseRegister << 5) |
            (uint)first);

    /// <summary>Emits <c>ldp xT1, xT2, [xN, #offset]</c>.</summary>
    /// <remarks>
    /// <b>Encoding.</b> As <see cref="StorePair"/> with bit 22 = <c>L</c> = 1: bits 31..22 =
    /// 1010100101, which is 0xA9400000.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=CCAA04
    // Broiler-Human:        PENDING
    public void LoadPair(int first, int second, int baseRegister, int offset) =>
        words.Add(
            0xA9400000u |
            (PairOffset(offset) << 15) |
            ((uint)second << 10) |
            ((uint)baseRegister << 5) |
            (uint)first);

    /// <summary>Emits <c>ldp xT1, xT2, [sp], #offset</c> - a load pair that writes sp back after.</summary>
    /// <remarks>
    /// <b>Encoding.</b> Bits 25..23 = 001 (post-index) and bit 22 = <c>L</c> = 1: bits 31..22 =
    /// 1010100011, which is 0xA8C00000.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=835BE5
    // Broiler-Falsified-If: the word this method writes leaves the stack pointer at a value other than its entry value plus the offset
    // Broiler-Human:        PENDING
    public void LoadPairPostIndex(int first, int second, int baseRegister, int offset) =>
        words.Add(
            0xA8C00000u |
            (PairOffset(offset) << 15) |
            ((uint)second << 10) |
            ((uint)baseRegister << 5) |
            (uint)first);

    /// <summary>Emits <c>ret</c>, which returns through x30.</summary>
    /// <remarks>
    /// <para>
    /// <b>Encoding.</b> Bits 31..10 = 1101011001011111000000, bits 9..5 = <c>Rn</c>, bits 4..0 = 0.
    /// The base word is 0xD65F0000 and x30 in <c>Rn</c> contributes 30 &lt;&lt; 5 = 0x3C0, giving
    /// 0xD65F03C0.
    /// </para>
    /// <para>
    /// <b>A64's <c>ret</c> TAKES NO IMMEDIATE, AND THAT IS THE REASON THIS IS THE SECOND BACKEND
    /// AHEAD OF x86-32.</b> The core's retained anecdote is a hand-written function that returned
    /// with a stack adjustment its calling convention did not want, so every call leaked eight bytes
    /// of stack and the process died a long way from the defect. That instruction has no A64
    /// spelling: returning and adjusting the stack are two different instructions here, the
    /// adjustment is the epilogue's own <c>ldp</c> writeback, and the convention is caller-pops in
    /// every AAPCS64 variant. So the whole defect class is UNREPRESENTABLE on this architecture,
    /// while x86-32 is the one calling convention in the declared matrix that can still spell it -
    /// which is why x86-32 is not a backend and this is.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=BB9CCC
    // Broiler-Falsified-If: the word this method writes is anything but a plain return through the link register
    // Broiler-Human:        PENDING
    public void Return() => words.Add(0xD65F0000u | ((uint)Link << 5));

    // ---- moves and constants ---------------------------------------------------------------

    /// <summary>Emits <c>mov xD, xM</c>, which the architecture spells <c>orr xD, xzr, xM</c>.</summary>
    /// <remarks>
    /// <b>Encoding.</b> ORR (shifted register), 64-bit: bits 31..21 = 10101010000, bits 20..16 =
    /// <c>Rm</c>, bits 15..10 = shift amount = 0, bits 9..5 = <c>Rn</c> = 31 (the zero register),
    /// bits 4..0 = <c>Rd</c>. The base word is 0xAA000000 and the zero register in <c>Rn</c>
    /// contributes 31 &lt;&lt; 5 = 0x3E0.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A81F87
    // Broiler-Human:        PENDING
    public void Move(int destination, int source) =>
        words.Add(
            0xAA000000u | ((uint)source << 16) | ((uint)StackOrZero << 5) | (uint)destination);

    /// <summary>Emits <c>mov xD, sp</c>, which the architecture spells <c>add xD, sp, #0</c>.</summary>
    /// <remarks>
    /// <b>Encoding.</b> ADD (immediate), 64-bit: bits 31..23 = 100100010, bit 22 = <c>sh</c> = 0,
    /// bits 21..10 = <c>imm12</c> = 0, bits 9..5 = <c>Rn</c> = 31 (here the stack pointer), bits
    /// 4..0 = <c>Rd</c>. The base word is 0x91000000. It is not the <c>orr</c> form above because
    /// register 31 means the ZERO register there and the STACK POINTER here, and the two
    /// instructions disagree about which one it is.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=471D54
    // Broiler-Human:        PENDING
    public void MoveFromStackPointer(int destination) =>
        words.Add(0x91000000u | ((uint)StackOrZero << 5) | (uint)destination);

    /// <summary>Emits <c>movz xD, #value, lsl #(shift * 16)</c>.</summary>
    /// <remarks>
    /// <b>Encoding.</b> MOVZ, 64-bit: bit 31 = <c>sf</c> = 1, bits 30..29 = <c>opc</c> = 10, bits
    /// 28..23 = 100101, bits 22..21 = <c>hw</c> (the shift, counted in halfwords), bits 20..5 =
    /// <c>imm16</c>, bits 4..0 = <c>Rd</c>. The base word is 0xD2800000. Every bit the immediate
    /// does not cover is ZEROED, which is what separates this from <c>movk</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=B428C8
    // Broiler-Human:        PENDING
    public void MoveWide(int destination, ushort value, int halfword) =>
        words.Add(
            0xD2800000u | ((uint)halfword << 21) | ((uint)value << 5) | (uint)destination);

    /// <summary>Emits <c>movz wD, #value</c> - the 32-bit form, which zeroes the upper 32 bits.</summary>
    /// <remarks>
    /// <b>Encoding.</b> As <see cref="MoveWide"/> with bit 31 = <c>sf</c> = 0: the base word is
    /// 0x52800000.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=531925
    // Broiler-Human:        PENDING
    public void MoveWide32(int destination, ushort value) =>
        words.Add(0x52800000u | ((uint)value << 5) | (uint)destination);

    /// <summary>Emits <c>movk wD, #value, lsl #16</c> - keeping the bits it does not write.</summary>
    /// <remarks>
    /// <b>Encoding.</b> MOVK, 32-bit: bit 31 = <c>sf</c> = 0, bits 30..29 = <c>opc</c> = 11, bits
    /// 28..23 = 100101, bits 22..21 = <c>hw</c> = 01, bits 20..5 = <c>imm16</c>, bits 4..0 =
    /// <c>Rd</c>. The base word is 0x72800000 and <c>hw</c> = 1 contributes 1 &lt;&lt; 21 =
    /// 0x200000, giving 0x72A00000.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=45A3D7
    // Broiler-Human:        PENDING
    public void MoveKeep32High(int destination, ushort value) =>
        words.Add(0x72A00000u | ((uint)value << 5) | (uint)destination);

    /// <summary>Emits <c>fmov dD, xN</c> - the 64 bits of an integer register, read as a double.</summary>
    /// <remarks>
    /// <b>Encoding.</b> Convert between floating-point and integer, 64-bit: bit 31 = <c>sf</c> = 1,
    /// bits 30..29 = 00, bits 28..24 = 11110, bits 23..22 = <c>type</c> = 01 (double), bit 21 = 1,
    /// bits 20..19 = <c>rmode</c> = 00, bits 18..16 = <c>opcode</c> = 111, bits 15..10 = 0, bits
    /// 9..5 = <c>Rn</c>, bits 4..0 = <c>Rd</c>. Bits 31..21 = 10011110011 give 0x9E600000, and
    /// <c>opcode</c> = 111 contributes 7 &lt;&lt; 16 = 0x70000, giving 0x9E670000. It moves BITS
    /// and does not convert: this is how a NaN with a chosen payload is built.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8ADAFD
    // Broiler-Human:        PENDING
    public void MoveBitsToDouble(int destination, int source) =>
        words.Add(0x9E670000u | ((uint)source << 5) | (uint)destination);

    /// <summary>Emits <c>scvtf dD, xN</c> - a signed 64-bit integer converted to a double.</summary>
    /// <remarks>
    /// <b>Encoding.</b> As <see cref="MoveBitsToDouble"/> with <c>opcode</c> = 010: 2 &lt;&lt; 16 =
    /// 0x20000, giving 0x9E620000. It CONVERTS rather than reinterprets, which is what turns the
    /// zero or one a <c>cset</c> produced into the 0.0 or 1.0 a JavaScript Boolean is represented
    /// by in a slab of doubles.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C9470E
    // Broiler-Human:        PENDING
    public void ConvertToDouble(int destination, int source) =>
        words.Add(0x9E620000u | ((uint)source << 5) | (uint)destination);

    /// <summary>Emits <c>cset xD, cond</c> - one if the condition holds, zero if it does not.</summary>
    /// <remarks>
    /// <b>Encoding.</b> CSET is CSINC <c>Xd</c>, xzr, xzr, <c>invert(cond)</c>: bit 31 = <c>sf</c> =
    /// 1, bits 30..21 = 0011010100, bits 20..16 = <c>Rm</c> = 31, bits 15..12 = the condition, bits
    /// 11..10 = 01, bits 9..5 = <c>Rn</c> = 31, bits 4..0 = <c>Rd</c>. That is 0x9A800000 with
    /// 31 &lt;&lt; 16 = 0x1F0000, 1 &lt;&lt; 10 = 0x400 and 31 &lt;&lt; 5 = 0x3E0, giving
    /// 0x9A9F07E0. <b>The condition in the word is the INVERSE of the one asked for</b>, because
    /// the instruction increments when its condition FAILS; inverting an A64 condition is
    /// exclusive-or with one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=7A2EBF
    // Broiler-Falsified-If: the word this method writes sets its destination to one in the case where the condition it was given does not hold
    // Broiler-Human:        PENDING
    public void SetIfCondition(int destination, JsArm64Condition condition) =>
        words.Add(0x9A9F07E0u | (((uint)condition ^ 1u) << 12) | (uint)destination);

    // ---- integer arithmetic ----------------------------------------------------------------

    /// <summary>Emits <c>subs xD, xN, #immediate</c> - a subtract that sets the flags.</summary>
    /// <remarks>
    /// <b>Encoding.</b> SUB (immediate), 64-bit, with <c>S</c> set: bit 31 = <c>sf</c> = 1, bit 30 =
    /// <c>op</c> = 1 (subtract), bit 29 = <c>S</c> = 1 (set flags), bits 28..23 = 100010, bit 22 =
    /// <c>sh</c> = 0 (no 12-bit shift), bits 21..10 = <c>imm12</c>, bits 9..5 = <c>Rn</c>, bits
    /// 4..0 = <c>Rd</c>. Bits 31..23 = 111100010 give 0xF1000000. The immediate is TWELVE bits
    /// unshifted, so a caller with a larger amount subtracts more than once.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=E6B894
    // Broiler-Human:        PENDING
    public void SubtractSetting(int destination, int source, int immediate) =>
        words.Add(
            0xF1000000u | ((uint)immediate << 10) | ((uint)source << 5) | (uint)destination);

    /// <summary>Emits <c>orr xD, xN, xM</c>.</summary>
    /// <remarks>
    /// <b>Encoding.</b> ORR (shifted register), 64-bit, shift amount zero: bits 31..21 =
    /// 10101010000, bits 20..16 = <c>Rm</c>, bits 15..10 = 0, bits 9..5 = <c>Rn</c>, bits 4..0 =
    /// <c>Rd</c>. The base word is 0xAA000000.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F68E5A
    // Broiler-Human:        PENDING
    public void Or(int destination, int left, int right) =>
        words.Add(
            0xAA000000u | ((uint)right << 16) | ((uint)left << 5) | (uint)destination);

    // ---- loads and stores ------------------------------------------------------------------

    /// <summary>Emits <c>ldr xT, [xN, #offset]</c>, the unsigned-offset form.</summary>
    /// <remarks>
    /// <b>Encoding.</b> LDR (immediate, unsigned offset), 64-bit: bits 31..30 = <c>size</c> = 11,
    /// bits 29..27 = 111, bit 26 = <c>V</c> = 0, bits 25..24 = 01, bits 23..22 = <c>opc</c> = 01
    /// (load), bits 21..10 = <c>imm12</c>, bits 9..5 = <c>Rn</c>, bits 4..0 = <c>Rt</c>. The base
    /// word is 0xF9400000. <c>imm12</c> is an UNSIGNED multiple of eight, so the offset is divided
    /// by eight and cannot be negative: this form reaches 32760 bytes forward and nothing back.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6521AF
    // Broiler-Human:        PENDING
    public void LoadRegister(int destination, int baseRegister, int offset) =>
        words.Add(
            0xF9400000u |
            (Scaled(offset, 8) << 10) |
            ((uint)baseRegister << 5) |
            (uint)destination);

    /// <summary>Emits <c>str xT, [xN, #offset]</c>, the unsigned-offset form.</summary>
    /// <remarks>
    /// <b>Encoding.</b> As <see cref="LoadRegister"/> with <c>opc</c> = 00: the base word is
    /// 0xF9000000.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=735DCD
    // Broiler-Human:        PENDING
    public void StoreRegister(int source, int baseRegister, int offset) =>
        words.Add(
            0xF9000000u |
            (Scaled(offset, 8) << 10) |
            ((uint)baseRegister << 5) |
            (uint)source);

    /// <summary>Emits <c>str wT, [xN, #offset]</c> - a 32-bit store.</summary>
    /// <remarks>
    /// <b>Encoding.</b> As <see cref="StoreRegister"/> with <c>size</c> = 10: the base word is
    /// 0xB9000000, and <c>imm12</c> is an unsigned multiple of FOUR rather than of eight.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=66E4E1
    // Broiler-Human:        PENDING
    public void StoreWord(int source, int baseRegister, int offset) =>
        words.Add(
            0xB9000000u |
            (Scaled(offset, 4) << 10) |
            ((uint)baseRegister << 5) |
            (uint)source);

    /// <summary>Emits <c>ldr dT, [xN, #offset]</c> - a double loaded from memory.</summary>
    /// <remarks>
    /// <b>Encoding.</b> LDR (SIMD and floating-point, immediate, unsigned offset), 64-bit: bits
    /// 31..30 = <c>size</c> = 11, bits 29..27 = 111, bit 26 = <c>V</c> = 1 (a vector or
    /// floating-point register), bits 25..24 = 01, bits 23..22 = <c>opc</c> = 01, bits 21..10 =
    /// <c>imm12</c>, bits 9..5 = <c>Rn</c>, bits 4..0 = <c>Rt</c>. The base word is 0xFD400000. The
    /// only difference from <see cref="LoadRegister"/> is bit 26, which is why an encoder that
    /// hard-coded one word for both would load into the wrong register file.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=C29FF5
    // Broiler-Falsified-If: the word this method writes names an integer register rather than a floating-point one
    // Broiler-Human:        PENDING
    public void LoadDouble(int destination, int baseRegister, int offset) =>
        words.Add(
            0xFD400000u |
            (Scaled(offset, 8) << 10) |
            ((uint)baseRegister << 5) |
            (uint)destination);

    /// <summary>Emits <c>str dT, [xN, #offset]</c> - a double stored to memory.</summary>
    /// <remarks>
    /// <b>Encoding.</b> As <see cref="LoadDouble"/> with <c>opc</c> = 00: the base word is
    /// 0xFD000000.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=764A8D
    // Broiler-Falsified-If: the word this method writes names an integer register rather than a floating-point one
    // Broiler-Human:        PENDING
    public void StoreDouble(int source, int baseRegister, int offset) =>
        words.Add(
            0xFD000000u |
            (Scaled(offset, 8) << 10) |
            ((uint)baseRegister << 5) |
            (uint)source);

    // ---- double arithmetic -----------------------------------------------------------------

    /// <summary>Emits <c>fadd dD, dN, dM</c>.</summary>
    /// <remarks>
    /// <b>Encoding.</b> Floating-point data-processing, two sources: bits 31..21 = 00011110011
    /// (0x1E600000), bits 20..16 = <c>Rm</c>, bits 15..12 = <c>opcode</c>, bits 11..10 = 10, bits
    /// 9..5 = <c>Rn</c>, bits 4..0 = <c>Rd</c>. The <c>opcode</c> and the fixed 10 together occupy
    /// bits 15..10, so FADD's 0010 gives 001010 there, which is 0x2800.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A788EC
    // Broiler-Human:        PENDING
    public void AddDouble(int destination, int left, int right) =>
        words.Add(
            0x1E602800u | ((uint)right << 16) | ((uint)left << 5) | (uint)destination);

    /// <summary>Emits <c>fsub dD, dN, dM</c>.</summary>
    /// <remarks><b>Encoding.</b> As <see cref="AddDouble"/> with bits 15..10 = 001110: 0x3800.</remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=C16CBA
    // Broiler-Human:        PENDING
    public void SubtractDouble(int destination, int left, int right) =>
        words.Add(
            0x1E603800u | ((uint)right << 16) | ((uint)left << 5) | (uint)destination);

    /// <summary>Emits <c>fmul dD, dN, dM</c>.</summary>
    /// <remarks><b>Encoding.</b> As <see cref="AddDouble"/> with bits 15..10 = 000010: 0x0800.</remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=3B1847
    // Broiler-Human:        PENDING
    public void MultiplyDouble(int destination, int left, int right) =>
        words.Add(
            0x1E600800u | ((uint)right << 16) | ((uint)left << 5) | (uint)destination);

    /// <summary>Emits <c>fdiv dD, dN, dM</c>.</summary>
    /// <remarks><b>Encoding.</b> As <see cref="AddDouble"/> with bits 15..10 = 000110: 0x1800.</remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=08A8C6
    // Broiler-Human:        PENDING
    public void DivideDouble(int destination, int left, int right) =>
        words.Add(
            0x1E601800u | ((uint)right << 16) | ((uint)left << 5) | (uint)destination);

    /// <summary>Emits <c>fneg dD, dN</c>.</summary>
    /// <remarks>
    /// <b>Encoding.</b> Floating-point data-processing, one source: bits 31..21 = 00011110011
    /// (0x1E600000), bits 20..15 = <c>opcode</c> = 000010 (FNEG), bits 14..10 = 10000, bits 9..5 =
    /// <c>Rn</c>, bits 4..0 = <c>Rd</c>. That is 0x1E600000 with 2 &lt;&lt; 15 = 0x10000 and
    /// 16 &lt;&lt; 10 = 0x4000, giving 0x1E614000. It FLIPS THE SIGN BIT and does not subtract from
    /// zero, which is the difference that makes the negation of zero a negative zero and the
    /// negation of a NaN still a NaN.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CBEA2C
    // Broiler-Human:        PENDING
    public void NegateDouble(int destination, int source) =>
        words.Add(0x1E614000u | ((uint)source << 5) | (uint)destination);

    /// <summary>Emits <c>fcmp dN, dM</c>.</summary>
    /// <remarks>
    /// <b>Encoding.</b> Bits 31..21 = 00011110011 (0x1E600000), bits 20..16 = <c>Rm</c>, bits
    /// 15..10 = 001000 (0x2000), bits 9..5 = <c>Rn</c>, bits 4..0 = <c>opcode2</c> = 00000. The
    /// answer is FOUR-WAY - less, equal, greater, unordered - and the unordered case is what makes
    /// the naive choice of condition wrong for every JavaScript relational operator. See
    /// <see cref="JsArm64Condition"/>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=437B58
    // Broiler-Falsified-If: the word this method writes compares the two registers in the opposite order
    // Broiler-Human:        PENDING
    public void CompareDouble(int left, int right) =>
        words.Add(0x1E602000u | ((uint)right << 16) | ((uint)left << 5));

    /// <summary>Emits <c>fcmp dN, #0.0</c>.</summary>
    /// <remarks>
    /// <b>Encoding.</b> As <see cref="CompareDouble"/> with <c>Rm</c> = 00000 and <c>opcode2</c> =
    /// 01000, which sets bit 3: 0x1E602008. <b>It compares against a literal zero and not against a
    /// register that happens to hold one</b>, which is what lets a truthiness test cost one
    /// instruction and no scratch register.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=10613A
    // Broiler-Human:        PENDING
    public void CompareDoubleWithZero(int source) =>
        words.Add(0x1E602008u | ((uint)source << 5));
}
