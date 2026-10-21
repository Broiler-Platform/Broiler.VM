// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM.Abstractions;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;
using EvalFlags = Broiler.VM.Profile.JavaScript.Format.JsFormat.EvalRequestFlags;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The wide lowering's binder pass (SJL-12.6 item 1, JSD-0080 decision D11): the answers it reads
/// off the tree where the lowering used to read them off its own records.
/// </summary>
/// <remarks>
/// <para>
/// <b>An answer read off the tree agrees with the record only while the record holds what the tree
/// says.</b> The binder decides which block-level functions Annex B admits in a function from the
/// syntax - the parameter list's names, <c>arguments</c> and the body's own lexical names - where
/// the lowering used to take the names already declared in the function's record when it hoisted.
/// The two are the same set because nothing a source can spell is declared in that record before the
/// hoisting but the parameters and <c>arguments</c>: a named function expression's own name lives in a
/// record of its own outside the function, and every other early declaration is a temporary whose
/// name begins with <c>#</c>.
/// </para>
/// <para>
/// <b>These rows are what notices a change that breaks that</b>, by what the programs answer rather
/// than by which instructions they compile to. A block function named after a parameter is declined,
/// so the parameter keeps its value; a block function named after the function expression around it
/// is admitted, so its <c>var</c>-scoped binding shadows the expression's own name and holds
/// <c>undefined</c> until the declaration is evaluated. Moving the expression's name into the
/// function's record - or declaring any other name there before the hoisting - fails the second row,
/// whichever way the binder then answers.
/// </para>
/// <para>
/// <b>The binder also reports the language's early errors the lowering used to raise</b> (item 2,
/// decision D12). Each family's row is a source the parser admits, so the refusal is the binder's: its
/// code and its place are the lowering's, written <c>code@line:column</c>, and the refused unit has no
/// bytes. Then the list itself: in source order where the lowering reported a hoisted function first;
/// at most 64; once for a <c>finally</c> block the lowering compiled twice; once for each reference to
/// an evaluated program's caller's <c>arguments</c>, a reference the lowering read and wrote (and so
/// reported twice) included; and alone, because a unit the binder refuses is not lowered, so the
/// lowering's ceiling on parameters is not met beside it. <c>a: b: while (1) { continue a; }</c> stays
/// refused as the lowering refused it. Code the lowering never lowers is walked silently, so an
/// overridden function of sloppy eval code keeps the program accepted; a construct the binder refuses
/// is walked into, so a static block's <c>return</c> value is reported after its 2207.
/// </para>
/// <para>
/// <b>The language rules the lowering coded 2207 and 2104 are the binder's too</b> (JSD-0080 decision
/// D12): <c>return</c> in a class static block keeps its place beside another early error, where a
/// binder refusal of the other, with the 2207 left to the lowering, lost it; and <c>await</c> outside
/// an async function is refused in source order with the rest. The two other 2104-coded rules, a
/// logical assignment to a target that is neither a name nor a property and a <c>using</c> declaration
/// outside a statement list that can dispose it, have no row: the parser refuses every source that
/// would reach them first.
/// </para>
/// </remarks>
internal static class BinderChecks
{
    /// <summary>The identity these checks present to the verifier.</summary>
    private const string Caller = "js-slice-compiler://binder";

    /// <summary>Runs every row.</summary>
    internal static IEnumerable<(string Name, bool Passed, string Detail)> Run()
    {
        yield return Language(
            "annex b declines a block function a parameter of its function names",
            "function f(a) { { function a() { return 2; } } return a; } f(1);",
            "1");

        yield return Language(
            "annex b declines a block function a destructured parameter names, wherever it stands",
            "function f({ a }, b = 0) { if (b === 0) { switch (b) { default: function a() {} } } return a; }" +
            " f({ a: 'kept' });",
            "kept");

        yield return Language(
            "annex b admits a block function named after the function expression around it",
            "(function g() { var before = typeof g; { function g() {} } return before + ':' + typeof g; })();",
            "undefined:function");

        foreach (var row in EarlyErrors())
        {
            yield return row;
        }
    }

