// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   120
// Annotated:        120/120
// Exempt:           93
// Human-reviewed:   0/120
// IP risk:          Medium
// Security risk:    Medium
// Criteria:         1/0
// Resource impact:  6/10 max
// Unverified:       120
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>How much work a running match has done, handed to whatever owns the meter.</summary>
/// <remarks>
/// The matcher takes a callback rather than a budget of its own because backtracking is unbounded
/// work and this profile charges fuel for everything: a pattern that backtracks catastrophically
/// has to spend the guest's allowance and end as a resource exhaustion, not run to completion on a
/// private allowance nobody granted it. The callback is <c>JsEngine.Charge</c> in every caller this
/// assembly has, and that method is what raises the abort when the allowance is gone - which is why
/// nothing here inspects a return value, and why every instruction dispatched is counted.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=4; Fingerprint=B784C6
// Broiler-Human:        PENDING
internal delegate void JsRegExpCharge(ulong units);

/// <summary>A pattern the parser refused, carrying the reason the language would report.</summary>
/// <remarks>
/// It is a separate exception rather than a <c>JsThrow</c> because the parser is not given an
/// engine: it knows what is wrong with the text and not which realm is asking, and the caller that
/// does know turns this into the <c>SyntaxError</c> the guest sees.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=BD494E
// Broiler-Human:        PENDING
internal sealed class JsRegExpSyntaxError : System.Exception
{
    /// <summary>Creates a refusal carrying <paramref name="message"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F1AB47
    // Broiler-Human:        PENDING
    internal JsRegExpSyntaxError(string message)
        : base(message)
    {
    }
}

/// <summary>A match that reached one of the two ceilings the matcher declares for itself.</summary>
/// <remarks>
/// The backtrack stack and the undo trail are the two structures a pattern can grow without
/// consuming input, and a host that granted an unbounded fuel allowance would otherwise let one of
/// them grow until the process died of it. The ceilings are stated in
/// <see cref="JsRegExpMatcher"/>'s remarks and reached only by a pattern that was going to be
/// refused anyway; the caller reports this as a resource exhaustion the guest cannot catch.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=880EA2
// Broiler-Human:        PENDING
internal sealed class JsRegExpOverflowError : System.Exception
{
    /// <summary>Creates an overflow carrying <paramref name="message"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=94F856
    // Broiler-Human:        PENDING
    internal JsRegExpOverflowError(string message)
        : base(message)
    {
    }
}

/// <summary>
/// The specification's <c>Canonicalize</c>, and the closure over the characters it identifies.
/// </summary>
/// <remarks>
/// <para>
/// <b>Two different foldings, because the language has two.</b> Without <c>u</c>, canonicalisation
/// is the simple upper-case mapping with the rule that a non-ASCII character whose upper case is
/// ASCII does not fold - which is what keeps <c>/ſ/i</c> from matching <c>"S"</c>. With
/// <c>u</c> it is Unicode's simple case folding, under which the long s and the Kelvin sign DO join
/// their ASCII neighbours. Both are computed from the invariant culture's simple mappings rather
/// than from a table shipped here: <c>ToUpperInvariant</c> is a one-to-one mapping, so the
/// specification's "return the character unchanged when the upper case is longer than one
/// character" clause is satisfied by construction rather than by a length test.
/// </para>
/// <para>
/// <b>What that costs, stated rather than hidden.</b> Simple case folding is approximated as
/// <c>lower(upper(c))</c> with <c>U+0130</c> and <c>U+0131</c> excluded by hand, because those two
/// are the only characters in the invariant mapping whose round trip crosses a fold the
/// <c>CaseFolding</c> file does not make. Every other pair this reproduces - the final sigma, the
/// combining iota, the Kelvin sign, capital sharp s, the Cherokee case pairs - is the file's own
/// answer. A character folded wrongly by this approximation would show up as a case-insensitive
/// match the comparison engine does not make; none has been found.
/// </para>
/// <para>
/// <b>The closure is built once and read many times.</b> A class such as <c>[k]</c> under <c>iu</c>
/// has to match the Kelvin sign, and the only way to know that from the input character is to know
/// every character that folds to <c>k</c>. That is a reverse mapping, and it is built by walking
/// the Basic Multilingual Plane once on first use - about fifteen hundred characters have a fold
/// at all - rather than by iterating a class's ranges at compile time, which a class spanning the
/// whole plane would make quadratic.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=3; Fingerprint=854A69
// Broiler-Human:        PENDING
internal static class JsRegExpCase
{
    /// <summary>The answer for a character that is alone in its equivalence class.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3511E0
    // Broiler-Human:        PENDING
    private static readonly int[] Alone = [];

    /// <summary>Every character that simple-case-folds to a given character, that one included.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D314FC
    // Broiler-Human:        PENDING
    private static readonly System.Lazy<System.Collections.Generic.Dictionary<int, int[]>> FoldedVariants =
        new(static () => BuildVariants(true));

    /// <summary>Every character that upper-cases to a given character, that one included.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7926C2
    // Broiler-Human:        PENDING
    private static readonly System.Lazy<System.Collections.Generic.Dictionary<int, int[]>> UpperVariants =
        new(static () => BuildVariants(false));

    /// <summary>The specification's <c>Canonicalize</c> for one code point.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=3; Fingerprint=D8C8EA
    // Broiler-Human:        PENDING
    internal static int Canonicalize(int codePoint, bool unicode) =>
        unicode ? Fold(codePoint) : Upper(codePoint);

    /// <summary>
    /// Every code point whose canonical form is that of <paramref name="codePoint"/>, or an empty
    /// array when it stands alone.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=3; Fingerprint=D84E2D
    // Broiler-Human:        PENDING
    internal static int[] Variants(int codePoint, bool unicode)
    {
        var canonical = Canonicalize(codePoint, unicode);

        if (codePoint > 0xFFFF || canonical > 0xFFFF)
        {
            // ASTRAL CASE PAIRS ARE NOT IN THE TABLE, because building it over the whole of Unicode
            // to serve Deseret and Adlam would be a megabyte nobody asked for. The pair is
            // reconstructed instead: a folded form and whatever upper-cases to it.
            var mate = UpperOf(canonical);
            return mate == canonical ? [canonical] : [canonical, mate];
        }

        var table = unicode ? FoldedVariants.Value : UpperVariants.Value;
        return table.TryGetValue(canonical, out var found) ? found : Alone;
    }

    /// <summary>Unicode simple case folding, approximated from the invariant simple mappings.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=3; Fingerprint=6122F8
    // Broiler-Human:        PENDING
    private static int Fold(int codePoint)
    {
        // The dotted capital I and the dotless small i fold to themselves: the CaseFolding file
        // gives each only a Turkic and a full mapping, and a round trip through an upper-case
        // mapping that had them would otherwise join both to plain `i`.
        if (codePoint is 0x0130 or 0x0131)
        {
            return codePoint;
        }

        // THE LONG S IS THE ONE CHARACTER THE HOST'S INVARIANT MAPPINGS DO NOT CARRY. Compositions
        // of this profile run with globalization set to invariant, and that mode leaves U+017F
        // alone where Unicode upper-cases it to `S`; every other character in the plane folds the
        // same way with the mappings present and absent, which was checked rather than assumed.
        if (codePoint == 0x017F)
        {
            return 0x0073;
        }

        if (codePoint <= 0xFFFF)
        {
            return char.ToLowerInvariant(char.ToUpperInvariant((char)codePoint));
        }

        var text = char.ConvertFromUtf32(codePoint).ToUpperInvariant().ToLowerInvariant();

        if (text.Length == 2 && char.IsHighSurrogate(text[0]) && char.IsLowSurrogate(text[1]))
        {
            return char.ConvertToUtf32(text[0], text[1]);
        }

        return text.Length == 1 ? text[0] : codePoint;
    }

    /// <summary>The non-unicode canonicalisation: simple upper case, with the ASCII guard.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=3; Fingerprint=671101
    // Broiler-Human:        PENDING
    private static int Upper(int codePoint)
    {
        if (codePoint > 0xFFFF)
        {
            return codePoint;
        }

        var upper = char.ToUpperInvariant((char)codePoint);

        // A NON-ASCII CHARACTER WHOSE UPPER CASE IS ASCII DOES NOT FOLD. Without this clause the
        // long s would match "S" and the dotless i would match "I", neither of which the language
        // does outside `u` mode.
        return codePoint >= 128 && upper < 128 ? codePoint : upper;
    }

    /// <summary>The simple upper case of one code point, surrogate pairs included.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=83CDE6
    // Broiler-Human:        PENDING
    private static int UpperOf(int codePoint)
    {
        if (codePoint <= 0xFFFF)
        {
            return char.ToUpperInvariant((char)codePoint);
        }

        var text = char.ConvertFromUtf32(codePoint).ToUpperInvariant();

        if (text.Length == 2 && char.IsHighSurrogate(text[0]) && char.IsLowSurrogate(text[1]))
        {
            return char.ConvertToUtf32(text[0], text[1]);
        }

        return codePoint;
    }

    /// <summary>Walks the Basic Multilingual Plane once, collecting the reverse mapping.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=3; Fingerprint=9D333C
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.Dictionary<int, int[]> BuildVariants(bool unicode)
    {
        var collected =
            new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>>();

        for (var codePoint = 0; codePoint <= 0xFFFF; codePoint++)
        {
            if (char.IsSurrogate((char)codePoint))
            {
                continue;
            }

            var canonical = unicode ? Fold(codePoint) : Upper(codePoint);

            if (canonical == codePoint)
            {
                continue;
            }

            if (!collected.TryGetValue(canonical, out var members))
            {
                members = [canonical];
                collected[canonical] = members;
            }

            members.Add(codePoint);
        }

        var table = new System.Collections.Generic.Dictionary<int, int[]>(collected.Count);

        foreach (var pair in collected)
        {
            table[pair.Key] = pair.Value.ToArray();
        }

        return table;
    }
}

