namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Group W: the WebAssembly profile family's own rules.
/// </summary>
/// <remarks>
/// <para>
/// <b>The letter is chosen because every other one is taken or reserved.</b> A to E, H, J, K, L, M,
/// N and V are in use in this register; R1..R6 is the reserved namespace ADR 0003 records for ADR
/// 0012's six ownership roles; and F, G, P, S and T collide with clause labels inside ADRs 0007,
/// 0011 and 0012, which would make a sentence naming one ambiguous about which document it meant.
/// </para>
/// <para>
/// <b>Why a second family needs a second group at all.</b> Two of group N's rules are written
/// generically over the language segment - no cross-family edge, and no family project packable -
/// and they cover this family the day its projects exist, with no code change. The rest of group N
/// is hard-coded to the JavaScript assembly names, deliberately, because a rule about a family is a
/// rule about that family's own shape. This family's shape is not the JavaScript family's: it has
/// one project rather than three, no format pivot and no lowering, so the reference-set claim it
/// needs is a different sentence and gets a different row.
/// </para>
/// </remarks>
internal static class WebAssemblyFamilyRules
{
    /// <summary>The WebAssembly profile assembly itself.</summary>
    internal const string ProfileAssembly = "Broiler.VM.Profile.WebAssembly";

    /// <summary>The exact Broiler.VM-owned reference set this profile assembly may have.</summary>
    /// <remarks>
    /// ADR 0011's obligation P1, with nothing added to it. The JavaScript profile's set carries a
    /// third name because that family has a format sibling to hold a lowering and an executor apart;
    /// this family has neither, so the set is the two core assemblies and the rule says so.
    /// </remarks>
    internal static readonly string[] ProfileCoreReferences =
        ["Broiler.VM.Abstractions", "Broiler.VM.Binary"];

    /// <summary>
    /// W1: the WebAssembly profile assembly references exactly Abstractions and Binary; it never
    /// references the runtime and never grows a family sibling; it declares no PackageReference and
    /// opens its internals to nobody.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Four independent claims, each named separately even where the set comparison already implies
    /// it, so the message a reader gets says which property broke rather than reporting the most
    /// consequential violation in this family in the same words as a typo.
    /// </para>
    /// <para>
    /// <b>The sibling clause is the one that is not obvious.</b> Rules A11 and N2 both EXEMPT a
    /// sibling in the same profile family, keyed on the language segment, because the JavaScript
    /// family needs that exemption to have a format pivot at all. So a
    /// <c>Broiler.VM.Profile.WebAssembly.Format</c> appearing in this project's references would
    /// pass every rule in groups A and N. This clause is what makes the decision that there is no
    /// such assembly a property of the checkout rather than a paragraph in a record.
    /// </para>
    /// <para>
    /// Rule A13 does not reach this project: its subject is a test-only consumer profile, and this
    /// is a product one. Widening A13 to cover a product profile would weaken the rule that holds
    /// the two consumer profiles, which is the repair rule N1 declined for the same reason.
    /// </para>
    /// </remarks>
    internal static IEnumerable<string> W1(ComponentGraph.ProjectFile project)
    {
        if (!string.Equals(project.AssemblyName, ProfileAssembly, StringComparison.Ordinal))
        {
            yield break;
        }

        var referenced = project.ReferencedAssemblyNames
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        if (!referenced.SequenceEqual(ProfileCoreReferences, StringComparer.Ordinal))
        {
            yield return
                $"{project.RelativePath} references [{string.Join(", ", referenced)}] rather than " +
                $"[{string.Join(", ", ProfileCoreReferences)}]";
        }

        if (referenced.Contains("Broiler.VM.Runtime", StringComparer.Ordinal))
        {
            yield return
                $"{project.RelativePath} references Broiler.VM.Runtime, which obligation P1 of " +
                "ADR 0011 forbids a profile";
        }

        foreach (var sibling in referenced.Where(static name =>
                     name.StartsWith(ProfileAssembly + ".", StringComparison.Ordinal)))
        {
            yield return
                $"{project.RelativePath} references {sibling}, so this family would carry a " +
                "sibling it does not have";
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
}
