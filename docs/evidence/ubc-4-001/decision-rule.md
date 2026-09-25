<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle UBC-4-001's decision rule: the WebAssembly translator, judged by per-assertion parity

**Written:** 2026-09-25, at work package UBC-0.7 of
[the universal bytecode programme roadmap](../../universal-bytecode.roadmap.md), before any code of
milestone UBC-4 exists and before any code of the universal bytecode exists at all. **Milestone
judged:** UBC-4, exit gate clauses 4 and 5. **Owner:** the WebAssembly profile owner, with the core
architecture owner. **Reviewer:** none. Every role this rule names is held by one person (EX-30), and
this rule does not claim that anyone independent has read it.

**What this file is.** The one predeclared decision of bundle `ubc-4-001` and of its sibling
`ubc-4-002`: whether translating a validated module into universal bytecode and running it in the
bytecode emitter changed any answer the WebAssembly profile gives, other than in the one class that is
expected to change. It is committed before the first commit of milestone UBC-4. **It holds no figure.**
The evidence class is **conformance parity**, not a measurement.

**A precondition this rule states rather than hides.** The roadmap's UBC-4 names "the specification's
test suite through the harness, per assertion". **On this rule's date no such harness exists**: the
WebAssembly profile's own ledger records its oracle milestone as not started, with no suite pin and no
reader for the specification's script format, and the harness root
`Broiler.VM.Composition.WebAssembly.Harness` runs hand-written modules, a retained corpus and a
differential lane, and no published assertion. This rule therefore names two populations. Population
A is the one the roadmap means and cannot be judged until the reader exists; population B is what the
harness root runs today and is judged in any case. **This rule does not narrow population A to
population B.** If the base commit of UBC-4.1 has no script reader, the base run of population A is
not taken, UBC-4's clause 5 cannot be met, and the programme ledger's UBC-4 row says so, naming the
WebAssembly profile owner as the holder of the reader.

**How it is read.** Bundle `ubc-4-001` retains the base run, taken at the base commit before the
translator exists (roadmap UBC-4.1). Bundle `ubc-4-002` retains the run after, and the comparison.
Each README names this file's adding commit and the first code commit of UBC-4.

```rule
evidence class: conformance parity (not a measurement)
population A: every assertion of the WebAssembly specification's test suite at a pinned revision, read by a script reader in the harness root, under the feature manifest the family's table is selected by; precondition: the pin and the reader exist at the base commit, or population A is not judged and clause 5 of UBC-4 stays unmet
population B: every retained entry of src/tests/wasm/corpus (corpus.manifest), every execution check and every differential check the harness root Broiler.VM.Composition.WebAssembly.Harness runs, each replayed as that root replays it
comparison: per assertion (A) and per entry or check (B), the answer at the base commit against the answer after the translator, over the same set
admitted classes:
  (f) an assertion, entry or check that exercises one of the float-comparison instructions (W3C opcodes 0x5B to 0x66), whose base answer is the defect the base interpreter gives (a validated module ending in a contract violation) and whose answer after is the value the specification gives; each such row is named in ubc-4-002
  (r) a retained corpus entry of population B whose base answer is a refusal and whose answer after is a refusal of the same outcome category that the universal walk now gives, with a universal code in place of the profile's; each such entry is recorded with its new code and its reason in the corpus re-base of UBC-4.7
every other difference is a regression, including a float-comparison row whose answer after is anything but the specification's value, and a malformed module the decoder or the validator refused at the base that is admitted after
negative control: obligation E2's differential check over the family's float-comparison rows is run against the unmodified interpreter arms and is retained failing, naming the rows; the arms are corrected; the same check is retained passing
MET if and only if population A was judged, every difference in populations A and B is in class (f) or class (r) and is named, and the negative control was retained failing and then passing.
NOT MET otherwise, and in particular NOT MET while population A cannot be judged because its precondition is unmet.
```

**Where the classes come from.** The concept's section 2.2 records that the validator admits the
float comparisons and the interpreter's numeric dispatch routes them to an arm with no case for them;
[`docs/tasks/fix-webassembly-float-comparisons.md`](../../tasks/fix-webassembly-float-comparisons.md)
writes the repair out. Class (f) is that defect becoming an answer, and nothing else. Class (r) is the
roadmap's work package UBC-4.7 stated as a class: a malformed module keeps its refusal, and only which
verifier layer gives it, and so which code it carries, may move.

**What the verdict does.**
- **MET.** Clauses 4 and 5 of UBC-4's exit gate are demonstrated by `ubc-4-002`. That is evidence, not
  acceptance. While population A cannot be judged the verdict is NOT MET, and `ubc-4-002` may still
  retain population B's comparison and the negative control as partial evidence, named as such.

*(Revised 2026-09-25, the day it was written, before any base run and before any code of UBC-4 existed:
the block named class (f) only, while its "corpus re-base" line admitted refusals that move to the
universal walk with a new code - an admission the block did not name - and its verdict allowed a second
MET, for clause 4 alone. The block now names both classes and produces one decision. Nothing was judged
under the superseded text.)*
- **NOT MET.** The milestone stays `In progress`; a regression is fixed and the run after is taken
  again whole. The rule is not revised to admit a difference; a revision is a new dated file with this
  one quoted, and the base run is taken again under it.

**Disclosure: what had been seen when this rule was written.** Nothing of the universal bytecode or of
the translator, neither of which existed. The float-comparison defect was known from reading
`WasmInterpreter`'s numeric dispatch, as the concept records; no module exercising it had been run for
this rule.

**What this rule does not decide.** It judges no native form (that is `ubc-6b-001`'s rule), no speed,
and no machine but the one `ubc-4-002`'s manifest names. It does not take decisions UBC-D-2 (the memory
representation) or UBC-D-3 (the WA-5 manifest), and it does not mint the suite pin or the reader.
