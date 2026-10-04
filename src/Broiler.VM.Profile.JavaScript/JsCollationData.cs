// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   29
// Annotated:        29/29
// Exempt:           15
// Human-reviewed:   0/29
// IP risk:          Low
// Security risk:    Medium
// Criteria:         2/0
// Resource impact:  4/10 max
// Unverified:       29
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The root collation and its tailorings, decoded from the tables the composition handed over, and
/// the Unicode Collation Algorithm's element lookup over them (UTS #10, as CLDR's root modifies it;
/// JSD-0043).
/// </summary>
/// <remarks>
/// <para>
/// <b>Weights are scaled by 256 and packed into one <see cref="ulong"/></b>: the primary in the top 24
/// bits, the secondary in the next 17, the tertiary in the next 13 and the variable flag in the
/// lowest bit. A root weight is a multiple of 256, so a tailored weight - which the generator placed
/// one above its predecessor - sorts between two root weights without moving either.
/// </para>
/// <para>
/// <b>The layouts are the generator's</b> (<c>CldrTableGenerator</c>), little-endian. The root: a
/// count and runs of single elements whose primary rises with the code point (first code point,
/// length, then one element in five bytes: primary, secondary, tertiary with the variable flag in its
/// top bit); a count and the other mappings (key length, key code points, element count, elements);
/// the siniform implicit-weight ranges; and the unified-ideograph ranges. The tailorings: a count,
/// then per tailoring its name, its entries in packed scaled weights, and the code points whose root
/// contractions it suppresses.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=UTS10; IP=Low; Security=Medium; Resources=3; Fingerprint=98A2D4
// Broiler-Human:        PENDING
internal sealed class JsCollationData
{
    /// <summary>The scaled common secondary, 0x0020.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=751350
    // Broiler-Human:        PENDING
    internal const ulong CommonSecondary = 0x20 << 8;

    /// <summary>The scaled common tertiary, 0x0002.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=CFD8EF
    // Broiler-Human:        PENDING
    internal const ulong CommonTertiary = 0x02 << 8;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7BA27F
    // Broiler-Human:        PENDING
    private readonly int[] runFirst;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=5A0334
    // Broiler-Human:        PENDING
    private readonly int[] runLength;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=776A8C
    // Broiler-Human:        PENDING
    private readonly ulong[] runBase;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=273BC7
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<int, ulong[]> singles = [];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B9006A
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, ulong[]> contractions = new(System.StringComparer.Ordinal);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=72791E
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.HashSet<int> contractionStarts = [];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=E8FCB1
    // Broiler-Human:        PENDING
    private readonly (int First, int Last, int Base)[] siniform;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BDC76A
    // Broiler-Human:        PENDING
    private readonly (int First, int Last)[] unified;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=AE4C32
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, Tailoring> tailorings = new(System.StringComparer.Ordinal);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0DF4F7
    // Broiler-Human:        PENDING
    private readonly int maxContraction;

    /// <summary>The scaled primary of the digit zero, where numeric collation puts every number.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=04E0C0
    // Broiler-Human:        PENDING
    internal ulong DigitZero { get; }

    /// <summary>One tailoring: its mappings by key, the longest key, and the code points whose root contractions it suppresses.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=F6AFB1
    // Broiler-Human:        PENDING
    internal sealed class Tailoring
    {
        internal System.Collections.Generic.Dictionary<string, ulong[]> Entries { get; } = new(System.StringComparer.Ordinal);

        internal System.Collections.Generic.HashSet<int> Starts { get; } = [];

        internal System.Collections.Generic.HashSet<int> Suppressed { get; } = [];

