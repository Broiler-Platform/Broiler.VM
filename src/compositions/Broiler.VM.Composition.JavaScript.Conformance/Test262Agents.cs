// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Concurrent;
using System.Collections.Immutable;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// The suite's <c>$262.agent</c> for one test: the agents it starts, the broadcasts it sends them and
/// the reports they send back (INTERPRETING.md; JSD-0042).
/// </summary>
/// <remarks>
/// <para>
/// <b>An agent is a runtime</b>, as roadmap section 13 says: <c>start</c> compiles its source, builds a
/// runtime from the test's own manifest with a worker's host surface, and runs it on a thread of its
/// own. The shared block a broadcast hands it was charged to the test that made it.
/// </para>
/// <para>
/// <b>A test's agents share one aggregate budget</b>, which is what makes their ceiling shared rather
/// than multiplied: every aggregate dimension is bounded at the test's own runtime ceiling, once for
/// all of them, and at most <see cref="MaxAgents"/> are live. The test's own runtime is not under it -
/// it is created before anything knows whether the test will start an agent - so a test and its agents
/// together may spend two allowances, never more.
/// </para>
/// <para>
/// <b>A broadcast hands the block, not a copy</b>, through <see cref="JsHostRealm.ShareBlock"/> and
/// <see cref="JsHostRealm.AdoptBlock"/>, and returns once every agent has taken it, as INTERPRETING.md
/// says. An agent runs the callback it registered in a host turn and then drains its jobs.
/// </para>
/// <para>
/// <b>Every agent ends with its test</b>: when the test's verdict is reached the agents are cancelled,
/// which ends any wait they are in, joined, and disposed. Both the test's agent and its workers may
/// block, so the runner keeps skipping the suite's <c>CanBlockIsFalse</c> cases.
/// </para>
/// </remarks>
internal sealed class Test262Agents : System.IDisposable
{
    /// <summary>The identity a worker's source presents to the verifier.</summary>
    private const string Caller = "broiler-js-conformance://test262-agent";

    /// <summary>One clock for every agent, so their <c>monotonicNow</c> answers are comparable.</summary>
    private static readonly System.Diagnostics.Stopwatch Clock = System.Diagnostics.Stopwatch.StartNew();

    private readonly Test262Manifest manifest;

    private readonly ulong fuel;

    private readonly ulong wallClock;

    private readonly List<Test262Worker> workers = [];

    private readonly ConcurrentQueue<string> reports = new();

    private readonly CancellationTokenSource stopping = new();

    /// <summary>The aggregate ceilings the agents will share: the test runtime's, and <see cref="MaxAgents"/> live runtimes.</summary>
    private readonly ImmutableArray<VmCeilingSpec> aggregateCeilings;

    /// <summary>The agents' shared budget, made when the first is started.</summary>
    private VmAggregateBudget? aggregate;

    /// <summary>How many agents one test may have live; the suite starts at most four.</summary>
    internal const ulong MaxAgents = 8;

    /// <summary>Creates the agents of one test, under the test's manifest and its runtime's ceilings.</summary>
    internal Test262Agents(Test262Manifest manifest, ulong fuel, ulong wallClock, VmRuntime runtime)
    {
        this.manifest = manifest;
        this.fuel = fuel;
        this.wallClock = wallClock;

        // READ BEFORE THE TEST RUNS, so no host call made during it reads its own runtime's meter.
        var snapshot = runtime.GetBudgetSnapshot();
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (VmBudgetDimensions.CarriesAggregateScope(dimension))
            {
                ceilings.Add(VmCeilingSpec.Value(
                    dimension,
                    dimension == VmBudgetDimension.LiveRuntimes ? MaxAgents : snapshot.EffectiveCeiling(dimension)));
            }
        }

