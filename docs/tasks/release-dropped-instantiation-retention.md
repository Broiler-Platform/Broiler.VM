# Suggested task — Release the retention of an instantiation the core drops

**Status:** done 2026-09-28, in `45778cf` on the branch of PR #102, which merged milestone UBC-4 in
beside it: every instantiation path that publishes no instance now releases the instance level's
ceiling-class consumption from the runtime and parent levels through the helper instance disposal
uses, and contract tests watched failing before the change show `LiveBytes` back at baseline at the
runtime and at an aggregate parent. What stays true is that the core drops the state without
abandoning it through the executor. *(The status read "suggested, not scheduled, no owner." until
then, and the rest of this document is kept as it was written.)*

**Status as written:** suggested, not scheduled, no owner. Found 2026-09-25 while building the WebAssembly family
of [the universal bytecode programme](../universal-bytecode.roadmap.md)'s milestone UBC-4, and named in
that milestone's bundle [`ubc-4-002`](../evidence/ubc-4-002/README.md) as a defect of the core that the
milestone does not fix. This document is the task written out so that a session starting from it alone
has what it needs; it moves no ledger row and claims nothing about what any run has shown.

## In one paragraph

When a refusal or a cancellation latches on the meter during an instantiation that the executor
nevertheless answers as complete, the core answers an exhaustion or a cancellation, publishes no
instance, and drops the executor's state without releasing what the instance level holds of the
ceiling-class dimensions - `LiveBytes` above all. That consumption is also committed at runtime level
and, for a dimension of aggregate scope under an aggregate budget, at the parent's. It stays charged for
the runtime's life, and at the parent beyond it, because disposing of a runtime releases nothing to the
parent; a later instantiation in the same runtime, or in a sibling runtime under the same aggregate
budget, is refused early for room nobody holds.

## What is wrong, checkable against the files named

All paths are relative to the repository root.

- `src/Broiler.VM.Runtime/VmInstantiation.cs`, after the executor's `Instantiate` returns: it answers
  a cancellation when `meter.CancellationObserved` and a resource exhaustion when
  `meter.ExhaustionObserved`, including when the step is `Instantiated` with a state. On those paths
  the state is dropped, no instance is published, and nothing releases the instance level's
  ceiling-class consumption.
- `src/Broiler.VM.Runtime/VmMeter.cs` commits every charge at the runtime, instance and invocation
  levels, and charges a dimension of aggregate scope to the parent budget as well, so the dropped
  consumption is also held at runtime level and at the parent's.
- `src/Broiler.VM.Runtime/VmAggregateBudget.cs`, `ReleaseRuntime`, only counts a runtime out when it is
  disposed; it gives none of the runtime's consumption back to the parent.
- `src/Broiler.VM.Runtime/VmInstanceImplementation.cs`, `ReleaseRetained`, is how a disposed instance
  gives the same consumption back; nothing calls its equivalent on the dropped path.
- A latch can be set while the step still succeeds: the meter's poll can latch an aggregate
  wall-clock refusal and still return true, so an executor that checked every charge it made can
  still have its answer turned into an exhaustion.

The WebAssembly family releases what its store retained on every failure the bytecode emitter
answers; it cannot release on this path, because the core never tells it the instance was dropped.

## What done looks like

1. **A contract test that shows the leak** in `src/tests/Broiler.VM.Contract.Tests`, using the
   fixtures a test profile there already uses: an executor retains `LiveBytes` at instantiation, a
   refusal or a cancellation latches after it, the instantiation answers an exhaustion or a
   cancellation, and a second instantiation in the same runtime that needs the ceiling's remaining
   room is wrongly refused. Watch it fail before the change.
2. **Every instantiation path that publishes no instance releases** the instance level's remaining
   ceiling-class consumption from the runtime and parent levels, the way instance disposal does.
3. **Every suite stays green** - the contract and architecture suites after
   `dotnet build Broiler.VM.slnx -c Release`, with the assurance artefacts regenerated when an
   annotation changes - and the change says whether it touches the core contract's behaviour enough
   to need a row in ADR 0003's register.

## What this task is not

- It is not a change to any profile. A profile that releases what it retained on its own failures
  keeps doing so; this task closes the one path a profile cannot see.
- It is not blocked on the universal bytecode programme and does not depend on it.
