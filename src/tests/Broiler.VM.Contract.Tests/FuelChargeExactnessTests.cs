// xUnit1031 says to make the test async and await instead of blocking. It is suppressed here for
// the same reason as in ConcurrencyTests, and for one more. The subject of this file is what a
// budget reads WHILE a step is held, so the test thread is a participant rather than an observer:
// it holds the rendezvous, reads the budget against a step that cannot advance, and then releases
// it. The tasks it waits on are, by construction, unable to finish until this thread releases the
// gate, so the blocking is the ordering the test is asserting rather than an accident of style.
// Every wait carries a patience, so a step that never arrives fails the test instead of hanging
// the suite.
#pragma warning disable xUnit1031

using System.Collections.Concurrent;
using System.Collections.Immutable;
using Broiler.VM;
using Broiler.VM.Fixtures;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// What a fuel charge costs, to the unit, at every scope that can refuse one.
/// </summary>
/// <remarks>
/// <para>
/// <b>These tests are an oracle, and their oracle is order.</b> They are written and committed
/// against the metering path as it stands, and they must pass here before anything about how a
/// charge is applied changes. The same tests, unchanged, must then pass afterwards. Rule A10
/// forbids <c>InternalsVisibleTo</c> in a product project, so no test can reach inside the meter
/// and switch a mechanism off; what a test can do is state the figure the contract promises and
/// refuse to let it move.
/// </para>
/// <para>
/// Every expected figure below is an instruction count. The fixture profile charges exactly one
/// unit per instruction at the top of its dispatch loop, before the opcode runs, plus whatever a
/// spin charges in bulk, and it charges nothing at all at instantiation. So a fuel figure here is
/// a count of instructions that ran, which is a fact about the artifact rather than about the
/// meter - and that is what makes it safe to hold constant across a change to the meter.
/// </para>
/// <para>
/// Most of these tests carry a note naming the injected defect they are meant to catch. A test
/// whose assertions would survive the defect it names is not evidence of anything, so the figures
/// are chosen to differ under the defect rather than merely to be correct today.
/// </para>
/// <para>
/// T21 and T22 are about the capability boundary rather than about a charge. They live here because
/// the environment meter's answer is held across a capability call, keyed on the execution context,
/// and what a capability's return leaves installed decides whether the step's next charge can still
/// use that answer - so a change made for the sake of a charge's cost reaches that boundary too.
/// </para>
/// </remarks>
public sealed class FuelChargeExactnessTests
{
    /// <summary>
    /// How long a test waits for a rendezvous before calling it a deadlock.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A bounded wait rather than an unbounded one, for the reason the execution gate itself gives:
    /// a rendezvous that is never reached means the path under test did not run, and reporting that
    /// as a timeout is more useful than hanging the suite.
    /// </para>
    /// <para>
    /// It is a fraction of <see cref="FixtureExecutionGate.SelfRelease"/> rather than a figure of
    /// its own, and materially below it. The gate's countdown starts when the step arrives, not
    /// when this thread notices, so a patience equal to it would let a held step continue while a
    /// test that had waited nearly that long still believed it was held - and the run would then
    /// fail on a figure that moved instead of on the timeout the gate promises. A third leaves the
    /// rest of the gate's wait to the reads a held test performs.
    /// </para>
    /// </remarks>
    private static readonly TimeSpan Patience = FixtureExecutionGate.SelfRelease / 3;

