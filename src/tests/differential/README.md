# The differential probes

**Owner:** JavaScript profile owner. **Reviewer:** none.

Probes over the surface `broiler.javascript.wide` admits, run through the end-user host by
[`eng/run-differential.py`](../../../eng/run-differential.py). Each probe prints one numbered line
per case; each has a `.expected.txt` beside it holding what this build answered.
Case IDs may include lower-case suffixes (`39a`, `39b`) used by the existing async probes.

## Why these exist, and what they are not

**They are not a conformance claim.** Conformance is the pinned third-party suite's question and
the harness under `src/compositions/Broiler.VM.Composition.JavaScript.Conformance` asks it. These
are a bring-up instrument: a few hundred cases, written from what the language says, run against a
second engine to find out where this one disagrees.

**They exist because a rule over names cannot see a wrong method.** Rule N17 compares the set of
globals this realm publishes against the set any document claims is absent, and it passes. It has
nothing to say about a prototype that is missing six methods, an argument count that is wrong, or a
method that reads the array-like protocol where the language says the iteration protocol — all of
which were true of this realm while N17 was green, and all of which the first run of
`the-general-surface.js` found *(recorded as [JSC-91](../../Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-91))*.

## Script and module goals

The driver selects an explicit argument template from each probe's `.js` or `.mjs` extension.
VM receives `--quiet` and, for modules, `--module`. Broiler.JS receives `--script-host` or
`--module-host`. Node uses its filename-based goal. Both engines read the **original file**,
so directive prologues, script identity and relative imports are preserved. Dependencies under
`modules/` are hashed in the report and are not run as standalone probes.

Node preloads a separate CommonJS `print` helper through `--require`. It only defines `print`
when missing. Broiler hosts keep their native `print`; no helper is prepended to guest source.
The helper lives in a private platform-native temporary directory, removed after each probe.

## The two comparisons, and why both are needed

`eng/run-differential.py` always compares against the retained answers. That is a **regression**
check: it needs nothing but the built host, and it asserts that this build answers what the build
that retained the file answered.

Given `--against <engine>` it also runs each probe under that engine. That is the question the
retained file cannot answer — whether the answers are **right**. A retained file agrees with itself
by construction, and a component whose only oracle is its own previous output produces its own
claims about JavaScript rather than conformance, which is what bundle JS-4-001 records of every
fixture written here.

## Name the comparison engine

The original declarations were calibrated against Node, as documented in
[JSC-190](../../Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-190). Their reasons
and retained answers are unchanged; the J01 migration explicitly names `node`. The original Node
version is unknown: this migration records provenance, not a fresh validation of those declarations.
A current Node run can legitimately find stale declarations. They never exempt Broiler.JS.

J01 supplies the invocation, portability and timeout repairs described by
[JSC-191](../../Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-191).
The [Windows validation record](../../../docs/evidence/jsp-1-j01/README.md) distinguishes checks
actually run from the Windows/Linux CI lane added but not yet observed. Broader JSP-1 gates,
including the document-absence audit, remain separate.

## Declared divergences are data

An answer file may carry `#diverges <engine> <case> <reason>` lines. Engine names match the
configuration's `name` (`node` or `broiler-js` for the presets). These lines are authored, not
generated, and `--write` preserves declarations for **all** engines. Old unscoped declarations
and duplicate declarations are errors.

- A case named there and differing is reported as a **declared divergence**.
- A case not named there and differing is a **finding**, and the driver exits non-zero.
- A case named there and **not** differing is a **stale declaration**, and the driver exits non-zero
  as well — a declaration nobody removed is a claim about the code that has stopped being true.
- A case missing from either engine fails even if declared; a declaration for an absent case also
  fails. Duplicate IDs, reordered cases, empty output and non-case output cannot pass unnoticed.

Retained host answers remain an independent exact-line check. Only CRLF/LF transport differences
are normalized: numeric spelling, spaces, Unicode and case order are preserved. Stderr is recorded
separately and treated as a failure, even with exit zero. Nonzero exits, missing executables, invalid
UTF-8 and wall-clock timeouts fail visibly. A timeout terminates the engine process tree.

## What is here

Each probe is a surface, and the file's name says which. A probe is APPENDED to and never
renumbered: a case number is how a divergence is named, so inserting one would silently move every
declaration that points at another.

