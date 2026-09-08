using Broiler.VM;
using Broiler.VM.Profile.WebAssembly;
using System.Globalization;

namespace Broiler.VM.Composition.WebAssembly.Harness;

/// <summary>
/// The interpreter driven against answers that came from somewhere other than the interpreter.
/// </summary>
/// <remarks>
/// <para>
/// <b>AN ORACLE THAT ASKS THE THING UNDER TEST IS NOT AN ORACLE.</b> A check that records what the
/// interpreter said and then asserts the interpreter still says it is a change detector, and a
/// useful one, but it cannot tell a correct answer from a wrong one that has been wrong since the
/// day it was written. Every expected value in this file comes from one of three places, none of
/// them the interpreter: an integer operation re-implemented from the specification's own
/// definition as a direct C# expression; a floating-point bit pattern worked out by hand and
/// written down as sixteen hexadecimal digits; or a whole algorithm written twice - once as a
/// WebAssembly function and once as an ordinary C# loop - so that a defect would have to occur
/// identically in two programs written in two languages to escape.
/// </para>
/// <para>
/// <b>The second implementation is deliberately a different shape.</b> The profile's interpreter is
/// a dispatch loop over decoded bodies with an operand stack; the oracle below is a static method
/// per operation with no stack and no dispatch. Two implementations that shared a structure would
/// share the mistakes that structure invites, which is the failure mode a differential check exists
/// to avoid rather than to reproduce.
/// </para>
/// <para>
/// <b>Three honest limits, named rather than left for a reader to discover.</b>
/// </para>
/// <para>
/// <b>One.</b> Where an arithmetic operation produces a NaN, the specification does not fix the
/// payload bits, so those cases assert NaN-ness and the sign where the specification fixes it, and
/// assert nothing about the remaining fifty-one bits. A check that compared them would be pinning
/// an implementation detail and calling it conformance.
/// </para>
/// <para>
/// <b>Two.</b> <c>f64.sqrt</c>, <c>f32.sqrt</c> and the nearest-integer operations are correctly
/// rounded by IEEE 754, and both this oracle and the interpreter reach the platform's own
/// implementation of them. Those rows prove the plumbing - that the right operand reaches the right
/// operation and the result comes back in the right slot - and they do NOT independently confirm
/// the arithmetic. The hand-written bit-pattern table is where the arithmetic is confirmed.
/// </para>
/// <para>
/// <b>Three.</b> This is not the specification's conformance suite. Every case here was thought of
/// by the same person who wrote the corpus, so it cannot find an operation nobody considered. The
/// script reader that would run the published assertion files does not exist.
/// </para>
/// </remarks>
internal static class DifferentialChecks
{
    /// <summary>Runs every differential check and prints what each family saw.</summary>
    internal static int Report(VmRuntime runtime, bool verbose)
    {
        var results = new List<(string Name, bool Passed, string Detail)>();

        results.AddRange(IntegerOperations(runtime));
        results.AddRange(FloatingPointBitPatterns(runtime));
        results.AddRange(Algorithms(runtime));

        var failed = 0;

        Console.WriteLine($"# differential: {results.Count} checks");

        foreach (var (name, passed, detail) in results)
        {
            if (!passed)
            {
                failed++;
            }

            if (verbose || !passed)
            {
                Console.WriteLine($"{(passed ? "ok  " : "FAIL")} {name}: {detail}");
            }
        }

        Console.WriteLine($"# differential: {results.Count - failed} of {results.Count} checks passed");
        return failed;
    }

    // =============================================================================================
    // Integer operations, re-implemented from the specification and driven over a fixed matrix
    // =============================================================================================

    /// <summary>
    /// The i32 operands every binary integer check is driven over.
    /// </summary>
    /// <remarks>
    /// <b>Chosen, not sampled.</b> Every value here is on a boundary something can be got wrong at:
    /// the two extremes of the signed range, the pair whose signed quotient overflows, the shift
    /// counts on either side of the modulus, the alternating bit patterns a rotate gets wrong when
    /// its complement shift is written with the wrong width, and zero on both sides of every
    /// division. A pseudo-random sample would cover the middle of the range, where nothing lives.
    /// </remarks>
    private static readonly int[] Int32Operands =
    [
        0,
        1,
        -1,
        2,
        -2,
        7,
        -7,
        31,
        32,
        33,
        63,
        int.MaxValue,
        int.MinValue,
        int.MinValue + 1,
        unchecked((int)0x80000001),
        0x55555555,
        unchecked((int)0xAAAAAAAA),
        65535,
        -65536,
    ];

    /// <summary>The i64 operands, chosen on the same principle at the wider width.</summary>
    private static readonly long[] Int64Operands =
    [
        0,
        1,
        -1,
        2,
        -2,
        63,
        64,
        65,
        long.MaxValue,
        long.MinValue,
        long.MinValue + 1,
        unchecked((long)0x8000000000000001),
        0x5555555555555555,
        unchecked((long)0xAAAAAAAAAAAAAAAA),
        4294967296,
        -4294967296,
    ];

