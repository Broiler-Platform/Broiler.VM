// SJL-13.1 task 1.1, sub-area T, probe T32 (Broiler.VM at 4cf9ca4): every field of every token SliceTokenizer
// gives a text, as the slice path builds its stream (SliceSourceCompiler: new SliceTokenizer(source), HtmlLikeComments
// and StructuredTemplates at their defaults, off), which is the stream part F of the step-1 rule compares. One line per
// token; "source" is the text between Offset and End, which is what RawText is for every kind but an escaped
// identifier. A probe, not a tool: built only by the probe runner, never committed as a project.
using System;
using System.Globalization;
using System.IO;
using System.Text;
using Broiler.VM.Profile.JavaScript.Compiler;

internal static class Program
{
    private static int Main(string[] args)
    {
        var output = new StringBuilder();
        foreach (var path in args)
        {
            var text = File.ReadAllText(path, new UTF8Encoding(false, true));
            output.Append("=== ").Append(Path.GetFileName(path)).Append('\n');
            var tokenizer = new SliceTokenizer(text);
            var tokens = tokenizer.Tokenize();
            for (var i = 0; i < tokens.Length; i++)
            {
                var t = tokens[i];
                var source = t.Offset >= 0 && t.End >= t.Offset && t.End <= text.Length ? text[t.Offset..t.End] : "?";
                output.Append(i.ToString(CultureInfo.InvariantCulture))
                    .Append('\t').Append(t.Kind.ToString())
                    .Append("\traw=").Append(Quote(t.RawText))
                    .Append("\tsource=").Append(Quote(source))
                    .Append("\traw-is-source=").Append(string.Equals(t.RawText, source, StringComparison.Ordinal) ? "yes" : "no")
                    .Append("\tstring=").Append(Quote(t.StringValue))
                    .Append("\tnumber=").Append(t.NumericValue.ToString("R", CultureInfo.InvariantCulture))
                    .Append("\tat=").Append(t.Line.ToString(CultureInfo.InvariantCulture)).Append(':').Append(t.Column.ToString(CultureInfo.InvariantCulture))
                    .Append("\tspan=").Append(t.Offset.ToString(CultureInfo.InvariantCulture)).Append("..").Append(t.End.ToString(CultureInfo.InvariantCulture))
                    .Append("\tnl=").Append(t.PrecededByLineTerminator ? "true" : "false")
                    .Append("\toctal=").Append(t.IsLegacyOctal ? "true" : "false")
                    .Append("\tescaped=").Append(t.IsEscaped ? "true" : "false")
                    .Append("\tkeyword-table=").Append(SliceTokenizerKeywordKind(t))
                    .Append('\n');
            }

            foreach (var d in tokenizer.Diagnostics)
            {
                output.Append("diagnostic\t").Append(((int)d.Code).ToString(CultureInfo.InvariantCulture))
                    .Append(':').Append(d.Line.ToString(CultureInfo.InvariantCulture))
                    .Append(':').Append(d.Column.ToString(CultureInfo.InvariantCulture))
                    .Append('\t').Append(Quote(d.Message)).Append('\n');
            }
        }

        var stdout = Console.OpenStandardOutput();
        var bytes = new UTF8Encoding(false).GetBytes(output.ToString());
        stdout.Write(bytes, 0, bytes.Length);
        return 0;
    }

    // The kind the slice tokenizer's own table gives the token's RawText, which it does not ask for an escaped
    // identifier (SliceTokenizer.cs:1055). KeywordKind is internal, so the table is not called: an identifier's
    // RawText is re-tokenized alone, which reaches the same table for an unescaped word.
    private static string SliceTokenizerKeywordKind(SliceToken token)
    {
        if (token.Kind != SliceTokenKind.Identifier || token.RawText.Length == 0 || token.RawText[0] == '#')
            return "-";
        var alone = new SliceTokenizer(token.RawText).Tokenize();
        return alone.Length > 0 ? alone[0].Kind.ToString() : "?";
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
