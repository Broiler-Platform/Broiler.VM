# Area: regular expressions

**Delivered by:** [SJL-11](../milestones/sjl-11-regexp.md). **Owner:** Broiler.RegEx (package
`Broiler.Regex`). **Status:** design; its verdict is [JSD-0081](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md), all six sub-areas accepted and placed
with Broiler.RegEx under its owner conditions, and decision SJL-D-14 recorded, on 2026-10-21. Behaviour statements
about Broiler.Regex come from its source at `c1422f4d`, which is the commit its preview.4 package was built from.
*(Revised 2026-10-25, at SJL-11.1: the status read "design.")*

*(Added 2026-10-25, at SJL-11.1, by [JSD-0081](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md), rows of bundle `sjl-11-001`'s [correspondence](../../evidence/sjl-11-001/correspondence.md).)* The
correspondence re-read this design's statements from code at Broiler.VM `aaa39fb`, Broiler.JS `1f91b8f1` and
Broiler.Regex `c1422f4d`, and ran every behavioural cell. Each statement that no longer held is corrected beside it,
quoting what it read (JSD-0081 decision D40).

---

## 1. Scope

| Shared (the engine) | Stays in each JavaScript engine |
|---|---|
| Pattern syntax and early errors (Annex B included), compilation, matching, flags `d g i m s u v y` as **inputs**, named groups (duplicates included), lookbehind, modifiers, `v`-mode set operations and string properties, an anchored match, offset-only results, flag parsing, `RegExp.escape` text, escaping of the `source` text, the work budget | The `RegExp` object; `lastIndex` (including its adjustment inside a surrogate pair under `u`); `exec`'s result objects, `groups` and `indices`; the `Symbol.match`, `matchAll`, `replace`, `search` and `split` protocols; species; `GetSubstitution`'s evaluation; Broiler.JS's legacy `RegExp` statics |

---

## 2. Current state

| | Broiler.VM `JsRegExpMatcher` (Format) | Broiler.Regex 0.1.0-preview.4 | Broiler.JS's integration |
|---|---|---|---|
| Design | Parser → nodes → an instruction array → a backtracking interpreter with explicit heap frames and an undo trail | Parser → a public syntax tree → continuation-passing closures; explicit stack only in the general quantifier | .NET `Regex` through a large translator **by default**; Broiler.Regex only for `u`/`v` patterns and a few known-gap shapes; **silent fallback to .NET** when Broiler.Regex cannot parse a pattern |
| Literal validation | The compiler runs the **same** parser, "so that the two cannot disagree about what a pattern is" | (no compiler) | `RegExpValidator` compiles with .NET; `RegExpUnicodeValidator` in `Runtime`; **an invalid pattern becomes division** in the scanner; braced `\u{…}` escapes are rewritten at lex time, which changes `source` |
| Work budget | Every instruction, character run, property weight, back-reference length and prefilter skip is charged, in blocks, through `JsRegExpCharge` into the engine's fuel, which polls cancellation and the wall clock. Frames and trail are bounded, and overflowing them gives `JsAbort(Exhausted)`. | **A fixed step allowance, reset at each start position.** It is counted only in quantifier loops. Exhaustion returns **no match**, or, in the greedy fast path, **a shorter match**. No callback, no cancellation, no timeout. | .NET `Regex` with no timeout and no cancellation. On its `u` and `v` path, Broiler.Regex's two stops: its stack guard falls back to .NET's answer or to no match (row B5), and its spent budget answers no match, a shorter match, a wrong index or a flipped look-around (rows B1 to B3) *(Revised 2026-10-25, at SJL-11.1, by JSD-0081's correspondence; read ".NET `Regex` with no timeout and no cancellation; an overflow falls back to .NET or to no match")* |
| Compile cost | Charged in proportion to the source length only, so a `v`-mode class expansion is under-charged *(confirmed 2026-10-25, at SJL-11.1, by row B7's runs: each expansion needs exactly the allowance a class of plain characters of its length needs; the cell ended with the mark "(reading)")* | Uncharged | Uncharged |
| Nesting | Refused past a fixed depth | No limit in the parser or the compiler; its own annotations note that very deep nesting can overflow the stack | Not handled; a stack overflow cannot be caught |
| Property names | Exact | **Loose** (`\p{ascii}`, `\p{general_category=Lu}` accepted) | Exact since SJL-7.5: `JSUnicodePropertyNames` checks every name before Broiler.Regex's loose lookup sees it, and every name probed answers as this family's does (row S6) *(Revised 2026-10-25, at SJL-11.1, by JSD-0081's correspondence; read "Loose")* |
| `Katakana_Or_Hiragana` | Accepted (an empty set) | Missing | Answered as this family answers it, since SJL-7.5 (row S6) *(Revised 2026-10-25, at SJL-11.1, by JSD-0081's correspondence; read "Missing")* |
| Group names | Validated against the pinned identifier classes | **Not checked** (`(?<1>a)` accepted) | Validated by `RegExpGroupNames` since Broiler.JS's PR #1041, with this family's answers for every raw and escaped name probed (row S2), but for an escaped duplicate in one alternative (S2a) and an escaped reference refused on the literal path (S2b) *(Revised 2026-10-25, at SJL-11.1, by JSD-0081's correspondence; read "Runtime categories")* |
| Duplicate named groups (ES2025) | **Refused** (a documented divergence) | **Refused**, even across alternatives | **Supported** through the .NET translator |
| Results | Offsets only | `RegexMatch` with eager substrings and dictionaries | A match-data abstraction over both |
| Unicode data | Format's tables (SJL-7 regenerates them in place with the Unicode owner's generator) | `Broiler.Unicode.Properties` and the emoji package, plus its own case-folding table | Both packages, plus .NET |
| Flag parsing | Two places | `RegexFlagsParser` | Three places |
| `RegExp.escape` | WhiteSpace and LineTerminator through the shared lexical classes, `EcmaScriptLexical` (`RegExpEscapeText`) *(Revised 2026-10-25, at SJL-11.1, by JSD-0081's correspondence; read "`Zs` through `CharUnicodeInfo`")* | (none) | The specification's answers over every input of row T5, as this family's, U+0085 not escaped *(Revised 2026-10-25, at SJL-11.1, by JSD-0081's correspondence; read "`char.IsWhiteSpace`, which escapes U+0085 where Broiler.VM does not")* |
| Assurance and process | In Broiler.VM's J rules | Generated artefacts, no human-reviewed unit, review stale; test262 not in CI; no AOT flags | (none) |
| Native AOT | Runs inside Broiler.VM's Native AOT images, backtracking on an explicit stack | Publishes and runs under Native AOT without a trim or AOT warning, though it declares neither flag. A deeply nested pattern terminates the process through stack overflow, at different depths under JIT and Native AOT ([native-aot.md](../native-aot.md)). | Not a Native AOT target: the engine emits IL. The .NET translator would run interpreted there, and `RegexTiering` asks for compiled mode without checking whether dynamic code is supported. |

---

## 3. The requirements the shared engine meets

These are Broiler.VM's requirements. Decision [SJL-D-14](../decisions.md#sjl-d-14) took branch (a)
on 2026-10-11: Broiler.Regex is fixed until it meets every one of them, and then both engines move
onto it.

1. **A caller-owned budget.** Every unit of matching work is charged through the caller's meter,
   in blocks. **Exhaustion is a distinct result**, never "no match" and never a shorter match.
   Cancellation reaches the matcher through the meter.
2. **Compile-time charging** in proportion to the work, set expansions included.
3. **Bounded memory.** Ceilings on backtracking frames and the undo trail, from the caller's policy,
   reported as exhaustion.
4. **Bounded nesting without CLR recursion,** in the parser, the compiler and the matcher.
5. **Exact syntax.**
   - Property names and values exactly as ECMA-262's tables list them, `Katakana_Or_Hiragana`
     included.
   - Group names validated against the shared identifier classes (SJL-7).
   - Annex B syntax only where the specification allows it.
6. **Duplicate named groups,** as ES2025 defines them, before Broiler.JS retires its translator
   (decision `SJL-D-14` (i)).
7. **An anchored match** for sticky matching and protocol use.
8. **An offset-only result interface.** Captures are positions; strings are the caller's choice.
9. **Text helpers.** Flag parsing; `RegExp.escape` over the specification's whitespace set; escaping
   of `source`.
10. **Process.**
    - `IsAotCompatible` and `IsTrimmable` with zero warnings.
    - test262's `RegExp` directories in the component's CI at the pinned revision.
    - A fuzz target.
    - The `SL` conventions.
    - An assurance gate.
11. **Data through the Unicode owner's provider** (SJL-7), so that the engine brings no table into
    a JavaScript engine's closure (decision [SJL-D-6](../decisions.md#sjl-d-6) (c)).

*(Read 2026-10-25, at SJL-11.1, by [JSD-0081](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md), which revises none of the eleven.)* Broiler.RegEx adopts
them as owner conditions by the dated record of 2026-10-20 in its roadmap section 6 (filed at `fca7a6d`,
merged into Broiler.RegEx's default branch at `e568b88` by Broiler-Platform/Broiler.Regex#6), which numbers them R1 to R11;
one not met by the end of SJL-11.2 refuses the area (decision D1). Four are read:
- **Requirement 1's units are this family's**: one unit per instruction dispatched or character a run examines, a
  property class's weight per character tested, a back-reference's length and prefilter skips, prepaid in blocks of
  512 before the work; and requirement 2's charge is proportional to the expansion (decision D8).
- **Requirement 3's ceilings** are the caller's; both engines pass this family's, frames `1 << 20` and trail
  `1 << 21` (decision D10).
- **Requirement 4, "without CLR recursion"**, is read as no CLR recursion in the matcher, with the parser and the
  compiler bounded by the caller's nesting limit, checked before each descent and answered with the nesting status,
  under a library maximum shown to complete on the smallest declared stack under the JIT and in the Native AOT probe
  (decision D6). Both engines pass 512, counted as this family's matcher counts it: 511 nested groups accepted, 512
  refused (decision D7).
- **Requirement 10's test262** is a host job through Broiler.JS's shell at a retained commit, with negative tests,
  and a host-free library job over the literal syntax tests and the generated property and set tests (decision D14);
  and **requirement 11's provider contract**, `IUnicodeTables`, may appear in Broiler.Regex's public signatures, as
  contract convention 1 is read to admit (decision D11).

---

## 4. The route SJL-D-14 took: fixing Broiler.Regex

| | Branch (a), taken on 2026-10-11 |
|---|---|
| Work | Add requirements 1 to 8 and 11 to Broiler.Regex's closure-based design. Requirement 4 means removing CLR recursion from its parser, compiler and continuation chain. Its `Budget`, the reset at each start position and the shortened match on exhaustion go. *(Revised 2026-10-25, at SJL-11.1, by JSD-0081 decision D5. The work is a new backend written in Broiler.Regex, not a repair of the closure-based design: a compiled instruction program run by one loop with an explicit frame stack and an undo trail, with this family's design and charging units as its specification and no code of this family's ported. The continuation chain recurses once per term, so a flat sequence of terms ends the process (row B8), and the parser's recursion ends it on deep nesting (row S3b), which requirement 4's reading bounds. The fast paths are re-expressed as instructions under the meter. The cell read as above.)* |
| What survives | Broiler.Regex's fast paths, its public syntax tree and `CodePointSet` algebra for its consumers, and its differential history against V8 *(Revised 2026-10-25, at SJL-11.1, by JSD-0081 decision D5: what survives is the parser, the public syntax tree, the `CodePointSet` algebra and the flag parser; the fast paths survive only as instructions of the new backend. The differential history against V8 is local evidence that is not in Broiler.RegEx's repository, and no population cites it. The cell read "Broiler.Regex's fast paths, its public syntax tree and `CodePointSet` algebra for its consumers, and its differential history against V8".)* |
| What Broiler.VM keeps from its matcher | Nothing in code. Its charging units are the starting point for Broiler.Regex's, and its compile-time use of the run-time parser carries over: the compiler validates literals with Broiler.Regex's parser. *(Revised 2026-10-25, at SJL-11.1, by JSD-0081 decisions D8 and D18: its charging units are the contract, not a starting point; and the compiler's check, the binder's `CheckRegExp` since SJL-12.6 item 2, calls Broiler.Regex's validate entry, parse and early errors with the nesting limit and no meter, keeping this family's diagnostic code and the literal's span. The cell read "Its charging units are the starting point for Broiler.Regex's" and "the compiler validates literals with Broiler.Regex's parser".)* |
| Acceptance, per engine | Conformance on test262 `RegExp` at the pin; the adversarial resource corpus; each engine's own measurement. Planned once to choose between branches, it now judges each cut-over. |

Branch (b), Broiler.VM's matcher behind the `Broiler.Regex` identity, was not taken.
[decisions.md](../decisions.md#sjl-d-14) keeps the comparison of the two.

---

## 5. Deletion lists

**Broiler.VM:**

- `JsRegExpMatcher` in full, from the Format assembly;
- the public `JsRegExpMatcher` and `JsRegExpCharge` types. They leave the family's API baseline;
- Broiler.VM's flag parsers, replaced by the shared ones;
- `RegExpEscapeText` and `RegExpEscapeCodePoint`'s classification;
- **rule N18's guard** moves to a file that still exists.

*(Revised 2026-10-25, at SJL-11.1, before any evidence of SJL-11 existed, by [JSD-0081](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md), decision
D20.)* Also: the public `JsRegExpMatch`, `JsRegExpSyntaxError` and `JsRegExpOverflowError`, with every line their
types hold in rule N10's baseline; `RegExpEscapeHex` and `RegExpSourceText`; the deleted file's rows in
`HUMAN_REVIEW.md` and `assurance.manifest.json`; and every deleted public member registered in rule I5.
`RegExpFuzzing.cs`, `eng/collect-js-evidence.py`'s `PROFILE_MATCHER` and `REGEXP_CONTROLS`, and `Fuzzing.cs`'s remark
are re-pointed to the adapter, and the binder's `CheckRegExp` to the validate entry (decision D18). The list ended
at "**rule N18's guard** moves to a file that still exists."

**Broiler.JS:**

- in `JSRegExp`: everything from `ClampLargeQuantifiers` to `HexValue` that is not object-model
  code. That is every `Transform*`, `Rewrite*`, `Validate*`, `*CaseFold*`, `Translate*` and
  `Expand*` member;
- `JSRegExp.UnicodeSets`;
- `RegexTiering` and its diagnostics;
- `GapScan`, `TryRouteToBroiler`, `FromNet` and the .NET regex field;
- `Parser/RegExpValidator` and `Runtime/RegExpUnicodeValidator`;
- the scanner's `DecodeBracedUnicodeEscapes` and its parenthesis-balance patch;
- the direct `value.Match` call in string search;
- the flag parsers.

*(Revised 2026-10-25, at SJL-11.1, before any evidence of SJL-11 existed, by JSD-0081 decisions D20 to D22.)* Also:
`RegExp.escape`'s `Escape` and `TryAppendEscape`, and `EscapePatternSource`, onto the text helpers; the regular-expression
prechecks of `RegExpGroupNames` and `JSUnicodePropertyNames`, wherever SJL-12.5 moved them; both `CreateRegex`
overloads, under SJL-D-25 (c); and the tests and the benchmark of the deleted members. `Runtime/RegExpUnicodeValidator`
is not deleted but becomes an (a) forwarder to the validate entry, and the scanner's call of `RegExpValidator` is
re-pointed to that entry. The list read as above, with `Runtime/RegExpUnicodeValidator` among the deletions.

**Broiler.Regex:** its `Budget` and the start-position reset, replaced by the caller's meter.

*(Revised 2026-10-25, at SJL-11.1, before any evidence of SJL-11 existed, by JSD-0081 decision D20.)* Also:
`StepLimit`, the fast path's early exit, the stack guard and `RegexOverflowException`, with the continuation-passing
matcher the new backend replaces; `CaseFolding`, its data and its generator; the loose property lookup; and the
static cache. The line read "its `Budget` and the start-position reset, replaced by the caller's meter."

---

## 6. Risks

| Risk | Mitigation |
|---|---|
| Broiler.JS loses the speed of the compiled .NET matcher on simple patterns | Broiler.Regex's own roadmap already records the gap. Broiler.JS's measurement under its predeclared rule judges the cut-over. A slower but bounded matcher is a recorded trade. |
| Requirement 4 needs more than a repair of Broiler.Regex's closure-based design | It is a component clause of SJL-11: neither engine moves until it holds. If it cannot be met, the area is refused with its record, and each engine keeps its own engine with a pointer to that record. *(Read 2026-10-25, at SJL-11.1, by JSD-0081 decision D5: the risk is taken as true, and the new explicit-stack backend is the planned route, not a fallback; the refusal path stands for any requirement not met by the end of SJL-11.2, decision D1.)* |
| Broiler.JS loses duplicate named groups | Requirement 6; the gate clause "duplicate named groups still work" |
| Broiler.VM's exhaustion verdicts move | A gate clause. Charging units come from Broiler.VM's current charges. |
