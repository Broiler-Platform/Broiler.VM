<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle UBC-4-005's decision rule: the WebAssembly translator, judged again by per-assertion parity

**Written:** 2026-09-28. **Milestone judged:** UBC-4 of
[the universal bytecode programme roadmap](../../universal-bytecode.roadmap.md), exit gate clauses 4 and
5. **Owner:** the WebAssembly profile owner, with the core architecture owner. **Reviewer:** none.
Every role this rule names is held by one person (EX-30), and this rule does not claim that anyone
independent has read it.

**What this file is.** A revision of bundle `ubc-4-001`'s decision rule, which its own text requires to
be "a new dated file with this one quoted, and the base run is taken again under it". It is the one
decision of bundle `ubc-4-005`: whether translating a validated module into universal bytecode and
running it in the bytecode emitter changed any answer the WebAssembly profile gives, other than in the
classes named below. **It holds no figure.** The evidence class is **conformance parity**, not a
measurement.

**It is committed before any of the following exists or has been done**: the suite named below in this
repository; its script reader; any run of any of its commands by any build of this profile, at any
commit. The commit that adds this file is named in the bundle's README, with every commit after it.

## 1. What this rule supersedes, quoted whole

The rule of [`ubc-4-001/decision-rule.md`](../ubc-4-001/decision-rule.md), first committed in `6486add`
and revised in `3042855`, read:

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

Under it the decision was NOT MET, because population A could not be judged: the base commit, `72e7491`,
had neither the pin nor the reader, and bundle `ubc-4-002` retains population B's comparison and the
negative control as partial evidence, named as such.

## 2. Why a new rule, and what changed

- **The precondition cannot be met by the old rule's base commit.** `72e7491` cannot be given a pin and
  a reader after the fact. This rule names a different base commit, one that still has the retired path,
  and says in section 3 how the reader reaches it.
- **The suite was believed unreachable, and is not.** The profile owner left clauses 4 and 5 open on
  2026-09-28 on the understanding that the specification's repository could not be read from where this
  work is done. It can: the repository's tags and files are readable over git, and the owner directed
  that the clauses be taken up the same day, with the revision named in section 3 pinned.
- **This rule is not predeclared in the old rule's sense, and it says so.** The old rule was written
  before any code of UBC-4 existed. This one is written after all of it landed on `main`, in the merge
  `1a9966e`. Section 7 says what had been seen and what had not. What makes it a rule and not a
  description is the one thing that has still not been seen: any answer of population A, at either
  commit.

## 3. The rule

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

*(Revised 2026-09-28, the day it was written, before the suite was archived, before the reader existed
and before any command was run. The suite line read "the 72 files test/core/*.wast", and section 7 "there
are 72 files". The directory holds 73: the count was taken over a listing that also held `README.md` and
`run.py`, and the two were subtracted as one. The set the line names, every `test/core/*.wast` at the
commit, is unchanged, and nothing was judged under the superseded count.)*

## 4. How it is read

- **The base run and the run after are both this bundle's.** The base is collected in a separate working
  tree at `a56180b` with the reader's harness files applied; the run after at the after commit. Both are
  taken in the framework-dependent build. The run after is also taken in a trimmed and a Native AOT
  publish of the harness root, and those must print the lines the framework-dependent run prints; that is
  a condition on the evidence, not a class.
- **Population B is replayed again at the new base.** Its comparison in `ubc-4-002` was against
  `72e7491` and is not carried: the members it shares with this rule are judged again, at `a56180b` and
  the after commit, over the members present at both.
- **A regression is fixed, not admitted.** On NOT MET the milestone stays `In progress`, the regression is
  fixed and the run after is taken again whole. This rule is not revised to admit a difference; a
  revision is a new dated file with this one quoted, and the base run is taken again under it.
- **A defect of the reader is a defect of both runs.** The reader is the same code at both commits but
  for the adapter, so a reader that misreads a command misreads it twice. That can hide a difference in
  a command the reader gets wrong, and cannot create one; the bundle says how the reader was checked.

## 5. Where the classes come from

- **(f) and (r)** are the old rule's, unchanged in meaning. (f) is the routing defect the concept's
  section 2.2 records and
  [`docs/tasks/fix-webassembly-float-comparisons.md`](../../tasks/fix-webassembly-float-comparisons.md)
  writes out, becoming an answer. (r) is the roadmap's work package UBC-4.7 stated as a class, widened
  from population B's corpus to every command, because a refusal the walk now gives with a universal code
  is the same change wherever the module comes from.
- **(g)** is the change the programme roadmap's clause 6 was revised on 2026-09-28 to accept. The store
  charges a growth's allocation and retention before it allocates, and a growth a core budget refuses
  ends the operation. The retired executor answered the guest minus one and ran on. Under the suite's
  default limits a growth reaches the profile's own page ceiling, which is unchanged and answers minus
  one in both, long before any budget; the class is named because the change is known, not because any
  command is expected in it.
- **(v)** is new, and it is named because the old rule could not have needed it. That rule was written
  before the translator existed, when the retired interpreter was the reference and any difference was
  the translator's. Here the interpreter is retired and the specification is the oracle. A command the
  interpreter answered wrongly and the family answers rightly is still a difference, and still has to be
  named and explained. It is admitted only with its cause, so that it cannot absorb a difference nobody
  understands.

## 6. What the verdict does

- **MET.** Clauses 4 and 5 of UBC-4's exit gate are demonstrated by `ubc-4-005`: clause 5 by population
  A's comparison and the re-based ratchet, clause 4 by the negative control within the one decision. That
  is evidence, not acceptance.
- **NOT MET.** As section 4 says.

## 7. Disclosure: what had been seen when this rule was written

**Seen:**
- all of milestone UBC-4's code;
- bundle `ubc-4-002`'s population B comparison, in which only classes (f) and (r) moved;
- the family's answers to every harness check, and the fixes and bundles `ubc-4-003` and `ubc-4-004` on
  the branch that merged it.

While the revision was being chosen, the suite's files at the named commit were listed, and their
command keywords, their trap messages and their module forms were counted by pattern. That is how this
rule knows there are 73 files, that some modules are given as `module quote` and `module binary`, and
which trap messages the verdict must map. No command was read by the reader, which does not exist, and
**no module of the suite has been verified, instantiated or invoked by any build of this profile, at
any commit.**

**Classes (g) and (v) were added from what is known about the code**, the first from the clause 6
revision and the second from the interpreter no longer being the reference. Neither was added from any
answer of population A.

## 8. What this rule does not decide

- **It is not the WebAssembly profile's oracle.** That profile's milestone WA-4 asks for more:
  - sharding;
  - a self-check before every shard;
  - scope manifests;
  - per-family totals from an exact limit vector;
  - the ratchet's own discipline.

  None of that is built or claimed here. The floor this rule asks for is the programme's, for this
  comparison.
- **It confirms nothing about the licence.** The notice row the ingestion needs lands with the suite. The
  release owner's confirmation that the core's third-party claim stays scoped is that owner's to give.
- **It judges no native form, no speed, and no machine but the one the bundle's manifest names.**
