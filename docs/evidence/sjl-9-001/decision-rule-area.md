<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-9-001's area rule: locale data and the Intl algorithms, judged by test262 parity in both forms on hosted runners, by the Intl reference datasets, and by the fuel-parity twins

**Written:** 2026-10-26, at milestone SJL-9 (work package SJL-9.1), with the area verdict
[JSD-0082](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0082-locale-data-and-the-intl-algorithms-move-to-the-shared-libraries.md),
before any code of the area exists: no data-home generator, no shared Intl member and no cut-over. **Milestone
judged:** SJL-9 of
[the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-9-locale-data-and-intl.md):
its Broiler.VM clause 13 ("Whole-suite test262 in both forms, and the Intl reference datasets, meet the area rule.").
**Owner:** the JavaScript profile owner, with the core architecture owner. **Reviewer:** none. Every role this rule
names is held by one person (EX-30), and this rule does not claim that anyone independent has read it.

**What this file is.** Three predeclared decisions, one per part, each of one evidence class.
- **The templates.** Part A is in template T1 of
  [the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules),
  and part C in T7. Part B is a vector agreement in T3's sense, over the programs this family retains rather than the
  shared corpus.
- **Each part decides alone**, and no part's runs are read by another.
- **There is no measurement part.** SJL-9's exit gate has no measurement clause, and JSD-0082's decision D23 takes none,
  as SJL-8 and SJL-10 took none.
- **Other rules.** The Broiler.JS repository commits its own rule for its own engine. It is not read here.
- **It holds no figure.** Where a count or an allowance matters, it names the file that fixes it, or states it as a
  parameter of the rule.

**What earlier bundles taught this rule, before any run of SJL-9.**
- **Part A follows bundle `sjl-10-001`'s part A:** the hosted workflow for both runs. A row whose exhausted dimension
  moves on the wall clock is run again on the programme's local machine. That machine is the one bundles `sjl-5-001` to
  `sjl-10-001` were taken on, named in the bundle by its processor count, its SDK and its operating system, with no
  other build, test or run on it while a run of this bundle is taken.
- **This family's tables do not move.** Rule N28 holds `src/Broiler.VM.Profile.JavaScript.Intl/JsCldrTables.g.cs` byte
  for byte at both commits. Its CLDR release is 48.2.0, as `src/tests/cldr/pins/cldr.pin` pins it (rule N27). Its
  locales stay `de`, `de-DE`, `en` and `en-US` (JSD-0082 decision D5). Its test262 pin is `ccaac100`.
- **This family has three corrections in this area, rows Z8, L18 and L9** (JSD-0082 decisions D15 and D32). They are
  the rows the correspondence puts in its column "Correction of Broiler.VM", and SJL-9.4's cut-over makes all three.
  - **Z8.** The formatter reports a time-zone identifier as given, case-normalised, as
    `tc39/proposal-temporal@e8cc03fc`'s Intl text has it, where this family reports its primary. It still names the
    zone by that primary, as it does today (decision D15). The same change admits the `canonical-tz` proposal to the
    scored suite.
  - **L18.** `Intl.Locale.prototype.getCollations`, for a locale whose language `Intl.Collator` does not support in
    this family's data, answers DefaultLocale's collations, as CollationsOfLocale has it. Today this family answers
    `emoji` and `eor`, which the text gives only for `und`. This family's reference datasets hold its answers, so the
    change moves lines of `src/tests/cldr/locales/divergences.txt`. Part B's class L18 names them.
  - **L9.** A subdivision alias whose replacement is a region is written as UTS #35 writes it: the region in lower
    case, followed by `zzzz`. No test262 file and no line of the reference datasets reaches it.

  Any other C1 row is a finding against JSD-0082's correspondence.
- **Part B follows bundle `sjl-10-001`'s part B**, over every program that reads this family's CLDR tables. Part C is
  that bundle's part C unchanged: the command-line root carries no Intl data, so the twins reach none of the area's
  code.

**The commits, named before they exist** (JSD-0082 decision D4).
- **The rule commit** is the commit that carries this file.
- **The after commit** is the default-branch merge that lands SJL-9.4's cut-over. JSD-0082 decision D3 makes it one
  merge. It is named in bundle `sjl-9-001`.