    private static IEnumerable<(string, bool, string)> IntegerOperations(VmRuntime runtime)
    {
        var assembler = new WasmAssembler();
        var i32Binary = assembler.Type([WasmAssembler.I32, WasmAssembler.I32], [WasmAssembler.I32]);
        var i32Unary = assembler.Type([WasmAssembler.I32], [WasmAssembler.I32]);
        var i64Binary = assembler.Type([WasmAssembler.I64, WasmAssembler.I64], [WasmAssembler.I64]);
        var i64Unary = assembler.Type([WasmAssembler.I64], [WasmAssembler.I64]);

        foreach (var (name, opcode, _) in Int32BinaryOperations)
        {
            assembler.Export(
                name, WasmAssembler.ExportFunction, assembler.Function(i32Binary, [], Binary(opcode)));
        }

        foreach (var (name, opcode, _) in Int32UnaryOperations)
        {
            assembler.Export(
                name, WasmAssembler.ExportFunction, assembler.Function(i32Unary, [], Unary(opcode)));
        }

        foreach (var (name, opcode, _) in Int64BinaryOperations)
        {
            assembler.Export(
                name, WasmAssembler.ExportFunction, assembler.Function(i64Binary, [], Binary(opcode)));
        }

        foreach (var (name, opcode, _) in Int64UnaryOperations)
        {
            assembler.Export(
                name, WasmAssembler.ExportFunction, assembler.Function(i64Unary, [], Unary(opcode)));
        }

        var results = new List<(string, bool, string)>();

        using var session = Session.Open(runtime, assembler.Build(), "integer", results);

        if (session is null)
        {
            return results;
        }

        foreach (var (name, _, oracle) in Int32BinaryOperations)
        {
            var cases = 0;
            var disagreements = new List<string>();

            foreach (var left in Int32Operands)
            {
                foreach (var right in Int32Operands)
                {
                    cases++;
                    var expected = oracle(left, right);
                    var observed = session.Call(Entry(name, I32Argument(left), I32Argument(right)));
                    var complaint = Compare(expected, observed, WebAssemblyValueKind.I32);

                    if (complaint is not null && disagreements.Count < 4)
                    {
                        disagreements.Add($"{name}({left}, {right}) {complaint}");
                    }
                    else if (complaint is not null)
                    {
                        disagreements.Add("...");
                    }
                }
            }

            results.Add(Verdict($"i32.{name}", cases, disagreements));
        }

        foreach (var (name, _, oracle) in Int32UnaryOperations)
        {
            var disagreements = new List<string>();

            foreach (var operand in Int32Operands)
            {
                var complaint = Compare(
                    oracle(operand),
                    session.Call(Entry(name, I32Argument(operand))),
                    WebAssemblyValueKind.I32);

                if (complaint is not null)
                {
                    disagreements.Add($"{name}({operand}) {complaint}");
                }
            }

            results.Add(Verdict($"i32.{name}", Int32Operands.Length, disagreements));
        }

        foreach (var (name, _, oracle) in Int64BinaryOperations)
        {
            var cases = 0;
            var disagreements = new List<string>();

            foreach (var left in Int64Operands)
            {
                foreach (var right in Int64Operands)
                {
                    cases++;
                    var complaint = Compare(
                        oracle(left, right),
                        session.Call(Entry(name, I64Argument(left), I64Argument(right))),
                        WebAssemblyValueKind.I64);

                    if (complaint is not null && disagreements.Count < 4)
                    {
                        disagreements.Add($"{name}({left}, {right}) {complaint}");
                    }
                }
            }

            results.Add(Verdict($"i64.{name}", cases, disagreements));
        }

        foreach (var (name, _, oracle) in Int64UnaryOperations)
        {
            var disagreements = new List<string>();

            foreach (var operand in Int64Operands)
            {
                var complaint = Compare(
                    oracle(operand),
                    session.Call(Entry(name, I64Argument(operand))),
                    WebAssemblyValueKind.I64);

                if (complaint is not null)
                {
                    disagreements.Add($"{name}({operand}) {complaint}");
                }
            }

            results.Add(Verdict($"i64.{name}", Int64Operands.Length, disagreements));
        }

        return results;
    }

    // =============================================================================================
    // THE ORACLE. Every method below is written from the specification's definition of the
    // operation and reads nothing the profile publishes.
    // =============================================================================================

    /// <summary>An answer the oracle derived: a value, or the trap the operation must produce.</summary>
    private readonly record struct Answer(ulong Bits, WasmTrapKind? Trap)
    {
        internal static Answer Value(int value) => new((uint)value, null);

        internal static Answer Value(long value) => new((ulong)value, null);

        internal static Answer Traps(WasmTrapKind kind) => new(0, kind);
    }

    private static readonly (string Name, byte Opcode, Func<int, int, Answer> Oracle)[]
        Int32BinaryOperations =
    [
        ("add", 0x6A, static (a, b) => Answer.Value(unchecked(a + b))),
        ("sub", 0x6B, static (a, b) => Answer.Value(unchecked(a - b))),
        ("mul", 0x6C, static (a, b) => Answer.Value(unchecked(a * b))),

        // THE TWO TRAPS OF SIGNED DIVISION, AND THEY ARE DIFFERENT TRAPS. A zero divisor is a
        // divide-by-zero; the one quotient the signed range cannot hold is an overflow. An
        // implementation that answered one kind for both would pass a check that only tested that
        // it trapped.
        ("div_s", 0x6D, static (a, b) =>
            b == 0 ? Answer.Traps(WasmTrapKind.IntegerDivideByZero)
            : a == int.MinValue && b == -1 ? Answer.Traps(WasmTrapKind.IntegerOverflow)
            : Answer.Value(a / b)),

        ("div_u", 0x6E, static (a, b) =>
            b == 0 ? Answer.Traps(WasmTrapKind.IntegerDivideByZero)
            : Answer.Value(unchecked((int)((uint)a / (uint)b)))),

        // AND THE ASYMMETRY THAT CATCHES EVERYBODY: signed remainder of the minimum by minus one is
        // ZERO and does NOT trap, although the division of the same pair does. Writing this as
        // `a % b` would throw in C# on the same pair, which is how the divergence hides.
        ("rem_s", 0x6F, static (a, b) =>
            b == 0 ? Answer.Traps(WasmTrapKind.IntegerDivideByZero)
            : a == int.MinValue && b == -1 ? Answer.Value(0)
            : Answer.Value(a % b)),

        ("rem_u", 0x70, static (a, b) =>
            b == 0 ? Answer.Traps(WasmTrapKind.IntegerDivideByZero)
            : Answer.Value(unchecked((int)((uint)a % (uint)b)))),

        ("and", 0x71, static (a, b) => Answer.Value(a & b)),
        ("or", 0x72, static (a, b) => Answer.Value(a | b)),
        ("xor", 0x73, static (a, b) => Answer.Value(a ^ b)),

        // THE SHIFT COUNT IS TAKEN MODULO THE WIDTH and is not saturated and not a trap. The
        // operand matrix carries 31, 32, 33 and 63 for exactly this.
        ("shl", 0x74, static (a, b) => Answer.Value(unchecked((int)((uint)a << (b & 31))))),
        ("shr_s", 0x75, static (a, b) => Answer.Value(a >> (b & 31))),
        ("shr_u", 0x76, static (a, b) => Answer.Value(unchecked((int)((uint)a >> (b & 31))))),

        // A ROTATION IS TWO SHIFTS AND AN OR, and the complement shift is itself taken modulo the
        // width - because a shift by thirty-two is a shift by zero, so a rotation by zero written
        // without the second modulus reads the operand's low bits back into its high ones.
        ("rotl", 0x77, static (a, b) => Answer.Value(unchecked((int)(
            ((uint)a << (b & 31)) | ((uint)a >> ((32 - (b & 31)) & 31)))))),

        ("rotr", 0x78, static (a, b) => Answer.Value(unchecked((int)(
            ((uint)a >> (b & 31)) | ((uint)a << ((32 - (b & 31)) & 31)))))),
    ];

