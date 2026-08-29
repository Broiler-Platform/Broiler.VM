# Evidence bundle VM-2-001

**Milestone:** VM-2 — bounded artifacts, verification, and resource enforcement
**Collected:** 2026-08-29
**Core contract version:** 1
**Status of the milestone after this collection:** In progress, unaccepted.

This bundle records what was run and what happened. It does not accept a milestone: no reviewer has
read this work, `HUMAN_REVIEW.md` is unsigned and `PENDING`, and ledger update rule 7 puts
acceptance behind an owner and a reviewer confirming every objective exit condition. Under update
rule 8 the work could be built and landed without that decision; it could not be released, and
nothing here is a release.

What VM-2 was asked for is a boundary that holds against input chosen to break it. So the two
artefacts that matter most in this bundle are not code: they are `src/tests/corpus/vm-2`, eighty-seven
retained artifacts with their hashes and their expected answers, and `negative-control.log`, which
shows eight separate ways of breaking the component and the suite refusing each one.

---

## Field coverage

The status ledger's section 3 fixes the fields a bundle must carry. Each is below; a field this
milestone cannot supply says so rather than being omitted.

| Field | Where |
|---|---|
| Identity | Section 1 |
| Source | Section 2 |
| Dependencies and corpus | Section 3 |
| Environment | Section 4 |
| Procedure | Section 5 |
| Outputs | Section 6 |
| Decision | Section 7 |
| Validity | Section 8 |

---

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle ID | VM-2-001 |
| Milestone | VM-2 |
| Roadmap revision | `docs/roadmap.md` as committed, section 13's VM-2 gate |
| Core contract version | 1, unchanged. VM-2 mints no amendment. |
| Reason-registry revision | 2. One reason was added inside an existing category, which is additive and moves this number and not the contract version. |
| Owner | MaiRat, holding all six roles ADR 0012 records |
| Reviewer | None. No area verdict in `HUMAN_REVIEW.md` section 8 is set. |

---

## 2. Source

Collected against the component at commit `c0c1ed4`, the commit that landed the corpus replay mode,
with a **dirty working tree**. What was uncommitted is this bundle's own record set: the ledger row,
the review record's VM-2 paragraphs, the component overview's status section, the rule register's
milestone and its H5 row, the six H5 witness inputs the current bundle's figures pin, and the
collection script itself.

Not one product source file differs from that commit. `src/Broiler.VM.Abstractions`,
`src/Broiler.VM.Binary` and `src/Broiler.VM.Runtime` are exactly as committed, and `hashes.txt`
pins every file in them, so a reader can check that claim rather than take it. The dirty part is
the paperwork this bundle consists of, which cannot be committed before the collection it
describes has run.

Every file the result depends on is hashed in `hashes.txt`: the three product assemblies' sources,
the twelve boundary records, the two manifests, the vendored packaging props, the generated
assurance artefacts, and every one of the eighty-seven corpus artifacts with its manifest.

The corpus files are hashed individually and not only through their manifest. A bundle that hashed
the manifest alone would pin a description of the corpus rather than the corpus, and a minimized
fuzz regression has no declaration anywhere else to be checked against.

---

## 3. Dependencies and corpus

| Item | Value |
|---|---|
| SDK | 10.0.400, as `environment.txt` records. No pin exists; exclusion EX-03 stands. |
| Target framework | `net10.0` for all eight projects |
| Package references | The test SDK and xunit in the two test projects only. No product project references a package. |
| Malformed-input corpus | `src/tests/corpus/vm-2`: eighty-seven artifacts and a manifest, each artifact carrying its SHA-256 and its expected answer |
| Fuzz seed corpus | The same eighty-seven artifacts. A session is a total function of its seed and this set. |

The corpus is a file tree rather than a generator inside a test, because ledger update rule 4 forbids
promoting a subset generated to prove a contract into evidence for a corpus gate. The difference the
rule protects is retention: a case a test generates exists only while the test runs, so nothing can
cite it, diff it, or notice when its answer changes. Exclusion EX-48, which recorded that no corpus
existed, is closed by this one.

