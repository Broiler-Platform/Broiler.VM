// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   17
// Annotated:        17/17
// Exempt:           22
// Human-reviewed:   0/17
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  0/10 max
// Unverified:       17
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=8895FC
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
    }

    /// <summary>The constant-pool entry tags version 2 reads.</summary>
    /// <remarks>
    /// Tags 1 to 4 are version 1's, with the same payloads. <see cref="String"/> is new, and
    /// <see cref="JavaScriptFormat.ConstantTag.InternedName"/> - reserved by version 1 and admitted
    /// by no manifest there - is admitted here. The two are distinct because a property name is
    /// interned once per program and compared by reference, and a String value is a value.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=146A71
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
    }

    /// <summary>The flag bits a code-unit row carries.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=2E646A
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
        BindsParameters = 32,
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

    /// <summary>The most arguments one call instruction may pass.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=05BEFD
    // Broiler-Human:        PENDING
    public const uint CeilingCallArguments = 255;

    /// <summary>The most bytes one code section may hold.</summary>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=817566
    // Broiler-Human:        PENDING
    public const uint CeilingCodeBytes = 67_108_864;
}
