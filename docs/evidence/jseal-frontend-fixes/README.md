# Wave 1-2 follow-up fixes: JSeal VM-FIX-D (front end and compiler)

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It is a set of follow-up correctness fixes found during waves 1-2, not a numbered roadmap slice.
It refines JSP-2/JSP-3 (front end, early errors), JSP-6 (bindings and references) and the module
stage of JSW, and does not mark any of those stages complete.

## What changed

1. **A hoisted function sees its body's lexical bindings.** `HoistFunction` compiled the closures of
   a body's top-level function declarations before the body's `let`/`const`/`class` slots existed,
   so `function t(){ const a = 1; function f(){ return a; } return f(); }` resolved `a` as a global
   and threw `ReferenceError`. The body's whole lexical set is now declared (in its dead zone, an
   empty slot) before any closure is built, the rule blocks already followed (JSC-138). Parameter
   defaults still do not see body bindings. Covers sloppy, strict, generator, arrow, async and
   Annex B block functions.
2. **Directive prologue.** `ParseDirectives` treated a string followed by any line break as a
   directive; it is now one only where the next token cannot continue the expression (a binary,
   assignment, member, call, template, `?`, `,`, `in` or `instanceof` token continues it).
   `(0,eval)('"a"\n+ 1')` answers `"a1"` instead of `1`.
3. **Tagged-template registry.** New wide-format opcode **`GetTemplateObject` (`0x5E`, `u8` chunk
   count, pops count cooked then count raw strings, pushes one object)**. The executor keys a
   per-engine registry by loaded program and instruction offset (a `ConditionalWeakTable` on the
   program), builds the cooked and raw Arrays itself, defines `raw` with every attribute off and
   freezes both through the realm's own integrity routine. The global-object cache property
   (`#template@<script>:<line>:<column>`) and the calls through the guest-replaceable
   `Object.freeze`/`Object.defineProperty` are gone; two separately evaluated programs no longer
   share a strings object. The compiler's now-unused `scripts` counter is removed. The opcode is
   added to `JsOpcodes.All`, the shape and stack-effect tables, a baseline handler slot (block
   step), and `NativeTemplateScanChecks` as class P; it is not in `RunsAlone`. The public API
   baseline gains the one enum member (`BROILER_API_WRITE=1`).
4. **Strict reserved words.** `BindingName` refuses `implements`, `interface`, `package`,
   `private`, `protected`, `public` and `static` in strict code (including strict eval code); a
   strict-mode shorthand binding pattern key, a class name, and - retroactively, when the body's
   directive makes them strict - a function's name and parameters refuse the whole strict-only set
   (those words plus `let`, `yield`, `eval`, `arguments`). After review, strict code also refuses
   those seven words and `let` as an identifier reference (`"use strict"; public = 1`), a shorthand
   property or its cover initialiser (`({ public } = o)`), and a label (`static: ;`).
5. **Named function expression name.** The name's record is marked; an assignment that resolves
   to it is ignored in sloppy code and throws `TypeError` (`ThrowImmutable`) in strict code, for
   plain, compound, update, destructuring and `for-in` targets. After review, the same holds for a
   store from direct `eval` code: the eval scope map writes the name with a new binding flag,
   **`JsFormat.EvalBindingFunctionName` (`2` in this slice, renumbered to `8` when merged with V15, which took `2` and `4`; admitted by the verifier only together with
   `EvalBindingImmutable`)**, and a write through the map is ignored when the eval code is sloppy
   and a `TypeError` when it is strict. This is the "immutable function-expression name" binding
   kind JSD-0026 section 4 already lists. The public API baseline gains the constant.
6. **`**` early error.** A `UnaryExpression` (`- + ! ~ typeof void delete`, and `await`) directly as
   the base of `**` is a `SyntaxError` (`-2 ** 2`); a parenthesised base is not.
7. **Symbol completion value.** The profile's completion projection renders a Symbol as
   `String(symbol)` does (`Symbol(x)`) instead of calling `ToString`, which threw.
