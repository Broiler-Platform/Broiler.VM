<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0033 - The internal BigInt value, its constant, and the gate that keeps both unadvertised

**Status:** Proposed, 2026-09-21. **Owner decision pending.** The code this record describes is in
the tree: an internal value kind, an internal compile gate, a constant tag, and a gate surface that
no shipped composition admits. **The wide manifest still refuses a BigInt literal by name, there is
no `BigInt` global, and nothing here is a BigInt claim.** Taking the record would accept the
representation, the gate, the budgets and the version policy below as the basis for cards B03-B06;
not taking it leaves an internal value kind that no public compilation can produce.
*(Amended 2026-09-21, JSeal B05: the paragraph above describes the record as B01-B04 left it and is
kept as written. Section 7 admits the surface: the wide manifest lowers a BigInt literal, the
`BigInt` global exists in every realm whose composition admits `broiler.javascript.bigint`, and the
descriptor admitting every surface admits it. Taking the record now also accepts section 7.)*

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It answers cards B01 and B02 of the Broiler.JSeal VM feature
plan (`docs/roadmap.vm-features.md` in that repository). B01 asks for a representation "compatible
with the profile's portability/AOT rules, value layout, budgets, and artifact versions" and an audit
of every exhaustive value-kind switch; B02 asks for exact literals behind a gate, with the existing
named refusal kept for every manifest that does not admit BigInt.

**Context.** On 2026-09-08 the wide front end was found evaluating `9007199254740993n` as the
Number `9007199254740992`; the named refusal was restored in `JsParser.cs` (JSC-207), which moved
1,990 Test262 variants to `unsupported`. The BigInt64 typed arrays are absent for the same reason
(`JsBinary.cs`), and [JSD-0032](0032-the-internal-structured-clone-carrier.md) records BigInt as
"not in the realm".

---

## 1. What was true before, verified

| Question | Answer | Where |
|---|---|---|
| Does `JsType` have a BigInt kind? | No. Eight kinds, `Empty` to `Symbol` | `JsValue.cs` |
| What did a well-formed `1n` do? | Refused at compile time, `2104 ConstructOutsideManifest`, "a BigInt literal" | `JsParser.cs` `ParsePrimary` |
| What did a malformed `1.5n`, `01n` or `1e3n` do? | The same `2104` refusal: the tokenizer accepted the suffix on any numeric spelling, so a syntax error was reported as an unimplemented construct | `SliceTokenizer.cs` `FinishNumeric` |
| What did `{ 9007199254740993n: 1 }` do? | **Silently named the key `"9007199254740992"`.** The property-key path read the literal through its double and was not covered by the 2026-09-08 refusal | `JsParser.cs` `PropertyKey` |
| What would a BigInt value have met in the switches? | `ToNumber`, `ToString` and `ToObject` fall to an arm that calls `ToPrimitive`, which returns a primitive unchanged, so the first two would recurse without end; `typeof`, the host crossing and the clone walk would cast it to an object | `JsEngine.cs`, `JsValue.cs`, `JsHostRealm.cs`, `JsRealm.Clone.cs` |
| Does any rule forbid `System.Numerics.BigInteger`? | No. It is framework (rule B1), reaches no dynamic entry point (rule B5), and is trimming- and Native-AOT-safe | `ArchitectureRules.cs` |

## 2. The decision

1. **Representation.** A BigInt is an internal `JsBigInt`, an immutable class holding a
   `System.Numerics.BigInteger`. The class owns the width ceiling, the size a charge is computed
   from, and the one decoding of a constant, so no caller holds a bare `BigInteger` it could forget
   to bound. Equality and hashing are mathematical; identity means nothing.
2. **Value layout.** `JsType.BigInt = 8`. A `JsValue` stays the 24-byte tag, double and reference:
   the reference is the `JsBigInt` and the double is unused. No existing value changes its layout
   or its tag, and nothing that stores a `JsValue` needs a new field.
