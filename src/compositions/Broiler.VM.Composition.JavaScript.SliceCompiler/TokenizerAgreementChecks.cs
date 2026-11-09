// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.ECMAScript.Syntax;
using Broiler.Unicode.Algorithms;
using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;
using System.Globalization;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The shared and the slice tokenizers agree on the slice path's inputs: over every input the slice front end
/// accepts, the stream <see cref="SliceTokenizer"/> reads and the stream the shared library's parse produced are
/// the same, fact by fact, through a fixed field map (SJL-13's step 1, JSD-0083 decision D21; bundle
/// <c>sjl-13-001</c>'s part F).
/// </summary>
/// <remarks>
/// <para>
/// <b>Added 2026-11-09 by SJL-13's step 1.</b> The milestone keeps the slice path on <see cref="SliceTokenizer"/>
/// until SJL-15 and asks that the shared tokenizer's behaviour match it on the inputs the corpus shows. The
/// population is the parse corpus's sub-population B4 as the slice front end accepts it (each input compiled by
/// <see cref="SliceSourceCompiler"/> once, under its goal, its text unchanged; a JSON input has no slice goal) and
/// every program of <see cref="SliceSourcePrograms.Accepted"/>. An input the slice path refuses is not compared,
/// because its refusal may come before its stream is whole.
/// </para>
/// <para>
/// <b>The library's stream is its public one</b>: a parse of the text under the same goal with
/// <see cref="ParseRequest.RetainTokens"/> set and HTML-like comments off, as the slice path sets them, so that each
/// <c>/</c> is read as the parser's goal read it; <see cref="ParseResult.Tokens"/>. The request is handed the
/// format's Unicode tables, as the compiler's is, and a validator that admits every regular-expression literal,
/// since the comparison reads tokens and not refusals. The library
/// retains a stream for a parse it accepts and none for one it refuses, so an input the slice path accepts and the
/// library refuses has no library stream to compare: it is not compared, and the detail names each such input with
/// the library's refusal, so that it stays in view. Over the parse corpus two inputs are such, each a string with
/// a <c>\8</c> or <c>\08</c> escape in strict code, which the slice path admits and the specification refuses;
/// that is a fact of the slice path's strictness checks, not of either tokenizer.
/// </para>
/// <para>
/// <b>The field map</b> (decision D21), one token against one: the kind, through
/// the fixed kind map below applied to the library's kind, word and escaped flag, an escaped word mapping to an
/// identifier; for an identifier, <see cref="SliceToken.RawText"/> against the library token's cooked name, and the
/// text between <see cref="SliceToken.Offset"/> and <see cref="SliceToken.End"/> against the library token's raw
/// spelling; for every other token, the raw text; the numeric value of a numeric literal and the string value of a
/// string literal; line, column, offset and end; the preceded-by-line-terminator flag; the legacy-octal flag, which
/// <see cref="SliceToken.IsLegacyOctal"/> keeps for a legacy octal or non-octal decimal literal and for a string
/// with a legacy octal or <c>\8</c> or <c>\9</c> escape, and the library as four flags; and the escaped flag of an
/// identifier. Whether a <c>/</c> began a regular-expression literal is the kind. <b>The ends</b>: the library's
/// retained stream holds no end-of-input token, so the slice stream's <see cref="SliceTokenKind.EndOfSource"/>
/// is compared with the end of the text, and with the library's stream having ended there too; an end-of-input
/// token the library did retain would be compared by the map, as any other.
/// </para>
/// <para>
/// <b>A template</b>, one token in <see cref="SliceTokenizer"/>'s stream, is compared with the library's run of
/// template tokens for it one to n: the template's span against the run's first start and last end, and each
/// substitution's tokens, read by a slice tokenizer over the substitution's text, token by token as above. The
/// chunks are not compared: the slice tokenizer kept their places only in the wide compiler's structured mode,
/// which step 1 deletes (decision D20). The slice front end admits no template, so the population holds none, and
/// the detail counts any it met.
/// </para>
/// <para>
/// <b>It fails on a population that is not there</b>: a run that cannot find the parse corpus's manifest from the
/// process's directory or the current one, or that compares no input, fails rather than passing over nothing.
/// </para>
/// </remarks>
internal static class TokenizerAgreementChecks
{
    internal static IEnumerable<(string Name, bool Passed, string Detail)> Run()
    {
        yield return TheSharedAndSliceTokenizersAgree();
    }

