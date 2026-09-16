# Evidence bundle VM-5-002

**Milestone:** VM-5 - baseline the core's own overhead
**Collected:** 2026-09-16. The predeclared rule, the conformance parity rule and the two base test262
wall-clock lists were committed first, by `dbc8d37`, before the changed build was measured. The
evidence was retained by `c9afb0d`, after every run it describes, and later commits changed what
section 5.9 lists.
**Core contract version:** 1, unchanged. Fuel pre-admission adds no public member, mints no
amendment and adds no member to the meter interface.
**Status of the milestone after this collection:** In progress, unaccepted.

This bundle records what was run and what happened. It does not accept a milestone: no reviewer has
read this work, `HUMAN_REVIEW.md` is unsigned and `PENDING`, and ledger update rule 7 puts
acceptance behind an owner and a reviewer confirming every objective exit condition.

> **No language performance claim follows from any figure in this bundle.** Every measurement is of
> the **core's own overhead** around a fixture profile whose executor is a toy stack machine, or of
> one JavaScript shape on one workstation. A real language profile's cost is its own, and nothing
> here predicts it.

**What `dbc8d37` committed before the changed build was measured.** Sections 5.1, 5.2 and 5.3 stand
exactly as that commit wrote them, so they still say what was fixed in advance. That commit's header
said what it was, and its two paragraphs that no longer describe the bundle are kept here, as it
wrote them, rather than left standing as the header; "this commit" in them means `dbc8d37`:

> **Collected:** 2026-09-16, and incomplete. This commit retains the predeclared rule, the conformance
> parity rule and the two base test262 wall-clock lists, and nothing else. Every other section below
> says what it is waiting for.
>
> **Why this commit exists before the changed build was measured.** A rule written after the numbers
> are in is not a rule, it is a description. The subject of this bundle is a change made for speed, so
> the reading of its evidence is fixed here, in the repository, before the changed build is measured:
> what must hold, cell by cell, for the change to be described as faster, and what is named as failing
> when a cell does not hold. The two base test262 lists in section 5.3 are part of the same
> predeclaration - they are the rows the parity rule will later admit a difference on, they were taken
> from the unchanged build, and they are committed before the changed build runs the suite once.

Every other section was written after every run it describes, and says which run that was; section
5.9 lists what later commits changed.

**What the evidence says, in one paragraph.** The predeclared rule of section 5.1 is **not met as a
whole**. Item 1 fails on two witnesses: after the resolution fallback, the witness that stops the
ambient lookup comparing contexts still fails the environment-meter test but no longer fails the
two-thread invocation-allowance test the design named for it, and a witness added for the
suppressed-flow arm fails no test at all. Item 4 fails in both bench-host pairs.
Items 2 and 3 hold, item 3 only on its second run: its first run failed the ambient-concurrency part,
that failure triggered the fallback commit `34dbd7a`, and both runs are retained. Section 7.3 gives
the verdict per item and section 6 the figures.

---

## Field coverage

The status ledger's section 3 fixes the fields a bundle must carry.

| Field | Where | State at completion |
|---|---|---|
| Identity | Section 1 | Written |
| Source | Section 2 | Written |
| Dependencies and corpus | Section 3 | Written |
| Environment | Section 4 | Written |
| Procedure | Section 5 | 5.1 to 5.3 as `dbc8d37` committed them; 5.4 to 5.8 written at completion; 5.9 lists what later commits changed |
| Outputs | Section 6 | Written |
| Decision | Section 7 | Written |
| Validity | Section 8 | Written |

Section 9 carries the exclusions.

---

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle ID | VM-5-002 |
| Milestone | VM-5 |
| Roadmap revision | `docs/roadmap.md` as committed, section 13's VM-5 gate |
| Core contract version | 1, unchanged |
| Reason-registry revision | 2, unchanged |
| Owner | MaiRat, holding all six roles ADR 0012 records |
| Reviewer | None. No area verdict in `HUMAN_REVIEW.md` section 8 is set. |

**The commits this bundle is about**, on branch `claude/fuel-credit-block-steps`:

| Commit | Subject | What it carries |
|---|---|---|
| `cd362ac` | Pin fuel charging to the unit before changing how charges are applied | The fixture profile's windowed-polling variant and its environment observer, the fixture executor's windowed poll and its guest-load fuel ceiling, five fixture artifact writers, and the sixteen fuel-exactness contract tests. The meter is unchanged at this commit, which is what makes those tests an oracle rather than a description |
| `ba760d9` | Make the fuel-exactness tests fail on the claims they guard rather than on their timing | An amendment to the same tests, still on the unchanged meter |
| `e117162` | Admit fuel without the meter's lock inside a block checked at every level | `VmFuelPreAdmissions`, the `VmMeter` fast path and its split from the locked charge, the pre-admissions property on `VmBudgetLevel`, and the settle points in `VmRuntime`, `VmInstanceImplementation` and `VmInstantiation` |
| `a416c7a` | Resolve the ambient meter once per execution context | The resolution cache in `VmExecutionScope`, keyed by execution context, held in one field on the scope |
| `4490eda` | Request a guest load from a profile that polls on a window, so the remainder it is verified under is read with fuel outstanding | No new test: an amendment to T9, which now requests its guest load from `WindowedGuestLoads`, a fixture profile variant this commit adds that polls on a window, with a remark saying why |
| `738d9c7` | Answer a zero-unit fuel charge without a locked write, and make each settle say what it alone guarantees | A zero-unit fast path, and the remark on each settle point saying what that point alone guarantees |
| `c98011a` | Hold five steps of one runtime open, and suppress a flow on a thread that is in no step | Two further tests: a full pre-admission table's eviction, and a lookup with the execution flow suppressed |
| `dbc8d37` | Predeclare the rule the fuel pre-admission evidence is read against | Sections 5.1 to 5.3 of this README, before the changed build was measured |
| `34dbd7a` | Hold the resolved meter on the thread rather than on the scope | The resolution fallback the predeclared rule names for a failure of its ambient-concurrency part: the resolved scope, context and meter move to three thread-static fields, and `Leave` clears the leaving thread's |
| this commit | Retain the evidence for fuel admitted in blocks as bundle VM-5-002 | Everything else in this directory |

`34dbd7a` was first committed as `66efe30` and then amended in its message alone: the two commits
carry the same tree, and the binaries built after the fallback carry `66efe30` as their commit
stamp - as the lanes of E10's second run, the only ones of those binaries that survived, show in
`e10/lanes.txt`.

**The base for every comparison is commit `f127d92`**, "Pin every clause of the rule the granularity
harness applies, and refuse a shape with no twin" - the last commit on this branch before the meter
changed. It already carries the fuel-exactness tests and their amendment, so those tests are present
and passing in both builds, and it carries the per-block-steps harness commits, which touch no
product assembly.

The design this work follows names an earlier commit as the base, because it was written before
those test and harness commits landed. That commit's tree differs from `f127d92` only by them, and
`f127d92` is the commit that was actually built and run, so it is the base named here.

**The credit build is the head of the branch**, and which head each run used is stated with the run
in section 5.4: the runs before the fallback used the product at `c98011a` (`dbc8d37` changed this
README and nothing else), and the runs after it used the tree of `34dbd7a`.

---

## 2. Source

**The working tree when the evidence was retained** was `34dbd7a` with nothing modified: the only
entries `git status` showed were this bundle's own files and an untracked `.broiler-review/`
directory dated 2026-09-08, which no project includes. The collector additionally rewrote
`docs/evidence/vm-6/d1-outcome.txt` during its test step, and that change was reverted before this
commit (section 5.7).

**What the change touched in the product assemblies**, `f127d92` to `34dbd7a`, all in
`Broiler.VM.Runtime`:

| File | Change |
|---|---|
| `VmFuelPreAdmissions.cs` | New: the per-runtime table of at most four fuel blocks, the settle of one holder and of all, the pre-admission and its share of a contended level, single-victim eviction |
| `VmMeter.cs` | The lock-free fast path for a fuel charge that fits the meter's block, the locked exact charge split out of it, and the settles in the charge, the poll, the uncharged-work reader, the remaining-allowance reader, both sections of a retention and the snapshot |
| `VmBudgetLevel.cs` | The pre-admissions property a runtime level carries |
| `VmRuntime.cs` | A settle of every holder in the budget snapshot and at disposal |
| `VmInstanceImplementation.cs` | The step-end settle on the invocation and resume paths |
| `VmInstantiation.cs` | The step-end settle after instantiation |
| `VmExecutionScope.cs` | The resolution of the ambient meter: first a per-scope cache keyed by execution context, then the thread-static fallback |
| `VmVerification.cs` | A comment only: why a caller-driven verification meter gets no step-end settle |

**Outside them:** `FuelChargeExactnessTests` gained the two tests `c98011a` added, and `4490eda`
amended T9's profile variant (the file comments number the tests T1 to T18); three fixture files
changed with that amendment (`FixtureHostCapabilities`, `FixtureVmExecutor`, `FixtureVmProfile`),
to add the variant; the architecture rule tests'
covered-file count moved by the one new product file, and the generated `CODE-ASSURANCE.md`,
`HUMAN_REVIEW.md` and `assurance.manifest.json` were regenerated with every new or changed unit
`PENDING`. `docs/api/public-api.txt` is unchanged at every commit in the series (section 6.1, E1c).
No profile assembly and no profile source changed.

---

## 3. Dependencies and corpus

| Dependency | Pin | Used by |
|---|---|---|
| test262 | `tc39/test262` at `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`, 56,560 files, content digest `46d54f57ae3a4803c6ebc5f4625dd4b417254ed65058836732f182801e1cfe93`, archived under `src/tests/conformance/pins/`; every shard of every run verified the checkout against the pin | E7, E8 |
| ECMA-262 edition | ES2026, `tc39/ecma262` at `0248456c758431e4bb8e5d26333ff1865123c9cd`, as the conformance host prints it | E7, E8 |
| Octane | `chromium/octane` at `570ad1ccfe86e3eecba0636c8f932ac08edec517`, 37 files, content digest `aab618b86ebe2f18229c18f0147943e9a1bac69967582563e120d4cea273b4c4`, archived under `src/tests/octane/pins/` | E11 |
| JavaScript corpus | `src/tests/corpus/js-1`, replayed by the execution-only host's corpus rows | E6 |
| Core corpus | `src/tests/corpus/vm-2`, replayed by the evidence collector in three publish modes | Collector step 7 |

This work adds no corpus. The JavaScript shapes, the fuel-minimum programs, the probe and the
measurement scripts it ran are retained under `measurement/`, and they are measurement inputs rather
than a corpus: nothing replays them as a gate.

---

## 4. Environment

