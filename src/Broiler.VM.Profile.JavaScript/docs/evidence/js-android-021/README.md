# Bundle JS-ANDROID-021 — controls for the quotation exemption, and the escape hatch made visible

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** [JS-ANDROID-020](../js-android-020/README.md) gave rule J12 a quotation
exemption and covered it with a **witness test** — a fixture the rule is driven over. Three bundles
in this chain have already recorded why that is the weaker shape: it shows the function behaves,
not that the function is wired to the document it governs. These are injections into the **real
register**.

**Three are controls. The fourth is not, and says so.**

---

## 1. The three controls

The exemption needs a code span **and** an attributing verb. Each control supplies one half and
must still be reported.

| Control | Suite | What J12 said |
|---|---|---|
| `J12-a-code-span-with-no-verb-is-still-read` | failed | `D1 counts a subject the catalog computes without citing it: "45 covered source files"` |
| `J12-a-verb-with-no-code-span-is-still-read` | failed | the same |
| `J12-a-live-claim-inside-the-window-is-still-read` | failed | the same |

**The third is about the implementation, not the design.** It puts a live figure *between* the verb
and the span it attributes — `The row said 45 covered source files were compared, quoting `the old
wording` exactly.` Blanking the whole match rather than the quoted group alone would swallow it,
and the rule would be silently narrower than its own documentation says.

## 2. The fourth is a limit demonstration, not a control

`J12-a-quoted-live-claim-is-NOT-reported` injects a live figure a row chose to present as a
quotation. **The suite stays green on a stale number.**

That is the exemption working exactly as designed and equally the abuse it makes possible.
JS-ANDROID-020 stated it as exclusion 1 in prose; it is measured here. **Labelling matters:** an
injection that is deliberately not caught is a demonstration of a stated limit, and calling it a
control that passed would be the kind of claim this component treats as a stop condition.

## 3. What the fourth control changed

Silence was the wrong answer to a measured escape hatch: a reviewer had no way to see that anything
had been exempted at all. **The report now lists what the exemption let through**, on its own line:

```
[J12] 0 message(s)

[J12-exempt] 8 message(s)
    the register row for J12 is quoting a figure rather than stating it, so this rule did
    not read it: "THIS RULE IS RED AT THIS MILESTONE, WHICH IS THE CLAUSE WORKING"
    ... "Forty-four units are assessed High or Critical"
    ... "45 covered source files"
    the register row for A15 is quoting a figure rather than stating it, so this rule did
    not read it: "19 projects and 55 edges"
```

**These are not violations and are never counted as any.** J12's own count stays a count of things
wrong; the exempt list is what a reviewer auditing the hatch reads. It is the reporter's job
exactly: say what happened, decide nothing. Re-run, the fourth injection now shows the trace beside
the green suite.

Only quotations carrying something the rule *would* have read are listed — a row quoting a sentence
with no figure in it exempted nothing, and listing it would bury the eight that matter.

## 4. Results

Seventeen reports over a clean checkout; **77 of the register's 78 rules reported**, J12 silent with
eight traced exemptions. Suite: 172 architecture and 207 contract tests, green. Four injections,
four outcomes as expected, none unexpected — `negative-controls.log`.

## 5. Exclusions — what this bundle does not show

1. **The escape hatch is not closed and cannot be.** No rule distinguishes a quotation of a defect
   from a quotation used to smuggle a live claim without understanding what the quotation is about.
   Section 2 measures it; section 3 makes it visible; neither prevents it.
2. **The trace is a report, not a gate.** Nothing fails when a row exempts a figure. A reviewer who
   never reads the exempt list is in exactly the position they were in before this bundle.
3. **Eight exemptions is the current count and will grow**, because every rule minted after one
   that reads sentence shapes will want to quote its own defect. A list nobody prunes becomes a
   list nobody reads.
4. **The trace filter is a judgment.** Only quotations matching what a clause would have read are
   listed, so a quotation the vocabularies do not recognise is exempted and untraced — the same
   reach limit the clauses have, inherited.
5. **All four injections used one figure and one row.** A quotation exempting a figure in a row
   that also states one legitimately was not tried.
6. **A message is what a rule said, not proof that the rule is right.** Nothing runs on a device,
   no milestone moves, and nothing is reviewed.
