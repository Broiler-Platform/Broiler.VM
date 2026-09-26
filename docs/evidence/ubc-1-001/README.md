<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Evidence bundle UBC-1-001

**Milestone:** UBC-1 of [the universal bytecode programme roadmap](../../universal-bytecode.roadmap.md) -
`Broiler.VM.Ubc`: the format, the common family, the table schema, the primitive table, the walk and
the corpus.
**Collected:** 2026-09-25, at commit `1426a7c`, from a clean tree, by `collect.py` in this directory.
**Core contract version:** 1, unchanged. **Universal bytecode contract version:** 1.
**Status of the milestone after this collection:** `In progress`, every exit-gate clause met on the
evidence below, and unaccepted: under [`docs/mvp.md`](../../mvp.md) no milestone is accepted while
review is deferred, and nothing here has been read by anyone but its author.

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle | UBC-1-001 |
| Milestone | UBC-1, exit gate clauses 1 to 8 |
| Commits of the milestone | `3ff5ca8` the assembly; `ce6b51f` rules U1, U2, U4 and U9; `aaddefa` the malformed corpus, its runner and the codec's determinism check; `f2be54c` the diagnostic registry and rule U8; `4402bc8`, `01f9f9c`, `7a5e5ee`, `84b48e5`, `3e69488`, `9fedd35` and `0a03433` the corrections the corpus, the rules and two adversarial reviews found |
| Corpus | [`src/tests/corpus/ubc-1/`](../../../src/tests/corpus/ubc-1/), its `manifest.json` naming every entry's file, length, hash, expected outcome, reason and code |
| Registry | [`docs/ubc/diagnostics/registry.txt`](../../ubc/diagnostics/registry.txt) |
| API baseline | [`docs/ubc/api/public-api.txt`](../../ubc/api/public-api.txt) |
| Owner | the core architecture owner with the security owner |
| Reviewer | none |
| Evidence class | rule runs, a retained corpus replayed, and a round-trip check; no measurement of any kind |

## 2. What this bundle demonstrates, clause by clause

| Clause | What it asks | Where it is shown |
|---|---|---|
| 1 | `Broiler.VM.Ubc` references exactly Abstractions and Binary (U1), exports no banned identifier and no family row (U2, witness watched), and its surface equals the baseline in both directions (U9) | `architecture-tests.log`: every U1, U2 and U9 test in detail, the witnesses among them - a language's identifier, a static family table, each project-file breach, an omitted and a vanished member |
| 2 | The common family's every row is in the assembly with Appendix A's effect, read from one table (U4, witness watched) | `architecture-tests.log`: U4's row-for-row comparison, the appendix and the second-table witnesses |
| 3 | The codec round-trips every corpus control and sample artifact byte for byte, twice | `round-trip.log` |
| 4 | The corpus retained with hash, expected outcome, reason and code per entry; the runner replays it to its recorded answers; a mutated entry detected; every refusal of 6.1 has an entry; U8 holds the registry in both directions | The corpus manifest; `corpus-tests.log` (the corpus on disk is what the generator writes, every row's length and hash, every recorded answer, every code but the defensive ones reached by a named exact entry, a mutated entry detected by the tests and by the replayer); `corpus-replay.log` (the replayer's table, one line per entry); `architecture-tests.log`, rule U8 |
| 5 | Every operand shape, effect-descriptor form and target-descriptor form has a corpus entry that exercises it | `corpus-tests.log`, the shape, effect form, target form and kind test |
| 6 | ADR 0001's budget sentence, `graph.manifest.json` and the tree agree (A7, A15) | `architecture-tests.log` |
| 7 | `CoreContractVersionTests` and every existing suite pass unchanged; the core's baseline (M1) has not moved | `architecture-tests.log`, the core contract version tests and M1 in detail and then the whole suite; `contract-tests.log`, the whole contract suite. The solution holds these two suites and no other |
| 8 | The ledger's UBC-1 row names each unmet clause while any is unmet | The programme ledger, committed with this bundle |

## 3. What was run

```text
dotnet build Broiler.VM.slnx -c Release
python docs/evidence/ubc-1-001/collect.py
python eng/ubc-bundle-manifest.py --bundle docs/evidence/ubc-1-001 --milestone UBC-1 --evidence-class rules-and-corpus ...
```

`collect.py`'s header says what each file is. The replayer's table in `corpus-replay.log` is printed by
the fixture composition's `--ubc1-corpus` mode on the JIT lane, which compiles the replayer from the one
copy the contract suite compiles; bundle `ubc-2-001` retains the same table from all three publish
modes. The manifest command's full argument list is in `manifest.json`'s `inputs`, which name the corpus
manifest and every corpus artifact by hash.

## 4. Environment

Windows 11 Enterprise, the .NET SDK `10.0.401`, Python `3.11.9`, the checkout at the commit above with
nothing outside this directory changed. Nothing was published for this bundle.

## 5. The departures from the concept this milestone is responsible for

[Appendix G of the concept](../../universal-bytecode.md) records where the code departs from the text,
and each bundle repeats the list it is responsible for. This one's rows are: the family prefixes for
slots one to fourteen (5.4); listed and counted effects (5.6); region kinds as table data (5.10); the
hook's three calls (6.2); `jump_table` as a terminal row (Appendix A); the NaN rule for the entries the
flag governs, and the sign-bit operations outside it (Appendices C and D); identities and entry names
(Appendix E); landings as resume points and handlers (Appendix E); the walk's version (6); and the facts
the text does not state that the walk needs to be total. The rest of the list is UBC-2's.

## 6. What is NOT here, named rather than left as an absence

- **Nothing executes.** The walk verifies; nothing here runs an artifact. Execution is UBC-2's, in
  bundle `ubc-2-001`.
- **Reservations are estimates.** The reader's and the walk's reservations of decoded rows are stated
  constants, not measurements. The contract tests hold the walk's reservation per jump table to what
  it allocates; the reader's per-row estimate is below what it allocates for some row kinds, within
  a small constant factor, and is recorded as an estimate where it is declared.
- **Not every charge has a test of its own.** The walk charges every pass and search before it runs,
  and the record in `UbcVerifier.cs` says what; the contract tests watch the hook pass, the branch,
  region, position, local and landing searches, a push's count and a call's signature refused one
  unit short, and the bound held under every declared granularity, but the remaining charges of that
  kind are held by reading rather than each by a test.
- **No review.** Every record and line of code here is unreviewed, and the author and the owner are
  one person.

## 9. Exclusions

**This section defines no exclusion identifier, and that is deliberate rather than an omission.**
