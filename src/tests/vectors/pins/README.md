<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The shared conformance vectors, pinned

**Owner:** the JavaScript profile owner. **Reviewer:** none.

The corpus of the shared JavaScript libraries programme's milestone
[SJL-1](../../../../docs/javascript-shared-libraries/milestones/sjl-1-shared-conformance-vectors.md).
A vector pins one operation's outcome for one input, with where that outcome comes from; the corpus
runs against both engines, Broiler.JS and this profile, and later against each shared library. Its
home is the Broiler.JS repository, at `conformance/vectors/`, under decision SJL-D-1. **This
repository holds it the way it holds test262: as the archive of one named revision, beside a pin.**

| File | What it is |
|---|---|
| [`vectors.pin`](vectors.pin) | The Broiler.JS commit, the archive's SHA-256, the SHA-256 over the files it extracts to, and their count, in the retained-suite pin format of [`../../conformance/pins/test262.pin`](../../conformance/pins/test262.pin) |
| `broiler-js-vectors-<commit>.tar.gz` | The archive, as `git archive` of that commit wrote it |

## How it is read

```bash
dotnet run --project src/compositions/Broiler.VM.Composition.JavaScript.Conformance -c Release -- \
  --vectors src/tests/vectors/pins/broiler-js-vectors-b2c2e2243a086285d806866a64b83297791a8ec5.tar.gz \
  --expect src/tests/vectors/pins/vectors.pin --report vectors-report.tsv
```

The conformance root reads the archive in memory and refuses it, before any vector runs, unless
its digest, its files' digest and its file count are the pin's. It then evaluates every vector in a
fresh realm of this profile, under the wide manifest in the bytecode form and with every surface the
profile builds, Intl and Temporal included, renders the outcome
under the corpus's one rendering rule (its `SCHEMA.md` section 7), and compares. `--mutate <id>` is
the negative control: it changes one vector's expected value in the in-memory copy, and the run
must report that vector as an unclassified difference. **Rule N13** lists `tests/vectors` among the
directories no project file may name, so the corpus reaches the root on a command line and nowhere
else; its third witness names the directory from the profile's own project file.

## How a new revision is pinned

1. The corpus changes in Broiler.JS, by a reviewed change that raises its revision and adds a dated
   row to its `REVISIONS.md`.
2. The archive of the commit that carries it is made with the command `vectors.pin` records, twice;
   the two files must be byte-identical. The second is extracted and its files hashed there.
3. The old archive is removed, the new one added, and `vectors.pin` rewritten, in one change.
4. A run on the new pin is retained in the bundle of the milestone that asked for it.

## Terms

The corpus is Broiler.JS's, under Apache-2.0, the licence and the copyright holder of this repository. It quotes no
specification text and copies no Unicode data file: its provenance names ECMA-262 clauses by their identifiers and
states which code points a set holds. No vector at this revision is derived from test262: eight vectors of the Date
area and 206 of the syntax area name a test262 test in their provenance, by its path, as a test that confirms or
asserts their expected value, and their scripts and values are the corpus's own. *(Revised 2026-10-15, at SJL-6.2,
when the pin moved to revision 15. The sentence ended at "derived from test262.")* *(Revised 2026-11-01, at SJL-13,
when the pin moved to revision 30. The sentence read "eight vectors of the Date area name a test262 test in their
provenance, as a test that confirms their expected value", true of the Date area alone until revision 29.)* The
Temporal area, from revision 24, names clauses of the Temporal proposal's text and of the Intl era and month code
proposal's the same way, by identifier, and quotes neither. Its expected values are the answers of the Temporal
proposal's reference polyfill at `e8cc03fc`, which is under the ISC licence, except where the pinned text rules: the
corpus holds those answers and none of the polyfill's code or text, as this repository holds its own datasets'
polyfill answers. *(Added 2026-10-18, at SJL-10.2, when the pin moved to revision 25.)* The Intl area, from revision
27, names clauses of ECMA-402 at the commit its `sources/ecma402.pin` records, `tc39/ecma402` `3d4c558`, and of the
Temporal and Intl era and month code proposals' texts, the same way, by identifier, and quotes none of them. Its
expected values are ICU's answers, through the reference Node.js release Broiler.JS's bundle `sjl-9-001` names,
except where the pinned text rules, and, for a region's hour cycles, DurationFormat's `digital` data and this
profile's composition of a compound unit, CLDR 48.2.0's data as its generator reads it: the corpus holds those
answers and none of ICU's code or CLDR's files, as this repository holds its own datasets' answers. *(Added
2026-10-27, at SJL-9.2, when the pin moved to revision 28.)* The syntax area, from revision 29, names clauses of
ECMA-262 ES2026 at the commit its `sources/ecma262.pin` records, `tc39/ecma262` `0248456c`, by identifier, and
quotes none of them; each vector's source is written from the grammar, not taken from a test (the corpus's revision
log, row 29), and its expected value is the specification's, a refusal or a completion value. Where a vector cites
test262 at `ccaac100`, it names the test's path and copies none of its text. Its ten explicit-resource-management
vectors take their expected values from ECMA-262 main at `4feb0272`, which merged those rules after the pinned
edition, and cite their test262 tests alone. The corpus holds those outcomes and none of the specification's or
test262's text, as it holds the other areas'. *(Added 2026-11-01, at SJL-13, when the pin moved to revision 30.)*
The regular-expression area, from revision 31, names clauses of ECMA-262 ES2026 at `tc39/ecma262` `0248456c`, 22.2
and Annex B's B.1.2, by identifier, and the UCD 17.0.0 files its property and case-folding vectors read, and quotes
none of them. Every expected value but those of 24 vectors is the answer of the corpus's own reference, `regexpref`
(`ref:regexpref@1`, decision D25 of JSD-0081), the text's pattern grammar, early errors and matcher transliterated in
Broiler.JS's `scripts/conformance/regexpref.py`, over the pinned UCD read through `unicoderef.py`; no expected value
is read from an engine. The 24 are object-model vectors and the eval-code literals of row S9b, each cited to the
clauses that decide it, and row M4's empty match, for which the text has no answer and decision D41 records each
engine's. No vector of the area cites test262 in its provenance or is derived from a test262 test: 34 notes name a
test262 test at `ccaac100` by its path, 28 of them the test whose pattern the vector also tries, and the vector's
script is the corpus's own, with none of test262's text. Its 18 `status: exhausted` vectors are library-level, and the
conformance root skips each and names it in its summary line. The corpus holds the reference's answers and none of
the specification's or test262's text, as it holds the other areas'. *(Added 2026-11-02, at SJL-11.3, when the pin
moved to revision 32.)*
[`THIRD_PARTY_NOTICES.md`](../../../../THIRD_PARTY_NOTICES.md) lists it with the other material this
repository ingests.