    /// <summary>The slice kinds that are words, each the kind of the library word of its name.</summary>
    private static readonly SliceTokenKind[] WordKinds =
    [
        SliceTokenKind.Var, SliceTokenKind.Let, SliceTokenKind.Const, SliceTokenKind.If, SliceTokenKind.Else,
        SliceTokenKind.While, SliceTokenKind.Do, SliceTokenKind.For, SliceTokenKind.Break, SliceTokenKind.Continue,
        SliceTokenKind.True, SliceTokenKind.False, SliceTokenKind.Function, SliceTokenKind.Return, SliceTokenKind.This,
        SliceTokenKind.Null, SliceTokenKind.New, SliceTokenKind.Typeof, SliceTokenKind.Instanceof, SliceTokenKind.In,
        SliceTokenKind.Of, SliceTokenKind.Delete, SliceTokenKind.Void, SliceTokenKind.Try, SliceTokenKind.Catch,
        SliceTokenKind.Finally, SliceTokenKind.Throw, SliceTokenKind.Switch, SliceTokenKind.Case, SliceTokenKind.Default,
        SliceTokenKind.Class, SliceTokenKind.Extends, SliceTokenKind.Super, SliceTokenKind.Yield, SliceTokenKind.Await,
        SliceTokenKind.Async, SliceTokenKind.Import, SliceTokenKind.Export, SliceTokenKind.With, SliceTokenKind.Debugger,
        SliceTokenKind.Get, SliceTokenKind.Set, SliceTokenKind.Static,
    ];

    /// <summary>The fixed kind map's word half: a library word to its slice kind; any other word is an identifier.</summary>
    private static readonly Dictionary<Word, SliceTokenKind> Words = BuildWords();