    private static ulong Fuel(VmRuntime runtime) =>
        runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.Fuel);

    private static VmCatalog Windowed(FixtureExecutionGate? gate = null) =>
        FixtureComposition.Catalog(gate is null
            ? FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.WindowedPolling)
            : FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.WindowedPolling, gate));

    private static VmCatalog Observing(
        FixtureVmProfileVariant variant,
        Action<IVmExecutionEnvironment>? environmentObserver = null,
        Action<FixtureVmExecutor>? executorObserver = null) =>
        FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(variant, environmentObserver, executorObserver));

    /// <summary>Waits until <paramref name="count"/> steps are held at a gate.</summary>
    private static bool WaitForEntries(FixtureExecutionGate gate, int count)
    {
        var deadline = DateTime.UtcNow + Patience;

        while (gate.Entries < count)
        {
            if (DateTime.UtcNow > deadline)
            {
                return false;
            }

            Thread.Yield();
        }

        return true;
    }

    private static VmInstance Instantiate(VmRuntime runtime, VmVerifiedArtifact artifact, ulong instanceFuel)
    {
        var result = instanceFuel == 0
            ? runtime.Instantiate(artifact, CancellationToken.None)
            : runtime.Instantiate(
                artifact, VmLimitOverrides.Of(VmBudgetDimension.Fuel, instanceFuel), CancellationToken.None);

        Assert.True(result.IsSuccess, $"instantiation failed: {result.Outcome}/{result.Reason}");
        Assert.True(result.TryGetInstance(out var instance));

        return instance;
    }

    private static VmInvocationResult Invoke(VmInstance instance, ulong invocationFuel)
    {
        var request = new VmInvocationRequest(new VmUtf8Text("main"u8.ToArray()));

        return invocationFuel == 0
            ? instance.Invoke(in request, CancellationToken.None)
            : instance.Invoke(
                in request, VmLimitOverrides.Of(VmBudgetDimension.Fuel, invocationFuel), CancellationToken.None);
    }

    // ---- T1, T1b: a ceiling is spent to the unit ------------------------------------------

    /// <summary>
    /// A runtime fuel ceiling is spent exactly, whatever its size relative to the poll window.
    /// </summary>
    /// <remarks>
    /// The ceilings straddle the sixty-four-unit window deliberately: one below it, one on it, one
    /// just past it, and the same again around twice the window, so a charge that is admitted in a
    /// block can never be admitted one block too far. Witness W7 - a fast path that refuses instead
    /// of falling back to the locked one - fails here, because the run would stop at nothing like
    /// the ceiling.
    /// </remarks>
    [Theory]
    [InlineData(1UL)]
    [InlineData(2UL)]
    [InlineData(63UL)]
    [InlineData(64UL)]
    [InlineData(65UL)]
    [InlineData(127UL)]
    [InlineData(128UL)]
    [InlineData(129UL)]
    [InlineData(1000UL)]
    [InlineData(4096UL)]
    [InlineData(50000UL)]
    public void A_Runtime_Fuel_Ceiling_Is_Spent_To_The_Unit_Under_Windowed_Polling(ulong ceiling)
    {
        using var runtime = FixtureComposition.Runtime(
            Windowed(),
            FixtureComposition.Options(FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, ceiling)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Nops(60000));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ResourceExhaustion, result.Outcome);
        Assert.Equal(VmBudgetDimension.Fuel, result.Diagnostics.ExhaustedDimension);
        Assert.Equal(VmBudgetScope.Runtime, result.Diagnostics.ExhaustedScope);
        Assert.Equal(ceiling, Fuel(runtime));
    }

    /// <summary>
    /// The last instruction an allowance can pay for is the last one that runs.
    /// </summary>
    /// <remarks>
    /// Sixty thousand nops, a push and a return are 60,002 charges. One unit short of that must
    /// stop on the final instruction rather than a block early or a block late.
    /// </remarks>
    [Theory]
    [InlineData(60002UL, VmOutcome.Normal, 60002UL)]
    [InlineData(60001UL, VmOutcome.ResourceExhaustion, 60001UL)]
    public void A_Runtime_Fuel_Ceiling_At_The_Last_Instruction_Is_Exact(
        ulong ceiling, VmOutcome expected, ulong expectedFuel)
    {
        using var runtime = FixtureComposition.Runtime(
            Windowed(),
            FixtureComposition.Options(FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, ceiling)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Nops(60000));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(expected, result.Outcome);

        if (expected is VmOutcome.ResourceExhaustion)
        {
            Assert.Equal(VmBudgetDimension.Fuel, result.Diagnostics.ExhaustedDimension);
            Assert.Equal(VmBudgetScope.Runtime, result.Diagnostics.ExhaustedScope);
        }

        Assert.Equal(expectedFuel, Fuel(runtime));
    }

    // ---- T2, T2b: which level refused, and an allowance that spans invocations --------------

    /// <summary>
    /// When several levels would refuse the same charge, the outermost one names the scope.
    /// </summary>
    /// <remarks>
    /// A zero in a row means that level is left adopted rather than stated. The rows where two
    /// levels are tightened to the same figure are the load-bearing ones: both would refuse, and
    /// the answer must still be the outer of the two.
    /// </remarks>
    [Theory]
    [InlineData(5000UL, 0UL, 0UL, VmBudgetScope.Runtime)]
    [InlineData(0UL, 5000UL, 0UL, VmBudgetScope.Instance)]
    [InlineData(0UL, 0UL, 5000UL, VmBudgetScope.Invocation)]
    [InlineData(5000UL, 5000UL, 0UL, VmBudgetScope.Runtime)]
    [InlineData(0UL, 5000UL, 5000UL, VmBudgetScope.Instance)]
    [InlineData(5000UL, 0UL, 5000UL, VmBudgetScope.Runtime)]
    public void The_Outermost_Tightest_Level_Names_The_Exhausted_Scope(
        ulong runtimeFuel, ulong instanceFuel, ulong invocationFuel, VmBudgetScope expected)
    {
        var ceilings = runtimeFuel == 0
            ? FixtureComposition.AdoptedCeilings()
            : FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, runtimeFuel);

        using var runtime = FixtureComposition.Runtime(Windowed(), FixtureComposition.Options(ceilings));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Nops(60000));
        using var instance = Instantiate(runtime, artifact, instanceFuel);

        var result = Invoke(instance, invocationFuel);

        Assert.Equal(VmOutcome.ResourceExhaustion, result.Outcome);
        Assert.Equal(VmBudgetDimension.Fuel, result.Diagnostics.ExhaustedDimension);
        Assert.Equal(expected, result.Diagnostics.ExhaustedScope);
        Assert.Equal(5000UL, Fuel(runtime));
    }

    /// <summary>
    /// An instance allowance is one allowance across every invocation on that instance.
    /// </summary>
    /// <remarks>
    /// The first invocation ends part-way through a window, holding charged work that has not been
    /// accounted at a poll. Witness W4b - the step-end settle and the crowded-level settle removed
    /// together - fails here, because the second invocation is then admitted against a level that
    /// never learned what the first one spent, and the total overshoots by what the first was
    /// holding. The budget is deliberately not read between the two invocations: reading it settles
    /// every holder, which would empty the very state the witness needs to find.
    /// </remarks>
    [Fact]
    public void An_Instance_Allowance_Spans_Invocations_To_The_Unit()
    {
        using var runtime = FixtureComposition.Runtime(Windowed());

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Nops(3000));
        using var instance = Instantiate(runtime, artifact, instanceFuel: 5000);

        var first = Invoke(instance, 0);
        var second = Invoke(instance, 0);

        Assert.Equal(VmOutcome.Normal, first.Outcome);
        Assert.Equal(VmOutcome.ResourceExhaustion, second.Outcome);
        Assert.Equal(VmBudgetDimension.Fuel, second.Diagnostics.ExhaustedDimension);
        Assert.Equal(VmBudgetScope.Instance, second.Diagnostics.ExhaustedScope);
        Assert.Equal(5000UL, Fuel(runtime));
    }

    // ---- T3: a charge larger than any block ------------------------------------------------

    /// <summary>
    /// One charge far larger than any block is decided against the exact remainder.
    /// </summary>
    /// <remarks>
    /// Two million units in a single charge cannot be covered by any pre-admitted block, so it is
    /// decided on what is really left. One unit short and it is refused with the hundred and one
    /// units before it still spent; exactly enough and it is admitted whole. The third row runs the
    /// artifact to completion and is answered as a profile fault, because this variant never polls
    /// at all - and exhaustion outranks that breach in the two rows where both are true.
    /// </remarks>
    [Theory]
    [InlineData(2000100UL, VmOutcome.ResourceExhaustion, 101UL)]
    [InlineData(2000101UL, VmOutcome.ResourceExhaustion, 2000101UL)]
    [InlineData(2000103UL, VmOutcome.ProfileFault, 2000103UL)]
    public void A_Charge_Larger_Than_Any_Block_Is_Decided_On_The_Exact_Remainder(
        ulong ceiling, VmOutcome expected, ulong expectedFuel)
    {
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(
                FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.PollBoundBreaker)),
            FixtureComposition.Options(FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, ceiling)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.NopsSpinNops(100, 2000000, 0));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(expected, result.Outcome);

        if (expected is VmOutcome.ResourceExhaustion)
        {
            Assert.Equal(VmBudgetDimension.Fuel, result.Diagnostics.ExhaustedDimension);
            Assert.Equal(VmBudgetScope.Runtime, result.Diagnostics.ExhaustedScope);
        }
        else
        {
            Assert.Equal(VmReason.CancellationPollBoundExceeded, result.Reason);
        }

        Assert.Equal(expectedFuel, Fuel(runtime));
    }

    // ---- T4, T5: what a reader sees --------------------------------------------------------

    /// <summary>
    /// A budget snapshot taken while a step is held counts every charge that step has made.
    /// </summary>
    /// <remarks>
    /// The step is stopped inside a host capability, at a point where exactly 1,002 units have been
    /// charged, and it stays there while five threads read the budget five thousand times. Witness
    /// W1 - the settle removed from the runtime's own budget reader - fails here: the reads would
    /// see only what had been committed at the last poll, which is 960, and never the charges made
    /// since.
    /// </remarks>
    [Fact]
    public void A_Budget_Snapshot_Of_A_Held_Step_Counts_Every_Admitted_Charge()
    {
        var gate = new FixtureExecutionGate { HoldAt = FixtureGatePoint.Capability };

        using var runtime = FixtureComposition.Runtime(
            Windowed(),
            FixtureComposition.Options(capabilities: FixtureComposition.GatedCapabilities(gate)));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.NopsAroundHostCall(1000, FixtureHostCapabilities.DoubleBinding, 500));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var invocation = Task.Run(() => FixtureComposition.Invoke(instance));

        Assert.True(gate.WaitForEntry(Patience), "the host call never reached the handler");

        for (var read = 0; read < 1000; read++)
        {
            Assert.Equal(1002UL, Fuel(runtime));
        }

        Assert.Equal(1UL, runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.HostCalls));

        var disagreements = new ConcurrentQueue<ulong>();

        Parallel.For(0, 4, _ =>
        {
            for (var read = 0; read < 1000; read++)
            {
                var seen = Fuel(runtime);

                if (seen != 1002UL)
                {
                    disagreements.Enqueue(seen);
                }
            }
        });

        Assert.Empty(disagreements);

        gate.Release();

        var result = invocation.GetAwaiter().GetResult();

        Assert.Equal(VmOutcome.Normal, result.Outcome);
        Assert.Equal(1503UL, Fuel(runtime));
    }

    /// <summary>
    /// A budget read while a runtime is running never goes backwards and ends on the exact total.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Consumption is a sum of allowances already spent, and an allowance never refunds, so a
    /// reader watching a live runtime must see a monotone sequence bounded by the ceiling. Forty
    /// invocations of 20,002 charges each is the figure at the end.
    /// </para>
    /// <para>
    /// The observer has to be shown to have observed. A loop conditioned on a flag the main thread
    /// sets can be scheduled after that flag is already set, and would then enqueue no complaint
    /// having read nothing at all - an empty queue that means "no defect" and an empty queue that
    /// means "no evidence" are the same assertion, which is the shape this file's header refuses.
    /// So the observer signals its first read, the invocations do not start until that signal
    /// arrives, and the count of reads is asserted at the end beside the complaints.
    /// </para>
    /// </remarks>
    [Fact]
    public void Budget_Snapshots_Of_A_Running_Runtime_Never_Decrease_And_End_Exact()
    {
        using var runtime = FixtureComposition.Runtime(Windowed());

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Nops(20000));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var ceiling = runtime.GetBudgetSnapshot().EffectiveCeiling(VmBudgetDimension.Fuel);
        var complaints = new ConcurrentQueue<string>();
        var reads = 0;

        using var finished = new ManualResetEventSlim(false);
        using var watching = new ManualResetEventSlim(false);

        var observer = Task.Run(() =>
        {
            ulong previous = 0;

            while (!finished.IsSet)
            {
                var seen = Fuel(runtime);

                if (seen < previous)
                {
                    complaints.Enqueue($"consumption fell from {previous} to {seen}");
                }

                if (seen > ceiling)
                {
                    complaints.Enqueue($"consumption {seen} exceeded the ceiling {ceiling}");
                }

                previous = seen;
                Interlocked.Increment(ref reads);
                watching.Set();
            }
        });

        Assert.True(watching.Wait(Patience), "the observer never read the budget");

        for (var invocation = 0; invocation < 40; invocation++)
        {
            Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(instance).Outcome);
        }

        finished.Set();
        observer.GetAwaiter().GetResult();

        Assert.Empty(complaints);
        Assert.True(Volatile.Read(ref reads) > 0, "the observer enqueued nothing because it read nothing");
        Assert.Equal(40UL * 20002UL, Fuel(runtime));
    }

    // ---- T6, T7: two threads on one runtime ------------------------------------------------

    /// <summary>
    /// Two instances running at once spend one shared runtime ceiling to the unit.
    /// </summary>
    /// <remarks>
    /// Both steps are held at the entry to their invocation and released together, so the two runs
    /// genuinely overlap rather than taking turns. Together they want 120,004 units against a
    /// ceiling of 70,000, so at least one must be refused - and the total spent must be the ceiling
    /// exactly, because every charge is one unit. Witness W4 - a room test that always answers yes
    /// - fails here by letting the pair spend past the ceiling, though not on every iteration,
    /// which is why the test runs a hundred of them.
    /// </remarks>
    [Fact]
    public void Two_Instances_On_Two_Threads_Spend_A_Shared_Runtime_Ceiling_To_The_Unit()
    {
        var payload = FixtureArtifactWriter.Nops(60000);

        for (var iteration = 0; iteration < 100; iteration++)
        {
            var gate = new FixtureExecutionGate { HoldAt = FixtureGatePoint.Invoke };

            using var runtime = FixtureComposition.Runtime(
                Windowed(gate),
                FixtureComposition.Options(FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, 70000)));

            var artifact = FixtureComposition.Verify(runtime, payload);

            using var first = FixtureComposition.Instantiate(runtime, artifact);
            using var second = FixtureComposition.Instantiate(runtime, artifact);

            var a = Task.Run(() => FixtureComposition.Invoke(first));
            var b = Task.Run(() => FixtureComposition.Invoke(second));

            Assert.True(WaitForEntries(gate, 2), $"iteration {iteration}: both steps never entered");

            gate.Release();

            var left = a.GetAwaiter().GetResult();
            var right = b.GetAwaiter().GetResult();

            Assert.Equal(70000UL, Fuel(runtime));

            Assert.True(
                left.Outcome is VmOutcome.ResourceExhaustion || right.Outcome is VmOutcome.ResourceExhaustion,
                $"iteration {iteration}: neither run was refused: {left.Outcome} and {right.Outcome}");

            foreach (var result in new[] { left, right })
            {
                Assert.True(
                    result.Outcome is VmOutcome.Normal or VmOutcome.ResourceExhaustion,
                    $"iteration {iteration}: unexpected outcome {result.Outcome}/{result.Reason}");

                if (result.Outcome is VmOutcome.ResourceExhaustion)
                {
                    Assert.Equal(VmBudgetDimension.Fuel, result.Diagnostics.ExhaustedDimension);
                    Assert.Equal(VmBudgetScope.Runtime, result.Diagnostics.ExhaustedScope);
                }
            }
        }
    }

    /// <summary>
    /// Two concurrent operations are each stopped by their own invocation allowance.
    /// </summary>
    /// <remarks>
    /// Neither run can reach the runtime ceiling, so each must be refused by the bound stated for
    /// it alone, and the pair must together have spent exactly the sum of the two. A charge
    /// admitted against the wrong operation's remainder shows up here as a total that is not
    /// 70,000. Before the resolved answer was held per thread, witness W8 - an ambient meter that
    /// answers from a held context without checking it - failed here, because one run's charges
    /// reached the other's levels. A second thread can no longer see the first thread's answer, so
    /// T19 carries W8 on one thread.
    /// </remarks>
    [Fact]
    public void Two_Instances_On_Two_Threads_Are_Each_Stopped_By_Their_Own_Invocation_Allowance()
    {
        var payload = FixtureArtifactWriter.Nops(60000);

        for (var iteration = 0; iteration < 100; iteration++)
        {
            var gate = new FixtureExecutionGate { HoldAt = FixtureGatePoint.Invoke };

            using var runtime = FixtureComposition.Runtime(Windowed(gate));

            var artifact = FixtureComposition.Verify(runtime, payload);

            using var first = FixtureComposition.Instantiate(runtime, artifact);
            using var second = FixtureComposition.Instantiate(runtime, artifact);

            var a = Task.Run(() => Invoke(first, 30000));
            var b = Task.Run(() => Invoke(second, 40000));

            Assert.True(WaitForEntries(gate, 2), $"iteration {iteration}: both steps never entered");

            gate.Release();

            var left = a.GetAwaiter().GetResult();
            var right = b.GetAwaiter().GetResult();

            foreach (var result in new[] { left, right })
            {
                Assert.Equal(VmOutcome.ResourceExhaustion, result.Outcome);
                Assert.Equal(VmBudgetDimension.Fuel, result.Diagnostics.ExhaustedDimension);
                Assert.Equal(VmBudgetScope.Invocation, result.Diagnostics.ExhaustedScope);
            }

            Assert.Equal(70000UL, Fuel(runtime));
        }
    }

    // ---- T8: the poll bound counts every unit ----------------------------------------------

    /// <summary>
    /// The uncharged-work bound counts every fuel unit charged between two polls.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The artifact is <c>Nops(nopCount)</c> when that count is positive, and otherwise nops
    /// around one bulk spin. The spin's units never reach the executor's own poll window, so the
    /// bound is broken by work the profile did not count - which is the case the rule exists for.
    /// </para>
    /// <para>
    /// The sixty-one-unit row sits on the bound and completes; sixty-two is one unit past it and is
    /// found at completion, where the step-end settle makes the counter exact. Witness W2 - the
    /// step-end settle removed - turns that row into a normal completion. The last row is found
    /// earlier, at the poll after the sixty-fourth instruction charge, and witness W3 - the settle
    /// removed from the poll itself - lets that poll pass and finds the breach only at completion,
    /// by which time 133 units have been spent rather than 124.
    /// </para>
    /// </remarks>
    [Theory]
    [InlineData(10000, 0, 0L, 0, VmOutcome.Normal, 10002UL)]
    [InlineData(0, 0, 61L, 0, VmOutcome.Normal, 64UL)]
    [InlineData(0, 0, 62L, 0, VmOutcome.ProfileFault, 65UL)]
    [InlineData(0, 10, 60L, 60, VmOutcome.ProfileFault, 124UL)]
    public void Poll_Bound_Detection_Counts_Fuel_Charged_Between_Polls(
        int nopCount, int before, long units, int after, VmOutcome expected, ulong expectedFuel)
    {
        using var runtime = FixtureComposition.Runtime(Windowed());

        var payload = nopCount > 0
            ? FixtureArtifactWriter.Nops(nopCount)
            : FixtureArtifactWriter.NopsSpinNops(before, units, after);

        var artifact = FixtureComposition.Verify(runtime, payload);
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(expected, result.Outcome);

        if (expected is VmOutcome.ProfileFault)
        {
            Assert.Equal(VmReason.CancellationPollBoundExceeded, result.Reason);
        }

        Assert.Equal(expectedFuel, Fuel(runtime));
    }

    // ---- T9: what a nested verification is handed ------------------------------------------

    /// <summary>
    /// A guest-initiated load is verified under the exact fuel the operation has left.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Five thousand nops and the load instruction itself are 5,001 units, so the nested handle
    /// must carry the outer handle's instantiation ceiling less exactly that. Witness W5 - the
    /// settle removed from the remaining-allowance reader - fails here, because the remainder
    /// handed to the nested verification would be computed from a level that had not yet been told
    /// about the charges made since the last poll.
    /// </para>
    /// <para>
    /// The requesting profile polls on a window for exactly that reason. A profile that polls after
    /// every charge has committed everything it charged by the time it asks for a load, so the
    /// remainder is the same whether or not the reader settles first and the read guards nothing.
    /// With a window of sixty-four the last poll falls at 4,992 units, which leaves the nine
    /// charges before the request uncommitted at the moment the remainder is taken.
    /// </para>
    /// </remarks>
    [Fact]
    public void A_Guest_Load_Is_Verified_Under_The_Exact_Remaining_Invocation_Fuel()
    {
        var provider = new FixtureArtifactProvider(FixtureVmProfile.Id, FixtureArtifactWriter.Constant(99));

        FixtureVmExecutor? executor = null;

        using var runtime = FixtureComposition.Runtime(
            Observing(
                FixtureVmProfileVariant.WindowedGuestLoads,
                executorObserver: created => executor = created),
            FixtureComposition.Options(capabilities: FixtureComposition.WithProvider(provider)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.NopsThenLoad(5000, 1, 7));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var inherited = artifact.Identity.EffectiveCeilings.InstantiationCeilings[VmBudgetDimension.Fuel];

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, result.Outcome);
        Assert.Equal(1, provider.RequestCount);
        Assert.NotNull(executor);
        Assert.Equal(inherited - 5001UL, executor!.LastGuestLoadFuelCeiling);
    }

    // ---- T10: a park commits ----------------------------------------------------------------

    /// <summary>
    /// A parked operation has committed its fuel, and resumes against the same allowance.
    /// </summary>
    /// <remarks>
    /// <para>
    /// What the run spent before it parked is spent: an allowance does not refund at a suspension,
    /// and the resumed run draws on the same ceiling until it reaches it. Both polling shapes are
    /// exercised, because a park in the middle of a charge window is the case a profile that polls
    /// after every charge can never produce.
    /// </para>
    /// <para>
    /// The resume crosses a thread boundary deliberately. The profile is agile, so the core is free
    /// to resume a parked operation anywhere, and a resume that only ever happens on the thread
    /// that invoked would leave the poll phase a continuation carries, and the context comparison
    /// the meter makes when it resolves an operation, exercised on one thread only.
    /// </para>
    /// </remarks>
    [Theory]
    [InlineData(FixtureVmProfileVariant.Conforming)]
    [InlineData(FixtureVmProfileVariant.WindowedPolling)]
    public void A_Parked_Operation_Has_Committed_Its_Fuel_And_Resumes_Against_The_Same_Allowance(
        FixtureVmProfileVariant variant)
    {
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(variant)),
            FixtureComposition.Options(FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, 1500)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.NopsAroundYield(1000, 1000));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var parked = FixtureComposition.Invoke(instance);

        Assert.True(parked.IsSuspended, $"expected a suspension, got {parked.Outcome}/{parked.Reason}");
        Assert.Equal(1001UL, Fuel(runtime));
        Assert.True(parked.TryGetSuspension(out var suspension));

        var resumed = Task.Run(() => runtime.Resume(suspension)).GetAwaiter().GetResult();

        Assert.Equal(VmOutcome.ResourceExhaustion, resumed.Outcome);
        Assert.Equal(VmBudgetDimension.Fuel, resumed.Diagnostics.ExhaustedDimension);
        Assert.Equal(VmBudgetScope.Runtime, resumed.Diagnostics.ExhaustedScope);
        Assert.Equal(1500UL, Fuel(runtime));
    }

    // ---- T11: under an aggregate parent ------------------------------------------------------

    /// <summary>
    /// An aggregate parent sees every charge a held step has made, as it makes them.
    /// </summary>
    /// <remarks>
    /// A runtime under a parent shares that parent's allowance with its siblings, so nothing may be
    /// held back from it even briefly. Witness W6 - a block admitted where a parent is present -
    /// fails here, because the parent would then be told about a run of charges only when the block
    /// was settled rather than as each charge was admitted.
    /// </remarks>
    [Fact]
    public void An_Aggregate_Parent_Sees_Every_Charge_Of_A_Held_Step()
    {
        var gate = new FixtureExecutionGate { HoldAt = FixtureGatePoint.Capability };

        using var parent = AggregateBudget(liveRuntimes: 4, fuel: 10_000_000);

        using var runtime = FixtureComposition.Runtime(
            Windowed(),
            FixtureComposition.Options(
                parent: parent, capabilities: FixtureComposition.GatedCapabilities(gate)));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.NopsAroundHostCall(1000, FixtureHostCapabilities.DoubleBinding, 10));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var invocation = Task.Run(() => FixtureComposition.Invoke(instance));

        Assert.True(gate.WaitForEntry(Patience), "the host call never reached the handler");

        Assert.Equal(1002UL, parent.GetSnapshot().Consumed(VmBudgetDimension.Fuel));
        Assert.Equal(1002UL, Fuel(runtime));

        gate.Release();

        var result = invocation.GetAwaiter().GetResult();

        Assert.Equal(VmOutcome.Normal, result.Outcome);
        Assert.Equal(1013UL, parent.GetSnapshot().Consumed(VmBudgetDimension.Fuel));
    }

    // ---- T12, T13: which operation a charge reaches -------------------------------------------

    /// <summary>
    /// A charge through the environment's meter follows the execution context, and nothing else.
    /// </summary>
    /// <remarks>
    /// The executor holds one environment for the life of the runtime, and the meter behind it
    /// resolves to whichever operation is running. So a charge made on the step thread lands, a
    /// charge from a thread the step started lands too because the context flowed to it, and a
    /// charge from a thread started with the flow suppressed is refused - there is no operation
    /// there to bill. Outside a step the same meter refuses everything. Witness W8 - a held
    /// resolution that does not compare contexts - fails here: the test thread's refused lookup
    /// before the invocation leaves an answer for no meter, the defect hands that answer to the
    /// step's own first charge, and the run is refused.
    /// </remarks>
    [Fact]
    public void Fuel_Charged_Through_The_Environment_Meter_Follows_The_Execution_Context()
    {
        IVmExecutionEnvironment? captured = null;
        var answers = new List<bool>();

        VmHostCallOutcome Handler(ReadOnlySpan<long> arguments, out long result)
        {
            var environment = captured!;

            answers.Add(environment.Meter.TryCharge(VmBudgetDimension.Fuel, 5));

            var flowed = new Thread(() =>
                answers.Add(environment.Meter.TryCharge(VmBudgetDimension.Fuel, 7)));

            flowed.Start();
            flowed.Join();

            using (ExecutionContext.SuppressFlow())
            {
                var detached = new Thread(() =>
                    answers.Add(environment.Meter.TryCharge(VmBudgetDimension.Fuel, 11)));

                detached.Start();
                detached.Join();
            }

            result = 0;
            return VmHostCallOutcome.Completed;
        }

        using var runtime = FixtureComposition.Runtime(
            Observing(FixtureVmProfileVariant.Conforming, environment => captured = environment),
            FixtureComposition.Options(capabilities: FixtureComposition.CapabilitiesWithDouble(Handler)));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.NopsAroundHostCall(100, FixtureHostCapabilities.DoubleBinding, 100));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        Assert.NotNull(captured);
        Assert.False(captured!.Meter.TryCharge(VmBudgetDimension.Fuel, 1));

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, result.Outcome);
        Assert.Equal(new[] { true, true, false }, answers);
        Assert.False(captured.Meter.TryCharge(VmBudgetDimension.Fuel, 1));
        Assert.Equal(203UL + 5UL + 7UL, Fuel(runtime));
    }

    /// <summary>
    /// A thread a step started still charges that operation after the step has ended.
    /// </summary>
    /// <remarks>
    /// This pins the behaviour of commit <c>5130be9</c> - <c>VmExecutionScope.Leave</c>, which
    /// clears the step thread's own context and no other - and endorses nothing. A thread the
    /// profile started during the step therefore keeps the meter it captured, and its charges still
    /// land after the step has ended. A later change to how the meter is resolved, a cache over
    /// that resolution above all, must not alter that quietly, which is the whole reason the figure
    /// is written down here and named against that baseline.
    /// </remarks>
    [Fact]
    public void A_Thread_Started_Inside_A_Step_Still_Charges_Its_Operation_After_The_Step_Ends()
    {
        IVmExecutionEnvironment? captured = null;

        using var released = new ManualResetEventSlim(false);

        Thread? leaked = null;
        var charged = false;

        VmHostCallOutcome Handler(ReadOnlySpan<long> arguments, out long result)
        {
            var environment = captured!;

            leaked = new Thread(() =>
            {
                released.Wait(Patience);
                charged = environment.Meter.TryCharge(VmBudgetDimension.Fuel, 13);
            });

            leaked.Start();

            result = 0;
            return VmHostCallOutcome.Completed;
        }

        using var runtime = FixtureComposition.Runtime(
            Observing(FixtureVmProfileVariant.Conforming, environment => captured = environment),
            FixtureComposition.Options(capabilities: FixtureComposition.CapabilitiesWithDouble(Handler)));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.NopsAroundHostCall(100, FixtureHostCapabilities.DoubleBinding, 100));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, result.Outcome);

        var beforeTheLeakedCharge = Fuel(runtime);

        released.Set();
        Assert.NotNull(leaked);
        leaked!.Join();

        Assert.True(charged, "a thread the step started was refused after the step ended");
        Assert.Equal(beforeTheLeakedCharge + 13UL, Fuel(runtime));
    }

    // ---- T14, T15: retention and release ------------------------------------------------------

    /// <summary>
    /// A fuel retention is admitted against what every holder has charged, not against what has
    /// been committed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The retention report cannot refuse its caller, so its answer is latched and observed at the
    /// operation's end. With one holder the retention is offered 150 units against 102 already
    /// charged, and must be refused; the run then completes its remaining instructions and is
    /// reported as exhausted with 113 units spent. Witness W9 - the settle removed from the
    /// retention's admission check - admits it against the 64 units committed at the last poll and
    /// ends at 162.
    /// </para>
    /// <para>
    /// With two holders, a second operation retains while the first is held inside a capability
    /// with charges of its own outstanding. Witness W9b - a retention that settles only the meter
    /// doing the retaining - admits it against 166 committed units and ends with the first
    /// operation refused and 10,004 units spent.
    /// </para>
    /// </remarks>
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void A_Fuel_Retention_Is_Admitted_Against_Every_Holders_Charges(int holders)
    {
        if (holders == 1)
        {
            OneHolderRetention();
            return;
        }

        TwoHolderRetention();
    }

    private static void OneHolderRetention()
    {
        IVmExecutionEnvironment? captured = null;

        VmHostCallOutcome Handler(ReadOnlySpan<long> arguments, out long result)
        {
            captured!.Meter.ReportRetained(VmBudgetDimension.Fuel, 60);
            result = 0;
            return VmHostCallOutcome.Completed;
        }

        using var runtime = FixtureComposition.Runtime(
            Observing(FixtureVmProfileVariant.WindowedPolling, environment => captured = environment),
            FixtureComposition.Options(
                FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, 150),
                capabilities: FixtureComposition.CapabilitiesWithDouble(Handler)));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.NopsAroundHostCall(100, FixtureHostCapabilities.DoubleBinding, 10));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ResourceExhaustion, result.Outcome);
        Assert.Equal(VmBudgetDimension.Fuel, result.Diagnostics.ExhaustedDimension);
        Assert.Equal(VmBudgetScope.Runtime, result.Diagnostics.ExhaustedScope);
        Assert.Equal(113UL, Fuel(runtime));
    }

    private static void TwoHolderRetention()
    {
        IVmExecutionEnvironment? captured = null;
        var gate = new FixtureExecutionGate { HoldAt = FixtureGatePoint.Capability };
        var calls = 0;

        VmHostCallOutcome Handler(ReadOnlySpan<long> arguments, out long result)
        {
            result = 0;

            if (Interlocked.Increment(ref calls) == 1)
            {
                gate.Reached(FixtureGatePoint.Capability);
            }
            else
            {
                captured!.Meter.ReportRetained(VmBudgetDimension.Fuel, 9800);
            }

            return VmHostCallOutcome.Completed;
        }

        using var runtime = FixtureComposition.Runtime(
            Observing(FixtureVmProfileVariant.WindowedPolling, environment => captured = environment),
            FixtureComposition.Options(
                FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, 10000),
                capabilities: FixtureComposition.CapabilitiesWithDouble(Handler)));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.NopsAroundHostCall(100, FixtureHostCapabilities.DoubleBinding, 10));

        using var first = FixtureComposition.Instantiate(runtime, artifact);
        using var second = FixtureComposition.Instantiate(runtime, artifact);

        var held = Task.Run(() => FixtureComposition.Invoke(first));

        Assert.True(gate.WaitForEntry(Patience), "the first host call never reached the handler");

        var retaining = FixtureComposition.Invoke(second);

        gate.Release();

        var completed = held.GetAwaiter().GetResult();

        Assert.Equal(VmOutcome.ResourceExhaustion, retaining.Outcome);
        Assert.Equal(VmBudgetDimension.Fuel, retaining.Diagnostics.ExhaustedDimension);
        Assert.Equal(VmBudgetScope.Runtime, retaining.Diagnostics.ExhaustedScope);
        Assert.Equal(VmOutcome.Normal, completed.Outcome);
        Assert.Equal(226UL, Fuel(runtime));
    }

    /// <summary>
    /// Releasing fuel refunds nothing, at any level.
    /// </summary>
    /// <remarks>
    /// Fuel is an allowance, and an allowance never refunds - a released one would be a refund
    /// wearing another name. This is a pin rather than a witness: no injected defect is expected to
    /// fail it. It is here because it is the reason the release path is left alone entirely, and a
    /// change that made releasing fuel do something would otherwise be invisible.
    /// </remarks>
    [Fact]
    public void Releasing_Fuel_Refunds_Nothing_At_Any_Level()
    {
        IVmExecutionEnvironment? captured = null;

        VmHostCallOutcome Handler(ReadOnlySpan<long> arguments, out long result)
        {
            captured!.Meter.ReportReleased(VmBudgetDimension.Fuel, 50);
            result = 0;
            return VmHostCallOutcome.Completed;
        }

        using var runtime = FixtureComposition.Runtime(
            Observing(FixtureVmProfileVariant.WindowedPolling, environment => captured = environment),
            FixtureComposition.Options(capabilities: FixtureComposition.CapabilitiesWithDouble(Handler)));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.NopsAroundHostCall(100, FixtureHostCapabilities.DoubleBinding, 10));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, result.Outcome);
        Assert.Equal(113UL, Fuel(runtime));
    }

    // ---- T16: a charge on another dimension ---------------------------------------------------

    /// <summary>
    /// A charge on another dimension never spends fuel.
    /// </summary>
    /// <remarks>
    /// The host-call ceiling is one, so the second invocation must be refused at its host call with
    /// 102 units of fuel spent on the way there. Witness W10 - a fast path that does not check
    /// which dimension it was handed - spends a unit of the fuel block for each host call and lets
    /// the second invocation complete normally.
    /// </remarks>
    [Fact]
    public void A_Non_Fuel_Charge_Never_Spends_A_Fuel_Block()
    {
        using var runtime = FixtureComposition.Runtime(
            Windowed(),
            FixtureComposition.Options(FixtureComposition.CeilingsWith(VmBudgetDimension.HostCalls, 1)));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.NopsAroundHostCall(100, FixtureHostCapabilities.DoubleBinding, 10));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var first = FixtureComposition.Invoke(instance);
        var second = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, first.Outcome);
        Assert.Equal(VmOutcome.ResourceExhaustion, second.Outcome);
        Assert.Equal(VmBudgetDimension.HostCalls, second.Diagnostics.ExhaustedDimension);
        Assert.Equal(VmBudgetScope.Runtime, second.Diagnostics.ExhaustedScope);
        Assert.Equal(1UL, runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.HostCalls));
        Assert.Equal(215UL, Fuel(runtime));
    }

    // ---- T17: the block an eviction makes room for --------------------------------------------

    /// <summary>
    /// A pre-admission that has to evict a holder commits what that holder spent.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Four meters of one runtime may hold a block at once; a fifth is made room for by settling
    /// the oldest. Five steps of one runtime held together inside a capability is the shape that
    /// reaches that, and the only one this suite otherwise never builds: each of the five charges a
    /// thousand units before its host call, so each wants a block, and none gives one up while it
    /// is held. No budget is read while they are held, deliberately - a runtime snapshot settles
    /// every holder, and would empty the table the fifth meter has to find full.
    /// </para>
    /// <para>
    /// The defect is an eviction that drops its victim's block instead of committing it. The victim
    /// then holds a block the table no longer knows of: it spends the rest of it without the gate
    /// and nothing ever commits that, and because the block's size is still recorded it never takes
    /// another one either. The total comes out short by a whole block for every eviction, which is
    /// 128 units here - twice the sixty-four-unit window this profile declares. The same figure
    /// fails an eviction that removes its victim before committing it, which commits whichever
    /// holder moved into the hole and orphans the one it meant to settle.
    /// </para>
    /// </remarks>
    [Fact]
    public void A_Full_Table_Commits_The_Block_It_Evicts()
    {
        const int Holders = 5;

        var gate = new FixtureExecutionGate { HoldAt = FixtureGatePoint.Capability };

        using var runtime = FixtureComposition.Runtime(
            Windowed(),
            FixtureComposition.Options(capabilities: FixtureComposition.GatedCapabilities(gate)));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.NopsAroundHostCall(1000, FixtureHostCapabilities.DoubleBinding, 500));

        var instances = new VmInstance[Holders];
        var outcomes = new VmInvocationResult[Holders];
        var steps = new Thread[Holders];

        for (var index = 0; index < Holders; index++)
        {
            instances[index] = FixtureComposition.Instantiate(runtime, artifact);
        }

        try
        {
            // Threads of its own rather than the pool. All five block inside the gate at once, and
            // a pool that injects its last worker on a timer would turn the rendezvous into a race
            // against this test's own patience rather than a property of the runtime.
            for (var index = 0; index < Holders; index++)
            {
                var slot = index;

                steps[slot] = new Thread(() => outcomes[slot] = FixtureComposition.Invoke(instances[slot]));
                steps[slot].Start();
            }

            Assert.True(WaitForEntries(gate, Holders), "five steps were never held at once");

            gate.Release();

            foreach (var step in steps)
            {
                Assert.True(step.Join(Patience), "a held step never finished");
            }

            foreach (var outcome in outcomes)
            {
                Assert.Equal(VmOutcome.Normal, outcome.Outcome);
            }

            Assert.Equal((ulong)Holders, runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.HostCalls));
            Assert.Equal(Holders * 1503UL, Fuel(runtime));
        }
        finally
        {
            gate.Release();

            foreach (var instance in instances)
            {
                instance.Dispose();
            }
        }
    }

    // ---- T18: what a suppressed flow may be answered from --------------------------------------

    /// <summary>
    /// A lookup made with the execution flow suppressed is answered, and never cached.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A suppressed flow captures no context at all, so a resolution has nothing to be keyed on.
    /// Such a lookup is answered from the AsyncLocal and publishes nothing, and that is
    /// load-bearing rather than tidy: an absent context is a key every suppressed flow in the
    /// process shares, so a pair published under it would be handed to the next thread that looks
    /// up with its own flow suppressed - whichever operation that thread is in, and whether it is
    /// in one at all.
    /// </para>
    /// <para>
    /// The step thread charges five units with its own flow suppressed, and they land: suppressing
    /// the flow stops a context from reaching a new thread and does not take this thread out of its
    /// step, so the AsyncLocal still holds this operation's meter. A thread with no operation of
    /// its own then charges eleven with its flow suppressed too, and must be refused.
    /// </para>
    /// <para>
    /// The test pins the cross-thread shape the scope-wide cache got wrong, where a pair published
    /// under the absent context was handed to the second thread. Since the answer moved to the
    /// thread, a defect of the resolution fails it only if it both shares the held answer across
    /// threads and holds it under the absent context. T20 carries the suppressed-flow witness on one
    /// thread.
    /// </para>
    /// </remarks>
    [Fact]
    public void A_Suppressed_Flow_Is_Answered_But_Never_Cached()
    {
        IVmExecutionEnvironment? captured = null;
        var answers = new List<bool>();

        VmHostCallOutcome Handler(ReadOnlySpan<long> arguments, out long result)
        {
            var environment = captured!;

            using (ExecutionContext.SuppressFlow())
            {
                answers.Add(environment.Meter.TryCharge(VmBudgetDimension.Fuel, 5));

                var detached = new Thread(() =>
                {
                    using (ExecutionContext.SuppressFlow())
                    {
                        answers.Add(environment.Meter.TryCharge(VmBudgetDimension.Fuel, 11));
                    }
                });

                detached.Start();
                detached.Join();
            }

            result = 0;
            return VmHostCallOutcome.Completed;
        }

        using var runtime = FixtureComposition.Runtime(
            Observing(FixtureVmProfileVariant.Conforming, environment => captured = environment),
            FixtureComposition.Options(capabilities: FixtureComposition.CapabilitiesWithDouble(Handler)));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.NopsAroundHostCall(100, FixtureHostCapabilities.DoubleBinding, 100));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, result.Outcome);
        Assert.Equal(new[] { true, false }, answers);
        Assert.Equal(203UL + 5UL, Fuel(runtime));
    }

    // ---- T19: a context change on one thread ----------------------------------------------------

    /// <summary>
    /// A charge made on one thread under another operation's execution context bills that operation,
    /// and each operation is still stopped by its own invocation allowance to the unit.
    /// </summary>
    /// <remarks>
    /// <para>
    /// B is held inside its capability call, having captured the context it runs under there. A runs
    /// on the test thread with an invocation allowance of exactly what it charges. Inside A's
    /// capability call the thread charges five units under its own context, then seven under B's
    /// context, and then goes back to its own. The seven land on B, so A completes on 208 units and B
    /// on 210.
    /// </para>
    /// <para>
    /// Witness W8 - a lookup that returns the thread's held answer without comparing contexts -
    /// answers the seven from A's meter, which it holds from the five-unit charge. A is then refused
    /// before its last instructions, and the runtime ends seven units short of 418. This is the claim
    /// T7 was written to carry, made on one thread, which is the only place a held answer can now be
    /// reused.
    /// </para>
    /// </remarks>
    [Fact]
    public void A_Charge_Made_Under_Another_Operations_Context_Bills_That_Operation()
    {
        IVmExecutionEnvironment? captured = null;
        ExecutionContext? held = null;
        var gate = new FixtureExecutionGate { HoldAt = FixtureGatePoint.Capability };
        var calls = 0;
        var answers = new List<bool>();

        VmHostCallOutcome Handler(ReadOnlySpan<long> arguments, out long result)
        {
            result = 0;

            if (Interlocked.Increment(ref calls) == 1)
            {
                held = ExecutionContext.Capture();
                gate.Reached(FixtureGatePoint.Capability);
                return VmHostCallOutcome.Completed;
            }

            var environment = captured!;
            var inside = ExecutionContext.Capture()!;

            answers.Add(environment.Meter.TryCharge(VmBudgetDimension.Fuel, 5));

            ExecutionContext.Restore(held!);

            try
            {
                answers.Add(environment.Meter.TryCharge(VmBudgetDimension.Fuel, 7));
            }
            finally
            {
                ExecutionContext.Restore(inside);
            }

            return VmHostCallOutcome.Completed;
        }

        using var runtime = FixtureComposition.Runtime(
            Observing(FixtureVmProfileVariant.Conforming, environment => captured = environment),
            FixtureComposition.Options(capabilities: FixtureComposition.CapabilitiesWithDouble(Handler)));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.NopsAroundHostCall(100, FixtureHostCapabilities.DoubleBinding, 100));

        using var a = Instantiate(runtime, artifact, instanceFuel: 0);
        using var b = Instantiate(runtime, artifact, instanceFuel: 0);

        VmInvocationResult? bResult = null;
        var bStep = new Thread(() => bResult = Invoke(b, 0));

        bStep.Start();

        try
        {
            Assert.True(gate.WaitForEntry(Patience), "B's host call never reached the handler");
            Assert.NotNull(held);

            var aResult = Invoke(a, 203UL + 5UL);

            gate.Release();
            Assert.True(bStep.Join(Patience), "B never finished");

            Assert.Equal(VmOutcome.Normal, aResult.Outcome);
            Assert.True(bResult.HasValue, "B finished without a result");
            Assert.Equal(VmOutcome.Normal, bResult!.Value.Outcome);
            Assert.Equal(new[] { true, true }, answers);
            Assert.Equal((203UL + 5UL) + (203UL + 7UL), Fuel(runtime));
        }
        finally
        {
            gate.Release();
        }
    }

    // ---- T20: two suppressed lookups on one thread ----------------------------------------------

    /// <summary>
    /// A lookup made with the flow suppressed is answered from the context it was made in, even when
    /// an earlier suppressed lookup on the same thread was answered with a meter.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Inside the step's capability call the thread charges five units with its flow suppressed, and
    /// they land. It then puts itself under a context captured outside any step, suppresses the flow
    /// again, and charges eleven, which must be refused. The thread-held answer is what this guards:
    /// a suppressed flow captures no context, so the absent context would be the same key for both
    /// lookups.
    /// </para>
    /// <para>
    /// Witness W12 - a suppressed lookup held under the absent context - answers the second charge
    /// from the first one's meter, and the run ends eleven units over.
    /// </para>
    /// </remarks>
    [Fact]
    public void A_Suppressed_Lookup_Is_Not_Answered_From_An_Earlier_One_On_The_Same_Thread()
    {
        IVmExecutionEnvironment? captured = null;
        var answers = new List<bool>();
        var outside = ExecutionContext.Capture();

        Assert.NotNull(outside);

        VmHostCallOutcome Handler(ReadOnlySpan<long> arguments, out long result)
        {
            var environment = captured!;
            var inside = ExecutionContext.Capture()!;

            using (ExecutionContext.SuppressFlow())
            {
                answers.Add(environment.Meter.TryCharge(VmBudgetDimension.Fuel, 5));
            }

            ExecutionContext.Restore(outside!);

            try
            {
                using (ExecutionContext.SuppressFlow())
                {
                    answers.Add(environment.Meter.TryCharge(VmBudgetDimension.Fuel, 11));
                }
            }
            finally
            {
                ExecutionContext.Restore(inside);
            }

            result = 0;
            return VmHostCallOutcome.Completed;
        }

        using var runtime = FixtureComposition.Runtime(
            Observing(FixtureVmProfileVariant.Conforming, environment => captured = environment),
            FixtureComposition.Options(capabilities: FixtureComposition.CapabilitiesWithDouble(Handler)));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.NopsAroundHostCall(100, FixtureHostCapabilities.DoubleBinding, 100));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, result.Outcome);
        Assert.Equal(new[] { true, false }, answers);
        Assert.Equal(203UL + 5UL, Fuel(runtime));
    }

    // ---- T21: what a capability's return puts back ---------------------------------------------

    /// <summary>
    /// A value a capability sets in its execution context is still set when the step's next
    /// capability call runs, and the capability's depth is released when it returns.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This pins the behaviour of commit <c>f127d92</c>: a synchronous callee's AsyncLocal write is
    /// seen by its caller afterwards. The first capability call of each invocation sets a value, and
    /// the second reads it. A second invocation of the same instance must then be admitted, which it
    /// is only if neither call left its non-reentrancy depth on the thread. Four instructions per
    /// invocation, and two host calls each.
    /// </para>
    /// <para>
    /// It is written down because putting back the context a call was entered from is a way of
    /// leaving a capability that is cheaper than writing the depth back, and it must never discard a
    /// change the capability made. Witness W16 - putting that context back without checking what the
    /// capability left installed - loses the value, and the second call reads nothing. Witnesses W17
    /// and W18 - dropping the write-back when the capability changed its context, and recording the
    /// entry context after the depth was raised - leave the raised depth on the thread, and the
    /// second invocation is refused as a call from inside a capability.
    /// </para>
    /// </remarks>
    [Fact]
    public void A_Capability_That_Changes_Its_Context_Keeps_The_Change_And_Releases_Its_Depth()
    {
        const string Marker = "set by the first capability call";
        var marker = new AsyncLocal<string?>();
        var seen = new List<string?>();
        var calls = 0;

        VmHostCallOutcome Handler(ReadOnlySpan<long> arguments, out long result)
        {
            if (++calls % 2 == 1)
            {
                marker.Value = Marker;
            }
            else
            {
                seen.Add(marker.Value);
            }

            result = 0;
            return VmHostCallOutcome.Completed;
        }

        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.Conforming)),
            FixtureComposition.Options(capabilities: FixtureComposition.CapabilitiesWithDouble(Handler)));

        var artifact = FixtureComposition.Verify(
            runtime,
            FixtureArtifactWriter.Write(
                [21],
                [
                    FixtureFormat.OpPushConst, 0,
                    FixtureFormat.OpHostCall, FixtureHostCapabilities.DoubleBinding,
                    FixtureFormat.OpHostCall, FixtureHostCapabilities.DoubleBinding,
                    FixtureFormat.OpReturn,
                ]));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        try
        {
            var first = FixtureComposition.Invoke(instance);

            Assert.Equal(VmOutcome.Normal, first.Outcome);
            Assert.Equal(new string?[] { Marker }, seen);

            var second = FixtureComposition.Invoke(instance);

            Assert.Equal(VmOutcome.Normal, second.Outcome);
            Assert.Equal(new string?[] { Marker, Marker }, seen);
            Assert.Equal(8UL, Fuel(runtime));
            Assert.Equal(4UL, runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.HostCalls));
        }
        finally
        {
            // The value is this test's own, and it would otherwise stay on the thread that ran it.
            marker.Value = null;
        }
    }

    // ---- T22: a capability called from inside another ------------------------------------------

    /// <summary>
    /// A non-reentrant capability that calls another capability through the step's table is still
    /// inside its own boundary when the inner call returns.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The handler's first call invokes the same capability again through the environment's capability
    /// table, which enters the boundary a second time on the same thread, and then asks the runtime
    /// to verify an artifact. That request is a runtime call from inside a non-reentrant capability
    /// and must be refused as one, because the outer call has not returned. Once it has, the depth is
    /// released and a second invocation is admitted. Two host calls and 203 units.
    /// </para>
    /// <para>
    /// Nesting is the only shape in which leaving the inner call can release the outer call's depth.
    /// Witness W19 - a return that puts back the context the inner call was entered from and then
    /// writes the depth back as well - lowers the depth a second time, and the runtime call is
    /// admitted.
    /// </para>
    /// </remarks>
    [Fact]
    public void A_Capability_Called_From_Inside_Another_Leaves_The_Outer_Call_Inside_Its_Boundary()
    {
        IVmExecutionEnvironment? captured = null;
        VmRuntime? current = null;
        var calls = 0;
        var nested = VmHostCallOutcome.Unavailable;
        var refusal = VmReason.None;
        var observed = false;

        VmHostCallOutcome Handler(ReadOnlySpan<long> arguments, out long result)
        {
            result = 0;

            if (++calls != 1)
            {
                return VmHostCallOutcome.Completed;
            }

            Span<long> inner = stackalloc long[1];
            nested = captured!.Capabilities.Invoke(FixtureHostCapabilities.DoubleBinding, inner, out _);

            var descriptor = FixtureComposition.Descriptor();
            var attempt = current!.Verify(in descriptor, FixtureArtifactWriter.Constant(1), CancellationToken.None);

            refusal = attempt.Reason;
            observed = true;
            attempt.TryGetArtifact(out var verified);
            verified?.Dispose();

            return VmHostCallOutcome.Completed;
        }

        using var runtime = FixtureComposition.Runtime(
            Observing(FixtureVmProfileVariant.Conforming, environment => captured = environment),
            FixtureComposition.Options(capabilities: FixtureComposition.CapabilitiesWithDouble(Handler)));

        current = runtime;

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.NopsAroundHostCall(100, FixtureHostCapabilities.DoubleBinding, 100));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, result.Outcome);
        Assert.True(observed, "the outer capability call never reached its runtime call");
        Assert.Equal(VmHostCallOutcome.Completed, nested);
        Assert.Equal(VmReason.ReentrantRuntimeCallFromCapability, refusal);
        Assert.Equal(2UL, runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.HostCalls));
        Assert.Equal(203UL, Fuel(runtime));

        Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(instance).Outcome);
    }

    // ---- T23: a poll after a refused poll ------------------------------------------------------

    /// <summary>
    /// A poll refused on the uncharged-work bound is refused again while nothing has reset the count.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A refusal resets nothing, so a profile that ignores one and polls again must not be told the
    /// breach went away. The fixture executor stops at its first refused poll, which is why the second
    /// poll here is the handler's own.
    /// </para>
    /// <para>
    /// The executor polled after its sixty-fourth instruction charge, so 38 units are unpolled when
    /// the host call runs. The handler charges 27 more in one charge, one past the bound of 64, and
    /// then polls twice: both are refused. The executor's own poll after its 128th instruction charge
    /// is refused as well, and the run is a profile fault at 155 units. Witness W21 - a refused poll
    /// that records what it counted as counted - answers the second poll true, lets the executor's
    /// polls pass, and ends at 230. Witness W21b - the reset moved before the bound test - answers
    /// every poll true.
    /// </para>
    /// </remarks>
    [Fact]
    public void A_Poll_After_A_Refused_Poll_Is_Refused_Again()
    {
        IVmExecutionEnvironment? captured = null;
        var answers = new List<bool>();

        VmHostCallOutcome Handler(ReadOnlySpan<long> arguments, out long result)
        {
            var meter = captured!.Meter;

            answers.Add(meter.TryCharge(VmBudgetDimension.Fuel, 27));
            answers.Add(meter.Poll());
            answers.Add(meter.Poll());

            result = 0;
            return VmHostCallOutcome.Completed;
        }

        using var runtime = FixtureComposition.Runtime(
            Observing(FixtureVmProfileVariant.WindowedPolling, environment => captured = environment),
            FixtureComposition.Options(capabilities: FixtureComposition.CapabilitiesWithDouble(Handler)));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.NopsAroundHostCall(100, FixtureHostCapabilities.DoubleBinding, 100));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(new[] { true, false, false }, answers);
        Assert.Equal(VmOutcome.ProfileFault, result.Outcome);
        Assert.Equal(VmReason.CancellationPollBoundExceeded, result.Reason);
        Assert.Equal(155UL, Fuel(runtime));
    }

    private static VmAggregateBudget AggregateBudget(ulong liveRuntimes, ulong fuel)
    {
        var builder = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            if (!VmBudgetDimensions.CarriesAggregateScope(dimension))
            {
                continue;
            }

            builder.Add(dimension switch
            {
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.Value(dimension, liveRuntimes),
                VmBudgetDimension.Fuel => VmCeilingSpec.Value(dimension, fuel),
                _ => VmCeilingSpec.Value(dimension, 1_000_000_000),
            });
        }

        return VmAggregateBudget.Create(builder.ToImmutable());
    }
}
