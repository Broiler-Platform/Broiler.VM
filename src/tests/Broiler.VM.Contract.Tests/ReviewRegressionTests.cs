using Broiler.VM;
using Broiler.VM.Fixtures;
using System.Collections.Immutable;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// One test per defect the adversarial review found, so none of them can come back quietly.
/// </summary>
/// <remarks>
/// Every case here failed against the first implementation. They are grouped by what they protect
/// rather than by which file they touch, because the point is the behaviour, not the code that
/// happens to produce it today.
/// </remarks>
public sealed class ReviewRegressionTests
{
    // ---- the frozen outcome-to-instance-state mapping -------------------------------------

    [Fact]
    public void Cancellation_Faults_The_Instance()
    {
        // Mandatory and admitting no implementation freedom: the profile stack was abandoned at an
        // arbitrary point, so it has no owner-visible state. Returning the instance to Live left it
        // re-invocable, which would make every later isolation claim meaningless.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Sum(1, 2));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();

        var request = new VmInvocationRequest(new VmUtf8Text("main"u8));
        var result = instance.Invoke(in request, cancelled.Token);

        Assert.Equal(VmOutcome.Cancellation, result.Outcome);
        Assert.Equal(VmInstanceState.Faulted, instance.State);

        var again = FixtureComposition.Invoke(instance);
        Assert.Equal(VmOutcome.InvalidState, again.Outcome);
        Assert.Equal(VmReason.TerminalFault, again.Reason);
    }

