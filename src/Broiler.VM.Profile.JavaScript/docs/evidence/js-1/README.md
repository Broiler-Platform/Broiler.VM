# Evidence bundle JS-1-001

**Milestone:** JS-1 — close the whole contract loop on the smallest JavaScript that is still
JavaScript.

**Verdict this bundle supports:** JS-1 is **In progress**, not accepted. One exit-gate clause is
open and named in the exclusions, and **no human has reviewed anything**, which the roadmap makes
a precondition for `Accepted` on any milestone.

This bundle was produced by `eng/collect-js-evidence.py`. Every file beside this one is its
output. **A command written in a plan is not evidence that the command ran**; the logs are what
ran.

**No result from any other component is evidence here.** No figure, total, conformance result,
benchmark or Native AOT sample from the Broiler.VM core, from the legacy JavaScript engine, or
from any other component appears in this bundle or is cited by it.

## Identity

| Field | Value |
|---|---|
| Bundle | `JS-1-001` |
| Milestone | JS-1 |
| Core contract version | 1 (implemented; **not accepted** — see exclusions) |
| Format version | 1, accepted range 1–1 |
| Feature manifest set | `broiler.javascript.slice`, and nothing else |
| Claimed RID | `win-x64`, and nothing else |
| Owner | profile contract owner |
| Reviewer | **none** |

Owner and reviewer are the same person, and the roadmap requires the non-independence to be
recorded rather than resolved by assertion. **No decision in this bundle was reviewed by anyone
who did not make it.**

## What this milestone claims, in one paragraph

One feature manifest is minted and one format version defined. All seven core-facing types are
implemented. A descriptor is filled in one full-arity construction and admitted by a catalog. The
verifier answers all five outcomes and throws on nothing across a 51-entry retained corpus. The
executor answers four of the five step kinds and the fifth is declared unreachable at this
surface. Two composition roots **publish and run on `win-x64` under JIT, trimmed self-contained
and Native AOT**, with trim and AOT warnings treated as errors, and their published closures are
read off the published output. **Nothing here is a conformance claim**: no suite is pinned, no
harness exists, and the corpus is this component's own.

## Procedure and results

| Step | Log | Result |
|---|---|---|
| Release build of the whole solution | `build.log` | Succeeded, **0 warnings**, 0 errors |
| Whole test suite | `suite.log` | 207 contract tests and 127 architecture tests passed, 0 failed |
| Assurance gate mode | `assurance-gate.log` | Passed: every generated artefact is byte-identical to what the generator would write |
| Assurance release mode | `assurance-release.log` | **Refused, as it must.** Every relevant unit is `HUMAN_PENDING` |
| Publish and run, both roots × three modes | `publish-and-run.log` | **6 publishes, 6 runs, all exit 0.** Catalog tables byte-identical across modes for both roots |
| Closure reports | `closure-executiononly.txt`, `closure-slicecompiler.txt` | Read off the published output |
| Negative controls, suite-judged | `negative-controls.log` | 8 injected, 8 failed while injected and passed after revert |
| Negative controls, corpus-judged | `corpus-controls.log` | 4 injected, 4 failed while injected and passed after revert |

## The closures are the execution-only claim

Read off the published output, not asserted from a project file:

| Composition | Managed non-framework assemblies (JIT and trimmed) | Native AOT |
|---|---|---|
| `…JavaScript.ExecutionOnly` | **6**: the three core, the root, the profile, and the **format** | 0 managed assemblies — a native image |
| `…JavaScript.SliceCompiler` | **7**: the same, plus `Broiler.VM.Profile.JavaScript.Compiler` | 0 managed assemblies — a native image |

**The two images differ by exactly the lowering.** That is the whole of the `execution-only`
label, and it is a property of the reference set rather than a promise: the execution-only root
cannot turn source into an artifact however it is invoked, and every artifact it runs is
precompiled and read as bytes from the retained corpus.

**One correction to a core exclusion, found here.** EX-42 records that the Native AOT publish
needs a `vcvars64` shell on Windows. It does not — it needs `vswhere.exe` on `PATH`. The
ILCompiler package's own `findvcvarsall.bat` calls it unqualified, and when it is missing the
batch file's error text is substituted into the property that becomes the linker path, so the
publish fails with MSB3073 naming a command that reads as a sentence. A `vcvars64` shell without
the Installer directory on `PATH` still fails; the Installer directory without a `vcvars64` shell
succeeds.

## The corpus

51 retained entries at `src/tests/corpus/js-1/`, each carrying its bytes, its SHA-256, and the
outcome, reason, diagnostic code and completion value it is expected to produce. The replay
re-hashes what it reads, so a corpus whose bytes changed without its manifest changing is a
failure rather than a drift.

- **16 control entries verify successfully.** A corpus in which nothing passes would not notice a
  verifier that rejects everything.
- **All five verifier outcomes are produced by named entries.** `Normal`, `InvalidArtifact`,
  `ResourceExhaustion`, `Cancellation` and `UnsupportedProfile`. The last three need a host rather
  than bytes — a tight section-count ceiling, an already-cancelled token, and a descriptor naming
  a profile the catalog does not hold — so their bytes are the same well-formed artifact as a
  control's, and what the row proves is a property of the host.
- **32 invalid-artifact entries, each carrying a diagnostic code and a position.**
- **The corpus replays twice with no residue**, compared row by row.
- **The verifier threw on none of it.**

### The control entries are claims about the language, not about arithmetic

Each was chosen because a plausible implementation gets it wrong:

