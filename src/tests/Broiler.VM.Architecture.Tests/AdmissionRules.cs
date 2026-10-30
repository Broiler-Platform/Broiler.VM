using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;

namespace Broiler.VM.Architecture.Tests;

/// <summary>One entry of rule I1's allow-list, admitted-packages.json.</summary>
internal sealed record AdmittedPackage(
    string Id,
    string Version,
    string ContentHash,
    string Use,
    IReadOnlyList<string> Projects,
    IReadOnlyList<string> Sources,
    string Pin,
    string? ArchiveHash = null)
{
    internal bool IsTestOnly => string.Equals(Use, "test-only", StringComparison.Ordinal);

    /// <summary>The committed feed's copy of the package, beside its pin.</summary>
    internal string FeedPath => Path.Combine(
        Path.GetDirectoryName(Path.Combine(ComponentGraph.Root, Pin))!, Id + "." + Version + ".nupkg");
}

/// <summary>A lock file's resolution of one package: what restored and the content hash it recorded.</summary>
internal sealed record RestoredPackage(string Id, string Resolved, string ContentHash);

/// <summary>
/// Group I: the admission of shared JavaScript library packages (ADR 0014; the programme's
/// governance, section 3.1). Each rule is a function from what it reads to the violations it
/// finds, as every other group's is.
/// </summary>
/// <remarks>
/// <para>
/// <b>Before SJL-2 nothing reached this component by package at all.</b> Rule A2 refused every
/// Broiler package, N1 refused every package in the profile, and the product graph declared none.
/// The shared libraries are the first packages a product project will ever reference, so the
/// admission is spelled out per package and per project, and the hash that restored is part of
/// the tree: a project that references an admitted package uses NuGet assets metadata, and the lock
/// metadata content hash is held to the list.
/// </para>
/// </remarks>
internal static class AdmissionRules
{
    /// <summary>The allow-list, read once.</summary>
    internal static IReadOnlyList<AdmittedPackage> AllowList { get; } = LoadAllowList(
        Path.Combine(ComponentGraph.Root, "src", "tests", "Broiler.VM.Architecture.Tests", "admitted-packages.json"));

    /// <summary>
    /// The JavaScript family, the only projects that may hold an admitted reference: the profile
    /// and its siblings. Composition roots declare no package reference (rule A12) and receive a
    /// shared package transitively, through the family project that references it.
    /// </summary>
    internal static bool IsJavaScriptFamily(string assemblyName) =>
        assemblyName.StartsWith("Broiler.VM.Profile.JavaScript", StringComparison.Ordinal);

    /// <summary>
    /// Whether a package identifier names a Broiler package, however it is spelled: <c>Broiler.*</c>,
    /// and also <c>BroilerJS.*</c> and any other identifier that opens with "Broiler", which the
    /// earlier <c>StartsWith("Broiler.")</c> test let through.
    /// </summary>
    internal static bool IsBroilerPackage(string id) =>
        id.StartsWith("Broiler", StringComparison.OrdinalIgnoreCase);

    internal static AdmittedPackage? Admitted(string id) =>
        AllowList.SingleOrDefault(package => string.Equals(package.Id, id, StringComparison.OrdinalIgnoreCase));

