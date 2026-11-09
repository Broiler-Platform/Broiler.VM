// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   6
// Annotated:        6/6
// Exempt:           0
// Human-reviewed:   0/6
// IP risk:          Low
// Security risk:    Medium
// Criteria:         4/0
// Resource impact:  1/10 max
// Unverified:       6
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.ECMAScript.Syntax;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The texts of <see cref="JsFrontendDiagnostics"/>: one sentence for each shared code, stating that code's own
/// condition, and its rendering with the library's arguments and the source's words (decision D11, as
/// JSD-0083's note of 2026-11-09, later that day, on decision D11 states it: one true text per code). <i>(Revised 2026-11-09, later that day: it
/// read "each shared code's sentence as this family rendered it at step 1's base"; the base's sites that the comments
/// beside the texts kept from then name are read at Broiler.VM <c>d82b07ad</c>, an ancestor of the base, where the
/// catalogue's completeness report read them, and not at the base itself.)</i>
/// </summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=88B6FF
// Broiler-Falsified-If: a shared code is rendered with a sentence that is false of a source the library refuses under that code
// Broiler-Human:        PENDING
internal static partial class JsFrontendDiagnostics
{
    /// <summary>
    /// The text of one library diagnostic: its shared code's sentence from <see cref="Texts"/>, with the
    /// library's arguments and the source's words put in (decision D11).
    /// </summary>
    /// <remarks>
    /// <b>A code the table holds no sentence for renders its message identifier and arguments</b>, as the
    /// library reports them. From JSD-0083's note of 2026-11-09, later that day, on decision D11 the table holds a sentence for every shared
    /// code of the mapping, so only a code the mapping does not hold, which <see cref="JsFrontendDiagnostics"/> refuses
    /// as a defect of the build before it renders, could reach this. <i>(Revised 2026-11-09, later that day: it went on
    /// "no text of this family covers its rule, so none is borrowed for it", which 45 codes did.)</i>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CC93E7
    // Broiler-Falsified-If: a diagnostic whose shared code the table holds renders anything but that sentence with its placeholders filled, or a placeholder reads outside the source text
    // Broiler-Human:        PENDING
    private static string Render(SyntaxDiagnostic diagnostic, string source)
    {
        var arguments = diagnostic.Arguments;

        if (!Texts.TryGetValue(diagnostic.Code, out var template))
        {
            return diagnostic.MessageId + (arguments.Count == 0 ? string.Empty : ": " + string.Join(", ", arguments));
        }

        var text = new System.Text.StringBuilder(template.Length + 16);

        for (var index = 0; index < template.Length; index++)
        {
            var close = template[index] == '{' ? template.IndexOf('}', index) : -1;

            if (close < 0)
            {
                text.Append(template[index]);
                continue;
            }

            var name = template[(index + 1)..close];
            index = close;

            if (int.TryParse(name, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var which))
            {
                text.Append(which < arguments.Count ? arguments[which] : string.Empty);
            }
            else if (name == "tok")
            {
                // The token the library names, or, where it names none (a refusal of a node at its span), the
                // source text the span covers; the end of the source only where the span stands there.
                text.Append(arguments.Count != 0 && arguments[0].Length != 0 ? arguments[0] : Found(diagnostic.Span, source));
            }
            else if (name == "found")
            {
                text.Append(Found(diagnostic.Span, source));
            }
            else if (name == "cp")
            {
                text.Append(CodePoint(diagnostic.Span, source));
            }
        }

        return text.ToString();
    }

    /// <summary>What this family's texts call the place past the last token.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=05F545
    // Broiler-Human:        PENDING
    private const string EndOfSource = "end of source";

    /// <summary>The source text a span covers, up to its first line terminator, or the end of the source.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=954740
    // Broiler-Falsified-If: a span outside the source text is read
    // Broiler-Human:        PENDING
    private static string Found(SourceSpan span, string source)
    {
        if (span.Length <= 0 || span.Offset < 0 || span.Offset >= source.Length)
        {
            return EndOfSource;
        }

        var text = source.Substring(span.Offset, System.Math.Min(span.Length, source.Length - span.Offset));
        var end = text.IndexOfAny(['\n', '\r', '\u2028', '\u2029']);

        return end < 0 ? text : text[..end];
    }

