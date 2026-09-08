// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           0
// Human-reviewed:   0/5
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript.Format;

/// <summary>
/// The <c>broiler.javascript.numeric</c> feature manifest: the numeric subset of the language a
/// whole artifact can be compiled from, and the closed set of instructions a lowering under it may
/// write.
/// </summary>
/// <remarks>
/// <para>
/// <b>It is a third manifest beside <c>slice</c> and <c>wide</c>, and it exists so that a form
/// other than bytecode can be a WHOLE-ARTIFACT form.</b> This profile's non-goals pin the rule the
/// manifest is shaped by: the output form is chosen when an artifact is compiled and fixed when it
/// is verified, one executor and one form per handle, no promotion. A form that compiled the
/// functions it liked and interpreted the rest would be choosing per unit what the rule says is
/// chosen per handle, so the only admissible answer is a language small enough that EVERY unit of
/// an artifact is compilable. This is that language.
/// </para>
/// <para>
/// <b>What it admits, exhaustively.</b> Number values; the arithmetic, comparison, bitwise and
/// unary operators; assignment to a binding, simple and compound, and <c>++</c> and <c>--</c> on
/// a binding; <c>let</c>, <c>const</c> and <c>var</c> bindings of numbers, each with an
/// initialiser; <c>if</c>, <c>while</c>, <c>for</c>, <c>do</c>, blocks, the empty statement, and
/// unlabelled <c>break</c> and <c>continue</c>; function declarations at the program's top level
/// whose parameters are plain names and whose bodies terminate in a <c>return</c> of a value;
/// direct calls to those functions by name; and <c>return</c>.
/// <i>(Corrected 2026-09-08. This enumeration named itself exhaustive while omitting three
/// constructs the admission pass admits - assignment to a binding, the update operators on a
/// binding, and the empty statement - and its own vocabulary did not reach them, since the unary
/// operators this manifest admits are exactly <c>+</c>, <c>-</c>, <c>!</c> and <c>~</c>. A list
/// headed "exhaustively" that understates the surface is the same defect as one that overstates
/// it, which is why the omission is recorded here rather than repaired silently.)</i>
/// </para>
/// <para>
/// <b>What it admits nothing of.</b> Objects, arrays, strings, templates, regular expressions,
/// <c>null</c>, Boolean literals, classes, <c>this</c>, <c>super</c>, <c>new</c>, property access
/// of any spelling, function expressions, arrow functions, nested function declarations - so no
/// function admitted here closes over another function's bindings - generators,
/// <c>async</c>, <c>await</c>, <c>yield</c>, exceptions, <c>switch</c>, labels, <c>with</c>,
/// <c>for-in</c>, <c>for-of</c>, destructuring, spread, default and rest parameters, modules,
/// <c>eval</c>, and every other dynamic construct. Each is refused BY NAME by the front end, at
/// compile time, which roadmap section 6 makes a first-class answer rather than a fallback.
/// </para>
/// <para>
/// <b>THE COST, STATED RATHER THAN HIDDEN: the admitted language is small and it is not
/// JavaScript.</b> An artifact under this manifest runs numeric kernels and nothing else. The wide
/// manifest stays the one that carries real programs, and the interpreter stays the only thing that
/// runs them. Nothing here narrows what the wide manifest admits and nothing here is a step towards
/// compiling it.
/// </para>
/// <para>
/// <b>UNDEFINED IS IN THE ADMITTED SET AND IT IS THE ONE VALUE HERE THAT IS NOT A NUMBER.</b> The
/// lowering gives a program body a completion slot and initialises it to <c>undefined</c> before
/// the first statement runs, so <see cref="JsOpcode.LoadUndefined"/> is written by every program of
/// this manifest whatever its text says. The manifest admits no way for a source to produce another
/// one - a binding with no initialiser is refused, and every function body must terminate in a
/// <c>return</c> of a value - but a reader who was told "Number values only" and then found this
/// instruction would be right to call that untrue, so it is said here instead.
/// </para>
/// <para>
/// <b>A TOP-LEVEL BINDING OF A SCRIPT IS THE REALM'S AND NOT THE UNIT'S, AND THAT IS WHY THE GLOBAL
/// INSTRUCTIONS ARE ADMITTED.</b> The language says a script's top-level <c>var</c> is a property
/// of the global object and its top-level <c>let</c> and <c>const</c> are bindings of the realm's
/// global lexical environment, so the wide lowering - which this manifest reuses whole rather than
/// forking - writes the global instructions for exactly the declarations this manifest admits.
/// Refusing them would be refusing <c>let x = 1;</c> at the top level of a script. The consequence
/// for a machine-code backend is real and is not answered here: a unit that reaches one of those
/// instructions reaches the realm, which is an object graph, so it is not a unit whose frame is
/// doubles alone. See <see cref="JsNativeFrame"/>, which says what a frame carrying no managed
/// reference can hold.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=DA1927
// Broiler-Human:        PENDING
public static class JsNumericManifest
{
    /// <summary>The feature-manifest identity an artifact of this manifest names in its header.</summary>
    /// <remarks>
    /// It is a header manifest and not an optional surface, because a surface is something an
    /// artifact declares BESIDE its manifest to reach more than the manifest gives it, and this one
    /// gives strictly less. A narrowing cannot be spelled as an addition.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=7711B5
    // Broiler-Human:        PENDING
    public const string ManifestId = "broiler.javascript.numeric";