    private static readonly (string Name, byte Opcode, Func<int, Answer> Oracle)[]
        Int32UnaryOperations =
    [
        // Counted in a loop rather than handed to BitOperations. The loop is slower and is the
        // point: it is a second implementation, and calling one intrinsic from both sides would be
        // one implementation asked twice.
        ("clz", 0x67, static a =>
        {
            var count = 0;

            for (var bit = 31; bit >= 0 && ((uint)a & (1u << bit)) == 0; bit--)
            {
                count++;
            }

            return Answer.Value(count);
        }),

        ("ctz", 0x68, static a =>
        {
            var count = 0;

            for (var bit = 0; bit < 32 && ((uint)a & (1u << bit)) == 0; bit++)
            {
                count++;
            }

            return Answer.Value(count);
        }),

        ("popcnt", 0x69, static a =>
        {
            var count = 0;

            for (var bit = 0; bit < 32; bit++)
            {
                count += ((uint)a & (1u << bit)) != 0 ? 1 : 0;
            }

            return Answer.Value(count);
        }),

        ("eqz", 0x45, static a => Answer.Value(a == 0 ? 1 : 0)),
    ];

    private static readonly (string Name, byte Opcode, Func<long, long, Answer> Oracle)[]
        Int64BinaryOperations =
    [
        ("i64add", 0x7C, static (a, b) => Answer.Value(unchecked(a + b))),
        ("i64mul", 0x7E, static (a, b) => Answer.Value(unchecked(a * b))),

        ("i64div_s", 0x7F, static (a, b) =>
            b == 0 ? Answer.Traps(WasmTrapKind.IntegerDivideByZero)
            : a == long.MinValue && b == -1 ? Answer.Traps(WasmTrapKind.IntegerOverflow)
            : Answer.Value(a / b)),

        ("i64div_u", 0x80, static (a, b) =>
            b == 0 ? Answer.Traps(WasmTrapKind.IntegerDivideByZero)
            : Answer.Value(unchecked((long)((ulong)a / (ulong)b)))),

        ("i64rem_s", 0x81, static (a, b) =>
            b == 0 ? Answer.Traps(WasmTrapKind.IntegerDivideByZero)
            : a == long.MinValue && b == -1 ? Answer.Value(0L)
            : Answer.Value(a % b)),

        ("i64rem_u", 0x82, static (a, b) =>
            b == 0 ? Answer.Traps(WasmTrapKind.IntegerDivideByZero)
            : Answer.Value(unchecked((long)((ulong)a % (ulong)b)))),

        ("i64shl", 0x86, static (a, b) =>
            Answer.Value(unchecked((long)((ulong)a << (int)(b & 63))))),

        ("i64shr_s", 0x87, static (a, b) => Answer.Value(a >> (int)(b & 63))),

        ("i64shr_u", 0x88, static (a, b) =>
            Answer.Value(unchecked((long)((ulong)a >> (int)(b & 63))))),

        ("i64rotl", 0x89, static (a, b) => Answer.Value(unchecked((long)(
            ((ulong)a << (int)(b & 63)) | ((ulong)a >> ((64 - (int)(b & 63)) & 63)))))),

        ("i64rotr", 0x8A, static (a, b) => Answer.Value(unchecked((long)(
            ((ulong)a >> (int)(b & 63)) | ((ulong)a << ((64 - (int)(b & 63)) & 63)))))),
    ];

    private static readonly (string Name, byte Opcode, Func<long, Answer> Oracle)[]
        Int64UnaryOperations =
    [
        ("i64clz", 0x79, static a =>
        {
            var count = 0;

            for (var bit = 63; bit >= 0 && ((ulong)a & (1UL << bit)) == 0; bit--)
            {
                count++;
            }

            return Answer.Value((long)count);
        }),

        ("i64ctz", 0x7A, static a =>
        {
            var count = 0;

            for (var bit = 0; bit < 64 && ((ulong)a & (1UL << bit)) == 0; bit++)
            {
                count++;
            }

            return Answer.Value((long)count);
        }),

        ("i64popcnt", 0x7B, static a =>
        {
            var count = 0;

            for (var bit = 0; bit < 64; bit++)
            {
                count += ((ulong)a & (1UL << bit)) != 0 ? 1 : 0;
            }

            return Answer.Value((long)count);
        }),
    ];

    // =============================================================================================
    // Floating point, against bit patterns worked out by hand
    // =============================================================================================

