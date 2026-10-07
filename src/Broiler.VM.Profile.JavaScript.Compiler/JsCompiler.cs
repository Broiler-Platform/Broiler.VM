// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   23
// Annotated:        23/23
// Exempt:           47
// Human-reviewed:   0/23
// IP risk:          Low
// Security risk:    High
// Criteria:         1/1
// Resource impact:  3/10 max
// Unverified:       23
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
/// <param name="Referrer">
/// <b>What a dynamic <c>import()</c> written in this script is resolved against.</b> A module has a
/// key the composition resolved it to and needs no second identity; a script is a text a host
/// handed over, so a relative specifier inside one means whatever the host says it means, and this
/// is where the host says it. It is empty for a script the host does not place, which is not an
/// error: a specifier a resolver can answer without a referrer still resolves, and one it cannot
/// is refused at run time by the resolver rather than here.
/// </param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E0781B
// Broiler-Human:        PENDING
public sealed record JsScriptUnit(
    string Name,
    string Text,
    SliceParseOptions Options,
    bool ForceStrict = false,
    string Referrer = "")
{
    /// <summary>
    /// <b>The name a diagnostic about this text is attributed to, and nothing else.</b>
    /// </summary>
    /// <remarks>
    /// <para>
    /// A host that holds a label for the text - a file, a URL, a script element's identity - says
    /// it here, and every diagnostic this unit's tokenizing, parsing, static semantics or lowering
    /// refuses it with carries it as <see cref="SliceSourceDiagnostic.SourceName"/>. It is three
    /// identities apart from the unit's others: <see cref="Name"/> is the entry point a host
    /// invokes, <see cref="Referrer"/> is what a dynamic <c>import()</c> resolves against, and
    /// this is what a person reading a refusal is told.
    /// </para>
    /// <para>
    /// <b>It reaches no artifact and decides nothing.</b> No byte of the artifact depends on it,
    /// so two compilations differing only here are the same bytes; it selects no strictness,
    /// goal, manifest, entry point or resolution, and it is no permission. An init-only member
    /// rather than a sixth positional parameter, so the constructor and deconstruction every
    /// existing caller names are unchanged. Empty, the default, means no name was given.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6731DA
    // Broiler-Human:        PENDING
    public string SourceName
    {
        get => sourceName;
        init => sourceName = value ?? string.Empty;
    }

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=CCA0B1
    // Broiler-Human:        PENDING
    private readonly string sourceName = string.Empty;
}

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
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=53FFE6
// Broiler-Human:        PENDING
public sealed record JsModuleUnit(
    string Key,
    string Text,
    SliceParseOptions Options,
    System.Collections.Generic.IReadOnlyList<JsResolvedRequest>? Requests = null)
{
    /// <summary>
    /// The module type the composition loaded the text as: empty for a program, or
    /// <see cref="JsFormat.JsonModuleType"/> for a JSON document.
    /// </summary>
    /// <remarks>
    /// <b>A JSON module is a document and not a program</b>, and the front end reads it as one: the
    /// text is parsed as JSON - a text that is not JSON is refused, as the language's load refuses
    /// it with a <c>SyntaxError</c> - and the module is the synthetic one whose only export,
    /// <c>default</c>, is the parsed value.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=99A9E1
    // Broiler-Human:        PENDING
    public string Type { get; init; } = string.Empty;
}

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

/// <summary>Which feature manifest a compilation is asked to lower under.</summary>
/// <remarks>
/// <b>The manifest is a property of the COMPILATION and not of the source, which is why it is
/// asked for here.</b> One text can be admissible under both - a numeric kernel is a perfectly
/// ordinary wide-surface program - and which manifest an artifact names decides what a composition
/// is answering when it admits it. A front end that inferred the manifest from the source would be
/// deciding that for the caller, and would answer differently the day the source grew a string.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=A74AE9
// Broiler-Human:        PENDING
public enum JsFeatureManifest
{
    /// <summary>The wide surface: objects, closures, exceptions and a standard library.</summary>
    Wide = 0,

    /// <summary>The numeric subset, which a whole artifact can be emitted from.</summary>
    Numeric = 1,
}

