// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   52
// Annotated:        52/52
// Exempt:           59
// Human-reviewed:   0/52
// IP risk:          None
// Security risk:    High
// Criteria:         1/1
// Resource impact:  1/10 max
// Unverified:       52
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// Format version 2: the sections, vocabularies and structural ceilings the
/// <c>broiler.javascript.wide</c> surface adds to version 1.
/// </summary>
/// <remarks>
/// <para>
/// The framing is version 1's, unchanged: the same magic, a variable-length format version, a
/// manifest identity, then a declared section count and a sequence of framed sections in strictly
/// ascending kind order. A version-2 artifact is refused by a version-1 reader because the version
/// integer differs, and not because a section it did not expect turned up.
/// </para>
/// <para>
/// <b>What version 2 adds is a function table and an environment model.</b> Version 1 declares one
/// frame and one flat set of locals, which is what a program with no functions needs. This version
/// declares a code unit per function - parameters, environment slots, operand-stack maximum, code
/// range and flags - and every binding lives in an environment record reached by a static (depth,
/// slot) pair. Nothing addresses a variable by name at run time except a global, which is a
/// property of an object and therefore a name by definition.
/// </para>
/// <para>
/// <b>Exception regions carry a scope depth and a stack height.</b> A handler that did not know
/// both would have to reconstruct them by walking back, and a handler entered with the wrong
/// operand-stack height is exactly the defect a verifier exists to make unrepresentable.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=42DA3A
// Broiler-Human:        PENDING
public static class JsFormat
{
    /// <summary>The format version this surface is written and read at.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=BA7B98
    // Broiler-Human:        PENDING
    public const uint FormatVersion = 2;

    /// <summary>The feature manifest this format version is defined against.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=D84DC4
    // Broiler-Human:        PENDING
    public const string ManifestId = "broiler.javascript.wide";

    /// <summary>The section kinds version 2 adds to <see cref="JavaScriptFormat.SectionKind"/>.</summary>
    /// <remarks>
    /// The numbering continues version 1's rather than restarting, so one reader can name a section
    /// kind without first knowing which format version it is reading. Kinds 1 to 7 keep their
    /// version-1 meanings; their bodies are read under version 2's rules where those differ, and
    /// the two places they differ - the limits body and the exception-region body - say so.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=9D29BE
    // Broiler-Human:        PENDING
    public enum SectionKind : uint
    {
        /// <summary>Declared maxima: operand stack, environment slots, frames, constants.</summary>
        Limits = 1,

        /// <summary>The constant pool.</summary>
        Constants = 2,

        /// <summary>The instruction stream, holding every code unit's code back to back.</summary>
        Code = 3,

        /// <summary>The named entry points, each naming a code unit rather than a code offset.</summary>
        Entries = 4,

        /// <summary>Exception regions, each carrying a scope depth and an operand-stack height.</summary>
        ExceptionRegions = 5,

        /// <summary>Suspension and resume targets. Framed, and admitted by no manifest.</summary>
        SuspensionTargets = 6,

        /// <summary>The canonical bytecode-offset to source-position table.</summary>
        Positions = 7,

        /// <summary>The code units: one row per function, plus row zero for the program body.</summary>
        Functions = 8,

        /// <summary>
        /// The optional feature manifests this artifact declares beside the one it names in its
        /// header.
        /// </summary>
        /// <remarks>
        /// It is optional and its absence means the artifact declares none, which is what every
        /// artifact written before this kind existed says. See <see cref="JsSurfaces"/> for why a
        /// surface made only of globals has to be declared at all.
        /// </remarks>
        Surfaces = 9,

        /// <summary>
        /// The module records: what each module of a graph requests, imports and exports.
        /// </summary>
        /// <remarks>
        /// Admitted only by an artifact that declares <see cref="JsSurfaces.Modules"/> beside its
        /// manifest, for the reason every optional surface is declared: a composition has to be
        /// able to decline module resolution separately from admitting objects and closures.
        /// </remarks>
        Modules = 10,

        /// <summary>
        /// The emitted machine code: an architecture, a backend version, an alignment, a length,
        /// and the bytes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>THE ORDINARY CODE SECTION STAYS IN THE SAME ARTIFACT AND IS NOT REPLACED BY THIS
        /// ONE.</b> Three separate things need it and none of them is a fallback: the differential
        /// oracle runs the same source under both forms and compares the transcripts, which is the
        /// compensating control a backend may not ship without; re-emission-equality verification
        /// recompiles the carried bytecode with the same deterministic backend and compares the
        /// result to these bytes, which is the only sense in which machine code can be verified at
        /// all; and a reader who cannot see the bytecode cannot check either claim. An artifact
        /// that dropped the bytecode would be asking to be trusted rather than read.
        /// </para>
        /// <para>
        /// <b>It is NOT an interpreter fallback, and the distinction is the whole of this
        /// profile's non-goal.</b> A verified handle has one form, fixed when the artifact was
        /// compiled; the presence of both sections does not make the form a run-time choice, and a
        /// code path that picked between them by observing a run would be the second execution arm
        /// that paragraph refuses.
        /// </para>
        /// </remarks>
        NativeCode = 11,

        /// <summary>
        /// Which code unit each run of emitted code belongs to: a count, then a function index and
        /// an offset per unit.
        /// </summary>
        /// <remarks>
        /// It is a section of its own rather than a column on the function rows, because a function
        /// row is read by every artifact of this format version and a native offset means nothing
        /// to the overwhelming majority of them. A column that was zero in every artifact but a
        /// handful would be a field readers learn to skip.
        /// </remarks>
        NativeSymbols = 12,

        /// <summary>
        /// The eval scope map: what each direct-<c>eval</c> site can see, and what each evaluated
        /// program declares.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Three tables: scope shapes, sites and eval declarations</b> (JSD-0026 section 4). A
        /// scope shape is one compile-time scope that is visible from at least one site - its kind,
        /// its parent and the names it binds, each with a slot and binding flags. A site row names a
        /// <see cref="JsOpcode.CallEval"/> or <see cref="JsOpcode.CallEvalSpread"/> by its code
        /// offset, the innermost shape it sees, how many of those shapes lie inside its own code unit,
        /// and the <see cref="EvalRequestFlags"/> an evaluation from it is asked under. An eval
        /// declaration row belongs to a unit flagged <see cref="FunctionFlags.EvalCode"/> and says
        /// what the evaluated program declares and whether this build refuses to run it.
        /// </para>
        /// <para>
        /// <b>It is optional, and its absence means "no site sees anything"</b>, which is what every
        /// artifact written before the kind existed says: a direct <c>eval</c> in a function unit with
        /// no row keeps the explicit refusal it always had. It is admitted only beside the
        /// <see cref="JsSurfaces.Dynamic"/> surface, because a map nobody can evaluate against is a
        /// declaration of a surface the artifact does not reach. <b>It grants nothing</b>: it names
        /// slots the caller's own instructions could already reach, and contains no code.
        /// </para>
        /// </remarks>
        EvalScopes = 13,

