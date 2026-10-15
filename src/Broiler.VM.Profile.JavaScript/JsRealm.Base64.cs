// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   12
// Annotated:        12/12
// Exempt:           0
// Human-reviewed:   0/12
// IP risk:          Low
// Security risk:    Medium
// Criteria:         0/0
// Resource impact:  3/10 max
// Unverified:       12
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using Broiler.ECMAScript;
using Broiler.ECMAScript.Text;

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
/// <para>
/// <b>The codecs are the shared library's</b> (JSD-0071), which charge the units these members
/// charged before. What stays here is everything a script can observe around them: the receiver,
/// the options and their order, bounds, the result object and the error messages.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=60DD8D
// Broiler-Human:        PENDING
internal sealed partial class JsRealm
{
    /// <summary>Builds the six members on <c>Uint8Array</c> and its prototype.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=A6420F
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
            var alphabet = Base64ReadAlphabet(engine, options);
            var lastChunk = Base64ReadLastChunk(engine, options);
            var source = text.AsString();

            // No `maxLength`: a destination no decode of the text can fill.
            var bytes = new byte[Base64Codec.DecodedLengthBound(source.Length)];
            var status = Base64Codec.FromBase64(source, alphabet, lastChunk, bytes, out _, out var written, out var failure, engine.WorkCharge);

            if (status != ResultStatus.Completed)
            {
                throw engine.Error("SyntaxError", Base64Message(source, failure));
            }

