// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   23
// Annotated:        23/23
// Exempt:           55
// Human-reviewed:   0/23
// IP risk:          None
// Security risk:    High
// Criteria:         9/7
// Resource impact:  2/10 max
// Unverified:       23
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>The token kinds the slice grammar distinguishes.</summary>
/// <remarks>
/// Keywords are their own kinds rather than identifiers the parser compares by text, because a
/// text comparison in the parser is a second place that knows the keyword set. Reserved words the
/// slice does not use are one kind, <see cref="ReservedWord"/>, so that <c>class</c> is refused
/// as a construct outside the manifest and not as an undeclared identifier.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=647491
// Broiler-Human:        PENDING
public enum SliceTokenKind
{
    /// <summary>The end of the source.</summary>
    EndOfSource = 0,

    /// <summary>An identifier that is no keyword.</summary>
    Identifier,

    /// <summary>A numeric literal.</summary>
    NumericLiteral,

    /// <summary>A string literal.</summary>
    StringLiteral,

    /// <summary>A reserved word this manifest does not admit in any position.</summary>
    ReservedWord,

    // ---- keywords the slice uses -----------------------------------------------------------

    /// <summary><c>var</c>.</summary>
    Var,

    /// <summary><c>let</c>.</summary>
    Let,

    /// <summary><c>const</c>.</summary>
    Const,

    /// <summary><c>if</c>.</summary>
    If,

    /// <summary><c>else</c>.</summary>
    Else,

    /// <summary><c>while</c>.</summary>
    While,

    /// <summary><c>do</c>.</summary>
    Do,

    /// <summary><c>for</c>.</summary>
    For,

    /// <summary><c>break</c>.</summary>
    Break,

    /// <summary><c>continue</c>.</summary>
    Continue,

    /// <summary><c>true</c>.</summary>
    True,

    /// <summary><c>false</c>.</summary>
    False,

    // ---- punctuators -------------------------------------------------------------------------

    /// <summary><c>{</c>.</summary>
    OpenBrace,

    /// <summary><c>}</c>.</summary>
    CloseBrace,

    /// <summary><c>(</c>.</summary>
    OpenParen,

    /// <summary><c>)</c>.</summary>
    CloseParen,

    /// <summary><c>;</c>.</summary>
    Semicolon,

    /// <summary><c>,</c>.</summary>
    Comma,

    /// <summary><c>?</c>.</summary>
    Question,

    /// <summary><c>:</c>.</summary>
    Colon,

    /// <summary><c>=</c>.</summary>
    Equals,

    /// <summary><c>+</c>.</summary>
    Plus,

    /// <summary><c>-</c>.</summary>
    Minus,

    /// <summary><c>*</c>.</summary>
    Star,

    /// <summary><c>/</c>.</summary>
    Slash,

    /// <summary><c>%</c>.</summary>
    Percent,

    /// <summary><c>!</c>.</summary>
    Bang,

    /// <summary><c>&lt;</c>.</summary>
    LessThan,

    /// <summary><c>&lt;=</c>.</summary>
    LessThanEquals,

    /// <summary><c>&gt;</c>.</summary>
    GreaterThan,

    /// <summary><c>&gt;=</c>.</summary>
    GreaterThanEquals,

    /// <summary><c>===</c>.</summary>
    EqualsEqualsEquals,

    /// <summary><c>!==</c>.</summary>
    BangEqualsEquals,

    /// <summary><c>==</c>, which this manifest does not admit.</summary>
    EqualsEquals,

    /// <summary><c>!=</c>, which this manifest does not admit.</summary>
    BangEquals,

    /// <summary><c>&amp;</c>.</summary>
    Ampersand,

    /// <summary><c>|</c>.</summary>
    Bar,

    /// <summary><c>^</c>.</summary>
    Caret,

    /// <summary><c>~</c>, which this manifest does not admit: there is no bitwise-not opcode.</summary>
    Tilde,

