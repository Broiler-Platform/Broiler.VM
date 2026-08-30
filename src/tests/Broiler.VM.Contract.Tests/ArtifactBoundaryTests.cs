using Broiler.VM;
using Broiler.VM.Fixtures;

namespace Broiler.VM.Contract.Tests;

/// <summary>
/// What a verified handle owes its caller once verification has returned: that the bytes it was
/// built from stop mattering, and that a load requested from inside an operation stays inside that
/// operation's bounds.
/// </summary>
public sealed class ArtifactBoundaryTests
{
    [Fact]
    public async Task Overwriting_The_Callers_Buffer_Concurrently_Cannot_Change_What_Was_Verified()
    {
        // Mutation after the fact is already covered; this is the harder half. Another thread
        // rewrites the caller's array continuously while the handle is instantiated and invoked, so
        // a handle that had kept a reference into that array rather than owning a decoded form
        // would produce a different answer, or a torn one, rather than the same answer every time.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var payload = FixtureArtifactWriter.Sum(20, 22);
        var artifact = FixtureComposition.Verify(runtime, payload);

        using var stop = new CancellationTokenSource();

        var vandal = Task.Run(
            () =>
            {
                var pattern = (byte)0;

                while (!stop.Token.IsCancellationRequested)
                {
                    // Deliberately not a single overwrite: a repeated one gives a handle that
                    // aliased the buffer every chance to observe a half-written state.
                    for (var index = 0; index < payload.Length; index++)
                    {
                        payload[index] = pattern;
                    }

                    pattern++;
                }
            },
            CancellationToken.None);

        try
        {
            for (var round = 0; round < 200; round++)
            {
                using var instance = FixtureComposition.Instantiate(runtime, artifact);
                var result = FixtureComposition.Invoke(instance);

                Assert.Equal(VmOutcome.Normal, result.Outcome);
                Assert.True(FixtureVmProfileResults.TryGetValue(in result, out var value));
                Assert.Equal(42, value.Value);
            }
        }
        finally
        {
            await stop.CancelAsync();
            await vandal.WaitAsync(TimeSpan.FromSeconds(10));
        }
    }

    [Fact]
    public void Returning_The_Callers_Buffer_To_A_Pool_And_Reusing_It_Changes_Nothing()
    {
        // The disposal case, in the shape a .NET host actually meets it. A pooled array is the
        // nearest thing the platform has to freeing a buffer: it comes back, it is handed to
        // somebody else, and it is filled with something unrelated.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var source = FixtureArtifactWriter.Sum(20, 22);
        var rented = System.Buffers.ArrayPool<byte>.Shared.Rent(source.Length);

        VmVerifiedArtifact artifact;

        try
        {
            source.CopyTo(rented, 0);
            artifact = FixtureComposition.Verify(runtime, rented[..source.Length]);
        }
        finally
        {
            System.Buffers.ArrayPool<byte>.Shared.Return(rented, clearArray: true);
        }

        // Take it back out and fill it with something else, which is what the next tenant would do.
        var reused = System.Buffers.ArrayPool<byte>.Shared.Rent(source.Length);
        Array.Fill(reused, (byte)0xA5);

        try
        {
            using var instance = FixtureComposition.Instantiate(runtime, artifact);
            var result = FixtureComposition.Invoke(instance);

            Assert.Equal(VmOutcome.Normal, result.Outcome);
            Assert.True(FixtureVmProfileResults.TryGetValue(in result, out var value));
            Assert.Equal(42, value.Value);
        }
        finally
        {
            System.Buffers.ArrayPool<byte>.Shared.Return(reused);
        }
    }

    [Fact]
    public void Verifying_The_Same_Bytes_Twice_Produces_Two_Handles_With_One_Identity()
    {
        // Identity is what the sharing predicate compares and it is a function of the composition,
        // not of the call. The instance identity is not: two handles over identical bytes are two
        // handles, and disposing one may not disturb the other.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var payload = FixtureArtifactWriter.Sum(20, 22);

        var first = FixtureComposition.Verify(runtime, payload);
        var second = FixtureComposition.Verify(runtime, payload);

        Assert.Equal(first.Identity, second.Identity);
        Assert.NotEqual(first.VerifiedArtifactInstanceId, second.VerifiedArtifactInstanceId);

        first.Dispose();

        Assert.Equal(VmVerifiedArtifactState.Disposed, first.State);
        Assert.Equal(VmVerifiedArtifactState.Ready, second.State);

        using var instance = FixtureComposition.Instantiate(runtime, second);
        Assert.Equal(VmOutcome.Normal, FixtureComposition.Invoke(instance).Outcome);
    }

    [Fact]
    public void A_Disposed_Handle_Still_Names_Itself()
    {
        // Reading what a handle WAS is not a use of it. A diagnostic that could not name a disposed
        // handle would be useless exactly when it is needed.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());
        var artifact = FixtureComposition.Verify(runtime, FixtureArtifactWriter.Constant(1));

        var identity = artifact.Identity;
        artifact.Dispose();

        Assert.Equal(identity, artifact.Identity);
        Assert.Equal(VmReason.HandleDisposed, runtime.Instantiate(artifact, CancellationToken.None).Reason);
    }

