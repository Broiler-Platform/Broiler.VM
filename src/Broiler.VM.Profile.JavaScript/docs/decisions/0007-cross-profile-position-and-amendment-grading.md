# JSD-0007 - The cross-profile position, and the grading of the amendment candidates

**Status:** Accepted for JS-0. **No amendment is filed and none is admissible.**

**Date:** 2026-08-31

**Owner:** profile architecture owner. **Milestone:** JS-0.

## Part 1 - the `WebAssembly` surface is a named exclusion, not an inference

The browser that consumes this profile first will also carry a WebAssembly profile, because the
web reaches WebAssembly **through JavaScript**. `WebAssembly.Module`, `.Instance`, `.Memory`,
`.Table` and `.Global` are a separate specification's objects, defined in terms of JavaScript
values, with a defined coercion in both directions - so implementing them means two core
runtimes, carrying two profiles, exchanging values.

**This profile owns none of that implementation, and the support table says so by name.** The
`WebAssembly` host-object surface appears in no allocated feature manifest and is named as **not
provided**, rather than left for a reader to infer from a second profile being in the image. A
browser image containing this profile beside a WebAssembly one is exactly where a reader will
assume the namespace works.

**Two frozen facts settle the shape, and both are recorded here because the core states them at
its own boundary, which is not where a browser team will look.**

1. **A guest-initiated load may not name another profile.** The provider must answer with an
   artifact of the profile that asked; a different profile is a provider contract breach reported
   as a host failure. So `WebAssembly.instantiate` is **not** a mediated load with a different
   descriptor on it, and no amount of amendment to the mediator makes it one.
2. **Cross-runtime reentry is legal and depth-bounded**, and was admitted deliberately so a host
   object may bridge two independent runtimes. That is the route: the embedder receives the call
   from this profile, converts arguments into the core's transfer types, invokes the other
   profile's runtime, and converts results back.

**The depth bound has a precondition, and it is this profile's business because this profile
originates the call.** The chain is bounded by aggregate call depth, which is only a bound when
both runtimes were created under **one shared parent**. A composition that creates two unparented
runtimes has **no bound on the chain at all**. Stating fact 2 without its precondition would
leave a browser team believing the core bounds something it does not.

### What this profile co-signs, and what it refuses

- **It refuses to ask the core for a cross-profile value channel.** A linear memory exposed as a
  JavaScript buffer is the hard case, not the call: the core's transfer types are integers, byte
  spans and opaque references, none of which is a *shared mutable region*, and a shared mutable
  region is shared semantics by another name. Either the embedder mediates every access, which is
  unusably slow, or something outside both profiles owns the region. **This refusal is co-signed
  here rather than left for the other profile to carry alone**, and it records that growth on one
  side invalidating a view held on the other is a rule nobody currently owns.
- **It does not foreclose the seam.** Where a design choice in this profile would make the seam
  harder - a host-object model with no stable identity, a realm model that cannot hold a foreign
  exotic object, a transfer surface that cannot carry an opaque reference - the choice is recorded
  with that consequence noted at the milestone that takes it. JS-4, which takes the object model
  and routes the front end and the executor through a realm object, is where this bites first.
- **It names the owner.** A browser integration is a consumer of two profile families and belongs
  to whichever component composes them. That component owns the two-profile composition's closure
  report, its Native AOT evidence, its shared aggregate budget, and the reconciliation of two
  profiles' **defaults**. That component does not exist and has no owner, which the status ledger
  carries as an unopened dependency.

**One price, stated so a browser team meets it here rather than in a benchmark:** every
JavaScript-to-WebAssembly call is **two host-boundary transits and a conversion in each
direction**. That is the correct price for two profiles that share no semantics, and it is a
price that shows up in exactly the measurement people run first.

## Part 2 - the amendment candidates, graded

Each candidate is a **proposal or a refusal**, never a workaround inside the core's execution
loop. Each carries the counterweight test: would a profile with no parser, no text format and no
dynamic loads need this too, or is this one language's need wearing a general shape?

**The counterweight answer is not this profile's to write alone.** The core's procedure requires
every amendment record to state whether the other intended profile could use the capability, is
unaffected, or refuses it - so a row graded without knowing the other profile's grade is a row
whose answer changes depending on which component files first.