    /// <summary><c>&amp;&amp;</c>.</summary>
    AmpersandAmpersand,

    /// <summary><c>||</c>.</summary>
    BarBar,

    /// <summary><c>&lt;&lt;</c>.</summary>
    LessThanLessThan,

    /// <summary><c>&gt;&gt;</c>.</summary>
    GreaterThanGreaterThan,

    /// <summary><c>&gt;&gt;&gt;</c>.</summary>
    GreaterThanGreaterThanGreaterThan,
}

/// <summary>One token: its kind, its text, where it starts, and two facts about how it was read.</summary>
/// <remarks>
/// <para>
/// <b>The last two fields are why this type exists rather than a kind-and-span pair.</b> The seed
/// keeps only a token span on its syntax nodes and therefore re-tokenizes raw source text in two
/// places - once to tell a directive prologue entry from a string expression with the same value,
/// and once to tell a legacy octal literal from a decimal one. Roadmap section 9 asks for those
/// re-scans to be deleted and for the facts they recover to be carried on the tree instead.
/// <see cref="RawText"/> and <see cref="IsLegacyOctal"/> are those facts, recorded once by the
/// pass that already had the characters in hand.
/// </para>
/// <para>
/// <see cref="PrecededByLineTerminator"/> is the third: automatic semicolon insertion is a
/// question about the whitespace between two tokens, and a parser that had to look back into the
/// source text to answer it would be a third re-scan.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=95A77A
// Broiler-Falsified-If: any consumer of this type reads the original source text to recover a fact a field here carries
// Broiler-Human:        PENDING
public readonly record struct SliceToken(
    SliceTokenKind Kind,
    string RawText,
    double NumericValue,
    string StringValue,
    int Line,
    int Column,
    bool PrecededByLineTerminator,
    bool IsLegacyOctal);