    /// <summary>The fixed kind map's other half: every library kind but a name, a template piece and the invalid one.</summary>
    private static readonly Dictionary<TokenKind, SliceTokenKind> Kinds = new()
    {
        [TokenKind.EndOfInput] = SliceTokenKind.EndOfSource,
        [TokenKind.PrivateIdentifier] = SliceTokenKind.Identifier,
        [TokenKind.NumericLiteral] = SliceTokenKind.NumericLiteral,
        [TokenKind.StringLiteral] = SliceTokenKind.StringLiteral,
        [TokenKind.RegularExpressionLiteral] = SliceTokenKind.RegularExpressionLiteral,
        [TokenKind.LeftBrace] = SliceTokenKind.OpenBrace,
        [TokenKind.RightBrace] = SliceTokenKind.CloseBrace,
        [TokenKind.LeftParen] = SliceTokenKind.OpenParen,
        [TokenKind.RightParen] = SliceTokenKind.CloseParen,
        [TokenKind.LeftBracket] = SliceTokenKind.OpenBracket,
        [TokenKind.RightBracket] = SliceTokenKind.CloseBracket,
        [TokenKind.Dot] = SliceTokenKind.Dot,
        [TokenKind.Ellipsis] = SliceTokenKind.DotDotDot,
        [TokenKind.Semicolon] = SliceTokenKind.Semicolon,
        [TokenKind.Comma] = SliceTokenKind.Comma,
        [TokenKind.LessThan] = SliceTokenKind.LessThan,
        [TokenKind.GreaterThan] = SliceTokenKind.GreaterThan,
        [TokenKind.LessThanEquals] = SliceTokenKind.LessThanEquals,
        [TokenKind.GreaterThanEquals] = SliceTokenKind.GreaterThanEquals,
        [TokenKind.EqualsEquals] = SliceTokenKind.EqualsEquals,
        [TokenKind.NotEquals] = SliceTokenKind.BangEquals,
        [TokenKind.EqualsEqualsEquals] = SliceTokenKind.EqualsEqualsEquals,
        [TokenKind.NotEqualsEquals] = SliceTokenKind.BangEqualsEquals,
        [TokenKind.Plus] = SliceTokenKind.Plus,
        [TokenKind.Minus] = SliceTokenKind.Minus,
        [TokenKind.Star] = SliceTokenKind.Star,
        [TokenKind.Slash] = SliceTokenKind.Slash,
        [TokenKind.Percent] = SliceTokenKind.Percent,
        [TokenKind.StarStar] = SliceTokenKind.StarStar,
        [TokenKind.PlusPlus] = SliceTokenKind.PlusPlus,
        [TokenKind.MinusMinus] = SliceTokenKind.MinusMinus,
        [TokenKind.LeftShift] = SliceTokenKind.LessThanLessThan,
        [TokenKind.RightShift] = SliceTokenKind.GreaterThanGreaterThan,
        [TokenKind.UnsignedRightShift] = SliceTokenKind.GreaterThanGreaterThanGreaterThan,
        [TokenKind.Ampersand] = SliceTokenKind.Ampersand,
        [TokenKind.Bar] = SliceTokenKind.Bar,
        [TokenKind.Caret] = SliceTokenKind.Caret,
        [TokenKind.Bang] = SliceTokenKind.Bang,
        [TokenKind.Tilde] = SliceTokenKind.Tilde,
        [TokenKind.AmpersandAmpersand] = SliceTokenKind.AmpersandAmpersand,
        [TokenKind.BarBar] = SliceTokenKind.BarBar,
        [TokenKind.QuestionQuestion] = SliceTokenKind.QuestionQuestion,
        [TokenKind.Question] = SliceTokenKind.Question,
        [TokenKind.QuestionDot] = SliceTokenKind.QuestionDot,
        [TokenKind.Colon] = SliceTokenKind.Colon,
        [TokenKind.Equals] = SliceTokenKind.Equals,
        [TokenKind.PlusEquals] = SliceTokenKind.CompoundAssign,
        [TokenKind.MinusEquals] = SliceTokenKind.CompoundAssign,
        [TokenKind.StarEquals] = SliceTokenKind.CompoundAssign,
        [TokenKind.SlashEquals] = SliceTokenKind.CompoundAssign,
        [TokenKind.PercentEquals] = SliceTokenKind.CompoundAssign,
        [TokenKind.StarStarEquals] = SliceTokenKind.CompoundAssign,
        [TokenKind.LeftShiftEquals] = SliceTokenKind.CompoundAssign,
        [TokenKind.RightShiftEquals] = SliceTokenKind.CompoundAssign,
        [TokenKind.UnsignedRightShiftEquals] = SliceTokenKind.CompoundAssign,
        [TokenKind.AmpersandEquals] = SliceTokenKind.CompoundAssign,
        [TokenKind.BarEquals] = SliceTokenKind.CompoundAssign,
        [TokenKind.CaretEquals] = SliceTokenKind.CompoundAssign,
        [TokenKind.AmpersandAmpersandEquals] = SliceTokenKind.CompoundAssign,
        [TokenKind.BarBarEquals] = SliceTokenKind.CompoundAssign,
        [TokenKind.QuestionQuestionEquals] = SliceTokenKind.CompoundAssign,
        [TokenKind.Arrow] = SliceTokenKind.EqualsGreaterThan,
    };

    /// <summary>The legacy-octal facts the library keeps as four flags and <see cref="SliceToken"/> as one.</summary>
    private const TokenFlags LegacyOctalFlags =
        TokenFlags.LegacyOctal | TokenFlags.NonOctalDecimal | TokenFlags.LegacyOctalEscape | TokenFlags.NonOctalDecimalEscape;

    private static Dictionary<Word, SliceTokenKind> BuildWords()
    {
        var words = new Dictionary<Word, SliceTokenKind> { [Word.Enum] = SliceTokenKind.ReservedWord };

        foreach (var kind in WordKinds)
        {
            words.Add(Enum.Parse<Word>(kind.ToString()), kind);
        }

        return words;
    }

