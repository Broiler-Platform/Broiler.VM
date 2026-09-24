<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0035 - The value form: emitted instruction semantics over NaN-boxed values and a per-instance handle table

**Status:** Proposed, design only. 2026-09-24. **Nothing described here is implemented**, no measurement
of it exists, and no bundle retains one. Nobody has signed the record, so it claims no approval.
Approvals are deferred under the MVP terms.

**Owner:** JavaScript profile owner. **Co-signer:** the core's security owner, because the design adds
a rooting mechanism and a new class of emitted template. **Both roles are held by one person**, and
this record does not claim the co-signature is independent.

**Milestone:** none yet. If adopted, it settles the alternative branch of route
[MVP-8](../../../../docs/mvp.md#5-routes-taken-without-a-decision) - "a value-carrying tier over an
unmanaged value representation, with a rooting scheme" - and opens a stage of its own, named **JSV**
here until the roadmap allocates it.

## What was open

This profile has three output forms. **Bytecode**, run by the interpreter. The **numeric form**
([JSD-0025](0025-the-baseline-native-form-over-the-wide-manifest.md) calls it the computing form),
whose emitted code executes the instructions of `broiler.javascript.numeric` itself over slabs of
`double`s. And the **baseline form**, whose emitted code decides only which interpreter step to call
next: every instruction still runs in the interpreter's own arm, so its work is the interpreter's work
plus the transitions between emitted and managed code. JSD-0025 says so in its own words, and makes no
speed claim for it.

**Working-tree observations, taken on 2026-09-23 on one `linux-x64` machine and retained nowhere,
motivate this record and prove nothing.** No figure from them is written here, by the rule that keeps
figures out of decision records. The whole-suite conformance run gave the same verdicts in both forms
apart from wall-clock races, and the baseline form was the slower. It was also slower than bytecode on
every Octane benchmark. The numeric form was far faster than bytecode on a program inside its
manifest. The largest native-only cost was re-entering the interpreter's dispatch method at every
block, whose frame is interpreter-sized and zero-initialised on each entry. The largest single cost in
both forms was the per-instruction fuel charge through the meter. What those observations point at is
the claim this record rests on: **a native form is faster than the interpreter only where its emitted
code executes instruction semantics itself**, and the one form that does so today admits only Numbers.

Three records stand between the wide manifest and such a form, and this record answers each:

- [JSD-0011](0011-the-value-frame-and-call-abi.md) Row 1 registers NaN boxing and does not adopt it,
  because on a moving collector it means "an object table and a handle indirection - a second lifetime
  mechanism to design, test and re-prove under Native AOT".
- JSD-0011 Row 2 decides that every value-holding region is a managed array of the value struct, with
  "no manual rooting, no handle table, no finalizer, no `GCHandle`", because "a rooting bug in a
  hand-written scheme is not a bug a corpus finds".
- JSD-0025 section 3 names this route, "a value-carrying tier over an unmanaged value representation",
  as "not refused for ever", and section 5 holds a native form to exact fuel.

## Decision (proposed)

### 1. A third form, fixed at compile time

**The value form is a whole-artifact output form over `broiler.javascript.wide`, for both x86-64
conventions and no other architecture.** It is chosen when the artifact is compiled, like the other
two. One form holds per handle and per instance, nested loads included: an `eval`, a `Function`
constructor or an import compiled for a value-form instance is compiled in the value form, and a
program of any other form is an internal defect there, as it is in a baseline instance today. Nothing
chooses a form from a run-time observation, so this is a form and not a tier, by the definition VM-7
gives ("a tier is a code generator in the running image; a form is not"). The arm64 backend stays
emitting-only and emits no value form.

**The interpreter and the baseline form are unchanged.** Every region the interpreter uses stays a
managed array of `JsValue`, and JSD-0025 is not amended for the form it decides.

### 2. The value word

**A value-form value is a 64-bit word, `JsWord`, and emitted code only ever holds words.**

| Bits 63-48 | Meaning | Payload (bits 47-0) |
|---|---|---|
| anything below `0xFFF9` | a Number: the word *is* the IEEE-754 binary64 | the rest of the double |
| `0xFFF9` | a special constant | `0` undefined, `1` null, `2` false, `3` true, `4` empty (an uninitialised binding), `5` hole |
| `0xFFFA` | a String handle | generation (16 bits) and index (32 bits) |
| `0xFFFB` | an Object handle, every function and array included | generation and index |
| `0xFFFC` | a Symbol handle | generation and index |
| `0xFFFD` | a BigInt handle | generation and index |
| `0xFFFE` | a frame header, never a value (section 3) | unit index and published operand height |
| `0xFFFF` | reserved | - |

- **Is-a-Number is one shift and one compare.**
- **The tag space leaves both quiet NaNs x86-64 produces as Numbers.** The arithmetic unit's default
  NaN is `0xFFF8_0000_0000_0000` and a sign flip of it is `0x7FF8_0000_0000_0000`. Arithmetic on
  operands outside the tag space cannot produce a word inside it, so inline arithmetic needs no
  canonicalisation step.
- **Every double that enters from managed code is canonicalised.** A typed-array read can produce a
  NaN with any payload, including one that would read as a tag. The encoder maps every NaN to
  `0x7FF8_0000_0000_0000`, and JavaScript cannot observe a NaN's payload except through typed-array
  bytes, which never hold a `JsWord`.
- **The encoding is this form's alone.** The numeric form's two reserved NaN patterns stay the numeric
  form's, and no word crosses between the two forms.

### 3. Where words live: the value slab

**Each value-form instance owns one pinned slab of words**, allocated as the numeric form allocates its
operand slab: a pinned array that holds no managed reference and never moves. Every activation of
value-form code takes a region of it, and a region is laid out as:

1. one **frame header** word, tagged `0xFFFE`: the unit index and the operand height the frame last
   published;
2. the **arguments**;
3. the **resident bindings** (below);
4. the **operand stack**.

A unit's prologue checks that its region fits before it bumps the slab pointer. **A call also leaves
the format's argument ceiling of headroom past the end of the region it checked.** The numeric form's
missing headroom let argument stores run past its slab, and the correction dated 2026-09-23 fixed that
defect in the numeric form; this form carries the headroom from its first line.

**A binding is resident, living in the slab, when a static analysis in the lowering proves that
nothing outside its own activation can reach it.** That means no closure captures its scope, no direct
`eval` or `with` can name it, and it is not a sloppy-mode parameter aliased by a mapped `arguments`
object. Every other binding stays in a managed `JsEnvironment` exactly as the interpreter keeps it, and
emitted code reaches it only through a helper (section 5). The analysis generalises the numeric
backend's walk, which is sound only because its manifest admits no closure. It is a pure function of
the image, so re-emission reproduces it.

**Frame metadata that is not a value** - the return address, the saved registers and the entry
program counter - lives on the machine stack, as the baseline form's frames do. Every prologue also
compares the stack pointer with a limit written into the instance context at entry. A deep recursion
therefore answers the same `RangeError` the interpreter gives, and never overflows the machine stack.

### 4. The handle table: what makes a reference safe to hold as an integer

**A handle is an index into a per-instance managed table, `JsHandleTable`, plus a generation.** Each
entry holds an ordinary managed reference, so the collector traces it because it is a field, which is
Row 2's own reason. Emitted code never dereferences a handle: it moves, compares and tag-tests it, and
only a managed helper turns a handle back into an object. **So JSD-0025 section 4's property holds
unchanged for this form: no emitted byte addresses a managed object**, and objects move freely while
emitted code runs, which it does in preemptive mode.

**Every decode checks the index range, the generation and the kind.** A mismatch is an internal defect
that fails closed; it never reads another object. A stale handle is therefore a refusal, never a
wrong answer.

**Liveness is decided by a precise scan, not by scopes.** The roots of the table are:

- the words in the live extent of every value-form frame of the instance, where each frame is walked
  from its header up to its published operand height;
- the instance's constant handles, which are permanent for its life;
- the words a helper holds while it runs.

A **compaction** marks every handle reachable from those roots and puts every other entry on the free
list, bumping its generation. It runs only inside a helper, on the instance's own thread, with every
emitted frame of the instance suspended at a call. It runs when an allocation finds the free list empty
and the table has grown by a stated factor since the last compaction, so its cost is amortised against
allocation. Because every word is self-describing, the scan cannot mistake an integer for a handle: a
word either carries a handle tag or it does not.

**The answer to "a rooting bug in a hand-written scheme is not a bug a corpus finds" is to make it
one.** A **handle-stress mode** compacts at every helper call, poisons every freed entry and bumps its
generation. Under it, any word the scan failed to root decodes to a generation mismatch on its next use
and fails that variant by name. The whole pinned conformance suite in the value form under
handle-stress is an exit gate of the stage (section 10), in the way a GC-stress run is for a runtime.

**Suspension leaves no handle outside the slab.** A generator or async frame that suspends is decoded
into the interpreter's own `JsFrame` of `JsValue`s, and is re-encoded when it resumes. So a handle can
be held only by a live slab word, by a constant, or by a running helper.

### 5. Instruction semantics: inline, helper, or control

**Inline templates are the pure set: instructions whose effect is on the slab alone and which cannot
run guest code.** They are:

- loading a constant that is a Number, a special constant or an already-handled String;
- loading and storing arguments and resident bindings, including the empty check of an uninitialised
  binding;
- the stack operations: `Pop`, `Duplicate`, `DuplicateTwo`, `Swap` and `Pick`;
- `Add`, `Subtract`, `Multiply`, `Divide` and `Negate`, the comparisons, and the bitwise and shift
  operators, on two Numbers;
- `Not`, and the conditional jumps on a Boolean, a Number, `undefined` or `null`;
- `Jump`, `Increment` and `Decrement` on a Number, `Void`, and `TypeOf` of a non-handle word.

**Each inline template is guarded by a type test, and by a range test where the machine instruction is
exact only on part of its domain.** A bitwise operator's `ToInt32` of a double outside the 32-bit range
and a result whose sign of zero the machine instruction would lose are such cases, and they take the
helper; `Remainder` is left out of the pure set for the second reason. When a guard fails, the same
instruction runs through its helper. That is the general case of this form's own semantics, not a fallback to another
form, and it is how section 1's "one form" survives a guard. The numeric backend's SSE2 templates for
arithmetic, comparison and NaN handling are the model, and every inline template must agree bit for bit
with the interpreter's arm, which the differential gate in section 10 checks.

**Every other instruction runs through a helper, and a helper is the interpreter's own arm, not a
second copy of it.** JSD-0025 rejected handlers with "their own pop and push glue" because a second copy
of every arm is a second place for evaluation order to be wrong. So a helper:

1. decodes the instruction's input words into a per-instance mirror window of `JsValue`s;
2. runs the interpreter's per-opcode instantiation for that one instruction (the kind the baseline form
   already uses for its run-alone opcodes), whose frame keeps one arm rather than the whole switch;
