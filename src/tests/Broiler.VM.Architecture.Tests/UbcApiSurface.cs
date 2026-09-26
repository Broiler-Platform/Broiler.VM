using System.Reflection;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The public surface of the universal bytecode assembly, <c>Broiler.VM.Ubc</c>, as text and as the
/// identifiers it exports.
/// </summary>
/// <remarks>
/// <para>
/// <b>A fourth list, a fourth baseline, and the same describer.</b> The packable three, the
/// JavaScript family and the WebAssembly family each freeze their own surface in their own file, and
/// <see cref="ProfileApiSurface"/>'s remark says why a new subject adds a list rather than widening
/// one: two subjects are two published artefacts, and a rule holding both to one file would freeze a
/// surface whose halves move independently. The universal bytecode is not a profile family - it is
/// the format, schema and mechanism every family is written against - so it is a subject of its own
/// and rule U9 holds it to <c>docs/ubc/api/public-api.txt</c>.
/// </para>
/// <para>
/// <b>Why a <see cref="MetadataLoadContext"/> and not a project reference.</b> The architecture test
/// project does not reference <c>Broiler.VM.Ubc</c>, and adding the edge would grow the frozen graph
/// for a test's convenience - rules A7 and A15 hold <c>graph.manifest.json</c> and ADR 0001's budget
/// sentence to the checkout. Reading the build output with a metadata-only context describes the
/// assembly without running it, which is the reason <see cref="WebAssemblyApiSurface"/> gives and the
/// reason every line here is written by <see cref="ApiSurface"/>'s own describer: the four baselines
/// are one format.
/// </para>
/// <para>
/// <b>Rule U2 reads the same load.</b> Its identifier clause scans every name the assembly declares
/// on its exported surface - types, members, enum members, parameters and generic parameters - and
/// its family-row clause reads the described lines this class produces, so the rule that bans a word
/// and the rule that freezes the surface cannot be looking at two different builds.
/// </para>
/// </remarks>
internal static class UbcApiSurface
{
    /// <summary>The universal bytecode's assemblies: one, because the format has one home.</summary>
    internal static readonly string[] Assemblies =
    [
        "Broiler.VM.Ubc",
    ];

    /// <summary>Describes the public surface of the assembly, sorted.</summary>
    internal static IReadOnlyList<string> Describe()
    {
        var lines = new List<string>();

        Load(assembly => lines.AddRange(ApiSurface.Describe(assembly)));

        lines.Sort(StringComparer.Ordinal);
        return lines;
    }

    /// <summary>
    /// Every identifier the assembly declares on its exported surface, with where it is declared.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The identifiers are the names an author CHOSE: an exported type's name without its generic
    /// arity, its namespace, its generic parameters, and for every member the describer counts as
    /// surface - public, protected and protected internal - the member's name, its generic parameters
    /// and its parameters' names. An enum member is a field here and is scanned as one.
    /// </para>
    /// <para>
    /// Names the compiler writes rather than an author are left out, because no author can rename them
    /// and a vocabulary rule that fired on one would be a rule nobody could satisfy: constructor
    /// names, property and event accessors (their property or event is scanned instead), operator
    /// method names (their parameters are scanned), and the special <c>value__</c> field of an enum.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<UbcRules.ExportedIdentifier> Identifiers()
    {
        var identifiers = new List<UbcRules.ExportedIdentifier>();

        Load(assembly => identifiers.AddRange(IdentifiersOf(assembly)));

        return identifiers;
    }

    /// <summary>Which of the assemblies were found on disk, in the order named.</summary>
    internal static IReadOnlyList<string> Found() => Assemblies
        .Where(static name => File.Exists(AssemblyPath(name)))
        .ToArray();

    private static void Load(Action<Assembly> read)
    {
        var files = Assemblies
            .Select(AssemblyPath)
            .Where(File.Exists)
            .ToArray();

        if (files.Length == 0)
        {
            return;
        }

        var resolverPaths = new List<string>(files);
        resolverPaths.AddRange(Directory.EnumerateFiles(
            Path.GetDirectoryName(typeof(object).Assembly.Location)!, "*.dll"));
        resolverPaths.AddRange(Directory.EnumerateFiles(AppContext.BaseDirectory, "*.dll"));

        using var context = new MetadataLoadContext(
            new PathAssemblyResolver(resolverPaths.Distinct(StringComparer.OrdinalIgnoreCase)));

        foreach (var file in files)
        {
            read(context.LoadFromAssemblyPath(file));
        }
    }