    internal static IReadOnlyList<AdmittedPackage> LoadAllowList(string path)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        return document.RootElement.GetProperty("packages").EnumerateArray()
            .Select(static entry => new AdmittedPackage(
                entry.GetProperty("id").GetString()!,
                entry.GetProperty("version").GetString()!,
                entry.GetProperty("contentHash").GetString()!,
                entry.GetProperty("use").GetString()!,
                entry.GetProperty("projects").EnumerateArray().Select(static p => p.GetString()!).ToArray(),
                entry.GetProperty("sources").EnumerateArray().Select(static s => s.GetString()!).ToArray(),
                entry.GetProperty("pin").GetString()!,
                entry.TryGetProperty("archiveHash", out var archiveHash) ? archiveHash.GetString() : null))
            .ToArray();
    }

    /// <summary>The packages a lock file resolved for net10.0, direct and transitive.</summary>
    internal static IReadOnlyList<RestoredPackage> ReadLockFile(string text)
    {
        using var document = JsonDocument.Parse(text);
        return document.RootElement.GetProperty("dependencies").EnumerateObject()
            .SelectMany(static framework => framework.Value.EnumerateObject())
            .Where(static package => package.Value.TryGetProperty("contentHash", out _))
            .Select(static package => new RestoredPackage(
                package.Name,
                package.Value.GetProperty("resolved").GetString()!,
                package.Value.GetProperty("contentHash").GetString()!))
            .ToArray();
    }

    /// <summary>Read the actual package versions/hashes from ordinary NuGet restore output.</summary>
    internal static IReadOnlyList<RestoredPackage> ReadRestoreAssets(string text)
    {
        using var document = JsonDocument.Parse(text);
        return document.RootElement.GetProperty("libraries").EnumerateObject()
            .Where(static library => library.Value.GetProperty("type").GetString() == "package")
            .Select(static library => new RestoredPackage(
                library.Name[..library.Name.LastIndexOf('/')],
                library.Name[(library.Name.LastIndexOf('/') + 1)..],
                library.Value.GetProperty("sha512").GetString()!))
            .ToArray();
    }

    /// <summary>
    /// I1: every PackageReference is on the allow-list, from a project the list names for it, in
    /// the JavaScript family, restored with NuGet assets metadata whose content hash equals the list's pin.
    /// A test-only package is admitted for no product project.
    /// </summary>
    /// <remarks>
    /// <b>Revised 2026-10-16, at SJL-7.4</b>, when the owner's table generator reached the
    /// architecture tests as a pinned, test-only package (JSD-0074, decision SJL-D-6 (c)): a test
    /// project may hold an admitted reference through a test-only entry that names it, restored with
    /// restore metadata at the pin like any other, and never through a product entry. A test project's
    /// test frameworks are not Broiler packages and are not this rule's subject; rule A2 refuses a
    /// Broiler package outside the list in every project. Until then the rule read "every
    /// PackageReference ... from a project the list names for it, in the JavaScript family", and a
    /// test project outside the family was refused whatever its entry said.
    /// </remarks>
    /// <param name="project">The project file.</param>
    /// <param name="lockFile">Its resolved package metadata, or null before restore.</param>
    internal static IEnumerable<string> I1(ComponentGraph.ProjectFile project, IReadOnlyList<RestoredPackage>? lockFile)
    {
        foreach (var reference in project.PackageReferences)
        {
            if (project.IsTestOnly && !IsBroilerPackage(reference))
            {
                continue;
            }

            var admitted = Admitted(reference);
            var where = $"{project.RelativePath} ({project.AssemblyName}) -> package {reference}";

            if (admitted is null)
            {
                yield return $"{where}: the allow-list does not admit it";
                continue;
            }

            if (project.IsTestOnly)
            {
                if (!admitted.IsTestOnly)
                {
                    yield return $"{where}: admitted for product use, and a test project holds an admitted reference only through a test-only entry";
                    continue;
                }
            }
            else if (!IsJavaScriptFamily(project.AssemblyName))
            {
                yield return $"{where}: an admitted reference outside the JavaScript family";
                continue;
            }

            if (!admitted.Projects.Contains(project.AssemblyName, StringComparer.Ordinal))
            {
                yield return $"{where}: the allow-list admits it for [{string.Join(", ", admitted.Projects)}] only";
                continue;
            }

            if (admitted.IsTestOnly && (project.IsProduct || project.IsComposition))
            {
                yield return $"{where}: admitted test-only, and this is a shipping project";
                continue;
            }

            var locked = lockFile?.SingleOrDefault(package => string.Equals(package.Id, admitted.Id, StringComparison.OrdinalIgnoreCase));

            if (locked is null)
            {
                yield return $"{where}: no restore metadata records what restored";
            }
            else if (!string.Equals(locked.Resolved, admitted.Version, StringComparison.Ordinal))
            {
                yield return $"{where}: restore resolved {locked.Resolved}, the allow-list pins {admitted.Version}";
            }
            else if (!string.Equals(locked.ContentHash, admitted.ContentHash, StringComparison.Ordinal))
            {
                yield return $"{where}: the restored content hash {locked.ContentHash} differs from the pin {admitted.ContentHash}";
            }
        }
    }

    /// <summary>
    /// I1's other half: every allow-list entry agrees with its pin, its package is on the committed
    /// feed hashing to the pin when that is a source, and every project it names is in the
    /// JavaScript family - or, for a test-only entry, a test project, and never a shipping one.
    /// </summary>
    /// <remarks>
    /// <b>Revised 2026-10-16, at SJL-7.4</b>, beside <see cref="I1"/>: a test-only entry may name a
    /// test project of the graph, which is what admits the owner's generator to the architecture
    /// tests. A product entry naming a test project, and a test-only entry naming a project that ships
    /// or composes, are each refused. Until then every named project had to be in the JavaScript
    /// family, whatever the entry's use.
    /// </remarks>
    internal static IEnumerable<string> I1Entries(IReadOnlyList<AdmittedPackage> list)
    {
        foreach (var package in list)
        {
            foreach (var project in package.Projects)
            {
                var graph = ComponentGraph.Projects.FirstOrDefault(p => string.Equals(p.AssemblyName, project, StringComparison.Ordinal));
                var isTestProject = graph is { IsTestOnly: true };

                if (package.IsTestOnly && graph is { IsTestOnly: false })
                {
                    yield return $"{package.Id}: admitted test-only, and the allow-list names {project}, which ships or composes";
                }
                else if (!package.IsTestOnly && isTestProject)
                {
                    yield return $"{package.Id}: admitted for product use, and the allow-list names the test project {project}; only a test-only entry may name one";
                }
                else if (!isTestProject && !IsJavaScriptFamily(project))
                {
                    yield return $"{package.Id}: the allow-list names {project}, outside the JavaScript family";
                }
            }

            var pinPath = Path.Combine(ComponentGraph.Root, package.Pin);

            if (!File.Exists(pinPath))
            {
                yield return $"{package.Id}: its pin {package.Pin} does not exist";
                continue;
            }

            var pin = File.ReadLines(pinPath)
                .Where(static line => line.Length > 0 && !line.StartsWith('#'))
                .Select(static line => line.Split(' ', 2))
                .ToDictionary(static parts => parts[0], static parts => parts[1], StringComparer.Ordinal);

            if (pin.GetValueOrDefault("id") != package.Id || pin.GetValueOrDefault("version") != package.Version
                || pin.GetValueOrDefault("content-hash") != package.ContentHash
                || pin.GetValueOrDefault("archive-hash") != package.ArchiveHash)
            {
                yield return $"{package.Id}: the allow-list and {package.Pin} disagree on identity or content hash";
            }

            if (package.Sources.Contains("committed-feed", StringComparer.Ordinal))
            {
                if (!File.Exists(package.FeedPath))
                {
                    yield return $"{package.Id}: {package.Version} is not on the committed feed";
                }
                else if (ContentHash(File.ReadAllBytes(package.FeedPath)) != (package.ArchiveHash ?? package.ContentHash))
                {
                    yield return $"{package.Id}: the committed feed's copy does not hash to the pin";
                }
            }
        }
    }

    /// <summary>NuGet's content hash: the base64 SHA-512 of the .nupkg's bytes, which a lock file records.</summary>
    internal static string ContentHash(byte[] package) => Convert.ToBase64String(SHA512.HashData(package));

    /// <summary>The assemblies a package carries for net10.0, as (file name, bytes).</summary>
    internal static IReadOnlyList<(string Name, byte[] Image)> PackageAssemblies(string nupkg)
    {
        using var archive = ZipFile.OpenRead(nupkg);
        return archive.Entries
            .Where(static entry => entry.FullName.StartsWith("lib/net10.0/", StringComparison.Ordinal)
                && entry.FullName.EndsWith(".dll", StringComparison.Ordinal))
            .Select(static entry =>
            {
                using var stream = entry.Open();
                using var copy = new MemoryStream();
                stream.CopyTo(copy);
                return (Path.GetFileNameWithoutExtension(entry.Name), copy.ToArray());
            })
            .ToArray();
    }

    /// <summary>The source commit a package's nuspec records in its repository element, or null.</summary>
    internal static string? NuspecCommit(string nupkg)
    {
        using var archive = ZipFile.OpenRead(nupkg);
        var nuspec = archive.Entries.Single(static entry => entry.FullName.EndsWith(".nuspec", StringComparison.Ordinal));
        using var stream = nuspec.Open();
        var document = System.Xml.Linq.XDocument.Load(stream);
        return document.Descendants().FirstOrDefault(static e => e.Name.LocalName == "repository")?.Attribute("commit")?.Value;
    }

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// Members whose answer comes from the platform's culture, time zone, Unicode data or regular
    /// expressions: the metadata equivalents of rules N18 and N23, which read this component's
    /// sources and so can never read a package's.
    /// </summary>
    private static readonly string[] PlatformMembers =
    [
        "System.String.Normalize", "System.String.IsNormalized",
        "System.String.ToUpper", "System.String.ToLower", "System.String.ToUpperInvariant", "System.String.ToLowerInvariant",
        "System.Char.ToUpper", "System.Char.ToLower", "System.Char.ToUpperInvariant", "System.Char.ToLowerInvariant",
        "System.String.Compare", "System.String.CompareTo",
    ];

    private static readonly string[] PlatformTypePrefixes =
    [
        "System.Text.RegularExpressions.", "System.TimeZoneInfo", "System.Globalization.CultureInfo",
        "System.Globalization.TextInfo", "System.Globalization.CompareInfo", "System.Globalization.StringNormalizationExtensions",
    ];

    /// <summary>
    /// I2: an admitted assembly reaches nothing on the member lists of B5, B5b and B5c, and no
    /// platform globalization or regular-expression API.
    /// </summary>
    /// <remarks>
    /// B5, B5b and B5c read only this solution's own projects, so a package was outside every one
    /// of them; this rule applies the same lists to every admitted assembly, read from the package
    /// itself. ILC, publishing with warnings as errors, already fails on a trim or AOT warning in
    /// package code a root reaches; I2 also covers the code no root reaches yet.
    /// </remarks>
    internal static IEnumerable<string> I2(AssemblyFacts assembly)
    {
        foreach (var violation in ArchitectureRules.B5(assembly).Concat(ArchitectureRules.B5b(assembly)).Concat(NativeMappingRules.B5c(assembly)))
        {
            yield return violation;
        }

        foreach (var member in assembly.MemberReferences.Where(static member => PlatformMembers.Contains(member, StringComparer.Ordinal)))
        {
            yield return $"{assembly.Name} calls {member}";
        }

        foreach (var type in assembly.TypeReferences.Where(static type => PlatformTypePrefixes.Any(prefix =>
                     type.StartsWith(prefix, StringComparison.Ordinal))))
        {
            yield return $"{assembly.Name} names {type}";
        }
    }

    // ---------------------------------------------------------------------------------------------

    /// <summary>The shared frontend's assemblies: the compile-time-only library no compiler-free root may carry.</summary>
    internal static readonly string[] FrontendAssemblies = ["Broiler.ECMAScript.Frontend"];

    /// <summary>The roots that are compiler-free by definition: an execution-only deployment and the mobile head.</summary>
    internal static readonly string[] CompilerFreeRoots =
        ["Broiler.VM.Composition.JavaScript.ExecutionOnly", "Broiler.VM.Composition.JavaScript.Android"];

    /// <summary>
    /// Optional data: the assemblies a root carries only when it has an Intl or zoned-Temporal surface.
    /// Since SJL-7.4 the Unicode area's optional segmentation package is one (JSD-0074's X5): only the
    /// Intl data assembly references it.
    /// </summary>
    internal static readonly string[] OptionalDataAssemblies = ["Broiler.VM.Profile.JavaScript.Intl", "Broiler.Unicode.Segmentation"];

    /// <summary>
    /// I3: a compiler-free root's closure holds no frontend assembly, and a root carries optional data
    /// only when docs/compositions.md declares it in that root's closure, which is where an Intl or
    /// zoned-Temporal surface is declared.
    /// </summary>
    /// <param name="root">The composition root's assembly name.</param>
    /// <param name="closure">The assemblies its closure holds: from the project graph, or from an mstat.</param>
    /// <param name="rootsDeclaringData">The roots whose compositions.md row lists optional data.</param>
    internal static IEnumerable<string> I3(string root, IEnumerable<string> closure, IReadOnlySet<string> rootsDeclaringData)
    {
        var held = closure.ToHashSet(StringComparer.Ordinal);

        if (CompilerFreeRoots.Contains(root, StringComparer.Ordinal))
        {
            foreach (var frontend in FrontendAssemblies.Where(held.Contains))
            {
                yield return $"{root} is compiler-free and its closure holds {frontend}";
            }
        }

        if (!rootsDeclaringData.Contains(root))
        {
            foreach (var data in OptionalDataAssemblies.Where(held.Contains))
            {
                yield return $"{root} declares no Intl or zoned-Temporal surface and its closure holds {data}";
            }
        }
    }

    /// <summary>
    /// A root's closure from the project graph: every project it reaches by ProjectReference, and
    /// every package any of them references - the shape a published directory's managed assemblies
    /// take, which is what a JIT-mode closure listing holds.
    /// </summary>
    internal static IReadOnlyList<string> GraphClosure(string rootAssembly)
    {
        var byPath = ComponentGraph.Projects.ToDictionary(static p => Path.GetFullPath(p.Path), StringComparer.OrdinalIgnoreCase);
        var start = ComponentGraph.Projects.Single(p => string.Equals(p.AssemblyName, rootAssembly, StringComparison.Ordinal));
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<ComponentGraph.ProjectFile>([start]);

        while (queue.Count > 0)
        {
            var project = queue.Dequeue();
            if (!seen.Add(project.AssemblyName))
            {
                continue;
            }

            foreach (var package in project.PackageReferences)
            {
                seen.Add(package);
            }

            foreach (var reference in project.ProjectReferences)
            {
                if (byPath.TryGetValue(Path.GetFullPath(reference), out var next))
                {
                    queue.Enqueue(next);
                }
            }
        }

        return seen.Order(StringComparer.Ordinal).ToArray();
    }

    /// <summary>
    /// A Native AOT image's closure, read from ILC's mstat file. The publish directory of a native
    /// image holds no managed assembly, so the collector asks ILC for its record of the image
    /// (<c>IlcGenerateMstatFile</c>), and that record's AssemblyRef table names every assembly the
    /// image was compiled from.
    /// </summary>
    internal static IReadOnlyList<string> MstatClosure(AssemblyFacts mstat) => mstat.AssemblyReferences
        .Where(static name => !name.StartsWith("System.", StringComparison.Ordinal))
        .ToArray();

    /// <summary>The roots whose row in docs/compositions.md lists an optional-data assembly in its closure.</summary>
    internal static IReadOnlySet<string> RootsDeclaringOptionalData()
    {
        var compositions = File.ReadAllLines(Path.Combine(ComponentGraph.Root, "docs", "compositions.md"));
        return compositions
            .Where(static line => line.StartsWith("| `Broiler.VM.Composition.", StringComparison.Ordinal))
            .Where(static line => OptionalDataAssemblies.Any(data => line.Contains("`" + data + "`", StringComparison.Ordinal)))
            .Select(static line => line.Split('`')[1])
            .ToHashSet(StringComparer.Ordinal);
    }

    // ---------------------------------------------------------------------------------------------

    /// <summary>The names under which an admitted library declares the Unicode data version it reads.</summary>
    internal static readonly string[] UnicodeVersionFields = ["UnicodeVersion", "PinnedVersion"];

    /// <summary>
    /// The types whose <c>PinnedVersion</c> is not a Unicode version but the time data home's pin readers'
    /// (JSD-0076): a tzdb release and a CLDR release, which I4's time clauses read instead.
    /// </summary>
    /// <remarks>
    /// Added 2026-10-17, at SJL-8.5, when <c>Broiler.ECMAScript.Generation</c> was admitted: the Unicode
    /// clause read a <c>PinnedVersion</c> of any type as a Unicode version, and the home's
    /// <c>TzdbPin.PinnedVersion</c> is <c>2026e</c>.
    /// </remarks>
    internal static readonly string[] TimePinTypes =
    [
        "Broiler.ECMAScript.Generation.Time.TzdbPin",
        "Broiler.ECMAScript.Generation.Time.CldrPin",
    ];

    /// <summary>
    /// I4's pin clause: the pin this repository's embedded tables are generated from equals its home
    /// repository's, file by file, by length and SHA-256, and by version (SJL-D-6 (c): the owner holds
    /// the pin, and each engine generates from its own). The comparison is the owner's own
    /// <c>UnicodePin.Differences</c>, which keys a UCD file by its pinned name and an ECMAScript
    /// property table by its file name, wherever each repository keeps it.
    /// </summary>
    /// <param name="familyPin">This repository's pin, as the owner's generator parses it.</param>
    /// <param name="homePin">The home repository's pin, as its generator package carries it.</param>
    internal static IEnumerable<string> I4Pins(Broiler.Unicode.Generation.UnicodePin familyPin, Broiler.Unicode.Generation.UnicodePin homePin) =>
        familyPin.Differences(homePin).Select(static difference =>
            $"src/tests/unicode/pins/unicode.pin and Broiler.Unicode's pin disagree: {difference} (rule I4: a pin moves only by a dated decision, in both repositories at once)");

    /// <summary>
    /// I4's version clause: every Unicode data version an admitted assembly declares - a literal
    /// string field named as <see cref="UnicodeVersionFields"/> lists, public or not, but a
    /// <c>PinnedVersion</c> of one of the <see cref="TimePinTypes"/> - equals the pin's version.
    /// </summary>
    /// <param name="pinVersion">The version the pin names.</param>
    /// <param name="declared">Each declaration: its assembly, its field and its value.</param>
    internal static IEnumerable<string> I4Versions(string pinVersion, IEnumerable<(string Assembly, string Field, string Value)> declared) =>
        declared
            .Where(declaration => !string.Equals(declaration.Value, pinVersion, StringComparison.Ordinal))
            .Select(declaration =>
                $"{declaration.Assembly} declares {declaration.Field} = \"{declaration.Value}\", and the pin names {pinVersion} (rule I4)");

    /// <summary>
    /// Every literal string field of an assembly image whose name <see cref="UnicodeVersionFields"/>
    /// lists, read from its metadata tables without loading it, but a <c>PinnedVersion</c> of one of the
    /// <see cref="TimePinTypes"/>.
    /// </summary>
    internal static IReadOnlyList<(string Assembly, string Field, string Value)> DeclaredUnicodeVersions(string assemblyName, byte[] image) =>
        DeclaredStrings(assemblyName, image, static (type, name) =>
            UnicodeVersionFields.Contains(name, StringComparer.Ordinal) &&
            !(name == "PinnedVersion" && (TimePinTypes.Contains(type, StringComparer.Ordinal)
                || type == "Broiler.ECMAScript.Generation.Intl.IntlCldrPin")));

    /// <summary>
    /// Every literal string field of an assembly image that <paramref name="select"/> chooses by its
    /// declaring type's full name and its own name, read from its metadata tables without loading it.
    /// </summary>
    internal static IReadOnlyList<(string Assembly, string Field, string Value)> DeclaredStrings(
        string assemblyName, byte[] image, Func<string, string, bool> select)
    {
        using var stream = new MemoryStream(image, writable: false);
        using var pe = new System.Reflection.PortableExecutable.PEReader(stream);
        var reader = System.Reflection.Metadata.PEReaderExtensions.GetMetadataReader(pe);
        var found = new List<(string, string, string)>();

        foreach (var typeHandle in reader.TypeDefinitions)
        {
            var type = reader.GetTypeDefinition(typeHandle);
            var typeName = reader.GetString(type.Namespace) + "." + reader.GetString(type.Name);

            foreach (var fieldHandle in type.GetFields())
            {
                var field = reader.GetFieldDefinition(fieldHandle);
                var name = reader.GetString(field.Name);

                if (!select(typeName, name) || field.GetDefaultValue().IsNil)
                {
                    continue;
                }

                var constant = reader.GetConstant(field.GetDefaultValue());

                if (constant.TypeCode == System.Reflection.Metadata.ConstantTypeCode.String)
                {
                    var value = reader.GetBlobReader(constant.Value).ReadUTF16(reader.GetBlobReader(constant.Value).Length);
                    found.Add((assemblyName, typeName + "." + name, value));
                }
            }
        }

        return found;
    }

    /// <summary>Every Unicode version every admitted assembly declares, read from the committed feed's packages.</summary>
    internal static IReadOnlyList<(string Assembly, string Field, string Value)> AdmittedUnicodeVersions() => AllowList
        .Where(static package => File.Exists(package.FeedPath))
        .SelectMany(static package => PackageAssemblies(package.FeedPath))
        .SelectMany(static assembly => DeclaredUnicodeVersions(assembly.Name, assembly.Image))
        .ToArray();

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// The time data home's pins, as I4 holds this repository's to them (JSD-0076, decision SJL-D-6 (c);
    /// milestone SJL-8.5): each home file's SHA-256 and length, read at Broiler.JS commit
    /// <see cref="TimeHomeCommit"/> under <c>Broiler.ECMAScript/data/time/</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why constants, and not the package.</b> The Unicode area's generator package carries its
    /// owner's pin, so I4 reads it from the admitted package. The time data home's generator,
    /// <c>Broiler.ECMAScript.Generation</c>, carries no pin file: its caller hands it the pins' text.
    /// So the home's pins are recorded here as the digests of its files, with the commit they were read
    /// at, and the package's own declared data versions are read from the admitted package (the version
    /// clause). A move of the home's pins is a dated decision in both repositories at once (JSD-0076),
    /// and these constants move with it, naming the new commit.
    /// </para>
    /// <para>
    /// <b>What they are.</b> The home's <c>tzdb/tzdb.pin</c> and <c>calendars/calendars.pin</c> are
    /// byte-identical copies of this repository's, taken at Broiler.VM <c>9508bd5</c> (milestone SJL-8.2),
    /// so their digests are those of this repository's two pin files. The home's <c>cldr/cldr.pin</c>
    /// names one of the 114 files this repository's names, CLDR 48.2.0's <c>bcp47/timezone.json</c>, by
    /// this repository's own line for it; so the CLDR clause holds that one entry, by path, length and
    /// SHA-256, and not the pin file whole. Each pin names its archive's or its members' digests, so
    /// equal pins mean equal archived bytes, which rules N29 and N31 hold to the archives here.
    /// </para>
    /// </remarks>
    internal const string TimeHomeCommit = "2d37a38a2560b076ebf2ca29989bfe20db64ed64";

    /// <summary>The home's <c>Broiler.ECMAScript/data/time/tzdb/tzdb.pin</c>: its length and SHA-256 at <see cref="TimeHomeCommit"/>.</summary>
    internal static readonly (long Bytes, string Sha256) TimeHomeTzdbPin =
        (2775, "ef9c7443115a802db68895b29ad6c584ddec666280120b61e38061fcf5215f6f");

    /// <summary>The home's <c>Broiler.ECMAScript/data/time/calendars/calendars.pin</c>: its length and SHA-256 at <see cref="TimeHomeCommit"/>.</summary>
    internal static readonly (long Bytes, string Sha256) TimeHomeCalendarPin =
        (3687, "53c5b2d380a8c69b06648ed2b57db76684619d8fd3493582c196942423d795da");

    /// <summary>The one file the home's <c>cldr/cldr.pin</c> names, as its <c>file</c> line records it at <see cref="TimeHomeCommit"/>.</summary>
    internal static readonly (string Name, long Bytes, string Sha256) TimeHomeCldrTimeZoneKeys =
        ("cldr-48.2.0/json/cldr-bcp47/bcp47/timezone.json", 59702, "c11423884420f00cf52733c854de5c898ddb6657ce560334c638a2bd994a1448");

    /// <summary>
    /// I4's time pin clause: this repository's tzdb and calendar pin files equal the home's, by length
    /// and SHA-256, and its CLDR pin names the home's one time zone file with the home's length and digest.
    /// </summary>
    /// <param name="tzdbPin">This repository's <c>src/tests/tzdb/pins/tzdb.pin</c>, as bytes.</param>
    /// <param name="calendarPin">This repository's <c>src/tests/calendars/pins/calendars.pin</c>, as bytes.</param>
    /// <param name="cldrEntries">The files this repository's <c>src/tests/cldr/pins/cldr.pin</c> names: path, length and SHA-256.</param>
    internal static IEnumerable<string> I4TimePins(byte[] tzdbPin, byte[] calendarPin, IEnumerable<(string Name, long Bytes, string Sha256)> cldrEntries)
    {
        foreach (var (path, bytes, home) in new[]
        {
            ("src/tests/tzdb/pins/tzdb.pin", tzdbPin, TimeHomeTzdbPin),
            ("src/tests/calendars/pins/calendars.pin", calendarPin, TimeHomeCalendarPin),
        })
        {
            var digest = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

            if (bytes.LongLength != home.Bytes || !string.Equals(digest, home.Sha256, StringComparison.Ordinal))
            {
                yield return $"{path} is {bytes.LongLength} bytes hashing to {digest} here, and the time data home's copy is {home.Bytes} bytes hashing to {home.Sha256} at Broiler.JS {TimeHomeCommit[..8]} (rule I4: a pin moves only by a dated decision, in both repositories at once)";
            }
        }

        var keys = TimeHomeCldrTimeZoneKeys;
        var named = cldrEntries.Where(entry => string.Equals(entry.Name, keys.Name, StringComparison.Ordinal)).ToArray();

        if (named.Length != 1)
        {
            yield return $"src/tests/cldr/pins/cldr.pin names {keys.Name} {named.Length} times, and the time data home's cldr.pin names it once (rule I4)";
        }
        else if (named[0].Bytes != keys.Bytes || !string.Equals(named[0].Sha256, keys.Sha256, StringComparison.Ordinal))
        {
            yield return $"src/tests/cldr/pins/cldr.pin names {keys.Name} as {named[0].Bytes} bytes hashing to {named[0].Sha256}, and the time data home's cldr.pin as {keys.Bytes} bytes hashing to {keys.Sha256} at Broiler.JS {TimeHomeCommit[..8]} (rule I4)";
        }
    }

    /// <summary>
    /// The time data versions the home's generator declares and the pin of this repository that governs
    /// each: the field's full name, and the key of the version it is held to - <c>tzdb</c>, <c>cldr</c>,
    /// or a calendar crate's name.
    /// </summary>
    internal static readonly IReadOnlyDictionary<string, string> TimeVersionFields = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["Broiler.ECMAScript.Generation.Time.TimeDataVersions.TzdbRelease"] = "tzdb",
        ["Broiler.ECMAScript.Generation.Time.TimeDataVersions.CldrVersion"] = "cldr",
        ["Broiler.ECMAScript.Generation.Time.TimeDataVersions.IcuCalendarVersion"] = "icu_calendar",
        ["Broiler.ECMAScript.Generation.Time.TimeDataVersions.CalendricalCalculationsVersion"] = "calendrical_calculations",
        ["Broiler.ECMAScript.Generation.Time.TzdbPin.PinnedVersion"] = "tzdb",
        ["Broiler.ECMAScript.Generation.Time.CldrPin.PinnedVersion"] = "cldr",
    };

    /// <summary>
    /// Every time data version an assembly image declares: every literal string field of a type named
    /// <c>TimeDataVersions</c>, and the <c>PinnedVersion</c> of one of the <see cref="TimePinTypes"/>, read
    /// from its metadata tables without loading it.
    /// </summary>
    internal static IReadOnlyList<(string Assembly, string Field, string Value)> DeclaredTimeVersions(string assemblyName, byte[] image) =>
        DeclaredStrings(assemblyName, image, static (type, name) =>
            type.EndsWith(".TimeDataVersions", StringComparison.Ordinal) ||
            (name == "PinnedVersion" && TimePinTypes.Contains(type, StringComparer.Ordinal)));

    /// <summary>Every time data version every admitted assembly declares, read from the committed feed's packages.</summary>
    internal static IReadOnlyList<(string Assembly, string Field, string Value)> AdmittedTimeVersions() => AllowList
        .Where(static package => File.Exists(package.FeedPath))
        .SelectMany(static package => PackageAssemblies(package.FeedPath))
        .SelectMany(static assembly => DeclaredTimeVersions(assembly.Name, assembly.Image))
        .ToArray();

    /// <summary>
    /// I4's time version clause: every time data version an admitted assembly declares equals the
    /// version this repository's pin that governs it states; a declared version no pin governs is
    /// reported too, so that a new constant cannot pass unread.
    /// </summary>
    /// <param name="pinned">The versions this repository's pins state, by <see cref="TimeVersionFields"/>' keys.</param>
    /// <param name="declared">Each declaration: its assembly, its field and its value.</param>
    internal static IEnumerable<string> I4TimeVersions(IReadOnlyDictionary<string, string> pinned, IEnumerable<(string Assembly, string Field, string Value)> declared)
    {
        foreach (var (assembly, field, value) in declared)
        {
            if (!TimeVersionFields.TryGetValue(field, out var key) || !pinned.TryGetValue(key, out var version))
            {
                yield return $"{assembly} declares {field} = \"{value}\", and no pin of this repository governs it (rule I4)";
            }
            else if (!string.Equals(value, version, StringComparison.Ordinal))
            {
                yield return $"{assembly} declares {field} = \"{value}\", and this repository's {key} pin states {version} (rule I4)";
            }
        }
    }

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// The admitted types a decision has baselined on the family's public surface. None: an
    /// admitted type on that surface would make a shared package part of the family's public
    /// contract, and no decision has taken that.
    /// </summary>
    internal static readonly string[] BaselinedAdmittedTypes = [];

    /// <summary>
    /// I6: no admitted type appears in the JavaScript family's public API baseline unless a decision
    /// baselines it - rule B4 extended to the family and to the shared packages, which B4 does not
    /// reach.
    /// </summary>
    internal static IEnumerable<string> I6(string baselineName, IEnumerable<string> baselineLines, IEnumerable<string> admittedTypes)
    {
        var patterns = admittedTypes
            .Where(static type => !BaselinedAdmittedTypes.Contains(type, StringComparer.Ordinal))
            .Select(static type => (Type: type, Pattern: new System.Text.RegularExpressions.Regex(
                @"(?<![\w.])" + System.Text.RegularExpressions.Regex.Escape(type) + @"(?!\w)")))
            .ToArray();

        foreach (var line in baselineLines.Where(static line => !line.StartsWith('#')))
        {
            foreach (var (type, pattern) in patterns.Where(entry => entry.Pattern.IsMatch(line)))
            {
                yield return $"{baselineName} names the admitted type {type}: {line.Trim()}";
            }
        }
    }

    /// <summary>Every public type of every admitted assembly, read from the committed feed's packages.</summary>
    internal static IReadOnlyList<string> AdmittedPublicTypes() => AllowList
        .Where(static package => File.Exists(package.FeedPath))
        .SelectMany(static package => PackageAssemblies(package.FeedPath))
        .SelectMany(static assembly => AssemblyFacts.ReadImage(assembly.Name, assembly.Image).PublicTypeNames)
        .Distinct(StringComparer.Ordinal)
        .Order(StringComparer.Ordinal)
        .ToArray();

    /// <summary>
    /// The admitted assemblies as files, extracted from the committed feed into a directory keyed by
    /// content hash, so that a metadata-only reflection context can resolve a public signature that
    /// names one - which is what lets I6 see such a signature rather than the baseline run failing
    /// on it.
    /// </summary>
    internal static IReadOnlyList<string> AdmittedAssemblyPaths()
    {
        var paths = new List<string>();

        foreach (var package in AllowList.Where(static package => File.Exists(package.FeedPath)))
        {
            var key = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.ASCII.GetBytes(package.ContentHash)))[..16];
            var directory = Path.Combine(Path.GetTempPath(), "broiler-vm-admitted", package.Id + "." + package.Version + "-" + key);
            Directory.CreateDirectory(directory);

            foreach (var (name, image) in PackageAssemblies(package.FeedPath))
            {
                var path = Path.Combine(directory, name + ".dll");
                if (!File.Exists(path) || !File.ReadAllBytes(path).AsSpan().SequenceEqual(image))
                {
                    File.WriteAllBytes(path, image);
                }

                paths.Add(path);
            }
        }

        return paths;
    }

    // ---------------------------------------------------------------------------------------------

    /// <summary>
    /// I7: a bundle manifest that runs a composition carrying an admitted package records each such
    /// package's identifier, version, content hash and the source commit its nuspec names, so that
    /// the upstream revision a release's review gate reads is in the evidence (decision SJL-D-5 (a)).
    /// </summary>
    /// <param name="manifestName">The manifest's path, for messages.</param>
    /// <param name="manifest">The manifest.</param>
    /// <param name="closures">Each composition the bundle ran, with the assemblies its closure holds.</param>
    internal static IEnumerable<string> I7(
        string manifestName,
        JsonElement manifest,
        IReadOnlyDictionary<string, IReadOnlyList<string>> closures)
    {
        var recorded = manifest.TryGetProperty("admittedPackages", out var list)
            ? list.EnumerateArray().ToArray()
            : [];

        foreach (var (composition, closure) in closures)
        {
            foreach (var package in AllowList.Where(package => closure.Contains(package.Id, StringComparer.Ordinal)))
            {
                var entry = recorded.FirstOrDefault(e => e.TryGetProperty("id", out var id) && id.GetString() == package.Id);

                if (entry.ValueKind != JsonValueKind.Object)
                {
                    yield return $"{manifestName}: {composition} carries {package.Id} and the manifest does not record it";
                    continue;
                }

                foreach (var field in (string[])["version", "contentHash", "commit"])
                {
                    if (!entry.TryGetProperty(field, out var value) || string.IsNullOrWhiteSpace(value.GetString()))
                    {
                        yield return $"{manifestName}: {composition} carries {package.Id} and the manifest records no {field}";
                    }
                }
            }
        }
    }
}