        /// <summary>
        /// The script declarations: what each script body declares at the global scope, so the
        /// executor can run <c>GlobalDeclarationInstantiation</c>'s checks before its first
        /// instruction.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>One row per script body that declares anything</b> (JSeal V15-host, JSD-0024 section
        /// 15): its unit, then four runs of interned names - its top-level <c>let</c>, <c>const</c> and
        /// <c>class</c> declarations, its <c>var</c> names that are no top-level function's, its
        /// top-level function names (one per name), and the block-level functions Annex B may hoist
        /// (sloppy only). The instructions that CREATE the bindings are unchanged; the row is what
        /// lets every conflict and definability check run first, so a script that fails one creates
        /// nothing.
        /// </para>
        /// <para>
        /// <b>It is optional, and its absence means "no checks"</b>, which is what every artifact
        /// written before the kind existed says: such a script keeps the lenient instantiation it
        /// always had. It is admitted beside every manifest, because every manifest has scripts, and
        /// <b>it grants nothing</b>: it names globals the body's own instructions create anyway.
        /// </para>
        /// </remarks>
        ScriptDeclarations = 14,

        /// <summary>
        /// The script referrers: what each script body is placed at, so a dynamic <c>import()</c> in
        /// eval code or in a <c>Function</c> body it creates carries it (JSeal I12-upstream).
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>One row per script body a host placed</b> (JSD-0024 section 20): its unit and an
        /// interned name, the referrer the compilation was given for it (<c>JsScriptUnit.Referrer</c>).
        /// A dynamic <c>import()</c> written in the script itself already carries that referrer as
        /// its operand; what the row adds is the script's identity at run time, which is what the
        /// language's <c>GetActiveScriptOrModule</c> answers for code that has no referrer of its own -
        /// eval code the script evaluates, and a function the <c>Function</c> constructor makes while
        /// the script is running.
        /// </para>
        /// <para>
        /// <b>It is optional, and its absence means "placed nowhere"</b>, which is what every artifact
        /// written before the kind existed says and what a script compiled with no referrer says:
        /// such code offers its embedder an empty referrer, as before. It is admitted beside every
        /// manifest and <b>it grants nothing</b>: it is a name a resolver reads, and no module reaches
        /// a realm by it.
        /// </para>
        /// </remarks>
        ScriptReferrers = 15,
    }

    /// <summary>What one import entry binds its local name to.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=7A6F7F
    // Broiler-Human:        PENDING
    public enum ImportKind : byte
    {
        /// <summary>One exported name of the requested module: <c>import { a } from …</c>.</summary>
        Named = 0,

        /// <summary>The requested module's namespace object: <c>import * as ns from …</c>.</summary>
        Namespace = 1,
    }

    /// <summary>The constant-pool entry tags version 2 reads.</summary>
    /// <remarks>
    /// Tags 1 to 4 are version 1's, with the same payloads. <see cref="String"/> is new, and
    /// <see cref="JavaScriptFormat.ConstantTag.InternedName"/> - reserved by version 1 and admitted
    /// by no manifest there - is admitted here. The two are distinct because a property name is
    /// interned once per program and compared by reference, and a String value is a value.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=FD3A5D
    // Broiler-Human:        PENDING
    public enum ConstantTag : byte
    {
        /// <summary>The one value <c>undefined</c>. No payload.</summary>
        Undefined = 1,

        /// <summary>A Boolean. One payload byte, which must be 0 or 1.</summary>
        Boolean = 2,

        /// <summary>A Number. Eight payload bytes, IEEE 754 binary64, little-endian.</summary>
        Number = 3,

        /// <summary>A property name: a length-prefixed UTF-8 run, interned at load.</summary>
        InternedName = 4,

        /// <summary>A String value: a length-prefixed UTF-8 run.</summary>
        String = 5,

        /// <summary>The one value <c>null</c>. No payload.</summary>
        Null = 6,

        /// <summary>
        /// A BigInt: one sign byte (0 or 1), a variable-length byte count, then the magnitude's
        /// bytes least significant first.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>It is admitted only beside <see cref="JsSurfaces.BigInt"/></b>, which no composition
        /// shipped at this build admits (decision JSD-0033). A well-formed one in an otherwise valid
        /// artifact without that declaration is refused with the code a build without the tag
        /// answered - an unknown constant tag - so every artifact written before the tag existed
        /// keeps its meaning and every older reader refuses the new bytes by name rather than
        /// misreading them. (The payload is decoded first, so a malformed one is refused as
        /// malformed, and the refusal is issued once every section has been read.)
        /// </para>
        /// <para>
        /// <b>The encoding is canonical</b>: zero is sign 0 with no bytes, and otherwise the last
        /// byte is not zero. A payload wider than <see cref="CeilingBigIntConstantBytes"/> is refused
        /// before a byte of it is read.
        /// </para>
        /// </remarks>
        BigInt = 7,
    }

    /// <summary>The flag bits a code-unit row carries.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=EEC5BA
    // Broiler-Human:        PENDING
    [System.Flags]
    public enum FunctionFlags : uint
    {
        /// <summary>No flag.</summary>
        None = 0,

        /// <summary>The unit's code is strict-mode code.</summary>
        Strict = 1,

        /// <summary>The unit is an arrow function: it has no <c>this</c> of its own.</summary>
        Arrow = 2,

        /// <summary>The unit reads <c>arguments</c>, so the frame materialises one.</summary>
        UsesArguments = 4,

        /// <summary>The unit is the program body rather than a function.</summary>
        ProgramBody = 8,

        /// <summary>The unit may be used as a constructor.</summary>
        Constructible = 16,

