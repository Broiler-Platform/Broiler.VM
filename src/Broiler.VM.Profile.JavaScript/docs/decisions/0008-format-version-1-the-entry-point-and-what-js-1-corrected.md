# JSD-0008 - Format version 1, the entry-point answer, and four things JS-1 corrected

**Status:** Accepted for JS-1.

**Date:** 2026-08-31

**Owner:** profile contract owner. **Co-signer:** the release and AOT reviewer of the composition
root. **Both roles are held by one person** and this record does not claim the co-signature is
independent.

**Milestone:** JS-1.

## Decision: what format version 1 carries

Version 1 is defined with `broiler.javascript.slice` and carries, from the first byte, everything
whose retrofit would be a format-version break:

| Carried | Shape at version 1 |
|---|---|
| Magic, format version, feature-manifest identity | `BJSB`, a variable-length version, and the manifest the artifact was produced for, all before any section |
| Length-framed sections with a declared count | Read through the core's bounded reader. Strictly ascending kinds, each at most once, an unknown kind refused outright |
| A constant pool | Tagged entries: `undefined`, Boolean, Number, and **`InternedName` reserved and admitted by no manifest** |
| A code section with fixed instruction boundaries | One opcode byte and a fixed operand width, so a boundary is computable without decoding |
| Exception regions with nesting and `finally` targets | **Framed and parsed from version 1; a non-zero count is refused** |
| Suspension and resume targets | **Framed and parsed from version 1; a non-zero count is refused** |
| A canonical position table | Bytecode offset to line and column, strictly ascending, every offset an instruction boundary |
| Declared maxima for operand stack, locals, frames, constants | **Declared for checking and never used to size an allocation before the bound comparison** |

**The two reserved sections are the load-bearing part of this decision.** The slice admits neither
exception handling nor suspension, and a format that simply lacked those sections would need a
version break to gain them. Framing them now, parsing them, and refusing a non-zero count means
JS-7 and a later manifest grow into a shape that already exists - and means no artifact has ever
carried one silently.

## Decision: the entry-point answer

**An artifact declares named program entries and an invocation names one.** Roadmap section 10
offers three answers to the problem that an invocation request carries one UTF-8 name and no
argument channel; this is the first of them.

- Arguments, where a program needs them, are **encoded by the lowering into the artifact the host
  asked for**. That is what a browser does anyway, because the caller-driven path compiles a
  *program* rather than a call.
- The entry-point name is resolved against the artifact's own table. A name nothing is bound to is
  a **ReferenceError**, carried as this profile's typed fault - because resolving a name and
  finding nothing is what that error is in the language, and what an entry-point name means is
  this profile's business.
- The conventional name is `main`. Nothing in the format privileges it.

**The cost is recorded rather than hidden.** A host that wants to call `f(1, 2)` against an
already-instantiated realm cannot: it must lower a new program and verify it. Roadmap section 18's
argument-channel row - re-graded **strong** at JS-0, on the other profile's grounds as much as this
one's - is the amendment that would remove the cost, and it is filed and held rather than
scheduled.

**Rejected: encoding the call into the entry-point text.** It works and it is ugly, and it makes
the entry-point name a parsed surface with its own grammar, its own escaping and its own early
errors - a second format inside a string, in the one place the contract deliberately carries bytes
it does not interpret.

**Not yet available: lowering a one-line calling program as a guest-initiated load.** It is
correct, it costs a verification, and it needs the mediator, which is JS-8's.

## Correction 1 - four budget rows say `NotApplicable` where JSD-0004 said charged

[JSD-0004](0004-limit-defaults-hard-maxima-and-the-budget-matrix.md) records the intended matrix
for the profile this component is growing into: every dimension charged. The JS-1 descriptor
declares **`HostCalls`, `NestedLoadDepth`, `NestedLoadFanOut` and `NestedLoadBytes` inapplicable**,
because the slice imports no host capability and declares no guest-initiated load, which makes
those four structurally unreachable rather than merely unused. Declaring them charged would be a
claim the rest of the descriptor contradicts two rows further down.

**JS-6 flips the host-call row** when the standard library imports something, and **JS-8 flips the
three nested rows** when guest loads are declared. The defaults on all four stay generous, for the
reason JSD-0004 gives: a default is what reaches a neighbour, and a zero on a dimension this
profile does not use is a claim about everyone composed beside it.

## Correction 2 - five descriptor rows are provisional and JS-5 settles them

`CallDepth`'s default and maximum, `MaxUnchargedWork`, `ChargingGranularity` and
`CancellationPollBound` each carry a value that is **safe rather than right**. Roadmap section 8
makes each of them measured rather than chosen, and JS-5 owns the measurement. The descriptor
marks all five and the ledger carries them, so that a reader never mistakes a placeholder for a
result.

**One of the five is sharper than the others.** A recursing program must be refused as
`ResourceExhaustion` naming `CallDepth`, on every claimed RID under Native AOT, rather than
terminating the process - and the slice has no functions, so there is nothing here to recurse and
nothing to measure yet. The number is carried, not earned.

## Correction 3 - the composition roots are not named `Broiler.VM.Profile.JavaScript.Composition.*`

Roadmap section 5 proposes that name. **It cannot be used**, and the reason is worth recording
because it is a naming collision rather than a preference: every architecture rule that identifies
a profile assembly does so by the `Broiler.VM.Profile.` prefix, so a composition root under that
prefix *is* a profile assembly to rules A8, A11 and A13. Rule A8 - no profile project references
the runtime - fired on the first build, correctly, because a composition root must reference the
runtime.

The roots are therefore `Broiler.VM.Composition.JavaScript.ExecutionOnly` and
`Broiler.VM.Composition.JavaScript.SliceCompiler`, matching the core's own
`Broiler.VM.Composition.*` convention. Rule N4 no longer covers them and A12 and the composition
register hold them instead, which is the right pair of rules for a composition root.

## Correction 4 - two composition roots, not one binary with two modes

The gate names one composition; this milestone builds two projects. **They differ by exactly one
reference - the lowering - and that difference is the whole of the `execution-only` label.** The
execution-only root names the profile and not the compiler, so it cannot turn source into an
artifact however it is invoked, and every artifact it runs is precompiled and read as bytes from
the retained corpus. The slice-compiler root names both, lowers the slice programs, writes that
corpus, and carries the cross-profile checks that need a neighbour.

A flag on one binary would have made the difference a run-time choice inside one closure, and **a
closure report cannot see a flag**. The published closures are the evidence: six managed
assemblies for the execution-only image and seven for the compiler-bearing one, differing by
`Broiler.VM.Profile.JavaScript.Compiler` and nothing else.

**The slice-compiler root claims no composition label.** `narrow-runtime-compiler` belongs to a
composition carrying a lowering for a named restricted **source** surface, and there is no source
surface until JS-3b writes the tokenizer and the static semantics. What this root lowers is a
programmatic builder. It is registered as a demonstration and JS-3b claims the label.

## What this record does not decide

- **The value representation.** `JavaScriptValue` is a sixteen-byte tagged struct over three
  primitive kinds, and it is **provisional**: JSD-0004 and roadmap section 8 make the
  representation a gate on entry to JS-4, because the standard library is typed against whatever
  answer it gets. Nothing in this milestone's fixtures or figures may be read as settling it.
- **The diagnostic registry.** The codes exist and are grouped by the stage that emits them;
  JS-3a publishes and versions the registry, binds it in both directions, and records which half
  each code belongs to.
- **Anything about conformance.** No suite is pinned, no harness exists, and the retained corpus
  is this component's own - it is not a conformance result and no total appears anywhere.
