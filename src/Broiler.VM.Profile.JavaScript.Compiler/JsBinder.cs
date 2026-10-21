// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   19
// Annotated:        19/19
// Exempt:           32
// Human-reviewed:   0/19
// IP risk:          None
// Security risk:    High
// Criteria:         6/3
// Resource impact:  3/10 max
// Unverified:       19
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The wide lowering's binder pass: what one compilation unit declares, and how each of its
/// functions is shaped, computed once after the parse and read by every emission.
/// </summary>
/// <remarks>
/// <para>
/// <b>IT ANSWERS QUESTIONS THE LOWERING USED TO ASK OF THE TREE WHILE IT WAS EMITTING</b>
/// (SJL-12.6 item 1, JSD-0080 decision D11). Every hoisting step walked the statements it was about to
/// declare - the <c>var</c> names of a body, its function declarations, the lexical names of each
/// block, the declarations Annex B aliases, whether a function needs an <c>arguments</c> object or a
/// variable environment of its own - and walked them again each time it was emitted, which a
/// <c>finally</c> block is several times. Those walks are here now, made once per unit; the lowering
/// reads their answers and keeps everything that is its own: slot and record allocation, interning,
/// the order units are compiled in and the order instructions are emitted in.
/// </para>
/// <para>
/// <b>ITS FACTS ARE TODAY'S ANSWERS, APPROXIMATIONS INCLUDED, AND NOT THE SPECIFICATION'S.</b> The
/// decision that brought it in is that no artifact's bytes move, so each fact is computed exactly as
/// the lowering computed it: <c>arguments</c> is materialised for any mention of <c>eval</c>, a body
/// gets a variable environment of its own only behind the <c>!simple</c> and <c>!strict</c> gates,
/// and a jump's target is the one the lowering's exit stack would find. SJL-15's shared model
/// replaces these with the specification's facts, under its own rules.
/// </para>
/// <para>
/// <b>EVERY FACT IS IMMUTABLE AND KEYED BY NODE IDENTITY.</b> The nodes are records, whose equality
/// is their content, so two textually identical blocks would be one entry under the default
/// comparer; each table here uses <see cref="System.Collections.Generic.ReferenceEqualityComparer"/>.
/// The tables are filled while the unit is bound and never written afterwards, and none is ever
/// enumerated: every ordered fact is a list in source order that keeps a name's multiplicity, so no
/// hash decides any order the lowering emits.
/// </para>
/// <para>
/// <b>EVERY WALK IS ITERATIVE</b> (decision D17). A tree is as deep as
/// <see cref="SliceParseOptions.MaximumTreeDepth"/> allows, and a pass that recursed once per node
/// would spend the margin that bound was derived with before the lowering had begun.
/// </para>
/// <para>
/// <b>It holds nothing beyond the unit it was made for</b>, and nothing static: a binder is made per
/// unit by <see cref="Bind"/> and dropped with the compiler that asked for it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=BE330C
// Broiler-Falsified-If: a fact the binder answers differs from what the lowering computed for the same node before SJL-12.6 item 1, so that an artifact's bytes or diagnostics move
// Broiler-Human:        PENDING
internal sealed partial class JsBinder
{
    /// <summary>What each block, catch block and <c>switch</c> declares, by node.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=4D5237
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<JsStatement, Declarations> lists =
        new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

    /// <summary>Each function's facts, by node.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=01219A
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<JsFunctionNode, FunctionFacts> functions =
        new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

    /// <summary>The block-level function declarations Annex B also gives a <c>var</c>-scoped binding.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=045A57
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.HashSet<JsFunctionDeclaration> annexB =
        new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

    /// <summary>The private names each class body declares, by class.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=D64B87
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<JsClassNode, string[]> privateNames =
        new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

    /// <summary>
    /// The statement each <c>break</c> and <c>continue</c> leaves, or <see langword="null"/> where
    /// there is none, by jump.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=DE314E
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<JsStatement, JsStatement?> jumps =
        new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

    /// <summary>The names each <c>catch</c> clause's parameter binds, by <c>try</c> statement.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=5B9794
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<JsTryStatement, string[]> catches =
        new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

    /// <summary>
    /// The names each <c>for</c>, <c>for-in</c> and <c>for-of</c> head declares lexically, by loop.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=EAB6B2
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<JsStatement, string[]> heads =
        new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