        /// <summary>
        /// The unit is a class constructor: calling it without <c>new</c> is a <c>TypeError</c>.
        /// </summary>
        /// <remarks>
        /// It is a flag on the unit rather than a check the lowering emits, because the refusal has
        /// to happen at every call site including the ones the lowering never sees - a method
        /// handed to <c>Array.prototype.map</c>, a constructor reached through
        /// <c>Function.prototype.call</c>. A guard in the callee's own first instruction would
        /// answer for none of them, since the call never reaches the callee's code.
        /// </remarks>
        ClassConstructor = 32,

        /// <summary>
        /// The unit is the constructor of a class with a heritage: its <c>this</c> does not exist
        /// until <c>super()</c> returns.
        /// </summary>
        /// <remarks>
        /// This is what makes a derived constructor more than sugar. The frame is entered with no
        /// <c>this</c> at all rather than with a fresh object, so reading <c>this</c> early is a
        /// <c>ReferenceError</c> and the object the constructor ends up with is the one the BASE
        /// constructor made from <c>new.target</c> - which is how an instance of a three-deep chain
        /// gets the prototype of the class that was actually constructed.
        /// </remarks>
        DerivedConstructor = 64,

        /// <summary>
        /// The unit binds its own parameters, so the frame copies no argument into a slot.
        /// </summary>
        /// <remarks>
        /// <b>Without this flag <c>ParameterCount</c> means two things at once, and they part
        /// company the moment a parameter list stops being simple.</b> It is the arity the function
        /// reports as <c>length</c> - which counts nothing at or after the first default and never
        /// counts a rest parameter - and it is how many arguments the frame copies into slots zero
        /// upward. A default has to run code, a rest parameter has to build an Array and a pattern
        /// has to destructure, so those units bind their parameters in their own prologue and this
        /// flag is what tells the frame to keep its hands off. The slots then start EMPTY, which is
        /// what makes a default reading a later parameter the <c>ReferenceError</c> the
        /// specification says it is rather than a read of <c>undefined</c>.
        /// </remarks>
        BindsParameters = 128,

        /// <summary>
        /// The unit is a generator body: calling it builds a generator object rather than running
        /// it, and it is the only kind of unit whose code may suspend.
        /// </summary>
        /// <remarks>
        /// <b>It is a flag on the unit and not a property of the call site</b>, because whether an
        /// invocation gets a heap-allocated frame has to be decidable before any of its code runs.
        /// The verifier refuses a suspension opcode in a unit without this bit, so a frame the
        /// executor did not allocate can never be the frame an instruction tries to suspend.
        /// </remarks>
        Generator = 256,

        /// <summary>
        /// The unit is an async function body: calling it STARTS the body and answers a promise,
        /// and it is the only kind of unit whose code may <c>await</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>It is not <see cref="Generator"/> with a different driver, and the difference is
        /// observable from the first line.</b> A generator is suspended-start: calling it runs no
        /// instruction. An async function is not: its body runs synchronously up to the first
        /// <c>await</c>, so <c>async function f(){ print(1); await 0; } f(); print(2)</c> prints
        /// <c>1</c> before <c>2</c>, and a unit flagged as both would have to be one of the two.
        /// The verifier refuses the pairing rather than choosing.
        /// </para>
        /// <para>
        /// <b>It pairs with <see cref="Arrow"/> and <see cref="Generator"/> does not.</b> An async
        /// arrow is an ordinary arrow whose body may suspend - it has no <c>this</c>,
        /// <c>new.target</c> or <c>super</c> of its own and reads the enclosing function's - so the
        /// frame it suspends on has to carry what an arrow's frame is entered with, which is why
        /// the frame records a <c>new.target</c> and a <c>this</c> box that a generator's never
        /// needs.
        /// </para>
        /// </remarks>
        Async = 512,

        /// <summary>
        /// The unit is the entry of an evaluated program: <c>eval</c> code, compiled for one direct
        /// evaluation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Its free names are resolved through the caller, not through the global object.</b> The
        /// unit's own record is the eval boundary: its <c>let</c>, <c>const</c> and <c>class</c>
        /// declarations, and in strict code its <c>var</c> and function declarations, are slots of
        /// that record, and every name it does not bind is reached by the eval name instructions,
        /// which walk to the boundary and ask the scope map the caller's site row names (JSD-0026
        /// section 5).
        /// </para>
        /// <para>
        /// <b>A unit carrying it carries an eval declaration row</b>, is not a program body, and is
        /// none of the function kinds: the verifier refuses the flag beside
        /// <see cref="ProgramBody"/>, <see cref="Arrow"/>, <see cref="Constructible"/>,
        /// <see cref="ClassConstructor"/>, <see cref="DerivedConstructor"/>,
        /// <see cref="BindsParameters"/>, <see cref="UsesArguments"/>, <see cref="Generator"/> or
        /// <see cref="Async"/>.
        /// </para>
        /// </remarks>
        EvalCode = 1024,
    }

    /// <summary>What kind of record one row of the eval scope map describes.</summary>
    /// <remarks>
    /// <b>The two roots are the ends of every chain.</b> A <see cref="Program"/> row is a script
    /// body's entry record, and past it the chain continues in the realm's global scope; an
    /// <see cref="Eval"/> row is the boundary record of an evaluated program, and past it the chain
    /// continues in whatever its own caller's site saw. Every other kind has a parent. A
    /// <see cref="With"/> row names nothing: its names are whatever its object has when it is asked.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=38A7BA
    // Broiler-Human:        PENDING
    public enum EvalScopeKind : byte
    {
        /// <summary>A function's own record: parameters, <c>var</c>s, functions and <c>arguments</c>.</summary>
        Function = 1,

        /// <summary>A block, loop head, <c>switch</c> body or class record.</summary>
        Block = 2,

        /// <summary>A <c>catch</c> clause's record, which Annex B.3.4 treats differently (V15).</summary>
        Catch = 3,

        /// <summary>The object environment record a <c>with</c> statement pushes.</summary>
        With = 4,

        /// <summary>A script body's entry record: the root of a chain that ends in the global scope.</summary>
        Program = 5,

        /// <summary>An evaluated program's boundary record: the root of a chain that ends in its caller's.</summary>
        Eval = 6,

        /// <summary>
        /// A function body's own variable environment, which the unit pushes inside its parameters'
        /// record when the parameter list has expressions (JSeal V15-finish): the variable
        /// environment of a direct eval in the body.
        /// </summary>
        FunctionBody = 7,

