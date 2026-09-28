using Broiler.VM;
using Broiler.VM.Profile.WebAssembly;
using System.Globalization;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// The translator's determinism: every module this root knows, translated twice and compared byte for
/// byte.
/// </summary>
/// <remarks>
/// <para>
/// <b>Two translations of one module under one set of ceilings must answer the same bytes</b>, or the
/// artifact a host verified and the artifact it would verify on the next run are two programs with one
/// name. The modules are every one this root holds: the canonical module, every entry of the corpus
/// store - the source of truth the retained corpus was written from, so no directory is needed - and
/// every module the execution and differential lanes built on this run. A module the translator
/// refuses is held to the same rule: both refusals must agree in every field the core would report.
/// </para>
/// <para>
/// <b>It runs only under <c>--determinism</c></b>, after the three lanes population B is made of, and
/// its header opens with none of their names.
/// </para>
/// </remarks>
internal static class Determinism
{
    /// <summary>Translates every module twice, prints what each pair did, and answers the failures.</summary>
    internal static int Report(
        VmRuntime runtime, IReadOnlyList<(string Lane, string Label, byte[] Module)> built, bool verbose)
    {
        var modules = new List<(string Name, byte[] Module)>
        {
            ("canonical: module", CorpusStore.CanonicalModule()),
        };

        var corpus = CorpusStore.Entries();

        foreach (var entry in corpus)
        {
            modules.Add(($"corpus: {entry.Name}", entry.Bytes));
        }

        foreach (var (lane, label, module) in built)
        {
            modules.Add(($"{lane}: {label}", module));
        }

        // The ceilings the lanes translated under: the runtime's, the profile's hard maxima, no request.
        var descriptor = ModuleVerification.ArtifactDescriptor(
            WebAssemblyProfile.SliceManifest, "composition-wasm-harness://determinism");
        var ceilings = ModuleVerification.Ceilings(runtime, in descriptor);

        Console.WriteLine(
            $"# determinism: {modules.Count} modules translated twice - the canonical module, " +
            $"{corpus.Count} corpus entries and {built.Count} modules the execution and differential " +
            "lanes built");

        var failed = 0;
        var artifacts = 0;
        var artifactBytes = 0L;

        foreach (var (name, module) in modules)
        {
            var first = WasmTranslator.Translate(module, ceilings, CancellationToken.None);
            var second = WasmTranslator.Translate(module, ceilings, CancellationToken.None);
            string? complaint = null;

            if (first.Succeeded != second.Succeeded || !string.Equals(Answer(first), Answer(second), StringComparison.Ordinal))
            {
                complaint = $"answered {Answer(first)} and then {Answer(second)}";
            }
            else if (first.Succeeded && !first.Artifact.AsSpan().SequenceEqual(second.Artifact.AsSpan()))
            {
                complaint =
                    $"translated to {first.Artifact.Length.ToString(CultureInfo.InvariantCulture)} and then " +
                    $"{second.Artifact.Length.ToString(CultureInfo.InvariantCulture)} bytes that differ at " +
                    $"offset {FirstDifference(first, second).ToString(CultureInfo.InvariantCulture)}";
            }

            if (complaint is not null)
            {
                failed++;
                Console.WriteLine($"FAIL {name}: {module.Length.ToString(CultureInfo.InvariantCulture)} bytes {complaint}");
                continue;
            }

            if (first.Succeeded)
            {
                artifacts++;
                artifactBytes += first.Artifact.Length;
            }

            if (verbose)
            {
                Console.WriteLine(
                    $"ok   {name}: {module.Length.ToString(CultureInfo.InvariantCulture)} bytes " +
                    (first.Succeeded
                        ? $"translated twice to the same {first.Artifact.Length.ToString(CultureInfo.InvariantCulture)} bytes"
                        : $"refused twice alike, {Answer(first)}"));
            }
        }

        Console.WriteLine(
            $"# determinism: {(modules.Count - failed).ToString(CultureInfo.InvariantCulture)} of " +
            $"{modules.Count.ToString(CultureInfo.InvariantCulture)} modules answered alike twice, " +
            $"{artifacts.ToString(CultureInfo.InvariantCulture)} artifacts of " +
            $"{artifactBytes.ToString(CultureInfo.InvariantCulture)} bytes compared byte for byte");

        return failed;
    }

    /// <summary>Every field a translation answers in; the artifact itself is compared apart.</summary>
    private static string Answer(WasmTranslation translation) =>
        translation.Succeeded
            ? $"{translation.Outcome}/{translation.Reason}"
            : $"{translation.Outcome}/{translation.Reason}/{((int)translation.Code).ToString(CultureInfo.InvariantCulture)}/" +
              (translation.Outcome is VmOutcome.ResourceExhaustion
                  ? $"{translation.Dimension}/{translation.Scope}"
                  : "-/-") +
              $" at ({translation.Position.SectionIndex.ToString(CultureInfo.InvariantCulture)}," +
              $"{translation.Position.ByteOffset.ToString(CultureInfo.InvariantCulture)}," +
              $"{translation.Position.ProfileCoordinate0.ToString(CultureInfo.InvariantCulture)}," +
              $"{translation.Position.ProfileCoordinate1.ToString(CultureInfo.InvariantCulture)})";

    /// <summary>The first offset at which two artifacts differ, or the shorter one's length.</summary>
    private static int FirstDifference(WasmTranslation first, WasmTranslation second)
    {
        var length = System.Math.Min(first.Artifact.Length, second.Artifact.Length);

        for (var index = 0; index < length; index++)
        {
            if (first.Artifact[index] != second.Artifact[index])
            {
                return index;
            }
        }

        return length;
    }
}
