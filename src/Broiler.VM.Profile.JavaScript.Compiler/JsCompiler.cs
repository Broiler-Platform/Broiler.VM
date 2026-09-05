// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   80
// Annotated:        80/80
// Exempt:           50
// Human-reviewed:   0/80
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       80
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>One source text to compile into a code unit of the same artifact.</summary>
/// <param name="Name">The entry-point name the host will invoke this unit by.</param>
/// <param name="Text">The source.</param>
/// <param name="Options">The parse options: goal symbol and nesting bound.</param>
/// <param name="ForceStrict">
/// Whether to compile the unit as strict code whatever its own prologue says. The conformance
/// harness's strict variant is exactly this, and it is a flag rather than a text edit because
/// prepending a directive changes the source positions every diagnostic reports.
/// </param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4443B7
// Broiler-Human:        PENDING
public sealed record JsScriptUnit(
    string Name, string Text, SliceParseOptions Options, bool ForceStrict = false);

/// <summary>One source text to compile into a module record of the same artifact.</summary>
/// <param name="Key">
/// <b>The key the COMPOSITION resolved, not the specifier the source wrote.</b> Turning
/// <c>"./b.mjs"</c> into the identity of a module is a host decision - a file path, a URL, a name in
/// a bundle - and this component takes no part in it. What arrives here is the composition's
/// answer, and a request in one module is matched against it by exact comparison and nothing else.
/// </param>
/// <param name="Text">The source.</param>
/// <param name="Options">
/// The parse options. The goal is checked rather than assumed: a module compiled under the script
/// goal would refuse its own <c>import</c>.
/// </param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
// Broiler-Human:        PENDING
public sealed record JsModuleUnit(
    string Key,
    string Text,
    SliceParseOptions Options,
    System.Collections.Generic.IReadOnlyList<JsResolvedRequest>? Requests = null);

/// <summary>One specifier a module names, and the key the composition resolved it to.</summary>
/// <param name="Specifier">The specifier as the source wrote it, unchanged.</param>
/// <param name="Key">The key of the module it names.</param>
/// <remarks>
/// <b>Both halves are carried into the artifact and neither is derivable from the other here.</b>
/// The specifier is what the composition is later asked to rule on, and the key is what the module
/// records are matched by; a producer that carried only the key would leave the running host unable
/// to ask whether the resolution was its own, and one that carried only the specifier would be
/// asking this component to resolve it.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
// Broiler-Human:        PENDING
public sealed record JsResolvedRequest(string Specifier, string Key);

/// <summary>What compiling a set of scripts produced.</summary>
/// <param name="Succeeded">Whether an artifact was produced.</param>
/// <param name="Artifact">The bytes, or <see langword="null"/> when the source was refused.</param>
/// <param name="Diagnostics">Every refusal, in source order.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=ED5B3C
// Broiler-Human:        PENDING
public sealed record JsCompilation(
    bool Succeeded,
    byte[]? Artifact,
    System.Collections.Generic.IReadOnlyList<SliceSourceDiagnostic> Diagnostics);

