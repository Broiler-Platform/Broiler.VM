<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0084 - Collation moves to the shared Intl library: SJL-17's area verdict, Broiler.JS's part an adoption, JSD-0043 §5 to §7 amended where they move, and the owner's decisions of 2026-11-01

**Status:** Decided 2026-11-01.
- **The area is accepted**: the Unicode Collation Algorithm over CLDR's root collation and the tailorings of each
  embedding's list, with the four options `Intl.Collator` resolves (`sensitivity`, `ignorePunctuation`, `numeric`,
  `caseFirst`), moves to `Broiler.ECMAScript.Intl` as `UcaCollator`, over the collation tables the data home's generator
  writes (JSD-0082's decision D11).
- **It is an adoption, not an extraction** (X1, decision E2). Broiler.JS implements no collation algorithm: it hands the
  strings to the host's `CompareInfo`. The shared collator is this family's `JsCollator` and `JsCollationData`, and
  Broiler.JS adopts it for the locales the shared data covers.
- **This family has no correction in this area.** No row of the correspondence is in its column "Correction of
  Broiler.VM"; row O3 is a reading the vectors check (finding F1). **Broiler.JS's corrections are listed by row**: O5a,
  O5b, O7, O8, O10, O12, O14 and O18.
- **The owner's decisions E1 to E14 of 2026-11-01 are recorded**, each taken as SJL-17's plan recommends it, under the
  owner's direction of 2026-10-18. None was decided again: where the correspondence found a citation drifted or a
  reading the plan's text leaves open, the decision says so beside it, and its text stands.
- **JSD-0043 §5 to §7 are amended where they move**, each by a dated note in JSD-0043 beside the text it reads, which
  it quotes.
- Nothing in this profile changes with this record. The cut-overs are SJL-17.4's and SJL-17.5's own changes, and this
  record is what they apply.
- *(Added 2026-11-02.)* **The owner's decisions E-han, E-o5b and E-o3j of 2026-11-02 are recorded**, in section "The
  owner's decisions of 2026-11-02", each taken as recommended under the same direction. SJL-17.2's first runs of the
  collation vectors stopped on them. None changes a row's class or a rule, and row O3's reading stays open.

**Date:** 2026-11-01.

**Owner:** the JavaScript profile owner.

**Co-signers:**
- the Broiler.JS maintainer, as the owner of the library and of the data home, and of the other engine;
- the core architecture owner, for rule I1's allow-list and the composition register.

**All three roles are held by one person** (EX-30), and this record does not claim that anyone independent has read
it.

**Milestone:** the programme's SJL-17, work package SJL-17.1.
- The same verdict is recorded in Broiler.JS's shared-libraries plan, `docs/roadmap/SharedLibraries.md`, section "Area
  verdicts", "Collation (SJL-17.1)", with this record's decisions that bind Broiler.JS, by these numbers. It is filed in
  Broiler.JS's own change, observed on its branch until it merges; the ledger names its merge commit when it has.
- **SJL-17 started before SJL-9, which it waits on, met its gate**, at the owner's words of 2026-10-19, which the
  programme ledger's SJL-17.0 entry records with this record. SJL-17 waits on SJL-9 for the provider interface, the
  locale core and the data home, and through SJL-9 on SJL-7 for normalization. It may not meet its gate before SJL-9
  meets SJL-9's, and so not before SJL-7, SJL-8 and SJL-3 meet theirs. This record moves no gate.

**The owner's words of 2026-10-19.** The owner wrote: "Also continue with the other remaining SJL milestones". The
programme reads it as the choice to start SJL-9, SJL-11 and SJL-17 before the milestones they wait on meet their gates
(JSD-0081, JSD-0082). **Each milestone files its own start entry** (JSD-0081's decision D2): SJL-17's is the ledger's
SJL-17.0 entry, filed with this record, once SJL-9.3a merged the library's provider and locale core in Broiler.JS
(`b22e7d6a`, Broiler-Platform/Broiler.JS#1083, observed), as JSD-0082 and the SJL-9.0 entry foresaw.

**The owner's direction.** On 2026-10-18 the owner wrote: "Please do all recommended decisions and skip waiting for
review." This record reads it in two parts, as JSD-0078 and JSD-0080 to JSD-0083 did:
- each decision SJL-17's plan leaves open, E1 to E14, is taken as the plan recommends it, and recorded below as the
  owner's decision, dated 2026-11-01, the day it is filed. The correspondence's findings F1 to F7 are each answered the
  same day: by a reading recorded beside the decision it bears on, by a rule's text, or by a dated note;
- the programme proceeds through SJL-17's work packages without pausing for the owner between them.

**The direction changes no rule of the programme.**
- `Accepted` still needs review (the ledger's update rule 7), so nothing of SJL-17 is accepted by it.
- SJL-17 still may not meet its gate before SJL-9 meets its own.
- Rules still come before runs. Bundle `sjl-17-001`'s rules are committed in each repository before any code they
  judge.
- It covers decisions, not publication. The stream version that carries the collator is published only with the
  owner's approval, asked for when it is ready, and SJL-17.4's re-pin waits on that publication. By the owner's
  direction of 2026-11-01, "Please keep using version 0.1.0-x scheme!", that version is a `0.1.0-preview.N` of the
  shared stream.

**Names.**
- **Decisions.** "Decision En" is always this record's. Another record's decision is named with its record, as in
  "JSD-0082's decision D7".
- **Rows.** The correspondence's rows are lettered O (orderings), O1 to O19, with O5 and O11 split from `a`.
- **Findings.** F1 to F7 are the correspondence's, in its section "What the rows find against the plan and the
  records".
- **Steps.** A step of SJL-17 is named by its work package, SJL-17.1 to SJL-17.6. SJL-17's plan is held in no
  repository, so its text appears here only where a decision quotes it.
- **Classes.** C1 to C5 are the classes of the programme's comparison templates
  ([`validation.md`](../../../../docs/javascript-shared-libraries/validation.md) section 4).
- **Paths.** `VM/` is `src/Broiler.VM.Profile.JavaScript/`, as in the correspondence, and a line number is the line at
  `83290f9`.

---

## Context

The area's design is the programme's
[`areas/locale-data-and-intl.md`](../../../../docs/javascript-shared-libraries/areas/locale-data-and-intl.md), section
3.4. Collation left SJL-9 by decision SJL-D-24 (c), which JSD-0082 recorded; SJL-9's generator moved whole, its
collation tables included, so the data home already writes this family's collation tables byte for byte, under rules
N27 and N28 since SJL-9.2c.

**The correspondence is from code, and every behavioural cell was run.** It reads this family at **`83290f9`**,
whose collation code is SJL-9's rule commit's; Broiler.JS at **`61cb15b5`**, the default branch after SJL-9.3b, with
the library's provider and locale core; and test262 **`ccaac100`**. Each column was run so:
- **this family** through the conformance root, each probe as a raw test262 test, once in the bytecode form and once
  in the native form. No row differs by form;
- **Broiler.JS** through its shell, every probe again under invariant globalization, and the rows that read the
  default locale again under a German and a Turkish host culture;
- **Node v26.12.0** (ICU 78.3, CLDR 48.0), the reference Broiler.JS's bundle `sjl-9-001` names, as a witness, never
  the arbiter of a class.

[`correspondence.md`](../../../../docs/evidence/sjl-17-001/correspondence.md), which bundle `sjl-17-001` retains with
every probe and each engine's answers, names every member by file and line and every row with its class.

**What the correspondence found:**
- **Every row is classified**, in the seven classes bundle `sjl-9-001` used, and no row's class is unknown.
- **This family orders both of UCA's archived conformance files in full**, and the witness does; the host's ICU 74
  that Broiler.JS reads does not, and under invariant globalization Broiler.JS falls back to an ordinal comparison.
- **Broiler.JS resolves `numeric` and `caseFirst` and applies neither** (O7, O8), reads the options in another order
  and `caseFirst` twice (O10), ignores symbols under `ignorePunctuation` (O5a), makes a new `compare` on every get
  (O12), and compares by code unit under invariant globalization (O14). Each is a correction of Broiler.JS.
- **Of the `orderings` reference dataset**, Broiler.JS differs only on the lines that ask for `numeric` or `caseFirst`,
  and this family answers every line as the retained answer.
- **Broiler.JS passes test262 files that need collations the plan's covered data does not carry** (F2), and under
  invariant globalization the available-locale policy resolves every covered locale but `en` to `en-US` (F6).
- **One reading is open**: Han across blocks (O3, F1).

## The gate

**X1 - met, as an adoption** (decision E2).
- **This family**: UCA over CLDR 48.2.0's root collation (`allkeys_CLDR.txt` with UCA 17's implicit weights) and the
  tailorings of its list, German `phonebk` and `search` and root `search`, with the four options (`VM/JsCollator.cs`,
  `VM/JsCollationData.cs`; JSD-0043 §5).
- **Broiler.JS**: no algorithm. The host's `CompareInfo` over every host locale, after Broiler.Unicode's NFD, with
  `sensitivity` and `ignorePunctuation` mapped onto `CompareOptions` and `numeric` and `caseFirst` resolved but not
  applied (`Broiler.JS/Broiler.JavaScript.BuiltIns/Intl/JSIntl.cs:6204-6251`).

**X2 - met.**
- **Same:** O4, O6, O9, O13, and O19, where Broiler.JS reaches this family's answer by its tie-break, which input
  reaches through the host's Thai collation and which decision E11 deletes.
- **Corrections of Broiler.JS:** O5a, O5b, O7, O8, O10, O12, O14, O18.
- **Corrections of this family:** none.
- **Implementation-defined:** O11a, the `co` values of the search locale data, which the shared collator takes from
  this family.
- **Data policy (C5):** O1, O2, O3, O5c, O11b, O16: CLDR 48.2.0's root and the tailorings of each embedding's list
  replace the host's ICU for the covered locales.
- **Engine policy, a parameter:** O15, the default-locale provider; O17, the charges (decision E7).
- **Code no input reaches:** none.

**X3 - neutral contract.** `UcaCollator` takes the resolved options as a record (strength, the case level, the case
order, the shifted alternate, numeric collation, the tailoring by name), never an options object; the strings; and the
collation tables through `IIntlDataSet`, the identifiers `CollationRoot` and `CollationTailorings` that JSD-0082's
decision D8 kept. It answers -1, 0 or 1. Normalization is Broiler.Unicode's (decision E6), and the meter is the
caller's (decision E7). **No culture** enters it (JSD-0082's decision D19).

**X4 - no edge between the engines, none into Broiler.VM.** `UcaCollator` lives in `Broiler.ECMAScript.Intl`, which
references `Broiler.ECMAScript.Algorithms` and `Broiler.Unicode.Algorithms` (JSD-0082's decision D9), neither engine,
and nothing in this repository.

**X5 - boundaries hold.**
- **No platform globalization API enters the library.** Broiler.JS's host provider keeps `CompareInfo` in Broiler.JS,
  behind the provider, for the locales and collations the shared data does not cover (decision E3).
- **Optional data stays optional.** This family's collation tables stay in its optional `.Intl` assembly, byte for
  byte under rule N28, and rule I3 holds that.
- **No host data is needed.** Under invariant globalization nothing collates by code unit (decision E4).

**X6 - the deletion lists** (decision E11, with finding F4's lines).
- **This family:** `JsCollator`, `JsCollationData`, `NewCollator`'s tailoring core (`VM/JsRealm.Intl.cs:202-215`) and
  `CollatorKeyData` (`:235-242`).
- **Broiler.JS:** `ResolveCompareInfo` for the covered locales, `StripIgnorablePunctuation`'s tie-break, the
  `CompareOptions` mapping and the hard-coded Thai default; `NetCollationSuffix` moves into the host provider.
  `JSIntlCollator` keeps every public member (SJL-D-25 (a)).

**X7 - owners accept.** The JavaScript profile owner, the Broiler.JS maintainer and the core architecture owner accept
the area and record decisions E1 to E14 under the owner's direction of 2026-10-18. One person holds the roles (EX-30).

## The owner's decisions of 2026-11-01

Each decision below was taken on 2026-11-01 by the owner, under the direction quoted above, as SJL-17's plan recommends
it, with its reason. Each takes the plan's text, with its task numbers given as the steps they name, and with no
figure (validation principle 5). Where the correspondence bears on a decision, a reading follows it, marked as such;
no decision's text is replaced. The decisions that bind Broiler.JS, in its engine, its data home or its rule, are E2
to E14. Broiler.JS's area entry records them by these numbers.

**Decision E1 - when SJL-17 starts.** **With SJL-17.1**, once SJL-9.3a has merged. One ledger entry quotes the owner's
words of 2026-10-19, which cover it; SJL-9.0's entry says so. The row stays `Not started` until then. The reason: update
rule 4 would otherwise put up an `In progress` row with nothing in it, and SJL-17's rule needs the provider's shape.
SJL-9.3a merged as Broiler.JS `b22e7d6a` (observed), and the ledger's SJL-17.0 entry is filed with this record.

**Decision E2 - X1.** **Adoption.** Broiler.JS delegates to `CompareInfo` and implements no UCA. It moves anyway, and
the record says so. The reason: the milestone asks the verdict to say which.

**Decision E3 - which locales Broiler.JS's `UcaCollator` covers.** **Coverage per component**, with its own list: root,
and the tailorings CLDR 48.2.0 gives for `de` (phonebook, search), root `search`, `en`, `es`, `sv` and `th`. These are
the locales test262 `Collator` and the `orderings` dataset exercise. **CJK stays on the host provider** until a data
decision sizes its tailorings. This family's list is unchanged (rule N28). SJL-17.5's "the locales the shared data
covers" and exit clause 3 are read against this list (a dated note on the milestone). The reason: the CJK tailorings
are unsized, and Thai's default `ignorePunctuation` then comes from data.

*Reading (findings F2 and F6).* "Per component" is read per locale and collation: a collation the shared data carries
for a covered locale is the shared collator's, and a collation it does not carry stays the host provider's, as an
uncovered locale does. Broiler.JS passes, with the host's ICU, test262 files that ask `de` for `eor` and `en` for at
least one collation (JSD-0082's decision D33 (c)); the data home's rule engine refuses the syntax CLDR's `eor` and
`emoji` rules use, so SJL-17.3 either carries them or leaves them to the host provider, and Broiler.JS's rule names the
getCollations file. Under invariant globalization the available-locale policy (JSD-0082's decision D7) resolves every
request but `en` and `en-US` to `en-US`, so no tailoring applies there; part C of Broiler.JS's rule reads its answers
for the locale each line resolves (risk K5).

**Decision E4 - invariant globalization.** Uncovered and unavailable locales resolve as the available-locale policy
says (JSD-0082's decision D7) and **collate by UCA root or `en`, never by code unit**. It is a listed correction. The
reason: today `'a'.localeCompare('B')` answers a code-unit difference under invariant globalization (row O14).

**Decision E5 - Thai.** Thai's `[alternate shifted]` comes from the tailoring data; the hard-coded default is deleted.
The reason: it is data, not engine code.

**Decision E6 - normalization.** NFD through `Broiler.Unicode.Algorithms`, with the meter. The reason: SJL-17.3's
design, and the programme's `architecture.md`, which puts normalization with the Unicode owner.

**Decision E7 - metering.** `UcaCollator` takes `WorkCharge`; this family passes `JsCollator`'s charge in its units;
Broiler.JS passes none. No class C2. The reason: as JSD-0082's decision D18. Row O17 names the points: each element of
both strings, and NFD as `normalize` charges.

**Decision E8 - the bound `compare`.** Cached; a Broiler.JS correction at SJL-17.5 (row O12). The reason: the area
design's section 1, "bound format and compare functions and their caching", which the engines keep.

**Decision E9 - vectors.** Root order across scripts (from the CollationTest files and the CLDR 48 reference), every
tailoring of decision E3, the four options, lone surrogates; the CollationTest files as the library's L4 oracle. The
reason: SJL-17.2's work package. *Reading (finding F1):* the vectors of root order carry Han across blocks, where this
family's implicit weights and ICU's order part (row O3).

**Decision E10 - the rules' parts.** This family: A (test262 in both forms), B (the `orderings` dataset and the
collation checks, with every other Intl check as a guard) and C (the fuel-parity twins). Broiler.JS: A (UTC), B
(`orderings`), C (invariant). No New York part. No measurement. The reason: collation reads no zone; JSD-0082's decision
D23 is the precedent.

**Decision E11 - the deletion lists.** This family: `JsCollator`, `JsCollationData`, `NewCollator`'s core and
`CollatorKeyData`. Broiler.JS: `ResolveCompareInfo` for the covered locales, `StripIgnorablePunctuation`'s tie-break,
the `CompareOptions` mapping; `NetCollationSuffix` moves into the host provider. `JSIntlCollator` stays with every
public member (SJL-D-25 (a)). The reason: X6. *Reading (finding F4):* the plan's text cites `NewCollator`'s core as
`JsRealm.Intl.cs:165-235` and `CollatorKeyData` as `:240-246`; at `83290f9` the constructor is `:168-227`, its core
`:202-215`, and `CollatorKeyData` `:235-242`. The hard-coded Thai default goes with decision E5.

**Decision E12 - records.** A verdict JSD amending JSD-0043 §5 to §7; a dated note on SJL-17.5's normalization item;
the area design's section 3.4 corrected. The reason: the roadmap's section 11. *Reading:* SJL-17.5's normalization item
already carries JSD-0082's dated note of 2026-10-26, and the area design's section 3.4 its note on the stale "Today"
cell; this record's notes stand beside them and say what SJL-17 adds.

**Decision E13 - data size.** This family: rule N28's budget holds the unchanged tables (clause 9). Broiler.JS: the
collation size stated in its bundle.

**Decision E14 - windows.** One merge per engine; the bases after SJL-9's after commits; the library merges outside
SJL-9's windows. The reason: as JSD-0082's decision D4.

## The owner's decisions of 2026-11-02

*(Added 2026-11-02.)* SJL-17.2's first engine-level runs of the collation vectors met two answers no row recorded as
written, and one choice the vectors had to make, on row O3's open reading. The vectors are Broiler.JS's corpus revision
33, whose expected values are the reference's, ICU 78.3 with CLDR 48.0. Broiler.JS's first run was taken at its
`d85da200` under `TZ=UTC`, on the machine the correspondence names, with ICU 74.2. This family's was taken at `b43d0bb`
from a scratch archive of revision 33, and it has no difference on the area. Each item was decided on 2026-11-02 as
recommended, under the owner's direction of 2026-10-18 quoted above: "Please do all recommended decisions and skip
waiting for review." Each is recorded here as the owner's decision of 2026-11-02. Broiler.JS's corpus revision 34
classifies Broiler.JS's run by them. The decision numbers E1 to E14 are the plan's, so these three carry names:
**E-han**, **E-o5b** and **E-o3j**. The correspondence carries a dated note for each, beside the text it reads.

**Decision E-han - Han across blocks is each engine's own data's.** ECMA-402 leaves the collation order to the
implementation and does not mandate CLDR's root. UTS #35 Part 5 (CLDR 48) lets an implementation order Han characters
by the UCA's implicit weights or by radical and stroke. The order of unified ideographs across blocks is where the
engines' data differ: this family's tables take the implicit weights by block, and the reference and Broiler.JS's host
take radical and stroke. So the 34 Han vectors whose answer moves with the order are `impl-defined`. Their `out` is the
reference's answer, and each engine's expectation is its own data's answer: this family's from its tables at
`4e6a41e9` (it meets every one), and Broiler.JS's from its host, the radical-stroke order. They are not a correction
of either engine. Finding F1, row O3's reading, stays open. SJL-17.3 decides it, with the radical-stroke data sized
against rule N28 and the data home, and row O3's class stays data policy (C5). This record's "One reading is open: Han
across blocks (O3, F1)" stands as written.

**Decision E-o5b - row O5b holds every sensitivity below the tertiary.** Row O5b read "NBSP against space at `base`";
its Broiler.JS cell reads "the host's `IgnoreCase | IgnoreNonSpace` keeps the width and compatibility difference".
Broiler.JS's run found the same answer at `accent` and `case`: a fullwidth letter, `Ａ` against `A`, and U+3300
against its katakana, each `<` or `>` where the reference and this family answer `=`. Row O6's probes held no width
form, so no row recorded it. The host's ICU 74.2 answers `=` at the secondary and primary strengths. So the difference
is Broiler.JS's `CompareOptions` mapping, which passes no `IgnoreWidth`, and row O5b holds it at `base`, `accent` and
`case`: the same correction of Broiler.JS, listed among its corrections above, which SJL-17.5 makes.

*(Note of 2026-11-02, at the review of SJL-17.2's change. Recorded under the owner's direction of 2026-10-18, as the
decision was. It corrects what the decision says the run showed, and changes no row, class or rule.)* The decision
above reads "Broiler.JS's run found the same answer at `accent` and `case`: a fullwidth letter, `Ａ` against `A`, and
U+3300 against its katakana". With the correspondence's first note on row O5b, it says that row O5b holds Broiler.JS's
"width and compatibility differences" at every sensitivity below the tertiary. That is more than the run showed:
- The run compared `Ａ` against `A`, and U+3300 against its katakana, at `base` and `accent` only. Broiler.JS answered
  `>` and `<` there.
- It compared `a` against fullwidth `ａ` at `base`, `accent` and `case`. Broiler.JS answered `<` at each.
- What Broiler.JS keeps apart below the tertiary strength is width (a fullwidth letter, and NBSP against a space) and
  the squared katakana word U+3300.
- The other compatibility forms the vectors hold, Broiler.JS answers as the reference and this family do: `ﬁ`, `ǆ`,
  `①`, `½`, `Ⅳ` and `²` at `base` and `accent`, and `ª` and `ⓐ` at `base`, `accent` and `case`.

The decision stands as to its row, its class and its reach. Row O5b holds these differences at `base`, `accent` and
`case`, and they are a correction of Broiler.JS that SJL-17.5 makes. The list of Broiler.JS's corrections below, "`base`
folding width and compatibility differences (O5b)", says what the shared collator does, and it stands. The correspondence
carries a second dated note on row O5b. Broiler.JS's corpus revision 34 words its reason `js-o5b` the same way (on its
branch `sjl-17-2-vectors`, observed).

**Decision E-o3j - Extension J on Broiler.JS's host is data policy.** Row O3's Broiler.JS column reads "𠀀 < 中, as the
host's ICU orders unified ideographs (radical and stroke)". Broiler.JS's run found that the host's ICU 74.2, which
carries Unicode 15.1, does not know CJK Extension J (U+323B0 to U+33479, Unicode 17.0). It weighs those code points as
unassigned and puts them after every ideograph it knows, so 中 < U+323B0 where the reference has U+323B0 < 中. The
difference is the host's data, and row O3's class, data policy (C5), holds it. A host whose ICU carries Unicode 17
answers as the reference, and SJL-17.5 takes Broiler.JS's root from the shared data.

**What did not move.** No row changes class, no rule changes, and this record's list of Broiler.JS's corrections names
the same rows. Every other difference of Broiler.JS's run is under a row as written: O2 (the host's UCA 15.1 weights
on the conformance files' pairs), O5a, O7 and O8.

## Every row, classified

| Same | Correction of Broiler.JS | Correction of Broiler.VM | Implementation-defined | Data policy (C5) | Engine policy | Code no input reaches |
|---|---|---|---|---|---|---|
| O4, O6, O9, O13, O19 | O5a, O5b, O7, O8, O10, O12, O14, O18 | | O11a | O1, O2, O3, O5c, O11b, O16 | O15, O17 | |

**Broiler.JS's corrections**, each a fix under decision SJL-D-16 (a), are listed in its changelog at SJL-17.5 with
their rows: `ignorePunctuation` ignoring punctuation and not symbols (O5a); `base` folding width and compatibility
differences (O5b); `numeric` and `caseFirst` applied (O7, O8, and the `orderings` lines they move, O18); the
constructor's reads in the specification's order, once each, with a `RangeError` for a malformed `collation` (O10); the
bound `compare` cached (O12); and no code-unit comparison under invariant globalization (O14). Its data changes (C5)
are listed with the decision they follow (decisions E3 and E5).

## The correspondence's findings, and what answers each

| Finding | What it found | Answered by |
|---|---|---|
| F1 | Han across blocks: this family's implicit weights against ICU's order; nothing archived separates them | Decision E9's reading; the falsifier below. SJL-17.2's vectors check it |
| F2 | Broiler.JS's passing files need `eor` for `de` and a collation for `en` | Decision E3's reading; Broiler.JS's rule, part A, names the getCollations file (JSD-0082's decision D33 (c)) |
| F3 | The `orderings` program has no divergences file | Broiler.JS's rule, part B, and its `divergence-groups.tsv`, which holds no group |
| F4 | Decision E11's citations have drifted | Decision E11's reading |
| F5 | The plan's part C cites "JSD-0081 decision D7" | Broiler.JS's rule, part C, cites JSD-0082's decision D7 |
| F6 | Under invariant globalization only `en` and `en-US` are available, so no tailoring applies there | Decision E3's reading; Broiler.JS's `invariant-probe.expected.txt` |
| F7 | JSD-0043 §5's "the profile's own normalization" has been Broiler.Unicode's since SJL-7.4 | JSD-0043 §5's dated note |

## JSD-0043, where it moves

JSD-0043 §5 to §7 carry dated notes of 2026-11-01, each beside the text it reads, which it quotes:
- **§5, the collator**, moves to `Broiler.ECMAScript.Intl` at SJL-17.4: the algorithm, the element lookup and the
  options' mapping onto UCA settings become `UcaCollator`'s, unchanged; NFD is Broiler.Unicode's (decision E6, finding
  F7); the charges are the caller's meter at the same points and in the same units (decision E7).
  `localeCompare`'s routing through the realm's own `%Intl.Collator%` and the locale-named case methods stay in the
  profile.
- **§6, validation**: the archived CollationTest files become the library's L4 oracle as well (decision E9), and the
  `orderings` dataset the acceptance data of both engines (the milestone's SJL-17.1); the slice compiler's checks stay
  and are part B of bundle `sjl-17-001`'s rule. Its test262 figures are its own date's, and stay as written.
- **§7, what is not done**: for this family it stands, since its list and its tables do not change (decisions E3 and
  E13). For Broiler.JS's embedding, Thai's tailoring and its default come from the shared data (decisions E3 and E5);
  `eor` stays outside the shared data until the rule engine implements its syntax (finding F2).

## Decision

1. **Collation moves to `Broiler.ECMAScript.Intl`** as `UcaCollator`, under X3's constraints: resolved options as a
   record, data through `IIntlDataSet`, normalization through Broiler.Unicode, and the caller's meter (decisions E6 and
   E7). SJL-17.3 settles the names.
2. **It is an adoption** (decision E2): the shared collator is this family's, and Broiler.JS takes it for the locales
   and collations decision E3 lists, keeping a host provider for the rest.
3. **The data** is the data home's generator's output: this family's tables byte for byte under rule N28, Broiler.JS's
   for decision E3's list, its size stated in its bundle (decision E13).
4. **The engines keep:** their `Intl.Collator` objects and prototypes, the option reads in the specification's order,
   `localeCompare`'s coercion, their errors, the bound `compare` and its caching, and their default-locale and
   available-locale providers.
5. **Corrections.** This family has none. Broiler.JS's are the rows of its column, fixes under SJL-D-16 (a), listed in
   its changelog at SJL-17.5.
6. **The deletion lists** are X6's (decision E11).
7. **The `orderings` dataset** is the acceptance data of both engines, and the bases are each repository's bundle
   `sjl-17-001`'s, at the first parent of its cut-over merge. The rules are committed with this record.
8. **JSD-0043 §5 to §7 are amended where they move**, by dated notes.

## What this rejects, and why

- **Extraction from Broiler.JS**: it has no algorithm to extract (decision E2).
- **One locale list for both engines**: this family's table cannot grow (rule N28), and Broiler.JS may not lose a
  passing test (decision E3).
- **CJK in the shared data now**: its tailorings are unsized (decision E3).
- **A code-unit fallback under invariant globalization**: the area design asks that no host data be needed, and the
  answer would depend on the host's configuration (decision E4).
- **A class C2**: the shared collator charges where this family does (decision E7).
- **A part in a zone with daylight-saving time, or a measurement part**: collation reads no zone, and the milestone
  measures nothing (decision E10).

## Settling condition

This record is settled when SJL-17.4's and SJL-17.5's cut-overs have landed with both deletion lists absent, and every
part of both repositories' `sjl-17-001` rules has been judged, whatever each decides.

## Correction entries

None in this profile's plan documents at this record, and no JSC: SJL-17.4's cut-over takes the next free JSC, or
records its changes in a section of this record. These carry dated notes beside the text they correct, each quoting
it, in this record's change: JSD-0043 §5, §6 and §7; SJL-17's milestone file; and the area design's section 3.4.

## What this record does not decide

- **The cut-overs.** SJL-17.4 and SJL-17.5 apply this record.
- **The vectors.** SJL-17.2 adds them, and they may falsify this record.
- **The names of the library's members.** SJL-17.3 settles them.
- **Any CJK tailoring**, which waits on a data decision that sizes it (decision E3).
- **Publication**, which waits on the owner's approval.
- **A speed claim of any kind.** SJL-17 measures nothing (decision E10).

## Falsified if

- A vector or a dataset line finds a difference the correspondence does not classify.
- A reading of the pinned text or of CLDR 48.2.0's data in the correspondence that the vectors refute, until the row
  is reclassified here; row O3 is named (finding F1).
- The home's generator, from this family's pins, does not write `JsCldrTables.g.cs` byte for byte.
- This family's `orderings` lines, or any other Intl check's, move at its cut-over.
- A test262 file Broiler.JS passes at its base fails after its cut-over, outside the corrections this record lists.