        internal int MaxKey { get; set; } = 1;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=09AE3F
    // Broiler-Falsified-If: a table decodes to mappings other than the ones the generator encoded
    // Broiler-Human:        PENDING
    internal JsCollationData(System.ReadOnlySpan<byte> root, System.ReadOnlySpan<byte> tailored)
    {
        var at = 0;
        var runs = ReadInt32(root, ref at);
        runFirst = new int[runs];
        runLength = new int[runs];
        runBase = new ulong[runs];

        for (var index = 0; index < runs; index++)
        {
            runFirst[index] = ReadInt24(root, ref at);
            runLength[index] = ReadInt16(root, ref at);
            runBase[index] = ReadElement(root, ref at);
        }

        var entries = ReadInt32(root, ref at);

        for (var index = 0; index < entries; index++)
        {
            var key = ReadKey(root, ref at, out var first, out var length);
            var elements = new ulong[root[at++]];

            for (var element = 0; element < elements.Length; element++)
            {
                elements[element] = ReadElement(root, ref at);
            }

            if (length == 1)
            {
                singles[first] = elements;
            }
            else
            {
                contractions[key] = elements;
                contractionStarts.Add(first);
                maxContraction = System.Math.Max(maxContraction, length);
            }
        }

        siniform = new (int, int, int)[ReadInt32(root, ref at)];

        for (var index = 0; index < siniform.Length; index++)
        {
            siniform[index] = (ReadInt24(root, ref at), ReadInt24(root, ref at), ReadInt16(root, ref at));
        }

        unified = new (int, int)[ReadInt32(root, ref at)];

        for (var index = 0; index < unified.Length; index++)
        {
            unified[index] = (ReadInt24(root, ref at), ReadInt24(root, ref at));
        }

        at = 0;
        var count = tailored.IsEmpty ? 0 : ReadInt32(tailored, ref at);

        for (var index = 0; index < count; index++)
        {
            var nameLength = tailored[at++];
            var name = System.Text.Encoding.ASCII.GetString(tailored.Slice(at, nameLength));
            at += nameLength;

            var tailoring = new Tailoring();
            var mappings = ReadInt32(tailored, ref at);

            for (var mapping = 0; mapping < mappings; mapping++)
            {
                var key = ReadKey(tailored, ref at, out var first, out var length);
                var elements = new ulong[tailored[at++]];

                for (var element = 0; element < elements.Length; element++)
                {
                    elements[element] = System.Buffers.Binary.BinaryPrimitives.ReadUInt64LittleEndian(tailored.Slice(at, 8));
                    at += 8;
                }

                tailoring.Entries[key] = elements;
                tailoring.Starts.Add(first);
                tailoring.MaxKey = System.Math.Max(tailoring.MaxKey, length);
            }

            var suppressed = ReadInt32(tailored, ref at);

            for (var cp = 0; cp < suppressed; cp++)
            {
                tailoring.Suppressed.Add(ReadInt24(tailored, ref at));
            }

            tailorings[name] = tailoring;
        }

        DigitZero = Primary(Single('0')![0]);
    }

    /// <summary>A tailoring by name - <c>und-search</c>, <c>de-phonebk</c>, <c>de-search</c> - or nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CB72A2
    // Broiler-Human:        PENDING
    internal Tailoring? TailoringNamed(string name) => tailorings.TryGetValue(name, out var tailoring) ? tailoring : null;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=66B2C6
    // Broiler-Human:        PENDING
    internal static ulong Primary(ulong element) => element >> 40;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E90D92
    // Broiler-Human:        PENDING
    internal static ulong Secondary(ulong element) => (element >> 23) & 0x1FFFF;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=215174
    // Broiler-Human:        PENDING
    internal static ulong Tertiary(ulong element) => (element >> 10) & 0x1FFF;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=64F9E2
    // Broiler-Human:        PENDING
    internal static bool Variable(ulong element) => (element & 1) != 0;