| Field | Value |
|---|---|
| Machine | One Windows workstation, `DESKTOP-UK9RCOH` |
| OS | Windows 11 Enterprise, build 26200 |
| Architecture and RID | x86_64, `win-x64` |
| Processor | AMD Ryzen 7 5800X, 8 cores, 16 logical processors |
| Memory | 32 GB |
| Power scheme | High performance |
| SDK | .NET SDK 10.0.401, not pinned (EX-03) |
| Python | 3.11.9 |
| Configuration | Release throughout; the bench host on the JIT lane, and on Native AOT in the collector's run |
| GC | Workstation, as the bench host reports it |
| Lane | None. One machine, one RID, no CI (EX-45) |

`machine-state-before.txt` and `machine-state-after.txt` bracket the timing runs E10 to E13, and
`environment.txt` is the collector's record of the SDKs and runtimes this machine resolved.

**The machine was not idle during the timing runs**, and the retained notes say so rather than this
README guessing: between the two snapshots, 55 minutes apart, the agent processes that both
snapshots list among the eight busiest accumulated **at least** 3,900 CPU-seconds - about 1.2 of the
16 logical processors running continuously - and the desktop processes both list at least a few
minutes more. Those are lower bounds: the second snapshot lists another agent process and a Git status cache
that the first snapshot's eight busiest do not, so what those two accumulated in between is unknown,
and so is anything neither list names.
No build ran during any timing run. Every timing figure in this bundle is a property of this machine
in that state (EX-45).

---

## 5. Procedure

Sections 5.1 to 5.3 are what had to be written **before** the changed build was measured, and they
stand here exactly as commit `dbc8d37` wrote them: the rule the timing evidence is read against, the
rule the conformance parity is read against, and the two lists the parity rule admits a difference
on. Sections 5.4 to 5.8 were written at completion: which build each run used, the order in which
things happened, every command with the file that retains its transcript, how this directory was
collected, and every place the collection departed from the design this work follows. Section 5.9
lists what commits after the bundle's own changed in it, after a review.

Throughout this section members and types are named rather than source lines, per rule H3.

### 5.1 The predeclared rule, committed 2026-09-16

This is the rule. It is committed before any timing run of the changed build, and before the changed
build has run the conformance suite at all.

1. **Every correctness result holds.** The build is clean with warnings as errors; every test project
   passes; the generated assurance and review records are regenerated and the public API file is
   unchanged; the fuel-exactness tests pass unchanged on both builds; every injected-defect witness
   fails the test it names and passes again when reverted; both profile assemblies are byte-identical
   between the two builds under a deterministic build; the JavaScript checks, the corpus replay and
   the host-lifetime checks pass in both builds with identical verdicts; the conformance parity of
   section 5.2 holds; the low-fuel runs over the named subtrees agree row for row in both forms; and
   the fuel minima measured through the command-line host are identical in both builds and both
   forms.
2. **Shapes.** For each shape and form, the credit build's median is at or below the base build's,
   and the gap is larger than the A/A lane's spread.
3. **Concurrency**, three parts, all of which must hold:
   - in the probe's `concurrent-bench` mode, the credit build's nanoseconds per admitted charge is
     at or below base in every cell with two or more threads - threads 2, 4, 5 and 8, at each
     runtime ceiling measured;
   - in the probe's `concurrent-ambient` mode, the build carrying the execution-context resolution
     cache is at or below the build carrying only the pre-admission table, at every thread count of
     two or more;
   - the median duration over five runs of the concurrent exactness test alone, alternating builds,
     is at or below base on the credit build.
4. **The per-instruction meter row.** In the bench host, `meter-per-instruction` for credit is at or
   below base plus the A/A spread. That row polls after every charge, so each admitted block covers
   exactly one charge: it measures the settle and the re-admission that moved into the poll, not the
   saving the change was made for. Passing it shows only that a profile polling after every
   instruction does not regress.

**What a failure does.** A cell that fails item 2, 3 or 4 is named as failing in this bundle, and
nothing in this repository may describe the change as faster for that cell. A `concurrent-ambient`
failure triggers the resolution cache's documented fallback. A `concurrent-bench` or concurrent-test
failure triggers a change to the pre-admission table's capacity, its block cap or its share divisor.
Either way the change is made, the measurement is taken again, and **both** runs are retained - the
failing one is not dropped.

**No registered baseline funds this change.** VM-5's gate funds optimisation only against one of its
baselines, and the only baseline touching this path is the per-instruction meter row, which by item
4 cannot show the saving. The shape measurements and the core-charge measurements are bundle-local.
No new baseline is registered for this work and the figures in `docs/baselines.md` are not edited;
that gap is recorded as an exclusion in section 9 rather than repaired.

### 5.2 The conformance parity rule

Four whole-suite runs of the pinned test262 checkout: the base build and the credit build, each in
the bytecode form and in the emitted-machine-code form, every one under the wide manifest at the
default fuel allowance and a 60,000 ms wall clock.

**The rule, per form.** The sorted `result` rows of the base report and the credit report are
identical, **except** on rows in that form's predeclared list in section 5.3, where the credit row
may be `Passed` or an exhaustion of another allowance. Every other difference fails this rule and is
recorded as failing - including a credit row exhausted on `WallClock` that is not in that form's
list, and a listed row that became `Failed`.

**The comparison script is retained, not the gate.** The form-comparison script is run and its
output retained, but every row it names is checked against the list by hand. That script is written
to hold a bytecode run against a machine-code run and admits no wall-clock difference at 60,000 ms
or more, so a listed row that legitimately changed makes it exit non-zero; that exit is not itself a
failure of this rule.

**Within each build**, the bytecode-against-machine-code comparison names the same set of rows in
the base build as in the credit build, apart from rows in either form's list.

**Why the lists exist at all.** A wall-clock exhaustion is the one verdict in the report that is a
property of the machine rather than of the engine: the same variant on the same build can spend the
allowance on one run and finish on the next. Fixing the lists before the changed build runs is what
stops that verdict from being read backwards later - as a row the change repaired, or as a
regression the change caused.

### 5.3 The two base wall-clock lists

Both lists are read off the merged whole-run report of a base run. Both runs were taken from the
build in `D:/Broiler.VM-base` at commit `f127d92`, against the checkout the retained suite pin
admits - `tc39/test262` at revision `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`, whose 56,560 files
hash to the content digest that pin declares - under manifest `broiler.javascript.wide` with no
surface declined, at an allowance of 100,000,000 fuel and 60,000 ms per variant, in 56 shards across
14 processes. Each run's merged report states that it may be retained: pinned, whole, and its five
verdicts account for its variants. Every shard transcript of both runs is retained outside this
bundle until the bundle is completed.

The two forms exhausted the wall clock on the same variants, and on no others.

**Bytecode form.** Two rows.

| Path | Variant |
|---|---|
| test/staging/sm/regress/regress-1507322-deep-weakmap.js | sloppy |
| test/staging/sm/regress/regress-1507322-deep-weakmap.js | strict |

**Emitted-machine-code form.** Two rows.

| Path | Variant |
|---|---|
| test/staging/sm/regress/regress-1507322-deep-weakmap.js | sloppy |
| test/staging/sm/regress/regress-1507322-deep-weakmap.js | strict |

Nothing else in either run spent the wall clock. Every other exhaustion in both runs was on a
deterministic allowance - fuel, the nested-load fan-out or the nested-load byte count - and those
rows are held to exact equality by the rule in section 5.2 like any pass or failure, with no
tolerance at all.

### 5.4 The builds, and which run used which

Every build is Release. A binary's commit stamp - the source revision the SDK writes into its
informational version - is how a binary is tied to a commit below, where one survives.

| Build | Tree | Where it was built | Runs |
|---|---|---|---|
| Base | `f127d92` | A worktree at `D:/Broiler.VM-base`, outside the repository | Every base side of E4 to E13 |
| C2 | `e117162`: the pre-admission table without the resolution cache | A worktree at `D:/Broiler.VM-c2`, removed after E12 | The C2 column of E12's ambient-concurrency mode |
| Credit before the fallback | The product at `c98011a` (the checkout later moved to `dbc8d37`, which changed this README alone) | The main checkout | E4, E5, E6, E9 (its binaries survived, and `e9/binaries.txt` gives their stamp, `c98011a`), E7, E8, E10's first run, and every E12 file named "before-fallback" |
| Credit after the fallback | The tree of `34dbd7a`, built first before it was committed and then again after it was committed as `66efe30` | The main checkout | The gate runs in `gates/` named "fallback", E2's credit half, E10's second run (its lanes survived, and `e10/lanes.txt` gives their stamp, `66efe30`), E11, E12's files without "before-fallback", E13 and the T6 timing |
| Witnesses | `34dbd7a` with one injected defect at a time | A worktree at `D:/broiler-arms/wit`, removed afterwards | E3 |
| Oracle | `f127d92` with the head's fuel-exactness test and fixture files copied over it | A worktree at `D:/broiler-arms/oracle`, removed afterwards | E2's base half |
| Profile assemblies, deterministic | Both trees, with debug information and the commit stamp off and a path map | Output directories under `D:/Broiler.VM-e4` | E4 |
| Collector | `34dbd7a` | The main checkout | Every top-level log in this directory (section 5.7) |
| Cold builds, after a review | `f127d92` and `c98011a`, each checked out fresh | Worktrees at `D:/broiler-arms/cold-base` and `D:/broiler-arms/cold-c98011a`, removed afterwards | E1's cold builds of the base and of the product before the fallback |

No worktree was created inside the repository, so architecture rule A14's test failures inside a
worktree do not arise; every run of the architecture tests was in the main checkout.

**Where the tie between a run and its build is retained, and where it is not.** E3's transcripts
name the commit of their worktree, and so do E2's base half and the cold builds of E1. The binaries of E9 and of E10's second
run survived in the throwaway directory, and `e9/binaries.txt` and `e10/lanes.txt`, written after a
review, give each file's digest and commit stamp. For every other run - E2's credit half, E4 to E8,
E10's first run, E11, E12 and E13 - no retained file names the commit, or holds a digest of the
binary, that the run used: their transcripts carry neither, the base and C2 worktrees were removed,
and the main checkout's outputs were rebuilt after the fallback. Nor can their content tell the
builds apart where it is identical: E7's base and credit reports are byte-identical in each form,
E5's plain transcripts are byte-identical, and `e9/e9-bisect.log` tells base from credit only by the
variant name its script printed, which the binaries in `e9/binaries.txt` now back. For those runs the
table above rests on the procedure (EX-117).

### 5.5 The order in which things happened

