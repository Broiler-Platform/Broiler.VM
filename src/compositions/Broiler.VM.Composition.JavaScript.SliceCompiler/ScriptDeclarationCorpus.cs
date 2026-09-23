// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The retained entries of the script-declarations section (JSeal V15-host): one per clause of the
/// verifier's link rules, and three that verify.
/// </summary>
/// <remarks>
/// <para>
/// <b>One code and several clauses, and the rows are what tell the clauses apart.</b> Every
/// structural disagreement is <see cref="JavaScriptDiagnosticCodes.MalformedScriptDeclarations"/>, and
/// each malformed row below breaks exactly one clause of an otherwise sound row, so a verifier that
/// refused every row would satisfy them all - which is why three of the entries verify and run.
/// </para>
/// <para>
/// <b>The malformed rows are written by hand</b>, because the lowering writes sound rows. The sound one
/// they vary is a one-unit script body, <c>0 DeclareGlobal "x"; 3 LoadConstant 1; 6 Return</c>, whose
/// row declares <c>x</c> as a <c>var</c>: it completes with the Number the constant holds.
/// </para>
/// </remarks>
internal static class ScriptDeclarationCorpus
{
    /// <summary>Every script-declarations entry, the malformed rows first.</summary>
    internal static CorpusEntry[] Build() =>
    [
        // A ROW FOR A UNIT THE TABLE DOES NOT HAVE: the artifact has one code unit and the row names
        // the second.
        Entry("script-declarations-a-row-naming-no-unit", Artifact(unit: 1)),

        // A NAME THAT IS NOT A NAME: the row spells its `var` with the Number constant.
        Entry("script-declarations-a-name-spelled-with-a-number", Artifact(varName: 1)),

        // A ROW FOR A UNIT THAT IS NOT A SCRIPT BODY: the one unit is flagged as eval code too, which
        // the function table refuses first - so the unit here is an ordinary function instead.
        Entry(
            "script-declarations-a-row-for-a-function",
            Artifact(flags: JsFormat.FunctionFlags.None, entries: false)),

        // ANNEX B CANDIDATES ON A STRICT BODY, which hoists none.
        Entry(
            "script-declarations-annex-b-on-a-strict-body",
            Artifact(flags: JsFormat.FunctionFlags.ProgramBody | JsFormat.FunctionFlags.Strict, annexB: true)),

        // THE SAME UNIT NAMED TWICE, which would be two instantiations of one body.
        Entry("script-declarations-a-unit-named-twice", Artifact(twice: true)),

        // ---- and three that verify and run --------------------------------------------------------
        //
        // THE HAND-WRITTEN SOUND ROW, run: the body declares its `var` and completes with 1.
        new CorpusEntry(
            "script-declarations-a-sound-row-that-verifies",
            WideCorpus.Mode,
            "Normal",
            "NormalCompleted",
            0,
            "1",
            "-",
            "-",
            "-",
            Artifact()),

        // A ROW THE LOWERING WROTE, whose checks pass: a function, a `var` and a `let`.
        new CorpusEntry(
            "script-declarations-a-body-the-lowering-wrote",
            WideCorpus.Mode,
            "Normal",
            "NormalCompleted",
            0,
            "6",
            "-",
            "-",
            "-",
            Compiled("function f() { return 1; }\nvar v = 2;\nlet l = 3;\nf() + v + l;\n")),

        // AND ONE WHOSE CHECK FAILS BEFORE ITS FIRST INSTRUCTION: `undefined` is a non-configurable
        // property of the global object, so a lexical declaration of it is the SyntaxError
        // `GlobalDeclarationInstantiation` throws - and the `var` before it was never created.
        new CorpusEntry(
            "script-declarations-a-lexical-over-a-restricted-global",
            WideCorpus.Mode,
            "Normal",
            "NormalCompleted",
            0,
            "uncaught:SyntaxError",
            "-",
            "-",
            "-",
            Compiled("var before = 1;\nlet undefined;\n")),
    ];

    /// <summary>One malformed script-declarations entry, replayed under the wide mode.</summary>
    private static CorpusEntry Entry(string name, byte[] bytes) =>
        new(
            name,
            WideCorpus.Mode,
            "InvalidArtifact",
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedScriptDeclarations,
            "-",
            "-",
            "-",
            "-",
            bytes);

    /// <summary>A program the lowering wrote, its script-declarations row included.</summary>
    private static byte[] Compiled(string source)
    {
        var compiled = Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.Compile(
            source, Broiler.VM.Profile.JavaScript.Compiler.SliceParseOptions.Script);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            throw new System.InvalidOperationException(
                "the retained script-declarations control did not compile: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString()));
        }

        return compiled.Artifact;
    }

    /// <summary>The sound one-unit artifact, with exactly one thing about it changed.</summary>
    private static byte[] Artifact(
        uint unit = 0,
        uint varName = 0,
        JsFormat.FunctionFlags flags = JsFormat.FunctionFlags.ProgramBody,
        bool entries = true,
        bool annexB = false,
        bool twice = false)
    {
        byte[] body =
        [
            (byte)JsOpcode.DeclareGlobal, 0x00, 0x00,
            (byte)JsOpcode.LoadConstant, 0x01, 0x00,
            (byte)JsOpcode.Return,
        ];

        var row = new JsScriptDeclarationRow(unit, [], [varName], [], annexB ? [0u] : []);

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

        var sections = new System.Collections.Generic.List<JavaScriptArtifactWriter.Section>
        {
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Limits,
                JsArtifactWriter.Limits(16, 16, 4, 4)),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Constants,
                JsArtifactWriter.Constants(
                [
                    JsArtifactWriter.InternedNameConstant("x"),
                    JsArtifactWriter.NumberConstant(1),
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
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.ScriptDeclarations,
                JsArtifactWriter.ScriptDeclarations(twice ? [row, row] : [row])),
        };

        return JsArtifactWriter.Write(JsFormat.ManifestId, sections.ToArray());
    }
}
