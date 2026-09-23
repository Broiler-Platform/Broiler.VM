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
/// The checks for detached clone adoption across realms and threads (JSeal I17):
/// <c>DetachClone</c> on one runtime's thread, <c>AdoptClone</c> on another's.
/// </summary>
/// <remarks>
/// <para>
/// <b>Every runtime here lives on one host thread of its own, from creation to disposal</b>, and
/// is reached only through the turn model the rest of this lane uses: a program, then a
/// <c>#host-turn</c> invocation in which the embedder does its work, then another program. The
/// carrier is the only thing handed from one host thread to another, through
/// <see cref="Thread.Join()"/>, which is a synchronising handoff.
/// </para>
/// <para>
/// <b>What the destination's objects are is judged by the destination's guest</b>: prototypes are
/// compared with its own intrinsics and identities with its own <c>===</c>. What only the host can
/// see - which thread a turn ran on, a refusal, a budget snapshot - is recorded as a line and
/// compared beside what the guest printed.
/// </para>
/// </remarks>
internal static partial class HostSurfaceChecks
{
    /// <summary>The graph the sending realm builds: every brand, a cycle, a shared object, holes.</summary>
    private const string CloneSource =
        """
        var shared = { tag: 'shared' };
        var buf = new ArrayBuffer(8);
        var u8 = new Uint8Array(buf);
        for (var i = 0; i < 8; i++) { u8[i] = i + 1; }
        var sent = {
            value: 'original', a: shared, b: shared,
            list: [1, , 3], when: new Date(86400000), re: /ab+c/gi,
            map: new Map([['k', shared]]), set: new Set([shared]),
            err: new RangeError('out of range'),
            u8: u8, dv: new DataView(buf, 2, 4),
        };
        sent.self = sent;
        """;

    /// <summary>What the receiving realm asks of what it was handed, in its own vocabulary.</summary>
    private const string CloneJudge =
        """
        print('protos=' + [
            Object.getPrototypeOf(got) === Object.prototype,
            Array.isArray(got.list) && Object.getPrototypeOf(got.list) === Array.prototype,
            got.when instanceof Date, got.re instanceof RegExp, got.map instanceof Map,
            got.set instanceof Set, got.err instanceof RangeError, got.u8 instanceof Uint8Array,
            got.dv instanceof DataView, got.u8.buffer instanceof ArrayBuffer].join(','));
        print('identity=' + [got.self === got, got.a === got.b, got.map.get('k') === got.a,
            got.set.has(got.a), got.u8.buffer === got.dv.buffer].join(','));
        print('values=' + [got.value, got.list.length, 1 in got.list, got.when.getTime(),
            got.re.source + '/' + got.re.flags, got.err.message, got.u8.join(''),
            got.dv.getUint8(0)].join(','));
        """;

    /// <summary>Runs the clone-adoption checks, answering the failure count.</summary>
    private static int RunClone()
    {
        var failures = 0;

        failures += CheckCloneAcrossThreads();
        failures += CheckCloneTransferIsSingleUse();
        failures += CheckCloneRefusals();
        failures += CheckCloneBudget();

        return failures;
    }

