// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   5
// Annotated:        5/5
// Exempt:           0
// Human-reviewed:   0/5
// IP risk:          Low
// Security risk:    Critical
// Criteria:         5/5
// Resource impact:  1/10 max
// Unverified:       5
//
// GENERATED - DO NOT EDIT MANUALLY

using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

/// <summary>
/// The codec between the interpreter's value and a value-form word, over every kind the interpreter
/// has (JSD-0035 section 2).
/// </summary>
/// <remarks>
/// <para>
/// <b>IT IS THE WHOLE OF WHAT A HELPER ADDS TO AN ARM.</b> A value-form helper decodes an instruction's
/// input words, runs the interpreter's own arm over them, and encodes its outputs, so every difference
/// between the two forms that is not in an inline template is in this type. That is why it is small
/// and why the checks drive it over every kind and every NaN payload rather than over examples.
/// </para>
/// <para>
/// <b>ONE VALUE IS NOT ALWAYS CARRIED BIT FOR BIT: A NaN.</b> A NaN whose payload could carry it into a tag
/// is encoded as the canonical one (<see cref="Format.JsWord.NaNTagReach"/>); every other NaN, every one
/// the arithmetic unit and the runtime produce among them, is carried as it is. JavaScript observes a
/// NaN's payload only through typed-array bytes, which never hold a word, so the canonical NaN decodes to
/// a value the language cannot tell from the one encoded.
/// </para>
/// </remarks>
// Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=3AF53F
// Broiler-Falsified-If: decoding the encoding of a value answers a value of another kind, another reference, or a Number whose bits differ other than by a NaN's payload
// Broiler-Human:        PENDING
internal static class JsWordCodec
{
    /// <summary>The word for <paramref name="value"/>, allocating a handle for a referenced value.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=C0F227
    // Broiler-Falsified-If: a value of some kind is encoded under another kind's tag, or a referenced value is encoded without a handle the table holds
    // Broiler-Human:        PENDING
    internal static ulong Encode(in JsValue value, JsHandleTable table) => value.Type switch
    {
        JsType.Empty => JsWord.Empty,
        JsType.Undefined => JsWord.Undefined,
        JsType.Null => JsWord.Null,
        JsType.Boolean => value.AsBoolean() ? JsWord.True : JsWord.False,
        JsType.Number => JsWord.FromNumber(value.AsNumber()),
        JsType.String => table.HandleFor(value.AsString(), JsWord.StringTag),
        JsType.Object => table.HandleFor(value.AsObject(), JsWord.ObjectTag),
        JsType.Symbol => table.HandleFor(value.AsSymbol(), JsWord.SymbolTag),
        JsType.BigInt => table.HandleFor(value.AsBigInt(), JsWord.BigIntTag),
        _ => throw new JsAbort(JsAbortKind.InternalDefect, "a value of a kind the value form has no word for"),
    };

    /// <summary>The value a word carries, or false when the word carries none.</summary>
    /// <remarks>
    /// A frame header, a reserved word, a special payload outside the vocabulary and a handle the table
    /// refuses all answer false. The caller reports that as an internal defect; nothing here guesses.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Critical; Resources=1; Fingerprint=B6DDAF
    // Broiler-Falsified-If: this answers true for a frame header, a reserved word, an undefined special payload or a handle the table refuses, or answers a referenced value of a kind other than the word's tag
    // Broiler-Human:        PENDING
    internal static bool TryDecode(ulong word, JsHandleTable table, out JsValue value)
    {
        value = JsValue.Undefined;

        if (JsWord.IsNumber(word))
        {
            value = JsValue.Number(JsWord.ToNumber(word));
            return true;
        }

        switch (JsWord.Tag(word))
        {
            case JsWord.SpecialTag:
                return TryDecodeSpecial(word, out value);

            case JsWord.StringTag when table.TryResolve(word, out var target) && target is string text:
                value = JsValue.String(text);
                return true;

            case JsWord.ObjectTag when table.TryResolve(word, out var target) && target is JsObject obj:
                value = JsValue.Object(obj);
                return true;

            case JsWord.SymbolTag when table.TryResolve(word, out var target) && target is JsSymbol symbol:
                value = JsValue.Symbol(symbol);
                return true;

            case JsWord.BigIntTag when table.TryResolve(word, out var target) && target is JsBigInt big:
                value = JsValue.BigInt(big);
                return true;

            default:
                return false;
        }
    }

    /// <summary>The value a word carries, or an internal defect.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=1; Fingerprint=708398
    // Broiler-Falsified-If: a word TryDecode refuses answers a value here rather than a defect
    // Broiler-Human:        PENDING
    internal static JsValue Decode(ulong word, JsHandleTable table) =>
        TryDecode(word, table, out var value)
            ? value
            : throw new JsAbort(JsAbortKind.InternalDefect, "a value-form word names no value");

    /// <summary>One of the five special constants, by payload.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=0; Fingerprint=D684DB
    // Broiler-Falsified-If: a payload at or past SpecialCount answers a value
    // Broiler-Human:        PENDING
    private static bool TryDecodeSpecial(ulong word, out JsValue value)
    {
        value = word switch
        {
            JsWord.Undefined => JsValue.Undefined,
            JsWord.Null => JsValue.Null,
            JsWord.False => JsValue.False,
            JsWord.True => JsValue.True,
            JsWord.Empty => JsValue.Empty,
            _ => JsValue.Undefined,
        };

        return (word & JsWord.PayloadMask) < JsWord.SpecialCount;
    }
}