- **The base commit** is the after commit's first parent. It must meet three conditions:
  - it descends from the rule commit;
  - it descends from the commit bundle `sjl-12-001` names as its after commit in this repository;
  - the after commit is its only first-parent successor that changes a path under `src/Broiler.VM.Profile.JavaScript*/`,
    `src/compositions/`, `eng/packages/` or `Directory.Packages.props`. If the cut-over had to land in more than one
    merge, each is SJL-9.4's and is named in bundle `sjl-9-001`, and the base is the first one's first parent.
- **Part A's base and after runs** are dispatched at those commits, at any time after each exists. **Parts B and C** are
  taken locally from a clean checkout of each.

A part whose base was taken at any other commit, or whose base commit fails those conditions, is void and is not
judged.

## Part A - test262 parity, in both forms, on hosted runners (template T1)

```rule
evidence class: conformance parity (not a measurement)
population: the whole pinned suite - test262 at revision ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e, as src/tests/conformance/pins/test262.pin pins and archives it - every variant the wide manifest scores at each commit, once in the bytecode form and once in the native form; at the base commit the proposals src/compositions/Broiler.VM.Composition.JavaScript.Conformance/SuiteFeatures.cs does not admit are skipped, and at the after commit the same less canonical-tz, which SJL-9.4 admits with row Z8's correction (JSD-0082 decision D15); test/intl402 and every toLocale* directory are in it, and a regression on any other path fails this part too
configuration: .github/workflows/test262-evidence.yml with its driver eng/test262-evidence.py, dispatched from the default branch with forms both and the tested commit's forty-digit identifier, the workflow and the driver at the dispatched ref byte-identical to the base commit's; it runs the tested commit's eng/run-test262.py at the pin, through the conformance composition root that commit builds in Release on every runner, with the allowances and floors eng/collect-js-release-candidate.py fixes for its wide-bytecode and wide-native runs, over the workflow's partition of four runners of four shards per form; the base runs are taken at the base commit and the after runs at the after commit, as this file's header defines them; each dispatch's artifact test262-evidence is imported into the bundle unchanged, with the digest of the zip as downloaded
completeness: each hosted form's run is whole and retainable, as bundle sjl-6-001's decision-rule.md states it: every shard of the partition reported, the run log's exit code (eng/run-test262.py's rule, applied by the driver) is 0, 1 or 4, the merged report's retention object says retainable, and every runner and the merging job built the same assemblies (one distinct list in the form's binaries log)
comparison: per variant, joined on path and variant, the verdict and any exhausted dimension at the base against the same after, through eng/compare-test262-forms.py --same-form, the base's report as control and the after's as candidate, once for the bytecode pair as it is and once for the native pair with --exempt-guest-loads over the extracted suite
admitted difference classes:
  C1 correction, of the kinds JSD-0082 names before any run: (Z8) a variant of a file whose features include canonical-tz, skipped at the base because the proposal was not admitted and passing after, or a variant failing at the base that passes after because the formatter reports the identifier as given; and any row JSD-0082's section "Every row, classified" puts in its column "Correction of Broiler.VM" (rows L18 and L9, decision D32, which reach no variant this part scores); each listed with its path, its variant, its row and the clause it follows; a C1 row of no named kind is recorded in the bundle as a finding that JSD-0082's correspondence missed
  C4 harness and environment - compare-test262-forms.py --same-form's own classes: (a) and (b) on either side when both runs are native, (b) only under --exempt-guest-loads, and (c') a wall-clock exhaustion on either side; each (c') row is run again alone at both commits on the local machine this file's header names, each run one process of the conformance root over the row's path under the part's manifest, form and allowances, and the re-runs are retained; a (c') row whose re-runs agree is admitted, and a (c') row whose re-runs differ is admitted only under (e)
  C4 (e) a wall-clock race - a (c') row whose re-runs differ is run alone once more at each commit on the same machine, one process of the conformance root over the row's path under the part's manifest, form and fuel allowance, with the wall-clock allowance raised to ten times the part's and no other build, test or run on the machine, and both runs are retained; the row is admitted if and only if the two runs give the same verdict, and that verdict is neither a wall-clock exhaustion nor an internal defect; a row whose two runs differ, or either of which is still exhausted on the wall clock, is a regression
no C2: the shared members and this family's adapters charge where this family charged, in the units it charged, at the points JSD-0082 decision D18 names, so an exhaustion verdict that moves on fuel is a regression, not a class
no C3 and no C5: the implementation's choices the specification leaves open move with the code unchanged, and the data does not move (rule N28), so a verdict that moves because a choice, a policy or the data moved is a regression, not a class
every other difference is a regression, including a canonical-tz variant scored after that fails, is exhausted or is unsupported, a variant passing at the base that fails, is exhausted or is unsupported after, and an internal defect on either side
MET if and only if all four runs are complete as above, every changed verdict of the bytecode pair is in C1 or C4 and is listed, and the native pair meets the same conditions.
NOT MET otherwise.
```

