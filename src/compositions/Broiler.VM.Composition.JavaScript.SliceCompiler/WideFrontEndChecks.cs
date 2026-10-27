// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Compiler;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The wide path's halves of two claims the slice path's rows already hold: two goals in one process
/// do not reach each other, and a deep-nesting corpus is refused rather than survived (SJL-13's tools
/// commit, JSD-0083 decision D23).
/// </summary>
/// <remarks>
/// <para>
/// <b>Written 2026-10-27, at SJL-13's tools commit, before any change bundle <c>sjl-13-001</c>'s rule
/// judges.</b> The rows that held these items until now, "two goals in one process do not reach each
/// other" and "deep nesting is refused rather than survived" in <see cref="SourceFrontEndChecks"/>,
/// compile through <see cref="SliceSourceCompiler"/> only. JS-2's "two parses with different goals run
/// concurrently" and "a deep-nesting corpus refused rather than terminating the process" are about the
/// wide path too, and that is the path SJL-13 moves onto the shared syntax library, so the rule's part C
/// runs these two rows at both of step 1's commits.
/// </para>
/// <para>
/// <b>Every source here is one the base already answers as the row requires.</b> A row that failed at
/// the base could not be judged at both commits (decision D23). So the deep-nesting corpus holds no
/// chain whose left operand's spine the base leaves uncharged: no binary chain whose first operand is a
/// parenthesised chain or a member or call chain, no template whose substitution holds such a chain,
/// and no member or call chain whose object is a parenthesised binary chain (bundle
/// <c>sjl-12-001</c>'s README, "The parser's left-operand gap"; the correspondence's row R14). Step 1
/// adds those four shapes as a row of their own.
/// </para>
/// </remarks>
internal static class WideFrontEndChecks
{
    internal static IEnumerable<(string Name, bool Passed, string Detail)> Run()
    {
        yield return TwoGoalsOnTheWidePathDoNotReachEachOther();
        yield return ADeepNestingCorpusIsRefusedOnTheWidePath();
    }

    /// <summary>How many rounds the two-goals row runs, each a Script and a Module compilation at once.</summary>
    private const int Rounds = 200;

    /// <summary>
    /// The sources the two-goals row compiles under both goals, each answered differently by the two.
    /// </summary>
    /// <remarks>
    /// <b>Each source's answer depends on its goal and on nothing else.</b> The first is accepted under
    /// both and lowers to two different artifacts, because <c>this</c> at a module's top level is not
    /// the global object; the second and third are accepted as a script and refused as a module, a
    /// module being strict and <c>await</c> reserved in it. A compilation answered under the other
    /// goal's request therefore shows as a different artifact hash, or as a refusal where an artifact
    /// was due, or an artifact where a refusal was. The filler declarations make each compilation long
    /// enough that two of them overlap on a machine with more than one core.
    /// </remarks>
    private static readonly (string Name, string Text, SliceSourceDiagnosticCode? ModuleRefusal)[] GoalSources =
    [
        ("this at the top level", Padded("var x = this; x;"), null),
        ("a legacy octal literal", Padded("var y = 0123; y;"), SliceSourceDiagnosticCode.LegacyOctalInStrictCode),
        ("await as a binding name", Padded("var await = 1; await;"), SliceSourceDiagnosticCode.ReservedWordAsBinding),
    ];

    /// <summary>One answer of the wide path: an artifact's hash, or the first refusal's code.</summary>
    private readonly record struct Answer(bool Accepted, string Hash, SliceSourceDiagnosticCode? First)
    {
        public override string ToString() =>
            Accepted
                ? "accepted " + Hash[..16]
                : "refused " + (First?.ToString() ?? "with no diagnostic");
    }

