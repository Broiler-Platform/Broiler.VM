namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Every architecture rule as a predicate, so that the same code decides both halves of the
/// question a rule has to answer: does the checkout violate it, and would it catch a violation?
/// </summary>
/// <remarks>
/// Expressing a rule twice - once to assert the component is clean and once to assert a witness
/// is rejected - is how a rule stops being a shape-only stub. Roadmap section 13's VM-0 gate
/// asks for rules that "express every forbidden edge"; a rule that has never rejected anything
/// expresses nothing. ADR 0001 owns group A, group B and rule D1; the register in
/// rules.register.json records each rule's status and its witness.
/// </remarks>
internal static class ArchitectureRules
{
    /// <summary>The three package identities the component is permitted to publish.</summary>
    internal static readonly string[] DeclaredPackageIds =
        ["Broiler.VM.Abstractions", "Broiler.VM.Binary", "Broiler.VM.Runtime"];

    /// <summary>Every assembly the component itself produces.</summary>
    internal static readonly string[] ComponentAssemblies =
    [
        "Broiler.VM.Abstractions",
        "Broiler.VM.Binary",
        "Broiler.VM.Runtime",
        "Broiler.VM.Fixtures",
        "Broiler.VM.Architecture.Tests",
    ];

    /// <summary>
    /// The complete legal ProjectReference edge multiset, generated from graph.manifest.json.
    /// Abstractions and Binary are sinks, which is what makes the graph acyclic by construction
    /// rather than by inspection.
    /// </summary>
    internal static IReadOnlyList<GraphManifest.Edge> DeclaredEdges => GraphManifest.Edges;

    // ---- Group A: project files -------------------------------------------------------------

    /// <summary>A1: no ProjectReference resolves outside the component root.</summary>
    internal static IEnumerable<string> A1(ComponentGraph.ProjectFile project) =>
        project.ProjectReferences
            .Where(static reference => !reference.Contains('$'))
            .Where(reference => !reference.StartsWith(ComponentGraph.Root, StringComparison.OrdinalIgnoreCase))
            .Select(reference => $"{project.RelativePath} -> {reference}");

    /// <summary>A2: no PackageReference names a Broiler package.</summary>
    internal static IEnumerable<string> A2(ComponentGraph.ProjectFile project) =>
        project.PackageReferences
            .Where(static package => package.StartsWith("Broiler.", StringComparison.OrdinalIgnoreCase))
            .Select(package => $"{project.RelativePath} -> package {package}");

    /// <summary>
    /// A3: no source or import item escapes the component root. A shared source link satisfies
    /// A1 and A2 while creating exactly the coupling roadmap section 9 forbids by name.
    /// </summary>
    internal static IEnumerable<string> A3(ComponentGraph.ProjectFile project) =>
        project.SourceItemPaths
            .Where(static item => !item.Contains('$'))
            .Where(item => !item.StartsWith(ComponentGraph.Root, StringComparison.OrdinalIgnoreCase))
            .Select(item => $"{project.RelativePath} includes {item}");

    /// <summary>A4: no product project references a test-only project.</summary>
    internal static IEnumerable<string> A4(ComponentGraph.ProjectFile project)
    {
        if (project.IsTestOnly)
        {
            return [];
        }

        var testsRoot = Path.Combine(ComponentGraph.Root, "src", "tests");

        return project.ProjectReferences
            .Where(reference => reference.StartsWith(testsRoot, StringComparison.OrdinalIgnoreCase))
            .Select(reference => $"{project.RelativePath} -> {Path.GetFileName(reference)}");
    }

    /// <summary>
    /// A5: every test-only project literally declares IsPackable false. The text, not the
    /// evaluated property: the vendored packaging props turn packability on by name suffix, and
    /// Broiler.VM.Fixtures matches no suffix rule, so the opt-out has to be visible in the file.
    /// </summary>
    internal static IEnumerable<string> A5(ComponentGraph.ProjectFile project)
    {
        var isTestShaped = project.IsTestOnly ||
            (project.IsWitness && project.AssemblyName.StartsWith("Broiler.VM.Fixtures", StringComparison.Ordinal));

        if (!isTestShaped)
        {
            return [];
        }

        return project.RawText.Contains("<IsPackable>false</IsPackable>", StringComparison.Ordinal)
            ? []
            : [$"{project.RelativePath} does not literally declare <IsPackable>false</IsPackable>"];
    }

