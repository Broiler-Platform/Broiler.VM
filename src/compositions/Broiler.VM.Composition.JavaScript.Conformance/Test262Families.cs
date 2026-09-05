// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Globalization;

namespace Broiler.VM.Composition.JavaScript.Conformance;

/// <summary>One row of a tally: a name, how often a run met it, and one case that did.</summary>
/// <param name="Name">The family or the dimension. Never blank.</param>
/// <param name="Count">How many variants of this run landed here.</param>
/// <param name="Example">The ordinally first case, so the row is the same on every machine.</param>
internal sealed record Test262TallyRow(string Name, int Count, string Example);

/// <summary>
/// The <c>unsupported</c> families and the exhausted dimensions a run met, named rather than
/// counted.
/// </summary>
/// <remarks>
/// <para>
/// <b>This is the clause that makes an <c>unsupported</c> column readable rather than a number.</b>
/// The workload roadmap's section 3.3 says refusal BY NAME is the property this programme must not
/// spend, and JSW-10's gate asks for every <c>unsupported</c> family to be named. A column that said
/// only "forty thousand" would tell a reader that the manifest declines a lot and nothing about
/// what; a table that says <c>a class declaration</c>, <c>a generator function</c> and
/// <c>a spread element</c> with a count and a path each tells the next stage which family to admit
/// first.
/// </para>
/// <para>
/// <b>There is no static list of families and there deliberately is not one.</b> Every row is built
/// from what this run actually met, so a family the front end has stopped refusing stops appearing
/// the moment it stops being refused - which is exactly the movement JSW-5 exists to produce. A
/// hand-kept list would have to be edited to show that movement, and a list nobody edited would go
/// on naming a family that had already gone.
/// </para>
/// <para>
/// <b>The exhausted dimensions are tallied the same way and are not folded into the failures.</b> An
/// absence, a refusal and a failure are three different answers, and "we did not wait long enough"
/// is a fourth; a run whose failed column silently carried it would be a run whose failures nobody
/// can act on.
/// </para>
/// </remarks>
internal static class Test262Families
{
    /// <summary>The suffix the front end's manifest refusals are written with.</summary>
    /// <remarks>
    /// The one the front end actually emits - <c>JsParser</c> composes every named refusal as the
    /// construct followed by this - so the family is the front end's own words rather than a
    /// vocabulary this harness invented beside it. A second vocabulary here would describe one
    /// construct in two ways and a reader comparing the two would be comparing nothing.
    /// </remarks>
    internal const string ManifestSuffix = " is not admitted by the declared feature manifest";

    /// <summary>The shorter suffix the lowering uses where the manifest is already implied.</summary>
    /// <remarks>
    /// Searched only after <see cref="ManifestSuffix"/>, because that one begins with this one and a
    /// reader that tried the short form first would cut every message at the same place and produce
    /// one family called <c>a class declaration</c> and another called
    /// <c>a class declaration by the declared feature manifest</c>.
    /// </remarks>
    internal const string ShortSuffix = " is not admitted";

    /// <summary>The family a refusal message names, or the whole message where it names none.</summary>
    /// <remarks>
    /// <b>The message and not the diagnostic code.</b> Every construct outside the manifest is
    /// refused under one code, so grouping by code would produce a table with exactly one row in it.
    /// The name is what the front end wrote, and a message this reader does not recognise the shape
    /// of becomes a family of its own rather than being dropped: an unrecognised refusal is
    /// something a reader must see, not something to round down.
    /// </remarks>
    internal static string Of(string message)
    {
        var flattened = message.Replace('\r', ' ').Replace('\n', ' ').Trim();
        var index = flattened.IndexOf(ManifestSuffix, StringComparison.Ordinal);

        if (index > 0)
        {
            return flattened[..index];
        }

        index = flattened.IndexOf(ShortSuffix, StringComparison.Ordinal);

        return index > 0 ? flattened[..index] : flattened;
    }

    /// <summary>Tallies observations by name, most-met first.</summary>
    /// <remarks>
    /// <para>
    /// <b>Ordered by count and then by name, and the example is the ordinally first path.</b> A
    /// merge adds shard reports in whatever order a directory listing produced, so a table whose
    /// example was "the first one seen" would differ between two merges of the same run and a
    /// reviewer diffing two transcripts would be reading noise.
    /// </para>
    /// <para>
    /// A blank name is refused by being replaced rather than dropped, for the same reason an
    /// unrecognised message becomes its own family: a row that vanished would make the tally
    /// disagree with the totals it is a breakdown of.
    /// </para>
    /// </remarks>
    internal static IReadOnlyList<Test262TallyRow> Tally(IEnumerable<(string Name, string Case)> met)
    {
        var counts = new Dictionary<string, (int Count, string Example)>(StringComparer.Ordinal);

        foreach (var (rawName, one) in met)
        {
            var name = rawName.Length == 0 ? "(unnamed)" : rawName;

            if (counts.TryGetValue(name, out var seen))
            {
                counts[name] = (
                    seen.Count + 1,
                    string.CompareOrdinal(one, seen.Example) < 0 ? one : seen.Example);
            }
            else
            {
                counts[name] = (1, one);
            }
        }

        return counts
            .Select(static entry => new Test262TallyRow(entry.Key, entry.Value.Count, entry.Value.Example))
            .OrderByDescending(static row => row.Count)
            .ThenBy(static row => row.Name, StringComparer.Ordinal)
            .ToArray();
    }

    /// <summary>The lines a run prints for one tally, or the one line that says it met none.</summary>
    internal static IReadOnlyList<string> Describe(
        IReadOnlyList<Test262TallyRow> rows,
        string heading,
        string empty)
    {
        if (rows.Count == 0)
        {
            return ["# " + empty];
        }

        var lines = new List<string>(rows.Count + 1)
        {
            "# " + heading + ": " + rows.Count.ToString(CultureInfo.InvariantCulture) +
                " distinct, " +
                rows.Sum(static row => row.Count).ToString(CultureInfo.InvariantCulture) +
                " variants",
        };

        foreach (var row in rows)
        {
            lines.Add(
                "#   " + row.Count.ToString(CultureInfo.InvariantCulture).PadLeft(7) + "  " +
                row.Name + "  e.g. " + row.Example);
        }

        return lines;
    }
}
