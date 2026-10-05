<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0053 - The IANA Time Zone Database, and admitting Temporal at proposal revision `e8cc03fc`

**Status:** Proposed, 2026-10-05. **Owner decision pending.** The time zone data this record
describes is in the tree; Temporal is not yet. The record opens phase F8 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile) as the delivery plan
draws it: "a record admitting the proposal at a pinned revision, a time-zone data boundary modelled on
JSD-0031's (tzdb archived, generated, pinned), and the implementation".

**What the owner decided.** On 2026-10-05 the repository owner chose to start F8 by archiving and
pinning the IANA Time Zone Database. That answers [JSD-0027](0027-intl-scope-and-data-strategy.md)'s
owner decision (d), "whether IANA tzdb is ever in scope": it is. Nothing else here is signed.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the first record of phase F8.

---

## 1. The archive (rule N29)

- **tzdb 2026e**, released 2026-09-29, the latest release on the day it was taken, archived under
  [`src/tests/tzdb/pins/`](../../../tests/tzdb/pins/README.md) as IANA publishes it: the one data
  tarball, retrieved twice on 2026-10-05, byte-identical. IANA's detached signature was not checked,
  because no key this repository trusts signs it.
- **The pin records the tarball's length, SHA-256 and SHA-512**, and the length and SHA-256 of each of
  the twelve members a generator reads: the nine source files `zic` compiles by default, `zone.tab`,
  `version` and `LICENSE`. `backzone` is not read.
- **Rule N29** holds the archive to the pin, as N27 holds CLDR's: a changed byte of the tarball or of
  a member, a missing member, a release stating another version, and a file beside the pin it does not
  name are each refused, each with a witness made in memory.
- **The data is in the public domain**, as the release's `LICENSE` states, so no licence text has to
  travel with it. `THIRD_PARTY_NOTICES.md` names it anyway, beside CLDR.

## 2. The compiler and the tables (rule N30)

- **`TzdbCompiler` compiles the source as `zic` does, for UTC offsets only.** Each zone's lines are
  taken in order, and each line's rules in chronological order. The save in force at a line's start
  is the latest rule's at or before it. A transition whose wall clock does not advance past the one
  before it is merged into that one, as `zic`'s `writezone` merges it. Abbreviations and the daylight
  saving flag are not kept, so a change of either alone is no transition, as Temporal's transitions
  are offset changes.
- **After the last year whose transitions are listed**, a zone continues by its recurring rules,
  computed per year when asked. A zone with none keeps its last offset for ever.
- **`TzdbTableGenerator` writes three tables** into `JsTzdbTables.g.cs` in the Intl data assembly,
  which rule N30 holds to it byte for byte, as N28 holds the CLDR file:
  - `TimeZones`, binary: each of the 344 Zones' first offset, palette of offsets, 17,061
    transitions and recurring rules;
  - `TimeZoneIds`: each of the 597 Zone and Link names, the zone it reads, and its primary
    identifier where that is another;
  - `TimeZoneRegions`: `zone.tab`'s zones of each country.
- **The primary identifiers are CLDR's**, as ECMA-402 6.5's note recommends, from the CLDR archive's
  `bcp47/timezone.json`. The names CLDR gives one time zone key are one zone, whose primary identifier
  is the key's `_iana` name, or its first alias where it states none. A name CLDR does not give
  resolves through tzdb's Links. `Etc/UTC`, `Etc/GMT` and `GMT`, and every name resolving to them,
  resolve to `UTC`. So `Asia/Calcutta` is `Asia/Kolkata`, `Europe/Kiev` is `Europe/Kyiv`, and
  `Europe/Bratislava`, in `zone.tab`, is primary though tzdb links it to Prague.
- **An identifier reads its primary identifier's offsets**, so two names of one zone never disagree.
  `EST5EDT` is still a Zone of its own in tzdb, but CLDR makes it `America/New_York`'s, and it reads
  New York's history.

**How the compiler was checked.** A prototype of it was compared on 2026-10-05 with `zic -b fat` of
the same nine files. Every one of the 597 identifiers gave `zic`'s offset at each of its transitions
and the second before, and every 41 days from 1800 to 2500. The first two attempts did not, and each
failure taught a rule above: the merge, and the look-back for a line's starting save. The compiler is
that prototype's port. Each of the 344 zones in the generated table decodes to exactly the
prototype's transitions. N30 also holds a few well-known histories to their offsets: New York, Kolkata,
Lord Howe's half-hour daylight saving, Kiritimati's skipped day, and local mean time.

## 3. The size

The tables are 98,718 bytes, in the Intl data assembly beside the CLDR tables, and rule N28's budget
test counts both files. The data is 670,742 bytes, 115,690 under the owner's budget of 786,432 bytes
(768 KiB), in an assembly of 683,008 bytes. A separate assembly was not chosen: Temporal is admitted
only with the Intl surface (section 6), so a composition that has the zones has the rest of the data.

## 4. What reads the data now

- **`Intl.DateTimeFormat` accepts every IANA name**, matched without regard to ASCII case. Its
  `timeZone` is the name's primary identifier, as ECMA-402 11.1.2 step 36 states. It formats each
  instant at the offset in force then. A local mean time is written with its seconds,
  `GMT-04:56:02`, as ICU writes it. Before this, every name but UTC's and the `Etc/GMT` offset
  zones' was a RangeError.
- **`Intl.supportedValuesOf("timeZone")`** lists every primary identifier, 445 of them (ECMA-402
  6.5.3).
