<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0025 - The baseline native form over the wide manifest, and where its references are rooted

**Status:** Taken. 2026-09-15.

**Owner:** MaiRat, as this profile's owner. **Co-signer:** MaiRat, as the core's contract and
security owner, which are the roles [MVP-7](../../../../docs/mvp.md#5-routes-taken-without-a-decision)
names as the deciders beside this profile's owner. **Both roles are held by one person**, and this
record does not claim the co-signature is independent - there is no second signature to claim it of.

**Milestone:** none in the `JS-` series, and that is stated rather than filled in. The work this
decides is stage [JSB-11](../roadmap.backends.md#jsb-11--the-baseline-form-over-the-wide-manifest-and-a-frame-that-still-holds-no-managed-reference)
of [the backend roadmap](../roadmap.backends.md), a proposal-document stage with no owner in the
milestone sense and no milestone row in the ledger; the ledger records its work only as observed
repository state, in three rows *(corrected 2026-09-15, after collection: this read "in three `[NONE]`
rows"; they are `[PARTIAL]` since bundle JSB-11-001 was retained, which is not acceptance:
[JSC-223](../roadmap.corrections.md#jsc-223))*. The index names that stage because it is where the exit
gate this record depends on is written down, not because a stage is a milestone.

**Context.** On 2026-09-15 the repository owner asked that the test262 run in the machine-code output
profile pass as many tests as the run in the bytecode profile. Recorded as an instruction, because
everything below is an argument about what it reaches. **The native form as it stood could not be
brought near that by any amount of work on its own terms.** It was admitted only under
`broiler.javascript.numeric`, a manifest that refuses at compile time, by name, every program that
touches a property, holds a string, builds an array or catches an error - so almost every variant of
the suite is refused before an artifact exists, and the route register's own row says why that is
not a later increment: **"Widening the manifest is a different design with a rooting scheme in it and
not a later increment of this one."** [MVP-7](../../../../docs/mvp.md#5-routes-taken-without-a-decision)
names what would settle the route: an amendment to this profile's non-goals, or a recorded refusal of
one. This record is the amendment's authority, and it takes a different design than the one that
sentence expected - one with no rooting scheme in emitted code at all.

---

## 1. What the numeric form reaches, and why the instruction is not a request to widen it

**The numeric form answers the rooting question by admitting no value that holds a reference, and
that is the whole reason its language is small.** Its emitted code computes: operands, locals and
constants are slabs of `double`, the frame is `JsNativeFrame`, and a construct whose value could carry
a managed reference is refused at compile time rather than represented at run time. None of that is
wrong and none of it changes here. **What it cannot do is become the wide manifest's form by growing.**
`JsValue` is a tag, a double and an object reference, and a computing form over it would put that
reference into emitted frames, which is what the core's risk row stops and what
[JSD-0011](0011-the-value-frame-and-call-abi.md)'s Row 2 refuses by name.

**So the question the instruction actually asks is a different one**: whether a native form exists
for the wide manifest whose emitted code holds no managed reference, runs every program the wide
manifest runs, and answers what the interpreter answers. The design below is that form, and it is
called the **baseline** form to keep it apart from the numeric one, which keeps its name, its
templates, its frame, its executor and its answers.

## 2. The decision

**Every code unit of an artifact compiled under `broiler.javascript.wide` with the native output form
is emitted as x86-64 machine code, and every instruction of it is one call into the interpreter's own
dispatch for that one instruction.**

- **The granularity is taken, not weighed.** One call per instruction is what this record decides the
  form does, and no measurement weighed it against one call per run of instructions; section 3 names
  that option and [MVP-8](../../../../docs/mvp.md#5-routes-taken-without-a-decision) records the route.
- **Control flow between instructions is emitted; the semantics of each instruction is not.** Each
  bytecode instruction gets a label. An opcode for which `JsOpcodes.HasCodeTarget` holds branches
  natively to its static target, and a compare tree over each unit's statically known landing offsets
  - its entry, its exception-region handlers, and the resume points of `Yield`, `Await`,
  `YieldDelegate` and `EnterBody` - dispatches to any offset a handler answers that is not the
  fall-through. An offset the tree does not know answers `Defect`.
- **Each instruction is `call qword [rbx + opcode*8]`**, where RBX holds the address of a handler
  table that arrives through the frame. The slot is the opcode byte itself, so there is no slot
  numbering to drift from the opcode table. Each slot of a defined opcode holds an
  `[UnmanagedCallersOnly]` wrapper around an instantiation of `JsEngine.ExecuteCore<TMode>` - the
  interpreter's own method body, specialised over a struct mode - which runs exactly one instruction:
  the same `current = pc`, the same `Charge`, the same arm text, the same catch filter and finally
  search, and the same landing. Every other slot holds a wrapper that answers `Defect` and touches
  nothing.
- **A handler answers the next bytecode offset or a negative status**, and the two cannot be confused
  because an offset is never negative: `JsBaselineStatus.Exit` (a return or a suspension, whose value
  stays in the managed activation), `Threw` (an exception escaped the unit's own regions, and the
  activation holds it for the caller to raise), and `Defect`. A unit answers only a status.
- **A handler refuses unless three things hold**: the offset it is handed equals the offset the
  managed side computed, the byte at that offset is the handler's own opcode, and the frame's cookie
  is this activation's. A wrong native decision therefore becomes `InternalDefect` and never a
  different JavaScript answer. **That is what the design leans on instead of proving placement**: the
  template-closure scan, with the shape clauses below, can prove that every call lands in the table, and cannot prove which handler
  belongs at which offset, and the wrapper closes that gap at run time.
- **The frame is `JsBaselineFrame`**, declared in the format assembly, which the lowering and the
  profile both already reference, because the handlers are in the profile, which may not reference
  its lowering (rule N1), and the emitter is in the lowering, which does not reference the profile:
  the handler table's address at offset 0 and a 64-bit activation cookie at offset 8, and nothing
  else. `JsBaselineAbi` restates the
  offsets and the per-convention stack reservation once.
- **The handler table lives in unmanaged process memory**, allocated once by `JsBaselineHandlers`'s
  static constructor, filled with one function pointer per slot, self-checked - every defined opcode
  byte has its own wrapper and every other byte the defect wrapper - and never freed. **If the
  self-check fails the table's address is zero, and every instantiation of a wide native artifact
  answers `UnsatisfiedHostAssumption`** rather than running.
- **The template tables for the baseline form are separate from the numeric ones and are selected by
  the artifact's manifest**, through `JsNativeTier`, not by a field the artifact carries: a second
  field naming the same fact would be a second place for the two to disagree. The numeric tables, the
  numeric golden bytes, `JsNativeExecution` and every numeric answer are untouched.
- **Shape clauses S1 to S4 hold for the baseline tables.** (S1) A unit's first six instantiations are
  exactly the prologue templates, in order. (S2) Those templates occur nowhere else in the unit, the
  unit's last four instantiations are exactly the epilogue templates, and those occur nowhere else,
  so every unit has one prologue and one epilogue. (S3) No unit-local branch targets the prologue, a
  `pop` of the epilogue or its `ret`. (S4) The baseline tables contain no template branching to
  another unit's entry and no template with a memory destination. A payload violating any of them is
  refused under the existing `NativePayloadNotTemplateClosed` code, with two new named outcomes.
- **The form is whole-artifact, and one form holds per handle and per instance, nested loads
  included.** An engine is constructed for one form; a program a guest loads - `eval`, the `Function`
  constructor, a dynamic or static import - whose form or architecture differs from its instance's is
  an internal defect, not a fallback. **There is no per-unit choice, no entry guard, no promotion, no
  bail-out and no interpreter fallback**: the managed code a handler runs is the interpreter's method
  body for one instruction, reached only from emitted code, and nothing chooses between that and the
  interpreter's own loop at run time. The one refusal the form adds at compile time is an artifact
  whose emitted code would exceed the format's existing native-code ceiling, refused whole.
- **Code pages stay W^X exactly as they are**, mapped writable, written, armed readable-and-executable,
  never writable again, and the arming path stays in the one type and the three files rule X1 already
  pins.

## 3. What was considered

**The comparison the backend roadmap said nobody had made comes first, because every row of the
table below depends on it.** [Section 4](../roadmap.backends.md#4-the-routes-this-document-takes-without-a-decision)
of that document took the computing form without a decision and described the other as emitting "one
call per opcode into the managed helpers the interpreter's own switch arms already call", and closed:
"**No decision record has weighed the two.**" This one does.

- **The computing form** answers the rooting question by admitting no reference; it is the only one of
  the two whose emitted code does the arithmetic itself; it reaches a numeric subset and nothing more;
  its fuel is charged at a granularity of its own, so it exhausts at different points from the
  interpreter; and its answers agree with the interpreter's by testing, not by construction.
- **The delegating form** answers the rooting question by never handing a reference to emitted code at
  all; it reaches every program the wide manifest runs, because it has one lowering per opcode class
  and no type facts to be wrong about; its answers agree with the interpreter's by construction,
  because every guest-observable effect happens inside the interpreter's own method body; and its fuel
  is charged per instruction at the interpreter's own point. What it costs is section 6.
- **The sentence that made the delegating form inadmissible was a claim about one way of building it,
  and it is not a property of delegation.** Section 4 said "every one of its frames holds managed
  references". A design that passes emitted code an operand stack, a scope list or a value to hand to
  a helper does exactly that. **This one passes a frame pointer and an offset, and nothing else**: the
  operand stack, the scopes and every value stay in managed memory that only managed code touches, so
  the emitted frames hold no reference. The correction is [JSC-217](../roadmap.corrections.md#jsc-217).
- **So the two are not rivals for one manifest.** The instruction asks for reach, and reach is the
  delegating form's; the numeric manifest keeps the computing form it already has. Which form an
  artifact carries is fixed by the manifest it was compiled under.

| Option | What it does | Why it was not taken |
|---|---|---|
| **Widen the computing form with a rooting scheme** | Emitted code holds `JsValue`s, and a scheme - reported stack maps, pinned handles or a handle table - tells the collector where their references are | It is the design section 3 of the backend roadmap and MVP-7 both call different, and it collides with [JSD-0011](0011-the-value-frame-and-call-abi.md)'s Row 2, which decides "no manual rooting, no handle table, no finalizer, no `GCHandle`" for every value-holding region and says why: a rooting bug in a hand-written scheme is not a bug a corpus finds. It would also have to answer for every opcode's semantics in emitted code, which is a second implementation of the language that has to agree with the first |
| **A value-carrying tier over an unmanaged value representation** | Values are packed into words emitted code can hold - NaN boxing, or an object table indexed from a payload | JSD-0011 already records the price: a moving collector may not have a reference hidden in a payload word, so this is an object table and a handle indirection, a second lifetime mechanism to design and re-prove under Native AOT. It is named in [MVP-8](../../../../docs/mvp.md#5-routes-taken-without-a-decision) as the alternative a later decision could adopt against a retained measurement of this form's cost, and it is not refused for ever |
| **A mixed per-unit form** | Emit the units a backend likes and interpret the rest, which is MVP-7's alternative | It is still what the amended non-goal refuses: one form per handle. Nothing here relaxes that, and the baseline form does not need it relaxed, because it emits every unit |
| **Extract each arm into a method over `ref` state** | The interpreter loop and each handler call shared per-opcode methods taking the operand stack, the stack pointer and the program counter by reference | It risks address exposure of the stack pointer and program counter in the interpreter's largest method, and the runtime's JIT refuses inlines past its locals and budget limits in a method that size, so the interpreter would pay on every instruction for a form it is not running |
| **Handlers with their own pop and push glue** | Each handler reads its operands from the activation and writes its result back, calling the existing helpers | That is a second copy of every arm, including the evaluation-order details the arms get right by being where they are - a store that pops before it throws, a binary operator that converts its right operand before its left. A second copy is a second place for those to be wrong |
| **An interpreter that calls a step method in a loop** | The interpreter becomes a loop over the same one-instruction step the handlers use | It makes the bytecode form pay a call on every instruction, which is a regression of the form every other composition runs, bought for the one that asked for this |
| **A source generator** | Per-opcode handlers are generated at build time from the arm text | It adds a project, which moves the product-project count rules A7 and J7 hold and the solution graph, and a generator is a second reader of the arms whose output nobody reviews at the line it runs |
| **A static-abstract "stepping" getter guarding the loop** | The loop tests a static property of the mode rather than the mode's type | Folding the guard needs the getter inlined into the interpreter's largest method, so the interpreter's instantiation would depend on inline budget; comparing the mode type folds at import without inlining anything |
| **A `GCHandle` to the activation, carried in the frame** | Emitted code passes the handle to each handler, which resolves it | An allocation and a free per call; a missed free leaks for the life of the process, because the core never disposes instance state; and it sits against the wording of JSD-0011's Row 2 even where that row's subject is not engaged |
| **Per-block steps** | One call into managed code for a run of instructions rather than one for each, with everything else as section 2 | It was not weighed, because no measurement of either granularity exists. One call per instruction is taken because it makes each handler exactly one instruction of the interpreter's own method, which the placement checks of section 2 and the fuel argument of section 5 rest on. Taking it without a measurement is a route, recorded as [MVP-8](../../../../docs/mvp.md#5-routes-taken-without-a-decision), and a later decision may take this option against a retained measurement |
| **A read-only mapped page for the handler table** | The table is written into a mapped page that is then protected read-only | It was in the design until implementation and it is dropped. It buys protection against a write no emitted template can perform - S4 admits no memory destination - at the price of a third protection value on the arming path, a new page state, and a change to rule X1's statement. Unmanaged memory allocated once and never written after the static constructor gives the scan argument everything it needs |
| **The baseline form: native control flow, one call per instruction into the interpreter's own dispatch** | Section 2 | Taken |

## 4. Where references are rooted

**The emitted frame holds no managed reference, and neither does anything it points at.** An emitted
unit and `JsBaselineFrame` hold the address of the handler table, which is unmanaged memory that lives
for the process; a 64-bit cookie; and integers in the return and argument registers. No emitted byte
addresses a managed object, so objects may move freely while emitted code runs.

**Every managed object a handler touches is reachable from the activation**, `JsNativeActivation`: the
engine, the program, the unit, the entry values exactly as `Execute` received them, the operand stack,
the scope list, the stack pointer and program counter, and the outcome - whether the unit exited, its
result, and a pending exception. **The activation is rooted in two places, both visible to the
collector.** The first is a local of the `RunNative` frame that entered the emitted code, kept alive
across the call. The second is a thread-static slot.

**How a collection proceeds while emitted code is on the stack.** Emitted code runs in preemptive mode,
because the page's entry is deliberately not a suppressed transition. A handler's reverse transition
switches to cooperative mode; the collector walks the handler's managed frames, then the
transition-frame chain down to `RunNative`'s frame, and finds nothing to trace in between.

**The thread-static slot is how a handler reaches its activation, and it is not the ambient holder rule
N20 exists to report.** N20 reads this family's source for any static or thread-static holder of an
`IJsHostSurface` or a `JsHostRealm`, because in a process hosting two compositions such a holder makes
one embedder reachable from the other's realm. The activation reaches an engine and therefore a realm,
so the argument is owed rather than assumed, and it has three parts:

- **It is set only by `RunNative`, for exactly the lifetime of one emitted call**, and the previous
  value is restored in a `finally`. Nothing reads a value left behind, because none is left behind.
- **It is read only by the handler step, after the cookie check**, so a read that found another
  activation's value would answer `Defect` rather than touch it.
- **It is never observable from another composition's code**, because emitted code runs synchronously
  on the thread whose `RunNative` set the slot and units never call each other directly, so the
  innermost emitted frame on a thread always belongs to that thread's current activation, whose realm
  is the one whose code is on that stack.

**Those three are properties of where the slot is written and read, so they are meant to be checked
rather than read.** The implementation plan registers them as a text rule over the family's source -
the unmanaged-entry attribute only in the handler file, the slot written only in the engine's native
entry file and read only in the activation and handler files - and until the register carries that row
the property is held by reading, which this record says rather than implies otherwise.

**[JSD-0011](0011-the-value-frame-and-call-abi.md)'s Row 2 is not engaged, and it is not amended.** Its
decision is scoped in its own first sentence: "Every value-holding region is a managed array of the
value struct: operand slots, locals, arguments, captured bindings." Every such region stays exactly
that under the baseline form - the activation's operand stack is the interpreter's own array and its
scopes are the interpreter's own environments - so the row's "no manual rooting, no handle table, no
finalizer, no `GCHandle`" binds unchanged, and nothing here roots a value by hand. The handler table is
a table of function pointers to static methods, which is not a handle table; the activation is an
ordinary managed object rooted by ordinary means.

**The code mapping is owned by a `SafeHandle`, and a code mapping is not a value region.** A program's
page is cached on the program and lives exactly as long as the program does - closures, suspended
frames, the engine's module graphs and pending jobs all hold the program - so a program a guest loads
gets its own page and releases it when it becomes unreachable. The release is a `SafeHandle`'s, with
memory pressure reported while the mapping exists, and an eager `Dispose` still releases at once. The
handle holds an address of executable bytes and never a `JsValue`, so Row 2's subject is not reached,
and reading the row as forbidding it would forbid the arming type the numeric form already carries.
The same ownership repairs a defect in the checkout that this design found and did not create:
[JSC-222](../roadmap.corrections.md#jsc-222) records it.

**This is the answer to the core's risk row**, which says that a profile that cannot state where its
emitted code's references are rooted has not earned the form. This section is the statement, and
section 11 says what would make it false.

## 5. Fuel

**Fuel is exact per instruction.** Each handler charges the per-instruction fuel before the opcode's
effects, at the same point in the same method as the interpreter, and every charge inside an arm or a
callee is the interpreter's own. Nothing is charged for entering a unit, dispatching, landing on a
handler or resuming, and the interpreter charges nothing there either. So exhaustion lands on the same
instruction, and cancellation and wall-clock polls happen at the same crossings. **A form that is
slower per instruction reaches a wall-clock allowance after fewer instructions**, and that is the one
way a deterministic budget and a wall-clock budget can answer differently across the forms from the
same fuel.

**The one named divergence is a program a guest loads.** Verifying it charges the verifier's work to
fuel (`JavaScriptProfile.cs` and `VmArtifactLoadMediator.cs`, where guest-load verification work is
metered), and a native artifact is a larger payload than its bytecode alone, which the guest-load byte
bound also counts. So a variant that evaluates, constructs a function from text or imports may exhaust
fuel at a different point in the two forms. The conformance catalog verifies with the scan and without
re-emission to keep that small, and the checks that compare fuel across the forms exclude guest-loading
programs by name rather than tolerating a disagreement in them.

**The numeric form's sentence stands for the numeric form.** `JsNativeFrame`'s remark that "the two
forms exhaust at different points" was written about a form that decrements an unmanaged counter, and
it remains true of that form and only of it. [JSC-219](../roadmap.corrections.md#jsc-219) records the
split.

## 6. What it costs

Stated as properties of the design, because no figure about any of them is retained and this record
states none:

- **Emitted code is many times larger than the bytecode it comes from.** Every instruction carries a
  call sequence and a tail, and every unit a prologue, a compare tree, a defect block and an epilogue.
  The format's native-code ceiling does not move, so a large enough program is refused whole with a
  message naming that ceiling - which is a refusal at compile time, not a failure at run time.
- **Every instruction crosses a reverse transition and passes three checks** before it does what the
  interpreter's loop would have done without either. **No speed is claimed for this form**, in any
  direction, and the design does not rest on one.
- **The per-opcode instantiations are compiled code of their own.** In a JIT-built image they begin
  unoptimised, and in a Native AOT image they add to the image. Gates on the interpreter's own
  instantiation, on per-step pruning, on cold-start cost, on depth and on image size are named in
  [JSB-11](../roadmap.backends.md#jsb-11--the-baseline-form-over-the-wide-manifest-and-a-frame-that-still-holds-no-managed-reference),
  and **the fallback is decided now rather than after a gate fails**: every wrapper can call one
  shared instantiation that reads the opcode from the code, with identical semantics and the same
  checks, switched by one constant.
- **A JavaScript call costs more native stack in this form**, because `RunNative`, the transition, the
  emitted frame and the wrapper sit between two interpreter frames. The call-depth ceiling and the
  stack probe are unchanged; whether the guest thread's stack still reaches the depth ceiling before
  the probe refuses is a gate, and if it does not, a native-only stack figure would be a correction of
  its own.
- **Some variants that complete under a wall-clock allowance in bytecode will exhaust it in this
  form.** That is section 5's consequence and it is named here as a cost rather than as noise.
- **The interpreter's method becomes generic.** Its interpreted instantiation is today's loop with the
  mode tests folded away; that this is true of the compiled code, and not only of the source, is a gate
  to be collected, not a property this record asserts.

## 7. The version rule

**The backend's semantic version is not bumped by this decision, and it covers both tables.** The
numeric artifacts the checkout retains keep verifying by re-emission with the version they carry. **The
first later change to either table - numeric or baseline - bumps the version and re-bases the retained
numeric artifacts in the same change**, because a version that covers two tables and moved for one of
them would otherwise leave the other's retained artifacts naming a version whose bytes no longer
reproduce.

## 8. arm64, and the compositions that do not wire the form

- **A wide artifact naming arm64 is scanned against the unchanged arm64 table**, verifies only if it is
  closed under it, and is refused at instantiation with `UnsatisfiedHostAssumption` on every host,
  exactly as a numeric arm64 artifact is. The retained corpus entry for an architecture no host arms
  keeps its answer.
- **The arm64 backend refuses to emit for the wide manifest**, with a message naming the baseline form
  as having no arm64 emitter. arm64 stays **emitting-only** for the numeric manifest and emits nothing
  for this one. [JSC-221](../roadmap.corrections.md#jsc-221) records the reading.
- **On an arm64 host no x86-64 architecture is armed**, so every wide native artifact answers
  `UnsatisfiedHostAssumption` there. That is a refusal, never a fallback.
- **The polyglot command line is out of scope.** It keeps refusing `--native` without `--numeric`, with
  a message saying the native form is not wired in that composition. The end-user command line and the
  conformance composition take the compile request, so the programs their providers load are compiled
  in the instance's form.

## 9. What this does not provide, stated flatly

- **No speed claim**, and no outcome figure of any kind - no count, rate, timing or measured size - in
  any record this decision touches. The constants stated, such as the frame's offsets and the number
  of prologue and epilogue templates, are properties of the code and not outcomes.
- **No acceptance, no review and no evidence in this record.** **A decision recorded here is not
  evidence that it was implemented**, which is this series' own rule. *(Corrected 2026-09-15, after
  collection: this bullet read "Nothing described here is retained in a bundle, and the stage's bundle
  is to be collected. The code this decision governs is being written at this record's date". The code
  was committed on that date, and [bundle JSB-11-001](../evidence/jsb-11-001/README.md) retains a
  collection over it on one `win-x64` workstation that demonstrates some clauses of
  [JSB-11](../roadmap.backends.md#jsb-11--the-baseline-form-over-the-wide-manifest-and-a-frame-that-still-holds-no-managed-reference)
  and names the rest as open. That bundle is not acceptance and nothing in it has been reviewed; this
  record still states no outcome figure, and what the collection showed is the bundle's to say.)*
- **No runtime identifier.** The support table claims none for either native form and this adds none.
  The System V convention of the baseline form has executed on hosted Linux runners, in a
  pull-request lane job and in a machine-code workflow run, and
  [bundle JSB-11-001](../evidence/jsb-11-001/README.md) retains what those runs printed; neither is a
  claim for `linux-x64`, and the lane itself retains nothing. *(Corrected 2026-09-15: this bullet read
  "The System V convention of the baseline form is to be executed in the continuous-integration lane,
  which retains nothing", written before either run existed:
  [JSC-224](../roadmap.corrections.md#jsc-224).)*
- **No proof of handler placement.** The scan proves every call lands in the table; the wrapper's
  checks catch a misplaced handler at run time; nothing proves at verification that the right handler
  is at the right offset.
- **No change to the numeric form**, its templates, its executor or its answers.

## 10. Governance: no core amendment is made, and none is needed

**The decision needs nothing in `Broiler.VM.Abstractions` or `Broiler.VM.Runtime` to change.** Every
type it adds is in this profile's three assemblies, and the public types it adds to the format assembly
are this family's own baseline's subject, which is what a new public surface in a profile package is
supposed to move. ADR 0003 section 6 covers this in terms: "Implementing something version 1 already
admits (section 8) is not an amendment" (`docs/adr/0003-core-contract-v1-and-amendments.md`). An
artifact payload the core carries opaquely, verified by the profile's own verifier, is version 1 as
shipped.

**Invariant 14 holds, in each of its halves.** Native execution stays declared and never ambient: the
compositions that can arm are the ones whose register cells already read `x86-64`, and the baseline form
arms through the same type and the same files. Pages stay W^X. **The core generates no machine code and
learns no encoding** - the baseline emitter is in this profile's lowering assembly, beside the numeric
one, and the core carries the bytes it emits as it carries every other profile payload.

**VM-7's "no tiering" holds, and the reason is the one VM-7 gives.** "An artifact is bytecode or it is
native, chosen when it is compiled and fixed when it is verified." The baseline form is chosen by the
compile request and pinned by verification; nothing observes a running program and changes what runs
it; and the handlers are the interpreter's own code, compiled as every other method of the profile is,
not code generated from a running program. **A tier is a code generator in the running image, and an
execution-only image of this form still holds none.**

**The non-goal this profile does amend is its own**, and it is amended rather than read around.
[The plan](../roadmap.md#non-goals)'s *second execution arm* entry read "One executor, one form per
handle, no promotion", and MVP-7 made an amendment to it, or a recorded refusal of one, the condition
that settles that route. The corrected entry reads one form per handle **and per instance**, no
promotion and no bail-out, with the native form admitted under the numeric and wide manifests and
whole-artifact under both *(corrected: [JSC-215](../roadmap.corrections.md#jsc-215))*. **Minting a core
contract version was neither wanted nor available**, for the reason [JSD-0024](0024-the-in-realm-host-surface.md)
section 10 already records: the procedure needs a co-signature `docs/mvp.md` section 2.4 defers, and one
person holds every role that would give one.

## 11. What would falsify this

- **A native verdict that differs from the bytecode verdict for the same variant, on the same build and
  machine, other than in three named classes**: a refusal naming the native-code ceiling; a fuel
  exhaustion on a variant that loads a program; and, under a wall-clock allowance, a wall-clock
  exhaustion in the native form. Any other difference is a defect in this design and not a cost of it.
- **A fuel ceiling at which the two forms disagree on a program that loads nothing.** Section 5's claim
  is exactness, and one such ceiling ends it.
- **An emitted frame, or `JsBaselineFrame`, holding a reference the collector traces.** Section 4 is the
  whole rooting argument, and that observation reopens [MVP-3](../../../../docs/mvp.md#5-routes-taken-without-a-decision)
  as that row says it would.
- **A scan-accepted baseline payload whose indirect call leaves the handler table**, or a unit that
  returns to its caller with RBX or R14 not restored. The scan argument of section 2 is a chain, and
  either observation breaks a link of it.
- **A swapped handler that answers a JavaScript value.** An emitted call rewritten to another defined
  opcode passes the scan by design; if invoking it produces anything but an internal defect, the
  wrapper's checks are not doing the work section 2 gives them.
- **A catch filter in this profile that is not pure.** The interpreter runs an outer frame's filters
  before an inner frame's finally blocks, and the baseline form runs the inner finally blocks first,
  because a wrapper catches everything before its emitted frame returns. Those orders are equal only
  while filters observe nothing a finally block changes. The design reading found one catch filter in
  the profile assembly, the pure search for a covering region, and the stage's bundle is to record that
  audit rather than this record asserting it; a filter that is not pure would make the forms
  observably different.
- **A read or write of the activation slot outside the places section 4 names**, or an activation
  reachable from a thread whose emitted code is not on the stack. That is the ambient holder rule N20
  exists against, reached sideways.
- **A numeric-form answer that changes.** The numeric form is untouched by this decision, and a
  retained numeric golden byte, corpus answer or checks row that moves is this decision reaching
  somewhere it said it did not.
