# ADR 0013 - The Universal Bytecode Extraction Record

**Status:** Proposed

**Date:** 2026-09-25

**Core contract:** not contract-bearing

## Context

[ADR 0011](0011-source-level-profile-contract.md) opens a shared component between profiles only
through its extraction gate: two or more product profiles already implement the behaviour (G1),
the implementations have been compared and the shared part identified from real merged code (G2),
the shared part is expressible without naming any language concept (G3), and extraction creates no
profile-to-profile dependency (G4). Its record clause says every invocation produces a record in this
ADR set, "because it changes the core graph", naming six things: the two implementations with file
paths and source revisions; the measured duplication, shown as a correspondence; the proposed public
surface, written with no identifier drawn from any language's vocabulary; the resulting graph edges
demonstrating G4; the two named consumers; and the verdict with its date and deciding owner. The
record is filed whether the verdict is accept or refuse.

[The universal bytecode concept](../universal-bytecode.md) proposes one instruction encoding, one
container, one verifier walk and one dispatch loop for every language profile, with every language
instruction's meaning owned by the profile that declares it, and it proposes emitter profiles that
turn that bytecode into something a process executes. [The programme roadmap](../universal-bytecode.roadmap.md)
makes filing this record its first milestone, UBC-0.

**This record is that invocation.** It was invoked on 2026-09-25 by the core architecture owner. The
roadmap's work package UBC-0.1 has each profile supply its half - file paths, a revision and
correspondence rows - and record no verdict; **here both halves were compiled by this record's author
from the checkout at the revision named below**, and neither profile recorded a supply of its own.
The core architecture owner, both profile owners, and every role ADR 0012 names are one person
(Exclusion EX-30 of ADR 0012), and nothing below claims that a second reader has checked any of it.

**It is not contract-bearing.** It names no lifecycle transition, no result category and no public type
of core contract version 1, and it adds nothing to the core's three assemblies: the assembly it admits,
`Broiler.VM.Ubc`, references the core and is referenced by nothing in it. It changes no graph on its
date either, because no project it admits exists yet; each project arrives with its own dated revision
of ADR 0001, in the milestone that writes it.

**The concept proposes two shared assemblies, and the gate can be invoked for one of them.**
**Candidate A** is the universal bytecode itself: the container, the common family, the family
instruction-table schema, the primitive table, the verifier walk, the family contracts and the
descriptor factory (the proposed `Broiler.VM.Ubc`), together with the one dispatch loop that executes
it (the proposed `Broiler.VM.Emitter.Bytecode`). **Candidate B** is the native-form mechanism the
concept places beside it: the emitted frame and activation contracts, the handler-table wrappers, the
template schema, the template-closure scan and the form-verifier contract (the proposed
`Broiler.VM.Ubc.Native`). Candidate A receives the verdict below. For candidate B, G1 is unsatisfied,
so under ADR 0011 the gate cannot be invoked for it at all, and this record carries instead the dated
note that record prescribes for that state, which carries no verdict.

## The two implementations

Both are read from the checkout at revision `51e0d60ee43ce34e8d21b1d9b10d086cdb723501` (`main`, a clean
working tree), which is the revision every file, member and correspondence row below is taken from.

| Implementation | Assemblies | Last commit touching the tree at that revision |
|---|---|---|
| The JavaScript profile | `src/Broiler.VM.Profile.JavaScript` (the verifier, the interpreter, the runtime), `src/Broiler.VM.Profile.JavaScript.Format` (the opcodes, the container schema, the native template tables and the scan) | `0473fc3` (2026-09-25) |
| The WebAssembly profile | `src/Broiler.VM.Profile.WebAssembly` (the decoder, the validator, the interpreter, the store) | `d16a6aa` (2026-09-19) |

Both are product profiles in ADR 0011's sense: neither is the fixture profile or an application-local
consumer profile, and both are merged code at the revision named. The bundle of this milestone,
[`docs/evidence/ubc-0-001/`](../evidence/ubc-0-001/README.md), names every source file this record
cites by its git blob identifier at that revision.

## The correspondence table

Each row names one mechanism, the members that implement it on each side, and the part of it both
sides do and that names no language. The member lists are the entry points; the section after the
table says what each side does, and the section after that lists what only one side does. **The
shared-part column is scanned for the banned vocabulary** of
[`docs/ubc/banned-vocabulary.txt`](../ubc/banned-vocabulary.txt) by `eng/ubc-vocabulary-scan.py`, the
list rule U2 will read; the run is retained in the bundle.