    /// <summary>
    /// An element's case: 0 lower or uncased, 1 mixed, 2 upper. A tailored element states it in bits
    /// 1 and 2 with bit 3 set, as the generator read it from the tailored string's root elements; a
    /// root element's case is its tertiary's, by UTS #10's tertiary weight table.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=UTS10 s3.6; IP=Low; Security=Low; Resources=1; Fingerprint=BDAAEA
    // Broiler-Human:        PENDING
    internal static int Case(ulong element)
    {
        if ((element & 0b1000) != 0)
        {
            return (int)((element >> 1) & 0b11);
        }

        var root = Tertiary(element) >> 8;
        return root < (ulong)UpperTertiary.Length && UpperTertiary[root] ? 2 : 0;
    }

    /// <summary>The root tertiaries that mark an upper-case character (UTS #10's tertiary weight table).</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=610477
    // Broiler-Human:        PENDING
    private static readonly bool[] UpperTertiary = UpperTable();

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=BC8151
    // Broiler-Human:        PENDING
    private static bool[] UpperTable()
    {
        var table = new bool[0x20];

        foreach (var upper in new[] { 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x1D })
        {
            table[upper] = true;
        }

        return table;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C92AFD
    // Broiler-Human:        PENDING
    internal static ulong Pack(ulong primary, ulong secondary, ulong tertiary, bool variable) =>
        (primary << 40) | (secondary << 23) | (tertiary << 10) | (variable ? 1UL : 0UL);

    /// <summary>The root's own elements for one code point, or nothing when no key names it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=DFA891
    // Broiler-Human:        PENDING
    internal ulong[]? Single(int codePoint)
    {
        if (singles.TryGetValue(codePoint, out var elements))
        {
            return elements;
        }

        var low = 0;
        var high = runFirst.Length - 1;

        while (low <= high)
        {
            var middle = (low + high) >> 1;

            if (codePoint < runFirst[middle])
            {
                high = middle - 1;
            }
            else if (codePoint >= runFirst[middle] + runLength[middle])
            {
                low = middle + 1;
            }
            else
            {
                return [runBase[middle] + ((ulong)(codePoint - runFirst[middle]) << 48)];
            }
        }

        return null;
    }

    /// <summary>UCA 17's implicit weights for a code point no key names.</summary>
    // Broiler-AI:           Origin=AI; Spec=UTS10 s10.1; IP=Low; Security=Low; Resources=1; Fingerprint=A49CE3
    // Broiler-Human:        PENDING
    internal ulong[] Implicit(int codePoint)
    {
        foreach (var (first, last, @base) in siniform)
        {
            if (codePoint >= first && codePoint <= last)
            {
                var origin = int.MaxValue;

                foreach (var (other, _, otherBase) in siniform)
                {
                    if (otherBase == @base)
                    {
                        origin = System.Math.Min(origin, other);
                    }
                }

                return
                [
                    Pack((ulong)@base << 8, CommonSecondary, CommonTertiary, false),
                    Pack((ulong)((codePoint - origin) | 0x8000) << 8, 0, 0, false),
                ];
            }
        }

        var isUnified = false;

        foreach (var (first, last) in unified)
        {
            if (codePoint >= first && codePoint <= last)
            {
                isUnified = true;
                break;
            }
        }

        var core = codePoint is (>= 0x4E00 and <= 0x9FFF) or (>= 0xF900 and <= 0xFAFF);
        var primary = isUnified && core ? 0xFB40 : isUnified ? 0xFB80 : 0xFBC0;

        return
        [
            Pack((ulong)(primary + (codePoint >> 15)) << 8, CommonSecondary, CommonTertiary, false),
            Pack((ulong)((codePoint & 0x7FFF) | 0x8000) << 8, 0, 0, false),
        ];
    }

    /// <summary>
    /// The collation elements of an NFD code point sequence under a tailoring: UCA's longest match,
    /// its discontiguous extension over unblocked non-starters, and implicit weights for the rest;
    /// numeric runs collapsed to their value when asked for.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=UTS10 s7; IP=Low; Security=Medium; Resources=4; Fingerprint=D27C2E
    // Broiler-Falsified-If: a string's elements differ from the root collation's for any line of the archived CollationTest files
    // Broiler-Human:        PENDING
    internal void Elements(int[] text, Tailoring? tailoring, bool numeric, System.Collections.Generic.List<ulong> output)
    {
        var consumed = new bool[text.Length];

        for (var at = 0; at < text.Length; at++)
        {
            if (consumed[at])
            {
                continue;
            }

            if (numeric && DigitValue(text[at]) >= 0)
            {
                at = Number(text, at, consumed, output) - 1;
                continue;
            }

            var (length, elements) = LongestMatch(text, at, consumed, tailoring);
            var key = length > 1 || CanStartContraction(text[at], tailoring) ? Key(text, at, length) : null;

            // DISCONTIGUOUS MATCHING (UCA S2.1.1-S2.1.3): an unblocked non-starter after the match
            // extends it when the extended key exists, and is consumed out of order.
            if (key is not null)
            {
                var lastSkipped = 0;

                for (var next = at + length; next < text.Length; next++)
                {
                    if (consumed[next])
                    {
                        continue;
                    }

                    var ccc = JsUnicodeNormalization.CombiningClass(text[next]);

                    if (ccc == 0)
                    {
                        break;
                    }

                    if (lastSkipped != 0 && lastSkipped >= ccc)
                    {
                        lastSkipped = ccc;
                        continue;
                    }

                    var extended = key + char.ConvertFromUtf32(Valid(text[next]));

                    if (Find(extended, text[at], tailoring) is { } found)
                    {
                        key = extended;
                        elements = found;
                        consumed[next] = true;
                        continue;
                    }

                    lastSkipped = ccc;
                }
            }

            output.AddRange(elements);
            at += length - 1;
        }
    }

    /// <summary>The longest key at <paramref name="at"/> over contiguous unconsumed code points, and its elements.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=3; Fingerprint=C76060
    // Broiler-Human:        PENDING
    private (int Length, ulong[] Elements) LongestMatch(int[] text, int at, bool[] consumed, Tailoring? tailoring)
    {
        var first = text[at];

        if (CanStartContraction(first, tailoring))
        {
            var longest = System.Math.Max(maxContraction, tailoring?.MaxKey ?? 1);

            for (var length = System.Math.Min(longest, text.Length - at); length > 1; length--)
            {
                var contiguous = true;

                for (var index = at + 1; index < at + length; index++)
                {
                    contiguous &= !consumed[index];
                }

                if (contiguous && Find(Key(text, at, length), first, tailoring) is { } found)
                {
                    return (length, found);
                }
            }
        }

        if (tailoring is not null && tailoring.Entries.TryGetValue(char.ConvertFromUtf32(Valid(first)), out var tailored))
        {
            return (1, tailored);
        }

        return (1, Single(first) ?? Implicit(first));
    }

    /// <summary>A key's elements in the tailoring, then in the root unless the tailoring suppresses the root's contractions of its first code point.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=5F3510
    // Broiler-Human:        PENDING
    private ulong[]? Find(string key, int first, Tailoring? tailoring)
    {
        if (tailoring is not null && tailoring.Entries.TryGetValue(key, out var tailored))
        {
            return tailored;
        }

        if (tailoring is not null && tailoring.Suppressed.Contains(first))
        {
            return null;
        }

        return contractions.TryGetValue(key, out var elements) ? elements : null;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9554E6
    // Broiler-Human:        PENDING
    private bool CanStartContraction(int codePoint, Tailoring? tailoring) =>
        contractionStarts.Contains(codePoint) && (tailoring is null || !tailoring.Suppressed.Contains(codePoint)) ||
        tailoring is not null && tailoring.Starts.Contains(codePoint);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=45CA74
    // Broiler-Human:        PENDING
    private static string Key(int[] text, int at, int length)
    {
        var key = new System.Text.StringBuilder(length * 2);

        for (var index = at; index < at + length; index++)
        {
            key.Append(char.ConvertFromUtf32(Valid(text[index])));
        }

        return key.ToString();
    }

    /// <summary>A code point a key string can hold: a lone surrogate keys as U+FFFD, which no key names.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=548CD9
    // Broiler-Human:        PENDING
    private static int Valid(int codePoint) => codePoint is >= 0xD800 and <= 0xDFFF ? 0xFFFD : codePoint;

    /// <summary>The value of a decimal digit by its root primary, or -1 for anything else.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=9440FA
    // Broiler-Human:        PENDING
    private int DigitValue(int codePoint)
    {
        if (Single(codePoint) is not { Length: 1 } elements)
        {
            return -1;
        }

        var value = (long)(Primary(elements[0]) >> 8) - (long)(DigitZero >> 8);
        return value is >= 0 and <= 9 && Primary(elements[0]) % 256 == 0 ? (int)value : -1;
    }

    /// <summary>
    /// A run of digits as its value: one element for the count of significant digits, then one per
    /// digit, every primary in the gap after the zero's, so numbers sort among themselves by value and
    /// where a digit would among everything else.
    /// </summary>
    // Broiler-AI:           Origin=AI; Spec=UTS35 numeric collation; IP=Low; Security=Low; Resources=2; Fingerprint=9E1457
    // Broiler-Human:        PENDING
    private int Number(int[] text, int at, bool[] consumed, System.Collections.Generic.List<ulong> output)
    {
        var digits = new System.Collections.Generic.List<int>();

        while (at < text.Length && !consumed[at] && DigitValue(text[at]) is var digit and >= 0)
        {
            if (digits.Count != 0 || digit != 0)
            {
                digits.Add(digit);
            }

            at++;
        }

        if (digits.Count == 0)
        {
            digits.Add(0);
        }

        output.Add(Pack(DigitZero + 1 + (ulong)System.Math.Min(digits.Count, 244), CommonSecondary, CommonTertiary, false));

        foreach (var value in digits)
        {
            output.Add(Pack(DigitZero + 1 + (ulong)value, CommonSecondary, CommonTertiary, false));
        }

        return at;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=CF2FEA
    // Broiler-Human:        PENDING
    private static string ReadKey(System.ReadOnlySpan<byte> data, ref int at, out int first, out int length)
    {
        length = data[at++];
        var key = new System.Text.StringBuilder(length * 2);
        first = 0;

        for (var index = 0; index < length; index++)
        {
            var codePoint = ReadInt24(data, ref at);

            if (index == 0)
            {
                first = codePoint;
            }

            key.Append(char.ConvertFromUtf32(Valid(codePoint)));
        }

        return key.ToString();
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=73B4F7
    // Broiler-Human:        PENDING
    private static ulong ReadElement(System.ReadOnlySpan<byte> data, ref int at)
    {
        var primary = (ulong)ReadInt16(data, ref at);
        var secondary = (ulong)ReadInt16(data, ref at);
        var tertiary = data[at++];
        return Pack(primary << 8, secondary << 8, (ulong)(tertiary & 0x7F) << 8, (tertiary & 0x80) != 0);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=E6C3DC
    // Broiler-Human:        PENDING
    private static int ReadInt32(System.ReadOnlySpan<byte> data, ref int at)
    {
        var value = data[at] | (data[at + 1] << 8) | (data[at + 2] << 16) | (data[at + 3] << 24);
        at += 4;
        return value;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D342BF
    // Broiler-Human:        PENDING
    private static int ReadInt24(System.ReadOnlySpan<byte> data, ref int at)
    {
        var value = data[at] | (data[at + 1] << 8) | (data[at + 2] << 16);
        at += 3;
        return value;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=D282AD
    // Broiler-Human:        PENDING
    private static int ReadInt16(System.ReadOnlySpan<byte> data, ref int at)
    {
        var value = data[at] | (data[at + 1] << 8);
        at += 2;
        return value;
    }
}
