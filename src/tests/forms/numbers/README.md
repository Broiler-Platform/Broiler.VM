<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The number-conversion workloads the shared libraries pilot is measured over

These are the workloads of part C of
[`docs/evidence/sjl-3-001/decision-rule.md`](../../../../docs/evidence/sjl-3-001/decision-rule.md):
Broiler.VM's own measurement of the operations milestone SJL-3 of
[the shared JavaScript libraries programme](../../../../docs/javascript-shared-libraries/README.md)
moves into a shared library. They were committed with that rule, at milestone SJL-0, before any
code the rule judges exists. They compile under **`broiler.javascript.wide`**, the command-line
host's default manifest, and run in both output forms.

**No figure produced from them is retained anywhere in this repository except in an evidence
bundle**: `sjl-0-001` holds the base arm measured alone, and SJL-3's bundle holds the comparison. A
figure from them in a plan, a ledger, a decision record or a remark is a defect in the change that
put it there. Each workload's answer is not a figure in that sense: it is the condition a lane is
checked against, and the bundles record it.

## What each workload prices

| Workload | Control | What it prices |
|---|---|---|
| `to-string.js` | `to-string.control.js` | Number to string: `String(x)`, `toString` at radixes 2, 7, 16 and 36, `toFixed`, `toExponential` and `toPrecision`, over every value class the conversion branches on |
| `to-number.js` | `to-number.control.js` | String to number: `Number(s)`, `parseFloat`, and `parseInt` at radixes 10 and 36, over strings covering the grammar's branches and the whitespace set |
| `literals.js` | `literals.control.js` | The compiler's numeric literal values: a body repeating every literal form, compiled again on every iteration through the `Function` constructor |

**Each control is the same workload minus the thing measured**, which is the first rule of the
core's [baseline register](../../../../docs/baselines.md#1-what-a-measurement-is-and-what-it-is-not):
the same values and loops with each conversion replaced by a comparison or a length read, and the
same body compiled the same number of times with every literal written as `0`. A difference between
a workload and its control is the operations' cost; a difference between two different programs
would be a comparison, not an attribution.

## How they are run

Every workload is deterministic, takes its iteration counts as literals, and ends in an expression
statement whose value is its answer. The collector of bundle `sjl-0-001` runs each one and its
control through the command-line composition root, in both forms, under the measurement rules the
core's baseline register and the JavaScript profile's
[roadmap section 17](../../../Broiler.VM.Profile.JavaScript/docs/roadmap.gates.md#17-measurement-discipline)
state: interleaved lanes, an A/A lane, every repetition retained, the answer checked before and
after every lane, and each child's effective configuration recorded and compared.

**The workloads are not changed after they are committed.** A change would change the population the
rule was written over, and the rule's own section on revisions says what that costs.