Times are the workstation's, on 2026-09-16. Commit times are the reflog's. A run's time is its
transcript's where the transcript carries one - E2's base half, E3, E10, the T6 results and the two
machine-state files. Otherwise it is read from a file this list names: for E7 and E8, the creation
time of each run's output directory and the write times of its shard transcripts, retained in
`e7/run-times.txt` and `e8/run-times.txt`; for E12, the `started=` header of each probe file, which
the probe script writes after the probe process returns, so that each is the time a mode
**finished**, not when it began. No retained transcript of E4, E5, E6 or E9 carries a time, and
`e5/transcript-times.txt` gives only the time their copies were written, together. Every file-system
time here was read after a review, from the throwaway directory (EX-117).

| Time | What |
|---|---|
| 09:29 | `f127d92`, the base, committed |
| 09:49 to 10:51 | `e117162`, `a416c7a`, `4490eda`, `738d9c7` and `c98011a` committed |
| 10:51 to 11:09 | E4's deterministic profile builds, E9's bisections, E5 and E6 in both builds: after `c98011a`, whose stamp E9's credit binaries carry, and before 11:09, when the copies of all four runs' transcripts were written |
| 11:21 to 14:08 | The two base test262 runs, bytecode then emitted machine code, from the first run's output directory to the second run's merge |
| 14:11:59 | `dbc8d37` committed: the predeclared rule and the two base wall-clock lists |
| 14:14 to 15:29 | The two credit test262 runs. Both began after the rule was committed: the first credit run's output directory was created at 14:14:17, and none of its shard processes started before that |
| 15:30 to 17:25 | E8, the low-fuel series, from the first run's output directory to the last run's merge |
| 16:22:56 to 16:52:03 | **The main checkout was on branch `main`**, by the reflog, while E8 ran (section 5.8) |
| 17:33 | Machine state recorded; E10's first run |
| 17:42 to 17:47 | E12 before the fallback, by the times its modes finished; the ambient-concurrency part of rule item 3 fails |
| 17:52:23 | The fallback committed as `66efe30` |
| 17:54 to 18:28 | E12's ambient-concurrency mode again (finished by 17:54), E10's second run, E11, E13, the T6 timing, and E12's other modes again (finished from 18:27 to 18:28) |
| 18:28 | Machine state recorded again; `66efe30` amended in its message to `34dbd7a` |
| 18:41 to 18:45 | E3's witnesses, then E2's base half |
| 18:45 to 18:49 | The evidence collector |
| after 18:49 | This README and `hashes.txt` written, then E1b run over the finished bundle |

### 5.6 Every command, and where its transcript is

`<tree>` is `D:/Broiler.VM-base` for the base build and `D:/Broiler.VM` for the credit build. Every
transcript is retained whatever it shows.

| ID | What | Command | Retained |
|---|---|---|---|
| E1 | Build | `dotnet build Broiler.VM.slnx -c Release -warnaserror` in both trees, and the collector's own `--no-incremental` build; and, after a review, `dotnet build Broiler.VM.slnx -c Release --no-incremental -warnaserror` in a fresh worktree at `f127d92` and one at `c98011a` | `gates/build-*.log`, `gates/final-build-*.log`, `gates/rebuild-*.log`, `gates/fallback-build.log`, `build.log`, `gates/cold-build-base.log`, `gates/cold-build-c98011a.log` |
| E1a | Regenerate the generated records | The test run with the assurance and API write switches set, after the fallback | `gates/fallback-test-write.log` |
| E1b | Every test project | `dotnet test Broiler.VM.slnx -c Release` after the fallback; the collector's test step; and once more over the finished bundle | `gates/fallback-test-plain.log`, `test.log`, `gates/test-after-bundle.log` |
| E1c | The public API file | `git diff --exit-code` of `docs/api/public-api.txt` from `f127d92` to every commit of the series | `gates/public-api-diff.log` |
| E2 | The tests were the oracle | The fuel-exactness tests, filtered by class, on the credit build; and on the base meter with the head's test and fixture files transplanted onto `f127d92` | `e2/oracle-on-credit.log`, `e2/oracle-on-base.log`, `e2/oracle-transplant.txt`, `e2/test-file-diff-stat.txt` |
| E3 | Injected-defect witnesses | `measurement/run-witnesses.sh`: for each witness, `measurement/witnesses.py` applies the edit, `dotnet test src/tests/Broiler.VM.Contract.Tests -c Release --filter FullyQualifiedName~FuelChargeExactnessTests --logger "console;verbosity=normal"` runs, and `git checkout -- src/Broiler.VM.Runtime` restores the tree; a clean run before the first witness and after the last | `e3/` - one `.apply.txt` with the applied diff and one `.log` per witness |
| E4 | Profile assemblies unchanged | `dotnet build` of the JavaScript and WebAssembly profile projects in both trees with `-p:DebugType=none -p:DebugSymbols=false -p:IncludeSourceRevisionInInformationalVersion=false` and a path map, into `D:/Broiler.VM-e4/<build>/{js,wasm}/`, then `sha256sum` | `e4/` |
| E5 | JavaScript checks | `dotnet run --project <tree>/src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler -c Release -- --checks`, plain and with `--verbose` (base twice, as an A/A pair) | `e5/` |
| E6 | Corpus replay and host lifetime | `dotnet run --project <tree>/src/compositions/Broiler.VM.Composition.JavaScript.ExecutionOnly -c Release -- --corpus src/tests/corpus/js-1`, plain and with `--verbose` (verbose twice per build) | `e6/` |
| E7 | test262, whole suite, both forms, both builds | `python eng/run-test262.py --suite <pinned checkout> --binary-directory <tree>/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0 --form {bytecode,native} --wall 60000 --out artifacts/fc/t262-<build>-<form>`, base runs first; then `python eng/compare-test262-forms.py` within each build and across builds, and `measurement/e7-rule.py` applying section 5.2 per form | `e7/`: each run's driver transcript, merge transcript and gzip-compressed merged report; the four comparisons; the two rule transcripts |
| E8 | Low fuel over four subtrees | `measurement/e8-run.sh`: fuel 1,000, 2,500, 10,000, 30,000 and 100,000, both forms, both builds, `--wall 600000` and `--dir` for `test/built-ins/Promise`, `test/language/statements/class`, `test/built-ins/Array` and `test/language/expressions/object`; then the form-comparison script per (form, fuel), a sorted-row comparison and a count of fuel exhaustions | `e8/`, and `e8/off-series/` for the six runs section 5.8 explains |
| E9 | Fuel minima through the command-line host | `measurement/patch_measure2.diff` applied to the command-line host of both trees as a measurement-only change, built, copied out and reverted; `measurement/p_bisect.py` over `a_1000`, `a_2000`, `c_small` then `a_1000`, and `d_small`, both forms | `e9/` |
| E10 | Long JavaScript shapes | `measurement/measure-shapes.py`: base, credit and a byte copy of credit as the A/A lane; interleaved shape, lane, form; 2 warm-ups and 7 repetitions; `--wall 600000 --fuel 1000000000000`, `--native x86-64-win64` for the native form; `invoke_ms` read from the E9 patch applied to both trees, `process_ms` from `time.perf_counter` | `e10/`, one run before and one after the fallback |
| E11 | Octane | `python eng/run-octane.py --binary-directory <tree>/src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0 --only richards --only deltablue --only crypto --only raytrace --only navier-stokes --report <file>`, five repetitions alternating base then credit | `e11/` |
| E12 | Core charge cost, traces, concurrency | `measurement/run-probe.py` over the probe in `measurement/probe/`, which binds `VmBudgetLevel.FuelPreAdmissions` and `VmFuelPreAdmissions.SettleAll` by reflection; modes `trace`, `stress`, `bench`, `concurrent-bench` (base and credit) and `concurrent-ambient` (base, C2 and credit); threads 1, 2, 4, 5 and 8; runtime ceilings unbounded, 2<sup>20</sup> and 4 x 2<sup>20</sup>; each thread's own meter with poll bound 65,536, polling every 16,384; minimum of 3 | `e12/` |
| E13 | Fixture benchmark, and T6 alone | `dotnet run --project <tree>/src/tests/Broiler.VM.Bench.Host -c Release`, two pairs (base first, then credit first); `measurement/measure-t6.py --reps 5` for the two-thread shared-ceiling test alone, alternating builds, reading each run's own duration from its TRX result | `e13/` |

### 5.7 How this directory was collected

**The collector was tried first, and it ran.** `python eng/collect-evidence.py --bundle VM-5-002 --out
docs/evidence/vm-5-002 --rebench --skip-controls`, from the main checkout at `34dbd7a`, wrote every
top-level log here: `build.log`, `test.log`, `pack.log`, `nuspecs.txt`,
`publish-jit-and-trimmed.log`, `publish-aot.log`, `corpus-replay.log`, `fuzz.log`, the two
`composition-*.log` with their `catalog-*.txt` and `closure-*.txt`, `soak.log`, `bench.log`,
`feed-consumer.log` and `environment.txt`. Its console output is `collector.log`. The negative
controls were skipped, as the design asks.

**The collector does not know E1 to E13**, so their transcripts were hand-retained from the
throwaway directory they were written to, as bundle JSB-11-001 did: text normalised to LF, which is
how the repository stores it, and each merged test262 report compressed with gzip at a zero timestamp
and with no embedded name, so that the compressed bytes depend on the report alone. The scripts,
shapes, patch and probe those runs used are under `measurement/`; the probe's project file carries a
`.txt` suffix so that no build and no architecture rule takes it for a project of this component.
Section 5.3 says the shard transcripts of the two base runs were kept outside this bundle until it
was completed; they are not retained here, for any of the test262 runs, because each merged report
carries every row its shards wrote and the driver and merge transcripts are retained beside it.

**`hashes.txt` was then rewritten**, by `measurement/make-hashes.py`, replacing the one the collector
wrote, which is not retained. It keeps the set the collector hashes, adds the files design section 8.4
names with the drivers and pins the runs used, adds every file in this directory apart from this
README and itself, and records the digest of each uncompressed test262 report. Every digest of a
tracked file is of the blob the repository stores at the retained commit, and the script refuses a
working copy that differs from it in anything but line endings. That refusal mattered once: the
working copy of `VmExecutionScope.cs` carried CRLF line endings although the repository stores the
file LF - `git status` compares after normalising and showed nothing - so the collector, which reads
working-copy bytes, had hashed bytes the repository does not hold. `hashes.txt` names that file in
its last section. As `c9afb0d` committed it, `hashes.txt` also named `docs/baselines.md`; section 8
says why that row was later withdrawn.

**One thing the collector did outside this directory**, and it was undone: its test step rewrote
`docs/evidence/vm-6/d1-outcome.txt`, which was restored with `git checkout`. Nothing else outside
this directory changed, and `docs/baselines.md` did not change.

### 5.8 Where the collection departed from the design, and why

