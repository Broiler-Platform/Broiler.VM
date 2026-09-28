<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Evidence bundle UBC-4-006

**Milestone:** UBC-4 of [the universal bytecode programme roadmap](../../universal-bytecode.roadmap.md) -
the WebAssembly family, exit-gate clauses 4 and 5, judged under the dated rule
[`decision-rule.md`](decision-rule.md) in this directory. That rule revises bundle `ubc-4-005`'s by one
class, (n), added after that bundle's runs were seen.
**Collected:** 2026-09-28, at commit `6c01478`, from a clean tree, by `collect.py` in this directory. The
rule's after commit is `6f792fd`. `6c01478` differs from it inside the evidence directories
`docs/evidence/ubc-4-005` and `docs/evidence/ubc-4-006`, and in three record documents: the programme
ledger, the programme roadmap and the WebAssembly profile's status ledger. No product, harness or test
file differs. The base is `a56180b` with the reader's harness files applied, in a separate working tree.
**Core contract version:** 1, unchanged. **Universal bytecode contract version:** 2, unchanged.
**Universal bytecode format version:** 1, unchanged.
**The rule's decision: MET, under a rule revised after the runs it judges were seen.** Read the
qualification with the verdict. Bundle `ubc-4-005`'s rule was committed before any run, and its decision
was NOT MET on one condition. This bundle's rule adds the one class that condition lacked, and it was
written knowing which rows the class would admit. Clauses 4 and 5 are met on this evidence. The
milestone is not accepted: under [`docs/mvp.md`](../../mvp.md) no milestone is accepted while review is
deferred, and nothing here has been read by anyone but its author.

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle | UBC-4-006 |
| Milestone | UBC-4, exit-gate clauses 4 and 5 |
| Rule | [`decision-rule.md`](decision-rule.md), committed in `ea2227f` after bundle `ubc-4-005` was retained in `66fc0a1`, on the owner's decision of the same day. It quotes `ubc-4-005`'s rule whole and adds class (n) |
| Suite | `test/core` of `WebAssembly/spec` at `977f97014c962f7bd1291fcc6d28b41a924882bf` (tag `wg-1.0`), pinned at [`src/tests/wasm/spec/`](../../../src/tests/wasm/spec/README.md) |
| Reader | The harness root's `--spec` lane at `6f792fd`, unchanged since `ubc-4-005` |
| Base / after | `a56180b` with the reader applied as a harness-only change / `6f792fd` |
| Compositions | `Broiler.VM.Composition.WebAssembly.Harness`, never advertised |
| RID | `linux-x64` |
| Owner | the WebAssembly profile owner; one person holds every role |
| Reviewer | none |
| Evidence class | conformance parity under a revised rule: two runs of the specification's scripts compared per command, and two runs of the harness root's own population compared per check, with a negative control; no measurement of any kind |

## 2. The decision, condition by condition

| The rule's condition | Holds | Where it is shown |
|---|---|---|
| Both runs of population A taken, each twice, and not void | yes | `determinism.log`. `base-run.log` and `after-run.log`: 19,262 commands each from 73 scripts. Both print, command line aside, exactly the lines bundle `ubc-4-005`'s runs printed |
| The run after prints the same lines in three publish modes | yes | `after-modes.log`, `publish.log` |
| Every difference in population A is in a class, and named | yes | `comparison.log` lists 6,492 differences, and `classes.log` names each. **5,060 are class (f)**: the retired interpreter's float-comparison routing defect at the base and the specification's value after, each command acting on a module whose code has a float comparison. **1,432 are class (n)**: section 3. None is in no class |
| Every difference in population B is in a class, and named | yes | `comparison.log`: the 434 members present at both commits answer identically |
| The negative control retained failing and then passing | yes | `control-failing.log`: with `control.patch`, obligation E2's lane agrees on 0 of the 12 float comparisons, naming each, exit 1. `control-passing.log`: after the revert, 12 of 12, exit 0 |
| The ratchet re-based by hand with the classes named | yes | `base-verdicts.txt` is the floor from the base, and `floor-after.log` shows the run after holding all 13,309 commands it passes. `floor-rebased.txt` is the floor re-based to the run after, its 5,060 moved commands each carrying `(f)` |

**No verdict gets worse.** 13,309 commands pass at both commits, 5,060 fail at the base and pass after,
463 fail at both, and 430 are excluded as class (t).

## 3. Class (n), and what to check it against

Class (n)'s 1,432 commands are the canonical- and arithmetic-NaN assertions of:

- `f32.wast` (686);
- `f64.wast` (686);
- `float_exprs.wast` (53);
- `conversions.wast` (6);
- `float_misc.wast` (1).

Each passes at both commits. At the base each answered the NaN its arithmetic carried over: 535 negative
canonical NaNs, 461 positive NaNs with other payload bits, and 436 negative ones. After, each answered
exactly the positive canonical NaN of its width. The cause is the family's primitive table, which sets
the NaN-canonicalisation flag roadmap work package UBC-4.3 names.

`classes.log` names each command, and `comparison.log` shows its two answers. A reader who doubts the
class was drawn fairly can check every row against the class's own bounds:

- one NaN value of one width at both commits;
- the verdict passing at both;
- the answer after exactly the positive canonical NaN.

`classify.py` applies those bounds and nothing looser.

## 4. What this bundle does not demonstrate

- **A predeclared verdict.** The decision that was predeclared is `ubc-4-005`'s, and it is NOT MET. This
  bundle's MET rests on a rule whose one added class was written to describe rows already seen. It is
  reported that way wherever it is reported.
- **Conformance.** Population A's answers are parity evidence, not a conformance claim. The profile's
  standing gaps are listed in `ubc-4-005`'s section 4, and they are the same here:
  - custom-section names not checked as UTF-8;
  - multi-result function types admitted;
  - `call_indirect` type mismatches;
  - imports refused;
  - the reader's one known gap, `inline-module.wast`.
- **The WebAssembly profile's oracle (WA-4), the licence confirmation, a second architecture, review.**
  As `ubc-4-005`'s section 6.

## 5. What was run

```text
python3 docs/evidence/ubc-4-006/collect.py --rid linux-x64 --wabt <directory holding node_modules/wabt 1.0.39>
python3 docs/evidence/ubc-4-006/classify.py <the extracted test/core> docs/evidence/ubc-4-006
python3 eng/ubc-bundle-manifest.py --bundle docs/evidence/ubc-4-006 --milestone UBC-4 --evidence-class conformance-parity ...
```

The tooling is `ubc-4-005`'s, pointed at this directory. `classify.py` also knows class (n), and
`base/ScriptVerification.cs` is byte-identical to `ubc-4-005`'s copy. Logs hide the checkout's path as
`<root>` and the scratch directory as `<scratch>`. The manifest names every source by its git blob and
every input and retained file by hash.

## 6. Environment

As bundle `ubc-4-005`'s section 8:

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3;
- Node.js with wabt 1.0.39, for the reader check;
- clang 18 with lld, for the Native AOT link.
