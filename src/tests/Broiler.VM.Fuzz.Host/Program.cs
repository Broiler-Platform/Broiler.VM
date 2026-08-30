using Broiler.VM;
using Broiler.VM.Fixtures;
using System.Collections.Immutable;

namespace Broiler.VM.Fuzz.Host;

/// <summary>
/// The fuzz target host: a composition root that drives mutated artifact bytes through the one
/// verification entry point and looks for a counterexample to the load-stage invariants.
/// </summary>
/// <remarks>
/// <para>
/// It finds nothing by itself. What it produces when it does find something is the valuable part: a
/// minimized input written into the retained corpus, where the behavioural suite picks it up on the
/// next run and keeps checking it forever. A fuzz finding that lives only in a console log is an
/// anecdote.
/// </para>
/// <para>
/// A session is a total function of its seed and the corpus it seeds from, so a finding is
/// reproduced by naming both. There is no wall-clock budget and no thread count: those would make
/// the same session behave differently on two machines, which is the nondeterministic failure class
/// this component's own gate forbids.
/// </para>
/// </remarks>
internal static class Program
{
    private const long IterationMillisecondBudget = 2_000;

    private static int Main(string[] arguments)
    {
        var iterations = Argument(arguments, "--iterations", 20_000);
        var seed = (ulong)Argument(arguments, "--seed", 1);
        var root = FindRoot();

        var replay = Value(arguments, "--replay");

        if (replay is not null)
        {
            return Replay(replay);
        }

        var seeds = LoadSeedCorpus(root);

        if (seeds.Count == 0)
        {
            Console.Error.WriteLine(
                "The seed corpus is empty. Generate it with " +
                $"{FixtureCorpusStore.WriteVariable}=1 dotnet test Broiler.VM.slnx -c Release.");

            return 2;
        }

        Console.WriteLine(
            $"Broiler.VM fuzz target: {iterations} iterations, seed {seed}, {seeds.Count} seed artifacts.");

        var mutator = new FixtureFuzzMutator(seed);
        var histogram = new Dictionary<VmOutcome, int>();

        for (var iteration = 0; iteration < iterations; iteration++)
        {
            var input = mutator.Next(seeds);
            var observation = Verify(input);

            histogram[observation.Outcome] = histogram.GetValueOrDefault(observation.Outcome) + 1;

            var violations = FixtureFuzzInvariants.Violations(observation, IterationMillisecondBudget);

            if (violations.Count == 0)
            {
                continue;
            }

            Console.Error.WriteLine($"Finding at iteration {iteration} of session seed {seed}:");

            foreach (var violation in violations)
            {
                Console.Error.WriteLine("  " + violation);
            }

            var minimized = Minimize(input);
            var written = Retain(root, minimized);

            Console.Error.WriteLine($"  minimized from {input.Length} to {minimized.Length} bytes");
            Console.Error.WriteLine($"  retained as {written}");
            Console.Error.WriteLine(
                "  Add it to the manifest with " +
                $"{FixtureCorpusStore.WriteVariable}=1 dotnet test Broiler.VM.slnx -c Release, " +
                "after changing its provenance to Minimized.");

            return 1;
        }

        Console.WriteLine("No counterexample found. Outcome histogram:");

        foreach (var pair in histogram.OrderBy(static entry => entry.Key.ToString(), StringComparer.Ordinal))
        {
            Console.WriteLine($"  {pair.Key}: {pair.Value}");
        }

        // A histogram with one entry is a session that never reached the verifier, which is a
        // finding about the session rather than about the core - and reporting it as success would
        // let a broken seed corpus read as twenty thousand clean iterations.
        if (histogram.Count < 2)
        {
            Console.Error.WriteLine(
                "Every iteration answered the same way. The session exercised one path and proves nothing.");

            return 3;
        }

        return 0;
    }

    private static int Replay(string path)
    {
        var input = File.ReadAllBytes(path);
        var observation = Verify(input);
        var violations = FixtureFuzzInvariants.Violations(observation, IterationMillisecondBudget);

        Console.WriteLine($"{path}: {observation.Outcome}, {input.Length} bytes, {observation.ElapsedMilliseconds}ms");

        foreach (var violation in violations)
        {
            Console.Error.WriteLine("  " + violation);
        }

        return violations.Count == 0 ? 0 : 1;
    }

    /// <summary>
    /// Reduces a failing input while it keeps failing, so what is retained is the smallest thing
    /// that still shows the defect.
    /// </summary>
    /// <remarks>
    /// Delete-a-window first and then byte-by-byte, both greedy. It is not the smallest possible
    /// reduction and it does not need to be: what a reviewer needs is an input small enough to read,
    /// and what the corpus needs is bytes that still break the invariant.
    /// </remarks>
    private static byte[] Minimize(byte[] input)
    {
        var current = input;

        for (var window = current.Length / 2; window >= 1; window /= 2)
        {
            var progress = true;

            while (progress)
            {
                progress = false;

                for (var start = 0; start + window <= current.Length; start++)
                {
                    var candidate = new byte[current.Length - window];
                    Array.Copy(current, 0, candidate, 0, start);
                    Array.Copy(current, start + window, candidate, start, current.Length - start - window);

                    if (StillFails(candidate))
                    {
                        current = candidate;
                        progress = true;
                        break;
                    }
                }
            }
        }

        return current;
    }

