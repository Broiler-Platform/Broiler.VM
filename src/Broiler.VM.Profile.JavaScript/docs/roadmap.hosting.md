<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The hosting roadmap - what the host surface would take for a real embedder

**What this document is.** A design analysis and a proposed programme for one objective: that an
embedder with a document-shaped object model - a DOM, a style declaration, a storage area, an event
target - can put its objects in this profile's realms and be answered honestly about what it may and
may not do there. It is written against the seam, the engine and the core as they stand in this
checkout, and it names, for every obligation it records, either the stage that owns it or the fact
that nothing does.

**One of its stages is built and the rest are proposals nobody has scheduled.**
[JSH-1](#jsh-1---the-seam-values-identity-a-step-bracket-and-an-abort-that-cannot-be-swallowed) is
code in this checkout: the values, the identity table, the step bracket, the abort latch, the mint
and define and read members, the exotic object, the call back into the guest, and a lane of checks
each judged by what the guest printed. JSH-2 through JSH-8 are **written down and nothing more**.
None has an owner, none has a date, none is in any plan file, and a reader who takes the list below
as a schedule has read it wrongly.

**What this document is not.** It is not the ledger and it moves no row in one:
[section 2 of the evidence ledger](roadmap.status.md#2-current-milestone-status) remains the only
authority on what this component has done. **Owning code is not acceptance and is not evidence**: no
bundle has been retained for the seam, no human has read a line of it, and the stage below that owns
code says so in the same breath as it says what it does. It is not a milestone set:
[section 19 of the delivery file](roadmap.delivery.md#19-milestones) holds `JS-0` through `JS-10`,
and this document mints no identifier in that namespace, because a `JS-` identifier with no ledger
row would read as a milestone somebody is tracking. It is not a status: what the seam does today is
[section 2](#2-what-is-built-stated-as-a-mechanism-rather-than-as-a-capability), stated as a
mechanism a reader can check against the source, and not as a capability anybody may declare. And it
is not a fourth gap analysis - [the workload roadmap](roadmap.workloads.md) owns the absences in the
language surface, [the parity roadmap](roadmap.parity.md) owns the mechanisms inside the surface
already admitted, and [the backend roadmap](roadmap.backends.md) owns the second output form. Where
this document meets one of theirs it points at it rather than re-minting it. Its stages are `JSH-n`
and they are proposals for where the existing milestones would have to grow.

**And it is not the browser's plan.** The component that would consume this seam is a JavaScript
engine abstraction in a different repository, with its own contracts, its own conformance suite and
its own capability flags, and **nothing in this document declares a flag there or schedules a
provider**. What this document can honestly do about that layer is
[JSH-8](#jsh-8---the-flag-ledger-which-declaration-each-stage-would-make-honest): say which
mechanism each of those flags names and which stage here would supply it. Making the declaration is
an act in that repository, against a provider that does not exist, and the mapping is the embedder's
to make rather than this profile's to assert.

**And it carries no figure of any kind** - no timing, no ratio, no per-crossing cost, no score. That
rule is not precautionary here; it is the thing
[JSH-6](#jsh-6---proportional-charging-measured-rather-than-chosen) exists to fix. The fuel a
crossing charges is a constant somebody chose, no measurement of what a crossing costs relative to
an interpreted instruction exists in this repository, and under the ledger's
[update rule 10](roadmap.status.md#5-update-rules) a number with no retained record behind it is not
a number this document family may state. Where a figure would be the natural way to say something,
this document names the member, the call site or the command instead.

**And one record stands above this one.** The MVP programme at
[`docs/mvp.md`](../../../docs/mvp.md) states what this stage of work defers and what deferring a
decision does not defer, and this document defers to it: what is deferred is approval of the
boundary records, human review, evidence-bundle collection and milestone acceptance, and the
co-signing step of the core contract amendment procedure, and what is **not** deferred is the
automated gates, the status vocabulary, the stop condition on an untruthful support claim, the
non-advertisement of every composition and the three-package pack set, and the prohibition on
publishing. Beside all of them stands the rule that a document plans work and records state rather
than reporting a capability no run has shown. Every sentence below is written under that rule.

---

## Contents

1. [What hosting means here, and where it is not the capability channel](#1-what-hosting-means-here-and-where-it-is-not-the-capability-channel)
2. [What is built, stated as a mechanism rather than as a capability](#2-what-is-built-stated-as-a-mechanism-rather-than-as-a-capability)
3. [The constraint this document is organised around: no core gate is engaged](#3-the-constraint-this-document-is-organised-around-no-core-gate-is-engaged)
4. [The routes this document takes without a decision](#4-the-routes-this-document-takes-without-a-decision)
5. [What this roadmap does not advance, and the mistake it would be](#5-what-this-roadmap-does-not-advance-and-the-mistake-it-would-be)
6. [The stages](#6-the-stages)
7. [Order, and what is schedulable today](#7-order-and-what-is-schedulable-today)
8. [What this roadmap does not promise](#8-what-this-roadmap-does-not-promise)
9. [What a stage would owe if it were scheduled](#9-what-a-stage-would-owe-if-it-were-scheduled)

---

## 1. What hosting means here, and where it is not the capability channel

**A host object was believed impossible in this profile, the reasoning was sound at every step, and
the conclusion did not follow.** The reasoning ran: a DOM accessor returns an object; a value
capability answers a `long` or an opaque reference; an opaque reference is by construction not
dereferenceable; therefore no registration any composition could make would let a host answer a
guest with an object. Every one of those steps is true. What the conclusion assumes is that the
object has to travel through the **capability channel**, and it does not.

**A host object is an ordinary object in the realm.** Its methods are ordinary `JsNativeFunction`
values, and calling one is `JsEngine.Call` from inside a native body - the same mechanism
`Array.prototype.map` uses to call the function it was handed, and the same one every `JsProxy` trap
already uses. The object model has no idea an embedder built it. That is why it can return an
object, carry a string and call back into the guest synchronously, none of which the capability
channel can express, and it is why nothing about it reaches the core.

**What the capability table still decides is whether any of this exists, and that is the whole
permission.** A realm is handed to an embedder only where the composition registered
`broiler.javascript.host-surface`, a fourth optional import whose handler is never invoked: the
profile asks `IsBound` once, at instantiation, in `JsExecution.Instantiate`, and nothing else in the
profile addresses the slot. So what is registered is the answer to one question asked once - may
this composition's embedder put objects in this runtime's realms - rather than a channel carrying
traffic. **Registration is the permission and not the door**, which is a distinction this component's
records did not previously draw:
[JSD-0024](decisions/0024-the-in-realm-host-surface.md) is the decision that draws it, with the four
alternatives it weighed and did not take, and [JSC-210](roadmap.corrections.md#jsc-210) is the
correction to the non-goal that had been read the other way.

**So "hosting" in this document means exactly one thing**: what an embedder can build inside a realm
it was permitted to reach, and what the profile charges it for doing so. It does not mean widening
the capability channel, it does not mean a new `VmCapabilityKind`, and it does not mean anything a
guest can reach without an embedder. Where a stage below would need a core change it says so and
stops; none of them does.

---

## 2. What is built, stated as a mechanism rather than as a capability

This section is the ground the stages are proposed against. Every sentence in it is checkable
against a named member in this checkout, and none of it is an acceptance claim.

**The values are the wide surface's own layout with the engine's reference replaced by an
identity.** `JsHostValue` is a kind, a `double` and a reference; where the engine's own value would
carry a `JsObject`, this carries a `JsHostRef` naming one. `JsHostRef` has no members at all: an
embedder may hold one, compare two by reference and key a table on one, and there is nothing to
read off it. The realm canonicalises through a `ConditionalWeakTable` keyed on the guest object, so
two reads of one object answer one instance, a wrapper table an embedder keys on it finds what it
stored, and the guest's `===` and the host's `ReferenceEquals` agree. Nothing has to be released:
the table's reference to the ref is a dependent handle rather than a root, so an embedder that drops
a ref makes both collectable and no protocol was needed.

**Every crossing is charged, and it is charged because the boundary charge does not apply.** A
crossing does not reach `VmCapabilityBinding.TryEnter`, so the charge the core applies at a
capability boundary is not applied for the seam. `JsEngine.ChargeHostCrossing` charges `HostCalls`
one unit and then fuel, and every public member of `JsHostRealm` enters through the private `Enter`
that calls it. The widening of what the `HostCalls` dimension counts is
[JSC-211](roadmap.corrections.md#jsc-211): the row now names every crossing into host code rather
than the one mechanism that used to realise it, which is the profile charging more than the core's
definition requires and not less.

**The realm is usable only inside a step, on the guest's own thread.** `BeginStep` records the
managed thread id and raises a depth; `Enter` refuses with `JsHostRefusal.RealmNotCurrent` when the
depth is not positive or the calling thread is not that one. The window is opened in
`JsExecution.RunOnGuestStack` around a run of an entry point and around a drain, and separately
around the embedder's installer in `InstallHostSurface`. It is a depth rather than a flag because a
step can contain a host call containing a guest call containing another host call.

**An abort reaching host code is latched, and `EndStep` answers it rather than throwing it.** A
provider writing `catch (Exception)` around a guest call is ordinary defensive style, and without
the latch it would turn a spent allowance into a completed operation. `Latch` stores the abort and
raises a distinct `JsHostTerminatedException`; catching that clears nothing, because `Enter` refuses
every later crossing while the latch is set, and the native body the seam installs re-raises the
latched abort when a host body reports a termination. `EndStep` **answers** rather than throws
because it runs in a `finally`, and an exception raised from a `finally` replaces whatever was
already propagating - which is precisely the case the latch exists to preserve. The caller re-raises
what it was answered, where nothing else is in flight.

**The exotic object is a subclass and it is base-first.** `JsHostObject` overrides the property
lookup, the name list and the count; the first two consult `base` before the handler and the third
is derived from the second, so a name the object's own storage holds is answered from storage and
the embedder is never asked about it. That ranking is the specification's for named properties and
getting it backwards is
silently wrong rather than loudly wrong: a collection containing an element named `item` would begin
shadowing its own `item()` method while every other use of the object kept working. It is a subclass
rather than a `JsProxy` because a proxy would cost a property read on a handler object plus a call
for every operation, on the path a page uses most.

**A composition reaches all of it through a third descriptor door.**
`JavaScriptProfile.DescriptorHostingRealms` takes the embedder and the surfaces it admits;
`JavaScriptProfile.HostSurfaceCapability` is what the composition registers when it creates the
runtime. **Both are required and they answer different questions** - what would be installed, and
whether this runtime permits installing anything - and a descriptor built with an embedder in a
runtime that registered nothing produces realms with no host object in them, silently and correctly.

**The descriptor declares the capability `ReentrantIntoInvokingRuntime`, and it is the first
descriptor this profile publishes to declare that mode.** A fixture capability in the contract
tests declares it too and landed before it, which is how the mode's admitted arm came to be tested
at all - [section 3](#3-the-constraint-this-document-is-organised-around-no-core-gate-is-engaged) -
but nothing a composition can register had ever declared it. The declaration is not load-bearing
for the mechanism, because nothing here crosses the core and nothing here depends on the core
admitting a re-entry. What it does is refuse to understate what registering permits: a host
object's method calls guest code, and a composition reading the descriptor before it registers is
entitled to know that.

**Two properties the argument above rests on are held by rules rather than by habit, and neither is
visible at a call site.** Rule N20 forbids any source in this profile family from holding a surface
or a realm in static or thread-static state, and holds the host function delegate and the surface
callback to receiving their realm as a parameter. That is the rule that would break by
convenience - threading a realm through several frames to reach a callback is exactly the
simplification a reviewer asks for, and what it would actually build is one composition's embedder
within reach of another composition's realm, in a process that hosts two, underneath the table that
permits it. Rule N21 holds every public member of the realm to entering the crossing charge
**before** it acts, and its rejecting shape is the member that charges after the work rather than
the member that charges nothing: an allowance exhausted after the fact stops the *next* crossing
rather than this one, so a granted bound is exceeded by exactly one crossing every time. Both carry
a witness that has been **watched failing against this checkout and watched passing after revert**,
and both are registered where the evidence script can collect them.

**The checks live in a composition root because nothing else may look**, and each is judged by what
the guest printed rather than by a host-side return, because a host method that was never called
leaves "it succeeded" perfectly true. They are run by:

```
dotnet run --project src/compositions/Broiler.VM.Composition.JavaScript.Cli -c Release -- --host-surface
```

They cover a host method answering an object whose string property the guest reads; wrapper identity
in both directions, so the guest's strict equality of two reads holds and the host recognises the
value it handed out; an accessor running host code on every read; a host method calling a guest
function synchronously and returning after it, with the printed order fixed; a listener's
`RangeError` arriving in the guest's own `catch`; a host-raised `TypeError` satisfying an
`instanceof` test; an exotic object answering a name it was never given while an ordinary property
of the same name still wins; and a composition that registered no permission seeing the host global
as `undefined`.

**One of them found a real defect on its first run**, which is the entire argument for judging on
printed output. `DefineValue` installed its property with the built-in attribute set - writable and
configurable, deliberately not enumerable, because a `for...in` must not walk `Object.prototype`'s
methods - so `Object.keys` on a host object answered without the members the embedder had just
installed. The interface-description language a DOM-shaped embedder implements says its members are
enumerable, writable and configurable, so that is the set it installs now: the same one an ordinary
guest assignment produces.

---

## 3. The constraint this document is organised around: no core gate is engaged

**The reason a seam was possible at all is that none of the core's gates is reached, and "not
reached" is a stronger statement than "relaxed".** `VmRuntime.TryBeginCall`,
`VmRuntime.EnterCapability` and `VmInstanceImplementation.TryAdmit` are never entered by a crossing
of this seam. **No rule of ADR 0004 is relaxed** - the rules are not relaxed, they are not
**engaged** - and that distinction is the difference between a design and an exemption.

**It matters because lifting the instance gate would have corrupted several things at once, and each
is a fact of this checkout rather than a fear.** `VmExecutionScope.Enter` and `Leave` are not
nestable and have no restore, so after an inner call returned the outer step would find its ambient
meter cleared. `VmArtifactLoadMediator.EnterScope` zeroes its fan-out, its byte total and its
verifier work whenever it is handed a different operation id, and never restores them, which makes a
nested operation a bound-evasion primitive rather than a nuisance. An inner operation gets its own
meter with its own stopwatch committing to the same runtime level and the same aggregate parent, so
the nested interval would be billed twice. No operation holds a parent reference, so cancelling the
outer would never reach the inner. And ADR 0004 says in its own words that a profile may **tighten**
the reentrancy rules and may never relax them.

**Reentrancy was measured rather than read, and the measurement changed what this component believed
about it.** `VmCapabilityBinding.TryEnter` hands the descriptor's own declared mode to
`VmRuntime.EnterCapability`, which raises the in-capability depth **only for `NonReentrant`**, and
`TryBeginCall` refuses on that depth. ADR 0011's field F5 says the refusal applies where the
capability declared `NonReentrant`. Nothing in the descriptor, the catalog or runtime validation
refuses a re-entrant **value** capability; the only pairing `VmHostCapabilityDescriptor.IsWellFormed`
refuses is an artifact provider that also declares reentrancy. Two tests in
`src/tests/Broiler.VM.Contract.Tests/ConcurrencyTests.cs` pin both halves against a fixture
capability that declares `ReentrantIntoInvokingRuntime`: a nested `Verify` from inside such a call
**completes**, and its control - the same call with the declaration flipped - answers
`ReentrantRuntimeCallFromCapability`, which is a wrong answer rather than an exception. The
complementary limit is pinned by the second: `VmInstanceImplementation.TryAdmit` refuses an
`Executing` instance with `ReentrancyRefused` and never reads the declaration at all.

**So the honest statement of the boundary is two sentences, and a stage below that blurred them
would be proposing something else.** A re-entrant host capability may re-enter its **runtime**, and
that is now demonstrated rather than assumed. It may not re-enter the **instance whose step it is
inside**, and no declaration changes that. The seam does not need the second and does not have it:
a host body reached from guest code calls back into the guest through `JsEngine.Call`, which is a
call inside the interpreter and not an `Invoke` on the instance.

**And nothing here is a core amendment.** Nothing in `Broiler.VM.Abstractions` or
`Broiler.VM.Runtime` changed for the seam, and the core's public API baseline is untouched, which is
the mechanical statement that no core contract surface moved. ADR 0003's *Not an amendment* clause
covers implementing something version 1 already admits. Minting a version needs role R5 and a
co-signature the amendment procedure requires; the core's own record states that procedure is
unexecutable while one person holds the minting role and both co-signing roles, and
[`docs/mvp.md`](../../../docs/mvp.md) section 2.4 records the co-signing step as deferred. **No stage
below asks for a version to be minted**, and a stage that did would be blocked on that and would say
so.

---

## 4. The routes this document takes without a decision

**A deferred decision is a decision nobody took, not a decision that went a particular way**, and
where a route is taken in place of a decision the route is named where its consequence is.
[JSD-0024](decisions/0024-the-in-realm-host-surface.md) weighed four alternatives and took one, and
every one of them is a question about **where the traffic goes** - a capability trampoline, a lifted
instance gate, the suspension channel, a second runtime. **None of the four is a question about the
shape of the seam itself**, and the four below are, which is why they are routes here rather than
options there. Each is recorded as **taken without a decision**:

- **That the seam is a set of realm members rather than a proxy handler an embedder implements.** The
  alternative is real: one interface with the object model's own internal methods on it, implemented
  by the embedder, with `JsProxy` doing the dispatch. It would express every operation
  [JSH-2](#jsh-2---the-property-operations-the-seam-still-lacks) has to add member by member, and it
  would put the cost of a property read on a handler object plus a call on the path a page uses most.
  This document proposes the members because the built seam has them, and **no decision record has
  weighed the two.**
- **That an embedder installs values and never types.** Nothing an embedder puts in a realm is a CLR
  type the guest can name, which is what the profile's CLR-interop non-goal has always said and what
  [JSC-210](roadmap.corrections.md#jsc-210) leaves untouched. The consequence is paid at every stage
  below: a host object is built by calling mint-and-define members rather than by describing a class,
  so an interface with many members is many crossings and therefore many charges. A description-driven
  route - the embedder hands over a table and the profile installs it - would pay one charge and would
  need the profile to own a description language. **Nobody has chosen between them.**
- **That the realm is single-threaded and step-scoped.** The gate refuses a crossing on any thread
  but the one that opened the step. That is what makes the charge land on the operation that caused
  it, and it is also what forecloses an embedder holding a realm and calling into it from its own
  event loop. [JSH-5](#jsh-5---one-guest-thread-per-instance) and
  [JSH-7](#jsh-7---a-second-realm-and-what-may-cross-between-two) are where that route's cost is,
  and **it was taken because the meter is ambient and not because anybody compared it to the
  alternative.**
- **That the exotic object's named property is not writable.** `JsHostObject` installs what the
  handler answered as enumerable and configurable and **not** writable, so a guest assignment creates
  an ordinary own property that shadows the handler from then on. That is a defensible reading - a
  named property is the embedder's to answer, and a write through it would land in storage the
  embedder does not read - and it is not the only one: a live style declaration is a thing pages
  assign to. JSH-2 is where the other reading would be built, and **the current behaviour is a route
  rather than a ruling.**

---

## 5. What this roadmap does not advance, and the mistake it would be

**Nothing below makes this profile run more JavaScript, and conflating the two is how a component
ends up embeddable in a language nobody can write.** A seam carries values the engine already has
between an embedder and a realm; a construct the front end refuses never becomes a value, so there
is nothing for a seam to carry. **The programmes divide by question and neither re-mints the
other's stages**:

- The construct families still refused and the surfaces still absent from the realm are
  [the workload roadmap's](roadmap.workloads.md) -
  [`JSW-5`](roadmap.workloads.md#jsw-5--the-core-language-surface-still-refused-by-name) for the
  language surface refused by name and
  [`JSW-6`](roadmap.workloads.md#jsw-6--the-core-library-still-absent-from-the-realm) for the library
  still absent.
- **The guest-side job queue is [`JSW-7`](roadmap.workloads.md#jsw-7--settling-the-job-queue)'s and
  not this document's.** [JSH-3](#jsh-3---jobs-and-promises-on-the-seam) is the seam's *view* of that
  queue, and the queue itself, its drain point and its charge belong to that stage.
- The module goal is [`JSW-8`](roadmap.workloads.md#jsw-8--the-module-goal)'s, and
  [JSH-4](#jsh-4---host-script-source-the-embedder-authored-and-source-the-guest-asked-for) is
  careful to be about *script* rather than about modules for exactly that reason.
- The host surface an embedder meets first - defaults, ceilings, feature detection, refusal reasons
  that are true of the realm they are raised in - is
  [`JSP-10`](roadmap.parity.md#jsp-10--the-host-surface-an-embedder-meets-first)'s. That stage is
  about what an embedder finds; this document is about what an embedder may build.
- A second output form for what already compiles is [the backend roadmap's](roadmap.backends.md),
  and it and this document share no stage and no obligation.

**The mistake is specific.** A seam is demonstrated by writing an embedder, and the embedder that is
easy to write is the one that only needs what the seam already has. A programme that grows the seam
towards its own demonstration will keep choosing work whose value is visible in a check it also
wrote. **Every stage below therefore states what would judge it in terms of what a GUEST prints**,
because the guest is the only party that cannot be persuaded by the shape of the seam.

---

## 6. The stages

Each stage states what it delivers, what it costs, and what would judge it, with the gate written
the way [section 19's](roadmap.delivery.md#19-milestones) gates are written - as conditions a run
can decide, not as work items. **Only the first owns code**, and it carries a State bullet saying
which of its own clauses that code meets and which it does not. **None of the rest is scheduled and
none has an owner**; assigning either is the act that would turn a stage into a milestone with a
ledger row, and the ledger carries no row that any of them would move.

### JSH-1 - The seam: values, identity, a step bracket, and an abort that cannot be swallowed

- **Delivers.** The value type and its kinds, including a `Missing` that is not `undefined` so a
  method resolving on arity can tell a call with no argument from a call with an explicit one; the
  canonicalised identity and its weak table; the step bracket and its thread and depth gate; the
  abort latch and its answer-rather-than-throw discipline; the mint members for an object, an array,
  a non-constructable method, a constructor and an exotic object; `DefineValue`, `DefineAccessor`,
  `GetProperty`, `SetProperty`, `HasProperty` and `OwnPropertyNames`; `Invoke` and `Construct` into
  the guest; `ToJsString` and `ToNumber` as realm members rather than value members, because both can
  run guest code and a conversion that can execute guest code should not read like a field access;
  the error mint that answers a throwable rather than throwing, so a host body reads
  `throw realm.Error(kind, message)`; the third descriptor door; and the checks.
- **Costs.** Two weak tables and a handful of fields per realm that ever had an embedder, built
  lazily so a composition that registered no host surface pays for none of it. A charge on every
  crossing, which is the point rather than the price. And a public surface on a profile assembly,
  which is a thing the profile family's API baseline now carries and a thing that cannot be narrowed
  quietly later.
- **Judged by.** A lane whose every check compares printed guest output, including one composition
  that registers no permission and must see no host object, because a permission that is not checked
  in the negative is a permission nobody has tested.

- **State: built, and observed passing on this working tree on 2026-09-09.** The lane's checks all
  pass under the command in [section 2](#2-what-is-built-stated-as-a-mechanism-rather-than-as-a-capability),
  and rules N20 and N21 hold the two properties the seam's argument rests on with witnesses watched
  failing and watched passing after revert. **Open clauses, and they are stated rather than left to
  be discovered.** No bundle retains any of it and no human has read a line, so every sentence above
  is a fact about one working tree on one machine on one date, and a green suite is not a bundle.
  `JsHostRefusal.HostReentryTooDeep` is declared in the public enum and is **raised nowhere**, so a
  member of a published surface names a condition this build cannot produce - which is either a
  refusal somebody has to write or a member somebody has to withdraw, and doing neither is what
  leaves a reader planning against a guarantee. And `JsHostRealm.OwnPropertyNames` describes itself
  as answering the object's own **enumerable** string keys in creation order, while
  `JsObject.OwnPropertyNames` answers every own string key, enumerable or not, with index keys first
  in ascending numeric order and the rest after them - so the member's own summary is wrong about
  both the filter and the order, which is the kind of defect that produces an embedder built on a
  promise the code never made.

### JSH-2 - The property operations the seam still lacks

- **Delivers.** `DefineIndex` and `GetIndex`; `DeleteProperty`; `GetPrototype` and `SetPrototype`;
  own keys including symbol-keyed ones; and a writable path for an exotic named property. **The
  point of the stage is that a DOM-shaped object model needs all of them and the seam has none**: a
  live collection is indexed, a storage area is deleted from, a wrapper's prototype is what makes
  `instanceof` answer, and a style declaration is assigned to.
- **What the object model already supports, so the seam is a wrapper rather than engine work.**
  `JsEngine.GetIndexed` exists and takes a value key, so `GetIndex` is a projection over it.
  `JsObject.DeleteOwnProperty` is virtual and is what the interpreter's own `DeleteProperty` arm
  calls, so the only new decision is which answer the seam gives where the object refused - the
  interpreter adds the strict-mode throw above the object model, and the seam has to choose the same
  or say why not. An index **is** a string key in this object model - `JsObject.IsArrayIndex` reads
  one back out of a key and `JsArray.SetOwnProperty` maintains `length` from it - so `DefineIndex` is
  `DefineValue` over a canonical index string and nothing more. `JsObject.Prototype` is a virtual
  get and set, so `GetPrototype` is a projection. `JsObject.OwnKeys` is virtual and answers string
  keys then symbol keys, never interleaved, and `TryGetOwnSymbol`, `SetOwnSymbol`, `DeleteOwnSymbol`
  and `OwnSymbolKeys` are all there beside it.
- **What needs work inside the profile, stated separately because the two costs are not alike.**
  `SetPrototype` must not be the virtual setter: the specification's `OrdinarySetPrototypeOf`, with
  its extensibility test and its cycle check, is `JsRealm.Object.cs`'s `ObjectSetPrototype`, and it is
  `private static`. **The cycle check is the load-bearing half and not politeness** - every property
  lookup walks the chain with a plain loop, so a chain closed on itself is an unkillable spin rather
  than a wrong answer - and a seam member that assigned the virtual property directly would hand an
  embedder that spin. Exposing it is a visibility change plus a ruling on the proxy branch, which
  throws where a trap refused. **Symbol keys are the sharper gap**: `Wrap` already answers a
  `Symbol`-kinded value, so a host can *hold* a symbol it read off the realm, and every keyed member
  of the seam takes a `string name`, so a host **cannot use one as a key**; the realm's well-known
  symbols are internal properties of `JsRealm` and the seam names none of them, and there is no mint
  for a fresh one. **And the writable exotic property needs an interface member that does not
  exist**: `IJsHostExotic` answers `TryGetNamed` and `SupportedNames` and nothing else, and
  `JsHostObject` installs what the handler answered as not writable, so a guest assignment shadows
  the handler permanently. A writable path needs a set member on the interface, an override of the
  base's write path, and a ruling on what a delete of a named property means.
- **Costs.** Every member added is a crossing, and rule N21 holds it to entering its charge **before
  it acts**, so each is a charge somebody has to size - which is JSH-6's subject and not this
  stage's, and which this stage therefore adds to. Every override added is a place the base-first
  ordering can be got backwards, which is the failure this seam's one exotic subclass already
  carries a falsification line about. And the public surface grows again, on a profile assembly
  whose baseline is regenerated rather than reviewed.
- **Judged by.** Guest-printed checks in the same lane: an indexed read and an indexed define
  answering through the guest's own subscript; a delete of a non-configurable property answering
  `false` in sloppy code and throwing in strict code, both printed; a prototype set that makes
  `instanceof` answer true, and a prototype cycle **refused rather than hung**, with the refusing
  case run under a bounded allowance so a regression is a failure rather than a hang; `Object.keys`,
  `Object.getOwnPropertySymbols` and `Reflect.ownKeys` each agreeing with what the seam answered for
  the same object; and an assignment to a writable named property read back through the handler
  rather than through a shadow.

### JSH-3 - Jobs and promises on the seam

- **Delivers.** Enqueueing a host job, asking whether any job is pending, and a promise an embedder
  can settle. **A DOM-shaped embedder cannot do without the third**: `fetch`, a definition-ready
  signal and a streams polyfill all hand a page a deferred result, and an embedder with no settleable
  promise has nothing to hand back.
- **What the engine already has, all of it internal.** `JsEngine.EnqueueJob` puts a callable on the
  queue and charges for it, reporting retention as well, so a program that enqueues without bound
  spends its own allowance rather than the host's memory. `HasPendingJobs` and `PendingJobCount`
  answer the question. `DrainJobs` runs the queue to exhaustion and `StepOneJob` runs exactly one,
  which is the member an embedder with an event loop of its own needs, and `DropPendingJobs` is the
  terminal unwind's whole of the work. `JsRealm.Promise.cs`'s `PromiseResolvers` mints the resolving
  pair with its latch, and `PromiseSchedule` is the single choke point through which every reaction
  reaches the queue.
- **What an embedder can already do without this stage, which is worth stating because it bounds the
  stage's value.** The seam can read `Promise` off the global with `GetProperty`, build an executor
  with `NewMethod`, `Construct` the promise with it, and capture the resolve and reject functions as
  values to `Invoke` later. That **is** a host-settleable promise today, written by the embedder. What
  no embedder can reach is the queue itself: the drain is reached by the host invoking the reserved
  entry point `#drain-jobs` through the core's own invocation path, and whether anything is pending
  is an internal question. So the honest delivery of this stage is *a first-class pair and a queue
  question*, not *the first promise*.
- **The ordering obligation, and it is the clause a stage here is most likely to break.** A promise
  settled from host code **must not run its reactions until the next turn**. That property holds
  today by construction rather than by a check: `PromiseSchedule` only ever enqueues, and nothing
  drains implicitly, so a reaction runs when the host asks and not before. A seam member that settled
  a promise **and drained the queue** - which is the convenient shape, because it makes a host's
  `resolve` look like it worked - would run guest code inside the host's own frame at a point no
  program could have predicted, and would be a different execution order from the one every other
  path in this profile produces. **The stage's first obligation is that its settle member does not
  drain.**
- **Costs.** More crossings and their charges. A public surface over a queue whose drain point
  is the host's decision, which means the seam has to expose the question without exposing the
  decision. And a widening of what an embedder can keep alive across invocations, since a pending job
  is retention the meter already reports and a host-enqueued one is retention nobody asked the guest
  about.
- **Judged by.** A guest that prints a line after registering a reaction, a host that settles the
  promise inside the same step, and a printed order in which the synchronous line comes first and the
  reaction comes after the next drain - the same order an ordinary guest settle produces, asserted
  side by side rather than separately. A host-enqueued job that throws not stopping the stepping. And
  a program whose jobs enqueue jobs without bound ending by naming its spent dimension rather than
  hanging, which is the answer the queue already gives a guest and must give a host.

### JSH-4 - Host script: source the embedder authored and source the guest asked for

- **Delivers.** A way for an embedder to evaluate source **it** wrote, distinguishable at the
  provider from source the **guest** asked for.
- **Why the distinction is the whole stage.** A content policy forbids the second and does not forbid
  the first. An embedder's own polyfills, its own bindings and its own bootstrap are host script; a
  page's `eval` and `new Function` are guest evaluation; and a realm built for a page whose policy
  forbids evaluation must run the first and refuse the second. **On the surface as it stands the two
  are the same request and a provider cannot tell them apart.**
- **What exists, read before proposing anything.** The seam cannot evaluate source at all - no member
  of `JsHostRealm` compiles anything. The profile's only route from a string to code is
  `JsEngine.Evaluate`, which refuses a direct `eval` inside a function by name, refuses when no
  provider is registered, charges proportionally to the source, and then asks the mediator. **The
  request payload is the source text, UTF-8, and nothing else**: the request carries the profile's
  identity, a nesting depth of one, the cancellation token and the bytes, and every other field is
  default. `SourceProviderCapability`'s own remarks say so and say why - the core carries the payload
  without decoding it, and a provider that wanted to know whether it was a direct `eval` cannot be
  told, because the answer would not change what it may compile.
- **The cost, and it is the sharp one.** The distinction has to be a property of the **request**, and
  the request's payload is the source and nothing else. So a stage here chooses between two routes and
  the choice is not this document's to make. Either the profile widens what it puts in a request,
  which is a decision about a core-shaped payload that must not be made quietly and which the
  provider capability's own remarks currently argue against; **or the composition registers a second
  provider identity for host script and the two are told apart by which binding was invoked**, which
  needs no core change at all, keeps the payload's meaning exactly as it is, and makes the policy
  decision a registration rather than a flag. **The second route is the one to weigh first**, and it
  costs a capability identity, a binding index and a descriptor row.
- **Costs beyond that.** A crossing whose charge must grow with the source, as `Evaluate`'s already
  does, because a flat charge over source an embedder supplies is a charge an embedder can dilute.
  And a second way into the same realm, which is a second place the artifact-load bound has to hold.
- **Judged by.** One composition registering a provider that admits host source and refuses guest
  source, in one realm: a host script that installs a binding and runs, and a guest `eval` of the
  same text refused, both printed by the guest. And a composition registering neither, where both
  refuse, so the absence is an absence rather than a silent success.

### JSH-5 - One guest thread per instance

- **Delivers.** An instance that runs its invocations on one thread rather than on a fresh thread
  each time.
- **What exists, and its own argument for itself.** `JsExecution.RunOnGuestStack` creates a thread per
  invocation with a large reserved stack, and the file's remarks argue the choice: a thread's stack is
  reserved address space committed a page at a time, so a program that never recurses pays for none of
  it; an instance can outlive many invocations and a thread parked between them is a resource held
  while doing nothing; and a fresh thread starts every invocation with the same stack whatever the
  previous one did. **Those are good reasons and this stage does not dispute them.**
- **The trade, stated honestly.** What a thread per invocation costs is a thread creation per
  invocation, and **the shape that makes that cost matter is exactly the shape this document is
  about**: an embedder dispatching many small events crosses into the guest constantly, and each
  crossing that begins at the core is an invocation. Against that, a per-instance thread holds
  reserved address space for the instance's whole life, including while it does nothing, and it does
  not reset the stack between invocations - so a program that nearly overran on one invocation begins
  the next in whatever state the previous left. Neither of those is a small consideration and this
  stage claims no answer, only that the question is now worth asking.
- **The obligation that makes it more than a thread cache, and it is the reason this stage is not
  cheap.** `VmExecutionScope` holds its meter and its operation in `AsyncLocal`, and a thread captures
  the creating thread's `ExecutionContext` when it is started. **A thread made once and reused across
  invocations therefore carries the first invocation's context for ever**, so the ambient meter would
  resolve to the first invocation's meter and every later invocation's charges would land on it. A
  per-instance thread has to capture the `ExecutionContext` per invocation and run the body inside it;
  there is no version of this stage that is only a thread-pool change, and a stage that treated it as
  one would produce a runtime that bills correctly in every test with one invocation in it.
- **And a second obligation, on the seam's own gate.** `BeginStep` records the current managed thread
  id and `EndStep` clears it at depth zero. A per-instance thread keeps one id across invocations,
  which makes the thread half of the gate weaker rather than wrong - the depth half still closes the
  window, so a realm touched between invocations is still refused. A stage doing this **owes a case
  that a realm touched between two invocations, on the very thread that ran the first, is still
  refused by name**, because that is the case the thread identity used to catch on its own.
- **Judged by.** A sequence of invocations on one instance whose charges land on the invocation that
  caused them, asserted per invocation rather than in aggregate, with a negative control that removes
  the per-invocation context capture and is **watched failing and watched passing after revert**. The
  gate case above. And a deep-recursion case on the second and later invocations of one instance,
  because a stack that is not reset is a stack whose first invocation's depth is not the second's.

### JSH-6 - Proportional charging, measured rather than chosen

- **Delivers.** Fuel amounts per crossing that were derived from a measurement, and a retained record
  of that measurement.
- **What is true today, stated exactly.** Every crossing charges `HostCalls` one unit, which is not
  in question. The **fuel** is a small integer chosen at each call site, and the choice was made by
  reading rather than by measuring. Some crossings already grow with what they carry - `Invoke` and
  `Construct` grow with the argument count, `NewArray` with the element count, and `ToJsString` and
  `OwnPropertyNames` charge through the engine's text charge, which grows with the length it was
  handed. **Some do not.** A property name is a string an embedder or a guest chooses, and
  `DefineValue`, `GetProperty`, `SetProperty` and `HasProperty` charge a constant for one however
  long it is. **A flat charge over a guest-controlled quantity is a charge a guest can dilute**, and
  that sentence is written in the seam's own source as the reason the growing charges grow; it is as
  true of the ones that do not.
- **What the stage would measure, and it is a ratio rather than a duration.** What a crossing costs
  **relative to an interpreted instruction**, because fuel is what an instruction is charged in and a
  crossing charged in unrelated units is a crossing whose price is arbitrary. Absolute timings are not
  the subject and would not survive this document's own rule about figures.
- **The shapes it must cover, because one ratio over one shape is a ratio for that shape.** A
  crossing that only allocates in the realm; one that copies text across the boundary; one that calls
  back into the guest and therefore pays for whatever the guest does; one that runs a host accessor
  from inside a guest property read, which is the path a page uses most; and one that mints a
  function, which allocates more than it looks. **And the negative shape**: a crossing whose charge
  does not grow, driven over a quantity a guest controls, to show what dilution actually buys before
  the charge is changed.
- **Costs.** A lane somebody runs and a bundle somebody retains, which is the whole cost, and it is
  larger than it sounds because this component has no measurement lane for the seam and
  [update rule 10](roadmap.status.md#5-update-rules) makes a figure with no retained record behind it
  unstatable. **A stage that measured and retained nothing would have produced nothing this document
  family may say.**
- **Judged by.** A retained record in the form the ledger's evidence section asks for, with its
  failures and exclusions retained rather than summarised; the constants in the source derived from it
  rather than beside it, so a reader can see which measurement each came from; and a case that drives
  a diluting shape and observes the allowance spent where the old constants would have let it run.

### JSH-7 - A second realm, and what may cross between two

- **Delivers.** More than one realm in one process, and a stated rule for what may pass between them.
- **What does not exist today, said plainly rather than implied.** One realm per engine, constructed
  in the engine's own constructor and nowhere else; **nothing in this profile creates a second**.
  There is no structured clone anywhere in the tree. `SharedArrayBuffer` and `Atomics` are absent,
  and absent **deliberately rather than incidentally**: the binary manifest's own remarks say they
  are the multi-agent surface and need an agent model this profile does not have, and that folding
  them in would let a composition asking for an ordinary byte buffer admit cross-agent shared memory
  by accident. The step bracket pins a realm to one thread for the duration of a step. **So there is
  no worker story, and a reader who needs one needs this stage and the stage does not exist.**
- **What does exist, and it is the beginning of the answer rather than a coincidence.** `JsHostRef`
  carries the realm that minted it, and `Unwrap` refuses a ref from another realm by name with
  `JsHostRefusal.ForeignRealm`. **That refusal is written for a case that cannot yet arise**, which is
  the right order to write it in: a value from one realm silently accepted by another would corrupt
  both, and the check that stops it is cheaper to write before there are two than after.
- **What crossing would mean, and the line is drawn by the value's own layout.** A value is a kind, a
  double and a reference, and **only the reference is per-realm**. Strings and numbers cross by
  construction - the seam's string mint deliberately takes no realm, because a string is not a realm's
  object and nothing about it has to be canonicalised, which is what lets an embedder build an
  argument list before it has chosen a realm. **Objects do not cross and must not.** What a clone
  produces is a new object in the receiving realm, so identity is not preserved and a stage that let a
  ref cross would have made `ForeignRealm` a lie rather than a guarantee.
- **Costs.** A second realm is a second engine and a second meter, and the core charges per operation,
  so **which operation a second realm's work bills to is the first question and not the last** - an
  answer that let one operation's realm charge another's meter would be the bound-evasion
  [section 3](#3-the-constraint-this-document-is-organised-around-no-core-gate-is-engaged) is written
  about. A clone is a graph walk over guest data and is therefore a charge proportional to what it
  copies, with a cycle rule and a refusal for what cannot be cloned. And two realms on two threads
  make the seam's thread gate a per-realm question rather than a per-process one.
- **Judged by.** A value sent to a realm on another thread and an answer received, printed by both
  guests. A ref minted by one realm and presented to the other refused by name rather than resolved.
  **Identity not surviving a clone, asserted rather than assumed**, because an embedder that
  discovered that property by testing would have built a wrapper table on it. And a charge landing on
  the operation that owns the realm that did the work, with a case that would catch it landing on the
  other.

### JSH-8 - The flag ledger: which declaration each stage would make honest

- **Delivers.** A reading, not a mechanism: which embedder-facing capability flag could honestly be
  declared after which stage.
- **The frame, and it is the part to get right.** **This repository does not own those flags.** They
  belong to the JavaScript engine abstraction in the browser repository, they are declared by a
  provider rather than discovered from an engine, and **the mapping from a stage here to a flag there
  is the embedder's to make**. What this document can honestly do is name the mechanism each flag
  describes and say which stage would supply it. Declaring one is an act in another repository,
  against a provider that does not exist, and **nothing in this document licenses that act.**
- **The reading.**

| Flag, as that layer defines it | The mechanism it names | Which stage supplies it | What is still missing |
|---|---|---|---|
| A host function may call back into the guest from inside a host frame | `Invoke` routing through `JsEngine.Call` from a native body | JSH-1 | Nothing mechanically. That layer's own record says this is the flag that decides whether an engine can host a document at all, and states that this repository does not have it - a statement written against the **capability channel**, at a pinned commit, and true of that channel still. Whether it now reads differently is that repository's to decide and not this document's to announce. |
| Exotic objects whose property lookup the host defines | `IJsHostExotic` and `JsHostObject`, base-first | JSH-1 for the read path, JSH-2 for the rest | Index, delete and a writable named property, which are JSH-2's whole subject |
| The global object doubles as the realm's variable scope | A top-level `var` or function declaration becoming a property of the global object | **No stage. It is already true of this profile** | Nothing. A stage claiming it would be claiming credit for behaviour that predates the seam |
| Promises exposed to the host: created pending, settled from host code | The engine's queue and the realm's resolving pair | JSH-3 | The queue question and a first-class settleable pair; an embedder can already build one out of `Construct`, which bounds what the stage adds |
| Runs JavaScript the embedder authored | Compilation of host-authored source, distinguishable from guest-asked-for source | JSH-4 | The whole stage, and the routing decision inside it |
| Runs JavaScript the page supplied | `JsEngine.Evaluate` through the source-provider capability | **No stage. It is already reachable** | Nothing from the seam. A composition that registers a provider has it and one that does not has an `eval` that refuses, which is a registration question rather than a hosting one |
| Static module binding, and dynamic import through a host module map | The module goal, and the resolve capability's ruling on a specifier the composition already resolved | **No stage here.** [`JSW-8`](roadmap.workloads.md#jsw-8--the-module-goal) owns the goal | Everything, and none of it is this seam's |
| A second realm on another thread with values moved by structured clone | Two realms and a clone | JSH-7 | The second realm, the clone, and the charging answer - and the clone does not exist in this tree at all |
| The composite a document-bearing page load needs | The conjunction of several of the above | The conjunction | It is last by construction and is not a stage of its own |

- **Costs.** None in code. The cost is a discipline: **a flag declared from a mechanism that exists is
  honest, and a flag declared from a stage that is written down is not.** A written stage is not a
  mechanism, and this table's only value is that it is hard to misread in that direction.
- **Judged by.** Nothing here, and that is the point. This stage is judged in the other repository, by
  a conformance suite run against a provider, and a row above that turned out to be wrong would be
  wrong there rather than here. What this document owes is that each row names a mechanism a reader
  can find in this checkout, or says that the mechanism is absent.

---

## 7. Order, and what is schedulable today

**Two stages need nothing that does not exist, and they are the two most likely to be passed over.**
JSH-2 is members over virtuals the object model already has, plus one visibility change and one
interface member; nothing in it waits on a decision, a machine or a core change. JSH-6 needs no
mechanism at all - a lane and a retained record - and it is the stage that makes every charge in the
seam something other than a guess. **The sibling document records exactly this failure against
itself**: [the backend roadmap's](roadmap.backends.md) first stage needed nothing that did not exist
and was skipped outright, and its own section 7 says that an ordering with no gate behind it is a
preference. That is the risk here and it is named rather than assumed away.

**JSH-2 comes first among the rest, and not because it is small.** An embedder building a
document-shaped object model reaches for an index, a delete and a prototype in its first hour, and a
seam that answers a method call and cannot answer `collection[0]` is a seam that will be worked
around rather than extended. Working around it means an embedder installing ordinary properties where
a live answer was wanted, which is a correctness defect in the embedder that this profile's records
would have caused.

**JSH-3 needs nothing mechanically and needs one decision that is not this document's.** The queue
and the resolvers exist; what a seam member may do about the drain point is bound by
[`JSW-7`](roadmap.workloads.md#jsw-7--settling-the-job-queue), which says the host decides when jobs
run and states the point. A settle member that drained would have taken that decision by
implementation, which is the thing that stage exists to prevent.

**JSH-4 waits on a route rather than on work.** Both routes it names are buildable today; the reason
it is not schedulable is that choosing between them decides whether a request payload's meaning
changes, and the capability's own remarks argue against changing it. **That is a decision with a
record to write, not a task with an estimate.**

**JSH-5 and JSH-7 are the expensive ones and they are ordered last for different reasons.** JSH-5 is
expensive because its obvious form is wrong: a reused thread that does not re-capture the execution
context bills every invocation to the first, and every test with one invocation in it passes. JSH-7 is
expensive because it multiplies the meter question, and the meter question is the one
[section 3](#3-the-constraint-this-document-is-organised-around-no-core-gate-is-engaged) says the
core's own gates exist to answer.

**JSH-8 is last because it is a reading of the rest**, and a reading written before the rest exists
would be a table of intentions.

**Nothing here waits on the snapshot, and nothing here waits on a core amendment - with one
qualification stated rather than glossed.** `JS-2` is `Blocked` in the ledger and stays blocked. No
stage above needs a rule relaxed or a gate lifted, which is the property
[section 3](#3-the-constraint-this-document-is-organised-around-no-core-gate-is-engaged) is written
to establish and the reason this programme could be proposed at all. **The qualification is JSH-4's
first route**: widening what a profile puts in a load request is a question about a core-shaped
payload, and whether it reaches the amendment procedure at all is for whoever weighs the two routes
to say. **The second route reaches it nowhere**, which is the strongest of the several reasons that
stage names for weighing it first.

---

## 8. What this roadmap does not promise

Each of these is a rule rather than a scheduling note, and a stage that needed one relaxed would be a
different programme rather than a later one:

- **No provider for the browser's engine abstraction.** That is work in a different repository,
  against contracts this repository does not own. Nothing here builds one, schedules one, or declares
  a capability flag on one's behalf.
- **No relaxation of a core gate, and none engaged.** `VmInstanceImplementation.TryAdmit` still
  refuses a nested invocation with `ReentrancyRefused` and never reads a declaration, so a host may
  not call `Invoke` on the instance whose step it is inside. **The seam does not need to and does
  not**, and no stage above proposes changing that.
- **No CLR type reaches the guest.** What an embedder installs is this profile's own values. Nothing
  resolves a type by name, constructs a generic type, or enumerates members, and no stage above
  widens that by a member.
- **No structured clone, no second realm today, no module loader on the seam, and no worker.** JSH-7
  is where the first two would be proposed and it is a proposal; the module loader belongs to the
  workload roadmap and is named nowhere in this seam; and there is no worker story because there is
  no second realm.
- **No promise or job surface until JSH-3, and no host script until JSH-4.** An embedder reading this
  document today has neither, and the paragraphs above that say what an embedder can build out of
  `Construct` are about what the seam permits rather than what it provides.
- **No claim about what a crossing costs.** JSH-6 is the stage that would produce one, and it produces
  a retained record or it produces nothing this document family may state. **A record may say the seam
  charges; it may not say what the charge is worth.**
- **No capability flag declared anywhere.** JSH-8 is a reading and its own last clause says so.
- **It does not advance the language.** No stage above admits a construct, fills an absence in the
  realm, or widens a manifest. Section 5 says whose that work is.
- **And it does not accept anything.** No stage moves a ledger row, and a stage's gate being met is
  not acceptance: acceptance needs an owner and a reviewer decision, which nothing in this component
  has.

---

## 9. What a stage would owe if it were scheduled

Nothing in this document changes the obligations any work in this component already carries, and they
are restated here only because a stage list invites the reading that a plan replaces them:

- A retained bundle per [section 4's](roadmap.status.md#4-required-evidence-bundle) nine fields,
  collected by this profile's own script, with its failures and exclusions retained rather than
  summarised - and, for anything on this list, with the negative-controls field carrying more than
  one entry, because every stage above is defined by what it can be seen to refuse.
- A registered rule in the architecture rule register for every new mechanism, with a witness whose
  file name starts with the rule identifier, and negative controls that have been **watched failing
  and watched passing after revert**. A control nobody has seen fail is worth nothing. The seam met
  this for the two properties its own argument rests on and met it in a later change than the one
  that built the mechanism, which is weaker than doing it first and is worth saying rather than
  rounding up.
- The two-line assurance annotation on every new declaration, the falsification line on anything
  assessed at the top of the security scale - which the members of this seam that carry the gate,
  the latch and the identity all are, because a crossing that skipped its charge or its gate would
  be work nobody paid for - and no preprocessor directive in any covered source.
- Appended entries in [the corrections file](roadmap.corrections.md), never edits to old ones, with
  the plan pointing at them by bare marker, and the decision record a correction cites present in
  the decisions directory rather than only linked.
- Ledger rows moved in the same change that changes what they claim, and no count, total or score
  copied into prose anywhere.
