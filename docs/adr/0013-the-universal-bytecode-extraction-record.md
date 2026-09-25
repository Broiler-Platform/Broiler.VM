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
makes filing this record its first milestone, UBC-0, and says a refusal there ends the programme with
the record filed and is a met gate.

**This record is that invocation.** It was invoked on 2026-09-25 by the core architecture owner, with
the JavaScript and WebAssembly profile owners supplying their halves - the file paths, the revisions
and the correspondence rows below - and recording no verdict of their own: the verdict is the core's.
All of those roles, and the four others ADR 0012 names, are held by one person (Exclusion EX-30 of ADR
0012), and nothing below claims that a second reader has checked any of it.

**It is not contract-bearing.** It names no lifecycle transition, no result category and no public type
of core contract version 1, and it adds nothing to the core's three assemblies: the assembly it admits,
`Broiler.VM.Ubc`, references the core and is referenced by nothing in it. It changes no graph on its
date either, because no project it admits exists yet; each project arrives with its own dated revision
of ADR 0001, in the milestone that writes it.

**Two candidates are ruled on, separately.** The concept proposes two shared assemblies, and the
evidence for them is not the same. **Candidate A** is the universal bytecode itself: the container, the
common family, the family instruction-table schema, the primitive table, the verifier walk, the family
and emitter contracts and the descriptor factory (the proposed `Broiler.VM.Ubc`), together with the one
dispatch loop that executes it (the proposed `Broiler.VM.Emitter.Bytecode`). **Candidate B** is the
native-form mechanism the concept places beside it: the emitted frame and activation contracts, the
handler-table wrappers, the template schema and the template-closure scan (the proposed
`Broiler.VM.Ubc.Native`). Section "The verdict" rules on each.

## The two implementations

Both are read from the checkout at revision `51e0d60ee43ce34e8d21b1d9b10d086cdb723501` (`main`, a clean
working tree), which is the revision every file, member and correspondence row below is taken from.

| Implementation | Assemblies | Last commit touching the tree at that revision |
|---|---|---|
| The JavaScript profile | `src/Broiler.VM.Profile.JavaScript` (the verifier, the interpreter, the runtime), `src/Broiler.VM.Profile.JavaScript.Format` (the opcodes, the container schema, the native template tables and the scan) | `0473fc3` (2026-09-25) |
| The WebAssembly profile | `src/Broiler.VM.Profile.WebAssembly` (the decoder, the validator, the interpreter, the store) | `d16a6aa` (2026-09-19) |

Both are product profiles in ADR 0011's sense: neither is the fixture profile or an application-local
consumer profile, and both are merged code at the revision named. The bundle of this milestone,
[`docs/evidence/ubc-0-001/`](../evidence/ubc-0-001/README.md), retains every file this record cites,
named by its git blob identifier at that revision.

## The correspondence table

Each row names one mechanism, the member that implements it on each side, and the part of it that
names no language. The member lists are the entry points; the section after the table says what each
side does. **The shared-part column is scanned for the banned vocabulary** of
[`docs/ubc/banned-vocabulary.txt`](../ubc/banned-vocabulary.txt) by `eng/ubc-vocabulary-scan.py`, the
scan rule U2 will read the same list with; the run is retained in the bundle.