3. **The gate has two halves, and both are closed to every public caller.** Each is opened only
   through an internal member that the slice compiler's checks reach with an `UnsafeAccessor`.
   - *Compile:* `JsCompileRequest.AdmitsBigIntLiterals` is internal and false. Set (through the
     internal `AdmittingBigIntLiterals`), a well-formed literal is lowered exactly; clear, it meets
     the refusal it has always met. Only the slice compiler's checks open it, through one
     `UnsafeAccessor`, as JSD-0032 section 2 records for the clone carrier.
   - *Verify:* a BigInt constant is admitted only when the artifact declares
     `broiler.javascript.bigint` (`JsSurfaces.BigInt`). The name is **known and deliberately not in
     `JsSurfaces.All`**, so `JavaScriptProfile.Descriptor` - which admits every advertised surface
     and is what the end-user host and the conformance harness register - declines it
     (`1608 SurfaceOutsideComposition`). **Every public descriptor door - `DescriptorAdmitting`,
     `DescriptorReEmittingWith` and `DescriptorHostingRealms` - refuses the name with an
     `ArgumentException` naming BigInt**, so no embedder can build a descriptor that admits it. The
     one descriptor that does is built by the internal
     `JavaScriptProfile.DescriptorAdmittingTheBigIntGate`, which only the checks reach.
     *(Amended 2026-09-21 after review: as first written, `DescriptorAdmitting` accepted the name,
     so any embedder could open the verify half, and this paragraph's "closed" was wrong for it.)*
     The name `JsSurfaces.BigInt`, the tag `JsFormat.ConstantTag.BigInt` and the writer
     `JsArtifactWriter.BigIntConstant` are public, as every other surface name, tag and writer of the
     format is: they describe bytes and admit nothing.
4. **The constant.** Tag `7` (`JsFormat.ConstantTag.BigInt`): one sign byte, a variable-length
   byte count, then the magnitude least significant byte first. **Canonical**: zero is sign 0 and no
   bytes, otherwise the last byte is not zero, so one integer has one spelling and `-0n` cannot be
   written. **Bounded**: at most `JsFormat.CeilingBigIntConstantBytes` = 8,192 bytes (65,536 bits),
   refused on the declared length before a byte is read. The lowering interns BigInt constants by
   their decimal spelling, so `0x10n` and `16n` share one entry.
5. **Artifact-version policy.** The format version stays 2. **A well-formed** BigInt constant in an
   otherwise valid artifact that does not declare the gate is refused with `1301 UnknownConstantTag`
   naming the constant - the code every build before the tag answered - so no existing artifact
   changes meaning and an older reader refuses the new bytes by name instead of misreading them.
   The answer is not identical in every respect: the payload is decoded first, so a malformed or
   over-wide one answers `1305` or `1201` even without the declaration, and the `1301` is issued at
   link time, after every section is read, so a defect in a later section is now reported ahead of
   it where an older build stopped at the tag byte. The numeric manifest refuses
   the tag outright with the same code; format version 1 never had it. A non-canonical payload is
   `1305 MalformedBigIntConstant` (registry revision 14); an over-wide one is
   `1201 DeclaredMaximumTooLarge`.
6. **Budgets.** Decoding a constant is linear in a payload the bounded reader already charges per
   byte. `ToString` charges its decimal-digit bound before converting. Equality charges the smaller
   operand's word count, as String equality charges the shorter length. `JsBigInt.MaximumBits`
   (1,048,576 bits, 128 KiB) is the realm's value ceiling, which card B03's arithmetic must refuse
   past and charge in proportion to; no operation at this build can create a value wider than a
   constant, so the ceiling is stated rather than yet exercised.
7. **Source.** The tokenizer refuses every spelling the grammar gives no BigInt - a fraction, an
   exponent, a legacy octal, any leading zero, a second suffix, a digit or identifier after the
   suffix - as `2003 MalformedNumericLiteral` at the literal, **with or without the gate**, because
   a malformed literal is a syntax error rather than an absent feature. A gated literal wider than
   the constant ceiling is refused at the literal as `2104`, bounded on the digit count before any
   conversion. A BigInt property key is its exact decimal spelling with or without the gate: it is a
   String, and no BigInt value is made.
8. **What a gated BigInt answers, and what it refuses.** No operation reads a BigInt as a Number.

   | Operation | At this build | Owner |
   |---|---|---|
   | `typeof` | `"bigint"` | B01 |
   | `ToBoolean` | false for `0n` only | B01 |
   | `===`, `!==`, `SameValue`, `SameValueZero`, Map/Set keys | mathematical | B01 |
   | `ToString`, `ToPropertyKey`, string concatenation, templates | exact decimal digits | B01 |
   | `+ - * / % **`, unary `-`, `++`, `--` and their compound assignments | exact BigInt arithmetic; mixing with a Number is a `TypeError`; a zero divisor, a negative exponent and a result past the ceiling are `RangeError`s (section 6) | B03 |
   | `& \| ^ ~ << >>` and their compound assignments | two's-complement BigInt operations; mixing is a `TypeError`; `>>>` is a `TypeError`; an oversized left shift is a `RangeError` before it allocates (section 6) | B04 |
   | `asIntN`, `asUintN` | internal operations only (`JsBigInt.AsIntN`, `JsBigInt.AsUintN`); the `BigInt` global exposing them is B05's | B04 / B05 |
   | `ToNumber` (unary `+`, relational comparison, `Number(x)`, `Math`) | `TypeError` naming BigInt - the specification's own answer for `ToNumber` (and for unary `+`) | B05 |
   | `==` against another type | `TypeError` naming BigInt, never `false` | B05 |
   | `ToObject`, property access, method calls | `TypeError` naming BigInt: `BigInt.prototype` does not exist | B05 |
   | `JSON.stringify` | `TypeError` naming BigInt, never an omitted value | B05 |
   | Host surface (`JsHostRealm.Wrap`) | `TypeError` at the crossing; a BigInt the guest **throws** reaches the host as a `JsHostThrowException` carrying a `TypeError`, not the value *(since B06, 2026-09-22: the exact value, JSD-0024 section 19)* | B06 |
   | Structured clone | refused, `Unrepresentable` *(since B06, 2026-09-22: carried, JSD-0032 section 5)* | B06 / JSD-0032 |
   | Typed-array element write with no engine to hand | the invocation is aborted by name (unreachable: every guest write converts first) | B05 |

## 3. What was considered and rejected

- **Rejected: a hand-written limb array.** It would be a second arbitrary-precision implementation
  to get right, and B03 is where a wrong carry would surface as a plausible wrong answer. The
  base class library's type is allowed by every rule this profile holds itself to.
- **Rejected: storing the `BigInteger` directly in the value's reference field.** It would box a
  struct no code bounds, and every caller would have to remember the ceiling.
- **Rejected: a small-integer fast path in the double.** It would give one mathematical value two
  representations, which every equality, hash and clone would then have to reconcile. It can be
  added later behind `JsBigInt` without changing any caller.
- **Rejected: a new manifest identity for the gated path.** A header manifest is a claim a reader
  reads as a surface; a gate surface that no shipped descriptor admits says exactly "declared, and
  declined here" and needs no second verifier path.
- **Rejected: adding the gate to `JsSurfaces.All`.** Every root registering
  `JavaScriptProfile.Descriptor` would then verify BigInt artifacts, which is admission.
- **Rejected: a new format version.** A tag an older reader already refuses by name is not a
  change of meaning for any existing artifact, which is what a version break is for.

## 4. Evidence in the tree

`BigIntChecks.cs` in the slice-compiler root: 38 `--checks` rows (gated compile, verify and
execute; both gate halves, including every public descriptor door refusing the gate; a thrown
BigInt reaching the host as a host throw; malformed spellings at their positions, gated and
ungated; over-wide literals; the numeric manifest). Three retained corpus entries `wide-a-bigint-constant-*` pin the
three verifier refusals. `src/tests/differential/the-bigint-literal-boundary.js` pins the end-user
host's side. The local validation record is `docs/evidence/jseal-b01-b02/README.md`.

## 5. What this leaves open, and what would falsify it

- **Mathematical equality between two distinct instances is not observable from guest code yet:**
  constants are interned, so one value is one instance until B03 computes new ones. The
  implementation compares integers; B03 must add the guest-level check. *(Closed by B03, section 6:
  computed values are distinct instances, and `===`, `Object.is`, `Set`, `Map` and `includes` are
  checked against constants of the same integer.)*
- The superlinear cost of formatting a wide value is not charged as such. **Measured baseline for
  B03** (2026-09-21, Release, this machine, default fuel allowance): converting the widest constant
  (65,536 bits, 19,729 digits) takes about 4 to 7 ms, and a loop of such conversions exhausts the
  allowance after about 5.0 s, where a loop of `String('x')` exhausts it after about 1.05 s - so the
  linear digit charge buys roughly 4.7 times the wall time of ordinary charged work. It is bounded
  by the constant ceiling at this build; B03 must charge a size-squared term (or measure one away)
  once values up to `MaximumBits` can exist.
- JSD-0032's matrix row "BigInt: not in the realm" stays true of every shipped composition.
- **Falsified if** any public compilation lowers a BigInt literal, any public descriptor door
  returns a descriptor admitting the gate, any operation answers a Number, `false` or an omitted value for a
  BigInt operand where the table above says refused, or a constant decodes to an integer other than
  the one its text spelled.

## 6. Amended 2026-09-21: arithmetic and bitwise operations (JSeal B03-B04)

Still behind both closed halves of the gate; the wide manifest's named refusal is unchanged for
every public compilation, and nothing here is a BigInt claim.

1. **Operands.** The arithmetic, bitwise and shift operators convert with `ToNumeric` (left operand
   first, each converted once), take the Number path they always took when both are Numbers, and
   otherwise call one BigInt dispatch (`JsEngine.BigIntBinary`): a Number and a BigInt together are a
   `TypeError` before anything is computed; `>>>` on two BigInts is a `TypeError`; `/` and `%` by
   `0n` and `**` with a negative exponent are `RangeError`s. Unary `-` and `~` convert with
   `ToNumeric`; unary `+` keeps `ToNumber`, whose `TypeError` is the specification's answer. No
   operand is ever read through a double.
2. **Update expressions.** The format has no `ToNumeric` instruction, and adding opcodes to the
   shared instruction table for a gated path was weighed and refused. A **gated** compilation lowers
   `x++`, `--x` and their member, private and `super` forms as two `Negate` instructions (exactly
   `ToNumeric`: one conversion, then a sign flip back that runs no guest code) and a `typeof` test
   that adds `1n` to a BigInt and `1` to anything else; it declares the gate surface, since it holds a
   BigInt constant. An **ungated** compilation lowers updates exactly as before, so no artifact that
   does not declare the gate changes a byte.
3. **The ceiling.** A computed result wider than `JsBigInt.MaximumBits` (2^20 bits, measured as
   `BigInteger.GetBitLength`, the shortest two's-complement width without the sign bit) is a
   `RangeError` ("Maximum BigInt size exceeded"). A product, a power or a left shift whose operand
   widths already prove the result too wide is refused before the result is allocated; a right shift
   past the value's width answers `0n` or `-1n` without shifting, whatever the count; a power of
   `0n`, `1n` or `-1n` answers for any exponent.
4. **Charges** (`JsBigInt.LinearCost`, `ProductCost`, and the formatting leaf cost), charged before
   each step to the engine's fuel, which is also where cancellation is polled. Linear operations
   charge one unit a word. A multiplication, a division, each squaring or multiplication of a power,
   and each split of a decimal conversion charge `a*b/16 + 32(a+b)` for operands of `a` and `b`
   words - the size-squared term section 5 asked for. Measured on 2026-09-21 (Release, this machine)
   against about 21 ns a unit of ordinary work, that covers the measured time by a factor of about
   1.5 to 20 at every size from 2 to 16,384 words.
5. **Formatting.** `ToString` of a BigInt is now divide and conquer over squared powers of ten,
   with leaves of at most 4,096 bits handed to the base class library. The widest value (2^20 bits)
   converts in about 0.21 s where the base class library's single call takes about 3.2 s, its
   longest uninterruptible step (the first division) is about 52 ms, and its charge (roughly 26
   million units) fits the default allowance; under a 5,000,000-unit ceiling it is exhausted.
6. **Bounded cancellation.** The longest step that runs without a charge between it and the next is
   one base-class-library multiplication or division at the ceiling: about 27 ms for a 2^19-bit
   square and 69 ms for a 2^20-bit value divided by a 2^19-bit one on this machine.
7. **Not changed:** relational comparison, `==` against another type, `ToObject`, property access,
   `JSON.stringify`, the host crossing and the clone walk still refuse by name (B05-B06). Eval code
   inside a gated program is compiled without the gate, so a BigInt literal there is refused and an
   update of a BigInt there is `ToNumber`'s `TypeError` - a refusal, never a Number.
8. **Constant folding audit.** Neither the wide lowering nor the parser folds constants: a
   literal is loaded and every operator is an instruction evaluated at run time, so no compile-time
   arithmetic can disagree with the runtime's. (The numeric manifest refuses BigInt outright.)
9. **Falsified if** a gated operator answers a Number for a BigInt operand or a BigInt for mixed
   operands, a result past the ceiling is answered, an operation proven too wide by its operands'
   widths allocates its result, a quotient or remainder rounds other than toward zero, a right shift
   rounds other than toward negative infinity, or a formatting of a wider value is charged less.
   Evidence: `BigIntChecks.cs` rows `bigint/b03/...` and `bigint/b04/...`, the differential probe
   `the-bigint-arithmetic-boundary.js`, and `docs/evidence/jseal-b03-b04/README.md`.

## 7. Amended 2026-09-21: conversion, comparison and public admission (JSeal B05)

**This section admits BigInt.** Everything sections 2 to 6 kept behind the gate is public from this
build: a BigInt literal under the wide manifest, the `BigInt` global and `BigInt.prototype`, BigInt
objects, and every operation section 2's table left for B05. The record is still proposed and
unsigned; nothing here advances a milestone.

1. **How it is admitted: the identity already minted, advertised.** `broiler.javascript.bigint`
   joins `JsSurfaces.All`, so `JavaScriptProfile.Descriptor` - what the end-user host and the
   conformance harness register - admits it, and `JavaScriptProfile.BigIntManifest` names it for a
   composition that wants to decline it with `DescriptorAdmitting`. An artifact declares it where it
   writes a BigInt constant and where it reads the `BigInt` global (`JsSurfaces.BigIntGlobals`, as
   `eval` declares the dynamic surface); `typeof BigInt` declares nothing and answers `"undefined"`
   under a composition that declined it. The compile half follows the manifest
   (`JsCompileRequest.AdmitsBigIntLiterals` is `Manifest == Wide`); the numeric manifest keeps its
   named refusal. Both internal doors - `JsCompileRequest.AdmittingBigIntLiterals` and
   `JavaScriptProfile.DescriptorAdmittingTheBigIntGate` - are gone, and the public descriptor doors
   no longer refuse the name.
   A declining composition that admits the dynamic surface meets a BigInt reached through `eval`
   or the `Function` constructor as the guest `EvalError` every unanswered guest load is: the loaded
   source compiles under the wide manifest and the mediator refuses its artifact at verification
   (`InvalidArtifact/UnsupportedFeatureManifest`), so no value is ever produced (check row
   `bigint/b05/a-declining-composition-refuses-bigint-through-eval`).
   **Why this option and not the other the card allows.** A second manifest identity for the header
   was rejected in section 3 for the reason that still holds: a header manifest is a claim a reader
   reads as a surface, and the profile already expresses "declared, and declinable per
   composition" with an optional surface. Keeping the surface out of `All` and advertising it only
   to compositions that ask would have left the conformance harness declining it, so the wide
   run's `unsupported` column would go on naming "a BigInt literal" as an absence the realm no
   longer has - a record that stopped being true. Admitting it where every other optional surface
   is admitted keeps each record truthful: the surface is declared by name, declinable by name, and
   scored.
2. **What the surface admits, and what it does not.** The language's BigInt except, by name:
   `BigInt64Array` and `BigUint64Array` (card B07; they stay on the absent-globals list), the
   DataView BigInt accessors (card B08) - *both added on 2026-09-22 by JSeal B07-B08, see section
   8* - the host crossing and the structured clone (card B06; both
   still refuse a BigInt - and now a BigInt object - by name; *amended 2026-09-22: both carry it
   since B06, JSD-0024 section 19 and JSD-0032 section 5*), and anything needing a second realm
   (`$262.createRealm` is refused, so the cross-realm tests fail as every other cross-realm test
   does). `BigInt.prototype.toLocaleString` is `toString()` with no radix, the FIXED row of
   [JSD-0027](0027-intl-scope-and-data-strategy.md) section 6, amended in the same change.
3. **Section 2's table, the B05 rows as they now answer.** `ToNumber` of a BigInt is still the
   specification's `TypeError` (unary `+`, `Math`, `isNaN`, every `ToNumber` caller); `Number(x)`
   converts through `ToNumeric` and rounds to nearest, ties to even (`JsBigInt.ToNumber`, not the
   base class library's conversion, which truncates: measured to answer `Number.MAX_VALUE` for
   `2n ** 1024n - 2n ** 970n`, where the language answers `Infinity`). `BigInt(v)` takes an
   integral Number exactly (`NumberToBigInt`, a `RangeError` otherwise) and everything else through
   `ToBigInt`: Booleans, BigInts and `StringToBigInt` of a String (a `SyntaxError` outside the
   grammar), and a `TypeError` for `undefined`, `null`, a Symbol and - inside `ToBigInt` - a
   Number. `new BigInt()` is a `TypeError`. `asIntN`/`asUintN` are `ToIndex` then `ToBigInt`, then
   B04's operations. `ToObject` makes a BigInt object whose class is `Object` (its tag is
   `BigInt.prototype[Symbol.toStringTag]`); property access walks `BigInt.prototype`.
   `==` against a Number is exact (`JsBigInt.CompareToNumber`: an integral Number compared as its
   integer, a fraction by its floor), against a String through `StringToBigInt`, against a Boolean
   through its Number, against an object through one `ToPrimitive`. Relational comparison is
   exact the same way, a String beside a BigInt read with `StringToBigInt`, `NaN` and a
   non-integer String answering `undefined`, i.e. `false`. `JSON.stringify` consults `toJSON`
   (so `BigInt.prototype.toJSON` works), then the replacer, unwraps a BigInt object, and throws
   `TypeError` for a BigInt. Nothing reads a BigInt through a double.
4. **A String past the ceiling is exact too.** `StringToBigInt` reports a value wider than
   `MaximumBits` without refusing: `BigInt(text)` raises the ceiling's `RangeError`, `==` answers
   `false` (no value in the realm equals it) and a relational comparison answers by its sign. A
   decimal text of more than 315,653 significant digits is known too wide before a digit is
   converted.
5. **Budgets.** Parsing charges the text's length, then - for decimal text - a leaf cost per piece
   of at most 1,024 digits and `ProductCost` per join, split by squared powers of ten as the
   formatting of section 6 is: measured on 2026-09-21 (Release, this machine), the base class
   library parses 315,653 digits in one uninterruptible call of about 258 ms, and the split parse of
   315,000 digits takes about 215 ms with a charge between steps; under a 5,000,000-unit ceiling it
   is exhausted (check row `bigint/b05/a-long-decimal-text-is-charged-for-its-square`).
   `toString(radix)` is linear for a power-of-two radix and divide and conquer otherwise (the
   widest value in radix 7 took about 381 ms, inside the default allowance). Comparisons charge the
   narrower operand's words; `Number(x)` one pass.
6. **Format compatibility: the version stays 2, and three opcodes are added.** A BigInt can reach
   any wide update expression now, and `x++` is `Number::add(x, 1)` or `BigInt::add(x, 1n)` by the
   operand's type, known only at run time. B03's gated lowering answered that with two `Negate`s
   and a `typeof` branch; as every wide program's path, a branch in every loop counter was refused,
   and the format gained `ToNumeric` (`0xB0`), `Increment` (`0xB1`) and `Decrement` (`0xB2`), each
   one byte, popping one value and pushing one, run in a block step like `Negate` (they reach guest
   code only behind `ToPrimitive`'s object test). The wide lowering of every update is now
   `ToNumeric` then `Increment`/`Decrement`; the numeric manifest keeps `ToNumber`, the constant
   `1` and `Add`, byte for byte. **An older reader refuses the new bytes by name**, as
   `UnknownOpcode` at the instruction, so no artifact of this build is misread by an older one; an
   artifact of an older build keeps its meaning here, and its `x++` on a BigInt another artifact
   made is `ToNumber`'s `TypeError` - a refusal, never a Number. The retained corpus regenerates
   byte for byte (no corpus program holds an update expression), so it was not rewritten.
7. **The floor.** The whole-suite ratchet `src/tests/conformance/floors/test262-wide.floor` is
   re-based by hand, as its header requires when the engine rather than the suite moves: its
   `atMost unsupported 1990` - which counted exactly the variants refused as "a BigInt literal" -
   is retired with its reason and replaced by the whole run's new figure. The other rows were not
   crossed and are left as they are. The figures and the run are in
   `docs/evidence/jseal-b05/README.md`.
8. **Evidence.** `BigIntChecks.cs` rows `bigint/b05/...` (and every earlier row moved off the
   internal doors onto the public path), the differential probe `the-bigint-public-surface.js`
   (62 cases, one declared divergence: the B07-B08 absences), the amended retained answers of
   `the-bigint-literal-boundary.js` and `the-bigint-arithmetic-boundary.js`, and the pinned
   Test262 selection recorded in `docs/evidence/jseal-b05/README.md`.
9. **Falsified if** a public compilation under the numeric manifest lowers a BigInt literal, a
   composition that declined the surface verifies an artifact holding a BigInt constant or naming
   `BigInt`, `ToNumber` of a BigInt answers anything but a `TypeError`, `Number(x)` rounds a BigInt
   other than to nearest with ties to even, a mixed comparison or equality answers other than the
   mathematical values do, `BigInt(x)` answers a value for a non-integral Number or a String outside
   `StringIntegerLiteral`, `JSON.stringify` answers anything but a `TypeError` for a BigInt that no
   `toJSON` or replacer replaced, or an update of a Number answers other than `x + 1` or `x - 1`.


## 8. Amended 2026-09-22: the BigInt typed arrays and DataView accessors (JSeal B07-B08)

The two exceptions section 7 item 2 named by card are closed. The record is still proposed and
unsigned; nothing here advances a milestone.

1. **Two element kinds and two surfaces.** `JsElementKind.BigInt64` and `BigUint64` (eight bytes,
   `JsElements.HoldsBigInts` true) are built only when a composition admits this identity and the
   binary one: an element read makes a BigInt, which a realm declining BigInt may never hold.
   `BigInt64Array` and `BigUint64Array` are on both `JsSurfaces.BinaryGlobals` and
   `JsSurfaces.BigIntGlobals`, and naming either declares both surfaces, so a composition declining
   either refuses the artifact at verification (1608). The four `DataView` accessors are built
   under the same condition.
2. **The element currency is a value.** Typed-array elements now move as a `JsValue` (a Number, or
   a BigInt for the two kinds), never as a `double`; the Number read and write paths refuse a
   BigInt kind by name (`InternalDefect`) rather than rounding. Every conversion into an element is
   `JsEngine.ToElementValue`: `ToBigInt` for a BigInt kind (a Number is a `TypeError`), `ToNumber`
   otherwise; a value wider than 64 bits is narrowed once and charged for its words. A write stores
   the value modulo 2**64 (`ToBigInt64`/`ToBigUint64`); copying between the two content types -
   `set`, construction from a typed array, a species result - is a `TypeError`. The engine-less
   `JsElements.ElementOf`, reached through `JsObject.SetOwnProperty`, keeps a BigInt kind exact for
   a BigInt or a Boolean and ends the invocation for anything else; so every guest-reachable write
   must reach the typed array's `[[DefineOwnProperty]]` or `[[Set]]` instead. The B07 review found
   one that did not - `JSON.parse`'s reviver - and it now stores by `CreateDataProperty`
   (a refusal ignored, a conversion error propagated); the audit covered the `SetOwnProperty` call
   sites whose target a guest can choose.
3. **Not changed:** `Atomics` and `SharedArrayBuffer` stay absent (JSD-0028); the host crossing and
   the clone of a BigInt value stay card B06's. A BigInt typed array is cloned as any view is
   (JSD-0032's carrier names it by constructor), and a realm without the kind refuses the name.
4. **Falsified if** a BigInt kind stores or answers a Number, a Number kind accepts a BigInt, an
   element does not read back as its value modulo 2**64 in the kind's signedness, a copy between
   the two content types converts rather than throws, or a composition declining either surface
   verifies an artifact naming `BigInt64Array` or `BigUint64Array`, or any guest-reachable
   element write ends the invocation where the language owes a catchable `TypeError` or a stored
   value. Evidence:
   `docs/evidence/jseal-b07-b08/README.md`.
