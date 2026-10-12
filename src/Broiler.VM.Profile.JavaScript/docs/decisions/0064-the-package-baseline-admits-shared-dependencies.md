<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0064 - The package baseline admits shared dependencies, by rule I1's list

**Status:** Decided 2026-10-12. The programme's milestone SJL-2 revises rule N37 (work package SJL-2.4),
and this record files the profile's half of that revision: the family's package baseline,
[`packages.md`](../packages.md), no longer says that every dependency is a Broiler.VM package. **No
package of the family declares a shared dependency at this revision**; what changes is what the
baseline would accept and the rule that holds it.

**Date:** 2026-10-12.

**Owner:** the JavaScript profile owner. **Co-signer:** the core architecture owner, for rule I1's
allow-list, which this record relies on rather than restates. **Both roles are held by one person**
(EX-30), and this record does not claim the co-signature is independent.

**Milestone:** JS-10 (release gate 8's package baseline). Recorded at the programme's milestone SJL-2.

---

## Context

Release gate 8 froze the family's packages in a baseline compared in both directions, and
[JSD-0061](0061-packages-consumers-and-the-release-candidate.md) made rule N37 hold it. N37 also held
every `.nuspec` the candidate pack produced to declaring no dependency outside `Broiler.VM.`, and the
baseline said so in a paragraph headed "No foreign dependency".

[JSD-0063](0063-shared-libraries-admission-and-the-seed-copy-withdrawn.md) section 1 decided that the
family may depend on the shared JavaScript libraries core ADR 0014 admits, from the programme's
milestone SJL-2, and its settling condition names the change that revises N37. That change is SJL-2.4:
a produced `.nuspec` may declare a dependency outside `Broiler.VM.` only on a package rule I1 admits for
shipping, at the version the allow-list pins or a later one, and `PackableFamilyProjects` reads package
references as well as project references, so that a row's dependencies are compared with the project's
package edges too. The paragraph in `packages.md` would otherwise state a rule that no longer exists.

## Decision

1. **The paragraph is revised in place**, through [JSC-296](../roadmap.corrections.md#jsc-296): every
   dependency of the family's packages is a Broiler.VM package or a shared package rule I1 admits for
   shipping, at its pinned version or later; at this revision every one is a Broiler.VM package.
2. **A test-only admission never reaches a package.** The admission probe of SJL-2.9 is admitted
   test-only, and N37 refuses a produced `.nuspec` that declares it, as `eng/verify-feed.ps1` refuses a
   release that depends on it.
3. **A baseline row names a shared dependency like any other.** When a family project takes an admitted
   package, its row's dependency cell names it, the project's `PackageReference` and the produced
   `.nuspec` agree with the row, and N37 compares the three in both directions.

## What this rejects, and why

- **Leaving the paragraph as written.** It would state that a shared dependency fails N37, which is no
  longer true, and the first area cut-over (SJL-3) would contradict the baseline the day it lands.
- **Exempting shared packages from the baseline.** A dependency the baseline did not name would be one
  release gate 8 does not freeze; the gate is about what a consumer restores, and a consumer restores a
  shared package as surely as a Broiler.VM one.
- **Admitting a shared dependency at any version.** The allow-list pins a version and a content hash;
  a `.nuspec` minimum below the pin would let a consumer restore code no Broiler.VM run has seen.

## Settling condition

Settled in the change that revises rule N37 with its witness (SJL-2.4), which is the change that files
this record. **This record is reopened only by a new dated record.**

## Correction entries

[JSC-296](../roadmap.corrections.md#jsc-296) (`packages.md`, "No foreign dependency").

## What this record does not decide

- **Which shared package the family takes.** Each area's verdict is a JSD of its own, filed by the
  programme milestone that applies it.
- **Anything about the core.** Rule I1's allow-list and ADR 0014 are the core's.
- **Any evidence.** N37 still reads bundle JS-10-002, which predates this record; no bundle is cited as
  meeting anything here.

## Falsified if

- a family `.nuspec` declares a dependency outside `Broiler.VM.` that rule I1 does not admit for
  shipping, or one below its pinned version, and N37 passes;
- the baseline names a shared dependency that the project file or the produced `.nuspec` does not.