| Row | Mechanism | JavaScript implementation | WebAssembly implementation | Shared part |
|---|---|---|---|---|
| a | Artifact framing and the section walk | `JavaScriptVerifier.Verify` and `JsVerifier.Verify`, `ReadManifest`, `ReadSection` and one reader per section kind, then `Link` (`JavaScriptVerifier.cs`, `JsVerifier.cs`); `JavaScriptReadAdapter`, `JsVerifier.FromReader` | `WebAssemblyVerifier.VerifyCore`; `WasmDecoder.TryDecode`, `TryReadPreamble`, `TryReadOneSection`, `TryAdmitSectionPosition`, `TryDecodeSectionBody`, `TryCheckSectionAgreement` (`WasmDecoder.cs`); `WasmReadAdapter`, `WasmRefusal.FromReader` | A container is a magic tag, a format version and a sequence of framed sections. The verification ceilings are projected into one set of read bounds before the first byte is read, and one bounded reader with one meter adapter reads the whole payload. The descriptor's format version and feature manifest are ruled on against the payload's, and a disagreement is a descriptor mismatch. Each section is a kind and a byte length, admitted by kind, canonical order and uniqueness, entered (charging section count and nesting depth), read by the grammar of its kind and exited with a check that exactly the declared length was consumed. An undefined kind, an order or repetition fault, a length mismatch, trailing bytes and a missing required section are refusals. Every count is held to the declared-count ceiling before a buffer is sized from it, and agreement between sections is checked after the last. Every refusal is either an invalid-artifact outcome with a core reason, a registered diagnostic code and a position, or a resource exhaustion naming one budget dimension at artifact scope, and one mapping from the reader's latched status chooses between them. |
| b | The code walk with height and target checks | `JsVerifier.Link` and `JsVerifier.Walker.Walk`, `Seed`, `Check`, `Operand` over `JsOpcodes.IsDefined`, `InstructionWidth`, `Shape`, `TryDescribe`, `HasCodeTarget`, `IsTerminal` (`JsVerifier.cs`, `JsOpcode.cs`); `JavaScriptVerifier.Walk` for format version 1 | `WasmValidator.TryValidateModule`, `TryValidateBody`, `TryStep` and the control, operand and immediate helpers; `WasmFunctionBody.TrySeal` and the sealed `WasmJumpTarget` table (`WasmValidator.cs`, `WasmModule.cs`) | For each code unit, one walk over its instruction bytes computes the abstract operand state at every reachable instruction from a per-opcode effect: fixed pops and pushes, or a count taken from the operand. Every successor (fall-through, taken edge, and every extra entry a region declares) receives an expected state, and a second arrival with a different state is a join disagreement. The walk refuses an undefined opcode, an instruction cut short by the unit's end, underflow, a height above a ceiling, a transfer target outside the unit, a path that runs past the unit's end and an index outside the table it names, each with the instruction's offset. It charges verifier work and polls on the declared cadence. What it computes (the height high-water mark and every resolved transfer destination) is stored once, replacing any declared figure, and the executor sizes and branches from the stored results, never from payload numbers and never by searching the code. |
| c | The dispatch loop | `JsEngine.ExecuteCore<TMode>` with `Execute`, `U16`, `U32`, `Charge` (`JsEngine.cs`); `JavaScriptExecutor.Run` for format version 1 | `WasmInterpreter.Run` with `Call`, `Branch`, `MemoryAccess`, `TryNumeric` (`WasmInterpreter.cs`) | A verified unit is executed by a loop over its code bytes with an explicit program counter. At each instruction the loop records the offset for fault attribution, charges the instruction's fixed cost before its effect, reads the opcode byte, dispatches once through one switch, and lets the arm read its immediates and advance the counter or replace it with a target verification checked. A transfer never searches the code. An opcode verification admitted and the loop has no arm for is an internal defect of the executor, never a guest-visible fault. A call hands control to a new activation, and a return resumes the caller at the counter its activation saved. |
| d | The frame model | `ExecuteCore<TMode>`'s per-activation operand array and scope list; `JsEngine.Call`, `EnterCall`, `EnterDepth`, `LeaveCall`, `Invoke`; `JsFrame` for suspendable units (`JsEngine.cs`, `JsGenerator.cs`) | `WasmFrame`, the shared operand stack, `WasmInterpreter.TryPushFrame`, `PopFrame`, `EnsureStack` (`WasmInterpreter.cs`) | An activation is identified by its code unit and holds a program counter, an operand region sized from the verified high-water mark and the unit's local storage. Entry moves the arguments from the caller's operands into the callee's locals. Every activation pushed charges one unit of call depth through the core meter and releases it on every exit path, a fault or a budget stop included. A normal exit leaves exactly the unit's results where the caller expects them. An activation that can outlive its entry keeps its operand region, its nesting state and its counter in a heap object that re-entry restores. |
| e | Unwinding | the `Throw` arm; the filtered catch in `ExecuteCore` with `TryFindHandler`, `TryFindFinally`, `Land`; `JsThrow`, `JsAbort`; the executor-boundary mapping in `JsExecution` (`JsEngine.cs`, `JsThrow.cs`, `JsExecution.cs`) | `WasmInterpreter.Trapped` and the `Call` cleanup; `WasmTrapKind`, `WasmRunStatus`; `WebAssemblyExecutor.InvokeCore`, `Refused` (`WasmInterpreter.cs`, `WasmTrapKind.cs`, `WebAssemblyExecutor.cs`) | Three outcomes leave the loop and are never confused: a guest fault names a kind and a position and becomes a faulted step carrying a registered code; a budget refusal or a cancellation becomes a contract-violation step the core rewrites from its meter's latches, which no guest-level handler can intercept; an executor defect becomes a contract-violation step naming a profile contract violation. Where a unit declares handler regions, the search is a pure function of the unit and the instruction offset over the unit's region rows, innermost first; a landing truncates the operand region to the region's declared height, resumes at the region's handler offset, and lets the owner of the region kind place what lands there. With no covering region the activation is abandoned, its call depth released, and the search continues in the caller. |
| f | The charge and poll sites | `JsEngine.Charge`, `ChargeOnce`, `EnterDepth`, `ChargeHostCrossing` and the proportional charges inside arms (`JsEngine.cs`); the declared bounds in `JavaScriptProfile` | `WasmPacing.TryReserve`, `TryCharge`, `Observe`; `WasmMemoryInstance.Grow` (`WasmInterpreter.cs`, `WasmMemory.cs`); `WebAssemblyProfile.MaxUnchargedWork` | Fuel is charged through the core meter at a fixed cost per instruction, before the instruction's effect. Work done between polls never exceeds the declared uncharged-work bound: a poll is taken before any charge that would cross it, either by splitting the charge or by reserving the worst case first. Work proportional to a size the guest controls is charged by the instruction that performs it, and any charge made elsewhere is fed back into the poll accounting. A refused charge ends the step as exhaustion and a refused poll as cancellation, and the core's latches decide which the caller is told. |
| g | Constant and immediate decoding | `JsVerifier.ReadConstants`, `ReadBigInt`, `Walker.Operand`, `Walker.Check`; `JsOpcodes.Shape`, `OperandWidth`; `JsEngine.U16`, `U32` (`JsVerifier.cs`, `JsOpcode.cs`, `JsEngine.cs`) | `WasmLeb128` readers; `WasmValidator.TryReadVarU32` and its siblings; `WasmInterpreter.ReadU32`, `ReadS64`, `ReadFixed`, `SkipVarInt`; `WasmDecoder.TryReadConstantExpression` (`WasmLeb128.cs`, `WasmValidator.cs`, `WasmInterpreter.cs`, `WasmDecoder.cs`) | Every immediate an instruction carries sits in the code after the opcode byte. It is decoded and range-checked once at verification against the table it indexes, or the ceiling that bounds it, and the executor reads the same bytes again without re-checking them. A literal verification decoded is taken by index or re-read, never re-validated. A malformed immediate encoding is an invalid artifact with a malformed-encoding reason and a code of its own. A count carried in the code is held to the declared-count ceiling before it sizes anything or bounds a loop. |
| h | The native-form machinery | `JsNativeActivation`, `JsBaselineHandlers` (`src/Broiler.VM.Profile.JavaScript`); `JsNativeScan`, `JsNativeTemplates`, `JsBaselineBlocks`, `JsBaselineFrame`, `JsNativeFrame` (`src/Broiler.VM.Profile.JavaScript.Format`); the encoders under `src/Broiler.VM.Profile.JavaScript.Compiler` | none: the profile has no native form, no template table, no handler table, no emitter and no activation cookie | none. One implementation exists and there is no second to compare it with. |

