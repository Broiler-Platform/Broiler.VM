# Suggested task — Pace the WebAssembly decoder's bulk reads

**Status:** suggested, not scheduled, no owner. Found 2026-09-25 while building the WebAssembly
translator of [the universal bytecode programme](../universal-bytecode.roadmap.md)'s milestone UBC-4,
which reuses this decoder, and named in that milestone's bundle
[`ubc-4-002`](../evidence/ubc-4-002/README.md) as not fixed there. The decoder behaved the same way
inside the core's verification before that milestone. This document is the task written out so that a
session starting from it alone has what it needs; it moves no ledger row and claims nothing about what
any run has shown.

## In one paragraph

The WebAssembly decoder reads a function body, a data segment's contents and a skipped custom section
each as one bulk read, and the bounded reader charges a bulk read's verifier work whole and polls only
after it. When one read takes the work since the last poll past the profile's uncharged-work bound
(`WebAssemblyProfile.MaxUnchargedWork`), the meter latches a poll-bound breach and the module is
refused with `ResourceExhaustion` on `VerifierWork` although nothing is wrong with it: a single body
or segment longer than the bound, a shorter one read after enough unpolled work, or a large custom
section.

## What is wrong, checkable against the files named

All paths are relative to the repository root.

- `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs`: the code section reads each body with one
  `TryReadBytes(codeLength, ...)`, the data section each segment's contents with one
  `TryReadBytes(byteCount, ...)`, and a custom section is skipped whole with `TrySkipSectionBody`.
- `src/Broiler.VM.Binary/VmBoundedReader.cs`: a bulk read or a skip charges its whole length through
  `ChargeWork` before it polls.
- `src/Broiler.VM.Profile.WebAssembly/WasmTranslator.cs`: the translation meter refuses a poll when
  the work since the last one exceeds `WebAssemblyProfile.MaxUnchargedWork`, as the core's
  verification meter did for the retired verifier.
- `src/Broiler.VM.Ubc/UbcArtifactReader.cs` shows the shape of the fix: it charges verifier work in
  pieces no larger than the poll granularity and polls between them.

No retained corpus entry and no harness module is large enough to meet it, which is why every lane is
green.

## What done looks like

1. **A harness check that shows it**, in `src/compositions/Broiler.VM.Composition.WebAssembly.Harness`
   (rule A11 forbids a test project referencing the profile): a valid module with one large function
   body, one with a large data segment and one with a large custom section, each refused today.
   Watch them fail before the change.
2. **Every bulk read is paced**: charged before the read in pieces no larger than the poll bound,
   polling between them.
3. **Every harness check and every retained corpus entry answers as recorded**
   (`--verbose --corpus src/tests/wasm/corpus` from the repository root after
   `dotnet build Broiler.VM.slnx -c Release`), the architecture and contract suites stay green, and
   the assurance artefacts are regenerated.

## What this task is not

- It is not a change to what the decoder admits: a module that is refused for what it holds stays
  refused, with the same answer.
- It is not a question for the universal bytecode's own reader, which is already paced.
