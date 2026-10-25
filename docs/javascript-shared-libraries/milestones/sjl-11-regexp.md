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

*(Added 2026-10-25, at SJL-11.1, by
[JSD-0081](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md).)*
The area verdict is JSD-0081, decided on 2026-10-21: all six sub-areas accepted and placed with Broiler.RegEx under
the area design's requirements 1 to 11 as owner conditions, which Broiler.RegEx's roadmap numbers R1 to R11, and
decision SJL-D-14 recorded. It records the owner's decisions D1 to D41, taken under the owner's direction of
2026-10-18, which changes no rule. Its input is the correspondence of bundle
[`sjl-11-001`](../../evidence/sjl-11-001/correspondence.md), read from code in both engines and in Broiler.Regex's
`0.1.0-preview.4`, with every behavioural cell run. The three things above are judged by rules committed in each
repository's bundle `sjl-11-001` before any code they judge:
- **conformance**: this family's test262 in both forms, with its compiler's literal refusals over the parse corpus;
  Broiler.JS's test262 per path with negative tests; the component's test262 in its own CI (decision D14);
- **resource behaviour**: one adversarial corpus, in Broiler.RegEx, from which each engine's programs are generated
  with that engine's declared outcomes, the corpus's SHA-256 named in each manifest (decision D37);
- **each engine's measurement**: a predeclared part of its rule, judged and recorded, and **not a gate clause**. A
  NOT MET is recorded in the cut-over record as "a slower but bounded matcher", the trade the area design's section 6
  names, and Broiler.JS's part is void on a lane that is not controlled (decision D19).

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

*(Read 2026-10-25, at SJL-11.1, by
[JSD-0081](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md),
decisions D5, D6 and D14, which revise nothing.)*
- **"A nesting limit with no CLR recursion"**, and the objective's "nesting bounded without CLR recursion", are read
  as no CLR recursion in the matcher, with the parser and the compiler bounded by the caller's nesting limit, checked
  before each descent and answered with the nesting status, under a library maximum shown to complete on the
  smallest stack the adversarial corpus declares, under the JIT and in the Native AOT probe (decision D6). Both
  engines pass 512, counted as this family's matcher counts it, so 511 nested groups are accepted and 512 refused
  (decision D7).
- **The matcher is a new backend** written in Broiler.Regex, an explicit-stack machine with this family's design and
  charging units as its specification and no code of this family's ported; Broiler.Regex's parser, public tree,
  `CodePointSet` algebra and flag parser stay (decision D5).
- **"test262's `RegExp` directories in the component's CI"**, here and in exit clause 2, is read as two jobs: a host
  job running Broiler.JS's shell at a retained commit named by a dated record in Broiler.RegEx's roadmap, with the
  candidate package from a local feed and negative tests, whose expected failures are the host's alone and in which
  every duplicate-named-groups row passes; and a host-free library job over the literal syntax tests and the generated
  property-escape and set tests, under the JIT and through the Native AOT probe (decision D14).

**Removed from Broiler.Regex:** its `Budget`, the allowance reset at each start position, and the
greedy fast path's shortened match on exhaustion.

*(Revised 2026-10-25, at SJL-11.1, before any evidence of SJL-11 existed, by JSD-0081 decision D20.)* Also removed:
`StepLimit`, the stack guard and `RegexOverflowException`, with the continuation-passing matcher the new backend
replaces; `CaseFolding`, its data and its generator, under requirement 11; the loose property lookup; and the static
cache. The list read "its `Budget`, the allowance reset at each start position, and the greedy fast path's
shortened match on exhaustion."

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

*(Revised 2026-10-25, at SJL-11.1, before any evidence of SJL-11 existed, by
[JSD-0081](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md),
decision D20.)* The list is extended, and every deleted public member is registered in rule I5:
- the public `JsRegExpMatch`, `JsRegExpSyntaxError` and `JsRegExpOverflowError`, with every line their types hold in
  rule N10's baseline;
- `JsRealm.RegExp`'s `RegExpNormalizeFlags`, `RegExpEscapeText`, `RegExpEscapeCodePoint`, `RegExpEscapeHex` and
  `RegExpSourceText`, onto Broiler.Regex's flag parser and text helpers;
- the deleted file's rows in `HUMAN_REVIEW.md` and `assurance.manifest.json`;
- and re-pointed, not deleted: `RegExpFuzzing.cs` to the adapter, keeping the `broiler-js-regexp-fuzz` session;
  `eng/collect-js-evidence.py`'s `PROFILE_MATCHER` and `REGEXP_CONTROLS` to an adapter line whose mutation the
  controls still catch; `Fuzzing.cs`'s remark on the matcher; and rule N18's vacuity guard.