    /// <summary>The binder's early errors: a row for each family, then the list's order, cap and effect.</summary>
    private static IEnumerable<(string Name, bool Passed, string Detail)> EarlyErrors()
    {
        const EvalFlags inFunction = EvalFlags.InFunction;
        const EvalFlags inField =
            inFunction | EvalFlags.InMethod | EvalFlags.InClassFieldInitializer | EvalFlags.InClassBody;

        // super and return outside what admits them, in a script and in eval code
        yield return Refused(
            "early error: super member outside a method",
            "function f() { return super.x; }",
            "2101@1:23");
        yield return Refused(
            "early error: super call outside a derived constructor",
            "class A { constructor() { super(); } }",
            "2101@1:27");
        yield return Refused("early error: return outside a function", "return 1;", "2101@1:1");
        yield return Refused(
            "early error: super member in eval code outside a method",
            "super.x;",
            "2101@1:1",
            inFunction);

        // the cover grammar, __proto__ and regular-expression literals
        yield return Refused("early error: a cover initialiser in an object literal", "({ a = 1 });", "2101@1:4");
        yield return Refused("early error: __proto__ set twice", "({ __proto__: 1, __proto__: 2 });", "2101@1:18");
        yield return Refused("early error: a regular-expression pattern", "/(/;", "2101@1:1");
        yield return Refused("early error: regular-expression flags repeated", "/a/gg;", "2101@1:1");
        yield return Refused("early error: regular-expression flags u and v", "/a/uv;", "2101@1:1");

        // break and continue targets
        yield return Refused("early error: break outside a loop or switch", "break;", "2207@1:1");
        yield return Refused("early error: break to no such label", "a: { while (1) { break b; } }", "2207@1:18");
        yield return Refused("early error: continue outside a loop", "continue;", "2208@1:1");
        yield return Refused(
            "early error: continue to the outer label of a label set stays refused",
            "a: b: while (1) { continue a; }",
            "2208@1:19");

        // return in a class static block, under the lowering's 2207 (JSD-0080 decision D12): beside another
        // early error it is reported in its place, as the base's lowering reported both - while the 2207
        // was left to the lowering, a binder refusal of the other answered that alone - and once for a
        // finally block the lowering compiled twice
        yield return Refused(
            "early error: return in a class static block",
            "class C { static { return; } }",
            "2207@1:20");
        yield return Refused(
            "early error: return in a class static block, and a break after it",
            "class C { static { return; } } break;",
            "2207@1:20 2207@1:32");
        yield return Refused(
            "early error: return in a class static block, and a cover initialiser after it",
            "class C { static { return; } } ({a=1});",
            "2207@1:20 2101@1:34");
        yield return Refused(
            "early error: return in a class static block's finally block is reported once",
            "class C { static { try {} finally { return; } } }",
            "2207@1:37");

        // await outside an async function, under the lowering's 2104 (JSD-0080 decision D12): a template
        // substitution's own parse admits one into the parameters of an arrow that is not async, a unit
        // of its own, in a script and in a module that awaits at its top level; in source order, where
        // the lowering met it first in a hoisted function. A logical assignment to a target that is
        // neither a name nor a property, and a `using` declaration outside a statement list that can
        // dispose it, are the binder's under 2104 too, and no source reaches either: the parser refuses
        // every such target (2205) and every such declaration (2101) first
        yield return Refused(
            "early error: await outside an async function, in an arrow's parameters",
            "async function g() { (a = `${await 1}`) => a; }",
            "2104@1:30");
        yield return RefusedModule(
            "early error: await outside an async function, in a module that awaits",
            "(a = `${await 1}`) => a;",
            "2104@1:9");
        yield return Refused(
            "early errors are in source order where the lowering met a hoisted function's 2104 first",
            "break; async function g() { (a = `${await 1}`) => a; }",
            "2207@1:1 2104@1:37");

        // assignment targets
        yield return Refused("early error: update of a call in strict code", "'use strict'; f()++;", "2205@1:15");
        yield return Refused(
            "early error: a call as a for-of target in strict code",
            "'use strict'; for (f() of []) ;",
            "2205@1:20");
        yield return Refused(
            "early error: delete of a private member",
            "class A { #x; m() { delete this.#x; } }",
            "2205@1:21");

        // initialisers a declaration needs
        yield return Refused("early error: const without an initialiser", "const a;", "2203@1:7");
        yield return Refused("early error: a destructuring declaration without an initialiser", "var [a];", "2203@1:5");
        yield return Refused(
            "early error: const without an initialiser in a for head",
            "for (const a; ;) {}",
            "2203@1:12");

        // private names and arguments in class bodies
        yield return Refused(
            "early error: an undeclared private name",
            "class A { m() { return this.#y; } }",
            "2206@1:24");
        yield return Refused(
            "early error: an undeclared private name in eval code outside a class",
            "this.#x;",
            "2206@1:1",
            inFunction);
        yield return Refused(
            "early error: arguments in a field initialiser",
            "class A { x = () => arguments; }",
            "2206@1:11");
        yield return Refused(
            "early error: arguments in a static block",
            "class A { static { arguments; } }",
            "2206@1:11");
        yield return Refused(
            "early error: arguments in eval code in a field initialiser, once for each reference",
            "arguments; arguments;",
            "2206@0:0 2206@0:0",
            inField);
        yield return Refused(
            "early error: arguments in eval code in a field initialiser, read and written, once",
            "arguments += 1;",
            "2206@0:0",
            inField);

        // a module's declaration, collision and export checks
        yield return RefusedModule(
            "early error: a name imported twice",
            "import { a } from 'lib'; import { a } from 'lib';",
            "2201@1:35");
        yield return RefusedModule(
            "early error: a name imported and declared",
            "import { a } from 'lib'; var a;",
            "2201@0:0");
        yield return RefusedModule(
            "early error: a function declared twice in a module",
            "function f() {} function f() {}",
            "2201@1:17");
        yield return RefusedModule(
            "early error: a var and a function of one name in a module",
            "var f; function f() {}",
            "2202@1:8");
        yield return RefusedModule(
            "early error: a name exported twice",
            "var a; export { a }; export { a };",
            "2402@1:31");
        yield return RefusedModule("early error: an export of no declared binding", "export { b };", "2403@1:10");

        // the list: source order where the lowering reported the hoisted function first, the cap, and
        // one report for a finally block the lowering compiled twice
        yield return Refused(
            "early errors are in source order",
            "break; function f() { return super.x; }",
            "2207@1:1 2101@1:30");
        yield return Refused(
            "early errors stop at 64",
            string.Concat(Enumerable.Repeat("break; ", 70)),
            string.Join(" ", Enumerable.Range(0, 64).Select(i => "2207@1:" + (1 + (7 * i)))));
        yield return Refused(
            "early error in a finally block is reported once",
            "try {} finally { break; }",
            "2207@1:18");

        // a unit the binder refuses is not lowered: the lowering's ceiling on parameters (2301) is not met
        var wide = "function f(" + string.Join(", ", Enumerable.Range(0, 256).Select(i => "p" + i)) + ") {} ";
        yield return Refused("early error skips the lowering", wide + "break;", "2207@1:" + (wide.Length + 1));

        // code the lowering never lowers is walked silently: a sloppy evaluated program's function
        // that a later one of the same name overrides is never compiled, so its early error is none
        // and the program stays accepted
        yield return Accepted(
            "early error in an overridden function of sloppy eval code is not reported",
            "function f() { return super.x; } function f() {}",
            EvalFlags.None);

        // and what a construct the binder refuses holds is walked, as every refused construct is: the
        // value of a `return` in a static block, which the lowering lowered nothing of after its 2207,
        // is reported after it now that the 2207 is the binder's
        yield return Refused(
            "early error in a static block's return value is reported after the 2207",
            "class A { static { return /(/; } }",
            "2207@1:20 2101@1:27");
    }

