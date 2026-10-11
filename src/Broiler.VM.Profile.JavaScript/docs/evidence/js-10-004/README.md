# Bundle JS-10-004 — the JavaScript roots' catalogs and closures, collected again rather than edited

**Collected:** 2026-10-11. **Milestone:** JS-10, which it does not close.
**Owner:** as `identity.txt` records. **Reviewer:** none.

**What this bundle is.** Every JavaScript composition root - ExecutionOnly, SliceCompiler,
Conformance and Cli - published and run under JIT, trimmed self-contained and Native AOT on
`win-x64`, with the catalog table each binary printed and the closure read off each published
output. **It exists to replace evidence, not to add a claim.** Three retained closure reports had
been edited after their bundles were retained, so the composition register's rule K4 held three
rows to files no run had written ([JSC-290](../../roadmap.corrections.md#jsc-290)). Those edits are
reverted, and the register's four JavaScript rows name this bundle instead - the shape the
WebAssembly rows took when their images changed and their cells moved to bundle `ubc-4-002`.

**What this bundle is not.** Not a negative-control or fuzz collection: it was collected with
`--skip-controls` and `--skip-fuzz`, because both inject into the checkout and neither bears on what
a published image contains. **Not reviewed and not accepted**, and it advances no milestone row.

---

## 1. The required fields, and the file in this directory that carries each

| Field | Where it is |
|---|---|
| **Identity** | `identity.txt`: bundle, milestone, collection time, owner, reviewer |
| **Source** | `identity.txt`: commit `88d8b51d` and a **DIRTY** working tree, every dirty entry listed; `snapshot-identity.txt`; `hashes.txt` |
| **Dependencies and corpus** | `environment.txt`; `hashes.txt`, the corpus manifest among its inputs |
| **Environment** | `environment.txt` |
| **Procedure** | section 2 below, and `eng/collect-js-evidence.py` |
| **Results** | `build.log`, `suite.log`, `assurance-gate.log`, `assurance-release.log`, `publish-and-run.log`, `catalog-*.txt`, `closure-*.txt` |
| **Negative controls** | none in this bundle (`--skip-controls`) |
| **Closure** | `closure-*.txt`, read off the published output in each mode |
| **Exclusions** | section 5 |

---

## 2. How it was collected

From the component root:

```text
python eng/collect-js-evidence.py --bundle JS-10-004 \
    --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-10-004 \
    --milestone JS-10 --skip-controls --skip-fuzz
```

The tree was commit `88d8b51d` with uncommitted changes, which `identity.txt` lists one by one. The
ones that bear on what was built and checked:

- **`src/Broiler.VM.Profile.JavaScript/JsRealm.AsyncGenerator.cs` and `assurance.manifest.json`.**
  Commit `02ecb4d8` had turned four property initialisers from `= null!;` into `= null;`, which
  raises four CS8625 warnings. No build with warnings as errors compiles that, and the collector
  publishes every root with warnings as errors. The four initialisers are restored and their
  fingerprints regenerated. **A first attempt from a clean clone of `88d8b51d` failed on exactly
  this**, every compiler-bearing root refusing to publish, and was not retained.
- **The three restored closure reports** of JS-1-001, JS-3A-004 and JS-3B-001, and the
  architecture tests and register text changed the same day for rules B5c, X1 and K4.

The rest are documents.

---

## 3. Results

- **Build** (`build.log`): exit 0, with no warning.
- **Suite** (`suite.log`): every contract test passes, and every architecture test but one. **K4
  fails, and that failure is the state this bundle ends.** At collection the register still named
  the old bundles, whose restored closure reports lack assemblies the rows declare; a row cannot
  name a bundle before it exists. Once the rows name this bundle, the architecture suite passes
  in full - a run made after this collection and not retained here.
- **Assurance gate** (`assurance-gate.log`): the same single K4 failure, and nothing else.
- **Assurance release** (`assurance-release.log`): refused, as it must be while no unit is
  human-reviewed (rule J11), beside the same K4 failure.
- **Publish and run** (`publish-and-run.log`): every root published and ran in all three modes,
  every exit zero, and each root's catalog table was identical across its three modes.

**The closures.** Under JIT and trimming:

| Root | Non-framework assemblies in the published image |
|---|---|
| ExecutionOnly | the three core assemblies, the root, `Broiler.VM.Profile.JavaScript` and its Format |
| SliceCompiler | the same, plus the Compiler, `Broiler.VM.Profile.JavaScript.Intl` and `Broiler.VM.Profile.MachineCode` |
| Conformance | the same as SliceCompiler |
| Cli | the same as ExecutionOnly, plus the Compiler and `Broiler.VM.Profile.MachineCode` |

Under Native AOT each image holds no managed assembly. **The SliceCompiler, Conformance and Cli
closures are byte-identical to what the edited reports said**: the edits described today's images
correctly, inside bundles that record earlier ones. The ExecutionOnly closure is byte-identical to
the one JS-1-001 retained.

**The catalogs.** Each binary's table differs from the baseline checked in on 2026-08-31 or
2026-09-03. The profile line reads `1 4` where those baselines read `1 0`, and ExecutionOnly,
Conformance and Cli print the `broiler.javascript.wide` and `broiler.javascript.numeric` manifests
beside `broiler.javascript.slice`, with format versions 1 to 2. PolyglotCli's baseline, retained
from bundle `ubc-4-002`, already carried the same profile line. The four baselines are updated to
these tables in the change that retains this bundle, so rule K3 holds each to what its binary
printed. **The old bundles' catalogs agreed with the old baselines, so while the rows named them
K3 could not show the drift.**

---

## 4. What this bundle replaces, and what it does not

It supersedes no bundle. JS-1-001, JS-3A-004 and JS-3B-001 are restored byte for byte and remain
the record of those images when they were collected. This bundle replaces them only as the
composition register's evidence for the four JavaScript rows.

---

## 5. Exclusions

1. **No negative controls and no fuzzing** (`--skip-controls`, `--skip-fuzz`).
2. **One RID on one machine**: `win-x64`. No other RID is evidenced here.
3. **A dirty tree.** The bundle describes `88d8b51d` plus the entries `identity.txt` lists, not a
   committed state. A collection after those changes are committed would describe a clean one.
4. **The suite and assurance logs carry the K4 failure** that section 3 explains.
5. **The Android root and PolyglotCli are not collected here.** Their rows name their own bundles.
6. **Nothing here is reviewed or accepted**, and no support claim is made.