    /// <summary>
    /// Serialize on thread A, dispose A, adopt on thread B, and adopt again twice on thread C.
    /// </summary>
    private static int CheckCloneAcrossThreads()
    {
        const string Title =
            "a clone detached on one thread is adopted on two others after its source is disposed, realm-locally, repeatably";

        var lines = new List<string>();
        JsHostCloneCarrier? carrier = null;
        var turnThreads = new int[3];

        // THREAD A: build, detach in a turn, mutate the source afterwards, and dispose it all.
        var sending = OnThread(() =>
        {
            using var party = CloneParty.Open(
                [("make", CloneSource), ("mutate", "sent.value = 'mutated'; shared.tag = 'mutated'; u8[0] = 99;")],
                out var failure);

            if (party is null)
            {
                return failure;
            }

            return party.Run("make")
                ?? party.Turn(realm =>
                {
                    turnThreads[0] = System.Environment.CurrentManagedThreadId;
                    carrier = realm.DetachClone(realm.GetProperty(realm.Global, "sent"));
                })
                ?? party.Run("mutate");
        });

        if (sending is not null || carrier is null)
        {
            return Report(Title, "the sending thread: " + (sending ?? "no carrier"));
        }

        // THE SOURCE IS GONE: its instance, artifact and runtime were disposed on thread A.
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        lines.Add("single-use=" + carrier.IsSingleUse + ",charged=" + (carrier.ChargedBytes > 0));

        // THREAD B: adopt once and judge it.
        var receiving = OnThread(() =>
        {
            using var party = CloneParty.Open([("judge", CloneJudge)], out var failure);

            if (party is null)
            {
                return failure;
            }

            var outcome = party.Turn(realm =>
                {
                    turnThreads[1] = System.Environment.CurrentManagedThreadId;
                    realm.DefineValue(realm.Global, "got", realm.AdoptClone(carrier!));
                })
                ?? party.Run("judge");

            lines.AddRange(party.Printed);
            return outcome;
        });

        if (receiving is not null)
        {
            return Report(Title, "the receiving thread: " + receiving);
        }

        // THREAD C: the same carrier twice more, into one realm, and the two share nothing.
        var again = OnThread(() =>
        {
            using var party = CloneParty.Open(
                [(
                    "compare",
                    """
                    first.u8[1] = 42; first.a.tag = 'changed';
                    print('twice=' + [first !== second, first.a !== second.a,
                        first.u8.buffer !== second.u8.buffer, second.u8[1], second.a.tag,
                        Object.getPrototypeOf(second.map) === Map.prototype].join(','));
                    """)],
                out var failure);

            if (party is null)
            {
                return failure;
            }

            var outcome = party.Turn(realm =>
                {
                    turnThreads[2] = System.Environment.CurrentManagedThreadId;
                    realm.DefineValue(realm.Global, "first", realm.AdoptClone(carrier!));
                    realm.DefineValue(realm.Global, "second", realm.AdoptClone(carrier!));
                })
                ?? party.Run("compare");

            lines.AddRange(party.Printed);
            return outcome;
        });

        if (again is not null)
        {
            return Report(Title, "the third thread: " + again);
        }

        lines.Add("threads=" + (turnThreads[0] != turnThreads[1]
            && turnThreads[1] != turnThreads[2]
            && turnThreads[0] != turnThreads[2]
            && turnThreads[0] != 0));

        return Compare(
            Title,
            lines,
            [
                "single-use=False,charged=True",
                "protos=true,true,true,true,true,true,true,true,true,true",
                "identity=true,true,true,true,true",
                "values=original,3,false,86400000,ab+c/gi,out of range,12345678,3",
                "twice=true,true,true,2,shared,true",
                "threads=True",
            ]);
    }

    /// <summary>
    /// A carrier with transferred bytes: the source is detached at once, one of two racing
    /// adoptions on two threads wins, and every later one is refused.
    /// </summary>
    private static int CheckCloneTransferIsSingleUse()
    {
        const string Title =
            "a carrier with transferred bytes detaches its source at once and is adopted exactly once across racing threads";

        var lines = new List<string>();
        JsHostCloneCarrier? carrier = null;

        var sending = OnThread(() =>
        {
            using var party = CloneParty.Open(
                [
                    ("make", "var buf = new ArrayBuffer(8); var view = new Uint8Array(buf); view[7] = 77; var msg = { buf: buf, view: view };"),
                    ("after", "print('source=' + buf.byteLength + ':' + view.length);"),
                ],
                out var failure);

            if (party is null)
            {
                return failure;
            }

            var outcome = party.Run("make")
                ?? party.Turn(realm =>
                {
                    var buffer = realm.GetProperty(realm.Global, "buf");
                    carrier = realm.DetachClone(realm.GetProperty(realm.Global, "msg"), [buffer]);
                })
                ?? party.Run("after");

            lines.AddRange(party.Printed);
            return outcome;
        });

        if (sending is not null || carrier is null)
        {
            return Report(Title, "the sending thread: " + (sending ?? "no carrier"));
        }

        lines.Add("single-use=" + carrier.IsSingleUse + ",consumed=" + carrier.IsConsumed);

        // TWO RECEIVERS, EACH ON ITS OWN THREAD WITH ITS OWN RUNTIME, released together into their
        // turns. Whichever claims first wins; the other must be refused, never handed an empty buffer.
        var outcomes = new string[2];
        var printed = new List<string>[2];
        using var start = new Barrier(2);

        var racers = new Thread[2];

        for (var index = 0; index < 2; index++)
        {
            var at = index;
            racers[at] = new Thread(() =>
            {
                using var party = CloneParty.Open(
                    [("read", "print('got=' + got.buf.byteLength + ':' + got.view[7] + ':' + (got.view.buffer === got.buf));")],
                    out var failure);

                if (party is null)
                {
                    start.RemoveParticipant();
                    outcomes[at] = "open: " + failure;
                    printed[at] = [];
                    return;
                }

                var result = "not reached";
                start.SignalAndWait();

                var turn = party.Turn(realm =>
                {
                    try
                    {
                        realm.DefineValue(realm.Global, "got", realm.AdoptClone(carrier!));
                        result = "adopted";
                    }
                    catch (JsHostSurfaceException refusal)
                    {
                        result = refusal.Refusal.ToString();
                    }
                });

                if (turn is null && result == "adopted")
                {
                    turn = party.Run("read");
                }

                outcomes[at] = turn ?? result;
                printed[at] = [.. party.Printed];
            });
        }

        foreach (var racer in racers)
        {
            racer.Start();
        }

        foreach (var racer in racers)
        {
            racer.Join();
        }

        var winners = 0;
        var refused = 0;

        for (var at = 0; at < 2; at++)
        {
            if (outcomes[at] == "adopted")
            {
                winners++;
                lines.AddRange(printed[at]);
            }
            else if (outcomes[at] == nameof(JsHostRefusal.CarrierConsumed))
            {
                refused++;
            }
            else
            {
                return Report(Title, $"racer {at} answered {outcomes[at]}");
            }
        }

        lines.Add("race=" + winners + " adopted," + refused + " refused");

        // AND ONCE MORE, LATER, ON A THIRD THREAD: still refused, before anything is built.
        var late = OnThread(() =>
        {
            using var party = CloneParty.Open([("noop", "print('unused');")], out var failure);

            if (party is null)
            {
                return failure;
            }

            return party.Turn(realm => lines.Add("late=" + Refused(() => realm.AdoptClone(carrier!))));
        });

        if (late is not null)
        {
            return Report(Title, "the late thread: " + late);
        }

        lines.Add("consumed=" + carrier.IsConsumed);

        return Compare(
            Title,
            lines,
            [
                "source=0:0",
                "single-use=True,consumed=False",
                "got=8:77:true",
                "race=1 adopted,1 refused",
                "late=CarrierConsumed",
                "consumed=True",
            ]);
    }