---

## 4. Environment

| Field | Value |
|---|---|
| OS | Linux 6.18.44, glibc 2.39 |
| Architecture | x86_64 |
| RID | `linux-x64` |
| Processors | 4 |
| Configuration | Release |
| Publish modes | JIT, trimmed self-contained, Native AOT |
| Lane | None. One machine, one RID, no CI. |

**This is a different RID from VM-1's.** VM-1 collected on `win-x64`; this collection ran on
`linux-x64`, and the Native AOT publish needed no special shell here because the toolchain discovery
failure exclusion EX-42 describes is a Windows-only problem. That is evidence about two machines and
it is **not** a support claim: claiming a RID is a release act, and ADR 0012 owns the declared
matrix. Exclusion EX-45 stands and now covers a second single machine rather than one.

---

## 5. Procedure

Everything below was produced by `python eng/collect-evidence.py --bundle VM-2-001 --out
docs/evidence/vm-2`, run from the component root. The script is retained in the repository, which is
what both earlier bundles recorded as a gap and closed by promising rather than by landing a file.

| Step | Command | Log |
|---|---|---|
| 1 | `dotnet build Broiler.VM.slnx -c Release` | `build.log` |
| 2 | `dotnet test Broiler.VM.slnx -c Release` | `test.log` |
| 3 | `dotnet pack Broiler.VM.slnx -c Release -o <temp>` | `pack.log` |
| 4 | `dotnet run --project <fixtures host> -c Release -- --verbose` | `publish-jit-and-trimmed.log` |
| 5 | `dotnet publish <fixtures host> -c Release -r linux-x64 --self-contained true -p:PublishTrimmed=true`, then run the binary | `publish-jit-and-trimmed.log` |
| 6 | `dotnet publish <fixtures host> -c Release -r linux-x64 -p:PublishAot=true`, then run the native binary | `publish-aot.log` |
| 7 | Replay the corpus from each of the three modes and compare the tables | `corpus-replay.log` |
| 8 | Eight seeded fuzz sessions of 250,000 iterations each | `fuzz.log` |
| 9 | Eight negative controls, each injected, run, reverted, and run again | `negative-control.log` |
| 10 | Environment and hashes | `environment.txt`, `hashes.txt` |

The script judges nothing. It runs the procedure and retains what happened, failures included.

---

## 6. Outputs

**Build.** Eight projects build Release with 0 warnings and 0 errors.

**Tests.** 255 tests pass, 0 failed, 0 skipped — 90 architecture and 165 behavioural. VM-1 retained
221; VM-2 adds thirty-four behavioural tests and no architecture test, which is the honest shape of
this milestone: it adds no new forbidden edge and a great deal of behaviour.

**Pack.** Exactly three `.nupkg` and three `.snupkg`, one pair per product package. None of the five
test-only projects packs, including the fuzz host this milestone added.

**Publish and run.** The composition-root host publishes and runs in all three modes and passes its
five checks in each. The trimmed self-contained image is 78256 bytes and the Native AOT native image
is 1557288 bytes.

**Cross-mode failure classes.** The published host replays all eighty-seven corpus artifacts and
prints one line per artifact: the outcome, the reason, the profile's diagnostic code, the dimension
and scope a resource answer named, and whether a handle came back. The three tables are
byte-identical — one distinct SHA-256 across three modes. That is the gate's cross-mode clause, and
it is a claim about published binaries rather than about a test run: an enumeration rendered by name,
a switch the linker reshaped, or a generic instantiation the AOT compiler could not see would each
change what a host is told an artifact was, and none of them is visible to a suite running under the
JIT.

