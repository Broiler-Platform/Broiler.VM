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
  --vectors src/tests/vectors/pins/broiler-js-vectors-593f2c5bce1f055025864af2b7ec38ee6f2f835a.tar.gz \
  --expect src/tests/vectors/pins/vectors.pin --report vectors-report.tsv
```

The conformance root reads the archive in memory and refuses it, before any vector runs, unless
its digest, its files' digest and its file count are the pin's. It then evaluates every vector in a
fresh realm of this profile, under the wide manifest in the bytecode form, renders the outcome
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

The corpus is Broiler.JS's, under Apache-2.0, the licence and the copyright holder of this
repository. It quotes no specification text and copies no Unicode data file: its provenance names
ECMA-262 clauses by their identifiers and states which code points a set holds. No vector at this
revision is derived from test262. [`THIRD_PARTY_NOTICES.md`](../../../../THIRD_PARTY_NOTICES.md)
lists it with the other material this repository ingests.