    /// <summary>
    /// Foreign objects are refused; outside a step, and from another thread during one, nothing is
    /// done and nothing is claimed.
    /// </summary>
    private static int CheckCloneRefusals()
    {
        const string Title =
            "a foreign carrier is refused, and a crossing outside the realm's step or thread claims nothing";

        var lines = new List<string>();
        JsHostCloneCarrier? carrier = null;

        var failure = OnThread(() =>
        {
            using var source = CloneParty.Open(
                [("make", "var b = new ArrayBuffer(4); new Uint8Array(b)[0] = 5; var msg = { b: b };")],
                out var sourceFailure);

            if (source is null)
            {
                return sourceFailure;
            }

            var made = source.Run("make")
                ?? source.Turn(realm => carrier = realm.DetachClone(
                    realm.GetProperty(realm.Global, "msg"), [realm.GetProperty(realm.Global, "b")]));

            if (made is not null || carrier is null)
            {
                return "the source: " + (made ?? "no carrier");
            }

            using var party = CloneParty.Open(
                [
                    ("first", "print('first');"),
                    ("read", "print('adopted=' + got.b.byteLength + ':' + new Uint8Array(got.b)[0]);"),
                ],
                out var partyFailure);

            if (party is null)
            {
                return partyFailure;
            }

            var ran = party.Run("first");

            if (ran is not null)
            {
                return ran;
            }

            // BETWEEN INVOCATIONS: the realm is held but not current, and the single-use carrier
            // offered here must still be whole for the turn below.
            var realm = party.Surface.Realm!;
            lines.Add("outside-adopt=" + Refused(() => realm.AdoptClone(carrier!)));
            lines.Add("outside-detach=" + Refused(() => realm.DetachClone(JsHostValue.Number(1)).FormatVersion));

            return party.Turn(turn =>
                {
                    // MISSING IS NOT A VALUE: refused as the argument mistake it is, as the value or
                    // as a transfer entry, and nothing is detached.
                    lines.Add("missing=" + Misused(() => turn.DetachClone(JsHostValue.Missing)));
                    lines.Add("missing-transfer=" + Misused(() => turn.DetachClone(
                        JsHostValue.Undefined, [JsHostValue.Missing])));

                    lines.Add("foreign-object=" + Refused(() => turn.AdoptClone(new ForeignCarrier())));
                    lines.Add("foreign-text=" + Refused(() => turn.AdoptClone("a carrier, allegedly")));

                    try
                    {
                        turn.AdoptClone(null!);
                        lines.Add("null=not refused");
                    }
                    catch (System.ArgumentNullException refusal)
                    {
                        lines.Add("null=" + refusal.ParamName);
                    }

                    // FROM ANOTHER THREAD WHILE THIS ONE HOLDS THE STEP: refused, and unclaimed.
                    string? elsewhere = null;
                    var intruder = new Thread(() => elsewhere = Refused(() => turn.AdoptClone(carrier!)));
                    intruder.Start();
                    intruder.Join();
                    lines.Add("other-thread=" + elsewhere);
                    lines.Add("still-whole=" + !carrier.IsConsumed);

                    lines.Add("format=" + (carrier.FormatVersion == JsHostCloneCarrier.CurrentFormatVersion)
                        + "," + (carrier.Profile == JavaScriptProfile.Id));

                    turn.DefineValue(turn.Global, "got", turn.AdoptClone(carrier!));
                })
                ?? party.Run("read")
                ?? Append(lines, party.Printed);
        });

        if (failure is not null)
        {
            return Report(Title, failure);
        }

        return Compare(
            Title,
            lines,
            [
                "outside-adopt=RealmNotCurrent",
                "outside-detach=RealmNotCurrent",
                "missing=ArgumentException:value",
                "missing-transfer=ArgumentException:transfer",
                "foreign-object=ForeignCarrier",
                "foreign-text=ForeignCarrier",
                "null=carrier",
                "other-thread=RealmNotCurrent",
                "still-whole=True",
                "format=True,True",
                "first",
                "adopted=4:5",
            ]);
    }

