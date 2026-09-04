# Bundle JS-4-001 — the first configuration of this profile that runs JavaScript, and what pointing real material at it cost

**Collected:** 2026-09-04. **Milestones:** JS-4, JS-5 and JS-6 — none of which it closes.
**Owner:** MaiRat. **Reviewer:** none.

**What this bundle is.** The retained record of a second feature manifest,
`broiler.javascript.wide`, and a second bytecode format version, together with the value model,
interpreter, standard library, source front end and two host modes that make them run. It is the
first collection in this profile's series in which **a third-party benchmark and a third-party
conformance suite both go through the ordinary command line and produce output** rather than a
refusal naming a construct. `octane-benchmarks.log` and `test262-subtrees.log` are those two runs,
and they are retained whole — the failing rows with the passing ones.

**What this bundle is not.** Not a conformance score, not a benchmark floor, and not an accepted
anything. **Nothing here has been read by a human**; the reviewer field of `identity.txt` says
`none` and means it. `broiler.javascript.wide` has **no retained run of its own over the whole
pinned suite** — what section 4 below carries is runs over subtrees somebody chose — so roadmap
section 6's rule that a manifest with no retained conformance run of its own is not accepted is
unmet and stays unmet. The ledger's JS-4, JS-5 and JS-6 rows read `In progress` and `[PARTIAL]`
because that is what they are.

**And the tree it was collected from was dirty.** `identity.txt` lists every modified and untracked
path individually rather than counting them, which is what makes the collection re-derivable: the
component commit it names is the parent of this work, not the work.

---

## 1. The required fields, and the file in this directory that carries each

