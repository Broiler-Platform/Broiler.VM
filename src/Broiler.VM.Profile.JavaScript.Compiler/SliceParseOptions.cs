// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           5
// Human-reviewed:   0/8
// IP risk:          None
// Security risk:    Medium
// Criteria:         2/1
// Resource impact:  0/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>The grammar goal a parse is run against.</summary>
/// <remarks>
/// The two goals are not a flag on one grammar: a module is strict whatever its prologue says,
/// and <c>await</c> is a keyword at a module's top level and an identifier at a script's. Both
/// facts are read off this value and off nothing ambient.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=1F9F9B
// Broiler-Human:        PENDING
public enum SliceGoal
{
    /// <summary>The <c>Script</c> goal symbol. Strictness comes from the directive prologue.</summary>
    Script = 0,

    /// <summary>The <c>Module</c> goal symbol. Strict whatever the prologue says.</summary>
    Module = 1,
}

/// <summary>
/// Everything a parse needs to know that is not the source text, passed in by value.
/// </summary>
/// <remarks>
/// <para>
/// <b>This exists because the seed reads its two most consequential grammar switches out of
/// ambient async-local state in a different assembly</b>, which roadmap section 9 rejects for
/// three separate reasons: it is a hidden dependency across a boundary this fork removes, it makes
/// two concurrent parses with different goals mutually corrupting, and ambient per-thread state
/// in a profile is the shape the core's lifecycle rules exist to keep out. The replacement is this
/// value, and the gate is a test in which two parses with different goals run concurrently in one
/// process, each producing the goal-appropriate result, and which fails when the options are
/// replaced by a shared static.
/// </para>
/// <para>
/// <b>It is a readonly record struct and every field is set at construction.</b> There is no
/// mutable global, no <c>[ThreadStatic]</c> and no <c>AsyncLocal</c> anywhere in this assembly,
/// which is a property a scan can assert rather than a convention read out of the prose.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=8A7C8A
// Broiler-Falsified-If: any grammar or strictness decision in this assembly reads a value that did not arrive through this type
// Broiler-Human:        PENDING
public readonly record struct SliceParseOptions
{
    /// <summary>The default depth bound. See <see cref="MaximumNestingDepth"/> for why there is one.</summary>
    /// <remarks>
    /// Chosen so that the deepest program the retained nesting cases build is refused well inside
    /// the smallest stack this component publishes on, and not by measurement: a bound derived
    /// from a measured stack would be a number that moves with the runtime, and the whole point
    /// of the bound is that the refusal is the same answer everywhere.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=D4074B
    // Broiler-Human:        PENDING
    public const int DefaultMaximumNestingDepth = 64;

    /// <summary>Creates the options for <paramref name="goal"/> with every other switch defaulted.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=C3E535
    // Broiler-Human:        PENDING
    public SliceParseOptions(SliceGoal goal)
        : this(goal, allowTopLevelAwait: goal == SliceGoal.Module, DefaultMaximumNestingDepth)
    {
    }

    /// <summary>Creates the options with every switch stated.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=925097
    // Broiler-Human:        PENDING
    public SliceParseOptions(SliceGoal goal, bool allowTopLevelAwait, int maximumNestingDepth)
    {
        if (maximumNestingDepth < 1)
        {
            throw new System.ArgumentOutOfRangeException(
                nameof(maximumNestingDepth),
                maximumNestingDepth,
                "a parse with no depth allowance can accept no program at all");
        }

        Goal = goal;
        AllowTopLevelAwait = allowTopLevelAwait;
        MaximumNestingDepth = maximumNestingDepth;
    }

    /// <summary>The grammar goal symbol.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=CF303C
    // Broiler-Human:        PENDING
    public SliceGoal Goal { get; }

    /// <summary>Whether <c>await</c> is a keyword at the top level.</summary>
    /// <remarks>
    /// The slice manifest admits no <c>await</c> in any position, so today this switch only ever
    /// changes which refusal a program gets - a construct outside the manifest, or an identifier
    /// named <c>await</c>. It is carried anyway, because it is the second of the two switches the
    /// seed made ambient and leaving it out would leave the gate above testing one of them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=DB0BD1
    // Broiler-Human:        PENDING
    public bool AllowTopLevelAwait { get; }

    /// <summary>How deep the parser, the validator and the lowering may each recurse.</summary>
    /// <remarks>
    /// <para>
    /// <b>This is roadmap section 9's nesting answer, and the answer is an explicit bound rather
    /// than a worklist rewrite.</b> <c>CallDepth</c> bounds guest frames and reaches none of the
    /// three compile-time recursions; the seed mitigates the same problem with stack segmentation
    /// and an oversized thread, neither of which this component has. A bound is chosen over a
    /// worklist because the three recursions are over three different shapes, a worklist rewrite
    /// is three rewrites, and a nesting case must be <b>refused</b> rather than survived - a
    /// worklist that survives arbitrarily deep input answers a case the roadmap wants refused.
    /// </para>
    /// <para>
    /// The cost is stated rather than hidden: a program nested deeper than this is refused by
    /// this profile and accepted by every other JavaScript implementation, so the bound is a
    /// conformance exclusion and JS-3a's harness has to score it as one.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=0; Fingerprint=FA7632
    // Broiler-Falsified-If: a source nested deeper than this bound terminates the process instead of being refused
    // Broiler-Human:        PENDING
    public int MaximumNestingDepth { get; }

    /// <summary>Whether the goal itself forces strict code, before any directive is read.</summary>
    /// <remarks>
    /// The parser does not read this. It is the validator's, which is where the strict-mode
    /// ruling lives; see <c>SliceStaticSemantics</c> for the decision and its reason.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A5404E
    // Broiler-Human:        PENDING
    public bool GoalIsStrict => Goal == SliceGoal.Module;

    /// <summary>The options a script parse uses when a caller states nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=3C749D
    // Broiler-Human:        PENDING
    public static SliceParseOptions Script => new(SliceGoal.Script);

    /// <summary>The options a module parse uses when a caller states nothing.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=E7520C
    // Broiler-Human:        PENDING
    public static SliceParseOptions Module => new(SliceGoal.Module);
}