8. **Module top-level destructuring.** `DeclareModuleBindings` read `declarator.Name`, empty for a
   pattern, so `const {a} = o; const {b} = o;` was refused as a duplicate of the empty name. It now
   declares and checks every name in the pattern; a real duplicate is still refused and named.
9. **Module evaluation errors.** `JsModuleInstance.EvaluationError` records what an evaluation
   threw. A throwing body (synchronous, an async body that rejected before its first `await`, or one
   that rejected later) marks itself and every later module of the walk that depends on it; later
   modules that do not depend on it return to the unordered state so a later import can evaluate
   them. A later walk reaching an errored module rejects (or, for the entry walk, throws) with the
   identical value and runs no body. Previously a second `import()` of a module whose body threw
   resolved to a namespace in its dead zone, and an async entry module that threw before its first
   `await` completed silently. Three more changes came out of review:
   - **An import made while the module is still evaluating waits for it.** The ordering walk enters
     a module another walk has ordered, is running, or is still awaiting as a wait entry
     (`~index`). A dynamic import that reaches one gives back its own ordered-but-unreached
     modules, waits on the module's evaluation promise (or on a promise the module settles when it
     leaves the ordered or running state), and then orders and walks the graph again. Previously a
     second `import()` of an async module that was still awaiting resolved at once: it was fulfilled
     where the first import rejected, or handed out a namespace still in its dead zone. The entry
     point's own walk keeps its behaviour.
   - **Earlier members of the thrower's cycle get its error.** The failure walk marks every module
     of the order that reaches the thrower through its requests, not only the later ones. In a
     depth-first order such an earlier module is in the thrower's strongly connected component.
   - **An async module body's evaluation promise is fulfilled with `undefined`.** It was resolved
     with the body's completion value, which adopted a thenable, so a module whose last statement's
     value was a promise waiting on the module itself never finished. The base did not deadlock
     because its second import did not wait. Test262's `eval-self-once-module.js` found this.

Fuel charging, cancellation, allocation accounting and guest exception handling are unchanged: the
template registry charges one unit per chunk and the integrity routine its own; the module failure
walk charges one unit per module and per request edge it passes, and a walk that waits charges
one unit per module again when it is ordered and walked again. A wait is a promise reaction, never a
CLR wait. No opcode other than `0x5E` was added, no diagnostic code was added, and no decision record
was written.

## Executed on Windows (win-x64, worktree based on 484f389 plus base-vm-w3.patch)