            return JsValue.Object(Base64Allocate(engine, bytes.AsSpan(0, written)));
        });

        Method(constructor, "fromHex", 1, (engine, thisValue, arguments) =>
        {
            var text = Base64Argument(arguments, 0);

            if (!text.IsString)
            {
                return engine.ThrowTypeError("Uint8Array.fromHex requires a String");
            }

            var source = text.AsString();
            var bytes = new byte[source.Length / 2];
            var status = Base64Codec.FromHex(source, bytes, out _, out var written, out var failure, engine.WorkCharge);

            if (status != ResultStatus.Completed)
            {
                throw engine.Error("SyntaxError", Base64Message(source, failure));
            }

            return JsValue.Object(Base64Allocate(engine, bytes.AsSpan(0, written)));
        });

        Method(prototype, "toBase64", 0, static (engine, thisValue, arguments) =>
        {
            var view = Base64Receiver(engine, thisValue, "toBase64");
            var options = Base64Options(engine, Base64Argument(arguments, 0));
            var alphabet = Base64ReadAlphabet(engine, options);
            var omitPadding = engine.GetProperty(JsValue.Object(options), "omitPadding").ToBooleanValue();
            var bytes = Base64BytesOf(engine, view);
            return JsValue.String(Base64Codec.ToBase64(bytes, alphabet, omitPadding, engine.WorkCharge));
        });

        Method(prototype, "toHex", 0, static (engine, thisValue, arguments) =>
        {
            _ = arguments;
            var bytes = Base64BytesOf(engine, Base64Receiver(engine, thisValue, "toHex"));
            return JsValue.String(Base64Codec.ToHex(bytes, engine.WorkCharge));
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
            var alphabet = Base64ReadAlphabet(engine, options);
            var lastChunk = Base64ReadLastChunk(engine, options);
            var source = text.AsString();
            var status = Base64Codec.FromBase64(source, alphabet, lastChunk, Base64Destination(engine, into), out var read, out var written, out var failure, engine.WorkCharge);
            return Base64Settle(engine, source, status, failure, read, written);
        });

        Method(prototype, "setFromHex", 1, (engine, thisValue, arguments) =>
        {
            var into = Base64Receiver(engine, thisValue, "setFromHex");
            var text = Base64Argument(arguments, 0);

            if (!text.IsString)
            {
                return engine.ThrowTypeError("Uint8Array.prototype.setFromHex requires a String");
            }

            var source = text.AsString();
            var status = Base64Codec.FromHex(source, Base64Destination(engine, into), out var read, out var written, out var failure, engine.WorkCharge);
            return Base64Settle(engine, source, status, failure, read, written);
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

    /// <summary>Reads <c>alphabet</c>: <c>base64</c> or <c>base64url</c>.</summary>
    /// <remarks>
    /// The value is compared and not converted: only the two Strings are alphabets, and an object
    /// whose <c>toString</c> answers one of them is a <c>TypeError</c>, as the specification says.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3E88C2
    // Broiler-Human:        PENDING
    private static Base64Alphabet Base64ReadAlphabet(JsEngine engine, JsObject options)
    {
        var alphabet = engine.GetProperty(JsValue.Object(options), "alphabet");

        if (alphabet.Type == JsType.Undefined)
        {
            return Base64Alphabet.Base64;
        }

        if (alphabet.IsString && alphabet.AsString() is "base64" or "base64url")
        {
            return alphabet.AsString() == "base64url" ? Base64Alphabet.Base64Url : Base64Alphabet.Base64;
        }

        engine.ThrowTypeError("the alphabet must be \"base64\" or \"base64url\"");
        return Base64Alphabet.Base64;
    }

    /// <summary>Reads <c>lastChunkHandling</c>: <c>loose</c>, <c>strict</c> or <c>stop-before-partial</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=FB7A0E
    // Broiler-Human:        PENDING
    private static LastChunkHandling Base64ReadLastChunk(JsEngine engine, JsObject options)
    {
        var handling = engine.GetProperty(JsValue.Object(options), "lastChunkHandling");

        if (handling.Type == JsType.Undefined)
        {
            return LastChunkHandling.Loose;
        }

        if (handling.IsString)
        {
            switch (handling.AsString())
            {
                case "loose":
                    return LastChunkHandling.Loose;
                case "strict":
                    return LastChunkHandling.Strict;
                case "stop-before-partial":
                    return LastChunkHandling.StopBeforePartial;
            }
        }

        engine.ThrowTypeError(
            "lastChunkHandling must be \"loose\", \"strict\" or \"stop-before-partial\"");
        return LastChunkHandling.Loose;
    }

    /// <summary>
    /// The specification's <c>GetUint8ArrayBytes</c>: the view's bytes, or a <c>TypeError</c> when it
    /// is out of bounds.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=9F27B8
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
        return new Span<byte>(data, view.ByteOffset, length).ToArray();
    }

    /// <summary>
    /// Where a <c>setFrom…</c> call writes: the view's bytes, whose length is the
    /// <c>maxLength</c> the decode may not pass, or a <c>TypeError</c> when it is out of bounds.
    /// </summary>
    /// <remarks>
    /// <b>The decode writes straight into the buffer</b>, so what it decoded before an error is
    /// in the array when the error is thrown, which is the specification's order.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=DC3656
    // Broiler-Human:        PENDING
    private static Span<byte> Base64Destination(JsEngine engine, JsTypedArray view)
    {
        if (view.IsOutOfBounds)
        {
            engine.ThrowTypeError("the Uint8Array is out of bounds");
        }

        return view.Buffer.Data is { } data
            ? new Span<byte>(data, view.ByteOffset, view.Length)
            : Span<byte>.Empty;
    }

    /// <summary>Throws a <c>setFrom…</c> decode's error or answers <c>{ read, written }</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=3F56AB
    // Broiler-Human:        PENDING
    private JsValue Base64Settle(JsEngine engine, string text, ResultStatus status, Failure failure, int read, int written)
    {
        if (status != ResultStatus.Completed)
        {
            throw engine.Error("SyntaxError", Base64Message(text, failure));
        }

        var answer = new JsObject(ObjectPrototype);
        answer.DefineOrdinary("read", JsValue.Number(read));
        answer.DefineOrdinary("written", JsValue.Number(written));
        return JsValue.Object(answer);
    }

    /// <summary>A new <c>Uint8Array</c> holding <paramref name="bytes"/>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=1C25A4
    // Broiler-Human:        PENDING
    private JsTypedArray Base64Allocate(JsEngine engine, ReadOnlySpan<byte> bytes)
    {
        var made = BinaryNewTypedArray(engine, JsElementKind.Uint8, bytes.Length);
        bytes.CopyTo(new Span<byte>(made.Buffer.Data!, made.ByteOffset, bytes.Length));
        return made;
    }

    /// <summary>The <c>SyntaxError</c>'s message for a decode the library refused.</summary>
    /// <remarks>
    /// The library names the failure and where it is; the words are this profile's and are the ones
    /// it wrote before the cut-over.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=3; Fingerprint=96FF23
    // Broiler-Human:        PENDING
    private static string Base64Message(string text, Failure failure) => failure.MessageId switch
    {
        "base64.unpadded-chunk" => "the base64 text ends in an unpadded chunk",
        "base64.single-character-chunk" => "the base64 text ends in a chunk of one character",
        "base64.misplaced-padding" => "padding in base64 text where a chunk has fewer than two characters",
        "base64.incomplete-padding" => "the base64 text ends in a chunk with one padding character of two",
        "base64.text-after-padding" => "base64 text continues after its padding",
        "base64.extra-bits" => "the final base64 chunk has bits set past its last byte",
        "base64.wrong-alphabet" => "'" + text[failure.Position] + "' is not in the base64url alphabet",
        "base64.invalid-character" => "'" + text[failure.Position] + "' is not a base64 character",
        "hex.odd-length" => "hex text has an odd number of characters",
        "hex.invalid-digit" => "'" + text.Substring(failure.Position, 2) + "' is not a pair of hex digits",
        _ => failure.MessageId,
    };
}