/// <summary>SemVer 2.0 precedence, for the version clauses of N37 and I1: numeric core, then prerelease identifiers.</summary>
internal static class SemanticVersion
{
    internal static int Compare(string left, string right)
    {
        var (leftCore, leftPre) = Split(left);
        var (rightCore, rightPre) = Split(right);

        for (var i = 0; i < 3; i++)
        {
            var order = leftCore[i].CompareTo(rightCore[i]);
            if (order != 0)
            {
                return order;
            }
        }

        if (leftPre.Length == 0 || rightPre.Length == 0)
        {
            return rightPre.Length.CompareTo(leftPre.Length) == 0 ? 0 : leftPre.Length == 0 ? 1 : -1;
        }

        for (var i = 0; i < Math.Min(leftPre.Length, rightPre.Length); i++)
        {
            var leftNumeric = long.TryParse(leftPre[i], out var leftNumber);
            var rightNumeric = long.TryParse(rightPre[i], out var rightNumber);
            var order = (leftNumeric, rightNumeric) switch
            {
                (true, true) => leftNumber.CompareTo(rightNumber),
                (true, false) => -1,
                (false, true) => 1,
                _ => string.CompareOrdinal(leftPre[i], rightPre[i]),
            };

            if (order != 0)
            {
                return order;
            }
        }

        return leftPre.Length.CompareTo(rightPre.Length);
    }

    private static (long[] Core, string[] Prerelease) Split(string version)
    {
        var withoutMetadata = version.Split('+')[0];
        var dash = withoutMetadata.IndexOf('-', StringComparison.Ordinal);
        var core = (dash < 0 ? withoutMetadata : withoutMetadata[..dash]).Split('.');
        var numbers = new long[3];

        for (var i = 0; i < 3 && i < core.Length; i++)
        {
            numbers[i] = long.Parse(core[i], System.Globalization.CultureInfo.InvariantCulture);
        }

        return (numbers, dash < 0 ? [] : withoutMetadata[(dash + 1)..].Split('.'));
    }
}
