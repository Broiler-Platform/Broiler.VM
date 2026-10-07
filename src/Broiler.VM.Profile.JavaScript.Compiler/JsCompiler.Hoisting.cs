// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           0
// Human-reviewed:   0/17
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       17
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // ---- hoisting ------------------------------------------------------------------------------

    /// <remarks>
    /// <b>It answers what the body declares, for the script-declarations row</b> (JSeal V15-host): the
    /// lexical names, the <c>var</c> names that are no function's, the function names one per name,
    /// and the Annex B aliases one per name. The instructions below still create every binding; the
    /// row lets the executor run the specification's checks before the first of them.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=56C5E3
    // Broiler-Human:        PENDING
    private (string[] LexicalNames, string[] VarNames, string[] FunctionNames, string[] AnnexBNames) HoistProgram(
        System.Collections.Generic.IReadOnlyList<JsStatement> body)
    {
        var names = new System.Collections.Generic.List<string>();
        var functions = new System.Collections.Generic.List<JsFunctionNode>();
        CollectVarScope(body, names, functions, lexical: null);

        // A FUNCTION DECLARATION CREATES ITS GLOBAL BINDING BEFORE IT IS WRITTEN, and until
        // 2026-09-04 the write was the only step. It worked because a store to a name the global
        // object did not have created it — which is exactly what strict code may not do, so the
        // moment `StoreGlobal` began refusing that *(JSC-93)*, EVERY strict script with a function
        // declaration in it threw a `ReferenceError` about the function it was declaring. The
        // declaration is separate from the write in the specification for this reason: the binding
        // exists before anything assigns to it.
        //
        // AND IT CREATES IT BEFORE THE VAR BINDINGS, NOT AFTER *(corrected: JSC-214)*. The
        // specification's global declaration instantiation creates every function binding and then
        // every var binding, so a script declaring both puts the function on the global object
        // first - and the order own property keys come out in is observable, through
        // `Object.getOwnPropertyNames`, `Object.keys` and `for...in`. This loop ran second until
        // 2026-09-09, which made the order source order for a script whose var came first. A host
        // that recovers a script's declarations by diffing the global's own names across an
        // evaluation - which is how a nested browsing context does it - read them in an order no
        // other engine produces.
        foreach (var function in functions)
        {
            Emit(JsOpcode.DeclareGlobal, InternedName(function.Name));
        }

        foreach (var name in names)
        {
            Emit(JsOpcode.DeclareGlobal, InternedName(name));
        }

        foreach (var function in functions)
        {
            Emit(JsOpcode.Closure, (ushort)CompileFunction(function));
            Emit(JsOpcode.StoreGlobal, InternedName(function.Name));
        }

        // A SCRIPT-LEVEL `let`, `const` OR `class` IS NOT A PROPERTY OF THE GLOBAL OBJECT, and
        // until 2026-09-05 it was one. The three instructions below create bindings of the realm's
        // global LEXICAL environment instead, which is what makes `globalThis.x` not see one, a
        // read before the declaration a `ReferenceError` rather than `undefined`, and an assignment
        // to a script-level `const` a `TypeError` rather than a silent write.
        //
        // THE WHOLE SET IS DECLARED BEFORE THE FIRST STATEMENT RUNS, which is what puts every one
        // of them in its dead zone for exactly the span the language says: from the top of the
        // script to its own initialiser.
        programLexicals.Clear();
        CollectLexicalKinds(body, programLexicals);

        foreach (var pair in programLexicals)
        {
            Emit(
                pair.Value ? JsOpcode.DeclareGlobalConst : JsOpcode.DeclareGlobalLet,
                InternedName(pair.Key));
        }

        // ANNEX B GIVES A BLOCK-LEVEL FUNCTION A GLOBAL BINDING TOO, and it is created here with
        // the rest and left holding `undefined` until the declaration is reached. The property has
        // to exist by now for the same reason a declared function's does: `StoreGlobal` refuses to
        // create one *(JSC-93)*, and the write this binding exists for is written where the
        // declaration stands.
        var aliases = ScriptAliases(body);

        foreach (var alias in aliases)
        {
            Emit(JsOpcode.DeclareGlobal, InternedName(alias));
        }

        var functionNames = new System.Collections.Generic.List<string>();
        var functionSeen = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        foreach (var function in functions)
        {
            if (functionSeen.Add(function.Name))
            {
                functionNames.Add(function.Name);
            }
        }

        var varNames = new System.Collections.Generic.List<string>();
        var varSeen = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        foreach (var name in names)
        {
            if (!functionSeen.Contains(name) && varSeen.Add(name))
            {
                varNames.Add(name);
            }
        }

        var lexicalNames = new string[programLexicals.Count];
        programLexicals.Keys.CopyTo(lexicalNames, 0);

        var annexBNames = new System.Collections.Generic.List<string>();
        var aliasSeen = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        foreach (var alias in aliases)
        {
            if (aliasSeen.Add(alias))
            {
                annexBNames.Add(alias);
            }
        }

        return (lexicalNames, [.. varNames], [.. functionNames], [.. annexBNames]);
    }

    /// <summary>The Annex B aliases one script's top level owes, or nothing where it is strict.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=726B68
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<string> ScriptAliases(
        System.Collections.Generic.IReadOnlyList<JsStatement> body)
    {
        var aliases = new System.Collections.Generic.List<string>();

        if (strict)
        {
            return aliases;
        }

        var blocked = new System.Collections.Generic.HashSet<string>(
            System.StringComparer.Ordinal);

        foreach (var pair in programLexicals)
        {
            blocked.Add(pair.Key);
        }

        ScanAnnexB(body, blocked, aliases);
        return aliases;
    }

    /// <summary>Declares and initialises a function body's hoisted bindings.</summary>
    /// <param name="body">The body's statements.</param>
    /// <param name="parameters">
    /// The parameters' record when the body has a variable environment of its own - the current
    /// scope, one record inside it - or <see langword="null"/> when one record holds both.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=56D9CA
    // Broiler-Human:        PENDING
    private void HoistFunction(System.Collections.Generic.IReadOnlyList<JsStatement> body, Scope? parameters = null)
    {
        // THE PARAMETER NAMES ARE READ OFF THE SCOPE BEFORE ANYTHING ELSE IS PUT IN IT. Annex B
        // refuses to alias a name the parameter list already bound - and `arguments` where the
        // object was created is one of those names - so the set has to be taken while the scope
        // holds the parameters and nothing else.
        var bound = new System.Collections.Generic.HashSet<string>(
            (parameters ?? scope).Names, System.StringComparer.Ordinal);

        var names = new System.Collections.Generic.List<string>();
        var functions = new System.Collections.Generic.List<JsFunctionNode>();
        CollectVarScope(body, names, functions, lexical: null);

        foreach (var name in names)
        {
            if (scope.Has(name))
            {
                continue;
            }

            var slot = scope.Declare(name, constant: false);

            // A BODY `var` OF A PARAMETER'S NAME STARTS WITH THE PARAMETER'S VALUE when the body has
            // a variable environment of its own (FunctionDeclarationInstantiation step 28.f), and is
            // a different binding from then on; `arguments` is one of those names when the object
            // was bound beside the parameters.
            if (parameters is not null && parameters.TryGet(name, out var parameter, out _))
            {
                EmitScoped(JsOpcode.LoadScoped, (byte)Hops(parameters), parameter);
            }
            else
            {
                Emit(JsOpcode.LoadUndefined);
            }

            EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
        }

        // THE BODY'S OWN `let`, `const` AND `class` NAMES ARE DECLARED BEFORE ANY CLOSURE IS BUILT,
        // and left in their dead zone - a slot nothing has initialised is one. A hoisted function
        // is compiled here, above the first statement, so a body that mentioned a `const` of the
        // enclosing function resolved it to a GLOBAL when the slot was created only on reaching the
        // declaration: `function t() { const a = 1; function f() { return a; } return f(); }`
        // threw "a is not defined" (VM-FIX-D). A block already declared its whole set up front for
        // the same reason (JSC-138); the top level of a body is that rule's other half.
        var bodyLexical = new System.Collections.Generic.List<(string Name, bool Constant)>();
        CollectLexical(body, bodyLexical);
        DeclareLexical(bodyLexical);

        // THOSE SLOTS ARE THE BODY'S LEXICAL HALF even though they precede the function names and
        // the Annex B aliases declared below, so they are named to the scope explicitly: a direct
        // eval's conflict walk must see `let a` as lexical wherever its slot fell (JSeal V15).
        foreach (var (name, _) in bodyLexical)
        {
            if (scope.Has(name))
            {
                (scope.LexicalSlots ??= []).Add(scope.SlotOf(name));
            }
        }

        // EVERY HOISTED NAME IS DECLARED BEFORE ANY BODY IS COMPILED. Two sibling function
        // declarations call each other, and a compiler that declared and compiled them one at a
        // time would resolve the first one's reference to the second as a GLOBAL - which is not a
        // compile error, so the program runs and fails at the call with "not defined". The Octane
        // harness's own RunStep is exactly that shape.
        var slots = new System.Collections.Generic.List<int>(functions.Count);

        foreach (var function in functions)
        {
            slots.Add(
                scope.Has(function.Name)
                    ? scope.SlotOf(function.Name)
                    : scope.Declare(function.Name, constant: false));
        }

        for (var index = 0; index < functions.Count; index++)
        {
            Emit(JsOpcode.Closure, (ushort)CompileFunction(functions[index]));
            EmitScoped(JsOpcode.InitialiseScoped, 0, slots[index]);
        }

        if (strict)
        {
            return;
        }

        // ANNEX B'S OWN BINDINGS COME LAST AND HOLD `undefined`. A name the body already declared
        // - as a `var`, as a parameter or as a top-level function - keeps the binding it has and
        // gains nothing here, which is what makes `function f() { } { function f() { } }` ONE
        // binding that the block's declaration then updates rather than two that disagree.
        var blocked = new System.Collections.Generic.HashSet<string>(
            bound, System.StringComparer.Ordinal);

        // `arguments` IS A PARAMETER NAME WHENEVER THE OBJECT EXISTS, and the language says so in
        // those words: function declaration instantiation appends it to `parameterNames` before
        // Annex B's clause reads them. Naming it here rather than asking whether the object was
        // created keeps the answer on the safe side of the rule - the extension is declined, and
        // declining it changes nothing about a program that has no such declaration in it.
        blocked.Add("arguments");

        var lexical = new System.Collections.Generic.Dictionary<string, bool>(
            System.StringComparer.Ordinal);

        CollectLexicalKinds(body, lexical);

        foreach (var pair in lexical)
        {
            blocked.Add(pair.Key);
        }

        var aliases = new System.Collections.Generic.List<string>();
        ScanAnnexB(body, blocked, aliases);

        foreach (var alias in aliases)
        {
            if (scope.Has(alias))
            {
                continue;
            }

            var slot = scope.Declare(alias, constant: false);
            Emit(JsOpcode.LoadUndefined);
            EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
        }
    }

    /// <summary>
    /// Collects the <c>var</c> names and function declarations of one hoisting scope.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It descends through blocks, loops and <c>try</c> - a <c>var</c> anywhere inside a function
    /// belongs to that function - and stops at a nested function, which is a hoisting scope of its
    /// own. A collector that stopped at a block would answer that <c>if (x) { var y; }</c> declares
    /// nothing.
    /// </para>
    /// <para>
    /// <b>A FUNCTION DECLARATION IS COLLECTED AT THE TOP LEVEL OF THE SCOPE AND NOWHERE BELOW IT,
    /// which is the one respect in which it is unlike a <c>var</c>.</b> The two look alike from
    /// here — both are hoisted, both are initialised before the first statement runs — and the
    /// language separates them at exactly this point: a <c>var</c> anywhere in the body binds in
    /// the body's own scope, while a function declaration written inside a block binds in THAT
    /// BLOCK. Collecting it here anyway is what made
    /// <c>{ let n = 0; function fn() { n += 1; } fn(); }</c> a <c>ReferenceError</c>, because the
    /// closure was then built against a scope chain the block's record was not yet on
    /// <i>(JSC-138)</i>. Below the top level <paramref name="functions"/> is passed as
    /// <see langword="null"/> and <see cref="CompileBlock"/> owns the declaration instead.
    /// </para>
    /// <para>
    /// <b>A LABEL IS TRANSPARENT TO THAT RULE AND A BLOCK IS NOT.</b> <c>l: function f() { }</c> at
    /// the top level of a body declares <c>f</c> in the body, because the language's own
    /// <c>TopLevelVarScopedDeclarations</c> looks through a labelled statement to the declaration
    /// under it; <c>{ l: function f() { } }</c> declares it in the block. So the labelled case
    /// forwards <paramref name="functions"/> unchanged - which at the top level is the list and
    /// below it is already <see langword="null"/> - and every other descent drops it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8575D9
    // Broiler-Human:        PENDING
    private static void CollectVarScope(
        System.Collections.Generic.IReadOnlyList<JsStatement> body,
        System.Collections.Generic.List<string> names,
        System.Collections.Generic.List<JsFunctionNode>? functions,
        System.Collections.Generic.List<string>? lexical)
    {
        foreach (var statement in body)
        {
            switch (statement)
            {
                case JsVariableStatement variable when variable.Kind == SliceDeclarationKind.Var:
                    foreach (var declarator in variable.Declarators)
                    {
                        CollectDeclaratorNames(declarator, names);
                    }

                    break;

                case JsVariableStatement variable when lexical is not null:
                    foreach (var declarator in variable.Declarators)
                    {
                        CollectDeclaratorNames(declarator, lexical);
                    }

                    break;

                case JsFunctionDeclaration declaration:
                    functions?.Add(declaration.Function);
                    break;

                case JsBlockStatement block:
                    CollectVarScope(block.Body, names, null, null);
                    break;

                // A `with` IS NOT A HOISTING SCOPE. `with (o) { var x; function f() { } }` declares
                // both in the enclosing function, which is what makes `x` survive the body and what
                // makes `f` callable after it - and it is also why an assignment to `x` INSIDE the
                // body still asks the object first: the binding is the function's and the write is
                // resolved dynamically.
                case JsWithStatement scoped:
                    CollectVarScope([scoped.Body], names, null, null);
                    break;

                case JsIfStatement conditional:
                    CollectVarScope([conditional.Consequent], names, null, null);

                    if (conditional.Alternate is not null)
                    {
                        CollectVarScope([conditional.Alternate], names, null, null);
                    }

                    break;

                case JsWhileStatement loop:
                    CollectVarScope([loop.Body], names, null, null);
                    break;

                case JsDoWhileStatement loop:
                    CollectVarScope([loop.Body], names, null, null);
                    break;

                case JsForStatement loop:
                    if (loop.Initialiser is not null)
                    {
                        CollectVarScope([loop.Initialiser], names, null, null);
                    }

                    CollectVarScope([loop.Body], names, null, null);
                    break;

                case JsForInStatement loop:
                    if (loop.Declaration == SliceDeclarationKind.Var)
                    {
                        CollectHeadNames(loop.Name, loop.Pattern, names);
                    }

                    CollectVarScope([loop.Body], names, null, null);
                    break;

                case JsForOfStatement loop:
                    if (loop.Declaration == SliceDeclarationKind.Var)
                    {
                        CollectHeadNames(loop.Name, loop.Pattern, names);
                    }

                    CollectVarScope([loop.Body], names, null, null);
                    break;

                case JsTryStatement guarded:
                    CollectVarScope(guarded.Block.Body, names, null, null);

                    if (guarded.Handler is not null)
                    {
                        CollectVarScope(guarded.Handler.Body, names, null, null);
                    }

                    if (guarded.Finaliser is not null)
                    {
                        CollectVarScope(guarded.Finaliser.Body, names, null, null);
                    }

                    break;

                case JsSwitchStatement switched:
                    foreach (var clause in switched.Clauses)
                    {
                        CollectVarScope(clause.Body, names, null, null);
                    }

                    break;

                case JsLabelledStatement labelled:
                    CollectVarScope([labelled.Body], names, functions, null);
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Every name a script's top level declares lexically, and whether the declaration is immutable.
    /// </summary>
    /// <remarks>
    /// The same walk <see cref="CollectLexical"/> makes, keeping the one fact that walk discards:
    /// a <c>const</c> and a <c>let</c> are the same KIND of binding and only one of them admits an
    /// assignment, and at script level nothing downstream can recover which it was from a name.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B2A71D
    // Broiler-Human:        PENDING
    private static void CollectLexicalKinds(
        System.Collections.Generic.IReadOnlyList<JsStatement> body,
        System.Collections.Generic.Dictionary<string, bool> kinds)
    {
        foreach (var statement in body)
        {
            if (statement is JsVariableStatement variable && variable.Kind != SliceDeclarationKind.Var)
            {
                var names = new System.Collections.Generic.List<string>();

                foreach (var declarator in variable.Declarators)
                {
                    CollectDeclaratorNames(declarator, names);
                }

                foreach (var name in names)
                {
                    kinds[name] = variable.Kind == SliceDeclarationKind.Const;
                }
            }

            // A CLASS BINDING IS MUTABLE, which reads like a detail and is the reason `class C {}`
            // followed by `C = 1` is a program: the binding a class declaration makes is a `let`
            // and not a `const`, and only the binding inside the class's own body is immutable.
            if (statement is JsClassDeclaration declaration && declaration.Class.Name.Length != 0)
            {
                kinds[declaration.Class.Name] = false;
            }
        }
    }

    /// <summary>Every name one statement list declares lexically, and whether it is immutable.</summary>
    /// <remarks>
    /// <b>The whole set is collected so the whole set can be DECLARED before the list runs.</b>
    /// A block's record holds every lexical name the block declares from the moment the record
    /// exists — that is what the temporal dead zone is — and a lowering that created each slot when
    /// it reached the declaration had two consequences it did not want: a mention above the
    /// declaration resolved to whatever the enclosing scopes had rather than throwing, and a
    /// closure built at block entry, which is where a function declaration's closure has to be
    /// built, resolved the block's own <c>let</c> to a global because the slot did not exist yet.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E1ED8F
    // Broiler-Human:        PENDING
    private static void CollectLexical(
        System.Collections.Generic.IReadOnlyList<JsStatement> body,
        System.Collections.Generic.List<(string Name, bool Constant)> names)
    {
        foreach (var statement in body)
        {
            if (statement is JsVariableStatement variable && variable.Kind != SliceDeclarationKind.Var)
            {
                var declared = new System.Collections.Generic.List<string>();

                foreach (var declarator in variable.Declarators)
                {
                    CollectDeclaratorNames(declarator, declared);
                }

                foreach (var name in declared)
                {
                    names.Add((name, variable.Kind == SliceDeclarationKind.Const));
                }
            }

            // A CLASS DECLARATION IS A LEXICAL DECLARATION AND NOT A HOISTED ONE. Leaving it out
            // of this collection would mean the block enclosing it pushes no scope, and the class
            // binding would land in whatever scope happened to be current - which for a class in a
            // loop body is a slot the next turn overwrites.
            if (statement is JsClassDeclaration declaration && declaration.Class.Name.Length != 0)
            {
                names.Add((declaration.Class.Name, false));
            }

            // AND SO IS A FUNCTION DECLARATION, once it is written in a block rather than at the
            // top level of a body. The name belongs to the block's record, so the block has to
            // push one even where the declaration is the only thing in it: without this a
            // `{ function f() { } }` pushed nothing, and the slot the declaration then wanted was
            // the enclosing function's.
            if (Declared(statement) is { } function && function.Function.Name.Length != 0)
            {
                names.Add((function.Function.Name, false));
            }
        }
    }

    /// <summary>
    /// The function declaration one statement of a block is, looking through any labels on it.
    /// </summary>
    /// <remarks>
    /// <b>A LABEL DOES NOT MOVE THE BINDING IT LABELS.</b> <c>{ l: function f() { } }</c> declares
    /// <c>f</c> in the block exactly as <c>{ function f() { } }</c> does, because the language's
    /// <c>LexicallyDeclaredNames</c> of a statement list looks through a <c>LabelledStatement</c>
    /// to the declaration under it. A collector that matched only the bare declaration would leave
    /// the labelled one to be resolved as a global, which is the same defect one syntax away.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=B8CA09
    // Broiler-Human:        PENDING
    private static JsFunctionDeclaration? Declared(JsStatement statement)
    {
        while (statement is JsLabelledStatement labelled)
        {
            statement = labelled.Body;
        }

        return statement as JsFunctionDeclaration;
    }

    /// <summary>Every name one declarator introduces, whether it names one or destructures.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3DAD89
    // Broiler-Human:        PENDING
    private static void CollectDeclaratorNames(
        JsDeclarator declarator, System.Collections.Generic.List<string> names) =>
        CollectHeadNames(declarator.Name, declarator.Pattern, names);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=7A9717
    // Broiler-Human:        PENDING
    private static void CollectHeadNames(
        string name, JsPattern? pattern, System.Collections.Generic.List<string> names)
    {
        if (pattern is null)
        {
            names.Add(name);
            return;
        }

        CollectPatternNames(pattern, names);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=DB0D8E
    // Broiler-Human:        PENDING
    private static void CollectPatternNames(
        JsPattern pattern, System.Collections.Generic.List<string> names)
    {
        switch (pattern)
        {
            case JsTargetPattern { Target: JsIdentifier identifier }:
                names.Add(identifier.Name);
                return;

            case JsArrayPattern array:
                foreach (var element in array.Elements)
                {
                    if (element is not null)
                    {
                        CollectPatternNames(element.Target, names);
                    }
                }

                if (array.Rest is not null)
                {
                    CollectPatternNames(array.Rest, names);
                }

                return;

            case JsObjectPattern literal:
                foreach (var property in literal.Properties)
                {
                    CollectPatternNames(property.Value.Target, names);
                }

                if (literal.Rest is not null)
                {
                    CollectPatternNames(literal.Rest, names);
                }

                return;

            default:
                return;
        }
    }

    /// <summary>
    /// Whether a function's own code mentions <c>arguments</c>, parameter defaults included.
    /// </summary>
    /// <remarks>
    /// The defaults are part of the scan because <c>function f(a = arguments[0]) {}</c> is a
    /// function whose only mention of <c>arguments</c> is in a place a body-only walk never looks,
    /// and the object has to exist before the prologue that reads it runs.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A650F9
    // Broiler-Human:        PENDING
    private static bool UsesArguments(JsFunctionNode function)
    {
        foreach (var parameter in function.Parameters)
        {
            // A PARAMETER LIST'S DIRECT `eval` READS `arguments` THROUGH THE SCOPE MAP as the body's
            // does, and a `var arguments` it declares collides with the object (JSeal V15).
            if (parameter.Default is not null &&
                (Walk.Mentions(parameter.Default, "arguments") || Walk.Mentions(parameter.Default, "eval")))
            {
                return true;
            }

            if (Walk.Mentions(parameter.Target, "arguments") || Walk.Mentions(parameter.Target, "eval"))
            {
                return true;
            }
        }

        foreach (var statement in function.Body)
        {
            // A MENTION OF `eval` IS A POSSIBLE MENTION OF `arguments`, because a direct evaluation
            // in this function's own code - or in an arrow nested in it through arrows only - reads
            // the name through the scope map, and the map can only name a slot that exists
            // (JSD-0026 step 4). An arrow's mention counts for the same reason an arrow's mention of
            // `arguments` does: it has none of its own. A mention that turns out not to be a direct
            // call costs an object nobody reads.
            if (Walk.Mentions(statement, "arguments") || Walk.Mentions(statement, "eval"))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Whether a parameter list - defaults and patterns - mentions <paramref name="name"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=0900D2
    // Broiler-Human:        PENDING
    private static bool ParametersMention(
        System.Collections.Generic.IReadOnlyList<JsParameter> parameters, string name)
    {
        foreach (var parameter in parameters)
        {
            if ((parameter.Default is not null && Walk.Mentions(parameter.Default, name)) ||
                Walk.Mentions(parameter.Target, name))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Whether a parameter list has expressions - a default, a pattern element's default or a
    /// computed key - which is the specification's <c>hasParameterExpressions</c> (JSeal V15-finish).
    /// </summary>
    /// <remarks>
    /// A list without one - a rest parameter, a pattern of names - gives the body no variable
    /// environment of its own in the specification either, so a body evaluation's <c>var</c> of a
    /// parameter's name is the parameter.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=3B4CDC
    // Broiler-Human:        PENDING
    private static bool ParametersHaveExpressions(
        System.Collections.Generic.IReadOnlyList<JsParameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            if (parameter.Default is not null || PatternHasExpression(parameter.Target))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Whether a binding pattern holds a default or a computed key anywhere in it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=DC3790
    // Broiler-Human:        PENDING
    private static bool PatternHasExpression(JsPattern? pattern)
    {
        switch (pattern)
        {
            case JsArrayPattern array:
                foreach (var element in array.Elements)
                {
                    if (element is not null &&
                        (element.Default is not null || PatternHasExpression(element.Target)))
                    {
                        return true;
                    }
                }

                return PatternHasExpression(array.Rest);

            case JsObjectPattern shape:
                foreach (var property in shape.Properties)
                {
                    if (property.Computed is not null || property.Value.Default is not null ||
                        PatternHasExpression(property.Value.Target))
                    {
                        return true;
                    }
                }

                return PatternHasExpression(shape.Rest);

            default:
                return false;
        }
    }

    /// <summary>
    /// Whether a parameter list - defaults and patterns - may make a closure: a function, an arrow,
    /// a class or an object literal's method in its own syntax, or a mention of <c>eval</c>, whose
    /// source may make one (JSeal V15-finish).
    /// </summary>
    /// <remarks>
    /// Such a closure sees the parameters' record, and the specification keeps the body's
    /// declarations from it; a list that makes none cannot tell a body's variable environment from
    /// its own record, so this is the question that decides whether the body gets one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=33F920
    // Broiler-Human:        PENDING
    private static bool ParametersMakeClosures(
        System.Collections.Generic.IReadOnlyList<JsParameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            if ((parameter.Default is not null && Walk.MakesClosure(parameter.Default)) ||
                Walk.MakesClosure(parameter.Target))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Whether a body's own code - its arrows included - mentions the name <c>eval</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=57EF7D
    // Broiler-Human:        PENDING
    private static bool MentionsEval(System.Collections.Generic.IReadOnlyList<JsStatement> body)
    {
        foreach (var statement in body)
        {
            if (Walk.Mentions(statement, "eval"))
            {
                return true;
            }
        }

        return false;
    }
}