- Base first: `dotnet build Broiler.VM.slnx -c Release` then
  `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256.
- Regressions first, run on a separate build of the base tree (a `git archive` of the base tree):
  - [`the-front-end-repairs.js`](../../../src/tests/differential/the-front-end-repairs.js), 68 cases
    (61-68 added in review: stores to a function expression's name from direct `eval`, and strict
    reserved words as references and labels): on the base 44 answered wrongly, 6 of them in the
    added cases ([probe-before-front-end.txt](probe-before-front-end.txt)); after the change all 68
    match the retained answers and Node v24.17.0
    (`python eng/run-differential.py --only the-front-end-repairs --against node --timeout 20`, exit 0).
  - [`the-module-evaluation-errors.mjs`](../../../src/tests/differential/the-module-evaluation-errors.mjs),
    21 cases (14-21 added in review): the base refuses the probe at compile time (fix 8); a variant
    with one destructuring declaration shows the second-import cases resolving and case 9 failing.
    Run alone on the base, cases 14, 15 and 18 answer wrongly and 19-20 never print; case 21 is a
    guard the base answers, which this change deadlocked on before its completion fix
    ([probe-before-module.txt](probe-before-module.txt)). After the change all 21 match Node
    (`python eng/run-differential.py --only the-module-evaluation-errors --against node --timeout 20`, exit 0).
  - CLI acceptance row `runs/a-symbol-completion-value.js` (and the `runs` sweep count 89 -> 90):
    `python eng/run-cli-acceptance.py` fails 3 of 219 command lines on the base build and answers
    219 of 219 as declared after the change.
- After the change, the API write run and the assurance write run
  (`BROILER_ASSURANCE_WRITE=1 dotnet test ...`), a rebuild and the gate-mode run:
  Contract 267/267, Architecture 256/256.
- Full retained lane `python eng/run-differential.py --timeout 30`: exit 1 only for the known
  code-page case 36 of `the-later-library-methods`, which the base build answers identically.
- Test262 (pinned ccaac100, bytecode form, `--shards 1 --jobs 1`), per-variant lists in
  [test262-comparison.txt](test262-comparison.txt):
  - The card's selection (`statements/function`, `function-code`, `block-scope`,
    `annexB/language/function-code`, `directive-prologue`, `expressions/tagged-template`,
    `expressions/template-literal`, `expressions/exponentiation`, `module-code`,
    `expressions/dynamic-import`), 4094 variants: the pass count moves from 3364 to 3451; 87
    variants move from failing to passing and none moves the other way. Six of them come from the
    review fixes (`dynamic-import-of-waiting-module.js` in two variants, `fulfillment-order.js`,
    `rejection-order.js` and `unobservable-global-async-evaluation-count-reset.js` under
    `module-code/top-level-await`, and `directive-prologue/10.1.1-30-s.js`). 552 variants are
    skipped by the harness's feature rules in both runs, 20 are unsupported.
  - A guard selection (`built-ins/Array/fromAsync`, `future-reserved-words`, `eval-code/indirect`,
    `statements/class/definition`, `expressions/function`, `statements/let`, `statements/const`),
    1561 variants: the pass count moves from 1449 to 1489; the 10 `Array.fromAsync` variants the
    hoisting defect blocked now pass, as do 14 future-reserved-word variants and the function-
    expression name variants, including `named-no-strict-reassign-fn-name-in-body-in-eval.js` and
    `named-strict-error-reassign-fn-name-in-body-in-eval.js`; none moves the other way.
  - A review selection (`eval-code/direct`, `expressions/generators`, `expressions/async-function`,
    `expressions/async-generator`, `statements/async-function`, `reserved-words`, `identifiers`,
    `statements/labeled`, `expressions/object`, `expressions/assignment`), 6115 variants against the
    base build: the pass count moves from 5625 to 5674. This includes the six
    `*-reassign-fn-name-in-body-in-eval.js` variants of the generator, async-function and
    async-generator expressions. None moves the other way.

## Not exercised or not addressed

- **The native form was not exercised**: artifacts do not instantiate on this machine
  (UnsatisfiedHostAssumption, pre-existing). The `GetTemplateObject` baseline handler steps into the
  interpreter switch by construction, which has not been observed here.
- The skipped Test262 variants in the selection were not run by other means.
- A tagged template whose cooked chunk would be `undefined` (an invalid escape) is still handled by
  the parser as before; the new opcode supports the value but the front end never emits it.
- An async entry module that rejects after its first `await` is still not reported by the
  end-user host (pre-existing); a dynamic import of it rejects correctly.
- The cycle marking is judged over the failing walk's own order: a path to the thrower that
  passes through a module evaluated by an earlier walk, or through another walk's module, is not
  followed.
- The walk still evaluates a graph in one depth-first order and suspends at an awaiting module,
  so a sibling that does not depend on that module waits for it too (pre-existing;
  `async-module-does-not-block-sibling-modules.js` still fails). A module whose only `await` is
  inside a nested async arrow is still compiled as an async body (pre-existing, found in review:
  the parser does not count an arrow body as a function level when it notes a top-level `await`).
- Remaining failures in the selection are unrelated to these fixes (dynamic-import and
  top-level-await host cases, unpaired-surrogate export names, template-literal and tagged-template
  invalid-escape cases, and others listed as still failing in the report).
- No malformed-corpus entry was added for `0x5E`, or for the eval-scope binding flags `2`
  (refused) and `3` (admitted).
