# JSP-4 operand coercion order: JSeal slice V01

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It refines JSP-4 and does not mark that stage complete.

## What changed

`JsEngine.Binary` (shared by `-`, `*`, `/`, `%` and `**`) popped and converted the right operand
before the left one. Both operands now leave the stack before either is converted, and the left one
is converted first. A throw from the left conversion therefore leaves the right operand unconverted.
The arithmetic still receives the operands in source order. The audit found the same reversal in
the interpreter cases for `&`, `|`, `^`, `<<`, `>>` and `>>>`, and in `Relational` for `>` and
`>=`. The old comment there said those two convert the right operand first, which misreads
IsLessThan's LeftFirst = false. All four relational operators now convert left first; `<`/`<=`
were already correct. Compound assignments go through the same opcodes and are covered by this
change. `+` (`Add`) and the unary operators were already correct.

The baseline native form has no arithmetic of its own for these opcodes: each `JsBaselineHandlers`
entry point is a `JsNativeActivation.Step` back into the same `ExecuteCore` switch, so the fix
applies there as well. The slice executor (`JavaScriptExecutor`, `--slice`) handles primitives
only. Its conversions cannot run guest code, and it already converts left first. It is unchanged.
Fuel charging, cancellation, allocation accounting and exception propagation are untouched: only
the order of two existing conversion calls moved.

## Executed on Windows (win-x64, worktree based on 484f389)

- Regression first: [`the-numeric-operand-order.js`](../../../src/tests/differential/the-numeric-operand-order.js),
  44 cases. Before the fix, 41 differed from Node (and from the specification): `ba` orders, `ba` logs
  after a left-side throw, and reversed Symbol.toPrimitive/toString logs. After it, all 44 match.
  [windows-node.json](windows-node.json) is the comparison report
  (`python eng/run-differential.py --only the-numeric-operand-order --against node --timeout 20`,
  Node v24.17.0, exit 0). Node is the diagnostic; the answers were checked against the
  ApplyStringOrNumericBinaryOperator and IsLessThan steps.
- `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267 and Architecture 256/256,
  after the change and the assurance write run (the pre-change baseline of 267/256 is the
  repository's stated baseline and was not re-run here).
- Full retained lane `python eng/run-differential.py --timeout 10`: exit 1. The failures are the
  known `the-later-library-methods` case 36 (cube-root spelling), plus non-ASCII output in
  `the-later-library-methods` case 147 and `the-json-date-and-regexp-surface` cases 158-159,
  mangled by this console's code page 850. The unmodified main-tree build gives the same output
  for those cases. Every other probe, including the new one, agrees with its retained answers.
- Test262 (pinned ccaac100), 17 subtrees under `test/language/expressions/`: the card's twelve
  plus less-than, less-than-or-equal, greater-than, greater-than-or-equal and addition. Per-variant
  results are in [test262-comparison.txt](test262-comparison.txt).
  - bytecode: pass 1731 / fail 154 / unsupported 198 before; pass 1779 / fail 106 / unsupported
    198 after. 48 variants moved Failed to Passed. None moved the other way.
  - native (x86-64-win64): pass 45 / fail 1840 / unsupported 198 before **and** after. There were
    no changes. 1838 of the failures are "the artifact would not instantiate" (UnsatisfiedHostAssumption):
    this machine will not arm the native code page, and the CLI's `--native x86-64-win64` fails the
    same way on a one-line script. **The native form was therefore not exercised.** Its handlers
    reach the corrected switch by construction, but that has not been observed on this machine.

## Not addressed (separate defects seen in the same subsets)

- `compound-assignment/S11.13.2_A7.*_T4` (22 variants): `base[prop] op= v` converts `prop` with
  ToPropertyKey twice, once for the read and once for the write. That is the reference's key
  conversion, not operand coercion.
- `exponentiation/applying-the-exp-operator_A7/A8`: `-1 ** ±Infinity` gives 1 instead of NaN,
  because it inherits `System.Math.Pow`.
- The remaining compound-assignment failures are direct eval, `with`-scope and EvalError admission
  cases; the remaining exponentiation and division failures are early-error/ASI parser cases.
