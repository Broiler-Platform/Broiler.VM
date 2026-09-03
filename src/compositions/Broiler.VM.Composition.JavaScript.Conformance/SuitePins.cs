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
    bool Archived)
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
        "archived",
    ];

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

        foreach (var key in Keys.Where(key => !values.ContainsKey(key)))
        {
            found.Add($"{path} declares no `{key}`");
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
            string.Equals(values["archived"], "yes", StringComparison.Ordinal));
    }

    /// <summary>Why a suite this run read does not answer to this pin, or empty where it does.</summary>
    internal IReadOnlyList<string> Disagrees(SuiteRevision read, int files)
    {
        var complaints = new List<string>();

        if (!string.Equals(read.Name, Suite, StringComparison.Ordinal))
        {
            complaints.Add($"the retained pin is for `{Suite}` and the suite read is `{read.Name}`");
        }

        if (!read.IsPinned)
        {
            complaints.Add(
                $"the suite read carries no revision of its own, so there is nothing to compare " +
                $"with the retained `{ContentDigest}`");
        }
        else if (!string.Equals(read.Revision, ContentDigest, StringComparison.Ordinal))
        {
            complaints.Add(
                $"the suite read amounts to `{read.Revision}` and the retained pin names " +
                $"`{ContentDigest}`: this is not {Upstream} at {Revision}");
        }

        // The file count is checked beside the digest and is not redundant with it. A digest says
        // two things differ; a count says how, and a suite that gained or lost files is a
        // different accident from one whose bytes moved.
        if (files != Files)
        {
            complaints.Add($"the suite read holds {files} files and the retained pin names {Files}");
        }

        return complaints;
    }

    /// <summary>The pin on one line, in the shape a run prints it.</summary>
    internal string Describe() =>
        $"{Suite} {Upstream}@{Revision} content sha256 {ContentDigest} over " +
        $"{Files.ToString(CultureInfo.InvariantCulture)} files" +
        (Archived ? " - archived" : " - retrieved and hashed, NOT archived");
}