| Candidate | Grade | Note |
|---|---|---|
| An argument channel on invocation | **Strong** | Re-graded upward. An earlier draft graded it weak on the ground that a fixed-entry-point profile would not need it. The other intended profile rates it the strongest ask in its own document, on the ground that a language with no parser, no text format, no dynamic loads and no notion of a program still needs it - which is the counterweight test passing, not failing. And it stops being mild for this profile the moment it hosts another one: `instance.exports.f(a, b)` is a typed call whose arguments originate here |
| A result channel on invocation | **None needed** | **Split out of the row above, which is the second correction.** The typed payload already carries results, and several of them, so multi-value returns are expressible today. Filing argument and result as one amendment would put two differently-scoped versions of one capability into the register, which is how a capability gets approved at the wrong width |
| Multi-result host capabilities | Moderate | Any profile whose calling convention admits multiple results meets it; the other intended profile raises it independently. Until then the refusal is deterministic and published |
| A wider value slot on the capability channel | Weak | Recorded so it is not mistaken for the row above. A value wider than the slot must be split, which works and needs a published encoding |
| Nested instantiation through the mediator | Moderate | Not needed for `eval`, which runs in the caller's realm. Opened only if this profile's realm model requires a separate instance per module - and one instance may hold several realms, so a module needing its own realm does not by itself need its own instance |
| A charging hook for work done inside a host capability | Strong | General. Wall clock covers a slow capability; it does not cover one that allocates on this profile's behalf |
| An in-process producer input form - compiling straight to a verified handle | Moderate | General to any composition that compiles at run time; a profile shipped as pre-built artifacts never meets it. **Opened only against JS-10's verification-throughput-per-byte and cold-start figures, never against an intuition** |
| Lazy per-section verification | Moderate | Any profile with large artifacts and a cold-start budget meets it. Invariant 3 fixes the shape of any proposal this profile would sign: each section verified **completely** before that section's first execution, with no structural, index, stack-consistency or handler-nesting check migrating into execution |
| Streaming or incremental verification | Strong | General, and the core already carries a registered amendment shape for it. Reopened against a measurement |
| A persisted envelope | Strong | General, and already admitted by contract. It needs a gate rather than an amendment |

**Two workarounds are tried first for the argument channel and their cost recorded**, per the
roadmap: encoding the call into the entry-point text, which works and is ugly, and lowering a
one-line calling program and verifying it as a guest-initiated load, which is correct and costs a
verification. JS-1 picks one and records it.

**The rule that governs all of them:** a design that can only be hosted by a second core state
machine is refused. Exactly one core state machine and one core contract version exist in a
product graph at any time.

## Why every row is filed and held rather than scheduled

**The amendment procedure is currently unexecutable.** No amendment has been minted, and the
minting role and both co-signing roles are held by one person, so a co-signature would not be
independent. And a counterweight **refusal** by the other profile is **recorded, not blocking**:
a profile with a veto over a core amendment would be a profile-to-profile dependency established
by governance rather than by reference, which is what the extraction gate's fourth condition
exists to prevent.

None of these rows is admissible until it names a merged or approved capability. **JS-0 files
none.**

## Addendum, 2026-09-01 - the refusable retention member, which is the other profile's ask

Part 2 graded every candidate this profile might ask for and none it would not. One capability the
other intended profile asks for - **a refusable retention member on the metering surface**, so
that a ceiling-class dimension can carry a guest-observable refusal - had no row here, and the
core's procedure asks each amendment record to carry the other profile's position: could use it,
unaffected, or refuses it. The row is the one the other profile opens as blocking rather than
holds, so it is the row most likely to be filed first, and it was the one row whose counterweight
field this profile had left empty.

**This profile's position is *unaffected*.** It has no construct that needs a guest-observable
budget refusal - a JavaScript allocation failure is a host-level condition, not a value the
language reads back - and [JSD-0004](0004-limit-defaults-hard-maxima-and-the-budget-matrix.md)
records that as a property to preserve rather than a coincidence to rely on. So this profile
neither files the row nor obstructs it, which is what roadmap
[section 18](../roadmap.md#18-amendments-this-profile-expects-to-ask-of-the-core) has said in its
own voice since the corrections were lifted out of it; what this addendum adds is the date and the
record, so the position can be cited by the other profile's amendment record rather than read off
a plan.

**Nothing in Part 2 moves.** No grade changes, no row becomes admissible, and the procedure is
exactly as unexecutable as the section above records.
