// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using System.Globalization;
using System.Text;

namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// The JavaScript probes that carry <c>NormalizationTest.txt</c> to the end-user host: part of
/// <see cref="UnicodeTableGenerator"/>'s one write-or-check function (decision JSD-0031, slice U3).
/// </summary>
/// <remarks>
/// <para>
/// <b>Why probes and not a unit test.</b> Rule A11 lets only composition roots reference the
/// profile assembly, so no test project can call <c>normalize</c>. The vectors therefore travel as
/// guest programs under <c>src/tests/differential/</c>, run by <c>eng/run-differential.py</c>
/// through the CLI host against their retained answers and against a comparison engine. They are
/// generated here from the verified archive, in the same pass that writes the tables, so rule N22
/// holds them byte for byte to the pinned file: a probe cannot drift from the vectors.
/// </para>
/// <para>
/// <b>Two kinds.</b> A <i>vector</i> probe holds a bounded slice of one part's lines and checks the
/// UAX #15 conformance conditions for each; an <i>invariant</i> probe checks part 1's closing claim
/// - every code point not in its <c>c1</c> column is unchanged by all four forms - over one span of
/// the code space, lone surrogates included. Each prints a count as case 1 and one further case per
/// failing vector or code point, in ASCII only. The slice sizes keep every probe well inside the
/// driver's default 30-second timeout and the CLI host's default fuel.
/// </para>
/// </remarks>
internal static class UnicodeNormalizationProbes
{
    /// <summary>Where the probes are written.</summary>
    internal const string Directory = "src/tests/differential";

    /// <summary>Every probe's file name starts with this.</summary>
    internal const string Prefix = "the-unicode-normalization-";

    /// <summary>The most vectors one probe holds; a part is split into equal slices under it.</summary>
    internal const int VectorsPerProbe = 4500;

    /// <summary>The code points one invariant probe spans.</summary>
    internal const int CodePointsPerProbe = 0x20000;

    private const string Source = "ucd-17.0.0/NormalizationTest.txt";

    /// <summary>One line of the file: its line number, its part and its five columns as code points.</summary>
    internal sealed record Vector(int Line, int Part, int[][] Columns);

    /// <summary>The probe files, as repository path and text, from the verified archive.</summary>
    internal static IReadOnlyList<(string Path, string Text)> Write(IReadOnlyDictionary<string, byte[]> verified)
    {
        var (vectors, titles) = Read(UnicodePin.Decode(verified[Source]));
        var probes = new List<(string, string)>();

        foreach (var part in vectors.GroupBy(static vector => vector.Part).OrderBy(static group => group.Key))
        {
            var lines = part.ToArray();
            var slices = (lines.Length + VectorsPerProbe - 1) / VectorsPerProbe;
            var size = (lines.Length + slices - 1) / slices;

            for (var slice = 0; slice < slices; slice++)
            {
                var chunk = lines.Skip(slice * size).Take(size).ToArray();
                var name = slices == 1
                    ? $"{Prefix}part{part.Key}.js"
                    : $"{Prefix}part{part.Key}-{slice + 1}-of-{slices}.js";

                probes.Add(($"{Directory}/{name}", VectorProbe(chunk, part.Key, titles[part.Key], slice * size, lines.Length)));
            }
        }

        var listed = vectors.Where(static vector => vector.Part == 1).Select(static vector => vector.Columns[0][0]).ToHashSet();

        for (var first = 0; first <= UnicodeDataFile.MaxCodePoint; first += CodePointsPerProbe)
        {
            var last = Math.Min(first + CodePointsPerProbe - 1, UnicodeDataFile.MaxCodePoint);
            var name = $"{Prefix}invariants-{first.ToString("X6", CultureInfo.InvariantCulture)}.js";

            probes.Add(($"{Directory}/{name}", InvariantProbe(first, last, listed)));
        }

        return probes;
    }

