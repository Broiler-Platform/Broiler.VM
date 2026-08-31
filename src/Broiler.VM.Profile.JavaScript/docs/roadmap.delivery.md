# Broiler.VM.Profile.JavaScript roadmap — delivery

**This file is part of the [Broiler.VM.Profile.JavaScript roadmap](roadmap.md)**, which
[names every file](roadmap.md#how-this-roadmap-is-split). It carries sections 19–20 and 25:
the milestones, the order they are delivered in, and the map that ties every chapter of the plan
to the milestone that delivers it and the gate that closes it. **Section numbers are global and do
not change when a section moves**, so a reference written to any section below still resolves here.

The [evidence ledger](roadmap.status.md) is the authority for what has been accepted, and
[the corrections and rejections](roadmap.corrections.md) hold what an earlier reading of any
milestone below said before implementation replaced it.

---

## 19. Milestones

**This section is the authority for what is *planned* and for nothing else.** What has moved, what
each retained bundle demonstrates, and which exit-gate clauses are open are the
[ledger](roadmap.status.md)'s, and no milestone below restates them: each names its ledger row and
stops there.

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
  its retained evidence, and every open clause of the gate below.
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
  produced by it; the snapshot identity schema is recorded and **a second checkout re-derives the
  same identity from the record**; and the licence and notice carry the Apache-2.0 text, the
  upstream derivation, and the marking of modified files.
- **Seed:** Nothing is copied. Every mechanism here is this component's own code.

### JS-1 — Close the whole contract loop on the smallest JavaScript that is still JavaScript

- **Owner:** profile contract owner, with release and AOT review of the composition root.
- **Ledger:** JS-1's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and every open clause of the gate below.
- **Next action:** Mint `broiler.javascript.slice` and define format version 1 for it. Write the
  verifier over the core's bounded reader and allocator, supplying the bounds projection and the
  allocation-meter adapter. Implement all seven core-facing types. Fill every descriptor row in
  one full-arity construction, with the language-shaped rows of
  [section 8](roadmap.md#8-the-value-frame-and-call-model) marked **provisional** and each naming
  the milestone that will settle it. Write the lowering for this slice by hand in the lowering
  sibling. Stand up **two composition roots differing by exactly one reference, the lowering** —
  one that names the profile and not the compiler and therefore cannot turn source into an artifact
  however it is invoked, and one that names both and writes the retained corpus — each with a
  closure self-report mode, and **neither claiming a composition label**, because
  `narrow-runtime-compiler` belongs to a composition lowering a named restricted *source* surface
  and there is none until JS-3b. Decide
  and record the entry-point answer from
  [section 10](roadmap.md#10-execution-mapping-javascript-onto-the-core-lifecycle).
- **Dependencies:** JS-0. Deliberately **not** the copy, not a parser, and not core acceptance:
  the point of this milestone is to find contract defects against about two thousand readable
  lines rather than against a copied engine.
- **Objective exit gate:** The execution-only composition **publishes and runs** on every RID of
  this milestone's claimed set, which its bundle names and which is not empty — under JIT, trimmed
  self-contained, and
  Native AOT with trim and AOT warnings treated as errors, executing a verified artifact to its
  expected answer in every mode, each closure report containing exactly the declared assemblies
  and no test, reflection, dynamic-code, or IL-emission assembly. **Each of the five verifier
  outcomes** is produced by a named retained corpus case, the invalid-artifact case carrying a
  diagnostic code and a source position and the exhaustion case naming one dimension and one
  scope. **Each of the five execution-step kinds** is produced by a named test, including a
  contract violation from a deliberately non-conforming variant; if `Suspended` is unreachable
  from this surface the milestone declares it produced at JS-7 rather than minting an
  out-of-manifest opcode. The descriptor is admitted by a catalog build, and named negative cases
  produce each catalog refusal this descriptor can provoke. An artifact naming an absent profile
  answers `UnsupportedProfile` / `ProfileNotInCatalog` **with no payload byte examined**; one
  naming an unaccepted manifest answers `UnsupportedFeatureManifest`; one naming an out-of-range
  format version answers `UnsupportedProfileFormatVersion`. A second profile composed in the same
  catalog proves a foreign payload is dropped rather than projected, and every payload kind this
  profile can mint lies inside its declared range. A case proves the executor sizes its operand
  stack from a bound **computed at verification and stored on the verified state**, never from a
  number the payload chose. The descriptor is reachable through exactly one static accessor, and
  no aggregate profile-listing type exists in the graph. A permutation of registration orders
  over the same descriptor set produces a byte-identical catalog identity encoding. A case
  mutates, disposes, and concurrently overwrites the caller's payload buffer after verification
  returns, and neither the verified state nor the execution result changes. The slice corpus
  replays identically twice with no residue, contains at least one successful control entry, and
  the verifier throws on none of it.
- **Seed:** Nothing. This milestone's hand-written encoder and lowering are **scheduled for
  deletion at JS-4** with a named owner and a gate clause, because a second handle-producing path
  and a second lowering are non-goals.

### JS-2 — Take the snapshot; make the copied front end this component's own code

- **Owner:** profile front-end owner, with the release owner co-signing the attribution change.
- **Ledger:** JS-2's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and every open clause of the gate below.
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
  unblock condition. Three things this milestone once waited on are settled and no longer gate it:
  the per-item ruling of
  [section 4.2](roadmap.md#42-what-after-the-fix-work-lands-can-and-cannot-mean) and its stop
  condition, and the nullable and unsafe positions the seed forces, all taken at JS-0.
- **Objective exit gate:** The snapshot identity is recorded recursively and re-derivable; the
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
  its retained evidence, and every open clause of the gate below.
- **Next action:** Publish and version the diagnostic-code registry and the position encoding,
  stating which of the core position record's four fields this profile populates, what it carries
  in the two profile-owned coordinates, and what a section index of `-1` means here. **Record
  which half of the registry each code belongs to** — codes a verified artifact's rejection
  carries in a core result, and codes a pre-artifact source rejection carries on the embedder's
  own seam — per [section 9](roadmap.md#9-the-semantic-front-end-and-lowering)'s boundary
  question, which this milestone answers. Then pin a suite revision and build the harness, the
  self-check, the sharding, the merge, the scope manifests, and the audit command, and score the
  slice manifest.
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
  get; and a scan asserts the suite ingestion path, the corpus store, and every suite file appear in
  no product package and no closure report, with a negative control that adds a product reference to
  the ingestion path and observes the scan fail.
- **Seed:** Nothing is copied. Every mechanism here is this component's own code, and **no total,
  manifest entry, known-gap entry, or triage finding crosses the fork.**

### JS-3b — Static semantics as one verification stage, and the lowering

- **Owner:** verification-boundary owner.
- **Ledger:** JS-3b's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and every open clause of the gate below.
- **Next action:** Consolidate every early error the first manifest requires into one validation
  stage; carry on the tree the facts the two source re-scans recover, and delete the re-scans;
  take and record the strict-mode ownership decision; write the lowering that feeds the one
  verification entry point. Record [section 9](roadmap.md#9-the-semantic-front-end-and-lowering)'s
  boundary answer as a numbered decision: whether the verifier re-derives every early error from
  artifact bytes, or whether the lowering emits a deliberately invalid artifact the verifier
  rejects.
- **Dependencies:** JS-2 for the copied analysis; JS-1 for the format and the verifier shape;
  JS-3a for the registry the diagnostics land in.
- **Objective exit gate:** Every early error the manifest requires is produced by a named case
  carrying a registry code; an illegal format-version and manifest pair is refused by this profile's
  own verifier with a diagnostic code; a construct outside the declared manifest is refused at
  verification and not at first execution, by its own case; **an artifact that is both malformed in
  framing and invalid in static semantics reports exactly one of the two, by a named case that fails
  when the phases are fused**; each artifact is tokenized at most once during verification, asserted
  by a case; the parse-and-early-error slice is scored on JS-3a's harness against the ratchet; and
  the narrow-runtime-compiler composition publishes and runs on every claimed RID with warnings as
  errors, its closure containing the tokenizer and the lowering and no test assembly, and cited
  as evidence for no other composition kind.
- **Seed:** Copied and re-homed — the post-parse validation stage and the free-name analysis.
  Written fresh — the reason mapping, the replacement for both re-scans, and the lowering.

### JS-4 — The value representation and the object model

- **Owner:** profile runtime owner.
- **Ledger:** JS-4's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and every open clause of the gate below.
- **Next action:** Implement the eight-row ABI the entry gate already fixed, retaining the
  correctness fixtures and Native AOT representation probes the gate deferred to this milestone.
  Copy the property storage with its tests and its recorded defect history. Replace the reflective
  key-table initialiser with a generated table under a named owner and make key identity
  realm-scoped. Amputate the dynamic-metaobject interface from the value base type. Route what the
  front end and the executor need through a realm object the composition creates. **Delete JS-1's
  hand-written encoder and lowering**, and assert the deletion.
- **Dependencies:** JS-1 and JS-2. The [section 8](roadmap.md#8-the-value-frame-and-call-model)
  ABI decision is a **gate on entry** and is taken; a taken entry gate is not a started milestone,
  and this one still waits on JS-2.
- **Objective exit gate:** The numbered ABI decision exists with all eight rows, with fixtures and
  AOT representation probes retained; the object model builds with analyzers force-enabled and
  zero trim and AOT warnings in its closure, and a metadata test finds no dynamic-loading,
  reflection-invocation, IL-emit, reflective-member-write, thread-static, or ambient async-local
  construct, **each clause with its own witness**; two runtimes in one process each mint
  properties under the same key text and neither observes the other's storage, shape identity, or
  key identity, in a test that **fails when the key table is made process-wide again**; two
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
  its retained evidence, and every open clause of the gate below.
- **Next action:** Implement the interpreter over the ABI. Implement abrupt completion so
  `finally` runs on every applicable exit including a host exception crossing profile frames.
  Place every poll and every charge. Measure native frame cost per interpreter frame on each
  claimed RID and derive the `CallDepth` default from it. Choose the uncharged-work bound, the
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
  family of [section 8](roadmap.md#8-the-value-frame-and-call-model)**, each with an unsimplified
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
  its retained evidence, and every open clause of the gate below.
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
  its retained evidence, and every open clause of the gate below.
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
  its retained evidence, and every open clause of the gate below.
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
  the malformed corpus is **replayed through the nested path**; and each of the three compositions
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
parser and the regular-expression matcher — do not exist until JS-3b and JS-6, and a session over
surfaces that do not exist may not be read as covering them *(corrected: JSC-23)*.

- **Owner:** profile security owner with the fuzz-corpus owner.
- **Ledger:** JS-9's row in
  [section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) — its state,
  its retained evidence, and every open clause of the gate below.
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
  entries verify successfully, and a repeat leaves no residue; a **mutated corpus entry** proves
  the replay detects a changed observed triple; each fuzz session retains its corpus identity,
  its iteration budget with a stated floor, its runtime settings, and **every minimized
  counterexample**, and any counterexample is closed by a **named regression, never an allow-list
  entry**; the compile-time nesting bound holds under fuzz; a soak over a recorded number of
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
  its retained evidence, and every open clause of the gate below.
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
  plus the accepted format-version range, the accepted manifest set, and the conformance manifest
  identity and version, uses a vocabulary that never reads as a bare yes, gives every row an
  evidence cell naming a rule or a retained artifact, names a deterministic failure or an
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
  every suppression is inventoried with an owner and a reachability argument; and no figure,
  total, claim, or platform result from any other component appears anywhere.
- **Seed:** Nothing. Every figure is this component's own, from this component's own lane and
  commit.

---

## 20. Delivery order

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

Four documents describe one programme, and each is organised for its own kind of reading: the
argument by subject, the milestones by sequence, the gates by evidence class, the risks by failure
mode. **Nothing in that arrangement guarantees they cover the same ground**, and the two ways they
can fail are opposite and equally quiet — a chapter that argues for something no milestone
delivers, and a gate that demands something no chapter designed.

This map is the join. It is read in both directions, and **a blank cell is a finding**: a chapter
with no delivering milestone is a plan for work nobody scheduled, and an evidence area or a release
gate with no owning milestone is a gate nobody can close.

| Chapter | Delivered by | Evidence area, [section 21](roadmap.gates.md#21-test-and-evidence-matrix) | Release gate, [section 22](roadmap.gates.md#22-release-gates) | Stop condition, [section 23](roadmap.gates.md#23-risks-and-stop-conditions) |
|---|---|---|---|---|
| 1 Terminology and support claims | JS-0 fixes the identity; JS-10 issues the table | Identity and registration | 1 | untruthful published claim |
| 2 Engineering invariants | every milestone; each invariant is asserted by the milestone that could first violate it | all rows | 3, 4 | several |
| 3 What the core gives and refuses | JS-0 (the two vectors and the matrix); JS-1 (the descriptor) | Identity and registration; Composed-profile safety | 2 | declared defaults reaching a neighbour |
| 4 The seed | JS-2 | Front end; Licence and attribution | 12 | the seed becomes a dependency |
| 5 Package boundaries and the graph | JS-0 | Dependency architecture | 2 | placement assumed rather than decided |
| 6 Feature manifests | JS-0 allocates; JS-1 mints the first; each increment extends | Identity and registration; Conformance | 1, 9 | the manifest set drifts upward |
| 7 The format and the verifier | JS-1 builds it; JS-9 attacks it | Format and verifier safety | 3 | a check migrates into first execution |
| 8 The value, frame, and call model | JS-4 (the ABI); JS-5 (the measured numbers and the charging) | Value model and storage; Executor and lifecycle; Measurement | 4, 10 | a late value-representation decision; unproportional charging |
| 9 The front end and the lowering | JS-2 (ingest); JS-3b (the stage, the boundary, the lowering) | Front end | 3 | a nesting case terminating the process |
| 10 Execution on the core lifecycle | JS-1 (the loop end to end); JS-5 (the executor) | Executor and lifecycle | 4 | a language fault reported as a core category |
| 11 Guest-initiated loads | JS-8 | Guest loads and policy | 5 | a byte source other than the mediator |
| 12 Suspension | JS-7 | Suspension | 4 | a thread held across a pause |
| 13 Realms, agents, and the host boundary | JS-5 (binding and translation); JS-9 (agents under one parent) | Host boundary | 6 | a shared parent read as isolation |
| 14 The conformance oracle | JS-3a | Conformance | 9 | the oracle reports a failure as a pass; an aggregate percentage |
| 15 Compositions, Native AOT, the browser | JS-1, JS-3b and JS-8 build them; JS-10 advertises one | Native AOT | 7 | a publish cited for another kind; an implied `WebAssembly` namespace |
| 16 Persistence and the code cache | **no milestone delivers it**, by decision; JS-8 carries the exclusion clause and JS-10 measures the reopening trigger | Format and verifier safety | 3 | a second verifier, or a build-time shortcut past the one |
| 17 Measurement discipline | JS-10 stands up the lane; JS-5 produces the first figures it governs | Measurement | 10 | a figure without a control |
| 18 Amendments | JS-0 grades them; **none is filed**, and none is admissible until it names a merged capability | — | — | a requirement with no core row |
| — the standard library, which chapter 6's allocation admits rather than a chapter of its own | JS-6 | Standard library | 1, 9 | dynamic code hiding in the library |
| — the assurance floor and the review debt, which are repository policy rather than this plan's argument | JS-0 adopts the host component's mechanism and records what adoption costs; JS-10 gates the release on it | Assurance and review | 11 | unreviewed units accumulating |
| — packaging and consumers | JS-10 | Packaging and consumers | 8 | a package resolving a dependency from the internet |
| — operational ownership: diagnostics, cancellation, rollback, version rejection, corpus and suite drift, **vulnerability response**, recertification | JS-10 names every owner | Assurance and review | 13 | a role held by nobody |

**What the map shows that no single file does**, and each of the four is deliberate rather than a
gap to be closed:

- **[Section 16](roadmap.md#16-persistence-and-the-code-cache) is delivered by no milestone.** The
  core admits a persisted envelope by contract and implements none, so a profile-owned cache format
  would be a second serialization path with nothing to hold it to the first. The chapter exists to
  keep the design reachable at no cost, and its only obligations on this programme are one gate
  clause at JS-8 and one measurement at JS-10.
- **[Section 18](roadmap.md#18-amendments-this-profile-expects-to-ask-of-the-core) closes no gate
  and appears in no evidence area.** Every row in it is filed and held; the amendment procedure is
  unexecutable while one person holds the minting role and both co-signing roles.
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
- **One release gate is owned by no chapter, and the map is how that was found.** Gate 13 asks that
  the holders of diagnostics, cancellation, rollback, format-version rejection, corpus and
  suite-revision drift, **vulnerability response**, and recertification each be named. No chapter
  argues for it, because it is an operational obligation rather than a design one — and this
  component ships a parser and an interpreter over untrusted input, so a release with no named
  holder for a report about either is not a release. JS-10 names them or the gate refuses.
