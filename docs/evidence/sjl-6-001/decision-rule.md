<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-6-001's decision rule: the hosted test262 workflow takes the runs the local procedure takes

**Written:** 2026-10-15, at milestone SJL-6 (work package SJL-6.0), with
[`.github/workflows/test262-evidence.yml`](../../../.github/workflows/test262-evidence.yml) and its driver
[`eng/test262-evidence.py`](../../../eng/test262-evidence.py), committed in the same change and before the
workflow's first run. **Milestone judged:** SJL-6 of
[the shared JavaScript libraries programme](../../javascript-shared-libraries/milestones/sjl-6-date.md),
work package SJL-6.0's done-when, and nothing else. **Owner:** the JavaScript profile owner, with the core
architecture owner. **Reviewer:** none. Every role this rule names is held by one person (EX-30), and this
rule does not claim that anyone independent has read it.

**What this file is.** One predeclared decision, of one evidence class, in template T1 of
[the programme's validation document](../../javascript-shared-libraries/validation.md#4-predeclared-comparison-rules):
whether the workflow takes the runs that bundle `sjl-0-001`'s procedure takes on one machine, so that
SJL-6.1's rule may name it for SJL-6's base and after runs. It judges no area and no change of the engine:
the commit it runs at is one whose runs a bundle already holds. **It holds no figure.** Where a count or an
allowance matters, it names the file that fixes it.

**Why the comparison is across machines here, and nowhere else.** A hosted run and a local run spend
the wall clock differently, so the programme compares a hosted run with another hosted run. This rule is
the one exception the work package names: it compares the two machines' runs of one commit in order to
classify their differences, not to judge an engine by them.

## The check (template T1)

```rule
evidence class: conformance parity between two machines running one commit (not a measurement)
population: the pinned suite under the wide manifest, in the bytecode form and in the native form, at 72ad941e1c3248780af8231e548c3656b0984f99, bundle sjl-5-001's after commit, whose local runs of both forms that bundle retains (sjl-5-001/after/wide-bytecode.report.gz and sjl-5-001/after/wide-native.report.gz), taken by bundle sjl-0-001's procedure
configuration: .github/workflows/test262-evidence.yml and eng/test262-evidence.py as committed at the ref the dispatch runs from, which the artifact's run.txt names and which is this rule's commit or a descendant of it, dispatched once at that commit with both forms; its artifact test262-evidence imported unchanged into sjl-6-001/hosted/, with the digest of the zip as downloaded
completeness: each hosted form's run is whole and retainable: every shard the workflow's partition names reported, the run log's exit code (eng/run-test262.py's rule, applied by the driver) is 0, 1 or 4, the merged report's retention object says retainable, and every runner and the merging job built the same assemblies (one distinct list in the form's binaries log)
comparison: per variant, joined on path and variant, the local run's verdict and exhausted dimension against the hosted run's, through eng/compare-test262-forms.py --same-form with the local report as control and the hosted report as candidate, the bytecode pair as it is and the native pair with --exempt-guest-loads, as bundle sjl-5-001's part A compares its pairs
admitted difference classes: the script's own under --same-form: (c') a wall-clock exhaustion on either side, against a pass, a failure or an exhaustion of another allowance; and, for the native pair only, (a) a code-size refusal on either side and (b) fuel or nested-load bytes spent on a guest-load variant on either side
every other difference is unclassified
MET if and only if both hosted runs are complete as above, and the comparison of each pair exits 0, every difference in an admitted class.
NOT MET otherwise.
```

**How it is read.** The bundle retains the hosted artifact, the two comparisons' logs, and a table of
the differences by class. A (c') row is not run again: this check asks whether the workflow takes the
runs the procedure takes, and a row that ran out of wall clock on one machine and not on the other is a
race between the machines by construction. Totals do not decide it.

**If it is NOT MET.** SJL-6.1's rule does not name the workflow, and SJL-6's runs are taken locally as
SJL-5's were, until a later dated record says what was wrong and a new run of this check is MET.

## Revisions

A rule revised after any run it judges has been seen is a new dated file in this directory, quoting
this one whole and saying it was written afterwards; its decision is then weaker evidence.
