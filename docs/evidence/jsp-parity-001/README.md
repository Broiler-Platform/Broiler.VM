<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Record JSP-PARITY-001

**What this is:** a working record of the JavaScript profile. It retains the evidence for two stages of
[the parity roadmap](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.parity.md):
- **JSP-8, the places this component disagreed with itself.** Four clauses were open on this date:
  - a top-level `for await` whose artifact the verifier refused;
  - the `d` flag's missing `indices`;
  - two refusals that named a token rather than a construct;
  - a usage text nothing held to the manifest.
- **JSP-10's severable clause:** `Math.random` seeded per realm.

**It moves no row of [the profile's ledger](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.status.md).**
Neither stage is a ledger row, neither has an owner, and a stage's clauses met in a working tree are not
acceptance.

**Collected:** 2026-09-29, at commit `371015c`, from a clean tree, by `collect.py` in this directory, with
`--base 3b54373`.
- **The change:** `9ee4210`.
- **This directory's tooling:** `d685b76` and `371015c`. They change no product, harness or test file.
- **The base:** `3b54373`, the commit before the change, built from a worktree for the conformance
  comparison.
- **The retaining commit** adds this record's logs, the corrections JSC-229 to JSC-231, the parity
  roadmap's pointers and dated notes, and a dated note in the profile's ledger.

**Core contract version:** 1, unchanged. **Feature manifest:** `broiler.javascript.wide`, with
`broiler.javascript.slice` for the one `--slice` clause.

## 1. Identity

| Field | Value |
|---|---|
| Record | JSP-PARITY-001 |
| Stages touched | JSP-8 (every open clause), JSP-10 (the `Math.random` clause only) |
| Corrections | [JSC-229](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-229), [JSC-230](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-230), [JSC-231](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-231) |
| Conformance suite | test262 at `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`, from the archive the pin names |
| Comparison engine | Node `v22.22.2`, for the fixtures' answers only |
| Compositions | the command-line host, the slice-compiler root, the conformance root |
| RID and mode | `linux-x64`, framework-dependent JIT only |
| Owner | the JavaScript profile owner; one person holds every role |
| Reviewer | none |

## 2. The five changes and what each run shows

`build.log` is the solution built with warnings as errors. `acceptance.log` is every row of
[`src/tests/cli/expected.txt`](../../../src/tests/cli/expected.txt) judged on the host: all 245 answer as
declared. `probes.log` runs each clause through the host's command line and keeps what it printed.

| Change | Fixture | What `probes.log` shows |
|---|---|---|
| A top-level `for await` records a top-level await ([JSC-229](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-229)) | `modules/a-top-level-for-await.mjs` | the three lines the comparison engine prints, exit 0 |
| The `d` flag builds `indices` ([JSC-230](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-230)) | `runs/a-match-with-indices.js` | the comparison engine's value, exit 0 |
| A comma after a rest in a binding position names the rest (JSC-230) | four files under `refused/` | `2101` naming the element, the property and the parameter, exit 3; beside them the assignment pattern's existing `2205` |
| Under `--slice` a class static block is refused for its class (JSC-230) | `refused/a-class-static-block-under-the-slice.js` | see below |
| The usage text names the manifests from the profile (JSC-230) | the `--help` rows | the `--help` lines naming a manifest, and `--version` |

**One probe shows why the static block has a fixture of its own.** `probes.log` runs `--slice` over
`runs/a-class-static-block.js`, the wide surface's static-block program, and the slice refuses it for
an array literal on its sixth line: the first construct outside the slice manifest in that file is not
the class. The acceptance rows use `refused/a-class-static-block-under-the-slice.js`, whose only
construct is the class, and hold the refusal to `2104` naming it.

**The usage text is held for its manifest names only.** The `--slice` and `--numeric` lines and the
paragraph describing the default manifest read the identities from `JavaScriptProfile`. The BigInt
paragraph still types the optional surfaces' names, `broiler.javascript.bigint` and
`broiler.javascript.binary`, and no row holds them.

`checks.log` is the slice-compiler root's `--checks`, with the two `Math.random` lines and the closing
total kept: three realms of one process draw three sequences, two processes draw two, and every check
passes ([JSC-231](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-231)).

## 3. The pinned conformance suite, either side

`test262-base.log` and `test262-change.log` run the same eight subtrees of the pinned test262 checkout,
on the base commit's conformance root and on this commit's:
- `test/language/module-code`;
- `test/language/statements/for-await-of`;
- `test/built-ins/RegExp`;
- `test/built-ins/Math`;
- `test/language/rest-parameters`;
- `test/language/expressions/arrow-function`;
- `test/language/statements/variable`;
- `test/language/statements/function`.

These are every area a change here could reach. `test262-moves.log` lists every variant whose verdict
differs: 26, all under `test/built-ins/RegExp/match-indices`, and all from failed to passed. **No
variant newly fails.** Neither run is a whole-suite figure, and each says so.

**The top-level `for await` moves nothing, and that is the finding rather than a gap in the run.**
Thirteen modules in the pinned checkout write `for await`. Twelve also await an expression in the
loop's head or body, which set the flag the loop did not, so they passed on the base. The thirteenth is
a parse-phase negative test that never runs. No file in the suite asks the question alone;
`modules/a-top-level-for-await.mjs` does. The `Math/random` and rest-position
tests pass on both sides: the first asks only that a draw be in range, and the second is scored on
whether a syntax error is raised, not on what it names.

## 4. The controls

Each control is a patch in this directory. `control-<name>.log` applies it, builds, runs what it must
fail, reverts it and builds again. The five host controls are judged against the rows this change added
to `expected.txt`, as a table of their own: 18 rows.

| Control | What it reverts | What failed |
|---|---|---|
| `control-for-await` | the parser's record at the `for await` head | the three fixture rows, each answering exit 4 in place of its value |
| `control-indices` | `indices` (the flag test reads `D`) | the `d` fixture's row: a `TypeError` in place of the value |
| `control-rest` | the named refusal after a rest | all eight rest rows: the old "was expected" answers |
| `control-static-block` | the slice parser's static-block member | both `--slice` rows: an unexpected brace again |
| `control-usage` | the `--slice` line's identity, typed as `broiler.javascript.slim` | the row holding that line |
| `control-constant-seed` | the per-realm seed, back to the constant | both `Math.random` checks |
| `control-counter-seed` | a seed that counts the realms a process has made | the two-process check only: three realms of one process do differ |

Each answers a wrong value, a refusal or a failed check that the rows or checks read, and none stops a
driver. `rows-after-controls.log` is the 18 rows after every revert, all answering as declared.
`tests.log` is the solution's test projects, every test passing.

## 5. What this record does not demonstrate

- **Acceptance, an owner, a reviewer.** Neither stage has an owner, nothing here was read by anyone but
  its author, and no row of the ledger moves.
- **The rest of JSP-10.** The host capabilities present and throwing, the format ceilings, the
  allowance defaults, the file order and the source encodings are untouched.
- **The other bullets of section 4.7.** `FinalizationRegistry.prototype.cleanupSome` is still shipped,
  and the host's `read` and `$262` are still present.
- **The cascade under `--all`.** A named rest refusal is the first the host reports. Under `--all` the
  parser's usual refusals of the tokens after it follow, as they do for any input it does not recover
  from.
- **A whole-suite run, other forms, other platforms.** The conformance runs are eight subtrees in the
  bytecode form. The native and value forms, Native AOT, a second RID and Windows were not run.
- **Unpredictability.** `Math.random` is still a xorshift. Only its seed stopped being a constant.

## 6. What was run

```text
python3 docs/evidence/jsp-parity-001/collect.py --base 3b54373
```

The fixtures' answers came from Node `v22.22.2`: `node -p` over the `d` fixture, and the module fixture
imported with `print` bound to `console.log`.

## 7. Environment

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3;
- Node `v22.22.2`, for the fixtures' answers.
