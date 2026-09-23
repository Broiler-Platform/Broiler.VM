// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0

using Broiler.VM;
using Broiler.VM.Profile.JavaScript;
using Broiler.VM.Profile.JavaScript.Compiler;
using Broiler.VM.Profile.JavaScript.Format;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Broiler.VM.Composition.JavaScript.SliceCompiler;

/// <summary>
/// The BigInt value kind, its literals, operators, conversions, comparisons and global (JSeal cards
/// B01-B05), judged from source through compilation, verification and execution.
/// </summary>
/// <remarks>
/// <para>
/// <b>Since card B05 the surface is public</b> (decision JSD-0033 section 7): the wide manifest
/// lowers a BigInt literal, an artifact holding one or naming the <c>BigInt</c> global declares
/// <c>broiler.javascript.bigint</c>, and the descriptor admitting every surface admits it. Until
/// B05 these checks opened an internal gate through two unsafe accessors; both doors are gone, and
/// the rows that exercised them now ask the public path and the refusal a composition that
/// declines the surface gives.
/// </para>
/// <para>
/// <b>The internal operations are still reached through unsafe accessors</b> where a row judges one
/// on its own - <c>asIntN</c> and <c>asUintN</c> at widths no guest program can name cheaply - as
/// <see cref="CloneChecks"/> reaches the clone carrier. The accessors are bound at compile time and
/// invoke nothing dynamically, which is what rule B5 requires of this root.
/// </para>
/// </remarks>
internal static class BigIntChecks
{
    /// <summary>The identity these checks present to the verifier.</summary>
    private const string Caller = "js-slice-compiler://bigint";

    /// <summary>The embedder-seam code of a construct the manifest does not admit.</summary>
    private const int ConstructOutsideManifest = 2104;

    /// <summary>The embedder-seam code of a malformed numeric literal.</summary>
    private const int MalformedNumericLiteral = 2003;

