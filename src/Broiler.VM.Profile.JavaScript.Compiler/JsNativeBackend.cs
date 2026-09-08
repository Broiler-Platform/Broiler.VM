// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           7
// Human-reviewed:   0/12
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  2/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript.Compiler;

/// <summary>
/// The finished bytecode of one artifact, as a backend receives it: the seam a second output form
/// attaches at.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE SEAM IS THE FINISHED BYTECODE AND NOT THE SYNTAX TREE, AND THAT IS WHAT MAKES A SECOND
/// BACKEND A SECOND EXIT FROM ONE FRONT END RATHER THAN A SECOND FRONT END.</b> The core roadmap
/// states the rule as "a bytecode backend and an x86 backend are two exits from one front end", and
/// the tree is the wrong place to take it: lowering this profile's syntax is a hundred and twenty
/// mutually recursive methods carrying eight pieces of ambient state, and a second walk would
/// duplicate every hoisting rule, every completion-value rule and every scope rule - which is the
/// fork the roadmap forbids by name.
/// </para>
/// <para>
/// <b>The bytecode already carries everything a machine-code emitter needs, which is why it can be
/// the seam at all.</b> Per-unit code ranges, a declared operand-stack maximum, a declared slot
/// count per unit, explicit scope depth, exception regions carrying their entry height and depth,
/// and branch targets that are absolute offsets rather than displacements. There is no analysis
/// here a backend has to redo and no fact it has to infer.
/// </para>
/// <para>
/// <b>The constants arrive ENCODED rather than decoded, and that is deliberate.</b> A pool entry is
/// a tag and a payload; handing a backend an array of <c>double</c> would mean deciding here what a
/// non-Number entry becomes, and every answer to that is a lie about a pool that has one. A backend
/// for the numeric manifest decodes the tags it admits and refuses the rest, which is a refusal it
/// can state rather than a substitution it cannot see.
/// </para>
/// </remarks>
/// <param name="ManifestId">The feature manifest the artifact will name in its header.</param>
/// <param name="Code">The whole code section: every unit's bytes, back to back.</param>
/// <param name="Functions">One row per code unit, tiling <paramref name="Code"/> in order.</param>
/// <param name="ExceptionRegions">The artifact's exception regions, empty when it has none.</param>
/// <param name="Constants">The constant pool, one encoded entry each, in pool order.</param>
/// <param name="MaximumOperandStack">The deepest operand stack any unit declares.</param>
/// <param name="MaximumScopeSlots">The most slots any unit's own environment declares.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=847CEC
// Broiler-Human:        PENDING
public sealed record JsAssembledProgram(
    string ManifestId,
    byte[] Code,
    System.Collections.Generic.IReadOnlyList<JsFunctionRow> Functions,
    System.Collections.Generic.IReadOnlyList<JsExceptionRegionRow> ExceptionRegions,
    System.Collections.Generic.IReadOnlyList<byte[]> Constants,
    uint MaximumOperandStack,
    uint MaximumScopeSlots);

/// <summary>What a backend produced: the emitted bytes and the table that says which unit is where.</summary>
/// <param name="Architecture">The instruction set and calling convention the bytes were written for.</param>
/// <param name="BackendSemanticVersion">
/// The emitting backend's version. It travels in the artifact so that an image can refuse a payload
/// emitted by a backend it is not the same as, rather than running bytes written against a contract
/// it has since changed.
/// </param>
/// <param name="CodeAlignment">The alignment every unit's entry point is written at.</param>
/// <param name="Code">The emitted bytes, every unit's back to back.</param>
/// <param name="Symbols">One row per code unit, in code-unit order, tiling <paramref name="Code"/>.</param>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=405BB0
// Broiler-Human:        PENDING
public sealed record JsNativeEmission(
    JsNativeArchitecture Architecture,
    uint BackendSemanticVersion,
    uint CodeAlignment,
    byte[] Code,
    JsNativeSymbolRow[] Symbols);