    [Fact]
    public void The_Cumulative_Nested_Byte_Bound_Terminates_A_Run_Of_Loads()
    {
        // Fan-out counts requests and this counts what they returned. A composition that allowed
        // eight requests of a gigabyte each would be bounded by the first number and unbounded in
        // the thing that matters.
        var answer = FixtureArtifactWriter.Constant(1);
        var provider = new FixtureArtifactProvider(FixtureVmProfile.Id, answer);

        using var runtime = FixtureComposition.Runtime(
            DeclaringCatalog(),
            FixtureComposition.Options(
                capabilities: FixtureComposition.WithProvider(provider),
                guestLoadBounds: VmGuestLoadBoundsSpec.Explicit(
                    new VmGuestLoadBounds(
                        nestedLoadDepth: 4,
                        nestedLoadFanOut: 8,
                        nestedLoadBytes: (ulong)answer.Length + 1,
                        verifierWork: 1_000_000))));

        var result = FixtureComposition.Invoke(
            Instance(runtime, LoadsThenReturns(4)));

        Assert.NotEqual(VmOutcome.Normal, result.Outcome);

        // One answer fitted and the second did not, so the provider was asked exactly twice: the
        // bound stopped the run rather than being noticed after all four had been paid for.
        Assert.Equal(2, provider.RequestCount);
    }

    [Fact]
    public void The_Cumulative_Nested_Verifier_Work_Bound_Terminates_A_Run_Of_Loads()
    {
        // The fourth bound, and the one that was carried in the descriptor and read nowhere until
        // VM-2. It is separate from the operation's own verifier-work allowance and tighter: an
        // operation may spend its whole allowance on the artifact it was handed, and this is all
        // that the loads it requests may consume.
        var provider = new FixtureArtifactProvider(FixtureVmProfile.Id, FixtureArtifactWriter.Constant(1));

        using var runtime = FixtureComposition.Runtime(
            DeclaringCatalog(),
            FixtureComposition.Options(
                capabilities: FixtureComposition.WithProvider(provider),
                guestLoadBounds: VmGuestLoadBoundsSpec.Explicit(
                    new VmGuestLoadBounds(
                        nestedLoadDepth: 4,
                        nestedLoadFanOut: 8,
                        nestedLoadBytes: 64 * 1024,
                        verifierWork: 1))));

        var result = FixtureComposition.Invoke(
            Instance(runtime, LoadsThenReturns(4)));

        Assert.NotEqual(VmOutcome.Normal, result.Outcome);
        Assert.True(provider.RequestCount < 4, $"the provider was asked {provider.RequestCount} times");
    }

    [Fact]
    public void A_Composition_May_Tighten_The_Guest_Load_Bounds_And_May_Never_Loosen_Them()
    {
        // The profile's declared maxima are a hard maximum and not a suggestion. A composition
        // asking for more is refused at runtime creation, where the composition root is on the
        // stack and the mistake is cheap to find.
        var created = VmRuntime.Create(
            DeclaringCatalog(),
            FixtureComposition.Options(
                capabilities: FixtureComposition.WithProvider(
                    new FixtureArtifactProvider(FixtureVmProfile.Id, FixtureArtifactWriter.Constant(1))),
                guestLoadBounds: VmGuestLoadBoundsSpec.Explicit(
                    new VmGuestLoadBounds(
                        nestedLoadDepth: 4,
                        nestedLoadFanOut: 9,
                        nestedLoadBytes: 64 * 1024,
                        verifierWork: 1_000_000))));

        Assert.Equal(VmOutcome.HostFailure, created.Outcome);
        Assert.Equal(VmReason.GuestLoadBoundExceedsProfileMaximum, created.Reason);
    }

    [Fact]
    public void A_Profile_Cannot_Instantiate_What_A_Guest_Load_Returned_So_Nesting_Never_Recurses()
    {
        // The honest state of the depth bound at contract version 1, asserted rather than assumed.
        // A nested load hands the profile a verified handle, and instantiation lives on VmRuntime,
        // which nothing an executor is given can reach - so the profile cannot run what it loaded,
        // a provider is mandatorily non-reentrant, and a verifier is handed no mediator at all.
        // Depth is therefore bounded at one by construction and the configured bound is never the
        // thing that stops a run. Exclusion EX-80 records that VM-2 leaves it untested above one.
        var reachable = typeof(IVmExecutionEnvironment)
            .GetMethods()
            .Concat(typeof(IVmArtifactLoadMediator).GetMethods())
            .Concat(typeof(IVmVerificationContext).GetMethods())
            .Select(static method => method.ReturnType)
            .ToArray();

        Assert.DoesNotContain(typeof(VmRuntime), reachable);
        Assert.DoesNotContain(typeof(VmInstance), reachable);
        Assert.DoesNotContain(typeof(VmInstantiationResult), reachable);

        Assert.Empty(
            typeof(VmGuestLoadResult)
                .GetMethods()
                .Where(static method => method.ReturnType == typeof(VmInstantiationResult)));
    }

