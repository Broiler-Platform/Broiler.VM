# Bundle JS-ANDROID-010 — the rule-message report over every group whose rules are functions

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** [JS-ANDROID-009](../js-android-009/README.md) built a reporter that writes
what a rule *said* rather than only whether it went red, and its first exclusion was the scope:
*"Group K only. A report over another group needs that group's inputs and is a separate piece of
work."* That work is done for **every group whose rules are expressible as functions** — and the
groups where they are not are named rather than left as an absence.

---

## 1. Coverage, and the line that decides it

**46 of the register's 76 rules are reported.** The split is not a matter of effort:

| Reported | Why |
|---|---|
| **A** (A1–A6, A8–A13), **B** (8), **K** (4), **N** (N1–N9, N11), **V** (12) | each rule is a **function returning the messages it would report** — `ArchitectureRules.A1`, `ApiBaselineRules.V5`, `CompositionRules.K2` — so a reporter can call one and write the answer down |

| Not reported | Why |
|---|---|
| **C** (3), **D** (1), **E** (5), **H** (5), **J** (11), **L** (1), **M** (1), plus **A7**, **A14**, **N10** | these assert **inline**: the rule *is* the test body, with local helpers and assertions rather than a function returning messages. **There is nothing for a reporter to call** |

Giving the second group a report means extracting a function from each test — a refactor of the
tests themselves, carrying the risk that the extracted function and the test drift apart. That is
the exact defect this reporting idea exists to prevent, so it is a separate piece of work with its
own argument to make, and it is named here rather than left for a reader to discover by counting.

## 2. What the mechanism guarantees

- **It reports and does not judge.** The tests assert the rules are silent; this writes what they
  say. A control's verdict is still the suite's exit code.
- **It runs only when asked**, through `BROILER_RULE_MESSAGES` naming a **directory** — one file
  per group, because xunit runs test classes in parallel and one file would interleave.
- **It asserts it wrote something** when asked, per group. A report that was asked for and wrote
  nothing would read to a control as a silent rule rather than a broken harness.
- **Each report mirrors its tests' inputs exactly.** Where a test sweeps every project, so does
  the report; where it names three assemblies, so does the report.
- **A rule that throws is recorded as throwing.**

## 3. Results

`clean-A.txt`, `clean-B.txt`, `clean-K.txt`, `clean-N.txt`, `clean-V.txt`: the five reports over a
clean checkout, unedited. **Every one of the 46 rules said nothing**, which is what the green suite
asserts in its own way and what gives the report a floor — a reporter that could not produce
silence would be reporting something other than the rules.

Two injections were run against it and are described rather than retained, because their artefacts
are a doctored checkout:

- the **N7** control — a registry row naming a case the corpus does not have — produced
  `registry row 1401 names the case an-opcode-nobody-wrote-a-case-for, and no corpus entry of that
  name records code 1401`, in group N and nowhere else;
- the **N4** control — a family project declaring a `PackageId` — produced messages in **two**
  rules at once: `A6` on the undeclared package identity and `N4` on the family's own prohibition.
  A6 was not what that control's row names, and no exit code would have said so.

## 4. Exclusions — what this bundle does not show

1. **Thirty rules have no report**, for the structural reason in section 1.
2. **A message is what a rule said, not proof that the rule is right.** The report quotes; it does
   not check.
3. **The clean reports are this checkout on this machine.** They are a floor, not a measurement.
4. **No control matrix was run for this bundle.** The two injections above were spot checks of the
   mechanism, and the retained control logs live in JS-ANDROID-009 and earlier.
5. **Nothing here runs on a device**, no milestone moves, and nothing is reviewed.
