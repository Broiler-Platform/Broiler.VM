# The universal bytecode and the emitter profiles — a concept and a proposed programme

**Status:** Concept. Written 2026-09-25 against the request its section 1 quotes. **It decides
nothing, mints no identifier in any decision series, moves no ledger row, and schedules no work.**
Its stages are `UBC-n`, a namespace of its own for the reason every proposal document in this
repository gives for theirs: a `VM-`, `JS-` or `WA-` identifier with no ledger row would read as a
milestone somebody is tracking.

**Owner:** the Broiler.VM architecture owner, who also holds the release, security, contract-minting
and review roles [ADR 0012](adr/0012-security-ownership-and-support-matrix.md) names — six roles held
by one person, recorded as EX-30. Nothing here is co-signed, and this document does not claim that a
co-signature would be independent if one were given.

**Core contract version:** 1. **This concept needs no amendment of it**, and section 3 states the one
place where that claim is a reading rather than a fact.

**What this document is.** A design and a proposed programme for one change to the shape of this
component: that a language profile lowers only to one shared, profile-neutral bytecode — the
**universal bytecode**, `UBC` below — and that what turns that bytecode into something a process can
execute is a separate component, an **emitter profile**, of which the first is the bytecode emitter
(an interpreter of the universal bytecode) and the next two are the `x86-64` and `arm64` machine-code
emitters. It is written against the checkout as it stands on its date, section 2 says what that
checkout holds, and it names, for every rule and record its design meets or moves, which one and what
would have to change.