/// <summary>Which output form a compilation is asked to produce.</summary>
/// <remarks>
/// <para>
/// <b>THE FORM IS CHOSEN HERE, ONCE, AND NOTHING LATER MAY CHANGE IT.</b> This profile's non-goals
/// pin exactly that: the form is chosen when an artifact is compiled and fixed when it is verified,
/// one executor and one form per handle, no promotion. So the choice is a compilation input beside
/// the manifest and the sources - not a run-time observation, not a per-unit judgement, and not
/// something a running host may revisit.
/// </para>
/// <para>
/// <b>It defaults to <see cref="Bytecode"/>, and the default is load-bearing rather than
/// polite.</b> Every existing caller of this compiler gets the artifact it always got, byte for
/// byte, and a form that maps executable memory is something a caller has to ask for by name.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=62A179
// Broiler-Human:        PENDING
public enum JsOutputForm
{
    /// <summary>Bytecode: the form every artifact of this profile has had.</summary>
    Bytecode = 0,

    /// <summary>
    /// Machine code beside the bytecode, emitted for one architecture by one named backend.
    /// </summary>
    /// <remarks>
    /// The bytecode stays in the artifact, and it is not a fallback: the differential oracle and
    /// re-emission-equality verification both read it, and neither is an execution path.
    /// </remarks>
    Native = 1,

    /// <summary>
    /// Machine code beside the bytecode in the wide manifest's value form, emitted for one x86-64
    /// convention by one named backend (JSD-0035).
    /// </summary>
    /// <remarks>
    /// It is a form of its own and not an option of <see cref="Native"/>, because it is fixed when the
    /// artifact is compiled and recorded in the artifact, exactly as the other two are: nothing chooses
    /// it from a run.
    /// </remarks>
    Value = 2,

    /// <summary>
    /// The value form with every binding left in its managed environment: the control JSD-0035 names for
    /// its residency analysis, recorded in the artifact's form byte so the verifier re-plans it that way.
    /// </summary>
    ValueFlat = 3,
}

/// <summary>What a caller asks a compilation for beside its sources.</summary>
/// <param name="Manifest">The feature manifest the artifact will name.</param>
/// <param name="Form">The output form. Bytecode unless a caller says otherwise.</param>
/// <param name="Backend">
/// The name of the backend to emit with, which is meaningful only for
/// <see cref="JsOutputForm.Native"/>. It is a NAME and not a probe of the running machine: which
/// architecture an artifact is emitted for is a property of the artifact, and a compiler that read
/// the host's architecture would answer one source with two artifacts on two machines.
/// </param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=C2C6CD
// Broiler-Human:        PENDING
public sealed record JsCompileRequest(
    JsFeatureManifest Manifest = JsFeatureManifest.Wide,
    JsOutputForm Form = JsOutputForm.Bytecode,
    string Backend = "")
{
    /// <summary>
    /// Whether an exact BigInt literal is lowered rather than refused by name: under the wide
    /// manifest, and never under the numeric one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This was the compile half of the BigInt gate</b> (decision JSD-0033, JSeal cards B01-B02):
    /// internal, false for every public request, and opened only by the slice compiler's checks.
    /// Card B05 completed the surface and admitted it (JSD-0033 section 7), so it now follows the
    /// manifest. A lowered literal writes a BigInt constant and the artifact declares
    /// <see cref="JsSurfaces.BigInt"/>, which a composition may still decline at verification.
    /// </para>
    /// <para>
    /// <b>It also chooses the update lowering</b>: the wide manifest converts with
    /// <see cref="JsOpcode.ToNumeric"/> and steps with <see cref="JsOpcode.Increment"/> or
    /// <see cref="JsOpcode.Decrement"/>; the numeric manifest, which admits no BigInt and none of
    /// those instructions, keeps <c>ToNumber</c>, the constant <c>1</c> and <c>Add</c>.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=BAEFB1
    // Broiler-Human:        PENDING
    internal bool AdmitsBigIntLiterals => Manifest == JsFeatureManifest.Wide;

    /// <summary>
    /// Whether the artifact carries the source text each function was defined from, which
    /// <c>Function.prototype.toString</c> answers (JSD-0037). True unless a caller says otherwise.
    /// </summary>
    /// <remarks>
    /// <b>A size-sensitive composition may drop it, and loses nothing else.</b> The section grants
    /// nothing and no instruction reads it: without it a function renders as a native one, which is
    /// what every artifact said before the section existed.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=010B4F
    // Broiler-Human:        PENDING
    public bool KeepsSourceText { get; init; } = true;
}

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
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=F3B442
// Broiler-Human:        PENDING
public sealed partial class JsCompiler
{
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D4AAEE
    // Broiler-Human:        PENDING
    private const int MaximumSlots = 60000;

