// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   10
// Annotated:        10/10
// Exempt:           2
// Human-reviewed:   0/10
// IP risk:          Low
// Security risk:    Medium
// Criteria:         1/0
// Resource impact:  2/10 max
// Unverified:       10
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The operands of a formatted number that a plural rule reads (UTS #35 Part 3 section 5.1.1): the
/// absolute value, the integer digits, the visible fraction digits with and without trailing zeros,
/// and the compact exponent.
/// </summary>
/// <remarks>
/// <b>Each operand is held as its last digits and whether it is an integer</b>: a rule takes a value
/// modulo a small number or compares it with a small range, so a huge integer part needs only its
/// low digits and a flag that it exceeds every range.
/// </remarks>
// Broiler-AI:           Origin=AI; Spec=UTS35 Part 3 s5.1.1; IP=Low; Security=Low; Resources=1; Fingerprint=63F0D0
// Broiler-Human:        PENDING
internal readonly record struct JsPluralOperands(
    long Integer, bool IntegerHuge, string Fraction, int CompactExponent)
{
    /// <summary>The operands of a formatted decimal string: ASCII digits with an optional <c>.</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=B987AF
    // Broiler-Human:        PENDING
    internal static JsPluralOperands Of(string formatted, int compactExponent)
    {
        var point = formatted.IndexOf('.');
        var integer = point < 0 ? formatted : formatted[..point];
        var fraction = point < 0 ? string.Empty : formatted[(point + 1)..];

        var trimmed = integer.TrimStart('0');
        var huge = trimmed.Length > 15;
        var low = trimmed.Length == 0 ? 0 : long.Parse(huge ? trimmed[^15..] : trimmed, System.Globalization.CultureInfo.InvariantCulture);

        return new JsPluralOperands(low, huge, fraction, compactExponent);
    }

    /// <summary>The number of visible fraction digits, <c>v</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=112422
    // Broiler-Human:        PENDING
    internal int V => Fraction.Length;

    /// <summary>The visible fraction digits without trailing zeros.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=0AF713
    // Broiler-Human:        PENDING
    internal string T => Fraction.TrimEnd('0');
}

/// <summary>
/// One CLDR plural rule: an <c>or</c> of <c>and</c>s of relations (UTS #35 Part 3 section 5.1).
/// </summary>
// Broiler-AI:           Origin=AI; Spec=UTS35 Part 3 s5.1; IP=Low; Security=Medium; Resources=2; Fingerprint=B58D04
// Broiler-Human:        PENDING
internal sealed class JsPluralRule
{
    /// <summary>One relation: an operand, an optional modulus, whether it is negated, and its ranges.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=8D05DC
    // Broiler-Human:        PENDING
    private sealed record Relation(char Operand, long Modulus, bool Negated, (long Low, long High)[] Ranges);

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=69AE46
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<System.Collections.Generic.List<Relation>> alternatives;

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=0; Fingerprint=C1E0B9
    // Broiler-Human:        PENDING
    private JsPluralRule(System.Collections.Generic.List<System.Collections.Generic.List<Relation>> alternatives) =>
        this.alternatives = alternatives;

    /// <summary>Reads a rule's condition; the empty condition holds always.</summary>
    // Broiler-AI:           Origin=AI; Spec=UTS35 Part 3 s5.1; IP=Low; Security=Medium; Resources=2; Fingerprint=06CC3C
    // Broiler-Falsified-If: a rule from the pinned plurals.json reads as a condition other than the one it states
    // Broiler-Human:        PENDING
    internal static JsPluralRule Parse(string condition)
    {
        var alternatives = new System.Collections.Generic.List<System.Collections.Generic.List<Relation>>();

        if (condition.Trim().Length == 0)
        {
            return new JsPluralRule(alternatives);
        }

        foreach (var alternative in condition.Split(" or "))
        {
            var relations = new System.Collections.Generic.List<Relation>();

            foreach (var text in alternative.Split(" and "))
            {
                relations.Add(ParseRelation(text.Trim()));
            }

            alternatives.Add(relations);
        }

        return new JsPluralRule(alternatives);
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=098049
    // Broiler-Human:        PENDING
    private static Relation ParseRelation(string text)
    {
        var words = text.Replace("!=", " \u0001 ").Replace("=", " = ").Replace("\u0001", "!=").Replace("%", " % ")
            .Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        var at = 0;
        var operand = words[at++][0];
        long modulus = 0;

        if (words[at] is "%" or "mod")
        {
            modulus = long.Parse(words[at + 1], System.Globalization.CultureInfo.InvariantCulture);
            at += 2;
        }

        var negated = false;

        switch (words[at])
        {
            case "!=":
                negated = true;
                at++;
                break;
            case "=":
                at++;
                break;
            case "is":
                at++;

                if (words[at] == "not")
                {
                    negated = true;
                    at++;
                }

                break;
            default:
                if (words[at] == "not")
                {
                    negated = true;
                    at++;
                }

                at++;
                break;
        }

        var ranges = new System.Collections.Generic.List<(long, long)>();

        foreach (var item in string.Concat(words[at..]).Split(','))
        {
            var dots = item.IndexOf("..", System.StringComparison.Ordinal);
            ranges.Add(dots < 0
                ? (long.Parse(item, System.Globalization.CultureInfo.InvariantCulture), long.Parse(item, System.Globalization.CultureInfo.InvariantCulture))
                : (long.Parse(item[..dots], System.Globalization.CultureInfo.InvariantCulture), long.Parse(item[(dots + 2)..], System.Globalization.CultureInfo.InvariantCulture)));
        }

        return new Relation(operand, modulus, negated, [.. ranges]);
    }

    /// <summary>Whether the rule holds for a formatted number's operands.</summary>
    // Broiler-AI:           Origin=AI; Spec=UTS35 Part 3 s5.1; IP=Low; Security=Low; Resources=1; Fingerprint=E67421
    // Broiler-Human:        PENDING
    internal bool Holds(JsPluralOperands operands)
    {
        if (alternatives.Count == 0)
        {
            return true;
        }

        foreach (var relations in alternatives)
        {
            if (relations.TrueForAll(relation => Holds(relation, operands)))
            {
                return true;
            }
        }

        return false;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=4E9B73
    // Broiler-Human:        PENDING
    private static bool Holds(Relation relation, JsPluralOperands operands)
    {
        // THE OPERAND'S VALUE: an integer (its low digits, and whether more lie above them), or for
        // n with a visible nonzero fraction, a value no integer range contains.
        long value;
        bool integral;
        bool huge;

        switch (relation.Operand)
        {
            case 'n':
                value = operands.Integer;
                integral = operands.T.Length == 0;
                huge = operands.IntegerHuge;
                break;
            case 'i':
                value = operands.Integer;
                integral = true;
                huge = operands.IntegerHuge;
                break;
            case 'v':
                value = operands.V;
                integral = true;
                huge = false;
                break;
            case 'w':
                value = operands.T.Length;
                integral = true;
                huge = false;
                break;
            case 'f':
            case 't':
                var digits = (relation.Operand == 'f' ? operands.Fraction : operands.T).TrimStart('0');
                huge = digits.Length > 15;
                value = digits.Length == 0 ? 0 : long.Parse(huge ? digits[^15..] : digits, System.Globalization.CultureInfo.InvariantCulture);
                integral = true;
                break;
            case 'c':
            case 'e':
                value = operands.CompactExponent;
                integral = true;
                huge = false;
                break;
            default:
                return false;
        }

        if (relation.Modulus != 0)
        {
            value %= relation.Modulus;
            huge = false;
        }

        var inRange = false;

        if (integral && !huge)
        {
            foreach (var (low, high) in relation.Ranges)
            {
                if (low <= value && value <= high)
                {
                    inRange = true;
                    break;
                }
            }
        }

        return inRange != relation.Negated;
    }
}
