// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The retained entries of the eval scope map (JSeal V14, JSD-0026 step 2): one per clause of the
/// verifier's section 4 rules, and four that verify.
/// </summary>
/// <remarks>
/// <para>
/// <b>Two codes and many clauses, and the rows are what tell the clauses apart.</b> A map without
/// the dynamic surface is <see cref="JavaScriptDiagnosticCodes.EvalScopesOutsideManifest"/>; every
/// structural disagreement the bytes can show is
/// <see cref="JavaScriptDiagnosticCodes.MalformedEvalScopes"/>, and each malformed row below breaks
/// exactly one clause of an otherwise sound map, so a verifier that refused every map would satisfy
/// them all - which is why four of the rows verify and run.
/// </para>
/// <para>
/// <b>The malformed rows are written by hand</b>, because the lowering writes sound maps and a
/// corpus of refusals has to be able to say what an unsound one is. The sound one they vary is a
/// script body with one direct-eval site at its own entry record: a root program row, one site at
/// the <see cref="JsOpcode.CallEval"/> of <c>0 LoadGlobal Number; 3 LoadUndefined; 4 LoadConstant
/// "x"; 7 CallEval 1; 9 Return</c>, at depth zero, sloppy. Its callee is not the intrinsic, so it
/// runs as the ordinary call the identity check makes of it and needs no provider.
/// </para>
/// </remarks>
internal static class EvalScopeCorpus
{
    /// <summary>Every eval scope entry, the code-bearing rows first.</summary>
    internal static CorpusEntry[] Build() =>
    [
        // A MAP NO SURFACE DECLARES is a program built for evaluation presented to a composition that
        // was never asked whether it admits evaluation.
        Entry(
            "eval-scopes-a-map-no-surface-declares",
            Artifact(declareSurface: false),
            "UnknownFeature",
            JavaScriptDiagnosticCodes.EvalScopesOutsideManifest),

        // A SITE THAT IS NOT AN EVAL CALL: offset 3 holds a `LoadUndefined`, which is an instruction
        // boundary and not a direct evaluation.
        Entry(
            "eval-scopes-a-site-that-is-not-an-eval-call",
            Artifact(siteOffset: 3),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedEvalScopes),

        // A SITE INSIDE AN INSTRUCTION: offset 5 is the constant index of the `LoadConstant`.
        Entry(
            "eval-scopes-a-site-inside-an-instruction",
            Artifact(siteOffset: 5),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedEvalScopes),

        // A SITE WHOSE CHAIN DOES NOT REACH ITS UNIT'S ROOT AT ITS DEPTH: depth one from a root row.
        Entry(
            "eval-scopes-a-chain-past-its-root",
            Artifact(siteDepth: 1),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedEvalScopes),

        // A SITE WHOSE DEPTH IS NOT THE ABSTRACT PASS'S: a block row under the root and a declared
        // depth of one, in code that pushes no record. The chain is sound; the frame at the call
        // does not hold it, and only the abstract pass can say so.
        Entry(
            "eval-scopes-a-depth-the-code-does-not-push",
            Artifact(blockUnderRoot: true, siteDepth: 1),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedEvalScopes),

        // A NAME THAT IS NOT A NAME: the row spells a binding with the Number constant.
        Entry(
            "eval-scopes-a-binding-spelled-with-a-number",
            Artifact(blockUnderRoot: true, siteDepth: 1, pushScope: true, nameConstant: 1),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedEvalScopes),

        // A FUNCTION BODY'S ROW IN A PROGRAM BODY: the record a unit pushes for a body's own variable
        // environment is admitted only directly inside a function's own record (JSeal V15-finish),
        // and a script body's chain that claims one would make the variable environment of its
        // direct eval a block.
        Entry(
            "eval-scopes-a-function-body-row-in-a-program-body",
            Artifact(
                blockUnderRoot: true,
                siteDepth: 1,
                pushScope: true,
                innerKind: JsFormat.EvalScopeKind.FunctionBody),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedEvalScopes),

        // AN IMPORT OUTSIDE A MODULE'S ROW: a block row names a binding as an import entry (JSeal
        // V15-module), which only a module's own record can hold.
        Entry(
            "eval-scopes-an-import-outside-a-module-row",
            Artifact(
                blockUnderRoot: true,
                siteDepth: 1,
                pushScope: true,
                nameFlags: JsFormat.EvalBindingImport | JsFormat.EvalBindingImmutable),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedEvalScopes),

        // A MODULE'S ROW AT THE ROOT OF A SCRIPT BODY'S SITE: the chain of a program body that is no
        // module's ends at a program row, and one that claimed a module's record would read the
        // import table of a module the script is not (JSeal V15-module).
        Entry(
            "eval-scopes-a-module-row-under-a-script-body",
            Artifact(rootKind: JsFormat.EvalScopeKind.Module),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedEvalScopes),

        // A REQUEST FLAG THIS BUILD DOES NOT DEFINE, which no provider could be asked to honour.
        Entry(
            "eval-scopes-a-request-flag-nothing-defines",
            Artifact(siteFlags: 0x40),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedEvalScopes),

        // AN EVAL NAME INSTRUCTION IN AN ARTIFACT THAT IS NOT EVAL CODE, which could only ever reach
        // a record that no evaluation entered.
        Entry(
            "eval-scopes-an-eval-name-outside-eval-code",
            Artifact(code: [
                (byte)JsOpcode.LoadEvalName, 0x00, 0x00, 0x00,
                (byte)JsOpcode.Return,
            ], withSite: false),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedEvalScopes),

        // AN EVAL-CODE UNIT WITH NO DECLARATION ROW, whose answer the executor could not bind to any
        // request.
        Entry(
            "eval-scopes-eval-code-with-no-declaration",
            Artifact(
                code: [(byte)JsOpcode.LoadUndefined, (byte)JsOpcode.Return],
                withSite: false,
                flags: JsFormat.FunctionFlags.EvalCode),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedEvalScopes),

        // AN EVAL-CODE UNIT THAT IS ALSO A PROGRAM BODY, which is two entries at once.
        Entry(
            "eval-scopes-eval-code-that-is-a-program-body",
            Artifact(
                code: [(byte)JsOpcode.LoadUndefined, (byte)JsOpcode.Return],
                withSite: false,
                flags: JsFormat.FunctionFlags.EvalCode | JsFormat.FunctionFlags.ProgramBody),
            "InconsistentStructure",
            JavaScriptDiagnosticCodes.MalformedFunctionRow),

        // ---- and four that verify and run ---------------------------------------------------------
        //
        // THE HAND-WRITTEN SOUND MAP, run: the site's callee is the global `Number`, so the identity
        // check makes an ordinary call of it and the program completes with `Number("x")` - which a
        // verifier that refused every map could not produce, and which needs no provider.
        new CorpusEntry(
            "eval-scopes-a-sound-map-that-verifies",
            WideCorpus.Mode,
            "Normal",
            "NormalCompleted",
            0,
            "NaN",
            "-",
            "-",
            "-",
            Artifact()),

        // AND A MAP THE LOWERING WROTE: a function whose direct eval of a non-String answers it
        // unchanged, which is what `eval` does wherever it is called and needs no provider either.
        new CorpusEntry(
            "eval-scopes-a-function-site-the-lowering-wrote",
            WideCorpus.Mode,
            "Normal",
            "NormalCompleted",
            0,
            "42",
            "-",
            "-",
            "-",
            Compiled("function f(v) { let k = v; return eval(k); }\nf(42);\n")),

        // AND THE SAME ROUTE ASKED FOR CODE BY A COMPOSITION THAT REGISTERED NO PROVIDER: the site
        // has a row and the map grants nothing, so the evaluation is refused where every other
        // route's is, with the catchable EvalError the replay host records as uncaught.
        new CorpusEntry(
            "eval-scopes-a-site-no-provider-answers",
            WideCorpus.Mode,
            "Normal",
            "NormalCompleted",
            0,
            "uncaught:EvalError",
            "-",
            "-",
            "-",
            Compiled("function f() { let k = 1; return eval(\"k\"); }\nf();\n")),

        // AND A SITE IN A BODY WITH A VARIABLE ENVIRONMENT OF ITS OWN (JSeal V15-finish): the
        // parameter list makes a closure, so the lowering pushes the body's record and writes its
        // row; the body's `a` starts with the parameter's value and the closure still sees 1.
        new CorpusEntry(
            "eval-scopes-a-function-body-site-the-lowering-wrote",
            WideCorpus.Mode,
            "Normal",
            "NormalCompleted",
            0,
            "3",
            "-",
            "-",
            "-",
            Compiled("function f(a, g = () => a) { var a = 2; return eval(a) + g(); }\nf(1);\n")),
    ];

