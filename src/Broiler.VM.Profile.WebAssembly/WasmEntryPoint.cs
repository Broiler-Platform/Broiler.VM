// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   8
// Annotated:        8/8
// Exempt:           0
// Human-reviewed:   0/8
// IP risk:          Low
// Security risk:    High
// Criteria:         5/5
// Resource impact:  4/10 max
// Unverified:       8
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.WebAssembly;

/// <summary>
/// The entry-point encoding: how an export name and its arguments travel through the one UTF-8
/// field an invocation request carries.
/// </summary>
/// <remarks>
/// <para>
/// <b>THE CONTRACT HAS NO ARGUMENT CHANNEL, AND THIS IS THE ANSWER TAKEN INSTEAD OF WAITING FOR
/// ONE.</b> An invocation request carries one UTF-8 entry-point text and nothing else, and the core
/// carries those bytes verbatim - it does not decode them, re-encode them or trim them. So the
/// arguments are encoded into that text. <i>The alternatives not taken</i> are a typed argument
/// vector on the request, which is a core contract amendment nobody has minted, and a host
/// capability the module calls to fetch its own arguments, which needs the module to cooperate and
/// the specification's own test modules never will.
/// </para>
/// <para>
/// <b>THE GRAMMAR IS LENGTH-PREFIXED, AND THAT IS THE WHOLE REASON IT IS SAFE.</b> A WebAssembly
/// export name is arbitrary well-formed UTF-8: it may contain a colon, a digit, a bracket, or the
/// exact text of an argument group. A separator-delimited grammar would be ambiguous over that name
/// space and would fail on an export named <c>f(i32)</c> or <c>a:3:i32</c> - both of which are legal
/// names and both of which have a corpus entry beside this code. Counting bytes sidesteps the
/// question entirely.
/// </para>
/// <para>
/// The text is <c>&lt;nameByteCount&gt;:&lt;name bytes&gt;</c> followed by zero or more groups
/// <c>&lt;type&gt;:&lt;literalByteCount&gt;:&lt;literal bytes&gt;</c>, with no separator between the
/// name and the first group and none between groups: after a byte count has been consumed, exactly
/// that many bytes follow and the next thing is a type token or the end of the text. The counts are
/// ASCII decimal. The types are <c>i32</c>, <c>i64</c>, <c>f32</c> and <c>f64</c>.
/// </para>
/// <para>
/// <b>FLOATS ARE HEXADECIMAL BIT PATTERNS AND NEVER DECIMAL TEXT.</b> Eight hex digits for
/// <c>f32</c> and sixteen for <c>f64</c>, written exactly. A decimal spelling would round-trip
/// nearly all of the time, would lose the distinction between one NaN payload and another entirely,
/// and would make the encoding's correctness a property of two independent formatters. An integer
/// literal is ASCII decimal, optionally signed, or <c>0x</c> followed by hex digits when a caller
/// would rather write the bit pattern.
/// </para>
/// <para>
/// So <c>3:add</c> invokes the export named <c>add</c> with no arguments;
/// <c>3:addi32:1:7i32:2:35</c> invokes it with 7 and 35; <c>6:f(i32)i32:1:1</c> invokes an export
/// whose own name contains brackets and a type name; and <c>7:a:3:i32</c> invokes an export whose
/// name is <c>a:3:i32</c> - the encoding's own separators and a type token, and it resolves to the
/// name and no arguments because the count said seven bytes.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=A39A6F
// Broiler-Falsified-If: two different (name, argument) pairs encode to the same text, or a float literal does not round-trip its exact bit pattern
// Broiler-Human:        PENDING
internal static class WasmEntryPoint
{
    /// <summary>
    /// The most arguments one entry point may carry, which bounds the caller's scratch arrays.
    /// </summary>
    /// <remarks>
    /// It is this profile's own number rather than the format's: a function may declare more
    /// parameters than this, and an entry point naming such a function is refused rather than
    /// truncated. Sixty-four is the point past which an argument vector encoded into a name has
    /// stopped being a workaround and started being a format.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=1; Fingerprint=66816C
    // Broiler-Human:        PENDING
    internal const int MaximumArguments = 64;

