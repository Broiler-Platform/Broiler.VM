# Calendar data pin

**Owner:** the profile built-ins owner named in JSD-0005, as the holder of the ledger's Unicode and
locale data row. **Reviewer:** none.

The two published crates the JavaScript profile's calendar tables are generated from, retrieved
once, hashed, and archived here unmodified. [`calendars.pin`](calendars.pin) records each crate's
length, SHA-256 and SHA-512 and each member a generator reads; decision
[JSD-0056](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0056-temporal-in-the-cldr-calendars.md)
is the design this follows.

## What is here

| | |
|---|---|
| `icu_calendar` 2.3.0 | ICU4X's calendar crate, as crates.io serves it, under the Unicode License v3. The generator reads four of its files: the Chinese years 1912 to 2102 (`china_data.rs`), the Korean years 1912 to 2102 (`korea_data.rs`), the Qing-era years 1900 to 1911 both calendars share (`qing_data.rs`), and the Umm al-Qura years 1300 to 1600 AH (`ummalqura_data.rs`). ICU4X states that its Chinese years agree with the Purple Mountain Observatory's and the Hong Kong Observatory's, its Korean years with KASI's to 2050, and its Umm al-Qura years with ICU4C's KACST table |
| `calendrical_calculations` 0.2.4 | its dependency, under the Apache License 2.0. The generator reads one array of `persian.rs`: the 78 years from 1502 AP that the Persian calendar's 33-year rule makes leap and the astronomical calendar does not |
| Versions | each crate's `Cargo.toml`, which states its name and version; the pin names both |
| Licences | each crate's `LICENSE`, inside the crate; `THIRD_PARTY_NOTICES.md` carries both |
| Retrieved | 2026-10-05, **twice**, into two directories; `cmp` found them byte-identical, and the first copies are archived. Each crate's SHA-256 is also the `cksum` the crates.io index states for its version |

The retrieval was performed by Claude on 2026-10-05. The repository owner asked that day for phase
F8's slices to be continued; slice T3 begins with this archive, and JSD-0056 names the data source
and the licences. No permission specific to this retrieval was given, and that is recorded here
rather than implied. Nobody has signed anything.

## Why here

**Nothing but a test reads these files**, as for tzdb: a generator lives in the architecture test
project and writes a table a build compiles, so a build needs neither these files nor a network.
They sit in the same shape as [`../../tzdb/pins/`](../../tzdb/pins/README.md).

**Both crates are declared `binary` in `.gitattributes`**, so no checkout filter rewrites a byte
under a recorded hash.

## How the pin is enforced

Rule **N31** (`CalendarRuleTests` in the architecture suite), on every run:

- hashes each crate and compares its length, SHA-256 and SHA-512, then hashes every member the pin
  names after extraction;
- refuses a crate whose `Cargo.toml` states another name or version than the pinned one;
- lists this directory and requires the pin to name every file in it and nothing else.

Rule **N32** regenerates `JsCalendarTables.g.cs` in the Intl data assembly from those members and
compares it byte for byte.

**Re-pinning** to a later crate release is a recorded decision, not an update.
