using Broiler.VM;
using Broiler.VM.Fixtures;
using System.Collections.Immutable;

namespace Broiler.VM.Fixtures.Host;

/// <summary>
/// The test-only composition root: it names two fixture profiles by their own descriptor
/// accessors, builds a catalog, creates a runtime, and runs a fixture artifact end to end.
/// </summary>
/// <remarks>
/// <para>
/// This exists to be <em>published and run</em>, trimmed and under Native AOT. Invariant 7
/// classifies analyzer success and a trimmed build as inputs: a composition is demonstrated only by
/// publishing it and running its representative workload. A host that compiled but was never
/// executed would prove that the linker was satisfied, which is not the claim.
/// </para>
/// <para>
/// It names each profile through a direct static accessor on that profile's own type. There is no
/// aggregate type listing several profiles, because one would reference every profile assembly and
/// defeat the exact-closure reporting a composition depends on.
/// </para>
/// </remarks>
internal static class Program
{
    private static int Main(string[] args)
    {
        var verbose = args.Contains("--verbose", StringComparer.Ordinal);

        try
        {
            var corpus = Value(args, "--corpus");

            if (corpus is not null)
            {
                return ReplayCorpus(corpus);
            }

            var checks = new List<(string Name, bool Passed, string Detail)>
            {
                SingleProfileComposition(),
                TwoProfileComposition(),
                SuspendAndResume(),
                DeterministicGuestLoadRefusal(),
                UnsupportedProfileIsNotAnInvalidArtifact(),
            };

            var failed = 0;

            foreach (var (name, passed, detail) in checks)
            {
                if (!passed)
                {
                    failed++;
                }

                if (verbose || !passed)
                {
                    Console.WriteLine($"{(passed ? "ok  " : "FAIL")} {name}: {detail}");
                }
            }

            Console.WriteLine(
                failed == 0
                    ? $"broiler-vm-fixtures-host: {checks.Count} checks passed, core contract version {VmCoreContract.Version}"
                    : $"broiler-vm-fixtures-host: {failed} of {checks.Count} checks FAILED");

            return failed == 0 ? 0 : 1;
        }
        catch (Exception failure)
        {
            Console.WriteLine($"broiler-vm-fixtures-host: unhandled {failure.GetType().Name}: {failure.Message}");
            return 2;
        }
    }

    /// <summary>One profile, named directly, verified, instantiated and invoked.</summary>
    private static (string, bool, string) SingleProfileComposition()
    {
        var catalog = VmCatalog.CreateBuilder()
            .Add(FixtureVmProfile.Descriptor)
            .Build();

        var created = VmRuntime.Create(catalog, Options());

        if (!created.TryGetRuntime(out var runtime))
        {
            return ("single-profile", false, $"runtime creation {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            var descriptor = Descriptor(FixtureVmProfile.Id, FixtureVmProfile.Manifest);
            var verified = runtime.Verify(in descriptor, FixtureArtifactWriter.Sum(20, 22), CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                return ("single-profile", false, $"verification {verified.Outcome}/{verified.Reason}");
            }

            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                return ("single-profile", false, $"instantiation {instantiated.Outcome}/{instantiated.Reason}");
            }

            var request = new VmInvocationRequest(new VmUtf8Text("main"u8));
            var result = instance.Invoke(in request, CancellationToken.None);

            if (!FixtureVmProfileResults.TryGetValue(in result, out var value) || value.Value != 42)
            {
                return ("single-profile", false, $"invocation {result.Outcome}/{result.Reason}");
            }

            return ("single-profile", true, $"returned {value.Value}");
        }
    }

    /// <summary>Two profiles in one composition, each reached by naming its own descriptor.</summary>
    private static (string, bool, string) TwoProfileComposition()
    {
        var catalog = VmCatalog.CreateBuilder()
            .Add(FixtureVmProfile.Descriptor)
            .Add(SecondFixtureVmProfile.Descriptor)
            .Build();

        var created = VmRuntime.Create(catalog, Options());

        if (!created.TryGetRuntime(out var runtime))
        {
            return ("two-profile", false, $"runtime creation {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            foreach (var (id, manifest) in new[]
            {
                (FixtureVmProfile.Id, FixtureVmProfile.Manifest),
                (SecondFixtureVmProfile.Id, SecondFixtureVmProfile.Manifest),
            })
            {
                var descriptor = Descriptor(id, manifest);
                var verified = runtime.Verify(in descriptor, FixtureArtifactWriter.Constant(7), CancellationToken.None);

                if (!verified.TryGetArtifact(out _))
                {
                    return ("two-profile", false, $"{id} verification {verified.Outcome}/{verified.Reason}");
                }
            }

            return ("two-profile", true, $"{catalog.Count} profiles composed");
        }
    }

