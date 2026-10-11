// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   11
// Annotated:        11/11
// Exempt:           0
// Human-reviewed:   0/11
// IP risk:          Low
// Security risk:    High
// Criteria:         7/7
// Resource impact:  7/10 max
// Unverified:       11
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using Broiler.VM.Abstractions;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    // ---- the dynamic import ---------------------------------------------------------------------

    /// <summary>
    /// Answers the promise of the module <paramref name="specifier"/> names from
    /// <paramref name="referrer"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A DYNAMIC IMPORT IS TWO ROUTES AND THE ARTIFACT'S OWN MODULES ARE THE FAST ONE.</b> A
    /// specifier the referring module already requested statically names a module this artifact
    /// carries, whose key the composition confirmed at link and whose instance this realm may
    /// already hold — so that call is answered with no host round trip at all, and it is answered
    /// with the SAME instance the static import got, which is the identity the language requires.
    /// Every other specifier is a value nobody resolved before the bytes were written, so it goes
    /// to the mediator: the referrer and the specifier become the request payload, the composition's
    /// provider answers with a verified artifact carrying the module the pair names, and this realm
    /// links and evaluates that artifact's graph beside its own.
    /// </para>
    /// <para>
    /// <b>Neither route compiles anything here, which is the point of having the second one.</b>
    /// <c>import(x + y)</c> is a specifier no compiler saw; answering it means somebody has to turn
    /// a specifier into bytes at run time, and in this profile that somebody is never this
    /// component. It is the same door <c>eval</c> goes through, distinguished by the payload's
    /// first byte — see <see cref="JsFormat.ModuleRequestMark"/> — and it is gated by the same
    /// surface, which is why an artifact containing a dynamic import declares
    /// <c>broiler.javascript.dynamic</c> whether or not any particular call reaches the mediator.
    /// </para>
    /// <para>
    /// <b>Three refusals a reader will meet and must not confuse.</b> A composition that DECLINED
    /// the dynamic surface never gets here: its artifact was refused at verification, as an invalid
    /// artifact the guest never sees. A composition that admits the surface and registers NO
    /// provider gets here and is refused at run time, by a rejection the guest may catch. And a
    /// composition whose provider cannot answer for this specifier is refused at run time too, by a
    /// different rejection naming the specifier. That is the same three-way distinction
    /// <see cref="Evaluate"/> draws for <c>eval</c>, and this joins it rather than inventing a
    /// fourth shape.
    /// </para>
    /// <para>
    /// <b>Nothing here throws.</b> Every failure — a specifier that will not coerce, an attribute
    /// nothing honours, a module nothing can find, a module whose body threw — settles the promise
    /// as a rejection, because a production that answers a promise and sometimes throws is one no
    /// <c>catch</c> can be written against.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=ADB554
    // Broiler-Falsified-If: this throws instead of rejecting, or a specifier reaches bytes without passing through the mediator or the artifact's own records
    // Broiler-Human:        PENDING
    internal JsValue DynamicImport(JsProgram program, string referrer, JsValue specifierValue, JsValue optionsValue)
    {
        var promise = Realm.NewAsyncPromise();
        Charge(FuelPerInstruction);

        try
        {
            // THE SPECIFIER IS COERCED BEFORE THE OPTIONS ARE READ, which is the specification's
            // order and is observable: `import({toString(){throw a}}, {get with(){throw b}})`
            // rejects with `a`.
            // A TYPED REQUEST IS A DIFFERENT MODULE FROM THE UNTYPED ONE, so the type is part of
            // the string every lookup below matches (JsFormat.TypedSpecifier).
            var type = HonourableAttributesType(optionsValue);
            var specifier = JsFormat.TypedSpecifier(ToStringValue(specifierValue), type);

            if (!TryOwnRequest(program, referrer, specifier, out var found))
            {
                // A SPECIFIER NOBODY RESOLVED BEFORE THE BYTES WERE WRITTEN MAY BE ANSWERED LATER.
                // An embedder that loads its modules asynchronously takes the request here and
                // completes it from a turn of its own; until then the promise is simply pending,
                // and nothing on this path waits for it (JSD-0024 section 15). A typed request is
                // not offered: the embedder's seam names a specifier and no type, and answering a
                // JSON request with whatever that specifier loads as a program would be wrong.
                if (type.Length == 0 &&
                    hostRealm is { ModuleLoader: not null } seam &&
                    seam.OfferModuleRequest(referrer, specifier, promise))
                {
                    return JsValue.Object(promise);
                }

                found = MediatedModule(referrer, specifier);
            }

            EvaluateInto(found.Program, found.Index, promise, settleWithNamespace: true);
        }
        catch (JsThrow thrown)
        {
            Realm.SettleAsyncPromise(this, promise, thrown.Value, rejected: true);
        }

        return JsValue.Object(promise);
    }

    /// <summary>
    /// Evaluates the graph rooted at one module and settles <paramref name="promise"/> when it is
    /// finished: with the module's namespace for an import, with <c>undefined</c> for a host.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The promise is settled when the graph is finished and not when it is started</b>, from
    /// the reaction to the evaluation's own promise, so every caller has one channel for every
    /// failure and a namespace is never handed out before its module has run.
    /// </para>
    /// <para>
    /// <b>An import links and evaluates from a job</b>, as <c>ContinueDynamicImport</c> does once
    /// its load promise settles (JSeal I11-async): the evaluation never begins inside the body that
    /// asked for it, so an import of a module that is still on the walk waits for it rather than
    /// meeting it half evaluated. A host's evaluation starts at once, unless it too arrives while an
    /// evaluation is under way.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=50B9DF
    // Broiler-Falsified-If: the promise settles before every module of the graph has finished, or a failure escapes as a throw
    // Broiler-Human:        PENDING
    internal void EvaluateInto(JsProgram program, int root, JsPromiseObject promise, bool settleWithNamespace)
    {
        if (settleWithNamespace || moduleEvaluationDepth != 0)
        {
            var ready = Realm.NewHostPromise(this);
            Realm.SettleAsyncPromise(this, ready, JsValue.Undefined, rejected: false);

            Realm.ReactOn(this, ready, (engine, value, threw) => engine.LinkAndEvaluate(program, root, promise, settleWithNamespace));

            return;
        }

        LinkAndEvaluate(program, root, promise, settleWithNamespace);
    }

    /// <summary>
    /// Links the graph rooted at one module, evaluates it, and settles <paramref name="promise"/>
    /// from the evaluation's reaction.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=F6359B
    // Broiler-Falsified-If: the promise settles other than through the job queue, or a failure escapes as a throw
    // Broiler-Human:        PENDING
    private void LinkAndEvaluate(JsProgram program, int root, JsPromiseObject promise, bool settleWithNamespace)
    {
        try
        {
            var instances = Graph(program);
            var evaluation = EvaluateModule(instances[root]);

            // A HOST'S EVALUATION THAT FINISHED ON THE WALK IS SETTLED AT ONCE, as JSD-0024 section
            // 15 has always settled it; its reactions still run from the queue. An import waits for
            // the evaluation's reaction, which is `ContinueDynamicImport`'s own step.
            if (!settleWithNamespace && evaluation.State != JsPromiseState.Pending)
            {
                var rejected = evaluation.State == JsPromiseState.Rejected;
                Realm.SettleAsyncPromise(
                    this, promise, rejected ? evaluation.Result : JsValue.Undefined, rejected);

                return;
            }

            Realm.ReactOn(this, evaluation, (engine, value, threw) =>
            engine.Realm.SettleAsyncPromise(engine, promise, threw ? value : settleWithNamespace ? JsValue.Object(instances[root].Namespace!) : JsValue.Undefined, threw));
        }
        catch (JsThrow thrown)
        {
            Realm.SettleAsyncPromise(this, promise, thrown.Value, rejected: true);
        }
    }

    /// <summary>
    /// Loads and links the graph a host names, evaluating nothing, and answers its root.
    /// </summary>
    /// <remarks>
    /// <b>The same door a dynamic import goes through, and no other.</b> The request is a module
    /// request to the composition's artifact provider, verified by the core under this operation's
    /// allowance, and answered only by an artifact whose entry is a module graph - so a provider
    /// that compiled the text as a classic script is refused by name. The graph is instantiated
    /// here, so its namespace exists and its declarations are initialised, but no body runs until
    /// a host asks for an evaluation.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=85EF5C
    // Broiler-Falsified-If: a module body runs before a host asks for an evaluation, or a module request is answered without the mediator
    // Broiler-Human:        PENDING
    internal (JsProgram Program, int Index) LinkHostModule(string referrer, string specifier)
    {
        var found = MediatedModule(referrer, specifier);
        _ = Graph(found.Program);
        return found;
    }

    /// <summary>
    /// Completes an import an embedder deferred: loads through the mediator now, and settles the
    /// import's promise when the graph is finished.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=6; Fingerprint=DD5620
    // Broiler-Falsified-If: a deferred import settles other than through the job queue, or a failure escapes as a throw
    // Broiler-Human:        PENDING
    internal void CompleteImport(string referrer, string specifier, JsPromiseObject promise)
    {
        try
        {
            var found = MediatedModule(referrer, specifier);
            EvaluateInto(found.Program, found.Index, promise, settleWithNamespace: true);
        }
        catch (JsThrow thrown)
        {
            Realm.SettleAsyncPromise(this, promise, thrown.Value, rejected: true);
        }
    }

    /// <summary>The key the composition resolved one module of one artifact to.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=709ABF
    // Broiler-Human:        PENDING
    internal static string ModuleKey(JsProgram program, int index) => program.Modules[index].Key;

    /// <summary>The namespace of one module of one artifact, which linking has already built.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6DBC15
    // Broiler-Human:        PENDING
    internal JsValue ModuleNamespace(JsProgram program, int index) => JsValue.Object(Graph(program)[index].Namespace!);

    /// <summary>
    /// Reads a dynamic import's second argument, and answers the module type it asks for: empty
    /// for none, or <see cref="JsFormat.JsonModuleType"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The shape is checked in full before any attribute is judged</b>, and that order is what
    /// a conformance suite grades: <c>import('./m', null)</c> and <c>import('./m', {with: 1})</c>
    /// and <c>import('./m', {with: {type: 1}})</c> are three different programs and the language
    /// rejects each of them for its own reason, none of which is "this host has no loader for
    /// that type". The attributes are the object's enumerable own string keys, read through its
    /// own operations, so a proxy is asked exactly what <c>EnumerableOwnProperties</c> asks.
    /// </para>
    /// <para>
    /// <b><c>type: "json"</c> is honoured and every other attribute is declined</b>, as the static
    /// form is in the front end: an attribute says what KIND of thing the specifier names, and the
    /// JSON document is the one kind a composition of this profile can load besides a program.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1847A1
    // Broiler-Human:        PENDING
    private string HonourableAttributesType(JsValue options)
    {
        if (options.Type == JsType.Undefined)
        {
            return string.Empty;
        }

        if (!options.IsObject)
        {
            ThrowTypeError("the second argument of import() is an object or undefined");
        }

        var attributes = GetProperty(options, "with");

        if (attributes.Type == JsType.Undefined)
        {
            return string.Empty;
        }

        if (!attributes.IsObject)
        {
            ThrowTypeError("the `with` property of an import options object is an object");
        }

        var carrier = attributes.AsObject();
        var declined = string.Empty;
        var type = string.Empty;

        foreach (var key in carrier.OwnPropertyNames())
        {
            Charge(1);

            if (!carrier.TryGetOwnProperty(key, out var property) || !property.Enumerable)
            {
                continue;
            }

            var value = GetProperty(attributes, key);

            if (!value.IsString)
            {
                ThrowTypeError("the import attribute `" + key + "` is not a string");
            }

            if (string.Equals(key, "type", StringComparison.Ordinal) &&
                string.Equals(value.AsString(), JsFormat.JsonModuleType, StringComparison.Ordinal))
            {
                type = JsFormat.JsonModuleType;
                continue;
            }

            if (declined.Length == 0)
            {
                declined = key;
            }
        }

        if (declined.Length != 0)
        {
            ThrowTypeError(
                "no composition of this profile can honour the import attribute `" + declined +
                "`, so the module this call names cannot be loaded");
        }

        return type;
    }

    /// <summary>
    /// Finds the module one specifier names from one referrer in the referring artifact's own
    /// request table, answering false when the referrer never requested it statically.
    /// </summary>
    /// <remarks>
    /// <b>A module this realm already holds is never loaded a second time</b>, and the two routes
    /// meet at that rule rather than each keeping their own answer. This fast path finds it through
    /// the referring module's own request table; the mediator's answer
    /// (<see cref="MediatedModule"/>) is matched against the keys this realm has already instanced,
    /// so an artifact that arrives carrying a module the realm has is a second VIEW of that module
    /// and not a second copy of it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=5AFEC6
    // Broiler-Falsified-If: one module key is evaluated twice in one realm
    // Broiler-Human:        PENDING
    private bool TryOwnRequest(JsProgram program, string referrer, string specifier, out (JsProgram Program, int Index) found)
    {
        foreach (var record in program.Modules)
        {
            if (!string.Equals(record.Key, referrer, StringComparison.Ordinal))
            {
                continue;
            }

            for (var index = 0; index < record.RequestSpecifiers.Length; index++)
            {
                Charge(1);

                if (string.Equals(
                    record.RequestSpecifiers[index], specifier, StringComparison.Ordinal))
                {
                    found = (program, record.Requests[index]);
                    return true;
                }
            }

            break;
        }

        found = default;
        return false;
    }

    /// <summary>
    /// Asks the composition's artifact provider for the module one specifier names from one
    /// referrer, and answers the verified graph's root.
    /// </summary>
    /// <remarks>
    /// The answer must be a module graph: an artifact whose <c>module</c> entry is a module body.
    /// Anything else - a classic script compiled from the same text, a program with no such entry -
    /// is refused, so a module payload never runs under the script goal.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=7; Fingerprint=5D98BF
    // Broiler-Falsified-If: a module request is answered by an artifact whose root is not a module body
    // Broiler-Human:        PENDING
    private (JsProgram Program, int Index) MediatedModule(string referrer, string specifier)
    {
        if (Loader is null)
        {
            ThrowTypeError(
                "this composition registered no artifact provider, so '" + specifier +
                "' cannot be loaded");
        }

        var payload = JsFormat.ModuleRequest(referrer, specifier);
        Charge(1 + (ulong)payload.Length);

        var request = new VmArtifactRequest(JavaScriptProfile.Id, default, default, 1, default, cancellation, new VmBytes(payload));

        var loaded = Loader!.RequestLoad(in request);

        if (loaded.Outcome != VmOutcome.Normal || !loaded.TryGetArtifact(out var artifact))
        {
            // A MODULE THAT WILL NOT PARSE OR WILL NOT LINK IS A `SyntaxError`, AND THE REST IS A
            // `TypeError`, and the two answer different questions. `ProviderRefused` is what the
            // mediator reports when the provider's front end refused the module's SOURCE;
            // `InvalidArtifact` is what the core reports when the graph the provider supplied did
            // not link - a name two star re-exports supply, an import of a name nothing exports, a
            // resolution that walks a cycle. The language calls all of those the module's own
            // syntax, and the suite tests for it directly: a hundred and thirty variants assert
            // `SyntaxError` on the rejection of a dynamic import, and a `TypeError` there failed
            // cases whose subject this host answers correctly. Everything else - no such module, no
            // provider, a surface the composition declined, an allowance spent - is this host's own
            // plumbing rather than the module's text, and keeps the `TypeError` it had.
            var linkage = loaded.Reason == VmReason.ProviderRefused ||
                (loaded.Outcome == VmOutcome.InvalidArtifact &&
                    loaded.Reason is not VmReason.UnsupportedFeatureManifest
                        and not VmReason.UnknownFeature);

            throw Error(
                linkage ? "SyntaxError" : "TypeError",
                "the module '" + specifier + "' could not be loaded from '" + referrer + "': " +
                    loaded.Outcome.ToString() + "/" + loaded.Reason.ToString());
        }

        if (!artifact.TryGetState(out var state) || state is not JsProgram loadedProgram)
        {
            throw Error("TypeError", "the artifact provider answered with a foreign program");
        }

        RequireInstanceForm(loadedProgram);

        if (!loadedProgram.TryFindEntry(ModuleEntryName, out var unit) ||
            loadedProgram.ModuleOfUnit[(int)unit] is var _ and < 0)
        {
            ThrowTypeError("the artifact answered for '" + specifier + "' carries no module graph");
        }

        return (loadedProgram, loadedProgram.ModuleOfUnit[(int)unit]);
    }
}
