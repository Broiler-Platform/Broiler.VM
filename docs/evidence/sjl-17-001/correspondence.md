<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# SJL-17.1's correspondence from code: collation in both engines

**Written:** 2026-11-01, at SJL-17.1 (work package SJL-17.1, task 17.1a), as the input of the area verdict
[JSD-0084](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0084-collation-moves-to-the-shared-intl-library.md)
and of the two `sjl-17-001` rules, before any shared collation member or cut-over exists. The rows are lettered **O**
(orderings), as SJL-17's plan names them, and cover what the plan lists (root order, the tailorings, the four options,
lone surrogates, host dependence, invariant globalization) and what the code showed besides: the conformance files,
Han across blocks, the option reads and refusals, the resolved options and the collations, the bound `compare`,
`localeCompare`'s routing, the available locales, the meter, the test262 population and the `orderings` dataset.
**Every behavioural cell was run in both engines**, and **each row carries its class**, one of the seven bundle
`sjl-9-001`'s correspondence uses, named below; no row's class is unknown. A class that rests on a reading of the
pinned text or of CLDR's data stands until SJL-17.2's vectors check that reading; a reading they refute is a finding
against the area verdict, and the row is reclassified there.

**The classes** (bundle `sjl-9-001`'s, unchanged):
- **Same in both:** the two engines give the same answer.
- **Correction of Broiler.JS:** Broiler.JS's answer departs from the pinned text and Broiler.VM's does not; the shared
  collator follows the text, and Broiler.JS's answer moves at SJL-17.5.
- **Correction of Broiler.VM:** the reverse; Broiler.VM's answer moves at SJL-17.4. **No row is in this class.**
- **Implementation-defined:** the pinned text leaves the answer to the implementation; the shared collator keeps one
  answer, Broiler.VM's unless the row says otherwise.
- **Data policy (C5):** the answers differ by collation data: each engine's locale list, CLDR 48.2.0's root and
  tailorings against the host's ICU.
- **Engine policy (a parameter):** a policy each engine keeps through a parameter of the shared code or in its
  adapter: the default locale, the available locales, the meter.
- **Code no input reaches:** the engines' code differs where no input can reach it.

**X1 is an adoption** (the plan's decision E2): Broiler.JS implements no Unicode Collation Algorithm. Its `Intl.Collator`
hands the strings to the host's `CompareInfo` after Broiler.Unicode's NFD (`BI/Intl/JSIntl.cs:6204-6251`). The shared
`UcaCollator` is Broiler.VM's `JsCollator` and `JsCollationData`, which Broiler.JS adopts for the locales the shared data
covers; nothing of Broiler.JS's moves into the library but its option record's shape, which both engines already share.

**Read and run at**
- **Broiler.VM `83290f9a56dbce0d538509dd77ab8dee6ea00f41`**, the default branch (Broiler-Platform/Broiler.VM#178),
  whose collation code is SJL-9's rule commit's: no file of `JsCollator.cs`, `JsCollationData.cs`, `JsRealm.Intl.cs`'s
  Collator, `JsRealm.String.cs`'s `localeCompare` or `src/tests/cldr/orderings/` changed since bundle `sjl-9-001`'s
  correspondence read `cd1928d` (`git diff --stat cd1928d 83290f9` over those paths is empty).
- **Broiler.JS `61cb15b5472955079f3b0bf98c600f4128094614`**, the default branch after Broiler-Platform/Broiler.JS#1085
  (SJL-9.3b and SJL-13.2a), which holds SJL-9.3a's `IIntlDataSet`, `ILocaleData` and the locale core
  (`Broiler.ECMAScript/Broiler.ECMAScript.Intl/`), SJL-17's start condition (the plan's decision E1), with the packages
  its `Directory.Packages.props` restores, `Broiler.Unicode.Algorithms` `0.1.0-preview.6` among them.
- **test262 `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`**, read only.
- **A witness, never the arbiter of a class:** Node `v26.12.0` (ICU 78.3, CLDR 48.0, Unicode 17.0), the reference
  Node.js release Broiler.JS's bundle `sjl-9-001`'s `reference.txt` names. Its tarball was retrieved once into its own
  directory and its SHA-256 equals the digest that file records (`fd47865e...`); only its `bin/node` was extracted.

**How each column was run.**
- **The builds.** Each engine was built once, in Release with `--disable-build-servers`, in a scratch worktree of its
  own detached at the commit above (`/home/user/vm-s17p`, `/home/user/js-s17p`, the paths the run files' commands name),
  one build at a time, with `PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet`: Broiler.VM's conformance composition
  root (`src/compositions/Broiler.VM.Composition.JavaScript.Conformance`) and Broiler.JS's shell
  (`Broiler.JS/Broiler.JavaScript`, `BroilerJS.dll`). The worktrees and their `bin/` and `obj/` were removed after.
- **A Broiler.VM cell** was run as bundle `sjl-9-001` ran it: the probe as a raw test262 test through the conformance
  root, under the manifest the root scores with, fuel 200,000,000,000 unless the probe's first line names an allowance,
  wall clock 600,000 ms, `TZ=UTC`, no `LANG`, `LC_ALL` or `LANGUAGE`, **once in the bytecode form and once in the native
  form** (`--form native`, x86-64-sysv) ([`runs/run-vm.sh`](runs/run-vm.sh)). **Every probe's native lines are
  byte-identical to its bytecode lines, and both forms give every test262 variant the same verdict**, so no row differs
  by form. The command-line root composes no Intl, as at SJL-9.1, and answers no row.
- **A Broiler.JS cell** was run through the shell, `BroilerJS.dll --script-host`, `TZ=UTC`, no `LANG`, `LC_ALL` or
  `LANGUAGE`, so .NET's current culture is the invariant culture and ICU is loaded ([`runs/run-js.sh`](runs/run-js.sh));
  **every probe again under `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1`** (`*.bjs.invariant.txt`), and the rows that read
  the default locale under `LANG=LC_ALL=de_DE.UTF-8` and `tr_TR.UTF-8` (`*.bjs.de_DE.txt`, `*.bjs.tr_TR.txt`). test262
  files were run through Broiler.JS's own runner, `scripts/compliance/run_test262.py`, with `--include-negative`, normal
  and invariant ([`runs/run-js-t262.sh`](runs/run-js-t262.sh)); Broiler.VM's through the conformance root in both forms
  ([`runs/run-vm-t262.sh`](runs/run-vm-t262.sh)).
- **The witness** ran the same built programs ([`runs/run-witness.sh`](runs/run-witness.sh)).
- **The machine** for every run: Ubuntu 24.04.5 LTS, 4 processors, .NET SDK 10.0.401, ICU 74 (`libicuuc.so.74`), which .NET
  loads when globalization is not invariant.
- **How a value is printed:** bundle `sjl-9-001`'s `probes/prelude.js` and `epilogue.js`, unchanged but for a first line
  naming this bundle ([`probes/lib/`](probes/lib/)). A sort prints the words in order with `<` or `=` between neighbours,
  as `src/tests/cldr/orderings/orderings.js` does.
- *(reading)* marks a cell read from code and not executed.

**References.** "The specification" is ECMA-402 at **`tc39/ecma402` `3d4c558180f643d7dea75f011a6355fb285f377e`**, as
JSD-0082's decision D14 pins it: `spec/collator.html` (the Collator constructor's reads, ResolveLocale over
`[[SortLocaleData]]` and `[[SearchLocaleData]]`, CompareStrings) and `spec/locale-sensitive-functions.html`
(`String.prototype.localeCompare`). The collation data is CLDR 48.2.0's root collation and tailorings
(`src/tests/cldr/pins/cldr-48.2.0/common/{uca,collation}/`, rule N27) with UCA 17's implicit weights, and UTS #10 and
UTS #35 Part 5 as they read for that release. A clause is cited by its `emu-clause` id.

**Paths.** In Broiler.VM, `VM/` is `src/Broiler.VM.Profile.JavaScript/`. In Broiler.JS, `BI/` is
`Broiler.JS/Broiler.JavaScript.BuiltIns/` and `ES/` is `Broiler.ECMAScript/`. A line number is the line at the revision
above; a bare `:n` is in the file named last in the same cell.

**The runs.** [`probes/`](probes/) holds `lib/` (the prelude and epilogue), `rows-O/` (each probe as run, and `built/`,
each program as run), `scripts/` (the two generators) and `t262-files.txt` (the test262 files run); [`runs/`](runs/)
holds the scripts and `vm/`, `js/` and `witness/`, each with a `hashes.txt` that `sha256sum -c` verifies, as
[`probes/hashes.txt`](probes/hashes.txt) does for the probes.
- The two CollationTest probes, `o2-collationtest-non-ignorable.js` and `o2-collationtest-shifted.js`, are not retained:
  each holds a whole archived file, and `scripts/make_collationtest_probes.py` writes them again byte for byte from the
  archived files of this repository; [`probes/generated.sha256`](probes/generated.sha256) records the SHA-256 of each
  probe and built program as run, and the generator, run again on this commit's archived files, writes both probes
  with those digests.
- `probes/rows-O/invariant-probe.js` is Broiler.JS's bundle `sjl-17-001` `invariant-probe.js` as that bundle commits
  it, run here in Broiler.JS's shell under invariant globalization at `61cb15b5` (row O14); `o14b-invariant-probe-policy.js`
  is the same program with each request mapped to the locale Broiler.JS's policy resolves, run in Broiler.VM.
- A run file's commands name the worktrees by their paths on the machine, as run, the harness folder `<harness>/`, a
  rows folder `<rows>/` and a runner's temporary folder `<tmp>/`.
- **It holds no figure of a run in prose.** Every count of a run is in a table or a run file.

## Where each engine's code is

- **Broiler.VM.** `VM/JsCollator.cs` (292 lines): `JsCaseFirst` `:26-36`; the collator's settings `:69-94`; `Compare`
  `:100-112` (equal strings short-cut `:102-105`, the charge `:109`); `Elements` `:117-169` (NFD by
  `JsRealm.NormalizeText` `:119`, a lone surrogate kept as its own code point `:122-132`, the shifted alternate
  `:137-168`); the level comparison `:174-209` (primary, the case level, secondary, tertiary); `CaseRank` `:267-271`;
  `TertiaryWeight` `:281-291`. `VM/JsCollationData.cs` (621 lines): the decoder `:122-214`; `TailoringNamed` `:219`;
  `Case` `:244`; `Single` `:282`; `Implicit` `:316-359` (UCA 17's siniform and unified-ideograph implicit weights);
  `Elements` `:369-432` (longest match, discontiguous contractions, numeric runs); `LongestMatch` `:437`; `Valid` `:510`
  (a lone surrogate keys as U+FFFD, which no key names); `DigitValue` `:515`; `Number` `:533-560`.
  `VM/JsRealm.Intl.cs`: `SetupCollator` `:90-152` (`supportedLocalesOf` `:107-112`; the `compare` getter `:114-133`,
  whose bound function is made once `:119-130`; `resolvedOptions` `:135-148`); `NewCollator` `:168-227` (the reads in
  the specification's order `:174-197`, ResolveLocale `:189-193`, `sensitivity` and `ignorePunctuation` `:199-200`, the
  tailoring chosen by usage and collation `:202-206`, the collator `:208-215`); `CollatorKeyData` `:235-242`;
  `AvailableLocales` `:247-260`. `VM/JsRealm.String.cs`: `localeCompare` `:624-657` (its charge `:628`; through the
  realm's own `%Intl.Collator%` `:632-640`; ordinal over NFD where no Intl is built `:642`). `VM/JsRealm.Locale.cs`:
  `getCollations` `:126-151`. `VM/JsIntlTables.cs`: the decoded data `:70`, `:110`, `:162`. The tables: rule N28's
  `src/Broiler.VM.Profile.JavaScript.Intl/JsCldrTables.g.cs`, `CollationRoot` and `CollationTailorings` (`und-search`,
  `de-phonebk`, `de-search`). Their generator moved at SJL-9.2 into the data home:
  Broiler.JS `ES/Broiler.ECMAScript.Generation/Intl/CldrTableGenerator.Collation.cs` (the root and the rule engine;
  its refusals of syntax it does not implement `:641`, `:655`, `:798`, `:823`), with
  `IntlGeneratorOptions.BroilerVmTailorings` and `Tailorings` (`ES/Broiler.ECMAScript.Generation/Intl/IntlGeneratorOptions.cs:80`,
  `:92`), which this family's rules N27 and N28 run since SJL-9.2c.
- **Broiler.JS.** `BI/Intl/JSIntl.cs`: `CreateCollatorConstructor` `:678-706` (the `compare` getter `:685-699`, a new
  function on every get `:691`); `AvailableLocales` `:318-340` (the host's cultures, `CultureInfo.GetCultures` `:325`);
  `IsLocaleAvailable` `:365`; `ValidateConstructorArguments` `:711`; `SupportedCollationsSorted` `:822`;
  `CollatorRelevantKeys` `:2395`; `ResolveLocaleFromCanonical` `:2382`; `JSIntlCollator` `:6003-6328`: the constructor
  `:6014-6085` (the locale resolved first `:6016-6019`, then `usage`, `sensitivity`, `ignorePunctuation`, `numeric`,
  `caseFirst` twice and `collation` `:6035-6050`), `ResolveCompareInfo` `:6094-6123` (search `:6103-6110`, a tailoring
  by `NetCollationSuffix` `:6112-6120`, the invariant culture's `CompareInfo` when the host has none `:6121-6122`),
  `NetCollationSuffix` `:6127-6146`, `IsCollationAvailableForLocale` `:6163-6170` (`emoji` and `eor` everywhere
  `:6167-6168`), `LocaleDefaultIgnorePunctuation` `:6174-6181` (Thai hard-coded), `StripIgnorablePunctuation`
  `:6188-6202`, `Compare` `:6204-6251` (Broiler.Unicode's NFD `:6218-6219`, the `CompareOptions` mapping `:6221-6229`,
  the host's comparison `:6231`, the tie-break `:6239-6248`), `ResolvedOptionsPrototype` `:6253-6267`;
  `JSIntlLocale.GetCollationsPrototype` `:4212`. `BI/String/JSStringPrototype.Search.cs`: `LocaleCompare` `:167-184`
  (a new `JSIntlCollator` on every call). No meter. The shared library has no collation member: `ES/Broiler.ECMAScript.Intl/`
  holds the table identifiers `CollationRoot` and `CollationTailorings` (`IIntlDataSet.cs:98`, `:101`) and the
  Collator's locale data that CollationsOfLocale reads (`ICollatorLocaleData`, `LocaleInfo.cs:419-431`), and this
  engine's embedding of the home's tables (`ES/data/intl/generated/collation-root.bin`, `collation-tailorings.bin`)
  holds Broiler.VM's three tailorings, which nothing in `BI/` reads before SJL-17.5.

## The rows

| Row | Input *(probe)* | Broiler.VM (`83290f9`) | Broiler.JS (`61cb15b5`) | The specification and the data | Class |
|---|---|---|---|---|---|
| O1 | Root order across scripts under `en`: Latin with `ß`, `æ`, `ø`, `œ`, `þ`, `ð`, `ı`; Greek, Cyrillic, Georgian, Armenian, Hebrew, Arabic, Ethiopic, Devanagari, Thai, Tibetan, Mongolian, Cherokee, Hangul, kana, Yi, core Han; an emoji; digits of three scripts; `½`, `①`; and `und` against `en` *(run, `o1-root`)* | UCA over CLDR 48.2.0's root (`VM/JsCollationData.cs:369-432`): the order of the run file; `und` resolves to `en-US` and orders as `en` | the host's ICU 74 root (`:6121`, `:6231`): **the same order** but for Han across blocks (O3) | CompareStrings (`sec-collator-comparestrings`) leaves the order to the implementation; the programme's data is CLDR 48.2.0's root | **Data policy (C5):** CLDR 48.2.0's root replaces the host's ICU for the covered locales; on these inputs no answer moves |
| O2 | UCA's conformance files as the slice compiler's checks read them, every consecutive pair: `CollationTest_CLDR_NON_IGNORABLE_SHORT.txt` under `{sensitivity: 'variant'}` and `CollationTest_CLDR_SHIFTED_SHORT.txt` under `{sensitivity: 'variant', ignorePunctuation: true}`, both under `en` *(run, `o2-collationtest-non-ignorable`, `o2-collationtest-shifted`)* | **every pair in order** in both files (as the checks `intl/i1/the-root-collation-orders-collationtest-*` hold) | **pairs out of order in both files**, more in the shifted one (the table below); under invariant globalization far more (O14) | UTS #10's conformance test for CLDR 48.2.0's root (UCA 17) | **Data policy (C5):** the host's ICU 74 carries UCA 15.1's weights, and the shifted file adds O5b's symbols; the witness orders every pair |
| O3 | Han across blocks: U+4E2D against U+20000 (Extension B), U+4E59 against U+20000, U+3400 (Extension A) against U+4E2D *(run, `o1-root`; witness)* | UCA 17's implicit weights (`VM/JsCollationData.cs:316-359`): core ideographs (base `FB40`) before Extension A and B (`FB80`), so 中 < 𠀀 | 𠀀 < 中, as the host's ICU orders unified ideographs (radical and stroke) | CLDR 48.2.0's root data as this family reads it: `allkeys_CLDR.txt` with UCA 17's implicit weights. The witness (ICU 78.3) orders as Broiler.JS. The archived CollationTest files carry no pair across these blocks | **Data policy (C5)** for Broiler.JS: CLDR 48.2.0's root as Broiler.VM reads it replaces the host's ICU. **A reading for SJL-17.2's vectors to check** (finding F1): if CLDR 48.2.0's root orders unified ideographs by radical and stroke, as ICU builds it, this row is a correction of Broiler.VM that neither rule names |
| O4 | Canonical equivalence and ignorables: `å`/`a◌̊`, `Å` (U+212B)/`Å`, `Ω` (U+2126)/`Ω`, reordered combining marks (`q◌̇◌̣`, `ḋ◌̣`, a run across classes 230, 232, 222), `ä`/`a◌̈`; soft hyphen, ZWSP, NUL and ZWNBSP inside `ab`; Thai prevowel `เก`/`กเ`; a conjoining jamo pair against its syllable; compatibility forms (`ﬁ`, `ǆ`, `①`, `Ａ`, `ª`) *(run, `o1-root`)* | equivalent strings `=`; the four ignorables `=`; compatibility forms `>` their base form at the tertiary level (the run file) | **the same** on every pair: Broiler.Unicode's NFD first (`:6218-6219`), then the host | CompareStrings: canonically equivalent strings compare equal (`sec-collator-comparestrings`, note); UTS #10's completely ignorable code points | **Same in both** |
| O5a | `ignorePunctuation: false` and `true` over strings that differ by space, `_`, `-`, `.`, NBSP, U+2010, `$`, `+`, `€`, at `variant` and at `base` *(run, `o4-options`)* | `true` is UCA's shifted alternate with CLDR's `maxVariable` punctuation (`VM/JsCollator.cs:137-168`): spaces and punctuation ignored, **symbols kept**: `"a+b" < "a$b" < "a€b" < "ab" = "a b" = …` | `true` is .NET's `IgnoreSymbols` (`:6228-6229`): **symbols ignored too**: `"ab" = "a b" = … = "a$b" = "a+b" = "a€b"` | `[[IgnorePunctuation]]` (`sec-intl-collator-internal-slots`): punctuation is ignored, which CLDR's root reads as its variable range up to punctuation; symbols are not in it | **Correction of Broiler.JS** (the witness orders as Broiler.VM) |
| O5b | NBSP against space at `base` with `ignorePunctuation: false` *(run, `o4-options`)* | `"a b" = "a b"`: NBSP is space with a tertiary difference | `"a b" < "a b"`: the host's `IgnoreCase \| IgnoreNonSpace` keeps the width and compatibility difference | `sensitivity: "base"` compares base letters only; UTS #10's primary strength | **Correction of Broiler.JS** (the witness orders as Broiler.VM) |
| O5c | Thai's default `ignorePunctuation`: `new Intl.Collator('th')`, `th-u-ka-noignore` *(run, `o4-options`; test262 `Collator/prototype/resolvedOptions/ignorePunctuation-default.js`)* | `th` is not available, so `en-US` resolves and `ignorePunctuation` is `false` (`VM/JsRealm.Intl.cs:200`); the test262 file fails in both forms | `th` resolves and `ignorePunctuation` is `true` from a hard-coded language check (`:6174-6181`); `ka` is no Collator key, so the keyword changes nothing in either engine | ResolveLocale over `[[SortLocaleData]]`; the default `ignorePunctuation` is locale data (CLDR's `th` tailoring, `[alternate shifted]`) | **Data policy (C5):** each engine's locale list (JSD-0082 decision D5; this family's does not grow, rule N28); Broiler.JS's default comes from the tailoring data after SJL-17.5 (the plan's decision E5), with no answer moved |
| O6 | Sensitivity: `base`, `accent`, `case` and `variant` over `a`, `A`, `á`, `Á`, `a◌̈`, `Ä`, `b`, with `usage` `sort` and `search`; the resolved defaults of `en`, `en` search, `de` search *(run, `o4-options`)* | `base` primary, `accent` secondary, `case` primary with the case level, `variant` tertiary (`VM/JsRealm.Intl.cs:208-215`): the run file's orders; `sensitivity` resolves `variant` for both usages | **the same orders** (`:6221-6227`); the same defaults | `[[Sensitivity]]`; for `usage: "search"` the default is locale data, `variant` in both engines | **Same in both** |
| O7 | `numeric`: `a1`, `a01`, `a001`, `a2`, `a02`, `a9`, `a10`, Arabic-Indic and fullwidth digits, `1.5`/`1.10`, `9`/`10`; by option, by `-u-kn`, `-u-kn-false`, and an option that overrides the keyword; through `localeCompare` *(run, `o4-options`; `o15-orderings` lines 7, 10, 17)* | applied: runs of decimal digits compare by value, leading zeros ignored, any script's decimal digits (`VM/JsCollationData.cs:515-560`): `"a2" < "a10"` | **resolved and not applied**: `resolvedOptions().numeric` is `true`, the order is the code-unit-like digit order (`"a10" < "a2"`): `Compare` passes no option for it (`:6221-6229`) | `[[Numeric]]` (`sec-intl-collator-internal-slots`): "numeric sorting"; UTS #35's `kn` | **Correction of Broiler.JS** (the milestone's SJL-17.5 list) |
| O8 | `caseFirst`: `false`, `upper`, `lower`, with `variant` and with `case`; `-u-kf-upper`, an option over the keyword; German phonebook with `kf-upper`; through `localeCompare` *(run, `o4-options`; `o15-orderings` lines 8, 10, 16, 20, 26, 27)* | applied on the tertiary and case levels, a tailored element's case stated by the generator (`VM/JsCollator.cs:267-291`, JSD-0043 §2): `"A" < "a"` under `upper` | **resolved and not applied**: `"a" < "A"` whatever the option | `[[CaseFirst]]`; UTS #35's `kf` | **Correction of Broiler.JS** (the milestone's SJL-17.5 list) |
| O9 | Lone surrogates: `\ud800` against `\udc00`, `�`, `a`, `\udbff`, itself, a pair, U+FFFF, U+0378, U+4E00; in a longer string; with a combining mark before; at `base`; through `localeCompare` *(run, `o8-surrogates`)* | a lone surrogate is its own code point with an unassigned code point's implicit weight (`VM/JsCollator.cs:122-132`, `VM/JsCollationData.cs:316-359`), and keys no contraction (`:510`): the run file's answers | **the same answer on every pair**, at both strengths (Broiler.Unicode's NFD passes a lone surrogate through; the host's ICU weighs it as Broiler.VM does) | CompareStrings over code units; UTS #10's handling of ill-formed code units is the implementation's | **Same in both** |
| O10 | The option reads: a recording options bag through `new Intl.Collator` and `localeCompare`; `collation` values that are not a type sequence (`a`, `abc-`); bad `usage`, `sensitivity`, `caseFirst`, `localeMatcher`; `numeric: 'false'` *(run, `o9-objects`)* | `usage,localeMatcher,collation,numeric,caseFirst,sensitivity,ignorePunctuation` (`VM/JsRealm.Intl.cs:174-200`); a malformed `collation` is a `RangeError`; each bad value a `RangeError`; `'false'` is `true` (ToBoolean) | **`localeMatcher,usage,sensitivity,ignorePunctuation,numeric,caseFirst,caseFirst,collation`**: the locale is resolved before the options, `caseFirst` is read twice (`:6045-6046`), `collation` last; a malformed `collation` is **accepted** and resolves `default`; each bad value a `RangeError`; `'false'` is `true` | The Intl.Collator constructor's steps (`sec-intl.collator`, InitializeCollator): the order of the left column, one read each, and a `RangeError` for a `collation` that is not `(3*8alphanum) *("-" (3*8alphanum))` | **Correction of Broiler.JS** (the order and the double read are observable; test262's `constructor-options-throwing-getters.js` does not see them) |
| O11a | Resolved options: `de-u-co-phonebk-kn-kf-upper`, `en-u-co-phonebk`, `de-u-co-search`, `de-u-co-standard`, `de-u-co-phonebk` with `usage: 'search'`, `xx`, no locale, `new Intl.Collator('de', {collation: 'phonebk'})` *(run, `o9-objects`, `o3-tailorings`)* | the keys in the pinned order; keyword values reflected only where the locale data has them; `search` and `standard` never a collation; under `usage: 'search'` the `co` data has no `phonebk` (`VM/JsRealm.Intl.cs:235-242`), so `de` and `default` | the same, but under `usage: 'search'` **`de-u-co-phonebk` and `phonebk` are reported while the host's search collation compares** (`:6094-6123`) | ResolveLocale over `[[SearchLocaleData]]`, whose `co` values the implementation chooses (`sec-intl-collator-internal-slots`) | **Implementation-defined:** the shared collator keeps Broiler.VM's search locale data, so Broiler.JS's report follows what it compares |
| O11b | The collations: `en` with `collation: 'emoji'`, `en-u-co-eor`, `zh-u-co-pinyin`, `Intl.supportedValuesOf('collation')`, `getCollations()` of `en`, `de`, `und`, `zh`, `sv` *(run, `o9-objects`; test262 `Collator/prototype/resolvedOptions/resolved-collation-unicode-extensions-and-options.js`, `Locale/prototype/getCollations/output-array-values.js`)* | `default` for `emoji` and `eor` (its data has neither); `zh` resolves `en-US`; `["phonebk"]`; `getCollations` `[]`, `["phonebk"]`, and `["emoji", "eor"]` for `und`, `zh`, `sv` (row L18 of bundle `sjl-9-001`, which SJL-9.4 corrects); `resolved-collation-…` fails (no `eor` for `de`) | `emoji` and `eor` **resolve for every locale** and are not applied (`:6163-6170`); `pinyin` resolves for `zh`; nine collations; `getCollations` `["default"]` for each (row L17, which SJL-9.5 corrects); every test262 file passes | AvailableCanonicalCollations and CollationsOfLocale; which collations a locale has is locale data | **Data policy (C5):** each engine's collation data (rows L32 and L40 of bundle `sjl-9-001`). Broiler.JS's passing files depend on `eor` for `de` and on a collation for `en` (finding F2) |
| O12 | The bound `compare`: its identity across two gets, its name and length, detached and as a sort comparator, its coercion of arguments, the getter on a plain object *(run, `o9-objects`; test262 `Collator/prototype/compare/*`)* | **made once and kept** (`VM/JsRealm.Intl.cs:119-130`): `c.compare === c.compare` is `true`; name `""`, length 2, no `prototype`; `TypeError` on a non-collator | **a new function on every get** (`:691`): `false`; the rest the same | `get Intl.Collator.prototype.compare` (`sec-intl.collator.prototype.compare`): step 4 creates [[BoundCompare]] once and step 5 returns it | **Correction of Broiler.JS** (the plan's decision E8). test262's `bound-to-collator-instance.js` does not test identity, so no verdict moves |
| O13 | `localeCompare`: after `Intl.Collator` is replaced by a throwing function; refusals; a malformed tag; `'a'.localeCompare('B')` and `'B'.localeCompare('a')` *(run, `o9-objects`; test262 `String/prototype/localeCompare/*`)* | through the realm's own `%Intl.Collator%` (`VM/JsRealm.String.cs:632-640`): unaffected; the Collator's `RangeError`s; `<` and `>` | through a new `JSIntlCollator` (`BI/String/JSStringPrototype.Search.cs:167-184`): unaffected; the same constructors; `<` and `>` | `String.prototype.localeCompare` (`sup-String.prototype.localeCompare`): as if by a new Collator from the realm's intrinsic | **Same in both.** The coercion stays in each engine (the milestone's "Does not do") |
| O14 | Invariant globalization, Broiler.JS alone: every probe and every test262 file again under `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1`; the comparisons and sorts of Broiler.JS's bundle `sjl-17-001` `invariant-probe.js` *(run, `o14-invariant`, `o14b-invariant-probe-policy`, `invariant-probe`)* | takes nothing from the host (bundle `sjl-9-001`); with each request resolved as Broiler.JS's policy resolves it under invariant globalization, the `en` and `en-US` collators answer every line of `invariant-probe.expected.txt` (`o14b-…`, both forms) | every request resolves to `en-US` (row L44, which SJL-9.5 corrects to `en-US` and `en`), `supportedLocalesOf` answers `[]`, and **every comparison is ordinal**: `'a'.localeCompare('B')` is `>`, `ä` sorts after `z`, the conformance files fall far out of order, `numeric` and `caseFirst` change nothing, `base` folds ASCII case alone; the probe's run at `61cb15b5` differs from its expected answers on most lines; ten test262 files fail, nine of which pass with ICU loaded (`missing-unicode-ext-value-defaults-to-true.js`, `numeric-and-caseFirst.js`, `compare/ignorePunctuation.js`, `ignorePunctuation-default.js`, the three `resolved-*-unicode-extensions-and-options.js`, `supportedLocalesOf/basic.js`, `usage-de.js`), and `proto-from-ctor-realm.js`, which fails with ICU too | CompareStrings requires an order that is consistent and that treats canonically equivalent strings as equal; a code-unit order of NFD forms meets that, and the programme's area design (section 3.3) asks that no host data be needed | **Correction of Broiler.JS** (the plan's decision E4): uncovered and unavailable locales collate by UCA root or `en`, never by code unit, and Broiler.JS's rule, part C, holds it |
| O15 | Host dependence, Broiler.JS alone: the default locale and its collation under `LANG=de_DE.UTF-8` and `tr_TR.UTF-8` (`new Intl.Collator().resolvedOptions().locale`, `i` against `ı`) *(run, `o9-objects`, `o14-invariant`, `.de_DE`, `.tr_TR`)* | `en-US` whatever the host (bundle `sjl-9-001`, `runs/vm/build/host-defaults.*`) | `de-DE` and `tr-TR`; under `tr-TR`, `i` sorts after `ı` (the host's Turkish tailoring) | DefaultLocale is the implementation's (`sec-defaultlocale`) | **Engine policy (a parameter):** the default-locale provider (JSD-0082 decision D7; SJL-D-19); a covered default collates by the shared data after SJL-17.5, an uncovered one by the host provider |
| O16 | Tailorings in the data: German `phonebk` and `search`, root `search` under `en` and `und`, `es` (with `collation: 'trad'`), `sv`, `fi`, `th`, `tr`, `da`, and CJK (`zh` with `stroke`, `ja`, `ko`); `supportedLocalesOf` of each *(run, `o3-tailorings`; test262 `Collator/usage-de.js`, `compare/non-normative-phonebook.js`)* | `de` phonebook and search and root search as its data has them (`VM/JsRealm.Intl.cs:202-206`); every other locale resolves `en-US` and orders by the root; `supportedLocalesOf` keeps `de`, `de-DE`, `en`, `en-US`, `en-GB` | the host's ICU tailoring of each locale (`ñ` after `nz`, `å ä ö` after `z`, Thai shifted, pinyin, stroke, Japanese and Korean orders, Turkish dotless i); `collation: 'trad'` resolves `default` for `es`; every locale supported | ResolveLocale; the tailorings are CLDR data | **Data policy (C5):** each engine's locale list. Broiler.VM's does not grow (rule N28); Broiler.JS's covered list is the plan's decision E3's, with CJK, `tr`, `da`, `fi` and every other locale on the host provider |
| O17 | Charging *(reading)* | `Compare` charges each element of both strings (`VM/JsCollator.cs:109`), NFD as `normalize` does (`:119`), `localeCompare` the strings' length first (`VM/JsRealm.String.cs:628`) | no meter | none (X3, the programme's) | **Engine policy (a parameter):** the shared collator takes the caller's `WorkCharge` at these points, in Broiler.VM's units, and Broiler.JS passes none (the plan's decision E7); no class C2 |
| O18 | The `orderings` reference dataset: `src/tests/cldr/orderings/orderings.js` by indirect eval, against its retained answer `orderings.icu-77.1.txt` *(run, `o15-orderings`)* | **every line equals the retained answer**, in both forms (the check `intl/i1/german-and-english-orderings-match-icu`) | equal **but on the lines that ask for `numeric` or `caseFirst`** (lines 7, 8, 10, 16, 17, 20, 26, 27): O7's and O8's corrections, nothing else; under invariant globalization nearly every line differs (O14) | the dataset's README: ICU 77.1's answer; the witness answers every line as the retained answer | **Correction of Broiler.JS** (rows O7 and O8); the dataset has **no divergences file**, so no line has a divergence |
| O19 | Thai's default and its tie-break: `""` against `" "` with `ignorePunctuation: false` under `th` *(reading; test262 `Collator/prototype/compare/ignorePunctuation.js`)* | `th` resolves `en-US`; `""` < `" "` by the root | the host's Thai collation ties them, and `StripIgnorablePunctuation` breaks the tie by code units (`:6233-6248`) | `[[IgnorePunctuation]]` false: punctuation counts | **Code no input reaches** after SJL-17.5: the tie-break exists only for the host's Thai, which E3 covers; it is on the deletion list (the plan's decision E11) |

**The conformance files** (row O2), consecutive pairs out of order, from the run files:

| File | Options | Broiler.VM (both forms) | Broiler.JS | Broiler.JS, invariant | Witness |
|---|---|---|---|---|---|
| `CollationTest_CLDR_NON_IGNORABLE_SHORT.txt` | `{sensitivity: 'variant'}` | 0 of 206297 | 944 | 51752 | 0 |
| `CollationTest_CLDR_SHIFTED_SHORT.txt` | `{sensitivity: 'variant', ignorePunctuation: true}` | 0 of 227808 | 7810 | 66778 | 0 |

## The test262 population

The files run are every file under `test/intl402/Collator/`, `test/intl402/String/prototype/localeCompare/`,
`test/built-ins/String/prototype/localeCompare/` and `test/intl402/Locale/prototype/getCollations/` at `ccaac100`
([`probes/t262-files.txt`](probes/t262-files.txt)), in Broiler.VM in both forms (strict and sloppy), and in Broiler.JS in
its original variant with negative tests, normal and invariant.

| File | Broiler.VM (both forms) | Broiler.JS | Broiler.JS, invariant | Row |
|---|---|---|---|---|
| `Collator/prototype/resolvedOptions/ignorePunctuation-default.js` | fail | pass | fail | O5c, O14 |
| `Collator/prototype/resolvedOptions/resolved-collation-unicode-extensions-and-options.js` | fail | pass | fail | O11b, O14 |
| `Collator/proto-from-ctor-realm.js` | pass | fail | fail | none: the realm's, as bundle `sjl-9-001` found for the other services; outside Broiler.JS's population |
| `Locale/prototype/getCollations/*.js` (five files) | skipped (`Intl.Locale-info` not admitted) | pass | pass | O11b |
| `missing-unicode-ext-value-defaults-to-true.js`, `numeric-and-caseFirst.js`, `compare/ignorePunctuation.js`, `resolved-case-first-…`, `resolved-numeric-…`, `supportedLocalesOf/basic.js`, `usage-de.js` | pass | pass | fail | O14 |
| every other file | pass | pass | pass | |

So Broiler.JS passes every file of the population but the realm's, and no file it passes fails because of a row
classed a correction of Broiler.JS. **What SJL-17.5 must keep passing** in Broiler.JS, with ICU loaded: Thai's default
(O5c, from the tailoring data, E5), `eor` for `de` (O11b) and a collation for `en` (O11b; JSD-0082's decision D33 (c)),
and `usage-de.js` and `non-normative-phonebook.js` (O16, from the shared tailorings).

## X6: what each engine deletes

- **Broiler.VM**, at SJL-17.4: `JsCollator.cs` and `JsCollationData.cs` whole; `NewCollator`'s tailoring core
  (`VM/JsRealm.Intl.cs:202-215`, the data, the tailoring's choice and the collator's construction) and `CollatorKeyData`
  (`:235-242`), which become the adapter's calls into the library. The reads in the specification's order, the
  resolved-option slots, the bound `compare` and `localeCompare`'s coercion stay. The plan's decision E11 cites
  `JsRealm.Intl.cs:165-235` and `:240-246`; at `83290f9` the constructor is `:168-227` and `CollatorKeyData` `:235-242`
  (finding F4).
- **Broiler.JS**, at SJL-17.5: `ResolveCompareInfo` for the covered locales (`:6094-6123`); `StripIgnorablePunctuation`
  and its tie-break (`:6188-6202`, `:6239-6248`, O19); the `CompareOptions` mapping (`:6221-6229`); the hard-coded Thai
  default (`:6174-6181`, E5); `NetCollationSuffix` (`:6127-6146`) moves into the host provider. `JSIntlCollator` keeps
  every public member (SJL-D-25 (a)).

## What the rows find against the plan and the records

| Finding | What it found | Bears on |
|---|---|---|
| F1 | Han across blocks (O3): Broiler.VM orders by UCA 17's implicit weights, and the host's ICU 74 and the witness's ICU 78.3 by radical and stroke. The archived CollationTest files hold no pair that separates the two, and neither rule's part reaches it (no test262 file and no `orderings` line holds Han across blocks) | SJL-17.2's vectors (decision E9): root order across scripts from the CollationTest files and the CLDR 48 reference; a reading the vectors may refute |
| F2 | Broiler.JS passes test262 files that need collations the plan's covered data does not carry: `eor` for `de` (`resolved-collation-unicode-extensions-and-options.js`) and at least one collation for `en` (`Locale/prototype/getCollations/output-array-values.js`, JSD-0082's decision D33 (c)). The data home's rule engine refuses the syntax CLDR's `eor` and `emoji` rules use (`[reorder]`, `[before 1]`, the star relations; `CldrTableGenerator.Collation.cs:641`, `:655`, `:798`) | Decision E3 (coverage per component: a collation the shared data lacks stays on the host provider); Broiler.JS's rule, part A, names the getCollations file, as D33 (c) asks |
| F3 | The plan's rule for Broiler.JS, part B, reads the `orderings` program "with its retained answer and divergences file": the program has no divergences file at `83290f9`, so its expected answer is the retained answer, and part B's classes V and R hold no line | Broiler.JS's `decision-rule.md`, part B, and its `divergence-groups.tsv`, which holds no group |
| F4 | Decision E11's citations of `JsRealm.Intl.cs` (`:165-235`, `:240-246`) have drifted: `:168-227` and `:235-242` at `83290f9` | Decision E11, read with this section's lines |
| F5 | The plan's rule for Broiler.JS, part C, cites the available-locale policy as "JSD-0081 decision D7"; JSD-0081's decision D7 is the regular-expression nesting limit, and the policy is JSD-0082's decision D7 | Broiler.JS's `decision-rule.md`, part C |
| F6 | Under invariant globalization the policy resolves every request but `en` and `en-US` to `en-US` (O14), so the covered tailorings (`de` phonebook, `sv`, `es`, `th`) do not apply there: risk K5, as the plan foresaw it. The invariant probe's expected answers are therefore the root's, for the resolved locale each line names, and the probe's run at `61cb15b5` differs from them on most lines | Broiler.JS's `invariant-probe.expected.txt`, taken from the reference with each collator made for the resolved locale |
| F7 | JSD-0043 §5 reads "NFD charges as `normalize` does" through "the profile's own normalization"; since SJL-7.4 that normalization is Broiler.Unicode's, which decision E6 keeps | JSD-0043 §5, by a dated note |

## The rows by class

| Class | Rows |
|---|---|
| Same in both | O4, O6, O9, O13 |
| Correction of Broiler.JS | O5a, O5b, O7, O8, O10, O12, O14, O18 |
| Correction of Broiler.VM | none (O3 is a reading for the vectors, F1) |
| Implementation-defined | O11a |
| Data policy (C5) | O1, O2, O3, O5c, O11b, O16 |
| Engine policy (a parameter) | O15, O17 |
| Code no input reaches | O19 |

Every difference between the two engines' answers in the run files is in one of these rows: the probes'
side-by-side comparisons were read line by line, and each differing line is named by the row of its probe line.
