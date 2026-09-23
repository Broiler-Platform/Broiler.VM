# Symbol-keyed extensibility and descriptor integrity: JSeal slices V02 and V03

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It relates to the JSP-5 stage and does not mark that stage complete.

## What changed

- **V02.** A Symbol key now takes the same extensibility decision as a String key. Assignment
  (`JsEngine.SetSymbol`) refuses a new Symbol key on a non-extensible, sealed or frozen object:
  silently in sloppy code, as a `TypeError` in strict code (and in `Object.assign`, which writes
  strictly). `Object.defineProperty`, `Object.defineProperties`, `Object.create`'s property map,
  `__defineGetter__`/`__defineSetter__` and a class field named by a computed Symbol throw;
  `Reflect.defineProperty` and a trapless Proxy's `[[DefineOwnProperty]]` answer `false`.
  Internal initialisation (`SetOwnSymbol` on intrinsics, object literals, `Object.fromEntries`)
  is unchanged: the refusal lives in the language-facing callers, not in the storage helper.
- **V03.** Symbol-keyed definitions now go through the same `ValidateAndApplyPropertyDescriptor`
  as String keys (`ObjectValidateAndMerge`, factored out of `ObjectApplyDescriptor` unchanged), so
  frozen values cannot change, non-configurable properties cannot flip configurability,
  enumerability or data/accessor kind, SameValue redefinitions succeed and partial descriptors
  merge. An assignment to an existing own writable Symbol-keyed property now keeps its attributes
  instead of resetting them to the default set (which re-opened sealed properties).
  `Object.freeze`/`Object.seal` on a Proxy now hand the `defineProperty` trap the partial
  descriptor `SetIntegrityLevel` names; trap invariant checks are unchanged and still run.
- The Annex B accessor helpers' String-key path goes through the same checked form, because it
  shares the helper; this is what makes `__defineGetter__/define-non-*` pass.
- **Review follow-up.** A class field whose instance is a Proxy now makes exactly one
  `[[DefineOwnProperty]]` call (the `defineProperty` trap), as `CreateDataPropertyOrThrow`
  specifies. The first version of this patch pre-checked the standing property and the
  extensibility, which also ran the `getOwnPropertyDescriptor` and `isExtensible` traps; that was a
  regression for Symbol-named fields (HEAD called only the `defineProperty` trap) and a
  pre-existing defect for String-named fields, and both helpers are now fixed the same way. A
  Symbol-keyed write on a primitive base now walks the wrapper's prototype, as the String write
  does, so an inherited setter runs with the primitive as `this`; strict code throws only when no
  setter is found.

## What ran (Windows 11, .NET SDK 10.0.401, Release)

- New probe [the-symbol-keyed-integrity.js](../../../src/tests/differential/the-symbol-keyed-integrity.js),
  67 cases. Cases 1-60: on a pre-change build 38 of the 60 cases differed from Node. That build
  was the D:/Broiler.VM main tree at the same HEAD, which also carries other slices' uncommitted
  changes, so it is not a pristine baseline; the untouched worktree build (HEAD only, measured
  when the probe had 57 cases) differed on 36 of those 57. Cases 61-67 were added after review;
  on the first version of this patch 4 of them differed from Node (61 and 62 logged
  `gopd,isExt,dp` instead of `dp`, 64 threw `TypeError` instead of calling the inherited setter,
  65 did not call it). After: `python eng/run-differential.py --only the-symbol-keyed-integrity
  --against node --timeout 20` exits 0, all 67 agree. Each retained answer was checked against
  the specification (OrdinarySet, ValidateAndApplyPropertyDescriptor, SetIntegrityLevel, CreateDataPropertyOrThrow).
- Full retained lane `python eng/run-differential.py --timeout 10`: every probe agrees except two
  pre-existing failures that also fail on the untouched build: `the-later-library-methods`
  (invalid UTF-8 / case 36 cube-root spelling) and `the-json-date-and-regexp-surface` (cases
  158-159, non-ASCII output).
- `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256
  (after the assurance write run and rebuild).
- Test262 (pinned `ccaac100`), bytecode form, selection: `built-ins/Object/{defineProperty,
  defineProperties,freeze,seal,preventExtensions,isFrozen,isSealed,assign,prototype/__defineGetter__,
  prototype/__defineSetter__}`, `built-ins/Reflect`, `built-ins/Proxy/{defineProperty,set}`,
  `language/expressions/assignment`, `language/statements/class/elements`:
  8496 variants, before pass 8188 / fail 293, after pass 8210 / fail 271; 22 variants newly pass
  (Object.assign non-extensible/sealed targets, defineProperty symbol-data-property-writable,
  preventExtensions/seal symbol-object-contains-symbol-properties, freeze/seal
  proxy-with-defineProperty-handler, `__defineGetter__`/`__defineSetter__` define-non-configurable
  and define-non-extensible), 0 newly fail.
  Re-run after the review follow-up: pass 8210 / fail 271, the identical per-variant failure
  set.
- Test262 bytecode form, `language/statements/class/subclass` and `built-ins/Symbol`
  (403 variants), first patch version against the review follow-up: pass 352 / fail 44 /
  skipped 7 both times, no per-variant change.
- Test262 native form (`--form native`), selection `built-ins/Object/{freeze,seal,
  preventExtensions,isFrozen,isSealed,assign}`, `built-ins/Reflect`, `built-ins/Proxy/set`,
  `language/expressions/assignment`: 1838 variants, pass 147 / fail 1687 both before and after,
  no per-variant change. The native form fails most of this selection for reasons outside this
  slice, so it exercises the changed path only weakly.

## Not exercised

Linux; the full Test262 tree; the native form after the review follow-up (it cannot
instantiate the probe script and passes too little of the selection to show the change); retained
conformance floors (partial runs do not touch them); Broiler.JS comparison. Remaining failures in the selection (resizable/BigInt typed arrays,
admission of `AsyncFunction` and related constructors, `seal-symbol.js` on a Symbol primitive,
direct-eval assignment cases, coerced-key ordering) are outside V02/V03 and unchanged.
