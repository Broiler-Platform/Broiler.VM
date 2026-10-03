<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Record JSP-MECH-001

**What this is:** a working record of the JavaScript profile. It retains the evidence for the clauses of
[the parity roadmap](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.parity.md)'s JSP-4, JSP-5
and JSP-6 exit gates that still did not hold: the abstract operations under the library, the integrity
clauses on the object model, and the protocols the realm published and did not consult. Each change has
a fixture in the host's acceptance table, and each fixture is run beside the comparison engine.

**It moves no row of [the profile's ledger](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.status.md).**
None of the three stages is a ledger row, none has an owner, and a stage's gate met in a working tree is
not acceptance.

**Collected:** 2026-10-03, at commit `a7e69c7`, from a clean tree, by `collect.py` in this directory,
against the base `30529e0`.
- **The change:** `b59a147`.
- **This directory's tooling:** `a7e69c7`. It changes no product, harness or test file.
- **The retaining commit** adds this record's logs and README.

## 1. Identity

| Field | Value |
|---|---|
| Record | JSP-MECH-001 |
| Stages touched | JSP-4, JSP-5 and JSP-6 (the clauses that still did not hold) |
| Corrections | [JSC-236](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-236), [JSC-237](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-237), [JSC-238](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-238) |
| Compositions | the command-line host, the conformance root, the slice-compiler root, the execution-only root |
| Comparison engine | Node `v22.22.0` |
| Suite | the pinned test262 at `ccaac100`: `test/language`, `test/annexB`, and thirteen built-in subtrees |
| RID and mode | `linux-x64`, framework-dependent JIT only |
| Owner | the JavaScript profile owner; one person holds every role |
| Reviewer | none |

## 2. The changes and what each run shows

`build.log` is the solution built with warnings as errors. `acceptance.log` is every row of
[`src/tests/cli/expected.txt`](../../../src/tests/cli/expected.txt) judged on the host: 288 command
lines, all answering as declared. `comparison.log` runs each fixture on the host and on the comparison
engine and says whether the two agree as declared.

| Stage | Change | Fixture | What it shows |
|---|---|---|---|
| JSP-4 | `apply` reads its list's length with `ToLength`, and a list past the ceiling is a `RangeError` | `runs/apply-reads-its-length-with-tolength.js` | a length of `-1` is an empty list and `2**32 + 2` is too long to call with, as the engine answers |
| JSP-4 | a `super[k]` that is read and then written converts `k` once, after reading the base | `runs/a-super-key-converted-once.js` | each compound, update and logical form counts one conversion, and the base is read before the key's `toString` re-points it |
| JSP-4 | `delete` converts its base with `ToObject` | `runs/delete-through-a-primitive-base.js` | deleting through a primitive answers as the engine does, and through `undefined` or `null` throws |
| JSP-5 | `for … in` asks for each name when it reaches it | `runs/a-for-in-that-observes-deletion.js` | a name deleted, or made non-enumerable, before it is reached is not visited |
| JSP-5 | a `String` object's exotic keys, in the language's order, and validated rather than stored by a define | `runs/a-string-object-keeps-its-own-keys.js` | the index keys come before `length` and before keys added later, and redefining an index throws |
| JSP-5 | a keyed collection's constructor builds from `new.target` and calls its own adder | `runs/a-collection-subclass-uses-its-own-adder.js` | a subclass's overridden `set` or `add` is what the constructor calls, as the engine does |
| JSP-5 | a Symbol `Symbol.for` did not make can be held weakly | `runs/a-symbol-held-weakly.js` | as a `WeakMap` key, a `WeakSet` member, a `WeakRef` target and a registry target; a registered symbol is refused |
| JSP-6 | `Function.prototype[Symbol.hasInstance]` | `runs/function-prototype-has-instance.js` | it exists with every attribute off, carries its name and length, and `instanceof` consults it |
| JSP-6 | an object spread copies enumerable Symbol-keyed properties | `runs/an-object-spread-copies-symbol-keys.js` | the Symbol key arrives, and a non-enumerable one does not |
| JSP-6 | an anonymous function is named by a computed key and by a class field, and a literal's computed key is converted before its value | `runs/a-function-named-by-a-computed-key-or-a-field.js` | the names the engine gives, and a key converted once |

`comparison.log` ends with every one of the ten fixtures agreeing as declared. Eight print the engine's
value. Two print the specification's, and the record declares the disagreement:
- **`a-super-key-converted-once.js`.** The engine converts the key twice and reads the base after the
  key, so it prints `compound 11:2` and `base -99` where the host prints `compound 11:1` and `base 2`.
- **`a-for-in-that-observes-deletion.js`.** The engine visits a name made non-enumerable after the loop
  began, so it prints `a,b,c` where the host prints `a,c`.

The pinned suite asks both questions and passes the host's answer.

## 3. The suite, the corpus and the probes

`test262-base.log` and `test262-change.log` run the pinned suite on the base commit's conformance root
and on this change's. The trees are `test/language`, `test/annexB`, and the built-in subtrees of
`Function`, `Map`, `Set`, `WeakMap`, `WeakSet`, `WeakRef`, `FinalizationRegistry`, `String`, `Object`,
`Reflect`, `Array`, `Symbol` and `Proxy`. That is 34,670 files and 64,757 variants each. The base
passes 62,768 variants and the change 62,920.

`test262-moves.log` lists the 152 variants whose verdict changed. **Every one went from failing to
passing, and none moved the other way.** They sit in the changes' own directories:
- object literals and classes, for the names and the computed keys;
- `Function.prototype`, for `Symbol.hasInstance`;
- `WeakMap`, `WeakSet`, `WeakRef` and `FinalizationRegistry`, for Symbols held weakly;
- `delete`, `super`, `new`, `call` and array literals, for the conversions;
- `for … in`, `for … of`, and `Object.getOwnPropertyNames`, for enumeration and a `String`'s keys.

- `checks.log` is the slice-compiler root's checks: 589 passing, and 3 not run on this machine and
  claimed by nothing.
- `corpus.log` is the retained corpus. It replays among the execution-only root's checks, and when the
  slice-compiler root re-derives it no entry's bytes differ from the retained ones.
- `differential.log` is the differential probes, answering ok.
- `differential-node.log` is the comparison engine over the probes, on the base host and on this one.
  The change added no finding. It removed two, `node/295` and `node/301`, the function names a computed
  key gives. Their declarations were removed from the probe's answer file with the change.

## 4. The controls

Each control is a patch in this directory. `control-<name>.log` applies it, builds the host, judges the
rows this change added to `expected.txt` as a table of their own, reverts it and builds again. Every
control made that table fail, and each failed exactly one row.

| Control | What it reverts | What failed |
|---|---|---|
| `control-apply-length` | `ToLength`, back to `ToUint32` | the `apply` row: `-1` read as four billion |
| `control-apply-ceiling` | the `RangeError` past the ceiling | the `apply` row: the allowance spent on a list too long to call with |
| `control-super-keep-key` | the instruction that keeps the converted key, so the write converts it again | the `super` row |
| `control-super-base-first` | the base read before the key | the `super` row |
| `control-delete-to-object` | `ToObject` on the base | the `delete` row |
| `control-for-in-deleted` | the check at reach time that a name is still there | the `for … in` row |
| `control-for-in-demoted` | the enumerability read at reach time, back to a snapshot at the start of the object | the `for … in` row |
| `control-string-define` | the validation of a define on a `String`'s index key | the `String` row |
| `control-string-order` | the index keys first, with `length` moved ahead of them | the `String` row |
| `control-collection-new-target` | the prototype from `new.target` | the collections row: the subclass's adder not called |
| `control-weak-symbol` | a Symbol as a weak key at all | the weak-symbol row |
| `control-weak-registered` | the refusal of a registered Symbol | the weak-symbol row |
| `control-has-instance` | `Function.prototype[Symbol.hasInstance]` | the `hasInstance` row |
| `control-spread-symbols` | the Symbol-keyed properties in a spread | the spread row |
| `control-name-computed-member` | a function named by a literal's computed key | the names row |
| `control-name-field` | a function named by a class field | the names row |
| `control-name-computed-field` | a function named by a computed class field key | the names row |
| `control-field-key-once` | a class element's key converted once | the names row |
| `control-literal-key-first` | a literal's computed key converted before its value | the names row |

`rows-after-controls.log` is the added rows after every revert, all answering as declared. `tests.log`
is the solution's test projects, every test passing.

## 5. What this record does not demonstrate

- **Acceptance, an owner, a reviewer.** Nothing here was read by anyone but its author, and no row of
  the ledger moves.
- **The clauses of the three gates that already held.** They were read again and are not re-run here.
- **An object rest that reads an excluded key's getter once.** It still reads it twice, as
  [JSC-238](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-238) records.
- **The comparison engine as an oracle for a super key or a demoted name.** On both the engine and the
  specification disagree, and this host follows the specification.
- **Other forms, other platforms.** Framework-dependent JIT on `linux-x64` only.

## 6. What was run

```text
python3 docs/evidence/jsp-mech-001/collect.py --base 30529e0 --node /opt/node22/bin/node
```

## 7. Environment

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3;
- Node `v22.22.0`, the comparison engine.