        /// <summary>
        /// A module's own environment: the root of a chain that ends in the global scope, whose
        /// names are the module's slots and its imports (JSeal V15-module).
        /// </summary>
        /// <remarks>
        /// An import is listed with <see cref="EvalBindingImport"/>, and its slot is then its entry
        /// in the artifact's import table: an evaluation reads it through the exporting module's
        /// environment every time, as <see cref="JsOpcode.LoadImport"/> does.
        /// </remarks>
        Module = 8,
    }

    /// <summary>The binding flag that makes a write through the eval scope map a <c>TypeError</c>.</summary>
    /// <remarks>
    /// It is the same fact the lowering's own <c>constant</c> answer carries, and a store through the
    /// map throws where <see cref="JsOpcode.ThrowImmutable"/> would have thrown for the same name.
    /// <see cref="EvalBindingFunctionName"/> is admitted only together with this one;
    /// <see cref="EvalBindingFlagBits"/> lists every bit a name's flags byte may carry.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=A80A45
    // Broiler-Human:        PENDING
    public const byte EvalBindingImmutable = 1;

    /// <summary>
    /// The binding flag that marks a named function expression's own name, which is immutable but
    /// not strict: a write through the map is ignored by sloppy eval code and a <c>TypeError</c> in
    /// strict eval code.
    /// </summary>
    /// <remarks>
    /// It is the same fact the lowering answers with for a static store to that name, and it is
    /// only admitted together with <see cref="EvalBindingImmutable"/>, so a reader that knows only
    /// that bit still refuses the write rather than performing it (VM-FIX-D).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=1F4DBD
    // Broiler-Human:        PENDING
    public const byte EvalBindingFunctionName = 8;

    /// <summary>
    /// The binding flag that makes a name a lexical declaration a sloppy evaluation's <c>var</c> of
    /// the same name collides with (JSeal V15).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is what <c>EvalDeclarationInstantiation</c>'s conflict walk asks each record</b>: a name
    /// flagged with it between a direct-<c>eval</c> site and the variable environment makes an
    /// evaluation that declares the same name as a <c>var</c> or a function a <c>SyntaxError</c>
    /// before anything is created, and keeps an Annex B block function of that name from being
    /// hoisted. A name without it is a <c>var</c>, a parameter, a function, <c>arguments</c> or a
    /// catch clause's parameter - which the walk passes, the last because Annex B.3.4 exempts the
    /// record of a catch clause whatever the form of its parameter.
    /// </para>
    /// <para>
    /// <b>A function row needs it and the others merely carry it.</b> This lowering keeps a
    /// function's top-level <c>let</c>, <c>const</c> and <c>class</c> in the function's own record
    /// beside its <c>var</c>s, where the specification has two records; the flag is what tells the
    /// two halves apart. A map written before the flag existed carries none, and an evaluation
    /// against it finds no conflict a lexical declaration of its caller's should have raised - nor
    /// do that artifact's closures search the names the evaluation introduces. No revision marker
    /// tells such a map apart: the JSeal V14 lowering that wrote them was never an accepted format,
    /// the retained corpus was rewritten with JSeal V15, and an artifact a V14 build wrote has to be
    /// compiled again rather than run under this executor.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=906733
    // Broiler-Human:        PENDING
    public const byte EvalBindingLexical = 2;

    /// <summary>
    /// The binding flag that makes a name of a function's record invisible from a site in the
    /// function's own parameter list (JSeal V15, JSD-0026 step 9).
    /// </summary>
    /// <remarks>
    /// <b>This lowering keeps a function's parameters and its body's declarations in one record</b>,
    /// where the specification evaluates the parameter list before the body's variable environment
    /// exists. A row describing the function as a parameter-initialiser site sees it carries the
    /// parameters with <see cref="EvalBindingLexical"/> - a <c>var</c> the evaluation declares of the
    /// same name collides with them - and every name the body declares with this flag: a name
    /// resolved through the row passes it by, and an evaluation that declares one gets a binding of
    /// the function's eval-variables set, which the parameter list's later code finds and which the
    /// body's own declaration of the name - a slot, found first - shadows for the body, as the
    /// specification's separate variable environment does.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=17E7F0
    // Broiler-Human:        PENDING
    public const byte EvalBindingHidden = 4;

    /// <summary>
    /// The binding flag that makes a name of a module row an import: its slot is an entry of the
    /// artifact's import table, not a slot of the module's record (JSeal V15-module).
    /// </summary>
    /// <remarks>
    /// <b>An imported binding is an indirection and never a copy</b>, so a read through the map goes
    /// to the exporting module's environment on every access - its dead zone included - and a write
    /// is the <c>TypeError</c> every assignment to an import is. The flag is admitted only on a
    /// <see cref="EvalScopeKind.Module"/> row and only together with <see cref="EvalBindingImmutable"/>,
    /// so a reader that knows only that bit still refuses the write.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=A95AA7
    // Broiler-Human:        PENDING
    public const byte EvalBindingImport = 16;

    /// <summary>Every binding flag an eval scope map row may carry.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=A87556
    // Broiler-Human:        PENDING
    public const byte EvalBindingFlagBits =
        EvalBindingImmutable | EvalBindingLexical | EvalBindingHidden | EvalBindingFunctionName |
        EvalBindingImport;

    /// <summary>
    /// What a direct-<c>eval</c> site tells the compiler of the evaluated source, before it is parsed.
    /// </summary>
    /// <remarks>
    /// <b>These are exactly the facts the specification's <c>PerformEval</c> reads from the calling
    /// context</b>: whether the caller is strict, whether it is inside a function (so
    /// <c>new.target</c> parses), a method (so a <c>super</c> property does), a derived constructor
    /// (so <c>super()</c> does) or a class field initialiser (so <c>arguments</c> does not), plus
    /// one fact of this profile's: whether an enclosing class declares private names, which the
    /// scope map does not describe and an evaluation that names one is therefore refused by name
    /// rather than parsed as an error it is not.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=8F2E16
    // Broiler-Human:        PENDING
    [System.Flags]
    public enum EvalRequestFlags : byte
    {
        /// <summary>A sloppy caller at a script's top level.</summary>
        None = 0,

        /// <summary>The caller is strict code, so the evaluated source is too.</summary>
        Strict = 1,

        /// <summary>The caller is inside a non-arrow function, so <c>new.target</c> is admitted.</summary>
        InFunction = 2,

        /// <summary>The caller is inside a method, so a <c>super</c> property parses.</summary>
        InMethod = 4,