    /// <summary>A guest suspension and its resumption through the single resume entry point.</summary>
    private static (string, bool, string) SuspendAndResume()
    {
        var catalog = VmCatalog.CreateBuilder().Add(FixtureVmProfile.Descriptor).Build();
        var created = VmRuntime.Create(catalog, Options());

        if (!created.TryGetRuntime(out var runtime))
        {
            return ("suspend-resume", false, $"runtime creation {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            var descriptor = Descriptor(FixtureVmProfile.Id, FixtureVmProfile.Manifest);
            var verified = runtime.Verify(in descriptor, FixtureArtifactWriter.YieldThenConstant(17), CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                return ("suspend-resume", false, $"verification {verified.Outcome}/{verified.Reason}");
            }

            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                return ("suspend-resume", false, $"instantiation {instantiated.Outcome}/{instantiated.Reason}");
            }

            var request = new VmInvocationRequest(new VmUtf8Text("main"u8));
            var suspended = instance.Invoke(in request, CancellationToken.None);

            if (!suspended.TryGetSuspension(out var suspension))
            {
                return ("suspend-resume", false, $"expected a suspension, got {suspended.Outcome}/{suspended.Reason}");
            }

            var resumed = runtime.Resume(suspension);

            if (!FixtureVmProfileResults.TryGetValue(in resumed, out var value) || value.Value != 17)
            {
                return ("suspend-resume", false, $"resume {resumed.Outcome}/{resumed.Reason}");
            }

            return ("suspend-resume", true, $"resumed with {value.Value}");
        }
    }

    /// <summary>
    /// A composition that registers no artifact provider refuses every guest-initiated load.
    /// </summary>
    /// <remarks>
    /// This is the shape a content policy takes: the refusal is a contract outcome of a composition
    /// that declined to register a capability, not a check inside an engine.
    /// </remarks>
    private static (string, bool, string) DeterministicGuestLoadRefusal()
    {
        var catalog = VmCatalog.CreateBuilder()
            .Add(FixtureVmProfile.DescriptorFor(FixtureVmProfileVariant.DeclaresGuestLoads))
            .Build();

        var created = VmRuntime.Create(catalog, Options());

        if (!created.TryGetRuntime(out var runtime))
        {
            return ("no-provider-refusal", false, $"runtime creation {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            var descriptor = Descriptor(FixtureVmProfile.Id, FixtureVmProfile.Manifest);
            var verified = runtime.Verify(in descriptor, FixtureArtifactWriter.LoadThenConstant(1, 7), CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                return ("no-provider-refusal", false, $"verification {verified.Outcome}/{verified.Reason}");
            }

            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                return ("no-provider-refusal", false, $"instantiation {instantiated.Outcome}/{instantiated.Reason}");
            }

            var request = new VmInvocationRequest(new VmUtf8Text("main"u8));
            var first = instance.Invoke(in request, CancellationToken.None);
            var second = instance.Invoke(in request, CancellationToken.None);

            if (first.Outcome is VmOutcome.Normal || first.Outcome != second.Outcome || first.Reason != second.Reason)
            {
                return ("no-provider-refusal", false, $"not deterministic: {first.Outcome}/{first.Reason} then {second.Outcome}/{second.Reason}");
            }

            return ("no-provider-refusal", true, $"refused twice as {first.Outcome}/{first.Reason}");
        }
    }

    /// <summary>A profile the composition does not contain is its own outcome, not a corrupt file.</summary>
    private static (string, bool, string) UnsupportedProfileIsNotAnInvalidArtifact()
    {
        var catalog = VmCatalog.CreateBuilder().Add(FixtureVmProfile.Descriptor).Build();
        var created = VmRuntime.Create(catalog, Options());

        if (!created.TryGetRuntime(out var runtime))
        {
            return ("unsupported-profile", false, $"runtime creation {created.Outcome}/{created.Reason}");
        }

        using (runtime)
        {
            var descriptor = Descriptor(SecondFixtureVmProfile.Id, SecondFixtureVmProfile.Manifest);
            var result = runtime.Verify(in descriptor, FixtureArtifactWriter.Constant(1), CancellationToken.None);

            return result.Outcome is VmOutcome.UnsupportedProfile
                ? ("unsupported-profile", true, $"{result.Outcome}/{result.Reason}")
                : ("unsupported-profile", false, $"expected UnsupportedProfile, got {result.Outcome}/{result.Reason}");
        }
    }

