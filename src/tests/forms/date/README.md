<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The Date area's fuel-parity probes

These are part B's population in bundle
[`sjl-6-001`](../../../../docs/evidence/sjl-6-001/decision-rule-area.md)'s area rule. Milestone SJL-6 of
the shared JavaScript libraries programme moves this family's Date algorithms - the time-value
arithmetic, the proleptic Gregorian civil-day helpers, the Date string grammar and the Date string
renderings - into `Broiler.ECMAScript.Algorithms`
([JSD-0072](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0072-date-algorithms-move-to-the-shared-algorithms-library.md)),
and `Date.parse` keeps charging fuel through this family's meter. **Nothing here is timed.** No figure from
them is retained anywhere in this repository except in bundle `sjl-6-001`.

They were written, and committed with the rule, before any code of SJL-6 existed.

## The probes (part B)

**They exist to show that the cut-over moves no charge**: each probe's smallest completing fuel allowance
is bisected at the milestone's base commit, and after the cut-over every probe must complete at that
figure and refuse at one less, in both output forms.

| Probe | What it charges |
|---|---|
| `time-arithmetic.small.js` | `Date.UTC` and the constructor from components across the representable range, before 1970, around leap days and near the range's ends; every getter, local and UTC, and `getTimezoneOffset`; and every setter with integral arguments, each of which recomposes a time value through `MakeTime`, `MakeDay`, `MakeDate` and `TimeClip` |
| `parse.small.js` | `Date.parse` and the string constructor over every shape of the Date Time String Format, with `Z`, with offsets and with none, expanded years of both signs, and this family's own `toString`, `toUTCString` and `toISOString` read back |
| `renderings.small.js` | `toString`, `toDateString`, `toTimeString`, `toUTCString`, `toISOString`, `toJSON` and `String` of a Date across the range: negative years, years of one to six digits and the range's ends |

**Every probe's answer is one the cut-over does not change.** No probe passes a fractional, non-finite or
out-of-range argument, a month past 11, a year from 0 to 99 or an invalid time value; and no string it
parses lies outside the format, other than this family's own renderings of positive years, names a day
its month does not have, or has a fraction of other than three digits. So nothing raises an error,
touches one of the three corrections the area verdict's
[correspondence](../../../../docs/evidence/sjl-6-001/correspondence.md) lists for this family (T4, G3,
P9), or reaches an implementation-defined surface it names (P4 to P7) other than this family's reading of
its own renderings (P8), which its dialect keeps. Each answer is the one Node.js 22 gives for the same
script under `TZ=UTC`, which is this family's local time (JSD-0053 §7). A probe whose answer changed after
the cut-over would be a defect of the probe or of a shared member, and part B reads it as a moved
verdict.

**What stays out.** `Temporal` and `Intl`, whose civil-day and zone arithmetic the cut-over also moves,
are outside `broiler.javascript.wide`, the command-line root's manifest, so no probe reaches them; part A's
test262 runs carry their exhaustion verdicts.

Every probe is deterministic, takes its counts as literals, and ends in an expression statement whose
value is the answer. Each compiles under `broiler.javascript.wide` and gives the same answer in both forms.
