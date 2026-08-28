using System.Text;
using System.Text.Json;

namespace Broiler.VM.Architecture.Tests;

/// <summary>One row of <c>assurance.manifest.json</c>: one code unit, watched.</summary>
internal sealed record AssuranceManifestEntry(
    string Name,
    string File,
    bool Exempt,
    string Exemption,
    string Fingerprint);

/// <summary>
/// One row of the manifest's <c>files</c> array: one covered file, watched WHOLE.
/// </summary>
/// <remarks>
/// The unit entries beside it are a whitelist of declaration kinds, and what a whitelist does not
/// name has no entry. This row has no such gap: its fingerprint is taken over the complete token
/// stream of the file's compilation unit, so an assembly-level attribute, a using directive, a
/// declaration kind nobody has thought of yet and a reordering of an enum's members all move it.
/// </remarks>
internal sealed record AssuranceManifestFile(string File, string Fingerprint);

/// <summary>
/// The manifest: every covered file in the product tree with a fingerprint over the whole of it,
/// and every code unit inside them, exempt and relevant alike, with a fingerprint of its own.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why it exists.</b> Three rounds of adversarial review produced the same defeat in five
/// different places, and every one of them had the same shape: the exemption predicate answered
/// EXEMPT, an exempt unit carries no annotation, an unannotated unit carries no fingerprint, and a
/// unit with no fingerprint has no record of any kind - so a semantic change to it was invisible to
/// every rule in group J. Patching the predicate case by case failed three times, because each
/// patch moved the same defeat one case over: the constructor case was narrowed and the property
/// case inherited it; <c>const</c> fields became units and property initializers were still
/// unwatched.
/// </para>
/// <para>
/// The repair is to stop fusing two questions that are not the same question:
/// </para>
/// <code>
/// does this unit need a human annotation?   the exemption predicate decides this
/// is this unit watched for change?          EVERY unit is, and this file is the record
/// </code>
/// <para>
/// <b>And why the units are not enough on their own.</b> A fourth round attacked the ENUMERATION
/// rather than the predicate. A unit exists only for a declaration kind the scanner names, so an
/// enum member, a type declaration header, an event field and - the one that can never be fixed by
/// naming it - an <c>[assembly: InternalsVisibleTo("anything")]</c> were in no unit, no fingerprint
/// and no entry, while this file's header said it covered every code unit. The first three are
/// units now. The fourth is why the <c>files</c> array exists: one fingerprint per covered file over
/// its complete token stream, so the record is complete rather than merely wider.
/// </para>
/// <para>
/// <b>What a manifest entry is, and what it is not.</b> It is a change-detection record. An entry
/// says what a declaration's token texts hashed to when the generator last ran, and nothing else.
/// It is NOT a review, NOT an assessment, and NOT a claim that anyone has looked at the unit: the
/// exempt units in here carry no annotation, no human line and no reviewer, exactly as before.
/// What changes is that editing one of them now moves a value in a generated file that the gate
/// compares byte for byte, so the edit shows up in a diff and fails a suite that has not been
/// regenerated. Nothing here promotes a unit towards being reviewed, and
/// <c>CODE-ASSURANCE.md</c> and the register row for rule J7 say the same thing in the same words.
/// </para>
/// <para>
/// <b>Ordering.</b> By file and then by unit name, both ordinal, so a diff is readable and a
/// regeneration that changes nothing is byte-identical. Deliberately NOT by line: an entry that
/// carried a line number would churn every time a comment was added above it, which is exactly
/// what the generator does.
/// </para>
/// </remarks>
internal static class AssuranceManifest
{
    /// <summary>Where the manifest lives, relative to the component root.</summary>
    internal const string RelativePath = "assurance.manifest.json";

    /// <summary>
    /// The sentence that must appear in the manifest, in <c>CODE-ASSURANCE.md</c> and in rule J7's
    /// register row, so that no reader takes a covered fingerprint for a reviewed unit.
    /// </summary>
    internal const string ChangeDetectionStatement =
        "This manifest is a change-detection record, not a review.";

    /// <summary>
    /// The sentence that states what the file fingerprints add, in the same three places for the
    /// same reason: it is the claim a reader is entitled to make from this file.
    /// </summary>
    internal const string CompletenessStatement =
        "Nothing in a covered file can change without something moving here, whatever kind of " +
        "declaration it is.";

