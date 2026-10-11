// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Broiler Code Assurance
// ----------------------
// Relevant units:   16
// Annotated:        16/16
// Exempt:           0
// Human-reviewed:   0/16
// IP risk:          Low
// Security risk:    High
// Criteria:         7/7
// Resource impact:  5/10 max
// Unverified:       16
//
// GENERATED - DO NOT EDIT MANUALLY

using System;
using Broiler.VM.Profile.JavaScript.Format;

namespace Broiler.VM.Profile.JavaScript;

// Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=7BBE7E
// Broiler-Human:        PENDING
internal sealed partial class JsEngine
{

    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=CFEDD9
    // Broiler-Human:        PENDING
    private static void Binary(JsValue[] stack, ref int sp, JsOpcode opcode, Func<double, double, double> operation, JsEngine engine)
    {
        // BOTH OPERANDS LEAVE THE STACK BEFORE EITHER IS CONVERTED, AND THE LEFT IS CONVERTED FIRST.
        // ToNumeric can run guest code (valueOf, toString, Symbol.toPrimitive), so the order is
        // observable, and a throw from the left operand's conversion must leave the right one
        // unconverted. Two Numbers take the operation they always took; anything else is BigInt
        // arithmetic or the TypeError for mixing the two (JSeal B03).
        var rightValue = stack[--sp];
        var leftValue = stack[--sp];
        var left = engine.ToNumeric(leftValue);
        var right = engine.ToNumeric(rightValue);

        stack[sp++] = left.IsNumber && right.IsNumber
            ? JsValue.Number(operation(left.AsNumber(), right.AsNumber()))
            : engine.BigIntBinary(opcode, left, right);
    }

    /// <summary>The <c>+</c> operator, which is concatenation when either side is a String.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=454988
    // Broiler-Human:        PENDING
    internal JsValue Add(JsValue left, JsValue right)
    {
        var primitiveLeft = ToPrimitive(left, "default");
        var primitiveRight = ToPrimitive(right, "default");

        if (primitiveLeft.IsString || primitiveRight.IsString)
        {
            return JsValue.String(ToStringValue(primitiveLeft) + ToStringValue(primitiveRight));
        }

        var numericLeft = ToNumeric(primitiveLeft);
        var numericRight = ToNumeric(primitiveRight);

        return numericLeft.IsNumber && numericRight.IsNumber
            ? JsValue.Number(numericLeft.AsNumber() + numericRight.AsNumber())
            : BigIntBinary(JsOpcode.Add, numericLeft, numericRight);
    }

    /// <summary>
    /// The six bitwise and shift operators: <c>ToNumeric</c> on both operands, left first, then the
    /// 32-bit Number operation or the BigInt one.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=1760EF
    // Broiler-Human:        PENDING
    internal JsValue Bitwise(JsOpcode opcode, JsValue leftValue, JsValue rightValue)
    {
        var left = ToNumeric(leftValue);
        var right = ToNumeric(rightValue);

        if (!left.IsNumber || !right.IsNumber)
        {
            return BigIntBinary(opcode, left, right);
        }

        var a = left.AsNumber();
        var b = right.AsNumber();

        return opcode switch
        {
            JsOpcode.BitwiseOr => JsValue.Number(JsValue.ToInt32(a) | JsValue.ToInt32(b)),
            JsOpcode.BitwiseAnd => JsValue.Number(JsValue.ToInt32(a) & JsValue.ToInt32(b)),
            JsOpcode.BitwiseXor => JsValue.Number(JsValue.ToInt32(a) ^ JsValue.ToInt32(b)),
            JsOpcode.ShiftLeft => JsValue.Number(JsValue.ToInt32(a) << (int)(JsValue.ToUint32(b) & 31)),
            JsOpcode.ShiftRight => JsValue.Number(JsValue.ToInt32(a) >> (int)(JsValue.ToUint32(b) & 31)),
            _ => JsValue.Number(JsValue.ToUint32(a) >> (int)(JsValue.ToUint32(b) & 31)),
        };
    }

