using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Reads a built assembly's metadata tables directly, without loading it.
/// </summary>
/// <remarks>
/// <para>
/// Group B rules read the AssemblyRef, MemberRef, TypeDef and CustomAttribute tables rather than
/// loading an assembly and reflecting over it. Loading would run module initializers - which
/// invariant 2 forbids and which rule B5b exists to detect - and would let a rule pass because
/// the runtime resolved something the metadata never named. Reading the tables asks the question
/// the rule actually asks: what does this file say it needs?
/// </para>
/// <para>
/// Product assemblies are located in the test output directory, which is where the four
/// ProjectReferences copy them.
/// </para>
/// </remarks>
internal sealed class AssemblyFacts
{
    private static readonly string[] FrameworkPrefixes = ["System", "Microsoft.CSharp", "netstandard", "mscorlib"];

    private AssemblyFacts(
        string name,
        ImmutableArray<string> assemblyReferences,
        ImmutableArray<string> memberReferences,
        ImmutableArray<string> typeReferences,
        ImmutableArray<string> customAttributeTypes,
        ImmutableArray<string> publicTypeNames,
        ImmutableArray<string> platformInvokes)
    {
        Name = name;
        AssemblyReferences = assemblyReferences;
        MemberReferences = memberReferences;
        TypeReferences = typeReferences;
        CustomAttributeTypes = customAttributeTypes;
        PublicTypeNames = publicTypeNames;
        PlatformInvokes = platformInvokes;
    }

    internal string Name { get; }

    /// <summary>Every assembly named in the AssemblyRef table.</summary>
    internal ImmutableArray<string> AssemblyReferences { get; }

    /// <summary>Every member reference, as "Namespace.Type.Member".</summary>
    internal ImmutableArray<string> MemberReferences { get; }

    /// <summary>
    /// Every type named in the TypeRef table, as "Namespace.Type". A type can be reached without
    /// any member reference - held in a field, named in a signature - so a rule that only reads
    /// MemberRef would miss it.
    /// </summary>
    internal ImmutableArray<string> TypeReferences { get; }

    /// <summary>Every custom attribute type applied anywhere in the assembly.</summary>
    internal ImmutableArray<string> CustomAttributeTypes { get; }

    /// <summary>Every public type, as "Namespace.Type".</summary>
    internal ImmutableArray<string> PublicTypeNames { get; }

    /// <summary>
    /// Every platform invoke this assembly declares, as "module!entryPoint".
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the table a rule about native memory has to read, and it is not the MemberRef
    /// table.</b> A call to <c>VirtualProtect</c> or <c>mprotect</c> names no member of any
    /// referenced assembly: the target is a row in this assembly's own ImplMap, reached from a
    /// method definition carrying <c>PinvokeImpl</c>, and the library and entry-point names live
    /// in the ModuleRef and ImplMap tables that the other properties here never open. A rule
    /// written over member and type references alone is blind to it by construction.
    /// </para>
    /// <para>
    /// <b><c>[DllImport]</c> and <c>[LibraryImport]</c> are caught identically</b>, because the
    /// interop source generator emits an ordinary <c>PinvokeImpl</c> method into the same assembly
    /// rather than calling out to one. Reading the metadata is therefore the reading that does not
    /// depend on which of the two spellings an author chose.
    /// </para>
    /// </remarks>
    internal ImmutableArray<string> PlatformInvokes { get; }

    /// <summary>References to anything that is not part of the framework.</summary>
    internal IEnumerable<string> NonFrameworkReferences =>
        AssemblyReferences.Where(static reference => !IsFramework(reference));

    internal static AssemblyFacts Abstractions { get; } = Read("Broiler.VM.Abstractions");

    internal static AssemblyFacts Binary { get; } = Read("Broiler.VM.Binary");

    internal static AssemblyFacts Runtime { get; } = Read("Broiler.VM.Runtime");

    internal static AssemblyFacts Fixtures { get; } = Read("Broiler.VM.Fixtures");

    /// <summary>
    /// The test assembly, used as the witness input for the group B rules it genuinely violates:
    /// it references xunit, so it breaks B1's framework-only rule, and it contains
    /// <see cref="DynamicLoadingWitness"/>, so it breaks B5.
    /// </summary>
    internal static AssemblyFacts TestAssembly { get; } = Read("Broiler.VM.Architecture.Tests");

    /// <summary>The three product assemblies, in dependency order.</summary>
    internal static IReadOnlyList<AssemblyFacts> Product { get; } = [Abstractions, Binary, Runtime];

