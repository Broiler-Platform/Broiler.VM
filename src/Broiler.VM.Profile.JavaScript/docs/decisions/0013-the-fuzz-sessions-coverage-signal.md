# JSD-0013 - What a fuzz session observes, and why it is not edge coverage

**Status:** Accepted for JS-9.

**Date:** 2026-09-01

**Owner:** profile security owner. **Co-signer:** the fuzz-corpus owner. **Both roles are held by
one person** and this record does not claim the co-signature is independent.

**Milestone:** JS-9.

## The clause, and the state it was in

Roadmap [section 7](../roadmap.md#7-the-bytecode-format-and-the-verifier) asks for
**coverage-guided** fuzzing over four untrusted-input surfaces. Two of the four exist at this
milestone — the verifier, and the executor over verified-but-adversarial artifacts — and the
sessions this component retains covered those two by **seeded mutation**: every mutant drawn from
the fixed retained corpus, with nothing a mutant reached feeding back into what the next one was
drawn from. The sessions and their bundle called themselves coverage-guided until 2026-09-01, when
[JSC-38](../roadmap.corrections.md#jsc-38) corrected both.

So the clause needed a feedback loop, and a feedback loop needs a signal. **This record decides
what the signal is**, because the obvious reading of the section's adjective is one this component
cannot honestly deliver, and the difference is worth writing down rather than leaving to whoever
reads a session log next.

## Decision: the signal is the answer this profile publishes about a mutant

One string per observation, and a mutant whose string no seed artifact produces is kept as a
further seed:

| The mutant | Its signal |
|---|---|
| was refused as an invalid artifact | the core reason and this profile's diagnostic code — and a code **names the site that refused**, which is what makes this a statement about where the mutant reached |
| was refused as a resource exhaustion | the budget dimension, which an exhaustion carries **instead of** a code |
| verified and ran | what the executor did with it: the completion, the instantiation refusal, or the fault kind |
| threw out of the verifier | the exception's type — a finding rather than a signal, and never folded into its neighbours |

The seed pool opens as the retained corpus and is **primed** with everything that corpus reaches
under every host a session uses, so *new* means new against the corpus rather than new since the
last iteration. Priming costs one observation per entry per host, about two per cent of a full
session.

**Why the code is the right granularity to reach for.** This profile publishes forty diagnostic
codes, one per refusal site family, and
[rule N6](../roadmap.status.md#2-current-milestone-status) already holds every one of them to the
emission sites that carry it. A signal keyed on the code is therefore keyed on something the
component already maintains as a partition of its own refusal surface — not on an incidental
string.

## Decision: this is not edge coverage, and the sessions say so in those words

Nothing here observes a branch, a basic block or a line. **Two mutants that take different paths
to the same published answer are one signal**, so a defect on a path that answers like its
neighbour is invisible to the guidance. The signal is as fine as this profile's diagnostic
vocabulary and no finer.

Every session prints that bound in its guidance line, the ledger's JS-9 row carries it, and
[JSC-42](../roadmap.corrections.md#jsc-42) records the plan's replaced adjective. A session may be
described as **answer-guided**; describing one as coverage-guided is the error JSC-38 already
corrected once.

## Rejected: instrumenting the profile so a mutant's reached edges are the signal

This is what the section's adjective means in the tools it was borrowed from, and it was
considered first.

**Why not.** A mutant's edges can only be observed by instrumenting the code that runs it, and the
code that runs it is a **composition root** — rule A11 forbids a test project to reference this
profile's assemblies, so the sessions live in the root and nowhere else
([JSC-34](../roadmap.corrections.md#jsc-34) drew that consequence for the mutator and the soak).
An instrumented session therefore means one of two things, and both are refused for the same
reason:

- **An IL rewriter or coverage host reachable from the execution-only root.** The root's whole
  value is that its published closure is a closure a reader believes: six managed assemblies, no
  reflection, nothing discovered. A coverage host in that closure would be a dependency this
  component does not have, in the one image whose dependency list is evidence.
- **Instrumentation compiled into the profile assembly itself**, behind a build flag. That makes
  the fuzzed artefact a different artefact from the shipped one, which is the failure mode the
  whole evidence discipline exists to prevent: a session over instrumented code is evidence about
  instrumented code.

**What that costs, stated rather than glossed.** Real coverage guidance finds defects this will
not. A path that answers exactly like its neighbour — two arms producing one code, a loop bound
that differs only in how many times it runs — is invisible here and would not be to an edge-guided
fuzzer. This is a smaller mechanism than the section asked for, and the honest form of delivering
it is to say which one it is.

**What would falsify the rejection.** A coverage mechanism that needs no reference in the published
closure and no change to the shipped assembly — a runtime-provided profiler attached from outside
the process, or a separate never-advertised root that carries the instrumentation the way the
conformance harness will carry the suite ([JSC-40](../roadmap.corrections.md#jsc-40)). Neither
exists in this checkout; if one is built, this decision is re-examined and this paragraph is what
it is re-examined against.

## Decision: growth is reported and never judged

How much a session grows its seed set is a fact about **the corpus** as much as about the mutator:
a corpus that already reaches every answer the mutator can reach makes an honest session keep
nothing. A rule that failed a session for keeping nothing would fail harder the better the corpus
got, and this component's corpus grows at every format-growing milestone.

So what a session judges about itself is the **mechanism**, in two clauses that hold whatever the
corpus contains: every mutant it drew was offered to the pool — a session that offered fewer
mutants than it drew exits non-zero and says it may not be read as guided — and the pool keeps a
new answer while refusing a repeat, which the composition asserts as a named check of its own so a
publish that never fuzzes still carries it. Two negative controls break exactly those two clauses.

## Decision: one declining host per dimension, rotated

A session's declining host used to tighten four dimensions at once. That could reach the arms of
only four of the seven a verification can exhaust, and — worse — could not attribute what it did
reach: an artifact refused under a vector that tightened four things says nothing about which of
the four the verifier answered on. There are seven vectors now, each tightening one dimension, and
an iteration's host rotates by iteration index so the rotation draws nothing from the mutator's
stream. A session remains a total function of its seed and its seed corpus.

**One of the seven needed a meter arm that did not exist.** The root's recording meter never
refused a poll, so the verifier's poll refusal — the branch that decides between a cancelled
caller and a spent wall clock — was reachable from no session and no check. It refuses where the
stated wall-clock ceiling is zero, which is a fact about the host and not a clock reading: a meter
that compared elapsed time would make a session's answer depend on how busy the machine was.

## What this record does not decide

- **Nothing about the two surfaces that do not exist.** The source tokenizer and parser and the
  regular-expression matcher are JS-3b's and JS-6's, and a session may not be read as covering
  them.
- **Nothing about session budgets.** The iteration counts and the seeds are stated so a run is
  reproducible, not because any of them is a number something justifies. JS-9's gate still owes a
  budget with a stated floor.
- **Nothing is accepted.** The ledger is the authority for what an implemented mechanism proves,
  and closing JS-9's guidance clause needs a retained bundle that nobody has collected.
- **Nothing is reviewed.** No human has read the loop, the signal or this record.
