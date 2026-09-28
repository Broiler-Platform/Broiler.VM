<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Evidence bundle UBC-4-005

**Milestone:** UBC-4 of [the universal bytecode programme roadmap](../../universal-bytecode.roadmap.md) -
the WebAssembly family, exit-gate clauses 4 and 5, judged under the dated rule
[`decision-rule.md`](decision-rule.md) in this directory, which quotes and revises bundle `ubc-4-001`'s.
**Collected:** 2026-09-28, at commit `a0a8451`, from a clean tree, by `collect.py` in this directory. The
rule's after commit is `6f792fd`, the first commit carrying the reader; `a0a8451` differs from it only
inside this directory. The base is `a56180b` with the reader's harness files applied, in a separate
working tree.
**Core contract version:** 1, unchanged. **Universal bytecode contract version:** 2, unchanged.
**Universal bytecode format version:** 1, unchanged.
**The rule's decision: NOT MET.** One condition fails. 1,432 commands of population A answer a different
NaN at the two commits, both answers passing the specification's verdict, and the rule admits no class
for them. Every other condition holds. Clauses 4 and 5 stay unmet, and the milestone stays
`In progress`. It is not accepted either: under [`docs/mvp.md`](../../mvp.md) no milestone is accepted
while review is deferred, and nothing here has been read by anyone but its author.

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle | UBC-4-005 |
| Milestone | UBC-4, exit-gate clauses 4 and 5 |
| Rule | [`decision-rule.md`](decision-rule.md), committed in `e46a45f` and corrected the same day in `45015a3` (a file count), before the suite was archived, the reader existed or any command of the suite had run |
| Suite | `test/core` of `WebAssembly/spec` at `977f97014c962f7bd1291fcc6d28b41a924882bf` (tag `wg-1.0`), archived and pinned at [`src/tests/wasm/spec/`](../../../src/tests/wasm/spec/README.md) in `3489b0f` |
| Reader | The harness root's `--spec` lane, committed in `6f792fd` before any command of the suite ran |
| Base | `a56180b`, the retired interpreter with every change `main` carries that is not UBC-4's; the core is byte-identical to the after commit's and the profile's default limits are the same |
| After | `6f792fd`, on `main` as merged in `1a9966e`, with no product file changed since |
| Compositions | `Broiler.VM.Composition.WebAssembly.Harness`, never advertised |
| RID | `linux-x64` |
| Owner | the WebAssembly profile owner; one person holds every role |
| Reviewer | none |
| Evidence class | conformance parity: two runs of the specification's scripts compared per command, and two runs of the harness root's own population compared per check, with a negative control; no measurement of any kind |

## 2. The decision, condition by condition

| The rule's condition | Holds | Where it is shown |
|---|---|---|
| Both runs of population A taken, each twice, and not void | yes | `determinism.log`: the base's two runs print the same lines, and so do the after commit's. `base-run.log` and `after-run.log`: 19,262 commands each, from 73 scripts, each script in a runtime of its own under the effective limits both logs print on their second line |
| The run after prints the same lines in the three publish modes | yes | `after-modes.log`: the trimmed and Native AOT runs print the framework-dependent run's lines. `publish.log` |
| Every difference in population A is in class (f), (r), (g) or (v), and named | **no** | `comparison.log` lists 6,492 differences and `classes.log` names each. **5,060 are class (f)**: the base answers the retired interpreter's routing defect, `ProfileFault/ProfileContractViolation`, the run after answers the specification's value, and the module each acts on has a float comparison in its code. They are 2,400 commands each of `f32_cmp.wast` and `f64_cmp.wast`, 207 of `float_exprs.wast`, and 53 across `call_indirect`, `loop`, `left-to-right`, `if`, `block` and `memory`. **1,432 are in no class**: section 3 |
| Every difference in population B is in a class, and named | yes | `comparison.log`: the 434 members present at both commits answer identically. The 65 present only after are the checks milestone UBC-4 and the branch after it added, which the base does not have |
| The negative control retained failing and then passing | yes | `control-failing.log`: with `control.patch` restoring the reference arms' routing to the retired interpreter's, obligation E2's lane agrees on 111 of 111 rows outside the control and on 0 of the control's 12, naming each float comparison, exit 1. The patch is reverted and rebuilt. `control-passing.log`: 12 of 12 agree, exit 0 |
| The ratchet re-based by hand with the classes named | yes | `base-verdicts.txt` is the floor set from the base. `floor-after.log`: the run after holds all 13,309 commands the base passes, none regressed. `floor-rebased.txt` is the floor re-based to the run after, its 5,060 moved commands each carrying `(f)` |

**No command's verdict gets worse.** 13,309 pass at both commits, 5,060 fail at the base and pass after,
463 fail at both, and 430 are excluded as class (t) at both. No command passes at the base and fails
after.

## 3. The 1,432 commands no class admits

Every one is an `assert_return_canonical_nan` (503) or an `assert_return_arithmetic_nan` (929), in
`f32.wast` (686), `f64.wast` (686), `float_exprs.wast` (53), `conversions.wast` (6) and `float_misc.wast`
(1). Every one passes its verdict at both commits. The base answers a NaN whose sign or payload it
carried over from an operand: 535 negative canonical NaNs, 461 positive NaNs with other payload bits,
and 436 negative ones with other payload bits. The run after answers the positive canonical NaN,
`7FC00000` or `7FF8000000000000`, every time.

