<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0085 - The profile on the shared Intl library: SJL-9.4's cut-over

**Status:** Decided 2026-11-04. It applies
[JSD-0082](0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md), the area's verdict, which names
this record as "the second record" SJL-9.4 files (its decision D2). **The code landed in the changes before this
record**, on one branch that lands as one merge (JSD-0082 decision D3), so that the local pre-checks this record cites
could be taken at commits of the code it judges; this record is a change after them.

**Date:** 2026-11-04.

**Owner:** the JavaScript profile owner. **Co-signers:** the core architecture owner, for rule I1's allow-list, rule
I5's register and the composition register, and the release owner, for the notices rows. **All three roles are held
by one person** (EX-30), and this record does not claim that anyone independent has read it.

**Milestone:** the shared JavaScript libraries programme's SJL-9, work package SJL-9.4
([the milestone](../../../../docs/javascript-shared-libraries/milestones/sjl-9-locale-data-and-intl.md)). SJL-9
started before SJL-7 and SJL-8, which it waits on, met their gates, and it may not meet its gate before they meet
theirs, and so not before SJL-3 meets SJL-3's; the ledger records both. **The owner's direction of 2026-10-18**,
"Please do all recommended decisions and skip waiting for review.", is read as JSD-0082 reads it: the decisions this
record takes within the records are its own, recorded below, and nothing here is accepted by it. **The owner's
decision of 2026-11-04**, taken under that direction as the recommended option, is recorded in JSD-0082's dated note on
decision D15 and in the dated revision of bundle `sjl-9-001`'s rule (section 4).

---

## Context

JSD-0082 accepted the five sub-areas of locale data and the Intl algorithms, (a) to (e), and placed them in a new
library, `Broiler.ECMAScript.Intl`, over locale data this family's generator writes and each engine embeds. SJL-9.3
built the library in the Broiler.JS repository; the owner published it on 2026-11-03 as the shared stream's
`0.1.0-preview.9`, from Broiler.JS `6f7284da`, beside the algorithms and generator packages this family already
admits at that version. This record is the change: every Intl answer this family computed in its own code is the
shared library's, that code is deleted, and rows Z8, L18 and L9 are corrected. The evidence that judges it is bundle
[`sjl-9-001`](../../../../docs/evidence/sjl-9-001/README.md), under the rule SJL-9.1 committed before any of this code
existed, as revised before its runs (section 4).

**The changes**, each building, each with the tests it touches green at its commit, on the branch
`sjl-9-4-cutover`, which lands in one merge: its after commit is that merge, and its base the merge's first parent
(JSD-0082 decision D4). `c4beaf1` alone fails rules N37 and N38, as `3008a99` failed N37 until `908e942` took the baseline again, and
`4842f8d` makes both pass.

| Change | What it does |
|---|---|
| `c4beaf1` | Task 7.1: admits `Broiler.ECMAScript.Intl` `0.1.0-preview.9` from nuget.org for the profile alone, and references it from the profile |
| `4842f8d` | Takes the package baseline again, bundle [JS-10-015](../evidence/js-10-015/README.md), at `c4beaf1` (rule N37), and folds the notices row (rule N38) |
| `2b798a7` | Task 7.2, sub-area (a): the locale core onto the library |
| `3d05880` | The closures of every root, taken at `2b798a7`, and the composition register (rules K2 to K4, I7) |
| `cc55d06`, `bce9af1` | The review of stage 1: `JsIntlTables` says which table groups were decoded twice until (b) to (e); the branch's dated notes quote what they replace |
| `9d9758a` | Task 7.3, sub-area (b), and the number formats and relative times of (c) |
| `c0066b8` | Task 7.3, the list and display-name formats of (c) |
| `4303ceb` | The review of stage 2: a DisplayNames message restored, the chain reads recorded, two members without a reader removed |
| `5862252` | Task 7.4, sub-area (d), without row Z8 |
| `ee0fd1f` | Row Z8's correction, and `canonical-tz` admitted to the scored suite |
| `15306a9` | Task 7.5, sub-area (e) |
| `044dc40` | The review of stage 3: records and one option's parse |
| `0c3d982` | Part B's class Z8 takes one guard's literal, by a dated revision of the rule (section 4) |
| `69f9e6a` | Rule I5 registers every type and member the cut-over deleted (section 8) |
| `65ad719` | After this record: rule I5's test reads the files the rest of the registered members lived in (section 8) |

