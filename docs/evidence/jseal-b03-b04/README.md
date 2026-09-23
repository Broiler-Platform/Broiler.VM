# BigInt arithmetic, bitwise and shift operations behind the gate: JSeal slices B03 and B04

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence**. Both halves of the
BigInt gate stay closed to public callers (decision
[JSD-0033](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0033-the-internal-bigint-value-and-the-gated-literal.md),
proposed, amended here as section 6): the wide manifest still refuses a BigInt literal by name, there
is no `BigInt` global, and nothing here is a BigInt conformance claim. Card B05 owns admission.

Checkout: a detached worktree at `484f389` of Broiler.VM plus the merged waves 1-3 patch
(`base-vm-w4`). Release build, Windows 11, .NET SDK 10.0.401. Node v24.17.0 was a diagnostic
comparison only. Pinned suite: `test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`.

## What changed

- **B03, arithmetic** (`JsEngine.cs`, `JsBigInt.cs`). `+ - * / % **`, unary `-` and the compound
  assignments convert with a new `ToNumeric` (left operand first, each once) and keep their Number
  path; anything else goes to `JsEngine.BigIntBinary`: mixing a Number with a BigInt is a
  `TypeError`, `/` and `%` by `0n` and a negative exponent are `RangeError`s, division and remainder
  truncate toward zero, and a result wider than `JsBigInt.MaximumBits` (2^20 bits) is a
  `RangeError`. Products, powers and left shifts that their operand widths already prove too wide
  are refused before the result is allocated; `0n`, `1n` and `-1n` raised to any exponent answer at
  once. Unary `+` keeps `ToNumber` (its `TypeError` is the specification's).
- **Update expressions** (`JsCompiler.cs`). The format has no `ToNumeric` instruction and no opcode
  was added: a **gated** compilation lowers `++`/`--` (name, member, computed, private and `super`
  forms) as two `Negate` instructions (exactly `ToNumeric`) and a `typeof` branch adding `1n` or `1`.
  An ungated compilation emits the bytes it always did.
- **B04, bitwise and shifts.** `& | ^ ~ << >>` are two's-complement at any width; `>>>` on BigInts
  and every mixed pair are `TypeError`s; a negative shift count reverses the direction; a right
  shift past the value's width answers `0n` or `-1n` for any count. `JsBigInt.AsIntN` and
  `JsBigInt.AsUintN` implement `BigInt.asIntN`/`asUintN` internally; **the global is deferred to
  B05**, because the gate exposes no `BigInt` global.
- **Charges and cancellation.** Every operation charges fuel before each step (linear operations
  one unit a word; multiplication, division, each power step and each formatting split
  `a*b/16 + 32(a+b)` words, measured against about 21 ns a unit on this machine). `ToString` of a
  BigInt is now divide and conquer over squared powers of ten with 4,096-bit leaves: the widest
  value converts in about 0.2-0.4 s instead of one uninterruptible 3.2 s base-class-library call,
  its longest uninterruptible step is about 52-69 ms, and it needs about 26.2 million units of
  fuel (measured by binary search), inside the default allowance of 50 million.
- **Constant folding audit.** Neither the wide lowering nor the parser folds constants; every
  operator is evaluated by the runtime, so there is nothing for a folder to disagree with.
- No new opcode, diagnostic code, decision number, product file or public API. No roadmap ledger
  was edited.

## Executed

- `dotnet build Broiler.VM.slnx -c Release`, `BROILER_ASSURANCE_WRITE=1 dotnet test ... --no-build`,
  rebuild, `dotnet test Broiler.VM.slnx -c Release --no-build`. Base: Contract 267/267,
  Architecture 256/256. After: Contract 267/267, Architecture 256/256.
- `Broiler.VM.Composition.JavaScript.SliceCompiler.exe --checks --verbose`: the pass count moves
  from 327 to 349, the two `not-run` x86-64-sysv rows unchanged, no row failing
  ([bigint-checks.txt](bigint-checks.txt)). 18 program rows and 4 resource rows are new; one B01 row
  lost the four operations it no longer refuses.
- **Failing first** ([checks-before.txt](checks-before.txt)): with the new rows and the base
  product code, 19 rows failed (every B03-B04 row except the two "Number unchanged" controls); the
  `as-int-n` row could not run because its accessor target did not exist yet.
- **Falsification** ([mutations.txt](mutations.txt)): formatting through one base-class-library call,
  size-squared charge removed, floor-modulo remainder and reference equality each fail rows. The
  first form of the cancellation row did not detect the uninterruptible conversion (cancellation is
  also seen after it); the row now also bounds the latency at one second, and then detected it
  (4,020 ms). Reference equality - which no B01 row could detect - now fails the equality row.
- CLI acceptance: 219 of 219 command lines answered as declared, before and after. Host-surface
  lane (`--host-surface`): every check passed. Corpus replay (`ExecutionOnly --corpus
  src/tests/corpus/js-1 --verbose`): 152 entries replayed to their recorded answers.
- Differential: new probe `the-bigint-arithmetic-boundary.js` (24 cases) passes against Node with
  four declared divergences (cases 21-24: the wide manifest still refuses a BigInt literal by name
  inside eval). Cases 1-20 pin the Number paths the new dispatch now runs through; they were not
  run on the base build. `the-bigint-literal-boundary` still passes. The whole lane
  (`python eng/run-differential.py --timeout 60`) has one retained mismatch, the known case 36 of
  `the-later-library-methods` (console code page).
- A formatting self-check (722 values - powers of ten and two, their neighbours, random widths to
  2^20 bits, long zero runs) agrees with `BigInteger.ToString` on every value.

## Test262

- Public harness, before (base tree built in a scratch directory) and after, identical per-variant
  reports on three subsets ([test262-comparison.txt](test262-comparison.txt)): the operator
  directories (pass count 1,839, fail count 60, unsupported 160, both times), the B01-B02 subset, and all of
  `test/language/expressions` plus `statements/for`. The well-formed BigInt tests stay
  `unsupported` there, as they must while the gate is closed; the retained floor is unaffected.
- **Through the gate** ([gated-test262.txt](gated-test262.txt), runner
  [gated262-runner.cs.txt](gated262-runner.cs.txt), a scratch program outside every project): the
  83 BigInt-feature files of the B03-B04 operator directories, with the suite's harness files, give
  130 variants that pass and 36 that fail. All 36 are the 18 `bigint-wrapped-values.js` /
  `bigint-non-primitive.js` files, which build `Object(2n)` - card B05's `ToObject`.

## Not exercised

- The whole Test262 suite; the native execution form (pre-existing instantiation fault here);
  Native AOT publishing; the fuzz host; Linux.
- Relational comparison, `==` with another type, `BigInt()`/`BigInt.asIntN` as globals, wrappers,
  JSON and the host crossing (B05-B06). Eval code inside a gated program is compiled without the
  gate, so a BigInt literal there is refused and `x++` on a BigInt there is a `TypeError`.
- Fuel charges are calibrated on this machine only; `LiveBytes` is not charged for BigInt results,
  as it is not for strings.