### What each side does, row by row

**Row a.** The JavaScript side builds a read adapter over the verification meter, projects the
ceilings, reads a four-byte magic and a canonical variable-length format version, and checks the
version against the descriptor first; format version 2 then reads a manifest identity held to the
descriptor's and a declared section count, and walks that many sections with a poll before each, the
kind and the length canonical, kinds strictly ascending, every exit failure a section-length mismatch;
trailing bytes and missing required sections are refused, and `Link` checks the cross-section rules.
Buffers are allocated after a count has passed the declared-count bound and a format ceiling, and are
not reserved against the allocated-bytes allowance. The WebAssembly side rules on the descriptor before
reading any byte, checks a four-byte magic and a fixed-width binary version, and reads sections until
the payload ends rather than to a declared count; it admits each by order rank and duplication before
entering it, reads lengths with a reader that accepts padded encodings (the wire format requires it),
skips custom sections, reserves every buffer through the bounded allocator, and checks that the
function and code sections agree after the last section.

**Row b.** The JavaScript side walks each unit with a worklist of offsets, one height and one scope
depth per offset, seeding the entry and every region handler of the unit, charging one work unit per
worklist pop, reading every effect from `JsOpcodes.TryDescribe`, checking the taken edge's height for
every opcode with a code target, and requiring a `Return` at height exactly one; a target is checked
against the unit's range and not against a boundary set, which format version 1 still keeps. The
WebAssembly side walks each body once, in the specification's algorithm, over a typed operand stack and
a control stack, charging structural depth per nesting level, and seals each body with its operand
bound, its label bound and a table of every block, loop and if boundary that the interpreter later
reads instead of searching.

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
larger charge into windows of half the declared poll bound, polling once a window is spent. The
WebAssembly loop reserves poll headroom before each charge and polls only when the next charge would
cross the bound, and memory growth reserves the worst case first, charges pages and allocation, and
feeds the charged amount back into the poll accounting.

