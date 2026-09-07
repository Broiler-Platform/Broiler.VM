# Bundle JS-7-001 — the fifth step kind, two charges that were flat, and the first clean publish-and-run transcript on this runtime identifier

**Collected:** 2026-09-06. **Milestones:** JS-4, JS-5 and JS-7 — none of which it closes.
**Owner:** MaiRat. **Reviewer:** none.

**What this bundle is.** The retained record of two things this profile had never had — a **pause it
can produce and a host can resume**, and a **measured charge for every operation family whose cost
grows with its input** — collected on a machine that publishes and runs every composition root under
JIT, trimming and Native AOT.

**What is new about the publish half is narrower than it looks, and worth stating exactly.** This
profile has published Native AOT since bundle JS-1, on `win-x64`, and bundle JSW-10-001 published on
`linux-x64` a day before this one. **What that bundle did not have was a clean transcript**: nine of
its fifteen runs exited non-zero — the conformance harness in all three modes and the end-user
host's sweep in all three — and the collector has since been changed to fail rather than retain such
a run silently. This is the first collection on `linux-x64` whose twelve publishes and fifteen runs
all exit zero. On `win-x64` bundle JS-3B-002 already had one.

**What this bundle is not.** Not an accepted anything. **Nothing here has been read by a human**;
the reviewer field of `identity.txt` says `none` and means it. No runtime identifier is claimed —
claiming one is a release act and JS-10 owns it — so a publish that ran on `linux-x64` is a
**recorded** RID and not a supported one. Section 8 lists every clause this leaves open, and there
are more of them than there are clauses it reaches.

---

## 1. The required fields, and the file in this directory that carries each