    /// <summary>The early errors the walk found, in the order it met them.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=E02314
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<SliceSourceDiagnostic> found = [];

    /// <summary>What kind of unit this is, and what its caller permits it when it is eval code.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=4F0742
    // Broiler-Human:        PENDING
    private readonly SliceParseOptions unit;

    /// <summary>Binds one unit.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4C7C4B
    // Broiler-Human:        PENDING
    private JsBinder(JsProgramNode program, bool strict, SliceParseOptions options)
    {
        unit = options;
        Program = BindProgram(program, strict);
        ModuleDeclarations = options.Goal == SliceGoal.Module ? ModuleDeclarationsOf(program.Body) : [];
        BindTree(program, Program.Strict);

        if (options.Goal == SliceGoal.Module)
        {
            CheckModule(program.Body);
        }

        Diagnostics = InSourceOrder(found);
    }

    /// <summary>
    /// What the unit's own top level declares - a script's, an evaluated program's or a module's -
    /// and the strictness it is lowered under.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=E57119
    // Broiler-Human:        PENDING
    internal HoistingScope Program { get; }

    /// <summary>
    /// What a module's own top level declares lexically - each class, <c>let</c> and <c>const</c>
    /// name, an exported one in the export's place - in source order and as often as it is declared;
    /// none for a script or eval code.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=31DBB9
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.IReadOnlyList<ModuleDeclaration> ModuleDeclarations { get; }

    /// <summary>
    /// The unit's early errors - the language early errors the lowering used to raise while it
    /// emitted - each once, in source order, at most as many as the parser's diagnostics hold.
    /// </summary>
    /// <remarks>
    /// <b>A unit with any is not lowered</b> (SJL-12.6 item 2, JSD-0080 decisions D10 and D12): the
    /// compilation is refused with these and nothing else, so an early error never produces bytes,
    /// and none is reported twice because a <c>finally</c> block is lowered twice.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=E0BF86
    // Broiler-Falsified-If: a unit the lowering would have refused with a language early error is answered with none, or a unit it would have lowered without one is answered with one
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.IReadOnlyList<SliceSourceDiagnostic> Diagnostics { get; }

    /// <summary>
    /// Binds one compilation unit: a script, an evaluated program or a module, after it parsed and
    /// after the manifest admitted it, and finds its early errors.
    /// </summary>
    /// <param name="program">The unit's tree.</param>
    /// <param name="imposed">
    /// Whether the caller makes the unit strict whatever its own prologue says: a script's
    /// <see cref="JsScriptUnit.ForceStrict"/>, an evaluation request's strict flag, and every module.
    /// </param>
    /// <param name="options">
    /// The options the unit was parsed under: its goal, and, for eval code, what its caller permits
    /// it - which decides whether <c>super</c>, <c>new.target</c> and <c>arguments</c> name anything
    /// at its own top level.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=9EDD90
    // Broiler-Falsified-If: a unit is lowered under a strictness other than its prologue's or its caller's, or two binds of one tree answer differently
    // Broiler-Human:        PENDING
    internal static JsBinder Bind(JsProgramNode program, bool imposed, SliceParseOptions options) =>
        new(program, program.IsStrict || imposed, options);

    /// <summary>What a function is, and what its body declares.</summary>
    /// <param name="function">
    /// The node the source wrote. A lowering that renames a function before compiling it - a class
    /// constructor takes its class's name, an anonymous function the name it is assigned to - asks
    /// for the node it renamed, which is the one that was bound.
    /// </param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=722DDD
    // Broiler-Human:        PENDING
    internal FunctionFacts Function(JsFunctionNode function) =>
        functions.TryGetValue(function, out var facts) ? facts : throw Unbound("function", function);

    /// <summary>
    /// What one block's statement list declares: a block, a <c>try</c> or <c>finally</c> block, or a
    /// catch block.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=E474E8
    // Broiler-Human:        PENDING
    internal Declarations Block(JsBlockStatement block) =>
        lists.TryGetValue(block, out var declarations) ? declarations : throw Unbound("block", block);

    /// <summary>What a <c>switch</c>'s clauses declare, which are one statement list and one record.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=6F7499
    // Broiler-Human:        PENDING
    internal Declarations Switch(JsSwitchStatement switched) =>
        lists.TryGetValue(switched, out var declarations) ? declarations : throw Unbound("switch", switched);