| Row | Mechanism | JavaScript implementation | WebAssembly implementation | Shared part |
|---|---|---|---|---|
| a | Artifact framing and the section walk | `JavaScriptVerifier.Verify` (`JavaScriptVerifier.cs`); `JsVerifier.Verify`, `ReadManifest`, `ReadSection` and one reader per section kind, then `Link` (`JsVerifier.cs`); `JavaScriptReadAdapter` (`JavaScriptDiagnostics.cs`); `JsVerifier.FromReader` | `WebAssemblyVerifier.Verify`, `VerifyCore` (`WebAssemblyVerifier.cs`); `WasmDecoder.TryDecode`, `TryReadPreamble`, `TryReadOneSection`, `TryAdmitSectionPosition`, `TryDecodeSectionBody`, `TryCheckSectionAgreement` (`WasmDecoder.cs`); `WasmReadAdapter` (`WasmReadAdapter.cs`); `WasmRefusal.FromReader` (`WebAssemblyDiagnostics.cs`) | A container is a magic tag, a format version and a sequence of framed sections. The verification ceilings are projected into one set of read bounds before the first byte is read, and one bounded reader with one meter adapter reads the whole payload. The descriptor's format version and feature manifest are ruled on before a section is read. Each section is a kind and a byte length, admitted by kind, canonical order and uniqueness, entered (charging section count and nesting depth), read by the grammar of its kind and exited with a check that exactly the declared length was consumed. An undefined kind, an order or repetition fault and a length mismatch are refusals. Every count is held to the declared-count ceiling before a buffer is sized from it, and agreement between sections is checked after the last. Every refusal is either an invalid-artifact outcome with a core reason, a registered diagnostic code and a position, or a resource exhaustion naming one budget dimension at artifact scope, and one mapping from the reader's latched status chooses between them. |
| b | The code walk with height and target checks | `JsVerifier.Link` and `JsVerifier.Walker.Walk`, `Seed`, `Check`, `Operand` over `JsOpcodes.IsDefined`, `InstructionWidth`, `Shape`, `TryDescribe`, `HasCodeTarget`, `IsTerminal` (`JsVerifier.cs`, `JsOpcode.cs`); `JavaScriptVerifier.Walk` for format version 1 | `WasmValidator.TryValidateModule`, `TryValidateBody`, `TryStep` and the control, operand and immediate helpers (`WasmValidator.cs`); `WasmFunctionBody.TrySeal` and the sealed `WasmJumpTarget` table (`WasmModule.cs`) | For each code unit, one walk over its instruction bytes computes the abstract operand state at every reachable instruction from a per-opcode effect: fixed pops and pushes, or a count taken from the operand. Every successor (fall-through, taken edge, and every extra entry the unit declares) receives an expected state, and a second arrival with a different state is a join disagreement. The walk refuses an undefined opcode, an instruction cut short by the unit's end, underflow, a height above a ceiling, a transfer target outside the unit, a path that runs past the unit's end and an index outside the table it names, each with the instruction's offset. It charges verifier work and polls on the declared cadence. The operand-height high-water mark it computes is stored once, replacing any declared figure, and the executor sizes each activation's operand region from it; the executor takes a transfer only to a destination the walk checked. |
| c | The dispatch loop | `JsEngine.ExecuteCore<TMode>` with `Execute`, `U16`, `U32`, `Charge` (`JsEngine.cs`); `JavaScriptExecutor.Run` for format version 1 (`JavaScriptExecutor.cs`) | `WasmInterpreter.Run` with `Call`, `Branch`, `MemoryAccess`, `TryNumeric` (`WasmInterpreter.cs`) | A verified unit is executed by a loop over its code bytes with an explicit program counter. At each instruction the loop records the offset for fault attribution, charges the instruction's fixed cost before its effect, reads the opcode byte, dispatches once through one switch, and lets the arm read its immediates and advance the counter or replace it with a target verification checked. An opcode verification admitted and the loop has no arm for is an internal defect of the executor, never a guest-visible fault. A call hands control to a new activation, and a return resumes the caller at the counter its activation saved. |
| d | The frame model | `ExecuteCore<TMode>`'s per-activation operand array and scope list; `JsEngine.Call`, `EnterCall`, `EnterDepth`, `LeaveCall`, `Invoke`; `JsFrame` for suspendable units (`JsEngine.cs`, `JsGenerator.cs`) | `WasmFrame`, the shared operand stack, `WasmInterpreter.TryPushFrame`, `PopFrame`, `EnsureStack` (`WasmInterpreter.cs`) | An activation is identified by its code unit and holds a program counter, an operand region sized from the verified high-water mark and the unit's local storage. Entry moves the arguments from the caller into the callee's locals. Every activation pushed charges one unit of call depth through the core meter and releases it on every exit path, a fault or a budget stop included. A normal exit leaves exactly the unit's results where the caller expects them. |
| e | Unwinding | the `Throw` arm; the filtered catch in `ExecuteCore` with `TryFindHandler`, `TryFindFinally`, `Land`; `JsThrow`, `JsAbort`; the executor-boundary mapping in `JsExecution` (`JsEngine.cs`, `JsThrow.cs`, `JsExecution.cs`) | `WasmInterpreter.Trapped` and the `Call` cleanup; `WasmTrapKind`, `WasmRunStatus`; `WebAssemblyExecutor.InvokeCore`, `Refused` (`WasmInterpreter.cs`, `WasmTrapKind.cs`, `WebAssemblyExecutor.cs`) | Three outcomes leave the loop and are never confused: a guest fault names a kind and a position and becomes a faulted step carrying a registered code; a budget refusal or a cancellation becomes a contract-violation step the core rewrites from its meter's latches, which no guest-level handler can intercept; an executor defect becomes a contract-violation step naming a profile contract violation. An activation a fault leaves without a handler is abandoned and its call depth released, and the fault continues outward to the step's end. |
| f | The charge and poll sites | `JsEngine.Charge`, `ChargeOnce`, `EnterDepth`, `ChargeHostCrossing` and the proportional charges inside arms (`JsEngine.cs`); the declared bounds in `JavaScriptProfile` (`JavaScriptProfile.cs`) | `WasmPacing.TryReserve`, `TryCharge`, `Observe`; `WasmMemoryInstance.Grow` (`WasmInterpreter.cs`, `WasmMemory.cs`); `WebAssemblyProfile.MaxUnchargedWork` (`WebAssemblyProfile.cs`) | Fuel is charged through the core meter at a fixed cost per instruction, before the instruction's effect. Work done between polls never exceeds the declared uncharged-work bound: a poll is taken before any charge that would cross it, either by splitting the charge or by reserving the worst case first. Work proportional to a size the guest controls is charged by the instruction that performs it, and any charge made elsewhere is fed back into the poll accounting. A refused charge ends the step as exhaustion and a refused poll as cancellation, and the core's latches decide which the caller is told. |
| g | Constant and immediate decoding | `JsVerifier.ReadConstants`, `ReadBigInt`, `Walker.Operand`, `Walker.Check`; `JsOpcodes.Shape`, `OperandWidth`; `JsEngine.U16`, `U32` (`JsVerifier.cs`, `JsOpcode.cs`, `JsEngine.cs`) | `WasmLeb128` readers (`WasmLeb128.cs`); `WasmValidator.TryReadVarU32` and its siblings (`WasmValidator.cs`); `WasmInterpreter.ReadU32`, `ReadS64`, `ReadFixed`, `SkipVarInt` (`WasmInterpreter.cs`); `WasmDecoder.TryReadConstantExpression` (`WasmDecoder.cs`) | Every immediate an instruction carries sits in the code after the opcode byte. It is decoded and range-checked once at verification against the table it indexes, or the ceiling that bounds it, and the executor reads the same bytes again without re-checking them. A literal verification decoded is taken by index or re-read, never re-validated. A malformed immediate encoding is an invalid artifact with a malformed-encoding reason and a code of its own. A count carried in the code is held to the declared-count ceiling before it sizes anything or bounds a loop. |
| h | The native-form machinery | `JsNativeActivation.Step`, `StepValue`, `PrepareCall`, `FinishCall` (`JsNativeActivation.cs`); `JsBaselineHandlers.Table` and its wrappers (`JsBaselineHandlers.cs`); `JsEngine.RunNative`, `RunValue` (`JsEngine.Baseline.cs`); `JsNativeScan.Scan` (`JsNativeScan.cs`); `JsNativeTemplates.For` (`JsNativeTemplates.cs`); `JsBaselineBlocks.TryPlan` (`JsBaselineBlocks.cs`); `JsBaselineFrame`, `JsBaselineAbi` (`JsBaselineFrame.cs`); `JsNativeFrame` (`JsNativeFrame.cs`); the encoders `JsX64Backend`, `JsX64BaselineEmitter`, `JsX64ValueEmitter`, `JsX64Assembler`, `JsX64Abi`, `JsArm64Backend`, `JsArm64Assembler`, `JsArm64Walk`, `JsNativeBackend`, `JsNativeCompiler` (the files of those names under `src/Broiler.VM.Profile.JavaScript.Compiler`) | none: the profile has no native form, no template table, no handler table, no emitter and no activation cookie | none. One implementation exists and there is no second to compare it with. |

