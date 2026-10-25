<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0081 - Regular expressions move to Broiler.Regex under its owner conditions: SJL-11's area verdict, sub-area by sub-area, decision SJL-D-14 recorded, and the owner's decisions of 2026-10-20 and 2026-10-21

**Status:** Decided 2026-10-21.
- **All six sub-areas are accepted** and placed with Broiler.RegEx, in its package `Broiler.Regex`:
  - **S**, syntax and early errors, with this family's compile-time validation of a literal;
  - **M**, compilation and matching;
  - **B**, the budget and the limits;
  - **F**, flags;
  - **T**, the text helpers, `RegExp.escape` and `source`;
  - **U**, the use of Unicode data.
- **The placement is under owner conditions**: the area design's requirements 1 to 11, which Broiler.RegEx's
  roadmap numbers R1 to R11, adopted by its dated record of 2026-10-20 (merged into Broiler.RegEx's default branch at `e568b88` by Broiler-Platform/Broiler.Regex#6;
  "Milestone" below). **A condition not met by the end of SJL-11.2 refuses the area** (decision D1).
- **Decision SJL-D-14 is recorded**, as this area applies it: under (a), a new backend is written in Broiler.Regex
  with this family's matcher as its specification and no code ported, and this family's matcher is deleted at
  SJL-11.4 (decision D5); under (i), duplicate named groups are implemented there before Broiler.JS retires its
  translator.
- **The owner's decisions D1 to D41 are recorded**: D1 to D40 taken on 2026-10-20 and D41 on 2026-10-21, under
  the owner's direction of 2026-10-18. Where the correspondence showed a recommendation's text to be wrong, the
  decision is recorded as corrected, and the text it replaces is quoted.
- **This family's corrections are listed before any run**: row S1 (kind D), and rows S5, S10, M4 (a non-empty
  match), M7 and T3 (kind R). **Broiler.JS's are listed by row** (decision D23), revised before evidence.
- Nothing in this profile changes with this record. The cut-overs are SJL-11.4's and SJL-11.5's own changes, and
  this record is what they apply.

**Date:** 2026-10-21.

**Owner:** the JavaScript profile owner.

**Co-signers:**
- the Broiler.RegEx maintainer, as the area's owner under SJL-D-14 (a);
- the Broiler.JS maintainer, as the other engine's owner;
- the core architecture owner, for rule I1's allow-list and the composition register.

**All four roles are held by one person** (EX-30), and this record does not claim that anyone independent has
read it.