    /// <summary>
    /// One row: the script - eval code when <paramref name="evaluated"/> names its caller - compiles
    /// to bytes with no diagnostic at all.
    /// </summary>
    private static (string, bool, string) Accepted(string name, string source, EvalFlags? evaluated = null)
    {
        var compiled = JsCompiler.Compile([
            new JsScriptUnit(
                "main",
                source,
                evaluated is { } flags ? SliceParseOptions.Eval(flags) : SliceParseOptions.Script),
        ]);

        var answer = string.Join(" ", compiled.Diagnostics.Select(d => (int)d.Code + "@" + d.Line + ":" + d.Column));

        return (
            name,
            compiled.Succeeded && compiled.Artifact is not null && compiled.Diagnostics.Count == 0,
            $"expected the compilation to produce bytes with no diagnostic, and it reported `{answer}`" +
                (compiled.Artifact is null ? " with no bytes" : string.Empty));
    }

    /// <summary>
    /// One row: the script - eval code when <paramref name="evaluated"/> names its caller - is refused
    /// with exactly these diagnostics, each <c>code@line:column</c> in the order reported, and no bytes.
    /// </summary>
    private static (string, bool, string) Refused(
        string name,
        string source,
        string expected,
        EvalFlags? evaluated = null) =>
        Refused(
            name,
            JsCompiler.Compile([
                new JsScriptUnit(
                    "main",
                    source,
                    evaluated is { } flags ? SliceParseOptions.Eval(flags) : SliceParseOptions.Script),
            ]),
            expected);

