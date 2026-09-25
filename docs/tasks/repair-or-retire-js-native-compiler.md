# Suggested task — Repair or retire the unreachable JavaScript-to-BMC native compiler

**Status:** suggested, not scheduled, no owner. Found 2026-09-25 while surveying the tree for
[the universal bytecode concept](../universal-bytecode.md) (its section 2.3 describes the component).
This document is the task written out so that a session starting from it alone has what it needs; it
moves no ledger row and decides nothing.

## In one paragraph

`JsNativeCompiler` is the only implementation of the core's native-compilation seam
`IVmNativeCompiler`. Nothing in the repository calls it, and it disagrees with the format it reads and
with the profile it targets in five separate ways, so it cannot have worked on any input. The profile
it produces artifacts for, `broiler.machinecode`, is registered in no catalog either. The component
should be made correct and exercised, or removed with the reason recorded; a quiet fix is the one
outcome the repository's records discourage.

## The pieces involved

All paths are relative to the repository root.

| Piece | Where |
|---|---|
| The core seam | `IVmNativeCompiler` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs`; `VmRuntime.CompileToMachineCode` in `src/Broiler.VM.Runtime/VmRuntime.cs`; `VmNativeCompilationResult` in `src/Broiler.VM.Runtime/VmNativePipeline.cs` |
| The one implementation | `src/Broiler.VM.Profile.JavaScript.Compiler/JsNativeCompiler.cs` |
| The target profile | `src/Broiler.VM.Profile.MachineCode/` — `MachineCodeProfile.cs`, `MachineCodeFormat.cs` (the `"BMC\0"` container), `MachineCodeVerifier.cs`, `MachineCodeExecutor.cs`, `VmNativeFrame.cs`, `VmNativePage*.cs` |
| The only test of the seam | `src/tests/Broiler.VM.Contract.Tests/NativePipelineContractTests.cs`, which uses a stub compiler and never touches `JsNativeCompiler` |
| What the compositions actually use | `VmNativePage` only, through the `JsNativePage.Mapper` hook, in `src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler`, `...JavaScript.Conformance` and `...JavaScript.Cli` (`Program.cs` in each). None registers `MachineCodeProfile.Descriptor`. |

## The defects, each checkable against the files named

1. **Entry section layout.** `JsNativeCompiler.TryExtractSections` reads each entry of the
   JavaScript artifact's Entries section as `(functionIndex, nameLength, name)`. The verifier's
   `ReadEntries` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` reads `nameLength` first, then
   the name bytes, then the function index, and the writer `JsArtifactWriter.Entries` in
   `src/Broiler.VM.Profile.JavaScript.Format/JsArtifactWriter.cs` writes that order. Slicing with the
   misread length can throw on an ordinary entry such as `main`.
2. **Native section header width.** It reads the NativeCode header's four fields (architecture,
   backend version, alignment, code length) as variable-length integers. `JsArtifactWriter.NativeCode`
   writes four fixed-width little-endian `u32` fields (sixteen bytes) before the code. The misread
   "existing emission" is reused whenever the misread architecture value happens to equal the
   requested one; it also hard-codes emitter version `1` and alignment `16` and ignores the form byte.
3. **Architecture codes swapped.** `EmitBmcArtifact` writes `X64Windows` as `1` and `X64SystemV`
   as `2`. Both `JsNativeArchitecture` (`src/Broiler.VM.Profile.JavaScript.Format/JsNativeFrame.cs`)
   and `VmNativeArchitecture` (`src/Broiler.VM.Profile.MachineCode/VmNativeFrame.cs`) define
   `X64SystemV = 1` and `X64Windows = 2`.
4. **Invalid manifest identity.** It calls `VmFeatureManifestId.Parse("broiler.machinecode.x86_64")`.
   The identity grammar in `src/Broiler.VM.Abstractions/VmProfileId.cs` refuses an underscore, so
   this throws for every `x86-64` target — and `x86-64` is the default when no architecture is named.
   `MachineCodeProfile` declares `broiler.machinecode.x86-64` with a hyphen, and
   `MachineCodeVerifier` checks `manifestStr.EndsWith("x86_64", …)`, which can never match for the
   same reason, so the verifier's `x86-64` architecture check never runs.
