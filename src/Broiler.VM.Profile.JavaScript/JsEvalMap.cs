// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   15
// Annotated:        15/15
// Exempt:           18
// Human-reviewed:   0/15
// IP risk:          Low
// Security risk:    High
// Criteria:         3/3
// Resource impact:  2/10 max
// Unverified:       15
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>One compile-time scope a direct-<c>eval</c> site can see, as the verifier admitted it.</summary>
/// <remarks>
/// <b>The names are a table built at verification and read at run time</b>, so a name resolved
/// through the map costs one lookup per record walked and never a scan of the row. The slots are
/// not bounded here: the record a row describes may be a closure's, and the executor bounds each
/// slot where it reads one, exactly as it bounds a <c>LoadScoped</c>.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=5B71B7
// Broiler-Human:        PENDING
internal sealed class JsEvalShape(
    Format.JsFormat.EvalScopeKind kind,
    int parent,
    System.Collections.Generic.Dictionary<string, (int Slot, bool Immutable, bool Lexical, bool Hidden, bool FunctionName, bool Import)> names)
{
    /// <summary>What kind of record the scope is at run time.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=0F022D
    // Broiler-Human:        PENDING
    internal Format.JsFormat.EvalScopeKind Kind { get; } = kind;

    /// <summary>The enclosing scope's row, or -1 at a root.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=293935
    // Broiler-Human:        PENDING
    internal int Parent { get; } = parent;

    /// <summary>Finds the slot the scope binds <paramref name="name"/> to, if it binds it.</summary>
    /// <remarks>
    /// <paramref name="functionName"/> says the binding is a named function expression's own name,
    /// whose immutability is not strict: sloppy code's write to it is ignored (VM-FIX-D).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=86E0C6
    // Broiler-Human:        PENDING
    internal bool TryFind(string name, out int slot, out bool immutable, out bool functionName)
    {
        // A NAME THE ROW HIDES IS NOT BOUND FROM WHERE THE ROW IS SEEN: a function body's declaration,
        // seen from the function's own parameter list (JSeal V15). An import is not a slot at all,
        // and only TryFindImport answers it (JSeal V15-module).
        if (names.TryGetValue(name, out var found) && !found.Hidden && !found.Import)
        {
            slot = found.Slot;
            immutable = found.Immutable;
            functionName = found.FunctionName;
            return true;
        }

        slot = 0;
        immutable = false;
        functionName = false;
        return false;
    }

    /// <summary>
    /// Finds the import entry a module row binds <paramref name="name"/> to, if the name is one of
    /// the module's imports (JSeal V15-module).
    /// </summary>
    /// <remarks>
    /// The entry indexes the verified artifact's import table, which the verifier bounded it by; the
    /// binding it names is read through the exporting module's environment on every access.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=7B86ED
    // Broiler-Human:        PENDING
    internal bool TryFindImport(string name, out int entry)
    {
        if (names.TryGetValue(name, out var found) && found.Import)
        {
            entry = found.Slot;
            return true;
        }

        entry = 0;
        return false;
    }

    /// <summary>Whether the scope binds <paramref name="name"/> at all.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=ED3E0C
    // Broiler-Human:        PENDING
    internal bool Binds(string name) => names.TryGetValue(name, out var found) && !found.Hidden;

    /// <summary>
    /// Whether the scope binds <paramref name="name"/> as a lexical declaration, which a sloppy
    /// evaluation's <c>var</c> of the same name collides with (JSeal V15).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=05D8E8
    // Broiler-Human:        PENDING
    internal bool BindsLexically(string name) => names.TryGetValue(name, out var found) && found.Lexical;
}

/// <summary>One direct-<c>eval</c> site, as the verifier admitted it.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=982807
// Broiler-Human:        PENDING
internal sealed class JsEvalSite(int scope, Format.JsFormat.EvalRequestFlags flags)
{
    /// <summary>The row of the innermost scope the site sees.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=AA7453
    // Broiler-Human:        PENDING
    internal int Scope { get; } = scope;

    /// <summary>What an evaluation from the site is compiled under.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6336E5
    // Broiler-Human:        PENDING
    internal Format.JsFormat.EvalRequestFlags Flags { get; } = flags;
}

