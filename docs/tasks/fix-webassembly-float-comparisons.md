# Suggested task — Fix WebAssembly float comparisons never executing

**Status:** suggested, not scheduled, no owner. Found 2026-09-25 while surveying the tree for
[the universal bytecode concept](../universal-bytecode.md) (its section 2.2 names it). This document
is the task written out so that a session starting from it alone has what it needs; it moves no
ledger row and claims nothing about what any run has shown.

## In one paragraph

The WebAssembly interpreter validates the twelve float comparison instructions but routes them to its
integer arm, so any module that compares two floats verifies and then fails at run time with a
contract violation rather than an answer. The correct comparison code exists in the same file and is
called from nowhere.

## What is wrong, checkable against the files named

All paths are relative to the repository root.

- `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs`
  - `TryNumeric` routes every opcode from `0x45` to `0x8A` to `Integer(opcode, ref trap)`.
  - `Integer` handles the integer opcodes only: the `eqz` pair, the `i32` comparison and arithmetic
    ranges, and `(>= 0x51 and <= 0x5A) or (>= 0x7C and <= 0x8A)` for `i64`. It has no case for
    `0x5B` to `0x66` and answers `false` for them.
  - A `false` from the numeric dispatch ends the run as `WasmRunStatus.Defect`.
  - `FloatComparison(byte opcode)` holds the correct logic for the twelve opcodes (`f32.eq`,
    `f32.ne`, `f32.lt`, `f32.gt`, `f32.le`, `f32.ge`, and the six `f64` counterparts). Searching the
    file for `FloatComparison` finds only its declaration.
- `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` accepts the same opcodes (the arms
  `>= 0x5B and <= 0x60` and `>= 0x61 and <= 0x66`), so a module using them is a verified artifact.
- `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` maps `Defect` to
  `ContractViolation(ProfileContractViolation)`, which is the answer the caller sees: a defect in the
  profile reported as such, never a trap and never a result.
- `src/Broiler.VM.Profile.WebAssembly/docs/roadmap.status.md` states that the numeric surface runs
  for all four types. That sentence is untrue on this point until the fix lands.

No test module in the harness corpus exercises these opcodes, which is why the suite is green.

## What done looks like

1. **Route the twelve opcodes to `FloatComparison`**, or fold that logic into the numeric dispatch,
   keeping fuel charging and the stack discipline identical to the neighbouring arms. No other
   opcode's behaviour moves.
2. **Add a retained fixture** to the WebAssembly harness corpus that exercises every one of the
   twelve comparisons, including:
   - NaN operands: every comparison with a NaN operand answers `0` except `ne`, which answers `1`;
   - signed zeros: `-0.0` and `+0.0` compare equal (`eq` answers `1`, `lt` and `gt` answer `0`);
   - the ordinary ordered cases at both widths.

   Look at how the existing corpus modules under `src/tests/wasm/` and the harness root
   `src/compositions/Broiler.VM.Composition.WebAssembly.Harness` are written and add the module the
   same way. The harness roots are composition roots and not test projects, and rule A11 is why; do
   not add a test project that references the profile assembly.
3. **Watch the fixture fail on the unmodified interpreter and pass after the change.** A fixture
   that was only ever seen passing is not a negative control.
4. **Record the correction rather than editing the ledger silently.** The profile's plan files never
   correct inline: append a dated `WAC-nn` entry to
   `src/Broiler.VM.Profile.WebAssembly/docs/roadmap.corrections.md` with the five fields the existing
   entries carry (identifier, where, what the plan said, what replaced it, authority and date), and
   put a bare `*(corrected: WAC-nn)*` beside the sentence in `roadmap.status.md`.
5. **Regenerate the assurance artefacts** a source change requires: the generated file headers,
   `CODE-ASSURANCE.md`, `HUMAN_REVIEW.md` and `assurance.manifest.json`. The rules in
   `src/tests/Broiler.VM.Architecture.Tests` (group J) say what must be byte-identical, and the
   generator lives in that project. Every touched unit stays `HUMAN_PENDING`.

## What this task is not

- It is not a claim that the numeric surface is otherwise complete. The `br_table` label vector is
  read whole on every execution for one fuel unit, and frame locals and operand-stack growth are not
  charged to `AllocatedBytes`; both were seen in the same survey and are separate, smaller items.
- It is not blocked on, and does not depend on, the universal bytecode concept. The concept names
  this defect as the shape its differential check exists to catch; fixing it now is independent of
  whether that concept is ever taken.