    /// <summary>What this compilation was asked for beside its sources.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=01398D
    // Broiler-Human:        PENDING
    private readonly JsCompileRequest request;

    /// <summary>Creates a compiler that lowers the wide surface to bytecode.</summary>
    /// <remarks>
    /// The parameterless form is kept so that every caller written before an output form could be
    /// chosen still names the thing it always meant: the wide manifest, in bytecode.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=AAF58E
    // Broiler-Human:        PENDING
    public JsCompiler()
        : this(new JsCompileRequest())
    {
    }

    /// <summary>Creates a compiler that answers <paramref name="request"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=174E1D
    // Broiler-Human:        PENDING
    public JsCompiler(JsCompileRequest request) => this.request = request;

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

    /// <summary>Whether the constant pool's ceiling has been met and refused, so it is refused once.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FE7A04
    // Broiler-Human:        PENDING
    private bool constantPoolFull;

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

    /// <summary>
    /// What a dynamic import written in the SCRIPT being lowered is resolved against.
    /// </summary>
    /// <remarks>
    /// <b>A module is its own referrer and a script has to be told what it is.</b> A module carries
    /// the key the composition resolved it to, so <c>import('./m')</c> inside one is resolved
    /// against that key without anybody being asked. A script has no key of its own - it is a text
    /// a host handed over - so the host says what a relative specifier in it means, and a host that
    /// says nothing gets the empty referrer its own resolver will have to make sense of.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=FAA825
    // Broiler-Human:        PENDING
    private string scriptReferrer = string.Empty;

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

    /// <summary>
    /// The block-level function declarations Annex B also gives a <c>var</c>-scoped binding.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Membership is a property of the DECLARATION and not of the name</b>, which is why this
    /// holds nodes rather than strings. <c>{ function f() { } { function f() { } } }</c> has two
    /// declarations of one name and the language admits only the outer one to the extension — the
    /// inner one is refused because <c>var f</c> written in its place would collide with the outer
    /// block's lexical <c>f</c> — so a set of names could not tell the two apart.
    /// </para>
    /// <para>
    /// <b>The comparer is reference identity because the nodes are records</b>, whose equality is
    /// their content: two textually identical declarations in two compiled sources would be one
    /// entry under the default comparer, and admitting one would admit the other.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=E9C427
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.HashSet<object> annexB =
        new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

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

    /// <summary>
    /// Whether the code being lowered is a class field initialiser's own body, where <c>arguments</c>
    /// names nothing.
    /// </summary>
    /// <remarks>
    /// <b>It is <see cref="insideMethod"/>'s pattern</b>: an arrow inherits it and any other function
    /// resets it. It is read by one thing, the request flags of a direct-<c>eval</c> site, because
    /// the early error an initialiser's own text gets is <see cref="RefuseArguments"/>'s and a
    /// String evaluated there has to be told (JSeal V14).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=4E6FAE
    // Broiler-Human:        PENDING
    private bool insideFieldInitialiser;

    /// <summary>
    /// The name a field initialiser's anonymous function takes, while that initialiser's own body is
    /// lowered, and <see langword="null"/> everywhere else.
    /// </summary>
    /// <remarks>
    /// <b>The initialiser's body is one <c>return</c> of the value the field was written with</b>, and
    /// the language names an anonymous function there after the field: <c>class C { f = function ()
    /// {} }</c> gives it <c>f</c>, and a private field <c>#f</c>. Every other function, an arrow
    /// included, resets it, so a <c>return</c> nested inside the value names nothing. A computed key
    /// leaves the name empty here, and the executor names the value instead (JSP-6, JSC-238).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=36E506
    // Broiler-Human:        PENDING
    private string? fieldValueName;

    /// <summary>
    /// The boundary scope of the evaluated program being lowered, or <see langword="null"/> when the
    /// source is not direct <c>eval</c> code.
    /// </summary>
    /// <remarks>
    /// <b>Every free name below it is a name of the caller's</b>, so the static load, store,
    /// <c>typeof</c>, <c>delete</c> and callee lowerings emit an eval name instruction addressed to
    /// this scope's record instead of a global instruction (JSD-0026 section 5). A name the program
    /// binds itself still resolves to its slot, exactly as it would anywhere else.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=9F98A2
    // Broiler-Human:        PENDING
    private Scope? evalRoot;