        aggregateCeilings = ceilings.ToImmutable();
    }

    /// <summary>The agents of the test running on this logical call, which flows to its guest threads.</summary>
    internal static AsyncLocal<Test262Agents?> Current { get; } = new();

    /// <summary>Installs the test's <c>$262.agent</c> on <paramref name="harness"/>.</summary>
    internal void InstallMain(JsHostRealm realm, JsHostValue harness)
    {
        var agent = realm.NewObject();

        Define(realm, agent, "start", 1, Start);
        Define(realm, agent, "broadcast", 2, Broadcast);
        Define(realm, agent, "getReport", 0, GetReport);
        Define(realm, agent, "sleep", 1, Sleep);
        Define(realm, agent, "monotonicNow", 0, MonotonicNow);

        realm.DefineValue(
            harness, "agent", agent, JsHostPropertyFlags.Writable | JsHostPropertyFlags.Configurable);
    }

    /// <summary>Installs a worker's <c>$262.agent</c>, whose callback and reports belong to <paramref name="worker"/>.</summary>
    internal void InstallWorker(JsHostRealm realm, Test262Worker worker)
    {
        var harness = realm.GetProperty(realm.Global, "$262");
        var agent = realm.NewObject();

        Define(realm, agent, "receiveBroadcast", 1, (r, _, arguments) =>
        {
            worker.Callback = arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined;
            return JsHostValue.Undefined;
        });

        Define(realm, agent, "report", 1, (r, _, arguments) =>
        {
            reports.Enqueue(r.ToJsString(arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined));
            return JsHostValue.Undefined;
        });

        Define(realm, agent, "leaving", 0, (_, _, _) =>
        {
            worker.Left = true;
            return JsHostValue.Undefined;
        });
        Define(realm, agent, "sleep", 1, Sleep);
        Define(realm, agent, "monotonicNow", 0, MonotonicNow);

        realm.DefineValue(
            harness, "agent", agent, JsHostPropertyFlags.Writable | JsHostPropertyFlags.Configurable);
    }

    /// <summary><c>$262.agent.start(source)</c>: compiles, builds and starts an agent.</summary>
    private JsHostValue Start(JsHostRealm realm, JsHostValue thisValue, ReadOnlySpan<JsHostValue> arguments)
    {
        var source = realm.ToJsString(arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined);
        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("agent", source, SliceParseOptions.Script, false, Caller)],
            [],
            manifest.CompileRequest);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            throw realm.Error(JsHostErrorKind.SyntaxError, "$262.agent.start: the agent's source did not compile");
        }

        var worker = new Test262Worker(this, compiled.Artifact, stopping.Token);
        VmAggregateBudget shared;

        lock (workers)
        {
            shared = aggregate ??= VmAggregateBudget.Create(aggregateCeilings);
        }

        if (worker.Start(manifest, fuel, wallClock, shared) is { } failure)
        {
            worker.Dispose();
            throw realm.Error(JsHostErrorKind.TypeError, "$262.agent.start: " + failure);
        }

        lock (workers)
        {
            workers.Add(worker);
        }

        return JsHostValue.Undefined;
    }

    /// <summary>
    /// <c>$262.agent.broadcast(buffer, id)</c>: hands every agent the buffer's block and returns once each
    /// has taken it.
    /// </summary>
    private JsHostValue Broadcast(JsHostRealm realm, JsHostValue thisValue, ReadOnlySpan<JsHostValue> arguments)
    {
        var block = realm.ShareBlock(arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined);

        // THE ID CROSSES ONLY AS A PRIMITIVE: a number, a BigInt or a string is no realm's.
        var id = arguments.Length > 1 && !arguments[1].IsObject ? arguments[1] : JsHostValue.Undefined;

        Test262Worker[] targets;

        lock (workers)
        {
            targets = [.. workers];
        }

        // NOT DISPOSED HERE: an agent that takes the broadcast after this gave up still signals it,
        // and the event must outlive that late signal. It holds no handle until one is asked for.
        var taken = new CountdownEvent(targets.Length);

        foreach (var worker in targets)
        {
            worker.Post(new Test262Broadcast(block, id, taken));
        }

        try
        {
            if (!taken.Wait(TimeSpan.FromMilliseconds(wallClock), stopping.Token))
            {
                throw realm.Error(JsHostErrorKind.TypeError, "$262.agent.broadcast: an agent did not take the broadcast");
            }
        }
        catch (OperationCanceledException)
        {
            throw realm.Error(JsHostErrorKind.TypeError, "$262.agent.broadcast: the test ended");
        }

        return JsHostValue.Undefined;
    }

    /// <summary>How long <c>getReport</c> waits for a running agent's report before answering <c>null</c>, in milliseconds.</summary>
    private const int ReportWait = 100;

    /// <summary>
    /// <c>$262.agent.getReport()</c>: the oldest report not yet read, or <c>null</c> - after waiting up to
    /// <see cref="ReportWait"/> milliseconds for one while an agent is still running.
    /// </summary>
    /// <remarks>
    /// <b>The wait is a host reading another thread's mailbox, and INTERPRETING.md leaves it the
    /// host's.</b> Without it, a test that asks for a report a moment after the notification that
    /// produces it answers <c>null</c>, and the suite's asynchronous stand-in for <c>setTimeout</c>
    /// then spins promise jobs for a whole second before asking again - which this profile charges to
    /// the operation's live bytes, job by job, until it is spent. A report that never comes is still
    /// <c>null</c>, a tenth of a second later.
    /// </remarks>
    private JsHostValue GetReport(JsHostRealm realm, JsHostValue thisValue, ReadOnlySpan<JsHostValue> arguments)
    {
        if (reports.TryDequeue(out var report))
        {
            return JsHostValue.String(report);
        }

        var until = Clock.ElapsedMilliseconds + ReportWait;

        while (Clock.ElapsedMilliseconds < until && AnyRunning() && !stopping.IsCancellationRequested)
        {
            Thread.Sleep(1);

            if (reports.TryDequeue(out report))
            {
                return JsHostValue.String(report);
            }
        }

        return JsHostValue.Null;
    }

    /// <summary>Whether any agent this test started is still running and has not said it is leaving.</summary>
    private bool AnyRunning()
    {
        lock (workers)
        {
            foreach (var worker in workers)
            {
                if (worker.IsRunning)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary><c>$262.agent.sleep(ms)</c>, for any agent.</summary>
    private static JsHostValue Sleep(JsHostRealm realm, JsHostValue thisValue, ReadOnlySpan<JsHostValue> arguments)
    {
        var milliseconds = realm.ToNumber(arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined);

        if (milliseconds > 0)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(Math.Min(milliseconds, 60_000)));
        }

        return JsHostValue.Undefined;
    }

    /// <summary><c>$262.agent.monotonicNow()</c>, in milliseconds on one clock for every agent.</summary>
    private static JsHostValue MonotonicNow(JsHostRealm realm, JsHostValue thisValue, ReadOnlySpan<JsHostValue> arguments) =>
        JsHostValue.Number(Clock.Elapsed.TotalMilliseconds);

    /// <summary>Defines one method with the attributes every built-in method has.</summary>
    private static void Define(JsHostRealm realm, JsHostValue target, string name, int length, JsHostFunction body) =>
        realm.DefineValue(
            target,
            name,
            realm.NewMethod(name, body, length),
            JsHostPropertyFlags.Writable | JsHostPropertyFlags.Configurable);

    /// <summary>Ends every agent: cancels, joins and disposes them.</summary>
    public void Dispose()
    {
        stopping.Cancel();

        Test262Worker[] started;

        lock (workers)
        {
            started = [.. workers];
            workers.Clear();
        }

        foreach (var worker in started)
        {
            worker.Dispose();
        }

        _ = aggregate?.Dispose();
        stopping.Dispose();
    }
}

