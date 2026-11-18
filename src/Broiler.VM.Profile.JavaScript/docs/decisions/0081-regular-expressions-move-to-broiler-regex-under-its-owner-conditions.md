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
  *(Note of 2026-11-01, decision D-s5: S5 leaves this family's list. `regexpref` refutes the reading it rested on,
  and this family's acceptance of `\k` in a class beside a named group is the text's answer. This family's
  corrections are row S1 (kind D) and rows S10, M4 (a non-empty match), M7 and T3 (kind R).)*
  *(Note of 2026-11-16, decision D-wiv: row S8a joins this family's list, kind R, so its corrections are row S1 (kind
  D) and rows S8a, S10, M4 (a non-empty match), M7 and T3 (kind R); Broiler.JS's gain rows S12 to S15, M8 and M9 by
  decision D-jsrows.)* *(Note of 2026-11-16, later, decision D-jsexec: Broiler.JS's gain rows M10 to M15 too, and row
  B1's correction of Broiler.JS gains P3's `O06` by its widening.)* *(Note of 2026-11-16, after SJL-15.1 merged,
  decision D-jsrest: Broiler.JS's gain rows M16 to M26 too, rows M16, M17, M18 and M19 corrections of
  Broiler.Regex's preview.4 as well.)* *(Note of 2026-11-18, decision D-jssticky: Broiler.JS's gain row M28 too, the
  `y` flag on its members that answer without `exec`, whose `IsMatch` and public `Replace` half SJL-11.5 corrects and
  whose `split` fallback half it does not.)*
- *(Added 2026-11-01.)* **The owner's decisions D-s5, D-p5, D-widen and D-partd of 2026-11-01 are recorded**, under
  the same direction, in section "The owner's decisions of 2026-11-01". SJL-11.3's first runs of the
  regular-expression vectors found them, on Broiler.JS's corpus revision 31. They are classified at revision 32.
  Row S5's class case is reread as `regexpref` reads it. Rows M4, S3b and B8 record Broiler.JS's answers through
  Broiler.Regex `0.1.0-preview.5`. Rows M2, S7, S8 and F3 are widened to the inputs the vectors reach. Broiler.JS's
  part D population leaves out the status vectors. No sub-area's verdict changes. Every vector difference is now
  classified by a row, and no clause of "Falsified if" stands.
- *(Added 2026-11-16.)* **The owner's decisions D-wiv and D-jsrows of 2026-11-16 are recorded**, under the same
  direction, in section "The owner's decisions of 2026-11-16". Broiler.Regex's reference arms found differences no
  row named. Row S8a, this family's `\w` in a class difference under `iv`, is a correction of this family, kind R, and
  bundle `sjl-11-001`'s rule lists it by a revision filed before any of its runs. Rows S12 to S15, M8 and M9 are
  corrections of Broiler.JS. No sub-area's verdict changes. The inputs the arbiter cannot reach stay class (c) of the
  arms under their rule, which is Broiler.Regex's, and are recorded, not decided. The section names the reading of
  "Falsified if" these rows answer. The arms' class (c) is still not empty. *(Note of 2026-11-16, later: "these rows
  answer" read row M8 as naming every `exec` answer of Broiler.JS that differs; by decision D-jsexec, in the bullet
  below, row M8 reads with its list, and the clause stands for the answers no row names.)*
- *(Added 2026-11-16, later.)* **The owner's decision D-jsexec of 2026-11-16 is recorded**, under the same direction,
  in section "The owner's decisions of 2026-11-16". Broiler.Regex's classifier `@5` read row M8 with its list and left
  in class (c) Broiler.JS's other `exec` answers of the arms, with P3's `O06`. Row M8 reads with its list. Rows M10 to
  M15 are corrections of Broiler.JS, one row to a mechanism, each taking an input only where the condition its text
  states holds. Row B1's correction of Broiler.JS is widened to `O06`. No sub-area's verdict changes. On the reading
  of "Falsified if" taken, its first clause stands for the `exec` answers of Broiler.JS that no row names, until a row
  names them: this record is not revised for them, and the owner's choice is open. A row naming those answers by what
  is left, with no mechanism, was not taken: it would class answers whose engine is not read as Broiler.JS's defects,
  and by reading most of them are answered by Broiler.Regex's preview.4, to which `GapScan` routes them. The arms'
  class (c) is still not empty. *(Note of 2026-11-16, after SJL-15.1 merged: "by reading most of them are answered by
  Broiler.Regex's preview.4, to which `GapScan` routes them" did not hold. Arm J's copy, run again outside this
  repository with a report of its route, hands only a few of those patterns to preview.4; most stay on the .NET path,
  several because preview.4's parser refuses or misreads a construct the text accepts, so `GapScan` never sees them.
  Decision D-jsrest, below, names them by mechanism.)*
- *(Added 2026-11-16, after SJL-15.1 merged.)* **The owner's decision D-jsrest of 2026-11-16 is recorded**, under the
  same direction, in section "The owner's decisions of 2026-11-16". Broiler.Regex's classifier `@7`
  (Broiler-Platform/Broiler.Regex#23) read rows M10 to M15 and left in class (c) the `exec` answers of Broiler.JS that
  no row names, with `O06`. Rows M16 to M26 are corrections of Broiler.JS, one row to a mechanism read in arm J's copy
  and, where the pattern reaches Broiler.Regex's preview.4 or its parser decides the route, in preview.4's source; rows
  M16, M17, M18 and M19 are corrections of preview.4 too. Rows M8, M12, M13 and M15 carry dated notes, M13's fixing its
  reading where the nested class is not the class's last element. No sub-area's verdict changes. On the reading of
  "Falsified if" taken, its first clause now stands only for the inputs of pattern `P4-47050`, whose mechanism is not
  read in full, until a row names them, and the owner's choice is open. `O06`'s reading is the reference-arm rule
  owner's. The arms' class (c) is still not empty. *(Note of 2026-11-16, after rows M16 to M26 merged: "its first
  clause now stands only for the inputs of pattern `P4-47050`, whose mechanism is not read in full, until a row names
  them, and the owner's choice is open" now reads: their mechanism is read, and row M27 names them, `P4-47050-s-05` in
  arm J's three cultures, under decision D-jsrest, below. On the reading of "Falsified if" taken, the clause stands for
  no difference the arms found once Broiler.Regex's classifier reads row M27, and until then for those inputs.
  Broiler.Regex's classifier `@8` (Broiler-Platform/Broiler.Regex#24) classes `O06` by row B1 under the reference-arm
  rule's revision of 2026-11-16c, the owner's ruling. Nothing is declared met.)*
- *(Added 2026-11-18.)* **The owner's decision D-jssticky of 2026-11-18 is recorded**, under the same direction, in
  section "The owner's decisions of 2026-11-18". Broiler.JS's SJL-11.5 branch changes how three of its members that
  answer without `exec` read the `y` flag, a change its records name as a finding for this record's correspondence.
  Row M28 holds it, in section M after row M27, a correction of Broiler.JS in two halves: `IsMatch` and the public
  `Replace`, which SJL-11.5 corrects, and `String.prototype.split`'s fallback when a RegExp's `@@split` is `undefined`
  or `null`, which it does not. Both of the row's engine cells are readings of the code. Row M5 is unchanged. No
  sub-area's verdict changes.
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
  *(Noted 2026-11-16, decisions D-wiv and D-jsrows: the rows also hold S8a, S12 to S15, M8 and M9.)*
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
  *(Note of 2026-11-01, decision D-s5: the clause "accepts `\k` in a class beside a named group (S5)" leaves this
  list of defects. That acceptance is the text's answer, as `regexpref` reads B.1.2, and so no defect of this
  family. The defects this paragraph lists are rows S1, S10, M4, M7 and T3, and S9b's, which is the front end's.)*
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
*(Noted 2026-11-01, by the owner's version decision of that day ("Please keep using version 0.1.0-x scheme!"),
recorded under decision D12 below: the package the candidate reading's commit is packed as is a later version of the
`0.1.0-preview.N` stream than the `0.1.0-preview.5` the owner published on 2026-10-29, `0.1.0-preview.6` or after, and
not `0.2.0-preview.1`. The sentence above stands as written.)*

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
- **Corrections of this family:** S1 (kind D); S5's class case and S10 (kind R). *(Note of 2026-11-01, decision
  D-s5: S1 (kind D) and S10 (kind R). S5's class case is the text's answer in this family, and a correction of
  Broiler.JS.)* *(Note of 2026-11-16, decision D-wiv: and S8a (kind R), `\w` as the first operand of a class
  difference under `iv`, which the reference arms reach and no probe reached.)*
- **Corrections of Broiler.JS:** S2a, S2b, S7, S8, S10's class case and S11; S3b's process abort, with the library.
  *(Note of 2026-11-01: and S5's class case, by decision D-s5. S7 and S8 are read with the inputs decision D-widen
  adds. S3b's 200,000 is accepted since Broiler.JS's `a58d31d1`, by decision D-p5, and so reads as engine policy
  under D7, as 20,000 does.)* *(Note of 2026-11-16, decision D-jsrows: and S12 to S15, the reference arms' groups no
  row named: patterns the text refuses that Broiler.JS accepts (S12, S13), patterns B.1.2 admits that it refuses
  without `u` or `v` (S14), and a quantifier bound past 2^31 − 1 it refuses under `i`, with or without `u` or `v`
  (S15). Every row is still classified.)*
