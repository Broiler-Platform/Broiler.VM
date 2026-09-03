// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           0
// Human-reviewed:   0/5
// IP risk:          None
// Security risk:    Medium
// Criteria:         2/0
// Resource impact:  1/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// Whether control can reach the end of a statement, which is what the lowering needs before it
/// emits a loop's continuation.
/// </summary>
/// <remarks>
/// <para>
/// <b>This exists because the lowering emitted code nothing could reach, and the format refuses
/// that.</b> A loop's back-edge is reached by falling off the end of its body; a <c>for</c> loop's
/// update expression is reached by falling off the end of its body or by a <c>continue</c>. A body
/// that always breaks does neither, so both regions are unreachable - and the verifier refuses the
/// artifact as <c>1411:UnreachableCode</c>, which made <c>for (;;) { break; }</c> a program this
/// profile could not run. Thirteen files of a real conformance suite are exactly that shape, and
/// the compiler's own remark had named a different one *(corrected: JSC-58)*.
/// </para>
/// <para>
/// <b>It is a syntactic question and it is answered conservatively.</b> Every answer below either
/// suppresses a region nothing can reach or admits it may be reachable; where the analysis is
/// unsure it says "reachable", which is what the lowering did unconditionally before. So no
/// program that verifies today can stop verifying because of this - the only bytes that move
/// belong to programs the verifier was already refusing.
/// </para>
/// <para>
/// <b>The statement set is small and closed, which is what makes this tractable.</b>
/// <c>broiler.javascript.slice</c> admits declarations, expression statements, blocks, <c>if</c>,
/// the three loops, <c>break</c> and <c>continue</c>, and nothing else reaches the lowering: no
/// function and so no <c>return</c>, no <c>throw</c>, no <c>try</c>, no <c>switch</c> and no
/// labels. A general front end would need a real reachability pass; this one needs a walk over
/// nine node kinds.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=C86662
// Broiler-Human:        PENDING
internal static class SliceControlFlow
{
    /// <summary>
    /// Whether control cannot reach the point just after <paramref name="statement"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A loop always answers <c>false</c>, deliberately, and that is not laziness.</b> A loop
    /// with no exit - <c>for (;;) { var x = 1; }</c> - genuinely never falls through, so a true
    /// answer would be more precise. It would also make everything after such a loop unreachable
    /// INCLUDING THE PROGRAM'S OWN TAIL, and suppressing that leaves a function with no
    /// terminator: a different invalid artifact rather than a valid one. That shape stays the
    /// documented exclusion it already was, and answering <c>false</c> here is what keeps it
    /// exactly as refused as it is today rather than newly broken in another way.
    /// </para>
    /// <para>
    /// <b><c>if</c> with no <c>else</c> also answers <c>false</c></b>, because the test may be
    /// false and the branch not taken - whatever the consequent does.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=757CFC
    // Broiler-Falsified-If: a statement control can reach past is reported as terminating, which would suppress reachable code
    // Broiler-Human:        PENDING
    internal static bool Terminates(SliceStatement statement) => statement switch
    {
        // The two transfers this manifest has. Neither falls through.
        SliceBreakStatement => true,
        SliceContinueStatement => true,

        // A block cannot be fallen out of once one of its statements cannot be fallen out of.
        // Every statement after that one is dead, which is a separate shape this does not repair -
        // see the remark on `SliceSourceCompiler`.
        SliceBlockStatement block => AnyTerminates(block.Body),

        // Both arms, or neither. A missing alternate is a path around the whole statement.
        SliceIfStatement branch =>
            branch.Alternate is not null &&
            Terminates(branch.Consequent) &&
            Terminates(branch.Alternate),

        // Conservative for the reason the remark above gives.
        SliceWhileStatement => false,
        SliceDoWhileStatement => false,
        SliceForStatement => false,

        // A declaration, an expression statement, an empty statement, and anything a later
        // manifest admits: assume control continues. The lowering emitted the continuation
        // unconditionally before this type existed, so this arm is the behaviour that was.
        _ => false,
    };

    /// <summary>Whether any statement in a block cannot be fallen out of.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=F0A7CA
    // Broiler-Human:        PENDING
    private static bool AnyTerminates(
        System.Collections.Generic.IReadOnlyList<SliceStatement> body)
    {
        foreach (var statement in body)
        {
            if (Terminates(statement))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Whether a <c>continue</c> inside <paramref name="body"/> targets the loop that
    /// <paramref name="body"/> is the body of.
    /// </summary>
    /// <remarks>
    /// <b>The walk stops at a nested loop, and that is the whole of the question.</b> A
    /// <c>continue</c> inside an inner loop targets the inner loop, so it says nothing about
    /// whether the outer loop's update expression can be reached. Counting it would keep emitting
    /// an update nothing reaches - which is the defect - and missing a real one would suppress an
    /// update something does reach, which is worse. So the recursion descends through blocks and
    /// <c>if</c> arms and refuses to enter a loop.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=1116FB
    // Broiler-Falsified-If: a `continue` targeting an inner loop is counted for an outer one, or one targeting this loop is missed
    // Broiler-Human:        PENDING
    internal static bool ContinuesThisLoop(SliceStatement body) => body switch
    {
        SliceContinueStatement => true,
        SliceBlockStatement block => AnyContinuesThisLoop(block.Body),
        SliceIfStatement branch =>
            ContinuesThisLoop(branch.Consequent) ||
            (branch.Alternate is not null && ContinuesThisLoop(branch.Alternate)),

        // A nested loop owns every `continue` inside it. The walk stops here.
        SliceWhileStatement => false,
        SliceDoWhileStatement => false,
        SliceForStatement => false,

        _ => false,
    };

    /// <summary>Whether any statement in a block continues the loop the block is the body of.</summary>
    /// <remarks>
    /// Written as a loop rather than with a query, because this assembly names its types in full
    /// and takes no implicit usings; a query here would be the only one in it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=C51208
    // Broiler-Human:        PENDING
    private static bool AnyContinuesThisLoop(
        System.Collections.Generic.IReadOnlyList<SliceStatement> body)
    {
        foreach (var statement in body)
        {
            if (ContinuesThisLoop(statement))
            {
                return true;
            }
        }

        return false;
    }
}
