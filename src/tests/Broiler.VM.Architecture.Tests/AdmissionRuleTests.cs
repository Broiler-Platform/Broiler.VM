using System.Reflection;
using System.Text.Json;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group I, the admission of shared JavaScript library packages (milestone SJL-2), and the
/// admission probe that exercises it end to end (SJL-2.9).
/// </summary>
public sealed class AdmissionRuleTests
{
    private static string WitnessText(string file) => File.ReadAllText(Path.Combine(WitnessAssemblies.Directory, file));

    // ---- I1 ---------------------------------------------------------------------------------

    [Fact]
    public void I1_Every_Package_Reference_Is_Admitted_From_A_Listed_Family_Project_At_Its_Pinned_Hash()
    {
        // The product graph: every project that ships or composes. Test projects reference their
        // test frameworks, and the rule is about what reaches an image.
        var shipping = ComponentGraph.Projects.Where(static project => !project.IsTestOnly).ToArray();
        Assert.NotEmpty(shipping);

        var violations = shipping.SelectMany(static project =>
        {
            var lockPath = Path.Combine(Path.GetDirectoryName(project.Path)!, "packages.lock.json");
            return AdmissionRules.I1(project, File.Exists(lockPath) ? AdmissionRules.ReadLockFile(File.ReadAllText(lockPath)) : null);
        });
        Assert.Empty(violations);

        // REVISED 2026-10-16, at SJL-7.4: a test project holds an admitted reference through a
        // test-only entry that names it. The architecture tests take the owner's generator so; its
        // reference is read with its lock file like a shipping project's, and the sweep is not
        // vacuous over it.
        var testProjects = ComponentGraph.Projects
            .Where(static project => project.IsTestOnly && project.PackageReferences.Any(AdmissionRules.IsBroilerPackage))
            .ToArray();
        Assert.Contains(testProjects, static project => project.AssemblyName == "Broiler.VM.Architecture.Tests");
        Assert.Empty(testProjects.SelectMany(static project =>
        {
            var lockPath = Path.Combine(Path.GetDirectoryName(project.Path)!, "packages.lock.json");
            return AdmissionRules.I1(project, File.Exists(lockPath) ? AdmissionRules.ReadLockFile(File.ReadAllText(lockPath)) : null);
        }));

        // The list itself: every entry agrees with its pin, its package is on the committed feed at
        // that hash, and every project it names is in the JavaScript family - or, for a test-only
        // entry, a test project.
        Assert.NotEmpty(AdmissionRules.AllowList);
        Assert.Empty(AdmissionRules.I1Entries(AdmissionRules.AllowList));
        Assert.Contains(AdmissionRules.AllowList, static package => package.IsTestOnly && package.Projects.Contains("Broiler.VM.Architecture.Tests"));

        // The revision's two witnesses: a product entry naming a test project, and a test-only
        // entry naming a project that ships. Each is otherwise a real entry, so its pin, hash and
        // feed copy agree and the refusal is the one the revision adds.
        var productEntry = AdmissionRules.I1Entries(AdmissionRules.LoadAllowList(Path.Combine(WitnessAssemblies.Directory, "I1-product-entry-names-a-test-project.json.witness"))).ToArray();
        Assert.Equal(["Broiler.Unicode.Algorithms: admitted for product use, and the allow-list names the test project Broiler.VM.Architecture.Tests; only a test-only entry may name one"], productEntry);
        var testOnlyEntry = AdmissionRules.I1Entries(AdmissionRules.LoadAllowList(Path.Combine(WitnessAssemblies.Directory, "I1-test-only-entry-names-a-shipping-project.json.witness"))).ToArray();
        Assert.Equal(["Broiler.Unicode.Generation: admitted test-only, and the allow-list names Broiler.VM.Profile.JavaScript.Format, which ships or composes"], testOnlyEntry);

        // The witnesses: an unlisted package, a listed one in the runtime, a listed one in Format.
        Assert.Contains(
            AdmissionRules.I1(ComponentGraph.Witness("I1-unlisted-package.csproj.witness"), null),
            static message => message.Contains("Newtonsoft.Json: the allow-list does not admit it", StringComparison.Ordinal));
        Assert.Contains(
            AdmissionRules.I1(ComponentGraph.Witness("I1-admitted-package-in-the-runtime.csproj.witness"), null),
            static message => message.Contains("outside the JavaScript family", StringComparison.Ordinal));
        Assert.Contains(
            AdmissionRules.I1(ComponentGraph.Witness("I1-admitted-package-in-the-format.csproj.witness"), null),
            static message => message.Contains("admits it for [Broiler.VM.Profile.JavaScript, Broiler.VM.Profile.JavaScript.Compiler, Broiler.VM.Profile.JavaScript.AdmissionProbe] only", StringComparison.Ordinal));

        // And a lock file whose content hash is not the pin, read with the probe's own project file.
        using var probe = AdmissionProbe.Create();
        Assert.Contains(
            AdmissionRules.I1(probe.Project, AdmissionRules.ReadLockFile(WitnessText("I1-lock-file-hash-differs.packages.lock.json.witness"))),
            static message => message.Contains("differs from the pin", StringComparison.Ordinal));
    }