    private static IEnumerable<UbcRules.ExportedIdentifier> IdentifiersOf(Assembly assembly)
    {
        const BindingFlags Flags =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        var namespaces = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var type in assembly.GetExportedTypes().OrderBy(static type => type.FullName, StringComparer.Ordinal))
        {
            var owner = Display(type);

            if (type.Namespace is { } name)
            {
                namespaces.Add(name);
            }

            yield return new("type", owner, Bare(type.Name));

            // A nested type repeats its declaring type's generic parameters; only the ones it
            // declares itself are its own.
            var inherited = type.IsNested ? type.DeclaringType!.GetGenericArguments().Length : 0;

            foreach (var parameter in type.GetGenericArguments().Skip(inherited))
            {
                yield return new("generic parameter", owner, parameter.Name);
            }

            foreach (var member in type.GetMembers(Flags).OrderBy(static member => member.Name, StringComparer.Ordinal))
            {
                foreach (var identifier in IdentifiersOf(type, owner, member))
                {
                    yield return identifier;
                }
            }
        }

        foreach (var name in namespaces)
        {
            yield return new("namespace", name, name);
        }
    }

    private static IEnumerable<UbcRules.ExportedIdentifier> IdentifiersOf(Type type, string owner, MemberInfo member)
    {
        switch (member)
        {
            case FieldInfo field when Visible(field.IsPublic, field.IsFamily, field.IsFamilyOrAssembly) &&
                !field.IsSpecialName:
                yield return new(type.IsEnum ? "enum member" : "field", owner + "." + field.Name, field.Name);
                break;

            case PropertyInfo property when property.GetAccessors(nonPublic: true)
                .Any(static accessor => Visible(accessor.IsPublic, accessor.IsFamily, accessor.IsFamilyOrAssembly)):
                yield return new("property", owner + "." + property.Name, property.Name);

                foreach (var parameter in property.GetIndexParameters())
                {
                    yield return new("parameter", owner + "." + property.Name + "[" + parameter.Name + "]", parameter.Name ?? string.Empty);
                }

                break;

            case EventInfo declared when declared.AddMethod is { } add &&
                Visible(add.IsPublic, add.IsFamily, add.IsFamilyOrAssembly):
                yield return new("event", owner + "." + declared.Name, declared.Name);
                break;

            case MethodBase method when Visible(method.IsPublic, method.IsFamily, method.IsFamilyOrAssembly):
            {
                // Accessors are scanned as their property or event, whose name is the author's.
                if (method is MethodInfo { IsSpecialName: true } accessor &&
                    (accessor.Name.StartsWith("get_", StringComparison.Ordinal) ||
                     accessor.Name.StartsWith("set_", StringComparison.Ordinal) ||
                     accessor.Name.StartsWith("add_", StringComparison.Ordinal) ||
                     accessor.Name.StartsWith("remove_", StringComparison.Ordinal)))
                {
                    yield break;
                }

                var where = owner + "." + (method is ConstructorInfo ? "ctor" : method.Name);

                // A constructor's and an operator's name are the compiler's; their parameters are not.
                if (!method.IsSpecialName && method is not ConstructorInfo)
                {
                    yield return new("method", where, method.Name);
                }

                if (method.IsGenericMethodDefinition)
                {
                    foreach (var parameter in method.GetGenericArguments())
                    {
                        yield return new("generic parameter", where, parameter.Name);
                    }
                }

                foreach (var parameter in method.GetParameters().Where(static parameter => parameter.Name is not null))
                {
                    yield return new("parameter", where + "(" + parameter.Name + ")", parameter.Name!);
                }

                break;
            }
        }
    }

    /// <summary>The same visibility the describer counts as surface.</summary>
    private static bool Visible(bool isPublic, bool isFamily, bool isFamilyOrAssembly) =>
        isPublic || isFamily || isFamilyOrAssembly;

    private static string Display(Type type) =>
        type.IsNested ? Display(type.DeclaringType!) + "." + Bare(type.Name) : (type.Namespace is { } name ? name + "." : string.Empty) + Bare(type.Name);

    private static string Bare(string name)
    {
        var tick = name.IndexOf('`', StringComparison.Ordinal);
        return tick >= 0 ? name[..tick] : name;
    }

    /// <summary>
    /// Where the assembly's build output sits: the configuration and framework this test run was built
    /// in, read the way <see cref="ProfileApiSurface"/> reads them.
    /// </summary>
    private static string AssemblyPath(string assemblyName) => Path.Combine(
        ComponentGraph.Root, "src", assemblyName, "bin",
        ProfileApiSurface.Configuration, ProfileApiSurface.TargetFramework,
        assemblyName + ".dll");
}
