# Validation: evidence layers, comparison rules and bundles

**Date:** 2026-10-11. **Status:** plan. This document describes how the programme's milestones are
judged. The milestone files hold the gates, and this document holds the methods behind them.

---

## 1. Principles

1. **The specification and Test262 are the oracles.** Shared code means shared bugs. Agreement
   between the two engines, through the differential harness or Broiler.JSeal's suite, is a
   cross-check and never an oracle. Broiler.VM's JavaScript profile already states this: "a
   differential against another implementation is a cross-check, never the oracle".
2. **Each repository is judged on its own evidence.** A Broiler.VM clause is met by a bundle
   Broiler.VM collected, running Broiler.VM against the shared package version Broiler.VM pins.
   The same holds for Broiler.JS and the component repositories. **No evidence transfers.**
3. **Rules come before runs.** Every comparison is judged by a `decision-rule.md` committed before
   the code or run it judges, holding no figure. A rule revised after its runs were seen is a new
   dated file that quotes the old one whole and says it was written afterwards. Its decision is
   reported as weaker evidence, which is the discipline of Broiler.VM's bundle `ubc-4-006`.
4. **Base before change.** Every comparison has a base taken at a named commit before the engine
   changes: SJL-0 for the pilot, and each area's first work package for later areas.
5. **Figures live only in bundles.** No plan, ledger, decision or support document quotes a count,
   timing, ratio or score.
6. **No cross-engine performance comparison.** Broiler.VM's JavaScript profile forbids "comparison
   against any other engine or component, in either direction", and its release gate 10 enforces
   this. Every measurement is **one engine's candidate against that engine's control**.
7. **Negative controls.** Every check that could pass vacuously is shown failing on an injected
   defect, and passing again on revert.

---

## 2. Evidence layers

| Layer | What it shows | Where it runs | Tools today / to build |
|---|---|---|---|
| **L1. Vectors** | Each operation's answers against independently sourced expectations | Library level in the home, engine level in both engines | Built at SJL-1 ([areas/conformance-vectors.md](areas/conformance-vectors.md)) |
| **L2. Reference arms** | The shared implementation against **both** engines' pre-cut-over implementations over a declared input population; every difference classified | The home's test project | Built per area (section 3) |
| **L3. Library unit and property tests** | Contracts: metering, limits, statuses, thread safety, determinism | The home | Per library |
| **L4. Unicode, CLDR and IANA test files** | The data algorithms against their standards' own tests | The data owner | `NormalizationTest`, the break tests, `CaseFolding`, the collation tests, `zic` comparisons |
| **L5. Test262** | Each engine's conformance at the common pin `ccaac100…`, base against after | Each engine with its own runner | Broiler.VM: `eng/run-test262.py`, `eng/compare-test262-forms.py` (`--same-form` compares two builds of one form). Broiler.JS: `scripts/compliance/run_test262.py` plus a per-path comparison script built at SJL-0. **The two runners' totals are never compared with each other.** They differ in variants, negative tests, admitted proposals, host hooks and allowances. |
| **L6. Engine suites** | Each engine's own tests | Each engine | Broiler.VM: architecture and contract tests, composition checks modes, corpora, CLI acceptance. Broiler.JS: its test projects. |
| **L7. Reference datasets** | Intl and Temporal behaviour against independent reference answers | Both engines | Broiler.VM's `src/tests/cldr/<area>/` and `src/tests/temporal/` programs, which use no host functions |
| **L8. Cross-checks** | Engine agreement on whole programs and on the embedding surface | Broiler.VM's differential harness; Broiler.JSeal's suites | `eng/run-differential.py` with `--against-kind broiler-js`. It compares whole-program output and is run manually; its declarations are pinned to older Broiler.JS commits, so it is refreshed at SJL-0. Broiler.JSeal's `eng/test-package-consumer.ps1` takes candidate feeds. |
| **L9. Fuzzing** | No crash, hang or unbounded work on hostile input | Broiler.VM's fuzz host and source fuzz seeds; the home's fuzz targets | Existing in Broiler.VM; to build in the home |
| **L10. Publish and run** | Native AOT, trimming and JIT closures with the shared assemblies | Broiler.VM: every JavaScript root, three modes, closure retained, with ILC's mstat file for the Native AOT mode. The home: the Native AOT probe, with Broiler.VM's root settings. Broiler.JS: its Native AOT sample for the portable interpreter, which no shared library reaches (SJL-14). | Broiler.VM's lane and collectors; the home's probe from SJL-2 |
| **L11. Packages** | Pack, nuspec dependencies, pristine restore, rollback, package validation | Every repository | Broiler.VM: `eng/pack.ps1`, `eng/verify-feed.ps1`, `eng/collect-js-packages.py`, rules N37 and A14. Broiler.JS: `eng/pack.ps1`, `eng/verify-feed.ps1`, `scripts/packaging/test_pristine_consumer.py` (repaired at SJL-2). |
| **L12. API baselines** | Public surfaces, compared in both directions | Every repository | Broiler.VM: rules M1, N10 and U9. Home: SL6. Broiler.JS: its first baseline at SJL-0 and SJL-2. |
| **L13. Determinism** | Identical outputs across runs and threads; byte-identical Broiler.VM artifacts | The home (SL9); Broiler.VM | Two runs and two threads per corpus |
| **L14. Resource accounting** | Broiler.VM's fuel verdicts and exhaustion classes unchanged | Broiler.VM | The fuel-parity twins (`src/tests/forms/wide/*.small.js`); the exhaustion rows in test262 verdict files |
| **L15. Measurement** | No unacceptable slowdown, per engine | Each engine | Broiler.VM: `eng/measure-js-baselines.py` and harnesses written under a rule. Broiler.JS: its BenchmarkDotNet project and `scripts/performance`. |

