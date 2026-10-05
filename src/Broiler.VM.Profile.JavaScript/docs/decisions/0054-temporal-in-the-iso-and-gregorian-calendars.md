<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0054 - Temporal in the ISO 8601 and Gregorian calendars

**Status:** Proposed, 2026-10-05. **Owner decision pending.** The implementation is in the tree. It
is phase F8's second record, the first slice of `Temporal` that
[JSD-0053](0053-time-zone-data-and-temporal-admission.md) section 6 admits at
`tc39/proposal-temporal` `e8cc03fc`, and it revises that section's slice plan (section 2).

**What the owner decided.** On 2026-10-05 the repository owner chose to start phase F8 with tzdb and
to build Temporal slice by slice, each slice followed by a whole test262 run. Nothing here is signed
beyond that choice.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. Phase F8, slice T1.

---

## 1. What is built

- **The `Temporal` namespace and its eight types**, `Duration`, `Instant`, `PlainDate`, `PlainTime`,
  `PlainDateTime`, `ZonedDateTime`, `PlainYearMonth` and `PlainMonthDay`, each with the constructor,
  static methods, accessors and prototype methods the pinned draft states; `Temporal.Now`; and
  `Date.prototype.toTemporalInstant`.
- **Two calendars, `iso8601` and `gregory`.** The Gregorian calendar is the ISO calendar with its two
  eras, `ce` and `bce` (`ad` and `bc` accepted as aliases), and no week numbers, as CLDR states it.
  Every other calendar identifier is a RangeError, as `CanonicalizeCalendar` answers one an
  implementation does not support.
- **Every time zone JSD-0053 archived**, through the same tables `Intl.DateTimeFormat` reads, and
  every UTC offset to the minute. A zone's possible instants for a wall-clock time are the offsets in
  force a day either side, kept where the instant they imply has that offset. Its transitions are the
  listed ones, then its recurring rules year by year.
- **The draft's grammar** (s13.31), parsed by a recursive-descent parser whose alternatives backtrack
  as the grammar's do, with the annotation and critical-flag rules.
- **Exact arithmetic.** Epoch nanoseconds and time durations are `BigInteger`s. A total, a fraction of
  two of them, is converted to a Number correctly rounded. Duration fields are float64-representable
  integers, as the draft keeps them.
- **The draft's rounding of a duration relative to a date** (s7.5.33 to s7.5.39): the nudge window,
  the nudge to a calendar unit, to a zoned time or to a day or time, and the bubbling up of a carried
  unit.
- **`Intl.DurationFormat` reads its argument with ToTemporalDuration** where Temporal is admitted, as
  the proposal amends ECMA-402 s15.10.1, so `format('PT1H')` formats a duration string. Without
  Temporal it reads as before.

## 2. The slice plan, revised

JSD-0053 section 6 planned four slices: the ISO arithmetic with `Instant`, `Duration` and `Now`; the
plain types; `ZonedDateTime`; and Intl with the other calendars. **That plan could not be built in its
order.** `Duration.prototype.round` and `total` read a `relativeTo` that is a `PlainDate` or a
`ZonedDateTime`, and `Instant.prototype.toString` and `toZonedDateTimeISO` need time zones. A first
slice without them would have had to leave the draft's own steps half-taken. So:

- **T1, this record**: every type, `Temporal.Now`, and the ISO 8601 and Gregorian calendars.
- **T2**: Intl over Temporal objects. `Intl.DateTimeFormat`'s `format`, `formatToParts` and ranges
  over the plain types and instants (ECMA-402 as the proposal amends it, s15.6), the `toLocaleString`
  of the plain types and `ZonedDateTime`, and the `iso8601` calendar in `Intl.supportedValuesOf` and
  the formatter.
- **T3**: the calendars beyond ISO 8601 and Gregorian that CLDR's data carries, with their eras,
  month codes and leap months.

Each slice is followed by a whole run of the pinned suite.

## 3. The identity and its admission

- **`broiler.javascript.temporal`**, minted in this change with the global it publishes, as JSD-0053
  said it would be. A program naming `Temporal` declares it, as one naming `Intl` declares that
  surface.