    /// <summary>
    /// A binary numeric operator over two values <c>ToNumeric</c> has already produced, at least
    /// one of them a BigInt: the TypeError for mixing, or the BigInt operation (JSeal B03-B04).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>No operand is ever read as a Number here.</b> Mixing is refused before anything is
    /// computed, as the specification's <c>ApplyStringOrNumericBinaryOperator</c> refuses it, and
    /// <c>&gt;&gt;&gt;</c> is refused on two BigInts because <c>BigInt::unsignedRightShift</c> is a
    /// TypeError - a BigInt has no fixed width to fill with zeros.
    /// </para>
    /// <para>
    /// The errors the specification gives the arithmetic itself are raised here, before the
    /// operation runs: a zero divisor for <c>/</c> and <c>%</c> and a negative exponent for
    /// <c>**</c> are RangeErrors. A result wider than the realm's ceiling is a RangeError too
    /// (<see cref="BigIntResult"/>); the operations charge fuel before each step.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=A07DA3
    // Broiler-Falsified-If: a Number operand is mixed into a BigInt result, a BigInt is read as a Number, a zero divisor or negative exponent answers anything but a RangeError, or >>> answers a value
    // Broiler-Human:        PENDING
    internal JsValue BigIntBinary(JsOpcode opcode, JsValue left, JsValue right)
    {
        if (!left.IsBigInt || !right.IsBigInt)
        {
            return ThrowTypeError("Cannot mix BigInt and other types, use explicit conversions");
        }

        var a = left.AsBigInt();
        var b = right.AsBigInt();

        return opcode switch
        {
            JsOpcode.Divide or JsOpcode.Remainder when b.IsZero => ThrowRangeError("Division by zero"),
            JsOpcode.Exponent when b.Value.Sign < 0 => ThrowRangeError("Exponent must be non-negative"),
            JsOpcode.ShiftRightUnsigned => ThrowTypeError("BigInts have no unsigned right shift, use >> instead"),
            _ => BigIntResult(opcode switch
            {
                JsOpcode.Add => JsBigInt.Add(a, b, ChargeFuel),
                JsOpcode.Subtract => JsBigInt.Subtract(a, b, ChargeFuel),
                JsOpcode.Multiply => JsBigInt.Multiply(a, b, ChargeFuel),
                JsOpcode.Divide => JsBigInt.Divide(a, b, ChargeFuel),
                JsOpcode.Remainder => JsBigInt.Remainder(a, b, ChargeFuel),
                JsOpcode.Exponent => JsBigInt.Power(a, b, ChargeFuel),
                JsOpcode.BitwiseAnd => JsBigInt.And(a, b, ChargeFuel),
                JsOpcode.BitwiseOr => JsBigInt.Or(a, b, ChargeFuel),
                JsOpcode.BitwiseXor => JsBigInt.Xor(a, b, ChargeFuel),
                JsOpcode.ShiftLeft => JsBigInt.ShiftLeft(a, b, ChargeFuel),
                JsOpcode.ShiftRight => JsBigInt.ShiftRight(a, b, ChargeFuel),
                _ => throw new InvalidOperationException("not a binary numeric operator: " + opcode),
            }),
        };
    }

    /// <summary>
    /// One step of an update expression: <c>x + 1</c> or <c>x - 1</c> for a Number, <c>x + 1n</c>
    /// or <c>x - 1n</c> for a BigInt, after a <c>ToNumeric</c> of anything else.
    /// </summary>
    /// <remarks>
    /// The specification's update is <c>Number::add(oldValue, 1)</c> or <c>BigInt::add(oldValue,
    /// 1n)</c> by the operand's type (and the same with subtract); mixing cannot arise, because the
    /// one is always of the operand's own type. The BigInt step is charged and bounded exactly as
    /// the <c>+</c> and <c>-</c> operators are.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=27D8C7
    // Broiler-Falsified-If: a BigInt operand is stepped through a double or answers a Number, or a Number operand answers anything but x + 1 or x - 1
    // Broiler-Human:        PENDING
    internal JsValue NumericStep(JsValue value, bool increment)
    {
        var numeric = ToNumeric(value);

        if (numeric.IsNumber)
        {
            return JsValue.Number(increment ? numeric.AsNumber() + 1 : numeric.AsNumber() - 1);
        }

        return BigIntResult(
            increment
                ? JsBigInt.Add(numeric.AsBigInt(), JsBigInt.One, ChargeFuel)
                : JsBigInt.Subtract(numeric.AsBigInt(), JsBigInt.One, ChargeFuel));
    }