- **`Intl.Locale.prototype.getTimeZones`** answers `zone.tab`'s zones of the locale's region
  (15.5.13), where it answered an empty list.

## 5. Validation

- **The retained time zone dataset**
  ([`src/tests/cldr/timezones/`](../../../tests/cldr/timezones/README.md)): every one of the 597 names
  at eight instants from 1850 to 2150, against Node 22.22.0's ICU 77.1 with tzdb 2025b. 559 of 610
  lines agree. 51 differ, in three groups, each named in `divergences.txt`:
  - ECMA-402 resolves a name to its IANA Zone, and V8 to CLDR's older canonical name (45 lines, the
    offsets agreeing on each);
  - Morocco's permanent +00 from 2026-09-20, in tzdb 2026e and not in 2025b (2);
  - the same resolution in the region lists and the listed identifiers, which V8 also gives without
    UTC and the `Etc/GMT` zones (4).
- **The retained Locale dataset** of [JSD-0046](0046-intl-locale.md) answered no time zones for 21
  lines. 18 now agree with Node's, and 3 differ by the same resolution.
- **test262**: `test/intl402` and `test/built-ins/Date` score as in the run
  [JSC-278](../roadmap.corrections.md#jsc-278) records. The cases that format IANA zones also take
  Temporal objects, and are skipped as the proposal they claim.

## 6. Admitting Temporal

- **The proposal is pinned at `tc39/proposal-temporal` `e8cc03fc970a65a3359e8870e3b35e687ac94e55`**,
  committed 2026-07-27, whose specification is published as the "Stage 4 Draft / July 27, 2026". Its
  text is not archived here, as [JSD-0034](0034-admitting-explicit-resource-management-ahead-of-the-edition.md)'s
  and [JSD-0040](0040-admitting-shadowrealm.md)'s proposal texts are not. The suite's cases under
  `test/built-ins/Temporal` and `test/intl402/Temporal` at `ccaac100` measure it: 4,588 and 2,006
  files. The pinned suite's `features.txt` still lists `Temporal` among proposals.
- **The identity is `broiler.javascript.temporal`**, an optional surface owning the global `Temporal`
  and the `Date.prototype.toTemporalInstant` method, **admitted only together with
  `broiler.javascript.intl`**, whose data holds the zones. It is minted in the change that publishes
  `Temporal`, as JSD-0040 minted its identity with its global, and not before: an identity that admits
  nothing would be a promise.
- **The conformance command scores the `Temporal` flag from that change**, beside
  `explicit-resource-management` and `ShadowRealm`. Until then the cases stay skipped, so no whole
  run counts thousands of failures for a global nobody has published.
- **Temporal is built in slices, each with a whole run**:
  - T1: the ISO 8601 date and time arithmetic, `Temporal.Instant`, `Temporal.Duration` and
    `Temporal.Now`;
  - T2: `PlainDate`, `PlainTime`, `PlainDateTime`, `PlainYearMonth` and `PlainMonthDay` in the ISO 8601
    calendar;
  - T3: `ZonedDateTime`, over the zones this record archives;
  - T4: `Intl.DateTimeFormat` and `toLocaleString` over Temporal objects, and the calendars CLDR's data
    can carry.

  Phase F8's exit gate is both suite directories passing, and `Temporal` leaving the `absent-globals`
  block in the change that publishes it.

## 7. What is not done

- **Zone names other than the GMT format.** A short or long specific name, `EST` or `MESZ`, and a
  generic one, `Eastern Time`, need CLDR's metazones, which the archive does not hold. A zone is named
  in the localized GMT format, as ICU names a zone without a metazone.
- **The local time zone stays UTC.** `Date`'s local time and `DefaultTimeZone` read no host zone, as
  before; the host's zone is a host capability no composition has asked for.
- **`backzone`'s pre-1970 histories** are not compiled. The default build's are, as ICU's are.
- **Temporal itself**, section 6.

## 8. What would falsify this

- An offset the tables give that `zic` of the same files does not give at the same instant.
- An identifier whose primary identifier is not ECMA-402 6.5's, or two names of one zone answering
  different offsets.
- A line of the retained dataset that the profile answers otherwise and `divergences.txt` does not
  name, or a divergence whose reason the release's data does not bear out.
- A table byte that is not the generator's, which N30 would report, or the data past the owner's
  budget, which N28 would.

## Amended 2026-10-05: slice T1 built, and the plan of section 6 revised (unsigned)

*Recorded with phase F8's slice T1; it signs nothing. Corrections entry
[JSC-280](../roadmap.corrections.md#jsc-280).*

- **`Temporal` is published** under proposed
  [JSD-0054](0054-temporal-in-the-iso-and-gregorian-calendars.md), and the identity
  `broiler.javascript.temporal` is minted with it. The `Temporal` flag is scored from the same change,
  and the name has left the `absent-globals` block.
- **The identity is admitted only together with Intl and BigInt.** Section 6 named Intl alone;
  JSD-0054 section 3 adds BigInt, whose values an instant's epoch nanoseconds are.
- **The four slices of section 6 are three.** `Duration`'s rounding reads a `PlainDate` or a
  `ZonedDateTime`, so the first slice could not stop at `Instant`, `Duration` and `Now`. T1 built
  every type in the ISO 8601 and Gregorian calendars; T2 is Intl over Temporal objects; T3 the other
  calendars (JSD-0054 section 2). Section 6 is kept as written.
