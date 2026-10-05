<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# IANA Time Zone Database pin

**Owner:** the profile built-ins owner named in JSD-0005, as the holder of the ledger's Unicode and
locale data row. **Reviewer:** none.

The IANA Time Zone Database release the JavaScript profile's time zone data is generated from,
retrieved once, hashed, and archived here unmodified. [`tzdb.pin`](tzdb.pin) records the tarball's
length, SHA-256 and SHA-512, and the length and SHA-256 of each member a generator reads. Decision
[JSD-0053](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0053-time-zone-data-and-temporal-admission.md)
is the design this follows, and rule N29 holds the archive to the pin.

## What is here

| | |
|---|---|
| Release | **tzdb 2026e**, released 2026-09-29, the latest on the day it was retrieved |
| Archive | [`tzdata2026e.tar.gz`](tzdata2026e.tar.gz), the release's data tarball as IANA publishes it at `https://data.iana.org/time-zones/releases/` |
| Members read | the nine source files `zic` compiles by default - `africa`, `antarctica`, `asia`, `australasia`, `europe`, `northamerica`, `southamerica`, `etcetera`, `backward` - and `zone.tab`, `version` and `LICENSE`. `backzone` is not read: ECMA-402's primary identifiers are taken from CLDR, as its note recommends, and the offsets are the default build's |
| Licence | the release's `LICENSE`: the data is in the public domain |
| Retrieved | 2026-10-05, **twice**, into two files; `cmp` found them byte-identical, and the first is archived. IANA's detached PGP signature was not checked, because no key this repository trusts signs it |

The retrieval was performed by Claude on 2026-10-05. The repository owner chose that day to start
phase F8 by archiving and pinning tzdb, which answers JSD-0027's owner decision (d). Nobody has
signed anything.

## Why here

**Nothing but a test reads this file**, as for the UCD and CLDR: `TzdbTableGenerator` lives in the
architecture test project and writes `src/Broiler.VM.Profile.JavaScript.Intl/JsTzdbTables.g.cs`, which
rule N30 compares byte for byte, so a build needs neither the archive nor a network. It sits in the
shape of [`../../cldr/pins/`](../../cldr/pins/README.md): a `pins` directory under
`src/tests/<subject>/`, a pin file beside the archive, under no product project directory and named by
no project file.

## Updating

A later release is a new archive and a new pin, by the same two retrievals; the generator then
writes new tables, and the change is a decision recorded beside the release's NEWS, since a release
can move offsets that programs have already observed.