    /// <summary>
    /// A computed BigInt as a value, or the RangeError for one wider than the realm's ceiling.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=FAB1EA
    // Broiler-Falsified-If: an operation whose result is past JsBigInt.MaximumBits answers a value or escapes as anything but a RangeError
    // Broiler-Human:        PENDING
    internal JsValue BigIntResult(JsBigInt? result) =>
        result is null
            ? ThrowRangeError(
                "Maximum BigInt size exceeded: the result would be wider than " +
                JsBigInt.MaximumBits.ToString(System.Globalization.CultureInfo.InvariantCulture) + " bits")
            : JsValue.BigInt(result);

    /// <summary>The four relational operators, through one abstract comparison.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=D817E1
    // Broiler-Human:        PENDING
    internal bool Relational(JsOpcode opcode, JsValue left, JsValue right)
    {
        // THE ORDER OF EVALUATION IS THE SPECIFICATION'S: all four operators convert the LEFT
        // operand first. `>` and `>=` swap the operands of IsLessThan but pass LeftFirst = false,
        // which converts that call's second argument - the source's left operand - first. It is
        // observable through a valueOf with a side effect, and it is the kind of thing only a
        // conformance suite ever notices.
        var first = ToPrimitive(left, "number");
        var second = ToPrimitive(right, "number");

        if (first.IsString && second.IsString)
        {
            var firstText = first.AsString();
            var secondText = second.AsString();

            // An ordinal comparison stops at the first difference, so the shorter operand bounds
            // the work whatever the longer one holds.
            ChargeText(Math.Min(firstText.Length, secondText.Length));

            var order = string.CompareOrdinal(firstText, secondText);

            return opcode switch
            {
                JsOpcode.LessThan => order < 0,
                JsOpcode.LessThanOrEqual => order <= 0,
                JsOpcode.GreaterThan => order > 0,
                _ => order >= 0,
            };
        }

        // A BIGINT ON EITHER SIDE IS COMPARED EXACTLY (JSeal B05), never through a double:
        // `9007199254740993n > 9007199254740992` is true, which no rounding of the left operand
        // could answer. An undefined comparison - a NaN, or a String that is not an integer - is
        // false for all four operators, as the Number path's IEEE comparison already makes it.
        if (first.IsBigInt || second.IsBigInt)
        {
            return BigIntOrder(first, second) is { } order && opcode switch
            {
                JsOpcode.LessThan => order < 0,
                JsOpcode.LessThanOrEqual => order <= 0,
                JsOpcode.GreaterThan => order > 0,
                _ => order >= 0,
            };
        }

        var a = ToNumber(first);
        var b = ToNumber(second);

        return opcode switch
        {
            JsOpcode.LessThan => a < b,
            JsOpcode.LessThanOrEqual => a <= b,
            JsOpcode.GreaterThan => a > b,
            _ => a >= b,
        };
    }

    /// <summary>
    /// The order of two primitives at least one of which is a BigInt and which are not both
    /// Strings: negative, zero or positive, or <see langword="null"/> where the specification's
    /// <c>IsLessThan</c> answers <c>undefined</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A String beside a BigInt is read with <c>StringToBigInt</c>, not <c>ToNumber</c></b>, so
    /// <c>'9007199254740993' &gt; 9007199254740992n</c> is true; a String outside the grammar is
    /// undefined, and one spelling a value wider than the realm's ceiling is ordered by its sign,
    /// which is exact because no value in the realm is that wide.
    /// </para>
    /// <para>
    /// Otherwise both sides take <c>ToNumeric</c>, which refuses a Symbol, and a Number beside a
    /// BigInt is compared by <see cref="JsBigInt.CompareToNumber"/>: a NaN is undefined and an
    /// infinity is beyond every BigInt.
    /// </para>
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=309DC2
    // Broiler-Falsified-If: a BigInt and a Number or String are ordered other than by their mathematical values, or either is read through a double
    // Broiler-Human:        PENDING
    private int? BigIntOrder(JsValue first, JsValue second)
    {
        if (first.IsBigInt && second.IsString)
        {
            if (!JsBigInt.TryParseStringInteger(second.AsString(), ChargeFuel, out var parsed, out var wide))
            {
                return null;
            }

            return parsed is null ? -wide : CompareBigInts(first.AsBigInt(), parsed);
        }

        if (first.IsString && second.IsBigInt)
        {
            if (!JsBigInt.TryParseStringInteger(first.AsString(), ChargeFuel, out var parsed, out var wide))
            {
                return null;
            }

            return parsed is null ? wide : CompareBigInts(parsed, second.AsBigInt());
        }

        var a = ToNumeric(first);
        var b = ToNumeric(second);

        if (a.IsBigInt && b.IsBigInt)
        {
            return CompareBigInts(a.AsBigInt(), b.AsBigInt());
        }

        return a.IsBigInt ? OrderAgainstNumber(a.AsBigInt(), b.AsNumber()) : -OrderAgainstNumber(b.AsBigInt(), a.AsNumber());
    }