1. **The base is `f127d92`, not the commit the design names**, for the reason section 1 gives.
2. **The credit build is `34dbd7a`, not the design's C3.** The ambient-concurrency part of rule item 3
   failed on `c98011a`'s product, and the rule names the resolution fallback as what that failure
   triggers. Both runs are retained: every E12 file named "before-fallback" is the failing run, and
   E10 is retained from both builds too.
3. **E2 is not an empty diff.** The design asks that the test file be unchanged since the commit that
   pinned it. It is not: `ba760d9` (before the base), and `4490eda` and `c98011a` (after it) changed
   it, as `738d9c7`'s message corrects. So the oracle was established the way that message says it
   must be stated: the head's test file and the three fixture files it depends on were copied onto
   the base commit in a worktree, with the runtime untouched, and the tests ran on the base meter.
   `e2/oracle-transplant.txt` records the copy and its digests.
4. **E3 was run at collection time, on `34dbd7a`.** The witnesses were run before `e117162` and
   `a416c7a` landed, but no transcript of those runs was kept. One of them was already a finding then:
   the witness that deletes the settle in the remaining-allowance reader passed the test written for
   it, and `4490eda` changed that test so that it fails. After the fallback, the design's own fallback
   clause asks for the witnesses again. So all of them ran here, with a transcript each, against the
   head as it is committed. Two changes to the design's table: the witness for the ambient lookup
   was re-expressed for the thread-static resolution (it drops the context comparison, which is what
   it dropped before), and two witnesses were added - W11 and W12 - for the two arms `c98011a` wrote
   tests for and named as defects those tests catch. The design also asks for a passing transcript
   after each witness is reverted; the script kept none per witness. It restored the tree after
   each, checked that `git status` was empty, and ran the class once clean before the first witness
   and once after the last (section 6.2).
5. **E4 was not re-run after the fallback.** The fallback changed `VmExecutionScope` alone, in the
   runtime assembly, which neither profile assembly references, and `git diff` from `c98011a` to
   `34dbd7a` over both profiles, `Broiler.VM.Abstractions`, `Broiler.VM.Binary` and
   `Directory.Build.props` is empty. The outputs built before the fallback were hashed at collection.
6. **E5 to E9 ran on the product before the fallback and were not re-run after it** (EX-116). The
   design's fallback clause asks for E1 to E4, E12 and the T6 timing again, and not for them.
7. **E8's series was driven for half an hour from another branch's driver** (EX-115). From 16:22:56 to
   16:52:03 the main checkout was on `main`, by the reflog, which records the checkout and not what
   made it. `eng/run-test262.py` is read from the checkout when each run starts. Four bytecode
   runs - fuel 30,000 and 100,000, base and credit - started in that window and were driven by
   `main`'s driver, which takes a bytecode run under the wide manifest as this branch's does, and their
   reports say so. Six native runs - fuel 10,000, 30,000 and 100,000, base and credit - started in the
   same window and were taken under the numeric manifest, which `main`'s driver chooses for a native
   run given none. They are not the series: they are retained in `e8/off-series/` with a name that
   says so, and the native series at those three fuel values was run again under the wide manifest
   after the branch was restored, from 16:52 to 17:25. `e8/e8-driver.log` is the series driver's own
   transcript, so its exit codes for native fuel 10,000 and above are the off-series runs'. The credit
   binaries the four bytecode runs used were built on this branch before the window; that rests on
   file timestamps alone - no Release build output in the checkout carries a time inside the window -
   and not on a stamp in the reports, which carry none.
8. **E10 read `invoke_ms`**, from the measurement-only patch E9 uses, applied to both trees, built,
   snapshotted and reverted at once. `process_ms` is retained beside it. The A/A spread each verdict
   uses is the largest of the credit and A/A medians' difference and the two lanes' ranges.
9. **E11 is bytecode only**, because `eng/run-octane.py` has no form flag. `e11/trial-richards-credit.*`
   is a sizing run, not a repetition.
10. **E12's first `trace`, `stress` and `bench` runs were overwritten.** They wrote numbers with this
    workstation's decimal comma; the probe was changed to write invariant numbers and the three modes
    were run again over the same files. No log of the overwritten run survives, so what it showed is
    not retained: `collection-notes.txt` says only that its stress mode passed and its traces
    matched, and no longer quotes the figures it first quoted (section 5.9). `e12/trial-conc-base.txt` is the first complete base run of `concurrent-bench`, taken to
    size the workload, and is retained beside the base run that counts.
11. **The collector's test step failed two architecture tests** for a reason outside the product: a
    copy of the probe's project file sat under the git-ignored `artifacts/` directory, and group A
    rule A14 holds every project file in the checkout outside the solution to be a sample. The copy
    was renamed, and E1b was run once more over the finished bundle. Both transcripts are retained.
12. **The collector's feed-consumer step could not publish the consumer as Native AOT**: the
    toolchain discovery EX-42 records failed with MSB3073, because that step runs no `vcvars64`
    shell. Every other step of it, and the collector's own Native AOT publishes, succeeded.

### 5.9 What later commits changed, after a review

A review of the committed bundle found places where it said more than its files show. Every change
below was made by a commit after `c9afb0d`. None of them re-ran a measurement or replaced a retained
transcript, and git holds the text each one replaced.

1. **`docs/baselines.md` was withdrawn from `hashes.txt`**, after the records commit's note to the
   register hit this bundle's first recertification trigger. Section 8 says why, and that the bundle
   stood expired until the row was withdrawn.
2. **What the throwaway directory still showed about times and builds was retained**:
   `e5/transcript-times.txt`, `e7/run-times.txt`, `e8/run-times.txt`, `e9/binaries.txt` and
   `e10/lanes.txt`, written by `measurement/list-run-identity.py` from file-system times and from
   the binaries that survived. Section 5.5's times now name their source and E12's are named as
   finish times, the stamps section 5.4 cited are now in those files, and what could not be recovered
   is EX-117.
3. **The build clause of rule item 1 gained cold builds of the base and of the product before the
   fallback.** The transcripts section 6.1 read it from were incremental and record no command, so
   they could not show a warning; `gates/cold-build-base.log` and `gates/cold-build-c98011a.log`
   were run after the review, with the collector's `--no-incremental -warnaserror` command, in fresh
   worktrees outside the repository.
4. **Rule item 1's witness clause is read as failing on W12 as well as W8**, where the verdict table
   and EX-114 had counted W8 alone while section 6.2 already marked W12 `[UNMET]`. The W5 row now says
   that W5 passed its test before `4490eda` amended it, and the verdict no longer says each witness
   passed again when reverted, since no witness has a passing run of its own.
5. **Four statements were narrowed to what the files show.** `4490eda` is described as the amendment
   to T9 it is, not as a further test. Section 6.5 no longer compares the single-threaded `bench`
   figures across builds, since no rule item reads that row. EX-110 and section 7.2 say that W2 does
   remove the resume-path pair, and that no test fails for it. EX-108 names which consumption figures
   a run compares and which rest on argument.
6. **`collection-notes.txt` was corrected**: the overwritten probe run's figures, which no log backs,
   are removed; the paths it gave for the fallback's gate logs, the probe's source and the E10 lanes
   now name where those files are, or say they are not retained; and its CPU-time figures, like
   section 4's, are lower bounds, since the second machine-state snapshot lists two processes the
   first does not.
7. **The header says the bundle is collected.** `c9afb0d` kept `dbc8d37`'s header, whose collection
   line called the bundle incomplete and said every other section was waiting; that line and the
   paragraph on why `dbc8d37` came first are now quoted, as that commit wrote them, under the header.
8. **Section 8's first trigger names the one kind of change that does not trip it**: a correction to
   a file this directory retains, made by a commit that regenerates `hashes.txt` with it and is listed
   here. Items 1, 2, 3 and 6 are such corrections. A change to any tracked file in the first three
   sections of `hashes.txt` still trips it.

### 5.10 The owner-directed remedy

This section and section 5.11 were committed after the remedy's product and test commits and before
any run of the evidence collected again for it. Nothing in sections 5.1 to 5.9 is changed by them.

**The remedy is directed by the owner after seeing the result.** The result is this bundle's first
collection, as section 7.3 reads it: rule item 1 failed on its witness clause, because witnesses W8
and W12 passed tests they were meant to fail, and rule item 4 failed in both bench-host pairs. After
seeing that, on 2026-09-16, the owner directed that both failures be fixed before any per-block
commit, that only the evidence the fix expires be collected again, and that the failing first runs
be kept beside the new ones. The rule of section 5.1 names no change for a failure of item 1 or of
item 4, so the remedy is not a change that rule triggers: it is made under the owner's direction.

**What the owner decided, and what the owner did not.** The owner directed that the two failures be
fixed. The owner did not choose how. **The departure from the default "`Poll` re-admits", the change
to the capability boundary, and item 5 are not decided by the owner.** The remedy's design asked for
the owner's word on each of those three before its product commits were written, and the questions
were not put to the owner: the product commits were written on the direction of an orchestrating
agent, which is not the owner's decision. For that reason item 5 of section 5.11 is reported without
a verdict.

**The commits of the remedy**, on branch `claude/fuel-credit-block-steps`, after `fc00248`:

| Commit | Subject | What it carries |
|---|---|---|
| `4279862` | Witness a context change on one thread, a suppressed lookup after another, nested and context-changing capabilities, and a poll after a refused poll | Tests T19 to T23 in the fuel-exactness class; corrected remarks for T7, T12 and T18; and, in the review-regression class, a variant of the per-thread leak test whose capability returns under a different context object. The product is `fc00248`'s |
| `9e9377d` | Count a held block's work at a poll instead of settling and renewing it there | Part A, below; the remarks of T2b, T4, T8, T9 and T14; regenerated assurance and review records |
| `965e6ad` | Return a capability's caller to the context it entered from when the capability left it unchanged | Part B, below; regenerated assurance and review records |
| `542c715` | Witness a host call made with the flow suppressed, a lookup through another runtime's scope, a poll beside a second charging thread, and a capability return that puts its caller's context back | Tests T24 to T26, a second row of T23, a value of its own for each invocation of T21, and a test in the review-regression class that a capability which changes nothing returns its caller to the very context object it was called under. The product is `965e6ad`'s |
| `16e3d6d` | Say that any settle ends a held block, and that a capability's return puts no context back when its entry recorded none | Remarks of `VmMeter.BlockCap`, `VmMeter.Poll` and `VmRuntime.LeaveCapability`, and one falsifier clause moved from `VmRuntime.EnterCapability` to `VmRuntime.LeaveCapability`. No code |

