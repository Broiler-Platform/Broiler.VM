// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Compiler;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// Structured templates (SJL-12.6 item 4, JSD-0080 decisions D15 and D17): a template's substitutions
/// are read by the parse that reads the template, and the tokenizer's scan of a template is one loop.
/// </summary>
/// <remarks>
/// <para>
/// <b>Each correction JSD-0080 lists has a row, and each row is one a run of the base could not
/// pass.</b> The wide parser parsed a substitution with a parser of its own, which started at depth
/// zero, at tree depth zero and outside any parameter list: so <c>${yield}</c> and <c>${await 1}</c>
/// compiled in formal parameters, nesting templates did not count toward the nesting bound, and a
/// substitution's tree did not count toward the tree-depth bound. Templates nested a few thousand deep
/// ended the process in that parser, and a hundred thousand deep in the tokenizer's scan, which both
/// front ends read.
/// </para>
/// <para>
/// <b>Where the base ended the process, a run that reaches the row's assertion at all is a run in
/// which a bound answered</b>, as the slice front end's deep-nesting row says of itself. Both halves
/// of a bound are asked where there is a bound: a nesting at it compiles, and one past it is refused.
/// </para>
/// </remarks>
internal static class TemplateChecks
{
    /// <summary>Runs every row.</summary>
    internal static IEnumerable<(string Name, bool Passed, string Detail)> Run()
    {
        yield return TheSliceFrontEndScansADeepTemplateToItsEnd();

        // `${yield}` and `${await …}` in formal parameters are refused with 2101, as the same expressions
        // are outside a template; each was compiled before, into a unit that may not suspend
        yield return Refused(
            "template: a yield in a substitution in a generator's parameters is refused",
            "function* g(a = `${yield}`) {}",
            "2101@1:20");
        yield return Refused(
            "template: an await in a substitution in an async function's parameters is refused",
            "async function f(a = `${await 1}`) {}",
            "2101@1:25");
        yield return Refused(
            "template: a yield in a substitution in an arrow's parameters inside a generator is refused",
            "function* g() { (a = `${yield}`) => a; }",
            "2101@1:25");

        // and where the substitution is not in parameters, or a function boundary is between, nothing moves
        yield return Accepted(
            "template: a yield in a substitution in a generator's body is a yield",
            "function* g() { yield `${yield 1}`; }");
        yield return Accepted(
            "template: an await in an async arrow's body, in a parameter's default, is the arrow's",
            "async function f(a = async () => `${await 1}`) {}");

        yield return NestingCountsTowardTheBound();
        yield return ASubstitutionsTreeCountsTowardTheTreeDepthBound();

        // the places a substitution's refusals name are where they stand in the source: the parser's,
        // from a run tokenized in place, and the lexer's, reported when the parse reaches the substitution
        yield return Refused(
            "template: a refusal in a substitution names its own line and column",
            "var x = `a\n${\n  y z\n}`;",
            "2101@3:5");
        yield return Refused(
            "template: a lexer refusal in a substitution is reported where the parse reaches it",
            "var x = `${ 08_1 }` +; var y = ;",
            "2003@1:13 2101@1:22 2102@1:24");
    }

    /// <summary>
    /// Templates nested to the nesting bound compile, and nested past it are refused with 2103 - a
    /// hundred thousand deep included, which ended the process in the wide parser before.
    /// </summary>
    private static (string, bool, string) NestingCountsTowardTheBound()
    {
        // `var x = ` and its initialiser take two levels of the default 64, and each template one more: 62
        // templates is the deepest the bound admits, and the refusal names the 63rd's substitution
        var atTheBound = JsCompiler.Compile("var x = " + Nest(62) + ";", SliceParseOptions.Script);
        var past = JsCompiler.Compile("var x = " + Nest(63) + ";", SliceParseOptions.Script);
        var farPast = JsCompiler.Compile("var x = " + Nest(100_000) + ";", SliceParseOptions.Script);

        var passed = atTheBound.Succeeded &&
            Answer(past) == "2103@1:198" && !past.Succeeded &&
            Answer(farPast) == "2103@1:198" && !farPast.Succeeded;

        return (
            "template: nesting counts toward the nesting bound, and past it is refused with 2103",
            passed,
            $"62 deep {(atTheBound.Succeeded ? "compiled" : "refused `" + Answer(atTheBound) + "`")}; 63 deep " +
                $"`{Answer(past)}`; 100,000 deep `{Answer(farPast)}` (expected 2103@1:198 for both)");
    }