    /// <summary>What the calling site permitted the evaluated program being lowered.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=716B8F
    // Broiler-Human:        PENDING
    private JsFormat.EvalRequestFlags evalFlags;

    /// <summary>
    /// The private names the evaluated program being lowered uses and does not declare, spelled as
    /// their slots, which a class around its call must declare (JSeal V15-finish).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=169A3B
    // Broiler-Human:        PENDING
    private System.Collections.Generic.List<string> evalPrivateNames = [];

    /// <summary>
    /// The function scopes whose parameter lists are being lowered right now, innermost last.
    /// </summary>
    /// <remarks>
    /// <b>A direct <c>eval</c> in a parameter initialiser sees the function from its parameter
    /// list</b> (JSeal V15, JSD-0026 step 9): the specification evaluates the list against a record
    /// of its own, before the body's <c>var</c>s exist, so a site under one of these scopes gets the
    /// function's row as its parameter list sees it, with the parameters as bindings its
    /// declarations collide with. Since JSeal V15-finish such a function's body has a record of its
    /// own, pushed inside this one, and the row names nothing of the body's.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=CCB349
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.HashSet<Scope> parameterScopes =
        new(System.Collections.Generic.ReferenceEqualityComparer.Instance);

    /// <summary>What each evaluated program lowered into this artifact declares, by unit.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=6B1489
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<(int Unit, JsFormat.EvalRequestFlags Flags, JsFormat.EvalRefusal Refusal, string[] VarNames, string[] LexicalNames, string[] FunctionNames, string[] AnnexBNames, string[] PrivateNames)> evalDeclarations = [];

    /// <summary>
    /// What each script body lowered into this artifact declares at the global scope, by unit
    /// (JSeal V15-host): the rows the executor's <c>GlobalDeclarationInstantiation</c> checks read.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=954E36
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<(int Unit, string[] LexicalNames, string[] VarNames, string[] FunctionNames, string[] AnnexBNames)> scriptDeclarations = [];

    /// <summary>
    /// The referrer each script body lowered into this artifact was placed at, by unit (JSeal
    /// I12-upstream): the rows that let eval code and <c>Function</c> bodies the script creates carry
    /// it at run time.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=5D308E
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<(int Unit, string Referrer)> scriptReferrers = [];

    /// <summary>Compiles one source text as a script called <c>main</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=D206EC
    // Broiler-Human:        PENDING
    public static JsCompilation Compile(string source, SliceParseOptions options) =>
        Compile([new JsScriptUnit("main", source, options)]);

    /// <summary>Compiles several source texts into one artifact, one entry point each.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=24405A
    // Broiler-Human:        PENDING
    public static JsCompilation Compile(System.Collections.Generic.IReadOnlyList<JsScriptUnit> scripts) =>
        CompilationStack.Run(() =>
        {
            var compiler = new JsCompiler();
            return compiler.Run(scripts, []);
        });

    /// <summary>
    /// Reads the payload of a program request - an eval request, an embedder's script request, or
    /// the source text of a script - into the unit a provider compiles, or answers false for a payload no
    /// provider of this format may compile.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the dispatch source-provider version 2 obliges a provider to perform</b> (JSeal
    /// V14, JSD-0026 section 8), in one place so that every provider performs the same one. An eval
    /// request becomes a unit named <c>eval</c> under <see cref="SliceParseOptions.Eval"/> with the
    /// request's flags; an embedder's script request (JSeal V15-host) becomes a script named
    /// <c>main</c>, strict when the request says so and attributed to the request's source name; a
    /// payload that begins with either mark and is not a well-formed request, or that begins with any
    /// other reserved control byte, is answered false and a provider refuses it as a malformed
    /// encoding; anything else is the source text of a script named <c>main</c>, which is what the
    /// <c>Function</c> constructor sends.
    /// </para>
    /// <para>
    /// <b>A provider that applies a policy to guest evaluation reads the first byte itself</b>: only
    /// <see cref="JsFormat.ScriptRequestMark"/> is the embedder's own request, and every other program
    /// request is one a guest made.
    /// </para>
    /// <para>
    /// <b>A module request is not a program request</b> and is the caller's to recognise first, with
    /// <see cref="JsFormat.TryReadModuleRequest"/>: it names a graph a composition resolves, which is
    /// a question this component does not answer.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=3; Fingerprint=4CF8C0
    // Broiler-Falsified-If: a payload that begins with a reserved mark, or with the eval or script mark and no well-formed request, is answered as source to compile
    // Broiler-Human:        PENDING
    public static bool TryReadProgramRequest(System.ReadOnlySpan<byte> payload, out JsScriptUnit script)
    {
        if (JsFormat.TryReadEvalRequest(payload, out var flags, out var evaluated))
        {
            script = new JsScriptUnit("eval", evaluated, SliceParseOptions.Eval(flags));
            return true;
        }

        // AN EMBEDDER'S SCRIPT (JSeal V15-host): the script goal, strict when the embedder asked, and
        // every diagnostic attributed to the name it gave. Only the host surface writes this mark.
        if (JsFormat.TryReadScriptRequest(payload, out var scriptFlags, out var sourceName, out var text))
        {
            script = new JsScriptUnit(
                "main",
                text,
                SliceParseOptions.Script,
                (scriptFlags & JsFormat.ScriptRequestFlags.Strict) != 0)
            {
                SourceName = sourceName,
            };

            return true;
        }

        script = null!;

        if (payload.Length != 0 &&
            (payload[0] == JsFormat.EvalRequestMark || payload[0] == JsFormat.ScriptRequestMark ||
                JsFormat.StartsWithReservedMark(payload)))
        {
            return false;
        }

        script = new JsScriptUnit(
            "main", System.Text.Encoding.UTF8.GetString(payload), SliceParseOptions.Script);

        return true;
    }