    private static (string, bool, string) TheSharedAndSliceTokenizersAgree()
    {
        const string Name = "the shared and slice tokenizers agree on the slice path's inputs";

        var manifest = FindManifest();

        if (manifest is null)
        {
            return (Name, false, "no directory above this process's or the current one holds src/tests/corpus/parse-1/corpus.manifest");
        }

        if (!ParseCorpus.TryReadInputs(manifest, out var corpus, out var failure))
        {
            return (Name, false, "the parse corpus could not be read: " + failure);
        }

        var inputs = corpus
            .Where(static input => !string.Equals(input.Goal, "json", StringComparison.Ordinal))
            .Select(static input => (input.Id, Module: string.Equals(input.Goal, "module", StringComparison.Ordinal), input.Text))
            .Concat(SliceSourcePrograms.Accepted.Select(static program => (Id: "accepted:" + program.Name, Module: false, Text: program.Source)))
            .ToArray();

        var compared = 0;
        var tokens = 0;
        var templates = 0;
        var refused = new List<string>();
        var disagreements = new List<string>();

        foreach (var (id, module, text) in inputs)
        {
            var compiled = SliceSourceCompiler.Compile(text, module ? SliceParseOptions.Module : SliceParseOptions.Script);

            if (!compiled.Succeeded)
            {
                continue;
            }

            var disagreement = Compare(text, module, ref tokens, ref templates, out var refusal);

            if (refusal is not null)
            {
                refused.Add(id + " (" + refusal + ")");
                continue;
            }

            compared++;

            if (disagreement is not null)
            {
                disagreements.Add(id + ": " + disagreement);
            }
        }

        if (compared == 0)
        {
            return (Name, false, "no input of the population was accepted by the slice front end, so nothing was compared");
        }

        return (
            Name,
            disagreements.Count == 0,
            disagreements.Count == 0
                ? $"{compared.ToString(CultureInfo.InvariantCulture)} inputs the slice front end accepts (parse corpus B4 and " +
                  $"SliceSourcePrograms.Accepted), {tokens.ToString(CultureInfo.InvariantCulture)} tokens, each the same in " +
                  "both streams by the field map: kind, cooked name and raw spelling, value, line, column, offset, end, " +
                  $"line-terminator, legacy-octal and escaped flags; {templates.ToString(CultureInfo.InvariantCulture)} templates; " +
                  $"{refused.Count.ToString(CultureInfo.InvariantCulture)} accepted by the slice path and refused by the library, so " +
                  "with no library stream, not compared" + (refused.Count == 0 ? string.Empty : ": " + string.Join(", ", refused))
                : $"{disagreements.Count.ToString(CultureInfo.InvariantCulture)} of {compared.ToString(CultureInfo.InvariantCulture)} " +
                  "inputs disagree; the first: " + string.Join("; ", disagreements.Take(5)));
    }

    /// <summary>The first disagreement between the two streams of one text, or null when they agree.</summary>
    private static string? Compare(string text, bool module, ref int tokens, ref int templates, out string? refusal)
    {
        refusal = null;
        var slice = new SliceTokenizer(text).Tokenize();
        var goal = module ? ParseGoal.Module : ParseGoal.Script;
        var request = new ParseRequest(new SourceText(text), goal, FormatTables.Instance, AdmitEveryPattern.Instance)
        {
            Switches = ParseSwitches.For(goal) with { HtmlLikeComments = false },
            RetainTokens = true,
        };

        var result = SyntaxParser.Parse(request);

        if (result.Diagnostics.Count != 0)
        {
            var first = result.Diagnostics[0];
            refusal = $"{first.Code} {first.MessageId} at {first.Span.Line}:{first.Span.Column}";
            return null;
        }

        var shared = result.Tokens;
        var at = 0;

        foreach (var token in slice)
        {
            if (at >= shared.Count)
            {
                if (token.Kind == SliceTokenKind.EndOfSource && token.Offset == text.Length)
                {
                    tokens++;
                    continue;
                }

                return $"the library's stream ends before the slice token {token.Kind} at {token.Line}:{token.Column}";
            }

            if (token.Kind == SliceTokenKind.TemplateLiteral)
            {
                templates++;
                var template = CompareTemplate(text, token, shared, ref at, ref tokens);

                if (template is not null)
                {
                    return template;
                }

                continue;
            }

            var disagreement = CompareToken(text, token, shared[at]);

            if (disagreement is not null)
            {
                return disagreement;
            }

            tokens++;
            at++;
        }

        return at == shared.Count ? null : $"the library's stream goes on past the slice stream's end, at token {at}";
    }