**How it is read.** As bundle `sjl-10-001`'s part A. The bundle retains:
- both dispatches' artifacts;
- each comparison's output, which lists every differing row with its class or as unclassified;
- a table naming each C1 row with its kind, row and clause;
- each (c') and (e) row with its re-runs.

The floor verdicts the harness prints are retained and do not decide part A. Each native run exits 4, which
completeness admits. A row the comparison cannot classify is a regression, even when the totals improve. The skipped
`Intl.NumberFormat-v3` and `Intl.Locale-info` cases stay skipped at both commits, so they move no verdict.

Rows L18 and L9 move no variant this part scores: every test262 file that calls `getCollations` claims
`Intl.Locale-info`, and no file reaches L9. Rows S7 and T8 move nothing either (JSD-0082 decision D33). Their files
are `RelativeTimeFormat/prototype/format/en-us-numeric-always.js`, and
`DateTimeFormat/prototype/resolvedOptions/hourCycle-dateStyle.js` and `hourCycle.js`. They keep their base verdicts,
and no class admits a move.

## Part B - the reference datasets and the Intl checks unchanged (vector agreement, T3's sense)

```rule
evidence class: conformance parity (not a measurement)
population: the programs this family retains as reference datasets, as committed at the base commit, each with its retained answer and its divergences file: every program under src/tests/cldr/ (dates, displaynames, durations, lists, locales, numbers, orderings, plurals, relativetimes, segments, timezones) and under src/tests/temporal/ (temporal.js, intl/temporal-intl.js, calendars/temporal-calendars.js against both its answers, calendars-intl/calendar-formats.js, zone-names/zone-names.js); the area's programs are every one but orderings (SJL-17's), segments (SJL-7's), temporal.js and temporal-calendars.js (SJL-10's), which this part holds unchanged as guards
configuration: the slice compiler's --checks, built in Release from a clean checkout, at the base commit and at the after commit, on the local machine this file's header names; the checks are every check of src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/IntlChecks.cs whose name begins intl/i0/, intl/i1/, intl/i2/, intl/i3/, intl/i4/, tzdb/ or temporal/, as the base commit names them - the area's: intl/i0/a-door-handed-no-data-builds-no-intl, intl/i0/a-door-naming-intl-without-data-is-refused, intl/i0/canonicalization-replaces-aliases, intl/i2/german-and-english-numbers-match-icu, intl/i3/german-and-english-dates-match-icu, intl/i4/locales-match-icu, intl/i4/german-and-english-plurals-match-icu, intl/i4/german-and-english-lists-match-icu, intl/i4/german-and-english-relative-times-match-icu, intl/i4/german-and-english-durations-match-icu, intl/i4/german-and-english-display-names-match-icu, intl/i4/a-variants-option-is-lowered-by-ascii-rules, tzdb/time-zone-offsets-match-icu, temporal/t2/intl-over-temporal-matches-the-reference-polyfill, temporal/t4/calendar-formats-match-the-reference-polyfill and temporal/t5/zone-names-match-the-reference-polyfill; the guards: every intl/i1/ check (collation), intl/i4/segments-match-icu and every intl/i4/the-segmenter-passes-*-break-test check, and every temporal/t1/ and temporal/t3/ check; each check's verdict and output are retained
comparison: per check, its verdict at the base against its verdict after; and per program, its text, its retained answer and its divergences file at the base against the same after, byte for byte (git diff between the two commits over src/tests/cldr/ and src/tests/temporal/)
admitted difference classes:
  Z8 the resolved identifier (JSD-0082 decision D15) - a line of a divergences file under src/tests/cldr/ or src/tests/temporal/ whose key is a time-zone identifier that is not its own primary, and whose resolved name after ` => ` changes only from that primary to the identifier as given: rewritten by SJL-9.4's change, or removed by it where the retained answer's line already resolves the identifier to itself, or added by it where the retained answer resolves the identifier to another name; and the comment lines that head those lines, rewritten to give decision D15's reason; each listed with the line at both commits and the retained answer's line; no program and no retained answer changes
  L18 the collations of a language the Collator lacks (JSD-0082 decision D32) - a line of src/tests/cldr/locales/divergences.txt whose key asks collations of a locale that carries no co keyword and no collation option, and whose language is neither und nor one Intl.Collator supports in this family's data (de, en), and whose answer changes only from [emoji,eor] to [], DefaultLocale's collations: rewritten by SJL-9.4's change where a divergence line held [emoji,eor], added by it where the retained answer's line reads [emoji,eor], or removed by it where the retained answer's line already reads []; and the comment lines that head those lines, rewritten or added to give decision D32's reading of CollationsOfLocale; each listed with the line at both commits and the retained answer's line; no program and no retained answer changes
every other difference is a regression: a check that fails at either commit, a line that moves outside Z8 and L18, a divergence added or removed outside Z8 and L18, and any change under src/tests/cldr/ or src/tests/temporal/ outside Z8 and L18
MET if and only if every check named passes at both commits, and every difference between the two commits under src/tests/cldr/ and src/tests/temporal/ is in Z8 or L18 and is listed.
NOT MET otherwise.
```

**How it is read.** A check compares every line of its program with the retained answer, after its divergences are
applied, and fails on any line that differs. So a passing check at both commits, with only Z8's and L18's lines
changed, means every line of every program is the same at both commits, but for the resolved names JSD-0082
decision D15 moves and the collations decision D32 moves. A zone's names are not among them: decision D15 has this
family name the zone by its primary, as it does at the base, so a line that moves on a zone's name is a regression.
Broiler.JS's runs of these programs are judged by its own rule.

## Part C - the fuel-parity twins (template T7)

```rule
evidence class: resource accounting (not a measurement)
population: every *.small.js twin in src/tests/forms/wide, as committed at the base commit, in the bytecode form and in the native form under the backend the host arms (x86-64-sysv on linux-x64, x86-64-win64 on win-x64)
configuration: the command-line composition root built in Release, on the local machine this file's header names; at the base commit, each twin's smallest completing fuel allowance bisected under the bytecode form, with its answer, as sjl-9-001/base/fuel-parity.log retains it; the wall allowance ample and the same at both commits
ceilings tried: for each twin, its base figure and one less
comparison: per twin and form, the verdict at each ceiling tried at the base against the verdict at the same ceiling after, and the answer it completes with
admitted difference classes: none; JSD-0082 adds no charge and moves none (decision D18), so a moved figure is a defect of a shared member or of an adapter, not a class
MET if and only if every twin, in both forms, after gives the verdict it gave at the base at each ceiling tried, and completes with the base's answer where it completes.
NOT MET otherwise.
```

**How it is read.** As bundle `sjl-10-001`'s part C. The command-line root carries no Intl data. Its twins show that
the cut-over moved no charge outside the area. Inside the area, part A's exhaustion verdicts on fuel are the charge
evidence.

## Revisions

A rule revised after any run it judges has been seen is a new dated file in this directory. It quotes this one whole
and says it was written afterwards, and its decision is then weaker evidence. **Before SJL-9.3's code lands, a later
bundle shows this file unchanged**, as SJL-3.2 and SJL-4 to SJL-10 did for theirs.

## What this file does not decide

- **The data home and the home clauses** (SJL-9's clauses 1 to 6): the pinned, hashed inputs, byte-identical
  regeneration, the public CLDR version, the agreed locale set, each sub-area under the conventions, its vectors and
  the SL rules. They are judged in the Broiler.JS repository, where the home is.
- **Broiler.JS's test262 parity and its runs of these programs.** Broiler.JS's rule judges them.
- **This family's other clauses**, each by its own mechanism, not by a comparison this file states:
  - rules I1 to I7, with I4's CLDR clause (clause 11);
  - N27 and N28 on the home's generator, the table byte for byte under its budget (clause 12);
  - every root published and run in three modes (clause 14);
  - optionality, as rule I3 shows (clause 15).

  Each is judged by the architecture tests and the release candidate's runs.
- **The vectors.** SJL-9.2 adds them, and they may falsify the area record.
- **A speed claim of any kind.** SJL-9 measures nothing. A hosted run's wall clock is not compared with a local one,
  and part A's (e) raises the wall clock so that speed decides nothing.