**What did not move.** This family's tables (`JsCldrTables.g.cs`, held by rule N28), its CLDR pin (rule N27), the
Format assembly's `IJsIntlData` and `JsIntlTable`, `JsComposition.IntlData`, the test262 pin `ccaac100`, the vectors
pin, the composition roots' data, the default locale `en-US`, the available locales, and optionality: nothing under
`src/Broiler.VM.Profile.JavaScript.Intl/`, `src/Broiler.VM.Profile.JavaScript.Format/` or `src/tests/cldr/pins/`
changes on the branch.

## Decision

### 1. Admission (task 7.1)

**`Broiler.ECMAScript.Intl` `0.1.0-preview.9`**, as published to nuget.org from Broiler.JS
`6f7284daa2d2fbec3104a9498d5f7c989f956946` (its publish-shared workflow, run 41679821770, tag
`ecmascript-v0.1.0-preview.9`), is admitted by the procedure this repository's main used for `0.1.0-preview.8` and
`0.1.0-preview.9` ([JSC-322](../roadmap.corrections.md#jsc-322)):
- downloaded twice, the copies byte-identical, the archive hashing to the SHA-512 nuget.org's catalog publishes for it;
- the allow-list's product entry, for `Broiler.VM.Profile.JavaScript` alone, holds that archive hash and the content
  hash a restore into an empty package cache recorded;
- its nuspec names commit `6f7284da` and depends on `Broiler.ECMAScript.Algorithms` `0.1.0-preview.9` and
  `Broiler.Unicode.Algorithms` `0.1.0-preview.6`, both admitted; it carries no third-party component and no data;
- `Directory.Packages.props`, the profile's project, `graph.manifest.json` (rule A7), `eng/packages/README.md` and
  ADR 0001's package-edge revision move with it. No `.nupkg`, `.pin` or lock file enters the tree, as since `9d0f80e`.

**The package baseline moved with it**, to bundle JS-10-015: rule N37 holds each row of the baseline to the packages
its project declares and to what the bundle's candidate pack produced, and JS-10-014's candidate did not declare this
one.
`packages.md` and `support.md` section 9 name it ([JSC-324](../roadmap.corrections.md#jsc-324)).

**The notices.** `THIRD_PARTY_NOTICES.md` gives the library the row of the algorithms package it is published with,
from the same stream and commit. A row of its own repeated that row's blocker identity for release gate 12, which rule
N38 refuses; the row is not confirmed, so the blocker stands, under its name. No other notice moves at SJL-9.4.

**The register and the closures.** Every JavaScript row of [`docs/compositions.md`](../../../../docs/compositions.md)
names `Broiler.ECMAScript.Intl` among its shared assemblies, and the five rows whose roots this machine publishes name
bundle `sjl-9-001`'s [`cutover/closures/`](../../../../docs/evidence/sjl-9-001/cutover/README.md) as their evidence
(rules K2, K3, K4, I7). Trimming keeps the assembly in every root, the roots that compose no Intl data among them,
because the profile builds its Intl surface at run time from whether a composition hands it data. The closures are
taken again from a clean checkout in the change after this record, after the last change to a product source, as
stage 1 took them (section 11).

### 2. The data and the seams (decisions D8 and D31)

| Seam | What it is |
|---|---|
| `JsIntlDataSet` (new, internal) | The library's `IIntlDataSet` over the composition's `IJsIntlData`: each table by its number; a number `JsIntlTable` does not define is answered empty and never passed to the embedder |
| `JsIntlTables.LocaleData` | The library's `TableLocaleData` over it, made once per Intl data, which decodes every table group once; `AvailableLocales` reads its locale list without `und` |
| `JsIntlTables.FormatData` | The library's `CompositeLocaleData` over `LocaleData` with no secondary provider: the formatters read through it, a locale, then its truncations, then `und` |
| `JsIntlTables.DateTimeData` | The library's `DateTimeData` over `FormatData`, the time zones and the calendars, made once |
| Unchanged | `IJsIntlData`, `JsIntlTable` and `JsComposition.IntlData`, public in the Format assembly; `JsIntlTables`' name and `JsEngine`'s constructor, so that `BigIntChecks`' accessor still binds (decision D31; the slice compiler's `--checks` passes, `BigIntChecks` among them) |

Between sub-areas (a) and (d) some table groups were decoded twice, by this family's remaining decoders and by
`TableLocaleData`; `cc55d06` and `4303ceb` said so beside the constructor, and since sub-area (d) no group is.

### 3. The sub-areas

Each adapter keeps what JSD-0082's section "Decision", item 4, leaves the engine: the Intl objects and prototypes,
every option and argument read in the specification's order and checked as it is read, coercion, the errors and their
messages, bound functions, the parts arrays with their charges, the legacy constructors and the default-locale and
available-locale policies. Each library status is mapped back to the same `RangeError` or `TypeError` with the same
message.

- **(a) Locale identifiers, negotiation, Locale info and `supportedValuesOf`** (`2b798a7`, rows L).
  CanonicalizeLocaleList keeps its walk, its type checks and its charge per element, and calls
  `LocaleId.TryCanonicalize`; ResolveLocale calls `LocaleNegotiation.MatchLocale` and `ResolveLocale`; FilterLocales
  keeps its option read and charge and calls `LocaleNegotiation.FilterLocales`; IsTypeSequence is
  `LocaleId.IsUnicodeType`; `Intl.supportedValuesOf` is `SupportedValues.Of`; the `Intl.Locale` constructor reads every
  option in order and `LocaleRecord.TryCreate` builds the record, which `JsLocaleObject.Record` holds; its parts,
  `maximize` and `minimize` are `LocaleId`'s, and its information methods `LocaleInfo`'s. Callers repointed:
  `TransformCase`, each Intl object's data-locale language, DateTimeFormat's default region, and DisplayNames' codes.
- **(b) The mathematical value, digit options and plurals** (`9d9758a`, rows M and P). `IntlDecimal` is the
  mathematical value; `SetNumberFormatDigitOptions` keeps its reads and conversions, `DigitOptionRules.Plan` chooses
  which pairs to convert, and the library takes a `DigitOptionValues` record; `PluralRulesRecord` and `PluralEngine`
  select and select ranges. `Number.prototype.toLocaleString` and `BigInt.prototype.toLocaleString` are repointed.
- **(c) Number, list, relative-time and display-name formats** (`9d9758a`, `c0066b8`, `4303ceb`, rows N and S).
  `NumberFormatter`, `RelativeTimeFormatter`, `ListFormatter` and `DisplayNamesCore`, each held by its Intl object.
- **(d) Date-time formats** (`5862252`, `ee0fd1f`, rows T and Z8). `CreateDateTimeFormat` keeps every read in order
  and builds the library's `DateTimeFormat` over a `DateTimeFormatOptions` record; `HandleDateTimeValue`, the range
  values and `ResolveTimeZone` (onto `ResolvedTimeZone.Resolve`) are thin adapters. Two members were renamed to free
  the library's type names: `DateTimeComponents` is `DateTimeComponentOptions`, the read table, and `DateTimeValue` is
  `FormatValue`. `Date.prototype.toLocale*` and every Temporal type's `toLocaleString` are repointed.
- **(e) Duration formats** (`15306a9`, rows U). `DurationFormatCore`, held by the Intl object. The constructor's reads
  stay, through `Base64Options`; `GetDurationUnitOptions` keeps its two reads; ECMA-402's ToDurationRecord stays here,
  as decision D24 keeps it, and validates through the shared `TemporalLimits.CheckDuration` (row U22);
  `Temporal.Duration.prototype.toLocaleString` is repointed.

**No member had to stop:** the library lacked nothing an adapter needed.

### 4. Corrections: every answer that moved, with its row

These are the only answers the cut-over moved. The local pre-check before each commit found no other (section 9).

- **Row L18** (decision D32; `2b798a7`). `Intl.Locale.prototype.getCollations`, for a locale whose language
  `Intl.Collator` does not support in this family's data, answers DefaultLocale's collations, `[]` for this family's
  `en-US`; it answers `emoji` and `eor` only for `und`. The change rewrote and added `collations` lines of
  `src/tests/cldr/locales/divergences.txt` and rewrote their group's header, quoting the old one: exactly the lines
  part B's class L18 names. No test262 variant scored by the rule moves.
- **Row L9** (decision D32; `2b798a7`). A subdivision alias whose replacement is a region is written as UTS #35
  writes it: `en-u-sd-fi01` canonicalises to `en-u-sd-axzzzz`, where it gave `en-u-sd-AX`, and the malformed
  `en-u-ax-sd` as a Locale, which the correspondence had not recorded. No test262 file and no dataset line reaches it.
- **Row Z8** (decision D15; `ee0fd1f`). The date-time format reports a time-zone identifier as given, in the
  database's case: `resolvedOptions().timeZone` for `Asia/Calcutta` is `Asia/Calcutta`, where it was the primary,
  `Asia/Kolkata`. The zone is still named by its primary (row T26), and no name moves.
  - `SuiteFeatures.AdmittedProposals` admits `canonical-tz` (section 12), so its test262 files, skipped at the base,
    are scored, and each passes in both forms.
  - `src/tests/cldr/timezones/divergences.txt`'s lines move only after ` => `, from the primary to the identifier as given:
    rewritten, removed where Node's retained line already resolves the identifier to itself, or added; the first
    group's header gives decision D15's reason, quoting the old one. These are the lines part B's class Z8 names.
- **The guard's literal** (row Z8; `ee0fd1f`, its note `044dc40`). The slice compiler's guard
  `temporal/t1/data-without-the-tz-release-reads-its-zones` formats a zone its program names as `'asia/calcutta'`,
  so its output and the literal `IntlChecks.cs` compares it with moved from `-04:00 3 Asia/Kolkata` to
  `-04:00 3 Asia/Calcutta`. Its verdict passes on both sides, and what it holds does not move. Neither decision D15
  nor part B's class Z8 foresaw it. **The owner's decision of 2026-11-04**, taken as recommended under the direction
  of 2026-10-18, admits it by a dated revision of the rule filed before any run it judges,
  [`decision-rule-area-revised-2026-11-04.md`](../../../../docs/evidence/sjl-9-001/decision-rule-area-revised-2026-11-04.md)
  (`0c3d982`), which adds this one guard's literal and output, moving from the primary to the identifier as given,
  its verdict passing at both commits, to part B's class Z8, and nothing else; JSD-0082 carries a dated note on
  decision D15 quoting what it extends, and the bundle's README names the revision. **The alternative not taken:**
  leave the rule as written and record the move as a finding against decision D15 and class Z8; part B as filed could
  then have been read as holding the guard's output unchanged, and so as reading a correction this family decided,
  with a verdict that does not move, as a regression. Part B compares each check's verdict, and the trees under
  `src/tests/cldr/` and `src/tests/temporal/` byte for byte, and this guard's verdict is ok at both commits and
  `IntlChecks.cs` is outside both trees, so the rule as filed may also be read as never seeing the move; the
  revision only narrows the rule either way, and its premise, that part B as filed "reads its output as a line that
  moves outside Z8 and L18", is to be read as that possible reading. Its class text's "names that identifier as
  given" is decision D15's "as given, case-normalised": the program gives `'asia/calcutta'`, and the output names
  `Asia/Calcutta`. The revision's file, a rule, is not edited for either; this paragraph says how it is read.

### 5. Same answers, a different reading: what only an embedder's data can show

Every shipped answer is unchanged by these, and no probe moved; an embedder that hands the profile its own
`IJsIntlData` can see a difference. `JsIntlTables.FormatData`'s remarks record them, with dated notes (`4303ceb`,
`044dc40`).
- **The chain reads (decision D8).** The number, list, relative-time, display-name, date-time and duration formats
  read a locale, then its truncations, then `und`, where this family read the data locale's language alone: the
  decimal symbol, the zone naming's region names, and `symbols.timeSeparator` and `duration.hms` among them (row
  U13). Data with region-locale lines is now read where it was ignored. Plural rules still read by language.
- **Hour cycles (row T10).** `hourCycle`'s 12-hour and 24-hour cycles come from CLDR's allowed formats: an embedder's
  `ja` resolves `h11`, where this family gave `h12`.
- **Numbering systems (rows T37 and U16, decisions D13 and D21).** The date-time and duration formats put the
  locale's default numbering system first: an embedder's locale whose default is `arab` gives `arab`.
- **`getNumberingSystems` (row L22).** It reads the data's default, `latn` in this family's tables, so its answer is
  unchanged because of the data, not the code.
- **The compact exponent (row P4, decision D20).** In compact notation PluralRules passes the compact exponent, code
  no input of this family's data reaches.

### 6. Charging (decision D18)

**Every library call takes `engine.WorkCharge`, and the charges are this family's, in its units.** Tag parsing and
canonicalisation charge nothing (row L43), so the display names and the time-zone resolution pass no meter. The
relative-time adapter charges what the nested NumberFormat and PluralRules constructors charged through
CanonicalizeLocaleList. The engine-side charges stay where decision D18 names them, and its citations have moved with the
code *(this list revised 2026-11-04, after review: it gave the first three moves below, and said that "the
formatter's length charge and PartitionDurationFormatPattern's per-unit charge are the library's")*:
- CanonicalizeLocaleList's and FilterLocales' per-element charges are at `VM/JsRealm.Intl.cs:346` and `:484` (cited
  `:282` and `:550`);
- the `formatToParts` per-part charges are at `VM/JsRealm.DateTimeFormat.cs:388` (cited `:460`),
  `VM/JsRealm.ListFormat.cs:99` (cited `:94`), `VM/JsRealm.NumberFormat.cs:292` (cited `:216`) and
  `VM/JsRealm.RelativeTimeFormat.cs:115` (cited `:106`); `VM/JsRealm.DurationFormat.cs:124` still holds its;
- StringListFromIterable's per-element charge is still at `VM/JsRealm.ListFormat.cs:179`.

The charges decision D18 names in this family's deleted code are the library's now, at the same points and in the
same units: the mathematical value of a String and of a BigInt (cited `VM/JsDecimal.cs:215` and `:198-204`),
FormatNumericToString (cited `VM/JsNumberFormatter.cs:821`), CreatePartsFromList (cited
`VM/JsRealm.ListFormat.cs:217`, a line the file no longer has), the date formatter's length charge (cited
`VM/JsDateTimeFormatter.cs:298` and `:316`) and PartitionDurationFormatPattern's per-unit charge (cited
`VM/JsRealm.DurationFormat.cs:597`). The correspondence's
charge probes of rows M9, N16, T40 and U19, and the branch's own, complete at their base figures and run out one
below, at every commit of the branch (section 9). **No C2.**