### What each side does, row by row

**Row a.** The JavaScript side builds a read adapter over the verification meter and projects the
ceilings. Its format-version-2 pass (`JsVerifier`) reads a four-byte magic and a canonical
variable-length format version and compares the version with the descriptor's first; the
format-version-1 pass checks the payload's version against its own range before the descriptor. Format
version 2 then reads a manifest identity held to the descriptor's and a declared section count, and walks
that many sections with a poll before each, the kind and the length canonical, kinds strictly ascending,
every exit failure a section-length mismatch; trailing bytes and missing required sections are refused,
and `Link` checks the cross-section rules. Buffers are allocated after a count has passed the
declared-count bound and a format ceiling, and are not reserved against the allocated-bytes allowance.
The WebAssembly side rules on the descriptor alone before reading any byte - its payload carries no
manifest to compare - checks a four-byte magic and a fixed-width binary version, and reads sections until
the payload ends rather than to a declared count, so it has no trailing-bytes and no missing-section
refusal; it admits each section by order rank and duplication before entering it, reads lengths with a
reader that accepts padded encodings (the wire format requires it), skips custom sections, reserves every
buffer through the bounded allocator, and checks that the function and code sections agree after the
last section.

**Row b.** The JavaScript side walks each unit with a worklist of offsets, one height and one scope
depth per offset, seeding the entry and every region handler of the unit, charging one work unit per
worklist pop, reading every effect from `JsOpcodes.TryDescribe`, checking the taken edge's height for
every opcode with a code target, and requiring a `Return` at height exactly one. A target is checked
against the unit's range, which format version 1 strengthens with a boundary set; the walk stores only the
computed height, and the interpreter branches to the target its operand bytes carry. The WebAssembly side
walks each body once, in the specification's algorithm, over a typed operand stack and a control stack,
charging structural depth per nesting level, and seals each body with its operand bound, its label bound
and a table of every block, loop and if boundary that the interpreter later reads instead of searching.

