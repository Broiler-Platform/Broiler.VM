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

**State, 2026-10-16, at SJL-7.4: the base is taken, the vectors are at corpus revision 19, and the cut-over has
landed (section 4); nothing here is a verdict.**
- Parts A and B's bases were taken at the commit that carries the rule, `7b44dc7`, before any code of SJL-7
  lands. Section 2 records them.
- Part C's base arm is built from the same commit when the after arm exists, because the rule interleaves
  the arms inside each repetition.
- The after runs follow SJL-7.4's cut-over, at SJL-7.6.

*(Revised 2026-10-16, at SJL-7.4. The heading read "at SJL-7.3: the base is taken, the engine-level vectors are
run and the pin is at corpus revision 19, and nothing here is a verdict.")* *(Revised 2026-10-16, at SJL-7.3, when the vectors were run. The heading read "at SJL-7.2: the base is taken, and
nothing here is a verdict.")* *(Revised 2026-10-16, at SJL-7.2, when the base was taken. The state read: "rules and correspondence only,
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

[`area-manifest.json`](area-manifest.json) lists the digest of every file of the base and of the vector runs
(section 3), the collector, the rule and the correspondence.

## 3. Engine-level vectors on the Unicode area (SJL-7.3)

Broiler.JS's corpus revision 18 (`b69eb709`) adds the Unicode area: 37,738 vectors in five files. This
profile's conformance root ran on it, and every vector matched. Revision 19 (`c976cbea`) classifies both engines'
first runs. It holds none of this profile's differences, since there were none, and 3,678 of Broiler.JS's. The
pin then moved to revision 19, in `d81c0c1`, by its README's procedure: the archive made twice,
byte-identical, its 42 files hashed after extraction.

| Run | Commit | Result | Files |
|---|---|---|---|
| Revision 18 | `286fd23`, whose product sources are `24632b1`'s, taken 2026-10-16 from 17:43Z with the pin [`vectors/revision-18.pin`](vectors/revision-18.pin) beside a scratch copy of its archive, which `git archive` of `b69eb709` makes again | 88,465 vectors: 88,465 match, 0 classified divergences, 0 unclassified differences | [`run-revision-18.log`](vectors/run-revision-18.log), [`run-revision-18.tsv.gz`](vectors/run-revision-18.tsv.gz) |
| Revision 19 | `d81c0c1`, clean, from 18:35Z, on the committed pin | 88,465 vectors: 88,465 match | [`run-revision-19.log`](vectors/run-revision-19.log), [`run-revision-19.tsv.gz`](vectors/run-revision-19.tsv.gz) |
| Its negative control | the same, with `--mutate unicode/casing-upper/0001` | 88,464 match, 1 unclassified difference, the mutated vector | [`run-negative-control-revision-19.log`](vectors/run-negative-control-revision-19.log), [`run-negative-control-revision-19.tsv.gz`](vectors/run-negative-control-revision-19.tsv.gz) |

**What the vectors cannot show of this profile's corrections.** The area verdict names three: the casing
ceiling, the normalization ceiling and the segmenter's charge, each on the whole result (rows C12, N6 and G5).
None is visible in a vector of an ordinary length. Broiler.Unicode's reference-arm comparison holds the first
two against a copy of this profile's code, and part B's fuel parity holds the units.

**The record.** That comparison's first run found differences of Broiler.JS that this bundle's correspondence did
not name. It found none of this profile's. Rows C2, C3, C6 and N1 are revised in [`9b031e9`](correspondence.md),
each with a dated note, and so is JSD-0074's list of Broiler.JS's data-policy rows.

## 4. The cut-over (SJL-7.4)

[JSD-0075](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0075-the-profile-on-broiler-unicode.md) records
it. The changes, each building, with the tests it touches green:

| Commit | What it does |
|---|---|
| `093feb8` | The owner's generator reaches the architecture tests, test-only, under a dated revision of rule I1; rule I4 holds this family's Unicode pin to the owner's and each admitted assembly's Unicode version to the pin's |
| `0271008` | The lookups, lexical classes, folding, properties of strings and segmentation move to the owner. Format, the runtime, the compiler and the Intl data reference `Broiler.Unicode.Algorithms`, and the Intl data alone `Broiler.Unicode.Segmentation`. The closures of every JavaScript root in three modes ([`cutover/`](cutover/README.md)) and bundle JS-10-007's pack land with them |
| `b0ac6e2` | Normalization and case mapping move to the owner, and Soft_Dotted leaves the Intl data |
| `8aa88db` | Rules N18 and N23 read the Intl data assembly, and say where the moved code is held |
| `d4f7d54` | JSD-0075, the record |
| `8e9e411` | The committed feed holds the three packages as Broiler.Unicode's Publish workflow pushed them to nuget.org (JSC-310), from the commit the copies before it were packed from |

**This family's three corrections, and their consequence.**
- **C12 and N6.** The casing and normalization ceilings are checked on the whole result.
- **G5.** Segmentation charges every code point it reads.
- **The consequence:** a text longer than the ceiling is refused even when the operation would leave it unchanged.
  Such a text is reachable only by concatenation.

JSD-0075's section 6 lists each, with the cells it checked.

**At `8e9e411`, from a clean tree, with the packages as published:**
- **The shared vectors at corpus revision 19 match, every one:** 88,465 vectors, 88,465 match, 0 classified
  divergences, 0 unclassified differences
  ([`vectors/cut-over-revision-19.log`](vectors/cut-over-revision-19.log),
  [`vectors/cut-over-revision-19.tsv.gz`](vectors/cut-over-revision-19.tsv.gz)).
- **Part B's twins and probes keep their base figures.** Each of the ten wide twins and the three Unicode probes
  completes at the figure section 2's table gives, with its answer, and is refused on fuel at one less, in both
  forms. [`cutover/fuel-parity.log`](cutover/fuel-parity.log) is identical to the base's
  [`base/fuel-parity.log`](base/fuel-parity.log) line for line. Part B's after run is SJL-7.6's, at the commit
  this bundle names then. This is the same check, taken at the cut-over.
- **The tests pass:** 383 architecture tests and 473 contract tests, after a locked restore.

**Also run at the cut-over's own commits** (JSD-0075, section 7):
- **The vectors and the twins:** at `0271008`'s and `b0ac6e2`'s trees, with the copies packed locally.
- **The CLI acceptance suite:** 375 of 375.
- **The differential probes.** One probe fails, a `Math.cbrt` digit, which fails identically at the base and is
  not this area's.
- **A local test262 subset over the area's directories:** the same verdict as the base on every variant.

SJL-7.6 judges parts A and B; this section judges nothing.