    /// <summary>
    /// Script and Module compilations of one text, run concurrently through
    /// <see cref="JsCompiler.Compile(IReadOnlyList{JsScriptUnit}, IReadOnlyList{JsModuleUnit}, JsCompileRequest)"/>,
    /// each answer the one that goal gives alone.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is roadmap section 9's parse-options gate on the wide path, and it fails when the request
    /// is shared between the goals.</b> The expected answers are taken first, one compilation at a time,
    /// and then every round starts a Script and a Module compilation of one text together; a front end
    /// that kept the goal, the strictness or any other part of the request anywhere but in the
    /// compilation answers one of them under the other's rule on some rounds and not on others, which is
    /// why it runs many rounds rather than one.
    /// </para>
    /// <para>
    /// <b>The expected answers are checked before they are used</b>: the first source's two artifacts
    /// must differ, and the other two must be accepted as a script and refused as a module with the code
    /// the language's rule gives. A pair of goals that answered alike would make a mix-up invisible.
    /// </para>
    /// </remarks>
    private static (string, bool, string) TwoGoalsOnTheWidePathDoNotReachEachOther()
    {
        const string Name = "two goals on the wide path in one process do not reach each other";
        var failed = new List<string>();
        var expected = new (Answer Script, Answer Module)[GoalSources.Length];

        for (var index = 0; index < GoalSources.Length; index++)
        {
            var (name, text, refusal) = GoalSources[index];
            expected[index] = (Compile(text, SliceGoal.Script), Compile(text, SliceGoal.Module));
            var (script, module) = expected[index];

            if (!script.Accepted)
            {
                failed.Add($"{name}: refused as a script, alone: {script}");
            }

            if (refusal is null
                    ? !module.Accepted || module.Hash == script.Hash
                    : module.Accepted || module.First != refusal)
            {
                failed.Add(
                    $"{name}: as a module, alone, answered {module}, where " +
                    (refusal is null ? "an artifact other than the script's" : "a refusal with " + refusal) +
                    " was due");
            }
        }

        if (failed.Count != 0)
        {
            return (Name, false, "the goals' own answers are not distinct: " + string.Join("; ", failed));
        }

        var wrong = 0;
        var first = string.Empty;

        Parallel.For(0, Rounds, round =>
        {
            var index = round % GoalSources.Length;
            var text = GoalSources[index].Text;
            Answer script = default, module = default;

            Parallel.Invoke(
                () => script = Compile(text, SliceGoal.Script),
                () => module = Compile(text, SliceGoal.Module));

            foreach (var (goal, answer, due) in new[]
            {
                ("script", script, expected[index].Script),
                ("module", module, expected[index].Module),
            })
            {
                if (answer != due)
                {
                    if (Interlocked.Increment(ref wrong) == 1)
                    {
                        Interlocked.Exchange(
                            ref first,
                            $"round {round.ToString(CultureInfo.InvariantCulture)}, {GoalSources[index].Name} " +
                            $"as a {goal}: {answer} where it answers {due} alone");
                    }
                }
            }
        });

        return (
            Name,
            wrong == 0,
            wrong == 0
                ? $"{Rounds} concurrent Script and Module compilations of {GoalSources.Length} texts through " +
                  "JsCompiler.Compile, each answering as its goal answers alone: one text two artifacts, two " +
                  "texts an artifact as a script and a refusal as a module"
                : $"{wrong} of {Rounds * 2} compilations answered as the other goal does; the first: {first}");
    }