        /// <summary>The caller is inside a derived constructor, so <c>super()</c> parses.</summary>
        InDerivedConstructor = 8,

        /// <summary>The caller is a class field initialiser, so <c>arguments</c> is a syntax error.</summary>
        InClassFieldInitializer = 16,

        /// <summary>An enclosing class declares private names the scope map does not describe.</summary>
        InClassBody = 32,
    }

    /// <summary>Every bit <see cref="EvalRequestFlags"/> defines.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=BB6DCA
    // Broiler-Human:        PENDING
    public const EvalRequestFlags EvalRequestFlagBits =
        EvalRequestFlags.Strict | EvalRequestFlags.InFunction | EvalRequestFlags.InMethod |
        EvalRequestFlags.InDerivedConstructor | EvalRequestFlags.InClassFieldInitializer |
        EvalRequestFlags.InClassBody;

    /// <summary>Why this build refuses to run an evaluated program that compiled.</summary>
    /// <remarks>
    /// <b>A refusal is data in the evaluated artifact and not a compile failure</b>, because a
    /// compile failure is what the guest sees as a <c>SyntaxError</c> and none of these is one: each
    /// is a program the language admits whose semantics this build does not yet implement, and the
    /// executor answers it with an explicit <c>EvalError</c> before the first instruction runs.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=193C09
    // Broiler-Human:        PENDING
    public enum EvalRefusal : byte
    {
        /// <summary>Nothing is refused.</summary>
        None = 0,

        /// <summary>
        /// A sloppy evaluation declares a <c>var</c> or a function in its caller's variable scope.
        /// </summary>
        /// <remarks>
        /// JSeal V14 wrote it for every such evaluation; since JSeal V15 (JSD-0026 steps 6-8) the
        /// lowering writes it for none, because the executor instantiates the declarations, and the
        /// value stays defined so that an evaluated artifact carrying it is still refused by name.
        /// </remarks>
        VarDeclarations = 1,

        /// <summary>The evaluated source refers to its caller's <c>super</c>.</summary>
        /// <remarks>
        /// JSeal V14 and V15 wrote it; since JSeal V15-finish the lowering writes it for none, because
        /// the eval frame answers <c>super</c> through its caller's method, and the value stays
        /// defined so that an evaluated artifact carrying it is still refused by name.
        /// </remarks>
        SuperReference = 2,

        /// <summary>The evaluated source names a private name its caller's class may declare.</summary>
        /// <remarks>
        /// JSeal V14 and V15 wrote it; since JSeal V15-finish the lowering writes it for none, because
        /// the caller's map carries its classes' private names and the declaration row lists the ones
        /// the program uses, and the value stays defined so that an evaluated artifact carrying it is
        /// still refused by name.
        /// </remarks>
        PrivateName = 3,
    }

    /// <summary>What an exception region does when control reaches its handler.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=735820
    // Broiler-Human:        PENDING
    public enum HandlerKind : byte
    {
        /// <summary>A <c>catch</c> clause: the handler consumes the thrown value.</summary>
        Catch = 0,

        /// <summary>
        /// The exceptional path of a <c>finally</c>: the handler runs the block and rethrows.
        /// </summary>
        /// <remarks>
        /// The rethrow is emitted by the lowering rather than performed by an opcode, so a
        /// <c>finally</c> that returns or breaks is a jump out of the handler and needs nothing
        /// from the executor that a <c>catch</c> does not already need.
        /// </remarks>
        Finally = 1,
    }

    /// <summary>The most operand-stack entries one code unit may declare.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=9A560B
    // Broiler-Human:        PENDING
    public const uint CeilingOperandStack = 4096;

    /// <summary>The most environment slots one scope may declare.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=932F8B
    // Broiler-Human:        PENDING
    public const uint CeilingScopeSlots = 65_535;

    /// <summary>The most code units one artifact may declare.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=8C4C88
    // Broiler-Human:        PENDING
    public const uint CeilingFunctions = 65_536;

    /// <summary>The most constants a pool may declare.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=7EF4ED
    // Broiler-Human:        PENDING
    public const uint CeilingConstants = 65_536;

    /// <summary>The most exception regions one artifact may declare.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=4B53EB
    // Broiler-Human:        PENDING
    public const uint CeilingExceptionRegions = 262_144;

    /// <summary>The most entry points one artifact may declare.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=4D3618
    // Broiler-Human:        PENDING
    public const uint CeilingEntries = 256;

    /// <summary>The most position-table rows one artifact may declare.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=20A56C
    // Broiler-Human:        PENDING
    public const uint CeilingPositions = 4_194_304;

    /// <summary>The deepest static scope nesting one code unit may address.</summary>
    /// <remarks>
    /// The depth operand is one byte, so 255 is what the encoding can say. The ceiling is stated
    /// anyway, because a bound that happens to equal a field width is a bound nobody checked.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=C6380C
    // Broiler-Human:        PENDING
    public const uint CeilingScopeDepth = 255;

    /// <summary>The most rows of each of the eval scope map's three tables one artifact may declare.</summary>
    /// <remarks>
    /// A site row names an instruction and a shape row a scope that contains one, so neither can
    /// outnumber the code units' instructions; the bound is the function ceiling because it is the
    /// nearest bound a reader already trusts, and a bound that happens to equal another is stated
    /// anyway.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=AD7A91
    // Broiler-Human:        PENDING
    public const uint CeilingEvalScopeRows = CeilingFunctions;

    /// <summary>The most arguments one call instruction may pass.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=05BEFD
    // Broiler-Human:        PENDING
    public const uint CeilingCallArguments = 255;

    /// <summary>The most bytes one code section may hold.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=817566
    // Broiler-Human:        PENDING
    public const uint CeilingCodeBytes = 67_108_864;

    /// <summary>The most optional surfaces one artifact may declare.</summary>
    /// <remarks>
    /// It is deliberately smaller than the number of names it bounds. An artifact declaring more
    /// surfaces than this build has is declaring something nobody wrote, and a ceiling that
    /// tracked the roster would have to move every time the roster did.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=6A8F76
    // Broiler-Human:        PENDING
    public const uint CeilingSurfaces = 16;

    /// <summary>The most magnitude bytes one BigInt constant may carry: 65,536 bits.</summary>
    /// <remarks>
    /// <b>It bounds what a constant costs to decode and what a literal costs to parse</b>, and it is
    /// the front end's bound as well: a literal wider than this is refused at its source position
    /// rather than written. It is a format ceiling and not the realm's ceiling on a BigInt value,
    /// which arithmetic (JSeal B03) will need to state separately (decision JSD-0033).
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=1BB4FD
    // Broiler-Human:        PENDING
    public const uint CeilingBigIntConstantBytes = 8192;

