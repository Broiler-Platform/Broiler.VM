<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0086 - The profile collates with the shared Intl library: SJL-17.4's cut-over

**Status:** Decided 2026-11-04. It applies
[JSD-0084](0084-collation-moves-to-the-shared-intl-library.md), the area's verdict, which leaves this cut-over to
"the next free JSC, or ... a section of this record" (its section "Correction entries"). **The code landed in the
changes before this record**, on one branch that lands as one merge (JSD-0084 decision E14), so that the local
pre-checks this record cites could be taken at commits of the code it judges; this record is a change after them. A
review of the branch found three small corrections to the product source, which `066f012` makes after this record was
first written and before its revision that says so; it changes no answer (section 4).

**Date:** 2026-11-04.

**Owner:** the JavaScript profile owner. **Co-signers:** the core architecture owner, for rule I5's register and the
composition register. **Both roles are held by one person** (EX-30), and this record does not claim that anyone
independent has read it.

**Milestone:** the shared JavaScript libraries programme's SJL-17, work package SJL-17.4
([the milestone](../../../../docs/javascript-shared-libraries/milestones/sjl-17-collation.md)). SJL-17 started before
SJL-9, which it waits on, met its gate, and it may not meet its gate before SJL-9 meets SJL-9's, and so not before
SJL-7, SJL-8 and SJL-3 meet theirs; the ledger records both. **The owner's direction of 2026-10-18**, "Please do all
recommended decisions and skip waiting for review.", is read as JSD-0084 reads it: the choices this record makes within
the records are its own, recorded below, and nothing here is accepted by it. No owner's decision is taken by this record.

---

## Context

JSD-0084 accepted collation as an adoption: the shared collator is this family's `JsCollator` and `JsCollationData`,
ported to `Broiler.ECMAScript.Intl` as `UcaCollator` and `UcaCollationData` at SJL-17.3 (Broiler-Platform/Broiler.JS#1094,
`c884f87b`, observed). The owner published the shared stream's `0.1.0-preview.9` on 2026-11-03 from Broiler.JS
`6f7284da`, which descends from `c884f87b`, and this family admitted `Broiler.ECMAScript.Intl` at that version at
SJL-9.4 ([JSD-0085](0085-the-profile-on-the-shared-intl-library.md)). This record is the change: the profile's
collation is the shared library's, and the code it replaced is deleted. The evidence that judges it is bundle
[`sjl-17-001`](../../../../docs/evidence/sjl-17-001/README.md), under the rule SJL-17.1 committed before any of this code
existed; its runs are SJL-17.6's.

**Why a record of its own, and not a section of JSD-0084.** JSD-0084 and SJL-17's plan allow either. JSD-0084 is the
area's verdict, decided on 2026-11-01, and its status says "Nothing in this profile changes with this record. The
cut-overs are SJL-17.4's and SJL-17.5's own changes, and this record is what they apply." A section there would make a
decided verdict carry a change, and its settling condition reads both cut-overs, which land in different repositories at
different times. SJL-9.4's record, JSD-0085, is the precedent: a cut-over record of its own beside the verdict it
applies. JSD-0084 gains one dated status line that names this record, and nothing else of it is edited.

**The package.** No package moves. `Broiler.ECMAScript.Intl` `0.1.0-preview.9`, as restored from nuget.org with the
content hash rule I1's allow-list holds, carries the collator: the package's documentation file names
`UcaCollator`, `UcaCollatorOptions`, `UcaCollationData`, `CollationStrength` and `CollationCaseFirst`, and the
assembly declares them, with the members the library's `PublicAPI.txt` at `6f7284da` lists. So SJL-17's plan's "package
re-pin to the version carrying the collator (owner approval)" has nothing to do, and nothing is published.

