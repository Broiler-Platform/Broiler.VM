<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle UBC-3-001's decision rule: the JavaScript cut-over, judged by per-variant parity

**Written:** 2026-09-25, at work package UBC-0.7 of
[the universal bytecode programme roadmap](../../universal-bytecode.roadmap.md), before any code of
milestone UBC-3 exists and before any code of the universal bytecode exists at all. **Milestone
judged:** UBC-3, exit gate clauses 5, 6 and 7. **Owner:** the JavaScript profile owner, with the core
architecture owner. **Reviewer:** none. Both roles, and every other role this rule names, are held by
one person (EX-30), and this rule does not claim that anyone independent has read it.

**What this file is.** The one predeclared decision of bundle `ubc-3-001` and of its sibling
`ubc-3-002`: whether moving the JavaScript profile's bytecode form from its own interpreter onto the
universal bytecode and the bytecode emitter changed any verdict. It is committed before the first
commit of milestone UBC-3, so no observation of the new form can have shaped it. **It holds no
figure.** The evidence class is **conformance parity**, not a measurement: nothing here is timed,
nothing is ranked, and no score is compared. The bundles' READMEs, written when each run is retained,
are the only place any figure from these runs may be written.

**How it is read.** Bundle `ubc-3-001` retains the base run, taken at the base commit before any work
package of UBC-3 other than UBC-3.1 changes the engine (roadmap UBC-3.2). Bundle `ubc-3-002` retains
the cut-over run and the comparison. Each bundle's README names this file's adding commit, and names
the first code commit of UBC-3, so that a reader can check with
`git merge-base --is-ancestor <adding commit> <first UBC-3 code commit>` that this file came first.
The comparison is made per variant by `eng/compare-test262-forms.py --same-form` over the two merged
reports, and every row it prints is then classed by hand against the classes below: the script is
retained as a tool, and the classing is the rule's.

**The arms.** `base` is the conformance host built from the base commit, running the profile's
current interpreter. `cut-over` is the conformance host built from the commit that completes UBC-3.9,
running the same source through the JavaScript lowering, the universal bytecode and the bytecode
emitter. Both are named by commit and by the hash of every binary in `ubc-3-002`'s manifest.

```rule
evidence class: conformance parity (not a measurement)
population: every variant of the pinned tc39/test262 suite (src/tests/conformance/pins/test262.pin), whole, under the feature manifest broiler.javascript.wide, driven by eng/run-test262.py in the bytecode form; the fuel-parity twins src/tests/forms/wide/*.small.js; the Octane benchmarks the pin src/tests/octane/pins/octane.pin names, driven by eng/run-octane.py
configuration: base and cut-over run with identical eng/run-test262.py and eng/run-octane.py options, the fuel and wall-clock allowances among them, stated in ubc-3-002's manifest before the cut-over run starts; neither run is partial (no --dir)
retainability: each merged test262 report is one eng/run-test262.py marks retainable; a run that is not retainable is not judged and is re-run whole
comparison: per variant, the verdict row of the base report against the verdict row of the cut-over report, over the same set of variants
admitted classes:
  (a) a variant whose base verdict depended on the old verifier admitting an artifact the universal walk refuses, where that refusal is recorded, entry by entry, in the corpus re-base of UBC-3.8 with its reason;
  (b) a variant the re-based ratchet retires by name, with its reason written beside it in ubc-3-002 (the floors under src/tests/conformance/floors hold totals only, so the named list is retained in the bundle and the floor's retired line cites it);
  (c) a wall-clock exhaustion on either side is not a verdict: that variant is re-run alone, under the same allowances, on both arms; if one arm still exhausts the wall clock and the other does not, the difference is not admitted
every other difference is a regression
octane: every benchmark the pin names reports a score in the cut-over form; no score is compared, ranked or quoted outside ubc-3-002's README
twins: every fuel-parity twin gives one verdict at every fuel ceiling tried, and the verdict at each ceiling is the base commit's
MET if and only if every difference is in an admitted class and is named in ubc-3-002, every Octane benchmark reports a score, and every twin agrees at every ceiling tried.
NOT MET otherwise.
```

**Where the classes come from.** Class (a) and class (b) are the two the roadmap's UBC-0.7 names. Class
(c) is not an admission: it is the procedure by which a wall-clock exhaustion, which depends on the
machine and not only on the program, is separated from a verdict, and it ends in a regression when the
separation does not hold. **No other class exists**, in particular none for a fuel exhaustion: the
programme's principle 7 is that fuel is exact per instruction at the same point in every form, and the
bytecode form's charges move from the interpreter's charge sites to the loop's without changing any
row's cost.

**What the verdict does.**
- **MET.** Clauses 5, 6 and 7 of UBC-3's exit gate are demonstrated by `ubc-3-002`. That is evidence,
  not acceptance: the programme ledger's UBC-3 row still names every other clause, and under
  [`docs/mvp.md`](../../mvp.md) no row reaches `Accepted`.
- **NOT MET.** The milestone stays `In progress`. A regression is fixed and the cut-over run is taken
  again whole; the rule is not revised to admit it. A rule revision, if one is ever needed, is a new
  dated file beside this one with this file quoted, and the base run is taken again under it, because
  evidence gathered for an older rule is not carried forward silently (programme ledger update rule 5).

**Disclosure: what had been seen when this rule was written.** Nothing of the universal bytecode: no
line of `Broiler.VM.Ubc`, of the bytecode emitter or of the JavaScript family's tables existed. The
author had seen the profile's own whole-suite runs of earlier dates, in the bytecode form and in the
baseline native form, as the profile's bundles record them; no figure from them is used here, and none
is recorded.

**What this rule does not decide.** It judges no native form (that is `ubc-6b-001`'s rule), no speed,
no Native AOT image, and no machine but the one `ubc-3-002`'s manifest names. It does not decide
whether the value form's substrate is retired (decision UBC-D-1). It says nothing about UBC-3's other
clauses — determinism, the corpus re-base, the reference sets — which the milestone meets on their
own evidence.