    /// <summary>
    /// Parses one entry-point text into a name window and a typed argument vector.
    /// </summary>
    /// <remarks>
    /// It reads bytes and answers; it allocates nothing and it throws on nothing. The name is
    /// reported as an offset and a length into the caller's own span rather than copied, because the
    /// caller compares it against export names it already holds.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=4; Fingerprint=B56F30
    // Broiler-Falsified-If: a byte count is trusted past the end of the text, or a malformed literal is read as a value
    // Broiler-Human:        PENDING
    internal static bool TryParse(
        System.ReadOnlySpan<byte> text,
        System.Span<WasmValue> arguments,
        System.Span<WasmValueType> argumentTypes,
        out int nameOffset,
        out int nameLength,
        out int argumentCount,
        out WebAssemblyEntryPointProblem problem)
    {
        nameOffset = 0;
        nameLength = 0;
        argumentCount = 0;
        problem = WebAssemblyEntryPointProblem.None;

        if (text.Length == 0)
        {
            problem = WebAssemblyEntryPointProblem.Empty;
            return false;
        }

        var at = 0;

        if (!TryReadCount(text, ref at, out var declaredNameLength))
        {
            problem = WebAssemblyEntryPointProblem.MalformedLengthPrefix;
            return false;
        }

        if (declaredNameLength > (uint)(text.Length - at))
        {
            problem = WebAssemblyEntryPointProblem.NameTruncated;
            return false;
        }

        nameOffset = at;
        nameLength = (int)declaredNameLength;
        at += nameLength;

        while (at < text.Length)
        {
            if (argumentCount == MaximumArguments || argumentCount == arguments.Length)
            {
                problem = WebAssemblyEntryPointProblem.TooManyArguments;
                return false;
            }

            if (!TryReadType(text, ref at, out var type))
            {
                problem = WebAssemblyEntryPointProblem.UnknownArgumentType;
                return false;
            }

            if (!TryReadCount(text, ref at, out var literalLength) ||
                literalLength > (uint)(text.Length - at))
            {
                problem = WebAssemblyEntryPointProblem.MalformedLengthPrefix;
                return false;
            }

            var literal = text.Slice(at, (int)literalLength);
            at += (int)literalLength;

            if (!TryReadLiteral(type, literal, out var value))
            {
                problem = WebAssemblyEntryPointProblem.MalformedLiteral;
                return false;
            }

            argumentTypes[argumentCount] = type;
            arguments[argumentCount] = value;
            argumentCount++;
        }

        return true;
    }

    /// <summary>Reads an ASCII decimal byte count terminated by a colon.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=2; Fingerprint=7C643C
    // Broiler-Falsified-If: a count is accumulated past what a 32-bit length can hold
    // Broiler-Human:        PENDING
    private static bool TryReadCount(System.ReadOnlySpan<byte> text, ref int at, out uint count)
    {
        count = 0;
        var digits = 0;

        while (at < text.Length && text[at] is >= (byte)'0' and <= (byte)'9')
        {
            // A count is a length into a text a caller supplied, so it cannot legitimately exceed
            // that text's own length; refusing at that point keeps the accumulation from wrapping.
            count = (count * 10) + (uint)(text[at] - (byte)'0');

            if (count > (uint)text.Length)
            {
                return false;
            }

            digits++;
            at++;
        }

        if (digits == 0 || at >= text.Length || text[at] != (byte)':')
        {
            return false;
        }

        at++;
        return true;
    }

