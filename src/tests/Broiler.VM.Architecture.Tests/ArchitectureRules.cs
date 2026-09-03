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
            .Where(name => !IsSameProfileFamily(project.AssemblyName, name))
            .Select(name => $"{project.RelativePath} -> {name}");
    }

    /// <summary>
    /// Whether two assembly names belong to the same profile family: the same language under
    /// <c>Broiler.VM.Profile.</c>, so that <c>Broiler.VM.Profile.JavaScript.Compiler</c> and
    /// <c>Broiler.VM.Profile.JavaScript.Format</c> are family and
    /// <c>Broiler.VM.Profile.WebAssembly</c> is not.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is A11's one exemption and it is narrow on purpose. ADR 0011 P1's 2026-08-31 revision
    /// states that a profile component's own siblings - its format assembly, its lowering - are
    /// not members of the Broiler.VM-owned reference set P1 bounds, and the JavaScript profile's
    /// roadmap section 5 makes the format assembly the PIVOT the executor and the lowering both
    /// depend on. Without the exemption that graph is illegal under a rule written before any
    /// product profile existed, and the profile could not reference its own bytecode format.
    /// </para>
    /// <para>
    /// The exemption is keyed on the LANGUAGE segment rather than on the
    /// <c>Broiler.VM.Profile.</c> prefix, which is the whole point: two profiles in one image are
    /// composed by a composition root and are never linked to each other, so a JavaScript project
    /// referencing a WebAssembly one is still a violation and rule N2 asserts that half by its
    /// own witness. A prefix-wide exemption would have dissolved exactly the boundary the
    /// extraction gate's fourth condition exists to keep.
    /// </para>
    /// </remarks>
    internal static bool IsSameProfileFamily(string left, string right)
    {
        var leftFamily = ProfileFamily(left);

        return leftFamily is not null &&
            string.Equals(leftFamily, ProfileFamily(right), StringComparison.Ordinal);
    }

    /// <summary>
    /// The <c>Broiler.VM.Profile.&lt;Language&gt;</c> prefix of an assembly name, or null when the
    /// name is not a Broiler-owned profile assembly at all.
    /// </summary>
    internal static string? ProfileFamily(string assemblyName)
    {
        const string Prefix = "Broiler.VM.Profile.";

        if (!assemblyName.StartsWith(Prefix, StringComparison.Ordinal))
        {
            return null;
        }

        var language = assemblyName[Prefix.Length..].Split('.')[0];

        return language.Length == 0 ? null : Prefix + language;
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

    // ---- Group N: the JavaScript profile family -----------------------------------------------
    //
    // Group N holds the shape of the Broiler.VM.Profile.JavaScript family's project graph. The
    // letter is chosen because A-E, V, F, M, G, P, S and T are recorded as in use, H replaces the
    // reverted group R, J is the assurance group, K the composition register, L the baselines and
    // M the API baseline; R1..R6 stays the reserved namespace ADR 0003 records for ADR 0012's six
    // ownership roles.
    //
    // The rules live in this register rather than in one of the profile's own because the profile
    // is a set of product projects INSIDE this component - decision JSD-0001 - so there is one
    // graph, one manifest and one register to hold it. The JavaScript roadmap's JS-0 exit gate
    // asks for the profile's own rule register; decision JSD-0006 records the adoption of this one
    // as a deviation with its reason, rather than standing up a second register over one graph.
    //
    // Every N rule is over PROJECT FILES. The profile assemblies are deliberately absent from the
    // architecture test project's reference set - A11 forbids it - so no N rule can read compiled
    // metadata, and none pretends to. The metadata half arrives with the composition roots at
    // JS-1, which is where a published closure exists to read.

    /// <summary>The exact Broiler.VM-owned reference set the JavaScript profile assembly may have.</summary>
    /// <remarks>
    /// ADR 0011 P1, whose 2026-08-31 revision states that the set is of Broiler.VM-OWNED
    /// assemblies and that a profile's own siblings are not members of it. The format assembly is
    /// therefore admitted by <see cref="N1"/> explicitly rather than by adding it to this list,
    /// because the two admissions have different authorities and folding them together would make
    /// one unreadable from the other.
    /// </remarks>
    internal static readonly string[] JavaScriptProfileCoreReferences =
        ["Broiler.VM.Abstractions", "Broiler.VM.Binary"];

    /// <summary>The format sibling both the profile and the lowering are permitted to reference.</summary>
    internal const string JavaScriptFormatAssembly = "Broiler.VM.Profile.JavaScript.Format";

    /// <summary>The lowering, which the profile assembly may never reference.</summary>
    internal const string JavaScriptCompilerAssembly = "Broiler.VM.Profile.JavaScript.Compiler";

    /// <summary>The profile assembly itself.</summary>
    internal const string JavaScriptProfileAssembly = "Broiler.VM.Profile.JavaScript";

    /// <summary>
    /// N1: the JavaScript profile assembly references exactly Abstractions, Binary and its own
    /// format sibling; it never references the runtime and never references the lowering; it
    /// declares no PackageReference and opens its internals to nobody.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Four independent claims, and the roadmap makes each of them load-bearing for a different
    /// reason. The runtime exclusion is ADR 0011 P1. The lowering exclusion is what makes an
    /// execution-only composition contain a format, a verifier and an interpreter and no compiler
    /// at all - a deployment property, not a build switch. The package exclusion keeps the
    /// published closure readable off the project file. The internals exclusion is P2: a profile
    /// is written against the public source contract or the contract is not what it claims.
    /// </para>
    /// <para>
    /// Rule A13 does not reach this project, and widening it would have been the wrong repair.
    /// A13's subject is a test-only profile, which is what the two consumer profiles are, and it
    /// states a reference set of exactly two assemblies; a product profile with a format sibling
    /// has three, so widening A13 would have weakened the rule that holds the consumer profiles.
    /// N1 states the stronger claim over its own subject instead.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> N1(ComponentGraph.ProjectFile project)
    {
        if (!string.Equals(project.AssemblyName, JavaScriptProfileAssembly, StringComparison.Ordinal))
        {
            yield break;
        }

        var expected = JavaScriptProfileCoreReferences
            .Append(JavaScriptFormatAssembly)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        var referenced = project.ReferencedAssemblyNames
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        if (!referenced.SequenceEqual(expected, StringComparer.Ordinal))
        {
            yield return
                $"{project.RelativePath} references [{string.Join(", ", referenced)}] rather than " +
                $"[{string.Join(", ", expected)}]";
        }

        // Named separately from the set comparison above even though the set implies it. A message
        // naming the lowering is what a reader needs to see when the execution-only property is
        // the thing that broke, and a single "the set differs" line would report the most
        // consequential violation in this component in the same words as a typo.
        if (referenced.Contains(JavaScriptCompilerAssembly, StringComparer.Ordinal))
        {
            yield return
                $"{project.RelativePath} references {JavaScriptCompilerAssembly}, so an " +
                "execution-only composition would carry a lowering";
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

    /// <summary>
    /// N2: no project in one profile family references a project in another, in either direction.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The JavaScript roadmap states this as a rule separate from the reference-set clause, and
    /// says why: the reference-set clause already tolerates one further Broiler.VM-named assembly,
    /// this profile's own format, so it cannot also carry the cross-profile prohibition. Two
    /// profiles in one browser image are composed by a composition root; they are not linked to
    /// each other, and the extraction gate's fourth condition IS this property.
    /// </para>
    /// <para>
    /// Both halves are swept in one rule because both are the same edge seen from its two ends,
    /// and a rule that checked only the outbound half would be satisfied from the side that never
    /// changes. A composition root is exempt on both halves - composing two profiles is what a
    /// composition root is for - and A12 is what bounds a root's reference set instead.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> N2(ComponentGraph.ProjectFile project)
    {
        if (project.IsComposition)
        {
            yield break;
        }

        var family = ProfileFamily(project.AssemblyName);

        foreach (var name in project.ReferencedAssemblyNames)
        {
            var target = ProfileFamily(name);

            if (target is null)
            {
                continue;
            }

            if (family is null)
            {
                yield return
                    $"{project.RelativePath} is outside every profile family and references {name}";
                continue;
            }

            if (!string.Equals(family, target, StringComparison.Ordinal))
            {
                yield return $"{project.RelativePath} ({family}) -> {name} ({target})";
            }
        }
    }

    /// <summary>
    /// N3: the JavaScript format assembly references nothing at all - not the core, not the
    /// profile, not the lowering.
    /// </summary>
    /// <remarks>
    /// The format is the pivot. The executor and the lowering must agree on the bytecode and
    /// neither may depend on the other, so both reference the format and the format references
    /// neither; a single edge out of it would put one of its two consumers on the other's
    /// dependency graph and the pivot would stop being one. It is a sink, so this half of the
    /// profile's subgraph is acyclic by construction rather than by inspection - the same argument
    /// ADR 0001 makes for Abstractions and Binary.
    /// </remarks>
    internal static IEnumerable<string> N3(ComponentGraph.ProjectFile project) =>
        string.Equals(project.AssemblyName, JavaScriptFormatAssembly, StringComparison.Ordinal)
            ? project.ReferencedAssemblyNames.Select(name => $"{project.RelativePath} -> {name}")
            : [];

    /// <summary>
    /// N4: no project in the JavaScript profile family declares a PackageId, and every one carries
    /// the literal element <c>IsPackable false</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The status ledger's standing claim is that no composition is advertised, none is packable
    /// and no runtime identifier is claimed. Packaging is JS-10's decision, and until it is taken
    /// the honest state is that these assemblies ship nowhere. Without this rule the claim decays
    /// silently: the vendored packaging props default <c>IsPackable</c> to true for any project
    /// whose name matches none of their test-and-tooling suffixes, which none of these does, so a
    /// family project that merely forgot the element would pack under its assembly name.
    /// </para>
    /// <para>
    /// The literal element is asserted rather than the evaluated property, which is rule A5's
    /// discipline applied to a second partition and for the same reason: an evaluated property can
    /// be true because of an import a reader of the project file cannot see.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> N4(ComponentGraph.ProjectFile project)
    {
        if (ProfileFamily(project.AssemblyName) is null)
        {
            yield break;
        }

        if (project.PackageId is not null)
        {
            yield return $"{project.RelativePath} declares PackageId {project.PackageId}";
        }

        if (!project.RawText.Contains("<IsPackable>false</IsPackable>", StringComparison.Ordinal))
        {
            yield return $"{project.RelativePath} does not carry the literal <IsPackable>false</IsPackable>";
        }
    }

    // ---- Group N, second half: the published diagnostic registry ------------------------------
    //
    // JS-3a publishes the diagnostic-code registry and the position encoding, and the four rules
    // below are what "published and bound in both directions" means mechanically. Each binds the
    // registry to a DIFFERENT independently written artefact - the enum, the emission sites, the
    // retained corpus, the composition's restated constants - so that the registry cannot be made
    // to agree with everything by being edited to match one thing.
    //
    // Every one of them is over TEXT parsed with Roslyn or over a manifest read off disk, for the
    // same reason the first half of group N is over project files: rule A11 keeps the profile
    // assembly out of this project's reference set, so there is no metadata to read and none of
    // these rules pretends there is.

    /// <summary>
    /// N5: the registry and the code vocabularies are the same set, half by half. Every member of
    /// <c>JavaScriptDiagnosticCode</c> has exactly one <c>core-result</c> row carrying its name and
    /// its number, every member of <c>SliceSourceDiagnosticCode</c> has exactly one
    /// <c>embedder-seam</c> row, every row names a member of the vocabulary its half declares, no
    /// number appears twice across both halves, the registry states its own revision, and every
    /// row's <c>since</c> is a revision that exists.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The revision half is not bookkeeping. A retained corpus entry records a diagnostic code and
    /// nothing else about it, so a code that changed meaning between two releases would silently
    /// invalidate every entry that recorded it - and the only thing that lets a reader date an
    /// entry is the registry stating which revision it is and each row stating the revision its
    /// meaning dates from.
    /// </para>
    /// <para>
    /// <b>Two vocabularies rather than one, since revision 2.</b> They are declared in two
    /// assemblies that do not reference each other, so neither compiler can see the other's
    /// numbers and a number used in both would be a defect nothing in the build could notice. This
    /// rule is the only reader of both, which is why the duplicate check sweeps the whole registry
    /// while the membership check is per half: a row in the wrong half names a member of the wrong
    /// vocabulary, and that has to fail rather than pass by finding the name somewhere.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> N5(
        IReadOnlyList<DiagnosticRegistryRow> registry,
        IReadOnlyList<(string Name, int Value)> vocabulary,
        IReadOnlyList<(string Name, int Value)> seamVocabulary,
        int revision)
    {
        if (revision < 1)
        {
            yield return "the registry states no revision of its own";
        }

        foreach (var duplicate in registry
            .GroupBy(static row => row.Code)
            .Where(static group => group.Count() > 1))
        {
            yield return $"the registry has {duplicate.Count()} rows for code {duplicate.Key}";
        }

        foreach (var row in registry)
        {
            var seam = string.Equals(row.Half, "embedder-seam", StringComparison.Ordinal);
            var declared = seam ? seamVocabulary : vocabulary;
            var stages = seam ? DiagnosticRegistry.SeamStages : DiagnosticRegistry.Stages;

            if (!declared.Any(member => member.Name == row.Name && member.Value == row.Code))
            {
                yield return
                    $"registry row {row.Code} names {row.Name}, which is not a member of that " +
                    $"number in the {row.Half} code vocabulary";
            }

            if (row.Since < 1 || row.Since > revision)
            {
                yield return
                    $"registry row {row.Code} dates from revision {row.Since}, and the registry " +
                    $"is at revision {revision}";
            }

            if (!DiagnosticRegistry.Halves.Contains(row.Half, StringComparer.Ordinal))
            {
                yield return $"registry row {row.Code} claims the half {row.Half}, which is not one";
            }
            else if (!stages.Contains(row.Stage, StringComparer.Ordinal))
            {
                yield return
                    $"registry row {row.Code} names the stage {row.Stage}, which is not one the " +
                    $"{row.Half} half has";
            }
        }

        foreach (var member in vocabulary)
        {
            if (!registry.Any(row => row.Code == member.Value && row.Name == member.Name &&
                string.Equals(row.Half, "core-result", StringComparison.Ordinal)))
            {
                yield return
                    $"the code vocabulary declares {member.Name} = {member.Value} and the " +
                    "registry has no core-result row for it";
            }
        }

        foreach (var member in seamVocabulary)
        {
            if (!registry.Any(row => row.Code == member.Value && row.Name == member.Name &&
                string.Equals(row.Half, "embedder-seam", StringComparison.Ordinal)))
            {
                yield return
                    $"the seam code vocabulary declares {member.Name} = {member.Value} and the " +
                    "registry has no embedder-seam row for it";
            }
        }
    }

    /// <summary>
    /// N6: every code maps onto exactly one core reason, and it is the reason its emission sites
    /// actually carry. No code is emitted with two reasons, no registry row names a reason that is
    /// not a member of the core's own vocabulary, and no declared code is emitted by nothing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// "Exactly one reason" is the clause that makes a code mean something. A code emitted as
    /// <c>InconsistentStructure</c> in one place and <c>SemanticValidationFailed</c> in another is
    /// two rejections sharing a number, and every corpus entry that recorded it recorded a triple
    /// that does not identify what happened.
    /// </para>
    /// <para>
    /// The reason names are held to <see cref="VmReason"/> itself rather than to a list here,
    /// which is what "with no invented or aliased reason" requires: a registry naming a reason the
    /// core does not have would otherwise pass every other check in this file.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> N6(
        IReadOnlyList<DiagnosticRegistryRow> registry,
        IReadOnlyList<DiagnosticRegistry.EmissionSite> sites,
        IReadOnlyList<(string Name, int Value)> vocabulary)
    {
        var reasons = Enum.GetNames<VmReason>();

        foreach (var row in registry)
        {
            // An embedder-seam row carries no reason and must not pretend to. Its rejection never
            // reaches a core result, so there is no envelope for a reason to travel in; a row that
            // named one would be claiming a transport the code does not use, and a reader tracing
            // the code into a VmReason would find nothing there.
            if (string.Equals(row.Half, "embedder-seam", StringComparison.Ordinal))
            {
                if (!string.Equals(row.Reason, "-", StringComparison.Ordinal))
                {
                    yield return
                        $"registry row {row.Code} is an embedder-seam row naming the reason " +
                        $"{row.Reason}, and a rejection of source reaches no core result";
                }

                continue;
            }

            if (!reasons.Contains(row.Reason, StringComparer.Ordinal))
            {
                yield return
                    $"registry row {row.Code} names the reason {row.Reason}, which the core does " +
                    "not have";
            }
        }

        foreach (var group in sites.GroupBy(static site => site.Code, StringComparer.Ordinal))
        {
            var carried = group
                .Select(static site => site.Reason)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(static reason => reason, StringComparer.Ordinal)
                .ToArray();

            if (carried.Length > 1)
            {
                yield return
                    $"{group.Key} is emitted with {carried.Length} reasons " +
                    $"[{string.Join(", ", carried)}], and a code carries one";
            }

            var row = registry.FirstOrDefault(candidate => candidate.Name == group.Key);

            if (row is null)
            {
                yield return $"{group.Key} is emitted and the registry has no row for it";
                continue;
            }

            foreach (var site in group.Where(site =>
                !string.Equals(site.Reason, row.Reason, StringComparison.Ordinal)))
            {
                yield return
                    $"{site.File}({site.Line}) emits {site.Code} with {site.Reason}, and the " +
                    $"registry says {row.Reason}";
            }
        }

        foreach (var member in vocabulary)
        {
            if (!sites.Any(site => string.Equals(site.Code, member.Name, StringComparison.Ordinal)))
            {
                yield return
                    $"{member.Name} is declared and no site emits it, so nothing can reach it";
            }
        }
    }

    /// <summary>
    /// The registry rows no artifact reaches, and the only ones permitted to claim it.
    /// </summary>
    /// <remarks>
    /// <b>The list is here rather than in the registry, and that is the whole point of it.</b> A
    /// row claiming to be unreachable is a row excused from the backward binding, so if the
    /// registry alone decided which rows may claim it, the excuse would be available by editing
    /// the file the rule reads. Adding a fourth is an edit to this test, which is a review.
    /// </remarks>
    internal static readonly int[] DefensiveCodes = [1003, 1006, 1903, 2303];

    /// <summary>
    /// N7: every registry row is reachable from a named case. A <c>corpus</c> row names an entry
    /// of the retained corpus manifest that records that exact code; a <c>defensive</c> row is one
    /// of <see cref="DefensiveCodes"/> and states why no artifact reaches it.
    /// </summary>
    /// <remarks>
    /// This is the backward half of the binding and the expensive one to satisfy honestly: a
    /// registry can always be made to agree with the enum, and it takes a corpus to show that the
    /// codes are reachable at all. Three rows are not, each for a reason that is a fact about the
    /// build rather than a gap - two because the core screens the descriptor before this profile
    /// is called, one because the reader's status set is exhausted by the arms above it - and each
    /// states its reason where a reader meets the row.
    /// </remarks>
    internal static IEnumerable<string> N7(
        IReadOnlyList<DiagnosticRegistryRow> registry,
        ILookup<int, string> corpus,
        ILookup<string, string> sourceCorpus)
    {
        foreach (var row in registry)
        {
            if (!DiagnosticRegistry.Reachabilities.Contains(row.Reachability, StringComparer.Ordinal))
            {
                yield return
                    $"registry row {row.Code} claims the reachability {row.Reachability}, which " +
                    "is not one";

                continue;
            }

            // A `source` row is reached by a refused SOURCE and never by an artifact, which is the
            // whole of the boundary decision expressed as a binding: the case it names is an entry
            // of the retained source corpus, and looking for it in the artifact corpus would be
            // looking for bytes the refusal is defined by never having produced.
            if (string.Equals(row.Reachability, "source", StringComparison.Ordinal))
            {
                if (!string.Equals(row.Half, "embedder-seam", StringComparison.Ordinal))
                {
                    yield return
                        $"registry row {row.Code} is reached by a source and is not an " +
                        "embedder-seam row, so it claims to travel in a core result it never reaches";
                }

                if (!sourceCorpus[row.Name].Contains(row.Case, StringComparer.Ordinal))
                {
                    yield return
                        $"registry row {row.Code} names the case {row.Case}, and no retained " +
                        $"source entry of that name is refused with {row.Name}";
                }

                continue;
            }

            if (string.Equals(row.Reachability, "defensive", StringComparison.Ordinal))
            {
                if (!DefensiveCodes.Contains(row.Code))
                {
                    yield return
                        $"registry row {row.Code} claims to be unreachable and is not one of the " +
                        "rows this rule admits as unreachable";
                }

                if (row.Case.Trim().Length < 20)
                {
                    yield return
                        $"registry row {row.Code} claims to be unreachable and states no reason";
                }

                continue;
            }

            if (!corpus[row.Code].Contains(row.Case, StringComparer.Ordinal))
            {
                yield return
                    $"registry row {row.Code} names the case {row.Case}, and no corpus entry of " +
                    $"that name records code {row.Code}";
            }
        }

        foreach (var code in DefensiveCodes)
        {
            var row = registry.FirstOrDefault(candidate => candidate.Code == code);

            if (row is not null &&
                !string.Equals(row.Reachability, "defensive", StringComparison.Ordinal))
            {
                yield return
                    $"registry row {code} is admitted as unreachable and claims to be reachable " +
                    "by a case, which is a stronger claim than this rule was told to expect";
            }
        }
    }

    /// <summary>
    /// N8: the codes a composition restates agree with the registry, name for name and number for
    /// number.
    /// </summary>
    /// <remarks>
    /// The corpus producer writes its expected codes out rather than reading them from the profile
    /// it is testing, so that a renumbering moves one side and not both. That duplication is only
    /// worth its cost while something holds the two halves to a third thing, and this is it: the
    /// registry is the third thing, and neither half is the other's authority.
    /// </remarks>
    internal static IEnumerable<string> N8(
        IReadOnlyList<DiagnosticRegistryRow> registry,
        IReadOnlyList<(string Name, int Value)> mirror)
    {
        foreach (var restated in mirror)
        {
            var row = registry.FirstOrDefault(candidate => candidate.Name == restated.Name);

            if (row is null)
            {
                yield return
                    $"the composition restates {restated.Name} = {restated.Value}, and the " +
                    "registry has no row of that name";

                continue;
            }

            if (row.Code != restated.Value)
            {
                yield return
                    $"the composition restates {restated.Name} = {restated.Value}, and the " +
                    $"registry publishes it as {row.Code}";
            }
        }
    }

    /// <summary>
    /// N9: a core position is constructed in the file that decides the encoding and nowhere else.
    /// </summary>
    /// <remarks>
    /// The core's position record carries two fields whose meaning it does not interpret, which is
    /// exactly the shape in which one component builds two conventions against one struct without
    /// noticing. JSD-0009 writes the convention down; this keeps every position in the assembly
    /// going through the two factories that implement it, so a call site cannot quietly answer
    /// with a third shape.
    /// </remarks>
    internal static IEnumerable<string> N9(
        IReadOnlyList<DiagnosticRegistry.PositionProducer> producers,
        IReadOnlyList<string> namedConstructions)
    {
        foreach (var producer in producers.Where(static producer => producer.Constructs))
        {
            if (!string.Equals(producer.File, DiagnosticRegistry.PositionPath, StringComparison.Ordinal))
            {
                yield return
                    $"{producer.File} builds a {DiagnosticRegistry.PositionType} in " +
                    $"{producer.Member}, and the encoding is decided in " +
                    $"{DiagnosticRegistry.PositionPath}";
            }
        }

        foreach (var construction in namedConstructions.Where(construction =>
            !construction.StartsWith(DiagnosticRegistry.PositionPath, StringComparison.Ordinal)))
        {
            yield return
                $"{construction} names {DiagnosticRegistry.PositionType} in a construction outside " +
                $"{DiagnosticRegistry.PositionPath}";
        }

        if (!producers.Any(static producer =>
            producer.Constructs &&
            string.Equals(producer.File, DiagnosticRegistry.PositionPath, StringComparison.Ordinal)))
        {
            yield return
                $"{DiagnosticRegistry.PositionPath} builds no position at all, so this rule is " +
                "quantifying over nothing";
        }
    }

    /// <summary>
    /// N11: every budget dimension the profile can answer a resource exhaustion on is pinned by a
    /// corpus entry that records the dimension and the scope the answer named, and every pair a
    /// corpus entry records is one the profile can answer and names members of the core's own two
    /// enumerations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is N7's clause for the answers N7 cannot reach.</b> The registry's both-directions
    /// binding is keyed on a diagnostic code, and a resource exhaustion carries none: the category
    /// and the reason are all a corpus row would otherwise hold, and they are the same two values
    /// for a section-count ceiling and a structural-depth one. The dimension and the scope are
    /// what tell them apart, so they are what a corpus entry has to record and this is what holds
    /// the corpus to the verifier that produces them.
    /// </para>
    /// <para>
    /// <b>Both directions, and they fail differently.</b> A dimension the profile answers and no
    /// entry pins is an arm nothing exercises - the defect this rule was written for. A dimension
    /// an entry pins and the profile never answers is a row recording an answer this profile
    /// cannot give, which is either a stale entry or a claim about the core that belongs in the
    /// core's own suite.
    /// </para>
    /// <para>
    /// <b>One asymmetry is deliberate and is not a violation.</b> A site's scope is the scope the
    /// verifier can attribute unaided; where the answer is charged through the meter, the meter
    /// reports the level that actually refused and the observed scope is that one instead. So the
    /// scopes are held to the core's vocabulary rather than to the site, and the entry records
    /// what was observed.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> N11(
        IReadOnlyList<DiagnosticRegistry.ExhaustionAnswer> answers,
        IReadOnlyList<DiagnosticRegistry.CorpusOutcome> corpus,
        IReadOnlyCollection<string> dimensions,
        IReadOnlyCollection<string> scopes)
    {
        var exhaustions = corpus
            .Where(static row => string.Equals(row.Outcome, "ResourceExhaustion", StringComparison.Ordinal))
            .ToArray();

        foreach (var answer in answers)
        {
            if (!dimensions.Contains(answer.Dimension, StringComparer.Ordinal))
            {
                yield return
                    $"{answer.File}:{answer.Line} answers with {answer.Dimension}, which is not a " +
                    $"member of {DiagnosticRegistry.DimensionType}";

                continue;
            }

            if (!exhaustions.Any(row => string.Equals(row.Dimension, answer.Dimension, StringComparison.Ordinal)))
            {
                yield return
                    $"the profile answers a resource exhaustion on {answer.Dimension} at " +
                    $"{answer.File}:{answer.Line}, and no corpus entry pins that dimension";
            }
        }

        foreach (var row in exhaustions)
        {
            if (!dimensions.Contains(row.Dimension, StringComparer.Ordinal))
            {
                yield return
                    $"corpus entry {row.Name} records the dimension {row.Dimension}, which is not " +
                    $"a member of {DiagnosticRegistry.DimensionType}";
            }
            else if (!answers.Any(answer =>
                string.Equals(answer.Dimension, row.Dimension, StringComparison.Ordinal)))
            {
                yield return
                    $"corpus entry {row.Name} records the dimension {row.Dimension}, and no site " +
                    "in the profile answers on it";
            }

            if (!scopes.Contains(row.Scope, StringComparer.Ordinal))
            {
                yield return
                    $"corpus entry {row.Name} records the scope {row.Scope}, which is not a " +
                    $"member of {DiagnosticRegistry.ScopeType}";
            }
        }

        // And the vacuity clause. A profile that answers no exhaustion at all, or a manifest with
        // no exhaustion row, would satisfy every loop above by quantifying over nothing - which is
        // exactly the state this rule exists to detect the return of.
        if (answers.Count == 0)
        {
            yield return "the profile answers no resource exhaustion at all, so this rule is quantifying over nothing";
        }

        if (exhaustions.Length == 0)
        {
            yield return "the corpus pins no exhaustion at all, so this rule is quantifying over nothing";
        }
    }

    /// <summary>
    /// N12: the lowering assembly holds no state that outlives a call. No mutable static field, no
    /// settable static property, no <c>[ThreadStatic]</c>, and no <c>AsyncLocal</c> or
    /// <c>ThreadLocal</c> anywhere in it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is roadmap section 9's parse-options answer, held structurally.</b> The seed reads
    /// its two most consequential grammar switches - the module-versus-script goal and the
    /// top-level-await permission - out of ambient async-local state in a different assembly, and
    /// section 9 rejects that for three separate reasons: it is a hidden dependency across a
    /// boundary the fork removes, it makes two concurrent parses with different goals mutually
    /// corrupting, and ambient per-thread state in a profile is the shape the core's lifecycle
    /// rules exist to keep out. The replacement is an options value passed in.
    /// </para>
    /// <para>
    /// <b>Section 9 states the gate as a runtime test, and that test is the weaker half.</b> Two
    /// parses with different goals running concurrently, each goal-appropriate, failing when the
    /// options become a shared static - the producer composition runs exactly that. But it can only
    /// fail over a static those two parses reach, and a switch moved into a third construct nobody
    /// wrote a concurrent case for would leave it green. This rule has no such gap: its subject is
    /// every declaration in the assembly, so a hiding place has to not exist rather than not be
    /// looked in.
    /// </para>
    /// <para>
    /// <b>A readonly static is not its subject.</b> The tokenizer's punctuator table and the
    /// validator's reserved-name list are static and neither can carry anything out of a parse,
    /// because nothing can write them. Reporting those would make this a rule about a keyword.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> N12(IReadOnlyList<AmbientStateSite> sites, int filesScanned)
    {
        foreach (var site in sites)
        {
            yield return
                $"{site.File} declares {site.Member}, which is {site.Kind}; a parse's state may " +
                "not outlive the call it belongs to";
        }

        // The vacuity clause, and it is not a formality here: this rule passes by finding nothing,
        // so a run over an empty file list would be the strongest-looking clean result in the
        // register and would mean that the assembly had been renamed.
        if (filesScanned == 0)
        {
            yield return
                "no file of the lowering assembly was scanned, so this rule is quantifying over " +
                "nothing";
        }
    }

    /// <summary>
    /// N13: the conformance harness's ingestion path is never advertised, and that is asserted
    /// rather than assumed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>What the scan asserts is deliberately not "no published closure".</b> The harness is a
    /// composition root - roadmap section 5 explains why it can be nothing else - and a root
    /// publishes a closure of its own for its own evidence, so a rule phrased as "appears in no
    /// published closure" would be falsified by the harness's own publish. The property that is
    /// actually wanted is that it appears in <b>no package and in no advertised composition's
    /// closure</b>, and that no other project reaches it at all. Correction JSC-40 records the
    /// distinction.
    /// </para>
    /// <para>
    /// <b>Six independent clauses, because each is a different way the path could ship.</b> A
    /// project reference is the one that would actually happen - the execution-only root wanting
    /// "just the fixtures" is the plausible mistake, and it is the direction the negative control
    /// takes. A package identity ships it to a consumer directly, and a missing non-packable
    /// declaration is the same thing one edit away. An advertised register row makes
    /// it something a consumer may depend on. A closure report naming it says a published image
    /// already contains it. And a project file naming the suite directory carries suite FILES into
    /// an assembly's output without any reference changing, which is the one that leaves the
    /// dependency graph looking clean.
    /// </para>
    /// <para>
    /// <b>The suite clause is what the attribution obligation hangs on.</b> A third-party suite is
    /// separately licensed material, and the moment a suite file is copied into a build output it
    /// is being redistributed. Today no suite in this checkout is third-party, so the clause
    /// guards a path rather than a body of code - which is the right time to write it, because
    /// the change that first ingests one is the change that must not also have to invent this
    /// rule.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> N13(
        string harness,
        IReadOnlyList<ComponentGraph.ProjectFile> projects,
        IReadOnlyList<CompositionRules.Row> rows,
        IReadOnlyList<(string Composition, CompositionRules.ClosureMode Mode)> closures,
        IReadOnlyList<string> suiteDirectories)
    {
        var harnessProjects = projects
            .Where(project => string.Equals(project.AssemblyName, harness, StringComparison.Ordinal))
            .ToArray();

        // The vacuity clause first. This rule passes by finding nothing, so a run in which the
        // harness has been renamed out from under it would be the cleanest-looking row in the
        // register while asserting nothing at all.
        if (harnessProjects.Length != 1)
        {
            yield return
                $"{harnessProjects.Length} projects in the checkout build {harness}, not exactly " +
                "one: this rule is quantifying over nothing";

            yield break;
        }

        var harnessProject = harnessProjects[0];

        foreach (var project in projects.Where(candidate =>
                     !string.Equals(candidate.AssemblyName, harness, StringComparison.Ordinal) &&
                     candidate.ReferencedAssemblyNames.Contains(harness, StringComparer.Ordinal)))
        {
            yield return
                $"{project.AssemblyName} references {harness}: the conformance harness's " +
                "ingestion path may be reached from nothing";
        }

        if (harnessProject.PackageId is not null)
        {
            yield return $"{harness} declares the package identity {harnessProject.PackageId}";
        }

        if (!harnessProject.RawText.Contains("<IsPackable>false</IsPackable>", StringComparison.Ordinal))
        {
            yield return $"{harness} does not carry the literal element IsPackable false";
        }

        foreach (var row in rows.Where(row =>
                     string.Equals(row.Kind, "advertised", StringComparison.Ordinal)))
        {
            if (string.Equals(row.Composition, harness, StringComparison.Ordinal))
            {
                yield return $"{harness} is registered as an advertised composition";
            }

            if (row.ProfileAssemblies.Concat(row.Siblings).Contains(harness, StringComparer.Ordinal))
            {
                yield return
                    $"the advertised composition {row.Composition} declares {harness} in its closure";
            }
        }

        foreach (var (composition, mode) in closures.Where(closure =>
                     !string.Equals(closure.Composition, harness, StringComparison.Ordinal) &&
                     closure.Mode.Assemblies.Contains(harness, StringComparer.Ordinal)))
        {
            yield return $"{composition} [{mode.Name}] ships {harness}";
        }

        foreach (var project in projects)
        {
            // Separator-insensitively, because a project file writes Windows separators and the
            // fragment this rule is given is a path. A clause that compared the two spellings
            // literally would be defeated by the spelling MSBuild actually uses.
            var text = project.RawText.Replace('\\', '/');

            foreach (var directory in suiteDirectories.Where(directory =>
                         text.Contains(directory, StringComparison.Ordinal)))
            {
                yield return
                    $"{project.AssemblyName} names the suite directory {directory} in its project " +
                    "file: a suite file carried into a build output is a suite file redistributed";
            }
        }
    }

    /// <summary>
    /// N14: the pinned language edition says the same thing in the code, its decision record and
    /// the ledger, and the code's own account of whether it is archived binds the ledger's.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A pin recorded in one place is a pin one edit can move.</b> The profile declares the
    /// edition, the revision and the digest as constants; a decision record states them in prose
    /// and argues for them; the ledger's open-dependency row is where a reader learns the pin's
    /// state. Three independently written places is the same discipline rules N5 through N9 apply
    /// to the diagnostic registry, and for the same reason: agreement is only evidence when
    /// disagreement was possible.
    /// </para>
    /// <para>
    /// <b>The clause that matters most is the last one, because it guards the overclaim.</b>
    /// Retrieving, hashing and archiving a third-party document is a human action and only two of
    /// the three have been performed, so roadmap section 24 makes the pin PROVISIONAL and requires
    /// the ledger to carry a named exclusion. The code says which state it is in with a boolean.
    /// If that boolean is flipped without the ledger's exclusion being resolved - or resolved
    /// without the boolean being flipped - the component would be claiming a fully taken pin in
    /// one document and a provisional one in another, and the direction that hides is the one
    /// where the code claims more.
    /// </para>
    /// <para>
    /// <b>Over source text rather than over the loaded constants</b>, because rule A11 keeps the
    /// profile assembly out of this test project's reference set. The same parser the registry
    /// rules use reads the declarations, so this rule sees what a reader of the file sees.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> N14(
        IReadOnlyDictionary<string, string> declared,
        string decisionRecord,
        string ledger)
    {
        string[] required = ["Year", "Revision", "DocumentDigest", "Archived"];
        var missing = required.Where(name => !declared.ContainsKey(name)).ToArray();

        // The vacuity clause first, for the reason N13's carries one: a rule that passes by
        // finding nothing would be the cleanest row in the register the day its subject was
        // renamed out from under it.
        if (missing.Length != 0)
        {
            yield return
                $"the pinned-edition declaration names {declared.Count} constants and not " +
                $"[{string.Join(", ", missing)}]: this rule is quantifying over nothing";

            yield break;
        }

        foreach (var name in new[] { "Year", "Revision", "DocumentDigest" })
        {
            if (!decisionRecord.Contains(declared[name], StringComparison.Ordinal))
            {
                yield return
                    $"the decision record does not name the declared {name} `{declared[name]}`";
            }
        }

        if (!ledger.Contains(declared["Revision"], StringComparison.Ordinal))
        {
            yield return
                $"the ledger does not name the declared revision `{declared["Revision"]}`, so a " +
                "reader of the open-dependency row cannot tell which pin it is describing";
        }

        var archived = string.Equals(declared["Archived"], "true", StringComparison.Ordinal);
        var ledgerSaysProvisional = ledger.Contains("provisional", StringComparison.OrdinalIgnoreCase);

        if (!archived && !ledgerSaysProvisional)
        {
            yield return
                "the declaration says the document is not archived and the ledger calls the pin " +
                "nothing of the sort: an unarchived pin carries a named exclusion";
        }

        if (archived && ledgerSaysProvisional)
        {
            yield return
                "the declaration says the document is archived and the ledger still calls the pin " +
                "provisional: one of the two is describing a state that has passed";
        }
    }

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