The list ended at its second item, "the public types `JsRegExpMatcher` and `JsRegExpCharge`."

**Moved to the shared engine:**

- the compiler's literal validation in `JsCompiler.Literals`;
- `JsRealm.RegExp`'s matching calls.

*(Revised 2026-10-25, at SJL-11.1, by JSD-0081 decision D18.)* Since SJL-12.6 item 2, merged on 2026-10-21 at
`daa92eb`, the literal's validation is the binder's `CheckRegExp` (`JsBinder.EarlyErrors.cs`), moved there from the
lowering's `CompileRegExpLiteral` and `RefuseRegExpLiteralFlags` unchanged. **`CheckRegExp` is what SJL-11.4
re-points**, onto Broiler.Regex's validate entry, with no meter, and onto the shared flag parser, keeping this family's
diagnostic code and the literal's span; `CompileRegExpLiteral`'s remark that the grammar lives in the format assembly
is corrected in the same change. The Compiler references Broiler.Regex for this from SJL-11.4 until SJL-13. The first
item read "the compiler's literal validation in `JsCompiler.Literals`".

The adapter maps the shared statuses to Broiler.VM's errors and to `JsAbort(Exhausted)`.

*(Added 2026-10-25, at SJL-11.1, by JSD-0081 decisions D7, D8 and D10.)* The adapter passes this family's policy:
frames `1 << 20` and trail `1 << 21`, its `MaximumFrames` and `MaximumTrail` (decision D10), and nesting 512 (decision
D7); the realm's fuel is the meter, charged in this family's units (decision D8). A limit exceeded becomes
`JsAbort(Exhausted)`, as the matcher's ceilings do today, and the nesting and syntax statuses `SyntaxError`.
`RegExpMatchOne`'s step back to a surrogate pair's start moves the matcher's start only; the `"index"` and the
substring it reports start at `lastIndex` (row M4).

