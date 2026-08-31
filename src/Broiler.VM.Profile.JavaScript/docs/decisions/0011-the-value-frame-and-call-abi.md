# JSD-0011 - The value, frame and call ABI, and what it costs JS-6

**Status:** Accepted for JS-4 **as a gate on entry**. This record is the decision roadmap
[section 8](../roadmap.md#8-the-value-frame-and-call-model) requires before JS-4 begins and
before any standard-library source file is copied. **JS-4 is not started**: nothing below is
implemented beyond what JS-1 already ships, and a decision record is not implementation evidence.

**Date:** 2026-08-31

**Owner:** profile runtime owner. **Co-signer:** the profile built-ins owner, whose milestone this
re-scopes. **Both roles are held by one person** and this record does not claim the co-signature
is independent.

**Milestone:** JS-4, entry gate.

## Why this is taken now, and not at JS-4

The standard library is typed against whatever answer this gets. Roadmap
[section 23](../roadmap.gates.md#23-risks-and-stop-conditions) makes copying a standard-library
source file while this decision is open a **stop condition**, and makes the re-scoping of JS-6
something that happens *before* that milestone starts rather than during it. JS-2 is blocked on
the core's acceptance gate, so waiting for it would have left the stop condition open across
every milestone that could otherwise proceed. **This decision needs no copied code and is taken
against JS-1.**

## The decision in one line

**This profile keeps its own struct value and does not adopt the seed's boxed hierarchy — so
JS-6 is re-scoped from a copy to a rewrite, now.**

## Row 1 - Representation

| | |
|---|---|
| **Decided** | A tagged struct with an explicit kind, a `double` payload and a managed-reference payload. Number, Boolean, Undefined and Null are the struct alone and allocate nothing; String, Symbol, BigInt and every object are the reference field. |
| **Buys** | No allocation on the arithmetic path, which is the seed's own most-measured defect avoided rather than inherited. A real managed reference in a real field, so the collector scans it without handles, an object table, or anything that would have to be re-proved under Native AOT. |
| **Costs** | **Twenty-four bytes per slot, not sixteen**, and this record says the number rather than rounding it: an eight-byte reference, an eight-byte double and a one-byte kind, padded. Every operand slot, local, argument and captured binding pays it. |

**The compact packings are registered and not adopted.** NaN-boxing the whole value into eight
bytes, or folding the kind into the reference's null-ness plus the double's NaN payload, would
each reach sixteen or eight. Neither is taken here, for two separate reasons. A moving collector
may not have a reference hidden inside a payload word, so NaN boxing on this runtime means an
object table and a handle indirection - a second lifetime mechanism to design, test and re-prove
under Native AOT, bought with the allocation the representation exists to avoid. And roadmap
section 8 says in its own words that **a representation is not accepted because it looks
compact**. The packing is a candidate optimisation with a named gate: JS-5 measures the
interpreter with the readable representation, and a packing is adopted only against that figure.

**Falsified if:** an arithmetic sequence over Numbers allocates, or a value's reference field is
reachable by the collector only through a mechanism this profile wrote.

**What JS-1 ships is the same decision one field short.** Its value is the kind and the double,
sixteen bytes, over three primitive kinds - and JS-1's own record calls it provisional. It is not
superseded; it is completed. The reference field is what a manifest with strings and objects adds,
and no figure recorded against the three-kind struct may be read as a figure about this one.

## Row 2 - Rooting and lifetime

| | |
|---|---|
| **Decided** | Every value-holding region is a managed array of the value struct: operand slots, locals, arguments, captured bindings. The collector roots them because the reference field is an ordinary field. **No manual rooting, no handle table, no finalizer, no `GCHandle`.** |
| **Ownership** | Constants belong to the **verified artifact** and are immutable and shareable across runtimes with matching identity. Everything else belongs to the **operation** that created it and dies with it. |
| **Buys** | Nothing to get wrong. A rooting bug in a hand-written scheme is not a bug a corpus finds. |
| **Costs** | A value array cannot be a raw memory block, so an operand stack is a managed array and not a `stackalloc` span. |

**One consequence is load-bearing and is stated here rather than discovered at JS-4's gate.** A
constant that carries a managed reference - a String constant, once a manifest admits one - must
hold nothing instance-owned, because JS-4's exit gate requires that no instance-owned cache, shape
table or feedback be reachable from a shareable handle. So a String constant is either an
immutable value the artifact owns outright, or a per-realm interning of it that the *instance*
owns and the handle does not. **The second is the answer**, and it is why `InternedName` is
reserved in format version 1 and admitted by no manifest yet: the pool entry names a string, and
the realm-scoped identity is minted at instantiation.

**Falsified if:** a shareable handle transitively reaches a per-instance interning table, or a
value's reference outlives the operation that made it without the artifact owning it.

## Row 3 - Call and construct

| | |
|---|---|
| **Decided** | A call is an explicit frame pushed onto an explicit frame list. `this`, `new.target` and the arguments object are **slots in the frame**, not ambient state and not thread-static. A host call crosses the core's capability surface and never a CLR interface the guest can name. |
| **Buys** | Two operations in one process cannot corrupt each other's call state, which is the same property roadmap section 9 demands of parse options and for the same reason. |
| **Costs** | Every call writes three slots that a CLR-stack design would carry in registers. |

**Falsified if:** a metadata scan finds a thread-static field or an ambient async-local type in the
executor assembly, or two concurrent operations observe each other's `this`.

## Row 4 - Frames

| | |
|---|---|
| **Decided** | An interpreter frame is a **heap object owned by the operation**, and the dispatch loop is one CLR frame regardless of guest depth. Guest recursion grows a list, not the CLR stack. |
| **Buys** | Two things that are otherwise not available at all. `CallDepth` becomes a **counted number compared against a bound**, so a recursing program is refused as `ResourceExhaustion` naming `CallDepth` rather than terminating the process - which roadmap section 8 requires and which a stack-probing design cannot promise under Native AOT. And a frame with no CLR-stack state is a frame that can be **captured**, which is what Row 6 needs. |
| **Costs** | An allocation per call, and a dispatch loop that cannot use the CLR call stack for its own recursion either. |

**`CallDepth`'s default is still measured and this record does not choose it.** What this row fixes
is that the number bounds *heap frames* and is therefore knowable; JS-5 measures the per-frame cost
on each claimed RID and records the figure with the measurement. The value JS-1 declares is safe,
not right.

**Falsified if:** a recursion case terminates the process on any claimed RID under Native AOT, or
the dispatch loop's CLR stack depth grows with guest call depth.

## Row 5 - Completion

| | |
|---|---|
| **Decided** | An explicit completion record - normal, return, break, continue, throw - carrying a value and, for break and continue, a target. It is **produced by the instruction**, not inferred from where the dispatch loop happened to exit. |
| **Buys** | An abrupt completion crossing a `finally` is a value the code passes around rather than a control-flow accident, which is the only shape in which the interaction is testable. |
| **Costs** | Every step returns a record even where nothing is abrupt. |

**Falsified if:** a completion kind is distinguishable only by which `return` statement in the
dispatch loop ran.

## Row 6 - Suspension

| | |
|---|---|
| **Decided** | A suspension is the frame list, stopped. Because Row 4 puts every frame on the heap and no interpreter state on the CLR stack, capture is "keep the frame graph and the instruction pointer" and resume is "re-enter the dispatch loop at the saved offset". **Designed here, implemented at JS-7.** |
| **Buys** | The retrofit that is otherwise impossible. A frame model that cannot be captured cannot be made capturable later without rewriting the executor. |
| **Costs** | The heap frame of Row 4, paid on every call whether or not anything ever suspends. |

**Falsified if:** any executor state that a resume needs lives somewhere other than the frame graph.

## Row 7 - Safepoints

| | |
|---|---|
| **Decided** | Source, exception, suspension and diagnostic safepoints are all canonical against **bytecode offsets**, and a position reported at one is the encoding [JSD-0009](0009-the-diagnostic-registry-and-the-position-encoding.md) publishes: the code section's ordinal, the offset into it, and the line and column of the covering position-table row. |
| **Buys** | One coordinate system. A stack trace, a breakpoint, a resume point and a diagnostic all name the same thing, and the format's position table is already keyed on it from version 1. |
| **Costs** | A later specialization may not move an instruction boundary without moving the table with it. |

**Falsified if:** a safepoint is reported in coordinates JSD-0009 does not define, or two safepoints
at one bytecode offset report different positions.

## Row 8 - Metering

| | |
|---|---|
| **Decided** | `Poll()` at every backward branch and at every call and return - the two places a guest can loop. `AllocatedBytes` charged **before** each heap allocation the executor makes, which Row 4 makes a per-call charge. Executor work charged per instruction retired at the declared granularity, in this profile's own work units and never in measured time. |
| **Buys** | A charge that is where the cost is. A representation that made charging awkward would be a decision with a hidden cost, which is what section 8 asks this row to expose. |
| **Costs** | Row 4's per-call allocation is now also a per-call **charge**, so a call is metered twice - once for depth and once for bytes - and both are deliberate. |

**Falsified if:** an allocation reaches the heap before its charge, or a poll interval exceeds the
profile's declared uncharged-work bound on any path.

## The cost this record accepts: JS-6 is a rewrite

Roadmap section 23 requires this to be said now rather than during the milestone.

The seed's every value is a heap-allocated reference type and its standard library is typed
against that base type. **This profile is not adopting that hierarchy**, so the library cannot be
copied and re-typed: it is re-implemented against the value struct above.

**Why not adopt it, given how much a copy would save.** The reason is not that a struct is faster,
though roadmap section 4.4 records the boxing as the seed's own most-measured defect. It is that
**this profile already has a struct value model with an executor written against it and a
fifty-nine-entry corpus pinning its semantics**. Choosing the seed's hierarchy would not be
avoiding a rewrite; it would be moving the rewrite onto JS-1's executor, and replacing this
component's own working code with a defect the seed has measured and not fixed.

**Rejected: a mechanical re-typing of the library.** If the seed's library named one abstract base
and nothing else, a re-typing would be a copy with a find-and-replace. It does not: a library that
pattern-matches on concrete value subclasses is re-implemented wherever it does, and the parts that
survive arrive as unreviewed code that no test in this component covers, wearing a copy's name and
a copy's schedule. Naming it a rewrite is the cheaper mistake.

**What is still copied.** This decision re-scopes JS-6 and touches nothing else in
[the copy table](../roadmap.md#43-the-copy-table). The property storage, the shapes and transition
table, the element arrays and the named-property store are copied with their tests and their
recorded defect history, because they are about *storage keyed by a value*, not about the value's
representation. The front-end analyses JS-3b re-homes are untouched.

**What JS-6 must now carry that a copy would not have:** its own scope estimate, its own review
budget - a rewrite is unreviewed code written here rather than unreviewed code copied here, and
this component's review debt counts both - and an explicit exclusion list published on the day it
lands, because a rewritten library is smaller than a copied one and the difference is a support
claim.

## What this record does not decide

- **No implementation.** No object model, no property storage, no string, no frame object, and no
  executor change. JS-4 owns all of it and depends on JS-2, which is blocked.
- **No numbers.** `CallDepth`, `MaxUnchargedWork`, `ChargingGranularity` and
  `CancellationPollBound` are measured at JS-5 and this record chooses none of them.
- **No fixtures and no probes.** Section 8 requires a correctness fixture and a Native AOT
  representation probe beside each row; **none is retained by this record**, because seven of the
  eight rows have nothing to exercise until JS-4 and JS-5 exist. The gate that requires them is
  JS-4's, not this one's, and the ledger records that they are absent.
- **Nothing is reviewed.** No human has read this record or anything it decides.