    [Fact]
    public void An_In_Bounds_Allocation_Is_Not_A_Broken_Poll_Contract()
    {
        // The regression for the unit conflation the corpus found. The uncharged-work counter used
        // to take every dimension, so one correctly metered allocation of half a megabyte breached
        // a poll bound of a thousand instantly - and the poll-bound path reports a profile fault
        // and poisons the runtime, so a core defect was billed to the profile.
        using var runtime = FixtureComposition.Runtime(FixtureComposition.AlphaCatalog());

        var payload = FixtureArtifactWriter.Allocate(65_536);
        var artifact = FixtureComposition.Verify(runtime, payload);

        using var instance = FixtureComposition.Instantiate(runtime, artifact);
        var result = FixtureComposition.Invoke(instance);

        Assert.Equal(VmOutcome.Normal, result.Outcome);
        Assert.NotEqual(VmReason.CancellationPollBoundExceeded, result.Reason);
        Assert.Equal(VmRuntimeState.Ready, runtime.State);
    }

    [Fact]
    public void A_Bounded_Fuzz_Session_Finds_No_Counterexample()
    {
        // The fuzz target, run in the suite at a size a test can afford. The long sessions belong to
        // Broiler.VM.Fuzz.Host and their results to the evidence bundle; what this asserts is that
        // the target still runs, still reaches more than one answer, and still finds nothing - so a
        // target that had quietly stopped exercising the verifier would be visible here rather than
        // in a report nobody reruns.
        var seeds = Directory
            .GetFiles(FixtureCorpusStore.Directory(CorpusRunner.Root), "*" + FixtureCorpusStore.ArtifactExtension)
            .OrderBy(static path => path, StringComparer.Ordinal)
            .Select(File.ReadAllBytes)
            .ToArray();

        Assert.NotEmpty(seeds);

        var mutator = new FixtureFuzzMutator(0xC0FFEE);
        var outcomes = new HashSet<VmOutcome>();
        var violations = new List<string>();

        for (var iteration = 0; iteration < 4_000; iteration++)
        {
            var input = mutator.Next(seeds);
            var observation = Observe(input);

            outcomes.Add(observation.Outcome);
            violations.AddRange(
                FixtureFuzzInvariants.Violations(observation, millisecondBudget: 2_000)
                    .Select(violation => $"iteration {iteration}: {violation}"));

            if (violations.Count > 0)
            {
                break;
            }
        }

        Assert.Empty(violations);

        // A session that only ever answered one way exercised one path, and reporting it as clean
        // would let a broken seed corpus read as four thousand successful iterations.
        Assert.True(outcomes.Count > 1, "every iteration answered the same way");
    }

    private static FixtureFuzzObservation Observe(byte[] input)
    {
        var clock = System.Diagnostics.Stopwatch.StartNew();

        var outcome = VmOutcome.None;
        var producedHandle = false;
        var escaped = false;
        var escapedTypeName = string.Empty;
        CorpusObservation? observation = null;

        try
        {
            observation = CorpusRunner.Run(input, FixtureFormat.FormatVersion, artifactBytesRequest: 0);
            outcome = observation.Outcome;
            producedHandle = observation.ProducedHandle;
        }
        catch (Exception exception)
        {
            escaped = true;
            escapedTypeName = exception.GetType().FullName ?? "unknown";
        }

        clock.Stop();

        var recorder = observation?.Recorder;

        return new FixtureFuzzObservation(
            outcome,
            producedHandle,
            escaped,
            escapedTypeName,
            recorder?.ReservedBytes ?? 0,
            recorder?.PolicyObserved == true
                ? recorder.ObservedPolicy[VmBudgetDimension.AllocatedBytes]
                : 0,
            recorder?.PolicyObserved ?? false,
            recorder?.PolicyPrecededEveryRead ?? true,
            recorder?.NoReservationPrecededTheFirstRead ?? true,
            clock.ElapsedMilliseconds);
    }

    private static VmCatalog DeclaringCatalog() =>
        FixtureComposition.Catalog(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.DeclaresGuestLoads));

    private static VmInstance Instance(VmRuntime runtime, byte[] payload) =>
        FixtureComposition.Instantiate(runtime, FixtureComposition.Verify(runtime, payload));

    private static byte[] LoadsThenReturns(int loads)
    {
        var code = new List<byte>();

        for (var index = 0; index < loads; index++)
        {
            code.Add(FixtureFormat.OpLoad);
            code.Add(0);
        }

        code.Add(FixtureFormat.OpPushConst);
        code.Add(0);
        code.Add(FixtureFormat.OpReturn);

        return FixtureArtifactWriter.Write([3], code.ToArray());
    }
}
