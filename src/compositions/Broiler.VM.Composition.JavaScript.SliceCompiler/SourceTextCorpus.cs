// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The retained entries of the source-text section (phase F3, the proposed JSD-0037): one per clause
/// of the verifier's link rules, and two that verify.
/// </summary>
/// <remarks>
/// <para>
/// <b>One code and several clauses, and the rows are what tell the clauses apart.</b> Every
/// structural disagreement is <see cref="JavaScriptDiagnosticCodes.MalformedSourceText"/>, and each
/// malformed row below breaks exactly one clause of an otherwise sound row, so a verifier that
/// refused every row would satisfy them all - which is why two of the entries verify and run.
/// </para>
/// <para>
/// <b>The malformed rows are written by hand</b>, because the lowering writes sound rows. The sound
/// one they vary is a one-unit script body, <c>0 LoadConstant 1; 3 Return</c>, whose row spans the
/// three characters <c>1;\n</c> of the String constant <c>"1;\n"</c>: it completes with the Number
/// the second constant holds.
/// </para>
/// </remarks>
internal static class SourceTextCorpus
{
    /// <summary>Every source-text entry, the malformed rows first.</summary>
    internal static CorpusEntry[] Build() =>
    [
        // A SPAN THAT RUNS PAST ITS TEXT: three characters from the second, of a text of three.
        Entry("source-text-a-span-past-its-text", Artifact(start: 1)),

        // A ROW FOR A UNIT THE TABLE DOES NOT HAVE: the artifact has one code unit and the row names
        // the second.
        Entry("source-text-a-row-naming-no-unit", Artifact(unit: 1)),

        // A TEXT THAT IS NOT A STRING: the row spells it with the Number constant.
        Entry("source-text-a-text-spelled-with-a-number", Artifact(text: 1)),

        // THE EMPTY SPAN, which is said by writing no row.
        Entry("source-text-an-empty-span", Artifact(length: 0)),

        // THE SAME UNIT NAMED TWICE, which would be two texts for one function.
        Entry("source-text-a-unit-named-twice", Artifact(twice: true)),

        // ---- and two that verify and run ----------------------------------------------------------
        //
        // THE HAND-WRITTEN SOUND ROW, run: the body completes with 1.
        new CorpusEntry(
            "source-text-a-sound-row-that-verifies",
            WideCorpus.Mode,
            "Normal",
            "NormalCompleted",
            0,
            "1",
            "-",
            "-",
            "-",
            Artifact()),

        // A ROW THE LOWERING WROTE, read back: `function f(a) { return a; }` is 27 characters.
        new CorpusEntry(
            "source-text-a-function-the-lowering-wrote",
            WideCorpus.Mode,
            "Normal",
            "NormalCompleted",
            0,
            "27",
            "-",
            "-",
            "-",
            Compiled("function f(a) { return a; }\nf.toString().length;\n")),
    ];

    /// <summary>One malformed source-text entry, replayed under the wide mode.</summary>
    private static CorpusEntry Entry(string name, byte[] bytes) =>
        new(
            name,
            WideCorpus.Mode,
            "InvalidArtifact",
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedSourceText,
            "-",
            "-",
            "-",
            "-",
            bytes);

    /// <summary>A program the lowering wrote, its source-text rows included.</summary>
    private static byte[] Compiled(string source)
    {
        var compiled = Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.Compile(
            [
                new Broiler.VM.Profile.JavaScript.Compiler.JsScriptUnit(
                    "main",
                    source,
                    Broiler.VM.Profile.JavaScript.Compiler.SliceParseOptions.Script),
            ]);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            throw new System.InvalidOperationException(
                "the retained source-text control did not compile: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString()));
        }

        return compiled.Artifact;
    }

    /// <summary>The sound one-unit artifact, with exactly one thing about it changed.</summary>
    private static byte[] Artifact(
        uint unit = 0,
        uint text = 0,
        uint start = 0,
        uint length = 3,
        bool twice = false)
    {
        byte[] body =
        [
            (byte)JsOpcode.LoadConstant, 0x01, 0x00,
            (byte)JsOpcode.Return,
        ];

        (uint, uint, uint, uint) row = (unit, text, start, length);

        var sections = new System.Collections.Generic.List<JavaScriptArtifactWriter.Section>
        {
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Limits,
                JsArtifactWriter.Limits(16, 16, 4, 4)),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Constants,
                JsArtifactWriter.Constants(
                [
                    JsArtifactWriter.StringConstant("1;\n"),
                    JsArtifactWriter.NumberConstant(1),
                ])),
            new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Code, body),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Entries,
                JsArtifactWriter.Entries([("main", 0u)])),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Positions,
                JsArtifactWriter.Positions([(0, 1, 1)])),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Functions,
                JsArtifactWriter.Functions(
                    [new JsFunctionRow(0, 0, 1, 16, 0, (uint)body.Length, (uint)JsFormat.FunctionFlags.ProgramBody)])),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.SourceText,
                JsArtifactWriter.SourceText(twice ? [row, row] : [row])),
        };

        return JsArtifactWriter.Write(JsFormat.ManifestId, sections.ToArray());
    }
}