    /// <summary>
    /// Every vector and every part title. Throws on a line that is not five columns of code
    /// points, on a part-1 line whose <c>c1</c> is not one code point, or on a part numbered out of
    /// order: nothing is generated from a file this reader does not fully understand.
    /// </summary>
    internal static (IReadOnlyList<Vector> Vectors, IReadOnlyDictionary<int, string> Titles) Read(string text)
    {
        var vectors = new List<Vector>();
        var titles = new Dictionary<int, string>();
        var part = -1;
        var number = 0;

        foreach (var raw in text.Split('\n'))
        {
            number++;

            if (raw.StartsWith("@Part", StringComparison.Ordinal))
            {
                var hash = raw.IndexOf('#', StringComparison.Ordinal);
                var next = int.Parse(raw[5..(hash < 0 ? raw.Length : hash)].Trim(), NumberStyles.None, CultureInfo.InvariantCulture);

                if (next != part + 1)
                {
                    throw new InvalidDataException($"{Source}({number}): part {next} follows part {part}");
                }

                part = next;
                titles[part] = hash < 0 ? string.Empty : raw[(hash + 1)..].Trim();
                continue;
            }

            var comment = raw.IndexOf('#', StringComparison.Ordinal);
            var line = (comment >= 0 ? raw[..comment] : raw).Trim();

            if (line.Length == 0)
            {
                continue;
            }

            var fields = line.Split(';');

            if (part < 0 || fields.Length != 6 || fields[5].Trim().Length != 0)
            {
                throw new InvalidDataException($"{Source}({number}): not a vector of five columns inside a part");
            }

            var columns = fields.Take(5).Select(field => field.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(code =>
                {
                    var value = int.Parse(code, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);

                    return value is >= 0 and <= UnicodeDataFile.MaxCodePoint and not (>= 0xD800 and <= 0xDFFF)
                        ? value
                        : throw new InvalidDataException($"{Source}({number}): {code} is not a scalar value");
                }).ToArray()).ToArray();

            if (columns.Any(static column => column.Length == 0) || (part == 1 && columns[0].Length != 1))
            {
                throw new InvalidDataException($"{Source}({number}): an empty column, or a part-1 c1 that is not one code point");
            }

            vectors.Add(new Vector(number, part, columns));
        }

        return (vectors, titles);
    }

    private static string VectorProbe(Vector[] chunk, int part, string title, int skipped, int total)
    {
        var text = Header();

        text.Append("// NormalizationTest.txt part ").Append(Number(part)).Append(" (").Append(title).Append("): vectors ")
            .Append(Number(skipped + 1)).Append(" to ").Append(Number(skipped + chunk.Length)).Append(" of ").Append(Number(total))
            .Append(", file lines ").Append(Number(chunk[0].Line)).Append(" to ").Append(Number(chunk[^1].Line)).Append(".\n");
        text.Append("// Each vector is checked against the UAX #15 conformance conditions through\n");
        text.Append("// String.prototype.normalize, twenty checks in all:\n");
        text.Append("//   c2 == NFC(c1) == NFC(c2) == NFC(c3),   c4 == NFC(c4) == NFC(c5)\n");
        text.Append("//   c3 == NFD(c1) == NFD(c2) == NFD(c3),   c5 == NFD(c4) == NFD(c5)\n");
        text.Append("//   c4 == NFKC(c1) == ... == NFKC(c5),     c5 == NFKD(c1) == ... == NFKD(c5)\n");
        text.Append("// Case 1 counts the vectors, the checks and the failing vectors; each failing vector then\n");
        text.Append("// prints one case naming its file line, its c1 as UTF-16 units in hex, and the checks it failed.\n\n");
        text.Append("var vectors = 0, checks = 0, failed = [];\n");
        text.Append("function hex(s) { var out = []; for (var i = 0; i < s.length; i++) { out.push(s.charCodeAt(i).toString(16)); } return out.join(\" \"); }\n");
        text.Append("function v(line, c1, c2, c3, c4, c5) {\n");
        text.Append("  var c = [c1, c2, c3, c4, c5], bad = [];\n");
        text.Append("  function e(form, want, from) { for (var k = 0; k < from.length; k++) { checks++; if (c[from[k]].normalize(form) !== c[want]) { bad.push(form + \"(c\" + (from[k] + 1) + \")\"); } } }\n");
        text.Append("  vectors++;\n");
        text.Append("  e(\"NFC\", 1, [0, 1, 2]); e(\"NFC\", 3, [3, 4]);\n");
        text.Append("  e(\"NFD\", 2, [0, 1, 2]); e(\"NFD\", 4, [3, 4]);\n");
        text.Append("  e(\"NFKC\", 3, [0, 1, 2, 3, 4]); e(\"NFKD\", 4, [0, 1, 2, 3, 4]);\n");
        text.Append("  if (bad.length) { failed.push(\"line \" + line + \" c1 [\" + hex(c1) + \"] \" + bad.join(\" \")); }\n");
        text.Append("}\n");

        foreach (var vector in chunk)
        {
            text.Append("v(").Append(Number(vector.Line));

            foreach (var column in vector.Columns)
            {
                text.Append(", \"");

                foreach (var codePoint in column)
                {
                    foreach (var unit in char.ConvertFromUtf32(codePoint))
                    {
                        text.Append("\\u").Append(((int)unit).ToString("X4", CultureInfo.InvariantCulture));
                    }
                }

                text.Append('"');
            }

            text.Append(");\n");
        }

        text.Append("print(\"1 part ").Append(Number(part)).Append(", file lines ").Append(Number(chunk[0].Line)).Append(" to ")
            .Append(Number(chunk[^1].Line)).Append(": \" + vectors + \" vectors, \" + checks + \" checks, \" + failed.length + \" failing\");\n");
        text.Append("for (var i = 0; i < failed.length; i++) { print((i + 2) + \" \" + failed[i]); }\n");

        return text.ToString();
    }

