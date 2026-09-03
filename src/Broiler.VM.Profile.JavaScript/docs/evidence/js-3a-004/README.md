# Bundle JS-3A-004 — the conformance oracle, and the proof that a failing test comes back as a failure

**Collected:** 2026-09-03. **Milestone:** JS-3a. **Owner:** profile architecture owner.
**Reviewer:** none.

**What this bundle is.** JS-3a's oracle half, which
[Bundle JS-3A-001](../js-3a/README.md) explicitly was not: it recorded the diagnostic registry and
said, in its own closing column, that *no harness, self-check, sharding, merge, audit or scope
tooling exists, no per-host-mode totals are published and no ratchet is set.* All six exist now.

**What this bundle is not.** It is not a conformance score for JavaScript, and it retains no
figure that could be read as one. **No third-party conformance suite is in this repository, none is
pinned, and this harness fetches none.** Roadmap
[section 14](../../roadmap.md#14-the-conformance-oracle) builds the harness against the smallest
scoring target that exists rather than after the language it will eventually score, and section
3's pin — retrieve, hash, archive — is a human action nobody has performed. Everything scored here
is this component's own material, and the totals below are about the instrument.

**And it accepts nothing.** No milestone is accepted, JS-3a's exit gate has clauses open that this
does not touch, and a floor holding is not evidence for any gate.

---

## 1. The harness's first job is not scoring

> An engine that grades itself is not evidence… The harness's first job is not to score anything:
> it is to prove that a failing test comes back as a failure.

That is section 14's own sentence, and it is what this bundle is mostly about. Every run — every
shard, every time — begins with the harness's **own regression suite** (25 checks, no profile
composed, nothing lowered) and then with the **self-check**: nine fixtures whose declared verdicts
the harness has to reach, three of them controls that must pass. A mismatch stops the run on an
exit code of its own, before a single suite test is scored.

`conformance-controls.log` is the evidence, and it holds seven injections:

| Control | Injected | Caught by |
|---|---|---|
| 001 | any refusal matches any *declared* refusal | 1 of 9 self-check fixtures |
| 002 | every case is scored as a pass | 4 of 9 self-check fixtures |
| 003 | one runtime is shared by every case | 1 of 9 self-check fixtures |
| 004 | a fixture is edited without re-pinning | `MissingSuiteRevision`, and the pin mismatch named |
| 005b | one shard report is removed from a merge | `IncompleteShardCoverage`, twice, with the figures |
| 006 | the execution-only root references the harness | rule **N13**, and A7, A12, A15 and K2 beside it |
| 007 | a project carries the suite into its build output | rule **N13** — **and nothing else** |

**Control 001 is the one worth reading.** A harness that only asks *whether* a program was refused
scores a wrong refusal as clean, and every pass rate it publishes stays the same. **Control 002 is
the failure mode a pass rate cannot see at all**, because it produces the best number the
instrument has ever produced. **Control 007 is caught by one rule and by nothing else**, which is
why the suite-directory clause has a witness of its own: no reference changes and no assembly
changes, and separately licensed material is copied into a build output.

