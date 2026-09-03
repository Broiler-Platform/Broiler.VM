using Broiler.VM.Profile.JavaScript.Compiler;
using System.Text;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// The raw host mode's fixtures: artifact bytes, and the sidecar that declares what must happen.
/// </summary>
/// <remarks>
/// <para>
/// <b>They are written by this root rather than hand-authored</b>, for the reason the retained
/// corpus is: a byte sequence nobody can regenerate is a fixture nobody can review, and one that
/// was hand-edited would drift the moment the format's writer changed. Each recipe below starts
/// from a program this profile compiles and then breaks it in one named way.
/// </para>
/// <para>
/// <b>Two of the three are deliberately broken and one is a control.</b> A raw mode made only of
/// good artifacts would be passed by a verifier that accepted everything, and one made only of bad
/// ones by a verifier that accepted nothing.
/// </para>
/// </remarks>
internal static class RawFixtures
{
    /// <summary>The source every recipe starts from: a program with a completion value of 3.</summary>
    private const string Seed = "1 + 2;\n";

    /// <summary>One raw fixture: its name, what it is, and how its bytes are produced.</summary>
    private sealed record Recipe(
        string Name,
        string Description,
        ConformanceExpectation Expectation,
        Func<byte[], byte[]> Break);

    private static readonly Recipe[] Recipes =
    [
        new(
            "an-artifact-that-runs",
            "artifact bytes with no lowering consulted, running to their completion value",
            new ConformanceExpectation(ExpectationKind.Completion, "3"),
            static bytes => bytes),

        new(
            "an-artifact-whose-magic-is-wrong",
            "the first byte of the magic is not the format's, so the read stage refuses the header",
            new ConformanceExpectation(ExpectationKind.RefusedByVerifier, "WrongMagic"),
            static bytes =>
            {
                var broken = (byte[])bytes.Clone();
                broken[0] = (byte)(broken[0] ^ 0xFF);
                return broken;
            }),

        new(
            "an-artifact-that-stops-early",
            "the payload ends before the sections it declares do",
            new ConformanceExpectation(ExpectationKind.RefusedByVerifier, "Truncated"),
            static bytes => bytes[..(bytes.Length - 3)]),
    ];

    /// <summary>Writes every raw fixture and its sidecar into a suite's test directory.</summary>
    /// <remarks>
    /// The sidecar is written from the same record the bytes are, so a fixture whose recipe changed
    /// and whose declaration did not cannot exist. What CAN still be wrong is the declaration
    /// itself - a recipe declaring the wrong refusal - and that is what running the suite finds.
    /// </remarks>
    internal static IReadOnlyList<string> Write(string directory)
    {
        var written = new List<string>();
        var compiled = SliceSourceCompiler.Compile(Seed);

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            throw new InvalidOperationException(
                $"the raw fixtures' seed program did not compile: {Seed}");
        }

        Directory.CreateDirectory(directory);

        foreach (var recipe in Recipes)
        {
            var artifact = Path.Combine(directory, recipe.Name + Suite.ArtifactExtension);
            var sidecar = artifact + Suite.SidecarExtension;

            File.WriteAllBytes(artifact, recipe.Break(compiled.Artifact));

            var text = new StringBuilder()
                .Append("/*---\n")
                .Append("description: ").Append(recipe.Description).Append('\n')
                .Append("expected: ").Append(recipe.Expectation).Append('\n')
                .Append("flags: [raw]\n")
                .Append("---*/\n")
                .ToString();

            File.WriteAllText(sidecar, text);
            written.Add(recipe.Name);
        }

        return written;
    }
}