5. **A probe of the host.** With no architecture named it chooses the convention from
   `OperatingSystem.IsWindows()`. The roster remark in
   `src/Broiler.VM.Profile.JavaScript.Compiler/JsNativeBackend.cs` forbids exactly that — "a name
   and not a probe of the running machine" — because one source would then compile to two different
   artifacts on two machines, which breaks the determinism rule re-emission verification rests on.
6. **`CanCompile` admits the wrong manifests**: it accepts `broiler.javascript` (a profile identity,
   not a manifest), `broiler.javascript.slice` (format version 1, which has no native form) and
   `broiler.javascript.numeric`, and refuses `broiler.javascript.wide`, which the baseline native form
   compiles.

Two neighbouring observations in the same component, worth deciding together:

- `MachineCodeVerifier` casts untrusted counts and lengths to `int` without checked arithmetic, sizes
  `new List<MachineCodeEntry>((int)entryCount)` from an untrusted count, and allocates the symbols
  array without reserving against the meter; it also never decodes an instruction, so it answers
  nothing about the code it admits. The WebAssembly verifier next door is total over malformed input
  and is the shape to copy if the verifier is kept.
- `MachineCodeExecutor` gives native code a fixed fuel allowance of `1L << 32` regardless of the
  budget, charges the spent fuel to the meter after the call and ignores whether the charge was
  admitted, never polls, and never reads a result out of the frame; its descriptor declares
  `CallDepth`, `LiveBytes` and `WallClock` as charged while nothing charges them.

## What done looks like — one of two routes, stated in the commit and in a record

### Route A — repair

1. Fix the six defects. Prefer reading the JavaScript artifact through the format assembly's own
   reader over a second hand-written decoder, so the layout cannot drift again.
2. Add a contract test that compiles a real `broiler.javascript.numeric` artifact through
   `VmRuntime.CompileToMachineCode` and verifies and instantiates the result with the MachineCode
   profile's descriptor in a catalog, under JIT at least.
3. Make `MachineCodeVerifier` total over malformed input, with a small retained corpus in the form
   ADR 0011 publishes (bytes, hash, pinned answer), and make the executor's charging match its
   descriptor or narrow the descriptor to what is charged.
4. Record, in the JavaScript profile's `roadmap.backends.md` or its decision series, that the BMC
   form drops the bytecode beside the machine code, which the differential oracle and re-emission
   verification both need (that document's section 1), so it cannot be the profile's native form and
   is a separate, weaker artifact kind.

### Route B — retire

1. Remove `JsNativeCompiler`. If the owner agrees, remove the BMC format, verifier and executor too,
   keeping `VmNativePage` and its two platform halves, which the JavaScript compositions use and which
   rules B5c and X1 allowlist.
2. Decide what happens to `IVmNativeCompiler` and `VmRuntime.CompileToMachineCode`: keeping them with
   no implementation is legal but should be said; removing them moves the core's public API baseline
   (`docs/api/public-api.txt`, rule M1) and the JavaScript family's baseline
   (`src/Broiler.VM.Profile.JavaScript/docs/api/public-api.txt`, rule N10).
3. Record the reason as a dated revision of `docs/adr/0001-component-topology-and-dependency-graph.md`
   — the 2026-09-18 revision that added the MachineCode profile is the one to extend, and
   `docs/compositions.md` already notes that no decision record explains that extraction — plus a
   `JSC-nn` entry in `src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md`.
4. Update `graph.manifest.json` under `src/tests/Broiler.VM.Architecture.Tests` and ADR 0001's
   budget sentence so rules A7 and A15 agree with the tree.

### Under either route

- Regenerate the assurance artefacts (generated headers, `CODE-ASSURANCE.md`, `HUMAN_REVIEW.md`,
  `assurance.manifest.json`) as group J of the architecture rules requires. Every touched unit stays
  `HUMAN_PENDING`.
- State no figure and no capability claim: the MachineCode profile has no retained evidence and no
  runtime identifier, and this task adds neither.

## Relationship to the universal bytecode concept

`docs/universal-bytecode.md` proposes superseding this component: a native form becomes an emission
section inside one universal container rather than a second artifact, and the arming path becomes the
execution half of an emitter profile. That concept is a proposal with no decision behind it. This
task does not depend on it and should be decided on its own merits; if Route B is taken, the concept's
section 2.4 already treats the BMC form as retired and nothing there needs to change.
