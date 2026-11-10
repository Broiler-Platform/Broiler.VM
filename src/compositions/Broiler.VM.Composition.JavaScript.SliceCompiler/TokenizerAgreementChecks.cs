// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.ECMAScript.Syntax;
using Broiler.Unicode.Algorithms;
using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;
using System.Formats.Tar;
using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

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
/// library refuses has no library stream to compare. <b>The row lists such an input and does not compare it</b>: its
/// detail names each one with the library's shared code, the catalogue row the mapping gives that code and the
/// registry code it maps to, and the row passes over every input the library's parse accepts (part F as the area
/// rule's second revision of 2026-11-09 reads it, item 4; JSD-0083's note of that day on decision D21, "Exempt and
/// record"). Over the parse corpus two inputs are such, <c>J000262</c> and <c>J001009</c>, each a string with a
/// <c>\8</c> or <c>\08</c> escape in strict code, which the slice path admits and the specification refuses (the
/// catalogue's EE0004, shared code JSE1204, registry code 2210): the slice path's known defect, handed on as H25. The
/// detail names H25 only for an input refused under JSE1204, and says whether those are exactly the two the rule
/// names; an input refused under any other code is listed with no reason, since neither the rule nor this row gives
/// one, and the bundle gives it its own. A shared code the mapping does not hold fails the row, since the input could
/// then not be listed with its row. <i>(Revised 2026-11-09, later that day, at the review of SJL-13.3's step 1: the
/// detail named every listed input "the slice path's known defect, hand-off H25", whatever its code, "as part F's
/// configuration lists them", where the configuration asks the row to list them; and a run in which the library
/// refused every input the slice front end accepts said "no input of the population was accepted by the slice front
/// end".)</i>
/// <i>(Revised 2026-11-09, at the review of SJL-13.3's step 1: the row passed over such an input, "it is not
/// compared, and the detail names each such input with the library's refusal, so that it stays in view", and its
/// failing is a question for the owner, whose answer a dated revision of the rule would record.)</i> <i>(Revised
/// 2026-11-09, later that day, with the owner's decision "Exempt and record": it read "<b>The row fails on such an
/// input</b> and names it with the library's refusal: part F's population is every input the slice path accepts, and
/// its rule admits no class of difference, so an input of the population the row cannot compare is not one that
/// agrees.")</i>
/// </para>
/// <para>
/// <b>The population is read as the <c>--parse-corpus</c> mode reads it, without that mode's code</b>: the
/// manifest's rows, each stored input from the corpus and each test262 input from the archive the manifest pins,
/// every input's bytes checked against the manifest's SHA-256 and decoded as the conformance root decodes a file, so
/// the texts are the mode's texts byte for byte. The reader is this file's own so that <c>ParseCorpus.cs</c>, which
/// part B fixes as the base commit holds it, stays unchanged. <i>(Revised 2026-11-09, at the review of SJL-13.3's
/// step 1: the row read its population through a method it had added to <c>ParseCorpus</c>.)</i>
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
/// template tokens for it one to n: the template's span against the run's first start and last end; each chunk,
/// read from the template's text from its backtick or its <c>}</c> to the next unescaped <c>${</c> or backtick,
/// against the library's piece: its span, its cooked value (the TV of clause 12.9.6, none where it holds a
/// <c>NotEscapeSequence</c>) and its raw value (the TRV, every CR and CR LF read as LF); and each substitution, its
/// end the first <c>}</c> a slice tokenizer over the text after its <c>${</c> leaves unmatched, against the library's
/// next piece's start, and its tokens, token by token as above. The slice front end admits no template, so the
/// population holds none, and the detail counts any it met. <i>(Revised 2026-11-09, at the review of SJL-13.3's
/// step 1: the chunks were not compared, "the slice tokenizer kept their places only in the wide compiler's
/// structured mode, which step 1 deletes (decision D20)", and a substitution's text ran to the library's next
/// piece; part F's configuration compares each cooked and raw chunk.)</i>
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

    /// <summary>
    /// The shared code of the library's refusals that hand-off H25 covers on the slice path (the catalogue's EE0004),
    /// and the inputs the area rule's second revision of 2026-11-09 names under it.
    /// </summary>
    private const string HandedOnCode = "JSE1204";

    private static readonly string[] HandedOnInputs = ["J000262", "J001009"];

    private static (string, bool, string) TheSharedAndSliceTokenizersAgree()
    {
        const string Name = "the shared and slice tokenizers agree on the slice path's inputs";

        var manifest = FindManifest();

        if (manifest is null)
        {
            return (Name, false, "no directory above this process's or the current one holds src/tests/corpus/parse-1/corpus.manifest");
        }

        if (!TryReadCorpus(manifest, out var corpus, out var failure))
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
        var refused = new List<(string Id, string Code, string MessageId, int Line, int Column)>();
        var disagreements = new List<string>();

        foreach (var (id, module, text) in inputs)
        {
            var compiled = SliceSourceCompiler.Compile(text, module ? SliceParseOptions.Module : SliceParseOptions.Script);

            if (!compiled.Succeeded)
            {
                continue;
            }

            var disagreement = Compare(text, module, ref tokens, ref templates, out var refusal);

            if (refusal is { } first)
            {
                refused.Add((id, first.Code, first.MessageId, first.Span.Line, first.Span.Column));
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
            return (Name, false, refused.Count == 0
                ? "no input of the population was accepted by the slice front end, so nothing was compared"
                : "no input the slice front end accepts was accepted by the library, so nothing was compared");
        }

        var agreed =
            $"{compared.ToString(CultureInfo.InvariantCulture)} inputs the slice front end accepts (parse corpus B4 and " +
            $"SliceSourcePrograms.Accepted) compared, {tokens.ToString(CultureInfo.InvariantCulture)} tokens, each the same in " +
            "both streams by the field map: kind, cooked name and raw spelling, value, line, column, offset, end, " +
            $"line-terminator, legacy-octal and escaped flags; {templates.ToString(CultureInfo.InvariantCulture)} templates";
        var mapping = ReadMapping(manifest);
        var unmapped = new List<string>();
        var handedOn = new List<string>();
        var listed = new List<string>();

        foreach (var (id, code, messageId, line, column) in refused)
        {
            if (mapping is not null && mapping.TryGetValue(code, out var row))
            {
                var entry = $"{id} ({code} {messageId}, catalogue row {row.Row}, registry code {row.Registry}, at {line}:{column})";

                if (string.Equals(code, HandedOnCode, StringComparison.Ordinal))
                {
                    handedOn.Add(entry);
                }
                else
                {
                    listed.Add(entry);
                }
            }
            else
            {
                unmapped.Add($"{id} ({code} {messageId} at {line}:{column})");
            }
        }

        var namedByRule = handedOn.Count == HandedOnInputs.Length &&
            refused.Where(static r => string.Equals(r.Code, HandedOnCode, StringComparison.Ordinal))
                .Select(static r => r.Id).Order(StringComparer.Ordinal).SequenceEqual(HandedOnInputs, StringComparer.Ordinal);
        var uncompared = (handedOn.Count + listed.Count == 0
                ? "; no input the slice path accepts is refused by the library"
                : $"; {(handedOn.Count + listed.Count).ToString(CultureInfo.InvariantCulture)} accepted by the slice path and " +
                  "refused by the library, which the row lists and does not compare, as part F's configuration asks") +
            (handedOn.Count == 0
                ? string.Empty
                : $"; {handedOn.Count.ToString(CultureInfo.InvariantCulture)} under {HandedOnCode}, the slice path's known " +
                  "defect handed on as H25" +
                  (namedByRule
                      ? ", the two inputs the rule names"
                      : ", where the rule names " + string.Join(" and ", HandedOnInputs) + " and no other") +
                  ": " + string.Join(", ", handedOn)) +
            (listed.Count == 0
                ? string.Empty
                : $"; {listed.Count.ToString(CultureInfo.InvariantCulture)} under another code, for which neither the rule " +
                  "nor this row gives a reason, so the bundle must give each its own: " + string.Join(", ", listed)) +
            (unmapped.Count == 0
                ? string.Empty
                : $"; {unmapped.Count.ToString(CultureInfo.InvariantCulture)} refused by the library under a shared code the " +
                  "mapping does not hold, so not listed with a catalogue row: " + string.Join(", ", unmapped));

        return (
            Name,
            disagreements.Count == 0 && unmapped.Count == 0,
            disagreements.Count == 0
                ? agreed + uncompared
                : $"{disagreements.Count.ToString(CultureInfo.InvariantCulture)} of {compared.ToString(CultureInfo.InvariantCulture)} " +
                  "inputs disagree; the first: " + string.Join("; ", disagreements.Take(5)) + uncompared);
    }

    /// <summary>The first disagreement between the two streams of one text, or null when they agree.</summary>
    private static string? Compare(string text, bool module, ref int tokens, ref int templates, out SyntaxDiagnostic? refusal)
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
            refusal = result.Diagnostics[0];
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
                var template = CompareTemplate(text, token.Offset, token.End, token.Line, token.Column, shared, ref at, ref tokens, ref templates);

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
    /// A template, one slice token, against the library's run for it: its span; each chunk's span, cooked value and
    /// raw value; and each substitution's end and tokens, read by a slice tokenizer over the substitution's text.
    /// </summary>
    private static string? CompareTemplate(
        string text, int offset, int end, int templateLine, int templateColumn, IReadOnlyList<Token> shared, ref int at,
        ref int tokens, ref int templates)
    {
        var where = $"the template at {templateLine}:{templateColumn}";
        var first = shared[at];

        if (first.Kind is not (TokenKind.NoSubstitutionTemplate or TokenKind.TemplateHead) || first.Start != offset)
        {
            return $"{where}: the library's token there is {first.Kind} at {first.Start}";
        }

        // THE CHUNK AFTER THE BACKTICK, AND AFTER EACH SUBSTITUTION'S `}`, RUNS TO THE NEXT UNESCAPED `${` OR BACKTICK.
        var chunkStart = offset + 1;
        var piece = first;
        at++;

        while (true)
        {
            var chunkEnd = ChunkEnd(text, chunkStart, end);

            if (chunkEnd < 0)
            {
                return $"{where}: the chunk at {chunkStart} reaches the template's end without `${{` or a backtick";
            }

            var opens = text[chunkEnd] != '`';
            var pieceEnd = chunkEnd + (opens ? 2 : 1);

            if (piece.Start != chunkStart - 1 || piece.End != pieceEnd)
            {
                return $"{where}: the chunk's piece is {chunkStart - 1}..{pieceEnd}, the library's {piece.Kind} {piece.Start}..{piece.End}";
            }

            if (piece.Kind != (opens
                    ? (chunkStart == offset + 1 ? TokenKind.TemplateHead : TokenKind.TemplateMiddle)
                    : (chunkStart == offset + 1 ? TokenKind.NoSubstitutionTemplate : TokenKind.TemplateTail)))
            {
                return $"{where}: the chunk at {chunkStart} is the library's {piece.Kind}";
            }

            var chunk = text[chunkStart..chunkEnd];
            var cooked = Cooked(chunk);
            var raw = chunk.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');

            if (!string.Equals(cooked, piece.Value, StringComparison.Ordinal))
            {
                return $"{where}: the chunk at {chunkStart} cooks to {(cooked is null ? "no value" : "its TV")}, the library's {(piece.Value is null ? "to no value" : "to another")}";
            }

            if (!string.Equals(raw, piece.Raw, StringComparison.Ordinal))
            {
                return $"{where}: the chunk at {chunkStart} has another raw value than the library's";
            }

            if (!opens)
            {
                return pieceEnd == end ? null : $"{where}: it ends at {end}, the chunks at {pieceEnd}";
            }

            // THE SUBSTITUTION RUNS TO THE FIRST `}` A SLICE TOKENIZER OVER THE TEXT AFTER ITS `${` LEAVES UNMATCHED.
            var substitutionStart = pieceEnd;
            var close = SubstitutionEnd(text, substitutionStart, end);

            if (close < 0)
            {
                return $"{where}: the substitution at {substitutionStart} has no `}}` a slice tokenizer leaves unmatched";
            }

            var next = at;
            var depth = 0;

            while (next < shared.Count)
            {
                var kind = shared[next].Kind;

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

                next++;
            }

            if (next >= shared.Count || shared[next].Start != close)
            {
                return $"{where}: the substitution at {substitutionStart} ends at {close}, the library's at {(next >= shared.Count ? "no piece" : shared[next].Start.ToString(CultureInfo.InvariantCulture))}";
            }

            var substitution = text[substitutionStart..close];
            var prefix = text[..substitutionStart];
            var line = prefix.Count(static c => c == '\n');
            var column = substitutionStart - (prefix.LastIndexOf('\n') + 1);
            var inner = new SliceTokenizer(substitution).Tokenize();

            foreach (var part in inner.Where(static part => part.Kind != SliceTokenKind.EndOfSource))
            {
                if (at >= next)
                {
                    return $"{where}: the substitution at {substitutionStart} has more slice tokens than the library's";
                }

                if (part.Kind == SliceTokenKind.TemplateLiteral)
                {
                    // A TEMPLATE IN THE SUBSTITUTION IS ONE SLICE TOKEN AND A RUN OF THE LIBRARY'S, COMPARED AS ITS OWN.
                    templates++;
                    var nested = CompareTemplate(
                        text, substitutionStart + part.Offset, substitutionStart + part.End, part.Line + line,
                        part.Line == 1 ? part.Column + column : part.Column, shared, ref at, ref tokens, ref templates);

                    if (nested is not null)
                    {
                        return where + ", a substitution: " + nested;
                    }

                    continue;
                }

                var disagreement = CompareToken(text, part, shared[at], substitutionStart, line, column);

                if (disagreement is not null)
                {
                    return where + ", a substitution: " + disagreement;
                }

                tokens++;
                at++;
            }

            if (at != next)
            {
                return $"{where}: the substitution at {substitutionStart} has fewer slice tokens than the library's";
            }

            piece = shared[at];
            at++;
            chunkStart = close + 1;
        }
    }

    /// <summary>
    /// Where a template chunk that begins at <paramref name="start"/> ends: the offset of the first unescaped
    /// <c>${</c> or backtick before <paramref name="limit"/>, or -1.
    /// </summary>
    private static int ChunkEnd(string text, int start, int limit)
    {
        for (var index = start; index < limit; index++)
        {
            switch (text[index])
            {
                case '\\':
                    index++;
                    break;
                case '`':
                    return index;
                case '$' when index + 1 < limit && text[index + 1] == '{':
                    return index;
            }
        }

        return -1;
    }

    /// <summary>
    /// Where a substitution that begins at <paramref name="start"/> ends: the offset of the first <c>}</c> the slice
    /// tokenizer's stream over the text from there leaves unmatched, before <paramref name="limit"/>, or -1. A
    /// template nested in it is one token of that stream, so its own braces are not counted.
    /// </summary>
    private static int SubstitutionEnd(string text, int start, int limit)
    {
        var depth = 0;

        foreach (var token in new SliceTokenizer(text[start..limit]).Tokenize())
        {
            if (token.Kind == SliceTokenKind.OpenBrace)
            {
                depth++;
            }
            else if (token.Kind == SliceTokenKind.CloseBrace)
            {
                if (depth == 0)
                {
                    return start + token.Offset;
                }

                depth--;
            }
        }

        return -1;
    }

    /// <summary>
    /// The TV of a template chunk (ECMA-262 clause 12.9.6), or null where it holds a <c>NotEscapeSequence</c>: each
    /// escape cooked, a line continuation dropped, and every CR and CR LF read as LF.
    /// </summary>
    private static string? Cooked(string chunk)
    {
        var value = new StringBuilder(chunk.Length);

        for (var index = 0; index < chunk.Length; index++)
        {
            var c = chunk[index];

            if (c == '\r')
            {
                value.Append('\n');

                if (index + 1 < chunk.Length && chunk[index + 1] == '\n')
                {
                    index++;
                }

                continue;
            }

            if (c != '\\')
            {
                value.Append(c);
                continue;
            }

            if (++index >= chunk.Length)
            {
                return null;
            }

            c = chunk[index];

            switch (c)
            {
                case 'b': value.Append('\b'); break;
                case 't': value.Append('\t'); break;
                case 'n': value.Append('\n'); break;
                case 'v': value.Append('\v'); break;
                case 'f': value.Append('\f'); break;
                case 'r': value.Append('\r'); break;
                case '\r':
                    if (index + 1 < chunk.Length && chunk[index + 1] == '\n')
                    {
                        index++;
                    }

                    break;
                case '\n' or '\u2028' or '\u2029':
                    break;
                case '0' when index + 1 >= chunk.Length || !char.IsAsciiDigit(chunk[index + 1]):
                    value.Append('\0');
                    break;
                case >= '0' and <= '9':
                    return null;
                case 'x':
                    if (index + 2 >= chunk.Length || !char.IsAsciiHexDigit(chunk[index + 1]) || !char.IsAsciiHexDigit(chunk[index + 2]))
                    {
                        return null;
                    }

                    value.Append((char)int.Parse(chunk.AsSpan(index + 1, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture));
                    index += 2;
                    break;
                case 'u':
                    if (index + 1 < chunk.Length && chunk[index + 1] == '{')
                    {
                        var close = chunk.IndexOf('}', index + 2);

                        if (close < 0 || close == index + 2)
                        {
                            return null;
                        }

                        var codePoint = 0;

                        for (var digit = index + 2; digit < close; digit++)
                        {
                            if (!char.IsAsciiHexDigit(chunk[digit]) || (codePoint = (codePoint * 16) + HexValue(chunk[digit])) > 0x10FFFF)
                            {
                                return null;
                            }
                        }

                        // A surrogate code point is its one code unit (clause 11.1.4's UTF16EncodeCodePoint).
                        if (codePoint is >= 0xD800 and <= 0xDFFF)
                        {
                            value.Append((char)codePoint);
                        }
                        else
                        {
                            value.Append(char.ConvertFromUtf32(codePoint));
                        }

                        index = close;
                        break;
                    }

                    for (var digit = index + 1; digit <= index + 4; digit++)
                    {
                        if (digit >= chunk.Length || !char.IsAsciiHexDigit(chunk[digit]))
                        {
                            return null;
                        }
                    }

                    value.Append((char)int.Parse(chunk.AsSpan(index + 1, 4), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture));
                    index += 4;
                    break;
                default:
                    value.Append(c);

                    if (char.IsHighSurrogate(c) && index + 1 < chunk.Length && char.IsLowSurrogate(chunk[index + 1]))
                    {
                        value.Append(chunk[++index]);
                    }

                    break;
            }
        }

        return value.ToString();
    }

    /// <summary>The value of one hexadecimal digit.</summary>
    private static int HexValue(char digit) => digit <= '9' ? digit - '0' : (digit | 0x20) - 'a' + 10;

    /// <summary>
    /// The parse corpus's inputs, each with its goal and its text, read from the manifest as the
    /// <c>--parse-corpus</c> mode reads them: a stored input from the corpus's directory, a test262 input from the
    /// archive the manifest pins (its SHA-256 checked), every input's bytes checked against the manifest's SHA-256
    /// and decoded as the conformance root decodes a file (UTF-8, a leading byte-order mark dropped).
    /// </summary>
    private static bool TryReadCorpus(string manifest, out IReadOnlyList<(string Id, string Goal, string Text)> inputs, out string failure)
    {
        inputs = [];
        var lines = File.ReadAllText(manifest, new UTF8Encoding(false, true)).Split('\n');
        var rows = new List<string[]>();
        string? archivePath = null;
        string? archiveSha256 = null;
        var declared = -1;

        foreach (var line in lines)
        {
            if (line.StartsWith("#% pin-test262-archive\t", StringComparison.Ordinal))
            {
                var cells = line.Split('\t');
                (archivePath, archiveSha256) = cells.Length == 3 ? (cells[1], cells[2]) : (null, null);
            }
            else if (line.StartsWith("#% rows\t", StringComparison.Ordinal))
            {
                declared = int.Parse(line["#% rows\t".Length..], NumberStyles.None, CultureInfo.InvariantCulture);
            }
            else if (line.Length != 0 && line[0] != '#' && !line.StartsWith("id\t", StringComparison.Ordinal))
            {
                var cells = line.Split('\t');

                if (cells.Length != 9)
                {
                    failure = "a manifest row does not hold the nine columns of format 1: " + line[..Math.Min(line.Length, 40)];
                    return false;
                }

                rows.Add(cells);
            }
        }

        if (rows.Count != declared)
        {
            failure = $"the manifest declares {declared.ToString(CultureInfo.InvariantCulture)} rows and holds {rows.Count.ToString(CultureInfo.InvariantCulture)}";
            return false;
        }

        var corpus = Path.GetDirectoryName(Path.GetFullPath(manifest)) ?? ".";
        var suite = new Dictionary<string, byte[]>(StringComparer.Ordinal);

        if (rows.Any(static row => row[1] == "test262"))
        {
            var archive = archivePath is null ? null : FindUpwards(corpus, archivePath);

            if (archive is null)
            {
                failure = "no test262 archive at " + (archivePath ?? "the path the manifest pins");
                return false;
            }

            using (var stream = File.OpenRead(archive))
            {
                if (!string.Equals(Convert.ToHexStringLower(SHA256.HashData(stream)), archiveSha256, StringComparison.Ordinal))
                {
                    failure = "the archive at " + archive + " does not hash to the SHA-256 the manifest pins";
                    return false;
                }
            }

            using var file = File.OpenRead(archive);
            using var gzip = new GZipStream(file, CompressionMode.Decompress);
            using var reader = new TarReader(gzip);

            while (reader.GetNextEntry() is { } entry)
            {
                var slash = entry.Name.IndexOf('/', StringComparison.Ordinal);

                if (entry.EntryType is not (TarEntryType.RegularFile or TarEntryType.V7RegularFile) || entry.DataStream is null || slash < 0)
                {
                    continue;
                }

                using var copy = new MemoryStream();
                entry.DataStream.CopyTo(copy);
                suite[entry.Name[(slash + 1)..]] = copy.ToArray();
            }
        }

        var read = new List<(string Id, string Goal, string Text)>(rows.Count);

        foreach (var row in rows)
        {
            var (id, origin, path, sha256, goal) = (row[0], row[1], row[2], row[3], row[4]);
            byte[]? bytes;

            if (origin == "test262")
            {
                suite.TryGetValue(path, out bytes);
            }
            else
            {
                var stored = Path.Combine(corpus, path.Replace('/', Path.DirectorySeparatorChar));
                bytes = File.Exists(stored) ? File.ReadAllBytes(stored) : null;
            }

            if (bytes is null)
            {
                failure = id + ": no input at " + path;
                return false;
            }

            if (!string.Equals(Convert.ToHexStringLower(SHA256.HashData(bytes)), sha256, StringComparison.Ordinal))
            {
                failure = id + ": " + path + " does not hash to the manifest's SHA-256";
                return false;
            }

            var start = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF ? 3 : 0;
            read.Add((id, goal, Encoding.UTF8.GetString(bytes, start, bytes.Length - start)));
        }

        inputs = read;
        failure = string.Empty;
        return true;
    }

    /// <summary>A repository path found by walking up from a directory, or null.</summary>
    private static string? FindUpwards(string start, string relative)
    {
        for (var directory = new DirectoryInfo(start); directory is not null; directory = directory.Parent)
        {
            var candidate = Path.Combine(directory.FullName, relative.Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
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

    /// <summary>
    /// The front end's mapping, read from the checkout the parse corpus's manifest stands in: each shared code's catalogue
    /// row and registry code, from its <c>code</c> rows; null where the file is not there.
    /// </summary>
    private static Dictionary<string, (string Row, string Registry)>? ReadMapping(string manifest)
    {
        var root = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(manifest)))));
        var path = root is null ? null : Path.Combine(root, "src", "Broiler.VM.Profile.JavaScript", "docs", "diagnostics", "frontend-mapping.tsv");

        if (path is null || !File.Exists(path))
        {
            return null;
        }

        var rows = new Dictionary<string, (string Row, string Registry)>(StringComparer.Ordinal);

        foreach (var line in File.ReadLines(path))
        {
            if (line.StartsWith('#'))
            {
                continue;
            }

            var cells = line.Split('\t');

            if (cells.Length > 6 && string.Equals(cells[0], "code", StringComparison.Ordinal))
            {
                rows[cells[1]] = (cells[2], cells[6]);
            }
        }

        return rows;
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
