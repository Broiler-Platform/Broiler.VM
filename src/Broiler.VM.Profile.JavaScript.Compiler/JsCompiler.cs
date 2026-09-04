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
        return compiler.Run(scripts);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E1BE6C
    // Broiler-Human:        PENDING
    private JsCompilation Run(System.Collections.Generic.IReadOnlyList<JsScriptUnit> scripts)
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

        return new JsCompilation(true, Assemble(), diagnostics);
    }

    // ---- assembly ------------------------------------------------------------------------------

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=EEE3A5
    // Broiler-Human:        PENDING
    private byte[] Assemble()
    {
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

        return JsArtifactWriter.Write(JsFormat.ManifestId, sections.ToArray());
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
        var outerExits = exits;
        exits = [];

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
        exits = outerExits;
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
        var outerExits = exits;
        exits = [];

        strict = strict || function.IsStrict;

        var flags = Strictness() | JsFormat.FunctionFlags.Constructible;

        if (function.IsArrow)
        {
            flags = (flags & ~JsFormat.FunctionFlags.Constructible) | JsFormat.FunctionFlags.Arrow;
        }

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

        var usesArguments = !function.IsArrow && UsesArguments(function);

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
        exits = outerExits;
        return index;
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=3FFD44
    // Broiler-Human:        PENDING
    private JsFormat.FunctionFlags Strictness() =>
        strict ? JsFormat.FunctionFlags.Strict : JsFormat.FunctionFlags.None;

    // ---- parameters ----------------------------------------------------------------------------

    /// <summary>Whether every parameter is one name with no initialiser and no <c>...</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
                ApplyDefault(parameter.Default);
            }

            BindPattern(parameter.Target, BindMode.Initialise);
        }
    }

    // ---- destructuring -------------------------------------------------------------------------

    /// <summary>What a pattern's leaf does with the value that reached it.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
    /// At script top level nothing is declared here: a declaration there becomes a property of the
    /// global object, and a slot with the same name would shadow it for the rest of the unit.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private void ApplyDefault(JsExpression? initialiser)
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
        CompileExpression(initialiser);
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
            Emit(JsOpcode.StoreGlobal, InternedName(name.Name));
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
    /// <b>The iterator record stays on the operand stack for the whole pattern.</b> A slot would
    /// have been the safer-looking choice - it is what <c>for … in</c> does - but a pattern nests,
    /// so two of them can be live at once and each nesting level would need a slot of its own
    /// chosen at compile time. The stack already nests correctly, and nothing here branches out of
    /// the pattern, so the only join is the one <c>IterateNext</c> makes and it is balanced by hand
    /// below.
    /// </para>
    /// <para>
    /// <b>An exhausted iterator supplies <c>undefined</c> rather than ending the pattern</b>, which
    /// is what makes <c>const [a, b] = [1]</c> give <c>b</c> the value <c>undefined</c> and what
    /// lets <c>[a = 1] = []</c> take its default.
    /// </para>
    /// <para>
    /// <b>ONE DECLARED DIVERGENCE: an exception raised part-way through a pattern does not close
    /// the iterator, and the specification says it should.</b> A <c>for … of</c> body that throws
    /// does close it - that path has an exception region - and this one cannot have one, because a
    /// region's handler is entered at a fixed operand-stack height and a pattern is applied in the
    /// middle of an expression whose stack is not empty. What it costs is visible only to an
    /// iterator whose <c>return</c> is observable AND a pattern element whose default throws, and
    /// what it would cost to close is a spill of every live operand to slots at every pattern.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private void BindArrayPattern(JsArrayPattern pattern, BindMode mode)
    {
        Emit(JsOpcode.IterateStart);

        foreach (var element in pattern.Elements)
        {
            var exhausted = NewLabel();
            var ready = NewLabel();

            Emit(JsOpcode.Duplicate);
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

            ApplyDefault(element.Default);
            BindPattern(element.Target, mode);
        }

        if (pattern.Rest is not null)
        {
            Emit(JsOpcode.Duplicate);
            Emit(JsOpcode.IterateRest);
            BindPattern(pattern.Rest, mode);
        }

        // A pattern that stopped before the iterator did owes it a `return`, and one that ran the
        // iterator out does not. The opcode reads the record's own done flag rather than being told
        // which case this is, so a rest element and an exhausted iterator both make it a no-op.
        Emit(JsOpcode.IterateClose, (byte)0);
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
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
            Emit(JsOpcode.Duplicate);

            if (property.Computed is null)
            {
                Emit(JsOpcode.GetProperty, InternedName(property.Key));
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

            ApplyDefault(property.Value.Default);
            BindPattern(property.Value.Target, mode);
        }

        if (pattern.Rest is null)
        {
            Emit(JsOpcode.Pop);
            return;
        }

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

        BindPattern(pattern.Rest, mode);
        Emit(JsOpcode.Pop);
    }

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
                    CollectDeclaratorNames(declarator, names);
                }
            }
        }
    }

    /// <summary>Every name one declarator introduces, whether it names one or destructures.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
    // Broiler-Human:        PENDING
    private static void CollectDeclaratorNames(
        JsDeclarator declarator, System.Collections.Generic.List<string> names) =>
        CollectHeadNames(declarator.Name, declarator.Pattern, names);

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=755214
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=085DC1
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
        Emit(JsOpcode.IterateStart);

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
        };

        Mark(top);
        EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, record);
        Branch(JsOpcode.IterateNext, exit.Break!);

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
        EmitScoped(JsOpcode.LoadScoped, (byte)outerDepth, record);
        Emit(JsOpcode.IterateClose, (byte)1);
        Emit(JsOpcode.Throw);
        Mark(after);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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

    /// <summary>Runs one enclosing exit's finaliser, or closes its iterator, on the way out.</summary>
    /// <remarks>
    /// The scopes between here and the exit are discarded first, because a finaliser's body and an
    /// iterator's slot are both addressed relative to the depth the exit was created at.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
            EmitScoped(JsOpcode.LoadScoped, (byte)blockDepth, target.IteratorSlot);
            Emit(JsOpcode.IterateClose, (byte)0);
        }

        Branch(JsOpcode.Jump, wantsContinue ? target.Continue! : target.Break!);
        blockDepth = savedDepth;
        scope = savedScope;
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
            Emit(JsOpcode.ArrayHoles, (ushort)holes);
        }
    }

    /// <summary>Whether an argument list carries a spread, so the count is not a constant.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=592172
    // Broiler-Human:        PENDING
    private void CompileObject(JsObjectLiteral literal)
    {
        Emit(JsOpcode.NewObject);

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
            case SliceTokenKind.Typeof when unary.Operand is JsIdentifier name && !Resolvable(name.Name):
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

        Emit(JsOpcode.LoadGlobal, InternedName(name));
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=838085
    // Broiler-Human:        PENDING
    private void StoreName(SliceSourceSpan span, string name)
    {
        if (TryResolve(name, out var hops, out var slot, out var constant))
        {
            if (constant)
            {
                Refuse(
                    span,
                    SliceSourceDiagnosticCode.AssignmentToConstant,
                    "`" + name + "` is a constant binding");
            }

            Emit(JsOpcode.Duplicate);
            EmitScoped(JsOpcode.StoreScoped, (byte)hops, slot);
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=7BBB55
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

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=7108BA
    // Broiler-Human:        PENDING
    private enum ScopeKind
    {
        Program,
        Function,
        Block,
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
        // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=TBF
        // Broiler-Human:        PENDING
        internal int IteratorSlot { get; init; } = -1;

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