    /// <summary>
    /// Whether Annex B gives <paramref name="declaration"/>, a function declared in a block, a
    /// <c>var</c>-scoped binding too, which the declaration writes where it stands.
    /// </summary>
    /// <remarks>
    /// <b>Membership is a property of the DECLARATION and not of the name</b>: <c>{ function f() { } {
    /// function f() { } } }</c> declares one name twice and the language admits only the outer
    /// declaration, because <c>var f</c> written in the inner one's place would collide with the outer
    /// block's lexical <c>f</c>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=64999A
    // Broiler-Falsified-If: a block-level declaration is answered admitted when a `var` of its name could not be added to its scope, or in strict code
    // Broiler-Human:        PENDING
    internal bool AdmitsAnnexB(JsFunctionDeclaration declaration) => annexB.Contains(declaration);

    /// <summary>
    /// Every private name one class body declares, in source order and once each: <c>get #a</c> and
    /// <c>set #a</c> declare one name and not two.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=142D2C
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.IReadOnlyList<string> PrivateNames(JsClassNode definition) =>
        privateNames.TryGetValue(definition, out var names) ? names : throw Unbound("class", definition);

    /// <summary>
    /// The loop, <c>switch</c> or labelled statement a <c>break</c> or <c>continue</c> leaves, or
    /// <see langword="null"/> when nothing around it in its function is one it may leave.
    /// </summary>
    /// <remarks>
    /// <b>It is the target the lowering's own exit stack finds, with the lowering's reading of a
    /// label</b>: a loop or a <c>switch</c> carries the label written directly on it, a labelled
    /// statement of any other kind is a target only for <c>break</c>, and a <c>continue</c> naming a
    /// label that is not a loop's looks further out for one that is. <b>Its one reader is the
    /// binder's own refusal of a jump with nowhere to go</b> (SJL-12.6 item 2), which reads it back
    /// as soon as the walk records it; the lowering still finds its targets on its own stack, because
    /// a <c>finally</c> body it emits at a jump is emitted inside the statements the jump leaves,
    /// which no fact about the tree can describe.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=C740E1
    // Broiler-Falsified-If: a jump's recorded target differs from the exit the lowering's stack finds for it where no finally body is being emitted at another jump
    // Broiler-Human:        PENDING
    internal JsStatement? JumpTarget(JsStatement jump) =>
        jumps.TryGetValue(jump, out var target) ? target : throw Unbound("jump", jump);

    /// <summary>
    /// Every name a <c>try</c> statement's <c>catch</c> parameter binds, in the order its pattern writes
    /// them; none when the clause has no parameter or there is no clause.
    /// </summary>
    /// <remarks>
    /// The names live in the record the clause's block is inside of, so the block and the parameter's
    /// own defaults resolve them before anything outside. <b>Its reader is the binder's own walk</b>
    /// (SJL-12.6 item 2), which reads it back as soon as it records it, to decide whether
    /// <c>arguments</c> names a binding of the unit there; the lowering still reads a catch
    /// parameter from the tree.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=909824
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.IReadOnlyList<string> CatchParameters(JsTryStatement guarded) =>
        catches.TryGetValue(guarded, out var names) ? names : throw Unbound("try", guarded);

    /// <summary>
    /// Every name a <c>for</c>, <c>for-in</c> or <c>for-of</c> head declares with <c>let</c> or
    /// <c>const</c>, in the order the head writes them; none for a <c>var</c> head or an expression.
    /// </summary>
    /// <remarks>
    /// <b>They are the head's record's names and not the body's</b>: a <c>for</c> head's names are
    /// declared before its initialiser runs and seen by its test, its update and its body; a
    /// <c>for-in</c> or <c>for-of</c> head's are in their dead zone while the object is evaluated and
    /// bound afresh for each turn of the body. <b>Its reader is the binder's own walk</b> (SJL-12.6
    /// item 2), which reads it back as soon as it records it, to decide whether <c>arguments</c>
    /// names a binding of the unit there; the lowering still reads a loop head from the tree.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=3EBA32
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.IReadOnlyList<string> LoopHead(JsStatement loop) =>
        heads.TryGetValue(loop, out var names) ? names : throw Unbound("loop", loop);

