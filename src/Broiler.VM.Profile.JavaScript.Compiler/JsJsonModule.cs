// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   15
// Annotated:        15/15
// Exempt:           3
// Human-reviewed:   0/15
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B49F8D
    // Broiler-Human:        PENDING
    internal static bool TryTranslate(string json, out string source, out string error, out bool tooDeep)
    {
        var reader = new Reader(json);
        var written = new System.Text.StringBuilder(json.Length + 32);
        written.Append("export default (");

        var read = reader.Value(written, 0) && reader.End();
        tooDeep = reader.TooDeep;

        if (!read)
        {
            source = string.Empty;
            error = reader.Error;
            return false;
        }

        written.Append(");\n");
        source = written.ToString();
        error = string.Empty;
        return true;
    }

    /// <summary>A cursor over one JSON text.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=54E035
    // Broiler-Human:        PENDING
    private sealed class Reader(string text)
    {
        private int position;

        /// <summary>Why the text was refused.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=78F8BF
        // Broiler-Human:        PENDING
        internal string Error { get; private set; } = string.Empty;

        /// <summary>Whether the refusal is the nesting ceiling.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=801F2B
        // Broiler-Human:        PENDING
        internal bool TooDeep { get; private set; }

        /// <summary>Reads one value and writes it as a literal.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CB1B92
        // Broiler-Human:        PENDING
        internal bool Value(System.Text.StringBuilder written, int depth)
        {
            if (depth > MaximumDepth)
            {
                TooDeep = true;
                return Fail("the document nests deeper than " + MaximumDepth + " levels");
            }

            SkipSpace();

            if (position == text.Length)
            {
                return Fail("the text ends where a value was expected");
            }

            switch (text[position])
            {
                case '{':
                    return Object(written, depth);

                case '[':
                    return Array(written, depth);

                case '"':
                    return String(written);

                case 't':
                    return Word("true", written);

                case 'f':
                    return Word("false", written);

                case 'n':
                    return Word("null", written);

                default:
                    return Number(written);
            }
        }

        /// <summary>Whether nothing but white space follows the value.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=CDA2CD
        // Broiler-Human:        PENDING
        internal bool End()
        {
            SkipSpace();
            return position == text.Length || Fail("text follows the value");
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B32183
        // Broiler-Human:        PENDING
        private bool Object(System.Text.StringBuilder written, int depth)
        {
            position++;
            written.Append('{');
            SkipSpace();

            if (Next('}'))
            {
                written.Append('}');
                return true;
            }

            while (true)
            {
                SkipSpace();

                if (position == text.Length || text[position] != '"')
                {
                    return Fail("an object key is a string");
                }

                written.Append('[');

                if (!String(written))
                {
                    return false;
                }

                written.Append("]: ");
                SkipSpace();

                if (!Next(':'))
                {
                    return Fail("an object key is followed by `:`");
                }

                if (!Value(written, depth + 1))
                {
                    return false;
                }

                SkipSpace();

                if (Next('}'))
                {
                    written.Append('}');
                    return true;
                }

                if (!Next(','))
                {
                    return Fail("an object member is followed by `,` or `}`");
                }

                written.Append(", ");
            }
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B517CA
        // Broiler-Human:        PENDING
        private bool Array(System.Text.StringBuilder written, int depth)
        {
            position++;
            written.Append('[');
            SkipSpace();

            if (Next(']'))
            {
                written.Append(']');
                return true;
            }

            while (true)
            {
                if (!Value(written, depth + 1))
                {
                    return false;
                }

                SkipSpace();

                if (Next(']'))
                {
                    written.Append(']');
                    return true;
                }

                if (!Next(','))
                {
                    return Fail("an array element is followed by `,` or `]`");
                }

                written.Append(", ");
            }
        }

        /// <summary>Reads one string and writes it as a double-quoted literal of printable ASCII.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BB9BF8
        // Broiler-Human:        PENDING
        private bool String(System.Text.StringBuilder written)
        {
            position++;
            written.Append('"');

            while (true)
            {
                if (position == text.Length)
                {
                    return Fail("a string is not closed");
                }

                var unit = text[position++];

                if (unit == '"')
                {
                    written.Append('"');
                    return true;
                }

                if (unit < 0x20)
                {
                    return Fail("a string holds an unescaped control character");
                }

                if (unit == '\\')
                {
                    if (position == text.Length)
                    {
                        return Fail("a string ends inside an escape");
                    }

                    var escape = text[position++];

                    switch (escape)
                    {
                        case '"':
                        case '\\':
                        case '/':
                            unit = escape;
                            break;

                        case 'b':
                            unit = '\b';
                            break;

                        case 'f':
                            unit = '\f';
                            break;

                        case 'n':
                            unit = '\n';
                            break;

                        case 'r':
                            unit = '\r';
                            break;

                        case 't':
                            unit = '\t';
                            break;

                        case 'u':
                            if (position + 4 > text.Length ||
                                !ushort.TryParse(
                                    System.MemoryExtensions.AsSpan(text, position, 4),
                                    System.Globalization.NumberStyles.AllowHexSpecifier,
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    out var code))
                            {
                                return Fail("a `\\u` escape is four hexadecimal digits");
                            }

                            position += 4;
                            unit = (char)code;
                            break;

                        default:
                            return Fail("a string holds an escape JSON does not define");
                    }
                }

                if (unit is >= ' ' and <= '~' && unit != '"' && unit != '\\')
                {
                    written.Append(unit);
                }
                else
                {
                    written.Append("\\u").Append(((int)unit).ToString("X4", System.Globalization.CultureInfo.InvariantCulture));
                }
            }
        }

        /// <summary>Reads one number and writes its text unchanged, which is a literal of the language.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=F2B30A
        // Broiler-Human:        PENDING
        private bool Number(System.Text.StringBuilder written)
        {
            var start = position;
            Next('-');

            if (Next('0'))
            {
                // A leading zero is a whole integer part: `01` is not JSON.
            }
            else if (!Digits())
            {
                return Fail("a value is an object, an array, a string, a number, true, false or null");
            }

            if (Next('.') && !Digits())
            {
                return Fail("a fraction has a digit");
            }

            if (Next('e') || Next('E'))
            {
                _ = Next('+') || Next('-');

                if (!Digits())
                {
                    return Fail("an exponent has a digit");
                }
            }

            written.Append(text, start, position - start);
            return true;
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=E0E3FA
        // Broiler-Human:        PENDING
        private bool Digits()
        {
            var start = position;

            while (position < text.Length && text[position] is >= '0' and <= '9')
            {
                position++;
            }

            return position != start;
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=B71F09
        // Broiler-Human:        PENDING
        private bool Word(string word, System.Text.StringBuilder written)
        {
            if (string.CompareOrdinal(text, position, word, 0, word.Length) != 0)
            {
                return Fail("a value is an object, an array, a string, a number, true, false or null");
            }

            position += word.Length;
            written.Append(word);
            return true;
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=F5DB03
        // Broiler-Human:        PENDING
        private bool Next(char expected)
        {
            if (position < text.Length && text[position] == expected)
            {
                position++;
                return true;
            }

            return false;
        }

        /// <summary>Skips JSON's four white-space characters, and no others.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=0E908F
        // Broiler-Human:        PENDING
        private void SkipSpace()
        {
            while (position < text.Length && text[position] is ' ' or '\t' or '\n' or '\r')
            {
                position++;
            }
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=85566E
        // Broiler-Human:        PENDING
        private bool Fail(string reason)
        {
            if (Error.Length == 0)
            {
                Error = reason + " (at offset " + position + ")";
            }

            return false;
        }
    }
}
