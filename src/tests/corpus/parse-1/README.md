<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The parse corpus `parse-1`, copied from Broiler.JS

**Written:** 2026-10-20, at milestone SJL-12 of the shared JavaScript libraries programme, work package SJL-12.4.
**Owner:** the JavaScript profile owner. **Reviewer:** none.

This folder is the population of bundle `sjl-12-001`'s parts B and C, in
[`decision-rule-area.md`](../../../../docs/evidence/sjl-12-001/decision-rule-area.md): the inputs the slice
compiler's `--parse-corpus` mode compiles, and the manifest that names them. The corpus's home is the Broiler.JS
repository, at `conformance/parse/`, under decision D32 of
[JSD-0080](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0080-the-shared-frontend-contract-and-the-in-place-preparation.md).
**This folder is a byte-for-byte copy of that home's `corpus.manifest` and `inputs/`**, at Broiler.JS's corpus
commit. It holds inputs only. No outcome, artifact or verdict is here: the baselines are the bundle's.

| | |
|---|---|
| Broiler.JS's corpus commit | `BROILERJS_CORPUS_COMMIT` |
| SHA-256 of `corpus.manifest` | `1c5342104766f14d0c17d950537e4ed078a0c438c3652f8c65bff8c6c347e9cc` |
| Manifest format | `broiler-parse-corpus 1` |

## What is here

| Path | What |
|---|---|
| `corpus.manifest` | One row per input, 55,751 rows, and a header that fixes the virtual root `/test262/`, the stored root `/parse-1/`, the strict prologue, the request-flag sets E0 to E7 and the pins. The home's README, section "The manifest", defines every field. |
| `inputs/broiler-vm/` | This repository's inputs, taken at the rule commit `889ea1568f59bf37aa21c0035b423d0d030eb87f`. `js-1/` holds the files of `src/tests/corpus/js-1/source/accepted/` and `refused/`, and `generated/` the two `SliceSourcePrograms` texts that `source.manifest` marks generated. |
| `inputs/broiler-js/` | Broiler.JS's parser and compiler test inputs, each stored under its SHA-256. |
| `README.md` | This file. It is this repository's own and is not part of the copy. |

The manifest has 2,270 stored rows, one for each file under `inputs/`.

**What is not copied.**
- **test262's inputs.** Each is named by its path in the suite and its SHA-256. The mode reads it from the pinned
  archive, `src/tests/conformance/pins/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e.tar.gz`, which the
  manifest's header names with its SHA-256.
- **The home's `README.md`.** It defines the manifest, the population and how the builder made them. It is read in
  Broiler.JS at the same commit.
- **The home's `provenance/`.** It records where each stored input was taken from.
  - JSD-0080 decision D32 names "a byte-identical copy" of the home. It also gives the copy a README of its own,
    one that names the Broiler.JS commit, so the copy cannot be the whole folder byte for byte. D32 does not say
    which other files the copy holds.
  - The home's README does: "a byte-identical copy of `corpus.manifest` and `inputs/`".
  - The rule's population is read from those two and the archive, and neither the mode nor the comparison reads
    provenance.

## How it is read

```sh
dotnet build src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler -c Release
SC=src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.SliceCompiler.dll
dotnet $SC --parse-corpus src/tests/corpus/parse-1/corpus.manifest report.tsv.gz --workers 1
dotnet $SC --census src/tests/corpus/parse-1/inputs <the extracted suite>/test > census.txt
```

The `--parse-corpus` mode reads the manifest and refuses the whole run, before it compiles anything, when among
other things:
- the header is not format 1's, or names another strict prologue or other request-flag sets;
- the archive's SHA-256 is not the manifest's pin;
- any input's bytes differ from its row's SHA-256.

It writes one report line per compilation of sub-populations B1 to B4. `eng/compare-parse-corpus.py` joins two
reports and holds each to this manifest's digest and population. The bundle's procedure, not this file, fixes
each run's arguments.

## How the copy is checked

Each check below is exact.
1. **The manifest's digest.** `sha256sum src/tests/corpus/parse-1/corpus.manifest` prints the SHA-256 above.
2. **Every stored input against its row, and no file without one.** From this folder:

   ```sh
   awk -F'\t' '!/^#/ && $1 != "id" && $2 != "test262" { print $4 "  " $3 }' corpus.manifest \
     | sha256sum --check --quiet --strict
   awk -F'\t' '!/^#/ && $1 != "id" && $2 != "test262"' corpus.manifest | wc -l
   find inputs -type f | wc -l
   ```

   The first command prints nothing and exits 0. The other two print the same number.
3. **Against the home, file for file.** Both repositories use SHA-1 object names. A tree's name covers every file's
   bytes, name and mode, so equal names mean an identical copy:

   ```sh
   git -C <Broiler.JS> rev-parse BROILERJS_CORPUS_COMMIT:conformance/parse/corpus.manifest \
     BROILERJS_CORPUS_COMMIT:conformance/parse/inputs
   git rev-parse <this repository's commit>:src/tests/corpus/parse-1/corpus.manifest \
     <this repository's commit>:src/tests/corpus/parse-1/inputs
   ```

   The two commands print the same two lines.

**Line endings.** The copied files hold no CR and no NUL. This repository's `.gitattributes` line
`* text=auto eol=lf` therefore stores and checks them out unchanged, as Broiler.JS's `conformance/** text eol=lf`
does there. Check 3 compares what is stored.

## Rules and terms

- **Rule N13 does not apply.** No project item names this folder or any file in it, so rule N13 is untouched and no
  rule is revised (JSD-0080 decision D32). N13 holds that no project file names a conformance-suite directory. The
  mode takes the manifest on its command line, and it reads test262's files from the archive, not from here.
- **The copy changes only with its home.** Bundle `sjl-12-001`'s part B requires the manifest's digest at every run
  to equal the corpus commit's. A different population is a new corpus, with a new name and a new manifest, in both
  repositories.
- **Terms.** The corpus is Broiler.JS's. It is under Apache-2.0, the licence of this repository, and has the same
  copyright holder.