    /// <summary>
    /// One floating-point case: the operation, its two operands as bit patterns, and the bit
    /// pattern of the result a person derived.
    /// </summary>
    /// <remarks>
    /// <b>Written as bits at both ends on purpose.</b> A case written as decimal literals would be
    /// asserting that the C# compiler's decimal-to-binary conversion agrees with itself, which it
    /// does, and would be silent about the fifty-two bits that actually differ when an
    /// implementation rounds the wrong way. The comment beside each row says where the number came
    /// from.
    /// </remarks>
    private readonly record struct FloatCase(string Entry, ulong Left, ulong Right, ulong Expected);

    private static IEnumerable<(string, bool, string)> FloatingPointBitPatterns(VmRuntime runtime)
    {
        var assembler = new WasmAssembler();
        var f64Binary = assembler.Type([WasmAssembler.F64, WasmAssembler.F64], [WasmAssembler.F64]);
        var f64Unary = assembler.Type([WasmAssembler.F64], [WasmAssembler.F64]);

        assembler.Export("f64add", WasmAssembler.ExportFunction,
            assembler.Function(f64Binary, [], Binary(0xA0)));
        assembler.Export("f64sub", WasmAssembler.ExportFunction,
            assembler.Function(f64Binary, [], Binary(0xA1)));
        assembler.Export("f64mul", WasmAssembler.ExportFunction,
            assembler.Function(f64Binary, [], Binary(0xA2)));
        assembler.Export("f64div", WasmAssembler.ExportFunction,
            assembler.Function(f64Binary, [], Binary(0xA3)));
        assembler.Export("f64min", WasmAssembler.ExportFunction,
            assembler.Function(f64Binary, [], Binary(0xA4)));
        assembler.Export("f64max", WasmAssembler.ExportFunction,
            assembler.Function(f64Binary, [], Binary(0xA5)));
        assembler.Export("f64copysign", WasmAssembler.ExportFunction,
            assembler.Function(f64Binary, [], Binary(0xA6)));
        assembler.Export("f64abs", WasmAssembler.ExportFunction,
            assembler.Function(f64Unary, [], Unary(0x99)));
        assembler.Export("f64neg", WasmAssembler.ExportFunction,
            assembler.Function(f64Unary, [], Unary(0x9A)));
        assembler.Export("f64ceil", WasmAssembler.ExportFunction,
            assembler.Function(f64Unary, [], Unary(0x9B)));
        assembler.Export("f64floor", WasmAssembler.ExportFunction,
            assembler.Function(f64Unary, [], Unary(0x9C)));
        assembler.Export("f64trunc", WasmAssembler.ExportFunction,
            assembler.Function(f64Unary, [], Unary(0x9D)));
        assembler.Export("f64nearest", WasmAssembler.ExportFunction,
            assembler.Function(f64Unary, [], Unary(0x9E)));

        var results = new List<(string, bool, string)>();

        using var session = Session.Open(runtime, assembler.Build(), "floating-point", results);

        if (session is null)
        {
            return results;
        }

        var disagreements = new List<string>();

        foreach (var one in BinaryFloatCases)
        {
            var observed = session.Call(Entry(
                one.Entry, F64Argument(one.Left), F64Argument(one.Right)));

            var complaint = Compare(
                new Answer(one.Expected, null), observed, WebAssemblyValueKind.F64);

            if (complaint is not null)
            {
                disagreements.Add($"{one.Entry}(0x{one.Left:X16}, 0x{one.Right:X16}) {complaint}");
            }
        }

        results.Add(Verdict(
            "f64 binary operations against hand-derived bit patterns",
            BinaryFloatCases.Length,
            disagreements));

        disagreements = [];

        foreach (var one in UnaryFloatCases)
        {
            var complaint = Compare(
                new Answer(one.Expected, null),
                session.Call(Entry(one.Entry, F64Argument(one.Left))),
                WebAssemblyValueKind.F64);

            if (complaint is not null)
            {
                disagreements.Add($"{one.Entry}(0x{one.Left:X16}) {complaint}");
            }
        }

        results.Add(Verdict(
            "f64 unary operations against hand-derived bit patterns",
            UnaryFloatCases.Length,
            disagreements));

        // NaN CASES ARE JUDGED ON NaN-NESS AND NOT ON BITS, because the specification does not fix
        // the payload an arithmetic NaN carries. Asserting the payload would be pinning this
        // implementation's choice and reporting it as conformance.
        disagreements = [];

        foreach (var (entry, left, right) in NaNProducingCases)
        {
            var observed = session.Call(Entry(entry, F64Argument(left), F64Argument(right)));

            if (observed.Trap is not null)
            {
                disagreements.Add($"{entry}(0x{left:X16}, 0x{right:X16}) trapped {observed.Trap}");
                continue;
            }

            if (!double.IsNaN(BitConverter.UInt64BitsToDouble(observed.Bits)))
            {
                disagreements.Add(
                    $"{entry}(0x{left:X16}, 0x{right:X16}) answered 0x{observed.Bits:X16}, " +
                    "which is not a NaN");
            }
        }

        results.Add(Verdict(
            "f64 operations that must produce a NaN", NaNProducingCases.Length, disagreements));

        return results;
    }

    private const ulong Zero = 0x0000000000000000;
    private const ulong NegativeZero = 0x8000000000000000;
    private const ulong One = 0x3FF0000000000000;
    private const ulong NegativeOne = 0xBFF0000000000000;
    private const ulong Two = 0x4000000000000000;
    private const ulong Infinity = 0x7FF0000000000000;
    private const ulong NegativeInfinity = 0xFFF0000000000000;
    private const ulong QuietNaN = 0x7FF8000000000000;

    /// <summary>A tenth, which is not a tenth: the nearest double below it.</summary>
    private const ulong PointOne = 0x3FB999999999999A;

