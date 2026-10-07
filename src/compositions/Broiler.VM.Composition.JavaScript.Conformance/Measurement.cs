using Broiler.VM.Abstractions;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// The measurement lane's children (phase F9 slice R1, JSD-0059): the two figures roadmap sections
/// 16 and 18 open their questions against, measured under the rules of roadmap section 17.
/// </summary>
/// <remarks>
/// <para>
/// <b>Verification throughput per byte</b> is measured here, in one process: the candidate verifies
/// the artifact the lowering writes for a named source, and the control is the same bytes passed
/// through a checksum, so the difference is what verifying costs over reading the bytes at all. The
/// lowering runs once, before anything is timed.
/// </para>
/// <para>
/// <b>Cold-start cost</b> is a property of a process, so it is measured by the parent
/// (<c>eng/measure-js-baselines.py</c>), which launches this root as a child: <c>--cold-start</c>
/// composes the runtime, lowers a one-statement script, verifies it, instantiates it and runs it to
/// its completion, and <c>--cold-start-control</c> is the same process doing none of that. Both
/// report the configuration the runtime actually took, because roadmap section 17's rule 7 asks for
/// the effective configuration rather than the requested one.
/// </para>
/// <para>
/// <b>The harness is the core's, restated</b>: interleaved candidate and control, an A/A lane,
/// seven repetitions all retained, no outlier policy and no statistical model, and a condition
/// checked before and after every lane. It is restated rather than referenced because the core's
/// lives in a test project, which a composition root may not reference and which may not reference
/// a profile (rule A11).
/// </para>
/// </remarks>
internal static class Measurement
{
    /// <summary>How many repetitions each lane runs. Every one is retained (roadmap section 17, JS-4's count).</summary>
    internal const int Repetitions = 7;

    /// <summary>How many untimed iterations run before the first timed one.</summary>
    internal const int WarmupIterations = 5;

    /// <summary>How many verifications one timed lane makes.</summary>
    internal const int VerifyIterations = 20;

    /// <summary>The script a cold start runs, and the completion it must answer.</summary>
    internal const string ColdStartSource = "1 + 1";

    private const string ColdStartCompletion = "2";

    private const string Caller = "broiler-js-conformance://measurement";

    /// <summary>The configuration the runtime took, as each child reports it: roadmap section 17's rule 7.</summary>
    internal static string Configuration() =>
        $"rid={RuntimeInformation.RuntimeIdentifier} " +
        $"arch={RuntimeInformation.ProcessArchitecture} " +
        $"gc={(GCSettings.IsServerGC ? "server" : "workstation")} " +
        $"concurrent={Setting("System.GC.Concurrent")} " +
        $"tiered={Setting("System.Runtime.TieredCompilation")} " +
        $"aot={(RuntimeFeature.IsDynamicCodeSupported ? "no" : "yes")}";

    private static string Setting(string name) =>
        AppContext.GetData(name)?.ToString()?.ToLowerInvariant() ?? "default";

    /// <summary>The cold-start candidate: compose, lower, verify, instantiate, run, and answer.</summary>
    internal static int ColdStart()
    {
        var completion = Run(Compile(ColdStartSource), out var problem);

        if (completion != ColdStartCompletion)
        {
            Console.WriteLine($"cold-start FAILED: {problem ?? "completion " + completion}");
            return 1;
        }

        Console.WriteLine($"cold-start completion={completion} {Configuration()}");
        return 0;
    }

    /// <summary>The cold-start control: the same process, doing none of the profile's work.</summary>
    internal static int ColdStartControl()
    {
        Console.WriteLine($"cold-start-control {Configuration()}");
        return 0;
    }

    /// <summary>
    /// Verification throughput per byte over the artifact the lowering writes for
    /// <paramref name="sourcePath"/>, against a checksum of the same bytes.
    /// </summary>
    internal static int VerifyThroughput(string sourcePath)
    {
        var source = File.ReadAllText(sourcePath);
        var bytes = Compile(source);

        if (bytes is null)
        {
            Console.WriteLine($"verify-throughput FAILED: {sourcePath} does not lower");
            return 1;
        }

        Console.WriteLine($"configuration {Configuration()}");
        Console.WriteLine($"workload source={Path.GetFileName(sourcePath)} source-bytes={System.Text.Encoding.UTF8.GetByteCount(source)} artifact-bytes={bytes.Length} artifact-sha256={Convert.ToHexStringLower(System.Security.Cryptography.SHA256.HashData(bytes))}");

        VmRuntime? runtime = null;
        ulong checksum = 0;

        void Reset()
        {
            runtime?.Dispose();
            runtime = NewRuntime();
        }

        void Verify()
        {
            var descriptor = ArtifactDescriptor();
            var verified = runtime!.Verify(in descriptor, bytes, CancellationToken.None);

            if (verified.TryGetArtifact(out var artifact))
            {
                artifact.Dispose();
            }
        }

        void Checksum()
        {
            // FNV-1a over every byte: a pass that reads what the verifier reads and judges nothing.
            var hash = 14695981039346656037UL;

            foreach (var b in bytes)
            {
                hash = (hash ^ b) * 1099511628211UL;
            }

            checksum ^= hash;
        }

        string? Condition()
        {
            var descriptor = ArtifactDescriptor();
            var verified = runtime!.Verify(in descriptor, bytes, CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                return $"verification refused: {verified.Outcome}/{verified.Reason}";
            }

            artifact.Dispose();
            return null;
        }

        Reset();

        var result = Harness(
            "verify-throughput", "byte", bytes.Length, Verify, Checksum, VerifyIterations, Condition, Reset);

        runtime!.Dispose();
        GC.KeepAlive(checksum);
        return result;
    }