    /// <summary>One slice token against one library token, by the field map.</summary>
    private static string? CompareToken(string text, SliceToken token, Token shared, int offset = 0, int line = 0, int column = 0)
    {
        var where = $"{token.Kind} at {token.Line}:{token.Column}";
        var start = token.Offset + offset;
        var end = token.End + offset;
        var tokenLine = token.Line + line;
        var tokenColumn = token.Line == 1 ? token.Column + column : token.Column;

        if (Map(shared) is not { } kind || kind != token.Kind)
        {
            return $"{where}: the library's kind {shared.Kind} ({shared.Word}{(shared.IsEscaped ? ", escaped" : string.Empty)}) maps to {(Map(shared)?.ToString() ?? "no slice kind")}";
        }

        if (start != shared.Start || end != shared.End || tokenLine != shared.Line || tokenColumn != shared.Column)
        {
            return $"{where}: offset {start}..{end} line {tokenLine} column {tokenColumn}, the library's {shared.Start}..{shared.End} line {shared.Line} column {shared.Column}";
        }

        var raw = text[shared.Start..shared.End];

        if (token.Kind == SliceTokenKind.Identifier || (shared.Kind == TokenKind.IdentifierName && !shared.IsEscaped && Words.ContainsKey(shared.Word)))
        {
            if (!string.Equals(token.RawText, shared.Value, StringComparison.Ordinal))
            {
                return $"{where}: the name {token.RawText}, the library's cooked name {shared.Value}";
            }

            if (!string.Equals(text[start..end], raw, StringComparison.Ordinal))
            {
                return $"{where}: the spelling {text[start..end]}, the library's {raw}";
            }

            if (token.Kind == SliceTokenKind.Identifier && token.IsEscaped != shared.IsEscaped)
            {
                return $"{where}: escaped {token.IsEscaped}, the library's {shared.IsEscaped}";
            }
        }
        else if (token.Kind != SliceTokenKind.EndOfSource && !string.Equals(token.RawText, raw, StringComparison.Ordinal))
        {
            return $"{where}: the raw text {token.RawText}, the library's {raw}";
        }

        if (token.Kind == SliceTokenKind.NumericLiteral &&
            !(token.NumericValue.Equals(shared.Number) || (double.IsNaN(shared.Number) && (shared.Flags & TokenFlags.BigInt) != 0)))
        {
            return $"{where}: the value {token.NumericValue.ToString("R", CultureInfo.InvariantCulture)}, the library's {shared.Number.ToString("R", CultureInfo.InvariantCulture)}";
        }

        if (token.Kind == SliceTokenKind.StringLiteral && !string.Equals(token.StringValue, shared.Value, StringComparison.Ordinal))
        {
            return $"{where}: the string value differs from the library's";
        }

        if (token.PrecededByLineTerminator != shared.NewLineBefore)
        {
            return $"{where}: preceded by a line terminator {token.PrecededByLineTerminator}, the library's {shared.NewLineBefore}";
        }

        if (token.IsLegacyOctal != ((shared.Flags & LegacyOctalFlags) != 0))
        {
            return $"{where}: legacy octal {token.IsLegacyOctal}, the library's flags {shared.Flags}";
        }

        return null;
    }