### 7. Messages

**No message moved.** Commit `c0066b8` changed `Intl.DisplayNames.prototype.of`'s `RangeError` for a `dateTimeField`
code to the option-value wording, and `4303ceb` restored "invalid date-time field code: …"; a probe of all six types
gives the base's name and message for each.

### 8. Deleted, and rule I5 (decision D24)

The code JSD-0082's X6 lists for this family is deleted, and every type and member among it is registered in rule
I5's `retired-members.json` (`69f9e6a`), each with what in `Broiler.ECMAScript.Intl` replaced it. The rows were taken
from the assurance manifests at the base and at the head: every unit of the family that the head no longer declares,
registered by type where its type went whole and by member where its type stays.
- **Whole files:** `JsLocaleTag.cs`, `JsDecimal.cs`, `JsPluralRule.cs`, `JsNumberData.cs`, `JsNumberFormatter.cs`,
  `JsDateData.cs`, `JsDatePatternGenerator.cs`, `JsDateIntervalFormat.cs`, `JsDateTimeFormatter.cs`, `JsZoneNames.cs`
  and `JsLocaleInfo.cs`, with every type in them, among them decision D24's helper types.
- **From the types that stay:** every `JsIntlTables` decoder and its lazy; the cores of `JsRealm.Intl`,
  `JsRealm.Locale`, `JsRealm.NumberFormat`, `JsRealm.PluralRules`, `JsRealm.RelativeTimeFormat`,
  `JsRealm.ListFormat`, `JsRealm.DisplayNames`, `JsRealm.DateTimeFormat` (with `JsTemporalDateFormats`) and
  `JsRealm.DurationFormat`; and the fields of the Intl objects the library's records replace.
