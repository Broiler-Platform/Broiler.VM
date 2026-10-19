# Milestone SJL-1 — The shared conformance vectors

[← Roadmap](../roadmap.md) · [Ledger](../status.md) · [Area design](../areas/conformance-vectors.md)

- **Objective.** One vector corpus lives in its home and runs against both engines' current
  implementations, beginning with number conversion. It has a schema, recorded provenance, and a
  classification of every place where the specification lets engines differ. Every difference
  between a vector and an engine is classified.
- **Owner roles.** The Broiler.JS maintainer, for the home and Broiler.JS's runner. The JavaScript
  profile owner, for Broiler.VM's runner and pin. One person.
- **Repositories.** Broiler.JS (the home, under [SJL-D-1](../decisions.md#sjl-d-1)) and
  Broiler.VM.
- **Waits on.** SJL-0, including its decision-rule file for this milestone.

**Why this comes before any shared code.** The feasibility report: "The algorithms are not
identical today, so extraction should start with a common test corpus." It also lists "Test vectors
and conformance fixtures" as "Useful immediately". A corpus run against both engines **before**
anything moves tells each engine where it is wrong today. That separates a correction, which a
cut-over may make, from a regression, which it may not. **Broiler.VM consumes the corpus as a
pinned archive**, as it already consumes test262, so no package admission is needed and this
milestone does not wait on SJL-2.

---

## Work packages

### SJL-1.1 — The schema

A versioned, line-oriented format, defined in [areas/conformance-vectors.md](../areas/conformance-vectors.md).
Each vector carries:

- an identifier;
- an area;
- an operation, named after the specification's abstract operation or built-in, for example
  `Number::toString` or `StringToNumber`;
- typed inputs: doubles as IEEE 754 bit patterns, strings as UTF-16 code units with lone surrogates
  allowed, BigInts as text;
- the expected outcome: a value, a status, or an error kind;
- its provenance: a specification clause, a test262 path, an independent reference computation, or a
  generator with its seed;
- its classification: *normative*, *implementation-defined* with per-engine expectations, or
  *known engine divergence* with a reason and a link to that engine's plan;
- the corpus revision it was added in;
- **a script form**: the JavaScript expression that evaluates the operation through an engine.

A validator checks the schema. **It also refuses any vector whose expected value has no source
independent of both engines.**

*Done when* the schema document and the validator exist, and the validator refuses a malformed
negative-control file.

### SJL-1.2 — The home and how each engine receives it

- **Broiler.JS** holds the corpus, the content of the test-only `Broiler.ECMAScript.Conformance`
  package ([SJL-D-2](../decisions.md#sjl-d-2)), under a directory with the working name
  `conformance/vectors/`. It has a README, a provenance note, and the licence notice of every source
  a vector derives from, test262's included.
- **Broiler.VM** receives the corpus as an archive in a pin directory such as
  `src/tests/vectors/pins/`, with a `.pin` file recording its source commit, archive hash and
  content hash, in the shape of `src/tests/conformance/pins/test262.pin`. Rule N13's suite-directory
  list gains the directory, so that no product project names it.

*Done when* the corpus exists in its home, and Broiler.VM holds a pinned archive of a named corpus
revision.

### SJL-1.3 — The runners

| Runner | Where | How |
|---|---|---|
| Library level | The home's test project | Calls a library's API directly. It has nothing to call until SJL-3, so here it is built and run against an empty library with the expected "not implemented" outcome. |
| Engine level, Broiler.JS | A Broiler.JS test project | Renders each vector's script form, evaluates it in a fresh realm, and compares the printed result |
| Engine level, Broiler.VM | A test-only harness root, the conformance composition or a sibling under rule N13, reading the pinned archive | The same rendering through the JavaScript profile. The comparison is exact and normalises only line endings. |

The two engine-level runners share **one rendering rule**, documented with the schema, so a
difference between them is never a difference in rendering.

*Done when* all three runners exist, and a negative control is caught by both engine-level
runners. The control mutates one vector's expected value.

### SJL-1.4 — The number-conversion vectors

The seed set for SJL-3. It covers:

- **`Number::toString` at radix 10:**
  - zeros of both signs, `NaN`, the infinities;
  - powers of two and of ten across the whole exponent range;
  - every subnormal boundary;
  - the neighbours of `1e21` and `1e-7`;
  - a seeded random population checked for shortest round-trip.
- **Radix conversion:** integer and fraction parts, every radix from 2 to 36, values beyond 2^53.
- **`toFixed`, `toExponential` and `toPrecision`:** argument range edges and exact ties.
- **`StringToNumber`:**
  - the whitespace set, including U+FEFF, which is whitespace, and U+0085, which is not;
  - every prefix;
  - signed and unsigned `Infinity`;
  - separators refused;
  - values that need correct rounding beyond 2^53.
- **`parseInt` and `parseFloat`:** every radix edge and correctly rounded long inputs.
- **Numeric literal values:** legacy octal, `08` and `09`, separators, long radix literals beyond
  2^53, BigInt literals including `0n`, `01n` refused, and the width limit as a parameter.

**Sources:**

- the specification's own algorithms, computed by an independent reference implementation in the
  generator using exact big-integer arithmetic;
- test262's directories for these built-ins and for numeric literals;
- Broiler.VM's differential probes on number formatting;
- Broiler.JS's round-trip tests.

**Expected values are never taken from either engine.**

*Done when* the vectors exist with provenance, and the validator passes them.

### SJL-1.5 — The first run against both engines

Both engine-level runners run the corpus against each engine's **current** implementation, under
the decision rule committed at SJL-0. Each difference is classified as one of:

- **engine defect**, naming the engine;
- **vector defect**, in which case the vector is fixed and the run repeated;
- **implementation-defined**, in which case the classification moves into the vector.

[areas/numbers.md](../areas/numbers.md) lists differences the survey already expects to see, in
both engines.

*Done when* both runs are retained and every difference carries one of the three classes. **Any
unclassified difference leaves the milestone `In progress`.**

### SJL-1.6 — The process for later areas

The corpus's README documents how a vector is added, reviewed and retired. In particular:

- a vector that an engine fails is never deleted to make the engine pass;
- a classification change is a dated corpus entry;
- every area milestone (SJL-3 to SJL-11, SJL-13 to SJL-15, and SJL-17) adds its vectors **before** its
  shared code lands. *(Revised 2026-10-19, at SJL-12.1, by
  [JSD-0080](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md)
  decision D1, as roadmap section 5's range is: SJL-12 is preparation, not an area milestone, and writes no
  shared code, so it owes no vectors under this item. The parenthesis read "(SJL-3 to SJL-15, and SJL-17)".)*

*Done when* the README states the process.

---

## Deliverables

- The schema and its validator.
- The corpus with the number vectors.
- The library-level runner and both engine-level runners.
- Broiler.VM's pin and the N13 revision.
- Both first runs, classified.
- The corpus process.

---

## Exit gate

**Broiler.JS (home) clauses**

1. The schema and validator exist. The validator refuses the malformed negative-control file and
   any vector without an independent source.
2. The number vectors exist with provenance on every vector.
3. Broiler.JS's engine-level runner has run the corpus. Its results are retained with every
   difference classified, under the predeclared rule.

**Broiler.VM clauses**

4. A named corpus revision is pinned by hash and read only by a test-only root. Rule N13 passes
   with the new directory and fails on the witness that names it from a product project.
5. Broiler.VM's engine-level runner has run the corpus. Its results are retained with every
   difference classified, under the predeclared rule.
6. The negative control, a mutated expected value, is caught by Broiler.VM's runner and by
   Broiler.JS's runner.

---

## Evidence bundles

- **Broiler.JS `docs/evidence/sjl-1-001/`:**
  - the corpus revision and its hash;
  - the validator logs, including the negative controls;
  - the engine-level run and the classification table;
  - `decision-rule.md`, committed at SJL-0.
- **Broiler.VM `docs/evidence/sjl-1-001/`:**
  - the pin;
  - the engine-level run and the classification table;
  - the N13 witness run;
  - `decision-rule.md`.

---

## Does not do

- **It ships no shared code** and admits no package.
- **It fixes nothing in either engine.** A defect it finds goes to that engine's own plans, or
  becomes a correction an area's cut-over makes and lists.
- **It does not compare the engines with each other.** Each engine is compared with the vectors.
