# Bundle JS-3B-001 — the end-user host, and what it does to real JavaScript

**Collected:** 2026-09-03. **Milestone:** JS-3b. **Owner:** profile architecture owner.
**Reviewer:** none.

**What this bundle is.** JS-3b's composition half: `Broiler.VM.Composition.JavaScript.Cli`, the
first root in this checkout handed **source from outside its own image**, and the first to claim a
deployment label. Point it at a `.js` file and it compiles, verifies, runs and prints the
completion value; point it at a directory and it sweeps every `.js` file under it and prints the
distribution.

**Why it is a new root rather than a mode of an old one.** The composition register recorded that
no root here held `narrow-runtime-compiler`, that the slice-compiler root "is only shaped like
one", and that the label waited on a source surface JS-3b would write. JS-3b wrote it, and what the
label actually waited on turned out to be narrower: **a composition a person hands a path to.**
Every other JavaScript root reads its input from inside its own image — a programmatic builder, an
embedded corpus, a fixture tree this repository also wrote — and a root lowering its own input
cannot demonstrate what the label means. [JSC-56](../../roadmap.corrections.md#jsc-56) records it.

**What this bundle is not.** It is not a conformance score, and this host compares nothing against
a declared expectation: it has no pinned suite, no self-check and no ratchet, which is the
[section 14](../../roadmap.md#14-the-conformance-oracle) harness and a different composition. **It
does not close JS-3b's gate clause either** — the composition exists and runs on one RID, and the
clause asks for every claimed one.

**And it accepts nothing.** No milestone is accepted, the advertised set is still empty, and this
root is a demonstration like every other.

---

## 1. What it does to the Octane benchmark, which is the answer worth having

`octane-sweep-default.log` and `octane-sweep-depth-512.log` are the transcripts. The host was
pointed at a checkout that already existed on this machine, outside this repository; it takes a
path and keeps no copy.

| | Files | Ran | Refused |
|---|---:|---:|---:|
| The Octane checkout, at the default nesting bound of 64 | 24 | **0** | 24 |
| The same, at the largest bound the parser supports | 24 | **0** | 24 |

**Zero, and that is the honest end-user answer.** `broiler.javascript.slice` admits no function, no
object, no string value and no property access. Every file names the first construct it needs, which
is the useful part: `an object literal`, `the construct New`, `a call`, `a function`, `a string
value`.

**Two of the twenty-four change their answer between the two bounds, and that is a finding**
([JSC-57](../../roadmap.corrections.md#jsc-57)). At the default, `earley-boyer.js` and
`mandreel.js` are refused with `2103:NestingTooDeep` — **before the manifest is consulted at all**.
Raise the bound and both get past the parser and are refused by the manifest instead. The construct
census reads at the largest bound deliberately and therefore reports all 24 as parsed, so **the
census measures the language and this host at its default measures the product**, and nothing said
they disagree until now. `NestingTooDeep` is also the less useful diagnostic: it is a ceiling the
specification permits an implementation to have, so it says nothing about the manifest, which is
what a reader wants to know.

**Whether 64 is the right default is not decided by this bundle.** It now has a measurement behind
it and still needs an owner.

---

## 2. How the host is judged, and what is deliberately absent from it

`cli-acceptance.log` is the transcript: **18 of 18 command lines answered as declared.**

**Nothing is injected.** `src/tests/cli/` holds input files; `eng/run-cli-acceptance.py` drives the
**built binary** and judges exit codes and output. No source of this component is patched to make a
case fail, no internal type is reached for, and no JavaScript is embedded in a test method. **A
control here is an input.**

That is forced as much as chosen. The subject is a binary's argument parsing, its seven exit codes,
which of its two streams carries which message, and what it does with a file that is not UTF-8 —
and none of that is reachable from a test project, because rule A11 forbids a test project to
reference a profile assembly, so a test could not compose this host at all.

The eighteen cover, among others:

- **The nesting bound both ways.** The same file reports `2103:NestingTooDeep` at the default and
  prints `1` under `--max-depth 512`, which is the acceptance-suite form of the Octane finding.
- **A byte-order mark runs**, because the language defines U+FEFF as format-control whitespace a
  source text may open with, and a tokenizer handed one would refuse an ordinary file saved by an
  ordinary editor.
- **Bytes that are not UTF-8 are refused, not decoded.** A replacement character changes the
  program; a host that ran a different program than the file contains is worse than one that
  declines.
- **A program with no exit ends by spending its allowance** and says so, in a bounded number of
  instructions rather than a number of seconds.
- **Two files at once report the worst code**, which is how a corpus sweep surfaces one defect
  among many ordinary refusals.

**And the driver was shown to be able to fail.** Pointed at a table of three deliberately wrong
rows it reported 3 of 3 failed, each naming what differed — an exit code, or a missing substring.
A driver whose every row passes cannot otherwise offer that check. The wrong table is not retained,
because it is not part of the suite; the option that reads it is (`--expected`).

---

## 3. The closure, which is what makes the label checkable

`catalog-cli.txt` is what the published image printed and it is byte-identical to the reviewed
baseline at `src/tests/Broiler.VM.Architecture.Tests/catalogs/cli.catalog.txt`, which rule K3
compares in both directions. Its `label` line reads **`narrow-runtime-compiler`** where every
sibling's reads `narrow-runtime-compiler-shaped`.

`closure-cli.txt` is read off the published output in three modes rather than asserted.

**This is the only JavaScript root here whose closure needs no paragraph of exceptions.** The
register records what the others carry beyond the image each demonstrates — a corpus replay,
ordering assertions, a fuzz mutator, a soak, a corpus writer, cross-profile checks, a conformance
harness — all forced there by rules A11 and A12 leaving such code nowhere else. This root reads a
file, compiles it, verifies it, runs it and reports.

---

## 4. Exclusions — what this bundle does not show

- **One machine, one RID.** Everything here was collected on `win-x64`. **JS-3b's gate asks for
  every claimed RID and this is one**, so the clause is narrowed and not closed. The component lane
  publishes and runs the host, and runs its acceptance suite, on every cell of the matrix from this
  change onward — and no run of it existed when this was collected.
- **No third-party suite was run through the host, because none is available.** The Octane checkout
  is local and was read. **test262 is not**: the only cache on this machine is a directory skeleton
  holding three files, so no sweep of it exists and none is reported here. Retrieving it is the
  human action [section 3](../../roadmap.status.md#3-open-external-dependencies) records as open.
- **The host runs no program that returns a value from a function, touches an object, or handles a
  string**, because the manifest admits none of that. Every case in the acceptance suite that
  completes is arithmetic, a comparison or structured control flow. The exit codes for a thrown
  error (1) and for an artifact the verifier refuses (4) are **reachable by construction and
  reached by no case here**: this manifest produces no runtime fault from source, and an artifact
  its own lowering produced and its own verifier refused would be a defect nobody has.
- **The Octane figures are one checkout with no pin and no digest**, because retrieving, hashing and
  archiving is the human action that has not happened. They are a scope input under section 1's
  third category and satisfy no gate.
- **The label is claimed and not reviewed.** Claiming it is a change to a reviewed register, and
  the register's reviewer is the same person as its owner — which
  [JSD-0017](../../decisions/0017-the-end-user-host-and-what-an-exit-code-promises.md) records
  rather than resolves.
- **Nothing is advertised.** This is the first root here a person would expect to be shipped and it
  is not, because a tool advertised as a JavaScript host has to be able to run JavaScript.
