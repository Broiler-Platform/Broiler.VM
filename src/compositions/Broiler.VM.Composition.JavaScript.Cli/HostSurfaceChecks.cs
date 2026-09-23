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
internal static partial class HostSurfaceChecks
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
            "a buffer the host filled reads as half precision in the guest, and back as bytes",
            """
            var b = broilerHost.halfBuffer();
            var f = new Float16Array(b);
            print('read=' + f.join(','));
            f[0] = 65504;
            f[1] = 1 / 3;
            print('bytes=' + broilerHost.bytesOf(b));
            print('host=' + broilerHost.halfAt(b, 1));
            """,
            ["read=1,-2", "bytes=ff,7b,55,35", "host=0.333251953125"]);

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

        failures += Check(
            "the host detaches a buffer the guest handed it, and only an ArrayBuffer",
            """
            var b = new ArrayBuffer(8);
            var v = new Uint8Array(b);
            v[0] = 7;
            broilerHost.detach(b);
            print('detached=' + b.byteLength + ':' + v.length + ':' + v[0]);
            broilerHost.detach(b);
            print('again=' + b.byteLength);
            try { broilerHost.detach(v); print('no-throw'); } catch (e) { print('view=' + (e instanceof TypeError)); }
            try { broilerHost.detach(1); print('no-throw'); } catch (e) { print('number=' + (e instanceof TypeError) + ':' + e.message); }
            var own = new ArrayBuffer(2);
            broilerHost.keep(own);
            print('own=' + broilerHost.detachKept() + ':' + own.byteLength);
            broilerHost.keep(new ArrayBuffer(4));
            """,
            [
                "detached=0:0:undefined",
                "again=0",
                "view=true",
                "number=true:DetachArrayBuffer requires an ArrayBuffer",
                "own=detached:0",
            ]);

        // THIS CHECK READS WHAT THE ONE ABOVE LEFT BEHIND, and that is its whole premise: the
        // buffer it was handed was minted by the previous check's realm, which is a different realm
        // from this one, so detaching it here must be refused rather than performed.
        failures += Check(
            "a buffer from another realm is refused rather than detached",
            "print('foreign=' + broilerHost.detachKept());",
            ["foreign=refused:foreign"]);

        failures += CheckTurn();
        failures += RunBinary();

        // ---- deletion (JSeal I05) --------------------------------------------------------------

        failures += Check(
            "a deleting exotic takes a claimed name away, and the ordinary deletion runs after it",
            """
            var s = broilerHost.storage();
            s.stored = 'written';
            s.expando = 'ordinary';
            print('before=' + s.stored + ',' + s.expando + ',' + Object.keys(s).join(','));
            print('deleted=' + (delete s.stored) + ',' + (delete s.expando));
            print('after=' + s.stored + ',' + s.expando + ',' + Object.keys(s).join(','));
            print('offered=' + broilerHost.deletions());
            """,
            [
                "before=written,ordinary,expando,stored",
                "deleted=true,true",
                "after=undefined,undefined,",
                "offered=stored,expando",
            ]);

        failures += Check(
            "an index key and a symbol are never offered to the deletion hook, and 007 is a name",
            """
            var s = broilerHost.storage();
            var k = Symbol('mark');
            s[7] = 'seven'; s['007'] = 'padded'; s[4294967295] = 'max'; s[k] = 'marked';
            var r = [delete s[7], delete s['007'], delete s[4294967295], delete s[k]];
            print('deleted=' + r.join(','));
            print('gone=' + [s[7], s['007'], s[4294967295], s[k]].join(',') + ':' + (7 in s));
            print('offered=' + broilerHost.deletions());
            """,
            ["deleted=true,true,true,true", "gone=,,,:false", "offered=007,4294967295"]);

        failures += Check(
            "Reflect, a trapless Proxy and the host's own delete each offer a name exactly once",
            """
            var s = broilerHost.storage();
            s.a = 'one'; s.b = 'two'; s.c = 'three';
            print('reflect=' + Reflect.deleteProperty(s, 'a'));
            print('proxy=' + (delete new Proxy(s, {}).b));
            print('host=' + broilerHost.hostDelete(s, 'c'));
            print('left=' + [s.a, s.b, s.c].join(',') + ':' + Object.keys(s).length);
            print('offered=' + broilerHost.deletions());
            """,
            ["reflect=true", "proxy=true", "host=true", "left=,,:0", "offered=a,b,c"]);

        failures += Check(
            "a declined name keeps answering, and a non-configurable ordinary property stays",
            """
            var s = broilerHost.storage();
            print('constant=' + (delete s.constant) + ':' + s.constant);
            print('sloppy=' + (delete s.fixed) + ':' + s.fixed);
            print('strict=' + (function () {
                'use strict';
                try { delete s.fixed; return 'no-throw'; } catch (e) { return e instanceof TypeError; }
            })() + ':' + s.fixed);
            print('offered=' + broilerHost.deletions());
            var c = broilerHost.collection();
            print('no-hook=' + (delete c.banner) + ':' + c.banner);
            """,
            [
                "constant=true:always",
                "sloppy=false:pinned",
                "strict=true:pinned",
                "offered=constant,fixed,fixed",
                "no-hook=true:element:banner",
            ]);

        failures += Check(
            "a deletion hook that throws ends the deletion, and the property is left in place",
            """
            var s = broilerHost.storage();
            s.explosive = 'kept'; s.thrower = 'also kept';
            try { delete s.explosive; print('no-throw'); }
            catch (e) { print('caught=' + (e instanceof TypeError) + ':' + e.message); }
            try { delete s.thrower; print('no-throw'); } catch (e) { print('raw=' + e); }
            print('kept=' + s.explosive + ',' + s.thrower);
            """,
            ["caught=true:the host refused the deletion", "raw=a raw reason", "kept=kept,also kept"]);

        failures += Check(
            "a deletion hook refused at the seam gives the guest a TypeError and keeps the property",
            """
            var s = broilerHost.storage();
            s.refusing = 'kept';
            try { delete s.refusing; print('no-throw'); }
            catch (e) { print('caught=' + (e instanceof TypeError) + ':' + e.message); }
            print('kept=' + s.refusing + ':' + broilerHost.deletions());
            """,
            ["caught=true:the value presented to Invoke is not callable", "kept=kept:refusing"]);

        failures += Check(
            "a deletion hook whose guest callback exhausts the allowance ends the run, uncatchably",
            """
            var s = broilerHost.storage();
            s.spinning = 'kept';
            var spin = function () { for (;;) {} };
            print('before');
            try { delete s.spinning; print('no-throw'); } catch (e) { print('caught=' + e); }
            print('after');
            """,
            ["before"],
            outcome: VmOutcome.ResourceExhaustion);

        // ---- exotic hooks at the crossing (VM-FIX-I) -------------------------------------------

        // THE FOREIGN VALUE IS THE BUFFER THE DETACH CHECKS LEFT BEHIND, minted by an earlier
        // check's realm: a hook answering it answers a reference that means nothing in this one.
        failures += Check(
            "an exotic hook that throws, is refused, or answers a foreign value gives the guest a catchable error",
            """
            var h = broilerHost.hostile('');
            function t(f) {
                try { return 'value:' + f(); }
                catch (e) { return e instanceof TypeError ? 'TypeError:' + e.message : 'raw:' + e; }
            }
            print('named-throw=' + t(function () { return h.thrower; }));
            print('named-error=' + t(function () { return h.erring; }));
            print('named-refused=' + t(function () { return h.refusing; }));
            print('named-foreign=' + t(function () { return h.foreign; }).slice(0, 10));
            print('index-foreign=' + t(function () { return h[0]; }).slice(0, 10));
            print('index-throw=' + t(function () { return h[1]; }));
            print('set-throw=' + t(function () { h.setThrow = 1; return 'set'; }));
            print('set-refused=' + t(function () { h.setRefuse = 1; return 'set'; }));
            print('ordinary=' + h.plain + ':' + t(function () { h.expando = 2; return h.expando; }));
            """,
            [
                "named-throw=raw:a raw reason",
                "named-error=TypeError:the host refused the read",
                "named-refused=TypeError:the value presented to Invoke is not callable",
                "named-foreign=TypeError:",
                "index-foreign=TypeError:",
                "index-throw=raw:an index reason",
                "set-throw=raw:a write reason",
                "set-refused=TypeError:the value presented to Invoke is not callable",
                "ordinary=an ordinary answer:value:2",
            ]);

        failures += Check(
            "an exotic hook that throws while the object is enumerated gives the guest the thrown value",
            """
            function keys(h) {
                try { return 'value:' + Object.keys(h).join(','); }
                catch (e) { return e instanceof TypeError ? 'TypeError:' + e.message : 'raw:' + e; }
            }
            print('names=' + keys(broilerHost.hostile('names')));
            print('length=' + keys(broilerHost.hostile('length')));
            print('quiet=' + keys(broilerHost.hostile('')));
            """,
            [
                "names=raw:a names reason",
                "length=TypeError:the value presented to Invoke is not callable",
                "quiet=value:plain",
            ]);

        failures += Check(
            "an exotic hook whose guest callback exhausts the allowance ends the run, uncatchably",
            """
            var h = broilerHost.hostile('');
            var spin = function () { for (;;) {} };
            print('before');
            try { print('read=' + h.spinning); } catch (e) { print('caught=' + e); }
            print('after');
            """,
            ["before"],
            outcome: VmOutcome.ResourceExhaustion);

        // ---- exotic hooks are charged crossings (VM-FIX-J) -------------------------------------

        // THE SAME LOOP OVER AN ORDINARY PROPERTY AND OVER A NAMED ONE, under one host-call ceiling:
        // the ordinary property is answered by the object's own storage and never reaches the
        // handler, so it costs no crossing; each named read enters one (JSD-0024 section 19.2).
        failures += Check(
            "an ordinary property of an exotic object is read without a crossing",
            """
            var c = broilerHost.collection();
            var n = 0;
            for (var i = 0; i < 1000; i++) { if (c.item) { n++; } }
            print('ordinary=' + n);
            """,
            ["ordinary=1000"],
            ceilings: new Dictionary<VmBudgetDimension, ulong> { [VmBudgetDimension.HostCalls] = 200 });

        failures += Check(
            "every exotic hook is a charged crossing, so named reads spend the host-call allowance",
            """
            var c = broilerHost.collection();
            print('before');
            var n = 0;
            for (var i = 0; i < 1000; i++) { if (c.banner) { n++; } }
            print('named=' + n);
            """,
            ["before"],
            outcome: VmOutcome.ResourceExhaustion,
            ceilings: new Dictionary<VmBudgetDimension, ulong> { [VmBudgetDimension.HostCalls] = 200 });

        // THE ENGINE'S OWN READS AFTER THE STEP HAS CLOSED are not the guest's crossings: rendering an
        // uncaught exotic value, or a completion value that is one, does not ask the handler at all,
        // so the run ends as any uncaught value or any completion does rather than as a contract
        // violation (JSD-0024 section 19.2).
        failures += Check(
            "an exotic object thrown uncaught ends the run as an ordinary guest fault",
            """
            print('a');
            throw broilerHost.collection();
            """,
            ["a"],
            outcome: VmOutcome.ProfileFault,
            reason: VmReason.ProfileFaultUnspecified);

        failures += Check(
            "a script whose completion value is an exotic object completes normally",
            """
            print('a');
            broilerHost.collection();
            """,
            ["a"]);

        // ---- promise capabilities (JSeal I07) --------------------------------------------------

        failures += Check(
            "a host promise settles through the job queue and never synchronously",
            """
            var order = [];
            var p = broilerHost.promise();
            print('kind=' + (p instanceof Promise) + ':' + Object.prototype.toString.call(p));
            p.then(function (v) { order.push('then:' + v); });
            print('settle=' + broilerHost.resolve('done'));
            order.push('sync');
            print('before=' + order.join(','));
            print('ran=' + broilerHost.drain());
            print('after=' + order.join(','));
            """,
            [
                "kind=true:[object Promise]",
                "settle=Accepted",
                "before=sync",
                "ran=1",
                "after=sync,then:done",
            ]);

        failures += Check(
            "the first settlement of a host promise wins, and a late one is reported as such",
            """
            var seen = [];
            var p = broilerHost.promise();
            p.then(function (v) { seen.push('fulfilled:' + v); },
                   function (r) { seen.push('rejected:' + r); });
            print('first=' + broilerHost.resolve('one'));
            print('second=' + broilerHost.resolve('two'));
            print('third=' + broilerHost.reject('three'));
            var q = broilerHost.promise();
            q.then(function (v) { seen.push('fulfilled:' + v); },
                   function (r) { seen.push('rejected:' + r); });
            print('reject=' + broilerHost.reject('no'));
            print('late=' + broilerHost.resolve('yes'));
            print('queued=' + broilerHost.pending());
            broilerHost.drain();
            print('seen=' + seen.join(','));
            """,
            [
                "first=Accepted",
                "second=AlreadyResolved",
                "third=AlreadyResolved",
                "reject=Accepted",
                "late=AlreadyResolved",
                "queued=true",
                "seen=fulfilled:one,rejected:no",
            ]);

        failures += Check(
            "a thenable is adopted the way the engine's own resolve procedure adopts one",
            """
            var log = [];
            var thenable = { get then() {
                log.push('get');
                return function (resolve) { log.push('call'); resolve('adopted'); };
            } };
            var p = broilerHost.promise();
            p.then(function (v) { log.push('value:' + v); });
            print('settle=' + broilerHost.resolve(thenable));
            log.push('sync');
            print('before=' + log.join(','));
            broilerHost.drain();
            print('after=' + log.join(','));
            var reasons = [];
            var self = broilerHost.promise();
            self.catch(function (e) { reasons.push('self:' + (e instanceof TypeError)); });
            broilerHost.resolve(self);
            var bad = broilerHost.promise();
            bad.catch(function (e) { reasons.push('getter:' + e); });
            broilerHost.resolve({ get then() { throw 'boom'; } });
            broilerHost.drain();
            print('reasons=' + reasons.join(','));
            """,
            [
                "settle=Accepted",
                "before=get,sync",
                "after=get,sync,call,value:adopted",
                "reasons=self:true,getter:boom",
            ]);

        failures += Check(
            "a guest replacing Promise changes nothing about how the host builds one",
            """
            var Original = Promise;
            var originalThen = Promise.prototype.then;
            Promise = function () { throw new Error('replaced'); };
            var p = broilerHost.promise();
            print('proto=' + (Object.getPrototypeOf(p) === Original.prototype));
            var got = [];
            originalThen.call(p, function (v) { got.push(v); });
            broilerHost.resolve('native');
            broilerHost.drain();
            print('got=' + got.join(','));
            """,
            ["proto=true", "got=native"]);

        failures += Check(
            "a host promise settled from another CLR thread is refused, and the promise stays pending",
            """
            var p = broilerHost.promise();
            var state = 'pending';
            p.then(function () { state = 'settled'; });
            print('thread=' + broilerHost.resolveFromAnotherThread('x'));
            broilerHost.drain();
            print('state=' + state);
            """,
            ["thread=RealmNotCurrent", "state=pending"]);

        failures += Check(
            "Missing is refused as a settlement value, and the promise stays open for a real one",
            """
            var p = broilerHost.promise();
            var got = [];
            p.then(function (v) { got.push(typeof v + ':' + v); });
            print('missing=' + broilerHost.resolveMissing());
            print('reject-missing=' + broilerHost.rejectMissing());
            print('pending=' + broilerHost.pending());
            print('real=' + broilerHost.resolve(undefined));
            broilerHost.drain();
            print('got=' + got.join(','));
            """,
            [
                "missing=ArgumentException:value",
                "reject-missing=ArgumentException:reason",
                "pending=false",
                "real=Accepted",
                "got=undefined:undefined",
            ]);

        failures += Check(
            "an [[IsHTMLDDA]] object is typeof undefined, falsy and == null, and an object to everything else",
            """
            var d = broilerHost.htmlDda;
            print('typeof=' + typeof d);
            print('bool=' + !!d + ',' + (d ? 'then' : 'else') + ',' + ((d && 'and') === d) + ',' + (d || 'or'));
            print('loose=' + (d == null) + ',' + (d == undefined) + ',' + (null == d) + ',' + (d != undefined));
            print('strict=' + (d === undefined) + ',' + (d === null) + ',' + (d == d) + ',' + (d == 0));
            print('call=' + d() + ',' + d(''));
            print('object=' + ((d ?? 1) === d) + ',' + (d?.x) + ',' + (Object(d) === d) + ',' + Boolean(d));
            var e = d; e ??= 1; var o = { d: d }; o.d ??= 2;
            print('nullish=' + (e === d) + ',' + (o.d === d) + ',' + d?.() + ',' + o.d?.length);
            """,
            [
                "typeof=undefined",
                "bool=false,else,true,or",
                "loose=true,true,true,false",
                "strict=false,false,true,false",
                "call=null,null",
                "object=true,undefined,true,false",
                "nullish=true,true,null,0",
            ]);

        failures += CheckPromiseLifetime();
        failures += RunModules();

        // ---- scripts (JSeal V15-host) ----------------------------------------------------------

        failures += RunScripts();

        // ---- detached clone adoption across realms and threads (JSeal I17) ---------------------

        failures += RunClone();

        // ---- BigInt across the seam (JSeal B06) -------------------------------------------------

        failures += RunBigInt();

        // ---- Missing, the referrer of eval and Function code, and module state (JSD-0024 20) ------

        failures += RunMissing();
        failures += RunReferrers();
        failures += RunModuleStates();

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

                    // THE SAME REFUSAL FOR A DETACH, which is the member that can empty a buffer
                    // a program is still holding: outside a step it may not act at all.
                    try
                    {
                        realm.DetachArrayBuffer(JsHostValue.Undefined);
                        return Report(Title, "the realm detached from outside a step");
                    }
                    catch (JsHostSurfaceException refusal)
                        when (refusal.Refusal is JsHostRefusal.RealmNotCurrent)
                    {
                        // Refused before the argument was looked at.
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

    /// <summary>
    /// A host promise across invocations: refused outside a step, refused in the wrong realm,
    /// settled in a turn, run only by a drain, reported late, and refused after disposal.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Like the turn check, this one cannot be a script</b>, because what it is about happens
    /// between invocations and across two instances. Each host-side outcome is recorded as a line
    /// and compared beside what the guest printed, so a refusal that never happened and a settlement
    /// that never ran are both visible.
    /// </para>
    /// <para>
    /// <b>Two instances of one artifact are two realms</b>, and that is how a wrong-realm settlement
    /// is reached without inventing a second embedder: the second realm is handed a capability and a
    /// value the first one minted.
    /// </para>
    /// </remarks>
    private static int CheckPromiseLifetime()
    {
        const string Title =
            "a host promise settles only in a turn of its own realm, late is reported, disposed is refused";

        var printed = new List<string>();
        var surface = new DemonstrationSurface();
        JsHostPromiseCapability? held = null;

        var compiled = JsCompiler.Compile(
            [
                new JsScriptUnit(
                    "first",
                    "var got = 'pending'; hostPromise.then(function (v) { got = 'fulfilled:' + v; });",
                    SliceParseOptions.Script,
                    false,
                    Caller),
                new JsScriptUnit(
                    "check", "print('before-drain=' + got);", SliceParseOptions.Script, false, Caller),
                new JsScriptUnit(
                    "second", "print('after-drain=' + got);", SliceParseOptions.Script, false, Caller),
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
                var first = runtime.Instantiate(artifact, CancellationToken.None);
                var second = runtime.Instantiate(artifact, CancellationToken.None);

                if (!first.TryGetInstance(out var owner) || !second.TryGetInstance(out var other))
                {
                    return Report(
                        Title, $"instantiation refused: {first.Outcome}/{second.Outcome}");
                }

                JsHostRealm? owning = null;

                using (other)
                {
                    using (owner)
                    {
                        // THE CAPABILITY IS MINTED IN A TURN, as an embedder would mint one for a
                        // request the guest made earlier, and the guest is handed its promise.
                        surface.Pending = asked =>
                        {
                            owning = asked;
                            held = asked.NewPromiseCapability();
                            asked.DefineValue(asked.Global, "hostPromise", held.Promise);
                        };

                        if (Run(owner, JavaScriptProfile.TurnEntryPoint) is { } mint)
                        {
                            return Report(Title, "the minting turn: " + mint);
                        }

                        if (held is null || owning is null)
                        {
                            return Report(Title, "the turn minted no capability");
                        }

                        if (Run(owner, "first") is { } firstFailure)
                        {
                            return Report(Title, "the first program: " + firstFailure);
                        }

                        // BETWEEN INVOCATIONS: nothing is running, so there is no turn to settle in.
                        printed.Add("outside=" + Refusal(() => owning.ResolvePromise(held, JsHostValue.String("x"))));

                        // THE OTHER REALM, inside a turn of its own: the capability and a value are
                        // both the first realm's, and each is refused by name.
                        var capability = held;
                        surface.Pending = asked =>
                        {
                            printed.Add("foreign-capability=" + Refusal(
                                () => asked.ResolvePromise(capability, JsHostValue.String("x"))));

                            var own = asked.NewPromiseCapability();
                            printed.Add("foreign-value=" + Refusal(
                                () => asked.ResolvePromise(own, capability.Promise)));
                        };

                        if (Run(other, JavaScriptProfile.TurnEntryPoint) is { } foreign)
                        {
                            return Report(Title, "the foreign turn: " + foreign);
                        }

                        // ITS OWN REALM, in a turn: the first settlement is accepted and the second
                        // is late.
                        surface.Pending = asked =>
                        {
                            printed.Add("turn=" + asked.ResolvePromise(capability, JsHostValue.String("from a turn")));
                            printed.Add("late=" + asked.RejectPromise(capability, JsHostValue.String("too late")));
                        };

                        if (Run(owner, JavaScriptProfile.TurnEntryPoint) is { } settle)
                        {
                            return Report(Title, "the settling turn: " + settle);
                        }

                        if (Run(owner, "check") is { } checkFailure)
                        {
                            return Report(Title, "the check program: " + checkFailure);
                        }

                        if (Run(owner, JavaScriptProfile.DrainEntryPoint) is { } drain)
                        {
                            return Report(Title, "the drain: " + drain);
                        }

                        if (Run(owner, "second") is { } secondFailure)
                        {
                            return Report(Title, "the second program: " + secondFailure);
                        }
                    }

                    // THE INSTANCE IS RELEASED, and the capability outlives it in the embedder's
                    // hands. No step can open for this realm again, so it can never settle.
                    printed.Add("disposed=" + Refusal(() => owning.ResolvePromise(held, JsHostValue.String("x"))));
                }
            }
        }

        string[] expected =
        [
            "outside=RealmNotCurrent",
            "foreign-capability=ForeignRealm",
            "foreign-value=ForeignRealm",
            "turn=Accepted",
            "late=AlreadyResolved",
            "before-drain=pending",
            "after-drain=fulfilled:from a turn",
            "disposed=RealmNotCurrent",
        ];

        if (printed.Count != expected.Length)
        {
            return Report(
                Title,
                $"recorded {printed.Count} line(s) and {expected.Length} were expected: "
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

    /// <summary>Names the refusal a settlement met, or says it was not refused.</summary>
    private static string Refusal(System.Func<JsHostSettlement> settle)
    {
        try
        {
            return "not refused: " + settle();
        }
        catch (JsHostSurfaceException refusal)
        {
            return refusal.Refusal.ToString();
        }
    }

    /// <summary>Names the argument refusal a settlement met, or says it was not refused.</summary>
    private static string Misuse(System.Func<JsHostSettlement> settle)
    {
        try
        {
            return "not refused: " + settle();
        }
        catch (System.ArgumentException refusal)
        {
            return refusal.GetType().Name + ":" + refusal.ParamName;
        }
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
    private static int Check(
        string title,
        string source,
        string[] expected,
        bool permitted = true,
        VmOutcome outcome = VmOutcome.Normal,
        IReadOnlyDictionary<VmBudgetDimension, ulong>? ceilings = null,
        VmReason? reason = null)
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

        var created = VmRuntime.Create(catalog, Options(printed, permitted, ceilings));

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

                    if (result.Outcome != outcome)
                    {
                        return Report(
                            title,
                            $"the run answered {result.Outcome}/{result.Reason} and {outcome} was expected");
                    }

                    if (reason is { } named && result.Reason != named)
                    {
                        return Report(
                            title,
                            $"the run answered {result.Outcome}/{result.Reason} and {outcome}/{named} was expected");
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

    /// <summary>
    /// The runtime options: `print` always, the host-surface permission conditionally, and the
    /// profile's default ceilings except where a check names its own.
    /// </summary>
    private static VmRuntimeCreationOptions Options(
        List<string> printed, bool permitted, IReadOnlyDictionary<VmBudgetDimension, ulong>? explicitCeilings = null)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (explicitCeilings is not null && explicitCeilings.TryGetValue(dimension, out var ceiling))
            {
                ceilings.Add(VmCeilingSpec.Value(dimension, ceiling));
                continue;
            }

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
    private sealed partial class DemonstrationSurface : IJsHostSurface
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

            InstallBinary(realm, host);
            InstallBigInt(realm, host);
            // A MISBEHAVING EXOTIC (VM-FIX-I): every hook of its handler has a name or a mode on
            // which it throws, is refused at the seam, or answers a value the realm refuses.
            realm.DefineValue(
                host,
                "hostile",
                realm.NewMethod(
                    "hostile",
                    (r, _, arguments) => r.NewExotic(new HostileElements(r.ToJsString(arguments[0]), kept)),
                    1));

            // THE DELETION MEMBERS. `storage` mints a storage area whose handler implements the
            // optional deletion hook; `deletions` answers every name that hook was offered, in
            // order; `hostDelete` deletes through the realm's own member rather than the guest's.
            realm.DefineValue(
                host,
                "storage",
                realm.NewMethod("storage", (r, _, _) => Storage(r)));

            realm.DefineValue(
                host,
                "deletions",
                realm.NewMethod(
                    "deletions",
                    (_, _, _) => JsHostValue.String(
                        storage is null ? string.Empty : string.Join(",", storage.Offered))));

            realm.DefineValue(
                host,
                "hostDelete",
                realm.NewMethod(
                    "hostDelete",
                    (r, _, arguments) => JsHostValue.Boolean(
                        r.DeleteProperty(arguments[0], r.ToJsString(arguments[1]))),
                    2));

            // THE PROMISE MEMBERS. `promise` mints a capability and answers its promise; `resolve`
            // and `reject` settle the most recent one and answer the outcome's name, so the guest
            // prints what the host was told.
            realm.DefineValue(
                host,
                "promise",
                realm.NewMethod(
                    "promise",
                    (r, _, _) =>
                    {
                        capability = r.NewPromiseCapability();
                        return capability.Promise;
                    }));

            realm.DefineValue(
                host,
                "resolve",
                realm.NewMethod(
                    "resolve",
                    (r, _, arguments) => JsHostValue.String(
                        r.ResolvePromise(capability!, arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined)
                            .ToString()),
                    1));

            realm.DefineValue(
                host,
                "reject",
                realm.NewMethod(
                    "reject",
                    (r, _, arguments) => JsHostValue.String(
                        r.RejectPromise(capability!, arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined)
                            .ToString()),
                    1));

            // A SETTLEMENT WITH NO VALUE AT ALL, which is an embedder's mistake and is refused as
            // one; each answers the exception's type and the parameter it names.
            realm.DefineValue(
                host,
                "resolveMissing",
                realm.NewMethod(
                    "resolveMissing",
                    (r, _, _) => JsHostValue.String(
                        Misuse(() => r.ResolvePromise(capability!, JsHostValue.Missing)))));

            realm.DefineValue(
                host,
                "rejectMissing",
                realm.NewMethod(
                    "rejectMissing",
                    (r, _, _) => JsHostValue.String(
                        Misuse(() => r.RejectPromise(capability!, JsHostValue.Missing)))));

            // A SETTLEMENT FROM A THREAD THAT IS NOT THE GUEST'S, made while the guest's step is
            // open. The step being open is the point: the refusal is about the thread, not the turn.
            realm.DefineValue(
                host,
                "resolveFromAnotherThread",
                realm.NewMethod(
                    "resolveFromAnotherThread",
                    (r, _, arguments) =>
                    {
                        var value = arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined;
                        var held = capability!;
                        var outcome = "not run";

                        var worker = new Thread(() =>
                        {
                            try
                            {
                                outcome = "not refused: " + r.ResolvePromise(held, value);
                            }
                            catch (JsHostSurfaceException refusal)
                            {
                                outcome = refusal.Refusal.ToString();
                            }
                        });

                        worker.Start();
                        worker.Join();
                        return JsHostValue.String(outcome);
                    },
                    1));

            // THE BUFFER ROUND TRIP. The host builds a buffer out of the realm's own constructors and
            // writes half-precision bit patterns into it byte by byte; the guest reads them through
            // a Float16Array and writes back; the host reads the bytes again, and reads an element
            // through a Float16Array of its own. Every step goes through the public realm API
            // (Construct, GetIndex, SetProperty), so the bytes cross as Numbers: the host surface
            // has no raw view of a buffer's bytes yet (the host-copy contracts I01-I04), and this
            // proves the two sides agree on the layout, not a copy of raw memory.
            realm.DefineValue(
                host,
                "halfBuffer",
                realm.NewMethod(
                    "halfBuffer",
                    (r, _, _) =>
                    {
                        System.Span<JsHostValue> length = [JsHostValue.Number(4)];
                        var buffer = r.Construct(r.GetProperty(r.Global, "ArrayBuffer"), length);
                        System.Span<JsHostValue> over = [buffer];
                        var bytes = r.Construct(r.GetProperty(r.Global, "Uint8Array"), over);

                        // 1 and -2, little-endian: 0x3C00 and 0xC000.
                        r.SetProperty(bytes, "1", JsHostValue.Number(0x3C));
                        r.SetProperty(bytes, "3", JsHostValue.Number(0xC0));
                        return buffer;
                    }));

            realm.DefineValue(
                host,
                "bytesOf",
                realm.NewMethod(
                    "bytesOf",
                    (r, _, arguments) =>
                    {
                        System.Span<JsHostValue> over = [arguments[0]];
                        var bytes = r.Construct(r.GetProperty(r.Global, "Uint8Array"), over);
                        var parts = new List<string>();

                        for (uint at = 0; at < 4; at++)
                        {
                            parts.Add(((int)r.ToNumber(r.GetIndex(bytes, at))).ToString(
                                "x2", System.Globalization.CultureInfo.InvariantCulture));
                        }

                        return JsHostValue.String(string.Join(",", parts));
                    },
                    1));

            realm.DefineValue(
                host,
                "halfAt",
                realm.NewMethod(
                    "halfAt",
                    (r, _, arguments) =>
                    {
                        System.Span<JsHostValue> over = [arguments[0]];
                        var halves = r.Construct(r.GetProperty(r.Global, "Float16Array"), over);
                        return r.GetIndex(halves, (uint)r.ToNumber(arguments[1]));
                    },
                    2));

            // THE DETACH MEMBERS. `detach` lets a guest TypeError propagate as the guest's own;
            // `keep` holds a value past the end of its realm, so the next check can hand a buffer
            // from one realm to another and see the refusal.
            realm.DefineValue(
                host,
                "detach",
                realm.NewMethod(
                    "detach",
                    (r, _, arguments) =>
                    {
                        r.DetachArrayBuffer(arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined);
                        return JsHostValue.Undefined;
                    },
                    1));

            realm.DefineValue(
                host,
                "keep",
                realm.NewMethod(
                    "keep",
                    (_, _, arguments) =>
                    {
                        kept = arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined;
                        return JsHostValue.Undefined;
                    },
                    1));

            realm.DefineValue(
                host,
                "detachKept",
                realm.NewMethod(
                    "detachKept",
                    (r, _, _) =>
                    {
                        try
                        {
                            r.DetachArrayBuffer(kept);
                            return JsHostValue.String("detached");
                        }
                        catch (JsHostSurfaceException refusal)
                            when (refusal.Refusal is JsHostRefusal.ForeignRealm)
                        {
                            return JsHostValue.String("refused:foreign");
                        }
                    }));

            // AN OBJECT WITH AN [[IsHTMLDDA]] SLOT (Annex B.3.6), which only a host can make.
            realm.DefineValue(host, "htmlDda", realm.NewHtmlDdaObject());

            realm.DefineValue(realm.Global, "broilerHost", host);
        }

        private StorageArea? storage;
        private JsHostPromiseCapability? capability;

        private JsHostValue Storage(JsHostRealm realm)
        {
            storage = new StorageArea();
            var area = realm.NewExotic(storage);

            // A NON-CONFIGURABLE ORDINARY PROPERTY whose name the handler also holds an item for,
            // which is the case where the offer and the ordinary deletion disagree.
            realm.DefineValue(area, "fixed", JsHostValue.String("pinned"), JsHostPropertyFlags.None);

            return area;
        }

        /// <summary>A value held across checks, and therefore across realms.</summary>
        private static JsHostValue kept;

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

    /// <summary>
    /// A storage area: it owns the names written to it and takes them away when they are deleted,
    /// which is the one shape the optional deletion hook exists for.
    /// </summary>
    private sealed class StorageArea : IJsHostExotic, IJsHostExoticDeletion
    {
        private readonly Dictionary<string, string> items = new(System.StringComparer.Ordinal)
        {
            ["fixed"] = "an item under a pinned property",
        };

        /// <summary>Every name the deletion hook was offered, claimed or declined, in order.</summary>
        internal List<string> Offered { get; } = [];

        public bool TryGetNamed(JsHostRealm realm, string name, out JsHostValue value)
        {
            // A NAME THE HANDLER ANSWERS AND WILL NOT DELETE, which is what a named property with
            // no deleter is: the deletion is declined and the name goes on answering.
            if (string.Equals(name, "constant", System.StringComparison.Ordinal))
            {
                value = JsHostValue.String("always");
                return true;
            }

            if (items.TryGetValue(name, out var stored))
            {
                value = JsHostValue.String(stored);
                return true;
            }

            value = JsHostValue.Missing;
            return false;
        }

        public bool TryGetIndex(JsHostRealm realm, uint index, out JsHostValue value)
        {
            value = JsHostValue.Missing;
            return false;
        }

        // IT CLAIMS EVERY NAME BUT FIVE, so each check has names the handler owns and names that
        // are ordinary expandos - the two sides the deletion hook must keep apart.
        public bool TrySetNamed(JsHostRealm realm, string name, JsHostValue value)
        {
            if (name is "expando" or "explosive" or "thrower" or "refusing" or "spinning")
            {
                return false;
            }

            items[name] = realm.ToJsString(value);
            return true;
        }

        public uint IndexedLength(JsHostRealm realm) => 0;

        public IReadOnlyList<string> SupportedNames(JsHostRealm realm) => [.. items.Keys];

        public bool TryDeleteNamed(JsHostRealm realm, string name)
        {
            Offered.Add(name);

            if (string.Equals(name, "explosive", System.StringComparison.Ordinal))
            {
                throw realm.Error(JsHostErrorKind.TypeError, "the host refused the deletion");
            }

            if (string.Equals(name, "thrower", System.StringComparison.Ordinal))
            {
                throw realm.Throw(JsHostValue.String("a raw reason"));
            }

            // A REFUSAL AT THE SEAM, reached the way an embedder's mistake is: invoking a value
            // that is not callable.
            if (string.Equals(name, "refusing", System.StringComparison.Ordinal))
            {
                _ = realm.Invoke(JsHostValue.Undefined, JsHostValue.Undefined);
            }

            // A TERMINATION, reached by calling back into a guest function that never returns;
            // the allowance runs out inside the callback and the hook sees the latched abort.
            if (string.Equals(name, "spinning", System.StringComparison.Ordinal))
            {
                _ = realm.Invoke(realm.GetProperty(realm.Global, "spin"), JsHostValue.Undefined);
            }

            return items.Remove(name);
        }
    }

    /// <summary>
    /// An exotic handler every hook of which misbehaves on a name or a mode (VM-FIX-I): it throws a
    /// value or an error, is refused at the seam, answers a value from another realm or a BigInt,
    /// or calls back into a guest function that never returns.
    /// </summary>
    private sealed class HostileElements(string mode, JsHostValue foreign) : IJsHostExotic
    {
        public bool TryGetNamed(JsHostRealm realm, string name, out JsHostValue value)
        {
            switch (name)
            {
                case "thrower":
                    throw realm.Throw(JsHostValue.String("a raw reason"));

                case "erring":
                    throw realm.Error(JsHostErrorKind.TypeError, "the host refused the read");

                case "refusing":
                    _ = realm.Invoke(JsHostValue.Undefined, JsHostValue.Undefined);
                    break;

                case "foreign":
                    value = foreign;
                    return true;

                case "big":
                    value = JsHostValue.BigInt(5);
                    return true;

                case "plain":
                    value = JsHostValue.String("an ordinary answer");
                    return true;

                case "spinning":
                    value = realm.Invoke(realm.GetProperty(realm.Global, "spin"), JsHostValue.Undefined);
                    return true;
            }

            value = JsHostValue.Missing;
            return false;
        }

        public bool TryGetIndex(JsHostRealm realm, uint index, out JsHostValue value)
        {
            if (index == 1)
            {
                throw realm.Throw(JsHostValue.String("an index reason"));
            }

            value = foreign;
            return index == 0;
        }

        public bool TrySetNamed(JsHostRealm realm, string name, JsHostValue value)
        {
            if (string.Equals(name, "setThrow", System.StringComparison.Ordinal))
            {
                throw realm.Throw(JsHostValue.String("a write reason"));
            }

            if (string.Equals(name, "setRefuse", System.StringComparison.Ordinal))
            {
                _ = realm.Invoke(JsHostValue.Undefined, JsHostValue.Undefined);
            }

            return false;
        }

        public uint IndexedLength(JsHostRealm realm)
        {
            if (string.Equals(mode, "length", System.StringComparison.Ordinal))
            {
                _ = realm.Invoke(JsHostValue.Undefined, JsHostValue.Undefined);
            }

            return 0;
        }

        public IReadOnlyList<string> SupportedNames(JsHostRealm realm) =>
            string.Equals(mode, "names", System.StringComparison.Ordinal)
                ? throw realm.Throw(JsHostValue.String("a names reason"))
                : ["plain"];
    }
}