**Milestone:** the programme's SJL-11, work package SJL-11.1.
- The same verdict is recorded in Broiler.JS's shared-libraries plan, `docs/roadmap/SharedLibraries.md`, section
  "Area verdicts", "Regular expressions (SJL-11.1)" (filed at `0f21a84c`,
  merged into Broiler.JS's default branch at `f918b040` by Broiler-Platform/Broiler.JS#1069).
- Broiler.RegEx's `docs/roadmap.md` section 6 records the owner conditions it adopts, by its record of 2026-10-20
  (filed at `fca7a6d`, merged into Broiler.RegEx's default branch at `e568b88` by Broiler-Platform/Broiler.Regex#6).
- Each of the two was observed on its branch until its change reached its repository's default branch; both had
  merged before this record did, and the ledger names their merge commits.
- **SJL-11 started before SJL-7, which it waits on, and SJL-3 met their gates**, at the owner's words of
  2026-10-19, "Also continue with the other remaining SJL milestones". SJL-11 may not meet its gate before SJL-7
  meets SJL-7's, and so not before SJL-3 meets SJL-3's. This record moves no gate.

**The owner's direction.** On 2026-10-18 the owner wrote: "Please do all recommended decisions and skip waiting
for review." This record reads it in two parts, as JSD-0078 and JSD-0080 did:
- each decision SJL-11's plan leaves open is taken as the plan recommends, and recorded below as the owner's
  decision under that direction. Decisions D1 to D40 were taken on 2026-10-20, the date Broiler.RegEx's record
  carries. The corrections the correspondence forced on their text were recorded at filing, on 2026-10-21: decision
  D5's reason, D7's count, D10's attribution of the ceilings, D18's check, D23's rows, D30's reason, D36's rows
  S10 and M7, and D37's presumptions. Decision D41 was taken on 2026-10-21 under the same direction, as the
  correspondence's review recommended it that day: a dated owner decision that records the one row no other
  class could hold as a defect of the specification's text;
- the programme proceeds through SJL-11's work packages without pausing for the owner between them.

**The direction changes no rule of the programme.**
- `Accepted` still needs review (the ledger's update rule 7), so nothing of SJL-11 is accepted by it.
- SJL-11 still may not meet its gate before SJL-7 and SJL-3 meet theirs.
- Rules still come before runs. Bundle `sjl-11-001`'s rules are committed in each repository before any code they
  judge.

**Names.**
- **Decisions.** "Decision Dn" is always this record's. Another record's decision is named with its record, as in
  "JSD-0080's decision D23". The programme's risks are "risk D8" and "risk D10" (`risks.md`).
- **Rows.** "Row S1" to "row U3" are the correspondence's rows, by sub-area: S1 to S11 (with S2a, S2b, S3b and
  S9b), M1 to M7 (with M2a to M2c), B1 to B8, F1 to F3, T1 to T5, U1 to U3. A correction of this family is of kind
  D (duplicate named groups) or kind R (any other row), as bundle `sjl-11-001`'s rule names them.
- **Requirements.** R1 to R11 are Broiler.RegEx's roadmap's numbers for the area design's requirements 1 to 11.
- **Steps.** A step of SJL-11 is named by its work package, SJL-11.1 to SJL-11.6, and by the file or member it
  changes. SJL-11's plan is held in no repository, so its task numbers appear here only where a decision quotes
  its text.

---

## Context

The area's design is the programme's
[`areas/regexp.md`](../../../../docs/javascript-shared-libraries/areas/regexp.md). It has no vectors yet. SJL-11.3
adds them after this record, in the corpus's next revision, and classifies both engines' first runs in the revision
after it.

**The correspondence is from code, and every behavioural cell was run.** It reads:
- this family at **`aaa39fb`**, the default branch after the preview.7 re-pin;
- Broiler.JS at **`1f91b8f1`**, the default branch after its PR #1057;
- **Broiler.Regex `0.1.0-preview.4`**, which Broiler.JS restores for every `u` and `v` pattern, at the commit its
  nuspec names, `c1422f4d`.

Each cell was run in this family through the conformance root, and some also through the command-line root, to
show how it reports a stop. In Broiler.JS each was run through its shell, and the case-insensitive rows again under
`tr-TR` and `lt-LT`. Where the shell's fallback to .NET hides the package's own answer, the package was called
directly.
[`correspondence.md`](../../../../docs/evidence/sjl-11-001/correspondence.md), which this record's change retains
in bundle `sjl-11-001` with every probe and each engine's answers in `correspondence-runs/`, names every member by
file and line and every row with its class.

**The default branches moved while the cells were taken**, and the correspondence reads each move:
- at this family's `daa92eb`, merged on 2026-10-21 by SJL-12.6 item 2, the literal's check moved from the
  lowering's `CompileRegExpLiteral` and `RefuseRegExpLiteralFlags` into the binder's `CheckRegExp`
  (`JsBinder.EarlyErrors.cs`), unchanged by its own remark: the same flag checks, the same compile by this
  family's matcher, the same code at the literal's span;
- at Broiler.JS's `792ff56e`, the validators moved to `Broiler.JavaScript.Ast` with comments and a forwarder as
  their only change, and the scanner's literal scan did not change;
- no file of Broiler.Regex's package changed.

No cell could answer differently at either default branch, by reading, and none was run again there.
**SJL-11.3's vectors and the reference arms confirm or refute each row**, and a difference they find that this
record does not classify falsifies it.

**What the correspondence found:**
- **This family's matcher is the reference for resource behaviour, and has defects of its own.** It refuses
  every duplicate group name (row S1), accepts `\k` in a class beside a named group (S5), refuses a lone lead escape
  before a braced escape under `u` (S10), reports the pair's start as `"index"` when `lastIndex` is inside a
  surrogate pair (M4), compares a back-reference by code units under `u` (M7), and copies a raw line terminator
  into `source` (T3). Rows S10, M7 and T3 each have a test262 file that fails at this family's `sjl-12-001` base.
  Its scanner faults on a literal ending in a backslash (S9b), which is the front end's, not this area's.
- **Its own remark is refuted.** The matcher says neither ceiling "is reachable by a pattern that was going to
  answer". A greedy `a*` over a long enough subject stops at the frame ceiling, and a loop of captures at the trail
  ceiling (rows B1 and B6): a greedy quantifier is one `Run` instruction over a class only.
- **A ceiling stop names no dimension.** The conformance root reports it as `ProfileFault/AllowanceExhausted` with
  no payload, and the command-line root exits with its "host defect" code, where a fuel exhaustion exits with its
  own.
- **Broiler.JS's defects are its translator's, its routing's and its literal check's**: .NET's casing and the
  host culture (M2, M2a, M2b, U3), .NET's line terminators (M3), a quantified look-behind (S7), the `v` flag's
  reserved syntax (S8), errors reported only when a literal is evaluated (S11), an escaped duplicate name (S2a), a
  literal check that disagrees with the constructor (S2b), a class that joins a lead escape to a braced trail
  escape (S10), `source` rewriting (T1), `lastIndex` inside a pair (M4) and `String.prototype.search`'s direct
  call (M6).
- **Broiler.Regex `0.1.0-preview.4`'s defects reach Broiler.JS on its `u` and `v` patterns**: a budget reset at
  every start position that answers a wrong index, a flipped look-around, a shortened match or no match (B1 to B3);
  uncharged alternation (B4); a stack guard that falls back to .NET's answer or to no match (B5); a matcher that
  recurses once per term (B8) and a parser that recurses once per nesting level (S3b), each ending the process; and
  a backward back-reference that starts inside a pair (M7). Others are masked in Broiler.JS by its fallback: loose
  property names (S6), escaped group names and their exception kind (S2), the refusal of valid duplicate names
  (S1), `\0` before a non-ASCII digit (S4) and `\k` accepted by the parse alone (S5).
- **One row is a defect of the specification's text**: the empty match from a `lastIndex` inside a pair under
  `u` (M4) gives a Match Record whose start passes its end. Decision D41 classes it.
- **Some test262 files under the area's paths are exhausted at this family's base on a nested-load allowance**,
  which no regular-expression code charges. They are outside the matcher, and the correspondence names them without
  a row.

**What the design reads as current and is not:** section "The design's readings".

## The owner conditions (SJL-11.1)

SJL-D-14 (a) places the area with Broiler.Regex **if it meets every requirement the area design lists**, which
are this family's. Broiler.RegEx adopts them as owner conditions by the dated record of 2026-10-20 in its
`docs/roadmap.md` section 6, which numbers them R1 to R11 (filed at `fca7a6d`,
merged into Broiler.RegEx's default branch at `e568b88` by Broiler-Platform/Broiler.Regex#6). It meets them at SJL-11.2, and its bundle `sjl-11-001`'s rule judges them at the
candidate reading, the commit `0.2.0-preview.1` is packed from:

| | Requirement | Where Broiler.Regex stands at `c1422f4d` (correspondence) |
|---|---|---|
| R1 | A caller-owned work budget, exhaustion a distinct result, cancellation through the meter | A fixed allowance reset at every start position (B2), counted only in quantifier loops (B4); exhaustion answers no match (B3) or a shorter match (B1) |
| R2 | Compile-time charging in proportion to the work | Uncharged (B7) |
| R3 | Frame and trail ceilings from the caller's policy | None beyond the step limit |
| R4 | Bounded nesting without CLR recursion | The parser recurses per nesting level (S3b) and the matcher per term (B8); the stack guard falls back (B5) |
| R5 | Exact syntax: property names, group names, Annex B | Loose names (S6), escaped group names refused or thrown (S2), `\0` before a non-ASCII digit (S4), `\k` accepted by the parse (S5), a lone lead escape before a braced one refused (S10) |
| R6 | ES2025 duplicate named groups, before Broiler.JS retires its translator | Every duplicate refused (S1), and an escaped duplicate in one alternative accepted (S2a) |
| R7 | An anchored match | A forward search only |
| R8 | An offset-only result interface | Eager substrings |
| R9 | Flag parsing, `RegExp.escape`'s text, `source` escaping | Flag parsing only |
| R10 | Process: AOT and trimming declared, a Native AOT probe, test262 in CI, a fuzz target, the `SL` conventions, an assurance gate | Publishes under Native AOT without a warning, declaring neither flag; no probe, fuzz target, test262 run or assurance gate in its CI |
| R11 | Data through the Unicode owner's provider | Tables compiled into the package, a case-folding table of its own |

**The refusal path.** If any condition is not met by the end of SJL-11.2, the area is refused. Neither engine moves
onto the package, and each keeps its own engine with a source-level pointer to this record, so the duplication
stays visible (the programme's roadmap section 5, "On refusal"). SJL-D-14 (a) has no fallback home, so SJL-11's
row then moves to `Refused` under the ledger's update rule 9, in the change that files the refusal, naming the
failed condition, what would reopen it and where the source pointers are. A new dated choice may then reopen
SJL-D-14 (risk D8), which moves the row back to `In progress`. **An engine cut over before every condition holds is
risk D10's stop.** Nothing here weakens either.

## The gate, sub-area by sub-area

### S - syntax and early errors, with literal validation

**X1 - met.**
- **This family** parses with its matcher's parser (`JsRegExpMatcher`, in Format), and its compiler validates a
  literal with the same parser, "so that the two cannot disagree about what a pattern is": since SJL-12.6 item 2,
  in the binder's `CheckRegExp`.
- **Broiler.JS** validates a literal in `Parser/RegExpValidator` with three prechecks (the Unicode pattern, group
  names, property names) ending in a .NET `Regex`, refuses in the constructor through the same prechecks and the
  translator, and parses every pattern with Broiler.Regex's parser first.

**X2 - met.** Every row is classified:
- **Corrections of this family:** S1 (kind D); S5's class case and S10 (kind R).
- **Corrections of Broiler.JS:** S2a, S2b, S7, S8, S10's class case and S11; S3b's process abort, with the library.
- **Corrections of Broiler.Regex's preview.4**, where Broiler.JS's fallback masks them: S1's refusal of valid
  duplicates, S2, S2a, S4, S5's parse-only acceptance, S6 and S10's refusal.
- **Engine policy that becomes a parameter:** S3, the nesting limit, and S3b's deep but finite nesting, which
  becomes a listed refusal in Broiler.JS (decision D7).
- **Outside SJL-11:** S9, an invalid literal read as division, which is the shared front end's and SJL-14's
  (decision D22); and S9b, this family's scanner, which is SJL-12's and SJL-13's front end. S9b's test262 file fails
  at both commits of SJL-11.4 and is no C1 row.

**X6 - extended.**
- **This family:** the parser in `JsRegExpMatcher.cs` and the public `JsRegExpSyntaxError`, deleted with the file.
  The binder's `CheckRegExp` is re-pointed onto Broiler.Regex's validate entry and the shared flag parser, keeping
  its diagnostic code and the literal's span (decision D18). `CompileRegExpLiteral`'s remark, which says the
  grammar lives in the format assembly, is corrected in the same change.
- **Broiler.JS:** `Parser/RegExpValidator` (the scanner's `IsValid` call re-pointed to the validate entry);
  `RegExpUnicodeValidator`, an (a) forwarder to the validate entry wherever SJL-12.5 moved it; the regular-expression
  prechecks of `RegExpGroupNames` and `JSUnicodePropertyNames`; the scanner's `DecodeBracedUnicodeEscapes` and its
  parenthesis-balance patch. The division fallback stays, SJL-14's.

### M - compilation and matching

**X1 - met.**
- **This family** compiles to an instruction array run by a backtracking interpreter with explicit frames and an
  undo trail (`JsRegExpMatcher`), called from the realm (`JsRealm.RegExp`).
- **Broiler.JS** translates every pattern but its `u` and `v` patterns and a few gap shapes to .NET's `Regex`, and
  runs those through Broiler.Regex's continuation closures.

**X2 - met.**
- **Corrections of this family:** M4's `"index"` and substring for a non-empty match, and M7, both kind R.
- **Corrections of Broiler.JS:** M2 (F2 of bundle `sjl-7-001`), M2a, M2b, M3 (W5, and `$` without `m`), M4's
  half-pair matches and M6. M7's look-behind is a correction of Broiler.Regex's preview.4 that Broiler.JS shows.
- **Implementation-defined by decision:** M4's empty match (decision D41).
- **Same in both:** M1, M2c and M5. M5's sticky emulation costs work on Broiler.JS's .NET path that no answer
  shows; the anchored argument removes it.

**X6 - extended.**
- **This family:** `JsRegExpMatcher.cs` whole: the public `JsRegExpMatcher` and `JsRegExpMatch`, and
  `JsRegExpCase` and `JsRegExpCharSet` with the lowering and the runner. The realm's `RegExpCompile`, `RegExpRun`
  and `Reinitialise` become the adapter (decision D10), and `RegExpMatchOne` keeps its step back to the pair's start
  for the matcher only: the `"index"` and the substring it reports start at `lastIndex` (row M4).
- **Broiler.JS:** the translator, every member from `ClampLargeQuantifiers` to `HexValue` that is not object-model
  code; `JSRegExp.UnicodeSets`; `RegexTiering` and its diagnostics; `GapScan`, `TryRouteToBroiler`, `FromNet`, the
  overflow fallback and the .NET `value` field; both `CreateRegex` overloads; `String.prototype.search`'s direct
  `value.Match` call. `Exec`, `Split`, `Replace`, `matchAll`, the string methods and `LegacyRegExpState` keep their
  members and re-point their bodies.

### B - budget and limits

**X1 - met, through Broiler.Regex for Broiler.JS.**
- **This family** charges every instruction, character examined, property weight, back-reference length and
  prefilter skip to the realm's fuel in blocks, and stops a match at its frame and trail ceilings and its nesting
  limit.
- **Broiler.JS** bounds nothing on its .NET path, and on its `u` and `v` path Broiler.Regex's per-start allowance
  and stack guard bound it.

**X2 - met.**
- **Corrections of Broiler.JS and of Broiler.Regex's preview.4:** B1's shortened match and no match, B2's wrong
  index, B3's flipped look-around, B5's fallbacks and B8's process abort.
- **Correction of Broiler.Regex's preview.4:** B4, uncharged alternation. Broiler.JS passes no meter, so its answer
  does not move.
- **Engine policy that becomes a parameter:** B1's and B6's stops in this family, at its frame and trail ceilings,
  which it passes to the new backend (decision D10); B7, the compile charge, which moves under decision D8.

**X6 - extended.**
- **This family:** `JsRegExpCharge`, `JsRegExpOverflowError` and the runner's ceilings, with the file; the
  compile charge `source.Length + 32` in `RegExpBuild`, replaced by the meter.
- **Broiler.Regex:** `Budget`, `StepLimit`, the per-start reset, the fast path's early exit, the stack guard and
  `RegexOverflowException`.

### F - flags

**X1 - met.** This family parses flags in `RegExpNormalizeFlags` and, for a literal, in the binder's
`CheckRegExp`. Broiler.JS parses them in `ParseFlags` and `BuildFlagsString`, and Broiler.Regex in
`RegexFlagsParser`.

**X2 - met.** F1 and F2 are the same in both. F3 is the same in both for the valid modifiers and for the
constructor; the literal's late refusals are row S11's.

**X6.** This family's `RegExpNormalizeFlags` and the binder's flag checks onto the shared flag parser; Broiler.JS's
`ParseFlags` and `BuildFlagsString` likewise.

### T - `RegExp.escape` and `source`

**X1 - met.** This family escapes in `RegExpEscapeText`, `RegExpEscapeCodePoint` and `RegExpEscapeHex`, and writes
`source` in `RegExpSourceText`. Broiler.JS escapes in `RegExp.escape`'s `Escape` and `TryAppendEscape`, and writes
`source` in `EscapePatternSource`.

**X2 - met.**
- **Correction of this family:** T3, a raw line terminator in `source` after a backslash (kind R).
- **Correction of Broiler.JS:** T1, a literal's `source` rewritten where its constructor twin's is not.
- **Implementation-defined:** T2, whether a `/` in a class is escaped: each text is equivalent, and each engine
  gives one text for one pattern and flags.
- **Same in both:** T4 and T5.

**X6.** This family's four members, deleted, onto R9's helpers. Broiler.JS's `Escape`, `TryAppendEscape` and
`EscapePatternSource` onto the helpers.

### U - data use

**X1 - met.** This family reads Format's tables, which Broiler.Unicode's generator writes since SJL-7.4. Broiler.JS
reads Broiler.Unicode's tables through `JSUnicodePropertyNames` and Broiler.Regex, and .NET's casing data on its
non-`u` path.

**X2 - met.** U1 and U2 are the same in both. U3 is a correction of Broiler.JS by data decision, class C5 under
SJL-D-6 (c): .NET's casing predates the pinned Unicode version.

**X6.** Broiler.Regex's own tables, `CaseFolding`, its data and its generator, deleted under R11 (decision D11).
Nothing of this family's: its tables stay where SJL-7.4 put them.

### X3 - neutral contract, all six sub-areas

**X3 holds only when SJL-11.2 ends**, which is why the placement is under owner conditions. The contract the
conditions give is neutral (decision D9):
- the meter is Broiler.Regex's own nullable delegate, and a status enum answers a match, no match, a limit
  exceeded with its dimension, or a syntax status with a code, a message identifier and a position;
- results are offsets into the caller's slots;
- data comes through the provider contract `IUnicodeTables` (decision D11);
- no engine type, no coercion, no property access, no culture.

The contract conventions (the programme's `architecture.md` section 4) that Broiler.Regex breaches at `c1422f4d`
are:
- convention 1, neutral types: BCL collections and `Func` types in public signatures;
- convention 3, failure as data: exceptions for expected failures, `ArgumentOutOfRangeException` among them (row S2);
- convention 4, the caller's meter: none;
- convention 5, explicit limits: none, and the stack as a limit (rows S3b and B8);
- convention 7, no mutable static state: a static cache;
- convention 9, platform independence: `char.IsDigit` (row S4) and `char.ToLowerInvariant` beyond ASCII;
- convention 10, thread safety: shared `CodePointSet`s normalised lazily in place;
- convention 12, Native AOT and trimming: neither `IsAotCompatible` nor `IsTrimmable` declared.

R1 to R11 close each of them (decisions D9, D11 and D31). Broiler.RegEx's architecture tests assert the list of
today's violations from SJL-11.2's first change, and shrink it to none by its end.

### X4 - no edge between the engines, none into Broiler.VM

Broiler.Regex references neither engine and nothing in this repository. After R11 its one dependency is
`Broiler.Unicode.Algorithms`, which both engines already admit. Neither engine references the other.

### X5 - boundaries hold

- **No platform regular-expression API enters.** Broiler.Regex references no `System.Text.RegularExpressions`
  type, and rule I2 refuses one in an admitted package; Broiler.JS's .NET path retires with its translator.
- **R11 removes the table packages from both closures.** Broiler.Regex's preview.4 brings `Broiler.UniCode.*`
  packages transitively, which no rule of this family refuses; R11 replaces them with the provider (decision D29).
- **The Compiler gains a reference to Broiler.Regex for literal validation from SJL-11.4 until SJL-13** (decision
  D18). The runtime's reference replaces this family's matcher, which Format carries into every root today, the
  compiler-free ones included; Broiler.Regex brings no JavaScript front end into any of them.

### X7 - owners accept

The JavaScript profile owner, the Broiler.JS maintainer and the Broiler.RegEx maintainer accept all six sub-areas
under the owner conditions. One person holds the three roles (EX-30).

## Decision SJL-D-14, as this area applies it

The programme took SJL-D-14 on 2026-10-11: (a), chosen by the owner, and (i), as recommended. This record files it.
- **(a): Broiler.Regex is fixed until it meets every requirement, then both engines move onto it, and this family's
  matcher is deleted.** The fix is a new backend written in Broiler.Regex (decision D5). This family's matcher is
  its specification, for its design and its charging units, and no code of it is ported: (b), which would have made
  this family's matcher the package's implementation, was not taken.
- **The three-part comparison** planned to choose between (a) and (b) is each cut-over's acceptance check: each
  repository's bundle `sjl-11-001` rule, committed before any code it judges.
- **(i): duplicate named groups are implemented in Broiler.Regex** (R6) before Broiler.JS retires its translator.
  This family, which refuses them today, gains them at SJL-11.4 as a listed correction (row S1, SJL-D-16 (a)).

## The design's readings

Each is corrected in `areas/regexp.md`, or in the milestone where it repeats one, beside the reading, which is
quoted there, in this record's change (decision D40).
- **Requirement 4, "without CLR recursion"**, is read as no CLR recursion in the matcher, the parser and the
  compiler bounded by a nesting limit checked before each descent (decision D6).
- **Section 4's Work and What survives**: the fix is a new backend, not a repair of the closure-based design, and
  what survives is the parser, the public tree, the `CodePointSet` algebra and the flag parser (decision D5). The
  differential history against V8 is local evidence not in Broiler.RegEx's repository; no population cites it.
- **Section 6's second risk row** reads the new backend as the planned route (decision D5).
- **The charging units** are this family's, re-expressed as the backend's instructions (decision D8).
- **Convention 1 admits `IUnicodeTables`** in a public signature (decision D11).
- **SJL-D-3's "inside a major version"** reads, for a `0.x` preview, as inside a `0.y` minor version (decision D12).
- **The admission's departure** from `SuiteFeatures.cs`'s condition (decision D15).
- **Section 2's stale cells**: this family's `RegExp.escape` classifies white space through the shared lexical
  classes, not `CharUnicodeInfo`; Broiler.JS's property names and group names are exact since SJL-7.5 and its
  PR #1041; and the overflow cell is two rows, a stack guard that falls back and a budget that answers no match.
- **Section 5's lists** are extended (decision D20).

## Decision

Each decision was taken by the owner under the direction quoted above, as SJL-11's plan recommends it, on
2026-10-20 (D1 to D40), or as the correspondence's review recommended it, on 2026-10-21 (D41), and states its
reason. The corrections the correspondence forced on decisions D1 to D40 were recorded at filing, on 2026-10-21,
each beside the plan's text it replaces. Broiler.RegEx's roadmap record carries decisions D5, D6, D9, D11, D12, D14
and D30 to D32, and Broiler.JS's area entry decisions D7, D10, D20 to D23, D33 and D34, by these numbers.

### The verdict and its records

**Decision D1 - the verdict's shape.** Accept the area in its six sub-areas, placed with Broiler.Regex under R1 to
R11 as owner conditions, adopted by a dated record in its roadmap section 6. Any condition not met by the end of
SJL-11.2 refuses the area, and each engine keeps its own engine with a source pointer to this record. The reason:
JSD-0074's owner-condition model; X3 and X5 hold only after R1 to R5, R8 and R11; `areas/regexp.md` section 6
already names the refusal.

**Decision D2 - the start record.** One dated ledger entry, SJL-11.0, quoting the owner's words of 2026-10-19 and
the standing direction; the row to `In progress`; SJL-11 may not meet its gate before SJL-7 and SJL-3. **One start
entry per milestone**, so SJL-9 and SJL-17 file their own. The plan left the alternative open: "One entry for SJL-9,
SJL-11 and SJL-17 if they start together (task 0.2)", and the plan's preparatory step chose one each. The reason:
the precedent of SJL-8.0 and SJL-10.0, and one entry per milestone keeps each row's history in one place.

**Decision D3 - numbers.** This record is JSD-0081; JSD-0080 is SJL-12's. The cut-over JSD takes the next free
number at SJL-11.4. Bundle `sjl-11-001` in all three repositories. No JSC entry and no exclusion at SJL-11.1: a JSC
at SJL-11.4 and an EX at SJL-11.6, each the next free at filing. The plan's text named the numbers free when it
was written, "JSC-319 today" and "EX-133 today"; this record names none, because SJL-12 may take either first and
a number is taken at filing. The reason: nothing reserves numbers; the precedent of SJL-6 to SJL-10.

**Decision D4 - this record's people.** The owner, the co-signers, the EX-30 sentence and "Decided" with the filing
date, as the header states. The reason: as JSD-0074.

### The library

**Decision D5 - the backend.** A new explicit-stack, defunctionalised backtracking machine written in Broiler.Regex:
a compiled instruction program run by one loop with a frame stack and an undo trail. This family's design and units
are its specification, and **no code is ported**. The parser, the public tree, the `CodePointSet` algebra and the
flag parser stay; the fast paths are re-expressed as instructions under the meter. It is within SJL-D-14 (a). The
reason: the continuation chain recurses once per term, so a flat sequence of terms ends the process (row B8);
SJL-D-14 (b) and `areas/regexp.md`'s "Nothing in code" forbid the port; Broiler.RegEx's own roadmap section 4 names
this step. The plan's reason read "(a flat [n]-term pattern kills the process; parse-only does not)", its size
left out here (validation principle 5), the input being row B8's run in bundle `sjl-11-001`: the second half holds
for a flat sequence only, since deep nesting ends the process in the parser (row S3b), which decision D6 bounds.

**Decision D6 - requirement 4's reading.** No CLR recursion in the matcher. The parser and the compiler may recurse,
bounded by the caller's nesting limit, checked before each descent and answered with the nesting status, under a
library maximum shown to complete on the smallest stack the adversarial corpus declares, under the JIT and in the
Native AOT probe. The reason: R4's exit criterion ("a nesting limit checked before the stack is, an explicit stack
where recursion remains"), the programme's governance obligation 2 ("depth limits that are checked before the stack
is") and contract convention 5 admit depth limits checked before the stack; this family's own parser recurses
within its limit; rewriting a parser that is not the defect would add risk without a gain. The plan's text read
"under a library maximum proven on a 1 MiB thread under JIT and Native AOT", and its reason named governance
obligation 2 alone and said this family's parser "recurses within 512". Broiler.RegEx's adversarial corpus declares
that stack in its header, and this record names the declaration rather than the size, so that the two cannot drift
apart; convention 5 is added because it states the same bound for the contract; and the limit is decision D7's.

**Decision D7 - the nesting limit.** A caller parameter with its own status. **Both engines pass 512, counted as
this family's matcher counts it**: the pattern's own disjunction is depth 1, each group of any kind adds 1, and under
`v` each class adds 1, the outermost included. A limit admits its own depth and refuses one more, so **511 nested
groups are accepted and 512 refused**, in this family today and in both engines after (row S3). This family maps the
nesting status to its `SyntaxError`, so its verdicts do not move. Broiler.JS's deeper patterns, which it accepts
today (row S3b's finite depths), become listed refusals, class C3 of its rule (SJL-D-16). The plan's text read
"Both engines pass 512." without the count. The reason: it keeps this family's verdicts; JSD-0080's decision D27
sets the same policy for source nesting.

**Decision D8 - the charging units.** This family's units are the contract: one unit per instruction dispatched or
character a run examines, a property class's weight per character tested, a back-reference's length, prefilter
skips; **prepaid in blocks of 512 before the work**, with the over-charge below one block documented; compilation
charged in proportion to the expansion, which replaces this family's `source.Length + 32` (row B7). A moved
exhaustion verdict of this family is class C2 of its rule, only on a variant that reaches a regular expression and
only if the variant passes at ten times the allowance. There is no regular-expression fuel twin. The reason: the
milestone admits a moved verdict "admitted"; template T7 has no C2, so a twin would fail by construction.

**Decision D9 - the meter and the statuses.** Broiler.Regex's own nullable delegate, `RegexWorkCharge(ulong units)`;
a status enum (a match, no match, a limit exceeded with its dimension: steps, frames, trail, nesting or compile; a
syntax status with a code, a message identifier and a position); no exception for an expected failure; a meter that
throws propagates, and leaves no shared state changed. The reason: contract conventions 1 and 3 to 5, JSD-0074's
"its contract types are its own", and SJL-D-11's shape.

**Decision D10 - the frame and trail ceilings, and Broiler.JS's budget policy.**
- **Both engines pass frames `1 << 20` and trail `1 << 21`**, this family's `MaximumFrames` and `MaximumTrail`.
  This family maps a limit exceeded to `JsAbort(Exhausted)`, as `RegExpRun` maps its matcher's ceilings today, so
  its stops at those ceilings stay (rows B1 and B6). Its roots report such a stop as they do today: the conformance
  root as `ProfileFault/AllowanceExhausted` with no dimension, the command-line root with its "host defect" exit
  code. How this family's rule reads that report is stated in its header and its parts A and D (bundle
  `sjl-11-001`'s `decision-rule-area.md`).
- **Broiler.JS passes no meter**, as in every shared area, and maps a limit exceeded to a catchable `RangeError` and
  the nesting status to its `SyntaxError`. The spent-allowance no-match becomes a correction: a catastrophic `u` or
  `v` pattern runs to the specification's answer, as its non-`u` patterns already do through .NET, listed as a
  behaviour change.
- **A dated milestone revision**: SJL-11.6's corpus "in both engines" holds in Broiler.JS for the classes that
  complete; the unbounded classes K and A are judged at library level, where risk S1's stop is read. Broiler.JS's
  test262 template T2 gains class C3 for a limit row (decision D39).

The plan attributed this family's ceilings to the nesting decision: its step for SJL-11.4's adapter (task 5.2) read
"frame `1<<20`, trail `1<<21`, nesting 512 (decision D7)". The ceilings are this decision's, and the nesting limit
decision D7's. The reason: consistent with Broiler.JS's shared-libraries plan and contract convention 4; it removes
the silent wrong answers clause 3 forbids; a per-match allowance would be a metering policy no other area has.

**Decision D11 - data (R11).** `Broiler.Unicode.Algorithms` `0.1.0-preview.5` only; compilation takes an
`IUnicodeTables`; `CaseFolding`, its data and its generator are deleted; the tests and the probe read
`Broiler.Unicode.Tables` test-only; the package declares no data version of its own, and rule I4 reads the
provider's. Contract convention 1 is read as admitting the provider contract `IUnicodeTables` in a public signature
(SJL-D-6 (c)), as JSD-0080's decision D20 types its providers. The reason: this family admits Algorithms and not
Tables, and the provider already has exact names, lexical classes, both foldings and string properties.

**Decision D12 - the API and the version.** R11 forces breaks of the `0.1` surface, and the backend removes
`RegexOverflowException` (decision D20). The SJL-11 surface ships as **`0.2.0-preview.1`**, validated against
`0.1.0-preview.4` with every break suppressed and listed with its R item; the string-bearing `RegexMatch` stays, as
R8 allows; no shim survives for a member R11 makes impossible. SJL-D-3's "inside a major version" is read, for a
`0.x` preview, as inside a `0.y` minor version. **No release of either engine depends on `0.2` before both cut-overs
merge**, and Broiler.JS moves its pin only at SJL-11.5. The reason: a table-less constructor cannot survive R11, and
minimum version ranges cannot protect a process that hosts both engines, so the release train does.

**The consumer check of SJL-D-25 (c)**, on which decision D21's removals rest, is recorded here. A GitHub code
search over the default branches of the `Broiler-Platform` organisation, run on 2026-10-19, found:
- no use of `CreateRegex`, `RegexTiering`, `RegexTieringDiagnostics`, `RegExpUnicodeValidator`, `JSRegExp`,
  `CaptureGroupMap` or `LegacyRegExpState` outside Broiler.JS, except Broiler.Unicode's test-only reference-arm
  copies;
- no call into Broiler.Regex's API from Broiler or Broiler.Browser, whose only references are `using` aliases of
  .NET's `System.Text.RegularExpressions.Regex` that work around the namespace.

Broiler.JS's area record states the same search for the members decision D21 removes (merged, as
"Milestone" above says). SJL-11.5 runs it again and records its queries and results in Broiler.JS's
`docs/public-api.md`.

**Decision D13 - publication.** `0.2.0-preview.1` is published to nuget.org only with the owner's approval, as
the earlier previews were. This family may admit a locally packed candidate on a branch for development, and re-pins
to the published package before it merges. The reason: the precedent of SJL-4.4 and SJL-10.4.

**Decision D14 - test262 in the component's CI.** A host job runs Broiler.JS's shell at a retained commit named by a
dated roadmap record (SJL-11.5's branch until it merges, then Broiler.JS's default branch), with the candidate from a
local feed and negative tests, holding an expected-failures list of host-attributed entries only, and every
duplicate-named-groups row passing. A host-free library job runs the literal syntax tests,
`property-escapes/generated` and `unicodeSets/generated` under the JIT and through the AOT probe. These are the dated
readings of R5, R6 and R10's exit criteria. The reason: a library cannot run test262 alone; before SJL-11.5 only the
cut-over branch exercises every pattern; Broiler.JS needs no committed feed to take a candidate.

### This family's cut-over

**Decision D15 - admitting duplicate named groups.** `regexp-duplicate-named-groups` enters `SuiteFeatures.cs`'s
`AdmittedProposals`, naming this record, in the **admission commit**, the first commit of SJL-11.4's branch, so the
files whose front matter lists the feature are in both of part A's populations. This departs from the condition its
remark states, that a proposal is admitted when "the profile implements the whole of it", and the departure is
recorded here with its reason. It also revisits JSD-0019's finding that these files are about the pinned edition,
which JSD-0018's exclusion kept out: they are admitted, as that finding says they should be. The plan's text read
"so the [n] files are in both populations", its count left out here (validation principle 5): the files are named by
their front matter, which decides the population. The reason: `eng/compare-test262-forms.py` ignores a row present
in one report only, so an admission at the cut-over would drop the files from part A silently.

**Decision D16 - where the bases are taken.** This family's at the admission commit, the after merge's first parent
being its parent; Broiler.JS's at the after merge's first parent, under a hold. A base whose commit is not the after
merge's first parent, or the admission commit on it, is void and is taken again. The reason: a base at the rule
commit would attribute SJL-12's, SJL-9's and SJL-17's changes to SJL-11 (validation principle 4, read on 2026-10-20).

**Decision D17 - where the inputs live.** Each engine's adversarial programs and workloads under its bundle's
`inputs/`, committed with its rule; Broiler.JS's benchmark class in a later inputs commit, after SJL-12's corpus
commit there. The reason: SJL-12's corpus-commit conditions admit only `docs/` (and in Broiler.JS `scripts/` and
`conformance/parse/`) between its rule and corpus commits, so a benchmark class under `Broiler.JS/benchmarks/` waits.

**Decision D18 - this family's literal validation.** Broiler.Regex's **validate entry**: parse and early errors, the
nesting limit, no expansion, no meter. This family keeps its diagnostic code and the literal's span. **The check
SJL-11.4 re-points is the binder's `CheckRegExp`** (`JsBinder.EarlyErrors.cs`), where SJL-12.6 item 2 moved it on
2026-10-21. The plan's step for SJL-11.4's compiler change (task 5.3) read "`JsCompiler.CompileRegExpLiteral` onto
the validate entry with no meter, keeping this family's diagnostic code and the literal token's span;
`RefuseRegExpLiteralFlags` onto the shared flag parser; the stale remark `:419-425`", which is where the check was
when the plan was written. The remark SJL-11.4 must correct, at `daa92eb`, is `CompileRegExpLiteral`'s, which says
the grammar lives in the format assembly: true while this family's matcher is there, and stale once SJL-11.4 moves
the grammar to Broiler.Regex. The Compiler references Broiler.Regex from SJL-11.4 **until SJL-13**, by a dated note
on `architecture.md` section 3.1. The entry is the
seam JSD-0080's decision D41 names for SJL-13. The reason: the compiler has no fuel; expansions are compilation's; one
parser keeps "the two cannot disagree".

**Decision D19 - the measurements.** A predeclared part of template T5 in each engine's rule, judged and recorded;
**not a gate clause**. A NOT MET is recorded in the cut-over JSD as "a slower but bounded matcher", the trade
`areas/regexp.md` section 6 names. Broiler.JS's part is void on a lane that is not controlled. The reason: SJL-11.1
names the measurement and no exit clause does; a clause would make the gate unmeetable for Broiler.JS, as SJL-3's
clause 7 is.

**Decision D20 - the deletion lists (X6).**
- **This family adds** to the milestone's list: the public `JsRegExpMatch`, `JsRegExpSyntaxError` and
  `JsRegExpOverflowError`, with every line their types hold in rule N10's baseline; `RegExpEscapeHex` and
  `RegExpSourceText`. It re-points `RegExpFuzzing.cs` to the adapter, keeping the `broiler-js-regexp-fuzz` session;
  `eng/collect-js-evidence.py`'s `PROFILE_MATCHER` and `REGEXP_CONTROLS` to an adapter line whose mutation the
  controls still catch; `Fuzzing.cs`'s remark on the matcher; and rule N18's vacuity guard. It drops the deleted
  file's rows from `HUMAN_REVIEW.md` and `assurance.manifest.json`, and registers every deleted public member in rule
  I5. The binder's `CheckRegExp` is re-pointed, not deleted (decision D18).
- **Broiler.JS adds**: `ParseFlags`, `BuildFlagsString` and the escape helpers onto R9's; the regular-expression
  prechecks of `RegExpGroupNames` and `JSUnicodePropertyNames`, wherever SJL-12.5 moved them; the tests and the
  benchmarks that test only the deleted members (`RegExpTranslatorFallbackTests`, `RegexTieringTests`, the clamp
  parts of `Issue810RegExpLargeQuantifierTests`, `RegexTieringMetrics`, and `RegexProfileMetrics`' .NET rows or the
  whole file), whose behaviour tests that stay are rewritten onto the adapter; `String.prototype.search`'s direct
  call.
- **Broiler.Regex**: `Budget`, `StepLimit`, the per-start reset, the fast path's early exit, `CaseFolding` and its
  data, the generator, the loose lookup, the static cache and `RegexOverflowException`.

The plan's text counted the baseline lines the three public types hold; the count is not a record's (validation
principle 5). The reason: rule I5 and the programme roadmap's section 6 item 11, deletion in the same change and no
dead path.

### Broiler.JS's cut-over

**Decision D21 - SJL-D-25's dispositions.** `RegexTiering`, `RegexTieringDiagnostics` and both `CreateRegex`
overloads: **(c) removal**, with the consumer check decision D12 records, re-run at SJL-11.5; their own tests
deleted with them;
`SharedUnicodeTests.cs:204-235` rewritten. `RegExpUnicodeValidator`: **(a)**, a forwarder to the validate entry.
`JSRegExp.Match`, `Split` and `Replace`: kept, their bodies re-pointed. `CaptureGroupMap`: kept if used, else (b).
The reason: (a) and (b) would keep a .NET `Regex` return type and the translator.

**Decision D22 - the scanner.** SJL-11.5 deletes `DecodeBracedUnicodeEscapes` and the parenthesis-balance patch,
which corrects `source` (row T1), and re-points the scanner's `RegExpValidator.IsValid` call to the validate entry,
which corrects the literal path's rows S2b, S7, S8 and S11. **The division fallback stays SJL-14's**, listed (row
S9). JSD-0080's decision D23 merged on 2026-10-19 as written, keeping the literal's `\u{…}` decoding for SJL-14, so
the plan's amendment before its merge did not happen; this change files a dated note beside JSD-0080's decision D23
instead, and beside the two other records that state the same placement: the SJL-12 milestone's reading of its
clause 1, and Broiler.JS's SJL-12 record of decision D23, whose note Broiler.JS's own change files (merged into Broiler.JS's default branch at `f918b040` by Broiler-Platform/Broiler.JS#1069). The reason: the scan is speculative, and the goal symbol is the shared front end's.

**Decision D23 - the corrections lists, revised before evidence.**
- **Broiler.JS's (SJL-11.5).** The milestone's list is revised, quoting the old text: loose property names and
  invalid group names were corrected at SJL-7.5 and by Broiler.JS's PR #1041, so they become deletions of the
  prechecks. The list is, by row:
  - the culture-dependent and .NET case-insensitive matching: M2 (F2), M2a in `tr-TR` and in `lt-LT`, M2b, and U3
    by data (class C5);
  - the translator's line terminators: M3 (W5), with `$` without `m`;
  - `lastIndex` inside a surrogate pair: M4's half-pair matches. **SJL-11.5's adapter, stepping back out of a pair,
    may move only the matcher's start, never the reported `"index"` or substring**;
  - `source` rewriting: T1;
  - the literal check: S2b, S7, S8 (test262's breaking-change patterns and two more `v`-mode classes) and S11 (the
    invalid modifiers and `v`-mode classes reported only at evaluation), their literal path closed by decision
    D22's re-pointing, and S7's and S8's constructor path, where the library's refusal is caught and .NET admits
    the translation, by the translator's retirement at SJL-11.5;
  - an escaped duplicate name in one alternative: S2a;
  - a class that joins a lead escape to a braced trail escape: S10's class case;
  - with Broiler.Regex's preview.4: the budget no-match, the shortened match, the wrong index and the look-around
    flip (B1 to B3); the stack-guard fallback, to .NET and to no match (B5, two rows); the process aborts of a flat
    sequence and of deep nesting (B8, S3b); the look-behind back-reference inside a pair (M7);
  - `String.prototype.search`'s direct call, with an instance's own `@@search` undefined or null and the
    `ReferenceError` on a pattern only Broiler.Regex holds: M6.
  - **Removed from the plan's list:** `\0` before a non-ASCII digit, which the correspondence shows is
    Broiler.Regex's alone and not observable in Broiler.JS (row S4); it is a library fix (decision D36). An invalid
    literal read as division stays SJL-14's (row S9).
- **This family's (SJL-11.4).** The milestone lists duplicate named groups only (row S1, kind D). The correspondence
  adds, each kind R: S5's `\k` in a class beside a named group, S10's lone lead escape before a braced escape, M4's
  `"index"` and substring for a non-empty match, M7's back-reference across a surrogate pair, and T3's raw line
  terminator in `source`. Its rule's part A lists each with its row, and the test262 files that fail at its base
  (rows S10, M7 and T3) are the ones a C1 (R) row must name.

The plan's text read: "added: the budget no-match, the stack-guard fallback to .NET or no-match (two rows), the
shortened match, the wrong index, the look-around flip, `String.search`'s direct call, `\0` before a non-ASCII digit,
F2 and W5". The reason: the precedent of SJL-7.1 and SJL-10.1; a list taken from the milestone's text would
misclassify test262 rows, and a C1 row no correspondence row names is a finding against this record.

### The rules and the evidence

**Decision D24 - the reference arms** are required (layer L2): arm V, a test-only copy of this family's matcher, and
arm J, a test-only copy of Broiler.JS's routing and translator over preview.4 as its own executable under three
cultures; the population generated, the V8 corpora not cited; charges recorded and not compared. Their class (b)
admits a policy's outcome and M4's empty match by decision D41. The reason: the programme's evidence matrix; the arms
find corrections and defects before either cut-over.

**Decision D25 - the vectors' oracle** is `regexpref.py`, ECMA-262 22.2 transliterated, with UCD through
`unicoderef.py`; never V8. `status: exhausted` vectors are library-level, with a margin of a hundred times from the
reference's step count. The correspondence's readings of the specification's text are checked by it, and a reading
it refutes reclassifies the row by a revision of this record. The reason: validation principle 1, and the earlier
areas' references written from the specification's text.

**Decision D26 - where the library's vectors run:** Broiler.RegEx's test project and probe, over a pinned copy of the
corpus files. The reason: the home's library runner covers only Algorithms.

**Decision D27 - the correspondence** is retained in this family's bundle as `correspondence.md` with
`correspondence-runs/`, its rows S, M, B, F, T and U; Broiler.Regex preview.4 is Broiler.JS's `u` and `v` column;
earlier rows are cited by bundle (F2 and W5 of `sjl-7-001`). The reason: the precedent of `sjl-10-001`.

**Decision D28 - the second Broiler.VM JSD**, "The profile on Broiler.Regex", at SJL-11.4, with its JSC entries
(JSW-4's gate, JSD-0031's matcher statements, `packages.md`'s baseline re-taken as a new `JS-10-0NN` bundle under
rule N37), the I5 rows and the N10 and N18 revisions; the EX at SJL-11.6. The reason: every area filed two, and
JSC-318 is rule N37's precedent.

**Decision D29 - the admission's scope:** product for `Broiler.VM.Profile.JavaScript` and its Compiler, not Format;
`NuGet.config` maps `Broiler.Regex`; rule I1's gap for preview.4's transitive `Broiler.UniCode.*` packages is
recorded as a finding, which R11 closes here, and no rule is minted (the programme roadmap's section 10 mints none at
SJL-11). The reason: SJL-11.4 moves literal validation to the shared engine.

**Decision D30 - the review state.** A development admission. A Broiler.VM release that depends on Broiler.Regex is
blocked until the revision it depends on carries a completed review (SJL-D-5 (a)). Broiler.RegEx's `HUMAN_REVIEW.md`
keeps its record, with a dated note. **The reason, corrected:** the reviewed commit, `d5b07a00`, is an ancestor of
Broiler.RegEx's default branch; the source under `Broiler.Regex/` changed after it, before `c1422f4d`; so, by the
record's own rule, no review covers the current code. The plan's reason read "[a count of the units reviewed]; the
recorded commit is absent", its count left out here (validation principle 5), and its section 3 "the reviewed commit
`d5b07a00` is absent after the history was rewritten": that was a reading of a shallow clone grafted at `c1422f4`,
and is false. The conclusion stands on the corrected ground.

**Decision D31 - thread safety.** `CodePointSet` is immutable after construction, no static field is mutable, and
concurrent compile and match answer as single-threaded calls do (part A's check A-11 of Broiler.RegEx's rule). The
reason: contract conventions 7 and 10, and the code's own falsifiers.

**Decision D32 - the fuzz target.** A seeded in-process grammar fuzzer in CI with every limit set and a
planted-defect control. This family's `broiler-js-regexp-fuzz` session is re-pointed, its controls kept. The reason:
R10, and risks S1 and S2.

**Decision D33 - Broiler.JS's culture part.** Its rule's part D: the regular-expression vectors identical under the
invariant culture, `tr-TR` and `lt-LT` at the after commit. The reason: case-insensitive matching on the .NET path
reads the host culture, and both `tr-TR` and `lt-LT` move its answers (row M2a), which no test262 run shows. The
plan's reason read "F2 reads the host culture; no test262 run shows it". That is false: row M2, which is bundle
`sjl-7-001`'s F2, answers alike in every culture the correspondence ran, and the row the culture moves is M2a.

**Decision D34 - Broiler.JS's deletion part.** Its rule's part E: a search and a scoped metadata check, with a
control; `JSIntl`'s, `DateParser`'s and the compiler's .NET `Regex` listed for their owners. The reason: it makes
clause 4 judgeable without claiming more than the area.

**Decision D35 - the order of the cut-overs.** Both after SJL-12's after runs and after the candidate reading;
either first; both in one release train; SJL-11.5's branch exists early for decision D14. The reason: risk D10's
stop, and processes that host both engines.

**Decision D36 - the small fixes in SJL-11.2.** `\0`'s lookahead ASCII (row S4); `\k` refused by the parser (S5);
group-name escapes and surrogates, and the duplicate check after decoding (S2, S2a); a lone lead escape before a
braced escape admitted (S10); a backward back-reference that never starts inside a pair under `u` and `v` (M7); the
publishing secret's name; the stale README and publishing text; `THIRD_PARTY_NOTICES.md`'s nested component;
`.gitattributes` and the test file with CRLF line endings. The reason: each is shown by a run or a read. Rows S10 and
M7 are added to the plan's list by the correspondence.

**Decision D37 - the adversarial corpus's home and its presumptions.** One library corpus in Broiler.RegEx; each
engine's programs generated from it with engine-specific declared outcomes, the corpus's SHA-256 and the commit in
each manifest. **The corpus's declared statuses presume four things of SJL-11.2's backend**, which every change of
SJL-11.2 that builds it keeps, from the parser's nesting check to the fast paths:
1. **no memoisation**: the backend searches in the specification's order and does not remember a failure at a
   position;
2. **no required-literal analysis** beyond the literal-prefix prefilter of SJL-11.2's fast paths, which charges its
   skips;
3. **a greedy run**: a greedy quantifier over one character class runs as one instruction with one backtrack point,
   adding no frame and writing no trail cell per character, as this family's `Run` does. **This holds for classes
   only**: this family runs a greedy quantifier over a single character or a group as a loop with a frame per
   iteration (row B1), so the corpus writes its greedy runs as the class `[a]`, and a backend that runs `a*` as a run
   moves B1's stop in this family into an answer, which its rule reads (bundle `sjl-11-001`'s
   `decision-rule-area.md`, part A's class C2);
4. **nesting depth counted as this family counts it** (decision D7), so that this family keeps its verdicts.

A backend that breaks one can answer as the specification does and still fail the corpus. The plan's decision read
only "One library corpus in Broiler.RegEx; each engine's programs generated from it with engine-specific declared
outcomes, the corpus's SHA-256 in each manifest." The reason: one source of cases, three judgements; the presumptions
are what makes a declared status hold across the change of backend.

**Decision D38 - this family's differential cross-check** (template T8): the differential programs' report retained
beside the parts without a rule. The reason: the precedent of `sjl-3-001`; T8 meets no conformance clause.

**Decision D39 - Broiler.JS's ceilings in test262.** The ceilings are decision D10's; any test262 row that reaches
one is class C3 of its rule, shown by a library run under the same policy. The reason: template T2 has no other class
for a limit row.

**Decision D40 - `areas/regexp.md`'s stale readings** are corrected at SJL-11.1, each by a dated note quoting it
(section "The design's readings"). The reason: the verdict must not rest on readings at `c1422f4d` that later
milestones changed.

### The owner's decision of 2026-10-21

**Decision D41 - the empty match from a `lastIndex` inside a surrogate pair is implementation-defined by this
decision, until the specification is corrected.** Row M4's `/(?:)/gu` at `lastIndex` 1 of a surrogate pair is a
defect of the specification's text: RegExpBuiltinExec sets `"index"` to `lastIndex` (1) and `lastIndex` to the match's
end (0), and builds a Match Record whose start passes its end, which GetMatchString's assertion forbids. The
correspondence records the text's reading and both engines' answers: this family answers `"index"` 0 and `lastIndex`
0, Broiler.JS `"index"` 1 and `lastIndex` 1.
- **The case is classed implementation-defined by this decision**, this case only, and not by the text: an
  ill-formed record is not a policy the specification leaves to the implementation, which is what the arms' class
  (b) otherwise admits.
- **The rules' class (b) cites this decision** for it: the reference arms' class (b), and the vectors'
  implementation-defined class at SJL-11.3, where the case is a vector with each engine's answer.
- **It lapses when the specification is corrected**: the corrected text's answer then decides, and a later
  revision of this record classes the case by it.

The decision is the owner's under the direction of 2026-10-18, taken as the correspondence's review recommended it on
2026-10-21: "a dated owner decision recorded as a defect in the specification". The reason: `regexpref`, written
from the text, has no answer to give for an ill-formed record, so without this decision the row would be class (c),
a defect of Broiler.Regex no change can fix; the non-empty matches beside it are determinate and stay corrections
(row M4).

## What this rejects, and why

- **Porting this family's matcher into the package** (SJL-D-14 (b)): the owner chose (a), and `areas/regexp.md`
  keeps "Nothing in code" (decision D5).
- **Keeping the `0.1` surface**: R11 makes the table-less constructors impossible (decision D12).
- **A per-match allowance in Broiler.JS**: a metering policy no other area has (decision D10).
- **Admitting duplicate named groups at the cut-over**: the comparator would drop the files silently (decision
  D15).
- **V8 as an oracle**: the specification and test262 are the oracles (decision D25).
- **Classing M4's empty match by the text**: the text has no well-formed answer (decision D41).

## Settling condition

This record is settled when SJL-11.4's and SJL-11.5's cut-overs have landed with both deletion lists absent, and
every part of the three repositories' `sjl-11-001` rules has been judged, whatever each decides.

## Correction entries

None in this profile's plan documents at this record. The JSC entries are SJL-11.4's (decision D28). These carry
dated notes beside the text they correct, each quoting it, in this record's change: the milestone file,
`areas/regexp.md`, `architecture.md` section 3.1, and SJL-D-14's "Recorded by" in `decisions.md`. JSD-0080's
decision D23 and the SJL-12 milestone's reading of its clause 1 carry dated notes in this change (decision D22).

## What this record does not decide

- **The cut-overs.** SJL-11.4 and SJL-11.5 apply this record, and SJL-11.4 files the second record.
- **The vectors.** SJL-11.3 adds them, and they may falsify this record.
- **The frontend's literal validation**, which is SJL-13's; the validate entry is its seam until then.
- **The division fallback**, which is SJL-14's.
- **Which rule part reads a ceiling stop how**: each repository's rule (for this family, bundle `sjl-11-001`'s
  `decision-rule-area.md`).
- **A speed claim of any kind.** Each engine's measurement answers its own rule's part, and nothing else.

## Falsified if

- A vector or a reference-arm difference that no row classifies.
- A requirement that cannot be met by the end of SJL-11.2: the area is then refused (decision D1), and this record
  is revised.
- This family's exhaustion verdicts moving outside its rule's class C2 population.
- The library's nesting maximum not completing on a declared thread under Native AOT.
- A reading of the specification's text in the correspondence that `regexpref` refutes, until the row is
  reclassified here.