    /// <summary>The most module records one artifact may declare.</summary>
    /// <remarks>
    /// A module graph is resolved whole at verification, and export resolution walks it, so this
    /// ceiling bounds a walk rather than a table. It is stated separately from the function ceiling
    /// because a module and a code unit are not the same thing: every module has two code units and
    /// most code units are not a module's.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=E9A0DE
    // Broiler-Human:        PENDING
    public const uint CeilingModules = 4_096;

    /// <summary>The most modules one module may request.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=3B7014
    // Broiler-Human:        PENDING
    public const uint CeilingModuleRequests = 4_096;

    /// <summary>The most import entries one artifact may declare.</summary>
    /// <remarks>
    /// <b>The bound is the operand width and is stated anyway.</b> An import read carries a
    /// <c>u16</c> index into the artifact-wide import table, so 65 536 is what the encoding can
    /// say - and a bound that happens to equal a field width is a bound nobody checked.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=B0BF18
    // Broiler-Human:        PENDING
    public const uint CeilingImportEntries = 65_536;

    /// <summary>The most export entries of one kind one module may declare.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=394835
    // Broiler-Human:        PENDING
    public const uint CeilingExportEntries = 65_536;

    /// <summary>The most bytes of emitted machine code one artifact may carry.</summary>
    /// <remarks>
    /// <b>It is the code section's own ceiling and not a larger one, and the equality is a
    /// decision rather than a coincidence.</b> An emitter for the numeric manifest turns one
    /// instruction into a fixed short sequence, so its output is bounded by a small multiple of the
    /// bytecode it was given; a ceiling above the bytecode ceiling would be reserving room for a
    /// payload no such emitter could produce. <b>NO EMITTER EXISTS AT THIS BUILD</b>, so this bound
    /// is what a payload may DECLARE and not a measurement of anything - and it is stated at the
    /// code section's figure so that the first emitter is written against a bound rather than
    /// choosing one.
    /// <i>(Corrected 2026-09-15. "NO EMITTER EXISTS AT THIS BUILD" stopped being true on 2026-09-07,
    /// and the small-multiple reasoning above is the numeric emitter's. The wide manifest's baseline
    /// emitter, decided by JSD-0025, writes a call sequence and a branch tail for every instruction and
    /// a prologue, a dispatch tree and an epilogue for every unit, so its output is a far larger
    /// multiple of the bytecode it is given. The ceiling does not move for it: a program whose emitted
    /// code would pass this bound is refused whole at compile time, with a message naming the ceiling,
    /// and the bound stays what a payload may declare.)</i>
    /// <i>(Corrected 2026-09-17. "A call sequence and a branch tail for every instruction" stopped being
    /// true when the baseline emitter began calling a handler only at the block heads of
    /// <see cref="JsBaselineBlocks"/>' partition. It writes a call sequence and a tail for every block
    /// head, and a block head can be every instruction, so its output can still be a far larger multiple
    /// of the bytecode it is given. The rest of the note above stands, the ceiling included.)</i>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=9A9EA9
    // Broiler-Human:        PENDING
    public const uint CeilingNativeCodeBytes = CeilingCodeBytes;

    /// <summary>The most emitted-code symbols one artifact may declare.</summary>
    /// <remarks>
    /// One per code unit at most, so the bound is the function ceiling. It is stated separately
    /// anyway, because a bound that happens to equal another bound is a bound nobody checked.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=B21D28
    // Broiler-Human:        PENDING
    public const uint CeilingNativeSymbols = CeilingFunctions;

    /// <summary>The coarsest code alignment an emitted code section may declare.</summary>
    /// <remarks>
    /// A page. An alignment larger than the granularity a mapping is made at would be an alignment
    /// nothing could honour, and one that is not a power of two is not an alignment at all - the
    /// verifier refuses both rather than rounding.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=E10CCD
    // Broiler-Human:        PENDING
    public const uint CeilingNativeCodeAlignment = 4_096;

    /// <summary>Encodes a JavaScript String for the constant and name tables.</summary>
    /// <remarks>
    /// <para>
    /// <b>A JavaScript String is a sequence of UTF-16 code UNITS and not of scalar values, and UTF-8
    /// cannot carry one.</b> <c>"\uD800"</c> is a legal String with a legal length and a legal
    /// <c>charCodeAt</c>; it is also an unpaired surrogate, which no UTF-8 sequence encodes. The
    /// platform's encoder answers a replacement character for it, silently, so a literal containing
    /// one reached the artifact as <c>U+FFFD</c> and every later answer about it — its length, its
    /// units, its comparison with another such literal — was about the replacement instead.
    /// </para>
    /// <para>
    /// <b>So a surrogate is written as its own three bytes, which UTF-8 forbids and this format
    /// therefore defines.</b> The encoding is WTF-8: identical to UTF-8 for every well-formed
    /// String, so an artifact that carries no unpaired surrogate has exactly the bytes it had
    /// before, byte for byte and digest for digest, and only a String no UTF-8 encoder could have
    /// carried is written differently.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=B10193
    // Broiler-Human:        PENDING
    public static byte[] EncodeText(string value)
    {
        var buffer = new System.Collections.Generic.List<byte>(value.Length + 8);

        for (var at = 0; at < value.Length; at++)
        {
            var unit = value[at];

            if (char.IsHighSurrogate(unit) && at + 1 < value.Length && char.IsLowSurrogate(value[at + 1]))
            {
                var scalar = char.ConvertToUtf32(unit, value[at + 1]);
                buffer.Add((byte)(0xF0 | (scalar >> 18)));
                buffer.Add((byte)(0x80 | ((scalar >> 12) & 0x3F)));
                buffer.Add((byte)(0x80 | ((scalar >> 6) & 0x3F)));
                buffer.Add((byte)(0x80 | (scalar & 0x3F)));
                at++;
                continue;
            }

            if (unit < 0x80)
            {
                buffer.Add((byte)unit);
                continue;
            }

            if (unit < 0x800)
            {
                buffer.Add((byte)(0xC0 | (unit >> 6)));
                buffer.Add((byte)(0x80 | (unit & 0x3F)));
                continue;
            }

            // THE UNPAIRED SURROGATE TAKES THIS PATH AND SO DOES EVERY ORDINARY THREE-BYTE
            // CHARACTER: the arithmetic is the same, and the only difference is that UTF-8 forbids
            // the result for one of them.
            buffer.Add((byte)(0xE0 | (unit >> 12)));
            buffer.Add((byte)(0x80 | ((unit >> 6) & 0x3F)));
            buffer.Add((byte)(0x80 | (unit & 0x3F)));
        }

        return buffer.ToArray();
    }