---

## 3. Reference arms

A reference arm is a **test-only copy** of an engine's implementation as it stood before the
cut-over, kept in the home's test project. For each area:

1. **Copy.** Both engines' implementations of the area are copied into the test project at the base
   commits, with their provenance. Broiler.VM's are internal, so they are copied, not referenced.
   Broiler.VM's roadmap allows copying ("Copying is allowed"), and both repositories are Apache-2.0.
2. **Declare the population.** It is declared in the area's rule file: every class of input, and the
   seeds of the random parts.
3. **Run.** The shared implementation and both arms run over the whole population, and every
   difference is recorded.
4. **Classify each difference:**
   - (a) one arm is wrong: a **correction**, listed with the engine it corrects;
   - (b) the specification leaves it to the implementation: an **implementation-defined**
     difference, moved into a vector;
   - (c) the shared implementation is wrong: a **defect**, fixed before the cut-over lands.

   **An unclassified difference leaves the area unmet.**
5. **Retire.** The arms are removed at SJL-16, or kept as a frozen regression population by decision.

**Reference arms run under JIT only**, in the home's test project. They copy code that Native AOT may
not accept, such as Broiler.JS's reflection-based JSON deserialization, and code that depends on the
host's globalization data. They are never part of the Native AOT probe.

---

## 4. Predeclared comparison rules

Each template below becomes a `decision-rule.md` with a dated header:

- Written;
- Milestone judged;
- Owner;
- Reviewer: none, with the EX-30 non-independence stated;
- a fenced `rule` block;
- "What this file is";
- "How it is read";
- "What it does not decide".

**None holds a figure.** The difference classes they admit come from this vocabulary:

| Class | Meaning |
|---|---|
| **C1. Correction** | The verdict improves because the shared implementation follows the specification where the engine did not. Each one is listed by name, with its vector or test path. |
| **C2. Charge model by decision** | An exhaustion verdict moves because a dated decision changed the charging units. Each one is listed. |
| **C3. Implementation-defined** | The verdict changes in an area the specification leaves to the implementation, and the area's vectors classify it. |
| **C4. Harness and environment** | Broiler.VM's existing admitted classes for forms comparison: code-size refusal, guest-load fuel, wall clock, under `compare-test262-forms.py`'s definitions |
| **C5. Data policy by decision** | An answer changes because a data decision moved a pin or a policy (SJL-7 to SJL-9, SJL-17). Each one is listed. |