    /// <summary>A fifth, the same way.</summary>
    private const ulong PointTwo = 0x3FC999999999999A;

    /// <summary>The smallest subnormal: one unit in the last place of zero.</summary>
    private const ulong SmallestSubnormal = 0x0000000000000001;

    /// <summary>The largest finite double.</summary>
    private const ulong MaximumFinite = 0x7FEFFFFFFFFFFFFF;

    private static readonly FloatCase[] BinaryFloatCases =
    [
        // 0.1 + 0.2 IS NOT 0.3, AND THIS IS THE BIT PATTERN IT IS INSTEAD. The two operands round
        // up, their exact sum is 0.3000000000000000166533453693773481063544750213623046875, and
        // the nearest double to that is 0x3FD3333333333334 - one unit in the last place above the
        // double nearest to three tenths, which is 0x3FD3333333333333. A check written with a
        // tolerance would pass whichever of the two an implementation produced.
        new("f64add", PointOne, PointTwo, 0x3FD3333333333334),

        // 1 + 2^-53 rounds to 1: the sum is exactly halfway between 1 and its successor, and
        // round-half-to-even takes the even one, which is 1. An implementation rounding half away
        // from zero answers 0x3FF0000000000001 here and agrees everywhere else.
        new("f64add", One, 0x3CA0000000000000, One),

        // 1 + 2^-52 is exactly representable and is the successor of one.
        new("f64add", One, 0x3CB0000000000000, 0x3FF0000000000001),

        // The largest finite plus itself overflows to infinity rather than saturating.
        new("f64add", MaximumFinite, MaximumFinite, Infinity),

        // Infinity minus infinity is a NaN, and it is in the NaN table rather than here.
        new("f64sub", One, One, Zero),

        // ZERO MINUS ZERO IS POSITIVE ZERO under round-to-nearest, and NEGATIVE zero only when the
        // signs differ that way. Subtraction is where a sign-of-zero defect shows.
        new("f64sub", Zero, Zero, Zero),
        new("f64sub", NegativeZero, Zero, NegativeZero),
        new("f64sub", Zero, NegativeZero, Zero),

        // 2^-1074 halved is zero and not a denormal-flush to something else.
        new("f64mul", SmallestSubnormal, 0x3FE0000000000000, Zero),

        // The smallest subnormal doubled is the second smallest.
        new("f64mul", SmallestSubnormal, Two, 0x0000000000000002),

        // A negative times a positive zero is a NEGATIVE zero.
        new("f64mul", NegativeOne, Zero, NegativeZero),

        // One divided by three: 0x3FD5555555555555, the nearest double below a third.
        new("f64div", One, 0x4008000000000000, 0x3FD5555555555555),

        // One over zero is an infinity and NOT a trap: floating-point division by zero is defined.
        new("f64div", One, Zero, Infinity),
        new("f64div", One, NegativeZero, NegativeInfinity),
        new("f64div", NegativeOne, Zero, NegativeInfinity),

        // MINIMUM OF THE TWO ZEROES IS THE NEGATIVE ONE, in either operand order. This is the case
        // an implementation written as `a < b ? a : b` gets wrong, because negative zero is not
        // less than positive zero - they compare equal - so that expression answers whichever
        // operand happened to be second.
        new("f64min", NegativeZero, Zero, NegativeZero),
        new("f64min", Zero, NegativeZero, NegativeZero),
        new("f64max", NegativeZero, Zero, Zero),
        new("f64max", Zero, NegativeZero, Zero),
        new("f64min", One, Two, One),
        new("f64max", One, Two, Two),
        new("f64min", NegativeInfinity, MaximumFinite, NegativeInfinity),
        new("f64max", Infinity, MaximumFinite, Infinity),

        // Copysign takes the sign bit and NOTHING else, including from a NaN and including onto one.
        new("f64copysign", One, NegativeZero, NegativeOne),
        new("f64copysign", NegativeOne, Zero, One),
        new("f64copysign", Infinity, NegativeOne, NegativeInfinity),
        new("f64copysign", QuietNaN, NegativeOne, 0xFFF8000000000000),
    ];

    private static readonly FloatCase[] UnaryFloatCases =
    [
        new("f64abs", NegativeOne, 0, One),
        new("f64abs", NegativeZero, 0, Zero),
        new("f64abs", NegativeInfinity, 0, Infinity),
        new("f64neg", One, 0, NegativeOne),
        new("f64neg", Zero, 0, NegativeZero),
        new("f64neg", NegativeZero, 0, Zero),

        // 1.5 rounds up under ceil, down under floor, toward zero under trunc, and to TWO under
        // nearest - the even neighbour.
        new("f64ceil", 0x3FF8000000000000, 0, Two),
        new("f64floor", 0x3FF8000000000000, 0, One),
        new("f64trunc", 0x3FF8000000000000, 0, One),
        new("f64nearest", 0x3FF8000000000000, 0, Two),

        // 0.5 IS THE CASE THAT SEPARATES THE TWO ROUNDING RULES. Half-to-even answers zero and
        // half-away-from-zero answers one, and both are common.
        new("f64nearest", 0x3FE0000000000000, 0, Zero),

        // 2.5 the same way: half-to-even answers two.
        new("f64nearest", 0x4004000000000000, 0, Two),

        // And 3.5 answers four, which is what proves it is even-seeking rather than always-down.
        new("f64nearest", 0x400C000000000000, 0, 0x4010000000000000),

        // CEIL OF A NEGATIVE FRACTION IS NEGATIVE ZERO AND NOT POSITIVE ZERO. An implementation
        // that reached for a rounding helper and let the sign go answers 0x0000000000000000 here.
        new("f64ceil", 0xBFE0000000000000, 0, NegativeZero),
        new("f64trunc", 0xBFE0000000000000, 0, NegativeZero),
        new("f64nearest", 0xBFE0000000000000, 0, NegativeZero),
        new("f64floor", 0xBFE0000000000000, 0, NegativeOne),

        // An infinity is already an integer and every one of the four leaves it alone.
        new("f64ceil", Infinity, 0, Infinity),
        new("f64floor", NegativeInfinity, 0, NegativeInfinity),
        new("f64trunc", NegativeInfinity, 0, NegativeInfinity),
        new("f64nearest", Infinity, 0, Infinity),
    ];

