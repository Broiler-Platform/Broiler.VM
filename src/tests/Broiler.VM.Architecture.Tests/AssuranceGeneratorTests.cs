namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The two modes: the harness that applies the plan or asserts it, and the fixed point that makes
/// the two agree.
/// </summary>
/// <remarks>
/// <para>
/// <c>BROILER_ASSURANCE_WRITE=1</c> makes this the generator; without it, it is the gate. One
/// plan, computed once, either applied or asserted - so the thing the gate checks is by
/// construction the thing the generator would have produced.
/// </para>
/// <para>
/// <b>Where the assurance PROPERTIES are asserted.</b> Not here. Coverage, well-formedness,
/// fingerprint currency, the prohibition on a machine-made approval and the currency of the
/// generated artefacts are architecture rules J1 to J9, in <c>AssuranceRuleTests</c>, each with
/// per-clause witness inputs under <c>witnesses/assurance/</c>. An earlier revision asserted four
/// of those properties here as bare facts over the checkout, with nothing showing them reject
/// anything; they were moved rather than duplicated, because two places claiming the same property
/// and only one of them witnessed is how a check quietly becomes decoration.
/// </para>
/// <para>
/// What is left here is the part that is about the MACHINERY rather than about the record: that
/// the plan is applied in write mode and asserted otherwise, and that one generation is a fixed
/// point. The fixed point is the non-obvious half. A file's header counts review states, a state
/// depends on a fingerprint, and the fingerprint is what the same pass is filling in - so a
/// generator that published the states it found rather than the states it was about to write
/// would leave a tree the gate could never accept.
/// </para>
/// </remarks>
public sealed class AssuranceGeneratorTests
{
    [Fact]
    public void The_Generated_Artefacts_Are_Written_In_Write_Mode_And_Asserted_Otherwise()
    {
        var plan = AssuranceGenerator.Current.Artefacts;

        Assert.NotEmpty(plan);
        Assert.Contains(plan, artefact =>
            string.Equals(artefact.RelativePath, AssuranceGenerator.ReportPath, StringComparison.Ordinal));

        if (!AssuranceGenerator.WriteRequested)
        {
            // The gate. Byte comparison, because "equivalent" is what a stale summary always
            // claims to be. The comparison itself is AssuranceGenerator.StaleArtefacts, which rule
            // J5 asserts as well and witnesses against a deliberately stale artefact - one
            // function, so the two places cannot drift into disagreeing about what "current"
            // means, and the function is pinned by a witness rather than only by its call sites.
            Assert.Empty(AssuranceGenerator.StaleArtefacts(plan));

            return;
        }

        AssuranceGenerator.Apply(plan);

        // The generator has to be a fixed point, or the gate can never be green after it runs -
        // and a summary is the easy way to get that wrong, because it counts states that the same
        // pass is changing. Everything is read back OFF DISK, rescanned, and regenerated.
        var rescanned = AssuranceSources.Files
            .Select(static file => AssuranceSources.ReadFile(file.FullPath, file.Assembly))
            .ToArray();

        var unstable = rescanned
            .Where(static reread => !string.Equals(
                AssuranceGenerator.DesiredSource(reread, AssuranceScanner.Scan(reread)),
                reread.Text,
                StringComparison.Ordinal))
            .Select(static reread => $"{reread.RelativePath} is not stable under a second generation")
            .ToArray();

        Assert.Empty(unstable);

        Assert.Equal(
            AssuranceGenerator.ComponentReport(rescanned.SelectMany(AssuranceScanner.Scan).ToArray()),
            File.ReadAllText(Path.Combine(ComponentGraph.Root, AssuranceGenerator.ReportPath)));
    }

}

/// <summary>The lines of a rendered artefact, for tests that read what a reader would see.</summary>
internal sealed class AssuranceTextLines : List<string>
{
    internal AssuranceTextLines(string text)
    {
        var split = new AssuranceText(text);

        for (var line = 0; line < split.Count; line++)
        {
            Add(split[line]);
        }
    }
}
