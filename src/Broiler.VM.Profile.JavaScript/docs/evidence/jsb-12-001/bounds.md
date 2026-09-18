<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The bounds bundle JSB-12-001 is judged by, fixed before any per-block code

**Written:** 2026-09-17, before any per-block code existed. **Stage:** JSB-12, which restates JSB-11's
exit gate against per-block baseline steps and is added only if bundle JSB-11-002's rule adopts them.
**Values:** fixed by the repository owner on 2026-09-17. **Owner:** profile architecture owner.
**Reviewer:** none.

**What this file is.** JSB-11's gate says that every tolerance or bound a clause names is predeclared
in the stage's bundle before the run it judges. JSB-11-001 predeclared none. This file is where JSB-12's
are predeclared, together with the route table the depth clause is judged over, the classes the
conformance comparisons are read against and the patch that lifts the depth bounds. The owner fixed the
values. They are not a restatement of JSB-11's, because JSB-11 has none.

**It holds no measured figure.** Every number here is a bound, a constant already in code, an allowance
passed to a script, a count, or an identifier. Bundle JSB-12-001's README, written when the stage's
evidence is collected, is the only place a figure the bounds are judged against goes.

**It may only tighten.** After this file's commit it may gain a route, a shape or a stricter bound, as a
dated addition that says which candidate figures had been seen when it was added. No bound, tolerance,
class or shape is ever removed or relaxed: an envelope widened after seeing a candidate is the failure
[roadmap.gates.md section 21](../../roadmap.gates.md#21-test-and-evidence-matrix) names for a
measurement.

**The arms.** "The control" is the last commit before the first per-block commit, whose baseline form
makes one call per instruction. "The candidate" is the commit the stage is collected at. Both carry the
core meter's fuel pre-admission (route MVP-9).

---

## 1. Disclosure: what had been seen when these values were set

- **Bundle VM-5-002's E10 and E11.** E10 timed four JavaScript shapes in both forms, and E11 timed a
  bytecode Octane subset. Both timed the control-side form, one call per instruction, on the core
  meter with fuel pre-admission, and both had been seen when these values were set. No figure from them
  is used here.
- **Bundle JSB-11-001's retained reports** of the per-instruction form: its Octane reports, its
  released-bounds frame-cost logs, its JIT summary lines and its Native AOT image sizes. They were in
  the tree when these values were set. No figure from them is used here.
- **Working-tree figures of the prototype commits** `525ea13`, `17747dc`, `47fbba5` and `9ec81de`. No
  figure from them is used, and none is recorded.
- **The depth-ten check of every shape family** ran on the per-instruction build before this file was
  written, and it read only that each family answers in both forms.
- **The lifted-bounds patch** was applied and built once, outside the repository, on a tree whose two
  patched files are the ones this commit's parent holds, to check that it compiles. Nothing ran on that
  build. Before this commit it was checked with `git apply --check` against this commit's parent.
- **No per-block code existed**, so no candidate figure of any kind had been observed.

---

## 2. The interpreter's instantiations (JIT)

**Read from** `DOTNET_JitDisasmSummary` and `DOTNET_JitDisasm` transcripts of each arm's JIT Release
hosts: the conformance host, after tier-up, which is the binary the gate names for the per-opcode
instantiations, and the command-line host. Each host of one arm runs the same inputs as the same host
of the other arm, in both forms. The stack reservation of an instantiation is its prologue's `sub rsp`
operand. A summary line labelled `Tier1` or `FullOpts` (which includes `Tier-0 switched to FullOpts`)
is optimised code, and `MinOpts` is not. Each bound compares one host with the same host of the other
arm, and it holds only if it holds in every transcript that shows what it compares. **An instantiation
that appears in no transcript of both arms is not shown, and a clause that is not shown is not met.**

| # | Bound | Holds when |
|---|---|---|
| 2.1 | `JsEngine.ExecuteCore[JsInterpreted]` against the control | in the control and in the candidate it reaches optimised code; its stack reservation is **equal** to the control's; its code size is **at most 2% larger** than the control's |
| 2.2 | `JsEngine.ExecuteCore[JsStepBlock]` against `ExecuteCore[JsInterpreted]` | in the candidate it reaches optimised code; its stack reservation is **at most** the candidate's `ExecuteCore[JsInterpreted]` reservation |
| 2.3 | each per-opcode instantiation of a run-alone opcode (the call class and the opcodes whose arms enter guest code on their common path, nineteen in all) | its code size in the candidate is **at most 2% larger** than the same instantiation's in the control, and it has no jump table |

**If 2.3 fails**, the fallback JSB-11's gate names is taken: `JsBaselineHandlers.PerOpcodeSteps` is set
to false, so every run-alone wrapper runs the block step, the bundle records the failure and the switch,
and the flip rule of section 8.3 applies before any depth clause is judged.

---

## 3. The bytecode frame cost

**Bound.** In the bytecode form, shape family `plain`, with the released bounds, the candidate's deepest
returning recursion and deepest throwing recursion are **equal** to the control's.

**Read from** `frame-cost-plain-bytecode.log` (the candidate) and `frame-cost-control-plain-bytecode.log`
(the control), each from `eng/measure-frame-cost.py --form bytecode --shape plain` with `--source-tree`
naming the tree the binary was built from.

---

## 4. The bytecode benchmark

**Bound.** The candidate's median Octane geometric mean, in bytecode form, is **at least 0.97 times**
the control's median Octane geometric mean.

**Procedure.** The Octane pin `eng/run-octane.py` reads (`src/tests/octane/pins/octane.pin`) and the
selection JSB-11-001's reports record, `richards`, `crypto`, `raytrace`, `splay`, `deltablue` and
`navier-stokes`, run by `eng/run-octane.py` in bytecode form from the control's and the candidate's JIT
Release command-line hosts, alternating the two arms on one machine, five reports each, named
`octane-control-<i>.report` and `octane-candidate-<i>.report`.

**The aggregation, as the owner ruled on 2026-09-17.** Each report's geometric mean is its own
`aggregate.score`, the geometric mean the script computes over the ratios of the selection. An arm's
figure is the **median over its five reports** of those geometric means, and the bound compares the
candidate's median with 0.97 times the control's median. Every figure is read off the reports only.

**A report that gives no geometric mean of the selection**, because `aggregate.score` is empty or
because its `aggregate.coverage` names a selected benchmark that did not exit zero, leaves its arm
without five geometric means, and then the bound is not shown.

---

## 5. The fresh process

**Bound.** For `src/tests/forms/wide/fresh-small.js`, the candidate's native run-minus-check is **at
most** the control's native run-minus-check.

**Quantity.** `Q(fresh-small.js, arm, native)`, the median over the retained repetitions of
`run_ms - check_ms`, as `eng/measure-baseline-granularity.py --fresh` computes it under JSB-11-002's
committed manifest and rule, with its output written to a directory of this bundle's own so that
nothing of JSB-11-002 is overwritten. The fresh-process mode computes no verdict of JSB-11-002's rule;
this bound is judged on its `summary.txt` alone.

---

## 6. Native AOT images

**Bound, per composition root.** For each of the four composition roots `JavaScript.Cli`,
`JavaScript.Conformance`, `JavaScript.ExecutionOnly` and `JavaScript.SliceCompiler`, the candidate's
Native AOT image for `win-x64`, published with warnings as errors, is **no larger in bytes** than the
control's image of the same root, published the same way. Each root is judged on its own, and the
bound is met only when it holds for all four.

---

## 7. The deep throw

**Bound.** The native form's time for the probe "a throw from depth 5000 caught at the top" of
`TheTwoFormsAgreeOverTheWideManifest` is **at most 2,000 ms**, which is the checks' own
`DeepThrowMilliseconds`. The row fails at that figure or above, so the bound holds in every retained
checks transcript in which the row passed, and it is judged against those transcripts.

---

## 8. The depth clause, in call-depth units

### 8.1 Terms

- **`K(s, f, r)`** is `deepest-returning-recursion` from a log of `eng/measure-frame-cost.py` run on a
  build carrying [`lifted-bounds.patch`](lifted-bounds.patch), for shape family `s` of the route table
  (section 9), form `f` and runtime identifier `r`, with `--ceiling 100000` and the declared guest stack
  of 96 MB.
  - A lifted log that ends `stopped-by-returning completed` completed a recursion as deep as the
    ceiling, so `K` is at least the ceiling, and the clause reads it as `K = 100000`.
  - Otherwise a lifted log is expected to end `stopped-by-returning backstop` and
    `stopped-by-throwing backstop`, to print `bytes-per-frame`, and to exit 0.
  - The ceiling the script grants is counted in `CallDepth` units, and the script reads a refusal at a
    depth below it as the stack. So for a family whose `u` is above one, a lifted log can end
    `backstop` at the granted ceiling, near `100000 / u` levels, and `K` is then a floor of the
    capacity rather than the capacity. Condition 1 below may read that floor, because a floor that
    meets the condition means the capacity does.
- **`u(s)`** is the number of `CallDepth` units one recursion level of `s` spends.
  - `K` counts levels. `JsEngine.MaximumCallDepth` (6,000) and the profile's grantable `CallDepth`
    maximum (8,192) count units. `JsEngine.Call`, `Construct`, `ResumeGenerator`, `ResumeAsync` and
    `ResumeAsyncGenerator` each spend one.
  - **The measured value** is `6000 / deepest-returning-recursion` from `s`'s released-bounds bytecode
    log on `win-x64`, rounded to the nearest integer, and the unrounded ratio is retained in
    `depth-units.txt`. **It is the value the clause uses, on both runtime identifiers.**
  - **The read value** is the route table's `u` column, which is what the code says. A measured value
    that differs from it, or a `linux-x64` released-bounds bytecode log whose rounded ratio differs
    from the `win-x64` one, is a finding the bundle records and explains before the clause is judged.
- **Which logs count.** Only a log whose header shows `per-opcode-steps` at the value the candidate
  ships counts toward any condition below. A log whose header prints `unknown` counts toward none.

### 8.2 The clause

**The clause is met on a runtime identifier if and only if, for every shape family `s` in the route
table (section 9):**
1. `K(s, native, r) × u(s) ≥ 2 × 8,192`: the stack holds twice the grantable maximum, counted in the
   units that maximum counts. That is the ordering JSC-139 restored.
2. The deepest returning and deepest throwing recursions of each lifted log agree within the script's
   own tolerance, which is what its exit code 0 says.
3. In both forms, the released-bounds logs show `stopped-by-returning bounded` and
   `stopped-by-throwing bounded`, with equal depths in the two forms, no `backstop` and no `died`. So
   in a released build the stack never reaches the probe before `RangeError`.

**On `win-x64`** the logs are taken on a workstation: `frame-cost-<shape>-<form>.log` with the released
bounds and `frame-cost-lifted-<shape>-<form>.log` with the patch, one per family and form.

**On `linux-x64`** the logs are taken by the `workflow_dispatch` workflow
`frame-cost-javascript-profile.yml`, which keeps its `lifted`, `released` and `both` options. The lifted
half applies this directory's patch and the released half builds the ref as committed, one job per
bounds, family and form, and the logs are `frame-cost-linux-x64-lifted-<shape>-<form>.log` and
`frame-cost-linux-x64-released-<shape>-<form>.log`. **All three conditions apply to this half**, so it
needs both bounds: one dispatch with `both`, or one with `lifted` and one with `released` at the same
commit. `u(s)` is the value measured on `win-x64`. **Each dispatch needs the owner's approval at the
time.** If no dispatch is approved when the stage is collected, the bundle names the `linux-x64` half of
the clause unmet. The clause is met only when both halves are.

### 8.3 What the clause does not allow

- **No relative branch.** A native capacity is never judged against the bytecode form's capacity.
- **A bytecode shortfall is its own correction.** If `K(s, bytecode, r) × u(s) < 2 × 8,192`, that is a
  shortfall of the bytecode form, filed as JSC-139 was. The native form must still meet the floor or
  take the ladder below.
- **The flip rule.** A flip of `JsBaselineHandlers.PerOpcodeSteps`, after any depth log exists, re-runs
  every lifted and every released-bounds depth log, on both runtime identifiers, on the flipped build.
  The clause is judged only on logs that show the value the candidate ships.

### 8.4 If the clause fails in the native form

Take these in order, stop at the first that makes the clause pass, and record which one was taken.
1. **Move into the run-alone set every opcode whose arm reaches the failing family's helper**, found by
   walking the call graph from every arm of `JsEngine.ExecuteCore` to that helper and not by taking the
   family's own opcode. Then re-run every family, not only the failing one. This changes emitted bytes:
   before the per-block change is merged the golden bytes are re-based in the same commit, and after it
   is merged it is a new backend version. If JSB-11-002 has already been collected, the candidate it
   measured is no longer the one shipped, and the measurement is re-collected as `jsb-11-003` under a
   new predeclared rule.
2. **Raise `JsExecution.GuestStackBytes`**, and re-measure both forms and every family. That is a
   correction in JSC-139's pattern.
3. **Never lower the grantable `CallDepth` maximum.** It is policy, not a machine figure.

### 8.5 The lifted-bounds patch

[`lifted-bounds.patch`](lifted-bounds.patch) raises `JsEngine.MaximumCallDepth` and the profile's
grantable `CallDepth` maximum in `JavaScriptProfile` to 1,000,000 each, and changes nothing else. It
raises no other maximum, so the nested-load depth stays as the profile declares it. It is committed
with this file so that one file is applied by the development runs on a workstation, by the `linux-x64`
workflow and by the bundle. On a workstation it is applied with `git apply` to a worktree outside the
repository, which is built there and removed afterwards. The bundle hashes it and does not re-create
it.

### 8.6 Addition of 2026-09-17: ladder step 2 is taken, and step 1 is skipped by owner ruling

*Added 2026-09-17, after the per-block baseline steps were committed and after the depth runs below
had been seen.* **This addition relaxes no bound.** It records that a remedy section 8.4 predeclared
has been taken, and that the owner ruled the step before it out; it removes no family, shape, class or
condition, it does not touch section 8.2's three conditions or their thresholds, and the route table
still names 36 families. It states no figure, as this file states none.

**Step 2 is taken.** `JsExecution.GuestStackBytes` is raised from 96 MB to **208 MB**, which is item 2
of the ladder in section 8.4, and `eng/measure-frame-cost.py`'s `DEFAULT_STACK_BYTES` is moved with it
in the same commit because that script states the figure rather than reading it. The raise is a
correction of its own, committed **outside the contiguous per-block range and after it**, so that
neither arm of measurement bundle `jsb-11-002` carries it; JSB-12's candidate, a later commit, does.
Profile correction JSC-226 records the change, and the remarks on the constant, on
`JsEngine.MaximumCallDepth` and on `JavaScriptProfile`'s `CallDepth` maximum carry a sentence each.

**What that means for the terms of section 8.1.** Where 8.1 says `K` is read from a lifted log taken
"with the declared guest stack of 96 MB", it means the guest stack the measured build declares. From
the commit that takes step 2 that is 208 MB, and every lifted and released log the clause is judged on
is re-taken on a build carrying it — on both runtime identifiers, in both forms, for all 36 families —
exactly as 8.4 item 2 says ("re-measure both forms and every family"). Logs taken on the 96 MB stack
count toward no condition, for the same reason the flip rule in 8.3 discards logs taken at the other
value of `PerOpcodeSteps`. Nothing else in 8.1, 8.2 or 8.3 changes: the threshold is still
`2 × 8,192`, `u(s)` is still the value measured on `win-x64`, and condition 3 still asks for bounded
stops and equal depths across the forms at the released bounds.

**Step 1 is skipped, by the repository owner's ruling of 2026-09-17, and this is the record of it.**
Section 8.4 says to take its items in order. The owner ruled that item 1 — moving into the run-alone
set every opcode whose arm reaches a failing family's helper — is **not** taken, on this evidence: the
control arm already runs those opcodes one at a time, through the same kind of per-opcode step, and it
still fails several of the same routes, so step 1 cannot make the clause pass by itself; and it would
put a large share of block-eligible instructions back onto steps of their own, which risks the Native
AOT image bound of section 6 and undoes most of what the stage measures. **This departs from 8.4's
"in order" wording and from nothing else.** No bound, tolerance, class or condition is changed by it,
item 3 of 8.4 (never lower the grantable `CallDepth` maximum) stands, and the stage's README states the
ruling with its evidence when the stage is collected.

**The `linux-x64` half.** It stays as 8.2 leaves it: unmet unless a dispatch of
`frame-cost-javascript-profile.yml` runs at the commit that carries the raise, with both bounds, and
**each dispatch needs the owner's approval at the time**. Nothing here claims that half.

**What had been seen when this addition was written.** Section 1 says what had been seen when the
values were set, and section 9's earlier addition says what had been seen when that one was written.
When this one was written, the candidate figures seen were: the lifted-bounds depth logs of every
route-table family in both forms at the last per-block commit, and the matching lifted-bounds logs of
the control arm, all of which print depths and bytes per frame; a lifted-bounds re-probe of the
bytecode `yield*` returning shape at the frame-fix commit after it; the released-bounds logs of every
family in both forms on both arms; the JIT summary lines, code sizes and stack reservations of the
interpreter, the block step and the run-alone instantiations on both arms; the Native AOT image sizes
of the four composition roots; and the checks transcripts of the per-block commits, which print a time
for every row. **No figure from any of them is used here**, and the arithmetic that chose 208 MB over a
smaller value is the diagnosis's, not this file's.

---

## 9. The route table

**What it is.** Every way one JavaScript level can nest under an instruction of `JsEngine.ExecuteCore`,
from an audit of every arm and the helpers it calls, before any per-block code. It names each route's
helper chain and opcodes by member, the shape family of `eng/measure-frame-cost.py` that measures it,
the arm that family recurses through, and `u`, the `CallDepth` units one level spends as read from the
code. **The depth clause (section 8) is judged over exactly the families this table names: 36
families, so 72 logs per bounds per runtime identifier.** A route with no family says why.

**Where several arms reach one route**, the family recurses through the arm whose helpers leave the
most native frames open between the arm and the guest call. That was decided by reading the helper
chain, not by measuring, and each row names the arm and the others that reach the route. Arms marked
"hand-assembled only" reach the route only from bytecode `JsCompiler` does not emit.

**What one level nests.** In bytecode, every route is `ExecuteCore[JsInterpreted]` → the arm's helpers
→ `JsEngine.Call` (or a construct or a resume) → `Invoke` → `Execute` → `ExecuteCore[JsInterpreted]`.
In the native form, a block route is the emitted unit's frame → the `JsBaselineHandlers` wrapper →
`JsNativeActivation.Step<JsStepBlock>` → `ExecuteCore[JsStepBlock]` → the arm's helpers →
`JsEngine.Call` → `Invoke` → `Execute` → `RunNative` → the callee's emitted unit. A run-alone route is
the same with the opcode's own per-opcode step while `PerOpcodeSteps` is true, and with the block step
when it is false.

### 9.1 Block routes: an instruction inside a block re-enters guest code

| Route | Family | The arm it recurses through, and the chain | Other arms that reach the route | u (read) |
|---|---|---|---|---|
| accessor read | `getter` | `GetProperty` → `JsEngine.GetProperty` → `Lookup` → `Call` of the getter | `GetIndex`, `LoadGlobal`, `LoadGlobalOrUndefined`, `LoadSuperProperty`, `SpreadObject`, `InstanceOf` (an inherited `prototype` or `Symbol.hasInstance` getter), `ResolveName` (`Symbol.unscopables`), `NewClass` (the heritage's `prototype`), `Throw` (through `Render`), `IterateAwaitStep` (no chain; section 9.3), and the coercion opcodes (a getter-valued `valueOf`, `toString` or `Symbol.toPrimitive`) | 1 |
| accessor read, global | `global` | `LoadGlobal` → `GetProperty` on the global object → `Lookup` → `Call` of the getter | `LoadGlobalOrUndefined` | 1 |
| accessor read, indexed | `index` | `GetIndex` → `GetIndexed` → `GetProperty` → `Lookup` → `Call` of the getter | — | 1 |
| accessor read, `super` | `super` | `LoadSuperProperty` → `Lookup` from the home object's prototype → `Call` of the getter; the level also crosses `GetProperty` | — | 2 |
| accessor write | `setter` | `SetProperty` → `JsEngine.SetProperty` → `Call` of the setter | `SetIndex` (through `SetIndexed`), `StoreGlobal` | 1 |
| `super` write | `superset` | `StoreSuperProperty` → `SetSuper` → `Call` of the setter found above the home object; the level also crosses `SetProperty` | — | 2 |
| coercion, `valueOf` | `valueof` | `ToNumber` → `JsEngine.ToNumber` → `ToPrimitive` → `OrdinaryToPrimitive` → `GetProperty` and `Call` of `valueOf` | `Add`, `Subtract`, `Multiply`, `Divide`, `Remainder`, `Exponent`, `Negate`, `BitwiseNot`, `BitwiseOr`, `BitwiseAnd`, `BitwiseXor`, `ShiftLeft`, `ShiftRight`, `ShiftRightUnsigned`, `LessThan`, `LessThanOrEqual`, `GreaterThan`, `GreaterThanOrEqual`, `LooseEquals`, `LooseNotEquals`; through `ToPropertyKey`: `GetIndex`, `SetIndex`, `DefineIndexed`, `DefineMethod`, `DeleteIndex`, `In`, `LoadSuperProperty`, `StoreSuperProperty`; through `ToNumber`: `SetProperty` and `SetIndex` on an Array `length` or a typed-array element; through `ToStringValue`: `Throw` (`Render`) | 1 |
| coercion, `toString` | `tostring` | `Add` → `ToPrimitive` → `OrdinaryToPrimitive` → `Call` of `toString` | as `valueof` | 1 |
| coercion, `Symbol.toPrimitive` | `toprimitive` | `ToNumber` → `ToPrimitive` → `TryGetSymbolMethod` → `Call` of `Symbol.toPrimitive` | as `valueof` | 1 |
| Proxy `get` | `proxy` | `GetProperty` → `Lookup` → `JsProxy.ProxyGet` → `Call` of the trap | `GetIndex`, `LoadGlobal`, `LoadGlobalOrUndefined` (a Proxy in the global object's chain), `LoadSuperProperty`, `SpreadObject`, `InstanceOf`, `ResolveName`, `NewClass`, `Throw`, `IterateAwaitStep`, the coercion opcodes on a Proxy operand | 1 |
| Proxy `has` | `in` | `In` → `HasProperty` → `JsProxy.ProxyHas` → `Call` of the trap | `LoadGlobal`, `LoadGlobalOrUndefined`, `StoreGlobal` (strict code; a Proxy in the global object's chain) | 1 |
| Proxy `has`, `with` | `with` | `ResolveName` → `HasProperty` → `JsProxy.ProxyHas` → `Call` of the trap; `PushObjectScope` only wraps the object and asks nothing | — | 1 |
| Proxy `ownKeys` | `forinproxy` | `ForInStart` → `JsRealm.CreateEnumerator` → `OwnPropertyNames` → `JsProxy.ProxyOwnKeys` → `Call` of the trap | `SpreadObject` (through `CopyDataProperties`) | 1 |
| Proxy `set` | `proxyset` | `SetIndex` → `SetIndexed` → `SetProperty` → `JsProxy.ProxySet` → `Call` of the trap | `SetProperty` and `StoreGlobal`, one frame fewer | 1 |
| Proxy `deleteProperty` | `proxydelete` | `DeleteProperty` → `JsProxy.DeleteOwnProperty` → `ProxyDelete` → `Call` of the trap | `DeleteIndex`, which ties: its `ToPropertyKey` returns before the override is called, so the named form stands for both | 1 |
| Proxy `getOwnPropertyDescriptor` | `proxydescriptor` | `SetIndex` → `SetIndexed` → `SetProperty` → `JsProxy.ProxySet` → `SetWithReceiver` → `LandOnReceiver` → `JsProxy.TryGetOwnProperty` → `ProxyGetOwnProperty` → `Call` of the trap | `SetProperty`, one frame fewer; `ForInStart` (`JsRealm.CreateEnumerator`), `SpreadObject` (`CopyDataProperties`) and `StoreSuperProperty` (`SetSuper`), four fewer; hand-assembled only: `DefineGetter`, `DefineSetter` and `DefineMethod` on a Proxy host | 1 |
| Proxy `defineProperty` | `proxydefine` | the same write, which `LandOnReceiver` ends in `JsProxy.SetOwnProperty` → `DefineOrThrow` → `ProxyDefineOwnProperty` → `Call` of the trap | `SetProperty`, one frame fewer; `StoreSuperProperty` (`SetSuper`), four fewer; hand-assembled only: `DefineField`, `DefineIndexed`, `DefineGetter`, `DefineSetter`, `DefineMethod` and `SpreadObject` on a Proxy host | 1 |
| Proxy `getPrototypeOf` | `proxyproto` | `InstanceOf` → `JsEngine.InstanceOf` → `JsProxy.Prototype` → `ProxyGetPrototypeOf` → `Call` of the trap | `ForInStart` (`JsRealm.CreateEnumerator`) and `StoreSuperProperty` (`SetSuper`), which tie at the same frames; `instanceof` stands for the three | 1 |
| Proxy `getPrototypeOf`, through the native `__proto__` getter | `proxyprotoget` | `ResolveName` asking a `with` object's `Symbol.unscopables`, which is the Proxy, whether it hides `__proto__`: `ResolveName` → `Unscopable` → `GetProperty` → `Lookup` → `JsProxy.ProxyGet`, which has no `get` trap to call → `GetWithReceiver` → `Lookup` → `Call` of the `Object.prototype.__proto__` getter → `JsProxy.Prototype` → `ProxyGetPrototypeOf` → `Call` of the trap | `GetIndex` (through `GetIndexed`), one frame fewer; `GetProperty`, two fewer; `LoadSuperProperty` (`Lookup`), three fewer; `SpreadObject` (`CopyDataProperties`) ties `GetIndex`, but reads the key only when the Proxy's `ownKeys` and `getOwnPropertyDescriptor` traps report a `__proto__` its target does not hold, so its level enters those traps as well. From the getter's call to the trap's, every arm takes the same frames | 2 |
| Proxy `isExtensible` | `proxyextensible` | the same write as `proxydescriptor`, which `LandOnReceiver` sends through `JsProxy.Extensible` → `ProxyIsExtensible` → `Call` of the trap before it defines; the key is new at every level, because `LandOnReceiver` asks only for a key the receiver does not hold | `SetProperty`, one frame fewer; `StoreSuperProperty` (`SetSuper`), four fewer | 1 |
| Proxy `setPrototypeOf`, through the native `__proto__` setter | `protoset` | `SetIndex` → `SetIndexed` → `SetProperty` → `JsProxy.ProxySet` → `SetWithReceiver` → `Call` of the `Object.prototype.__proto__` setter → `ObjectSetPrototype` → `ProxySetPrototypeOf` → `Call` of the trap | `SetProperty`, one frame fewer; hand-assembled only: `SetPrototypeLiteral` on a Proxy host, which reaches `ProxySetPrototypeOf` directly | 2 |
| private getter | `privateget` | `LoadPrivate` → `ReadPrivate` → `Call` of the getter | — | 1 |
| private setter | `privateset` | `StorePrivate` → `WritePrivate` → `Call` of the setter | — | 1 |
| `instanceof` hook | `hasinstance` | `InstanceOf` → `JsEngine.InstanceOf` → `TryGetSymbolMethod` → `Call` of `Symbol.hasInstance` | — | 1 |
| object spread | `spread` | `SpreadObject` → `CopyDataProperties` → `GetProperty` → `Call` of the getter | — | 1 |
| thrown-object rendering | `render` | `Throw` → `Render` → `GetProperty` of `message` → `Call` of the getter | — | 1 |

Of the realm's own accessors, only `Object.prototype.__proto__` reaches further guest code: its getter
is the `proxyprotoget` row and its setter the `protoset` row. An accessor an embedder installs with
`JsHostRealm.DefineAccessor` puts embedder code in the same place, which is the host-exotic row of
section 9.3; the measured host installs none.

### 9.2 Run-alone routes: the opcode keeps a step of its own

| Route | Family | The arm it recurses through, and the chain | Other arms that reach the route | u (read) |
|---|---|---|---|---|
| plain call | `plain` | `Call` → `JsEngine.Call` → `Invoke` → `Execute` | `Construct`, `CallSpread`, `ConstructSpread`, and `CallEval` on a callee that is not the eval intrinsic | 1 |
| field initialiser under `super()` | `superfield` | `SuperCallForwarded` (the default derived constructor) → `SuperConstruct` → `Construct` of the base, then `InitialiseInstanceElements` → `ApplyClassElements` → `Call` of the initialiser; the level also crosses `Construct` | `SuperCall`, `SuperCallSpread` | 3 |
| call through a built-in | `callback` | `Call` → `JsEngine.Call` of `Array.prototype.map` → the built-in's `Call` of the guest | the call class, through any built-in that calls back | 2 |
| call through a bound function | `bound` | `Call` → `JsEngine.Call` meets a `JsBoundFunction` → `Call` of its target | the call class | 2 |
| call through a callable Proxy | `proxyapply` | `Call` → `JsEngine.Call` → `JsProxy.ProxyCall` → `Call` of the `apply` trap | the call class; `ProxyConstruct` for `Construct` | 2 |
| iterator next | `forof` | `IterateNext` → `TryIterateNext` → `Call` of the record's `next` → `JsEngine.ResumeGenerator` | `SpreadArray` (through `IterateInto`), `IterateRest` (through `DrainIterator`) | 2 |
| synchronous delegation | `delegate` | `YieldDelegate` → `Delegate` → `TryIterateNext` → `Call` of `next` → `JsEngine.ResumeGenerator` | — | 2 |
| iterator open | `iterable` | `IterateStart` → `GetIterator` → `Call` of a guest `Symbol.iterator` method | `SpreadArray`, `YieldDelegate` | 1 |
| iterator close | `iterclose` | `IterateClose` → `CloseIterator` → `Call` of a guest `return` | — | 1 |
| static element | `staticblock` | `RunStaticElements` → `ApplyClassElements` → `Call` of a static block | a static field's initialiser, called from the same place | 2 |

### 9.3 Routes with no shape family, and why

**The depth clause is judged over the families of sections 9.1 and 9.2 only, so it claims nothing for
a route in this table.**

| Route | Opcodes | Why no family |
|---|---|---|
| asynchronous iteration: `for await` | `IterateStartAsync`, `IterateNextAsync`, `IterateCloseAsync` (through `JsRealm.GetAsyncIterator`, `Call` of `next`, `JsEngine.EnqueueAsyncGenerator` and `ResumeAsyncGenerator`, `Call` of `return`) | It settles through the job queue, and the script's `family()` template cannot write it: the owner's answer of 2026-09-17, recorded instead of a shape. The profile drains that queue from its execution driver, outside any step. A drain inside a step, which only an embedder's host function makes, is the job-queue drain row below, outside the stage's depth claim. |
| asynchronous delegation: `yield*` in an async generator | `YieldDelegate` (through `DelegateAsync`) | As `for await`: it settles through the job queue, `family()` cannot write it, and a drain inside a step is the job-queue drain row below. |
| the async step result | `IterateAwaitStep` (`GetProperty` of `done` and `value`) | The instruction is the successor of an `Await`, so it runs only after a resumption drained from the job queue, with the previous frame returned, and no chain of levels nests through it synchronously. A drain inside a step is the job-queue drain row below. |
| a job-queue drain inside a step | an instruction whose arm calls an embedder's host function made with `JsHostRealm.NewMethod` or `NewConstructor` (the call class, or a block opcode reaching one as an accessor, a coercion hook or through an exotic handler), when that function calls `JsHostRealm.DrainJobs` → `JsEngine.StepOneJob` → `JsEngine.Call` of each queued job: a promise reaction, or the resumption of an async function or an async generator | The measured host, the command-line host's run path, defines no host function that drains the queue (only its `--host-surface` checks do), so no family can reach the route. **A drain an embedder runs inside a step is outside the stage's depth claim**, as host exotic objects are, and JSD-0026 states that limit. Recorded by the owner's answer of 2026-09-17. |
| host exotic objects | every property-reading and property-writing block opcode, through `JsHostObject.TryGetOwnProperty`, `SetOwnProperty`, `OwnPropertyNames` and `OwnPropertyCount` → an `IJsHostExotic` handler, which may call `JsHostRealm.Invoke` and nest guest code | The measured host, the command-line host's run path, creates no exotic object, so no family can reach the route. **An embedder's exotic handler is outside the stage's depth claim**, and JSD-0026 states that limit. Recorded by the owner's answer of 2026-09-17. |
| direct eval | `CallEval` on the eval intrinsic (`Evaluate` → the artifact loader → `RunEntry` → `Execute`) | Direct-eval nesting is bounded by the profile's `NestedLoadDepth` (a default of 4 and a maximum of 64), which the lifted-bounds patch does not raise, and `JsEngine.Evaluate` charges no `CallDepth` unit, so the route cannot carry a chain deep enough to matter against the call-depth bound. The owner's answer of 2026-09-17, recorded instead of a shape. |
| dynamic import | `ImportCall` (`DynamicImport` → `Instantiate` → module evaluation) | A module key is instantiated once per realm, so repeating `import()` of a key does not evaluate it again. Nesting is bounded by the distinct modules the artifacts carry, not by a recursion. |
| a module's initialisers from a script unit | `LoadImport`, `ImportMeta`, hand-assembled only | `Instantiate` records the graph before it runs any initialiser, so a second instruction finds it. The route runs once per artifact, and a second level cannot nest through it. |

*Added 2026-09-17, after the per-block baseline steps were committed.* **This addition only narrows the
depth claim.** It removes and relaxes nothing: every family, shape, bound and class above stands, and the
route table still names 36 families. By the owner's answer of 2026-09-17 to a scope question raised when
the `proxyprotoget` family was added, **the trap and accessor families of section 9.1 do not cover two
constructs, and the depth clause claims nothing for either**:

- **A copy of a native accessor installed under another key.** The `proxyprotoget` and `protoset` families
  leave the `Object.prototype.__proto__` getter and setter where the realm installs them, so their arms
  were chosen among the instructions that read or write the key `__proto__`. A copy of either installed
  under another key is reached from arms and through chains those rows did not weigh.
- **A Proxy whose target is another trap-less Proxy.** A trap-less Proxy, one with no trap for the
  operation, forwards the operation to its target, and every Proxy the families create has an ordinary
  object or a function as its target. A Proxy whose target is another trap-less Proxy forwards the
  operation once more before a trap or an accessor is called, which puts about three more native frames
  into each level: for a read, `JsProxy.ProxyGet`, `JsEngine.GetWithReceiver` and `Lookup` again.

**What had been seen when this addition was written.** Section 1 says what had been seen when the values
were set; this is what had been seen when this addition was written. The checks transcripts of `0293c62`,
the commit that makes the baseline form run a block of instructions per step, **print a time for every
row, and they had been seen**, as had those of `940b7d0`, a later commit that fixed one of that commit's
review findings, and the other gate transcripts of both commits, among them the released-bounds logs of
`eng/measure-frame-cost.py` in both forms and a test262 run of a subset in both forms. No figure from them
is used here.

---

## 10. The conformance comparisons

Every comparison uses the pinned `tc39/test262` checkout JSB-11-001 names, `eng/run-test262.py` with
`--fuel 100000000`, and `eng/compare-test262-forms.py`. **Every log is read row by row against the
classes below, not by the script's exit code alone** (JSC-224). A difference in no class is
unclassified, and it is a finding to explain, not to reclassify.

### 10.1 The classes, as the script implements them

**Two forms, one build** (the script without `--same-form`; the first report is the bytecode run):
- **(a)** the native run answered `Unsupported` with the baseline emitter's refusal for an artifact
  whose emitted code would exceed the format's native-code ceiling;
- **(b)** the native run exhausted `Fuel` or `NestedLoadBytes` on a guest-loading variant, one whose
  source contains `eval(`, `Function(` or `import(`. Admitted only with `--exempt-guest-loads`;
- **(c)** a wall-clock exhaustion, admitted only when either run was taken below 60,000 ms, in three
  shapes: the native run exhausted the wall where the bytecode run passed, failed or exhausted another
  allowance; the bytecode run exhausted the wall and the native run passed; or the bytecode run
  exhausted the wall and the native run exhausted another allowance.

**One form, two builds** (`--same-form`; the first report is the control's):
- the script **refuses** two reports that name two forms, and exits 2;
- **(a)** a ceiling refusal on either side, admitted only when both runs are native;
- **(b)** admitted only when both runs are native and with `--exempt-guest-loads`: a guest-loading
  variant where one side exhausted `Fuel` or `NestedLoadBytes` **and the other side passed or exhausted
  an allowance**. A `Failed` or `Unsupported` other side is not (b);
- **(c')** a `WallClock` exhaustion on one side, against `Passed`, `Failed` or an exhaustion of another
  allowance on the other side, at any wall allowance. **Two exhaustions in two dimensions neither of
  which is `WallClock` are not (c')**;
- between two bytecode runs (a) and (b) are not read, so a difference that would be one is
  unclassified.

### 10.2 What each comparison must show

| Log | Reports compared | Every difference is in |
|---|---|---|
| `forms-b12-60000.log` | the candidate's bytecode and native runs at 60,000 ms, `--exempt-guest-loads` | (a) or (b), and the log ends `exit 0` |
| `forms-b12-5000.log` | the candidate's bytecode and native runs at 5,000 ms, `--exempt-guest-loads` | (a), (b) or (c) |
| `native-control-against-candidate-60000.log` | the control's and the candidate's native runs at 60,000 ms, `--same-form --exempt-guest-loads` | (a), (b) or (c') |
| `bytecode-control-against-candidate-60000.log` | the control's and the candidate's bytecode runs at 60,000 ms, `--same-form` | (c') alone, in verdict or in exhausted dimension |

The control's reports are built from the control's binaries on the same machine and allowance, and the
two builds' 60,000 ms runs of each form are taken one after the other. No report of JSB-11-001 is an
input.

### 10.3 The one re-run of a (c') row

Each (c') row is listed in the README and re-run once, alone, on both builds in one session, and the
re-run logs are retained. After the re-runs, a difference in verdict that remains is a defect, and the
bytecode per-variant verdict rows are identical.

---

## 11. The shard job limit

Every shard job of the machine-code run of `test262-javascript-profile.yml` completes inside its job
limit, which is the workflow's own `timeout-minutes: 340` on the `shard` job: the run's jobs, read with
`gh run view <id> --json jobs`, show no job concluded `timed_out` or `cancelled`. The run is dispatched
only with the owner's approval at the time.
