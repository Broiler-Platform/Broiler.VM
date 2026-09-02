# Bundle JS-ANDROID-018 — sweeping the ledgers and the bundles, and why only one of the two can be swept

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** [JS-ANDROID-017](../js-android-017/README.md) swept the register for
figures bound to a noun and recorded, as exclusion 1, that *"the same shape lives in evidence
bundles, in the status ledger and in ADR prose, and nothing here looked at any of them."* This is
that sweep.

**Three live claims were stale. The rest of what the sweep flagged is not a defect, and saying why
is most of this bundle.**

---

## 1. A bundle cannot be stale

An evidence bundle is **dated and immutable**. `vm-1/README.md` says the register held 54 rules; it
held 54 rules at VM-1. The sweep flags that as stale against today's 77, and **the log keeps the
marker rather than filtering it out**, because what a naive comparison produces is itself the
finding: 107 bundle hits, essentially all of them false positives by construction.

The discipline already handles this — a bundle is corrected by a dated `> **Correction**` block
with its logs left unedited, never by rewriting the number. **No bundle figure was touched.**

## 2. A ledger can be, and three were

| Document | Said | The truth |
|---|---|---|
| profile ledger | *Two composition roots exist, both registered as demonstrations* | **three** JavaScript roots are registered — the slice compiler, the execution-only image, the Android head |
| profile ledger | *One runtime identifier is recorded as published and run — `win-x64`* | **two**: `win-x64` and `android-x64` on an emulator |
| core ledger | the API baseline is *1,251 lines* | 1252, so it is `about 1,250 lines` now — an approximation, because nothing there can cite |

**Both profile claims are in the section headed "What this component is not claiming", and both
UNDERSTATE the evidence.** That is the safer direction to be wrong in and it is still wrong: a
reader deciding whether the Android head is demonstrated was being told it is not recorded, by the
one section written to be trusted about limits. The Android work is five bundles deep in this same
directory.

## 3. Why the ledger cannot be ruled the way the register is

The sweep flags ten figures in the profile ledger. **Seven of them are the ledger quoting the
defects it reported** — *"eight rows went on saying 45 covered source files, 48 artefacts, 689
annotated units"* — written there by the change that fixed them.

A mechanical sweep cannot tell *"45 files"* (a stale claim) from *"eight rows went on saying 45
files"* (a report of one). The register's clauses work because a register row states facts; a
ledger narrates, and narration quotes. **Ruling the ledger would need the tense-and-quotation
discipline that has already made rule J12's own row fire on itself twice** — and that is a rule
design, not a sweep, so it is named here rather than attempted.

## 4. Results

`ledger-and-bundle-sweep.log` retains the whole sweep, unedited, over both ledgers and all 27
evidence bundles: 13 ledger hits and 107 bundle hits. Suite: 165 architecture and 207 contract
tests, green after the three corrections.

## 5. Exclusions — what this bundle does not show

1. **ADR prose was not swept.** JS-ANDROID-017's exclusion named three documents and this covers
   two. The ADRs are the third and they are the ones the register cites as authoritative.

   > **Correction, 2026-09-02.** [Bundle JS-ANDROID-019](../js-android-019/README.md) sweeps
   > the ADRs and the profile decision records. **One defect**, made in this same session: ADR
   > 0001's revision for the Android composition root carries no budget paragraph, so the
   > record's last stated graph size was one revision behind. Rule **A15** now reads that
   > sentence. The other 94 figures are the decision itself rather than measurements, and were
   > classified by reading.
2. **No rule was minted.** The three corrections are prose fixes in live documents, and nothing
   stops the same figures going stale again — the exposure the register no longer has. Section 3
   says what a rule would need.
3. **The bundle hits were not individually reviewed.** They are treated as historical *by
   construction*, on the strength of every bundle carrying a collection date. A bundle that stated
   a figure as currently true, rather than as of its date, would be indistinguishable in this log
   and none was looked for.
4. **The vocabulary is the same fifteen subjects** rule J12 reads, so the same limits apply: a
   figure bound to a noun outside it is invisible, and a sentence carrying several figures is
   flagged once.
5. **`about 1,250 lines` is an approximation on purpose.** An exact figure in the core ledger would
   rot on the next public member; an approximate one is stable and less informative, which is the
   trade a document with no citation mechanism has to make.
6. **A message is what a sweep found, not proof the documents are otherwise true.** Nothing runs on
   a device, no milestone moves, and nothing is reviewed.
