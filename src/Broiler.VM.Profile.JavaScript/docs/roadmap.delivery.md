# Broiler.VM.Profile.JavaScript roadmap — delivery

**This file is part of the [Broiler.VM.Profile.JavaScript roadmap](roadmap.md)**, which
[names every file](roadmap.md#how-this-roadmap-is-split). It carries sections 19–20 and 25–26:
the milestones, the order they are delivered in, the map that ties every chapter of the plan
to the milestone that delivers it and the gate that closes it, and **the road to a full-featured
profile**, which is where a reader who wants to know what is finished and what comes next starts
([section 26](#26-the-road-to-a-full-featured-profile)). **Section numbers are global and do
not change when a section moves**, so a reference written to any section below still resolves here.

The [evidence ledger](roadmap.status.md) is the authority for what has been accepted, and
[the corrections and rejections](roadmap.corrections.md) hold what an earlier reading of any
milestone below said before implementation replaced it.

---

## 19. Milestones

**This section is the authority for what is *planned* and for nothing else.** What has moved and
what each retained bundle demonstrates are the [ledger](roadmap.status.md)'s, and no milestone
below restates them: each names its ledger row and stops there. **Which milestone a clause belongs
to is this section's**, so a gate saying a clause is carried to a later milestone is a plan
statement and not a status one — what the ledger holds is whether that clause has since been
closed by evidence.

The milestone set is JS-0, JS-1, JS-2, **JS-3a and JS-3b**, then JS-4 through JS-10 — twelve rows,
because what was one JS-3 is two, split by dependency rather than by size for the reason
[section 20](#20-delivery-order) gives *(corrected: JSC-15)*.

**Every milestone below has the same six parts, in the same order, and nothing else**: **Owner**,
**Ledger**, **Next action**, **Dependencies**, **Objective exit gate**, **Seed**. Where a milestone
must explain why it sits where it does, it does so in a paragraph before the six rather than in a
seventh bullet — a bullet only one milestone carries is a bullet a reader learns to skip, and it is
where a re-scope or a correction otherwise accumulates.

Two dependencies run through every milestone and are stated once. **The core's contract being
implemented and the core's contract being accepted are two different inputs**, so JS-0, JS-1 and
JS-3a need only the first while JS-2 onward additionally need the second — a gate this component
does not hold, recorded in the ledger with its holder rather than routed around. And
**owner and reviewer roles are named per milestone**; where one person holds several, the
non-independence is recorded as a limit on what these gates prove, not resolved by assertion.

One term below means one thing throughout. **A milestone's *claimed RID set* is the set its own
bundle published and ran on, named in that bundle and never empty.** Claiming a runtime identifier
as *supported* is a release act and JS-10 owns it: a milestone that published and ran on one
machine has **recorded** a RID, and a support table that has not been issued has claimed none.

### JS-0 — Boundary, placement, identity, and the assurance floor

- **Owner:** profile architecture owner, with the core's topology owner co-signing placement and
  the release owner co-signing the licence position.
- **Ledger:** JS-0's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and — once the row is `In progress` — every open clause of the gate below.
- **Next action:** Decide and record, each as a dated decision with a registered rule and a
  passing witness: where this component lives relative to the core and the aggregate repository;
  the profile ID `broiler.javascript` and the `Broiler.*` package identity it obliges; the
  assembly topology of [section 5](roadmap.md#5-package-boundaries-and-the-dependency-graph) and
  whether the profile is one assembly or several; the feature manifest allocation of
  [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted); the three
  composition labels and which are advertised (none, at first); the waited-on set and the snapshot
  stop condition of
  [section 4.2](roadmap.md#42-what-after-the-fix-work-lands-can-and-cannot-mean); the nullable and
  unsafe-code positions the seed forces; and the satellite-acquisition dependency and its owner.
  Record **this profile's fifteen hard maxima and fifteen defaults, with
  [section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses)'s
  catalog-wide consequence stated inside the decision itself** — that a maximum binds this
  profile's own artifacts alone, that an adopted **default** resolves to the tightest in the
  catalog and is therefore the declaration a neighbour actually feels, and that reconciling two
  profiles' defaults belongs to whichever component composes them. Record the reciprocal
  cross-profile position of
  [section 15](roadmap.md#15-deployment-compositions-native-aot-and-the-browser-embedding): the
  `WebAssembly` namespace is a host-object surface in no allocated manifest, named as an exclusion
  rather than left to be inferred, and the refusal of a cross-profile value channel is co-signed
  here rather than left to the other profile to carry alone. Re-grade
  [section 18](roadmap.md#18-amendments-this-profile-expects-to-ask-of-the-core)'s
  argument-channel row and split its result half out, since both need no code and the first
  amendment the programme files will be one of them. **Adopt the host component's assurance system,
  rule register and licence and notice files rather than standing up a second of each** — the
  annotation grammar, the exemption predicate, the review-state machine, the fingerprint definition
  and the release-gate semantics are repository policy, and a second implementation that quietly
  diverges is what the core's extraction gate exists to catch — and record every place adoption
  costs something as a dated deviation. What this milestone *does* stand up of its own is the part
  adoption cannot supply: **this profile's own evidence-bundle contract and collection script**,
  because a bundle collected by the host's script would merge two ledgers, and its own group in the
  rule register.
- **Dependencies:** Named ownership. No dependency on the seed, on the copy, or on any core
  milestone's acceptance.
- **Objective exit gate:** An acyclic shell graph builds Release with zero warnings; architecture
  rules express every forbidden edge **including both halves of the legacy-boundary rule and both
  halves of the no-edge-to-another-profile rule**, each with a passing witness and a negative
  control that fails when injected and passes after revert; **a two-profile catalog test composes
  this profile's descriptor beside a second profile whose declarations are deliberately hostile and
  proves that the neighbour's maxima do not reach this profile's artifacts at all, and that its
  adopted defaults do**, with a negative control that sets a guest-load *default* to zero on the
  neighbour, adopts defaults rather than stating ceilings, and observes `eval` refused — the
  exposure that survives, in the one configuration that still has it. **Neither half of that clause
  can close here**: this milestone lands no product code, so there is no descriptor to compose
  until JS-1, and no `eval` to refuse until JS-8. It is carried to both rather than satisfied with
  a fabricated descriptor;
  a scan asserts no source file, project file, or build item resolves outside the component root,
  and an unresolvable build item is **reported rather than skipped**; the public API baseline
  mechanism exists and compares in both directions, with an injected member failing it and a
  deleted member failing it too — **a clause this milestone cannot close, because the family
  exports nothing until JS-1 and the describer that would read it needs a reference the rules
  forbid, so it is carried and closed at JS-3a** *(corrected: JSC-16)*; the assurance generator is
  a fixed point — a regeneration moves
  no byte — and a negative control proves it refuses to write a reviewer identifier no source
  line carries; the release-mode gate names each blocking declaration individually rather than
  counting them; the evidence-collection script exists and this milestone's own bundle was
  produced by it; and the snapshot identity schema is recorded with **a second checkout re-deriving
  the same identity from the record**. The licence and notice are not on this list: the files are
  the host component's, and the row this profile lands in them carries an upstream derivation that
  does not exist until a tree is copied, so
  [section 4.5](roadmap.md#45-licence-attribution-and-one-notice-that-must-change) puts it at JS-2
  and JS-2's gate closes it.
- **Seed:** Nothing is copied. Every mechanism here is this component's own code.

### JS-1 — Close the whole contract loop on the smallest JavaScript that is still JavaScript

- **Owner:** profile contract owner, with release and AOT review of the composition root.
- **Ledger:** JS-1's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and — once the row is `In progress` — every open clause of the gate below.
- **Next action:** Mint `broiler.javascript.slice` and define format version 1 for it. Write the
  verifier over the core's bounded reader and allocator, supplying the bounds projection and the
  allocation-meter adapter. Implement all seven core-facing types. Fill every descriptor row in
  one full-arity construction, with the language-shaped rows of
  [section 8](roadmap.md#8-the-value-frame-and-call-model) marked **provisional** and each naming
  the milestone that will settle it. Write the lowering for this slice by hand in the lowering
  sibling. Stand up **two composition roots differing by exactly one reference, the lowering** —
  one that names the profile and not the compiler and therefore cannot turn source into an artifact
  however it is invoked, and one that names both and writes the retained corpus — each with a
  closure self-report mode. **The missing reference is the whole of the execution-only root's
  label**; the compiler-bearing root beside it claims none, because `narrow-runtime-compiler`
  belongs to a composition lowering a named restricted *source* surface and there is none until
  JS-3b. Decide
  and record the entry-point answer from
  [section 10](roadmap.md#10-execution-mapping-javascript-onto-the-core-lifecycle).
- **Dependencies:** JS-0. Deliberately **not** the copy, not a parser, and not core acceptance:
  the point of this milestone is to find contract defects against about two thousand readable
  lines rather than against a copied engine.
- **Objective exit gate:** **Each of the two composition roots publishes and runs** on every RID of
  this milestone's claimed set, which its bundle names and which is not empty — under JIT, trimmed
  self-contained, and Native AOT with trim and AOT warnings treated as errors, executing a verified
  artifact to its expected answer in every mode, each closure report containing exactly the
  declared assemblies and no test, reflection, dynamic-code, or IL-emission assembly, **and the two
  reports differing by exactly the lowering** — which is the whole of the `execution-only`
  property, and the reason it is two projects rather than one binary with a flag a closure report
  cannot see. **Each of the five verifier outcomes** is produced by a named retained corpus case,
  the invalid-artifact case carrying a diagnostic code and a source position and the exhaustion
  case naming one dimension and one scope. **Each of the five execution-step kinds** is produced by
  a named test, including a contract violation from a deliberately non-conforming variant; if
  `Suspended` is unreachable from this surface the milestone declares it produced at JS-7 rather
  than minting an out-of-manifest opcode. The descriptor is admitted by a catalog build, and named
  negative cases produce each catalog refusal this descriptor can provoke. An artifact naming an
  absent profile answers `UnsupportedProfile` / `ProfileNotInCatalog` **with no payload byte
  examined**; one naming an unaccepted manifest answers `UnsupportedFeatureManifest`; one naming an
  out-of-range format version answers `UnsupportedProfileFormatVersion`. A second profile composed
  in the same catalog proves a foreign payload is dropped rather than projected, and every payload
  kind this profile can mint lies inside its declared range. A case proves the executor sizes its
  operand stack from a bound **computed at verification and stored on the verified state**, never
  from a number the payload chose. The descriptor is reachable through exactly one static accessor,
  and no aggregate profile-listing type exists in the graph. A permutation of registration orders
  over the same descriptor set produces a byte-identical catalog identity encoding. A case mutates,
  disposes, and concurrently overwrites the caller's payload buffer after verification returns, and
  neither the verified state nor the execution result changes. The slice corpus replays identically
  twice with no residue, contains at least one successful control entry, and the verifier throws on
  none of it.
- **Seed:** Nothing. This milestone's hand-written encoder and its hand-written **programs** are
  **scheduled for deletion at JS-4** with a named owner and a gate clause, because a second
  handle-producing path and a second corpus of programs are non-goals. **The instruction buffer
  beside them is not scheduled for deletion** — constant interning, the local frame, label
  patching and the section framing are what any lowering needs, and JS-3b's source lowering uses
  them rather than writing a second copy *(corrected: JSC-45)*.

### JS-2 — Take the snapshot; make the copied front end this component's own code

- **Owner:** profile front-end owner, with the release owner co-signing the attribution change.
- **Ledger:** JS-2's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and — once the row is `In progress` — every open clause of the gate below.
- **Next action:** Record the snapshot recursively. Copy the tokenizer, the syntax tree and its
  visitors, the parse-time binding and scope analysis, the free-name analysis, and the allocation
  and string primitives. Decide and record whether the few neighbouring primitives the tree
  consumes are copied in or replaced. Rename every namespace to match its assembly on the first
  commit. Delete the dead attribute family and every conditional-compilation directive. Replace
  the ambient parse-goal and top-level-await reads with an explicit options value. Take the
  deep-nesting decision of [section 9](roadmap.md#9-the-semantic-front-end-and-lowering). Annotate
  every copied unit as ported.
- **Dependencies:** JS-1, plus **one external gate: the core contract accepted**, which this
  component does not hold and which the ledger records as a named blocker with its holder and its
  unblock condition. Three things this milestone does not wait on, because JS-0 took them: the
  per-item ruling of
  [section 4.2](roadmap.md#42-what-after-the-fix-work-lands-can-and-cannot-mean), its stop
  condition, and the nullable and unsafe positions the seed forces
  *(corrected: JSC-03, JSC-04)*. **Taken is not applied**:
  enabling nullable on the copied files, confining `AllowUnsafeBlocks` to the lowering project, and
  adding the rule that asserts that distribution are all this milestone's work.
- **Objective exit gate:** The snapshot identity is recorded recursively and re-derivable; **the
  seed's own gates are re-run from this component at the snapshot commit, and the defect
  [section 4.1](roadmap.md#41-the-snapshot-identity) inherits is either fixed before the snapshot or
  recorded as a named exception** — an inherited reading is not a verified one; the
  two-way boundary rule passes with its witnesses; the copied front end builds with the trim and
  AOT analyzers **force-enabled**, producing zero trim and AOT warnings **anywhere in its
  reference closure** rather than merely none attributed to the project, and a metadata scan finds
  no IL-emission assembly reference; scans assert zero conditional-compilation directives in
  covered files, zero occurrences of any legacy assembly name in any namespace, header, or
  documentation comment, and zero uses of assembly loading, name-based type resolution, activator
  construction, run-time generic construction, dynamic-method emission, IL generation, module
  initializers, or reflective member read or write; the parser takes goal and top-level-await
  permission as constructor arguments, a metadata scan finds no thread-static field and no
  ambient async-local type in the assembly, and **two parses with different goals run
  concurrently in one process each producing the goal-appropriate result, in a test that fails
  when the options are replaced by a shared static**; a nesting corpus proves a deeply nested
  program is refused rather than terminating the process; every relevant copied unit carries a
  parsed annotation with a current fingerprint, no placeholder, and a falsification criterion on
  every unit assessed at the top of the security vocabulary; the licence and notice changes are
  landed and the core's standing third-party claim is confirmed scoped or amended; and a **scan**
  over this component's roadmap and evidence tree finds no identifier from any other component
  cited as evidence.
- **Seed:** [Section 4.3](roadmap.md#43-the-copy-table)'s copy table. Not copied: the
  expression-model seam, the interop surface, the dynamic-metaobject surface, the module hosts,
  the dead attribute family, and the module-initializer bootstrap.

### JS-3a — The diagnostic registry and the oracle, standing before the copy arrives

**This milestone stands behind neither of the two things that hold JS-2, and that is why it is a
milestone of its own.** Nothing in the oracle method of
[section 14](roadmap.md#14-the-conformance-oracle) needs a copied line: it needs a scoring target,
and JS-1 produces one — every verifier outcome reached by a named retained corpus case, over a
corpus that replays with no residue and contains passing controls. Fusing the harness to the
static-semantics work would put this component's only external correctness signal behind the core
acceptance gate and behind the seed's snapshot, and a team that serialised them would spend the
whole acceptance wait with no oracle *(corrected: JSC-15)*.

**It deliberately does no static semantics and no lowering**, which are JS-3b's. A milestone that
stands up the oracle *and* consolidates the early errors would have its hardest scoping question
answered by whichever of the two ran late.

- **Owner:** conformance owner, with the verification-boundary owner for the registry half.
- **Ledger:** JS-3a's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and — once the row is `In progress` — every open clause of the gate below.
- **Next action:** Publish and version the diagnostic-code registry and the position encoding,
  stating which of the core position record's four fields this profile populates, what it carries
  in the two profile-owned coordinates, and what a section index of `-1` means here. **Record
  which half of the registry each code belongs to** — codes a verified artifact's rejection
  carries in a core result, and codes a pre-artifact source rejection carries on the embedder's
  own seam — per [section 9](roadmap.md#9-the-semantic-front-end-and-lowering)'s boundary
  question, which this milestone answers. Then build the harness, the self-check, the sharding,
  the merge, the scope manifests, and the audit command, and score the slice manifest.
  **The harness is built against the smallest scoring target that exists rather than after a
  revision is pinned**, per [section 14](roadmap.md#14-the-conformance-oracle) — the harness, the
  self-check, the sharding, the merge, the per-host-mode totals, the ratchet and the
  never-advertised composition root are in the checkout from 2026-09-03, scoring a fixture suite of
  this component's own; the scope manifests and the audit command are not, and neither is a pinned
  third-party revision, without which every run reports `MissingSuiteRevision`. **The ingestion
  path is in the checkout from 2026-09-03 too** — the dialect a real suite writes, the two
  strictness readings of one file, and the rule that only an earned refusal answers a question
  about the language *(corrected: JSC-53, JSC-54)*. It was built because the claim that the reader
  could be pointed at a real suite rather than replaced turned out to be false when it was tested.
  **What is left of this clause is the retrieval, and nothing else.** **In the change that first ingests a suite file, land the ingested suite's
  attribution row in the host component's notice**, mark modified suite files as changed, and
  re-confirm the core's standing third-party claim against what the ingestion adds to the tree, or
  amend it, with the release owner co-signing — the second of this component's two licence
  obligations, which
  [section 4.5](roadmap.md#45-licence-attribution-and-one-notice-that-must-change)'s seed change
  does not discharge and which no milestone before this one can take, because the notice content
  is the suite's own and does not exist here until the revision is retrieved.
- **Dependencies:** **JS-1 only.** Deliberately not JS-2, not the copy, and not core acceptance.
- **Objective exit gate:** The public API baseline clause carried from JS-0 closes here, over a
  baseline of this family's own, described from the build output **without loading or running
  anything** and compared in both directions so an addition, a removal and a signature change each
  fail it. The diagnostic-code registry is published, versioned, and bound in
  **both** directions — every emittable code appears in it, and every code in it is reachable from
  a named case **or is one of a rule-held list of defensive rows that no artifact reaches, each
  stating why**, because a defensive arm deleted for being unreachable answers with some other
  code at the moment the answer matters; each code maps onto exactly one core reason with no
  invented or aliased reason, and the registry states its own revision so that a retained corpus
  entry recording a code can be dated, because a code that changes meaning between releases
  silently invalidates every corpus entry that recorded it; **the self-check runs against the built
  profile before every shard** and every deliberately broken fixture returns its declared verdict
  alongside at least one passing
  control, with a negative control that injects a scoring regression, observes the mismatch, and
  reverts; the slice manifest runs to completion and publishes per-host-mode totals from an exact
  commit and an exact suite revision, and **that run sets the ratchet**; removing one shard's report
  reports incomplete coverage rather than a smaller total, a configuration field differing between
  shards reports a named inconsistency, and an empty selection and an all-skipped selection are each
  named configuration failures; negative-metadata tests are executed and reported as their own
  totals, with the uncaught error matched on its JavaScript type name; the failure manifest is
  proved to be a queue by a case where a listed path still fails and a case where a hand-written
  entry does not survive; the harness, merge, audit, and scope tooling each carry their own
  regression tests run before any shard starts; **no aggregate percentage is published, then or
  ever**; the effective limit vector each run was obtained under is published with its totals,
  because a total obtained under generous ceilings is not the total a product with tight ones would
  get; **the language-specification edition is pinned by immutable revision identifier, retrieved,
  hashed and archived, and the pin actually taken is recorded — or the exclusion naming it
  provisional is carried in the ledger with its holder and its unblock condition**, which is the
  clause [section 24](roadmap.gates.md#24-specification-and-platform-references) and
  [section 3 of the ledger](roadmap.status.md#3-open-external-dependencies) both put on this
  milestone and which this gate did not previously carry *(corrected: JSC-36)*: no manifest may be
  accepted against an unpinned edition, this is the first milestone that scores one, and a pin
  nobody can fail a gate over is a pin nobody takes;
  **the ingested suite's attribution row is landed in the host component's notice in the same
  change that first ingests a suite file, modified files are marked as changed, and the core's
  standing third-party claim is re-confirmed against what the ingestion adds to the tree or amended,
  with the release owner co-signing** —
  a clause no earlier milestone could close, because
  [section 4.5](roadmap.md#45-licence-attribution-and-one-notice-that-must-change)'s notice change
  is the seed's and the suite's own notice content is not in this checkout until the revision is
  retrieved *(corrected: JSC-30)*; **the harness is a composition root that is never advertised,
  and not a test project** — it drives this profile's own lowering, verifier and executor, which
  rule A11 forbids a test project to reference, and the composition register records it as never
  advertised *(corrected: JSC-40)*; and a scan asserts the suite ingestion path, its cache, and
  every suite file appear in no product package and in **no advertised composition's** closure
  report — not in *no closure report*, because the harness root publishes one of its own — with a
  negative control that adds the reference to the ingestion path **from the execution-only root**
  and observes the scan fail.
- **Seed:** Nothing is copied. Every mechanism here is this component's own code, and **no total,
  manifest entry, known-gap entry, or triage finding crosses the fork.**

### JS-3b — Static semantics as one verification stage, and the lowering

- **Owner:** verification-boundary owner.
- **Ledger:** JS-3b's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and — once the row is `In progress` — every open clause of the gate below.
- **Next action:** **The slice half is built** — the five answers
  [section 9](roadmap.md#9-the-semantic-front-end-and-lowering) asks for are taken in
  [JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md), over a
  tokenizer, a parser, one validation stage and a source lowering written in this checkout
  *(corrected: JSC-43)*. What is left is the half that needs something this component does not
  have: **the parse-and-early-error slice scored on JS-3a's harness against the ratchet**, which
  waits on the suite-revision dependency, and **a publish-and-run of the narrow-runtime-compiler
  composition on every claimed RID**, which is a collection nobody has made. **The first of those
  two is now waiting on one thing rather than three** *(corrected: JSC-53)*: the harness exists,
  and so does the path that reads a real suite's dialect and translates it — including the arm this
  clause depends on, which is that a parse-phase negative never executes and so never needs the
  assertion library this manifest cannot load. The suite itself is what is missing. The general front
  end — functions, objects, strings, `try`, modules — is still JS-2's ingest.
- **Dependencies:** JS-1 for the format and the verifier shape; JS-3a for the registry the
  diagnostics land in, and for the harness the scoring clause needs. **JS-2 is a dependency of the
  general front end and was never one of a front end at all** *(corrected: JSC-43)*: the slice
  surface was written from the grammar, and waiting for the ingest bought no code while costing
  every decision section 9 left open.
- **Objective exit gate:** Every early error the manifest requires is produced by a named case
  carrying a registry code; an illegal format-version and manifest pair is refused by this profile's
  own verifier with a diagnostic code; a construct outside the declared manifest is refused at
  verification and not at first execution, by its own case; **an artifact that is both malformed in
  framing and invalid in static semantics reports exactly one of the two, by a named case that fails
  when the phases are fused**; each source is tokenized at most once during compilation and the
  verifier tokenizes nothing, each asserted by a case *(corrected: JSC-46)*; **two parses with
  different goals run concurrently in one process, each goal-appropriate, in a case that fails when
  the options are replaced by a shared static, with a scan asserting the assembly holds no state
  that could outlive a call**; **a nesting case is refused rather than surviving, and a process
  termination on one blocks this milestone**; **the same source compiles twice to identical
  bytes**; the parse-and-early-error slice is scored on JS-3a's harness against the ratchet; and
  the narrow-runtime-compiler composition publishes and runs on every claimed RID with warnings as
  errors, its closure containing the tokenizer and the lowering and no test assembly, and cited
  as evidence for no other composition kind.
- **Seed:** **Nothing is copied for the slice surface** *(corrected: JSC-43)*. The tokenizer, the
  syntax tree, the parser, the one validation stage, the seam diagnostic vocabulary and the source
  lowering are this component's own code, and no total, manifest entry or triage finding crosses
  the fork. The copied-and-re-homed row — the post-parse validation stage and the free-name
  analysis — still describes the general front end, which is JS-2's and is blocked with it.

### JS-4 — The value representation and the object model

- **Owner:** profile runtime owner.
- **Ledger:** JS-4's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and — once the row is `In progress` — every open clause of the gate below.
- **Next action:** Implement the eight-row ABI the entry gate already fixed, retaining the
  correctness fixtures and Native AOT representation probes the gate deferred to this milestone.
  Copy the property storage with its tests and its recorded defect history. Replace the reflective
  key-table initialiser with a generated table under a named owner and make key identity
  realm-scoped. Amputate the dynamic-metaobject interface from the value base type. Route what the
  front end and the executor need through a realm object the composition creates. **Delete JS-1's
  hand-written encoder and its hand-written programs**, and assert the deletion; the instruction
  buffer beside them stays, because it is JS-3b's source lowering's back end
  *(corrected: JSC-45)*.
- **Dependencies:** JS-1 and JS-2. The [section 8](roadmap.md#8-the-value-frame-and-call-model)
  ABI decision is a **gate on entry** and is taken; a taken entry gate is not a started milestone,
  and this one still waits on JS-2.
- **Objective exit gate:** The numbered ABI decision exists with all eight rows, with fixtures and
  AOT representation probes retained; the object model builds with analyzers force-enabled and
  zero trim and AOT warnings in its closure, and a metadata test finds no dynamic-loading,
  reflection-invocation, IL-emit, reflective-member-write, thread-static, or ambient async-local
  construct, **each clause with its own witness**; two runtimes in one process each mint
  properties under the same key text and neither observes the other's storage, shape identity, or
  key identity — **asserted over the property directly, because the falsifier this clause used to
  name has nothing to switch on**: *a test that fails when the key table is made process-wide
  again* was written against the seed's interned key table and shape-transition table, JS-2 is what
  would bring them, and a store written here is a dictionary owned by one object
  *(corrected: JSC-195)*; two
  separately compiled programs whose first cache slot carries the same index run in separate
  runtimes and are evicted with no state crossing owners; two runtimes read one shareable handle
  concurrently with no synchronisation and a **structural scan** asserts no instance-owned cache,
  shape table, feedback, or warmed structure is reachable from a handle, with the scan's mechanism
  and its residual stated; each defect the copied storage carries in its recorded history has a
  named regression that fails when the fix is reverted; the copied storage's direct test coverage
  is **measured, not merely recorded**, with covered types named and uncovered public behaviour
  named with an owner, and closed to a stated line before the milestone closes; the representation
  decision is exercised by a retained figure per value kind under
  [section 17](roadmap.gates.md#17-measurement-discipline)'s rules; and JS-1's encoder and
  lowering are gone, asserted by scan.
- **Seed:** Copied with tests — shapes and the transition table, shape-only slot storage with its
  one-way materialization boundary, element arrays, the named-property store. Rewritten — the
  interned key table, the ambient context. Written fresh — the value representation, if the
  decision replaces the hierarchy.

### JS-5 — The executor: frames, calls, abrupt completion, and the budgets it charges

- **Owner:** profile runtime owner.
- **Ledger:** JS-5's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and — once the row is `In progress` — every open clause of the gate below.
- **Next action:** Implement the interpreter over the ABI. Implement abrupt completion so
  `finally` runs on every applicable exit including a host exception crossing profile frames.
  Place every poll and every charge. Measure the per-frame cost on each claimed RID and derive the
  `CallDepth` default from it *(corrected: JSC-27)*. Choose the uncharged-work bound, the
  charging granularity, and the cancellation poll bound from measurement. Catch every internal
  exception at this profile's own adapter. Run the vertical-slice loop until the first executable
  increment of `broiler.javascript.core` is complete.
- **Dependencies:** JS-4.
- **Objective exit gate:** Every executor answer is one of the five step kinds and a scan asserts
  no profile code names a core outcome category; a retained nested-handler and `finally` matrix
  passes in both directions across the boundary, covering `return`, `break`, `continue`, a
  language throw, and a host exception, with return and throw replacement by `finally` covered,
  the host exception surfacing as a host failure and a language throw as a typed payload behind a
  profile fault; **the host boundary is proved at binding time** — a value capability whose
  version, signature ID, or kind does not match a declared import is refused when the runtime is
  created and not at first call, each mismatch by its own named case; a failed required import
  leaves no partially bound runtime, asserted by a case that finds no usable runtime after the
  refusal; the unbound branch of at least one optional import is exercised; a scan asserts every
  argument and result crossing the boundary is one of the core's transfer types and no CLR type
  crosses it; and the translation precedence is proved per capability, a cancellation exception
  carrying the operation's own token as cancellation, an exhausted meter at the moment of the
  catch as resource exhaustion, and anything else as a host failure naming the capability; **no
  exception escapes the executor** across the increment's corpus; the `CallDepth` default is
  derived from a retained, reproducible frame-cost measurement on each claimed RID, and a
  recursing program is refused as resource exhaustion naming `CallDepth` and its scope **rather
  than terminating the process**, on every claimed RID under Native AOT; a deliberately
  non-polling variant completes as a profile fault with the poll-bound reason and the runtime
  poisoned to accept only disposal; **a proportionality fixture exists for each named operation
  family of [section 8](roadmap.md#8-the-value-frame-and-call-model) *that this increment ships***
  — which is not all of them, because regular-expression matching arrives with the matcher at JS-6
  and the string and array families arrive with the library, and each family's fixture arrives with
  its operations *(corrected: JSC-36)* — each with an unsimplified
  control, each showing fuel charged as a monotone non-decreasing function of input magnitude and
  at least the declared ceiling, with the declared function and granularity recorded — and an
  operation family without a fixture does not ship in the increment; a deliberately non-charging
  variant is detected and reported as a contract violation; each new opcode adds corpus entries
  covering its structural, index, and stack-consistency rejections; and the increment's suite
  results are published against the ratchet from an exact commit with the failure manifest
  regenerated and no host mode regressed.
- **Seed:** Copied and re-expressed — semantic operation bodies, value-conversion rules, the call
  surface and the identities a call must preserve. Written fresh — the opcode set and its
  encoding, the dispatch loop, every metering call, and the frame-cost measurement.

### JS-6 — The standard library

**This milestone is a rewrite, not a copy, and it was scoped that way before it started.** The
seed's library is typed against a boxed value base type this profile does not adopt, so it is
re-implemented against the value struct rather than copied and re-typed. **The storage half of
[the copy table](roadmap.md#43-the-copy-table) is untouched** — shapes and the transition table,
element arrays and the named-property store are about storage keyed by a value, not about the
value's representation — and the milestone keeps its place in the order; what changed is its size
*(corrected: JSC-17)*.

- **Owner:** profile built-ins owner, with the satellite-acquisition owner outside this component.
- **Ledger:** JS-6's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and — once the row is `In progress` — every open clause of the gate below.
- **Next action:** Copy the registration source generator and its attribute vocabulary, changing
  its generated prototype lookup to take a realm parameter. **Re-implement the core library
  against this profile's value struct**, taking the seed's semantics as specification and its
  tests as a port, and publish a scope estimate, a review budget, and an exclusion list on the day
  it lands — a rewritten library is smaller than a copied one and the difference is a support
  claim. Mint separate manifest identities for the temporal, internationalization, and
  regular-expression surfaces and leave all three out of `broiler.javascript.core`. Acquire the
  regular-expression matcher and the Unicode and locale data as this checkout's own dependencies
  and drop the dead date-time reference. Route regular expressions through the from-scratch
  matcher. Delete the module-initializer wiring — the initializer bodies and the satellite
  initializer files, and only those — after re-homing into the library proper the prototype
  patching that the same file happens to register. Delete the assembly probing.
- **Dependencies:** JS-3b for the general lowering, JS-4 for the object model, JS-5 for calls.
  **Satellite acquisition is an external dependency opened at JS-0**: if it has not landed, the
  first manifest excludes every surface that needs it and publishes each exclusion with its
  deterministic failure, rather than this milestone waiting.
- **Objective exit gate:** The library's closure contains no IL-emission assembly **and no call
  site constructing a compiled-mode regular expression**, each asserted by its own metadata test
  with its own witness; the generator's emitted output is compiled and walked and contains no
  run-time reflection and no ambient context read, failing when the realm parameter is replaced by
  an ambient; `broiler.javascript.core` is declared and an artifact naming an unaccepted manifest
  is refused; the ported library tests run against this component's object model with the pass
  count, the covered list, the excluded list, and a justification per exclusion recorded — and
  the milestone does not close on a recorded number alone: zero unexplained failures, every
  exclusion owned; **the exclusion list a rewrite makes necessary is published rather than
  discovered**, with the review budget the rewritten units carry; the satellites resolve from this
  checkout with nothing resolving outside the component root; and the compositions from JS-1 and
  JS-3b still publish and run with the library linked, closure reports unchanged in shape.
- **Seed:** Copied — the source generator and its attribute vocabulary. Taken as specification and
  re-implemented — the core library, against this profile's value struct. Ported and labelled as
  ported — the library's tests. Deleted at ingest — the dead attribute family, the dead date-time
  reference, the module-initializer wiring itself, the assembly probing. Re-homed rather than
  deleted — the prototype patching that wiring registers. Excluded by name — the interop assembly
  and the module hosts.

### JS-7 — Suspension: generators, async functions, top-level await, terminal unwind

- **Owner:** profile runtime owner.
- **Ledger:** JS-7's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and — once the row is `In progress` — every open clause of the gate below.
- **Next action:** Make the executor's continuation capturable and reconstitutable on the heap.
  Implement generators and async functions on it. Take and record the routing decision of section
  12 per pause kind, **with the live-suspension count a representative workload produces**.
  Declare asynchronous instantiation and implement top-level await. Decide and declare external
  suspension. Write the terminal-unwind entry point and defend the abandon budget. Publish the
  safepoint-density statement.
- **Dependencies:** JS-5 for the frame model, JS-6 for the prototypes and job-queue types
  generators and promises need. **The JS-7/JS-8 edge runs one way only**: JS-8 depends on JS-7's
  continuation capture, and JS-7 depends on nothing JS-8 delivers. Where a module graph's
  dependencies arrive through the mediator, that is a JS-8 concern operating on a JS-7 mechanism
  — and a guest-initiated load may not itself suspend, which is what keeps the edge acyclic
  rather than merely asserted to be.
- **Objective exit gate:** A generator and an async function each suspend and resume across at
  least two suspensions, **proved by a test that resumes on a different thread than the one that
  suspended**; a second resume, a resume after cancellation or disposal, and a resume presented
  to a runtime that does not own the continuation each return the named invalid-state reason; a
  suspended operation is cancelled and disposed **without ever being resumed**, on the disposing
  thread, with no instance published, the terminal unwind run under the tighter of the abandon and
  unwind budgets, and the release order observed; a budget snapshot across a suspension shows fuel,
  allocated bytes, host calls, and the nested-load counters frozen, the wall clock paused under
  every origin, and live bytes and live runtimes still metered; a module with top-level await
  suspends during instantiation, publishes **no** instance while suspended, resumes to a live
  instance, and a resume that suspends again is covered, while an undeclared park returns the
  named invalid-state reason and is not resumable; a composition that does not enable external
  suspension answers `ExternalSuspensionNotEnabled` and a descriptor that does not declare it
  answers `ExternalSuspensionNotDeclared`, distinguishably; the residency and live-suspension
  bounds each have a named case; the routing decision is recorded with its count; the terminal
  unwind runs no guest code able to request a load or to suspend, asserted by a case; a scan
  asserts no public member returns a task, value task, or custom awaitable, no product type
  implements a completion-notification interface, and no product assembly references a timer,
  delay, or thread-abort API, **each clause with its own witness**; and the suspension and handler
  framing add their own corpus entries.
- **Seed:** Copied as specification only — completion-record semantics, abrupt-completion cases,
  generator resumption semantics, module specifier and binding semantics. Written fresh —
  continuation capture by unwinding rather than by an IL-emitting state-machine rewriter, the
  suspension projection, the terminal-unwind entry point, and every test that pins a pause.

### JS-8 — Guest-initiated loads and the three compositions

- **Owner:** profile security owner with the host-capability owner.
- **Ledger:** JS-8's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and — once the row is `In progress` — every open clause of the gate below.
- **Next action:** Declare guest-initiated loads with finite maxima for all four bounds and a
  defended verifier-work-to-fuel rate. Route `eval`, the `Function` constructor, and dynamic
  `import()` through the mediator and remove every alternative byte source. Implement the
  conversion table. Replace the textual direct-`eval` decision with one the front end records, or
  record the deviation. Build the two compositions the claim needs — one registering a provider,
  one registering none — plus the general-runtime-compiler root.
- **Dependencies:** JS-5, JS-6, JS-7, and JS-0's placement ruling for where the lowering assembly
  may be referenced from.
- **Objective exit gate:** The declaration is admitted and named negative cases produce each of
  the guest-load catalog refusals; an architecture test asserts the profile assembly reaches no
  filesystem, socket, embedded resource, byte-returning host object, or in-process lowering
  shortcut, **with the check's mechanism and its residual stated**; registering value capabilities
  never satisfies an artifact-provider import, proved by a composition that registers only value
  capabilities and is refused when the runtime is created; a composition registering no provider
  refuses every request **before the request payload is inspected**, and a test asserts the
  refusal counter is non-zero on an operation that completed normally because guest code caught
  the resulting language error; the admission order is asserted step by step — depth, then
  fan-out, then already-exhausted allowances, all before the provider is called; then one
  host-call unit plus elapsed wall clock; then the returned length against the nested-bytes bound
  with an over-bound artifact **dropped unverified**. **The depth step is asserted for its
  unreachability, not for its ordering**: at core contract version 1 a nested load hands back a
  verified handle with no path to a nested core instantiation, and a provider is mandatorily
  non-reentrant, so nesting is bounded at one by construction and a failing depth case cannot be
  constructed. The core carries this as a standing exclusion; this milestone cites it, proves the
  unreachability from the public surface rather than asserting an order whose violation is
  impossible, and states the consequence for this language plainly — **a chain of `eval` calls
  consumes fan-out, not depth**, and fan-out is a per-operation counter whose reset the core's own
  measurement lane once found defective, so the fan-out assertions here are the load-bearing ones.
  A guest-initiated-origin handle is **ineligible for any persisted envelope and contributes to no
  persisted cache key**, asserted by a case over the module map of
  [section 11](roadmap.md#11-guest-initiated-loads-eval-the-function-constructor-dynamic-import-modules),
  which legitimately caches handles and is therefore where the rule has to bite; the conversion
  table passes case by case, with a variant surfacing an unconverted nested failure reported as
  such, and nested exhaustion and cancellation each proved **uncatchable from guest code** with
  bounded unwinding; a mediator used past its invocation is refused; a nested handle presented to
  a second runtime is refused **before** identity comparison and no member hands one to the host;
  the malformed corpus is **replayed through the nested path**; **the `eval`-refusal half of JS-0's
  two-profile catalog test closes here**, by a case that composes this profile beside a neighbour
  which writes a zero into a guest-load *default*, adopts defaults rather than stating ceilings,
  and observes `eval` refused with a resource exhaustion naming the dimension — this is the first
  milestone with an `eval` to refuse, which is why JS-0 carried the clause rather than fabricating
  a descriptor for it; and each of the three compositions
  publishes and runs on every claimed RID with warnings as errors, the execution-only closure
  containing no lowering and each runtime-compiler closure containing one, with no publish cited
  as evidence for another kind.
- **Seed:** Copied and rewritten — the single runtime-owned indirection the two dynamic entry
  points already funnel through, which becomes the mediator adapter; specifier resolution and
  import-syntax lowering, re-homed into the lowering sibling; the direct-`eval` early-error
  validation. Written fresh — the declaration and its bounds, the conversion table, the provider
  adapter, and the direct-`eval` decision.

### JS-9 — Adversarial input, agents, and soak

**This milestone opens against JS-1 and closes after JS-8.** The retained corpus grows from the
first product code onward, and the soak and the shared-aggregate-budget exercises need nothing a
later milestone delivers, so the work is schedulable immediately. What holds the milestone open is
the gate rather than the work: two of the four untrusted-input surfaces it must fuzz — the source
parser and the regular-expression matcher — were both absent until JS-3b, which wrote the first
of them; the matcher waits on JS-6. **A surface that exists and is unfuzzed is a gap rather than an
absence** *(corrected: JSC-47)*, and a session over
surfaces that do not exist may not be read as covering them *(corrected: JSC-23)*.

- **Owner:** profile security owner with the fuzz-corpus owner.
- **Ledger:** JS-9's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and — once the row is `In progress` — every open clause of the gate below.
- **Next action:** Grow the malformed corpus from slice scope to the full format. **Fuzz all four
  untrusted-input surfaces** — the verifier, the source parser, the regular-expression matcher
  over pattern and subject, and the executor over verified-but-adversarial artifacts — with
  recorded seeds, budgets, and runtime settings. Design and implement retained-bytes reporting
  over the object model and state the limits of what it measures. Run a soak over recycled
  runtimes. Exercise sibling runtimes under one aggregate budget.
- **Dependencies:** **To open, JS-1 only.** To close, JS-5 through JS-8 — for the full format the
  corpus must cover, the object model the retained-bytes report must measure, and two of the four
  fuzz surfaces.
- **Objective exit gate:** Every entry in the full corpus produces its recorded outcome, reason,
  and diagnostic code on JIT, trimmed, and Native AOT hosts, the verifier throws on none, control
  entries verify successfully, and a repeat leaves no residue; **each of the seven budget
  dimensions [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier) names as an exhaustion
  answer — the reader's structural depth, section count and declared counts; the allocator's
  allocated bytes and the work charge's verifier work; the poll's wall clock; and artifact bytes,
  which the core answers one call before this verifier is entered — is
  reached by a corpus entry of its own that records the dimension and the scope it named**, which
  needs a manifest column for the pair, because an exhaustion answer carries no diagnostic code and
  the registry's both-directions binding therefore reaches none of the seven
  *(corrected: JSC-39, JSC-41)*; where a dimension is unreachable at
  the manifest this milestone closes against, the bundle names it, names the milestone that makes it
  reachable, and carries the entry as owed rather than as absent — the clause
  [the map](#25-the-chapter-milestone-and-gate-map) and
  [section 21](roadmap.gates.md#21-test-and-evidence-matrix) both put on this milestone and which
  this gate did not carry *(corrected: JSC-37)*; a **mutated corpus entry** proves
  the replay detects a changed observed triple; each fuzz session retains its corpus identity,
  its iteration budget with a stated floor, its runtime settings, and **every minimized
  counterexample**, and any counterexample is closed by a **named regression, never an allow-list
  entry**; **each fuzz session is guided — it observes what a mutant reached and keeps
  the mutants that reached something new as further seeds, so its seed set grows with the surface
  it explores rather than staying the retained corpus** — and a session that mutates a fixed seed
  set is seeded mutation, which its bundle says in those words and which closes nothing here,
  because that is the discipline [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier)
  and [section 21](roadmap.gates.md#21-test-and-evidence-matrix) both ask for and this gate did
  not carry *(corrected: JSC-38)*; **what a session observes is the answer this profile publishes
  and not an edge, so the bundle states that bound where it states the guidance, and a session
  judges its own loop rather than its growth — every mutant it drew was offered to the seed pool,
  and the pool keeps a new answer while refusing a repeat — because how much a seed set grows is a
  fact about the corpus and a gate that failed on it would fail harder the better the corpus got**
  *(corrected: JSC-42)*; the compile-time nesting bound holds under fuzz; a soak over a recorded number of
  lifecycle cycles across recycled runtimes reaches a stated heap plateau and a disposed runtime
  leaves no per-thread state, each with a named regression that fails when the fix is reverted;
  two runtimes under one aggregate budget together spend no more than the parent's allowance,
  disposing a parent with live children is refused, sealing drains, and **no test asserts which
  sibling observes a shared-parent exhaustion**; and every negative control in this milestone's
  bundle fails when injected and passes after revert, with the running count recorded.
- **Seed:** Copied and rewritten — the corpus manifest schema, the negative-control discipline,
  the collection script that judges nothing. Written fresh — every corpus entry, every fuzz
  result, every retained-bytes report, every measurement. Defects the seed recorded are
  **hypotheses this component may test, carried without their numbers.**

### JS-10 — Baselines, packaging, the support table, and the release gate

- **Owner:** release owner with the package, security, API, performance, and documentation owners.
- **Ledger:** JS-10's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and — once the row is `In progress` — every open clause of the gate below.
- **Next action:** Stand up the controlled measurement lane and take this component's own
  baselines under [section 17](roadmap.gates.md#17-measurement-discipline), **including the two
  figures other chapters open an amendment against: verification throughput per byte, and
  cold-start cost.** Neither is optional: [section 16](roadmap.md#16-persistence-and-the-code-cache)
  makes the persistence question reopen against them and
  [section 18](roadmap.md#18-amendments-this-profile-expects-to-ask-of-the-core) opens the
  in-process-producer row against them, so a milestone that produces neither leaves two rows opened
  by nothing. Resolve JS-0's packaging decision into a shipped identity or a stated refusal.
  Publish the support table and the composition register. Claim a RID only where a retained bundle
  published and ran the named composition on it. Run the release gate that refuses the tree while
  any relevant unit lacks a human decision.

  **Three records this milestone takes because no earlier one can, and each is a state rather than
  a decision.** Publish the state of
  [section 18](roadmap.md#18-amendments-this-profile-expects-to-ask-of-the-core)'s amendment
  register — per row, filed, held or opened, the deterministic failure or named exclusion it leaves
  standing, and that the procedure is unexecutable — because a held amendment is a capability this
  profile does not provide and gate 1 already requires every one of those to be named. Record this
  profile's half of the extraction-gate comparison, or record that its first condition is
  unsatisfied and what would satisfy it; the condition is a second product profile's verifier
  having merged, which this component cannot schedule, so the state is recorded here whatever it is
  and **no verdict is recorded at all**. And name the declared-default vector as the neighbour-facing
  half whose reconciliation is unowned, because
  [section 15](roadmap.md#15-deployment-compositions-native-aot-and-the-browser-embedding) puts that
  reconciliation in a component that does not exist *(corrected: JSC-31, JSC-32, JSC-33)*.
- **Dependencies:** JS-3a and JS-9 for evidence, JS-8 for the composition set, JS-0 for the
  packaging ruling, and **a named human reading every relevant unit** — the largest
  single-owner task in the programme, decomposed and scheduled rather than assumed.
- **Objective exit gate:** Every published figure declares exactly one evidence class and returns
  exactly one predeclared decision, with an immutable manifest written before either arm ran, a
  comparable control, an A/A lane result, every repetition retained, and each measured child's
  effective configuration reported — and a candidate-versus-control difference smaller than the
  A/A difference is reported **below resolution**, not as a result; the baseline register and the
  retained log agree in both directions on both lanes, asserted by a rule; the support table names
  the core contract version **implemented** and the minimum **accepted** as two separate integers,
  plus the accepted format-version range, the accepted manifest set, **the pinned
  language-specification edition and the pinned suite revision by immutable identifier**, and the
  conformance manifest identity and version; **it publishes the list of surfaces this profile
  declares varying rather than fixed**, per
  [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted), because a
  determinism claim broader than that list is an untruthful support claim and the list is what
  bounds it — both of these being clauses
  [release gate 1](roadmap.gates.md#22-release-gates) has always asked for and this gate did not
  carry *(corrected: JSC-36)*; it uses a vocabulary that never reads as a bare yes, gives every
  row an evidence cell naming a rule or a retained artifact, names a deterministic failure or an
  exclusion for every unimplemented capability — **the `WebAssembly` host-object surface among
  them**, named rather than left to be inferred, because a browser image containing this profile
  beside another one is exactly where a reader will assume the namespace works — distinguishes
  what the contract admits from what
  this profile implements from what each composition provides, and closes with a section stating
  what the table does not say; **the accepted manifest set contains no manifest whose oracle
  totals show it failing**; the composition register and the checkout agree in both directions;
  every claimed RID has a retained publish-and-run bundle with its closure report, and every
  unclaimed one is listed with its reason; **verification throughput per byte and cold-start cost
  are each published under the measurement rules**, because two chapters name them as the trigger
  that would reopen a settled question; a pristine consumer restores and runs from a source
  containing only this component's packages with upstream feeds unreachable, and a rollback to the
  previous package set runs unchanged; the release gate refuses on each of its conditions, naming
  each blocker by its declaration, with a negative control proving the generator cannot invent a
  reviewer; a named human decision exists on **every** relevant unit before the first publish;
  every suppression is inventoried with an owner and a reachability argument; **the operational
  holders [release gate 13](roadmap.gates.md#22-release-gates) asks for are each named —
  diagnostics, cancellation, rollback, format-version rejection, corpus and suite-revision drift,
  vulnerability response, and recertification** — with a vacant one recorded vacant rather than
  passed to whoever is available, which is the clause
  [the map](#25-the-chapter-milestone-and-gate-map) has always said this milestone closes and which
  this gate did not carry *(corrected: JSC-36)*; **the amendment
  register's state is published, row by row, with the deterministic failure or named exclusion each
  held row leaves standing and with the procedure's unexecutability stated** — asserted by a rule
  that fails when a row of
  [section 18](roadmap.md#18-amendments-this-profile-expects-to-ask-of-the-core) has no
  corresponding published state; **the extraction-gate state is recorded — this profile's half, or
  the first condition unsatisfied with what would satisfy it — and a scan asserts no verdict and no
  identifier from another profile component appears in it**; **the declared-default vector is
  published as the neighbour-facing half with its reconciliation named as unowned**, so a browser
  composition meets it in the table rather than in a resource exhaustion naming a dimension this
  profile did not breach; and no figure,
  total, claim, or platform result from any other component appears anywhere.
- **Seed:** Nothing. Every figure is this component's own, from this component's own lane and
  commit.

---

## 20. Delivery order

**From 2026-10-03, the order of the work that remains is [section 26](#26-the-road-to-a-full-featured-profile)'s.**
The diagram below is the dependency order the milestones were planned in, and every gate in section
19 still cites it; most of its nodes have code in this checkout, and none is accepted.

```text
     JS-0  boundary, placement, identity, assurance floor, evidence contract
        │        no copied line yet, no product code
        │
        └→ JS-1  the whole contract loop on a narrow slice, written fresh
             │        publish-and-run on the smallest closure
             │
             ├→ JS-3a registry, position encoding, pinned suite, the harness
             │        ←── the suite revision is pinned (a human action)
             │        ←── an external correctness signal from here on, and it
             │            is behind NEITHER the core acceptance gate nor the
             │            snapshot
             │
             ┊··→ JS-9 OPENS here and closes far below. The retained corpus
             ┊        grows from JS-1 onward, so the corpus, the soak and the
             ┊        aggregate-budget half of its gate need nothing a later
             ┊        milestone delivers — while two of its four fuzz surfaces
             ┊        do not exist until JS-3b and JS-6, which is what holds
             ┊        the milestone open rather than the work.
             │
             └→ JS-2  seeding snapshot; the front end becomes this component's code
                  │        ←── (core contract accepted): external gate, held by
                  │            the core, open today — it binds JS-2 onward and
                  │            binds neither JS-0, JS-1, nor JS-3a
                  │        ←── the copy lands here, behind the boundary rules
                  │
                  ├→ JS-3b static semantics, the lowering, the boundary decision
                  │         (needs JS-3a's registry to land its codes in;
                  │          rejoins at JS-6, which is the first manifest
                  │          that needs the general lowering)
                  │
                  └→ JS-4  value representation decided; the object model
                            │
                            └→ JS-5  executor, abrupt completion, measured budgets
                                 │
                                 └→ JS-6  standard library; the core manifest
                                      │      ←── satellite acquisition lands
                                      │
                                      └→ JS-7  suspension; terminal unwind
                                           │
                                           └→ JS-8  guest loads; three compositions
                                                │
                                                └→ JS-9  corpus, fuzz, soak, agents
                                                     │    (opened against JS-1; it closes here,
                                                     │     because the parser and the matcher it
                                                     │     must also fuzz arrive at JS-3b and JS-6)
                                                     │
                                                     └→ JS-10 baselines, packaging,
                                                          │    support table,
                                                          │    release gate
                                                          │
                                                          └→ (an advertised composition:
                                                              a release decision)

Manifest increments 2..n re-enter JS-5's vertical-slice loop: each mints one
further feature-manifest identity, extends the retained corpus, re-runs the
oracle against the ratchet, and closes no milestone.
```

What this ordering does and does not imply:

- **Read the three arrow kinds differently.** A `└→` edge is milestone precedence. A `←──`
  annotation marks an input or an external gate entering at that node and constrains nothing
  above it. A `┊··→` edge marks a milestone that **opens** at that node and **closes** lower down:
  work may start there, and only the clauses of its gate that need a later milestone wait.
- **Nothing here waits on a core milestone's *evidence*, and no gate here closes a core gate.**
  JS-0, JS-1 and JS-3a depend on the core being *implemented*, which is why the acceptance gate
  hangs off JS-2 in the diagram rather than off the root. JS-2 onward additionally depend on the
  core contract being *accepted*, which this component does not hold and must record as a blocker
  rather than route around.
- **Three forks are drawn, and each is the point.** The first is **JS-3a**, which hangs off JS-1
  and off nothing else: the harness needs a scoring target, not a copied line, and JS-1 produces
  one. Fusing it into the post-copy work would put this component's only external correctness
  signal behind *both* of its blockers when it needs to be behind neither, and a team that
  serialises them spends the whole acceptance wait with no oracle. The second is **JS-3b beside
  JS-4**: both are gated on JS-2 and on nothing else, and they are different skills with different
  owners — the verification-boundary owner holds JS-3b's semantics and lowering, the profile
  runtime owner holds JS-4's ABI and object model. Once JS-2 closes, both may open. The third is
  **JS-9, which opens against JS-1 and closes after JS-8**: the corpus it grows, the soak it runs,
  and the aggregate-budget behaviour it exercises are all reachable from the first product code,
  while the parser and the regular-expression matcher two of its four fuzz surfaces need do not
  exist until JS-3b and JS-6. A milestone whose gate closes late is not a milestone that starts
  late. Every other edge in the diagram is a real prerequisite, and one of them is argued rather
  than assumed: **JS-8 depends on JS-7's continuation capture and JS-7 depends on nothing JS-8
  delivers**, which is what keeps that edge acyclic rather than merely asserted, so JS-8 cannot be
  staffed beside JS-7.
- **Several decisions and one whole milestone need no copied code** and are opened against JS-1
  rather than waiting on the acceptance gate: **the entire conformance harness of
  [section 14](roadmap.md#14-the-conformance-oracle)**, which is by a wide margin the largest of
  them and is JS-3a; the diagnostic registry and position encoding; the value and frame ABI; the
  continuation design; the suspension-versus-job-queue routing; and JS-9's corpus, soak and
  aggregate-budget work. **This is not a hypothetical**: the registry, the position encoding and
  the eight-row ABI have all been taken this way, ahead of a gate none of them needed. A team that
  reaches the acceptance gate after JS-1 should have prepared work rather than a hard stop.
- **Two milestones carry the bulk of the cost**, and a twelve-milestone diagram should not be
  read as twelve equal steps: JS-4, which is the ABI plus the object model, and JS-6, which is the
  standard library — larger now that it is a rewrite than it was as a copy.
- **Manifest increments are not milestones.** Each mints one identity with a reviewed scope,
  extends the corpus, and re-runs the oracle. The admission criterion for the next increment is
  [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)'s allocation
  table, not a judgement made per commit.

---

## 25. The chapter, milestone, and gate map

Three files describe one programme, each organised for its own kind of reading — the argument by
subject, the milestones by sequence, the gate material by evidence class and again by failure mode.
**Nothing in that arrangement guarantees they cover the same ground**, and the two ways they can
fail are opposite and equally quiet: a chapter that argues for something no milestone delivers, and
a gate that demands something no chapter designed.

This map is the join. It is read in both directions, and **a blank cell is a finding**: a chapter
with no delivering milestone is a plan for work nobody scheduled, and an evidence area or a release
gate with no owning milestone is a gate nobody can close.

| Chapter | Delivered by | Evidence area, [section 21](roadmap.gates.md#21-test-and-evidence-matrix) | Release gate, [section 22](roadmap.gates.md#22-release-gates) | Stop condition, [section 23](roadmap.gates.md#23-risks-and-stop-conditions) |
|---|---|---|---|---|
| 1 Terminology and support claims | JS-0 fixes the identity; JS-10 issues the table | Identity and registration | 1 | untruthful published claim |
| 2 Engineering invariants | every milestone; each invariant is asserted by the milestone that could first violate it | all rows | 3, 4 | several |
| 3 What the core gives and refuses | JS-0 (the two vectors and the matrix); JS-1 (the descriptor half of the two-profile test); JS-8 (its `eval`-refusal half); JS-10 publishes the defaults as the neighbour-facing half and names the reconciliation unowned | Identity and registration; Composed-profile safety | 1, 2 | declared defaults reaching a neighbour |
| 4 The seed | JS-2 | Front end; Licence and attribution | 12 | the seed becomes a dependency |
| 5 Package boundaries and the graph | JS-0; **JS-1, JS-3a and JS-9 fill the composition roots with the evidence-producing code a test project may not hold** — the corpus writer, the harness, the mutator and the soak; JS-10 records the extraction-gate state, which no earlier milestone can, and no verdict | Dependency architecture | 2 | placement assumed rather than decided; the extraction gate unanswered; an advertised closure reaching the ingestion path |
| 6 Feature manifests | JS-0 allocates; JS-1 mints the first; each increment extends | Identity and registration; Conformance | 1, 9 | the manifest set drifts upward |
| 7 The format and the verifier | JS-1 builds it; JS-9 attacks it and **owes the exhaustion entries the registry's binding cannot reach** | Format and verifier safety | 3 | a check migrates into first execution; a ceiling breach recorded as an invalid artifact |
| 8 The value, frame, and call model | JS-4 (the ABI); JS-5 (the measured numbers and the charging) | Value model and storage; Executor and lifecycle; Measurement | 4, 10 | a late value-representation decision; unproportional charging |
| 9 The front end and the lowering | JS-2 (ingest); JS-3b (the stage, the boundary, the lowering) | Front end | 3 | a nesting case terminating the process |
| 10 Execution on the core lifecycle | JS-1 (the loop end to end); JS-5 (the executor) | Executor and lifecycle | 4 | — |
| 11 Guest-initiated loads | JS-8 | Guest loads and policy | 5 | — |
| 12 Suspension | JS-7 | Suspension | 4 | — |
| 13 Realms, agents, and the host boundary | JS-5 (binding and translation); JS-9 (agents under one parent) | Host boundary | 6 | a shared parent read as isolation; mutable optimization state reachable from a shared handle |
| 14 The conformance oracle | JS-3a, which also lands the ingested suite's attribution, **pins the language-specification edition**, and **stands the harness up as a never-advertised composition root** | Conformance; Licence and attribution; Dependency architecture | 9, 12 | the oracle reports a failure as a pass; an aggregate percentage; a manifest scored against an unpinned edition; the ingestion path in an advertised closure |
| 15 Compositions, Native AOT, the browser | JS-1, JS-3b and JS-8 build them; JS-10 advertises one | Native AOT | 7 | a publish cited for another kind; an implied `WebAssembly` namespace |
| 16 Persistence and the code cache | **no milestone delivers it**, by decision; JS-8 carries the exclusion clause and JS-10 measures the reopening trigger | Format and verifier safety | 3 | a second verifier, or a build-time shortcut past the one |
| 17 Measurement discipline | JS-10 stands up the lane; JS-4 produces the first figure it governs and fixes the repetition count | Measurement | 10 | — |
| 18 Amendments | JS-0 grades them; **every row is filed and held** rather than scheduled, and none is admissible until it names a merged or approved capability; **JS-10 publishes the register's state** | Identity and registration | 1 | a requirement with no core row; the programme stalling on a precondition this component does not control |
| — the standard library, which chapter 6's allocation admits rather than a chapter of its own | JS-6 | Standard library | 1, 9 | dynamic code hiding in the library |
| — the assurance floor and the review debt, which are repository policy rather than this plan's argument | JS-0 adopts the host component's mechanism and records what adoption costs; JS-10 gates the release on it | Assurance and review | 11 | unreviewed units accumulating; owner and reviewer the same person |
| — packaging and consumers | JS-10 | Packaging and consumers | 8 | — |
| — operational ownership: diagnostics, cancellation, rollback, version rejection, corpus and suite drift, **vulnerability response**, recertification | JS-10 names every owner | Assurance and review | 13 | a role held by nobody |
| — the surfaces reopened on 2026-10-03, which chapter 6 lists and no milestone delivered | [section 26](#26-the-road-to-a-full-featured-profile)'s phases F2 to F8, each closing at JS-10 through F9 | Conformance; Standard library | 1, 9 | the manifest set drifts upward |

**What the map shows that no single file does.** The first four are deliberate; the last two are
the gaps it was built to find:

- **[Section 16](roadmap.md#16-persistence-and-the-code-cache) is delivered by no milestone.** The
  core admits a persisted envelope by contract and implements none, so a profile-owned cache format
  would be a second serialization path with nothing to hold it to the first. The chapter exists to
  keep the design reachable at no cost, and its only obligations on this programme are one gate
  clause at JS-8 and one measurement at JS-10.
- **[Section 18](roadmap.md#18-amendments-this-profile-expects-to-ask-of-the-core) delivers
  nothing and now closes one gate.** Every row in it is filed and held; the amendment procedure is
  unexecutable while one person holds the minting role and both co-signing roles, so no milestone
  can schedule an answer. What a milestone *can* do is publish the state, and JS-10 does: a held
  amendment is a capability this profile does not provide, and gate 1 already refuses a support
  table that leaves an unimplemented capability unnamed. **The gate is over the publication and
  never over the answer** — a release that named every row and moved none of them passes it, which
  is the honest shape *(corrected: JSC-31)*.
- **Three areas of the evidence matrix have no chapter of their own** — the standard library, the
  assurance floor, and packaging — because each is admitted by a chapter rather than argued by one.
  They are listed above so that no evidence area is left without a milestone.
- **[Section 2](roadmap.md#2-engineering-invariants)'s invariants are the one thing that is
  deliberately everywhere.** An invariant is not a milestone's deliverable; it is a property every
  later milestone must not break, which is why each appears in the gate of the first milestone that
  could violate it rather than in a milestone of its own.
- **Two things the map surfaced have been folded back into the gates it checks against**, which is
  the map doing its job rather than a defect in it: the family's frozen public surface, which two
  milestones own as an exit clause and which no evidence area tested and no release gate blocked
  on; and the operations gate below.
- **A later reading found four more of the same shape, and they are folded back too**
  *(corrected: JSC-36)*. Three were clauses a release gate asks for that no milestone's exit gate
  closed: **the pinned specification edition**, which
  [section 24](roadmap.gates.md#24-specification-and-platform-references) and the ledger both put
  on JS-3a while JS-3a's gate named only the suite; and **the varying-surface list** and **the
  operational holders**, both of which gate 1 and gate 13 require and which JS-10's gate did not
  mention even while the bullet below said JS-10 names them. The fourth was a gate that could not
  be met as written: **JS-5's proportionality clause**, which demanded a fixture for every family
  [section 8](roadmap.md#8-the-value-frame-and-call-model) names, including ones whose operations
  arrive at JS-6. **The pattern in all four is the same and worth naming**: a clause stated in the
  argument or the release gates, agreed by the map, and never written into the exit gate that would
  have to fail without it — which is the one failure mode a map read in only one direction cannot
  catch.
- **A fifth was found after that sweep, and its route is why the pattern is now stated generally
  rather than as four instances** *(corrected: JSC-37)*.
  [JSC-35](roadmap.corrections.md#jsc-35) added the four exhaustion-dimension corpus entries to
  [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier) and to
  [section 21](roadmap.gates.md#21-test-and-evidence-matrix), and the chapter-7 row above named JS-9
  as owing them — and JS-9's exit gate said nothing. **A correction is an edit to the plan and
  inherits the plan's own failure modes**, so a sweep that reads the release gates against the
  milestones cannot see a requirement that has just been added to neither. The rule this map now
  holds is the general one: **a requirement is not in the programme until an exit gate would fail
  without it**, whatever introduced it — a chapter, a release gate, or a correction.
- **A third reading found three more, and two of them are the rule above found from the other
  side** *(corrected: JSC-38, JSC-39, JSC-40)*. [Section 7](roadmap.md#7-the-bytecode-format-and-the-verifier)
  and [section 21](roadmap.gates.md#21-test-and-evidence-matrix) ask for *coverage-guided* fuzzing
  and JS-9's gate asked only that a session retain its seed, its budget and its counterexamples —
  so a seeded mutator with no coverage feedback satisfied the gate while the ledger called it
  coverage-guided, and the gate now carries the guidance. Section 7 named four exhaustion
  dimensions where the verifier as built names seven — the allocator's, the work charge's and the
  poll's answers carry no diagnostic code either — so a binding that covered four of seven arms
  read as complete. And [section 14](roadmap.md#14-the-conformance-oracle)'s harness was held to a
  scan over *no published closure* while [section 5](roadmap.md#5-package-boundaries-and-the-dependency-graph)
  had already put everything that drives the profile in a composition root, which the harness must
  be too; the scan is over advertised closures now, which is the reading the other intended
  profile reached from the same rule. **The lesson the first two add**: a gate can be satisfied by
  the thing the argument excludes when the argument's adjective — *coverage-guided*, *the four* —
  never reached the gate's clause.
- **One release gate is owned by no chapter, and the map is how that was found.** Gate 13 asks that
  the holders of diagnostics, cancellation, rollback, format-version rejection, corpus and
  suite-revision drift, **vulnerability response**, and recertification each be named. No chapter
  argues for it, because it is an operational obligation rather than a design one — and this
  component ships a parser and an interpreter over untrusted input, so a release with no named
  holder for a report about either is not a release. JS-10 names them or the gate refuses.

---

## 26. The road to a full-featured profile

*Added 2026-10-03 ([JSC-251](roadmap.corrections.md#jsc-251)). Sections 19, 20 and 25 stay the
authority for each milestone's gate and for the dependency order the milestones were planned in.
This section is the reading order for the work that remains. It summarises what is finished,
reopens every surface the plan used to decline, and orders what is left into phases that end in a
profile with no surface declined.*

### 26.1 What "full-featured" means here

**A full-featured profile is one whose realm declines nothing the language defines, and whose
release gate is met.** Four things together:

- **The whole pinned edition**, ES2026 as archived under [`specification/`](specification/README.md),
  with Annex B in the script goal. That includes the RegExp `v` flag, `SharedArrayBuffer`,
  `Atomics` and the agent model they need, and nested realms.
- **ECMA-402**, as `Intl` under its own manifest identity.
- **The two surfaces ahead of the edition this plan now schedules: Temporal and ShadowRealm.**
  Neither is in the pinned edition, and roadmap
  [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)'s rule holds for
  both: nothing ahead of the edition is admitted except what a decision record names. Each phase
  that delivers one opens with that record.
- **The two host-facing behaviours a program reads, which the edition leaves to the host:**
  `Function.prototype.toString` returning source text, and `Error.prototype.stack`.

**A full-featured profile is not an accepted one.** Acceptance is the [ledger](roadmap.status.md)'s
and needs evidence a person has read. Section 26.5 runs that track beside the phases, and phase F9
is where the two meet.

### 26.2 What is finished

**Nothing below is accepted.** "Implemented" means the work exists in this checkout, with fixtures
and suite runs recorded in the [corrections](roadmap.corrections.md) and in the retained records
under `docs/evidence/`. No owner or reviewer has read any of it. The milestone rows themselves are
the ledger's and are not restated here: [section 2 of the ledger](roadmap.status.md#2-current-milestone-status)
holds them.

**By area, what the profile has today:**

| Area | Implemented | Where it is argued |
|---|---|---|
| Front end | Two source front ends sharing one tokenizer: the slice and the wide surface, with the static semantics the wide one owes | [section 9](roadmap.md#9-the-semantic-front-end-and-lowering) |
| Format and verifier | Format versions 1 and 2, each verifier refusing at verification rather than at first execution | [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier) |
| Values and objects | The value model, the object model with its integrity clauses, BigInt, the binary surface | [section 8](roadmap.md#8-the-value-frame-and-call-model) |
| Execution | The interpreter, measured budgets, the value form, and two native forms (x86-64, arm64 emitting) | [section 10](roadmap.md#10-execution-mapping-javascript-onto-the-core-lifecycle), [backends](roadmap.backends.md) |
| Library | The edition's built-ins except the reopened surfaces of 26.3, Annex B, Unicode default case conversion, and the from-scratch RegExp matcher with pattern modifiers | [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted) |
| Suspension | Generators, async functions, async generators, the job queue, top-level await | [section 12](roadmap.md#12-suspension-generators-async-functions-and-top-level-await) |
| Guest loads | `eval`, the function constructors, dynamic `import()`, the module goal, each behind its identity | [section 11](roadmap.md#11-guest-initiated-loads-eval-the-function-constructor-dynamic-import-modules) |
| Host | The in-realm host surface (JSD-0024), the end-user host, the conformance host | [section 13](roadmap.md#13-realms-agents-and-the-host-boundary), [hosting](roadmap.hosting.md) |
| Oracle | The pinned edition, the pinned and archived test262, a floor over the whole suite, the differential probes | [section 14](roadmap.md#14-the-conformance-oracle) |

**The proposal documents' stages, as of this date:**

| Document | Implemented | Partly implemented | Not started |
|---|---|---|---|
| [Parity](roadmap.parity.md) | JSP-1 to JSP-10, every stage | — | — |
| [Workloads](roadmap.workloads.md) | JSW-1, JSW-3, JSW-5 (under `wide`), JSW-6, JSW-7 | JSW-2 (its exclusion now reopened), JSW-4 (`broiler.javascript.regexp` not minted), JSW-8 (the module identity's remainder), JSW-9 (Native AOT depth, and the five Octane workloads `MaximumTreeDepth` refuses), JSW-10 (no retained whole run on a claimed RID) | — |
| [Backends](roadmap.backends.md) | JSB-4, JSB-5, JSB-6 | JSB-2, JSB-3, JSB-7 to JSB-11 | JSB-1, and JSB-12 beyond its predeclared bounds |
| [Hosting](roadmap.hosting.md) | JSH-1, and JSH-2 and JSH-3 for the members their gates name | JSH-4 (the host-versus-guest provider route), JSH-7 (the clone carrier, without a second realm) | JSH-5, JSH-6, JSH-8 |

**What that leaves of the parity roadmap is its record, not its work.** Its section 4 is the gap
as it was measured on 2026-09-20; every stage that closed part of it says so in place. The phases
below carry what the other three documents still owe.

### 26.3 The surfaces reopened on 2026-10-03

**Every surface the plan declined by name is reopened.** It was declined for a reason that still
holds for the order of work but not for whether the work happens: each one needs a mechanism this
profile did not have, and none of them is optional for a full-featured profile. What a program
meets today is unchanged until the phase that delivers the surface lands, and roadmap
[section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted) states it.

| Surface | Governing record | What reopening changes in the record | Phase |
|---|---|---|---|
| The RegExp `v` flag | [JSD-0031](decisions/0031-unicode-data-source-and-build-boundary.md) | "Keep the `v` flag refused until a matcher slice is scheduled": the slice is scheduled *(performed 2026-10-04: JSD-0031 section 14)* | F2 |
| `Function.prototype.toString` source text | none yet; a record opens F3 *(proposed: [JSD-0037](decisions/0037-the-source-text-section.md), 2026-10-04)* | The artifact carries the source text a function was defined from | F3 |
| `Error.prototype.stack` | none yet; a record opens F3 *(proposed: [JSD-0038](decisions/0038-the-error-stack.md), 2026-10-04)* | The shape is chosen by that record, from the comparison engines' common form | F3 |
| `FinalizationRegistry` cleanup | [JSD-0029](decisions/0029-finalization-registry-cleanup-model.md) | D03-a is scheduled, and is taken on by the CLI composition by default *(performed 2026-10-04: JSD-0029 section 11)* | F4 |
| Nested realms, `$262.createRealm` | [JSD-0030](decisions/0030-shadowrealm-support-boundary.md) | SR-1, SR-2 and SR-7 are scheduled *(performed 2026-10-04: proposed [JSD-0039](decisions/0039-a-second-realm-on-one-engine.md))* | F5 |
| ShadowRealm | [JSD-0030](decisions/0030-shadowrealm-support-boundary.md) | The deferral is not taken; SR-3 to SR-5 follow F5's realm work under `broiler.javascript.shadowrealm` *(performed 2026-10-04: proposed [JSD-0040](decisions/0040-admitting-shadowrealm.md))* | F5 |
| Agents, `$262.agent` | [roadmap section 13](roadmap.md#13-realms-agents-and-the-host-boundary), [JSD-0028](decisions/0028-shared-memory-and-atomics.md) | A second agent is built, which is the first of JSD-0028's reopening conditions | F6 |
| `SharedArrayBuffer` and `Atomics` | [JSD-0028](decisions/0028-shared-memory-and-atomics.md) | The exclusion is not taken; slices S1 to S5 follow the agent work under their own identity *(performed 2026-10-04 for one agent, ahead of the agents, as far as one agent needs each slice: proposed [JSD-0041](decisions/0041-shared-memory-in-one-agent.md))* | F6 |
| `Intl` | [JSD-0027](decisions/0027-intl-scope-and-data-strategy.md), [JSD-0002](decisions/0002-feature-manifest-allocation.md) | The deferral is not taken; I0 to I4 are scheduled without waiting for a named consumer | F7 |
| Temporal | [JSD-0002](decisions/0002-feature-manifest-allocation.md) only; a record opens F8 | Admitted at a pinned revision of the proposal, with a time-zone data boundary | F8 |

**Each governing record gained a dated note saying so, and each is still unsigned.** Reopening is a
direction given on 2026-10-03; it takes no record, signs nothing, and moves no ledger row. The
`absent-globals` block in the ledger keeps all four of its names until the change that publishes
each one removes it, because the block states what the realm lacks, not what the plan intends.
*(Amended 2026-10-04: two remain; `SharedArrayBuffer` and `Atomics` left with phase F6's first slice,
[JSC-266](roadmap.corrections.md#jsc-266).)*

### 26.4 The phases

```text
  F1  conformance closure on the admitted surface
   │
   ├→ F2  text: the RegExp v flag
   │
   ├→ F3  source text and stacks ──────────────┐
   │                                            │
   ├→ F4  FinalizationRegistry cleanup          │
   │                                            │
   └→ F5  realms: createRealm, then ShadowRealm ←┘ (a child realm's functions render their source)
        │
        └→ F6  agents, then SharedArrayBuffer and Atomics
             │
  F7  Intl ──┼──────────────────────────────────── (needs nothing above F1; may run beside F2-F6)
   │         │
   └→ F8  Temporal
             │
             └→ F9  the release: JS-10, on every surface above
```

**Read the diagram the way section 20's is read.** A `└→` edge is a prerequisite. F2, F3, F4 and F7
need only F1 and may be staffed at once. F5 needs F3, because a realm built after source text is
kept does not have to be revisited for it. F6 needs F5, because an agent is a runtime with its own
realm and the realm work makes "its own" mean something. F8 needs F7, because Temporal's
`toLocaleString` and its calendar names go through `Intl`. F9 needs everything.

**Every phase has the same five parts**: what it delivers, the stages and slices it is made of,
what it needs, its manifest identity, and its exit gate. An exit gate here is observable in this
checkout: a test262 selection whose failures are zero or each named, a fixture in
[`src/tests/cli/expected.txt`](../../tests/cli/expected.txt), and the corpus, probe and build
checks a correction entry already records. **It is not acceptance**, which remains section 19's
gate and the ledger's row.

#### F1 — Conformance closure on the admitted surface

- **Delivers:** no test262 failure on the admitted surface that is not named, and the workload
  remainders.
- **Made of:**
  - the known defects: `new` through a revoked proxy, `delete super[key]`'s key conversion, and
    `yield` and `await` read as names before a `/`;
  - the runner's one hang, `staging/sm/regress/regress-1507322-deep-weakmap.js`, which passes its
    wall-clock allowance without ending *(ended 2026-10-04: the runner skips the suite's
    `host-gc-required` tests, [JSC-257](roadmap.corrections.md#jsc-257); the collector's stall over
    such a chain is still the engine's)* *(the stall ended 2026-10-04: a WeakMap's values live on
    their keys, [JSC-260](roadmap.corrections.md#jsc-260))*;
  - JSW-4's identity: mint `broiler.javascript.regexp`, which needs a person's decision
    (JSC-167);
  - JSW-8's remainder, JSW-9's depth refusals of five Octane workloads, and JSW-10's retained whole
    runs.
- **Needs:** nothing.
- **Identity:** the existing ones.
- **Exit gate:** a whole run of the pinned suite in which every failure belongs to a phase below or
  is named in a correction, and a retained whole run per manifest.

#### F2 — Text: the RegExp `v` flag

- **Delivers:** `v`-mode patterns, `unicodeSets`, properties of strings, set operations and
  `MaybeSimpleCaseFolding`.
- **Made of:** JSD-0031's matcher slice for string properties: archive `emoji-sequences.txt` and
  `emoji-zwj-sequences.txt` from the pinned Unicode 17.0.0 release under rule N22, generate the
  string-property tables, and extend the matcher with class strings and set operations.
- **Needs:** F1.
- **Identity:** `broiler.javascript.regexp` (or `wide` until F1 mints it).
- **Exit gate:**
  - the `unicodeSets`, `CharacterClassEscapes` and `property-escapes/generated/strings` subtrees
    pass;
  - the compile-time refusal of `v` and its diagnostic are removed;
  - the row asserting `unicodeSets` answers `false` is replaced.
- *Observed 2026-10-04, unreviewed: each clause of the gate holds - the three subtrees pass all of
  their 308 variants, the refusal is gone, and the row now asserts `true` for a `v` pattern
  ([JSC-262](roadmap.corrections.md#jsc-262)). The identity is still `wide`: F1 did not mint
  `broiler.javascript.regexp`, which needs a person's decision.*

#### F3 — Source text and stacks

- **Delivers:**
  - `Function.prototype.toString` returning the source text a function was defined from;
  - an `Error.prototype.stack` of the shape a decision record chooses.
- **Made of:**
  - a decision record for source text: an artifact section holding source spans, which manifests
    carry it, and what a size-sensitive composition may drop;
  - a decision record for stacks, choosing the accessor's shape from the comparison engines'
    common form and the position table the artifact already carries;
  - the two implementations.
- **Needs:** F1.
- **Identity:** a format-version increment if the record puts source in the artifact. *(Corrected
  2026-10-04: the record puts it in an optional section under format version 2, as sections 13 to 15
  were, and does not increment the version; see [JSC-259](roadmap.corrections.md#jsc-259).)*
- **Exit gate:**
  - `test/built-ins/Function/prototype/toString` passes;
  - the fixtures that pin the native rendering and `an-error-has-no-stack.js` are replaced;
  - section 6's two rows leave the table.
- *Observed 2026-10-04, unreviewed: each clause of the gate holds -
  [JSC-259](roadmap.corrections.md#jsc-259) and [JSC-261](roadmap.corrections.md#jsc-261). Both
  records are proposed and unsigned, so the phase is delivered in the tree and not accepted.*

#### F4 — FinalizationRegistry cleanup

- **Delivers:** cleanup callbacks that arrive.
- **Made of:** JSD-0029's D03-a, the host-drained sweep, with rule N25 kept: no guest code ever runs
  from a CLR finalizer.
- **Needs:** F1.
- **Identity:** a composition switch, on by default in the CLI and conformance compositions.
- **Exit gate:**
  - `test/built-ins/FinalizationRegistry` passes with the cleanup cases;
  - a fixture shows a callback arriving at a drain point;
  - N25 still passes.
- *Observed 2026-10-04, unreviewed: the fixture and N25 hold; `built-ins/FinalizationRegistry` passes
  every variant but its two `cross-realm` ones, which F5 owns. The suite's cases that need a callback
  to arrive are its `host-gc-required` ones, which the runner skips because it provides no `$262.gc`,
  so "with the cleanup cases" is met by the profile's own checks rather than by the suite
  ([JSC-263](roadmap.corrections.md#jsc-263)).*

#### F5 — Realms: `createRealm`, then ShadowRealm

- **Delivers:** a second realm on one engine, cross-realm identity, then ShadowRealm.
- **Made of:**
  - JSD-0030's SR-1 (agent-scoped Symbols), SR-2 (a running realm and every function's
    `[[Realm]]`) and SR-7 (`$262.createRealm`), with a JSD-0018 record admitting the suite's
    `cross-realm` feature;
  - JSH-7's second realm;
  - then SR-3 to SR-5 behind `broiler.javascript.shadowrealm`, with a record admitting the proposal
    at a pinned revision.
- **Needs:** F3.
- **Identity:** `broiler.javascript.shadowrealm`, admitted only with `broiler.javascript.dynamic`,
  as JSD-0030 recommends.
- **Exit gate:**
  - the suite's `cross-realm` cases pass;
  - `createRealm` stops refusing in [section 13](roadmap.md#13-realms-agents-and-the-host-boundary)'s
    table;
  - `test/built-ins/ShadowRealm` passes.
- *Observed 2026-10-04, unreviewed: the first two clauses hold for every `cross-realm` case outside
  phases F6 and F7 - SR-1, SR-2 and SR-7 are built under proposed
  [JSD-0039](decisions/0039-a-second-realm-on-one-engine.md), which is also the JSD-0018 record, and
  `createRealm` left section 13's table ([JSC-264](roadmap.corrections.md#jsc-264)). JSH-7's second
  realm exists on one engine, and its refusal of another view's ref is exercised; JSH-7's realm on
  another thread is phase F6's. ShadowRealm, the third clause, is not started.*
- *Observed again 2026-10-04, unreviewed: the third clause holds. `ShadowRealm` is built behind
  `broiler.javascript.shadowrealm`, admitted only with the dynamic surface, under proposed
  [JSD-0040](decisions/0040-admitting-shadowrealm.md), which admits the proposal at `9ff2a01f`; all
  124 scored variants of `test/built-ins/ShadowRealm` pass, `importValue`'s included, and JSD-0030
  section 6's policy cases are host-surface checks ([JSC-265](roadmap.corrections.md#jsc-265)). Every
  clause of the gate holds in the tree; the records are proposed and unsigned, so the phase is
  delivered and not accepted.*

#### F6 — Agents, then `SharedArrayBuffer` and `Atomics`

- **Delivers:** worker-style agents under one shared budget, `$262.agent`, then shared memory.
- **Made of:**
  - JSH-5 (one guest thread per instance) and JS-9's agents under one parent;
  - `$262.agent`'s five members;
  - a successor to JSD-0028 answering its sections 2.1 to 2.6;
  - slices S1 to S5.
- **Needs:** F5.
- **Identity:** a new `broiler.javascript.shared`, never folded into `broiler.javascript.binary`,
  which keeps JSD-0028's reason for the split.
- **Exit gate:**
  - `test/built-ins/Atomics`, `SharedArrayBuffer`, and the shared-buffer cases of `DataView` and
    the typed arrays pass;
  - the conformance adapter stops declining `CanBlockIsFalse` and `CanBlockIsTrue`;
  - the two names leave the ledger's `absent-globals` block in the same change that publishes them.
- *Observed 2026-10-04, unreviewed: the first slice is in the tree under proposed
  [JSD-0041](decisions/0041-shared-memory-in-one-agent.md), which mints `broiler.javascript.shared`.
  `SharedArrayBuffer` passes, the shared-buffer cases of `DataView` and the typed arrays pass, and the
  two names left the `absent-globals` block in the change that publishes them; `Atomics` passes every
  variant that does not start a second agent, and the conformance runner's main agent may block, so
  it runs its `CanBlockIsTrue` cases. Agents, `$262.agent` and the `CanBlockIsFalse` cases are the
  second slice ([JSC-266](roadmap.corrections.md#jsc-266)).*
- *Observed 2026-10-04, unreviewed: the second slice is in the tree under proposed
  [JSD-0042](decisions/0042-a-second-agent.md). A second agent is a runtime its host starts, holding a
  fixed-length block another agent made; the conformance runner's `$262.agent` starts real agents
  under one aggregate budget and scores the `CanBlockIsFalse` files as well as the `CanBlockIsTrue`
  ones. `test/built-ins/Atomics` passes all 752 of its scored variants; its six skipped files claim
  `Atomics.pause`, which the pinned suite lists as a proposal. The exit gate's three clauses are met
  as written, with one reading stated: the clause on declining is met by the `--test262` runner, and
  the slice-manifest ingestion translator still declines both flags because that manifest has no
  shared memory. JSH-5, a growable block across agents, retention against the aggregate, JSD-0028's
  S2 rule and audit, and S5 remain ([JSC-267](roadmap.corrections.md#jsc-267)).*

#### F7 — `Intl`

- **Delivers:** ECMA-402.
- **Made of:** JSD-0027's I0 (the pinned CLDR input, its generator and the data assembly), I1
  (`Collator` and a locale-aware `localeCompare`), I2 (`NumberFormat`), I3 (`DateTimeFormat`, UTC and
  fixed offsets), then I4 and later (the remaining constructors and time-zone data).
- **Needs:** F1, and nothing else above.
- **Identity:** `broiler.javascript.intl`.
- **Exit gate:**
  - `test/intl402` is admitted by a JSD-0018 record and passes per slice;
  - `Intl` leaves the `absent-globals` block in the change that publishes it, with N24's
    assertion about it amended in the same change.

*Progress, 2026-10-04: I0 and I1 are built under proposed
[JSD-0043](decisions/0043-intl-data-boundary-and-collation.md). The CLDR 48.2.0 tables are generated
into `Broiler.VM.Profile.JavaScript.Intl` (rules N27 and N28), which only a composition admitting
`broiler.javascript.intl` references. `Intl` has `getCanonicalLocales` and `Collator`, and
`localeCompare` routes through it. Both CollationTest files are in order, and the retained German and
English orderings agree with ICU 77.1. The second exit-gate clause is met: `Intl` left the block, and
N24's witness moved to `Temporal`. The first clause's admission is JSD-0018's amendment of 2026-10-04.
Its "passes per slice" holds for I1: `Collator` passes 124 of 130 `intl402` variants, and the three
failing files need `NumberFormat`, Thai's tailoring and the `eor` collation. I2 (`NumberFormat`) is
next ([JSC-269](roadmap.corrections.md#jsc-269)).*
*Progress, 2026-10-04: I2 is built under proposed
[JSD-0044](decisions/0044-intl-numberformat.md): `Intl.NumberFormat` whole, units, compact and
scientific notation included, and `toLocaleString` of Number, BigInt, Array and TypedArray through
it. The retained numbers agree with ICU 77.1 but for 24 named divergences. `NumberFormat` passes 280
of 324 `intl402` variants, and every failing one expects a locale the data lacks or `DateTimeFormat`.
I3 (`DateTimeFormat`) is next ([JSC-270](roadmap.corrections.md#jsc-270)).*
*Progress, 2026-10-04: I3 is built under proposed
[JSD-0045](decisions/0045-intl-datetimeformat.md): `Intl.DateTimeFormat` for the Gregorian calendar
with every component, both styles and ranges, the `Date.prototype.toLocale*` methods through it, and
`Intl.supportedValuesOf`. The time zones are UTC, offset strings and IANA's `Etc/GMT` offsets. The
retained dates agree with ICU 77.1 but for 352 named divergences. `DateTimeFormat` passes 326 of 350
`intl402` variants, and the failing ones need another calendar, `ja` or the `arab` decimal
separator. I4 (`PluralRules`,
`Locale` and the rest) is next ([JSC-271](roadmap.corrections.md#jsc-271)).*
*Progress, 2026-10-04: I4 is taken constructor by constructor, and `Intl.Locale` is built first under
proposed [JSD-0046](decisions/0046-intl-locale.md), with the draft's information methods and the
locale core's likely subtags corrected to UTS #35. The retained Locale dataset agrees with ICU 77.1
but for 150 named divergences. `Locale` passes all 218 scored `intl402` variants. `PluralRules` is
next ([JSC-272](roadmap.corrections.md#jsc-272)).*
*Progress, 2026-10-04: `Intl.PluralRules` is built under proposed
[JSD-0047](decisions/0047-intl-pluralrules.md), cardinal and ordinal over the number format's
rounding. The retained plural categories agree with ICU 77.1 but for 123 named divergences.
`PluralRules` passes 78 of 82 scored `intl402` variants, and the failing ones need other locales.
`ListFormat` is next ([JSC-273](roadmap.corrections.md#jsc-273)).*
*Progress, 2026-10-04: `Intl.ListFormat` is built under proposed
[JSD-0048](decisions/0048-intl-listformat.md), over CLDR's list patterns. The retained lists agree
with ICU 77.1 on every string. `ListFormat` passes 154 of 162 `intl402` variants, and the failing
ones need Spanish. `RelativeTimeFormat` is next ([JSC-274](roadmap.corrections.md#jsc-274)).*
*Progress, 2026-10-04: `Intl.RelativeTimeFormat` is built under proposed
[JSD-0049](decisions/0049-intl-relativetimeformat.md), over the relative time data slice I3 archived.
The retained relative times agree with ICU 77.1 but for 96 named divergences. `RelativeTimeFormat`
passes 148 of 160 `intl402` variants, and the failing ones need Polish. The generated data is 40,545
bytes under the provisional bound, less than `DisplayNames` needs, so `Segmenter` is next and
`DisplayNames` waits on the owner's size budget ([JSC-275](roadmap.corrections.md#jsc-275)).*
*Progress, 2026-10-04: `Intl.Segmenter` is built under proposed
[JSD-0050](decisions/0050-intl-segmenter.md), by UAX #29's default rules over UCD break data generated
into the Intl data assembly. It passes every line of the three pinned conformance files, and 154 of
158 `intl402` variants; the failing ones need Serbian. `DurationFormat` is next, and `DisplayNames`
still waits on the size budget ([JSC-276](roadmap.corrections.md#jsc-276)).*
*Progress, 2026-10-05: `Intl.DurationFormat` is built under proposed
[JSD-0051](decisions/0051-intl-durationformat.md), over the profile's own number and list formats,
and passes 208 of 210 scored `intl402` variants; the failing ones need Serbian. Every constructor of
slice I4 is built but `DisplayNames`, which waits on the owner's size budget (JSD-0027 decision (c))
([JSC-277](roadmap.corrections.md#jsc-277)).*
*Progress, 2026-10-05: the repository owner set the Intl data budget at 768 KiB (JSD-0027 decision
(c)), and `Intl.DisplayNames` is built under proposed
[JSD-0052](decisions/0052-intl-displaynames.md), over CLDR's locale display names. The retained
display names agree with ICU 77.1 but for 64 named divergences. `DisplayNames` passes all 114 scored
`intl402` variants, and `Intl` all 130; the whole pinned suite passes 87,022 of 95,058 variants.
**Every constructor of slice I4 is built, and the exit gate is met**: `test/intl402` is admitted and scored per slice, each failing variant needing a locale,
calendar or numbering system the data does not carry. F8 (Temporal) is next, starting with tzdb, as
the owner chose ([JSC-278](roadmap.corrections.md#jsc-278)).*

#### F8 — Temporal

- **Delivers:** the temporal surface.
- **Made of:** a record admitting the proposal at a pinned revision, a time-zone data boundary
  modelled on JSD-0031's (tzdb archived, generated, pinned), and the implementation.
- **Needs:** F7.
- **Identity:** `broiler.javascript.temporal`.
- **Exit gate:**
  - `test/built-ins/Temporal` and `test/intl402/Temporal` pass;
  - `Temporal` leaves the `absent-globals` block in the change that publishes it.

*Progress, 2026-10-05: the repository owner chose to start F8 with tzdb (JSD-0027 decision (d)). The
time-zone data boundary is built under proposed
[JSD-0053](decisions/0053-time-zone-data-and-temporal-admission.md): tzdb 2026e archived and pinned
(rule N29), compiled as `zic` compiles it into tables in the Intl data assembly (rule N30), and read by
`Intl.DateTimeFormat`, `supportedValuesOf` and `getTimeZones`; the whole pinned suite scores as before.
The same record pins the proposal at
`tc39/proposal-temporal` `e8cc03fc`, the Stage 4 draft of 2026-07-27, and plans `Temporal` in four
slices. T1 (the ISO arithmetic, `Instant`, `Duration` and `Now`) is next
([JSC-279](roadmap.corrections.md#jsc-279)).*

*Progress, 2026-10-05, slice T1: `Temporal` is published under proposed
[JSD-0054](decisions/0054-temporal-in-the-iso-and-gregorian-calendars.md), every type in the ISO 8601
and Gregorian calendars, and the identity `broiler.javascript.temporal` is minted, admitted only with
Intl and BigInt. The plan is now three slices: T2 is Intl over Temporal objects and T3 the other
calendars. `Temporal` left the `absent-globals` block. `test/built-ins/Temporal` passes all 9,156
scored variants; `test/intl402/Temporal` passes 464 of 930, its failures T2's and T3's, so the exit
gate is not yet met. Over the whole pinned suite 96,694 of 100,180 variants pass, and every variant
scored before keeps its verdict ([JSC-280](roadmap.corrections.md#jsc-280)).*

*Progress, 2026-10-05, slice T2: `Intl.DateTimeFormat` formats Temporal objects, every type's
`toLocaleString` is ECMA-402's, and the `iso8601` calendar is formatted from CLDR's root, archived
under N27, under proposed [JSD-0055](decisions/0055-intl-over-temporal-objects.md).
`test/intl402/Temporal` passes 598 of 930; nearly all the rest is T3's calendars, which is next.
Over the whole pinned suite 96,938 of 100,180 variants pass, none moving back
([JSC-281](roadmap.corrections.md#jsc-281)).*

*Progress, 2026-10-05, slice T3: `Temporal` reckons in every calendar of the Intl era and month code
proposal's Table 1 under proposed [JSD-0056](decisions/0056-temporal-in-the-cldr-calendars.md), the
Chinese, Korean and Umm al-Qura years and the Persian corrections taken from ICU4X's crates, archived
under a new rule N31 and generated under a new rule N32, and the `Intl.Era-monthcode` flag is scored.
`test/built-ins/Temporal` passes all 9,176 scored variants and `test/intl402/Temporal` 3,962 of 3,982;
the 20 left format in a calendar `Intl.DateTimeFormat` does not yet write, or name a zone's long name.
The plan gains a slice T4, `Intl.DateTimeFormat` in these calendars, which is next and closes the exit
gate if the owner's budget holds CLDR's names for them ([JSC-282](roadmap.corrections.md#jsc-282)).*

#### F9 — The release

- **Delivers:** JS-10 over every surface above.
- **Made of:** JS-10's own gate: the measurement lane, baselines, packaging, the support table, the
  composition register and claimed RIDs. Beside it, the backend stages left open in 26.2.
- **Needs:** F1 to F8, and the acceptance track below.
- **Identity:** every one above.
- **Exit gate:** section 19's JS-10 gate.

### 26.5 What runs beside the phases

**The acceptance track.** None of the work above advances a ledger row on its own. Beside every
phase:

- a named reviewer reads what the phase changed, which is the review debt JS-10's gate refuses;
- the ledger's open blocker for JS-2 is settled: either the core contract is accepted, or a record
  says the seed copy will not be taken and JS-2's gate is re-scoped;
- each milestone's remaining clauses, which [section 19](#19-milestones) names, are closed by
  retained records rather than by working-tree runs.

**The performance track.** The backend stages left partly done in 26.2 are not on the path to a
full-featured profile, and are not dropped either. They run beside F2 to F8 and meet the release at
F9.

### 26.6 What this section does not change

- **The ledger stays the authority for status**, and section 19 stays the authority for each
  milestone's gate. A phase closing is a fact about this checkout, not a change of state.
- **Section 20's diagram is not rewritten.** It is the dependency order the milestones were
  planned in, and the gates still cite it.
- **What a program meets today does not change until a phase lands.** Roadmap section 6 states it
  for every surface above.