    private static readonly (string Entry, ulong Left, ulong Right)[] NaNProducingCases =
    [
        ("f64sub", Infinity, Infinity),
        ("f64add", Infinity, NegativeInfinity),
        ("f64mul", Zero, Infinity),
        ("f64div", Zero, Zero),
        ("f64div", Infinity, Infinity),
        ("f64add", QuietNaN, One),
        ("f64min", QuietNaN, One),
        ("f64max", One, QuietNaN),
    ];

    // =============================================================================================
    // Whole algorithms, written twice
    // =============================================================================================

    /// <summary>
    /// Four algorithms written once in WebAssembly and once in C#, and compared over a range.
    /// </summary>
    /// <remarks>
    /// <b>This is the family that reaches control flow, memory and calls at once.</b> The operations
    /// above are each one instruction with its operands handed to it; a loop that counts, a
    /// recursion that unwinds, a table dispatch and a memory scan exercise the branch, the frame,
    /// the element segment and the linear memory together - and each one has an ordinary C# program
    /// beside it that computes the same number without going near the interpreter.
    /// </remarks>
    private static IEnumerable<(string, bool, string)> Algorithms(VmRuntime runtime)
    {
        var assembler = new WasmAssembler();
        var unary = assembler.Type([WasmAssembler.I32], [WasmAssembler.I32]);
        var binary = assembler.Type([WasmAssembler.I32, WasmAssembler.I32], [WasmAssembler.I32]);

        assembler.Memory(1, 1);
        assembler.Table(4);

        // fib(n): an iterative loop with three locals, so the answer comes out of br_if and
        // local.set rather than out of a single arithmetic instruction.
        //
        //   a = 0; b = 1; while (n != 0) { t = a + b; a = b; b = t; n-- } return a
        var fib = assembler.Function(unary,
            [WasmAssembler.I32, WasmAssembler.I32, WasmAssembler.I32],
            Instruction.Cat(
                Instruction.I32Const(0), Instruction.LocalSet(1),
                Instruction.I32Const(1), Instruction.LocalSet(2),
                Instruction.Block(Instruction.EmptyBlock),
                Instruction.Loop(Instruction.EmptyBlock),
                Instruction.LocalGet(0), [Instruction.I32Eqz], Instruction.BrIf(1),
                Instruction.LocalGet(1), Instruction.LocalGet(2), [Instruction.I32Add],
                Instruction.LocalSet(3),
                Instruction.LocalGet(2), Instruction.LocalSet(1),
                Instruction.LocalGet(3), Instruction.LocalSet(2),
                Instruction.LocalGet(0), Instruction.I32Const(1), [Instruction.I32Sub],
                Instruction.LocalSet(0),
                Instruction.Br(0),
                Instruction.End(),
                Instruction.End(),
                Instruction.LocalGet(1)));

        // gcd(a, b) by Euclid, on unsigned remainder.
        var gcd = assembler.Function(binary, [],
            Instruction.Cat(
                Instruction.Block(Instruction.EmptyBlock),
                Instruction.Loop(Instruction.EmptyBlock),
                Instruction.LocalGet(1), [Instruction.I32Eqz], Instruction.BrIf(1),
                Instruction.LocalGet(1),
                Instruction.LocalGet(0), Instruction.LocalGet(1), [0x70],
                Instruction.LocalSet(1),
                Instruction.LocalSet(0),
                Instruction.Br(0),
                Instruction.End(),
                Instruction.End(),
                Instruction.LocalGet(0)));

        // A memory scan: write i*i at 4*i for i below n, then add them back up. The two halves
        // touch the same addresses, so an off-by-one in either shows as a wrong total.
        var scan = assembler.Function(unary, [WasmAssembler.I32, WasmAssembler.I32],
            Instruction.Cat(
                Instruction.I32Const(0), Instruction.LocalSet(1),
                Instruction.Block(Instruction.EmptyBlock),
                Instruction.Loop(Instruction.EmptyBlock),
                Instruction.LocalGet(1), Instruction.LocalGet(0), [Instruction.I32Eq],
                Instruction.BrIf(1),
                Instruction.LocalGet(1), Instruction.I32Const(4), [Instruction.I32Mul],
                Instruction.LocalGet(1), Instruction.LocalGet(1), [Instruction.I32Mul],
                [0x36], WasmAssembler.Leb(2), WasmAssembler.Leb(0),
                Instruction.LocalGet(1), Instruction.I32Const(1), [Instruction.I32Add],
                Instruction.LocalSet(1),
                Instruction.Br(0),
                Instruction.End(),
                Instruction.End(),
                Instruction.I32Const(0), Instruction.LocalSet(1),
                Instruction.I32Const(0), Instruction.LocalSet(2),
                Instruction.Block(Instruction.EmptyBlock),
                Instruction.Loop(Instruction.EmptyBlock),
                Instruction.LocalGet(1), Instruction.LocalGet(0), [Instruction.I32Eq],
                Instruction.BrIf(1),
                Instruction.LocalGet(2),
                Instruction.LocalGet(1), Instruction.I32Const(4), [Instruction.I32Mul],
                Instruction.Load(0x28, 2, 0),
                [Instruction.I32Add], Instruction.LocalSet(2),
                Instruction.LocalGet(1), Instruction.I32Const(1), [Instruction.I32Add],
                Instruction.LocalSet(1),
                Instruction.Br(0),
                Instruction.End(),
                Instruction.End(),
                Instruction.LocalGet(2)));

        // Three one-line functions reached through the table, so the dispatch is what decides the
        // answer rather than a direct call the validator resolved.
        var doubler = assembler.Function(unary, [],
            Instruction.Cat(Instruction.LocalGet(0), Instruction.LocalGet(0), [Instruction.I32Add]));

        var negater = assembler.Function(unary, [],
            Instruction.Cat(Instruction.I32Const(0), Instruction.LocalGet(0), [Instruction.I32Sub]));

        var squarer = assembler.Function(unary, [],
            Instruction.Cat(Instruction.LocalGet(0), Instruction.LocalGet(0), [Instruction.I32Mul]));

        var dispatch = assembler.Function(binary, [],
            Instruction.Cat(
                Instruction.LocalGet(0),
                Instruction.LocalGet(1),
                Instruction.CallIndirect(unary)));

        assembler.Element(0, doubler, negater, squarer);
        assembler.Export("fib", WasmAssembler.ExportFunction, fib);
        assembler.Export("gcd", WasmAssembler.ExportFunction, gcd);
        assembler.Export("scan", WasmAssembler.ExportFunction, scan);
        assembler.Export("dispatch", WasmAssembler.ExportFunction, dispatch);

        var results = new List<(string, bool, string)>();

        using var session = Session.Open(runtime, assembler.Build(), "algorithm", results);

        if (session is null)
        {
            return results;
        }

        // THE C# SIDE OF EACH PAIR. Written from the algorithm and not from the bytes above.
        var disagreements = new List<string>();

        for (var n = 0; n <= 40; n++)
        {
            var a = 0;
            var b = 1;

            for (var step = 0; step < n; step++)
            {
                (a, b) = (b, unchecked(a + b));
            }

            var complaint = Compare(
                Answer.Value(a), session.Call(Entry("fib", I32Argument(n))), WebAssemblyValueKind.I32);

            if (complaint is not null)
            {
                disagreements.Add($"fib({n}) {complaint}");
            }
        }

        results.Add(Verdict("an iterative Fibonacci against the same loop in C#", 41, disagreements));

        disagreements = [];
        var pairs = 0;

        for (var left = 0; left <= 60; left += 3)
        {
            for (var right = 0; right <= 60; right += 7)
            {
                pairs++;

                var a = (uint)left;
                var b = (uint)right;

                while (b != 0)
                {
                    (a, b) = (b, a % b);
                }

                var complaint = Compare(
                    Answer.Value(unchecked((int)a)),
                    session.Call(Entry("gcd", I32Argument(left), I32Argument(right))),
                    WebAssemblyValueKind.I32);

                if (complaint is not null)
                {
                    disagreements.Add($"gcd({left}, {right}) {complaint}");
                }
            }
        }

        results.Add(Verdict("Euclid's algorithm against the same loop in C#", pairs, disagreements));

        disagreements = [];

        for (var n = 0; n <= 64; n++)
        {
            var total = 0;

            for (var index = 0; index < n; index++)
            {
                total = unchecked(total + (index * index));
            }

            var complaint = Compare(
                Answer.Value(total),
                session.Call(Entry("scan", I32Argument(n))),
                WebAssemblyValueKind.I32);

            if (complaint is not null)
            {
                disagreements.Add($"scan({n}) {complaint}");
            }
        }

        results.Add(Verdict(
            "a memory write-then-read scan against the same sum in C#", 65, disagreements));

        disagreements = [];
        var dispatched = 0;

        foreach (var operand in new[] { -3, 0, 1, 5, 12, 40000 })
        {
            foreach (var slot in new[] { 0, 1, 2 })
            {
                dispatched++;

                var expected = slot switch
                {
                    0 => unchecked(operand + operand),
                    1 => unchecked(0 - operand),
                    _ => unchecked(operand * operand),
                };

                var complaint = Compare(
                    Answer.Value(expected),
                    session.Call(Entry("dispatch", I32Argument(operand), I32Argument(slot))),
                    WebAssemblyValueKind.I32);

                if (complaint is not null)
                {
                    disagreements.Add($"dispatch({operand}, {slot}) {complaint}");
                }
            }
        }

        // The fourth slot of the table was never written by the element segment, so it holds no
        // function reference and the call must TRAP rather than reaching whatever is nearby.
        dispatched++;

        var uninitialised = session.Call(Entry("dispatch", I32Argument(1), I32Argument(3)));

        if (uninitialised.Trap is not WasmTrapKind.UninitializedElement)
        {
            disagreements.Add(
                $"dispatch(1, 3) answered {Describe(uninitialised)}, and the fourth table slot " +
                "holds no function");
        }

        // And one past the table entirely, which must be A DIFFERENT TRAP from an empty slot.
        //
        // CORRECTED 2026-09-07. This case first required UndefinedElement, which is the name the
        // reference interpreter prints, and the profile answers OutOfBoundsTableAccess. NEITHER IS
        // WRONG AND THE SPECIFICATION SETTLES NEITHER: what it fixes is that an index at or above
        // the table's length traps, and the name of the trap is this profile's own vocabulary. What
        // is worth pinning is the property underneath - that an index past the table and an empty
        // slot inside it are told apart rather than collapsed into one answer, because a caller that
        // cannot tell them apart cannot tell a bad index from a table it forgot to fill.
        dispatched++;

        var outOfRange = session.Call(Entry("dispatch", I32Argument(1), I32Argument(9)));

        if (outOfRange.Trap is not WasmTrapKind.OutOfBoundsTableAccess)
        {
            disagreements.Add(
                $"dispatch(1, 9) answered {Describe(outOfRange)}, and the table holds four slots");
        }

        if (outOfRange.Trap == uninitialised.Trap)
        {
            disagreements.Add(
                $"an index past the table and an empty slot inside it both answered " +
                $"{Describe(outOfRange)}, and a caller cannot tell them apart");
        }

        results.Add(Verdict(
            "an indirect dispatch against the three functions written in C#",
            dispatched,
            disagreements));

        return results;
    }