/// <summary>An emitter of machine code for one architecture and calling convention.</summary>
/// <remarks>
/// <para>
/// <b>A backend emits an ARTIFACT and not a unit, and the signature says so.</b> This profile's
/// non-goals fix an artifact's output form when it is compiled and pin it when it is verified: one
/// executor, one form per handle, no promotion. A backend that could answer "this unit yes, that
/// unit no" would be the per-unit choice that rule refuses, so the only two answers this method has
/// are the whole artifact and a refusal that names the reason.
/// </para>
/// <para>
/// <b>THE REFUSAL IS A STRING AND NOT A DIAGNOSTIC CODE, because it never crosses a core result
/// envelope.</b> A backend refusing to emit is a compilation that produced no artifact, which is
/// the embedder seam this component already answers with a source diagnostic; the caller turns this
/// sentence into one.
/// </para>
/// <para>
/// <b>Emission must be deterministic, and it is the implementation's obligation rather than this
/// interface's.</b> The determinism rule the roadmap states over bytecode - one source, one
/// lowering version, one format version, one byte-identical artifact - extends over machine code,
/// and it is what makes re-emission-equality verification possible at all: recompile the carried
/// bytecode and compare. An emitter whose register allocation or instruction scheduling depended on
/// a hash order would break that, and nothing in a type signature can catch it.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=AB414A
// Broiler-Human:        PENDING
public interface IJsNativeBackend
{
    /// <summary>The name a caller selects this backend by.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=E9048D
    // Broiler-Human:        PENDING
    string Name { get; }

    /// <summary>The instruction set and calling convention this backend writes for.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=BFE8C5
    // Broiler-Human:        PENDING
    JsNativeArchitecture Architecture { get; }

    /// <summary>This backend's version, which travels in the artifact it emits.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=A5ABAA
    // Broiler-Human:        PENDING
    uint SemanticVersion { get; }

    /// <summary>Emits the whole program, or refuses it and says why.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=A5E5E7
    // Broiler-Human:        PENDING
    bool TryEmit(JsAssembledProgram program, out JsNativeEmission emission, out string refusal);
}

/// <summary>
/// A backend that refuses every artifact, one per architecture this format can name.
/// </summary>
/// <remarks>
/// <para>
/// <b>IT EMITS NOTHING, AND SAYING SO IS ITS ENTIRE PURPOSE.</b> <i>(Revised 2026-09-07: this
/// paragraph continued "No instruction of any architecture is encoded anywhere in this component",
/// which was true when it was written and is now false - <see cref="JsX64Backend"/> and
/// <see cref="JsArm64Backend"/> both encode. The sentence is corrected rather than removed, because
/// the reason this type exists survives its own obsolescence.)</i> This type exists so that the path
/// a real emitter sits on - a caller choosing a form, a caller choosing a backend by name, an
/// unknown name being refused, a backend's refusal becoming a compilation diagnostic - was exercised
/// end to end before the first byte was encoded, and so that the shape a backend is asked for was
/// decided in the open rather than by whichever emitter happened to be written first. <b>No name in
/// the roster resolves to it today</b>, and it is kept rather than deleted because it is the only
/// answer a fourth architecture is entitled to give on the day its name is minted and before its
/// instruction table exists.
/// </para>
/// <para>
/// <b>A stub that answered with plausible bytes would be worse than nothing.</b> The one defect
/// class a verifier of machine code cannot catch is a well-framed sequence of wrong instructions,
/// so a placeholder emitter would be manufacturing exactly the input nothing downstream can
/// detect. This one refuses, which is the only answer a backend with no instruction table is
/// entitled to give.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=D7BAE1
// Broiler-Human:        PENDING
public sealed class JsUnwrittenNativeBackend : IJsNativeBackend
{
    /// <summary>Names a backend that will emit for <paramref name="architecture"/> and does not.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=888666
    // Broiler-Human:        PENDING
    public JsUnwrittenNativeBackend(string name, JsNativeArchitecture architecture)
    {
        Name = name;
        Architecture = architecture;
    }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=782A99
    // Broiler-Human:        PENDING
    public string Name { get; }

    /// <inheritdoc/>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=4992FA
    // Broiler-Human:        PENDING
    public JsNativeArchitecture Architecture { get; }

    /// <summary>Zero: the version of a backend that has emitted nothing.</summary>
    /// <remarks>
    /// <b>It is zero rather than one so that no artifact this component can produce carries a
    /// version an emitting backend might later claim.</b> A first real emitter takes version one,
    /// and nothing that ever ran will have said it was that.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=90179C
    // Broiler-Human:        PENDING
    public uint SemanticVersion => 0;

    /// <summary>Refuses, naming the architecture nobody has written an encoder for.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=40AF94
    // Broiler-Human:        PENDING
    public bool TryEmit(JsAssembledProgram program, out JsNativeEmission emission, out string refusal)
    {
        emission = null!;

        refusal =
            "the `" + Name + "` backend emits no machine code: this build encodes no instruction " +
            "of " + Architecture + ", and a backend with no instruction table can answer only by " +
            "refusing";

        return false;
    }
}

