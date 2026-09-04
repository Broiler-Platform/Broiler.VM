# JSD-0021 - The wide bring-up manifest, and format version 2

**Status:** Accepted **as a decision and nothing more.** This series'
[README](README.md) says a decision recorded here is not evidence that it was implemented, and
this record makes no claim about how well it was. **It accepts no manifest and advances no
milestone**: `broiler.javascript.wide` has no retained conformance run of its own over the whole
pinned suite, so the admission rule of
[section 6](../roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted) — a manifest
with no retained run of its own is not accepted — is unmet and stays unmet. Nothing here has been
read by a human.

**Date:** 2026-09-04

**Owner:** MaiRat. **Co-signer:** none. **Both roles are held by one person**, and this record does
not claim the co-signature is independent — there is no second signature to claim it of.

**Milestone:** JS-5, which [JSD-0002](0002-feature-manifest-allocation.md)'s allocation table names
as the milestone that opens `broiler.javascript.core`, and which therefore owns settling what this
identity is for.

## The decision in one line

**A second feature-manifest identity, `broiler.javascript.wide`, and a second bytecode format
version defined against it — rather than a widened `broiler.javascript.slice` or an early
`broiler.javascript.core`.**

## 1. Why a second identity and not a wider first one

**The slice's identity is load-bearing for material that is already written.** Its artifacts, its
retained corpus and its conformance fixtures all name `broiler.javascript.slice` and mean by it
exactly what they meant on the day they were written — numbers, arithmetic, comparison, local
variables, structured control flow, and by exclusion no objects, no strings, no functions, no
property access.

**Widening that identity in place would not have added a claim; it would have silently changed
every claim already recorded under the name.** A corpus entry that pins a refusal because the slice
does not admit a construct stops being a statement about anything the moment the slice admits it,
and it stops without any file changing and without any test failing. That is the shape of change
this component exists to make impossible, which is why
[section 6](../roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted) says the
allocation is extendable by a later milestone but **never silently widened**, and why JSD-0002's
first admission rule says increments do not inherit: manifest *n+1* admits what its own scope
names, and may not be justified by arguing that manifest *n* implies it. A second name is the only
form in which the older name keeps meaning what it meant.

## 2. Why not `broiler.javascript.core`

