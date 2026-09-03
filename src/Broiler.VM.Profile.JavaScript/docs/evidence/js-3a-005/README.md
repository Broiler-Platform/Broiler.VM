# Bundle JS-3A-005 — the ingestion path, and the refusals that answer nothing

**Collected:** 2026-09-03. **Milestone:** JS-3a. **Owner:** profile architecture owner.
**Reviewer:** none.

**What this bundle is.** The path that lets the conformance harness read a third-party suite, and
one rule inside it that decides whether a refusal answered anything at all.

**Why it exists, which is not why it was planned.**
[Bundle JS-3A-004](../js-3a-004/README.md) landed the harness, and its metadata reader carried a
remark that the shape it read was the suite's own — *"so that the day a pinned suite is retrieved,
this reader is pointed at it rather than replaced"*. That was tested rather than believed. Five
files written in the real dialect went through the harness on 2026-09-03: a nested `negative`
mapping, a folded `description`, an `info` block scalar, and no `expected` key, because the dialect
has none. **All five were refused**, each with `declares no readable expectation`, and a suite is
read whole — so the run scored nothing at all. Not a wrong total; no total.
[JSC-53](../../roadmap.corrections.md#jsc-53) records it.

**What this bundle is not.** It is not a conformance score for JavaScript and retains no figure
that could be read as one. **No third-party conformance suite is in this repository, none is
pinned, and nothing here fetches one.** Both suites scored below were written in this repository.
The Octane figures in section 5 come from a checkout that already existed on this machine, outside
this repository, read and not copied.

**And it accepts nothing.** No milestone is accepted, JS-3a's exit gate keeps every clause it had,
and a floor holding is evidence for no gate.

---

## 1. The rule this bundle is mostly about

An engine that refuses almost everything, and a suite whose negative tests almost all declare that
a refusal must happen, **agree on the observable outcome nearly every time they meet** — for
reasons that have nothing to do with each other.

`broiler.javascript.slice` admits no function, no object, no string value and no property access.
Point the harness at a real suite and the refusal it provokes over and over is
`ConstructOutsideManifest`: the source is valid JavaScript and this profile declined the construct,
without ever reaching the thing the test was about. Score that agreement and **a manifest that
admits almost nothing reports a near-perfect conformance total** — silently, at scale, in the
direction that flatters.

Section 14 opens by naming this exact failure — an engine that refused a test *"for the wrong
reason"* — and then specifies no mechanism that would catch it.
[JSC-54](../../roadmap.corrections.md#jsc-54) records the gap and
[JSD-0016](../../decisions/0016-ingesting-a-third-party-suite-and-the-refusals-that-answer-nothing.md)
decides it. Every source-refusal code now carries a declared language class:

| Class | Codes | May it answer a suite's expectation? |
|---|---:|---|
| Early error — the source is not JavaScript | 17 | **Yes** |
| Outside the manifest — the source *is* JavaScript | 1 | No |
| Divergence — this profile answers differently, or earlier | 2 | No |
| Implementation limit — a ceiling the specification permits | 4 | No |

The two divergences are worth naming: `AssignmentToConstant`, which the language throws as a
**runtime** `TypeError`, and `UnresolvableIdentifier`, which the language throws as a **runtime**
`ReferenceError` and whose own declaration already recorded the divergence before this change.

**A case whose refusal cannot score is reported unscorable** — not a pass, because the engine did
not earn one; not a failure, because nothing there is a defect and the failure manifest is a repair
queue. The rule runs **ahead of** the comparison, so no declaration can be written that gets past
it, and it holds in the positive direction too.

---

## 2. The negative controls

`ingestion-controls.log` holds nine entries: a clean reading, seven injections, and the clean
reading again. Each injection is applied, the thing that would have to notice it is run, and the
injection is reverted.

| Control | Injected | Caught by |
|---|---|---|
| 101 | any classified refusal may answer a negative test | 4 harness checks, one naming the code and the verdict |
| 102 | the suite's `raw` flag is taken for artifact bytes | 2 harness checks |
| 103 | a file the harness declines is dropped rather than counted | 1 harness check — **the scored run does not notice** |
| 104 | the strict reading is registered but not made strict | 1 harness check |
| 105 | the language-class map falls behind the vocabulary | 2 harness checks |
| 106 | a parse negative naming harness files is declined | 1 harness check |
| 108 | control 101 **plus** deleting the four checks it failed | 3 of 8 self-check fixtures |

**Control 101 is the one this bundle exists for.** Its output names the bug in one line —
`a-refusal-the-manifest-made-is-not-a-pass-on-a-negative-test: declared a SyntaxError, was refused
as ConstructOutsideManifest, scored Passed`.

**Control 103 is the one the scored run cannot see.** A translator that returns nothing for a file
it declines produces a clean total over a smaller candidate count, and the run exits zero. It is
caught by the harness's own checks, which is what they are for.

**Control 108 is the realistic path**, and it is why it takes two edits. Somebody relaxes the rule,
four checks go red, and the checks are deleted to make the build green. The self-check is the
second line of defence and it holds.

**One thing in that log is a defect in the control driver rather than in the harness**, and it is
recorded there rather than tidied away: reverting control 108 restored two saved copies of one file
in forward order, leaving the state after the first patch on disk. It was caught by re-running the
harness checks against a known count and repaired by hand. A control run that quietly leaves an
injection behind is worse than one that never ran.

---

## 3. What was scored, and in what

Both suites run in all three publish modes, and the harness's own checks and self-check run before
either scores anything. `publish-and-run.log` holds all six runs.

| Suite | Dialect | Candidates | Unselectable | Selected | Script | Module | Raw |
|---|---|---:|---:|---:|---|---|---|
| `js-3a` | native | 44 | 0 | 44 | 38/38 | 3/3 | 3/3 |
| `ingest-shape` | ingested | 12 | 8 | 4 | 3/3 | 1/1 | 0/0 |

**`js-3a` is identical to JS-3A-004 in every host mode**, which is the check that the native path
was not disturbed by any of this.

**`ingest-shape`'s eight declines are the point of it, not a shortfall.** They are one per arm of
the translation: the implicit assertion prelude, named harness files, an asynchronous completion
protocol, module resolution, an agent flag, a runtime failure of a type this profile has no fault
for, a parse failure declared as something other than a `SyntaxError`, and a source file carrying
no metadata block at all. **The last is declined and not refused**, because a suite ships its
assertion library and its module fixtures beside its tests and refusing one would refuse the whole
suite.

**Its `Raw` row is zero and that is a checked property.** The two dialects spell one flag the same
way and mean different things by it — source with no prelude, versus artifact bytes no front end
lowers. Control 102 is the injection that carries it across.

**42 harness checks**, up from 25, of which 17 are new and none composes a profile.
**8 self-check fixtures** in the ingested dialect, of which two are controls that must pass and
three assert that an unearned refusal is *not* scored.

---

## 4. The closure did not move, and that is the claim

`closure-conformance.txt` and `catalog-conformance.txt` are **byte-identical to JS-3A-004's**.
Seven non-framework assemblies under JIT and trimmed, zero under Native AOT. The ingestion path is
`internal` to a root that is never advertised: no package gains a dependency, no advertised
composition's closure moves, and rule N13's six clauses are unchanged and still hold.

---

## 5. The Octane measurement, and what was wrong with the old reading

The ledger recorded `The Octane benchmark | 24 | 24 | 0` and a ranked list of what those files
need. Every figure re-derives. **What was wrong is calling the twenty-four files "the Octane
benchmark"** ([JSC-55](../../roadmap.corrections.md#jsc-55)): three are the demonstration page's
own jQuery and Bootstrap, one is the harness, one the runner, and two are data blobs. **Seventeen
are benchmark sources.**

And the ranked list points the wrong way when read as a purchase order. The census now reports what
buying the top *k* would admit:

| Admitting, in ranked order over the seventeen | Benchmark sources compilable |
|---|---:|
| the first 13 | **0** |
| the first 14 | 1 |
| the first 17 | 6 |
| the first 21 | 10 |
| all 28 | 17 |

**Thirteen constructs buy nothing.** The nearest benchmark source needs nine; the median needs
sixteen. Over the twenty-four-file corpus one construct does admit a whole file — and it is
`typescript-input.js`, a data blob, which is exactly the reading the corrected composition exists
to prevent.

**The census keeps no copy.** It takes a path. The Octane checkout it read is a working tree that
already existed on this machine, outside this repository; nothing was fetched and nothing was
written back.

---

## 6. Exclusions — what this bundle does not show

- **No third-party suite was scored, because none is retrieved.** The path is exercised against
  twelve files written in this repository. A real checkout is some thousands of times larger and
  its distribution is unknown, so the eight declines and four scored cases are a **shape and not a
  forecast**. Section 3's suite-revision dependency stays open; it narrows from "a person and a
  reader" to "a person".
- **No runtime-negative case is executed by either suite.** The translation for one is written and
  checked, but this manifest reaches none of the three fault kinds from source, so that arm is
  declared and not run.
- **No `resolution`-phase case is executed**, for the same kind of reason: this manifest admits no
  import to reach a linker with.
- **One machine, one RID.** Everything here was collected on `win-x64`. The component lane runs
  both suites and both floors on the whole matrix from this change onward, and no run of it existed
  when this was collected.
- **`android-controls.log` reports 0 of 2 passed, and that is a COLLECTION GAP rather than a
  detected regression.** Both entries record the sentinel absent on the *reverted* run as well as
  the injected one, and that log's own opening paragraph says what that means: the sentinel's
  absence is what a broken run and a failed check have in common. `adb devices` on this machine
  lists none — the SDK is installed, so the collector did not skip the step outright, but no device
  or emulator was attached for it to deploy to, and all four runs produced nothing. **Neither
  control was actually judged.** The log is retained unedited rather than trimmed, because a bundle
  that keeps only the half that ran is not a bundle. Nothing in this change touches the Android
  head, its resource extraction, or the executor those two controls inject into; the last bundle to
  judge them is the one to read for that claim.
- **The Octane figures are one checkout on one machine**, with no pin and no digest, because
  retrieving, hashing and archiving is the human action that has not happened. They are a scope
  input under section 1's third category and satisfy no gate.
- **The test262 census was not re-run, because that checkout is no longer on this machine.** The
  ledger's test262 row stands as first recorded and nothing here reproduces it. Only the Octane
  half of the census is re-derived by this bundle, and [JSC-55](../../roadmap.corrections.md#jsc-55)
  corrects only that half.
- **The seventeen-file corpus is a judgement**, not a fact the checkout declares. Which files are
  benchmark sources was decided by reading them; the twenty-four-file row is kept beside it so the
  judgement can be checked rather than taken.
- **Nothing is accepted.** Two floors are set and hold. *Admitted* is not the ledger's `Accepted`,
  which needs a reviewer decision nothing in this component has.