- **It is admitted only together with `broiler.javascript.intl` and `broiler.javascript.bigint`.**
  JSD-0053 named Intl, whose data holds the zones. **BigInt is added here**: `epochNanoseconds` is a
  BigInt, and the constructors of `Instant` and `ZonedDateTime` take one, so a realm without BigInt
  values could not answer them. A descriptor naming the surface without either is refused when it is
  built. A door admitting every surface it can build admits Temporal only when it was handed the
  data, and the conformance runner's `--decline` of Intl or BigInt declines Temporal too.
- **The realm builds `Temporal` after `Intl`.** `Duration.prototype.toLocaleString` makes an
  `Intl.DurationFormat`.
- **The conformance command scores the `Temporal` flag** from this change, beside
  `explicit-resource-management` and `ShadowRealm`, so every case under `test/built-ins/Temporal` and
  `test/intl402/Temporal`, and the 82 cases elsewhere that claim the flag, are scored.
- **`Temporal` leaves the `absent-globals` block**, its last name: the block is empty, and rule N17
  now allows it to be.

## 4. Where the draft's text and its reference implementation disagree

The pinned draft is followed step by step. **In two places its text does not do what it means**, and
the proposal's polyfill at the same revision does; the profile follows the polyfill, and each place
is commented in the code:

- **ISODateSurpasses (s3.5.6)** returns false when `months` is zero before it reaches `weeks` and
  `days`, so CalendarDateUntil's loop over weeks (s12.3.9), which calls it with zero months, would
  never end. The profile reckons years and months by the surpass test and weeks and days by the
  difference in epoch days, as the polyfill's `CalendarDateUntil` does.
- **ComputeNudgeWindow (s7.5.33)** takes the origin as the window's start when `r1` is zero. A
  window of months over a duration of whole years has `r1` zero and a start that is not the origin,
  so an exact year rounded to months would gain a month. The polyfill tests whether the start
  duration is zero; the profile does too.

## 5. Choices the draft leaves to an implementation

- **`Temporal.Now.timeZoneId()` is `UTC`**, the profile's local time zone, as `Date`'s local time is
  ([JSD-0053](0053-time-zone-data-and-temporal-admission.md) section 7). The clock is `Date`'s, to the
  millisecond.
- **`toLocaleString`** is ECMA-402's for `Duration`, through an `Intl.DurationFormat`, and for
  `Instant`, through an `Intl.DateTimeFormat` made for the call, at the instant's millisecond. For the
  plain types and `ZonedDateTime` it writes the ISO string their `toJSON` writes, **until T2**; that
  is a declared divergence from ECMA-402 s15.11, not a reading of it.
- **Messages** name the operation and what was wrong; the draft states only the error's type.

## 6. Validation

- **The retained Temporal dataset** ([`src/tests/temporal/`](../../../tests/temporal/README.md)):
  1,629 lines of durations rounded and totalled against plain and zoned dates, dates in both
  calendars, times rounded in every unit and mode, seven time zones at instants near their
  transitions, wall-clock times in gaps and folds under each disambiguation, and instants rounded at
  exact ties, against the reference polyfill at `e8cc03fc` on Node 22.22.0. **Every line agrees**;
  `divergences.txt` names none. The first comparison differed on 17 lines, all the second place
  section 4 names, which is how it was found.