    /// <summary>Decodes what <see cref="EncodeText"/> wrote.</summary>
    /// <remarks>
    /// <b>Malformed input answers with replacement characters rather than throwing</b>, exactly as
    /// the platform's decoder does, because this runs on bytes a caller supplied: an artifact is
    /// untrusted input, and a decoder that threw would be a second way to end a verification that
    /// the verifier already ends by diagnosis.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=489A01
    // Broiler-Human:        PENDING
    public static string DecodeText(System.ReadOnlySpan<byte> bytes)
    {
        var built = new System.Text.StringBuilder(bytes.Length);

        for (var at = 0; at < bytes.Length;)
        {
            var lead = bytes[at];

            if (lead < 0x80)
            {
                built.Append((char)lead);
                at++;
                continue;
            }

            var width = lead >= 0xF0 ? 4 : lead >= 0xE0 ? 3 : lead >= 0xC0 ? 2 : 0;

            if (width == 0 || at + width > bytes.Length)
            {
                built.Append('\ufffd');
                at++;
                continue;
            }

            var scalar = lead & (0xFF >> (width + 1));
            var ok = true;

            for (var step = 1; step < width; step++)
            {
                var trail = bytes[at + step];

                if ((trail & 0xC0) != 0x80)
                {
                    ok = false;
                    break;
                }

                scalar = (scalar << 6) | (trail & 0x3F);
            }

            if (!ok)
            {
                built.Append('\ufffd');
                at++;
                continue;
            }

            at += width;

            if (scalar > 0x10FFFF)
            {
                built.Append('\ufffd');
                continue;
            }

            if (scalar > 0xFFFF)
            {
                built.Append(char.ConvertFromUtf32(scalar));
                continue;
            }

            // A SURROGATE ARRIVES AS ITSELF, which is the whole point of the pair: the unit the
            // encoder could not put through UTF-8 comes back as the unit it was.
            built.Append((char)scalar);
        }

        return built.ToString();
    }

    /// <summary>
    /// The first byte of a guest-initiated load that asks for a MODULE rather than for the program
    /// a String is.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>One door and two questions, and this byte is which question was asked.</b> A guest of
    /// this profile obtains code exactly one way — a request whose payload the profile defines and
    /// whose answer the core verifies — and there are two things it may ask for through it: the
    /// program a String is, which is what <c>eval</c> and the <c>Function</c> constructor ask, and
    /// the module a specifier names, which is what a dynamic <c>import()</c> asks when the artifact
    /// it is written in does not already carry that module. Two doors would have been two
    /// capabilities for a composition to register, two places for a mediator to be out of scope,
    /// and two chances to admit one and forget the other.
    /// </para>
    /// <para>
    /// <b>It is a byte no source can begin with, which is what makes the two payloads
    /// distinguishable rather than merely different.</b> U+0000 is not white space, is not part of
    /// an identifier and begins no token, so a program whose first character is one is a program
    /// every front end refuses — and a provider written before this byte existed therefore answers
    /// a module request by REFUSING it, which is a legible failure rather than a misreading.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=550A36
    // Broiler-Human:        PENDING
    public const byte ModuleRequestMark = 0x00;

    /// <summary>
    /// The request payload that asks a provider for the module <paramref name="specifier"/> names
    /// from <paramref name="referrer"/>.
    /// </summary>
    /// <remarks>
    /// <b>The referrer travels with the specifier because a relative specifier means nothing
    /// without one.</b> <c>"./m.mjs"</c> is not the identity of a module; it is the identity of a
    /// module RELATIVE to whatever wrote it, and this profile neither knows nor may guess what that
    /// relation is. So the profile states both halves and the composition answers with the module
    /// its own rules say that pair names.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=0B8924
    // Broiler-Human:        PENDING
    public static byte[] ModuleRequest(string referrer, string specifier)
    {
        var body = EncodeText(referrer + "\0" + specifier);
        var payload = new byte[body.Length + 1];
        payload[0] = ModuleRequestMark;
        System.Array.Copy(body, 0, payload, 1, body.Length);
        return payload;
    }

    /// <summary>Reads what <see cref="ModuleRequest"/> wrote, or answers false.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=BC7BF9
    // Broiler-Human:        PENDING
    public static bool TryReadModuleRequest(
        System.ReadOnlySpan<byte> payload, out string referrer, out string specifier)
    {
        referrer = string.Empty;
        specifier = string.Empty;

        if (payload.Length == 0 || payload[0] != ModuleRequestMark)
        {
            return false;
        }

        var text = DecodeText(payload[1..]);
        var separator = text.IndexOf('\0');

        if (separator < 0)
        {
            return false;
        }

        referrer = text[..separator];
        specifier = text[(separator + 1)..];
        return true;
    }

    /// <summary>
    /// The first byte of a guest-initiated load that asks for the program a String is, compiled as
    /// DIRECT <c>eval</c> code for one call site.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The third question through the same door, marked the way the second one is</b> (JSD-0026
    /// section 5). A direct evaluation needs the source compiled under the eval goal - its free names
    /// resolved through the caller rather than the global object, its lexical declarations kept in
    /// its own record - and under the caller's strictness and syntactic permissions, which the one
    /// <see cref="EvalRequestFlags"/> byte after the mark carries. The source follows, encoded as
    /// <see cref="EncodeText"/> encodes it.
    /// </para>
    /// <para>
    /// <b>U+0001 begins no program</b>, exactly as U+0000 begins none, so a provider written before
    /// this byte existed refuses a marked payload as source it cannot parse rather than compiling it
    /// as something else - and the executor binds every answer to the request besides, refusing an
    /// artifact whose entry is not eval code compiled under the flags it asked for.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=984BF7
    // Broiler-Human:        PENDING
    public const byte EvalRequestMark = 0x01;

