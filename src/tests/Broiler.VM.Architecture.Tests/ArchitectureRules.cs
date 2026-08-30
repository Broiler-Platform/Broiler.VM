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
        project.ProjectReferences.SelectMany(reference =>
            Escapes(reference, $"{project.RelativePath} -> "));

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
        project.SourceItemPaths.SelectMany(item =>
            Escapes(item, $"{project.RelativePath} includes "));

    /// <summary>
    /// Decides whether one declared path leaves the component, and refuses to answer "no" for a
    /// path it cannot resolve.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An earlier version of A1 and A3 skipped any path containing an MSBuild property, on the
    /// grounds that it could not be resolved to a real location. That is a hole rather than a
    /// simplification: `&lt;Compile Include="$(MSBuildThisFileDirectory)..\..\..\Broiler.JS\**\*.cs" /&gt;`
    /// is the shared source link roadmap section 9 forbids by name, and it would have cleared A3
    /// without being looked at.
    /// </para>
    /// <para>
    /// So an unresolvable path is a violation of its own kind, and a resolvable one is judged on
    /// where it lands. The strictness costs nothing today - no project in the component declares
    /// a property-bearing source item - and it means the rule can never clear something it did
    /// not understand.
    /// </para>
    /// </remarks>
    private static IEnumerable<string> Escapes(string path, string prefix)
    {
        if (path.Contains('$'))
        {
            // The property cannot be evaluated here, so the literal is judged instead: a path
            // whose parent-directory hops outrun its rooted segments leaves the component
            // wherever the property points.
            var segments = path.Split('/', '\\');
            var depth = 0;
            var escapes = false;

            foreach (var segment in segments)
            {
                if (segment == "..")
                {
                    depth--;
                    if (depth < 0)
                    {
                        escapes = true;
                    }
                }
                else if (segment.Length > 0 && segment != "." && !segment.Contains('$'))
                {
                    depth++;
                }
            }

            yield return escapes
                ? $"{prefix}{path} (unresolvable, and its parent-directory hops leave the component)"
                : $"{prefix}{path} (unresolvable path, cannot be cleared)";

            yield break;
        }

        if (!path.StartsWith(ComponentGraph.Root, StringComparison.OrdinalIgnoreCase))
        {
            yield return prefix + path;
        }
    }

    /// <summary>A4: no product project references a test-only project.</summary>
    /// <remarks>
    /// A composition root is exempt because it is not a product project - ADR 0001 revision 1 puts
    /// it in its own partition - and because the reference it needs is to a consumer profile, which
    /// lives at a test-only path for want of any other shape that fits. The exemption is not a hole:
    /// A12 states what a composition root may reference, and it forbids the fixture profile and
    /// every test project by name.
    /// </remarks>
    internal static IEnumerable<string> A4(ComponentGraph.ProjectFile project)
    {
        if (project.IsTestOnly || project.IsComposition)
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
    /// </summary>
    /// <remarks>
    /// <para>
    /// The allow-list was empty at VM-0 because the component declared no composition. VM-3 fills
    /// it with the two named roots under <c>src/compositions/</c>, and widens what counts as a
    /// profile assembly to the consumer profiles - so the rule now has real subjects on both sides
    /// rather than only a witness.
    /// </para>
    /// <para>
    /// The fixture profile is deliberately NOT in scope here. It is a profile by shape, but what
    /// keeps it out of a shipped image is its test-only path, which A4 and A5 already hold; folding
    /// it in would make every test project that uses a fixture a violation and force the allow-list
    /// to name them, which is the opposite of what this rule is for. A12 forbids the fixture inside
    /// a composition root, which is where it would actually do harm.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> A11(ComponentGraph.ProjectFile project)
    {
        if (project.IsComposition)
        {
            return [];
        }

        return project.ReferencedAssemblyNames
            .Where(IsComposableProfile)
            .Select(name => $"{project.RelativePath} -> {name}");
    }

    /// <summary>
    /// A12: a composition root references exactly the three core projects and one or more profile
    /// assemblies, and nothing else at all.
    /// </summary>
    /// <remarks>
    /// This is the project-file half of the exact-closure claim, and it is the half that can be
    /// checked without publishing anything. The published closure is the other half: a root whose
    /// reference set is clean here can still drag something in through a package, so both are
    /// asserted and neither is taken for the other.
    /// </remarks>
    internal static IEnumerable<string> A12(ComponentGraph.ProjectFile project)
    {
        var isCompositionShaped = project.IsComposition ||
            (project.IsWitness && project.AssemblyName.StartsWith("Broiler.VM.Composition.", StringComparison.Ordinal));

        if (!isCompositionShaped)
        {
            yield break;
        }

        var profiles = 0;

        foreach (var name in project.ReferencedAssemblyNames)
        {
            if (DeclaredPackageIds.Contains(name, StringComparer.Ordinal))
            {
                continue;
            }

            if (IsComposableProfile(name))
            {
                profiles++;
                continue;
            }

            yield return $"{project.RelativePath} -> {name}, which is neither a core package nor a composable profile";
        }

        if (profiles == 0)
        {
            yield return $"{project.RelativePath} composes no profile";
        }

        foreach (var package in project.PackageReferences)
        {
            yield return $"{project.RelativePath} declares PackageReference {package}";
        }
    }

    /// <summary>
    /// A13: a consumer profile's project references are exactly Abstractions and Binary, it
    /// declares no package reference, and it opens its internals to nobody.
    /// </summary>
    /// <remarks>
    /// ADR 0011's obligation P1 as a rule rather than as a sentence. The promise the milestone
    /// exists to demonstrate is that a profile is written against the public source contract and
    /// nothing else, and that promise is a property of the reference set: a profile that could name
    /// a runtime type would be relying on something no consumer outside this repository has.
    /// </remarks>
    internal static IEnumerable<string> A13(ComponentGraph.ProjectFile project)
    {
        var isProfileShaped = project.IsWitness
            ? IsComposableProfile(project.AssemblyName)
            : project.IsTestOnly && IsComposableProfile(project.AssemblyName);

        if (!isProfileShaped)
        {
            yield break;
        }

        var referenced = project.ReferencedAssemblyNames
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        if (!referenced.SequenceEqual(ConsumerProfileReferences, StringComparer.Ordinal))
        {
            yield return
                $"{project.RelativePath} references [{string.Join(", ", referenced)}] rather than " +
                $"[{string.Join(", ", ConsumerProfileReferences)}]";
        }

        foreach (var package in project.PackageReferences)
        {
            yield return $"{project.RelativePath} declares PackageReference {package}";
        }

        foreach (var target in project.InternalsVisibleTo)
        {
            yield return $"{project.RelativePath} opens internals to {target}";
        }
    }

    /// <summary>The exact reference set ADR 0011's obligation P1 allows a profile package.</summary>
    internal static readonly string[] ConsumerProfileReferences =
        ["Broiler.VM.Abstractions", "Broiler.VM.Binary"];

    /// <summary>
    /// Whether an assembly name is a profile a composition may name: a Broiler-owned language
    /// profile, or an application-local consumer profile under the documentation-reserved domain.
    /// </summary>
    /// <remarks>
    /// The consumer half is an enumeration of what this component actually contains rather than a
    /// general test for "someone's profile", because there is no general test: a profile is a
    /// profile by what it implements, and a project file does not say. Adding a third consumer
    /// profile therefore means adding it here, which is the review this list exists to force.
    /// </remarks>
    private static bool IsComposableProfile(string assemblyName) =>
        assemblyName.StartsWith("Broiler.VM.Profile.", StringComparison.Ordinal) ||
        assemblyName.StartsWith("Com.Example.", StringComparison.Ordinal);

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
                // A nested type is spelled Outer+Inner, so the leaf follows the last separator
                // of either kind.
                var simpleName = name[(name.LastIndexOfAny(['.', '+']) + 1)..];
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
        // Matched by exact member name rather than by prefix: a prefix test reads
        // System.Type.GetTypeFromHandle - which is ordinary typeof - as a call to
        // System.Type.GetType, and a rule that cries wolf gets suppressed.
        string[] forbiddenMembers =
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
            "System.Linq.Expressions.LambdaExpression.Compile",
            "System.Runtime.InteropServices.NativeLibrary.Load",
            "System.Runtime.InteropServices.NativeLibrary.TryLoad",
        ];

        // Whole namespaces and types where naming the thing at all is the violation.
        string[] forbiddenTypePrefixes =
        [
            "System.Reflection.Emit.",
            "System.Runtime.Loader.",
        ];

        var byMember = assembly.MemberReferences
            .Where(reference => forbiddenMembers.Contains(reference, StringComparer.Ordinal))
            .Select(reference => $"{assembly.Name} calls {reference}");

        // A type can be reached without any member reference - held in a field, named in a
        // signature - so the TypeRef table is swept as well as the MemberRef table.
        var byType = assembly.TypeReferences
            .Where(reference => forbiddenTypePrefixes.Any(prefix =>
                reference.StartsWith(prefix, StringComparison.Ordinal)))
            .Select(reference => $"{assembly.Name} names {reference}");

        return byMember.Concat(byType);
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

    /// <summary>
    /// Whether an assembly is profile-shaped for the purposes of A8: anything that implements the
    /// profile contract, whoever owns it and wherever it lives.
    /// </summary>
    /// <remarks>
    /// Wider than <see cref="IsComposableProfile"/>, and deliberately so. A8 says a profile never
    /// references the runtime, which is true of the fixture profile as much as of a consumer one -
    /// it is the claim that a verifier and an executor can be written against the contract alone.
    /// A11 is about what may be linked into a shipped image, where the fixture's test-only path is
    /// the mechanism that answers instead.
    /// </remarks>
    private static bool IsProfileShaped(string assemblyName) =>
        IsComposableProfile(assemblyName) ||
        string.Equals(assemblyName, "Broiler.VM.Fixtures", StringComparison.Ordinal);
}
