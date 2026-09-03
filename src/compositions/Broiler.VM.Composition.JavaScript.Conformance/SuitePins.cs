using System.Globalization;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>
/// A pin this repository retains for a suite whose content it does not hold.
/// </summary>
/// <remarks>
/// <para>
/// <b>This exists because a pin a suite writes about itself is not a pin.</b> The
/// <c>--pin</c> mode computes a digest over a directory and writes it into that directory, which
/// is exactly right for a suite this repository holds and worth nothing for one it does not: a
/// third-party checkout carrying a <c>suite.pin</c> somebody generated inside it certifies that
/// the directory has not changed since the harness last looked at it, and certifies nothing about
/// which upstream revision it is. Whoever can edit the suite can edit the pin in the same gesture.
/// </para>
/// <para>
/// <b>So the authority moves here.</b> A retained pin names the upstream revision - the immutable
/// commit roadmap section 14 asks for, "never a branch name" - beside the digest of the content
/// that revision produces, in a file the suite cannot reach. A run given one is a run whose suite
/// has to match something this repository decided, and a checkout that has drifted fails against a
/// figure it did not write.
/// </para>
/// <para>
/// <b>It holds no suite content and this component holds none.</b> The suite is 232 megabytes over
/// 56,560 files and is separately licensed; what is retained is the pin, which is the part a
/// verification needs. Archiving the material itself is a further human action, and the pin says
/// in its own field whether it has happened.
/// </para>
/// </remarks>
internal sealed record RetainedSuitePin(
    string Suite,
    string Upstream,
    string Revision,
    string Archive,
    string ArchiveDigest,
    string ContentDigest,
    int Files,
    bool Archived,
    string ArchivedAt)
{
    /// <summary>The header a retained pin carries, naming the format.</summary>
    internal const string Header = "# broiler-js-conformance retained suite pin 1";

    /// <summary>The keys a retained pin declares, all of them required.</summary>
    /// <remarks>
    /// <b>Required rather than defaulted, every one.</b> A reader that let a key be missing would
    /// let a pin be written without the field that makes it a pin, and the run would report a
    /// verified suite on the strength of whatever was left.
    /// </remarks>
    internal static IReadOnlyList<string> Keys { get; } =
    [
        "suite", "upstream", "revision", "archive", "archive-sha256", "content-sha256", "files",
        "archived", "archived-at",
    ];

    /// <summary>
    /// The one key that is required exactly when <c>archived</c> says yes, and refused otherwise.
    /// </summary>
    /// <remarks>
    /// <b>Conditional rather than always-required, because both mistakes are real.</b> A pin
    /// claiming the material is archived and not saying where has moved the search rather than
    /// ended it; a pin naming a path while claiming nothing is archived is describing a state one
    /// of its two fields does not believe in. The reader refuses both rather than picking a winner.
    /// </remarks>
    internal const string ArchivedAtKey = "archived-at";

    /// <summary>Reads a retained pin, or says why the file is not one.</summary>
    internal static RetainedSuitePin? Read(string path, out IReadOnlyList<string> complaints)
    {
        var found = new List<string>();

        if (!File.Exists(path))
        {
            complaints = [$"{path} holds no retained suite pin"];
            return null;
        }

        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var seenHeader = false;
        var number = 0;

        foreach (var raw in File.ReadAllLines(path))
        {
            number++;
            var line = raw.Trim();

            if (line.Length == 0)
            {
                continue;
            }

            if (line[0] == '#')
            {
                seenHeader |= string.Equals(line, Header, StringComparison.Ordinal);
                continue;
            }

            var space = line.IndexOf(' ', StringComparison.Ordinal);

            if (space <= 0)
            {
                found.Add($"{path}:{number}: `{line}` is not `<key> <value>`");
                continue;
            }

            var key = line[..space];

            if (!Keys.Contains(key, StringComparer.Ordinal))
            {
                found.Add($"{path}:{number}: `{key}` is not a key a retained pin declares");
                continue;
            }

            if (!values.TryAdd(key, line[(space + 1)..].Trim()))
            {
                found.Add($"{path}:{number}: `{key}` is declared twice");
            }
        }

        if (!seenHeader)
        {
            found.Add($"{path} does not carry `{Header}`");
        }

        foreach (var key in Keys.Where(key =>
                     !string.Equals(key, ArchivedAtKey, StringComparison.Ordinal) &&
                     !values.ContainsKey(key)))
        {
            found.Add($"{path} declares no `{key}`");
        }

        var claimsArchived = values.TryGetValue("archived", out var archived) &&
            string.Equals(archived, "yes", StringComparison.Ordinal);

        if (claimsArchived && !values.ContainsKey(ArchivedAtKey))
        {
            found.Add($"{path} says the suite is archived and declares no `{ArchivedAtKey}`");
        }

        if (!claimsArchived && values.ContainsKey(ArchivedAtKey))
        {
            found.Add(
                $"{path} declares `{ArchivedAtKey}` and says the suite is not archived");
        }

        if (found.Count != 0)
        {
            complaints = found;
            return null;
        }

        if (!int.TryParse(values["files"], NumberStyles.None, CultureInfo.InvariantCulture, out var files))
        {
            complaints = [$"{path}: `files {values["files"]}` is not a count"];
            return null;
        }

        // THE ARCHIVED FIELD IS READ AS A WORD AND NOT AS "ANYTHING BUT NO". A pin whose archived
        // line said `pending` would otherwise read as archived, which is the direction that
        // overclaims.
        if (values["archived"] is not ("yes" or "no"))
        {
            complaints = [$"{path}: `archived {values["archived"]}` is neither `yes` nor `no`"];
            return null;
        }

        complaints = found;

        return new RetainedSuitePin(
            values["suite"],
            values["upstream"],
            values["revision"],
            values["archive"],
            values["archive-sha256"],
            values["content-sha256"],
            files,
            claimsArchived,
            claimsArchived ? values[ArchivedAtKey] : string.Empty);
    }

    /// <summary>Why a suite this run read does not answer to this pin, or empty where it does.</summary>
    /// <remarks>
    /// <para>
    /// <b>Over what the run COMPUTED, not over what the checkout said about itself.</b> The first
    /// draft compared the retained pin against the <see cref="SuiteRevision"/> the harness had
    /// resolved - which comes from a <c>suite.pin</c> INSIDE the checkout, the artifact this whole
    /// mechanism exists to replace. It passed only because the working checkout happened to carry
    /// one somebody had generated in it, and a pristine extraction of the archived suite was
    /// refused for being called `unnamed-suite`. A retained pin that requires the suite to have
    /// already certified itself is a retained pin that certifies the self-certification.
    /// </para>
    /// <para>
    /// <b>So the name is not compared at all.</b> It is this component's label for the material,
    /// which the retained pin supplies; the content digest and the file count are properties of
    /// the bytes, and they are what a suite can disagree with. The count is not redundant with the
    /// digest: a digest says two things differ, a count says how, and a checkout that gained or
    /// lost files is a different accident from one whose bytes moved.
    /// </para>
    /// </remarks>
    internal IReadOnlyList<string> Disagrees(string computedDigest, int files)
    {
        var complaints = new List<string>();

        if (!string.Equals(computedDigest, ContentDigest, StringComparison.Ordinal))
        {
            complaints.Add(
                $"the suite read amounts to `{computedDigest}` and the retained pin names " +
                $"`{ContentDigest}`: this is not {Upstream} at {Revision}");
        }

        if (files != Files)
        {
            complaints.Add($"the suite read holds {files} files and the retained pin names {Files}");
        }

        return complaints;
    }

    /// <summary>The revision a run under this pin reports, which is this pin's own.</summary>
    /// <remarks>
    /// <b>A checkout verified against a retained pin is pinned, and its report says so.</b>
    /// Otherwise a pristine third-party extraction - which carries no pin of its own, correctly -
    /// would be scored under <see cref="ConfigurationFailure.MissingSuiteRevision"/> while being
    /// the most rigorously identified suite this harness can be given.
    /// </remarks>
    internal SuiteRevision AsRevision() => new(Suite, ContentDigest);

    /// <summary>The pin on one line, in the shape a run prints it.</summary>
    internal string Describe() =>
        $"{Suite} {Upstream}@{Revision} content sha256 {ContentDigest} over " +
        $"{Files.ToString(CultureInfo.InvariantCulture)} files" +
        (Archived ? " - archived at " + ArchivedAt : " - retrieved and hashed, NOT archived");
}
