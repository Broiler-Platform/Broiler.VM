namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group K: the composition-governance rules. Each one is a pure function over inputs a test
/// supplies, so it can be shown rejecting a violating one without editing the checkout.
/// </summary>
/// <remarks>
/// <para>
/// The group exists because VM-3's exit gate asks CI to detect five things - duplicate or reserved
/// IDs, undocumented entries, missing factories, forbidden edges and catalog drift - and only the
/// forbidden edges are decidable from project files, where group A already lives. The other four
/// are facts about what a composition COMPOSED, which is a fact about the register, the catalogs
/// each root prints, and the images they publish to.
/// </para>
/// <para>
/// K3 and K4 read the retained evidence bundle rather than running anything. That is deliberate and
/// it is the same arrangement group H uses: the architecture-test project may not reference a
/// profile assembly - rule A11 forbids exactly that, and it is one of the promises VM-3 exists to
/// make - so it cannot build a catalog of its own to compare against. What it can do is hold the
/// checked-in baseline to what the published binaries actually printed, which is a stronger claim
/// anyway: it compares against three published modes rather than against one in-process build.
/// </para>
/// </remarks>
internal static class CompositionRules
{
    /// <summary>The three core assemblies every composition links, whatever it composes.</summary>
    internal static readonly string[] CoreAssemblies =
        ["Broiler.VM.Abstractions", "Broiler.VM.Binary", "Broiler.VM.Runtime"];

    /// <summary>The two values a register row's Kind column may take.</summary>
    internal static readonly string[] CompositionKinds = ["advertised", "demonstration"];

    /// <summary>
    /// Assemblies whose presence in a published composition is the failure VM-3's gate names: a
    /// fixture, a testing framework, or a reflection or dynamic-code facility.
    /// </summary>
    /// <remarks>
    /// Named rather than derived. A path expression would say "nothing whose project lived under
    /// src/tests/", which is exactly what the consumer profiles do - so the honest test is a list
    /// of the things that must never ship, and the exact-set clause in
    /// <see cref="K4"/> catches everything else by being an equality rather than a subset.
    /// </remarks>
    internal static readonly string[] ForbiddenInAClosure =
    [
        "Broiler.VM.Fixtures",
        "xunit.core",
        "xunit.execution.dotnet",
        "xunit.assert",
        "Microsoft.TestPlatform.CommunicationUtilities",
        "System.Reflection.Emit",
        "System.Reflection.Emit.Lightweight",
        "System.Private.Reflection.Emit",
    ];

    /// <summary>One row of the register's composition table.</summary>
    /// <summary>
    /// One row of the composition register.
    /// </summary>
    /// <param name="Composition">The composition root's assembly name.</param>
    /// <param name="Kind">`advertised` or `demonstration`.</param>
    /// <param name="ProfileIds">The profile IDs the root registers, in catalog order.</param>
    /// <param name="ProfileAssemblies">The assemblies those profiles come from.</param>
    /// <param name="SiblingAssemblies">
    /// Assemblies the root links that are neither core, nor the root, nor a profile: a profile's
    /// own siblings, of which a lowering is the first.
    /// </param>
    /// <param name="Evidence">
    /// The repository-relative bundle directory holding this composition's retained catalog table
    /// and closure report.
    /// </param>
    /// <remarks>
    /// <b>The evidence column exists because this repository now has two milestone series.</b>
    /// Rules K3 and K4 used to read one bundle - the core's current one - which was correct while
    /// every composition belonged to the core. The JavaScript profile's roots belong to the JS
    /// series and their evidence lives in the profile's own bundle tree, and a rule that read the
    /// core's bundle for them would either fail or, worse, quietly compare a JavaScript closure
    /// against a file that was never written for it. Naming the bundle per row is what keeps the
    /// two ledgers apart while one rule still holds every composition to its own evidence.
    /// </remarks>
    internal sealed record Row(
        string Composition,
        string Kind,
        IReadOnlyList<string> ProfileIds,
        IReadOnlyList<string> ProfileAssemblies,
        IReadOnlyList<string>? SiblingAssemblies = null,
        string Evidence = "")
    {
        /// <summary>The sibling assemblies, never null.</summary>
        internal IReadOnlyList<string> Siblings => SiblingAssemblies ?? [];
    }

