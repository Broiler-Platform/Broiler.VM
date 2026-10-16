<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-7-001 - the Unicode area: Broiler.VM's verdict, rules and evidence

**Milestone:** SJL-7 of the shared JavaScript libraries programme,
[`sjl-7-unicode.md`](../../javascript-shared-libraries/milestones/sjl-7-unicode.md).

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-7.1 before any code of the milestone
existed, with its probes and workloads in `src/tests/forms/unicode`.

**Records:** [JSD-0074](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md),
whose input is [`correspondence.md`](correspondence.md).

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State, 2026-10-16, at SJL-7.2: the base is taken, and nothing here is a verdict.**
- Parts A and B's bases were taken at the commit that carries the rule, `7b44dc7`, before any code of SJL-7
  lands. Section 2 records them.
- Part C's base arm is built from the same commit when the after arm exists, because the rule interleaves
  the arms inside each repetition.
- The after runs follow SJL-7.4's cut-over.

*(Revised 2026-10-16, at SJL-7.2, when the base was taken. The state read: "rules and correspondence only,
2026-10-16, at SJL-7.1. **Nothing here is a verdict, and nothing has been taken.**", with the bullets "The base
of all three parts is taken at the commit that carries the rule, before any code of SJL-7 lands.", "The after
runs follow SJL-7.4's cut-over." and "A collector joins this bundle when there is a run to take.")*

The owner's clauses are judged in Broiler.Unicode's bundle of the same name, and Broiler.JS's in its own. This
bundle reads neither's runs.

## 1. SJL-7's area rule and its correspondence (SJL-7.1)

**The correspondence.** It was read from code in both engines, in Broiler.Unicode and in Broiler.Regex, at the
revisions it names. It numbers every difference the area verdict classifies (P, I, W, F, C, N, S, G), and every
behavioural cell its rows rest on was run in both engines. **This family's own corrections in the area are
three:**
- its casing ceiling is checked on the whole result;
- so is its normalization ceiling;
- its segmenter charges every code point it scans.

The rest of the differences correct Broiler.JS, or are SJL-11's.

**The rule has three parts.**
- **Part A** takes test262 in both forms with the hosted workflow at the base and after the cut-over. It
  admits a moved exhaustion verdict as class C2 on `test/intl402/Segmenter/` alone, where JSD-0074's decision 4
  changes segmentation's charging units, and nowhere else.
- **Part B** holds the wide twins and the Unicode probes to their base figures, with no class.
- **Part C** is the measurement that answers JSD-0031's fifth reason for this engine. Its three workloads
  answer with counts of completed operations, so that no correction can fail its condition.

## 2. The base (SJL-7.2)

[`collect-area.py`](collect-area.py) took it. It reuses bundle `sjl-6-001`'s area collector, and through it
that bundle's import of a hosted run and bundle `sjl-5-001`'s twin bisection, over the wide twins and the
Unicode probes. It judges nothing.

| | |
|---|---|
| Commit | `7b44dc7`, the commit that carries the rule, from a clean checkout ([`base/identity.txt`](base/identity.txt)). Against the default branch after SJL-6's merges, `24632b1`, it adds 10 files outside the documents, all of them part B's probes and part C's workloads in `src/tests/forms/unicode`, which the rule commits. Its product sources are `24632b1`'s, as the rule says |
| Rule | [`decision-rule-area.md`](decision-rule-area.md), `sha256=b4908773...` in the identity |
| Taken | 2026-10-16: part A's base by the workflow from 15:22Z to 15:30Z; part B's base locally from 16:24Z, on the programme's local machine (`linux-x64`, four logical processors, .NET SDK 10.0.401, the native backend `x86-64-sysv`), while other work ran beside it. Nothing in part B is timed |

**Part A's base.** [38923456434](https://github.com/Broiler-Platform/Broiler.VM/actions/runs/38923456434),
attempt 1, was dispatched from `main` at `16a35b2` on 2026-10-16 at 15:22:53Z, with both forms and the commit
`7b44dc7` named by all forty digits, and completed at 15:29:55Z ([`base/hosted/run.txt`](base/hosted/run.txt)).
- The workflow and its driver at the dispatched ref are byte-identical to the rule commit's: `git diff 16a35b2
  7b44dc7` names neither.
- The artifact `test262-evidence` is retained unchanged in [`base/hosted/`](base/hosted/).
  [`base/hosted/imported.txt`](base/hosted/imported.txt) records the digest of the zip as downloaded, which is
  the digest GitHub records.
- Every runner and the merging job ran image `ubuntu24` 20261012.334.1.

| Run | Complete ([`base/completeness.log`](base/completeness.log)) | Totals |
|---|---|---|
| `wide-bytecode` | exit 1, the floor holds; whole, retainable, no shard finding, one assembly list | pass 100,372, fail 154, exhausted 44, skipped 1,153, of 101,723 variants |
| `wide-native` | exit 4, a re-base, as the rule admits; whole, retainable, no shard finding, one assembly list | pass 100,338, fail 188, exhausted 44, skipped 1,153, of 101,723 variants |

The totals are bundle `sjl-6-001`'s after run's, figure for figure, as they should be for the same product
sources.

**Part B's base** ([`base/fuel-parity.log`](base/fuel-parity.log)).
- **The method:** each twin and probe's smallest completing fuel allowance was bisected under the bytecode
  form, then both forms were run at that figure and at one less.
- **The result:** every one completes at its figure and is refused on fuel at one less, in both forms, and
  both forms complete with the same answer. That figure is what part B's after run is held to.
- **The wide twins' figures and answers equal bundle `sjl-6-001`'s after run's.**

| Twin or probe | Figure | Answer |
|---|---|---|
| `wide/array-loop` | 73,082 | 999000 |
| `wide/call-loop` | 28,026 | 1000 |
| `wide/closure-loop` | 31,044 | 1000 |
| `wide/fib` | 7,682 | 144 |
| `wide/generator-loop` | 40,075 | 499500 |
| `wide/numeric-loop` | 23,022 | 498497 |
| `wide/property-loop` | 29,029 | 183125 |
| `wide/string-loop` | 26,083 | 25 |
| `wide/throw-loop` | 34,026 | 1000 |
| `wide/try-loop` | 34,022 | 499500 |
| `unicode/casing` | 328,460 | 179280 |
| `unicode/normalize` | 607,821 | 420519 |
| `unicode/properties` | 14,348,623 | 35050 |

[`area-manifest.json`](area-manifest.json) lists the digest of every file of the base, the collector, the rule
and the correspondence.
