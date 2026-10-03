<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Record JSP-7-001

**What this is:** a working record of the JavaScript profile. It retains the evidence for
[the parity roadmap](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.parity.md)'s JSP-7 exit gate,
the surfaces that were absent without being declared, and for Annex B admitted whole in the script goal.
Each change has a fixture in the host's acceptance table, and each fixture is run beside the comparison
engine.

**It moves no row of [the profile's ledger](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.status.md).**
JSP-7 is not a ledger row, it has no owner, and a stage's gate met in a working tree is not acceptance.

**Collected:** 2026-10-03, at commit `bfd6f0c`, from a clean tree, by `collect.py` in this directory,
against the base `a7e69c7`.
- **The change:** `8c9cbec`.
- **This directory's tooling:** `bfd6f0c`. It changes no product, harness or test file.
- **The retaining commit** adds this record's logs and README.

**The collector's verdict was that one step did not answer as expected: the build.** Every other step
answered. Section 2 says what failed and where it is repaired.

## 1. Identity

| Field | Value |
|---|---|
| Record | JSP-7-001 |
| Stage touched | JSP-7 (every item of the exit gate) |
| Correction | [JSC-239](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-239) |
| Compositions | the command-line host, the conformance root, the slice-compiler root, the execution-only root |
| Comparison engine | Node `v22.22.0` |
| Suite | the pinned test262 at `ccaac100`: `test/language`, `test/annexB`, and nine built-in subtrees |
| RID and mode | `linux-x64`, framework-dependent JIT only |
| Owner | the JavaScript profile owner; one person holds every role |
| Reviewer | none |

## 2. The build

`build.log` is the solution built with warnings as errors, and **it exited 1**. The compiler reported
`CS8618` on `Source`, `Flags` and `Matcher` of the realm's `RegExpObject`: the change moved their
assignment from the constructor into `Reinitialise`, so that Annex B's `compile` can set them again,
and nothing told the compiler that the constructor still assigns them. The ordinary build is
unaffected and reports nothing.

The assemblies every later step ran were the change's own. The conformance and slice-compiler roots'
copies of the profile carry the time of this build, and each control rebuilt the host from the
change's source. The defect is in the build's hygiene, not in what runs.

**It is repaired in `c893806`**, which declares the three members on `Reinitialise` and changes nothing
at run time. [Record JSP-10-001](../jsp-10-001/README.md) builds the solution with warnings as errors at
a commit after the repair.

## 3. The changes and what each run shows

`acceptance.log` is every row of [`src/tests/cli/expected.txt`](../../../src/tests/cli/expected.txt)
judged on the host: 298 command lines, all answering as declared. `comparison.log` runs each fixture on
the host and on the comparison engine and says whether the two agree as declared.

| Change | Fixture | What it shows |
|---|---|---|
| Annex B's `String` HTML methods, `trimLeft` and `trimRight`, `getYear`, `setYear`, `toGMTString` and `RegExp.prototype.compile` | `runs/annex-b-string-date-and-regexp-members.js` | the thirteen tags with the attribute escaped, the aliases as the very functions they alias, the two-digit year rule, and `compile` resetting `lastIndex`; the engine's value |
| HTML-like comments in a script | `runs/html-like-comments-in-a-script.js` | `<!--` and a line-leading `-->` are comments; the engine's value |
| The same in a module | `--module refused/an-html-like-comment-in-a-module.mjs` | refused at compile time, where the engine raises a `SyntaxError` |
| The `for (var x = 1 in o)` initialiser | `runs/a-for-in-initialiser.js` | the initialiser runs once, before the object is evaluated, and its value stays when no iteration runs; the engine's value |
| A call as an assignment target in non-strict code | `runs/a-call-as-an-assignment-target.js` | the call runs and a `ReferenceError` follows, with the result never converted and the right-hand side never evaluated, for `=`, a compound assignment, both updates and `for … in`; the engine's value |
| The same in strict code | `refused/a-call-as-an-assignment-target-in-strict-code.js` | refused at compile time with `2205`; the engine raises the `ReferenceError` late, which test262 does not admit |
| The edition's members the realm lacked, and `cleanupSome` | `runs/the-edition-members-the-realm-lacked.js` | `Error.isError`, `WeakMap`'s `getOrInsert` pair and `RegExp.prototype.unicodeSets` present, and `cleanupSome` gone |
| The `Uint8Array` codecs | `runs/uint8array-base64-and-hex.js` | the six members, each the specification's algorithm, and a decode that fails part way writes what it decoded |
| The reviver's context | `runs/a-reviver-reads-the-source-text.js` | a primitive's source text as written, and none for an object or a value a reviver replaced; the engine's value |
| `Error.prototype.stack` declined | `runs/an-error-has-no-stack.js` | no error has an own `stack` and nothing it inherits carries one |

`comparison.log` ends with every one of the ten fixtures agreeing as declared. Six print the engine's
value. The module row and the strict-code row are refusals beside the engine's error. Node 22 lacks
`Error.isError`, `WeakMap`'s pair and the codecs, so those two rows' values come from the specification's
algorithms and the engine's `TypeError` is what agrees. Node gives every error an own `stack`, which
this host declines, and that row is the declared disagreement.

## 4. The suite, the corpus and the probes

`test262-base.log` and `test262-change.log` run the pinned suite on the base commit's conformance root
and on this change's. The trees are `test/language`, `test/annexB`, and the built-in subtrees of
`Uint8Array`, `JSON`, `Error`, `WeakMap`, `RegExp`, `String`, `Date`, `FinalizationRegistry` and
`Function`. That is 29,681 files and 54,845 variants each. The base passes 52,558 variants and the
change 53,074.

`test262-moves.log` lists the 516 variants whose verdict changed. **Every one went from failing to
passing, and none moved the other way.** They sit in the changes' own directories:
- Annex B's `String`, `Date`, `RegExp` and `Function` built-ins;
- Annex B's HTML-like comments, a call as an assignment target, and the `for … in` initialiser;
- the `Uint8Array` codecs;
- `WeakMap`'s pair, `Error.isError`, `RegExp.prototype.unicodeSets` and `JSON.parse`'s reviver.

Of Annex B's 1,350 variants on the change, 1,319 pass, 27 are skipped by feature and 4 spend their
allowance before they decide: the two `RegExp` leading- and trailing-escape files.

- `checks.log` is the slice-compiler root's checks: 589 passing, and 3 not run on this machine and
  claimed by nothing.
- `corpus.log` is the retained corpus. It replays among the execution-only root's checks, and when the
  slice-compiler root re-derives it no entry's bytes differ from the retained ones.
- `differential.log` is the differential probes, answering ok.
- `differential-node.log` is the comparison engine over the probes, on the base host and on this one.
  The change added no finding. It removed one, `node/123`, a `JSON` reviver's context. Its declaration
  was removed from the probe's answer file with the change.

## 5. The controls

Each control is a patch in this directory. `control-<name>.log` applies it, builds the host, judges the
rows this change added to `expected.txt` as a table of their own, reverts it and builds again. Every
control made that table fail, and each failed exactly one row.

| Control | What it reverts | What failed |
|---|---|---|
| `control-uint8-codecs` | the six codecs | the codecs row: a member that is not a function |
| `control-partial-write` | the write before the throw, so a failed decode writes nothing | the codecs row |
| `control-reviver-context` | the reviver's third argument | the reviver row |
| `control-reviver-replaced` | the check that the value is still the one parsed, so a replaced value keeps its source | the reviver row |
| `control-html-methods` | the thirteen HTML methods | the Annex B members row |
| `control-trim-alias` | `trimLeft` as `trimStart` itself, back to a function of its own | the Annex B members row |
| `control-date-year` | `setYear`'s two-digit rule | the Annex B members row |
| `control-gmt-alias` | `toGMTString` as `toUTCString` itself | the Annex B members row |
| `control-regexp-compile` | `compile` resetting `lastIndex` | the Annex B members row |
| `control-html-comments` | HTML-like comments in a script | the comments row: refused |
| `control-html-comments-module` | the module goal reading them as operators | the module row: it runs |
| `control-for-in-initialiser` | the initialiser's evaluation | the initialiser row |
| `control-call-target` | a call as the target of `=` and a compound assignment in non-strict code | the call-target row: refused |
| `control-call-target-update` | the same for an update | the call-target row: refused |
| `control-call-target-strict` | the refusal in strict code | the strict row: it runs to the `ReferenceError` |
| `control-is-error` | the error slot, back to the prototype chain | the edition members row |
| `control-weakmap-upsert` | the upsert answering a key's existing value rather than inserting again | the edition members row |
| `control-unicode-sets` | `RegExp.prototype.unicodeSets` | the edition members row |
| `control-cleanup-some` | `cleanupSome`, put back | the edition members row |
| `control-error-stack` | a `stack` on the error prototype | the stack row |

`rows-after-controls.log` is the added rows after every revert, all answering as declared. `tests.log`
is the solution's test projects, every test passing.

## 6. What this record does not demonstrate

- **Acceptance, an owner, a reviewer.** Nothing here was read by anyone but its author, and no row of
  the ledger moves.
- **A build with warnings as errors at the change.** Section 2 says why and where it is repaired.
- **The four Annex B `RegExp` variants that spend their allowance.** They decide nothing.
- **A rule reading section 6's table of declined surfaces against the realm.** A member declined there
  that later appears would be stale without a rule saying so.
- **The comparison engine as an oracle for the edition's newest members or for a strict call target.**
  Node 22 lacks the members, and raises the strict case late.
- **Other forms, other platforms.** Framework-dependent JIT on `linux-x64` only.

## 7. What was run

```text
python3 docs/evidence/jsp-7-001/collect.py --base a7e69c7 --node /opt/node22/bin/node
```

## 8. Environment

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3;
- Node `v22.22.0`, the comparison engine.