    /// <summary>
    /// A substitution's tree counts toward the tree-depth bound with the tree around it: two chains of
    /// 6,000 additions, one in a substitution that is the other's last operand, are refused, where two of
    /// 4,000 compile.
    /// </summary>
    /// <remarks>
    /// Each half is within the bound alone, which is all the base's sub-parse saw, so it compiled a tree
    /// deeper than the bound every walk of the compiler is derived against (JSD-0080 decision D17).
    /// </remarks>
    private static (string, bool, string) ASubstitutionsTreeCountsTowardTheTreeDepthBound()
    {
        var within = JsCompiler.Compile(
            "var x = " + Chain(4_000) + " + `${" + Chain(4_000) + "}`;", SliceParseOptions.Script);
        var past = JsCompiler.Compile(
            "var x = " + Chain(6_000) + " + `${" + Chain(6_000) + "}`;", SliceParseOptions.Script);

        return (
            "template: a substitution's tree counts toward the tree-depth bound",
            within.Succeeded && !past.Succeeded && Answer(past) == "2103@1:12016",
            $"4,000 and 4,000 {(within.Succeeded ? "compiled" : "refused `" + Answer(within) + "`")}; " +
                $"6,000 and 6,000 `{Answer(past)}` (expected 2103@1:12016)");
    }

    /// <summary>
    /// The slice front end refuses a template nested a hundred thousand deep by name, and its census
    /// reads it, because the tokenizer's scan of a template is a loop with a stack of its own.
    /// </summary>
    /// <remarks>
    /// The slice manifest admits no template, so the answer is the one any template gets; what is new
    /// is that there is an answer. The scan called itself once per level and ended the process here
    /// (JSD-0080 decision D15, a listed slice correction: only a crash becomes a refusal).
    /// </remarks>
    private static (string, bool, string) TheSliceFrontEndScansADeepTemplateToItsEnd()
    {
        var source = "var x = " + Nest(100_000) + ";";
        var compiled = SliceSourceCompiler.Compile(source);
        var census = SliceConstructCensus.Take([source]);
        var answer = string.Join(" ", compiled.Diagnostics.Select(d => (int)d.Code + "@" + d.Line + ":" + d.Column));

        return (
            "template: the slice front end scans a template nested 100,000 deep to its end",
            !compiled.Succeeded && answer == "2104@1:9" && census.FilesRead == 1 && census.FilesThatFaulted == 0,
            $"refused `{answer}` (expected 2104@1:9, a template outside the manifest); census read " +
                $"{census.FilesRead} file, {census.FilesThatFaulted} faulted");
    }

    /// <summary><paramref name="depth"/> templates, each the whole substitution of the one around it.</summary>
    private static string Nest(int depth) =>
        string.Concat(Enumerable.Repeat("`${", depth)) + "1" + string.Concat(Enumerable.Repeat("}`", depth));

    /// <summary>A left-associative chain of <paramref name="additions"/> additions of 1.</summary>
    private static string Chain(int additions) => "1" + string.Concat(Enumerable.Repeat("+1", additions));

    /// <summary>The diagnostics of a compilation, each <c>code@line:column</c>, in the order reported.</summary>
    private static string Answer(JsCompilation compiled) =>
        string.Join(" ", compiled.Diagnostics.Select(d => (int)d.Code + "@" + d.Line + ":" + d.Column));

    /// <summary>One row: the script compiles to bytes with no diagnostic at all.</summary>
    private static (string, bool, string) Accepted(string name, string source)
    {
        var compiled = JsCompiler.Compile(source, SliceParseOptions.Script);

        return (
            name,
            compiled.Succeeded && compiled.Artifact is not null && compiled.Diagnostics.Count == 0,
            $"expected the compilation to produce bytes with no diagnostic, and it reported `{Answer(compiled)}`");
    }

    /// <summary>
    /// One row: the script is refused with exactly these diagnostics, each <c>code@line:column</c> in the
    /// order reported, and no bytes.
    /// </summary>
    private static (string, bool, string) Refused(string name, string source, string expected)
    {
        var compiled = JsCompiler.Compile(source, SliceParseOptions.Script);
        var answer = Answer(compiled);

        if (compiled.Succeeded || compiled.Artifact is not null)
        {
            return (name, false, $"expected `{expected}` and the compilation produced bytes (`{answer}`)");
        }

        return (
            name,
            string.Equals(answer, expected, System.StringComparison.Ordinal),
            $"expected `{expected}` and the compilation reported `{answer}`");
    }
}