    // ---- I2 ---------------------------------------------------------------------------------

    [Fact]
    public void I2_Every_Admitted_Assembly_Reaches_No_Dynamic_Code_And_No_Platform_Text_Api()
    {
        var assemblies = AdmissionRules.AllowList
            .SelectMany(static package => AdmissionRules.PackageAssemblies(package.FeedPath))
            .Select(static assembly => AssemblyFacts.ReadImage(assembly.Name, assembly.Image))
            .ToArray();

        // Not vacuous: the admitted assembly is read, and it references the framework.
        Assert.NotEmpty(assemblies);
        Assert.All(assemblies, static assembly => Assert.NotEmpty(assembly.TypeReferences));
        Assert.Empty(assemblies.SelectMany(AdmissionRules.I2));

        var witness = WitnessAssemblies.Read("I2-admitted-assembly-references-regular-expressions.cs.witness", "Broiler.ECMAScript.Algorithms");
        Assert.Contains(AdmissionRules.I2(witness), static message =>
            message.Contains("System.Text.RegularExpressions.Regex", StringComparison.Ordinal));
    }

    // ---- I3 ---------------------------------------------------------------------------------

    [Fact]
    public void I3_No_Compiler_Free_Root_Carries_The_Frontend_And_Only_Declared_Roots_Carry_Optional_Data()
    {
        var declaring = AdmissionRules.RootsDeclaringOptionalData();
        var roots = ComponentGraph.Projects.Where(static project => project.IsComposition).Select(static project => project.AssemblyName).ToArray();

        // Not vacuous over its second clause: two roots declare optional data and carry it.
        Assert.NotEmpty(declaring);
        Assert.Contains(roots, root => declaring.Contains(root) && AdmissionRules.GraphClosure(root).Contains("Broiler.VM.Profile.JavaScript.Intl"));
        Assert.All(AdmissionRules.CompilerFreeRoots, root => Assert.Contains(root, roots));

        Assert.Empty(roots.SelectMany(root => AdmissionRules.I3(root, AdmissionRules.GraphClosure(root), declaring)));

        // The witness, a closure listing: the execution-only root holding the frontend.
        var listing = WitnessText("I3-execution-only-closure-carries-the-frontend.txt.witness")
            .Split('\n')
            .Select(static line => line.Trim())
            .Where(static line => line.Length > 0 && !line.StartsWith('#'));
        Assert.Equal(
            ["Broiler.VM.Composition.JavaScript.ExecutionOnly is compiler-free and its closure holds Broiler.ECMAScript.Frontend"],
            AdmissionRules.I3("Broiler.VM.Composition.JavaScript.ExecutionOnly", listing, declaring));

        // Every image record a bundle retains (mstat-<root>.mstat, which the JavaScript collector
        // requests since SJL-2.7) is read the same way. None is retained before the first run of
        // that collector after this milestone, which the bundle says.
        var compositionsBySlug = roots
            .Where(static root => root.StartsWith("Broiler.VM.Composition.JavaScript.", StringComparison.Ordinal))
            .ToDictionary(static root => root["Broiler.VM.Composition.JavaScript.".Length..].ToLowerInvariant(), static root => root, StringComparer.Ordinal);
        foreach (var record in Directory.EnumerateFiles(Path.Combine(ComponentGraph.Root, "src"), "mstat-*.mstat", SearchOption.AllDirectories))
        {
            var slug = Path.GetFileNameWithoutExtension(record)["mstat-".Length..];
            if (compositionsBySlug.TryGetValue(slug, out var root))
            {
                var image = AssemblyFacts.ReadImage(slug, File.ReadAllBytes(record));
                Assert.Empty(AdmissionRules.I3(root, AdmissionRules.MstatClosure(image), declaring));
            }
        }

        // The Native AOT half: an image record naming the frontend, read as the root's mstat.
        var frontend = WitnessAssemblies.Compile(
            "Broiler.ECMAScript.Frontend", "namespace Broiler.ECMAScript.Frontend { public sealed class Parser { } }");
        var mstat = WitnessAssemblies.Read("I3-native-image-compiled-from-the-frontend.cs.witness", "Broiler.VM.Composition.JavaScript.ExecutionOnly.mstat", frontend);
        Assert.NotEmpty(AdmissionRules.I3("Broiler.VM.Composition.JavaScript.ExecutionOnly", AdmissionRules.MstatClosure(mstat), declaring));
    }