**Correction, listed:** duplicate named groups, which Broiler.VM's matcher refuses today, are
accepted ([SJL-D-14](../decisions.md#sjl-d-14) (i), [SJL-D-16](../decisions.md#sjl-d-16) (a)). The
area-verdict JSD revisits the exclusion of test262's `regexp-duplicate-named-groups` files, which
JSD-0019 found to be about the pinned edition.

*(Added 2026-10-25, at SJL-11.1, by JSD-0081 decisions D15 and D23.)* **The files are admitted**, in the admission
commit, the first commit of SJL-11.4's branch, which adds `regexp-duplicate-named-groups` to `SuiteFeatures.cs`'s
`AdmittedProposals` naming JSD-0081 and changes nothing else outside `docs/`. So they are in both of part A's
populations, and the departure from that file's condition, that a proposal is admitted when the profile implements
the whole of it, is recorded in JSD-0081 with its reason. **The correspondence adds five corrections of this family,
each listed** with its row (kind R): `\k` read as a letter in a class beside a named group (S5); a four-digit
lead-surrogate escape before a braced escape refused under `u` and `v` (S10); the `"index"` and substring of a
non-empty match from a `lastIndex` inside a surrogate pair (M4); a back-reference compared by code units under `u`
and `v` (M7); and a raw line terminator copied into `source` (T3). Row M4's empty match is no correction: decision
D41 classes it implementation-defined until the specification's text is corrected.

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

*(Revised 2026-10-25, at SJL-11.1, before any evidence of SJL-11 existed, by
[JSD-0081](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md),
decisions D20 to D22.)* The list is extended:
- `ParseFlags`, `BuildFlagsString`, and the escape helpers `Escape`, `TryAppendEscape` and `EscapePatternSource`,
  onto Broiler.Regex's flag parser and text helpers;
- the regular-expression prechecks of `RegExpGroupNames` and `JSUnicodePropertyNames`, wherever SJL-12.5 moved them;
- both `CreateRegex` overloads, removed with `RegexTiering` and its diagnostics under SJL-D-25 (c), after a recorded
  consumer check (decision D21);
- the scanner's parenthesis-balance patch, beside the rewriting of braced escapes the list names; the scanner's
  `RegExpValidator` call is re-pointed to Broiler.Regex's validate entry, and the division fallback stays, SJL-14's
  (decision D22);
- `String.prototype.search`'s direct `value.Match` call;
- the tests and the benchmark that test the deleted members (plan task 6.4).

`RegExpUnicodeValidator` is not deleted: it becomes an (a) forwarder to the validate entry under SJL-D-25, wherever
SJL-12.5 moved it (decision D21). The list read as above, and its sixth item, "`Runtime`'s `RegExpUnicodeValidator`",
named a deletion.

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

*(Revised 2026-10-25, at SJL-11.1, before any evidence of SJL-11 existed, by JSD-0081 decision D23.)* **The list is
read by the correspondence's rows.** Loose property names and invalid group names were corrected at SJL-7.5 and by
Broiler.JS's PR #1041, so they are no longer corrections here: their prechecks are deleted. The list is:
- the culture-dependent and .NET case-insensitive matching: rows M2 (bundle `sjl-7-001`'s F2), M2a under `tr-TR`
  and under `lt-LT`, and M2b; and row U3, by data decision (class C5);
- the translator's line terminators, with `$` without `m`: row M3 (bundle `sjl-7-001`'s W5);
- `lastIndex` inside a surrogate pair: row M4's matches of half a pair. The step out of a pair moves only the
  matcher's start, never the reported `"index"` or substring;
- `source` rewriting: row T1;
- the literal check: rows S2b, S7, S8 and S11, closed by re-pointing the scanner's check to the validate entry;
- an escaped duplicate name in one alternative: row S2a;
- a class that joins a lead-surrogate escape to a braced trail escape: row S10's class case;
- with Broiler.Regex's `0.1.0-preview.4`: the budget's no-match, shortened match, wrong index and flipped
  look-around (rows B1 to B3); the stack guard's fallback, to .NET and to no match (row B5); the process aborts of a
  flat sequence and of deep nesting (rows B8 and S3b); and the look-behind back-reference inside a pair (row M7);
- `String.prototype.search`'s direct call: row M6.

`\0` before a non-ASCII digit is not on it: it is Broiler.Regex's alone and not observable in Broiler.JS (row S4).
An invalid literal becoming division stays the frontend's (row S9), SJL-14's. The list read "loose property names;
invalid group names; the culture-dependent case-insensitive matching; `lastIndex` in the middle of a surrogate pair;
`source` rewriting; an invalid literal becoming division".

### SJL-11.6 — Evidence

The same as SJL-3.6, plus the adversarial resource corpus in both engines. **Broiler.VM's
exhaustion verdicts must be unchanged or admitted.**

*(Revised 2026-10-25, at SJL-11.1, before any evidence of SJL-11 existed, by
[JSD-0081](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md),
decisions D8, D10, D16, D17 and D19.)*
- **"In both engines" holds in Broiler.JS for the classes that complete.** Broiler.JS passes no meter, as in every
  shared area, so the corpus's unbounded classes, K catastrophic backtracking and A alternation without quantifiers,
  are judged at library level, by Broiler.RegEx's rule, where risk S1's stop is read (decision D10). The sentence
  read "The same as SJL-3.6, plus the adversarial resource corpus in both engines."
- **"Admitted"** means admitted by this family's rule: a moved exhaustion verdict, a stop at a frame or trail ceiling
  among them, is class C2 of its test262 part only on a variant that reaches a regular expression and only if the
  variant passes at ten times the allowance, and class C2 of its adversarial part only as a move between fuel and a
  ceiling (decision D8). There is no regular-expression fuel twin.
- **The bases** are this family's at the admission commit, the first commit of SJL-11.4's branch, whose parent the
  after merge must have as its first parent, and Broiler.JS's at the first parent of the merge that lands SJL-11.5,
  under a hold; a base taken anywhere else is void and is taken again (decision D16).
- **The inputs**, each engine's adversarial programs and workloads, are committed with its rule under its bundle's
  `inputs/`; Broiler.JS's benchmark class follows in a later inputs commit (decision D17).
- **The measurements** are predeclared parts of each engine's rule and not gate clauses (decision D19).

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
  *(Added 2026-10-25, at SJL-11.1.)* The area-verdict JSD is
  [JSD-0081](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md),
  which records SJL-D-14. SJL-11.4 files a second record, "The profile on Broiler.Regex", at the next free number,
  with its correction entries, the rows rule I5 registers and the revisions of rules N10 and N18; SJL-11.6 files an
  exclusion at the next free number for the code that leaves assurance coverage (JSD-0081 decisions D3 and D28).
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