    /// <summary>
    /// A6: a project that declares a PackageId declares one of exactly three, and its identity
    /// fields agree with each other and with the file name.
    /// </summary>
    internal static IEnumerable<string> A6(ComponentGraph.ProjectFile project)
    {
        if (project.PackageId is null)
        {
            yield break;
        }

        if (!DeclaredPackageIds.Contains(project.PackageId, StringComparer.Ordinal))
        {
            yield return $"{project.RelativePath} declares undeclared PackageId {project.PackageId}";
        }

        var fileBaseName = Path.GetFileNameWithoutExtension(
            project.IsWitness ? Path.GetFileNameWithoutExtension(project.Path) : project.Path);

        foreach (var (field, value) in new[]
                 {
                     ("AssemblyName", project.AssemblyName),
                     ("RootNamespace", project.RootNamespace),
                     ("file name", project.IsWitness ? project.PackageId : fileBaseName),
                 })
        {
            if (!string.Equals(value, project.PackageId, StringComparison.Ordinal))
            {
                yield return $"{project.RelativePath}: {field} '{value}' != PackageId '{project.PackageId}'";
            }
        }
    }

    /// <summary>A8: a profile project never references the runtime.</summary>
    internal static IEnumerable<string> A8(ComponentGraph.ProjectFile project)
    {
        if (!IsProfileShaped(project.AssemblyName))
        {
            return [];
        }

        return project.ReferencedAssemblyNames
            .Where(static name => string.Equals(name, "Broiler.VM.Runtime", StringComparison.Ordinal))
            .Select(name => $"{project.RelativePath} -> {name}");
    }

    /// <summary>A9: the runtime is a library, never a composition root.</summary>
    internal static IEnumerable<string> A9(ComponentGraph.ProjectFile project)
    {
        if (!string.Equals(project.AssemblyName, "Broiler.VM.Runtime", StringComparison.Ordinal))
        {
            return [];
        }

        return project.OutputType is null or "Library"
            ? []
            : [$"{project.RelativePath} declares OutputType {project.OutputType}"];
    }

    /// <summary>A10: no product project opens its internals.</summary>
    internal static IEnumerable<string> A10(ComponentGraph.ProjectFile project)
    {
        var isProductShaped = project.IsProduct ||
            (project.IsWitness && DeclaredPackageIds.Contains(project.AssemblyName, StringComparer.Ordinal));

        if (!isProductShaped)
        {
            return [];
        }

        return project.InternalsVisibleTo
            .Select(target => $"{project.RelativePath} opens internals to {target}");
    }

    /// <summary>
    /// A11: no project outside the composition-root allow-list references a profile assembly.
    /// The allow-list is empty at VM-0 because the component declares no composition, so every
    /// profile reference violates the rule.
    /// </summary>
    internal static IEnumerable<string> A11(ComponentGraph.ProjectFile project) =>
        project.ReferencedAssemblyNames
            .Where(static name => name.StartsWith("Broiler.VM.Profile.", StringComparison.Ordinal))
            .Select(name => $"{project.RelativePath} -> {name}");

    // ---- Group B: compiled metadata ---------------------------------------------------------

    /// <summary>B1: an assembly references nothing outside the framework.</summary>
    internal static IEnumerable<string> B1(AssemblyFacts assembly) =>
        assembly.NonFrameworkReferences.Select(reference => $"{assembly.Name} -> {reference}");

    /// <summary>
    /// B2: the runtime references nothing outside Abstractions and Binary. Stated as a subset at
    /// VM-0 because the runtime is a shell and names neither yet; it tightens to equality at
    /// VM-1, when it uses them.
    /// </summary>
    internal static IEnumerable<string> B2(AssemblyFacts assembly) =>
        assembly.NonFrameworkReferences
            .Where(static reference => reference is not ("Broiler.VM.Abstractions" or "Broiler.VM.Binary"))
            .Select(reference => $"{assembly.Name} -> {reference}");

    /// <summary>
    /// B3: no assembly names a Broiler assembly outside the component's own set. The
    /// assembly-level twin of A1 and A2: it survives a package-based reintroduction of the
    /// legacy edge, which is why both levels are required rather than either alone.
    /// </summary>
    internal static IEnumerable<string> B3(AssemblyFacts assembly) =>
        assembly.AssemblyReferences
            .Where(static reference => reference.StartsWith("Broiler.", StringComparison.Ordinal))
            .Where(static reference => !ComponentAssemblies.Contains(reference, StringComparer.Ordinal))
            .Select(reference => $"{assembly.Name} -> {reference}");

    /// <summary>B6: no product assembly references an assembly built from src/tests/.</summary>
    internal static IEnumerable<string> B6(AssemblyFacts assembly) =>
        assembly.AssemblyReferences
            .Where(static reference =>
                reference is "Broiler.VM.Fixtures" or "Broiler.VM.Architecture.Tests")
            .Select(reference => $"{assembly.Name} -> {reference}");