/// <summary>The backends this build carries, and the name each is selected by.</summary>
/// <remarks>
/// <para>
/// <b>A NAME AND NOT A PROBE OF THE RUNNING MACHINE.</b> Which backend compiles an artifact is a
/// property of the artifact being compiled and not of the machine compiling it, so it is chosen by
/// a caller that says which one. A selector that read
/// <c>RuntimeInformation.ProcessArchitecture</c> would make one source compile to two different
/// artifacts on two machines, which is the determinism rule broken by the one mechanism nobody
/// would think to test.
/// </para>
/// <para>
/// <b>THE ROSTER NAMES THREE AND ALL THREE EMIT; WHAT DIFFERS IS WHETHER ANYTHING RUNS THE
/// RESULT.</b> <i>(Revised 2026-09-07: this paragraph read "THE ROSTER NAMES THREE AND THE THREE DO
/// NOT ALL EMIT", which was true while every name resolved to
/// <see cref="JsUnwrittenNativeBackend"/> and stopped being true when the two encoders landed. It is
/// corrected rather than deleted, because a roster describing itself as more aspirational than it is
/// misleads in the safe direction and a reader should be able to see which way it moved.)</i> The
/// names are the three architecture and convention pairs this format can carry. Which resolves to
/// which encoder is decided in <see cref="TryFind"/> and nowhere else, so a reader who wants the
/// answer reads one switch rather than a paragraph that can go stale.
/// </para>
/// <para>
/// <b>The distinction the roster cannot express, and which therefore has to be read here.</b> Both
/// x86-64 names emit code that this component also <i>arms and executes</i>; the arm64 name emits
/// code that nothing here executes, and the support table calls that backend <b>emitting-only</b>.
/// The difference is not a gap in the encoder - it is that an arm64 page needs an instruction-cache
/// maintenance sequence with no managed expression, so bytes are all this component can honestly
/// produce for it. A golden byte is a claim about what the encoder wrote; it is not a claim about
/// what a processor would do, and no figure and no capability claim attaches to arm64 on its
/// strength.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=A158DC
// Broiler-Human:        PENDING
public static class JsNativeBackends
{
    /// <summary>x86-64 under the System V AMD64 convention.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=4565D4
    // Broiler-Human:        PENDING
    public const string X64SystemV = "x86-64-sysv";

    /// <summary>x86-64 under the Windows x64 convention.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=CA0F1D
    // Broiler-Human:        PENDING
    public const string X64Windows = "x86-64-win64";

    /// <summary>arm64 under AAPCS64.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=E23D03
    // Broiler-Human:        PENDING
    public const string Arm64 = "arm64-aapcs64";

    /// <summary>Every backend name this build knows, in ascending ordinal order.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=A9C76A
    // Broiler-Human:        PENDING
    public static readonly string[] Names = [Arm64, X64SystemV, X64Windows];

    /// <summary>Finds the backend named <paramref name="name"/>.</summary>
    /// <remarks>
    /// <b>A fresh instance per lookup, because rule N12 forbids this assembly state that outlives a
    /// call and a cached roster is exactly that.</b> A backend holds nothing between calls, so
    /// constructing one is free and caching it would buy nothing but a static field the rule
    /// exists to keep out.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=2; Fingerprint=A5B6F3
    // Broiler-Human:        PENDING
    public static bool TryFind(string name, out IJsNativeBackend backend)
    {
        switch (name)
        {
            case X64SystemV:
                backend = new JsX64Backend(JsX64Abi.SystemV);
                return true;

            case X64Windows:
                backend = new JsX64Backend(JsX64Abi.Windows);
                return true;

            case Arm64:
                // EMITTING-ONLY, AND THE ROSTER IS WHERE THAT WORD FIRST BITES. This resolves to a
                // real encoder: `arm64-aapcs64` produces machine code, and the golden rows in the
                // slice-compiler root pin every word of it against a hand-derived encoding. What it
                // does NOT produce is a run - nothing in this component arms an arm64 page, because
                // the instruction-cache maintenance sequence a written page needs before it is
                // executed has no managed expression and no dependable library export. So a caller
                // that asks for this backend gets bytes it may compare and may not execute, and the
                // support table names the backend `emitting-only` for exactly that reason.
                backend = new JsArm64Backend();
                return true;

            default:
                backend = null!;
                return false;
        }
    }
}