    /// <summary>The order of two BigInts, charged on the narrower one's words.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=DBA0AD
    // Broiler-Human:        PENDING
    private int CompareBigInts(JsBigInt left, JsBigInt right)
    {
        ChargeText(Math.Min(left.Words, right.Words));
        return left.Value.CompareTo(right.Value);
    }

    /// <summary>
    /// The order of a BigInt against a Number, or <see langword="null"/> for a NaN; an infinity is
    /// beyond every BigInt.
    /// </summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=5CB7FA
    // Broiler-Falsified-If: a BigInt is ordered against a Number other than by their mathematical values
    // Broiler-Human:        PENDING
    private int? OrderAgainstNumber(JsBigInt value, double number) =>
        double.IsNaN(number) ? null
        : double.IsPositiveInfinity(number) ? -1
        : double.IsNegativeInfinity(number) ? 1
        : value.CompareToNumber(number, ChargeFuel);

    /// <summary>
    /// Charges an equality comparison for the text it may have to read.
    /// </summary>
    /// <remarks>
    /// Only the string-against-string case can read anything: every other pair of the same type
    /// compares a word or a reference, and two different types answer without looking. Equality
    /// answers immediately when the lengths differ, so like the relational comparison beside it the
    /// shorter operand bounds the work.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=FF3066
    // Broiler-Falsified-If: comparing two long equal strings costs what comparing two short ones costs
    // Broiler-Human:        PENDING
    private void ChargeComparison(JsValue left, JsValue right)
    {
        if (left.IsString && right.IsString)
        {
            ChargeText(Math.Min(left.AsString().Length, right.AsString().Length));
        }

        // Two BigInts compare word by word, and like two Strings the shorter bounds the work.
        if (left.IsBigInt && right.IsBigInt)
        {
            ChargeText(Math.Min(left.AsBigInt().Words, right.AsBigInt().Words));
        }
    }

    /// <summary>The abstract equality comparison, <c>==</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=8DCC17
    // Broiler-Human:        PENDING
    internal bool LooselyEquals(JsValue left, JsValue right)
    {
        if (left.Type == right.Type)
        {
            ChargeComparison(left, right);
            return left.StrictlyEquals(right);
        }

        if (left.IsNullish && right.IsNullish)
        {
            return true;
        }

        // AN [[IsHTMLDDA]] OBJECT IS LOOSELY EQUAL TO BOTH NULLISH VALUES (Annex B.3.6.2), and it is
        // the only object that is.
        if (left.IsNullish || right.IsNullish)
        {
            return (left.IsObject && left.AsObject().IsHtmlDda) || (right.IsObject && right.AsObject().IsHtmlDda);
        }

        if (left.Type == JsType.Number && right.Type == JsType.String)
        {
            return left.AsNumber() == ToNumber(right);
        }

        if (left.Type == JsType.String && right.Type == JsType.Number)
        {
            return ToNumber(left) == right.AsNumber();
        }

        // A BIGINT BESIDE A STRING READS THE STRING WITH `StringToBigInt` (JSeal B05): `1n == '1'`
        // is true, `1n == '1.0'` is false because the text is not an integer, and a text spelling
        // a value wider than the realm's ceiling equals nothing the realm can hold.
        if (left.IsBigInt && right.Type == JsType.String)
        {
            return BigIntEqualsText(left.AsBigInt(), right.AsString());
        }

        if (left.Type == JsType.String && right.IsBigInt)
        {
            return BigIntEqualsText(right.AsBigInt(), left.AsString());
        }

        if (left.Type == JsType.Boolean)
        {
            return LooselyEquals(JsValue.Number(left.AsBoolean() ? 1 : 0), right);
        }

        if (right.Type == JsType.Boolean)
        {
            return LooselyEquals(left, JsValue.Number(right.AsBoolean() ? 1 : 0));
        }

        // AN OBJECT BESIDE ANY OF THE FOUR PRIMITIVES THE SPECIFICATION LISTS IS CONVERTED ONCE:
        // String, Number, BigInt and Symbol. The Symbol arm was missing, so `sym == Object(sym)`
        // answered false (Test262 `equals/coerce-symbol-to-prim-return-prim.js`).
        if (left.IsObject && right.Type is JsType.Number or JsType.String or JsType.BigInt or JsType.Symbol)
        {
            return LooselyEquals(ToPrimitive(left, "default"), right);
        }

        if (right.IsObject && left.Type is JsType.Number or JsType.String or JsType.BigInt or JsType.Symbol)
        {
            return LooselyEquals(left, ToPrimitive(right, "default"));
        }

        // A BIGINT BESIDE A NUMBER IS EQUAL ONLY TO THE SAME INTEGER, compared exactly: a NaN and
        // an infinity equal no BigInt, and `9007199254740993n == 9007199254740992` is false.
        if (left.IsBigInt && right.IsNumber)
        {
            return OrderAgainstNumber(left.AsBigInt(), right.AsNumber()) == 0;
        }

        if (left.IsNumber && right.IsBigInt)
        {
            return OrderAgainstNumber(right.AsBigInt(), left.AsNumber()) == 0;
        }

        return false;
    }

