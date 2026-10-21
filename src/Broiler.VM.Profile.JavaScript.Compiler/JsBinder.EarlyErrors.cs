// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   20
// Annotated:        20/20
// Exempt:           1
// Human-reviewed:   0/20
// IP risk:          None
// Security risk:    High
// Criteria:         5/3
// Resource impact:  3/10 max
// Unverified:       20
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The binder's early errors: the language early errors the wide lowering used to raise while it
/// emitted, found once per unit, before any of it is lowered.
/// </summary>
/// <remarks>
/// <para>
/// <b>ONE STAGE</b> (SJL-12.6 item 2, JSD-0080 decisions D10 and D12). After the parse, the wide path
/// has one early-error stage, and this is it: a unit it refuses is not lowered, so an early error
/// never produces bytes, and none is reported once per emission - a <c>finally</c> block the lowering
/// emits three times is walked here once. The parser keeps its own checks until SJL-13 deletes it.
/// </para>
/// <para>
/// <b>EVERY CODE AND EVERY MESSAGE IS THE LOWERING'S, AT THE PLACE IT GAVE.</b> No registry code moves
/// (decision D13): a refusal the catalogue records as a split candidate keeps the number it has. What
/// the lowering decided from its own flags - whether code is a method's, a derived constructor's, a
/// function's; which private names and which <c>arguments</c> are in scope; which statement a jump
/// leaves - is decided here from the same reading of the same position.
/// </para>
/// <para>
/// <b>WHAT STAYS WITH THE LOWERING</b> is what is not a language early error: the format's ceilings,
/// the manifest's and the form's refusals, the module-load policy and the evaluated program's
/// scope-depth ceiling. <b>The three language rules the lowering refused under the manifest's code are
/// the binder's</b> - <c>await</c> outside an async function, a logical assignment to a target that is
/// neither a name nor a property, a <c>using</c> declaration outside a statement list that can dispose
/// it - and so is <c>return</c> in a static block, each under the code the catalogue records for it,
/// 2104 and 2207 (JSD-0080 decisions D12 and D13).
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=BE330C
// Broiler-Falsified-If: the binder refuses a unit the lowering lowered without a language early error, or admits one the lowering refused with one, or reports a code, a message or a place other than the lowering's
// Broiler-Human:        PENDING
internal sealed partial class JsBinder
{
    /// <summary>
    /// The top-level function declarations of a sloppy evaluated program that a later one of the same
    /// name overrides, which the lowering never compiles.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=DDC92E
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.HashSet<JsFunctionNode> overridden =
        new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

    /// <summary>Whether this unit is eval code whose caller is a class field initialiser.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D950F0
    // Broiler-Human:        PENDING
    private bool EvaluatedInField =>
        unit.IsEval && (unit.EvalFlags & Format.JsFormat.EvalRequestFlags.InClassFieldInitializer) != 0;

    /// <summary>Records one early error, unless the code it is in is code the lowering never lowers.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=5FDBF9
    // Broiler-Human:        PENDING
    private void Report(Context context, SliceSourceSpan span, SliceSourceDiagnosticCode code, string message)
    {
        if (!context.Is(Frame.Silent))
        {
            found.Add(new SliceSourceDiagnostic(code, message, span.Line, span.Column));
        }
    }