**Row g.** JavaScript literals live in a constant pool decoded once at verification; operands are fixed
width, the width a function of the opcode alone, decoded once by the walk and re-read by each arm.
WebAssembly has no pool: literals and indices are variable-length immediates, validated once with a
reader that accepts padding within the width's byte budget, and decoded again at every execution by
readers that never refuse.

**Row h.** Stated as one implementation, as the roadmap's work package UBC-0.1 asks. The JavaScript
profile has three native forms over its bytecode (the numeric form, the baseline form of
[JSD-0025](../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0025-the-baseline-native-form-over-the-wide-manifest.md)
and the refused value form of JSD-0035 kept opt-in): a thread-static activation with a cookie, a
handler table of `[UnmanagedCallersOnly]` wrappers in unmanaged memory, per-architecture template
tables, and a scan that decodes emitted bytes against them. The WebAssembly profile has nothing of the
kind.

### Differences the shared design must decide, found while reading

These are recorded because a correspondence that hid them would be asserting rather than showing, and
because each is a decision the universal bytecode takes rather than inherits:

- a declared section count against reading to the end of the payload;
- canonical variable-length integers against padded ones, and a variable-length against a fixed-width
  format version;
- ignorable custom sections, which only the WebAssembly side has;
- a reservation against the allocated-bytes allowance before every buffer, which only the WebAssembly
  side makes;
- a transfer target checked against the unit's range (JavaScript, format version 2) against a
  boundary set (format version 1) and a sealed table (WebAssembly);
- a typed operand stack (WebAssembly) against an untyped height (JavaScript);
- guest depth as CLR recursion (JavaScript) against one loop over heap frames (WebAssembly);
- faults as exceptions (JavaScript) against status codes (WebAssembly);
- every `TryExitSection` failure mapped to one code (JavaScript) against only one reader status mapped
  so (WebAssembly);
- a refused verifier poll answered as verifier-work exhaustion in one JavaScript pass, as wall-clock
  exhaustion in the other, and as an invalid artifact inside one JavaScript section reader;
- a verifier with no exception wrapper (JavaScript) against one that turns any escape into a named
  defect code (WebAssembly);
- instruction widths written into each interpreter arm (JavaScript) and no descriptor table at all
  (WebAssembly), which is what a one-table rule over the common family has to replace;
- the WebAssembly interpreter's numeric dispatch sends the float comparisons to its integer arm, which
  has no case for them, while the correct float-comparison code has no caller (the concept's section
  2.2; `docs/tasks/fix-webassembly-float-comparisons.md`);
- the WebAssembly `br_table` reads its whole label vector under one fuel charge.