**Fuzz.** Eight sessions, 250,000 iterations each, 2,000,000 in total, no counterexample. Every
session reached more than one outcome; the distribution is roughly 97 per cent invalid artifact, 2
per cent resource exhaustion and 1 per cent valid, which is what the histograms in `fuzz.log`
record. A session in which every iteration answered the same way exits non-zero, because a broken
seed corpus would otherwise read as a quarter of a million clean iterations.

**Corpus.** Eighty-seven artifacts across ten families: three well-formed controls, prefixes too
short to carry a header, a damaged magic, unknown and non-canonical format versions, section framing
that over-declares, under-declares and claims a terabyte inside twenty bytes, unknown section kinds,
constant counts at and past the declared-count ceiling, code lengths and operand indexes past their
bodies, an artifact refused against a ceiling its own descriptor asked for, and two systematic
sweeps — the canonical artifact truncated at every offset and every one of its bytes inverted.

Forty entries pin the outcome, the reason and the profile's diagnostic code by hand, beside the
bytes. The sweeps, which have no hand-computed answer worth more than the sweep itself, pin the
closed set and hold the rest to a recorded observation under version control. The three controls and
the several inverted bytes that still verify are what make the corpus able to fail in both
directions: a corpus in which everything fails passes just as happily under a verifier that rejects
whatever it is handed.

### 6.1 What the first corpus run found

The entry that declares a constant pool at exactly the declared-count ceiling answered
`ResourceExhaustion` naming `VerifierWork`, where the hand-written expectation said the pool would be
admitted and the artifact would then run out of bytes.

The expectation was right and the implementation was wrong. The uncharged-work counter summed every
budget dimension, so one correctly metered, in-bounds allocation of half a megabyte breached a poll
bound of a thousand instantly — and the poll-bound path reports a profile fault and poisons the
runtime, so a core unit conflation was being billed to the profile as a broken metering contract. The
bound is on work performed between two polls; only `Fuel` and `VerifierWork` are denominated in work,
and only they reach the counter now. The corpus entry is the regression test, and
`ArtifactBoundaryTests` carries a named one beside it.

Two further gaps were found while writing the suite rather than by running it. The cumulative nested
verifier-work bound was carried in the profile descriptor, validated at runtime creation, and read
nowhere, so the fourth of four guest-load bounds did nothing; it is measured across each nested
verification and accumulated per operation now. And the artifact-limit clamp that the precedence
algorithm requires was computed and discarded: an over-asking descriptor was silently given the
intersection with no record that it had asked for more.

### 6.2 The negative controls

Eight, each one edit that a named rule must reject. Every one failed the suite when injected and left
it green after revert. The first four are VM-1's, re-run here; the last four are VM-2's and exist
because a corpus that has never rejected anything is a file tree and not a gate.

| # | The injected defect | What rejected it |
|---|---|---|
| 1 | `Broiler.VM.Runtime` references the test-only fixture assembly | Rules A4 and A7 |
| 2 | An edge the checkout has is deleted from `graph.manifest.json` | Rule A7 |
| 3 | A retired name is exported from a product assembly | Rule V3 |
| 4 | The deterministic no-provider refusal is removed | The behavioural suite |
| 5 | The verifier reserves an allocation before reading a byte | The materialization-ordering assertion, for every corpus artifact at once |
| 6 | The verifier escapes instead of answering | The fuzz target, at iteration 15 of session seed 1, minimizing the input from 20 bytes to 5 and retaining it |
| 7 | One declared corpus expectation is changed to a wrong reason | The pinned-answer assertion |
| 8 | One byte of one retained corpus artifact is changed | The manifest hash, and then the pinned answer |

Control 6 is the one worth reading. It is the only demonstration in this bundle that the fuzz
target's retention mechanism works end to end, because no genuine session has found anything to
retain — exclusion EX-79.

---

## 7. Decision

The VM-2 gate, clause by clause. The marks are evidence verdicts set by the author about what the
retained evidence shows; no reviewer has set anything.