/// <summary>
/// The wide surface's lowering: a syntax tree in, one verifiable artifact out.
/// </summary>
/// <remarks>
/// <para>
/// <b>Several scripts, one artifact, one realm.</b> The conformance suite requires its harness
/// files to be evaluated as SEPARATE scripts in the test's realm - concatenating them into one
/// changes <c>this</c> inside a constructor, changes what <c>delete</c> does, and changes the
/// directive-prologue semantics some tests are entirely about. So this compiler takes a list of
/// scripts and produces one artifact with one code unit and one named entry point per script. The
/// host invokes them in order against a single instance, and the instance is the realm.
/// </para>
/// <para>
/// <b>Every binding is resolved statically.</b> A name reaches either a (depth, slot) pair in an
/// environment or a property of the global object, and the decision is made here rather than at
/// run time. Script-level <c>var</c> and function declarations are global properties, which is
/// what the specification says and what makes one script's declarations visible to the next.
/// </para>
/// <para>
/// <b>One declared deviation, stated where it is made.</b> Script-level <c>let</c> and
/// <c>const</c> also become global properties rather than bindings of a separate global lexical
/// environment. The observable difference is that a read before the declaration answers
/// <c>undefined</c> instead of throwing, and that <c>globalThis.x</c> sees them. Nothing this
/// profile is built to run depends on either.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C4927F
// Broiler-Human:        PENDING
public sealed class JsCompiler
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D4AAEE
    // Broiler-Human:        PENDING
    private const int MaximumSlots = 60000;

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=35BE4C
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<SliceSourceDiagnostic> diagnostics = [];

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BFC91C
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<byte[]> constants = [];

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8CF226
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, ushort> constantIndex =
        new(System.StringComparer.Ordinal);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=90F5D0
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<UnitBuffer> units = [];

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=81178C
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<(string Name, uint Unit)> entries = [];

    /// <summary>The module records built so far, in declaration order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<ModuleBuild> built = [];

    /// <summary>
    /// Every import entry of the artifact, in the order the modules declare them.
    /// </summary>
    /// <remarks>
    /// <b>One table for the artifact rather than one per module</b>, because the index is what an
    /// import read carries and the executor must not have to know which module a code unit belongs
    /// to before it can read the operand.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<JsImportEntryRow> importEntries = [];

    /// <summary>The module being lowered, or <see langword="null"/> outside one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private ModuleBuild? module;

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=237FC7
    // Broiler-Human:        PENDING
    private UnitBuffer buffer = null!;

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5B1FB6
    // Broiler-Human:        PENDING
    private Scope scope = null!;

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=78C4BD
    // Broiler-Human:        PENDING
    private int blockDepth;

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=989AE3
    // Broiler-Human:        PENDING
    private bool strict;

    /// <summary>Compiles one source text as a script called <c>main</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D206EC
    // Broiler-Human:        PENDING
    public static JsCompilation Compile(string source, SliceParseOptions options) =>
        Compile([new JsScriptUnit("main", source, options)]);

    /// <summary>Compiles several source texts into one artifact, one entry point each.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BFE933
    // Broiler-Human:        PENDING
    public static JsCompilation Compile(System.Collections.Generic.IReadOnlyList<JsScriptUnit> scripts)
    {
        var compiler = new JsCompiler();
        return compiler.Run(scripts, []);
    }

    /// <summary>What one source text requests, so a composition can resolve and load it.</summary>
    /// <param name="Succeeded">Whether the source parsed.</param>
    /// <param name="Specifiers">Every module specifier the source names, in source order.</param>
    /// <param name="Diagnostics">Every refusal, when it did not.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    public sealed record JsModuleRequests(
        bool Succeeded,
        System.Collections.Generic.IReadOnlyList<string> Specifiers,
        System.Collections.Generic.IReadOnlyList<SliceSourceDiagnostic> Diagnostics);

    /// <summary>
    /// Answers what one module requests, without lowering it and without loading anything.
    /// </summary>
    /// <remarks>
    /// <b>This is the seam a composition walks a module graph through.</b> Following a specifier is
    /// the host's act - it is the thing that touches a filesystem, a URL scheme or a table - and
    /// this component neither performs it nor knows how it is performed. What the front end can
    /// answer, and the host cannot without a parser, is which specifiers a source names; so it
    /// answers that and stops.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    public static JsModuleRequests Requests(string text, SliceParseOptions options)
    {
        var compiler = new JsCompiler();

        if (!compiler.TryParse(text, options, forceStrict: true, out var program))
        {
            return new JsModuleRequests(false, [], compiler.diagnostics);
        }

        var specifiers = new System.Collections.Generic.List<string>();

        foreach (var statement in program.Body)
        {
            var specifier = statement switch
            {
                JsImportDeclaration import => import.Specifier,
                JsExportDeclaration exported => exported.From,
                _ => string.Empty,
            };

            if (specifier.Length != 0 && !specifiers.Contains(specifier))
            {
                specifiers.Add(specifier);
            }
        }

        return new JsModuleRequests(true, specifiers, compiler.diagnostics);
    }

    /// <summary>The entry point a module artifact's root module is invoked by.</summary>
    /// <remarks>
    /// <b>One entry point for a graph, and not one per module.</b> A module is not something a host
    /// calls: it is linked and evaluated as part of a graph, and evaluating one out of order is
    /// exactly the thing the specification's post-order is for. So the artifact names the root and
    /// nothing else, and every other module is reached through a request.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    public const string ModuleEntry = "module";

    /// <summary>
    /// Compiles a module graph, with any scripts that share its realm, into one artifact.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The scripts come first and they are still scripts.</b> A conformance run evaluates
    /// <c>assert.js</c> and <c>sta.js</c> in the realm a module test runs in, and those are Script
    /// sources: they declare globals, they are not strict, and folding them into the module goal
    /// would change what they mean. So one artifact carries both, each under its own goal, and the
    /// host invokes them in order.
    /// </para>
    /// <para>
    /// <b>The first module is the root</b> and the rest are reached from it. A module the root
    /// cannot reach is still verified - it is in the artifact and its bytes must be sound - and is
    /// never evaluated, which is what the specification says of a module nothing requests.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    public static JsCompilation Compile(
        System.Collections.Generic.IReadOnlyList<JsScriptUnit> scripts,
        System.Collections.Generic.IReadOnlyList<JsModuleUnit> modules)
    {
        var compiler = new JsCompiler();
        return compiler.Run(scripts, modules);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E1BE6C
    // Broiler-Human:        PENDING
    private JsCompilation Run(
        System.Collections.Generic.IReadOnlyList<JsScriptUnit> scripts,
        System.Collections.Generic.IReadOnlyList<JsModuleUnit> modules)
    {
        foreach (var script in scripts)
        {
            if (!TryParse(script.Text, script.Options, script.ForceStrict, out var program))
            {
                return new JsCompilation(false, null, diagnostics);
            }

            var unit = CompileProgram(program, script.ForceStrict);

            if (diagnostics.Count != 0)
            {
                return new JsCompilation(false, null, diagnostics);
            }

            entries.Add((script.Name, (uint)unit));
        }

        for (var index = 0; index < modules.Count; index++)
        {
            var module = modules[index];

            if (module.Options.Goal != SliceGoal.Module)
            {
                Refuse(
                    default,
                    SliceSourceDiagnosticCode.ModuleDeclarationOutsideModuleGoal,
                    "the module `" + module.Key + "` was presented under the script goal");

                return new JsCompilation(false, null, diagnostics);
            }

            if (!TryParse(module.Text, module.Options, forceStrict: true, out var program))
            {
                return new JsCompilation(false, null, diagnostics);
            }

            CompileModule(program, module);

            if (diagnostics.Count != 0)
            {
                return new JsCompilation(false, null, diagnostics);
            }
        }

        if (built.Count != 0)
        {
            entries.Add((ModuleEntry, (uint)built[0].BodyUnit));
        }

        return new JsCompilation(true, Assemble(), diagnostics);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private bool TryParse(
        string text, SliceParseOptions options, bool forceStrict, out JsProgramNode program)
    {
        program = null!;
        var tokenizer = new SliceTokenizer(text);
        var tokens = tokenizer.Tokenize();

        if (tokenizer.Diagnostics.Count != 0)
        {
            diagnostics.AddRange(tokenizer.Diagnostics);
            return false;
        }

        var parser = new JsParser(tokens, options, forceStrict);
        program = parser.Parse();

        if (parser.Diagnostics.Count == 0)
        {
            return true;
        }

        diagnostics.AddRange(parser.Diagnostics);
        return false;
    }

    // ---- assembly ------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=EEE3A5
    // Broiler-Human:        PENDING
    private byte[] Assemble()
    {
        // THE MODULE ROWS ARE BUILT BEFORE THE POOL IS SNAPSHOTTED, and the order is not
        // cosmetic. A row names its module's key and every specifier it requests as CONSTANTS, so
        // building the rows interns names; doing it after the constant section had been encoded
        // left every one of those names past the end of the pool the artifact carries, and the
        // verifier refused the first module row of every module artifact this host produced.
        var moduleRows = built.Count == 0 ? [] : ModuleRows();

        var code = new System.Collections.Generic.List<byte>();
        var rows = new System.Collections.Generic.List<JsFunctionRow>();
        var regions = new System.Collections.Generic.List<JsExceptionRegionRow>();
        var positions = new System.Collections.Generic.List<(uint Offset, uint Line, uint Column)>();
        var bases = new uint[units.Count];
        var maximumStack = 1u;
        var maximumSlots = 1u;

        for (var index = 0; index < units.Count; index++)
        {
            bases[index] = (uint)code.Count;
            var unit = units[index];
            unit.FinishScopes();

            foreach (var site in unit.BranchSites)
            {
                var target = (uint)(
                    unit.Code[site] |
                    (unit.Code[site + 1] << 8) |
                    (unit.Code[site + 2] << 16) |
                    (unit.Code[site + 3] << 24)) + bases[index];

                for (var shift = 0; shift < 32; shift += 8)
                {
                    unit.Code[site + (shift / 8)] = (byte)((target >> shift) & 0xFF);
                }
            }

            code.AddRange(unit.Code);
            maximumStack = System.Math.Max(maximumStack, (uint)unit.MaximumStack);
            maximumSlots = System.Math.Max(maximumSlots, (uint)unit.SlotCount);

            rows.Add(new JsFunctionRow(
                unit.NameConstant,
                (uint)unit.ParameterCount,
                (uint)unit.SlotCount,
                (uint)unit.MaximumStack,
                bases[index],
                (uint)unit.Code.Count,
                (uint)unit.Flags));
        }

        for (var index = 0; index < units.Count; index++)
        {
            var unit = units[index];

            foreach (var region in unit.Regions)
            {
                regions.Add(new JsExceptionRegionRow(
                    (uint)index,
                    region.TryStart + bases[index],
                    region.TryEnd + bases[index],
                    region.Handler + bases[index],
                    region.ScopeDepth,
                    0,
                    region.Kind));
            }

            foreach (var (offset, line, column) in unit.Positions)
            {
                positions.Add((offset + bases[index], line, column));
            }
        }

        positions.Sort(static (left, right) => left.Offset.CompareTo(right.Offset));

        var sections = new System.Collections.Generic.List<JavaScriptArtifactWriter.Section>
        {
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Limits,
                JsArtifactWriter.Limits(
                    maximumStack, maximumSlots, (uint)units.Count, (uint)constants.Count)),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Constants,
                JsArtifactWriter.Constants(constants.ToArray())),
            new((JavaScriptFormat.SectionKind)JsFormat.SectionKind.Code, code.ToArray()),
            new(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Entries,
                JsArtifactWriter.Entries(entries.ToArray())),
        };

        if (regions.Count != 0)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.ExceptionRegions,
                JsArtifactWriter.ExceptionRegions(regions.ToArray())));
        }

        sections.Add(new JavaScriptArtifactWriter.Section(
            (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Positions,
            JsArtifactWriter.Positions(positions.ToArray())));

        sections.Add(new JavaScriptArtifactWriter.Section(
            (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Functions,
            JsArtifactWriter.Functions(rows.ToArray())));

        if (built.Count == 0)
        {
            return JsArtifactWriter.Write(JsFormat.ManifestId, sections.ToArray());
        }

        sections.Add(new JavaScriptArtifactWriter.Section(
            (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Modules,
            JsArtifactWriter.Modules(moduleRows)));

        // THE MANIFEST FOLLOWS THE SECTION AND NOT AN OPTION SOMEBODY PASSED. An artifact carrying
        // module records is a module artifact, and one that named the wide manifest while carrying
        // them would be asking a composition that declined the module surface to run modules.
        return JsArtifactWriter.Write(JsFormat.ModulesManifestId, sections.ToArray());
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private JsModuleRow[] ModuleRows()
    {
        var rows = new JsModuleRow[built.Count];

        for (var index = 0; index < built.Count; index++)
        {
            var build = built[index];
            var specifiers = new uint[build.Requests.Count];
            var requests = new uint[build.Requests.Count];

            for (var request = 0; request < requests.Length; request++)
            {
                specifiers[request] = InternedName(build.Requests[request]);
                requests[request] = InternedName(build.RequestKeys[request]);
            }

            rows[index] = new JsModuleRow(
                InternedName(build.Key),
                (uint)build.BodyUnit,
                (uint)build.InitialiserUnit,
                specifiers,
                requests,
                build.ImportRows.ToArray(),
                build.LocalExports.ToArray(),
                build.IndirectExports.ToArray(),
                build.StarExports.ToArray());
        }

        return rows;
    }

    // ---- units ---------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=567568
    // Broiler-Human:        PENDING
    private int CompileProgram(JsProgramNode program, bool forceStrict)
    {
        var outerBuffer = buffer;
        var outerScope = scope;
        var outerDepth = blockDepth;
        var outerStrict = strict;

        strict = program.IsStrict || forceStrict;
        var index = units.Count;
        buffer = new UnitBuffer(0, JsFormat.FunctionFlags.ProgramBody | Strictness());
        units.Add(buffer);
        scope = new Scope(ScopeKind.Program, null);
        blockDepth = 0;

        // SLOT ZERO OF A SCRIPT IS ITS COMPLETION VALUE. A script's value is the value of its last
        // value-producing statement, which is what a person running one file at a prompt expects to
        // see and what the host prints. Keeping it in a slot rather than on the operand stack is
        // what lets every statement leave the stack empty.
        var completion = scope.Declare("#completion", constant: false);

        Emit(JsOpcode.LoadUndefined);
        EmitScoped(JsOpcode.InitialiseScoped, 0, completion);

        HoistProgram(program.Body);
        CompileStatements(program.Body, completion);

        EmitScoped(JsOpcode.LoadScoped, 0, completion);
        Emit(JsOpcode.Return);

        buffer.SlotCount = scope.SlotCount;
        buffer = outerBuffer;
        scope = outerScope;
        blockDepth = outerDepth;
        strict = outerStrict;
        return index;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=88C57E
    // Broiler-Human:        PENDING
    private int CompileFunction(JsFunctionNode function)
    {
        var outerBuffer = buffer;
        var outerScope = scope;
        var outerDepth = blockDepth;
        var outerStrict = strict;

        strict = strict || function.IsStrict;

        var flags = Strictness() | JsFormat.FunctionFlags.Constructible;

        if (function.IsArrow)
        {
            flags = (flags & ~JsFormat.FunctionFlags.Constructible) | JsFormat.FunctionFlags.Arrow;
        }

        var index = units.Count;
        var name = function.Name.Length == 0 ? (ushort)0 : (ushort)(InternedName(function.Name) + 1);
        buffer = new UnitBuffer(name, flags) { ParameterCount = function.Parameters.Count };
        units.Add(buffer);
        scope = new Scope(ScopeKind.Function, outerScope);
        blockDepth = 0;

        foreach (var parameter in function.Parameters)
        {
            scope.Declare(parameter, constant: false);
        }

        var usesArguments = !function.IsArrow && UsesArguments(function.Body);

        if (usesArguments)
        {
            buffer.Flags |= JsFormat.FunctionFlags.UsesArguments;
            var slot = scope.Declare("arguments", constant: false);
            Emit(JsOpcode.NewArguments);
            EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
        }

        HoistFunction(function.Body);
        CompileStatements(function.Body, -1);
        Emit(JsOpcode.ReturnUndefined);

        buffer.SlotCount = scope.SlotCount;

        if (scope.SlotCount > MaximumSlots)
        {
            Refuse(function.Span, SliceSourceDiagnosticCode.TooManyLocals, "a function declares too many bindings");
        }

        buffer = outerBuffer;
        scope = outerScope;
        blockDepth = outerDepth;
        strict = outerStrict;
        return index;
    }

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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
                    var request = RequestIndex(build, import.Specifier);

                    foreach (var specifier in import.Specifiers)
                    {
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
                    RequestIndex(build, exported.From);
                    break;

                default:
                    break;
            }
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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

            Declared(build, function.Name, constant: false);
        }

        var declaredLexically = new System.Collections.Generic.HashSet<string>(
            declaredFunctions, System.StringComparer.Ordinal);

        foreach (var statement in Unwrapped(body))
        {
            if (statement is not JsVariableStatement variable ||
                variable.Kind == SliceDeclarationKind.Var)
            {
                continue;
            }

            foreach (var declarator in variable.Declarators)
            {
                if (!declaredLexically.Add(declarator.Name))
                {
                    Refuse(
                        declarator.Span,
                        SliceSourceDiagnosticCode.DuplicateLexicalDeclaration,
                        "`" + declarator.Name + "` is declared twice at this module's top level");
                }

                if (names.Contains(declarator.Name))
                {
                    Refuse(
                        declarator.Span,
                        SliceSourceDiagnosticCode.VarAndLexicalCollision,
                        "`" + declarator.Name + "` is declared both as a `var` and lexically");
                }

                Declared(
                    build, declarator.Name, variable.Kind == SliceDeclarationKind.Const);
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
                build.StarExports.Add(RequestIndex(build, exported.From));
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
                    var request = RequestIndex(build, exported.From);

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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private int EmitModuleBody(JsProgramNode program, ModuleBuild build)
    {
        var index = units.Count;
        buffer = new UnitBuffer(0, JsFormat.FunctionFlags.ProgramBody | JsFormat.FunctionFlags.Strict);
        units.Add(buffer);

        var completion = build.Scope.Declare("#completion", constant: false);
        Emit(JsOpcode.LoadUndefined);
        EmitScoped(JsOpcode.InitialiseScoped, 0, completion);

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
                    CompileExpression(
                        value is JsFunctionExpression { Function.Name.Length: 0 } anonymous
                            ? anonymous with
                            {
                                Function = anonymous.Function with { Name = "default" },
                            }
                            : value);

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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3FFD44
    // Broiler-Human:        PENDING
    private JsFormat.FunctionFlags Strictness() =>
        strict ? JsFormat.FunctionFlags.Strict : JsFormat.FunctionFlags.None;

    // ---- hoisting ------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A1AF9D
    // Broiler-Human:        PENDING
    private void HoistProgram(System.Collections.Generic.IReadOnlyList<JsStatement> body)
    {
        var names = new System.Collections.Generic.List<string>();
        var functions = new System.Collections.Generic.List<JsFunctionNode>();
        CollectVarScope(body, names, functions, lexical: null);

        foreach (var name in names)
        {
            Emit(JsOpcode.DeclareGlobal, InternedName(name));
        }

        foreach (var function in functions)
        {
            Emit(JsOpcode.Closure, (ushort)CompileFunction(function));
            Emit(JsOpcode.StoreGlobal, InternedName(function.Name));
        }

        // Script-level `let` and `const` are global properties here; the class remark records the
        // deviation. Declaring them up front is what makes a later script see them.
        var lexicalNames = new System.Collections.Generic.List<string>();
        CollectLexical(body, lexicalNames);

        foreach (var name in lexicalNames)
        {
            Emit(JsOpcode.DeclareGlobal, InternedName(name));
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=522F83
    // Broiler-Human:        PENDING
    private void HoistFunction(System.Collections.Generic.IReadOnlyList<JsStatement> body)
    {
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
            Emit(JsOpcode.LoadUndefined);
            EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
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
    }

    /// <summary>
    /// Collects the <c>var</c> names and function declarations of one hoisting scope.
    /// </summary>
    /// <remarks>
    /// It descends through blocks, loops and <c>try</c> - a <c>var</c> anywhere inside a function
    /// belongs to that function - and stops at a nested function, which is a hoisting scope of its
    /// own. A collector that stopped at a block would answer that <c>if (x) { var y; }</c> declares
    /// nothing.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=276041
    // Broiler-Human:        PENDING
    private static void CollectVarScope(
        System.Collections.Generic.IReadOnlyList<JsStatement> body,
        System.Collections.Generic.List<string> names,
        System.Collections.Generic.List<JsFunctionNode> functions,
        System.Collections.Generic.List<string>? lexical)
    {
        foreach (var statement in body)
        {
            switch (statement)
            {
                case JsVariableStatement variable when variable.Kind == SliceDeclarationKind.Var:
                    foreach (var declarator in variable.Declarators)
                    {
                        names.Add(declarator.Name);
                    }

                    break;

                case JsVariableStatement variable when lexical is not null:
                    foreach (var declarator in variable.Declarators)
                    {
                        lexical.Add(declarator.Name);
                    }

                    break;

                case JsFunctionDeclaration declaration:
                    functions.Add(declaration.Function);
                    break;

                case JsBlockStatement block:
                    CollectVarScope(block.Body, names, functions, null);
                    break;

                case JsIfStatement conditional:
                    CollectVarScope([conditional.Consequent], names, functions, null);

                    if (conditional.Alternate is not null)
                    {
                        CollectVarScope([conditional.Alternate], names, functions, null);
                    }

                    break;

                case JsWhileStatement loop:
                    CollectVarScope([loop.Body], names, functions, null);
                    break;

                case JsDoWhileStatement loop:
                    CollectVarScope([loop.Body], names, functions, null);
                    break;

                case JsForStatement loop:
                    if (loop.Initialiser is not null)
                    {
                        CollectVarScope([loop.Initialiser], names, functions, null);
                    }

                    CollectVarScope([loop.Body], names, functions, null);
                    break;

                case JsForInStatement loop:
                    if (loop.Declaration == SliceDeclarationKind.Var)
                    {
                        names.Add(loop.Name);
                    }

                    CollectVarScope([loop.Body], names, functions, null);
                    break;

                case JsTryStatement guarded:
                    CollectVarScope(guarded.Block.Body, names, functions, null);

                    if (guarded.Handler is not null)
                    {
                        CollectVarScope(guarded.Handler.Body, names, functions, null);
                    }

                    if (guarded.Finaliser is not null)
                    {
                        CollectVarScope(guarded.Finaliser.Body, names, functions, null);
                    }

                    break;

                case JsSwitchStatement switched:
                    foreach (var clause in switched.Clauses)
                    {
                        CollectVarScope(clause.Body, names, functions, null);
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=789E2D
    // Broiler-Human:        PENDING
    private static void CollectLexical(
        System.Collections.Generic.IReadOnlyList<JsStatement> body,
        System.Collections.Generic.List<string> names)
    {
        foreach (var statement in body)
        {
            if (statement is JsVariableStatement variable && variable.Kind != SliceDeclarationKind.Var)
            {
                foreach (var declarator in variable.Declarators)
                {
                    names.Add(declarator.Name);
                }
            }
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=755214
    // Broiler-Human:        PENDING
    private static bool UsesArguments(System.Collections.Generic.IReadOnlyList<JsStatement> body)
    {
        foreach (var statement in body)
        {
            if (Walk.Mentions(statement, "arguments"))
            {
                return true;
            }
        }

        return false;
    }

    // ---- statements ----------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F73A6A
    // Broiler-Human:        PENDING
    private void CompileStatements(
        System.Collections.Generic.IReadOnlyList<JsStatement> body, int completion)
    {
        foreach (var statement in body)
        {
            CompileStatement(statement, completion);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=2B58B6
    // Broiler-Human:        PENDING
    private void CompileStatement(JsStatement statement, int completion)
    {
        Position(statement.Span);

        switch (statement)
        {
            case JsEmptyStatement:
            case JsDebuggerStatement:
                break;

            case JsExpressionStatement expression:
                CompileExpression(expression.Expression);

                if (completion >= 0)
                {
                    EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, completion);
                }
                else
                {
                    Emit(JsOpcode.Pop);
                }

                break;

            case JsVariableStatement variable:
                CompileVariable(variable);
                break;

            case JsFunctionDeclaration:
                break;

            case JsBlockStatement block:
                CompileBlock(block, completion);
                break;

            case JsIfStatement conditional:
                CompileIf(conditional, completion);
                break;

            case JsWhileStatement loop:
                CompileWhile(loop, completion, string.Empty);
                break;

            case JsDoWhileStatement loop:
                CompileDoWhile(loop, completion, string.Empty);
                break;

            case JsForStatement loop:
                CompileFor(loop, completion, string.Empty);
                break;

            case JsForInStatement loop:
                CompileForIn(loop, completion, string.Empty);
                break;

            case JsBreakStatement jump:
                CompileJumpOut(jump.Span, jump.Label, wantsContinue: false);
                break;

            case JsContinueStatement jump:
                CompileJumpOut(jump.Span, jump.Label, wantsContinue: true);
                break;

            case JsReturnStatement returned:
                CompileReturn(returned);
                break;

            case JsThrowStatement thrown:
                CompileExpression(thrown.Value);
                Emit(JsOpcode.Throw);
                break;

            case JsTryStatement guarded:
                CompileTry(guarded, completion);
                break;

            case JsSwitchStatement switched:
                CompileSwitch(switched, completion, string.Empty);
                break;

            case JsLabelledStatement labelled:
                CompileLabelled(labelled, completion);
                break;

            default:
                Refuse(
                    statement.Span,
                    SliceSourceDiagnosticCode.ConstructOutsideManifest,
                    "this statement is not admitted by the declared feature manifest");

                break;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CC65D0
    // Broiler-Human:        PENDING
    private void CompileLabelled(JsLabelledStatement labelled, int completion)
    {
        switch (labelled.Body)
        {
            case JsWhileStatement loop:
                CompileWhile(loop, completion, labelled.Label);
                return;

            case JsDoWhileStatement loop:
                CompileDoWhile(loop, completion, labelled.Label);
                return;

            case JsForStatement loop:
                CompileFor(loop, completion, labelled.Label);
                return;

            case JsForInStatement loop:
                CompileForIn(loop, completion, labelled.Label);
                return;

            case JsSwitchStatement switched:
                CompileSwitch(switched, completion, labelled.Label);
                return;

            default:
            {
                var exit = new Exit(ExitKind.Label, labelled.Label, blockDepth)
                {
                    Break = NewLabel(),
                };

                exits.Add(exit);
                CompileStatement(labelled.Body, completion);
                exits.RemoveAt(exits.Count - 1);
                Mark(exit.Break!);
                return;
            }
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=085DC1
    // Broiler-Human:        PENDING
    private void CompileVariable(JsVariableStatement variable)
    {
        foreach (var declarator in variable.Declarators)
        {
            if (variable.Kind == SliceDeclarationKind.Const && declarator.Initialiser is null)
            {
                Refuse(
                    declarator.Span,
                    SliceSourceDiagnosticCode.ConstWithoutInitialiser,
                    "`const " + declarator.Name + "` needs an initialiser");

                continue;
            }

            // A MODULE'S TOP LEVEL IS NOT A SCRIPT'S. The branch below writes a global, which is
            // right for a script and wrong for a module: a module's declarations are slots of its
            // own environment, so lowering falls through to the slot paths beneath.
            if (scope.Kind == ScopeKind.Program && blockDepth == 0)
            {
                if (declarator.Initialiser is null)
                {
                    continue;
                }

                CompileExpression(declarator.Initialiser);
                Emit(JsOpcode.StoreGlobal, InternedName(declarator.Name));
                continue;
            }

            // A `var` NAMES A BINDING THAT ALREADY EXISTS. Hoisting created it in the
            // enclosing function or program scope, so a declaration inside a block writes THAT
            // one; declaring it again here would create a second binding the rest of the function
            // cannot see.
            if (variable.Kind == SliceDeclarationKind.Var)
            {
                if (declarator.Initialiser is null)
                {
                    continue;
                }

                CompileExpression(declarator.Initialiser);
                StoreName(declarator.Span, declarator.Name);
                Emit(JsOpcode.Pop);
                continue;
            }

            var slot = scope.Has(declarator.Name)
                ? scope.SlotOf(declarator.Name)
                : scope.Declare(declarator.Name, variable.Kind == SliceDeclarationKind.Const);

            if (declarator.Initialiser is null)
            {
                Emit(JsOpcode.LoadUndefined);
            }
            else
            {
                CompileExpression(declarator.Initialiser);
            }

            EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=92A3B0
    // Broiler-Human:        PENDING
    private void CompileBlock(JsBlockStatement block, int completion)
    {
        var lexical = new System.Collections.Generic.List<string>();
        CollectLexical(block.Body, lexical);
        var pushed = lexical.Count != 0;
        var outer = scope;

        if (pushed)
        {
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var at = buffer.Code.Count;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((at + 1, scope));
        }

        CompileStatements(block.Body, completion);

        if (pushed)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = outer;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=DF30EA
    // Broiler-Human:        PENDING
    private void CompileIf(JsIfStatement conditional, int completion)
    {
        CompileExpression(conditional.Test);
        var otherwise = NewLabel();
        Branch(JsOpcode.JumpIfFalse, otherwise);
        CompileStatement(conditional.Consequent, completion);

        if (conditional.Alternate is null)
        {
            Mark(otherwise);
            return;
        }

        var end = NewLabel();
        Branch(JsOpcode.Jump, end);
        Mark(otherwise);
        CompileStatement(conditional.Alternate, completion);
        Mark(end);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6504CE
    // Broiler-Human:        PENDING
    private void CompileWhile(JsWhileStatement loop, int completion, string label)
    {
        var top = NewLabel();
        var exit = new Exit(ExitKind.Loop, label, blockDepth)
        {
            Break = NewLabel(),
            Continue = top,
        };

        Mark(top);
        CompileExpression(loop.Test);
        Branch(JsOpcode.JumpIfFalse, exit.Break!);
        exits.Add(exit);
        CompileStatement(loop.Body, completion);
        exits.RemoveAt(exits.Count - 1);
        Branch(JsOpcode.Jump, top);
        Mark(exit.Break!);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=761801
    // Broiler-Human:        PENDING
    private void CompileDoWhile(JsDoWhileStatement loop, int completion, string label)
    {
        var top = NewLabel();
        var exit = new Exit(ExitKind.Loop, label, blockDepth)
        {
            Break = NewLabel(),
            Continue = NewLabel(),
        };

        Mark(top);
        exits.Add(exit);
        CompileStatement(loop.Body, completion);
        exits.RemoveAt(exits.Count - 1);
        Mark(exit.Continue!);
        CompileExpression(loop.Test);
        Branch(JsOpcode.JumpIfTrue, top);
        Mark(exit.Break!);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C10AE9
    // Broiler-Human:        PENDING
    private void CompileFor(JsForStatement loop, int completion, string label)
    {
        var lexical = new System.Collections.Generic.List<string>();

        if (loop.Initialiser is JsVariableStatement head && head.Kind != SliceDeclarationKind.Var)
        {
            foreach (var declarator in head.Declarators)
            {
                lexical.Add(declarator.Name);
            }
        }

        var outer = scope;
        var pushed = lexical.Count != 0;

        if (pushed)
        {
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var headSite = buffer.Code.Count + 1;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((headSite, scope));
        }

        if (loop.Initialiser is JsVariableStatement declaration)
        {
            CompileVariable(declaration);
        }
        else if (loop.Initialiser is JsExpressionStatement expression)
        {
            CompileExpression(expression.Expression);
            Emit(JsOpcode.Pop);
        }

        // EACH TURN OF THE LOOP GETS ITS OWN BINDING, and WHERE the copy is made is the whole of
        // whether that works. The specification copies the environment once before the loop and
        // then again after each body and BEFORE the increment: a closure the body created keeps the
        // value the body saw, and the increment lands in the copy the next turn will use. Copying
        // before the body instead - the obvious placement - makes every closure see the value after
        // its own increment, which is the classic `for (let i …)` defect with the sign flipped.
        if (pushed)
        {
            var firstSite = buffer.Code.Count + 1;
            Emit(JsOpcode.CopyScope, (ushort)0);
            buffer.ScopeSites.Add((firstSite, scope));
        }

        var top = NewLabel();
        var exit = new Exit(ExitKind.Loop, label, blockDepth)
        {
            Break = NewLabel(),
            Continue = NewLabel(),
        };

        Mark(top);

        if (loop.Test is not null)
        {
            CompileExpression(loop.Test);
            Branch(JsOpcode.JumpIfFalse, exit.Break!);
        }

        exits.Add(exit);
        CompileStatement(loop.Body, completion);
        exits.RemoveAt(exits.Count - 1);
        Mark(exit.Continue!);

        if (pushed)
        {
            var copySite = buffer.Code.Count + 1;
            Emit(JsOpcode.CopyScope, (ushort)0);
            buffer.ScopeSites.Add((copySite, scope));
        }

        if (loop.Update is not null)
        {
            CompileExpression(loop.Update);
            Emit(JsOpcode.Pop);
        }

        Branch(JsOpcode.Jump, top);
        Mark(exit.Break!);

        if (pushed)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = outer;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=40F438
    // Broiler-Human:        PENDING
    private void CompileForIn(JsForInStatement loop, int completion, string label)
    {
        // THE ENUMERATOR LIVES IN A SLOT, NOT ON THE OPERAND STACK. Every abrupt exit from the body
        // - `break`, `continue` to an outer loop, `return` - would otherwise have to know how many
        // operand-stack entries the enclosing loops are holding and pop exactly that many. One slot
        // costs an environment entry and removes the whole class of defect.
        var function = FunctionScope();
        var enumerator = function.Declare("#forin" + function.SlotCount, constant: false);
        CompileExpression(loop.Right);
        Emit(JsOpcode.ForInStart);
        EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, enumerator);

        var outer = scope;
        var pushed = loop.Declaration is SliceDeclarationKind.Let or SliceDeclarationKind.Const;

        if (pushed)
        {
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var headSite = buffer.Code.Count + 1;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((headSite, scope));
            scope.Declare(loop.Name, loop.Declaration == SliceDeclarationKind.Const);
        }

        var top = NewLabel();
        var exit = new Exit(ExitKind.Loop, label, blockDepth)
        {
            Break = NewLabel(),
            Continue = NewLabel(),
        };

        Mark(top);
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, enumerator);
        Branch(JsOpcode.ForInNext, exit.Break!);

        if (pushed)
        {
            EmitScoped(JsOpcode.InitialiseScoped, 0, scope.SlotOf(loop.Name));
        }
        else if (loop.Declaration == SliceDeclarationKind.Var)
        {
            StoreName(loop.Span, loop.Name);
            Emit(JsOpcode.Pop);
        }
        else if (loop.Target is not null)
        {
            CompileStoreTo(loop.Target);
            Emit(JsOpcode.Pop);
        }
        else
        {
            Emit(JsOpcode.Pop);
        }

        exits.Add(exit);
        CompileStatement(loop.Body, completion);
        exits.RemoveAt(exits.Count - 1);
        Mark(exit.Continue!);
        Branch(JsOpcode.Jump, top);
        Mark(exit.Break!);

        if (pushed)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = outer;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=0E810C
    // Broiler-Human:        PENDING
    private void CompileSwitch(JsSwitchStatement switched, int completion, string label)
    {
        var outer = scope;
        scope = new Scope(ScopeKind.Block, outer);
        blockDepth++;
        var scopeSite = buffer.Code.Count + 1;
        Emit(JsOpcode.PushScope, (ushort)0);
        buffer.ScopeSites.Add((scopeSite, scope));

        var discriminant = scope.Declare("#switch", constant: false);
        CompileExpression(switched.Discriminant);
        EmitScoped(JsOpcode.InitialiseScoped, 0, discriminant);

        var exit = new Exit(ExitKind.Switch, label, blockDepth)
        {
            Break = NewLabel(),
        };

        var bodies = new Label[switched.Clauses.Count];

        for (var index = 0; index < bodies.Length; index++)
        {
            bodies[index] = NewLabel();
        }

        var defaultAt = -1;

        for (var index = 0; index < switched.Clauses.Count; index++)
        {
            var clause = switched.Clauses[index];

            if (clause.Test is null)
            {
                defaultAt = index;
                continue;
            }

            EmitScoped(JsOpcode.LoadScoped, 0, discriminant);
            CompileExpression(clause.Test);
            Emit(JsOpcode.StrictEquals);
            Branch(JsOpcode.JumpIfTrue, bodies[index]);
        }

        Branch(JsOpcode.Jump, defaultAt >= 0 ? bodies[defaultAt] : exit.Break!);
        exits.Add(exit);

        for (var index = 0; index < switched.Clauses.Count; index++)
        {
            Mark(bodies[index]);
            CompileStatements(switched.Clauses[index].Body, completion);
        }

        exits.RemoveAt(exits.Count - 1);
        Branch(JsOpcode.Jump, exit.Break!);
        Mark(exit.Break!);
        Emit(JsOpcode.PopScope);
        blockDepth--;
        scope = outer;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=7DF5E3
    // Broiler-Human:        PENDING
    private void CompileTry(JsTryStatement guarded, int completion)
    {
        var end = NewLabel();
        var hasFinally = guarded.Finaliser is not null;

        if (hasFinally)
        {
            exits.Add(new Exit(ExitKind.Finally, string.Empty, blockDepth)
            {
                Finaliser = guarded.Finaliser,
            });
        }

        var tryStart = buffer.Code.Count;

        if (guarded.Handler is not null)
        {
            var catchHandler = NewLabel();
            var afterCatch = NewLabel();
            buffer.PendingRegions.Add(
                new PendingRegion(tryStart, catchHandler, blockDepth, JsFormat.HandlerKind.Catch));

            CompileBlock(guarded.Block, completion);
            buffer.CloseRegion(tryStart, buffer.Code.Count);
            Branch(JsOpcode.Jump, afterCatch);
            Mark(catchHandler);

            var outer = scope;
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var scopeSite = buffer.Code.Count + 1;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((scopeSite, scope));

            var parameter = guarded.CatchParameter.Length == 0
                ? scope.Declare("#caught", constant: false)
                : scope.Declare(guarded.CatchParameter, constant: false);

            EmitScoped(JsOpcode.InitialiseScoped, 0, parameter);
            CompileStatements(guarded.Handler.Body, completion);
            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = outer;
            Mark(afterCatch);
        }
        else
        {
            CompileBlock(guarded.Block, completion);
        }

        if (!hasFinally)
        {
            Mark(end);
            return;
        }

        var finallyStart = tryStart;
        var rethrow = NewLabel();
        buffer.PendingRegions.Add(
            new PendingRegion(finallyStart, rethrow, blockDepth, JsFormat.HandlerKind.Finally));

        buffer.CloseRegion(finallyStart, buffer.Code.Count);
        exits.RemoveAt(exits.Count - 1);

        // The normal path runs the finaliser inline and continues.
        CompileBlock(guarded.Finaliser!, completion);
        Branch(JsOpcode.Jump, end);

        // The exceptional path stores the thrown value, runs the same block, and rethrows it.
        Mark(rethrow);
        var handlerOuter = scope;
        scope = new Scope(ScopeKind.Block, handlerOuter);
        blockDepth++;
        var site = buffer.Code.Count + 1;
        Emit(JsOpcode.PushScope, (ushort)0);
        buffer.ScopeSites.Add((site, scope));
        var pending = scope.Declare("#pending", constant: false);
        EmitScoped(JsOpcode.InitialiseScoped, 0, pending);
        CompileBlock(guarded.Finaliser!, completion);
        EmitScoped(JsOpcode.LoadScoped, 0, pending);
        Emit(JsOpcode.PopScope);
        blockDepth--;
        scope = handlerOuter;
        Emit(JsOpcode.Throw);
        Mark(end);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=985B7A
    // Broiler-Human:        PENDING
    private void CompileReturn(JsReturnStatement returned)
    {
        if (scope.Kind == ScopeKind.Program)
        {
            Refuse(
                returned.Span,
                SliceSourceDiagnosticCode.ConstructOutsideManifest,
                "`return` outside a function is not admitted");

            return;
        }

        var finallys = new System.Collections.Generic.List<Exit>();

        for (var index = exits.Count - 1; index >= 0; index--)
        {
            if (exits[index].Kind == ExitKind.Finally && !exits[index].Running)
            {
                finallys.Add(exits[index]);
            }
        }

        if (finallys.Count == 0)
        {
            if (returned.Value is null)
            {
                Emit(JsOpcode.ReturnUndefined);
                return;
            }

            CompileExpression(returned.Value);
            Emit(JsOpcode.Return);
            return;
        }

        // A `return` inside a `try … finally` must run every enclosing finaliser before it leaves,
        // so the value is parked in a slot of the function's own environment while they run.
        var function = FunctionScope();
        var slot = function.Declare("#return" + function.SlotCount, constant: false);

        if (returned.Value is null)
        {
            Emit(JsOpcode.LoadUndefined);
        }
        else
        {
            CompileExpression(returned.Value);
        }

        EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, slot);
        var savedDepth = blockDepth;

        foreach (var finaliser in finallys)
        {
            while (blockDepth > finaliser.Depth)
            {
                Emit(JsOpcode.PopScope);
                blockDepth--;
            }

            finaliser.Running = true;
            CompileBlock(finaliser.Finaliser!, -1);
            finaliser.Running = false;
        }

        while (blockDepth > 0)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
        }

        EmitScoped(JsOpcode.LoadScoped, 0, slot);
        Emit(JsOpcode.Return);
        blockDepth = savedDepth;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=9F7D51
    // Broiler-Human:        PENDING
    private void CompileJumpOut(SliceSourceSpan span, string label, bool wantsContinue)
    {
        var targetAt = -1;

        for (var index = exits.Count - 1; index >= 0; index--)
        {
            var candidate = exits[index];

            if (candidate.Kind == ExitKind.Finally)
            {
                continue;
            }

            if (label.Length != 0)
            {
                if (!string.Equals(candidate.Label, label, System.StringComparison.Ordinal))
                {
                    continue;
                }

                if (wantsContinue && candidate.Continue is null)
                {
                    continue;
                }

                targetAt = index;
                break;
            }

            if (wantsContinue)
            {
                if (candidate.Continue is null)
                {
                    continue;
                }

                targetAt = index;
                break;
            }

            if (candidate.Kind is ExitKind.Loop or ExitKind.Switch)
            {
                targetAt = index;
                break;
            }
        }

        if (targetAt < 0)
        {
            Refuse(
                span,
                wantsContinue
                    ? SliceSourceDiagnosticCode.IllegalContinue
                    : SliceSourceDiagnosticCode.IllegalBreak,
                wantsContinue
                    ? "`continue` names no enclosing loop"
                    : "`break` names no enclosing loop or switch");

            return;
        }

        var target = exits[targetAt];
        var savedDepth = blockDepth;

        for (var index = exits.Count - 1; index > targetAt; index--)
        {
            if (exits[index].Kind != ExitKind.Finally || exits[index].Running)
            {
                continue;
            }

            while (blockDepth > exits[index].Depth)
            {
                Emit(JsOpcode.PopScope);
                blockDepth--;
            }

            exits[index].Running = true;
            CompileBlock(exits[index].Finaliser!, -1);
            exits[index].Running = false;
        }

        var wanted = wantsContinue ? target.Depth : target.Depth;

        while (blockDepth > wanted)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
        }

        Branch(JsOpcode.Jump, wantsContinue ? target.Continue! : target.Break!);
        blockDepth = savedDepth;
    }

    // ---- expressions ---------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=469A26
    // Broiler-Human:        PENDING
    private void CompileExpression(JsExpression expression)
    {
        Position(expression.Span);

        switch (expression)
        {
            case JsNumberLiteral number:
                Emit(JsOpcode.LoadConstant, NumberConstant(number.Value));
                break;

            case JsStringLiteral text:
                Emit(JsOpcode.LoadConstant, StringConstant(text.Value));
                break;

            case JsBooleanLiteral boolean:
                Emit(boolean.Value ? JsOpcode.LoadTrue : JsOpcode.LoadFalse);
                break;

            case JsNullLiteral:
                Emit(JsOpcode.LoadNull);
                break;

            case JsThisExpression:
                Emit(JsOpcode.LoadThis);
                break;

            case JsIdentifier identifier:
                LoadName(identifier.Span, identifier.Name);
                break;

            case JsRegExpLiteral pattern:
                Emit(JsOpcode.LoadGlobal, InternedName("RegExp"));
                Emit(JsOpcode.LoadConstant, StringConstant(pattern.Pattern));
                Emit(JsOpcode.LoadConstant, StringConstant(pattern.Flags));
                Emit(JsOpcode.Construct, (byte)2);
                break;

            case JsArrayLiteral array:
                CompileArray(array);
                break;

            case JsObjectLiteral literal:
                CompileObject(literal);
                break;

            case JsFunctionExpression function:
                CompileFunctionExpression(function.Function);
                break;

            case JsUnaryExpression unary:
                CompileUnary(unary);
                break;

            case JsUpdateExpression update:
                CompileUpdate(update);
                break;

            case JsBinaryExpression binary:
                CompileExpression(binary.Left);
                CompileExpression(binary.Right);
                Emit(BinaryOpcode(binary.Operator));
                break;

            case JsLogicalExpression logical:
                CompileLogical(logical);
                break;

            case JsConditionalExpression conditional:
            {
                CompileExpression(conditional.Test);
                var otherwise = NewLabel();
                var end = NewLabel();
                Branch(JsOpcode.JumpIfFalse, otherwise);
                CompileExpression(conditional.WhenTrue);
                Branch(JsOpcode.Jump, end);
                Mark(otherwise);
                CompileExpression(conditional.WhenFalse);
                Mark(end);
                break;
            }

            case JsAssignmentExpression assignment:
                CompileAssignment(assignment);
                break;

            case JsMemberExpression member:
                CompileExpression(member.Target);

                if (member.Computed is null)
                {
                    Emit(JsOpcode.GetProperty, InternedName(member.Name));
                }
                else
                {
                    CompileExpression(member.Computed);
                    Emit(JsOpcode.GetIndex);
                }

                break;

            case JsCallExpression call:
                CompileCall(call);
                break;

            case JsNewExpression construction:
                CompileExpression(construction.Callee);

                foreach (var argument in construction.Arguments)
                {
                    CompileExpression(argument);
                }

                Emit(JsOpcode.Construct, (byte)construction.Arguments.Count);
                break;

            case JsSequenceExpression sequence:
                for (var index = 0; index < sequence.Expressions.Count; index++)
                {
                    CompileExpression(sequence.Expressions[index]);

                    if (index != sequence.Expressions.Count - 1)
                    {
                        Emit(JsOpcode.Pop);
                    }
                }

                break;

            default:
                Refuse(
                    expression.Span,
                    SliceSourceDiagnosticCode.ConstructOutsideManifest,
                    "this expression is not admitted by the declared feature manifest");

                Emit(JsOpcode.LoadUndefined);
                break;
        }
    }

    /// <summary>
    /// Emits a closure, giving a NAMED function expression a scope holding its own name.
    /// </summary>
    /// <remarks>
    /// <c>var f = function g() { return g; };</c> must see <c>g</c> inside the body and must not
    /// introduce it outside. The specification's answer is a one-binding environment created around
    /// the function object and holding it; this is that environment. Popping it afterwards does not
    /// destroy it - the closure captured the record itself, and what is popped is only this frame's
    /// view of the chain.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8F95C8
    // Broiler-Human:        PENDING
    private void CompileFunctionExpression(JsFunctionNode function)
    {
        if (function.Name.Length == 0 || function.IsArrow)
        {
            Emit(JsOpcode.Closure, (ushort)CompileFunction(function));
            return;
        }

        var outer = scope;
        scope = new Scope(ScopeKind.Block, outer);
        blockDepth++;
        var site = buffer.Code.Count + 1;
        Emit(JsOpcode.PushScope, (ushort)0);
        buffer.ScopeSites.Add((site, scope));
        var slot = scope.Declare(function.Name, constant: false);
        Emit(JsOpcode.Closure, (ushort)CompileFunction(function));
        Emit(JsOpcode.Duplicate);
        EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
        Emit(JsOpcode.PopScope);
        blockDepth--;
        scope = outer;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=379F3A
    // Broiler-Human:        PENDING
    private void CompileArray(JsArrayLiteral array)
    {
        var dense = true;

        foreach (var element in array.Elements)
        {
            if (element is null)
            {
                dense = false;
                break;
            }
        }

        if (dense && array.Elements.Count <= 1024)
        {
            foreach (var element in array.Elements)
            {
                CompileExpression(element!);
            }

            Emit(JsOpcode.NewArray, (ushort)array.Elements.Count);
            return;
        }

        Emit(JsOpcode.NewArray, (ushort)0);

        for (var index = 0; index < array.Elements.Count; index++)
        {
            if (array.Elements[index] is not { } element)
            {
                continue;
            }

            Emit(JsOpcode.LoadConstant, NumberConstant(index));
            CompileExpression(element);
            Emit(JsOpcode.DefineIndexed);
        }

        if (array.Elements.Count != 0)
        {
            Emit(JsOpcode.LoadConstant, NumberConstant(array.Elements.Count));
            Emit(JsOpcode.SetProperty, InternedName("length"));
            Emit(JsOpcode.Pop);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=592172
    // Broiler-Human:        PENDING
    private void CompileObject(JsObjectLiteral literal)
    {
        Emit(JsOpcode.NewObject);

        foreach (var entry in literal.Entries)
        {
            if (entry.Computed is not null)
            {
                CompileExpression(entry.Computed);
                CompileExpression(entry.Value);
                Emit(JsOpcode.DefineIndexed);
                continue;
            }

            CompileExpression(entry.Value);

            Emit(
                entry.Kind switch
                {
                    JsPropertyKind.Get => JsOpcode.DefineGetter,
                    JsPropertyKind.Set => JsOpcode.DefineSetter,
                    _ => JsOpcode.DefineField,
                },
                InternedName(entry.Key));
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B036E8
    // Broiler-Human:        PENDING
    private void CompileUnary(JsUnaryExpression unary)
    {
        switch (unary.Operator)
        {
            // `typeof` OF AN IMPORT IS NOT `typeof` OF AN UNDECLARED NAME. The form below exists so
            // that asking about a name nothing declares answers `"undefined"` instead of throwing;
            // an imported binding IS declared, so a read of one before its module initialised it is
            // the dead-zone `ReferenceError` the language gives - and answering `"undefined"` here
            // would have hidden exactly the case a cyclic import produces.
            case SliceTokenKind.Typeof when unary.Operand is JsIdentifier name &&
                !Resolvable(name.Name) &&
                (module is null || !module.Imports.ContainsKey(name.Name)):
                Emit(JsOpcode.LoadGlobalOrUndefined, InternedName(name.Name));
                Emit(JsOpcode.TypeOf);
                return;

            case SliceTokenKind.Delete when unary.Operand is JsMemberExpression member:
                CompileExpression(member.Target);

                if (member.Computed is null)
                {
                    Emit(JsOpcode.DeleteProperty, InternedName(member.Name));
                }
                else
                {
                    CompileExpression(member.Computed);
                    Emit(JsOpcode.DeleteIndex);
                }

                return;

            case SliceTokenKind.Delete:
                CompileExpression(unary.Operand);
                Emit(JsOpcode.Pop);
                Emit(JsOpcode.LoadTrue);
                return;

            default:
                break;
        }

        CompileExpression(unary.Operand);

        Emit(unary.Operator switch
        {
            SliceTokenKind.Minus => JsOpcode.Negate,
            SliceTokenKind.Plus => JsOpcode.ToNumber,
            SliceTokenKind.Bang => JsOpcode.Not,
            SliceTokenKind.Tilde => JsOpcode.BitwiseNot,
            SliceTokenKind.Typeof => JsOpcode.TypeOf,
            _ => JsOpcode.Void,
        });
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=272015
    // Broiler-Human:        PENDING
    private void CompileUpdate(JsUpdateExpression update)
    {
        var one = NumberConstant(1);
        var add = update.Operator == SliceTokenKind.PlusPlus ? JsOpcode.Add : JsOpcode.Subtract;

        if (update.Operand is JsIdentifier name)
        {
            LoadName(update.Span, name.Name);
            Emit(JsOpcode.ToNumber);

            if (!update.Prefix)
            {
                Emit(JsOpcode.Duplicate);
                Emit(JsOpcode.LoadConstant, one);
                Emit(add);
                StoreName(update.Span, name.Name);
                Emit(JsOpcode.Pop);
                return;
            }

            Emit(JsOpcode.LoadConstant, one);
            Emit(add);
            StoreName(update.Span, name.Name);
            return;
        }

        if (update.Operand is not JsMemberExpression member)
        {
            Refuse(
                update.Span,
                SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                "the operand of an update operator is not a reference");

            Emit(JsOpcode.LoadUndefined);
            return;
        }

        var function = FunctionScope();
        var temporary = function.Declare("#update" + function.SlotCount, constant: false);
        CompileExpression(member.Target);

        if (member.Computed is null)
        {
            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.GetProperty, InternedName(member.Name));
            Emit(JsOpcode.ToNumber);

            if (!update.Prefix)
            {
                Emit(JsOpcode.Duplicate);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, temporary);
            }

            Emit(JsOpcode.LoadConstant, one);
            Emit(add);

            if (update.Prefix)
            {
                Emit(JsOpcode.Duplicate);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, temporary);
            }

            Emit(JsOpcode.SetProperty, InternedName(member.Name));
            Emit(JsOpcode.Pop);
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, temporary);
            return;
        }

        CompileExpression(member.Computed);
        Emit(JsOpcode.DuplicateTwo);
        Emit(JsOpcode.GetIndex);
        Emit(JsOpcode.ToNumber);

        if (!update.Prefix)
        {
            Emit(JsOpcode.Duplicate);
            EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, temporary);
        }

        Emit(JsOpcode.LoadConstant, one);
        Emit(add);

        if (update.Prefix)
        {
            Emit(JsOpcode.Duplicate);
            EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, temporary);
        }

        Emit(JsOpcode.SetIndex);
        Emit(JsOpcode.Pop);
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, temporary);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E27AE6
    // Broiler-Human:        PENDING
    private void CompileLogical(JsLogicalExpression logical)
    {
        CompileExpression(logical.Left);
        var end = NewLabel();
        Emit(JsOpcode.Duplicate);

        switch (logical.Operator)
        {
            case SliceTokenKind.AmpersandAmpersand:
                Branch(JsOpcode.JumpIfFalse, end);
                break;

            case SliceTokenKind.BarBar:
                Branch(JsOpcode.JumpIfTrue, end);
                break;

            default:
                Emit(JsOpcode.LoadNull);
                Emit(JsOpcode.LooseEquals);
                Emit(JsOpcode.Not);
                Branch(JsOpcode.JumpIfTrue, end);
                break;
        }

        Emit(JsOpcode.Pop);
        CompileExpression(logical.Right);
        Mark(end);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=7AEB32
    // Broiler-Human:        PENDING
    private void CompileAssignment(JsAssignmentExpression assignment)
    {
        if (assignment.Operator == SliceTokenKind.Equals)
        {
            if (assignment.Target is JsIdentifier name)
            {
                CompileExpression(assignment.Value);
                StoreName(assignment.Span, name.Name);
                return;
            }

            if (assignment.Target is JsMemberExpression member)
            {
                CompileExpression(member.Target);

                if (member.Computed is null)
                {
                    CompileExpression(assignment.Value);
                    Emit(JsOpcode.SetProperty, InternedName(member.Name));
                    return;
                }

                CompileExpression(member.Computed);
                CompileExpression(assignment.Value);
                Emit(JsOpcode.SetIndex);
                return;
            }

            Refuse(
                assignment.Span,
                SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                "the left-hand side of an assignment is not a reference");

            Emit(JsOpcode.LoadUndefined);
            return;
        }

        if (assignment.Operator is SliceTokenKind.AmpersandAmpersand or SliceTokenKind.BarBar or
            SliceTokenKind.QuestionQuestion)
        {
            CompileLogicalAssignment(assignment);
            return;
        }

        var opcode = BinaryOpcode(assignment.Operator);

        if (assignment.Target is JsIdentifier target)
        {
            LoadName(assignment.Span, target.Name);
            CompileExpression(assignment.Value);
            Emit(opcode);
            StoreName(assignment.Span, target.Name);
            return;
        }

        if (assignment.Target is JsMemberExpression access)
        {
            CompileExpression(access.Target);

            if (access.Computed is null)
            {
                Emit(JsOpcode.Duplicate);
                Emit(JsOpcode.GetProperty, InternedName(access.Name));
                CompileExpression(assignment.Value);
                Emit(opcode);
                Emit(JsOpcode.SetProperty, InternedName(access.Name));
                return;
            }

            CompileExpression(access.Computed);
            Emit(JsOpcode.DuplicateTwo);
            Emit(JsOpcode.GetIndex);
            CompileExpression(assignment.Value);
            Emit(opcode);
            Emit(JsOpcode.SetIndex);
            return;
        }

        Refuse(
            assignment.Span,
            SliceSourceDiagnosticCode.InvalidAssignmentTarget,
            "the left-hand side of an assignment is not a reference");

        Emit(JsOpcode.LoadUndefined);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=9DF2D1
    // Broiler-Human:        PENDING
    private void CompileLogicalAssignment(JsAssignmentExpression assignment)
    {
        if (assignment.Target is not JsIdentifier name)
        {
            Refuse(
                assignment.Span,
                SliceSourceDiagnosticCode.ConstructOutsideManifest,
                "a logical assignment to a property is not admitted");

            Emit(JsOpcode.LoadUndefined);
            return;
        }

        LoadName(assignment.Span, name.Name);
        var end = NewLabel();
        Emit(JsOpcode.Duplicate);

        switch (assignment.Operator)
        {
            case SliceTokenKind.AmpersandAmpersand:
                Branch(JsOpcode.JumpIfFalse, end);
                break;

            case SliceTokenKind.BarBar:
                Branch(JsOpcode.JumpIfTrue, end);
                break;

            default:
                Emit(JsOpcode.LoadNull);
                Emit(JsOpcode.LooseEquals);
                Emit(JsOpcode.Not);
                Branch(JsOpcode.JumpIfTrue, end);
                break;
        }

        Emit(JsOpcode.Pop);
        CompileExpression(assignment.Value);
        StoreName(assignment.Span, name.Name);
        Mark(end);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5E4FBA
    // Broiler-Human:        PENDING
    private void CompileCall(JsCallExpression call)
    {
        if (call.Callee is JsMemberExpression member)
        {
            CompileExpression(member.Target);
            Emit(JsOpcode.Duplicate);

            if (member.Computed is null)
            {
                Emit(JsOpcode.GetProperty, InternedName(member.Name));
            }
            else
            {
                CompileExpression(member.Computed);
                Emit(JsOpcode.GetIndex);
            }

            // The receiver is under the callee and the calling convention wants it above, so one
            // exchange turns [receiver, callee] into [callee, receiver].
            Emit(JsOpcode.Swap);
        }
        else
        {
            CompileExpression(call.Callee);
            Emit(JsOpcode.LoadUndefined);
        }

        foreach (var argument in call.Arguments)
        {
            CompileExpression(argument);
        }

        if (call.Arguments.Count > 255)
        {
            Refuse(
                call.Span,
                SliceSourceDiagnosticCode.ConstructOutsideManifest,
                "a call with more than 255 arguments is not admitted");
        }

        Emit(JsOpcode.Call, (byte)System.Math.Min(call.Arguments.Count, 255));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F5D034
    // Broiler-Human:        PENDING
    private void CompileStoreTo(JsExpression target)
    {
        switch (target)
        {
            case JsIdentifier name:
                StoreName(name.Span, name.Name);
                break;

            case JsMemberExpression member when member.Computed is null:
            {
                var function = FunctionScope();
                var slot = function.Declare("#target" + function.SlotCount, constant: false);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, slot);
                CompileExpression(member.Target);
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, slot);
                Emit(JsOpcode.SetProperty, InternedName(member.Name));
                break;
            }

            case JsMemberExpression member:
            {
                var function = FunctionScope();
                var slot = function.Declare("#target" + function.SlotCount, constant: false);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, slot);
                CompileExpression(member.Target);
                CompileExpression(member.Computed!);
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, slot);
                Emit(JsOpcode.SetIndex);
                break;
            }

            default:
                Refuse(
                    target.Span,
                    SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                    "the left-hand side of an assignment is not a reference");

                break;
        }
    }

    // ---- names ---------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8C6533
    // Broiler-Human:        PENDING
    private void LoadName(SliceSourceSpan span, string name)
    {
        _ = span;

        if (TryResolve(name, out var hops, out var slot, out _))
        {
            EmitScoped(JsOpcode.LoadScoped, (byte)hops, slot);
            return;
        }

        // AN IMPORT IS CONSULTED AFTER THE SCOPES AND BEFORE THE GLOBAL, and that order is the
        // shadowing rule. A nearer declaration of the same name wins - a function may declare a
        // local called `a` while the module imports an `a` - and a name that is neither is still a
        // global, because a module's code sees the realm's globals like any other code.
        if (module is { } current && current.Imports.TryGetValue(name, out var entry))
        {
            Emit(JsOpcode.LoadImport, (ushort)entry);
            return;
        }

        Emit(JsOpcode.LoadGlobal, InternedName(name));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=838085
    // Broiler-Human:        PENDING
    private void StoreName(SliceSourceSpan span, string name)
    {
        if (TryResolve(name, out var hops, out var slot, out var constant))
        {
            // A WRITE TO A CONSTANT COMPILES AND THROWS, and until 2026-09-04 it was refused at the
            // front end instead. The language makes it a run-time `TypeError`, which is why
            // `assert.throws(TypeError, function () { x = 1; })` is a program every engine runs -
            // and refusing it said this manifest does not admit an assignment, when what it does
            // not admit is the assignment SUCCEEDING.
            if (constant)
            {
                Emit(JsOpcode.Duplicate);
                Emit(JsOpcode.ThrowImmutable, InternedName(name));
                return;
            }

            Emit(JsOpcode.Duplicate);
            EmitScoped(JsOpcode.StoreScoped, (byte)hops, slot);
            return;
        }

        // AN IMPORTED BINDING IS IMMUTABLE, and a write to one fails the same way a write to a
        // constant does: at the moment it runs, with a `TypeError`. It is not an early error, and
        // the conformance suite is emphatic about it - a whole family of its module tests wraps the
        // assignment in `assert.throws(TypeError, ...)`, which needs the program to compile.
        if (module is { } current && current.Imports.ContainsKey(name))
        {
            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.ThrowImmutable, InternedName(name));
            return;
        }

        Emit(JsOpcode.Duplicate);
        Emit(JsOpcode.StoreGlobal, InternedName(name));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=2CC181
    // Broiler-Human:        PENDING
    private bool Resolvable(string name) => TryResolve(name, out _, out _, out _);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6F5338
    // Broiler-Human:        PENDING
    private bool TryResolve(string name, out int hops, out int slot, out bool constant)
    {
        hops = 0;
        var current = scope;

        while (current is not null)
        {
            if (current.TryGet(name, out slot, out constant))
            {
                return true;
            }

            hops++;
            current = current.Parent;
        }

        slot = 0;
        constant = false;
        return false;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B568D1
    // Broiler-Human:        PENDING
    private Scope FunctionScope()
    {
        var current = scope;

        while (current.Kind == ScopeKind.Block && current.Parent is not null)
        {
            current = current.Parent;
        }

        return current;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BE4181
    // Broiler-Human:        PENDING
    private int Hops(Scope target)
    {
        var hops = 0;
        var current = scope;

        while (current is not null && !ReferenceEquals(current, target))
        {
            hops++;
            current = current.Parent;
        }

        return hops;
    }

    // ---- emission ------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=7BBB55
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<Exit> exits = [];

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=85D476
    // Broiler-Human:        PENDING
    private void Emit(JsOpcode opcode)
    {
        JsArtifactWriter.Emit(buffer.Code, opcode);
        buffer.Track(opcode, 0);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E6E1D2
    // Broiler-Human:        PENDING
    private void Emit(JsOpcode opcode, ushort operand)
    {
        JsArtifactWriter.Emit(buffer.Code, opcode, operand);
        buffer.Track(opcode, operand);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FB6C8B
    // Broiler-Human:        PENDING
    private void Emit(JsOpcode opcode, byte operand)
    {
        JsArtifactWriter.Emit(buffer.Code, opcode, operand);
        buffer.Track(opcode, operand);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=70C504
    // Broiler-Human:        PENDING
    private void EmitScoped(JsOpcode opcode, byte hops, int slot)
    {
        JsArtifactWriter.Emit(buffer.Code, opcode, hops, (ushort)slot);
        buffer.Track(opcode, 0);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=9CA5E1
    // Broiler-Human:        PENDING
    private void Branch(JsOpcode opcode, Label label)
    {
        var site = JsArtifactWriter.EmitBranch(buffer.Code, opcode);
        buffer.BranchSites.Add(site);
        label.Sites.Add(site);
        buffer.Track(opcode, 0);

        if (label.Offset >= 0)
        {
            JsArtifactWriter.PatchBranch(buffer.Code, site, (uint)label.Offset);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D48A65
    // Broiler-Human:        PENDING
    private Label NewLabel()
    {
        var label = new Label();
        buffer.Labels.Add(label);
        return label;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D41CF2
    // Broiler-Human:        PENDING
    private void Mark(Label label)
    {
        label.Offset = buffer.Code.Count;

        foreach (var site in label.Sites)
        {
            JsArtifactWriter.PatchBranch(buffer.Code, site, (uint)label.Offset);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FFE885
    // Broiler-Human:        PENDING
    private void Position(SliceSourceSpan span)
    {
        if (buffer.LastLine == span.Line && buffer.LastColumn == span.Column)
        {
            return;
        }

        buffer.LastLine = span.Line;
        buffer.LastColumn = span.Column;

        buffer.Positions.Add(
            ((uint)buffer.Code.Count, (uint)System.Math.Max(1, span.Line), (uint)System.Math.Max(1, span.Column)));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=9BC56B
    // Broiler-Human:        PENDING
    private ushort NumberConstant(double value)
    {
        var key = "n" + value.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
        return Intern(key, JsArtifactWriter.NumberConstant(value));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=82BF59
    // Broiler-Human:        PENDING
    private ushort StringConstant(string value) =>
        Intern("s" + value, JsArtifactWriter.StringConstant(value));

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=64AB04
    // Broiler-Human:        PENDING
    private ushort InternedName(string value) =>
        Intern("i" + value, JsArtifactWriter.InternedNameConstant(value));

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=542486
    // Broiler-Human:        PENDING
    private ushort Intern(string key, byte[] encoded)
    {
        if (constantIndex.TryGetValue(key, out var found))
        {
            return found;
        }

        if (constants.Count >= 65535)
        {
            Refuse(
                default,
                SliceSourceDiagnosticCode.TooManyConstants,
                "the constant pool is full");

            return 0;
        }

        var index = (ushort)constants.Count;
        constants.Add(encoded);
        constantIndex[key] = index;
        return index;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8A5C9D
    // Broiler-Human:        PENDING
    private static JsOpcode BinaryOpcode(SliceTokenKind kind) => kind switch
    {
        SliceTokenKind.Plus => JsOpcode.Add,
        SliceTokenKind.Minus => JsOpcode.Subtract,
        SliceTokenKind.Star => JsOpcode.Multiply,
        SliceTokenKind.Slash => JsOpcode.Divide,
        SliceTokenKind.Percent => JsOpcode.Remainder,
        SliceTokenKind.StarStar => JsOpcode.Exponent,
        SliceTokenKind.LessThan => JsOpcode.LessThan,
        SliceTokenKind.LessThanEquals => JsOpcode.LessThanOrEqual,
        SliceTokenKind.GreaterThan => JsOpcode.GreaterThan,
        SliceTokenKind.GreaterThanEquals => JsOpcode.GreaterThanOrEqual,
        SliceTokenKind.EqualsEqualsEquals => JsOpcode.StrictEquals,
        SliceTokenKind.BangEqualsEquals => JsOpcode.StrictNotEquals,
        SliceTokenKind.EqualsEquals => JsOpcode.LooseEquals,
        SliceTokenKind.BangEquals => JsOpcode.LooseNotEquals,
        SliceTokenKind.Bar => JsOpcode.BitwiseOr,
        SliceTokenKind.Ampersand => JsOpcode.BitwiseAnd,
        SliceTokenKind.Caret => JsOpcode.BitwiseXor,
        SliceTokenKind.LessThanLessThan => JsOpcode.ShiftLeft,
        SliceTokenKind.GreaterThanGreaterThan => JsOpcode.ShiftRight,
        SliceTokenKind.GreaterThanGreaterThanGreaterThan => JsOpcode.ShiftRightUnsigned,
        SliceTokenKind.Instanceof => JsOpcode.InstanceOf,
        SliceTokenKind.In => JsOpcode.In,
        _ => JsOpcode.Add,
    };

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3032B3
    // Broiler-Human:        PENDING
    private void Refuse(SliceSourceSpan span, SliceSourceDiagnosticCode code, string message)
    {
        if (diagnostics.Count >= 64)
        {
            return;
        }

        diagnostics.Add(new SliceSourceDiagnostic(code, message, span.Line, span.Column));
    }

    // ---- supporting types ------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=7108BA
    // Broiler-Human:        PENDING
    private enum ScopeKind
    {
        Program,
        Function,
        Block,

        /// <summary>
        /// A module's own environment, which is a scope of SLOTS rather than of globals.
        /// </summary>
        /// <remarks>
        /// It is a kind of its own and not <see cref="Program"/> with a flag, because every place
        /// that asks the question asks it about the top level: a script's top-level declaration is
        /// a property of the global object and a module's is a slot nothing outside the module can
        /// name.
        /// </remarks>
        Module,
    }

    /// <summary>What lowering one module has established so far.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private sealed class ModuleBuild(string key, Scope environment)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal string Key { get; } = key;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal Scope Scope { get; } = environment;

        /// <summary>The module specifiers this module requests, in source order.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<string> Requests { get; } = [];

        /// <summary>The key each request resolved to, parallel to <see cref="Requests"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<string> RequestKeys { get; } = [];

        /// <summary>The unit this module was presented as, which carries its resolutions.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal JsModuleUnit? Unit { get; init; }

        /// <summary>Each imported local name, and its index in the artifact's import table.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.Dictionary<string, int> Imports { get; } =
            new(System.StringComparer.Ordinal);

        /// <summary>This module's import entries, in the order the artifact writes them.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<JsImportEntryRow> ImportRows { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<JsLocalExportRow> LocalExports { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<JsIndirectExportRow> IndirectExports { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<uint> StarExports { get; } = [];

        /// <summary>Every name this module publishes, so a second use of one is an early error.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.HashSet<string> ExportNames { get; } =
            new(System.StringComparer.Ordinal);

        /// <summary>The function declarations the initialiser gives their closures.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<JsFunctionNode> Functions { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal int InitialiserUnit { get; set; }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal int BodyUnit { get; set; }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3336D6
    // Broiler-Human:        PENDING
    private enum ExitKind
    {
        Loop,
        Switch,
        Label,
        Finally,
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=418641
    // Broiler-Human:        PENDING
    private sealed class Label
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8A0B10
        // Broiler-Human:        PENDING
        internal int Offset { get; set; } = -1;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BDD300
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<int> Sites { get; } = [];
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D1A5EA
    // Broiler-Human:        PENDING
    private sealed class Exit(ExitKind kind, string label, int depth)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=57323F
        // Broiler-Human:        PENDING
        internal ExitKind Kind { get; } = kind;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FFD2E9
        // Broiler-Human:        PENDING
        internal string Label { get; } = label;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=DCD9D5
        // Broiler-Human:        PENDING
        internal int Depth { get; } = depth;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F69F0D
        // Broiler-Human:        PENDING
        internal Label? Break { get; init; }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B027D9
        // Broiler-Human:        PENDING
        internal Label? Continue { get; init; }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=90737C
        // Broiler-Human:        PENDING
        internal JsBlockStatement? Finaliser { get; init; }

        /// <summary>
        /// Whether this finaliser's body is being emitted right now.
        /// </summary>
        /// <remarks>
        /// <b>A `return` inside a `finally` must not run that same `finally` again.</b> The exit
        /// stack still carries the entry while its body is being emitted - it has to, because the
        /// body is emitted in the middle of compiling the statement that leaves - so without this
        /// flag the compiler emits the finaliser, meets the `return` inside it, and emits the
        /// finaliser again, forever. It is a compile-time loop rather than a wrong program, which
        /// is the only reason it is not worse.
        /// </remarks>
        internal bool Running { get; set; }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=04B3E8
    // Broiler-Human:        PENDING
    private sealed class Scope(ScopeKind kind, Scope? parent)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A869FD
        // Broiler-Human:        PENDING
        private readonly System.Collections.Generic.Dictionary<string, (int Slot, bool Constant)> names =
            new(System.StringComparer.Ordinal);

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A516E8
        // Broiler-Human:        PENDING
        internal ScopeKind Kind { get; } = kind;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A9A20D
        // Broiler-Human:        PENDING
        internal Scope? Parent { get; } = parent;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=069DEC
        // Broiler-Human:        PENDING
        internal int SlotCount { get; private set; }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=DB65E3
        // Broiler-Human:        PENDING
        internal int Declare(string name, bool constant)
        {
            if (names.TryGetValue(name, out var existing))
            {
                return existing.Slot;
            }

            var slot = SlotCount++;
            names[name] = (slot, constant);
            return slot;
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=1735B6
        // Broiler-Human:        PENDING
        internal bool Has(string name) => names.ContainsKey(name);

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=9E0F15
        // Broiler-Human:        PENDING
        internal int SlotOf(string name) => names[name].Slot;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=553A6E
        // Broiler-Human:        PENDING
        internal bool TryGet(string name, out int slot, out bool constant)
        {
            if (names.TryGetValue(name, out var found))
            {
                slot = found.Slot;
                constant = found.Constant;
                return true;
            }

            slot = 0;
            constant = false;
            return false;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=18DE55
    // Broiler-Human:        PENDING
    private readonly record struct PendingRegion(
        int TryStart, Label Handler, int ScopeDepth, JsFormat.HandlerKind Kind);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=1E79DA
    // Broiler-Human:        PENDING
    private sealed class ClosedRegion(
        uint tryStart, uint tryEnd, Label handler, uint scopeDepth, JsFormat.HandlerKind kind)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=61A751
        // Broiler-Human:        PENDING
        internal uint TryStart { get; } = tryStart;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D123E4
        // Broiler-Human:        PENDING
        internal uint TryEnd { get; } = tryEnd;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=707074
        // Broiler-Human:        PENDING
        internal Label HandlerLabel { get; } = handler;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CCD6C7
        // Broiler-Human:        PENDING
        internal uint Handler => (uint)System.Math.Max(0, HandlerLabel.Offset);

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=2DCEC1
        // Broiler-Human:        PENDING
        internal uint ScopeDepth { get; } = scopeDepth;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=1F1D54
        // Broiler-Human:        PENDING
        internal JsFormat.HandlerKind Kind { get; } = kind;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3F0624
    // Broiler-Human:        PENDING
    private sealed class UnitBuffer(ushort nameConstant, JsFormat.FunctionFlags flags)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=491BB2
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<byte> Code { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3FBC4A
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<int> BranchSites { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=62A27A
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<Label> Labels { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4380B9
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<(int Site, Scope Scope)> ScopeSites { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6DBCE6
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<PendingRegion> PendingRegions { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C4FF50
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<ClosedRegion> Regions { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4C27F0
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<(uint Offset, uint Line, uint Column)> Positions { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A31EDA
        // Broiler-Human:        PENDING
        internal ushort NameConstant { get; } = nameConstant;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=0E6DFE
        // Broiler-Human:        PENDING
        internal JsFormat.FunctionFlags Flags { get; set; } = flags;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E45E5F
        // Broiler-Human:        PENDING
        internal int ParameterCount { get; init; }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=10AC31
        // Broiler-Human:        PENDING
        internal int SlotCount { get; set; }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BB8115
        // Broiler-Human:        PENDING
        internal int MaximumStack { get; private set; } = 8;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=96DBEF
        // Broiler-Human:        PENDING
        internal int Height { get; private set; }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E86B38
        // Broiler-Human:        PENDING
        internal int LastLine { get; set; } = -1;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=50E8CA
        // Broiler-Human:        PENDING
        internal int LastColumn { get; set; } = -1;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F88AC0
        // Broiler-Human:        PENDING
        internal void Track(JsOpcode opcode, uint operand)
        {
            if (!JsOpcodes.TryDescribe(opcode, operand, out var pops, out var pushes))
            {
                return;
            }

            Height = System.Math.Max(0, Height - pops + pushes);
            MaximumStack = System.Math.Max(MaximumStack, Height + 24);
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=78C1AF
        // Broiler-Human:        PENDING
        internal void CloseRegion(int tryStart, int tryEnd)
        {
            for (var index = PendingRegions.Count - 1; index >= 0; index--)
            {
                if (PendingRegions[index].TryStart != tryStart)
                {
                    continue;
                }

                var pending = PendingRegions[index];
                PendingRegions.RemoveAt(index);

                Regions.Add(new ClosedRegion(
                    (uint)tryStart,
                    (uint)tryEnd,
                    pending.Handler,
                    (uint)pending.ScopeDepth,
                    pending.Kind));

                return;
            }
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5BEFB8
        // Broiler-Human:        PENDING
        internal void FinishScopes()
        {
            foreach (var (site, target) in ScopeSites)
            {
                var count = (ushort)target.SlotCount;
                Code[site] = (byte)(count & 0xFF);
                Code[site + 1] = (byte)(count >> 8);
            }
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4D0D1E
    // Broiler-Human:        PENDING
    private static class Walk
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=EDC888
        // Broiler-Human:        PENDING
        internal static bool Mentions(JsNode node, string name)
        {
            switch (node)
            {
                case JsIdentifier identifier:
                    return string.Equals(identifier.Name, name, System.StringComparison.Ordinal);

                case JsFunctionExpression:
                case JsFunctionDeclaration:
                    return false;

                default:
                    break;
            }

            foreach (var child in Children(node))
            {
                if (child is not null && Mentions(child, name))
                {
                    return true;
                }
            }

            return false;
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5B4E67
        // Broiler-Human:        PENDING
        private static System.Collections.Generic.IEnumerable<JsNode?> Children(JsNode node)
        {
            switch (node)
            {
                case JsExpressionStatement statement:
                    yield return statement.Expression;
                    break;

                case JsVariableStatement statement:
                    foreach (var declarator in statement.Declarators)
                    {
                        yield return declarator.Initialiser;
                    }

                    break;

                case JsBlockStatement statement:
                    foreach (var inner in statement.Body)
                    {
                        yield return inner;
                    }

                    break;

                case JsIfStatement statement:
                    yield return statement.Test;
                    yield return statement.Consequent;
                    yield return statement.Alternate;
                    break;

                case JsWhileStatement statement:
                    yield return statement.Test;
                    yield return statement.Body;
                    break;

                case JsDoWhileStatement statement:
                    yield return statement.Body;
                    yield return statement.Test;
                    break;

                case JsForStatement statement:
                    yield return statement.Initialiser;
                    yield return statement.Test;
                    yield return statement.Update;
                    yield return statement.Body;
                    break;

                case JsForInStatement statement:
                    yield return statement.Target;
                    yield return statement.Right;
                    yield return statement.Body;
                    break;

                case JsReturnStatement statement:
                    yield return statement.Value;
                    break;

                case JsThrowStatement statement:
                    yield return statement.Value;
                    break;

                case JsTryStatement statement:
                    yield return statement.Block;
                    yield return statement.Handler;
                    yield return statement.Finaliser;
                    break;

                case JsSwitchStatement statement:
                    yield return statement.Discriminant;

                    foreach (var clause in statement.Clauses)
                    {
                        yield return clause.Test;

                        foreach (var inner in clause.Body)
                        {
                            yield return inner;
                        }
                    }

                    break;

                case JsLabelledStatement statement:
                    yield return statement.Body;
                    break;

                case JsUnaryExpression expression:
                    yield return expression.Operand;
                    break;

                case JsUpdateExpression expression:
                    yield return expression.Operand;
                    break;

                case JsBinaryExpression expression:
                    yield return expression.Left;
                    yield return expression.Right;
                    break;

                case JsLogicalExpression expression:
                    yield return expression.Left;
                    yield return expression.Right;
                    break;

                case JsAssignmentExpression expression:
                    yield return expression.Target;
                    yield return expression.Value;
                    break;

                case JsConditionalExpression expression:
                    yield return expression.Test;
                    yield return expression.WhenTrue;
                    yield return expression.WhenFalse;
                    break;

                case JsMemberExpression expression:
                    yield return expression.Target;
                    yield return expression.Computed;
                    break;

                case JsCallExpression expression:
                    yield return expression.Callee;

                    foreach (var argument in expression.Arguments)
                    {
                        yield return argument;
                    }

                    break;

                case JsNewExpression expression:
                    yield return expression.Callee;

                    foreach (var argument in expression.Arguments)
                    {
                        yield return argument;
                    }

                    break;

                case JsSequenceExpression expression:
                    foreach (var inner in expression.Expressions)
                    {
                        yield return inner;
                    }

                    break;

                case JsArrayLiteral expression:
                    foreach (var element in expression.Elements)
                    {
                        yield return element;
                    }

                    break;

                case JsObjectLiteral expression:
                    foreach (var entry in expression.Entries)
                    {
                        yield return entry.Computed;
                        yield return entry.Value;
                    }

                    break;

                default:
                    break;
            }
        }
    }
}