**And one sentence of the concept that the code does not bear out.** The concept's section 5.9 quotes a
roadmap risk row, "`CallDepth` is declared by every profile and charged by no code in this repository";
at the revision named both profiles charge it (`JsEngine.EnterDepth`, `WasmInterpreter.TryPushFrame`).
Its section 5.9 also says that in the interpreter a frame is a heap object and the loop one CLR frame
regardless of guest depth: that is true of the WebAssembly side today and not of the JavaScript
bytecode path. Neither observation changes a verdict below; both are carried to the concept as dated
notes by the milestone that corrects it.

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
- **Slots, shapes and descriptors:** `UbcSlotType`, `UbcOperandShape`, `UbcOperandShapes`,
  `UbcEffect`, `UbcEffectForm`, `UbcTarget`, `UbcTrapCode`.
- **The common family:** `UbcOpcode`, `UbcOpcodes`, `UbcCommonRow`, `UbcCommonEffect`.
- **The family table schema and the primitive table:** `UbcInstructionRow`, `UbcInstructionTable`,
  `UbcInstructionKind`, `UbcRegionKindRow`, `UbcRegionDeclaration`, `UbcFamilyTrap`, `UbcTrapMapping`,
  `UbcPrimitive`, `UbcPrimitives`.
- **The contracts a family implements:** `IUbcFamily`, `IUbcFamilyVerifier`, `IUbcValuePlane`,
  `UbcActivation`, `UbcStatus`, `UbcStatusKind`, `UbcCallRequest`, `UbcHookAnswer`,
  `UbcHookInstruction`, the frame codec members of `IUbcFamily`.
- **Registration, emitters and the descriptor factory:** `UbcContract`, `UbcFamilyRegistration`,
  `UbcFamilyDeclaration`, `IUbcEmitter`, `IUbcFormVerifier`, `IUbcExecutorFactory`, `UbcEmitterSet`,
  `UbcDescriptors`, `UbcCompositionException`.
- **The walk:** `UbcVerifier`, `UbcVerifiedProgram`, `UbcDiagnosticCode`.

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

**Decided 2026-09-25 by the core architecture owner.** The owner gave the verdict in advance of the
table, in these terms: accept where the correspondence table holds, and refuse any condition the
evidence fails. The table above was then written from the checkout at the revision named, each
condition was tested against it, and this section records the outcome of that test as the owner's
verdict. The owner, the JavaScript profile owner and the WebAssembly profile owner are one person; the
verdict is not independent of the proposal it rules on, and this record says so rather than resolving
it.

| Candidate | G1 | G2 | G3 | G4 | Verdict |
|---|---|---|---|---|---|
| **A** - the universal bytecode and its one loop (`Broiler.VM.Ubc`, `Broiler.VM.Emitter.Bytecode`) | **Met.** Both product profiles implement framing, a code walk with height and target checks, a dispatch loop, activations with a charged call depth, fault exits, per-instruction charges and immediate decoding (rows a to g). | **Met.** The shared part of each row is identified from the merged code of both, at the revision named, and shown as the correspondence above rather than asserted. | **Met.** Every shared-part cell and the proposed public surface name no language concept, and the banned-vocabulary scan over both is retained clean. | **Met.** The edges above create no profile-to-profile dependency. | **ACCEPT** |
| **B** - the native-form mechanism (`Broiler.VM.Ubc.Native`) | **Unsatisfied.** One product profile implements a native form; the other has no emitted code at all (row h). | Not reached: there is no second implementation to compare. | Not ruled. | Not ruled. | **REFUSE**, as a refused extraction |

**Where candidate A's G1 is weakest, stated rather than argued away.** Three features the universal
bytecode carries have one implementation each, all on the JavaScript side: region landing (row e), the
suspension of an activation (row d), and a plane of language values beside the machine words. The
verdict admits them because each is a feature of a behaviour both profiles implement - a loop over
activations that ends a step in one of the core's step kinds - and the WebAssembly side is the empty
case of each: a unit with no regions, a table with no suspending row, a family with no value plane. The
shared mechanism is identified from the implementation that has the feature, which is merged code and
not anticipation. **Row h is not of that kind**, and that is the line between the two candidates: the
WebAssembly profile has no emitted code of which "no native form" is the empty case, so there is no
behaviour two product profiles implement.

