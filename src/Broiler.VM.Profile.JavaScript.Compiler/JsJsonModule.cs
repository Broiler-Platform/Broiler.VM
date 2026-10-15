// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           0
// Human-reviewed:   0/5
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Broiler.ECMAScript;
using Broiler.ECMAScript.Text;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// Reads a JSON module's text and writes the synthetic module it is: one <c>default</c> export
/// whose value is the parsed document.
/// </summary>
/// <remarks>
/// <para>
/// <b>The language's JSON module is <c>CreateDefaultExportSyntheticModule(JSON.parse(source))</c>,
/// and this writes that module as source</b> rather than adding a second kind of module record to
/// the format: <c>export default (</c> a literal <c>);</c>. Every value JSON has is a literal of the
/// language, so the module's evaluation builds exactly the value <c>JSON.parse</c> would - fresh
/// ordinary objects and arrays, extensible, with the intrinsic prototypes, created once per module
/// instance and so shared by every importer of it.
/// </para>
/// <para>
/// <b>Every key is written computed</b>, <c>["k"]: v</c>, because a literal is not a reviver-free
/// <c>JSON.parse</c> in one place: <c>{"__proto__": v}</c> as a literal sets the prototype, and the
/// document's key is an own property. A computed key is never the prototype form. A key written
/// twice keeps its first position and its last value, which is what both readings do.
/// </para>
/// <para>
/// <b>The text is checked against the JSON grammar here, and only here.</b> A text that is not
/// JSON is refused before the module is parsed as JavaScript, so a document that happens to be a
/// valid expression - <c>{a: 1}</c>, <c>'x'</c>, <c>0x10</c> - is refused as the load refuses it
/// rather than evaluated. Strings are written back with every code unit outside printable ASCII
/// escaped, so a lone surrogate or a line separator in the document survives unchanged.
/// </para>
/// <para>
/// <b>The reading is the shared library's <see cref="JsonReader"/></b> (JSD-0071), the reader
/// <c>JSON.parse</c> uses, under this translation's own depth limit and with no meter: the
/// translation runs at compile time, where nothing is charged. The writer is this family's.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=3E7D2B
// Broiler-Human:        PENDING
internal static class JsJsonModule
{
    /// <summary>The deepest a document may nest before the translation declines it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=81D030
    // Broiler-Human:        PENDING
    internal const int MaximumDepth = 1024;

    /// <summary>
    /// Writes the module source for one JSON text, or answers why the text is not JSON.
    /// </summary>
    /// <param name="json">The module's text.</param>
    /// <param name="source">The synthetic module's source, when the text is JSON.</param>
    /// <param name="error">Why it is not, when it is not.</param>
    /// <param name="tooDeep">Whether the refusal is this front end's nesting ceiling.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D878CC
    // Broiler-Human:        PENDING
    internal static bool TryTranslate(string json, out string source, out string error, out bool tooDeep)
    {
        var reader = new JsonReader(json, new DepthLimit(MaximumDepth));
        var written = new StringBuilder(json.Length + 32);
        written.Append("export default (");

        // One entry per open container: whether it is an object, and whether a member or an
        // element has been written in it, which is when the next one needs its separator.
        var inObject = new List<bool>();
        var separated = new List<bool>();
        tooDeep = false;

        while (true)
        {
            var status = reader.Read();

            if (status != ResultStatus.Completed)
            {
                tooDeep = status == ResultStatus.LimitExceeded;
                source = string.Empty;
                error = Describe(json, reader.Failure);
                return false;
            }

            var token = reader.Token;

            if (token == JsonToken.EndOfText)
            {
                break;
            }

            if (token is JsonToken.EndObject or JsonToken.EndArray)
            {
                written.Append(token == JsonToken.EndObject ? '}' : ']');
                inObject.RemoveAt(inObject.Count - 1);
                separated.RemoveAt(separated.Count - 1);
                continue;
            }

            // A member's name opens a member; any other value opens one only in an array, since
            // in an object it is the value of the name just written.
            var opensEntry = token == JsonToken.PropertyName ||
                (inObject.Count > 0 && !inObject[^1]);

            if (opensEntry)
            {
                if (separated[^1])
                {
                    written.Append(", ");
                }

                separated[^1] = true;
            }

            switch (token)
            {
                case JsonToken.PropertyName:
                    written.Append('[');
                    WriteString(written, reader.StringValue);
                    written.Append("]: ");
                    break;

                case JsonToken.BeginObject:
                    written.Append('{');
                    inObject.Add(true);
                    separated.Add(false);
                    break;

                case JsonToken.BeginArray:
                    written.Append('[');
                    inObject.Add(false);
                    separated.Add(false);
                    break;

                case JsonToken.String:
                    WriteString(written, reader.StringValue);
                    break;

                case JsonToken.Number:
                    // A number's text is a literal of the language as it stands.
                    written.Append(json, reader.TokenStart, reader.TokenEnd - reader.TokenStart);
                    break;

                case JsonToken.True:
                    written.Append("true");
                    break;

                case JsonToken.False:
                    written.Append("false");
                    break;

                default:
                    written.Append("null");
                    break;
            }
        }

        written.Append(");\n");
        source = written.ToString();
        error = string.Empty;
        return true;
    }

    /// <summary>Writes one string as a double-quoted literal of printable ASCII.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=0624D9
    // Broiler-Human:        PENDING
    private static void WriteString(StringBuilder written, string value)
    {
        written.Append('"');

        foreach (var unit in value)
        {
            if (unit is >= ' ' and <= '~' && unit != '"' && unit != '\\')
            {
                written.Append(unit);
            }
            else
            {
                written.Append("\\u").Append(((int)unit).ToString("X4", CultureInfo.InvariantCulture));
            }
        }

        written.Append('"');
    }

    /// <summary>Why the reader refused the text, in this front end's words, with the offset it stopped at.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=B82F94
    // Broiler-Human:        PENDING
    private static string Describe(string json, Failure failure)
    {
        var reason = failure.MessageId switch
        {
            "json.too-deep" => "the document nests deeper than " + MaximumDepth + " levels",
            "json.unterminated-string" => "a string is not closed",
            "json.control-character" => "a string holds an unescaped control character",
            "json.bad-escape" => "a string holds an escape JSON does not define",
            "json.bad-unicode-escape" => "a `\\u` escape is four hexadecimal digits",
            _ when failure.Position >= json.Length => "the text ends where the grammar expects more",
            _ => "the grammar has no place for `" + json[failure.Position] + "`",
        };

        return reason + " (at offset " + failure.Position.ToString(CultureInfo.InvariantCulture) + ")";
    }
}