    /// <summary>The early errors found, in source order, at most <see cref="SliceSourceDiagnostic.MaximumPerStage"/> of them.</summary>
    /// <remarks>
    /// <para>
    /// <b>Source order is the order of the places reported</b>, line and then column, and two at one
    /// place keep the order the walk met them in. The walk meets nodes in source order, but a few
    /// errors are reported at a place other than the node's own - a field initialiser's at the
    /// field, a module's import collision at no place at all, which is line 0 - so the order is made
    /// rather than assumed.
    /// </para>
    /// <para>
    /// <b>The cap is applied after the sort, and that is a known limit</b> of reporting in source
    /// order within it. The lowering kept the first 64 it met, in its own order - a hoisted function
    /// first - so a unit with more than 64 early errors can keep a different set of codes here than
    /// it did there: 64 <c>break;</c> followed by <c>function f() { return super.x; }</c> was
    /// [2101, 2207 x 63] and is [2207 x 64]. Class S1 of the bundle's rule admits a reorder of the
    /// same codes only. No input of the parse corpus reaches it: no unit the lowering refused had 64
    /// diagnostics of the lowering's at the base.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=1D5458
    // Broiler-Falsified-If: two early errors are answered in an order other than their places', or more than the cap is answered
    // Broiler-Human:        PENDING
    private static SliceSourceDiagnostic[] InSourceOrder(System.Collections.Generic.List<SliceSourceDiagnostic> errors)
    {
        if (errors.Count == 0)
        {
            return [];
        }

        var order = new int[errors.Count];

        for (var at = 0; at < order.Length; at++)
        {
            order[at] = at;
        }

        System.Array.Sort(order, (left, right) =>
        {
            var compared = errors[left].Line.CompareTo(errors[right].Line);

            if (compared == 0)
            {
                compared = errors[left].Column.CompareTo(errors[right].Column);
            }

            return compared != 0 ? compared : left.CompareTo(right);
        });

        var kept = new SliceSourceDiagnostic[System.Math.Min(order.Length, SliceSourceDiagnostic.MaximumPerStage)];

        for (var at = 0; at < kept.Length; at++)
        {
            kept[at] = errors[order[at]];
        }

        return kept;
    }

    /// <summary>What a class member's body is to the lowering: a method, a constructor or a static block.</summary>
    /// <remarks>
    /// Every member is compiled as a method - a field's initialiser and a static block's body included
    /// - and the constructor the class body wrote is a derived one when the class has a heritage.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=53F3D4
    // Broiler-Human:        PENDING
    private static Use MemberUse(
        JsClassMember member, JsFunctionNode body, JsFunctionNode? constructor, bool derived)
    {
        if (ReferenceEquals(body, constructor))
        {
            return derived ? Use.Derived : Use.Method;
        }

        return member.Kind == JsMethodKind.StaticBlock ? Use.StaticBlock : Use.Method;
    }

    /// <summary>
    /// Refuses an <c>arguments</c> anywhere in a field initialiser's or a static block's own code.
    /// </summary>
    /// <remarks>
    /// <b>The search is the lowering's own</b> (<see cref="JsCompiler.Walk.Mentions"/>): an arrow's body
    /// is the field's code and an ordinary function's is not, and an arrow's parameter list is not
    /// searched, which is what the lowering asked. One refusal per member, at its body.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=7C49CF
    // Broiler-Human:        PENDING
    private void RefuseArgumentsIn(JsClassMember member, Context context)
    {
        if (member.Function is not { } body ||
            member.Kind is not (JsMethodKind.StaticBlock or JsMethodKind.Field))
        {
            return;
        }

        foreach (var statement in body.Body)
        {
            if (JsCompiler.Walk.Mentions(statement, "arguments"))
            {
                Report(
                    context,
                    body.Span,
                    SliceSourceDiagnosticCode.UnresolvableIdentifier,
                    "`arguments` names nothing inside a class field initialiser or a static block");

                return;
            }
        }
    }

    /// <summary>
    /// Refuses a private name no class around the reference declares, unless eval code's caller may.
    /// </summary>
    /// <remarks>
    /// <b>The innermost class that declares the spelling is the one it names</b>, so the search goes
    /// outward through the class bodies the walk is inside of. Eval code whose caller is inside a class
    /// reads a name it does not declare through its caller, and the executor refuses one no class there
    /// declares before anything runs (JSeal V15-finish).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=60075F
    // Broiler-Falsified-If: a private name a class around the reference declares is refused, or one none declares is admitted outside eval code whose caller is in a class
    // Broiler-Human:        PENDING
    private void ResolvePrivate(Context context, SliceSourceSpan span, string name)
    {
        for (var scope = context.Privates; scope is not null; scope = scope.Outer)
        {
            if (System.Array.IndexOf(scope.Names, name) >= 0)
            {
                return;
            }
        }

        if (unit.IsEval && (unit.EvalFlags & Format.JsFormat.EvalRequestFlags.InClassBody) != 0)
        {
            return;
        }

        Report(
            context,
            span,
            SliceSourceDiagnosticCode.UnresolvableIdentifier,
            "`" + name + "` is not declared by any class this expression is inside of");
    }