- **test262**, against the pinned suite `ccaac100`:
  - `test/built-ins/Temporal`: **every one of its 9,156 scored variants passes**; 10 are skipped as
    the `Intl.Era-monthcode` proposal's;
  - `test/intl402/Temporal`: 464 of 930 scored variants pass. Of the 466 that fail, 154 are
    `toLocaleString` over the plain types and `ZonedDateTime` (T2), and 312 use a calendar other than
    ISO 8601 and Gregorian (T3). 1,541 are skipped as the `Intl.Era-monthcode` (1,526) and
    `canonical-tz` (15) proposals';
  - the 82 cases outside those directories that claim the flag: 52 variants pass and 108 fail, every
    failure an `Intl.DateTimeFormat` case formatting a Temporal object (T2); 2 are skipped.

  Elsewhere under `test/intl402`, `test/built-ins/Date` and `test/staging`, every variant scores as in
  the run [JSC-279](../roadmap.corrections.md#jsc-279) records, but for three `test/staging/sm` variants
  that ran out of wall time under the run's load and pass alone.

  The whole pinned suite, run after the change: 100,180 variants, 96,694 passing, 746 failing, 44
  exhausted and 2,696 skipped, every variant scored before with the same verdict
  ([JSC-280](../roadmap.corrections.md#jsc-280)).

  The run found two defects before this record was written, each fixed: a tie under `halfEven` rounded
  by the remainder's parity rather than the quotient's (instants and `ZonedDateTime` strings), and a
  named zone's possible instants checked a day range the draft checks only for offsets, which refused
  a wall-clock time a day before the limit west of UTC.
- **The slice compiler's checks**: three new, 641 in all. A door handed no data builds no `Temporal`;
  a descriptor naming it without Intl or without BigInt is refused; and the dataset above.

## 7. What is not done

- **T2 and T3**, section 2. Every scored case that fails is one of theirs: 262 variants are T2's,
  `toLocaleString` and `Intl.DateTimeFormat` over Temporal objects, and 312 are T3's.
- **The `Intl.Era-monthcode` and `canonical-tz` proposals' cases** stay skipped as the proposals they
  claim: 10 under `test/built-ins/Temporal`, 1,541 under `test/intl402/Temporal`.
- **`Intl.supportedValuesOf("calendar")`** lists `gregory` alone; `iso8601` joins it with T2, when the
  formatter accepts it.

## 8. What would falsify this

- A line of the retained dataset that the profile answers otherwise and `divergences.txt` does not
  name, or a polyfill line this record says follows the draft that the draft's text contradicts.
- A scored case under `test/built-ins/Temporal` that fails for a reason other than one section 7
  names.
- A realm admitting the surface without Intl's data or without BigInt, or a `Temporal` in a realm
  whose composition did not admit it.
- A place where the profile follows the polyfill against the draft that section 4 does not name.

## Amended 2026-10-05: slice T2 built (unsigned)

*Recorded with phase F8's slice T2; it signs nothing. Corrections entry
[JSC-281](../roadmap.corrections.md#jsc-281).*

- **T2 is built** under proposed [JSD-0055](0055-intl-over-temporal-objects.md): `Intl.DateTimeFormat`
  over Temporal objects, ECMA-402's `toLocaleString` for every type, and the `iso8601` calendar in the
  formatter and `Intl.supportedValuesOf`. Section 5's declared divergence, the ISO string written by
  the plain types' and `ZonedDateTime`'s `toLocaleString`, is gone. Sections 5 to 7 are kept as
  written.

## Amended 2026-10-05: slice T3 built (unsigned)

*Recorded with phase F8's slice T3; it signs nothing. Corrections entry
[JSC-282](../roadmap.corrections.md#jsc-282).*

- **T3 is built** under proposed [JSD-0056](0056-temporal-in-the-cldr-calendars.md): Temporal in every
  calendar of the Intl era and month code proposal's Table 1, and that proposal's `Intl.Era-monthcode`
  flag scored. Section 7's 312 variants of T3's now pass, and so does every scored variant under
  `test/built-ins/Temporal`. The Gregorian calendar's eras are now the proposal's general ones, and an
  era name is matched exactly, as CanonicalizeEraInCalendar compares it. Sections 1 to 8 are kept as
  written.

## Amended 2026-10-05: slice T4 built (unsigned)

*Recorded with phase F8's slice T4; it signs nothing. Corrections entry
[JSC-283](../roadmap.corrections.md#jsc-283).*

- **T4 is built** under proposed [JSD-0057](0057-intl-datetimeformat-in-the-cldr-calendars.md):
  `Intl.DateTimeFormat` writes every calendar of the Intl era and month code proposal's Table 1 from
  CLDR's names and patterns, and `Intl.supportedValuesOf("calendar")` lists them. Every type's
  `toLocaleString` in those calendars now formats. `test/intl402/Temporal` passes 3,980 of 3,982
  scored variants; the two left write a zone's long name. Sections 1 to 8 are kept as written.