/// <summary>One broadcast as an agent receives it.</summary>
internal sealed record Test262Broadcast(JsHostSharedBlock Block, JsHostValue Id, CountdownEvent Taken);

/// <summary>One agent: its runtime and instance, the thread it runs on, and the broadcasts waiting for it.</summary>
internal sealed class Test262Worker : IJsHostSurface, IJsHostAgentPolicy, System.IDisposable
{
    private const string Caller = "broiler-js-conformance://test262-agent";

    private readonly Test262Agents agents;

    private readonly byte[] artifact;

    private readonly CancellationToken stopping;

    private readonly BlockingCollection<Test262Broadcast> inbox = new();

    private VmRuntime? runtime;

    private VmVerifiedArtifact? verified;

    private VmInstance? instance;

    private Thread? thread;

    private Test262Broadcast? pending;

    /// <summary>Set once the agent's thread has built its runtime, or failed to.</summary>
    private readonly ManualResetEventSlim built = new();

    /// <summary>Why the agent's runtime was not built, or nothing.</summary>
    private string? buildFailure;

    internal Test262Worker(Test262Agents agents, byte[] artifact, CancellationToken stopping)
    {
        this.agents = agents;
        this.artifact = artifact;
        this.stopping = stopping;
    }

    /// <summary>Whether the agent called <c>leaving</c>: it will report nothing more.</summary>
    internal bool Left
    {
        get => Volatile.Read(ref left);
        set => Volatile.Write(ref left, value);
    }

