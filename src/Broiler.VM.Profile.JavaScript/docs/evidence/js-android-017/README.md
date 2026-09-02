# Bundle JS-ANDROID-017 — the sweep for figures bound to a noun, and the assurance family it found stale

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** [JS-ANDROID-016](../js-android-016/README.md) found rule H1's row saying
*"the two evidence-bundle READMEs"* where there are dozens, and recorded that **rule J12's count
clause never saw it** — the figure was bound to a noun rather than to the word *exist*. It was
found by reading, which is not a method, and its exclusion 2 said so: *"There may be others;
nothing here swept for them."*

**The sweep was asked for. There were others: the entire assurance figure family.**

---

## 1. What the sweep found

169 figures across 46 rows are bound to a noun. Most are a rule describing **itself** — clauses,
rounds, witnesses, copies, halves — and do not move when the checkout moves. Filtering to subjects
the tree computes left 25 sites, and the stale ones share one cause:

| Row(s) | Said | The tree holds |
|---|---:|---:|
| H3, J5, J6 ×2, J7, J8 ×2, J9 | **45** covered source files | **61** |
| J5, J8, J9 | **48** artefacts | **64** |
| J2, J3 | **689** annotations / annotated units | **905** |
| J3, J7 | **903** exempt | **1082** |
| J3, J7 | **1,592** units in the tree | **1987** |
| H2 | **five** review documents | **34** |

**One event caused all of it.** The JavaScript profile came under assurance coverage; the covered
set grew from 45 files to 61 and every figure derived from it moved. Eight rows went on stating
the old numbers, and the register's row-equality test was green throughout, because each row
agreed with its own hardcoded copy.

## 2. The fifth clause, and the discipline that keeps it repairable

A figure standing immediately before a **countable subject** — `covered source files`, `units`,
`artefacts`, `product assemblies`, `edges` — must be a citation.

**Every subject the clause names has a figure behind it**, asserted by
`J12_Every_Countable_Subject_Has_A_Figure_Behind_It`. That is not decoration: a subject with no
metric would make the rule report a sentence nobody could repair, and a rule that has to be
suppressed rather than obeyed is worse than no rule.

**What is deliberately absent** is every noun the register counts while describing a rule rather
than the tree. `Four clauses have witnesses of their own` is prose about design; demanding a
citation for it would be asking the register to stop explaining itself.

## 3. Three sentences reworded rather than cited

- J7's `the four units and their four fingerprints` counted a **witness set**, not the tree.
- J8's `a unit set whose three annotations are chosen` counted a **chosen subset**; it reads
  `a three-annotation unit set` now, which is more accurate prose as well as a shape the clause
  does not read.
- **J12's own row** quoted the stale figures it forbids and fired on itself — for the second time
  in this rule's short life. It states them in reported speech now.

## 4. Results

Sixteen reports over a clean checkout; **76 of the register's 77 rules reported and J12 silent**.
Suite: 165 architecture and 207 contract tests, green.

**Four controls**, in `negative-controls.log`, **each an injection of a sentence the register
actually carried**:

| Control | Suite | What J12 said |
|---|---|---|
| `J12-a-covered-file-count-is-typed-back-in` | failed | `J6 counts a subject the catalog computes without citing it: "45 files"` |
| `J12-an-artefact-count-is-typed-back-in` | failed | the same for J5's `"Forty-eight artefacts"` |
| `J12-an-annotation-count-is-typed-back-in` | failed | the same for J2's `"689 annotations"` |
| `J12-an-assembly-count-that-is-correct-today` | failed | `"three product assemblies"` — **which is right today** |

## 5. Exclusions — what this bundle does not show

1. **The sweep covered the REGISTER only.** The same shape lives in evidence bundles, in the
   status ledger and in ADR prose, and nothing here looked at any of them. A figure in this
   bundle's own text is unchecked.

   > **Correction, 2026-09-02.** [Bundle JS-ANDROID-018](../js-android-018/README.md) sweeps the
   > two ledgers and all 27 bundles. **Three live claims were stale**, two of them the profile
   > ledger's own limit statements, which understated the evidence. **The bundle hits are false
   > positives by construction** - a bundle is dated and immutable, so a figure it states was true
   > when collected - and no bundle figure was touched. **ADR prose is still unswept.**
2. **The vocabulary is fifteen subjects.** A figure bound to a countable noun outside it —
   `bundles`, `exclusions`, `logs`, `rows`, `members`, `types` — is invisible, and rule H2's
   `Thirty-two exclusions` and `two bundles` are two such claims left standing in this pass.
   **They are named here rather than left for the next sweep to discover.**
3. **A subset count reads like a tree count.** Sections 3's three rewordings were judgment calls:
   nothing mechanical distinguishes "three annotations were chosen" from "three annotations
   exist", and a future row could evade the clause the same way honestly or dishonestly.
4. **EX-102 is unchanged.** This widens the reach of an existing clause; the general problem —
   prose that stops being true — remains unsolvable by a rule.
5. **The adjacency limit stands.** Each clause reports the figure nearest the word or subject, so
   a sentence carrying several is flagged once.
6. **Nothing here checks the catalogued figures are right.** J8 holds the report to the units and
   A7 holds the manifest to the graph; J12 inherits whatever they miss.
7. **A message is what a rule said, not proof that the rule is right.** Nothing runs on a device,
   no milestone moves, and nothing is reviewed.