    /// <summary>
    /// Every assembly a published image of this repository can contain, read from its build
    /// output: the core three, the profile families, the composition roots, the consumer profiles
    /// a demonstration composition links, and the hosts.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Group B used to evaluate over <see cref="Product"/> while its register rows said "the
    /// product graph", and the gap was not a wording problem.</b> The three core assemblies are
    /// the three that could least plausibly reach a dynamic-loading or a memory-mapping API; every
    /// assembly that could was outside the sweep. A profile could have referenced
    /// <c>System.Reflection.Emit</c>, or declared a platform invoke to <c>VirtualProtect</c>, and
    /// the rule that forbids both would have stayed green while the register published the
    /// prohibition. The scope is the graph the rows already claimed.
    /// </para>
    /// <para>
    /// <b>The partition is by what ships and not by the path partition ADR 0001 uses elsewhere.</b>
    /// The two xunit projects are excluded, because they are the collector rather than the
    /// collected and one of them is deliberately the witness that every group B rule is shown
    /// rejecting. Everything else in the checkout is in, including the two consumer profiles that
    /// live under <c>src/tests/</c> and ship inside a demonstration composition's closure - a
    /// mapping call in one of those would be in a published image, which is the only question
    /// these rules ask.
    /// </para>
    /// <para>
    /// <b>An assembly with no build output is named rather than skipped.</b> The mobile head needs
    /// a workload the main solution does not require, so a plain <c>dotnet test</c> never produces
    /// it; <see cref="Unread"/> carries the names, and the rules that sweep this list assert what
    /// covers those assemblies instead rather than letting an absence read as a clean result.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<AssemblyFacts> Shipping => ShippingScan.Read;

    /// <summary>The shipping assemblies whose build output this run could not find.</summary>
    internal static IReadOnlyList<string> Unread => ShippingScan.Unread;

    /// <summary>The one scan behind both, so the two answers cannot disagree about a run.</summary>
    private static readonly (IReadOnlyList<AssemblyFacts> Read, IReadOnlyList<string> Unread)
        ShippingScan = ReadShipping();

    internal static bool IsFramework(string assemblyName) =>
        FrameworkPrefixes.Any(prefix =>
            string.Equals(assemblyName, prefix, StringComparison.Ordinal) ||
            assemblyName.StartsWith(prefix + ".", StringComparison.Ordinal));

    /// <summary>
    /// Every shipping assembly whose build output is on disk, and the names of the ones that are
    /// not.
    /// </summary>
    /// <remarks>
    /// The configuration and target framework come from this test run's own output path, the way
    /// <see cref="ProfileApiSurface"/> takes them, so what is read is the build that just happened
    /// rather than a Release build that might be from last week.
    /// </remarks>
    private static (IReadOnlyList<AssemblyFacts> Read, IReadOnlyList<string> Unread) ReadShipping()
    {
        var read = new List<AssemblyFacts>();
        var unread = new List<string>();

        foreach (var project in ComponentGraph.Projects
                     .Where(static project => !project.IsWitness)
                     .Where(static project => !project.RawText.Contains(
                         "<IsTestProject>true</IsTestProject>", StringComparison.Ordinal))
                     .OrderBy(static project => project.AssemblyName, StringComparer.Ordinal))
        {
            var path = Path.Combine(
                Path.GetDirectoryName(project.Path)!,
                "bin",
                ProfileApiSurface.Configuration,
                ProfileApiSurface.TargetFramework,
                project.AssemblyName + ".dll");

            if (File.Exists(path))
            {
                read.Add(ReadAt(project.AssemblyName, path));
                continue;
            }

            unread.Add(project.AssemblyName);
        }

        return (read, unread);
    }

