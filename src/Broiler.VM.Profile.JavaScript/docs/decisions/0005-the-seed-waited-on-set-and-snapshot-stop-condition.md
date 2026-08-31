# JSD-0005 - The seed: the waited-on set, the snapshot stop condition, satellites, and the nullable and unsafe positions

**Status:** Accepted for JS-0. **No snapshot has been taken and nothing has been copied.**

**Date:** 2026-08-31

**Owner:** profile architecture owner. **Co-signer:** the release owner, for the licence and
attribution item. **Both roles are held by one person** and this record does not claim the
co-signature is independent.

**Milestone:** JS-0. It unblocks JS-2's second external dependency; it does not unblock the
first, which is the core contract's acceptance and is not this component's to hold.

## Context

The core roadmap's seeding condition says the snapshot is taken "once the legacy fix work has
landed". **There is no programme in the seed under that name.** What exists is several
concurrently open programmes, most of which cannot be forecast to complete, so a precondition
written that way would be a precondition that never fires. The roadmap replaces it with an
itemised waited-on set, and this record is the ruling on each item.

## Decision: the waited-on set, item by item

| Open work in the seed | Would rewrite | Ruling |
|---|---|---|
| The module/ESM conformance push, and the generator, async and early-error correctness work beside it | Parser, static semantics and the built-in library - precisely the copied surface | **Wait.** These are semantics this profile wants correct in its seed, and re-deriving them after the fork costs more than waiting |
| Regular-expression backend adoption: one match-data abstraction across exec, split and replace, retiring the translator | The regular-expression surface of the library | **Do not wait, and scope the first manifest to exclude regular expressions**, publishing the exclusion. `broiler.javascript.regexp` is already a separate identity in JSD-0002, so the exclusion is a manifest that is not yet minted rather than a hole in one that is |
| The standard-library split into core, temporal, internationalization and regular-expression parts | The library's assembly shape | **Do not wait.** This component performs its own split at ingest along manifest lines, which is a different split for a different reason |
| A rename of every assembly, namespace and package ID across the seed | Every file a copy takes | **Do not wait.** This component renames on ingest into its own namespace on the first commit, which subsumes it. Waiting for a rename in order to rename again is pure delay |
| The project-shell restructure that would extract a backend-neutral front end | Nothing, by its own terms - it is forbidden from moving production code | **Do not wait, and do not plan against it.** This component performs its own extraction |

**One `Wait` and four `Do not wait`, and the shape of that answer is deliberate.** Waiting is
justified only where the seed is about to become *more correct in the surface being copied*.
Waiting for a reshuffle of code this component is going to reshuffle anyway buys nothing and
costs the difference between an early snapshot and a late one - which is not zero, because the
seed moves at a rate that makes every further release more to adapt and more to re-review.

## Decision: the snapshot stop condition

**The snapshot is taken as-is on 2026-11-30, or after 400 further commits on the seed's default
branch beyond the candidate revision, whichever comes first.** After that trigger the remaining
waited-on item is re-derived on this side of the fork, and JS-2 records what it cost.

A precondition without a deadline is how a fork becomes a permanent postponement, and the status
ledger records the absence of a stop condition as a named risk. This record closes that half of
JS-2's second blocker. **What it does not close** is the ruling's dependence on the seed actually
landing the awaited work; if 2026-11-30 arrives with the module and early-error work unlanded,
the stop condition fires and the wait is abandoned rather than extended.

## The candidate snapshot identity is re-derivable from this checkout

The roadmap records a candidate identity so the record has a shape. It is **a candidate, not a
taken snapshot**, and JS-2 replaces these values with the ones it actually took or records why it
took different ones.

| Field | Recorded value |
|---|---|
| Seed component commit | `0341e5c98553b43569217aa7a30c8a01a1eada0c` (branch `main`, 2026-08-27) |
| Nested submodule - extended date-time | `d0c036783bdeeedaeb657a69bea6e2d5f5d438e9` |
| Nested submodule - regular-expression engine | `4df3fb8e005d9688921c235ccc44e2e89746180e` |
| Nested submodule - Unicode and locale data | `151799bb010bd8c882e07bace636ed12197c3410` |
| Resolved package graph | Recorded at snapshot time, with the lockfile identity |
| SDK and runtime | Recorded at snapshot time |
| Working tree | Clean, asserted, or a retained patch identity |

**A snapshot identity is not one commit**, which is why the three nested revisions are part of
the record: a second checkout that resolved them differently would build a different tree from
the same headline commit. `eng/js-snapshot-identity.py` re-derives the four revisions from an
aggregate checkout and compares them against this table, and the JS-0 evidence bundle retains its
output. That is the "a second checkout re-derives the same identity from the record" clause of
JS-0's exit gate, discharged against the candidate.

**One honest defect in the candidate, recorded rather than discovered later.** A repository gate
in the seed is red at that commit: a configuration test asserts a smaller ownership set than the
tree contains. A snapshot precondition asking for every gate green at the snapshot commit is not
satisfied by this candidate today. **JS-2 owns verifying that claim against the seed's own suite
and either fixing it before the snapshot or recording the exception**; JS-0 records that nobody
has re-run the seed's gates from this component, so the defect is inherited from the roadmap's
own reading rather than independently confirmed here.

## Decision: satellite acquisition is opened now, with a named owner

The regular-expression matcher, the Unicode property tables and the locale data are
independently versioned components, not part of the seed's own tree. This component acquires them
as **its own dependencies**, and the Unicode side is not only tables - it carries hand-maintained
calendar, plural-rule and special-casing code that lands inside this component's root and under
JS-0's warning and resolution gates. The dead extended date-time reference is dropped.

**Owner: the profile built-ins owner.** The dependency is opened at JS-0 - this record is the
opening - and consumed at JS-6.

**If it has not landed by JS-6, JS-6 does not wait.** The first manifest excludes every surface
that needs it and publishes each exclusion with its deterministic failure. That is already the
ruling for regular expressions above, and it extends to the internationalization and temporal
surfaces, which JSD-0002 defers by name.

## Decision: the nullable and unsafe positions

Both are forced by the seed and both must be settled before the first copied file compiles.

**Nullable reference types are enabled**, matching the component's own `Directory.Build.props`,
which deliberately does not chain to the aggregate repository's suppression list. The aggregate
repository suppresses fourteen nullable diagnostics across more than twenty projects for a
legacy reason this component does not have, and inheriting that suppression at ingest would
import the reason along with the code. **Copied files are fixed rather than exempted**, and JS-2
records the cost.

**Unsafe blocks are permitted in the copied front end, and nowhere else, and not by default.**
The seed's syntax tree requires them - a visitor takes the address of a stack local, and the
pervasive string type is an unsafe struct over source, offset and length - so refusing unsafe
outright would mean rewriting the best-conditioned material in the seed as the first act of the
copy. `AllowUnsafeBlocks` is therefore set **on the lowering project only**, at JS-2, in the
project file rather than in a shared props file so that a reader of that project sees it, and the
profile and format assemblies never set it. JS-2 owns adding a rule that asserts exactly that
distribution; JS-0 does not add one, because with no unsafe block anywhere in the checkout it
would be a rule with no subject.

## What this record does not decide

Nothing about what the copy contains. The copy table, the per-file verdicts, the ingest deletions
and the attribution changes are JS-2's, and JS-2 is blocked on the core contract's acceptance -
a blocker this component does not hold and must record rather than route around.