**Row c.** The JavaScript loop is one generic method whose mode parameter decides only where it stops; a
guest call recurses through the engine's `Call` into a nested run of the loop, so guest depth is CLR
depth on a guest thread with a declared stack, and faults travel as CLR exceptions. The WebAssembly
loop is one CLR frame for every guest depth over a heap array of frames; a call pushes a frame and
returns to the outer loop, faults are return codes, and immediates are decoded again on every
execution.

**Row d.** On the JavaScript bytecode path there is no heap frame list: each activation allocates its
operand array and scope list, and a heap frame exists only for a suspendable unit, where it carries the
operand array, the scopes and the counter across a suspension. `EnterDepth` probes the CLR stack,
compares a language-level depth bound, and charges call depth; `LeaveCall` releases it. The WebAssembly
side reuses heap frames, moves parameters off one shared operand stack into the frame's locals, sizes
the operand region from the verified bound, and charges and releases call depth per frame.

**Row e.** A JavaScript guest throw is a CLR exception carrying a value, caught by an exception filter
whose handler search is a pure linear scan of the program's regions for the innermost covering row;
landing truncates scopes and operands to the region's declared depth and height and pushes the value,
and a forced return searches finally regions only. Budget, cancellation and defect aborts are a
different exception type the filter never catches. WebAssembly has no region: a trap returns a status
out of the loop with its kind and position, every activation is abandoned at once, and the executor
maps each status to a step.

**Row f.** The JavaScript loop charges a fixed fuel cost at the top of every instruction and splits any
larger charge into fixed windows (`JsEngine.PollWindow`) well inside the declared poll bound, polling once
a window is spent. The WebAssembly loop reserves poll headroom before each charge and polls only when the
next charge would cross the bound, and memory growth reserves the worst case first, charges pages and
allocation, and feeds the charged amount back into the poll accounting.

**Row g.** JavaScript literals live in a constant pool decoded once at verification; operands are fixed
width, the width a function of the opcode alone, decoded once by the walk and re-read by each arm.
WebAssembly has no pool: integer literals and indices are variable-length immediates, validated once with
a reader that accepts padding within the width's byte budget, while floating-point literals are
fixed-width; all of them are decoded again at every execution by readers that never refuse.

**Row h.** Stated as one implementation, as the roadmap's work package UBC-0.1 asks. The JavaScript
profile has three native forms over its bytecode (the numeric form, the baseline form of
[JSD-0025](../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0025-the-baseline-native-form-over-the-wide-manifest.md)
and the refused value form of JSD-0035 kept opt-in): a thread-static activation with a cookie, a
handler table of `[UnmanagedCallersOnly]` wrappers in unmanaged memory, per-architecture template
tables, and a scan that decodes emitted bytes against them. The WebAssembly profile has nothing of the
kind.

### What only one side does

These are recorded because a correspondence that hid them would be asserting rather than showing. None
of them is in a shared-part cell; each is a decision the universal bytecode takes rather than a behaviour
it extracts:

- a descriptor ruled on against the payload's own identities (JavaScript, format version 2) against a
  descriptor ruled on alone (WebAssembly, whose payload carries no manifest), and a payload version
  checked against the format's range before the descriptor (JavaScript, format version 1);
- a declared section count, with a trailing-bytes and a missing-section refusal (JavaScript), against
  reading to the end of the payload with neither (WebAssembly);
