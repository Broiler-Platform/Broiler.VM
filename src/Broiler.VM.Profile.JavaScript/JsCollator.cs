// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           12
// Human-reviewed:   0/9
// IP risk:          Low
// Security risk:    Medium
// Criteria:         1/0
// Resource impact:  4/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>How a collator orders case at the tertiary level and on its case level.</summary>
// Broiler-AI:           Origin=AI; Spec=ECMA-402 s10; IP=Low; Security=Low; Resources=0; Fingerprint=695E11
// Broiler-Human:        PENDING
internal enum JsCaseFirst
{
    /// <summary>The root's order: lower case first, by tertiary weight.</summary>
    Off,

    /// <summary>Upper case first.</summary>
    Upper,

    /// <summary>Lower case first, stated.</summary>
    Lower,
}

/// <summary>
/// One collator's resolved settings and its comparison: the strings' collation elements compared
/// level by level, as UTS #10 orders them and ECMA-402's options choose (JSD-0043).
/// </summary>
/// <remarks>
/// <para>
/// <b>Sensitivity is a strength and a case level</b>: <c>base</c> compares primaries only,
/// <c>accent</c> adds secondaries, <c>case</c> compares primaries and then case, and <c>variant</c>
/// adds tertiaries. <c>ignorePunctuation</c> is UCA's shifted alternate: variable elements and the
/// ignorables after them weigh nothing at the levels compared.
/// </para>
/// <para>
/// <b>A string is normalized to NFD before its elements are looked up</b>, with the profile's own
/// normalization, so canonically equivalent strings compare equal, and comparing charges the work as
/// <c>normalize</c> does.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=UTS10, ECMA-402 s10; IP=Low; Security=Medium; Resources=3; Fingerprint=C83459
// Broiler-Human:        PENDING
internal sealed class JsCollator
{
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B1C622
    // Broiler-Human:        PENDING
    private readonly JsCollationData data;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=A30379
    // Broiler-Human:        PENDING
    private readonly JsCollationData.Tailoring? tailoring;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B9E018
    // Broiler-Human:        PENDING
    internal JsCollator(
        JsCollationData data, JsCollationData.Tailoring? tailoring, int strength, bool caseLevel, JsCaseFirst caseFirst, bool shifted, bool numeric)
    {
        this.data = data;
        this.tailoring = tailoring;
        Strength = strength;
        CaseLevel = caseLevel;
        CaseFirst = caseFirst;
        Shifted = shifted;
        Numeric = numeric;
    }

    /// <summary>1 for primaries only, 2 with secondaries, 3 with tertiaries.</summary>
    internal int Strength { get; }

    /// <summary>Whether case is compared after the primaries.</summary>
    internal bool CaseLevel { get; }

    /// <summary>The case order.</summary>
    internal JsCaseFirst CaseFirst { get; }

    /// <summary>Whether variable elements are shifted out of the compared levels.</summary>
    internal bool Shifted { get; }

    /// <summary>Whether runs of digits compare by value.</summary>
    internal bool Numeric { get; }

    /// <summary>ECMA-402's <c>CompareStrings</c>: -1, 0 or 1.</summary>
    // Broiler-AI:           Origin=AI; Spec=ECMA-402 s10.3.3.1; IP=Low; Security=Medium; Resources=4; Fingerprint=BC15C3
    // Broiler-Falsified-If: canonically equivalent strings compare unequal, or the order differs from the root collation's on the archived CollationTest lines
    // Broiler-Human:        PENDING
    internal int Compare(JsEngine engine, string x, string y)
    {
        if (string.Equals(x, y, System.StringComparison.Ordinal))
        {
            return 0;
        }

        var left = Elements(engine, x);
        var right = Elements(engine, y);
        engine.Charge((ulong)(left.Count + right.Count));

        return Compare(left, right);
    }

