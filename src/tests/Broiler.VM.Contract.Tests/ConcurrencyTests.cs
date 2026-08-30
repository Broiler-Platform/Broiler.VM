// xUnit1031 says to make the test async and await instead of blocking. That advice is wrong for
// this file, and narrowly so: the subject here is THREAD IDENTITY. An operation pinned to the
// thread that started it must be refused on every other thread and still resume on its own, so the
// assertion after the refusal has to run on the thread that opened the operation. An await hands
// the continuation to the pool - there is no synchronization context under xUnit to hand it back -
// which would run that assertion somewhere else and invert the result. Blocking is what keeps the
// test on one thread, and it is bounded rather than indefinite: every wait carries a patience.
#pragma warning disable xUnit1031

using Broiler.VM;
using Broiler.VM.Fixtures;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// What the core promises while more than one thread is using it.
/// </summary>
/// <remarks>
/// <para>
/// Every claim here is arranged rather than raced. A test that started a thread and slept would be
/// asserting that this machine is slow enough, and would pass or fail for reasons that have nothing
/// to do with the code; each test below holds the profile inside a step with an execution gate, does
/// the thing under test while it is held, and releases it.
/// </para>
/// <para>
/// VM-1 carried the declared thread affinity and never exercised it across threads, which is what
/// Exclusion EX-44 recorded. These are the tests that close it.
/// </para>
/// </remarks>
public sealed class ConcurrencyTests
{
    private static readonly TimeSpan Patience = TimeSpan.FromSeconds(10);

