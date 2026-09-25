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