    /// <summary>
    /// The request payload that asks a provider for <paramref name="source"/> compiled as direct
    /// <c>eval</c> code under <paramref name="flags"/>.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=6AB473
    // Broiler-Human:        PENDING
    public static byte[] EvalRequest(EvalRequestFlags flags, string source)
    {
        var body = EncodeText(source);
        var payload = new byte[body.Length + 2];
        payload[0] = EvalRequestMark;
        payload[1] = (byte)flags;
        System.Array.Copy(body, 0, payload, 2, body.Length);
        return payload;
    }

    /// <summary>Reads what <see cref="EvalRequest"/> wrote, or answers false.</summary>
    /// <remarks>
    /// A flags byte naming a bit <see cref="EvalRequestFlagBits"/> does not define is not a request
    /// this build wrote, and it is answered false rather than read with the bit dropped: a provider
    /// that compiled it would be compiling under a permission nobody granted.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=F267FC
    // Broiler-Human:        PENDING
    public static bool TryReadEvalRequest(
        System.ReadOnlySpan<byte> payload, out EvalRequestFlags flags, out string source)
    {
        flags = EvalRequestFlags.None;
        source = string.Empty;

        if (payload.Length < 2 || payload[0] != EvalRequestMark ||
            (payload[1] & ~(byte)EvalRequestFlagBits) != 0)
        {
            return false;
        }

        flags = (EvalRequestFlags)payload[1];
        source = DecodeText(payload[2..]);
        return true;
    }

    /// <summary>
    /// Whether a payload begins with a control byte this format reserves for a request mark and
    /// does not define, which a source provider refuses as a malformed encoding.
    /// </summary>
    /// <remarks>
    /// <b>No program begins with any of these characters</b>: U+0000 to U+0008 and U+000E to U+001F
    /// are neither white space nor a line terminator and begin no token. So a payload that begins
    /// with one is either a request vocabulary a later build defines or a source every front end
    /// refuses, and a provider that compiled it as source would be answering a question it cannot
    /// have understood. The three marks this build defines are not reserved: they are read.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=53A2BE
    // Broiler-Human:        PENDING
    public static bool StartsWithReservedMark(System.ReadOnlySpan<byte> payload) =>
        payload.Length != 0 &&
        payload[0] != ModuleRequestMark &&
        payload[0] != EvalRequestMark &&
        payload[0] != ScriptRequestMark &&
        (payload[0] <= 0x08 || payload[0] is >= 0x0E and <= 0x1F);

    /// <summary>
    /// The first byte of a load that asks for a String compiled as a SCRIPT an embedder is running
    /// in an existing realm (JSeal V15-host, JSD-0024 section 15).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A fourth question through the same door, and the only one no guest can ask.</b> The
    /// profile writes this mark from exactly one place, <c>JsHostRealm.EvaluateScript</c>, which an
    /// embedder calls; a guest's <c>eval</c> sends <see cref="EvalRequestMark"/>, a dynamic
    /// <c>import()</c> sends <see cref="ModuleRequestMark"/>, the <c>Function</c> constructor sends
    /// source that begins with <c>(</c>, and a guest String that begins with a control byte is
    /// answered as a <c>SyntaxError</c> before anything is sent. So a provider may treat the mark as
    /// the embedder's authorisation - which is what lets it answer a host script under a policy that
    /// refuses guest evaluation.
    /// </para>
    /// <para>
    /// <b>The flags byte and the source name travel with the source</b>, because a provider compiles
    /// the script and only it can attribute a refusal's position to a name: the name decides nothing
    /// about the bytes. A provider that does not speak this mark refuses it as a reserved one, which
    /// the embedder is told as the <c>SyntaxError</c> every refusal becomes.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=High; Resources=1; Fingerprint=F9CFC2
    // Broiler-Falsified-If: a guest-initiated load can send a payload that begins with this byte
    // Broiler-Human:        PENDING
    public const byte ScriptRequestMark = 0x02;

    /// <summary>What an embedder asks of the script it hands a provider.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=147F0A
    // Broiler-Human:        PENDING
    [System.Flags]
    public enum ScriptRequestFlags : byte
    {
        /// <summary>The script is strict only if its own directive prologue says so.</summary>
        None = 0,

        /// <summary>The script is strict code whatever its directive prologue says.</summary>
        Strict = 1,
    }

    /// <summary>Every bit <see cref="ScriptRequestFlags"/> defines.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=1BFB36
    // Broiler-Human:        PENDING
    public const ScriptRequestFlags ScriptRequestFlagBits = ScriptRequestFlags.Strict;

    /// <summary>
    /// The request payload that asks a provider for <paramref name="source"/> compiled as a script
    /// named <paramref name="sourceName"/> under <paramref name="flags"/>.
    /// </summary>
    /// <remarks>
    /// The mark, the flags byte, then the name and the source as <see cref="EncodeText"/> encodes
    /// them, separated by one NUL - so a name may not contain one, and this refuses it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=2F3E86
    // Broiler-Human:        PENDING
    public static byte[] ScriptRequest(ScriptRequestFlags flags, string sourceName, string source)
    {
        if (sourceName.Contains('\0'))
        {
            throw new System.ArgumentException("a source name may not contain U+0000", nameof(sourceName));
        }

        var body = EncodeText(sourceName + "\0" + source);
        var payload = new byte[body.Length + 2];
        payload[0] = ScriptRequestMark;
        payload[1] = (byte)flags;
        System.Array.Copy(body, 0, payload, 2, body.Length);
        return payload;
    }

    /// <summary>Reads what <see cref="ScriptRequest"/> wrote, or answers false.</summary>
    /// <remarks>
    /// A flags byte naming a bit <see cref="ScriptRequestFlagBits"/> does not define, or a body with
    /// no separator, is not a request this build wrote and is answered false.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=1; Fingerprint=40BE27
    // Broiler-Human:        PENDING
    public static bool TryReadScriptRequest(
        System.ReadOnlySpan<byte> payload,
        out ScriptRequestFlags flags,
        out string sourceName,
        out string source)
    {
        flags = ScriptRequestFlags.None;
        sourceName = string.Empty;
        source = string.Empty;

        if (payload.Length < 2 || payload[0] != ScriptRequestMark ||
            (payload[1] & ~(byte)ScriptRequestFlagBits) != 0)
        {
            return false;
        }

        var text = DecodeText(payload[2..]);
        var separator = text.IndexOf('\0');

        if (separator < 0)
        {
            return false;
        }

        flags = (ScriptRequestFlags)payload[1];
        sourceName = text[..separator];
        source = text[(separator + 1)..];
        return true;
    }
}