    /// <summary>
    /// Records a reference to <paramref name="name"/>, which in eval code inside a field initialiser is
    /// an early error when it is <c>arguments</c> and nothing in the program binds it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The language makes it one error of the whole evaluation</b> (<c>PerformEval</c>'s
    /// <c>ContainsArguments</c>), and the lowering reported it at no place, once for each eval name
    /// instruction it emitted for such a name. <b>It is reported here once for each reference</b>, at
    /// the same place: distinct references are distinct reports, as they were, and a reference the
    /// lowering emitted twice - one it read and wrote, as <c>arguments += 1</c> and
    /// <c>arguments++</c> do, or one in a <c>finally</c> block it lowered on more than one path - is
    /// one report rather than two (class S2 of the bundle's rule).
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=8DE4DF
    // Broiler-Human:        PENDING
    private void Referenced(Context context, string name)
    {
        if (!EvaluatedInField || context.Is(Frame.ArgumentsBound) || context.Is(Frame.Silent) ||
            !IsArguments(name))
        {
            return;
        }

        Report(
            context,
            default,
            SliceSourceDiagnosticCode.UnresolvableIdentifier,
            "`arguments` names nothing inside a class field initialiser");
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=6476B5
    // Broiler-Human:        PENDING
    private static bool IsArguments(string name) =>
        string.Equals(name, "arguments", System.StringComparison.Ordinal);

    /// <summary>This context inside a record that binds <c>arguments</c> when <paramref name="bindsArguments"/> says so.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=5CB3EB
    // Broiler-Human:        PENDING
    private Context InsideRecord(Context context, bool bindsArguments) =>
        EvaluatedInField && bindsArguments ? context.And(Frame.ArgumentsBound) : context;

    /// <summary>This context where a record holding <paramref name="names"/> is entered.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=8DDEF7
    // Broiler-Human:        PENDING
    private Context InsideRecord(Context context, System.Collections.Generic.IReadOnlyList<string> names)
    {
        if (!EvaluatedInField)
        {
            return context;
        }

        foreach (var name in names)
        {
            if (IsArguments(name))
            {
                return context.And(Frame.ArgumentsBound);
            }
        }

        return context;
    }

    /// <summary>This context where a statement list's record, holding <paramref name="lexical"/>, is entered.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D10DA4
    // Broiler-Human:        PENDING
    private Context InsideRecord(Context context, System.Collections.Generic.IReadOnlyList<Declaration> lexical)
    {
        if (!EvaluatedInField)
        {
            return context;
        }

        foreach (var declaration in lexical)
        {
            if (IsArguments(declaration.Name))
            {
                return context.And(Frame.ArgumentsBound);
            }
        }

        return context;
    }

    /// <summary>Whether an evaluated program's own record declares <c>arguments</c>.</summary>
    /// <remarks>
    /// Its <c>let</c>, <c>const</c> and <c>class</c> names always, and in strict code its <c>var</c>s
    /// and functions; a sloppy program's are its caller's.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=C6A309
    // Broiler-Human:        PENDING
    private bool EvalRootDeclaresArguments(bool strict)
    {
        foreach (var declaration in Program.TopLevel.Lexical)
        {
            if (!declaration.IsFunction && IsArguments(declaration.Name))
            {
                return true;
            }
        }

        if (!strict)
        {
            return false;
        }

        foreach (var name in Program.VarNames)
        {
            if (IsArguments(name))
            {
                return true;
            }
        }

        foreach (var function in Program.Functions)
        {
            if (IsArguments(function.Name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Whether a parameter list binds the name <c>arguments</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=323AFD
    // Broiler-Human:        PENDING
    private static bool ParametersBindArguments(JsFunctionNode function)
    {
        var names = new System.Collections.Generic.List<string>();

        foreach (var parameter in function.Parameters)
        {
            PatternNames(parameter.Target, names);
        }

        foreach (var name in names)
        {
            if (IsArguments(name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Whether a function's body declares <c>arguments</c> in its own record.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=B19B66
    // Broiler-Human:        PENDING
    private static bool BodyDeclaresArguments(FunctionFacts facts)
    {
        foreach (var name in facts.Body.VarNames)
        {
            if (IsArguments(name))
            {
                return true;
            }
        }

        foreach (var declaration in facts.Body.TopLevel.Lexical)
        {
            if (IsArguments(declaration.Name))
            {
                return true;
            }
        }

        foreach (var function in facts.Body.Functions)
        {
            if (IsArguments(function.Name))
            {
                return true;
            }
        }

        foreach (var alias in facts.Body.AnnexBAliases)
        {
            if (IsArguments(alias))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Finds the top-level function declarations of a sloppy evaluated program that a later declaration
    /// of the same name overrides: the lowering instantiates the last of each name and no other.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=239FAA
    // Broiler-Human:        PENDING
    private void FindOverriddenFunctions()
    {
        var seen = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        for (var at = Program.Functions.Count - 1; at >= 0; at--)
        {
            if (!seen.Add(Program.Functions[at].Name))
            {
                overridden.Add(Program.Functions[at]);
            }
        }
    }

    /// <summary>Whether the lowering can write through <paramref name="target"/>.</summary>
    /// <remarks>
    /// A name, a property - dotted, computed, private or <c>super</c>'s - and, in sloppy code only, a
    /// call, which Annex B lets stand as a target whose write throws when it runs.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=ACD45C
    // Broiler-Human:        PENDING
    private static bool IsReference(JsExpression target, bool strict) =>
        target is JsIdentifier or JsMemberExpression or JsPrivateMemberExpression or JsSuperMemberExpression ||
        (target is JsCallExpression && !strict);

    /// <summary>Refuses a regular-expression literal whose flags or pattern the language refuses.</summary>
    /// <remarks>
    /// <para>
    /// <b>The flags first</b> (ES2026 IsValidRegularExpressionLiteral): drawn from <c>dgimsuyv</c>,
    /// none twice, and not <c>u</c> with <c>v</c>. <b>Then the pattern, by the matcher's own
    /// grammar</b>, which the format assembly holds so that this front end and the executor cannot
    /// disagree about what a pattern is; its message is the refusal's.
    /// </para>
    /// <para>
    /// A literal the lowering never lowers is not compiled at all, here either.
    /// </para>
    /// <para>
    /// <b>The flags were not always an early error</b>: <c>/a/x</c> and <c>/a/gg</c> behind a
    /// <c>false</c> branch were accepted, and <c>/a/v</c> - a flag the language has and the matcher
    /// then did not - passed the front end and was refused only when the literal ran, as a
    /// SyntaxError from the constructor. A flag outside <c>dgimsuyv</c>, a repeated flag, and
    /// <c>u</c> with <c>v</c> became the language's SyntaxError in the lowering, and a well-formed
    /// <c>v</c> an early SyntaxError naming the flag as unsupported (JSeal slice JSD-0031-later).
    /// <b>A well-formed <c>v</c> is admitted</b> since phase F2 gave the matcher the flag's class
    /// syntax, set operations and properties of strings; until then it was refused as unsupported,
    /// under the reading of decision JSD-0031 section 12 (JSC-262). The checks moved here from the
    /// lowering's <c>RefuseRegExpLiteralFlags</c> and <c>CompileRegExpLiteral</c> with SJL-12.6
    /// item 2, unchanged.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=7B69FD
    // Broiler-Falsified-If: a literal the matcher compiles is refused, or one it refuses, or whose flags the language refuses, is admitted
    // Broiler-Human:        PENDING
    private void CheckRegExp(Context context, JsRegExpLiteral pattern)
    {
        if (context.Is(Frame.Silent))
        {
            return;
        }

        var flags = pattern.Flags;

        for (var at = 0; at < flags.Length; at++)
        {
            if ("dgimsuyv".IndexOf(flags[at], System.StringComparison.Ordinal) < 0 ||
                flags.IndexOf(flags[at], at + 1) >= 0)
            {
                Report(
                    context,
                    pattern.Span,
                    SliceSourceDiagnosticCode.UnexpectedToken,
                    "Invalid regular expression flags: " + flags);

                return;
            }
        }

        if (flags.Contains('v', System.StringComparison.Ordinal) &&
            flags.Contains('u', System.StringComparison.Ordinal))
        {
            Report(
                context,
                pattern.Span,
                SliceSourceDiagnosticCode.UnexpectedToken,
                "Invalid regular expression flags: " + flags + " (u and v may not be combined)");

            return;
        }

        try
        {
            _ = Format.JsRegExpMatcher.Compile(
                pattern.Pattern,
                flags.Contains('i', System.StringComparison.Ordinal),
                flags.Contains('m', System.StringComparison.Ordinal),
                flags.Contains('s', System.StringComparison.Ordinal),
                flags.Contains('u', System.StringComparison.Ordinal),
                flags.Contains('v', System.StringComparison.Ordinal));
        }
        catch (Format.JsRegExpSyntaxError failure)
        {
            Report(context, pattern.Span, SliceSourceDiagnosticCode.UnexpectedToken, failure.Message);
        }
    }

    /// <summary>
    /// A module's declaration checks: its imports, its top-level declarations against each other and
    /// against its imports, and its exports.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The lowering's three passes, in its order and with its answers</b>: an import binding
    /// <c>arguments</c> or <c>eval</c>, or a name imported twice; a function declared twice, a function or
    /// a lexical name that is also a <c>var</c>, a lexical name declared twice, and a name both declared
    /// and imported, which the lowering reported at no place, once for each declaration of it; an export
    /// name published twice, and a local export no declaration and no import binds.
    /// </para>
    /// <para>
    /// <b>The import attribute is not one of them</b>: an attribute no composition can honour is the
    /// module-load policy, which stays with the lowering (decision D12).
    /// </para>
    /// <para>
    /// <b>Their history moved with them</b> from the lowering's <c>DeclareImports</c>,
    /// <c>DeclareModuleBindings</c>, <c>Declared</c> and <c>DeclareExports</c> (SJL-12.6 item 2).
    /// An import binds a name in strict code, where <c>arguments</c> and <c>eval</c> are not binding
    /// names (13.1.1); until 2026-10-03 both imported (JSC-253). A second function declaration of one
    /// name is an error in a module and not in a script, because a script's top-level declarations
    /// are properties of the global object and a module's are lexical (16.2.1.1); for the same
    /// reason a module's function collides with a <c>var</c> of its name, and until 2026-10-03
    /// <c>var f; function f() {}</c> compiled in a module (JSC-253). A destructuring declarator has no
    /// name of its own and declares every name in its pattern: reading the declarator's name gave the
    /// empty string for each one, so two top-level <c>const {a} = o; const {b} = o;</c> were refused
    /// as the same undeclared name twice, and a pattern's real names were never checked (VM-FIX-D) -
    /// which is why <see cref="ModuleDeclarations"/> lists every name a pattern binds.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=2; Fingerprint=B0189E
    // Broiler-Falsified-If: a module the lowering's declaration checks admitted is refused here, or one they refused is admitted, or a refusal's code, message or place differs from theirs
    // Broiler-Human:        PENDING
    private void CheckModule(System.Collections.Generic.IReadOnlyList<JsStatement> body)
    {
        var context = new Context(true, null, Frame.None, null);
        var imports = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        foreach (var statement in body)
        {
            if (statement is not JsImportDeclaration import)
            {
                continue;
            }

            foreach (var specifier in import.Specifiers)
            {
                if (specifier.Local is "arguments" or "eval")
                {
                    Report(
                        context,
                        specifier.Span,
                        SliceSourceDiagnosticCode.ReservedWordAsBinding,
                        "`" + specifier.Local + "` is not a binding name in a module");

                    continue;
                }

                if (!imports.Add(specifier.Local))
                {
                    Report(
                        context,
                        specifier.Span,
                        SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                        "`" + specifier.Local + "` is imported twice");
                }
            }
        }

        var declared = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);
        var varNames = new System.Collections.Generic.HashSet<string>(Program.VarNames, System.StringComparer.Ordinal);

        foreach (var name in Program.VarNames)
        {
            Declares(context, imports, declared, name);
        }

        var declaredLexically = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        foreach (var function in Program.Functions)
        {
            if (!declaredLexically.Add(function.Name))
            {
                Report(
                    context,
                    function.Span,
                    SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                    "`" + function.Name + "` is declared twice at this module's top level");
            }

            if (varNames.Contains(function.Name))
            {
                Report(
                    context,
                    function.Span,
                    SliceSourceDiagnosticCode.VarAndLexicalCollision,
                    "`" + function.Name + "` is declared both as a `var` and as a function");
            }

            Declares(context, imports, declared, function.Name);
        }

        foreach (var declaration in ModuleDeclarations)
        {
            if (!declaredLexically.Add(declaration.Name))
            {
                Report(
                    context,
                    declaration.Span,
                    SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                    "`" + declaration.Name + "` is declared twice at this module's top level");
            }

            if (!declaration.IsClass && varNames.Contains(declaration.Name))
            {
                Report(
                    context,
                    declaration.Span,
                    SliceSourceDiagnosticCode.VarAndLexicalCollision,
                    "`" + declaration.Name + "` is declared both as a `var` and lexically");
            }

            Declares(context, imports, declared, declaration.Name);
        }

        foreach (var statement in body)
        {
            if (statement is JsExportDeclaration { Kind: JsExportKind.Default, Default: not null })
            {
                declared.Add(JsParser.DefaultBindingName);
            }
        }

        var exported = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        foreach (var statement in body)
        {
            if (statement is not JsExportDeclaration export ||
                (export.Kind == JsExportKind.All && export.Specifiers.Count == 0))
            {
                continue;
            }

            foreach (var specifier in export.Specifiers)
            {
                if (!exported.Add(specifier.Exported))
                {
                    Report(
                        context,
                        specifier.Span,
                        SliceSourceDiagnosticCode.DuplicateExportName,
                        "`" + specifier.Exported + "` is exported twice");

                    continue;
                }

                if (export.From.Length != 0 || imports.Contains(specifier.Local) ||
                    declared.Contains(specifier.Local))
                {
                    continue;
                }

                Report(
                    context,
                    specifier.Span,
                    SliceSourceDiagnosticCode.ExportNameNotDeclared,
                    "`" + specifier.Local + "` is exported and this module declares no such binding");
            }
        }
    }

    /// <summary>
    /// Declares one of a module's own names, unless an import already binds it, which is refused.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=B1C0AA
    // Broiler-Human:        PENDING
    private void Declares(
        Context context,
        System.Collections.Generic.HashSet<string> imports,
        System.Collections.Generic.HashSet<string> declared,
        string name)
    {
        if (name.Length == 0)
        {
            return;
        }

        if (imports.Contains(name))
        {
            Report(
                context,
                default,
                SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                "`" + name + "` is both imported and declared in this module");

            return;
        }

        declared.Add(name);
    }
}