    /// <summary>What one source text requests, so a composition can resolve and load it.</summary>
    /// <param name="Succeeded">Whether the source parsed.</param>
    /// <param name="Specifiers">
/// Every module request the source names, in source order: the specifier, and the type after it
/// when the request asks for one (<see cref="JsFormat.TypedSpecifier"/>).
/// </param>
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=846E05
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
                JsImportDeclaration import => JsFormat.TypedSpecifier(
                    import.Specifier, compiler.AttributeType(import.Span, import.Attributes, refuse: false)),
                JsExportDeclaration { From.Length: not 0 } exported => compiler.TypedFrom(exported),
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=326E18
    // Broiler-Human:        PENDING
    public static JsCompilation Compile(
        System.Collections.Generic.IReadOnlyList<JsScriptUnit> scripts,
        System.Collections.Generic.IReadOnlyList<JsModuleUnit> modules) =>
        CompilationStack.Run(() =>
        {
            var compiler = new JsCompiler();
            return compiler.Run(scripts, modules);
        });

    /// <summary>
    /// Compiles under a named feature manifest, into a named output form.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>THIS IS THE ONLY PLACE AN OUTPUT FORM IS EVER CHOSEN, AND IT IS CHOSEN ONCE.</b> The
    /// form travels into the artifact and no later stage may revisit it: the verifier pins it, the
    /// executor reads it, and a handle keeps the form it was verified with for as long as it
    /// exists. A second door - a property, an environment variable, a run-time probe - would be the
    /// second execution arm this profile's non-goals refuse, wearing a different hat.
    /// </para>
    /// <para>
    /// <b>The native form is a whole-artifact form under either manifest.</b> Every unit is
    /// emitted or the compilation is refused. Under the numeric manifest the emitted code computes
    /// over doubles; under the wide manifest it is the baseline form, whose emitted code calls into
    /// the interpreter's own dispatch at every block head, so every program the wide
    /// front end lowers can be emitted. There is no mixed artifact for a request to ask for.
    /// </para>
    /// <para>
    /// <b>A native request is answered by the backend it names.</b> The manifest is checked, the
    /// backend is looked up by name, the assembled program is handed over, and a refusal becomes a
    /// diagnostic naming what the backend declined - the arm64 backend declines the wide manifest,
    /// and the x86-64 backend declines a baseline emission past the format's native-code ceiling.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=106931
    // Broiler-Human:        PENDING
    public static JsCompilation Compile(
        System.Collections.Generic.IReadOnlyList<JsScriptUnit> scripts,
        System.Collections.Generic.IReadOnlyList<JsModuleUnit> modules,
        JsCompileRequest request) =>
        CompilationStack.Run(() =>
        {
            var compiler = new JsCompiler(request);
            return compiler.Run(scripts, modules);
        });


    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=80F932
    // Broiler-Human:        PENDING
    private JsCompilation Run(
        System.Collections.Generic.IReadOnlyList<JsScriptUnit> scripts,
        System.Collections.Generic.IReadOnlyList<JsModuleUnit> modules)
    {
        foreach (var script in scripts)
        {
            var first = diagnostics.Count;
            var tokenizer = new SliceTokenizer(script.Text)
            {
                HtmlLikeComments = script.Options.Goal != SliceGoal.Module,
            };

            var tokens = tokenizer.Tokenize();

            if (tokenizer.Diagnostics.Count != 0)
            {
                diagnostics.AddRange(tokenizer.Diagnostics);
                return Refused(first, script.SourceName);
            }

            // EVAL CODE IS PARSED UNDER ITS CALLER'S STRICTNESS AND FUNCTION CONTEXT, which is all
            // the parser can be told: `with` in strict code and `new.target` outside a function are
            // syntax errors of the parse, and the request flags are what say where the source is
            // being evaluated (JSD-0026 section 5).
            var evaluated = script.Options.IsEval;
            var forceStrict = script.ForceStrict ||
                (evaluated && (script.Options.EvalFlags & JsFormat.EvalRequestFlags.Strict) != 0);

            var parser = new JsParser(
                tokens,
                script.Options,
                forceStrict,
                enclosingFunctionDepth:
                    evaluated && (script.Options.EvalFlags & JsFormat.EvalRequestFlags.InFunction) != 0
                        ? 1
                        : 0)
            {
                AdmitsBigInt = request.AdmitsBigIntLiterals,
            };

            var program = parser.Parse();

            if (parser.Diagnostics.Count != 0)
            {
                diagnostics.AddRange(parser.Diagnostics);
                return Refused(first, script.SourceName);
            }

            // THE MANIFEST IS JUDGED BEFORE THE LOWERING RUNS, AND THAT ORDER IS WHAT MAKES A
            // REFUSAL A REFUSAL BY NAME. The lowering meets a construct as a node to emit code
            // for; the admission pass meets it as a construct with a name and a position, which is
            // what an author needs to be told. Running it afterwards would mean reporting whatever
            // the lowering happened to trip over first.
            if (request.Manifest == JsFeatureManifest.Numeric)
            {
                diagnostics.AddRange(JsNumericAdmission.Judge(program));

                if (diagnostics.Count != 0)
                {
                    return Refused(first, script.SourceName);
                }
            }

            scriptReferrer = script.Referrer;
            currentSource = AddSource(script.Text);
            var unit = evaluated
                ? CompileEvalProgram(program, script.Options.EvalFlags)
                : CompileProgram(program, script.ForceStrict);
            scriptReferrer = string.Empty;

            // A SCRIPT A HOST PLACED SAYS WHERE AT RUN TIME TOO (JSD-0024 section 20): its own
            // import() carries the referrer as an operand, and eval code or a Function body it
            // creates has no operand to carry, so the executor reads the script's row instead. Eval
            // code gets no row - its referrer is its caller's, which only the run can know.
            if (!evaluated && script.Referrer.Length != 0)
            {
                scriptReferrers.Add((unit, script.Referrer));
            }

            if (diagnostics.Count != 0)
            {
                return Refused(first, script.SourceName);
            }

            entries.Add((script.Name, (uint)unit));
        }

        // A MODULE GRAPH IS OUTSIDE THE NUMERIC MANIFEST AND IS REFUSED BEFORE ONE IS PARSED. The
        // module surface is resolution, live bindings and an evaluation order over a graph, and
        // none of the three is a Number; refusing the graph here rather than each `import` inside
        // it names the thing the caller actually asked for.
        if (request.Manifest == JsFeatureManifest.Numeric && modules.Count != 0)
        {
            Refuse(
                default,
                SliceSourceDiagnosticCode.ConstructOutsideManifest,
                "a module graph is not admitted by the declared feature manifest");

            return new JsCompilation(false, null, diagnostics);
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

            var text = module.Text;

            if (module.Type.Length != 0)
            {
                // A TYPED MODULE IS READ AS ITS TYPE SAYS, and JSON is the one type this front end
                // has a reading for: the document becomes the source of the synthetic module that
                // exports it as `default`, and a text that is not JSON is refused as the load would
                // refuse it.
                if (!string.Equals(module.Type, JsFormat.JsonModuleType, System.StringComparison.Ordinal))
                {
                    Refuse(
                        default,
                        SliceSourceDiagnosticCode.UnsupportedImportAttribute,
                        "the module `" + JsFormat.SplitTypedSpecifier(module.Key, out _) +
                            "` was loaded as the type `" + module.Type +
                            "`, which no composition of this profile has a reading for");

                    return new JsCompilation(false, null, diagnostics);
                }

                if (!JsJsonModule.TryTranslate(module.Text, out text, out var error, out var tooDeep))
                {
                    Refuse(
                        default,
                        tooDeep
                            ? SliceSourceDiagnosticCode.NestingTooDeep
                            : SliceSourceDiagnosticCode.InvalidJsonModule,
                        "the JSON module `" + JsFormat.SplitTypedSpecifier(module.Key, out _) + "` " +
                            (tooDeep ? "cannot be read: " : "is not JSON: ") + error);

                    return new JsCompilation(false, null, diagnostics);
                }
            }

            if (!TryParse(text, module.Options, forceStrict: true, out var program))
            {
                return new JsCompilation(false, null, diagnostics);
            }

            currentSource = AddSource(text);
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

        // THE ARTIFACT IS BUILT AND THEN THE DIAGNOSTICS ARE CHECKED, because assembly is the pass
        // that judges the manifest against what was actually emitted and that asks a backend for
        // machine code. Both of those can refuse, and a compilation that returned bytes it had
        // already refused would be a compilation whose answer contradicted its own diagnostics.
        var artifact = Assemble();

        return diagnostics.Count != 0
            ? new JsCompilation(false, null, diagnostics)
            : new JsCompilation(true, artifact, diagnostics);
    }

    /// <summary>
    /// The refusal of the script whose diagnostics begin at <paramref name="first"/>, each of them
    /// attributed to the name its unit was given.
    /// </summary>
    /// <remarks>
    /// Only this unit's diagnostics are named: a refusal stops the compilation at the first unit
    /// that has one, so everything from <paramref name="first"/> on is this unit's, and a
    /// diagnostic no unit owns - one the assembly of the whole artifact raises - stays unnamed.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=9CE1A7
    // Broiler-Human:        PENDING
    private JsCompilation Refused(int first, string sourceName)
    {
        if (sourceName.Length != 0)
        {
            for (var at = first; at < diagnostics.Count; at++)
            {
                diagnostics[at] = diagnostics[at] with { SourceName = sourceName };
            }
        }

        return new JsCompilation(false, null, diagnostics);
    }

    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=3; Fingerprint=63730F
    // Broiler-Human:        PENDING
    private bool TryParse(
        string text, SliceParseOptions options, bool forceStrict, out JsProgramNode program)
    {
        program = null!;
        awaited = false;
        var tokenizer = new SliceTokenizer(text) { HtmlLikeComments = options.Goal != SliceGoal.Module };
        var tokens = tokenizer.Tokenize();

        if (tokenizer.Diagnostics.Count != 0)
        {
            diagnostics.AddRange(tokenizer.Diagnostics);
            return false;
        }

        var parser = new JsParser(tokens, options, forceStrict) { AdmitsBigInt = request.AdmitsBigIntLiterals };
        program = parser.Parse();
        awaited = parser.SawTopLevelAwait;

        if (parser.Diagnostics.Count == 0)
        {
            return true;
        }

        diagnostics.AddRange(parser.Diagnostics);
        return false;
    }

    /// <summary>Every source text this compilation lowered while keeping source text.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=2182BA
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.List<string> sources = [];

    /// <summary>The source the unit being lowered was parsed from, or -1.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=FB6845
    // Broiler-Human:        PENDING
    private int currentSource = -1;

    /// <summary>Each unit's span in its source, by unit.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Low; Resources=1; Fingerprint=32A922
    // Broiler-Human:        PENDING
    private readonly System.Collections.Generic.Dictionary<int, (int Source, int Start, int End)> sourceSpans = [];

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

    /// <summary>
    /// How many statements that own exception regions enclose the cursor: a <c>try</c>, a
    /// <c>for … of</c> and a resource scope each count one.
    /// </summary>
    /// <remarks>
    /// <b>It is what tells a region which inlined unwinding it must not cover.</b> Code a
    /// <c>break</c>, <c>continue</c> or <c>return</c> emits to leave the statement at level N runs
    /// after that statement has completed, so no region of level N or deeper may catch what it
    /// throws; the regions of shallower statements still enclose it. See
    /// <see cref="UnitBuffer.Leave"/>.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=490FAD
    // Broiler-Human:        PENDING
    private int regionLevel;
}