    /// <summary>
    /// The sender's live bytes pay for the carrier and the receiver's for the buffers it rebuilds;
    /// a ceiling that refuses the carrier ends the turn and detaches nothing.
    /// </summary>
    private static int CheckCloneBudget()
    {
        const string Title =
            "the sender's live bytes pay for a carrier, the receiver's for what it rebuilds, and a carrier the ceiling refuses ends the turn";

        var lines = new List<string>();
        JsHostCloneCarrier? copied = null;
        JsHostCloneCarrier? moved = null;

        var failure = OnThread(() =>
        {
            using var sender = CloneParty.Open(
                [("make", "var text = 'x'.repeat(1000); var bytes = new ArrayBuffer(4096); var t = new ArrayBuffer(4096); var msg = { text: text, bytes: bytes };")],
                out var senderFailure,
                withParent: true);

            if (sender is null)
            {
                return senderFailure;
            }

            var made = sender.Run("make");

            if (made is not null)
            {
                return made;
            }

            // ONE TURN, ONE CARRIER, AND THE PARENT'S LIVE BYTES MOVE BY EXACTLY ITS CHARGE.
            var before = sender.LiveBytes();
            made = sender.Turn(realm => copied = realm.DetachClone(realm.GetProperty(realm.Global, "msg")));
            var delta = sender.LiveBytes() - before;

            if (made is not null || copied is null)
            {
                return "the copying turn: " + (made ?? "no carrier");
            }

            lines.Add("copy=" + (delta == (ulong)copied.ChargedBytes) + "," + (copied.ChargedBytes >= 4096 + 2000));

            before = sender.LiveBytes();
            made = sender.Turn(realm =>
            {
                var t = realm.GetProperty(realm.Global, "t");
                moved = realm.DetachClone(t, [t]);
            });
            delta = sender.LiveBytes() - before;

            if (made is not null || moved is null)
            {
                return "the moving turn: " + (made ?? "no carrier");
            }

            // MOVED BYTES ARE NOT CHARGED TWICE: the sender retained them when it made the buffer.
            lines.Add("move=" + (delta == (ulong)moved.ChargedBytes) + "," + (moved.ChargedBytes < 4096));

            using var receiver = CloneParty.Open(
                [("read", "print('rebuilt=' + a.text.length + ':' + a.bytes.byteLength + ':' + b.byteLength);")],
                out var receiverFailure,
                withParent: true);

            if (receiver is null)
            {
                return receiverFailure;
            }

            before = receiver.LiveBytes();
            made = receiver.Turn(realm =>
            {
                realm.DefineValue(realm.Global, "a", realm.AdoptClone(copied!));
                realm.DefineValue(realm.Global, "b", realm.AdoptClone(moved!));
            });
            delta = receiver.LiveBytes() - before;

            if (made is not null)
            {
                return "the adopting turn: " + made;
            }

            lines.Add("receiver=" + delta);
            return receiver.Run("read") ?? Append(lines, receiver.Printed);
        });

        if (failure is not null)
        {
            return Report(Title, failure);
        }

        // A CEILING THE CARRIER DOES NOT FIT UNDER. The copy is about three mebibytes against a
        // four-mebibyte ceiling that already holds the source's three: the turn ends as an
        // exhaustion, and the host's catch clause buys it nothing.
        failure = OnThread(() =>
        {
            using var party = CloneParty.Open(
                [
                    ("make", "var keep = new ArrayBuffer(3145728); var t = new ArrayBuffer(8); new Uint8Array(t)[0] = 9;"),
                    ("after", "print('after=' + t.byteLength + ':' + new Uint8Array(t)[0] + ':' + keep.byteLength);"),
                ],
                out var partyFailure,
                liveBytes: 4L * 1024 * 1024);

            if (party is null)
            {
                return partyFailure;
            }

            var made = party.Run("make");

            if (made is not null)
            {
                return made;
            }

            var outcome = party.TurnOutcome(realm =>
            {
                try
                {
                    var t = realm.GetProperty(realm.Global, "t");
                    var keep = realm.GetProperty(realm.Global, "keep");
                    _ = realm.DetachClone(realm.NewArray([keep, t]), [t]);
                    lines.Add("refused=no");
                }
                catch (JsHostTerminatedException)
                {
                    lines.Add("refused=terminated");
                }
            });

            // THE INSTANCE IS SPENT, so the still-attached buffer cannot be read back through it:
            // what this check can see is that the refusal ended the turn and nothing ran after it.
            // That no buffer was detached rests on the order in the serializer (the charge comes
            // before the commit loop), as the binary checks' resize refusal does.
            lines.Add("outcome=" + outcome);
            lines.Add("after=" + (party.Run("after") ?? "ran"));
            return Append(lines, party.Printed);
        });

        if (failure is not null)
        {
            return Report(Title, failure);
        }

        return Compare(
            Title,
            lines,
            [
                "copy=True,True",
                "move=True,True",
                "receiver=8192",
                "rebuilt=1000:4096:4096",
                "refused=terminated",
                "outcome=ResourceExhaustion",
                "after=invoking 'after' answered InvalidState/TerminalFault",
            ]);
    }

