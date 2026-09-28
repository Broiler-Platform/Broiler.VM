<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle UBC-4-006's decision rule: bundle UBC-4-005's, with one class added after its runs were seen

**Written:** 2026-09-28. **Milestone judged:** UBC-4 of
[the universal bytecode programme roadmap](../../universal-bytecode.roadmap.md), exit gate clauses 4 and
5. **Owner:** the WebAssembly profile owner, with the core architecture owner, who decided on 2026-09-28
that the rule be revised rather than the clauses left unmet. **Reviewer:** none. Every role this rule
names is held by one person (EX-30), and this rule does not claim that anyone independent has read it.

**What this file is.** A revision of bundle `ubc-4-005`'s decision rule, which its own section 4 says
must be "a new dated file with this one quoted, and the base run is taken again under it". It is the one
decision of bundle `ubc-4-006`.

**READ THIS FIRST: THIS RULE IS NOT PREDECLARED.** It is written after bundle `ubc-4-005` was collected,
after every answer of both of its runs was seen, and after its decision came out NOT MET. It adds one
class, and that class describes exactly the rows that decided NOT MET. A MET under this rule is
therefore weaker evidence than a MET under a rule written before its runs. Section 3 argues why the
class is a true account of those rows and not a way round them. The ledger and the bundle's README say
the same thing where they report the decision.

## 1. What this rule supersedes, quoted whole

The rule of [`ubc-4-005/decision-rule.md`](../ubc-4-005/decision-rule.md), committed in `e46a45f` and
corrected in `45015a3`, read:

```rule
evidence class: conformance parity (not a measurement)
suite: the WebAssembly specification's core test scripts, the 73 files test/core/*.wast of github.com/WebAssembly/spec at commit 977f97014c962f7bd1291fcc6d28b41a924882bf (the commit the tag wg-1.0 names), archived unmodified in this repository with a pin naming that commit and a digest over every file
reader: a script reader in the harness root Broiler.VM.Composition.WebAssembly.Harness, which reads a script's text, encodes each text module to the binary format itself, and drives every command through the core's public verification, instantiation and invocation surface; its own files are identical at the base commit and after except one adapter file holding the catalog and the verification call, which milestone UBC-4 changed from the profile's own descriptor to the translate-first path, and each tree's Program.cs gains the same lines handing the reader its arguments; both versions of the adapter are retained in the bundle
base commit: a56180b (the retired interpreter, with every change main carries that is not milestone UBC-4's), with the reader applied as a change to the harness root alone; no product file differs from a56180b
after commit: the first commit on main that carries the reader, named in the bundle
population A: every command of the suite except register, each identified by its file and its ordinal among that file's commands; excluded by name, (t): an assert_malformed whose module is a module quote, because it judges a text-format reader and the profile never receives text
population B: every retained entry of src/tests/wasm/corpus (corpus.manifest), every execution check and every differential check the harness root runs, each replayed as that root replays it
runtime: each script file runs in a runtime of its own, created with the harness root's options - every budget dimension adopting the profile's default except LiveRuntimes, which adopts the parent's remaining - and the effective limit vector is printed at the head of the run
answer: per command, one line - for a module, the instance published, or the verification's outcome, reason, diagnostic code and byte offset, or the instantiation's outcome, reason and trap kind; for an action, the result values by type and bit pattern, or the trap kind, or the outcome and reason with the exhausted dimension and scope, or the entry-point fault, or that the module it names has no instance; for an assertion, the answer of the module or action it wraps
verdict: per assertion, against the specification's expectation - assert_return: equal values, floats compared by bit pattern; assert_return_canonical_nan and assert_return_arithmetic_nan: a NaN of that kind; assert_trap: a trap whose kind's message and the expected message are one a prefix of the other, with out of bounds table access and undefined element both answering "undefined element"; assert_exhaustion: a resource exhaustion naming CallDepth; assert_malformed and assert_invalid: refused at verification; assert_unlinkable: no instance; a module or action command: an instance published, or an action answering without a trap
determinism: each run is taken twice and the two print the same lines, or the run is void and is not compared
comparison: per command (A) and per entry or check (B), the answer at the base commit against the answer at the after commit, over the members present at both
admitted classes:
  (f) a command exercising one of the float-comparison instructions (W3C opcodes 0x5B to 0x66) - an action of, or an assertion over, a module whose code contains one - whose base answer is the base interpreter's defect (the invocation ending ProfileFault/ProfileContractViolation with no results) and whose answer after passes the verdict
  (r) a command, entry or check whose base answer is a refusal at verification and whose answer after is a refusal of the same outcome that the universal walk gives, with a universal code in place of the profile's
  (g) a command whose base answer carries the value -1 from a memory.grow a core budget refused, and whose answer after is a resource exhaustion naming that budget's dimension: the change the programme's clause 6 revision of 2026-09-28 accepted
  (v) a command whose base answer fails the verdict and whose answer after passes it, outside (f), (r) and (g), each admitted only with the base defect it corrects written out; a (v) row whose defect cannot be named is a regression
each row of each class is named in the bundle; every other difference is a regression, including a command whose base answer passes the verdict and whose answer after fails it, and a module the decoder or the validator refused at the base that is admitted after
negative control: obligation E2's differential check over the family's float-comparison rows, run on the after commit with the reference arms' routing restored to the unmodified interpreter's, is retained failing and naming the rows; the patch is reverted and the same check is retained passing
ratchet: a verdict floor for population A, recording the suite's commit, is set from the base run's verdicts and re-based by hand to the run after's, every command that moved named with its class
MET if and only if both runs of population A were taken and are not void, every difference in populations A and B is in class (f), (r), (g) or (v) and is named, the negative control was retained failing and then passing, and the ratchet was re-based with the classes named.
NOT MET otherwise.
```