- canonical variable-length integers against padded ones, and a variable-length against a fixed-width
  format version;
- ignorable custom sections, which only the WebAssembly side has;
- a reservation against the allocated-bytes allowance before every buffer, which only the WebAssembly
  side makes;
- resolved transfer destinations stored once and read by the executor instead of the payload's own
  bytes, which only the WebAssembly side does (its sealed table); the JavaScript executor branches to the
  checked target its operand carries;
- a transfer target checked against the unit's range (JavaScript, format version 2) against a boundary
  set (format version 1) and a sealed table (WebAssembly);
- a typed operand stack (WebAssembly) against an untyped height (JavaScript);
- handler regions with a pure innermost-first search and a landing, and suspendable activations kept
  in heap objects across a suspension, which only the JavaScript side has;
- guest depth as CLR recursion (JavaScript) against one loop over heap frames (WebAssembly);
- faults as exceptions (JavaScript) against status codes (WebAssembly);
- every `TryExitSection` failure mapped to one code (JavaScript) against only one reader status mapped
  so (WebAssembly);
- a refused verifier poll answered as verifier-work exhaustion in `JsVerifier`'s section loop, as
  wall-clock exhaustion in `JavaScriptVerifier`'s, and as an invalid artifact inside the format-version-2
  section readers `ReadConstants`, `ReadModules` and `ReadEvalScopes`;
- a verifier with no exception wrapper (JavaScript) against one that turns any escape into a named
  defect code (WebAssembly);
- instruction widths written into each interpreter arm (JavaScript) and no descriptor table at all
  (WebAssembly), which is what a one-table rule over the common family has to replace;
- the WebAssembly interpreter's numeric dispatch sends the float comparisons to its integer arm, which
  has no case for them, while the correct float-comparison code has no caller (the concept's section
  2.2; `docs/tasks/fix-webassembly-float-comparisons.md`);
- the WebAssembly `br_table` reads its whole label vector under one fuel charge.

**And sentences of the concept the code does not bear out.** The concept's section 5.9 quotes a roadmap
risk row, "`CallDepth` is declared by every profile and charged by no code in this repository"; at the
revision named both profiles charge it (`JsEngine.EnterDepth`, `WasmInterpreter.TryPushFrame`). Its
section 5.9 also says that in the interpreter a frame is a heap object and the loop one CLR frame
regardless of guest depth: that is true of the WebAssembly side today and not of the JavaScript
bytecode path. Neither changes the verdict below; both are carried to the concept as dated notes.

## The proposed public surface

Every name is in the `Broiler.VM.Ubc` namespace of an assembly of that name, and none is drawn from any
language's vocabulary: the same scan that reads the table's last column is run over this list and
retained. The list is a proposal, as the record clause asks; the assembly's own baseline,
`docs/ubc/api/public-api.txt`, becomes the authority on the surface in the milestone that writes it
(UBC-1), and rule U9 holds the assembly to it in both directions.

- **The container:** `UbcFormat`, `UbcSectionKind`, `UbcUnitFlags`, `UbcArtifact`, `UbcHeader`,
  `UbcFamilyEntry`, `UbcSignature`, `UbcLocalRun`, `UbcUnit`, `UbcJumpTable`, `UbcRegion`, `UbcEntry`,
  `UbcPosition`, `UbcFamilyData`, `UbcEmission`, `UbcSymbol`, `UbcArtifactReader`, `UbcArtifactWriter`,
  `UbcSectionEncoder`, `UbcCodeBuilder`, `UbcRefusal`.
- **Slots, shapes and descriptors:** `UbcSlotType`, `UbcSlotTypes`, `UbcPlane`, `UbcOperandShape`,
  `UbcOperandShapes`, `UbcEffect`, `UbcEffectForm`, `UbcTarget`, `UbcTrapCode`.
- **The common family:** `UbcOpcode`, `UbcOpcodes`, `UbcCommonRow`, `UbcCommonEffect`.
- **The family table schema and the primitive table:** `UbcInstructionRow`, `UbcInstructionTable`,
  `UbcInstructionKind`, `UbcRegionKindRow`, `UbcRegionDeclaration`, `UbcFamilyTrap`, `UbcTrapMapping`,
  `UbcPrimitive`, `UbcPrimitives`, `UbcPrimitiveResult`.
- **The contracts a family implements:** `IUbcFamily`, `IUbcFamilyVerifier`, `IUbcValuePlane`,
  `UbcActivation`, `UbcStatus`, `UbcStatusKind`, `UbcCallRequest`, `UbcHookAnswer`,
  `UbcHookInstruction`, the frame codec members of `IUbcFamily`.
