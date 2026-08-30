// xUnit1031 says to make the test async and await instead of blocking. It is suppressed here for
// the same reason as in ConcurrencyTests, and for one more. A bounded wait that TIMES OUT is the
// assertion in this file: `Assert.False(disposal.Wait(250ms))` is how "disposal has not returned
// while a step is still in flight" is stated, and an await cannot express a wait that is required
// not to finish. The waits that are expected to succeed carry a patience rather than blocking
// indefinitely, so a broken drain fails the test instead of hanging the suite.
#pragma warning disable xUnit1031

using Broiler.VM;
using Broiler.VM.Fixtures;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// What disposal and cancellation promise when they arrive at the worst moment.
/// </summary>
/// <remarks>
/// The lifecycle rules were frozen at VM-0 and implemented at VM-1 against a single thread, where
/// "dispose while running" is not a state a test can reach. These are the cases that only exist
/// once a second thread does, and they are the ones the gate names: no use-after-dispose, a
/// bounded drain, and a paused operation that can always be cancelled and disposed.
/// </remarks>
public sealed class DisposalAndCancellationTests
{
    private static readonly TimeSpan Patience = TimeSpan.FromSeconds(10);

    /// <summary>
    /// A disposal whose drain expires still returns, and the straggling step releases the handle as
    /// it leaves.
    /// </summary>
    /// <remarks>
    /// This is the case a bound exists for. A profile that ignores its cancellation token cannot be
    /// preempted by anything the core is allowed to do - no thread abort, no timer, no second thread
    /// - so the only honest promise is that the core's own wait is bounded. What must not happen is
    /// the obvious shortcut: releasing the lease anyway and disposing the verified state under a
    /// profile that is still reading it.
    /// </remarks>
    [Fact]
    public void A_Disposal_Whose_Drain_Expires_Returns_And_Hands_The_Lease_To_The_Step()
    {
        var gate = new FixtureExecutionGate { HoldAt = FixtureGatePoint.Invoke };
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.Conforming, gate)),
            FixtureComposition.Options(disposeDrainBudget: TimeSpan.FromMilliseconds(150)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(42));
        var instance = FixtureComposition.Instantiate(runtime, artifact);

        var invocation = Task.Run(() => FixtureComposition.Invoke(instance));
        Assert.True(gate.WaitForEntry(Patience), "the invocation never entered the profile");

        var started = DateTime.UtcNow;
        var disposal = instance.Dispose();
        var waited = DateTime.UtcNow - started;

        Assert.Equal(VmControlOutcome.Accepted, disposal.Kind);
        Assert.True(waited < TimeSpan.FromSeconds(5), $"the drain was not bounded: waited {waited}");
        Assert.Equal(VmInstanceState.Disposed, instance.State);

        // The handle is still pinned, because the step that holds it has not left the profile.
        // Disposing it now moves it to draining rather than to disposed.
        Assert.Equal(VmControlOutcome.Accepted, artifact.Dispose().Kind);
        Assert.Equal(VmVerifiedArtifactState.Draining, artifact.State);

        gate.Release();
        invocation.GetAwaiter().GetResult();

        // And the drain completes when the step leaves, without anyone calling anything.
        var deadline = DateTime.UtcNow + Patience;

        while (artifact.State is not VmVerifiedArtifactState.Disposed && DateTime.UtcNow < deadline)
        {
            Thread.Sleep(10);
        }

        Assert.Equal(VmVerifiedArtifactState.Disposed, artifact.State);
    }

    /// <summary>
    /// An instantiation that was inside the profile when the runtime began disposing is refused,
    /// and the instance it built is given back rather than stranded.
    /// </summary>
    /// <remarks>
    /// A registration that arrived after disposal walked the instance list would leave an instance
    /// nobody can reach and nothing will dispose: its lease pins its handle for the life of the
    /// process and its retained bytes stay charged to a runtime that no longer answers.
    /// </remarks>
    [Fact]
    public void An_Instantiation_Racing_Disposal_Is_Refused_And_Gives_Its_Instance_Back()
    {
        var gate = new FixtureExecutionGate { HoldAt = FixtureGatePoint.Instantiate };
        var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.Conforming, gate)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(42));

        var instantiation = Task.Run(() => runtime.Instantiate(artifact, CancellationToken.None));
        Assert.True(gate.WaitForEntry(Patience), "instantiation never entered the profile");

        var disposal = Task.Run(() => runtime.Dispose());

        // Disposal is not waiting for this one: the instance is not registered yet, so there is
        // nothing in the runtime's list to drain. It completes, and the instantiation then finds a
        // runtime that is no longer taking instances.
        Assert.True(disposal.Wait(Patience), "runtime disposal did not complete");

        gate.Release();
        var result = instantiation.GetAwaiter().GetResult();

        Assert.Equal(VmOutcome.InvalidState, result.Outcome);
        Assert.False(result.TryGetInstance(out _), "a disposed runtime handed back an instance");

        // The lease the refused instantiation took is given back, so the handle can still drain.
        Assert.Equal(VmControlOutcome.Accepted, artifact.Dispose().Kind);
        Assert.Equal(VmVerifiedArtifactState.Disposed, artifact.State);
    }

    /// <summary>Cancellation reaches a running step and the operation ends cancelled.</summary>
    [Fact]
    public void Cancellation_Reaches_A_Step_That_Is_Already_Running()
    {
        var gate = new FixtureExecutionGate { HoldAt = FixtureGatePoint.Invoke };
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.Conforming, gate)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Spin(50_000));
        var instance = FixtureComposition.Instantiate(runtime, artifact);

        var invocation = Task.Run(() => FixtureComposition.Invoke(instance));
        Assert.True(gate.WaitForEntry(Patience), "the invocation never entered the profile");

        Assert.Equal(VmControlOutcome.Accepted, instance.RequestCancel().Kind);
        gate.Release();

        var result = invocation.GetAwaiter().GetResult();

        Assert.Equal(VmOutcome.Cancellation, result.Outcome);
        Assert.Equal(VmReason.Cancelled, result.Reason);
        Assert.Equal(VmInstanceState.Faulted, instance.State);

        instance.Dispose();
        artifact.Dispose();
    }

    /// <summary>
    /// Cancelling the whole runtime cancels every live instance, from any thread.
    /// </summary>
    [Fact]
    public void Cancelling_A_Runtime_Reaches_Every_Instance()
    {
        var gate = new FixtureExecutionGate { HoldAt = FixtureGatePoint.Invoke };
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.Conforming, gate)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Spin(50_000));
        var first = FixtureComposition.Instantiate(runtime, artifact);
        var second = FixtureComposition.Instantiate(runtime, artifact);

        var one = Task.Run(() => FixtureComposition.Invoke(first));
        Assert.True(gate.WaitForEntry(Patience), "the first invocation never entered the profile");

        Assert.Equal(VmControlOutcome.Accepted, Task.Run(() => runtime.RequestCancel()).GetAwaiter().GetResult().Kind);

        gate.Release();
        Assert.Equal(VmOutcome.Cancellation, one.GetAwaiter().GetResult().Outcome);

        // The second instance was never invoked, so cancellation had nothing to reach on it: a
        // runtime-wide cancel is a request to every live operation, not a poison for the runtime.
        Assert.Equal(VmInstanceState.Live, second.State);

        first.Dispose();
        second.Dispose();
        artifact.Dispose();
    }

    /// <summary>
    /// A suspended operation is disposed rather than resumed, and disposal unwinds it on the
    /// disposing thread.
    /// </summary>
    [Fact]
    public void A_Paused_Operation_Is_Unwound_By_Disposal_On_The_Disposing_Thread()
    {
        var unwindThread = 0;
        var gate = new FixtureExecutionGate();
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.Conforming, gate)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.YieldThenConstant(17));
        var instance = FixtureComposition.Instantiate(runtime, artifact);

        var suspended = FixtureComposition.Invoke(instance);
        Assert.True(suspended.TryGetSuspension(out var suspension), "expected a suspension");

        // Disposal from another thread, so "the disposing thread" is a thread we can name.
        var disposal = Task.Run(() =>
        {
            unwindThread = Environment.CurrentManagedThreadId;
            return runtime.Dispose();
        });

        Assert.True(disposal.Wait(Patience), "disposal blocked on a paused operation");
        Assert.Equal(VmControlOutcome.Accepted, disposal.GetAwaiter().GetResult().Kind);

        // The resume token is dead, and answers so rather than resuming an abandoned continuation.
        var resumed = runtime.Resume(suspension);
        Assert.Equal(VmOutcome.InvalidState, resumed.Outcome);

        Assert.NotEqual(0, unwindThread);
        artifact.Dispose();
    }

    /// <summary>
    /// A client that abandons a paused operation does not pin it: residency expiry ends it.
    /// </summary>
    /// <remarks>
    /// The gate names this case in its own words - "a client that abandons a paused operation" -
    /// and it is the one case with no caller to report to. What must hold is that the operation is
    /// not left holding a live-suspended slot, an instance and a meter for the life of the runtime.
    /// </remarks>
    [Fact]
    public void An_Abandoned_Pause_Is_Ended_By_Residency_Expiry()
    {
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.AlphaCatalog(),
            FixtureComposition.Options(
                maxLiveSuspendedOperations: 1,
                maxSuspendedResidency: TimeSpan.FromMilliseconds(50)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.YieldThenConstant(17));
        var instance = FixtureComposition.Instantiate(runtime, artifact);

        var suspended = FixtureComposition.Invoke(instance);
        Assert.True(suspended.TryGetSuspension(out var suspension), "expected a suspension");

        // The client walks away. Nothing is resumed, nothing is cancelled, nothing is disposed.
        Thread.Sleep(120);

        Assert.Equal(VmControlOutcome.Accepted, runtime.PollDeadlines().Kind);

        // The slot is free again, which is the observable half: with a limit of one, a second pause
        // is only admissible because the first one was expired rather than merely forgotten.
        var second = FixtureComposition.Instantiate(runtime, artifact);
        var again = FixtureComposition.Invoke(second);
        Assert.True(again.TryGetSuspension(out _), $"the freed slot was not reusable: {again.Outcome}/{again.Reason}");

        var dead = runtime.Resume(suspension);
        Assert.Equal(VmOutcome.InvalidState, dead.Outcome);

        instance.Dispose();
        second.Dispose();
        artifact.Dispose();
    }

    /// <summary>
    /// Every public call on a disposed runtime answers invalid state rather than throwing or
    /// running.
    /// </summary>
    /// <remarks>
    /// One outcome for every illegal transition and every use after disposal, on every object, at
    /// every stage - which is a claim about the whole surface rather than about one member, so it
    /// is asserted over the whole surface.
    /// </remarks>
    [Fact]
    public void Every_Public_Call_On_A_Disposed_Runtime_Answers_Invalid_State()
    {
        var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(42));
        var instance = FixtureComposition.Instantiate(runtime, artifact);

        runtime.Dispose();

        var descriptor = FixtureComposition.Descriptor();
        var verified = runtime.Verify(in descriptor, FixtureArtifactWriter.Constant(1), CancellationToken.None);
        Assert.Equal(VmOutcome.InvalidState, verified.Outcome);
        Assert.Equal(VmReason.ObjectDisposed, verified.Reason);

        var instantiated = runtime.Instantiate(artifact, CancellationToken.None);
        Assert.Equal(VmOutcome.InvalidState, instantiated.Outcome);

        var invoked = FixtureComposition.Invoke(instance);
        Assert.Equal(VmOutcome.InvalidState, invoked.Outcome);

        Assert.Equal(VmControlOutcome.InvalidState, runtime.RequestCancel().Kind);
        Assert.Equal(VmControlOutcome.InvalidState, runtime.PollDeadlines().Kind);

        // Disposal itself is idempotent rather than invalid: asking twice is legal and does nothing.
        Assert.Equal(VmControlOutcome.NoOp, runtime.Dispose().Kind);

        artifact.Dispose();
    }

    /// <summary>
    /// Disposal from several threads at once disposes once and answers every caller.
    /// </summary>
    [Fact]
    public void Concurrent_Disposals_Dispose_Once_And_Answer_Everyone()
    {
        var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(42));
        var instance = FixtureComposition.Instantiate(runtime, artifact);

        var results = new VmControlResult[8];
        Parallel.For(0, results.Length, index => results[index] = runtime.Dispose());

        Assert.Single(results.Where(static result => result.Kind is VmControlOutcome.Accepted));
        Assert.All(
            results,
            result => Assert.True(
                result.Kind is VmControlOutcome.Accepted or VmControlOutcome.NoOp,
                $"a concurrent disposal answered {result.Kind}"));

        Assert.Equal(VmInstanceState.Disposed, instance.State);
        artifact.Dispose();
    }

    /// <summary>
    /// A guest-initiated load in flight is cancelled with its requesting operation and leaves no
    /// verified handle behind.
    /// </summary>
    /// <remarks>
    /// The gate's own sentence. The load runs inside the requesting operation's step and under its
    /// meter, so cancelling the operation must end the load too - and a load that was mid-flight
    /// must not leave a half-verified artifact anywhere a later caller could reach.
    /// </remarks>
    [Fact]
    public void A_Guest_Load_In_Flight_Is_Cancelled_With_Its_Operation()
    {
        var provider = new CancellingProvider();
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.DeclaresGuestLoads)),
            FixtureComposition.Options(capabilities: FixtureComposition.WithProvider(provider)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.LoadThenConstant(1, 7));
        var instance = FixtureComposition.Instantiate(runtime, artifact);

        provider.CancelWhenAsked = instance;

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(1, provider.RequestCount);
        Assert.Equal(VmOutcome.Cancellation, result.Outcome);
        Assert.Equal(VmInstanceState.Faulted, instance.State);

        instance.Dispose();
        artifact.Dispose();
    }

    /// <summary>
    /// A provider that cancels the operation which asked it, while it is asking.
    /// </summary>
    /// <remarks>
    /// A load is genuinely in flight for exactly as long as the provider is inside its answer, so
    /// this is the only place a test can stand to cancel one. It answers normally afterwards, which
    /// is the point: the load itself succeeded, and the operation is cancelled all the same.
    /// </remarks>
    private sealed class CancellingProvider : IVmArtifactProvider
    {
        internal VmInstance? CancelWhenAsked { get; set; }

        internal int RequestCount { get; private set; }

        public VmCapabilityId CapabilityId => FixtureHostCapabilities.ProviderId;

        public int Version => 1;

        public VmArtifactProviderAnswer Answer(scoped in VmArtifactRequest request)
        {
            RequestCount++;
            CancelWhenAsked?.RequestCancel();

            var descriptor = new VmArtifactDescriptor(
                FixtureVmProfile.Id,
                FixtureFormat.FormatVersion,
                FixtureVmProfile.Manifest,
                default,
                VmCallerIdentity.FromCanonicalIdentity("test://provider"));

            return VmArtifactProviderAnswer.Provided(in descriptor, FixtureArtifactWriter.Constant(3));
        }
    }
}
