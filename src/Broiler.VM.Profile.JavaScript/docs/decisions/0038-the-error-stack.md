<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0038 - An error's `stack`: V8's shape, captured from the running frames and the position table

**Status:** Proposed, 2026-10-04. **Owner decision pending.** The code this record describes is in
the tree. Taking the record accepts the shape in section 2, the per-instruction site write in
section 3 and the extra position rows in section 4; not taking it leaves a member the edition does
not define and that the realm would have to remove again.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the second record of phase F3 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile), which asks for "a
decision record for stacks, choosing the accessor's shape from the comparison engines' common form
and the position table the artifact already carries". The first is
[JSD-0037](0037-the-source-text-section.md).

**Context.** `stack` is not a member of the pinned edition. Until this record `error.stack` read
`undefined`, nothing an error inherits carried one, and `runs/an-error-has-no-stack.js` pinned that
answer (JSP-7, *JSC-239*). Every engine this profile is compared against gives errors a stack, and
programs read it: logging, test harnesses - the suite's own SpiderMonkey shell harness calls
`exc.stack.trim()` - and libraries that parse it. The comparison engine of the differential probes is
node, which is V8.

---

## 1. What was true before, verified

| Question | Answer | Where |
|---|---|---|
| What did `new Error('x').stack` answer? | `undefined`; `'stack' in error` was `false` | `runs/an-error-has-no-stack.js` |
| Does the artifact map code to source positions? | Yes, a position row per statement and expression start, ascending by code offset; the verifier judged the rows and kept only their count | `JsCompiler.cs` `Position`, `JsVerifier.cs` `ReadPositions` |
| Where did a call or construction's instruction map to? | To its last argument's row, because the arguments are compiled after the expression placed itself | `JsCompiler.cs` `CompileCall` |
| Did the engine know the running frames? | Only as a depth count, and as the legacy list of sloppy plain calls JSC-257 keeps | `JsEngine.cs` |
| Where does the interpreter keep the running instruction? | In a local of the dispatch loop, beside the fuel charge every instruction pays | `JsEngine.cs` `ExecuteCore` |
| How did the comparison engines differ? | V8: an own accessor pair, listed before `message`, formatted on first read, `    at name (file:line:column)` lines, ten frames. JavaScriptCore: an own data property. SpiderMonkey: an accessor on `Error.prototype`, `name@file:line:column` lines | node 22; the engines' documentation |

## 2. The shape

1. **Every error has an own `stack`**: an accessor pair, configurable and not enumerable, defined
   before any other own property - so `Object.getOwnPropertyNames(new Error('x'))` answers
   `stack,message`, as in V8. Every error means what the `Error` constructors make (all six native
   kinds, `AggregateError` and `SuppressedError`, with or without `new`, directly or through
   `super()`), what the engine raises, what disposal combines, and what the clone carrier rebuilds.
   `Error.prototype` has no `stack`.
2. **One getter and one setter are shared by every error.** The getter answers `undefined` for a
   receiver that is not an error; the setter does nothing for one. Assigning `stack` replaces the
   value and the property stays an accessor, as in V8.
3. **The frames are captured when the error is made, and rendered when `stack` is first read.** The
   text is then the error's value: a `message` changed before the first read is shown, one changed
   after is not. A header that throws renders as `<error>`, as in V8.
4. **The text is V8's.** The header is what `Error.prototype.toString` answers. Each frame, innermost
   first, is `    at name (place:line:column)`, `    at new name (...)` for a construction, and
   `    at place:line:column` for an anonymous frame - a script, module or eval body, or an anonymous
   function. The place is the frame's script or module referrer, `<anonymous>` when it has none. The
   name is the function's own `name` when that is a String data property, read without running an
   accessor, and the unit's name otherwise.
5. **At most ten frames**, V8's default `Error.stackTraceLimit`. A construction through `super()`
   leaves out the constructors that reached the `Error` constructor, down to and including the one
   `new` named, as V8 does.

**What is not V8's, on purpose.** A method frame is named by the function, not prefixed by the
receiver's constructor (`m`, not `C.m`). A built-in has no frame (V8 shows `at Array.map
(<anonymous>)`). A frame a proper tail call replaced is gone, which in the bytecode form is every
strict tail call (*JSC-256*); JavaScriptCore shows the same. A call whose callee is not a name or a
member is placed at the callee's start, not at its argument list. There is no
`Error.captureStackTrace`, no `Error.stackTraceLimit` and no `Error.prepareStackTrace`: each is a V8
extension, and a program that tests for them finds them absent.

## 3. How the frames are known

- **One site per running frame.** `Execute` takes a site for every frame it runs - a call, a
  construction, a generator's or async function's resumption, a script or module body - and the
  value form's direct call takes one, because it enters a frame without `Execute`. The sites are an
  array reused by depth; a site whose frame ended keeps no reference to its program or function.
- **Every instruction the dispatch loop runs writes its offset into the innermost site**, next to
  the fuel charge. That one store is what places a frame that called out - through a call, a getter,
  a `valueOf`, a proxy trap - at the instruction that did. The emitted forms run their instructions
  through the same loop, one step or block at a time and always for the innermost frame; a direct
  call, the one way emitted code leaves a frame without the loop, places the caller's site itself.
  The three forms render the same text.
- **Cost, measured.** On a call-, loop- and property-heavy script, five runs of each build: 1.645
  seconds against 1.638 before, inside the spread between runs. A capture costs one unit of fuel per
  frame and builds no string; the first read charges the text's length.

## 4. Where a frame is placed

- **The position table is kept by the verifier** and searched for the last row at or before the
  frame's instruction within its unit.
- **A call and a construction are placed again just before their instruction**, after their
  arguments: a construction at its `new`, a call of a named member at the member's name, any other
  call at its start. **A named member read is placed at its name**, so an error it raises is shown
  there. These are the columns V8 reports. The cost is one more position row per call with
  arguments and per named member read, and every artifact the lowering writes changes bytes.

## 5. What it amends and what it does not decide

- **JSD-0014's bullet "No guest-visible metadata carries it, because none carries a position"** is
  no longer true of the position table if this record is taken: `stack` reads it. Nothing else in
  that bullet changes - there is still no `fileName` or `lineNumber`, and an `eval` refusal still
  carries no position.
- **JSD-0032 is unchanged**: the carrier does not carry `stack`. A rebuilt error has a stack of its
  own, captured where it is adopted.
- **Not decided:** `Error.captureStackTrace` and the other V8 extensions; async stack frames
  (`at async f`); a host's control over the referrer shown for its scripts beyond the referrer it
  already gives a compilation (JSD-0024 section 20).