3. encodes its outputs back into the slab.

The arm's pops, pushes, conversions and throws stay the arm's own. What the helper adds is the codec,
and the codec is small enough to test exhaustively over the kinds.

**Control is emitted.** Branches, landings, and exception-region dispatch by a compare tree over the
unit's landings are the baseline form's machinery, and they carry over. A helper for an instruction
with a code target - `ForInNext`, `IterateNext`, `IterateAwaitStep`, `IterateCloseAsync`,
`DisposeStep` - answers which way it went, and the branch itself is emitted. So the compare tree is
needed only where control re-enters a unit from outside: its entry, its exception handlers and its
resume points.

### 6. Calls

**A call to a value-form script function of the same instance is a direct machine call.**

- **Preparation.** A call-prepare helper does everything the interpreter's `Call` does before it
  executes the callee: the fuel charge, the call-depth charge, the callee's kind and constructor
  checks, the `this` binding, an `arguments` object where the callee needs one, and a new
  `JsEnvironment` only where the callee has non-resident bindings.
- **The call itself.** The helper answers the callee's entry address, and emitted code calls that entry
  with the arguments already in the new region.
- **The return.** The callee returns its value in its region's first slot and a status in the return
  register.

A call to anything else - a built-in, a host function, a proxy, a bound function or a baseline-form
function - goes through a helper. **So a value-form call pays no activation object, no entry step and
no pair of collector transitions**, which are the per-call costs the baseline form carries today.

