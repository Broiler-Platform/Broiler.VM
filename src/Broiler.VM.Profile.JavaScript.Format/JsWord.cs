// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   32
// Annotated:        32/32
// Exempt:           0
// Human-reviewed:   0/32
// IP risk:          Low
// Security risk:    High
// Criteria:         9/9
// Resource impact:  0/10 max
// Unverified:       32
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The layout of a value-form word: one 64-bit word that is either a Number or a tagged value, as
/// decision JSD-0035 section 2 describes it.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE LAYOUT LIVES HERE BECAUSE THREE PARTIES WILL READ IT AND ONLY ONE MAY STATE IT.</b> The
/// profile's codec and handle table read words today; a value-form emitter and the verifier's
/// template scan will read them once stage JSV-1 exists. This assembly is the one all three
/// reference, and a tag written as a literal in two places is a tag the two can disagree about.
/// </para>
/// <para>
/// <b>EVERY WORD BELOW <see cref="FirstTag"/> IN ITS TOP SIXTEEN BITS IS A NUMBER, AND THE TAGS
/// SIT WHERE NO ARITHMETIC CAN LAND.</b> The x86-64 arithmetic unit's default NaN is
/// <c>0xFFF8_0000_0000_0000</c> and a sign flip of it is <c>0x7FF8_0000_0000_0000</c>; both are
/// below the first tag, and an operation on operands outside the tag space propagates one of its
/// operands' NaNs or produces the default one, so it cannot produce a tagged word. A double that
/// enters from managed code is a different matter - a typed-array read can hold any NaN payload -
/// and <see cref="FromNumber"/> canonicalises every NaN for that reason.
/// </para>
/// <para>
/// <b>This stage defines the layout and nothing reads it but managed code.</b> No emitted code
/// exists for the value form, and no artifact carries a word.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=D89940
// Broiler-Falsified-If: a word this type classifies as a Number is also classified as a tagged value, or a tagged word can be produced by IEEE-754 arithmetic on two Numbers
// Broiler-Human:        PENDING
public static class JsWord
{
    /// <summary>How far a word's tag is shifted: the top sixteen bits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=EE13EB
    // Broiler-Human:        PENDING
    public const int TagShift = 48;

    /// <summary>The smallest tag; every word whose top sixteen bits are below it is a Number.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=B77857
    // Broiler-Falsified-If: the default NaN of an SSE2 operation, or its sign flip, has top sixteen bits at or above this value
    // Broiler-Human:        PENDING
    public const ushort FirstTag = 0xFFF9;

    /// <summary>The tag of a special constant: undefined, null, false, true or empty.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=8332E3
    // Broiler-Human:        PENDING
    public const ushort SpecialTag = 0xFFF9;

    /// <summary>The tag of a String handle.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=1CD6D5
    // Broiler-Human:        PENDING
    public const ushort StringTag = 0xFFFA;

    /// <summary>The tag of an Object handle, every function and array included.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=6703DD
    // Broiler-Human:        PENDING
    public const ushort ObjectTag = 0xFFFB;

    /// <summary>The tag of a Symbol handle.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=293105
    // Broiler-Human:        PENDING
    public const ushort SymbolTag = 0xFFFC;

    /// <summary>The tag of a BigInt handle.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=51BA0F
    // Broiler-Human:        PENDING
    public const ushort BigIntTag = 0xFFFD;

    /// <summary>The tag of a frame header, which is never a value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2C47B0
    // Broiler-Human:        PENDING
    public const ushort HeaderTag = 0xFFFE;

    /// <summary>The tag no word may carry at this layout's version.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=604189
    // Broiler-Human:        PENDING
    public const ushort ReservedTag = 0xFFFF;

    /// <summary>The word for <c>undefined</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=D1FF7A
    // Broiler-Human:        PENDING
    public const ulong Undefined = 0xFFF9_0000_0000_0000UL;

    /// <summary>The word for <c>null</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B506A7
    // Broiler-Human:        PENDING
    public const ulong Null = 0xFFF9_0000_0000_0001UL;

    /// <summary>The word for <c>false</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=849524
    // Broiler-Human:        PENDING
    public const ulong False = 0xFFF9_0000_0000_0002UL;

    /// <summary>The word for <c>true</c>, which is <see cref="False"/> with the low bit set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=B25791
    // Broiler-Human:        PENDING
    public const ulong True = 0xFFF9_0000_0000_0003UL;

    /// <summary>
    /// The word for an empty slot: an uninitialised binding, and an array hole, which the interpreter
    /// represents by the same empty value.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=398351
    // Broiler-Falsified-If: an empty slot and undefined decode to the same value, so a read in the temporal dead zone is not refused
    // Broiler-Human:        PENDING
    public const ulong Empty = 0xFFF9_0000_0000_0004UL;

    /// <summary>How many special payloads the vocabulary defines; every other payload is refused.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=5E543C
    // Broiler-Human:        PENDING
    public const ulong SpecialCount = 5;