    /// <summary>
    /// A template, one slice token, against the library's run for it: its span, and each substitution's tokens read
    /// by a slice tokenizer over the substitution's text.
    /// </summary>
    private static string? CompareTemplate(string text, SliceToken token, IReadOnlyList<Token> shared, ref int at, ref int tokens)
    {
        var where = $"the template at {token.Line}:{token.Column}";
        var first = shared[at];

        if (first.Kind is not (TokenKind.NoSubstitutionTemplate or TokenKind.TemplateHead) || first.Start != token.Offset)
        {
            return $"{where}: the library's token there is {first.Kind} at {first.Start}";
        }

        at++;

        if (first.Kind == TokenKind.NoSubstitutionTemplate)
        {
            return first.End == token.End ? null : $"{where}: it ends at {token.End}, the library's at {first.End}";
        }

        var open = first;

        while (true)
        {
            // THE SUBSTITUTION'S TOKENS RUN TO THE NEXT MIDDLE OR TAIL OF THIS TEMPLATE AT ITS DEPTH.
            var substitutionStart = open.End;
            var depth = 0;
            var close = at;

            while (close < shared.Count)
            {
                var kind = shared[close].Kind;

                if (kind == TokenKind.TemplateHead)
                {
                    depth++;
                }
                else if (kind is TokenKind.TemplateMiddle or TokenKind.TemplateTail)
                {
                    if (depth == 0)
                    {
                        break;
                    }

                    if (kind == TokenKind.TemplateTail)
                    {
                        depth--;
                    }
                }

                close++;
            }

            if (close >= shared.Count)
            {
                return $"{where}: the library's run has no middle or tail";
            }

            var substitution = text[substitutionStart..shared[close].Start];
            var prefix = text[..substitutionStart];
            var line = prefix.Count(static c => c == '\n');
            var column = substitutionStart - (prefix.LastIndexOf('\n') + 1);
            var inner = new SliceTokenizer(substitution).Tokenize();

            foreach (var piece in inner.Where(static piece => piece.Kind != SliceTokenKind.EndOfSource))
            {
                if (at >= close)
                {
                    return $"{where}: the substitution at {substitutionStart} has more slice tokens than the library's";
                }

                var disagreement = CompareToken(text, piece, shared[at], substitutionStart, line, column);

                if (disagreement is not null)
                {
                    return where + ", a substitution: " + disagreement;
                }

                tokens++;
                at++;
            }

            if (at != close)
            {
                return $"{where}: the substitution at {substitutionStart} has fewer slice tokens than the library's";
            }

            open = shared[at];
            at++;

            if (open.Kind == TokenKind.TemplateTail)
            {
                return open.End == token.End ? null : $"{where}: it ends at {token.End}, the library's at {open.End}";
            }
        }
    }

    /// <summary>The fixed kind map applied to one library token: its kind, its word and its escaped flag.</summary>
    private static SliceTokenKind? Map(Token token)
    {
        if (token.Kind == TokenKind.IdentifierName)
        {
            return token.IsEscaped || !Words.TryGetValue(token.Word, out var word) ? SliceTokenKind.Identifier : word;
        }

        return Kinds.TryGetValue(token.Kind, out var kind) ? kind : null;
    }

    private static string? FindManifest()
    {
        foreach (var start in new[] { AppContext.BaseDirectory, Environment.CurrentDirectory })
        {
            for (var directory = new DirectoryInfo(start); directory is not null; directory = directory.Parent)
            {
                var manifest = Path.Combine(directory.FullName, "src", "tests", "corpus", "parse-1", "corpus.manifest");

                if (File.Exists(manifest))
                {
                    return manifest;
                }
            }
        }

        return null;
    }

    /// <summary>The format's Unicode tables, as the compiler hands them to its own requests.</summary>
    private sealed class FormatTables : IUnicodeTables
    {
        internal static readonly FormatTables Instance = new();

        public string UnicodeVersion => JsUnicodeData.UnicodeVersion;

        public ReadOnlySpan<byte> Table(UnicodeTable table) => JsUnicodeData.Table((int)table);
    }

    /// <summary>A validator that admits every pattern: the row compares tokens, not refusals.</summary>
    private sealed class AdmitEveryPattern : IRegularExpressionValidator
    {
        internal static readonly AdmitEveryPattern Instance = new();

        public RegularExpressionVerdict Validate(string pattern, string flags) => RegularExpressionVerdict.Valid;
    }
}
