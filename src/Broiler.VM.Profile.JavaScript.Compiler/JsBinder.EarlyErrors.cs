// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   4
// Annotated:        4/4
// Exempt:           0
// Human-reviewed:   0/4
// IP risk:          None
// Security risk:    High
// Criteria:         2/1
// Resource impact:  3/10 max
// Unverified:       4
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The binder's early errors: the language early errors the wide lowering used to raise while it
/// emitted, found once per unit, before any of it is lowered.
/// </summary>
/// <remarks>
/// <para>
/// <b>ONE STAGE</b> (SJL-12.6 item 2, JSD-0080 decisions D10 and D12). After the parse, the wide path
/// has one early-error stage, and this is it: a unit it refuses is not lowered, so an early error
/// never produces bytes, and none is reported once per emission - a <c>finally</c> block the lowering
/// emits three times is walked here once. The parser keeps its own checks until SJL-13 deletes it.
/// </para>
/// <para>
/// <b>EVERY CODE AND EVERY MESSAGE IS THE LOWERING'S, AT THE PLACE IT GAVE.</b> No registry code moves
/// (decision D13): a refusal the catalogue records as a split candidate keeps the number it has. What
/// the lowering decided from its own flags - whether code is a method's, a derived constructor's, a
/// function's; which private names and which <c>arguments</c> are in scope; which statement a jump
/// leaves - is decided here from the same reading of the same position.
/// </para>
/// <para>
/// <b>WHAT STAYS WITH THE LOWERING</b> is what is not a language early error: the format's ceilings,
/// the manifest's refusals, the module-load policy and the evaluated program's scope-depth ceiling -
/// and, under the codes the catalogue records for them, the three language rules the lowering refuses
/// as constructs outside the manifest and <c>return</c> in a static block. Code the lowering lowers no
/// further after one of those is walked silently here, so that refusal stays the unit's answer.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=BE330C
// Broiler-Falsified-If: the binder refuses a unit the lowering lowered without a language early error, or admits one the lowering refused with one, or reports a code, a message or a place other than the lowering's
// Broiler-Human:        PENDING
internal sealed partial class JsBinder
{
    /// <summary>How many early errors a unit is answered with at most, as the parser's diagnostics are.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=BBF362
    // Broiler-Human:        PENDING
    private const int MaximumDiagnostics = 64;

    /// <summary>Records one early error.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=8CCA25
    // Broiler-Human:        PENDING
    private void Report(SliceSourceSpan span, SliceSourceDiagnosticCode code, string message) =>
        found.Add(new SliceSourceDiagnostic(code, message, span.Line, span.Column));

    /// <summary>The early errors found, in source order, at most <see cref="MaximumDiagnostics"/> of them.</summary>
    /// <remarks>
    /// <b>Source order is the order of the places reported</b>, line and then column, and two at one
    /// place keep the order the walk met them in. The walk meets nodes in source order, but a few
    /// errors are reported at a place other than the node's own - a field initialiser's at the
    /// field, a module's import collision at no place at all, which is line 0 - so the order is made
    /// rather than assumed.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=E19B87
    // Broiler-Falsified-If: two early errors are answered in an order other than their places', or more than the cap is answered
    // Broiler-Human:        PENDING
    private static SliceSourceDiagnostic[] InSourceOrder(System.Collections.Generic.List<SliceSourceDiagnostic> errors)
    {
        if (errors.Count == 0)
        {
            return [];
        }

        var order = new int[errors.Count];

        for (var at = 0; at < order.Length; at++)
        {
            order[at] = at;
        }

        System.Array.Sort(order, (left, right) =>
        {
            var compared = errors[left].Line.CompareTo(errors[right].Line);

            if (compared == 0)
            {
                compared = errors[left].Column.CompareTo(errors[right].Column);
            }

            return compared != 0 ? compared : left.CompareTo(right);
        });

        var kept = new SliceSourceDiagnostic[System.Math.Min(order.Length, MaximumDiagnostics)];

        for (var at = 0; at < kept.Length; at++)
        {
            kept[at] = errors[order[at]];
        }

        return kept;
    }

}
