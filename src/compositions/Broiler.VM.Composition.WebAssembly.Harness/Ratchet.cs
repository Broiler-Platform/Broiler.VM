using Broiler.VM.Profile.WebAssembly;
using System.Globalization;
using System.Text;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// The conformance ratchet: the passing totals of the two assertion families WA-4 scores, set by the
/// first run that admits them, and the floor no later run under the same revision and limits falls
/// below.
/// </summary>
/// <remarks>
/// <para>
/// <b>What the roadmap asks of it.</b> "The first per-family totals admitted for a manifest by the
/// milestone that scores it are the floor. No later run of that manifest regresses against them." WA-4's
/// gate has the run of the malformed and invalid families set the ratchet "for those two families and for
/// no others", and admitting a total is a measurement discipline, not the ledger's acceptance. So the
/// file names those two families and nothing else, and a file naming another is refused rather than
/// read.
/// </para>
/// <para>
/// <b>A floor is compared only where it was set.</b> It records the suite revision, the feature manifest
/// and the effective limit vector of the run that set it, and a run that differs in any of them is not
/// compared: a suite that added commands would otherwise read as a regression, and ceilings a host
/// tightened would lower the bar without anyone deciding to. Re-basing is writing the file again under
/// the new revision, and the old floor stays in the file's history.
/// </para>
/// <para>
/// <b>It never lowers.</b> Writing over a floor of the same revision, manifest and limits with a lower
/// total is refused; an equal or higher one raises it.
/// </para>
/// </remarks>
internal static class Ratchet
{
    /// <summary>The families the ratchet holds, and no others.</summary>
    internal static readonly string[] Families = ["malformed", "invalid"];

    private static string Manifest => WebAssemblyProfile.SliceManifest.ToString();

    /// <summary>Holds this run's totals to the floor in <paramref name="path"/>; answers the lane's exit code.</summary>
    internal static int Check(string path, string revision, string limits, IReadOnlyDictionary<string, SpecSuite.FamilyTotals> totals)
    {
        if (!TryRead(path, out var floor, out var failure))
        {
            Console.WriteLine($"# spec: CONFIGURATION FAILURE ratchet {path}: {failure}");
            return 4;
        }

        if (Incomparable(floor, revision, limits) is { } reason)
        {
            Console.WriteLine($"# spec: RATCHET {path} {reason}, and a floor is compared only where it was set");
            return 1;
        }

        var regressed = 0;
        var held = new List<string>();

        foreach (var family in Families)
        {
            var now = totals.TryGetValue(family, out var counted) ? counted.Passed : 0;
            var least = floor.Floors[family];

            if (now < least)
            {
                Console.WriteLine($"# spec: RATCHET family {family} passed {Text(now)}, below its floor of {Text(least)}");
                regressed++;
            }

            held.Add($"{family} {Text(now)} against {Text(least)}");
        }

        Console.WriteLine($"# spec: ratchet {path}: {string.Join(", ", held)}; {(regressed == 0 ? "held" : "REGRESSED")}");
        return regressed == 0 ? 0 : 1;
    }

    /// <summary>Sets or raises the floor in <paramref name="path"/> from this run's totals; answers the lane's exit code.</summary>
    internal static int Write(string path, string revision, string limits, IReadOnlyDictionary<string, SpecSuite.FamilyTotals> totals)
    {
        if (File.Exists(path) && TryRead(path, out var existing, out _) && Incomparable(existing, revision, limits) is null)
        {
            foreach (var family in Families)
            {
                var now = totals.TryGetValue(family, out var counted) ? counted.Passed : 0;

                if (now < existing.Floors[family])
                {
                    Console.WriteLine(
                        $"# spec: RATCHET not written: family {family} passed {Text(now)}, below the floor of {Text(existing.Floors[family])} " +
                        "already set under this revision, manifest and limits, and a ratchet never lowers");
                    return 1;
                }
            }
        }

        var text = new StringBuilder()
            .Append("# The WebAssembly profile's conformance ratchet: the passing totals of the families WA-4 scores,\n")
            .Append("# the floor no run of the same revision, manifest and limits falls below. Written by the harness\n")
            .Append("# root's --spec lane with --write-ratchet; read with --ratchet. See Ratchet.cs.\n")
            .Append("revision ").Append(revision).Append('\n')
            .Append("manifest ").Append(Manifest).Append('\n')
            .Append("limits ").Append(limits).Append('\n');

        foreach (var family in Families)
        {
            var counted = totals.TryGetValue(family, out var value) ? value : default;
            text.Append("family ").Append(family)
                .Append(" passed ").Append(Text(counted.Passed))
                .Append(" of ").Append(Text(counted.Executed)).Append(" executed")
                .Append(", ").Append(Text(counted.Selected)).Append(" selected\n");
        }

        File.WriteAllText(path, text.ToString(), new UTF8Encoding(false));
        Console.WriteLine($"# spec: ratchet written to {path}");
        return 0;
    }

    private sealed record Floor(string Revision, string Manifest, string Limits, Dictionary<string, int> Floors);

    private static string? Incomparable(Floor floor, string revision, string limits) =>
        !string.Equals(floor.Revision, revision, StringComparison.Ordinal) ? $"was set under revision {floor.Revision}, and this run is of {revision}"
        : !string.Equals(floor.Manifest, Manifest, StringComparison.Ordinal) ? $"was set for manifest {floor.Manifest}, and this run scores {Manifest}"
        : !string.Equals(floor.Limits, limits, StringComparison.Ordinal) ? $"was set under the limits {floor.Limits}, and this run is under {limits}"
        : null;

    /// <summary>Reads a ratchet file, refusing one that names a family other than the two, or misses one.</summary>
    private static bool TryRead(string path, out Floor floor, out string failure)
    {
        floor = new Floor(string.Empty, string.Empty, string.Empty, new Dictionary<string, int>(StringComparer.Ordinal));

        if (!File.Exists(path))
        {
            failure = "no such file";
            return false;
        }

        string? revision = null, manifest = null, limits = null;
        var floors = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var line in File.ReadAllLines(path).Where(static line => line.Length > 0 && line[0] != '#'))
        {
            var space = line.IndexOf(' ', StringComparison.Ordinal);
            var (key, value) = space < 0 ? (line, string.Empty) : (line[..space], line[(space + 1)..]);

            switch (key)
            {
                case "revision":
                    revision = value;
                    break;
                case "manifest":
                    manifest = value;
                    break;
                case "limits":
                    limits = value;
                    break;
                case "family":
                    var parts = value.Split(' ');

                    if (parts.Length < 3 || parts[1] != "passed" || !int.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out var passed))
                    {
                        failure = $"the line \"{line}\" is not a family's floor";
                        return false;
                    }

                    if (!Families.Contains(parts[0], StringComparer.Ordinal))
                    {
                        failure = $"it names the family {parts[0]}, and the ratchet holds {string.Join(" and ", Families)} and no others";
                        return false;
                    }

                    floors[parts[0]] = passed;
                    break;
                default:
                    failure = $"the line \"{line}\" is not one a ratchet holds";
                    return false;
            }
        }

        if (revision is null || manifest is null || limits is null || Families.Any(family => !floors.ContainsKey(family)))
        {
            failure = "it does not state a revision, a manifest, the limits and a floor for each family it holds";
            return false;
        }

        floor = new Floor(revision, manifest, limits, floors);
        failure = string.Empty;
        return true;
    }

    private static string Text(int value) => value.ToString(CultureInfo.InvariantCulture);
}