    /// <summary>The gated programs: a title, the program, and the one answer it must give.</summary>
    private static readonly (string Name, string Source, string Expected)[] Programs =
    [
        (
            "bigint/b02/a-value-past-two-to-the-53-survives-compile-verify-execute",
            "`${9007199254740993n}|${18446744073709551617n}`;",
            "9007199254740993|18446744073709551617"),
        (
            "bigint/b02/every-radix-and-separators-are-exact",
            "[0x1fffffffffffffffn, 0o7777777777777777777n, " +
            "0b1111111111111111111111111111111111111111111111111111111n, " +
            "1_000_000_000_000_000_000_001n, 0XABn, 0O17n, 0B101n, 0n, 0x0n].join(',');",
            "2305843009213693951,144115188075855871,36028797018963967,1000000000000000000001,171,15,5,0,0"),
        (
            "bigint/b02/a-two-hundred-bit-literal-is-exact",
            "String(0x100000000000000000000000000000000000000000000000000n);",
            "1606938044258990275541962092341162602522202993782792835301376"),
        (
            "bigint/b02/the-widest-constant-the-format-carries-survives",
            "String(0x" + new string('f', 16384) + "n).length;",
            "19729"),
        (
            "bigint/b01/typeof-and-truthiness",
            "[typeof 1n, typeof 0n, 0n ? 't' : 'f', 1n ? 't' : 'f', !0n, !!0x0n, !!-0 === !!0n].join(',');",
            "bigint,bigint,f,t,true,false,true"),
        (
            "bigint/b01/strict-equality-is-mathematical",
            "[1n === 1n, 0x10n === 16n, 9007199254740993n === 9007199254740992n, 1n === 1, 1n !== 1, " +
            "1n == 1n, 1n == null, undefined == 0n, Object.is(1n, 1n), Object.is(0n, 0)].join(',');",
            "true,true,false,false,true,true,false,false,true,false"),
        (
            "bigint/b01/same-value-zero-keys-are-mathematical",
            "var m = new Map([[2n, 'two']]); " +
            "[[1n].includes(1n), [1n].indexOf(1n), new Set([1n, 1n, 0x1n, 1]).size, m.get(0x2n), " +
            "m.has(2)].join(',');",
            "true,0,2,two,false"),
        (
            "bigint/b02/a-bigint-property-key-is-its-exact-decimal-spelling",
            "var o = { 9007199254740993n: 'k', 0x10n: 'h' }; o[1n] = 'one'; " +
            "Object.keys(o).join('|') + ':' + o['9007199254740993'] + o[16] + o['1'];",
            "1|16|9007199254740993:khone"),
        (
            "bigint/b01/string-conversion-is-exact-everywhere-it-is-reached",
            "[String(12345678901234567890n), '' + 12345678901234567890n, [0n, 7n].join('+'), " +
            "`${0x7fn}`].join(',');",
            "12345678901234567890,12345678901234567890,0+7,127"),
        (
            "bigint/b05/the-operations-b01-refused-by-name-answer-as-the-language-does",
            Classify + "[k(() => 1n < 2n), k(() => 1n == 1), k(() => Number(1n)), " +
            "k(() => JSON.stringify({ a: 1n })), k(() => (1n).toString()), k(() => typeof Object(1n)), " +
            "k(() => Math.max(1n))].join(',');",
            "true,true,1,TypeError,1,object,TypeError"),

        // ---- B03: arithmetic ----------------------------------------------------------------
        (
            "bigint/b03/addition-and-subtraction-are-exact-and-signed",
            "[9007199254740993n + 2n, 18446744073709551615n + 1n, -5n + 3n, 5n - 8n, " +
            "-18446744073709551616n - 1n, 0n - 0n, 0x7fffffffffffffffn + 0x7fffffffffffffffn].join(',');",
            "9007199254740995,18446744073709551616,-2,-3,-18446744073709551617,0,18446744073709551614"),
        (
            "bigint/b03/multiplication-is-exact-and-signed",
            "[123456789012345678901234567890n * 987654321098765432109876543210n, -3n * 4n, -3n * -4n, " +
            "0n * -5n, 4294967296n * 4294967296n].join(',');",
            "121932631137021795226185032733622923332237463801111263526900,-12,12,0,18446744073709551616"),
        (
            "bigint/b03/division-and-remainder-truncate-toward-zero",
            "[7n / 2n, -7n / 2n, 7n / -2n, -7n / -2n, 7n % 2n, -7n % 2n, 7n % -2n, -7n % -2n, 1n / 3n, " +
            "-1n / 3n, 1000000000000000000000000000000n / 1000000000000000n, " +
            "18446744073709551617n % 4294967296n].join(',');",
            "3,-3,-3,3,1,-1,1,-1,0,0,1000000000000000,1"),
        (
            "bigint/b03/exponentiation-is-exact-including-huge-exponents-of-trivial-bases",
            "[2n ** 100n, (-3n) ** 3n, (-2n) ** 64n, 0n ** 0n, 5n ** 0n, " +
            "1n ** 1000000000000000000000000000000n, (-1n) ** 1000000000000000000000000000001n, " +
            "0n ** 1000000000000000000000000000000n, 10n ** 30n].join(',');",
            "1267650600228229401496703205376,-27,18446744073709551616,1,1,1,-1,0,1000000000000000000000000000000"),
        (
            "bigint/b03/zero-divisors-negative-exponents-and-mixed-operands-are-the-specified-errors",
            Classify + "[k(() => 1n / 0n), k(() => 1n % 0n), k(() => 0n / 0n), k(() => 2n ** -1n), " +
            "k(() => 0n ** -1n), k(() => 1n + 1), k(() => 1 - 1n), k(() => 1n * 1), k(() => 1n / 1), " +
            "k(() => 1n % 1), k(() => 1n ** 1), k(() => 2 ** 1n), k(() => +1n), k(() => 1n + undefined), " +
            "k(() => 1n - null), k(() => 1n * true), k(() => 1n / '1')].join(',');",
            "RangeError,RangeError,RangeError,RangeError,RangeError,TypeError,TypeError,TypeError,TypeError," +
            "TypeError,TypeError,TypeError,TypeError,TypeError,TypeError,TypeError,TypeError"),
        (
            "bigint/b03/increment-and-decrement-keep-a-bigint-a-bigint-on-every-reference-kind",
            "var a = 5n; var b = a++; var c = ++a; var d = a--; var e = --a; " +
            "var o = { p: 9007199254740993n }; var p1 = o.p++; ++o.p; var arr = [0n]; arr[0]--; " +
            "var key = 'q', h = { q: -1n }; h[key]++; " +
            "[a, b, c, d, e, o.p, p1, arr[0], h.q, typeof b].join(',');",
            "5,5,7,7,5,9007199254740995,9007199254740993,-1,0,bigint"),
        (
            "bigint/b03/an-update-converts-its-operand-once-with-to-numeric",
            "var v = { valueOf() { return 10n; } }; var old = v++; var w = { valueOf() { return 7n; } }; --w; " +
            "class C { #x = 1n; static s = 4n; inc() { return this.#x++ + this.#x; } " +
            "static bump() { return ++C.s; } } " +
            "class D extends C { static up() { return super.s++; } } " +
            "[typeof old, old, v, w, new C().inc(), C.bump(), C.s, D.up(), C.s, D.s].join(',');",
            "bigint,10,11,6,3,5,5,5,5,6"),
        (
            "bigint/b03/unary-minus-is-exact-and-zero-has-one-sign",
            "var a = 5n; [-a, -(-a), -0n, typeof -0n, -(2n ** 64n), -(-9007199254740993n)].join(',');",
            "-5,5,0,bigint,-18446744073709551616,9007199254740993"),
        (
            "bigint/b03/number-arithmetic-and-updates-are-unchanged-under-the-gate",
            "var n = -0; var n0 = n++; var m = '5'; var m0 = m++; var q = { valueOf() { return 2; } }; q--; " +
            "var f = 1.5; f++; var u; u++; " +
            "[Object.is(n0, -0), n, m0, typeof m0, m, q, f, u, 1 + 2, '1' - 1, 2 ** 10, 7 % -2, -7 / 2].join(',');",
            "true,1,5,number,6,1,2.5,NaN,3,0,1024,1,-3.5"),
        (
            "bigint/b03/a-string-operand-still-concatenates",
            "[1n + 'x', 'a' + 2n, `${-3n}` + 4n, 10n + '' + 5n, [1n + 2n] + ''].join(',');",
            "1x,a2,-34,105,3"),
        (
            "bigint/b03/computed-bigints-are-equal-to-constants-by-value-not-identity",
            "[(1n + 1n) === 2n, 2n ** 64n === 18446744073709551616n, " +
            "new Set([2n, 1n + 1n, 4n / 2n, 3n - 1n]).size, new Map([[3n, 't']]).get(1n + 2n), " +
            "[6n].includes(2n * 3n), [6n].indexOf(2n * 3n), Object.is(0n, -0n), Object.is(5n - 5n, 0n), " +
            "(1n + 1n) !== 2n, (2n ** 70n) / (2n ** 6n) === 2n ** 64n].join(',');",
            "true,true,1,t,true,0,true,true,false,true"),
        (
            "bigint/b03/compound-assignment-is-exact",
            "var x = 10n; x += 5n; x -= 1n; x *= 3n; var s1 = x; x /= 4n; x %= 7n; x **= 3n; " +
            "var y = -7n; y /= 2n; var z = -7n; z %= 3n; [s1, x, y, z].join(',');",
            "42,27,-3,-1"),
        (
            "bigint/b03/results-wider-than-the-ceiling-are-range-errors",
            Classify + "var t = 1n << 1048575n; " +
            "[k(() => 2n ** 1048576n), k(() => t * 2n), k(() => t + t), " +
            "k(() => 2n ** 10000000000000000000000n), k(() => typeof (-t - t)), k(() => -(-t - t)), " +
            "k(() => 3n ** 700000n), k(() => typeof (3n ** 600000n)), k(() => (-2n) ** 1048575n === -t), " +
            "k(() => t * t)].join(',');",
            "RangeError,RangeError,RangeError,RangeError,bigint,RangeError,RangeError,bigint,true,RangeError"),

        // ---- B04: bitwise and shift operations -----------------------------------------------
        (
            "bigint/b04/bitwise-operations-are-twos-complement-at-any-width",
            "[5n & 3n, 5n | 3n, 5n ^ 3n, -5n & 3n, -5n | 3n, -5n ^ 3n, -5n & -3n, -5n | -3n, -5n ^ -3n, " +
            "~5n, ~-5n, ~0n, ~-1n, (2n ** 64n - 1n) & -(2n ** 32n), (2n ** 70n) | 1n, " +
            "-(2n ** 70n) ^ -1n].join(',');",
            "1,7,6,3,-5,-8,-7,-1,6,-6,4,-1,0,18446744069414584320,1180591620717411303425,1180591620717411303423"),
        (
            "bigint/b04/shifts-sign-extend-and-a-negative-count-reverses-the-direction",
            "[1n << 64n, -1n << 64n, 5n << 0n, 5n << -1n, 5n >> 1n, -5n >> 1n, -5n >> 100n, 5n >> 100n, " +
            "5n >> -2n, -1n >> 1n, (2n ** 100n) >> 99n, -(2n ** 100n) >> 99n, -(2n ** 100n + 1n) >> 99n, " +
            "0n << 1000000000000n, 7n >> 1000000000000000000000n, -7n >> 1000000000000000000000n, " +
            "7n << -1000000000000000000000n].join(',');",
            "18446744073709551616,-18446744073709551616,5,2,2,-3,-1,0,20,-1,2,-2,-3,0,0,-1,0"),
        (
            "bigint/b04/unsigned-shift-and-mixed-operands-are-type-errors",
            Classify + "[k(() => 1n >>> 0n), k(() => -1n >>> 1n), k(() => 1n >>> 1), k(() => 1n & 1), " +
            "k(() => 1 | 1n), k(() => 1n ^ 1), k(() => 1n << 1), k(() => 1 >> 1n), " +
            "k(() => ~{ valueOf() { return 2n; } }), k(() => { var a = 1n; a >>>= 1n; return a; }), " +
            "k(() => { var a = 1n; a <<= 3n; a |= 16n; a &= 24n; a ^= 1n; a >>= 1n; return a; })].join(',');",
            "TypeError,TypeError,TypeError,TypeError,TypeError,TypeError,TypeError,TypeError,-3,TypeError,12"),
        (
            "bigint/b04/number-bitwise-operations-are-unchanged-under-the-gate",
            "[5 & 3, -5 | 3, 5 ^ 3, ~5, 1 << 33, -5 >> 1, -5 >>> 28, '3' << 1, " +
            "({ valueOf() { return 6; } }) & 3].join(',');",
            "1,-5,6,-6,2,-3,15,6,2"),
        (
            "bigint/b04/an-oversized-shift-is-refused-before-it-allocates",
            Classify + "[k(() => 1n << 1048576n), k(() => 1n << 10000000000000000000000n), " +
            "k(() => -1n << 1048575n === -(2n ** 1048575n)), k(() => 0n << 10000000000000000000000n), " +
            "k(() => (1n << 1048575n) >> 1048575n)].join(',');",
            "RangeError,RangeError,true,0,1"),

        // ---- B05: conversion, comparison and the public surface ---------------------------
        (
            "bigint/b05/the-bigint-function-converts-parses-and-refuses-as-specified",
            Classify + "[BigInt(0), BigInt(-0), BigInt(2 ** 60), BigInt(-1e21), BigInt(true), BigInt('0x10'), BigInt(' -7 '), BigInt('0b11'), BigInt(''), k(() => BigInt(1.5)), k(() => BigInt(NaN)), k(() => BigInt('1n')), k(() => BigInt('1.0')), k(() => BigInt('-0x1')), k(() => BigInt(undefined)), k(() => BigInt(null)), k(() => BigInt(Symbol())), k(() => new BigInt(1)), k(() => BigInt({ valueOf() { return 3n; } })), k(() => BigInt('9'.repeat(40)))].join(',');",
            "0,0,1152921504606846976,-1000000000000000000000,1,16,-7,3,0,RangeError,RangeError,SyntaxError,SyntaxError,SyntaxError,TypeError,TypeError,TypeError,TypeError,3,9999999999999999999999999999999999999999"),
        (
            "bigint/b05/as-int-n-and-as-uint-n-take-to-index-and-to-bigint",
            Classify + "[BigInt.asIntN(8, 255n), BigInt.asUintN(8, -1n), BigInt.asIntN('64', 2n ** 63n), BigInt.asUintN(undefined, 5n), k(() => BigInt.asIntN(-1, 1n)), k(() => BigInt.asUintN(2 ** 53, 1n)), k(() => BigInt.asIntN(8, 1)), BigInt.asUintN(8, '0x1ff'), k(() => BigInt.asUintN(2 ** 53 - 1, -1n)), BigInt.asIntN(2 ** 53 - 1, -3n)].join(',');",
            "-1,255,-9223372036854775808,0,RangeError,RangeError,TypeError,255,RangeError,-3"),
        (
            "bigint/b05/to-string-is-exact-in-every-radix-and-checks-its-receiver",
            Classify + "var w = 2n ** 200n - 1n; [w.toString(36), (-w).toString(2).length, w.toString(7), (255n).toString(16), (0n).toString(3), (10n).toString(undefined), k(() => (1n).toString(1)), k(() => (1n).toString(37)), k(() => BigInt.prototype.toString.call(1)), (5n).toLocaleString()].join(',');",
            "bnklg118comha6gqury14067gur54n8won6guf3,201,141246066533632643213232344050606053061443446006544361632102630555343053,ff,0,10,RangeError,RangeError,TypeError,5"),
        (
            "bigint/b05/loose-equality-with-numbers-strings-and-objects-is-exact",
            "[1n == 1, 1 == 1n, 1n == 1.5, 9007199254740993n == 9007199254740992, 2n ** 53n == 2 ** 53, 1n == NaN, 1n == Infinity, 1n == '1', '0x10' == 16n, 1n == '1.0', 0n == '', 1n == 'x', 1n == true, 0n == false, 1n == null, 1n == Object(1n), 1n == { valueOf() { return 1; } }, 2n ** 64n == 18446744073709551616].join(',');",
            "true,true,false,false,true,false,false,true,true,false,true,false,true,true,false,true,true,true"),
        (
            "bigint/b05/relational-comparison-with-numbers-and-strings-is-exact",
            Classify + "[1n < 1.5, 2n > 1.5, 9007199254740993n > 9007199254740992, 9007199254740992 < 9007199254740993n, -1n < -0.5, 0n <= -0, 1n < Infinity, 1n > -Infinity, 1n < NaN, 1n >= NaN, 2n ** 1100n > 1.7976931348623157e308, 1n < '2', '10' > 9n, 1n < 'x', 1n >= 'x', '9007199254740993' > 9007199254740992n, 1n < true, k(() => 1n < Symbol()), -(2n ** 1100n) < -1.7976931348623157e308].join(',');",
            "true,true,true,true,true,true,true,true,false,false,true,true,true,false,false,true,false,TypeError,true"),
        (
            "bigint/b05/wrappers-reach-the-prototype-and-unwrap-exactly",
            "var w = Object(7n); BigInt.prototype.twice = function () { return this * 2n; }; [typeof w, w instanceof BigInt, w + 1n, w == 7n, w === 7n, (21n).twice(), Object.prototype.toString.call(1n), BigInt.prototype[Symbol.toStringTag], BigInt.prototype.valueOf.call(w), typeof BigInt.prototype.valueOf.call(w), BigInt.length, BigInt.name].join(',');",
            "object,true,8,true,false,42,[object BigInt],BigInt,7,bigint,1,BigInt"),
        (
            "bigint/b05/json-refuses-a-bigint-after-consulting-to-json",
            Classify + "var a = k(() => JSON.stringify({ a: 1n })); var b = k(() => JSON.stringify([Object(2n)])); BigInt.prototype.toJSON = function () { return this + 'n'; }; [a, b, JSON.stringify({ a: 1n, b: [Object(2n)] })].join('|');",
            "TypeError|TypeError|{\"a\":\"1n\",\"b\":[\"2n\"]}"),
        (
            "bigint/b05/number-of-a-bigint-rounds-to-nearest-even-and-to-number-refuses",
            Classify + "[Number(2n ** 53n + 1n), Number(2n ** 53n + 3n), Number(-(2n ** 53n + 1n)), Number(2n ** 1024n), Number(2n ** 1024n - 2n ** 970n), Number(2n ** 1024n - 2n ** 970n - 1n) === Number.MAX_VALUE, Number(123456789012345678901234567890n), Number(0n), Object.is(Number(-0n), 0), k(() => +1n), k(() => Math.abs(1n)), k(() => isNaN(1n)), parseInt(12n)].join(',');",
            "9007199254740992,9007199254740996,-9007199254740992,Infinity,Infinity,true,1.2345678901234568e+29,0,true,TypeError,TypeError,TypeError,12"),
    ];