    private static bool StillFails(byte[] candidate) =>
        FixtureFuzzInvariants.Violations(Verify(candidate), IterationMillisecondBudget).Count > 0;

    private static string Retain(string root, byte[] minimized)
    {
        var directory = FixtureCorpusStore.Directory(root);
        Directory.CreateDirectory(directory);

        var name = "minimized-" + FixtureCorpusStore.Hash(minimized)[..16] + FixtureCorpusStore.ArtifactExtension;
        File.WriteAllBytes(Path.Combine(directory, name), minimized);

        return FixtureCorpusStore.RelativeDirectory + "/" + name;
    }

    /// <summary>
    /// One verification, in its own runtime, with the read-order recorder attached.
    /// </summary>
    /// <remarks>
    /// A fresh runtime per input deliberately: an allowance never refunds, so a shared runtime would
    /// make an input's answer depend on which inputs ran before it, and a finding would not
    /// reproduce from its own bytes.
    /// </remarks>
    private static FixtureFuzzObservation Verify(byte[] input)
    {
        var recorder = new FixtureReadOrderRecorder();
        var clock = System.Diagnostics.Stopwatch.StartNew();

        var outcome = VmOutcome.None;
        var producedHandle = false;
        var escaped = false;
        var escapedTypeName = string.Empty;

        // The composition is built OUTSIDE the guarded region. A runtime this host cannot create is
        // a defect in this host, and reporting it as an escaping exception would have every
        // iteration of a mis-wired session look like a finding about the core.
        using var runtime = Runtime(Catalog(recorder));

        var descriptor = new VmArtifactDescriptor(
            FixtureVmProfile.Id,
            FixtureFormat.FormatVersion,
            FixtureVmProfile.Manifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity("fuzz://vm-2"));

        try
        {
            var result = runtime.Verify(in descriptor, input, CancellationToken.None);

            outcome = result.Outcome;
            producedHandle = result.TryGetArtifact(out var artifact);
            artifact?.Dispose();
        }
        catch (Exception exception)
        {
            escaped = true;
            escapedTypeName = exception.GetType().FullName ?? "unknown";
        }

        clock.Stop();

        return new FixtureFuzzObservation(
            outcome,
            producedHandle,
            escaped,
            escapedTypeName,
            recorder.ReservedBytes,
            recorder.PolicyObserved ? recorder.ObservedPolicy[VmBudgetDimension.AllocatedBytes] : 0,
            recorder.PolicyObserved,
            recorder.PolicyPrecededEveryRead,
            recorder.NoReservationPrecededTheFirstRead,
            clock.ElapsedMilliseconds);
    }

    private static VmCatalog Catalog(FixtureReadOrderRecorder recorder)
    {
        var builder = VmCatalog.CreateBuilder();
        builder.Add(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.Conforming, recorder));
        return builder.Build();
    }

    private static VmRuntime Runtime(VmCatalog catalog)
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension is VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var options = new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 4,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: Capabilities());

        var created = VmRuntime.Create(catalog, options);

        if (!created.TryGetRuntime(out var runtime))
        {
            throw new InvalidOperationException(
                $"The fuzz composition could not create a runtime: {created.Outcome}/{created.Reason}.");
        }

        return runtime;
    }

    /// <summary>
    /// The value capabilities the fixture profile declares imports for.
    /// </summary>
    /// <remarks>
    /// Binding is the composition root's job and a profile package cannot construct a registration,
    /// so the list lives here. No artifact provider is registered: a provider is a separate
    /// capability kind, and a fuzz session over the verification stage has no use for one.
    /// </remarks>
    private static ImmutableArray<VmCapabilityRegistration> Capabilities()
    {
        var builder = ImmutableArray.CreateBuilder<VmCapabilityRegistration>();

        builder.Add(VmCapabilityRegistration.Value(
            FixtureHostCapabilities.Double, FixtureHostCapabilities.DoubleHandler));
        builder.Add(VmCapabilityRegistration.Value(
            FixtureHostCapabilities.Throwing, FixtureHostCapabilities.ThrowingHandler));
        builder.Add(VmCapabilityRegistration.Value(
            FixtureHostCapabilities.Refusing, FixtureHostCapabilities.RefusingHandler));

        return builder.ToImmutable();
    }

    private static IReadOnlyList<byte[]> LoadSeedCorpus(string root)
    {
        var directory = FixtureCorpusStore.Directory(root);

        if (!Directory.Exists(directory))
        {
            return [];
        }

        return Directory
            .GetFiles(directory, "*" + FixtureCorpusStore.ArtifactExtension)
            .OrderBy(static path => path, StringComparer.Ordinal)
            .Select(File.ReadAllBytes)
            .ToArray();
    }

    private static string? Value(string[] arguments, string name)
    {
        for (var index = 0; index + 1 < arguments.Length; index++)
        {
            if (string.Equals(arguments[index], name, StringComparison.Ordinal))
            {
                return arguments[index + 1];
            }
        }

        return null;
    }

    private static long Argument(string[] arguments, string name, long fallback) =>
        long.TryParse(Value(arguments, name), out var parsed) ? parsed : fallback;

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Broiler.VM.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("No directory above the fuzz host holds Broiler.VM.slnx.");
    }
}