    /// <summary>
    /// K1: the register and the checkout name the same compositions, and every row declares a
    /// legal kind.
    /// </summary>
    /// <remarks>
    /// Both directions, because each catches a different mistake. A root with no row is the
    /// undocumented entry the gate names; a row with no root is a register describing a composition
    /// that was deleted, which is worse than saying nothing because it reads as a support claim.
    /// </remarks>
    internal static IEnumerable<string> K1(IReadOnlyList<string> roots, IReadOnlyList<Row> rows)
    {
        foreach (var root in roots)
        {
            var matching = rows.Count(row => string.Equals(row.Composition, root, StringComparison.Ordinal));

            if (matching != 1)
            {
                yield return $"{root} has {matching} rows in docs/compositions.md, not exactly one";
            }
        }

        foreach (var row in rows)
        {
            if (!roots.Contains(row.Composition, StringComparer.Ordinal))
            {
                yield return $"docs/compositions.md names {row.Composition}, which is not a composition root in the checkout";
            }

            if (!CompositionKinds.Contains(row.Kind, StringComparer.Ordinal))
            {
                yield return $"{row.Composition} declares kind '{row.Kind}', which is not one of [{string.Join(", ", CompositionKinds)}]";
            }

            if (row.ProfileIds.Count == 0)
            {
                yield return $"{row.Composition} declares no profile";
            }

            if (row.ProfileIds.Count != row.ProfileAssemblies.Count)
            {
                yield return
                    $"{row.Composition} declares {row.ProfileIds.Count} profile IDs and " +
                    $"{row.ProfileAssemblies.Count} profile assemblies";
            }
        }
    }

    /// <summary>
    /// K2: a row's declared profiles agree with the composition's own reference set and with the
    /// catalog it prints.
    /// </summary>
    /// <remarks>
    /// Three independent statements of the same fact, held to each other: what the project links,
    /// what the register says, and what the running binary reports. Any two agreeing while the
    /// third differs is the drift this rule exists to find, and no one of them is taken as the
    /// authority over the other two.
    /// </remarks>
    internal static IEnumerable<string> K2(
        Row row,
        IReadOnlyList<string> referencedAssemblies,
        CatalogTable catalog)
    {
        var referencedProfiles = referencedAssemblies
            .Where(name => !CoreAssemblies.Contains(name, StringComparer.Ordinal))
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        // Two directions, and they are deliberately not the same direction.
        //
        // Every non-core assembly a root REFERENCES must be declared, as a profile or as a
        // sibling: a reference the register does not know about is how a closure grows silently.
        //
        // Every declared PROFILE must be referenced directly, because a composition names its
        // profiles - that is what composing is. A declared SIBLING need not be: a profile's format
        // assembly arrives transitively through the profile and appears in no composition's
        // project file, while being unmistakably in the image. Requiring it here would force a
        // root to name an assembly it does not compose, which is the opposite of what the
        // reference set is for.
        var declaredAssemblies = row.ProfileAssemblies
            .Concat(row.Siblings)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        foreach (var undeclared in referencedProfiles.Where(name =>
                     !declaredAssemblies.Contains(name, StringComparer.Ordinal)))
        {
            yield return
                $"{row.Composition} references {undeclared}, which the register declares as " +
                "neither a profile assembly nor a sibling";
        }

        foreach (var unreferenced in row.ProfileAssemblies.Where(name =>
                     !referencedProfiles.Contains(name, StringComparer.Ordinal)))
        {
            yield return
                $"{row.Composition} declares profile assembly {unreferenced} and does not " +
                "reference it";
        }

        if (!string.Equals(catalog.Composition, row.Composition, StringComparison.Ordinal))
        {
            yield return $"{row.Composition}: the catalog table names {catalog.Composition}";
        }

        if (!catalog.ProfileIds.SequenceEqual(row.ProfileIds, StringComparer.Ordinal))
        {
            yield return
                $"{row.Composition}: the catalog composed [{string.Join(", ", catalog.ProfileIds)}] " +
                $"and the register declares [{string.Join(", ", row.ProfileIds)}]";
        }

        // Package identities come from the composed profiles alone: a sibling is in the closure and
        // is not something a profile is FROM, so it never appears in a catalog table.
        if (!catalog.ProfileAssemblies.SequenceEqual(row.ProfileAssemblies, StringComparer.Ordinal))
        {
            yield return
                $"{row.Composition}: the catalog reported package identities " +
                $"[{string.Join(", ", catalog.ProfileAssemblies)}] and the register declares " +
                $"[{string.Join(", ", row.ProfileAssemblies)}]";
        }

        // Duplicate and reserved IDs, the two the gate names. The runtime refuses both at catalog
        // construction; this is the record-level check that the composition a bundle documents did
        // not somehow contain one, and it is cheap enough to be worth having twice.
        foreach (var duplicate in catalog.ProfileIds
                     .GroupBy(static id => id, StringComparer.Ordinal)
                     .Where(static group => group.Count() > 1))
        {
            yield return $"{row.Composition} composes {duplicate.Key} {duplicate.Count()} times";
        }

        // The reserved first label is reserved FOR Broiler, not forbidden. What the core refuses at
        // catalog construction is the PAIRING: a profile ID whose first label is `broiler` must
        // carry a `Broiler.*` package identity. Until 2026-08-31 this check forbade the namespace
        // outright, which was indistinguishable from the pairing rule while every composed profile
        // was a consumer one under a documentation domain - and became wrong the moment a genuine
        // Broiler-owned profile was composed. The catalog table prints both halves side by side,
        // so the pairing is what is checked here.
        for (var index = 0; index < catalog.ProfileIds.Count; index++)
        {
            if (!IsReservedNamespace(catalog.ProfileIds[index]))
            {
                continue;
            }

            var package = index < catalog.ProfileAssemblies.Count
                ? catalog.ProfileAssemblies[index]
                : string.Empty;

            if (!package.StartsWith("Broiler.", StringComparison.Ordinal))
            {
                yield return
                    $"{row.Composition} composes {catalog.ProfileIds[index]}, which claims the " +
                    $"reserved first label, under package identity '{package}'";
            }
        }
    }

