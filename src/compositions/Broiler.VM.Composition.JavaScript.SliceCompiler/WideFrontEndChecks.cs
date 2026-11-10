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
        yield return ALeftOperandsSpineIsChargedToTheChainItBegins();
    }

    /// <summary>How many rounds the two-goals row runs, each a Script and a Module compilation at once.</summary>
    private const int Rounds = 200;

    /// <summary>
    /// The sources the two-goals row compiles under both goals, each accepted under one goal and refused
    /// under the other.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Each source's answer depends on its goal and on nothing else, and the parse decides it.</b>
    /// The first is a module declaration, which a module accepts and a script refuses; the second and
    /// third are accepted as a script and refused as a module, a module being strict and <c>await</c>
    /// reserved in it. So a compilation parsed under the other goal's request shows, in either
    /// direction, as an artifact where a refusal was due or a refusal where an artifact was, and an
    /// artifact's hash is still compared with the one its own goal gives alone. The filler declarations
    /// make each compilation long enough that two of them overlap on a machine with more than one core.
    /// </para>
    /// <para>
    /// <b>No source is accepted under both goals, because two such artifacts would differ whatever the
    /// parse did</b>: a script and a module are lowered as different kinds of unit under different names,
    /// so a check that their hashes differ could not fail, and a mix-up of the goals' requests that left
    /// the parse unchanged would not show.
    /// </para>
    /// </remarks>
    private static readonly (string Name, string Text, SliceSourceDiagnosticCode? ScriptRefusal, SliceSourceDiagnosticCode? ModuleRefusal)[] GoalSources =
    [
        ("an export declaration", Padded("export var e = 1; e;"), SliceSourceDiagnosticCode.ModuleDeclarationOutsideModuleGoal, null),
        ("a legacy octal literal", Padded("var y = 0123; y;"), null, SliceSourceDiagnosticCode.LegacyOctalInStrictCode),
        ("await as a binding name", Padded("var await = 1; await;"), null, SliceSourceDiagnosticCode.ReservedWordAsBinding),
    ];

    /// <summary>
    /// One answer of the wide path: an artifact's hash, the first refusal's code, or the exception that
    /// left the compiler.
    /// </summary>
    private readonly record struct Answer(bool Accepted, string Hash, SliceSourceDiagnosticCode? First, string? Threw = null)
    {
        public override string ToString() =>
            Threw is not null
                ? "threw " + Threw
                : Accepted
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
    /// <b>The expected answers are checked before they are used</b>: each source must be accepted under
    /// the goal that admits it and refused under the other with the code the language's rule gives. A
    /// pair of goals that answered alike would make a mix-up invisible. An exception that leaves the
    /// compiler during the rounds is an answer the row reports, not one that stops the checks.
    /// </para>
    /// </remarks>
    private static (string, bool, string) TwoGoalsOnTheWidePathDoNotReachEachOther()
    {
        const string Name = "two goals on the wide path in one process do not reach each other";
        var failed = new List<string>();
        var expected = new (Answer Script, Answer Module)[GoalSources.Length];

        for (var index = 0; index < GoalSources.Length; index++)
        {
            var (name, text, scriptRefusal, moduleRefusal) = GoalSources[index];
            expected[index] = (Compile(text, SliceGoal.Script), Compile(text, SliceGoal.Module));

            foreach (var (goal, answer, refusal) in new[]
            {
                ("script", expected[index].Script, scriptRefusal),
                ("module", expected[index].Module, moduleRefusal),
            })
            {
                if (refusal is null
                        ? !answer.Accepted
                        : answer.Accepted || answer.Threw is not null || answer.First != refusal)
                {
                    failed.Add(
                        $"{name}: as a {goal}, alone, answered {answer}, where " +
                        (refusal is null ? "an artifact" : "a refusal with " + refusal) + " was due");
                }
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
                  "JsCompiler.Compile, each answering as its goal answers alone: one text a refusal as a " +
                  "script and an artifact as a module, two texts an artifact as a script and a refusal as a module"
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

    /// <summary>
    /// A chain whose left operand's spine the base left uncharged is refused past the tree-depth bound,
    /// in each of the four shapes the correspondence's row R14 names and in each residual shape
    /// JSD-0083's note on decision D14 lists, and a source within the bound that the base refused is
    /// compiled (SJL-13's step 1, JSD-0083 decisions D14 and D23).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Added 2026-11-09 by SJL-13's step 1, and judged at the after commit only</b> (bundle
    /// <c>sjl-13-001</c>'s part C, its fifth row, as the rule's revision of 2026-11-02 reads it). The base
    /// compiled the first four shapes past the bound, because <c>JsParser.Deepen</c> charged a chain one
    /// level per node it added and did not add a left operand's own spine to the chain it began; the row
    /// the tools commit added holds none of them for that reason (bundle <c>sjl-12-001</c>'s README, "The
    /// parser's left-operand gap"). The shared library charges a chain node's height, one more than its
    /// highest child's, wherever the tree is built, so each shape past the bound is refused with the
    /// contract's tree-depth diagnostic, the process alive.
    /// </para>
    /// <para>
    /// <b>The four shapes</b>: a binary chain whose first operand is a parenthesised chain, a template whose
    /// substitution holds one, a binary chain whose first operand is a member or call chain, and a member
    /// or call chain whose object is a parenthesised binary chain. <b>The residual shapes</b>, one source
    /// each: a chain as the right operand of a chain's early node, and as a prefix, <c>await</c>,
    /// <c>yield</c> or update operand, an array element, an object value, a computed key, a call's,
    /// <c>new</c>'s or <c>import()</c>'s argument, a substitution, a parenthesised sequence, conditional or
    /// assignment, a function body, a class heritage, a field initialiser, and <c>#x in</c>'s right
    /// operand. A decorator's expression is the one residual shape left out: this family admits no
    /// decorator, so the source would be refused for its decorator before its depth. <b>Within the
    /// bound</b>: a chain of additions whose last right operand is a chain of multiplications, the two
    /// together past the bound and each within it, which <c>JsParser</c>'s <c>Continue</c> refused by
    /// summing the depth the chain had reached and the operand's, and which the library compiles.
    /// </para>
    /// </remarks>
    private static (string, bool, string) ALeftOperandsSpineIsChargedToTheChainItBegins()
    {
        const string Name = "a left operand's spine is charged to the chain it begins";
        const int Half = SliceParseOptions.MaximumTreeDepth * 6 / 10;
        const int Past = SliceParseOptions.MaximumTreeDepth * 12 / 10;
        const int Within = SliceParseOptions.MaximumTreeDepth * 3 / 4;

        string Chain(int length, string op = " + ") => "a" + Repeat(op + "a", length);
        var deep = Chain(Past);

        (string Shape, string Source)[] past =
        [
            ("a binary chain whose first operand is a parenthesised chain", "var a = 1; (" + Chain(Half) + ")" + Repeat(" + a", Half) + ";"),
            ("a template whose substitution holds one", "var a = 1; `${(" + Chain(Half) + ")" + Repeat(" + a", Half) + "}`;"),
            ("a binary chain whose first operand is a member chain", "var a = { b: 1 }; a" + Repeat(".b", Half) + Repeat(" + a", Half) + ";"),
            ("a binary chain whose first operand is a call chain", "function a() { return a; } a" + Repeat("()", Half) + Repeat(" + a", Half) + ";"),
            ("a member chain whose object is a parenthesised binary chain", "var a = 1; (" + Chain(Half) + ")" + Repeat(".b", Half) + ";"),
            ("a call chain whose object is a parenthesised binary chain", "var a = 1; (" + Chain(Half) + ")" + Repeat("()", Half) + ";"),
            ("the right operand of a chain's early node", "var a = 1; a + (" + deep + ") + a;"),
            ("a prefix operand", "var a = 1; !(" + deep + ");"),
            ("an await operand", "async function f() { var a = 1; await (" + deep + "); }"),
            ("a yield operand", "function* f() { var a = 1; yield (" + deep + "); }"),
            ("an update operand", "var a = { b: 1 }; ++(" + deep + ").b;"),
            ("an array element", "var a = 1; [" + deep + "];"),
            ("an object value", "var a = 1; ({ k: " + deep + " });"),
            ("a computed key", "var a = 1; ({ [" + deep + "]: 1 });"),
            ("a call's argument", "function a() {} a(" + deep + ");"),
            ("new's argument", "function a() {} new a(" + deep + ");"),
            ("import()'s argument", "var a = 1; import(" + deep + ");"),
            ("a substitution", "var a = 1; `${" + deep + "}`;"),
            ("a parenthesised sequence", "var a = 1; (a, " + deep + ");"),
            ("a parenthesised conditional", "var a = 1; (a ? " + deep + " : a);"),
            ("a parenthesised assignment", "var a = 1; (a = " + deep + ");"),
            ("a function body", "function f() { var a = 1; return " + deep + "; }"),
            ("a class heritage", "var a = 1; class C extends (" + deep + ") {}"),
            ("a field initialiser", "var a = 1; class C { x = " + deep + "; }"),
            ("#x in's right operand", "var a = 1; class C { #x; m() { return #x in (" + deep + "); } }"),
        ];

        var failed = new List<string>();

        foreach (var (shape, source) in past)
        {
            var answer = Compile(source, SliceGoal.Script);

            if (answer.Accepted || answer.First != SliceSourceDiagnosticCode.NestingTooDeep)
            {
                failed.Add($"{shape}: {answer}");
            }
        }

        var within = Compile("var a = 1; " + Chain(Within) + " + " + Chain(Within, " * ") + ";", SliceGoal.Script);

        if (!within.Accepted)
        {
            failed.Add($"within the bound, {Within.ToString(CultureInfo.InvariantCulture)} additions and " +
                $"{Within.ToString(CultureInfo.InvariantCulture)} multiplications: {within}");
        }

        return (
            Name,
            failed.Count == 0,
            failed.Count == 0
                ? $"{past.Length} sources past the tree-depth bound of " +
                  $"{SliceParseOptions.MaximumTreeDepth.ToString(CultureInfo.InvariantCulture)} through JsCompiler.Compile, " +
                  "the four shapes the base compiled and one of each residual shape, each refused as NestingTooDeep " +
                  "with the process alive; and a chain within the bound that the base refused, compiled"
                : string.Join("; ", failed));
    }

    /// <summary>
    /// One text through the wide path under one goal, as an embedder compiles it; an exception that
    /// leaves the compiler is the answer rather than the end of the checks.
    /// </summary>
    private static Answer Compile(string text, SliceGoal goal)
    {
        JsCompilation compiled;

        try
        {
            compiled = goal == SliceGoal.Module
                ? JsCompiler.Compile(
                    [],
                    [new JsModuleUnit("mem:/main.mjs", text, SliceParseOptions.Module, [])],
                    new JsCompileRequest())
                : JsCompiler.Compile(
                    [new JsScriptUnit("main", text, SliceParseOptions.Script)],
                    [],
                    new JsCompileRequest());
        }
        catch (Exception failure) when (failure is not OutOfMemoryException)
        {
            return new Answer(false, string.Empty, null, failure.GetType().Name);
        }

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