    /// <summary>Runs <paramref name="body"/> on a new host thread and waits for its answer.</summary>
    private static string? OnThread(System.Func<string?> body)
    {
        string? answer = "the thread never answered";

        var thread = new Thread(() =>
        {
            try
            {
                answer = body();
            }
            catch (System.Exception failure)
            {
                answer = "threw " + failure.GetType().Name + ": " + failure.Message;
            }
        });

        thread.Start();
        thread.Join();
        return answer;
    }

    /// <summary>Names the host-surface refusal an action met, or says it was not refused.</summary>
    private static string Refused<T>(System.Func<T> action)
    {
        try
        {
            _ = action();
            return "not refused";
        }
        catch (JsHostSurfaceException refusal)
        {
            return refusal.Refusal.ToString();
        }
    }

    /// <summary>
    /// Names the argument refusal a member met, the other exception it threw, or says it was not
    /// refused.
    /// </summary>
    private static string Misused<T>(System.Func<T> action)
    {
        try
        {
            _ = action();
            return "not refused";
        }
        catch (System.ArgumentException refusal)
        {
            return refusal.GetType().Name + ":" + refusal.ParamName;
        }
        catch (System.Exception other)
        {
            return other.GetType().Name;
        }
    }

    /// <summary>Appends what a party printed, answering no complaint.</summary>
    private static string? Append(List<string> lines, IEnumerable<string> printed)
    {
        lines.AddRange(printed);
        return null;
    }

    /// <summary>Something another engine might have minted: not a carrier of this profile build.</summary>
    private sealed class ForeignCarrier
    {
    }

    /// <summary>
    /// One runtime, one artifact and one instance, owned by the host thread that opened them.
    /// </summary>
    private sealed class CloneParty : System.IDisposable
    {
        private readonly VmAggregateBudget? parent;
        private readonly VmRuntime runtime;
        private readonly VmVerifiedArtifact artifact;
        private readonly VmInstance instance;

        private CloneParty(
            VmAggregateBudget? parent,
            VmRuntime runtime,
            VmVerifiedArtifact artifact,
            VmInstance instance,
            DemonstrationSurface surface,
            List<string> printed)
        {
            this.parent = parent;
            this.runtime = runtime;
            this.artifact = artifact;
            this.instance = instance;
            Surface = surface;
            Printed = printed;
        }