Under it the decision was NOT MET on one condition, as bundle `ubc-4-005`'s README says:

- the rule named no class for 1,432 commands of population A;
- every one was a canonical- or arithmetic-NaN assertion that both runs pass;
- in every one, the base answered a NaN carrying the sign or payload its arithmetic brought over, and the
  run after answered the positive canonical NaN.

Every other condition held.

## 2. The rule

The only change is class (n), and its name in the MET line.

```rule
evidence class: conformance parity (not a measurement)
suite: the WebAssembly specification's core test scripts, the 73 files test/core/*.wast of github.com/WebAssembly/spec at commit 977f97014c962f7bd1291fcc6d28b41a924882bf (the commit the tag wg-1.0 names), archived unmodified in this repository with a pin naming that commit and a digest over every file
reader: a script reader in the harness root Broiler.VM.Composition.WebAssembly.Harness, which reads a script's text, encodes each text module to the binary format itself, and drives every command through the core's public verification, instantiation and invocation surface; its own files are identical at the base commit and after except one adapter file holding the catalog and the verification call, which milestone UBC-4 changed from the profile's own descriptor to the translate-first path, and each tree's Program.cs gains the same lines handing the reader its arguments; both versions of the adapter are retained in the bundle
base commit: a56180b (the retired interpreter, with every change main carries that is not milestone UBC-4's), with the reader applied as a change to the harness root alone; no product file differs from a56180b
after commit: the first commit on main that carries the reader, named in the bundle
population A: every command of the suite except register, each identified by its file and its ordinal among that file's commands; excluded by name, (t): an assert_malformed whose module is a module quote, because it judges a text-format reader and the profile never receives text
population B: every retained entry of src/tests/wasm/corpus (corpus.manifest), every execution check and every differential check the harness root runs, each replayed as that root replays it
runtime: each script file runs in a runtime of its own, created with the harness root's options - every budget dimension adopting the profile's default except LiveRuntimes, which adopts the parent's remaining - and the effective limit vector is printed at the head of the run
answer: per command, one line - for a module, the instance published, or the verification's outcome, reason, diagnostic code and byte offset, or the instantiation's outcome, reason and trap kind; for an action, the result values by type and bit pattern, or the trap kind, or the outcome and reason with the exhausted dimension and scope, or the entry-point fault, or that the module it names has no instance; for an assertion, the answer of the module or action it wraps
verdict: per assertion, against the specification's expectation - assert_return: equal values, floats compared by bit pattern; assert_return_canonical_nan and assert_return_arithmetic_nan: a NaN of that kind; assert_trap: a trap whose kind's message and the expected message are one a prefix of the other, with out of bounds table access and undefined element both answering "undefined element"; assert_exhaustion: a resource exhaustion naming CallDepth; assert_malformed and assert_invalid: refused at verification; assert_unlinkable: no instance; a module or action command: an instance published, or an action answering without a trap
determinism: each run is taken twice and the two print the same lines, or the run is void and is not compared
comparison: per command (A) and per entry or check (B), the answer at the base commit against the answer at the after commit, over the members present at both
admitted classes:
  (f) a command exercising one of the float-comparison instructions (W3C opcodes 0x5B to 0x66) - an action of, or an assertion over, a module whose code contains one - whose base answer is the base interpreter's defect (the invocation ending ProfileFault/ProfileContractViolation with no results) and whose answer after passes the verdict
  (r) a command, entry or check whose base answer is a refusal at verification and whose answer after is a refusal of the same outcome that the universal walk gives, with a universal code in place of the profile's
  (g) a command whose base answer carries the value -1 from a memory.grow a core budget refused, and whose answer after is a resource exhaustion naming that budget's dimension: the change the programme's clause 6 revision of 2026-09-28 accepted
  (v) a command whose base answer fails the verdict and whose answer after passes it, outside (f), (r) and (g), each admitted only with the base defect it corrects written out; a (v) row whose defect cannot be named is a regression
  (n) a command whose answer at both commits is one NaN value of the same width, whose verdict passes at both, and whose answer after is the positive canonical NaN of that width: the sign and payload the retired interpreter's arithmetic carried over, against the NaN the family's primitive table produces under the NaN-canonicalisation flag roadmap work package UBC-4.3 names
each row of each class is named in the bundle; every other difference is a regression, including a command whose base answer passes the verdict and whose answer after fails it, and a module the decoder or the validator refused at the base that is admitted after
negative control: obligation E2's differential check over the family's float-comparison rows, run on the after commit with the reference arms' routing restored to the unmodified interpreter's, is retained failing and naming the rows; the patch is reverted and the same check is retained passing
ratchet: a verdict floor for population A, recording the suite's commit, is set from the base run's verdicts and re-based by hand to the run after's, every command that moved named with its class
MET if and only if both runs of population A were taken and are not void, every difference in populations A and B is in class (f), (r), (g), (v) or (n) and is named, the negative control was retained failing and then passing, and the ratchet was re-based with the classes named.
NOT MET otherwise.
```