    /// <summary>Whether a String spells exactly this BigInt under <c>StringToBigInt</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=High; Resources=5; Fingerprint=783A53
    // Broiler-Falsified-If: a String that is not an integer, or spells another integer, is answered equal
    // Broiler-Human:        PENDING
    private bool BigIntEqualsText(JsBigInt value, string text) =>
        JsBigInt.TryParseStringInteger(text, ChargeFuel, out var parsed, out _) &&
        parsed is not null &&
        CompareBigInts(value, parsed) == 0;

    /// <summary>The specification's <c>InstanceofOperator</c>.</summary>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=4752A7
    // Broiler-Human:        PENDING
    private bool InstanceOf(JsValue left, JsValue right)
    {
        if (!right.IsObject)
        {
            ThrowTypeError("Right-hand side of 'instanceof' is not an object");
        }

        // `Symbol.hasInstance` COMES FIRST AND IS CONSULTED ON ANY OBJECT, callable or not. That
        // ordering is what lets a plain object answer `instanceof` at all, and checking callability
        // before it would refuse the one case the Symbol exists for. Every function inherits the
        // realm's own `Function.prototype[Symbol.hasInstance]` (JSP-6, JSC-238), and calling it is
        // `OrdinaryHasInstance` with nothing observable in between, so it is answered here directly.
        if (TryGetSymbolMethod(right, Realm.HasInstanceSymbol, out var custom))
        {
            return custom.IsObject && ReferenceEquals(custom.AsObject(), Realm.HasInstanceFunction)
                ? OrdinaryHasInstance(right, left)
                : Call(custom, right, [left]).ToBooleanValue();
        }

        if (!right.AsObject().IsCallable)
        {
            ThrowTypeError("Right-hand side of 'instanceof' is not callable");
        }

        return OrdinaryHasInstance(right, left);
    }

    /// <summary>
    /// The specification's <c>OrdinaryHasInstance</c>: whether <paramref name="constructor"/>'s
    /// <c>prototype</c> is on <paramref name="value"/>'s chain.
    /// </summary>
    /// <remarks>
    /// A constructor that is not callable answers <see langword="false"/> rather than throwing - the
    /// throw is <c>instanceof</c>'s, for a right-hand side with no <c>Symbol.hasInstance</c> - and a
    /// bound function answers for its target through <c>instanceof</c> again.
    /// </remarks>
    // Broiler-AI:           Origin=AI; IP=Low; Security=Medium; Resources=5; Fingerprint=FCA9E9
    // Broiler-Human:        PENDING
    internal bool OrdinaryHasInstance(JsValue constructor, JsValue value)
    {
        if (!constructor.IsObject || !constructor.AsObject().IsCallable)
        {
            return false;
        }

        if (constructor.AsObject() is JsBoundFunction bound)
        {
            return InstanceOf(value, JsValue.Object(bound.Target));
        }

        if (!value.IsObject)
        {
            return false;
        }

        var prototype = GetProperty(constructor, "prototype");

        if (!prototype.IsObject)
        {
            ThrowTypeError("Function has non-object prototype in instanceof");
        }

        var target = prototype.AsObject();
        var walk = value.AsObject().Prototype;

        while (walk is not null)
        {
            if (ReferenceEquals(walk, target))
            {
                return true;
            }

            walk = walk.Prototype;
        }

        return false;
    }
}