    /// <summary>
    /// The manifest's <c>$comment</c> block, exactly as it is written.
    /// </summary>
    /// <remarks>
    /// Exposed rather than private because it is the text the generator publishes about itself, and
    /// nothing held it to anything: appending two lines here made the manifest assert that every
    /// unit was VERIFIED, human-reviewed and eligible for release, with both modes of the gate
    /// green and every human line in the tree still reading PENDING. Rule J8 holds this array to a
    /// hand-maintained copy in <see cref="AssuranceArtefactShape"/>, and rule J9 forbids the
    /// vocabulary that attack used anywhere in any generated artefact.
    /// </remarks>
    internal static readonly string[] Header =
    [
        "GENERATED - DO NOT EDIT MANUALLY. Regenerate with",
        "`BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release`.",
        "",
        ChangeDetectionStatement + " Every code unit in the three product",
        "assemblies is listed here, exempt and relevant alike, with the fingerprint of its",
        "declaration. An entry records what that declaration's tokens hashed to when the generator",
        "last ran. It is not an assessment, it is not an approval, and it is not evidence that",
        "anyone has read the unit. Exempt units still need no annotation and carry none, and no",
        "human line in this component has moved off PENDING.",
        "",
        "The 'files' array beside the units is what makes this record COMPLETE. A unit entry exists",
        "only for a declaration kind the scanner enumerates, and that enumeration is a whitelist: an",
        "assembly-level attribute is a member of nothing and can be in no unit at all. Each file",
        "entry is a fingerprint over the complete token stream of that file's compilation unit.",
        CompletenessStatement,
        "",
        "What the manifest adds is that a unit the exemption predicate treats as trivial is no",
        "longer invisible: a semantic change to one moves a fingerprint in a generated file the",
        "gate compares byte for byte, so the change appears in a diff and fails an unregenerated",
        "suite. Rule J7 holds this file to the tree - every unit and every covered file present, no",
        "extras, every fingerprint current.",
    ];

    /// <summary>The manifest the current tree implies, in the order it is written.</summary>
    internal static IReadOnlyList<AssuranceManifestEntry> Entries(IEnumerable<AssuranceUnit> units) =>
        units
            .Select(static unit => new AssuranceManifestEntry(
                Name: unit.Name,
                File: unit.File.RelativePath,
                Exempt: unit.IsExempt,
                Exemption: unit.Exemption.ToString(),
                Fingerprint: unit.Fingerprint))
            .OrderBy(static entry => entry.File, StringComparer.Ordinal)
            .ThenBy(static entry => entry.Name, StringComparer.Ordinal)
            .ThenBy(static entry => entry.Fingerprint, StringComparer.Ordinal)
            .ToArray();

    /// <summary>
    /// The file rows the current tree implies, in the order they are written.
    /// </summary>
    /// <remarks>
    /// One row per covered file, including the files that declare no code unit at all: a file with
    /// no unit is exactly the file a unit-only record cannot say anything about.
    /// </remarks>
    internal static IReadOnlyList<AssuranceManifestFile> FileEntries(IEnumerable<AssuranceSourceFile> files) =>
        files
            .Select(static file => new AssuranceManifestFile(
                File: file.RelativePath,
                Fingerprint: AssuranceFingerprint.OfFile(file.Tree)))
            .OrderBy(static entry => entry.File, StringComparer.Ordinal)
            .ToArray();

    /// <summary>The manifest as it is written to disk. Deterministic, LF, no trailing whitespace.</summary>
    internal static string Render(IEnumerable<AssuranceSourceFile> files, IEnumerable<AssuranceUnit> units)
    {
        var entries = Entries(units);
        var covered = FileEntries(files);
        var json = new StringBuilder();

        json.Append("{\n  \"$comment\": [\n");

        for (var line = 0; line < Header.Length; line++)
        {
            json.Append("    \"").Append(Escape(Header[line])).Append('"');
            json.Append(line == Header.Length - 1 ? "\n" : ",\n");
        }

        json.Append("  ],\n");
        json.Append("  \"files\": [\n");

        for (var index = 0; index < covered.Count; index++)
        {
            json.Append("    {");
            json.Append(" \"file\": \"").Append(Escape(covered[index].File)).Append("\",");
            json.Append(" \"fingerprint\": \"").Append(Escape(covered[index].Fingerprint)).Append("\" }");
            json.Append(index == covered.Count - 1 ? "\n" : ",\n");
        }

        json.Append("  ],\n");
        json.Append("  \"units\": [\n");

        for (var index = 0; index < entries.Count; index++)
        {
            var entry = entries[index];

            json.Append("    {");
            json.Append(" \"name\": \"").Append(Escape(entry.Name)).Append("\",");
            json.Append(" \"file\": \"").Append(Escape(entry.File)).Append("\",");
            json.Append(" \"exempt\": ").Append(entry.Exempt ? "true" : "false").Append(',');
            json.Append(" \"exemption\": \"").Append(Escape(entry.Exemption)).Append("\",");
            json.Append(" \"fingerprint\": \"").Append(Escape(entry.Fingerprint)).Append("\" }");
            json.Append(index == entries.Count - 1 ? "\n" : ",\n");
        }

        json.Append("  ]\n}\n");

        return json.ToString();
    }