    /// <summary>The failure a question about a node this binder never bound raises.</summary>
    /// <remarks>
    /// <b>It names the node, because the defect is never the source's.</b> Every node of the unit's
    /// tree was bound before the lowering began, so a question about another is the lowering's: a node
    /// it built itself, or a copy it renamed and asked about in place of the one the source wrote -
    /// <see cref="JsCompiler"/> passes the written node for those. Answering with a bare key lookup
    /// left that to be read from a stack trace.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=4D309A
    // Broiler-Falsified-If: a question about a node the binder did not bind is answered, or fails without naming the node
    // Broiler-Human:        PENDING
    private static System.InvalidOperationException Unbound(string kind, JsNode node) =>
        new("the binder bound no " + kind + " at " + Where(node) +
            "; the lowering asked about a node the unit's tree does not hold");

    /// <summary>Where a node stands, as a diagnostic names a place.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=71D8FA
    // Broiler-Human:        PENDING
    private static string Where(JsNode node) => Where(node.Span);

    /// <summary>
    /// A place, as a diagnostic names one - <c>line:column</c> - for the binder's failures and the
    /// lowering's alike.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=00A7C1
    // Broiler-Human:        PENDING
    internal static string Where(SliceSourceSpan span) =>
        span.Line.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":" +
        span.Column.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>One name a module's top level declares lexically, and where.</summary>
    /// <param name="Name">The name.</param>
    /// <param name="Constant">Whether a <c>const</c> declares it; a class's binding and a <c>let</c>'s are mutable.</param>
    /// <param name="Span">
    /// Where the declaration stands: a class declaration's start, or the declarator's for a
    /// <c>let</c> or <c>const</c>, every name of a destructuring declarator sharing its declarator's.
    /// </param>
    /// <param name="IsClass">Whether a class declaration declares it.</param>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=F696F0
    // Broiler-Human:        PENDING
    internal readonly record struct ModuleDeclaration(
        string Name, bool Constant, SliceSourceSpan Span, bool IsClass);

    /// <summary>One name a statement list declares lexically.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=5735E7
    // Broiler-Human:        PENDING
    internal readonly struct Declaration
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=59FF8F
        // Broiler-Human:        PENDING
        internal Declaration(string name, bool constant, bool isFunction)
        {
            Name = name;
            Constant = constant;
            IsFunction = isFunction;
        }

        /// <summary>The name.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=2FD56F
        // Broiler-Human:        PENDING
        internal string Name { get; }

        /// <summary>Whether the binding is immutable: a <c>const</c> or a resource.</summary>
        /// <remarks>
        /// A class's binding is mutable, and so is a block's function: <c>class C {}</c> followed by
        /// <c>C = 1</c> is a program, and only the binding inside the class's own body is immutable.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=8BDC16
        // Broiler-Human:        PENDING
        internal bool Constant { get; }