**The core name is allocated to JS-5 and was not free to take.** JSD-0002's table and section 6's
copy of it both put `broiler.javascript.core` at JS-5, and JS-6 — the milestone that owns the
standard library — is required by
[its entry in the delivery plan](../roadmap.delivery.md#js-6-the-standard-library) to mint the
regular-expression surface as its own identity and leave it out of `broiler.javascript.core`.

**This manifest admits a RegExp.** Taking the core name would therefore have contradicted that
requirement on the day the name was taken, and the contradiction would have lived in a name rather
than in code — which is the kind that survives, because a reader does not read a name as a claim
until it is too late to unread it. JSD-0002's admission criterion also asks for a reviewed scope
before an identity is minted; this record has no such scope and does not pretend to one, and
attaching the core name to an unreviewed bring-up surface would have spent the reviewed name on the
unreviewed thing.

## 3. Why a format version and not more opcodes in version 1

**Version 2 keeps version 1's framing and changes what a reader must find before it can check
anything.** The magic, the variable-length version integer, the manifest identity, the declared
section count, the framed sections in strictly ascending kind order and the rule that an
instruction is one opcode plus a fixed operand are all
[version 1](0008-format-version-1-the-entry-point-and-what-js-1-corrected.md)'s, unchanged. Three
things it carries cannot be reached by adding opcodes to version 1:

- **A section declaring code units.** Version 1 declares one frame and one flat set of locals,
  which is what a program with no functions needs. A function table is a new section kind, and
  **adding a section kind to a frozen format is what a version break is for** — a version-1 reader
  shown these bytes refuses them because the version integer differs, and not because a section it
  did not expect turned up.
- **An environment model addressed by depth and slot.** A captured binding is not a local. Reaching
  one needs a chain of environment records and a static (depth, slot) pair, which changes what a
  binding instruction *means* rather than adding instructions beside the ones that exist.
- **Exception regions that carry a scope depth.** A region body that states a depth beside its
  operand-stack height is a different body under a section kind version 1 already numbers, so no
  opcode reaches it; and a handler that did not know both would have to reconstruct them by walking
  back, which is the reconstruction a verifier exists to make unnecessary.

**Branch targets settle it.** Version 2's are absolute rather than version 1's displacements,
because all code units share one code section and an absolute target is checkable against the range
of the unit that contains the branch. That is a changed meaning for an operand encoding that is
already in use, and there is no opcode you can add to version 1 that changes what version 1's
existing operands mean.

## What this refuses to do

- **Rejected: widening `broiler.javascript.slice`.** Section 1's reason. The cost of the refusal is
  the whole of this record's last section; the cost of the alternative is every recorded slice
  answer quietly becoming a statement about a different surface.
- **Rejected: taking the `broiler.javascript.core` name early.** Section 2's reason. A name that
  JS-6's own requirement already forbids this content is a name that would have had to be corrected
  rather than kept.
- **Rejected: a tree-walking interpreter beside the bytecode path.** It was the shorter route to a
  running workload, and it would have answered fewer of the questions this roadmap is built to
  answer. **The format, the verifier, the budget model and the host boundary are the decisions
  under test**, and an interpreter that bypassed them would have measured none of them: a program
  that runs without being verified says nothing about what
  [section 7](../roadmap.md#7-the-bytecode-format-and-the-verifier)'s verifier admits, and work
  charged by a tree walk is not the metering
  [JSD-0011](0011-the-value-frame-and-call-abi.md)'s eighth row puts under test. A second execution
  path would have bought a demonstration and cost the evidence.
- **Rejected: a second verifier for version 2.**
  [Section 23](../roadmap.gates.md#23-risks-and-stop-conditions) makes **two verifiers that must
  agree a security defect with a schedule**, and that stop condition does not soften because the
  two would be reading different format versions — two readers that must agree about what an
  artifact is are two readers that will one day disagree. So the version-2 pass is reached through
  the **one verifier object**, which dispatches on the descriptor and **reads no payload byte to
  decide how to read the payload**. The descriptor names the version the caller says the bytes are;
  the payload names the version they are; each pass checks the second against the first. Reading
  the payload to find out how to read the payload is the ordering this component refuses to have,
  and a dispatch that did it would have been that ordering wearing a version number.

## What is provisional, and what would settle it

**`broiler.javascript.wide` is a bring-up surface, and this record writes that down rather than
leaving a reader to notice it.** Three things follow, and none of them may be inferred away by
anyone who sees the name in a descriptor:

- **It has no retained conformance run of its own over the whole pinned suite.** What exists is
  runs over subtrees somebody chose, which measure those subtrees. Section 6's rule is unmet, and
  JSD-0002 already says what a reader should do with the name in the meantime: **a manifest name is
  not a conformance claim.**
- **It is not a replacement for `broiler.javascript.core` and does not pre-empt JS-5's scope.**
  Nothing decided here narrows what core may admit, and nothing here may be cited as core having
  been partly delivered under another name.
- **The condition that settles it is JS-5 minting `broiler.javascript.core` with a reviewed scope
  and a retained run of its own.** At that point this identity is either **retired**, with its
  artifacts re-minted under core, or **narrowed to what core does not cover** — the regular
  expressions JS-6 keeps out, and whatever else core's scope leaves outside itself. **JS-5 owns
  that decision**, and until it is taken the wide identity is a name for a surface under
  construction rather than a surface to build against.

## The cost this decision accepts

**Two manifests and two format versions is two of everything a reader has to keep in their head.**
Two section-kind vocabularies, two constant-tag sets, two branch-target conventions, and an answer
for what an artifact naming one manifest at the other's format version gets. None of that is free
and none of it is temporary until JS-5 takes the decision above.

**The retained corpus is no longer of one format version.** Its entries of both live in one
manifest — `src/tests/corpus/js-1/corpus.manifest` — told apart by a column rather than by a
directory, and a reader who does not look at that column will read a version-2 refusal as a
version-1 one. Naming that here is cheaper than discovering it at JS-5.

**One thing the cost bought is worth recording beside it.** While this profile registered one
format version and its descriptor named one manifest, the core screened a descriptor against a set
with one member in it, so a descriptor mismatch could not be observed, and the diagnostic rows for
it had been `defensive` since the registry's first revision. Registering a second of each made both
screens reachable, and each now has a corpus entry that reaches it. A second identity is not only a
bookkeeping cost; it is the first thing that made the identity check observable at all.