| Verdict | Clause | What the evidence shows |
|---|---|---|
| `[MET]` | Truncated, corrupt, oversized, mismatched, unknown-version and resource-hostile artifacts fail before execution without out-of-budget allocation | All six kinds are families of the retained corpus. Every entry that does not verify produces no handle, and the reserved-byte total for every entry, failing ones included, stays under the allocation ceiling the frozen policy carried. |
| `[MET]` | Effective policy is computed before allocation and never exceeds the host ceiling | A recorder stamps three points a profile cannot reorder — the policy arriving, the first byte consumed, the first allocation reserved — and every corpus artifact is asserted to freeze its policy before both. The policy each verification received is recomputed from the three layers independently and compared dimension by dimension against the intersection, and against the host ceiling. |
| `[MET]` | Execution consumes only the verified handle; mutate, dispose and concurrently overwrite the caller's buffer | Mutation was VM-1's. This milestone adds the other two: a pooled array returned, taken back out and filled with an unrelated pattern, and another thread rewriting the caller's array continuously through two hundred instantiate-and-invoke rounds. Both answer 42 every time. |
| `[PART]` | Unit, property and fuzz suites retain minimized regressions, and the same failure categories are stable in JIT, trimmed and Native AOT hosts | The three suites exist and the cross-mode tables are byte-identical. What is not shown is retention of a real regression: no session has found one, so the corpus holds none, and the mechanism is demonstrated only by negative control 6. Exclusion EX-79. |
| `[MET]` | Omitted limits inherit materialized bounded policy, invocation overrides only tighten it, and a raised ceiling requires a newly verified handle | An omitted override inherits; a tightening applies; a raise is refused as a host failure naming the dimension, and a set with one raising entry applies none of its others. A wider ceiling is reachable only by verifying again, and because the effective ceilings are part of identity the two handles cannot be confused — the tight handle presented to the wider runtime is refused on the ceiling clause. |
| `[PART]` | A guest-initiated load cannot exceed, extend or escape its requesting operation's budget; recursive and fan-out provider requests terminate at their configured bounds; a composition with no provider refuses every request deterministically | Fan-out, cumulative nested bytes and cumulative nested verifier work each terminate a run at their configured bound, and a provider-less composition refuses twice with the same answer. **Recursive** requests are the gap: contract version 1 gives an executing profile no way to instantiate the handle a load returns, so nesting is bounded at one by construction and the configured depth bound is never what stops a run. Exclusion EX-78, with a test asserting the unreachability rather than a comment claiming it. |
| `[N/A]` | Bounded outer-envelope parsing "where approved" | Nothing approves it. ADR 0010 records that VM-0 through VM-6 contain no persistence gate, so the word has no referent, and shipping an envelope reader before one is named is forbidden by that record's own prohibition table. VM-2 ships none, and rule V10 asserts that no public member can express one. Exclusion EX-25. |

### 7.1 Deviations recorded rather than amended

| Deviation | Why it is an erratum |
|---|---|
| The artifact-limit clamp is recorded on the verified handle, not in `VmDiagnostics` | ADR 0005 freezes the diagnostics field set and reserves it explicitly, so adding a group is an amendment rather than an implementation. The handle is host-facing and already carries the effective ceilings the clamp explains. What it costs is that a verification which fails records no clamp: exclusion EX-82. |
| A refused budget override populates the diagnostics dimension and scope | ADR 0005's table annotates that group "For `ResourceExhaustion`", and this is a host failure. The group is the only one that can name a dimension, and ADR 0007 requires the refusal to name one. What ADR 0007 forbids is mapping the refusal into an exhaustion *metric*, and the category is what a metric keys on. Exclusion EX-83. |

---

## 8. Validity

**Reproduction.** `python eng/collect-evidence.py --bundle VM-2-001 --out docs/evidence/vm-2`. The
script derives the RID, the binary's extension and the Native AOT invocation from the platform, so
the same command reproduces this bundle's shape on a different one — with different numbers, which
is the point of retaining the environment beside the figures.