    /// <summary>A classifier the error rows share: a completion, or the constructor of the throw.</summary>
    private const string Classify =
        "function k(f) { try { return String(f()); } catch (e) { return e instanceof TypeError ? 'TypeError' : " +
        "e instanceof RangeError ? 'RangeError' : e instanceof SyntaxError ? 'SyntaxError' : 'other:' + e; } } ";

    /// <summary>
    /// Literals every compilation refuses as malformed, with the column the refusal must name.
    /// </summary>
    private static readonly (string Literal, string Why)[] Malformed =
    [
        ("1.5n", "a fraction"),
        ("1e3n", "an exponent"),
        ("01n", "a legacy octal"),
        ("08n", "a leading zero"),
        (".5n", "a leading point"),
        ("1_n", "a trailing separator"),
        ("0x_1n", "a leading separator"),
        ("1nn", "a second suffix"),
        ("1n0", "a digit after the suffix"),
        ("0b2n", "a digit outside the radix"),
    ];

    /// <summary>Runs every BigInt check.</summary>
    internal static System.Collections.Generic.List<(string Name, bool Passed, string Detail)> Run()
    {
        var results = new System.Collections.Generic.List<(string Name, bool Passed, string Detail)>();

        foreach (var (name, source, expected) in Programs)
        {
            results.Add(Judge(name, source, expected));
        }

        foreach (var (literal, why) in Malformed)
        {
            results.Add(MalformedLiteralIsRefusedAtItsPosition(literal, why, JsFeatureManifest.Wide));
            results.Add(MalformedLiteralIsRefusedAtItsPosition(literal, why, JsFeatureManifest.Numeric));
        }

        results.Add(TheNumericManifestKeepsTheNamedRefusal());
        results.Add(AnOversizedLiteralIsRefusedAtItsPosition("1" + new string('0', 19729), "a decimal"));
        results.Add(AnOversizedLiteralIsRefusedAtItsPosition("0x1" + new string('0', 16384), "a hexadecimal"));
        results.Add(ACompositionDecliningTheSurfaceRefusesEveryWayIn());
        results.Add(ADecliningCompositionRefusesBigIntThroughEval());
        results.Add(EveryPublicDescriptorDoorAdmitsTheSurfaceWhenNamed());
        results.Add(AnUpdateExpressionDeclaresNothingAndStepsANumber());
        results.Add(AThrownBigIntReachesTheHostAsAHostThrow());
        results.Add(ADecliningRealmRefusesAHostBigInt());
        results.Add(AHostCrossingIsChargedPerWord());
        results.Add(ABigIntPropertyKeyDeclaresNothing());
        results.Add(ALongDecimalTextIsChargedForItsSquare());
        results.Add(AWideMultiplicationIsChargedForItsSize());
        results.Add(FormattingTheWidestValueIsChargedForItsSquare());
        results.Add(CancellationStopsALongFormatting());
        results.Add(AsIntNAndAsUintNWrapAtTheirWidth());
        results.Add(TheBigIntTypedArraysNeedBothSurfaces());
        results.Add(AWideElementWriteIsChargedForItsNarrowing());
        return results;
    }