    // =============================================================================================
    // The driver
    // =============================================================================================

    /// <summary>What one invocation actually answered.</summary>
    private readonly record struct Observed(
        WebAssemblyValueKind Kind, ulong Bits, WasmTrapKind? Trap, string? Failure);

    /// <summary>
    /// A verified, instantiated module held open across many invocations.
    /// </summary>
    /// <remarks>
    /// One instance and not one per call. A differential lane makes thousands of invocations and
    /// re-verifying for each would make the lane a measurement of the decoder; it also means the
    /// interpreter is asked to answer correctly on an instance that has already answered thousands
    /// of times, which is the state a per-call instance would never reach.
    /// </remarks>
    private sealed class Session : IDisposable
    {
        private readonly VmVerifiedArtifact artifact;
        private readonly VmInstance instance;

        private Session(VmVerifiedArtifact artifact, VmInstance instance)
        {
            this.artifact = artifact;
            this.instance = instance;
        }

        internal static Session? Open(
            VmRuntime runtime,
            byte[] module,
            string label,
            List<(string, bool, string)> results)
        {
            var descriptor = new VmArtifactDescriptor(
                WebAssemblyProfile.Id, 1, WebAssemblyProfile.SliceManifest, default,
                VmCallerIdentity.FromCanonicalIdentity("composition-wasm-harness://differential"));

            var verified = runtime.Verify(in descriptor, module, CancellationToken.None);

            if (!verified.TryGetArtifact(out var artifact))
            {
                results.Add((
                    $"{label}: verification",
                    false,
                    $"{verified.Outcome}/{verified.Reason}/" +
                    $"{verified.Diagnostics.ProfileDiagnosticCode} at offset " +
                    verified.Diagnostics.SourcePosition.ByteOffset));

                return null;
            }

            var instantiated = runtime.Instantiate(artifact, CancellationToken.None);

            if (!instantiated.TryGetInstance(out var instance))
            {
                results.Add((
                    $"{label}: instantiation",
                    false,
                    $"{instantiated.Outcome}/{instantiated.Reason}"));

                artifact.Dispose();
                return null;
            }

            results.Add((
                $"{label}: the module verifies and instantiates",
                true,
                $"{module.Length} bytes"));

            return new Session(artifact, instance);
        }