    /// <summary>A string's collation elements, after NFD and, when shifted, with variable elements removed.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=DF7BC0
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<ulong> Elements(JsEngine engine, string text)
    {
        var nfd = JsRealm.NormalizeText(engine, text, compose: false, compatibility: false);
        var codePoints = new System.Collections.Generic.List<int>(nfd.Length);

        for (var at = 0; at < nfd.Length; at++)
        {
            if (char.IsHighSurrogate(nfd[at]) && at + 1 < nfd.Length && char.IsLowSurrogate(nfd[at + 1]))
            {
                codePoints.Add(char.ConvertToUtf32(nfd[at], nfd[at + 1]));
                at++;
                continue;
            }

            codePoints.Add(nfd[at]);
        }

        var elements = new System.Collections.Generic.List<ulong>(codePoints.Count + 4);
        data.Elements([.. codePoints], tailoring, Numeric, elements);

        if (!Shifted)
        {
            return elements;
        }

        // SHIFTED (UTS #10 s4.3): A VARIABLE ELEMENT, AND EVERY IGNORABLE ELEMENT AFTER IT UNTIL A
        // NON-VARIABLE PRIMARY, WEIGHS NOTHING AT THE THREE LEVELS COMPARED.
        var kept = new System.Collections.Generic.List<ulong>(elements.Count);
        var afterVariable = false;

        foreach (var element in elements)
        {
            if (JsCollationData.Variable(element))
            {
                afterVariable = true;
                continue;
            }

            if (JsCollationData.Primary(element) == 0 && afterVariable)
            {
                continue;
            }

            if (JsCollationData.Primary(element) != 0)
            {
                afterVariable = false;
            }

            kept.Add(element);
        }

        return kept;
    }

    /// <summary>The level-by-level comparison of two element sequences.</summary>
    // Broiler-AI:           Origin=AI; Spec=UTS10 s7.4; IP=Low; Security=Low; Resources=3; Fingerprint=F59935
    // Broiler-Human:        PENDING
    private int Compare(System.Collections.Generic.List<ulong> left, System.Collections.Generic.List<ulong> right)
    {
        var primary = Level(left, right, static element => JsCollationData.Primary(element), static _ => true);

        if (primary != 0)
        {
            return primary;
        }

        if (CaseLevel)
        {
            var caseOrder = Level(left, right, CaseWeight, static element => JsCollationData.Primary(element) != 0);

            if (caseOrder != 0)
            {
                return caseOrder;
            }
        }

        if (Strength >= 2)
        {
            var secondary = Level(left, right, static element => JsCollationData.Secondary(element), static _ => true);

            if (secondary != 0)
            {
                return secondary;
            }
        }

        if (Strength >= 3)
        {
            return Level(left, right, TertiaryWeight, static _ => true);
        }

        return 0;
    }

    /// <summary>One level: the non-zero weights of the counted elements, compared in order, the shorter sequence first.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=2; Fingerprint=6EB89C
    // Broiler-Human:        PENDING
    private static int Level(
        System.Collections.Generic.List<ulong> left,
        System.Collections.Generic.List<ulong> right,
        System.Func<ulong, ulong> weight,
        System.Func<ulong, bool> counted)
    {
        var i = 0;
        var j = 0;

        while (true)
        {
            var a = Next(left, ref i, weight, counted);
            var b = Next(right, ref j, weight, counted);

            if (a != b)
            {
                return a < b ? -1 : 1;
            }

            if (a == 0)
            {
                return 0;
            }
        }
    }

    /// <summary>The next non-zero weight of a counted element, or zero at the end.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=7EA1C9
    // Broiler-Human:        PENDING
    private static ulong Next(System.Collections.Generic.List<ulong> elements, ref int at, System.Func<ulong, ulong> weight, System.Func<ulong, bool> counted)
    {
        while (at < elements.Count)
        {
            var element = elements[at++];

            if (!counted(element))
            {
                continue;
            }

            var value = weight(element);

            if (value != 0)
            {
                return value;
            }
        }

        return 0;
    }

    /// <summary>
    /// The rank of an element's case in the order asked for: upper, mixed, lower when upper case goes
    /// first, and lower, mixed, upper otherwise.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=6992ED
    // Broiler-Human:        PENDING
    private ulong CaseRank(ulong element)
    {
        var stated = JsCollationData.Case(element);
        return (ulong)(CaseFirst == JsCaseFirst.Upper ? 3 - stated : 1 + stated);
    }

    /// <summary>The case level's weight: the element's case rank.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=EE115D
    // Broiler-Human:        PENDING
    private ulong CaseWeight(ulong element) => CaseRank(element);

    /// <summary>The tertiary weight, with case ordered first when a case order is asked for.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=1EFFFE
    // Broiler-Human:        PENDING
    private ulong TertiaryWeight(ulong element)
    {
        var tertiary = JsCollationData.Tertiary(element);

        if (tertiary == 0 || CaseFirst == JsCaseFirst.Off)
        {
            return tertiary;
        }

        return (CaseRank(element) << 13) | tertiary;
    }
}