    // ---- I6 ---------------------------------------------------------------------------------

    private const string FamilyBaseline = "src/Broiler.VM.Profile.JavaScript/docs/api/public-api.txt";

    [Fact]
    public void I6_No_Admitted_Type_Appears_In_The_Family_Api_Baseline()
    {
        var admitted = AdmissionRules.AdmittedPublicTypes();
        Assert.Contains("Broiler.ECMAScript.Failure", admitted);

        var baseline = File.ReadAllLines(Path.Combine(ComponentGraph.Root, FamilyBaseline));
        Assert.True(baseline.Length > 100, "the family baseline was read");
        Assert.Empty(AdmissionRules.I6(FamilyBaseline, baseline, admitted));

        // The witness: a family public member returning an admitted type, compiled against the
        // admitted assembly and described through a resolver that knows it.
        var admittedPaths = AdmissionRules.AdmittedAssemblyPaths();
        var image = WitnessAssemblies.Compile(
            "Broiler.VM.Profile.JavaScript.Witness",
            WitnessText("I6-public-member-returns-an-admitted-type.cs.witness"),
            [.. admittedPaths.Select(File.ReadAllBytes)]);
        var described = Describe(image, admittedPaths);

        Assert.Contains(AdmissionRules.I6("witness", described, admitted), static message =>
            message.Contains("Broiler.ECMAScript.Failure", StringComparison.Ordinal));
    }

    /// <summary>Describes an image's public surface as the family baseline is described.</summary>
    private static IReadOnlyList<string> Describe(byte[] image, IEnumerable<string> extra)
    {
        var resolver = Directory.EnumerateFiles(Path.GetDirectoryName(typeof(object).Assembly.Location)!, "*.dll").Concat(extra);
        using var context = new MetadataLoadContext(new PathAssemblyResolver(resolver));
        return ApiSurface.Describe(context.LoadFromByteArray(image)).ToArray();
    }

    // ---- I7 ---------------------------------------------------------------------------------