    [Fact]
    public void Resource_Exhaustion_Faults_The_Instance()
    {
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.AlphaCatalog(),
            FixtureComposition.Options(FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, 50)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Spin(500));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ResourceExhaustion, result.Outcome);
        Assert.Equal(VmInstanceState.Faulted, instance.State);
    }

    [Fact]
    public void An_Invalid_State_Leaves_The_Instance_Unchanged()
    {
        // The call never entered the profile, so nothing about the instance has become untrue.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.YieldThenConstant(1));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        Assert.True(FixtureComposition.Invoke(instance).IsSuspended);
        Assert.Equal(VmInstanceState.Suspended, instance.State);

        var refused = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.InvalidState, refused.Outcome);
        Assert.Equal(VmInstanceState.Suspended, instance.State);
    }

    // ---- the frozen precedence order ------------------------------------------------------

    [Fact]
    public void Cancellation_Outranks_A_Poll_Bound_Breach()
    {
        // One precedence order for every stage: cancellation is second, a profile fault is
        // seventh. Reporting the breach first blamed the profile for a condition it did not cause
        // and dropped the cancellation from the result entirely.
        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.PollBoundBreaker));

        using var runtime = FixtureComposition.Runtime(catalog);
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Spin(4096));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();

        var request = new VmInvocationRequest(new VmUtf8Text("main"u8));
        var result = instance.Invoke(in request, cancelled.Token);

        Assert.Equal(VmOutcome.Cancellation, result.Outcome);
    }

    [Fact]
    public void Exhaustion_Outranks_A_Poll_Bound_Breach_And_Keeps_Its_Dimension()
    {
        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.PollBoundBreaker));

        using var runtime = FixtureComposition.Runtime(
            catalog, FixtureComposition.Options(FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, 40)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Spin(4096));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ResourceExhaustion, result.Outcome);

        // The dimension and scope survive, which they did not when the breach was reported first.
        Assert.Equal(VmBudgetDimension.Fuel, result.Diagnostics.ExhaustedDimension);
    }

    // ---- host failure actually terminates -------------------------------------------------

    [Fact]
    public void A_Terminate_Operation_Capability_Ends_The_Operation_As_A_Host_Failure()
    {
        // The declared translation mode has to mean something. Both modes previously reached the
        // profile identically and the operation reported whatever the profile said, so a host
        // defect was billed to the guest and the capability identity the result must carry was
        // computed and discarded.
        VmHostCallOutcome Throwing(ReadOnlySpan<long> arguments, out long result)
        {
            result = 0;
            throw new InvalidOperationException("host defect");
        }

        var capabilities = ImmutableArray.Create(
            VmCapabilityRegistration.Value(FixtureHostCapabilities.Double, Throwing),
            VmCapabilityRegistration.Value(FixtureHostCapabilities.Throwing, FixtureHostCapabilities.ThrowingHandler),
            VmCapabilityRegistration.Value(FixtureHostCapabilities.Refusing, FixtureHostCapabilities.RefusingHandler));

        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.AlphaCatalog(), FixtureComposition.Options(capabilities: capabilities));

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.HostCall(1, FixtureHostCapabilities.DoubleBinding));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);
        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.HostFailure, result.Outcome);
        Assert.Equal(VmReason.HostCapabilityFaulted, result.Reason);
        Assert.Equal(FixtureHostCapabilities.DoubleId, result.Diagnostics.CapabilityId);
        Assert.Equal(1, result.Diagnostics.CapabilityVersion);
        Assert.Equal(VmInstanceState.Faulted, instance.State);
        Assert.Equal(VmRuntimeState.Ready, runtime.State);
    }

    [Fact]
    public void An_Observable_Fault_Capability_Leaves_The_Answer_To_The_Profile()
    {
        // The other half of the same rule: where the capability declared the fault observable, the
        // profile converts it and the converted outcome is the profile's own. That is a
        // control-flow fact, not a precedence question.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var artifact = FixtureComposition.Verify(
            runtime, FixtureArtifactWriter.HostCall(1, FixtureHostCapabilities.ThrowingBinding));

        using var instance = FixtureComposition.Instantiate(runtime, artifact);
        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ProfileFault, result.Outcome);
        Assert.Equal(VmRuntimeState.Ready, runtime.State);
    }

    // ---- aggregate accounting -------------------------------------------------------------

    [Fact]
    public void A_Retention_The_Parent_Refuses_Is_Not_Released_From_The_Parent()
    {
        // The asymmetry let the parent's live sum be driven below the true sum across its children
        // and then to zero, at which point it admitted a retention it should have refused.
        using var parent = Parent(liveBytes: 1000);
        var catalog = FixtureComposition.AlphaCatalog();
        var ceilings = FixtureComposition.CeilingsWith(VmBudgetDimension.LiveBytes, 1000);

        using var first = FixtureComposition.Runtime(catalog, FixtureComposition.Options(ceilings, parent: parent));
        using var second = FixtureComposition.Runtime(catalog, FixtureComposition.Options(ceilings, parent: parent));

        var retain = FixtureArtifactWriter.Write(
            [900], [FixtureFormat.OpRetain, 0, FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn]);

        using var a = FixtureComposition.Instantiate(first, FixtureComposition.Verify(first, retain));
        Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(a).Outcome);

        var afterA = parent.GetSnapshot().Consumed(VmBudgetDimension.LiveBytes);
        Assert.Equal(900ul, afterA);

        // The second retention exceeds the parent's remaining 100 and must be refused rather than
        // recorded locally and forgotten.
        using var b = FixtureComposition.Instantiate(second, FixtureComposition.Verify(second, retain));
        FixtureComposition.Invoke(b);

        var release = FixtureArtifactWriter.Write(
            [900], [FixtureFormat.OpRelease, 0, FixtureFormat.OpPushConst, 0, FixtureFormat.OpReturn]);

        using var releaser = FixtureComposition.Instantiate(second, FixtureComposition.Verify(second, release));
        FixtureComposition.Invoke(releaser);

        // The parent must still hold what runtime A genuinely retains.
        var afterRelease = parent.GetSnapshot().Consumed(VmBudgetDimension.LiveBytes);
        Assert.True(
            afterRelease >= 900,
            $"the parent was credited a retention it never accepted: {afterRelease} after {afterA}");
    }

    [Fact]
    public void A_Spent_Parent_Admits_No_Further_Runtime()
    {
        // The only guard used to be the per-dimension ceiling comparison, which passes trivially
        // when the host asks for zero or adopts the remainder - so a spent parent kept handing out
        // runtimes that could do no work.
        using var parent = Parent(fuel: 0);

        var refused = VmRuntime.Create(
            FixtureComposition.AlphaCatalog(),
            FixtureComposition.Options(
                FixtureComposition.CeilingsWith(VmBudgetDimension.Fuel, 0), parent: parent));

        Assert.Equal(VmOutcome.ResourceExhaustion, refused.Outcome);
        Assert.Equal(VmReason.ParentExhausted, refused.Reason);
        Assert.False(refused.TryGetRuntime(out _));
    }

    [Fact]
    public void An_Operation_Under_A_Spent_Parent_Is_Not_Resumed()
    {
        // Once a shared parent has no remaining allowance no runtime may be created and no
        // operation may be resumed. There was no resume admission check at all: the runtime
        // validated its own state, the handle and the token, and dispatched.
        //
        // Host calls are the dimension used here because they can be spent exactly: one call
        // against a ceiling of one leaves the parent with nothing, deterministically.
        using var parent = Parent(hostCalls: 1);
        var catalog = FixtureComposition.AlphaCatalog();

        // The runtime's own ceiling has to fit inside the parent's remaining allowance, which is
        // one call - a per-runtime ceiling may never exceed it.
        using var runtime = FixtureComposition.Runtime(
            catalog,
            FixtureComposition.Options(
                FixtureComposition.CeilingsWith(VmBudgetDimension.HostCalls, 1), parent: parent));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.YieldThenConstant(7));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var suspended = FixtureComposition.Invoke(instance);
        Assert.True(suspended.TryGetSuspension(out var suspension));

        using var burner = FixtureComposition.Instantiate(
            runtime,
            FixtureComposition.Verify(
                runtime, FixtureArtifactWriter.HostCall(1, FixtureHostCapabilities.DoubleBinding)));

        FixtureComposition.Invoke(burner);

        Assert.Equal(0ul, parent.GetSnapshot().Remaining(VmBudgetDimension.HostCalls));

        var resumed = runtime.Resume(suspension);

        Assert.Equal(VmOutcome.ResourceExhaustion, resumed.Outcome);
        Assert.Equal(VmReason.ParentExhausted, resumed.Reason);
        Assert.Equal(VmBudgetScope.Aggregate, resumed.Diagnostics.ExhaustedScope);
    }

    // ---- lifecycle leaks -------------------------------------------------------------------

    [Fact]
    public void An_Abandoned_Operation_Gives_Back_Its_Suspended_Slot()
    {
        // Abandonment left the terminal operation in the runtime's suspended set for the life of
        // the runtime, consuming a live-suspended slot and pinning its meter and its instance.
        using var runtime = FixtureComposition.Runtime(
            FixtureComposition.AlphaCatalog(),
            FixtureComposition.Options(maxLiveSuspendedOperations: 1));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.YieldThenConstant(1));

        var first = FixtureComposition.Instantiate(runtime, artifact);
        Assert.True(FixtureComposition.Invoke(first).IsSuspended);

        first.Dispose();

        using var second = FixtureComposition.Instantiate(runtime, artifact);
        var result = FixtureComposition.Invoke(second);

        Assert.True(
            result.IsSuspended,
            $"the abandoned slot was never returned: {result.Outcome}/{result.Reason}");
    }

    [Fact]
    public void A_Cancelled_Suspended_Operation_Is_Not_Resumed()
    {
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.YieldThenConstant(1));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var suspended = FixtureComposition.Invoke(instance, out var handle);
        Assert.True(suspended.TryGetSuspension(out var suspension));

        Assert.Equal(VmControlOutcome.Accepted, handle.RequestCancel().Kind);

        var resumed = runtime.Resume(suspension);

        Assert.Equal(VmOutcome.Cancellation, resumed.Outcome);
        Assert.Equal(VmReason.Cancelled, resumed.Reason);
    }

    // ---- artifact leases and clause order ---------------------------------------------------

    [Fact]
    public void A_Handle_Backing_A_Live_Instance_Drains_Rather_Than_Disposing()
    {
        // Instantiation took no lease, so a handle with a live instance derived from it went
        // straight to Disposed while that instance was still invocable.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(5));

        var instance = FixtureComposition.Instantiate(runtime, artifact);
        Assert.Equal(1, artifact.LeaseCount);

        Assert.Equal(VmControlOutcome.Accepted, artifact.Dispose().Kind);
        Assert.Equal(VmVerifiedArtifactState.Draining, artifact.State);

        instance.Dispose();

        Assert.Equal(VmVerifiedArtifactState.Disposed, artifact.State);
        Assert.Equal(0, artifact.LeaseCount);
    }

    [Fact]
    public void A_Disposed_Handle_Reports_Its_Disposal_Even_Where_The_Profile_Is_Absent()
    {
        // The clause order is load-bearing: hoisting the catalog lookup ahead of the handle state
        // made the answer depend on which of two true things the code looked at first.
        using var owner = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(owner, FixtureArtifactWriter.Constant(1));
        artifact.Dispose();

        using var stranger = FixtureComposition.Runtime(
            FixtureComposition.Catalog(SecondFixtureVmProfile.Descriptor));

        var result = stranger.Instantiate(artifact, CancellationToken.None);

        Assert.Equal(VmOutcome.InvalidState, result.Outcome);
        Assert.Equal(VmReason.HandleDisposed, result.Reason);
    }

    // ---- guest loads ------------------------------------------------------------------------

    [Fact]
    public void A_Profile_Cannot_Swallow_A_Nested_Resource_Exhaustion()
    {
        // The mediator's own bound checks fire before any meter charge, so a refusal they produced
        // left no trace and a profile that ignored the result completed Normal.
        var provider = new FixtureArtifactProvider(FixtureVmProfile.Id, FixtureArtifactWriter.Constant(1));

        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.MisconvertingNestedOutcome));

        using var runtime = FixtureComposition.Runtime(
            catalog, FixtureComposition.Options(capabilities: FixtureComposition.WithProvider(provider)));

        var code = new List<byte>();

        for (var index = 0; index < 12; index++)
        {
            code.Add(FixtureFormat.OpLoad);
            code.Add(0);
        }

        code.Add(FixtureFormat.OpPushConst);
        code.Add(0);
        code.Add(FixtureFormat.OpReturn);

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Write([3], code.ToArray()));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.ResourceExhaustion, result.Outcome);
        Assert.Equal(VmInstanceState.Faulted, instance.State);
    }

    [Fact]
    public void Fan_Out_Is_Not_Refreshed_By_A_Suspension()
    {
        // A resumed operation is the same operation: it keeps its budget remainder and its
        // nested-load counters. Resetting them per step let a profile that yields between loads
        // have as many as it liked.
        var provider = new FixtureArtifactProvider(FixtureVmProfile.Id, FixtureArtifactWriter.Constant(1));

        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.DeclaresGuestLoads));

        using var runtime = FixtureComposition.Runtime(
            catalog, FixtureComposition.Options(capabilities: FixtureComposition.WithProvider(provider)));

        // Five loads, a yield, then five more: ten in one operation against a fan-out maximum of
        // eight. The suspension must not refresh the count.
        var code = new List<byte>();

        for (var index = 0; index < 5; index++)
        {
            code.Add(FixtureFormat.OpLoad);
            code.Add(0);
        }

        code.Add(FixtureFormat.OpYield);

        for (var index = 0; index < 5; index++)
        {
            code.Add(FixtureFormat.OpLoad);
            code.Add(0);
        }

        code.Add(FixtureFormat.OpPushConst);
        code.Add(0);
        code.Add(FixtureFormat.OpReturn);

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Write([3], code.ToArray()));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var suspended = FixtureComposition.Invoke(instance);
        Assert.True(suspended.TryGetSuspension(out var suspension));

        runtime.Resume(suspension);

        Assert.True(
            provider.RequestCount <= 8,
            $"the suspension refreshed the fan-out count: {provider.RequestCount} requests past a bound of 8");
    }

    // ---- composition guards ------------------------------------------------------------------

    [Fact]
    public void A_Value_Descriptor_Cannot_Smuggle_In_An_Artifact_Provider()
    {
        // The registration and the duplicate guard keyed on different fields, so a provider
        // registered under a Value descriptor was handed to the mediator and was invisible to the
        // at-most-one check - a composition that registered no provider answered loads anyway.
        var provider = new FixtureArtifactProvider(FixtureVmProfile.Id, FixtureArtifactWriter.Constant(1));

        Assert.Throws<ArgumentException>(() =>
            VmCapabilityRegistration.ArtifactProvider(FixtureHostCapabilities.Double, provider));
    }

    // ---- verification -----------------------------------------------------------------------

    [Fact]
    public void An_Escaping_Verifier_Exception_Propagates_Rather_Than_Becoming_An_Invalid_Artifact()
    {
        // Translating it let a verifier bug masquerade as a malicious artifact: the same category
        // and reason were reported for a verifier that dereferenced null and for bytes that were
        // genuinely invalid, so a corpus labelled by (category, reason) could not tell them apart.
        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.ThrowingVerifier));

        using var runtime = FixtureComposition.Runtime(catalog);
        var descriptor = FixtureComposition.Descriptor();

        Assert.Throws<InvalidOperationException>(
            () => runtime.Verify(in descriptor, FixtureArtifactWriter.Constant(1), CancellationToken.None));

        // The runtime is left usable, and the budget already charged stays charged: work was
        // genuinely done, so there is no refund by throwing.
        Assert.Equal(VmRuntimeState.Ready, runtime.State);
    }

    [Fact]
    public void A_Verifier_That_Answers_Normal_With_No_State_Is_A_Core_Defect()
    {
        // The core itself detected the breach, and an artifact cannot cause it, so it cannot be
        // reported as one.
        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.StatelessVerifier));

        using var runtime = FixtureComposition.Runtime(catalog);
        var descriptor = FixtureComposition.Descriptor();

        Assert.Throws<VmCoreDefectException>(
            () => runtime.Verify(in descriptor, FixtureArtifactWriter.Constant(1), CancellationToken.None));

        // The runtime is poisoned, so no later call computes anything from state the core no longer
        // trusts.
        Assert.Equal(VmRuntimeState.Poisoned, runtime.State);

        var after = runtime.Verify(in descriptor, FixtureArtifactWriter.Constant(1), CancellationToken.None);
        Assert.Equal(VmOutcome.InvalidState, after.Outcome);
        Assert.Equal(VmReason.TerminalFault, after.Reason);
    }

    [Fact]
    public void A_Cancelled_Verification_Reports_Cancellation_Before_It_Resolves_A_Profile()
    {
        // Cancellation ranks second, above unsupported profile: the latch is observed before any
        // input is examined, which is what stops a cancelled request naming one category or the
        // other depending on thread timing.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();

        var absent = FixtureComposition.Descriptor(
            SecondFixtureVmProfile.Id, SecondFixtureVmProfile.Manifest);

        var result = runtime.Verify(in absent, FixtureArtifactWriter.Constant(1), cancelled.Token);

        Assert.Equal(VmOutcome.Cancellation, result.Outcome);
    }

    [Fact]
    public void A_Provider_Call_Is_Charged_As_A_Host_Call()
    {
        // A provider is a capability. Without the charge a guest could drive an unbounded number of
        // provider requests against a runtime whose host-call allowance was already spent.
        var provider = new FixtureArtifactProvider(FixtureVmProfile.Id, FixtureArtifactWriter.Constant(1));

        var catalog = FixtureComposition.Catalog(
            FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.DeclaresGuestLoads));

        using var runtime = FixtureComposition.Runtime(
            catalog,
            FixtureComposition.Options(
                FixtureComposition.CeilingsWith(VmBudgetDimension.HostCalls, 0),
                capabilities: FixtureComposition.WithProvider(provider)));

        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.LoadThenConstant(1, 7));
        using var instance = FixtureComposition.Instantiate(runtime, artifact);

        var result = FixtureComposition.Invoke(instance);

        Assert.NotEqual(VmOutcome.Normal, result.Outcome);
        Assert.Equal(0, provider.RequestCount);
    }

    [Fact]
    public void The_Accepted_Manifest_Set_Is_Normalized_In_The_Listing()
    {
        // Declaration order is not retained and has no observable effect anywhere, and the
        // host-facing listing is somewhere it would otherwise have been observable.
        var catalog = FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.MultiManifest));

        Assert.True(catalog.TryGetEntry(FixtureVmProfile.Id, out var entry));

        var manifests = entry.AcceptedFeatureManifests;

        for (var index = 1; index < manifests.Length; index++)
        {
            Assert.True(
                manifests[index - 1].CompareTo(manifests[index]) < 0,
                $"the accepted manifest set is not in ascending order at {index}");
        }
    }

    private static readonly System.Threading.AsyncLocal<object?> AmbientProbe = new();

    [Fact]
    public void A_Disposed_Runtime_Leaves_No_Per_Thread_State_Behind()
    {
        // A runtime kept its capability depth in an async-local, and an async-local entry is
        // released only when it is set to null. A value-typed one never can be: returning the depth
        // to zero stores a boxed zero, which is a present value, so the entry stayed on the thread
        // for the life of the process - one per runtime that ever ran a capability there, released
        // by nothing, not even disposing the runtime.
        //
        // Nothing observable failed, which is why no other test here catches it. What grew was the
        // COST of every later async-local write on that thread, because each one copies the whole
        // map: an operation that allocated ten kilobytes early in a process allocated a megabyte
        // once seventy thousand runtimes had come and gone, and VM-5's benchmark took twelve times
        // as long as it does now. This test is the invariant, not the symptom - a disposed runtime
        // leaves nothing of itself on the thread that used it.
        var baseline = AmbientWriteCost();

        for (var index = 0; index < 500; index++)
        {
            using var runtime = FixtureComposition.Runtime(
                FixtureComposition.AlphaCatalog(),
                FixtureComposition.Options(capabilities: FixtureComposition.ValueCapabilities()));

            var artifact = FixtureComposition.Verify(
                runtime, FixtureArtifactWriter.HostCall(21, FixtureHostCapabilities.DoubleBinding));

            using var instance = FixtureComposition.Instantiate(runtime, artifact);

            Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(instance).Outcome);
        }

        var after = AmbientWriteCost();

        // A quadruple, against a defect that multiplied the figure by five thousand at this scale.
        // The generous factor is deliberate: the failure this guards is unbounded growth, so any
        // threshold that catches it at all catches it enormously, and a tight one would only buy
        // false alarms from a machine having a bad moment.
        Assert.True(
            after <= baseline * 4,
            $"five hundred disposed runtimes left per-thread state behind: an async-local write " +
            $"cost {baseline} bytes before them and {after} bytes after");
    }

    /// <summary>What one async-local write costs on this thread, in bytes.</summary>
    /// <remarks>
    /// The measure is allocation rather than time because the growth is a map copy, and a copy is
    /// exactly as many bytes as the map is large however fast the machine is. Repeated so a single
    /// write's fixed cost does not dominate the difference being looked for.
    /// </remarks>
    private static long AmbientWriteCost()
    {
        const int Writes = 64;

        var marker = new object();

        // Warm, so the first write's one-off costs are not counted as growth.
        AmbientProbe.Value = marker;
        AmbientProbe.Value = null;

        var before = GC.GetAllocatedBytesForCurrentThread();

        for (var write = 0; write < Writes; write++)
        {
            AmbientProbe.Value = marker;
            AmbientProbe.Value = null;
        }

        return (GC.GetAllocatedBytesForCurrentThread() - before) / Writes;
    }

    private static VmAggregateBudget Parent(
        ulong fuel = 10_000_000,
        ulong liveBytes = 1_000_000_000,
        ulong hostCalls = 1_000_000)
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
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.Value(dimension, 8),
                VmBudgetDimension.Fuel => VmCeilingSpec.Value(dimension, fuel),
                VmBudgetDimension.LiveBytes => VmCeilingSpec.Value(dimension, liveBytes),
                VmBudgetDimension.HostCalls => VmCeilingSpec.Value(dimension, hostCalls),
                _ => VmCeilingSpec.Value(dimension, 1_000_000_000),
            });
        }

        return VmAggregateBudget.Create(builder.ToImmutable());
    }
}
