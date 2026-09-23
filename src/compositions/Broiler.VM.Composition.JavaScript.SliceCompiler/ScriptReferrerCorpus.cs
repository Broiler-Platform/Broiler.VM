// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The retained entries of the script-referrers section (JSeal I12-upstream, JSD-0024 section 20):
/// one per clause of the verifier's link rules, and two that verify.
/// </summary>
/// <remarks>
/// <para>
/// <b>One code and several clauses, and the rows are what tell the clauses apart.</b> Every
/// structural disagreement is <see cref="JavaScriptDiagnosticCodes.MalformedScriptReferrers"/>, and
/// each malformed row below breaks exactly one clause of an otherwise sound row, so a verifier that
/// refused every row would satisfy them all - which is why two of the entries verify and run.
/// </para>
/// <para>
/// <b>The malformed rows are written by hand</b>, because the lowering writes sound rows. The sound one
/// they vary is a one-unit script body, <c>0 LoadConstant 1; 3 Return</c>, placed at the interned
/// name <c>page.js</c>: it completes with the Number the constant holds.
/// </para>
/// </remarks>
internal static class ScriptReferrerCorpus
{
    /// <summary>Every script-referrers entry, the malformed rows first.</summary>
    internal static CorpusEntry[] Build() =>
    [
        // A ROW FOR A UNIT THE TABLE DOES NOT HAVE: the artifact has one code unit and the row names
        // the second.
        Entry("script-referrers-a-row-naming-no-unit", Artifact(unit: 1)),

        // A REFERRER THAT IS NOT A NAME: the row spells it with the Number constant.
        Entry("script-referrers-a-referrer-spelled-with-a-number", Artifact(referrer: 1)),

        // THE EMPTY REFERRER, which places nothing and is said by writing no row.
        Entry("script-referrers-an-empty-referrer", Artifact(referrer: 2)),

        // A ROW FOR A UNIT THAT IS NOT A SCRIPT BODY: an ordinary function, reached by a second,
        // sound program body.
        Entry(
            "script-referrers-a-row-for-a-function",
            Artifact(flags: JsFormat.FunctionFlags.None, entries: false)),

        // THE SAME UNIT NAMED TWICE, which would be two places for one script.
        Entry("script-referrers-a-unit-named-twice", Artifact(twice: true)),

        // ---- and two that verify and run ----------------------------------------------------------
        //
        // THE HAND-WRITTEN SOUND ROW, run: the body completes with 1.
        new CorpusEntry(
            "script-referrers-a-sound-row-that-verifies",
            WideCorpus.Mode,
            "Normal",
            "NormalCompleted",
            0,
            "1",
            "-",
            "-",
            "-",
            Artifact()),

        // A ROW THE LOWERING WROTE, for a script a host placed at `retained.js`.
        new CorpusEntry(
            "script-referrers-a-body-the-lowering-wrote",
            WideCorpus.Mode,
            "Normal",
            "NormalCompleted",
            0,
            "3",
            "-",
            "-",
            "-",
            Compiled("var placed = 1;\nplaced + 2;\n")),
    ];

    /// <summary>One malformed script-referrers entry, replayed under the wide mode.</summary>
    private static CorpusEntry Entry(string name, byte[] bytes) =>
        new(
            name,
            WideCorpus.Mode,
            "InvalidArtifact",
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedScriptReferrers,
            "-",
            "-",
            "-",
            "-",
            bytes);

    /// <summary>A program the lowering wrote, its script-referrers row included.</summary>
    private static byte[] Compiled(string source)
    {
        var compiled = Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.Compile(
            [
                new Broiler.VM.Profile.JavaScript.Compiler.JsScriptUnit(
                    "main",
                    source,
                    Broiler.VM.Profile.JavaScript.Compiler.SliceParseOptions.Script,
                    Referrer: "retained.js"),
            ]);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            throw new System.InvalidOperationException(
                "the retained script-referrers control did not compile: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString()));
        }

        return compiled.Artifact;
    }

    /// <summary>The sound one-unit artifact, with exactly one thing about it changed.</summary>
    private static byte[] Artifact(
        uint unit = 0,
        uint referrer = 0,
        JsFormat.FunctionFlags flags = JsFormat.FunctionFlags.ProgramBody,
        bool entries = true,
        bool twice = false)
    {
        byte[] body =
        [
            (byte)JsOpcode.LoadConstant, 0x01, 0x00,
            (byte)JsOpcode.Return,
        ];

        // A FUNCTION UNIT IS REACHED BY A SECOND, SOUND PROGRAM BODY, so the only thing wrong with the
        // artifact is the row: a function no entry names would be unreachable code instead.
        byte[] code = entries
            ? body
            : [.. body, (byte)JsOpcode.LoadUndefined, (byte)JsOpcode.Return];

        JsFunctionRow[] functions = entries
            ? [new JsFunctionRow(0, 0, 1, 16, 0, (uint)body.Length, (uint)flags)]
            :
            [
                new JsFunctionRow(0, 0, 1, 16, 0, (uint)body.Length, (uint)flags),
                new JsFunctionRow(
                    0, 0, 1, 16, (uint)body.Length, 2, (uint)JsFormat.FunctionFlags.ProgramBody),
            ];

        (uint, uint) row = (unit, referrer);

        var sections = new System.Collections.Generic.List<JavaScriptArtifactWriter.Section>
        {
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Limits,
                JsArtifactWriter.Limits(16, 16, 4, 4)),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Constants,
                JsArtifactWriter.Constants(
                [
                    JsArtifactWriter.InternedNameConstant("page.js"),
                    JsArtifactWriter.NumberConstant(1),
                    JsArtifactWriter.InternedNameConstant(string.Empty),
                ])),
            new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Code, code),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Entries,
                JsArtifactWriter.Entries([("main", entries ? 0u : 1u)])),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Positions,
                JsArtifactWriter.Positions([(0, 1, 1)])),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Functions,
                JsArtifactWriter.Functions(functions)),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.ScriptReferrers,
                JsArtifactWriter.ScriptReferrers(twice ? [row, row] : [row])),
        };

        return JsArtifactWriter.Write(JsFormat.ManifestId, sections.ToArray());
    }
}