    /// <summary>
    /// A multiplication is charged for the product of its operands' sizes: under one small fuel
    /// ceiling a loop over narrow operands completes and one wide multiplication does not.
    /// </summary>
    private static (string, bool, string) AWideMultiplicationIsChargedForItsSize()
    {
        const string Name = "bigint/b03/a-wide-multiplication-is-charged-for-its-size";
        const ulong Fuel = 1_000_000;

        var narrow = RunWide("var a = 1n << 63n; for (var i = 0; i < 200; i++) a * a; 'done';", Fuel);
        var wide = RunWide("var a = 1n << 262143n; a * a; 'done';", Fuel);

        return (
            Name,
            narrow == "done" && wide == "ResourceExhaustion/AllowanceExhausted",
            $"narrow answered {narrow}; wide answered {wide}");
    }

    /// <summary>
    /// Converting a BigInt to text is charged for the square of its width, not only for its digits:
    /// the widest value's text costs more than a fuel ceiling its digit count fits under.
    /// </summary>
    private static (string, bool, string) FormattingTheWidestValueIsChargedForItsSquare()
    {
        const string Name = "bigint/b03/formatting-the-widest-value-is-charged-for-its-square";
        const ulong Fuel = 5_000_000;

        var widest = RunWide("String(1n << 1048575n).length;", Fuel);
        var unbounded = RunWide("String(1n << 1048575n).length;", fuel: null);

        return (
            Name,
            widest == "ResourceExhaustion/AllowanceExhausted" && unbounded == "315653",
            $"under {Fuel} fuel it answered {widest}; under the default allowance {unbounded}");
    }

    /// <summary>
    /// Cancellation requested while the widest value is being formatted is honoured promptly: the
    /// request arrives 40 ms into the run and the run must end, cancelled, within one second.
    /// </summary>
    /// <remarks>
    /// The base class library's own conversion of this value is one call of about 3.2 s here, which
    /// nothing can interrupt, so a conversion made that way is seen cancelled only after it - past
    /// the second. The divide-and-conquer conversion charges between steps of at most about 52 ms.
    /// </remarks>
    private static (string, bool, string) CancellationStopsALongFormatting()
    {
        const string Name = "bigint/b03/cancellation-stops-a-long-formatting";
        var clock = System.Diagnostics.Stopwatch.StartNew();
        var answer = RunWide(
            "var x = 1n << 1048575n; String(x).length + '|finished';",
            fuel: 1_000_000_000_000,
            cancelAfter: System.TimeSpan.FromMilliseconds(40));

        var elapsed = clock.ElapsedMilliseconds;

        return (
            Name,
            answer == "Cancellation/Cancelled" && elapsed < 1000,
            $"the run answered {answer} after {elapsed} ms");
    }