        internal Observed Call(string entry)
        {
            var request = new VmInvocationRequest(
                new VmUtf8Text(System.Text.Encoding.UTF8.GetBytes(entry)));

            var answered = instance.Invoke(in request, CancellationToken.None);

            if (WebAssemblyProfile.TryGetTrap(in answered, out var trap))
            {
                return new Observed(default, 0, trap.Kind, null);
            }

            if (!WebAssemblyProfile.TryGetResults(in answered, out var values))
            {
                return new Observed(
                    default, 0, null, $"{answered.Outcome}/{answered.Reason}, no results payload");
            }

            if (values.Count != 1 || !values.TryGetValue(0, out var value))
            {
                return new Observed(
                    default, 0, null, $"{values.Count} results, and one was expected");
            }

            return new Observed(value.Kind, value.Bits, null, null);
        }

        public void Dispose() => artifact.Dispose();
    }

    /// <summary>
    /// Whether an observation is the answer the oracle derived, and what differs where it is not.
    /// </summary>
    private static string? Compare(Answer expected, Observed observed, WebAssemblyValueKind kind)
    {
        if (observed.Failure is not null)
        {
            return observed.Failure;
        }

        if (expected.Trap is not null)
        {
            return observed.Trap == expected.Trap
                ? null
                : $"answered {Describe(observed)}, and the oracle derived a {expected.Trap} trap";
        }

        if (observed.Trap is not null)
        {
            return $"trapped {observed.Trap}, and the oracle derived 0x{expected.Bits:X}";
        }

        if (observed.Kind != kind)
        {
            return $"answered a {observed.Kind}, and a {kind} was expected";
        }

        return observed.Bits == expected.Bits
            ? null
            : $"answered 0x{observed.Bits:X}, and the oracle derived 0x{expected.Bits:X}";
    }

    private static string Describe(Observed observed) =>
        observed.Failure ?? (observed.Trap is not null
            ? $"a {observed.Trap} trap"
            : $"{observed.Kind} 0x{observed.Bits:X}");

    private static (string, bool, string) Verdict(
        string name, int cases, List<string> disagreements) =>
        disagreements.Count == 0
            ? (name, true, $"{cases} cases agreed with the oracle")
            : (name, false,
                $"{disagreements.Count} of {cases} cases disagreed: " +
                string.Join("; ", disagreements.Take(4)));

    private static byte[] Binary(byte opcode) => Instruction.Cat(
        Instruction.LocalGet(0), Instruction.LocalGet(1), [opcode]);

    private static byte[] Unary(byte opcode) => Instruction.Cat(Instruction.LocalGet(0), [opcode]);

    private static string Entry(string name, params string[] arguments) =>
        string.Concat(
            System.Text.Encoding.UTF8.GetByteCount(name).ToString(CultureInfo.InvariantCulture),
            ":",
            name,
            string.Concat(arguments));

    private static string I32Argument(int value) =>
        Argument("i32", value.ToString(CultureInfo.InvariantCulture));

    private static string I64Argument(long value) =>
        Argument("i64", value.ToString(CultureInfo.InvariantCulture));

    private static string F64Argument(ulong bits) =>
        Argument("f64", bits.ToString("X16", CultureInfo.InvariantCulture));

    private static string Argument(string type, string literal) =>
        $"{type}:{literal.Length.ToString(CultureInfo.InvariantCulture)}:{literal}";
}
