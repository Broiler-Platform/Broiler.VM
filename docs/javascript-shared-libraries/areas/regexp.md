# Area: regular expressions

**Delivered by:** [SJL-11](../milestones/sjl-11-regexp.md). **Owner:** Broiler.RegEx (package
`Broiler.Regex`). **Status:** design. Behaviour statements about Broiler.Regex come from its source at
`c1422f4d`, which is the commit its preview.4 package was built from.

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
| Work budget | Every instruction, character run, property weight, back-reference length and prefilter skip is charged, in blocks, through `JsRegExpCharge` into the engine's fuel, which polls cancellation and the wall clock. Frames and trail are bounded, and overflowing them gives `JsAbort(Exhausted)`. | **A fixed step allowance, reset at each start position.** It is counted only in quantifier loops. Exhaustion returns **no match**, or, in the greedy fast path, **a shorter match**. No callback, no cancellation, no timeout. | .NET `Regex` with no timeout and no cancellation; an overflow falls back to .NET or to no match |
| Compile cost | Charged in proportion to the source length only, so a `v`-mode class expansion is under-charged *(reading)* | Uncharged | Uncharged |
| Nesting | Refused past a fixed depth | No limit in the parser or the compiler; its own annotations note that very deep nesting can overflow the stack | Not handled; a stack overflow cannot be caught |
| Property names | Exact | **Loose** (`\p{ascii}`, `\p{general_category=Lu}` accepted) | Loose |
| `Katakana_Or_Hiragana` | Accepted (an empty set) | Missing | Missing |
| Group names | Validated against the pinned identifier classes | **Not checked** (`(?<1>a)` accepted) | Runtime categories |
| Duplicate named groups (ES2025) | **Refused** (a documented divergence) | **Refused**, even across alternatives | **Supported** through the .NET translator |
| Results | Offsets only | `RegexMatch` with eager substrings and dictionaries | A match-data abstraction over both |
| Unicode data | Format's tables (SJL-7 regenerates them in place with the Unicode owner's generator) | `Broiler.Unicode.Properties` and the emoji package, plus its own case-folding table | Both packages, plus .NET |
| Flag parsing | Two places | `RegexFlagsParser` | Three places |
| `RegExp.escape` | `Zs` through `CharUnicodeInfo` | (none) | `char.IsWhiteSpace`, which escapes U+0085 where Broiler.VM does not |
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

---

## 4. The route SJL-D-14 took: fixing Broiler.Regex

| | Branch (a), taken on 2026-10-11 |
|---|---|
| Work | Add requirements 1 to 8 and 11 to Broiler.Regex's closure-based design. Requirement 4 means removing CLR recursion from its parser, compiler and continuation chain. Its `Budget`, the reset at each start position and the shortened match on exhaustion go. |
| What survives | Broiler.Regex's fast paths, its public syntax tree and `CodePointSet` algebra for its consumers, and its differential history against V8 |
| What Broiler.VM keeps from its matcher | Nothing in code. Its charging units are the starting point for Broiler.Regex's, and its compile-time use of the run-time parser carries over: the compiler validates literals with Broiler.Regex's parser. |
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

**Broiler.Regex:** its `Budget` and the start-position reset, replaced by the caller's meter.

---

## 6. Risks

| Risk | Mitigation |
|---|---|
| Broiler.JS loses the speed of the compiled .NET matcher on simple patterns | Broiler.Regex's own roadmap already records the gap. Broiler.JS's measurement under its predeclared rule judges the cut-over. A slower but bounded matcher is a recorded trade. |
| Requirement 4 needs more than a repair of Broiler.Regex's closure-based design | It is a component clause of SJL-11: neither engine moves until it holds. If it cannot be met, the area is refused with its record, and each engine keeps its own engine with a pointer to that record. |
| Broiler.JS loses duplicate named groups | Requirement 6; the gate clause "duplicate named groups still work" |
| Broiler.VM's exhaustion verdicts move | A gate clause. Charging units come from Broiler.VM's current charges. |
