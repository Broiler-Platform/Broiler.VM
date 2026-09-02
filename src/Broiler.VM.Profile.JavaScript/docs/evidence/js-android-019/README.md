# Bundle JS-ANDROID-019 — the ADR sweep, a budget paragraph I did not write, and rule A15

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** [JS-ANDROID-018](../js-android-018/README.md) swept the ledgers and the
bundles and recorded, as exclusion 1, that *"ADR prose was not swept… The ADRs are the third and
they are the ones the register cites as authoritative."* This is that sweep.

**It found one defect, and I made it earlier in this same session.**

---

## 1. An ADR is a third kind of document

Not a register row and not a dated bundle. An ADR is **revised in place** — ADR 0001 is on revision
5 — so it is live, and a figure it states that the tree contradicts is a live defect.

But most of its figures are **the decision itself**. `exactly three packages` is not a measurement
that can go stale; it is the budget the record sets, and rule C1 asserting the tree matches it is
the record working. The sweep flags 95 figures across twelve ADRs and eleven decision records and
**cannot tell those two apart** — `ADR 0005 rules on nowhere by name` is parsed as five rules, and
`100 units` in ADR 0007 is a fuel budget. The log keeps every flag; the classification was reading.

## 2. The defect: a budget paragraph that was never written

ADR 0001's budget section is what authorises a project set at all — it says the set *"may not grow
without a dated revision"* — and **every project-adding revision states the growth**:

> The graph goes from 17 projects and 46 edges to 19 and 55.

The revision that added `Broiler.VM.Composition.JavaScript.Android` on 2026-09-02 states what the
head composes, why it is an application rather than a console program, and what it does not settle.
**It carries no budget paragraph.** The record's last stated size was 19 projects and 55 edges; the
graph held **20 and 59**.

**I wrote that revision, in this session, five bundles ago.** The project is authorised — the dated
revision exists and names it — and the arithmetic every other revision carries is missing.

## 3. Why nothing caught it, which is the familiar shape

Rule **A7** holds `graph.manifest.json` to the project files. **Both of those are the tree.** A
manifest and a graph agreeing tells you nothing about a document describing them. The budget
sentence was prose no rule read — rule J10's register row, one document over.

## 4. Rule A15

The record's **last** budget sentence states the project and edge counts the graph holds now.
Earlier sentences are history — *"goes from 8 projects to 12"* was true at VM-3 and must stay
written — so a rule reading all of them would demand the record forget what it recorded.

**A record stating no budget at all is reported rather than passing.** That clause is not defensive
tidiness: the failure this rule was minted for was an *absent paragraph*, and a rule reading "no
sentence, nothing to disagree with" would have been green on exactly the tree that produced it.

The missing revision is **appended, not folded into the one above it**. That revision says *"Every
revision above stands as written"*, and a record whose author edits yesterday's entry when today's
sweep finds it short is a record with no history — even when yesterday was this morning and the
author is the same.

## 5. Results

Seventeen reports over a clean checkout; **77 of the register's 78 rules reported**, A15 silent.
Suite: 171 architecture and 207 contract tests, green.

**Three controls**, in `adr-sweep.log`, over the real record:

| Control | Suite | What A15 said |
|---|---|---|
| `A15-the-budget-revision-is-missing` | failed | `states 19 projects and the graph holds 20` **and** `states 55 edges and the graph holds 59` |
| `A15-the-edge-count-alone-is-wrong` | failed | `states 57 edges and the graph holds 59` |
| `A15-the-budget-states-projects-and-no-edges` | failed | `states a project count and no edge count` |

The first is **the exact state the record was in when this rule was minted**, and the suite was
green on it.

## 6. A pattern worth naming: a rule cannot describe its own defect

A15's register row fired rule **J12** on first write, for quoting *"19 projects and 55 edges"*. That
is the **fourth** time a rule's own row has tripped a rule in this chain — J12's row did it twice,
J10's once, now A15's.

It is not an accident and not a bug in either rule. **A rule that forbids a sentence shape cannot
use that shape to explain why it exists**, and the register's rows are where this component explains
why rules exist. The repair has been reported speech every time. If a fifth appears, the thing to
reconsider is the recogniser, not the prose.

## 7. Exclusions — what this bundle does not show

1. **A15 reads two figures**: the project count and the edge count. Everything else a budget
   paragraph says — which projects, which kind, why, whether the packable set moved — is prose, and
   a revision that added a fourth packable assembly while stating the right totals would satisfy
   this rule completely. That is EX-103; rule A6 holds the packable set.
2. **It reads a sentence shape.** A revision wording its budget differently states no budget as far
   as A15 can tell. The failure mode is a false report rather than a missed one, which is the safe
   direction and is still a cost.
3. **The other 94 ADR figures were classified by reading, not by rule.** Nothing here stops an ADR
   figure outside the budget sentence from going stale, and the decision records were swept and
   found clean by the same unaided judgment.
4. **No ADR figure was converted to a citation.** ADRs are read by people outside the test
   assembly, and a `{graph:edges}` token in a decision record trades a reader's comprehension for a
   guarantee. The budget sentence is checked instead of rewritten.
5. **Three documents were named for sweeping and three are now swept**, but the sweep vocabulary is
   the same fifteen subjects throughout, so the same reach limit applies to all of them.
6. **A message is what a rule said, not proof that the rule is right.** Nothing runs on a device,
   no milestone moves, and nothing is reviewed.