**The cause is known, and it is milestone UBC-4's design.** The family's primitive table sets the
NaN-canonicalisation flag roadmap work package UBC-4.3 names. The E2 lane reports it on every run as "NaN
canonicalised yes". So every NaN a primitive produces is the positive canonical one, and the retired
interpreter's NaNs were whatever its arithmetic carried over. The specification leaves a NaN result's
sign and payload nondeterministic within exactly the bounds both answers meet, so neither run is wrong.

**The rule admits it in no class, and it says what happens next.** Its answer is the bit pattern, and its
classes were written from what was known about the code. The canonicalisation was known and was not
thought of. By the rule's text every such difference is a regression. Its section 4 forbids both ways
out that would not need the owner:

- The rule is not revised to admit a difference. A revision is a new dated file with this one quoted,
  and the base run is taken again under it. Such a file would be written after seeing these rows, and it
  would have to say so.
- A regression is fixed and the run after taken again. Here the fix would be to stop canonicalising
  NaNs, giving up the determinism the flag exists for so that a comparison with the retired interpreter
  agrees. This bundle does not do that.

Which of the two to take, or neither, is the WebAssembly profile owner's decision.

## 4. What population A shows besides the comparison

These are the profile's answers to the specification. They are reported because they were observed, and
none bears on the decision, since each is the same answer at both commits:

- **180 `assert_malformed` modules are admitted.**
  - 176 are in `utf8-custom-section-id.wast`: a custom section's name is not checked as UTF-8.
  - 2 in `binary-leb128.wast` are 32-bit integers written in more than five bytes.
  - 2 are in `custom.wast`.
- **4 `assert_invalid` modules are admitted**, each a function type with more than one result, which
  version 1.0 does not allow.
- **21 `call_indirect` assertions trap** as an indirect call type mismatch where the specification
  expects a value.
- **Every module that imports is refused at verification**, with the profile's unknown-feature code 2403:
  84 module commands. The commands that act on those modules answer `no-instance`, and the
  `assert_unlinkable` commands pass.
- **An exhaustion faults its instance**, because the core does so always. 64 later commands on the same
  modules answer `InvalidState/TerminalFault`, 11 of them `assert_exhaustion`.
- **11 `get` actions have no surface.** The profile's invocation surface reaches functions, not globals.
- **The reader does not read `inline-module.wast`'s three fields as the one implicit module** the script
  format makes of them. It answers each as its own refusal, in both runs. That is a defect of the reader,
  and it is the same in both runs, as the rule's section 4 says such a defect is.

## 5. How the reader was checked, and what was seen when

- **The encoder**, before any command ran. `reader-check.log` has the reader's `--encode-to`, which
  encodes the 1,820 text modules of the scripts and verifies or runs none, and wabt 1.0.39's encoding of
  the same texts with every post-1.0 feature off. All 1,820 are byte-identical. `elem.wast`'s first module
  is handed to wabt as `(elem (table $t) ...)`, because wabt reads the 1.0 spelling `(elem $t ...)` as
  2.0 does. `reader-check/` holds both scripts.
- **The runner**, before any command ran, on a hand-written script of 19 commands with declared verdicts,
  nine of them declared to fail or to be excluded. The lane runs it before every run, and every run in
  this bundle begins with it answering as declared.
- **The collection was taken three times.**
  - The first stopped at the base run's pin check, before any script was read.
  - The second completed. Its architecture suite failed, because the base working tree was still under
    `artifacts/` when rule A14 read every project file there. That run's population A answers were seen:
    they are where section 4's findings and the NaN rows were first noticed.
  - The third, retained here, was taken after only the collection script's step order changed (`a0a8451`).
    The reader, the rule and the product were unchanged, and its two runs of each commit agree line for
    line.

## 6. What this bundle does not demonstrate

- **Clauses 4 and 5.** The decision is NOT MET.
- **The WebAssembly profile's oracle.** Milestone WA-4's sharding, per-shard self-check, scope manifests,
  per-family totals and failure queue are not built. The floor here is the programme's, for this
  comparison.
- **The licence confirmation.** The notice row landed with the suite. The release owner's confirmation
  that the core's third-party claim stays scoped is recorded as owed.
- **A second architecture, and review.** One RID, `linux-x64`, and nothing here has been read by anyone
  but its author.

## 7. What was run

```text
python3 docs/evidence/ubc-4-005/collect.py --rid linux-x64 --wabt <directory holding node_modules/wabt 1.0.39>
python3 docs/evidence/ubc-4-005/classify.py <the extracted test/core> docs/evidence/ubc-4-005
python3 eng/ubc-bundle-manifest.py --bundle docs/evidence/ubc-4-005 --milestone UBC-4 --evidence-class conformance-parity ...
```

`collect.py`'s header says what each file is. `compare.py` joins the two runs and classifies nothing, and
`classify.py` names each difference's class and re-bases the floor. The manifest names every source by
its git blob and every input and retained file by hash. Logs hide the checkout's path as `<root>` and
the scratch directory as `<scratch>`.

## 8. Environment

The collection ran on:

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3;
- Node.js with wabt 1.0.39 from npm, for the reader check only;
- clang 18 with lld, for the Native AOT link.

The base was built in a working tree of this repository at `a56180b`, created and removed by the
collection.