- **Registration, forms and the descriptor factory:** `UbcContract`, `UbcFamilyRegistration`,
  `UbcFamilyDeclaration`, `IUbcExecutorFactory`, `UbcForm`, `UbcEmitterSet`, `UbcDescriptors`,
  `UbcCompositionException`.
- **The walk:** `UbcVerifier`, `UbcVerifiedProgram`, `UbcDiagnosticCode`.

**What the list leaves out, and why.** The container keeps a section kind for emitted code
(`UbcEmission`, `UbcSymbol`), because an artifact of a native form has to be recognised and refused by
name in an image that composes only the bytecode form, which is framing and belongs to row a. The
contracts that would admit such an artifact - a form verifier that scans emitted bytes, and the emitting
half of a native emitter - are candidate B's mechanism and are not proposed here: until the gate can be
invoked for candidate B, no composition can admit an artifact that carries an Emission section.

## The resulting graph edges

Each edge below arrives with the project that declares it, in that milestone's dated revision of ADR
0001; none exists on this record's date.

```text
Broiler.VM.Ubc                         -> Broiler.VM.Abstractions, Broiler.VM.Binary
Broiler.VM.Emitter.Bytecode            -> Broiler.VM.Abstractions, Broiler.VM.Binary, Broiler.VM.Ubc
Broiler.VM.Profile.JavaScript.Format   -> Broiler.VM.Ubc                      (at UBC-3)
Broiler.VM.Profile.JavaScript          -> Abstractions, Binary, Broiler.VM.Ubc, the format (at UBC-3)
Broiler.VM.Profile.JavaScript.Compiler -> Abstractions, Broiler.VM.Ubc, the format (at UBC-3)
Broiler.VM.Profile.WebAssembly         -> Abstractions, Binary, Broiler.VM.Ubc (at UBC-4)
```

**G4 holds on this shape.** No edge runs between the JavaScript family and the WebAssembly family in
either direction, so rule N2 stands as it is. No edge runs from `Broiler.VM.Ubc` to any profile, and
none from the bytecode emitter to any profile: the emitter receives a family's tables and handlers as
values at composition, through the contracts in `Broiler.VM.Ubc`. No edge runs from the core's three
assemblies to `Broiler.VM.Ubc`, so rules B1 and B2 stand, and the core's public API baseline does not
move.

## The two named consumers

The JavaScript profile family (`Broiler.VM.Profile.JavaScript`, `.Format`, `.Compiler`) and the
WebAssembly profile (`Broiler.VM.Profile.WebAssembly`). Each will reference `Broiler.VM.Ubc`, lower to
its bytecode, declare one family table per feature manifest, and give up its own verifier walk and its
own dispatch loop. The fixture family the programme writes under `src/tests/` for milestone UBC-2 is not
a consumer for this record's purposes, exactly as ADR 0011 excludes fixture profiles from the count.

## The verdict

**Decided 2026-09-25 by the core architecture owner, on candidate A.** The owner gave the verdict in
advance of the table, in these terms: accept where the correspondence table holds, and refuse any
condition the evidence fails. The table above was then written from the checkout at the revision named,
each condition was tested against it, and this section records the outcome of that test as the owner's
verdict. The owner, the JavaScript profile owner and the WebAssembly profile owner are one person; the
verdict is not independent of the proposal it rules on, and this record says so rather than resolving
it.

| Candidate | G1 | G2 | G3 | G4 | Verdict |
|---|---|---|---|---|---|
| **A** - the universal bytecode and its one loop (`Broiler.VM.Ubc`, `Broiler.VM.Emitter.Bytecode`) | **Met.** Both product profiles implement framing, a code walk with height and target checks, a dispatch loop, activations with a charged call depth, fault exits, per-instruction charges and immediate decoding (rows a to g). | **Met.** The shared part of each row is identified from the merged code of both, at the revision named, and shown as the correspondence above; what only one side does is listed apart and is not claimed as shared. | **Met.** Every shared-part cell and the proposed public surface name no language concept, and the banned-vocabulary scan over both is retained clean in the bundle. | **Met.** The edges above create no profile-to-profile dependency. | **ACCEPT** |

**Where candidate A's G1 is weakest, stated rather than argued away.** Three features the universal
bytecode carries have one implementation each, all on the JavaScript side, and the list above names
them: region landing, the suspension of an activation, and a plane of language values beside the
machine words. The verdict admits them because each is a feature of a behaviour both profiles implement
- a loop over activations that ends a step in one of the core's step kinds - and the WebAssembly side is
the empty case of each: a unit with no regions, a table with no suspending row, a family with no value
plane. The shared mechanism for each is taken from the implementation that has the feature, which is
merged code and not anticipation. **Row h is not of that kind**, and that is the line between the two
candidates: the WebAssembly profile has no emitted code of which "no native form" is the empty case, so
there is no behaviour two product profiles implement.

