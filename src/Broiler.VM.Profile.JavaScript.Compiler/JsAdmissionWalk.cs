// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           3
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    High
// Criteria:         4/4
// Resource impact:  2/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Numbers;
using Broiler.ECMAScript.Syntax;
using NumericLiteral = Broiler.ECMAScript.Syntax.NumericLiteral;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The admission walk: the policy and limit refusals the wide parser made during the parse, made over
/// the shared tree after a parse that reported nothing, and before the numeric admission and the binder
/// (JSD-0083, decision D18; milestone SJL-13.3, step 1).
/// </summary>
/// <remarks>
/// <para>
/// <b>Three refusals, each this family's and none the library's</b> (JSD-0080's "What each engine
/// keeps"): a BigInt literal under a manifest that does not admit BigInt (2104, EX0017), a BigInt literal
/// or key wider than the format's constant ceiling (2106, EX0028), and a module specifier that is not
/// well-formed Unicode (2101, EX0029). Each has the code, the text and the place the wide parser gave it;
/// a BigInt key, which the wide parser read through its property-key rule, is held to the ceiling under
/// every manifest, as it was.
/// </para>
/// <para>
/// <b>It is made as the adapter visits the tree</b>, once per node, and its refusals are reported in
/// source order, up to the cap a stage reports; a unit with any is refused and stops, as the parse's
/// refusal stopped it. So such a refusal is absent when the parse refuses anywhere in the unit, and no
/// longer precedes a later parse refusal (bundle <c>sjl-13-001</c>'s part B, class S3, and part A's kind
/// (B)).
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=420490
// Broiler-Falsified-If: a BigInt literal outside an admitting manifest, a BigInt wider than the format's ceiling, or a module specifier that is not well-formed reaches the numeric admission, the binder or the lowering unrefused, or a source the wide parser admitted is refused here
// Broiler-Human:        PENDING
internal sealed class JsAdmissionWalk
{
    /// <summary>Whether the request's manifest admits BigInt literals.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=2BF083
    // Broiler-Human:        PENDING
    private readonly bool admitsBigInt;

    /// <summary>The refusals found so far, each with the offset it is ordered by.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=7A49F2
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<(int Offset, SliceSourceDiagnostic Diagnostic)> found = [];

    /// <summary>A walk under a manifest that does or does not admit BigInt literals.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=5EC5D3
    // Broiler-Human:        PENDING
    internal JsAdmissionWalk(bool admitsBigIntLiterals)
    {
        admitsBigInt = admitsBigIntLiterals;
    }

    /// <summary>How many refusals the walk has made.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=89E3D2
    // Broiler-Human:        PENDING
    internal int Count => found.Count;

    /// <summary>The walk's refusals in source order, up to the cap a stage reports.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=99297B
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.IReadOnlyList<SliceSourceDiagnostic> Refusals()
    {
        var ordered = new System.Collections.Generic.List<(int Offset, int Order, SliceSourceDiagnostic Diagnostic)>(found.Count);

        for (var at = 0; at < found.Count; at++)
        {
            ordered.Add((found[at].Offset, at, found[at].Diagnostic));
        }

        ordered.Sort(static (a, b) => a.Offset != b.Offset ? a.Offset.CompareTo(b.Offset) : a.Order.CompareTo(b.Order));

        var refusals = new System.Collections.Generic.List<SliceSourceDiagnostic>(System.Math.Min(ordered.Count, SliceSourceDiagnostic.MaximumPerStage));

        foreach (var entry in ordered)
        {
            if (refusals.Count >= SliceSourceDiagnostic.MaximumPerStage)
            {
                break;
            }

            refusals.Add(entry.Diagnostic);
        }

        return refusals;
    }

    /// <summary>
    /// A BigInt literal in an expression: refused under a manifest that does not admit BigInt, and
    /// otherwise held to the format's ceiling; its value when it is admitted.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=39B2DD
    // Broiler-Falsified-If: a BigInt literal is admitted under a manifest that does not admit BigInt, or past the format's ceiling
    // Broiler-Human:        PENDING
    internal bool BigIntLiteral(NumericLiteral literal, out System.Numerics.BigInteger value)
    {
        value = default;

        if (!admitsBigInt)
        {
            Refuse(
                literal,
                SliceSourceDiagnosticCode.ConstructOutsideManifest,
                "a BigInt literal is not admitted by the declared feature manifest");

            return false;
        }

        return BigIntKey(literal, out value);
    }

    /// <summary>A BigInt literal held to the format's ceiling, as a key is under every manifest; its value when it fits.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=408D34
    // Broiler-Falsified-If: a BigInt wider than the format's constant ceiling is admitted, or one within it is refused
    // Broiler-Human:        PENDING
    internal bool BigIntKey(NumericLiteral literal, out System.Numerics.BigInteger value)
    {
        var status = NumericLiterals.Evaluate(
            literal.Text,
            false,
            new Broiler.ECMAScript.LengthLimit(Format.JsFormat.CeilingBigIntConstantBytes * 8L),
            out var evaluated,
            out _);

        value = evaluated.BigInt;

        if (status == Broiler.ECMAScript.ResultStatus.Completed && evaluated.Kind == NumericLiteralKind.BigInt)
        {
            return true;
        }

        Refuse(
            literal,
            SliceSourceDiagnosticCode.BigIntWiderThanTheFormat,
            "a BigInt literal wider than " +
                (Format.JsFormat.CeilingBigIntConstantBytes * 8).ToString(System.Globalization.CultureInfo.InvariantCulture) +
                " bits is not admitted by the declared feature manifest");

        return false;
    }

    /// <summary>
    /// A module specifier, refused where it holds a lone surrogate (EX0029, with its code's one text, JSE9309's).
    /// <i>(Revised 2026-11-09, later that day, with JSD-0083's note of 2026-11-09, later that day, on decision D11: it read "with the wide
    /// parser's text", which was the text of an export name's refusal, "an export name holds a lone surrogate, which is
    /// not well-formed Unicode", and is false of a module specifier.)</i>
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=7D0C54
    // Broiler-Falsified-If: a module specifier holding a lone surrogate is admitted, or a well-formed one refused
    // Broiler-Human:        PENDING
    internal void ModuleSpecifier(StringLiteral specifier)
    {
        if (!IsWellFormed(specifier.Value))
        {
            Refuse(
                specifier,
                SliceSourceDiagnosticCode.UnexpectedToken,
                "a module specifier holds a lone surrogate, which is not well-formed Unicode");
        }
    }

    /// <summary>Whether <paramref name="text"/> pairs every surrogate it holds.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=B9C1FD
    // Broiler-Human:        PENDING
    private static bool IsWellFormed(string text)
    {
        for (var at = 0; at < text.Length; at++)
        {
            if (char.IsHighSurrogate(text[at]) && at + 1 < text.Length && char.IsLowSurrogate(text[at + 1]))
            {
                at++;
            }
            else if (char.IsSurrogate(text[at]))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Records one refusal at a node's first character.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=6FC4F9
    // Broiler-Human:        PENDING
    private void Refuse(SyntaxNode node, SliceSourceDiagnosticCode code, string message) =>
        found.Add((node.Span.Offset, new SliceSourceDiagnostic(code, message, node.Span.Line, node.Span.Column)));
}