| Program | Value | What a wrong implementation answers |
|---|---|---|
| `1 / 0` | `Infinity` | a fault, if it guarded division like a calculator |
| `0 / 0` | `NaN` | a fault, or zero |
| `1 / -0` | `-Infinity` | `Infinity`, if the constant pool interned `-0` with `+0` |
| `-5 % 3` | `-2` | `1`, under a floored modulo |
| `2147483648 \| 0` | `-2147483648` | `2147483647`, under a C# cast instead of `ToInt32` |
| `-1 >>> 0` | `4294967295` | `-1`, or `0` |
| `1 < 2` | `true` (a Boolean) | `1`, over a bare-double value model |
| `(0/0) === (0/0)` | `false` | `true`, under a bit comparison |
| `true + 1` | `2` | `NaN`, without `ToNumber` on a Boolean |
| `1 === true` | `false` | `true`, if strict equality ignored the kind |
| an unassigned local | `undefined` | `0` |
| a counting loop to 10 | `55` | anything, if the jumps or the join checking are wrong |

**Four of those are pinned by negative controls** (`corpus-controls.log`): division by zero made a
fault, strict equality made kind-blind, `ToUint32` replaced by a cast, and the verifier's
unreachable-code check removed. Each injection made the replay fail, and naming the entry that
stopped agreeing; each revert made it pass. **A corpus that could not detect a semantic regression
would be a directory of bytes rather than a gate**, and these four are what shows it is not.

## Contract-loop checks

Run by the execution-only root in every publish mode:

| Check | What it shows |
|---|---|
| `unsupported-profile-examines-no-payload-byte` | A foreign descriptor with an **empty** payload answers `UnsupportedProfile`/`ProfileNotInCatalog`. A verifier that read anything first would fail on the empty span with a framing answer, so this is evidence about the ordering |
| `execution-step-kinds` | `Instantiated`, `Completed`, `Faulted` as a `ReferenceError` from an unbound entry-point name, and `ContractViolation` from a resume this surface never produced a continuation for |
| `suspended-is-declared-produced-at-js-7` | The fifth kind is unreachable here and is **declared** rather than reached by minting an out-of-manifest opcode |
| `operand-stack-sized-from-verification` | The artifact declares 16 and verification computes 2. The executor sizes from the computed number; the declared one is checked against it and does not become it |
| `the-caller-buffer-may-change-afterwards` | The caller's buffer is overwritten after verification returns and the answer does not move |

Run by the slice-compiler root, which is where a neighbour exists:

| Check | What it shows |
|---|---|
| `a-neighbours-maximum-does-not-reach-this-profile` | A neighbour declaring a section-count **maximum** of 1 does not stop this profile's artifact verifying. A maximum binds the artifacts of the profile that declared it |
| `a-neighbours-adopted-default-does-reach-this-profile` | The same neighbour's **default** of 1 does refuse it, naming `SectionCount` — a dimension this profile did not breach, in a verifier that did nothing wrong. **This is the exposure that survives**, and the roadmap says reconciling it belongs to whichever component composes both |
| `a-foreign-payload-is-not-projected` | A payload minted by the neighbour projects as neither of this profile's types |
| `registration-order-does-not-change-the-catalog-identity` | Two registration orders over one descriptor set encode identically |
| `descriptor-admitted-and-its-refusals-named` | The descriptor is admitted, and four named negative cases — a manifest outside its namespace, a default above its maximum, an unconstrained default, and the reserved namespace without a Broiler package — are each refused |

**The first two together are JS-0's carried two-profile catalog clause, discharged.**

## Exclusions — what this bundle does not show

1. **The public API baseline still does not cover the profile's assemblies, and the reason is now
   known rather than pending.** JS-0 carried this expecting JS-1 to close it. It cannot be closed
   the obvious way: `ApiSurface` describes a surface by `Assembly.Load`, which needs the assembly
   in the test output, which needs a project reference — and **rule A11 forbids a test project to
   reference a profile assembly**. Two routes exist and neither is a five-minute change: describe
   the surface from metadata, as `AssemblyFacts` already does for group B, or have a composition
   root print its own surface and compare against a retained artefact the way rule K3 compares a
   catalog table. **The clause is carried to JS-3b with the two routes named**, and the profile's
   public surface is frozen by nothing today.
2. **`Suspended` is declared, not produced.** The slice has no generator, no async function and no
   module, so nothing can park. JS-7 produces it. Minting an opcode that suspended in order to
   reach the answer would be widening a manifest to satisfy a gate.
3. **Five descriptor rows are provisional**: `CallDepth`'s default and maximum,
   `MaxUnchargedWork`, `ChargingGranularity` and `CancellationPollBound`. Each is measured rather
   than chosen and **JS-5 owns the measurement**; the values carried here are safe, not right. In
   particular **no recursion case exists and none can**, because the slice has no functions.
4. **No proportionality fixture ships, and that is correct here.** No operation at this surface
   has a cost that grows with its input. JS-5 introduces the families and the fixtures, and an
   operation family without one does not ship in that increment.
5. **The value representation is provisional.** It is a sixteen-byte tagged struct over three
   primitive kinds; JSD-0004 and roadmap section 8 make the representation a gate on entry to
   JS-4. No fixture or figure here settles it.
6. **One RID, one machine, one operating system.** `win-x64` is claimed because it was published
   and run; nothing here is evidence about any other, and no RID matrix is declared.
7. **No conformance result exists.** No suite is pinned, no harness is built, and the language
   specification edition is unpinned. The corpus is this component's own and is not a conformance
   claim.
8. **The core contract is implemented but not accepted**, which blocks JS-2 onward and not this
   milestone.
9. **Nothing is fuzzed.** JS-9 owns the four untrusted-input surfaces; the corpus is a fixed set
   of hand-written entries and finds only what it was written to find.
10. **Nothing is reviewed.** Every relevant unit in this component is `HUMAN_PENDING`.
