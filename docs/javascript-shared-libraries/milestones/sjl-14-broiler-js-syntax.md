# Milestone SJL-14 — Broiler.JS on the shared syntax library

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Area design](../areas/frontend.md)

- **Objective.** Broiler.JS's compiler and its portable compiler parse with
  `Broiler.ECMAScript.Frontend`. These are deleted: `FastParser`, `FastScanner`, `FastTokenStream`,
  the remains of `SyntaxValidation`, the .NET-based regular-expression literal validation, and the
  numeric-literal code. Broiler.JS's public `Ast` and `Parser` packages become adapter packages
  under decision [SJL-D-12](../decisions.md#sjl-d-12) (i), and its downstream consumers keep
  working.
- **Owner roles.** The Broiler.JS maintainer.
- **Repositories.** Broiler.JS. Broiler.JSeal and Broiler.HtmlBridge are checked as consumers, and
  are not changed.
- **Waits on.** SJL-13.

*(Read 2026-11-03, at SJL-14.1, before any evidence of SJL-14 existed, by decision D41 as
[JSD-0083](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md)
revised it.)* In this file "decision Dn" is SJL-13's plan's numbering, which JSD-0083 keeps: D40 to D56 and D64 are
SJL-14's, the owner's decisions under the owner's direction of 2026-10-18, recorded in Broiler.JS's
`docs/roadmap/SharedLibraries.md`, section "Broiler.JS on the shared syntax library (SJL-14)", which is filed in
Broiler.JS's own change and observed until it merges: D41 and D44 to D47 as JSD-0083 revised them on 2026-10-26, the
owner's decisions of that day; the others of 2026-11-03, with D45 revised again that day (JSD-0083's note under
D45).
- **"the remains of `SyntaxValidation`"** is read as its compiler-stage checks, which SJL-15.5 deletes when the
  compiler moves to the shared tree, and its two token re-scans, `SyntaxValidation.ContainsLegacyOctalLiteral` and
  `DirectEvalSupport.ContainsStrictReservedWordUsage`, which SJL-14's cut-over deletes with the direct-eval
  `"use strict";` prefix (decision D41). The objective's "These are deleted" holds at SJL-14 for the re-scans only;
  SJL-14.5's "**`SyntaxValidation` and the compiler's early-error code are not deleted here.**" holds for the rest.
- **"the numeric-literal code"** is read as `FastScanner`'s literal scanning (`ReadNumber` and the members it
  calls), which goes with `FastScanner`'s scanning code. `Parser`'s `NumberCoercion` class is already gone (SJL-14.5's
  note below).

---

## Work packages

### SJL-14.1 — Comparison rules

The following are committed before any code:

- test262 per path;
- the old-tree-against-new-tree comparison over the parse corpus;
- the portable compiler's suite;
- Broiler.JS's own parse measurement under a rule.

*(Added 2026-11-03, at SJL-14.0 and SJL-14.1.)* SJL-14 started on 2026-11-03 at the owner's words of 2026-10-25,
"Continue with the remaining SJL milestones", read as the choice to start SJL-14, which the [ledger](../status.md)'s
SJL-14.0 entry quotes and reads; the reading is recorded as a reading. **SJL-14 files no verdict of its own**
(decision D40): SJL-13.1's verdict, JSD-0083, covers both engines' front ends, and SJL-14.1 completes its X6's
Broiler.JS half by a dated note on JSD-0083, from Broiler.JS's dispositions. **The rule is committed before any
code**, in Broiler.JS's bundle `docs/evidence/sjl-14-001/` (`decision-rule.md`, parts A to K): test262 per path (part
A); the bridge's identity with the switch off at its merge and at each later library merge (part B); the side-by-side
run (part C); the parse corpus across the cut-over, through SJL-12's parse-corpus tool built unchanged against the
adapter packages, which is the old-tree-against-new-tree comparison (part D); the portable compiler's suite and its
Native AOT sample (part E); the packages, the downstream consumers and the public API (parts F to H); Broiler.JS's
parse measurement (part I); fuzzing (part J); and the deletion search (part K). The same bundle holds the corrections
by row (`corrections.md`), the disposition of every public type and member of `Ast` and `Parser`
(`dispositions.md`), the inherited-context probes and the rule's inputs. It is filed in Broiler.JS's own change, on
its branch `sjl-14-1-records`, and is observed until it merges.

### SJL-14.2 — The bridge

An adapter builds Broiler.JS's existing `AstNode` tree from the shared tree. It synthesises the
token references and the mutable fields the compiler reads, so the compiler is unchanged at first.

