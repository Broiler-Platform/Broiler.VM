# JSD-0003 - Deployment composition labels

**Status:** Accepted for JS-0. **No composition root exists and none is advertised.**

**Date:** 2026-08-31

**Owner:** profile architecture owner. **Milestone:** JS-0.

## Decision: three labels, and no fourth is minted

| Label | Contains at run time | What its Native AOT gate must prove | First built at |
|---|---|---|---|
| `execution-only` | Format, verifier, executor, standard library. **No tokenizer, no lowering** | The approved precompiled surface verifies and executes under Native AOT | JS-1 |
| `narrow-runtime-compiler` | The above plus tokenizer, static semantics and lowering for a named restricted surface | Approved source is compiled and executed inside the published Native AOT application | JS-3b |
| `general-runtime-compiler` | The above for the approved general surface | Approved general source is compiled and executed inside the published Native AOT application | JS-8 |

**They describe when source is compiled, not how much of the language is supported.** That
sentence is repeated in the support table because it is the most likely misreading of the table:
a `general-runtime-compiler` composition carrying only `broiler.javascript.slice` supports one
manifest and compiles at run time, and an `execution-only` composition carrying
`broiler.javascript.core` supports far more language and compiles nothing.

## Decision: none is advertised at JS-0, and advertising one is a release decision

The status ledger's standing claim is that no composition is advertised, none is packable and no
runtime identifier is claimed. JS-0 changes none of that: there is no composition root in the
checkout, the profile's three assemblies are unpackable by rule N4, and the composition register
the core keeps at `docs/compositions.md` gains no row.

**A label is not an advertisement.** A composition root may exist, publish and run as a gate
artefact long before anything advertises it; what makes a composition advertised is a row in the
register, and that row obliges a published closure report, a claimed RID matrix and a support
table entry. Milestone JS-10 owns the first such row.

## Decision: what a label obliges when a root is built

1. **The closure is read off the published output**, not asserted from a project file. An
   execution-only closure that contains the lowering assembly is a failed gate, not a warning.
2. **No publish is evidence for another kind.** An execution-only publish never appears in a
   compiler-bearing composition's evidence bundle, and the reverse holds too.
3. **Each claimed RID needs its own retained publish-and-run bundle.** A RID with no bundle is
   listed as unclaimed with its reason.
4. **Trim and AOT warnings are errors**, and every suppression is inventoried with an owner and a
   reachability argument.

## The browser is always a runtime-compiler composition

There is no ahead-of-time path for the open web, because a page cannot be compiled before it is
visited. A browser composition links the tokenizer, the static semantics and the lowering into
the image, and its Native AOT gate proves **that** closure publishes and runs - not the smaller
execution-only one. This is recorded here so that an execution-only result is never read as
evidence for the composition a browser would actually ship.

**A content policy forbidding dynamic evaluation is expressed by registering no artifact
provider**, which makes the refusal a contract outcome with recorded evidence rather than an
ad-hoc check inside an engine. That is a property of a composition and belongs in its register
row, not in a manifest.

## What this record does not decide

Which compositions this profile will eventually advertise, on which RIDs, or whether the browser
composition lives in this repository at all. A browser integration composes two profile families
and belongs to whichever component composes them; [JSD-0007](0007-cross-profile-position-and-amendment-grading.md)
records what that component inherits.
