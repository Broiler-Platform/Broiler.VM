<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Evidence bundle UBC-4-001

**Milestone:** UBC-4 of [the universal bytecode programme roadmap](../../universal-bytecode.roadmap.md) -
the WebAssembly family. This bundle is work package UBC-4.1: the base run, retained beside the rule it
will be judged by.
**Collected:** 2026-09-25, at commit `72e7491`, the base commit, from a clean tree, by `collect.py` in
this directory. No code of the translator, the family or the store exists at that commit.
**Core contract version:** 1, unchanged.
**Status:** the base of a comparison, not a verdict. The comparison is bundle `ubc-4-002`'s.

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle | UBC-4-001 |
| Milestone | UBC-4, work package UBC-4.1 |
| Base commit | `72e7491` |
| Rule | [`decision-rule.md`](decision-rule.md), first committed in `6486add` and revised in `3042855`, both before any code of this milestone |
| Population judged | B only: every retained entry of `src/tests/wasm/corpus`, every execution check and every differential check of `Broiler.VM.Composition.WebAssembly.Harness` |
| Owner | the WebAssembly profile owner |
| Reviewer | none |
| Evidence class | conformance parity, the base half; no measurement of any kind |

## 2. What this bundle holds

| File | What it is |
|---|---|
| `decision-rule.md` | The predeclared rule: the populations, the comparison, the two admitted classes and the one decision. Unchanged by this bundle |
| `base-run.log` | The harness root run once at the base commit with `--verbose --corpus src/tests/wasm/corpus`: every member of population B on one line as the root prints it, and the root's exit code |
| `base-population.txt` | The same lines reduced to one row per member - lane, name, verdict, answer - sorted. Bundle `ubc-4-002` joins its run after against these rows by lane and name |
| `corpus-integrity.log` | `eng/wasm-corpus-integrity.py --hashes-only` over the retained corpus: the manifest's shape and every file's hash |
| `collect.py` | What wrote the three files above |
| `manifest.json` | The base commit, every source the run read by its git blob at that commit, the corpus by hash, and the retained files by hash |

## 3. What the base run shows

Every retained corpus entry answers what its manifest records. Every differential check agrees with its
oracle. Every execution check passes except the float comparisons, and every float comparison fails the
same way: the invocation ends in `ProfileFault/ProfileContractViolation` with no results payload. That is
the interpreter's defect the rule's class (f) names - the comparison bytes are routed to the integer
arm, which has no case for them - and the root's exit code is 1 because of it.

**The float comparisons were added to the harness at the base commit, before this run**, because the
harness held none and class (f) would otherwise have had no member in population B. They exercise all
twelve comparison instructions over ordered, equal, unordered and signed-zero operands, each expecting
the specification's value. Bundle `ubc-4-002` judges each of them under class (f): its answer after must
be that value.

The roadmap's work package UBC-4.1 speaks of "the twelve float-comparison assertions". The rule's class
is defined by instruction - the twelve comparison instructions - and not by a count of assertions, and
this bundle reads it that way.

## 4. What is NOT here, named rather than left as an absence

- **No population A.** The specification's test suite is not in the repository, and no script reader
  for its text format exists. On 2026-09-25 the WebAssembly profile owner decided that UBC-4 is carried
  out on population B alone. Under the rule, population A's precondition is unmet, the rule's verdict
  cannot be MET, and exit-gate clause 5 cannot be met; the programme ledger's UBC-4 row says so and names
  the WebAssembly profile owner as the holder of the suite's pin and its reader. Bundle `ubc-4-002` may
  retain population B's comparison and the negative control as partial evidence named as such.
- **No mutation pass over the corpus.** `eng/wasm-corpus-integrity.py`'s mutation pass needs a replay
  that exits 0 on the intact corpus, and at the base the root exits 1 because of the defect above. The
  hashes-only pass is retained; the replay inside the root re-hashes every entry as well.
- **No review.** The author and the owner are one person.

## 9. Exclusions

**This section defines no exclusion identifier, and that is deliberate rather than an omission.**
