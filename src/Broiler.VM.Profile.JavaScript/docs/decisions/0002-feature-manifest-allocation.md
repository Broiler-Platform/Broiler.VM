# JSD-0002 - Feature manifest allocation

**Status:** Accepted for JS-0. No manifest is minted, accepted or scored by this record.

**Date:** 2026-08-31

**Owner:** profile architecture owner. **Milestone:** JS-0.

## Context

The core fixes a manifest's shape and identity; this profile fixes its content. A manifest ID
must begin with its own profile's ID followed by a dot and at least one further label, so every
identity below has the shape `broiler.javascript.<surface>`.

**A profile name is not a conformance claim and neither is a manifest name.** A manifest claims
only what its own retained oracle run shows, and no manifest below has a run of any kind.

## Decision: seven identities, and the milestone that may first mint each

| Manifest | Admits | Earliest milestone |
|---|---|---|
| `broiler.javascript.slice` | Numbers, arithmetic, comparison, local variables, structured control flow. No objects, no strings, no functions, no property access | JS-1 |
| `broiler.javascript.core` | Objects, prototypes, properties, closures, functions, classes, exceptions, iteration, destructuring, strict mode, and the core standard library | JS-5 opens it; increments extend it |
| `broiler.javascript.modules` | Module records, live bindings, import and export forms, and - where declared - top-level await | JS-7 |
| `broiler.javascript.dynamic` | `eval`, the `Function` constructor, and dynamic `import()` | JS-8 |
| `broiler.javascript.regexp` | Regular expressions, over the from-scratch matcher | JS-6, or excluded with a published failure |
| `broiler.javascript.intl` | Internationalization | Deferred; excluded by name until it has a run |
| `broiler.javascript.temporal` | The temporal surface | Deferred; excluded by name until it has a run |

**The slice is deliberately not JavaScript anyone would ship.** Its purpose is to close the whole
contract loop - descriptor, verifier, five outcomes, executor, five step kinds, composition,
publish and run - against about two thousand readable lines, so that a contract defect is found
against code a reader can hold in their head rather than against a copied engine.

**`dynamic` is separate from `core` for a policy reason, not a size one.** A composition that
registers no artifact provider must be able to decline exactly that surface and say so. The two
refusals it produces are different events with different catchabilities, and the support table has
to keep them apart: a composition that declines the manifest refuses `eval` **at verification**
with an invalid-artifact reason, and a composition that admits the manifest but registers no
provider refuses **at run time** with `ProviderNotRegistered`, which guest code may catch.

**`intl` and `temporal` are deferred rather than planned.** Together with `regexp` they are about
half the seed's standard library. Each gets its own identity so that a composition can decline it
truthfully, and each is named as an exclusion in the support table until it has a retained run.

## Decision: the admission criterion for the next increment

An increment mints **one** further manifest identity with a reviewed scope, extends the retained
malformed corpus, and re-runs the oracle against the ratchet. Three rules govern it:

1. **Increments do not inherit.** Manifest *n+1* admits what its own scope names. It may not be
   justified by arguing that manifest *n* implies it.
2. **A manifest with no retained run of its own is not accepted**, and the support table says so.
3. **An increment closes no milestone.** It re-enters JS-5's vertical-slice loop.

**A feature outside the declared manifest is rejected at verification, not at first execution.**
That is invariant 3 applied to the manifest boundary, and JS-1 states it with format version 1.

## Decision: the deliberately underspecified surfaces are listed before a corpus entry is written

A retained corpus compares an observed answer against a recorded one, byte for byte, across three
publish modes, so a component whose whole method is recorded expected answers cannot hold a
corpus whose expected answers legitimately vary. Every surface the specification leaves
implementation-defined, implementation-approximated or host-defined is therefore either **fixed
by this profile and recorded as fixed**, so a corpus entry may pin it, or **declared varying and
excluded from the corpus by name**.

The list JS-1 must publish with format version 1, each entry marked fixed or varying:

- property enumeration order where the specification does not fix it;
- the contents and format of stack traces and error messages;
- number-to-string and string-to-number precision at the edges left to the implementation;
- locale-, calendar- and time-zone-sensitive behaviour;
- anything the host supplies rather than the language.

**A determinism claim broader than that list is an untruthful support claim.** This record
allocates the identities; it does not settle a single entry of the list, which is JS-1's.

## What this record does not decide

No manifest is minted. No scope is reviewed. No corpus exists. No oracle exists, no suite
revision is pinned, and the language-specification edition is unpinned - which the status ledger
carries as a named open dependency, because no manifest may be accepted against a moving
document.
