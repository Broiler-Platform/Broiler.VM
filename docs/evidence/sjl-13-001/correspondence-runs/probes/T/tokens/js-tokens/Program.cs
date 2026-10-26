// SJL-13.1 task 1.1, sub-area T, probe T32 (Broiler.JS at d6795fc8): every field of every token FastScanner gives a
// text under FastParserOptions.Script (the scanner's default constructor), the stream the parser reads. One line per
// token. A probe, not a tool: built only by the probe runner, never committed as a project.
using System;
using System.Globalization;
using System.IO;
using System.Text;
using Broiler.JavaScript.Ast.Misc;
using Broiler.JavaScript.Parser;

internal static class Program
{
    private static int Main(string[] args)
    {
        var output = new StringBuilder();
        foreach (var path in args)
        {
            var text = File.ReadAllText(path, new UTF8Encoding(false, true));
            output.Append("=== ").Append(Path.GetFileName(path)).Append('\n');
            try
            {
                var scanner = new FastScanner(new FastPool(), new StringSpan(text));
                for (var i = 0; i < 10_000; i++)
                {
                    var t = scanner.Token;
                    if (t == null || t.Type == TokenTypes.EOF)
                        break;
                    output.Append(i.ToString(CultureInfo.InvariantCulture))
                        .Append('\t').Append(t.Type.ToString())
                        .Append("\tspan=").Append(Quote(t.Span.Value))
                        .Append("\tat=").Append(t.Span.Offset.ToString(CultureInfo.InvariantCulture)).Append("..")
                        .Append((t.Span.Offset + t.Span.Length).ToString(CultureInfo.InvariantCulture))
                        .Append("\tcooked=").Append(Quote(t.CookedText))
                        .Append("\tkeyword=").Append(t.IsKeyword ? "true" : "false").Append('/').Append(t.Keyword.ToString())
                        .Append("\tcontextual=").Append(t.ContextualKeyword.ToString())
                        .Append("\tescaped-reserved=").Append(t.IsEscapedReservedWord ? "true" : "false")
                        .Append("\tnumber=").Append(t.Number.ToString("R", CultureInfo.InvariantCulture))
                        .Append("\tstart=").Append(t.Start.Line.ToString(CultureInfo.InvariantCulture)).Append(':').Append(t.Start.Column.ToString(CultureInfo.InvariantCulture))
                        .Append('\n');
                    scanner.ConsumeToken();
                }
            }
            catch (Exception exception)
            {
                output.Append("threw\t").Append(exception.GetType().Name).Append('\t').Append(Quote(exception.Message)).Append('\n');
            }
        }

        var stdout = Console.OpenStandardOutput();
        var bytes = new UTF8Encoding(false).GetBytes(output.ToString());
        stdout.Write(bytes, 0, bytes.Length);
        return 0;
    }

    private static string Quote(string? value)
    {
        if (value == null)
            return "null";
        var builder = new StringBuilder(value.Length + 2).Append('"');
        foreach (var c in value)
        {
            if (c == '\\') builder.Append("\\\\");
            else if (c == '"') builder.Append("\\\"");
            else if (c >= 0x20 && c < 0x7F) builder.Append(c);
            else builder.Append("\\u").Append(((int)c).ToString("X4", CultureInfo.InvariantCulture));
        }

        return builder.Append('"').ToString();
    }
}
