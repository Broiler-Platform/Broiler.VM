<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The JavaScript profile's package baseline

**What this is.** Release gate 8 ([section 22](roadmap.gates.md#22-release-gates)) asks that
**whatever JS-10 makes packable be frozen in a baseline of its own, compared in both directions so an
addition and a removal each fail**. This is that baseline: the JavaScript family's packable set, each
package's dependencies and each package's files, as the candidate pack of
**bundle [JS-10-007](evidence/js-10-007/README.md)** produced them *(corrected: JSC-300, JSC-302,
JSC-308; it was bundle JS-10-006's pack, before the Unicode cut-over gave the family's packages the
Unicode area's dependencies, before that bundle JS-10-005's, before the numerics cut-over moved the
shared dependency's version, and before that bundle JS-10-002's, before the family took its first
shared dependency)*. Rule **N37** holds it to the
checkout's packable projects and their references, and to the bundle's retained metadata, in both
directions.

**The dated budget.** On 2026-10-05, under proposed
[JSD-0061](decisions/0061-packages-consumers-and-the-release-candidate.md), the family packs **four**
packages, the four rows below. A fifth is a revision of this baseline and of ADR 0001, not a row
added here.

**What this is not.** Not a publication: nothing here was pushed to any feed, and the two versions
published to nuget.org on 2026-09-23 were published before any review (the
[support table](support.md) section 9 records it). Not the family's build-output baseline, which
freezes what a composition root in this repository can bind to rather than what a consumer outside it
restores. `Broiler.VM.Profile.MachineCode` is packable and is its own family's, not this one's.

## The family's packages

| Package | Depends on | Files |
|---|---|---|
| `Broiler.VM.Profile.JavaScript` | `Broiler.VM.Abstractions`, `Broiler.VM.Binary`, `Broiler.VM.Profile.JavaScript.Format`, `Broiler.ECMAScript.Algorithms`, `Broiler.Unicode.Algorithms` | `README.md`, `THIRD_PARTY_NOTICES.md`, `icon.png`, `lib/net10.0/Broiler.VM.Profile.JavaScript.dll`, `lib/net10.0/Broiler.VM.Profile.JavaScript.xml` |
| `Broiler.VM.Profile.JavaScript.Compiler` | `Broiler.VM.Abstractions`, `Broiler.VM.Profile.JavaScript.Format`, `Broiler.ECMAScript.Algorithms`, `Broiler.Unicode.Algorithms` | `README.md`, `THIRD_PARTY_NOTICES.md`, `icon.png`, `lib/net10.0/Broiler.VM.Profile.JavaScript.Compiler.dll`, `lib/net10.0/Broiler.VM.Profile.JavaScript.Compiler.xml` |
| `Broiler.VM.Profile.JavaScript.Format` | `Broiler.Unicode.Algorithms` | `README.md`, `THIRD_PARTY_NOTICES.md`, `icon.png`, `lib/net10.0/Broiler.VM.Profile.JavaScript.Format.dll`, `lib/net10.0/Broiler.VM.Profile.JavaScript.Format.xml` |
| `Broiler.VM.Profile.JavaScript.Intl` | `Broiler.VM.Profile.JavaScript.Format`, `Broiler.Unicode.Algorithms`, `Broiler.Unicode.Segmentation` | `README.md`, `THIRD_PARTY_NOTICES.md`, `icon.png`, `lib/net10.0/Broiler.VM.Profile.JavaScript.Intl.dll`, `lib/net10.0/Broiler.VM.Profile.JavaScript.Intl.xml` |

**No foreign dependency.** Every dependency above is a Broiler.VM package but three, each a shared
package rule I1 admits for shipping, for the projects that declare it and no other:

- `Broiler.ECMAScript.Algorithms`, the shared JavaScript libraries' algorithms package, which
  `Broiler.VM.Profile.JavaScript` and `.Compiler` declare at `0.1.0-preview.4` since the Date cut-over
  ([JSD-0073](decisions/0073-the-profile-on-the-shared-date-algorithms.md)), as the baseline's candidate
  pack does; they declared it at `0.1.0-preview.3` from the text cut-over
  ([JSD-0071](decisions/0071-the-profile-on-the-shared-text-algorithms.md)) until then, at
  `0.1.0-preview.2`, the version it was published under, from 2026-10-14 until the text cut-over, at
  `0.2.0-preview.1`, the version first planned for it, from the numerics cut-over
  ([JSD-0069](decisions/0069-the-profile-on-the-shared-numerics.md)), and at `0.1.0-preview.1` from the
  number-conversion cut-over
  ([JSD-0066](decisions/0066-the-profile-on-the-shared-number-conversions.md))
  *(corrected: JSC-301, JSC-303, JSC-304, JSC-306, JSC-308)*;
- `Broiler.Unicode.Algorithms`, the Unicode area's code package, which all four declare at
  `0.1.0-preview.4` since the Unicode cut-over (JSD-0075, on
  [JSD-0074](decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md)), packed from
  Broiler.Unicode `6e14f93` and published to nuget.org on 2026-10-16 *(added: JSC-308; until JSC-310 it read "and not yet on nuget.org")*;
- `Broiler.Unicode.Segmentation`, its optional segmentation package, which `.Intl` alone declares, at
  the same version and from the same commit, so that a composition that declines the Intl surface
  carries none of it (rule I3) *(added: JSC-308)*.

Rule N37 holds every `.nuspec` the candidate pack produced - the family's and the seven others' - to
declaring none outside `Broiler.VM.` except a shared package rule I1 admits for shipping, at the version its allow-list pins or
a later one *(corrected: JSC-296, JSC-300)*.

## The consumer

The pristine consumer, `samples/Broiler.VM.Sample.JavaScriptConsumer`, references the three core
packages and these four and nothing else (rule A14); it restores `Broiler.ECMAScript.Algorithms`
through them, and the Unicode area's `Broiler.Unicode.Algorithms` and `Broiler.Unicode.Segmentation`
with them. Bundle JS-10-007 records it restoring and running with upstream feeds unreachable, from
a local feed that holds the shared packages beside the family's, rolling back to the published
`0.1.0-preview.5` set and forward again, failing to restore a version only nuget.org holds, and
running as Native AOT; rule N37 holds the bundle's transcript to having done each *(corrected:
JSC-300, JSC-302, JSC-308; it named bundle JS-10-006, before that bundle JS-10-005, and before that
bundle JS-10-002, whose consumer had no shared package to restore)*.