## 3. Why class (n) is an account of those rows and not a way round them

- **The cause is a design the milestone states, not a defect found by looking.** Roadmap work package
  UBC-4.3 gives the family's primitive table "the NaN-canonicalisation flag set". The family's E2 lane
  prints "NaN canonicalised yes" on every run. A primitive that produces a NaN produces the positive
  canonical one. The retired interpreter's NaNs were whatever its arithmetic carried over. `ubc-4-005`'s
  rule was written knowing all of this, and did not think of it.
- **The specification makes both answers right.** A NaN result's sign is nondeterministic. So is its
  payload, within the canonical or arithmetic bounds each assertion states. Both runs pass every one of
  these commands, so the class admits no command whose verdict differs.
- **The class is as narrow as the rows.** It needs the answer at both commits to be one NaN value of one
  width, the verdict to pass at both, and the answer after to be exactly the positive canonical NaN. It
  cannot admit:
  - a NaN assertion either run fails;
  - a NaN of another shape after;
  - a non-NaN answer;
  - a NaN at one commit and a number at the other.

  A command outside those bounds is still a regression.
- **What it costs.** A reader cannot tell from this rule alone whether the class was drawn to fit the
  rows, and it was drawn after seeing them. What a reader can check is the rows themselves. Bundle
  `ubc-4-006`'s `classes.log` names every command the class admits, and `comparison.log` shows each
  one's two answers.

## 4. How it is read

As `ubc-4-005`'s section 4, which this rule keeps, with one addition. **The base run is taken again
under this rule** at the same base, `a56180b`, and the run after at the same after commit, `6f792fd`.
The reader, the suite and the product are unchanged since `ubc-4-005`, so both runs are expected to print
what they printed there. Nothing from `ubc-4-005` is carried: its runs are its own, and `ubc-4-006`
retains its own.

## 5. What the verdict does

- **MET.** Clauses 4 and 5 of UBC-4's exit gate are demonstrated by `ubc-4-006`, under a rule revised
  after the runs it revises for, and they are reported that way. That is evidence, not acceptance.
- **NOT MET.** As `ubc-4-005`'s section 6.

## 6. Disclosure: what had been seen when this rule was written

Everything:
- every answer of both runs of bundle `ubc-4-005`;
- its classification;
- its decision.

The class was written to describe the 1,432 commands that decided that bundle, and it describes no other
command of either run.

## 7. What this rule does not decide

The same as `ubc-4-005`'s section 8. It is not the WebAssembly profile's oracle, it confirms nothing about
the licence, and it judges no native form, no speed and no other machine.