        /// <summary>
        /// Whether a function declaration makes it, which a script's and an evaluated program's top
        /// level do not count as lexical: there the declaration is <c>var</c>-scoped.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=5409E3
        // Broiler-Human:        PENDING
        internal bool IsFunction { get; }
    }

    /// <summary>What one statement list declares where it stands.</summary>
    /// <remarks>
    /// <b>The whole set, so the whole set can be declared before the list runs.</b> A block's record
    /// holds every lexical name the block declares from the moment the record exists - that is the
    /// dead zone - and a function declared in it is built at block entry, with the block's record
    /// current, so two functions of one block can call each other.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=03A603
    // Broiler-Human:        PENDING
    internal sealed class Declarations
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=F444D0
        // Broiler-Human:        PENDING
        internal Declarations(Declaration[] lexical, JsFunctionDeclaration[] functions)
        {
            Lexical = lexical;
            Functions = functions;
        }

        /// <summary>
        /// Every name the list declares lexically, in source order and as often as it is declared:
        /// each <c>let</c>, <c>const</c> and resource name, each class, and each function declaration,
        /// a labelled one included.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=B578CE
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.IReadOnlyList<Declaration> Lexical { get; }

        /// <summary>
        /// The named function declarations written directly in the list, looking through any labels
        /// on them, in source order.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=2D59E1
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.IReadOnlyList<JsFunctionDeclaration> Functions { get; }
    }

    /// <summary>
    /// What one hoisting scope declares: a script's, an evaluated program's or a module's top level,
    /// or a function body.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=7141FB
    // Broiler-Human:        PENDING
    internal sealed class HoistingScope
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=7D145C
        // Broiler-Human:        PENDING
        internal HoistingScope(
            bool strict,
            Declarations topLevel,
            string[] varNames,
            JsFunctionNode[] functions,
            string[] annexBAliases)
        {
            Strict = strict;
            TopLevel = topLevel;
            VarNames = varNames;
            Functions = functions;
            AnnexBAliases = annexBAliases;
        }

        /// <summary>Whether the scope's code is strict, which a function's is when its definer's is.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=7384C1
        // Broiler-Human:        PENDING
        internal bool Strict { get; }

        /// <summary>What the scope's own statement list declares lexically.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=2A94C9
        // Broiler-Human:        PENDING
        internal Declarations TopLevel { get; }

        /// <summary>
        /// Every name a <c>var</c> anywhere in the scope declares - in blocks, loops, <c>try</c>,
        /// <c>with</c> and <c>switch</c> too, and in a <c>for-in</c> or <c>for-of</c> head - in source
        /// order and as often as it is declared; nothing inside a nested function.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A14A28
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.IReadOnlyList<string> VarNames { get; }

        /// <summary>
        /// The function declarations at the scope's own top level, looking through labels and into
        /// no block, in source order and as often as a name is declared.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=9D7391
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.IReadOnlyList<JsFunctionNode> Functions { get; }

        /// <summary>
        /// The names of the block-level declarations Annex B aliases in this scope, in the order the
        /// declarations are written and once per declaration; none in strict code.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D9CF15
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.IReadOnlyList<string> AnnexBAliases { get; }
    }

    /// <summary>What one function is, as the lowering decides it, and what its body declares.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=68AB04
    // Broiler-Human:        PENDING
    internal sealed class FunctionFacts
    {
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=9E7FE8
        // Broiler-Human:        PENDING
        internal FunctionFacts(
            HoistingScope body,
            bool simpleParameters,
            bool usesArguments,
            bool bodyMayEvaluate,
            bool separateBody,
            bool parametersMayEvaluate)
        {
            Body = body;
            SimpleParameters = simpleParameters;
            UsesArguments = usesArguments;
            BodyMayEvaluate = bodyMayEvaluate;
            SeparateBody = separateBody;
            ParametersMayEvaluate = parametersMayEvaluate;
        }

        /// <summary>What the body declares, and the function's strictness.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=411E66
        // Broiler-Human:        PENDING
        internal HoistingScope Body { get; }

        /// <summary>Whether every parameter is one name with no initialiser and no <c>...</c>.</summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=D7D0F0
        // Broiler-Human:        PENDING
        internal bool SimpleParameters { get; }

        /// <summary>
        /// Whether the function materialises an <c>arguments</c> object: it is not an arrow, no
        /// parameter binds the name, and its own code or an arrow's in it mentions <c>arguments</c> or
        /// <c>eval</c>.
        /// </summary>
        /// <remarks>
        /// <b>The mention of <c>eval</c> is the over-approximation, and it is kept.</b> A direct
        /// evaluation reads the name through the scope map, which can only name a slot that exists, so
        /// any mention costs the object; a mention that is not a direct call costs an object nobody
        /// reads.
        /// </remarks>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=F86B76
        // Broiler-Human:        PENDING
        internal bool UsesArguments { get; }

        /// <summary>
        /// Whether the function is sloppy and its body - its arrows included - mentions <c>eval</c>,
        /// so a direct evaluation may give it bindings by name.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=7CBCD6
        // Broiler-Human:        PENDING
        internal bool BodyMayEvaluate { get; }

        /// <summary>
        /// Whether the body gets a variable environment of its own: the parameter list is not simple,
        /// has expressions, and either may make a closure or sits beside a body that may evaluate.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=A47E9C
        // Broiler-Human:        PENDING
        internal bool SeparateBody { get; }

        /// <summary>
        /// Whether a parameter list that is not simple belongs to a sloppy function and mentions
        /// <c>eval</c>, so its evaluations may introduce bindings before the body runs.
        /// </summary>
        // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=0; Fingerprint=4B36FC
        // Broiler-Human:        PENDING
        internal bool ParametersMayEvaluate { get; }
    }
}