The old parser and the bridged parser run side by side under a test-only switch over:

- the parse corpus;
- test262;
- Broiler.JS's own suites.

Every difference is classified. Expected differences are corrections, among them:

- early errors Broiler.JS missed;
- invalid regular-expression literals that are no longer read as division;
- the `m` suffix, which now arrives through the extension parameter.

*Done when* the side-by-side run is retained with every difference classified.

*(Read 2026-11-03, at SJL-14.1, before any evidence of SJL-14 existed, by decisions D42 to D46.)* The list
"Expected differences are corrections, among them: early errors Broiler.JS missed; invalid regular-expression
literals that are no longer read as division; the `m` suffix, which now arrives through the extension parameter." is
not false but incomplete; later records hand SJL-14 more. **The corrections are K1 to K10 of decision D45**, each named
by row in Broiler.JS's bundle `sjl-14-001`'s `corrections.md` before any evidence:
- **K1** the strict-mode `NonOctalDecimalIntegerLiteral` (`strict-non-octal-decimal`, the number vectors
  `numbers/literal/0082`, `0084`, `0088` and `0090`), **K2** the digit lookahead after `?.` and `.`, **K3** the tagged
  template's raw-segment trimming and **K4** line terminators in the lexer, which the bundles of SJL-3, SJL-7 and
  SJL-12 left to SJL-14;
