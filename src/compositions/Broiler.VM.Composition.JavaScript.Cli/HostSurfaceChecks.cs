// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;

namespace Broiler.VM.Composition.JavaScript.Cli;

/// <summary>
/// The checks that say what the host surface does, by running a guest program against one.
/// </summary>
/// <remarks>
/// <para>
/// <b>Every check here is judged by what the GUEST printed, never by what the host returned.</b>
/// A host method that was never called, a property that answered <c>undefined</c>, and a listener
/// that never ran all leave a host-side assertion of "it succeeded" perfectly true, and the shape
/// of defect this lane exists to catch is exactly that one. So each check runs a script that
/// prints an answer only reachable if the seam did its work, and compares the printed lines.
/// </para>
/// <para>
/// <b>It lives in a composition root because nothing else may look.</b> Rule A11 forbids any
/// project outside <c>src/compositions/</c> to reference a profile assembly, so a test project
/// cannot construct a realm, and the only lawful observer of this seam is a composition. This root
/// already describes itself as a demonstration, and this is one.
/// </para>
/// </remarks>
internal static class HostSurfaceChecks
{
    private const string Caller = "broiler-js-cli://host-surface";

    /// <summary>Runs every check, printing one line each, and answers the failure count.</summary>
    internal static int Run()
    {
        var failures = 0;

        failures += Check(
            "a host method answers an object, and the guest reads a string off it",
            """
            var d = broilerHost.document();
            print('title=' + d.title);
            print('kind=' + typeof d);
            """,
            ["title=a page the host owns", "kind=object"]);

        failures += Check(
            "wrapper identity survives the crossing in both directions",
            """
            print('same=' + (broilerHost.document() === broilerHost.document()));
            print('known=' + broilerHost.isKnown(broilerHost.document()));
            """,
            ["same=true", "known=true"]);

        failures += Check(
            "an accessor the host installed runs host code on every read",
            """
            var d = broilerHost.document();
            print('one=' + d.reads);
            print('two=' + d.reads);
            """,
            ["one=1", "two=2"]);

        failures += Check(
            "a host method calls back into the guest, synchronously, before it returns",
            """
            var order = [];
            order.push('before');
            broilerHost.dispatch(function (name) { order.push('listener:' + name); });
            order.push('after');
            print(order.join(','));
            """,
            ["before,listener:click,after"]);

        failures += Check(
            "a listener's throw reaches the host and is re-raised into the guest",
            """
            try {
                broilerHost.dispatch(function () { throw new RangeError('from the listener'); });
                print('no-throw');
            } catch (e) {
                print('caught=' + (e instanceof RangeError) + ':' + e.message);
            }
            """,
            ["caught=true:from the listener"]);

        failures += Check(
            "an error the host raises is a real guest error the guest can catch",
            """
            try {
                broilerHost.refuse();
                print('no-throw');
            } catch (e) {
                print('caught=' + (e instanceof TypeError) + ':' + e.message);
            }
            """,
            ["caught=true:the host refused"]);

        failures += Check(
            "an exotic object answers a name it was never given, and enumerates it",
            """
            var c = broilerHost.collection();
            print('named=' + c.banner);
            print('keys=' + Object.keys(c).join(','));
            print('ordinary=' + c.item);
            """,
            ["named=element:banner", "keys=0,1,item,banner,footer", "ordinary=an ordinary member"]);

        failures += Check(
            "an exotic object answers an index, and enumerates its elements before its names",
            """
            var c = broilerHost.collection();
            print('at0=' + c[0] + ',at1=' + c[1]);
            print('past=' + c[2]);
            print('keys=' + Object.keys(c).join(','));
            """,
            ["at0=first,at1=second", "past=undefined", "keys=0,1,item,banner,footer"]);

        failures += Check(
            "an exotic object takes the name it claims and lets every other assignment through",
            """
            var c = broilerHost.collection();
            c.claimed = 'taken';
            c.expando = 'ordinary';
            print('claimed=' + broilerHost.lastAssigned());
            print('expando=' + c.expando);
            print('shadowed=' + c.claimed);
            """,
            ["claimed=taken", "expando=ordinary", "shadowed=taken"]);

        failures += Check(
            "the host links a wrapper to a prototype the guest can see",
            """
            var w = broilerHost.wrapper();
            print('proto=' + (Object.getPrototypeOf(w) === broilerHost.wrapperPrototype()));
            print('inherited=' + w.describe());
            print('array=' + broilerHost.three().join(',') + ':' + broilerHost.three().length);
            """,
            ["proto=true", "inherited=a wrapper", "array=a,b,c:3"]);

        failures += Check(
            "a realm in a composition that registered no permission has no host object",
            "print('absent=' + (typeof broilerHost));",
            ["absent=undefined"],
            permitted: false);

        failures += Check(
            "the host queues a job beside the guest's own, and a drain runs both in order",
            """
            var order = [];
            broilerHost.queue(function () { order.push('guest'); });
            broilerHost.queueHost();
            print('before=' + order.join(',') + ':' + broilerHost.pending());
            print('ran=' + broilerHost.drain());
            print('after=' + order.join(',') + ':' + broilerHost.pending());
            """,
            ["before=:true", "ran=2", "after=guest,host:false"]);

        failures += CheckTurn();

        return failures;
    }