**Every other difference is a regression**, and the rule's decision is NOT MET.

### T1. Broiler.VM test262 parity

**Evidence class:** conformance.

**Population:** the whole pinned suite, under the wide manifest, in the bytecode form and the native
form.

**Configuration:** `eng/run-test262.py` at the pin, with the allowances the release-candidate
collector uses.

**Comparison:** each variant's verdict at the base commit against the same variant after the
change, through `eng/compare-test262-forms.py --same-form`.

**MET if, and only if**, all of these hold:

- both runs are retainable;
- no variant passing at base fails, is exhausted or is unsupported after, except under C2;
- every changed verdict falls in C1 to C5 and is listed;
- the native form's comparison meets the same conditions.

**NOT MET** otherwise.

### T2. Broiler.JS test262 parity

**Population:** the canonical configuration of `run_test262.py` at the pin.

**Comparison:** per path, base against after, by the comparison script SJL-0 builds.

**MET if, and only if**, all of these hold:

- no path passing at base fails or times out after;
- every changed verdict falls in C1, C3 or C5 and is listed.

### T3. Vector agreement

**Population:** the area's vectors at a named corpus revision.

**MET if, and only if**, every vector passes at library level, and at engine level in the engine
being cut over. Implementation-defined vectors are judged against their per-engine expectation.

### T4. Reference arms

**Population:** as declared for the area (section 3).

**MET if, and only if**, every difference is classified, and no class (c) difference remains.

### T5. Per-engine measurement

**Evidence class:** measurement, in Broiler.VM's evidence vocabulary.

**Arms:** the engine at its base commit against the engine after the cut-over. **Never** one engine
against the other.

**Configuration:** the engine's own harness; interleaved arms; an A/A control; repetitions and a
tolerance, both stated in the rule before any run.

**MET if, and only if**, the after arm is within the stated tolerance of the base arm for every
named workload.

**The rule says what it does not decide:** a speed claim of any kind.

### T6. Artifact identity (Broiler.VM)

**Population:** the parse corpus (SJL-12.4) and the retained source corpus.

**MET if, and only if**, every accepted input produces byte-identical artifacts at base and after,
or each difference is classified as one of:

- a corrected literal value (C1);
- an ordering fix by decision, such as stage JSB-1's;
- an early-error refusal that moved stage under the contract, with the same code.

### T7. Fuel-parity twins (Broiler.VM)

**MET if, and only if**, every twin gives one verdict at every ceiling tried, at base and after.

### T8. Differential cross-check (Broiler.VM)

**MET if, and only if**, the harness shows no new undeclared finding against base, and every
declaration a correction changes is updated with its reason. This rule **cannot** meet a
conformance clause on its own.

---

## 5. The evidence matrix

| Milestone | L1 | L2 | L4 | L5 | L7 | L8 | L9 | L10 | L11 | L12 | L13 | L14 | L15 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| SJL-0 | | | | base | | base | | base | base | base | | base | base |
| SJL-1 | ● | | | | | | | | | | | | |
| SJL-2 | | | | | | | | probe | probe | ● | | | |
| SJL-3 | ● | ● | | ● | | ● | | ● | ● | ● | ● | ● | ● |
| SJL-4 | ● | ● | | ● | | ● | | ● | ● | ● | | ● | ● |
| SJL-5 | ● | ● | | ● | | ● | ● | ● | ● | ● | ● (JSON modules) | ● | ● |
| SJL-6 | ● | ● | | ● | | ● | | ● | ● | ● | | ● | ● |
| SJL-7 | ● | ● | ● | ● | | ● | ● | ● | ● | ● | | ● | ● |
| SJL-8 | ● | ● | ● (zic) | ● | ● | | | ● | ● | ● | | ● | ● |
| SJL-9 | ● | ● | | ● | ● | | | ● | ● | ● | | ● | ● |
| SJL-10 | ● | ● | | ● | ● | | | ● | ● | ● | | ● | ● |
| SJL-11 | ● | ● | | ● | | ● | ● | ● | ● | ● | | ● | ● |
| SJL-12 | | | | ● | | | | | | | ● | | |
| SJL-13 | ● | | | ● | | ● | ● | ● | ● | ● | ● | ● | ● |
| SJL-14 | ● | | | ● | | | ● | ● | ● | ● | | | ● |
| SJL-15 | ● | | | ● | | | ● | ● | ● | ● | ● | ● | ● |
| SJL-16 | sweep | | | | | | | | | | | | |
| SJL-17 | ● | ● | ● (collation) | ● | ● | | | ● | ● | ● | | ● | ● |