    private bool left;

    /// <summary>Whether the agent's thread is running and the agent has not said it is leaving.</summary>
    internal bool IsRunning => !Left && thread is { IsAlive: true };

    /// <summary>The callback the agent's source registered with <c>receiveBroadcast</c>.</summary>
    internal JsHostValue Callback { get; set; } = JsHostValue.Undefined;

    /// <inheritdoc/>
    /// <remarks>An agent the runner starts may block, as a worker may.</remarks>
    public bool CanBlock => true;

    /// <inheritdoc/>
    public void OnRealmCreated(JsHostRealm realm) => agents.InstallWorker(realm, this);

    /// <inheritdoc/>
    /// <remarks>The turn the agent's thread asks for when a broadcast arrives: the callback, with the adopted block.</remarks>
    public void OnTurn(JsHostRealm realm)
    {
        if (pending is not { } message || !Callback.IsObject)
        {
            return;
        }

        pending = null;
        var buffer = realm.AdoptBlock(message.Block);

        try
        {
            _ = realm.Invoke(Callback, JsHostValue.Undefined, [buffer, message.Id]);
        }
        catch (JsHostThrowException)
        {
            // AN AGENT THAT THROWS SIMPLY STOPS REPORTING; the test that waits for it fails on its own
            // terms, as it would in any host.
        }
    }

    /// <summary>Builds the agent's runtime and instance from the test's manifest, under the agents' shared budget.</summary>
    private bool TryBuild(
        Test262Manifest manifest, ulong fuel, ulong wallClock, VmAggregateBudget aggregate, out string failure)
    {
        // A RUN ADMITTING NO OPTIONAL SURFACE NEVER REACHES HERE - it has no `$262` host - and an
        // agent built from an empty list would be admitted every surface its test declined.
        if (manifest.Admitted.Length == 0)
        {
            failure = "the test's composition admits no optional surface, so it has no agents";
            return false;
        }

        var surfaces = new VmFeatureManifestId[manifest.Admitted.Length];

        for (var index = 0; index < surfaces.Length; index++)
        {
            surfaces[index] = VmFeatureManifestId.Parse(manifest.Admitted[index]);
        }

        var catalog = VmCatalog.CreateBuilder()
            .Add(Test262Manifest.Descriptor(this, manifest.Form, surfaces))
            .Build();

        var created = VmRuntime.Create(catalog, Test262Run.Options(manifest, fuel, wallClock, [], aggregate));

        if (!created.TryGetRuntime(out runtime))
        {
            failure = $"the agent's runtime was refused: {created.Outcome}/{created.Reason}";
            return false;
        }

        var descriptor = new VmArtifactDescriptor(
            JavaScriptProfile.Id,
            manifest.FormatVersion,
            manifest.Id,
            default,
            VmCallerIdentity.FromCanonicalIdentity(Caller));

        var checkedArtifact = runtime.Verify(in descriptor, artifact, CancellationToken.None);

        if (!checkedArtifact.TryGetArtifact(out verified))
        {
            failure = $"the agent's artifact was refused: {checkedArtifact.Outcome}/{checkedArtifact.Reason}";
            return false;
        }

        var instantiated = runtime.Instantiate(verified, CancellationToken.None);

        if (!instantiated.TryGetInstance(out instance))
        {
            failure = $"the agent would not instantiate: {instantiated.Outcome}/{instantiated.Reason}";
            return false;
        }

        failure = string.Empty;
        return true;
    }

