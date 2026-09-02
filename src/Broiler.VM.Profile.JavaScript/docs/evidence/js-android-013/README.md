# Bundle JS-ANDROID-013 — controls for J10 and J11, a rule that no exit code can judge, and a register row that was false

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** Controls were asked for on the last two rules the rule-message report did
not cover. Building them established that the reason they were not covered was **a misreading**,
that one of them **cannot be judged by the suite's exit code at all**, and that **J10's register
row states something false about this tree**.

**75 of 76 rules are reported.** Only `E5` remains, and only because it is a Deferred row no test
may assert.

---

## 1. The exclusion was a misreading, in both cases

Three bundles said J10 and J11 "assert their clean direction over a WITNESS input rather than over
this checkout". Both tests assert over a witness **in the middle** and over the checkout **last**:

| Rule | Its last clause | Over |
|---|---|---|
| `J10` | `MissingFalsificationCriteria(ProductUnits)` | the real product units |
| `J11` | `Blockers(AssuranceGenerator.Current)` | the real plan |

The exclusion was written from the witness clause without reading to the end of either test. This
is the third exclusion in this chain that described the shape of a rule rather than its body, and
it is recorded as such rather than repaired quietly.

## 2. J11 is not silent on a clean checkout, and that is the rule working

`clean-J.txt` retains it: **J10 says nothing and J11 says 905 things.** Every relevant unit in this
component is `HUMAN_PENDING`, which the owner's ruling permits, so the release gate has a great
deal to say about this tree, and its own test asserts `NotEmpty` on an ordinary run.

**Silence is therefore not the universal shape of a clean report.** Every earlier bundle could say
"every rule said nothing", and a reader who generalised that would read J11's report as a broken
harness rather than as the list of what a publish is waiting on.

## 3. J11 is the first control here that an exit code cannot judge

Its ordinary-run assertion is `Assert.NotEmpty(Blockers(Current))`. Injecting a blocker turns 905
into 906. **J11's own assertion passes in every control below, clean or injected.** The suite still
went red in all three — through `J5`, `J3` and `J10` — so an exit code says a defect was caught and
says nothing about whether the release gate saw it.

The report is the only instrument that can. This is the case
[JS-ANDROID-009](../js-android-009/README.md) built the reporter for, arriving in a stronger form
than the one it anticipated: there, an exit code could not distinguish *which clause* of K2 an
injection reached; here, an exit code cannot see the rule at all.

## 4. Results

Three controls, in `negative-controls.log`. Each injects into a real product source file, runs the
suite with the report on, and is reverted by copying back a backup — never `git checkout --`.

| Control | Suite | J11's own assertion | What the rules gained |
|---|---|---|---|
| `J10-a-high-unit-loses-its-falsification-criterion` | failed | unmoved | **J10 +1**, naming `VmBoundedAllocator.TryAllocateExact<T>` as `assessed Security=High and carries no '// Broiler-Falsified-If:' line`. J11 +5, J5 moved |
| `J11-a-recorded-fingerprint-is-not-what-the-declaration-hashes-to` | failed | unmoved | **J11 +1**, and **not the clause it was aimed at** (§5.3). J3 and J5 moved |
| `J11-an-assurance-comment-is-attached-to-no-declaration` | failed | unmoved | **J11 +28**, including `an assurance comment that is attached to no declaration` — the clause aimed at. J3 and J5 moved |

**No control fires its rule alone, and the reason is structural rather than sloppy.** The number of
criteria and the shape of every annotation are *generated figures*: `CODE-ASSURANCE.md`,
`HUMAN_REVIEW.md` and each file's own header carry them. Any injection that changes what J10 or
J11 reads therefore also changes what the generator would write, so `J5` moves with it. That is not
a defect in the controls; it is what it means for the assurance record to describe the tree.

## 5. J10's register row was false, and nothing could see it

The row's `nonVacuousWhen` began **"THIS RULE IS RED AT THIS MILESTONE, WHICH IS THE CLAUSE
WORKING. Forty-four units are assessed High or Critical; three of them … carry a criterion, and the
rule names the other 41 one by one."**

The generated report in the same repository says:

```
| Units carrying a criterion | 114 |
| Units required to carry one | 79 |
| Required and missing | 0 |
```

**The rule is green, 79 units are required to carry a criterion, and every one of them does.** The
work the row describes as outstanding was done; the row was not updated.

**Why no test caught it.** `AssertTheRegisterRowIsWhatTheRulesImplement` compares the row in
`rules.register.json` against a hardcoded copy of the same prose in `AssuranceRegisterRows.cs`. Two
copies of a claim agreeing with each other is not the claim being true, and neither copy is
compared to the tree. The row is corrected here, in both places, and the stale text is quoted in
the correction rather than erased — how it survived is the useful part.

This is the same finding as the register parser that re-indexed columns, the cell-count check that
compared a split length, and the disagreement message that omitted its new columns: **a check that
looks like it is checking.**

## 6. Exclusions — what this bundle does not show

1. **`E5` is still unreported**, and is the only rule that is. It is a Deferred row superseded at
   VM-1 whose activation milestone is `never`, and `RuleRegisterTests` requires that no test assert
   it.
2. **No control fires J10 or J11 alone.** Section 4 says why. A reader wanting "this injection
   reached this rule and no other" will not find it here, and cannot: the assurance record is
   generated from the thing being injected into.
3. **The fingerprint control did not reach the clause it was aimed at.** It was written to fire
   J11's third clause — a recorded fingerprint that is not what the declaration hashes to — and it
   reached the **first** instead: the artefact on disk is not what the generator would write.
   `AssuranceGenerator.Plan()` rescans each file "as they will read once the artefacts are
   written", so a hand-edited fingerprint is already repaired before the third clause sees it. **The
   third clause is therefore not reachable by editing a real file at all**, only by a witness plan,
   which is what its test uses. Recorded as a limit of the control rather than presented as a
   success, and the aim was wrong rather than the rule.
4. **J11's gate mode is not exercised.** `BROILER_ASSURANCE_RELEASE=1` asserts the blockers are
   empty, and over this tree they are 905, so the gate is red on a clean checkout by design. No
   control can distinguish an injected blocker from the 905 in that mode either.
5. **The corrected J10 row is prose, and prose is not checked.** The correction states figures that
   were true on 2026-09-02; nothing compares them to the tree on any later day, which is the defect
   in section 5 still standing. Closing it needs a rule that reads the generated figures, and that
   is a rule nobody has minted.
6. **A message is what a rule said, not proof that the rule is right.**
7. **The clean reports are this checkout on this machine.** Nothing here runs on a device, no
   milestone moves, and nothing is reviewed.