### 7. Fuel: the same verdict at every ceiling, without a meter amendment

**A pure instruction adds one to an unmanaged debt counter in the frame context, and the debt is
charged to the meter with the existing `TryCharge` at settlement points:**

- at every helper call, before the helper runs;
- at every call and return;
- on a back edge, and after every run of a stated number of straight-line pure instructions, once the
  debt reaches a stated threshold, so the debt between two polls is bounded on straight-line code as
  well as in loops;
- at every exit.

**Every other instruction is charged by its helper, per instruction, exactly as the interpreter charges
it.**

**Why the verdict cannot differ at any ceiling.** Every instruction is charged exactly once in both
forms, so a program that loads nothing has the same total charge in both. (A program that loads code
at run time is charged for verifying it, and a native payload is larger; that is JSD-0025's existing
named divergence and is not widened here.) A run completes under a ceiling in the value form
exactly when it does in the interpreter. When it exhausts, the only instructions that ran uncharged
past the interpreter's exhaustion point are pure ones. Their effects are on the slab alone, the
operation ends in exhaustion, and they are observable by nothing: no guest code, no host capability,
no realm state. Cancellation and wall-clock polls happen at settlements, whose spacing is bounded by
the threshold and by the pure set's lack of calls.

**What can differ at exhaustion, named rather than hidden.** `TryCharge` is all-or-nothing: a refused
charge commits nothing and names the outermost budget level that would refuse it (`VmMeter.cs`, the
outermost-first admission and `Refuse`). The interpreter charges one unit at a time, so it uses up
exactly the remaining allowance and then names the level that ran out. A refused settlement of a debt
of *k* therefore differs from the interpreter in two ways:

- the fuel reported as consumed is lower, by less than *k*;
- when two budget levels are both within *k* of their ceilings, it can name the outer level where the
  interpreter names the inner one.

Neither changes whether the operation completed. The value form states them as its **named
divergences at exhaustion**, and the verdict-equality gate compares the outcome kind and the exhausted
dimension, not the consumed figure or the level. A later record may narrow them only by means that do
not read the remaining allowance, because reading it is the amendment MVP-9 says cannot be minted.

**This needs no core amendment.** MVP-9 records that an allowance held by the profile, sized by a new
meter member that reads remaining fuel, is a breaking amendment the procedure cannot mint. Settling
debt after the fact uses only `TryCharge`, and it never asks for fuel it has not already spent.
JSD-0025 section 5's exactness binds the baseline form, which is not changed. This form states its own
equivalence, which is verdict equality at every ceiling, and section 10 tests that with the fuel-parity
twins.

### 8. Exceptions, generators and async

- **Throw and catch.** A helper that catches a guest exception parks it in the instance context and
  answers "threw" with the program counter. Emitted code dispatches it through the unit's region table
  to a landing, or returns "threw" to its caller. **No managed exception ever crosses an emitted frame**,
  as in the baseline form. A throw that crosses several value-form frames is therefore a chain of
  status returns, not one managed rethrow per level.
- **Generators and async functions.** A body enters through the landings the baseline plan already
  computes (`Yield`, `Await`, `EnterBody`). `Yield` and `Await` are helpers that decode the frame into
  its `JsFrame` and answer "suspended". Resumption re-encodes the frame and enters at the resume
  landing. Abrupt resumptions are raised by the resuming helper, so they land in regions or propagate
  exactly as the interpreter's do.