Roadmap [section 4](../../roadmap.status.md#4-required-evidence-bundle) names nine. **A field is
satisfied by a file that was written by the collection, never by a sentence here**, so this table
points and does not transcribe.

| Field | Where it is | What is worth reading in it rather than assuming |
|---|---|---|
| **Identity** | `identity.txt`, `snapshot-identity.txt` | Bundle id, milestone, collection timestamp, component commit, branch, owner and reviewer. The **format version is two of them and the feature manifest set is two of them**, which is the change this bundle is about: version 1 and `broiler.javascript.slice` are unchanged and still exercised, version 2 and `broiler.javascript.wide` are new beside them. The core contract version is 1 and no part of this raises it. The roadmap and its gates carry dates rather than revision numbers, so the authority for both is the commit `identity.txt` names together with the hashes below. |
| **Source** | `identity.txt`, `hashes.txt` | The component commit, the branch, `working tree: DIRTY`, and **every dirty entry listed by name**. `snapshot-identity.txt` re-derives JSD-0005's candidate seed revisions recursively and reports mismatches; a match there is not a taken snapshot, and JS-2 is what takes one. |
| **Dependencies and corpus** | `environment.txt`, `hashes.txt`, `corpus-integrity.log`, `../../../../tests/corpus/js-1/corpus.manifest`, `../../../../tests/conformance/pins/test262.pin` | SDK and toolchain identity; the digest of every project, marker, register and roadmap document the collection hashes. **The retained corpus now holds entries of two format versions, and the `mode` column of its manifest is what distinguishes them** — `default` for the slice's, `wide` for the new ones. The pinned conformance suite is the archive this repository holds beside its pin file; the pin names the immutable upstream revision and the digest taken over the checkout it produces. |
| **Environment** | `environment.txt` | OS, architecture, RID, SDK, installed workloads. One machine, one RID, JIT. |
| **Procedure** | section 2 below, and the header of each workload log | Every command verbatim, with its working directory. The two workload logs open with the command that produced every row beneath. |
| **Results** | section 5 below | Every log named against what it is read for. |
| **Negative controls** | section 6 below; `negative-controls.log`, `corpus-controls.log`, `fuzz-controls.log`, `android-controls.log` | Each control, its injection, and its revert. **The counts are stated by the logs and not here.** |
| **Closure** | section 7 below | There is none, and that is a statement rather than an omission. |
| **Exclusions** | section 8 below | The long section, and the one to read if only one is read. |

---

## 2. Procedure

Every command below was run from the component root, `D:\Broiler.Browser\Broiler.VM`, on `win-x64`,
in `Release`, against the working tree `identity.txt` describes.

**The collection itself**, which wrote every file in this directory except the two workload logs and
this README:

```
BROILER_ASSURANCE_WRITE=1 BROILER_API_WRITE=1 dotnet test Broiler.VM.slnx -c Release
python eng/collect-js-evidence.py \
    --bundle JS-4-001 --milestone JS-4 \
    --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-4-001 \
    --owner "MaiRat" --reviewer "none" \
    --corpus src/tests/corpus/js-1 \
    --skip-publish
```

`--skip-publish` is why section 7 has nothing in it.

**The Octane benchmark**, one benchmark per run, through the end-user host, with no argument that
is not on this line:

```
dotnet run --project src/compositions/Broiler.VM.Composition.JavaScript.Cli -c Release --no-build -- \
    --fuel 1000000000000 --wall 300000 --max-depth 512 \
    <octane>/base.js <octane>/<benchmark>.js src/tests/octane/run-one.js
```

`<octane>` is a checkout that already existed on this machine, **outside this repository, with no
pin and no digest**. The host takes a path and keeps no copy; nothing was fetched and nothing
written back. Retrieving, hashing **and archiving** it is the human action
[section 3](../../roadmap.status.md#3-open-external-dependencies) records as open, and the third of
those three has not happened, so **these figures set no floor and satisfy no gate**.

`src/tests/octane/run-one.js` is this repository's own driver and holds no copy of any benchmark. It
is here rather than in the checkout because Octane's own `run.js` loads every benchmark and reports
a geometric mean, which is not what a profile being brought up wants. **It re-throws after the
harness returns** when a benchmark reported an error, so a run that printed no score exits non-zero
instead of exiting 0 on a failure — the exit column in `octane-benchmarks.log` is only worth reading
because of that.

**test262**, per subtree, through the conformance composition:

```
dotnet run --project src/compositions/Broiler.VM.Composition.JavaScript.Conformance -c Release --no-build -- \
    --test262 <checkout> --dir <subtree>
```

`<checkout>` is the archive this repository holds at
`src/tests/conformance/pins/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e.tar.gz`, extracted to a
scratch directory outside this repository and run from there. Nothing was fetched and nothing
written back. The subtrees are named in the log's header. Each test runs in a fresh realm, in both
the strict and the sloppy variant the metadata asks for, with the harness files evaluated as
separate scripts in that realm as `INTERPRETING.md` requires.

---

## 3. What the two workloads showed, and what they did not settle

**What they are evidence of is the path, end to end, with nothing stubbed in the middle of it.**
Source text that this repository did not write, read by this profile's own tokenizer and parser,
lowered to a format version 2 artifact, handed to the one verifier as bytes with a descriptor,
verified structurally and abstractly, instantiated, and executed by an interpreter charging fuel per
instruction against a budget — and at the end of that, a number that a benchmark harness computed
about itself and a verdict that a conformance test computed about itself. Every link in that chain
is exercised by every row of both logs. Before this collection **no link past the parser had ever
been exercised by material from outside this repository**, because the slice manifest admits no
object, no string, no function and no property access, and the end-user host refused every real file
by name.

**They are also the reason three defects in this checkout are known rather than latent**, and each
is recorded rather than quietly repaired: a declared cancellation poll bound smaller than the
verifier's own largest read ([JSC-72](../../roadmap.corrections.md#jsc-72)); a per-iteration scope
copy on the wrong side of a loop body, which made every closure created in a `for (let …)` loop
share one binding; and a conformance case that **terminated the process**
([JSC-79](../../roadmap.corrections.md#jsc-79)). None of the three is reachable by any test this
repository wrote, because all three needed a program longer and stranger than anything anybody here
would think to write.

**What they do not settle is almost everything a reader will want them to.** A benchmark score is
not a conformance figure and a subtree pass count is not one either. The Octane figures have no pin
and no digest and therefore no floor. The test262 figures are of the subtrees named in the log and
of nothing else: **the subtrees were chosen, and a run over a chosen list measures that list**. No
total anywhere in this bundle is a conformance figure for the suite or for this manifest, and
section 8 says why claiming one would be untrue rather than merely unsupported.

---

## 4. What the `unsupported` verdict is for

`--test262` reports **four** verdicts where the obvious design reports two: `pass`, `fail`,
`unsupported`, `skipped`. The third is the one a reader will most want to misread, so it is worth
stating exactly what it means and what it is protecting against.

**A construct this feature manifest does not admit is neither a pass nor a failure.** A test that
declares a class is not a test this configuration got wrong; it is a test this configuration cannot
be asked. Counting it as a failure would make the manifest's own scope look like a defect rate.
**But counting it as a pass would be much worse**, and that is the reason the verdict exists: a
great many test262 cases expect a `SyntaxError`, and a front end that refuses a class declaration
with a syntax error **produces exactly the outcome such a test wants** — for entirely the wrong
reason. With two verdicts, every unimplemented feature would quietly become a point, and the score
would rise as the implementation stayed still.

So a refusal that names a construct the manifest does not admit is separated out before the verdict
is decided, and it is counted in its own column. **The consequence to keep in mind while reading
`test262-subtrees.log` is that `pass + fail` is not the size of the subtree**, and no ratio computed
from any two of the four columns is a conformance figure.

**The first collection of this bundle showed that separation leaking, and the repair is in this
one.** The verdict is decided on one diagnostic code, `ConstructOutsideManifest` — so it is only as
good as the front end's promise to refuse an unadmitted construct by name. Every front-end refusal
in the first collection's log broke that promise the same way: an `async function` in *expression*
position came back as `ExpectedToken`, naming a token rather than the construct, because the
refusal by name was written at statement position and the expression path fell through to the
generic one.

**Two things followed, and the second was the bad one.** Those variants scored `fail` rather than
`unsupported`, which is pessimistic and harmless. But a test expecting a `SyntaxError` at parse,
whose construct was refused that way, scored **`pass`** — the front end did produce a syntax error,
for entirely the wrong reason. That is the false point this whole verdict exists to prevent, and it
was not hypothetical.

**So the whole refusal surface was audited rather than the one instance repaired.** Every construct
family the manifest excludes was checked against every syntactic position it can appear in, and the
leak was in six of them. What is now refused by name, and was not: `async` functions and `async`
arrows in every expression position; `async` and generator methods in an object literal; `for
await`; dynamic `import()` and `import.meta` as expressions; `await` and `yield` as the callee of a
`new`, which never reaches the operator path that refused them; a destructuring assignment with no
declaration, which was called an invalid assignment target — saying the program was wrong when the
program is fine and this front end is the narrow one; `let` before a binding pattern, which parsed
as indexing; and a label that is a contextual keyword, which kept the statement after it from
being reached at all.

**What the repair moved is stated here with its figures, and this is the one place in this bundle
that does that, because the log the figures would otherwise be read from IS NOT RETAINED.** The
pre-repair collection was overwritten when this bundle was re-collected on the repaired tree, so
there is no before-and-after pair here to compare — only the after, in `test262-subtrees.log`.
Naming the earlier numbers is therefore the only way the claim is checkable at all, and a reader
who wants to re-derive them has to revert the front-end change and re-run. Over three subtrees,
measured before and after on the same pinned checkout:

| subtree | before | after |
|---|---|---|
| `test/harness` | pass 126, fail 74, unsupported 32 | pass 126, fail 38, unsupported 68 |
| `test/language/statements/if` | pass 40, fail 51, unsupported 32, skipped 2 | pass 39, fail 50, unsupported 34, skipped 2 |
| `test/language/statements/while` | pass 38, fail 25, unsupported 8, skipped 1 | pass 37, fail 24, unsupported 10, skipped 1 |

Thirty-six harness variants moved from `fail` to `unsupported` — they were never failures. And
**the pass count went DOWN in two of the three**, which is the result worth reading: those were the
false points, and a repair that makes a conformance figure smaller is the one that made it true.
The admitted programs that share these tokens were checked too and still compile and still answer:
`async(1)` is a call of a function named `async`, `{ async: 1 }` is a property, `{ async() {} }` is
a method, and `let[0]` is indexing an array named `let`.

---

## 5. Results — every log, and what it is read for

The suite, the build and the gates:

- `build.log` — the solution, `Release`.
- `suite.log` — the whole acceptance suite, contract and architecture tests together.
- `assurance-gate.log` — the architecture tests in gate mode.
- `assurance-release.log` — the same tests with `BROILER_ASSURANCE_RELEASE=1`. **This one is
  expected to refuse**, and its own header says so: every relevant unit in this component is
  `HUMAN_PENDING`, the profile's among them, so a release gate that passed here would be the
  defect. What it is read for is that each blocking declaration is named individually rather than
  counted.

The corpus and the fuzzer:

- `corpus-integrity.log` — every retained entry rehashed against `corpus.manifest` and replayed
  against the outcome, reason, diagnostic code and completion the manifest pins. It now spans two
  format versions.
- `fuzz.log` — the fuzz sessions over the corpus.

The two workloads:

- `octane-benchmarks.log` — one section per benchmark, each with the exit code and every line the
  host printed. **A benchmark that does not appear there as `score` did not produce one**, and the
  lines beneath it say what it did instead. The failures are in the file with the successes.
- `test262-subtrees.log` — one section per subtree, each with the exit code, the run's own summary
  line, and the first failures it reported, up to twenty, with a line saying how many more the run
  contains when it holds more than that. **Every subtree in it has failures.**

The controls are section 6. Identity, environment, snapshot identity and hashes are section 1.

**No figure from any of these files is repeated in this README.** Roadmap section 5's update rule 10
is why: a figure restated in prose is a second copy that nothing re-derives, and the two disagree
the first time anything is re-run.

---

## 6. Negative controls

`negative-controls.log`, `corpus-controls.log`, `fuzz-controls.log` and `android-controls.log` each
carry, per control, the injection that must make the suite fail and the revert that must make it
pass again. **Each log states its own count**, and the counts grow across milestones rather than
being restated here. A control that could not be injected is logged as `SKIPPED`, and the log says
in its own words that a skipped control is a gap rather than a smaller total — a row that is a name
with nothing behind it.

**The one new control-relevant fact in this collection** is that the retained corpus gained
version-2 entries, so `corpus-controls.log` now injects into and reverts entries of two format
versions rather than one. That matters because the corpus controls are what prove the replay is
reading the manifest rather than agreeing with itself: mutating an expected diagnostic code must
make the replay fail.

**And one control anchor moved in this change, which is worth reading as a worked example of what
the SKIPPED verdict is for.** Three controls inject by replacing the Android head's row of the
composition register verbatim. The profile gained an optional capability import, every root's
imports cell moved with it, and the first collection of this bundle reported two of those three as
`SKIPPED` — the injection changed nothing, so the row in the log was a name with nothing behind it.
The repair is in `eng/collect-js-evidence.py`: the row is one constant the three injections share,
so a row that moves again is repaired once. **It was deliberately not made a fuzzy match**, because
an anchor that tolerated drift would stop reporting `SKIPPED`, and a control that quietly matches
something adjacent is worse than one that says it found nothing.

`negative-controls.log` also carries its own stated limit about rule N2's cross-family half, which
has a witness input rather than a control because a second profile family does not exist in this
graph. It is repeated in section 8 rather than left only there.

---

## 7. Closure — there is none, and that is a statement rather than an omission

**No Native AOT claim is made in this bundle, and no published image was read.** The collector was
run with `--skip-publish`, so this directory contains no `closure-*.txt`, no `catalog-*.txt` and no
`publish-and-run.log`, and section 4's Closure field is answered by their absence rather than by an
assertion. Native AOT was not published on the machine this was collected on. **The component lane
is the authority for every claim that depends on publishing**, and nothing here narrows or
substitutes for it.

---

## 8. Exclusions — what this bundle does not show

- **No human has read any of it, and nothing is accepted.** The reviewer is `none`. No row moved to
  `Accepted`, `assurance-release.log` refuses as it must, and the advertised composition set stays
  empty. A configuration that runs a benchmark is not a supported one.
- **`broiler.javascript.wide` has no conformance run over the whole pinned suite.** Roadmap
  section 6 does not admit a manifest without one, and this does not supply one. The runs here are
  over the subtrees the log names.
- **The Octane checkout has no pin and no digest.** It is a working tree that already existed on
  this machine, outside this repository. It is a scope input under roadmap section 1's third
  category and satisfies no gate; **no floor is set over any figure in `octane-benchmarks.log`**.
- **The Octane RegExp benchmark runs and fails its own checksum**, and the row is retained rather
  than dropped. RegExp is translated to `System.Text.RegularExpressions` and is declared an
  approximation in the file that does the translating; a benchmark that checksums its own match
  results is exactly the workload that notices.
- **One machine, one RID, JIT.** Everything here is `win-x64` under the JIT.
  [JSC-79](../../roadmap.corrections.md#jsc-79)'s repair is asked by the gate for **every claimed
  RID under Native AOT**, and one machine under JIT does not show it.
- **The stack for a guest invocation is chosen against an estimate, not a measurement.** The
  per-frame cost of this interpreter is not measured anywhere in this bundle. The same discipline
  [JSC-72](../../roadmap.corrections.md#jsc-72) satisfied for two other descriptor rows is not
  satisfied here.
- **Three descriptor rows remain provisional.** Two moved on the strength of construction rather
  than benchmark and say so in [JSC-72](../../roadmap.corrections.md#jsc-72); the rest of the matrix
  is unchanged and unmeasured.
- **JS-2 is still blocked and this does not unblock it.** The wide front end was written in this
  checkout rather than ingested from the seed, exactly as JS-3b's was. `snapshot-identity.txt`
  re-derives the candidate revisions and matches them; **a match is not a taken snapshot**.
- **The front end refuses these by name**, at their own position rather than as an unexpected
  token: a class declaration or expression, `super`, a generator function, an `async` function,
  `await`, `yield`, a module declaration, `with`, `for … of`, an optional chain, a template literal,
  a tagged template, `new.target`, a destructuring pattern, a destructuring parameter, a
  destructuring catch parameter, a rest parameter, a parameter default, and a spread argument,
  element or property. A test that needs one of them is `unsupported` and is counted apart, for the
  reason section 4 gives.
- **These are absent from the realm rather than refused by the front end**, so a program that
  reaches for one gets a `ReferenceError` at run time and not a diagnostic at compile time: `Proxy`,
  `Reflect`, `Symbol`, `BigInt`, every typed array, and `eval`. `$262.evalScript` is bound and
  refuses in its own words. **`Function` is the deliberate exception**: the constructor is bound so
  that `typeof Function` answers truthfully and its prototype hangs off it, and what it does when
  called or constructed is throw a `TypeError` — this manifest declines compiling a string into a
  function at run time, and says so where a guest can see it.
- **And that boundary is enforced by absence rather than at verification, which is not what roadmap
  [section 6](../../roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted) describes
  for it.** That section says a composition declining `broiler.javascript.dynamic` refuses `eval` at
  verification with an invalid-artifact reason, and distinguishes that from a run-time refusal the
  guest may catch. Nothing in a version-2 artifact names `eval` as a construct — a call to it is an
  ordinary global lookup of an ordinary name — so there is nothing for the pass to refuse, and what
  a guest meets is a `ReferenceError` it can catch. This bundle records the difference rather than
  claiming the section's outcome.
- **A function's source text is not kept**, so `Function.prototype.toString` renders every function
  as a native one. That is a stated approximation in the file that makes it: an engine executing
  verified bytecode threw the text away long before a guest could ask.
- **The "refused by name" promise is repaired but is not proven exhaustive.** Section 4 says what
  was audited and closed. Two known leaks are left, both deliberately: a template literal whose
  substitution contains a nested template, a string holding `${`, or a comment holding a brace is
  mis-scanned by the TOKENIZER, so the parser never reaches the by-name refusal templates
  otherwise get; and `await` and `yield` are keyword tokens unconditionally, so `var await = 1` —
  valid in a sloppy script — is refused as a bad binding name rather than admitted or named. The
  first is a tokenizer repair and the second is a decision about a contextual keyword, and neither
  is made here.
- **`Date` fixes the local time zone to UTC**, so every local-time case measures UTC and passes or
  fails for a reason that is not the one it names.
- **`arguments` is unmapped**, so writing to a parameter does not change `arguments[i]` and the
  reverse. And **`arguments` inside an arrow function does not reach the enclosing function's
  arguments object**: a function materialises one only when its own body mentions the name, and the
  scan that decides that does not descend into a nested arrow. Neither target workload reaches it —
  both are written in a dialect with no arrows in it — which is why it is a recorded gap and not a
  repair.
- **An object literal's `__proto__` key is an ordinary property** rather than a prototype
  assignment.
- **Script-level `let` and `const` become properties of the global object** rather than bindings of
  a separate global lexical environment. The observable difference is that a read before the
  declaration answers `undefined` where the language says it throws.
- **There is no job queue**, so a promise never settles and an asynchronous test262 case cannot
  complete. Those cases are not in the subtrees this bundle runs, which means the gap is stated
  here rather than measured anywhere.
- **The retained corpus is small and this bundle does not enlarge that claim.** Eight entries were
  added for format version 2 — five malformed, two mislabelled by the caller, one whole program that
  completes — and one whole program is one, not a body of programs.
- **Rule N2 has a control for its inbound half and none for its cross-family half**, because a
  second profile family does not exist in this graph: an injected edge would name a project that is
  not there and the build would fail before any rule ran. That half has a witness input instead, and
  the control becomes constructible when the WebAssembly profile's JS-0 equivalent lands.
- **Both Android device controls ran and both FAILED, so nothing they name is shown.** This
  machine has an Android SDK, so the controls were injected rather than skipped — and
  `android-controls.log` reports the sentinel ABSENT on the reverted run as well as the injected
  one, which is what a head that never reached a device looks like. The log reads the sentinel's
  absence deliberately, because an application hands no exit code to a harness and a control
  looking for the word `FAIL` would pass on a run that never started. **What that costs here is
  the whole Android half**: the claim that the embedded corpus survives its round trip into the
  image, and the claim that this head detects a real engine regression rather than only its own
  plumbing, are both unshown in this bundle.
- **The tree was dirty at collection.** Every entry is listed in `identity.txt`; the commit named
  there is the parent of this work rather than the work, and re-deriving this bundle means
  reproducing that tree.