**What this document is not.** It is not the ledger and moves no row in one. It is not a decision:
no `ADR`, `JSD` or `WAC` identifier is minted here, and section 13 says which records a stage would
have to file before it emits a byte. It carries **no figure of any kind** — no timing, no ratio, no
instruction count, no score — under the rule every plan in this repository writes for itself, and where
a figure would be the natural way to say something it names the command or the bundle instead. **And
it makes no speed claim in either direction**: a native form is admitted here as a capability, exactly
as [VM-7](roadmap.md#vm-7--admit-a-native-artifact-form-and-in-process-native-execution) admits one,
and what any form is worth is a measurement nobody has taken.

**One record stands above this one.** [`docs/mvp.md`](mvp.md) states what this stage of work defers
and what deferring a decision does not defer, and this document defers to it: approval of boundary
records, human review, evidence collection and milestone acceptance, and the co-signing step of the
amendment procedure are deferred; the automated gates, the status vocabulary, the stop condition on an
untruthful support claim, the non-advertisement of every composition with the three-package pack set,
and the prohibition on publishing are not. Every sentence below is written under that rule.

---

## Contents

0. [The shape, on one page](#0-the-shape-on-one-page)
1. [The request, and the vocabulary this document uses for it](#1-the-request-and-the-vocabulary-this-document-uses-for-it)
2. [What the checkout holds on 2026-09-25, and what the concept replaces](#2-what-the-checkout-holds-on-2026-09-25-and-what-the-concept-replaces)
3. [The rules this concept meets, and the ones it asks to move](#3-the-rules-this-concept-meets-and-the-ones-it-asks-to-move)
4. [The dependency graph after the refactoring](#4-the-dependency-graph-after-the-refactoring)
5. [The universal bytecode](#5-the-universal-bytecode)
6. [Verification: one verifier, three layers](#6-verification-one-verifier-three-layers)
7. [The emitter profiles](#7-the-emitter-profiles)
8. [The language profiles after the refactoring](#8-the-language-profiles-after-the-refactoring)
9. [The compositions after the refactoring](#9-the-compositions-after-the-refactoring)
10. [What was considered and not taken](#10-what-was-considered-and-not-taken)
11. [What it costs](#11-what-it-costs)
12. [Routes this concept takes without a decision](#12-routes-this-concept-takes-without-a-decision)
13. [The programme: stages UBC-0 to UBC-9](#13-the-programme-stages-ubc-0-to-ubc-9)
14. [What would falsify this concept](#14-what-would-falsify-this-concept)
15. [What this concept does not do, stated flatly](#15-what-this-concept-does-not-do-stated-flatly)

Appendices: [A. The common family](#appendix-a--the-common-family), [B. The JavaScript
family](#appendix-b--the-javascript-family), [C. The WebAssembly family](#appendix-c--the-webassembly-family),
[D. The primitive table](#appendix-d--the-primitive-table), [E. The container, field by
field](#appendix-e--the-container-field-by-field), [F. Every rule and record this concept
touches](#appendix-f--every-rule-and-record-this-concept-touches).

---

## 0. The shape, on one page

```text
                 language profiles                      emitter profiles
                 (translate, and only translate)        (execute a form)

  JavaScript source ──► JavaScript lowering ──┐        ┌──► bytecode emitter ─── interprets UBC
                                              │        │      (Broiler.VM.Profile.Bytecode)
                                              ▼        │
                                     ┌─────────────────┴──┐
  WebAssembly module ──► WebAssembly ─►   universal        ├──► x86-64 emitter ──── emits machine code,
  (decoded, validated)   translator  │   bytecode (UBC)   │      arms and runs it
                                     │  Broiler.VM.Ubc    │      (Broiler.VM.Profile.MachineCode.X64
                                     └─────────────────┬──┘       + Broiler.VM.Profile.MachineCode)
                                              ▲        │
        a family: its instruction table,      │        └──► arm64 emitter ────── emits, and runs nowhere
        its handlers, its runtime library,    │                 (Broiler.VM.Profile.MachineCode.Arm64,
        its verifier hook, its payloads ──────┘                  emitting-only)
```

- **One bytecode, several families.** The universal bytecode is one instruction encoding with a
  **common family** every emitter executes itself — `return`, `jump`, `call`, locals, constants, the
  stack shuffles — and any number of **language families**, each a namespace of instructions whose
  meaning belongs to the language profile that declares it: `js.add` is the JavaScript profile's,
  `wasm.i64.add` is the WebAssembly profile's, and no emitter ever learns what either means.
- **A family instruction is executed through the family's own handler, or through a primitive the
  family names.** Every family instruction has a handler the family supplies, which is the authority
  for its meaning. A family may additionally classify an instruction as one of a closed, language-free
  set of **primitives** — a two's-complement add, an IEEE-754 multiply, a bounds-checked load from a
  region the family owns — and an emitter may then implement it without calling the handler, on the
  condition, checked differentially, that the two agree bit for bit. That is how `wasm.i64.add` stays
  the WebAssembly profile's instruction and still becomes one machine instruction.
- **Two planes of operand.** A slot is `i32`, `i64`, `f32`, `f64` or `v`. The four numeric types are
  machine words the emitter holds in registers, in a native stack or in an array. `v` is a **language
  value** whose representation the family owns; emitted code never holds one, never addresses one and
  never learns its layout, which is the property [JSD-0025](../src/Broiler.VM.Profile.JavaScript/docs/decisions/0025-the-baseline-native-form-over-the-wide-manifest.md)
  section 4 and route [MVP-3](mvp.md#5-routes-taken-without-a-decision) already rest on, carried over
  unchanged.
- **The core is untouched.** `Broiler.VM.Abstractions`, `Broiler.VM.Binary` and `Broiler.VM.Runtime`
  change by nothing. The core still sees one `VmProfileDescriptor` per language, with the language's
  own identity, limits, maxima, capability imports and payload range; what changes is that the verifier
  and the executor that descriptor names are built by an emitter from the language's declaration rather
  than written by the language. The core still generates no machine code and learns no encoding: the
  universal bytecode is not a core assembly and the core never references it.
- **One form per artifact, fixed at compile time and pinned at verification.** An artifact is the
  bytecode form or one native form, whole; there is no per-unit choice, no guard, no fallback, no
  promotion and no tier. That is VM-7's rule and the JavaScript profile's amended non-goal, both kept.
- **What moves out of the language profiles is mechanism: the verifier walk, the dispatch loop, the
  frame, the call, the unwinding, the fuel charge, the arming path.** What stays in them is every value,
  every instruction's meaning, every diagnostic, every conformance suite and every payload. That is the
  line [section 8 of the roadmap](roadmap.md#8-sharing-between-profiles-without-a-lowest-common-denominator-core)
  draws, and section 3 below says exactly where this concept stands on either side of it.

---

## 1. The request, and the vocabulary this document uses for it

**The request, quoted.** "Refactor this component as follows: the language profile translates only to
universal bytecode. The universal bytecode will be emitted by an emitter profile. The first language
profile will be JavaScript and the first emitter profile is bytecode. Future language profile is
WebAssembly and future emitter profiles will be x64 and arm64. The instruction set for the bytecode
should contain common instructions like `Return`, JavaScript-specific like `add`, WebAssembly-specific
like `i64add`, and so on."

**Four things in that request are read as instructions rather than as suggestions**, and each is a
design constraint below: that a language profile *only* translates, so it owns no interpreter and no
verifier walk of its own; that the executable form is produced by a component the language profile
does not contain; that the instruction set is one set with three kinds of member — common, and one
family per language — rather than three sets; and that the emitters arrive in the order bytecode,
`x86-64`, `arm64`, with JavaScript before WebAssembly on the other axis.

**The vocabulary.** This repository already uses the word *profile* for several boundaries, and
[ADR 0003 section 9](adr/0003-core-contract-v1-and-amendments.md) freezes ten terms and bans a list of
synonyms for one of them. This document keeps the request's two phrases and fixes what each means
here, so that a reader who knows the frozen vocabulary can map them:

| The request says | This document means | The frozen term, where one applies |
|---|---|---|
| **language profile** | An *input profile*: a product project family under `src/Broiler.VM.Profile.<Language>*` that owns a language's front end or decoder, its lowering **to the universal bytecode**, its instruction family, its runtime library, its handlers, its diagnostics, its payloads and its conformance suite. From the core's side it is still exactly one **VM profile**: one descriptor, one identity, one set of limits. What it no longer owns is an executor of its own, a verifier walk of its own or a bytecode format of its own. | **VM profile**, in the roadmap's section 1 sense, for what the core sees. |
| **emitter profile** | An *artifact output form* promoted to a component: a product project family that consumes verified universal bytecode and produces the thing a process executes — the interpreter for the bytecode form, an encoder plus an arming path for a machine-code form. It has an identity, a ledger, a support-table row and a version. **It is not a VM profile**: it has no descriptor of its own in a catalog, no limits of its own, and no language. The word *profile* is kept because the request uses it and because [`eng/nuget/README.md`](../eng/nuget/README.md) and JSD-0025's quoted instruction already call the machine-code component an *output profile*; where this document needs the frozen term it says **artifact output form**. | **Artifact output form**, in the roadmap's section 1 sense. |
| **universal bytecode** | The one instruction encoding, container format, verifier walk and family contract every language profile lowers to and every emitter profile consumes. Abbreviated **UBC**. It is a shared *mechanism* component under [section 8](roadmap.md#8-sharing-between-profiles-without-a-lowest-common-denominator-core)'s rule, and section 3 says which halves of it are mechanism and which half asks a standing refusal to be reopened. | None. The term is new, and it is not a synonym for any frozen one. |
| **family** | One namespace of the universal bytecode's instructions: the common family, owned by the bytecode itself, or a language family, owned by one language profile. A family is what an artifact declares it uses and what a composition declares it composes. | None. |
| **form** | Which emitter's output an artifact carries: `bytecode`, `x86-64-sysv`, `x86-64-win64` or `arm64-aapcs64`. Fixed when the artifact is compiled, recorded in its header, pinned when it is verified. | **Artifact output form.** |

**The words *backend*, *engine*, *plug-in*, *extension* and *implementation* are not used for any of
these**, because T4 bans them as synonyms for a VM profile and a document that used one for the
emitter would invite exactly the reading it bans. Where an existing type is named — `IJsNativeBackend`,
`JsEngine` — the name is quoted as code and not adopted as a term.

**What "translate only" is read to mean, precisely.** A language profile's output is a universal
bytecode artifact and nothing else: it produces no artifact of its own format, it interprets nothing,
it verifies nothing on its own authority, and it arms no page. What it keeps is everything that gives
its instructions meaning, because that meaning is exactly what section 8 says may not be shared:
the JavaScript profile keeps its realm, its object model, its strings, its `JsValue`, the arms of its
interpreter as handlers, its front end and its diagnostics; the WebAssembly profile keeps its decoder,
its validator, its store, its memory, its trap kinds and its numeric semantics. **A language profile
after this refactoring is a translator and a runtime library with an instruction table between them.**

---

## 2. What the checkout holds on 2026-09-25, and what the concept replaces

This section is a reading of the tree on the concept's date, written so that a reader can check each
sentence against a file and so that the refactoring in section 8 starts from what exists rather than
from what the plans say. Files are named; line numbers are not, because a line moves.

### 2.1 The JavaScript profile: three assemblies, two formats, three forms

- **Three assemblies, with the format as the pivot.** `Broiler.VM.Profile.JavaScript.Format` references
  nothing at all and holds the opcodes, the container schema, the encoder tables and the native
  template tables. `Broiler.VM.Profile.JavaScript.Compiler` references Abstractions and the format and
  holds the tokenizer, the parser, the static semantics, the lowering, and the three native encoders.
  `Broiler.VM.Profile.JavaScript` references Abstractions, Binary and the format and holds the verifier,
  the interpreter, the realm, the value model, the handlers of the two wide-manifest native forms and the
  descriptor. The compiler and the profile never reference each other; rule N3 keeps the format a sink.
- **Two bytecode formats under one verifier object.** Format version 1 (`JavaScriptOpcode`, the
  `broiler.javascript.slice` manifest, relative jumps, one frame) and format version 2 (`JsOpcode`, the
  `broiler.javascript.wide` and `broiler.javascript.numeric` manifests plus five declinable surfaces,
  absolute code offsets, a function table, environment records, exception regions). Both are one opcode
  byte followed by a fixed operand width, so an instruction boundary is computable from the opcode
  alone, and version 2's stack effects are a function of opcode and operand (`JsOpcodes.TryDescribe`).
  Rule N16 binds each format version to the manifest it is defined against and requires one verifier
  object to read both.
- **Three output forms, chosen by the compile request and pinned at verification.** The **bytecode
  form**, run by `JsEngine.ExecuteCore<TMode>`, a dispatch loop over heap frames of managed `JsValue`
  arrays. The **numeric form** over `broiler.javascript.numeric`, whose emitted code computes over
  pinned slabs of `double`, holds no managed reference, calls no helper, and refuses at compile time
  every construct outside the manifest ([`roadmap.backends.md`](../src/Broiler.VM.Profile.JavaScript/docs/roadmap.backends.md)
  section 3). The **baseline form** over `broiler.javascript.wide`, decided by JSD-0025, whose emitted
  code is control flow between instructions and one `call qword [rbx + opcode*8]` per instruction into
  a handler table of `[UnmanagedCallersOnly]` wrappers around per-opcode instantiations of the
  interpreter's own method body, with every value left in managed memory and the emitted frame holding
  a table address and a cookie. A fourth, the **value form** of JSD-0035, NaN-boxes values into words
  in a pinned slab with a generation-checked handle table; it was refused on 2026-09-25 on its own
  predeclared measurement and kept in the tree as an unadopted opt-in form.
- **The seam a native form attaches at is the finished bytecode, not the syntax tree.**
  `JsAssembledProgram` carries the whole code section, the function rows, the exception regions, the
  constants encoded rather than decoded, and the declared maxima; `IJsNativeBackend.TryEmit` answers
  with the whole artifact's emission or a refusal naming a reason, and nothing in between. The backend
  roadmap's section 2 argues why the seam sits there — a second walk of the tree would duplicate every
  hoisting and completion rule — and its section 4 records, as a route taken without a decision, that
  **the bytecode is the back-end-neutral intermediate form the plan's section 9 promised and the tree
  does not have**. JSB-3's exit gate asks for that sentence to be corrected or discharged by a dated
  decision, and it is still open.
- **Verification of a native payload is three layers**: structural framing; a template-closure scan in
  every image, which decodes the emitted bytes against a closed table of every instruction template the
  encoders emit and refuses a byte sequence that matches no template, two templates, or an operand
  outside the closed set its field admits; and re-emission equality where the encoder is in the image.
  The tables sit in the format assembly for the stated reason that an execution-only image must reach
  them and the encoders must too.
- **The arming path is no longer this profile's.** On 2026-09-18 the mapping and protection calls moved
  to `Broiler.VM.Profile.MachineCode` (`VmNativePage` and its two platform halves), and the JavaScript
  profile reaches them through a hook, `JsNativePage.Mapper`, that a composition root fills — because
  rule N2 forbids one family referencing another. Rules B5c and X1's register statements still name the
  JavaScript assembly and `JsNativePage`; the tests that enforce them read `Broiler.VM.Profile.MachineCode`
  and `VmNativePage`. The composition register says in its own words that no decision record explains
  that extraction.

### 2.2 The WebAssembly profile: one assembly, an interpreter over the wire format

- **One assembly, no format sibling, by its own roadmap's argument**: there is no compiler, the payload
  is a bare W3C binary module, and "a third project would be a boundary with nothing on either side of
  it" ([its section 5](../src/Broiler.VM.Profile.WebAssembly/docs/roadmap.md#5-package-boundaries-and-the-dependency-graph)).
  It references exactly Abstractions and Binary (rule W1) and declines the canonical-only
  variable-length readers of `Broiler.VM.Binary` because the W3C format admits padded encodings.
- **The decoder keeps function bodies as bytes and the validator walks them once**, sealing each body
  with its operand-stack bound, its label-depth bound and a jump table of block, loop and if boundaries.
  **The interpreter dispatches on the raw bytes** with an explicit program counter, reading immediates
  at run time, over a sixteen-byte untyped value slot (`WasmValue`: sixty-four bits of payload and
  sixty-four reserved for vectors, route MVP-2), heap frames, one shared operand stack, and traps as
  return codes rather than exceptions. Fuel is one unit per instruction; `CallDepth` is charged per
  frame; `memory.grow` charges pages, allocation and retention.
- **The opcode vocabulary is exactly the single-byte MVP set**, `0x00` to `0xBF`, one hundred and
  seventy-two members; every prefixed family, the reference instructions and the sign-extension
  operators are outside the surface and refused at validation as not admitted.
- **Its plan rules out everything this concept asks of it**: a compiler ("no Broiler WebAssembly
  compiler, no lowering assembly, and no compiler sibling"), a second execution arm, a format assembly,
  and any shared representation with the other profile (its section 17 restates the core's refusal in
  the core's words). Section 3 lists those as records this concept asks to move.
- **One observation the differential half of section 6 would have caught, recorded here because it
  is the shape of defect the concept's checks exist for.** The validator admits the twelve float
  comparisons (`0x5B` to `0x66`) and the interpreter's numeric dispatch routes every opcode from `0x45`
  to `0x8A` to its integer arm, which has no case for them; a module that compares two floats validates
  and then answers a defect at run time. The correct float-comparison code is in the file and reached
  from nowhere (`WasmInterpreter.cs`, `TryNumeric` and `FloatComparison`). This document does not fix
  it and does not move a ledger row for it; it names it because a family whose instruction table
  declares `wasm.f32.eq` as a primitive would have had that primitive checked against the family's
  handler on the day the table was written. The repair is written out as a suggested task in
  [`docs/tasks/fix-webassembly-float-comparisons.md`](tasks/fix-webassembly-float-comparisons.md).

### 2.3 The MachineCode profile: an arming path and an unreachable second artifact form

- **What it holds.** The arming path (`VmNativePage`, W^X, `SafeHandle`-owned release), a fixed-layout
  native frame that is field for field the JavaScript numeric form's `JsNativeFrame`, an artifact
  format of its own (`"BMC\0"`, fixed-width sections: limits, `double` constants, native code, symbols,
  entries), a verifier that checks that format's framing and never decodes an instruction, and an
  executor that arms the code and calls an entry through an unmanaged function pointer.
- **What reaches it.** Three JavaScript composition roots reference the project, and every one of them
  uses only the arming path, through the hook above. **No composition registers its descriptor
  (`broiler.machinecode`), so the BMC verifier and executor are reachable from nowhere.** The core's
  `IVmNativeCompiler` and `VmRuntime.CompileToMachineCode` exist and are exercised by one contract
  test with a stub; the one implementation, `JsNativeCompiler`, reads the JavaScript container with a
  different entry layout and a different native-header width than the writer and verifier use, writes
  the two `x86-64` convention codes the other way round from the enum it targets, and names a manifest
  (`broiler.machinecode.x86_64`) whose underscore the identity grammar refuses. Nothing in the tree
  calls it. Repairing or retiring it is written out as a suggested task in
  [`docs/tasks/repair-or-retire-js-native-compiler.md`](tasks/repair-or-retire-js-native-compiler.md),
  independent of this concept.
- **Why this matters to the concept.** The BMC path was a first attempt at what this document
  proposes — an output form as a component the language profile hands its bytecode to — and it shows
  the two things such a component cannot do without: a *verifiable* payload (the BMC verifier scans no
  instruction, so an execution-only image would trust provenance, which the JavaScript profile's own
  history on 2026-09-08 showed to be the four-zero-bytes defect), and the *bytecode beside the machine
  code*, which BMC drops and which the differential oracle and re-emission both need. The concept keeps
  the arming path and the frame and retires the second artifact format: a native form is an emission
  section inside the universal bytecode container, never a separate artifact.

### 2.4 The seams that already exist, and which of them the concept keeps

| Seam | Where | Kept? |
|---|---|---|
| Descriptor built by a static accessor, verifier and executor named directly | `JavaScriptProfile.Descriptor`, `WebAssemblyProfile.Descriptor` | **Kept as the core sees it**; built by an emitter from a family declaration (section 7.1). |
| Finished bytecode as the native seam, whole artifact or refusal | `JsAssembledProgram`, `IJsNativeBackend` | **Kept and generalised**: the universal bytecode is that seam for every language, and the two-answer rule becomes the emitter contract's. |
| Handler table in unmanaged memory, `[UnmanagedCallersOnly]` wrappers, activation cookie, thread-static activation slot, `Threw`/`Exit`/`Defect` statuses | `JsBaselineHandlers`, `JsNativeActivation`, `JsBaselineFrame` | **Kept as the family handler mechanism** for every family (section 7.3). |
| Template tables and template-closure scan in a pivot assembly | `JsNativeTemplates`, `JsNativeScan` | **Kept, moved** to the machine-code family's own pivot (section 4). |
| Re-emission equality through a descriptor carrying the encoder | `JavaScriptProfile.DescriptorReEmittingWith` | **Kept** (section 6.3). |
| Arming path through a hook the composition fills | `JsNativePage.Mapper` ← `VmNativePage` | **Retired**: the emitter's execution half owns the arming path directly and no language profile arms anything. |
| Core native-compilation seam | `IVmNativeCompiler`, `VmRuntime.CompileToMachineCode` | **Kept, re-read**: implemented by the native emitters, universal bytecode in and the same artifact with an emission section out (route UBC-R5). |
| Second artifact format for machine code | `"BMC\0"` | **Retired.** |
| Wire-format interpreter | `WasmInterpreter` over raw module bytes | **Retired as an executor**; its arms become the WebAssembly family's handlers, its validator stays. |

---

## 3. The rules this concept meets, and the ones it asks to move

**This section is the reason the document is long.** Every design choice below either fits a published
rule or asks one to move, and a concept that did not say which would be asking to be trusted rather
than read. The full list is Appendix F; this section argues the five that decide whether the concept
is admissible at all.

### 3.1 "Share mechanism, never share semantics" — met by construction for four of five halves, and reopened for the fifth

[Roadmap section 8](roadmap.md#8-sharing-between-profiles-without-a-lowest-common-denominator-core)
and [ADR 0011](adr/0011-source-level-profile-contract.md)'s standing-refusals table say a shared value
representation, frame layout or opcode set is "refused permanently; these are the semantics the core
exists not to own", and the roadmap's non-goals refuse "one universal opcode set, tagged value, or frame
ABI shared across languages". **This concept proposes a universal opcode set. It does not pretend
otherwise, and it does not pretend the standing refusal does not apply.** What it argues is narrower and
is stated in the refusal's own terms:

| What section 8 calls semantics | What this concept shares | Shared, or owned? |
|---|---|---|
| **Values** | Nothing. A `v` slot's representation, lifetime and meaning are the family's. `JsValue` stays a twenty-four-byte struct in managed arrays under JSD-0011 Rows 1 and 2; `WasmValue` is not used at all, because the WebAssembly family holds no `v`. The four numeric slot types are machine words, which is what a validated WebAssembly module already treats them as. | **Owned.** |
| **Frames** | The frame *mechanism* — a heap frame in the interpreter, a machine frame in a native form, a counted call depth, a captured frame for a suspension. The frame's *contents* are typed slots whose `v` half is the family's. No family reads a frame's layout; it reaches its values through an activation contract. | **Mechanism shared; layout opaque to the family.** This is the half the refusal names as "frame layout", and the concept's answer is that the layout is shared by nobody: the emitter owns it and the families never see it. |
| **Types** | Five slot types, four of them machine widths and one opaque. No language type. | **Mechanism.** |
| **Opcodes** | The *encoding*: one byte space, fixed operand widths, a prefix per family, a table schema every family fills in. The *common family*: control transfer, calls, locals, constants, stack shuffles, trap. The *meaning* of every family instruction stays in the family's handler. | **The encoding and the common family are shared. The meanings are owned.** This is the reopening. |
| **Syntax trees** | Nothing. | **Owned.** |

**The argument for reopening the "opcode set" refusal is the one ADR 0011 admits: a language-free
formulation has been found.** The refusal's own reopening clause reads "a language-free formulation is
found", and the record says reopening "produces a new dated verdict rather than an edit of the old
one". The universal bytecode names no language concept anywhere in `Broiler.VM.Ubc`: it names slot
widths, operand shapes, families as numbered slots an artifact declares, a handler signature, a
primitive table of machine operations, and a verifier walk over all of them. The JavaScript family's
instruction table lives in the JavaScript format assembly; the WebAssembly family's lives in the
WebAssembly assembly; the shared assembly holds the *schema* of a table and not a row of either. That is
what gate condition **G3** asks, and section 13's first stage is the record that files the verdict.

**Conditions G1, G2 and G4.** G1 — two product profiles already implement the behaviour in merged code
— holds on the concept's date for the behaviour actually being extracted: a stack-machine code walk
with height and target checking (`JsVerifier`, `WasmValidator`), a dispatch loop over heap frames with
per-instruction fuel (`JsEngine.ExecuteCore`, `WasmInterpreter`), an unwinding protocol (exception
regions in one, traps as return codes in the other), and, for one of the two, native emission over the
finished bytecode. G2 — compared from real merged code, not anticipated — is not met by this document
and is not claimed: it is what stage UBC-0's correspondence table is for, and the verdict is not filed
until that table exists. G4 — no profile-to-profile dependency — is met by the graph in section 4:
both families reference the shared assembly and neither references the other, rule N2 stays exactly as
it is, and an emitter references no family at all.

**What the reopening does not do.** It does not admit a shared value, a shared realm, a shared object
model or a cross-profile value channel. A `js.*` instruction and a `wasm.*` instruction never appear in
one unit, one artifact declares the families it uses and a composition declares the families it
composes, and the cross-profile boundary of section 5.13 is the core's boundary unchanged: a JavaScript
program reaches a WebAssembly instance through the embedder's seam and two host-boundary transits,
exactly as [the roadmap's cross-profile section](roadmap.md#the-cross-profile-boundary-and-why-it-is-not-an-extraction-question)
prices it.

### 3.2 Invariants 4 and 14, and VM-7's "the core learns no encoding" — met

**The core is untouched, and that sentence is checkable.** Rules B1 and B2 hold as they are:
Abstractions and Binary reference nothing, Runtime references Abstractions and Binary, and nothing in
the core references `Broiler.VM.Ubc`. Invariant 4 says the core "provides lifecycle and safety
contracts, not a lowest-common-denominator ISA", and the universal bytecode is neither a core interface
nor a lowest common denominator: it is a *union* with namespaces, in which a language's every
instruction survives with its own meaning, and the only thing the languages have in common is what a
stack machine has in common with itself. Invariant 14's "the core generates no machine code and knows
no encoding" holds literally — the encoders live in the machine-code family, the core carries their
bytes as it carries every other profile payload, and "the core owns only whether a composition may
execute the result" is still the composition register's native-execution column read by rule K5.

**Where this concept sits is one level below the core and one level above the languages, and that
level is new.** The three-package core stays exactly three. `Broiler.VM.Ubc` is a fourth Broiler.VM-owned
assembly that is not part of core contract version 1, not packable during the MVP, and referenced by
every language and emitter family. Whether it becomes a fourth package is a release decision this
document names in stage UBC-9 and does not take.

### 3.3 ADR 0011's promise P1 — the one place this concept is a reading, and it is recorded as a route

P1 says a profile's Broiler.VM reference set "is exactly {Broiler.VM.Abstractions, Broiler.VM.Binary}"
and that "the set is of Broiler.VM-owned assemblies". Every language family and every emitter family
under this concept references a third Broiler.VM-owned assembly, so P1's literal set moves. ADR 0011
is contract-bearing, and the amendment procedure that would change a contract-bearing record is
"written and currently unexecutable" while one person holds every role.

**Two readings exist and the concept takes the first without a decision.** The first reading is that
P1's set is the *consequence* of section 8's extraction verdict and moves with it: ADR 0011's own
extraction-record clause says a verdict "changes the core graph" and names "the resulting graph edges"
as part of the record, so a shared assembly opened by the gate joins the set by the gate's own
mechanism, and the 2026-08-31 editorial revision of P1 — which added the "Broiler.VM-owned" qualifier
without minting a version — is the precedent for recording that as an editorial revision. The second
reading is that P1's set is contract content and adding a member is an additive amendment that mints
core contract version 2, which the procedure cannot currently do, so the concept is `Blocked` naming
the core as the holder. **The concept takes the first reading, records it as route UBC-R1 in section
12, and names the ruling that settles it.** A ruling for the second reading does not unbuild anything:
it blocks the extraction record's filing, and everything built on it becomes work that waits on a mint
— which is the cost `docs/mvp.md` section 5 says a route carries.

### 3.4 "No second lowering, no second verifier, one verifier ever" — met, and stronger than before

- **One lowering.** The JavaScript lowering keeps one front end and acquires, in place of its three
  exits, exactly one: universal bytecode. Which form an artifact takes is decided *after* the lowering,
  by which emitter the composition hands the bytecode to, so the front end no longer knows that forms
  exist. That is [section 10](roadmap.md#10-where-compilation-lives)'s "a backend is a choice inside one
  lowering" taken one step further: the choice leaves the lowering altogether.
- **One verifier, ever.** Each language's descriptor names one verifier object, built by the universal
  bytecode from the family's hook and the composed emitters' form layers (section 6). A language profile
  no longer *has* a verifier of its own that could disagree with anything: its hook is called by the one
  walk and can only refuse, never admit. Two languages sharing one walk is the verification framework
  section 8 said to "extract later, do not predict", extracted on the second product verifier, which is
  when the roadmap said it would be.
- **Deterministic lowering** is required of both translators, and stage UBC-3 asserts it over the
  whole wide corpus before any emitter consumes a byte — which is the ordering JSB-1 asked for and did
  not get.

### 3.5 The WebAssembly plan's own refusals — asked to move, each by name

The WebAssembly roadmap refuses a compiler, a lowering assembly, a format sibling, a second execution
arm, and any framing of the module. Under this concept the WebAssembly profile gains a **translator**
from a validated module to universal bytecode, the module becomes **source** the profile compiles at
verification rather than the artifact the core verifies, and the executor is the emitter's. Every one
of those is a dated correction in that profile's `roadmap.corrections.md` (a `WAC-nn` each) and an
amended non-goal, filed by stage UBC-4 and not before; the reasons are section 8.2's, and the cost —
a translation on every load, and a harness that feeds modules through a translator before scoring
them — is section 11's.

---

## 4. The dependency graph after the refactoring

```text
Broiler.VM.Abstractions                    ──→ (nothing)                                   [core, packable]
Broiler.VM.Binary                          ──→ (nothing)                                   [core, packable]
Broiler.VM.Runtime                         ──→ Abstractions + Binary                       [core, packable]

Broiler.VM.Ubc                             ──→ Abstractions + Binary                       [NEW: the universal bytecode —
                                                                                              format, common family, table
                                                                                              schema, primitive table, verifier
                                                                                              walk, family and emitter contracts;
                                                                                              not packable during the MVP]

Broiler.VM.Profile.JavaScript.Format       ──→ Ubc                                         [the js.* instruction tables per
                                                                                              manifest, the constant-pool codec,
                                                                                              the family sections' schema]
Broiler.VM.Profile.JavaScript.Compiler     ──→ Abstractions + Ubc + JavaScript.Format      [source → UBC; no encoder]
Broiler.VM.Profile.JavaScript              ──→ Abstractions + Binary + Ubc + JavaScript.Format
                                                                                           [family runtime: realm, values,
                                                                                              handlers, hook, payloads,
                                                                                              declaration; no loop, no walk,
                                                                                              no arming]
Broiler.VM.Profile.WebAssembly             ──→ Abstractions + Binary + Ubc                 [decoder, validator, translator,
                                                                                              wasm.* tables, store, memory,
                                                                                              handlers, hook, payloads]

Broiler.VM.Profile.Bytecode                ──→ Abstractions + Binary + Ubc                 [emitter profile 1: the interpreter
                                                                                              and the bytecode-form executor]
Broiler.VM.Profile.MachineCode.Format      ──→ Ubc                                         [the machine-code family's pivot:
                                                                                              frames, conventions, template
                                                                                              tables, the scan]
Broiler.VM.Profile.MachineCode             ──→ Abstractions + Binary + Ubc + MachineCode.Format
                                                                                           [emitter profiles 2 and 3, execution
                                                                                              half: arming path, native
                                                                                              activation, form verifier layers,
                                                                                              native executor]
Broiler.VM.Profile.MachineCode.X64         ──→ Ubc + MachineCode.Format                    [emitter profile 2, emitting half]
Broiler.VM.Profile.MachineCode.Arm64       ──→ Ubc + MachineCode.Format                    [emitter profile 3, emitting half,
                                                                                              emitting-only]

src/compositions/*                         ──→ the three core packages + the families an image composes
                                                (Ubc and the pivots arrive transitively, as the
                                                 JavaScript format does today, and are declared in the
                                                 register's sibling column)
```

**Six properties of this graph, each of which a rule holds or will hold:**

1. **The core references nothing new** (B1, B2 unchanged), and nothing in the core names a universal
   bytecode type. The core's public API baseline (`docs/api/public-api.txt`, rule M1) does not move.
2. **No family references another family, in either direction** (N2 unchanged). The JavaScript,
   WebAssembly, Bytecode and MachineCode families are four `Broiler.VM.Profile.<Segment>` families to
   rules A11 and N2, keyed on the segment, exactly as JavaScript and MachineCode are two families
   today. An emitter family never references a language family: it receives a family's tables and
   handlers *as values* at composition, through the contracts in `Broiler.VM.Ubc`.
3. **Every family references `Broiler.VM.Ubc`, and `Broiler.VM.Ubc` references only the two core
   sinks.** It is a sink of the shared graph the way `Broiler.VM.Binary` is: it holds format, schema and
   mechanism and no language concept, which rule U2 (stage UBC-1) asserts by scanning its exported
   identifiers against a banned vocabulary.
4. **The JavaScript format assembly stops being a sink of the whole graph and stays a sink of its
   family**: it references `Broiler.VM.Ubc` and nothing else (rule N3 revised to say exactly that). The
   reason it exists is unchanged — the lowering and the runtime library must agree on the family's
   tables without depending on each other.
5. **The machine-code family acquires the shape the JavaScript family has, for the reason the roadmap
   gives for that shape**: there are two parties, an encoder at compile time and a verifier-plus-executor
   at run time, that must agree on templates and frames without referencing each other, so a pivot holds
   the tables. The template-closure scan moves from the JavaScript format assembly into
   `Broiler.VM.Profile.MachineCode.Format`, where an execution-only image reaches it and an encoder does
   too.
6. **A composition root's reference list is still "the three core packages plus one or more profile
   assemblies"** (A12), because every family assembly matches the profile pattern; `Broiler.VM.Ubc` and
   the two pivots are siblings in the register's sense and appear in the sibling column, as the
   JavaScript format does today.

**Project and edge budget.** ADR 0001's last budget sentence authorises the graph the checkout holds
("from 27 projects and 88 edges to 27 and 90"). This concept adds four product projects
(`Broiler.VM.Ubc`, `Broiler.VM.Profile.Bytecode`, `Broiler.VM.Profile.MachineCode.Format`,
`Broiler.VM.Profile.MachineCode.X64`) and one emitting-only project
(`Broiler.VM.Profile.MachineCode.Arm64`), one application-local fixture family under `src/tests/` for
invariant 13 (section 8.3), and the reference edges the diagram shows; each arrival is a dated revision
of ADR 0001 with the new counts, and rules A7 and A15 fail until `graph.manifest.json` and the record
say the same thing. **The counts are not stated here as figures because they are not this document's
to fix**: a stage that adds a project writes the revision, and a revision written in advance of the
project would be a budget authorising something that does not exist.

---

## 5. The universal bytecode

### 5.1 Principles, each of which is a rule a stage's gate can decide

1. **An instruction boundary is computable from the tables alone.** Every instruction is an optional
   family prefix, one opcode byte, and a fixed-width operand whose shape the family's table declares.
   No operand is variable-length, no immediate is decoded before validation, and a jump target is
   checked against the boundary set the way both JavaScript format versions check it today. The
   WebAssembly translator re-encodes every LEB immediate into a fixed width, which is what removes the
   canonical-against-padded disagreement between that profile and `Broiler.VM.Binary`: the universal
   bytecode is Broiler's own format and every integer in it is canonical.
2. **Control flow is flat, with absolute targets and a region table**, as JavaScript format version 2
   has it: `jump`, two conditional jumps over an `i32`, a jump table, `call`, `return`, `trap`, and
   exception regions as rows of a section rather than as instructions. Structured control flow is the
   WebAssembly translator's to lower, and it already knows every label's target and arity, because
   its validator computes them.
3. **The operand stack is typed, statically, per position.** The verifier's abstract state at every
   pc is a vector of slot types, as the WebAssembly validator's is, with one type the validator does
   not have: `v`. A join point requires equal vectors. Unreachable code is refused — an instruction no
   path reaches is a defect in the translation — which is stricter than the WebAssembly grammar and
   exactly as strict as the JavaScript verifier is today; the WebAssembly translator drops dead code.
4. **Meaning is owned; execution is delegated.** The common family's instructions have one meaning
   the universal bytecode fixes and every emitter implements. A family instruction's meaning is its
   handler's; an emitter reaches it by calling the handler, or by implementing a primitive the family
   named for it, and never by knowing what the instruction is.
5. **One form per artifact, whole.** An emitter's two answers are the whole artifact and a refusal
   naming a reason. A program a guest loads is compiled in its instance's form or is an internal defect.
   There is no guard, no bail-out, no promotion and no tier.
6. **Everything is deterministic**: one source, one translator version, one universal bytecode format
   version, one artifact, byte for byte; and one artifact, one emitter version, one emission, byte for
   byte. Re-emission equality and the differential oracle both rest on it.
7. **Fuel is exact per instruction at the same point in every form**, so a budget verdict is a
   property of the program and not of the form that ran it. JSD-0025 section 5's exactness claim is
   carried over as a universal rule, with the one divergence class it names.
8. **Emitted code holds no managed reference, by construction and not by scheme.** The word plane is
   the only thing emitted code addresses; the value plane and every family object stay in managed
   memory reached through an activation the collector roots by ordinary means. MVP-3 and JSD-0025
   section 4 are this rule's precedents and its falsifiers apply unchanged.

### 5.2 The container

A universal bytecode artifact is one byte string, framed with the bounded readers of
`Broiler.VM.Binary`, all integers canonical variable-length unless a field says fixed-width. Appendix
E gives every field; this is the shape:

| Order | Section | Owner | Holds |
|---|---|---|---|
| — | **Header** | UBC | magic `"BUBC"`; universal bytecode format version; the profile identity and feature manifest identity of the language the artifact was lowered from; the **form**: `bytecode`, or a form identity and emitter version when an Emission section follows; the translator's identity and version. |
| 1 | **Families** | UBC | One row per family the code uses: slot index (the prefix byte the code will use, see 5.4), family identity, family table version, and the manifest the family's table is selected by. Slot 0 is always the common family and is not written. |
| 2 | **Types** | UBC | Signatures: parameter slot types and result slot types, deduplicated, referenced by index. |
| 3 | **Units** | UBC | One row per code unit: signature index, local slot types (as a run-length list), declared maximum operand height, code range, flags (`Suspendable`, `Entry`, family-defined flag bits above bit 8), and the family slot every family instruction in the unit belongs to — a unit is single-family. |
| 4 | **Code** | UBC | Every unit's instructions, back to back, at the offsets the unit rows declare. |
| 5 | **JumpTables** | UBC | For `jump_table`: rows of absolute targets, last one the default. |
| 6 | **Regions** | UBC | Exception regions: unit, start, end, handler offset, entry height (word plane), entry height (value plane), region kind byte (family-defined), nesting order. |
| 7 | **Entries** | UBC | Named entry points: UTF-8 name → unit index, for the core's `VmInvocationRequest`. |
| 8 | **Positions** | UBC | The diagnostic position table, in the core's own position record, unit and offset ranged. |
| 9+ | **FamilyData** *(one per family slot, in slot order)* | the family | Opaque to the universal bytecode: framed by it, read by the family's hook and runtime. JavaScript: the constant pool, scope maps, module records, script declarations, eval scopes, surfaces. WebAssembly: function types, memory and table definitions, globals with their constant initialisers, data and element segments, export names. |
| last | **Emission** *(present in a native form only)* | the emitter | Form identity, emitter semantic version, alignment, the emitted bytes, one symbol row per unit, and the family handler-table shape the bytes were emitted against. |

**Three properties of the container are load-bearing.** The universal bytecode reads every section
it owns and never opens a FamilyData section; a family's hook reads its own FamilyData section and
never another family's; and an Emission section is verifiable without any encoder in the image, by
the form verifier layer of section 6.3. The bytecode beside the emission is not a fallback — no
executor path chooses between them — and it is required, because the differential oracle compares
the two forms of one artifact and re-emission recompiles the carried bytecode.

**Format version.** The universal bytecode has one format version, starting at 1, distinct from every
family table version, every emitter version and the core contract version. A family table version
moves when a family's instruction rows change; the universal bytecode version moves only when the
container, the common family, the slot types, the operand shapes or the table schema change. Rule N16's
pairing principle holds one level up: a family table version is defined against the manifest it is
selected by, and a family table admitted under one manifest is refused under another.

### 5.3 Slot types and the two planes

| Slot type | What it is | Where it lives in the interpreter | Where it lives in a native form |
|---|---|---|---|
| `i32` | thirty-two-bit two's-complement word | an element of the word plane, a `ulong[]` | a general register or a native stack slot |
| `i64` | sixty-four-bit two's-complement word | same | same |
| `f32` | IEEE-754 binary32, held as its bits | same | an SSE or NEON register, or a native stack slot |
| `f64` | IEEE-754 binary64, held as its bits | same | same |
| `v` | a **language value**: whatever the family says it is | an element of the family's **value plane**, an array of the family's own value type | **nowhere in emitted code**: an index into the family's value plane, which the emitter passes to handlers and to plane helpers as an integer |

**The two planes share one abstract height and one static type vector.** Position *k* of the operand
stack is either a word or a value, the verifier knows which at every pc, and the emitter keeps the two
planes as two storages with one bookkeeping. A `v` position's plane index is fixed per (unit, pc,
position) by the verifier's walk, so an emitter can address the value plane by constant offset from the
activation's plane base without ever computing it at run time — which is what lets emitted code move a
`v` between positions through a plane helper (`copy from-index to-index`) and never through a register.

**The value plane is the family's array and the family's type.** For the JavaScript family it is a
managed `JsValue[]` — the interpreter's own operand stack today — so JSD-0011 Row 2's "every
value-holding region is a managed array of the value struct, no manual rooting, no handle table, no
finalizer, no `GCHandle`" holds word for word. For the WebAssembly family there is no value plane at
all: its five slot types are the four numeric ones and its table declares no `v` instruction, so a
WebAssembly unit's entire operand stack is words and a native form of it holds every operand in a
register or a native slot. The fixture family of section 8.3 has a value plane of one trivial type, to
prove the contract without a language.

**Locals are typed slots too**, declared per unit, held in the same two planes.

### 5.4 Instruction encoding

- **Opcode byte space.** `0x00` to `0xEF` are the common family's opcodes, unprefixed. `0xF0` to
  `0xFE` are **family prefixes**: `0xF0 + s` introduces one instruction of family slot *s* (1 to 15) of
  the artifact's Families section, followed by the family's opcode byte and its operand. `0xFF` is
  reserved for an extended prefix carrying a sixteen-bit family slot and is not defined by format
  version 1; an artifact using it is refused. Fifteen families per artifact is not a limit anything in
  this repository approaches, and it is a limit rather than an assumption.
- **Family opcode numbers are the family's.** The JavaScript family keeps the byte values of
  `JsOpcode` where an instruction survives the mapping, so `js.add` is `0xF1 0x40` in an artifact whose
  slot 1 is `broiler.javascript` — a reader who knows format version 2 reads the second byte as they
  always did. The WebAssembly family keeps the W3C byte values for the instructions that stay in the
  family, so `wasm.i64.add` is `0xF1 0x7C` in an artifact whose slot 1 is `broiler.webassembly`.
  Nothing depends on this; it is a courtesy to readers and to diff tools.
- **Operand shapes.** A closed enumeration, each of fixed width, little-endian: `None`, `U8`, `U16`,
  `U32`, `I32`, `I64`, `F32`, `F64`, `U8U8`, `U8U16`, `U8U32`, `U16U16`. A family may use any; it may
  not mint one. `U8U8` is the common family's `squash` shape; `U8U16` is the JavaScript family's
  depth-and-index shape and the common `trap`'s; `U8U32` is the WebAssembly family's
  alignment-and-offset shape; `U16U16` is reserved for a family that needs two indices.
- **Effect descriptors.** A table row states the instruction's stack effect as a small closed
  language, so the verifier computes heights and types from the row and not from a family method that
  could disagree with the handler: `pops: [types…]`, `pushes: [types…]`, or `pops: operand + k` for the
  instructions whose effect depends on the encoded count, the shape `JsOpcodes.TryDescribe` already
  has. A row whose effect is not expressible in that language is not admitted, which is deliberate.
- **Target descriptors.** A row states whether its operand is a code target, and if so, the height
  adjustment on the taken edge — the two-effect shape `IterateNext` has today, where the fall-through
  pushes one and the taken edge pushes none.

### 5.5 The common family

The common family is what every emitter implements itself, with no family callback and one fixed
meaning. It is deliberately small, and the test for membership is stated so that it can be argued
about: **an instruction is common only if its meaning is fully stated by the universal bytecode without
reference to any language's specification, and if it would mean the same thing in a third language
nobody has planned.** Appendix A is the whole table; the groups are:

| Group | Instructions | What it is for |
|---|---|---|
| Nothing | `nop` | a hole in a patched stream is a defect; `nop` exists so that a translator never needs one |
| Termination | `trap u8 u16` | a fault the family names: the `u8` is the family slot whose payload will describe it, the `u16` a code the family's table defines; `unreachable` is `trap` with code zero |
| Control | `jump u32`, `jump_if_zero u32`, `jump_if_nonzero u32`, `jump_table u16` | flat control flow over an `i32` condition; the JavaScript `ToBoolean` branch is a family instruction with a target, not a common one, because its condition is a `v` |
| Calls | `call u32`, `return` | a direct call to a unit of the same artifact, signature-driven; a return of the unit's declared results from the top of the stack, discarding what is beneath |
| Stack | `drop`, `dup`, `dup2`, `swap`, `pick u8`, `select`, `squash u8 u8` | shuffles over either plane; `squash n k` keeps the top *k* and discards the *n* beneath them, which is how a WebAssembly branch with results unwinds to its label's height |
| Locals | `local.get u16`, `local.set u16`, `local.tee u16` | typed by the unit's local table |
| Constants | `const.i32 i32`, `const.i64 i64`, `const.f32 f32`, `const.f64 f64` | word constants; a `v` constant is a family's, because only the family knows what a pool entry means |

**What is deliberately not common, and why.** No arithmetic, no comparison, no conversion: the request
places `i64add` with WebAssembly, and section 10 records the alternative — a common typed-arithmetic
group — and why it is not taken. No memory access: a region is a family's. No `throw` and no `catch`:
an exception is a `v`, and what lands at a handler is the family's to say (5.10). No suspension: the
mechanism is the emitter's and the instruction is the family's (5.11). No fuel or poll instruction:
charging is implicit and table-driven (5.12).

### 5.6 Family instruction tables: the schema

A family is registered into a composition as a value of one type, `UbcFamilyRegistration`, that carries
its identity, its declaration (the descriptor rows the core needs — 7.1), and **one instruction table
per feature manifest the family admits**. A table is an immutable array of rows indexed by the family's
opcode byte, and a row states:

| Field | Meaning | Who reads it |
|---|---|---|
| `Mnemonic` | for diagnostics and disassembly, e.g. `js.get_property`, `wasm.i64.add` | tools, corpus files |
| `Shape` | the operand shape, from 5.4's closed set | verifier, emitters |
| `Effect` | the stack effect descriptor, from 5.4's closed language | verifier, emitters |
| `Target` | none, or "the operand is a code target" with the taken-edge adjustment | verifier, emitters |
| `Kind` | one of the closed set below | verifier, emitters |
| `Primitive` | for `Kind = Primitive`: which entry of Appendix D, and for a region primitive, which family region | emitters |
| `Cost` | fuel charged before the instruction's effects; default one | emitters |
| `Traps` | for a primitive: which trap codes it may raise | verifier (the codes must be ones the family's table defines), emitters |
| `Handler` | the family's handler for this opcode: a static method of a fixed signature (7.2) | the interpreter directly; the native emitters through the handler table |

**The kinds**, closed:

| Kind | What the emitter does | Examples |
|---|---|---|
| `Dynamic` | calls the handler; the handler answers a status | `js.get_property`, `js.add` under the wide manifest, `wasm.memory.grow` |
| `Primitive` | may implement the row's primitive inline over the word plane instead of calling the handler; must call the handler where it does not | `wasm.i64.add`, `wasm.f64.sqrt`, `wasm.i32.load`, `js.add` under the numeric manifest |
| `Branch` | calls the handler, which answers taken or not; the emitter transfers to the row's target or falls through | `js.jump_if_false`, `js.iterate_next`, `js.for_in_next` |
| `Call` | calls the handler, which answers a **call request** — a unit index of the same program and an argument layout — or completes without one; the emitter performs the frame push (5.9) | `js.call`, `js.construct`, `wasm.call_indirect` |
| `Suspend` | calls the handler, which may answer **suspend**; the emitter captures the frame (5.11) | `js.yield`, `js.await`, `js.enter_body` |
| `Throw` | calls the handler, which answers **threw**; the emitter unwinds (5.10) | `js.throw`, `js.throw_immutable` |

**The authority rule.** For every row of every kind the handler is the meaning. A `Primitive` row's
inline implementation is an emitter's optimisation, admitted only because the primitive table's entry
is a total function over machine words with a stated result for every input, and stage UBC-6's gate
requires an emitter's inline implementation of every primitive it uses to answer, bit for bit, what the
family's handler answers over a retained input corpus — including every NaN, every signed-zero, every
overflow and every trap edge. A family that classifies an instruction as a primitive is asserting that
its handler *is* that function; a handler that is not — because the language says something the
primitive does not — is the family's defect, and the differential check is what finds it.

**Per-manifest tables are how a form becomes a property of a manifest.** `js.add` under
`broiler.javascript.wide` is `Dynamic`; under `broiler.javascript.numeric` — whose front end refuses,
by name, every construct whose value is not a Number — it is `Primitive f64.add`. That is the numeric
form's computing templates recovered without a form of their own: the emitter reads a table row, not a
manifest name, and the JavaScript profile's decision that a numeric program admits only Numbers is
written where it always was, in the front end and the manifest.

**The primitive table** (Appendix D) is the closed set of operations an emitter implements: integer
arithmetic, comparison and bit operations at two widths; floating-point arithmetic, comparison and
rounding at two widths; the conversions; region loads and stores at every width with sign and zero
extension; region size. A family's table states whether its floating-point primitives **canonicalise
NaN results**, because the WebAssembly profile implements the specification's deterministic profile and
must, and the JavaScript profile's Numbers observe no NaN payload and need not; an emitter honours the
flag, at a cost the WebAssembly family pays and the JavaScript family does not. **The primitive table
names no language**: its entries are what a processor does, stated in IEEE-754 and two's-complement
terms, and a reader who thinks it is a shared opcode set under another name should read section 10's
row on it and disagree there.

### 5.7 The JavaScript family, `js.*`

Appendix B maps every member of `JsOpcode` and `JavaScriptOpcode` to the universal bytecode. The
shape of the mapping:

- **What becomes common.** `Nop`; `Jump`; `Return` (the unit's one result is a `v`);
  `ReturnUndefined` (lowered as `js.load_undefined` followed by `return`); `Pop`, `Duplicate`,
  `DuplicateTwo`, `Swap`, `Pick` (plane shuffles). Format version 1's `LoadLocal` and `StoreLocal`
  become common locals of type `v`; its relative jumps become absolute ones.
- **What stays, with a target.** `JumpIfFalse`, `JumpIfTrue` (`ToBoolean` is the language's),
  `ForInNext`, `IterateNext`, `IterateAwaitStep`, `IterateCloseAsync`, `DisposeStep` — kind `Branch`,
  each with the taken-edge adjustment its `TryDescribe` comment states today.
- **What stays as a call.** `Call`, `CallEval`, `Construct`, `SuperCall`, `SuperCallForwarded`,
  `CallSpread`, `CallEvalSpread`, `ConstructSpread`, `SuperCallSpread`, `ImportCall` — kind `Call`. The
  handler resolves the callee; when it is a script function of the same program it answers a call
  request naming the unit and the emitter pushes the frame; when it is a realm intrinsic, a host
  function, a bound function or a proxy trap, the handler completes the call itself, exactly as the
  interpreter's arm does today.
- **What stays as a suspension.** `Yield`, `YieldDelegate`, `Await`, and `EnterBody` as the seam a
  generator's parameter-binding prologue suspends at.
- **What stays as a throw.** `Throw`, `ThrowImmutable`, and format version 1's
  `ThrowUninitializedBinding` guard.
- **Everything else stays `Dynamic`**: literals, bindings, environment records, `with`, objects and
  properties, closures, classes, private names, spread, template objects, iteration protocol steps,
  modules, direct-eval names, resource scopes, the operators. Under the numeric manifest the
  arithmetic, comparison and unary rows the numeric form's computing templates cover become
  `Primitive` rows over `f64` (Appendix B says which); remainder, exponent and the bitwise operators
  stay `Dynamic` under every manifest, which is what the `x86-64` numeric form refuses today, now
  expressed as a table row rather than a refusal.
- **The surfaces.** The five declinable surfaces (`bigint`, `binary`, `dynamic`, `modules`, `native`)
  are family data a composition declines at verification, as today; `native` is no longer a surface but
  the artifact's form, declined by a composition that composes no native emitter.
- **Value representation, unchanged**: `JsValue` in managed arrays, the realm, the handle-free rooting
  of JSD-0011. **The value form of JSD-0035 is not carried over**: a NaN-boxed word as the universal
  `v` is section 10's row, and JSD-0011 Row 1's "registered and not adopted" stands.

### 5.8 The WebAssembly family, `wasm.*`

Appendix C maps every member of `WasmOpcode` to the universal bytecode. The shape:

- **What becomes common.** `unreachable` (→ `trap`), `nop`, `br` (→ `jump`, with a `squash` before
  it where the label's height differs), `br_if` (→ `jump_if_nonzero`, with the adjustment on a
  trampoline when needed), `br_table` (→ `jump_table`, likewise), `return`, `call`, `drop`, `select`,
  `local.get/set/tee`, the four constants. `block`, `loop`, `if`, `else` and `end` produce no
  instruction: the translator resolves every label to an absolute offset from the validator's jump
  table, lowers `if` to `jump_if_zero`, and drops the dead code the grammar allows after an
  unconditional transfer.
- **What stays as primitives.** Every numeric, comparison and conversion instruction — one hundred and
  twenty-three rows — as `Primitive` rows over Appendix D, the trapping ones (`div_s`, `div_u`, `rem_s`,
  `rem_u` at both widths; the eight `trunc` conversions) with their trap codes; every load and store as a
  region primitive over the family's `memory0` region, bounds-checked, with the trap code for an
  out-of-bounds access; `memory.size` as the region-size primitive; `global.get` and `global.set` as
  region primitives over the family's `globals` region.
- **What stays `Dynamic`.** `memory.grow` (allocation, charging, the guest-observable refusal value that
  route MVP-1 records). **What stays a call.** `call_indirect`, whose handler reads the table, checks the
  signature, and answers a call request.
- **What the family owns beyond instructions.** The store, the linear memory (a managed `byte[]` today;
  a region an emitter can address natively must be pinned or native, which is section 8.2's decision),
  the table, the globals, the segments, the trap kinds and their payloads, and the specification's
  deterministic profile including NaN canonicalisation.
- **The W3C module is source, not the artifact.** Decoding and validation are the profile's front end,
  performed once when a module is compiled to universal bytecode; the artifact the core verifies is the
  universal bytecode. That is the reversal section 3.5 names, and section 8.2 says what it costs.

### 5.9 Frames, calls and returns

- **Frames are the emitter's.** In the interpreter a frame is a heap object owned by the operation,
  the dispatch loop is one CLR frame regardless of guest depth, and `CallDepth` is a counted number
  compared against a bound — JSD-0011 Row 4's properties, now for every family. In a native form a
  frame is a machine frame with the activation cookie, and a call between units of the same program is
  a machine call with the same counted depth (the mechanism JSD-0035's stage JSV-3 built).
- **`call u32` is signature-driven.** The callee's Units row says how many slots of which types are
  taken from the top of the caller's stack into the callee's locals and how many come back; word
  parameters move by copy, value parameters move by plane copy from the caller's window to the
  callee's. That is the whole of a WebAssembly direct call.
- **A family call is a call request.** A `Call`-kind handler answers either *completed* (it did the
  call itself — a host function, an intrinsic, a proxy — and its results are on the stack) or a
  *request*: the unit index, and how the callee's parameters are to be found. The emitter performs the
  request exactly as it performs `call`, so the family never pushes a frame, never reads the call
  depth and never holds a return address. For the JavaScript family the request also names the
  environment the callee closes over, which is a `v` the handler placed in the value plane.
- **`return` discards what it does not return**, so a translator need not clean the stack before it
  and the WebAssembly `return`-from-inside-a-block lowers to one instruction.
- **`CallDepth` is charged by the emitter on every frame push**, for every family, in one place, which
  closes the risk row the roadmap has carried since 2026-08-31 — "`CallDepth` is declared by every
  profile and charged by no code in this repository" — for the JavaScript profile too, once its calls
  go through the emitter's frame.

### 5.10 Exceptions, traps and unwinding

- **A trap is a `trap` instruction or a primitive's trap outcome.** Either way the emitter answers
  the family's fault-payload factory with the family slot and the code, the family builds its typed
  payload (`WebAssemblyTrap` with its kind and position), and the step ends `Faulted`. No region
  catches a trap: a trap is an abort by definition, and the WebAssembly surface here has no exception
  handling.
- **An exception is a `v` in flight.** A `Throw`-kind handler answers *threw* with the value held in
  the family's activation state, never in emitted code. The emitter searches the current unit's Regions
  rows for the pc, innermost first. **If a region covers it**, the emitter truncates both planes to the
  region's entry heights, jumps to the handler offset, and calls the family's `OnLand(region kind)`
  before the first handler instruction runs; the family pushes what its lowering expects — the exception
  for a catch region, a completion record for a finally region — so the region kind byte is the
  family's and the landing mechanism is the emitter's. **If no region covers it**, the frame is popped,
  `CallDepth` released, and the search continues in the caller's unit; at the entry unit the step ends
  `Faulted` with the family's uncaught payload.
- **Filters are pure, and that is now a universal rule rather than one profile's audit.** JSD-0025
  section 11 names an impure catch filter as a falsifier because the native form runs inner `finally`
  blocks before outer filters; under this concept the emitter runs one unwinding algorithm in every form,
  so the orders agree by construction, and the rule survives as the simpler one: a family's `OnLand` may
  not observe or change anything another region's landing could.

### 5.11 Suspension

- **A `Suspend`-kind handler answers *suspend*** with a family-owned continuation object as the
  reason (the generator object, the promise reaction). The emitter captures the frame — the word plane
  window, the value plane window as plane indices, the locals, the pc, the region nesting — into that
  object through the family's frame codec (`CaptureFrame`, `RestoreFrame`), and the step's answer
  travels outward as the family says: a generator's `next` returns to guest code; an `await` parks the
  frame until the job queue resumes it; nothing crosses the core's suspension boundary unless the family
  declared external suspension, which neither family does.
- **In the interpreter the codec is trivial**: the heap frame is the continuation. In a native form it
  is JSD-0035's stage JSV-4 mechanism — resident state written to the family's records and read back at
  the resumption — with the difference that the only resident state is the word plane, because `v`
  slots were never resident.
- **Resumption re-enters the unit at a landing offset the Units row declares**, one of the statically
  known set the baseline form's compare tree already dispatches over.

### 5.12 Fuel, polls and cost

- **Every instruction charges its row's cost before its effects, at the same point in every form.**
  Common rows cost one; a family row's cost is its table's, default one. A primitive charges exactly
  what its handler would, because the charge precedes the choice between them.
- **Polls are the emitter's**, at the family's declared `MaxUnchargedWork` and `CancellationPollBound`,
  which the family declaration carries into the descriptor unchanged.
- **The one divergence class is JSD-0025's**: a program a guest loads is a larger payload in a native
  form than in the bytecode form, so verification work and the guest-load byte bound can exhaust at
  different points. The forms comparison excludes guest-loading programs by name, as the JavaScript
  profile's checks do today.
- **Proportional charging stays the family's** (`memory.grow`, segment initialisation, `br_table`'s
  label vector — the roadmap's CO-1): a family row that grows with its input declares its cost function
  in its handler and states it in its own plan; the universal bytecode charges the row's fixed cost and
  the handler charges the rest.
- **Debt settlement is not taken.** JSD-0035's fuel-as-debt for inline instructions was part of a
  refused form; the primitive path charges at the instruction like every other row.

### 5.13 Guest loads, host capabilities and the cross-profile boundary

- **Guest-initiated loads are unchanged in mechanism and in rule.** A family runtime that declares
  them asks the mediator for bytes, the composition's provider answers with a universal bytecode
  artifact of the same language compiled in the instance's form, the same descriptor's verifier verifies
  it, and the nested instance runs in the same emitter. A provider that answered with another language's
  artifact is the contract breach it is today.
- **Host capabilities are the family's imports**, carried in the family declaration into descriptor
  row 17 unchanged, invoked from handlers through the bound table by index as today.
- **The cross-profile boundary is the core's, unchanged.** No unit mixes families, no artifact of one
  language calls a unit of another, and a JavaScript program reaches a WebAssembly instance through the
  embedder's host object and two host-boundary transits, bounded by aggregate `CallDepth` only where
  both runtimes share a parent. **Composing two families into one emitter does not open a value
  channel between them, and this document refuses one in the same words the roadmap does.** A `v` of
  the JavaScript family and an `i32` of the WebAssembly family never occupy one frame, and the only
  place they could be made to is a composition root that chose to write the conversion — at the
  embedder's seam, where its price is visible.

---

## 6. Verification: one verifier, three layers

**One `IVmProfileVerifier` per language descriptor, built by `Broiler.VM.Ubc`, whose verifier
semantic version is the universal bytecode's.** It runs three layers in a fixed order and a refusal in
an earlier layer is never reached past.

### 6.1 The universal walk

Structural first: magic, format version, the header's identities against the descriptor, section
order and uniqueness, every length inside the payload, every count against the ceilings, the Families
section against the families the composition composed — **an artifact naming a family the image does
not compose is refused at verification, by name, with an invalid-artifact reason, before any
instruction of it is reachable**, which is the property the JavaScript surfaces already have. Then the
code walk, per unit: instruction boundaries from the tables; every family opcode defined in the
selected table; every operand in range of its section (a unit index, a type index, a jump-table row, a
local index against the unit's local table); every target on a boundary inside the unit; the typed
abstract stack from the effect descriptors, with equal vectors at every join and the taken-edge
adjustment applied at the target; height never above the unit's declared maximum; a terminal
instruction ending every path; no unreachable instruction; regions well nested, their offsets on
boundaries, their entry heights consistent with the walk; every `Suspend` row only in a unit flagged
`Suspendable`; every `Call` and `call` signature consistent with the Units and Types sections. Every
refusal maps to exactly one core reason and one universal diagnostic code, and is reachable from a
retained corpus entry (the corpus discipline VM-2 set and both profiles copied).

### 6.2 The family hook

At every family instruction and over every FamilyData section the walk calls the family's
`IUbcFamilyVerifier`, which **may refuse and may not admit**: it checks what only the family knows —
that a `js.load_constant` operand names a pool entry whose tag the manifest admits, that a
`js.load_scoped` depth does not exceed the scope map, that a `wasm.i32.load` is used only where the
family data declares a memory, that a `wasm.call_indirect` type index exists — and answers with a
family diagnostic code mapped to a core reason under the family's registry rules (rules N5 to N8 for
the JavaScript family, their WebAssembly counterparts for the other). The hook receives the walk's
typed state at the instruction, read-only. It is charged to the same verifier-work budget as the walk,
through the same meter, and it is not a second verifier: it cannot run without the walk, it cannot
contradict a refusal, and it is versioned as part of the family table it belongs to.

### 6.3 The form layers, for a native form

Absent for the bytecode form. For an artifact with an Emission section, the composed emitter's
`IUbcFormVerifier` runs after the walk: **structural** — form identity one this image composes,
emitter version the one this image's execution half was built against, alignment, one symbol row per
unit, ascending, inside the bytes; **template closure**, in every image — the emitted bytes decoded
against the closed template table of that form for that emitter version, with the shape clauses S1 to
S4 of JSD-0025 section 2 generalised (one prologue and one epilogue per unit, no branch into either,
every indirect call through the handler table's slot for a row of the unit's family, no template with a
memory destination outside the word plane and the frame); and **re-emission equality**, where the
encoder is in the image — recompile the carried bytecode with the same encoder and compare byte for
byte, through a descriptor that carries the encoder, as `DescriptorReEmittingWith` does today. The
limit VM-7 states is restated here rather than softened: **no layer catches a well-formed emission from
a wrong encoder**, and what pins the encoder is the differential oracle of 7.5 and nothing in this
section.

### 6.4 Determinism and corpora

- Stage UBC-3 asserts, before any emitter consumes a byte, that both translators are deterministic
  over their whole corpora — the universal bytecode's own malformed corpus (self-authored bytes with
  hash and pinned answer, replayed across three publish modes, per ADR 0011's published schema), the
  JavaScript retained corpus re-based onto the new container, and the WebAssembly corpus of modules the
  translator refuses.
- The corpus entry schema is the core's; the runner is each family's, under the extraction gate's
  standing verdict on the corpus method, which this concept does not reopen.

---

## 7. The emitter profiles

### 7.1 What an emitter profile is, and how the core sees it

**An emitter profile is a product project family that turns verified universal bytecode into an
executable form, and it has two halves.** The *emitting half* runs at compile time: it consumes a
verified universal bytecode program and the registrations of the families it uses, and answers with
an Emission section or a refusal naming a reason. The *executing half* runs at run time: it is the
`IVmProfileExecutor` the language's descriptor names, and — for a native form — the verifier layer of
6.3 and the arming path. The bytecode emitter's emitting half is the identity: it emits nothing and its
executing half interprets the bytecode as it stands. The two halves agree on the form through a pivot
assembly (7.3) for the same reason the JavaScript lowering and runtime agree on a format through one.

**The core sees one descriptor per language, and an emitter builds it.** `Broiler.VM.Ubc` exports one
factory:

```text
VmProfileDescriptor UbcDescriptors.Build(
    UbcFamilyRegistration family,          // the language's identity, tables, handlers, hook, payloads
    UbcFamilyDeclaration declaration,      // the language's descriptor rows: limits, maxima, matrix,
                                           //   capability imports, guest loads, suspension, payload
                                           //   range, diagnostics identity, package identity,
                                           //   conformance manifest, affinity, poll bounds, fault
                                           //   recovery, sharing
    UbcEmitterSet emitters)                // the forms this composition executes, each with its
                                           //   form verifier and executor factory
```

Row by row: rows 1 to 3 (identity, display name, descriptor revision) are the family's; row 4 (format
versions) is the universal bytecode's; row 5 (accepted manifests) is the family's manifests whose
tables every composed emitter admits; rows 6 and 7 (verifier, executor factory) are built here; rows 8
to 30 are copied from the declaration unchanged. **So the JavaScript descriptor keeps its identity, its
limits, its maxima, its budget matrix, its four capability imports, its guest-load declaration, its
payload range and its diagnostics identity, and the WebAssembly descriptor keeps its own**; a composition
composing both still folds nothing, and the catalog's tightest-default rule reaches each exactly as it
does today. This is what makes the concept need no core amendment: the core receives the thirty rows
it always received, from a factory it never sees.

**An emitter has an identity of its own — a form identity and a semantic version — that the core never
sees.** It appears in the artifact header, in the support table's output-form rows (section 3a of
[`docs/support.md`](support.md), which becomes the emitter table), in the composition register's
native-execution column for the forms that arm, and in the emitter family's own ledger under its own
`docs/`. The frozen term for it is *artifact output form*, and this document says so where it matters.

**The executor an emitter set produces routes by the artifact's form, never by observation.** A
composition that composes the bytecode emitter and the `x86-64` emitter executes a bytecode-form
artifact in the interpreter and an `x86-64` artifact natively, because the form is in the verified
handle; a composition that composes only the native emitters refuses a bytecode-form artifact at
verification, and one that composes only the bytecode emitter refuses a native one there. **There is
no image in which both paths exist for one artifact**, which is the amended non-goal — one executor,
one form per handle and per instance — restated as a property of a factory.

### 7.2 The bytecode emitter: `Broiler.VM.Profile.Bytecode`

- **One dispatch loop, generic over the family.** `UbcInterpreter<TFamily>` where `TFamily` is a
  struct the family supplies implementing `IUbcFamily` with static abstract members — the value plane
  type, the handler dispatch, the plane operations, the frame codec, the landing, the payload factories.
  The JIT specialises the loop per family and the Native AOT compiler instantiates it per composed
  family, so a WebAssembly composition carries a loop whose `v` operations are unreachable and whose
  primitives are the C# operators, and a JavaScript composition carries a loop whose handler dispatch
  is a `switch` over the family's opcode byte into the arms that are the interpreter's today. The
  technique is the one `JsEngine.ExecuteCore<TMode>` already uses to fold a mode into a method.
- **The loop implements the common family itself**: it moves words and plane indices, pushes and pops
  heap frames, charges cost and `CallDepth`, polls, searches regions, and lands. It calls the family for
  every family row — `Dynamic`, `Branch`, `Call`, `Suspend`, `Throw` — and for a `Primitive` row it
  executes the primitive from Appendix D directly over the word plane, with the family's NaN flag
  honoured. The handler signature is fixed by `Broiler.VM.Ubc`:

  ```text
  static UbcStatus Handle(ref UbcActivation activation, byte familyOpcode, uint operand)
  ```

  with `UbcStatus` one of `Next`, `Taken`, `Request` (a call request the activation now holds),
  `Suspend`, `Threw`, `Trap(code)`, `Defect`. The activation gives a handler its unit, its pc, both
  planes as windows, the family's instance state, the meter, the capability invoker, and nothing else.
- **Frames are heap objects owned by the operation; the CLR stack does not grow with guest depth.**
  JSD-0011 Row 4 holds for every family, and `CallDepth` is a counted number the loop compares.
- **The interpreter is an emitter profile with a form identity `bytecode`, a ledger, and a support-table
  row**, so that the two axes stay two: a composition composing it and no native emitter is an
  execution-only image with no code generator, and its register row declares `none`.
- **What it does not do.** No quickening, no inline caches in the bytecode (invariant 5: canonical
  bytecode carries no warmed state; a family may keep per-instance feedback in its own state), no
  threaded code, no computed goto, no per-block steps; the loop is a `switch` over a byte, and whether a
  different loop is worth having is a measurement nobody has taken.

### 7.3 The `x86-64` emitter: `Broiler.VM.Profile.MachineCode.X64` and `Broiler.VM.Profile.MachineCode`

- **Inputs.** A verified universal bytecode program, the registrations of its families, a convention
  (`x86-64-sysv` or `x86-64-win64`), and the family handler-table shapes. **Outputs.** An Emission
  section: one unit of machine code per Units row, a symbol row per unit, the form identity and the
  emitter's semantic version.
- **What it emits, per row kind.** For a common row, a template from the pivot's table over registers
  and the native stack: the word plane lives in registers and native slots, the value plane in the
  family's managed array reached by index. For a `Primitive` row, the primitive's template over the
  word plane, with the trap edge to a trap block and the NaN canonicalisation sequence where the
  family's table says so. For every other family row, a call through the family's handler table —
  `call qword [rbx + slot*8]`, the JSD-0025 mechanism generalised: `rbx` holds the family's table,
  the frame holds the activation cookie, and the wrapper checks the cookie, the pc and the opcode
  before it runs the family's handler and answers a status the emitted code branches on. For a `v`
  plane shuffle, a call to the family's plane helper (`copy`, `clear`). For `call` and for a call
  request, the direct-call protocol of JSD-0035's stage JSV-3: a machine call from one emitted frame
  into the next unit's entry, with `CallDepth` charged in the prologue and the family's `Prepare` and
  `Finish` helpers around it. For a `Suspend` answer, an exit to the managed entry that captures the
  frame through the codec. For a `Throw` answer, the unwinding of 5.10 performed by the managed entry,
  which lands by re-entering the unit at the handler offset through the compare tree over the unit's
  known landings.
- **The rooting statement, restated for this emitter and falsifiable in the same words as
  JSD-0025 section 4.** The emitted frame holds the handler table's address, the activation cookie, the
  word plane, and integers. No emitted byte addresses a managed object; the value plane and every family
  object are reachable from the activation, which is rooted by the managed frame that entered emitted
  code and by the thread-static slot that frame sets and restores; emitted code runs in preemptive
  mode; a handler's reverse transition switches to cooperative mode and the collector walks the managed
  frames and finds nothing to trace in between. The same three properties of the thread-static slot
  hold, and the same text rule is registered over the machine-code family's source instead of the
  JavaScript profile's.
- **The pivot, `Broiler.VM.Profile.MachineCode.Format`.** The frame layouts and their offsets, the two
  conventions' stack reservations and argument registers, the handler-table slot rule, the template
  tables per (convention, emitter version) with each template's fixed bytes and the closed value set of
  each variable field, and the template-closure scan. It is where `JsNativeFrame`, `JsBaselineFrame`,
  `JsNativeTemplates`, `JsNativeScan`, `JsX64Abi` and `VmNativeFrame` go, unified, and it is
  language-free: a template that calls a handler names a slot, not an opcode of any family.
- **The executing half, `Broiler.VM.Profile.MachineCode`.** The arming path (`VmNativePage`, W^X,
  `SafeHandle`, one type and three files, rule X1's subject moved to where it already is), the native
  activation, the `[UnmanagedCallersOnly]` wrappers per family table slot, the managed entry that
  enters a unit and lands a suspension or an unwinding, the form verifier layer, and the executor
  factory the descriptor names. Rules B5c and X1 allowlist this one assembly, and their register
  statements say so instead of naming the JavaScript profile.
- **Which manifests it admits.** Every family table whose every row it can emit: a family row of a
  kind it does not implement, or a primitive its template table lacks, is a refusal of the whole
  artifact naming the row. It admits the JavaScript wide and numeric manifests (the baseline and
  numeric forms recovered) and the WebAssembly manifests (a native WebAssembly form for the first time
  in this repository, whose whole operand stack is words).
- **The composition register's native-execution column is unchanged in meaning.** A root that
  composes this family declares `x86-64`; one that does not declares `none` and must be unable to arm,
  which rule K5 reads against the ImplMap tables as today.

### 7.4 The `arm64` emitter: `Broiler.VM.Profile.MachineCode.Arm64`, emitting-only

- **The same inputs, the same pivot, its own template table, and no execution.** It emits for every
  family table it can, under the AAPCS64 convention, into the same Emission section shape; an artifact
  carrying its form verifies through the scan against the `arm64` table and **refuses to instantiate on
  every host with the unsatisfied-host-assumption contract violation**, exactly as today. The
  exclusion's reason is unchanged and is not a shortage of machines: the architecturally required
  instruction-cache maintenance sequence has no managed expression and no dependable library export
  (route MVP-4, VM-7's rule), and what would close the exclusion is a maintenance path this component
  can name, call and test.
- **Its evidence is its encodings**: a golden-byte test per template and a corpus every entry of which
  a disassembler reads back and compares against the instruction the table names, both failing on a
  changed byte. **No figure and no capability claim attaches to it**, and the support table's row says
  *emitting-only* in the row, which release gate 11 requires and a rule fails the release for omitting.
- **It admits what it can emit and refuses the rest by name.** Whether it emits the JavaScript wide
  manifest's handler-call form — which the arm64 encoder refuses today — is a decision for the stage that
  writes it, and this document does not take it.

### 7.5 What every emitter must satisfy: the emitter obligations

| # | Obligation | Checked by |
|---|---|---|
| E1 | The common family's every row is executed with the meaning Appendix A fixes, in every form. | the fixture family's corpus (8.3), run in every composed form |
| E2 | A `Primitive` row executed inline answers, bit for bit, what the family's handler answers, over the retained primitive corpus, NaN, signed zero, overflow and trap edges included. | the primitive differential check, per (emitter, family, manifest) |
| E3 | The emitter is deterministic: one program, one emitter version, one emission. | re-emission equality; a two-compile comparison in the checks lane |
| E4 | The emitter answers with the whole artifact or a refusal naming a row, and never with a per-unit choice. | the emitter contract's signature; a negative control |
| E5 | The emitted frame holds no managed reference. | rule X2 generalised to the machine-code family's frame types |
| E6 | Fuel is charged per row at the same point as the interpreter charges it; a budget verdict agrees with the bytecode form's at every ceiling, outside the one named divergence class. | the fuel-parity twins, per family |
| E7 | The differential oracle holds: one source, the bytecode form and this form, equal transcripts outside the named classes (a refusal naming the emission ceiling; a fuel exhaustion on a guest-loading program; a wall-clock exhaustion). | `eng/compare-forms.py`, generalised to every family and form, and the conformance suites run in both forms |
| E8 | The template-closure scan accepts everything the emitter emits and every template is reached by the corpus; byte strings that are legal machine code the emitter never emits are refused by name. | the checks lane, both directions |
| E9 | An emitter never names a family, a language or an opcode of either: its source is scanned for the banned vocabulary. | rule U3 (stage UBC-1) |

### 7.6 The path from bytecode to executable memory, step by step

**A reader who wants to know whether an emitter may take a run of bytecode instructions, translate
it into a run of native instructions and put that run into executable memory should read this
subsection: the answer is yes, under three conditions, and every step below names where each
condition is enforced.** The subsection restates what sections 5.1, 6.3, 7.1, 7.3 and 7.5 already say,
in the order the bytes travel, because a reader who assembled it from those five places could
reasonably have concluded either more or less than the concept allows.

```text
  a verified universal bytecode program              every unit of the artifact, and the family
        │                                             registrations it names
        ▼
  1. the emitting half translates every unit,        Broiler.VM.Profile.MachineCode.X64 or .Arm64:
     or refuses the whole artifact naming a row       references Broiler.VM.Ubc and the pivot only;
        │                                             it has no path to a memory-mapping call
        ▼
  2. the emission is written into the SAME           the Emission section of the container (5.2),
     artifact, beside the bytecode                    form identity and emitter version in the header
        │
        ▼
  3. the artifact is verified as a whole             Broiler.VM.Ubc: the walk and the family hook,
     (walk, hook, structural, template-closure        then the form layers of 6.3; the scan runs in
      scan, re-emission)                              every image, re-emission where the encoder is in it
        │
        ▼
  4. the executing half maps a page writable,        Broiler.VM.Profile.MachineCode: VmNativePage,
     copies the bytes, arms it read-execute           the one arming path, W^X, declared in the
        │                                             composition register and read by rule K5
        ▼
  5. an entry is called through an unmanaged         the handler table, the activation and the cookie
     function pointer; a Dynamic row re-enters        of 7.3; a Primitive or common row runs without
     managed code through its handler                 leaving emitted code
```

**Condition 1 — the run is the whole artifact.** An emitter's two answers are the emission of every
unit and a refusal naming the row it cannot emit (principle 5 of section 5.1, obligation E4). It may
not compile the units it likes and leave the rest to the interpreter, and no image executes one
artifact in two forms (7.1). The reason is VM-7's "no tiering" rule and the JavaScript profile's amended
non-goal, both kept: the form is a property of the artifact, fixed when it is compiled and pinned when it
is verified, and nothing observes a running program to decide what to compile.

**Condition 2 — the bytes reach executable memory only as a verified artifact, and only through the
arming path.** The encoder writes bytes into a container and nothing else; the assembly that holds it
does not reference the assembly that holds the arming path, so it cannot map a page by construction
rather than by discipline (the graph in section 4). The core's invariant 3 — verification produces the
only executable input — applies to a native form exactly as to bytecode, which is why the template-closure
scan runs in every image and not only where re-emission is possible. Arming is one type in one assembly,
written readable-writable and executed read-execute and never both at once, allowlisted by rules B5c and
X1, and permitted only in a composition whose register row declares the architecture. **No byte is
executed that the verifier has not admitted, and no emitter has the means to arrange otherwise.**

**Condition 3 — the translation is a function of the tables and of nothing observed at run time.**
Within a unit an emitter translates runs of common and primitive-classified rows into straight-line
native code with no callback, and it may translate several bytecode instructions into one native
sequence — but only as a template its pivot's table registers, because the scan refuses a byte
sequence that matches no template (6.3, obligation E8), and re-emission must reproduce it byte for byte
(obligation E3). A `Dynamic` row is one handler call, at the granularity route UBC-R6 records and no
measurement has weighed. What an emitter may not do is fuse, reorder or specialise on the strength of a
profile, a counter, a type observed at run time or an earlier execution of the same artifact: a form
carries no warmed state (invariant 5) and is the same bytes on every machine that compiles it.

**When this may happen.** At compile time, into an artifact a host stores or hands to a verifier
later; or at run time, in one process, in a composition that declares both the compiler and the
architecture — which is what the JavaScript end-user command line does today with its native option,
and what happens for every program a guest loads with `eval`, the `Function` constructor or an import
inside a native-form instance, since such a program is compiled in its instance's form (5.13). It may
never happen in an execution-only image, which holds no encoder and whose register row declares `none`,
and it may never happen as a response to something the running program did.

**What this path is not.** It is not a just-in-time tier. The difference is not whether an encoder is
in the running image — a compiler-bearing composition declares one, as VM-7 admits — but whether
anything chooses *what* to compile from a running program: nothing does, and section 14's falsifiers
name the observations that would show otherwise, a code path selecting a form from run-time observation
and an artifact executable in two forms among them.

---

## 8. The language profiles after the refactoring

### 8.1 JavaScript

**What moves, what stays, and what is retired**, by assembly:

| Today | After | Note |
|---|---|---|
| `JsOpcode`, `JavaScriptOpcode`, `JsOpcodes.*` tables, `JsNumericManifest` (Format) | the `js.*` family tables, one per manifest, as `UbcInstructionTable` values (Format) | byte values kept; the tables *are* the rows Appendix B lists |
| `JsFormat`, `JavaScriptFormat`, `JsArtifactWriter`, `JavaScriptArtifactWriter` (Format) | the JavaScript FamilyData sections' schema and codec (Format); the container is the universal bytecode's | **format versions 1 and 2 are retired**; nothing is published, so no external artifact exists to be invalidated |
| `JsCompiler.Assemble` → `JsAssembledProgram` → `IJsNativeBackend` (Compiler) | `JsCompiler.Assemble` → a universal bytecode program, and nothing after it (Compiler) | the second exit becomes the only exit; `JsOutputForm` leaves the compile request |
| `JsX64Backend`, `JsX64BaselineEmitter`, `JsX64ValueEmitter`, `JsX64Walk`, `JsX64Assembler`, `JsX64Abi`, `JsArm64*` (Compiler) | `Broiler.VM.Profile.MachineCode.X64` and `.Arm64` | language-free after the move: what was a template per `JsOpcode` becomes a template per common row, per primitive, and one per handler call |
| `JsNativeTemplates`, `JsNativeScan`, `JsBaselineBlocks`, `JsValueLayout`, `JsNativeFrame`, `JsBaselineFrame`, `JsValueFrame`, `JsWord` (Format) | `Broiler.VM.Profile.MachineCode.Format`, unified; `JsValueFrame`, `JsValueLayout` and `JsWord` are not carried | the value form is not carried (5.7) |
| `JsVerifier`, `JavaScriptVerifier` (Profile) | `JsFamilyVerifier : IUbcFamilyVerifier` — the family hook — and nothing else | the walk is `Broiler.VM.Ubc`'s |
| `JsEngine.ExecuteCore<TMode>`'s arms (Profile) | the family handlers, one static method per row, behind `JsFamily : IUbcFamily` | the arm text is the meaning and it moves as text; what is dropped is the loop, the frame list, the region search and the charge sites around it |
| `JsBaselineHandlers`, `JsValueHelpers`, `JsNativeActivation`, `JsEngine.Baseline` (Profile) | the family's handler table is built by the machine-code family from the same handler methods; the activation is the machine-code family's | one wrapper mechanism for every family |
| `JsNativePage`, `JsNativeExecution`, `JsNativeAbi` (Profile) | retired; the arming path is the machine-code family's and no language profile arms a page | rule N1's reference list loses nothing and gains `Broiler.VM.Ubc` |
| `JavaScriptProfile.Descriptor` and its four variants (Profile) | `JavaScriptFamily.Registration` and `JavaScriptFamily.Declaration`, consumed by `UbcDescriptors.Build`; the variants become parameters of the declaration (admitted surfaces, host realms, handle stress is gone with the value form) | a composition root writes `UbcDescriptors.Build(JavaScriptFamily.Registration, JavaScriptFamily.Declaration, emitters)` |
| `JsNativeCompiler : IVmNativeCompiler` (Compiler) | retired; the native emitters implement `IVmNativeCompiler` (route UBC-R5) | |

**What does not move: the realm, `JsValue`, `JsObject`, strings, symbols, BigInt, the job queue, the
module graph, host realms, the structured-clone carrier, every `JsRealm.*` file, the diagnostics
registry, the conformance harness, the test262 pin, the Octane pin, the differential runner.** The
language is where it was; what left is the machinery around it.

**The three forms, recovered.** The bytecode form is the bytecode emitter over the wide table. The
numeric form is the `x86-64` emitter over the numeric table, whose operator rows are primitives, so its
emitted code computes over `f64` words exactly as today's computing templates do — with two
differences a reader should expect: the frame is the machine-code family's rather than a slab of
doubles with a bailout pc, and the units' locals are word locals rather than scope slots the numeric
walk flattened. The baseline form is the `x86-64` emitter over the wide table, whose rows are
`Dynamic`, so its emitted code is control flow and handler calls exactly as JSD-0025 decides — with the
difference that `Return`, `Jump` and the stack shuffles are now templates rather than handler calls,
and that JSD-0025's route MVP-8 (one call per instruction) is kept as the granularity for `Dynamic`
rows and is still a route nobody measured.

**The retained corpora are re-based, and that is a cost stated rather than hidden.** Every retained
JavaScript malformed-corpus entry is a format-version-1-or-2 artifact whose pinned answer names a
JavaScript refusal; under the universal bytecode the same malformation is either the universal walk's
refusal or the hook's, with a different code where the walk owns it. Stage UBC-3's gate requires every
entry re-authored onto the new container with its old answer or a recorded reason for the new one, and
the registry rules (N5 to N8) held in both directions over the result. The whole-suite conformance
floors under `src/tests/conformance` are re-based by hand for the same reason the profile re-based
them on 2026-09-08 and 2026-09-21: the engine moved on purpose and the ratchet must answer `Regressed`.

### 8.2 WebAssembly

**The module becomes source.** A WebAssembly binary module reaches the profile through the same
embedder seam a JavaScript source file does — a composition's compile entry point, or a provider that
answers a guest load — and the profile's front end decodes it, validates it with the validator it has
today, and **translates** the validated module to a universal bytecode artifact, deterministically.
The artifact the core verifies is that artifact. The translation is a walk over each validated body's
bytes with the validator's jump table in hand: labels become absolute offsets, block results become
`squash` sequences, `if`/`else` become conditional jumps, LEB immediates become fixed-width operands,
dead code is dropped, and everything else maps row for row per Appendix C.

**What the profile keeps**: the decoder and its refusals, the validator and its diagnostic registry,
the store, `WasmMemoryInstance`, the table, the globals, the segments and their atomic initialisation,
the trap kinds and payloads, the entry-point argument grammar of route MVP-6, the deterministic profile
and its NaN rule, and the conformance harness with its corpus store.

**What the profile loses**: `WasmInterpreter` as a loop (its arms become the `wasm.*` handlers, of
which the primitives are also the reference implementations for E2), `WasmValue` (no `v`, so no value
plane), the sixteen-byte slot and the vector reservation of route MVP-2 (a `v128` slot type would be a
universal bytecode format version 2 question and is not answered here), and its status as an
execution-only profile with no compiler: it now has a translator, and its execution composition carries
it.

**Three decisions the stage that does this must take, and this document does not:**

1. **A format sibling or not.** The pivot argument now applies — a translator and a runtime library
   must agree on the family tables — but both live in one assembly today and can go on doing so; a
   second assembly is the "dated decision, not a convenience" that profile's section 5 already reserves.
2. **The memory representation.** A native form that addresses `memory0` through a region primitive
   needs a base address that does not move for the life of the instance: a pinned managed array, or
   native memory owned by a `SafeHandle`, each with its growth story — a successful `memory.grow`
   invalidates every view, which that profile's section 13 already rules. This is the row of its
   section 9 value-model decision that the native form makes urgent.
3. **Which manifests exist.** The code admits more than `broiler.webassembly.slice` under that one
   identity and says so; the universal bytecode's per-manifest tables make the gap visible, because a
   table is selected by manifest. Minting `broiler.webassembly.numeric1` is that profile's WA-5 act.

**The float-comparison defect of 2.2 is what stage UBC-4's E2 check exists to find**, and it is named
in that stage's gate as the negative control: the check must fail on the checkout's arms before the
arms are corrected, and pass after.

### 8.3 The fixture family

**The core is provable without a product profile (invariant 13), and so is the universal bytecode.**
Stage UBC-2 writes an application-local family under `src/tests/`, in the position `Com.Example.*`
occupies today, against the public surface of `Broiler.VM.Ubc` and nothing else: one value type of one
field, three family rows (one `Dynamic`, one `Primitive`, one `Throw`), a hook that refuses one thing, a
payload, and a trivial lowering from a fixed program list. Every emitter's obligations E1 to E8 are
first demonstrated over it, in every composed form, before either language family exists on the new
graph. It never ships in a product package and rule A11's exemption does not reach it, so it is composed
by a demonstration root under `src/compositions/` like every other family.

---

## 9. The compositions after the refactoring

| Composition root | Composes | Forms | Native execution | Note |
|---|---|---|---|---|
| `Broiler.VM.Composition.JavaScript.ExecutionOnly` | JavaScript family; bytecode emitter | `bytecode` | `none` | the execution-only image: a format, a walk, a hook and a loop, and no compiler, no encoder, no arming path |
| `Broiler.VM.Composition.JavaScript.SliceCompiler` | JavaScript family with its lowering; bytecode emitter; `x86-64` emitter (both halves) | `bytecode`, `x86-64-*` | `x86-64` | the checks lane: determinism, re-emission, the scan in both directions, the primitive differential check, the forms comparison |
| `Broiler.VM.Composition.JavaScript.Conformance` | as above | as above | `x86-64` | the conformance oracle in either form, per manifest, whole |
| `Broiler.VM.Composition.JavaScript.Cli` | as above | as above | `x86-64` | the end-user host; one `--form` option naming `bytecode`, `x86-64-sysv`, `x86-64-win64` or `arm64-aapcs64` |
| `Broiler.VM.Composition.JavaScript.Android` | JavaScript family; bytecode emitter | `bytecode` | `none` | unchanged in shape |
| `Broiler.VM.Composition.WebAssembly.Execution` | WebAssembly family (translator inside); bytecode emitter | `bytecode` | `none` | no longer execution-only in the roadmap's sense: it carries a translator; the register's sibling cell says so |
| `Broiler.VM.Composition.WebAssembly.Harness` | as above | `bytecode` | `none` | the corpus store and encoder, unchanged; the harness feeds modules through the translator |
| `Broiler.VM.Composition.PolyglotCli` | JavaScript family with its lowering; WebAssembly family; bytecode emitter | `bytecode` | `none` | **one interpreter, two families**: the image the concept exists to make possible, and the two-profile catalog test section 14 of the roadmap has asked for since VM-3 |
| `Broiler.VM.Composition.Calculator`, `Workbench` | the core's fixture profiles | — | `none` | unchanged: they do not use the universal bytecode, which is what proves the core does not need it |
| *new* `Broiler.VM.Composition.Ubc.Fixture` | the fixture family; bytecode emitter; `x86-64` emitter | `bytecode`, `x86-64-*` | `x86-64` | the emitter obligations over a family that is not a language |

**Every row is a demonstration; the advertised set stays empty.** The register's schema needs no new
column: the sibling column lists `Broiler.VM.Ubc` and the pivots, the native-execution column is read
by K5 as today, and the profile-assemblies column lists the family assemblies. **A composition that
composes a native emitter and not the bytecode emitter is a legal image** — a format, a walk, a hook,
a form verifier and an arming path, with no interpreter loop and no encoder — and is the shape VM-7's
exit gate clause 3 asks an execution-only composition to demonstrate; whether one is written is a
stage's choice and none of the rows above is it.

---

## 10. What was considered and not taken

| Option | What it does | Why it is not taken |
|---|---|---|
| **The emitter as the core-facing profile** (`broiler.bytecode`, `broiler.machinecode` as descriptors, the language as a manifest under them) | One descriptor per emitter; an artifact names the emitter as its profile and the language surface as its manifest | The descriptor's thirty rows are per profile, so two languages under one descriptor share one set of limits, maxima, capability imports and one payload range: a WebAssembly module would be bounded by the JavaScript profile's hard maxima, which is the hostile-neighbour defect the roadmap's second risk row exists against, and fixing it needs per-manifest rows, which is a core amendment the procedure cannot mint. The language-centric shape of 7.1 needs no amendment and keeps every existing identity and ledger meaningful. |
| **Typed arithmetic in the common family** (`i64.add`, `f64.mul` as common rows; `wasm.*` reduced to the trapping and memory rows) | The emitter implements arithmetic as a common meaning; the WebAssembly family shrinks | The request places `i64add` with WebAssembly, and the placement is defensible on its own terms: the meaning of `i64.add` is a sentence of the WebAssembly specification, including its interaction with that specification's deterministic profile, and a common row with one meaning would have two owners the day a second language wanted a different NaN rule. The primitive classification of 5.6 gives an emitter the same instruction selection with the meaning left where the request puts it. **This is the row a reader should disagree with hardest**, because the primitive table is arithmetic under another name; the answer is that a primitive is an implementation the family's handler is checked against, and a common row would be a meaning nothing is checked against. |
| **A NaN-boxed sixty-four-bit word as the universal `v`** (JSD-0035's route, made universal) | Every slot is a word; emitted code holds values; a handle table roots references | Refused on 2026-09-25 on its own predeclared measurement for the one form that built it, and JSD-0011 Row 1 keeps it registered and not adopted. Making it universal would make every family pay for a second lifetime mechanism, and the value plane by index gives the emitters everything they need for a WebAssembly unit — which holds no `v` at all — and gives the JavaScript family the rooting answer JSD-0025 already has. |
| **A typed intermediate form with a control-flow graph and register allocation** (SSA, a scheduler, an optimiser) | A real compiler middle end between the languages and the emitters | It is what an intermediate form would be for, and it is not what this repository can verify: re-emission equality and the template-closure scan rest on a closed template table over a *linear* form, and an optimiser's output is not closed under any table a scan can hold. The universal bytecode is deliberately the same kind of thing the JavaScript bytecode is — a stack machine with static heights — so that every verification layer the repository has carries over. A middle end above it would be a fourth tier of the graph and a different design. |
| **A shared syntax tree, a generic compiler host, a pluggable front end** | Languages contribute parsers to one compiler | Refused permanently by section 8, and rightly: the two front ends share nothing, one of them is a decoder over a wire format, and this concept touches neither. |
| **Keeping `"BMC\0"` and `broiler.machinecode` as a separate artifact form and profile** | The native form is a second artifact the language artifact is compiled into | It drops the bytecode beside the machine code, which the differential oracle and re-emission both need; its verifier decodes no instruction, so an execution-only image would trust provenance; and its descriptor is registered nowhere today. An Emission section inside the one container is what VM-7's JavaScript form already does, and it is kept. |
| **Per-unit forms, a guard, a fallback** | Compile the units an emitter likes and interpret the rest | Still what the amended non-goal and VM-7's "no tiering" refuse, and still not needed: an emitter admits a whole table or refuses a row by name. |
| **Structured control flow in the universal bytecode** (`block`/`loop`/`if`/`end`, labels with arity) | The WebAssembly translator is nearly the identity; the JavaScript lowering has to invent block structure | Flat control with absolute targets is what the JavaScript format has, what the baseline form's compare tree dispatches over, what the region table needs, and what a native emitter branches on directly; structured control is a validation convenience of one wire format and its validator already computes the flat targets. |
| **Variable-length operands** | Smaller code | The instruction boundary must be computable from the tables without decoding, which every JavaScript format version has held and which the scan and the region checks depend on. Size is not a claim this component makes. |
| **A sixteen-bit opcode space, no prefixes** | Every family in one flat space with a central allocation | A central allocation is a registry the core would own, and a family byte that indexes the artifact's own Families section is a declaration the verifier checks against the composition, which is the property every other declared thing here has. |
| **One interpreter per family** (a `JsInterpreter`, a `WasmInterpreter`, each over the universal bytecode) | Each family owns its loop | It is the second dispatch loop section 3.4 exists to remove, and a loop generic over a family struct folds to the same code per instantiation. Whether it folds *in the compiled image* is a gate (UBC-2), not an assertion. |
| **Charging fuel as a debt after inline primitives** | Fewer meter crossings on the primitive path | It was part of the refused value form, and it makes exhaustion land at a different instruction than the interpreter's, which E6 forbids. Whether a cheaper exact charge exists is a measurement nobody has taken. |

---

## 11. What it costs

Stated as properties, because no figure about any of them is retained and this document states none:

- **Two verifiers and two interpreters are rewritten into one of each, and the rewriting is not a
  move.** The JavaScript verifier's walk and the WebAssembly validator's stack walk become one walk;
  the JavaScript interpreter's loop and the WebAssembly interpreter's loop become one loop; the arms
  survive as handlers, the loops do not. Every line of the arms is unreviewed today and stays
  unreviewed after it moves, and moving unreviewed code is not review.
- **Both languages' retained corpora are re-based**, with every entry's old answer carried or its new
  answer recorded beside a reason, and both whole-suite floors are re-based by hand with the retired
  rows and their reasons written beside them. A floor that had to be lowered by hand is a weaker claim
  than one that never moved, and this concept lowers two.
- **The JavaScript profile's format versions 1 and 2 are retired, and the WebAssembly profile's "the
  module is the artifact, unwrapped" is reversed.** Nothing is published, so no external consumer holds
  an artifact either change invalidates; every internal consumer — the corpora, the harnesses, the CLI,
  the differential runner, the collection scripts under `eng/` — is rewritten.
- **A WebAssembly module is translated on every load**, which is work the checkout does not do today,
  charged to the verifier-work budget of the load that asked for it. The harness scores modules through
  the translator, so a translator defect scores as an engine defect; that is the price of the module
  being source.
- **Every dynamic JavaScript instruction in a native form is still one handler call**, with the
  reverse transition and the three checks JSD-0025 section 6 prices; this concept removes the calls for
  the common rows and the numeric primitives and removes none for the rest. **No speed is claimed for
  the result in any direction.**
- **A native WebAssembly form's memory must not move**, so the family's memory representation changes
  from a plain managed array to a pinned or native region, with a growth story to re-prove.
- **A fourth Broiler.VM-owned assembly, four to five product projects and one test family join the
  graph**, each a dated revision of ADR 0001, each with assurance annotations, generated headers and
  fingerprints the generator produces, and each an assembly the publish lane must trim and AOT-compile
  in every composition that carries it.
- **Every rule Appendix F lists moves**, and a rule that moves is a rule re-witnessed: each carries a
  negative control watched failing and passing after revert, which is the repository's own price for a
  rule and is paid roughly two dozen times here.
- **The records move**: the roadmap's non-goal and section 8 table row, ADR 0011's standing refusal
  and P1, the JavaScript plan's second-execution-arm and second-lowering entries and its section 9
  intermediate-form sentence, the WebAssembly plan's compiler and framing sections, the composition
  register's sibling cells, the support table's section 3a as an emitter table, and three MVP route
  rows. Each is a dated correction with the superseded text quoted, and a concept that moved them
  quietly would be the failure this repository's correction discipline exists against.
- **What is deferred stays deferred.** Nothing here is reviewed, accepted, advertised, packed, claimed
  for a runtime identifier or measured, and the deferral's cost — decisions nobody took — is paid in
  section 12.

---

## 12. Routes this concept takes without a decision

Filed here as the rows [`docs/mvp.md`](mvp.md) section 5 would carry, in that record's four-column
shape, and **to be filed there as `MVP-10` onward by the first stage that lands code**; a row is not a
decision and reverses without a correction entry, and what is built on it may have to be unbuilt.

| # | Route taken | Alternative not taken | What would settle it | Who would decide |
|---|---|---|---|---|
| UBC-R1 | **ADR 0011's P1 set gains `Broiler.VM.Ubc` by editorial revision, as the consequence of an extraction verdict the record's own clause says changes the core graph** (3.3) | **An additive amendment minting core contract version 2**, which the procedure cannot currently perform; the concept would be `Blocked` naming the core as holder | A ruling by the core contract owner on whether P1's member list is contract content or the recorded consequence of a gate the contract already contains; either answer is an act, and silence is neither | The core contract owner — one person |
| UBC-R2 | **The primitive table is mechanism**: a closed set of machine operations an emitter implements, checked against each family's handler, naming no language (5.6) | **Typed arithmetic as common rows with a meaning the universal bytecode fixes**, which section 10 records and the request's placement of `i64add` argues against | A dated verdict in the extraction record (UBC-0) that the table passes G3, or a refusal naming the row that does not | The core architecture owner, with both language profiles' owners supplying their halves — one person |
| UBC-R3 | **A `v` slot is an index into a family-owned managed plane, never a word emitted code holds** (5.3) | **A NaN-boxed universal word with a handle table**, JSD-0035's design made universal | A decision record adopting such a word on a retained measurement of the plane-by-index design's cost in a native form — the condition MVP-8 already names for the JavaScript profile alone, now for every family | The JavaScript profile's owner with the core's security owner — one person |
| UBC-R4 | **A WebAssembly binary module is source the profile translates, and the universal bytecode is the artifact** (8.2) | **The module stays the artifact and a translation is performed at instantiation into an executor-owned form**, which keeps that profile's section 7 true and makes the translation an execution strategy rather than a payload — and makes the differential oracle compare a form nothing retained | That profile's owner amending its section 7 and its compiler non-goal, or recording a refusal; the WA-5 memory decision is taken in the same act | The WebAssembly profile's owner — one person |
| UBC-R5 | **`IVmNativeCompiler` is kept and implemented by the native emitters: universal bytecode in, the same artifact with an Emission section out, under the *input* profile's identity** | **Retiring the interface**, which moves the core's public API baseline (rule M1) and the frozen public-name table; or keeping today's reading, in which the output is a `broiler.machinecode` artifact | A ruling on whether a member of Abstractions whose only implementation is unreachable is contract content; and the contract test's expectation (`NativePipelineContractTests`) rewritten to the new reading | The core contract owner — one person |
| UBC-R6 | **One call per `Dynamic` row in a native form** (MVP-8 kept, for every family) | **Per-block steps**, one call per run of dynamic rows | The retained measurement MVP-8 names and nobody has taken | The JavaScript profile's owner with the core's security owner — one person |
| UBC-R7 | **One dispatch loop generic over a family struct, instantiated per composed family** (7.2) | **One loop per family**, hand-written | Stage UBC-2's gate: the instantiated loop's compiled code folds the family tests away, shown on the Native AOT image and not only in source; a loop that does not fold reopens this row | The core architecture owner — one person |

**UBC-R1 is the row a reader should be most uncomfortable with**, for the reason 3.3 gives: it is the
one place this concept's "no core amendment" is a reading of a record rather than a fact about the
code, and the reading is taken by the person who would otherwise have to mint the version. It is
recorded rather than argued away.

---

## 13. The programme: stages UBC-0 to UBC-9

Each stage states an objective, what it waits on, and an exit gate written as conditions a run can
decide. **None is scheduled, none has an owner, and none has a ledger row**; assigning any of the
three is the act that would make a stage a milestone somebody is tracking. Owning code is not
acceptance: a stage whose every clause were met would still be a stage. Where a stage would file a
record, the record is named and not written here.

### UBC-0 — The extraction record, the correspondence table and the records that move

- **Objective.** Section 8's gate is invoked with real merged code on both sides, and every record
  this concept asks to move is moved or refused, before a line of the shared assembly exists.
- **Waits on.** Nothing. Both implementations are in the checkout.
- **Exit gate.** A correspondence table between `JsVerifier`'s walk and `WasmValidator`'s, between
  `JsEngine.ExecuteCore`'s loop and `WasmInterpreter`'s, between the two frame models, the two
  unwinding protocols and the two charge sites, with file paths and source revisions, shown as a
  correspondence rather than asserted, and with the language-free part identified from it (G2); an
  extraction record filed as **ADR 0013**, not contract-bearing, carrying the record clause's six
  items (the two implementations, the measured duplication, the proposed public surface with no
  identifier from any language's vocabulary, the graph edges demonstrating G4, the two named consumers,
  the verdict with its date and deciding owner), and rules E2 and E4 passing over the index; the
  standing refusal of ADR 0011 reopened by a new dated verdict row rather than an edit; the roadmap's
  non-goal and section 8 table row revised with the superseded text quoted, through ADR 0003 section
  11's roadmap-amendment register; P1's route filed as an MVP row; the JavaScript plan's section 9
  intermediate-form sentence discharged in a dated decision (**JSD-0036**) that names the universal
  bytecode as the form, closing JSB-3's open clause; the WebAssembly plan's compiler, framing and
  second-execution-arm entries amended with `WAC-nn` corrections; and the composition register's
  missing explanation of the MachineCode extraction supplied in the same ADR. **A verdict of refuse on
  any condition ends the programme here with the record filed**, which is a passing gate.

### UBC-1 — `Broiler.VM.Ubc`: the format, the common family, the tables' schema, the walk

- **Objective.** The shared assembly exists, holds no language concept, reads and refuses its own
  format, and is pinned by a corpus of its own before any family exists.
- **Waits on.** UBC-0's verdict.
- **Exit gate.** The assembly references exactly Abstractions and Binary (rule **U1**); its exported
  identifiers contain none of a banned vocabulary (`Js`, `Wasm`, `JavaScript`, `WebAssembly`, and every
  family mnemonic prefix) and no family row (rule **U2**), watched failing on an injected identifier;
  the common family's every row is in Appendix A and nowhere else, with a stack-effect table that the
  verifier, the encoder and the interpreter all read (rule **U4**, the analogue of the JavaScript
  format's "one place the two halves meet"); the container codec round-trips every section byte for
  byte; the walk refuses a retained malformed corpus of its own — self-authored bytes, each with its
  hash and pinned answer, replayed across three publish modes with byte-identical failure-class tables,
  mutated-entry detection, and control entries that verify — with every refusal mapping to one core
  reason and one universal diagnostic code, registered in both directions; every operand shape,
  effect-descriptor form and target-descriptor form of 5.4 has a corpus entry that exercises it; ADR
  0001 revised with the new counts and A7 and A15 green; and the public surface of the assembly
  captured in a baseline of its own under `docs/api/`, compared in both directions.

### UBC-2 — `Broiler.VM.Profile.Bytecode` and the fixture family

- **Objective.** The universal bytecode executes, over a family that is not a language, in one loop
  generic over the family.
- **Waits on.** UBC-1.
- **Exit gate.** The fixture family of 8.3 exists under `src/tests/` against the public surface alone,
  composed by a demonstration root that publishes and runs under JIT, trimming and Native AOT with its
  closure read off the published output; emitter obligations E1, E3, E4 and E6 hold over the fixture
  family's retained program corpus; `UbcDescriptors.Build` produces a descriptor the catalog admits,
  whose thirty rows equal the declaration's where the declaration supplies them and the universal
  bytecode's where it does; the loop's Native AOT instantiation over the fixture family folds the
  family tests away (route UBC-R7, shown on the image); guest recursion is refused as
  `ResourceExhaustion` naming `CallDepth` on every published mode rather than terminating the process;
  a suspension round-trips through the codec with both planes restored; and the two-profile catalog
  test — the fixture family beside a deliberately adverse neighbour, the neighbour's maxima not reaching
  it while its adopted defaults do — exists in that root, which is the row roadmap section 14 has asked
  for since VM-3.

### UBC-3 — The JavaScript family

- **Objective.** The JavaScript lowering has one exit, the profile has no loop, no walk and no format
  of its own, and every verdict the bytecode form gives today it gives after.
- **Waits on.** UBC-2.
- **Exit gate.** Rule N1's reference list is Abstractions, Binary, `Broiler.VM.Ubc` and the format;
  N3 reads "exactly `Broiler.VM.Ubc`"; N12, N19 and N20 hold unchanged; N16 is retired with its reason
  recorded (there is one format and it is not this family's); the wide lowering is deterministic over
  the whole wide and module corpora, compiled twice and compared byte for byte, **before** any emitter
  consumes its output, with the global lexical hoisting order derived from source and its negative
  control watched (JSB-1's gate, met here); every `JsOpcode` and `JavaScriptOpcode` member has a row in
  Appendix B's table or a recorded reason for its absence; the retained corpus is re-based entry by
  entry with each old answer carried or its new answer recorded, and N5 to N8 hold in both directions
  over the result; **the whole pinned test262 suite driven under the wide manifest by
  `eng/run-test262.py` gives the same verdict per variant as the last retained run of the old
  interpreter, outside the classes the ratchet's re-base records by name**, and the floors are re-based
  by hand with retired rows and reasons; the fifteen pinned Octane benchmarks report a score in the new
  form (a score, not a figure this document states); the fuel-parity twins give one verdict at every
  ceiling between the old interpreter and the new loop; the execution-only root's closure holds no
  compiler, no encoder and no arming path; and `JsNativeCompiler`, `JsNativePage`, `JsNativeExecution`
  and the two retired formats are gone from the tree, with `HUMAN_REVIEW.md` regenerated.

### UBC-4 — The WebAssembly family

- **Objective.** The WebAssembly profile translates, owns its family, and executes through the
  emitter, with the suite's verdicts unchanged.
- **Waits on.** UBC-2; independent of UBC-3.
- **Exit gate.** Rule W1's list gains `Broiler.VM.Ubc` and nothing else; the translator is
  deterministic over the harness corpus; every `WasmOpcode` member has a row in Appendix C's table
  (as a common mapping or a family row); the `wasm.*` handlers are the interpreter's arms and the
  primitive rows are checked against them by E2 over a retained input corpus, **with the
  float-comparison defect of 2.2 as the negative control — the check fails on the checkout's arms and
  passes after the correction, both watched**; the specification's test suite driven through the
  harness gives the same verdict per assertion as the last run before the translator, outside classes
  named by the re-based ratchet; `memory.grow`'s guest-observable refusal (route MVP-1) is unchanged;
  the memory representation decision of 8.2 is taken and recorded in that profile's series; the WA-5
  manifest is minted or its absence recorded; and the WebAssembly execution root's closure holds the
  translator and its register row's sibling cell says so.

### UBC-5 — The machine-code family's pivot and execution half

- **Objective.** One arming path, one activation, one handler-table mechanism and one scan, for every
  family, in a family of their own.
- **Waits on.** UBC-2.
- **Exit gate.** `Broiler.VM.Profile.MachineCode.Format` holds the frames, conventions, template
  tables and scan, references exactly `Broiler.VM.Ubc`, and names no family (U3); rules B5c and X1 name
  `Broiler.VM.Profile.MachineCode` and `VmNativePage` in their statements as their tests already do,
  and X2 is generalised to every frame type the pivot declares; X3's `[UnmanagedCallersOnly]` sweep
  names the machine-code family's wrapper file and nothing else; the thread-static activation slot's
  three properties are a registered text rule over that family's source; the form verifier layer runs
  the scan in every image and re-emission through a descriptor carrying the encoder; and `JsNativePage`
  and every hook that filled it are gone.

### UBC-6 — The `x86-64` emitter

- **Objective.** Every family table the emitter admits is emitted, armed, executed and compared, under
  both conventions.
- **Waits on.** UBC-5; UBC-3 and UBC-4 for the language families it admits.
- **Exit gate.** E1 to E9 hold over the fixture family, the JavaScript wide and numeric tables and
  the WebAssembly tables; **the numeric manifest's forms comparison (`src/tests/forms`,
  `eng/compare-forms.py`) runs both forms of every kernel and prints the emitter's refusal for the
  named Octane file exactly as it does today**; the whole test262 suite in the `x86-64` form gives the
  bytecode form's verdicts outside JSD-0025 section 11's three classes; the WebAssembly suite in the
  `x86-64` form gives the bytecode form's verdicts, which is the first native WebAssembly run this
  repository has held and is a capability and not a figure; the scan accepts everything both
  conventions emit, every template is reached by the corpus or named as unreached, and the negative
  half — legal machine code no emitter emits — is refused by name; re-emission equality holds across
  the corpus; the emitter's semantic version rule of JSD-0025 section 7 is restated for the pivot's
  tables; and the support table's section 3a names the form's runtime-identifier status exactly as
  today — **none claimed**.

### UBC-7 — The `arm64` emitter, emitting-only

- **Objective.** A second architecture against a fixed contract, pinned without a run.
- **Waits on.** UBC-6, for the pivot's shape to have met a second family.
- **Exit gate.** A golden-byte test per template and a disassembler-checked corpus, both failing on a
  changed byte; an artifact in the form verifies through the scan and refuses to instantiate on every
  host by name; the support table's row says *emitting-only*, claims no runtime identifier and carries
  no figure, and a rule fails the release where it does not; and which family tables it admits is
  stated per table with a refusal by name for the rest.

### UBC-8 — The polyglot composition

- **Objective.** One interpreter, two families, one image.
- **Waits on.** UBC-3 and UBC-4.
- **Exit gate.** `Broiler.VM.Composition.PolyglotCli` composes the JavaScript and WebAssembly
  registrations into one bytecode emitter, publishes and runs under JIT, trimming and Native AOT with
  its closure read off the output and holding exactly its register row's assemblies; a JavaScript
  program and a WebAssembly module run in one process from one command line with one set of stated
  ceilings; the two-profile catalog test of UBC-2 is repeated over the two product families; **a
  negative control asserts that no unit of either artifact can name the other family's slot** — an
  artifact declaring both families is refused at verification by the walk; and the cross-profile call
  chain, where the root writes one, is bounded by the shared parent's `CallDepth`, witnessed by the
  two-row case the roadmap's risk row asks for.

### UBC-9 — The records, the support table and the packability decision

- **Objective.** What exists is described exactly, and nothing more.
- **Waits on.** Whichever stages have landed; this stage can run after any of them.
- **Exit gate.** Section 3a of the support table is the emitter table: one row per form, with
  *exists*, *published and run* and *deterministic refusal elsewhere* answered on retained evidence and
  *emitting-only* in the row where it applies; the composition register's rows carry the sibling cells
  and native-execution cells of section 9; ADR 0001's budget sentence, `graph.manifest.json` and the
  tree agree; the packability of `Broiler.VM.Ubc` is answered in a dated ADR 0001 revision — a fourth
  package naming the boundary it enforces, or a refusal — and rules A6 and C1 say what the answer says;
  every MVP route row of section 12 is filed in `docs/mvp.md` with its state; and every correction this
  programme made is an entry in the file that owns it with the superseded text quoted.

---

## 14. What would falsify this concept

- **An emitter that names a family.** A `switch` on a family identity, an opcode of any language, or a
  type from a language assembly anywhere in `Broiler.VM.Ubc` or an emitter family. Rules U2 and U3 are
  the automated form; a reviewer finding one by reading is the same falsification.
- **A common row whose meaning depends on the family**, or two families that need one common row to
  mean two things. That is the lowest-common-denominator model invariant 4 refuses, arriving through
  the door this concept opened, and the answer is to move the row into both families rather than to
  parameterise it.
- **A `Primitive` row whose inline implementation disagrees with its handler** on any input of the
  retained corpus, in any emitter, after the family classified it. Either the classification is wrong
  or the table is, and E2 is the check.
- **An emitted frame, a plane helper, or a handler-table slot holding a reference the collector
  traces.** Section 7.3's rooting statement is the whole argument, and one such reference reopens
  MVP-3 as that row says it would.
- **A verdict of the bytecode form after UBC-3 or UBC-4 that differs from the last retained verdict of
  the old interpreter for the same variant**, outside the classes the re-based ratchet names. The
  refactoring claims to move machinery and not meaning, and one such variant is meaning that moved.
- **A fuel ceiling at which two forms of one program that loads nothing disagree.** E6 is exactness.
- **A translation that is not deterministic**, in either language: re-emission equality, the scan
  and the corpus discipline all rest on it.
- **The core referencing `Broiler.VM.Ubc`**, or a universal bytecode type appearing in the core's public
  API baseline. Rules B1, B2 and M1 are the automated form.
- **A composition in which one artifact can be executed in two forms**, by any path, including a
  guest-loaded program compiled in a form other than its instance's. That is the second execution arm
  under a new name.
- **A `v` of one family and a slot of another in one frame**, or an artifact declaring two language
  families that the walk admits. That is the cross-profile value channel under a new name.

---

## 15. What this concept does not do, stated flatly

- **No speed claim**, in any direction, for any form, family or emitter, and no figure of any kind.
- **No decision.** Nothing here is taken, accepted, approved or signed; every record it names is
  filed by a stage, and stage UBC-0's verdict may be a refusal.
- **No schedule and no owner.** Stages are proposals; a ledger row is the act that would change that.
- **No core contract amendment**, with the one reading of P1 recorded as route UBC-R1.
- **No runtime identifier**, no advertised composition, no fourth package during the MVP, no
  publication.
- **No cross-profile value channel**, no shared value, no shared realm, no shared syntax tree.
- **No change to what the JavaScript language or the WebAssembly language means here**: the
  conformance suites decide that, and both stages' gates are that they decide the same thing before and
  after.
- **No review.** Everything this concept would touch is `HUMAN_PENDING` today and would be
  `HUMAN_PENDING` after, and moving unreviewed code is not reviewing it.

---

## Appendix A — The common family

Unprefixed opcodes `0x00` to `0xEF`; the ones defined by universal bytecode format version 1 are
listed. Effects read bottom to top, `t` any slot type, `w` any word type, `n`, `k` the encoded
operand. Every row costs one fuel unit. Every row is executed by every emitter itself.

| Byte | Mnemonic | Operand | Effect | Meaning |
|---|---|---|---|---|
| `0x00` | `nop` | — | `[] → []` | nothing; never emitted by a translator, admitted so that a patched stream has a legal filler |
| `0x01` | `trap` | `u8 u16` | terminal | abort: family slot `u8` builds the fault payload from code `u16`; slot 0 code 0 is the universal *unreachable*; no region catches it |
| `0x02` | `jump` | `u32` | terminal | continue at absolute code offset `u32`, inside the same unit |
| `0x03` | `jump_if_zero` | `u32` | `[i32] → []` | jump when the popped word is zero |
| `0x04` | `jump_if_nonzero` | `u32` | `[i32] → []` | jump when the popped word is not zero |
| `0x05` | `jump_table` | `u16` | `[i32] → []` | jump to row `i32` of jump table `u16`, or to its last row when out of range |
| `0x06` | `return` | — | `[… results] → terminal` | return the unit's declared results from the top of the stack; whatever is beneath is discarded |
| `0x07` | `call` | `u32` | `[params] → [results]` | call unit `u32` of the same artifact with the signature its Units row declares; `CallDepth` charged; word parameters copied, value parameters plane-copied |
| `0x08` | `drop` | — | `[t] → []` | discard the top |
| `0x09` | `dup` | — | `[t] → [t t]` | copy the top |
| `0x0A` | `dup2` | — | `[t u] → [t u t u]` | copy the top two, in order |
| `0x0B` | `swap` | — | `[t u] → [u t]` | exchange the top two |
| `0x0C` | `pick` | `u8` | `[t …] → [t … t]` | copy the value `u8` places below the top |
| `0x0D` | `select` | — | `[t t i32] → [t]` | keep the first when the word is not zero, the second otherwise; both candidates one type |
| `0x0E` | `squash` | `u8 u8` | `[… n k] → [k]` | keep the top `k` slots and discard the `n` beneath them; how a WebAssembly branch unwinds to a label's height |
| `0x10` | `local.get` | `u16` | `[] → [t]` | push local `u16`, typed by the unit's local table |
| `0x11` | `local.set` | `u16` | `[t] → []` | pop into local `u16` |
| `0x12` | `local.tee` | `u16` | `[t] → [t]` | store into local `u16` and keep the value |
| `0x20` | `const.i32` | `i32` | `[] → [i32]` | push the constant |
| `0x21` | `const.i64` | `i64` | `[] → [i64]` | push the constant |
| `0x22` | `const.f32` | `f32` | `[] → [f32]` | push the constant, bits as written |
| `0x23` | `const.f64` | `f64` | `[] → [f64]` | push the constant, bits as written |
| `0xF0`–`0xFE` | *family prefix* | — | — | the next byte is an opcode of family slot `byte − 0xF0` |
| `0xFF` | *reserved* | — | — | extended prefix; not defined by format version 1, refused |

**Rules of the table.** A row's meaning is stated here and in the assembly's one stack-effect table
(rule U4) and nowhere else. Adding a row is a universal bytecode format version. `jump`, `trap` and
`return` are the terminal rows; every path of every unit ends in one. A `v`-typed `dup`, `pick`,
`swap`, `select`, `local.*` or `squash` is a plane operation the emitter performs through the family's
plane helper; the same rows over words are register or slot moves.

## Appendix B — The JavaScript family

Slot identity `broiler.javascript`; one table per manifest. The **wide** table is the reference and is
listed row for row against `JsOpcode`, byte values kept. The **numeric** table selects the forty-seven
rows `JsNumericManifest.AdmittedOpcodes` admits and reclassifies them over word slots as the last column
says; every other row is absent from it, so an artifact naming the numeric manifest and using an
absent row is refused at verification by the walk, one stage after the front end refused the construct
by name. The **slice** table (format version 1's `JavaScriptOpcode`) follows.

Kinds: **D** `Dynamic`, **B** `Branch`, **C** `Call`, **S** `Suspend`, **T** `Throw`, **→** lowered to
common rows. Effects are the wide interpreter's, from `JsOpcodes.TryDescribe`.

| Byte | `JsOpcode` | Universal mnemonic | Operand | Effect (wide, over `v`) | Kind | Numeric table |
|---|---|---|---|---|---|---|
| `0x00` | `Nop` | → `nop` | — | `[] → []` | → | absent (the numeric lowering writes none) |
| `0x01` | `LoadUndefined` | `js.load_undefined` | — | `[] → [v]` | D | → `const.f64` of the family's reserved *undefined* word; D where the stage finds a case the admission does not pin |
| `0x02` | `LoadNull` | `js.load_null` | — | `[] → [v]` | D | absent |
| `0x03` | `LoadTrue` | `js.load_true` | — | `[] → [v]` | D | absent |
| `0x04` | `LoadFalse` | `js.load_false` | — | `[] → [v]` | D | absent |
| `0x05` | `LoadConstant` | `js.load_constant` | `u16` | `[] → [v]` | D | `[] → [f64]`, hook requires a Number pool entry |
| `0x06` | `LoadThis` | `js.load_this` | — | `[] → [v]` | D | absent |
| `0x07` | `NewArguments` | `js.new_arguments` | — | `[] → [v]` | D | absent |
| `0x08` | `LoadNewTarget` | `js.load_new_target` | — | `[] → [v]` | D | absent |
| `0x09` | `LoadArgument` | `js.load_argument` | `u16` | `[] → [v]` | D | absent |
| `0x0A` | `RestArguments` | `js.rest_arguments` | `u16` | `[] → [v]` | D | absent |
| `0x10` | `LoadScoped` | `js.load_scoped` | `u8 u16` | `[] → [v]` | D | `[] → [f64]`; or → `local.get` where the front end's static scoping proves the binding local (the numeric walk's flattening, moved into the lowering) |
| `0x11` | `StoreScoped` | `js.store_scoped` | `u8 u16` | `[v] → []` | D | `[f64] → []`; or → `local.set` |
| `0x12` | `InitialiseScoped` | `js.initialise_scoped` | `u8 u16` | `[v] → []` | D | `[f64] → []`; or → `local.set` |
| `0x13` | `LoadGlobal` | `js.load_global` | `u16` | `[] → [v]` | D | `[] → [f64]`, D (the realm's global object is the family's) |
| `0x14` | `StoreGlobal` | `js.store_global` | `u16` | `[v] → []` | D | `[f64] → []`, D |
| `0x15` | `LoadGlobalOrUndefined` | `js.load_global_or_undefined` | `u16` | `[] → [v]` | D | absent |
| `0x16` | `PushScope` | `js.push_scope` | `u16` | `[] → []` | D | D, or absent where flattened |
| `0x17` | `PopScope` | `js.pop_scope` | — | `[] → []` | D | D, or absent where flattened |
| `0x18` | `CopyScope` | `js.copy_scope` | `u16` | `[] → []` | D | D, or absent where flattened |
| `0x19` | `DeclareGlobal` | `js.declare_global` | `u16` | `[] → []` | D | D |
| `0x1A` | `PushObjectScope` | `js.push_object_scope` | — | `[v] → []` | D | absent |
| `0x1B` | `ResolveName` | `js.resolve_name` | `u8 u16` | `[] → [v]` | D | absent |
| `0x20` | `NewObject` | `js.new_object` | — | `[] → [v]` | D | absent |
| `0x21` | `NewArray` | `js.new_array` | `u16` | `[v×n] → [v]` | D | absent |
| `0x22` | `GetProperty` | `js.get_property` | `u16` | `[v] → [v]` | D | absent |
| `0x23` | `SetProperty` | `js.set_property` | `u16` | `[v v] → [v]` | D | absent |
| `0x24` | `GetIndex` | `js.get_index` | — | `[v v] → [v]` | D | absent |
| `0x25` | `SetIndex` | `js.set_index` | — | `[v v v] → [v]` | D | absent |
| `0x26` | `DefineField` | `js.define_field` | `u16` | `[v] → []` (object beneath stays) | D | absent |
| `0x27` | `DefineIndexed` | `js.define_indexed` | — | `[v v] → []` | D | absent |
| `0x28` | `DeleteProperty` | `js.delete_property` | `u16` | `[v] → [v]` | D | absent |
| `0x29` | `DeleteIndex` | `js.delete_index` | — | `[v v] → [v]` | D | absent |
| `0x2A` | `DefineGetter` | `js.define_getter` | `u16` | `[v] → []` | D | absent; defined and never emitted, kept for the reason the format keeps it |
| `0x2B` | `DefineSetter` | `js.define_setter` | `u16` | `[v] → []` | D | absent; as above |
| `0x2C` | `DefineMethod` | `js.define_method` | `u8` | `[v v] → []` | D | absent |
| `0x2D` | `LoadSuperProperty` | `js.load_super_property` | — | `[v] → [v]` | D | absent |
| `0x2E` | `StoreSuperProperty` | `js.store_super_property` | — | `[v v] → [v]` | D | absent |
| `0x2F` | `ArrayAppend` | `js.array_append` | — | `[v] → []` | D | absent |
| `0x30` | `Closure` | `js.closure` | `u16` | `[] → [v]` | D | D: a function value the numeric admission allows to be called by name only |
| `0x31` | `Call` | `js.call` | `u8` | `[v v v×n] → [v]` | C | C over `[v v f64×n] → [f64]`; the request names a unit of the artifact |
| `0x32` | `Construct` | `js.construct` | `u8` | `[v v×n] → [v]` | C | absent |
| `0x33` | `Return` | → `return` | — | `[v] → terminal` | → | → `return` of one `f64` |
| `0x34` | `ReturnUndefined` | → `js.load_undefined` `return` | — | `[] → terminal` | → | → `const.f64` *undefined* word, `return` |
| `0x35` | `CallEval` | `js.call_eval` | `u8` | `[v v v×n] → [v]` | C | absent |
| `0x36` | `SuperCall` | `js.super_call` | `u8` | `[v×n] → [v]` | C | absent |
| `0x37` | `SuperCallForwarded` | `js.super_call_forwarded` | — | `[] → [v]` | C | absent |
| `0x38` | `NewClass` | `js.new_class` | `u8` | `[v] → [v]`, or `[v v] → [v]` with the derived bit | D | absent |
| `0x39` | `ArrayHoles` | `js.array_holes` | `u16` | `[] → []` | D | absent |
| `0x3A` | `SpreadArray` | `js.spread_array` | — | `[v] → []` | D | absent |
| `0x3B` | `SpreadObject` | `js.spread_object` | — | `[v] → []` | D | absent |
| `0x3C` | `CallSpread` | `js.call_spread` | — | `[v v v] → [v]` | C | absent |
| `0x3D` | `ConstructSpread` | `js.construct_spread` | — | `[v v] → [v]` | C | absent |
| `0x3E` | `SuperCallSpread` | `js.super_call_spread` | — | `[v] → [v]` | C | absent |
| `0x3F` | `SetPrototypeLiteral` | `js.set_prototype_literal` | — | `[v] → []` | D | absent |
| `0x40` | `Add` | `js.add` | — | `[v v] → [v]` | D | **Primitive `f64.add`** |
| `0x41` | `Subtract` | `js.subtract` | — | `[v v] → [v]` | D | **Primitive `f64.sub`** |
| `0x42` | `Multiply` | `js.multiply` | — | `[v v] → [v]` | D | **Primitive `f64.mul`** |
| `0x43` | `Divide` | `js.divide` | — | `[v v] → [v]` | D | **Primitive `f64.div`** |
| `0x44` | `Remainder` | `js.remainder` | — | `[v v] → [v]` | D | D over `[f64 f64] → [f64]` (no primitive; today's emitter refuses `%`) |
| `0x45` | `Exponent` | `js.exponent` | — | `[v v] → [v]` | D | D over words (no primitive) |
| `0x46` | `Negate` | `js.negate` | — | `[v] → [v]` | D | **Primitive `f64.neg`** |
| `0x47` | `ToNumber` | `js.to_number` | — | `[v] → [v]` | D | **Primitive `word.keep`** (a Number is its own `ToNumber`) |
| `0x48` | `Not` | `js.not` | — | `[v] → [v]` | D | **Primitive `i32.eqz`** over `[i32] → [i32]`; the admission must keep a Number out of it, else D |
| `0x49` | `BitwiseNot` | `js.bitwise_not` | — | `[v] → [v]` | D | D over words (`ToInt32` is not the one available conversion; today's emitter refuses it) |
| `0x4A` | `LessThan` | `js.less_than` | — | `[v v] → [v]` | D | **Primitive `f64.lt`** over `[f64 f64] → [i32]` |
| `0x4B` | `LessThanOrEqual` | `js.less_than_or_equal` | — | `[v v] → [v]` | D | **Primitive `f64.le`** |
| `0x4C` | `GreaterThan` | `js.greater_than` | — | `[v v] → [v]` | D | **Primitive `f64.gt`** |
| `0x4D` | `GreaterThanOrEqual` | `js.greater_than_or_equal` | — | `[v v] → [v]` | D | **Primitive `f64.ge`** |
| `0x4E` | `StrictEquals` | `js.strict_equals` | — | `[v v] → [v]` | D | **Primitive `f64.eq`** |
| `0x4F` | `StrictNotEquals` | `js.strict_not_equals` | — | `[v v] → [v]` | D | **Primitive `f64.ne`** |
| `0x50` | `LooseEquals` | `js.loose_equals` | — | `[v v] → [v]` | D | **Primitive `f64.eq`** (over Numbers the two equalities agree) |
| `0x51` | `LooseNotEquals` | `js.loose_not_equals` | — | `[v v] → [v]` | D | **Primitive `f64.ne`** |
| `0x52` | `BitwiseOr` | `js.bitwise_or` | — | `[v v] → [v]` | D | D over words (see `BitwiseNot`) |
| `0x53` | `BitwiseAnd` | `js.bitwise_and` | — | `[v v] → [v]` | D | D over words |
| `0x54` | `BitwiseXor` | `js.bitwise_xor` | — | `[v v] → [v]` | D | D over words |
| `0x55` | `ShiftLeft` | `js.shift_left` | — | `[v v] → [v]` | D | D over words |
| `0x56` | `ShiftRight` | `js.shift_right` | — | `[v v] → [v]` | D | D over words |
| `0x57` | `ShiftRightUnsigned` | `js.shift_right_unsigned` | — | `[v v] → [v]` | D | D over words |
| `0x58` | `TypeOf` | `js.typeof` | — | `[v] → [v]` | D | absent |
| `0x59` | `InstanceOf` | `js.instanceof` | — | `[v v] → [v]` | D | absent |
| `0x5A` | `In` | `js.in` | — | `[v v] → [v]` | D | absent |
| `0x5B` | `Void` | `js.void` | — | `[v] → [v]` | D | absent |
| `0x5C` | `RequireCoercible` | `js.require_coercible` | `u16` | `[v] → [v]` | D | absent |
| `0x5D` | `ToPropertyKey` | `js.to_property_key` | — | `[v v] → [v v]` | D | absent |
| `0x5E` | `GetTemplateObject` | `js.get_template_object` | `u8` | `[v×2n] → [v]` | D | absent |
| `0x60` | `Jump` | → `jump` | `u32` | terminal | → | → `jump` |
| `0x61` | `JumpIfFalse` | `js.jump_if_false` | `u32` | `[v] → []` | B | → `jump_if_zero` over the `i32` a comparison primitive left; B where the operand is an `f64` (`ToBoolean` of a Number) |
| `0x62` | `JumpIfTrue` | `js.jump_if_true` | `u32` | `[v] → []` | B | → `jump_if_nonzero`; B likewise |
| `0x63` | `Throw` | `js.throw` | — | `[v] → []` | T | absent |
| `0x64` | `ForInStart` | `js.for_in_start` | — | `[v] → [v]` | D | absent |
| `0x65` | `ForInNext` | `js.for_in_next` | `u32` | `[v] → [v]`; taken edge `[v] → []` | B | absent |
| `0x66` | `IterateStart` | `js.iterate_start` | — | `[v] → [v]` | D | absent |
| `0x67` | `IterateNext` | `js.iterate_next` | `u32` | `[v] → [v]`; taken `[v] → []` | B | absent |
| `0x68` | `IterateRest` | `js.iterate_rest` | — | `[v] → [v]` | D | absent |
| `0x69` | `IterateClose` | `js.iterate_close` | `u8` | `[v] → []` | D | absent |
| `0x6A` | `Yield` | `js.yield` | — | `[v] → [v]` | S | absent |
| `0x6B` | `YieldDelegate` | `js.yield_delegate` | — | `[v] → [v]` | S | absent |
| `0x6C` | `Await` | `js.await` | — | `[v] → [v]` | S | absent |
| `0x6D` | `LoadImport` | `js.load_import` | `u16` | `[] → [v]` | D | absent |
| `0x6E` | `ThrowImmutable` | `js.throw_immutable` | `u16` | `[v] → []` | T (not terminal: the row falls through for the verifier, as today) | absent |
| `0x6F` | `DefineClassElement` | `js.define_class_element` | `u8` | `[v v] → []` (pair beneath stays) | D | absent |
| `0x70` | `Pop` | → `drop` | — | `[v] → []` | → | → `drop` |
| `0x71` | `Duplicate` | → `dup` | — | `[v] → [v v]` | → | → `dup` |
| `0x72` | `DuplicateTwo` | → `dup2` | — | `[v v] → [v v v v]` | → | absent |
| `0x73` | `Swap` | → `swap` | — | `[v v] → [v v]` | → | absent |
| `0x74` | `Pick` | → `pick` | `u8` | `[…] → [… v]` | → | absent |
| `0x75` | `NewPrivateName` | `js.new_private_name` | `u16` | `[] → [v]` | D | absent |
| `0x76` | `LoadPrivate` | `js.load_private` | — | `[v v] → [v]` | D | absent |
| `0x77` | `StorePrivate` | `js.store_private` | — | `[v v v] → [v]` | D | absent |
| `0x78` | `HasPrivate` | `js.has_private` | — | `[v v] → [v]` | D | absent |
| `0x79` | `RunStaticElements` | `js.run_static_elements` | — | `[] → []` | D | absent |
| `0x7A` | `IterateStartAsync` | `js.iterate_start_async` | — | `[v] → [v]` | D | absent |
| `0x7B` | `IterateNextAsync` | `js.iterate_next_async` | — | `[] → [v]` (record beneath stays) | D | absent |
| `0x7C` | `IterateAwaitStep` | `js.iterate_await_step` | `u32` | `[v v] → [v]`; taken `[v v] → []` | B | absent |
| `0x7D` | `IterateCloseAsync` | `js.iterate_close_async` | `u32` | `[v] → [v]`; taken `[v] → []` | B | absent |
| `0x7E` | `IterateCloseCheck` | `js.iterate_close_check` | — | `[v] → []` | D | absent |
| `0x7F` | `DeclareGlobalLet` | `js.declare_global_let` | `u16` | `[] → []` | D | D |
| `0x80` | `DeclareGlobalConst` | `js.declare_global_const` | `u16` | `[] → []` | D | D |
| `0x81` | `InitialiseGlobalLexical` | `js.initialise_global_lexical` | `u16` | `[v] → []` | D | D over `[f64] → []` |
| `0x82` | `DeleteGlobalBinding` | `js.delete_global_binding` | `u16` | `[] → [v]` | D | absent |
| `0x83` | `EnterBody` | `js.enter_body` | — | `[] → []` | S (a seam: a resumable landing that moves no operand) | absent |
| `0x85` | `ImportCall` | `js.import_call` | `u16` | `[v v] → [v]` | C | absent |
| `0x86` | `ImportMeta` | `js.import_meta` | `u16` | `[] → [v]` | D | absent |
| `0x90` | `CallEvalSpread` | `js.call_eval_spread` | — | `[v v v] → [v]` | C | absent |
| `0x91` | `LoadEvalName` | `js.load_eval_name` | `u8 u16` | `[] → [v]` | D | absent |
| `0x92` | `LoadEvalNameOrUndefined` | `js.load_eval_name_or_undefined` | `u8 u16` | `[] → [v]` | D | absent |
| `0x93` | `StoreEvalName` | `js.store_eval_name` | `u8 u16` | `[v] → []` | D | absent |
| `0x94` | `LoadEvalNameWithBase` | `js.load_eval_name_with_base` | `u8 u16` | `[] → [v v]` | D | absent |
| `0x95` | `DeleteEvalName` | `js.delete_eval_name` | `u8 u16` | `[] → [v]` | D | absent |
| `0x9A` | `WithBaseObject` | `js.with_base_object` | — | `[v] → [v]` | D | absent |
| `0x9B` | `StoreEvalVariable` | `js.store_eval_variable` | `u8 u16` | `[v] → []` | D | absent |
| `0xA0` | `DisposeScope` | `js.dispose_scope` | — | `[] → [v]` | D | absent |
| `0xA1` | `DisposeAdd` | `js.dispose_add` | `u8` | `[v] → []` (value beneath stays) | D | absent |
| `0xA2` | `DisposeFold` | `js.dispose_fold` | — | `[v v] → []` | D | absent |
| `0xA3` | `DisposeStep` | `js.dispose_step` | `u32` | `[v] → [v]`; taken `[v] → []` | B | absent |
| `0xA4` | `DisposeEnd` | `js.dispose_end` | `u8` | `[v] → []`, or `[v] → [v]` with operand one | D | absent |
| `0xB0` | `ToNumeric` | `js.to_numeric` | — | `[v] → [v]` | D | absent |
| `0xB1` | `Increment` | `js.increment` | — | `[v] → [v]` | D | absent |
| `0xB2` | `Decrement` | `js.decrement` | — | `[v] → [v]` | D | absent |

**Rows the wide table adds that `JsOpcode` does not have: none.** What lands at a region's handler is
the family's `OnLand` (5.10) and needs no instruction; a catch handler's first instruction today is
already written against an exception the executor pushed.

**Format version 1, the slice table.** The thirty rows of `JavaScriptOpcode` map to: `LoadConstant`
→ `js.load_constant`; `LoadLocal`/`StoreLocal` → `local.get`/`local.set` of type `v`;
`ThrowUninitializedBinding` → `js.throw_uninitialised_binding` (kind **T**, moving no operand, not
terminal, as its own remark insists); the arithmetic, unary, comparison and bitwise rows → the wide
rows of the same names (byte values differ between the two tables, and the slice table keeps its own);
`Jump`/`JumpIfFalse`/`JumpIfTrue` → `jump`/`js.jump_if_false`/`js.jump_if_true` with **absolute**
targets the translator computes from the relative ones; `Pop`/`Duplicate` → `drop`/`dup`; `Return` →
`return`. One observation, recorded for the stage: `JavaScriptOpcodes.All` omits
`ThrowUninitializedBinding` while `IsDefined` accepts it, so the table the stage builds from `All`
would be one row short and the corpus would say so.

## Appendix C — The WebAssembly family

Slot identity `broiler.webassembly`; one table per manifest (`broiler.webassembly.slice` today, and
the WA-5 manifest when it is minted). Byte values kept from the W3C encoding for the rows that stay in
the family. Effects are the specification's, over word slots only; the family declares no `v`. Kinds as
in Appendix B, plus **P** `Primitive` (the Appendix D entry named) and **→** lowered by the translator
to common rows. The translator's structural lowering is stated once above the table.

**Structural lowering.** `block`, `loop`, `if`, `else` and `end` (`0x02`–`0x05`, `0x0B`) produce no
row: every label's target and arity come from the validator's sealed jump table. `if` becomes
`jump_if_zero` to the `else` or `end` offset; `else` becomes `jump` to `end`; a `loop` label targets
its start and every other label its end; dead code after an unconditional transfer is dropped. A branch
whose label height is below the current height less its arity is preceded by `squash n k`; a
conditional branch that needs one branches to a trampoline holding the `squash` and the `jump`. A
`br_table` whose targets need differing adjustments gets one trampoline per distinct adjustment. The
translator is deterministic: trampolines are emitted in first-use order after the unit's body.

| Byte | W3C name | Universal row | Operand | Effect | Kind |
|---|---|---|---|---|---|
| `0x00` | `unreachable` | → `trap` slot 0 | `u8 u16` | terminal | → |
| `0x01` | `nop` | → `nop` | — | `[] → []` | → |
| `0x02`–`0x05`, `0x0B` | `block` `loop` `if` `else` `end` | structural, no row | | | → |
| `0x0C` | `br` | → `squash`? `jump` | `u32` | terminal | → |
| `0x0D` | `br_if` | → `jump_if_nonzero`, via a trampoline when adjusted | `u32` | `[i32] → []` | → |
| `0x0E` | `br_table` | → `jump_table`, rows in the JumpTables section, trampolines as needed | `u16` | `[i32] → []` | → |
| `0x0F` | `return` | → `return` | — | `[… results] → terminal` | → |
| `0x10` | `call` | → `call` | `u32` | signature-driven | → |
| `0x11` | `call_indirect` | `wasm.call_indirect` | `u16` (type index) | `[params i32] → [results]` | C — the handler reads table 0, refuses a null or out-of-range entry and a signature mismatch with the family's trap codes, and answers a request naming the unit |
| `0x1A` | `drop` | → `drop` | — | `[w] → []` | → |
| `0x1B` | `select` | → `select` | — | `[w w i32] → [w]` | → |
| `0x20`–`0x22` | `local.get` `local.set` `local.tee` | → `local.get` `local.set` `local.tee` | `u16` | typed by the unit's local table | → |
| `0x23` | `global.get` | `wasm.global.get` | `u16` | `[] → [w]` | P `region.load.w` over the `globals` region, offset `index × 8` |
| `0x24` | `global.set` | `wasm.global.set` | `u16` | `[w] → []` | P `region.store.w` over `globals`; the hook refuses a set of an immutable global |
| `0x28` | `i32.load` | `wasm.i32.load` | `u8 u32` | `[i32] → [i32]` | P `region.load.i32` over `memory0`, address `i32 + offset`, trap *out of bounds* |
| `0x29` | `i64.load` | `wasm.i64.load` | `u8 u32` | `[i32] → [i64]` | P `region.load.i64` |
| `0x2A` | `f32.load` | `wasm.f32.load` | `u8 u32` | `[i32] → [f32]` | P `region.load.f32` |
| `0x2B` | `f64.load` | `wasm.f64.load` | `u8 u32` | `[i32] → [f64]` | P `region.load.f64` |
| `0x2C` | `i32.load8_s` | `wasm.i32.load8_s` | `u8 u32` | `[i32] → [i32]` | P `region.load.i32.8s` |
| `0x2D` | `i32.load8_u` | `wasm.i32.load8_u` | `u8 u32` | `[i32] → [i32]` | P `region.load.i32.8u` |
| `0x2E` | `i32.load16_s` | `wasm.i32.load16_s` | `u8 u32` | `[i32] → [i32]` | P `region.load.i32.16s` |
| `0x2F` | `i32.load16_u` | `wasm.i32.load16_u` | `u8 u32` | `[i32] → [i32]` | P `region.load.i32.16u` |
| `0x30` | `i64.load8_s` | `wasm.i64.load8_s` | `u8 u32` | `[i32] → [i64]` | P `region.load.i64.8s` |
| `0x31` | `i64.load8_u` | `wasm.i64.load8_u` | `u8 u32` | `[i32] → [i64]` | P `region.load.i64.8u` |
| `0x32` | `i64.load16_s` | `wasm.i64.load16_s` | `u8 u32` | `[i32] → [i64]` | P `region.load.i64.16s` |
| `0x33` | `i64.load16_u` | `wasm.i64.load16_u` | `u8 u32` | `[i32] → [i64]` | P `region.load.i64.16u` |
| `0x34` | `i64.load32_s` | `wasm.i64.load32_s` | `u8 u32` | `[i32] → [i64]` | P `region.load.i64.32s` |
| `0x35` | `i64.load32_u` | `wasm.i64.load32_u` | `u8 u32` | `[i32] → [i64]` | P `region.load.i64.32u` |
| `0x36` | `i32.store` | `wasm.i32.store` | `u8 u32` | `[i32 i32] → []` | P `region.store.i32` |
| `0x37` | `i64.store` | `wasm.i64.store` | `u8 u32` | `[i32 i64] → []` | P `region.store.i64` |
| `0x38` | `f32.store` | `wasm.f32.store` | `u8 u32` | `[i32 f32] → []` | P `region.store.f32` |
| `0x39` | `f64.store` | `wasm.f64.store` | `u8 u32` | `[i32 f64] → []` | P `region.store.f64` |
| `0x3A` | `i32.store8` | `wasm.i32.store8` | `u8 u32` | `[i32 i32] → []` | P `region.store.i32.8` |
| `0x3B` | `i32.store16` | `wasm.i32.store16` | `u8 u32` | `[i32 i32] → []` | P `region.store.i32.16` |
| `0x3C` | `i64.store8` | `wasm.i64.store8` | `u8 u32` | `[i32 i64] → []` | P `region.store.i64.8` |
| `0x3D` | `i64.store16` | `wasm.i64.store16` | `u8 u32` | `[i32 i64] → []` | P `region.store.i64.16` |
| `0x3E` | `i64.store32` | `wasm.i64.store32` | `u8 u32` | `[i32 i64] → []` | P `region.store.i64.32` |
| `0x3F` | `memory.size` | `wasm.memory.size` | `u8` (reserved zero) | `[] → [i32]` | P `region.size` over `memory0`, in pages |
| `0x40` | `memory.grow` | `wasm.memory.grow` | `u8` (reserved zero) | `[i32] → [i32]` | D — allocation, charging, the guest-observable `−1` of route MVP-1 |
| `0x41` | `i32.const` | → `const.i32` | `i32` | `[] → [i32]` | → |
| `0x42` | `i64.const` | → `const.i64` | `i64` | `[] → [i64]` | → |
| `0x43` | `f32.const` | → `const.f32` | `f32` | `[] → [f32]` | → |
| `0x44` | `f64.const` | → `const.f64` | `f64` | `[] → [f64]` | → |
| `0x45` | `i32.eqz` | `wasm.i32.eqz` | — | `[i32] → [i32]` | P `i32.eqz` |
| `0x46`–`0x4F` | `i32.eq` `ne` `lt_s` `lt_u` `gt_s` `gt_u` `le_s` `le_u` `ge_s` `ge_u` | `wasm.i32.<name>` | — | `[i32 i32] → [i32]` | P `i32.<name>` |
| `0x50` | `i64.eqz` | `wasm.i64.eqz` | — | `[i64] → [i32]` | P `i64.eqz` |
| `0x51`–`0x5A` | `i64.eq` … `i64.ge_u` | `wasm.i64.<name>` | — | `[i64 i64] → [i32]` | P `i64.<name>` |
| `0x5B`–`0x60` | `f32.eq` `ne` `lt` `gt` `le` `ge` | `wasm.f32.<name>` | — | `[f32 f32] → [i32]` | P `f32.<name>` — **the rows the checkout's interpreter does not reach (2.2), and UBC-4's negative control** |
| `0x61`–`0x66` | `f64.eq` … `f64.ge` | `wasm.f64.<name>` | — | `[f64 f64] → [i32]` | P `f64.<name>` — likewise |
| `0x67`–`0x69` | `i32.clz` `ctz` `popcnt` | `wasm.i32.<name>` | — | `[i32] → [i32]` | P |
| `0x6A`–`0x6C` | `i32.add` `sub` `mul` | `wasm.i32.<name>` | — | `[i32 i32] → [i32]` | P |
| `0x6D`–`0x70` | `i32.div_s` `div_u` `rem_s` `rem_u` | `wasm.i32.<name>` | — | `[i32 i32] → [i32]` | P, traps *divide by zero*, and *integer overflow* for `div_s` |
| `0x71`–`0x78` | `i32.and` `or` `xor` `shl` `shr_s` `shr_u` `rotl` `rotr` | `wasm.i32.<name>` | — | `[i32 i32] → [i32]` | P |
| `0x79`–`0x7B` | `i64.clz` `ctz` `popcnt` | `wasm.i64.<name>` | — | `[i64] → [i64]` | P |
| `0x7C`–`0x7E` | `i64.add` `sub` `mul` | `wasm.i64.<name>` — **`wasm.i64.add` is `0xF1 0x7C`** | — | `[i64 i64] → [i64]` | P |
| `0x7F`–`0x82` | `i64.div_s` `div_u` `rem_s` `rem_u` | `wasm.i64.<name>` | — | `[i64 i64] → [i64]` | P, traps as the `i32` rows |
| `0x83`–`0x8A` | `i64.and` … `i64.rotr` | `wasm.i64.<name>` | — | `[i64 i64] → [i64]` | P |
| `0x8B`–`0x91` | `f32.abs` `neg` `ceil` `floor` `trunc` `nearest` `sqrt` | `wasm.f32.<name>` | — | `[f32] → [f32]` | P, NaN canonicalised (the family's flag) |
| `0x92`–`0x98` | `f32.add` `sub` `mul` `div` `min` `max` `copysign` | `wasm.f32.<name>` | — | `[f32 f32] → [f32]` | P, NaN canonicalised |
| `0x99`–`0x9F` | `f64.abs` … `f64.sqrt` | `wasm.f64.<name>` | — | `[f64] → [f64]` | P, NaN canonicalised |
| `0xA0`–`0xA6` | `f64.add` … `f64.copysign` | `wasm.f64.<name>` | — | `[f64 f64] → [f64]` | P, NaN canonicalised |
| `0xA7` | `i32.wrap_i64` | `wasm.i32.wrap_i64` | — | `[i64] → [i32]` | P |
| `0xA8`–`0xAB` | `i32.trunc_f32_s` `trunc_f32_u` `trunc_f64_s` `trunc_f64_u` | `wasm.i32.<name>` | — | `[f] → [i32]` | P, trap *invalid conversion* |
| `0xAC`–`0xAD` | `i64.extend_i32_s` `extend_i32_u` | `wasm.i64.<name>` | — | `[i32] → [i64]` | P |
| `0xAE`–`0xB1` | `i64.trunc_f32_s` … `trunc_f64_u` | `wasm.i64.<name>` | — | `[f] → [i64]` | P, trap *invalid conversion* |
| `0xB2`–`0xB5` | `f32.convert_i32_s` `convert_i32_u` `convert_i64_s` `convert_i64_u` | `wasm.f32.<name>` | — | `[i] → [f32]` | P |
| `0xB6` | `f32.demote_f64` | `wasm.f32.demote_f64` | — | `[f64] → [f32]` | P, NaN canonicalised |
| `0xB7`–`0xBA` | `f64.convert_i32_s` … `convert_i64_u` | `wasm.f64.<name>` | — | `[i] → [f64]` | P |
| `0xBB` | `f64.promote_f32` | `wasm.f64.promote_f32` | — | `[f32] → [f64]` | P, NaN canonicalised |
| `0xBC`–`0xBF` | `i32.reinterpret_f32` `i64.reinterpret_f64` `f32.reinterpret_i32` `f64.reinterpret_i64` | `wasm.<name>` | — | bits unchanged | P |

**What the family owns beside the rows.** The trap codes its `trap` and primitive rows raise
(*unreachable*, *divide by zero*, *integer overflow*, *invalid conversion*, *out of bounds*, *null
indirect call*, *indirect call signature mismatch*, *out-of-bounds table index*), each mapped to its
`WasmTrapKind` payload; the two regions `memory0` and `globals` with their sizes and growth; the
proportional charges of `memory.grow`, segment initialisation and the `br_table` label vector; and the
deterministic-profile NaN rule. Prefixed instructions, reference instructions and sign-extension
operators are outside the table exactly as they are outside `WasmOpcode`, and the validator refuses
them as not admitted before the translator sees them.

## Appendix D — The primitive table

The closed set of operations an emitter implements itself. Every entry is a total function over word
slots with a stated result for every input, or a partial one whose every undefined input is a named
trap. Names are the operation's, not any language's; a family row names an entry and the differential
check E2 holds the family's handler to it. Adding an entry is a universal bytecode format version.

| Group | Entries | Notes |
|---|---|---|
| **Integer arithmetic**, `i32` and `i64` | `add` `sub` `mul` `and` `or` `xor` `shl` `shr_s` `shr_u` `rotl` `rotr` `clz` `ctz` `popcnt` `eqz` `eq` `ne` `lt_s` `lt_u` `gt_s` `gt_u` `le_s` `le_u` `ge_s` `ge_u` | two's complement, shift counts masked to the width, comparisons answer an `i32` zero or one |
| **Trapping integer**, `i32` and `i64` | `div_s` `div_u` `rem_s` `rem_u` | trap *divide by zero*; `div_s` traps *integer overflow* on the one case; `rem_s` of that case is zero |
| **Floating point**, `f32` and `f64` | `add` `sub` `mul` `div` `min` `max` `abs` `neg` `sqrt` `ceil` `floor` `trunc` `nearest` `copysign` `eq` `ne` `lt` `gt` `le` `ge` | IEEE-754, round to nearest ties to even; `min`/`max` propagate NaN and order the zeros; `nearest` is ties to even; comparisons answer an `i32`; **NaN canonicalisation of results is a per-family flag the emitter honours** |
| **Conversions** | `i32.wrap_i64`; `i64.extend_i32_s` `extend_i32_u`; `f32.convert_i32_s` `convert_i32_u` `convert_i64_s` `convert_i64_u` and the four `f64.convert_*`; `f32.demote_f64`; `f64.promote_f32`; the four `reinterpret` entries | total |
| **Trapping conversions** | `i32.trunc_f32_s` `trunc_f32_u` `trunc_f64_s` `trunc_f64_u`; the four `i64.trunc_*` | trap *invalid conversion* on NaN and out of range |
| **Region access** | `region.load.{i32,i64,f32,f64}`, `region.load.i32.{8s,8u,16s,16u}`, `region.load.i64.{8s,8u,16s,16u,32s,32u}`, `region.store.{i32,i64,f32,f64}`, `region.store.i32.{8,16}`, `region.store.i64.{8,16,32}`, `region.size` | over a region the family declares by name with a base that does not move for the instance's life and a length the emitter reads at the access; the effective address is `i32 address + u32 static offset` computed without overflow; trap *out of bounds* when `address + width` exceeds the length; unaligned access is admitted and the alignment operand is a hint |
| **Word** | `word.keep` | the operand unchanged; a row whose meaning is the identity under a manifest, so that a family need not declare a `Dynamic` handler for a no-op |

**What is deliberately not an entry.** No remainder, exponent or transcendental over floating point
(no machine has the instruction and a helper is a family's), no `ToInt32`-style modular conversion
(it is a language's rule and the JavaScript family's `Dynamic` rows carry it), no string, no object,
no allocation, no call. A family that needs an operation not here has a `Dynamic` row, which is the
correct answer rather than a gap.

## Appendix E — The container, field by field

All integers canonical variable-length through `Broiler.VM.Binary`'s bounded readers unless marked
*fixed*. Every section is `kind`, `length`, `body`; kinds strictly ascending and unique; unknown kinds
refused (no custom sections: a family that wants one has FamilyData).

| Section | Fields |
|---|---|
| **Header** | magic `"BUBC"` *(fixed, four bytes)*; universal format version; language profile identity *(length-prefixed UTF-8)*; feature manifest identity *(length-prefixed UTF-8)*; form identity *(length-prefixed UTF-8, `bytecode` or an emitter's form identity)*; translator identity *(length-prefixed UTF-8)*; translator semantic version; section count |
| **Families** *(kind 1)* | count; per row: slot *(1–15)*; family identity *(length-prefixed UTF-8)*; family table version; selecting manifest identity *(length-prefixed UTF-8)* |
| **Types** *(kind 2)* | count; per row: parameter count, parameter slot types *(one byte each: `1` i32, `2` i64, `3` f32, `4` f64, `5` v)*; result count, result slot types |
| **Units** *(kind 3)* | count; per row: type index; family slot *(0 for a unit with no family rows)*; local count; locals as runs *(count, slot type)*; maximum word height; maximum value height; code offset; code length; flags *(bit 0 `Suspendable`, bit 1 `Entry`, bits 8–15 family-defined)*; landing count and landing offsets *(resume points, for the emitters' compare trees)* |
| **Code** *(kind 4)* | the bytes, tiled by the Units rows in order with no gap |
| **JumpTables** *(kind 5)* | count; per row: unit index; target count; targets *(absolute, last is the default)* |
| **Regions** *(kind 6)* | count; per row: unit index; start; end *(exclusive)*; handler offset; word entry height; value entry height; kind *(family-defined byte)*; regions of one unit in nesting order, inner before outer, non-overlapping unless nested |
| **Entries** *(kind 7)* | count; per row: name *(length-prefixed UTF-8, exactly the core's entry-point bytes)*; unit index |
| **Positions** *(kind 8)* | count; per row: unit index; offset; the core's position record's fields as the family populates them |
| **FamilyData** *(kinds 9 to 23, one per family slot 1 to 15)* | opaque body, framed only; read by the family's hook and runtime |
| **Emission** *(kind 24, at most one)* | form identity *(length-prefixed UTF-8)*; emitter semantic version; alignment; family handler-table shape hash *(fixed, eight bytes)*; code length *(fixed, four bytes)*; the emitted bytes; symbol count; per symbol: unit index, offset *(offsets ascending, one per unit, aligned, inside the bytes)* |

**Ceilings.** Every count is checked against the core's `DeclaredCount`, `SectionCount`,
`StructuralDepth` and `ArtifactBytes` ceilings through the family's declared limits, before the
allocation it would justify, in the order the read-order recorder's canonical form asks (policy handed
over, first payload byte, first allocation).

## Appendix F — Every rule and record this concept touches

**Rules of the architecture register.** *Unchanged* means the rule's statement and witness hold on
the new graph as they are; *revised* means the statement moves and the witness is re-watched; *new*
means a rule this concept mints, each with a negative control watched failing and passing after revert.

| Rule | Today | Under this concept |
|---|---|---|
| A6, C1, C2, C3 | exactly three packable | unchanged during the MVP; `Broiler.VM.Ubc`'s packability is UBC-9's dated decision |
| A7, A15 | the graph manifest and ADR 0001's budget sentence hold the tree | revised at every stage that adds a project or an edge |
| A11, A12, N2 | families keyed on the segment; roots reference core plus profile assemblies | unchanged: four families, no cross-family edge, roots as today |
| B1, B2 | the core references nothing new | unchanged, and the check that the concept keeps the core untouched |
| B4 | no exported member names a type outside `System.*` and `Broiler.VM` | unchanged (the universal bytecode's namespace is under `Broiler.VM`) |
| B5, B5b | no dynamic loading, IL emit or native-code preparation outside the arming path | unchanged in scope; the member allowlist names the machine-code family |
| B5c, X1 | statements name `Broiler.VM.Profile.JavaScript` and `JsNativePage`; tests read the MachineCode assembly | revised to say what the tests already read; the arming path is the machine-code family's execution half |
| X2 | `JsBaselineFrame` holds no reference | revised: every frame type the machine-code pivot declares holds none |
| X3 | `[UnmanagedCallersOnly]` only in two named JavaScript files | revised: only in the machine-code family's wrapper file |
| X4 | baseline handlers route by the block partition and keep run-time checks | revised: every family's wrappers keep the cookie, pc and opcode checks |
| K1–K5 | the composition register held to the tree | unchanged in meaning; the sibling and native-execution cells move with section 9 |
| M1 | the core's public API baseline in both directions | unchanged; `IVmNativeCompiler` stays (route UBC-R5) |
| N1 | the JavaScript profile references exactly Abstractions, Binary and its format | revised: plus `Broiler.VM.Ubc` |
| N3 | the JavaScript format declares no reference | revised: exactly `Broiler.VM.Ubc` |
| N5–N8 | the JavaScript diagnostic registry in both directions | unchanged in rule; the registry's rows move as the re-based corpus says |
| N9 | positions constructed in one file | unchanged |
| N10, W2 | the families' public surfaces in their baselines | unchanged in rule; the baselines move |
| N12, N19, N20, N21 | no mutable statics in the lowering; the compilation stack; no ambient host holder; the crossing charge | unchanged |
| N16 | two JavaScript format versions under one verifier, bound to manifests | retired with its reason: the family has no format version of its own; the pairing principle survives one level up as a rule over family table versions (new, **U5**) |
| W1 | the WebAssembly profile references exactly Abstractions and Binary | revised: plus `Broiler.VM.Ubc` |
| **U1** *(new)* | | `Broiler.VM.Ubc` references exactly Abstractions and Binary |
| **U2** *(new)* | | `Broiler.VM.Ubc` exports no identifier of a banned vocabulary and no family row |
| **U3** *(new)* | | no emitter family's source names a language family, a language or an opcode of one |
| **U4** *(new)* | | the common family's stack-effect table is one table the verifier, the interpreter and every encoder read |
| **U5** *(new)* | | a family table version is bound to the manifest that selects it, and one walk reads every table |
| **U6** *(new)* | | every `Primitive` row of every composed family has a retained input corpus and a passing differential check per emitter |
| **U7** *(new)* | | the thread-static activation slot is written only by the machine-code family's managed entry and read only after the cookie check |
| E2, E4 | the ADR index and the contract-bearing set | unchanged: ADR 0013 is not contract-bearing and the index gains its row |
| H1–H5, J1–J12 | review documents and assurance | unchanged; every moved unit is re-annotated and re-fingerprinted by the generator |

**Records.**

| Record | What moves | Filed by |
|---|---|---|
| `docs/roadmap.md` section 1 non-goals | "one universal opcode set, tagged value, or frame ABI shared across languages" — revised to refuse a shared value and frame ABI and to admit a shared *encoding* whose meanings are owned, with the superseded text quoted, through ADR 0003 section 11 | UBC-0 |
| `docs/roadmap.md` section 8, the candidate table row and the extraction register | the row's verdict re-dated; a new row for the universal bytecode with G1–G4 answered | UBC-0 |
| `docs/roadmap.md` section 10 | "there is no generic compiler host" stands; "a backend is a choice inside one lowering" gains the sentence that the choice may leave the lowering for an emitter family, dated | UBC-0 |
| `docs/roadmap.md` section 16 | the first risk row's mitigation names the universal bytecode's U-rules | UBC-0 |
| ADR 0001 | dated revisions per project and edge; the packability decision | UBC-1, UBC-2, UBC-5, UBC-6, UBC-7, UBC-9 |
| ADR 0011 | the standing-refusals table gains a dated verdict row; P1's route | UBC-0 |
| **ADR 0013** *(new, not contract-bearing)* | the extraction record | UBC-0 |
| `docs/mvp.md` section 5 | rows UBC-R1 to UBC-R7 as `MVP-10` onward; MVP-3, MVP-7 and MVP-8 annotated as carried over; MVP-2's vector reservation annotated as a universal format question | UBC-0 and after |
| `docs/support.md` section 3a | becomes the emitter table | UBC-9 |
| `docs/compositions.md` | sibling and native-execution cells; the missing MachineCode extraction explanation | UBC-0, UBC-5, UBC-8 |
| JavaScript `roadmap.md` non-goals, section 9 | the second-execution-arm entry (the executor is the emitter's), the second-lowering entry (unchanged in substance), the intermediate-form sentence discharged | UBC-0, as JSD-0036 |
| JavaScript `roadmap.backends.md` | the whole document describes forms this concept re-homes; a dated note at its head, and its stages' State bullets closed or superseded by name | UBC-3, UBC-6 |
| JSD-0011, JSD-0025, JSD-0035 | not amended; each cited for the property this concept carries over | — |
| WebAssembly `roadmap.md` sections 5, 7, 17, the compiler and second-execution-arm non-goals | `WAC-nn` corrections with the superseded text; section 9's memory row taken | UBC-4 |
| `HUMAN_REVIEW.md`, `CODE-ASSURANCE.md`, `assurance.manifest.json` | regenerated at every stage; every moved unit `HUMAN_PENDING` | every stage |
