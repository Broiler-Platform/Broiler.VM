# Bundle JS-9-005 — the fourth surface, and the control that makes a clean session worth reading

**Collected:** 2026-09-06. **Milestone:** JS-9, which it does not close.
**Owner:** MaiRat. **Reviewer:** none.

**What this bundle is.** The retained record of a fuzz session over **the regular-expression
matcher**, which is the fourth of roadmap [section 7](../../roadmap.md#7-the-bytecode-format-and-the-verifier)'s
four untrusted-input surfaces and the last one nothing reached. It sits beside the sessions
[bundle JS-7-001](../js-7-001/README.md) retains over the other three — the verifier, the executor,
and the source tokenizer and parser — and it is a **further bundle rather than an edit to that
one**, because a clause found open after a collection is closed by a further collection.

**Why it exists as a bundle of its own rather than a re-collection.** JS-7-001's exclusions name
this surface as reached by no session, and that was true of the tree it was collected from. It is
not true of this one. Editing that bundle to say otherwise would make a retained record a moving
one.

**What this bundle is not.** Not a publish. It was collected with `--skip-publish`, so there is no
`closure-*.txt`, no `catalog-*.txt` and no `publish-and-run.log` here, and no Native AOT claim is
made in it — JS-7-001 carries that, one commit earlier. And **nothing here is accepted**: the
reviewer field of `identity.txt` says `none` and means it.

---

## 1. The required fields, and the file in this directory that carries each

Roadmap [section 4](../../roadmap.status.md#4-required-evidence-bundle) names nine. **A field is
satisfied by a file the collection wrote, never by a sentence here.**

| Field | Where it is |
|---|---|
| **Identity** | `identity.txt`, `snapshot-identity.txt` |
| **Source** | `identity.txt`, `hashes.txt` |
| **Dependencies and corpus** | `environment.txt`, `hashes.txt`, `corpus-integrity.log` |
| **Environment** | `environment.txt` — one machine, one RID, JIT |
| **Procedure** | section 2 below, and the header of each log |
| **Results** | `fuzz.log`, `suite.log`, `build.log`, the assurance logs |
| **Negative controls** | `negative-controls.log`, `corpus-controls.log`, `fuzz-controls.log`, `android-controls.log` |
| **Closure** | **none, and deliberately** — section 5 |
| **Exclusions** | section 6 |

---

## 2. Procedure

```text
python3 eng/collect-js-evidence.py \
  --bundle JS-9-005 \
  --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-9-005 \
  --milestone JS-9 \
  --rid linux-x64 \
  --skip-publish
```

---

## 3. What the matcher session is, and what counts as a finding

**Both halves of the input are mutated.** A pattern is text a program wrote and a subject is text a
program passed, so a session that mutated only the pattern would leave unexplored the half where a
backtracker spends its time. The seeds are this repository's own — no third-party corpus is read.

**The matcher declares exactly two refusals and the caller knows exactly two.** A pattern that is
not one, and a ceiling the matcher declares for itself. So an escaping third exception is a
**counterexample**, because it is an answer no guest could be given; everything else — a refusal, an
overflow, a match, no match — is an answer. The session reports the class of every answer it saw and
ends non-zero on the first exception it cannot account for.

**The guidance is by published answer**, as
[JSD-0013](../../decisions/0013-the-fuzz-sessions-coverage-signal.md) requires of every session
here, and its seed pool grows until it reaches a ceiling it states. That is a stronger loop than the
source session in JS-7-001, which kept nothing at all — and the difference is a fact about the two
surfaces rather than about the two mutators.

**A session's own integrity clause.** A run that reached neither a match nor a refusal exits
non-zero rather than reporting clean iterations it did not earn.

---

## 4. Negative controls — including the one this bundle exists to demonstrate

**A clean fuzz session is worth exactly what the demonstration that it would have found something is
worth.** `fuzz-controls.log` carries that demonstration for the matcher: one declared refusal is
turned into an ordinary exception, and the session must report the pattern and the subject that
reach it and exit with a finding. The revert must then pass.

The other three control families ran unchanged and are retained here for the same reason they are
retained in every bundle: the suite's, the corpus replay's, and — reported as a gap rather than a
total — the Android head's, which needs an SDK this machine does not have.

---

## 5. Closure — there is none, and that is a statement rather than an omission

**No Native AOT claim is made in this bundle and no published image was read.** The collector was
run with `--skip-publish`. [Bundle JS-7-001](../js-7-001/README.md) carries the publish-and-run
evidence, one commit earlier, and nothing here narrows or substitutes for it.

---

## 6. Exclusions — what this bundle does not show

- **No human has read any of it, and nothing is accepted.** `assurance-release.log` refuses as it
  must.
- **One machine, one runtime identifier, and no publish.** See section 5.
- **A session is not a proof.** What is retained is a bounded number of iterations from stated
  seeds. The matcher is a hand-written backtracking engine of some thousands of lines, and no
  session of this size explores it.
- **The seed set is this repository's own.** It reaches the constructs its authors thought of, and
  the guidance grows it by answer rather than by structure — so a construct no seed and no mutation
  reaches is not covered and would not be reported as uncovered.
- **The session's ceiling is not the matcher's.** An iteration that spends more than the session's
  own work ceiling is abandoned rather than reported, so a pattern that backtracks catastrophically
  is counted as an overflow answer and not as a finding. That is what the ceiling is for, and it
  means this session says nothing about how much work such a pattern would cost a real caller.
- **JS-9's gate is not closed by this.** Its corpus clauses, its soak, its agent clauses and its
  per-dimension exhaustion entries are all elsewhere or still open, and three of the four surfaces
  are covered by JS-7-001 rather than by this.
