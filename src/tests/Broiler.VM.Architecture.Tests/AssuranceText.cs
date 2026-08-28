namespace Broiler.VM.Architecture.Tests;

/// <summary>
/// A file split into lines and the separators between them, so that a rewrite can change one line
/// and put the file back together byte for byte.
/// </summary>
/// <remarks>
/// Splitting on a single detected newline and rejoining with it would silently normalize a file
/// that mixes endings, which is a behavioural change to a file the generator is only supposed to
/// add comments to. Each separator is therefore carried beside the line it terminates, and a line
/// the generator does not touch is reassembled exactly as it was read.
/// </remarks>
internal sealed class AssuranceText
{
    private readonly List<string> lines = [];
    private readonly List<string> separators = [];

    internal AssuranceText(string text)
    {
        var start = 0;

        for (var index = 0; index < text.Length; index++)
        {
            var separator = text[index] switch
            {
                '\r' when index + 1 < text.Length && text[index + 1] == '\n' => "\r\n",
                '\r' => "\r",
                '\n' => "\n",
                _ => null,
            };

            if (separator is null)
            {
                continue;
            }

            this.lines.Add(text[start..index]);
            this.separators.Add(separator);
            index += separator.Length - 1;
            start = index + 1;
        }

        // The trailing fragment after the last newline, which is empty for a file that ends in one.
        this.lines.Add(text[start..]);
        this.separators.Add(string.Empty);
    }

    internal int Count => this.lines.Count;

    internal string this[int index]
    {
        get => this.lines[index];
        set => this.lines[index] = value;
    }

    /// <summary>The newline this file predominantly uses, for lines the generator inserts.</summary>
    internal string NewLine =>
        this.separators.Where(static separator => separator.Length > 0).FirstOrDefault() ?? "\n";

    internal void Insert(int index, IEnumerable<string> inserted)
    {
        var materialized = inserted.ToArray();

        this.lines.InsertRange(index, materialized);
        this.separators.InsertRange(index, Enumerable.Repeat(NewLine, materialized.Length));
    }

    internal void RemoveRange(int index, int count)
    {
        this.lines.RemoveRange(index, count);
        this.separators.RemoveRange(index, count);
    }

    internal string Render()
    {
        var builder = new System.Text.StringBuilder();

        for (var index = 0; index < this.lines.Count; index++)
        {
            builder.Append(this.lines[index]).Append(this.separators[index]);
        }

        return builder.ToString();
    }

    /// <summary>The leading whitespace of a line, so a rewritten line keeps its indentation.</summary>
    internal static string IndentOf(string line) =>
        line[..(line.Length - line.TrimStart().Length)];
}