    [Fact]
    public void I7_A_Bundle_Carrying_An_Admitted_Package_Records_Its_Upstream_Identity()
    {
        var bundles = Directory.EnumerateFiles(Path.Combine(ComponentGraph.Root, "docs"), "closure-*.txt", SearchOption.AllDirectories)
            .Concat(Directory.EnumerateFiles(Path.Combine(ComponentGraph.Root, "src"), "closure-*.txt", SearchOption.AllDirectories))
            .Where(static path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .GroupBy(static path => Path.GetDirectoryName(path)!, StringComparer.Ordinal)
            .ToArray();

        // The subject exists: retained bundles with closure listings are read.
        Assert.NotEmpty(bundles);

        var violations = new List<string>();
        foreach (var bundle in bundles)
        {
            // Keyed by listing, not by root: a bundle may retain more than one listing of a root
            // (a re-collection beside the first), and each is a closure the rule reads.
            var closures = bundle.ToDictionary(
                static path => (File.ReadLines(path).FirstOrDefault()?.Split(' ').ElementAtOrDefault(2) ?? "?") + " (" + Path.GetFileName(path) + ")",
                static path => (IReadOnlyList<string>)File.ReadLines(path).Select(static line => line.Trim()).Where(static line => line.Length > 0 && !line.StartsWith('#') && !line.StartsWith('[')).ToArray(),
                StringComparer.Ordinal);
            // The identity record is the manifest's admittedPackages field (ubc-bundle-manifest.py), or
            // the admitted-packages.json the composition collectors write beside their closures.
            var manifestPath = Path.Combine(bundle.Key, "manifest.json");
            var identityPath = Path.Combine(bundle.Key, "admitted-packages.json");
            var recordPath = File.Exists(identityPath) ? identityPath : manifestPath;
            using var manifest = JsonDocument.Parse(File.Exists(recordPath) ? File.ReadAllText(recordPath) : "{}");
            violations.AddRange(AdmissionRules.I7(Path.GetRelativePath(ComponentGraph.Root, recordPath), manifest.RootElement, closures));
        }

        Assert.Empty(violations);

        using var witness = JsonDocument.Parse(WitnessText("I7-manifest-records-no-commit.json.witness"));
        var witnessClosures = witness.RootElement.GetProperty("closures").EnumerateObject().ToDictionary(
            static p => p.Name,
            static p => (IReadOnlyList<string>)p.Value.EnumerateArray().Select(static e => e.GetString()!).ToArray(),
            StringComparer.Ordinal);
        Assert.Equal(
            ["witness: Broiler.VM.Composition.JavaScript.Cli carries Broiler.ECMAScript.Algorithms and the manifest records no commit"],
            AdmissionRules.I7("witness", witness.RootElement, witnessClosures));
    }

    // ---- The admission probe, SJL-2.9 ---------------------------------------------------------

    /// <summary>When set, the probe writes its transcript there, for an evidence bundle to retain.</summary>
    internal const string TranscriptVariable = "BROILER_ADMISSION_PROBE_LOG";

    [Fact]
    public void The_Admission_Probe_Is_Admitted_And_Every_Negative_Control_Is_Refused()
    {
        using var probe = AdmissionProbe.Create();
        probe.Note("# The admission probe of milestone SJL-2.9: " + probe.Package.Id + " " + probe.Package.Version + ", admitted " + probe.Package.Use + ".");
        probe.Note(string.Empty);

        // Positive: restore through the repository's own source mapping, then again in locked mode,
        // then build; I1, I2 and I6 hold over what restored.
        Assert.Equal(0, probe.Restore(locked: false));
        Assert.Equal(0, probe.Restore(locked: true));
        Assert.Equal(0, probe.Build());

        var lockFile = probe.LockFile;
        Assert.NotNull(lockFile);
        Assert.Equal(probe.Package.ContentHash, Assert.Single(lockFile, entry => entry.Id == probe.Package.Id).ContentHash);

        var i1 = AdmissionRules.I1(probe.Project, lockFile).ToArray();
        var restored = AssemblyFacts.ReadImage("Broiler.ECMAScript.Algorithms", File.ReadAllBytes(probe.RestoredAssembly("Broiler.ECMAScript.Algorithms")));
        var i2 = AdmissionRules.I2(restored).ToArray();
        var surface = Describe(File.ReadAllBytes(probe.BuiltAssembly), [probe.RestoredAssembly("Broiler.ECMAScript.Algorithms")]);
        var i6 = AdmissionRules.I6("probe", surface, AdmissionRules.AdmittedPublicTypes()).ToArray();
        Record(probe, "positive: I1", i1);
        Record(probe, "positive: I2 over the restored assembly", i2);
        Record(probe, "positive: I6 over the probe's public surface (" + surface.Count.ToString(System.Globalization.CultureInfo.InvariantCulture) + " lines)", i6);
        Assert.Empty(i1);
        Assert.Empty(i2);
        Assert.NotEmpty(surface);
        Assert.Empty(i6);

        // Negative 1: the probe referenced from a core project.
        var core = AdmissionRules.I1(ComponentGraph.Witness("I1-admitted-package-in-the-runtime.csproj.witness"), lockFile).ToArray();
        Record(probe, "negative: the probe package referenced from Broiler.VM.Runtime, I1", core);
        Assert.NotEmpty(core);

        // Negative 2: the probe with a changed hash - the lock file edited, and locked mode refusing it too.
        var original = File.ReadAllText(probe.LockFilePath);
        File.WriteAllText(probe.LockFilePath, original.Replace(probe.Package.ContentHash, "AA" + probe.Package.ContentHash[2..], StringComparison.Ordinal));
        var changed = AdmissionRules.I1(probe.Project, probe.LockFile).ToArray();
        Record(probe, "negative: the lock file's content hash changed, I1", changed);
        Assert.NotEmpty(changed);
        Assert.NotEqual(0, probe.Restore(locked: true));
        File.WriteAllText(probe.LockFilePath, original);

        // Negative 3: a probe build that references System.Text.RegularExpressions.
        var regex = AdmissionRules.I2(WitnessAssemblies.Read("I2-admitted-assembly-references-regular-expressions.cs.witness", "Broiler.ECMAScript.Algorithms")).ToArray();
        Record(probe, "negative: a probe build referencing System.Text.RegularExpressions, I2", regex);
        Assert.NotEmpty(regex);

        if (Environment.GetEnvironmentVariable(TranscriptVariable) is { Length: > 0 } target)
        {
            File.WriteAllText(target, probe.Transcript);
        }
    }

    private static void Record(AdmissionProbe probe, string what, IReadOnlyCollection<string> violations)
    {
        probe.Note("## " + what + ": " + (violations.Count == 0 ? "no violation" : violations.Count.ToString(System.Globalization.CultureInfo.InvariantCulture) + " violation(s)"));
        foreach (var violation in violations)
        {
            probe.Note("  " + violation);
        }

        probe.Note(string.Empty);
    }
}