**The changes**, each building, each with the tests it touches green at its commit, on the branch
`sjl-17-4-cutover`, which lands in one merge: its after commit is that merge, and its base the merge's first parent,
`68cad8b` when the branch was cut (bundle `sjl-17-001`'s rule names them).

| Change | What it does |
|---|---|
| `eb29fb8` | The cut-over: `JsCollator`, `JsCaseFirst` and `JsCollationData` deleted, `NewCollator`'s tailoring core and `CollatorKeyData` replaced by calls to the library, `localeCompare` and the bound `compare` repointed; the review record's count; the assurance artefacts |
| `f2bcca4` | Rule I5 registers every type and member the cut-over deleted (section 5) |
| `066f012` | After the review: `CompareStrings` is private, as its only callers are `JsRealm`'s own; two comments on the collation data's adapter and one on `CollatorLocaleData` say what they read; the assurance artefacts |

**What did not move.** This family's tables (`JsCldrTables.g.cs`, held by rule N28), its CLDR pin (rule N27), the
Format assembly's `IJsIntlData` and `JsIntlTable`, `JsComposition.IntlData`, `JsIntlTables`' name, `JsEngine`'s
constructor (JSD-0082 decisions D8 and D31), the test262 pin `ccaac100`, the vectors pin, the packages, the composition
roots' assemblies, the default locale `en-US`, the available locales, and optionality: nothing under
`src/Broiler.VM.Profile.JavaScript.Intl/`, `src/Broiler.VM.Profile.JavaScript.Format/`, `src/tests/cldr/`,
`src/tests/temporal/` or `src/compositions/` changes on the branch.

## Decision

### 1. The data and the seam (decision E11; JSD-0082 decisions D8 and D31)

| Seam | What it is |
|---|---|
| `JsIntlTables.CollationData` (new, internal) | The library's `UcaCollationData` over a `JsIntlDataSet` of its own, the adapter type `LocaleData` reads too, which holds only the data's reference and hands it the composition's `CollationRoot` and `CollationTailorings` unchanged; it decodes them once, on first use, thread-safely, as the deleted lazy did |
| The settings table | `UcaCollationData` also reads `IntlTable.CollationSettings` (32), which SJL-17.3 appended for Broiler.JS's Thai; `JsIntlTable` does not define 32, so `JsIntlDataSet` answers it empty (JSD-0085 section 2), and no tailoring of this family's data states a setting |
| Unchanged | `IJsIntlData`, `JsIntlTable` and `JsComposition.IntlData`; `JsIntlTables`' name and `JsEngine`'s constructor |

**N28, byte for byte.** `src/Broiler.VM.Profile.JavaScript.Intl/` does not change on the branch: `JsCldrTables.g.cs`
hashes to the same SHA-256 at `68cad8b` and at the branch's head, so the collation tables in it, the root, root
`search`, and German `phonebook` and `search`, are the same bytes; and rules N27 and N28, which regenerate the file with
the data home's generator from this family's pins and compare it byte for byte, pass at every commit of the branch.

### 2. The adapter (decision E11; JSD-0084's "Decision", item 4)

- **`NewCollator`** (`VM/JsRealm.Intl.cs`) keeps the prototype from the new target, CanonicalizeLocaleList, and every
  option read in the specification's order, each checked as it is read: `usage`, `localeMatcher`, `collation` with its
  type check and `RangeError`, `numeric`, `caseFirst`, then ResolveLocale, then `sensitivity` and `ignorePunctuation`.
  What the deleted core computed is the library's:
  - the relevant keys' values ResolveLocale reads are `UcaCollationData.KeyValues` for the locale's language, the key and
    the usage, where `CollatorKeyData` answered them (`co`: `null`, then `phonebk` for German sorting; `kn` and `kf`
    their fixed lists);
  - the tailoring is `UcaCollationData.TailoringFor` for the data locale's language, the usage and the resolved
    collation: `de-search` or `und-search` for search, `de-phonebk` for German phonebook sorting, and the root
    otherwise, as the deleted choice was;
  - `ignorePunctuation`'s default is the tailoring's `ShiftedByDefault`, which is false for every tailoring this
    family's data carries, as the literal `false` was (decision E5's data, which only Broiler.JS's tables state);
  - the collator is a `UcaCollator` under `UcaCollatorOptions.FromResolved(sensitivity, ignorePunctuation, caseFirst,
    numeric, tailoring)`, the same mapping the deleted constructor call made, with the profile's own
    `UnicodeNormalization`, `JsUnicode.Normalization`.
- **`CompareStrings`** (new, private, `VM/JsRealm.Intl.cs`) is ECMA-402's CompareStrings for a collator object: it
  calls `UcaCollator.Compare` with the engine's string-length ceiling and its meter, and maps the library's
  `NormalizedStringTooLong` to the `RangeError` "Invalid string length" that `normalize` throws, which `JsCollator`
  threw through `NormalizeText`. A probe added after the review, of strings whose NFD form falls below, at and past the
  ceiling, through the bound `compare` and `localeCompare` under the root, German search and phonebook, answers as at the
  base, and its refusal's charge is the base's (section 4).
- **Repointed callers:** the bound `compare` (`VM/JsRealm.Intl.cs:148`, was `:135` at `68cad8b`) and
  `String.prototype.localeCompare` (`VM/JsRealm.String.cs:641`, was `:639`), which still makes its collator with the
  realm's own `%Intl.Collator%`; `CollatorLocaleData.SortLocaleData`, which `Intl.Locale.prototype.getCollations`
  (`VM/JsRealm.Locale.cs:125`) reads through the library's Locale info, now reads `KeyValues`, where it read
  `CollatorKeyData`. `Intl.supportedValuesOf("collation")` does not read it: the library's `SupportedValues.Of` reads
  `CollatorLocaleData.Collations`, the fixed list `phonebk` the adapter already held at `68cad8b`, which this change
  does not touch and which reads no table.
- **What stays the engine's**, as JSD-0084's decision item 4 lists it: the `Intl.Collator` object and prototype,
  `JsCollatorObject` with its resolved options, the bound `compare` and its caching, `localeCompare`'s coercion, the
  errors and their messages, and the default-locale and available-locale providers.

**No member had to stop:** the library answered every line of the local pre-check as `JsCollator` did (section 4).

### 3. Charging (decision E7, row O17)

`UcaCollator.Compare` takes the engine's `WorkCharge`, and charges where and as `JsCollator` charged, in this family's
units: equal strings charge nothing; NFD of each string through `UnicodeNormalization.Transform`, whose charge is the
one `normalize` makes, the same call `NormalizeText` made with the engine's `UnicodeCharge` over the same `Charge`; and
then one unit per collation element of both strings, after both are looked up, at the deleted
`JsCollator.cs:107-109`'s point. The option reads and ResolveLocale charge as before, in the engine. **No C2:** the
charge probes of the local pre-check complete at their base figures and run out one below, in both forms, and part C's
twins answer as at the base (section 4).

### 4. Evidence at this change

Local pre-checks, on binaries built at each commit of the branch that changes product source (a commit that changes
none keeps the binaries before it), against base answers on binaries built at `68cad8b` and kept, with the scripts and
logs kept outside the tree; they are not evidence of the rule, whose runs are SJL-17.6's at the base and after commits.
No figure of them is quoted here. **When they were taken.** The base's test262 runs, checks, twins, probes and charge
probes were taken before any change was committed, and the cut-over's (`eb29fb8`) before the next commit. The collation
vectors were not: the base's ran on the kept `68cad8b` binaries after `eb29fb8` was committed, and the cut-over's
finished after `f2bcca4` was, which changes no product source. The review's corrections, `066f012`, were taken whole
again, on binaries built at it, against the same base answers, before the next commit; with them, two probes the review
asked for, run on the kept base binaries first, and a second run of the base's checks.
- **test262**, `test/intl402/Collator`, `test/intl402/String/prototype/localeCompare` and
  `test/built-ins/String/prototype/localeCompare`, in the bytecode and the native forms: no variant moved from its base
  answer.
- **The wider net**, `test/intl402` whole and every `toLocale*` directory with `localeCompare`, in the bytecode form: no
  variant moved.
- **Fuel ceilings**, the collation selection at four low ceilings in the bytecode form: no variant moved.
- **The slice compiler's `--checks`**, built in Release and run in a `git archive` of the commit: every check passes, and
  every line of the intl, tzdb and temporal checks, part B's collation checks and the `orderings` check among them,
  reads as at the base. Of the other checks' lines, with addresses and wall-clock timings set aside, one differed at
  the cut-over: a native baseline layout check's allocated-byte figure, whose verdict holds at both commits. A second
  run of the base's checks on the same base binaries moves that figure too, and nothing else, so it varies between runs
  of one commit; at `066f012` every line reads as at the base.
- **Part C's twins**, every `*.small.js` twin of `src/tests/forms/wide`, bisected at the base under the bytecode form
  with the command-line root: each completes at its base figure and is refused on fuel one below, in both forms, with
  the base's answer.
- **Probes**: the branch's own, of root order across scripts with canonically equivalent and ignorable sequences,
  contractions and Han across blocks, German phonebook and search, every tailoring this family's tables carry under
  every usage, the four options and their extensions, lone surrogates, `resolvedOptions`, the option read order, the
  errors with their names and messages, `localeCompare`, the bound `compare`, `getCollations` and
  `supportedValuesOf("collation")`, in both forms; and five charge probes, bisected at the base: every line answers as
  at the base. After the review, a probe of the string-length ceiling through `CompareStrings` (section 2) and a sixth
  charge probe, of its refusal, bisected at the base, answer as at the base at `066f012`.
- **The collation vectors**: the conformance root over Broiler.JS's corpus revision 34, from the scratch archive bundle
  `sjl-17-001`'s `vectors/scratch-pin-revision-34.pin` pins, at the base and at the cut-over: every collation vector
  answers alike, and so does every other.
- **The architecture and contract tests** pass, and the solution builds in Release with no warning.

### 5. Deleted, and rule I5 (decision E11)

The code decision E11 lists for this family is deleted, and every type and member among it is registered in rule I5's
`retired-members.json`, each with what in `Broiler.ECMAScript.Intl` replaced it. The rows were taken from the assurance
manifests at the base and at the cut-over: every unit the cut-over no longer declares, registered by type where its type
went whole and by member where its type stays.
- **Whole files:** `JsCollator.cs`, with `JsCollator` and `JsCaseFirst`, and `JsCollationData.cs`, with
  `JsCollationData` and its nested `Tailoring`.
- **From the types that stay:** `JsRealm.CollatorKeyData`, and `JsIntlTables.Collation` with its lazy `collation`.
  `NewCollator`'s tailoring core is lines of a member that stays, not a member, and `JsCollatorObject`'s constructor
  stays with the library's type as its parameter.

**What stays, and holds no collation algorithm:** the adapters of section 2. The register's comment names them, and
says, by a dated note on SJL-9.4's paragraph, that `CollatorKeyData`, which that paragraph names as staying, went here.
Rule I5's register row names SJL-17.4 and this record, dated. Rule I5's test needs no new file in its non-vacuity list:
the files the registered members lived in were deleted whole or are `VM/JsRealm.Intl.cs` and `VM/JsIntlTables.cs`, which
it reads already, where the adapters live.

### 6. Rules and public API

- **I1, A7, N37, N38**: no package, edge, baseline or notice moves.
- **K2, K3, K4, I7**: no root's assemblies move. The profile has referenced `Broiler.ECMAScript.Intl` since SJL-9.4, and
  trimming kept the assembly in every root then; the composition register's rows and their closures, bundle
  `sjl-9-001`'s `cutover/closures/`, stand, and SJL-17.6 retakes them at the after commit.
- **I2** reads the library's assembly from its package as it did at SJL-9.4; the collator names no culture and no
  platform collation, normalization or casing API (JSD-0084 X3 and X5).
- **I3**: optionality is unchanged; what `localeCompare` does in a composition with no Intl data does not move.
- **I6**: no type of the library appears in the family's public API baseline; `docs/api/public-api.txt` does not move.
- **I5**: section 5. **N27 and N28**: section 1.
- **The review record** counts the files the deletions leave, with a dated paragraph, and the JSD-0043 paragraph naming
  the two files gains a dated note.

### 7. Records

- **Assurance.** Regenerated at each commit; every human line stays PENDING. The code that left this component's
  assurance coverage is the home's to cover, as EX-125 to EX-132 say of the earlier areas; SJL-17.6 files its exclusion
  in bundle `sjl-17-001`, as SJL-9.6 does for SJL-9.4's.
- **Correction entry.** [JSC-325](../roadmap.corrections.md#jsc-325).
- **Dated notes**, each quoting what it reads: JSD-0084's status; the milestone's SJL-17.4 section; the area design's
  deletion lists (section 3.4's companion, "the lists"); `inventory.md`'s collation row; the I5 register's SJL-9.4
  paragraph; the remarks of `JsRealm.Intl` and `JsIntlTables`; and bundle `sjl-17-001`'s README.

## Readings only an embedder's data can show

Every shipped answer is unchanged by these, and no probe moved.
Each is read from the code, not run: no composition of this family carries other collation tables, and the pre-check
runs only this family's data.
- **When the tables are decoded.** `JsCollationData` was decoded when `NewCollator` first read `JsIntlTables.Collation`,
  after the `sensitivity` and `ignorePunctuation` reads; `UcaCollationData` decodes on the first question, which is now
  ResolveLocale's `co` values, before them, and `Intl.Locale.prototype.getCollations` now asks it too
  (`supportedValuesOf` does not; section 2). Decoding charges nothing, so no meter reading moves; an embedder's malformed
  tables fail at that earlier point, with the library's `InvalidDataException` where the deleted decoder failed with an
  index error, as an internal defect either way.
- **Data without the collation tables.** `IJsIntlData` lets an embedder's data answer a table empty, and the profile
  does not require the collation tables. With no root table, the deleted code failed in `NewCollator` and so in
  `localeCompare`, after the `sensitivity` and `ignorePunctuation` reads, with an index error; the library fails there
  too, with its `InvalidDataException` ("the data set carries no root collation"), before those two reads. And
  `getCollations`, which answered the deleted fixed list (`phonebk` for German, nothing otherwise) without reading the
  tables, now fails the same way, as an internal defect, where it answered.
- **Fewer tailorings.** Tables with a root but without German `phonebook` no longer offer `phonebk`: `getCollations`
  drops it, and `de-u-co-phonebk` or the `collation` option `phonebk` resolve to `de` and `default`, where the deleted
  code resolved `phonebk` and then compared by the root, as it does now; `supportedValuesOf("collation")` still lists
  `phonebk`. Tables with `und-search` but not German `search` now give German search `und-search`, where the deleted
  code compared by the root.
- **More tailorings.** An embedder's tables that carried a tailoring named `language-type` would now be offered as that
  language's collation by `KeyValues`, and so by `getCollations`, and chosen by `TailoringFor`, but
  `supportedValuesOf("collation")` would not list it, as it reads the fixed `Collations` list and not the tables; and a
  `CollationSettings` table its data could not carry through `IJsIntlData` would not be read. This family's tables carry
  the root and three tailorings, German `phonebook` and `search` and root `search`, and the answers are the deleted
  code's.
- **Not taken.** Keeping the deleted fixed lists for such data would put back the key data decision E11 deletes, so the
  adapter reads the data; making `Collations` read the tables is a product change of its own, with its own pre-check, and
  SJL-17.4 does not make it.

## Where the plan, JSD-0084 and the code disagreed

JSD-0084 wins over the plan in each case, and the code follows JSD-0084. None edits JSD-0084 but its dated status line.
1. **The re-pin.** The plan's outputs for SJL-17.4 include "package re-pin to the version carrying the collator (owner
   approval)". `0.1.0-preview.9`, admitted at SJL-9.4, already carries it (Context), so there is no re-pin and nothing
   to publish.
2. **The deletion list's lines.** The plan's decision E11 cites `NewCollator`'s core as `JsRealm.Intl.cs:165-235` and
   `CollatorKeyData` as `:240-246`; JSD-0084's reading of finding F4 gives `:202-215` and `:235-242` at `83290f9`. At
   `68cad8b`, the branch's base, `NewCollator` is `:177-237`, its core the key data and the tailoring's choice with the
   collator's construction, `:197-224`, and `CollatorKeyData` `:238-251` with its comments.
3. **What the list does not name.** Neither the plan nor JSD-0084's X6 names `JsCaseFirst`, which `JsCollator.cs`
   declared, or `JsIntlTables`' `Collation` and its lazy, the decoder's handle; each went with what it served, and rule I5
   registers it.
4. **The record.** The plan's outputs read "the cut-over JSD or a section of the verdict JSD", and JSD-0084's "a section of
   this record" or the next JSC: this record and JSC-325 (Context).
5. **The data's identifiers.** JSD-0084's X3 says the collator reads "the identifiers `CollationRoot` and
   `CollationTailorings`"; SJL-17.3 added a third, `CollationSettings`, which this family's data never carries (section 1).
6. **Decision E5** ("Thai's `[alternate shifted]` comes from the tailoring data") names Broiler.JS's Thai; the adapter
   reads the default from data as the specification's steps do, and this family's data states none (section 2).

## Correction entries

[JSC-325](../roadmap.corrections.md#jsc-325): rule I5's register and its row, the review record's count, the remarks of
`JsRealm.Intl` and `JsIntlTables`, and the programme's documents that named the deleted types as this family's. No other
plan document of this profile says what changed.

## Settling condition

Settled in the change that files it. **This record is reopened only by a new dated record.**

## What this record does not decide

- **Whether SJL-17 is met.** Its evidence is bundle `sjl-17-001`, read at SJL-17.6 under its rule, and the ledger records
  the result.
- **Broiler.JS's cut-over**, SJL-17.5, which its own changelog records with its corrections (JSD-0084's decision item 5).
- **A move of the test262 pin, the CLDR pin, the vectors pin or this family's tables.** None moves.
- **Any CJK tailoring**, which waits on a data decision that sizes it (decision E3).

## Falsified if

- a member rule I5 registers is declared again in the family's product sources;
- an adapter of section 2 computes a collation element, a weight, a level comparison, a tailoring's choice or a key's
  values itself, `CollatorLocaleData.Collations` aside: the fixed list `supportedValuesOf("collation")` reads, which
  the adapter held at `68cad8b` and this change does not touch (section 2);
- a library status reaches a script as another error type or message than the base gave;
- a test262 verdict, a check, a dataset line, a twin's figure or a charge moves at the after commit, without a dated
  decision admitting it;
- this family's tables, regenerated by rule N28, differ from the committed ones by a byte.
