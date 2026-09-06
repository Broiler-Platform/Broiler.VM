<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0022 — What the nesting bound bounds, and the stack it is derived against

**Status:** Taken. 2026-09-06.

**Context.** [JSC-185](../roadmap.corrections.md#jsc-185) closed two parser recursions the depth
counter was never entered from, and recorded that a third shape still terminated the process and
was not the same defect. This record takes the decision that shape needed.

---

## 1. The shape, and why it is not the one JSC-185 closed

`var x = 1+1+1+…;` ends the process at a few thousand terms. So does a chain of `&&`, and a member
chain `o.a.a.a…`. It happens under `--check`, which parses and verifies and runs nothing, and it
happens at every `--max-depth`, including the smallest one a caller may state.

**The counter cannot see it, and that is a property of the grammar rather than an oversight.**
Precedence climbing is **iterative** for a left-associative operator: `ParseBinary` loops rather
than recursing, so a chain of any length costs one parser activation. The call-chain loop is the
same shape. What those loops build is a **left spine as long as the source**, and
`CompileExpression` descends it once per node.

**So two resources were being spent and one number was counting one of them.** The nesting bound
is what the *parser's own recursion* costs — a parenthesis costs parser stack and builds no node at
all. The tree is what the *walks* cost, and nothing counted it.

---

## 2. What was considered

| Option | What it does | Why it was not taken alone |
|---|---|---|
| **Make the walks iterative** | Rewrite the lowering's expression walk to use an explicit stack for left spines | It is the only option that changes no admitted program, and it was measured against the wrong problem: the sweep found **eleven** shapes on both front ends, through binary, logical, member, index and call nodes, and the short-circuit operators need jump patching that an explicit stack makes materially harder to get right. It also fixes the walks that exist — the next recursive walk somebody writes re-opens the whole thing |
| **Bound the tree** | Count what the iterative builders add, and refuse past a ceiling | Correct in kind, and at the existing ceiling it would refuse a sixty-five-term addition, which is not a language this profile could claim to admit |
| **Bound chain length** | Refuse a chain longer than N | The same language cost as bounding the tree, arrived at less directly: for a left spine, chain length *is* tree depth |

**The three are not alternatives in the way the question assumed.** Bounding the tree and bounding
the chain are the same bound for the shape in question. The real variable is not *whether* to
bound but **what the ceiling can afford to be**, and that is a question about a stack.

---

## 3. The decision

**The nesting bound stays what it is — a bound on the parser's recursion, stated by the host — and
a second bound is minted on the depth of the tree the front end hands to a walk. Its ceiling is
derived against a stack this component declares for compilation, and it is not the host's to set.**

Three parts, and the record takes all three together because no two of them are sufficient:

1. **`SliceParseOptions.MaximumTreeDepth`**, charged by the two iterative builders in both front
   ends — the precedence-climbing loop and the call-chain loop — and reported as
   `2103:NestingTooDeep` with a message naming the tree rather than the source.
2. **`CompilationStack`**, which runs every public compile entry point on a thread whose stack size
   this component states. This is not a new idea here: `JsExecution` already runs a guest
   invocation on a declared stack and states why — *otherwise the call-depth ceiling means a
   different thing on every host*, and *a stack overflow is the one failure the CLR cannot turn
   into an exception*. Every word of that applies to a walk over a syntax tree. The decision was
   taken for execution and never extended to compilation.
3. **The ceiling is a constant of the assembly and not a field of the parse options.** A host
   states policy; this is a property of a stack the host cannot see and cannot measure, and a host
   raising it past that stack would be asking for the termination the bound exists to prevent, in
   a call that looks like configuration.

**Rule N19 holds all three**, because a checkout with any two of them is broken in a way the
remaining one hides.

---

## 4. What it costs

**A program whose expression tree is deeper than the ceiling is refused, and no program anybody
writes is.** The ceiling is ten thousand nodes. Hand-written JavaScript does not reach it; the
shape that would is generated source, which is exactly the material this profile's workloads are
made of — so the ceiling was chosen to sit above what a minifier emits rather than above what a
person types. A five-thousand-term sum compiles, runs, and prints its answer, and there is a
retained command line asserting that it does.

**The refusal is honest but it is not free of judgement.** Ten thousand is a number this record
chose. It is derived — the measured failure point on the caller's stack, multiplied by the declared
stack's ratio to it, divided by a margin of about six — and every input to that derivation is in
the constant's own remark, so a later reader who thinks the margin is wrong can re-derive it rather
than argue with it.

**A thread per compilation is spent that was not spent before.** It costs a fraction of a
millisecond against a compile that costs milliseconds at least, and it is the same trade the
execution side already made per invocation.

**And the ceiling is a cliff that has been moved rather than removed.** That is stated plainly
because it is the honest description: the bound is what makes the answer a refusal at every size,
and the declared stack is only what lets the bound sit somewhere harmless. If a future walk costs
materially more stack per node, the derivation changes and the constant has to move with it — which
is why N19 asserts that the constant's remark names the stack it was derived against.

---

## 5. What this does not decide

- **It does not make the walks iterative.** That remains the change that would remove the ceiling
  rather than place it, and this record does not close the door on it: a lowering that walked a
  left spine iteratively would let the constant rise or go. Nothing here depends on it.
- **It does not bound anything the parser already bounds.** The nesting bound is untouched, its
  default and maximum are untouched, and `--max-depth` still means exactly what it meant.
- **It does not claim the sweep was exhaustive.** It covered every construct in the admitted
  grammar that nests, on both front ends, and a construct nobody named is a shape nobody swept.
  What changed is that the failure mode for anything missed is a refusal in the general case: the
  bound is charged where trees are built, not where particular operators are recognised.
- **And it settles nothing about the run-time stack.** `CallDepth` and its declared guest stack are
  a different resource with a different record, and the two ceilings are derived separately even
  though they name the same number of bytes today.

---

## 6. Falsifiable claims

- A source whose tree is deeper than `MaximumTreeDepth` is compiled rather than refused.
- A compilation walks a syntax tree on the caller's stack.
- The tree bound becomes reachable through parse options or a command line.
- A source at any size terminates the process on either front end.