    private static string InvariantProbe(int first, int last, HashSet<int> listed)
    {
        var ranges = new List<(int First, int Last)>();

        for (var codePoint = first; codePoint <= last; codePoint++)
        {
            if (!listed.Contains(codePoint))
            {
                continue;
            }

            if (ranges.Count > 0 && ranges[^1].Last == codePoint - 1)
            {
                ranges[^1] = (ranges[^1].First, codePoint);
            }
            else
            {
                ranges.Add((codePoint, codePoint));
            }
        }

        var skipped = ranges.Sum(static range => range.Last - range.First + 1);
        var text = Header();

        text.Append("// NormalizationTest.txt part 1 closes with a claim: every code point that is not in its c1 column\n");
        text.Append("// is unchanged by NFC, NFD, NFKC and NFKD. This probe checks it for U+").Append(Hex(first)).Append(" to U+")
            .Append(Hex(last)).Append(", a surrogate\n");
        text.Append("// code point standing alone as the lone code unit the language passes through. The skip list is\n");
        text.Append("// part 1's c1 column inside the span, as inclusive ranges.\n");
        text.Append("// Case 1 counts what was checked and what failed; each failing code point then prints one case\n");
        text.Append("// naming it and the forms that changed it.\n\n");
        text.Append("var skip = [");

        for (var index = 0; index < ranges.Count; index++)
        {
            text.Append(index % 6 == 0 ? "\n  " : " ");
            text.Append("0x").Append(Hex(ranges[index].First)).Append(", 0x").Append(Hex(ranges[index].Last)).Append(',');
        }

        text.Append(ranges.Count > 0 ? "\n];\n" : "];\n");
        text.Append("var checked = 0, failed = [], forms = [\"NFC\", \"NFD\", \"NFKC\", \"NFKD\"], k = 0;\n");
        text.Append("for (var cp = 0x").Append(Hex(first)).Append("; cp <= 0x").Append(Hex(last)).Append("; cp++) {\n");
        text.Append("  if (k < skip.length && cp === skip[k]) { cp = skip[k + 1]; k += 2; continue; }\n");
        text.Append("  var s = String.fromCodePoint(cp), bad = [];\n");
        text.Append("  checked++;\n");
        text.Append("  for (var f = 0; f < forms.length; f++) { if (s.normalize(forms[f]) !== s) { bad.push(forms[f]); } }\n");
        text.Append("  if (bad.length) { failed.push(\"U+\" + cp.toString(16) + \" \" + bad.join(\" \")); }\n");
        text.Append("}\n");
        text.Append("print(\"1 U+").Append(Hex(first)).Append(" to U+").Append(Hex(last)).Append(": \" + checked + \" code points checked in four forms, ")
            .Append(Number(skipped)).Append(" left to part 1, \" + failed.length + \" failing\");\n");
        text.Append("for (var i = 0; i < failed.length; i++) { print((i + 2) + \" \" + failed[i]); }\n");

        return text.ToString();
    }

    private static StringBuilder Header()
    {
        var text = new StringBuilder();

        text.Append("// SPDX-FileCopyrightText: 2026 Broiler Platform contributors\n");
        text.Append("// SPDX-License-Identifier: Apache-2.0 AND Unicode-3.0\n");
        text.Append("//\n");
        text.Append("// GENERATED by UnicodeTableGenerator (src/tests/Broiler.VM.Architecture.Tests) from the Unicode\n");
        text.Append("// 17.0.0 NormalizationTest.txt that src/tests/unicode/pins/unicode.pin names (JSD-0031, slice U3).\n");
        text.Append("// Rule N22 regenerates this file and compares it byte for byte: change the generator, never this\n");
        text.Append("// file, and run the architecture tests with BROILER_UNICODE_WRITE=1. The vectors are Unicode data\n");
        text.Append("// under the Unicode License v3, whose text THIRD_PARTY_NOTICES.md carries.\n");
        text.Append("//\n");

        return text;
    }

    private static string Number(int value) => value.ToString(CultureInfo.InvariantCulture);

    private static string Hex(int value) => value.ToString("X4", CultureInfo.InvariantCulture);
}
