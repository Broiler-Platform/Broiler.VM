// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   167
// Annotated:        167/167
// Exempt:           83
// Human-reviewed:   0/167
// IP risk:          None
// Security risk:    High
// Criteria:         7/6
// Resource impact:  3/10 max
// Unverified:       167
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
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D9AAD7
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
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F9A1BA
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
/// <b>Every binding is resolved statically.</b> A name reaches a (depth, slot) pair in an
/// environment, a binding of the realm's global lexical environment, or a property of the global
/// object, and the decision is made here rather than at run time. Script-level <c>var</c> and
/// function declarations are global properties and script-level <c>let</c>, <c>const</c> and
/// <c>class</c> are lexical bindings, which is what the specification says and what makes one
/// script's declarations visible to the next either way.
/// </para>
/// <para>
/// <b><c>with</c> is the one construct that suspends that, and it suspends it for exactly the names
/// it must.</b> Inside a <c>with</c> body a name is resolved against the object FIRST — through
/// <c>HasProperty</c>, so the prototype chain counts, and minus whatever
/// <c>Symbol.unscopables</c> hides — and only then against the enclosing scopes. So the lowering
/// resolves such a name twice: statically, exactly as it always did, and again at run time by a
/// search over the object environment records between the reference and that static answer. The
/// static answer is what the search falls back to, which is what keeps the static half of the model
/// intact: a name inside a <c>with</c> body can reach an object a <c>with</c> put on the chain, or
/// the binding the language's own scope rules give it, and nothing else. <b>A name outside such a
/// body pays none of this</b>, because <c>Shadowable</c> answers false for it and the lowering is
/// the one that was already here.
/// </para>
/// <para>
/// <b>Script-level <c>let</c> and <c>const</c> are bindings of the realm's global LEXICAL
/// environment, which is not the global object.</b> They were global properties until 2026-09-05,
/// and the deviation is corrected rather than merely narrowed: a read before the declaration is
/// the <c>ReferenceError</c> the dead zone owes, an assignment to a script-level <c>const</c> is a
/// <c>TypeError</c> wherever it is written, and <c>globalThis</c> does not show either. The
/// bindings are the REALM's and not the unit's, because a conformance run's harness files publish
/// helpers with <c>const</c> from scripts of their own and the test that reads them is a later
/// script in the same realm.
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

    /// <summary>
    /// The names the script being lowered declares lexically at its top level, and whether each is
    /// immutable.
    /// </summary>
    /// <remarks>
    /// <b>It is the one thing the initialisation sites cannot see from where they stand.</b> A
    /// script-level lexical declaration is hoisted into the realm's global lexical environment
    /// before the first statement runs, and the statement that writes its value is compiled much
    /// later, in a method that knows only a name and a value; without this it could not tell that
    /// name apart from a <c>var</c>'s, which is a write to the global OBJECT and not to a binding.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=1F02DD
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<string, bool> programLexicals = [];

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

    /// <summary>The optional surfaces this artifact reaches, one entry each.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=137511
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.SortedSet<string> surfaces =
        new(System.StringComparer.Ordinal);

    /// <summary>The module records built so far, in declaration order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FC5AF9
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=1060A8
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<JsImportEntryRow> importEntries = [];

    /// <summary>The module being lowered, or <see langword="null"/> outside one.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=12B4BB
    // Broiler-Human:        PENDING
    private ModuleBuild? module;

    /// <summary>Whether the module being lowered has an <c>await</c> at its own top level.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6F207B
    // Broiler-Human:        PENDING
    private bool awaited;

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

    /// <summary>How many scripts of this artifact have been begun, which names their sites apart.</summary>
    /// <remarks>
    /// A tagged template's strings object is cached per CALL SITE, and the several scripts of one
    /// artifact share one realm - so two scripts with a template at the same line and column would
    /// share a cache entry and hand a tag the wrong strings. This counter is what makes the key
    /// unique, and it is a count of scripts rather than of code units because a nested function's
    /// unit index moves as the enclosing script is compiled.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C04DD3
    // Broiler-Human:        PENDING
    private int scripts;
    /// <summary>
    /// Whether the code being lowered is inside a method, so <c>super.x</c> resolves.
    /// </summary>
    /// <remarks>
    /// <b>An arrow function inherits it and every other function resets it</b>, which is the whole
    /// of the rule the language states as "an arrow has no <c>super</c> of its own". A method's
    /// nested arrow may write <c>super.m()</c>; a function expression nested in the same method may
    /// not, and refusing that at the parse would have needed the parser to track what it was
    /// inside of.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F81A3B
    // Broiler-Human:        PENDING
    private bool insideMethod;

    /// <summary>Whether the code being lowered is inside a derived constructor, so <c>super()</c> resolves.</summary>
    /// <remarks><inheritdoc cref="insideMethod" path="/remarks"/></remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A84723
    // Broiler-Human:        PENDING
    private bool insideDerivedConstructor;

    /// <summary>Whether the code being lowered is inside a function, so <c>new.target</c> resolves.</summary>
    /// <remarks><inheritdoc cref="insideMethod" path="/remarks"/></remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F1806D
    // Broiler-Human:        PENDING
    private bool insideFunction;

    /// <summary>
    /// Whether the code being lowered is a class static block's own body, where <c>return</c> has
    /// nothing to return from.
    /// </summary>
    /// <remarks>
    /// <b>It is NOT the pattern the three flags above use, and the difference is the arrow.</b>
    /// Those three are inherited by an arrow, because <c>this</c>, <c>super</c> and
    /// <c>new.target</c> all reach outward from one; this one is cleared by every nested body
    /// including an arrow's, because an arrow's <c>return</c> returns from the arrow rather than
    /// from whatever encloses it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=05C97B
    // Broiler-Human:        PENDING
    private bool insideStaticBlock;

    /// <summary>Compiles one source text as a script called <c>main</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D206EC
    // Broiler-Human:        PENDING
    public static JsCompilation Compile(string source, SliceParseOptions options) =>
        Compile([new JsScriptUnit("main", source, options)]);

    /// <summary>Compiles several source texts into one artifact, one entry point each.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=16DA79
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B1AB7D
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=78D70A
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=1C2D3D
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=9872AC
    // Broiler-Human:        PENDING
    public static JsCompilation Compile(
        System.Collections.Generic.IReadOnlyList<JsScriptUnit> scripts,
        System.Collections.Generic.IReadOnlyList<JsModuleUnit> modules)
    {
        var compiler = new JsCompiler();
        return compiler.Run(scripts, modules);
    }


    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=13553B
    // Broiler-Human:        PENDING
    private JsCompilation Run(
        System.Collections.Generic.IReadOnlyList<JsScriptUnit> scripts,
        System.Collections.Generic.IReadOnlyList<JsModuleUnit> modules)
    {
        foreach (var script in scripts)
        {
            var tokenizer = new SliceTokenizer(script.Text);
            var tokens = tokenizer.Tokenize();

            if (tokenizer.Diagnostics.Count != 0)
            {
                diagnostics.AddRange(tokenizer.Diagnostics);
                return new JsCompilation(false, null, diagnostics);
            }

            var parser = new JsParser(tokens, script.Options, script.ForceStrict);
            var program = parser.Parse();

            if (parser.Diagnostics.Count != 0)
            {
                diagnostics.AddRange(parser.Diagnostics);
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E349FF
    // Broiler-Human:        PENDING
    private bool TryParse(
        string text, SliceParseOptions options, bool forceStrict, out JsProgramNode program)
    {
        program = null!;
        awaited = false;
        var tokenizer = new SliceTokenizer(text);
        var tokens = tokenizer.Tokenize();

        if (tokenizer.Diagnostics.Count != 0)
        {
            diagnostics.AddRange(tokenizer.Diagnostics);
            return false;
        }

        var parser = new JsParser(tokens, options, forceStrict);
        program = parser.Parse();
        awaited = parser.SawTopLevelAwait;

        if (parser.Diagnostics.Count == 0)
        {
            return true;
        }

        diagnostics.AddRange(parser.Diagnostics);
        return false;
    }

    // ---- assembly ------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F8DFF1
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
                    region.StackHeight,
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

        // THE SURFACES SECTION IS WRITTEN ONLY WHEN THERE IS ONE, and its absence is what every
        // artifact written before the kind existed says: this program reaches no optional surface.
        // An empty section would say the same thing in more bytes, and would make the difference
        // between "declares none" and "declares nothing" a difference a reader has to look for.
        // THE MODULE SURFACE IS DECLARED WHERE THE RECORDS ARE WRITTEN, and not by a global read.
        // It is the one optional surface no name puts a program inside: what makes a program a
        // module is that it carries module records, so this is where it says so.
        if (built.Count != 0)
        {
            surfaces.Add(JsSurfaces.Modules);
        }

        if (surfaces.Count != 0)
        {
            var declared = new string[surfaces.Count];
            surfaces.CopyTo(declared);

            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Surfaces,
                JsArtifactWriter.Surfaces(declared)));
        }

        if (built.Count != 0)
        {
            sections.Add(new JavaScriptArtifactWriter.Section(
                (JavaScriptFormat.SectionKind)JsFormat.SectionKind.Modules,
                JsArtifactWriter.Modules(moduleRows)));
        }

        return JsArtifactWriter.Write(JsFormat.ManifestId, sections.ToArray());
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=79FFCF
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=72A149
    // Broiler-Human:        PENDING
    private int CompileProgram(JsProgramNode program, bool forceStrict)
    {
        var outerBuffer = buffer;
        var outerScope = scope;
        var outerDepth = blockDepth;
        var outerStrict = strict;
        var outerMethod = insideMethod;
        var outerDerived = insideDerivedConstructor;
        var outerFunction = insideFunction;
        var outerExits = exits;

        insideMethod = false;
        insideDerivedConstructor = false;
        insideFunction = false;
        exits = [];

        strict = program.IsStrict || forceStrict;
        scripts++;
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
        insideMethod = outerMethod;
        insideDerivedConstructor = outerDerived;
        insideFunction = outerFunction;
        exits = outerExits;
        return index;
    }

    /// <summary>Lowers one function body into a code unit of its own.</summary>
    /// <param name="function">The body.</param>
    /// <param name="extra">
    /// Flags the CALLER knows and the body does not - that this unit is a class constructor, and
    /// whether it is a derived one. Nothing in a constructor's own text says either.
    /// </param>
    /// <param name="isMethod">
    /// Whether this is a method, which decides two unrelated things: that <c>super</c> resolves
    /// inside it, and that it is <b>not a constructor</b>. <c>new (C.prototype.m)()</c> is a
    /// TypeError in the language, and the flag is what makes it one here.
    /// </param>
    /// <param name="isDerived">Whether this is the constructor of a class with a heritage.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CB02A9
    // Broiler-Human:        PENDING
    private int CompileFunction(
        JsFunctionNode function,
        JsFormat.FunctionFlags extra = JsFormat.FunctionFlags.None,
        bool isMethod = false,
        bool isDerived = false,
        bool isStaticBlock = false)
    {
        var outerBuffer = buffer;
        var outerScope = scope;
        var outerDepth = blockDepth;
        var outerStrict = strict;
        var outerMethod = insideMethod;
        var outerDerived = insideDerivedConstructor;
        var outerFunction = insideFunction;
        var outerExits = exits;
        exits = [];

        // EVERY NESTED BODY CLEARS THIS AND AN ARROW CLEARS IT TOO, which is the one thing that
        // separates it from the three flags below. `this` and `super` reach outward from an arrow;
        // a `return` does not - it returns from the arrow - so an arrow written inside a static
        // block may `return` even though the block may not.
        var outerStaticBlock = insideStaticBlock;
        insideStaticBlock = isStaticBlock;

        // AN ARROW INHERITS EVERY ONE OF THESE AND ANY OTHER FUNCTION RESETS THEM. That single
        // difference is what `super`, `this` and `new.target` mean inside an arrow.
        if (!function.IsArrow)
        {
            insideMethod = isMethod;
            insideDerivedConstructor = isDerived;
            insideFunction = true;
        }

        strict = strict || function.IsStrict;

        var flags = Strictness() | JsFormat.FunctionFlags.Constructible;

        if (function.IsArrow)
        {
            flags = (flags & ~JsFormat.FunctionFlags.Constructible) | JsFormat.FunctionFlags.Arrow;
        }
        else if (isMethod)
        {
            flags &= ~JsFormat.FunctionFlags.Constructible;
        }

        flags |= extra;

        // THE PARAMETER LIST DECIDES WHO BINDS THE PARAMETERS, and the two answers are not
        // interchangeable. A simple list is copied into slots by the frame, which costs nothing;
        // anything else has to run code - a default is an expression, a rest parameter is an Array,
        // a pattern is a destructuring - so the unit binds its own and declares that it does.
        //
        // A REPEATED NAME TAKES THE SECOND PATH TOO, and that is a repair rather than a nicety.
        // `function f(a, a) {}` is an ordinary sloppy-mode program every engine runs, and two
        // parameters sharing one name declare ONE slot - so the frame's copy loop was told to fill
        // two slots that were not there and the VERIFIER REFUSED an artifact this host had just
        // produced. Binding them in the prologue writes the same slot twice, left to right, which
        // is what the language says the second one does.
        var simple = IsSimpleParameterList(function.Parameters) &&
            !HasRepeatedName(function.Parameters);

        if (!simple)
        {
            flags |= JsFormat.FunctionFlags.BindsParameters;
        }

        // A GENERATOR IS NOT A CONSTRUCTOR, and dropping the bit is what makes `new g()` a type
        // error from the ordinary construction path rather than a special case somewhere in it.
        if (function.IsGenerator)
        {
            flags = (flags & ~JsFormat.FunctionFlags.Constructible) | JsFormat.FunctionFlags.Generator;
        }

        // AN ASYNC FUNCTION IS NOT A CONSTRUCTOR EITHER, and the arrow bit is left alone. That is
        // the one place the two suspension kinds differ in this method: a generator arrow is not a
        // production of the grammar, and an async arrow is - so the flags are set independently and
        // the verifier's own consistency check admits `Async | Arrow` while refusing
        // `Generator | Arrow`.
        if (function.IsAsync)
        {
            flags = (flags & ~JsFormat.FunctionFlags.Constructible) | JsFormat.FunctionFlags.Async;
        }

        var index = units.Count;
        var name = function.Name.Length == 0 ? (ushort)0 : (ushort)(InternedName(function.Name) + 1);

        buffer = new UnitBuffer(name, flags)
        {
            ParameterCount = ExpectedArgumentCount(function.Parameters),
        };

        units.Add(buffer);
        scope = new Scope(ScopeKind.Function, outerScope);
        blockDepth = 0;

        // EVERY PARAMETER NAME IS DECLARED BEFORE ANY DEFAULT IS EMITTED, and the slots start
        // empty. That is what makes `function f(a = b, b) {}` the ReferenceError the specification
        // says it is: `b` resolves to a binding that exists and has not been initialised, rather
        // than to a global of the same name that happens to be lying around.
        foreach (var parameter in function.Parameters)
        {
            DeclarePatternNames(parameter.Target, constant: false);
        }

        // A PARAMETER THAT BINDS `arguments` IS THE BINDING, AND THE OBJECT IS NOT CREATED AT ALL.
        // `Scope.Declare` answers with the existing slot when the name is already declared, so a
        // function whose formal parameter list contains `arguments` used to have its third or
        // fourth actual overwritten by the arguments object between entry and the first statement -
        // the parameter's value was simply gone. The specification says the same thing from the
        // other end: function declaration instantiation sets `argumentsObjectNeeded` to false when
        // `arguments` is one of the parameter names - and PARAMETER NAMES ARE BOUND NAMES, so
        // `function f({ arguments }) {}` shadows the object exactly as `function f(arguments) {}`
        // does. A `var arguments` or a function declaration of that name is NOT this case: each is
        // initialised after the object is, which is the order the specification asks for and the
        // order the code below already produces *(corrected: JSC-82)*.
        var parameterNames = new System.Collections.Generic.List<string>();

        foreach (var parameter in function.Parameters)
        {
            CollectPatternNames(parameter.Target, parameterNames);
        }

        var shadowedByParameter = false;

        foreach (var parameterName in parameterNames)
        {
            if (string.Equals(parameterName, "arguments", System.StringComparison.Ordinal))
            {
                shadowedByParameter = true;
                break;
            }
        }

        var usesArguments = !function.IsArrow && !shadowedByParameter && UsesArguments(function);

        if (usesArguments)
        {
            buffer.Flags |= JsFormat.FunctionFlags.UsesArguments;
            var slot = scope.Declare("arguments", constant: false);
            Emit(JsOpcode.NewArguments);
            EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
        }

        if (!simple)
        {
            CompileParameters(function.Parameters);

            // THE SEAM IS EMITTED FOR A GENERATOR AND FOR NOTHING ELSE, because a generator is the
            // only unit whose parameter list and whose body run at two different times. The
            // language binds the list at the CALL - `EvaluateGeneratorBody` and
            // `EvaluateAsyncGeneratorBody` both perform function declaration instantiation before
            // they create the object - and everything above this instruction is that binding.
            //
            // AN ASYNC FUNCTION IS DELIBERATELY NOT ON THE LIST, and it is the one arm of the three
            // that was already right. Its promise is made BEFORE the binding runs, so a default
            // that throws rejects the promise the call already answered with rather than throwing
            // at the call - which is what this engine's async arm already does by running the body
            // synchronously to its first `await`. Marking the seam there would have bought nothing
            // and cost one dispatch on every async call *(corrected: JSC-220)*.
            if (function.IsGenerator)
            {
                Emit(JsOpcode.EnterBody);
            }
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
        insideMethod = outerMethod;
        insideDerivedConstructor = outerDerived;
        insideFunction = outerFunction;
        insideStaticBlock = outerStaticBlock;
        exits = outerExits;
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=78D914
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=83E318
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=006D8D
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=AC06D2
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3FFD44
    // Broiler-Human:        PENDING
    private JsFormat.FunctionFlags Strictness() =>
        strict ? JsFormat.FunctionFlags.Strict : JsFormat.FunctionFlags.None;

    // ---- parameters ----------------------------------------------------------------------------

    /// <summary>Whether every parameter is one name with no initialiser and no <c>...</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8D6266
    // Broiler-Human:        PENDING
    private static bool IsSimpleParameterList(
        System.Collections.Generic.IReadOnlyList<JsParameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            if (parameter.IsRest || parameter.Default is not null ||
                parameter.Target is not JsTargetPattern { Target: JsIdentifier })
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Whether two parameters share one name, which makes them share one slot.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CCD0DC
    // Broiler-Human:        PENDING
    private static bool HasRepeatedName(
        System.Collections.Generic.IReadOnlyList<JsParameter> parameters)
    {
        for (var at = 1; at < parameters.Count; at++)
        {
            if (parameters[at].Target is not JsTargetPattern { Target: JsIdentifier later })
            {
                continue;
            }

            for (var before = 0; before < at; before++)
            {
                if (parameters[before].Target is JsTargetPattern { Target: JsIdentifier earlier } &&
                    string.Equals(earlier.Name, later.Name, System.StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// What the function reports as its <c>length</c>: the parameters before the first one that
    /// has a default or is a rest.
    /// </summary>
    /// <remarks>
    /// <b>A pattern with no initialiser COUNTS and a rest parameter never does.</b>
    /// <c>function f({a}, b = 1, c) {}</c> reports 1, not 3 and not 2 - the count stops at the first
    /// default and everything after it, default or not, is invisible to <c>length</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=AC49D2
    // Broiler-Human:        PENDING
    private static int ExpectedArgumentCount(
        System.Collections.Generic.IReadOnlyList<JsParameter> parameters)
    {
        var count = 0;

        foreach (var parameter in parameters)
        {
            if (parameter.IsRest || parameter.Default is not null)
            {
                break;
            }

            count++;
        }

        return count;
    }

    /// <summary>Emits the prologue that binds a parameter list the frame will not copy.</summary>
    /// <remarks>
    /// <para>
    /// Left to right, because a later default may read an earlier parameter and the specification
    /// says it sees the bound value rather than the argument.
    /// </para>
    /// <para>
    /// <b>ONE DECLARED DIVERGENCE: a repeated name in a list that is not simple is an EARLY ERROR
    /// the language raises and this front end does not.</b> <c>function f(a, a = 1) {}</c> is a
    /// <c>SyntaxError</c> everywhere else and runs here, binding <c>a</c> twice and keeping the
    /// second. The direction is the safe one - a negative test asserting that error scores a
    /// failure rather than a false pass - and the check belongs with the other early errors rather
    /// than bolted onto the lowering.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=ED8285
    // Broiler-Human:        PENDING
    private void CompileParameters(
        System.Collections.Generic.IReadOnlyList<JsParameter> parameters)
    {
        for (var at = 0; at < parameters.Count; at++)
        {
            var parameter = parameters[at];
            Position(parameter.Span);

            if (parameter.IsRest)
            {
                // A REST PARAMETER IS AN ARRAY AND NEVER `arguments`. It is dense, it has
                // Array.prototype, and a caller that passed nothing gives it length zero rather
                // than leaving it undefined.
                Emit(JsOpcode.RestArguments, (ushort)at);
            }
            else
            {
                Emit(JsOpcode.LoadArgument, (ushort)at);

                // A DEFAULT RUNS WHEN THE ARGUMENT IS `undefined`, WHICH IS NOT THE SAME AS ABSENT.
                // `f(undefined)` takes the default and `f(null)` does not, so the test is against
                // the value rather than against the argument count.
                ApplyDefault(parameter.Default, InferredFrom(parameter.Target));
            }

            BindPattern(parameter.Target, BindMode.Initialise);
        }
    }

    // ---- destructuring -------------------------------------------------------------------------

    /// <summary>What a pattern's leaf does with the value that reached it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D11D27
    // Broiler-Human:        PENDING
    private enum BindMode
    {
        /// <summary>Initialise a lexical binding, a parameter, or a script-level global.</summary>
        Initialise,

        /// <summary>Write the <c>var</c> binding hoisting already created.</summary>
        Var,

        /// <summary>Store through an arbitrary reference, which may be a member expression.</summary>
        Assign,
    }

    /// <summary>Declares every name a binding pattern introduces, before any of it is emitted.</summary>
    /// <remarks>
    /// At script top level nothing is declared here: a declaration there becomes a binding of the
    /// realm's global lexical environment, or a property of the global object when it is a
    /// <c>var</c>, and a slot with the same name would shadow both for the rest of the unit.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BD3639
    // Broiler-Human:        PENDING
    private void DeclarePatternNames(JsPattern pattern, bool constant)
    {
        if (scope.Kind == ScopeKind.Program && blockDepth == 0)
        {
            return;
        }

        switch (pattern)
        {
            case JsTargetPattern { Target: JsIdentifier name }:
                scope.Declare(name.Name, constant);
                return;

            case JsArrayPattern array:
                foreach (var element in array.Elements)
                {
                    if (element is not null)
                    {
                        DeclarePatternNames(element.Target, constant);
                    }
                }

                if (array.Rest is not null)
                {
                    DeclarePatternNames(array.Rest, constant);
                }

                return;

            case JsObjectPattern literal:
                foreach (var property in literal.Properties)
                {
                    DeclarePatternNames(property.Value.Target, constant);
                }

                if (literal.Rest is not null)
                {
                    DeclarePatternNames(literal.Rest, constant);
                }

                return;

            default:
                return;
        }
    }

    /// <summary>Replaces <c>undefined</c> on the top of the stack with an initialiser's value.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=2AB9C0
    // Broiler-Human:        PENDING
    private void ApplyDefault(JsExpression? initialiser, string inferred)
    {
        if (initialiser is null)
        {
            return;
        }

        var keep = NewLabel();
        Emit(JsOpcode.Duplicate);
        Emit(JsOpcode.LoadUndefined);
        Emit(JsOpcode.StrictEquals);
        Branch(JsOpcode.JumpIfFalse, keep);
        Emit(JsOpcode.Pop);

        // A DEFAULT IS ONE OF THE PLACES A NAME IS INFERRED. `function f(g = () => {})` gives the
        // arrow the name `g`, and so does `var { g = () => {} } = {}` - the binding the default is
        // for is the name, and it is the leaf's rather than the property's key.
        CompileNamedValue(initialiser, inferred);
        Mark(keep);
    }

    /// <summary>
    /// Destructures the value on top of the stack, which this consumes.
    /// </summary>
    /// <remarks>
    /// <b>One lowering serves declarations and assignments, and the mode is the whole of the
    /// difference.</b> The nesting, the defaults, the elisions and the rest handling are identical
    /// for <c>var [a, [b] = [2], ...c] = x</c> and for <c>[a, [b] = [2], ...c] = x</c>; only what a
    /// leaf does with its value differs. Writing it twice would have meant two copies of the
    /// iterator protocol, and the second copy is where the missing <c>IteratorClose</c> would have
    /// been.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C0E1B1
    // Broiler-Human:        PENDING
    private void BindPattern(JsPattern pattern, BindMode mode)
    {
        switch (pattern)
        {
            case JsTargetPattern leaf:
                BindLeaf(leaf.Target, mode);
                return;

            case JsArrayPattern array:
                BindArrayPattern(array, mode);
                return;

            case JsObjectPattern literal:
                BindObjectPattern(literal, mode);
                return;

            default:
                Emit(JsOpcode.Pop);
                return;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=EF721D
    // Broiler-Human:        PENDING
    private void BindLeaf(JsExpression target, BindMode mode)
    {
        if (mode == BindMode.Assign)
        {
            CompileStoreTo(target);
            Emit(JsOpcode.Pop);
            return;
        }

        if (target is not JsIdentifier name)
        {
            Refuse(
                target.Span,
                SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                "a declaration's pattern binds names, and this is not one");

            Emit(JsOpcode.Pop);
            return;
        }

        if (mode == BindMode.Var)
        {
            StoreName(target.Span, name.Name);
            Emit(JsOpcode.Pop);
            return;
        }

        if (scope.Kind == ScopeKind.Program && blockDepth == 0)
        {
            Emit(
                programLexicals.ContainsKey(name.Name)
                    ? JsOpcode.InitialiseGlobalLexical
                    : JsOpcode.StoreGlobal,
                InternedName(name.Name));

            return;
        }

        var slot = scope.Has(name.Name)
            ? scope.SlotOf(name.Name)
            : scope.Declare(name.Name, constant: false);

        EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
    }

    /// <summary>
    /// Destructures through the iteration protocol, which is what an array pattern is.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The iterator record lives in a slot, and the pattern is guarded by two exception regions
    /// that close it.</b> The record was on the operand stack until [JSC-151], because the stack
    /// nests where a slot chosen at compile time appeared not to - and the appearance was wrong.
    /// The nesting depth of a pattern is known while it is being lowered, so each nesting level
    /// declares a temporary of its own exactly as a computed member target already does, and two
    /// live records never share one.
    /// </para>
    /// <para>
    /// <b>The regions are what close the iterator when a completion abandons the pattern, and their
    /// handlers are entered at the height the pattern began at rather than at zero.</b> A region row
    /// has carried its own entry height since format version 1 and the verifier has always seeded
    /// the handler at that height plus the one value the executor pushes; only the lowering wrote
    /// zero, because until this pattern every region a lowering opened began at a statement
    /// boundary. So a region CAN guard an expression, and the objection this remark used to record
    /// - that a handler is entered at a fixed height and a pattern is applied where the stack is not
    /// empty - was an objection to the constant, not to the mechanism.
    /// </para>
    /// <para>
    /// <b>Two regions and not one, because the completion decides how the iterator is closed.</b>
    /// The language closes under a throw completion QUIETLY - whatever <c>return</c> does is
    /// discarded, because the exception already travelling is the one the program is owed - and
    /// closes under every other abrupt completion loudly, where an error from <c>return</c>
    /// propagates and a <c>return</c> that answers a non-object is itself a <c>TypeError</c>. The
    /// only other completion that reaches a pattern is the forced return a generator's
    /// <c>return()</c> raises at a <c>yield</c> inside it, and a <c>finally</c>-kind region is what
    /// catches that. The catch region is recorded FIRST so that a throw finds it, and the finally
    /// region second so that a forced return, which passes catch regions over, finds that one.
    /// </para>
    /// <para>
    /// <b>An element's target reference is evaluated BEFORE the iterator is stepped</b>, which is
    /// the order the language states and the reverse of the order an assignment uses everywhere
    /// else. <c>[ {}[f()] ] = iterable</c> calls <c>f</c> and never calls <c>next</c>, and a
    /// lowering that stepped first would have called <c>next</c> once before finding out that the
    /// reference throws. It matters most at a rest element, where stepping first drains the whole
    /// iterator before the reference gets a chance to fail.
    /// </para>
    /// <para>
    /// <b>An exhausted iterator supplies <c>undefined</c> rather than ending the pattern</b>, which
    /// is what makes <c>const [a, b] = [1]</c> give <c>b</c> the value <c>undefined</c> and what
    /// lets <c>[a = 1] = []</c> take its default.
    /// </para>
    /// <para>
    /// <b>Letting the executor close what an exception left on the operand stack was the design
    /// refused.</b> It needed no lowering at all: the values between a handler's height and the
    /// live top are exactly what the abandoned expression had built, and an iterator record among
    /// them is identifiable at run time. It was refused for three reasons. It gives the frame that
    /// unwinds WITHOUT a handler nowhere to do the work - which is the case the generator tests
    /// turn on, since a forced return with no <c>finally</c> in the frame leaves through the
    /// executor's own dispatch - and buying that case back needs a filter or a catch in every
    /// frame, which is the shape that killed the process at depth and that the dispatch's own
    /// remark records. It moves a rule of the LANGUAGE into the executor, where the artifact no
    /// longer says what closes and the verifier can no longer check it. And it grows the executor,
    /// which is the one budget a lowering-only change leaves alone.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3BECA4
    // Broiler-Falsified-If: a pattern leaves an iterator that is not done unclosed on any completion
    // Broiler-Human:        PENDING
    private void BindArrayPattern(JsArrayPattern pattern, BindMode mode)
    {
        var owner = FunctionScope();
        var record = owner.Declare("#iterator" + owner.SlotCount, constant: false);

        Emit(JsOpcode.IterateStart);
        EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, record);

        // THE HEIGHT IS READ HERE AND NOT AT THE FIRST ELEMENT, because this is the height the
        // handlers unwind to: everything the pattern pushes above it is what the abandoned
        // completion was holding, and the executor discards exactly that.
        var height = buffer.Height;
        var guarded = buffer.Code.Count;
        var raised = NewLabel();
        var forced = NewLabel();

        buffer.PendingRegions.Add(
            new PendingRegion(guarded, forced, blockDepth, JsFormat.HandlerKind.Finally, height));

        buffer.PendingRegions.Add(
            new PendingRegion(guarded, raised, blockDepth, JsFormat.HandlerKind.Catch, height));

        foreach (var element in pattern.Elements)
        {
            var prepared = PrepareTarget(element?.Target, mode);
            var exhausted = NewLabel();
            var ready = NewLabel();

            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, record);
            Branch(JsOpcode.IterateNext, exhausted);
            Branch(JsOpcode.Jump, ready);
            Mark(exhausted);
            Emit(JsOpcode.LoadUndefined);
            Mark(ready);

            if (element is null)
            {
                // An elision still advances the iterator, which is why the step above happens
                // before this test rather than instead of it.
                Emit(JsOpcode.Pop);
                continue;
            }

            ApplyDefault(element.Default, InferredFrom(element.Target));
            BindPrepared(element.Target, mode, prepared);
        }

        if (pattern.Rest is not null)
        {
            var prepared = PrepareTarget(pattern.Rest, mode);
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, record);
            Emit(JsOpcode.IterateRest);
            BindPrepared(pattern.Rest, mode, prepared);
        }

        // `var [] = x` protects nothing, and a region whose start equals its end is one the
        // verifier refuses and nothing could enter.
        ProtectSomething(guarded);

        // The catch region is closed first because `CloseRegion` closes the most recently ADDED,
        // and the executor takes the first region in that order whose range covers the throw.
        buffer.CloseRegion(guarded, buffer.Code.Count);
        buffer.CloseRegion(guarded, buffer.Code.Count);

        // A pattern that stopped before the iterator did owes it a `return`, and one that ran the
        // iterator out does not. The opcode reads the record's own done flag rather than being told
        // which case this is, so a rest element and an exhausted iterator both make it a no-op.
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, record);
        Emit(JsOpcode.IterateClose, (byte)0);

        var after = NewLabel();
        Branch(JsOpcode.Jump, after);

        // Both handlers are entered with one value where the pattern's own operands were: the
        // thrown value, or the parked forced return. `Throw` re-raises either as what it was.
        Mark(raised);
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, record);
        Emit(JsOpcode.IterateClose, (byte)1);
        Emit(JsOpcode.Throw);

        Mark(forced);
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, record);
        Emit(JsOpcode.IterateClose, (byte)0);
        Emit(JsOpcode.Throw);

        Mark(after);

        // THE STRAIGHT-LINE PASS WALKED THROUGH TWO HANDLERS IT CANNOT REACH, each entered at a
        // height nothing before it establishes, so the model it carries here is not the height the
        // code arriving by the jump actually has.
        buffer.Rejoin(height);
    }

    /// <summary>
    /// What evaluating an assignment pattern's target reference ahead of the value produced.
    /// </summary>
    /// <param name="Holds">Whether anything was evaluated, which only an assignment ever does.</param>
    /// <param name="Base">The slot holding the object the reference reads through.</param>
    /// <param name="Key">The slot holding a computed key, or -1 where the key is not computed.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=04C660
    // Broiler-Human:        PENDING
    private readonly record struct PreparedTarget(bool Holds, int Base, int Key);

    /// <summary>
    /// Evaluates an assignment pattern element's target reference, before the iterator is stepped.
    /// </summary>
    /// <remarks>
    /// <b>A name and a nested pattern evaluate nothing, and the language says so rather than this
    /// being an optimisation.</b> The step that evaluates the target is stated only for a target
    /// that is neither an object nor an array literal, and an identifier reference is resolved
    /// where it is STORED - which is why an assignment to an undeclared name in strict code still
    /// fails at the store and not here.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=14277C
    // Broiler-Human:        PENDING
    private PreparedTarget PrepareTarget(JsPattern? target, BindMode mode)
    {
        if (!PreparesTarget(target, mode))
        {
            return default;
        }

        switch (((JsTargetPattern)target!).Target)
        {
            case JsPrivateMemberExpression privateAccess:
                return new PreparedTarget(true, Spill(privateAccess.Target), -1);

            case JsMemberExpression member when member.Computed is null:
                return new PreparedTarget(true, Spill(member.Target), -1);

            case JsMemberExpression member:
            {
                // THE ORDER OF THESE TWO IS THE LANGUAGE'S. A computed member reference evaluates
                // its base before its key, and both before anything the value needs.
                var basis = Spill(member.Target);
                return new PreparedTarget(true, basis, Spill(member.Computed!));
            }

            default:
                return default;
        }
    }

    /// <summary>
    /// Whether this pattern's target is a reference the language evaluates ahead of the value.
    /// </summary>
    /// <remarks>
    /// It is asked twice for one property of an object assignment pattern - once to decide whether
    /// the pattern's own computed key has to be parked in a temporary, and once by
    /// <see cref="PrepareTarget"/> - and the two must give the same answer, which is why it is a
    /// predicate rather than the same three cases written out again.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4988A7
    // Broiler-Human:        PENDING
    private static bool PreparesTarget(JsPattern? target, BindMode mode) =>
        mode == BindMode.Assign &&
        target is JsTargetPattern
        {
            Target: JsMemberExpression or JsPrivateMemberExpression,
        };

    /// <summary>Compiles one expression and parks its value in a temporary of the function.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=64DEAE
    // Broiler-Human:        PENDING
    private int Spill(JsExpression expression)
    {
        var owner = FunctionScope();
        var slot = owner.Declare("#held" + owner.SlotCount, constant: false);
        CompileExpression(expression);
        EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, slot);
        return slot;
    }

    /// <summary>
    /// Stores the value on top of the stack through a reference already evaluated, and consumes it.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3E2685
    // Broiler-Human:        PENDING
    private void BindPrepared(JsPattern target, BindMode mode, in PreparedTarget prepared)
    {
        if (!prepared.Holds)
        {
            BindPattern(target, mode);
            return;
        }

        var leaf = (JsTargetPattern)target;
        var owner = FunctionScope();
        var held = owner.Declare("#held" + owner.SlotCount, constant: false);
        EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, held);
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, prepared.Base);

        switch (leaf.Target)
        {
            case JsPrivateMemberExpression privateAccess:
                EmitPrivateName(privateAccess.Span, privateAccess.Name);
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, held);
                Emit(JsOpcode.StorePrivate);
                break;

            case JsMemberExpression member when member.Computed is null:
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, held);
                Emit(JsOpcode.SetProperty, InternedName(member.Name));
                break;

            default:
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, prepared.Key);
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, held);
                Emit(JsOpcode.SetIndex);
                break;
        }

        Emit(JsOpcode.Pop);
    }

    /// <summary>Destructures by reading properties, which is what an object pattern is.</summary>
    /// <remarks>
    /// <b>Reading properties and NOT iterating</b>, which is the whole difference from an array
    /// pattern: <c>var { 0: first } = [1]</c> reads an index and never asks the Array for an
    /// iterator. The nullish check is explicit rather than left to the first property read, because
    /// <c>var {} = undefined</c> reads nothing and still has to refuse.
    /// <para>
    /// <b>ONE DECLARED DIVERGENCE, and only where a rest property meets a computed key:</b> the key
    /// expression is evaluated once, as the language says, but the value it produced is converted
    /// to a property key twice - once to read the property and once to exclude it from the rest.
    /// A key object whose <c>toString</c> has a side effect therefore runs it twice. Converting
    /// once would need an opcode whose only job is <c>ToPropertyKey</c>.
    /// </para>
    /// <para>
    /// <b>AN ASSIGNMENT PATTERN PREPARES EVERY TARGET REFERENCE BEFORE IT READS THE PROPERTY THAT
    /// FEEDS IT, and a declaration pattern prepares nothing</b>, which is not a symmetry this
    /// lowering chose. <c>KeyedDestructuringAssignmentEvaluation</c> evaluates the target - and
    /// only a target that is neither an object nor an array literal - BEFORE the <c>GetV</c> that
    /// supplies its value, so <c>({ a: o[k()] } = src)</c> calls <c>k</c> before it reads
    /// <c>src.a</c> and <c>({ ...o[k()] } = src)</c> calls it before <c>CopyDataProperties</c> runs
    /// at all. A declaration has no reference to evaluate: it initialises a binding, and the
    /// binding is found where it is written *(corrected: JSC-221)*.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5A65F9
    // Broiler-Human:        PENDING
    private void BindObjectPattern(JsObjectPattern pattern, BindMode mode)
    {
        var named = pattern.Properties.Count != 0 && pattern.Properties[0].Computed is null
            ? pattern.Properties[0].Key
            : string.Empty;

        Emit(JsOpcode.RequireCoercible, InternedName(named));
        var excluded = new System.Collections.Generic.List<int>();

        foreach (var property in pattern.Properties)
        {
            // A COMPUTED KEY IS PARKED BEFORE THE TARGET IS PREPARED, and both happen before the
            // source is read. `PropertyName` is step 1 of the property's own evaluation and the
            // target is step 1 of the element's, so the two run in this order and the `GetV`
            // between them runs after both. Parking is what makes that possible at all: the key
            // used to be pushed on top of the source object, which is where the read needs it and
            // nowhere near where a reference evaluated before it could sit.
            //
            // ONLY WHERE THERE IS A REFERENCE TO PUT BETWEEN THEM. A name and a nested pattern
            // evaluate nothing, so for them the key still goes straight onto the stack and this
            // costs no slot - which is every shorthand property and every plain `{ a: x }`.
            var held = -1;

            if (PreparesTarget(property.Value.Target, mode) && property.Computed is not null)
            {
                held = Spill(property.Computed);

                if (pattern.Rest is not null)
                {
                    excluded.Add(held);
                }
            }

            var prepared = PrepareTarget(property.Value.Target, mode);
            Emit(JsOpcode.Duplicate);

            if (property.Computed is null)
            {
                Emit(JsOpcode.GetProperty, InternedName(property.Key));
            }
            else if (held >= 0)
            {
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, held);
                Emit(JsOpcode.GetIndex);
            }
            else
            {
                CompileExpression(property.Computed);

                if (pattern.Rest is not null)
                {
                    // A REST PROPERTY EXCLUDES THE KEYS THE PATTERN NAMED, and a computed key is
                    // only a key once. Re-evaluating the expression to build the exclusion list
                    // would run its side effects twice, which is observable with any key whose
                    // expression is a call.
                    var owner = FunctionScope();
                    var slot = owner.Declare("#key" + owner.SlotCount, constant: false);
                    Emit(JsOpcode.Duplicate);
                    EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, slot);
                    excluded.Add(slot);
                }

                Emit(JsOpcode.GetIndex);
            }

            ApplyDefault(property.Value.Default, InferredFrom(property.Value.Target));
            BindPrepared(property.Value.Target, mode, prepared);
        }

        if (pattern.Rest is null)
        {
            Emit(JsOpcode.Pop);
            return;
        }

        // THE REST TARGET IS PREPARED BEFORE THE REST OBJECT EXISTS, because
        // `RestDestructuringAssignmentEvaluation` evaluates it first and only then performs
        // `CopyDataProperties`. A source with a getter on it makes the difference visible without
        // any error at all: the getter must run after `k` in `({ ...o[k()] } = src)`.
        var restTarget = PrepareTarget(pattern.Rest, mode);

        Emit(JsOpcode.NewObject);
        Emit(JsOpcode.Pick, (byte)1);
        Emit(JsOpcode.SpreadObject);

        foreach (var property in pattern.Properties)
        {
            if (property.Computed is not null)
            {
                continue;
            }

            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.DeleteProperty, InternedName(property.Key));
            Emit(JsOpcode.Pop);
        }

        foreach (var slot in excluded)
        {
            Emit(JsOpcode.Duplicate);
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, slot);
            Emit(JsOpcode.DeleteIndex);
            Emit(JsOpcode.Pop);
        }

        BindPrepared(pattern.Rest, mode, restTarget);
        Emit(JsOpcode.Pop);
    }

    // ---- hoisting ------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=09C42C
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

        // A FUNCTION DECLARATION CREATES ITS GLOBAL BINDING BEFORE IT IS WRITTEN, and until
        // 2026-09-04 the write was the only step. It worked because a store to a name the global
        // object did not have created it — which is exactly what strict code may not do, so the
        // moment `StoreGlobal` began refusing that *(JSC-93)*, EVERY strict script with a function
        // declaration in it threw a `ReferenceError` about the function it was declaring. The
        // declaration is separate from the write in the specification for this reason: the binding
        // exists before anything assigns to it.
        foreach (var function in functions)
        {
            Emit(JsOpcode.DeclareGlobal, InternedName(function.Name));
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4A2764
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
                    functions.Add(declaration.Function);
                    break;

                case JsBlockStatement block:
                    CollectVarScope(block.Body, names, functions, null);
                    break;

                // A `with` IS NOT A HOISTING SCOPE. `with (o) { var x; function f() { } }` declares
                // both in the enclosing function, which is what makes `x` survive the body and what
                // makes `f` callable after it - and it is also why an assignment to `x` INSIDE the
                // body still asks the object first: the binding is the function's and the write is
                // resolved dynamically.
                case JsWithStatement scoped:
                    CollectVarScope([scoped.Body], names, functions, null);
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
                        CollectHeadNames(loop.Name, loop.Pattern, names);
                    }

                    CollectVarScope([loop.Body], names, functions, null);
                    break;

                case JsForOfStatement loop:
                    if (loop.Declaration == SliceDeclarationKind.Var)
                    {
                        CollectHeadNames(loop.Name, loop.Pattern, names);
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=308C05
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
                    CollectDeclaratorNames(declarator, names);
                }
            }

            // A CLASS DECLARATION IS A LEXICAL DECLARATION AND NOT A HOISTED ONE. Leaving it out
            // of this collection would mean the block enclosing it pushes no scope, and the class
            // binding would land in whatever scope happened to be current - which for a class in a
            // loop body is a slot the next turn overwrites.
            if (statement is JsClassDeclaration declaration && declaration.Class.Name.Length != 0)
            {
                names.Add(declaration.Class.Name);
            }
        }
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F31EC4
    // Broiler-Human:        PENDING
    private static bool UsesArguments(JsFunctionNode function)
    {
        foreach (var parameter in function.Parameters)
        {
            if (parameter.Default is not null && Walk.Mentions(parameter.Default, "arguments"))
            {
                return true;
            }

            if (Walk.Mentions(parameter.Target, "arguments"))
            {
                return true;
            }
        }

        foreach (var statement in function.Body)
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=22E7D6
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

            case JsClassDeclaration declaration:
                CompileClassDeclaration(declaration);
                break;

            case JsBlockStatement block:
                CompileBlock(block, completion);
                break;

            case JsWithStatement scoped:
                CompileWith(scoped, completion);
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

            case JsForOfStatement loop:
                CompileForOf(loop, completion, string.Empty);
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6B9B33
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

            case JsForOfStatement loop:
                CompileForOf(loop, completion, labelled.Label);
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=AB7555
    // Broiler-Human:        PENDING
    private void CompileVariable(JsVariableStatement variable)
    {
        foreach (var declarator in variable.Declarators)
        {
            if (declarator.Pattern is not null)
            {
                // A DESTRUCTURING DECLARATION ALWAYS HAS AN INITIALISER, whatever its keyword.
                // `var a;` is fine and `var [a];` is not, because there is nothing for the pattern
                // to take apart - and the grammar says so for `var` as loudly as for `const`.
                if (declarator.Initialiser is null)
                {
                    Refuse(
                        declarator.Span,
                        SliceSourceDiagnosticCode.ConstWithoutInitialiser,
                        "a destructuring declaration needs an initialiser");

                    continue;
                }

                if (variable.Kind != SliceDeclarationKind.Var)
                {
                    DeclarePatternNames(
                        declarator.Pattern, variable.Kind == SliceDeclarationKind.Const);
                }

                CompileExpression(declarator.Initialiser);

                BindPattern(
                    declarator.Pattern,
                    variable.Kind == SliceDeclarationKind.Var ? BindMode.Var : BindMode.Initialise);

                continue;
            }

            if (variable.Kind == SliceDeclarationKind.Const && declarator.Initialiser is null)
            {
                Refuse(
                    declarator.Span,
                    SliceSourceDiagnosticCode.ConstWithoutInitialiser,
                    "`const " + declarator.Name + "` needs an initialiser");

                continue;
            }

            if (scope.Kind == ScopeKind.Program && blockDepth == 0)
            {
                // `let x;` STILL HAS TO RUN. A `var` with no initialiser writes nothing, because
                // hoisting already made the property and `undefined` is what it holds; a lexical
                // binding with no initialiser is in its dead zone until the declaration is REACHED,
                // and what reaches it is this instruction. Skipping it would leave `let x;` in a
                // dead zone for the rest of the program.
                if (programLexicals.ContainsKey(declarator.Name))
                {
                    if (declarator.Initialiser is null)
                    {
                        Emit(JsOpcode.LoadUndefined);
                    }
                    else
                    {
                        CompileNamedValue(declarator.Initialiser, declarator.Name);
                    }

                    Emit(JsOpcode.InitialiseGlobalLexical, InternedName(declarator.Name));
                    continue;
                }

                if (declarator.Initialiser is null)
                {
                    continue;
                }

                CompileNamedValue(declarator.Initialiser, declarator.Name);
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

                CompileNamedValue(declarator.Initialiser, declarator.Name);
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
                CompileNamedValue(declarator.Initialiser, declarator.Name);
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

    /// <summary>Lowers <c>with</c>: one object environment record around one statement.</summary>
    /// <remarks>
    /// <para>
    /// <b>It is a block whose record holds an object, and every exit is the exits a block already
    /// has.</b> The depth counter rises with the record and falls with it, so
    /// <see cref="Unwrap"/> — which is what <c>break</c>, <c>continue</c> and <c>return</c> unwind
    /// through — discards it without knowing what kind of record it is, and an exception region
    /// opened inside the body records the depth WITH it and is truncated back to the same figure by
    /// the executor. Nothing about the object record needed its own unwinding path, which is the
    /// whole reason it is a record on the ordinary chain rather than a second one.
    /// </para>
    /// <para>
    /// <b>The scope it pushes declares nothing and cannot.</b> A <c>var</c> inside the body was
    /// hoisted to the enclosing function or to the global object before this ran, and a lexical
    /// declaration cannot be a <c>with</c> body at all — the parser refuses that as the syntax error
    /// the language calls it. So a name declared inside a <c>with</c> body is a name declared in the
    /// block inside it, which pushes a record of its own.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4F33F5
    // Broiler-Human:        PENDING
    private void CompileWith(JsWithStatement statement, int completion)
    {
        CompileExpression(statement.Object);
        Emit(JsOpcode.PushObjectScope);

        var outer = scope;
        scope = new Scope(ScopeKind.With, outer);
        blockDepth++;

        CompileStatement(statement.Body, completion);

        Emit(JsOpcode.PopScope);
        blockDepth--;
        scope = outer;
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BE62B9
    // Broiler-Human:        PENDING
    private void CompileFor(JsForStatement loop, int completion, string label)
    {
        var lexical = new System.Collections.Generic.List<string>();

        if (loop.Initialiser is JsVariableStatement head && head.Kind != SliceDeclarationKind.Var)
        {
            foreach (var declarator in head.Declarators)
            {
                CollectDeclaratorNames(declarator, lexical);
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=7B19B6
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

            if (loop.Pattern is null)
            {
                scope.Declare(loop.Name, loop.Declaration == SliceDeclarationKind.Const);
            }
            else
            {
                DeclarePatternNames(loop.Pattern, loop.Declaration == SliceDeclarationKind.Const);
            }
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

        // A `for … in` HEAD MAY DESTRUCTURE THE KEY, which reads oddly and is ordinary grammar:
        // the value bound each turn is a String, so `for (const [a, b] in o)` takes its first two
        // characters apart. The lowering is the same one every other position uses.
        if (loop.Pattern is not null)
        {
            BindPattern(
                loop.Pattern,
                pushed ? BindMode.Initialise
                    : loop.Declaration == SliceDeclarationKind.Var ? BindMode.Var : BindMode.Assign);
        }
        else if (pushed)
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

    /// <summary>
    /// Lowers <c>for … of</c>: the iteration protocol, and the <c>IteratorClose</c> every way out
    /// of it owes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The head's bindings exist, uninitialised, while the source expression is evaluated.</b>
    /// That is what makes <c>for (const x of x)</c> a <c>ReferenceError</c> rather than a read of
    /// whatever <c>x</c> meant outside, and it is why the head scope is pushed before the right-hand
    /// side and popped straight after it. The loop then pushes a SECOND scope, because the head's
    /// is gone by the time the first turn starts.
    /// </para>
    /// <para>
    /// <b>Every abrupt exit closes the iterator, and there are four of them.</b> Running out is the
    /// one that does not - the iterator said it was finished. <c>break</c> and a labelled
    /// <c>break</c> close it in <see cref="CompileJumpOut"/>, <c>return</c> closes it in
    /// <see cref="CompileReturn"/>, and a <c>throw</c> from the body reaches the handler this
    /// method installs. <c>continue</c> is the one that must NOT close it, which is why the exit
    /// record carries the slot rather than the loop emitting a close at its own bottom.
    /// </para>
    /// <para>
    /// <b>The per-iteration copy is what a closure in the body captures.</b> Without it every
    /// closure a loop created would share one binding and see the last value, which is the most
    /// reproduced defect in the language.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=ACE79E
    // Broiler-Human:        PENDING
    private void CompileForOf(JsForOfStatement loop, int completion, string label)
    {
        var lexical = loop.Declaration is SliceDeclarationKind.Let or SliceDeclarationKind.Const;
        var constant = loop.Declaration == SliceDeclarationKind.Const;
        var outerDepth = blockDepth;
        var outer = scope;

        if (lexical)
        {
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var headSite = buffer.Code.Count + 1;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((headSite, scope));
            DeclareHead(loop.Name, loop.Pattern, constant);
        }

        CompileExpression(loop.Right);

        // `for await` READS `Symbol.asyncIterator` AND FALLS BACK, and the falling back is the
        // operation rather than a courtesy: an Array has no `Symbol.asyncIterator`, so a loop that
        // refused anything without one would refuse `for await (const x of [p, q])`, which is the
        // case the statement exists for. What the fall-back builds awaits each VALUE, which is the
        // whole difference between iterating promises and iterating what they resolve to.
        Emit(loop.IsAwait ? JsOpcode.IterateStartAsync : JsOpcode.IterateStart);

        if (lexical)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = outer;
        }

        // The record lives in a slot of the function's own environment for the same reason the
        // `for … in` enumerator does: a `break` out of the body would otherwise have to know how
        // many operand-stack entries every enclosing loop is holding.
        var owner = FunctionScope();
        var record = owner.Declare("#forof" + owner.SlotCount, constant: false);
        EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, record);

        var loopScope = scope;

        if (lexical)
        {
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var bodySite = buffer.Code.Count + 1;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((bodySite, scope));
            DeclareHead(loop.Name, loop.Pattern, constant);
            loopScope = scope;
        }

        var guarded = buffer.Code.Count;
        var unwind = NewLabel();

        buffer.PendingRegions.Add(
            new PendingRegion(guarded, unwind, outerDepth, JsFormat.HandlerKind.Catch));

        var top = NewLabel();
        var exit = new Exit(ExitKind.Loop, label, blockDepth)
        {
            Break = NewLabel(),
            Continue = NewLabel(),
            IteratorSlot = record,
            IteratorIsAsync = loop.IsAwait,
        };

        Mark(top);
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, record);

        // THE STEP IS THREE INSTRUCTIONS WHERE THE SYNCHRONOUS ONE IS ONE, and the middle one is a
        // suspension. `next` is called, what it answered is awaited, and only then is the answer
        // asked whether it is done - so the record stays on the stack under the awaited value and
        // the pair is consumed together. Folding the three into one instruction would have needed
        // an instruction that suspends in the middle of itself.
        if (loop.IsAwait)
        {
            Emit(JsOpcode.IterateNextAsync);
            Emit(JsOpcode.Await);
            Branch(JsOpcode.IterateAwaitStep, exit.Break!);
        }
        else
        {
            Branch(JsOpcode.IterateNext, exit.Break!);
        }

        if (lexical)
        {
            var copySite = buffer.Code.Count + 1;
            Emit(JsOpcode.CopyScope, (ushort)0);
            buffer.ScopeSites.Add((copySite, loopScope));
        }

        if (loop.Pattern is not null)
        {
            BindPattern(
                loop.Pattern,
                lexical ? BindMode.Initialise
                    : loop.Declaration == SliceDeclarationKind.Var ? BindMode.Var : BindMode.Assign);
        }
        else if (lexical)
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
        buffer.CloseRegion(guarded, buffer.Code.Count);

        if (lexical)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = outer;
        }

        var after = NewLabel();
        Branch(JsOpcode.Jump, after);

        // THE HANDLER IS ENTERED WITH THE THROWN VALUE AND NOTHING ELSE, at the depth outside the
        // loop's own scope, because the executor trims the scope chain to the region's declared
        // depth before it lands here. It closes quietly and rethrows: an error the iterator's
        // `return` raises must not replace the one already travelling.
        Mark(unwind);

        if (loop.IsAwait)
        {
            CompileAsyncUnwind(record, outerDepth);
        }
        else
        {
            EmitScoped(JsOpcode.LoadScoped, (byte)outerDepth, record);
            Emit(JsOpcode.IterateClose, (byte)1);
            Emit(JsOpcode.Throw);
        }

        Mark(after);
    }

    /// <summary>
    /// The <c>for await</c> handler: park the exception, close asynchronously, re-raise it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The exception is parked in a slot because the close SUSPENDS, and a value the operand
    /// stack holds is not what a re-entry finds.</b> The synchronous handler keeps the thrown value
    /// under the record and throws it three instructions later; here the close awaits, so the frame
    /// leaves and comes back at a height the handler no longer controls. A slot of the function's
    /// own environment is where every other value that has to outlive a jump already lives.
    /// </para>
    /// <para>
    /// <b>The close is wrapped in a region that SWALLOWS, and the swallowing is the specification's
    /// own.</b> <c>AsyncIteratorClose</c> under a throw completion reads <c>return</c>, calls it,
    /// awaits what it answered - and then discards every failure of those three, because the
    /// exception already travelling is the one the program is owed. Emitting a region rather than a
    /// quiet variant of <c>Await</c> is what keeps the suspension one opcode: the swallowing is
    /// control flow, and this format already expresses control flow.
    /// </para>
    /// <para>
    /// <b>The value check is NOT emitted here.</b> A <c>return</c> answering a primitive is a
    /// <c>TypeError</c> under a normal completion and is discarded under this one, which is why
    /// <c>IterateCloseCheck</c> appears on the <c>break</c> and <c>return</c> paths and not on this
    /// one.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=2D2442
    // Broiler-Human:        PENDING
    private void CompileAsyncUnwind(int record, int outerDepth)
    {
        var owner = FunctionScope();
        var parked = owner.Declare("#forawait" + owner.SlotCount, constant: false);
        EmitScoped(JsOpcode.InitialiseScoped, (byte)outerDepth, parked);

        var guarded = buffer.Code.Count;
        var swallow = NewLabel();
        var closed = NewLabel();
        var rethrow = NewLabel();

        buffer.PendingRegions.Add(
            new PendingRegion(guarded, swallow, outerDepth, JsFormat.HandlerKind.Catch));

        EmitScoped(JsOpcode.LoadScoped, (byte)outerDepth, record);
        Branch(JsOpcode.IterateCloseAsync, closed);
        Emit(JsOpcode.Await);
        Emit(JsOpcode.Pop);
        Mark(closed);
        buffer.CloseRegion(guarded, buffer.Code.Count);
        Branch(JsOpcode.Jump, rethrow);

        // THE HANDLER IS ENTERED WITH THE SWALLOWED VALUE AND NOTHING ELSE, at the height every
        // handler of this format is entered at, and it discards it. What is re-raised below is the
        // value that was parked, which is the exception the loop body actually threw.
        Mark(swallow);
        buffer.Rejoin(1);
        Emit(JsOpcode.Pop);
        Mark(rethrow);
        EmitScoped(JsOpcode.LoadScoped, (byte)outerDepth, parked);
        Emit(JsOpcode.Throw);
    }

    /// <summary>
    /// <c>AsyncIteratorClose</c> under a normal or <c>break</c>-shaped completion, inline.
    /// </summary>
    /// <remarks>
    /// <b>Every failure propagates here, where the handler above discards them all.</b> That is the
    /// one difference between the two closes and it is the specification's: a <c>break</c> out of a
    /// <c>for await</c> whose iterator's <c>return</c> rejects rejects the enclosing async
    /// function, and a <c>throw</c> out of the same loop does not.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=36BF98
    // Broiler-Human:        PENDING
    private void CompileAsyncClose(int record)
    {
        var closed = NewLabel();
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, record);
        Branch(JsOpcode.IterateCloseAsync, closed);
        Emit(JsOpcode.Await);
        Emit(JsOpcode.IterateCloseCheck);
        Mark(closed);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E0BD37
    // Broiler-Human:        PENDING
    private void DeclareHead(string name, JsPattern? pattern, bool constant)
    {
        if (pattern is null)
        {
            scope.Declare(name, constant);
            return;
        }

        DeclarePatternNames(pattern, constant);
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CB2015
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
            ProtectSomething(tryStart);
            buffer.CloseRegion(tryStart, buffer.Code.Count);
            Branch(JsOpcode.Jump, afterCatch);
            Mark(catchHandler);

            var outer = scope;
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var scopeSite = buffer.Code.Count + 1;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((scopeSite, scope));

            if (guarded.CatchPattern is not null)
            {
                DeclarePatternNames(guarded.CatchPattern, constant: false);
                BindPattern(guarded.CatchPattern, BindMode.Initialise);
            }
            else
            {
                var parameter = guarded.CatchParameter.Length == 0
                    ? scope.Declare("#caught", constant: false)
                    : scope.Declare(guarded.CatchParameter, constant: false);

                EmitScoped(JsOpcode.InitialiseScoped, 0, parameter);
            }

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

        ProtectSomething(finallyStart);
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

    /// <summary>
    /// Puts an instruction inside a protected range that would otherwise be empty.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b><c>try { } catch (e) { }</c> lowered to a region whose start equalled its end, and the
    /// verifier refused the artifact this lowering had just produced</b> — the same shape as
    /// [JSC-81], found the same way, by a program written to cover the surface rather than to
    /// confirm it. The verifier is right to refuse it: a region protecting no instruction is a
    /// region nothing can enter, and its handler is code the abstract pass would nonetheless seed
    /// as an entry, at a height nothing establishes.
    /// </para>
    /// <para>
    /// <b>So the lowering makes the range real rather than the verifier making the rule weaker.</b>
    /// The alternative considered was to emit no region and no handler for an empty block, and it
    /// is worse in a way that matters here: the handler's code would still be in the unit, reached
    /// by nothing, and an instruction stream carrying code no entry seeds is how unverified code
    /// gets into a verified artifact. A <c>Nop</c> costs one instruction in a block that had none,
    /// and only in that block — every non-empty <c>try</c> is unchanged.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=EAD16F
    // Broiler-Falsified-If: a region is emitted whose start offset equals its end offset
    // Broiler-Human:        PENDING
    private void ProtectSomething(int from)
    {
        if (buffer.Code.Count == from)
        {
            Emit(JsOpcode.Nop);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=654A6B
    // Broiler-Human:        PENDING
    private void CompileReturn(JsReturnStatement returned)
    {
        // THE QUESTION IS WHICH HOISTING SCOPE ENCLOSES THIS, NOT WHICH SCOPE IS CURRENT. A block
        // that declares a lexical name pushes a scope of its own, and so does a `with`, so asking
        // the current scope let `{ let a; return 1; }` at the top of a script through - and admitting
        // `with` would have added a second way in.
        if (FunctionScope().Kind == ScopeKind.Program)
        {
            Refuse(
                returned.Span,
                SliceSourceDiagnosticCode.ConstructOutsideManifest,
                "`return` outside a function is not admitted");

            return;
        }

        // A STATIC BLOCK COMPILES TO A FUNCTION AND IS NOT ONE, which is why the scope test above
        // does not catch this. The block's body is a code unit so that it can close over the class
        // scope and be called with the constructor as its `this`; nothing about it is a function a
        // program can return FROM, and the specification makes the word an early error there.
        if (insideStaticBlock)
        {
            Refuse(
                returned.Span,
                SliceSourceDiagnosticCode.IllegalBreak,
                "`return` has nothing to return from inside a class static block");

            return;
        }

        var unwinds = false;

        for (var index = exits.Count - 1; index >= 0 && !unwinds; index--)
        {
            unwinds = (exits[index].Kind == ExitKind.Finally && !exits[index].Running) ||
                exits[index].IteratorSlot >= 0;
        }

        if (!unwinds)
        {
            if (returned.Value is null)
            {
                Emit(JsOpcode.ReturnUndefined);
                return;
            }

            CompileExpression(returned.Value);
            AwaitTheReturnedValue();
            Emit(JsOpcode.Return);
            return;
        }

        // A `return` that has to unwind must run every enclosing finaliser and close every
        // enclosing `for … of` iterator before it leaves, so the value is parked in a slot of the
        // function's own environment while they run. Returning it on the operand stack instead
        // would reach `Return` with more than one value the moment a finaliser pushed anything,
        // which is what the verifier refuses.
        var function = FunctionScope();
        var slot = function.Declare("#return" + function.SlotCount, constant: false);

        if (returned.Value is null)
        {
            Emit(JsOpcode.LoadUndefined);
        }
        else
        {
            CompileExpression(returned.Value);
            AwaitTheReturnedValue();
        }

        EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, slot);
        var savedDepth = blockDepth;
        var savedScope = scope;

        for (var index = exits.Count - 1; index >= 0; index--)
        {
            Unwind(exits[index]);
        }

        Unwrap(0);
        EmitScoped(JsOpcode.LoadScoped, 0, slot);
        Emit(JsOpcode.Return);
        blockDepth = savedDepth;
        scope = savedScope;
    }

    /// <summary>
    /// Awaits what a <c>return</c> is carrying, in the one body kind where the language does.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is the ASYNC GENERATOR and not the async function, and the asymmetry is the
    /// language's.</b> <c>ReturnStatement : return Expression</c> awaits its value when
    /// <c>GetGeneratorKind()</c> is <c>async</c>, and that answer is <c>async</c> only inside an
    /// async generator: an ordinary async function has no generator component at all. So
    /// <c>async function* g() { return Promise.resolve(1); }</c> completes with <c>1</c> where
    /// <c>async function f() { return Promise.resolve(1); }</c> completes with the promise - and the
    /// second is invisible, because the promise the CALL answered adopts it either way.
    /// </para>
    /// <para>
    /// <b>The await happens before the unwinding and not after it</b>, because it belongs to the
    /// evaluation of the return statement rather than to the completion travelling out. A
    /// <c>finally</c> that observes the world therefore sees it after the value has settled, which
    /// is the same ordering <c>gen.return(p)</c> has for the same reason.
    /// </para>
    /// <para>
    /// A <c>return</c> with no expression is not on this path at all: it completes with
    /// <c>undefined</c>, which the language does not await and which has nothing to wait for.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3768F5
    // Broiler-Human:        PENDING
    private void AwaitTheReturnedValue()
    {
        const JsFormat.FunctionFlags both =
            JsFormat.FunctionFlags.Async | JsFormat.FunctionFlags.Generator;

        if ((buffer.Flags & both) == both)
        {
            Emit(JsOpcode.Await);
        }
    }

    /// <summary>Runs one enclosing exit's finaliser, or closes its iterator, on the way out.</summary>
    /// <remarks>
    /// The scopes between here and the exit are discarded first, because a finaliser's body and an
    /// iterator's slot are both addressed relative to the depth the exit was created at.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D7F0F6
    // Broiler-Human:        PENDING
    private void Unwind(Exit exit)
    {
        if (exit.Kind == ExitKind.Finally)
        {
            if (exit.Running)
            {
                return;
            }

            Unwrap(exit.Depth);
            exit.Running = true;
            CompileBlock(exit.Finaliser!, -1);
            exit.Running = false;
            return;
        }

        if (exit.IteratorSlot < 0)
        {
            return;
        }

        Unwrap(exit.Depth);

        if (exit.IteratorIsAsync)
        {
            CompileAsyncClose(exit.IteratorSlot);
            return;
        }

        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, exit.IteratorSlot);
        Emit(JsOpcode.IterateClose, (byte)0);
    }

    /// <summary>
    /// Discards scopes down to <paramref name="depth"/>, at COMPILE time as well as at run time.
    /// </summary>
    /// <remarks>
    /// <b>The compile-time scope has to move with the counter, and it did not.</b> Every unwinding
    /// path emitted <c>PopScope</c> and decremented <c>blockDepth</c> while leaving <c>scope</c>
    /// pointing at the block it had just discarded - so a name a finaliser read afterwards was
    /// resolved one hop too far out. <c>function f() { try { throw x; } catch (e) { return 1; }
    /// finally { outer.push(1); } }</c> is the shortest witness: <c>outer</c> was resolved against
    /// the catch scope that had already been popped, and the finaliser read whatever slot of that
    /// index the grandparent environment held. It compiled, it verified, and it answered the wrong
    /// value - which is the failure mode a stack-height check cannot see.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A5998A
    // Broiler-Human:        PENDING
    private void Unwrap(int depth)
    {
        while (blockDepth > depth && scope.Parent is not null)
        {
            Emit(JsOpcode.PopScope);
            blockDepth--;
            scope = scope.Parent;
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6C86B0
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
        var savedScope = scope;

        for (var index = exits.Count - 1; index > targetAt; index--)
        {
            Unwind(exits[index]);
        }

        Unwrap(target.Depth);

        // `break` LEAVES THE LOOP AND `continue` DOES NOT, and that is the whole of why the target
        // is treated differently from the loops passed on the way. A `continue` that closed the
        // iterator would end the loop it was asked to keep going.
        if (!wantsContinue && target.IteratorSlot >= 0)
        {
            if (target.IteratorIsAsync)
            {
                CompileAsyncClose(target.IteratorSlot);
            }
            else
            {
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, target.IteratorSlot);
                Emit(JsOpcode.IterateClose, (byte)0);
            }
        }

        Branch(JsOpcode.Jump, wantsContinue ? target.Continue! : target.Break!);
        blockDepth = savedDepth;
        scope = savedScope;
    }

    // ---- expressions ---------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6F386E
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
                CompileRegExpLiteral(pattern);
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

            case JsClassExpression definition:
                CompileClass(definition.Class, string.Empty);
                break;

            case JsSuperMemberExpression member:
                CompileSuperKey(member);
                Emit(JsOpcode.LoadSuperProperty);
                break;

            case JsSuperCallExpression call:
                CompileSuperCall(call);
                break;

            case JsNewTargetExpression target:
                // `new.target` OUTSIDE A FUNCTION IS A SYNTAX ERROR AND NOT A MANIFEST REFUSAL.
                // The manifest admits it perfectly well; the program wrote it where the language
                // has nothing for it to mean, and the diagnostic code is what tells a conformance
                // runner which of the two happened.
                if (!insideFunction)
                {
                    Refuse(
                        target.Span,
                        SliceSourceDiagnosticCode.UnexpectedToken,
                        "`new.target` is only admitted inside a function");

                    Emit(JsOpcode.LoadUndefined);
                    break;
                }

                Emit(JsOpcode.LoadNewTarget);
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

            // A PRIVATE READ IS NOT A PROPERTY READ AND THE ABSENT CASE IS WHY. `o.x` answers
            // `undefined` for a name nothing defined; `o.#x` on an object the declaring class never
            // constructed is a TypeError, because a private name is not a key that object could
            // have had.
            case JsPrivateMemberExpression privateAccess:
                CompileExpression(privateAccess.Target);
                EmitPrivateName(privateAccess.Span, privateAccess.Name);
                Emit(JsOpcode.LoadPrivate);
                break;

            case JsPrivateInExpression brand:
                CompileExpression(brand.Target);
                EmitPrivateName(brand.Span, brand.Name);
                Emit(JsOpcode.HasPrivate);
                break;

            case JsCallExpression call:
                CompileCall(call);
                break;

            case JsNewExpression construction:
            {
                CompileExpression(construction.Callee);

                if (HasSpread(construction.Arguments))
                {
                    CompileArgumentArray(construction.Arguments);
                    Emit(JsOpcode.ConstructSpread);
                    break;
                }

                foreach (var argument in construction.Arguments)
                {
                    CompileExpression(argument);
                }

                if (construction.Arguments.Count > 255)
                {
                    Refuse(
                        construction.Span,
                        SliceSourceDiagnosticCode.ConstructOutsideManifest,
                        "a construction with more than 255 arguments is not admitted");
                }

                Emit(
                    JsOpcode.Construct,
                    (byte)System.Math.Min(construction.Arguments.Count, 255));

                break;
            }

            // A DESTRUCTURING ASSIGNMENT'S VALUE IS THE RIGHT-HAND SIDE AND NOT THE PATTERN'S
            // RESULT. `print([a, b] = [1, 2])` prints the array, so the value is duplicated before
            // the pattern consumes its copy.
            case JsDestructuringAssignment destructuring:
                CompileExpression(destructuring.Value);
                Emit(JsOpcode.Duplicate);
                BindPattern(destructuring.Target, BindMode.Assign);
                break;

            case JsTemplateLiteral template:
                CompileTemplate(template);
                break;

            case JsTaggedTemplate tagged:
                CompileTaggedTemplate(tagged);
                break;

            case JsChainExpression chain:
                CompileChain(chain);
                break;

            // `yield` LEAVES ONE VALUE WHERE IT TOOK ONE. The operand is pushed and the opcode
            // replaces it with whatever the resumption sent, so a `yield` in the middle of an
            // expression needs nothing around it - which is what lets it appear in an argument
            // list, a loop condition or an object literal without the lowering knowing where it is.
            case JsYieldExpression yielded:
                if (yielded.Operand is null)
                {
                    Emit(JsOpcode.LoadUndefined);
                }
                else
                {
                    CompileExpression(yielded.Operand);
                }

                // A `yield` IN AN ASYNC GENERATOR AWAITS ITS OPERAND FIRST, and it is two
                // instructions rather than a mode on one because the awaiting is a SUSPENSION with
                // its own resumption. `Yield(v)` in the specification is
                // `AsyncGeneratorYield(? Await(v))` when the generator is async - so
                // `yield Promise.resolve(1)` hands the consumer `1` and not the promise, which is
                // the single most visible difference between the two kinds of generator. The
                // `Await` leaves what it resolved on the stack and the `Yield` takes it from there,
                // so the pair needs no temporary and no new opcode.
                if (yielded.IsDelegate)
                {
                    Emit(JsOpcode.YieldDelegate);
                    break;
                }

                if ((buffer.Flags & JsFormat.FunctionFlags.Async) != 0)
                {
                    Emit(JsOpcode.Await);
                }

                Emit(JsOpcode.Yield);
                break;

            // `await` LEAVES ONE VALUE WHERE IT TOOK ONE, exactly as `yield` does, so it needs
            // nothing around it either and may stand anywhere an expression may.
            case JsAwaitExpression awaited:
                CompileExpression(awaited.Operand);

                // AND THE UNIT IS CHECKED HERE, where the alternative is the worst failure shape
                // this component has: an `Await` in a unit carrying no async flag is refused by
                // THIS HOST'S OWN VERIFIER, on bytes THIS HOST'S OWN lowering produced, which is
                // the internal-consistency failure roadmap section 3.4 names. The parser's
                // `[Await]` contexts are what make this unreachable; this is what makes it a
                // diagnostic naming the construct rather than a refused artifact if they ever stop.
                if ((buffer.Flags & JsFormat.FunctionFlags.Async) == 0)
                {
                    Refuse(
                        awaited.Span,
                        SliceSourceDiagnosticCode.ConstructOutsideManifest,
                        "`await` is only admitted inside an async function");

                    break;
                }

                Emit(JsOpcode.Await);
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

    /// <summary>Binds a class declaration's name after building the class.</summary>
    /// <remarks>
    /// The binding is mutable, which a reader who has just seen the class's own binding declared
    /// constant will want explained: they are two different bindings. <c>class C { }</c> introduces
    /// <c>C</c> in the enclosing scope as an ordinary <c>let</c>, and separately introduces a
    /// constant <c>C</c> that only the class body can see - which is why <c>C = 1</c> after the
    /// declaration is fine and <c>C = 1</c> inside a method is not.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=38B06E
    // Broiler-Human:        PENDING
    private void CompileClassDeclaration(JsClassDeclaration declaration)
    {
        var name = declaration.Class.Name;
        CompileClass(declaration.Class, name);

        if (scope.Kind == ScopeKind.Program && blockDepth == 0)
        {
            Emit(
                programLexicals.ContainsKey(name)
                    ? JsOpcode.InitialiseGlobalLexical
                    : JsOpcode.StoreGlobal,
                InternedName(name));

            return;
        }

        var slot = scope.Has(name) ? scope.SlotOf(name) : scope.Declare(name, constant: false);
        EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
    }

    /// <summary>
    /// Lowers a class, leaving the constructor on the operand stack.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The shape is: a scope, the heritage, the constructor, the class, then the members.</b>
    /// The scope holds the class's own name and is pushed FIRST, because every closure the body
    /// creates - the constructor included - has to capture it; a scope pushed after the constructor
    /// closure would leave <c>C</c> unresolvable inside <c>C</c>'s own methods. It is popped at the
    /// end, and popping it destroys nothing: the closures captured the record itself.
    /// </para>
    /// <para>
    /// <b>The class binding is initialised LAST, after every member has been defined</b>, and that
    /// ordering is observable: a computed member key that names the class reads a binding still in
    /// its dead zone and throws, which is what the specification asks for and what a lowering that
    /// initialised it early would answer <c>undefined</c> to.
    /// </para>
    /// <para>
    /// While the members are defined the stack holds the constructor and the prototype, in that
    /// order, so a prototype member needs no reload and a static member is one
    /// <see cref="JsOpcode.Pick"/> away. The alternative - reading <c>C.prototype</c> before each
    /// prototype member - would be a property lookup per member for no gain, and it would go
    /// through a property this instruction set deliberately makes non-writable.
    /// <b>That pair is also what <see cref="JsOpcode.DefineClassElement"/> reads</b>, which is why
    /// a field costs no reload either.
    /// </para>
    /// <para>
    /// <b>A class body has FOUR times in it and not one, and every ordering rule below is one of
    /// them.</b> The private names are minted first, because a method compiled after them has to
    /// capture the slots they live in. Then every key in the body is evaluated, in source order,
    /// including the keys of static fields whose initialisers have not run. Then the class binding
    /// is initialised. Only then do the static initialisers and blocks run, which is what lets
    /// <c>static { C.tag = 1 }</c> name the class and stops
    /// <c>static [C.name] = 1</c> from doing so. A lowering that performed a static field where it
    /// was written would have collapsed the middle two and been wrong about both.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=DF7518
    // Broiler-Human:        PENDING
    private void CompileClass(JsClassNode node, string inferredName)
    {
        var outer = scope;
        var named = node.Name.Length != 0;
        var privates = PrivateNamesOf(node);

        // THE SCOPE EXISTS FOR THE PRIVATE NAMES TOO AND NOT ONLY FOR THE CLASS'S OWN BINDING. An
        // anonymous class with a `#x` in it has nowhere else to keep the name, and the name has to
        // be somewhere every method of the body captures - which is the same requirement the class
        // binding has and is met by the same record.
        var scoped = named || privates.Count != 0;

        if (scoped)
        {
            scope = new Scope(ScopeKind.Block, outer);
            blockDepth++;
            var site = buffer.Code.Count + 1;
            Emit(JsOpcode.PushScope, (ushort)0);
            buffer.ScopeSites.Add((site, scope));

            if (named)
            {
                scope.Declare(node.Name, constant: true);
            }

            foreach (var privateName in privates)
            {
                var slot = scope.Declare(PrivateSlot(privateName), constant: true);
                Emit(JsOpcode.NewPrivateName, StringConstant(privateName));
                EmitScoped(JsOpcode.InitialiseScoped, 0, slot);
            }
        }

        if (node.HasHeritage)
        {
            CompileExpression(node.Heritage!);
        }

        var name = named ? node.Name : inferredName;
        var constructor = FindConstructor(node);

        var flags = JsFormat.FunctionFlags.ClassConstructor | JsFormat.FunctionFlags.Constructible |
            (node.HasHeritage ? JsFormat.FunctionFlags.DerivedConstructor : JsFormat.FunctionFlags.None);

        var unit = constructor is null
            ? CompileImplicitConstructor(node.Span, name, node.HasHeritage, flags)
            : CompileFunction(
                constructor with { Name = name }, flags, isMethod: true, isDerived: node.HasHeritage);

        Emit(JsOpcode.Closure, (ushort)unit);
        Emit(JsOpcode.NewClass, (byte)(node.HasHeritage ? JsOpcodes.ClassIsDerived : 0));

        var defines = node.Members.Count != 0 &&
            (constructor is null || node.Members.Count != 1);

        if (defines)
        {
            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.GetProperty, InternedName("prototype"));
        }

        var statics = false;

        foreach (var member in node.Members)
        {
            if (member.Function is not null && ReferenceEquals(member.Function, constructor))
            {
                continue;
            }

            Position(member.Span);

            if (member.Kind is JsMethodKind.Field or JsMethodKind.StaticBlock || member.IsPrivate)
            {
                statics |= member.IsStatic;
                CompileClassElement(member);
                continue;
            }

            if (member.IsStatic)
            {
                Emit(JsOpcode.Pick, (byte)1);
            }

            if (member.Computed is null)
            {
                Emit(JsOpcode.LoadConstant, StringConstant(member.Key));
            }
            else
            {
                CompileExpression(member.Computed);
            }

            Emit(JsOpcode.Closure, (ushort)CompileFunction(member.Function!, isMethod: true));
            Emit(JsOpcode.DefineMethod, MemberOperand(member.Kind, enumerable: false));

            if (member.IsStatic)
            {
                Emit(JsOpcode.Pop);
            }
        }

        if (defines)
        {
            Emit(JsOpcode.Pop);
        }

        if (named)
        {
            Emit(JsOpcode.Duplicate);
            EmitScoped(JsOpcode.InitialiseScoped, 0, scope.SlotOf(node.Name));
        }

        // THE STATIC ELEMENTS RUN AFTER THE BINDING AND BEFORE THE SCOPE GOES, and both halves of
        // that matter. A static block that names the class needs the binding initialised; a static
        // block that reads a private name needs the record the names live in still to be the
        // innermost one, because the block's own closure resolves them by hop count.
        if (statics)
        {
            Emit(JsOpcode.RunStaticElements);
        }

        if (!scoped)
        {
            return;
        }

        Emit(JsOpcode.PopScope);
        blockDepth--;
        scope = outer;
    }

    /// <summary>
    /// Lowers one field, private member or static block into a record on the constructor.
    /// </summary>
    /// <remarks>
    /// <b>Every arm leaves the stack exactly as it found it</b>, which is what lets the caller run
    /// a whole class body over the one constructor-and-prototype pair it loaded once. The key and
    /// the initialiser are pushed and consumed by the one instruction, and the pair beneath them is
    /// read rather than popped.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=21D83F
    // Broiler-Human:        PENDING
    private void CompileClassElement(JsClassMember member)
    {
        var flags = (byte)(
            (member.IsStatic ? JsOpcodes.ElementIsStatic : 0) |
            (member.IsPrivate ? JsOpcodes.ElementIsPrivate : 0));

        if (member.Kind == JsMethodKind.StaticBlock)
        {
            RefuseArguments(member.Function!);
            Emit(JsOpcode.LoadUndefined);

            Emit(
                JsOpcode.Closure,
                (ushort)CompileFunction(member.Function!, isMethod: true, isStaticBlock: true));

            Emit(JsOpcode.DefineClassElement, (byte)(flags | JsOpcodes.ElementIsBlock));
            return;
        }

        if (member.IsPrivate)
        {
            EmitPrivateName(member.Span, member.Key);
        }
        else if (member.Computed is null)
        {
            Emit(JsOpcode.LoadConstant, StringConstant(member.Key));
        }
        else
        {
            CompileExpression(member.Computed);
        }

        if (member.Kind != JsMethodKind.Field)
        {
            flags |= member.Kind switch
            {
                JsMethodKind.Get => (byte)(JsOpcodes.ElementIsMethod | JsOpcodes.ElementIsGetter),
                JsMethodKind.Set => (byte)(JsOpcodes.ElementIsMethod | JsOpcodes.ElementIsSetter),
                _ => JsOpcodes.ElementIsMethod,
            };

            Emit(JsOpcode.Closure, (ushort)CompileFunction(member.Function!, isMethod: true));
            Emit(JsOpcode.DefineClassElement, flags);
            return;
        }

        // A FIELD WITH NO INITIALISER IS `undefined` AND NOT AN ABSENT FIELD. `class C { x }`
        // defines `x` on every instance, so the field is recorded with no initialiser rather than
        // not recorded - and the executor tells the two apart by what is pushed here.
        if (member.Function is null)
        {
            Emit(JsOpcode.LoadUndefined);
        }
        else
        {
            RefuseArguments(member.Function);
            Emit(JsOpcode.Closure, (ushort)CompileFunction(member.Function, isMethod: true));
        }

        Emit(JsOpcode.DefineClassElement, flags);
    }

    /// <summary>
    /// Refuses an <c>arguments</c> anywhere inside a field initialiser or a static block.
    /// </summary>
    /// <remarks>
    /// <b>Neither has an <c>arguments</c> of its own and neither may borrow one</b>, which is what
    /// makes this an early error rather than a read of the enclosing function's object. Both run
    /// with a <c>this</c> the class body does not have and with no argument list at all, so the
    /// specification makes the WORD a Syntax Error there rather than leaving a program to discover
    /// at run time that the object it named is somebody else's. An arrow inside one is included,
    /// because an arrow has no <c>arguments</c> either and reaches outward for it; an ordinary
    /// nested function is not, because it has one of its own - which is exactly the boundary the
    /// walk this calls already draws for the enclosing function's own materialisation question.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=21BAC5
    // Broiler-Human:        PENDING
    private void RefuseArguments(JsFunctionNode body)
    {
        foreach (var statement in body.Body)
        {
            if (!Walk.Mentions(statement, "arguments"))
            {
                continue;
            }

            Refuse(
                body.Span,
                SliceSourceDiagnosticCode.UnresolvableIdentifier,
                "`arguments` names nothing inside a class field initialiser or a static block");

            return;
        }
    }

    /// <summary>
    /// Every private name one class body declares, in source order and once each.
    /// </summary>
    /// <remarks>
    /// <b><c>get #a</c> and <c>set #a</c> declare ONE name and not two</b>, which is why this
    /// de-duplicates rather than counting members. Two slots would have made the setter write an
    /// element the getter could not see, and the brand check would then answer differently
    /// depending on which half a program happened to ask through.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=14A539
    // Broiler-Human:        PENDING
    private static System.Collections.Generic.List<string> PrivateNamesOf(JsClassNode node)
    {
        var found = new System.Collections.Generic.List<string>();

        foreach (var member in node.Members)
        {
            if (member.IsPrivate && !found.Contains(member.Key))
            {
                found.Add(member.Key);
            }
        }

        return found;
    }

    /// <summary>Pushes the private name a class body in scope declared under this spelling.</summary>
    /// <remarks>
    /// <b>It resolves through the ordinary scope chain, so the INNERMOST class that declares the
    /// spelling wins</b> - which is what the specification's PrivateEnvironment says, and what makes
    /// a nested class able to declare its own <c>#x</c> without disturbing the outer one's. A
    /// spelling no enclosing class declared is a refusal here and not a run-time absence, because
    /// there is no object a name nobody minted could be found on.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FEA545
    // Broiler-Human:        PENDING
    private void EmitPrivateName(SliceSourceSpan span, string name)
    {
        if (TryResolve(PrivateSlot(name), out var hops, out var slot, out _))
        {
            EmitScoped(JsOpcode.LoadScoped, (byte)hops, slot);
            return;
        }

        Refuse(
            span,
            SliceSourceDiagnosticCode.UnresolvableIdentifier,
            "`" + name + "` is not declared by any class this expression is inside of");

        Emit(JsOpcode.LoadUndefined);
    }

    /// <summary>The slot name a private name is kept under.</summary>
    /// <remarks>
    /// <b>The second <c>#</c> is what keeps a private name apart from the lowering's own
    /// temporaries.</b> A compound assignment declares a slot called <c>#target0</c> in the
    /// enclosing FUNCTION scope, which is inside the class scope a private name lives in, so a
    /// class that declared <c>#target0</c> - a perfectly ordinary private name - would have had its
    /// name shadowed by a temporary and every access would have read whatever the last assignment
    /// left there. No private name can begin with a second <c>#</c>, because an identifier cannot,
    /// so this spelling is one no source can collide with.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6DE4AA
    // Broiler-Human:        PENDING
    private static string PrivateSlot(string name) => "#" + name;

    /// <summary>The class body's own <c>constructor</c>, when it wrote one.</summary>
    /// <remarks>
    /// A STATIC member called <c>constructor</c> is not it, and neither is a getter of that name
    /// nor a computed key that happens to evaluate to the string: the specification decides this
    /// syntactically, on a non-static, non-computed method whose property name is
    /// <c>constructor</c>, and so does this.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6BC9DC
    // Broiler-Human:        PENDING
    private static JsFunctionNode? FindConstructor(JsClassNode node)
    {
        foreach (var member in node.Members)
        {
            if (!member.IsStatic &&
                !member.IsPrivate &&
                member.Kind == JsMethodKind.Method &&
                member.Computed is null &&
                string.Equals(member.Key, "constructor", System.StringComparison.Ordinal))
            {
                return member.Function;
            }
        }

        return null;
    }

    /// <summary>Builds the constructor a class body did not write.</summary>
    /// <remarks>
    /// <b>There is no syntax tree behind this unit, and there could not be.</b> A base class's
    /// implicit constructor is <c>constructor() { }</c>, which a tree could express; a derived
    /// class's is <c>constructor(...args) { super(...args); }</c>, and this manifest admits neither
    /// a rest parameter nor a spread argument - both are refused by name. Rather than admit half of
    /// each to write one function nobody typed, the forwarding is an instruction.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=432C40
    // Broiler-Human:        PENDING
    private int CompileImplicitConstructor(
        SliceSourceSpan span, string name, bool derived, JsFormat.FunctionFlags flags)
    {
        var outerBuffer = buffer;
        var outerScope = scope;
        var outerDepth = blockDepth;
        var outerStrict = strict;

        strict = true;
        var index = units.Count;
        var constant = name.Length == 0 ? (ushort)0 : (ushort)(InternedName(name) + 1);
        buffer = new UnitBuffer(constant, flags | JsFormat.FunctionFlags.Strict);
        units.Add(buffer);
        scope = new Scope(ScopeKind.Function, outerScope);
        blockDepth = 0;
        Position(span);

        if (derived)
        {
            Emit(JsOpcode.SuperCallForwarded);
            Emit(JsOpcode.Pop);
        }

        Emit(JsOpcode.ReturnUndefined);
        buffer.SlotCount = scope.SlotCount;
        buffer = outerBuffer;
        scope = outerScope;
        blockDepth = outerDepth;
        strict = outerStrict;
        return index;
    }

    /// <summary>The <see cref="JsOpcode.DefineMethod"/> operand for one member.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CBC5E3
    // Broiler-Human:        PENDING
    private static byte MemberOperand(JsMethodKind kind, bool enumerable) => (byte)(
        (kind switch
        {
            JsMethodKind.Get => JsOpcodes.MemberIsGetter,
            JsMethodKind.Set => JsOpcodes.MemberIsSetter,
            _ => 0,
        }) | (enumerable ? JsOpcodes.MemberIsEnumerable : 0));

    /// <summary>
    /// Lowers a value whose name the language takes from what it is being bound to.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>const C = class { };</c> gives the class the name <c>C</c>, and <c>var f = function () {}</c>
    /// gives the function the name <c>f</c>, neither of which the text contains. It is done here
    /// rather than in the executor because the name is baked into the code unit, and a unit belongs
    /// to exactly one syntactic site - so the name a site infers is the name every closure over that
    /// site has.
    /// </para>
    /// <para>
    /// <b>The closure is emitted here rather than through <see cref="CompileFunctionExpression"/>,
    /// and the difference is a binding.</b> A function expression with a name in its TEXT binds that
    /// name inside its own body: <c>var f = function g () { return g; }</c> can see <c>g</c>. An
    /// inferred name is not that - <c>var f = function () { f = 1; }</c> assigns the outer <c>f</c> -
    /// so the unit is named without the surrounding scope that a written name creates.
    /// </para>
    /// <para>
    /// <b>A name is inferred only where the language infers one.</b> The positions are the ones that
    /// call this: a declarator, an assignment to a name, a member of an object literal, and a default
    /// - for a parameter or inside a pattern. <c>o.p = function () {}</c> is NOT one of them and its
    /// function is anonymous, which is the case a reader is most likely to expect here and be wrong
    /// about.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D0EF9A
    // Broiler-Human:        PENDING
    private void CompileNamedValue(JsExpression value, string inferred)
    {
        if (inferred.Length == 0)
        {
            CompileExpression(value);
            return;
        }

        if (value is JsClassExpression anonymous && anonymous.Class.Name.Length == 0)
        {
            Position(value.Span);
            CompileClass(anonymous.Class, inferred);
            return;
        }

        if (value is JsFunctionExpression function && function.Function.Name.Length == 0)
        {
            Position(value.Span);
            Emit(JsOpcode.Closure, (ushort)CompileFunction(function.Function with { Name = inferred }));
            return;
        }

        CompileExpression(value);
    }

    /// <summary>The name a pattern's leaf infers for an anonymous default, or the empty string.</summary>
    /// <remarks>
    /// <b>Only a leaf that is one NAME infers anything.</b> <c>[a = function () {}]</c> names the
    /// function <c>a</c>; <c>[o.p = function () {}]</c> names nothing, and neither does a leaf that is
    /// itself a pattern - there is no one name for the default to take.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=27021F
    // Broiler-Human:        PENDING
    private static string InferredFrom(JsPattern target) =>
        target is JsTargetPattern { Target: JsIdentifier name } ? name.Name : string.Empty;

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=873B5E
    // Broiler-Human:        PENDING
    private void CompileArray(JsArrayLiteral array)
    {
        var dense = true;

        foreach (var element in array.Elements)
        {
            if (element is null or JsSpreadElement)
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

        // EVERY OTHER SHAPE GOES THROUGH THE APPENDING PATH, holes and spreads alike, and the
        // sparse path this replaced was a JSC-81-shaped defect: it set `length` with SetProperty,
        // which pops the Array as well as the value and pushes only the value back, so `[1, , 3]`
        // left NOTHING on the operand stack and the verifier refused the whole artifact. That the
        // verifier caught it is the good outcome; that a plain elision reached it at all is what
        // this repair is for.
        CompileSpreadArray(array.Elements);
    }

    /// <summary>
    /// Builds an Array whose length is not known until it has been built.
    /// </summary>
    /// <remarks>
    /// <b>Every index after a spread is dynamic, so the whole literal switches to appending.</b>
    /// The index of <c>b</c> in <c>[a, ...xs, b]</c> depends on how many values <c>xs</c> yielded,
    /// and holes still have to count: <c>[...xs, , b]</c> leaves one behind. Emitting a constant
    /// index for the elements before the first spread and appending after it would have been
    /// half a lowering with a seam in the middle for no gain.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=772EAF
    // Broiler-Human:        PENDING
    private void CompileSpreadArray(
        System.Collections.Generic.IReadOnlyList<JsExpression?> elements)
    {
        Emit(JsOpcode.NewArray, (ushort)0);
        var holes = 0;

        foreach (var element in elements)
        {
            if (element is null)
            {
                holes++;
                continue;
            }

            if (holes != 0)
            {
                Emit(JsOpcode.ArrayHoles, (ushort)holes);
                holes = 0;
            }

            if (element is JsSpreadElement spread)
            {
                CompileExpression(spread.Argument);
                Emit(JsOpcode.SpreadArray);
                continue;
            }

            CompileExpression(element);
            Emit(JsOpcode.ArrayAppend);
        }

        if (holes != 0)
        {
            // TRAILING HOLES ARE STILL LENGTH, and `ArrayHoles` is how they are said. The
            // sparse path this replaced set `length` with `SetProperty`, which pops the Array as
            // well as the value and pushes only the value back, so the literal left NOTHING on the
            // operand stack and the verifier refused the artifact *(corrected: JSC-81)*. One
            // instruction that grows the Array without defining an element has no such seam.
            Emit(JsOpcode.ArrayHoles, (ushort)holes);
        }
    }

    /// <summary>Whether an argument list carries a spread, so the count is not a constant.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3FEE9
    // Broiler-Human:        PENDING
    private static bool HasSpread(
        System.Collections.Generic.IReadOnlyList<JsExpression> arguments)
    {
        foreach (var argument in arguments)
        {
            if (argument is JsSpreadElement)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Builds the one Array a spread call or a spread construction passes its arguments in.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5BBE2C
    // Broiler-Human:        PENDING
    private void CompileArgumentArray(
        System.Collections.Generic.IReadOnlyList<JsExpression> arguments)
    {
        Emit(JsOpcode.NewArray, (ushort)0);

        foreach (var argument in arguments)
        {
            if (argument is JsSpreadElement spread)
            {
                CompileExpression(spread.Argument);
                Emit(JsOpcode.SpreadArray);
                continue;
            }

            CompileExpression(argument);
            Emit(JsOpcode.ArrayAppend);
        }
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=2D1356
    // Broiler-Human:        PENDING
    private void CompileObject(JsObjectLiteral literal)
    {
        Emit(JsOpcode.NewObject);

        var prototyped = false;

        foreach (var entry in literal.Entries)
        {
            // A `{ a = 1 }` THAT REACHED THE LOWERING WAS NEVER A PATTERN. The parser could not
            // tell an object literal from the cover grammar of an assignment pattern until the
            // token after the brace, so it marked the shape and let it through; whatever arrives
            // here is the half that no `=` followed, and it is a syntax error.
            if (entry.Cover)
            {
                Refuse(
                    entry.Span,
                    SliceSourceDiagnosticCode.UnexpectedToken,
                    "`=` is not an object literal entry outside a destructuring assignment");

                continue;
            }

            if (entry.Kind == JsPropertyKind.Spread)
            {
                CompileExpression(entry.Value);
                Emit(JsOpcode.SpreadObject);
                continue;
            }

            // A METHOD AND A PROPERTY WHOSE VALUE IS A FUNCTION ARE DIFFERENT OBJECTS. `{ m() {} }`
            // makes a method - it has a home object, so `super` inside it resolves, and it is not
            // a constructor - and `{ m: function () {} }` makes an ordinary function. They were
            // one lowering until classes were admitted, which was invisible only because nothing
            // could ask a function for its home object.
            if (entry.IsMethod)
            {
                if (entry.Computed is null)
                {
                    Emit(JsOpcode.LoadConstant, StringConstant(entry.Key));
                }
                else
                {
                    CompileExpression(entry.Computed);
                }

                CompileMethodValue(entry.Value);

                Emit(
                    JsOpcode.DefineMethod,
                    MemberOperand(
                        entry.Kind switch
                        {
                            JsPropertyKind.Get => JsMethodKind.Get,
                            JsPropertyKind.Set => JsMethodKind.Set,
                            _ => JsMethodKind.Method,
                        },
                        enumerable: true));

                continue;
            }

            if (entry.Computed is not null)
            {
                CompileExpression(entry.Computed);
                CompileExpression(entry.Value);
                Emit(JsOpcode.DefineIndexed);
                continue;
            }

            // `__proto__: p` IS THE ONE MEMBER THAT IS NOT A MEMBER. The language spells it like a
            // property and gives it a different meaning - it sets the prototype - and the three
            // spellings that do NOT mean that are all excluded above or here: a computed key, a
            // method, and the shorthand. Writing it twice is a syntax error, because a literal that
            // set its prototype twice would have an order nobody could read off the source.
            if (!entry.Shorthand && string.Equals(entry.Key, "__proto__", System.StringComparison.Ordinal))
            {
                if (prototyped)
                {
                    Refuse(
                        entry.Span,
                        SliceSourceDiagnosticCode.UnexpectedToken,
                        "an object literal may set `__proto__` once");

                    continue;
                }

                prototyped = true;
                CompileExpression(entry.Value);
                Emit(JsOpcode.SetPrototypeLiteral);
                continue;
            }

            CompileNamedValue(entry.Value, entry.Key);
            Emit(JsOpcode.DefineField, InternedName(entry.Key));
        }
    }

    /// <summary>Emits a closure for a member written in method form.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E977A0
    // Broiler-Human:        PENDING
    private void CompileMethodValue(JsExpression value)
    {
        if (value is JsFunctionExpression method)
        {
            Emit(JsOpcode.Closure, (ushort)CompileFunction(method.Function, isMethod: true));
            return;
        }

        CompileExpression(value);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D1EE4A
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
                if (Shadowable(name.Name, out var absent))
                {
                    // `with (o) { typeof x }` is the object's value when it has one and the absent
                    // global's `"undefined"` when nothing has it, so the search's fall-back arm is
                    // the read that does not throw.
                    EmitDynamicName(name.Name, absent, wantsBase: false, orUndefined: true);
                }
                else
                {
                    Emit(JsOpcode.LoadGlobalOrUndefined, InternedName(name.Name));
                }

                Emit(JsOpcode.TypeOf);
                return;

            // `delete x` INSIDE A `with` BODY DELETES A PROPERTY WHEN THE OBJECT HAS THE NAME, which
            // is the one spelling of `delete` that reaches an environment record at all. When no
            // object on the chain has it the answer is about a binding rather than about a property:
            // a slot binding is not configurable and the language answers `false`, which is what the
            // fall-back arm pushes. The operand is NOT evaluated on either path, because `delete` of
            // a reference never evaluates it.
            case SliceTokenKind.Delete when unary.Operand is JsIdentifier bare &&
                Shadowable(bare.Name, out var reachable):
            {
                var live = buffer.Height;
                var key = InternedName(bare.Name);
                var enclosing = NewLabel();
                var settled = NewLabel();

                EmitResolve(reachable, key);
                Emit(JsOpcode.Duplicate);
                Branch(JsOpcode.JumpIfFalse, enclosing);
                Emit(JsOpcode.DeleteProperty, key);
                Branch(JsOpcode.Jump, settled);

                Mark(enclosing);
                Emit(JsOpcode.Pop);
                if (Resolvable(bare.Name))
                {
                    Emit(JsOpcode.LoadFalse);
                }
                else
                {
                    Emit(JsOpcode.DeleteGlobalBinding, InternedName(bare.Name));
                }
                Mark(settled);
                buffer.Rejoin(live + 1);
                return;
            }

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

            // `delete a?.b` DELETES, AND WHEN `a` IS NULLISH IT ANSWERS `true`. The chain is not
            // evaluated to a value and then deleted - that would delete nothing and answer `true`
            // for a reason the language does not give. It is the same chain lowering with the
            // deletion as its last link and `true` as what its short circuit produces, which is
            // exactly what the specification says a short-circuited `delete` completes with.
            case SliceTokenKind.Delete when unary.Operand is JsChainExpression chain &&
                chain.Chain is JsMemberExpression optional:
            {
                var end = NewLabel();
                EmitChainLink(optional.Target, end, shortIsTrue: true);

                if (optional.Optional)
                {
                    EmitNullishGuard(end, held: 1, shortIsTrue: true);
                }

                if (optional.Computed is null)
                {
                    Emit(JsOpcode.DeleteProperty, InternedName(optional.Name));
                }
                else
                {
                    CompileExpression(optional.Computed);
                    Emit(JsOpcode.DeleteIndex);
                }

                Mark(end);
                return;
            }

            // `delete o.#x` IS A SYNTAX ERROR AND NOT A DELETION THAT ANSWERS `true`. A private
            // element is not a property and there is no operation that removes one, so the language
            // refuses the spelling rather than giving it a reading. Falling through to the arm
            // below would have evaluated the access - throwing on a foreign object - and then
            // answered `true` for a deletion that never happened.
            case SliceTokenKind.Delete when unary.Operand is JsPrivateMemberExpression:
                Refuse(
                    unary.Span,
                    SliceSourceDiagnosticCode.InvalidAssignmentTarget,
                    "a private element cannot be deleted");

                Emit(JsOpcode.LoadTrue);
                return;

            // `delete x` FOR A BARE NAME NEVER EVALUATES `x`. The operator takes a reference and
            // asks whether it can be removed, so a lowering that read the name and threw the value
            // away answered `true` for a `var` the language says is not deletable and threw a
            // `ReferenceError` for a name nobody declared - where the language answers `true`. A
            // name this unit resolves to a SLOT is answered here, because a slot binding is never
            // deletable and the compiler already knows which names those are.
            case SliceTokenKind.Delete when unary.Operand is JsIdentifier plain:
                if (Resolvable(plain.Name))
                {
                    Emit(JsOpcode.LoadFalse);
                    return;
                }

                Emit(JsOpcode.DeleteGlobalBinding, InternedName(plain.Name));
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=54EE9D
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

        if (update.Operand is JsSuperMemberExpression inherited)
        {
            var owner = FunctionScope();
            var kept = owner.Declare("#update" + owner.SlotCount, constant: false);
            CompileSuperKey(inherited);
            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.LoadSuperProperty);
            Emit(JsOpcode.ToNumber);

            if (!update.Prefix)
            {
                Emit(JsOpcode.Duplicate);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, kept);
            }

            Emit(JsOpcode.LoadConstant, one);
            Emit(add);

            if (update.Prefix)
            {
                Emit(JsOpcode.Duplicate);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, kept);
            }

            Emit(JsOpcode.StoreSuperProperty);
            Emit(JsOpcode.Pop);
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, kept);
            return;
        }

        if (update.Operand is JsPrivateMemberExpression privateOperand)
        {
            var owner = FunctionScope();
            var kept = owner.Declare("#update" + owner.SlotCount, constant: false);
            CompileExpression(privateOperand.Target);
            EmitPrivateName(privateOperand.Span, privateOperand.Name);
            Emit(JsOpcode.DuplicateTwo);
            Emit(JsOpcode.LoadPrivate);
            Emit(JsOpcode.ToNumber);

            if (!update.Prefix)
            {
                Emit(JsOpcode.Duplicate);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, kept);
            }

            Emit(JsOpcode.LoadConstant, one);
            Emit(add);

            if (update.Prefix)
            {
                Emit(JsOpcode.Duplicate);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, kept);
            }

            Emit(JsOpcode.StorePrivate);
            Emit(JsOpcode.Pop);
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, kept);
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3504BE
    // Broiler-Human:        PENDING
    private void CompileAssignment(JsAssignmentExpression assignment)
    {
        if (assignment.Operator == SliceTokenKind.Equals)
        {
            if (assignment.Target is JsIdentifier name)
            {
                CompileNamedValue(assignment.Value, name.Name);
                StoreName(assignment.Span, name.Name);
                return;
            }

            if (assignment.Target is JsSuperMemberExpression inherited)
            {
                CompileSuperKey(inherited);
                CompileExpression(assignment.Value);
                Emit(JsOpcode.StoreSuperProperty);
                return;
            }

            if (assignment.Target is JsPrivateMemberExpression privateTarget)
            {
                CompileExpression(privateTarget.Target);
                EmitPrivateName(privateTarget.Span, privateTarget.Name);
                CompileExpression(assignment.Value);
                Emit(JsOpcode.StorePrivate);
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

        if (assignment.Target is JsSuperMemberExpression host)
        {
            // The key is computed once and duplicated, so the read and the write agree about it
            // even when it is an expression with a side effect.
            CompileSuperKey(host);
            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.LoadSuperProperty);
            CompileExpression(assignment.Value);
            Emit(opcode);
            Emit(JsOpcode.StoreSuperProperty);
            return;
        }

        // THE OBJECT AND THE NAME ARE PUSHED ONCE AND DUPLICATED, so `o.#x += f()` evaluates `o`
        // once - which is what the language says and what re-compiling the target for the write
        // would have got wrong for any target with a side effect.
        if (assignment.Target is JsPrivateMemberExpression privateAccess)
        {
            CompileExpression(privateAccess.Target);
            EmitPrivateName(privateAccess.Span, privateAccess.Name);
            Emit(JsOpcode.DuplicateTwo);
            Emit(JsOpcode.LoadPrivate);
            CompileExpression(assignment.Value);
            Emit(opcode);
            Emit(JsOpcode.StorePrivate);
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=9394AE
    // Broiler-Human:        PENDING
    private void CompileLogicalAssignment(JsAssignmentExpression assignment)
    {
        if (assignment.Target is JsMemberExpression member)
        {
            CompileLogicalMemberAssignment(assignment, member);
            return;
        }

        if (assignment.Target is JsPrivateMemberExpression privateTarget)
        {
            CompileLogicalPrivateAssignment(assignment, privateTarget);
            return;
        }

        if (assignment.Target is JsSuperMemberExpression superTarget)
        {
            CompileLogicalSuperAssignment(assignment, superTarget);
            return;
        }

        if (assignment.Target is not JsIdentifier name)
        {
            Refuse(
                assignment.Span,
                SliceSourceDiagnosticCode.ConstructOutsideManifest,
                "a logical assignment to a target that is neither a name nor a property is not admitted");

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
        CompileNamedValue(assignment.Value, name.Name);
        StoreName(assignment.Span, name.Name);
        Mark(end);
    }

    /// <summary>Lowers <c>o.x ||= v</c> and its two siblings.</summary>
    /// <remarks>
    /// <para>
    /// <b>THE REFERENCE IS EVALUATED ONCE AND THE WRITE HAPPENS ONLY WHEN THE TEST WANTS IT.</b>
    /// Both halves are observable: <c>f().x ||= v</c> calls <c>f</c> exactly once, and
    /// <c>o.x ||= v</c> on a truthy <c>o.x</c> performs no <c>[[Set]]</c> at all - so a setter does
    /// not run, and a read-only property does not throw in strict mode. Lowering this as
    /// <c>o.x = o.x || v</c>, which is the rewrite it looks like, gets both wrong.
    /// </para>
    /// <para>
    /// <b>The two paths meet at one height</b>, which is what lets the verifier check this at all:
    /// the assigning path ends with the store's own result on the stack and the short-circuiting
    /// path unwinds the base - and the key, when there is one - from underneath the value it read.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B4A313
    // Broiler-Human:        PENDING
    private void CompileLogicalMemberAssignment(
        JsAssignmentExpression assignment, JsMemberExpression member)
    {
        var end = NewLabel();
        var kept = NewLabel();
        CompileExpression(member.Target);

        if (member.Computed is not null)
        {
            CompileExpression(member.Computed);
            Emit(JsOpcode.DuplicateTwo);
            Emit(JsOpcode.GetIndex);
        }
        else
        {
            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.GetProperty, InternedName(member.Name));
        }

        Emit(JsOpcode.Duplicate);

        switch (assignment.Operator)
        {
            case SliceTokenKind.AmpersandAmpersand:
                Branch(JsOpcode.JumpIfFalse, kept);
                break;

            case SliceTokenKind.BarBar:
                Branch(JsOpcode.JumpIfTrue, kept);
                break;

            default:
                Emit(JsOpcode.LoadNull);
                Emit(JsOpcode.LooseEquals);
                Emit(JsOpcode.Not);
                Branch(JsOpcode.JumpIfTrue, kept);
                break;
        }

        Emit(JsOpcode.Pop);
        CompileExpression(assignment.Value);

        if (member.Computed is not null)
        {
            Emit(JsOpcode.SetIndex);
        }
        else
        {
            Emit(JsOpcode.SetProperty, InternedName(member.Name));
        }

        Branch(JsOpcode.Jump, end);
        Mark(kept);

        // THE VALUE THAT WAS ALREADY THERE IS THE ANSWER, and what is under it is this lowering's
        // own working: the base, and the key when the member was computed. Both are dropped here
        // rather than left for the enclosing expression to trip over.
        Emit(JsOpcode.Swap);
        Emit(JsOpcode.Pop);

        if (member.Computed is not null)
        {
            Emit(JsOpcode.Swap);
            Emit(JsOpcode.Pop);
        }

        Mark(end);
    }

    /// <summary>Lowers <c>super.x ||= v</c> and its two siblings.</summary>
    /// <remarks>
    /// <b>The key is computed once and kept</b>, which is what the compound form beside this one
    /// already does and for the same reason: <c>super[f()] ||= v</c> calls <c>f</c> once, and the
    /// read and the write have to agree about the key it answered. A <c>super</c> reference carries
    /// no base on the stack — the home object and the receiver are the frame's — so this is the
    /// narrowest of the four shapes and the only one whose short circuit drops a single value.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=673298
    // Broiler-Human:        PENDING
    private void CompileLogicalSuperAssignment(
        JsAssignmentExpression assignment, JsSuperMemberExpression member)
    {
        var end = NewLabel();
        var kept = NewLabel();

        CompileSuperKey(member);
        Emit(JsOpcode.Duplicate);
        Emit(JsOpcode.LoadSuperProperty);
        Emit(JsOpcode.Duplicate);

        switch (assignment.Operator)
        {
            case SliceTokenKind.AmpersandAmpersand:
                Branch(JsOpcode.JumpIfFalse, kept);
                break;

            case SliceTokenKind.BarBar:
                Branch(JsOpcode.JumpIfTrue, kept);
                break;

            default:
                Emit(JsOpcode.LoadNull);
                Emit(JsOpcode.LooseEquals);
                Emit(JsOpcode.Not);
                Branch(JsOpcode.JumpIfTrue, kept);
                break;
        }

        Emit(JsOpcode.Pop);
        CompileExpression(assignment.Value);
        Emit(JsOpcode.StoreSuperProperty);
        Branch(JsOpcode.Jump, end);
        Mark(kept);

        Emit(JsOpcode.Swap);
        Emit(JsOpcode.Pop);
        Mark(end);
    }

    /// <summary>Lowers <c>o.#x ||= v</c> and its two siblings.</summary>
    /// <remarks>
    /// <b>It is the computed member's shape with the private pair in place of the index pair</b>,
    /// and it is a method of its own rather than a branch inside that one because the two carry
    /// different things on the stack under the value they read: a base and a KEY there, a base and a
    /// PRIVATE NAME here. <b>The short circuit is what the suite tests</b>: <c>o.#m ??= v</c> where
    /// <c>#m</c> is a private METHOD is a program when <c>#m</c> is not nullish, because the store
    /// that would refuse never runs — so the assigning path must be the only one that stores.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C3CB76
    // Broiler-Human:        PENDING
    private void CompileLogicalPrivateAssignment(
        JsAssignmentExpression assignment, JsPrivateMemberExpression member)
    {
        var end = NewLabel();
        var kept = NewLabel();

        CompileExpression(member.Target);
        EmitPrivateName(member.Span, member.Name);
        Emit(JsOpcode.DuplicateTwo);
        Emit(JsOpcode.LoadPrivate);
        Emit(JsOpcode.Duplicate);

        switch (assignment.Operator)
        {
            case SliceTokenKind.AmpersandAmpersand:
                Branch(JsOpcode.JumpIfFalse, kept);
                break;

            case SliceTokenKind.BarBar:
                Branch(JsOpcode.JumpIfTrue, kept);
                break;

            default:
                Emit(JsOpcode.LoadNull);
                Emit(JsOpcode.LooseEquals);
                Emit(JsOpcode.Not);
                Branch(JsOpcode.JumpIfTrue, kept);
                break;
        }

        Emit(JsOpcode.Pop);
        CompileExpression(assignment.Value);
        Emit(JsOpcode.StorePrivate);
        Branch(JsOpcode.Jump, end);
        Mark(kept);

        // The base and the private name are this lowering's own working, and the value that was
        // already there is the answer, so both are dropped from under it.
        Emit(JsOpcode.Swap);
        Emit(JsOpcode.Pop);
        Emit(JsOpcode.Swap);
        Emit(JsOpcode.Pop);
        Mark(end);
    }

    /// <summary>Lowers a regular-expression literal, refusing a pattern that is not one.</summary>
    /// <remarks>
    /// <para>
    /// <b>A LITERAL'S PATTERN IS CHECKED HERE AND NOT WHERE IT RUNS, because the language makes it
    /// an EARLY error.</b> <c>/(/ </c> is a program that does not parse, in the same way that
    /// <c>var = 1</c> is; a front end that emitted it and let the constructor refuse it at run time
    /// answered the right kind of error at the wrong time, and a program that never reached the
    /// literal - one behind a `false` branch, which is how the pinned suite writes these - was
    /// accepted outright.
    /// </para>
    /// <para>
    /// <b>The check is the matcher's own and not a second opinion.</b> The pattern grammar lives in
    /// the format assembly, which both this front end and the executor read, precisely so that the
    /// two cannot disagree about what a pattern is: a front end with its own idea of the grammar
    /// would either refuse a pattern the executor runs or emit one it cannot.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C7D37D
    // Broiler-Human:        PENDING
    private void CompileRegExpLiteral(JsRegExpLiteral pattern)
    {
        try
        {
            // THE FLAGS ARE READ THE WAY THE MATCHER READS THEM and not re-validated here: a flag
            // this front end does not know is refused by the tokenizer that read the literal, and
            // the four the matcher takes are the four that change what the pattern MEANS.
            _ = JsRegExpMatcher.Compile(
                pattern.Pattern,
                pattern.Flags.Contains('i', System.StringComparison.Ordinal),
                pattern.Flags.Contains('m', System.StringComparison.Ordinal),
                pattern.Flags.Contains('s', System.StringComparison.Ordinal),
                pattern.Flags.Contains('u', System.StringComparison.Ordinal) ||
                    pattern.Flags.Contains('v', System.StringComparison.Ordinal));
        }
        catch (JsRegExpSyntaxError failure)
        {
            Refuse(pattern.Span, SliceSourceDiagnosticCode.UnexpectedToken, failure.Message);
        }

        Emit(JsOpcode.LoadGlobal, InternedName("RegExp"));
        Emit(JsOpcode.LoadConstant, StringConstant(pattern.Pattern));
        Emit(JsOpcode.LoadConstant, StringConstant(pattern.Flags));
        Emit(JsOpcode.Construct, (byte)2);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=75497B
    // Broiler-Human:        PENDING
    private void CompileCall(JsCallExpression call)
    {
        // `super.m()` IS THE ONE CALL WHOSE CALLEE AND RECEIVER COME FROM DIFFERENT PLACES. The
        // function is found through the method's home object and the receiver is `this`, so the
        // ordinary member path - which duplicates the base and uses it for both - would have
        // called the inherited method against the prototype instead of against the instance.
        if (call.Callee is JsSuperMemberExpression inherited)
        {
            CompileSuperKey(inherited);
            Emit(JsOpcode.LoadSuperProperty);
            Emit(JsOpcode.LoadThis);
            CompileArguments(call);
            return;
        }
        EmitCallee(call.Callee);

        // A DIRECT `eval` IS A FACT ABOUT THE SPELLING, and this is the only place that fact
        // still exists. `eval(s)` and `(0, eval)(s)` reach the same function object with the same
        // receiver and the same arguments; the language says the first evaluates in the caller's
        // scope and the second in the global one, and no executor can recover the difference from
        // the operand stack. So the lowering says it, with an opcode whose stack effect is the
        // ordinary call's - which is what lets the verifier check it while knowing nothing about
        // what it means. A locally bound `eval` is not this: it resolves to a slot and this
        // condition is false.
        var direct = call.Callee is JsIdentifier callee &&
            string.Equals(callee.Name, "eval", System.StringComparison.Ordinal) &&
            !Resolvable(callee.Name);

        CompileArguments(call, direct);
    }

    /// <summary>Pushes a callee and the receiver the calling convention wants above it.</summary>
    /// <remarks>
    /// <b>Which receiver a call gets is decided by the SPELLING of its callee and by nothing
    /// else.</b> <c>o.f()</c> passes <c>o</c> and <c>(o.f)()</c> passes it too, while <c>(0,
    /// o.f)()</c> and a tagged template whose tag is parenthesised pass <c>undefined</c> - the
    /// difference being whether the callee expression is still a reference when the call reaches
    /// it. This is the one place that decision is made, so an ordinary call and a tagged template
    /// cannot drift apart on it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B2B5D6
    // Broiler-Human:        PENDING
    private void EmitCallee(JsExpression callee)
    {
        if (callee is JsMemberExpression member)
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
            return;
        }

        // `o.#m()` IS CALLED AGAINST `o` EXACTLY AS `o.m()` IS. The private element is found in a
        // different table and the calling convention is the same one, so the shape below is the
        // member arm's with the private instruction where the property read was.
        if (callee is JsPrivateMemberExpression privateCallee)
        {
            CompileExpression(privateCallee.Target);
            Emit(JsOpcode.Duplicate);
            EmitPrivateName(privateCallee.Span, privateCallee.Name);
            Emit(JsOpcode.LoadPrivate);
            Emit(JsOpcode.Swap);
            return;
        }

        // A CALLEE RESOLVED THROUGH AN OBJECT ENVIRONMENT RECORD IS CALLED AGAINST THAT OBJECT.
        // `with (o) { f() }` runs `f` with `o` as its `this` and `with ({}) { f() }` runs it with
        // `undefined`, and the difference is decided by which of the two branches the search took -
        // which is why the receiver is produced by the same lowering that produced the callee
        // rather than pushed after it.
        if (callee is JsIdentifier bare && Shadowable(bare.Name, out var limit))
        {
            EmitDynamicName(bare.Name, limit, wantsBase: true, orUndefined: false);
            return;
        }

        CompileExpression(callee);
        Emit(JsOpcode.LoadUndefined);
    }

    // ---- templates -----------------------------------------------------------------------------

    /// <summary>Lowers a template literal to the concatenation it is.</summary>
    /// <remarks>
    /// <para>
    /// <b>A template is not sugar for <c>+</c>, and the difference is the coercion.</b> <c>`${x}`</c>
    /// is <c>ToString(x)</c>, which asks an object for <c>toString</c> FIRST; <c>"" + x</c> is
    /// addition, which asks for <c>valueOf</c> first. For <c>{ valueOf() { return 1 }, toString()
    /// { return "s" } }</c> the two answer differently, and every engine answers <c>"s"</c>. So the
    /// substitution goes through the realm's <c>String</c> rather than through <see
    /// cref="JsOpcode.Add"/>, and only the joining is addition - of two Strings, where addition has
    /// no coercion left to get wrong.
    /// </para>
    /// <para>
    /// <b>Except for a Symbol, which must throw, and <c>String</c> is the one function that does
    /// not.</b> <c>String(symbol)</c> is the language's single explicit Symbol-to-String coercion
    /// and answers <c>"Symbol(x)"</c>; a template must throw a <c>TypeError</c> instead. So the
    /// lowering tests the type first and, for a Symbol, reaches the throw the only way an opcode
    /// set without a <c>ToString</c> instruction can - by adding it to a String, which is exactly
    /// the implicit coercion the type refuses. The added value is never used: that path always
    /// throws, and it falls through to the call only so that the two paths meet the verifier at one
    /// height.
    /// </para>
    /// <para>
    /// <b>The declared cost is that this reads the global <c>String</c>.</b> A program that
    /// replaces it changes what a template produces, which the language does not allow. It is the
    /// same dependency a regular-expression literal already takes on the global <c>RegExp</c>, and
    /// it is taken for the same reason: this instruction set has no opcode for the operation, and a
    /// wrong coercion in every template is a worse answer than a coercion a hostile program can
    /// move.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=3E5E65
    // Broiler-Falsified-If: a substitution coerces through `valueOf` before `toString`, or a Symbol substitution does not throw
    // Broiler-Human:        PENDING
    private void CompileTemplate(JsTemplateLiteral template)
    {
        Emit(JsOpcode.LoadConstant, StringConstant(template.Cooked[0]));

        for (var index = 0; index < template.Substitutions.Count; index++)
        {
            EmitToString(template.Substitutions[index]);
            Emit(JsOpcode.Add);

            var tail = template.Cooked[index + 1];

            if (tail.Length != 0)
            {
                Emit(JsOpcode.LoadConstant, StringConstant(tail));
                Emit(JsOpcode.Add);
            }
        }
    }

    /// <summary>Pushes <c>ToString</c> of one expression, throwing for a Symbol as the language does.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=A802C8
    // Broiler-Falsified-If: the two paths reach the call at different operand-stack heights
    // Broiler-Human:        PENDING
    private void EmitToString(JsExpression value)
    {
        Emit(JsOpcode.LoadGlobal, InternedName("String"));
        Emit(JsOpcode.LoadUndefined);
        CompileExpression(value);

        var ordinary = NewLabel();
        Emit(JsOpcode.Duplicate);
        Emit(JsOpcode.TypeOf);
        Emit(JsOpcode.LoadConstant, StringConstant("symbol"));
        Emit(JsOpcode.StrictEquals);
        Branch(JsOpcode.JumpIfFalse, ordinary);

        // The Symbol path. `Add` of a String and a Symbol is the implicit coercion the type
        // refuses, so this throws and never arrives below; it leaves one value behind on paper so
        // that the fall-through and the branch agree about the height.
        Emit(JsOpcode.LoadConstant, StringConstant(string.Empty));
        Emit(JsOpcode.Swap);
        Emit(JsOpcode.Add);

        Mark(ordinary);
        Emit(JsOpcode.Call, (byte)1);
    }

    /// <summary>Lowers a tagged template to the call it is.</summary>
    /// <remarks>
    /// The tag is called with the strings object first and every substitution after it, in source
    /// order, and the template is never concatenated at all. The strings object is built by
    /// <see cref="EmitTemplateStrings"/>, which is where the identity rule lives.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=04E30A
    // Broiler-Human:        PENDING
    private void CompileTaggedTemplate(JsTaggedTemplate tagged)
    {
        EmitCallee(tagged.Tag);
        EmitTemplateStrings(tagged.Quasi);

        foreach (var substitution in tagged.Quasi.Substitutions)
        {
            CompileExpression(substitution);
        }

        var count = tagged.Quasi.Substitutions.Count + 1;

        if (count > 255)
        {
            Refuse(
                tagged.Span,
                SliceSourceDiagnosticCode.ConstructOutsideManifest,
                "a tagged template with more than 254 substitutions is not admitted");
        }

        Emit(JsOpcode.Call, (byte)System.Math.Min(count, 255));
    }

    /// <summary>
    /// Pushes the strings object of one tagged-template CALL SITE, the same object every time.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The identity is the specification's, and it is what a tag is usually written to
    /// exploit.</b> <c>function f() { return tag`x`; } f() === f()</c> compares the strings object
    /// of one site against itself and must answer true, because a tag that caches a compiled
    /// result against the strings object - which is the reason the rule exists - would otherwise
    /// recompile on every call and leak a cache entry each time.
    /// </para>
    /// <para>
    /// <b>So the cache is keyed by the site, and lives on the global object because that is the
    /// only per-realm store this instruction set can reach.</b> A slot of any environment is
    /// per-invocation and would answer false for exactly the program above; the constant pool is
    /// per-artifact and holds no objects. The key is the script's ordinal and the template's line
    /// and column, spelled with a <c>#</c> that no source can write, so it collides with nothing a
    /// program declares. <b>The declared cost is that the property is there</b>: a program that
    /// enumerates the global object sees one entry per tagged-template site it has evaluated.
    /// </para>
    /// <para>
    /// <b><see cref="JsOpcode.LoadGlobalOrUndefined"/> and not <see cref="JsOpcode.LoadGlobal"/></b>,
    /// because the first evaluation of a site is exactly the case where the property is absent, and
    /// the ordinary load throws for that.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=2E89AA
    // Broiler-Falsified-If: two evaluations of one call site produce two strings objects
    // Broiler-Human:        PENDING
    private void EmitTemplateStrings(JsTemplateLiteral quasi)
    {
        var key = InternedName(
            string.Create(
                System.Globalization.CultureInfo.InvariantCulture,
                $"#template@{scripts}:{quasi.Span.Line}:{quasi.Span.Column}"));

        var have = NewLabel();
        Emit(JsOpcode.LoadGlobalOrUndefined, key);
        Emit(JsOpcode.Duplicate);
        Emit(JsOpcode.LoadUndefined);
        Emit(JsOpcode.StrictEquals);
        Branch(JsOpcode.JumpIfFalse, have);

        Emit(JsOpcode.Pop);
        EmitFreshTemplateStrings(quasi);
        Emit(JsOpcode.Duplicate);
        Emit(JsOpcode.StoreGlobal, key);
        Mark(have);
    }

    /// <summary>Builds one frozen strings object carrying its frozen <c>raw</c>.</summary>
    /// <remarks>
    /// <para>
    /// This is <c>Object.freeze(Object.defineProperty(cooked, "raw", { value:
    /// Object.freeze(raw) }))</c>, emitted rather than written, and every part of that shape is
    /// load-bearing. <b>The freeze of <c>cooked</c> comes last</b>, because a frozen object accepts
    /// no new property and defining <c>raw</c> afterwards would silently fail in sloppy code and
    /// throw in strict. <b><c>defineProperty</c> rather than an ordinary assignment</b>, because
    /// <c>raw</c> is not enumerable in the language and an enumerable one would show up in
    /// <c>Object.keys(strings)</c>, which tags iterate.
    /// </para>
    /// <para>
    /// <b>It reaches the global <c>Object</c> to do it</b>, for the reason
    /// <see cref="CompileTemplate"/> reaches the global <c>String</c>: there is no opcode for
    /// integrity levels, and an unfrozen strings object is observably not the one the language
    /// describes. It runs once per site, behind the cache.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FFA443
    // Broiler-Human:        PENDING
    private void EmitFreshTemplateStrings(JsTemplateLiteral quasi)
    {
        Emit(JsOpcode.LoadGlobal, InternedName("Object"));
        Emit(JsOpcode.Duplicate);
        Emit(JsOpcode.GetProperty, InternedName("freeze"));
        Emit(JsOpcode.Swap);

        Emit(JsOpcode.LoadGlobal, InternedName("Object"));
        Emit(JsOpcode.Duplicate);
        Emit(JsOpcode.GetProperty, InternedName("defineProperty"));
        Emit(JsOpcode.Swap);

        foreach (var cooked in quasi.Cooked)
        {
            Emit(JsOpcode.LoadConstant, StringConstant(cooked));
        }

        Emit(JsOpcode.NewArray, (ushort)quasi.Cooked.Count);
        Emit(JsOpcode.LoadConstant, StringConstant("raw"));
        Emit(JsOpcode.NewObject);

        Emit(JsOpcode.LoadGlobal, InternedName("Object"));
        Emit(JsOpcode.Duplicate);
        Emit(JsOpcode.GetProperty, InternedName("freeze"));
        Emit(JsOpcode.Swap);

        foreach (var raw in quasi.Raw)
        {
            Emit(JsOpcode.LoadConstant, StringConstant(raw));
        }

        Emit(JsOpcode.NewArray, (ushort)quasi.Raw.Count);
        Emit(JsOpcode.Call, (byte)1);

        Emit(JsOpcode.DefineField, InternedName("value"));
        Emit(JsOpcode.Call, (byte)3);
        Emit(JsOpcode.Call, (byte)1);
    }

    // ---- optional chains -----------------------------------------------------------------------

    /// <summary>Lowers one whole optional chain.</summary>
    /// <remarks>
    /// <para>
    /// <b>One merge label for the chain, and every short circuit in it jumps there.</b> That is
    /// the shape the construct demands: in <c>a?.b.c.d</c> a nullish <c>a</c> must skip <c>.c</c>
    /// and <c>.d</c> as well, so the target of the jump is past the LAST link and not past the
    /// next one - a position only this method knows, because only this method knows where the
    /// chain ends.
    /// </para>
    /// <para>
    /// <b>Both paths reach the merge holding exactly one value, and that is the whole of what the
    /// verifier checks.</b> Each guard knows how many values the chain has pushed at the point it
    /// tests - one for a plain access, two for a call whose receiver is already under its callee -
    /// and pops exactly that many before pushing the answer. A guard that miscounted would reach
    /// the merge at the wrong height and be refused with an inconsistent join, which is the good
    /// failure: it is caught by the verifier rather than by the value being wrong at run time.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=E0C92D
    // Broiler-Falsified-If: a link after a short-circuited one is evaluated, or the two paths meet at different heights
    // Broiler-Human:        PENDING
    private void CompileChain(JsChainExpression chain)
    {
        var end = NewLabel();
        EmitChainLink(chain.Chain, end, shortIsTrue: false);
        Mark(end);
    }

    /// <summary>Emits one link of a chain, leaving its value on the operand stack.</summary>
    /// <remarks>
    /// The recursion descends the chain to its head and builds back up, which is the order the
    /// language evaluates in: the base, then the key or the arguments, and never the second when
    /// the first declined. Anything that is not a member or a call is the head, and is compiled as
    /// the ordinary expression it is - which is how a parenthesised chain inside another one keeps
    /// its own merge.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=1DBDA5
    // Broiler-Human:        PENDING
    private void EmitChainLink(JsExpression node, Label end, bool shortIsTrue)
    {
        switch (node)
        {
            case JsPrivateMemberExpression privateAccess:
            {
                EmitChainLink(privateAccess.Target, end, shortIsTrue);

                if (privateAccess.Optional)
                {
                    EmitNullishGuard(end, held: 1, shortIsTrue);
                }

                EmitPrivateName(privateAccess.Span, privateAccess.Name);
                Emit(JsOpcode.LoadPrivate);
                return;
            }

            case JsMemberExpression member:
            {
                EmitChainLink(member.Target, end, shortIsTrue);

                if (member.Optional)
                {
                    EmitNullishGuard(end, held: 1, shortIsTrue);
                }

                if (member.Computed is null)
                {
                    Emit(JsOpcode.GetProperty, InternedName(member.Name));
                }
                else
                {
                    CompileExpression(member.Computed);
                    Emit(JsOpcode.GetIndex);
                }

                return;
            }

            case JsCallExpression call when call.Callee is JsMemberExpression callee:
            {
                EmitChainLink(callee.Target, end, shortIsTrue);

                if (callee.Optional)
                {
                    EmitNullishGuard(end, held: 1, shortIsTrue);
                }

                Emit(JsOpcode.Duplicate);

                if (callee.Computed is null)
                {
                    Emit(JsOpcode.GetProperty, InternedName(callee.Name));
                }
                else
                {
                    CompileExpression(callee.Computed);
                    Emit(JsOpcode.GetIndex);
                }

                // THE RECEIVER IS STILL UNDER THE CALLEE HERE, which is why the guard says two.
                // Testing after the exchange would need to reach under the top of the stack for
                // the value being tested, and testing before it is simply the same test one
                // instruction earlier.
                if (call.Optional)
                {
                    EmitNullishGuard(end, held: 2, shortIsTrue);
                }

                Emit(JsOpcode.Swap);
                CompileArguments(call);
                return;
            }

            case JsCallExpression call:
            {
                EmitChainLink(call.Callee, end, shortIsTrue);

                if (call.Optional)
                {
                    EmitNullishGuard(end, held: 1, shortIsTrue);
                }

                Emit(JsOpcode.LoadUndefined);
                CompileArguments(call);
                return;
            }

            default:
                CompileExpression(node);
                return;
        }
    }

    /// <summary>Emits a call's arguments and the call instruction that consumes them.</summary>
    /// <param name="call">The call, whose callee and receiver are already on the stack.</param>
    /// <param name="direct">
    /// Whether the callee was spelled as the bare name <c>eval</c>, which is the one thing about a
    /// call that only its SPELLING knows. A call inside an optional chain never passes it:
    /// <c>eval?.(s)</c> is INDIRECT in the language, which is the same answer <c>(0, eval)(s)</c>
    /// gets.
    /// </param>
    /// <remarks>
    /// <b>Every call in this lowering ends here, and that is the point.</b> An ordinary call, a
    /// <c>super.m()</c>, an optional call and an optional member call each push their callee and
    /// receiver differently and then all want the same thing done with the argument list - so the
    /// spread test, the 255 ceiling and the choice of instruction live in one place. They did not,
    /// and a spread argument reached a lowering that had never heard of one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=B26156
    // Broiler-Human:        PENDING
    private void CompileArguments(JsCallExpression call, bool direct = false)
    {
        // A SPREAD MAKES THE COUNT A RUN-TIME QUANTITY, so the arguments travel as one Array and
        // the instruction that takes them has a fixed stack effect again. A direct `eval` spelled
        // with a spread loses its directness here, which is stated as a divergence rather than
        // hidden: there is no `CallEvalSpread`, and inventing one for `eval(...xs)` would add an
        // opcode to the published set for a spelling no program in the corpus uses.
        if (HasSpread(call.Arguments))
        {
            CompileArgumentArray(call.Arguments);
            Emit(JsOpcode.CallSpread);
            return;
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

        Emit(
            direct ? JsOpcode.CallEval : JsOpcode.Call,
            (byte)System.Math.Min(call.Arguments.Count, 255));
    }

    /// <summary>Short-circuits the whole chain when the value on top is <c>null</c> or <c>undefined</c>.</summary>
    /// <param name="end">Where the chain's two paths meet.</param>
    /// <param name="held">
    /// How many values this chain has on the operand stack right now, the tested one included. It
    /// is the count the short-circuit path pops, and it is passed rather than computed because the
    /// caller is the only thing that knows it.
    /// </param>
    /// <param name="shortIsTrue">
    /// Whether the short circuit answers <c>true</c> rather than <c>undefined</c>, which is what
    /// <c>delete a?.b</c> needs: deleting through a chain that declined to run succeeded, and the
    /// language says so.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F0C38B
    // Broiler-Human:        PENDING
    private void EmitNullishGuard(Label end, int held, bool shortIsTrue)
    {
        // WHAT THE CHAIN IS HOLDING WHEN THE TEST RUNS IS WHAT IT IS STILL HOLDING IF THE TEST
        // SAYS NO, and the straight-line stack model cannot see that, because the path it walks
        // from here is the one that throws the holdings away. It is told afterwards.
        var live = buffer.Height;

        var target = NewLabel();
        Emit(JsOpcode.Duplicate);
        Emit(JsOpcode.LoadNull);
        Emit(JsOpcode.LooseEquals);
        Branch(JsOpcode.JumpIfFalse, target);

        for (var index = 0; index < held; index++)
        {
            Emit(JsOpcode.Pop);
        }

        Emit(shortIsTrue ? JsOpcode.LoadTrue : JsOpcode.LoadUndefined);
        Branch(JsOpcode.Jump, end);
        Mark(target);
        buffer.Rejoin(live);
    }

    /// <summary>
    /// Pushes the key half of a <c>super</c> property access, refusing the access where the
    /// language has no <c>super</c> to start it from.
    /// </summary>
    /// <remarks>
    /// The key is pushed as a VALUE rather than encoded as a name operand, so that
    /// <c>super.x</c> and <c>super[e]</c> are one instruction with one stack effect. A pair of
    /// instructions would have bought nothing: the named form is a constant load either way.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=882E88
    // Broiler-Human:        PENDING
    private void CompileSuperKey(JsSuperMemberExpression member)
    {
        if (!insideMethod)
        {
            Refuse(
                member.Span,
                SliceSourceDiagnosticCode.UnexpectedToken,
                "`super` is only admitted inside a method");
        }

        if (member.Computed is null)
        {
            Emit(JsOpcode.LoadConstant, StringConstant(member.Name));
            return;
        }

        // THE RECEIVER IS READ BEFORE THE KEY EXPRESSION, and the read is emitted for its effect
        // alone. `super[f()]` in a derived constructor that has not called `super()` yet must be a
        // ReferenceError about `this` and must not run `f` at all - the specification takes the
        // this binding first and the key second. Leaving the read to the instruction that consumes
        // the key would have run `f` first and reported whatever `f` did.
        Emit(JsOpcode.LoadThis);
        Emit(JsOpcode.Pop);
        CompileExpression(member.Computed);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=2E5F93
    // Broiler-Human:        PENDING
    private void CompileSuperCall(JsSuperCallExpression call)
    {
        if (!insideDerivedConstructor)
        {
            Refuse(
                call.Span,
                SliceSourceDiagnosticCode.UnexpectedToken,
                "a `super` call is only admitted in the constructor of a class with a heritage");
        }

        // `super(...args)` IS NOT `f.apply`-SHAPED AND MUST NOT BORROW CallSpread. That
        // instruction takes a callee and a receiver off the operand stack; a super call takes the
        // superclass and the `new.target` from the FRAME, and there is nothing beneath the
        // argument Array for CallSpread to pop. The two families meeting is what
        // `SuperCallSpread` is for.
        if (HasSpread(call.Arguments))
        {
            CompileArgumentArray(call.Arguments);
            Emit(JsOpcode.SuperCallSpread);
            return;
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

        Emit(JsOpcode.SuperCall, (byte)System.Math.Min(call.Arguments.Count, 255));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8A09AF
    // Broiler-Human:        PENDING
    private void CompileStoreTo(JsExpression target)
    {
        switch (target)
        {
            case JsIdentifier name:
                StoreName(name.Span, name.Name);
                break;

            case JsPrivateMemberExpression privateAccess:
            {
                var owner = FunctionScope();
                var kept = owner.Declare("#target" + owner.SlotCount, constant: false);
                EmitScoped(JsOpcode.InitialiseScoped, (byte)blockDepth, kept);
                CompileExpression(privateAccess.Target);
                EmitPrivateName(privateAccess.Span, privateAccess.Name);
                EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, kept);
                Emit(JsOpcode.StorePrivate);
                break;
            }

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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=8A3878
    // Broiler-Human:        PENDING
    private void LoadName(SliceSourceSpan span, string name)
    {
        _ = span;

        if (Shadowable(name, out var limit))
        {
            EmitDynamicName(name, limit, wantsBase: false, orUndefined: false);
            return;
        }

        EmitStaticLoad(name, orUndefined: false);
    }

    /// <summary>Pushes a name the way the enclosing scopes alone would resolve it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=53ACFD
    // Broiler-Human:        PENDING
    private void EmitStaticLoad(string name, bool orUndefined)
    {
        if (TryResolve(name, out var hops, out var slot, out _))
        {
            EmitScoped(JsOpcode.LoadScoped, (byte)hops, slot);
            return;
        }

        // AN IMPORT IS CONSULTED AFTER THE SCOPES AND BEFORE THE GLOBAL, and that order is the
        // shadowing rule. A nearer declaration of the same name wins - a function may declare a
        // local called `a` while the module imports an `a` - and a name that is neither is still a
        // global, because a module's code sees the realm's globals like any other code.
        if (module is { } importing && importing.Imports.TryGetValue(name, out var entry))
        {
            Emit(JsOpcode.LoadImport, (ushort)entry);
            return;
        }

        DeclareSurfaceOf(name);

        Emit(
            orUndefined ? JsOpcode.LoadGlobalOrUndefined : JsOpcode.LoadGlobal,
            InternedName(name));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A65D36
    // Broiler-Human:        PENDING
    private void StoreName(SliceSourceSpan span, string name)
    {
        // A WRITE ASKS THE SAME OBJECTS A READ ASKS, AND THE TWO ANSWERS DIFFER. `with (o) { x = 1 }`
        // sets `o.x` when the object has the name and reaches the enclosing binding when it does
        // not, so the write is the read's shape with `SetProperty` where `GetProperty` was. Both
        // branches leave the assigned value on the stack, because an assignment is an expression.
        if (Shadowable(name, out var limit))
        {
            // The value is already on the stack and both branches give it back, so the height at
            // the join is the height here - which the straight-line model cannot see, because the
            // path it walks is only one of the two.
            var live = buffer.Height;
            var key = InternedName(name);
            var enclosing = NewLabel();
            var done = NewLabel();

            EmitResolve(limit, key);
            Emit(JsOpcode.Duplicate);
            Branch(JsOpcode.JumpIfFalse, enclosing);

            // The value is under the base and `SetProperty` wants it above, so one exchange turns
            // [value, base] into [base, value] and the instruction gives the value back.
            Emit(JsOpcode.Swap);
            Emit(JsOpcode.SetProperty, key);
            Branch(JsOpcode.Jump, done);

            Mark(enclosing);
            Emit(JsOpcode.Pop);
            EmitStaticStore(span, name);
            Mark(done);
            buffer.Rejoin(live);
            return;
        }

        EmitStaticStore(span, name);
    }

    /// <summary>Writes a name the way the enclosing scopes alone would resolve it.</summary>
    /// <remarks>
    /// <b>A WRITE TO AN IMMUTABLE BINDING IS AN INSTRUCTION AND NOT A REFUSAL, inside a
    /// <c>with</c> body and everywhere else.</b> This front end refused it at compile time until
    /// 2026-09-05, including in a <c>with</c> body where the answer was defended as consistency
    /// with the other occurrences; the consistency was real and the rule it was consistent with was
    /// wrong. Every path through here now emits <see cref="JsOpcode.ThrowImmutable"/>, so a
    /// <c>with</c> around the assignment changes which branch runs rather than what the rule is —
    /// which is what that remark always wanted and did not have.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BF1905
    // Broiler-Human:        PENDING
    private void EmitStaticStore(SliceSourceSpan span, string name)
    {
        if (TryResolve(name, out var hops, out var slot, out var constant))
        {
            // A WRITE TO A CONSTANT COMPILES AND THROWS, and until 2026-09-05 it was refused at the
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
        if (module is { } importing && importing.Imports.ContainsKey(name))
        {
            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.ThrowImmutable, InternedName(name));
            return;
        }

        Emit(JsOpcode.Duplicate);
        DeclareSurfaceOf(name);
        Emit(JsOpcode.StoreGlobal, InternedName(name));
    }

    /// <summary>
    /// Whether an enclosing <c>with</c> could bind <paramref name="name"/>, and over how many
    /// records the executor must look for one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the whole of what admitting <c>with</c> costs the binding model, and it costs it
    /// in exactly the names it has to.</b> The walk is the ordinary resolution walk with one extra
    /// question asked at each step: is this record an object one. A name that reaches its binding
    /// without passing an object record is lowered to the <c>(depth, slot)</c> pair it always was
    /// and pays nothing; a name that passes one is lowered to a search, a branch and then that same
    /// pair on the branch the search did not take.
    /// </para>
    /// <para>
    /// <b>The bound is the OUTERMOST object record before the binding, and not the binding.</b>
    /// Searching further would let an outer <c>with</c> shadow a declaration that already shadows
    /// it; searching less far would miss one. A name that resolves to nothing is a global, and then
    /// every record on the chain is between the reference and it.
    /// </para>
    /// <para>
    /// <b>It crosses function boundaries, because the scope chain does.</b> A closure created inside
    /// a <c>with</c> body captures the object record, so a free name in its body has to ask that
    /// object when the closure is CALLED — long after the <c>with</c> statement finished. The
    /// compile-time chain spans functions exactly as the run-time one does, which is what makes the
    /// hop count answerable at all.
    /// </para>
    /// <para>
    /// <b>A chain deeper than the format's scope-depth ceiling is clamped, and the clamp is safe in
    /// one direction only.</b> Clamping searches FEWER records, so a name falls through to the
    /// static address the language's own rules give it; the opposite clamp would have let a record
    /// past the binding answer. The same ceiling already bounds a hop count, so a chain this deep
    /// has a wrong <c>LoadScoped</c> in it before it has a wrong search.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=80888B
    // Broiler-Falsified-If: the bound reaches a record at or beyond the binding this name resolves to
    // Broiler-Human:        PENDING
    private bool Shadowable(string name, out int limit)
    {
        limit = 0;
        var outermost = -1;
        var hops = 0;
        var current = scope;

        while (current is not null)
        {
            if (current.Kind == ScopeKind.With)
            {
                outermost = hops;
            }
            else if (current.Has(name))
            {
                break;
            }

            hops++;
            current = current.Parent;
        }

        if (outermost < 0)
        {
            return false;
        }

        limit = System.Math.Min(outermost + 1, (int)JsFormat.CeilingScopeDepth);
        return true;
    }

    /// <summary>
    /// Lowers one dynamically resolved name: ask the objects, and fall back to the static address.
    /// </summary>
    /// <param name="name">The name being resolved.</param>
    /// <param name="limit">How many records the search covers.</param>
    /// <param name="wantsBase">
    /// Whether the receiver is wanted above the value, which is what a CALL through such a name
    /// needs: <c>with (o) { f() }</c> calls <c>o.f</c> with <c>o</c> as its <c>this</c>, and the
    /// object the search answered with is that receiver.
    /// </param>
    /// <param name="orUndefined">
    /// Whether an absent global answers <c>undefined</c> rather than throwing, which is what
    /// <c>typeof</c> needs and nothing else does.
    /// </param>
    /// <remarks>
    /// <b>A name inside a <c>with</c> body costs a search, a duplicate, a branch and a property
    /// read, and it costs that EVERY TIME it is mentioned.</b> Nothing is cached and nothing can be:
    /// the object may gain or lose the property between two reads in the same body, and the
    /// language says the second read sees that. That is the price of the construct rather than a
    /// shortcoming of this lowering, and it is why nothing outside a <c>with</c> body pays any of it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5522EE
    // Broiler-Human:        PENDING
    private void EmitDynamicName(string name, int limit, bool wantsBase, bool orUndefined)
    {
        var live = buffer.Height;
        var key = InternedName(name);
        var enclosing = NewLabel();
        var done = NewLabel();

        EmitResolve(limit, key);
        Emit(JsOpcode.Duplicate);
        Branch(JsOpcode.JumpIfFalse, enclosing);

        if (wantsBase)
        {
            Emit(JsOpcode.Duplicate);
        }

        Emit(JsOpcode.GetProperty, key);

        if (wantsBase)
        {
            // The receiver is under the callee and the calling convention wants it above, exactly
            // as it does for `o.f()`.
            Emit(JsOpcode.Swap);
        }

        Branch(JsOpcode.Jump, done);

        Mark(enclosing);
        Emit(JsOpcode.Pop);
        EmitStaticLoad(name, orUndefined);

        if (wantsBase)
        {
            Emit(JsOpcode.LoadUndefined);
        }

        Mark(done);

        // One value arrives, or two when the receiver was wanted. The straight-line model walked
        // both branches in sequence and would otherwise carry their sum onward.
        buffer.Rejoin(live + (wantsBase ? 2 : 1));
    }

    /// <summary>Emits the search itself, with its bound and the name it is looking for.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C4992C
    // Broiler-Human:        PENDING
    private void EmitResolve(int limit, ushort key) =>
        EmitScoped(JsOpcode.ResolveName, (byte)limit, key);

    /// <summary>
    /// Records that this artifact reaches an optional surface, when the free name belongs to one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A free name is the only evidence there is for a surface made of globals.</b> A construct
    /// the manifest refuses — <c>eval</c> as a direct call, a module declaration — is refused at the
    /// parse and never reaches an artifact at all. A typed array constructor is a name, and a
    /// program that reads it is, byte for byte, a program that reads a name. So the lowering
    /// records the surface here, in the one place a free name is resolved to a global, and
    /// <see cref="Assemble"/> writes what it recorded.
    /// </para>
    /// <para>
    /// <b>A name that resolves to a binding declares nothing</b>, which is why this is below the
    /// resolution rather than beside the parse. A program with its own <c>var Uint8Array</c> is a
    /// program about its own variable and reaches no surface at all.
    /// </para>
    /// <para>
    /// <b>And a <c>typeof</c> declares nothing either</b>, because it does not come through here:
    /// the lowering emits <c>LoadGlobalOrUndefined</c> for it, which is a different instruction
    /// with a different answer for an absent name. That is what keeps
    /// <c>typeof Uint8Array === "undefined"</c> — the shape a machine-generated program uses to
    /// find out whether it may go on — a question this profile answers rather than an artifact it
    /// refuses.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FC755E
    // Broiler-Human:        PENDING
    private void DeclareSurfaceOf(string name)
    {
        if (JsSurfaces.TryOwner(name, out var manifestId))
        {
            surfaces.Add(manifestId);
        }
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=14F70D
    // Broiler-Human:        PENDING
    private Scope FunctionScope()
    {
        var current = scope;

        // A `with` scope is walked through exactly as a block is: a temporary the lowering needs
        // belongs to the function, and a `with` record has nowhere to put one.
        while (current.Kind is ScopeKind.Block or ScopeKind.With && current.Parent is not null)
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

    /// <summary>
    /// The loops, switches, labels and finalisers a <c>break</c>, <c>continue</c> or <c>return</c>
    /// at the cursor would have to leave.
    /// </summary>
    /// <remarks>
    /// <b>It is per CODE UNIT and is exchanged when one is entered, which it was not until this
    /// stage.</b> A function body is a control-flow boundary: nothing inside it can reach a loop or
    /// a <c>finally</c> outside it. Sharing one stack across a nested function made a
    /// <c>return</c> inside a closure defined in a <c>try … finally</c> emit that finaliser's body
    /// into the CLOSURE, and a <c>return</c> inside a closure defined in a <c>for … of</c> body
    /// emit a read of the loop's iterator slot against an environment that has no such slot - which
    /// the executor answers by aborting the whole invocation as an internal defect. The stage that
    /// found it is the one that added the second of those two unwinds; the first was already there.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=37F036
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<Exit> exits = [];

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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=862FF3
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
        /// a property of the global object or a binding of the realm's lexical half, and a module's
        /// is a slot nothing outside the module can name.
        /// </remarks>
        Module,

        /// <summary>
        /// The object environment record a <c>with</c> puts on the chain, which declares nothing.
        /// </summary>
        /// <remarks>
        /// <b>It is in this chain so that the HOP COUNTS stay right.</b> The record exists at run
        /// time and every <c>(depth, slot)</c> pair emitted inside a <c>with</c> body counts it, so
        /// a compile-time chain that left it out would resolve every enclosing name one record too
        /// close. It also declares no name and never can, which is what makes a name resolved
        /// through it a lookup on an object rather than on a scope.
        /// </remarks>
        With,
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
        /// The slot holding this loop's iterator record, or -1 when it is not a <c>for … of</c>.
        /// </summary>
        /// <remarks>
        /// <b>Leaving a <c>for … of</c> owes the iterator a <c>return</c>, and the code that leaves
        /// is not written where the loop is.</b> A <c>break</c> three blocks in, a labelled
        /// <c>break</c> out of two loops and a <c>return</c> from anywhere all have to close every
        /// iterator they pass, innermost first, and they find them here rather than by walking the
        /// syntax back up. <c>continue</c> is the exit that must NOT close, which is the reason this
        /// is a slot on the record rather than a fixed instruction at the loop's bottom.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=39F68C
        // Broiler-Human:        PENDING
        internal int IteratorSlot { get; init; } = -1;

        /// <summary>
        /// Whether the iterator in <see cref="IteratorSlot"/> is an ASYNC one.
        /// </summary>
        /// <remarks>
        /// <b>The close is a different sequence and not a different operand.</b> A synchronous
        /// close is one instruction; an asynchronous one calls <c>return</c>, awaits what it
        /// answered and then requires it to be an object — three instructions with a suspension in
        /// the middle — and it skips all three when there is no <c>return</c> to call. Every exit
        /// that owes a close has to know which, and the exit record is the one thing all four of
        /// them already consult.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D82625
        // Broiler-Human:        PENDING
        internal bool IteratorIsAsync { get; init; }

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

    /// <summary>What lowering one module has established so far.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5AA680
    // Broiler-Human:        PENDING
    private sealed class ModuleBuild(string key, Scope environment)
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=71D82D
        // Broiler-Human:        PENDING
        internal string Key { get; } = key;

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FD8308
        // Broiler-Human:        PENDING
        internal Scope Scope { get; } = environment;

        /// <summary>The module specifiers this module requests, in source order.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=AE3191
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<string> Requests { get; } = [];

        /// <summary>The key each request resolved to, parallel to <see cref="Requests"/>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=50B8BD
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<string> RequestKeys { get; } = [];

        /// <summary>The unit this module was presented as, which carries its resolutions.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CDD8D9
        // Broiler-Human:        PENDING
        internal JsModuleUnit? Unit { get; init; }

        /// <summary>Each imported local name, and its index in the artifact's import table.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F84707
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.Dictionary<string, int> Imports { get; } =
            new(System.StringComparer.Ordinal);

        /// <summary>This module's import entries, in the order the artifact writes them.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=90DB02
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<JsImportEntryRow> ImportRows { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=30B446
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<JsLocalExportRow> LocalExports { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F68387
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<JsIndirectExportRow> IndirectExports { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=0AD7C7
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<uint> StarExports { get; } = [];

        /// <summary>Every name this module publishes, so a second use of one is an early error.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=EC9EB4
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.HashSet<string> ExportNames { get; } =
            new(System.StringComparer.Ordinal);

        /// <summary>The function declarations the initialiser gives their closures.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D9E337
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<JsFunctionNode> Functions { get; } = [];

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6BDB99
        // Broiler-Human:        PENDING
        internal int InitialiserUnit { get; set; }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BD9C34
        // Broiler-Human:        PENDING
        internal int BodyUnit { get; set; }
    }
    /// <summary>A region whose range is still being emitted.</summary>
    /// <param name="StackHeight">
    /// The operand height the handler is entered at, under the one value the executor pushes. It is
    /// zero for every region a STATEMENT opens, because a statement boundary is the one place the
    /// operand stack is reliably empty, and it is the height at the pattern for the two regions an
    /// array pattern opens - which is the whole of what lets a region guard an expression.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=342CD5
    // Broiler-Human:        PENDING
    private readonly record struct PendingRegion(
        int TryStart,
        Label Handler,
        int ScopeDepth,
        JsFormat.HandlerKind Kind,
        int StackHeight = 0);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=075549
    // Broiler-Human:        PENDING
    private sealed class ClosedRegion(
        uint tryStart,
        uint tryEnd,
        Label handler,
        uint scopeDepth,
        JsFormat.HandlerKind kind,
        uint stackHeight)
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

        /// <summary>The operand height the handler is entered at, under the value pushed there.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=695B57
        // Broiler-Human:        PENDING
        internal uint StackHeight { get; } = stackHeight;
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

        /// <summary>Re-states the height at a join this straight-line pass walked past.</summary>
        /// <remarks>
        /// <para>
        /// <b><see cref="Track"/> follows the code as written, and an optional chain's guard is the
        /// one lowering here whose written order is not its taken order.</b> The guard's
        /// fall-through is the SHORT CIRCUIT - it pops what the chain was holding and pushes one
        /// value - while the path that continues arrives by branch, holding everything the chain
        /// had. So the pass leaves the model one value low for every guard that was holding two,
        /// and a function with more of those than the twenty-four slots of slack absorb declares a
        /// stack it then overflows. The verifier catches it, which is the good outcome and a poor
        /// diagnosis: it names the instruction that overflowed and not the guard that mis-declared.
        /// </para>
        /// <para>
        /// <b>So the guard says what the height really is instead of the slack being widened to
        /// hide it.</b> Widening would buy a bigger number for every function in the artifact to
        /// pay for an error in a few, and would leave the model wrong - which is worse than a
        /// model that is right, because the next lowering to reach for it would inherit the error.
        /// </para>
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F87859
        // Broiler-Human:        PENDING
        internal void Rejoin(int height)
        {
            Height = System.Math.Max(0, height);
            MaximumStack = System.Math.Max(MaximumStack, Height + 24);
        }

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=AB5325
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
                    pending.Kind,
                    (uint)pending.StackHeight));

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
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=83CCA1
        // Broiler-Human:        PENDING
        internal static bool Mentions(JsNode node, string name)
        {
            switch (node)
            {
                case JsIdentifier identifier:
                    return string.Equals(identifier.Name, name, System.StringComparison.Ordinal);

                // AN ARROW FUNCTION IS NOT A BOUNDARY FOR THIS SEARCH, AND AN ORDINARY FUNCTION IS.
                // The question this walk answers is whether the enclosing function has to
                // materialise an `arguments` object, and an arrow has no `arguments` of its own -
                // a mention inside one reaches the enclosing function's. Stopping at arrows the way
                // this stopped at every function-like node left `function f() { return () =>
                // arguments[0]; }` with no `arguments` slot at all, so the inner reference fell
                // through to a global read and threw a `ReferenceError` at run time
                // *(corrected: JSC-83)*.
                case JsFunctionExpression expression:
                    if (!expression.Function.IsArrow)
                    {
                        return false;
                    }

                    foreach (var statement in expression.Function.Body)
                    {
                        if (Mentions(statement, name))
                        {
                            return true;
                        }
                    }

                    return false;

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

        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=2E101D
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
                        yield return declarator.Pattern;
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

                case JsWithStatement statement:
                    yield return statement.Object;
                    yield return statement.Body;
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
                    yield return statement.Pattern;
                    yield return statement.Right;
                    yield return statement.Body;
                    break;

                case JsForOfStatement statement:
                    yield return statement.Target;
                    yield return statement.Pattern;
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

                // A `yield` HAS A CHILD, and leaving it out of this walk would have made
                // `function* g() { yield arguments[0]; }` decide it does not use `arguments` - so
                // the frame would not materialise one and the read would fail at run time.
                case JsYieldExpression expression:
                    yield return expression.Operand;
                    break;

                // AND SO DOES AN `await`, for the same reason: without this arm,
                // `async function f(){ await arguments[0]; }` would decide it does not use
                // `arguments`, the frame would materialise none, and the read would fail at run
                // time inside a construct that has nothing to do with `arguments`.
                case JsAwaitExpression expression:
                    yield return expression.Operand;
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

                // A NODE WITHOUT AN ARM HERE IS OPAQUE TO THIS WALK, WHICH IS NOT A GAP THAT
                // FAILS LOUDLY. The one question the walk answers is whether a function mentions
                // `arguments`, so a template whose substitution is `arguments[0]` and an optional
                // chain whose head is `arguments` would each have left the enclosing function with
                // no `arguments` object at all - and the mention would have fallen through to a
                // global read that throws at run time. `new.target` has no children and needs no
                // arm; every other node this change adds does.
                case JsTemplateLiteral expression:
                    foreach (var substitution in expression.Substitutions)
                    {
                        yield return substitution;
                    }

                    break;

                // A CLASS BODY IS NOT PART OF THE ENCLOSING FUNCTION AND ITS HEAD IS. The heritage
                // and every computed member key are evaluated where the class is written, so an
                // `arguments` in one belongs to the enclosing function; a member's body is a
                // function of its own and has its own. Yielding the members but not their bodies
                // is how the walk gets the second half right.
                case JsClassExpression expression:
                    yield return expression.Class;
                    break;

                case JsClassDeclaration statement:
                    yield return statement.Class;
                    break;

                case JsClassNode definition:
                    yield return definition.Heritage;

                    foreach (var member in definition.Members)
                    {
                        yield return member;
                    }

                    break;

                case JsTaggedTemplate expression:
                    yield return expression.Tag;
                    yield return expression.Quasi;
                    break;

                case JsChainExpression expression:
                    yield return expression.Chain;
                    break;

                case JsClassMember member:
                    yield return member.Computed;
                    break;

                case JsSuperMemberExpression expression:
                    yield return expression.Computed;
                    break;

                // ONLY THE OBJECT IS A CHILD, because a private name is not an expression: it is
                // spelled where a property name is spelled and evaluates nothing. `a[i].#x` still
                // has to be walked, which is what the target is here for.
                case JsPrivateMemberExpression expression:
                    yield return expression.Target;
                    break;

                case JsPrivateInExpression expression:
                    yield return expression.Target;
                    break;

                case JsSuperCallExpression expression:
                    foreach (var argument in expression.Arguments)
                    {
                        yield return argument;
                    }

                    break;

                case JsSpreadElement expression:
                    yield return expression.Argument;
                    break;

                case JsDestructuringAssignment expression:
                    yield return expression.Target;
                    yield return expression.Value;
                    break;

                // The pattern arms exist so a mention inside a DEFAULT is found. Nothing else in a
                // pattern is an expression, and a leaf's identifier is a binding rather than a
                // read - but reporting `function f({arguments}) {}` as a mention only costs an
                // arguments object nobody looks at, where missing one would be wrong.
                case JsTargetPattern pattern:
                    yield return pattern.Target;
                    break;

                case JsPatternElement element:
                    yield return element.Target;
                    yield return element.Default;
                    break;

                case JsArrayPattern pattern:
                    foreach (var element in pattern.Elements)
                    {
                        yield return element;
                    }

                    yield return pattern.Rest;
                    break;

                case JsPatternProperty property:
                    yield return property.Computed;
                    yield return property.Value;
                    break;

                case JsObjectPattern pattern:
                    foreach (var property in pattern.Properties)
                    {
                        yield return property;
                    }

                    yield return pattern.Rest;
                    break;

                default:
                    break;
            }
        }
    }
}