    /// <summary>
    /// Every disagreement between a manifest text and the units it claims to cover: a unit that is
    /// missing, an entry that names no unit, and an entry whose recorded fingerprint or exemption
    /// state is not the one the code produces now.
    /// </summary>
    /// <remarks>
    /// The three are reported separately and by content, because they are three different facts
    /// about a checkout. A unit missing from the manifest is a unit nothing is watching; an entry
    /// for a unit that is gone is a record of code that no longer exists; and a fingerprint that is
    /// not current is a unit that changed since the manifest was written - which, for an exempt
    /// unit, is the only record that the change happened at all.
    /// </remarks>
    internal static List<string> Violations(
        IEnumerable<AssuranceSourceFile> files,
        IEnumerable<AssuranceUnit> units,
        string manifestText)
    {
        var violations = new List<string>();
        var expected = new Dictionary<(string File, string Name), AssuranceManifestEntry>();

        foreach (var entry in Entries(units))
        {
            if (!expected.TryAdd((entry.File, entry.Name), entry))
            {
                violations.Add(
                    $"{entry.File} declares more than one unit named {entry.Name}, and a manifest " +
                    "entry addresses a unit by its file and its name");
            }
        }

        var recorded = new Dictionary<(string File, string Name), AssuranceManifestEntry>();

        foreach (var entry in Read(manifestText, violations))
        {
            if (!recorded.TryAdd((entry.File, entry.Name), entry))
            {
                violations.Add(
                    $"{RelativePath} carries more than one entry for {entry.Name} in {entry.File}");
            }
        }

        foreach (var (key, entry) in expected.OrderBy(static pair => pair.Key))
        {
            if (!recorded.TryGetValue(key, out var found))
            {
                violations.Add(
                    $"{entry.File}: {entry.Name} is a code unit in the product tree and " +
                    $"{RelativePath} does not cover it, so nothing records a change to it");

                continue;
            }

            if (!string.Equals(found.Fingerprint, entry.Fingerprint, StringComparison.Ordinal))
            {
                violations.Add(
                    $"{entry.File}: {entry.Name} is recorded in {RelativePath} as " +
                    $"{found.Fingerprint} and the current code computes {entry.Fingerprint}");
            }

            if (!string.Equals(found.Exemption, entry.Exemption, StringComparison.Ordinal) ||
                found.Exempt != entry.Exempt)
            {
                violations.Add(
                    $"{entry.File}: {entry.Name} is recorded in {RelativePath} as {found.Exemption} " +
                    $"and the predicate answers {entry.Exemption}");
            }
        }

        foreach (var (key, entry) in recorded.OrderBy(static pair => pair.Key))
        {
            if (!expected.ContainsKey(key))
            {
                violations.Add(
                    $"{entry.File}: {RelativePath} carries an entry for {entry.Name}, which is not " +
                    "a code unit in the product tree");
            }
        }

        violations.AddRange(FileViolations(files, manifestText));

        return violations;
    }

    /// <summary>
    /// Every disagreement between the manifest's <c>files</c> array and the covered files: a file
    /// that is missing, an entry that names a file that is not covered, and an entry whose recorded
    /// fingerprint is not the one the file's tokens produce now.
    /// </summary>
    /// <remarks>
    /// Reported as three facts for the reason the unit disagreements are: a covered file missing
    /// from the array is a file NOTHING watches whole, so a declaration kind the unit enumeration
    /// does not name could change inside it with no record moving anywhere; an entry for a file
    /// that is not covered is a record of a file that is gone; and a stale file fingerprint is a
    /// file that changed since the manifest was written, which for anything outside a unit is the
    /// only record that the change happened at all.
    /// </remarks>
    private static List<string> FileViolations(IEnumerable<AssuranceSourceFile> files, string manifestText)
    {
        var violations = new List<string>();
        var expected = FileEntries(files).ToDictionary(
            static entry => entry.File, static entry => entry.Fingerprint, StringComparer.Ordinal);

        var recorded = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var entry in ReadFiles(manifestText, violations))
        {
            if (!recorded.TryAdd(entry.File, entry.Fingerprint))
            {
                violations.Add($"{RelativePath} carries more than one file entry for {entry.File}");
            }
        }

