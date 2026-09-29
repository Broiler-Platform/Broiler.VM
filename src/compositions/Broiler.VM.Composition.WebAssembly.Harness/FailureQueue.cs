using Broiler.VM.Profile.WebAssembly;
using System.Globalization;
using System.Text;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// The failure manifest: the scripts of the suite that fail, as a queue of work rather than an
/// allow-list of excuses.
/// </summary>
/// <remarks>
/// <para>
/// <b>What the roadmap asks of it.</b> "The failure manifest is a queue, not an allow-list. A path leaves it
/// only after a minimal repository regression exists, the focused reproduction passes, the affected shard
/// passes, and the record is updated. A hand-written entry that a run does not confirm does not survive."
/// An entry is a script of the suite, named as every answer line names it, with the count of its commands
/// that failed when the entry was written.
/// </para>
/// <para>
/// <b>What a run holds it to.</b> Every listed script the run ran must still fail: one that passes now is
/// reported, because it leaves the queue only in a change that carries its regression and updates the
/// record, never by being quietly left there. A listed name the run cannot confirm - one the suite does not
/// hold, or one the selection did not select - does not survive. And a script the run saw fail that the
/// queue does not list is reported too: a new failure joins the queue by a run writing it, not by a hand.
/// A shard run judges only the scripts it ran; the merge judges the rest.
/// </para>
/// <para>
/// <b>The queue is bound to the suite revision and the manifest it was written for</b>, as the ratchet is,
/// and a run of another is not compared with it.
/// </para>
/// </remarks>
internal static class FailureQueue
{
    private static string Manifest => WebAssemblyProfile.SliceManifest.ToString();

    /// <summary>A queue as read.</summary>
    internal sealed record Queue(string Revision, string Manifest, IReadOnlyDictionary<string, int> Entries);

    /// <summary>Reads a queue's text; answers why not when it cannot.</summary>
    internal static bool TryParse(string text, out Queue queue, out string failure)
    {
        queue = null!;
        string? revision = null, manifest = null;
        var entries = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var line in text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').Where(static line => line.Length > 0 && line[0] != '#'))
        {
            var parts = line.Split(' ');

            switch (parts)
            {
                case ["revision", var value]:
                    revision = value;
                    break;
                case ["manifest", var value]:
                    manifest = value;
                    break;
                case ["path", var script, "failing", var counted] when int.TryParse(counted, NumberStyles.None, CultureInfo.InvariantCulture, out var count):
                    if (!entries.TryAdd(script, count))
                    {
                        failure = $"the queue lists {script} twice";
                        return false;
                    }

                    break;
                default:
                    failure = $"the queue holds the line \"{line}\", which is not one a queue holds";
                    return false;
            }
        }

        if (revision is null || manifest is null)
        {
            failure = "the queue does not state the revision and the manifest it was written for";
            return false;
        }

        queue = new Queue(revision, manifest, entries);
        failure = string.Empty;
        return true;
    }

    /// <summary>
    /// Every way this run disagrees with the queue: the scripts it ran are <paramref name="ran"/>, the
    /// scripts the selection selected are <paramref name="selected"/>, and <paramref name="failing"/> counts
    /// each ran script's failing commands.
    /// </summary>
    internal static List<string> Check(
        Queue queue, string revision, IReadOnlySet<string> selected, IReadOnlySet<string> ran, IReadOnlyDictionary<string, int> failing)
    {
        var violations = new List<string>();

        if (!string.Equals(queue.Revision, revision, StringComparison.Ordinal) || !string.Equals(queue.Manifest, Manifest, StringComparison.Ordinal))
        {
            violations.Add($"the queue was written for revision {queue.Revision} and manifest {queue.Manifest}, and this run is of {revision} and {Manifest}, so it is not compared");
            return violations;
        }

        foreach (var (script, count) in queue.Entries.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            if (!selected.Contains(script))
            {
                violations.Add($"the queue lists {script}, which this run's selection does not hold, so the entry is not confirmed and does not survive");
                continue;
            }

            if (!ran.Contains(script))
            {
                // Another shard's script: the merge judges it.
                continue;
            }

            if (failing.GetValueOrDefault(script) == 0)
            {
                violations.Add($"the queue lists {script}, and every command of it passes now: it leaves the queue in a change carrying its regression");
            }
        }

        foreach (var (script, count) in failing.Where(pair => pair.Value > 0 && ran.Contains(pair.Key)).OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            if (!queue.Entries.ContainsKey(script))
            {
                violations.Add($"{script} fails {Text(count)} commands, and the queue does not list it: a failure joins the queue by a run writing it");
            }
        }

        return violations;
    }

    /// <summary>A queue of every failing script of a whole selection, as text.</summary>
    internal static string Write(string revision, IReadOnlyDictionary<string, int> failing)
    {
        var text = new StringBuilder()
            .Append("# The WebAssembly profile's failure queue: every script of the suite with a failing command, and how many.\n")
            .Append("# Written by the harness root's --spec or --merge with --write-queue from a whole selection; held with --queue.\n")
            .Append("# A path leaves it only in a change carrying its regression. See FailureQueue.cs.\n")
            .Append("revision ").Append(revision).Append('\n')
            .Append("manifest ").Append(Manifest).Append('\n');

        foreach (var (script, count) in failing.Where(static pair => pair.Value > 0).OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            text.Append("path ").Append(script).Append(" failing ").Append(Text(count)).Append('\n');
        }

        return text.ToString();
    }

    private static string Text(int value) => value.ToString(CultureInfo.InvariantCulture);
}