- **The two renamed members**, `JsRealm.DateTimeComponents` and `JsRealm.DateTimeValue`, so that a member of the old
  name cannot shadow the library's type.

**What stays, and holds no Intl algorithm:** the adapters of section 3 and the seams of section 2; `NewCollator` and
`CollatorKeyData`, collation, SJL-17's; `JsRealm.Segmenter.cs`, SJL-7's. The register's comment names them, and says
that `JsLocalTime.From` and `JsZoneNames`, which SJL-6.4's and SJL-8.5's paragraphs name as adapters that stay, went
with their types. Rule I5's register row names SJL-9.4 and this record, dated. Rule I5's test reads `JsRealm.NumberFormat.cs` and
`JsIntlTables.cs` where it read `JsDecimal.cs`, `JsZoneNames.cs` and `JsDateTimeFormatter.cs`, each with a dated
note, and since `65ad719` it also reads the files the rest of the registered members lived in, where their adapters
live now, and the data's adapter: `JsRealm.Intl.cs`, `JsRealm.Locale.cs`, `JsRealm.PluralRules.cs`,
`JsRealm.ListFormat.cs`, `JsRealm.RelativeTimeFormat.cs`, `JsRealm.DisplayNames.cs`, `JsRealm.DurationFormat.cs`
and `JsIntlDataSet.cs`, with a dated note.