### 9. Verification

**The value form gets its own closed template table and its own shape clauses, and the template-closure
scan runs over it in every image**, as it does for the other two forms. The clauses a value-form payload
must satisfy are:

- **V1.** Every memory destination is a slab-region operand addressed from the region register, or the
  frame context's debt word.
- **V2.** No template uses a handle payload as an address.
- **V3.** Every call goes through the helper-table register or to a unit entry of the same payload.
- **V4.** Every unit begins with the region check and the stack-limit check.
- **V5.** Every back edge carries the debt test.
- **V6.** Every inline template's guard branches to the helper call for its own instruction.

**Re-emission stays a pure function of the image**, residency analysis included, and verification
compares the bytes where an image carries the lowering. **The form is recorded in the artifact**, as a
one-byte tier in the emitted-code section's header. The manifest no longer fixes the tier, because the
wide manifest now has two native forms. A manifest with only one tier must still carry that tier, so
the byte and the manifest cannot disagree. The backend's semantic version and the format's minor
version both move.

**What emitted code may touch**, stated once for the security co-signer:

- its slab region;
- its frame context's unmanaged fields: the debt, the stack limit, the helper-table address and a
  cookie;
- its own code.

It holds no managed reference, dereferences no handle, and writes nothing outside its region. The
arming path, W^X, rule X1 and rule B5c are untouched.

### 10. How it would be built, and what gates each stage

| Stage | Delivers | Exit gate |
|---|---|---|
| **JSV-0** | `JsWord`, its codec, `JsHandleTable`, the scan and handle-stress, managed only | Codec round-trip over every kind and every NaN payload; scan and generation checks under a fuzz target; no emitted code |
| **JSV-1** | The value form with **every** instruction a helper, except the control flow, which is emitted | The whole pinned suite in the value form gives the same verdict per variant as bytecode, outside the admitted classes, with and without handle-stress; all fifteen Octane benchmarks report a score |
| **JSV-2** | Residency analysis, the pure inline set and fuel debt | JSV-1's gate again, plus the fuel-parity twins giving the same verdict at every ceiling, plus a differential run of every inline template against its arm |
| **JSV-3** | Direct calls and the stack limit | JSV-2's gate, plus recursion answering the interpreter's `RangeError` rather than exhausting the machine stack |
| **JSV-4** | Suspension through the frame codec, and status-chain exceptions | JSV-3's gate over the generator, async and exception subtrees under handle-stress |

