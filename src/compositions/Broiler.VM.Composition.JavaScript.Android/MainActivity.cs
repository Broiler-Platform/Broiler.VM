using Android.App;
using Android.OS;
using Broiler.VM;
using Broiler.VM.Composition.JavaScript.ExecutionOnly;
using Broiler.VM.Profile.JavaScript;

namespace Broiler.VM.Composition.JavaScript.Android;

/// <summary>
/// The Android head's driver: unpack the retained corpus, run the checks the desktop root runs,
/// and write one line a harness can read.
/// </summary>
/// <remarks>
/// <para>
/// <b>This is the only code in this root that is not the execution-only root's own.</b> The
/// checks themselves are that project's source, compiled here rather than re-implemented,
/// because the question this root exists to answer is whether <em>this component's code</em>
/// verifies and executes on an Android RID.
/// </para>
/// <para>
/// <b>What it runs, and what it deliberately does not.</b> The retained corpus replay - every
/// entry, twice, each re-hashed - and the ordering assertions. Not the soak, not the fuzz
/// sessions, not the aggregate-budget exercises: those are wall-clock and heap-shaped, an
/// emulator is neither a machine nor a stable one, and a plateau band read there would be a
/// figure attributable to nothing. A publish-and-run on this RID is a claim that the verifier
/// and the executor work on it, and that is what these two check sets are about.
/// </para>
/// <para>
/// <b>Why a log line rather than an exit code.</b> An Android application does not return one to
/// anything a hosted runner can read, so the harness reads logcat for the sentinel below. The
/// direction of the check matters: the harness fails when the sentinel is ABSENT, so a crash, a
/// failure to start and a failed check are all failures rather than a silent pass.
/// </para>
/// </remarks>
[Activity(Label = "Broiler.VM JavaScript", MainLauncher = true)]
public sealed class MainActivity : Activity
{
    /// <summary>The logcat tag the harness filters on.</summary>
    internal const string Tag = "broiler-js-android";

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        try
        {
            Run();
        }
        catch (Exception failure)
        {
            // An escaping exception is reported in the same channel as a failed check and with
            // the same sentinel shape, because a harness that only understood one of them would
            // read a crash as a run that never happened.
            global::Android.Util.Log.Error(
                Tag, $"{Tag}: unhandled {failure.GetType().Name}: {failure.Message}");
        }
    }

    private void Run()
    {
        var corpus = Unpack();
        var entries = CorpusReplay.ReadManifest(System.IO.Path.Combine(corpus, "corpus.manifest"));

        var checks = new List<(string Name, bool Passed, string Detail)>();

        var first = CorpusReplay.Replay(corpus, entries);
        var second = CorpusReplay.Replay(corpus, entries);

        var disagreements = entries
            .Where((entry, index) => !CorpusReplay.Agrees(entry, first[index]))
            .Select(entry => entry.Name)
            .ToArray();

        checks.Add(disagreements.Length == 0
            ? ("corpus-replay", true, $"{entries.Length} entries replayed to their recorded answers")
            : ("corpus-replay", false, string.Join(", ", disagreements)));

        var residue = entries
            .Where((entry, index) => first[index] != second[index])
            .Select(entry => entry.Name)
            .ToArray();

        checks.Add(residue.Length == 0
            ? ("corpus-replays-twice-with-no-residue", true, "both passes agreed row for row")
            : ("corpus-replays-twice-with-no-residue", false, string.Join(", ", residue)));

        checks.AddRange(OrderingChecks.Run(corpus, entries));

        var failed = 0;

        foreach (var (name, passed, detail) in checks)
        {
            if (!passed)
            {
                failed++;
                global::Android.Util.Log.Error(Tag, $"FAIL {name}: {detail}");
            }
            else
            {
                global::Android.Util.Log.Info(Tag, $"ok   {name}: {detail}");
            }
        }

        Report();

        global::Android.Util.Log.Info(
            Tag,
            failed == 0
                ? $"{Tag}: {checks.Count} checks passed, core contract version {VmCoreContract.Version}"
                : $"{Tag}: {failed} of {checks.Count} checks FAILED");
    }

    /// <summary>
    /// Prints the catalog table and the closure, in the format the desktop roots print them.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Byte-identical in shape to the execution-only root's <c>--closure</c> mode, because rules
    /// K3 and K4 read a checked-in baseline against what a published composition actually printed
    /// and neither knows which device printed it. A second format would need a second parser and
    /// would be a second thing to drift.
    /// </para>
    /// <para>
    /// <b>The closure is read off the loaded assemblies, filtered to this component's own.</b> The
    /// desktop roots read theirs off the published output directory, which an application package
    /// does not have in a form a process can enumerate; what both answer is the same question -
    /// which Broiler-owned assemblies are in the image - and the platform's own assemblies are
    /// framework in exactly the sense the desktop report already excludes.
    /// </para>
    /// </remarks>
    private static void Report()
    {
        var lines = new List<string>
        {
            $"# broiler-vm-composition core-contract-version={VmCoreContract.Version}",
            "composition Broiler.VM.Composition.JavaScript.Android",
            "label execution-only",
            "carries-lowering no",
            "profiles 1",
            string.Join(
                ' ',
                "profile",
                JavaScriptProfile.Id,
                JavaScriptProfile.Descriptor.PackageIdentity.PackageId,
                JavaScriptProfile.Descriptor.DescriptorRevision,
                JavaScriptProfile.Descriptor.HostCapabilityDescriptors.Length),
            string.Join(' ', "manifest", JavaScriptProfile.SliceManifest),
            string.Join(
                ' ',
                "format-versions",
                JavaScriptProfile.Descriptor.SupportedFormatVersions.Min,
                JavaScriptProfile.Descriptor.SupportedFormatVersions.Max),
        };

        foreach (var line in lines)
        {
            global::Android.Util.Log.Info(Tag, "catalog| " + line);
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetName().Name ?? string.Empty)
            .Where(name => name.StartsWith("Broiler.VM", StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        global::Android.Util.Log.Info(
            Tag,
            $"closure| # closure Broiler.VM.Composition.JavaScript.Android rid={Rid()}");
        global::Android.Util.Log.Info(Tag, "closure| ");
        global::Android.Util.Log.Info(Tag, $"closure| [mono] {assemblies.Length} non-framework assemblies");

        foreach (var name in assemblies)
        {
            global::Android.Util.Log.Info(Tag, "closure| " + name);
        }
    }

    /// <summary>The runtime identifier this process is actually running as.</summary>
    /// <remarks>
    /// Read from the device rather than from the build, because the two differ constantly: an
    /// emulator on an x64 host runs android-x64 whatever the publish declared, and a closure
    /// report naming the RID somebody intended would be the one field in it nobody could check.
    /// </remarks>
    private static string Rid() =>
        global::Android.OS.Build.SupportedAbis is { Count: > 0 } abis && abis[0] is "arm64-v8a"
            ? "android-arm64"
            : "android-x64";

    /// <summary>
    /// Writes the embedded corpus into the application's cache directory and answers where.
    /// </summary>
    /// <remarks>
    /// Byte for byte, and the replay re-hashes every entry it opens - so this method cannot
    /// quietly corrupt the evidence it is preparing. The resource names are the file names with
    /// one prefix, which is what the project file's <c>LogicalName</c> fixes.
    /// </remarks>
    private static string Unpack()
    {
        var assembly = typeof(MainActivity).Assembly;
        var directory = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(), "broiler-js-corpus");

        System.IO.Directory.CreateDirectory(directory);

        foreach (var resource in assembly.GetManifestResourceNames())
        {
            if (!resource.StartsWith("corpus.", StringComparison.Ordinal))
            {
                continue;
            }

            using var stream = assembly.GetManifestResourceStream(resource)
                ?? throw new InvalidOperationException($"embedded resource {resource} did not open");

            using var file = System.IO.File.Create(
                System.IO.Path.Combine(directory, resource["corpus.".Length..]));

            stream.CopyTo(file);
        }

        return directory;
    }
}
