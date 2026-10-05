<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0056 - Temporal in the CLDR calendars

**Status:** Proposed, 2026-10-05. **Owner decision pending.** The implementation is in the tree. It
is phase F8's slice T3, as [JSD-0054](0054-temporal-in-the-iso-and-gregorian-calendars.md) section 2
plans it: Temporal's arithmetic in the calendars beyond ISO 8601 and Gregorian, with their eras,
month codes and leap months, as the Intl era and month code proposal specifies it.

**What the owner decided.** On 2026-10-05 the repository owner chose to build Temporal slice by slice,
each slice followed by a whole test262 run, and asked that day for the slices to be continued.
Nothing here is signed beyond that choice.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. Phase F8, slice T3.

---

## 1. What is built

- **Every calendar of the proposal's Table 1**: `buddhist`, `chinese`, `coptic`, `dangi`, `ethioaa`,
  `ethiopic`, `gregory`, `hebrew`, `indian`, `islamic-civil`, `islamic-tbla`, `islamic-umalqura`,
  `iso8601`, `japanese`, `persian` and `roc`, and the aliases `ethiopic-amete-alem` and `islamicc`,
  which canonicalize to `ethioaa` and `islamic-civil` (Temporal s12.1.1 over the proposal's 1.1.1).
  `islamic` and `islamic-rgsa`, which only `Intl.DateTimeFormat` falls back from, stay RangeErrors.
- **The proposal's calendar operations** (its section 4.1) for every calendar but ISO 8601, the
  Gregorian one included: the eras of Table 2 and their arithmetic years (CalendarSupportsEra,
  CanonicalizeEraInCalendar, CalendarDateArithmeticYearForEraYear), the month codes of Table 3 and
  their constraint to a common month (IsValidMonthCodeForCalendar, ConstrainMonthCode,
  MonthCodeToOrdinal), NonISOResolveFields, NonISOCalendarDateToISO, NonISOMonthDayToISOReferenceDate
  with Table 6's Chinese and Korean reference years, NonISODateAdd, NonISODateUntil, the keys a
  `with` ignores (Japanese eras begin within a year), and the era fields every type's `from` and `with`
  read.
- **Each calendar's arithmetic**, in `JsCalendars.cs`, one class to a family:
  - **`gregory`, `buddhist`, `roc`, `japanese`**: ISO 8601's months and days, each year offset by
    Table 4's epoch; Japanese eras from Meiji 6 (1873) by the date each began, and Gregorian years
    before;
  - **`coptic`, `ethiopic`, `ethioaa`**: twelve months of 30 days and a thirteenth of five or six,
    from the Coptic and Ethiopian epochs (Calendrical Calculations, ch. 4);
  - **`indian`**: Saka years from 22 March, or 21 March in a Gregorian leap year;
  - **`persian`**: the 33-year rule, corrected from 1502 AP by 78 years that ICU4X's
    `calendrical_calculations` lists;
  - **`islamic-civil`, `islamic-tbla`**: the tabular calendar from the Friday and the Thursday
    epochs; **`islamic-umalqura`**: KACST's years 1300 to 1600 AH, the civil tabular years outside
    them, as Table 1 states;
  - **`hebrew`**: the molad of Tishrei and the four postponements (Calendrical Calculations, ch. 8);
  - **`chinese`, `dangi`**: the published years 1900 to 2102 (the Qing calendar to 1911, then China's
    and Korea's), and outside them ICU4X's approximation by mean new moons and mean solar terms in
    Beijing or Korean time, or Beijing's local mean time before 1900.
- **`monthsInYear`** reads the calendar; every other getter already read CalendarISOToDate.

## 2. The data and where it comes from

- **Two crates, archived under a new rule N31** in `src/tests/calendars/pins/`, as crates.io serves
  them: `icu_calendar` 2.3.0 (Unicode License v3) and `calendrical_calculations` 0.2.4 (Apache License
  2.0), each retrieved twice and byte-identical, each SHA-256 equal to the `cksum` the crates.io index
  states. The pin names the nine members a generator reads: the Chinese, Korean and Qing year tables,
  the Umm al-Qura table, `persian.rs`, and each crate's `Cargo.toml` and `LICENSE`.
- **A generator in the architecture tests**, `CalendarTableGenerator`, reads ICU4X's year tables as
  data - one constructor call a year - and writes `JsCalendarTables.g.cs` into the Intl data
  assembly, which **rule N32** holds byte for byte. A Chinese or Korean year is three bytes (month
  lengths, leap month, new year), an Umm al-Qura year two. The table is 1,958 bytes; **the data is
  679,058 bytes, 107,374 under the owner's budget of 768 KiB**, which now counts it.
- **The years are ICU4X's because the proposal's sources are.** Table 1 names, for the Chinese years,
  the Purple Mountain Observatory's; for the Korean, KASI's; for Umm al-Qura, KACST's; for the
  Persian, the Iranian calendar authority's. ICU4X publishes those years as source, states the
  agreement of each, and is the implementation the proposal's test262 cases were checked against.
  The approximation outside the published years is ported from the archived crate's
  `east_asian_traditional/simple.rs`, not generated; `THIRD_PARTY_NOTICES.md` carries both crates'
  licences.
- **No locale data is added.** Formatting in these calendars needs CLDR's month and era names and
  patterns, which this slice does not take (section 6).

## 3. Choices the proposal leaves to an implementation

- **Years outside the published ones.** Table 1 allows an implementation-defined approximation for
  the Chinese and Korean calendars outside 1900 to 2100 and 2050; the profile's is ICU4X's, so the
  same date answers the same in both. It anchors to the Gregorian calendar, so a year far from the
  present keeps its months near the seasons of the Gregorian one, not of the sky.
- **NonISODateUntil's loops are counted from estimates.** The proposal adds a year, a month, a week
  and a day at a time until NonISODateSurpasses says the next would pass the second date; the profile
  starts the years and months from the difference of the two dates' years and month counts and moves
  until the same condition holds, and reckons the weeks and days, which the proposal counts from the
  last regulated date, in days. The answers are the loops' wherever NonISODateSurpasses is monotonic,
  which it is in every calendar here.
- **A year far outside the limits is refused before it is computed**: no date of a year more than
  300,000 years from the epoch is within ISODateWithinLimits, so the operation throws the RangeError
  its limits would, after the proposal's TypeErrors.
- **The reference year of a month-day** is found by the rule NonISOMonthDayToISOReferenceDate states,
  the latest such ISO date from 1900 to 1972 and else the earliest to 2035; for `chinese` and `dangi`
  it is Table 6's.
- **Era names are matched exactly**, as CanonicalizeEraInCalendar compares them; T1 had lowercased a
  Gregorian era first.

## 4. Where the references disagree

The reference polyfill computes every calendar by formatting through Node's ICU4C 77.1; ICU4X 2.3.0
computes them itself. Where they differ, the profile answers as ICU4X does, and the dataset names
each line:

- **Chinese and Korean years outside the published ones**: ICU4C's are astronomical, ICU4X's are the
  approximation above (64 lines of the dataset);
- **five Chinese dates within the published years**, where ICU4C's astronomy disagrees with the
  observatories' calendars: the twelfth month of 1953, the leap month of 1987 (ICU-13195; test262's
  `chinese-calendar-dates.js` expects ICU4X's `M06L`) and the years 2026, 2027 and 2029 (ICU-23285);
- **the Hebrew year 5806**, which ICU4C makes 385 days long; its molad and postponements, and ICU4X,
  give 384.

## 5. Validation

- **The retained dataset** ([`src/tests/temporal/calendars/`](../../../tests/temporal/calendars/README.md)):
  3,999 lines in the fifteen calendars - the fields of 137 ISO dates from 501 BCE to the year 10000,
  dates from fields, arithmetic, differences, `with`, year-months, month-days and their reference
  years, durations relative to a date. **Against the reference polyfill at `e8cc03fc`**: 3,929 lines
  agree and 70 are named in the three groups above. **Against ICU4X 2.3.0**, which
  `icu4x-reference.rs` runs over the archived crates: all 2,055 conversion lines agree, the 70
  included.
- **test262**, against the pinned suite `ccaac100`, with the `Intl.Era-monthcode` proposal's cases now
  scored (`SuiteFeatures` admits the flag by this record):
  - `test/built-ins/Temporal` passes every one of its 9,176 scored variants, 20 more;
  - `test/intl402/Temporal` passes 3,962 of 3,982, where it passed 598 of 930: 312 variants move from
    failing to passing and 3,052 newly scored ones pass. The 20 that fail are ten cases of
    `toLocaleString`, which formats in another calendar (section 6), or names a zone's long name;
  - `test/intl402/DateTimeFormat` passes 456 of 488: the 10 newly scored variants that fail format in
    the Chinese, Korean or another calendar's names (section 6);
  - no variant scored before moves from passing.
- **The whole pinned suite**, run after the change: 101,723 variants, 100,324 passing, 202 failing, 44 exhausted and 1,153 skipped. With the flag's 1,543 files scored,
  312 variants moved from failing to passing and 3,074 newly scored ones pass; the 12 newly scored
  that fail - 10 under `test/intl402/DateTimeFormat` and 2 asking `Intl.supportedValuesOf` for every
  calendar - are section 6's divergence. Every other variant's verdict is the same
  ([JSC-282](../roadmap.corrections.md#jsc-282)).
- **The slice compiler's checks**: two new, 644 in all - the dataset against the polyfill, and its
  conversions against ICU4X.
- **Rules N31 and N32**: six new architecture tests - the archives are the pinned ones, a changed byte
  and another release are refused, the generated table is the generator's, generation is
  reproducible, and well-known years read back from the parse (Chinese New Year 2024 on 10 February,
  2012's leap month placed after the fourth month in China and the third in Korea, 1 Muharram 1445 on
  19 July 2023).

## 6. What is not done

- **`Intl.DateTimeFormat` in these calendars.** The formatter resolves `gregory` and `iso8601` only,
  and `Intl.supportedValuesOf("calendar")` lists those two, where the proposal's 1.1.1 asks that the
  available calendars be every one of Table 1 and that the formatter support each. **This is a
  declared divergence**: Temporal accepts calendars the formatter cannot yet write, so a Hebrew date's
  `toLocaleString` throws the calendar mismatch's RangeError under every locale, and
  `-u-ca-hebrew` resolves to `gregory`. Formatting them needs CLDR's month, era and cyclic year names
  and patterns for each calendar in `de` and `en`, which the 107,374 bytes left under the budget may
  or may not hold: the next slice measures it and, if it does not fit, puts the question to the
  owner.
- **The `canonical-tz` proposal's cases** stay skipped, as JSD-0054 says.

## 7. What would falsify this

- A line of the retained dataset that the profile answers otherwise and `divergences.txt` does not
  name, a divergence on which ICU4X does not answer as the profile, or a conversion line of ICU4X's
  the profile does not answer.
- A scored case under `test/built-ins/Temporal` or `test/intl402/Temporal` that fails for a reason
  other than section 6 names.
- A calendar table byte that is not what the generator writes from the archived crates, or a year
  the generator reads that is not the crate's.
- A difference between two dates in a calendar that the proposal's loops would count otherwise.

## Amended 2026-10-05: the formatter's calendars built (unsigned)

*Recorded with phase F8's slice T4; it signs nothing. Corrections entry
[JSC-283](../roadmap.corrections.md#jsc-283).*

- **Section 6's declared divergence is gone** under proposed
  [JSD-0057](0057-intl-datetimeformat-in-the-cldr-calendars.md): `Intl.DateTimeFormat` resolves and
  writes every calendar of Table 1, `Intl.supportedValuesOf("calendar")` lists them, a Hebrew date's
  `toLocaleString` formats in the Hebrew calendar, and `-u-ca-hebrew` resolves to `hebrew`. CLDR's
  names and patterns for the calendars fit under the budget: the data is 761,598 bytes, 24,834 under
  it.
- The formatter reads each date's fields from this record's calendars, so a date formats with the
  year, month, day and era its Temporal getters answer. Sections 1 to 5 and 7 are kept as written.
