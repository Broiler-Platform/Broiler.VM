# CLDR data pin

**Owner:** the profile built-ins owner named in JSD-0005, as the holder of the ledger's Unicode and
locale data row. **Reviewer:** none.

The Unicode Common Locale Data Repository files the JavaScript profile's `Intl` data will be
generated from, and the CLDR licence text, retrieved once, hashed, and archived here unmodified.
[`cldr.pin`](cldr.pin) records each file's length and SHA-256; decision
[JSD-0027](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0027-intl-scope-and-data-strategy.md)
section 5 is the design this follows, and its recommendation (a) - generate in-tree from pinned CLDR
JSON - is the starting point phase F7 takes.

## What is here

| | |
|---|---|
| Version | **CLDR 48.2.0**, whose collation data states UCA 17.0.0 - the Unicode version [`../../unicode/pins/`](../../unicode/pins/README.md) pins, so collation, normalization and casing read one UCD |
| JSON | the npm packages `cldr-core` and `cldr-bcp47` at 48.2.0, each tarball checked against the registry's sha512 integrity before it was unpacked; under [`cldr-48.2.0/json/`](cldr-48.2.0/json) at their package paths: `availableLocales.json`, `defaultContent.json`, `package.json`, and from `supplemental/` the likely subtags, the aliases and the parent locales; and every `bcp47/` file |
| Numbers, currencies, units, plurals | added 2026-10-04 for slice I2 (`Intl.NumberFormat`): the npm packages `cldr-numbers-full` and `cldr-units-full` at 48.2.0, integrity-checked the same way, under [`cldr-48.2.0/json/`](cldr-48.2.0/json): `numbers.json` for `de`, `en` and `und`, `currencies.json` for `de` and `en`, `units.json` for `de` and `en`, and both `package.json` files; and from `cldr-core`'s `supplemental/`, `currencyData.json`, `numberingSystems.json`, `plurals.json` and `pluralRanges.json` |
| Dates | added 2026-10-04 for slice I3 (`Intl.DateTimeFormat`): the npm package `cldr-dates-full` at 48.2.0, integrity-checked the same way, under [`cldr-48.2.0/json/`](cldr-48.2.0/json): `ca-gregorian.json`, `dateFields.json` and `timeZoneNames.json` for `de` and `en` and its `package.json`; and from `cldr-core`'s `supplemental/`, `timeData.json` and `dayPeriods.json` |
| Locale information, ordinals | added 2026-10-04 for slice I4 (`Intl.Locale` and `Intl.PluralRules`): from `cldr-core`, `scriptMetadata.json`, `supplemental/weekData.json` and `supplemental/ordinals.json` |
| Lists | added 2026-10-04 for slice I4 (`Intl.ListFormat`): the npm package `cldr-misc-full` at 48.2.0, integrity-checked the same way, under [`cldr-48.2.0/json/`](cldr-48.2.0/json): `listPatterns.json` for `de` and `en` and its `package.json` |
| Collation | from the CLDR repository at the tag `release-48-2`, under [`cldr-48.2.0/common/`](cldr-48.2.0/common) at their repository paths: `uca/allkeys_CLDR.txt`, the root collation in DUCET form as CLDR modifies it, and `collation/root.xml`, `de.xml` and `en.xml` |
| Test-only | `uca/CollationTest_CLDR_NON_IGNORABLE_SHORT.txt` and `uca/CollationTest_CLDR_SHIFTED_SHORT.txt`: no table is generated from them; they are the conformance input JSD-0027's slice I1 names |
| Licence | [`cldr-LICENSE.txt`](cldr-LICENSE.txt), the repository's `LICENSE` at that tag: the Unicode License v3 |
| Retrieved | 2026-10-04, **twice**, into two directories; `diff -r` found them byte-identical, and the first copy is the one archived. The slice I2 files were retrieved twice the same day, separately, and compared the same way |

The retrieval was performed by Claude on 2026-10-04. The repository owner asked that day for the
roadmap's phases to be continued; phase F7 begins with this archive, and JSD-0027's recommendation
names the data source and the licence. No permission specific to this retrieval was given, and that
is recorded here rather than implied. Nobody has signed anything.

## Why here

**Nothing but a test reads these files**, as for the UCD: a generator will live in the architecture
test project and write tables a build compiles, so a build needs neither these files nor a network.
They sit in the same shape as [`../../unicode/pins/`](../../unicode/pins/README.md): a `pins`
directory under `src/tests/<subject>/`, a pin file, a licence beside it, under no product project
directory and named by no project file.

**Every file is declared `binary` in `.gitattributes`**, so no checkout filter rewrites a line ending
under a recorded hash. The pin and this README stay text: nothing hashes them.

## How the pin is enforced

Rule **N27** (`CldrPinRuleTests` in the architecture suite), on every run:

- hashes every file the pin names and compares length and SHA-256;
- refuses a supplemental JSON file whose `_cldrVersion` is not `48`, a `package.json` whose version
  is not `48.2.0`, and a UCA file whose header does not state `# UCA Version: 17.0.0`; the `bcp47`
  files and the collation XML state no version and are held by their hashes alone;
- lists this directory and requires the pin to name every file in it and nothing else.

**Re-pinning** to a later CLDR release is a recorded decision, not an update, and moves together with
the Unicode pin when the UCA version moves.

## Licence

The files are © Unicode, Inc., distributed under the Unicode License v3 (SPDX `Unicode-3.0`); the text
is retained here as [`cldr-LICENSE.txt`](cldr-LICENSE.txt). **The files are unmodified**, which their
hashes attest. When tables derived from them first ship in a product assembly, the entry for them
joins [`THIRD_PARTY_NOTICES.md`](../../../../THIRD_PARTY_NOTICES.md), as the UCD's did.