### 9. Evidence at this change

Local pre-checks, taken before each commit of the branch on binaries built at it, against base answers taken at
`908e942` before any change, with the scripts and logs kept outside the tree; they are not evidence of the rule, whose
runs are SJL-9.6's at the base and after commits. No figure of them is quoted here.
- **test262**, `test/intl402` and every `toLocale*` directory in the bytecode form: no variant moved from its base
  answer but row Z8's, the `canonical-tz` files skipped at the base, which pass in both forms. The newly scored
  variants run out at the low fuel ceilings below, as new rows; no existing row moved.
- **Fuel ceilings**, the same selection at four low ceilings: no existing row moved.
- **The slice compiler's `--checks`**: every check passes at every commit, `BigIntChecks` among them. Against the
  base, `intl/i4/locales-match-icu` reads more lines as the divergences name them (row L18),
  `tzdb/time-zone-offsets-match-icu` likewise (row Z8), and the guard's output moves as section 4 says.
- **Probes**: the correspondence's probes of rows L, M, N, P, S, T and U, and the branch's own, answer as the base did
  but on rows L18, L9 and Z8.
- **Charges**: section 6.
- **The architecture and contract tests** pass, and the solution builds in Release with no warning.
- **The vectors**, at this family's pin, corpus revision 32 (Broiler.JS `b2c2e224`), through the conformance root
  built at `69f9e6a`, compared row by row with the run bundle
  [`sjl-11-001`](../../../../docs/evidence/sjl-11-001/README.md) retains on the same pin at the base's code: every vector
  answers as there but those of rows Z8, L9 and L18. The `divergence.broiler-vm` fields of the Intl area's vectors of
  those rows no longer hold: each now matches the common expectation. The time-zone area's
  `per-engine.broiler-vm` expectations of row Z8 (class `impl-defined`, since corpus revision 21), which name the
  primary, now differ: the answer is the vector's `out`, the identifier as given, which is Broiler.JS's answer too, and
  the runner reports each as an unclassified difference and exits with failure. Decision D15 makes the per-engine Z8
  expectations one at the corpus's next revision, which SJL-9.6 takes in Broiler.JS, with the retirement of rows L9's
  and L18's fields; the corpus is not changed here. No CI job and no architecture test runs the vectors (the
  architecture tests hold the pin's integrity alone), so none fails on them; the run is kept outside the tree.

