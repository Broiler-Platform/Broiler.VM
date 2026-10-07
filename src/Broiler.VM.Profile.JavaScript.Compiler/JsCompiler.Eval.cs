// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   9
// Annotated:        9/9
// Exempt:           0
// Human-reviewed:   0/9
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       9
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    /// <summary>
    /// The row of the eval scope map that describes <paramref name="target"/>, written after its
    /// parent's row the first time a site needs it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The names are read when the artifact is assembled and not when the site is lowered</b>,
    /// because a function body's top-level lexical declaration is given its slot where the statement
    /// stands, which may be after the site - and the language puts that binding in the site's scope
    /// from the top of the body, in its dead zone until the declaration runs. The slot starts empty,
    /// so a read through the map before then is the <c>ReferenceError</c> the language gives.
    /// </para>
    /// <para>
    /// <b>A name the lowering made up is not written</b>: a <c>#</c>-prefixed slot is a temporary no
    /// source can spell, and a map that carried them would be describing things no evaluation can
    /// reach. A private name's slot, <c>##</c> and the name, is written: an evaluation in the class
    /// reaches it through its own <c>#</c> spelling (JSeal V15-finish).
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D5E6E7
    // Broiler-Human:        PENDING
    private int EvalShapeRow(
        Scope target,
        System.Collections.Generic.List<JsEvalScopeRow> rows,
        System.Collections.Generic.Dictionary<Scope, int> known,
        System.Collections.Generic.Dictionary<Scope, int> knownParameters,
        System.Collections.Generic.HashSet<Scope>? parameters)
    {
        // A FUNCTION SEEN FROM ITS OWN PARAMETER LIST IS A DIFFERENT ROW from the same function seen
        // from its body (JSeal V15, JSD-0026 step 9): the parameters are a record between the site
        // and the variable environment, and the body's declarations are not visible at all.
        var fromParameters = parameters is not null && parameters.Contains(target);
        var memo = fromParameters ? knownParameters : known;

        if (memo.TryGetValue(target, out var existing))
        {
            return existing;
        }

        var parent = target.Parent is null
            ? 0u
            : (uint)EvalShapeRow(target.Parent, rows, known, knownParameters, parameters) + 1;
        var names = new System.Collections.Generic.List<JsEvalNameRow>();
        var spelled = new System.Collections.Generic.List<string>(target.Names);
        spelled.Sort(System.StringComparer.Ordinal);

        foreach (var name in spelled)
        {
            // A PRIVATE NAME'S SLOT IS WRITTEN, `##` and the name, because an evaluation in the class
            // may use it (JSeal V15-finish); no source can spell it as an identifier. A module's
            // `export default` slot is not: its name is one no source can spell either, and nothing
            // an evaluation writes could reach it.
            if ((name.StartsWith('#') && !name.StartsWith("##", System.StringComparison.Ordinal)) ||
                string.Equals(name, JsParser.DefaultBindingName, System.StringComparison.Ordinal) ||
                !target.TryGet(name, out var slot, out var constant))
            {
                continue;
            }

            // A named function expression's own name is written immutable-and-not-strict, so a
            // store through the map answers what EmitStaticStore answers for the same name.
            var flags = target.IsFunctionName
                ? (byte)(JsFormat.EvalBindingImmutable | JsFormat.EvalBindingFunctionName)
                : constant ? JsFormat.EvalBindingImmutable : (byte)0;

            if (fromParameters)
            {
                // A PARAMETER - and the `arguments` object, which is bound beside them - is in the
                // record the parameter list's evaluations see, which lies between them and the
                // variable environment their `var`s go to: a `var` of the same name collides. A name
                // the body declares is not visible from there at all.
                flags |= slot < target.ParameterLimit
                    ? JsFormat.EvalBindingLexical
                    : JsFormat.EvalBindingHidden;
            }
            else if (target.IsLexical(slot))
            {
                flags |= JsFormat.EvalBindingLexical;
            }

            names.Add(new JsEvalNameRow(InternedName(name), (uint)slot, flags));
        }

        // A MODULE'S IMPORTS ARE NAMES OF ITS ROW TOO, each an immutable indirection onto the
        // artifact's import table rather than a slot (JSeal V15-module): the row's "slot" for one is
        // its import entry, which the executor reads through the exporting module's environment on
        // every access, exactly as `LoadImport` does - so an evaluation sees a live binding.
        if (target.Kind == ScopeKind.Module && ModuleOf(target) is { } owner)
        {
            var imported = new System.Collections.Generic.List<string>(owner.Imports.Keys);
            imported.Sort(System.StringComparer.Ordinal);

            foreach (var name in imported)
            {
                names.Add(new JsEvalNameRow(
                    InternedName(name),
                    (uint)owner.Imports[name],
                    (byte)(JsFormat.EvalBindingImmutable | JsFormat.EvalBindingImport)));
            }
        }

        var kind = target.Kind switch
        {
            ScopeKind.Function => JsFormat.EvalScopeKind.Function,
            ScopeKind.Body => JsFormat.EvalScopeKind.FunctionBody,
            ScopeKind.With => JsFormat.EvalScopeKind.With,
            ScopeKind.Program => JsFormat.EvalScopeKind.Program,
            ScopeKind.Eval => JsFormat.EvalScopeKind.Eval,
            ScopeKind.Module => JsFormat.EvalScopeKind.Module,
            _ => target.IsCatch ? JsFormat.EvalScopeKind.Catch : JsFormat.EvalScopeKind.Block,
        };

        var index = rows.Count;
        rows.Add(new JsEvalScopeRow(kind, parent, [.. names]));
        memo[target] = index;
        return index;
    }

    /// <summary>The module whose own environment <paramref name="target"/> is, if any.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=40A5BE
    // Broiler-Human:        PENDING
    private ModuleBuild? ModuleOf(Scope target)
    {
        foreach (var candidate in built)
        {
            if (ReferenceEquals(candidate.Scope, target))
            {
                return candidate;
            }
        }

        return null;
    }

    /// <summary>
    /// Records a direct-<c>eval</c> site about to be emitted, with the scope it sees, when the site
    /// is one a map row admits.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Which sites get a row is the whole of what JSD-0026 step 4 admits</b>: every site in a
    /// function unit, every site in eval code, and a site in a script body when a record lies between
    /// it and the body's entry record. A site at a script body's top level with nothing between gets
    /// none and keeps the global path it always had, because there the caller's scope IS the
    /// global scope.
    /// </para>
    /// <para>
    /// <b>A site whose chain reaches a module's record gets a row too</b> (JSeal V15-module): the
    /// module's record is the root of its chain, a row of its own kind that lists the module's
    /// slots and, for each import, the import entry it reads through - and a site at the module's
    /// top level has a row even with no block around it, because there the caller's scope is the
    /// module's and not the global one. The two
    /// parameter-list shapes JSeal V15 refused here are answered since JSeal V15-finish: a body
    /// whose parameter list may make a closure has a variable environment of its own, so a body
    /// site's evaluation declares where the list's closures cannot see, and a parameter-list site's
    /// row shows the parameters' record, which the body's redeclarations never reach. A
    /// parameter-list site in a function whose list the closure walk did not find making one keeps
    /// the refusal, rather than a row that would show a closure the body's declarations.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C16CF3
    // Broiler-Human:        PENDING
    private void RecordEvalSite()
    {
        if ((buffer.Flags & JsFormat.FunctionFlags.ProgramBody) != 0 && blockDepth == 0 &&
            FunctionScope().Kind != ScopeKind.Module)
        {
            return;
        }

        var privateNames = evalRoot is not null &&
            (evalFlags & JsFormat.EvalRequestFlags.InClassBody) != 0;

        System.Collections.Generic.HashSet<Scope>? parameters = null;

        for (var current = scope; current is not null; current = current.Parent)
        {
            // A PARAMETER LIST'S SITE SEES THE PARAMETERS' RECORD AND NOT THE BODY'S DECLARATIONS
            // (JSeal V15, JSD-0026 step 9), and its row says so; the functions it belongs to are
            // remembered so that the row of each is the one seen from its parameter list. A body
            // whose parameter list could tell has a variable environment of its own, which no
            // parameter-list site's chain reaches (JSeal V15-finish).
            if (parameterScopes.Contains(current))
            {
                // A list the walk that decides the body's record could not see into keeps the
                // refusal rather than a row that would show its closures the body's declarations.
                if (!current.SeparateBody)
                {
                    return;
                }

                (parameters ??= new(System.Collections.Generic.ReferenceEqualityComparer.Instance)).Add(current);
            }

            foreach (var name in current.Names)
            {
                if (name.StartsWith("##", System.StringComparison.Ordinal))
                {
                    privateNames = true;
                    break;
                }
            }
        }

        var flags = strict ? JsFormat.EvalRequestFlags.Strict : JsFormat.EvalRequestFlags.None;

        if (insideFunction)
        {
            flags |= JsFormat.EvalRequestFlags.InFunction;
        }

        if (insideMethod)
        {
            flags |= JsFormat.EvalRequestFlags.InMethod;
        }

        if (insideDerivedConstructor)
        {
            flags |= JsFormat.EvalRequestFlags.InDerivedConstructor;
        }

        if (insideFieldInitialiser)
        {
            flags |= JsFormat.EvalRequestFlags.InClassFieldInitializer;
        }

        if (privateNames)
        {
            flags |= JsFormat.EvalRequestFlags.InClassBody;
        }

        buffer.EvalSites.Add((buffer.Code.Count, scope, blockDepth, flags, parameters));
    }

    /// <summary>
    /// How many records lie between the cursor and the evaluated program's boundary record, when a
    /// free name at the cursor is one of the caller's.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8A484B
    // Broiler-Human:        PENDING
    private bool TryEvalHops(out byte hops)
    {
        hops = 0;

        if (evalRoot is null)
        {
            return false;
        }

        var count = Hops(evalRoot);

        if (count > (int)JsFormat.CeilingScopeDepth)
        {
            Refuse(
                default,
                SliceSourceDiagnosticCode.NestingTooDeep,
                "evaluated source nests deeper than the scope-depth ceiling");

            return false;
        }

        hops = (byte)count;
        return true;
    }

    /// <summary>Emits one eval name instruction for a free name of the caller's.</summary>
    /// <remarks>
    /// <b>An <c>arguments</c> in a class field initialiser is refused here</b>, because this is the
    /// one place such a name is recognised as the caller's: a function the evaluated program
    /// declares has an <c>arguments</c> of its own and resolves the name to its own slot, so only a
    /// mention at the program's top level or in its arrows reaches this far - which is exactly the
    /// <c>ContainsArguments</c> the specification's <c>PerformEval</c> tests.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3456DB
    // Broiler-Human:        PENDING
    private void EmitEvalName(JsOpcode opcode, byte hops, string name)
    {
        if ((evalFlags & JsFormat.EvalRequestFlags.InClassFieldInitializer) != 0 &&
            string.Equals(name, "arguments", System.StringComparison.Ordinal))
        {
            Refuse(
                default,
                SliceSourceDiagnosticCode.UnresolvableIdentifier,
                "`arguments` names nothing inside a class field initialiser");
        }

        DeclareSurfaceOf(name);
        EmitScoped(opcode, hops, InternedName(name));
    }

    /// <summary>Lowers one evaluated program: direct <c>eval</c> code for one call site.</summary>
    /// <remarks>
    /// <para>
    /// <b>It is a script body with three differences</b> (JSD-0026 section 5). Its record is the
    /// eval boundary, whose parent at run time is the caller's current record; its top-level
    /// <c>let</c>, <c>const</c> and <c>class</c> declarations - and in strict code its <c>var</c>s
    /// and functions - are slots of that record rather than bindings of the realm; and every name it
    /// does not bind is an eval name instruction rather than a global one. Its completion value is
    /// slot zero, as a script's is.
    /// </para>
    /// <para>
    /// <b>A sloppy program's <c>var</c> and function declarations are its caller's</b> (JSeal V15,
    /// JSD-0026 steps 6-8): the declaration row lists them and the executor checks and creates them
    /// before the first instruction runs. A reference to its caller's <c>super</c> is its caller's
    /// method's, through the active function the eval frame is entered with; a private name it uses
    /// and does not declare is read through the caller's map and listed on the declaration row,
    /// which the executor checks against the classes around the call before anything is
    /// instantiated (JSeal V15-finish). What the language makes a syntax error in the
    /// caller's context - <c>new.target</c> outside a function, <c>super</c> outside a method,
    /// <c>arguments</c> in a field initialiser - is refused as one.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=0DC852
    // Broiler-Human:        PENDING
    private int CompileEvalProgram(JsProgramNode program, JsFormat.EvalRequestFlags flags)
    {
        var outerBuffer = buffer;
        var outerScope = scope;
        var outerDepth = blockDepth;
        var outerStrict = strict;
        var outerMethod = insideMethod;
        var outerDerived = insideDerivedConstructor;
        var outerFunction = insideFunction;
        var outerField = insideFieldInitialiser;
        var outerExits = exits;

        insideMethod = (flags & JsFormat.EvalRequestFlags.InMethod) != 0;
        insideDerivedConstructor = (flags & JsFormat.EvalRequestFlags.InDerivedConstructor) != 0;
        insideFunction = (flags & JsFormat.EvalRequestFlags.InFunction) != 0;
        insideFieldInitialiser = (flags & JsFormat.EvalRequestFlags.InClassFieldInitializer) != 0;
        exits = [];

        strict = program.IsStrict || (flags & JsFormat.EvalRequestFlags.Strict) != 0;
        var index = units.Count;
        buffer = new UnitBuffer(0, JsFormat.FunctionFlags.EvalCode | Strictness());
        units.Add(buffer);
        scope = new Scope(ScopeKind.Eval, null);
        evalRoot = scope;
        evalFlags = flags;
        evalPrivateNames = [];
        blockDepth = 0;

        var completion = scope.Declare("#completion", constant: false);

        Emit(JsOpcode.LoadUndefined);
        EmitScoped(JsOpcode.InitialiseScoped, 0, completion);

        var (varNames, lexicalNames, functionNames, annexBNames) = HoistEval(program.Body);

        // A DIRECTIVE'S STRING IS A COMPLETION VALUE IN EVAL CODE AS IN A SCRIPT: `eval("'1'")` is
        // `"1"`. The script lowering has always said so; this one did not until the global
        // evaluations of JSeal V15 moved onto it and the conformance suite noticed.
        if (program.Directives.Count != 0)
        {
            CompileExpression(program.Directives[^1]);
            EmitScoped(JsOpcode.InitialiseScoped, 0, completion);
        }

        CompileStatements(program.Body, completion);

        EmitScoped(JsOpcode.LoadScoped, 0, completion);
        Emit(JsOpcode.Return);

        buffer.SlotCount = scope.SlotCount;

        if (scope.SlotCount > MaximumSlots)
        {
            Refuse(program.Span, SliceSourceDiagnosticCode.TooManyLocals, "evaluated source declares too many bindings");
        }

        evalDeclarations.Add((
            index, flags, JsFormat.EvalRefusal.None, varNames, lexicalNames, functionNames, annexBNames,
            evalPrivateNames.ToArray()));

        evalRoot = null;
        buffer = outerBuffer;
        scope = outerScope;
        blockDepth = outerDepth;
        strict = outerStrict;
        insideMethod = outerMethod;
        insideDerivedConstructor = outerDerived;
        insideFunction = outerFunction;
        insideFieldInitialiser = outerField;
        exits = outerExits;
        return index;
    }

    /// <summary>
    /// Declares an evaluated program's own bindings in its boundary record, before any of its code
    /// runs, and answers what it declares.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The lexical names come first and every one starts in its dead zone</b>, which is what the
    /// specification's <c>EvalDeclarationInstantiation</c> gives them and what a function declared
    /// by the same program - compiled right after - must see when it names one.
    /// </para>
    /// <para>
    /// <b>A strict program's <c>var</c>s and functions are slots of the same record</b>, as a function
    /// body's are, and nothing is answered for its caller. <b>A sloppy program's are its caller's</b>
    /// (JSeal V15, JSD-0026 steps 6-8): none is declared here, so every mention of one is an eval name
    /// instruction resolved in the caller's scope; the answer lists them for the declaration row -
    /// the <c>var</c>s that are not also functions, the functions in the order the specification
    /// initialises them with the last declaration of a name winning, and the block functions Annex B
    /// may hoist - and the executor checks and creates them before the first instruction. What this
    /// prologue does is make the function objects, under the program's own record, and write each
    /// into the variable environment with <see cref="JsOpcode.StoreEvalVariable"/>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=392CDE
    // Broiler-Human:        PENDING
    private (string[] VarNames, string[] LexicalNames, string[] FunctionNames, string[] AnnexBNames) HoistEval(
        System.Collections.Generic.IReadOnlyList<JsStatement> body)
    {
        var lexical = new System.Collections.Generic.Dictionary<string, bool>(
            System.StringComparer.Ordinal);

        CollectLexicalKinds(body, lexical);

        foreach (var pair in lexical)
        {
            scope.Declare(pair.Key, pair.Value);
        }

        var lexicalNames = new string[lexical.Count];
        lexical.Keys.CopyTo(lexicalNames, 0);

        var names = new System.Collections.Generic.List<string>();
        var functions = new System.Collections.Generic.List<JsFunctionNode>();
        CollectVarScope(body, names, functions, lexical: null);

        if (strict)
        {
            HoistStrictEval(names, functions);
            return ([], lexicalNames, [], []);
        }

        // THE FUNCTIONS THE SPECIFICATION INITIALISES: one per name, the LAST declaration of a name
        // winning, in the order those winning declarations are written.
        var winners = new System.Collections.Generic.List<JsFunctionNode>();
        var functionSeen = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        for (var at = functions.Count - 1; at >= 0; at--)
        {
            if (functionSeen.Add(functions[at].Name))
            {
                winners.Add(functions[at]);
            }
        }

        winners.Reverse();

        var varNames = new System.Collections.Generic.List<string>();
        var varSeen = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        foreach (var name in names)
        {
            if (!functionSeen.Contains(name) && varSeen.Add(name))
            {
                varNames.Add(name);
            }
        }

        var aliases = new System.Collections.Generic.List<string>();

        ScanAnnexB(
            body,
            new System.Collections.Generic.HashSet<string>(lexical.Keys, System.StringComparer.Ordinal),
            aliases);

        var annexBNames = new System.Collections.Generic.List<string>();
        var aliasSeen = new System.Collections.Generic.HashSet<string>(System.StringComparer.Ordinal);

        foreach (var alias in aliases)
        {
            if (aliasSeen.Add(alias))
            {
                annexBNames.Add(alias);
            }
        }

        var functionNames = new string[winners.Count];

        for (var index = 0; index < winners.Count; index++)
        {
            functionNames[index] = winners[index].Name;
            Emit(JsOpcode.Closure, (ushort)CompileFunction(winners[index]));
            EmitScoped(JsOpcode.StoreEvalVariable, 0, InternedName(winners[index].Name));
        }

        return ([.. varNames], lexicalNames, functionNames, [.. annexBNames]);
    }

    /// <summary>Declares a strict evaluated program's <c>var</c>s and functions as its own slots.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=7539EC
    // Broiler-Human:        PENDING
    private void HoistStrictEval(
        System.Collections.Generic.List<string> names,
        System.Collections.Generic.List<JsFunctionNode> functions)
    {
        foreach (var name in names)
        {
            if (scope.Has(name))
            {
                continue;
            }

            var slot = scope.Declare(name, constant: false);
            Emit(JsOpcode.LoadUndefined);
            EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
        }

        foreach (var function in functions)
        {
            if (!scope.Has(function.Name))
            {
                var slot = scope.Declare(function.Name, constant: false);
                Emit(JsOpcode.LoadUndefined);
                EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
            }
        }

        foreach (var function in functions)
        {
            Emit(JsOpcode.Closure, (ushort)CompileFunction(function));
            EmitScoped(JsOpcode.InitialiseScoped, 0, scope.SlotOf(function.Name));
        }
    }
}