    /// <summary>One malformed eval scope entry, replayed under the wide mode.</summary>
    private static CorpusEntry Entry(string name, byte[] bytes, string reason, int code) =>
        new(name, WideCorpus.Mode, "InvalidArtifact", reason, code, "-", "-", "-", "-", bytes);

    /// <summary>A program the lowering wrote, its eval scope map included.</summary>
    private static byte[] Compiled(string source)
    {
        var compiled = Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.Compile(
            source, Broiler.VM.Profile.JavaScript.Compiler.SliceParseOptions.Script);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            throw new System.InvalidOperationException(
                "the retained eval scope control did not compile: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString()));
        }

        return compiled.Artifact;
    }

    /// <summary>The sound one-site artifact, with exactly one thing about it changed.</summary>
    private static byte[] Artifact(
        bool declareSurface = true,
        uint siteOffset = 7,
        uint siteDepth = 0,
        byte siteFlags = 0,
        bool blockUnderRoot = false,
        bool pushScope = false,
        uint nameConstant = 0,
        bool withSite = true,
        byte[]? code = null,
        JsFormat.FunctionFlags flags = JsFormat.FunctionFlags.ProgramBody,
        JsFormat.EvalScopeKind innerKind = JsFormat.EvalScopeKind.Block,
        byte nameFlags = 0,
        JsFormat.EvalScopeKind rootKind = JsFormat.EvalScopeKind.Program)
    {
        byte[] body = code ?? (pushScope
            ?
            [
                (byte)JsOpcode.PushScope, 0x01, 0x00,
                (byte)JsOpcode.LoadGlobal, 0x02, 0x00,
                (byte)JsOpcode.LoadUndefined,
                (byte)JsOpcode.LoadConstant, 0x00, 0x00,
                (byte)JsOpcode.CallEval, 0x01,
                (byte)JsOpcode.Return,
            ]
            :
            [
                (byte)JsOpcode.LoadGlobal, 0x02, 0x00,
                (byte)JsOpcode.LoadUndefined,
                (byte)JsOpcode.LoadConstant, 0x00, 0x00,
                (byte)JsOpcode.CallEval, 0x01,
                (byte)JsOpcode.Return,
            ]);

        // The site moves with the pushed record, so the variant that pushes one keeps naming its
        // CallEval and differs from the sound map only in the name it spells.
        var offset = pushScope ? siteOffset + 3 : siteOffset;

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
                    JsArtifactWriter.InternedNameConstant("Number"),
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
                [
                    new JsFunctionRow(0, 0, 1, 16, 0, (uint)body.Length, (uint)flags),
                ])),
        };

        if (declareSurface)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Surfaces,
                JsArtifactWriter.Surfaces([JsSurfaces.Dynamic])));
        }

        JsEvalScopeRow[] shapes = blockUnderRoot
            ?
            [
                new JsEvalScopeRow(rootKind, 0, []),
                new JsEvalScopeRow(innerKind, 1, [new JsEvalNameRow(nameConstant, 0, nameFlags)]),
            ]
            : [new JsEvalScopeRow(rootKind, 0, [])];

        JsEvalSiteRow[] sites = withSite
            ?
            [
                new JsEvalSiteRow(
                    0, offset, (uint)(shapes.Length - 1), siteDepth, (JsFormat.EvalRequestFlags)siteFlags),
            ]
            : [];

        sections.Add(new JavaScriptArtifactWriter.Section(
            (JavaScriptFormat.SectionKind)JsFormat.SectionKind.EvalScopes,
            JsArtifactWriter.EvalScopes(shapes, sites, [])));

        return JsArtifactWriter.Write(JsFormat.ManifestId, sections.ToArray());
    }
}