**The G3 finding does not rule on the primitive table.** It covers the shared-part cells and the names of
the proposed surface, and no row of the table above describes the primitive table's entries, so whether a
table of two's-complement and IEEE-754 operations is mechanism or semantics is not decided here. That
question stays the route MVP-11 records.

## The native-form mechanism: the unsatisfied-G1 note

**Dated 2026-09-25. G1 is unsatisfied for candidate B, and this note carries no verdict.** ADR 0011's
exclusion item 5 describes this state: the gate's failure branch "describes a gate that fired and failed a
condition; invocation is barred until G1 holds", and what is filed instead is "a dated note stating that
G1 is unsatisfied, naming what would satisfy it, and supplying that profile's own half", which "carries no
verdict". This is that note, filed in the record that invoked the gate for candidate A.

- **What is unsatisfied.** One product profile implements a native form over its bytecode (row h); the
  other has no emitted code at all.
- **The half this note supplies.** Row h of the table above: the JavaScript profile's files and members at
  revision `51e0d60`, against nothing on the WebAssembly side.
- **What would satisfy it.** In ADR 0011's words, the second product profile: a second product profile's
  own implementation of emitted code over its bytecode, in merged code.
- **What it is not.** It is not a refusal, and nothing here is refused: under ADR 0011 an unsatisfied G1
  "is not a failure at all". It is also not a verdict the programme's work package UBC-0.6 reads as a
  refusal of a condition, because no condition was ruled on for candidate B.

**What the note means for the programme, from the roadmap's own dependency fields.** Milestone UBC-5
writes `Broiler.VM.Ubc.Native`, so it cannot meet its first clause while G1 stays unsatisfied; UBC-6a and
UBC-7 build on UBC-5; UBC-3 waits on UBC-6a as a whole (it recovers the JavaScript native forms on the
`x86` emitter in the same milestone that retires their substrate, so that no form is absent between two
milestones); UBC-6b waits on UBC-3; UBC-8 waits on UBC-3; UBC-9 completes only after UBC-8 and its
packability decision names `Broiler.VM.Ubc.Native`; and UBC-10 waits on UBC-6b and UBC-7. **Only UBC-1,
UBC-2 and UBC-4 can meet their gates while the note stands.** Any milestone may still start, as the
roadmap's section 2 allows.

**And a circularity the note names rather than hides.** The programme planned the WebAssembly profile's
first native form to arrive through the `x86` emitter at UBC-6b, which is to say through the mechanism
this note says the gate cannot yet admit. Under ADR 0011 as written, the native half can therefore
proceed only if the WebAssembly profile acquires a native form of its own first - the second execution
arm its plan refuses and the concept exists to avoid - or if the owner rules that moving one profile's
native machinery out of the profile into an emitter family is not an extraction between profiles at all.
Neither is taken here. The ruling is the core architecture owner's, and until it is given the ledger
names that owner as the holder.

## The standing refusal this record's verdict reaches

ADR 0011's standing-refusals table has a row "A shared value representation, frame layout, or opcode
set", whose verdict is "Refused permanently; these are the semantics the core exists not to own". **That
row is not a refused extraction record**, so the reopening conditions of ADR 0011's failure clause - "a
language-free formulation is found" among them - are written for records like this one and do not
reach it; and the table's own heading asks that its rows "are not re-litigated". Accepting candidate A
nevertheless admits a shared opcode set as an encoding, which the row's words refuse. **This record does
not claim a clause of ADR 0011 that permits that.** It takes it as a route without a decision, filed as
[MVP-12](../mvp.md#5-routes-taken-without-a-decision), which names the ruling that would settle it and
who would give it. What the route takes, and what it leaves standing:

- **The opcode-set half, taken as an encoding.** What candidate A shares is the encoding (one byte space,
  fixed operand widths, a prefix per family, a table schema every family fills in), a common family whose
  every row's meaning is stated without reference to any language (control transfer, calls, locals, word
  constants, stack shuffles, a trap), and a primitive table of machine operations stated in
  two's-complement and IEEE-754 terms, against which a family's own handler is checked. The meaning of
  every family instruction stays in the family that declares it.
- **A shared value representation stands refused.** A slot of the universal bytecode is a machine word
  or a language value whose representation, lifetime and meaning belong to one family; no family reads
  another's, and no value crosses between families inside one artifact or one unit.
- **A shared frame layout stands refused.** The frame mechanism belongs to the emitter; no family reads a
  frame's layout, and the layout is not shared between languages because no language sees it.

ADR 0011's table is not edited. It gains an editorial pointer paragraph naming this record and the
route, as the programme roadmap's work package UBC-0.3 asks.

## The 2026-09-18 extraction into the MachineCode project, explained

