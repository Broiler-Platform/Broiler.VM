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
        ImmutableArray<string> publicTypeNames)
    {
        Name = name;
        AssemblyReferences = assemblyReferences;
        MemberReferences = memberReferences;
        TypeReferences = typeReferences;
        CustomAttributeTypes = customAttributeTypes;
        PublicTypeNames = publicTypeNames;
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

    internal static bool IsFramework(string assemblyName) =>
        FrameworkPrefixes.Any(prefix =>
            string.Equals(assemblyName, prefix, StringComparison.Ordinal) ||
            assemblyName.StartsWith(prefix + ".", StringComparison.Ordinal));

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

        return new AssemblyFacts(
            assemblyName, assemblyReferences, memberReferences, typeReferences,
            customAttributeTypes, publicTypeNames);
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
