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
            ["named=element:banner", "keys=item,banner,footer", "ordinary=an ordinary member"]);

        failures += Check(
            "a realm in a composition that registered no permission has no host object",
            "print('absent=' + (typeof broilerHost));",
            ["absent=undefined"],
            permitted: false);

        return failures;
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

        public void OnRealmCreated(JsHostRealm realm)
        {
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

            realm.DefineValue(
                host,
                "collection",
                realm.NewMethod("collection", (r, _, _) => Collection(r)));

            realm.DefineValue(realm.Global, "broilerHost", host);
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

        private static JsHostValue Collection(JsHostRealm realm)
        {
            var collection = realm.NewExotic(new NamedElements());

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

        public bool TryGetNamed(JsHostRealm realm, string name, out JsHostValue value)
        {
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