/// <summary>A set of code points: sorted, merged ranges and a negation bit.</summary>
/// <remarks>
/// Ranges rather than a bitmap, because a class under <c>u</c> spans a million code points and a
/// bitmap of that is 128 kilobytes per class. Membership is a binary search, which is the cost a
/// class pays per character; the alternative - expanding a range into its case-folded members at
/// compile time - is quadratic in the width of the range and was rejected for it.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0F1A03
// Broiler-Human:        PENDING
internal sealed class JsRegExpCharSet
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=49A16A
    // Broiler-Human:        PENDING
    private int[] bounds = new int[8];

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=71170F
    // Broiler-Human:        PENDING
    private int count;

    /// <summary>Whether membership is inverted, which is what <c>[^...]</c> sets.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5BBBFD
    // Broiler-Human:        PENDING
    internal bool Negated { get; set; }

    /// <summary>Adds one inclusive range.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=417C5B
    // Broiler-Human:        PENDING
    internal void Add(int low, int high)
    {
        if (high < low)
        {
            return;
        }

        if ((count * 2) == bounds.Length)
        {
            System.Array.Resize(ref bounds, bounds.Length * 2);
        }

        bounds[count * 2] = low;
        bounds[(count * 2) + 1] = high;
        count++;
    }

    /// <summary>Adds every range of <paramref name="other"/>, negation ignored.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=2828BB
    // Broiler-Human:        PENDING
    internal void AddAll(int[] pairs)
    {
        for (var at = 0; at < pairs.Length; at += 2)
        {
            Add(pairs[at], pairs[at + 1]);
        }
    }

    /// <summary>Adds the complement of <paramref name="pairs"/> up to <paramref name="ceiling"/>.</summary>
    /// <remarks>
    /// This is what puts <c>\D</c> inside a class: the language's <c>[\D]</c> is the set of members
    /// that are not digits, and a class carrying it alongside other members is their union rather
    /// than an inversion of the class as a whole.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=99BD00
    // Broiler-Human:        PENDING
    internal void AddComplement(int[] pairs, int ceiling)
    {
        var next = 0;

        for (var at = 0; at < pairs.Length; at += 2)
        {
            if (pairs[at] > next)
            {
                Add(next, pairs[at] - 1);
            }

            next = System.Math.Max(next, pairs[at + 1] + 1);
        }

        if (next <= ceiling)
        {
            Add(next, ceiling);
        }
    }

    /// <summary>Sorts and merges the ranges, after which the set may be searched.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=256762
    // Broiler-Human:        PENDING
    internal void Freeze()
    {
        var lows = new int[count];
        var highs = new int[count];

        for (var at = 0; at < count; at++)
        {
            lows[at] = bounds[at * 2];
            highs[at] = bounds[(at * 2) + 1];
        }

        System.Array.Sort(lows, highs);

        var written = 0;

        for (var at = 0; at < count; at++)
        {
            if (written > 0 && lows[at] <= bounds[((written - 1) * 2) + 1] + 1)
            {
                var end = bounds[((written - 1) * 2) + 1];
                bounds[((written - 1) * 2) + 1] = System.Math.Max(end, highs[at]);
                continue;
            }

            bounds[written * 2] = lows[at];
            bounds[(written * 2) + 1] = highs[at];
            written++;
        }

        count = written;
    }

    /// <summary>The frozen ranges, as low-high pairs, for a set built out of another.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=0E64A2
    // Broiler-Human:        PENDING
    internal int[] Pairs()
    {
        var pairs = new int[count * 2];
        System.Array.Copy(bounds, pairs, pairs.Length);
        return pairs;
    }

    /// <summary>Whether the set lists <paramref name="codePoint"/>, negation not applied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FECA63
    // Broiler-Human:        PENDING
    internal bool Lists(int codePoint)
    {
        var low = 0;
        var high = count - 1;

        while (low <= high)
        {
            var middle = (low + high) / 2;

            if (codePoint < bounds[middle * 2])
            {
                high = middle - 1;
            }
            else if (codePoint > bounds[(middle * 2) + 1])
            {
                low = middle + 1;
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>The specification's <c>CharacterSetMatcher</c>: membership, folded and inverted.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=3; Fingerprint=2598C5
    // Broiler-Human:        PENDING
    internal bool Matches(int codePoint, bool ignoreCase, bool unicode)
    {
        var found = Lists(codePoint);

        if (!found && ignoreCase)
        {
            // THE COMPARISON IS BETWEEN CANONICAL FORMS AND NOT BETWEEN CHARACTERS, so a class
            // listing `k` matches the Kelvin sign and a class listing the Kelvin sign matches `k`.
            // Walking the closure of the INPUT character is what makes both directions work with
            // the class left exactly as it was written.
            foreach (var variant in JsRegExpCase.Variants(codePoint, unicode))
            {
                if (variant != codePoint && Lists(variant))
                {
                    found = true;
                    break;
                }
            }
        }

        return Negated ? !found : found;
    }
}

/// <summary>One successful match: the code-unit offsets of the whole match and every capture.</summary>
/// <remarks>
/// A pair of offsets per group, with <c>-1</c> for a group that did not participate. Offsets rather
/// than strings because <c>replace</c> and <c>split</c> want the surrounding text as often as the
/// matched text, and because a group that matched nothing and a group that did not participate are
/// then told apart by a comparison rather than by a null check on a string.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1C3AB7
// Broiler-Human:        PENDING
internal sealed class JsRegExpMatch
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5B9EE1
    // Broiler-Human:        PENDING
    private readonly int[] slots;

    /// <summary>Creates a match over a slot array the runner has finished with.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B80C07
    // Broiler-Human:        PENDING
    internal JsRegExpMatch(int[] captured) => slots = captured;

    /// <summary>Where the whole match begins.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=522C62
    // Broiler-Human:        PENDING
    internal int Index => slots[0];

    /// <summary>One past where the whole match ends.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=F1E9E9
    // Broiler-Human:        PENDING
    internal int End => slots[1];

    /// <summary>How many code units the whole match covers.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5159FE
    // Broiler-Human:        PENDING
    internal int Length => slots[1] - slots[0];

    /// <summary>How many capture groups the pattern declared.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=1B44AF
    // Broiler-Human:        PENDING
    internal int CaptureCount => (slots.Length / 2) - 1;

    /// <summary>Whether the numbered group took part in this match.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=68DD0E
    // Broiler-Human:        PENDING
    internal bool Participated(int group) => slots[group * 2] >= 0 && slots[(group * 2) + 1] >= 0;

    /// <summary>The text the numbered group matched.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=B835E3
    // Broiler-Human:        PENDING
    internal string TextOf(string input, int group) =>
        input.Substring(slots[group * 2], slots[(group * 2) + 1] - slots[group * 2]);
}

/// <summary>
/// The regular-expression matcher this profile owns: a parser, a lowering, and a backtracking
/// machine with an explicit stack.
/// </summary>
/// <remarks>
/// <para>
/// <b>Compile then backtrack, and the lowering is to an instruction array rather than a walk of the
/// tree.</b> The parser builds a node tree, which is written once and read afterwards, and the
/// lowering turns it into a flat array of instructions. Walking the tree would have been less code;
/// what the array buys is the thing this profile cannot do without, which is a machine whose whole
/// state is four integers and three arrays it owns. A tree walk expresses backtracking as a
/// continuation - one CLR frame per pending alternative - and a pattern with a quantifier over a
/// long input then needs a native stack proportional to the INPUT, which is the shape that
/// terminated a process once already and is recorded as JSC-79. The instruction array's backtrack
/// points are entries in an array that grows on the heap, so the deepest pattern over the longest
/// input uses exactly as much CLR stack as the shallowest.
/// </para>
/// <para>
/// <b>The parser recurses and is bounded for it.</b> Recursive descent over the pattern grammar is
/// the readable form and it is kept, with a nesting counter that refuses past
/// <see cref="MaximumNestingDepth"/> groups deep with a <c>SyntaxError</c>. That is a divergence -
/// the language has no such bound and the comparison engine accepts far deeper - and it is the
/// declared one: a refusal the guest can see, rather than a stack the host cannot recover from. The
/// lowering recurses over the same tree and is bounded by the same counter.
/// </para>
/// <para>
/// <b>Every instruction dispatched is charged.</b> The machine counts steps and hands them to the
/// caller's meter in blocks; the meter is <c>JsEngine.Charge</c>, which is where a spent allowance
/// becomes an abort and where cancellation is polled. A catastrophically backtracking pattern
/// therefore spends the guest's fuel and ends as a resource exhaustion with a named dimension. Two
/// further ceilings exist for a host that granted an unbounded allowance: the backtrack stack is
/// capped at <see cref="MaximumFrames"/> entries and the undo trail at
/// <see cref="MaximumTrail"/>, and reaching either is reported the same way. Neither is reachable
/// by a pattern that was going to answer.
/// </para>
/// <para>
/// <b>What it does not do.</b> No <c>\p{...}</c> or <c>\P{...}</c> property escapes - under <c>u</c>
/// they are a <c>SyntaxError</c> and outside it they are the identity escape Annex B makes them. No
/// <c>v</c> flag and none of its set operations. The <c>d</c> flag is parsed, ordered and reported
/// by <c>hasIndices</c>, and no <c>indices</c> array is built for a result. Case folding is
/// computed from the invariant culture's simple mappings rather than from a shipped table, which
/// <see cref="JsRegExpCase"/> states the cost of.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=FD2409
// Broiler-Falsified-If: a pattern's nesting depth or an input's length drives the CLR stack this matcher uses
// Broiler-Human:        PENDING
internal sealed class JsRegExpMatcher
{
    /// <summary>How deeply a pattern may nest groups before the parser refuses it.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=ABF9A7
    // Broiler-Human:        PENDING
    private const int MaximumNestingDepth = 128;

    /// <summary>How many backtrack points one match may hold at once.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=B6D684
    // Broiler-Human:        PENDING
    private const int MaximumFrames = 1 << 20;

    /// <summary>How many cell writes one match may have to undo at once.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=572C8D
    // Broiler-Human:        PENDING
    private const int MaximumTrail = 1 << 21;

    /// <summary>How many instructions are dispatched between two charges to the meter.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=A0B9BF
    // Broiler-Human:        PENDING
    private const ulong StepsPerCharge = 512;

    /// <summary>The code points <c>\d</c> stands for.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=65340C
    // Broiler-Human:        PENDING
    private static readonly int[] DigitRanges = [0x30, 0x39];

    /// <summary>The code points <c>\w</c> stands for.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=415D23
    // Broiler-Human:        PENDING
    private static readonly int[] WordRanges = [0x30, 0x39, 0x41, 0x5A, 0x5F, 0x5F, 0x61, 0x7A];

    /// <summary>The code points <c>\s</c> stands for: <c>WhiteSpace</c> and <c>LineTerminator</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=E04EA7
    // Broiler-Human:        PENDING
    private static readonly int[] SpaceRanges =
    [
        0x09, 0x0D, 0x20, 0x20, 0xA0, 0xA0, 0x1680, 0x1680, 0x2000, 0x200A,
        0x2028, 0x2029, 0x202F, 0x202F, 0x205F, 0x205F, 0x3000, 0x3000, 0xFEFF, 0xFEFF,
    ];

    /// <summary>The word characters this pattern's flags make, as a set, for <c>\b</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=B617EE
    // Broiler-Human:        PENDING
    private readonly JsRegExpCharSet wordCharacters;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=12584A
    // Broiler-Human:        PENDING
    private readonly Instruction[] code;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=20A79C
    // Broiler-Human:        PENDING
    private readonly JsRegExpCharSet[] classes;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=F24452
    // Broiler-Human:        PENDING
    private readonly string?[] groupNames;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=5EA9C3
    // Broiler-Human:        PENDING
    private readonly int cellCount;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=8E74AE
    // Broiler-Human:        PENDING
    private readonly int prefilterKind;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=2FD701
    // Broiler-Human:        PENDING
    private readonly int prefilterValue;

    /// <summary>Creates a compiled matcher out of a finished lowering.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=B59D0F
    // Broiler-Human:        PENDING
    private JsRegExpMatcher(
        Instruction[] program,
        JsRegExpCharSet[] sets,
        string?[] names,
        int captures,
        int cells,
        bool ignoreCase,
        bool multiline,
        bool dotAll,
        bool unicode)
    {
        code = program;
        classes = sets;
        groupNames = names;
        wordCharacters = BuildSet(WordRangesFor(ignoreCase, unicode));
        CaptureCount = captures;
        cellCount = cells;
        IgnoreCase = ignoreCase;
        Multiline = multiline;
        DotAll = dotAll;
        Unicode = unicode;

        // THE PREFILTER IS THE ONE OPTIMISATION HERE AND IT IS DELIBERATELY THE CHEAPEST ONE. A
        // pattern whose first act is to consume a character cannot begin anywhere that character
        // does not appear, so the scan skips those positions without entering the machine at all.
        // Anything else - a split, an assertion, a loop head - leaves the scan as it was.
        var kind = 0;
        var value = 0;

        for (var at = 0; at < program.Length; at++)
        {
            var instruction = program[at];

            if (instruction.Op is Op.Save or Op.Clear or Op.SetCell or Op.MarkPos)
            {
                continue;
            }

            if (!instruction.Backward && instruction.Op is Op.Char or Op.Set)
            {
                kind = instruction.Op == Op.Char ? 1 : 2;
                value = instruction.A;
            }

            break;
        }

        prefilterKind = kind;
        prefilterValue = value;

        foreach (var name in names)
        {
            if (name is not null)
            {
                HasGroupNames = true;
                break;
            }
        }
    }

    /// <summary>How many capture groups the pattern declared.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=CE5256
    // Broiler-Human:        PENDING
    internal int CaptureCount { get; }

    /// <summary>Whether <c>i</c> was set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=E728A2
    // Broiler-Human:        PENDING
    internal bool IgnoreCase { get; }

    /// <summary>Whether <c>m</c> was set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=65CEE8
    // Broiler-Human:        PENDING
    internal bool Multiline { get; }

    /// <summary>Whether <c>s</c> was set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=F47481
    // Broiler-Human:        PENDING
    internal bool DotAll { get; }

    /// <summary>Whether <c>u</c> was set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=224DE1
    // Broiler-Human:        PENDING
    internal bool Unicode { get; }

    /// <summary>Whether any group in the pattern was given a name.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=99DE1D
    // Broiler-Human:        PENDING
    internal bool HasGroupNames { get; }

    /// <summary>The name of the numbered group, or <see langword="null"/> when it has none.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=4DFE52
    // Broiler-Human:        PENDING
    internal string? NameOf(int group) => group < groupNames.Length ? groupNames[group] : null;

    /// <summary>The number of the group carrying <paramref name="name"/>, or <c>-1</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=8AD358
    // Broiler-Human:        PENDING
    internal int NumberOf(string name)
    {
        for (var at = 1; at < groupNames.Length; at++)
        {
            if (string.Equals(groupNames[at], name, System.StringComparison.Ordinal))
            {
                return at;
            }
        }

        return -1;
    }

    /// <summary>Parses and lowers one pattern, or refuses it.</summary>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=52468E
    // Broiler-Human:        PENDING
    internal static JsRegExpMatcher Compile(
        string source, bool ignoreCase, bool multiline, bool dotAll, bool unicode)
    {
        var parser = new Parser(source, unicode, dotAll, ignoreCase);
        var root = parser.Parse();
        var emitter = new Emitter(ignoreCase, unicode);
        emitter.Lower(root);

        return new JsRegExpMatcher(
            emitter.Program(),
            emitter.Sets(),
            parser.Names(),
            parser.CaptureCount,
            emitter.CellCount,
            ignoreCase,
            multiline,
            dotAll,
            unicode);
    }

    /// <summary>
    /// Matches at or after <paramref name="start"/>, or exactly at it when
    /// <paramref name="anchored"/> is set.
    /// </summary>
    /// <remarks>
    /// <b>The anchored form is a real anchor and not a search whose answer is thrown away.</b> A
    /// sticky pattern that fails at <c>lastIndex</c> costs one attempt at that position and stops,
    /// which is what the flag is for and what an emulation over a forward-searching engine cannot
    /// give.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=3346A7
    // Broiler-Human:        PENDING
    internal JsRegExpMatch? Match(string input, int start, bool anchored, JsRegExpCharge? charge)
    {
        if (start < 0 || start > input.Length)
        {
            return null;
        }

        var runner = new Runner(this, input, charge);
        var at = start;

        while (true)
        {
            if (!anchored && prefilterKind != 0)
            {
                var skipped = 0;

                while (at < input.Length && !PrefilterAdmits(input, at))
                {
                    at = Advance(input, at, Unicode);
                    skipped++;
                }

                runner.Spend((ulong)skipped);

                if (at >= input.Length)
                {
                    runner.Settle();
                    return null;
                }
            }

            if (runner.Attempt(at))
            {
                runner.Settle();
                return new JsRegExpMatch(runner.Captured(CaptureCount));
            }

            if (anchored || at >= input.Length)
            {
                runner.Settle();
                return null;
            }

            at = Advance(input, at, Unicode);
        }
    }

    /// <summary>The specification's <c>AdvanceStringIndex</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=5E4C98
    // Broiler-Human:        PENDING
    internal static int Advance(string input, int index, bool unicode)
    {
        if (unicode &&
            index + 1 < input.Length &&
            char.IsHighSurrogate(input[index]) &&
            char.IsLowSurrogate(input[index + 1]))
        {
            return index + 2;
        }

        return index + 1;
    }

    /// <summary>Whether a start position survives the first-character filter.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=839DD4
    // Broiler-Human:        PENDING
    private bool PrefilterAdmits(string input, int at)
    {
        var unit = input[at];
        var codePoint = (int)unit;

        if (Unicode &&
            char.IsHighSurrogate(unit) &&
            at + 1 < input.Length &&
            char.IsLowSurrogate(input[at + 1]))
        {
            codePoint = char.ConvertToUtf32(unit, input[at + 1]);
        }

        if (prefilterKind == 1)
        {
            var folded = IgnoreCase ? JsRegExpCase.Canonicalize(codePoint, Unicode) : codePoint;
            return folded == prefilterValue;
        }

        return classes[prefilterValue].Matches(codePoint, IgnoreCase, Unicode);
    }

    /// <summary>
    /// The code points <c>\w</c> stands for under one set of flags.
    /// </summary>
    /// <remarks>
    /// <b>Under <c>u</c> AND <c>i</c> together the word characters are not just the ASCII ones.</b>
    /// The specification says so in as many words: a character whose canonical form is one of
    /// <c>[0-9A-Za-z_]</c> is itself a word character, which brings in the long s and the Kelvin
    /// sign - and takes them OUT of <c>\W</c>, which is the direction that is easy to get wrong.
    /// The set is computed from the folding rather than written down, so it stays right if the
    /// folding is ever corrected.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=9D5633
    // Broiler-Human:        PENDING
    private static int[] WordRangesFor(bool ignoreCase, bool unicode)
    {
        if (!ignoreCase || !unicode)
        {
            return WordRanges;
        }

        var set = new JsRegExpCharSet();
        set.AddAll(WordRanges);

        for (var pair = 0; pair < WordRanges.Length; pair += 2)
        {
            for (var codePoint = WordRanges[pair]; codePoint <= WordRanges[pair + 1]; codePoint++)
            {
                foreach (var variant in JsRegExpCase.Variants(codePoint, true))
                {
                    set.Add(variant, variant);
                }
            }
        }

        set.Freeze();
        return set.Pairs();
    }

    /// <summary>Builds a frozen set over one range table.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=1B9585
    // Broiler-Human:        PENDING
    private static JsRegExpCharSet BuildSet(int[] pairs)
    {
        var set = new JsRegExpCharSet();
        set.AddAll(pairs);
        set.Freeze();
        return set;
    }

    /// <summary>What one instruction of the lowered program does.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=4D7A7A
    // Broiler-Human:        PENDING
    private enum Op : byte
    {
        /// <summary>Consume one character equal to <c>A</c> once canonicalised.</summary>
        Char = 0,

        /// <summary>Consume one character the class numbered <c>A</c> admits.</summary>
        Set = 1,

        /// <summary>Continue at <c>A</c>, keeping <c>B</c> as the alternative.</summary>
        Split = 2,

        /// <summary>Continue at <c>A</c>.</summary>
        Jump = 3,

        /// <summary>Write the current position into cell <c>A</c>.</summary>
        Save = 4,

        /// <summary>Set cells <c>A</c> through <c>B</c> to "did not participate".</summary>
        Clear = 5,

        /// <summary>The <c>^</c> assertion.</summary>
        Bol = 6,

        /// <summary>The <c>$</c> assertion.</summary>
        Eol = 7,

        /// <summary>The <c>\b</c> assertion, or <c>\B</c> when <c>A</c> is one.</summary>
        Word = 8,

        /// <summary>Consume what capture group <c>A</c> matched.</summary>
        BackRef = 9,

        /// <summary>Open the assertion of kind <c>A</c>, whose continuation is <c>B</c>.</summary>
        AssertBegin = 10,

        /// <summary>Close the innermost open assertion, its body having matched.</summary>
        AssertEnd = 11,

        /// <summary>Write the constant <c>B</c> into cell <c>A</c>.</summary>
        SetCell = 12,

        /// <summary>Write the current position into cell <c>A</c>, for the empty check.</summary>
        MarkPos = 13,

        /// <summary>Add one to cell <c>A</c>.</summary>
        IncCell = 14,

        /// <summary>Continue at <c>C</c> when cell <c>A</c> is at least <c>B</c>.</summary>
        JumpIfAtLeast = 15,

        /// <summary>Continue at <c>C</c> when cell <c>A</c> is below <c>B</c>.</summary>
        JumpIfBelow = 16,

        /// <summary>Fail the iteration that consumed nothing when it was not a required one.</summary>
        EmptyCheck = 17,

        /// <summary>The whole pattern has matched.</summary>
        Accept = 18,
    }

    /// <summary>One lowered instruction: an operation and three operands.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=DB5F6A
    // Broiler-Human:        PENDING
    private struct Instruction
    {
        /// <summary>What this instruction does.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=78AEF8
        // Broiler-Human:        PENDING
        internal Op Op;

        /// <summary>The first operand.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=0D182F
        // Broiler-Human:        PENDING
        internal int A;

        /// <summary>The second operand.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=F34B25
        // Broiler-Human:        PENDING
        internal int B;

        /// <summary>The third operand.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=880103
        // Broiler-Human:        PENDING
        internal int C;

        /// <summary>Whether this instruction runs right to left, inside a lookbehind.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=FD6659
        // Broiler-Human:        PENDING
        internal bool Backward;
    }

    /// <summary>What one node of the parsed pattern is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=B059D9
    // Broiler-Human:        PENDING
    private enum NodeKind : byte
    {
        /// <summary>Matches the empty string.</summary>
        Empty = 0,

        /// <summary>One literal character.</summary>
        Char = 1,

        /// <summary>One character class, the dot included.</summary>
        Set = 2,

        /// <summary>Its children in order.</summary>
        Sequence = 3,

        /// <summary>The first of its children that matches.</summary>
        Alternation = 4,

        /// <summary>Its child, between <c>A</c> and <c>B</c> times.</summary>
        Repeat = 5,

        /// <summary>Its child, remembering where it began and ended.</summary>
        Capture = 6,

        /// <summary>The assertion of kind <c>A</c> over its child.</summary>
        Look = 7,

        /// <summary>Whatever capture group <c>A</c> matched.</summary>
        BackRef = 8,

        /// <summary>The <c>^</c> assertion.</summary>
        Bol = 9,

        /// <summary>The <c>$</c> assertion.</summary>
        Eol = 10,

        /// <summary>The <c>\b</c> assertion, or <c>\B</c> when <c>A</c> is one.</summary>
        WordBoundary = 11,
    }

    /// <summary>One node of the parsed pattern, written once and read afterwards.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=5346EC
    // Broiler-Human:        PENDING
    private sealed class Node
    {
        /// <summary>Which kind of node this is.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=82DB1B
        // Broiler-Human:        PENDING
        internal NodeKind Kind;

        /// <summary>The character, the group number, the assertion kind, or a quantifier's minimum.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=0D182F
        // Broiler-Human:        PENDING
        internal int A;

        /// <summary>A quantifier's maximum.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=F34B25
        // Broiler-Human:        PENDING
        internal int B;

        /// <summary>Whether a quantifier prefers to repeat.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=B89527
        // Broiler-Human:        PENDING
        internal bool Greedy;

        /// <summary>The class a <see cref="NodeKind.Set"/> stands for.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=8F30DF
        // Broiler-Human:        PENDING
        internal JsRegExpCharSet? Set;

        /// <summary>The children, for the kinds that have any.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=FF7F53
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<Node>? Children;

        /// <summary>The lowest group number inside a quantified body.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=3A5155
        // Broiler-Human:        PENDING
        internal int FirstGroup;

        /// <summary>The highest group number inside a quantified body.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=5461DE
        // Broiler-Human:        PENDING
        internal int LastGroup;
    }

    /// <summary>The recursive-descent parser over the pattern grammar, Annex B included.</summary>
    /// <remarks>
    /// <para>
    /// <b>Capture numbering is by opening parenthesis, left to right, named and unnamed alike.</b>
    /// That is the whole of the numbering rule and it is why this parser counts the groups in a
    /// pre-pass before it parses anything: a back-reference is only a back-reference when its number
    /// is one the pattern has, and <c>\k&lt;name&gt;</c> may name a group that appears later. The
    /// pre-pass answers both questions before the first node is built.
    /// </para>
    /// <para>
    /// <b>Annex B is parsed, not tolerated.</b> A lone <c>{</c>, a lone <c>]</c>, <c>\8</c> with no
    /// eighth group, a legacy octal escape, <c>\c</c> before something that is not a control letter,
    /// a class range whose end is a class escape, and a quantified lookahead are all accepted
    /// outside <c>u</c> mode and all refused inside it, which is what the grammar says.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=E58418
    // Broiler-Human:        PENDING
    private sealed class Parser
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=3F0751
        // Broiler-Human:        PENDING
        private readonly string pattern;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=BFB9BF
        // Broiler-Human:        PENDING
        private readonly bool unicode;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=B2BC6A
        // Broiler-Human:        PENDING
        private readonly bool dotAll;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=19E77C
        // Broiler-Human:        PENDING
        private readonly int ceiling;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=F7A381
        // Broiler-Human:        PENDING
        private readonly string?[] names;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=98C321
        // Broiler-Human:        PENDING
        private int at;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=329437
        // Broiler-Human:        PENDING
        private int depth;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=F0630A
        // Broiler-Human:        PENDING
        private int opened;

        /// <summary>The code points <c>\w</c> stands for under this pattern's flags.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=0A44FB
        // Broiler-Human:        PENDING
        private readonly int[] wordRanges;

        /// <summary>Reads the pattern once to count its groups, then prepares to parse it.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=8BD675
        // Broiler-Human:        PENDING
        internal Parser(string source, bool inUnicodeMode, bool inDotAllMode, bool foldsCase)
        {
            pattern = source;
            unicode = inUnicodeMode;
            dotAll = inDotAllMode;
            ceiling = inUnicodeMode ? 0x10FFFF : 0xFFFF;
            wordRanges = WordRangesFor(foldsCase, inUnicodeMode);

            var found = new System.Collections.Generic.List<string?> { null };
            var inClass = false;

            for (var scan = 0; scan < source.Length; scan++)
            {
                var character = source[scan];

                if (character == '\\')
                {
                    scan++;
                    continue;
                }

                if (inClass)
                {
                    inClass = character != ']';
                    continue;
                }

                if (character == '[')
                {
                    inClass = true;
                    continue;
                }

                if (character != '(')
                {
                    continue;
                }

                if (scan + 1 >= source.Length || source[scan + 1] != '?')
                {
                    found.Add(null);
                    continue;
                }

                if (scan + 3 >= source.Length ||
                    source[scan + 2] != '<' ||
                    source[scan + 3] == '=' ||
                    source[scan + 3] == '!')
                {
                    continue;
                }

                var close = source.IndexOf('>', scan + 3);

                if (close < 0)
                {
                    throw new JsRegExpSyntaxError("Invalid capture group name");
                }

                var name = source.Substring(scan + 3, close - scan - 3);

                if (!IsGroupName(name))
                {
                    throw new JsRegExpSyntaxError("Invalid capture group name");
                }

                if (found.Contains(name))
                {
                    throw new JsRegExpSyntaxError("Duplicate capture group name");
                }

                found.Add(name);
                scan = close;
            }

            names = found.ToArray();
            CaptureCount = names.Length - 1;
        }

        /// <summary>How many capture groups the pattern declares.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=CE5256
        // Broiler-Human:        PENDING
        internal int CaptureCount { get; }

        /// <summary>
        /// Whether a group name is one the language would accept.
        /// </summary>
        /// <remarks>
        /// The language's <c>RegExpIdentifierName</c> admits a <c>\u</c> escape and every character
        /// the ID_Start and ID_Continue properties cover; this admits the unescaped ones only, so a
        /// name spelled with an escape - <c>(?&lt;a&gt;x)</c> - is refused here and accepted by
        /// the comparison engine. Every name anybody writes is in the accepted set.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=7B71E5
        // Broiler-Human:        PENDING
        private static bool IsGroupName(string name)
        {
            if (name.Length == 0)
            {
                return false;
            }

            for (var at = 0; at < name.Length; at++)
            {
                var character = name[at];

                if (character is '$' or '_' || char.IsLetter(character))
                {
                    continue;
                }

                if (at > 0 && char.IsDigit(character))
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        /// <summary>The group names, indexed by group number.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=34336A
        // Broiler-Human:        PENDING
        internal string?[] Names() => names;

        /// <summary>Parses the whole pattern.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=DD7D64
        // Broiler-Human:        PENDING
        internal Node Parse()
        {
            var root = ParseDisjunction();

            if (at != pattern.Length)
            {
                throw new JsRegExpSyntaxError("Unmatched ')'");
            }

            return root;
        }

        /// <summary>The alternatives of one disjunction.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=635756
        // Broiler-Human:        PENDING
        private Node ParseDisjunction()
        {
            if (++depth > MaximumNestingDepth)
            {
                throw new JsRegExpSyntaxError("Regular expression is nested too deeply");
            }

            var alternatives = new System.Collections.Generic.List<Node> { ParseAlternative() };

            while (at < pattern.Length && pattern[at] == '|')
            {
                at++;
                alternatives.Add(ParseAlternative());
            }

            depth--;

            return alternatives.Count == 1
                ? alternatives[0]
                : new Node { Kind = NodeKind.Alternation, Children = alternatives };
        }

        /// <summary>The terms of one alternative.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=3443D9
        // Broiler-Human:        PENDING
        private Node ParseAlternative()
        {
            var terms = new System.Collections.Generic.List<Node>();

            while (at < pattern.Length && pattern[at] != '|' && pattern[at] != ')')
            {
                terms.Add(ParseTerm());
            }

            if (terms.Count == 0)
            {
                return new Node { Kind = NodeKind.Empty };
            }

            return terms.Count == 1
                ? terms[0]
                : new Node { Kind = NodeKind.Sequence, Children = terms };
        }

        /// <summary>One atom and whatever quantifier follows it.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=29F88D
        // Broiler-Human:        PENDING
        private Node ParseTerm()
        {
            var character = pattern[at];

            if (character == '^')
            {
                at++;
                RefuseQuantifier();
                return new Node { Kind = NodeKind.Bol };
            }

            if (character == '$')
            {
                at++;
                RefuseQuantifier();
                return new Node { Kind = NodeKind.Eol };
            }

            if (character == '\\' && at + 1 < pattern.Length && (pattern[at + 1] is 'b' or 'B'))
            {
                var negated = pattern[at + 1] == 'B';
                at += 2;
                RefuseQuantifier();
                return new Node { Kind = NodeKind.WordBoundary, A = negated ? 1 : 0 };
            }

            var first = opened;
            var atom = ParseAtom();
            var last = opened;

            if (!TryParseQuantifier(out var min, out var max, out var greedy))
            {
                return atom;
            }

            // ONLY A LOOKAHEAD MAY BE QUANTIFIED, AND ONLY OUTSIDE `u` MODE. `^*`, `\b?` and a
            // quantified lookbehind are all refused, which is what the grammar says and what the
            // comparison engine does.
            if (atom.Kind == NodeKind.Look && (unicode || atom.A >= 2))
            {
                throw new JsRegExpSyntaxError("Nothing to repeat");
            }

            return new Node
            {
                Kind = NodeKind.Repeat,
                A = min,
                B = max,
                Greedy = greedy,
                Children = [atom],
                FirstGroup = first + 1,
                LastGroup = last,
            };
        }

        /// <summary>Refuses a quantifier on an assertion that may not carry one.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=2C0E16
        // Broiler-Human:        PENDING
        private void RefuseQuantifier()
        {
            var mark = at;

            if (TryParseQuantifier(out _, out _, out _))
            {
                at = mark;
                throw new JsRegExpSyntaxError("Nothing to repeat");
            }
        }

        /// <summary>One atom: a character, a class, a group, an assertion or an escape.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=734070
        // Broiler-Human:        PENDING
        private Node ParseAtom()
        {
            var character = pattern[at];

            switch (character)
            {
                case '.':
                    at++;
                    return new Node { Kind = NodeKind.Set, Set = BuildDot() };

                case '(':
                    return ParseGroup();

                case '[':
                    return ParseClass();

                case '\\':
                    return ParseAtomEscape();

                case '*':
                case '+':
                case '?':
                    throw new JsRegExpSyntaxError("Nothing to repeat");

                case '{':
                    // A BRACE THAT DOES NOT OPEN A QUANTIFIER IS AN ORDINARY CHARACTER outside `u`
                    // mode, which is the only reason `/{/` and `/a{b}/` are patterns at all.
                    if (LooksLikeQuantifier())
                    {
                        throw new JsRegExpSyntaxError("Nothing to repeat");
                    }

                    if (unicode)
                    {
                        throw new JsRegExpSyntaxError("Lone quantifier brackets");
                    }

                    at++;
                    return new Node { Kind = NodeKind.Char, A = '{' };

                case '}':
                case ']':
                    if (unicode)
                    {
                        throw new JsRegExpSyntaxError("Lone quantifier brackets");
                    }

                    at++;
                    return new Node { Kind = NodeKind.Char, A = character };

                default:
                    return new Node { Kind = NodeKind.Char, A = ReadPatternCodePoint() };
            }
        }

        /// <summary>Reads one code point of the pattern, pairing surrogates under <c>u</c>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=4FD296
        // Broiler-Human:        PENDING
        private int ReadPatternCodePoint()
        {
            var unit = pattern[at];

            if (unicode &&
                char.IsHighSurrogate(unit) &&
                at + 1 < pattern.Length &&
                char.IsLowSurrogate(pattern[at + 1]))
            {
                at += 2;
                return char.ConvertToUtf32(unit, pattern[at - 1]);
            }

            at++;
            return unit;
        }

        /// <summary>The set the full stop stands for.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=971179
        // Broiler-Human:        PENDING
        private JsRegExpCharSet BuildDot()
        {
            var set = new JsRegExpCharSet();

            if (dotAll)
            {
                set.Add(0, ceiling);
            }
            else
            {
                set.Negated = true;
                set.Add(0x0A, 0x0A);
                set.Add(0x0D, 0x0D);
                set.Add(0x2028, 0x2029);
            }

            set.Freeze();
            return set;
        }

        /// <summary>A group: capturing, named, non-capturing, or one of the four assertions.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=D1C8CC
        // Broiler-Human:        PENDING
        private Node ParseGroup()
        {
            at++;
            var kind = -1;
            var capture = false;

            if (at < pattern.Length && pattern[at] == '?')
            {
                if (at + 1 >= pattern.Length)
                {
                    throw new JsRegExpSyntaxError("Invalid group");
                }

                switch (pattern[at + 1])
                {
                    case ':':
                        at += 2;
                        break;

                    case '=':
                        at += 2;
                        kind = 0;
                        break;

                    case '!':
                        at += 2;
                        kind = 1;
                        break;

                    case '<':
                        if (at + 2 < pattern.Length && pattern[at + 2] == '=')
                        {
                            at += 3;
                            kind = 2;
                            break;
                        }

                        if (at + 2 < pattern.Length && pattern[at + 2] == '!')
                        {
                            at += 3;
                            kind = 3;
                            break;
                        }

                        at = pattern.IndexOf('>', at + 2) + 1;

                        if (at == 0)
                        {
                            throw new JsRegExpSyntaxError("Invalid capture group name");
                        }

                        capture = true;
                        break;

                    default:
                        throw new JsRegExpSyntaxError("Invalid group");
                }
            }
            else
            {
                capture = true;
            }

            var number = 0;

            if (capture)
            {
                number = ++opened;
            }

            var body = ParseDisjunction();

            if (at >= pattern.Length || pattern[at] != ')')
            {
                throw new JsRegExpSyntaxError("Unterminated group");
            }

            at++;

            if (kind >= 0)
            {
                return new Node { Kind = NodeKind.Look, A = kind, Children = [body] };
            }

            if (capture)
            {
                return new Node { Kind = NodeKind.Capture, A = number, Children = [body] };
            }

            // A NON-CAPTURING GROUP IS WRAPPED RATHER THAN COLLAPSED INTO ITS BODY. Handing the
            // body back is a group that has stopped existing, and the quantifier rule then reads
            // `(?:(?<=a))?` as a quantified lookbehind and refuses a pattern the language accepts.
            // The wrapper costs no instruction: a sequence of one lowers to its child.
            return new Node { Kind = NodeKind.Sequence, Children = [body] };
        }

        /// <summary>Whether a quantifier begins here, without consuming it.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=2D2174
        // Broiler-Human:        PENDING
        private bool LooksLikeQuantifier()
        {
            var mark = at;
            var found = TryParseQuantifier(out _, out _, out _);
            at = mark;
            return found;
        }

        /// <summary>Reads a quantifier when one is here, and reports whether it was.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=D9B5BF
        // Broiler-Human:        PENDING
        private bool TryParseQuantifier(out int min, out int max, out bool greedy)
        {
            min = 0;
            max = 0;
            greedy = true;

            if (at >= pattern.Length)
            {
                return false;
            }

            switch (pattern[at])
            {
                case '*':
                    at++;
                    min = 0;
                    max = int.MaxValue;
                    break;

                case '+':
                    at++;
                    min = 1;
                    max = int.MaxValue;
                    break;

                case '?':
                    at++;
                    min = 0;
                    max = 1;
                    break;

                case '{':
                    if (!TryParseBraces(out min, out max))
                    {
                        return false;
                    }

                    break;

                default:
                    return false;
            }

            if (at < pattern.Length && pattern[at] == '?')
            {
                at++;
                greedy = false;
            }

            return true;
        }

        /// <summary>Reads <c>{n}</c>, <c>{n,}</c> or <c>{n,m}</c>, leaving the cursor alone if it is not one.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=D152F5
        // Broiler-Human:        PENDING
        private bool TryParseBraces(out int min, out int max)
        {
            var mark = at;
            min = 0;
            max = 0;
            at++;

            if (!TryReadDecimal(out min))
            {
                at = mark;
                return false;
            }

            max = min;

            if (at < pattern.Length && pattern[at] == ',')
            {
                at++;

                if (at < pattern.Length && pattern[at] == '}')
                {
                    max = int.MaxValue;
                }
                else if (!TryReadDecimal(out max))
                {
                    at = mark;
                    return false;
                }
            }

            if (at >= pattern.Length || pattern[at] != '}')
            {
                at = mark;
                return false;
            }

            at++;

            if (max < min)
            {
                throw new JsRegExpSyntaxError("numbers out of order in {} quantifier");
            }

            return true;
        }

        /// <summary>Reads a run of decimal digits, saturating rather than overflowing.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=302AB1
        // Broiler-Human:        PENDING
        private bool TryReadDecimal(out int value)
        {
            value = 0;
            var digits = 0;

            while (at < pattern.Length && pattern[at] is >= '0' and <= '9')
            {
                value = value > 100000000 ? int.MaxValue : (value * 10) + (pattern[at] - '0');
                digits++;
                at++;
            }

            return digits > 0;
        }

        /// <summary>An escape in atom position: a class escape, a back-reference or a character.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=D0B8AB
        // Broiler-Human:        PENDING
        private Node ParseAtomEscape()
        {
            if (at + 1 >= pattern.Length)
            {
                throw new JsRegExpSyntaxError("\\ at end of pattern");
            }

            var marker = pattern[at + 1];

            if (marker is 'd' or 'D' or 's' or 'S' or 'w' or 'W')
            {
                at += 2;
                return new Node { Kind = NodeKind.Set, Set = BuildClassEscape(marker) };
            }

            if (marker is >= '1' and <= '9')
            {
                var mark = at;
                at++;
                TryReadDecimal(out var number);

                if (number <= CaptureCount)
                {
                    return new Node { Kind = NodeKind.BackRef, A = number };
                }

                // A NUMBER LARGER THAN THE PATTERN HAS GROUPS IS NOT A BACK-REFERENCE. Under `u` it
                // is a SyntaxError; outside it, Annex B reads it as a legacy octal escape or, for
                // `\8` and `\9`, as the digit itself.
                at = mark;

                if (unicode)
                {
                    throw new JsRegExpSyntaxError("Invalid escape");
                }
            }

            if (marker == 'k')
            {
                var named = ParseNamedBackReference();

                if (named is not null)
                {
                    return named;
                }
            }

            return new Node { Kind = NodeKind.Char, A = ReadCharacterEscape(false) };
        }

        /// <summary>Reads <c>\k&lt;name&gt;</c>, or answers that this <c>\k</c> is not one.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=BBC9DB
        // Broiler-Human:        PENDING
        private Node? ParseNamedBackReference()
        {
            var hasNames = false;

            foreach (var name in names)
            {
                hasNames |= name is not null;
            }

            if (!hasNames && !unicode)
            {
                // WITH NO NAMED GROUP ANYWHERE IN THE PATTERN, `\k` IS THE IDENTITY ESCAPE Annex B
                // makes it, which is why `/\k/` matches "k" and `/(?<a>x)\k/` does not compile.
                return null;
            }

            if (at + 2 >= pattern.Length || pattern[at + 2] != '<')
            {
                throw new JsRegExpSyntaxError("Invalid named reference");
            }

            var close = pattern.IndexOf('>', at + 3);

            if (close < 0)
            {
                throw new JsRegExpSyntaxError("Invalid named reference");
            }

            var wanted = pattern.Substring(at + 3, close - at - 3);
            at = close + 1;

            for (var group = 1; group < names.Length; group++)
            {
                if (string.Equals(names[group], wanted, System.StringComparison.Ordinal))
                {
                    return new Node { Kind = NodeKind.BackRef, A = group };
                }
            }

            throw new JsRegExpSyntaxError("Invalid named capture referenced");
        }

        /// <summary>The set one of the six class escapes stands for.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=941D65
        // Broiler-Human:        PENDING
        private JsRegExpCharSet BuildClassEscape(char marker)
        {
            var set = new JsRegExpCharSet();
            AddClassEscape(set, marker);
            set.Freeze();
            return set;
        }

        /// <summary>Adds one class escape's members to a set under construction.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=7B0622
        // Broiler-Human:        PENDING
        private void AddClassEscape(JsRegExpCharSet set, char marker)
        {
            switch (marker)
            {
                case 'd':
                    set.AddAll(DigitRanges);
                    break;

                case 'D':
                    set.AddComplement(DigitRanges, ceiling);
                    break;

                case 'w':
                    set.AddAll(wordRanges);
                    break;

                case 'W':
                    set.AddComplement(wordRanges, ceiling);
                    break;

                case 's':
                    set.AddAll(SpaceRanges);
                    break;

                default:
                    set.AddComplement(SpaceRanges, ceiling);
                    break;
            }
        }

        /// <summary>A character class in brackets.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=CD7613
        // Broiler-Human:        PENDING
        private Node ParseClass()
        {
            at++;
            var set = new JsRegExpCharSet();

            if (at < pattern.Length && pattern[at] == '^')
            {
                set.Negated = true;
                at++;
            }

            while (true)
            {
                if (at >= pattern.Length)
                {
                    throw new JsRegExpSyntaxError("Unterminated character class");
                }

                if (pattern[at] == ']')
                {
                    at++;
                    break;
                }

                var low = ReadClassAtom(set, out var lowIsSet);

                if (at + 1 < pattern.Length && pattern[at] == '-' && pattern[at + 1] != ']')
                {
                    at++;
                    var high = ReadClassAtom(set, out var highIsSet);

                    if (lowIsSet || highIsSet)
                    {
                        // ANNEX B LETS A CLASS ESCAPE STAND AT EITHER END OF A DASH, and reads the
                        // dash as an ordinary member rather than as a range. `u` mode refuses it.
                        if (unicode)
                        {
                            throw new JsRegExpSyntaxError("Invalid character class");
                        }

                        set.Add('-', '-');

                        if (!lowIsSet)
                        {
                            set.Add(low, low);
                        }

                        if (!highIsSet)
                        {
                            set.Add(high, high);
                        }

                        continue;
                    }

                    if (high < low)
                    {
                        throw new JsRegExpSyntaxError("Range out of order in character class");
                    }

                    set.Add(low, high);
                    continue;
                }

                if (!lowIsSet)
                {
                    set.Add(low, low);
                }
            }

            set.Freeze();
            return new Node { Kind = NodeKind.Set, Set = set };
        }

        /// <summary>
        /// One member of a class: a code point, or a class escape added to <paramref name="set"/>
        /// directly.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=EF7074
        // Broiler-Human:        PENDING
        private int ReadClassAtom(JsRegExpCharSet set, out bool wasSet)
        {
            wasSet = false;

            if (pattern[at] != '\\')
            {
                return ReadPatternCodePoint();
            }

            if (at + 1 >= pattern.Length)
            {
                throw new JsRegExpSyntaxError("\\ at end of pattern");
            }

            var marker = pattern[at + 1];

            if (marker is 'd' or 'D' or 's' or 'S' or 'w' or 'W')
            {
                at += 2;
                wasSet = true;
                AddClassEscape(set, marker);
                return 0;
            }

            if (marker == 'b')
            {
                at += 2;
                return 0x08;
            }

            if (marker == '-')
            {
                at += 2;
                return '-';
            }

            return ReadCharacterEscape(true);
        }

        /// <summary>
        /// The escape sequences that stand for one character, in either position.
        /// </summary>
        /// <remarks>
        /// The cursor is on the backslash on entry and past the whole escape on exit. Everything
        /// Annex B relaxes is relaxed here and nowhere else: an unknown escape is the character
        /// itself outside <c>u</c> mode and a refusal inside it.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=769BD4
        // Broiler-Human:        PENDING
        private int ReadCharacterEscape(bool inClass)
        {
            var marker = pattern[at + 1];
            at += 2;

            switch (marker)
            {
                case 'n':
                    return 0x0A;

                case 'r':
                    return 0x0D;

                case 't':
                    return 0x09;

                case 'v':
                    return 0x0B;

                case 'f':
                    return 0x0C;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                    at -= 1;
                    return ReadOctalOrNull(marker);

                case '8':
                case '9':
                    if (unicode)
                    {
                        throw new JsRegExpSyntaxError("Invalid escape");
                    }

                    return marker;

                case 'x':
                    return ReadFixedHex(2, 'x');

                case 'u':
                    return ReadUnicodeEscape();

                case 'c':
                    return ReadControlEscape(inClass);

                case 'p':
                case 'P':
                    // PROPERTY ESCAPES ARE NOT IMPLEMENTED. Under `u` the language says this is a
                    // pattern with a property escape in it, and refusing it is the honest answer;
                    // outside `u` the grammar already says it is the letter itself.
                    if (unicode)
                    {
                        throw new JsRegExpSyntaxError(
                            "Unicode property escapes are not supported by this matcher");
                    }

                    return marker;

                default:
                    if (unicode && !IsUnicodeIdentityEscape(marker, inClass))
                    {
                        throw new JsRegExpSyntaxError("Invalid escape");
                    }

                    at--;
                    return ReadPatternCodePoint();
            }
        }

        /// <summary>Which characters <c>u</c> mode still allows an identity escape for.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=E8A830
        // Broiler-Human:        PENDING
        private static bool IsUnicodeIdentityEscape(char marker, bool inClass) =>
            marker switch
            {
                '^' or '$' or '\\' or '.' or '*' or '+' or '?' or '(' or ')' or
                '[' or ']' or '{' or '}' or '|' or '/' => true,
                '-' => inClass,
                _ => false,
            };

        /// <summary>The NUL escape, or the legacy octal escape Annex B keeps.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=112A98
        // Broiler-Human:        PENDING
        private int ReadOctalOrNull(char first)
        {
            var following = at + 1 < pattern.Length ? pattern[at + 1] : '\0';

            if (first == '0' && (following is < '0' or > '9'))
            {
                at++;
                return 0;
            }

            if (unicode)
            {
                throw new JsRegExpSyntaxError("Invalid escape");
            }

            var value = 0;
            var digits = 0;

            while (digits < 3 &&
                at < pattern.Length &&
                pattern[at] is >= '0' and <= '7' &&
                ((value * 8) + (pattern[at] - '0')) <= 255)
            {
                value = (value * 8) + (pattern[at] - '0');
                at++;
                digits++;
            }

            return value;
        }

        /// <summary>A fixed-width hexadecimal escape, or the letter itself when it is malformed.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=967EF4
        // Broiler-Human:        PENDING
        private int ReadFixedHex(int width, char marker)
        {
            if (at + width <= pattern.Length && AllHex(at, width))
            {
                var value = 0;

                for (var step = 0; step < width; step++)
                {
                    value = (value * 16) + HexValue(pattern[at + step]);
                }

                at += width;
                return value;
            }

            if (unicode)
            {
                throw new JsRegExpSyntaxError("Invalid escape");
            }

            return marker;
        }

        /// <summary>The <c>\u</c> escape, in both its fixed and its braced form.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=13A79C
        // Broiler-Human:        PENDING
        private int ReadUnicodeEscape()
        {
            if (unicode && at < pattern.Length && pattern[at] == '{')
            {
                var close = pattern.IndexOf('}', at);

                if (close < 0 || close == at + 1)
                {
                    throw new JsRegExpSyntaxError("Invalid Unicode escape");
                }

                var value = 0;

                for (var step = at + 1; step < close; step++)
                {
                    if (!IsHex(pattern[step]))
                    {
                        throw new JsRegExpSyntaxError("Invalid Unicode escape");
                    }

                    value = (value * 16) + HexValue(pattern[step]);

                    if (value > 0x10FFFF)
                    {
                        throw new JsRegExpSyntaxError("Invalid Unicode escape");
                    }
                }

                at = close + 1;
                return value;
            }

            var first = ReadFixedHex(4, 'u');

            // A SURROGATE PAIR SPELLED AS TWO ESCAPES IS ONE CODE POINT UNDER `u`, which is what
            // makes `/😀/u` one atom rather than two unmatched halves.
            if (unicode &&
                first is >= 0xD800 and <= 0xDBFF &&
                at + 1 < pattern.Length &&
                pattern[at] == '\\' &&
                pattern[at + 1] == 'u')
            {
                var mark = at;
                at += 2;
                var second = ReadFixedHex(4, 'u');

                if (second is >= 0xDC00 and <= 0xDFFF)
                {
                    return char.ConvertToUtf32((char)first, (char)second);
                }

                at = mark;
            }

            return first;
        }

        /// <summary>The <c>\c</c> control escape, with Annex B's two relaxations.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=FD6110
        // Broiler-Human:        PENDING
        private int ReadControlEscape(bool inClass)
        {
            if (at < pattern.Length && char.IsAsciiLetter(pattern[at]))
            {
                var letter = pattern[at];
                at++;
                return letter % 32;
            }

            if (inClass && at < pattern.Length && (char.IsAsciiDigit(pattern[at]) || pattern[at] == '_'))
            {
                var extra = pattern[at];
                at++;
                return extra % 32;
            }

            if (unicode)
            {
                throw new JsRegExpSyntaxError("Invalid escape");
            }

            // `\c` BEFORE ANYTHING ELSE IS A BACKSLASH AND THEN A `c`, so `/\c1/` matches the three
            // characters it looks like. Stepping back onto the `c` is how the caller sees that.
            at--;
            return '\\';
        }

        /// <summary>Whether the next <paramref name="width"/> characters are hexadecimal digits.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=2C07F1
        // Broiler-Human:        PENDING
        private bool AllHex(int from, int width)
        {
            for (var step = 0; step < width; step++)
            {
                if (!IsHex(pattern[from + step]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Whether one character is a hexadecimal digit.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=D1E042
        // Broiler-Human:        PENDING
        private static bool IsHex(char character) => char.IsAsciiHexDigit(character);

        /// <summary>The value of one hexadecimal digit.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=794238
        // Broiler-Human:        PENDING
        private static int HexValue(char character) =>
            character <= '9' ? character - '0' : (char.ToLowerInvariant(character) - 'a') + 10;
    }

    /// <summary>The lowering: one node tree in, one instruction array out.</summary>
    /// <remarks>
    /// A quantifier becomes a loop over two counter cells rather than a repeated copy of its body,
    /// because <c>x{1000000}</c> is a pattern the language admits and unrolling it is a megabyte of
    /// instructions. The two exceptions are the shapes where a counter would be dead weight -
    /// <c>?</c> and <c>*</c>, whose bounds are decided by the split alone - and both still carry the
    /// empty-iteration check, because <c>/(a?)?/</c> answering <c>undefined</c> for its group
    /// depends on it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=3F93EC
    // Broiler-Human:        PENDING
    private sealed class Emitter
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=22974A
        // Broiler-Human:        PENDING
        private readonly System.Collections.Generic.List<Instruction> code = [];

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=868766
        // Broiler-Human:        PENDING
        private readonly System.Collections.Generic.List<JsRegExpCharSet> sets = [];

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=853984
        // Broiler-Human:        PENDING
        private readonly bool ignoreCase;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=BFB9BF
        // Broiler-Human:        PENDING
        private readonly bool unicode;

        /// <summary>Creates a lowering for one set of flags.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=F6AF87
        // Broiler-Human:        PENDING
        internal Emitter(bool foldsCase, bool inUnicodeMode)
        {
            ignoreCase = foldsCase;
            unicode = inUnicodeMode;

            // Cells zero and one are the whole match's own bounds, which is why a capture group's
            // cells start at two and the counters start after every capture.
            CellCount = 2;
        }

        /// <summary>How many cells the machine has to allocate for one attempt.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=43FCA6
        // Broiler-Human:        PENDING
        internal int CellCount { get; private set; }

        /// <summary>Lowers a whole pattern, ending it with an acceptance.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=08755F
        // Broiler-Human:        PENDING
        internal void Lower(Node root)
        {
            Reserve(root);
            Emit(root, false);
            Add(Op.Save, 1, 0, 0, false);
            Add(Op.Accept, 0, 0, 0, false);
        }

        /// <summary>The finished instruction array.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=C31F82
        // Broiler-Human:        PENDING
        internal Instruction[] Program() => code.ToArray();

        /// <summary>The character classes the program refers to by number.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=857497
        // Broiler-Human:        PENDING
        internal JsRegExpCharSet[] Sets() => sets.ToArray();

        /// <summary>Makes room for every capture group's pair of cells before anything is emitted.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=D47A6A
        // Broiler-Human:        PENDING
        private void Reserve(Node node)
        {
            var pending = new System.Collections.Generic.Stack<Node>();
            pending.Push(node);

            while (pending.Count > 0)
            {
                var current = pending.Pop();

                if (current.Kind == NodeKind.Capture)
                {
                    CellCount = System.Math.Max(CellCount, (current.A * 2) + 2);
                }

                if (current.Children is null)
                {
                    continue;
                }

                foreach (var child in current.Children)
                {
                    pending.Push(child);
                }
            }
        }

        /// <summary>Appends one instruction and answers where it landed.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=2769CA
        // Broiler-Human:        PENDING
        private int Add(Op op, int a, int b, int c, bool backward)
        {
            code.Add(new Instruction { Op = op, A = a, B = b, C = c, Backward = backward });
            return code.Count - 1;
        }

        /// <summary>Rewrites the second operand of an already-emitted instruction.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=A01A8C
        // Broiler-Human:        PENDING
        private void PatchB(int position, int value)
        {
            var instruction = code[position];
            instruction.B = value;
            code[position] = instruction;
        }

        /// <summary>Rewrites the first operand of an already-emitted instruction.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=DF82FD
        // Broiler-Human:        PENDING
        private void PatchA(int position, int value)
        {
            var instruction = code[position];
            instruction.A = value;
            code[position] = instruction;
        }

        /// <summary>Rewrites the third operand of an already-emitted instruction.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=CBEBA6
        // Broiler-Human:        PENDING
        private void PatchC(int position, int value)
        {
            var instruction = code[position];
            instruction.C = value;
            code[position] = instruction;
        }

        /// <summary>Lowers one node, in the direction its enclosing assertion set.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=B3E4D5
        // Broiler-Human:        PENDING
        private void Emit(Node node, bool backward)
        {
            switch (node.Kind)
            {
                case NodeKind.Empty:
                    break;

                case NodeKind.Char:
                    Add(
                        Op.Char,
                        ignoreCase ? JsRegExpCase.Canonicalize(node.A, unicode) : node.A,
                        0,
                        0,
                        backward);
                    break;

                case NodeKind.Set:
                    sets.Add(node.Set!);
                    Add(Op.Set, sets.Count - 1, 0, 0, backward);
                    break;

                case NodeKind.Bol:
                    Add(Op.Bol, 0, 0, 0, backward);
                    break;

                case NodeKind.Eol:
                    Add(Op.Eol, 0, 0, 0, backward);
                    break;

                case NodeKind.WordBoundary:
                    Add(Op.Word, node.A, 0, 0, backward);
                    break;

                case NodeKind.BackRef:
                    Add(Op.BackRef, node.A, 0, 0, backward);
                    break;

                case NodeKind.Sequence:
                    EmitSequence(node, backward);
                    break;

                case NodeKind.Alternation:
                    EmitAlternation(node, backward);
                    break;

                case NodeKind.Capture:
                    EmitCapture(node, backward);
                    break;

                case NodeKind.Look:
                    EmitLook(node, backward);
                    break;

                default:
                    EmitRepeat(node, backward);
                    break;
            }
        }

        /// <summary>Lowers a concatenation, right to left when a lookbehind is running.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=6F095F
        // Broiler-Human:        PENDING
        private void EmitSequence(Node node, bool backward)
        {
            var children = node.Children!;

            if (backward)
            {
                for (var at = children.Count - 1; at >= 0; at--)
                {
                    Emit(children[at], true);
                }

                return;
            }

            foreach (var child in children)
            {
                Emit(child, false);
            }
        }

        /// <summary>Lowers an alternation as a chain of splits.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=06E1D6
        // Broiler-Human:        PENDING
        private void EmitAlternation(Node node, bool backward)
        {
            var children = node.Children!;
            var exits = new System.Collections.Generic.List<int>();

            for (var at = 0; at < children.Count; at++)
            {
                if (at == children.Count - 1)
                {
                    Emit(children[at], backward);
                    break;
                }

                var split = Add(Op.Split, 0, 0, 0, backward);
                PatchA(split, split + 1);
                Emit(children[at], backward);
                exits.Add(Add(Op.Jump, 0, 0, 0, backward));
                PatchB(split, code.Count);
            }

            foreach (var exit in exits)
            {
                PatchA(exit, code.Count);
            }
        }

        /// <summary>Lowers a capture group, whose ends swap when a lookbehind is running.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=E1CE97
        // Broiler-Human:        PENDING
        private void EmitCapture(Node node, bool backward)
        {
            var start = node.A * 2;
            Add(Op.Save, backward ? start + 1 : start, 0, 0, backward);
            Emit(node.Children![0], backward);
            Add(Op.Save, backward ? start : start + 1, 0, 0, backward);
        }

        /// <summary>Lowers one of the four assertions.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=18CE35
        // Broiler-Human:        PENDING
        private void EmitLook(Node node, bool backward)
        {
            var begin = Add(Op.AssertBegin, node.A, 0, 0, backward);
            Emit(node.Children![0], node.A >= 2);
            Add(Op.AssertEnd, 0, 0, 0, backward);
            PatchB(begin, code.Count);
        }

        /// <summary>Lowers a quantifier.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=1306FA
        // Broiler-Human:        PENDING
        private void EmitRepeat(Node node, bool backward)
        {
            var min = node.A;
            var max = node.B;
            var body = node.Children![0];

            if (max == 0)
            {
                return;
            }

            var progress = CellCount++;

            if (min == 0 && max == 1)
            {
                var once = Add(Op.Split, 0, 0, 0, backward);
                PatchA(once, node.Greedy ? once + 1 : 0);
                PatchB(once, node.Greedy ? 0 : once + 1);
                EmitIteration(node, body, progress, -1, 0, backward);

                if (node.Greedy)
                {
                    PatchB(once, code.Count);
                }
                else
                {
                    PatchA(once, code.Count);
                }

                return;
            }

            if (min == 0 && max == int.MaxValue)
            {
                var head = code.Count;
                var split = Add(Op.Split, 0, 0, 0, backward);
                PatchA(split, node.Greedy ? split + 1 : 0);
                PatchB(split, node.Greedy ? 0 : split + 1);
                EmitIteration(node, body, progress, -1, 0, backward);
                Add(Op.Jump, head, 0, 0, backward);

                if (node.Greedy)
                {
                    PatchB(split, code.Count);
                }
                else
                {
                    PatchA(split, code.Count);
                }

                return;
            }

            var counter = CellCount++;
            Add(Op.SetCell, counter, 0, 0, backward);
            var loop = code.Count;
            var ceiling = max == int.MaxValue ? -1 : Add(Op.JumpIfAtLeast, counter, max, 0, backward);
            var floor = min == 0 ? -1 : Add(Op.JumpIfBelow, counter, min, 0, backward);
            var choice = Add(Op.Split, 0, 0, 0, backward);
            PatchA(choice, node.Greedy ? choice + 1 : 0);
            PatchB(choice, node.Greedy ? 0 : choice + 1);

            if (floor >= 0)
            {
                PatchC(floor, code.Count);
            }

            EmitIteration(node, body, progress, counter, min, backward);
            Add(Op.Jump, loop, 0, 0, backward);

            if (node.Greedy)
            {
                PatchB(choice, code.Count);
            }
            else
            {
                PatchA(choice, code.Count);
            }

            if (ceiling >= 0)
            {
                PatchC(ceiling, code.Count);
            }
        }

        /// <summary>
        /// One iteration of a quantifier: the reset, the body, and the empty-iteration check.
        /// </summary>
        /// <remarks>
        /// The reset is INSIDE the choice on purpose. A group cleared by an iteration that then
        /// fails has to be un-cleared, because the specification's continuation runs against the
        /// state from before the reset - which is what makes <c>/(a*)*/</c> answer <c>"aaa"</c> for
        /// its group while <c>/(?:(a)|b)+/</c> answers <c>undefined</c> for its own. Both fall out
        /// of putting the reset where the undo trail can reach it.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=D6FFF4
        // Broiler-Human:        PENDING
        private void EmitIteration(
            Node node, Node body, int progress, int counter, int min, bool backward)
        {
            Add(Op.MarkPos, progress, 0, 0, backward);

            if (node.LastGroup >= node.FirstGroup)
            {
                Add(Op.Clear, node.FirstGroup * 2, (node.LastGroup * 2) + 1, 0, backward);
            }

            Emit(body, backward);

            if (counter < 0)
            {
                Add(Op.EmptyCheck, progress, -1, 0, backward);
                return;
            }

            // THE COUNT RISES BEFORE THE CHECK READS IT, because the check asks whether the
            // iteration that just finished was a required one, and that is a question about the
            // count after it rather than before.
            Add(Op.IncCell, counter, 0, 0, backward);
            Add(Op.EmptyCheck, progress, counter, min, backward);
        }
    }

    /// <summary>One backtrack point, or one open assertion.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=6F6BFA
    // Broiler-Human:        PENDING
    private struct Frame
    {
        /// <summary>Where to continue: the alternative, or an assertion's continuation.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=4E6E02
        // Broiler-Human:        PENDING
        internal int Pc;

        /// <summary>The position to restore.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=1FED1E
        // Broiler-Human:        PENDING
        internal int Sp;

        /// <summary>How much of the undo trail belongs to what came before.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=C3FBFC
        // Broiler-Human:        PENDING
        internal int Trail;

        /// <summary>Which assertion this frame opened, or <c>-1</c> for an ordinary alternative.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=7382C9
        // Broiler-Human:        PENDING
        internal int AssertKind;
    }

    /// <summary>The backtracking machine: an explicit stack and nothing on the CLR's.</summary>
    /// <remarks>
    /// One instance runs one <c>Match</c> call, attempt after attempt, and is not shared: guest code
    /// can re-enter a regular expression from a replacement function, and a machine holding its
    /// arrays on the object would answer that call with the outer call's state.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=F0400B
    // Broiler-Human:        PENDING
    private sealed class Runner
    {
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=9A45C4
        // Broiler-Human:        PENDING
        private readonly JsRegExpMatcher owner;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=A74471
        // Broiler-Human:        PENDING
        private readonly string input;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=8DAF23
        // Broiler-Human:        PENDING
        private readonly JsRegExpCharge? charge;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=8E97FF
        // Broiler-Human:        PENDING
        private readonly int[] cells;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=F031E0
        // Broiler-Human:        PENDING
        private int[] trailCell = new int[64];

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=B68D11
        // Broiler-Human:        PENDING
        private int[] trailValue = new int[64];

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=D5269D
        // Broiler-Human:        PENDING
        private int trailTop;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=3CEAE1
        // Broiler-Human:        PENDING
        private Frame[] frames = new Frame[64];

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=F7B0EC
        // Broiler-Human:        PENDING
        private int frameTop;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=803E7B
        // Broiler-Human:        PENDING
        private int[] assertions = new int[16];

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=A271D9
        // Broiler-Human:        PENDING
        private int assertionTop;

        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=CF09D0
        // Broiler-Human:        PENDING
        private ulong steps;

        /// <summary>Creates a machine over one input.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=4B77BD
        // Broiler-Human:        PENDING
        internal Runner(JsRegExpMatcher matcher, string text, JsRegExpCharge? meter)
        {
            owner = matcher;
            input = text;
            charge = meter;
            cells = new int[matcher.cellCount];
        }

        /// <summary>Counts work the scan did outside the machine, so the skip is not free.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=8A8459
        // Broiler-Human:        PENDING
        internal void Spend(ulong units)
        {
            steps += units;

            if (steps >= StepsPerCharge)
            {
                charge?.Invoke(steps);
                steps = 0;
            }
        }

        /// <summary>Hands the meter whatever has not been charged yet.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=F27C1E
        // Broiler-Human:        PENDING
        internal void Settle()
        {
            if (steps > 0)
            {
                charge?.Invoke(steps);
                steps = 0;
            }
        }

        /// <summary>The capture slots of the attempt that just succeeded.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=C77723
        // Broiler-Human:        PENDING
        internal int[] Captured(int captureCount)
        {
            var slots = new int[(captureCount + 1) * 2];
            System.Array.Copy(cells, slots, slots.Length);
            return slots;
        }

        /// <summary>Runs the program once, from exactly <paramref name="start"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=3AE975
        // Broiler-Human:        PENDING
        internal bool Attempt(int start)
        {
            trailTop = 0;
            frameTop = 0;
            assertionTop = 0;

            for (var at = 0; at < cells.Length; at++)
            {
                cells[at] = -1;
            }

            cells[0] = start;

            var code = owner.code;
            var position = start;
            var pc = 0;

            while (true)
            {
                if (++steps >= StepsPerCharge)
                {
                    charge?.Invoke(steps);
                    steps = 0;
                }

                var instruction = code[pc];
                var failed = false;

                switch (instruction.Op)
                {
                    case Op.Char:
                        failed = !TakeChar(ref position, instruction.A, instruction.Backward);
                        pc++;
                        break;

                    case Op.Set:
                        failed = !TakeSet(ref position, instruction.A, instruction.Backward);
                        pc++;
                        break;

                    case Op.Split:
                        PushFrame(instruction.B, position, -1);
                        pc = instruction.A;
                        break;

                    case Op.Jump:
                        pc = instruction.A;
                        break;

                    case Op.Save:
                        Write(instruction.A, position);
                        pc++;
                        break;

                    case Op.Clear:
                        for (var slot = instruction.A; slot <= instruction.B; slot++)
                        {
                            Write(slot, -1);
                        }

                        pc++;
                        break;

                    case Op.Bol:
                        failed = position != 0 &&
                            !(owner.Multiline && IsLineTerminator(input[position - 1]));
                        pc++;
                        break;

                    case Op.Eol:
                        failed = position != input.Length &&
                            !(owner.Multiline && IsLineTerminator(input[position]));
                        pc++;
                        break;

                    case Op.Word:
                        failed = AtWordBoundary(position) == (instruction.A == 1);
                        pc++;
                        break;

                    case Op.BackRef:
                        failed = !TakeBackReference(ref position, instruction.A, instruction.Backward);
                        pc++;
                        break;

                    case Op.AssertBegin:
                        PushFrame(instruction.B, position, instruction.A);
                        PushAssertion(frameTop - 1);
                        pc++;
                        break;

                    case Op.AssertEnd:
                        failed = !CloseAssertion(ref position, ref pc);
                        break;

                    case Op.SetCell:
                        Write(instruction.A, instruction.B);
                        pc++;
                        break;

                    case Op.MarkPos:
                        Write(instruction.A, position);
                        pc++;
                        break;

                    case Op.IncCell:
                        Write(instruction.A, cells[instruction.A] + 1);
                        pc++;
                        break;

                    case Op.JumpIfAtLeast:
                        pc = cells[instruction.A] >= instruction.B ? instruction.C : pc + 1;
                        break;

                    case Op.JumpIfBelow:
                        pc = cells[instruction.A] < instruction.B ? instruction.C : pc + 1;
                        break;

                    case Op.EmptyCheck:
                        failed = position == cells[instruction.A] &&
                            (instruction.B < 0 || cells[instruction.B] > instruction.C);
                        pc++;
                        break;

                    default:
                        Write(1, position);
                        return true;
                }

                if (failed && !Backtrack(ref position, ref pc))
                {
                    return false;
                }
            }
        }

        /// <summary>Whether one code unit ends a line.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=99F8A4
        // Broiler-Human:        PENDING
        private static bool IsLineTerminator(char unit) =>
            unit is '\n' or '\r' or (char)0x2028 or (char)0x2029;

        /// <summary>Pops the newest backtrack point, running the assertions it closes.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=9C52C3
        // Broiler-Human:        PENDING
        private bool Backtrack(ref int position, ref int pc)
        {
            while (frameTop > 0)
            {
                var frame = frames[--frameTop];
                Unwind(frame.Trail);

                if (frame.AssertKind < 0)
                {
                    position = frame.Sp;
                    pc = frame.Pc;
                    return true;
                }

                assertionTop--;

                // A NEGATIVE ASSERTION WHOSE BODY RAN OUT OF WAYS TO MATCH HAS SUCCEEDED. That is
                // the whole of what makes `(?!x)` an assertion rather than a match: its failure is
                // the answer, and it is delivered here, where the body's last alternative is gone.
                if (frame.AssertKind is 1 or 3)
                {
                    position = frame.Sp;
                    pc = frame.Pc;
                    return true;
                }
            }

            return false;
        }

        /// <summary>Closes the innermost assertion, its body having matched.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=EC8CC5
        // Broiler-Human:        PENDING
        private bool CloseAssertion(ref int position, ref int pc)
        {
            var index = assertions[--assertionTop];
            var frame = frames[index];

            // The assertion is ATOMIC: everything the body pushed goes, so nothing later can
            // backtrack into a lookahead that already answered.
            frameTop = index;

            if (frame.AssertKind is 0 or 2)
            {
                // A positive assertion keeps whatever its body captured, which is why
                // `/(?=(a))a/.exec("a")` reports "a" for its group.
                position = frame.Sp;
                pc = frame.Pc;
                return true;
            }

            Unwind(frame.Trail);
            position = frame.Sp;
            return false;
        }

        /// <summary>Records a backtrack point.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=1AACC2
        // Broiler-Human:        PENDING
        private void PushFrame(int pc, int position, int assertKind)
        {
            if (frameTop == frames.Length)
            {
                if (frames.Length >= MaximumFrames)
                {
                    throw new JsRegExpOverflowError(
                        "the regular expression exceeded its backtracking allowance");
                }

                System.Array.Resize(ref frames, frames.Length * 2);
            }

            frames[frameTop++] = new Frame
            {
                Pc = pc,
                Sp = position,
                Trail = trailTop,
                AssertKind = assertKind,
            };
        }

        /// <summary>Records which frame the innermost open assertion is.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=9C42ED
        // Broiler-Human:        PENDING
        private void PushAssertion(int frame)
        {
            if (assertionTop == assertions.Length)
            {
                System.Array.Resize(ref assertions, assertions.Length * 2);
            }

            assertions[assertionTop++] = frame;
        }

        /// <summary>Writes one cell, remembering what it held.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=974FF3
        // Broiler-Human:        PENDING
        private void Write(int cell, int value)
        {
            if (trailTop == trailCell.Length)
            {
                if (trailCell.Length >= MaximumTrail)
                {
                    throw new JsRegExpOverflowError(
                        "the regular expression exceeded its backtracking allowance");
                }

                System.Array.Resize(ref trailCell, trailCell.Length * 2);
                System.Array.Resize(ref trailValue, trailValue.Length * 2);
            }

            trailCell[trailTop] = cell;
            trailValue[trailTop] = cells[cell];
            trailTop++;
            cells[cell] = value;
        }

        /// <summary>Undoes every cell write back to <paramref name="mark"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=967626
        // Broiler-Human:        PENDING
        private void Unwind(int mark)
        {
            while (trailTop > mark)
            {
                trailTop--;
                cells[trailCell[trailTop]] = trailValue[trailTop];
            }
        }

        /// <summary>Reads the code point at a position, or the one before it.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=7FCCD7
        // Broiler-Human:        PENDING
        private int Read(int position, bool backward, out int width)
        {
            if (!backward)
            {
                var unit = input[position];

                if (owner.Unicode &&
                    char.IsHighSurrogate(unit) &&
                    position + 1 < input.Length &&
                    char.IsLowSurrogate(input[position + 1]))
                {
                    width = 2;
                    return char.ConvertToUtf32(unit, input[position + 1]);
                }

                width = 1;
                return unit;
            }

            var last = input[position - 1];

            if (owner.Unicode &&
                char.IsLowSurrogate(last) &&
                position - 2 >= 0 &&
                char.IsHighSurrogate(input[position - 2]))
            {
                width = 2;
                return char.ConvertToUtf32(input[position - 2], last);
            }

            width = 1;
            return last;
        }

        /// <summary>Consumes one literal character.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=85595E
        // Broiler-Human:        PENDING
        private bool TakeChar(ref int position, int wanted, bool backward)
        {
            if (backward ? position <= 0 : position >= input.Length)
            {
                return false;
            }

            var found = Read(position, backward, out var width);

            if (owner.IgnoreCase)
            {
                found = JsRegExpCase.Canonicalize(found, owner.Unicode);
            }

            if (found != wanted)
            {
                return false;
            }

            position = backward ? position - width : position + width;
            return true;
        }

        /// <summary>Consumes one character a class admits.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=3B109F
        // Broiler-Human:        PENDING
        private bool TakeSet(ref int position, int set, bool backward)
        {
            if (backward ? position <= 0 : position >= input.Length)
            {
                return false;
            }

            var found = Read(position, backward, out var width);

            if (!owner.classes[set].Matches(found, owner.IgnoreCase, owner.Unicode))
            {
                return false;
            }

            position = backward ? position - width : position + width;
            return true;
        }

        /// <summary>Whether the two characters around a position differ in wordness.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=88E183
        // Broiler-Human:        PENDING
        private bool AtWordBoundary(int position)
        {
            var before = position > 0 && IsWordCharacter(input[position - 1]);
            var after = position < input.Length && IsWordCharacter(input[position]);
            return before != after;
        }

        /// <summary>Whether one code unit is a word character, folding included.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=6; Fingerprint=C60933
        // Broiler-Human:        PENDING
        private bool IsWordCharacter(char unit) =>
            owner.wordCharacters.Matches(unit, false, false);

        /// <summary>Consumes whatever a capture group matched.</summary>
        // Broiler-AI:           Origin=AI; IP=Medium; Security=Medium; Resources=6; Fingerprint=E44A8F
        // Broiler-Human:        PENDING
        private bool TakeBackReference(ref int position, int group, bool backward)
        {
            var from = cells[group * 2];
            var to = cells[(group * 2) + 1];

            // A GROUP THAT DID NOT PARTICIPATE MATCHES THE EMPTY STRING AND DOES NOT FAIL, which is
            // what makes `/(a)?\1b/.test("b")` true.
            if (from < 0 || to < 0 || to <= from)
            {
                return true;
            }

            var length = to - from;
            var start = backward ? position - length : position;

            if (start < 0 || start + length > input.Length)
            {
                return false;
            }

            steps += (ulong)length;

            if (!owner.IgnoreCase)
            {
                if (string.CompareOrdinal(input, from, input, start, length) != 0)
                {
                    return false;
                }
            }
            else
            {
                var step = 0;

                while (step < length)
                {
                    var left = Read(from + step, false, out var leftWidth);
                    var right = Read(start + step, false, out var rightWidth);

                    if (leftWidth != rightWidth ||
                        JsRegExpCase.Canonicalize(left, owner.Unicode) !=
                        JsRegExpCase.Canonicalize(right, owner.Unicode))
                    {
                        return false;
                    }

                    step += leftWidth;
                }
            }

            position = backward ? position - length : position + length;
            return true;
        }
    }
}
