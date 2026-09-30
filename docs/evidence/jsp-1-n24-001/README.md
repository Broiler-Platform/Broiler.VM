<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Record JSP-1-N24-001

**What this is:** a working record of the JavaScript profile. It retains the evidence for the clause
of [the parity roadmap](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.parity.md)'s JSP-1
that names rule N17: "Rule N17 — or a rule beside it — reads every document that claims a name is
absent rather than the ledger's block alone, and is watched failing against an injected stale claim."
Rule N24 is that rule.

**It moves no row of [the profile's ledger](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.status.md).**
JSP-1 is not a ledger row, it has no owner, and a stage's clauses met in a working tree are not
acceptance.

**Collected:** 2026-09-29, at commit `6cb207c`, from a clean tree, by `collect.py` in this directory.
- **The change:** `bdefd88`. It adds rule N24, its witness and its register row, and marks or corrects
  the six stale claims N24's first run found.
- **This directory's tooling:** `6cb207c`. It changes no product, harness or test file.
- **The retaining commit** adds this record's logs and a dated note in the profile's ledger.

## 1. Identity

| Field | Value |
|---|---|
| Record | JSP-1-N24-001 |
| Stage touched | JSP-1 (the N17 clause) |
| Rule | N24, in [`N24AbsenceClaimsRuleTests.cs`](../../../src/tests/Broiler.VM.Architecture.Tests/N24AbsenceClaimsRuleTests.cs), Active from its minting |
| Correction | [JSC-232](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-232) |
| The realm's published set | [`docs/realm/globals.txt`](../../../src/Broiler.VM.Profile.JavaScript/docs/realm/globals.txt), unchanged |
| Comparison engine | Node `v22.22.2`, for the probes only |
| RID and mode | `linux-x64`, framework-dependent JIT only |
| Owner | the JavaScript profile owner; one person holds every role |
| Reviewer | none |

## 2. The rule

N24 reads every Markdown document under `src/Broiler.VM.Profile.JavaScript`. It skips two places:
- the corrections file, whose entries quote superseded readings by design;
- the evidence bundles, which are immutable.

It refuses a claim that a global the realm publishes is absent, in three shapes:
- an `absent-globals` block;
- a clause, "`Name` is absent" or "are absent", optionally "still" or "now";
- a bullet whose bold lead is a backticked name, under a heading that says "absent".

Text a document keeps as it was written sits between a dated `<!-- as-written, superseded YYYY-MM-DD -->`
line and a `<!-- /as-written -->` line. The rule reads nothing inside. It refuses any other form of
marker, a span opened inside another, one never closed, and a close with none open.

`n24.log` is its two tests, both passing:
- **The documents:** no document of the profile makes such a claim. The test first checks that the
  three documents making absence claims are read, and that the two excluded places are not.
- **The witness:** exactly its eight findings are reported:
  - one injected stale claim per shape (`Proxy` and `Reflect` in clauses, `Promise` in a bullet,
    `Object` in a block);
  - the four malformed markers.

  Four passing cases are not reported:
  - a genuinely absent `Intl`;
  - a present `Map` inside a marked span;
  - a `Symbol` said to have been absent;
  - an `Array` bullet under a heading about something else.

**Stated limit.** A claim phrased any other way, "lacks", "has no", "answers `undefined`", is not
seen. The survey in the parity roadmap's section 4.3 is introduced by one ("the wide realm lacks
these names"), and N24 reaches it through its bullets instead.

## 3. The controls

Each control is a patch in this directory. `control-<name>.log` applies it, runs N24's tests, and
reverts it. In every one the witness test still passes and the documents test fails.

| Control | What it does | What N24 reported |
|---|---|---|
| `control-first-run` | restores the parity and workload roadmaps to the base commit, `5f68726` | the six stale claims below: N24's first run, reproduced |
| `control-injected-claim` | adds "`Proxy` is absent from the realm" to the workload roadmap | that one clause, at the line it was added |
| `control-unmarked` | removes the as-written markers from the workload roadmap's "Absent still" paragraph | its `BigInt` and `Float16Array` clauses |
| `control-ledger-only` | narrows the rule to the ledger, which is what N17 reads | its non-vacuity check: the parity roadmap is not among the documents read |

**The six stale claims of the first run**, and what the change did with each:

| Where | Claim | Done |
|---|---|---|
| parity roadmap, section 4.3 | bullets leading with `BigInt` and `Float16Array` | the survey marked as written, superseded 2026-09-21, with a dated note of what holds now |
| parity roadmap, section 4.2 | "The `BigInt` global is absent" | the two paragraphs the section already keeps as written, marked, superseded 2026-09-08 |
| workload roadmap, section 3.2 | "`BigInt` is absent", "`Float16Array` is absent" | the paragraph its own note of 2026-09-22 overtook, marked |
| workload roadmap, section 3.2 | "the keyed collections and `Promise` are absent too" | a description of bundle JS-4-001's day, put in the past tense |

`tests.log` is the solution's test projects after every revert, every test passing.

## 4. The probes behind section 4.3's note

`survey-4.3.js` asks the realm, one member at a time, for each name section 4.3 lists, the way that
survey asked it. `unicode.js` asks for the Unicode behaviours its closing paragraph names.
`probes.log` runs both through the host and through Node `v22.22.2`. The note in section 4.3 is
written from the host's answers. Node is there so a reader can see the comparison engine's answers,
including that it lacks `Error.isError` too.

## 5. What this record does not demonstrate

- **Acceptance, an owner, a reviewer.** Nothing here was read by anyone but its author, and no row of
  the ledger moves.
- **The runner half of JSP-1 on Linux CI.** Slice J01's record retains Windows runs and notes Linux
  CI as wired and unobserved; this record adds nothing to that.
- **Documents outside the profile's directory.** The top-level `docs/` tree is not read, and no claim
  of absence in it was looked for.
- **Other phrasings,** as section 2 states.

## 6. What was run

```text
python3 docs/evidence/jsp-1-n24-001/collect.py
```

## 7. Environment

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3;
- Node `v22.22.2`.