- **K5** an invalid regular-expression literal refused, not read as division (row S9 of JSD-0081's correspondence),
  **K6** the braced-escape decoding if SJL-11.5 has not landed, and **K9** the regular-expression rows only if the
  provider wraps Broiler.Regex's validate entry;
- **K7** early errors the parser missed and **K8** the grammar failures Broiler.JS admits;
- **K10** the rows SJL-13's correspondence classes as Broiler.JS's corrections (hand-off H22 of JSD-0083, row P16b
  among them) and the rows SJL-14.1's inherited-context probes add.

Besides the corrections, the rule admits **class E**, an early error the library reports at the parse that
Broiler.JS checks only in its compiler, or that its parser let pass for that input and its compiler refused (decision
D45 as revised on 2026-11-03, JSD-0083's note under D45), **class T**, a `SyntaxError` raised before evaluation that
Broiler.JS raised later or not at all (decision D64), and **class M**, a message text that changes, listed by a
generated table, the texts hosts and tests read being kept (decision D46). JSD-0080's decision D27's listed refusals
are an implementation limit (C3), not corrections. The bridge also passes the inherited context of eval code and the
dynamic function's goals in the request (JSD-0080's "What each engine keeps"; decision D44). The test-only switch is
an internal member of the compilation options, off by default, and the bridge merges in one window, W1, judged by
identity with the switch off (decisions D42 and D43).

### SJL-14.3 — The compiler reads the shared tree

At SJL-14 the compiler parses through the bridge, which builds its `AstNode` tree from the shared
tree. It moves to the shared tree at SJL-15.5, with the semantic model. Nothing in the engine reads
the bridge after that, and the bridge remains only as the obsolete adapter packages'
implementation, until their removal version.

*(Revised 2026-11-03, at SJL-14.1, before any evidence of SJL-14 existed, by decision D41 as JSD-0083 revised it. The
paragraph read "The compiler moves from the bridged `AstNode` tree to the shared tree, in stages Broiler.JS's plan
names. Once nothing reads the bridge, the bridge goes." Clause 1 asks that the compiler "parse with" the shared
frontend, which the bridge meets. Moving the compiler at SJL-14 would port `SyntaxValidation`, `FastFunctionScope` and
`FreeNameScan`, which SJL-15.5 deletes; SJL-15's clause 10 is revised the same day to judge the move.)*

`PortableCompiler` switches to the shared frontend directly. **It gains the early errors it skips
today**, because it never runs `SyntaxValidation`. Its closure loses `Runtime`, `Storage` and
`Expressions`.

*(Noted 2026-11-03, at SJL-14.1, by decisions D50 and D53.)* "Its closure loses `Runtime`, `Storage` and
`Expressions`." is overtaken in part. `Runtime` and `Storage` left the portable compiler's closure at SJL-12.5
(Broiler.JS `792ff56e`, the merge of Broiler-Platform/Broiler.JS#1066): at Broiler.JS's `33d3093b`
`Portable.Compiler` references `Ast`, `Parser` and `Portable`, whose closure holds `Expressions` and `Algorithms` and
neither `Runtime` nor `Storage`. After SJL-14 it references the frontend and `Portable`, and `Expressions` only if it
uses a non-tree type that moves there (decision D53), so its closure loses `Expressions` only if it uses none.
`ModuleSourceAnalysis` also reads the shared tree directly (decision D41). Clause 4 names only the runtime, and is
not changed; Broiler.JS's rule judges it with a closure listing at the cut-over (part E).

### SJL-14.4 — Public packages and consumers

Per [SJL-D-12](../decisions.md#sjl-d-12) (i), taken on 2026-10-11:

- `Broiler.JavaScript.Ast` and `Broiler.JavaScript.Parser` become adapter packages that build the
  old tree from the shared one, marked obsolete with a removal version, and are removed in a later
  major version;
- the disposition is recorded in `docs/public-api.md`.

Also:

- the pristine consumer is updated;
- Broiler.JSeal's and Broiler.HtmlBridge's suites run against a candidate feed. Broiler.JSeal's
  `eng/test-package-consumer.ps1` supports this without changing their pins.

*(Read 2026-11-03, at SJL-14.1, before any evidence of SJL-14 existed, by decisions D49 and D50.)*
- **The disposition is decided per type at SJL-14.1**, in Broiler.JS's bundle `sjl-14-001`'s `dispositions.md`, and
  recorded in `docs/public-api.md` at the cut-over, which the rule's part H checks: `Parser`'s public types a facade
  over the bridge, keeping every member and throwing today's exception types; `Ast`'s tree types obsolete; the
  non-tree types moved to `Broiler.JavaScript.Expressions` with type forwarders; one removal version, `1.0.0`, for
  every mark but `UndoMark`'s, which keeps its mark of SJL-12.5 without a removal version until a consumer check is
  recorded (JSD-0080's decision D20). `Ast` is the engine's foundation, not only a tree, so it is not made obsolete
  whole.
- **The candidate-feed runs** are judged by clause 5 as revised below. Broiler.JSeal's module adapter reflects on
  `CoreScript` members SJL-12.5 removed, which is SJL-12.5's finding, recorded by a dated correction of Broiler.JS's
  `docs/public-api.md`, and not SJL-14's to fix; Broiler.HtmlBridge has no candidate script and runs in a scratch
  override.

### SJL-14.5 — Deletion

**Deleted:**

- `FastParser` and its partial files;
- `FastScanner`;
- `FastTokenStream`;
- `FastKeywordMap`, unless the extension parameter needs a remnant of it;
- `CharExtensions`' scanner use;
- `RegExpValidator`;
- what remains of `NumberCoercion`;
- the parser's half of `FastScope`.

**`SyntaxValidation` and the compiler's early-error code are not deleted here.** They go at SJL-15,
when the semantic model replaces them. They are listed here so that nothing is forgotten.

*(Revised 2026-11-03, at SJL-14.1, before any evidence of SJL-14 existed, with clause 2 below, under the
[ledger](../status.md)'s update rule 5, by decisions D41, D50 and D51 and JSD-0081's decision D22. The list's text
above is kept.)* The list is read item by item, and it gains three items. Broiler.JS's bundle `sjl-14-001`'s
`dispositions.md` gives every item member by member, with its lines at Broiler.JS's `33d3093b`, and JSD-0083's X6
carries it by a dated note of the same day.
- **What goes is the implementation**: `FastParser`'s partial files, `FastScanner`'s and `FastTokenStream`'s scanning
  code, `FastKeywordMap`'s table, `CharExtensions`' scanner members and the parser's half of `FastScope`. Their public
  names stay as obsolete facades over the bridge, as clause 2 now says (decision D50).
- **"`FastKeywordMap`, unless the extension parameter needs a remnant of it"**: it needs none. The `m` suffix arrives
  through the library's extension parameter (JSD-0083's decision D17), so the table goes and no remnant is kept.
- **"`RegExpValidator`"** is deleted if SJL-11.5 has landed when the cut-over merges. If not, it is relocated behind
  the request's regular-expression provider, not deleted, with `DecodeBracedUnicodeEscapes` and the parenthesis patch,
  and SJL-11.5's rule in Broiler.JS is revised by a new dated rule file (decision D51).
- **"what remains of `NumberCoercion`"**: nothing remains. `Parser`'s `NumberCoercion` class was deleted at SJL-3.5
  (Broiler.JS `93df3fd4`; [areas/numbers.md](../areas/numbers.md)), and at `33d3093b` only a comment in one of
  Broiler.JS's integration tests names it.
- **It gains** the division fallback of `FastScanner`'s regular-expression scan, which JSD-0081's decision D22 keeps
  SJL-14's (row S9); `DecodeBracedUnicodeEscapes` and the parenthesis patch, which JSD-0080's decision D23's dated note
  leaves to SJL-14 if SJL-11 has not deleted them, within decision D51's relocation; and the two token re-scans,
  `SyntaxValidation.ContainsLegacyOctalLiteral` and `DirectEvalSupport.ContainsStrictReservedWordUsage`, with the
  direct-eval `"use strict";` prefix and the two places that skip it, which JSD-0083's revised decision D41 deletes at
  the cut-over. "**`SyntaxValidation` and the compiler's early-error code are not deleted here.**" holds for
  everything else in them.

### SJL-14.6 — Evidence

- test262 per path;
- the side-by-side comparison;
- the portable compiler's suite and its Native AOT sample;
- the pristine consumer;
- the downstream candidate-feed runs;
- Broiler.JS's own parse measurement.

---

## Exit gate

**Broiler.JS clauses**

1. The compiler and the portable compiler parse with the shared frontend.
2. The implementations on SJL-14.5's deletion list are absent; the public names SJL-D-12 (i)'s
   adapter keeps stay only as obsolete facades over the bridge.
   *(Revised 2026-11-03, at SJL-14.1, before any evidence of SJL-14 existed, under the ledger's
   update rule 5, by decision D50: the clause read "The deletion list of SJL-14.5 is absent." The
   adapter packages SJL-D-12 (i) asks for keep `Parser`'s public names as facades, so that SJL-12's
   parse-corpus tool and the downstream consumers build and run unchanged; keeping the names changes
   what the clause means, which is a gate revision, not a reading. No evidence of SJL-14 existed to
   re-evaluate.)*
3. test262 per path meets the rule. Every correction is listed.
4. The portable compiler passes its suite with its new early errors. Its closure no longer contains
   the runtime. The Native AOT sample's program, regenerated by the portable compiler on the shared
   frontend, is identical to the checked-in copy, and the sample publishes with Native AOT and runs.
   *(Revised 2026-10-11, after the Native AOT assessment: the clause ended at "the runtime". The
   sample was listed as evidence in SJL-14.6 but in no clause.)*
5. The public `Ast` and `Parser` packages are adapter packages, marked obsolete with a removal
   version (`SJL-D-12` (i)). The pristine consumer passes, and the downstream candidate-feed runs
   lose no test: every test that passes against the base candidate passes against the after
   candidate.
   *(Revised 2026-10-11, when SJL-D-12 was taken: the clause began "The public `Ast` and `Parser`
   packages follow `SJL-D-12`.")*
   *(Revised 2026-11-03, at SJL-14.1, before any evidence of SJL-14 existed, under the ledger's
   update rule 5, by decision D49: the second sentence read "The pristine consumer and the
   downstream candidate-feed runs pass." Broiler.JSeal and Broiler.HtmlBridge are not changed by
   this programme, and a downstream failure SJL-14 did not cause, such as Broiler.JSeal's reflection
   on members SJL-12.5 removed, cannot be SJL-14's to fix; the change of meaning is a gate revision,
   not a reading. No evidence of SJL-14 existed to re-evaluate.)*
6. Broiler.JS's parse measurement meets its rule.
   *(Noted 2026-11-03, at SJL-14.1: not revised. What this clause becomes while no controlled lane
   exists is the owner's own choice (decision D48), which SJL-D-23's note of 2026-10-20 reserves:
   "That choice is the owner's own when it comes, not one taken under the owner's direction of
   2026-10-18." It is put to the owner with two alternatives, (i) keeping the clause, unmet until a
   controlled lane exists, and naming it by a dated note on SJL-D-23 among the clauses its trigger
   (b) lists, so that trigger (b) stays reachable, or (ii) revising it before any evidence, under update rule 5, to "Broiler.JS's parse
   measurement meets its rule on a controlled lane, or, until one exists, is taken under its rule
   and recorded void". Until the owner chooses, the clause stands as written, and Broiler.JS's
   rule's part I is void, neither MET nor NOT MET, on a lane that is not controlled.)*

---

## Evidence bundles

Broiler.JS `docs/evidence/sjl-14-001/`.

---

## Does not do

- **It does not share the static semantics.** That is SJL-15.
- **It does not change Broiler.JS's back ends,** neither the IL nor the portable bytecode back end.
