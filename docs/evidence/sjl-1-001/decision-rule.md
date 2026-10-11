<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-1-001's decision rule: every difference between a vector and Broiler.VM is classified

**Written:** 2026-10-11, at milestone SJL-0 (work package SJL-0.9), before the vector schema, the
validator, the corpus, Broiler.VM's pin or Broiler.VM's engine-level runner exists. **Milestone
judged:** SJL-1 of [the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-1-shared-conformance-vectors.md),
Broiler.VM clauses 5 and 6. **Owner:** the JavaScript profile owner. **Reviewer:** none. The owner,
the Broiler.JS maintainer who writes the corpus, and every other role this rule names are one person
(EX-30), and this rule does not claim that anyone independent has read it.

**What this file is.** The one predeclared decision of Broiler.VM's bundle `sjl-1-001`, in the
template of [the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules),
section 4 (T3, read for classification). The Broiler.JS repository commits its own rule at the same
path, judging its own engine; neither rule reads the other's runs. It holds no figure.

```rule
evidence class: conformance (vector classification), not a measurement
population: every vector of the corpus revision Broiler.VM's pin names (src/tests/vectors/pins/, in the shape of src/tests/conformance/pins/test262.pin), whatever its area; the negative-control file the validator is given is not in the population
configuration: Broiler.VM's engine-level runner, a test-only composition root reading the pinned archive and no other copy (rule N13), evaluating each vector's script field in a fresh realm of the JavaScript profile under the wide manifest in the bytecode form, with the conformance root's per-variant allowances, and rendering each result under the corpus's one rendering rule; the archive is checked against its pin's archive and content hashes before any vector runs
comparison: per vector, the rendered result against the vector's expected outcome: for a normative vector, its out field; for an impl-defined vector, the per-engine expectation it names for Broiler.VM; for a divergence vector, its out field, the divergence being the classification of the difference
admitted difference classes:
  (d) the vector is classed divergence, names engine broiler-vm, and gives a reason linking a plan of this profile; it is listed by identifier
  (i) the vector is classed impl-defined and Broiler.VM's result equals the per-engine expectation it names; this is a match, listed by identifier as implementation-defined
every other difference is unclassified, including a difference on a normative vector, an impl-defined vector naming no expectation for Broiler.VM, a divergence vector naming the other engine only, and a runner refusal or harness fault on any vector
provenance: the validator has accepted the pinned revision; no vector in it takes its expected value from either engine (provenance engine-observed only is refused); a vector Broiler.VM fails is never deleted or reclassified to make it pass, except by a dated corpus entry in a later revision
negative control: one vector's expected value is mutated in a copy of the archive; the runner, given that copy, reports the vector as a difference and does not classify it; the copy is then removed and the run on the pinned archive is retained
MET if and only if the archive matches its pin, the validator accepted the revision and refused its negative-control file, every vector of the population has a result, every difference is in class (d) and is listed, every match in class (i) is listed, no vector's expected value is engine-observed only, and the negative control was reported unclassified.
NOT MET otherwise.
```

**How it is read.** The bundle's README reads the runner's retained output against the block and
states the decision. The runner computes nothing beyond the per-vector comparison: it prints each
vector's identifier, its class, the expected outcome and Broiler.VM's rendered result, so the
classification can be checked line by line. A run that could not read the archive, or that stopped
before the last vector, is void and is not judged.

**Revisions.** A rule revised after any run it judges has been seen is a new dated file quoting this
one whole and saying it was written afterwards. Its decision is then weaker evidence, as the core's
bundle `ubc-4-006` records for its own.

## What it does not decide

- **Whether Broiler.VM is correct.** A divergence is a classified difference, not an excuse: it names
  the plan that answers it, and a cut-over that corrects it lists it as a correction.
- **Anything about Broiler.JS.** Broiler.JS's runner is judged by its own rule in its own repository.
  The two engines are compared with the vectors, never with each other.
- **Anything about test262.** Vector agreement meets no test262 clause; the programme's update rule 3
  says so.
- **Any performance.** No timing is taken by the runner, and none is read.
