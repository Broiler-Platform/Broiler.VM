# Milestone SJL-11 — Regular expressions

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Area design](../areas/regexp.md)

- **Objective.** One regular-expression engine, published as `Broiler.Regex`, serves both engines:
  their run-time matching and Broiler.VM's compile-time validation of literals. That engine has:
  - a caller-owned work budget, with a distinct exhausted result;
  - cancellation through the budget's meter;
  - bounded memory, and nesting bounded without CLR recursion;
  - exact ECMAScript syntax, including property names and group names;
  - ES2025 duplicate named groups;
  - an anchored match;
  - offset-only results.

  Broiler.VM's matcher and Broiler.JS's .NET translator are gone. Each engine keeps its `RegExp`
  objects, `lastIndex`, the `Symbol` protocols, species, `GetSubstitution`'s evaluation, and its
  legacy statics if it has them.
- **Owner roles.** The Broiler.RegEx maintainer, the JavaScript profile owner and the Broiler.JS
  maintainer. One person.
- **Repositories.** Broiler.RegEx, Broiler.JS, Broiler.VM.
- **Waits on.** SJL-7, for the Unicode data that both the matcher and the parser need. It applies
  decision [SJL-D-14](../decisions.md#sjl-d-14), taken on 2026-10-11:
  - **(a)** Broiler.Regex is fixed until it meets every Broiler.VM requirement, and then both
    engines move onto it. Broiler.VM's matcher is not moved into the package; it is deleted;
  - **(i)** duplicate named groups are implemented in Broiler.Regex before Broiler.JS retires its
    translator.

**Today the engines handle regular expressions very differently.**

- **Broiler.JS** routes only `u`- and `v`-flag patterns, plus a few known-gap shapes, to
  Broiler.Regex. Every other pattern goes through a large .NET translator. A pattern Broiler.Regex
  cannot parse falls back to .NET silently.
- **Broiler.Regex's budget** gives each start position a fresh fixed allowance. When the allowance
  runs out, the result is "no match". In its greedy fast path the result can even be a **shorter
  match**. It has no cancellation, and its parser and compiler recurse without a guard.
- **Broiler.VM's matcher** charges every step to the engine's fuel and polls cancellation, and it
  bounds frames and trail. The compiler validates literals with the same parser, "so that the two
  cannot disagree about what a pattern is".

---

## Work packages

### SJL-11.1 — Verdict and comparison rules

The area verdict is filed, and records decision [SJL-D-14](../decisions.md#sjl-d-14). The comparison
the decision once planned for choosing an implementation base is now each cut-over's acceptance
check. Its rules are committed first. They judge the fixed Broiler.Regex against each engine's
current implementation on three things:

- conformance on test262's `built-ins/RegExp` and the literal syntax tests;
- resource behaviour on an adversarial pattern corpus: catastrophic backtracking, deep nesting, and
  large `v`-mode class expansions;
- each engine's own candidate-versus-control measurement.

*(Revised 2026-10-11, when SJL-D-14 was taken as (a). The work package was titled "Decision, verdict
and comparison rules" and began "Decision SJL-D-14 is taken on a predeclared comparison of three
things".)*

### SJL-11.2 — Broiler.Regex meets the requirements

Broiler.Regex is changed until it meets the requirements listed in
[areas/regexp.md](../areas/regexp.md), which are Broiler.VM's (decision
[SJL-D-14](../decisions.md#sjl-d-14) (a)):

- **Budget and resources:**
  - the budget and its charge, with an exhausted status that is never "no match";
  - compile-time charging in proportion to the expansion;
  - frame and trail ceilings;
  - a nesting limit with no CLR recursion.
- **ECMAScript syntax:**
  - exact property names, `Katakana_Or_Hiragana` included;
  - group names validated against the shared lexical classes;
  - duplicate named groups, before Broiler.JS retires its translator
    ([SJL-D-14](../decisions.md#sjl-d-14) (i)).
- **Interface:**
  - an anchored-match argument;
  - an offset-only result interface, with no eager substrings;
  - flag parsing and `RegExp.escape` helpers that use the specification's whitespace set;
  - escaping of the `source` text.
- **Data:** Unicode property, folding and string-property data reached through the Unicode
  owner's provider (SJL-7), so that Broiler.Regex brings no table into an engine's closure
  ([SJL-D-6](../decisions.md#sjl-d-6) (c)).
- **Process:** `IsAotCompatible` and `IsTrimmable` declared, with no warning; a Native AOT probe
  in the component's CI; test262's `RegExp` directories in the component's CI; and a fuzz target.
  The package already publishes under Native AOT without a warning. Its unguarded parser
  recursion, which terminates the process at a nesting depth that differs between JIT and Native
  AOT, is what requirement 4 removes.

**Removed from Broiler.Regex:** its `Budget`, the allowance reset at each start position, and the
greedy fast path's shortened match on exhaustion.

### SJL-11.3 — Vectors

The corpus gains vectors for:

- pattern acceptance and refusal;
- match results with indices;
- case-insensitive matching with and without `u`, and with `v`;
- property escapes;
- lookbehind;
- modifiers;
- the Annex B syntax;
- the exhausted status at a given budget.

### SJL-11.4 — Broiler.VM's cut-over

**Deleted:**

- `JsRegExpMatcher` from the Format assembly;
- the public types `JsRegExpMatcher` and `JsRegExpCharge`. They leave the profile's API baseline,
  which rule N10 holds in both directions.

**Moved to the shared engine:**

- the compiler's literal validation in `JsCompiler.Literals`;
- `JsRealm.RegExp`'s matching calls.

The adapter maps the shared statuses to Broiler.VM's errors and to `JsAbort(Exhausted)`.

**Correction, listed:** duplicate named groups, which Broiler.VM's matcher refuses today, are
accepted ([SJL-D-14](../decisions.md#sjl-d-14) (i), [SJL-D-16](../decisions.md#sjl-d-16) (a)). The
area-verdict JSD revisits the exclusion of test262's `regexp-duplicate-named-groups` files, which
JSD-0019 found to be about the pinned edition.

**Rules.** N18's vacuity guard currently requires `JsRegExpMatcher.cs` to exist. It is re-pointed,
and N18 still refuses the platform engine in Broiler.VM's own sources. Rule `I2` refuses it in the
shared engine.

### SJL-11.5 — Broiler.JS's cut-over

**Deleted:**

- the translator: everything from `ClampLargeQuantifiers` to `HexValue` in `JSRegExp` that is not
  object-model code;
- `JSRegExp.UnicodeSets`;
- `RegexTiering` and its diagnostics;
- the routing (`GapScan`, `TryRouteToBroiler`, `FromNet`) and the `.NET` regex field;
- `Parser`'s `RegExpValidator`, which validates with .NET;
- `Runtime`'s `RegExpUnicodeValidator`;
- the scanner's rewriting of braced Unicode escapes, which changes `source` today.

**Every pattern goes through the shared engine.** Broiler.JS's legacy `RegExp` statics stay in
Broiler.JS.

**Corrections, listed:**

- loose property names;
- invalid group names;
- the culture-dependent case-insensitive matching;
- `lastIndex` in the middle of a surrogate pair;
- `source` rewriting;
- an invalid literal becoming division. **This last one is the frontend's, and is fixed at SJL-14
  if not here.**

### SJL-11.6 — Evidence

The same as SJL-3.6, plus the adversarial resource corpus in both engines. **Broiler.VM's
exhaustion verdicts must be unchanged or admitted.**

---

## Exit gate

**Component clauses.**

1. Every requirement of SJL-11.2 is met.
2. test262's `RegExp` directories run in the component's CI.
3. The adversarial corpus shows no silent exhaustion and no shortened match.

**Broiler.JS clauses.**

4. The translator and the validators are gone.
5. test262 `RegExp` per path meets the area rule.
6. Duplicate named groups still work.
7. Every correction is listed.

**Broiler.VM clauses.**

8. Rules `I1` to `I7` hold.
9. N10 and N18 are revised, with witnesses.
10. test262 in both forms meets the area rule.
11. The fuel-parity twins and the exhaustion verdicts meet the area rule.
12. Every root is published and run in three modes.
13. Compile-time literal refusals are unchanged, or every difference is classified.

**Records.**

- **Broiler.VM:** the area-verdict JSD, the API-baseline change, and decision `SJL-D-14`.
- **Broiler.JS:** its record, with `Component.md` §3 closed.
- **Broiler.RegEx:** its records.

---

## Evidence bundles

`docs/evidence/sjl-11-001/` in each repository.

---

## Does not do

- **It does not share the `RegExp` object**, `lastIndex`, the `Symbol.match`, `replace`, `search`
  and `split` protocols, species, or `GetSubstitution`'s evaluation.
- **It does not add regular-expression features** beyond the duplicate named groups Broiler.JS
  already has, which Broiler.VM gains as a listed correction.
