<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle UBC-6B-001's decision rule: the language families in the `x86-64` form, judged by per-verdict parity

**Written:** 2026-09-25, at work package UBC-0.7 of
[the universal bytecode programme roadmap](../../universal-bytecode.roadmap.md), before any code of
milestone UBC-6b exists and before any code of the universal bytecode exists at all. **Milestone
judged:** UBC-6b, exit gate clauses 2, 3, 4, 5 and 6. **Owner:** the core architecture owner, with both
language profile owners and the security owner. **Reviewer:** none. Every role this rule names is held
by one person (EX-30), and this rule does not claim that anyone independent has read it.

**What this file is.** The one predeclared decision of bundle `ubc-6b-001`: whether the `x86-64`
emitter, over the JavaScript and WebAssembly families' tables, gives the verdicts the bytecode emitter
gives for the same artifacts, and whether every primitive it implements inline answers what the
family's handler answers. It is committed before the first commit of milestone UBC-6b. **It holds no
figure.** The evidence class is **form parity**, not a measurement.

**How it is read.** Both forms are run from one build of one commit, the commit that completes
UBC-6b, on one machine; the bundle's manifest names the commit, the machine and every binary by hash,
and its README names this file's adding commit and the first code commit of UBC-6b. The whole-suite
comparisons are made by `eng/compare-test262-forms.py` (bytecode report as the reference, the `x86-64`
report as the candidate) and by the WebAssembly harness's own comparison, and every row either prints
is classed against the classes below.

```rule
evidence class: form parity (not a measurement)
population 1: every variant of the pinned tc39/test262 suite, whole, under broiler.javascript.wide, in the bytecode form and in the x86-64 form of the convention of the machine's runtime identifier
population 2: every assertion (population A of ubc-4-001's rule, when it exists) and every entry and check (population B of that rule) of the WebAssembly family, in the bytecode form and in the x86-64 form
population 3: obligation E2's primitive input corpus (every entry of the primitive table, every NaN class, both signed zeros, every overflow and every trap edge), for every Primitive row of the JavaScript numeric table and of the WebAssembly tables, the family handler's answer against the emitter's inline implementation, with each family's NaN-canonicalisation flag in force
population 4: the fuel-parity twins of both families, in both forms, at every fuel ceiling tried, programs that load a guest program excluded by name
population 5: the Octane benchmarks the pin src/tests/octane/pins/octane.pin names, in the x86-64 form
configuration: both forms of a population run with identical options and allowances, stated in the manifest before either run starts; no whole-suite run is partial
admitted classes, for populations 1 and 2 only (the ones JSD-0025 section 11 names):
  (a) a refusal naming the emission ceiling;
  (b) a fuel exhaustion on a variant that loads a guest program (eval, the Function constructor, a dynamic import);
  (c) under a wall-clock allowance, a wall-clock exhaustion in the x86-64 form
population 3 admits NO difference: the first differing bit of any answer, for any input, is a failure naming the primitive and the input
population 4 admits NO difference outside the named guest-loading exclusion
population 5: every benchmark reports a score in the x86-64 form; no score is compared, ranked or quoted outside the bundle's README
MET if and only if every difference in populations 1 and 2 is in an admitted class and is named in the bundle, populations 3 and 4 show no difference, and every benchmark of population 5 reports a score.
NOT MET otherwise.
```

**Where the classes come from.** From JSD-0025 section 11, which names exactly these as the
differences between a native and a bytecode verdict that are costs of a native form rather than
defects in it, and from the concept's obligations E2 and E6, which admit nothing. The primitive
corpus's population is the one the fixture family's UBC-2.4 retains, extended by the language
families' own rows.

**What the verdict does.**
- **MET.** Clauses 2 to 6 of UBC-6b's exit gate are demonstrated by `ubc-6b-001`. That is evidence and
  not acceptance, and it claims no runtime identifier for either convention.
- **NOT MET.** The milestone stays `In progress`; a difference outside the classes is a defect in the
  emitter or in a family's classification of a row, and is fixed before the runs are taken again whole.

**Disclosure: what had been seen when this rule was written.** Nothing of the `x86-64` emitter over the
universal bytecode, which did not exist. The author had seen the JavaScript profile's whole-suite
comparison of its own baseline native form against its bytecode form, as bundle `jsb-11-001` records it;
no figure from it is used here, and none is recorded.

**What this rule does not decide.** It judges no speed, no Native AOT image, no `arm64` form (UBC-7's
evidence is its encodings and no run), and no machine but the one the bundle's manifest names. It does
not weigh route UBC-R6 (one call per dynamic row).
