// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Compiler;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// Structured templates (SJL-12.6 item 4, JSD-0080 decision D15): the tokenizer's scan of a template
/// is one loop with a stack of its own.
/// </summary>
/// <remarks>
/// <b>The scan called itself once per level of nesting</b>, so a template nested a hundred thousand
/// deep ended the process in the tokenizer both front ends read. A run that reaches the row's
/// assertion at all is a run in which the scan ended, as the slice front end's deep-nesting row says
/// of itself.
/// </remarks>
internal static class TemplateChecks
{
    /// <summary>Runs every row.</summary>
    internal static IEnumerable<(string Name, bool Passed, string Detail)> Run()
    {
        yield return TheSliceFrontEndScansADeepTemplateToItsEnd();
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
}
