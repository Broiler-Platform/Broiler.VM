using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// Synthesized source, read as though it were a product file.
/// </summary>
/// <remarks>
/// The assurance rules have to be shown rejecting and accepting shapes the checkout does not
/// contain - a record, an <c>AssemblyMarker</c>, a review a human signed, a fingerprint that has
/// gone stale. This component has none of those and must not grow one to be tested against. The
/// probe is the equivalent of the group A witness project files: a deliberately shaped input, held
/// in the test rather than on the product tree.
/// </remarks>
internal static class AssuranceProbe
{
    internal static AssuranceSourceFile Source(string text, string name = "probe.cs") =>
        new(
            FullPath: name,
            RelativePath: name,
            Assembly: "Broiler.VM.Probe",
            Text: text,
            NewLine: "\n",
            Tree: AssuranceSources.Parse(text, name));

    internal static IReadOnlyList<AssuranceUnit> Scan(string text) =>
        AssuranceScanner.Scan(Source(text));

    internal static AssuranceUnit Unit(string text, string member) =>
        Scan(text).SingleOrDefault(unit => unit.Name.EndsWith(member, StringComparison.Ordinal))
        ?? throw new InvalidOperationException(
            $"No probe unit whose name ends with '{member}'. Scanned: " +
            string.Join(", ", Scan(text).Select(static unit => unit.Name)));

    /// <summary>
    /// The one unit whose simple name matches, whatever its signature. The fingerprint probes edit
    /// parameter types, so they cannot select a unit by a signature that is what they are changing.
    /// </summary>
    internal static AssuranceUnit Named(string text, string simpleName) =>
        Scan(text).Single(unit =>
            unit.Name.Contains($".{simpleName}(", StringComparison.Ordinal) ||
            unit.Name.EndsWith($".{simpleName}", StringComparison.Ordinal));

    /// <summary>The fingerprint of the one declaration in a probe, by member name.</summary>
    internal static string Fingerprint(string text, string member) => Named(text, member).Fingerprint;

    internal static string TokenStream(string text, string member) =>
        AssuranceFingerprint.TokenStream(Named(text, member).Declaration);

    /// <summary>The lines the generator would write for a probe, so a rewrite can be read.</summary>
    internal static IReadOnlyList<string> Rewritten(string text)
    {
        var file = Source(text);
        var rewritten = AssuranceGenerator.DesiredSource(file, AssuranceScanner.Scan(file));
        var lines = new List<string>();
        var split = new AssuranceText(rewritten);

        for (var line = 0; line < split.Count; line++)
        {
            lines.Add(split[line]);
        }

        return lines;
    }

    internal static string AnnotationLine(string text, string marker) =>
        Rewritten(text)
            .Select(static line => line.Trim())
            .Single(line => line.StartsWith(marker, StringComparison.Ordinal));

    internal static MemberDeclarationSyntax Declaration(string text, string member) =>
        Unit(text, member).Declaration;

    /// <summary>
    /// Writes <paramref name="text"/> to a real file and returns it as a generated artefact:
    /// <c>Current</c> is what is on disk, <c>Desired</c> is what the generator would write there.
    /// </summary>
    /// <remarks>
    /// <para>
    /// J5's primary clause is the comparison between the artefacts on disk and the artefacts the
    /// generator would produce, over the real tree. Every other J5 witness exercises
    /// <see cref="AssuranceGenerator.DesiredSource"/> on synthesized text held in memory, so the
    /// clause that actually gates the checkout - the one the register row and the report both
    /// describe - had no witness at all, and could be deleted from both places that asserted it
    /// with the suite green.
    /// </para>
    /// <para>
    /// This is that witness. The text is written to a temporary file and read back, so the
    /// comparison runs against bytes on a disk rather than against a string the test is holding,
    /// which is the only way the READ half of the clause is exercised at all. The file is written
    /// UTF-8 without a byte-order mark, exactly as <see cref="AssuranceGenerator.Apply"/> writes.
    /// </para>
    /// </remarks>
    internal static AssuranceArtefact ArtefactOnDisk(string text, string fileName)
    {
        var directory = Path.Combine(
            Path.GetTempPath(), "broiler-assurance-probe", Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, fileName);

        File.WriteAllText(path, text, AssuranceSources.Utf8NoBom);

        var onDisk = File.ReadAllText(path);

        var file = new AssuranceSourceFile(
            FullPath: path,
            RelativePath: fileName,
            Assembly: "Broiler.VM.Probe",
            Text: onDisk,
            NewLine: onDisk.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n",
            Tree: AssuranceSources.Parse(onDisk, path));

        return new AssuranceArtefact(
            fileName,
            path,
            onDisk,
            AssuranceGenerator.DesiredSource(file, AssuranceScanner.Scan(file)));
    }
}
