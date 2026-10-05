<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0043 - Intl's data boundary, the locale core and the collator

**Status:** Proposed, 2026-10-04. **Owner decision pending.** The code this record describes is in
the tree. It is the second half of slice I0 and all of slice I1 of
[JSD-0027](0027-intl-scope-and-data-strategy.md) section 7: the generator and the data assembly, the
identity `broiler.javascript.intl`, the locale core, `Intl.Collator`, and a `localeCompare` that
routes through it. With them, `Intl` leaves the ledger's `absent-globals` block.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the first implementation record of phase F7 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile).

---

## 1. The data boundary

- **The data is an assembly of its own**: `Broiler.VM.Profile.JavaScript.Intl`, a family project
  that references `Broiler.VM.Profile.JavaScript.Format` and nothing else. It holds one generated
  file of tables and one public class, `JsCldrData`, whose single instance hands them over. ADR 0001's
  revision of 2026-10-04 authorises the project, and it packs under its assembly name (rule N4).
- **The profile reads it and does not reference it.** The format declares `IJsIntlData`, which has a
  CLDR version and a table by name (`JsIntlTable`). The profile decodes each table the first time a
  guest needs it and keeps the decoded form for every runtime its descriptor makes, as it reads
  emitted machine code through `IJsNativeEmitter`. Rule N1's reference set for the profile is
  unchanged.
- **The surface is admitted only with its data.** `broiler.javascript.intl` is in `JsSurfaces.All`,
  and owns the global `Intl`. A door that names it without data is refused when the descriptor is
  built, with an `ArgumentException` naming `IJsIntlData`. The one door that takes data is new:
  `JavaScriptProfile.DescriptorComposing(JsComposition)`, which takes every choice the narrower doors
  take one at a time, plus the data. "Every surface" means every surface the composition can build,
  so it includes `Intl` only when data is handed over. **Every existing door and every existing
  composition builds the realm it built before.**
- **Two composition roots hand the data over**: the slice compiler, whose checks and globals ledger
  read a realm admitting every surface, and the conformance harness, which runs `test/intl402`.
  The command-line host, the execution-only and Android roots and the polyglot host do not
  reference the assembly, so their closures carry none of it, which JSD-0027 section 5 item 3 asks
  for. The two roots' retained closure listings and register rows name it.
- **Where `Intl` is not built, nothing changes**: `localeCompare` and the locale-named case methods
  keep the fixed answers JSD-0027 section 1 states.

## 2. The tables, and how they are made

`CldrTableGenerator` (architecture tests, not shipped) writes
`src/Broiler.VM.Profile.JavaScript.Intl/JsCldrTables.g.cs` from the files `cldr.pin` names, which rule
N27 holds, and the UCD files `unicode.pin` names, which rule N22 holds. Rule N28 regenerates the file
and compares it byte for byte. Each table member carries the per-unit exemption JSD-0031 chose for
the UCD tables, with the generator's own reason.

| Table | From | Form |
|---|---|---|
| `LikelySubtags` | `likelySubtags.json` | text: from, to |
| `Aliases` | `aliases.json`: language, script, region, variant and subdivision | text: kind, from, replacement |
| `Extensions` | every `bcp47/*.json`: keys, types, a deprecated type's preferred one, and the aliases that are well-formed types | text: extension, key, type, preferred |
| `Locales` | section 3 | text |
| `SoftDotted` | the UCD's `Soft_Dotted` ranges | text: first, last |
| `CollationRoot` | `allkeys_CLDR.txt`, single mappings folded into runs; the siniform and unified-ideograph implicit-weight ranges | binary |
| `CollationTailorings` | `root.xml`'s `search`, `de.xml`'s `phonebook` and `search`, built by the generator's rule engine | binary, weights scaled by 256 |

- **Tailored weights are made by scaling, not by renumbering.** Every root weight is multiplied by
  256, so a tailored weight is its predecessor's plus one and sorts between two root weights without
  moving either. The rule engine implements the syntax the three tailorings use: resets, `[last
  primary ignorable]`, the three relations and `=`, quoting and escapes, `[import]`,
  `[suppressContractions]` and `[normalization on]`. Any other syntax is refused when the generator
  runs, not skipped.
- **A tailored element states its case.** Its tertiary weight sits between two root weights that
  may disagree about case, so it cannot carry the case the way a root weight does. The generator
  states the case in spare bits of the element, reading it from the tailored string's own root
  elements, as ICU's `CollationBuilder::setCaseBits` does. Without this, `caseFirst: 'upper'` put `ä`
  before `Ä` under German phonebook order, which the ICU comparison in section 6 found.
- **Soft_Dotted is here, not with the UCD tables.** Only Lithuanian's casing reads it, and only where
  `Intl` is built. Rule N22 holds the UCD tables to their count of 30 and to the owner's 300 KiB cap,
  and this record changes neither.
- **The siniform ideographs' implicit-weight bases** are the six `@implicitweights` lines of UCA
  17.0.0's `allkeys.txt`, which `allkeys_CLDR.txt` leaves out. They are quoted in the generator, not
  archived, and the archived CollationTest files are what hold them.

## 3. The locales and the locale core

- **The supported locales are JSD-0027 section 5 item 4's list**: `de`, `de-DE`, `en` and `en-US`.
  `und` is in the data as the root and is not offered as a locale. **The default locale is `en-US`**,
  fixed, never the process culture.
- **Canonicalization is UTS #35's**, as ECMA-402's `CanonicalizeUnicodeLocaleId` requires. This
  covers structural validity, the alias rules (a language alias by its rank, with variant pairs; then
  script, region and variant aliases, a region alias choosing by the likely subtags), variants in
  order, `-u-` keywords deduplicated, sorted and canonicalized by the BCP 47 data (`rg` and `sd` by
  subdivision alias, `true` written as nothing), and `-t-` fields with their `tlang` canonicalized.