**Part A, the poll.** `VmMeter.Poll` no longer settles the polling meter's block and no longer takes a
fresh one. It reads the block once, under the meter's gate, and decides the uncharged-work bound on
the counter plus what the block has admitted since this meter last polled; where the count is reset
it records what the block had admitted, in a new gate-only field of `VmMeter`. A refused poll writes
nothing. `VmMeter.CommitPreAdmittedFuelLocked`, which every settle and every eviction of a block goes
through, folds into the counter only what the block admitted since that record, and clears the record
with the block. No fuel commit site is added or moved, and the settles
of every reader of consumption or of what is left are unchanged. A block ends when it is settled or
when a charge of its own does not fit what is left, and the next locked fuel charge begins a new
one. The remarks of `VmMeter.BlockCap`, `VmMeter.UnpolledWorkExceedsBound`,
`VmFuelPreAdmissions.MaxBlock` and the comment in `VmRuntime.GetBudgetSnapshot` say so. **This
departs from the design default "`Poll` re-admits"**, which the owner's answers to that design left
standing and which this bundle's first collection measured.

**Part B, the capability boundary.** `VmRuntime.EnterCapability` captures the thread's execution
context before it raises the capability depth, and the context the depth write installed, and
returns both as a `VmRuntime.CapabilityEntry`. `VmRuntime.LeaveCapability` puts the entry context back
and returns when the thread still runs under the context the depth write installed and the entry
recorded one; in every other case it writes the depth back as before. `VmRuntime.EnterProviderCall`
and `VmRuntime.LeaveProviderCall` carry the entry for a provider call, `VmCapabilityInvoker.TryEnter`,
`Leave`, `Invoke` and `InvokeBytes` carry it for a capability call, and `VmArtifactLoadMediator.Answer`
carries it from enter to leave. `VmExecutionScope` is unchanged.

**What the remedy did to this bundle.** Its product commits changed files `hashes.txt` names and the
metering path, so they hit the first two recertification triggers of section 8: **from `9e9377d`
on, this bundle is expired**, and it stays expired until the commit that retains the re-collection
regenerates `hashes.txt` against the remedy's head. The first collection's files, its verdicts and
sections 5.1 to 5.3 stay exactly as they are; the re-collection is retained beside them, under
`r2/`.

### 5.11 How the re-collection is read, committed before any run of it

The rule of section 5.1 is not changed. The re-collection is read against it item by item, as
follows, and this reading is committed before any run of the re-collection. "The remedy head" is
`16e3d6d`, the last product and test commit of the remedy; "the base" is `f127d92`, as in section 1.

**What was known when this reading was fixed.** The readings of items 4 and 5 below were written after
the first collection's result was seen, under the owner's direction; they are predeclared relative to
the re-collection only. And every witness of the table below was driven on the remedy's commits
before this section was written - once at `965e6ad` and once at `16e3d6d`, as development checks whose
transcripts this bundle does not retain - so their results on the remedy head were known when the
witness clause was fixed. The re-collection runs them again after this commit.

1. **Item 1, every clause, on the remedy head.**
   - *Build:* a cold `dotnet build Broiler.VM.slnx -c Release --no-incremental -warnaserror` of the
     remedy head in a fresh worktree outside the repository.
   - *Every test project passes:* `dotnet test Broiler.VM.slnx -c Release` at the remedy head.
   - *Records regenerated and the public API file unchanged:* a test run of the architecture and
     contract test projects with the assurance and API write switches set, then a plain run of both;
     and `git diff --exit-code` of `docs/api/public-api.txt` from the base to every commit of the
     series, the remedy's included.
   - *The fuel-exactness tests pass on both builds:* the class at the remedy head, and the same test
     file with the three fixture files `4490eda` changed, taken from the remedy head and copied onto the
     base, on the base meter. The test file is not unchanged since the first collection - the remedy's
     test commits changed it, and its diff from `fc00248` is retained - so this clause reads the file
     as it stands at the remedy head on both meters.
   - *Every injected-defect witness fails the test it names and passes again when reverted:* every
     witness in the table below, each applied alone in a worktree outside the repository at the remedy
     head, with the fuel-exactness class, both per-thread leak tests and the restore-path test run
     under the defect; then the tree restored and the same tests run clean; then the named test run
     alone, and its passing transcript retained. The same tests run clean once before the first
     witness. A sampled witness (W4, W24) is read as the first collection read W4: its test alone is
     run again, up to ten runs in all, until it fails, and every run is retained. **A witness that
     passes its named test or row is a failure of this clause**, recorded as such and not rewritten.
   - *Both profile assemblies byte-identical:* **satisfied by an empty source diff instead of a
     rebuild.** `git diff --stat c98011a 16e3d6d` over every project in the reference closure of the
     JavaScript and WebAssembly profile projects, and over `Directory.Build.props`,
     `Directory.Packages.props`, `global.json` and the property files under `eng`, is retained, and
     so is the diff from `c98011a` to `34dbd7a` that section 5.8 item 5 describes. `c98011a` is the
     tree the assemblies of E4 were built from. If either diff is not empty, E4 is rebuilt. This
     substitution is part of this reading and is committed with it.
   - *The JavaScript checks, the corpus replay and host lifetime:* the credit transcripts of the
     re-collection, plain and verbose, against the base transcripts the first collection retained, with
     the same masked comparison.
   - *Conformance parity:* section 5.2 applied to two credit whole-suite runs of the re-collection,
     one per form, against the two base reports and the two lists of section 5.3 retained from the
     first collection, with the hand rule and the four comparisons.
   - *Low fuel:* the credit runs of the re-collection at fuel 1,000, 2,500, 10,000, 30,000 and 100,000
     in both forms, driven by this branch's test262 driver, against the base reports the first
     collection retained, pair by pair - including the two base bytecode reports section 5.8 item 7
     says another branch's driver took. A pair not re-run is named as not re-collected, and the clause
     is not met for it.
   - *Fuel minima:* the minima the command-line host gives for the first collection's four programs on
     the remedy head in both forms, against the base minima the first collection retained.
2. **Item 2**, as section 5.1 states it, from a new run of all three lanes: the base with the
   measurement patch of E9, the remedy head with the same patch, and a byte copy of the second as the
   A/A lane.
3. **Item 3**, as section 5.1 states it, from new runs, with these meanings:
   - "the credit build" is the remedy head;
   - "the build carrying only the pre-admission table" is **C2′**: the remedy head with
     `src/Broiler.VM.Runtime/VmExecutionScope.cs` replaced by its blob at `e117162`, identified by that
     blob's id and by the digest of the runtime assembly built from it. C2′ is not a commit;
   - `concurrent-bench` and `concurrent-ambient` are read against a new base run, and T6's median over
     five runs alternating builds against a new base run.
4. **Item 4**, the per-instruction meter row, in two pairs of bench-host runs, the first base first and
   the second credit first. Two readings, and **both must hold in both pairs**:
   - *lane total:* the credit `candidate-ns` of `meter-per-instruction` is at or below the base
     `candidate-ns` plus the base `aa-ns` of the same pair;
   - *per instruction:* the credit `per-instruction-ns` is at or below the base `per-instruction-ns`
     plus the base `aa-ns` of the same pair divided by 256, the instructions one invocation of the row
     charges.
5. **Item 5, the host-call row**, which the owner's direction names as one of the two failures seen
   and which no item of section 5.1 reads. In each pair, the reading is whether the credit
   `per-call-ns` of `host-call` is at or below the base `per-call-ns` plus the base `aa-ns` of that
   pair. **It is reported without a verdict**, because the owner has not decided that it is an item
   (section 5.10).

**How a run is tied to its build.** Every transcript of the re-collection begins with lines its driver
writes before the run: the branch and head of the main checkout, the tree the run used and that tree's
head, which build it is, the start time, and the SHA-256 of `Broiler.VM.Runtime.dll` and of the host
executable the run loads. The collector writes its own logs and no such lines, so a wrapper writes the
same fields to a file of their own immediately before and after it runs.

**What a failure does.** A cell that fails is named as failing, and nothing in this repository may
describe the change as faster for it. No further change is made without the owner. Every run of the
first collection stays where it is, beside the new one, and its verdict in section 7.3 stands as the
first collection's.

---

## 6. Outputs

**What may be quoted here.** Core and fixture figures - the probe's (E12), the bench host's (E13) and
the T6 timing - are quoted, each equal to its value in the retained file named beside it, because
rule H5 holds a quoted figure to a log. The JavaScript shape measurements (E10), the Octane reports
(E11), the fuel minima of the JavaScript programs (E9) and every test262 report (E7, E8) are retained
and **not quoted**: the core's release gate 8 admits no language performance claimed or implied, and
its gate 11 lets a record say that a form exists and not what it is worth, which is how bundle
JSB-11-001 section 6 reads them too. For those items this section gives the verdict and the file, and
no number.

### 6.1 Correctness, E1 to E9