    private static AssemblyFacts Read(string assemblyName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, assemblyName + ".dll");

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"{assemblyName}.dll is not in the test output. The architecture tests assert " +
                "against built metadata, so every project in the graph must be referenced by " +
                "this test project.", path);
        }

        return ReadAt(assemblyName, path);
    }

    /// <summary>
    /// Reads one assembly's metadata from a path this project does not reference.
    /// </summary>
    /// <remarks>
    /// Rule A11 forbids a test project referencing a profile assembly, so a profile's build output
    /// is not in this test's own directory and cannot be. Reading the file's tables is not
    /// referencing it and is not loading it: nothing in the file runs, which is the same reason
    /// every other group B answer is read rather than reflected over.
    /// </remarks>
    private static AssemblyFacts ReadAt(string assemblyName, string path)
    {
        using var stream = File.OpenRead(path);
        using var reader = new PEReader(stream);
        var metadata = reader.GetMetadataReader();

        var assemblyReferences = metadata.AssemblyReferences
            .Select(handle => metadata.GetString(metadata.GetAssemblyReference(handle).Name))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToImmutableArray();

        var memberReferences = metadata.MemberReferences
            .Select(handle => Describe(metadata, metadata.GetMemberReference(handle)))
            .Where(static description => description is not null)
            .Select(static description => description!)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static description => description, StringComparer.Ordinal)
            .ToImmutableArray();

        var typeReferences = metadata.TypeReferences
            .Select(handle => metadata.GetTypeReference(handle))
            .Select(reference => Qualify(
                metadata.GetString(reference.Namespace), metadata.GetString(reference.Name)))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToImmutableArray();

        var customAttributeTypes = metadata.CustomAttributes
            .Select(handle => DescribeAttribute(metadata, metadata.GetCustomAttribute(handle)))
            .Where(static description => description is not null)
            .Select(static description => description!)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static description => description, StringComparer.Ordinal)
            .ToImmutableArray();

        // Nested types carry NestedPublic (0x2), never Public (0x1), so testing for Public alone
        // would hide a publicly reachable nested type from B7 - whose entire job is catching a
        // type named BuiltInProfiles - and from E5's "exactly one exported type". A nested type
        // is exported only when every type enclosing it is too, so the chain is walked.
        var publicTypeNames = metadata.TypeDefinitions
            .Select(metadata.GetTypeDefinition)
            .Where(definition => IsExported(metadata, definition))
            .Select(definition => ExportedName(metadata, definition))
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToImmutableArray();

        // The ImplMap walk. A platform invoke is a method definition of THIS assembly carrying
        // PinvokeImpl, whose import names a ModuleRef row and an entry point; the interop source
        // generator emits exactly that shape, so [DllImport] and [LibraryImport] are one reading.
        var platformInvokes = metadata.MethodDefinitions
            .Select(metadata.GetMethodDefinition)
            .Where(static definition =>
                (definition.Attributes & MethodAttributes.PinvokeImpl) != 0)
            .Select(definition => DescribeImport(metadata, definition))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static entry => entry, StringComparer.Ordinal)
            .ToImmutableArray();

        return new AssemblyFacts(
            assemblyName, assemblyReferences, memberReferences, typeReferences,
            customAttributeTypes, publicTypeNames, platformInvokes);
    }

    /// <summary>
    /// One platform invoke, as "module!entryPoint".
    /// </summary>
    /// <remarks>
    /// An import that names no entry point takes the method's own name, which is what the runtime
    /// does; an import that names no module is recorded with the module left blank rather than
    /// dropped, because a row this reader cannot describe is still a row.
    /// </remarks>
    private static string DescribeImport(MetadataReader metadata, MethodDefinition definition)
    {
        var import = definition.GetImport();

        var module = import.Module.IsNil
            ? string.Empty
            : metadata.GetString(metadata.GetModuleReference(import.Module).Name);

        var entryPoint = import.Name.IsNil
            ? metadata.GetString(definition.Name)
            : metadata.GetString(import.Name);

        return module + "!" + entryPoint;
    }

    /// <summary>
    /// True when the type is reachable from outside the assembly: public at the top level, or
    /// nested public inside a chain of types that are all themselves exported.
    /// </summary>
    private static bool IsExported(MetadataReader metadata, TypeDefinition definition)
    {
        var visibility = definition.Attributes & TypeAttributes.VisibilityMask;

        if (visibility == TypeAttributes.Public)
        {
            return true;
        }

        if (visibility != TypeAttributes.NestedPublic)
        {
            return false;
        }

        var declaring = definition.GetDeclaringType();

        return !declaring.IsNil && IsExported(metadata, metadata.GetTypeDefinition(declaring));
    }

    /// <summary>
    /// The exported name, with nesting spelled the way metadata spells it, so that a simple-name
    /// check still finds the leaf of a nested type.
    /// </summary>
    private static string ExportedName(MetadataReader metadata, TypeDefinition definition)
    {
        var name = metadata.GetString(definition.Name);
        var declaring = definition.GetDeclaringType();

        return declaring.IsNil
            ? Qualify(metadata.GetString(definition.Namespace), name)
            : ExportedName(metadata, metadata.GetTypeDefinition(declaring)) + "+" + name;
    }

    private static string? Describe(MetadataReader metadata, MemberReference member)
    {
        if (member.Parent.Kind != HandleKind.TypeReference)
        {
            return null;
        }

        var declaringType = metadata.GetTypeReference((TypeReferenceHandle)member.Parent);

        return Qualify(
            metadata.GetString(declaringType.Namespace),
            metadata.GetString(declaringType.Name)) + "." + metadata.GetString(member.Name);
    }

    private static string? DescribeAttribute(MetadataReader metadata, CustomAttribute attribute)
    {
        if (attribute.Constructor.Kind != HandleKind.MemberReference)
        {
            return null;
        }

        var constructor = metadata.GetMemberReference((MemberReferenceHandle)attribute.Constructor);

        if (constructor.Parent.Kind != HandleKind.TypeReference)
        {
            return null;
        }

        var declaringType = metadata.GetTypeReference((TypeReferenceHandle)constructor.Parent);

        return Qualify(
            metadata.GetString(declaringType.Namespace),
            metadata.GetString(declaringType.Name));
    }

    private static string Qualify(string @namespace, string name) =>
        string.IsNullOrEmpty(@namespace) ? name : @namespace + "." + name;
}