    /// <summary>Reads one of the four type tokens, terminated by a colon.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=65140E
    // Broiler-Human:        PENDING
    private static bool TryReadType(
        System.ReadOnlySpan<byte> text, ref int at, out WasmValueType type)
    {
        type = WasmValueType.I32;

        if (text.Length - at < 4 || text[at + 3] != (byte)':')
        {
            return false;
        }

        var token = text.Slice(at, 3);
        at += 4;

        if (System.MemoryExtensions.SequenceEqual(token, "i32"u8))
        {
            type = WasmValueType.I32;
            return true;
        }

        if (System.MemoryExtensions.SequenceEqual(token, "i64"u8))
        {
            type = WasmValueType.I64;
            return true;
        }

        if (System.MemoryExtensions.SequenceEqual(token, "f32"u8))
        {
            type = WasmValueType.F32;
            return true;
        }

        if (System.MemoryExtensions.SequenceEqual(token, "f64"u8))
        {
            type = WasmValueType.F64;
            return true;
        }

        return false;
    }

    /// <summary>Reads one literal in the spelling its type fixes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=DEABD5
    // Broiler-Falsified-If: a float literal of the wrong digit count is accepted, or an integer literal is accepted with a trailing byte
    // Broiler-Human:        PENDING
    private static bool TryReadLiteral(
        WasmValueType type, System.ReadOnlySpan<byte> literal, out WasmValue value)
    {
        value = WasmValue.Zero;

        switch (type)
        {
            case WasmValueType.F32:
                if (literal.Length != 8 || !TryReadHex(literal, out var single))
                {
                    return false;
                }

                value = WasmValue.FromBits(single);
                return true;

            case WasmValueType.F64:
                if (literal.Length != 16 || !TryReadHex(literal, out var doublePrecision))
                {
                    return false;
                }

                value = WasmValue.FromBits(doublePrecision);
                return true;

            default:
                if (!TryReadInteger(literal, out var bits))
                {
                    return false;
                }

                value = WasmValue.FromBits(
                    type is WasmValueType.I32 ? (uint)bits : bits);

                return true;
        }
    }

    /// <summary>Reads a fixed-width hexadecimal bit pattern.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=2; Fingerprint=3F8ADE
    // Broiler-Human:        PENDING
    private static bool TryReadHex(System.ReadOnlySpan<byte> literal, out ulong bits)
    {
        bits = 0;

        for (var index = 0; index < literal.Length; index++)
        {
            var digit = literal[index];

            var nibble = digit switch
            {
                >= (byte)'0' and <= (byte)'9' => digit - (byte)'0',
                >= (byte)'a' and <= (byte)'f' => digit - (byte)'a' + 10,
                >= (byte)'A' and <= (byte)'F' => digit - (byte)'A' + 10,
                _ => -1,
            };

            if (nibble < 0)
            {
                return false;
            }

            bits = (bits << 4) | (uint)nibble;
        }

        return true;
    }

    /// <summary>
    /// Reads a decimal integer, optionally signed, or a <c>0x</c>-prefixed bit pattern.
    /// </summary>
    /// <remarks>
    /// An unsigned decimal is accumulated modulo two to the sixty-four, which is what lets the same
    /// spelling carry both a signed and an unsigned reading of one 64-bit slot: the guest's
    /// instructions decide which of the two the bits mean, exactly as they do inside the module.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=3; Fingerprint=247F89
    // Broiler-Falsified-If: a literal with no digits or with a trailing non-digit is accepted
    // Broiler-Human:        PENDING
    private static bool TryReadInteger(System.ReadOnlySpan<byte> literal, out ulong bits)
    {
        bits = 0;

        if (literal.Length == 0)
        {
            return false;
        }

        if (literal.Length > 2 &&
            literal[0] == (byte)'0' &&
            (literal[1] == (byte)'x' || literal[1] == (byte)'X'))
        {
            return literal.Length <= 18 && TryReadHex(literal[2..], out bits);
        }

        var negative = literal[0] == (byte)'-';
        var digits = negative ? literal[1..] : literal;

        if (digits.Length == 0)
        {
            return false;
        }

        var magnitude = 0UL;

        for (var index = 0; index < digits.Length; index++)
        {
            if (digits[index] is < (byte)'0' or > (byte)'9')
            {
                return false;
            }

            magnitude = unchecked((magnitude * 10) + (ulong)(digits[index] - (byte)'0'));
        }

        bits = negative ? unchecked(0UL - magnitude) : magnitude;
        return true;
    }
}
