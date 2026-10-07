// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   13
// Annotated:        13/13
// Exempt:           0
// Human-reviewed:   0/13
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       13
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{

    // ---- modules -------------------------------------------------------------------------------

    /// <summary>
    /// Lowers one module: its declarations, its two code units and its record.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A module gets TWO code units and a script gets one, and the second is not a
    /// convenience.</b> The specification initialises a module's environment - <c>var</c> bindings
    /// to <c>undefined</c> and function declarations to their closures - for EVERY module in the
    /// graph before ANY module is evaluated. With no cycle in the graph the difference is invisible,
    /// because a dependency is evaluated before its dependent anyway. With a cycle it is the whole
    /// behaviour: the module that runs first calls a function of the module that has not run, and
    /// that call has to work. Doing the initialisation in the body's own prologue would make it a
    /// binding in the temporal dead zone instead, and a legal cyclic program would throw.
    /// </para>
    /// <para>
    /// <b>Module-level bindings are SLOTS and not properties of the global object</b>, which is
    /// where this differs most from the script lowering above. A module's declarations are not
    /// visible to the next script, exporting one has to name a slot the exporting environment keeps
    /// for the lifetime of the instance, and <c>globalThis.x</c> must not see it.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=344580
    // Broiler-Human:        PENDING
    private void CompileModule(JsProgramNode program, JsModuleUnit unit)
    {
        var key = unit.Key;

        foreach (var existing in built)
        {
            if (string.Equals(existing.Key, key, System.StringComparison.Ordinal))
            {
                Refuse(
                    program.Span,
                    SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                    "two modules were presented under the key `" + key + "`");

                return;
            }
        }

        var build = new ModuleBuild(key, new Scope(ScopeKind.Module, null)) { Unit = unit };
        built.Add(build);

        var outerStrict = strict;
        var outerScope = scope;
        var outerBuffer = buffer;
        var outerDepth = blockDepth;
        var outerModule = module;

        strict = true;
        module = build;
        scope = build.Scope;
        blockDepth = 0;

        DeclareImports(program.Body, build);
        var variables = DeclareModuleBindings(program.Body, build);
        DeclareExports(program.Body, build);

        build.InitialiserUnit = EmitModuleInitialiser(program, build, variables);
        build.BodyUnit = EmitModuleBody(program, build);

        units[build.InitialiserUnit].SlotCount = build.Scope.SlotCount;
        units[build.BodyUnit].SlotCount = build.Scope.SlotCount;

        if (build.Scope.SlotCount > MaximumSlots)
        {
            Refuse(
                program.Span,
                SliceSourceDiagnosticCode.TooManyLocals,
                "the module `" + key + "` declares too many bindings");
        }

        strict = outerStrict;
        scope = outerScope;
        buffer = outerBuffer;
        blockDepth = outerDepth;
        module = outerModule;
    }

    /// <summary>
    /// Records this module's requests and import entries, and binds the local names to them.
    /// </summary>
    /// <remarks>
    /// <b>No slot is declared for an import.</b> An imported name is an indirection onto the
    /// exporting module's slot, so giving it one here would create the copy that makes a live
    /// binding stale - see <see cref="JsOpcode.LoadImport"/>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=11D9F4
    // Broiler-Human:        PENDING
    private void DeclareImports(
        System.Collections.Generic.IReadOnlyList<JsStatement> body, ModuleBuild build)
    {
        foreach (var statement in body)
        {
            switch (statement)
            {
                case JsImportDeclaration import:
                {
                    var request = RequestIndex(
                        build,
                        JsFormat.TypedSpecifier(
                            import.Specifier, AttributeType(import.Span, import.Attributes, refuse: true)));

                    foreach (var specifier in import.Specifiers)
                    {
                        // An import binds a name in strict code, where `arguments` and `eval` are
                        // not binding names (13.1.1). Until 2026-10-03 both imported (JSC-253).
                        if (specifier.Local is "arguments" or "eval")
                        {
                            Refuse(
                                specifier.Span,
                                SliceSourceDiagnosticCode.ReservedWordAsBinding,
                                "`" + specifier.Local + "` is not a binding name in a module");

                            continue;
                        }

                        if (build.Imports.ContainsKey(specifier.Local))
                        {
                            Refuse(
                                specifier.Span,
                                SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                                "`" + specifier.Local + "` is imported twice");

                            continue;
                        }

                        var row = new JsImportEntryRow(
                            request,
                            specifier.Namespace ? 0u : InternedName(specifier.Imported),
                            specifier.Namespace
                                ? JsFormat.ImportKind.Namespace
                                : JsFormat.ImportKind.Named);

                        build.Imports[specifier.Local] = importEntries.Count;
                        importEntries.Add(row);
                        build.ImportRows.Add(row);
                    }

                    break;
                }

                case JsExportDeclaration exported when exported.From.Length != 0:
                    RequestIndex(
                        build,
                        JsFormat.TypedSpecifier(
                            exported.From, AttributeType(exported.Span, exported.Attributes, refuse: true)));
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// The module type an attributes clause asks for: empty for none, or
    /// <see cref="JsFormat.JsonModuleType"/>; every other attribute is declined.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>One attribute is honoured, and it is the one the language defines.</b> <c>type: "json"</c>
    /// asks for the module a specifier names to be loaded as a JSON document, and that request is a
    /// different module from the untyped one: the type travels with the specifier through every
    /// table that matches a request (<see cref="JsFormat.TypedSpecifier"/>), and the composition
    /// loads the text the key names as a document rather than as a program.
    /// </para>
    /// <para>
    /// <b>Every other attribute is declined, and the clause is not.</b> An attribute changes what
    /// the host LOADS, and for any key but <c>type</c>, or any type but <c>json</c>, no composition
    /// of this profile has a loader. A static import loads at compile time here - the graph is
    /// resolved before the artifact's bytes are written - so an attribute the loader cannot honour
    /// is discovered at exactly the moment an unresolvable specifier is, and the dynamic form
    /// refuses the same attribute at run time, by rejecting the promise it answered with. An empty
    /// clause asks nothing of the host and is not an attribute.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4D385E
    // Broiler-Human:        PENDING
    private string AttributeType(
        SliceSourceSpan span,
        System.Collections.Generic.IReadOnlyList<JsImportAttribute>? attributes,
        bool refuse)
    {
        if (attributes is null || attributes.Count == 0)
        {
            return string.Empty;
        }

        var type = string.Empty;

        foreach (var attribute in attributes)
        {
            if (string.Equals(attribute.Key, "type", System.StringComparison.Ordinal) &&
                string.Equals(attribute.Value, JsFormat.JsonModuleType, System.StringComparison.Ordinal))
            {
                type = JsFormat.JsonModuleType;
                continue;
            }

            if (refuse)
            {
                Refuse(
                    span,
                    SliceSourceDiagnosticCode.UnsupportedImportAttribute,
                    "no composition of this profile can honour the import attribute `" +
                        attribute.Key + "`, so the module this clause decorates cannot be loaded");
            }

            return string.Empty;
        }

        return type;
    }

    /// <summary>The request specifier of a re-export: its specifier under the type it asks for.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=7A3A69
    // Broiler-Human:        PENDING
    private string TypedFrom(JsExportDeclaration exported) =>
        JsFormat.TypedSpecifier(exported.From, AttributeType(exported.Span, exported.Attributes, refuse: false));

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C59449
    // Broiler-Human:        PENDING
    private uint RequestIndex(ModuleBuild build, string specifier)
    {
        for (var index = 0; index < build.Requests.Count; index++)
        {
            if (string.Equals(build.Requests[index], specifier, System.StringComparison.Ordinal))
            {
                return (uint)index;
            }
        }

        build.Requests.Add(specifier);
        build.RequestKeys.Add(Resolved(build, specifier));
        return (uint)(build.Requests.Count - 1);
    }

    /// <summary>
    /// The key the composition resolved a specifier to, or the specifier itself when it named none.
    /// </summary>
    /// <remarks>
    /// <b>An unresolved specifier is recorded verbatim rather than refused here, and the artifact is
    /// then refused by the verifier.</b> Resolution is not this component's, so a specifier with no
    /// resolution is a producer that did not finish its own job - and the honest place to say so is
    /// the pass that looks at the whole graph, which answers that the request names no module the
    /// artifact carries. Inventing a source diagnostic for it would blame the program for something
    /// its text cannot express.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B16A76
    // Broiler-Human:        PENDING
    private static string Resolved(ModuleBuild build, string specifier)
    {
        if (build.Unit?.Requests is { } requests)
        {
            foreach (var request in requests)
            {
                if (string.Equals(request.Specifier, specifier, System.StringComparison.Ordinal))
                {
                    return request.Key;
                }
            }
        }

        return specifier;
    }

    /// <summary>
    /// Declares every slot the module's own environment holds, before any code is emitted.
    /// </summary>
    /// <remarks>
    /// The <c>var</c> names are answered so the initialiser can set them to <c>undefined</c>; the
    /// lexical ones are declared and left uninitialised, which is the temporal dead zone and is what
    /// an importer that reads too early has to meet.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=50A13A
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<string> DeclareModuleBindings(
        System.Collections.Generic.IReadOnlyList<JsStatement> body, ModuleBuild build)
    {
        var names = new System.Collections.Generic.List<string>();
        var functions = new System.Collections.Generic.List<JsFunctionNode>();
        var lexical = new System.Collections.Generic.List<string>();
        CollectVarScope(Unwrapped(body), names, functions, lexical);

        foreach (var name in names)
        {
            Declared(build, name, constant: false);
        }

        // A SECOND FUNCTION DECLARATION OF ONE NAME IS AN ERROR IN A MODULE AND NOT IN A SCRIPT.
        // A script's top level lets the later declaration win, because a script's declarations are
        // properties of the global object; a module's are lexical, and lexical names may be
        // declared once. The two goals genuinely differ, so this check is here and not shared.
        var declaredFunctions = new System.Collections.Generic.HashSet<string>(
            System.StringComparer.Ordinal);

        foreach (var function in functions)
        {
            build.Functions.Add(function);

            if (!declaredFunctions.Add(function.Name))
            {
                Refuse(
                    function.Span,
                    SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                    "`" + function.Name + "` is declared twice at this module's top level");
            }

            // AND IT COLLIDES WITH A `var` OF THE SAME NAME, for the same reason: a module's
            // function is lexical (16.2.1.1). Until 2026-10-03 `var f; function f() {}` compiled in
            // a module (JSC-253).
            if (names.Contains(function.Name))
            {
                Refuse(
                    function.Span,
                    SliceSourceDiagnosticCode.VarAndLexicalCollision,
                    "`" + function.Name + "` is declared both as a `var` and as a function");
            }

            Declared(build, function.Name, constant: false);
        }

        var declaredLexically = new System.Collections.Generic.HashSet<string>(
            declaredFunctions, System.StringComparer.Ordinal);

        foreach (var statement in Unwrapped(body))
        {
            // A CLASS DECLARATION IS A LEXICAL DECLARATION OF THIS MODULE, and it needs its slot
            // before the body runs for the same reason a `let` does: an importer may hold a live
            // binding to it, and the slot it reads through has to exist and be uninitialised until
            // the class is evaluated.
            if (statement is JsClassDeclaration declaredClass &&
                declaredClass.Class.Name.Length != 0)
            {
                if (!declaredLexically.Add(declaredClass.Class.Name))
                {
                    Refuse(
                        declaredClass.Span,
                        SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                        "`" + declaredClass.Class.Name +
                            "` is declared twice at this module's top level");
                }

                Declared(build, declaredClass.Class.Name, constant: false);
                continue;
            }

            if (statement is not JsVariableStatement variable ||
                variable.Kind == SliceDeclarationKind.Var)
            {
                continue;
            }

            foreach (var declarator in variable.Declarators)
            {
                // A DESTRUCTURING DECLARATOR HAS NO NAME OF ITS OWN AND DECLARES EVERY NAME IN ITS
                // PATTERN. Reading `declarator.Name` gave the empty string for each one, so two
                // top-level `const {a} = o; const {b} = o;` were refused as the same undeclared name
                // twice (VM-FIX-D), and a pattern's real names were never checked at all.
                var declared = new System.Collections.Generic.List<string>();
                CollectDeclaratorNames(declarator, declared);

                foreach (var name in declared)
                {
                    if (!declaredLexically.Add(name))
                    {
                        Refuse(
                            declarator.Span,
                            SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                            "`" + name + "` is declared twice at this module's top level");
                    }

                    if (names.Contains(name))
                    {
                        Refuse(
                            declarator.Span,
                            SliceSourceDiagnosticCode.VarAndLexicalCollision,
                            "`" + name + "` is declared both as a `var` and lexically");
                    }

                    Declared(build, name, variable.Kind == SliceDeclarationKind.Const);
                }
            }
        }

        _ = lexical;

        // `export default <expression>` binds a slot with a name no source can write, so nothing
        // it collides with can be declared and no collision check is owed for it.
        foreach (var statement in body)
        {
            if (statement is JsExportDeclaration { Kind: JsExportKind.Default, Default: not null })
            {
                build.Scope.Declare(JsParser.DefaultBindingName, constant: false);
            }
        }

        return names;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CDC140
    // Broiler-Human:        PENDING
    private void Declared(ModuleBuild build, string name, bool constant)
    {
        if (name.Length == 0)
        {
            return;
        }

        if (build.Imports.ContainsKey(name))
        {
            Refuse(
                default,
                SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                "`" + name + "` is both imported and declared in this module");

            return;
        }

        build.Scope.Declare(name, constant);
    }

    /// <summary>Records what the module publishes, and refuses a name it publishes twice.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=112B2E
    // Broiler-Human:        PENDING
    private void DeclareExports(
        System.Collections.Generic.IReadOnlyList<JsStatement> body, ModuleBuild build)
    {
        foreach (var statement in body)
        {
            if (statement is not JsExportDeclaration exported)
            {
                continue;
            }

            if (exported.Kind == JsExportKind.All && exported.Specifiers.Count == 0)
            {
                build.StarExports.Add(RequestIndex(build, TypedFrom(exported)));
                continue;
            }

            foreach (var specifier in exported.Specifiers)
            {
                if (!build.ExportNames.Add(specifier.Exported))
                {
                    Refuse(
                        specifier.Span,
                        SliceSourceDiagnosticCode.DuplicateExportName,
                        "`" + specifier.Exported + "` is exported twice");

                    continue;
                }

                if (exported.From.Length != 0)
                {
                    var request = RequestIndex(build, TypedFrom(exported));

                    build.IndirectExports.Add(
                        new JsIndirectExportRow(
                            InternedName(specifier.Exported),
                            request,
                            specifier.Local == JsParser.StarName
                                ? 0u
                                : InternedName(specifier.Local),
                            specifier.Local == JsParser.StarName
                                ? JsFormat.ImportKind.Namespace
                                : JsFormat.ImportKind.Named));

                    continue;
                }

                // RE-EXPORTING AN IMPORT IS AN INDIRECT EXPORT AND NOT A LOCAL ONE. `import { a }
                // from './m'; export { a };` publishes the binding of the OTHER module, and
                // copying its value into a slot of this one would be the stale copy again - so it
                // is recorded as the indirection it is and resolved with the graph.
                if (build.Imports.TryGetValue(specifier.Local, out var entry))
                {
                    var row = importEntries[entry];

                    build.IndirectExports.Add(
                        new JsIndirectExportRow(
                            InternedName(specifier.Exported),
                            row.RequestIndex,
                            row.NameConstant,
                            row.Kind));

                    continue;
                }

                if (!build.Scope.TryGet(specifier.Local, out var slot, out _))
                {
                    Refuse(
                        specifier.Span,
                        SliceSourceDiagnosticCode.ExportNameNotDeclared,
                        "`" + specifier.Local + "` is exported and this module declares no such binding");

                    continue;
                }

                build.LocalExports.Add(
                    new JsLocalExportRow(InternedName(specifier.Exported), (uint)slot));
            }
        }
    }

    /// <summary>
    /// Emits the unit that initialises the module's environment before anything is evaluated.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=EF8BCE
    // Broiler-Human:        PENDING
    private int EmitModuleInitialiser(
        JsProgramNode program,
        ModuleBuild build,
        System.Collections.Generic.List<string> variables)
    {
        var index = units.Count;
        buffer = new UnitBuffer(0, JsFormat.FunctionFlags.ProgramBody | JsFormat.FunctionFlags.Strict);
        units.Add(buffer);
        Position(program.Span);

        foreach (var name in variables)
        {
            if (!build.Scope.TryGet(name, out var slot, out _))
            {
                continue;
            }

            Emit(JsOpcode.LoadUndefined);
            EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
        }

        foreach (var function in build.Functions)
        {
            if (!build.Scope.TryGet(function.Name, out var slot, out _))
            {
                continue;
            }

            Emit(JsOpcode.Closure, (ushort)CompileFunction(function));
            EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
        }

        Emit(JsOpcode.ReturnUndefined);
        return index;
    }

    /// <summary>Emits the module's body, which is its statements minus the declarations.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=17AA45
    // Broiler-Human:        PENDING
    private int EmitModuleBody(JsProgramNode program, ModuleBuild build)
    {
        var index = units.Count;

        // A MODULE THAT AWAITS IS ENTERED AS AN ASYNC FRAME, and that is the only difference
        // top-level `await` makes to the lowering. The body is the same instructions either way;
        // what the flag decides is whether the linker enters it through the async driver - which
        // can suspend and be resumed by a job - or runs it straight through on the native stack,
        // which cannot.
        var flags = JsFormat.FunctionFlags.ProgramBody | JsFormat.FunctionFlags.Strict;

        if (awaited)
        {
            flags |= JsFormat.FunctionFlags.Async;
        }

        buffer = new UnitBuffer(0, flags);
        units.Add(buffer);

        var completion = build.Scope.Declare("#completion", constant: false);
        Emit(JsOpcode.LoadUndefined);
        EmitScoped(JsOpcode.InitialiseScoped, 0, completion);

        // A MODULE'S OWN RESOURCES ARE DISPOSED WHEN ITS BODY ENDS, before the module's evaluation
        // settles - which, for an `await using`, is after the awaits the disposal owes. The body is
        // then an async unit because the parser saw the declaration as a top-level await.
        var resources = UsingIn(program.Body, out var first);
        var disposal = resources == JsUsing.None
            ? null
            : BeginDisposal(first, resources == JsUsing.Async);

        foreach (var statement in program.Body)
        {
            switch (statement)
            {
                // An `import` declaration has no run-time behaviour of its own: the request is in
                // the record and the binding is an indirection. A `function` declaration was
                // already given its closure by the initialiser.
                case JsImportDeclaration:
                case JsFunctionDeclaration:
                    break;

                case JsExportDeclaration { Declaration: JsFunctionDeclaration }:
                case JsExportDeclaration { Kind: JsExportKind.Named }:
                case JsExportDeclaration { Kind: JsExportKind.All }:
                    break;

                case JsExportDeclaration { Declaration: { } declared }:
                    CompileStatement(declared, completion);
                    break;

                case JsExportDeclaration { Kind: JsExportKind.Default, Default: { } value }:
                {
                    // AN ANONYMOUS FUNCTION EXPORTED AS THE DEFAULT IS NAMED `default`, which the
                    // language states as a step of the export's own evaluation rather than as a
                    // property of the function - so the name has to be applied here, where the
                    // export is, and not in the general lowering of a function expression.
                    // A CLASS IS NAMED THE SAME WAY, and `export default (class { })` reaches here
                    // as a class expression: until 2026-10-04 only a function was (JSC-255).
                    CompileNamedValue(value, "default");

                    EmitScoped(
                        JsOpcode.InitialiseScoped,
                        0,
                        build.Scope.SlotOf(JsParser.DefaultBindingName));

                    break;
                }

                default:
                    CompileStatement(statement, completion);
                    break;
            }
        }

        if (disposal is not null)
        {
            EndDisposal(disposal);
        }

        EmitScoped(JsOpcode.LoadScoped, 0, completion);
        Emit(JsOpcode.Return);
        return index;
    }

    /// <summary>
    /// A statement list with each exported declaration replaced by the declaration itself.
    /// </summary>
    /// <remarks>
    /// The collectors that find <c>var</c> names, function declarations and lexical names are the
    /// script lowering's and know nothing about <c>export</c>. Unwrapping here rather than teaching
    /// each of them the module goal keeps one answer to what a declaration is.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=67CC95
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<JsStatement> Unwrapped(
        System.Collections.Generic.IReadOnlyList<JsStatement> body)
    {
        var flattened = new System.Collections.Generic.List<JsStatement>(body.Count);

        foreach (var statement in body)
        {
            flattened.Add(
                statement is JsExportDeclaration { Declaration: { } declared } ? declared : statement);
        }

        return flattened;
    }
}