    /// <summary>
    /// Starts the agent's thread, which builds its runtime and runs its source, and answers once the
    /// runtime is built: nothing, or why it was not.
    /// </summary>
    /// <remarks>
    /// <b>The agent's thread inherits nothing of the test's.</b> The core keeps an operation's meter in
    /// the logical call context, and a thread started inside the test's host call would otherwise carry
    /// the test's operation into everything the agent builds and runs; so the flow is suppressed, and
    /// the agent builds its own runtime on its own thread.
    /// </remarks>
    internal string? Start(Test262Manifest manifest, ulong fuel, ulong wallClock, VmAggregateBudget aggregate)
    {
        thread = new Thread(() => Run(manifest, fuel, wallClock, aggregate))
        {
            IsBackground = true,
            Name = "test262-agent",
        };

        using (ExecutionContext.SuppressFlow())
        {
            thread.Start();
        }

        try
        {
            return built.Wait(TimeSpan.FromMilliseconds(wallClock), stopping)
                ? buildFailure
                : "the agent's runtime was not built within the test's wall clock";
        }
        catch (OperationCanceledException)
        {
            return "the test ended";
        }
    }

    /// <summary>Queues a broadcast for the agent.</summary>
    internal void Post(Test262Broadcast message)
    {
        try
        {
            inbox.Add(message, stopping);
        }
        catch (OperationCanceledException)
        {
        }
        catch (InvalidOperationException)
        {
        }
    }

    /// <summary>What ended the agent's thread when it was not the test's end, for the transcript.</summary>
    internal string? Failure { get; private set; }

    /// <summary>The agent's thread: its source, then a turn and a drain for each broadcast it takes.</summary>
    /// <remarks>
    /// <b>Nothing escapes it.</b> An exception on a thread of its own would end the runner's process
    /// and every other test of its shard with it, so whatever ends an agent early is kept as its
    /// <see cref="Failure"/> and the agent simply stops, as one that threw would in any host.
    /// </remarks>
    private void Run(Test262Manifest manifest, ulong fuel, ulong wallClock, VmAggregateBudget aggregate)
    {
        bool ok;
        string failure;

        try
        {
            ok = TryBuild(manifest, fuel, wallClock, aggregate, out failure);
        }
        catch (Exception thrown)
        {
            ok = false;
            failure = thrown.GetType().Name + ": " + thrown.Message;
        }

        if (!ok)
        {
            buildFailure = failure;
        }

        built.Set();

        if (!ok)
        {
            return;
        }

        try
        {
            // THE AGENT'S SOURCE, THEN ITS JOBS: a source may await before it registers anything, and
            // what it awaits runs only in a drain.
            if (!Invoke("agent") || !Invoke(JavaScriptProfile.DrainEntryPoint))
            {
                return;
            }

            while (!stopping.IsCancellationRequested)
            {
                Test262Broadcast message;

                try
                {
                    message = inbox.Take(stopping);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (InvalidOperationException)
                {
                    return;
                }

                pending = message;
                message.Taken.Signal();

                if (!Invoke(JavaScriptProfile.TurnEntryPoint) || !Invoke(JavaScriptProfile.DrainEntryPoint))
                {
                    return;
                }
            }
        }
        catch (Exception thrown)
        {
            Failure = thrown.GetType().Name + ": " + thrown.Message;
        }
    }

    /// <summary>Invokes one entry point of the agent's instance, answering whether the agent should go on.</summary>
    private bool Invoke(string entry)
    {
        var request = new VmInvocationRequest(new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(entry)));

        try
        {
            var result = instance!.Invoke(in request, stopping);
            return result.Outcome is VmOutcome.Normal or VmOutcome.ProfileFault;
        }
        catch (ObjectDisposedException)
        {
            return false;
        }
    }

    /// <summary>Waits for the agent's thread, then disposes its instance and runtime.</summary>
    public void Dispose()
    {
        inbox.CompleteAdding();
        thread?.Join(TimeSpan.FromSeconds(10));
        instance?.Dispose();
        verified?.Dispose();
        runtime?.Dispose();
        inbox.Dispose();
        built.Dispose();
    }
}