    /// <summary>
    /// The turn: an embedder reaching its realm from OUTSIDE a step, and being refused inside it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the one check that cannot be a script and its printed lines</b>, because what it
    /// is about happens between two invocations, where no script is running. It runs a program,
    /// then does host work in the gap, then runs a second program that can only print what it
    /// prints if that work happened.
    /// </para>
    /// <para>
    /// <b>It asserts the refusal first, and that ordering is the point.</b> A turn that worked
    /// would be worth little if the realm were reachable without one: the check that the realm
    /// REFUSES outside a step is what makes the turn a door rather than a decoration.
    /// </para>
    /// </remarks>
    private static int CheckTurn()
    {
        const string Title = "an embedder reaches its realm between invocations, and only by asking";

        var printed = new List<string>();
        var surface = new DemonstrationSurface();

        var compiled = JsCompiler.Compile(
            [
                new JsScriptUnit(
                    "first", "print('first=' + (typeof installed));", SliceParseOptions.Script, false, Caller),
                new JsScriptUnit(
                    "second", "print('second=' + installed);", SliceParseOptions.Script, false, Caller),
            ],
            [],
            new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return Report(Title, "the source was refused: " + Render(compiled));
        }

        var catalog = VmCatalog.CreateBuilder()
            .Add(JavaScriptProfile.DescriptorHostingRealms(surface))
            .Build();

        var created = VmRuntime.Create(catalog, Options(printed, permitted: true));

        if (!created.TryGetRuntime(out var runtime))
        {
            return Report(Title, $"the runtime refused creation: {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            var verified = runtime.Verify(in descriptor, compiled.Artifact, CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                return Report(Title, $"verification refused: {verified.Outcome}/{verified.Reason}");
            }

            using (artifact)
            {
                var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

                if (!instantiated.TryGetInstance(out var instance))
                {
                    return Report(
                        Title, $"instantiation refused: {instantiated.Outcome}/{instantiated.Reason}");
                }

                using (instance)
                {
                    if (Run(instance, "first") is { } firstFailure)
                    {
                        return Report(Title, "the first program: " + firstFailure);
                    }

                    // BETWEEN THE TWO, WITH NOTHING RUNNING. The realm exists and is refusable, and
                    // the embedder holds it - so this is exactly the moment the refusal is about.
                    var realm = surface.Realm;

                    if (realm is null)
                    {
                        return Report(Title, "the embedder was never handed a realm");
                    }

                    if (realm.IsCurrent)
                    {
                        return Report(Title, "the realm reports itself current with nothing running");
                    }

                    try
                    {
                        realm.NewObject();
                        return Report(Title, "the realm answered a crossing from outside a step");
                    }
                    catch (JsHostSurfaceException refusal)
                        when (refusal.Refusal is JsHostRefusal.RealmNotCurrent)
                    {
                        // The refusal this check exists to see.
                    }

                    surface.Pending = static asked => asked.DefineValue(
                        asked.Global, "installed", JsHostValue.String("from a turn"));

                    if (Run(instance, JavaScriptProfile.TurnEntryPoint) is { } turnFailure)
                    {
                        return Report(Title, "the turn: " + turnFailure);
                    }

                    if (surface.Turns != 1)
                    {
                        return Report(Title, $"the embedder was given {surface.Turns} turn(s)");
                    }

                    if (Run(instance, "second") is { } secondFailure)
                    {
                        return Report(Title, "the second program: " + secondFailure);
                    }
                }
            }
        }

        string[] expected = ["first=undefined", "second=from a turn"];

        if (printed.Count != expected.Length)
        {
            return Report(
                Title,
                $"printed {printed.Count} line(s) and {expected.Length} were expected: "
                    + string.Join(" | ", printed));
        }

        for (var at = 0; at < expected.Length; at++)
        {
            if (!string.Equals(printed[at], expected[at], System.StringComparison.Ordinal))
            {
                return Report(
                    Title, $"line {at + 1} was '{printed[at]}' and '{expected[at]}' was expected");
            }
        }

        System.Console.WriteLine("ok   " + Title);
        return 0;
    }