| ID | Result | Where |
|---|---|---|
| E1 | **Clean with warnings as errors in both trees, shown by cold builds.** The collector's `--no-incremental -warnaserror` build of `34dbd7a`, and - run after a review, each in a fresh worktree outside the repository with the collector's command - the same cold build of `f127d92` and of `c98011a`, whose product the runs before the fallback used: every project of the solution built, with 0 warnings and 0 errors, in each of the three. The other build transcripts in `gates/` also read 0 warnings and 0 errors, but none records its command and several finished in about two seconds: they are incremental builds that may have compiled nothing, and a project that is not compiled emits no warning, so they are not the evidence for this item | `build.log`, `gates/cold-build-base.log`, `gates/cold-build-c98011a.log`; the incremental builds in `gates/*build*.log` |
| E1a | The regeneration run rewrote the generated records; within that same run two review-record rules (H3 and H4) failed, because they compared the record as it stood before the rewrite. The plain run that followed passed both test assemblies. `HUMAN_REVIEW.md` still reads PENDING | `gates/fallback-test-write.log`, `gates/fallback-test-plain.log` |
| E1b | After the fallback, both test assemblies passed. **The collector's test step failed two architecture tests** - both rule A14 rows, on the copy of the probe's project file under `artifacts/` (section 5.8, item 11) - and passed every other test. The run over the finished bundle is in `gates/test-after-bundle.log` and section 6.8 reads it | `gates/fallback-test-plain.log`, `test.log`, `gates/test-after-bundle.log` |
| E1c | `docs/api/public-api.txt` is byte-identical between `f127d92` and each of the seven commits of the series; `git diff --exit-code` exits 0 for every one | `gates/public-api-diff.log` |
| E2 | Both halves pass. On the base meter, with the head's test file and the three fixture files it depends on transplanted onto `f127d92`, every fuel-exactness test passes; on the credit build after the fallback, every one passes - in a transcript that carries neither a commit nor a time, and is tied to that build only by being one of the fallback's gate runs (EX-117). The test file is not unchanged since the commit that pinned it (section 5.8, item 3), and `e2/test-file-diff-stat.txt` names the commits that changed it after the base | `e2/` |
| E3 | Section 6.2 | `e3/` |
| E4 | All seven pairs equal: `Broiler.VM.Abstractions`, `Broiler.VM.Binary` and `Broiler.VM.Profile.JavaScript.Format` and `Broiler.VM.Profile.JavaScript` in the JavaScript output, and `Broiler.VM.Abstractions`, `Broiler.VM.Binary` and `Broiler.VM.Profile.WebAssembly` in the WebAssembly output | `e4/e4-hashes.txt` |
| E5 | Every row passes in both builds, with the same two System V rows not run on this Windows x64 machine in both. The verbose transcripts of the two builds differ on 27 lines, and so do two verbose runs of the base build against each other; with elapsed times and addresses masked, both pairs are identical line for line, including every fuel row | `e5/`, and `e5/masked-compare.log` from `measurement/masked-compare.py` |
| E6 | Every check passes in both builds, including the corpus replay and the host-lifetime rows for two siblings under one aggregate parent. The verbose transcripts differ only in the heap figures of the recycled-runtime plateau row; with those masked they are identical, across the builds and within each | `e6/`, and `e6/masked-compare.log` |
| E7 | **The parity rule of section 5.2 holds in both forms.** In each form every sorted `result` row of the credit report equals the base row; no row differs, so none needed the predeclared list, and no credit row is exhausted on the wall clock outside it. The comparison script, run within each build and across the builds in each form, reports no differing variant in any of the four comparisons, so the within-build comparison names the same (empty) set of rows in both builds. All four merged reports state that they may be retained | `e7/` |
| E8 | **Holds for every (form, fuel).** For all ten pairs the sorted `result` rows are identical and the comparison script exits 0 with no differing variant. At fuel 1,000 and 2,500 both reports of each pair carry fuel exhaustions, the same count on both sides, so the low runs exercised what they were meant to. Every series driver transcript says the run may not be retained as a whole-suite figure, because a named selection is partial coverage; that is the driver's statement about coverage and not a failure of this item. Section 5.8, item 7, and EX-115 bound what this shows | `e8/` |
| E9 | **The minima are identical in both builds and both forms** for all four programs. One unit below each minimum the budget line reads `outcome=ResourceExhaustion reason=AllowanceExhausted dimension=Fuel scope=Runtime` with runtime fuel consumed one below the minimum, and the command-line host exits 5 naming `AllowanceExhausted on Fuel`, in every build and form | `e9/e9-bisect.log` |

### 6.2 The injected-defect witnesses

Each witness ran once, against `34dbd7a`, with the class's tests filtered in. A clean run before the
first witness and after the last passes every test, and `git status` is empty after every revert.
**No witness has a passing run of its own after its revert**: that each passes again when reverted
rests on the restored tree being the one both clean runs passed on, not on a run per witness. W11 and
W12 were added after the rule was committed; its witness clause reads "every injected-defect
witness", so both are read against it like the rest.
The T numbers are the ones the test file's own section comments use. (`e3/driver.log`'s one-line
summaries are empty: they grep for a summary format the normal-verbosity logger does not print. The
per-witness logs are the record.)

| Witness | Injected defect | Must fail | What failed | Verdict |
|---|---|---|---|---|
| W1 | The settle of every holder in the runtime's budget snapshot deleted | T4 | T4, and nothing else; T5 passes | `[MET]` |
| W2 | The settle in the uncharged-work reader and both step-end settles deleted together | T8, row 62 units | That row, and nothing else | `[MET]` |
| W3 | The settle in `Poll` deleted, with the re-admission it guards | T8, row 10, 60, 60 | That row, and 23 more rows; the failing rows fall in fifteen tests, T8 among them | `[MET]` |
| W4 | `LeavesRoomFor` answers true | T6 | T6, on the first run, so no further iteration was needed | `[MET]` |
| W4b | The uncharged-work reader's settle, the invocation-path step-end settle and the settle-all branch of the locked charge deleted | T2b | T2b; and T8's 62-unit row and T6 as well | `[MET]` |
| W5 | The settle in the remaining-allowance reader deleted | T9 | T9, and nothing else. **Not so at first**: run before `e117162` landed, with no transcript kept, this witness passed T9 as T9 then stood, and `4490eda` changed T9's profile variant so that it fails (section 5.8, item 4). This verdict is of T9 as amended, before the rule was committed | `[MET]` |
| W6 | The aggregate-parent test in `PreAdmit` deleted | T11 | T11, and nothing else | `[MET]` |
| W7 | The fast-path head returns false instead of calling the locked charge | T1 | Every test in the class, T1 among them | `[MET]` |
| W8 | The ambient lookup returns the thread's resolved meter without comparing contexts | T12 and T7 | **T12 only. T7 passes with the defect in place** | `[UNMET]` |
| W9 | The settle of every holder before a retention's admission check deleted | T14, both rows | Both rows, and nothing else | `[MET]` |
| W9b | That settle narrowed to the charging meter | T14, the two-holder row | That row, and nothing else | `[MET]` |
| W10 | The dimension test removed from the fast-path head | T16 | T16, and eight more rows in seven more tests | `[MET]` |
| W11 | An eviction drops its victim's block instead of committing it (added, from `c98011a`) | T17 | T17, and nothing else | `[MET]` |
| W12 | A lookup with the flow suppressed is resolved and held under the null context (added, from `c98011a`) | T18 | **Nothing. Every test passes with the defect in place** | `[UNMET]` |

**Why W8 and W12 no longer fail what they were written against - argued here, not tested.** Both tests were written while the
resolved meter was one field on the scope, which every thread charging through that scope shared. A
lookup that skipped the context comparison could then return another thread's meter, and a lookup
cached under the null context could hand one thread's meter to another thread with its flow
suppressed; T7 and T18 each put a second thread in exactly that position. The fallback holds the
resolved scope, context and meter per thread, so a second thread never sees the first thread's
triple, with or without the defect. What the context comparison still guards is a change of context
on one thread, and T12 witnesses that. What the rule against holding a null-context lookup still guards
is confined to one thread, as the scope's own remarks describe, and no test in the class reaches it. Neither
table row is rewritten here: both are failures of rule item 1's witness clause, and both are EX-114.

### 6.3 The JavaScript shapes, E10

Retained and not quoted. **In both runs, before and after the fallback, all eight cells hold rule
item 2**: for each of the four shapes in each form, the credit median of `invoke_ms` is below the base
median and the gap is larger than the A/A spread. The same holds for `process_ms`. Each run's log
ends with one verdict line per cell and quantity, and the JSON carries every repetition.

### 6.4 Octane, E11

Retained and not quoted. Ten repetitions, bytecode form, alternating base and credit, all against the
tree of `34dbd7a`. Nine exited zero for all five benchmarks. **The last credit repetition,
`e11/octane-credit-r4`, did not**: raytrace and navier-stokes exited 6, the command-line host
reporting each benchmark's file absent from the driver's scratch extraction of the pinned archive,
while the benchmarks before them in the same repetition ran. The cause was not established. The
report is retained as it was written, and the repetition is not replaced.

### 6.5 The core probe, E12

Every figure below is nanoseconds per admitted charge, the minimum of three repetitions, copied from
the named file.

**`trace`.** The traces of the base build and of both credit builds are identical line for line
after the probe's own header lines: every seed's hash, admitted count, refusing dimension and scope,
and consumption at every level. `e12/probe-trace-*.txt`.

**`stress`.** Ten rounds pass out of ten in the base build and in both credit builds, with no failure
reported by the per-meter, scope, exactness or observer checks. `e12/probe-stress-*.txt`.

**`bench`**, one thread, one runtime:

| File | Direct charge | Through the ambient meter |
|---|---|---|
| `e12/probe-bench-base.txt` | 14.36 | 27.04 |
| `e12/probe-bench-c3-before-fallback.txt` | 3.12 | 5.76 |
| `e12/probe-bench-c3.txt` | 3.37 | 8.24 |

No rule item reads this row.

**`concurrent-bench`**, each thread charging through its own meter on one runtime level
(`e12/probe-concurrent-bench-base.txt`, `-c3-before-fallback.txt`, `-c3.txt`):

| Threads | Runtime ceiling | Base | Credit before the fallback | Credit after the fallback |
|---|---|---|---|---|
| 1 | unbounded | 14.48 | 4.79 | 3.56 |
| 1 | 2^20 | 14.47 | 3.45 | 3.41 |
| 1 | 4x2^20 | 14.44 | 3.45 | 3.42 |
| 2 | unbounded | 26.56 | 1.76 | 1.75 |
| 2 | 2^20 | 24.36 | 1.76 | 1.71 |
| 2 | 4x2^20 | 22.37 | 1.73 | 1.81 |
| 4 | unbounded | 42.62 | 0.93 | 1.01 |
| 4 | 2^20 | 41.41 | 0.89 | 0.89 |
| 4 | 4x2^20 | 40.59 | 0.90 | 0.91 |
| 5 | unbounded | 42.78 | 1.36 | 1.41 |
| 5 | 2^20 | 43.21 | 1.53 | 1.45 |
| 5 | 4x2^20 | 42.72 | 1.46 | 1.50 |
| 8 | unbounded | 43.39 | 1.51 | 1.57 |
| 8 | 2^20 | 42.61 | 1.55 | 1.66 |
| 8 | 4x2^20 | 43.77 | 1.49 | 1.63 |

Every cell reports its runtime consumption equal to what was admitted, and no fault. With two or more
threads the charge counts are per thread against a shared level, so a figure below the one-thread
figure is a throughput per admitted charge across all threads, not a cheaper single charge.

**`concurrent-ambient`**, every thread charging through one ambient meter over one execution scope,
each having entered the scope with its own meter. The first three figure columns are the first run
(`-before-fallback.txt`); the last three are the run after the fallback commit:

| Threads | Runtime ceiling | Base | C2 | Credit, shared field | Base, again | C2, again | Credit, thread-static |
|---|---|---|---|---|---|---|---|
| 1 | unbounded | 21.18 | 8.65 | 5.67 | 20.95 | 8.62 | 7.55 |
| 1 | 2^20 | 21.30 | 8.59 | 5.61 | 20.93 | 8.54 | 7.44 |
| 1 | 4x2^20 | 21.23 | 8.70 | 5.60 | 20.91 | 8.48 | 7.47 |
| 2 | unbounded | 36.69 | 4.34 | 13.90 | 39.12 | 4.44 | 3.96 |
| 2 | 2^20 | 37.80 | 4.38 | 17.01 | 31.95 | 4.40 | 3.87 |
| 2 | 4x2^20 | 44.85 | 4.47 | 15.02 | 38.42 | 4.39 | 3.83 |
| 4 | unbounded | 73.29 | 2.50 | 20.27 | 74.42 | 2.43 | 2.22 |
| 4 | 2^20 | 73.61 | 2.27 | 22.37 | 72.95 | 2.26 | 1.96 |
| 4 | 4x2^20 | 73.03 | 2.35 | 21.15 | 74.97 | 2.26 | 2.02 |
| 5 | unbounded | 75.06 | 4.39 | 29.84 | 77.29 | 4.22 | 3.17 |
| 5 | 2^20 | 78.16 | 4.26 | 31.02 | 78.13 | 3.87 | 3.54 |
| 5 | 4x2^20 | 78.43 | 3.79 | 30.41 | 80.27 | 4.03 | 3.44 |
| 8 | unbounded | 77.06 | 4.70 | 29.71 | 76.49 | 4.74 | 4.05 |
| 8 | 2^20 | 78.14 | 4.62 | 31.11 | 76.90 | 4.67 | 4.06 |
| 8 | 4x2^20 | 76.37 | 4.76 | 30.31 | 75.53 | 4.67 | 4.08 |

In the first run the credit build with the shared field is above C2 in all twelve cells with two or
more threads: the failure of rule item 3's second part that triggered the fallback. In the run after
the fallback the credit build is below C2 in all twelve.

### 6.6 The fixture bench host, E13, and the T6 timing

The `meter-per-instruction` row of each run, copied from its `measurement` line. `candidate-ns` is
one invocation with fuel charging, `per-instruction-ns` the charging cost per instruction the host
derives, and `aa-ns` the host's A/A figure for the candidate. Every run reports all ten measurements
valid, every A/A lane inside its effect.

| Pair | File | `candidate-ns` | `per-instruction-ns` | `aa-ns` |
|---|---|---|---|---|
| 1, base first | `e13/bench-base.log` | 23862.9 | 26.9933 | 121.3 |
| 1 | `e13/bench-credit.log` | 30457.1 | 49.7851 | 44.5 |
| 2, credit first | `e13/bench-credit-run2.log` | 29780.2 | 46.9146 | 321.5 |
| 2 | `e13/bench-base-run2.log` | 23495.9 | 27.3083 | 119.0 |

**In both pairs the credit row is above base by far more than either lane's A/A figure**, so rule item
4 fails in both. This is the row that polls after every charge: each block covers one charge, and the
row measures the settle and the re-admission that moved into `Poll`. It shows that a profile polling
after every instruction pays more on the credit build, which is exactly what item 4 was written to
catch. The other nine rows of each run are retained; the rule reads none of them.

**T6 alone**, five runs per build, alternating, the test's own duration from its TRX result
(`e13/t6.json`, `e13/t6-trx/`): median 476.0 ms on base and 243.9 ms on credit, every run passing.

### 6.7 The collector's run

All from the collection at `34dbd7a`, in the files named.

| Step | Result | File |
|---|---|---|
| Build | 0 warnings, 0 errors | `build.log` |
| Test | Two architecture tests failed, both rule A14 rows, on the stray project file (section 5.8, item 11); every contract test and every other architecture test passed | `test.log` |
| Pack | Completed | `pack.log`, `nuspecs.txt` |
| Publish and run the fixtures host, JIT, trimmed and Native AOT | Every check passed in each mode | `publish-jit-and-trimmed.log`, `publish-aot.log` |
| Corpus replay, three modes | The three tables are identical | `corpus-replay.log` |
| Fuzz | 8 sessions, 2,000,000 iterations, no session reporting a finding | `fuzz.log` |
| Compositions | Both publish and run in all three modes with every check passing, and each composition's catalog is identical across its modes | `composition-*.log`, `catalog-*.txt`, `closure-*.txt` |
| Soak | 400,000 cycles completed, no fault, settled | `soak.log` |
| Bench, JIT and Native AOT | All ten measurements valid on both lanes, every A/A lane inside its effect; `meter-per-instruction` reads `per-instruction-ns=44.8399` on JIT and `per-instruction-ns=41.1233` on Native AOT. This is the credit build alone, with no base run beside it, so no rule item reads it | `bench.log` |
| Feed consumer | Both package versions packed, restored and ran, including the rollback; **the consumer's Native AOT publish failed** with MSB3073 (section 5.8, item 12; EX-42) | `feed-consumer.log` |
| Environment | Recorded | `environment.txt` |

### 6.8 E1b over the finished bundle

`dotnet test Broiler.VM.slnx -c Release` in the main checkout at `34dbd7a`, with every retained file
of this directory in place and this README written up to this paragraph, before `hashes.txt` was
rewritten: **both test assemblies passed with no failure**, the review-record rules H1 to H5 among
them, and the command exited 0. `gates/test-after-bundle.log` is the transcript.

This paragraph was written after that run, so the architecture tests were run once more over the
README as committed: `dotnet test src/tests/Broiler.VM.Architecture.Tests -c Release`, transcript
`gates/architecture-after-readme.log`.

---

## 7. Decision

The marks are evidence verdicts set by the author about what the retained evidence shows. No reviewer
has set anything, and nothing here accepts a milestone, a rule item or a change.

### 7.1 The VM-5 gate's funding clause

| Verdict | Clause | What the evidence shows |
|---|---|---|
| `[UNMET]` | Optimization is funded only against one of these baselines | This change was made for speed, and **no registered baseline shows the saving**. The only registered row on this path is `meter-per-instruction`, and the fixture executor's `Run` loop polls after every charge, so each pre-admitted block covers one charge and is settled at the next poll: the row measures the settle and re-admission that moved into `Poll`, and cannot show a saving that comes from charging inside a block. In both of this bundle's pairs that row is higher on the credit build (section 6.6). The saving is shown by the JavaScript shapes (E10) and the core probe (E12), and both are measurements local to this bundle, not baselines in the register. The owner recorded this as an exclusion rather than registering a windowed-polling baseline: EX-112. Bundle VM-5-001 reads this clause as met on the ground that no optimisation was performed; that reading no longer describes the tree, and this bundle does not edit that one |

This bundle re-reads no other VM-5 clause: bundle VM-5-001 carries them, and nothing retained here
bears on them except as section 8 says.

### 7.2 Why the change is exact, condensed

This is argued here and tested by T1 to T18, and it is no more than that: the tests are evidence,
several claims rest on sampled concurrent runs, and some parts rest on the argument alone, which the
end of this section names.

**The reference.** Call R the meter as it stands at the base: every admitted charge commits to every
level, and to the count of work since the last poll, inside its own lock section. The claim is that
any concurrent history of the pre-admitting meter, with any number of threads, meters, readers and
hosts, has a linearization in which every charge, poll, retention, release, snapshot and reader
returns what R returns in that order, and every latch - the failed dimension and scope, exhaustion,
poll-bound and cancellation observations - is the one R writes. A fast-path charge linearizes at its
successful compare-exchange; everything else at its lock section, with R's own sections where R has
two.

**The consumption identity.** At every instant and for every level, R's fuel consumption equals the
committed consumption plus what every holder chaining that level has spent from its block and not
yet committed.

**A fast-path charge is admitted only when R admits it.** A block is sized under the runtime gate to
at most what every level of its meter's chain has left once every other outstanding block against
that level is subtracted whole, and the table keeps the sum of blocks against a level within what the
level has left. So a charge that fits a block fits every level R would test. Nothing pre-admits under
an aggregate parent, so R has no parent check to make. The fast path writes no latch, and R writes
none on an admitted charge; the work count R adds at once is added at the settle, and every reader of
it settles first.

**A locked fuel charge decides as R does.** It settles its own block first. If the other holders'
blocks still leave room at every level, R's remainder is at least what the body tests against, both
admit, and both commit the same. If they do not, every holder is settled inside the section, every
fast charge that landed first is committed and every later one waits for the gate, so the body runs on
R's state and returns R's answer, including the dimension and the outermost scope it names.

**Readers see what R's readers see.** The runtime's budget snapshot settles every holder; the
remaining-allowance reader, the uncharged-work reader and `Poll` settle the meter whose own state they
read, which is enough because an invocation level belongs to one meter; a fuel retention settles every
holder in both of its sections, and R's retention was already two sections. Releasing fuel settles
nothing, and needs nothing: a release of an allowance is a no-op at every level, and a meter that holds
a block has no aggregate parent.

**The rest.** A runtime under an aggregate parent never pre-admits, so it runs R's body plus one
emptiness test. A step that parks returns through a step-end settle. Cancellation is checked before
the lock, as in R. Wall clock still accrues and is checked only in `Poll`, so an exhaustion is found at
the same poll crossing - only elapsed time differs, which is why no record may say every consumption
figure is unchanged (EX-108). The ambient lookup returns what the thread's `AsyncLocal` holds: a
context object determines every `AsyncLocal` value, and the fallback holds the answer per thread,
together with the scope and the context it was the answer for.

**What rests on the argument, or on sampled runs, alone:**

- **The second settle of a retention** has no deterministic test: only a concurrent locked fuel charge
  on another meter, landing between the retention's two lock sections, can pre-admit a block there
  (EX-109).
- **The step-end settle on the resume path, jointly with the uncharged-work reader's settle,** has no
  failing witness. No witness removes that pair alone, and W2, which removes it together with the
  invocation-path settle, fails only the invocation-path row of T8: T10, the park-and-resume test,
  passes with the pair gone. It rests on the resume path having the invocation path's shape (EX-110).
- **The concurrency claims** rest on sampled runs: T6 and T7, and the probe's `trace` and `stress`
  modes (EX-107).
- **After the fallback, the ambient lookup's context comparison across threads and its refusal to
  hold a null-context lookup** have no failing witness (section 6.2, EX-114).

### 7.3 The evidence verdict per item of the predeclared rule

