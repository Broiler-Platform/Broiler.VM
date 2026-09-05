// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   28
// Annotated:        28/28
// Exempt:           31
// Human-reviewed:   0/28
// IP risk:          None
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  1/10 max
// Unverified:       28
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=913500
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
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=5140F2
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

    /// <summary>The most optional surfaces one artifact may declare.</summary>
    /// <remarks>
    /// It is deliberately smaller than the number of names it bounds. An artifact declaring more
    /// surfaces than this build has is declaring something nobody wrote, and a ceiling that
    /// tracked the roster would have to move every time the roster did.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=None; Security=Medium; Resources=0; Fingerprint=6A8F76
    // Broiler-Human:        PENDING
    public const uint CeilingSurfaces = 16;

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
}