    /// <summary>
    /// Every instruction a lowering under this manifest may write.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>It is a closed set and the closure is what a backend is entitled to rely on.</b> An
    /// emitter for this manifest has to answer for every instruction it can meet; a set that grew
    /// whenever the wide lowering learned a new trick would be a set no emitter could be complete
    /// against, and an emitter meeting an instruction it had no template for would have to choose
    /// between emitting nothing and emitting something wrong.
    /// </para>
    /// <para>
    /// <b>The set is held here rather than in the lowering because the lowering and any executor of
    /// an emitted form may not reference each other.</b> The format assembly is the one place both
    /// can reach, which is the same reason <see cref="JsSurfaces"/> lives here.
    /// </para>
    /// <para>
    /// <b><see cref="JsOpcode.Nop"/> is deliberately absent.</b> The lowering writes one only to
    /// give an empty protected range a byte to cover, and this manifest admits no exception region
    /// at all, so admitting it would be admitting an instruction no source of this manifest can
    /// cause to be written.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=87908C
    // Broiler-Human:        PENDING
    public static readonly JsOpcode[] AdmittedOpcodes =
    [
        JsOpcode.LoadUndefined,
        JsOpcode.LoadConstant,
        JsOpcode.LoadScoped,
        JsOpcode.StoreScoped,
        JsOpcode.InitialiseScoped,
        JsOpcode.LoadGlobal,
        JsOpcode.StoreGlobal,
        JsOpcode.PushScope,
        JsOpcode.PopScope,
        JsOpcode.CopyScope,
        JsOpcode.DeclareGlobal,
        JsOpcode.Closure,
        JsOpcode.Call,
        JsOpcode.Return,
        JsOpcode.ReturnUndefined,
        JsOpcode.Add,
        JsOpcode.Subtract,
        JsOpcode.Multiply,
        JsOpcode.Divide,
        JsOpcode.Remainder,
        JsOpcode.Exponent,
        JsOpcode.Negate,
        JsOpcode.ToNumber,
        JsOpcode.Not,
        JsOpcode.BitwiseNot,
        JsOpcode.LessThan,
        JsOpcode.LessThanOrEqual,
        JsOpcode.GreaterThan,
        JsOpcode.GreaterThanOrEqual,
        JsOpcode.StrictEquals,
        JsOpcode.StrictNotEquals,
        JsOpcode.LooseEquals,
        JsOpcode.LooseNotEquals,
        JsOpcode.BitwiseOr,
        JsOpcode.BitwiseAnd,
        JsOpcode.BitwiseXor,
        JsOpcode.ShiftLeft,
        JsOpcode.ShiftRight,
        JsOpcode.ShiftRightUnsigned,
        JsOpcode.Jump,
        JsOpcode.JumpIfFalse,
        JsOpcode.JumpIfTrue,
        JsOpcode.Pop,
        JsOpcode.Duplicate,
        JsOpcode.DeclareGlobalLet,
        JsOpcode.DeclareGlobalConst,
        JsOpcode.InitialiseGlobalLexical,
    ];

    /// <summary>Whether <paramref name="opcode"/> is one this manifest admits.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=A056D8
    // Broiler-Human:        PENDING
    public static bool Admits(JsOpcode opcode)
    {
        foreach (var admitted in AdmittedOpcodes)
        {
            if (admitted == opcode)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Whether a code unit carrying <paramref name="flags"/> is one this manifest admits.</summary>
    /// <remarks>
    /// <b>The five refused bits are the five that send an invocation somewhere other than straight
    /// into the unit's own code.</b> A generator or an async body is entered by a driver holding a
    /// heap frame; a class constructor and a derived constructor are entered with a <c>this</c> a
    /// construction supplied; a unit that binds its own parameters runs a prologue this manifest
    /// admits no syntax for. Nothing this manifest admits produces any of them, so a unit carrying
    /// one did not come from this front end.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=BBD2D4
    // Broiler-Human:        PENDING
    public static bool AdmitsFlags(JsFormat.FunctionFlags flags) =>
        (flags & (JsFormat.FunctionFlags.Generator |
            JsFormat.FunctionFlags.Async |
            JsFormat.FunctionFlags.ClassConstructor |
            JsFormat.FunctionFlags.DerivedConstructor |
            JsFormat.FunctionFlags.BindsParameters)) == 0;
}