    /// <summary>
    /// The core's harness, restated: interleaved candidate, control and A/A lanes, every repetition
    /// printed, and the condition checked before anything and after every lane.
    /// </summary>
    private static int Harness(
        string id, string unit, long unitsPerIteration, Action candidate, Action control, int iterations,
        Func<string?> condition, Action reset)
    {
        if (condition() is { } before)
        {
            Console.WriteLine($"measurement {id} FAILED before any lane: {before}");
            return 1;
        }

        for (var i = 0; i < WarmupIterations; i++)
        {
            candidate();
            control();
        }

        var candidates = new double[Repetitions];
        var controls = new double[Repetitions];
        var second = new double[Repetitions];

        for (var repetition = 0; repetition < Repetitions; repetition++)
        {
            foreach (var (lane, into) in new[] { (candidate, candidates), (control, controls), (candidate, second) })
            {
                reset();
                into[repetition] = Time(lane, iterations);

                if (condition() is { } after)
                {
                    Console.WriteLine($"measurement {id} FAILED after a lane: {after}");
                    return 1;
                }
            }
        }

        var candidateTime = Median(candidates);
        var controlTime = Median(controls);
        var difference = candidateTime - controlTime;
        var floor = Math.Abs(candidateTime - Median(second));
        var resolved = floor <= Math.Abs(difference);

        Console.WriteLine(
            $"measurement {id} unit={unit} " +
            $"candidate-ns={candidateTime:F1} control-ns={controlTime:F1} " +
            $"difference-ns={difference:F1} per-{unit}-ns={difference / unitsPerIteration:F4} " +
            $"aa-ns={floor:F1} valid={(resolved ? "yes" : "no")} " +
            (resolved ? string.Empty : $"upper-bound-per-{unit}-ns={floor / unitsPerIteration:F4} ") +
            $"iterations={iterations} repetitions={Repetitions}");

        for (var repetition = 0; repetition < Repetitions; repetition++)
        {
            Console.WriteLine(
                $"  rep {id} {repetition} candidate-ns={candidates[repetition]:F1} " +
                $"control-ns={controls[repetition]:F1} aa-ns={second[repetition]:F1}");
        }

        return 0;
    }

    private static double Time(Action action, int iterations)
    {
        var start = Stopwatch.GetTimestamp();

        for (var i = 0; i < iterations; i++)
        {
            action();
        }

        return Stopwatch.GetElapsedTime(start).TotalNanoseconds / iterations;
    }

    private static double Median(double[] values)
    {
        var sorted = (double[])values.Clone();
        Array.Sort(sorted);
        return sorted[sorted.Length / 2];
    }

    private static byte[]? Compile(string source)
    {
        var compiled = JsCompiler.Compile(
            [new JsScriptUnit("main", source, SliceParseOptions.Script, false, Caller)], [], new JsCompileRequest());

        return compiled.Succeeded ? compiled.Artifact : null;
    }

    private static string? Run(byte[]? bytes, out string? problem)
    {
        problem = null;

        if (bytes is null)
        {
            problem = "the script does not lower";
            return null;
        }

        using var runtime = NewRuntime();
        var descriptor = ArtifactDescriptor();
        var verified = runtime.Verify(in descriptor, bytes, CancellationToken.None);

        if (!verified.TryGetArtifact(out var artifact))
        {
            problem = $"verification refused: {verified.Outcome}/{verified.Reason}";
            return null;
        }

        using (artifact)
        {
            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                problem = $"instantiation refused: {instantiated.Outcome}/{instantiated.Reason}";
                return null;
            }

            using (instance)
            {
                var request = new VmInvocationRequest(new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("main")));
                var result = instance.Invoke(in request, CancellationToken.None);

                if (JavaScriptProfile.TryGetWideCompletion(in result, out var completion))
                {
                    return completion.Value;
                }

                problem = $"{result.Outcome}/{result.Reason}";
                return null;
            }
        }
    }

    private static VmArtifactDescriptor ArtifactDescriptor() => new(
        JavaScriptProfile.Id,
        Broiler.VM.Profile.JavaScript.Format.JsFormat.FormatVersion,
        JavaScriptProfile.WideManifest,
        default,
        VmCallerIdentity.FromCanonicalIdentity(Caller));

    private static VmRuntime NewRuntime()
    {
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension == VmBudgetDimension.LiveRuntimes
                ? VmCeilingSpec.AdoptParentRemaining(dimension)
                : VmCeilingSpec.AdoptProfileDefault(dimension));
        }

        var created = VmRuntime.Create(
            VmCatalog.CreateBuilder().Add(JavaScriptProfile.Descriptor).Build(),
            new VmRuntimeCreationOptions(
                aggregateBudget: null,
                ceilings: ceilings.ToImmutable(),
                maxSuspendedResidency: TimeSpan.FromMinutes(1),
                maxLiveSuspendedOperations: 1,
                guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
                externalSuspension: VmExternalSuspensionMode.Disabled,
                capabilities: ImmutableArray<VmCapabilityRegistration>.Empty));

        return created.TryGetRuntime(out var runtime)
            ? runtime
            : throw new InvalidOperationException($"the runtime refused creation: {created.Outcome}/{created.Reason}");
    }
}
