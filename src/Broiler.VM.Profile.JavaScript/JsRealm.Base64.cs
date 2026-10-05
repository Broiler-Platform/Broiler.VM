// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   20
// Annotated:        20/20
// Exempt:           3
// Human-reviewed:   0/20
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       20
//
// GENERATED - DO NOT EDIT MANUALLY

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// <c>Uint8Array</c>'s base64 and hex members: <c>Uint8Array.fromBase64</c>,
/// <c>Uint8Array.fromHex</c>, and <c>toBase64</c>, <c>toHex</c>, <c>setFromBase64</c> and
/// <c>setFromHex</c> on its prototype.
/// </summary>
/// <remarks>
/// <para>
/// <b>They are members of the edition this profile is written against, and the realm did not have
/// them</b> (JSP-7, JSC-239). Each is the specification's algorithm step for step: the options are
/// read in its order, a decoding error is a <c>SyntaxError</c>, and <c>setFromBase64</c> and
/// <c>setFromHex</c> write every byte they decoded before the error they met, then report it.
/// </para>
/// <para>
/// <b>Nothing here runs guest code once the options have been read</b>, which is what lets the
/// decoders write straight into the buffer they validated: the specification notes the same, and it
/// is why no length is re-read after a decode.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>The standard base64 alphabet, which <c>base64url</c> differs from in two places.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=24CF9F
    // Broiler-Human:        PENDING
    private const string Base64Alphabet =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

    /// <summary>Builds the six members on <c>Uint8Array</c> and its prototype.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=D4E25A
    // Broiler-Human:        PENDING
    private void SetupUint8ArrayCodecs()
    {
        var constructor = typedArrayConstructors[JsElementKind.Uint8];
        var prototype = TypedArrayPrototypes[JsElementKind.Uint8];

        Method(constructor, "fromBase64", 1, (engine, thisValue, arguments) =>
        {
            var text = Base64Argument(arguments, 0);

            if (!text.IsString)
            {
                return engine.ThrowTypeError("Uint8Array.fromBase64 requires a String");
            }

            var options = Base64Options(engine, Base64Argument(arguments, 1));
            var urlSafe = Base64ReadAlphabet(engine, options);
            var lastChunk = Base64ReadLastChunk(engine, options);
            var result = Base64Decode(engine, text.AsString(), urlSafe, lastChunk, long.MaxValue);

            if (result.Error is { } error)
            {
                throw engine.Error("SyntaxError", error);
            }

            return JsValue.Object(Base64Allocate(engine, result.Bytes));
        });

        Method(constructor, "fromHex", 1, (engine, thisValue, arguments) =>
        {
            var text = Base64Argument(arguments, 0);

            if (!text.IsString)
            {
                return engine.ThrowTypeError("Uint8Array.fromHex requires a String");
            }

            var result = Base64DecodeHex(engine, text.AsString(), long.MaxValue);

            if (result.Error is { } error)
            {
                throw engine.Error("SyntaxError", error);
            }

            return JsValue.Object(Base64Allocate(engine, result.Bytes));
        });

        Method(prototype, "toBase64", 0, static (engine, thisValue, arguments) =>
        {
            var view = Base64Receiver(engine, thisValue, "toBase64");
            var options = Base64Options(engine, Base64Argument(arguments, 0));
            var urlSafe = Base64ReadAlphabet(engine, options);
            var omitPadding = engine.GetProperty(JsValue.Object(options), "omitPadding").ToBooleanValue();
            var bytes = Base64BytesOf(engine, view);
            return JsValue.String(Base64Encode(engine, bytes, urlSafe, omitPadding));
        });

        Method(prototype, "toHex", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var bytes = Base64BytesOf(engine, Base64Receiver(engine, thisValue, "toHex"));
            engine.Charge((ulong)bytes.Length * 2 + 1);
            return JsValue.String(System.Convert.ToHexStringLower(bytes));
        });

        Method(prototype, "setFromBase64", 1, (engine, thisValue, arguments) =>
        {
            var into = Base64Receiver(engine, thisValue, "setFromBase64");
            var text = Base64Argument(arguments, 0);

            if (!text.IsString)
            {
                return engine.ThrowTypeError("Uint8Array.prototype.setFromBase64 requires a String");
            }

            var options = Base64Options(engine, Base64Argument(arguments, 1));
            var urlSafe = Base64ReadAlphabet(engine, options);
            var lastChunk = Base64ReadLastChunk(engine, options);
            var capacity = Base64Capacity(engine, into);
            var result = Base64Decode(engine, text.AsString(), urlSafe, lastChunk, capacity);
            return Base64Settle(engine, into, result);
        });

        Method(prototype, "setFromHex", 1, (engine, thisValue, arguments) =>
        {
            var into = Base64Receiver(engine, thisValue, "setFromHex");
            var text = Base64Argument(arguments, 0);

            if (!text.IsString)
            {
                return engine.ThrowTypeError("Uint8Array.prototype.setFromHex requires a String");
            }

            var capacity = Base64Capacity(engine, into);
            var result = Base64DecodeHex(engine, text.AsString(), capacity);
            return Base64Settle(engine, into, result);
        });
    }

    /// <summary>Reads argument <paramref name="at"/>, which may not have been supplied.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=35C21F
    // Broiler-Human:        PENDING
    private static JsValue Base64Argument(JsValue[] arguments, int at) =>
        at < arguments.Length ? arguments[at] : JsValue.Undefined;

    /// <summary>
    /// The specification's <c>ValidateUint8Array</c>: the receiver, which must be a
    /// <c>Uint8Array</c> and not merely a typed array of bytes.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=96450A
    // Broiler-Human:        PENDING
    private static JsTypedArray Base64Receiver(JsEngine engine, JsValue value, string method)
    {
        if (value.AsObjectOrNull() is JsTypedArray { Kind: JsElementKind.Uint8 } view)
        {
            return view;
        }

        engine.ThrowTypeError("Uint8Array.prototype." + method + " requires a Uint8Array receiver");
        return null!;
    }

    /// <summary>
    /// The specification's <c>GetOptionsObject</c>: an object with no prototype for an absent bag, the
    /// bag itself for an object, and a <c>TypeError</c> for anything else.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=329935
    // Broiler-Human:        PENDING
    private static JsObject Base64Options(JsEngine engine, JsValue options)
    {
        if (options.Type == JsType.Undefined)
        {
            return new JsObject(null);
        }

        if (options.IsObject)
        {
            return options.AsObject();
        }

        engine.ThrowTypeError("the options argument is not an object");
        return null!;
    }

    /// <summary>Reads <c>alphabet</c>, answering whether it is <c>base64url</c>.</summary>
    /// <remarks>
    /// The value is compared and not converted: only the two Strings are alphabets, and an object
    /// whose <c>toString</c> answers one of them is a <c>TypeError</c>, as the specification says.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=948DB4
    // Broiler-Human:        PENDING
    private static bool Base64ReadAlphabet(JsEngine engine, JsObject options)
    {
        var alphabet = engine.GetProperty(JsValue.Object(options), "alphabet");

        if (alphabet.Type == JsType.Undefined)
        {
            return false;
        }

        if (alphabet.IsString && alphabet.AsString() is "base64" or "base64url")
        {
            return alphabet.AsString() == "base64url";
        }

        engine.ThrowTypeError("the alphabet must be \"base64\" or \"base64url\"");
        return false;
    }

    /// <summary>Reads <c>lastChunkHandling</c>: <c>loose</c>, <c>strict</c> or <c>stop-before-partial</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BC0FB5
    // Broiler-Human:        PENDING
    private static string Base64ReadLastChunk(JsEngine engine, JsObject options)
    {
        var handling = engine.GetProperty(JsValue.Object(options), "lastChunkHandling");

        if (handling.Type == JsType.Undefined)
        {
            return "loose";
        }

        if (handling.IsString && handling.AsString() is "loose" or "strict" or "stop-before-partial")
        {
            return handling.AsString();
        }

        engine.ThrowTypeError(
            "lastChunkHandling must be \"loose\", \"strict\" or \"stop-before-partial\"");
        return string.Empty;
    }

    /// <summary>
    /// The specification's <c>GetUint8ArrayBytes</c>: the view's bytes, or a <c>TypeError</c> when it
    /// is out of bounds.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=055594
    // Broiler-Human:        PENDING
    private static byte[] Base64BytesOf(JsEngine engine, JsTypedArray view)
    {
        if (view.IsOutOfBounds || view.Buffer.Data is not { } data)
        {
            engine.ThrowTypeError("the Uint8Array is out of bounds");
            return [];
        }

        var length = view.Length;
        engine.Charge((ulong)length + 1);
        return new System.Span<byte>(data, view.ByteOffset, length).ToArray();
    }

    /// <summary>
    /// How many bytes a <c>setFrom…</c> call may write: the view's length, or a <c>TypeError</c> when
    /// it is out of bounds.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=2B94B8
    // Broiler-Human:        PENDING
    private static long Base64Capacity(JsEngine engine, JsTypedArray view)
    {
        if (view.IsOutOfBounds)
        {
            engine.ThrowTypeError("the Uint8Array is out of bounds");
        }

        return view.Length;
    }

    /// <summary>
    /// Writes what a <c>setFrom…</c> decode produced, then throws its error or answers
    /// <c>{ read, written }</c>.
    /// </summary>
    /// <remarks>
    /// <b>The bytes are written before the error is thrown</b>, which is the specification's order:
    /// a decode that fails part way leaves what it decoded in the array.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A23CC8
    // Broiler-Human:        PENDING
    private JsValue Base64Settle(JsEngine engine, JsTypedArray into, Base64Result result)
    {
        if (into.Buffer.Data is { } data)
        {
            result.Bytes.CopyTo(data, into.ByteOffset);
        }

        if (result.Error is { } error)
        {
            throw engine.Error("SyntaxError", error);
        }

        var answer = new JsObject(ObjectPrototype);
        answer.DefineOrdinary("read", JsValue.Number(result.Read));
        answer.DefineOrdinary("written", JsValue.Number(result.Bytes.Count));
        return JsValue.Object(answer);
    }

    /// <summary>A new <c>Uint8Array</c> holding <paramref name="bytes"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=B12042
    // Broiler-Human:        PENDING
    private JsTypedArray Base64Allocate(JsEngine engine, System.Collections.Generic.List<byte> bytes)
    {
        var made = BinaryNewTypedArray(engine, JsElementKind.Uint8, bytes.Count);
        bytes.CopyTo(made.Buffer.Data!, made.ByteOffset);
        return made;
    }

    /// <summary>Encodes <paramref name="bytes"/> as RFC 4648 section 4 or section 5 base64.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=ABA28D
    // Broiler-Human:        PENDING
    private static string Base64Encode(JsEngine engine, byte[] bytes, bool urlSafe, bool omitPadding)
    {
        engine.Charge(((ulong)bytes.Length * 4 / 3) + 4);
        var text = System.Convert.ToBase64String(bytes);

        if (urlSafe)
        {
            text = text.Replace('+', '-').Replace('/', '_');
        }

        return omitPadding ? text.TrimEnd('=') : text;
    }

    /// <summary>What a decode produced: the bytes, how much of the text it read, and its error.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FE03F7
    // Broiler-Human:        PENDING
    private sealed class Base64Result
    {
        /// <summary>The bytes decoded before the decode stopped.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=BFBD94
        // Broiler-Human:        PENDING
        internal System.Collections.Generic.List<byte> Bytes { get; } = [];

        /// <summary>How many code units of the text the decoded bytes account for.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=095A82
        // Broiler-Human:        PENDING
        internal int Read { get; set; }

        /// <summary>The <c>SyntaxError</c>'s message, or <see langword="null"/> when there was none.</summary>
        // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=04608A
        // Broiler-Human:        PENDING
        internal string? Error { get; set; }
    }

    /// <summary>The specification's <c>SkipAsciiWhitespace</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=5D511B
    // Broiler-Human:        PENDING
    private static int Base64SkipSpace(string text, int at)
    {
        while (at < text.Length && text[at] is '\t' or '\n' or '\f' or '\r' or ' ')
        {
            at++;
        }

        return at;
    }

    /// <summary>
    /// The specification's <c>FromBase64</c>: decodes at most <paramref name="maxLength"/> bytes,
    /// answering what it read and the error it stopped at.
    /// </summary>
    /// <remarks>
    /// A chunk of four characters is three bytes. A final chunk of two or three characters is one or
    /// two, padded or not as <paramref name="lastChunk"/> allows; under <c>strict</c> its unused bits
    /// must be zero and it must be padded. A chunk that would not fit in what is left of
    /// <paramref name="maxLength"/> is not started, and <c>read</c> stops before it.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=442B42
    // Broiler-Human:        PENDING
    private static Base64Result Base64Decode(
        JsEngine engine, string text, bool urlSafe, string lastChunk, long maxLength)
    {
        var result = new Base64Result();

        if (maxLength == 0)
        {
            return result;
        }

        engine.Charge((ulong)text.Length + 1);
        var chunk = new char[4];
        var chunkLength = 0;
        var at = 0;

        while (true)
        {
            at = Base64SkipSpace(text, at);

            if (at == text.Length)
            {
                if (chunkLength > 0)
                {
                    if (lastChunk == "stop-before-partial")
                    {
                        return result;
                    }

                    if (lastChunk == "strict")
                    {
                        result.Error = "the base64 text ends in an unpadded chunk";
                        return result;
                    }

                    if (chunkLength == 1)
                    {
                        result.Error = "the base64 text ends in a chunk of one character";
                        return result;
                    }

                    Base64DecodeFinal(chunk, chunkLength, false, result.Bytes);
                }

                result.Read = text.Length;
                return result;
            }

            var character = text[at];
            at++;

            if (character == '=')
            {
                if (chunkLength < 2)
                {
                    result.Error = "padding in base64 text where a chunk has fewer than two characters";
                    return result;
                }

                at = Base64SkipSpace(text, at);

                if (chunkLength == 2)
                {
                    if (at == text.Length)
                    {
                        if (lastChunk != "stop-before-partial")
                        {
                            result.Error = "the base64 text ends in a chunk with one padding character of two";
                        }

                        return result;
                    }

                    if (text[at] == '=')
                    {
                        at = Base64SkipSpace(text, at + 1);
                    }
                }

                if (at < text.Length)
                {
                    result.Error = "base64 text continues after its padding";
                    return result;
                }

                if (!Base64DecodeFinal(chunk, chunkLength, lastChunk == "strict", result.Bytes))
                {
                    result.Error = "the final base64 chunk has bits set past its last byte";
                    return result;
                }

                result.Read = text.Length;
                return result;
            }

            if (urlSafe)
            {
                if (character is '+' or '/')
                {
                    result.Error = "'" + character + "' is not in the base64url alphabet";
                    return result;
                }

                character = character switch
                {
                    '-' => '+',
                    '_' => '/',
                    _ => character,
                };
            }

            if (Base64Alphabet.IndexOf(character, System.StringComparison.Ordinal) < 0)
            {
                result.Error = "'" + character + "' is not a base64 character";
                return result;
            }

            var remaining = maxLength - result.Bytes.Count;

            if ((remaining == 1 && chunkLength == 2) || (remaining == 2 && chunkLength == 3))
            {
                return result;
            }

            chunk[chunkLength++] = character;

            if (chunkLength == 4)
            {
                Base64DecodeFull(chunk, result.Bytes);
                chunkLength = 0;
                result.Read = at;

                if (result.Bytes.Count == maxLength)
                {
                    return result;
                }
            }
        }
    }

    /// <summary>The specification's <c>DecodeFullLengthBase64Chunk</c>: four characters, three bytes.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=19F167
    // Broiler-Human:        PENDING
    private static void Base64DecodeFull(char[] chunk, System.Collections.Generic.List<byte> bytes)
    {
        var bits = 0;

        for (var at = 0; at < 4; at++)
        {
            bits = (bits << 6) | Base64Alphabet.IndexOf(chunk[at], System.StringComparison.Ordinal);
        }

        bytes.Add((byte)(bits >> 16));
        bytes.Add((byte)(bits >> 8));
        bytes.Add((byte)bits);
    }

    /// <summary>
    /// The specification's <c>DecodeFinalBase64Chunk</c>: two characters are one byte and three are
    /// two, and under <paramref name="strict"/> the bits past the last byte must be zero.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=6EC2A6
    // Broiler-Human:        PENDING
    private static bool Base64DecodeFinal(
        char[] chunk, int length, bool strict, System.Collections.Generic.List<byte> bytes)
    {
        var padded = new[] { chunk[0], chunk[1], length == 3 ? chunk[2] : 'A', 'A' };
        var decoded = new System.Collections.Generic.List<byte>(3);
        Base64DecodeFull(padded, decoded);

        if (strict && decoded[length - 1] != 0)
        {
            return false;
        }

        bytes.Add(decoded[0]);

        if (length == 3)
        {
            bytes.Add(decoded[1]);
        }

        return true;
    }

    /// <summary>The value of one hex digit of either case, or -1 for any other character.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=F88009
    // Broiler-Human:        PENDING
    private static int Base64HexDigit(char character) => character switch
    {
        >= '0' and <= '9' => character - '0',
        >= 'a' and <= 'f' => character - 'a' + 10,
        >= 'A' and <= 'F' => character - 'A' + 10,
        _ => -1,
    };

    /// <summary>
    /// The specification's <c>FromHex</c>: pairs of hex digits, at most <paramref name="maxLength"/>
    /// bytes, and an error for an odd length or a character that is not one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=7DC176
    // Broiler-Human:        PENDING
    private static Base64Result Base64DecodeHex(JsEngine engine, string text, long maxLength)
    {
        var result = new Base64Result();

        if (text.Length % 2 != 0)
        {
            result.Error = "hex text has an odd number of characters";
            return result;
        }

        engine.Charge((ulong)text.Length + 1);

        while (result.Read < text.Length && result.Bytes.Count < maxLength)
        {
            var high = Base64HexDigit(text[result.Read]);
            var low = Base64HexDigit(text[result.Read + 1]);

            if (high < 0 || low < 0)
            {
                result.Error = "'" + text.Substring(result.Read, 2) + "' is not a pair of hex digits";
                return result;
            }

            result.Read += 2;
            result.Bytes.Add((byte)((high << 4) | low));
        }

        return result;
    }
}