    /// <summary>
    /// The internal <c>BigInt.asIntN</c> and <c>BigInt.asUintN</c> operations wrap at their width
    /// with the specified sign; the <c>BigInt</c> global exposes them since card B05, and the
    /// <c>bigint/b05/as-int-n-...</c> row asks them through it.
    /// </summary>
    private static (string, bool, string) AsIntNAndAsUintNWrapAtTheirWidth()
    {
        const string Name = "bigint/b04/as-int-n-and-as-uint-n-wrap-at-their-width";
        var one = System.Numerics.BigInteger.One;
        var two63 = one << 63;
        var two64 = one << 64;
        const ulong Safe = (1UL << 53) - 1;

        (ulong Bits, System.Numerics.BigInteger Value)[] signed =
        [
            (64, two63), (64, two64 - 1), (0, 5), (1, 1), (1, -1), (3, 25), (3, -25), (200, -5), (Safe, 5),
            (64, -two63 - 1),
        ];

        (ulong Bits, System.Numerics.BigInteger Value)[] unsigned =
        [
            (64, -1), (0, 5), (3, -25), (65, two64), (64, two64), (Safe, 5), (8, -129), (128, -two64),
            (Safe, -1), (1_048_577, -1),
        ];

        var charged = 0UL;
        var answers = new System.Collections.Generic.List<string>();

        foreach (var (bits, value) in signed)
        {
            answers.Add(Show(AsIntN(null, bits, NewBigInt(value), units => charged += units)));
        }

        answers.Add("|");

        foreach (var (bits, value) in unsigned)
        {
            answers.Add(Show(AsUintN(null, bits, NewBigInt(value), units => charged += units)));
        }

        var widest = AsUintN(null, 1_048_576, NewBigInt(-1), units => charged += units);
        answers.Add(widest is null
            ? "refused"
            : ValueOf(widest).ToString(System.Globalization.CultureInfo.InvariantCulture).Length
                .ToString(System.Globalization.CultureInfo.InvariantCulture));

        var answer = string.Join(",", answers);
        const string Expected =
            "-9223372036854775808,-1,0,-1,-1,1,-1,-5,5,9223372036854775807,|," +
            "18446744073709551615,0,7,18446744073709551616,0,5,127,340282366920938463444927863358058659840," +
            "refused,refused,315653";

        return (
            Name,
            answer == Expected && charged > 0,
            answer == Expected ? $"every width answered as specified, charging {charged}" : "answered " + answer);

        static string Show(object? result) =>
            result is null
                ? "refused"
                : ValueOf(result).ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Compiles one program under the wide manifest and runs it under the descriptor admitting every
    /// surface with a fuel ceiling and an optional cancellation, answering its completion or the
    /// outcome that ended it.
    /// </summary>
    private static string RunWide(string source, ulong? fuel, System.TimeSpan? cancelAfter = null)
    {
        var compiled = Compile(source, new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return "refused: " + First(compiled);
        }

        TryRun(compiled.Artifact, JavaScriptProfile.Descriptor, out var answer, fuel: fuel, cancelAfter: cancelAfter);
        return answer;
    }

    /// <summary>
    /// Compiles one program under the wide manifest, runs it under the descriptor admitting every
    /// surface, and compares its answer.
    /// </summary>
    private static (string, bool, string) Judge(string name, string source, string expected)
    {
        var compiled = Compile(source, new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return (name, false, "the source was refused: " + First(compiled));
        }

        if (!TryRun(compiled.Artifact, JavaScriptProfile.Descriptor, out var answer))
        {
            return (name, false, answer);
        }

        var passed = string.Equals(answer, expected, System.StringComparison.Ordinal);
        return (
            name,
            passed,
            passed
                ? "the program answered " + (answer.Length > 80 ? answer[..80] + "..." : answer)
                : $"the program answered '{answer}' and '{expected}' was expected");
    }

    /// <summary>
    /// A malformed literal is a malformed-literal refusal at the literal, under the manifest that
    /// admits BigInt and under the one that does not.
    /// </summary>
    private static (string, bool, string) MalformedLiteralIsRefusedAtItsPosition(
        string literal, string why, JsFeatureManifest manifest)
    {
        var admits = manifest == JsFeatureManifest.Wide;
        var name = $"bigint/b02/{(admits ? "wide" : "numeric")}/{why.Replace(' ', '-')}-is-malformed: {literal}";
        var compiled = Compile("var x = 1;\nvar a = " + literal + ";", new JsCompileRequest(manifest));

        if (compiled.Succeeded || compiled.Diagnostics.Count == 0)
        {
            return (name, false, "the literal was admitted");
        }

        var first = compiled.Diagnostics[0];
        var passed = (int)first.Code == MalformedNumericLiteral && first.Line == 2 && first.Column == 9;
        return (name, passed, first.ToString());
    }

    /// <summary>
    /// The numeric manifest is Number values only: a well-formed literal meets the refusal it always
    /// met, by name and at the literal.
    /// </summary>
    private static (string, bool, string) TheNumericManifestKeepsTheNamedRefusal()
    {
        const string Name = "bigint/b02/the-numeric-manifest-keeps-the-named-refusal";
        var compiled = Compile(
            "var x = 1;\nvar a = 9007199254740993n;", new JsCompileRequest(JsFeatureManifest.Numeric));

        if (compiled.Succeeded || compiled.Diagnostics.Count == 0)
        {
            return (Name, false, "the numeric manifest admitted a BigInt literal");
        }

        var first = compiled.Diagnostics[0];
        var passed = (int)first.Code == ConstructOutsideManifest && first.Line == 2 && first.Column == 9 &&
            first.Message.StartsWith("a BigInt literal is not admitted", System.StringComparison.Ordinal);

        return (Name, passed, first.ToString());
    }

    /// <summary>A literal wider than the constant ceiling is refused at the literal, not written.</summary>
    private static (string, bool, string) AnOversizedLiteralIsRefusedAtItsPosition(string digits, string radix)
    {
        var name = $"bigint/b02/{radix.Replace(' ', '-')}-literal-wider-than-the-ceiling-is-refused";
        var compiled = Compile("var a = " + digits + "n;", new JsCompileRequest());

        if (compiled.Succeeded || compiled.Diagnostics.Count == 0)
        {
            return (name, false, "the literal was admitted");
        }

        var first = compiled.Diagnostics[0];
        var passed = (int)first.Code == ConstructOutsideManifest && first.Line == 1 && first.Column == 9 &&
            first.Message.Contains("wider than 65536 bits", System.StringComparison.Ordinal);

        return (name, passed, first.ToString());
    }

    /// <summary>
    /// A composition that declines the BigInt surface refuses, at verification and as a surface it
    /// declined, an artifact holding a BigInt literal and one naming the <c>BigInt</c> global; a
    /// program asking <c>typeof BigInt</c> declares nothing and is told <c>"undefined"</c>.
    /// </summary>
    private static (string, bool, string) ACompositionDecliningTheSurfaceRefusesEveryWayIn()
    {
        const string Name = "bigint/b05/a-composition-declining-the-surface-refuses-every-way-in";
        var declining = JavaScriptProfile.DescriptorAdmitting(
            JavaScriptProfile.BinaryManifest, JavaScriptProfile.DynamicManifest);

        var answers = string.Join(
            ",",
            Verified("1n;", declining),
            Verified("BigInt('1');", declining),
            Verified("1n;", JavaScriptProfile.Descriptor),
            Verified("BigInt('1');", JavaScriptProfile.Descriptor),
            Answered("typeof BigInt;", declining),
            Answered("typeof BigInt;", JavaScriptProfile.Descriptor));

        return (
            Name,
            answers == "1608,1608,admitted,admitted,undefined,function",
            "the compositions answered " + answers);

        static string Verified(string source, VmProfileDescriptor profile)
        {
            var compiled = Compile(source, new JsCompileRequest());

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                return "refused-at-compile";
            }

            using var runtime = Runtime(profile);

            if (runtime is null)
            {
                return "no-runtime";
            }

            var descriptor = Descriptor();
            var outcome = runtime.Verify(in descriptor, compiled.Artifact, System.Threading.CancellationToken.None);

            if (outcome.TryGetArtifact(out var handle))
            {
                handle.Dispose();
                return "admitted";
            }

            return outcome.Outcome == VmOutcome.InvalidArtifact &&
                outcome.Reason == VmReason.UnsupportedFeatureManifest
                    ? outcome.Diagnostics.ProfileDiagnosticCode.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    : $"{outcome.Outcome}/{outcome.Reason}";
        }

        static string Answered(string source, VmProfileDescriptor profile)
        {
            var compiled = Compile(source, new JsCompileRequest());

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                return "refused-at-compile";
            }

            TryRun(compiled.Artifact, profile, out var answer);
            return answer;
        }
    }

    /// <summary>
    /// Storing a BigInt wider than 64 bits into a BigInt typed-array element narrows it once and
    /// is charged for its words (JSeal B07 review, 2026-09-22): under one fuel ceiling two hundred
    /// stores of a 64-bit value complete and two hundred stores of a 2**20-bit value do not.
    /// </summary>
    private static (string, bool, string) AWideElementWriteIsChargedForItsNarrowing()
    {
        const string Name = "bigint/b07/a-wide-element-write-is-charged-for-its-narrowing";
        const ulong Fuel = 1_000_000;

        var narrow = RunWide(
            "var t = new BigInt64Array(1); var w = 1n << 63n; for (var i = 0; i < 200; i++) t[0] = w; 'done';",
            Fuel);
        var wide = RunWide(
            "var t = new BigInt64Array(1); var w = 1n << 1048575n; for (var i = 0; i < 200; i++) t[0] = w; 'done';",
            Fuel);

        return (
            Name,
            narrow == "done" && wide == "ResourceExhaustion/AllowanceExhausted",
            $"narrow answered {narrow}; wide answered {wide}");
    }

    /// <summary>
    /// <c>BigInt64Array</c> and <c>BigUint64Array</c> belong to two surfaces (JSeal B07): naming
    /// either is refused at verification by a composition that declines BigInt, and by one that
    /// declines the binary surface; a realm declining BigInt builds neither constructor nor the
    /// <c>DataView</c> BigInt accessors, and keeps every Number kind.
    /// </summary>
    private static (string, bool, string) TheBigIntTypedArraysNeedBothSurfaces()
    {
        const string Name = "bigint/b07/the-bigint-typed-arrays-need-both-surfaces";
        var noBigInt = JavaScriptProfile.DescriptorAdmitting(
            JavaScriptProfile.BinaryManifest, JavaScriptProfile.DynamicManifest);
        var noBinary = JavaScriptProfile.DescriptorAdmitting(JavaScriptProfile.BigIntManifest);
        const string Shape =
            "[typeof BigInt64Array, typeof BigUint64Array, typeof DataView.prototype.getBigInt64, " +
            "typeof DataView.prototype.setBigUint64, typeof Float64Array].join();";

        var answers = string.Join(
            "|",
            Verified("BigInt64Array;", noBigInt),
            Verified("new BigUint64Array(1).length;", noBigInt),
            Verified("BigInt64Array;", noBinary),
            Verified("BigUint64Array;", JavaScriptProfile.Descriptor),
            Verified("new Float64Array(1).length;", noBigInt),
            Answered(Shape, noBigInt),
            Answered(Shape, JavaScriptProfile.Descriptor),
            Answered("String(new BigUint64Array([-1n])[0]);", JavaScriptProfile.Descriptor));

        return (
            Name,
            answers == "1608|1608|1608|admitted|admitted|undefined,undefined,undefined,undefined,function|" +
                "function,function,function,function,function|18446744073709551615",
            "the compositions answered " + answers);

        static string Verified(string source, VmProfileDescriptor profile)
        {
            var compiled = Compile(source, new JsCompileRequest());

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                return "refused-at-compile";
            }

            using var runtime = Runtime(profile);

            if (runtime is null)
            {
                return "no-runtime";
            }

            var descriptor = Descriptor();
            var outcome = runtime.Verify(in descriptor, compiled.Artifact, System.Threading.CancellationToken.None);

            if (outcome.TryGetArtifact(out var handle))
            {
                handle.Dispose();
                return "admitted";
            }

            return outcome.Outcome == VmOutcome.InvalidArtifact &&
                outcome.Reason == VmReason.UnsupportedFeatureManifest
                    ? outcome.Diagnostics.ProfileDiagnosticCode.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    : $"{outcome.Outcome}/{outcome.Reason}";
        }

        static string Answered(string source, VmProfileDescriptor profile)
        {
            var compiled = Compile(source, new JsCompileRequest());

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                return "refused-at-compile";
            }

            TryRun(compiled.Artifact, profile, out var answer);
            return answer;
        }
    }

    /// <summary>
    /// A composition that admits the dynamic surface and declines BigInt refuses a BigInt reached
    /// through <c>eval</c> or the <c>Function</c> constructor as a catchable <c>EvalError</c> naming
    /// the unsupported feature manifest, never a value; the same program under the descriptor
    /// admitting every surface answers the values.
    /// </summary>
    /// <remarks>
    /// The guest-loaded source compiles under the wide manifest, so it is the mediator's verification
    /// of the loaded artifact against the composition's descriptor that refuses it, and the guest
    /// meets it as the <c>EvalError</c> every unanswered guest load is (JSD-0024, JSD-0026). The row
    /// pins what the guest sees of that refusal.
    /// </remarks>
    private static (string, bool, string) ADecliningCompositionRefusesBigIntThroughEval()
    {
        const string Name = "bigint/b05/a-declining-composition-refuses-bigint-through-eval";
        var compiled = Compile(
            "var r = [];" +
            "function probe(f) { try { var v = f(); r.push(typeof v + ':' + String(v)); }" +
            " catch (e) { r.push('threw:' + e.name + (/UnsupportedFeatureManifest$/.test(e.message) ? ':manifest' : ':' + e.message)); } }" +
            "probe(function () { return eval('1n'); });" +
            "probe(function () { return eval('BigInt(1)'); });" +
            "probe(function () { return (0, eval)('2n'); });" +
            "probe(function () { return Function('return 3n')(); });" +
            "r.join(',');",
            new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return (Name, false, "the source was refused: " + First(compiled));
        }

        var declining = JavaScriptProfile.DescriptorAdmitting(
            JavaScriptProfile.BinaryManifest, JavaScriptProfile.DynamicManifest);

        TryRun(compiled.Artifact, declining, out var refused, provider: new EvalProvider());
        TryRun(compiled.Artifact, JavaScriptProfile.Descriptor, out var admitted, provider: new EvalProvider());

        return (
            Name,
            refused == "threw:EvalError:manifest,threw:EvalError:manifest,threw:EvalError:manifest,threw:EvalError:manifest" &&
                admitted == "bigint:1,bigint:1,bigint:2,bigint:3",
            $"declining answered {refused}; admitting answered {admitted}");
    }

    /// <summary>
    /// Every public descriptor door admits the BigInt surface when a composition names it, where
    /// until card B05 each refused the name with an <see cref="System.ArgumentException"/>.
    /// </summary>
    private static (string, bool, string) EveryPublicDescriptorDoorAdmitsTheSurfaceWhenNamed()
    {
        const string Name = "bigint/b05/every-public-descriptor-door-admits-the-surface-when-named";
        var compiled = Compile("String(2n ** 64n);", new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return (Name, false, "the source was refused: " + First(compiled));
        }

        var answers = new System.Collections.Generic.List<string>();

        foreach (var door in new System.Func<VmProfileDescriptor>[]
        {
            () => JavaScriptProfile.DescriptorAdmitting(JavaScriptProfile.BigIntManifest),
            () => JavaScriptProfile.DescriptorAdmitting(JavaScriptProfile.BinaryManifest, JavaScriptProfile.BigIntManifest),
            () => JavaScriptProfile.DescriptorHostingRealms(new ThrowingSurface(), JavaScriptProfile.BigIntManifest),
        })
        {
            try
            {
                TryRun(compiled.Artifact, door(), out var answer, hostSurface: true);
                answers.Add(answer);
            }
            catch (System.ArgumentException refusal)
            {
                answers.Add("refused: " + refusal.Message);
            }
        }

        var shown = string.Join(",", answers);
        const string Expected = "18446744073709551616,18446744073709551616,18446744073709551616";
        return (Name, shown == Expected, "the doors answered " + shown);
    }

    /// <summary>
    /// An update expression under the wide manifest converts with <c>ToNumeric</c> whatever the
    /// program holds, so it declares no surface: a program stepping only Numbers runs under a
    /// composition that declines BigInt, and answers what it always did.
    /// </summary>
    private static (string, bool, string) AnUpdateExpressionDeclaresNothingAndStepsANumber()
    {
        const string Name = "bigint/b05/an-update-expression-declares-nothing-and-steps-a-number";
        var compiled = Compile(
            "var n = -0, m = '5', q = { valueOf() { return 2; } }, u, f = 1.5; var n0 = n++; m++; q--; u++; ++f; " +
            "[Object.is(n0, -0), n, m, q, u, f, typeof m].join(',');",
            new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return (Name, false, "the source was refused: " + First(compiled));
        }

        var declining = JavaScriptProfile.DescriptorAdmitting(JavaScriptProfile.BinaryManifest);

        if (!TryRun(compiled.Artifact, declining, out var answer))
        {
            return (Name, false, answer);
        }

        return (Name, answer == "true,1,6,1,NaN,2.5,number", "the program answered " + answer);
    }

    /// <summary>
    /// A decimal text as long as a value inside the ceiling can be is parsed in charged steps: under
    /// a fuel ceiling its digit count fits the parse is exhausted, and under the default allowance
    /// it answers exactly.
    /// </summary>
    private static (string, bool, string) ALongDecimalTextIsChargedForItsSquare()
    {
        const string Name = "bigint/b05/a-long-decimal-text-is-charged-for-its-square";
        const string Source = "var s = '7'.repeat(315000); var b = BigInt(s); String(b % 1000000007n) + '|' + (b > 10n ** 314999n);";

        var bounded = RunWide(Source, 5_000_000);
        var unbounded = RunWide(Source, fuel: null);

        return (
            Name,
            bounded == "ResourceExhaustion/AllowanceExhausted" && unbounded.EndsWith("|true", System.StringComparison.Ordinal),
            $"under 5000000 fuel it answered {bounded}; under the default allowance {unbounded}");
    }

    /// <summary>
    /// A BigInt property key is a String and makes no BigInt value, so it is exact and the artifact
    /// declares nothing: it runs under a composition that declines the BigInt surface.
    /// </summary>
    private static (string, bool, string) ABigIntPropertyKeyDeclaresNothing()
    {
        const string Name = "bigint/b02/a-bigint-property-key-is-exact-and-declares-nothing";
        var compiled = Compile(
            "var o = { 9007199254740993n: 1 }; Object.keys(o)[0];", new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return (Name, false, "the source was refused: " + First(compiled));
        }

        if (!TryRun(compiled.Artifact, JavaScriptProfile.DescriptorAdmitting(), out var answer))
        {
            return (Name, false, answer);
        }

        return (Name, answer == "9007199254740993", "the program answered " + answer);
    }

    /// <summary>Compiles one script under <paramref name="request"/>.</summary>
    private static JsCompilation Compile(string source, JsCompileRequest request) =>
        JsCompiler.Compile(
            [new JsScriptUnit("main", source, SliceParseOptions.Script, false, Caller)],
            [],
            request);

    /// <summary>The first refusal of a compilation, or a note that there was none.</summary>
    private static string First(JsCompilation compiled) =>
        compiled.Diagnostics.Count == 0 ? "no diagnostic" : compiled.Diagnostics[0].ToString();

    /// <summary>
    /// A guest that throws a BigInt through a host call is seen by the host as a
    /// <see cref="JsHostThrowException"/>, never as an internal exception.
    /// </summary>
    /// <remarks>
    /// Until card B06 the exception carried a <c>TypeError</c> standing in for the BigInt; since
    /// B06 it carries the BigInt itself, which the host renders as <c>1</c>.
    /// </remarks>
    private static (string, bool, string) AThrownBigIntReachesTheHostAsAHostThrow()
    {
        const string Name = "bigint/b01/a-thrown-bigint-reaches-the-host-as-a-host-throw";
        var compiled = Compile(
            "bigintHost.invoke(function () { throw 1n; }) + '|' + bigintHost.invoke(function () { throw 2; });",
            new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return (Name, false, "the source was refused: " + First(compiled));
        }

        var hosting = JavaScriptProfile.DescriptorHostingRealms(new ThrowingSurface(), JavaScriptProfile.BigIntManifest);

        if (!TryRun(compiled.Artifact, hosting, out var answer, hostSurface: true))
        {
            return (Name, false, answer);
        }

        return (Name, answer == "host-throw:1|host-throw:2", "the program answered " + answer);
    }

    /// <summary>
    /// A BigInt a host body returns to a realm whose composition declined the BigInt surface is the
    /// guest <c>TypeError</c> the seam's <see cref="JsHostRefusal.SurfaceDeclined"/> refusal
    /// becomes, and the same body under an admitting composition answers the value (card B06).
    /// </summary>
    private static (string, bool, string) ADecliningRealmRefusesAHostBigInt()
    {
        const string Name = "bigint/b06/a-declining-realm-refuses-a-host-bigint";
        var compiled = Compile(
            "var r; try { r = typeof bigintHost.make(); } catch (e) { r = e instanceof TypeError ? 'TypeError' : 'other:' + e; } r;",
            new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return (Name, false, "the source was refused: " + First(compiled));
        }

        TryRun(
            compiled.Artifact,
            JavaScriptProfile.DescriptorHostingRealms(new ThrowingSurface(), JavaScriptProfile.BinaryManifest),
            out var declined,
            hostSurface: true);

        TryRun(
            compiled.Artifact,
            JavaScriptProfile.DescriptorHostingRealms(new ThrowingSurface()),
            out var admitted,
            hostSurface: true);

        return (
            Name,
            declined == "TypeError" && admitted == "bigint",
            $"declining answered {declined}; admitting answered {admitted}");
    }

    /// <summary>
    /// Crossing a BigInt through the host surface costs fuel in proportion to its words (card B06):
    /// twenty round trips of a value 1,024 words wider than another cost at least 20 x 2 x 1,024
    /// units more, with every other step of the two programs the same.
    /// </summary>
    private static (string, bool, string) AHostCrossingIsChargedPerWord()
    {
        const string Name = "bigint/b06/a-host-crossing-is-charged-per-word";
        const string Prefix = "var wide = 1n << 65536n, narrow = 1n; for (var i = 0; i < 20; i++) bigintHost.echo(";
        const string Suffix = "); 'done';";

        var candidate = Cost(Prefix + "wide" + Suffix);
        var control = Cost(Prefix + "narrow" + Suffix);

        if (candidate is null || control is null)
        {
            return (Name, false, $"a program did not complete: candidate {candidate}, control {control}");
        }

        const ulong Floor = 20UL * 2UL * 1024UL;
        var attributed = candidate.Value > control.Value ? candidate.Value - control.Value : 0;

        return (
            Name,
            attributed >= Floor,
            $"the wide crossings cost {attributed} units more than the narrow ones (floor {Floor})");
    }

    /// <summary>The smallest fuel allowance a host-surface program completes under, by bisection.</summary>
    private static ulong? Cost(string source)
    {
        const ulong Ceiling = 16_000_000;
        var compiled = Compile(source, new JsCompileRequest());

        if (!compiled.Succeeded || compiled.Artifact is null)
        {
            return null;
        }

        var hosting = JavaScriptProfile.DescriptorHostingRealms(new ThrowingSurface());

        if (!TryRun(compiled.Artifact, hosting, out _, hostSurface: true, fuel: Ceiling))
        {
            return null;
        }

        ulong low = 0, high = Ceiling;

        while (high - low > 1)
        {
            var middle = low + ((high - low) / 2);

            if (TryRun(compiled.Artifact, hosting, out _, hostSurface: true, fuel: middle))
            {
                high = middle;
            }
            else
            {
                low = middle;
            }
        }

        return high;
    }

    /// <summary>Verifies, instantiates and runs one artifact, answering its completion or why not.</summary>
    private static bool TryRun(
        byte[] artifact,
        VmProfileDescriptor profile,
        out string answer,
        bool hostSurface = false,
        ulong? fuel = null,
        System.TimeSpan? cancelAfter = null,
        IVmArtifactProvider? provider = null)
    {
        using var runtime = Runtime(profile, hostSurface, fuel, provider);

        if (runtime is null)
        {
            answer = "the runtime refused creation";
            return false;
        }

        var descriptor = Descriptor();
        var verified = runtime.Verify(in descriptor, artifact, System.Threading.CancellationToken.None);

        if (!verified.TryGetArtifact(out var handle))
        {
            answer = $"verification: {verified.Outcome}/{verified.Reason}/{verified.Diagnostics.ProfileDiagnosticCode}";
            return false;
        }

        using (handle)
        {
            var instantiated = runtime.Instantiate(handle, System.Threading.CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                answer = $"instantiation: {instantiated.Outcome}/{instantiated.Reason}";
                return false;
            }

            using (instance)
            {
                var request = new VmInvocationRequest(
                    new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes("main")));

                using var cancellation = new System.Threading.CancellationTokenSource();

                if (cancelAfter is { } delay)
                {
                    cancellation.CancelAfter(delay);
                }

                var invoked = instance.Invoke(in request, cancellation.Token);

                if (invoked.Outcome is VmOutcome.ResourceExhaustion or VmOutcome.Cancellation)
                {
                    answer = $"{invoked.Outcome}/{invoked.Reason}";
                    return false;
                }

                if (!JavaScriptProfile.TryGetWideCompletion(in invoked, out var completion))
                {
                    answer = $"the run answered {invoked.Outcome}/{invoked.Reason}";
                    return false;
                }

                answer = completion.Value;
                return true;
            }
        }
    }

    /// <summary>The descriptor a caller presents with these bytes.</summary>
    private static VmArtifactDescriptor Descriptor() =>
        new(
            JavaScriptProfile.Id,
            JsFormat.FormatVersion,
            JavaScriptProfile.WideManifest,
            default,
            VmCallerIdentity.FromCanonicalIdentity(Caller));

    /// <summary>
    /// A runtime over one profile descriptor, with no capability registered unless
    /// <paramref name="hostSurface"/> asks for the host-surface permission or
    /// <paramref name="provider"/> answers the source-provider capability.
    /// </summary>
    private static VmRuntime? Runtime(
        VmProfileDescriptor profile,
        bool hostSurface = false,
        ulong? fuel = null,
        IVmArtifactProvider? provider = null)
    {
        var capabilities = ImmutableArray.CreateBuilder<VmCapabilityRegistration>();

        if (hostSurface)
        {
            capabilities.Add(VmCapabilityRegistration.Value(
                JavaScriptProfile.HostSurfaceCapability,
                (VmBytes argument, out VmOpaqueRef result) =>
                {
                    result = default;
                    return VmHostCallOutcome.Completed;
                }));
        }

        if (provider is not null)
        {
            capabilities.Add(VmCapabilityRegistration.ArtifactProvider(
                JavaScriptProfile.SourceProviderCapability, provider));
        }

        var catalog = VmCatalog.CreateBuilder().Add(profile).Build();
        var ceilings = ImmutableArray.CreateBuilder<VmCeilingSpec>();

        foreach (var dimension in VmBudgetDimensions.All)
        {
            ceilings.Add(dimension switch
            {
                VmBudgetDimension.LiveRuntimes => VmCeilingSpec.AdoptParentRemaining(dimension),
                VmBudgetDimension.Fuel when fuel is { } stated => VmCeilingSpec.Value(dimension, stated),
                _ => VmCeilingSpec.AdoptProfileDefault(dimension),
            });
        }

        var created = VmRuntime.Create(
            catalog,
            new VmRuntimeCreationOptions(
                aggregateBudget: null,
                ceilings: ceilings.ToImmutable(),
                maxSuspendedResidency: System.TimeSpan.FromMinutes(1),
                maxLiveSuspendedOperations: 1,
                guestLoadBounds: VmGuestLoadBoundsSpec.AdoptProfileMaxima,
                externalSuspension: VmExternalSuspensionMode.Disabled,
                capabilities: capabilities.ToImmutable()));

        return created.TryGetRuntime(out var runtime) ? runtime : null;
    }

    /// <summary>A new internal <c>JsBigInt</c> holding <paramref name="value"/>.</summary>
    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    [return: UnsafeAccessorType(BigIntType)]
    private static extern object NewBigInt(System.Numerics.BigInteger value);

    /// <summary>The integer an internal <c>JsBigInt</c> holds.</summary>
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Value")]
    private static extern System.Numerics.BigInteger ValueOf([UnsafeAccessorType(BigIntType)] object instance);

    /// <summary><c>JsBigInt.AsIntN</c>, the internal <c>BigInt.asIntN</c>.</summary>
    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "AsIntN")]
    [return: UnsafeAccessorType(BigIntType)]
    private static extern object? AsIntN(
        [UnsafeAccessorType(BigIntType)] object? owner,
        ulong bits,
        [UnsafeAccessorType(BigIntType)] object value,
        System.Action<ulong> charge);

    /// <summary><c>JsBigInt.AsUintN</c>, the internal <c>BigInt.asUintN</c>.</summary>
    [UnsafeAccessor(UnsafeAccessorKind.StaticMethod, Name = "AsUintN")]
    [return: UnsafeAccessorType(BigIntType)]
    private static extern object? AsUintN(
        [UnsafeAccessorType(BigIntType)] object? owner,
        ulong bits,
        [UnsafeAccessorType(BigIntType)] object value,
        System.Action<ulong> charge);

    /// <summary>The internal BigInt value type, named for the accessors above.</summary>
    private const string BigIntType = "Broiler.VM.Profile.JavaScript.JsBigInt, Broiler.VM.Profile.JavaScript";

    /// <summary>
    /// A source provider answering every script request by compiling it under the wide manifest, as
    /// the first-party hosts' providers do, so the composition's descriptor alone decides admission.
    /// </summary>
    private sealed class EvalProvider : IVmArtifactProvider
    {
        /// <summary>The identity this provider is registered under.</summary>
        public VmCapabilityId CapabilityId => JavaScriptProfile.SourceProviderCapability.CapabilityId;

        /// <summary>Its exact version.</summary>
        public int Version => JavaScriptProfile.SourceProviderCapability.Version;

        /// <summary>Answers one script request by compiling its payload.</summary>
        public VmArtifactProviderAnswer Answer(scoped in VmArtifactRequest artifactRequest)
        {
            if (artifactRequest.RequestingProfileId != JavaScriptProfile.Id ||
                JsFormat.TryReadModuleRequest(artifactRequest.RequestPayload.Span, out _, out _))
            {
                return VmArtifactProviderAnswer.NotFound(VmReason.ProviderArtifactNotFound);
            }

            if (!JsCompiler.TryReadProgramRequest(artifactRequest.RequestPayload.Span, out var script))
            {
                return VmArtifactProviderAnswer.Refused(VmReason.MalformedEncoding);
            }

            var compiled = JsCompiler.Compile([script], [], new JsCompileRequest());

            if (!compiled.Succeeded || compiled.Artifact is null)
            {
                return VmArtifactProviderAnswer.Refused(VmReason.SemanticValidationFailed);
            }

            var descriptor = Descriptor();
            return VmArtifactProviderAnswer.Provided(in descriptor, compiled.Artifact);
        }
    }

    /// <summary>
    /// A host surface with <c>bigintHost.invoke(f)</c>, which calls <c>f</c> and answers how its
    /// throw reached the host, <c>bigintHost.echo(v)</c>, which answers its argument, and
    /// <c>bigintHost.make()</c>, which answers a BigInt the host built.
    /// </summary>
    private sealed class ThrowingSurface : IJsHostSurface
    {
        public void OnTurn(JsHostRealm realm)
        {
        }

        public void OnRealmCreated(JsHostRealm realm)
        {
            var host = realm.NewObject();

            realm.DefineValue(
                host,
                "invoke",
                realm.NewMethod(
                    "invoke",
                    (r, _, arguments) =>
                    {
                        var callee = arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined;

                        try
                        {
                            r.Invoke(callee, JsHostValue.Undefined, []);
                            return JsHostValue.String("returned");
                        }
                        catch (JsHostThrowException thrown)
                        {
                            var shown = thrown.Thrown.Kind == JsHostValueKind.Object
                                ? r.ToJsString(r.GetProperty(thrown.Thrown, "name"))
                                : r.ToJsString(thrown.Thrown);

                            return JsHostValue.String("host-throw:" + shown);
                        }
                        catch (System.Exception escaped) when (escaped is not JsHostTerminatedException)
                        {
                            return JsHostValue.String("escaped:" + escaped.GetType().Name);
                        }
                    },
                    1));

            realm.DefineValue(
                host,
                "echo",
                realm.NewMethod("echo", (_, _, arguments) => arguments.Length > 0 ? arguments[0] : JsHostValue.Undefined, 1));

            realm.DefineValue(
                host,
                "make",
                realm.NewMethod("make", (_, _, _) => JsHostValue.BigInt(System.Numerics.BigInteger.One)));

            realm.DefineValue(realm.Global, "bigintHost", host);
        }
    }
}