- **Negotiation is the lookup matcher**, which also answers for `best fit`. ECMA-402 lets an
  implementation's best fit be its lookup. The supported `-u-` keywords are kept, and an option
  overrides the keyword it names.
- **`Intl.getCanonicalLocales`** is published with the namespace. `supportedLocalesOf` is published
  on `Intl.Collator`.

## 4. The size, and the provisional bound

**Measured on 2026-10-04: 325,646 bytes of table data**, of which 163,807 are binary (160,603 of
them the root collation) and 161,839 are text (126 KB of that is likely subtags). The Release build
of the assembly is 333,312 bytes. JSD-0027's owner decision (c) asks for a budget from this
measurement, and **no budget is set here**. Rule N28 bounds the data at **512 KiB, provisionally**,
so growth past it is a decision rather than drift. Each further locale is a line a reviewer can read:
a tailoring, if it has one, and nothing else, because the root collation and the BCP 47 data serve
every locale.

## 5. The collator

- **The Unicode Collation Algorithm over CLDR's root.** A string is normalized to NFD by the
  profile's own normalization. Its elements are found by longest match, discontiguous contractions
  included, with implicit weights for unlisted ideographs and unassigned code points. The levels are
  compared in order.
- **Options map to UCA settings.** `sensitivity` is a strength and a case level: `base` is primary,
  `accent` secondary, `case` primary with the case level, and `variant` tertiary.
  `ignorePunctuation` is the shifted alternate. `numeric` (`kn`) compares runs of decimal digits by
  value. `caseFirst` (`kf`) orders upper or lower case first on the tertiary and case levels.
  `usage: 'search'` chooses the search tailoring: German's for `de`, the root's otherwise.
  `collation` (`co`) is honoured for `phonebk` with German. Every other value resolves to the default,
  as ECMA-402 lets an unsupported one.
- **It is metered.** Comparing charges each element of both strings, and NFD charges as `normalize`
  does.
- **`localeCompare` routes through the realm's own `%Intl.Collator%`**, whatever the global binding
  now holds. `toLocaleUpperCase` and `toLocaleLowerCase` read the first requested locale and apply
  SpecialCasing.txt's language rules for Turkish, Azeri and Lithuanian before the default mapping.
  ECMA-402 keys those rules on the language, not on the supported-locale list, so they apply although
  none of the three is a collation locale here.

## 6. Validation

- **UCA's own conformance files**: `CollationTest_CLDR_NON_IGNORABLE_SHORT.txt` and
  `CollationTest_CLDR_SHIFTED_SHORT.txt`, archived under N27, compare every consecutive pair in order
  through `Intl.Collator`, under `variant` and under `ignorePunctuation`. That is two slice-compiler
  checks.
- **The retained German and English orderings**, which JSD-0027's I1 acceptance asks for, are under
  [`src/tests/cldr/orderings/`](../../../tests/cldr/orderings/README.md). They are 81 words under 27
  collators, and Node 22.22.0's ICU 77.1 answered all of them. The profile answers every line as ICU
  did. The first run differed in one line, phonebook order under `caseFirst: 'upper'`, and section 2's
  stated case is the correction.
- **test262**, against the pinned suite:
  - `test/intl402` has 4,418 variants. 312 pass, against 50 in the whole run
    [JSC-267](../roadmap.corrections.md#jsc-267) records. 262 moved to passing and none moved back.
    1,876 fail, almost all of them needing a constructor of a later slice, and 2,230 are skipped
    as proposals (`Temporal` among them).
  - `Collator`: 124 of 130 pass. The six failing variants are three files: Thai's default
    `ignorePunctuation`, a `collation: 'eor'` option resolving against a `-u-co-phonebk` keyword,
    and `this-value-ignored.js`, which needs `NumberFormat`.
  - `Intl/getCanonicalLocales`: 74 of 76 pass. The failing file needs `Intl.Locale`.
  - `String/prototype/localeCompare`, `toLocaleLowerCase` and `toLocaleUpperCase`: all 38 pass.
  - The 40 failing variants of the files at the tree's root pass their `Collator` half and fail at
    `NumberFormat`.
  - `test/built-ins/String`: 2,439 pass and 2 are skipped, exactly as before.
- **Five more slice-compiler checks**: a door handed no data builds no `Intl`; a door naming it
  without data is refused; German phonebook and search orders; alias replacement; and the ICU
  orderings above.

## 7. What is not done

- **`Intl.NumberFormat` (I2), `Intl.DateTimeFormat` (I3), `Intl.Locale` and the remaining
  constructors** are later slices. Tests that need them fail as `Intl` lacking the member, and that
  failure is scored.
- **`Intl.supportedValuesOf`** arrives with I2 and I3, which add the values it would list.
- **Collations the data does not carry**: `eor` needs the `[reorder]` syntax and the European
  ordering rules. Thai's default `ignorePunctuation` needs Thai's tailoring and a locale the list does
  not have. Each is a line of section 4's size.
- **The composition images do not carry the notice.** `THIRD_PARTY_NOTICES.md` records this as owed,
  as it does for the UCD.

## 8. What would falsify this

- A composition that does not admit `broiler.javascript.intl` whose closure contains the Intl
  assembly, or whose realm has `Intl`.
- Two canonically equivalent strings that some collator calls unequal, or a CollationTest line out of
  order.
- A guest-visible answer that depends on the host's culture or time zone.
- A comparison or canonicalization whose work is not charged, or that allocates in proportion to
  CLDR rather than to its input.
- `JsCldrTables.g.cs` differing from what the generator writes from the two pinned archives.
