# JSD-0015 - The conformance oracle: what it scores, what a run may not hide, and what it refuses to score

**Status:** Accepted for JS-3a's oracle half.

**Date:** 2026-09-03

**Owner:** verification-boundary owner. **Co-signer:** the profile runtime owner. **Both roles are
held by one person** and this record does not claim the co-signature is independent.

**Milestone:** JS-3a.

## What was open

Roadmap [section 14](../roadmap.md#14-the-conformance-oracle) states a method in enough detail to
be built from, and until this change **none of it existed**. The [ledger](../roadmap.status.md)
recorded that in those words: no harness, no self-check, no sharding, no merge, no per-host-mode
totals, no ratchet, and no pinned suite. The registry half of JS-3a had been landed twice; the
oracle half was untouched and is the larger half.

One thing had to be decided before anything could be built, and section 14 already decides it:
**the harness is built against the smallest scoring target that exists rather than after the
language it will eventually score.** That is what makes this buildable today. Retrieving, hashing
and archiving a third-party conformance suite is a human action nobody has performed, and it is
still open — but the harness's first job is not to score anything, it is to prove that a failing
test comes back as a failure, and that needs no suite at all.

Seven decisions follow. They are one record because they are one design: where the harness lives
decides what it may reference, what it may reference decides how a test is presented, how a test is
presented decides what a verdict can say, and what a verdict can say decides what a total may hide.

---

## 1. The harness is a composition root, and its non-advertisement is a rule rather than a habit

Scoring a conformance test means lowering its source, verifying the artifact and running it. That
drives this profile's own lowering, verifier and executor, and rule A11 forbids a test project to
reference a profile assembly while rule A12 forbids a composition root to reference the fixture
assembly. Between them the harness has nowhere to be but a composition root. Roadmap
[section 5](../roadmap.md#5-package-boundaries-and-the-dependency-graph) says so in advance rather
than leaving it to be found here, and ADR 0001's revision of 2026-09-03 authorises the project.

**What the scan asserts is deliberately not "appears in no published closure".** This root
publishes a closure of its own, for its own evidence, so that phrasing would be falsified by the
bundle this milestone retains. Rule **N13** asserts the property that is actually wanted: the
harness appears in **no package and in no advertised composition's closure**, no other project
references it, and no project file names a conformance-suite directory. Correction
[JSC-40](../roadmap.corrections.md#jsc-40) records the distinction; this is the change that
implements it.

**The last clause is the one that would not have been written from a reference set alone.** A
project can carry suite *files* into a build output with a content item and no reference at all,
which is redistribution of separately licensed material by a mechanism every reference-set rule in
this component is blind to. It has its own witness, because a clause with no witness of its own is
a clause that can be deleted in the same patch as four others with nothing red.

**Both negative controls take directions that would actually ship.** One adds the reference from
the execution-only root — the plausible mistake is somebody wanting "just the fixtures" or "just
the report format" — and one adds a content item globbing the suite. A control over an edge nobody
would ever add proves only that the rule runs.

---

## 2. A suite is a directory with a pin the harness re-derives, and an unpinned suite is a named failure

A branch name is not a pin, and neither is a directory path. What makes a revision a pin is that
reading the suite twice either produces it twice or says so. So a suite carries a `suite.pin`
declaring a revision, the harness computes a digest over every path and content the suite holds,
and the three answers are deliberately different:

- **no pin file** resolves to an unpinned revision, which every run reports as the named
  configuration failure `MissingSuiteRevision` — a failure of that run, never a smaller total;
- **a pin that agrees** with the files resolves to itself;
- **a pin that disagrees** is refused rather than replaced, because a suite that moved under a pin
  somebody wrote is not a suite whose pin can be believed.

The digest is over the path-and-content *pairs* rather than the concatenated bytes, so a rename
moves the revision — and a rename is exactly what moves a test between shards. Writing a pin is a
separate command somebody has to type: a run that silently re-pinned a suite it found had moved
would turn the one check that catches an edited fixture into a no-op.

**The suite this harness reads today is the component's own fixture tree, and that is not a
placeholder.** It drives the lowering, the verifier and the executor, which is the scoring target
section 14 names. The reader is the one a pinned third-party suite will be pointed at, so the day a
revision is retrieved nothing here is replaced.

---

## 3. Every case gets a runtime of its own, and the first run of this harness is why

Section 14 specifies how results are aggregated in considerable detail and says nothing about
isolating one case from another. That gap is not theoretical, and it took one run to find:
**a fuel allowance is spent over a runtime's whole life rather than reset per invocation.** One
composed runtime for a whole shard means the first program that does not terminate spends the
allowance and every case after it is reported as a timeout. The first scored run of this suite
reported thirty-four timeouts and nothing else.

That reading is the exact thing a conformance total must never be able to produce: it is
indistinguishable from an engine that has stopped working. So a case's verdict has to be a property
of that case, and isolation is not an optimisation to be traded away here. Correction
[JSC-52](../roadmap.corrections.md#jsc-52) records it.

**The allowance is fuel and not the wall clock, for a reason that outlives this defect.** Fuel is
charged one unit per instruction, so a ceiling on it bounds a runaway program in a number of
instructions rather than in seconds. A wall-clock allowance would make the same test pass on one
machine and fail on a busier one, and a floor that moved with how busy a machine was would be a
floor nobody could act on.

---

## 4. A test declares its own verdict, and the harness compares the whole answer

The conformance suite this profile will eventually score declares a *negative* expectation in
metadata and leaves a positive one implicit in the assertions a test makes — which needs a harness
library `broiler.javascript.slice` cannot express, because it has no functions to call. A test
whose whole body is `1 + 2` cannot assert anything about itself.

So a test declares its verdict in metadata, in **the suite's own frontmatter shape**: a leading
`/*--- … ---*/` comment carrying `description`, `flags`, `features` and `includes`, with one key
of this component's own, `expected`. Copying a format is not copying a suite; no suite file is in
this repository and nothing here reads one. The block is a comment, so the file compiles as
written and what is scored is the bytes on disk rather than something the harness assembled.

**The expectation names one of this profile's four answering places** — a completion value, a
refusal by the front end, a refusal by the verifier, or an execution fault — and names a refusal by
its diagnostic code's **member name** rather than by its number. That is section 14's "reported by
its JavaScript type name so a parse-phase syntax error is matched on what it is", spelled in this
profile's vocabulary: the published registry keys on the name, and a test declaring `1401` would be
a test nobody could read and one a renumbering would silently invalidate.

**The comparison is of the whole answer and not of a pass/fail bit.** A harness that recorded only
whether a case failed cannot tell an engine that refused a program for the right reason from one
that refused it for the wrong one, and would score both the same. One of the self-check fixtures is
exactly that case.

**A negative-metadata test is one whose declared verdict is a refusal**, read from the expectation
rather than from a separate field, because two spellings of one fact can disagree. They are opt-in,
per section 14, and a default run withholds them and counts how many it withheld.

---

## 5. Three host modes, because this profile has three, and each reports its own totals

Script and module are two parse goals of one lowering. **Raw is artifact bytes with no lowering
consulted at all**, which is the only mode an execution-only image could ever run and is therefore
the one whose totals say something about that image. A raw test is a `.bjsb` artifact with a
`.bjsb.meta` sidecar carrying the same metadata block, so there is one metadata reader and not two
— and the `raw` flag is then checked in **both** directions: required where bytes exist, refused
where they do not. The absence check is the one that matters: a sidecar that forgot the flag would
put artifact bytes into the script mode's totals, where the lowering is handed a file it cannot
read and every such test fails for a reason nobody wrote.

Each mode reports selected, executed, passed, failed, skipped and timed-out counts of its own. A
mode that selects files and executes none is the named configuration failure
`IncompleteHostModeCoverage`, not a small total.

---

## 6. The configuration-failure set is closed at six, and the sixth is named for a behaviour the plan states without naming

Section 14 names five: inconsistent shard configuration, missing suite revision, incomplete variant
coverage, empty selection, no executed tests. It then states a sixth behaviour without giving it a
name — "removing one shard's report must produce incomplete coverage, not a smaller total". A
behaviour a run must have is a member the enumeration must carry, so `IncompleteShardCoverage`
exists and [JSC-51](../roadmap.corrections.md#jsc-51) records that this set is the roadmap's five
plus that one. "Incomplete variant coverage" is spelled `IncompleteHostModeCoverage` here, because
the axis this profile varies over is the host mode.

**A self-check mismatch is deliberately not a member.** The self-check runs before a shard is
configured at all, so a mismatch has no run to be a property of; it stops the process on an exit
code of its own. Folding it in would let a reader believe a run had been configured and had then
gone wrong, when in fact nothing ran.

**The exit codes are four and not two**, for the same reason: a conformance failure and a broken
harness must not share one. The first is the measurement working; the second is the measurement
being unreadable, and a caller that saw one number for both would retry the wrong one.

---

## 7. The floor is a measurement discipline, and it records the revision it was set under

*Admitted* is deliberately not the ledger's `Accepted`. Accepting a milestone needs a reviewer
decision nothing in this component has, so a floor that could only be set by an accepted milestone
would be a floor nothing could ever set.

A floor records the suite revision it was set under and **is never compared across revisions**: a
suite that added tests would otherwise read as a regression, and one that removed them would
silently lower the bar. A revision change re-bases the floor from the first run admitted on the new
revision, and the old floor and the reason stay in the file rather than being overwritten. This is
the discipline the diagnostic registry and the retained corpus already apply to their own pinned
revisions.

**A run carrying any configuration failure may not set a floor, and neither may a single shard.**
The whole value of a floor is that it was measured over a run that covered what it claimed to
cover, and a run missing a shard covered less than it claimed.

---

## What this record does not decide, stated as conformance exclusions

**The retained corpus is not scored by this harness.** Section 14 names "JS-1's verifier, executor
and retained corpus" as the scoring target, and the fixture tree drives the first two. Reading the
corpus manifest as a raw suite is available work that is not done, and the reason it was left is
worth stating: the execution-only root's replay already asks a *different* question of those bytes
— whether they still produce the observation recorded beside them, four extra columns included —
and two readers of one manifest that could disagree is the drift this component's rules exist to
prevent. Whether the two questions should be asked by one program is a decision this record does
not take.

**No third-party conformance suite is pinned, and the attribution obligation is unlanded with it.**
The suite is separately licensed material that does not exist in this checkout, so its notice row
carries content nobody has read. [JSC-30](../roadmap.corrections.md#jsc-30) puts the row in the
change that first ingests a suite file, and this is not that change.

**Two of the four completion kinds are reachable from no source this manifest accepts.** `broiler.
javascript.slice` admits no promise, no generator and no asynchronous function, so nothing it can
express settles twice or fails to settle at all. `NeverSettled` and `CompletedTwice` are exercised
by recorded marker sequences in the harness's own regression suite and by no fixture. That is a
reason to test the classifier rather than a reason to leave it unwritten: the day a suspension is
admitted, the fixture that produces one is the only thing that has to be written.

**Negative-metadata tests here are refusals rather than uncaught errors.** Section 14 asks for the
uncaught error's JavaScript type name; this manifest has no `throw`, no `try` and no error objects,
so the analogue that exists is the diagnostic code's name. When the manifest grows exceptions, the
`fault` expectation kind is already in the vocabulary and already compared — it is reached by no
test today.

**The known-incorrect list is empty and that is the honest state.** An entry says "the test is
wrong and the engine is right", which is the most self-serving claim a conformance run can make. A
reason is required and an entry without one is refused; nothing in this suite has earned one.

**One machine, one RID.** Everything measured here was measured on `win-x64`. Sharding is
content-independent by construction and the harness's own regression suite pins the hash against
stated values rather than against itself, but no second machine has yet agreed with the first.