    /// <summary>The one NaN a word carries: the positive quiet NaN with a zero payload.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=D48E42
    // Broiler-Falsified-If: FromNumber answers a NaN word other than this one, or this word is not a quiet NaN
    // Broiler-Human:        PENDING
    public const ulong CanonicalNaN = 0x7FF8_0000_0000_0000UL;

    /// <summary>The bits of a word below its tag.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=6071C0
    // Broiler-Human:        PENDING
    public const ulong PayloadMask = 0x0000_FFFF_FFFF_FFFFUL;

    /// <summary>How far a handle's generation is shifted: bits 47 to 32.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=598A00
    // Broiler-Human:        PENDING
    public const int GenerationShift = 32;

    /// <summary>The largest length a frame header can state for its region or its live words.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=0A35DD
    // Broiler-Human:        PENDING
    public const int MaximumFrameLength = 0xFF_FFFF;

    /// <summary>A word's tag: its top sixteen bits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7F3AAB
    // Broiler-Human:        PENDING
    public static ushort Tag(ulong word) => (ushort)(word >> TagShift);

    /// <summary>Whether a word is a Number: one shift and one compare.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=A20747
    // Broiler-Falsified-If: this answers true for a word carrying any tag from FirstTag upwards, or false for a word below it
    // Broiler-Human:        PENDING
    public static bool IsNumber(ulong word) => (word >> TagShift) < FirstTag;

    /// <summary>Whether a word is a handle of one of the four referenced kinds.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=5FE785
    // Broiler-Falsified-If: this answers true for a special constant, a frame header, a reserved word or a Number
    // Broiler-Human:        PENDING
    public static bool IsHandle(ulong word) => Tag(word) is >= StringTag and <= BigIntTag;

    /// <summary>Whether a word is a frame header.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=2BB786
    // Broiler-Human:        PENDING
    public static bool IsHeader(ulong word) => Tag(word) == HeaderTag;

    /// <summary>The word for a Number, with every NaN canonicalised.</summary>
    /// <remarks>
    /// Every other double is carried bit for bit, so negative zero, the infinities and every subnormal
    /// survive a round trip exactly. A NaN's payload is the one thing that does not, and JavaScript
    /// can observe it only through typed-array bytes, which never hold a word.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=F2600F
    // Broiler-Falsified-If: some double answers a word IsNumber rejects, or a non-NaN double does not answer its own bit pattern
    // Broiler-Human:        PENDING
    public static ulong FromNumber(double value) =>
        double.IsNaN(value) ? CanonicalNaN : (ulong)System.BitConverter.DoubleToInt64Bits(value);

    /// <summary>The double a Number word carries.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=CDBCCB
    // Broiler-Human:        PENDING
    public static double ToNumber(ulong word) => System.BitConverter.Int64BitsToDouble((long)word);

    /// <summary>The handle word for one table entry.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=205016
    // Broiler-Falsified-If: the word answered does not carry the tag, the generation and the index it was given, or carries a tag outside the four handle tags
    // Broiler-Human:        PENDING
    public static ulong Handle(ushort tag, ushort generation, uint index)
    {
        if (tag is < StringTag or > BigIntTag)
        {
            throw new System.ArgumentOutOfRangeException(nameof(tag));
        }

        return ((ulong)tag << TagShift) | ((ulong)generation << GenerationShift) | index;
    }

    /// <summary>A handle word's table index: its low thirty-two bits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=84809F
    // Broiler-Human:        PENDING
    public static uint HandleIndex(ulong word) => (uint)word;

    /// <summary>A handle word's generation: bits 47 to 32.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=AF2DFC
    // Broiler-Human:        PENDING
    public static ushort HandleGeneration(ulong word) => (ushort)(word >> GenerationShift);

    /// <summary>A frame header stating the frame's region length and how many of its words are live.</summary>
    /// <remarks>
    /// <b>THE REGION LENGTH IS IN THE HEADER SO THAT A SCAN WALKS FRAME BY FRAME AND NEVER SEARCHES.</b>
    /// A scan that looked for the next header by its tag would meet a stale header a deeper frame left
    /// in what is now a caller's dead words, and could swallow a live frame's words into the stale
    /// one's range and stop rooting them. Walking by stated lengths from the slab's base cannot.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=BC255B
    // Broiler-Falsified-If: a header answers a region or live length other than the ones it was built from, or is built with more live words than its region holds
    // Broiler-Human:        PENDING
    public static ulong Header(int region, int live)
    {
        if (region is < 0 or > MaximumFrameLength || live < 0 || live > region)
        {
            throw new System.ArgumentOutOfRangeException(nameof(live));
        }

        return ((ulong)HeaderTag << TagShift) | ((ulong)(uint)region << 24) | (uint)live;
    }

    /// <summary>The region length a frame header states.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=11F4E5
    // Broiler-Human:        PENDING
    public static int HeaderRegion(ulong word) => (int)((word >> 24) & MaximumFrameLength);

    /// <summary>The live length a frame header states.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=0; Fingerprint=7CB2C2
    // Broiler-Human:        PENDING
    public static int HeaderLive(ulong word) => (int)(word & MaximumFrameLength);
}