/// <summary>What one evaluated program declares, and whether this build runs it.</summary>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F844D4
// Broiler-Human:        PENDING
internal sealed class JsEvalDeclaration(
    Format.JsFormat.EvalRequestFlags flags,
    Format.JsFormat.EvalRefusal refusal,
    string[] varNames,
    string[] functionNames,
    string[] annexBNames,
    string[] privateNames)
{
    /// <summary>The request flags the program was compiled under.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=6336E5
    // Broiler-Human:        PENDING
    internal Format.JsFormat.EvalRequestFlags Flags { get; } = flags;

    /// <summary>Why the program is refused before it runs, if it is.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=05160A
    // Broiler-Human:        PENDING
    internal Format.JsFormat.EvalRefusal Refusal { get; } = refusal;

    /// <summary>
    /// The <c>var</c> names a sloppy program introduces into its caller's variable environment, none
    /// of which is also a top-level function's.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=DBEF61
    // Broiler-Human:        PENDING
    internal string[] VarNames { get; } = varNames;

    /// <summary>The top-level function declarations a sloppy program introduces, one per name.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=48F7EA
    // Broiler-Human:        PENDING
    internal string[] FunctionNames { get; } = functionNames;

    /// <summary>The block-level functions Annex B may hoist, decided per evaluation.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=380361
    // Broiler-Human:        PENDING
    internal string[] AnnexBNames { get; } = annexBNames;

    /// <summary>
    /// The private names the program uses and does not declare, spelled as their slots, each of
    /// which a class enclosing the call must declare (JSeal V15-finish).
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=551449
    // Broiler-Human:        PENDING
    internal string[] PrivateNames { get; } = privateNames;

    /// <summary>Whether the program introduces anything into its caller's variable environment.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=5D3AF3
    // Broiler-Human:        PENDING
    internal bool Introduces => VarNames.Length != 0 || FunctionNames.Length != 0 || AnnexBNames.Length != 0;
}

/// <summary>
/// The eval scope map of one verified artifact: its shapes, its sites by code offset, and its
/// eval-code units' declaration rows (JSD-0026 section 4).
/// </summary>
/// <remarks>
/// <b>It is immutable and holds nothing per instance</b>, like the rest of a verified program: the
/// live records a name is resolved in are the running frame's, and the map only says where in them
/// a name is.
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=DE8935
// Broiler-Falsified-If: a site, shape or declaration row reaches the executor without the verifier having checked it against the code and function tables
// Broiler-Human:        PENDING
internal sealed class JsEvalMap(
    JsEvalShape[] shapes,
    System.Collections.Generic.Dictionary<uint, JsEvalSite> sites,
    System.Collections.Generic.Dictionary<int, JsEvalDeclaration> declarations)
{
    /// <summary>The scope shapes, parents before children.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=921EEE
    // Broiler-Human:        PENDING
    internal JsEvalShape[] Shapes { get; } = shapes;

    /// <summary>Finds the site whose eval call instruction is at <paramref name="offset"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D41125
    // Broiler-Human:        PENDING
    internal bool TryFindSite(uint offset, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out JsEvalSite? site) =>
        sites.TryGetValue(offset, out site);

    /// <summary>Finds the declaration row of eval-code unit <paramref name="unit"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=377475
    // Broiler-Human:        PENDING
    internal bool TryFindDeclaration(
        int unit, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out JsEvalDeclaration? declaration) =>
        declarations.TryGetValue(unit, out declaration);
}

/// <summary>
/// What an eval boundary record was entered with: the caller's program and the site that asked.
/// </summary>
/// <remarks>
/// <para>
/// <b>It lives on the record, not on the engine, and that is the whole answer to nesting</b>
/// (JSD-0026 section 5). A closure the evaluated program created walks to the same boundary record
/// whenever it is called, long after the evaluation returned, and finds the same view; two
/// activations of one caller each enter their own boundary record with their own view; and an
/// evaluation inside evaluated code finds its caller's boundary on the chain and continues through
/// that one's view in turn.
/// </para>
/// <para>
/// <b>The caller's record is the boundary record's parent</b>, so it is not repeated here: the view
/// names only what the chain cannot say - which artifact's map describes the records past the
/// boundary, and which of its rows the walk starts at.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=6138EF
// Broiler-Falsified-If: a name resolved through a view reaches a record other than the ones the view's own site row describes
// Broiler-Human:        PENDING
internal sealed class JsEvalView
{
    /// <summary>A view of a site in <paramref name="program"/>'s map.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=087633
    // Broiler-Human:        PENDING
    internal JsEvalView(JsProgram program, JsEvalSite site)
    {
        Program = program;
        Site = site;
    }

