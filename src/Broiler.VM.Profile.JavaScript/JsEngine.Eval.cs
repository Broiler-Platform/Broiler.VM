// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           0
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    High
// Criteria:         7/7
// Resource impact:  7/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Generic;
using System.Text;
using Broiler.VM.Abstractions;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    /// <summary>
    /// Evaluates a String as a program, through the one route a guest may obtain code by.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Nothing here compiles anything.</b> The source becomes the opaque request payload of a
    /// guest-initiated load, the composition's registered artifact provider answers it with bytes,
    /// and the core verifies those bytes into their own immutable handle before a byte of them
    /// runs — under the requesting operation's remaining allowance, at a nesting depth the core
    /// counts. That is roadmap section 11's design and it is what keeps a compiler inside a
    /// composition's declared closure instead of inside this profile.
    /// </para>
    /// <para>
    /// <b>Two refusals a reader will meet and must not confuse.</b> A composition that DECLINES
    /// <c>broiler.javascript.dynamic</c> never gets here at all: its artifacts naming the surface
    /// were refused at verification, as an invalid artifact the guest never sees. A composition
    /// that admits the surface and registers NO provider gets here and is refused at run time, as
    /// an error the guest may catch. Section 6 draws exactly that distinction and this method is
    /// where the second half of it happens.
    /// </para>
    /// <para>
    /// <b>The direct form reaches this method only where it means the same thing as the indirect
    /// one.</b> A direct <c>eval</c> evaluates in the CALLER's scope, and the artifact this method
    /// asks for is compiled without any knowledge of the frame that asked for it, so its free names
    /// reach the global object. At the top level of a script with nothing between the call and the
    /// body's entry record that is exactly right, because the caller's scope IS the global scope. A
    /// site whose caller's artifact carries a scope-map row is evaluated against the caller's own
    /// records by <see cref="EvaluateAtSite"/> instead (JSeal V14, JSD-0026); a direct call that
    /// arrives here from anywhere else - a function unit whose site has no row - is refused by name
    /// rather than answered with a global's value. That refusal is the published exclusion, not a
    /// defect to be discovered later.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=E1E172
    // Broiler-Falsified-If: guest source becomes executable bytes without passing through the mediator
    // Broiler-Human:        PENDING
    internal JsValue Evaluate(JsValue[] arguments, bool direct, JsFormat.FunctionFlags callerFlags)
    {
        var source = arguments.Length == 0 ? JsValue.Undefined : arguments[0];

        // `eval` of anything that is not a String answers it unchanged. It is the one coercion the
        // language deliberately does not do, so that `eval(someObject)` is a value and not a
        // program.
        if (!source.IsString)
        {
            return source;
        }

        if (direct && (callerFlags & JsFormat.FunctionFlags.ProgramBody) == 0)
        {
            throw Error(
                "EvalError",
                "a direct eval at this call site is not admitted: its artifact describes no scope " +
                "for it - code whose scope reaches a module, a function body whose parameter list " +
                "both calls eval and creates closures, or an artifact written without an eval " +
                "scope map - so evaluated source cannot see the " +
                "calling frame's bindings. An indirect eval - (0, eval)(source) - evaluates in " +
                "the global scope and is admitted");
        }

        if (Loader is null)
        {
            throw Error(
                "EvalError",
                "this composition registered no artifact provider, so no source can become code");
        }

        var text = source.AsString();
        var bytes = Encoding.UTF8.GetBytes(text);

        // Proportional to the source, because a guest that could buy an unbounded compilation with
        // one instruction would have found the hole every budget dimension exists to close.
        Charge(1 + (ulong)bytes.Length);

        // A SOURCE THAT BEGINS WITH A CONTROL CHARACTER BEGINS NO PROGRAM, and it is answered here
        // rather than sent. U+0000 to U+0008 and U+000E to U+001F are neither white space nor a line
        // terminator and begin no token, so every front end refuses such a source - and the first two
        // of them are the marks a module request and an eval request begin with, so sending one would
        // ask the provider a different question than the guest did (JSeal V14). The answer is the
        // one the provider's refusal would have become.
        if (text.Length != 0 && (text[0] <= '\u0008' || text[0] is >= '\u000E' and <= '\u001F'))
        {
            return ThrowSyntaxError("the evaluated source is not a program this profile admits");
        }

        // The request carries the profile's identity, a nesting depth of one, and the source. The
        // core fills in and enforces everything else - the operation the work is charged to, the
        // remaining allowance, the real depth - which is why a profile may state a nesting depth
        // here without that being a way to relax one.
        var request = new VmArtifactRequest(JavaScriptProfile.Id, default, default, 1, default, cancellation, new VmBytes(bytes));

        var loaded = Loader.RequestLoad(in request);

        // A PROVIDER REFUSAL IS A `SyntaxError` AND NOT AN `EvalError`, and the two are different
        // answers to different questions. `ProviderRefused` is what the mediator reports when the
        // provider it asked answered "this is not a program I will supply" - which, for the only
        // providers this profile's compositions register, means the front end refused the SOURCE.
        // The language says `eval` of source that is not a program throws a `SyntaxError`, and
        // programs test for it: a conformance case that asserts `assert.throws(SyntaxError, ...)`
        // over an evaluated string is checking the language and not this host's plumbing, and an
        // `EvalError` there failed a case whose subject this host answers correctly. Every OTHER
        // way a load can fail - no provider registered, a budget exhausted, the mediator out of
        // scope, a foreign artifact - is this host's own plumbing and keeps the `EvalError` it had.
        if (loaded.Reason == VmReason.ProviderRefused)
        {
            return ThrowSyntaxError("the evaluated source is not a program this profile admits");
        }

        if (loaded.Outcome != VmOutcome.Normal || !loaded.TryGetArtifact(out var artifact))
        {
            throw Error(
                "EvalError",
                "the artifact provider did not supply a program: " +
                    loaded.Outcome.ToString() + "/" + loaded.Reason.ToString());
        }

        if (!artifact.TryGetState(out var state) || state is not JsProgram evaluated)
        {
            throw Error("EvalError", "the artifact provider answered with a foreign program");
        }

        RequireInstanceForm(evaluated);

        if (!evaluated.TryFindEntry("main", out var unit))
        {
            throw Error("EvalError", "the evaluated program declares no entry point");
        }

        // IT RUNS IN THIS REALM AND NOT IN A NEW ONE. The handle is a separate verified artifact -
        // its own constants, its own code, its own function table - but the global object it
        // reaches is this engine's, which is what makes `eval("var f = function () {}")` define
        // something the calling program can afterwards call. AND IT RUNS AS ITS CALLER'S SCRIPT OR
        // MODULE: this is dynamic code, whatever referrer a provider compiled it with (JSD-0024
        // section 20).
        return RunEntry(evaluated, unit, activeReferrer);
    }

    /// <summary>
    /// Performs a direct <c>eval</c> from one call site, or refuses it by name.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A direct evaluation without a site row is admitted only where it means what a global
    /// evaluation means</b>, and three places that used to be admitted are not (JSD-0026 step 1). A
    /// module's top level has bindings of its own - its declarations are slots and its imports are
    /// indirections - so evaluating there globally answered a <c>ReferenceError</c> for a name the
    /// module declares; since JSeal V15-module the lowering writes a row for every module site, so
    /// only an artifact without one meets that refusal. A script's body with a block, a <c>for</c>-<c>let</c> head, a
    /// <c>switch</c>, a <c>catch</c> or a <c>with</c> record around the call site has names the
    /// global scope does not, and the frame's current record is not the one the body was entered
    /// with exactly when one of those lies between: a block that declares nothing pushes no
    /// record and binds nothing a global evaluation could miss. Each of these used to run the
    /// source against the global scope and answer with the wrong binding's value.
    /// </para>
    /// <para>
    /// <b>A value that is not a String is answered unchanged before anything is refused</b>,
    /// because that is what <c>eval</c> does with one wherever it is called from: there is no
    /// program to evaluate, so there is nothing about the calling scope to get wrong.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=DB31AD
    // Broiler-Falsified-If: a direct eval with no site row at a module's top level, or in a script body whose current record is not its entry record, evaluates the source instead of throwing an EvalError
    // Broiler-Human:        PENDING
    private JsValue EvaluateDirect(JsProgram program, int unitIndex, int site, List<JsEnvironment> scopes, JsValue[] arguments,
        JsValue thisValue, JsCell? thisBinding, JsValue newTarget, JsScriptFunction? active)
    {
        if (arguments.Length == 0 || !arguments[0].IsString)
        {
            return arguments.Length == 0 ? JsValue.Undefined : arguments[0];
        }

        // A SITE THE CALLER'S MAP DESCRIBES IS EVALUATED AGAINST THE CALLER'S RECORDS, and every
        // other site keeps the answer it had: the global path at a script body's top level, and the
        // explicit refusal everywhere else (JSeal V14, JSD-0026 step 5).
        if (program.EvalMap is { } map && map.TryFindSite((uint)site, out var described))
        {
            return EvaluateAtSite(
                program, described, scopes[^1], arguments[0].AsString(),
                thisValue, thisBinding, newTarget, active);
        }

        var unit = program.Functions[unitIndex];

        if ((unit.Flags & JsFormat.FunctionFlags.ProgramBody) != 0)
        {
            if (program.ModuleOfUnit[unitIndex] >= 0)
            {
                throw Error(
                    "EvalError",
                    "a direct eval at a module's top level is not admitted: the module's own " +
                    "bindings and imports are not visible to evaluated source. An indirect " +
                    "eval - (0, eval)(source) - evaluates in the global scope and is admitted");
            }

            if (scopes.Count != 1)
            {
                throw Error(
                    "EvalError",
                    "a direct eval inside a block, loop head, switch, catch clause or with " +
                    "statement that has bindings of its own is not admitted: evaluated source " +
                    "cannot see them. An indirect eval - (0, eval)(source) - evaluates in the " +
                    "global scope and is admitted");
            }

            // A SCRIPT'S OWN TOP LEVEL WITH NOTHING BETWEEN THE CALL AND THE BODY'S ENTRY RECORD is
            // the global scope and nothing else, so the evaluation is global eval code: its lexical
            // declarations stay its own, it inherits the script's strictness, and its `var`s and
            // functions become configurable globals after the global checks (JSeal V15, JSD-0026
            // step 8). It answers with the caller's `this`, which at a script's top level is the
            // global one.
            return EvaluateGlobal(
                arguments[0].AsString(),
                unit.IsStrict ? JsFormat.EvalRequestFlags.Strict : JsFormat.EvalRequestFlags.None,
                thisValue);
        }

        return Evaluate(arguments, direct: true, unit.Flags);
    }

    /// <summary>
    /// Evaluates a String as global eval code: an indirect <c>eval</c>, or a direct one at a script's
    /// top level with no record around the call.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is eval code and not a script</b> (JSeal V15, JSD-0026 step 8), which is four differences
    /// a script lowering could not give: the program's <c>let</c>, <c>const</c> and <c>class</c>
    /// declarations are its own and never the realm's lexical half; a strict program's <c>var</c>s
    /// and functions are its own too; a sloppy program's become properties of the global object that
    /// are configurable, so <c>delete</c> removes them; and before any is created the global checks run
    /// - a <c>var</c> colliding with a global lexical declaration is a <c>SyntaxError</c> and a
    /// function the global object cannot take is a <c>TypeError</c>, with nothing created either way.
    /// </para>
    /// <para>
    /// <b>The route is the direct one's</b>: the source and the request flags - strict when a strict
    /// script asked directly, never for an indirect call - go to the one provider as an eval request,
    /// the answer is bound to it, and the boundary record it is entered with has no parent and a view
    /// of the global scope alone.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=9043CA
    // Broiler-Falsified-If: guest source becomes executable bytes without passing through the mediator, or a global evaluation's lexical declaration becomes a binding of the realm
    // Broiler-Human:        PENDING
    internal JsValue EvaluateGlobal(string text, JsFormat.EvalRequestFlags flags, JsValue thisValue)
    {
        var (evaluated, unit, declaration) = LoadEvalCode(flags, text);
        var view = JsEvalView.Global();

        // NO CLASS ENCLOSES A GLOBAL EVALUATION, so a private name it uses and does not declare is
        // the SyntaxError `AllPrivateIdentifiersValid` makes it (JSeal V15-finish).
        if (declaration.PrivateNames.Length != 0)
        {
            ThrowSyntaxError(
                "the evaluated source names the private name " + declaration.PrivateNames[0][1..] +
                ", which no class encloses");
        }

        if (declaration.Introduces)
        {
            InstantiateEvalDeclarations(view, null, declaration);
        }

        var code = evaluated.Functions[(int)unit];
        var boundary = new JsEnvironment((int)code.ScopeSlots, null, view);

        // EVAL CODE RUNS AS ITS CALLER'S SCRIPT OR MODULE (PerformEval, JSD-0024 section 20).
        return Execute(evaluated, (int)unit, boundary, thisValue, [], null, JsValue.Undefined, null, null, activeReferrer);
    }

    /// <summary>
    /// Evaluates a String as a SCRIPT an embedder is running in this realm, and answers its completion
    /// value (JSeal V15-host, JSD-0024 section 15).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is a script and not eval code</b>, which is everything the global eval path is not: its
    /// <c>let</c>, <c>const</c> and <c>class</c> declarations are bindings of the realm's global lexical
    /// environment that later scripts see, its <c>var</c>s and functions are non-configurable
    /// properties of the global object, and <c>GlobalDeclarationInstantiation</c>'s checks run before
    /// its first instruction - a lexical declaration colliding with an earlier script's lexical one or
    /// with a non-configurable global property is a <c>SyntaxError</c>, a <c>var</c> or function
    /// colliding with a global lexical one likewise, and a function or <c>var</c> the global object
    /// cannot take a <c>TypeError</c>, with nothing created either way.
    /// </para>
    /// <para>
    /// <b>The route is every guest-initiated load's, with the one mark no guest can write</b>: the
    /// source, its name and the strictness the embedder asked for go to the provider as a script
    /// request, the core verifies the answer under this operation's allowance, and the answer is
    /// bound to the request. Whether a guest may evaluate source is the provider's policy for the
    /// OTHER marks; a guest <c>eval</c> inside the script is still an eval request.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=98EC79
    // Broiler-Falsified-If: a host script's source becomes executable bytes without passing through the mediator, or an answer that is not a script compiled under the requested strictness runs
    // Broiler-Human:        PENDING
    internal JsValue EvaluateScript(string text, string sourceName, bool strict)
    {
        if (Loader is null)
        {
            throw Error(
                "EvalError",
                "this composition registered no artifact provider, so no source can become code");
        }

        var payload = JsFormat.ScriptRequest(
            strict ? JsFormat.ScriptRequestFlags.Strict : JsFormat.ScriptRequestFlags.None,
            sourceName,
            text);

        // Proportional to the request, exactly as every other evaluation is charged.
        Charge(1 + (ulong)payload.Length);

        var request = new VmArtifactRequest(JavaScriptProfile.Id, default, default, 1, default, cancellation, new VmBytes(payload));

        var loaded = Loader.RequestLoad(in request);

        // A REFUSAL IS THE SCRIPT'S `SyntaxError`, named by the source name the embedder gave: the
        // provider holds the diagnostic and its position, and the reason vocabulary cannot carry it.
        if (loaded.Reason == VmReason.ProviderRefused)
        {
            ThrowSyntaxError(
                (sourceName.Length == 0 ? "the host script" : "the host script " + sourceName) +
                " is not a program this profile admits");
        }

        if (loaded.Outcome != VmOutcome.Normal || !loaded.TryGetArtifact(out var artifact))
        {
            throw Error(
                "EvalError",
                "the artifact provider did not supply a program: " +
                    loaded.Outcome.ToString() + "/" + loaded.Reason.ToString());
        }

        if (!artifact.TryGetState(out var state) || state is not JsProgram evaluated)
        {
            throw Error("EvalError", "the artifact provider answered with a foreign program");
        }

        RequireInstanceForm(evaluated);

        if (!evaluated.TryFindEntry("main", out var unit) ||
            (evaluated.Functions[(int)unit].Flags &
                (JsFormat.FunctionFlags.ProgramBody | JsFormat.FunctionFlags.EvalCode)) !=
                JsFormat.FunctionFlags.ProgramBody ||
            evaluated.ModuleOfUnit[(int)unit] >= 0 ||
            (strict && !evaluated.Functions[(int)unit].IsStrict))
        {
            throw Error(
                "EvalError",
                "the artifact provider answered for another goal: a host script is answered by a " +
                "script compiled under the strictness its request asked for");
        }

        // IT RUNS IN THIS REALM AND NOT IN A NEW ONE, through the one entry every script takes.
        return RunEntry(evaluated, unit);
    }

    /// <summary>
    /// Asks the provider for eval code compiled under <paramref name="flags"/>, and binds its answer to
    /// the request or refuses it by name.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The route is the one every guest-initiated load takes, with a marked payload</b> (JSD-0026
    /// sections 5 and 8): the source and the request flags go to the one provider the composition
    /// registered, which is asked once per evaluation and may refuse, and the core verifies what it
    /// answers before a byte of it runs, under this operation's allowance and at a depth it counts.
    /// </para>
    /// <para>
    /// <b>The answer is bound to the request.</b> It must be a program whose entry named <c>eval</c> is
    /// eval code compiled under the flags that were asked for, and anything else - a provider that
    /// compiled the payload as a script, or for another site - is refused with an <c>EvalError</c>
    /// naming a provider that answered for another goal. A program an earlier lowering marked as
    /// one this build does not run - the refusal of sloppy declarations JSeal V14 wrote, or of a
    /// reference to the caller's <c>super</c> or private names V14 and V15 wrote - is refused by
    /// name before its first instruction.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=825417
    // Broiler-Falsified-If: guest source becomes executable bytes without passing through the mediator, or an answer that is not eval code compiled under the requested flags is returned
    // Broiler-Human:        PENDING
    private (JsProgram Program, uint Unit, JsEvalDeclaration Declaration) LoadEvalCode(JsFormat.EvalRequestFlags flags, string text)
    {
        if (Loader is null)
        {
            throw Error("EvalError", "this composition registered no artifact provider, so no source can become code");
        }

        var payload = JsFormat.EvalRequest(flags, text);

        // Proportional to the source, exactly as every other evaluation is charged.
        Charge(1 + (ulong)(payload.Length - 2));

        var request = new VmArtifactRequest(JavaScriptProfile.Id, default, default, 1, default, cancellation, new VmBytes(payload));

        var loaded = Loader.RequestLoad(in request);

        if (loaded.Reason == VmReason.ProviderRefused)
        {
            ThrowSyntaxError("the evaluated source is not a program this profile admits");
        }

        if (loaded.Outcome != VmOutcome.Normal || !loaded.TryGetArtifact(out var artifact))
        {
            throw Error("EvalError", "the artifact provider did not supply a program: " + loaded.Outcome.ToString() + "/" + loaded.Reason.ToString());
        }

        if (!artifact.TryGetState(out var state) || state is not JsProgram evaluated)
        {
            throw Error("EvalError", "the artifact provider answered with a foreign program");
        }

        RequireInstanceForm(evaluated);

        if (!evaluated.TryFindEntry("eval", out var unit) ||
            (evaluated.Functions[(int)unit].Flags & JsFormat.FunctionFlags.EvalCode) == 0 ||
            evaluated.EvalMap is not { } answered ||
            !answered.TryFindDeclaration((int)unit, out var declaration) ||
            declaration.Flags != flags)
        {
            throw Error("EvalError", "the artifact provider answered for another goal: an eval is answered by eval code compiled under the flags its call asked for");
        }

        return declaration.Refusal switch
        {
            JsFormat.EvalRefusal.VarDeclarations => throw Error("EvalError",
                                "the evaluated program carries the refusal of sloppy var and function " +
                                "declarations an earlier build wrote; this build instantiates them itself"),
            JsFormat.EvalRefusal.SuperReference => throw Error("EvalError",
                                "a direct eval whose source refers to the calling method's super is not admitted"),
            JsFormat.EvalRefusal.PrivateName => throw Error("EvalError",
                                "a direct eval whose source names a private name of the calling class is not " +
                                "admitted"),
            _ => (evaluated, unit, declaration),
        };
    }

    /// <summary>
    /// Evaluates a String as direct <c>eval</c> code against one site's caller: its records, its
    /// <c>this</c>, its <c>new.target</c> and its strictness.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The route is the one every guest-initiated load takes, with a marked payload</b> (JSD-0026
    /// sections 5 and 8): the source and the site's request flags go to the one provider the
    /// composition registered, which is asked once per evaluation and may refuse, and the core
    /// verifies what it answers before a byte of it runs, under this operation's allowance and at a
    /// depth it counts. Compile permission is therefore the provider's answer here exactly as it is
    /// for an indirect <c>eval</c>; the scope map grants nothing.
    /// </para>
    /// <para>
    /// <b>The answer is bound to the request.</b> It must be a program whose entry named <c>eval</c>
    /// is eval code compiled under the flags the site asked for, and anything else - a provider that
    /// compiled the payload as a script, or for another site - is refused with an <c>EvalError</c>
    /// naming a provider that answered for another goal. A program the provider compiled and this
    /// build does not run - a sloppy <c>var</c> or function declaration, a reference to the caller's
    /// <c>super</c> or to a private name - is refused by name before its first instruction.
    /// </para>
    /// <para>
    /// <b>The boundary record is new for every evaluation and its parent is the caller's current
    /// record</b>, so two activations of one caller, and two evaluations in one activation, never
    /// share the evaluated program's own bindings, while every write the program makes to one of
    /// its caller's is a write to the caller's own slot - which every closure over that record sees.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=EAD7E6
    // Broiler-Falsified-If: guest source becomes executable bytes without passing through the mediator, or an answer that is not eval code compiled under the site's flags runs
    // Broiler-Human:        PENDING
    private JsValue EvaluateAtSite(JsProgram caller, JsEvalSite site, JsEnvironment record, string text, JsValue thisValue, JsCell? thisBinding, JsValue newTarget, JsScriptFunction? active)
    {
        var (evaluated, unit, declaration) = LoadEvalCode(site.Flags, text);
        var view = new JsEvalView(caller, site);
        var code = evaluated.Functions[(int)unit];
        var boundary = new JsEnvironment((int)code.ScopeSlots, record, view);

        // EVERY PRIVATE NAME THE PROGRAM USES AND DOES NOT DECLARE IS ONE A CLASS AROUND THE CALL
        // DECLARES, or the evaluation is the SyntaxError `AllPrivateIdentifiersValid` makes it -
        // before anything is instantiated (JSeal V15-finish). The names are found through the same
        // verified map the program's own `LoadEvalName` reads them through, by a walk that reads
        // class slots alone - an early error runs no guest code and asks no `with` object.
        foreach (var name in declaration.PrivateNames)
        {
            if (!EvalPrivateNameDeclared(boundary, name))
            {
                ThrowSyntaxError(
                    "the evaluated source names the private name " + name[1..] +
                    ", which no class enclosing the call declares");
            }
        }

        // A SLOPPY EVALUATION'S `var` AND FUNCTION DECLARATIONS ARE ITS CALLER'S, checked and made
        // before its first instruction (JSeal V15, JSD-0026 steps 6-7).
        if (declaration.Introduces)
        {
            InstantiateEvalDeclarations(view, record, declaration);
        }

        // EVAL CODE RUNS AS ITS CALLER'S SCRIPT OR MODULE (PerformEval, JSD-0024 section 20),
        // which is the calling frame's and not necessarily the active function's.
        return Execute(evaluated, (int)unit, boundary, thisValue, [], active, newTarget, thisBinding, null, activeReferrer);
    }

    /// <summary>The eval boundary record <paramref name="hops"/> records out from the current one.</summary>
    /// <remarks>
    /// An eval name instruction that reaches a record with no view was not emitted by the lowering
    /// that wrote the verified program - the verifier admits these instructions only in eval code -
    /// so the answer is an internal defect and never a lookup somewhere else.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=D545DD
    // Broiler-Falsified-If: it answers a record that was not entered as an eval boundary
    // Broiler-Human:        PENDING
    private static JsEnvironment EvalBoundary(List<JsEnvironment> scopes, int hops)
    {
        var boundary = scopes[^1].Ancestor(hops);

        if (boundary?.EvalView is null)
        {
            throw new JsAbort(
                JsAbortKind.InternalDefect, "an eval name instruction reached no eval boundary");
        }

        return boundary;
    }
}