        foreach (var (file, fingerprint) in expected.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            if (!recorded.TryGetValue(file, out var found))
            {
                violations.Add(
                    $"{file} is a covered file and {RelativePath} records no fingerprint for it, so " +
                    "nothing watches the whole of it");

                continue;
            }

            if (!string.Equals(found, fingerprint, StringComparison.Ordinal))
            {
                violations.Add(
                    $"{file} is recorded in {RelativePath} as file fingerprint {found} and the " +
                    $"current file computes {fingerprint}");
            }
        }

        foreach (var (file, _) in recorded.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            if (!expected.ContainsKey(file))
            {
                violations.Add(
                    $"{RelativePath} carries a file entry for {file}, which is not a covered file");
            }
        }

        return violations;
    }

    /// <summary>Reads a manifest text, reporting rather than throwing when it is not one.</summary>
    private static IReadOnlyList<AssuranceManifestEntry> Read(string text, List<string> violations)
    {
        var entries = new List<AssuranceManifestEntry>();

        if (text.Length == 0)
        {
            violations.Add($"{RelativePath} is absent or empty, so no unit is covered at all");

            return entries;
        }

        JsonDocument document;

        try
        {
            document = JsonDocument.Parse(text);
        }
        catch (JsonException problem)
        {
            violations.Add($"{RelativePath} does not parse as JSON: {problem.Message}");

            return entries;
        }

        using (document)
        {
            if (!document.RootElement.TryGetProperty("units", out var units) ||
                units.ValueKind != JsonValueKind.Array)
            {
                violations.Add($"{RelativePath} carries no 'units' array");

                return entries;
            }

            foreach (var unit in units.EnumerateArray())
            {
                entries.Add(new AssuranceManifestEntry(
                    Name: Text(unit, "name"),
                    File: Text(unit, "file"),
                    Exempt: unit.TryGetProperty("exempt", out var exempt) &&
                        exempt.ValueKind == JsonValueKind.True,
                    Exemption: Text(unit, "exemption"),
                    Fingerprint: Text(unit, "fingerprint")));
            }
        }

        return entries;
    }

    /// <summary>Reads the <c>files</c> array, reporting rather than throwing when it is not one.</summary>
    private static IReadOnlyList<AssuranceManifestFile> ReadFiles(string text, List<string> violations)
    {
        var entries = new List<AssuranceManifestFile>();

        if (text.Length == 0)
        {
            // The unit reader has already said the file is absent or empty. Saying it twice would
            // report one fact as two.
            return entries;
        }

        JsonDocument document;

        try
        {
            document = JsonDocument.Parse(text);
        }
        catch (JsonException)
        {
            // Likewise: the unit reader names the parse failure with its message.
            return entries;
        }

        using (document)
        {
            if (!document.RootElement.TryGetProperty("files", out var files) ||
                files.ValueKind != JsonValueKind.Array)
            {
                violations.Add(
                    $"{RelativePath} carries no 'files' array, so no covered file is watched whole");

                return entries;
            }

            foreach (var file in files.EnumerateArray())
            {
                entries.Add(new AssuranceManifestFile(
                    File: Text(file, "file"),
                    Fingerprint: Text(file, "fingerprint")));
            }
        }

        return entries;
    }

    private static string Text(JsonElement element, string property) =>
        element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? string.Empty
            : string.Empty;

    /// <summary>
    /// JSON string escaping, written out rather than delegated.
    /// </summary>
    /// <remarks>
    /// The framework's default encoder is HTML-safe and escapes an angle bracket into its
    /// six-character unicode form, which would render every generic unit name unreadable in a file
    /// whose whole purpose is to be read in a diff. The relaxed encoder does not, but reaching it
    /// means threading a serializer options object through a document this code builds by hand.
    /// </remarks>
    private static string Escape(string value)
    {
        var escaped = new StringBuilder(value.Length);

        foreach (var character in value)
        {
            switch (character)
            {
                case '"': escaped.Append("\\\""); break;
                case '\\': escaped.Append("\\\\"); break;
                case '\n': escaped.Append("\\n"); break;
                case '\r': escaped.Append("\\r"); break;
                case '\t': escaped.Append("\\t"); break;
                default:
                    if (character < ' ')
                    {
                        escaped.Append("\\u").Append(((int)character).ToString("X4"));
                    }
                    else
                    {
                        escaped.Append(character);
                    }

                    break;
            }
        }

        return escaped.ToString();
    }
}
