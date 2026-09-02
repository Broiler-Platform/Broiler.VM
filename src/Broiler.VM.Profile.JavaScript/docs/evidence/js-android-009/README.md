# Bundle JS-ANDROID-009 — the rules' own messages, so a clause is observed rather than argued

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** The limitation four bundles carried, retired. From
[JS-ANDROID-006](../js-android-006/README.md) onward each said some version of: *"the clause
attribution is an argument, not an observation — the suite reports that K2 went red and no more,
because xunit prints an empty-collection assertion without printing the collection."*

**A harness reports the rules' own messages now**, and this log carries them for all twelve
controls over the composition register and the Android head's declared artefacts.

---

## 1. What was built

`RuleMessages_Are_Written_When_A_Report_Is_Asked_For`, in the test class that already holds group
K's inputs, so the report is over **exactly** what the four tests compare rather than over a second
copy of them.

**It reports and does not judge**, which is why it can sit beside those tests without duplicating
them: they assert the rules are silent, and it writes down what they say. Nothing in it can make a
defect pass, because nothing in it is what a defect has to get past — the control's verdict is
still the suite's exit code.

It runs only when `BROILER_RULE_MESSAGES` names a file, so an ordinary suite run neither writes
anything nor pays for it. When it is asked, **it asserts that it wrote something**: a reporter that
silently produced nothing would be worse than absent, because the control reading its output would
report no messages and look like a clean rule rather than a broken harness.

**A rule that throws is recorded as throwing** rather than crashing the report — two of these four
threw on a phantom register row until [JS-ANDROID-008](../js-android-008/README.md).

## 2. What it shows, in the rules' own words

Every clause each control claimed to reach is named in that control's messages. Some examples the
argument could not have settled:

- the reserved-label control produces **two** messages — the package identities *and* `composes
  broiler.javascript, which claims the reserved first label, under package identity
  'Com.Example.Ledger'` — which is exactly the pairing that clause is about;
- the duplicate control produces `composes broiler.javascript 2 times`;
- the different-profile control produces **three**, including both the referenced-but-undeclared
  and the declared-but-unreferenced clauses, which is more than its own row names.

**That last one is the point.** The log shows several injections reaching more clauses than their
rows claim. The rows were right about what they targeted and incomplete about what else they
touched, and nobody could have known which from an exit code.

## 3. Results

`composition-controls.log`, unedited. **Twelve run, twelve passed**, each with the rules' messages
beneath it.

**One row has no messages, and that is correct**: `the-register-row-loses-a-cell` makes the
register parser stop the read, so every rule that would have spoken — the reporter included — has
nothing to speak about. A parser that refuses to hand out a row nobody wrote silences the rules
downstream of it by construction.

## 4. Exclusions — what this bundle does not show

1. **Group K only.** The four rules here take their inputs from one test class's helpers; a report
   over another group needs that group's inputs and is a separate piece of work. The other
   seventeen controls in `CONTROLS` still have only an exit code behind them.

   > **Correction, 2026-09-02.** Done, for every group whose rules are functions:
   > [Bundle JS-ANDROID-010](../js-android-010/README.md) reports **46 of the register's 76 rules**
   > across groups A, B, K, N and V. The other thirty assert inline - the rule IS the test body -
   > so there is nothing for a reporter to call, and that bundle names them rather than counting
   > them out. **Nothing in the log below is edited.**
2. **A message is what a rule said, not proof that the rule is right.** The report quotes; it does
   not check. A rule whose message is misleading would produce a misleading line here.
3. **This is a SCOPED run**, twelve rows of twenty-nine, not the control matrix.
4. **Nothing here runs on a device**, no milestone moves, and nothing is reviewed.