- **Corrections of Broiler.Regex's preview.4**, where Broiler.JS's fallback masks them: S1's refusal of valid
  duplicates, S2, S2a, S4, S5's parse-only acceptance, S6 and S10's refusal.
- **Engine policy that becomes a parameter:** S3, the nesting limit, and S3b's deep but finite nesting, which
  becomes a listed refusal in Broiler.JS (decision D7).
- **Outside SJL-11:** S9, an invalid literal read as division, which is the shared front end's and SJL-14's
  (decision D22); and S9b, this family's scanner, which is SJL-12's and SJL-13's front end. S9b's test262 file fails
  at both commits of SJL-11.4 and is no C1 row.
  *(Noted 2026-11-10, at SJL-13's step 1's records, by
  [JSD-0087](0087-the-wide-path-parses-with-the-shared-syntax-library.md).)* "S9b's test262 file fails at both commits of
  SJL-11.4" no longer holds: step 1 corrects that file, `test/staging/sm/RegExp/7.8.5-01.js`, by bundle `sjl-13-001`'s
  row T33 (the library refuses `/a[bc\` with the unterminated literal's diagnostic where this family's tokenizer
  threw), so it is expected to pass at both commits of SJL-11.4, which descend from step 1. It is still no C1 row, and a
  change of its verdict between them is still a finding against this record.
  *(Noted 2026-11-12, at SJL-13's work package 9.4, when this family's vectors pin moved to Broiler.JS's corpus
  revision 39.)* The bullet reads "S9b, this family's scanner, which is SJL-12's and SJL-13's front end", and the note
  above says that step 1 corrects S9b's test262 file by bundle `sjl-13-001`'s row T33. The same correction reaches row
  S9b's vectors: corpus revision 39, merged in Broiler-Platform/Broiler.JS#1116 as `e4b6b49e`, retires their
  `divergence.broiler-vm` fields on that bundle's section 5's finding, and this family's vectors pin moved to it in
  `b781ebb`, where they are a plain match. Row S9b, which "What the correspondence found" lists as this family's
  scanner's defect, is no longer a difference of this family on the vectors; it was no C1 row, and is none now. This
  family's divergence fields of the regular-expression area that remain are those of rows S1, S10, M4, M7 and T3, its
  corrections, which SJL-11.4 makes. No decision is revised, and the text above is left as written.

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
- *(Noted 2026-11-16, decision D-jsrows.)* **Corrections of Broiler.JS** also: M8, `exec` answers that differ from the
  text's on inputs no other row's mechanism records, and M9, an `exec` its .NET engine throws on; and M4's half-pair
  matches hold its own vectors that the arms reached, by the row's widening.
- *(Noted 2026-11-16, later, decision D-jsexec.)* The note above's "M8, `exec` answers that differ from the text's on
  inputs no other row's mechanism records" reads with row M8's list: M8 holds the inputs its list names. Broiler.JS's
  other `exec` answers that differ from the text's are **corrections of Broiler.JS** by mechanism, each row taking an
  input where the condition its text states holds: M10, a group name its translator rewrites; M11, the empty
  iteration of a `?` that .NET keeps; M12, `\p{…}`, `\P{…}` and `\a` in a class without `u` or `v`; M13, a class
  subtraction; M14, `\-` starting no range; M15, `i` and `I` under `tr-TR` and `lt-LT`. The rest no row names yet,
  and on the reading of "Falsified if" taken the clause stands for them.
- *(Noted 2026-11-16, after SJL-15.1 merged, decision D-jsrest.)* The note above's "The rest no row names yet" now reads
  with rows M16 to M26, **corrections of Broiler.JS** by mechanism, each taking an input where the condition its text
  states holds: M16, `\c` with no control letter, which preview.4 reads as `\`; M17, a group name with `\u{…}`, which
  preview.4 refuses, so that .NET's loop answers; M18, `\k<…>` with `\u{…}`, which the translator writes as no
  back-reference; M19, a legacy octal escape preview.4 misreads, so that `GapScan` does not route a nullable
  quantifier; M20, a legacy octal escape before `\d` in a class; M21, `\S` in a class under `RegexOptions.ECMAScript`;
  M22, an empty group under a lazy quantifier with a minimum of 1; M23, `^` or `$` in a class under `m`; M24, a
  back-reference to a group with no capture without `RegexOptions.ECMAScript`; M25, a letter expanded inside a
  modifier group that removes `i`; M26, a class with `\W`, `\p` or `\P` under `i`. M16, M17, M18 and M19
  are **corrections of Broiler.Regex's preview.4** too. Rows M8, M12, M13 and M15 read further inputs of their own by
  dated notes. The inputs of pattern `P4-47050` no row names.
- *(Noted 2026-11-16, after rows M16 to M26 merged, decision D-jsrest.)* The note above's "The inputs of pattern
  `P4-47050` no row names" now reads with row M27, a **correction of Broiler.JS**: a loop over `\D`, or over a negated
  class of ASCII word characters, right before `\b` without `RegexOptions.ECMAScript`, which .NET makes atomic, and
  greedy where it was lazy. Row M8 keeps the inputs of that construct its list takes.
- *(Noted 2026-11-18, decision D-jssticky.)* **Corrections of Broiler.JS** also: M28, the `y` flag on the members that
  answer without `exec`, in two halves: `IsMatch` and the public `Replace` on a sticky pattern, which SJL-11.5
  corrects, and `String.prototype.split`'s fallback to `JSRegExp.Split` when a RegExp's `@@split` is `undefined` or
  `null`, which SJL-11.5 does not make. M5 stays **same in both**.

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
constructor; the literal's late refusals are row S11's. *(Note of 2026-11-01, decision D-widen: but `(?x:a)`, which
Broiler.JS's constructor accepts, passing `x` through as .NET's inline option. It is a correction of Broiler.JS that
SJL-11.5 answers.)*

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
  *(Noted 2026-10-26, at SJL-13.1, by [JSD-0083](0083-the-shared-syntax-library-and-the-wide-path.md) decision D7:
  "until SJL-13" is read as decision D18's note below reads it.)*

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
  *(Noted 2026-11-01: by the owner's version decision of that day, "Please keep using version 0.1.0-x scheme!",
  recorded under decision D12, the SJL-11 surface's binary breaks ship inside `0.1.0-preview.N`, and
  `0.1.0-preview.5` already carries R11's. SJL-D-3's compatibility minimum (ADR 0014: "Binary compatibility inside a
  major version is mandatory"), as this reading reads it, forbids that. This is the owner's own departure from the
  reading, and no record re-reads SJL-D-3; the reading above stands as written and does not hold of Broiler.Regex's
  `0.1` stream.)*
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

*(Revised 2026-11-01, by the owner's decision of that day, which is the owner's own and not one taken under the
direction of 2026-10-18. The owner wrote, of Broiler.Regex's version: "Please keep using version 0.1.0-x scheme!" **The
SJL-11 surface ships in the `0.1.0-preview.N` stream**, not as `0.2.0-preview.1`. The decision read, in the sentence
this replaces: "The SJL-11 surface ships as **`0.2.0-preview.1`**".
- **The stream already carries part of the surface** (observed 2026-11-01). The owner published Broiler.Regex
  `0.1.0-preview.5` to nuget.org on 2026-10-29; its nuspec names Broiler.Regex
  `24344b3c88b37afc3ace725a3b1e01af697b3dc6`, which descends from the merges of Broiler-Platform/Broiler.Regex#8 to
  #14 (SJL-11.2's tasks 2.2 to 2.7, and 2.11). So `0.1.0-preview.5` already ships R11's accepted breaks of the `0.1`
  surface, which `Broiler.Regex/CompatibilitySuppressions.xml` at that commit lists against `0.1.0-preview.4`: among
  them the removal of `RegexOverflowException`, `CaseFolding`, `RegexMatch` and `RegexGroup`, and of the table-less
  constructor `BroilerRegex(string, RegexFlags)`. The version that carries the whole surface is a later one of the
  stream, `0.1.0-preview.6` or after.
- **Broiler.JS depends on it** (observed). Broiler.JS's `Directory.Packages.props` has named `0.1.0-preview.5` since
  the owner's `a58d31d1` ("Update dependencies", 2026-10-29), on its default branch (`66973a29`) and at its tag
  `v0.1.0-preview.9`. Broiler.VM references no Broiler.Regex package.
- **The breaks' validation.** Each break stays suppressed and listed with its R item. The next pack is validated
  against the last published version, `0.1.0-preview.5`, as SJL-D-3's compatibility minimum (ADR 0014) requires; the
  decision's "validated against `0.1.0-preview.4`" no longer names it. Broiler.Regex's default branch (`cfb541e`,
  observed) still sets `PackageValidationBaselineVersion` to `0.1.0-preview.4`, and its CHANGELOG's "Unreleased"
  still names `0.2.0-preview.1`: a conflict with this record that stays open until Broiler.Regex moves them.
- **SJL-D-3's reading.** The breaks ship inside the `0.1` stream, which SJL-D-3's compatibility minimum, as this
  decision read it, forbids; "The design's readings" carries the note.
- **The bold sentence and the pin no longer describe the state.** The decision reads "**No release of either engine
  depends on `0.2` before both cut-overs merge**, and Broiler.JS moves its pin only at SJL-11.5." Broiler.JS's pin
  moved on 2026-10-29, before SJL-11.5, to a version that carries R11's breaks. Both sentences stand as written and do
  not hold. **The hazard this decision exists to prevent is open**: inside one `0.1` stream no version says where the
  surface broke, so minimum ranges can let a process that hosts both engines resolve a Broiler.Regex that one of them
  was not built against. It cannot arise while Broiler.VM references no
  Broiler.Regex package; it arises when SJL-11.4 admits one, and a later record of the owner's, or SJL-11.4's
  admission, settles it.)*

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
the earlier previews were. *(Noted 2026-11-01, by the owner's version decision of that day ("Please keep using
version 0.1.0-x scheme!"), recorded under decision D12: "`0.2.0-preview.1`" reads the version of the
`0.1.0-preview.N` stream that carries the whole SJL-11 surface, `0.1.0-preview.6` or after; `0.1.0-preview.5`, which
the owner published on 2026-10-29, already carries part of it, as decision D12's note says.)* This family may admit a locally packed candidate on a branch for development, and re-pins
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

*(Noted 2026-10-26, at SJL-13.1, by [JSD-0083](0083-the-shared-syntax-library-and-the-wide-path.md) decision D7.)*
The seam SJL-13 builds is a validator interface in the shared frontend's request, with **no package edge** from the
frontend to Broiler.Regex. From SJL-13's step 1 the binder's `CheckRegExp` leaves for that provider, which this
family's compiler implements over the validator `CheckRegExp` calls, Broiler.Regex's validate entry once SJL-11.4 has
landed, keeping this family's code and the literal's span. So "The Compiler references Broiler.Regex from SJL-11.4
**until SJL-13**" and X5's "from SJL-11.4 until SJL-13" are read as: until the frontend's own edge to Broiler.Regex,
which is decided at SJL-15.1, and only if SJL-11.4 and SJL-11.5 have both landed; meanwhile the compiler's reference
stays, for the provider. If SJL-13's step 1 lands before SJL-11.4, SJL-11.4 re-points the provider's implementation
instead of `CheckRegExp`, and SJL-11's rule here is revised by a new dated file before any of its runs (JSD-0083
decision D36).

*(Noted 2026-11-10, at SJL-13's step 1's records, by
[JSD-0087](0087-the-wide-path-parses-with-the-shared-syntax-library.md) and JSD-0083 decision D36.)* The note above
ends "If SJL-13's step 1 lands before SJL-11.4, SJL-11.4 re-points the provider's implementation instead of
`CheckRegExp`, and SJL-11's rule here is revised by a new dated file before any of its runs". Step 1 lands first: its
branch deletes `CheckRegExp`, and the provider is `JsRegularExpressionValidator`, which makes the checks `CheckRegExp`
made, in its order and with its texts, over this family's matcher; the library reports its refusal at the literal's
span under registry code 2215, `InvalidRegularExpression`, which revision 19 minted for the meaning `CheckRegExp`
reported under 2101. So the check this decision has SJL-11.4 re-point onto Broiler.Regex's validate entry and the
shared flag parser is `JsRegularExpressionValidator`, keeping that code and that span. Bundle `sjl-11-001`'s rule is
revised by
[`decision-rule-area-revised-2026-11-10.md`](../../../../docs/evidence/sjl-11-001/decision-rule-area-revised-2026-11-10.md),
filed with step 1's records before any of its runs. SJL-11.4's landing branch is not cut, and the windows do not
overlap.

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
  - *(added 2026-11-01, decisions D-s5 and D-widen)* `\k` in a class beside a named group, refused: S5's class
    case; and the invalid modifier `(?x:a)` accepted by the constructor: F3. Rows M2, S7 and S8 are read with the
    inputs D-widen adds: the Ohm sign and Kelvin by `(?i:)`; the quantified `^`, `$` and `\b`; and five more `v`-mode
    classes. No test262 file at `ccaac100` tries any of these, so none names a part A row;
  - a class that joins a lead escape to a braced trail escape: S10's class case;
  - with Broiler.Regex's preview.4: the budget no-match, the shortened match, the wrong index and the look-around
    flip (B1 to B3); the stack-guard fallback, to .NET and to no match (B5, two rows); the process aborts of a flat
    sequence and of deep nesting (B8, S3b); the look-behind back-reference inside a pair (M7);
  - `String.prototype.search`'s direct call, with an instance's own `@@search` undefined or null and the
    `ReferenceError` on a pattern only Broiler.Regex holds: M6.
  - *(added 2026-11-16, decision D-jsrows)* the reference arms' groups no row named: patterns the text refuses and
    Broiler.JS accepts (S12, S13), patterns B.1.2 admits and it refuses (S14), a bound past 2^31 − 1 it refuses under
    `i` (S15), `exec` answers that differ (M8) and an `exec` that throws (M9), and row M4's own vectors that the arms
    reached, by that row's widening. No test262 file at `ccaac100`
    is known to move for any of them, so none names a part A row;
  - *(added 2026-11-16, later, decision D-jsexec)* the arms' `exec` answers outside row M8's list, by mechanism: a
    rewritten group name (M10), a `?`'s empty iteration (M11), `\p{…}`, `\P{…}` and `\a` in a non-`u` class (M12), a
    class subtraction (M13), `\-` starting no range (M14) and `i` and `I` under `tr-TR` and `lt-LT` (M15); and
    `/^[a]*$/u` over a run of `a` one longer than the library's step budget, by row B1's widening. No test262 file at
    `ccaac100` is known to move for any of them, so none names a part A row;
  - *(added 2026-11-16, after SJL-15.1 merged, decision D-jsrest)* the arms' `exec` answers no row named, by mechanism:
    `\c` with no control letter on preview.4's path (M16), a group name and a `\k<…>` name with `\u{…}` (M17, M18), a
    legacy octal escape (M19, M20), `\S` in a class (M21), an empty group under a lazy quantifier (M22), `^` or `$` in a
    class under `m` (M23), a back-reference to a group with no capture (M24), a letter expanded inside a modifier group
    that removes `i` (M25) and a class with `\W`, `\p` or `\P` under `i` (M26); and the inputs rows M8, M12, M13 and M15
    read by their notes. No test262 file at `ccaac100` is known to move for any of them, so none names a part A row;
  - *(added 2026-11-16, after rows M16 to M26 merged, decision D-jsrest)* a loop over `\D`, or over a negated class of
    ASCII word characters, right before `\b` without `RegexOptions.ECMAScript`, which .NET makes atomic (M27). No
    test262 file at `ccaac100` is known to move for it, so it names no part A row;
  - *(added 2026-11-18, decision D-jssticky)* the `y` flag on `IJSRegExp.IsMatch` and the public `JSRegExp.Replace`,
    which answer without `exec` and honoured the flag only for a pattern routed to Broiler.Regex (M28, its first
    half). Its second half, `String.prototype.split`'s fallback to `JSRegExp.Split` when a RegExp's `@@split` is
    `undefined` or `null`, is a correction of Broiler.JS that SJL-11.5 does not make, and is not on this list. No
    test262 verdict is read for either half here;
  - **Removed from the plan's list:** `\0` before a non-ASCII digit, which the correspondence shows is
    Broiler.Regex's alone and not observable in Broiler.JS (row S4); it is a library fix (decision D36). An invalid
    literal read as division stays SJL-14's (row S9).
- **This family's (SJL-11.4).** *(Note of 2026-11-01, decision D-s5: S5's `\k` in a class beside a named group
  leaves this list. It is the text's answer, and the kind R rows are S10, M4, M7 and T3.)* The milestone lists
  duplicate named groups only (row S1, kind D). The correspondence
  adds, each kind R: S5's `\k` in a class beside a named group, S10's lone lead escape before a braced escape, M4's
  `"index"` and substring for a non-empty match, M7's back-reference across a surrogate pair, and T3's raw line
  terminator in `source`. Its rule's part A lists each with its row, and the test262 files that fail at its base
  (rows S10, M7 and T3) are the ones a C1 (R) row must name.
  *(Note of 2026-11-16, decision D-wiv: row S8a, `\w` as the first operand of a class difference under `iv`, joins
  this list, kind R, and the rule's revision of 2026-11-16 lists it in part A; no test262 file at `ccaac100` names
  it.)*

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
*(Note of 2026-11-01, decision D-partd: the part's population is the vectors of `regexp/` an engine-level run gives
a line to. That leaves out the 18 `status: exhausted` vectors, which Broiler.Regex's part D judges at library level.)*

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

*(Note of 2026-11-01, decision D-p5.)* Broiler.JS's answer recorded above, `"index"` 1 and `lastIndex` 1, is
Broiler.JS's at `1f91b8f1`, with Broiler.Regex `0.1.0-preview.4` on its `u` path. Since `a58d31d1` (2026-10-29)
Broiler.JS hands every `u` and `v` pattern to `0.1.0-preview.5`. Its run on corpus revision 31 answers `"index"` 0
and `lastIndex` 0, this family's answer. This decision records that answer as Broiler.JS's. The vector's
`per-engine.broiler-js` field holds it from revision 32, and its `out` still lists both answers this decision has
recorded. The class, implementation-defined by this decision, and its lapse do not change.

### The owner's decisions of 2026-11-01

*(Added 2026-11-01.)* SJL-11.3's first engine-level runs of the regular-expression vectors stopped on items this
record did not hold as the runs showed them. The vectors are Broiler.JS's corpus revision 31, which `regexpref` states.
Broiler.JS's first run was taken at its `d8cb8406`, whose engine is `66973a29`'s, under the invariant culture,
`tr-TR` and `lt-LT`. This family's was taken at `89760389`, on branch `sjl-11-3-vm`, from a scratch archive of
revision 31. Each item was decided on 2026-11-01 as recommended, under the owner's direction of 2026-10-18 quoted
above: "Please do all recommended decisions and skip waiting for review." Each is recorded here as the owner's
decision of 2026-11-01. Broiler.JS's corpus revision 32 classifies both runs by them. The decision numbers D1 to D41
are all assigned, so these four carry names: **D-s5**, **D-p5**, **D-widen** and **D-partd**. None changes a sub-area's
verdict or a rule.

**Decision D-s5 - row S5's class case is the text's answer in this family.** The correspondence's row S5 read
"`IdentityEscape[+NamedCaptureGroups]` excludes `k`, in a class too (B.1.2)". On that reading this family's acceptance
of `[\k<a>](?<a>x)` was a correction of kind R, "on this reading; `regexpref` checks it". `regexpref` refutes the
reading:
- **No production passes [NamedCaptureGroups] into a CharacterClass.** B.1.2's ExtendedAtom has
  `CharacterClass[~UnicodeMode, ~UnicodeSetsMode]`, and ClassContents, NonemptyClassRanges and ClassAtom pass only
  ?UnicodeMode on. So B.1.2's ClassEscape reads ~NamedCaptureGroups, and `SourceCharacterIdentityEscape`, "SourceCharacter
  but not c", admits `k`. Without `u` and `v`, `[\k<a>](?<a>x)` and `[\k](?<a>x)` are accepted.
- **This family's acceptance is the text's answer**, on all three paths. S5's class case is therefore no correction
  of this family:
  - this record's lists drop it (the dated notes above);
  - the kind R corrections are S10, M4 (a non-empty match), M7 and T3.
- **Broiler.JS's refusal is a correction of Broiler.JS.** Its `RegExpGroupNames` refuses a `\k` in a class whenever
  the pattern holds a group name, "invalid named reference". SJL-11.5 deletes that precheck (decision D20), and
  decision D23's list gains the row.
- **The other halves stand.** S5's parse-only acceptance of `\k<b>(?<a>x)` stays a correction of Broiler.Regex's
  preview.4 (decision D36).
- **No test262 file at `ccaac100` puts `\k` in a class beside a named group**, so no part A row of either engine
  moves for it.
- **Bundle `sjl-11-001`'s area rule still names S5 as a C1 (R) correction** of this family, in part A and in part B.
  That rule is unchanged, as its "Revisions" requires. Read with this decision, a move of the class case from
  acceptance to refusal at SJL-11.4 is away from the text. The bundle's README records the disagreement for the
  owner, and a new dated rule file before any base run is what can remove it.
- The reason: decision D25 has `regexpref` check the correspondence's readings, and a reading it refutes is
  reclassified by a revision of this record.

**Decision D-p5 - Broiler.JS's `u` and `v` path is Broiler.Regex `0.1.0-preview.5` since `a58d31d1`.** The
correspondence read Broiler.JS at `1f91b8f1`, whose `u` and `v` patterns went to `0.1.0-preview.4`. On 2026-10-29
`a58d31d1` ("Update dependencies") moved Broiler.JS to `0.1.0-preview.5`, built from Broiler.Regex's `24344b3`, which
holds SJL-11.2's tasks 2.1 to 2.7 and 2.11. It also re-pointed the routing's library calls. The runs show three
cells moved:
- **Row M4's empty match.** Broiler.JS answers `"index"` 0 and `lastIndex` 0, this family's answer. Decision D41's
  dated note records it, and the vector's `per-engine.broiler-js` field holds it from revision 32.
- **Row S3b's 200,000.** 200,000 nested `(?:` under `u` is accepted, where the process aborted. It reads as engine
  policy under D7, as 20,000 does.
- **Row B8's flat sequence.** It completes with the whole input in about 4 s evaluated alone on the runs' host, where
  the process aborted. The four first runs, taken beside each other, each rendered the runner's 5 s abort, and the
  field classifies that abort. What it classifies is the runner's time limit, not the row's class: R4, no CLR
  recursion in the matcher, preview.5 already meets, since the process no longer aborts. The time, which grows about
  as the square of the length, is not attributed to the library or to the adapter, and nothing yet shows that
  SJL-11.5 removes it. Its classification stays open for the owner. The vector so breaks the premise of SCHEMA.md
  section 7 of Broiler.JS's corpus, that a runner's limit is far above the time any vector takes, and a later corpus
  revision may restore it by a smaller input.
- **The classes do not change.** Each row records the new answer beside the one it holds.
- **The runs no longer show some preview.4 answers on the same inputs:** M4's half-pair matches, M7's look-behind and
  S10's class case. Their corrections are what JSD-0081 lists, and for Broiler.JS the package already made them. The
  correspondence's "rows by class" says so.
- **This record, both engines' rules and Broiler.JS's bundle still name `0.1.0-preview.4` as Broiler.JS's restored
  package.** The disagreement is recorded for the owner and changes no rule.
- The reason: each difference is classified by the row that holds it (decision D25), and a row's recorded answer
  that the engine no longer gives cannot hold it.

**Decision D-widen - rows M2, S7, S8 and F3 hold the inputs the vectors reach beyond their probes.** The vectors reach
inputs the rows' probes did not run. Broiler.JS answers each by the mechanism its row names, and each row is widened
by a dated note:
- **M2:** the Ohm sign U+2126 against `[\u03C9]` and `[\u03A9]` under `i` without `u`, and Kelvin against `k`
  through `(?i:\u212a)` without `u`: .NET's casing. Row M2c, same in both, is narrowed to the rest.
- **S7:** every quantified look-behind the vectors hold, and a quantified `^`, `$` and `\b` without `u`. The
  library's refusal is caught, and .NET accepts.
- **S8:** `[&&a]`, `[--a]`, `[a-]`, `[-a]` and `[\q{(}]` under `v`, by the same path.
- **F3:** the invalid modifier `(?x:a)`, which the constructor passes through as .NET's inline option. F3 gains a
  correction of Broiler.JS for it.

The classes of M2, S7 and S8 do not change, and decision D23's list reads them with these inputs. No test262 file at
`ccaac100` tries any of them. This family answers every one as the specification does. The reason: as decision D-asi
of JSD-0083 had it, a row read from probes that never reached an input cannot be stretched to hold it. But the
mechanism a row names can be recorded as reaching it, and the runs show it does.

**Decision D-partd - Broiler.JS's part D population is the regular-expression vectors an engine-level run gives a
line to.** Broiler.JS's bundle `sjl-11-001` rule's part D takes "every vector of conformance/vectors/regexp/" and holds
each to the outcome its class expects. Its 18 `status: exhausted` vectors are library-level (decision D25), and the
corpus's SCHEMA.md section 4 has an engine-level runner skip them. They are judged at library level only, by
Broiler.Regex's part D. Broiler.JS's engine-level part D population is the non-status vectors. The decision is a dated
note on Broiler.JS's filed rule, not a rewrite, and decision D33 carries a note of it. The reason: an engine-level run
can never give a status's outcome. The plan's recommendation, taken under the direction, keeps the rule's three-culture
identity and its class expectations over every vector that run can give.

### The owner's decisions of 2026-11-16

*(Added 2026-11-16.)* Broiler.Regex's task 2.13 ran the reference arms decision D24 requires, and its classification
(Broiler.Regex's roadmap, the record of task 2.13 and its dated notes of 2026-11-02, by its classifier `@3`, and the
record of 2026-11-15) left differences in class (c) of three kinds: a defect of this family no row of the correspondence
named; defects of Broiler.JS no row named; and inputs the arbiter, `regexpref`, cannot reach in its step cut-off. "Falsified
if" below reads "A vector or a reference-arm difference that no row classifies", so the first two kinds falsified this
record from that classification, and the roadmap asked for this record to be revised before either cut-over. The two
decisions below were taken on 2026-11-16 as recommended, under the owner's direction of 2026-10-18 quoted above, "Please
do all recommended decisions and skip waiting for review.", and are recorded here as the owner's decisions of that day,
each with the alternative not taken. Like the decisions of 2026-11-01 they carry names. Neither changes a sub-area's
verdict. *(Note of 2026-11-16, later: "The two decisions below" are D-wiv and D-jsrows; decision D-jsexec, after the
reading of "Falsified if" below, joins them, taken the same way, and it too changes no sub-area's verdict.)*
*(Note of 2026-11-16, after SJL-15.1 merged: decision D-jsrest, after D-jsexec's reading of "Falsified if", joins them
too, taken the same way, and changes no sub-area's verdict.)*

**Decision D-wiv - row S8a, `\w` in a class difference under `iv`, is a correction of this family, kind R.** Every
difference of arm V that no row named is `\w` as the first operand of a class difference under `iv`, in the arms'
population P4's patterns `[\w--I]`, `[\w--\ud83d--\S]?`, `[\w--[^]--K]\B|😀` and `𝐀+?[[\w--[\x6b\D\s]]\r]*`. Arm V is
a byte-for-byte copy of this family's `JsRegExpMatcher.cs`, so the answer is the matcher's.
- **What the matcher does.** Under `v`, its `ParseClassSetOperand` builds `\d`, `\s` and `\w` from their ranges and
  returns them unfolded, where every other operand passes through its `MaybeFold`, and a complement takes the folded
  universe. Under `iu` and `iv`, `\w`'s ranges hold `A` to `Z`, U+017F and U+212A, which fold to other code points.
  `--` subtracts the ranges as they stand. So an operand that holds `I`'s or `K`'s folding, or a complement over the
  folded universe (`[^]`, `\S`, `\D`), leaves those code points in the class, and the case-insensitive match admits
  their partners: `[\w--I]` matches `i` and `I`.
- **What the text answers.** CompileToCharSet's `CharacterClassEscape :: w` (22.2.2.9) returns
  "MaybeSimpleCaseFolding(_rer_, WordCharacters(_rer_))", so under `iv` the operand is `[0-9_a-z]` and the difference
  removes `i` from `[\w--I]`. S, Broiler.Regex's backend, and the arbiter answer so, and so does arm J.
- **The row.** The correspondence's row S8a records the inputs, the lines and the text's answer, and row S8's
  Broiler.VM cell is narrowed by a dated note. It is a correction of this family, kind R, which SJL-11.4 makes when the
  matcher is deleted. No test262 file at `ccaac100` puts a class difference under `i` and `v`, no vector of corpus
  revision 39 reaches it, and it moves no literal's refusal.
- **The rule.** Bundle `sjl-11-001`'s part A names its kind R corrections by a fixed list, so a variant row S8a explains
  would be a finding and a regression. Its revision of 2026-11-16,
  [`decision-rule-area-revised-2026-11-16.md`](../../../../docs/evidence/sjl-11-001/decision-rule-area-revised-2026-11-16.md),
  filed before any run of that rule, adds S8a to the list and changes nothing else. It neither admits the owner's
  workflow change of 2026-10-30 nor drops row S5: both stay the owner's.
- **Not taken:** no revision of the rule, so that row S8a would be a correction in the correspondence and here but a
  finding in part A, where a variant that moved because of it after SJL-11.4 would be a regression.
- The reason: decision D25 has `regexpref` check this record's readings, and the rule's arbiter shows this family's
  answer wrong, so the row is added as a correction rather than read as a policy.

**Decision D-jsrows - Broiler.JS's differences no row named are rows S12 to S15, M8 and M9, corrections of Broiler.JS.**
The differences of arm J that no row named are grouped by Broiler.JS's answer and, where the text refuses, by the
arbiter's reason (PatternSyntaxError's code), one row to a group:
- **S12**: Broiler.JS accepts a pattern the grammar refuses (the arbiter's `grammar:` reasons: a quantified assertion or
  a lone quantifier, an invalid escape or class escape under `u` or `v`, a lone syntax character, a trailing backslash,
  an unterminated class, a range as an operand of `&&`, an escaped modifier). The vectors among them that rows S7 and
  S8 already name are those rows', which the arms' classifier's predicates do not reach; the arms' other inputs of
  those rows' constructs are S12's, one group to the arbiter's reason, and widening S7 and S8 instead by dated notes
  is the alternative not taken;
- **S13**: Broiler.JS accepts a pattern an early error refuses (the arbiter's `early:` reasons: a decimal escape past
  the group count, a class range out of order, an undefined group name, a braced quantifier after an assertion, a
  negated class with strings);
- **S14**: without `u` or `v`, Broiler.JS refuses a pattern B.1.2 admits, split by what triggers the refusal: with no
  flag as well as under the pattern's flags, by the constructs the row lists (`\p{…}`, `\P{…}` and `\u{…}` read with
  their `u`-mode or .NET meaning, class ranges ending in a class escape, `\c`, `\q`, `\k` and `\8` in a class, octal
  escapes, a lone brace); only under `s`, where Broiler.JS's `ParseFlags` drops .NET's ECMAScript option, a reading;
  and only under `i` or only under `m`, not traced. The groups are also named by the arbiter's reason for the same
  pattern with `u` added, which names what B.1.2 admits, not what Broiler.JS refuses under one flag;
- **S15**: under `i`, with or without `u` or `v`, Broiler.JS refuses a quantifier bound past 2^31 − 1 in a pattern it
  does not refuse without `i`, which the text accepts;
- **M8**: an `exec` both answer, where Broiler.JS's answer differs (.NET's word set for `\b` and `\B`, case pairs no row
  records, an empty `\q{}`); *(note of 2026-11-16, later, decision D-jsexec: "an `exec` both answer, where Broiler.JS's
  answer differs" reads with the row's list, the parenthesis here; Broiler.JS's other `exec` answers are rows M10 to
  M15's where their conditions hold, and the rest no row's)*
- **M4**, widened by a dated note: its own vectors from a `lastIndex` inside a pair that the arms reached, on answers
  its cell did not record;
- **M9**: an `exec` on which Broiler.JS's .NET engine throws.

Each row names its inputs, examples and Broiler.JS's mechanism as read in arm J's copy of Broiler.JS's code at
`1f91b8f1`, marked a reading where no input was traced, and this family's answer, which on every one of these inputs is
the arbiter's but where rows S1 and M4 name this family's corrections.
- **Where the rows are.** In this record's correspondence, as decision D27 keeps it with Broiler.JS's column in every
  row, and because Broiler.JS's rule classifies by it ("The correspondence this rule classifies by is Broiler.VM's
  `docs/evidence/sjl-11-001/correspondence.md`"). Decision D23's list of Broiler.JS's corrections gains the six rows,
  and row M4's half-pair matches gain its own vectors that the arms reached.
- **Broiler.JS's test262 verdicts.** Broiler.JS's rule's part A names its C1 corrections by a fixed list of rows. No
  test262 file that Broiler.JS's bundle `sjl-10-001` after run fails, the run that rule names its files from, is
  explained by any of the six rows, so none is known to move a Broiler.JS part A row. A dated Broiler.JS rule file
  before SJL-11.5's base would add them to that list should one move; that is Broiler.JS's to file, and this record
  files none.
- **Two inputs the reduction reached.** Broiler.Regex's reduction of 2026-11-15 reduced two inputs the arbiter could not
  reach, `P4-38461-s-03` and `-s-05`, to prefixes it reaches, where S and the arbiter answer `null` and Broiler.JS
  refuses the pattern, which B.1.2 admits. It refuses it only under `s`: without `u` or `v` it accepts the same pattern
  under every other flag, so the refusal is not its reading of the quantified look-ahead or of `\q` in a class but
  follows the `s` flag, under which `ParseFlags` drops .NET's ECMAScript option. They are row S14's, of its inputs
  refused only under `s`.
- **Not taken:** keeping the rows out of this record and drafting them for Broiler.JS's own records; Broiler.JS's rule
  reads this file, so rows elsewhere would classify nothing.
- The reason: decision D25, as for D-wiv; and a row that names a mechanism by group, rather than one row to an input,
  is what the classifier's predicates read (the direction of a difference, its construct and the arbiter's reason).

**What neither decision changes.** The arms' own classification is Broiler.Regex's: its classifier reads each row by a
predicate, and the differences these rows hold leave class (c) there only when that classifier reads the new rows, a
change of that repository's before its judged run. Both decisions change no rule of Broiler.Regex's and no rule of
Broiler.JS's.

**The inputs the arbiter cannot reach, recorded and not decided.** The rule of the reference arms, Broiler.Regex's bundle
`sjl-11-001`'s `reference-arms.md`, whose owner is the Broiler.RegEx maintainer, reads "a difference the arbiter cannot
reach in its step cut-off is class (c) until reduced to an input it can". Broiler.Regex's reduction of 2026-11-15, by
its `reduce.py`, cut each such input's subject to the longest prefix the arbiter reaches and ran the arms over the
prefixes. It found three sets, which its roadmap's record of that day counts:
- **inputs no prefix reaches**, not even the empty one: patterns whose quantifier has a minimum of 4,294,967,296 over a
  body that can match the empty string, so the arbiter's cost is in the pattern. Under the rule as written they stay
  class (c);
- **inputs whose difference does not survive the reduction**: at the longest prefix the arbiter reaches, S and the arm
  answer alike, and S's answer is the arbiter's. The difference exists only at sizes the arbiter cannot reach. The
  rule's "until reduced to an input it can" is met for the input, which was cut to a prefix the arbiter reaches, but
  not for the difference, which no reduced input shows; so they are no class (a), (a') or (b) difference, and they
  stay class (c) by the rule's "every other difference is class (c)". That reading is open for the rule's owner;
- **inputs that reduce to another difference**, `P4-38461-s-03` and `-s-05` above: reduced to an input the arbiter
  reaches whose difference row S14 names. Under the rule as written they leave class (c) for class (a) under row S14
  when the arms' classification reads the reduction and that row; until then they are (c).

Whether the second set may leave class (c), and whether a stop of S at its own declared ceiling against an answer the
arbiter cannot check may be classed other than (c), are readings of that rule, and the rule is Broiler.Regex's. Neither
is decided here, and both are open for that rule's owner, by a dated rule file of that repository.

**The reading of "Falsified if" these decisions answer.** Its first clause reads "A vector or a reference-arm difference
that no row classifies". **Taken:** a reference-arm difference the arbiter decides is classified when a row of the
correspondence names its input and classes the engine the arbiter shows wrong, as rows S8a, S12 to S15, M8 and M9 now
do for every such difference the classification of 2026-11-15 left in class (c), with rows S7, S8 and M4 for the
vectors they name, M4 by its widening; an input the arbiter cannot reach is
no difference the arbiter decides, so it does not falsify this record, though it keeps the arms' class (c) non-empty
under their rule. **Not taken:** reading "classifies" as the arms' classification listing a difference outside class
(c), under which the clause stands until Broiler.Regex's classifier reads the new rows, and stands for the unreachable
inputs until that rule's owner decides them. On the reading taken, no clause of "Falsified if" stands for the
differences the arbiter decides; the arms' class (c) is still not empty.

*(Added 2026-11-16, later.)* Broiler.Regex's classifier `@5` (Broiler-Platform/Broiler.Regex#21) read the rows above,
row M8 to its list (its roadmap's record: "Row M8 read with its list (above). Not taken: its first sentence alone."),
and left in class (c) the `exec` answers of arm J that differ from the text's on inputs that list does not name
("Whether row M8 holds them is Broiler.VM's to say, by a dated widening of the row or a row of its own"), with one input
of P3, `O06`, whose difference the arbiter cannot reach. The decision below was taken on 2026-11-16 as recommended,
under the same direction, and is recorded as the owner's decision of that day, with the alternative not taken.

**Decision D-jsexec - Broiler.JS's `exec` answers outside row M8's list are rows M10 to M15 where their mechanism is
read, row M8 reads with its list, and `O06` is row B1's.** The differences are grouped by Broiler.JS's mechanism, read in arm J's copy of
Broiler.JS at `1f91b8f1`, one row to a mechanism, not by the shape of the answer:
- **M10**: the translator rewrites the pattern's text before it renames the groups, and its rewrites of `$` (under `m`,
  and beside an inline modifier group) and of a character it case-expands (under `i`, one whose Canonicalize class holds
  a non-ASCII member, or one .NET folds into another Canonicalize class) reach into a group's name, so
  `groups` reports another name for the same capture;
- **M11**: a `?` over a group whose body matches the empty string reaches .NET, which keeps that iteration and its
  captures, where RepeatMatcher discards it; the translator's rewrite of zero-width bodies and `GapScan`'s routing of
  quantifiers whose maximum is above 1 leave it there;
- **M12**: without `u` or `v`, `\p{…}`, `\P{…}` and `\a` in a class reach .NET as written, which reads a
  General_Category and U+0007 where B.1.2 reads the letters;
- **M13**: without `u` or `v`, `-[…]` in a class reaches .NET, which reads a class subtraction;
- **M14**: without `u` or `v`, `\-` in a class before `-` reaches .NET, which reads it as starting no range;
- **M15**: under `i` without `u` or `v`, `i` and `I` under `tr-TR` and `lt-LT`, row M2a's culture casing on the ASCII
  pair, where the invariant culture answers as the text does;
- **B1**, widened: `/^[a]*$/u` over a run of `a` one longer than the library's step budget, which Broiler.JS answers
  `null`, by the budget's early exit the row's cell reads for `/[a]*/u` and `/^a*$/u` (an inference from those answers,
  not traced in preview.4); this family answers it as the text does, so the row's half for this family gains nothing;
  the arbiter cannot reach it, so
  the arms' rule keeps it in class (c) whatever row names it, as it keeps the other inputs above whose difference the
  reduction does not show.

Each row's text states when it takes an input. Rows M11 to M14 take one only where the arbiter's answer to its
pattern, with the row's construct written as .NET reads it, is Broiler.JS's, up to row M10's names, and not the
text's; each was checked that way, outside this repository. An input that holds a row's construct where that reading
does not give Broiler.JS's answer is not the row's, as some under `i`, whose reading leaves out .NET's
case-insensitive matching of the class. Row M10 was checked by its names: on each input it takes the captures are the
text's, and each name the text gives and Broiler.JS does not is the text's name rewritten as the row says, at the same
capture. Row M15 was checked by the culture: arm J under the invariant culture answers each input it takes as the
text does. Neither asks the arbiter for a reading. The rows are read in order, M10 first, so an input two of them
take is the first's; in the arms' run, as read outside this repository, no input is taken by two.
Row M8 carries a dated note: its opening reads with its list, and its "`\w`, `\d` and `\s` beside them" reads
"beside `\b` or `\B` beside a non-ASCII letter", adding no input, as Broiler.Regex's classifier reads it; its `tr-TR`
inputs of P5 are row M15's mechanism and stay its own. Like D-jsrows, no test262 file that Broiler.JS's bundle
`sjl-10-001` after run fails is explained by these rows, so none is known to move a Broiler.JS part A row, and this
record files no rule; none moves a verdict of this family's.
- **Not taken:** reading row M8's opening as naming every `exec` answer of Broiler.JS that differs from the text's, the
  scope its decision D-jsrows wrote it with, so that no new row is needed. A classifier's predicate cannot read that
  scope without guessing, which Broiler.Regex's review of its classifier `@4` found, and it names no mechanism.
- The reason: decision D25, as for D-jsrows; and a row is what a predicate reads, so each row names the construct that
  moves the answer.
- **Not taken:** a row of its own for `O06`. Its mechanism is row B1's, the budget's early exit on a run, and the row's
  `/[a]*/u` and `/^a*$/u` already hold its two halves, so a dated widening of that row names it.
- **Not taken:** a row that names, by what is left, the `exec` answers no other row takes, with no mechanism, so that
  the first clause of "Falsified if" would not stand for them. It would class answers whose engine is not read as
  Broiler.JS's defects, and by reading most of them are answered by Broiler.Regex's preview.4, to which `GapScan` routes
  them; and a predicate for it would turn on every other row and on the classifier's order rather than on what the row
  says. *(Note of 2026-11-16, after SJL-15.1 merged, decision D-jsrest: "by reading most of them are answered by
  Broiler.Regex's preview.4, to which `GapScan` routes them" did not hold, as the status's note says; the reasons
  against a row by what is left stand.)*
- **Not taken:** readings for rows M12 and M13 that also model .NET's case-insensitive matching of a class, so that
  they take the inputs under `i` where it moves the answer with their construct. That matching is not read in arm J's
  copy or in .NET's source, so those inputs stay named by no row.
- **What it leaves.** The `exec` answers no row takes stay in class (c), named by no row, of two kinds. Most hold no
  construct of rows M11 to M14: by reading, most of these are patterns `GapScan` routes to Broiler.Regex's preview.4
  without `u` (a quantifier whose maximum is above 1 over a body that matches both empty and non-empty, or a capture
  inside a look-behind), and the others .NET readings no row records, such as a letter's case expansion inside a
  `(?-i:…)` group. The rest hold a construct of row M11, M12 or M13 where the row's reading does not give Broiler.JS's
  answer, most of them under `i`, where .NET's case-insensitive matching of the class, which no reading models, moves
  it too. They are open, below. *(Note of 2026-11-16, after SJL-15.1 merged, decision D-jsrest: "by reading, most of
  these are patterns `GapScan` routes to Broiler.Regex's preview.4 without `u`" did not hold; read again in arm J's
  copy, run with a report of its route, most of them stay on .NET, and decision D-jsrest names them, with the inputs
  under `i`, by mechanism.)*

**The reading of "Falsified if", as decision D-jsexec changes it.** *(Note of 2026-11-16, later: the paragraph above
read the clause as answered because "rows S8a, S12 to S15, M8 and M9 now do" name "every such difference the
classification of 2026-11-15 left in class (c)"; that held on row M8's opening read as naming every `exec` answer that
differs, the reading D-jsexec does not take.)* **Taken:** the same reading of "classifies", a row naming the input and
classing the engine the arbiter shows wrong, now with row M8 read to its list. Rows M10 to M15 name the `exec` answers
whose mechanism is read, where their conditions hold. The rest are differences the arbiter decides that no row names,
so on this reading the clause stands for them until a row names them; this record is not revised for them here, and
the owner's choice is open. **Not taken:** row M8's wider reading, under which the clause would not stand for them.
The inputs the arbiter cannot reach, `O06` among them, still do not falsify this record, as above.

*(Added 2026-11-16, after SJL-15.1 merged.)* Broiler.Regex's classifier `@7` (Broiler-Platform/Broiler.Regex#23, merged
as `23deeb63`) read rows M10 to M15 and left in class (c) the `exec` answers of arm J that no row names, with `O06`.
They were read again by mechanism in arm J's copy of Broiler.JS at `1f91b8f1`, and in Broiler.Regex `0.1.0-preview.4`'s
source at `c1422f4d` where the pattern reaches preview.4 or its parser decides the route. The decision below was taken
on 2026-11-16 as recommended, under the same direction, and is recorded as the owner's decision of that day, with the
alternatives not taken.

**Decision D-jsrest - Broiler.JS's `exec` answers no row named are rows M16 to M26 by mechanism, and rows M8, M12, M13
and M15 read further inputs by dated notes.** One row to a mechanism, each a correction of Broiler.JS:
- **M16**, `\c` that no ASCII letter follows, in a pattern `GapScan` hands to preview.4, whose parser reads it as `\`
  alone and drops the `c` (B.1.2's ExtendedAtom and ClassAtomNoDash read `\` and then `c`); a correction of preview.4
  too;
- **M17**, a group name written with `\u{…}`, which the text accepts and preview.4's parser refuses, so a pattern
  `GapScan` would route for a nullable quantifier stays on .NET, which ends the loop at an iteration that matched the
  empty string (RepeatMatcher discards it); a correction of preview.4 too;
- **M18**, `\k<…>` whose name is written with `\u{…}`, which the translator writes as `\k<u{…}>`, naming no group,
  and which preview.4's parser refuses as it refuses row M17's name, so a pattern `GapScan` would route stays on .NET;
  a correction of preview.4 too *(2026-11-16, after SJL-15.1 merged, at this decision's review; until then without
  "and which preview.4's parser refuses ..." and its class, a correction of Broiler.JS only)*;
- **M19**, a legacy octal escape `\0` and a digit, which preview.4's parser reads as `0` and the digit, so `GapScan`
  does not find a nullable quantifier around it and .NET keeps the empty iteration; a correction of preview.4 too;
- **M20**, the same escape right before `\d` in a class, which the translator writes as `0-9`, so .NET reads the
  octal escape with the first digit;
- **M21**, `\S` in a class, which the translator leaves to .NET, which reads it under `RegexOptions.ECMAScript` as the
  complement of ASCII white space;
- **M22**, a group with an empty body under `+?` or `{1,}?` in a pattern with more than one alternative, where .NET
  reports a match whose index or end is not the attempt's (inferred from the answers);
- **M23**, `^` or `$` as a member of a class under `m`, which the translator's anchor rewrite writes as its text;
- **M24**, a back-reference to a group that has not participated, which .NET fails without `RegexOptions.ECMAScript`
  (under `s`, or with a modifier group), where BackreferenceMatcher matches the empty string;
- **M25**, a letter the translator expands to its Canonicalize class, inside a modifier group that removes `i`;
- **M26**, a class with `\W`, `\p{…}` or `\P{…}` under `i`, which .NET matches case-insensitively by its own case
  pairs and lower-casing (inferred from the answers and from .NET runs).

Each row's mechanism was checked outside this repository by running arm J's copy again on the pattern with the row's
construct written another way the text reads the same, or by running .NET on the translated pattern with the construct
changed: each then answers as the text does on every input the row takes, with two qualifications the correspondence
states: row M18's rewrite lets `GapScan` route the pattern, so preview.4 answers it, and the translator's half rests on
.NET runs; and row M26's inputs that hold `-[` have a joint cause, row M13's subtraction with .NET's case pairs, which
row M13's rewrite, not row M26's check, shows. *(2026-11-16, after SJL-15.1 merged, at this decision's review; until
then without these qualifications.)* Rows M16, M21, M23 and M25 take an input
only where the arbiter's answer to the row's reading is Broiler.JS's, up to row M10's names, and not the text's; the
others by their construct and condition. They are read after row M15, in order. Each preview.4 row notes that
Broiler.JS routes to Broiler.Regex `0.1.0-preview.6` since Broiler-Platform/Broiler.JS#1119, whose parser reads those
constructs as the text does, by reading of its source. The dated notes: row M8's `\b` and `\B` read outside a class
lexed as the text lexes it, a `[` inside a class being a character, so that its list reads inputs Broiler.Regex's
classifier `@7` does not take; row M12's reading of a `-` before `\p{…}` or `\P{…}`, as the translator writes it; row
M13's reading, undefined where `-[` is not the class's last element, which .NET refuses, so that the row takes no input
of it, and its nested class lexed as .NET lexes it; row M15's invariant answer read up to row M10's names. Like
D-jsexec, no test262 file that Broiler.JS's bundle `sjl-10-001` after run fails is explained by these rows, so none is
known to move a Broiler.JS part A row, and this record files no rule; none moves a verdict of this family's.
- **Not taken:** new rows for the inputs rows M8, M12, M13 and M15 already name or would name by a reading of their
  own text. Their mechanisms are those rows', and a second row for one mechanism would make the classifier's order
  decide between them.
- **Not taken:** one row for every answer that reaches .NET because preview.4's parser refuses or misreads the pattern,
  whatever .NET then does. The answer is .NET's, and its mechanism differs from pattern to pattern; rows M17, M18 and
  M19 name both halves of each.
- **Not taken:** a row for pattern `P4-47050`, whose mechanism is not read in full, `[\u017f-\w]|\ud83d*((?<=s*
  )|\D+?|[\W&&\s&&[-]])?\b|\f{1(?!)` under `s`. It holds row M11's construct, whose reading does not give Broiler.JS's
  answer, and no non-ASCII letter for row M8; writing its `\b` another way moves .NET's answer, which does not show why.
  It stays in class (c). *(Note of 2026-11-16, after SJL-15.1 merged, at this decision's review: "writing its `\b`
  another way moves .NET's answer, which does not show why" reads: written with its `\b` as look-arounds over the
  ASCII word characters, which the text reads the same, the pattern gives the text's answer in each culture, so `\b`
  under `s`, which .NET reads by its own word set, is a necessary part of the cause; how .NET's `\b` differs on the
  input is still not read.)*
- **Not taken:** readings for rows M22, M24 and M26 that the arbiter answers. .NET's lazy loop, its back-reference
  without `RegexOptions.ECMAScript` and its case pairs have no ECMAScript text that reads as they do; those rows take an
  input by their construct and condition, after every other row.
- The reason: decision D25, as for D-jsrows and D-jsexec; and a row is what a predicate reads, so each row names the
  construct that moves the answer.

**The reading of "Falsified if", as decision D-jsrest changes it.** *(Note of 2026-11-16, after SJL-15.1 merged: the
paragraph above, which reads "The rest are differences the arbiter decides that no row names, so on this reading the
clause stands for them until a row names them", is read with decision D-jsrest as follows.)* **Taken:** the same reading
of "classifies". Rows M16 to M26 and the notes on rows M8, M12, M13 and M15 name the `exec` answers whose mechanism is
read, where their conditions hold. The clause now stands only for the inputs of pattern `P4-47050` above, until a row
names them; this record is not revised for them here, and the owner's choice is open. **Not taken:** a row naming them
by what is left, as under D-jsexec. `O06` and the other inputs the arbiter cannot reach still do not falsify this
record, as above. How a row-named input the arbiter cannot reach is classed is the reference-arm rule's,
Broiler.RegEx's, and this record does not decide it.

*(Note of 2026-11-16, after rows M16 to M26 merged.)* Decision D-jsrest's **Not taken** "a row for pattern `P4-47050`,
whose mechanism is not read in full, ... It stays in class (c)", with that item's note, "how .NET's `\b` differs on the
input is still not read", and the paragraph above's "The clause now stands only for the inputs of pattern `P4-47050`
above, until a row names them; this record is not revised for them here, and the owner's choice is open" now read as
follows. The mechanism is read, and this decision's own rule, one row to a mechanism, gives row M27; no new decision is
taken.
- **Row M27**, a correction of Broiler.JS: without `u` or `v`, under `s` or with a modifier group, a quantifier with a
  minimum of 1 or more and no maximum, lazy or not, over `\D`, or over a negated class of ASCII word characters, right
  before `\b`. Without `RegexOptions.ECMAScript` .NET parses `\b` as `RegexNodeKind.Boundary`. The translator's `\D` is
  stored by .NET as the negated range `[^0-9]`, and .NET's test of whether a loop's set lies inside `\w`
  (`RegexCharClass.IsKnownWordClassSubset`) enumerates its ranges without reading the negation. So `RegexNode` makes the
  loop atomic, and a lazy one greedy, and `\b` is tried only where the loop ends, where the text tries it after each
  iteration (RepeatMatcher, 22.2.2.3.1; IsWordChar, 22.2.2.4.1). Read in arm J's copy and in .NET's source at
  dotnet/runtime `v10.0.15`, the runtime the copy was run on, and checked outside this repository: arm J's copy, run
  with `\b` written `(?<=\b)`, and with `\D+?` written `(?:(?!\d)[^])+?`, both of which the text reads the same, gives
  the text's answer in every culture.
- **How it takes an input:** like rows M16, M21, M23 and M25, only where the arbiter's answer to its reading, each such
  loop written greedy and followed by `(?!X)`, X its atom, is Broiler.JS's, up to row M10's names, and not the text's.
  It is read after M26, so row M8 keeps the inputs of the same construct its list takes; on most of them `(?<=\b)`
  gives the text's answer, so this mechanism is their cause, or part of it.
- **On the reading of "Falsified if" taken:** once Broiler.Regex's classifier reads row M27, no difference the arms
  found is named by no row. `O06` is row B1's under the reference-arm rule's revision of 2026-11-16c
  (Broiler-Platform/Broiler.Regex#24), and the P4 patterns the owner excluded are judged in no class. Until the
  classifier reads row M27, the clause stands for these inputs. Nothing is declared met, and no sub-area's verdict
  changes.
- **Not taken:** a dated note on row M8. Its mechanism, .NET's word set beside a non-ASCII letter, is not this one:
  the input holds no non-ASCII letter, and `(?<=\b)`, which keeps .NET's word set, gives the text's answer.
- **Not taken:** a row by its construct alone, read after every other row, as rows M22, M24 and M26 are. `\D+(?!\D)` is
  ECMAScript text that reads as .NET runs the loop, so the arbiter can answer the reading.

### The owner's decisions of 2026-11-18

*(Added 2026-11-18.)* Broiler.JS's branch `sjl-11-5`, its SJL-11.5 cut-over, not merged, changes by its `4e2e00ad` three
of its members that answer without `RegExpBuiltinExec`: `IJSRegExp.IsMatch`, which its `assert` module's `assert.match`
calls, the public `JSRegExp.Replace`, and the public `JSRegExp.Split`, which `String.prototype.split` calls when a
RegExp's `@@split` is `undefined` or `null`. Its bundle `sjl-11-001`'s README, at the branch's head `f690836d`, records
the change as "a finding for Broiler.VM's correspondence, whose row is Broiler.VM's to add", and its
`Broiler.JS/CHANGELOG.md` lists it as "A change no row names". No row of the correspondence named it: row M5, the
sticky row, reads Broiler.JS's `y` through `exec` and the protocols, and is "Same in both". The decision below was taken
on 2026-11-18 as recommended, under the owner's direction of 2026-10-18 quoted above, "Please do all recommended
decisions and skip waiting for review.", and is recorded here as the owner's decision of that day, with the
alternatives not taken. It follows the pattern of decisions D-wiv and D-jsrows in section "The owner's decisions of
2026-11-16" above, each a row the correspondence lacked, recorded with its place, its class and what it changes. It is
not one of the owner's own answers of that day, and it changes no sub-area's verdict.

**Decision D-jssticky - the `y` flag on Broiler.JS's members that answer without `exec` is row M28, one row of section M
in two halves, a correction of Broiler.JS, filed now from a reading of both engines' code.**
- **Where and what.** Row M28, in section M after row M27, class **correction of Broiler.JS**, one row with two halves:
  - `IsMatch` and the public `Replace`. At the branch's first parent they honoured `y` only for a pattern routed to
    Broiler.Regex; SJL-11.5's `4e2e00ad` makes them answer as `exec` (RegExpBuiltinExec, 22.2.7.2) and `@@replace`
    (22.2.6.11) do, a sticky pattern tried at its start alone. This family has no such member: it answers a match only
    through `RegExpExec`, row M5's;
  - the `split` fallback. With a RegExp's `@@split` `undefined` or `null`, `String.prototype.split` (22.1.3.23)
    converts the separator with ToString and splits on that string, as this family does; Broiler.JS hands the RegExp
    to `JSRegExp.Split`, which splits by the pattern, with or without `y`. SJL-11.5 does not make this correction:
    Broiler.JS's `CHANGELOG.md` leaves the fallback "for a later change".
- **The cells are readings.** Both engine cells are read from code, Broiler.JS's at `f690836d` and this family's at
  `aaa39fb`, whose `JsRealm.RegExp.cs` is the same at `32b0fad0`, and are marked *(reading)* as the correspondence's
  "How each column was run" defines it; no cell was run. A probe of the `split` inputs through the correspondence's
  `run-vm.sh` and `run-js.sh` may be added by a later dated note on the row; until then its class rests on the
  reading, and a run that answers otherwise reclassifies it by a dated note.
- **What it changes.** Decision D23's list of Broiler.JS's corrections gains the row's first half, and the sub-area M
  section a note. This family's list is unchanged, and bundle `sjl-11-001`'s rule, whose part A lists this family's
  corrections, is not revised. Broiler.JS's rule names its part A corrections by a fixed list of rows; whether and
  how it lists row M28 is Broiler.JS's to file, and this record files nothing in Broiler.JS.
- **Not taken:** widening row M5 by a dated note to these members. Its class, "Same in both", would no longer hold for
  the `split` fallback, whose answers differ between the engines.
- **Not taken:** a row of section F, flags. The flag is `y`, but what differs is which members read it and how `split`
  falls back when no `@@split` applies; section M holds matching and the protocols' fallbacks, with row M5, and row
  M6, `search`'s.
- **Not taken:** two rows, one for `IsMatch` and `Replace` and one for the `split` fallback. Both halves are one change
  of Broiler.JS's, the one finding its records name, and both are corrections of Broiler.JS; one row with halves names
  it once, as row M4 holds halves of its own.
- **Not taken:** holding the row until its `split` inputs are run in both engines. Filed now from the reading, the row
  stands in the correspondence before Broiler.JS's rule can be revised to name it and before either cut-over's base is
  taken; a later run is added by a dated note.
- The reason: decision D27 keeps Broiler.JS's column in every row, and Broiler.JS's rule classifies by this
  correspondence ("The correspondence this rule classifies by is Broiler.VM's `docs/evidence/sjl-11-001/correspondence.md`"),
  so a change of Broiler.JS's that no row names is explained by no row there; and the text decides each half, so the
  row is a correction, not a policy.

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
  *(Note of 2026-11-16: Broiler.Regex's reference arms found differences of this family and of Broiler.JS that no
  row classified, which falsified this record from their classification until, on the reading of "classifies" taken
  in section "The owner's decisions of 2026-11-16", rows S8a, S12 to S15, M8 and M9, and row M4 by its widening,
  classified them, by decisions D-wiv and D-jsrows. That section also gives the reading of the inputs the arbiter
  cannot reach, and the reading not taken, under which the clause still stands.)* *(Note of 2026-11-16, later: the
  note above read "rows S8a, S12 to S15, M8 and M9, and row M4 by its widening, classified them, by decisions D-wiv
  and D-jsrows", which held on row M8 read as naming every `exec` answer of Broiler.JS that differs. By
  decision D-jsexec row M8 reads with its list, rows M10 to M15 name Broiler.JS's other `exec` answers whose mechanism
  is read, each where its condition holds, and row B1's widening names `O06`. The `exec` answers no row takes are
  named by no row, so on the reading taken the clause stands for them until a row names them; that section's note
  says so, and the owner's choice is open.)* *(Note of 2026-11-16, after SJL-15.1 merged: the note above's "The `exec`
  answers no row takes are named by no row" now reads with decision D-jsrest: rows M16 to M26 and the notes on rows M8,
  M12, M13 and M15 name them, but for the inputs of pattern `P4-47050`, whose mechanism is not read in full, for which
  the clause stands on the reading taken until a row names them, and the owner's choice is open.)* *(Note of 2026-11-16,
  after rows M16 to M26 merged: the note above's "but for the inputs of pattern `P4-47050`, whose mechanism is not read
  in full, for which the clause stands on the reading taken until a row names them, and the owner's choice is open" now
  reads: row M27 names them, `P4-47050-s-05` in arm J's three cultures, by decision D-jsrest, so on the reading taken
  no difference the arms found remains named by no row once Broiler.Regex's classifier reads row M27; until it does,
  the clause stands for them. Nothing is declared met.)*
- A requirement that cannot be met by the end of SJL-11.2: the area is then refused (decision D1), and this record
  is revised.
- This family's exhaustion verdicts moving outside its rule's class C2 population.
- The library's nesting maximum not completing on a declared thread under Native AOT.
- A reading of the specification's text in the correspondence that `regexpref` refutes, until the row is
  reclassified here. *(Note of 2026-11-01: `regexpref` refuted row S5's reading of `\k` in a class, and decision
  D-s5 reclassifies the row. The runs of revision 31 found no other refuted reading, and every vector difference is
  under a row, by decisions D-s5, D-p5 and D-widen.)*