    /// <summary>A view of the global scope alone: an indirect evaluation, or a script's top level.</summary>
    /// <remarks>
    /// <b>Nothing lies between such an evaluation and the global environment</b> (JSeal V15,
    /// JSD-0026 step 8): an indirect <c>eval</c> is global code by definition, and a direct one at a
    /// script's top level with no record around the call sees the global scope and nothing else.
    /// Every name the program does not bind resolves in the realm's global lexical half and then on
    /// the global object, and its variable environment is the global one.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=D5BEDA
    // Broiler-Human:        PENDING
    private JsEvalView()
    {
    }

    /// <summary>A fresh view of the global scope, one per evaluation.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=71455B
    // Broiler-Human:        PENDING
    internal static JsEvalView Global() => new();

    /// <summary>
    /// The caller's verified program, whose map describes the records past the boundary, or
    /// <see langword="null"/> in a view of the global scope.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=126BC3
    // Broiler-Human:        PENDING
    internal JsProgram? Program { get; }

    /// <summary>The site the evaluation was asked from, or <see langword="null"/> in a view of the global scope.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=F5F1B4
    // Broiler-Human:        PENDING
    internal JsEvalSite? Site { get; }

    /// <summary>
    /// The function record a sloppy evaluation's declarations went to, or <see langword="null"/>
    /// when they went to the global environment or the evaluation declared nothing outside itself.
    /// </summary>
    /// <remarks>
    /// Set once, by the declaration instantiation that runs before the program's first instruction
    /// (JSeal V15); <see cref="VariableShape"/> is the map row that describes the record.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=01FC74
    // Broiler-Human:        PENDING
    internal JsEnvironment? VariableRecord { get; set; }

    /// <summary>The map row describing <see cref="VariableRecord"/>, when it is set.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=68B6F5
    // Broiler-Human:        PENDING
    internal JsEvalShape? VariableShape { get; set; }

    /// <summary>
    /// The names the declaration instantiation declared that the program writes to its variable
    /// environment directly: its top-level functions and the Annex B aliases that were hoisted.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> until an instantiation declared one; an Annex B alias missing from it
    /// was kept from hoisting by the caller's own bindings, and its write is skipped.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=8C417F
    // Broiler-Human:        PENDING
    internal System.Collections.Generic.HashSet<string>? Declared { get; set; }
}

/// <summary>
/// The bindings direct evaluations introduced into one function's variable environment (JSeal V15,
/// JSD-0026 step 6).
/// </summary>
/// <remarks>
/// <para>
/// <b>It is an object only so that the one by-name lookup the chain already has can ask it</b>:
/// <c>ResolveName</c> searches a <c>with</c> object by its own properties, and a function whose own
/// code contains a direct <c>eval</c> is searched the same way for the names an evaluation declared
/// there, each a deletable binding. It has no prototype, so a search is a question about its own
/// properties and nothing else, and its properties are plain writable data.
/// </para>
/// <para>
/// <b>No guest code ever holds it.</b> It hangs off the function's record, not off any object a
/// program can reach; a call through a name it answered is given <c>undefined</c> as its receiver by
/// <see cref="Format.JsOpcode.WithBaseObject"/>; and its <c>Symbol.unscopables</c> is never read. So
/// it cannot gain an accessor, a prototype or a non-configurable property, which is what keeps every
/// binding in it a mutable, deletable binding.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=60C215
// Broiler-Falsified-If: an eval-variables object becomes reachable from guest code, as a receiver, a property value or through a prototype
// Broiler-Human:        PENDING
internal sealed class JsEvalVariables : JsObject
{
    /// <summary>Creates an empty set with no prototype.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=A8E714
    // Broiler-Human:        PENDING
    internal JsEvalVariables()
        : base(null)
    {
    }
}