**Speed is judged once, by a predeclared rule, against a retained measurement**, as
[roadmap.gates.md section 17](../roadmap.gates.md#17-measurement-discipline) requires. The rule file is
committed before JSV-2's code exists. Its arms are bytecode as the control, the baseline form, the value
form as the candidate, and an A/A lane of the candidate. Its population is the wide shapes and the
Octane benchmarks. The owner fixes its thresholds, and figures go only in that bundle's README. **A
REFUSE verdict ends the stage** and reverts it; it does not narrow the population until the rule
passes.

## What it amends, if adopted

- **JSD-0011 Row 1.** NaN boxing is adopted for the value slab, and for nothing else.
- **JSD-0011 Row 2.** A second rooting scheme is admitted for one region, the value slab. That scheme
  is a per-instance handle table, a precise tag scan, generation checks and a stress mode that makes a
  rooting bug a corpus failure. Every other value-holding region stays a managed array of `JsValue`.
- **MVP-8.** Its alternative branch is taken, on the measurement section 10 requires. Until then the
  route stands.
- **JSD-0025.** It is not amended. Its baseline form, its section 4, its section 5 and its clauses S1
  to S4 continue to bind that form. The value form states its own counterparts in sections 3, 4, 7 and
  9.
- **Architecture rules.**
  - **X2** extends to the value frame context, which declares no reference field.
  - **X3** extends to the one file that declares the value form's helpers.
  - A new rule holds clauses V1 to V6 and the residency analysis's purity.
  - **X1, B5c and K5** do not move: the same arming path, and the same `x86-64` register column.
- **Invariant 7.** If a later record adds inline caches, they live in a per-instance side table and
  never on the shared handle, as the invariant requires. Nothing in this record adds one.

## What this rejects

| Option | Why it is not taken |
|---|---|
| **Emitted code holding `JsValue`s, with stack maps, pinned handles or `GCHandle`s telling the collector where the references are** | JSD-0025 already rejects it against Row 2. It also needs the runtime to accept a code generator's GC information, which a Native AOT image does not offer |
| **Keeping emitted code cooperative, a suppressed transition** | Route MVP-3 takes the transition rather than suppressing it, and a thread that stays cooperative inside code with no safe point blocks every collection for as long as it runs |
| **Per-activation handle arenas freed on return, with no scan** | A long loop inside one activation, which is exactly what a benchmark's main loop is, would grow its arena without bound |
| **A `GCHandle` or a pinned object per referenced value** | An allocation and a free per value, a leak on any missed free, and pinning that fragments the heap. It is also what Row 2 names first |
| **Moving the interpreter itself onto `JsWord`** | It rewrites every arm's handling of values in the form that is the oracle for the other two, and it would put the handle table on the path of every composition, including those that never asked for a native form |
| **Exact per-block fuel reservation through a new meter member** | MVP-9 records that as a breaking core amendment the procedure cannot mint. Settling debt after the fact gives verdict equality with the existing member |
| **Helpers with their own pop and push glue** | JSD-0025's reason stands: a second copy of every arm. The mirror window over the interpreter's own per-opcode instantiation is used instead |
| **A mixed per-unit form, or emitting only hot units** | The profile's non-goal (one form per handle) and VM-7 (no profile-guided recompilation) refuse both |
| **Growing the numeric form to strings and objects** | The numeric form is exact over its manifest precisely because it has no references. Giving it references is this record's work under another name, and it would move that form's retained golden bytes |

## Risks this record names rather than resolves

- **Helper-dominated code may be slower than bytecode.** Property access, calls to built-ins and
  closures over captured bindings each pay a transition and the codec. The rule in section 10 is what
  catches this. Inline caches in a per-instance side table are a later record's work, not this one's.
- **Allocation-heavy loops churn the handle table.** Compaction cost scales with the live frames'
  extent, and the rule measures it rather than assumes it.
- **The residency analysis is a new static analysis whose error is a wrong answer.** A binding wrongly
  classed as resident is invisible to a closure or to `eval`. So the analysis is proved over the
  lowering's own scope model, and the whole-suite gate runs with a mode that classes every binding as
  non-resident, as a control.
- **Emitted frames still register no unwind information on Windows.** Debuggers, profilers and ETW
  stack walks stop at the entry, as they do for the baseline form.

## Falsified if

- A value-form artifact gives a verdict different from the interpreter's on any variant of the pinned
  suite, outside the admitted classes, under any ceiling. The consumed fuel and the budget level named
  at exhaustion are section 7's named divergences, not verdicts.
- A handle decodes to an object other than the one it was encoded from, or an object reachable only
  from a live slab word is released.
- An emitted template dereferences a handle payload, or writes outside its region and its context's
  debt word.
- A form is selected, or a unit re-emitted, from a run-time observation.

## What this does not decide

- **Whether the value form is faster.** That is the measurement's to answer, and this record makes no
  speed claim.
- Register allocation, which is left out on purpose: every word lives in the slab, as in the numeric
  form.
- Inline caches, shapes or any other per-instance optimisation state.
- Any change to the numeric form, the baseline form, the arm64 backend or the core contract.
- The settlement threshold, the table's growth factor, the slab's size and the generation width. The
  JSV-0 and JSV-2 records fix those against the stress and fuel-parity gates, not against speed.