    /// <summary>
    /// A capability running on one thread does not refuse an unrelated call on another.
    /// </summary>
    /// <remarks>
    /// Non-reentrancy is a statement about a CALL STACK: the capability must not call back into the
    /// runtime that invoked it. A second thread's verification is not that capability re-entering,
    /// and refusing it would make any capability that blocks - a lookup, a read, a lock - stop the
    /// whole runtime for every other caller.
    /// </remarks>
    [Fact]
    public void A_Running_Capability_Does_Not_Refuse_Another_Threads_Call()
    {
        var gate = new FixtureExecutionGate { HoldAt = FixtureGatePoint.Capability };
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.Descriptor),
            FixtureComposition.Options(capabilities: FixtureComposition.GatedCapabilities(gate)));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.HostCall(21, FixtureHostCapabilities.DoubleBinding));

        var instance = FixtureComposition.Instantiate(runtime, artifact);
        var invocation = Task.Run(() => FixtureComposition.Invoke(instance));

        Assert.True(gate.WaitForEntry(Patience), "the capability was never entered");

        // The capability is held inside the host call, on the other thread, right now.
        var descriptor = FixtureComposition.Descriptor();
        var concurrent = runtime.Verify(in descriptor, FixtureArtifactWriter.Constant(7), CancellationToken.None);

        gate.Release();
        var result = invocation.GetAwaiter().GetResult();

        Assert.True(
            concurrent.IsSuccess,
            $"a concurrent verification was refused while a capability ran: {concurrent.Outcome}/{concurrent.Reason}");

        Assert.Equal(VmOutcome.Normal, result.Outcome);

        concurrent.TryGetArtifact(out var verified);
        verified?.Dispose();
        instance.Dispose();
        artifact.Dispose();
    }

    /// <summary>
    /// A capability that calls back into its own invoking runtime is still refused.
    /// </summary>
    /// <remarks>
    /// The other half of the rule, and the half that must not be lost when the first is fixed. This
    /// is the same thread and the same call stack, which is what re-entrancy means.
    /// </remarks>
    [Fact]
    public void A_Capability_That_Re_Enters_Its_Own_Runtime_Is_Refused()
    {
        VmRuntime? current = null;
        var refusal = VmReason.None;
        var observed = false;

        VmHostCallOutcome Reentrant(ReadOnlySpan<long> arguments, out long result)
        {
            result = 0;
            var descriptor = FixtureComposition.Descriptor();
            var attempt = current!.Verify(in descriptor, FixtureArtifactWriter.Constant(1), CancellationToken.None);
            refusal = attempt.Reason;
            observed = true;
            attempt.TryGetArtifact(out var artifact);
            artifact?.Dispose();
            return VmHostCallOutcome.Completed;
        }

        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.Descriptor),
            FixtureComposition.Options(capabilities: FixtureComposition.CapabilitiesWithDouble(Reentrant)));

        current = runtime;

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.HostCall(21, FixtureHostCapabilities.DoubleBinding));

        var instance = FixtureComposition.Instantiate(runtime, artifact);
        FixtureComposition.Invoke(instance);

        Assert.True(observed, "the re-entrant capability never ran");
        Assert.Equal(VmReason.ReentrantRuntimeCallFromCapability, refusal);

        instance.Dispose();
        artifact.Dispose();
    }

    /// <summary>
    /// Disposing an instance while a step is executing does not return until the step has left the
    /// profile.
    /// </summary>
    /// <remarks>
    /// This is the use-after-dispose clause in its sharpest form. Disposal releases the artifact
    /// lease and gives back the instance's retained bytes; doing either while the executor is still
    /// reading the verified state would be the core pulling the ground out from under a profile it
    /// is still running.
    /// </remarks>
    [Fact]
    public void Disposing_An_Instance_Waits_For_The_Step_To_Leave_The_Profile()
    {
        var gate = new FixtureExecutionGate { HoldAt = FixtureGatePoint.Invoke };
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.Conforming, gate)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(42));
        var instance = FixtureComposition.Instantiate(runtime, artifact);

        var invocation = Task.Run(() => FixtureComposition.Invoke(instance));
        Assert.True(gate.WaitForEntry(Patience), "the invocation never entered the profile");

        var disposal = Task.Run(() => instance.Dispose());

        // Disposal must still be waiting: the profile is inside the step and has not returned.
        Assert.False(
            disposal.Wait(TimeSpan.FromMilliseconds(250)),
            "disposal completed while the profile was still executing a step");

        gate.Release();

        Assert.True(disposal.Wait(Patience), "disposal did not complete after the step returned");
        invocation.GetAwaiter().GetResult();

        Assert.Equal(VmInstanceState.Disposed, instance.State);
        artifact.Dispose();
    }

    /// <summary>
    /// Disposing the runtime while a step is executing does the same, and its drain is bounded.
    /// </summary>
    [Fact]
    public void Disposing_A_Runtime_Waits_For_Its_Instances_Steps()
    {
        var gate = new FixtureExecutionGate { HoldAt = FixtureGatePoint.Invoke };
        var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.Conforming, gate)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(42));
        var instance = FixtureComposition.Instantiate(runtime, artifact);

        var invocation = Task.Run(() => FixtureComposition.Invoke(instance));
        Assert.True(gate.WaitForEntry(Patience), "the invocation never entered the profile");

        var disposal = Task.Run(() => runtime.Dispose());

        Assert.False(
            disposal.Wait(TimeSpan.FromMilliseconds(250)),
            "runtime disposal completed while a profile step was still executing");

        gate.Release();

        Assert.True(disposal.Wait(Patience), "runtime disposal did not complete after the step returned");
        invocation.GetAwaiter().GetResult();
        artifact.Dispose();
    }

    /// <summary>
    /// A profile that declares an operation pinned to its starting thread is not resumed on another.
    /// </summary>
    /// <remarks>
    /// <c>VmThreadAffinity.OperationThreadPinned</c> and <c>VmReason.ThreadAffinityViolation</c> were
    /// both declared at VM-1 and neither was reachable: the affinity was carried in the descriptor
    /// and read by nothing. A declaration a host can make and the core ignores is worse than no
    /// declaration, because a profile author will believe it.
    /// </remarks>
    [Fact]
    public void A_Pinned_Operation_Is_Not_Resumed_On_Another_Thread()
    {
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(
                FixtureVmProfileVariant.Conforming, VmThreadAffinity.OperationThreadPinned)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.YieldThenConstant(17));
        var instance = FixtureComposition.Instantiate(runtime, artifact);

        var suspended = FixtureComposition.Invoke(instance);
        Assert.True(suspended.TryGetSuspension(out var suspension), $"expected a suspension, got {suspended.Outcome}");

        var elsewhere = Task.Run(() => runtime.Resume(suspension)).GetAwaiter().GetResult();

        Assert.Equal(VmOutcome.InvalidState, elsewhere.Outcome);
        Assert.Equal(VmReason.ThreadAffinityViolation, elsewhere.Reason);

        // And the operation is still resumable on the thread that started it, so the refusal is a
        // guard rather than a way to lose an operation.
        var here = runtime.Resume(suspension);
        Assert.Equal(VmOutcome.Normal, here.Outcome);

        instance.Dispose();
        artifact.Dispose();
    }

    /// <summary>An agile profile is resumed on any thread, which is what agile means.</summary>
    [Fact]
    public void An_Agile_Operation_Is_Resumed_On_Any_Thread()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.YieldThenConstant(17));
        var instance = FixtureComposition.Instantiate(runtime, artifact);

        var suspended = FixtureComposition.Invoke(instance);
        Assert.True(suspended.TryGetSuspension(out var suspension));

        var resumed = Task.Run(() => runtime.Resume(suspension)).GetAwaiter().GetResult();

        Assert.Equal(VmOutcome.Normal, resumed.Outcome);

        instance.Dispose();
        artifact.Dispose();
    }

    /// <summary>
    /// Two runtimes over one catalog do not share instance state, budgets, or diagnostics.
    /// </summary>
    [Fact]
    public void Two_Runtimes_Over_One_Catalog_Share_Nothing()
    {
        var catalog = FixtureComposition.AlphaCatalog();
        using var first = FixtureComposition.Runtime(catalog);
        using var second = FixtureComposition.Runtime(catalog);

        Assert.NotEqual(first.ObjectId, second.ObjectId);

        var artifact = FixtureComposition.Verify(first, FixtureArtifactWriter.Spin(500));
        var instance = FixtureComposition.Instantiate(first, artifact);
        Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(instance).Outcome);

        var spentHere = first.GetBudgetSnapshot().Consumed(VmBudgetDimension.Fuel);
        var spentThere = second.GetBudgetSnapshot().Consumed(VmBudgetDimension.Fuel);

        Assert.True(spentHere >= 500, $"expected the spin to be charged here, saw {spentHere}");
        Assert.Equal(0UL, spentThere);

        // A shareable handle IS usable by the other runtime - that is what shareable means, and
        // what makes one verification serve several runtimes without carrying either one's state
        // into the other. What must not cross is the consumption, and it does not: the second
        // runtime pays for its own instantiation out of its own allowance.
        var elsewhere = FixtureComposition.Instantiate(second, artifact);
        Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(elsewhere).Outcome);

        Assert.True(
            second.GetBudgetSnapshot().Consumed(VmBudgetDimension.Fuel) > 0,
            "the second runtime ran an instance and was charged nothing");

        Assert.Equal(spentHere, first.GetBudgetSnapshot().Consumed(VmBudgetDimension.Fuel));

        elsewhere.Dispose();
        instance.Dispose();
        artifact.Dispose();
    }

    /// <summary>
    /// Many threads driving many runtimes complete every operation and leak nothing between them.
    /// </summary>
    /// <remarks>
    /// The soak host measures a plateau over a long run; this asserts the property a plateau depends
    /// on, which is that a runtime's consumption is a function of what it was asked to do and of
    /// nothing another runtime did.
    /// </remarks>
    [Fact]
    public void Independent_Runtimes_Under_Load_Charge_Only_Their_Own_Work()
    {
        var catalog = FixtureComposition.AlphaCatalog();
        const int Runtimes = 8;
        const int Cycles = 25;

        var consumed = new ulong[Runtimes];

        Parallel.For(0, Runtimes, index =>
        {
            using var runtime = FixtureComposition.Runtime(catalog);
            var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Spin(100));

            for (var cycle = 0; cycle < Cycles; cycle++)
            {
                var instance = FixtureComposition.Instantiate(runtime, artifact);
                Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(instance).Outcome);
                instance.Dispose();
            }

            consumed[index] = runtime.GetBudgetSnapshot().Consumed(VmBudgetDimension.Fuel);
            artifact.Dispose();
        });

        // Every runtime did identical work, so every total is identical. A shared counter, a static
        // meter or a leaked scope would show up here as a spread.
        Assert.All(consumed, total => Assert.Equal(consumed[0], total));
        Assert.True(consumed[0] > 0, "no fuel was charged at all");
    }

    /// <summary>
    /// One instance admits one step at a time, even when many threads ask at once.
    /// </summary>
    [Fact]
    public void One_Instance_Admits_One_Step_At_A_Time()
    {
        var gate = new FixtureExecutionGate { HoldAt = FixtureGatePoint.Invoke };
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.Conforming, gate)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(42));
        var instance = FixtureComposition.Instantiate(runtime, artifact);

        var held = Task.Run(() => FixtureComposition.Invoke(instance));
        Assert.True(gate.WaitForEntry(Patience), "the first invocation never entered the profile");

        var refused = FixtureComposition.Invoke(instance);

        gate.Release();
        var admitted = held.GetAwaiter().GetResult();

        Assert.Equal(VmOutcome.InvalidState, refused.Outcome);
        Assert.Equal(VmReason.ReentrancyRefused, refused.Reason);
        Assert.Equal(VmOutcome.Normal, admitted.Outcome);
        Assert.Equal(1, gate.Entries);

        instance.Dispose();
        artifact.Dispose();
    }
}