| Probe | What it covers |
|---|---|
| `the-general-surface.js` | The realm as a whole: coercion, the globals, the error objects, the `Object` statics, function objects, the Array, and the name a function takes from what it is bound to |
| `the-statement-and-object-surface.js` | Hoisting and closure capture, labels, `switch`, `try` in every combination, property descriptors and own-key order, accessors, `this`, holes, number formatting, strict mode, the class body, and the two members of an object literal that are not property definitions |
| `the-later-library-methods.js` | The keyed collections and their set operations, the views, `JSON`, the promise as a value, and the members the language added after the first pass over each of them |
| `the-json-date-and-regexp-surface.js` | The pattern protocol, the matcher, the instant, and the number at its edges |
| `the-seam-between-generators-and-the-rest.js` | Where the suspension machinery meets everything merged beside it |
| `the-async-family.js` | `async` functions and `await`: what runs synchronously, what a turn is, and how a rejection travels |
| `the-settling-of-promises.js` | The combinators over the receiver, the capability every static builds through, and the turn a reaction runs on |
| `the-with-statement.js` | The object environment record, and the names a `with` body resolves through it |
| `the-mapped-arguments-object.js` | The mapped `arguments` object: the parameter alias in both directions, duplicate formals, the unmapped kinds, and disconnection by delete and redefinition (JSeal V05/V06) |
| `the-array-species-and-spreading.js` | `ArraySpeciesCreate` in `map`, `filter`, `slice`, `splice`, `concat`, `flat` and `flatMap`, the change-by-copy methods that ignore it, and `Symbol.isConcatSpreadable` |
| `the-numeric-operand-order.js` | Left-before-right operand coercion for the arithmetic, bitwise, shift, relational and compound-assignment operators, and a left-side throw that leaves the right operand unconverted (JSeal V01) |
| `the-symbol-keyed-integrity.js` | Symbol-keyed properties under `preventExtensions`, `seal` and `freeze`: creation, redefinition, deletion and the descriptor rules shared with String keys (JSeal V02/V03) |
| `the-regexp-guards-in-string-search.js` | IsRegExp in `startsWith`, `endsWith` and `includes`, `Symbol.match` overrides, and the `replaceAll`/`matchAll` non-global rejection and dispatch (JSeal V04) |
| `the-binary-species.js` | `Symbol.species` for typed-array `map`, `filter`, `slice` and `subarray`, and for `ArrayBuffer.prototype.slice`, with result validation and detachment (JSeal V10-V12) |
| `the-disposal-stacks.js` | The runtime half of explicit resource management: the disposal Symbols, `SuppressedError`, both disposal stacks and the iterator disposers, without `using` syntax |
| `the-detached-typed-array.js` | A typed array whose buffer was detached: the numeric keys answer empty and every other key stays an ordinary property; numeric keys that are not indices never reach the prototype chain or the ordinary map; the constructors read `new.target.prototype` before their last detach check |
| `the-array-from-async.js` | `Array.fromAsync`: async/sync/array-like input preference, awaited values and mapper results in order, the receiver as constructor, every failure a rejection, and which failures close the iterator (JSeal F16) |
| `the-direct-eval-boundary.js` | Direct `eval` against the caller's bindings: reads, writes, captures, nesting, `with`, `arguments`, `this` and `new.target`, the spread spelling, what stays global, and the shapes still refused by name (JSeal V14, JSD-0026 steps 1-5) |
| `the-direct-eval-in-a-module.mjs` | Direct `eval` under the module goal, which is refused at a module's top level and in its functions, beside the indirect form that is not (JSeal V14, JSD-0026 step 1) |
| `the-front-end-repairs.js` | Front-end repairs: hoisted functions seeing their body's lexical bindings, directive prologues ended only where a semicolon is inserted, one tagged-template strings object per site, strict reserved words as binding names, references and labels, a named function expression's immutable name (also through direct `eval`), and unary operators on the base of `**` (JSeal VM-FIX-D) |
| `the-module-evaluation-errors.mjs` | Several top-level destructuring declarations in a module, and a module whose evaluation threw: later imports reject with the identical value and run no body again, dependents and earlier members of its cycle fail with it, unreached siblings still evaluate, and an import made while the module is still evaluating waits for it (JSeal VM-FIX-D) |
| `the-module-identity.mjs` | One module per key across static and dynamic import, a graph started while a dependency awaits, missing modules, a dynamic cycle, errored modules staying errored for every route, and a cycle member waiting for (or failing with) its cycle root (JSeal I11 upstream) |
| `the-using-declarations.js` | `using` and `await using`: what parses and what is refused early, disposal order and timing on every exit, the loop heads, `catch` and `finally` bodies, how completions combine through `SuppressedError`, and the turns an `await using` owes (JSeal F21-F22, JSD-0034) |
| `the-using-declarations-in-a-module.mjs` | Resources declared at a module's top level, disposed as its body ends and before its evaluation settles (JSeal F22) |
| `the-unwinding-through-finally.js` | The code a `break`, `continue` or `return` runs on its way out: each `finally`, iterator close and disposal runs once, no `catch` of a statement already left sees what it throws, a generator suspended or forced to return in that code, and `await` in a class static block (JSeal F21) |
| `the-string-normalization.js` | `String.prototype.normalize`: receiver and form coercion and their order, the `RangeError` for an unknown form, canonical ordering, compatibility and Hangul forms, supplementary characters, lone surrogates passing through, and expansion-heavy input such as U+FDFA repeated (JSeal F08, JSD-0031 U3) |
| `the-unicode-normalization-*.js` | **Generated; never edit.** Every vector of the pinned Unicode 17.0.0 `NormalizationTest.txt` (parts 0-5, part 1 in four slices) against the UAX #15 conformance conditions, and part 1's claim that every other code point is unchanged, in nine spans of the code space. `UnicodeTableGenerator` writes them from the archive and rule N22 holds them to it byte for byte; each prints a count as case 1 and one case per failure (JSeal F08, JSD-0031 U3) |
| `the-unicode-property-escapes.js` | `\p{...}` and `\P{...}` under `u`: the lone and `name=value` forms, supplementary code points, classes, complements, the early SyntaxErrors of exact name matching, `\p` outside `u`, properties of strings and `v` still refused, `iu` through `CaseFolding.txt` over every `\p{Lu}` pair, and greedy runs over a whole code space (JSeal F09, JSD-0031 U4) |
| `the-unicode-lexical-grammar.js` | The Unicode half of the lexical grammar: literal and escaped identifiers against the pinned `ID_Start`/`ID_Continue` (supplementary ones, private names and escaped reserved words included), `WhiteSpace` as TAB, VT, FF, ZWNBSP and `Zs` (U+0085 refused), a literal's flags as an early error with `v` refused by name, group names from the same sets, the non-`u` Canonicalize from `UnicodeData.txt` over every BMP code unit it moves, and a `u`-mode `lastIndex` inside a surrogate pair (JSeal slice JSD-0031-later) |
| `the-generic-to-json-and-array-search.js` | `Date.prototype.toJSON` on any receiver (ToObject, ToPrimitive with hint number, `null` for a non-finite Number, then `toISOString` invoked), ToObject at the start of `Array.prototype.entries`/`keys`/`values`, `indexOf`/`lastIndexOf` start positions through ToIntegerOrInfinity (-0, the infinities, `lastIndexOf`'s default), `Array.of` over a BigInt typed array constructor, and the array iterator's `LengthOfArrayLike` length and element-free key step (JSeal VM-FIX-J) |

## Running them

```bash
dotnet build Broiler.VM.slnx -c Release
python3 eng/run-differential.py                                  # against the retained answers
python3 eng/run-differential.py --against /path/to/comparison    # and against a second engine
python3 eng/run-differential.py --write                          # retain what this build answered
```

`--write` is for after a deliberate change. Read the diff it produces before keeping it: a probe
whose answers moved because a repair landed and a probe whose answers moved because something broke
look identical in the file and different in the diff.

`--against` alone selects the Node preset. For a current Broiler.JS checkout on Windows:

```powershell
python eng/run-differential.py --against D:/Broiler.JS/Broiler.JS/Broiler.JavaScript/bin/Release/net10.0/BroilerJS.exe --against-kind broiler-js --against-root D:/Broiler.JS --timeout 30 --report artifacts/differential/broiler-js.json
```

On Linux use the equivalent `BroilerJS` apphost without `.exe`. The default VM apphost is resolved
with the current platform's executable suffix. Use `--binary-directory` for another VM build.

For arbitrary engines or `dotnet <assembly>`, use `--host-config <json>` and/or
`--against-config <json>`. Each configuration is explicit argv, never a shell command:

```json
{
  "name": "broiler-js",
  "executable": "dotnet",
  "scriptArgs": ["/absolute/path/BroilerJS.dll", "--script-host", "{probe}"],
  "moduleArgs": ["/absolute/path/BroilerJS.dll", "--module-host", "{probe}"],
  "sourceRoot": "/absolute/path/Broiler.JS"
}
```

`{probe}` is required as a complete argument in both arrays. Optional `{shim}` names the separate
CommonJS print helper for hosts supporting preloading. Bare executable names use PATH; executable
paths and `sourceRoot` are relative to the JSON file. Use absolute paths for other file arguments.
`sourceRoot` is optional; when present, revision and working-tree state are recorded. Custom engine
names can distinguish configurations/versions needing different exemptions.

Every run writes a UTF-8 JSON report (default `artifacts/differential/report.json`; choose distinct
`--report` paths to retain runs). It records platform/Python, source revisions where supplied,
executable and Broiler assembly hashes, argument templates, actual commands, input/dependency hashes,
raw stdout/stderr, exit codes, elapsed time, timeout status and both sides of accepted divergences.
The environment overrides are recorded: `TZ=UTC`, `PYTHONIOENCODING=utf-8`, and
`DOTNET_CLI_UI_LANGUAGE=en-US`. Arbitrary hosts must honor their own UTF-8/time-zone contract;
the driver does not erase host-specific date output or floating-point differences.

`--timeout` is a positive number of seconds per engine/probe (default 30). `--write` cannot be
combined with a comparison engine, so accepting host output never silently skips a requested
comparison. CLI misuse exits 2; failed runs exit 1; successful checks exit 0.

Tooling checks, including small real-engine script/module probes:

```bash
python -m unittest discover -s eng/tests -p test_differential.py -v
python eng/run-differential.py --probe-directory src/tests/differential/smoke --against node
```

The smoke module retains correct live-binding and top-level-`this` answers. Its authored
`broiler-js` declarations expose the current source engine's differences; Node receives no such
exemptions. These fixtures validate the instrument and do not imply the full corpus passes.