Roadmap [section 4](../../roadmap.status.md#4-required-evidence-bundle) names nine. **A field is
satisfied by a file that was written by the collection, never by a sentence here**, so this table
points and does not transcribe.

| Field | Where it is | What is worth reading in it rather than assuming |
|---|---|---|
| **Identity** | `identity.txt`, `snapshot-identity.txt` | Bundle id, milestone, collection timestamp, component commit, branch, owner and reviewer. The core contract version is 1 and no part of this raises it. |
| **Source** | `identity.txt`, `hashes.txt` | The component commit, the branch, and whether the tree was clean. `snapshot-identity.txt` re-derives JSD-0005's candidate seed revisions; a match there is not a taken snapshot, and JS-2 is what takes one. |
| **Dependencies and corpus** | `environment.txt`, `hashes.txt`, `corpus-integrity.log`, `../../../../tests/corpus/js-1/corpus.manifest` | SDK and toolchain identity, and the digest of every project, marker, register and roadmap document the collection hashes. |
| **Environment** | `environment.txt` | OS, architecture, RID, SDK. **One machine, one RID.** |
| **Procedure** | section 2 below, and the header of each log | Every command verbatim, with its working directory. |
| **Results** | every `*.log` in this directory | Retained whole, failures included. |
| **Negative controls** | `negative-controls.log`, `corpus-controls.log`, `fuzz-controls.log`, `android-controls.log` | Each control, its injection, and the revert. A control reported `SKIPPED` is a control that found no anchor and judged nothing. |
| **Closure** | `closure-*.txt`, `catalog-*.txt`, `publish-and-run.log` | **The field this bundle exists for.** Four composition roots, three publish modes each, each published image run and its closure read off the published output rather than asserted. |
| **Exclusions** | section 8 below | Every open gate clause this collection does not reach. |

---

## 2. Procedure

```text
python3 eng/collect-js-evidence.py \
  --bundle JS-7-001 \
  --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-7-001 \
  --milestone JS-7 \
  --rid linux-x64
```

Run from the component root. The script builds the solution, runs the whole suite, runs the
assurance gate in both modes, publishes and runs each composition root in three modes, runs the
fuzz sessions, runs every negative control, and hashes what it names. The header of each log carries
the commands beneath it.

---

## 3. What this collection is for, row by row

Three ledger rows named a missing collection rather than missing work, and this is that collection.
Nothing below closes a gate — the cases live in a composition root's check list, no human has read
any of it, and section 8 is longer than this section is.

| The row said | What this bundle carries |
|---|---|
| JS-4: *nothing retained shows the two-runtime clauses* | Four cases in `publish-and-run.log`, under all three publish modes: two runtimes minting the same key text and reading back only their own, one shareable handle instantiated by two runtimes, unsynchronised concurrent reads of one handle on two threads, and a handle that carries nothing the runs through it changed |
| JS-4 and JS-5: *Native AOT was not published on the machine this was written on* — *what is open there is a collection* | It is made. Every composition root published and run in three modes on `linux-x64`, closure read off the published image, every exit zero. The complaint was about bundle JS-4-001, which was collected with `--skip-publish`; other bundles in this series had published, and the most recent one on this runtime identifier had not done so cleanly |
| JS-5: *nothing retained shows a proportionality fixture for any operation family* | One fixture per shipped family, each measured against its own control by bisecting the fuel allowance, in the same log. The first run of them found two families charging a flat amount, and both were repaired before this collection |
| JS-5: *nothing retained shows the nested-handler and `finally` matrix … or the binding-time refusal of a capability* | Both, as named cases. Two clauses of the boundary gate are recorded **unreachable** rather than unmet, and the check that records them says why in its own output |
| JS-7: *`Suspended` is declared unreachable and JS-7 owns it* | The fifth step kind, produced and resumed. A pause, a resume, a second resume refused, a parked operation abandoned with its queue dropped, and the live-suspension bound answering by name |
| JS-9: *the source tokenizer and parser now EXIST and no session reaches them* | Source fuzz sessions in `fuzz.log`, beside the artifact sessions. The regular-expression matcher is still reached by no session |

---

## 4. What the published images showed

**Twelve publishes and fifteen runs, every one exiting zero.** Four composition roots — the
execution-only root, the slice compiler, the conformance harness and the end-user host — each
published as framework-dependent, as trimmed self-contained, and as Native AOT, and each published
image was **run** rather than inspected. `publish-and-run.log` carries every command, every exit
code and every line of output.

**The check lists ran under all three modes, not only under the one a developer uses.** The
execution-only root's twenty-two checks and the slice compiler's fifty each passed as
framework-dependent, as trimmed and as Native AOT — so the two-runtime cases, the host-boundary
cases, the proportionality fixtures, the suspension cases and the `finally` matrix are all answered
by a **published native image** and not only by a JIT run.

**The catalog table is byte-identical across the three modes for every root**, which the collection
compares rather than assumes: a descriptor that composed differently under Native AOT would be a
failure here rather than a footnote.

**The two closures differ by exactly one assembly, and it is the lowering.** The execution-only
root's published closure contains six non-framework assemblies and the slice compiler's contains
seven, the difference being `Broiler.VM.Profile.JavaScript.Compiler`. That is the whole of the
`execution-only` property as a fact about a published image rather than as a label, which JS-1's
gate asks for and which every collection since JS-1 has carried.

**A Native AOT image reports zero non-framework assemblies, and that is the right answer.** There
are no managed assemblies on disk beside a native image; the closure of one is the image. The
`[aot]` sections of `closure-*.txt` say `0` for that reason and not because nothing was read.

---

## 5. Results — every log, and what it is read for

| File | What it is, and what a reader should look at rather than assume |
|---|---|
| `build.log` | The whole solution, Release, zero warnings. The analyzers are on and warnings are not suppressed anywhere in this tree. |
| `suite.log` | The whole test suite. It covers the **core** and this component's rules; rule A11 forbids a test project to reference a profile assembly, so no behavioural claim about the JavaScript profile is in it. Those are in `publish-and-run.log`. |
| `assurance-gate.log` | The assurance generator asserting that every generated artefact is what it would write. A generation that is not a fixed point fails here. |
| `assurance-release.log` | **The release mode, which is expected to refuse.** Every relevant unit is `HUMAN_PENDING`, so a pass here would be the defect. What it is read for is that each blocking declaration is named individually rather than counted. |
| `publish-and-run.log` | **The largest file and the one this bundle exists for.** Twelve publishes, fifteen runs, every command and every exit code — and the check lists of the execution-only root and the slice compiler, run under all three publish modes. |
| `closure-*.txt` | The dependency closure of each published image, read off the published output. |
| `catalog-*.txt` | What each root composed, compared across modes. |
| `corpus-integrity.log` | The retained corpus re-hashed against its own manifest, and then **mutated on purpose**: two entries are altered and the replay must report both, which is what stops a corpus from being a directory of bytes that agrees with itself. |
| `fuzz.log` | The sessions, their guidance, their histograms and their counterexamples. Section 6a below reads it. |
| `negative-controls.log`, `corpus-controls.log`, `fuzz-controls.log`, `android-controls.log` | The controls, each with its injection and its revert. |
| `identity.txt`, `environment.txt`, `snapshot-identity.txt`, `hashes.txt` | Who, where, against what. |

---

## 5a. What the fuzz sessions showed, including the part that is weak

**No session found a counterexample**, and that is worth exactly what the controls in
`fuzz-controls.log` make it worth: each of those injects a defect and requires the session to find
it, so a clean session is a statement about a mutator that has been shown to work rather than a
statement about a mutator nobody has tested.

**The four artifact sessions have different mutation streams and converged on the same answers.**
Their histograms differ — the number of mutants reaching each published answer is not the same in
any two — and yet every one of them ended with the same two mutants kept as further seeds. That is
not a seed being ignored; it is the guidance signal being **coarse**, which
[JSD-0013](../../decisions/0013-the-fuzz-sessions-coverage-signal.md) already records as the price
of keying on the answer this profile publishes rather than on an edge. Four sessions are four
streams and are not four times the reach.

**The source sessions exercised the surface and their pool did not grow.** Every answer they
produced was already reached by the seed corpus alone, so nothing was kept and the guidance loop
never fired. What they demonstrate is that the tokenizer, the parser and the lowering are reached by
a session at all — the thing no retained bundle could say before — and not that the surface has been
explored.

---

---

## 6. Negative controls

**A control is a defect injected on purpose, a run that must fail because of it, and a revert that
must make the run pass again.** The four logs carry them in four families, judged by four different
things, because a rule about the graph and a rule about a language semantic cannot be judged by the
same run:

| Log | What judges the injection |
|---|---|
| `negative-controls.log` | The test suite. Graph rules, annotation rules, register rules. |
| `corpus-controls.log` | The execution-only root replayed against the retained corpus. A language semantic is not in the suite at all — rule A11 forbids a test project to reference a profile assembly — so a corpus that could not detect a semantic regression would be a directory of bytes rather than a gate. |
| `fuzz-controls.log` | A fuzz session. A session that reports no counterexample is worth what the demonstration that it *would* report one is worth, and this is that demonstration. |
| `android-controls.log` | The register rows for the mobile head. |

**What this collection ran: twenty-nine suite controls, thirteen corpus controls and two fuzz
controls, all forty-four passing.** Each failed while injected and passed after the revert, and the
logs carry both exit codes per row.

**The two Android controls were not injected at all**, because there is no Android SDK on this
machine. `android-controls.log` says so in those words: it is a **gap in this collection and not a
smaller total**, and the mobile head's register rows are unexercised here.

**One further control is unconstructible rather than skipped**, and `negative-controls.log` states
the limit rather than leaving it silent: rule N2's cross-family half would need a second profile
family in this graph, an injected edge would name a project that is not there, and the build would
fail before any rule ran. It becomes constructible when the WebAssembly profile's own JS-0
equivalent lands.

**A control reported `SKIPPED` judged nothing.** It means its anchor moved — a refactor renamed the
line the injection replaces — and the collection exits non-zero rather than letting a skipped
control read as a passing one. The count of skipped controls is in the collector's own exit, and
the rows are in the logs.

**Two of the fixtures added since the last collection carry their own control, and one of them is
the defect it was written against.** Injecting the flat charge back into string comparison fails the
proportionality fixtures naming the declared floor and the flat series, and reverting passes them —
which is recorded in [JSC-193](../../roadmap.corrections.md#jsc-193) rather than in this directory,
because it was run before the collection rather than by it.

---

## 7. Closure — and this time there is one

**Every claim in section 4 is read off a published image.** `closure-*.txt` lists what each
published output contains, mode by mode; `catalog-*.txt` carries what each root composed. A linker
annotation without execution is insufficient under
[gate 7](../../roadmap.gates.md#22-release-gates), and every image here was run.

**What that does not buy is a claimed runtime identifier.** `linux-x64` is recorded, not claimed:
the support table is JS-10's and does not exist, and until it does no publish here supports a row in
one.

---

## 8. Exclusions — what this bundle does not show

**Nothing here is accepted, and nothing here closes a milestone.** Every item below is a clause of a
gate this collection does not reach, or a limit on what it does reach.

- **No human has read any of it.** The reviewer field of `identity.txt` says `none` and means it.
  `assurance-release.log` refuses as it must, naming each blocking declaration individually. No row
  moved to `Accepted`, and the advertised composition set stays empty.
- **One machine and one runtime identifier.** `linux-x64`, on the container `environment.txt`
  describes. **A recorded RID is not a claimed one**: claiming a runtime identifier is a release act
  and JS-10 owns it, so a publish that ran here supports no support-table row.
- **The `CallDepth` measurement in JSC-192 is not in this bundle and was taken under JIT.** JS-5's
  gate asks for a frame-cost measurement *on each claimed RID under Native AOT*; that set is empty
  today, and a measurement cannot close a clause over an empty set. The recursion refusal itself is
  not exercised under Native AOT here either.
- **The structural scan JS-4 names is not here.** What stands in its place is behavioural, and the
  check says so in its own output: a per-instance structure the programs never observe would not be
  caught, and what bounds that is the construction rather than the check.
- **JS-1's hand-written encoder and its hand-written programs are not deleted**, which JS-4's gate
  asks for and asserts by scan.
- **No figure per value kind exists** under [section 17](../../roadmap.gates.md#17-measurement-discipline)'s
  rules. There is no measurement lane at all; JS-10 owns it. The proportionality fixtures here are
  charges rather than durations and are not figures under those rules.
- **A deliberately non-charging variant is not detected.** Constructing one means a second executor
  implementation for this profile, and nothing here has one.
- **The corpus gains no entry per new opcode.** JS-5's gate asks for entries covering each new
  opcode's structural, index and stack-consistency rejections; what the corpus carries is one entry
  per structural refusal the format adds, which is a different list.
- **Asynchronous instantiation is not declared and no instantiation parks**, so JS-7's
  top-level-await clause is untouched. **The residency bound has no case** — reaching it means
  letting a pause outlive a wall clock, which is a wait rather than an assertion. **Nothing reads
  the budget snapshot across a pause**, and the awaitable and timer absence scans are unwritten.
- **The other half of the external-suspension pair has no case.** A *declaring* profile in a
  composition that left the mode disabled answers `ExternalSuspensionNotEnabled`; this profile does
  not declare, and minting a second descriptor differing in that row alone would be composing a
  profile this repository does not ship in order to pass a check.
- **The regular-expression matcher is reached by no fuzz session.** Two of roadmap section 7's four
  untrusted-input surfaces are covered by the sessions here and a third is covered from this
  collection onward; the matcher is the fourth, and it is now a gap rather than an absence.
- **No conformance run over the whole pinned suite is in this bundle.** The conformance runs here
  are the harness's own fixture tree and the ingestion-shape tree, which score the instrument. The
  whole-suite run is bundle JSW-10-001's, taken against an earlier tree.
- **The Android controls were not injected**, for want of an SDK on this machine, and the mobile head is not published here at all. `android-controls.log` records that as a gap.
- **No benchmark, no floor, no baseline.** Nothing here measures a duration, and no figure in this
  directory may be read as a performance claim.
- **The soak, the aggregate-budget exercises and the corpus replay are the execution-only root's
  own checks** rather than a separate lane. They ran under all three publish modes, which is more
  than a previous collection could say, and they remain a check list rather than a measurement.