        internal DemonstrationSurface Surface { get; }

        internal List<string> Printed { get; }

        /// <summary>Compiles the units, and creates a runtime and an instance for them.</summary>
        internal static CloneParty? Open(
            (string Name, string Source)[] units,
            out string failure,
            bool withParent = false,
            long? liveBytes = null,
            VmFeatureManifestId[]? surfaces = null)
        {
            var scripts = new List<JsScriptUnit>();

            foreach (var (name, source) in units)
            {
                scripts.Add(new JsScriptUnit(name, source, SliceParseOptions.Script, false, Caller));
            }

            var compiled = JsCompiler.Compile([.. scripts], [], new JsCompileRequest());

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                failure = "the source was refused: " + Render(compiled);
                return null;
            }

            var printed = new List<string>();
            var surface = new DemonstrationSurface();
            var catalog = VmCatalog.CreateBuilder()
                .Add(JavaScriptProfile.DescriptorHostingRealms(surface, surfaces ?? []))
                .Build();

            var parent = withParent ? VmAggregateBudget.Create(ParentCeilings()) : null;
            var options = Options(
                printed,
                permitted: true,
                liveBytes is { } ceiling
                    ? new Dictionary<VmBudgetDimension, ulong> { [VmBudgetDimension.LiveBytes] = (ulong)ceiling }
                    : null);

            if (parent is not null)
            {
                options = UnderParent(options, parent);
            }

            var created = VmRuntime.Create(catalog, options);

            if (!created.TryGetRuntime(out var runtime))
            {
                parent?.Dispose();
                failure = $"the runtime refused creation: {created.Outcome}/{created.Reason}";
                return null;
            }

            var descriptor = new VmArtifactDescriptor(
                JavaScriptProfile.Id,
                Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
                JavaScriptProfile.WideManifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity(Caller));

            var verified = runtime.Verify(in descriptor, compiled.Artifact, CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                runtime.Dispose();
                parent?.Dispose();
                failure = $"verification refused: {verified.Outcome}/{verified.Reason}";
                return null;
            }

            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                artifact.Dispose();
                runtime.Dispose();
                parent?.Dispose();
                failure = $"instantiation refused: {instantiated.Outcome}/{instantiated.Reason}";
                return null;
            }

            failure = string.Empty;
            return new CloneParty(parent, runtime, artifact, instance, surface, printed);
        }

        /// <summary>Runs one program, answering a complaint or null.</summary>
        internal string? Run(string entryPoint) => HostSurfaceChecks.Run(instance, entryPoint);

        /// <summary>Gives the embedder one turn, answering a complaint or null.</summary>
        internal string? Turn(System.Action<JsHostRealm> work)
        {
            var outcome = TurnOutcome(work);
            return outcome is VmOutcome.Normal ? null : "the turn answered " + outcome;
        }

        /// <summary>Gives the embedder one turn, answering its outcome.</summary>
        internal VmOutcome TurnOutcome(System.Action<JsHostRealm> work)
        {
            Surface.Pending = work;

            var request = new VmInvocationRequest(
                new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(JavaScriptProfile.TurnEntryPoint)));

            return instance.Invoke(in request, CancellationToken.None).Outcome;
        }

        /// <summary>The live bytes the parent budget has recorded for this runtime.</summary>
        internal ulong LiveBytes() => parent!.GetSnapshot().Consumed(VmBudgetDimension.LiveBytes);

        public void Dispose()
        {
            instance.Dispose();
            artifact.Dispose();
            runtime.Dispose();
            parent?.Dispose();
        }
    }

    /// <summary>A parent with room on every aggregate dimension, read only for its live bytes.</summary>
    private static ImmutableArray<VmCeilingSpec> ParentCeilings()
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (VmBudgetDimensions.CarriesAggregateScope(dimension))
            {
                ceilings.Add(VmCeilingSpec.Value(dimension, ulong.MaxValue / 4));
            }
        }

        return ceilings.ToImmutable();
    }

    /// <summary>The same options, under a parent whose remaining the aggregate dimensions adopt.</summary>
    private static VmRuntimeCreationOptions UnderParent(VmRuntimeCreationOptions options, VmAggregateBudget parent)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(VmBudgetDimensions.CarriesAggregateScope(dimension)
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        return new VmRuntimeCreationOptions(
            aggregateBudget: parent,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 1,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: options.Capabilities);
    }
}
