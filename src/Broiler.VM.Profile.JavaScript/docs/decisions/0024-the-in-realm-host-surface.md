<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0024 - The in-realm host surface, and the capability table as permission rather than channel

**Status:** Taken. 2026-09-09.

**Owner:** MaiRat. **Co-signer:** none. **Both roles are held by one person**, and this record does
not claim the co-signature is independent - there is no second signature to claim it of.

**Milestone:** JS-5, which [the chapter, milestone and gate
map](../roadmap.delivery.md#25-the-chapter-milestone-and-gate-map) names as the milestone that owns
[section 13](../roadmap.md#13-realms-agents-and-the-host-boundary)'s host boundary.

**Context.** A document-bearing embedding - a host object a script reads properties off, calls
methods on, and hands listeners to - was believed impossible in this profile, and it was believed by
reading rather than by trying. The reading is written down in the checkout twice: in [section
18](../roadmap.md#18-amendments-this-profile-expects-to-ask-of-the-core)'s newest row, the only row
there reached by an observation rather than by a design reading *(corrected:
[JSC-84](../roadmap.corrections.md#jsc-84))*, and the built-in `read` in `JsRealm.Global.cs`, which
exists so a shell-shaped environment probe finds a name and refuses because - the comment said - no
registration could let it answer. Both argued from the shape of a value capability. Neither had
asked whether a host object has to travel through a value capability at all, and nothing in the
component had tried the alternative, so the question of what a composition may put in a realm had
never been decided either way.

---

## 1. The impossibility argument, and the step in it that does not follow

**Every step of the argument is true.** A DOM accessor answers an object. A value capability is
entered through `IVmHostCapabilityInvoker`, whose members answer a `long` and a `VmOpaqueRef`
(`VmProfileContracts.cs:372` and `:377`). A `VmOpaqueRef` carries a runtime, a generation and a slot
as unsigned integers (`VmIdentityPrimitives.cs:539-543`), with no member that yields content, so it
is by construction not dereferenceable. Therefore no registration any composition could make lets a
host answer a guest with an object.

**The conclusion does not follow, because the argument assumes the object travels through the
capability channel.** It does not have to. A host object is an ordinary `JsObject` in this profile's
realm. Its methods are ordinary `JsNativeFunction` values. Calling one from host code is
`JsEngine.Call` from inside a native body, which is the identical call `Array.prototype.map`
(`JsRealm.Array.cs:1049`) makes when it invokes the function it was handed, through `ArrayInvoke`
(`JsRealm.Array.cs:1557-1560`), and the identical one every `Proxy` trap makes when it invokes a
handler (`JsProxy.cs:320`). **Nothing about any of it reaches the core**, which is precisely why it
can do the things the capability channel cannot express: it can return an object, carry a string,
hold identity across repeated reads, and call back into the guest synchronously and return after it.

**What this decision adds is the seam through which a composition reaches that, not a new execution
mechanism.** `JsHostRealm` is the seam; `JsHostValue` and `JsHostRef` are the value and the identity
an embedder holds; `JsHostObject` is the exotic subclass for an object whose lookup an embedder
completes; `IJsHostSurface` is what a composition supplies and `IJsHostExotic` what it implements
for that subclass; `JavaScriptProfile.DescriptorHostingRealms` is the descriptor door beside
`DescriptorAdmitting` and `DescriptorReEmittingWith`. A reader looking for the second interpreter
will not find one, and that absence is the argument: `JsHostRealm.Invoke` routes through
`engine.Call`, so the call-depth ceiling is charged, fuel is charged, cancellation is polled, and a
guest `throw` unwinds through the C# frames exactly as it does for a comparison function.

## 2. Which core gates are engaged, and which are not reached at all

**No rule of ADR 0004 is relaxed here, and that is a weaker claim than the true one: the rules are
not engaged.** `VmRuntime.TryBeginCall`, `VmRuntime.EnterCapability` and
`VmInstanceImplementation.TryAdmit` are the gates a nested core operation would meet, and a host
object's method meets none of them, because it makes no core call. The seam has no member that
reaches the runtime, the instance, the mediator or the capability invoker; its widest member is
`Invoke`, and what `Invoke` reaches is the interpreter.

**The distinction is worth stating because the obvious alternative - lifting the instance gate
behind an opt-in declaration - would have corrupted invariants that live nowhere near the gate.**
Each of these was verified by reading the runtime rather than inferred from the design:

- **The execution scope is not nestable and has no restore.** `VmExecutionScope.Enter` assigns the
  ambient meter and owning operation and `Leave` nulls both (`VmExecutionScope.cs:60-72`), so an
  inner call that entered and left would return to an outer step whose `Current` is `null`. The
  outer profile would then be charging through a meter that resolves to no operation.
- **The load mediator's per-operation counters would become a bound-evasion primitive.**
  `VmArtifactLoadMediator.EnterScope` zeroes `fanOut`, `bytes` and `verifierWork` whenever the
  operation id differs from the one it holds, and never restores the outer operation's values
  (`VmArtifactLoadMediator.cs:95-113`). The comment there says exactly why the reset is conditional;
  a nested operation satisfies the condition, so the resumed outer operation would continue with its
  guest-load bounds cleared.
- **The nested interval would be billed twice.** A new operation gets a new `VmMeter`, which starts
  its own `Stopwatch` at construction (`VmMeter.cs:95`) and commits `WallClock` and the other
  aggregate-scoped dimensions to the same runtime level and the same aggregate parent
  (`VmMeter.cs:226-254`). A second meter running over the same wall interval charges the shared
  levels for it twice, which makes a host ceiling mean something different depending on how deeply
  an embedder nested.
- **Cancelling the outer would not reach the inner.** `VmOperation` holds a runtime, an instance, a
  profile, a meter and a `CancellationTokenSource` of its own, and no reference to a parent
  operation (`VmOperation.cs:40-54`). There is no walk from the outer operation to the inner one, so
  a host that cancelled the thing it started would leave the nested operation running.
- **ADR 0004 forbids the direction of the change.** Its lifecycle section closes with "A profile may
  tighten these rules, declaring that it accepts no reentrancy at all. It may never relax them"
  (`docs/adr/0004-lifecycle-and-state-machine.md:500-501`). An opt-in declaration that admitted a
  nested invocation is a relaxation whoever opts in.

**None of that is a cost this design pays, because none of it is a mechanism this design uses.** The
failures above are the price of the rejected option in section 3, and they are listed here rather
than there because they are also the reason the argument of section 1 is load-bearing rather than
clever: the seam is safe by not being on that path, not by being careful on it.

## 3. What was considered

| Option | What it does | Why it was not taken |
|---|---|---|
| **A capability trampoline** | The host answers a value capability with a directive - "call the function in that slot with these arguments" - and the profile performs the guest call on the host's behalf | It keeps the traffic in the channel and pays for it in every dimension that matters. A directive has to be encoded into a `long` or a `VmOpaqueRef` and decoded back, so the profile owns a wire format nobody specified; the host's own C# stack is unwound and re-entered between the request and the answer, so a provider cannot hold a local across the guest call it asked for, which is what a DOM method body is; an exception thrown by the guest arrives as data to be re-encoded rather than as a `catch` in the frame that caused it; and the profile ends up interpreting host intent, which is a second execution mechanism with a second set of rules. It also buys none of the safety, because the directive still has to reach guest code in the end |
| **Lift the `Executing` gate behind an opt-in descriptor declaration** | `VmInstanceImplementation.TryAdmit` admits a nested `Invoke` where the capability declared it | It is a core change of the one shape ADR 0004 forbids, and section 2 lists what it breaks: a non-nestable execution scope with no restore, a load mediator that zeroes an operation's fan-out and byte counters and never puts them back, a second meter billing one wall interval to the shared levels twice, and no parent reference for a cancellation to travel down. Each of those is silent - the nested call works, and the bound it evaded is the thing that stops being true |
| **The external-suspension park-and-resume channel** | The profile parks at a safepoint, the host builds its answer while parked, and the runtime resumes with it | **The channel is outbound-rich and inbound-empty.** `VmSuspension.TryGetProjection` carries a profile payload OUT (`VmLifecycleObjects.cs:196`); `VmRuntime.Resume(VmSuspension)` takes no payload IN (`VmRuntime.cs:320`). There is no core member through which a resumed operation receives anything, so an object built while parked has no way back. It is also the wrong shape for a synchronous method: a getter that parks the whole operation and hands control to the host is not a getter any script could have written, and [JSD-0023](0023-which-pauses-route-through-a-core-suspension.md) already records that external suspension stays undeclared at core contract version 1 because an executor cannot see the request |
| **A second runtime the embedder bridges** | The host objects live in a runtime of their own and the embedder marshals between the two | It is the design ADR 0003's standing rule refuses by name: exactly one core state machine exists in the product graph, and a design that can only be hosted by a second one is refused. It also fails on its own terms - section 13 says two agents under one aggregate parent are a channel and not isolation, and a bridge between two runtimes is a membrane somebody has to write, get right, and defend, in exchange for a host object that could be an ordinary object in the realm it is used in |
| **Host objects in the realm, with the capability table as the permission** | An embedder installs ordinary objects, and a composition's registration decides whether any embedder is reached at all | Taken |

## 4. The decision

**The host surface is this profile's own seam, and the capability table is the PERMISSION rather
than the CHANNEL.**

- **A host object is an ordinary object in the realm.** It is minted through `JsHostRealm`, its
  methods are native functions, and a call into guest code from one is `JsEngine.Call`. Nothing
  crosses the core, so nothing nests a core operation, re-enters a runtime, or asks a lifecycle gate
  for permission that gate was not designed to give.
- **A realm reaches an embedder only where the composition registered
  `broiler.javascript.host-surface`.** Section 5 states what keeps that true.
- **The seam is charged, bracketed and abort-latched**, and section 7 states each as the answer to a
  specific way this could have been unsafe.
- **The core is untouched.** Section 10 states the governance position in full.

## 5. Registration is the permission, and what keeps that true when no traffic goes through it

**The permission is an optional import the profile never invokes.** `HostSurfaceCapability` is
declared in `JavaScriptProfile.cs:425-437` with signature `(unit)->unit`, kind `Value`, and it
occupies the binding slot `HostSurfaceBindingIndex = 3` (`JavaScriptProfile.cs:284`). The profile
asks `IsBound` about that slot once, at instantiation (`JsExecution.cs:262-263`), and no other line
in the profile assembly addresses the slot at all. Its signature says `unit` in both directions
honestly, because nothing is asked and nothing is answered; a signature naming bytes it never
carries would be a shape a reader could reasonably expect to be used.

**Supplying an embedder and registering the permission are different acts, and requiring both is the
whole mechanism.** A composition supplies its embedder when it builds the descriptor, through
`DescriptorHostingRealms`; it registers the capability when it creates the runtime.
`JsExecution.Instantiate` installs the surface only when both are present, and the comment above
that condition says why: a build that linked an embedder still has no host object in its realms
unless the composition that ran it said so through the table the core owns. A descriptor built with
an embedder in a runtime that registered nothing produces realms with no host object in them,
silently and correctly - nothing was permitted, so nothing was installed.

**That sentence is executed rather than asserted.** The `--host-surface` lane's last check runs a
composition that registers no permission and asserts on what the guest printed: `typeof broilerHost`
is `undefined` (`HostSurfaceChecks.cs:111-115`). A reader who wants to know whether registration is
really the gate can delete the registration and watch the global disappear.

**Nothing holds a realm ambiently, which is what stops the permission being routed around.** Rule
N20 reads the profile family's sources as text and reports any static or thread-static holder of an
`IJsHostSurface` or a `JsHostRealm`, and asserts that both the host function delegate and the
surface callback take their realm as a parameter. The shape it rejects is the one a later change
produces by kindness rather than by carelessness: a callback deep in an embedder's own code needs
the realm, the parameter did not reach it, and a thread-static is the one-line simplification a
reviewer would otherwise ask for. In a process hosting two compositions that holder would make one
embedder reachable from the other's realm, underneath the table where no registration can see it.

## 6. What the reentrancy declaration says, and what a run showed it buys

**The descriptor declares `ReentrantIntoInvokingRuntime`, and it is the first shipping descriptor in
this tree to declare that mode** - the only other declaration is the test fixture minted alongside
it to exercise the admitted arm of the rule (`FixtureHostCapabilities.cs:104`). A host object's
method calls guest code - a listener, a promise reaction, a `toString` coercion - and a composition
reading the descriptor before it registers is entitled to know that. Nothing here depends on the
core admitting the re-entry, because nothing here crosses the core, so the declaration is not
load-bearing for the mechanism. What it does is refuse to understate what registering permits.

**Nothing refuses a re-entrant value capability, and that was established by running rather than by
reading.** The capability invoker's `TryEnter` hands the descriptor's own mode to
`VmRuntime.EnterCapability` (`VmCapabilityBinding.cs:232`), which raises the in-capability depth
only for `NonReentrant` (`VmRuntime.cs:615-621`), and `TryBeginCall` refuses on that depth
(`VmRuntime.cs:668-671`). ADR 0011's field F5 says the refusal applies "where the capability
declared `NonReentrant`". The only well-formedness rule that refuses the mode outright pairs it with
`ArtifactProvider` (`VmHostCapabilityDescriptor.cs:190-199`), which a value capability is not.
`ConcurrencyTests.A_Capability_Declaring_Reentrancy_May_Re_Enter_Its_Own_Runtime` proves a nested
`Verify` from inside such a call completes, and it asserts on the nested call's own reason rather
than on the outer invocation - because a refusal there is a returned reason rather than a throw, so
an assertion that the operation completed would pass whether the nested call ran, was refused, or
was never reached.

**The complementary limit is proved beside it, because the declaration reads like more than it is.**
"May re-enter the invoking runtime" reads like permission to call back into the guest, and it is
not: `VmInstanceImplementation.TryAdmit` refuses an `Executing` instance with `ReentrancyRefused`
and never reads the declaration (`VmInstanceImplementation.cs:860-862`).
`A_Re_Entrant_Capability_Still_Cannot_Re_Enter_The_Executing_Instance` asserts that reason. The seam
does not need the admission it does not get, because it never asks: a host method calling a guest
function calls `JsEngine.Call`, not `VmInstance.Invoke`.

## 7. Charged, bracketed, latched: the ways this could have been unsafe

**It is charged, because the boundary charge does not apply to a boundary nobody crosses.** The
capability invoker charges `HostCalls` a single unit in `TryEnter` (`VmCapabilityBinding.cs:225`),
and a seam that never reaches the invoker gets none of that for free. So every crossing goes through
`JsHostRealm.Enter`, which calls `JsEngine.ChargeHostCrossing`: a single unit of `HostCalls`, then
fuel proportional to what the crossing carries (`JsEngine.cs:697-705`). **Rule N21 holds that
mechanically rather than leaving it to review**: it reads `JsHostRealm` member by member and reports
any public member that does not OPEN by entering the crossing charge, because a member that charged
after its work would let an exhausted allowance stop the next crossing rather than this one - off by
one in the direction nobody audits. Left alone, an embedder could have moved unbounded work across
the seam and paid for none of it, which is the shape of the failure the budget matrix exists to
prevent; the matrix's `HostCalls` row now names the dimension by what it is for rather than by the
one mechanism that used to realise it *(corrected: [JSC-211](../roadmap.corrections.md#jsc-211))*.

**It is bracketed, because a realm touched outside a step has nothing to charge and no operation to
fault.** `BeginHostStep` and `EndHostStep` open and close the window on the guest's own thread
(`JsExecution.cs:707` and `:720`, and `JsExecution.cs:306-320` for the instantiation-time
installation, which brackets `OnRealmCreated` the same way), and `Enter` refuses with
`JsHostRefusal.RealmNotCurrent` when the depth is zero or the calling thread is not the guest
thread. An embedder that stashed the realm and used it later would otherwise be running work nobody
is paying for, on a heap nobody is holding still. The window is a depth rather than a flag because a
step can contain a host call containing a guest call containing another host call, and it closes
when the outermost one does.

**An abort is latched, because a provider's `catch` is the one thing that can silently un-enforce a
budget.** A `JsAbort` reaching host code - a spent allowance, a cancellation, a depth ceiling - is
converted to `JsHostTerminatedException` and the abort itself is latched (`JsHostRealm.cs:823-829`).
Catching the exception clears nothing: `Enter` refuses every later crossing with it, and the
profile's own native frame raises the latched abort again when the host body returns
(`JsHostRealm.cs:761-790`). An embedder writing `catch (Exception)` around a guest call is ordinary
defensive style and would otherwise turn a spent allowance into a completed operation. **`EndStep`
answers the latched abort rather than throwing it, and that is not a detail**: it runs in a
`finally`, and an exception raised from a `finally` replaces whatever was already propagating, so a
version that threw would discard the very abort it was preserving in exactly the case the latch
exists for. The caller re-raises what it answers, where nothing else is in flight
(`JsExecution.cs:722-728`).

**Each property is judged by what the guest printed, never by a host-side return.** A host method
that was never called and a property that answered `undefined` both leave a host-side "it succeeded"
perfectly true, so the `--host-surface` lane runs a script per check and compares printed lines: an
object answered by a host method whose string property the guest reads; wrapper identity in both
directions, so the guest's `d === d` holds and the host recognises the value it handed out; an
accessor running host code on every read; a host method calling a guest function synchronously and
returning after it, printed as `before,listener:click,after`; a listener's `RangeError` arriving in
the guest's own `catch`; a host-raised `TypeError` satisfying `e instanceof TypeError`; an exotic
object answering a name it was never given while an ordinary property of the same name still wins;
and the unregistered composition of section 5. **The method paid for itself on the first run.**
`DefineValue` installed its property with the built-in attribute set - writable and configurable,
deliberately not enumerable, so that a `for...in` does not walk `Object.prototype`'s methods - and
`Object.keys` on a host object therefore answered without the members the embedder had just
installed. The interface-description language a DOM-shaped embedder implements says its members are
enumerable, writable and configurable, so that is the set installed now: the same one an ordinary
guest assignment produces.

## 8. Governance: the sentence this corrects, and the sentence it does not

**The non-goal it touches is quoted here rather than paraphrased.** Before this decision, [the
plan](../roadmap.md#non-goals)'s CLR-interop non-goal read: "A host reaches guest code through
typed, allowlisted, versioned capabilities and through nothing else", which stood at
`roadmap.md:237-238` until the correction landed. It is not left standing unexamined and it is not
left standing unchanged; it is corrected by [JSC-210](../roadmap.corrections.md#jsc-210).

**The half that carries the force holds exactly as written.** No JavaScript-reachable surface
resolves a CLR type by name, constructs a generic type at run time, or enumerates CLR members. The
seam does not weaken that: what an embedder installs are this profile's own values. `JsHostValue` is
a kind, a double and a reference; the reference is a `JsHostRef`, which has no members at all and
exists so that identity survives a crossing. A guest that reaches a host object reaches a
`JsObject`, and there is no path from one to a CLR type it can name.

**The other half was a statement about a channel that was read as a statement about the number of
doors, and that reading is what this decision falsifies.** "Through nothing else" was true of the
capability channel and remains true of it; it was never true that a capability is the only thing
that can admit host code, because nobody had built the alternative to find out. A component
consuming this profile drew the wider reading and concluded a document-bearing embedding was
impossible. The corrected non-goal says what was actually meant: what a host may reach guest code
with is decided by a typed, allowlisted, versioned capability and by nothing else - a composition
that registered none reaches nothing - and the objects an embedder installs under that permission
are this profile's own values rather than CLR types the guest can name.

**The capability remains the permission, and a permission is a stronger gate than a channel rather
than a weaker one.** A channel gates traffic and can be probed message by message; a permission is
asked once, at instantiation, and either the door exists for that runtime or it does not. Nothing an
embedder does at run time can conjure the registration, because the registration is a fact about the
runtime the core built.

**The sentence in [section 13](../roadmap.md#13-realms-agents-and-the-host-boundary) is untouched,
and it is a statement about a different boundary.** "No CLR type crosses the boundary. Arguments and
results are the core's transfer types" (`roadmap.md:1461-1462`) is written about the core host
boundary, and this decision changes nothing there: the arguments and results that cross the core are
still `long`, `VmBytes` and `VmOpaqueRef`, `docs/api/public-api.txt` is byte-identical across this
whole branch, and the seam is not reachable from any core member. Reading that sentence as a
statement about the realm would prove too much - it would forbid `JsObject` itself, which is a CLR
type every guest object already is.

**Section 18's newest row is corrected in its conclusion and not in its subject** *(corrected:
[JSC-212](../roadmap.corrections.md#jsc-212))*. The capability channel still cannot answer a guest
with bytes, and that is what the row is about. What is no longer true is the conclusion it drew -
that a shell-shaped global like `read` can exist and refuse and can never do anything else - because
a composition that registers the host surface can install a `read` of its own that answers. The
refusal in `JsRealm.Global.cs` stands as the answer for a composition that did not.

## 9. What this does not provide, stated flatly

- **The capability channel still cannot answer a guest with bytes.** Nothing here amends that, and
  section 18's row stays filed and held as a request of the core.
- **Nothing here is a JSEAL provider.** That is browser-side work in a different repository, and no
  line of it exists in this component.
- **There is no promise or job surface on the seam**, no structured clone, no second realm, and no
  module loader. Guest-initiated loads still route through the mediator, so JS-8's exit-gate clause
  about the profile assembly reaching no byte-returning host object is unaffected: the seam gives an
  embedder a way to install guest-visible objects and gives the profile no byte source at all.
- **The realm is single-threaded and step-scoped.** There is no worker story here, and a realm
  touched off the guest thread is refused by name.
- **`VmInstanceImplementation.TryAdmit` still refuses a nested invocation**, so a host may not call
  `Invoke` on the instance whose step it is inside. The seam does not need that and does not do it.

## 10. Governance: no core amendment is made, and none is needed

**Nothing in `Broiler.VM.Abstractions` or `Broiler.VM.Runtime` changed**, and
`docs/api/public-api.txt` - the frozen public surface of the packable assemblies - is byte-identical
to what it was at this branch's merge base, which is the mechanical statement that no core contract
surface moved. The profile family's own baseline gains the seam, which is what a new public surface
in a profile package is supposed to do.

**ADR 0003 section 6 covers this case in terms.** "Implementing something version 1 already admits
(section 8) is not an amendment" (`docs/adr/0003-core-contract-v1-and-amendments.md:366-367`). An
optional value capability, asked about with `IsBound` and never invoked, is version 1 as shipped.

**Minting a version was neither wanted nor available, and this record says why neither.** The
amendment procedure of ADR 0003 section 6 requires a co-signature before a version may be minted,
and `docs/mvp.md` section 2.4 defers exactly that co-signing on 2026-09-07 - noting that the
procedure was already unexecutable, because one person holds the minting role R5 and both co-signing
roles, so no co-signature would be independent. This decision does not test that: it needs no
amendment, and a design that needed one would have been recorded `Blocked` rather than taken.

## 11. What would falsify this

- **A core member on the seam.** If any member of `JsHostRealm` ever reaches the runtime, the
  instance, the mediator or the capability invoker, section 2's claim that no core gate is engaged
  stops being structural and becomes a thing somebody has to keep true.
- **A crossing that is not charged.** If a member is added that does not open with `Enter`, the
  argument that an embedder cannot buy unmetered work is finished and the `HostCalls` row is
  overstated with it. Rule N21 is the instrument, and a rule that scans nothing reports its own
  vacuity rather than passing.
- **An operation that completes after its allowance was spent.** That is what the latch exists to
  prevent, and it is the falsifier written on `JsHostTerminatedException` itself. A provider
  catching it and continuing to a normal completion would mean the latch is not doing its work.
- **A realm reachable by an embedder a composition did not register.** Section 5 is the whole of the
  permission argument, and a single path that installs a surface without the binding being bound
  ends it. An ambient holder of a realm or a surface is the same failure reached sideways, which is
  what rule N20 reports.
- **The core gaining a value capability that can answer with an object.** If a later contract
  version carries one, the channel and the seam would overlap, and which one a composition should
  use becomes a decision this record does not take.
