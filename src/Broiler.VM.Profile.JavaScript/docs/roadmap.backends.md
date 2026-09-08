<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The backend roadmap — what a second and third output form would take

**What this document is.** A design analysis and a proposed programme for one objective: that a
JavaScript program this profile's front end compiles can be compiled to an artifact whose payload is
**machine code** and executed in this process. It is written against the front end, the format and
the runtime as they stand in this checkout, and it names, for every obligation it records, either the
milestone or stage that already owns it or the fact that nothing does.

**IT DESCRIBED A DIFFERENT DESIGN UNTIL 2026-09-07, AND THE DESIGN IT DESCRIBED WAS NOT THE ONE
BUILT.** As first written this document promised a **per-unit** form choice inside one artifact: a
backend that compiled the units it found eligible, an entry guard on each emitted unit, and the
interpreter carrying every unit the backend declined. What exists instead is **whole-artifact or
nothing**, under a feature manifest small enough that a whole program in it is compilable, with no
guard, no bailout and no fallback anywhere. **The reason for the change is not that the first design
was harder; it is that this repository's own published rules forbid it** — the reasoning is section 3
below and the correction is [JSC-199](roadmap.corrections.md#jsc-199). The old reading is recorded
there rather than deleted, because a reader who planned against a per-unit backend is owed the
retraction.

**What this document is not.** It is not the ledger and it moves no row in one:
[section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) remains the only
authority on what this component has done. **This profile now owns backend code** — a numeric
admission pass, two instruction encoders, an artifact section pair, a verifier arm and the one type
that maps a page executable — and **owning code is not acceptance and is not evidence**: no bundle
has been retained for any of it, no human has read a line of it, and every stage below that owns code
says so in the same breath as it says what it does not have. It is not a milestone set:
[section 19 of the delivery file](roadmap.delivery.md#19-milestones) holds `JS-0` through `JS-10`
and this document mints no identifier in that namespace, because a `JS-` identifier with no ledger
row would read as a milestone somebody is tracking. It is not a third gap analysis either —
[the workload roadmap](roadmap.workloads.md) owns the large absences in the language surface and
[the parity roadmap](roadmap.parity.md) owns the mechanisms inside the surface already admitted, and
where this document meets one of theirs it points at it rather than re-minting it. Its stages are
`JSB-n` and they are proposals for where the existing milestones would have to grow.

**And it is not the core's milestone.** The core opened
[VM-7](../../../docs/roadmap.md#vm-7--admit-a-native-artifact-form-and-in-process-native-execution),
which admits a native artifact form and a composition that executes one; the core's own ledger is
the authority for what VM-7 has done, and under the ledgers' shared update rule 6 nothing in this
file advances a row there and nothing there advances a row here. **VM-7 admits a form; it does not
supply a backend.** What a backend for *this* language would take is the subject below, and writing
it down schedules none of it.

**And it carries no figure of any kind** — no timing, no ratio, no instruction count, no score.
**That rule survived the form being built and it is now load-bearing rather than precautionary**: a
native form that runs invites exactly one number, an observation of that number has been made on one
machine with no A/A lane, no predeclared rule, no repetitions and no benchmark host, and it is
recorded nowhere in this repository and appears nowhere below.
Under the ledger's [update rule 10](roadmap.status.md#5-update-rules) a number with no retained
record behind it is not a number this document family may state, and the numbers a native form
invites are exactly the numbers nobody here has collected. Where a figure would be the natural way
to say something, this document names the command or the retained bundle instead.

**The probe VM-7 was opened against is unretained and unreproducible, and its figures are deleted
from the core record rather than borrowed into this one.** The core's own record says it plainly: the probe was two
throwaway programs on one machine, it was not collected by the benchmark host, it has no A/A lane,
no predeclared rule and no retained repetitions, `docs/evidence/` holds no `vm-7` directory because
there is no bundle to put in one, and adding the probe programs to the tree would put a project in
neither solution, which rule A14 reports. So the per-call and per-instruction timings that bullet
once carried are **deleted from the core record and appear nowhere below**, and the probe itself is
**unreproducible from this repository as it stands**. A reader who needs them re-derives them; a reader who wants to justify a
stage with them cannot, and that is the correct relationship between that probe and this programme.

**And one record stands above this one.** The MVP programme at
[`docs/mvp.md`](../../../docs/mvp.md) states what this stage of work defers and what deferring a
decision does not defer, and this document defers to it: what is deferred is approval of the
boundary records, human review, evidence-bundle collection and milestone acceptance, and the
co-signing step of the core contract amendment procedure, and what is **not** deferred is the
automated gates, the status vocabulary, the stop condition on an untruthful support claim, the
non-advertisement of every composition and the three-package pack set, and the prohibition on
publishing. **That is four deferred and five not deferred, counted exactly** *(corrected
2026-09-07: this document had carried three deferred items and a four-item not-deferred list with
the closing sentence of that record substituted for two of its terms)*, and the fourth not-deferred
item is one item with two halves that are never quoted apart. Beside all nine stands the rule that a
document plans work and records state rather than reporting a capability no run has shown. Every
sentence below is written under that rule.

---

## Contents

1. [The target, stated as an artifact rather than as a speed](#1-the-target-stated-as-an-artifact-rather-than-as-a-speed)
2. [The seam, named precisely: one lowering with a second exit](#2-the-seam-named-precisely-one-lowering-with-a-second-exit)
3. [The constraint this whole document is organised around](#3-the-constraint-this-whole-document-is-organised-around)
4. [The routes this document takes without a decision](#4-the-routes-this-document-takes-without-a-decision)
5. [What this roadmap does not advance, and the mistake it would be](#5-what-this-roadmap-does-not-advance-and-the-mistake-it-would-be)
6. [The stages](#6-the-stages)
7. [Order, and what is schedulable today](#7-order-and-what-is-schedulable-today)
8. [What this roadmap does not promise](#8-what-this-roadmap-does-not-promise)
9. [What a stage would owe if it were scheduled](#9-what-a-stage-would-owe-if-it-were-scheduled)

---

## 1. The target, stated as an artifact rather than as a speed

**A backend has arrived when an artifact this profile produced carries machine code, this profile's
verifier answers about it, and this profile's executor runs it — and when the artifact is native in
whole or is not native at all.** Concretely:

- **The artifact is one artifact, and it carries both payloads.** It carries the complete bytecode of
  every code unit exactly as it does now, and, beside it, the emitted code for **every** unit and a
  table saying which unit each emitted range belongs to. An artifact with no emitted code at all is
  the artifact this profile writes today, byte for byte.
- **The bytecode beside the machine code is not a fallback, and three things that are not fallbacks
  need it.** The differential oracle compiles one source to both forms and compares the transcripts;
  re-emission-equality verification recompiles the carried bytecode with the same deterministic
  backend and compares the result against the carried bytes; and a reader who cannot see the bytecode
  can check neither claim. **An artifact that dropped the bytecode would be asking to be trusted
  rather than read.** No executor path picks between the two sections, and one that did would be the
  second execution arm this profile's non-goals refuse.
- **There is no per-unit choice, so "one form per handle" holds literally rather than nearly.** A
  backend's two answers are *the whole artifact* and *a refusal naming a reason*; there is no third
  answer of the form "this unit yes, that unit no". **This is what a per-unit design could not have,
  and it is the whole reason the compilable language had to become small** — section 3.
- **The form is a property of the artifact and never of a moment.** Which form a program is compiled
  to is an input to the compile request, fixed when the artifact is written and pinned when it is
  verified. Nothing observes a running program and changes what runs it. This is the profile's own
  amended non-goal — one executor, one form per handle, no promotion.
- **There is no entry guard, because there is nothing for one to fall back to.** A native artifact's
  executor never reaches the interpreter; what it does when a machine cannot run the artifact's
  architecture is refuse to instantiate it, deterministically and by name, and that refusal is the
  only transfer of control this design has. *(This bullet said the opposite until 2026-09-07, when it
  described an entry-point check that failed to the interpreter at a unit's first instruction:
  [JSC-199](roadmap.corrections.md#jsc-199).)*

**The target is not a speed and no stage below is justified by one.** Throughput, baselines and the
measurement lane are `JS-10`'s subject in [section 19](roadmap.delivery.md#19-milestones), and
`JS-10` is `Not started` in the ledger. A native form is proposed here as a **capability**, on
exactly the terms the core states for itself: a record may say a native form exists and may not say
what it is worth. **Any figure about what a backend buys is a figure somebody has to collect**, in a
lane that does not exist, against a baseline that does not exist, and this document would be
untruthful the moment it implied otherwise.

---

## 2. The seam, named precisely: one lowering with a second exit

**The front end is one lowering with a second exit, not a second lowering, and the difference is a
rule rather than a preference.** The core's compilation table says it in the core's own words: a
bytecode backend and a machine-code backend are two exits from one front end, sharing the parse, the
static semantics and the analyses, and **a profile that forks its front end per target has written
the second lowering [section 10 of the core roadmap](../../../docs/roadmap.md) forbids** — the same
rule this profile already carries as its own *a second lowering* non-goal, which says that a
composition compiling at run time and one compiling ahead of time use one lowering assembly. A
target is not a reason to fork; it never was for hosts and it is not for architectures.

**Named precisely, the exit is the finished per-unit bytecode inside the assembly step.**
`JsCompiler.Compile` runs the shared tokenizer, then `JsParser.Parse`, then the per-program
compilation, and ends in `JsCompiler.Assemble`, which walks each `UnitBuffer`, calls `FinishScopes`,
relocates every branch site from unit-local to artifact-global, appends the bytes and records a
`JsFunctionRow`. **At that point, and only at that point, a unit is finished**: its code range is
known, its declared operand-stack height and scope-slot count are known, its exception regions are
closed, and its branch targets are absolute offsets rather than pending patches. A backend attaches
there, as an `internal` type beside `UnitBuffer` in the same assembly, consuming a code span, a
`JsFunctionRow` and the unit's closed regions, and answering with emitted bytes and an offset map or
with a refusal.

**It consumes bytecode, and that is the whole argument for where it sits.** The alternative — a
second walk of the `JsSyntax` tree — would duplicate every hoisting rule, every `Annex B` alias rule
and every completion-value rule across a body of mutually recursive compilation methods that carry
the lowering's ambient state between them, which is precisely the fork the rule above forbids. **The bytecode is
already the back-end-neutral form**: a stack machine with per-unit ranges, a declared maximum operand
height, declared slot counts, absolute branch targets, exception regions carrying entry depth *and*
entry height, and one shared table — `JsOpcodes.Shape`, `InstructionWidth`, `IsTerminal`,
`HasCodeTarget`, `TryDescribe` — that the verifier, an encoder and the interpreter would all read
rather than each re-deriving.

***And this profile's plan has said something else, in one sentence, since before there was a
backend to say it about.*** [Section 9](roadmap.md#9-the-semantic-front-end-and-lowering) says the
front-end contract returns a validated tree **or a back-end-neutral intermediate form**, and the
lowering consumes that. No such intermediate form exists: the abstract syntax tree is the only tree
and the bytecode is the only intermediate form. That sentence is not this document's to settle, and
[JSB-3](#jsb-3--the-backend-abstraction-and-where-the-form-is-chosen) states which of the two
readings a stage would have to fix before it emits anything — because a backend attached to a form
the plan promises and does not have is a backend attached to nothing.

**The seam is code as of 2026-09-07, and the signature it acquired carries the rule rather than
documenting it.** A backend receives the finished bytecode of one artifact — the whole code section,
the function rows tiling it, the exception regions, the constant pool **encoded rather than decoded**,
and the declared operand and slot maxima — and answers with the emitted bytes, the architecture, the
backend version, the alignment and a symbol row per unit, **or with a refusal naming a reason**.
**Those are the only two answers, and that is the rule of section 3 written into a method signature**:
an interface that could answer *this unit yes, that unit no* would be the per-unit choice the
non-goals refuse, and no amount of prose beside it would stop somebody using it. The pool arrives
encoded for the same reason: handing a backend an array of `double` would mean deciding at the seam
what a non-Number entry becomes, and every answer to that is a lie about a pool that has one.

**One more property of the seam is worth naming because it is what makes verification possible at
all.** The lowering is required to be deterministic — one source, one options value, one lowering
version, one artifact, byte for byte — and a backend extends that obligation over machine code,
where the iteration orders that matter are instruction selection and register allocation rather than
constant-pool ordering. **A backend that is not deterministic cannot be verified by re-emission**,
which section 6 below makes the only verification of emitted code that reaches the generator at all.
That is why [JSB-1](#jsb-1--determinism-over-bytecode-before-determinism-over-machine-code) comes
first and is about bytecode.

---

## 3. The constraint this whole document is organised around

**This profile's value is a struct that carries a managed reference, and emitted native code cannot
hold one.** `JsValue` is a readonly struct of a type tag, an IEEE-754 `double`, and an
`object? reference` — and that reference is traced by the CLR's collector, which finds it by knowing
the layout of every frame it walks. *(Its width in bytes is a fact of the checkout that this document
does not transcribe; update rule 10 is why, and `JsValue.cs` is where a reader reads it.)* A frame
the collector has never been told about is a frame it cannot scan and cannot update, and a moving
collector that relocates an object whose only live reference sits in such a frame has produced a
dangling pointer with no diagnostic anywhere near it.

**The core states this as a stop condition, and it points straight at this profile.** Its risk row
says that a profile which cannot state where its emitted code's references are rooted has not earned
the form, whatever its benchmarks say. This profile is the one that cannot, as things stand: every
operand on the interpreter's stack, every scope slot and every constant of a textual kind is a
`JsValue`, and half of what a `JsValue` may be is a reference.

**A SECOND RULE BOUNDS THE ANSWER, AND THE TWO TOGETHER EXCLUDE THE OBVIOUS DESIGN** *(this
subsection is new on 2026-09-07 and it is why the rest of the section changed:
[JSC-199](roadmap.corrections.md#jsc-199))*. The first rule is the rooting one above. The second is
this profile's own amended non-goal — **one executor, one form per handle, no promotion** — under
which a path that picks a form from run-time observation, or that re-maps a verified handle's
payload, is the second execution arm the paragraph refuses, and the core's risk row says the same
thing in its own words.

**So the obvious MVP is excluded, and by a published rule rather than by difficulty.** Compile the
hot numeric functions, guard on entry, fall back to the interpreter for everything else: a per-unit
guard that bails to the interpreter when an argument is not a Number **is deoptimization from a
compiled tier**, which the non-goals name and refuse by name. The weaker version — a per-unit
compile-time choice, with no run-time observation anywhere — is the same thing with the moment moved:
it leaves **one handle carrying two forms**, and the rule says one form per handle. Neither is
admissible without amending a record this MVP is not empowered to amend, and this document proposed
the first of them until the day it was read against the rule.

**The answer this roadmap proposes is therefore not a rooting scheme and not an eligibility test. It
is a manifest whose whole language has nothing to root.**

- **A third feature manifest, `broiler.javascript.numeric`, admits a numeric subset and refuses
  everything else at compile time, by name.** Number values, the arithmetic, comparison, bitwise and
  unary operators, `let`/`const`/`var` bindings of numbers, `if`/`while`/`for`/`do`, blocks, function
  declarations whose parameters and returns are numbers, direct calls by name, and `return`. No
  object, no string, no closure over a non-numeric value, no `this`, no exception, no generator, no
  property access, no dynamic anything. A construct outside it is refused with a diagnostic code and a
  source position, which is what
  [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted) already makes a
  first-class answer for a construct a front end can see.
- **A program in that manifest is compilable in whole, so the artifact is native or it is not.** Every
  code unit of a native artifact is emitted; none is declined; the executor for the native form never
  reaches the interpreter. That is what makes *one form per handle* a literal property of this design
  rather than a nearly-true one.
- **The emitted frame holds no managed reference at all**, and it holds none by construction rather
  than by an eligibility test somebody has to keep correct. The structure handed to an emitted entry
  point is unmanaged and fixed-layout — pointers to operand, local and constant storage, a fuel cell,
  and a helper table — and **the manifest admits no value that could put a reference in one**.
  Nothing in it needs reporting, and there is no scheme for the collector to get wrong because there
  is nothing for the collector to find.
- **No guard, no bailout, no deoptimization, no on-stack replacement, no tiering, and no map from an
  emitted offset back to a bytecode offset.** There is nothing to promote from and nothing to fall
  back to, so none of those mechanisms has anything to do. **The design is not that the bail-out is
  cheap; it is that there is no bail-out.**
- **And the differential oracle comes free with it.** The same source compiles under the same front
  end to a bytecode artifact and to a native artifact, both run, and the transcripts are compared —
  which is the compensating control the core roadmap names as the thing a native backend may not ship
  without, obtained by construction rather than by building a second harness.

**This answers the core's GC-rooting risk row by construction rather than by a scheme, and that is
the entire claim being made for it.** There is no write barrier to get wrong, no stack map to keep in
step with an optimiser, no suspend point to reason about, and no interaction with the transition the
runtime already performs when managed code calls unmanaged code. The property is checkable by reading
one struct declaration and one eligibility predicate, which is a far smaller thing to be right about
than a rooting scheme is.

**And the cost is real, is not a rounding error, and is paid at the front door rather than
discovered later.** **The compilable language is small and it is not JavaScript.** A native artifact
runs numeric kernels and nothing else: a program that touches a property, builds an array, holds a
string or catches an error is not compiled differently, it is **refused**, before any artifact
exists, with a diagnostic naming the construct and its position. Where the earlier design would have
declined such a program unit by unit and still produced a working artifact, this one produces no
artifact at all — **the cost moved from coverage inside an artifact to admission of the program**,
and it is the larger cost of the two. What the wide manifest gets is unchanged: it stays
bytecode-only, and the interpreter remains the only thing in this component that runs real
JavaScript.

**A subset is a real thing to have and it is not a general one**, and any record that lets the first
fact imply the second is the untruthful support claim both ledgers make a stop condition. The honest
sentence is that this design buys a demonstrable, verifiable native form over a narrow and precisely
stated **language**, and that widening it is not a later increment of the same work but a different
design with a rooting scheme in it, which nothing here proposes.

---

## 4. The routes this document takes without a decision

**A deferred decision is a decision nobody took, not a decision that went a particular way**, and the
MVP programme this component is running requires that where a route is taken in place of a decision,
the route is named where its consequence is rather than only in the record that defers it. Four are
taken above — three from the first writing and one added on 2026-09-07 when the design changed — and
each is recorded here as **taken without a decision**:

- **That the bytecode is the back-end-neutral form**, rather than building the intermediate form
  [section 9](roadmap.md#9-the-semantic-front-end-and-lowering) promises. The alternative is real work
  with a real argument behind it — an intermediate form would give a backend types, an explicit
  control-flow graph and a place to put an analysis, none of which the bytecode gives it. This
  document proposes the cheaper route because it is the one that does not fork the front end, and **no
  decision record has chosen between them.**
- **That the emitted body computes rather than delegates.** The alternative design emits one call per
  opcode into the managed helpers the interpreter's own switch arms already call, keeping the operand
  stack as it is and removing only dispatch and operand decoding. It compiles a far larger subset, it
  answers identically to the interpreter by construction rather than by testing — and every one of its
  frames holds managed references, which is the thing section 3 refuses. This document proposes the
  numeric route because it is the one that answers the rooting question. **No decision record has
  weighed the two**, and a reader should not read section 3's confidence as one having been taken.
- **That `arm64` is the second architecture and `x86-32` is not.** `x86-32` is the only callee-pops
  convention in the declared matrix and is exactly where the core's retained accident happened;
  `arm64` has a fixed-width encoding, one calling convention across its platforms, and a return
  instruction that takes no immediate, so that entire defect class is unrepresentable on it. Against
  that, `arm64` needs an instruction-cache maintenance sequence that has **no managed expression**
  and no dependable library export, which is why
  [JSB-9](#jsb-9--the-arm64-encoder-emitting-only) is **emitting-only** and why its execution half is
  **excluded rather than unscheduled**: what would close that exclusion is a maintenance path this
  component can name, call and test, and not a machine, a runner or a lane. **Neither exclusion
  is a decision anybody signed**, and the support consequence — which architectures are named, and
  what happens on a declared platform with no backend — belongs to whoever schedules the work.
  **The arm64 encoder now exists and the exclusion is unchanged by that**: it emits, its output is
  pinned against known-good encodings, and an image asked to instantiate an arm64 artifact refuses
  by name. Emitting is not arming, and nothing here claims otherwise.

- **That the native form is a WHOLE-ARTIFACT form under a restricted manifest**, rather than a
  mixed-form artifact with per-unit compilation and an interpreter fallback. This is the route the
  document took on 2026-09-07 when it stopped describing the other one, and it is the one a reader
  should weigh hardest: **the alternative is what a production engine would build**, it compiles far
  more of the language, and it is what the published rules of this repository currently forbid — one
  form per handle, no promotion, and the core's risk row making a per-unit fallback the same thing
  under another name. Whether those rules should be amended is a question for the deferred decision
  and not for this MVP. **It is [MVP-7](../../../docs/mvp.md#5-routes-taken-without-a-decision) in the
  route register**, with the alternative named there, and the consequence is here: the compilable
  language is small, it is not JavaScript, and a program outside it is refused rather than partly
  compiled.

**There is also a route this document explicitly does not take.** A managed compiled form — blocks
lowered to objects, or threaded through function pointers to statically compiled managed methods —
needs none of this: no native memory, no write-or-execute discipline, no per-architecture matrix, no
instruction-cache maintenance, and no answer to the rooting question, because the collector sees
everything. **It is also not a backend**, because it is built after verification from the bytecode and
is therefore an execution strategy rather than a payload, and the subject here is an artifact whose
payload is machine code. It is named because a reader comparing costs should know it exists, and
because the existence of this document is not an argument against it.

---

## 5. What this roadmap does not advance, and the mistake it would be

**Nothing below makes this profile compile more JavaScript, and conflating the two is how a component
ends up fast at the wrong language.** A backend takes bytecode the front end already produced; a
construct the front end refuses produces no bytecode, so there is nothing for a backend to be fast
at. **The two programmes divide by question and neither re-mints the other's stages**:

- The construct families still refused, and the surfaces still absent from the realm, are
  [the workload roadmap's](roadmap.workloads.md) —
  [`JSW-5`](roadmap.workloads.md#jsw-5--the-core-language-surface-still-refused-by-name) for the
  language surface refused by name,
  [`JSW-6`](roadmap.workloads.md#jsw-6--the-core-library-still-absent-from-the-realm) for the library
  still absent, and [`JSW-10`](roadmap.workloads.md#jsw-10--the-runs-per-manifest-whole) for the runs,
  per manifest, whole.
- The mechanisms inside the admitted surface are [the parity roadmap's](roadmap.parity.md) —
  [`JSP-2`](roadmap.parity.md#jsp-2--the-refusal-that-was-lost-a-bigint-literal-is-not-a-number) for
  the BigInt literal this front end currently consumes as a Number rather than refusing, and
  [`JSP-7`](roadmap.parity.md#jsp-7--the-surfaces-that-are-absent-without-being-declared) for the
  surfaces that stay out without being named.
- The three syntactic forms this front end refuses — a `using` declaration, a decorator, and an
  `accessor` class element — are catalogued in
  [parity section 4.3](roadmap.parity.md#43-the-types-and-surfaces-that-are-absent) and
  [workloads section 3.3](roadmap.workloads.md#33-the-syntax-that-is-refused-by-name), with the
  decorator refused by name and the other two refused as an unexpected token. **This document adds
  none of them and none of its stages moves one.**
- The regular-expression dialect, and everything else the absent Unicode character database blocks,
  is [`JSW-4`](roadmap.workloads.md#jsw-4--regular-expressions-over-the-from-scratch-matcher)'s, with
  the blocker outside this component and outside this document's reach.

**The mistake is specific and it is easy to make.** A backend is measured by running something, and
the something that is easy to run is the numeric kernel of a benchmark — so a programme that
optimises what it can already run will keep choosing work whose value is visible and will never
choose work that makes an absence stop being an absence. **Full-featured JavaScript is what those two
documents are about; this one is about what a second output form for the part that already compiles
would take**, and a record that let a reader read *faster* as *more complete* would be claiming a
capability no run has shown. Where this document needs to say what is still missing, it points at the
owner above rather than restating the gap.

---

## 6. The stages

Each stage states an objective, what it waits on, and an exit gate written the way
[section 19's](roadmap.delivery.md#19-milestones) gates are written — as conditions a run can decide,
not as work items. **None of them is scheduled and none has an owner**; assigning either is the act
that would turn a stage into a milestone with a ledger row, and the ledger carries no row that any of
them would move.

**Six of them now own code, and every one of them adds a State bullet on 2026-09-07 saying which
clauses of its own gate that code meets and which it does not** *(added with
[JSC-200](roadmap.corrections.md#jsc-200))*. Three rules govern what a State bullet may say, and they
are the reason the bullet is worth reading rather than skipping. **Owning code is not acceptance**:
no bundle has been retained for any stage below, no human has read a line, and a stage whose every
clause were met would still be a stage rather than an accepted milestone. **A gate clause is met or it
is not**, so a State bullet names the unmet clauses individually instead of summarising how far along
the stage is. And **the order in which the work actually landed is recorded even where it is not the
order this document set out** — two stages that were declared prerequisites were not done first, which
is stated in their own bullets rather than left for a reader to notice from a green suite.

### JSB-1 — Determinism over bytecode, before determinism over machine code

- **Objective.** The wide lowering is shown to be deterministic, and the one place its output order is
  not a function of source order is repaired. **Determinism has to hold for bytecode before it can be
  claimed over machine code**, and re-emission verification — the only check that reaches a code
  generator at all — is worthless without it.
- **Waits on.** Nothing. The front end and the corpora are in this checkout today.
- **Exit gate.** Every source in the wide and module corpora is compiled twice through
  `JsCompiler.Compile` and the artifacts compared byte for byte, in a check that runs where the slice
  front end's equivalent already runs — because the property the plan states is asserted today for one
  of the two front ends and not for the one that produces every artifact a backend would consume; the
  global lexical hoisting sites, whose emission order is a dictionary's enumeration order rather than
  the source's, emit in an order derived from the source, with a negative control that perturbs the
  order and is **watched failing and watched passing after revert**; and the corrections file carries
  an appended entry recording which half of the determinism claim was covered and which was not.

- **State on 2026-09-07: nothing here was built, and the backend was written over the front end this
  stage exists to pin.** The determinism assertion in the slice-compiler root's checks lane compiles
  the **slice** front end's sources twice and compares the bytes; there is no such assertion over the
  wide front end, and the wide front end is the one that lowers `broiler.javascript.numeric` and
  therefore the one that produced every artifact a backend has consumed. The global lexical hoisting
  order is untouched, the negative control does not exist, and the corrections file carries no entry
  about which half of the determinism claim is covered. **This stage's whole argument was that
  determinism has to hold for bytecode before it can be claimed over machine code, and the work went
  the other way round.** Re-emission equality is asserted today over a front end whose determinism is
  asserted for its sibling.

### JSB-2 — The enforcement, which is right whether or not a backend is ever written

- **Objective.** What this profile's records forbid and what its automated gates check say the same
  thing **before** any page in this family is mapped executable.
- **Waits on.** The core for the half it owns: rule B5's widening to the native-memory surface,
  matched by member and by platform-invoke target, its scope reaching profile assemblies rather than
  the core three, and the rule that reads the composition register's native-execution column — all
  of which belong to **the VM-7 next action that widens rule B5 to the mapping and arming surface
  and mints the rule that reads the register's native-execution column**, and are not this profile's
  to write. The column itself already exists, so what waits here is the rule and not the column, and
  the act is named rather than numbered so the reference cannot go stale on a renumber.
- **Exit gate.** This profile's own [release gate 2](roadmap.gates.md#22-release-gates) and the
  catch-all clause of [section 23](roadmap.gates.md#23-risks-and-stop-conditions) say what the core's
  now say — the core narrowed its prohibition from *dynamic code* to **undeclared** dynamic code and
  this profile's equivalents were not touched, so **a backend in this family would trip a stop
  condition this family publishes**, which is a defect in the records rather than in the design and is
  repaired here; the corrections file carries the entry that records the narrowing; and a witness
  naming a native-memory entry point is watched failing under the widened rule and watched passing
  after revert. **This stage improves this component if every other stage below is abandoned**, which
  is why it is not ordered behind them.

- **State on 2026-09-07: the half this stage waits on is written; the half it owns is not.** The core
  half landed with the work rather than ahead of it: rule B5's scope widened from the core assemblies
  to every assembly a published image can contain and its member list gained a native half; a second
  rule reads the platform-invoke declarations that no member reference names, permitting exactly one
  assembly of this family to declare a mapping or protection entry point; a third asserts that the
  arming path is the one place in the shipping tree that names such an API and that no protection it
  passes admits a write and an execute at once; and a fourth reads the composition register's
  native-execution column against the tree in **both** directions — a `none` over an image that can
  arm, and an architecture over an image that cannot. Each carries a witness. **Open clauses, and they
  are this family's own**: [release gate 2](roadmap.gates.md#22-release-gates) and the catch-all clause
  of [section 23](roadmap.gates.md#23-risks-and-stop-conditions) still say *dynamic code* unqualified
  where the core narrowed its own to **undeclared** dynamic code, so **this family still publishes a
  stop condition a backend in it would trip**, and the corrections entry recording that narrowing has
  not been written. **And the ordering this stage asked for was not the ordering taken**: it asked for
  the enforcement to be true *before* any page in this family was mapped, and enforcement and arming
  arrive in the same change. That is weaker than the stage asked for and it is not the failure the
  stage warned of, which was arming with no enforcement at all.

### JSB-3 — The backend abstraction, and where the form is chosen

- **Objective.** One lowering acquires a second exit, and the form is an input to a compile request
  rather than anything a running program can influence.
- **Waits on.** JSB-1, for the determinism the second exit inherits.
- **Exit gate.** A backend that compiles nothing — selected explicitly, refusing every unit — produces
  artifacts **byte-identical** to the ones this profile writes today, across the whole corpus, which
  is the only cheap proof that adding the second exit did not perturb the first; the form is read from
  the request and from nowhere else, with a case showing that two compilations of one source under two
  forms differ only in the sections the second adds; the new entry point is **inside** rule N19's
  compilation-stack pattern rather than named around it, because a rule dodged by a name is a rule
  made vacuous; rule N12 holds, so no scratch buffer, code cache or encoder table is a mutable static;
  rule N1 holds, so the profile assembly still does not reference the compiler and an execution-only
  composition still provably contains no lowering; and the intermediate-form sentence in
  [section 9](roadmap.md#9-the-semantic-front-end-and-lowering) is either corrected or discharged by a
  dated decision recording that the bytecode **is** that form, with the corrections file carrying
  whichever it was.

- **State on 2026-09-07: owns code, and the signature carries the rule.** The seam exists as a pair
  of records and an interface in the lowering assembly — the finished artifact in, the emitted bytes
  with their architecture, backend version, alignment and symbol rows out, **or a refusal naming a
  reason and nothing else**. The form and the backend name are inputs to the compile request and are
  read from nowhere else; the profile assembly still does not reference the lowering, so an
  execution-only composition still provably contains no code generator. **Open clauses**: no check
  shows that a backend selected and refusing every unit produces byte-identical artifacts across the
  whole corpus, because the design has no such backend — a refusal produces no artifact at all, which
  is a different property and is not the one this clause asks for and would have to be rewritten to
  ask for; and the intermediate-form sentence in
  [section 9](roadmap.md#9-the-semantic-front-end-and-lowering) is neither corrected nor discharged by
  a dated decision, so the plan still promises a back-end-neutral intermediate form this component
  does not have.

### JSB-4 — The artifact sections that carry emitted code and its symbols

- **Objective.** An artifact can carry emitted code and a table naming what each range is, without a
  second format version, a second verifier, or a second artifact.
- **Waits on.** JSB-3.
- **Exit gate.** Two section kinds are taken after the last kind the format defines today — one for
  the emitted bytes with their target architecture, their backend version and their alignment, one for
  the rows mapping a code unit to a range inside them — with the verifier's kind bound and its
  dispatch extended in the same change; the ordinary code section is still written in full for every
  unit — **not as a fallback, which this
  design does not have, but because the oracle of
  [JSB-10](#jsb-10--the-differential-oracle-one-source-two-forms) compares the two forms of one source
  and JSB-5's re-emission layer recompiles the carried bytecode to check the carried bytes**, and a
  reader who cannot see the bytecode can check neither *(this clause named an interpreter fallback
  until 2026-09-07: [JSC-199](roadmap.corrections.md#jsc-199))*; a manifest identity for
  the native surface joins the declared-surface table, so **a composition that declines it refuses an
  artifact naming it, at verification, with an invalid-artifact reason** rather than at run time, which
  is the property [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)
  describes and which this stage is the second chance to get right; rule N16 holds, so one verifier
  object reads every format version rather than a second being minted; and every refusal this stage
  adds maps to exactly one core reason and is reachable from a named corpus case, as the diagnostic
  rules already require of every refusal this profile publishes.

- **State on 2026-09-07: owns code, and most of this gate is code rather than evidence.** Two section
  kinds are taken after the last kind the format defined — one carrying the architecture, the backend
  version, the alignment and the emitted bytes, one carrying a symbol row per code unit — with the
  verifier's kind bound and its dispatch extended in the same change, no second format version and no
  second verifier. The ordinary code section is still written in full for every unit. A manifest
  identity for the native surface joined the declared-surface table, and a composition that declines
  it refuses an artifact naming it **at verification**, with an invalid-artifact reason, from a named
  corpus case. **Open clauses**: nothing is retained as a bundle, so every clause above is a fact about
  the checkout and not a demonstration.

### JSB-5 — Verification of a payload that is code, and what it cannot catch

- **Objective.** The verifier's answer for an artifact carrying emitted code is pinned by a retained
  corpus, and the limit of that answer is stated by this component rather than discovered by a
  consumer.
- **Waits on.** JSB-4.
- **Exit gate.** Three layers, each with cases of its own. **Structural, always**: section framing,
  emitted length within a declared ceiling, alignment, every symbol row's range strictly inside the
  emitted bytes and non-overlapping and ascending, the architecture a value this build knows, the
  backend version the one this build emits. **Re-emission equality, where a backend is in the image**:
  the artifact's own bytecode is recompiled by the same deterministic backend and the result compared
  byte for byte with what the artifact carries — the only layer that reaches the *generator* rather
  than the *payload*, and the one JSB-1 is what makes possible. **Template-closure scan, where no
  backend is in the image**: the emitted stream is decoded against the same fixed template table a
  backend emits from, and every byte must belong to a template instantiation with in-range operands.
  And the clause that keeps the record honest: **the third layer would not have caught the accident
  VM-7 retains as a fixture** — a wrong calling convention that returned the correct answer every time
  and leaked stack until the process died millions of calls later, with no exception and nothing a
  verifier of the artifact could see. A well-formed artifact from a wrong generator is well formed. So
  the bundle states, in its own words, that an execution-only composition's trust in an emitted payload
  rests on **provenance and not on verification**, names re-emission equality as the only layer that
  says otherwise, and names what would pin the generator instead — which is JSB-8 and JSB-10 and
  nothing in this stage. The corpus is retained in the same form as the one VM-2 retains: each entry
  with its hash, its expected outcome, its reason and its diagnostic code, replayed by this profile's
  own corpus check.

- **State on 2026-09-07: two of the three layers exist and the third does not, which is the layer an
  execution-only image would have needed.** The **structural** layer is code: section framing, a
  declared ceiling on emitted length, alignment, every symbol row strictly inside the emitted bytes
  and non-overlapping and ascending, the architecture a value this build knows, the backend version
  the one this build emits. The **re-emission-equality** layer is code and is reachable through a
  descriptor a compiler-bearing composition selects, which recompiles the artifact's own bytecode with
  the same deterministic backend and compares the result byte for byte. **The template-closure scan
  does not exist**, so the sentence this stage owes a reader is owed plainly and now: **an
  execution-only composition's trust in an emitted payload rests on provenance and on nothing this
  build can check**, re-emission equality is the only layer that says otherwise, and it is available
  only to an image that carries the lowering. **Open clauses**: the third layer; and the retained
  corpus, which has entries with hashes and recorded triples for the native surface in this profile's
  existing corpus but no replay under three publish modes and no bundle.

### JSB-6 — The x86-64 encoder, and a frame that holds no managed reference

- **Objective.** One architecture emits, for the eligibility test section 3 fixes, with no register
  allocator and no managed reference anywhere in an emitted frame.
- **Waits on.** JSB-4 for somewhere to put the bytes, JSB-5 for something to check them, and JSB-2 for
  the enforcement to be true before any of it runs.
- **Exit gate** *(its first two clauses were replaced on 2026-09-07, when the per-unit design they
  belonged to was: [JSC-199](roadmap.corrections.md#jsc-199); they asked for an eligibility predicate
  over an opcode allowlist and for an entry guard that failed to the interpreter, and this design has
  neither)*. **Every unit of the artifact is emitted or the artifact is refused whole**, with cases on
  both sides of the line — a program the manifest admits, and a program refused at compile time with a
  diagnostic code and a source position naming the construct; the emitted frame's declaration contains
  no reference type, asserted by a scan rather than by reading; **branches are emitted in the wide
  displacement form only**, with a case covering a branch whose target is patched late, because a short
  form that overflows on a late patch is a silent miscompile and relaxation is precisely the
  optimisation that produces the defect class VM-7 retains; the encoder is a pure function of its
  input, emitting identical bytes for identical bytecode across processes and machines of one
  architecture; and every artifact the stage produces satisfies JSB-5's re-emission layer.

- **State on 2026-09-07: owns code, emits, and executes.** One architecture emits for two calling
  conventions, with no register allocator; the frame handed to emitted code is an unmanaged
  fixed-layout structure declaring no reference type; branches are emitted in the wide displacement
  form only, and the encoder says in its own text that the short forms exist, are shorter, and are not
  written here because a relaxation pass that shortened a branch and then overflowed a late patch is a
  silent miscompile. A kernel compiled to the native form and the same source compiled to bytecode
  answer identically through the whole core lifecycle. **Two clauses changed rather than closed**: the
  eligibility predicate over an opcode allowlist does not exist and cannot, because the manifest
  admits no ineligible construct in the first place — what replaces it is the admission pass, which
  refuses a construct at compile time with a diagnostic and a position; and the entry-guard clause and
  its cases are **excluded by the design rather than unmet**, since there is no guard and nothing to
  fall back to. **Open clauses**: no bundle, and the cross-machine half of the determinism obligation —
  identical bytes across processes and machines of one architecture — has been observed on one machine
  only.

### JSB-7 — The arming path, one place, with a negative control

- **Objective.** Pages are written and pages are executed and never both at once, as a property of the
  mechanism rather than a habit of its callers.
- **Waits on.** JSB-2 absolutely — a page mapped before the enforcement lands is a page mapped while
  the component publishes a prohibition it does not check.
- **Exit gate.** Exactly one type in this family names the platform's mapping and protection calls,
  asserted by a source scan in the idiom of the rule that already asserts a single construction site
  for a type in this family; that type is a state machine — allocated writable, written, armed
  executable, never writable again, released — where a write after arming and an entry before arming
  each throw rather than being prevented by convention; **the write-and-execute protection constants
  appear nowhere in the tree**, with a witness file naming one and watched failing, which is the
  negative control the core's own gate asks for; arming completes **inside verification**, before a
  shareable handle is minted, because the verified state's contract requires everything reachable from
  it to be immutable and safe for unsynchronised concurrent readers and a writable page is neither;
  the platform split is runtime dispatch or separately compiled files and **never a preprocessor
  directive**, which rule J6 forbids outright; and the arming type carries the falsification line rule
  J10 requires of anything assessed at the top of the security scale, because it is that by
  construction.

- **State on 2026-09-07: owns code, and it landed before the enforcement it says it waits on
  absolutely.** Exactly one type in this family names the platform's mapping and protection calls,
  in one file and two platform halves split by a run-time test rather than by a preprocessor
  directive; it is a state machine — mapped writable, written, armed readable-and-executable, never
  writable again, released — where a write after arming and an entry pointer taken before it each
  throw; the two protection values that would make a page writable and executable at once are written
  nowhere in this repository; and the type carries the falsification line rule J10 requires of
  anything assessed at the top of the security scale. **The single-site property is asserted by a rule
  and not only by reading**, with a witness naming a second place that maps memory and a witness
  arming a page readable-writable-and-executable, each there to be watched failing. **Open clauses.**
  **Arming does not complete inside verification**: a mapping is made per instance when a handle is
  instantiated, not before the handle is minted. That answers what the clause was protecting — a
  writable page is never reachable from the shared verified handle, because it is never reachable from
  the handle at all — by a different route than the clause names, so the property holds and **the
  clause as written is unmet**. The witnesses exist and **no bundle records them watched failing and
  watched passing after revert**, which is what this component's evidence contract asks of a negative
  control and what a green suite does not supply. And the waits-on was met in the same change rather
  than ahead of it, which JSB-2's own bullet records.

### JSB-8 — The ABI obligations, as a table with one negative control per row

- **Objective.** Every calling-convention obligation an emitted entry point owes is a row somebody can
  read, with an injection that violates it and a fast deterministic test that fails.
- **Waits on.** JSB-6.
- **Exit gate.** The table below exists in this repository as cases rather than as prose, each row
  **watched failing under its injection and watched passing after revert**, and each test fast and
  deterministic rather than a soak — because the whole lesson of the accident VM-7 retains is that a
  delayed death must be forced to arrive early. A row without a control is a habit with a table around
  it.

| Obligation | Why it is a row rather than a habit | The injection | What the test observes |
|---|---|---|---|
| The frame pointer arrives in the register this platform's convention names | The two conventions in the declared matrix disagree about which register that is, and each is plausible on the other's platform | Emit the other platform's argument register | The entry point reads a frame that is not its own, and the answer is wrong rather than absent |
| The stack pointer is identical immediately before and immediately after the call | This is the retained accident's row: a disagreement about who pops leaks per call and kills the process nowhere near its cause | Emit a return that adjusts the stack | The pointer differs across a single call, before any leak has accumulated |
| A long run of calls neither exhausts the stack nor moves the stack pointer | A leak of a few bytes per call is invisible in one call and fatal in a loop, and one call is what a unit test writes | The same injection as the row above | The run completes with a stable pointer, rather than the process dying much later for a reason nobody traces back |
| Every register the convention makes callee-saved is unchanged across the call | The two conventions disagree about two of them, which is the single most likely source of silent corruption | Clobber one of the two disputed registers | A trampoline that spills and compares reports the register that changed |
| The stack is aligned as the convention requires at the call instruction | A misaligned call is correct until the first callee that uses an alignment-sensitive instruction | Emit a prologue that leaves it misaligned | A helper requiring alignment faults deterministically rather than occasionally |
| The caller-reserved area this platform's convention requires is reserved before any call | Omitting it corrupts the caller's own frame, and only when the callee writes to it | Emit a prologue that omits it | The caller's locals differ across a call that should not have touched them |
| The result is returned in the register the convention names, for the integer and the floating-point case alike | A result read from the wrong register is a plausible wrong number rather than a failure | Return the floating-point result in the integer register | The value is wrong while the call itself succeeds |

- **State on 2026-09-07: owns code, and the table above is checks rather than prose for all but one
  of its rows.** The obligations run in the slice-compiler root's checks lane, each conforming case
  paired with an injection asserted to be caught rather than merely asserted to exist, and each fast
  and deterministic rather than a soak — the stack pointer is compared immediately either side of one
  call and then across a long run, which is the delayed death forced to arrive early. The trampoline
  keeps its own frame in a callee-saved register precisely so that the caller-pops injection can be
  survived rather than crashed on. **Open clauses**: the last row of the table — the result returned
  in the register the convention names, for the integer and the floating-point case alike — has **no
  row of its own and no injection**; the property is covered incidentally by the differential lane,
  where a result read from the wrong register would surface as a wrong value, and covering a row
  incidentally is what this stage was written to stop. And no bundle retains any of it.

### JSB-9 — The arm64 encoder, emitting-only

- **Objective.** A second architecture is **emitting-only** — emitted and pinned, and not executed.
- **Waits on.** JSB-6 and JSB-8, whose obligations it inherits before it inherits anything else.
- **Exit gate.** Emission is byte-identical for identical input, into the same sections JSB-4 defines,
  with the target architecture distinguishing it; the encoder range-checks every branch displacement
  and **refuses rather than truncating**, which a fixed-width encoding makes a bounded and checkable
  obligation; the emitted bytes are pinned by a retained corpus and checked against a disassembler
  outside this repository as an explicitly external step the bundle names as external; **the support
  table names this backend `emitting-only`, in that word, and claims no runtime identifier for it**,
  which is what the core's [release gate 11](../../../docs/roadmap.md#15-release-gates) asks an
  emitting profile to make possible — an emitting-only backend names none of the RIDs rather than
  carrying a publish-and-run obligation it could not discharge; and **no arm64 platform is claimed
  anywhere**. The execution half is **excluded rather than unscheduled**, and the reason is not a
  shortage of machines: the architecturally required instruction-cache maintenance sequence has no
  managed expression and no dependable library export, so an image that armed a page and jumped to it
  on that architecture would be relying on behaviour it cannot state. Until that has a route — a
  maintenance path this component can name, call and test, and not a runner and not a lane — a
  platform with no backend **refuses an artifact carrying emitted code deterministically, with a
  diagnostic of its own**, rather than being silently absent — which is the support property the
  core's own gate asks for and which this stage owes even though it executes nothing.

- **State on 2026-09-07: owns code, emits, and has run nowhere.** A second architecture emits into
  the same sections, distinguished by its target architecture, with the emitted bytes pinned by golden
  rows in the same checks lane; an image asked to instantiate an arm64 artifact refuses by name with an
  unsatisfied-host-assumption contract violation rather than being silently absent, on every machine,
  including an arm64 one. **The word is carried**: the support table names this backend
  **emitting-only** and claims no runtime identifier for it. **Open clauses**: the golden bytes are
  checked against expectations written in this repository and **not** against a disassembler outside
  it, which is the external step this stage asks for by name and the bundle would have to record as
  external; the branch-displacement range check refusing rather than truncating is written and is not
  pinned by a case; and no bundle retains any of it. **The execution half is excluded and remains
  excluded**, on the reason it always carried and not on a shortage of machines.

### JSB-10 — The differential oracle: one source, two forms

- **Objective.** The two forms of this engine are compared against each other over the same source
  lowered by the same front end, so that a wrong generator has something that can catch it.
- **Waits on.** JSB-6.
- **Exit gate.** The differential driver gains a lane that compiles each probe **twice from one
  source** — once to bytecode, once with the backend selected — and diffs the two transcripts against
  each other and against the retained answers; every eligible unit in the corpus is covered by at least
  one probe that actually enters it, asserted rather than assumed, since a lane that compares two runs
  of the interpreter has compared nothing; **values are compared and budget outcomes are not**, because
  an emitted body cannot charge per instruction the way the dispatch loop does and its accounting
  granularity necessarily differs — so the resource-exhaustion dimensions rule N11's retained corpus
  pins per dimension will legitimately disagree across forms, and the bundle states that consequence
  rather than letting a lane go red for the right reason; and a negative control corrupts one emitted
  instruction and is watched failing. **Without this lane a backend is an unfalsifiable claim**, which
  is the core's own words for it, and no stage above closes that on its own.

- **State on 2026-09-07: owns code, over a handful of named probes rather than over the corpus.** A
  lane compiles each probe twice from one source — once to bytecode, once with a backend selected —
  drives both through the whole core lifecycle, and compares what each answered; values are compared
  and budget outcomes are not, for the reason this stage gives, because an emitted body charges the
  meter once for what a counter says was spent and the dispatch loop charges per instruction.
  **Open clauses**: the probes are a small named set written beside the lane rather than the corpus,
  and **nothing asserts that every code unit is entered by at least one of them**, which is the clause
  this stage calls the difference between a lane that compares two runs and a lane that has compared
  something; the negative control that corrupts one emitted instruction and is watched failing does
  not exist; and no bundle retains the lane.

---

## 7. Order, and what is schedulable today

**Two stages need nothing that does not exist, and they are the two that were not done.** JSB-1 is a
determinism check and one iteration order over a front end that is in this checkout today. JSB-2 is
records and enforcement, and its profile half — this family's own release gate and stop condition
still forbidding *dynamic code* unqualified where the core narrowed it to *undeclared* — is a defect
in this family's records that is worth repairing whether or not a backend is ever written.

**JSB-2 comes first among the rest, and not because it is small.** A profile in this repository that
mapped a page executable would pass every automated gate this component has while tripping a stop
condition this component publishes. That is a hole which predates every stage here and is created by
none of them, and a programme that emitted code before closing it would be building on an enforcement
gap it had itself written down.

***That paragraph was written in the conditional and the conditional no longer holds, which is worth
a sentence rather than a silent edit*** *(recorded 2026-09-07)*. A profile in this repository **does**
map a page executable. It does not pass every automated gate while doing so: the core's half of JSB-2
is written, so a rule now reads the platform-invoke declarations, a rule pins the single arming site
and its protections, and a rule holds the composition register's native-execution column to the tree.
**What is still true is the profile half** — this family's own release gate and stop condition, which
forbid *dynamic code* unqualified and which a backend in this family therefore trips.

**And JSB-1 was skipped outright, which no rule catches and nothing measured.** The two stages that
needed nothing that did not exist were the two most likely to be passed over, and one of them was:
the determinism this programme puts first is asserted for the slice front end and not for the front
end that lowers every artifact a backend has consumed. **An ordering with no gate behind it is a
preference**, and this is what that costs.

**The rest are ordered by what they unblock rather than by size.** JSB-3 needs JSB-1's determinism;
JSB-4 needs JSB-3's exit to have something to write; JSB-5 needs JSB-4's sections to have something to
check; JSB-6 needs all three and JSB-2 besides; JSB-8 and JSB-10 close over JSB-6 from two directions
— one pins the interface, the other pins the answers — and neither substitutes for the other. JSB-9 is
last because it inherits every obligation above it and adds a platform question none of them has.

**One dependency sits outside this component**, and it is the one VM-7 already carries: rule B5's
widening and the composition register's declaration column are the core's to write, and the core's own
record says the amendment procedure governing part of that ground is **unexecutable** while one person
holds the minting role and both co-signing roles. No sequencing inside this document moves it.

**And nothing here waits on the snapshot.** `JS-2` is `Blocked` in the ledger and stays blocked; the
argument that this does not block work written from a specification rather than ingested is
[JSC-15](roadmap.corrections.md)'s, and it applies here as it applied before — a backend reads
bytecode this checkout already produces.

---

## 8. What this roadmap does not promise

Each of these is a rule rather than a scheduling note, and a stage that needed one relaxed would be a
different programme rather than a later one:

- **No tiering.** An artifact is bytecode, or it carries emitted code, chosen when it is compiled and
  fixed when it is verified. There is no second tier for anything to reach.
- **No promotion, no deoptimization, and no on-stack replacement.** Nothing observes a running program
  and compiles it; nothing abandons emitted code mid-body and rebuilds an interpreter frame; nothing
  resumes a body in the other form at an instruction boundary. **And from 2026-09-07 there is no
  transfer between forms at all** — this bullet ended by naming the entry guard of section 3 as the one
  transfer, and the guard does not exist, because whole-artifact compilation leaves nothing on either
  side of it to transfer to ([JSC-199](roadmap.corrections.md#jsc-199)). What a machine that cannot run
  an artifact's architecture does is refuse to instantiate it.
- **No run-time form selection.** A path that picks a form from run-time observation, or that re-maps
  a verified handle's payload, is the second execution arm this profile's non-goals refuse, and it is
  refused here in the same words.
- **No register allocator in the MVP.** The abstract operand stack is the allocation. Register
  allocation is where the determinism obligation bites hardest, which makes it exactly the thing to
  leave out of a first backend rather than the thing to be clever about.
- **No managed reference in an emitted frame, ever, under any manifest.** Widening the compiled
  language past what section 3 admits is a different design with a rooting scheme in it, and this
  document proposes no rooting scheme. **The manifest is what holds this**, not an eligibility test
  over an opcode stream, so widening it is a change to a published language surface and not a
  loosening of a predicate somebody could relax quietly.
- **No `x86-32`.** It is the only callee-pops convention in the declared matrix and the exact source of
  the accident the core retains as a fixture. Excluding it is a rule here, not a backlog item.
- **No claim about speed, and this is the promise the built form puts under the most pressure.** No
  stage is justified by a measurement and none of them produces one. A native form that runs invites a
  number the moment anybody runs it twice, and **an observation of that kind has been made and is
  recorded nowhere**: one machine, once, no A/A lane, no predeclared rule, no repetitions, no benchmark
  host, so measurement rule L1 does not bind it and the baseline register does not carry it. **A record
  may say a native form exists and was observed to answer what the interpreter answered; it may not say
  what it is worth.**
  `JS-10` owns baselines and is `Not started`; a native form is proposed as a capability, and a record
  may say a form exists without saying what it is worth.
- **It does not advance the language, and from 2026-09-07 it has narrowed one.** Section 5 says whose
  the remaining coverage is; no stage here admits a construct, fills an absence, or widens a manifest.
  **What it did do is mint a third manifest that is smaller than both existing ones**, and a reader must
  not read a new manifest identity as new language: `broiler.javascript.numeric` admits strictly less
  than `broiler.javascript.slice` admits of anything but numbers, and a program the wide manifest runs
  today is a program the numeric one refuses.
- **And it does not accept anything.** No stage moves a ledger row, and a stage's exit gate being met
  is not acceptance: acceptance needs an owner and a reviewer decision, which nothing in this component
  has.

---

## 9. What a stage would owe if it were scheduled

Nothing in this document changes the obligations any work in this component already carries, and they
are restated here only because a stage list invites the reading that a plan replaces them:

- A retained bundle per [section 4's](roadmap.status.md#4-required-evidence-bundle) nine fields,
  collected by this profile's own script, with its failures and exclusions retained rather than
  summarised — and, for anything on this list, with the negative-controls field carrying more than one
  entry, because every stage above is defined by what it can be seen to catch.
- A registered rule in the architecture rule register for every new mechanism, with a witness whose
  file name starts with the rule identifier, and negative controls that have been **watched failing and
  watched passing after revert** — a control nobody has seen fail is worth nothing, and a control that
  makes the process crash has judged nothing.
- The two-line assurance annotation on every new declaration, the falsification line on anything
  assessed at the top of the security scale, and no preprocessor directive in any covered source.
- Appended entries in [the corrections file](roadmap.corrections.md), never edits to old ones, with the
  plan pointing at them by bare marker.
- Ledger rows moved in the same change that changes what they claim, and no count, total or score
  copied into prose anywhere.
