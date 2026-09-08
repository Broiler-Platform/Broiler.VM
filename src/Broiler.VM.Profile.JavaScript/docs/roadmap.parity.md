<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The parity roadmap — what standing level with the comparison engine would take

**What this document is.** A gap analysis and a proposed programme for one objective: that a
program which runs on the legacy component `Broiler.JS` runs the same way on this profile, or meets
a refusal that names what it asked for. It is written from a run of both engines over the same
sources on the same machine, one construct at a time, and it names for every gap either the
milestone or stage that already owns it or the fact that nothing does.

**What this document is not.** It is not the ledger and it moves no row in one:
[section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) remains the only
authority on what this component has done. It is not a milestone set —
[section 19 of the delivery file](roadmap.delivery.md#19-milestones) holds `JS-0` through `JS-10`
and this document mints no identifier in that namespace. And it is not a second workload roadmap:
[that document](roadmap.workloads.md) owns the large absences its own stages name, and where a gap
below is one of them this document points at it rather than re-minting it. Its stages are `JSP-n`
and they are proposals for where the existing milestones would have to grow.

**And it carries no figure of any kind** — no count, no score, no pass rate, no ratio. The runs
behind it were taken outside any retained bundle, and under the ledger's
[update rule 10](roadmap.status.md#5-update-rules) a number with no retained record behind it is not
a number this document family may state. Where a figure would be the natural way to say something,
this document names the construct and the command instead, so a reader re-derives it rather than
trusting it.

---

## Contents

1. [The target, and the word "parity" is doing work](#1-the-target-and-the-word-parity-is-doing-work)
2. [How the comparison was made, and the two ways it had been made wrongly](#2-how-the-comparison-was-made-and-the-two-ways-it-had-been-made-wrongly)
3. [The finding that reframes the rest](#3-the-finding-that-reframes-the-rest)
4. [The gap, by kind](#4-the-gap-by-kind)
5. [What already has an owner](#5-what-already-has-an-owner)
6. [The stages](#6-the-stages)
7. [Order, and what is schedulable today](#7-order-and-what-is-schedulable-today)
8. [What this roadmap does not promise](#8-what-this-roadmap-does-not-promise)
9. [What a stage would owe if it were scheduled](#9-what-a-stage-would-owe-if-it-were-scheduled)

---

## 1. The target, and the word "parity" is doing work

**A program is at parity when the two engines answer it the same way, or when this profile refuses
it by name.** The second clause is not a softening of the first. This profile admits a language
surface through a feature manifest and
[section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted) makes a refusal a
first-class answer: a construct outside the manifest is refused at compile time, naming the
construct, so a composition can decline a surface and say so. An absence that announces itself is a
supported outcome. **An absence that produces a plausible wrong value is not**, and section 3 below
is about the one place that distinction had been lost — [section 4.2](#42-the-refusal-that-was-lost)
is that place, and **the refusal was restored there on 2026-09-08**, on the strength of this
paragraph and nothing else. Until that date this sentence ended *"the one place that distinction has
been lost"* in the present tense, and it was true when it was written
*(corrected: [JSC-207](roadmap.corrections.md#jsc-207))*.

**Parity is not imitation, and this is the clause a reader is most likely to skip.** The comparison
engine is older than the edition this profile is written against, and running both over the same
sources found it answering a great many questions in ways the language does not permit — in the
suspension machinery, in `Annex B` block-scoped function semantics, in what `JSON.stringify` puts
between members, in the name an anonymous function takes from what it is assigned to, in whether a
`Date`-only string is read as UTC. **Those are not gaps in this profile and no stage below repairs
them.** Section 4.9 catalogues them precisely, and
[JSP-9](#jsp-9--what-must-not-be-taken-from-the-comparison-engine) exists so the catalogue is
maintained rather than rediscovered: a stage that
closed a difference by copying the comparison engine's answer would move this profile *away* from
the language.

**Parity is also not speed.** The two engines execute by different models —
[section 2](roadmap.md#2-engineering-invariants) says why this one verifies a bytecode artifact and
interprets it under a budget — and nothing here is justified by a measurement of throughput.
Baselines are `JS-10`'s.

---

## 2. How the comparison was made, and the two ways it had been made wrongly

**Both engines were run over the same file, and neither was read to decide what it does.** The
profile's end-user host and the comparison engine's both take a path and both publish a `print`
global, so one probe is portable:

```bash
# this profile; --quiet drops the completion value the host prints and the other does not,
# and --check compiles and verifies WITHOUT running, which is the only way to tell a
# front-end refusal from a binding that is merely absent from the realm
<Composition.JavaScript.Cli> [--quiet|--check|--all|--slice|--strict|--module] <file>

# the comparison engine; the goal comes from the FLAG and not from the file name
<BroilerJS> --script-host <file>     # script goal
<BroilerJS> --module-host <file>     # module goal
```

**Asking "is it absent?" by running a program gives the wrong answer**, which is why `--check` is in
every probe that touches a refusal: every absent binding makes a program fail, so a probe that only
runs things confirms whichever reading it started with.

**The instrument this component already owns is the differential probes** —
[`src/tests/differential/`](../../tests/differential/README.md) and `eng/run-differential.py`, which
compares this build against retained answers always and against a second engine when one is named.
Using it found two things about the instrument itself, and both matter more than any single case it
reports.

**The retained divergences are calibrated against a different engine than the documents say.** Each
answer file may carry authored `#diverges <case> <reason>` lines, and their reasons speak of "the
comparison engine" throughout. Run the driver against Node and every one of them holds exactly as
written, down to the wording — the members it calls absent are absent there, and the `import.meta`
it describes as carrying "a URL, a path and a resolver" is Node's. Run it against `Broiler.JS` and a
large part of that same set is stale, because the legacy engine *has* the members the declarations
say the comparison engine lacks. The driver's own usage line names a Node path. Meanwhile
[section 2 of the workload roadmap](roadmap.workloads.md#2-what-the-comparison-engine-admits-and-what-that-comparison-is-worth)
says the legacy component is the comparison it was asked for. **Two documents in this component say
"the comparison engine" and mean two different engines**, and nothing anywhere records which one a
given claim was taken against. That is the defect [JSP-1](#jsp-1--the-instrument-name-the-engine-and-make-the-comparison-runnable)
repairs, and it is a documentation defect with the same shape as a silent wrong answer.

**And the driver could not be pointed at the legacy engine correctly at all.** It invokes the named
engine with the file and no flag, so a script probe is not run under the script goal and a `.mjs`
is not run under the module goal — on a false premise its own comment states, that the comparison
engine takes its goal from the file name. Under the module goal the legacy engine hangs forever on
the probe's import cycle, and the driver passes no timeout. It also cannot run on `win-x64` at all:
it looks for the composition binary without the platform's executable suffix and exits before doing
any work, and its scratch directory is a POSIX path that on Windows is drive-relative. ADR 0012
declares `win-x64`, so a developer told to run this script on a declared platform cannot.

**One consequence for the retained answers themselves.** Running the regression half on this machine
disagrees with the retained file in two cases that are the machine and not the build: a
one-unit-in-the-last-place difference in a cube root, and a non-ASCII answer mangled by the console
code page. The retained answers are therefore not reproducible on an arbitrary declared platform,
which is a property the regression check is assumed to have and does not.

---

## 3. The finding that reframes the rest

**Where the existing probes look, this profile is not behind the comparison engine — it is ahead of
it.** Running the retained probes under both engines and adjudicating every disagreement against the
language found the overwhelming majority of them to be the comparison engine's defects, a handful to
be the local machine or a genuinely implementation-defined choice, and almost none to be this
profile's. On the module goal, which one document still records as a surface this host does not
have, the profile answers every case and the comparison engine cannot complete the probe at all.

**That is not the good news it looks like, and reading it as such is the trap.** The probes were
written from the surface this profile admits, and a probe written from what an implementation does
cannot ask about a mechanism the implementation has never had. The differences that matter are
almost all in questions the probes do not contain — and asking those questions, one specification
operation at a time, found a great deal. **The instrument agreeing with itself is what
[bundle JS-4-001](evidence/js-4-001/README.md) records as producing this component's own claims
about JavaScript rather than conformance**, and the shape of that hazard here is a probe suite that
passes because of what it omits.

**So the gap is not where the existing record says it is.** It is not, any longer, mostly a list of
constructs the front end refuses: section 4.1 records that the families
[section 3.3 of the workload roadmap](roadmap.workloads.md#33-the-syntax-that-is-refused-by-name)
names as refused by name are now admitted, every one of them. It is a small number of whole types
and surfaces that are absent, and a much larger number of *mechanisms inside the surface that is
present* — protocols the realm publishes and does not consult, abstract operations implemented as a
near neighbour of themselves, and integrity clauses on the object model that are not enforced.

---

## 4. The gap, by kind

Every row below was observed by running both engines. Nothing here is a status, and nothing here is
accepted.

### 4.1 What is no longer a gap, and what that costs the record

**Every construct family the workload roadmap lists as refused by name now compiles.** Asked one at
a time under `--check`, the wide front end admits the class declaration and expression, `super`,
`new.target`, class fields, class static blocks, private names and the brand check, generator
functions, `async` functions, `await`, `yield` and `yield*`, async generators, `for … of`,
`for await … of`, optional chaining, template literals and tagged templates, destructuring in every
position, rest and spread in every position, parameter defaults, the module declaration forms,
dynamic `import()`, and `with`. **The realm has grown the same way**: `Proxy` and `Reflect` are
present, and the workload roadmap's section 3.2 still lists both as absent.

**A document that has gone stale in this direction is more dangerous than one that never said
anything**, because it is the document a plan is written from. Rule **N17** exists to stop exactly
this and does not reach it: it checks the ledger's own machine-readable absent-globals block against
the set the realm publishes, and the workload roadmap's prose is a second claim about the same
subject that nothing checks. That is [JSP-1](#jsp-1--the-instrument-name-the-engine-and-make-the-comparison-runnable)'s
second clause.

### 4.2 The refusal that was lost

**This section's finding was acted on in the working tree on 2026-09-08, and the two paragraphs
below are kept exactly as they were written rather than repaired in place** — a reader who planned
against them is owed the reading and the retraction, not a section that has quietly stopped saying
what it said. **Read them in the past tense**, and read the record that closes this section beside
them. The heading is unchanged because two documents outside this file link to it by anchor
*(corrected: [JSC-207](roadmap.corrections.md#jsc-207))*.

**A BigInt literal is admitted by the front end and evaluated as a Number.** `typeof 1n` answers
`"number"`; `1n === 1` is `true`; `9007199254740993n` — a value chosen because no Number can hold
it — answers a different integer; `1n + 1` produces `2` where the language requires a `TypeError`;
`JSON.stringify(1n)` produces output where the language requires a throw. The `BigInt` global is
absent and the ledger's absent list says so, so the *binding* is declared — but the *literal* is
not refused, and the two together mean a program doing big-integer arithmetic silently gets wrong
numbers rather than meeting an absence.

**This is the one finding in this document that breaks a property the profile has rather than
missing one it never had.** [Section 3.3 of the workload roadmap](roadmap.workloads.md#33-the-syntax-that-is-refused-by-name)
calls refusal by name a property the programme must not spend, and this is it, spent — in the one
place where the front end admits a construct the value model cannot represent. The contrast is
visible one line away: the regular-expression `v` flag is *also* unimplemented, and it is refused
with its own diagnostic naming the flag. The host's own usage text, meanwhile, describes a wide
manifest that admits neither async functions nor class fields nor `Proxy`, all of which run.

**The refusal was restored on 2026-09-08, and what changed is narrower than deleting the paragraphs
above would suggest.** The superseded reading is the one quoted at the head of this section: *"A
BigInt literal is admitted by the front end and evaluated as a Number"*, with `typeof 1n` answering
`"number"`, `1n === 1` answering `true`, and `9007199254740993n` answering `9007199254740992`.
Observed in this working tree on 2026-09-08, `print(typeof 1n)` now answers
`2104:ConstructOutsideManifest at 1:14: a BigInt literal is not admitted by the declared feature
manifest`. **The refusal is restored; the value kind is not implemented**, and this section did not
ask for it to be: BigInt arithmetic is a manifest widening nothing has scheduled, and
[JSP-2](#jsp-2--the-refusal-that-was-lost-a-bigint-literal-is-not-a-number)'s gate is split in two
precisely so that the half that needs no type, no manifest identity and no decision could be taken
on its own. **The second half is untaken and is not closer to being taken.** So `1n + 1` does not
produce `2` any more — it produces no program at all — and the language's `TypeError` is still not
raised, because there is no BigInt to raise it about.

**What decided it is this document's own section 1 and not a measurement.** A refusal is a supported
answer and a plausible wrong value is not; the machinery for the refusal already existed and was
merely not reached, which [section 4.7](#47-where-the-profile-contradicts-itself) had recorded in
one line of its own. The change is one arm of the wide parser's primary-expression switch in
`src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs`, reading the `n` the tokenizer already
consumes and leaves on the token's raw text — `SliceTokenizer.FinishNumeric` says in its own comment
that it does so *so that a parser can notice it*, and until this date only the slice parser did.

**And it cost the pass total, which is the part a record of this change must not omit.** Over
`test/language/literals/bigint` the variants that had passed by accident and the variants that had
failed silently both became variants meeting a refusal that names the construct, so the subtree's
pass column fell and its `unsupported` column stopped being empty. **A smaller pass total that is
honest beats a larger one that is not**, and section 1 is the rule that says so rather than a
preference expressed here. **This document states no figure for any of it**, as its own header
promises: the movement is measured in
[the evidence ledger](roadmap.status.md#2-current-milestone-status) and in
[JSC-207](roadmap.corrections.md#jsc-207), and a reader who wants the numbers runs
`eng/run-test262.py` over that subtree either side of the change rather than trusting a count copied
into prose. The whole-suite figure after this change is being re-taken and no document in this
family carries one. **Nothing here is accepted, no row moved, and no bundle retains a byte of it.**

### 4.3 The types and surfaces that are absent

Asked with `typeof`, which answers for an undeclared name without throwing so that one absence
hides no other, the wide realm lacks these names that the comparison engine has:

- **`BigInt`**, with `BigInt64Array` and `BigUint64Array`, and the `DataView` big-integer accessors.
  Declared absent in the ledger; section 4.2 is what the declaration does not cover. **From
  2026-09-08 what it does not cover is the value kind alone**: the *literal* is refused by name from
  that date, so this row is an absence that announces itself in both halves rather than in one. Until
  then this bullet's second sentence covered a silent wrong value as well
  *(corrected: [JSC-207](roadmap.corrections.md#jsc-207))*.
- **`Intl`**, and with it every locale-sensitive method's behaviour: `localeCompare` is an ordinal
  comparison over code units, `toLocaleLowerCase` and `toLocaleUpperCase` ignore their locale
  argument, `Number.prototype.toLocaleString` and `Date.prototype.toLocaleString` ignore theirs, and
  `Array.prototype.toLocaleString` does not exist at all.
- **`Temporal`**. Declared absent, and deferred by name.
- **`SharedArrayBuffer` and `Atomics`**. Declared absent, and excluded deliberately.
- **`Float16Array`**, with `DataView`'s float16 accessors — and **this one is declared nowhere**:
  it is on no absent list and in no manifest, so it is a surface that stays out without being named.
- **The resizable `ArrayBuffer`**, whose options bag the constructor accepts and ignores rather than
  rejecting, and the `Uint8Array` base64 and hex methods.
- **`JSON.rawJSON` and `JSON.isRawJSON`**, and the reviver's source-text argument.
- **`Error.prototype.stack`**, which the comparison engine populates with named frames. Already a
  declared divergence, and named here because it is the most-reached-for member on the list.
- **Several `Annex B` members**: the `String` HTML-tag family, `String.prototype.trimLeft` and
  `trimRight`, and `Date`'s `getYear`, `setYear` and `toGMTString` — while other `Annex B` globals
  such as `escape` are present, so the surface is admitted in part without a rule saying which part.
- **The recent editions' additions**, absent as a group: the `Iterator` global and the iterator
  helpers on it, `Array.fromAsync`, `Error.isError`, `Symbol.dispose` and `Symbol.asyncDispose`
  with the `DisposableStack` pair, and `SuppressedError`. The comparison engine has all of them.
- **Three syntactic surfaces the comparison engine compiles and this front end refuses**: a `using`
  declaration, a decorator, and an `accessor` class element. All three are refused at compile time
  and one of them names a manifest exclusion, so this is the refusal property working — they are
  listed here because they are the surface a program written for the comparison engine will meet,
  not because the refusal is wrong.

**And one whole dependency reaches further than the stage that owns it.** The Unicode character
database this component has not acquired is already an open external dependency and already blocks
`JSW-4`. Its absence is *also* why `normalize` throws for any non-ASCII string, why `\p{…}` property
escapes are refused, why case conversion implements only the one-to-one mappings, and why one
identifier-start classification disagrees. That is a wider blast radius than a regular-expression
stage, and section 5 records it as owned-but-under-scoped rather than as new work.

### 4.4 The mechanisms the realm publishes and does not honour

**This is the largest kind, and none of it is visible as an absence.** Each of these is a protocol
the language defines, whose hook exists in the realm, and which the implementation does not consult:

- **`ArraySpeciesCreate` is not implemented.** `map`, `filter`, `slice`, `splice`, `concat`, `flat`
  and `flatMap` read the receiver's `constructor` zero times, so every one of them returns a plain
  `Array` from an `Array` subclass, and a `Symbol.species` accessor is never invoked. The same
  omission reaches `%TypedArray%.prototype.slice`, `subarray` and `map`, `ArrayBuffer.prototype.slice`,
  and `RegExp.prototype[@@split]`.
- **`Array.from` and `Array.of` ignore their `this`**, always constructing a plain `Array`.
- **`Symbol.isConcatSpreadable` is never read** — its getter is not invoked at all, so the flag is
  ignored in both directions.
- **`Function.prototype[Symbol.hasInstance]` does not exist**, so it can be created by assignment
  and thereby change `instanceof` for every function in the realm.
- **`Symbol.toStringTag` is missing from the builtins the language requires it on**, while
  `Object.prototype.toString` reports the tags anyway from a brand check — so `JSON` answers
  `[object JSON]` with no tag present, and assigning the symbol hijacks the answer.
- **The iterator prototype chain is collapsed.** The four kind-specific iterator prototypes and
  `%IteratorPrototype%` are one object, so a method installed for one built-in iterator is installed
  for all of them, and the chain the language specifies is not observable.
- **The `IsRegExp` check is absent** from `String.prototype.startsWith`, `includes` and `endsWith`,
  which answer `false` where the language requires a `TypeError`; `replaceAll` ignores a `RegExp`
  search value entirely rather than throwing for a non-global one.
- **The mapped `arguments` object does not exist.** In sloppy code with a simple parameter list a
  parameter and its `arguments` slot do not alias in either direction.
- **Function name inference is incomplete**: an anonymous function gets no name through a computed
  key, through a class field initialiser, or through a logical assignment — and a symbol-keyed
  method in an object literal gets none while the same key in a class body does, so the profile
  disagrees with itself.
- **Object spread does not copy symbol-keyed own enumerable properties**, and `Array.from` does not
  close its iterator when the mapping function throws.

### 4.5 The operations underneath, and the integrity clauses on top

**Two abstract operations are implemented as near neighbours of themselves**, and both are reachable
from ordinary code:

- **`ToLength` is `ToUint32`** throughout the array-like surface. A `length` the two conversions read
  differently is mishandled everywhere it is used, and a negative `length` becomes a value near
  2³², so a read-only generic over `{length: -1}` scans that many indices rather than none.
  `Function.prototype.apply` over the same object spends the whole allowance.
- **`ToPropertyKey` runs twice** on a computed key in a compound assignment, an increment or a
  decrement, and in a logical assignment that takes the write path — so a key object's
  `toString` is observed twice where the language observes it once.

And the evaluation order and the operators around them:

- **The right operand is coerced before the left** for every binary numeric operator except `+`, and
  for `>` and `>=`, so the wrong side's exception escapes.
- **`delete` through a primitive base answers `true`** without performing `ToObject`, and
  `delete undefined.x` answers `true` where the language requires a `TypeError`.
- **`Object(sym) == sym` is `false`**: the Object-versus-Symbol case of loose equality is missing.
- **`-2 ** 2` and `typeof 2 ** 2` compile**, where the grammar makes both an early error, and the
  profile picks its own grouping.

**The object model's integrity clauses are the other half**, and these are the ones that make
`freeze` mean less than it says:

- **The extensibility check is skipped for symbol-valued keys**, so a new symbol-keyed property can
  be added to a frozen, sealed or non-extensible object, through assignment and through
  `Object.defineProperty` and `Reflect.defineProperty` alike.
- **A typed array's `[[DefineOwnProperty]]` never rejects.** Out-of-bounds defines succeed, in-bounds
  non-writable and accessor descriptors are accepted, and `Object.freeze` returns normally over a
  still-mutable array.
- **Only non-negative integer strings are canonical numeric indices** on a typed array, so `"-1"`,
  `"-0"`, `"NaN"` and `"1.5"` become real own properties on one.
- **The array append path bypasses `OrdinarySet`**: an inherited setter is not called, defining an
  index past a non-writable `length` silently does nothing while `Reflect.defineProperty` reports
  success, and `pop`, `shift` and `splice` on a sealed array no-op or corrupt it instead of throwing.
- **`for … in` enumerates a stale snapshot**: a property deleted before the iterator reaches it is
  still visited, and mid-loop demotion to non-enumerable is ignored.
- **A `String` object's exotic indices misbehave**: redefining one with an identical descriptor adds
  a duplicate own key, and an added array-index property sorts after `length`.
- **`Map`, `Set`, `WeakMap` and `WeakSet` constructors resolve the adder on the intrinsic prototype**
  rather than on the object being constructed, so a subclass's `set` or `add` is not called; and
  symbols are rejected as weak keys and `WeakRef` targets.

### 4.6 The static semantics, and where declarations live

- **The legacy octal escape is not implemented anywhere.** `"\101"` decodes as the three characters
  rather than `"A"` in sloppy code, and none of the strict-mode early errors for `\1`–`\7`, `\8` and
  `\9` are raised. A tagged template with an illegal escape yields a decoded string where the cooked
  value must be `undefined`.
- **Several strict-mode early errors are missing**: most of the future reserved words are accepted as
  binding names on the wide surface, the restricted-name rule is not applied at the name position of
  a function or class, and a parameter named `arguments` or `eval` is accepted when the body carries
  the directive.
- **Two structural early errors are missing**: a class body with two `constructor` methods compiles,
  and a label duplicated by a nested labelled statement is accepted.
- **`??` unparenthesised beside `||` or `&&` compiles**, and an arrow function with a line terminator
  before `=>` compiles; both are early errors.
- **There is no temporal dead zone for `let`, `const` and `class` declared directly in a function
  body**: `typeof` answers `"undefined"` and a read resolves rather than throwing.
- **Global `eval` puts its declarations in the wrong place.** A `let`, `const` or `class` declared
  inside a direct `eval` at global scope leaks into the enclosing lexical scope and survives; a
  strict `eval`'s `var` becomes a global object property instead of staying in the eval's own
  variable environment; and a `var` declared where a global `let` of that name exists overwrites the
  lexical binding instead of raising an early error.
- **A direct `eval` inside function code is unimplemented**, throwing a catchable `EvalError` at run
  time rather than being refused at compile time or evaluated.
- **`for … in` with a `let` binding creates no per-iteration environment**, so every closure captures
  the last key, and the head expression resolves in the enclosing scope.
- **The name binding of a named function expression is mutable**, where the language makes it
  immutable.

### 4.7 Where the profile contradicts itself

These are not comparisons with anything. They are places the component disagrees with its own
documents or its own components, found while comparing:

- **The host's usage text describes a manifest that is not the one it runs**, naming async functions,
  class fields, private names and `Proxy` as outside the wide surface when all of them run.
- **A top-level `for await` in a module makes the lowering emit an artifact this component's own
  verifier rejects** — exit 4, which the host's own contract reserves for a defect in this host
  rather than in the input. That is the shape [JSC-81](roadmap.corrections.md#jsc-81) records for
  `pdfjs`, in a construct nothing had asked before.
- **The `v` flag passes `--check` and throws at run time**, so a refusal that is a compile-time
  refusal for every other unimplemented flag is invisible to compile-time verification for this one.
- **The `d` flag is accepted and `hasIndices` answers `true`**, but a match carries no `indices`.
- **Two refusal diagnostics degrade to token level** rather than naming the construct: a rest-element
  early error reports an expected bracket, and under `--slice` a class static block is refused as an
  unexpected brace. Naming the construct is the property section 3.3 of the workload roadmap calls
  load-bearing.
- **`Object.prototype.toString` answers `[object Error]` and `[object Date]` for the prototypes**,
  which carry no such internal slot, and `[object global]` for the global object with no tag on its
  chain.
- **`FinalizationRegistry.prototype.cleanupSome` is shipped** and is not in the language — a
  feature-detection hazard rather than a wrong answer, and the only *library* member found in that
  direction.
- **Every host capability is a function that throws.** `read`, and all of `$262`'s callables —
  `createRealm`, `evalScript`, `detachArrayBuffer` — are present and refuse unconditionally, so
  `typeof` cannot distinguish a capability this host has from one it does not, and a conformance
  harness that feature-detects will take the wrong branch. One of them refuses with a reason that is
  not true of this realm: `detachArrayBuffer` says the profile has no `ArrayBuffer` to detach, in a
  realm where `ArrayBuffer` exists and its own `transfer` detaches successfully.
- **The host runs several named files in sorted path order** while its usage text promises the order
  given — and that text is the one [JSC-75](roadmap.corrections.md#jsc-75) exists to state.
- **`--slice` disagrees with the wide surface about the completion value** of a statement whose
  completion is empty, renders negative zero differently for identical arithmetic, and refuses the
  identifier `undefined` at compile time although the usage text says the slice manifest admits it.
- **The front end knows what a BigInt literal is**: it names the construct in a `--slice` refusal,
  while the wide surface — whose own manifest text excludes BigInt — admits it silently. The
  machinery for the refusal of section 4.2 already exists and is not reached. **This is the one
  entry on this list that has been repaired: the wide parser reaches that machinery from 2026-09-08**
  and both surfaces now refuse the literal by name, which is the disagreement closing rather than
  being explained *(corrected: [JSC-207](roadmap.corrections.md#jsc-207))*.
- **`GeneratorFunction`, `AsyncFunction` and `AsyncGeneratorFunction` refuse with a reason about
  turning source into code at run time**, which is the dynamic surface's reason rather than theirs.

### 4.8 The host, and what an embedder meets

**A language comparison misses this whole kind, and a user does not.** These are differences in what
the two hosts do rather than in what the language says, and each is reachable without writing
anything unusual:

- **The default instruction allowance refuses an ordinary loop.** A three-million-iteration
  accumulation exits with the allowance-spent code, where the comparison engine completes it in
  about a second. The allowance is a declared property of this host and not a defect — but a
  default that an unremarkable loop exceeds is a default a first-time embedder meets before
  anything else, and no document records that it is reachable by ordinary code.
- **`Math.random` returns a fixed sequence.** It is identical on every process and identical in two
  distinct realms of one process. The language requires distinct realms to produce distinct
  sequences, and an implementation is expected to choose its seed. This is the one finding in this
  document a reader should not have to be told twice about: any guest relying on unpredictability
  gets none.
- **Three artifact-format ceilings surface as refusals with no basis in the language**: a call with
  more than a couple of hundred arguments is refused at compile time while the identical call
  through `apply` is admitted; a program with more than about sixty-five thousand distinct literal
  constants is refused with a diagnostic reported at position zero and no source location; and
  top-level `await` is refused inside a template-literal substitution while admitted in every other
  top-level position, with a diagnostic that names the wrong reason.
- **Source encoding differs**: a UTF-16LE file with a byte-order mark is unreadable to this host and
  runs on the comparison engine.
- **A throw in shared-realm multi-file mode abandons every remaining file**, and the diagnostic names
  the whole file list as the site of the error rather than the file that threw.

### 4.9 What must not be taken from the comparison engine

**The comparison engine is wrong more often than this profile is, in the surface both admit.** These
are recorded so that a stage closing a difference never closes it by imitation, and the list is
[JSP-9](#jsp-9--what-must-not-be-taken-from-the-comparison-engine)'s to maintain:

- **A `with` whose head completes abruptly poisons the global variable store.** The next indirect
  `eval` installs a snapshot of the global `var` bindings taken at the instant the head threw, and
  it is permanent rather than saved and restored. It was found because it silently re-numbered a
  probe's own case counter, which is the shape of hazard worth naming: every result taken after it
  in the same process is untrustworthy, and nothing reports that it happened.
- **The suspension machinery disagrees with the language in several places** — an async function
  that catches a rejected `await` and returns a value fulfils with `undefined`; `await` of a
  fulfilled native promise costs an extra turn while a thenable costs one too few; `yield*` over an
  async generator takes the wrong value; an async generator's `return` does not await its operand;
  a `for await` closes an iterator the language forbids closing.
- **`JSON.stringify` separates members with the platform's line ending** rather than the one the
  language mandates.
- **`Annex B` block-scoped function semantics are wrong in the common case**, where an inner
  declaration should overwrite an outer function of the same name and does not.
- **Function name inference is missing in the ordinary `var f = function () {}` case.**
- **A date-only string is parsed as local time** where the language requires UTC.
- **Several early errors are not raised**: a catch block redeclaring its parameter, an arrow body
  redeclaring a parameter, `arguments` in a class static block, `delete this.#x`, two `default`
  clauses in a `switch`.
- **`Promise.all` does not pass elements through the constructor's `resolve`.**
- **A cyclic import hangs forever**, an exported `async function*` does not parse, an arbitrary
  module namespace name does not parse, and an `import` declaration in a script terminates the
  process with an unhandled platform exception.
- **Ordinary functions carry own `caller` and `arguments` properties** the language removed, and the
  global object carries names the language deliberately does not expose.
- **`Object.getOwnPropertyNames(console)` throws**, and the message leaks a platform type name and a
  source path — as does every error it raises, which bakes its own build-tree source paths into
  `stack` and hands them to ordinary guest code.
- **`console.log` writes nothing to either stream and returns its argument instead**, under the
  script goal — so a program that reports through `console` produces no output at all. This one is
  worth knowing before taking any comparison run, because a harness that routes its output through
  `console` will read a silent engine as an engine that answered nothing.
- **A source file containing an invalid byte is decoded with a substitution character and run**, so
  it executes a program the file does not contain; this profile refuses it.
- **Its own property enumeration is not sound in two places**: `Object.getOwnPropertyNames` over the
  `String` prototype reports one name twice, and `for … in` does not enumerate through a `Proxy` in
  the prototype chain at all — the traps are never called.
- **The recursive built-ins have no depth guard**, so a deeply nested structure through
  `JSON.stringify` or `Array.prototype.join` overflows the platform stack and terminates the
  process, where the same engine guards its guest-level calls.

---

## 5. What already has an owner

**Nothing below is re-minted, and a stage in section 6 that touches one of these says so.**

| The gap | Who owns it |
|---|---|
| `Intl` and `Temporal` absent | Deferred by name in [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted); the workload roadmap's [section 7](roadmap.workloads.md#7-what-this-roadmap-does-not-promise) declines to promise either |
| `SharedArrayBuffer` and `Atomics` absent | [JSW-2](roadmap.workloads.md#jsw-2--the-binary-surface-and-a-manifest-identity-for-it) excludes both by name; the agent model of [section 13](roadmap.md#13-realms-agents-and-the-host-boundary) owns them |
| The Unicode character database | An open external dependency in the ledger's [section 3](roadmap.status.md#3-open-external-dependencies), blocking [JSW-4](roadmap.workloads.md#jsw-4--regular-expressions-over-the-from-scratch-matcher) — **but scoped there to the matcher, and section 4.3 above shows it reaching `normalize`, case conversion, collation and one identifier table** |
| `eval` and the `Function` constructor behind an identity | [JSW-3](roadmap.workloads.md#jsw-3--the-dynamic-surface-and-a-refusal-that-happens-where-the-plan-says-it-does) and `JS-8`. **The scoping defects of section 4.6 are not that stage's**: they are wrong answers inside the `eval` that exists |
| The binary surface and its manifest identity | [JSW-2](roadmap.workloads.md#jsw-2--the-binary-surface-and-a-manifest-identity-for-it). **The exotic-object integrity clauses of section 4.5 are its gate's**, and are recorded here because that gate is unmet |
| The realm publishing its own admitted set | [JSW-6](roadmap.workloads.md#jsw-6--the-core-library-still-absent-from-the-realm), which is where the completeness of the absent list belongs |
| The module goal | [JSW-8](roadmap.workloads.md#jsw-8--the-module-goal). **Its premise has been overtaken**: the goal exists and runs |
| Per-frame cost and recursion depth | [JSW-9](roadmap.workloads.md#jsw-9--the-depth-a-generated-program-needs). Proper tail calls are absent from this profile and present in the comparison engine, and that is a depth question rather than a new surface |
| Throughput | `JS-10`, and nothing here |

---

## 6. The stages

Each states an objective, what it waits on, and an exit gate written as conditions a run can decide.
**None is scheduled and none has an owner**; assigning either is the act that would turn a stage into
a milestone with a ledger row.

### JSP-1 — The instrument: name the engine, and make the comparison runnable

- **Objective.** A reader can tell which engine any recorded divergence was taken against, and the
  comparison can be re-run on a declared platform against either.
- **Waits on.** Nothing.
- **Exit gate.** The retained `#diverges` reasons name their engine, and a declaration stale against
  one engine and holding against another is expressible rather than being deleted — the retained
  answers currently carry declarations that would be removed as stale by a run against the legacy
  engine and are correct against the calibration engine. `eng/run-differential.py` resolves the
  composition binary on `win-x64`, uses a scratch directory that exists on every declared platform,
  passes the goal flag the named engine needs, carries a timeout so an engine that hangs fails
  rather than wedging a lane, and pins the child environment's time zone and encoding so a retained
  answer is reproducible off the machine that wrote it. The driver's comment claiming the comparison
  engine takes its goal from the file name is corrected. Rule **N17** — or a rule beside it — reads
  every document that claims a name is absent rather than the ledger's block alone, and is watched
  failing against an injected stale claim.

### JSP-2 — The refusal that was lost: a BigInt literal is not a Number

- **Objective.** No program silently gets a wrong number. A BigInt literal is either refused by name
  at compile time or evaluated as a BigInt; it is never a Number.
- **Waits on.** Nothing for the refusal. The type itself waits on the value representation of
  `JS-4`, and a manifest identity for it is a decision this stage takes rather than assumes.
- **Exit gate.** The cheap half first and separately: `1n` is refused at compile time with a
  diagnostic naming the construct, `--check` decides it, and a retained corpus entry carries it.
  Then, if the type is admitted rather than refused: `typeof` answers `bigint`, mixing with a Number
  throws, strict equality across the types is `false` and loose equality is `true`, a value past the
  Number range round-trips, `JSON.stringify` throws, and the literal forms including the hexadecimal
  one are exercised. A negative control for each half, watched failing and watched passing after the
  revert. **And the host's usage text is made to describe the manifest it runs**, because the same
  change that admits or refuses a construct is the one that can be checked against the prose.
- **Where this stands on 2026-09-08, stated in the direction that costs.** The first clause of the
  cheap half is met in the working tree — `1n` is refused at compile time with
  `2104:ConstructOutsideManifest` naming the construct, and `--check` decides it — and **the rest of
  the gate is not**: no retained corpus entry carries the refusal, no negative control has been
  watched failing and watched passing after a revert, the host's usage text is untouched, and the
  type half is exactly where it was. **Meeting a clause of a gate is not meeting the gate and is not
  acceptance**, which [section 8](#8-what-this-roadmap-does-not-promise) already says of every stage
  here; this stage has no owner and moved no row. The conformance consequence of the one clause that
  was taken is in section 4.2 and in the [ledger](roadmap.status.md#2-current-milestone-status).

### JSP-3 — The static semantics the wide front end does not have

- **Objective.** The wide surface raises the early errors the language requires, and its string
  lexer decodes what the language says it decodes.
- **Waits on.** `JS-3b`, whose subject static semantics as a verification stage is.
- **Exit gate.** Per family, because a set-level gate lets the last family in be the one nobody
  exercised: the legacy octal and non-octal decimal escape sequences decode per `Annex B` in sloppy
  code and are early errors in strict code; a tagged template with an illegal escape has an
  `undefined` cooked value and its raw text; the strict-mode reserved-name and restricted-name rules
  apply at every binding position including the name of a function or class and a parameter under a
  body directive; a duplicate `constructor`, a duplicate nested label, `??` beside `||` or `&&`, a
  unary operand of `**`, and a line terminator before `=>` are each refused; and the temporal dead
  zone exists for a lexical declaration in a function body. Each family is exercised by a fixture in
  this repository and **no family moves to refused-as-an-unexpected-token on the way** — which the
  two diagnostics named in section 4.7 already do and which this stage repairs.

### JSP-4 — The abstract operations underneath the library

- **Objective.** The operations the library is written on top of are the language's, so a defect in
  one stops being a defect in every method that calls it.
- **Waits on.** `JS-6`.
- **Exit gate.** `ToLength` is `ToLength` and not `ToUint32`, asserted at every array-like call site
  by a test over the lengths on which the two disagree, including a negative one and one past 2³²;
  `ToPropertyKey` is observed exactly once for a computed key in every compound and logical
  assignment and in increment and decrement, asserted by a key whose `toString` counts; operand
  evaluation is left before right for every binary operator and every relational one, asserted by
  which side's exception escapes; `delete` performs `ToObject` and answers per the language for a
  primitive and for `undefined`; and loose equality covers the Object-versus-Symbol case.

### JSP-5 — The integrity clauses on the object model

- **Objective.** `freeze`, `seal` and `preventExtensions` mean what they say, for every key kind and
  every exotic object this profile has.
- **Waits on.** `JS-4`. The typed-array half is [JSW-2](roadmap.workloads.md#jsw-2--the-binary-surface-and-a-manifest-identity-for-it)'s
  gate and is restated rather than re-minted.
- **Exit gate.** The extensibility check applies to symbol-valued keys through assignment,
  `Object.defineProperty` and `Reflect.defineProperty`; a typed array's `[[DefineOwnProperty]]`
  rejects an out-of-bounds index and a non-writable or accessor descriptor, and `Object.freeze` over
  a non-empty one throws; the canonical-numeric-index test admits the negative, negative-zero,
  non-finite and fractional forms so none of them becomes an own property; the array append path
  goes through `OrdinarySet` so an inherited setter runs, a define past a non-writable `length` is
  reported as it happened, and a mutator on a sealed array throws rather than corrupting it; and
  `for … in` observes deletion and demotion during enumeration. A negative control per clause.

### JSP-6 — The protocols the realm publishes and does not consult

- **Objective.** A well-known symbol installed by a guest changes what the realm does, everywhere the
  language says it does.
- **Waits on.** `JS-6`, and `JS-4` for the intrinsic graph.
- **Exit gate.** `ArraySpeciesCreate` is performed by every method the language specifies it in,
  across `Array`, `%TypedArray%`, `ArrayBuffer` and the `RegExp` protocols, asserted by a subclass
  and by a `Symbol.species` accessor that counts its invocations; `Array.from` and `Array.of` use
  their `this`; `Symbol.isConcatSpreadable` is read in both directions; the iterator prototype chain
  has the links the language gives it, asserted by installing a method on one kind's prototype and
  finding it absent from another's; `Symbol.toStringTag` is present on every builtin the language
  requires it on and `Object.prototype.toString` answers from the tag rather than from a brand
  check; `Function.prototype[Symbol.hasInstance]` exists and is non-writable; the `IsRegExp` check is
  performed by the `String` methods that require it; the mapped `arguments` object aliases its simple
  parameters in both directions; object spread copies symbol keys; an iterator is closed when a
  mapping function throws; and name inference covers the computed key, the class field initialiser
  and the logical assignment — with the object-literal and class-body symbol cases agreeing, since
  they currently do not.

### JSP-7 — The surfaces that are absent without being declared

- **Objective.** Every surface this profile does not have is named somewhere a reader and a rule can
  find, with its deterministic failure.
- **Waits on.** [JSW-6](roadmap.workloads.md#jsw-6--the-core-library-still-absent-from-the-realm) for
  the mechanism; this stage is the content.
- **Exit gate.** `Float16Array` and the float16 `DataView` accessors, the resizable `ArrayBuffer` and
  its ignored options bag, the `Uint8Array` base64 and hex methods, `JSON.rawJSON` and `JSON.isRawJSON`
  with the reviver's source argument, `Array.prototype.toLocaleString`, `Error.prototype.stack`, and
  the `Annex B` `String` and `Date` members are each either admitted or named as an exclusion with the
  failure a program meets; the rule that compares documents against the realm passes over the
  completed list; the constructor stops accepting an options bag it ignores; and the `Annex B`
  surface has a stated rule for which members are in rather than a partial set nobody chose.
  **`FinalizationRegistry.prototype.cleanupSome` is removed or recorded as a deliberate extension**,
  since a member this realm has and the language does not is the same hazard in the other direction.

### JSP-8 — The places this component disagrees with itself

- **Objective.** No document, diagnostic or usage text in this component describes a surface other
  than the one it has, and no lowering emits an artifact this component's own verifier rejects.
- **Waits on.** Nothing.
- **Exit gate.** The top-level `for await` artifact is diagnosed and repaired as
  [JSW-1](roadmap.workloads.md#jsw-1--the-two-defects-the-workloads-already-found) requires of a
  defect of that shape — a recorded entry naming which component was wrong, a fixture, and a control
  that produces a wrong answer rather than an exception; the `v` flag is refused where every other
  unimplemented flag is refused, at compile time; the `d` flag either produces `indices` or does not
  report `hasIndices`; the two token-level refusal diagnostics name their construct; the prototypes
  of `Error` and `Date` and the global object answer `Object.prototype.toString` from what they are;
  and the host's usage text is generated from the manifest or asserted against it, so the two cannot
  drift again.

### JSP-9 — What must not be taken from the comparison engine

- **Objective.** The catalogue of section 4.9 is maintained, so that a later stage closing a
  difference never closes it by imitation.
- **Waits on.** [JSP-1](#jsp-1--the-instrument-name-the-engine-and-make-the-comparison-runnable),
  because a catalogue that cannot say which engine it is about is not one.
- **Exit gate.** Every difference in section 4.9 is a declared divergence in the retained answers
  with the engine named and the language's own requirement stated, so that the driver reports it as
  declared rather than as a finding; a difference that stops holding is reported as stale rather than
  silently kept; and the poisoned-global-store hazard is recorded where a person taking a comparison
  run will meet it, because a result taken after it in the same process is worthless and nothing says
  so.

### JSP-10 — The host surface an embedder meets first

- **Objective.** The host's defaults, its capabilities and its format ceilings are things a reader
  can find out before meeting them, and none of them is a surprise reachable by ordinary code.
- **Waits on.** Nothing for the reporting half. The allowance defaults are a decision rather than a
  repair and belong with whoever owns [ADR 0004](decisions/0004-limit-defaults-hard-maxima-and-the-budget-matrix.md)'s
  budget matrix.
- **Exit gate.** `Math.random` is seeded per realm, so two realms in one process and two processes
  produce different sequences, asserted by a test that would fail against a fixed sequence — this
  clause is severable and should not wait for the rest. A host capability that is not implemented is
  **absent rather than present-and-throwing**, so `typeof` feature detection takes the right branch,
  or the profile states in its own documentation that it deliberately does the opposite and why; and
  no refusal reason names a cause that is untrue of the realm it is raised in. The three
  artifact-format ceilings — the argument count, the constant-pool size, and the top-level `await`
  position — are either raised, or refused with a diagnostic that names the ceiling and carries a
  source location, and each is recorded as a declared property of the format rather than met by a
  program that had no way to know. The default allowance is documented with a program that reaches
  it, so *reachable by ordinary code* is a stated property rather than a discovery. Several named
  files run in the order given, or the usage text stops promising it. And the source-encoding set the
  host reads is stated, with a file outside it refused by naming the encoding.

---

## 7. Order, and what is schedulable today

**Four stages need nothing that does not exist.** [JSP-1](#jsp-1--the-instrument-name-the-engine-and-make-the-comparison-runnable)
repairs an instrument; [JSP-8](#jsp-8--the-places-this-component-disagrees-with-itself) repairs
disagreements internal to this checkout; **the first half of
[JSP-2](#jsp-2--the-refusal-that-was-lost-a-bigint-literal-is-not-a-number) — refusing the literal —
needs no type, no manifest and no decision**, which is the point of splitting its gate in two —
**and it was taken on 2026-09-08**, so that clause of this paragraph reads in the past tense now
while the rest of JSP-2's gate stays open, which is the whole reason the split was worth drawing
*(corrected: [JSC-207](roadmap.corrections.md#jsc-207))*; and
**the `Math.random` clause of [JSP-10](#jsp-10--the-host-surface-an-embedder-meets-first) is one
change to one function**, which is why that gate says so rather than leaving it inside a stage that
waits on a budget decision.

**JSP-1 comes first and not because it is small.** Every other stage's evidence is a comparison run,
and the runs available today cannot say which engine they were taken against, cannot be executed on a
declared platform, and are not reproducible off the machine that produced them. A programme whose
measurements are ambiguous will produce corrections that are ambiguous too.

**None of them waits on the snapshot.** `JS-2` is `Blocked` in the ledger and stays blocked; the
argument that this does not block the work is [JSC-15](roadmap.corrections.md)'s, and it applies here
for the third time — every surface named above is written from the specification rather than ingested.

**The rest are ordered by what they unblock.** [JSP-4](#jsp-4--the-abstract-operations-underneath-the-library)
comes before [JSP-6](#jsp-6--the-protocols-the-realm-publishes-and-does-not-consult), because a
protocol built on an operation that is a near neighbour of itself will be re-tested when the operation
is corrected. [JSP-5](#jsp-5--the-integrity-clauses-on-the-object-model) and
[JSP-3](#jsp-3--the-static-semantics-the-wide-front-end-does-not-have) are independent of both and of
each other. [JSP-7](#jsp-7--the-surfaces-that-are-absent-without-being-declared) is content for a
mechanism `JSW-6` owns and cannot close before it.
[JSP-10](#jsp-10--the-host-surface-an-embedder-meets-first) is independent of every language stage
and touches none of their code. [JSP-9](#jsp-9--what-must-not-be-taken-from-the-comparison-engine)
closes over all of them.

**One family has a blocker outside this component**, and it is larger than the stage that currently
carries it: the Unicode character database blocks `JSW-4`'s matcher, and also `normalize`, case
conversion, collation and one identifier classification. No sequencing here moves it.

---

## 8. What this roadmap does not promise

- **It does not promise imitation.** Section 4.9 is the list of answers this profile must keep
  giving differently, and a stage that closed one of them by copying would be a regression.
- **It does not promise `Intl` or `Temporal`**, which are deferred by name, or
  `SharedArrayBuffer` and `Atomics`, which are excluded deliberately.
- **It does not promise speed**, and no stage is justified by a measurement of one.
- **It does not accept anything.** No stage moves a ledger row, and a stage's exit gate being met is
  not acceptance: acceptance needs an owner and a reviewer decision, which nothing in this component
  has.
- **It does not claim the gap list is complete.** It is the gap the probes written for it found, and
  section 3 is the standing warning that a probe suite cannot ask about a mechanism nobody thought
  to look for. The completeness of this list is a property of the questions asked, and the questions
  are in this repository so the next reader can ask better ones.

---

## 9. What a stage would owe if it were scheduled

Nothing here changes the obligations any work in this component already carries, restated because a
stage list invites the reading that a plan replaces them:

- A retained bundle per [section 4's](roadmap.status.md#4-required-evidence-bundle) nine fields,
  collected by this profile's own script, with its failures and exclusions retained rather than
  summarised.
- A registered rule in the architecture rule register for every new mechanism, with a witness whose
  file name starts with the rule identifier, and negative controls that have been **watched failing
  and watched passing after revert** — a control nobody has seen fail is worth nothing, and a control
  that makes the process crash has judged nothing.
- Appended entries in [the corrections file](roadmap.corrections.md), never edits to old ones, with
  the plan pointing at them by bare marker.
- Ledger rows moved in the same change that changes what they claim, and no count, total or score
  copied into prose anywhere.