### 10. Rules and public API

- **I1** admits the library for the profile alone; **A7**'s manifest has the edge; **N37** holds the baseline to
  JS-10-015; **N38** sees one blocker for the shared row.
- **K2, K3, K4, I7**: section 1. **I3**: optionality is unchanged, and no compiler-free root carries the Intl data.
- **I2** reads the new assembly from its package: it reaches no member rules B5 to B5c list and names no culture,
  time-zone, normalization or casing API of the platform.
- **I6**: no type of the library appears in the family's public API baseline; `docs/api/public-api.txt` does not move.
- **I5**: section 8. **N27 and N28** read and regenerate `JsCldrTables.g.cs` byte for byte, as at SJL-9.2c.
- **The review record** counts the files the deletions leave, with dated paragraphs.

### 11. Records

- **Assurance.** Regenerated at each commit; every human line stays PENDING. The code that left this component's
  assurance coverage is the home's to cover, as EX-125 to EX-132 say of the earlier areas; SJL-9.6 files its
  exclusion in bundle `sjl-9-001`, where rule H2 reads a definition.
  *(Noted 2026-11-11, at SJL-9.6's close-out.)* SJL-9.6 files it as EX-134, in bundle
  [`sjl-9-001`](../../../../docs/evidence/sjl-9-001/README.md)'s section 9: the code section 8 names, which
  `Broiler.ECMAScript.Intl` computes now and the home's own assurance covers in the Broiler.JS repository. The earlier
  areas' exclusions it reads with are EX-125 and EX-127 to EX-133: the bullet's "EX-125 to EX-132" takes in EX-126,
  the Android root's closure, which is no area's, and EX-133, bundle `sjl-13-001`'s, was filed after this record. The
  bullet above is left as written.
- **Packages.** `packages.md` and `support.md` name JS-10-015 ([JSC-324](../roadmap.corrections.md#jsc-324)).
- **Support.** `support.md`'s row of proposals not provided names `canonical-tz` among the five admitted, with a dated
  note.
- **The closures** are retaken from a clean checkout of `b3fd2ba`, after every product-source change but
  `65ad719`'s, rule I5's architecture test, which no root compiles, into `docs/evidence/sjl-9-001/cutover/`; its
  `identity.txt` names `b3fd2ba` and that checkout's product-sources digest. `cutover/README.md` records a retake from
  `1fac124`, after `65ad719`, whose closures, catalogs and package identities match byte for byte, and why it is
  not committed. SJL-9.6 takes them again at the after commit (exit clause 14).

### 12. `canonical-tz`'s entry names JSD-0082

**Decision: `SuiteFeatures.AdmittedProposals["canonical-tz"]` stays `"JSD-0082"`.** SJL-9's plan's task 7.4 says the
entry carries "this change's JSD". JSD-0082 wins over the plan: its decision D15 is the record that admits the proposal
("It is this family's correction, made at SJL-9.4, which also admits `canonical-tz` to its scored suite"), and every
other entry names the record that admitted its proposal. The entry's dated note of 2026-11-03 already says that the
cut-over's record "records the admission again", which this section does; the code does not move, and the closures
taken before this record stand.

## The records stale through SJL-9's window

Part B of bundle `sjl-9-001`'s rule admits no change under `src/tests/cldr/` outside classes Z8 and L18, so three
texts the cut-over made stale stay as committed until SJL-9.6's after runs, as JSD-0082 kept the calendar group's
header of `src/tests/cldr/locales/divergences.txt`. Each is recorded here as stale, and a change after SJL-9.6's after
runs may rewrite it.
- **`src/tests/cldr/locales/README.md`**: its counts of the divergence lines and of the `collations` lines are the
  base's; row L18 added lines to both.
- **`src/tests/cldr/timezones/README.md`**: its counts of the divergence lines, and of those in the first group, are
  the base's; row Z8 rewrote, removed and added lines.
- **The third group header of `src/tests/cldr/timezones/divergences.txt`**, "The same resolution, in the lists: …",
  gives the primary's reason, which row Z8 changed for the first group alone.

*(Noted 2026-11-11, at SJL-9.6's close-out.)* Bundle `sjl-9-001`'s runs at the base and after commits are taken, so the
window is over, and the three texts are brought up to the cut-over in a change after the after commit `68cad8b`, each
with a dated note quoting what it replaces: the two READMEs' counts and reasons name rows L18's and Z8's lines, and the
third group header gives the lists' own reason, the primary identifiers, where the first group's is the name as given.
No data line, program or retained answer moves, and the slice compiler's checks answer at the change's head as before
it. Part B of the bundle's rule compares the base and the after commit alone, so it reads none of this, and its verdict
stands. The calendar group's header of `src/tests/cldr/locales/divergences.txt`, which JSD-0082 and JSD-0046 §5 record
as stale, is not among the three, and neither is the locales README's calendars item: both stay as committed, for a
later change to rewrite. The list above is left as written.

## Where the plan, JSD-0082 and the code disagreed

JSD-0082 wins over the plan in each case, and the code follows JSD-0082. None edits JSD-0082 but the note of
section 4.
1. **The corrections the pre-check admits.** The plan's local pre-check lets row Z8 alone move a variant; decision D32
   adds L18 and L9 at SJL-9.4. L18 moved dataset lines only and L9 nothing scored; the guard's literal is section 4's.
2. **Task 7.1's admission.** The plan lists `eng/packages/`, every lock file and a packed preview first. None applies
   since `9d0f80e` and `a8e7cfc`, and the package was published before the cut-over (decision D29). The plan's list
   leaves out the package baseline (rule N37) and the closures (rule K4), both needed for the rules to pass.
3. **ADR 0001's package-edge sentence** of 2026-10-12, "the product graph has none", was false from SJL-3.4; its
   revision of 2026-11-03 records it.
4. **The deletion lists.** The plan's 7.3 and 7.4 name fewer types than X6 (decision D24): X6 adds the helper types,
   which were deleted with their files. The plan puts `supportedValuesOf`'s core and part of `JsLocaleInfo` in 7.4;
   X6 puts the core in (a), where it went, and `JsLocaleInfo` and its decoder in (d), where they went. `2b798a7`'s
   message cites the plan for `supportedValuesOf`, which is the wrong citation. `SanctionedUnits`, which X6 does not
   name, went with the NumberFormat core it lived in.
5. **Callers.** The plan's lines had drifted: its `JsRealm.String.cs:634` is the Collator, which does not move; its
   `:848-850` were `:850-852`; `JsRealm.Segmenter.cs` needed no edit, since the adapters kept their names; its
   `JsRealm.Number.cs:142` and `JsRealm.BigInt.cs:125` were `:143` and `:127`. The number decoder's removal also
   repointed DateTimeFormat's and DurationFormat's number reads before (d) and (e), and task 7.5 needed
   `JsRealm.TemporalDuration.cs` repointed besides `JsRealm.DurationFormat.cs`.
6. **Charges decision D18 does not name.** It names no charge for RelativeTimeFormat's nested constructors; their
   charges were kept, and the probes confirm them.
7. **New rows and moved rows.** The plan's "no variant moves except Z8's" holds; the `canonical-tz` variants newly
   scored add exhausted rows at the low fuel ceilings, which are new rows, not moved ones.
8. **The notices.** A row per package breaks rule N38 by a repeated blocker identity, so the library shares the
   algorithms package's row.
9. **`SuiteFeatures`' entry**: section 12.
10. **The datasets' READMEs and one header** move with the code, and part B forbids editing them in the window: the
    section above.
11. **One commit per sub-area.** Decision D3 says SJL-9.4 is "one cut-over merge, prepared as one commit per
    sub-area". Sub-areas (a), (d) and (e) are one commit each, but (b) and the number formats and relative times
    of (c) share `9d9758a`, and (c) spans `9d9758a` and `c0066b8`. The plan's task 7.3 takes (b) and (c) as one
    task, and the files deleted whole there served both: ResolvePlural reads FormatNumericToString, which lived in
    `JsNumberFormatter.cs` beside the number formatter, over `JsDecimal.cs`'s mathematical value.
    The one merge, which is what decisions D3 and D4 rest on, holds; each commit was pre-checked before the next.

## Commit messages this record corrects

Commits are not amended; these readings replace theirs.
- **`c4beaf1`** gives the wrong cause for rule N38's failure at that commit: N38 failed on the duplicate blocker
  identity `G12-unconfirmed-broiler.vm.profile.javascript`, which that commit's own notices row repeated, and
  `4842f8d` fixed it by folding the row; N37 had nothing to do with it.
- **`c4beaf1`** calls the `graph.manifest.json` entry dated; it is not. The final tree is consistent, and the ADR's and
  the manifest's wording holds from `3d05880`, when the register named the assembly.
- **`9d9758a`** also moved `JsRealm.RelativeTimeFormat.cs` onto the library's `RelativeTimeFormatter`, which its
  subject does not say.
- **`c0066b8` and `4303ceb`** changed DisplayNames' `dateTimeField` message and changed it back (section 7); no
  record needs a row for it.

## Correction entries

[JSC-324](../roadmap.corrections.md#jsc-324): the package baseline (`packages.md`, `support.md` section 9), the
Unicode package's version in `packages.md`, `support.md`'s proposals row, ADR 0001's package edge,
`eng/packages/README.md`, the composition register and the programme's documents that named the deleted types. No
other plan document of this profile says what changed.

## Settling condition

Settled in the change that files it. **This record is reopened only by a new dated record.**

## What this record does not decide

- **Whether SJL-9 is met.** Its evidence is bundle `sjl-9-001`, read at SJL-9.6 under its rule as revised on
  2026-11-04 (section 4), and the ledger records the result.
- **Broiler.JS's cut-over**, SJL-9.5, which its own changelog records with its corrections (decision D25).
- **The vectors' per-engine Z8, L9 and L18 expectations**, which the corpus's next revision retires (decision D15;
  SJL-9.6).
  *(Noted 2026-11-11, at SJL-9.6, when this family's vectors pin moved to Broiler.JS's corpus revision 38. This bullet's
  "which the corpus's next revision retires", and section 9's "Decision D15 makes the per-engine Z8 expectations one at
  the corpus's next revision, which SJL-9.6 takes in Broiler.JS", name corpus revision 38
  (Broiler-Platform/Broiler.JS#1112, merged as `3493462a`), the revision that retires the cut-overs' Intl divergence
  fields, and not the revision after this record: the revisions between kept them. Revision 38 makes row Z8's per-engine
  expectations one and retires this family's divergence fields of rows Z8, L9 and L18, and this family's vectors pin
  moved to it in `61394d0`; JSD-0082's notes of this date on its decisions D15 and D32 record it. This reopens no
  decision of this record, and the bullet and section 9 are left as written.)*
- **Collation** (SJL-17), and **optionality of Intl code** (a later decision).
- **A move of the test262 pin, the CLDR pin or this family's tables.** None moves.

## Falsified if

- a member rule I5 registers is declared again in the family's product sources;
- an adapter of section 3 computes a locale identifier, a negotiation, a number, a plural category, a list, a relative
  time, a display name, a date-time or a duration format itself;
- a library status reaches a script as another error type or message than the base gave;
- a test262 verdict, a check, a dataset line or a charge moves at the after commit outside rows Z8, L18 and L9 and the
  guard of section 4, without a dated decision admitting it;
- this family's tables, regenerated by rule N28, differ from the committed ones by a byte.