    /// <summary>Invokes one entry point, answering a complaint or null.</summary>
    private static string? Run(VmInstance instance, string entryPoint)
    {
        var request = new VmInvocationRequest(
            new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(entryPoint)));

        var result = instance.Invoke(in request, CancellationToken.None);

        return result.Outcome is VmOutcome.Normal
            ? null
            : $"invoking '{entryPoint}' answered {result.Outcome}/{result.Reason}";
    }

    /// <summary>Runs one script against a realm this lane builds, and compares what it printed.</summary>
    private static int Check(string title, string source, string[] expected, bool permitted = true)
    {
        var printed = new List<string>();
        var surface = new DemonstrationSurface();

        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("check", source, SliceParseOptions.Script, false, Caller)],
            [],
            new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return Report(title, "the source was refused: " + Render(compiled));
        }

        // THE DESCRIPTOR CARRIES THE SURFACE AND THE OPTIONS CARRY THE PERMISSION, and the two
        // halves are what the `permitted: false` case exists to separate. Both are supplied here;
        // only the registration is withheld, so the failing case differs from the passing one in
        // exactly the composition's own act and in nothing else.
        var catalog = VmCatalog.CreateBuilder()
            .Add(JavaScriptProfile.DescriptorHostingRealms(surface))
            .Build();

        var created = VmRuntime.Create(catalog, Options(printed, permitted));

        if (!created.TryGetRuntime(out var runtime))
        {
            return Report(title, $"the runtime refused creation: {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            var verified = runtime.Verify(in descriptor, compiled.Artifact, CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                return Report(title, $"verification refused: {verified.Outcome}/{verified.Reason}");
            }

            using (artifact)
            {
                var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

                if (!instantiated.TryGetInstance(out var instance))
                {
                    return Report(
                        title, $"instantiation refused: {instantiated.Outcome}/{instantiated.Reason}");
                }

                using (instance)
                {
                    var request = new VmInvocationRequest(
                        new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("check")));
                    var result = instance.Invoke(in request, CancellationToken.None);

                    if (result.Outcome is not VmOutcome.Normal)
                    {
                        return Report(title, $"the run answered {result.Outcome}/{result.Reason}");
                    }
                }
            }
        }

        if (printed.Count != expected.Length)
        {
            return Report(
                title,
                $"printed {printed.Count} line(s) and {expected.Length} were expected: "
                    + string.Join(" | ", printed));
        }

        for (var at = 0; at < expected.Length; at++)
        {
            if (!string.Equals(printed[at], expected[at], System.StringComparison.Ordinal))
            {
                return Report(
                    title, $"line {at + 1} was '{printed[at]}' and '{expected[at]}' was expected");
            }
        }

        System.Console.WriteLine("ok   " + title);
        return 0;
    }

    private static int Report(string title, string detail)
    {
        System.Console.WriteLine("FAIL " + title + ": " + detail);
        return 1;
    }

    private static string Render(JsCompilation compiled)
    {
        if (compiled.Diagnostics.Count == 0)
        {
            return "no diagnostic was named, which is a defect here";
        }

        return compiled.Diagnostics[0].ToString();
    }

    /// <summary>The runtime options: `print` always, the host-surface permission conditionally.</summary>
    private static VmRuntimeCreationOptions Options(List<string> printed, bool permitted)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var capabilities = ImmutableArray.CreateBuilder<VmCapabilityRegistration>();

        capabilities.Add(VmCapabilityRegistration.Value(
            JavaScriptProfile.WriteCapability,
            (VmBytes argument, out VmOpaqueRef answer) =>
            {
                answer = default;
                printed.Add(System.Text.Encoding.UTF8.GetString(argument.Span).TrimEnd('\n'));
                return VmHostCallOutcome.Completed;
            }));

        if (permitted)
        {
            // THE PERMISSION, AND IT CARRIES NO TRAFFIC. The handler is never invoked: the profile
            // asks `IsBound` once, at instantiation, and nothing else ever addresses this slot. A
            // body that did anything would be a body nothing could call.
            capabilities.Add(VmCapabilityRegistration.Value(
                JavaScriptProfile.HostSurfaceCapability,
                (VmBytes argument, out VmOpaqueRef answer) =>
                {
                    answer = default;
                    return VmHostCallOutcome.Completed;
                }));
        }

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: capabilities.ToImmutable());
    }

    /// <summary>An embedder small enough to read, and shaped like the one this seam is for.</summary>
    private sealed class DemonstrationSurface : IJsHostSurface
    {
        private JsHostValue document;
        private bool built;
        private double reads;

        /// <summary>What the embedder does when a host asks for a turn.</summary>
        internal System.Action<JsHostRealm>? Pending { get; set; }

        /// <summary>How many turns it has been given.</summary>
        internal int Turns { get; private set; }

        /// <summary>The realm this embedder was handed, so a check can hold it between steps.</summary>
        internal JsHostRealm? Realm { get; private set; }

        public void OnTurn(JsHostRealm realm)
        {
            Turns++;

            var work = Pending;
            Pending = null;
            work?.Invoke(realm);
        }

        public void OnRealmCreated(JsHostRealm realm)
        {
            Realm = realm;
            var host = realm.NewObject();

            realm.DefineValue(
                host,
                "document",
                realm.NewMethod("document", (r, _, _) => Document(r)));

            realm.DefineValue(
                host,
                "isKnown",
                realm.NewMethod(
                    "isKnown",
                    (r, _, arguments) => JsHostValue.Boolean(
                        arguments.Length > 0 && built && arguments[0] == document),
                    1));

            // THE RE-ENTRANT ONE. It receives a guest function and calls it, synchronously, before
            // it returns - which is what an event dispatch is and what the whole seam is for.
            realm.DefineValue(
                host,
                "dispatch",
                realm.NewMethod(
                    "dispatch",
                    (r, _, arguments) =>
                    {
                        if (arguments.Length > 0)
                        {
                            System.Span<JsHostValue> listenerArguments = [JsHostValue.String("click")];
                            r.Invoke(arguments[0], JsHostValue.Undefined, listenerArguments);
                        }

                        return JsHostValue.Undefined;
                    },
                    1));

            realm.DefineValue(
                host,
                "refuse",
                realm.NewMethod(
                    "refuse",
                    (r, _, _) => throw r.Error(JsHostErrorKind.TypeError, "the host refused")));

            // THE JOB MEMBERS. `queue` takes a guest function and enqueues it; `queueHost` enqueues
            // an action of the embedder's own; `drain` runs them and answers how many ran. The
            // check is that the two kinds share one queue and come out in the order they went in.
            realm.DefineValue(
                host,
                "queue",
                realm.NewMethod(
                    "queue",
                    (r, _, arguments) =>
                    {
                        if (arguments.Length > 0)
                        {
                            var callback = arguments[0];
                            r.EnqueueJob(() => r.Invoke(callback, JsHostValue.Undefined));
                        }

                        return JsHostValue.Undefined;
                    },
                    1));

            realm.DefineValue(
                host,
                "queueHost",
                realm.NewMethod(
                    "queueHost",
                    (r, _, _) =>
                    {
                        r.EnqueueJob(() =>
                        {
                            var order = r.GetProperty(r.Global, "order");
                            var push = r.GetProperty(order, "push");
                            System.Span<JsHostValue> pushed = [JsHostValue.String("host")];
                            r.Invoke(push, order, pushed);
                        });

                        return JsHostValue.Undefined;
                    }));

            realm.DefineValue(
                host,
                "pending",
                realm.NewMethod("pending", (r, _, _) => JsHostValue.Boolean(r.HasPendingJobs)));

            realm.DefineValue(
                host,
                "drain",
                realm.NewMethod("drain", (r, _, _) => JsHostValue.Number(r.DrainJobs())));

            realm.DefineValue(
                host,
                "collection",
                realm.NewMethod("collection", (r, _, _) => Collection(r)));

            realm.DefineValue(
                host,
                "lastAssigned",
                realm.NewMethod(
                    "lastAssigned",
                    (_, _, _) => elements is null || elements.Assigned is null
                        ? JsHostValue.Null
                        : JsHostValue.String(elements.Assigned)));

            realm.DefineValue(
                host,
                "wrapperPrototype",
                realm.NewMethod("wrapperPrototype", WrapperPrototype));

            realm.DefineValue(
                host,
                "wrapper",
                realm.NewMethod(
                    "wrapper",
                    (r, _, _) =>
                    {
                        var wrapper = r.NewObject();
                        r.SetPrototype(wrapper, WrapperPrototype(r, JsHostValue.Undefined, default));
                        return wrapper;
                    }));

            // AN ARRAY BUILT ONE INDEX AT A TIME, which is the case DefineIndex exists for: an
            // index written through the ordinary string path would not move the Array's length,
            // and `join` walks length rather than the property map.
            realm.DefineValue(
                host,
                "three",
                realm.NewMethod(
                    "three",
                    (r, _, _) =>
                    {
                        var array = r.NewArray();
                        r.DefineIndex(array, 0, JsHostValue.String("a"));
                        r.DefineIndex(array, 1, JsHostValue.String("b"));
                        r.DefineIndex(array, 2, JsHostValue.String("c"));
                        return array;
                    }));

            realm.DefineValue(realm.Global, "broilerHost", host);
        }

        private JsHostValue prototype;
        private bool prototypeBuilt;
        private NamedElements? elements;

        private JsHostValue WrapperPrototype(
            JsHostRealm realm, JsHostValue thisValue, System.ReadOnlySpan<JsHostValue> arguments)
        {
            if (prototypeBuilt)
            {
                return prototype;
            }

            prototype = realm.NewObject();
            realm.DefineValue(
                prototype,
                "describe",
                realm.NewMethod("describe", (_, _, _) => JsHostValue.String("a wrapper")));

            prototypeBuilt = true;
            return prototype;
        }

        private JsHostValue Document(JsHostRealm realm)
        {
            if (built)
            {
                return document;
            }

            document = realm.NewObject();
            realm.DefineValue(document, "title", JsHostValue.String("a page the host owns"));

            // An accessor, so the check can see host code run on every read rather than once.
            realm.DefineAccessor(document, "reads", (_, _, _) => JsHostValue.Number(++reads));

            built = true;
            return document;
        }

        private JsHostValue Collection(JsHostRealm realm)
        {
            elements ??= new NamedElements();
            var collection = realm.NewExotic(elements);

            // An ORDINARY property with a name the handler also supports, which is the case the
            // base-first rule decides: this must win.
            realm.DefineValue(collection, "item", JsHostValue.String("an ordinary member"));

            return collection;
        }
    }

    /// <summary>A live collection whose members are names rather than a fixed list.</summary>
    private sealed class NamedElements : IJsHostExotic
    {
        private static readonly string[] Names = ["item", "banner", "footer"];

        private static readonly string[] Elements = ["first", "second"];

        /// <summary>What a guest assigned to a name this collection claims.</summary>
        internal string? Assigned { get; private set; }

        public bool TryGetIndex(JsHostRealm realm, uint index, out JsHostValue value)
        {
            if (index < (uint)Elements.Length)
            {
                value = JsHostValue.String(Elements[index]);
                return true;
            }

            value = JsHostValue.Missing;
            return false;
        }

        // IT CLAIMS ONE NAME AND DECLINES EVERY OTHER, which is the case that matters: a guest
        // assigning a name the embedder does not own must land on the object as an ordinary
        // property rather than vanishing into a handler that took everything.
        public bool TrySetNamed(JsHostRealm realm, string name, JsHostValue value)
        {
            if (!string.Equals(name, "claimed", System.StringComparison.Ordinal))
            {
                return false;
            }

            // IT READS BACK THROUGH THE HANDLER, which is what makes this a property the embedder
            // owns rather than a write that vanished. A style declaration behaves exactly this way:
            // the assignment goes to the embedder's own state and the next read comes back from it.
            Assigned = realm.ToJsString(value);
            return true;
        }

        public uint IndexedLength(JsHostRealm realm) => (uint)Elements.Length;

        public bool TryGetNamed(JsHostRealm realm, string name, out JsHostValue value)
        {
            if (string.Equals(name, "claimed", System.StringComparison.Ordinal))
            {
                value = Assigned is null ? JsHostValue.Missing : JsHostValue.String(Assigned);
                return Assigned is not null;
            }

            for (var at = 0; at < Names.Length; at++)
            {
                if (string.Equals(Names[at], name, System.StringComparison.Ordinal))
                {
                    value = JsHostValue.String("element:" + name);
                    return true;
                }
            }

            value = JsHostValue.Missing;
            return false;
        }

        public IReadOnlyList<string> SupportedNames(JsHostRealm realm) => Names;
    }
}