[The composition register](../compositions.md) says in its own words that no decision record explains
the extraction of 2026-09-18 or names the roots it was meant to leave behind. This section supplies the
account; **it does not decide the extraction after the fact**, and it does not say the extraction was
right.

**What happened.** Commit `db57c2f` (2026-09-18) created `src/Broiler.VM.Profile.MachineCode`,
referencing Abstractions and Binary with unsafe code allowed. It moved the two platform halves of the
JavaScript profile's page-arming path into it as `VmNativePage.Unix.cs` and `VmNativePage.Windows.cs`,
deleted `JsNativePage.cs`, added an artifact format of its own (`"BMC\0"`) with a writer, a verifier
that decodes no instruction and an executor, added `IVmNativeCompiler` to the core's Abstractions and
`VmRuntime.CompileToMachineCode` to the Runtime together with a contract test over a stub, added
`JsNativeCompiler` to the JavaScript lowering, and set every native-execution cell of the register to
`none`. Its ADR 0001 revision is two sentences and a graph count, with no rationale. Commit `7bfe561` of
the same day re-created `JsNativePage.cs` as a hook, `JsNativePage.Mapper`, that a composition root
fills with `VmNativePage`, and restored the reference to one root (`SliceCompiler`), whose cell went
back to `x86-64`; the others stayed `none` because nobody put the assembly back. The revision of
2026-09-23 restored the conformance harness and the end-user host.

**What it departed from.** Roadmap section 10 said the mapping mechanism "leaves on the second consumer
and not on the first". The arming path left the JavaScript profile with one consumer, and no gate was
invoked. No composition registers the `broiler.machinecode` descriptor, so its verifier and executor are
reachable from nowhere, and the one implementation of `IVmNativeCompiler` is reachable from nowhere
either ([the suggested task](../tasks/repair-or-retire-js-native-compiler.md) describes its defects).

**What this record does with it.** It takes no action on the MachineCode project. The programme plans to
rename that project as the `x86` emitter's execution half at milestone UBC-5 (work package UBC-5.3),
keeping the arming path there until a second executing emitter exists (route UBC-R8), and retiring the
`"BMC\0"` form; that milestone cannot meet its gate while candidate B's G1 stays unsatisfied. Until a
later record rules on it, the arming path's move stays what the register calls it: a refactor with no
record behind it, which this section now describes.

## Routes this record takes, filed as rows of the MVP register

- **UBC-R1** is filed as [MVP-10](../mvp.md#5-routes-taken-without-a-decision). ADR 0011's promise P1
  bounds a profile's Broiler.VM reference set to Abstractions and Binary. A profile that references
  `Broiler.VM.Ubc` moves that set, and this record reads the move as the consequence of the verdict
  above - the record clause says a verdict "changes the core graph" and names the resulting edges as part
  of the record - to be written into P1 by editorial revision when a profile first takes the edge, not
  as an amendment minting core contract version 2. That is a reading, and the route says who would
  settle it. P1's text is not edited by this record.
- **UBC-R2** is filed as [MVP-11](../mvp.md#5-routes-taken-without-a-decision): the primitive table is
  mechanism. This record does not settle it, for the reason its verdict section gives.
- **The reopening of a permanent standing refusal** is filed as
  [MVP-12](../mvp.md#5-routes-taken-without-a-decision), for the reason the section above gives.

## Exclusions

This record defines no exclusion identifier.

## Consequences

- The ADR index gains this record. It is the third record that is not contract-bearing, beside ADR 0001
  and ADR 0012; the contract-bearing set, 0002 through 0011, is unchanged, which rule E2 asserts.
- [ADR 0003 section 11](0003-core-contract-v1-and-amendments.md)'s roadmap-amendment register gains rows
  19 to 22, applied on this record's date: the roadmap's section 1 non-goal, its section 8 candidate
  row, its section 10 output-form row and its section 16 first risk row, each with the superseded text
  quoted in the register and beside the revised sentence.
- [ADR 0011](0011-source-level-profile-contract.md) gains the pointer paragraph beside its
  standing-refusals table; its table and its promise P1 are not edited.
- The README's component-boundary sentences carry a dated correction; the composition register's
  paragraph on the 2026-09-18 extraction gains a pointer to the section above.
- The JavaScript plan's intermediate-form sentence is answered by decision JSD-0036, and both profile
  plans carry a dated paragraph naming the concept as a proposal standing beside them. No WebAssembly
  correction is filed: nothing in that tree has changed.
- [The programme ledger](../universal-bytecode.status.md) moves UBC-0, and names for UBC-3 and UBC-5 to
  UBC-10 the unsatisfied G1 of candidate B as the reason they cannot meet their gates, with its holder.
- **Nothing is reviewed, accepted, advertised, packed or published by this record**, under
  [`docs/mvp.md`](../mvp.md).