**Expiry.** The figures here are true of the logs as retained. Rule H5 holds every quoted headline
figure to those logs and cannot hold the logs to the checkout: a stale log and a stale document agree
with each other. Exclusion EX-54 records that, and this section is what covers it.

**Recertification triggers.** Any of these invalidates this bundle and requires a fresh collection:

- a change to any file in `hashes.txt`, which includes every corpus artifact;
- a change to the core contract version, or to the reason-registry revision;
- a change to the profile descriptor's fifteen limit defaults or hard maxima, which are the layers
  the precedence oracle recomputes from;
- an SDK change, since none is pinned;
- a claim about any RID other than the one section 4 records;
- a fuzz session that finds a counterexample, whose minimized input belongs in the corpus and whose
  discovery changes what section 6 says.

---

## 9. Exclusions

Each is a named limit on what this bundle shows. Carried-forward exclusions keep their earlier
identifiers and are listed only where VM-2 changes what they cover. **Status** is `Open` where the
limit still stands and `Closed` where a dated decision or a later milestone has discharged it; a
closed exclusion is retained, not deleted.

| ID | Status | Exclusion |
|---|---|---|
| EX-03 | Open | No SDK pin exists. The toolchain is what this machine resolved, and `environment.txt` records what that was. |
| EX-25 | Open | **The persisted envelope is admitted by the contract and implemented by no milestone.** VM-2's next action says "bounded outer-envelope parsing where approved", and ADR 0010 records that the word has no referent: VM-0 through VM-6 contain no persistence gate. VM-2 therefore ships no envelope reader and no envelope writer, publishes no outer schema version and offers no byte compatibility, which is what that record's prohibition table requires before a gate exists. Rule V10 asserts the absence. Closed by: a persistence gate the roadmap does not yet contain. |
| EX-42 | Open | The Native AOT publish on `win-x64` requires a `vcvars64` environment because the ILCompiler package's own toolchain discovery fails on the VM-1 machine. **It did not apply to this collection**: on `linux-x64` the ordinary publish command produced and ran a native image, and the collection script now takes that path on any non-Windows platform. The exclusion stays open because the Windows lane is unchanged and is the one ADR 0012 reserves. |
| EX-45 | Open | **One RID, one machine, one lane — and now a second single machine.** VM-1 collected on `win-x64` and this bundle on `linux-x64`. Two single machines are not a matrix: no arm64, no macOS, no second machine per RID, and no CI. Nothing here claims a RID; claiming one is a release act. |
| EX-48 | Closed | **Closed 2026-08-29 by this bundle.** VM-1 recorded that its seven deliberate corruptions were generated rather than stored and that no corpus, fuzz target or minimized regression existed. `src/tests/corpus/vm-2` is retained, hashed and cited; the fuzz target exists and has run two million iterations. What is not closed is retention of a real regression, which is EX-79 and a narrower thing. |
| EX-52 | Open | **Twenty-nine review findings of major and minor severity remain unaddressed**, less the ones VM-2 happened to reach. VM-2 did not set out to work that list and did not: what it corrected is what its own corpus and suite found. |
| EX-54 | Open | Rule H5 checks document against log, not log against checkout. Section 8's expiry clause is what covers the drift. |
| EX-78 | Open | **The guest-load nesting-depth bound is unreachable at contract version 1.** A nested load hands the requesting profile a verified handle, and instantiation lives on `VmRuntime`, which nothing an executor is given can reach; a provider is mandatorily non-reentrant; and a verifier is handed no mediator at all. Depth is therefore bounded at one by construction, the configured bound is never what stops a run, and VM-2 leaves it untested above one. A test asserts the unreachability from the public surface rather than a comment claiming it, so the day a member makes recursion possible the assertion fails and the bound has to be exercised. Closed by: an amendment that lets a profile instantiate what it loaded, or a provider that may re-enter — either of which makes the bound live. |
| EX-79 | Open | **The corpus retains no minimized regression, because no session has found one.** Two million fuzz iterations across eight seeds produced no counterexample, so the gate clause asking that the suites "retain minimized regressions" is satisfied only in the sense that there is nothing to retain. The mechanism is shown working by negative control 6, which found an injected escape at iteration 15, reduced the input from twenty bytes to five and wrote it into the corpus directory. A demonstrated mechanism with an empty result is what this bundle claims and no more. Closed by: a session that finds something, or a reviewer accepting that an empty result is the honest one. |
| EX-80 | Open | **The fuzz session varies the payload and never the descriptor.** Every iteration presents its mutated bytes under one descriptor: the fixture profile, format version 1, its one accepted manifest, and no requested limits. So the whole of the descriptor-facing surface — an unsupported format version, an unaccepted feature manifest, a profile the catalog lacks, a requested limit that clamps or starves — is exercised by the corpus and the behavioural suite and by no fuzz iteration at all. Mutating the descriptor as well is a strictly larger space and a strictly better target. Closed by: a descriptor mutator. |
| EX-81 | Open | **Rule H5 admits a figure from any bundle a document links.** The status ledger is a dated history as well as a current-state record — update rule 1 requires earlier evidence links and decisions to be preserved — so a superseded milestone's row goes on quoting the figures its own bundle retained, and comparing every line of the ledger against the current bundle would make preserving that history a violation. The link is what keeps it a citation rather than a loophole: a figure from a bundle the document never mentions is still caught. What it cannot do is tell which row a figure belongs to, so within such a document a figure correct for VM-1 quoted in the row for VM-2 is admitted. The anti-deletion guards are unweakened: they compare the current bundle's values and still fail if no document quotes them. Closed by: figures carried as data a document renders, which is also EX-56's closure. |
| EX-82 | Open | **A verification that fails records no clamp.** The artifact-limit clamp is carried on the verified handle, because `VmDiagnostics` has a frozen field set that an implementation may not extend, so a descriptor that asked for more than it could have and then presented bytes that did not verify reports its failure and not its clamp. Nothing is lost that the caller cannot recompute — it supplied the request and configured the ceiling — but the precedence algorithm's diagnostic is, at that moment, not emitted. Closed by: an amendment that gives the diagnostics record a clamp group, or a decision that the handle is the right and only place. |
| EX-83 | Open | **A refused budget override populates a diagnostics group ADR 0005 annotates for a different category.** Group 7 carries a budget dimension and a scope and the record's table annotates it "For `ResourceExhaustion`". A `BudgetRaiseRefused` host failure names a dimension there because it is the only group that can name one and ADR 0007 requires the refusal to name it. What ADR 0007 forbids is mapping the refusal into an exhaustion metric, and the outcome category is what such a metric keys on, so the prohibition is intact. It is filed as an erratum rather than an amendment because it changes no member and no shape. Closed by: an amendment that widens the annotation, or a reviewer rejecting the reading. |
| EX-84 | Open | **The cross-mode table is produced under one descriptor.** The published host replays every corpus artifact with the ordinary descriptor, so the three entries the behavioural suite presents under a varied one contribute their ordinary answers to the comparison instead. Reading the manifest in the host would mean a JSON reader inside a trimmed and AOT-published binary, whose own behaviour under trimming would then be part of what the table measures. Closed by: descriptor variations encoded in the corpus file names, or a manifest format the host can read without a serializer. |
| EX-85 | Open | **`DescriptorMismatch` is unreachable through the public path.** The fixture profile supports exactly one profile-format version, so a descriptor whose version disagrees with the payload's is refused by the core as an unsupported format version before the verifier is entered, and the reason the verifier reserves for the disagreement is never produced. The corpus records the reachable case and names the unreachable one. Closed by: a fixture profile accepting a range, which is a fixture change and not a core one. |