    /// <summary>One row as a script's is, for a module importing a library.</summary>
    private static (string, bool, string) RefusedModule(string name, string source, string expected) =>
        Refused(
            name,
            JsCompiler.Compile(
                [],
                [
                    new JsModuleUnit("lib", "export var a = 1;", SliceParseOptions.Module),
                    new JsModuleUnit("main", source, SliceParseOptions.Module, [new JsResolvedRequest("lib", "lib")]),
                ]),
            expected);

    /// <summary>Compares a compilation's refusal with what the row expects.</summary>
    private static (string, bool, string) Refused(string name, JsCompilation compiled, string expected)
    {
        var answer = string.Join(" ", compiled.Diagnostics.Select(d => (int)d.Code + "@" + d.Line + ":" + d.Column));

        if (compiled.Succeeded || compiled.Artifact is not null)
        {
            return (name, false, $"expected `{expected}` and the compilation produced bytes (`{answer}`)");
        }

        return (
            name,
            string.Equals(answer, expected, System.StringComparison.Ordinal),
            $"expected `{expected}` and the compilation reported `{answer}`");
    }

    /// <summary>One row: compile and run the program, and compare its completion value.</summary>
    private static (string, bool, string) Language(string name, string source, string expected)
    {
        using var runtime = Runtime();

        if (runtime is null)
        {
            return (name, false, "the runtime refused creation");
        }

        if (!TryRun(runtime, source, out var answer))
        {
            return (name, false, answer);
        }

        return (
            name,
            string.Equals(answer, expected, System.StringComparison.Ordinal),
            $"expected `{expected}` and the program answered `{answer}`");
    }

    /// <summary>Compiles, verifies, instantiates and invokes one script, answering its completion or why not.</summary>
    private static bool TryRun(VmRuntime runtime, string source, out string answer)
    {
        var compiled = JsCompiler.Compile([new JsScriptUnit("main", source, SliceParseOptions.Script)]);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            answer = "the source was refused: " +
                (compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString());

            return false;
        }

        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
            JavaScriptProfile.WideManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity(Caller));

        var verified = runtime.Verify(in descriptor, compiled.Artifact, System.Threading.CancellationToken.None);

        if (!verified.TryGetArtifact(out var handle))
        {
            answer = $"verification: {verified.Outcome}/{verified.Reason}";
            return false;
        }

        using (handle)
        {
            var instantiated = runtime.Instantiate(handle, System.Threading.CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                answer = $"instantiation: {instantiated.Outcome}/{instantiated.Reason}";
                return false;
            }

            using (instance)
            {
                var request = new VmInvocationRequest(
                    new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("main")));

                var invoked = instance.Invoke(in request, System.Threading.CancellationToken.None);

                if (invoked.Outcome is not VmOutcome.Normal ||
                    !JavaScriptProfile.TryGetWideCompletion(in invoked, out var completion))
                {
                    answer = $"the run answered {invoked.Outcome}/{invoked.Reason}";
                    return false;
                }

                answer = completion.Value;
                return true;
            }
        }
    }

    /// <summary>A runtime over this profile with no capability registered.</summary>
    private static VmRuntime? Runtime()
    {
        var catalog = VmCatalog.CreateBuilder().Add(JavaScriptProfile.Descriptor).Build();
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var created = VmRuntime.Create(
            catalog,
            new VmRuntimeCreationOptions(
                aggregateBudget: null,
                ceilings: ceilings.ToImmutable(),
                maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
                maxLiveSuspendedOperations: 1,
                guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
                externalSuspension: VmExternalSuspensionMode.Disabled,
                capabilities: []));

        return created.TryGetRuntime(out var runtime) ? runtime : null;
    }
}