| Verdict | Item of section 5.1 | What the evidence shows |
|---|---|---|
| `[UNMET]` | 1. Every correctness result holds | **Fails on the witness clause, on two witnesses.** W8 fails T12 but not T7, the second test the design names for it; and W12 fails no test at all, not T18, which it was added against (section 6.2). W12 was added after the rule was committed, and the clause covers every injected-defect witness, so it counts as W8 does. Every other clause holds as section 6.1 reads it: the build is clean with warnings as errors, by cold builds of the base, the product before the fallback and the head, the first two run after a review; the generated records were regenerated and `HUMAN_REVIEW.md` reads PENDING; the public API file is unchanged; the fuel-exactness tests pass on both meters; every other witness fails the test it names - W5 only since `4490eda` amended T9, before the rule was committed (section 5.8, item 4) - and the tree was restored after each, with the class passing clean before the first witness and after the last, though no witness has a passing run of its own after its revert (section 6.2); the profile assemblies are byte-identical; the JavaScript checks, corpus replay and host lifetime give identical verdicts; the parity rule holds; the low-fuel runs agree row for row in both forms; and the fuel minima are identical. The test-project clause failed once, in the collector's run, for the stray project file, and held in the run over the finished bundle (section 6.8). E5 to E9 ran before the fallback (EX-116), and four E8 runs were driven from another branch's driver (EX-115). Which build most of these runs used rests on the procedure, not on a retained stamp or digest (EX-117) |
| `[MET]` | 2. Shapes | In both runs, all eight shape-and-form cells: the credit median below base, by more than the A/A spread (section 6.3) |
| `[MET]` | 3. Concurrency, three parts | **On the second run.** `concurrent-bench`: the credit build below base in all twelve cells with two or more threads, in both runs. `concurrent-ambient`: **the first run failed**, the credit build above C2 in all twelve cells, which triggered the fallback; after the fallback the credit build is below C2 in all twelve. T6 alone: median 243.9 ms on credit against 476.0 ms on base. Both runs of the ambient part are retained (sections 6.5 and 6.6) |
| `[UNMET]` | 4. The per-instruction meter row | **Fails in both pairs**: the credit `meter-per-instruction` row is above base by far more than either lane's A/A figure (section 6.6). By the rule's own terms nothing in this repository may describe the change as faster for this row, and a profile that polls after every instruction pays more on the credit build |

**What the failures trigger.** Item 3's first failure triggered the resolution fallback, as the rule
says, and the measurement was taken again; both runs are retained. The rule names no change for a
failure of item 1 or item 4, only that the cell is named as failing, which this section does.

**Reviewer:** none.

---

## 8. Validity

**Reproduction.** The collector's part: `python eng/collect-evidence.py --bundle VM-5-002 --out
docs/evidence/vm-5-002 --rebench --skip-controls` from a checkout of the credit commit. Every other
part: the commands in section 5.6, with the scripts, shapes, patch and probe in `measurement/`, a base
worktree at `f127d92` outside the repository, and a C2 worktree at `e117162`. The scripts carry the
absolute paths of the workstation they ran on, as they ran.

**Expiry.** Everything here is true of the logs as retained. Rules H5 and L1 hold quoted figures to
logs and cannot hold the logs to the checkout (EX-54). `hashes.txt` is what ties this bundle to the
files it depends on.

**The register was withdrawn from `hashes.txt` after this bundle was committed, and the bundle stood
expired until it was.** As `c9afb0d` committed it, the third section of `hashes.txt` also named
`docs/baselines.md`, as the register this bundle does not edit. That row tied the bundle to a file no
run, figure or verdict here reads, and the design this work follows never named it. The records
commit `ecf52bf` then added a dated note to the register, as that design's records section asks,
which changed the file's bytes and none of its figures - and so hit the first trigger below. From
that commit until the one that withdrew the row, this bundle was expired by its own rule. The row was
withdrawn rather than the bundle re-collected, because what it was there to show - that the commit
retaining this bundle left the register alone - is git's to show, and git shows it: `c9afb0d` changes
nothing outside this directory. The register's figures are bound by rule L1 to the benchmark log of
bundle VM-6-001, not to this bundle. `measurement/make-hashes.py` was changed with it, and now reads
every tracked file at `34dbd7a` rather than at the checkout's head, so that running it again refuses
a hashed file changed since that commit instead of hashing the change.

**Recertification triggers.** Any one of these invalidates this bundle:

- a change to any file `hashes.txt` names, except a correction to a file this directory retains that is
  made by a commit regenerating `hashes.txt` with it and listed in section 5.9 - without that exception
  this bundle could not be corrected at all, and every such correction is listed where a reader looks;
- a change to the metering path - the meter, the pre-admission table, the budget levels, the ambient
  meter's resolution, the step-end settles, or the fixture executor's poll;
- a change to the core contract version or the reason-registry revision;
- **any change of machine** for a timing figure, E10 to E13 above all: they are absolute times on one
  workstation that was not idle (EX-45);
- an SDK change, since none is pinned (EX-03).

---

## 9. Exclusions

| ID | Status | Exclusion |
|---|---|---|
| EX-03 | Open | No SDK pin exists. `environment.txt` records what this machine resolved |
| EX-42 | Open | The Native AOT publish on `win-x64` needs a `vcvars64` environment. The collector's fixtures-host, composition and bench-host publishes ran in one and succeeded; its feed-consumer publish runs in none and failed with MSB3073 |
| EX-45 | Open | **One RID, one machine, one lane.** It binds hardest on E10 to E13: absolute times on one sixteen-processor workstation that was not idle |
| EX-54 | Open | Rules H5 and L1 check document against log, not log against checkout |
| EX-105 | Open | **Pre-admission is never exercised under an aggregate parent**, by design: a runtime with a parent never pre-admits. T11 shows it is refused there. Nothing shows a design that pre-admits under a parent. Closed by: a design that does, with its own evidence |
| EX-106 | Open | **The table's capacity of four, the block limit, the block cap of twice the declared poll bound, the share divisor and single-victim eviction are choices made by argument.** The probe's `concurrent-bench` rows and the T6 timing are the only evidence about them, on one machine. Closed by: a measurement that varies each |
| EX-107 | Open | **The concurrency and trace evidence drives internal types by reflection.** The probe binds `VmBudgetLevel.FuelPreAdmissions` and `VmFuelPreAdmissions.SettleAll` by name, because architecture rule A10 forbids exposing internals to another assembly; a rename breaks it silently into a base-shaped run. The in-tree evidence is the behavioural tests T4 to T7, T11 and T14, and the concurrent ones among them are sampled races. Closed by: nothing short of an internal test surface A10 would have to admit |
| EX-108 | Open | **Wall-clock consumption figures are not held equal, and most other consumption figures are held by argument rather than by a run.** Less time is spent in the lock, so every wall-clock consumption moves, and no record may say "every figure". What the retained runs compare between the builds is narrower than every other dimension: fuel consumption, in E2's tests, E9's budget lines and E12's traces; runtime call-depth and live-byte consumption, in E9's budget lines for four programs; and verdicts, including which allowance a variant exhausted, in E7 and E8. That host-call, allocated-byte, verifier-work and nested-load consumption are unchanged is argued - a charge of any dimension but fuel never takes a block and runs the locked path as before - and no retained run compares those figures |
| EX-109 | Open | **The second settle of a fuel retention has no deterministic test.** Only a concurrent locked fuel charge on another meter, landing between the retention's two lock sections, can pre-admit a block there. It rests on the invariant that every commit of fuel first checks room or settles every holder, and on the readers' argument in section 7.2 |
| EX-110 | Open | **The step-end settle on the resume path, with the uncharged-work reader's settle, has no failing witness.** No witness removes the resume-path pair alone. W2 removes it together with the invocation-path step-end settle, and the only test that fails is the invocation-path row of T8: T10, the park-and-resume test, passes with the pair gone. The resume path is the same code shape as the invocation path, and that is what the pair rests on |
| EX-111 | Open | **A remaining-correlated timing signal between concurrent operations of one runtime.** With two or more holders sharing a runtime or instance level, blocks shrink as that level nears its ceiling, so extra locked charges grow more frequent as the remainder falls, and a guest timing its own charges can learn roughly how much of the shared level remains. No remaining value becomes readable through the metering surface; a coarse one becomes timeable. ADR 0007's acceptance of timing as a channel covers runtimes under a shared aggregate parent, and says nothing about operations inside one runtime. The owner recorded this as an exclusion and declined the variant that refuses pre-admission near a shared ceiling, which would leak one threshold bit instead. Closed by: that variant, or a decision that accepts the signal |
| EX-112 | Open | **No registered baseline shows the saving.** VM-5 funds optimisation only against a registered baseline, and the one registered row on this path, `meter-per-instruction`, polls after every charge and cannot show it; in this bundle that row is higher on the credit build. E10 and E12 show the saving and are bundle-local measurements. The owner recorded this as an exclusion: no windowed-polling baseline is registered, VM-6-001's benchmark log is not re-collected, and the figures in `docs/baselines.md` are not edited. Closed by: registering a windowed-polling measurement and collecting it before and after a change, or the performance owner recording the clause as unmet for this change |
| EX-113 | Open | **The conformance parity is held except on predeclared base wall-clock rows**, which may become passes or other exhaustions in the credit build. In this collection no row differed, so the exception was not used; it stands because a wall-clock verdict belongs to the machine, and a later collection may need it |
| EX-114 | Open | **After the thread-static fallback, two arms of the ambient lookup have no failing witness.** Dropping the context comparison fails T12 and no longer fails T7, and holding a lookup made with the flow suppressed fails no test: both tests put a second thread where a scope-wide field would have leaked across threads, and a per-thread answer cannot. Both are failures of rule item 1's witness clause. Closed by: a test that changes context on one thread in the way each arm guards, with a witness that fails it |
| EX-115 | Open | **Part of the low-fuel series was driven while the checkout was on another branch.** From 16:22:56 to 16:52:03 the main checkout was on `main`. Four bytecode runs at fuel 30,000 and 100,000, base and credit, were driven by `main`'s test262 driver, and the identity of the credit binaries they used rests on file timestamps, not on anything in the reports. Six native runs in that window were taken under the numeric manifest; they are retained apart as off-series and were not counted, and the native series at those fuel values was run again afterwards |
| EX-116 | Open | **The JavaScript and conformance evidence was taken before the resolution fallback.** E4 to E9 used the product at `c98011a`; the fallback changed `VmExecutionScope` alone, and the design's fallback clause asks for E1 to E4, E12 and the T6 timing again but not for E5 to E9. E4's inputs are unchanged across the fallback; E5 to E9 were not re-run on `34dbd7a`. Closed by: re-running E5 to E9 on the head |
| EX-117 | Open | **Which build most runs used, and when several of them ran, rests on the procedure and on file-system times, not on a retained stamp.** E3's transcripts name their commit, and the binaries of E9 and of E10's second run survived for their stamps and digests to be retained after the fact. For E2's credit half, E4 to E8, E10's first run, E11, E12 and E13, no retained file names the commit or holds a digest of the binary the run used, and E7's base and credit reports and E5's plain transcripts are byte-identical, so their content cannot tell the builds apart. No transcript of E4 to E9 carries a time: the times of E7 and E8 - including that both credit test262 runs began after the predeclared rule was committed - are output-directory creation and write times read from the throwaway directory after a review, and E4 to E6 have no time of their own. Closed by: a driver that writes the binary's commit stamp, its digest and its start time into its own transcript |