/// <summary>
/// The one pass over the source characters. Every artifact is tokenized at most once.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is a whole-input pass producing an array, not a pull lexer.</b> The parser needs one
/// token of lookahead and the validator needs none, so a pull lexer would buy nothing; and an
/// array makes "tokenized at most once" a property of the call graph - the tokenizer is called
/// from exactly one place - rather than a property of a cache nobody can see the bottom of.
/// </para>
/// <para>
/// <b>It refuses rather than throws.</b> Every malformed input answers with a diagnostic in the
/// returned list and a token stream truncated at the refusal; nothing here throws for input, which
/// is what lets the front end have one refusal path rather than two.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=5006C2
// Broiler-Falsified-If: a second call site for this type appears, or a consumer re-reads the source text
// Broiler-Human:        PENDING
public sealed class SliceTokenizer
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=123268
    // Broiler-Human:        PENDING
    private readonly string source;
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=35BE4C
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<SliceSourceDiagnostic> diagnostics = [];
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=75DAF6
    // Broiler-Human:        PENDING
    private int index;
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=8AAA9C
    // Broiler-Human:        PENDING
    private int line = 1;
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=2F3BE1
    // Broiler-Human:        PENDING
    private int lineStart;

    /// <summary>Creates a tokenizer over <paramref name="source"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=C8A05D
    // Broiler-Human:        PENDING
    public SliceTokenizer(string source) =>
        this.source = source ?? throw new System.ArgumentNullException(nameof(source));

    /// <summary>Every refusal this pass produced, in source order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=162F14
    // Broiler-Human:        PENDING
    public System.Collections.Generic.IReadOnlyList<SliceSourceDiagnostic> Diagnostics => diagnostics;

    /// <summary>Reads every token, ending with one <see cref="SliceTokenKind.EndOfSource"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=CC1F07
    // Broiler-Falsified-If: a token is produced after a refusal, or the stream does not end with exactly one EndOfSource
    // Broiler-Human:        PENDING
    public SliceToken[] Tokenize()
    {
        var tokens = new System.Collections.Generic.List<SliceToken>();

        while (true)
        {
            var sawNewline = SkipTrivia();

            if (diagnostics.Count > 0)
            {
                break;
            }

            var startLine = line;
            var startColumn = index - lineStart + 1;

            if (index >= source.Length)
            {
                tokens.Add(new SliceToken(
                    SliceTokenKind.EndOfSource, string.Empty, 0, string.Empty,
                    startLine, startColumn, sawNewline, false));

                return tokens.ToArray();
            }

            var token = ReadToken(startLine, startColumn, sawNewline);

            if (diagnostics.Count > 0)
            {
                break;
            }

            tokens.Add(token);
        }

        // A refusal ends the stream. The parser is never handed tokens after a tokenizing failure,
        // so it never reports a cascade of parse errors that are all one bad character.
        tokens.Add(new SliceToken(
            SliceTokenKind.EndOfSource, string.Empty, 0, string.Empty,
            line, index - lineStart + 1, false, false));

        return tokens.ToArray();
    }

    /// <summary>Skips whitespace and comments; answers whether a line terminator was among them.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=CED953
    // Broiler-Human:        PENDING
    private bool SkipTrivia()
    {
        var sawNewline = false;

        while (index < source.Length)
        {
            var c = source[index];

            if (IsLineTerminator(c))
            {
                sawNewline = true;
                AdvanceLine(c);
                continue;
            }

            if (IsWhiteSpace(c))
            {
                index++;
                continue;
            }

            if (c == '/' && index + 1 < source.Length && source[index + 1] == '/')
            {
                index += 2;

                while (index < source.Length && !IsLineTerminator(source[index]))
                {
                    index++;
                }

                continue;
            }

            if (c == '/' && index + 1 < source.Length && source[index + 1] == '*')
            {
                var commentLine = line;
                var commentColumn = index - lineStart + 1;
                index += 2;
                var closed = false;

                while (index < source.Length)
                {
                    if (source[index] == '*' && index + 1 < source.Length && source[index + 1] == '/')
                    {
                        index += 2;
                        closed = true;
                        break;
                    }

                    if (IsLineTerminator(source[index]))
                    {
                        // A multi-line comment counts as a line terminator for semicolon
                        // insertion, which is a rule a reader forgets and a parser must not.
                        sawNewline = true;
                        AdvanceLine(source[index]);
                        continue;
                    }

                    index++;
                }

                if (!closed)
                {
                    Refuse(
                        SliceSourceDiagnosticCode.UnterminatedComment,
                        "a block comment reaches the end of the source without closing",
                        commentLine,
                        commentColumn);

                    return sawNewline;
                }

                continue;
            }

            break;
        }

        return sawNewline;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=0ABF02
    // Broiler-Falsified-If: a character that starts an identifier is read as a punctuator, or a numeric literal is read as an identifier
    // Broiler-Human:        PENDING
    private SliceToken ReadToken(int startLine, int startColumn, bool sawNewline)
    {
        var c = source[index];

        if (IsIdentifierStart(c))
        {
            return ReadIdentifierOrKeyword(startLine, startColumn, sawNewline);
        }

        if (char.IsAsciiDigit(c) || (c == '.' && index + 1 < source.Length && char.IsAsciiDigit(source[index + 1])))
        {
            return ReadNumericLiteral(startLine, startColumn, sawNewline);
        }

        if (c is '"' or '\'')
        {
            return ReadStringLiteral(startLine, startColumn, sawNewline);
        }

        return ReadPunctuator(startLine, startColumn, sawNewline);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=6ED2BA
    // Broiler-Human:        PENDING
    private SliceToken ReadIdentifierOrKeyword(int startLine, int startColumn, bool sawNewline)
    {
        var start = index;

        while (index < source.Length && IsIdentifierPart(source[index]))
        {
            index++;
        }

        var text = source[start..index];

        return new SliceToken(
            KeywordKind(text), text, 0, string.Empty, startLine, startColumn, sawNewline, false);
    }

    /// <summary>The keyword table. One place knows which words are words.</summary>
    /// <remarks>
    /// <c>let</c> is here rather than treated contextually. The slice grammar has no production in
    /// which <c>let</c> is a legal identifier reference, so the contextual treatment the language
    /// requires would buy a program nobody can write and cost a rule nobody can see.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=778A3A
    // Broiler-Human:        PENDING
    private static SliceTokenKind KeywordKind(string text) => text switch
    {
        "var" => SliceTokenKind.Var,
        "let" => SliceTokenKind.Let,
        "const" => SliceTokenKind.Const,
        "if" => SliceTokenKind.If,
        "else" => SliceTokenKind.Else,
        "while" => SliceTokenKind.While,
        "do" => SliceTokenKind.Do,
        "for" => SliceTokenKind.For,
        "break" => SliceTokenKind.Break,
        "continue" => SliceTokenKind.Continue,
        "true" => SliceTokenKind.True,
        "false" => SliceTokenKind.False,

        // Every other reserved word of the language, in one kind. They are refused as constructs
        // outside the manifest rather than as unknown identifiers, because "this profile has no
        // functions" and "you did not declare `function`" are different sentences and only one of
        // them is true.
        "await" or "case" or "catch" or "class" or "debugger" or "default" or "delete" or
        "enum" or "export" or "extends" or "finally" or "function" or "import" or "in" or
        "instanceof" or "new" or "null" or "return" or "super" or "switch" or "this" or
        "throw" or "try" or "typeof" or "void" or "with" or "yield" or "static" or
        "implements" or "interface" or "package" or "private" or "protected" or "public" =>
            SliceTokenKind.ReservedWord,

        _ => SliceTokenKind.Identifier,
    };

    /// <summary>
    /// Reads a numeric literal, recording whether its shape is a legacy octal.
    /// </summary>
    /// <remarks>
    /// The legacy-octal fact is <b>recorded and not ruled on</b>. Whether it is an error depends
    /// on strictness, strictness is the validator's, and a tokenizer that knew about strictness
    /// would be the ambient parse state this component removed.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=2FD146
    // Broiler-Falsified-If: the value this produces differs from the language's MV for the same literal text
    // Broiler-Human:        PENDING
    private SliceToken ReadNumericLiteral(int startLine, int startColumn, bool sawNewline)
    {
        var start = index;
        var legacyOctal = false;
        double value;

        if (source[index] == '0' && index + 1 < source.Length &&
            (source[index + 1] is 'x' or 'X' or 'o' or 'O' or 'b' or 'B'))
        {
            var radix = source[index + 1] switch { 'x' or 'X' => 16, 'o' or 'O' => 8, _ => 2 };
            index += 2;
            var digitsStart = index;

            while (index < source.Length && IsRadixDigit(source[index], radix))
            {
                index++;
            }

            if (index == digitsStart)
            {
                return RefuseNumeric(start, startLine, startColumn, "a radix prefix with no digits");
            }

            value = 0;

            for (var at = digitsStart; at < index; at++)
            {
                value = (value * radix) + DigitValue(source[at]);
            }
        }
        else
        {
            // A leading zero followed only by decimal digits is a legacy octal when every digit is
            // an octal one, and a decimal literal otherwise - 08 is 8, and 010 is 8. The shape is
            // recorded either way; the value follows the shape.
            if (source[index] == '0' && index + 1 < source.Length && char.IsAsciiDigit(source[index + 1]))
            {
                var scan = index + 1;
                var octal = true;

                while (scan < source.Length && char.IsAsciiDigit(source[scan]))
                {
                    if (source[scan] is '8' or '9')
                    {
                        octal = false;
                    }

                    scan++;
                }

                if (octal && (scan >= source.Length || (source[scan] != '.' && source[scan] is not ('e' or 'E'))))
                {
                    legacyOctal = true;
                    index++;
                    var digitsStart = index;

                    while (index < source.Length && char.IsAsciiDigit(source[index]))
                    {
                        index++;
                    }

                    value = 0;

                    for (var at = digitsStart; at < index; at++)
                    {
                        value = (value * 8) + (source[at] - '0');
                    }

                    return FinishNumeric(start, value, legacyOctal, startLine, startColumn, sawNewline);
                }
            }

            while (index < source.Length && char.IsAsciiDigit(source[index]))
            {
                index++;
            }

            if (index < source.Length && source[index] == '.')
            {
                index++;

                while (index < source.Length && char.IsAsciiDigit(source[index]))
                {
                    index++;
                }
            }

            if (index < source.Length && source[index] is 'e' or 'E')
            {
                index++;

                if (index < source.Length && source[index] is '+' or '-')
                {
                    index++;
                }

                var exponentStart = index;

                while (index < source.Length && char.IsAsciiDigit(source[index]))
                {
                    index++;
                }

                if (index == exponentStart)
                {
                    return RefuseNumeric(start, startLine, startColumn, "an exponent with no digits");
                }
            }

            if (!double.TryParse(
                    System.MemoryExtensions.AsSpan(source, start, index - start),
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out value))
            {
                return RefuseNumeric(start, startLine, startColumn, "a numeric literal that is not a number");
            }
        }

        return FinishNumeric(start, value, legacyOctal, startLine, startColumn, sawNewline);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=DAF6FC
    // Broiler-Human:        PENDING
    private SliceToken FinishNumeric(
        int start, double value, bool legacyOctal, int startLine, int startColumn, bool sawNewline)
    {
        // A numeric literal may not be followed immediately by an identifier start: `3in` is not
        // `3 in`, it is an error, and a tokenizer that split it would hand the parser a program
        // the language does not have.
        if (index < source.Length && (IsIdentifierStart(source[index]) || char.IsAsciiDigit(source[index])))
        {
            return RefuseNumeric(start, startLine, startColumn, "a numeric literal touching an identifier");
        }

        return new SliceToken(
            SliceTokenKind.NumericLiteral,
            source[start..index],
            value,
            string.Empty,
            startLine,
            startColumn,
            sawNewline,
            legacyOctal);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=62A2FD
    // Broiler-Human:        PENDING
    private SliceToken RefuseNumeric(int start, int startLine, int startColumn, string what)
    {
        Refuse(SliceSourceDiagnosticCode.MalformedNumericLiteral, what, startLine, startColumn);

        return new SliceToken(
            SliceTokenKind.NumericLiteral, source[start..index], 0, string.Empty,
            startLine, startColumn, false, false);
    }

    /// <summary>
    /// Reads a string literal, keeping both its raw text and its value.
    /// </summary>
    /// <remarks>
    /// <b>Both, because a directive prologue is about the raw text.</b> <c>"use strict"</c> is a
    /// directive; <c>"use strict"</c> has the same value and is not one. The seed recovers
    /// this by re-tokenizing the source at validation time; carrying the raw text on the token is
    /// what deletes that scan.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=3AC8F9
    // Broiler-Falsified-If: a directive is recognised from the string's value rather than from its raw text
    // Broiler-Human:        PENDING
    private SliceToken ReadStringLiteral(int startLine, int startColumn, bool sawNewline)
    {
        var quote = source[index];
        var start = index;
        index++;
        var value = new System.Text.StringBuilder();

        while (true)
        {
            if (index >= source.Length || IsLineTerminator(source[index]))
            {
                Refuse(
                    SliceSourceDiagnosticCode.UnterminatedStringLiteral,
                    "a string literal reaches the end of the line without closing",
                    startLine,
                    startColumn);

                return new SliceToken(
                    SliceTokenKind.StringLiteral, source[start..index], 0, value.ToString(),
                    startLine, startColumn, false, false);
            }

            var c = source[index];

            if (c == quote)
            {
                index++;
                break;
            }

            if (c != '\\')
            {
                value.Append(c);
                index++;
                continue;
            }

            index++;

            if (index >= source.Length)
            {
                continue;
            }

            var escape = source[index];
            index++;

            switch (escape)
            {
                case 'n': value.Append('\n'); break;
                case 't': value.Append('\t'); break;
                case 'r': value.Append('\r'); break;
                case 'b': value.Append('\b'); break;
                case 'f': value.Append('\f'); break;
                case 'v': value.Append('\v'); break;
                case '0': value.Append('\0'); break;
                case '\\': value.Append('\\'); break;
                case '\'': value.Append('\''); break;
                case '"': value.Append('"'); break;

                case 'u':
                    if (index + 3 >= source.Length ||
                        !ushort.TryParse(
                            System.MemoryExtensions.AsSpan(source, index, 4),
                            System.Globalization.NumberStyles.HexNumber,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out var unit))
                    {
                        Refuse(
                            SliceSourceDiagnosticCode.UnknownEscapeSequence,
                            "a unicode escape with fewer than four hexadecimal digits",
                            startLine,
                            startColumn);

                        return new SliceToken(
                            SliceTokenKind.StringLiteral, source[start..index], 0, value.ToString(),
                            startLine, startColumn, false, false);
                    }

                    value.Append((char)unit);
                    index += 4;
                    break;

                default:
                    Refuse(
                        SliceSourceDiagnosticCode.UnknownEscapeSequence,
                        $"the escape sequence \\{escape} is not one this grammar defines",
                        startLine,
                        startColumn);

                    return new SliceToken(
                        SliceTokenKind.StringLiteral, source[start..index], 0, value.ToString(),
                        startLine, startColumn, false, false);
            }
        }

        return new SliceToken(
            SliceTokenKind.StringLiteral,
            source[start..index],
            0,
            value.ToString(),
            startLine,
            startColumn,
            sawNewline,
            false);
    }

    /// <summary>Reads one punctuator, longest match first.</summary>
    /// <remarks>
    /// Longest first is not a preference: reading <c>&gt;</c> before <c>&gt;&gt;&gt;</c> would
    /// silently turn an unsigned right shift into two comparisons, which is a program that parses
    /// and means something else.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=1F2B04
    // Broiler-Falsified-If: a shorter punctuator is matched where a longer one starting at the same character exists
    // Broiler-Human:        PENDING
    private SliceToken ReadPunctuator(int startLine, int startColumn, bool sawNewline)
    {
        foreach (var (text, kind) in Punctuators)
        {
            if (index + text.Length <= source.Length &&
                string.CompareOrdinal(source, index, text, 0, text.Length) == 0)
            {
                index += text.Length;

                return new SliceToken(
                    kind, text, 0, string.Empty, startLine, startColumn, sawNewline, false);
            }
        }

        Refuse(
            SliceSourceDiagnosticCode.UnexpectedCharacter,
            $"the character U+{(int)source[index]:X4} begins no token this grammar defines",
            startLine,
            startColumn);

        index++;

        return new SliceToken(
            SliceTokenKind.EndOfSource, string.Empty, 0, string.Empty,
            startLine, startColumn, false, false);
    }

    /// <summary>Every punctuator, in descending length so that the loop above is a longest match.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=E76BE8
    // Broiler-Falsified-If: this table is not in descending order of text length
    // Broiler-Human:        PENDING
    private static readonly (string Text, SliceTokenKind Kind)[] Punctuators =
    [
        (">>>", SliceTokenKind.GreaterThanGreaterThanGreaterThan),
        ("===", SliceTokenKind.EqualsEqualsEquals),
        ("!==", SliceTokenKind.BangEqualsEquals),
        ("<<", SliceTokenKind.LessThanLessThan),
        (">>", SliceTokenKind.GreaterThanGreaterThan),
        ("<=", SliceTokenKind.LessThanEquals),
        (">=", SliceTokenKind.GreaterThanEquals),
        ("==", SliceTokenKind.EqualsEquals),
        ("!=", SliceTokenKind.BangEquals),
        ("&&", SliceTokenKind.AmpersandAmpersand),
        ("||", SliceTokenKind.BarBar),
        ("{", SliceTokenKind.OpenBrace),
        ("}", SliceTokenKind.CloseBrace),
        ("(", SliceTokenKind.OpenParen),
        (")", SliceTokenKind.CloseParen),
        (";", SliceTokenKind.Semicolon),
        (",", SliceTokenKind.Comma),
        ("?", SliceTokenKind.Question),
        (":", SliceTokenKind.Colon),
        ("=", SliceTokenKind.Equals),
        ("+", SliceTokenKind.Plus),
        ("-", SliceTokenKind.Minus),
        ("*", SliceTokenKind.Star),
        ("/", SliceTokenKind.Slash),
        ("%", SliceTokenKind.Percent),
        ("!", SliceTokenKind.Bang),
        ("<", SliceTokenKind.LessThan),
        (">", SliceTokenKind.GreaterThan),
        ("&", SliceTokenKind.Ampersand),
        ("|", SliceTokenKind.Bar),
        ("^", SliceTokenKind.Caret),
        ("~", SliceTokenKind.Tilde),
    ];

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=5365F8
    // Broiler-Human:        PENDING
    private void AdvanceLine(char c)
    {
        // CRLF is one line terminator, not two, or every line number after the first Windows
        // newline is wrong by the count of them.
        if (c == '\r' && index + 1 < source.Length && source[index + 1] == '\n')
        {
            index++;
        }

        index++;
        line++;
        lineStart = index;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=846430
    // Broiler-Human:        PENDING
    private void Refuse(SliceSourceDiagnosticCode code, string message, int atLine, int atColumn) =>
        diagnostics.Add(new SliceSourceDiagnostic(code, message, atLine, atColumn));

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=FBEFF9
    // Broiler-Human:        PENDING
    private static bool IsLineTerminator(char c) => c is '\n' or '\r' or '\u2028' or '\u2029';

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=C49F38
    // Broiler-Human:        PENDING
    private static bool IsWhiteSpace(char c) =>
        c is ' ' or '\t' or '\v' or '\f' or '\u00a0' or '\ufeff' || char.IsWhiteSpace(c);

    /// <summary>
    /// Whether <paramref name="c"/> may start an identifier.
    /// </summary>
    /// <remarks>
    /// <b>ASCII plus <c>$</c> and <c>_</c>, and this is an exclusion rather than an omission.</b>
    /// The language's answer is the Unicode <c>ID_Start</c> property, which needs the Unicode data
    /// this component has not acquired - it is an open dependency with a named holder. A
    /// non-ASCII identifier is therefore refused as an unexpected character, which is a refusal
    /// rather than a silent acceptance, and the decision record carries it as a conformance
    /// exclusion.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=ABBC79
    // Broiler-Falsified-If: this admits a character outside ASCII while the Unicode data dependency is open
    // Broiler-Human:        PENDING
    private static bool IsIdentifierStart(char c) => char.IsAsciiLetter(c) || c is '$' or '_';

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=FD31B5
    // Broiler-Human:        PENDING
    private static bool IsIdentifierPart(char c) => IsIdentifierStart(c) || char.IsAsciiDigit(c);

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=4EACD3
    // Broiler-Human:        PENDING
    private static bool IsRadixDigit(char c, int radix) => radix switch
    {
        16 => char.IsAsciiHexDigit(c),
        8 => c is >= '0' and <= '7',
        _ => c is '0' or '1',
    };

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=61C94C
    // Broiler-Human:        PENDING
    private static int DigitValue(char c) =>
        char.IsAsciiDigit(c) ? c - '0' : (char.ToLowerInvariant(c) - 'a') + 10;
}