**Control 003 is not hypothetical: it is the defect this harness had on its first scored run**, and
[JSC-52](../../roadmap.corrections.md#jsc-52) records it. A budget allowance is spent over a
runtime's life rather than reset per invocation, so one composed runtime for a whole shard made the
first non-terminating program spend the allowance and **every case after it reported a timeout** —
thirty-four of them, a total indistinguishable from an engine that had stopped working. The
self-check did not catch it, because the one non-terminating fixture happened to sort last. It
carries a fixture now whose only job is to run *after* that one and pass.

---

## 2. What was scored, and what the totals are about

The suite is `broiler.javascript.slice.js-3a`, pinned at
`268f3a8d702a2fbdecd61d637284d2ce9ea51c8033b9a3eac934ff59a31eae12` — a digest over every path and
content the suite holds, recomputed on every read. **A branch name is not a pin and neither is
nothing**: a suite with no pin resolves to an unpinned revision and every run reports
`MissingSuiteRevision`, which control 004 shows.

| Host mode | Selected | Executed | Passed | Failed | Skipped | Timed out |
|---|---:|---:|---:|---:|---:|---:|
| Script | 38 | 38 | 38 | 0 | 0 | 0 |
| Module | 3 | 3 | 3 | 0 | 0 | 0 |
| Raw | 3 | 3 | 3 | 0 | 0 | 0 |

Selection, recorded stage by stage: 44 candidates, 0 known-incorrect, 0 out of scope, 0 filtered by
feature metadata, 0 unselectable, 44 selected. Without `--include-negative`, **10 negative-metadata
tests are withheld** and the selection is 34 — the opt-in section 14 asks for, counted rather than
implied.

**Every one of those declarations is a human's answer about JavaScript, written before the engine
was asked**, in the same discipline the retained corpus's completions are written under: `-5 % 3`
is `-2` because the remainder takes the dividend's sign, `1 << 31` is `-2147483648` because a shift
goes through a signed 32-bit integer, `010` is `8` in code that is not strict and a syntax error in
a module. The engine agreed with all forty-four. **That is a claim about this component and not a
conformance result**, and it is worth being explicit about why: the same party wrote the tests and
the code they judge, which is exactly what an external suite fixes and exactly what this checkout
does not have.

**Three host modes because this profile has three.** Script and module are two parse goals of one
lowering; **raw is artifact bytes with no lowering consulted at all**, which is the only mode an
execution-only image could ever run. Two of the three raw fixtures are deliberately broken
artifacts — a flipped magic byte, a payload cut short — and one is a control that runs.

---

## 3. Where the harness lives, and the rule that keeps it there

`Broiler.VM.Composition.JavaScript.Conformance`, a composition root that is **never advertised**.
It is a composition root because scoring a test means driving this profile's own lowering, verifier
and executor, and rule A11 forbids a test project to reference a profile assembly; roadmap
[section 5](../../roadmap.md#5-package-boundaries-and-the-dependency-graph) states that in advance
and ADR 0001's revision of 2026-09-03 authorises the project. The graph goes from 20 projects and
59 edges to 21 and 64, and the packable set is unchanged at exactly three.

**Rule N13 asserts the non-advertisement rather than assuming it**, in six clauses: no other
project references the harness, it declares no package identity, it carries `IsPackable false`, no
advertised register row names it, no retained closure report outside its own contains it, and no
project file names a conformance-suite directory. It is deliberately **not** phrased as "appears in
no published closure" — this root publishes one of its own, for this bundle — which is
[JSC-40](../../roadmap.corrections.md#jsc-40)'s distinction made real.

`publish-and-run.log` records the root published and run in three modes on `win-x64`: JIT, trimmed
self-contained, and **Native AOT**, each exiting 0 with the self-check green and 44 of 44 scored.
The catalog tables are identical across all three. `closure-conformance.txt` lists seven
non-framework assemblies under JIT and trimming and none under Native AOT.

---

## 4. The ratchet

The floor is a reviewed record of its own at
[`docs/conformance/floor.txt`](../../conformance/floor.txt), set from a merged three-shard run:
Script 38/38, Module 3/3, Raw 3/3, at the pinned revision above. The component's CI lane compares
against it and never writes it, and a run on a revision the floor was not set under reports that
and changes nothing.

**Sharding is content-independent**: a test's shard is an FNV-1a hash of its normalized path modulo
the shard count, so shard membership does not move when the selection changes. That is pinned by
the harness's own regression suite against stated values rather than against itself, because a hash
that were randomized per process would agree with itself in one run and disagree between two
machines.

---

## 5. Results

| What | Result |
|---|---|
| `build.log` | Release build of the whole solution, 0 warnings |
| `suite.log` | 386 tests green (207 contract, 179 architecture) |
| `assurance-gate.log` | green |
| `assurance-release.log` | **refuses**, naming each blocking declaration individually — which is the correct result while nothing in this component has been reviewed |
| `publish-and-run.log` | three composition roots × three publish modes on `win-x64`, every one exit 0; the conformance root's catalog identical across modes |
| `catalog-conformance.txt`, `closure-conformance.txt` | what the published binary composed and what the image contains |
| `conformance-controls.log` | seven injections, each caught, each reverted |
| `hashes.txt` | the files this bundle's claims are about |

---

## 6. Exclusions

1. **No third-party conformance suite is pinned**, and this bundle scores none. The attribution
   row and the core's standing-claim confirmation travel with the first ingested suite file
   ([JSC-30](../../roadmap.corrections.md#jsc-30)) and are open with it.
2. **The tests and the code they judge have one author.** Forty-four declarations agreeing with the
   engine is a statement about internal consistency. An external oracle is the thing that would
   make it a conformance result, and this component does not have one.
3. **The retained JS-1 corpus is not scored by this harness.** Section 14 names it as part of the
   scoring target; the fixture tree drives the same verifier and executor, and reading the corpus
   manifest as a raw suite is available work that is not done.
   [JSD-0015](../../decisions/0015-the-conformance-oracle-and-what-it-refuses-to-score.md) records
   why it was left.
4. **Two of the four completion kinds are reachable from no source this manifest accepts.**
   `NeverSettled` and `CompletedTwice` are exercised by recorded marker sequences in the harness's
   own regression suite and by no fixture, because `broiler.javascript.slice` admits no promise, no
   generator and no asynchronous function.
5. **The `fault` expectation kind is reached by no test**, for the same reason: this manifest has no
   `throw`, no `try` and no error objects.
6. **The known-incorrect list is empty**, and an entry requires a reason a run refuses to do
   without.
7. **One machine, one RID.** Everything here was measured on `win-x64`. The component's CI lane runs
   the harness on its whole RID matrix from this change onward, and no run of it existed when this
   bundle was collected.
8. **This collection skipped the fuzz sessions and the corpus, suite and Android controls**, which
   belong to JS-9 and JS-1 and are unchanged by this work. The controls this bundle retains are its
   own.
9. **Nothing here is reviewed.** Every unit in this component is `HUMAN_PENDING`, and the release
   mode's refusal above is the record of that rather than a failure.