    /// <summary>
    /// K3: the checked-in catalog baseline and the catalog the published composition printed are
    /// byte for byte the same.
    /// </summary>
    /// <remarks>
    /// This is catalog drift, and the comparison is against what a PUBLISHED binary printed rather
    /// than against a table built in the test process. A baseline that only ever agreed with an
    /// in-process build would say nothing about the image a consumer runs, which is where trimming
    /// and Native AOT can change what is composed.
    /// </remarks>
    internal static IEnumerable<string> K3(string composition, string baseline, string retained)
    {
        if (string.IsNullOrWhiteSpace(retained))
        {
            yield return $"{composition}: the bundle retained no catalog table";
            yield break;
        }

        var expected = Normalize(baseline);
        var actual = Normalize(retained);

        if (!string.Equals(expected, actual, StringComparison.Ordinal))
        {
            yield return
                $"{composition}: the catalog drifted from its baseline\n" +
                $"  baseline: {expected.Replace("\n", " / ", StringComparison.Ordinal)}\n" +
                $"  retained: {actual.Replace("\n", " / ", StringComparison.Ordinal)}";
        }
    }

    /// <summary>
    /// K4: every published mode's closure contains exactly the composition, the three core
    /// assemblies and the declared profiles - and nothing forbidden.
    /// </summary>
    /// <remarks>
    /// The Native AOT mode is allowed to contain nothing at all, and normally does: a native image
    /// carries no managed assembly, which is a stronger result than the equality rather than an
    /// exemption from it. What it may never contain is something outside the allowed set, and that
    /// clause applies to every mode.
    /// </remarks>
    internal static IEnumerable<string> K4(Row row, IReadOnlyList<ClosureMode> modes)
    {
        if (modes.Count == 0)
        {
            yield return $"{row.Composition}: the bundle retained no closure report";
            yield break;
        }

        var allowed = CoreAssemblies
            .Concat([row.Composition])
            .Concat(row.ProfileAssemblies)
            .Concat(row.Siblings)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        foreach (var mode in modes)
        {
            foreach (var forbidden in mode.Assemblies.Where(name =>
                         ForbiddenInAClosure.Contains(name, StringComparer.Ordinal)))
            {
                yield return $"{row.Composition} [{mode.Name}] ships {forbidden}";
            }

            foreach (var unexpected in mode.Assemblies.Where(name =>
                         !allowed.Contains(name, StringComparer.Ordinal)))
            {
                yield return $"{row.Composition} [{mode.Name}] ships {unexpected}, which it does not declare";
            }

            if (mode.Assemblies.Count == 0)
            {
                // A native image, or a mode that produced nothing. Either way there is no managed
                // assembly to be wrong about, and the equality below would fail on a stronger
                // result than it asks for.
                continue;
            }

            var actual = mode.Assemblies
                .OrderBy(static name => name, StringComparer.Ordinal)
                .ToArray();

            if (!actual.SequenceEqual(allowed, StringComparer.Ordinal))
            {
                yield return
                    $"{row.Composition} [{mode.Name}] ships [{string.Join(", ", actual)}] " +
                    $"rather than [{string.Join(", ", allowed)}]";
            }
        }
    }

    /// <summary>The profiles one composition's catalog table reported.</summary>
    internal sealed record CatalogTable(
        string Composition,
        IReadOnlyList<string> ProfileIds,
        IReadOnlyList<string> ProfileAssemblies);

    /// <summary>The non-framework assemblies one published mode contained.</summary>
    internal sealed record ClosureMode(string Name, IReadOnlyList<string> Assemblies);

    /// <summary>
    /// Whether a profile ID claims the reserved first label, under the same ASCII fold the core
    /// applies.
    /// </summary>
    internal static bool IsReservedNamespace(string profileId)
    {
        var dot = profileId.IndexOf('.', StringComparison.Ordinal);
        var first = dot < 0 ? profileId : profileId[..dot];

        return string.Equals(first, "broiler", StringComparison.OrdinalIgnoreCase);
    }

    private static string Normalize(string text) =>
        string.Join(
            '\n',
            text.Replace("\r\n", "\n", StringComparison.Ordinal)
                .Split('\n')
                .Select(static line => line.TrimEnd())
                .Where(static line => line.Length != 0));
}
