<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The workload roadmap — what running the two third-party corpora whole would take

**What this document is.** A gap analysis and a proposed programme for one objective: that the two
third-party workloads this profile already points at — the Octane benchmark and the pinned test262
suite — run **whole**, rather than in the part of each that the admitted surface happens to reach.
It is written against the surface as it stands, established by asking each host what it admits one
name at a time, and it names for every gap either the milestone that already owns it or the fact
that nothing does.

**What this document is not.** It is not the ledger and it moves no row in one:
[section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) remains the only
authority on what this component has done, and nothing here is a status, an acceptance, or a
schedule. It is not a milestone set either.
[Section 19 of the delivery file](roadmap.delivery.md#19-milestones) holds `JS-0` through `JS-10`,
and this document mints no identifier in that namespace, because a `JS-` identifier with no ledger
row would read as a milestone somebody is tracking. Its stages are `JSW-n` and they are proposals
for where the existing milestones would have to grow.

**And it carries no figure of any kind** — no score, no count, no pass rate, no ratio. Every claim
below is about behaviour: what a host admits, what a workload does when it meets an absence, and
what would have to exist for it to do something else. The runs that prompted this document were
taken outside any retained bundle, and under the ledger's own update rule 10 a number with no
retained record behind it is not a number this document family may state. Where a figure would be
the natural way to say something, this document names the workload and the command instead.

---

## 1. The target, stated as behaviour rather than as a score

**A workload runs whole when the host's answer is the workload's own.** Concretely:

- **Octane.** Every benchmark in the checkout reports a score through the ordinary command line of
  [the end-user host](roadmap.md#15-deployment-compositions-native-aot-and-the-browser-embedding),
  driven one benchmark per process by `src/tests/octane/run-one.js`, and the process exit code
  agrees with whether a score was produced. A benchmark that reports its own error is not running
  whole even when the host exits cleanly, which is why that driver re-throws.
- **test262.** Every subtree of the pinned suite runs, in both variants the metadata asks for, and
  the `unsupported` column is **empty for the manifest that claims the surface the case needs**.

That second clause is the one worth stating precisely, because the obvious reading of it is wrong.
`unsupported` exists to keep an unadmitted construct out of both the pass and the fail columns —
bundle [JS-4-001](evidence/js-4-001/README.md) section 4 records what it is protecting against, and
that a great many cases expect a `SyntaxError` and would score a false pass against a front end
that refuses their construct for the wrong reason. **The target here is not that the column is
hidden or reinterpreted. It is that the manifest grows until the column empties honestly**, one
construct family at a time, with every family that stays out named as an exclusion rather than
absorbed into a verdict.

**Running whole is not running fast, and this document is not about speed.** Throughput,
baselines and the measurement lane are `JS-10`'s subject in
[section 19](roadmap.delivery.md#19-milestones); nothing here proposes a performance target, a
representation change, or an execution-model change, and section 7 below says so again where a
reader is most likely to want otherwise.

---

## 2. What the comparison engine admits, and what that comparison is worth

The legacy component `Broiler.JS` is the comparison this document was asked for, and **the
comparison is a surface comparison and not a speed one.**

Asked one name at a time, its script host answers to every global named in section 3.2 below, and
it compiles every syntactic form named in section 3.3. The only form in that probe it refused is a
`super` call in the constructor of a class with no heritage — which the language refuses too, so
the probe found no refusal that was the engine's rather than the specification's.

**What does not transfer is everything about how it gets there.** That engine compiles guest
functions to delegates and lets the host runtime generate machine code for them; this profile
verifies a bytecode artifact and interprets it under a fuel budget, and
[section 2](roadmap.md#2-engineering-invariants) is why. Nothing in this document proposes adopting
the comparison engine's execution model, its value representation, or its code-generation path, and
no stage below is justified by anything it measures. **What transfers is the list**: the set of
surfaces a JavaScript implementation has to have before third-party material stops meeting an
absence, established by an implementation in this repository rather than argued from first
principles.

---

## 3. The gap, workload by workload

### 3.1 What each workload meets today

**This table has two columns of history and one of the present.** It was written when every row
below was a gap; the stages of section 5 have since been built, and what each row *met* then and
*meets now* are both recorded, because a plan that quietly replaced the first with the second would
be a plan a reader could not check *(corrected: JSC-87)*.

| Workload | What it met when this was written | What it meets now |
|---|---|---|
| Octane `mandreel` | A reference error naming a typed array constructor | It reports a score |
| Octane `gbemu` | The benchmark's own report that typed arrays are unsupported | It reports a score |
| Octane `zlib` | A reference error naming `eval` | It reports a score, past a **host** absence the realm answers by refusing rather than by being absent — `read`, which no manifest owns and which core contract version 1 has no shape for *(JSC-84)* — and under a memory allowance the caller states *(JSC-90)* |
| Octane `code-load` | A reference error naming `eval` | It reports a score |
| Octane `regexp` | It runs, and the benchmark's own checksum disagrees with what it produced | Its own checksum agrees |
| Octane `pdfjs` | **The verifier refuses an artifact this host produced** | It reports a score *(JSC-81)* |
| Octane `typescript` | A type error reading a property of `undefined`, after running for a while | It reports a score *(JSC-82)* |
| test262, asynchronous cases | They cannot complete, because a promise never settles | A promise settles, at a drain point the host states and asks for |
| test262, module cases | They are not run as modules by this host | Unchanged: there is no module goal |
| test262, the suite as a whole | Only chosen subtrees have run | Unchanged: roadmap [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)'s rule is still unmet, and JSW-10 still owns it |

**Every row above is a behaviour, and every one of them is reproducible from the commands the
retained bundle's procedure section already carries.** Two rows were defects rather than absences and
section 3.4 separates them out, because a plan that schedules a repair as though it were a feature
has mis-stated both.

**One row changed kind before it closed, and that is the finding worth keeping.** `zlib` stopped
meeting a language absence and met a **shell** the benchmark assumes and this host does not have.
Nothing in the seven-identity allocation of section 6 would ever have caught it, because it is not a
language surface at all — and asking for it is the first amendment this profile has an observed
reason to put to the core. The row closed once the realm answered that probe by REFUSING rather than
by being absent, and once the memory allowance stopped being a figure only a rebuild could move: the
benchmark printed its score and then met a ceiling, so the process exited non-zero on a run that had
produced exactly what section 1 asks for *(JSC-90)*.

### 3.2 The surface that is absent from the realm

Asked with `typeof` — which answers for an undeclared name without throwing, so that one absence
hides no other — the wide manifest's realm answers to `Object`, `Function`, `Array`, `String`,
`Number`, `Boolean`, `Math`, `JSON`, `Date`, `RegExp`, `Error` and `globalThis`, plus the host's
own `print`.

**Absent when this was written, and present in the comparison engine:** `Symbol`, `BigInt`,
`Proxy`, `Reflect`, `Promise`, `WeakRef`, `FinalizationRegistry`, `Map`, `Set`, `WeakMap`,
`WeakSet`, `ArrayBuffer`, `SharedArrayBuffer`, `DataView`, every typed array constructor,
`Atomics`, `Intl`, `Temporal`, and `eval`.

**Absent still, and the list is shorter than it was** *(corrected: JSC-87)*. `Proxy`, `Reflect`,
`BigInt`, `Intl` and `Temporal` are absent; `SharedArrayBuffer` and `Atomics` are absent
**deliberately**, excluded by name from the binary identity for the reason section 4 gives. The
rest — `Symbol`, `Promise`, the keyed collections, the weak references, `ArrayBuffer`, `DataView`,
the typed array constructors and `eval` — are present, three of them behind an optional surface a
composition may decline.

**That set is wider than the retained bundle's exclusion list, and the difference is the reason
this section exists rather than pointing.** Bundle [JS-4-001](evidence/js-4-001/README.md) names
`Proxy`, `Reflect`, `Symbol`, `BigInt`, every typed array and `eval` as absent from the realm; the
keyed collections and `Promise` are absent too and are not on that list. The bundle is not wrong —
its list is of the absences a reader would most expect to ask about, and it does not claim to be
exhaustive — but a plan has to work from the whole set, and an exclusion list that a later reader
mistakes for the whole set is how a gap survives a review. **JSW-6 carries the obligation to make
the two agree**, by publishing the realm's admitted set from the realm itself rather than from
prose.

### 3.3 The syntax that is refused by name

Bundle JS-4-001 records what the front end refuses at its own position, with the construct named: a
class declaration or expression, `super`, a generator function, an `async` function, `await`,
`yield`, a module declaration, `with`, `for … of`, an optional chain, a template literal, a tagged
template, `new.target`, a destructuring pattern, a destructuring parameter, a destructuring catch
parameter, a rest parameter, a parameter default, and a spread argument, element or property.

**Refusal by name is a property this programme must not spend.** It is what keeps an unadmitted
construct out of the pass column, it is what section 4 of that bundle records as having leaked
once and been repaired, and every stage below that admits a construct family removes a refusal that
something was relying on. So each such stage carries the same clause: the family moves from
*refused by name* to *admitted and exercised*, and no family moves to *refused as an unexpected
token* on the way.

**And admitting a family CREATES positions the audit has never seen.** That is the clause a reader
is most likely to skip and the one that has cost most. A class field initialiser and a class static
block did not exist as syntactic positions until the class body was admitted; an async generator
written in either was refused for the enclosing construct and never reached on its own, so a family
that answered correctly everywhere the audit looked could answer with a surprise token in a position
the audit had no row for. The audit is therefore re-run over a MATRIX rather than a list, in both
directions — every position a refused family can be written in, and every position an admitted one
can — and it is retained as `eng/audit-refusals.py` so that a later stage runs the same question
rather than a remembered version of it.

### 3.4 The two failures that are defects rather than absences

**`pdfjs` is refused by this component's own verifier, on bytes this component's own lowering
produced.** That is an internal-consistency failure and not a missing feature: nothing about the
program is outside the manifest, or the front end would have said so by name. Either the lowering
emits something [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier)'s format does not
admit, or the format admits something the verifier's semantic stage then rejects. **Which of those
two it is, is the first question JSW-1 answers**, and the answer decides whether the repair belongs
to the lowering or to the format's specification.

**The answer was neither, and the fork was drawn one level too high** *(corrected:
[JSC-81](roadmap.corrections.md#jsc-81))*. The lowering emitted only instructions the format admits
and the verifier decodes, and the verifier was right to refuse the composition of them: an array
literal that is not dense-and-small left **nothing** on the operand stack where its caller expected
one value. A lowering can be internally inconsistent while emitting nothing either named component
could object to on its own, and a stage that had only looked for a disagreement between the two
would not have found it.

**`typescript` fails with a type error against a value the program did not expect to be
`undefined`.** That is a semantic defect somewhere inside the admitted surface, found the way the
three defects bundle JS-4-001 records were found — by a program longer and stranger than anything
anybody in this repository would write. It is exactly the outcome section 3 of that bundle says the
workloads exist to produce, and recording it as a defect rather than repairing it quietly is the
same discipline.

**It was the `arguments` binding, and the program that found it is one nobody here would have
written** *(corrected: [JSC-82](roadmap.corrections.md#jsc-82))*. A formal parameter named
`arguments` had its value destroyed on entry, because the compile-time scope answers a repeat
declaration with the slot it already has and the arguments object was written into the parameter's
own slot. The Octane TypeScript compiler has
`function FuncDecl(name, bod, isConstructor, arguments, …)` and then reads `this.arguments`. Asking
what else the same walk answered wrongly found a second defect the workload never reached
*(corrected: [JSC-83](roadmap.corrections.md#jsc-83))*, which is the habit rather than the benchmark
producing the finding.

---

## 4. The gap nothing currently owns: the binary surface

**Typed arrays, `ArrayBuffer` and `DataView` appear in no manifest and in no milestone.** The
allocation in [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)
names `slice`, `core`, `modules`, `dynamic`, `regexp`, `intl` and `temporal`, and the binary
surface is in none of them; the milestones in
[section 19 of the delivery file](roadmap.delivery.md#19-milestones) name none of it either. Two
Octane benchmarks need it outright, machine-generated code in the wild assumes it, and a large part
of test262 is written against it.

**It needs a manifest identity of its own, for the reason `broiler.javascript.dynamic` has one.**
That identity exists so a composition registering no artifact provider can decline exactly one
thing and say so. A buffer is shared mutable memory addressed by index, handed to a guest whose
whole argument is a verified artifact executing under a metered budget — so whether a guest may
hold one is precisely the kind of question a composition has to be able to answer separately, and
an identity is how [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)
makes a question answerable.

**Proposed:** `broiler.javascript.binary`, admitting `ArrayBuffer`, `DataView` and the typed array
constructors. **`SharedArrayBuffer` and `Atomics` are deliberately not in it**: they are the
multi-agent surface, they need the agent model of
[section 13](roadmap.md#13-realms-agents-and-the-host-boundary), and folding them in would let a
composition that wanted an ordinary byte buffer admit cross-agent shared memory by accident.

Two consequences a reader should not have to derive. **A typed array is an exotic object**, so it
lands on the object model of [section 8](roadmap.md#8-the-value-frame-and-call-model) rather than
beside it, and the integer-indexed exotic behaviour is part of the surface rather than an
optimisation of it. And **detachment is observable**: a detached buffer changes what every view
over it does, which is a state transition the executor has to charge and the verifier has to have
no opinion about.

---

## 5. The stages

Each stage below states an objective, what it waits on, and an exit gate written the way
[section 19's](roadmap.delivery.md#19-milestones) gates are written — as conditions a run can
decide, not as work items. **None of them is scheduled and none has an owner**; assigning either is
the act that would turn a stage into a milestone with a ledger row.

### JSW-1 — The two defects the workloads already found

- **Objective.** `pdfjs` and `typescript` stop failing for reasons that are this component's rather
  than the manifest's. Either they produce a score, or they meet a named absence like every other
  row in section 3.1.
- **Waits on.** Nothing. Both defects are in the surface as it stands today.
- **Exit gate.** For each defect: a diagnosis recorded as an appended entry in
  [the corrections file](roadmap.corrections.md), naming which of the two components named in
  section 3.4 was wrong; a regression fixture inside this repository that reproduces it without the
  third-party file, since a fixture that needs an unpinned checkout is not a fixture; a negative
  control that fails when the repair is reverted **and produces a wrong answer rather than an
  exception**, because a control that crashes has judged nothing; and both workloads re-run through
  the ordinary command line with their new behaviour recorded whole, failures included.

### JSW-2 — The binary surface, and a manifest identity for it

- **Objective.** `broiler.javascript.binary` exists, is declared by a composition, and `mandreel`
  and `gbemu` stop meeting an absent constructor.
- **Waits on.** The object model — `JS-4` in the ledger's
  [section 2](roadmap.status.md#2-current-milestone-status) — for exotic objects. It does **not**
  wait on the snapshot: nothing here is ingested, and the argument is the one
  [JSC-15](roadmap.corrections.md) made when it split a milestone by dependency rather than by size.
- **Exit gate.** A dated decision allocating the identity, stating the `SharedArrayBuffer` and
  `Atomics` exclusion by name and why the agent model owns them; the integer-indexed exotic
  behaviour exercised including the out-of-range and detached cases; **a composition that declines
  the identity refuses an artifact naming it, at verification, with an invalid-artifact reason** —
  which is the property [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)
  describes and which bundle JS-4-001's exclusions record as unmet for `eval` today; both benchmarks
  reporting a score through the ordinary command line; and the retained corpus growing entries that
  carry the surface, with the fuzz sessions of `JS-9` covering them.

### JSW-3 — The dynamic surface, and a refusal that happens where the plan says it does

- **Objective.** `eval` and the `Function` constructor exist under `broiler.javascript.dynamic`, so
  `zlib` and `code-load` stop meeting an absent binding — and, equally, so that a composition
  declining the identity refuses them the way the plan describes.
- **Waits on.** `JS-8` in the delivery file, whose subject this is;
  [section 11](roadmap.md#11-guest-initiated-loads-eval-the-function-constructor-dynamic-import-modules)
  is the design.
- **Exit gate.** The mediator and the provider exist and a composition registers neither, one, or
  both, with each configuration's behaviour recorded; **the refusal for a declining composition
  happens at verification with an invalid-artifact reason rather than as a run-time
  `ReferenceError`** — bundle JS-4-001 records the current behaviour as the difference between what
  section 6 describes and what the realm does, and closing that difference is this stage's, not a
  later one's; direct and indirect `eval` are distinguished and each has a witness; and both
  benchmarks report a score.

### JSW-4 — Regular expressions over the from-scratch matcher

- **Objective.** `regexp` stops disagreeing with its own checksum, because the matcher is the one
  this profile owns rather than a translation to another engine's dialect.
- **Waits on.** The satellite acquisition that `JS-6` names, and which the ledger's
  [section 3](roadmap.status.md#3-open-external-dependencies) carries as an open external
  dependency. **This is the one stage in this document whose blocker is outside the component.**
- **Exit gate.** `broiler.javascript.regexp` is declared and the closure contains no call site
  constructing a compiled-mode regular expression from the host platform, asserted by its own
  metadata test — which is already `JS-6`'s clause and is restated here because this stage is what
  makes it reachable; the benchmark's own checksum agrees; and the dialect differences that
  remain — if any survive the matcher — are published as exclusions with a deterministic failure
  each, rather than discovered by the next workload.
- **What the stage did, and the one clause of its own gate it did not take.** The matcher is this
  profile's, written here rather than translated onto the platform's engine, and it lives in the
  format assembly because the front end needs it too: an invalid pattern in a literal is an early
  error, so the same grammar has to be readable by the thing that compiles a literal and by the
  thing that runs one. **The benchmark's checksum agrees**, which is the sentence this stage was
  written to make true, and the closure holds no call site into the platform's engine — asserted
  from 2026-09-05 by rule **N18**, watched failing against an injected import and passing after the
  revert, because the state of a tree that nothing checks is a state one edit can undo.
  **`broiler.javascript.regexp` is not declared**, and that clause is left unmet deliberately
  rather than quietly: the surface is inside `broiler.javascript.wide`, moving it would re-scope a
  manifest every retained artifact names, and minting an identity nothing declares would be a
  manifest with no artifacts rather than a boundary. That is a decision for a person to take, and
  [JSC-167](roadmap.corrections.md#jsc-167) records it as owed rather than done. **The dialect
  differences that remain are the Unicode data's**, which the ledger's
  [section 3](roadmap.status.md#3-open-external-dependencies) still carries as an open external
  dependency with a holder.

### JSW-5 — The core language surface still refused by name

- **Objective.** The construct families listed in section 3.3 move from refused to admitted, under
  `broiler.javascript.core`, in the order that empties test262's `unsupported` column fastest.
- **Waits on.** The lowering — `JS-3b` — and the executor — `JS-5`. Generators, `async`, `await`
  and `yield` additionally wait on JSW-7, because a suspension that cannot be resumed by a queue is
  half a feature.
- **Exit gate.** Per family, and the gate is per family rather than for the set, because a set-level
  gate would let the last family in be the one nobody exercised: the family is admitted, exercised
  by a fixture inside this repository, **and no longer refused as an unexpected token in any
  syntactic position it can appear in** — the audit section 4 of bundle JS-4-001 describes, re-run
  for the family, since that audit is what caught a refusal that produced the right outcome for the
  wrong reason. `unsupported` shrinks for the manifest claiming the family, with the subtree runs
  that show it retained whole, failures included.
- **What the stage has admitted, family by family.** The class DECLARATION and EXPRESSION, `super`
  and `new.target`; the generator family; the `async` family and `await`; `with`; and, on
  2026-09-05, the whole of the class BODY — a class field with its initialiser, a class static
  block, a private name in every form the grammar gives it (a field, a method, a getter, a setter,
  each of those `static` as well, `this.#x`, `o.#x`, `o?.#x`, `#x in o`, and a private access as a
  destructuring target), and a generator member of a class body. Each is exercised by fixtures under
  `src/tests/cli` — `runs/` for the ones that produce a value and `threw/` for the three whose point
  is an uncaught throw — and by cases in the retained differential probe, whose answers were taken
  from the comparison engine before they were written down.
- **And, on 2026-09-05, the ASYNC GENERATOR and `for await`, which the paragraph this replaces left
  refused.** `async function*` as a declaration and as an expression, an async generator method of an
  object literal and of a class body — `static`, private, computed and Symbol-keyed — `await` and
  `yield` in one body without either being mistaken for the other, `yield*` over an async or a
  synchronous inner iterator, and `for await` in an async function, an async method, an async arrow
  and an async generator. **An async generator is not the two families it is spelled from**, and what
  it needed beyond them is what this stage built: a REQUEST QUEUE on the generator object, so that a
  second `next` before the first settles is answered in order rather than re-entering a running
  frame; `next`, `return` and `throw` each answering a promise, including when they are errors;
  `%AsyncGeneratorFunction%`, `%AsyncGeneratorFunction.prototype%`, `%AsyncGeneratorPrototype%` and
  `%AsyncIteratorPrototype%`, with `Symbol.asyncIterator` reaching them; and
  `%AsyncFromSyncIteratorPrototype%`, which is what makes `for await` over an Array of promises
  iterate what they resolve to. The suspension the two families share is told apart by the FRAME —
  `JsFrame.Suspension` is written by the instruction that left the dispatch loop — because an async
  generator's body is the first that can suspend two ways into one frame, and the two mean opposite
  things to the driver that receives the value.
- **What the family cost the call-depth measurement, which is the first time it cost anything.** Five
  dispatch arms took the executor's own frame from 4,073 bytes to 4,551 and the capacity on the
  declared stack from 16,478 calls to 14,737 — 1.80 times the ceiling a host may be granted, below
  the factor of two [JSC-126](roadmap.corrections.md#jsc-126) had already called the narrowest it had
  been. The guest stack was raised to ninety-six megabytes and re-measured at 22,122 calls, which is
  2.70 times that ceiling; [JSC-139](roadmap.corrections.md#jsc-139) records both figures and why the
  stack moved rather than the ceiling.
- **The decorator stays refused as well**, and it is not this stage's to admit: it is a proposal
  rather than a production of the pinned edition, and the tokenizer refuses it by name at the `@`.

### JSW-6 — The core library still absent from the realm

- **Objective.** The keyed collections, `Symbol`, `Promise` as a value, and the weak references stop
  being absent, and the realm can say what it admits without a document being consulted.
- **Waits on.** `JS-6`, whose rewrite this extends rather than replaces.
- **Exit gate.** The realm publishes its admitted global set from the realm itself, and a test
  compares that set against the exclusion list any document carries, **failing when the two
  disagree in either direction** — which is what section 3.2's discrepancy costs if nothing enforces
  it; `Symbol` exists far enough for the iteration protocol that `for … of` needs, so JSW-5's
  `for … of` family has something to iterate; and each surface that stays out is named with its
  deterministic failure.

### JSW-7 — Settling: the job queue

- **Objective.** A promise settles, so an asynchronous test262 case can complete rather than being
  a case this host cannot ask.
- **Waits on.** `JS-7`. The design needs no copied code and the delivery file already records that
  it may be opened early.
- **Exit gate.** The queue is drained by the host at a point the host chooses and states — an
  embedding decides when to run jobs, and a queue drained implicitly at an unstated point is a
  behaviour no embedder can reason about; the drain is charged against a budget dimension of
  [section 3](roadmap.md#3-what-the-core-already-gives-this-profile-and-what-it-refuses) rather than
  being free; a job that never settles is a resource exhaustion with a named dimension rather than a
  hang; and test262's asynchronous cases run and score.

### JSW-8 — The module goal

- **Objective.** `broiler.javascript.modules` exists and test262's module cases run as modules.
- **Waits on.** JSW-5 for the syntax, JSW-7 for top-level `await`.
- **Exit gate.** Module records, live bindings and the import and export forms are exercised; a
  cyclic import terminates with a named diagnostic rather than by exhausting a budget; the host's
  module resolution is the composition's rather than the profile's, and a composition that provides
  no resolver refuses a module artifact at verification; and the module subtrees run.

### JSW-9 — The depth a generated program needs

- **Objective.** A machine-generated program with deep recursion — the `mandreel` shape — reaches a
  score or a named exhaustion, rather than terminating the process or exhausting a budget chosen
  against an estimate.
- **Waits on.** Nothing, and it is worth doing early: the comparison engine fails this workload too,
  by exceeding its own stack budget, so this is not a gap relative to it — it is a gap relative to
  the target.
- **Exit gate.** **The per-frame cost of this interpreter is measured rather than estimated**, which
  bundle JS-4-001's exclusions record as an outstanding gap for the stack a guest invocation is
  given; the depth maximum is derived from that measurement and recorded with it; exceeding it is a
  resource exhaustion naming its dimension, never a process termination — the property
  [JSC-79](roadmap.corrections.md) exists for; and the recursive workload runs under Native AOT on
  every claimed runtime identifier, because that gate is where a stack claim is decided rather than
  on one machine under a JIT.

### JSW-10 — The runs, per manifest, whole

- **Objective.** Each manifest this programme creates or widens has a retained conformance run of
  its own over the whole pinned suite, and the Octane checkout is retrieved, hashed and archived so
  a run against it means something the next reader can check.
- **Waits on.** Everything above, and the third of the human actions the ledger's
  [section 3](roadmap.status.md#3-open-external-dependencies) records as open.
- **Exit gate.** For each manifest: a whole-suite run retained under
  [section 4's evidence contract](roadmap.status.md#4-required-evidence-bundle), with the four
  verdicts reported and every `unsupported` family named; the Octane checkout pinned and archived
  beside the conformance suite the way
  [section 14](roadmap.md#14-the-conformance-oracle)'s pinning already works, so that a benchmark
  result has an identity; and a lane that runs both workloads on every claimed runtime identifier,
  since a workload that has only ever run on one machine under a JIT is what let a failing Native
  AOT run reach two retained bundles before anything noticed.
- **What the stage did.** The Octane checkout was retrieved, hashed and archived on 2026-09-04 the
  way [section 14](roadmap.md#14-the-conformance-oracle)'s pinning already works — the archive
  `git archive` produced at the pinned commit, a digest over its own members, and the upstream
  licence retained beside it — and the obligation that archive triggers was discharged a day later,
  when `THIRD_PARTY_NOTICES.md` gained its row and rule N13 gained the directory that makes the
  row's own mechanism sentence true *([JSC-145](roadmap.corrections.md#jsc-145))*.
- **Both workloads run out of the published image on every claimed runtime identifier**, in the
  publish job of the lane, beside the component's own suites. Each runs over a selection, and each
  selection is stated in the step rather than left to be inferred: what changes with the runtime
  identifier is whether the published image can execute a corpus, not what the engine answers about
  it. The conformance selection is four subtrees on every cell and the whole suite once on one
  Linux runner, because the archive is 232 MB extracted. The Octane selection is the caller's —
  six benchmarks on the quick lane's two cells, all fifteen on the full lane's six — and that
  asymmetry is a **measurement rather than a plan**: the step's first run scored fifteen benchmarks
  in 23m28s on `ubuntu-latest`, on a job whose every other step totals 78 to 208 seconds
  *([JSC-180](roadmap.corrections.md#jsc-180))*. The per-benchmark allowance is the profile's own
  `WallClock` maximum rather than a figure the lane chose, because an hour is the most any
  composition of this profile may be granted — so a benchmark that meets it has met **this
  engine's ceiling**, which is a finding rather than a bound to move. **The exclusions are that no
  identifier but the Linux one has scored the whole conformance suite, that no identifier scores
  the whole Octane set on the push path, and that `zlib` has no known duration on any machine here
  — it meets that ceiling on the one it was measured on, and whether it fits inside the hour on a
  runner is what the first full lane answers.**
- **The whole-suite runs are retained per manifest**, under
  [section 4's evidence contract](roadmap.status.md#4-required-evidence-bundle), with the harness's
  five verdicts reported rather than four — a variant that spent an allowance is neither a pass, a
  failure, a construct outside the manifest nor a skip, and folding it into any of those would hide
  the one outcome a reader can act on by raising a number. **Every family the `unsupported` column
  names is named in the bundle**, and for `broiler.javascript.wide` there are none to name: the
  column is empty, which is what section 1 asked for and the form it asked for it in.

---

## 6. Order, and what is schedulable today

**Three stages need nothing that does not exist.** JSW-1 repairs defects in the surface as it
stands. JSW-9 measures a per-frame cost that is measurable today. JSW-2 needs the object model that
`JS-4` has already begun, and it is the largest single unlock in the list.

**None of the three waits on the snapshot.** `JS-2` is `Blocked` in the ledger and stays blocked;
the argument that this does not block the work is [JSC-15](roadmap.corrections.md)'s and it applies
again here — the dependency is of the ingest, not of a surface written from the specification.
Applying that test before accepting a blocker is a habit this profile has already needed twice.

**One stage has a blocker outside the component.** JSW-4 waits on the satellite acquisition, and no
amount of sequencing inside this document moves it.

**The rest are ordered by what they unblock rather than by size**: JSW-3 and JSW-6 are independent
of each other and both feed JSW-5; JSW-7 gates the suspension half of JSW-5 and all of JSW-8; and
JSW-10 closes over all of them, because a manifest's run is only worth taking once the manifest has
stopped moving.

---

## 7. What this roadmap does not promise

- **It does not promise speed, and no stage in it is justified by a measurement of speed.** The
  comparison engine executes by a different model and the difference between the two is not a defect
  this document is repairing. Throughput, baselines and the measurement lane are `JS-10`'s.
- **It does not promise `Intl` or `Temporal`.** Both have manifest identities and both are deferred
  by name in [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted).
  test262 has subtrees for each, so **"every test" is not what section 1 claims**, and a reader who
  wants those subtrees is asking for a scope this document does not propose.
- **It does not promise `SharedArrayBuffer` or `Atomics`.** Section 4 excludes them from the
  proposed identity deliberately, and the agent model owns them.
- **It does not accept anything.** No stage here moves a ledger row, and a stage's exit gate being
  met is not acceptance: acceptance needs an owner and a reviewer decision, which nothing in this
  component has.
- **It does not promise that test262 passes.** The target in section 1 is that the suite **runs**,
  with the `unsupported` column empty for the claiming manifest. A pass rate is a different claim
  and needs a retained run to state at all.
- **And it does not promise that refusal by name survives for free.** Section 3.3 says why that
  property is worth more than the constructs it currently refuses, and every stage that admits a
  family is a chance to lose it quietly.

---

## 8. What a stage would owe if it were scheduled

Nothing in this document changes the obligations any work in this component already carries, and
they are restated here only because a stage list invites the reading that a plan replaces them:

- A retained bundle per [section 4's](roadmap.status.md#4-required-evidence-bundle) nine fields,
  collected by this profile's own script, with its failures and exclusions retained rather than
  summarised.
- A registered rule in the architecture rule register for every new mechanism, with a witness whose
  file name starts with the rule identifier, and negative controls that have been **watched
  failing and watched passing after revert** — a control nobody has seen fail is worth nothing.
- Appended entries in [the corrections file](roadmap.corrections.md), never edits to old ones, with
  the plan pointing at them by bare marker.
- Ledger rows moved in the same change that changes what they claim, and no count, total or score
  copied into prose anywhere.