**What the refusal of candidate B means, under ADR 0011's failure clause.** The condition that failed
is G1. There is no duplication to document or to point at, because there is one implementation; the
refusal is not tracked as debt with a repayment schedule. **It reopens only when the failing condition
changes**, and this record names the two changes that would do it, each producing a new dated verdict
rather than an edit of this one: a second product profile's own implementation of emitted code over its
bytecode; or a change of dependency shape in which the machinery no longer belongs to any profile - for
example because it has moved wholly into one emitter family as that family's own, not shared, code - in
which case the new verdict first asks whether `Broiler.VM.Ubc.Native` is an extraction between profiles
at all.

**What the refusal of candidate B does to the programme.** Milestone UBC-5 (which writes
`Broiler.VM.Ubc.Native`) cannot meet its first clause while the refusal stands, and the milestones that
wait on it - UBC-6a, UBC-6b, UBC-7, and clause 8 of UBC-3 (the JavaScript native forms recovered on the
`x86` emitter) - cannot be met either. The programme ledger records UBC-5 as blocked on this record,
naming the core architecture owner as the holder. **Nothing in candidate A waits on candidate B**:
milestones UBC-1, UBC-2, UBC-4, UBC-8 and UBC-9, and every clause of UBC-3 but its eighth, proceed on the
acceptance of candidate A.

## The standing refusal this record reopens

ADR 0011's standing-refusals table has a row "A shared value representation, frame layout, or opcode
set", whose verdict is "Refused permanently; these are the semantics the core exists not to own." Its
failure clause says a refused record reopens when "a language-free formulation is found", and that
reopening "produces a new dated verdict rather than an edit of the old one". **This section is that new
dated verdict, and it is given on the opcode-set half of the row only.**

- **The opcode set, reopened and accepted on 2026-09-25, as an encoding.** What candidate A shares is
  the encoding (one byte space, fixed operand widths, a prefix per family, a table schema every family
  fills in), a common family whose every row's meaning is stated without reference to any language
  (control transfer, calls, locals, word constants, stack shuffles, a trap), and a primitive table of
  machine operations stated in two's-complement and IEEE-754 terms, against which a family's own handler
  is checked. The meaning of every family instruction stays in the family that declares it. That is the
  language-free formulation the reopening clause names, and G3 above is its test.
- **A shared value representation stands refused.** A slot of the universal bytecode is a machine word
  or a language value whose representation, lifetime and meaning belong to one family; no family reads
  another's, and no value crosses between families inside one artifact or one unit.
- **A shared frame layout stands refused.** The frame mechanism belongs to the emitter; no family reads a
  frame's layout, and the layout is not shared between languages because no language sees it.

ADR 0011's table is not edited. It gains an editorial pointer paragraph naming this record, as the
programme roadmap's work package UBC-0.3 asks.

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
`"BMC\0"` form; that milestone is blocked by candidate B's refusal above. Until a later record rules on
it, the arming path's move stays what the register calls it: a refactor with no record behind it, which
this section now describes.

## Routes this record takes, filed as rows of the MVP register

- **UBC-R1** is filed as [MVP-10](../mvp.md#5-routes-taken-without-a-decision). ADR 0011's promise P1
  bounds a profile's Broiler.VM reference set to Abstractions and Binary. A profile that references
  `Broiler.VM.Ubc` moves that set, and this record reads the move as the consequence of the verdict
  above - the record clause says a verdict "changes the core graph" and names the resulting edges as part
  of the record - to be written into P1 by editorial revision when a profile first takes the edge, not
  as an amendment minting core contract version 2. That is a reading, and the route says who would
  settle it. P1's text is not edited by this record.
- **UBC-R2** is filed as [MVP-11](../mvp.md#5-routes-taken-without-a-decision): the primitive table is
  mechanism. This record's G3 finding for candidate A is the dated verdict that route names as what
  would settle it, and the route stays in the register so that a reader who disagrees has a row to
  disagree with.

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
- The JavaScript plan's intermediate-form sentence is discharged by decision JSD-0036, and both profile
  plans carry a dated paragraph naming the concept as a proposal standing beside them. No WebAssembly
  correction is filed: nothing in that tree has changed.
- [The programme ledger](../universal-bytecode.status.md) moves UBC-0 and records UBC-5 as blocked on
  candidate B's refusal.
- **Nothing is reviewed, accepted, advertised, packed or published by this record**, under
  [`docs/mvp.md`](../mvp.md).