*(Revised 2026-10-11, when decision `SJL-D-24` moved collation to SJL-17: SJL-9's L4 cell read
"● (collation)".)*

---

## 6. Bundle shapes

| Repository | Shape |
|---|---|
| **Broiler.VM** | The universal bytecode programme's shape. A `README.md` stating what the bundle demonstrates, what it excludes and what it does not claim. A `manifest.json` written by the bundle-manifest script: sources as blob identifiers at the source revision; inputs and retained files as hashes of their LF-normalised bytes; `worktree-clean`. For every admitted package: its identifier, version, content hash and nuspec commit (rule `I7`). Also a `collect.py` inside the bundle, the `decision-rule.md`, the logs, and any control patches. An exclusions table defines any new exclusion identifier (rule H2). |
| **Broiler.JS** | New at SJL-0, modelled on Broiler.VM's: `docs/evidence/sjl-n-nnn/` with `README.md`, `manifest.json`, the collector, `decision-rule.md` and logs. Text is stored with LF line endings before hashing; Broiler.JS has no `.gitattributes` today, and the shared folder gains one at SJL-2. |
| **Component repositories** | The same shape, created by the milestone that first needs it there (SJL-7, SJL-11) |

Collection follows Broiler.VM's conventions:

- run on a clean tree;
- one log per step, failures included;
- the collector judges nothing; the README interprets;
- paths are hidden as `<root>` and `<scratch>`;
- every injected control is reverted, and the revert is checked;
- the UI language is fixed.

**A bundle is immutable once retained.** A test run never rewrites one. This is the lesson of
Broiler.VM's `BROILER_EVIDENCE_WRITE` switch.

---

## 7. Negative controls by milestone

| Milestone | Controls |
|---|---|
| SJL-1 | A mutated vector's expected value; a vector with engine-only provenance; a malformed record |
| SJL-2 | A non-admitted package; an admitted package in a core project, or in Format; a changed content hash; a probe referencing `System.Text.RegularExpressions`; the frontend in `ExecutionOnly`'s closure; a public member exposing a shared type; a bundle without the upstream commit; an unmapped package identifier; each `SL` rule's witness |
| Every cut-over | A re-added deleted member (rule `I5`); a vector changed to an engine's wrong answer; the pristine consumer restored without the shared package on its feed (it must fail with the missing-package error) |
| SJL-7 to SJL-9, SJL-17 | A Broiler.VM pin differing from its home's by one hash (rule `I4`); a regenerated table with one byte changed, in the home and in Broiler.VM |
| SJL-11 | A pattern whose budget runs out, which must report exhaustion and not "no match" |
| SJL-12 to SJL-15 | A frontend with a static counter (SL2); a hash-ordered output (SL9); a diagnostic code mapped to two registry meanings |

---

## 8. What no milestone runs

- **A comparison of one engine's speed, or fuel, against the other's.**
- **A run on a runtime identifier** that Broiler.VM's ADR 0012 does not declare, presented as
  evidence for it.
- **A comparison of the two test262 runners' totals.**
- **Any measurement whose figure a plan document would quote.**
