# Bundle JS-ANDROID-015 — EX-102 narrowed, and eight register rows that were counting a tree they no longer describe

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** [JS-ANDROID-014](../js-android-014/README.md) minted rule J12 over three
figures and recorded EX-102: *"A register row can be wrong about anything else — a count of
artefacts, a number of units, a milestone — with nothing here to stop it."* Closing that was asked
for. **One class inside it is now closed completely, the rest is not closeable, and the difference
is stated rather than blurred.**

Finding it out cost nothing and found a great deal: **eight rows were already wrong.**

---

## 1. What was actually in the register

A sweep for one shape — a figure, a noun phrase, and the word *exist* — found 18 counted claims.
Eight of them the tree contradicts:

| Row | Said | The tree holds |
|---|---:|---:|
| A4 | five test-only projects | **9** |
| A4 | two composition roots | **5** |
| A7 | eight edges | **59** |
| A11 | two composition roots | **5** |
| A12 | two composition roots | **5** |
| K1 | two composition roots | **5** |
| J1 | 689 relevant units | **905** |
| J1 | 903 exempt ones | **1082** |
| J1 | 1,592 code units | **1987** |
| J4 | 689 annotated units | **905** |

**Every one was green.** The register's row-equality test compares a row against a hardcoded copy
of its own prose, so a wrong number agreeing with itself is a passing test — the same mechanism
that let J10's row stand, found at scale rather than one row at a time.

Several carried a milestone tag — *"Eight edges exist at VM-0"* — which makes the sentence
defensible as history and useless as a description. The register declares itself at **VM-6**, and a
reader at VM-6 was being handed a figure off by a factor of seven.

## 2. The clause, and the principle behind it

**If the tree can compute it, cite it; if the tree cannot, do not state it as a number.**

The figure catalog is widened from three criteria metrics to sixteen — the report's assurance
figures, the graph's counts computed from `ComponentGraph.Projects` the way the graph rules compute
them, and the ADR counts. All 18 claims were converted: thirteen into citations, five reworded to
stop asserting a count nothing can check.

**A row whose subject has no catalogued figure is not exempt.** It is a row asserting a count
nothing can verify, which is the thing the rule exists to stop, and the fix is to say the sentence
without the number. That is what happened to A11's *"two consumer profiles"*, H1's *"four review
documents"*, K2's *"three statements"*, K3's *"two baselines"* and V12's *"all five contracts"*.

## 3. Results

Sixteen reports over a clean checkout; **76 of the register's 77 rules reported and J12 silent**.
Suite: 163 architecture and 207 contract tests, green.

**Four controls**, in `negative-controls.log`:

| Control | Suite | What J12 said |
|---|---|---|
| `J12-a-row-states-an-edge-count-the-tree-contradicts` | failed | `A7 states that Eight of something exist without citing a figure: "Eight edges exist"` |
| `J12-a-row-states-a-project-count-the-tree-contradicts` | failed | the same for A4's `"Five test-only projects exist"` |
| `J12-a-row-states-a-count-that-is-correct-today` | failed | reports `"Five composition roots exist"` — **which is right today** |
| `J12-a-row-cites-a-graph-figure-that-is-gone` | failed | `A7 cites the figure {graph:arrows}, and the figure catalog defines no such metric` |

The first two are **the sentences the register actually carried**. The third is the one that says
what the rule believes: a right number and a stale number are the same object seen at different
times.

## 4. Two repairs to the harness, both recorded because both failed silently

- **A control character in the rule's own regex.** The identifier guard was written through a
  shell heredoc, which collapsed `\\b` to `\b`, and a non-raw Python string turned that into
  **chr(8)**. The lookbehind then contained a backspace, could never match, and the guard it
  provided silently did nothing — the clause reported `ADR 0003` as a count. Found by reading the
  bytes, not the file; `od -c` shows it and the editor's own display does not.
- **The stale-assembly revert** from JS-ANDROID-014 §5.7 is fixed in this harness from the start.

## 5. Exclusions — what this bundle does not show

1. **EX-102 is narrowed, not closed, and the row says so.** The counted-existence class is closed.
   **Every figure worded outside that shape is unchecked**: a row may still say a rule covers some
   number of units, or that something held at a milestone, and nothing reads it. **The general
   problem — prose that stops being true — is not solvable by a rule**, and nothing here should be
   read as claiming otherwise.
2. **The clause reports the figure adjacent to the word.** J1's sentence carried three wrong
   numbers and is flagged once, on the nearest. That is enough to send a reader to the sentence and
   is not a count of everything wrong in it.
3. **A number spelled `one` is still invisible**, for the reason JS-ANDROID-014 gave.
4. **The five reworded claims lost information.** A11 no longer says how many consumer profiles
   exist, because nothing computes that; the row is weaker and honest rather than precise and
   unverifiable. A reader wanting the number must count.

   > **Correction, 2026-09-02.** [Bundle JS-ANDROID-016](../js-android-016/README.md) adds the
   > six figures those claims needed, and **all five state their numbers again** - A11 cites
   > `{graph:consumer-profiles}`, H1 `{review:documents}`, K3 two composition figures, V12 the
   > contract-set size and K2 rule K2's own arity. Two of the six are declared rather than
   > derived, and the rows citing them say so. **H1's row carried a second stale claim** - "the
   > two evidence-bundle READMEs" - which the count clause never saw because the figure was
   > attached to a noun rather than to "exist".
5. **The catalog is curated.** The report's tables reuse labels, so a catalog built by slugging
   every label would resolve a citation to whichever row it met first. Sixteen figures are
   catalogued; a row needing a seventeenth must add it.
6. **Nothing here checks that the catalogued figures are right** — J8 holds the report to the units
   and rule A7 holds the manifest to the graph. J12 inherits whatever they miss.
7. **A message is what a rule said, not proof that the rule is right.** Nothing runs on a device,
   no milestone moves, and nothing is reviewed.