    /// <summary>
    /// B7: no product assembly exports an aggregate profile type. Roadmap section 3 rejects a
    /// BuiltInProfiles-shaped type by name, because it would reference every profile assembly
    /// and defeat VM-3's exact-closure gates.
    /// </summary>
    internal static IEnumerable<string> B7(AssemblyFacts assembly) =>
        assembly.PublicTypeNames
            .Where(static name =>
            {
                var simpleName = name[(name.LastIndexOf('.') + 1)..];
                return simpleName is "BuiltInProfiles" or "DefaultProfiles"
                    or "AllProfiles" or "KnownProfiles";
            })
            .Select(name => $"{assembly.Name} exports {name}");

    /// <summary>
    /// B4: no exported member of a product assembly names a type outside System.* and
    /// Broiler.VM. The idiom is the one already proven in the aggregate checkout by
    /// DomArchitectureTests.Public_Surface_Does_Not_Leak_Forbidden_Broiler_Types.
    /// </summary>
    /// <remarks>
    /// Only Broiler.VM.Abstractions is passed here, because rule E5 establishes from metadata
    /// that the other two product assemblies export no type at all, and an assembly that exports
    /// no type has no exported member to leak.
    /// </remarks>
    internal static IEnumerable<string> B4(System.Reflection.Assembly assembly) =>
        assembly
            .GetExportedTypes()
            .SelectMany(static type => type.GetMembers(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly))
            .SelectMany(MemberTypes)
            .Where(static type => !IsPermittedOnThePublicSurface(type))
            .Select(static type => $"public surface names {type.FullName}")
            .Distinct(StringComparer.Ordinal);

    private static IEnumerable<Type> MemberTypes(System.Reflection.MemberInfo member) => member switch
    {
        System.Reflection.MethodInfo method =>
            [method.ReturnType, .. method.GetParameters().Select(static parameter => parameter.ParameterType)],
        System.Reflection.PropertyInfo property => [property.PropertyType],
        System.Reflection.FieldInfo field => [field.FieldType],
        System.Reflection.EventInfo eventInfo when eventInfo.EventHandlerType is not null =>
            [eventInfo.EventHandlerType],
        _ => [],
    };

    private static bool IsPermittedOnThePublicSurface(Type type)
    {
        var @namespace = type.Namespace ?? string.Empty;

        return @namespace.StartsWith("System", StringComparison.Ordinal) ||
            @namespace == "Broiler.VM" ||
            @namespace.StartsWith("Broiler.VM.", StringComparison.Ordinal);
    }

    /// <summary>
    /// B5: no assembly reaches a dynamic-loading, reflection-invocation or IL-emit API.
    /// Invariant 2 requires registration to be static and typed.
    /// </summary>
    internal static IEnumerable<string> B5(AssemblyFacts assembly)
    {
        string[] forbidden =
        [
            "System.Reflection.Assembly.Load",
            "System.Reflection.Assembly.LoadFrom",
            "System.Reflection.Assembly.LoadFile",
            "System.Reflection.Assembly.UnsafeLoadFrom",
            "System.Reflection.Assembly.GetTypes",
            "System.Reflection.Assembly.GetType",
            "System.Type.GetType",
            "System.Activator.CreateInstance",
            "System.Activator.CreateInstanceFrom",
            "System.Reflection.MethodBase.Invoke",
            "System.Runtime.Loader.AssemblyLoadContext",
            "System.Reflection.Emit.",
            "System.Linq.Expressions.LambdaExpression.Compile",
            "System.Runtime.InteropServices.NativeLibrary.Load",
        ];

        return assembly.MemberReferences
            .Where(reference => forbidden.Any(token =>
                reference.StartsWith(token, StringComparison.Ordinal)))
            .Select(reference => $"{assembly.Name} calls {reference}");
    }

    /// <summary>
    /// B5b: no module initializers. Invariant 2 forbids a module-initializer ordering dependency
    /// by name, and this is not hypothetical: the aggregate repository suppresses CA2255
    /// repository-wide because seven legacy assemblies auto-register built-ins that way.
    /// </summary>
    internal static IEnumerable<string> B5b(AssemblyFacts assembly) =>
        assembly.CustomAttributeTypes
            .Where(static attribute => string.Equals(
                attribute,
                "System.Runtime.CompilerServices.ModuleInitializerAttribute",
                StringComparison.Ordinal))
            .Select(attribute => $"{assembly.Name} applies {attribute}");

    private static bool IsProfileShaped(string assemblyName) =>
        assemblyName.StartsWith("Broiler.VM.Profile.", StringComparison.Ordinal) ||
        string.Equals(assemblyName, "Broiler.VM.Fixtures", StringComparison.Ordinal);
}
