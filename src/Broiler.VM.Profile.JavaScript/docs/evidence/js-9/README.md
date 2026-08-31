# Evidence bundle JS-9-001

**Milestone:** JS-9 — adversarial input, agents, soak. **What this bundle covers:** roadmap
[section 7](../../roadmap.md#7-the-bytecode-format-and-the-verifier)'s **second discipline**,
coverage-guided fuzzing, over **two of its four surfaces**.

**Verdict this bundle supports:** JS-9 is **In progress**, not accepted. Two of the four surfaces
are not fuzzed because they do not exist; there is no soak host, no aggregate-budget exercise and
no agents; and **nothing here has been reviewed**.

Produced by `eng/collect-js-evidence.py`. Every file beside this one is its output.

**No result from any other component is evidence here.** The core has a fuzz host of its own shape
and none of its sessions, seeds or findings appears in this bundle.

## Identity

| Field | Value |
|---|---|
| Bundle | `JS-9-001` |
| Milestone | JS-9 |
| Surfaces fuzzed | The verifier; the executor over verified-but-adversarial artifacts |
| Surfaces not fuzzed | The source tokenizer and parser; the regular-expression matcher — **neither exists** |
| Seed corpus | The 60 entries the `js-1` manifest names |
| Sessions | Seeds 1–4, 25,000 iterations each |
| Claimed RID | `win-x64`, and nothing else |
| Owner | profile contract owner |
| Reviewer | **none** |

## The sessions

A session is a **total function of its seed and its seed corpus**. There is no wall-clock budget
and no thread count: either would make the same session behave differently on two machines, which
is the nondeterministic failure class this component's gates forbid. A finding is reproduced by
naming the seed and the iteration.

| Seed | Iterations | Result | Reached the executor |
|---|---|---|---|
| 1 | 25,000 | no counterexample | 959 mutants verified, instantiated and invoked |
| 2 | 25,000 | no counterexample | 994 |
| 3 | 25,000 | no counterexample | 1,080 |
| 4 | 25,000 | no counterexample | 990 |

**Every executor fault carried this profile's own typed payload** — `ReferenceError`, from a
mutated entry-point name that nothing is bound to, which is the language faulting rather than
something escaping.

**Two non-vacuity exits, and they are the reason a clean session means anything.** A session whose
mutants all answered the same way exercised one path, and reporting it as success would let a
broken mutator read as twenty-five thousand clean iterations. A session in which nothing verified
never reached the executor, so half of what it claims to cover was untouched. Each exits non-zero.

## What the tool found about itself

Three things, each by being run against a deliberate defect rather than trusted. They are recorded
here because a fuzzer that finds nothing is worth exactly what the demonstration that it would
find something is worth.

**The undirected mutations could not reach the defect.** With the verifier's constant-index check
removed, an artifact verifies and then indexes past the pool — and **25,000 undirected iterations
did not find it**. A mutant must both still verify, which about three in a hundred do, *and* carry
a non-zero index in the two operand bytes of a specific opcode; a random byte poke satisfies both
about never. An operand-targeting mutation was written because of that, and the session finds the
class in under two hundred iterations now.

**The invariant that catches it did not exist.** A guest program is *allowed* to fault: a mutated
entry-point name is a `ReferenceError` and mutants produce them constantly, so `ProfileFault` alone
says nothing. What is not allowed is a fault **the profile did not author** — the core catching an
exception out of the executor and reporting it — because that is the verifier having admitted
something the executor could not run. The two are told apart by whether this profile's typed
payload came back with the result, and that distinction is now the executor-surface invariant.

**The fuzzer polluted its own seed corpus.** Findings were written into the corpus directory and
the seed set was a glob over it, so the session after a finding picked the finding up as a
sixty-first seed and answered differently for the same seed — which falsifies the determinism the
whole design rests on. Found by running the same session twice. Seeds come from the **manifest**
now, and a finding lands beside the corpus rather than in it.

## Procedure and results

| Step | Log | Result |
|---|---|---|
| Release build of the whole solution | `build.log` | Succeeded, **0 warnings**, 0 errors |
| Whole test suite | `suite.log` | 207 contract tests and 138 architecture tests passed |
| Assurance gate mode | `assurance-gate.log` | Passed |
| Assurance release mode | `assurance-release.log` | **Refused, as it must** |
| Publish and run, both roots × three modes | `publish-and-run.log` | **6 publishes, 6 runs, all exit 0** |
| Fuzz sessions | `fuzz.log` | **4 sessions, 100,000 iterations, 0 findings** |
| Negative controls, suite-judged | `negative-controls.log` | 16 injected, 16 caught, 0 skipped |
| Negative controls, corpus-judged | `corpus-controls.log` | 8 injected, 8 caught, 0 skipped |
| Negative controls, **fuzz-judged** | `fuzz-controls.log` | **1 injected, 1 caught, 0 skipped** |

### The fuzz control

`the-constant-index-is-admitted-unchecked` removes the verifier's check that a `LoadConstant`
operand addresses a pool entry. Injected, the session reports:

> FINDING at iteration 116 of seed 1 — a verified artifact faulted with no typed payload, so
> something threw out of the executor and the core reported it

and exits 1; reverted, it exits 0. **This defect is also caught by the retained corpus**, by
`a-constant-index-past-the-pool` — that is what the corpus is for. What the control shows is that
a session reaches the same class **from bytes nobody wrote**, which is what the corpus structurally
cannot do.

**The control reverts its finding as well as its source**, and that was not true when this bundle
was first collected. The injected session retained a counterexample, and a counterexample left on
disk for a defect that no longer exists reads as the one thing a fuzz finding must never read as:
an open finding nobody has closed.

## Exclusions — what this bundle does not show

1. **Two of the four surfaces are not fuzzed, because they do not exist.** The source tokenizer
   and parser arrive at JS-3b; the regular-expression matcher at JS-6, and its acquisition is an
   open external dependency. A session may not be read as covering them.
2. **Nothing else of JS-9 is started.** No soak host, no aggregate-budget exercise, no agents. The
   milestone is `In progress` for one discipline and no more.
3. **Four sessions is not a session budget.** The seeds and iteration counts are stated so a run is
   reproducible, not because a hundred thousand iterations is a number anything justifies. JS-9
   owns choosing one.
4. **A clean session is a statement about what the mutator reached.** It found nothing; the three
   findings above are about the tool, and the first of them is precisely a case the mutator could
   not reach until it was changed. There will be others.
5. **Minimization is byte-level and mostly blanks rather than shortens.** For a length-framed
   format every deletion moves a section boundary and the finding disappears with it, so the
   minimizer keeps the length and empties what the finding does not depend on. The reproduction is
   the seed and the iteration; the retained bytes are a reading aid.
6. **One RID, one machine.** `win-x64`.
7. **Nothing is reviewed.** Every relevant unit in this component is `HUMAN_PENDING`.