    /// <summary>
    /// Replays every retained corpus artifact and prints one line per artifact: the identifier, the
    /// outcome, the reason, the profile's diagnostic code, and the dimension and scope a resource
    /// answer named.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the whole of the cross-mode stability claim. The same table is produced by this host
    /// published three ways - JIT, trimmed, and Native AOT - and the three are compared byte for
    /// byte. A category that differs between them is a category a host cannot rely on, and an
    /// enumeration rendered by name rather than by number is exactly the kind of thing trimming and
    /// AOT change without warning.
    /// </para>
    /// <para>
    /// One descriptor for every artifact, deliberately. Three corpus entries are presented under a
    /// varied descriptor by the behavioural suite, and reproducing that here would mean reading the
    /// manifest - which means a JSON reader in a trimmed and AOT-published binary, whose own
    /// behaviour under trimming is then part of what the table measures.
    /// </para>
    /// </remarks>
    private static int ReplayCorpus(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Console.Error.WriteLine($"broiler-vm-fixtures-host: no corpus directory at {directory}");
            return 2;
        }

        var files = Directory.GetFiles(directory, "*.bin");
        Array.Sort(files, StringComparer.Ordinal);

        if (files.Length == 0)
        {
            Console.Error.WriteLine($"broiler-vm-fixtures-host: no corpus artifacts in {directory}");
            return 2;
        }

        Console.WriteLine($"# broiler-vm-corpus-replay core-contract-version={VmCoreContract.Version} artifacts={files.Length}");

        foreach (var file in files)
        {
            var payload = File.ReadAllBytes(file);
            var catalog = VmCatalog.CreateBuilder().Add(FixtureVmProfile.Descriptor).Build();
            var created = VmRuntime.Create(catalog, Options());

            if (!created.TryGetRuntime(out var runtime))
            {
                Console.Error.WriteLine(
                    $"broiler-vm-fixtures-host: runtime creation {created.Outcome}/{created.Reason}");

                return 2;
            }

            using (runtime)
            {
                var descriptor = Descriptor(FixtureVmProfile.Id, FixtureVmProfile.Manifest);
                var result = runtime.Verify(in descriptor, payload, CancellationToken.None);
                var handle = result.TryGetArtifact(out var artifact);

                artifact?.Dispose();

                Console.WriteLine(
                    string.Join(
                        ' ',
                        Path.GetFileNameWithoutExtension(file),
                        result.Outcome,
                        result.Reason,
                        result.Diagnostics.ProfileDiagnosticCode,
                        result.Diagnostics.ExhaustedDimension,
                        result.Diagnostics.ExhaustedScope,
                        handle ? "handle" : "no-handle"));
            }
        }

        return 0;
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

    private static VmArtifactDescriptor Descriptor(VmProfileId profileId, VmFeatureManifestId manifest) =>
        new(profileId, FixtureFormat.FormatVersion, manifest, default,
            VmCallerIdentity.FromCanonicalIdentity("fixtures-host://artifact"));

    private static VmRuntimeCreationOptions Options()
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
            FixtureHostCapabilities.Double, FixtureHostCapabilities.DoubleHandler));
        capabilities.Add(VmCapabilityRegistration.Value(
            FixtureHostCapabilities.Throwing, FixtureHostCapabilities.ThrowingHandler));
        capabilities.Add(VmCapabilityRegistration.Value(
            FixtureHostCapabilities.Refusing, FixtureHostCapabilities.RefusingHandler));

        return new VmRuntimeCreationOptions(
            aggregateBudget: null,
            ceilings: ceilings.ToImmutable(),
            maxSuspendedResidency: TimeSpan.FromMinutes(1),
            maxLiveSuspendedOperations: 4,
            guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
            externalSuspension: VmExternalSuspensionMode.Disabled,
            capabilities: capabilities.ToImmutable());
    }
}