    /// <summary>
    /// The code point a span names, as U+ and four or more hexadecimal digits: a unicode escape's value,
    /// or the span's first character.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=13A715
    // Broiler-Falsified-If: an escape's code point and the rendered one differ, or a span outside the source text is read
    // Broiler-Human:        PENDING
    private static string CodePoint(SourceSpan span, string source)
    {
        var at = span.Offset;

        if (at < 0 || at >= source.Length)
        {
            return EndOfSource;
        }

        var value = -1;

        if (source[at] == '\\' && at + 1 < source.Length && source[at + 1] == 'u')
        {
            var digits = at + 2;
            var braced = digits < source.Length && source[digits] == '{';
            var first = braced ? digits + 1 : digits;
            var last = first;

            while (last < source.Length && (braced ? last - first < 8 : last - first < 4) && char.IsAsciiHexDigit(source[last]))
            {
                last++;
            }

            if (last > first && int.TryParse(source.Substring(first, last - first), System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.InvariantCulture, out var parsed))
            {
                value = parsed;
            }
        }

        if (value < 0)
        {
            value = char.IsHighSurrogate(source[at]) && at + 1 < source.Length && char.IsLowSurrogate(source[at + 1])
                ? char.ConvertToUtf32(source[at], source[at + 1])
                : source[at];
        }

        return "U+" + value.ToString("X4", System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Each shared code's text: one sentence that states the code's own condition, true of every source the library
    /// refuses under it (decision D11, as JSD-0083's note of 2026-11-09, later that day, on decision D11 states it).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>One true text per code.</b> The library's shared codes are coarser than the sites of the wide front end that
    /// step 1 deleted, so a sentence one of those sites wrote can be false of a source another refuses under the same
    /// code. Each text here states the condition the code's catalogue row describes, in a wording general enough to be
    /// true wherever the library reports that row; a base text that already did so is kept, and the comment beside it
    /// still names where it was read. The texts the base gave each site retire. A text of a manifest refusal ends with
    /// the suffix " is not admitted by the declared feature manifest", which states its condition. Where a text holds
    /// the source's own words, a placeholder takes their place: <c>{0}</c> and on are the library's arguments,
    /// <c>{found}</c> the source text the diagnostic's span covers, <c>{tok}</c> the first argument, and <c>{cp}</c> the
    /// code point the span names; <c>{tok}</c> falls back on <c>{found}</c> where the library names no token. A comment
    /// "written 2026-11-09 (D11)" marks a text written then. <i>(Revised 2026-11-09, later that day, with the owner's
    /// decision "One true text per code": it read "<b>Read from the base, not written anew.</b> For a code the base
    /// refused at the library's place and with the code the mapping gives, the text is the one the base rendered there,
    /// the most frequent where the base's sites rendered several; for any other code, the text of the wide front end's
    /// site that the catalogue's completeness report assigns to the code's row [...]. A code no site of this family
    /// refused renders its message identifier and arguments, as no text of this family covers it.")</i>
    /// </para>
    /// <para>
    /// <b>Nothing here is mutable</b> (rule N12): one frozen table, read by <see cref="Render"/> alone.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Low; Resources=1; Fingerprint=ED4CFB
    // Broiler-Human:        PENDING
    private static readonly System.Collections.Frozen.FrozenDictionary<string, string> Texts =
        System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary(
            new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal)
            {
                ["JSE1201"] = "a unicode escape in an identifier names {cp}, which may not start an identifier", // identifier.escape.invalid-start, curated
                ["JSE1202"] = "a unicode escape in an identifier names a code point that may not continue an identifier", // identifier.escape.invalid-part, written 2026-11-09 (D11)
                ["JSE1203"] = "a legacy octal literal, or a decimal literal with a leading zero, is not admitted in strict code", // strict.legacy-octal-literal, written 2026-11-09 (D11)
                ["JSE1204"] = "a legacy octal escape or `\\8` or `\\9` in a string literal is not admitted in strict code", // strict.legacy-octal-escape, observed
                ["JSE1301"] = "`{0}` is not a binding name in strict code", // strict.binding.eval-or-arguments, observed
                ["JSE1302"] = "`{0}` is reserved in strict code, so it is not an identifier there", // strict.reserved-word, written 2026-11-09 (D11)
                ["JSE1303"] = "`{0}` is reserved in a module, so it is not an identifier there", // identifier.await.in-module, written 2026-11-09 (D11)
                ["JSE1304"] = "`{0}` is not a binding name inside a generator", // identifier.yield.in-generator, written 2026-11-09 (D11)
                ["JSE1305"] = "`{0}` is not a binding name inside an async function or a class static block", // identifier.await.in-async, written 2026-11-09 (D11)
                ["JSE1306"] = "`{0}` is reserved inside a generator, so it is not an identifier there, escaped or not", // identifier.yield.in-generator, written 2026-11-09 (D11)
                ["JSE1307"] = "`{0}` is reserved inside an async function or a class static block, so it is not an identifier there, escaped or not", // identifier.await.in-async, written 2026-11-09 (D11)
                ["JSE1308"] = "`{0}` is reserved in strict code, so it is not an identifier there", // strict.reserved-word, written 2026-11-09 (D11)
                ["JSE1309"] = "`{0}` is reserved in a module, so it is not an identifier there, escaped or not", // identifier.await.in-module, written 2026-11-09 (D11)
                ["JSE1310"] = "`{0}` is a reserved word here, and a unicode escape does not make it a name", // identifier.reserved-word.escaped, observed
                ["JSE1311"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1312"] = "`{0}` is a private name, which names a method only in a class body and not in an object literal", // object.method.private-name, written 2026-11-09 (D11)
                ["JSE1313"] = "`{0} = …` in an object literal is a shorthand with an initialiser, which only a destructuring pattern admits", // expression.shorthand-initializer, written 2026-11-09 (D11)
                ["JSE1314"] = "an object literal may set `__proto__` once", // object.proto.duplicate, d82b07ad's JsCompiler.Literals.cs:372
                ["JSE1315"] = "{0}", // regexp.literal.invalid, curated
                ["JSE1316"] = "a template that is not tagged holds an escape sequence that only a tagged template admits", // template.escape.invalid, written 2026-11-09 (D11)
                ["JSE1317"] = "a template literal holds 2^32 or more strings, more than a template may hold", // template.too-many-strings, written 2026-11-09 (D11)
                ["JSE1318"] = "a template that is not tagged holds an escape sequence that only a tagged template admits", // template.escape.invalid, written 2026-11-09 (D11)
                ["JSE1319"] = "a template that is not tagged holds an escape sequence that only a tagged template admits", // template.escape.invalid, written 2026-11-09 (D11)
                ["JSE1320"] = "a template that is not tagged holds an escape sequence that only a tagged template admits", // template.escape.invalid, written 2026-11-09 (D11)
                ["JSE1321"] = "a parenthesised list must be an expression unless `=>` follows it on the same line", // cover.parenthesized-expression, written 2026-11-09 (D11)
                ["JSE1322"] = "a tagged template has no reading inside an optional chain", // expression.optional-chain.tagged-template, observed
                ["JSE1323"] = "`{0}` names a module's own metadata and this source was presented as a script", // meta.import-meta.outside-module, observed
                ["JSE1324"] = "the operand of an update operator is not a reference", // assignment.target.invalid, written 2026-11-09 (D11)
                ["JSE1325"] = "the operand of an update operator is not a reference", // assignment.target.invalid, curated
                ["JSE1326"] = "in strict code, `delete` cannot be applied to a bare name or to a private member, parenthesised or not", // strict.delete.unqualified-or-private, written 2026-11-09 (D11)
                ["JSE1327"] = "in strict code, `delete` cannot be applied to a bare name or to a private member, parenthesised or not", // strict.delete.unqualified-or-private, written 2026-11-09 (D11)
                ["JSE1328"] = "an object or array literal on the left of `=` must be a valid destructuring pattern", // cover.assignment-pattern, written 2026-11-09 (D11)
                ["JSE1329"] = "the left-hand side of an assignment is not a reference", // assignment.target.invalid, observed
                ["JSE1330"] = "the left-hand side of an assignment is not a reference", // assignment.target.invalid, observed
                ["JSE1331"] = "the left-hand side of an assignment is not a reference", // assignment.target.not-simple, observed
                ["JSE1332"] = "`{0}` is not the target of a destructuring assignment in strict code", // assignment.target.not-simple, observed
                ["JSE1333"] = "the target of a rest property in a destructuring assignment cannot be an object or array pattern", // assignment.rest-property.nested-pattern, written 2026-11-09 (D11)
                ["JSE1334"] = "an object or array literal that is a destructuring target must be a valid destructuring pattern", // cover.assignment-pattern, written 2026-11-09 (D11)
                ["JSE1335"] = "a destructuring assignment target is not a reference", // assignment.target.not-simple, written 2026-11-09 (D11)
                ["JSE1401"] = "`{0}` is declared twice in one scope", // scope.lexical.duplicate, observed
                ["JSE1402"] = "`{0}` is declared both lexically and as a `var`", // scope.lexical.var-collision, observed
                ["JSE1403"] = "`{0}` is not a name a `let` or `const` declaration may bind", // declaration.let.bound-name, curated
                ["JSE1404"] = "`{0}` is declared twice by one declaration", // scope.lexical.duplicate, d82b07ad's JsParser.cs:3998
                ["JSE1405"] = "`{0} {found}` needs an initialiser", // declaration.const.missing-initializer, curated
                ["JSE1406"] = "a labelled function declaration cannot be the body of an `if` statement", // statement.labelled-function, written 2026-11-09 (D11)
                ["JSE1407"] = "a labelled function declaration cannot be the body of an `if` statement", // statement.labelled-function, written 2026-11-09 (D11)
                ["JSE1408"] = "a labelled function declaration cannot be the body of an `if` statement", // statement.labelled-function, written 2026-11-09 (D11)
                ["JSE1409"] = "a labelled function declaration cannot be the body of a loop", // statement.labelled-function, written 2026-11-09 (D11)
                ["JSE1410"] = "a labelled function declaration cannot be the body of a loop", // statement.labelled-function, written 2026-11-09 (D11)
                ["JSE1411"] = "a labelled function declaration cannot be the body of a loop", // statement.labelled-function, written 2026-11-09 (D11)
                ["JSE1412"] = "`{0}` is declared by this loop's head, so its body cannot declare it with `var`", // scope.lexical.var-collision, curated
                ["JSE1413"] = "a labelled function declaration cannot be the body of a loop", // statement.labelled-function, written 2026-11-09 (D11)
                ["JSE1414"] = "an object or array literal on the left of a `for … in` or `for … of` head must be a valid destructuring pattern", // cover.assignment-pattern, written 2026-11-09 (D11)
                ["JSE1415"] = "the left-hand side of a `for … in` or `for … of` head is not a reference", // assignment.target.invalid, written 2026-11-09 (D11)
                ["JSE1416"] = "`{0}` is not a name a `let` or `const` declaration may bind", // declaration.let.bound-name, curated
                ["JSE1417"] = "`{0}` is declared by this loop's head, so its body cannot declare it with `var`", // scope.lexical.var-collision, observed
                ["JSE1418"] = "`{0}` is declared twice by one declaration", // scope.lexical.duplicate, d82b07ad's JsParser.cs:3998
                ["JSE1419"] = "`continue` stands outside every loop", // jump.continue.no-iteration, written 2026-11-09 (D11)
                ["JSE1420"] = "`break` with no label stands outside every loop and `switch`", // jump.break.no-target, written 2026-11-09 (D11)
                ["JSE1421"] = "the `{0}` statement is a syntax error in strict code", // strict.with, observed
                ["JSE1422"] = "a labelled function declaration cannot be the body of a `with` statement", // statement.labelled-function, written 2026-11-09 (D11)
                ["JSE1423"] = "`{0}` is declared twice in one scope", // scope.lexical.duplicate, observed
                ["JSE1424"] = "`{0}` is declared both lexically and as a `var`", // scope.lexical.var-collision, observed
                ["JSE1425"] = "a function declaration cannot be labelled in strict code", // strict.labelled-function, written 2026-11-09 (D11)
                ["JSE1426"] = "`{0}` is bound twice by one `catch` parameter", // scope.catch.duplicate, d82b07ad's JsParser.cs:3893
                ["JSE1427"] = "`{0}` is the `catch` parameter, so the handler cannot declare it again lexically", // scope.catch.lexical-collision, written 2026-11-09 (D11)
                ["JSE1428"] = "`{0}` is bound by a destructuring `catch` parameter, so the handler cannot declare it with `var`", // scope.catch.var-collision, d82b07ad's JsParser.cs:3935
                ["JSE1501"] = "`{0}` is bound twice by one parameter list", // scope.parameter.duplicate, curated
                ["JSE1502"] = "`{0}` is bound twice by one parameter list", // scope.parameter.duplicate, curated
                ["JSE1503"] = "`{0}` is bound twice by one parameter list", // scope.parameter.duplicate, curated
                ["JSE1504"] = "`{0}` is not a function name in strict code", // strict.binding.eval-or-arguments, d82b07ad's JsParser.cs:2363
                ["JSE1505"] = "a body cannot declare `use strict` over a parameter list that has a default, a rest parameter or a pattern in it", // function.use-strict.non-simple-parameters, d82b07ad's JsParser.cs:4120
                ["JSE1506"] = "`{0}` is a parameter of this function, so its body cannot declare it again lexically", // scope.parameter.lexical-collision, written 2026-11-09 (D11)
                ["JSE1507"] = "`super` is only admitted inside a method, a class field initialiser or a class static block", // super.property.outside-method, written 2026-11-09 (D11)
                ["JSE1508"] = "`super` is only admitted inside a method, a class field initialiser or a class static block", // super.property.outside-method, written 2026-11-09 (D11)
                ["JSE1509"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1510"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1511"] = "`{0}` is declared twice in one scope", // scope.lexical.duplicate, d82b07ad's JsParser.cs:3745
                ["JSE1512"] = "`{0}` is declared both lexically and as a `var`", // scope.lexical.var-collision, d82b07ad's JsParser.cs:3757
                ["JSE1513"] = "the label `{0}` is already in force here, so a statement inside it cannot declare it again", // jump.label.duplicate, d82b07ad's JsParser.cs:580
                ["JSE1514"] = "`break` names a label that does not enclose it", // jump.break.undefined-label, written 2026-11-09 (D11)
                ["JSE1515"] = "`continue` names a label that is not on a loop enclosing it", // jump.continue.undefined-label, written 2026-11-09 (D11)
                ["JSE1516"] = "a `yield` expression is not admitted in a function's parameters", // function.parameters.yield, observed
                ["JSE1517"] = "an `await` expression is not admitted in a function's parameters", // function.parameters.await, observed
                ["JSE1518"] = "a body cannot declare `use strict` over a parameter list that has a default, a rest parameter or a pattern in it", // function.use-strict.non-simple-parameters, d82b07ad's JsParser.cs:4120
                ["JSE1519"] = "`{0}` is a parameter of this function, so its body cannot declare it again lexically", // scope.parameter.lexical-collision, written 2026-11-09 (D11)
                ["JSE1520"] = "the parenthesised list before `=>` is not a valid parameter list", // cover.arrow-formal-parameters, written 2026-11-09 (D11)
                ["JSE1521"] = "a body cannot declare `use strict` over a parameter list that has a default, a rest parameter or a pattern in it", // function.use-strict.non-simple-parameters, d82b07ad's JsParser.cs:4120
                ["JSE1522"] = "`{0}` is a parameter of this function, so its body cannot declare it again lexically", // scope.parameter.lexical-collision, written 2026-11-09 (D11)
                ["JSE1523"] = "`{0}` is bound twice by one parameter list", // scope.parameter.duplicate, d82b07ad's JsParser.cs:4161
                ["JSE1524"] = "a body cannot declare `use strict` over a parameter list that has a default, a rest parameter or a pattern in it", // function.use-strict.non-simple-parameters, d82b07ad's JsParser.cs:4120
                ["JSE1525"] = "`{0}` is a parameter of this function, so its body cannot declare it again lexically", // scope.parameter.lexical-collision, written 2026-11-09 (D11)
                ["JSE1526"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1527"] = "a `yield` expression is not admitted in a function's parameters", // function.parameters.yield, observed
                ["JSE1528"] = "a body cannot declare `use strict` over a parameter list that has a default, a rest parameter or a pattern in it", // function.use-strict.non-simple-parameters, d82b07ad's JsParser.cs:4120
                ["JSE1529"] = "`{0}` is a parameter of this function, so its body cannot declare it again lexically", // scope.parameter.lexical-collision, written 2026-11-09 (D11)
                ["JSE1530"] = "`{0}` is bound twice by one parameter list", // scope.parameter.duplicate, curated
                ["JSE1531"] = "`{0}` is not a function name in strict code", // strict.binding.eval-or-arguments, d82b07ad's JsParser.cs:2363
                ["JSE1532"] = "a body cannot declare `use strict` over a parameter list that has a default, a rest parameter or a pattern in it", // function.use-strict.non-simple-parameters, d82b07ad's JsParser.cs:4120
                ["JSE1533"] = "`{0}` is a parameter of this function, so its body cannot declare it again lexically", // scope.parameter.lexical-collision, written 2026-11-09 (D11)
                ["JSE1534"] = "a `yield` expression is not admitted in a function's parameters", // function.parameters.yield, observed
                ["JSE1535"] = "`super` is only admitted inside a method, a class field initialiser or a class static block", // super.property.outside-method, written 2026-11-09 (D11)
                ["JSE1536"] = "`super` is only admitted inside a method, a class field initialiser or a class static block", // super.property.outside-method, written 2026-11-09 (D11)
                ["JSE1537"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1538"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1539"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1540"] = "a `yield` expression is not admitted in a function's parameters", // function.parameters.yield, d82b07ad's JsParser.cs:4526
                ["JSE1541"] = "an `await` expression is not admitted in a function's parameters", // function.parameters.await, d82b07ad's JsParser.cs:5191
                ["JSE1542"] = "a body cannot declare `use strict` over a parameter list that has a default, a rest parameter or a pattern in it", // function.use-strict.non-simple-parameters, d82b07ad's JsParser.cs:4120
                ["JSE1543"] = "`{0}` is a parameter of this function, so its body cannot declare it again lexically", // scope.parameter.lexical-collision, written 2026-11-09 (D11)
                ["JSE1544"] = "`{0}` is bound twice by one parameter list", // scope.parameter.duplicate, d82b07ad's JsParser.cs:4161
                ["JSE1545"] = "`{0}` is not a function name in strict code", // strict.binding.eval-or-arguments, d82b07ad's JsParser.cs:2363
                ["JSE1546"] = "a body cannot declare `use strict` over a parameter list that has a default, a rest parameter or a pattern in it", // function.use-strict.non-simple-parameters, d82b07ad's JsParser.cs:4120
                ["JSE1547"] = "`{0}` is a parameter of this function, so its body cannot declare it again lexically", // scope.parameter.lexical-collision, written 2026-11-09 (D11)
                ["JSE1548"] = "a `yield` expression is not admitted in a function's parameters", // function.parameters.yield, observed
                ["JSE1549"] = "an `await` expression is not admitted in a function's parameters", // function.parameters.await, observed
                ["JSE1550"] = "`super` is only admitted inside a method, a class field initialiser or a class static block", // super.property.outside-method, written 2026-11-09 (D11)
                ["JSE1551"] = "`super` is only admitted inside a method, a class field initialiser or a class static block", // super.property.outside-method, written 2026-11-09 (D11)
                ["JSE1552"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1553"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1554"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1555"] = "a class body declares one `{0}`, and this is a second", // class.constructor.duplicate, observed
                ["JSE1556"] = "`{0}` is declared more than once by this class body", // class.private-name.duplicate, observed
                ["JSE1557"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1558"] = "a class's `constructor` cannot be an accessor, a generator or an async method", // class.constructor.special-method, written 2026-11-09 (D11)
                ["JSE1559"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1560"] = "the class definition declares `prototype` on the constructor, so a static member cannot declare it again", // class.static-method.prototype, d82b07ad's JsParser.cs:3028
                ["JSE1561"] = "a class field cannot be named `constructor`", // class.field.constructor, written 2026-11-09 (D11)
                ["JSE1562"] = "a static class field cannot be named `prototype` or `constructor`", // class.static-field.prototype-or-constructor, written 2026-11-09 (D11)
                ["JSE1563"] = "`arguments` names nothing inside a class field initialiser or a static block", // class.initializer.arguments, d82b07ad's JsCompiler.Classes.cs:348
                ["JSE1564"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1565"] = "`{0}` is a private name a class body reserves and cannot declare", // class.private-name.constructor, observed
                ["JSE1566"] = "`{0}` is declared twice in one scope", // scope.lexical.duplicate, observed
                ["JSE1567"] = "`{0}` is declared both lexically and as a `var`", // scope.lexical.var-collision, curated
                ["JSE1568"] = "the label `{0}` is already in force here, so a statement inside it cannot declare it again", // jump.label.duplicate, observed
                ["JSE1569"] = "`break` names a label that does not enclose it", // jump.break.undefined-label, written 2026-11-09 (D11)
                ["JSE1570"] = "`continue` names a label that is not on a loop enclosing it", // jump.continue.undefined-label, written 2026-11-09 (D11)
                ["JSE1571"] = "`arguments` names nothing inside a class field initialiser or a static block", // class.initializer.arguments, d82b07ad's JsCompiler.Classes.cs:348
                ["JSE1572"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1573"] = "`{0}` is a reserved word in a class static block", // class.static-block.await, observed
                ["JSE1574"] = "a body cannot declare `use strict` over a parameter list that has a default, a rest parameter or a pattern in it", // function.use-strict.non-simple-parameters, d82b07ad's JsParser.cs:4120
                ["JSE1575"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1576"] = "an `await` expression is not admitted in a function's parameters", // function.parameters.await, d82b07ad's JsParser.cs:5191
                ["JSE1577"] = "`{0}` is a parameter of this function, so its body cannot declare it again lexically", // scope.parameter.lexical-collision, written 2026-11-09 (D11)
                ["JSE1578"] = "a body cannot declare `use strict` over a parameter list that has a default, a rest parameter or a pattern in it", // function.use-strict.non-simple-parameters, d82b07ad's JsParser.cs:4120
                ["JSE1579"] = "an `await` expression is not admitted in a function's parameters", // function.parameters.await, d82b07ad's JsParser.cs:5191
                ["JSE1580"] = "`{0}` is bound twice by one parameter list", // scope.parameter.duplicate, curated
                ["JSE1581"] = "`{0}` is not a function name in strict code", // strict.binding.eval-or-arguments, d82b07ad's JsParser.cs:2363
                ["JSE1582"] = "`{0}` is a parameter of this function, so its body cannot declare it again lexically", // scope.parameter.lexical-collision, written 2026-11-09 (D11)
                ["JSE1583"] = "`super` is only admitted inside a method, a class field initialiser or a class static block", // super.property.outside-method, written 2026-11-09 (D11)
                ["JSE1584"] = "`super` is only admitted inside a method, a class field initialiser or a class static block", // super.property.outside-method, written 2026-11-09 (D11)
                ["JSE1585"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1586"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1587"] = "`{0}` is a parameter of this function, so its body cannot declare it again lexically", // scope.parameter.lexical-collision, written 2026-11-09 (D11)
                ["JSE1588"] = "the head of an async arrow function is not a valid parameter list", // cover.async-arrow-head, written 2026-11-09 (D11)
                ["JSE1589"] = "a `yield` expression is not admitted in a function's parameters", // function.parameters.yield, d82b07ad's JsParser.cs:4526
                ["JSE1590"] = "an `await` expression is not admitted in a function's parameters", // function.parameters.await, d82b07ad's JsParser.cs:5191
                ["JSE1591"] = "`{0}` is a parameter of this function, so its body cannot declare it again lexically", // scope.parameter.lexical-collision, written 2026-11-09 (D11)
                ["JSE1592"] = "a body cannot declare `use strict` over a parameter list that has a default, a rest parameter or a pattern in it", // function.use-strict.non-simple-parameters, d82b07ad's JsParser.cs:4120
                ["JSE1601"] = "`{0}` is declared twice in one scope", // scope.lexical.duplicate, observed
                ["JSE1602"] = "`{0}` is declared both lexically and as a `var`", // scope.lexical.var-collision, d82b07ad's JsParser.cs:3757
                ["JSE1603"] = "`super` is only admitted inside a method, a class field initialiser or a class static block", // super.outside-method, written 2026-11-09 (D11)
                ["JSE1604"] = "`{0}` names how a function was called and no function encloses this one", // meta.new-target.outside-function, observed
                ["JSE1605"] = "the label `{0}` is already in force here, so a statement inside it cannot declare it again", // jump.label.duplicate, d82b07ad's JsParser.cs:580
                ["JSE1606"] = "`break` names a label that does not enclose it", // jump.break.undefined-label, written 2026-11-09 (D11)
                ["JSE1607"] = "`continue` names a label that is not on a loop enclosing it", // jump.continue.undefined-label, written 2026-11-09 (D11)
                ["JSE1608"] = "`{0}` is not declared by any class this expression is inside of", // class.private-name.undeclared, d82b07ad's JsCompiler.Classes.cs:420
                ["JSE1609"] = "`{0}` is declared twice in one scope", // scope.lexical.duplicate, observed
                ["JSE1610"] = "`{0}` is declared both lexically and as a `var`", // scope.lexical.var-collision, curated
                ["JSE1611"] = "`{0}` is exported twice", // module.export.duplicate, d82b07ad's JsCompiler.Modules.cs:477
                ["JSE1612"] = "`{0}` is exported and this module declares no such binding", // module.export.undeclared, d82b07ad's JsCompiler.Modules.cs:523
                ["JSE1613"] = "`super` is only admitted inside a method, a class field initialiser or a class static block", // super.outside-method, written 2026-11-09 (D11)
                ["JSE1614"] = "`{0}` names how a function was called and no function encloses this one", // meta.new-target.outside-function, observed
                ["JSE1615"] = "the label `{0}` is already in force here, so a statement inside it cannot declare it again", // jump.label.duplicate, curated
                ["JSE1616"] = "`break` names a label that does not enclose it", // jump.break.undefined-label, written 2026-11-09 (D11)
                ["JSE1617"] = "`continue` names a label that is not on a loop enclosing it", // jump.continue.undefined-label, written 2026-11-09 (D11)
                ["JSE1618"] = "`{0}` is not declared by any class this expression is inside of", // class.private-name.undeclared, d82b07ad's JsCompiler.Classes.cs:420
                ["JSE1619"] = "an export name holds a lone surrogate, which is not well-formed Unicode", // module.export-name.ill-formed, observed
                ["JSE1620"] = "`{0}` is imported twice", // scope.lexical.duplicate, d82b07ad's JsCompiler.Modules.cs:150
                ["JSE1621"] = "the import attribute `{0}` is given twice", // module.import-attribute.duplicate-key, observed
                ["JSE1622"] = "a string names no local binding, so it is exported only with `from`", // module.export.string-local-name, observed
                ["JSE1623"] = "`{0}` is a reserved word, so it names no local binding to export", // module.export.reserved-local-name, written 2026-11-09 (D11)
                ["JSE2201"] = "a regular expression has 2^32 - 1 or more capturing groups", // regexp.pattern.too-many-groups, written 2026-11-09 (D11)
                ["JSE2202"] = "a regular expression gives two capturing groups that might both participate one name", // regexp.group-name.duplicate, written 2026-11-09 (D11)
                ["JSE2203"] = "a quantifier's minimum is greater than its maximum", // regexp.quantifier.out-of-order, written 2026-11-09 (D11)
                ["JSE2204"] = "a regular-expression modifier names a flag twice", // regexp.modifiers.repeated-flag, written 2026-11-09 (D11)
                ["JSE2205"] = "a regular-expression modifier group with `-` names no flag on either side", // regexp.modifiers.empty, written 2026-11-09 (D11)
                ["JSE2206"] = "a regular-expression modifier names a flag to add twice", // regexp.modifiers.repeated-flag, written 2026-11-09 (D11)
                ["JSE2207"] = "a regular-expression modifier names a flag to remove twice", // regexp.modifiers.repeated-flag, written 2026-11-09 (D11)
                ["JSE2208"] = "a regular-expression modifier both adds and removes one flag", // regexp.modifiers.added-and-removed, written 2026-11-09 (D11)
                ["JSE2209"] = "a named backreference names no capturing group", // regexp.backreference.undefined-group-name, written 2026-11-09 (D11)
                ["JSE2210"] = "a backreference's number is greater than the number of capturing groups", // regexp.backreference.group-number-too-large, written 2026-11-09 (D11)
                ["JSE2211"] = "a character class range has a class escape as an endpoint", // regexp.class.range-endpoint-is-class, written 2026-11-09 (D11)
                ["JSE2212"] = "a character class range's first character is greater than its last", // regexp.class.range-out-of-order, written 2026-11-09 (D11)
                ["JSE2213"] = "a character class range has a class escape as an endpoint", // regexp.class.range-endpoint-is-class, written 2026-11-09 (D11)
                ["JSE2214"] = "a character class range's first character is greater than its last", // regexp.class.range-out-of-order, written 2026-11-09 (D11)
                ["JSE2215"] = "a capturing group's name begins with a code point that cannot start an identifier", // regexp.group-name.invalid-start, written 2026-11-09 (D11)
                ["JSE2216"] = "a capturing group's name begins with a code point that cannot start an identifier", // regexp.group-name.invalid-start, written 2026-11-09 (D11)
                ["JSE2217"] = "a capturing group's name holds a code point that cannot continue an identifier", // regexp.group-name.invalid-part, written 2026-11-09 (D11)
                ["JSE2218"] = "a capturing group's name holds a code point that cannot continue an identifier", // regexp.group-name.invalid-part, written 2026-11-09 (D11)
                ["JSE2219"] = "a Unicode property escape names a property that does not exist", // regexp.property.unknown-name, written 2026-11-09 (D11)
                ["JSE2220"] = "a Unicode property escape names a value its property does not have", // regexp.property.unknown-value, written 2026-11-09 (D11)
                ["JSE2221"] = "a Unicode property escape names neither a binary property nor a General_Category value", // regexp.property.unknown-lone-name, written 2026-11-09 (D11)
                ["JSE2222"] = "a property of strings is admitted only with the `v` flag", // regexp.property.of-strings-outside-v-mode, written 2026-11-09 (D11)
                ["JSE2223"] = "a negated Unicode property escape names a property of strings", // regexp.property.negated-strings, written 2026-11-09 (D11)
                ["JSE2224"] = "a negated character class may contain strings", // regexp.class.negated-may-contain-strings, written 2026-11-09 (D11)
                ["JSE2225"] = "a negated character class may contain strings", // regexp.class.negated-may-contain-strings, written 2026-11-09 (D11)
                ["JSE2226"] = "a character class range's first character is greater than its last", // regexp.class.range-out-of-order, written 2026-11-09 (D11)
                ["JSE8101"] = "a braced quantifier has no atom to repeat", // regexp.quantifier.braced-without-atom, written 2026-11-09 (D11)
                ["JSE8102"] = "in Unicode mode, a character class range cannot have a class escape as an endpoint", // regexp.class.range-endpoint-is-class, written 2026-11-09 (D11)
                ["JSE8103"] = "a character class range's first character is greater than its last", // regexp.class.range-out-of-order, written 2026-11-09 (D11)
                ["JSE8104"] = "in Unicode mode, a character class range cannot have a class escape as an endpoint", // regexp.class.range-endpoint-is-class, written 2026-11-09 (D11)
                ["JSE8105"] = "a character class range's first character is greater than its last", // regexp.class.range-out-of-order, written 2026-11-09 (D11)
                ["JSE1901"] = "the source given to `eval` is not a valid script", // eval.source.invalid, written 2026-11-09 (D11)
                ["JSE1902"] = "`{0}` names how a function was called and no function encloses this one", // meta.new-target.outside-function, observed
                ["JSE1903"] = "`super` is only admitted inside a method, a class field initialiser or a class static block", // super.property.outside-method, written 2026-11-09 (D11)
                ["JSE1904"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE1905"] = "`arguments` names nothing inside a class field initialiser", // class.initializer.arguments, d82b07ad's JsCompiler.Eval.cs:309
                ["JSE1906"] = "a `var` in evaluated code names a binding the global scope declares lexically", // eval.var.global-lexical-collision, written 2026-11-09 (D11)
                ["JSE1907"] = "a `var` in evaluated code names a binding an enclosing scope declares lexically", // eval.var.lexical-collision, written 2026-11-09 (D11)
                ["JSE1908"] = "a `var` in evaluated code names a binding an enclosing scope declares lexically", // eval.var.lexical-collision, written 2026-11-09 (D11)
                ["JSE1909"] = "`{0}` is not declared by any class this expression is inside of", // class.private-name.undeclared, d82b07ad's JsCompiler.Classes.cs:420
                ["JSE2001"] = "the parameters given to a function constructor are not a valid parameter list", // dynamic-function.parameters.invalid, written 2026-11-09 (D11)
                ["JSE2002"] = "the body given to a function constructor is not a valid function body", // dynamic-function.body.invalid, written 2026-11-09 (D11)
                ["JSE2003"] = "the source a function constructor assembles is not a valid function", // dynamic-function.source.invalid, written 2026-11-09 (D11)
                ["JSE8106"] = "an HTML-like comment is not admitted in a module", // annexb.html-comment.module, written 2026-11-09 (D11)
                ["JSE8107"] = "Unicode mode does not admit the pattern grammar of Annex B", // annexb.regexp.unicode-mode, written 2026-11-09 (D11)
                ["JSE8301"] = "a function declaration cannot be labelled in strict code", // strict.labelled-function, written 2026-11-09 (D11)
                ["JSE8302"] = "`{0}` is declared twice in one scope", // scope.lexical.duplicate, d82b07ad's JsParser.cs:3745
                ["JSE8303"] = "a function declaration cannot be the body of an `if` statement in strict code", // annexb.if-function.strict, written 2026-11-09 (D11)
                ["JSE8304"] = "`{0}` is bound by a destructuring `catch` parameter, so the handler cannot declare it with `var`", // scope.catch.var-collision, d82b07ad's JsParser.cs:3935
                ["JSE8305"] = "a `for … in` head cannot initialise its binding in strict code", // annexb.for-in-initializer.strict, written 2026-11-09 (D11)
                ["JSE8306"] = "a call is not the target of a `for … in` or `for … of` head in strict code", // assignment.target.invalid, written 2026-11-09 (D11)
                ["JSE7101"] = "`{0}` is not a name a `using` declaration may bind", // declaration.let.bound-name, written 2026-11-09 (D11)
                ["JSE7102"] = "`{0}` is declared twice by one declaration", // scope.lexical.duplicate, d82b07ad's JsParser.cs:3998
                ["JSE7103"] = "the `await using` head of this declaration is not well formed", // cover.await-using-declaration-head, written 2026-11-09 (D11)
                ["JSE7104"] = "`{0}` is not a name an `await using` declaration may bind", // declaration.let.bound-name, written 2026-11-09 (D11)
                ["JSE7105"] = "`{0}` is declared twice by one declaration", // scope.lexical.duplicate, d82b07ad's JsParser.cs:3998
                ["JSE7106"] = "a `{0}` declaration needs a block, a function body, a module or a loop head to be disposed at, and a script's or an eval's top level or a case clause is none", // using.declaration.no-disposal-scope, curated
                ["JSE7107"] = "a `{0}` declaration needs a block, a function body, a module or a loop head to be disposed at, and a script's or an eval's top level or a case clause is none", // using.declaration.no-disposal-scope, curated
                ["JSE7108"] = "a `{0}` declaration needs a block, a function body, a module or a loop head to be disposed at, and a script's or an eval's top level or a case clause is none", // using.declaration.no-disposal-scope, curated
                ["JSE7201"] = "a decorator is not admitted by the declared feature manifest", // decorator.not-in-edition, curated
                ["JSE7202"] = "an `accessor` class element is not admitted by the declared feature manifest", // decorator.auto-accessor.not-in-edition, written 2026-11-09 (D11)
                ["JSE7301"] = "`import source` is not admitted by the declared feature manifest", // import-phase.source.not-in-edition, curated
                ["JSE7302"] = "`import.source` is not admitted by the declared feature manifest", // import-phase.source-call.not-in-edition, written 2026-11-09 (D11)
                ["JSE7303"] = "`import defer` is not admitted by the declared feature manifest", // import-phase.defer.not-in-edition, curated
                ["JSE7304"] = "`import.defer` is not admitted by the declared feature manifest", // import-phase.defer-call.not-in-edition, written 2026-11-09 (D11)
                ["JSE7401"] = "the source given to `eval` is not a valid script", // eval.source.invalid, written 2026-11-09 (D11)
                ["JSE7402"] = "`new.target` names how a function was called and no function encloses this one", // meta.new-target.outside-function, d82b07ad's JsParser.cs:5619
                ["JSE7403"] = "`super` is only admitted inside a method", // super.property.outside-method, d82b07ad's JsCompiler.Calls.cs:535
                ["JSE7404"] = "a `super` call is only admitted in the constructor of a class with a heritage", // super.call.outside-derived-constructor, d82b07ad's JsCompiler.Calls.cs:609
                ["JSE9101"] = "the character {cp} begins no token this grammar defines", // syntax.character.unexpected, curated
                ["JSE9102"] = "a block comment reaches the end of the source without closing", // syntax.comment.unterminated, observed
                ["JSE9103"] = "an invalid numeric literal", // syntax.numeric-literal.invalid, written 2026-11-09 (D11)
                ["JSE9104"] = "a string literal reaches the end of the line without closing", // syntax.string.unterminated, observed
                ["JSE9105"] = "a string literal holds a `\\x` or `\\u` escape that is not well formed", // syntax.string.escape.invalid, written 2026-11-09 (D11)
                ["JSE9106"] = "a regular-expression literal reaches the end of the line without closing", // syntax.regexp.unterminated, observed
                ["JSE9107"] = "a template literal reaches the end of the source without closing", // syntax.template.unterminated, observed
                ["JSE9108"] = "no production admits `{tok}` where it stands", // syntax.token.unexpected, written 2026-11-09 (D11)
                ["JSE9109"] = "`{0}` was expected and `{found}` was found", // syntax.token.missing, curated
                ["JSE9110"] = "`{0}` is a module declaration and this source was presented as a script", // syntax.module-item.in-script, observed
                ["JSE9201"] = "the source nests deeper than the {0} levels these parse options allow", // limit.nesting.too-deep, curated
                ["JSE9202"] = "the source builds a tree deeper than the {0} levels this compiler walks", // limit.tree.too-deep, curated
                ["JSE9203"] = "evaluated source nests deeper than the scope-depth ceiling", // limit.eval-scope.too-deep, d82b07ad's JsCompiler.Eval.cs:282
                ["JSE9204"] = "a function declares more locals than the artifact format admits", // limit.locals.too-many, written 2026-11-09 (D11)
                ["JSE9205"] = "the constant pool is full: an artifact of this format holds at most 65,535 distinct constants - numbers, strings, BigInts and names together - and this one needs more", // limit.constants.too-many, d82b07ad's JsCompiler.Emission.cs:199
                ["JSE9206"] = "the program needs a deeper operand stack than the artifact format admits", // limit.operand-stack.too-deep, written 2026-11-09 (D11)
                ["JSE9301"] = "{0} is not admitted by the declared feature manifest", // engine.manifest.construct-not-admitted, curated
                ["JSE9302"] = "a `const` binding is assigned to", // engine.constant.assigned, written 2026-11-09 (D11)
                ["JSE9303"] = "a name has no binding and no global object to resolve it on", // engine.name.unresolvable, written 2026-11-09 (D11)
                ["JSE9304"] = "two modules were presented under the key `{0}`", // engine.module.duplicate-key, d82b07ad's JsCompiler.Modules.cs:62
                ["JSE9305"] = "no composition of this profile can honour the import attribute `{0}`, so the module this clause decorates cannot be loaded", // engine.import-attribute.unsupported, d82b07ad's JsCompiler.Modules.cs:233
                ["JSE9306"] = "a JSON module's source text is not JSON", // engine.json-module.invalid, written 2026-11-09 (D11)
                ["JSE9307"] = "`{found}` is only admitted inside an async function or an async generator", // engine.top-level-await.disabled, curated
                ["JSE9308"] = "the compiler has no translation for this construct", // engine.construct.not-compiled, written 2026-11-09 (D11)
                ["JSE9111"] = "a unicode escape in an identifier that names no code point", // syntax.identifier.escape.invalid, observed
                ["JSE9207"] = "a regular-expression pattern nests groups and classes deeper than this engine's bound", // limit.regexp.too-deep, written 2026-11-09 (D11)
                ["JSE9208"] = "a JSON module nests deeper than this engine's bound", // limit.json-module.too-deep, written 2026-11-09 (D11)
                ["JSE9209"] = "a BigInt literal wider than the format is not admitted by the declared feature manifest", // limit.bigint-literal.too-wide, curated
                ["JSE9309"] = "a module specifier holds a lone surrogate, which is not well-formed Unicode", // engine.module-specifier.ill-formed, curated
                ["JSE9310"] = "this build cannot serve the compile request", // engine.request.unsupported, written 2026-11-09 (D11)
                ["JSE9311"] = "`{0}` is not declared by any class this expression is inside of", // engine.dynamic-function.private-name, d82b07ad's JsCompiler.Classes.cs:420
                ["JSE9112"] = "`return` has nothing to return from outside a function", // syntax.return.outside-function, d82b07ad's JsCompiler.Exceptions.cs:261
                ["JSE9113"] = "`return` has nothing to return from inside a class static block", // syntax.return.in-static-block, d82b07ad's JsCompiler.Exceptions.cs:275
            },
            System.StringComparer.Ordinal);
}