    /// <summary>
    /// A corpus nested past the wide path's nesting bound, and past its tree-depth bound by a spine
    /// the base charges, each refused with <see cref="SliceSourceDiagnosticCode.NestingTooDeep"/>, and
    /// the same shapes inside the bounds compiled.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Both halves, because only the pair says the bound is a bound</b>, as the slice row says: a
    /// front end that refused everything would pass the first half, and one that refused nothing the
    /// second. Each deep source is far past any plausible stack, so a run that reaches the assertion at
    /// all is a run in which the bound, and not the operating system, answered; that is what "the
    /// process alive" means here.
    /// </para>
    /// <para>
    /// <b>The shapes.</b> Nesting through brackets, parentheses, blocks, functions and template
    /// substitutions, each past <see cref="SliceParseOptions.DefaultMaximumNestingDepth"/>; and the tree
    /// depth through a spine the base charges one level per node, a binary chain whose first operand is
    /// a name and a member chain and a call chain whose object is a name, each past
    /// <see cref="SliceParseOptions.MaximumTreeDepth"/>. The refusal is read from the first diagnostic,
    /// the one every reader grades.
    /// </para>
    /// </remarks>
    private static (string, bool, string) ADeepNestingCorpusIsRefusedOnTheWidePath()
    {
        const string Name = "a deep-nesting corpus is refused on the wide path rather than survived";
        const int Inside = 20;
        const int Past = 100_000;
        const int SpineInside = SliceParseOptions.MaximumTreeDepth / 2;
        const int SpinePast = SliceParseOptions.MaximumTreeDepth * 100;

        (string Shape, Func<int, string> Source, int Inside, int Past)[] corpus =
        [
            ("brackets", depth => "var a = " + Repeat("[", depth) + "1" + Repeat("]", depth) + ";", Inside, Past),
            ("parentheses", depth => "var a = " + Repeat("(", depth) + "1" + Repeat(")", depth) + ";", Inside, Past),
            ("blocks", depth => Repeat("{", depth) + "1;" + Repeat("}", depth), Inside, Past),
            ("functions", depth => Repeat("function f() {", depth) + Repeat("}", depth), Inside, Past),
            ("template substitutions", depth => "var a = " + Repeat("`${", depth) + "1" + Repeat("}`", depth) + ";", Inside, Past),
            ("a binary chain", depth => "var a = 1; a" + Repeat(" + a", depth) + ";", SpineInside, SpinePast),
            ("a member chain", depth => "var a = {}; a" + Repeat(".b", depth) + ";", SpineInside, SpinePast),
            ("a call chain", depth => "function f() { return f; } f" + Repeat("()", depth) + ";", SpineInside, SpinePast),
        ];

        var failed = new List<string>();

        foreach (var (shape, source, inside, past) in corpus)
        {
            var within = Compile(source(inside), SliceGoal.Script);
            var beyond = Compile(source(past), SliceGoal.Script);

            if (!within.Accepted)
            {
                failed.Add($"{shape} {inside.ToString(CultureInfo.InvariantCulture)} deep: {within}");
            }

            if (beyond.Accepted || beyond.First != SliceSourceDiagnosticCode.NestingTooDeep)
            {
                failed.Add($"{shape} {past.ToString(CultureInfo.InvariantCulture)} deep: {beyond}");
            }
        }

        return (
            Name,
            failed.Count == 0,
            failed.Count == 0
                ? $"{corpus.Length} shapes through JsCompiler.Compile, each compiled inside its bound and refused " +
                  "as NestingTooDeep past it, with the process alive: brackets, parentheses, blocks, functions " +
                  $"and template substitutions {Past.ToString(CultureInfo.InvariantCulture)} deep, and a binary, " +
                  $"a member and a call chain {SpinePast.ToString(CultureInfo.InvariantCulture)} long"
                : string.Join("; ", failed));
    }

    /// <summary>One text through the wide path under one goal, as an embedder compiles it.</summary>
    private static Answer Compile(string text, SliceGoal goal)
    {
        var compiled = goal == SliceGoal.Module
            ? JsCompiler.Compile(
                [],
                [new JsModuleUnit("mem:/main.mjs", text, SliceParseOptions.Module, [])],
                new JsCompileRequest())
            : JsCompiler.Compile(
                [new JsScriptUnit("main", text, SliceParseOptions.Script)],
                [],
                new JsCompileRequest());

        return compiled.Succeeded && compiled.Artifact is { } artifact
            ? new Answer(true, Convert.ToHexStringLower(SHA256.HashData(artifact)), null)
            : new Answer(false, string.Empty, compiled.Diagnostics.Count == 0 ? null : compiled.Diagnostics[0].Code);
    }

    /// <summary>A text preceded by declarations that lengthen its compilation and change no answer.</summary>
    private static string Padded(string text)
    {
        var builder = new StringBuilder();

        for (var index = 0; index < 400; index++)
        {
            builder.Append("var p").Append(index.ToString(CultureInfo.InvariantCulture)).Append(" = ")
                .Append(index.ToString(CultureInfo.InvariantCulture)).Append(" + 1;\n");
        }

        return builder.Append(text).Append('\n').ToString();
    }

    private static string Repeat(string text, int count) =>
        new StringBuilder(text.Length * count).Insert(0, text, count).ToString();
}
