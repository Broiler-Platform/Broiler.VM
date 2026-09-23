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
  ambient meter and owning operation and `VmExecutionScope.Leave` nulls both, so an inner call that
  entered and left would return to an outer step whose `Current` is `null`. The outer profile would
  then be charging through a meter that resolves to no operation. *(Corrected 2026-09-17: this bullet
  and the third in this list cited the runtime by file and line, and the lines moved when the core
  meter's fuel pre-admission - route MVP-9 in `docs/mvp.md` - and its owner-directed remedy changed
  those files, so they name the members instead. No claim changed: each was read again against the
  members as they now stand.)*
- **The load mediator's per-operation counters would become a bound-evasion primitive.**
  `VmArtifactLoadMediator.EnterScope` zeroes `fanOut`, `bytes` and `verifierWork` whenever the
  operation id differs from the one it holds, and never restores the outer operation's values
  (`VmArtifactLoadMediator.cs:95-113`). The comment there says exactly why the reset is conditional;
  a nested operation satisfies the condition, so the resumed outer operation would continue with its
  guest-load bounds cleared.
- **The nested interval would be billed twice.** A new operation gets a new `VmMeter`, which starts
  its own `Stopwatch` in its constructor and commits `WallClock` and the other aggregate-scoped
  dimensions to the same runtime level and the same aggregate parent (`VmMeter.AccrueWallClock` for
  `WallClock`; `VmMeter.TryChargeLocked`, the locked body of `VmMeter.TryCharge`, for the others; and
  `VmMeter.CommitPreAdmittedFuelLocked` for fuel a meter with no aggregate parent held in a block).
  A second meter running over the same wall interval charges the shared levels for it twice, which
  makes a host ceiling mean something different depending on how deeply an embedder nested.
  *(Corrected 2026-09-17: named by member rather than by line, as the first bullet says.)*
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
reading.** The capability invoker's `VmCapabilityInvoker.TryEnter` hands the descriptor's own mode
to `VmRuntime.EnterCapability`, which raises the in-capability depth only for `NonReentrant`, and
`VmRuntime.TryBeginCall` refuses on that depth. *(Corrected 2026-09-17: this sentence cited the three
members by file and line, and the lines moved when the core meter's fuel pre-admission - route MVP-9
in `docs/mvp.md` - and its owner-directed remedy changed those files, so it names the members instead.
The claim is unchanged: the refusal is still keyed on the descriptor's mode, and the remedy's entry
record, which `VmRuntime.EnterCapability` now returns, is taken only for `NonReentrant`.)* ADR 0011's
field F5 says the refusal applies "where the capability
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
and never reads the declaration. *(Corrected 2026-09-17: named by member rather than by line, for
the reason the paragraph above gives.)*
`A_Re_Entrant_Capability_Still_Cannot_Re_Enter_The_Executing_Instance` asserts that reason. The seam
does not need the admission it does not get, because it never asks: a host method calling a guest
function calls `JsEngine.Call`, not `VmInstance.Invoke`.

## 7. Charged, bracketed, latched: the ways this could have been unsafe

**It is charged, because the boundary charge does not apply to a boundary nobody crosses.** The
capability invoker charges `HostCalls` a single unit in `VmCapabilityInvoker.TryEnter` *(corrected
2026-09-17: named by member rather than by line, for the reason section 6 gives)*, and a seam that
never reaches the invoker gets none of that for free. So every crossing goes through
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
  module loader. *(Corrected 2026-09-21: the job members `EnqueueJob`, `DrainJobs` and
  `HasPendingJobs` were already on the seam when this was re-read, and a promise capability joined
  them under section 13; the other three absences stand.)* Guest-initiated loads still route
  through the mediator, so JS-8's exit-gate clause about the profile assembly reaching no
  byte-returning host object is unaffected: the seam gives an embedder a way to install
  guest-visible objects and gives the profile no byte source at all.
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

## 12. Addendum, 2026-09-21: bulk byte members (JSeal slices I01 and I03)

*This section adds members to the seam and changes none of the decision above. No human review is
recorded for it; it is local implementation work, not an accepted milestone.*

**Why they exist.** An embedder that needs an `ArrayBuffer`'s bytes, or needs to hand bytes to the
guest, previously had to do it through guest intrinsics it had captured - a typed array constructor,
`join` and decimal parsing on the way out, `%TypedArray%.prototype.set` over per-byte value chunks
on the way in. That costs crossings proportional to the size, and every step is something a page can
redefine: a species, a `length` accessor, a `constructor`. Three members replace it:

- `JsHostBufferStatus TryReadArrayBuffer(JsHostValue value, out byte[] bytes)` copies all of a
  buffer's bytes into a new array the embedder owns.
- `JsHostBufferStatus TryReadArrayBuffer(JsHostValue value, Span<byte> destination, out int byteLength)`
  copies them into the front of a span the embedder supplies.
- `JsHostValue NewArrayBuffer(ReadOnlySpan<byte> bytes)` makes a new buffer holding a copy of the
  caller's bytes.

**The brand is the realm's own buffer type, and no guest-writable thing takes part.** Neither read
reads a property, calls a function or consults a global, and the construction uses the realm's
intrinsic `ArrayBuffer.prototype` whatever the global named `ArrayBuffer` has become. So no guest
code runs inside any of the three, and a page that replaced `ArrayBuffer`, `Uint8Array`, a species,
`join`, `subarray`, `set`, `length`, `byteLength` or `constructor` changes nothing they do. An
instance of a guest subclass of `ArrayBuffer` is a buffer; a typed array, a `DataView`, a proxy
around a buffer, an object whose prototype is `ArrayBuffer.prototype` and every primitive are not.

**The answers.** `Copied` is the only status under which bytes were written, and it is also the
answer for an empty buffer (zero bytes). `NotAnArrayBuffer` and `Detached` write nothing and answer
an empty array or a length of zero. `DestinationTooSmall` (span form only) writes nothing and answers
the length the buffer needs, so the embedder can size a destination and ask again. A value minted by
another realm, or a call outside a step, is still the refusal it always was
(`JsHostSurfaceException`), not a status.

**Ownership.** The realm's storage is never handed out: both reads copy, so later guest writes cannot
change what the embedder holds and embedder writes cannot change the buffer. The span passed to
either the span read or `NewArrayBuffer` is borrowed for that call only and is never retained; bytes
a caller changes after `NewArrayBuffer` returns are not in the buffer.

**Metering.** Each member enters the crossing charge first, as rule N21 requires (one `HostCalls`
unit plus 2 fuel for a read, 4 for the construction). A read that will copy `n > 0` bytes then
charges `n` fuel **before** copying. The construction charges `n` fuel - the same charge
`new ArrayBuffer(n)` makes - and then asks the meter to admit `n` live bytes with `TryCharge` rather
than reporting them afterwards, so a `LiveBytes` refusal is decided before the buffer exists rather
than at the next charge. A fuel charge larger than the engine's poll window is split at that window,
and each window polls cancellation. A refused charge, a refused retention or an observed cancellation
ends the operation exactly as any other crossing does: `JsHostTerminatedException`, latched. No
bytes were copied into the destination, the `out` array is not assigned, and no buffer was made - there
is no partial result. A span longer than `Array.MaxLength` is a `RangeError`, as `new ArrayBuffer(n)`
would be.

**Compatibility and versioning.** The members are additive; nothing existing changed shape, and the
profile's API baseline records them. `JsHostBufferStatus` may gain members in a later version - a
shared buffer, if this profile ever has one, would get its own answer rather than being folded into
`Copied` - so an embedder treats any status it does not recognise as "nothing was written".

*Amended 2026-09-21 (JSeal F04-F06).* This paragraph said the same of a **resizable** buffer. Now
that the realm has one, it is answered without a new member: a read is a single crossing in which
no guest code runs, so the length measured and the bytes copied are the same instant's, and a
resizable buffer answers `Copied` with the bytes it holds now - or `DestinationTooSmall` with its
current length, which a later resize may change before the embedder asks again - exactly as a
fixed-length buffer does. The maximum and the resizability are not reported. `DetachArrayBuffer`
detaches a resizable buffer as it does any other (it stays resizable, with no bytes);
`NewArrayBuffer` still builds a fixed-length buffer only. A shared buffer, which another agent
could change during the copy, is the case that would still need its own answer. There is no `SharedArrayBuffer` in this profile ([JSD-0028](0028-shared-memory-and-atomics.md)),
so there is nothing to exclude today. There is still no zero-copy or pinned external storage: every
transfer in either direction is a copy.

**Where it is exercised.** The `--host-surface` lane's byte checks: exact bytes for empty, short and
multi-chunk inputs in both directions, judged by a digest the guest computes itself; the span form's
too-small and untouched-tail cases; wrong brands, a subclass and a proxy; a detached buffer and the
one it moved to; replaced globals, prototypes, species and `join` with a record that no page code
ran; and fuel, live-bytes and cancellation failures in which the host sees the termination, its
destination is untouched, no value is answered and the operation ends `ResourceExhaustion` or
`Cancellation`. Section 9's statement that the seam gives the profile no byte source is unchanged:
these members move bytes when an embedder calls them, and nothing in the profile calls out for bytes.

## 13. Amendment of 2026-09-21: exotic deletion and host promise capabilities

**Status of this section:** implemented locally for JSeal slices I05 and I07 (the host-API half of
[section 13](../roadmap.md#13-realms-agents-and-the-host-boundary)'s hosting stages JSH-2 and
JSH-3). It is not accepted milestone evidence; the local validation record is
`docs/evidence/jseal-i05-i07/README.md` at the repository root. The owner and co-signer position of
this record is unchanged. Both additions are public members of the profile family's own baseline
(`docs/api/public-api.txt`, JSD-0012); nothing in `Broiler.VM.Abstractions` or `Broiler.VM.Runtime`
changed, so section 10 holds as written.

### 13.1 `IJsHostExoticDeletion`: an optional deletion hook

**It is a second interface, so no existing `IJsHostExotic` implementer changes.** A handler that
also implements it is recognised once, when `NewExotic` mints the object; a handler that does not is
minted and behaves exactly as before, and a `delete` on its object is the ordinary deletion alone.

- **Which keys are offered.** Every string key that is not a canonical array index, once per
  deletion. `"7"` is an index and is never offered; `"007"` and `"4294967295"` are names and are.
  A symbol-keyed deletion takes the engine's separate symbol path and is never offered. The route
  does not matter: `delete`, `Reflect.deleteProperty`, a `Proxy` with no `deleteProperty` trap of its
  own, and the host's own `JsHostRealm.DeleteProperty` all reach the one override in
  `JsHostObject.DeleteOwnProperty`, so each offers a name exactly once.
- **Order and result.** The handler is offered the name BEFORE the ordinary deletion, and the
  ordinary deletion runs whether the handler claimed or declined. What `delete` evaluates to is the
  ordinary deletion's answer, never the handler's. So a claimed name the handler held no property
  for answers `true` and stops answering; a declined expando is removed as on any object; a declined
  name the handler still answers answers `true` and goes on answering (the no-hook behaviour); and a
  non-configurable ordinary property is offered, stays, answers `false`, and throws a `TypeError` in
  strict code. That is the order and result of JSeal's `IJsExoticDelete` contract as both of its
  providers implement it today, and it is followed rather than improved on so that JSeal's adoption
  (I06) changes no observable behaviour.
- **Handler exceptions.** The offer is a charged crossing (`HostCalls` one unit, through the realm's
  `Enter`), and its exceptions get the translation every host function body gets: a
  `JsHostThrowException` reaches the guest as the value it carries, a `JsHostSurfaceException`
  reaches it as a `TypeError`, and a `JsHostTerminatedException` re-raises the latched abort. In each
  case the ordinary deletion does not run. Any other CLR exception is a defect in the embedder and is
  not translated, exactly as for the read and write hooks.
- **Rejected.** A `Proxy` inside the profile simulating the hook (the route JSeal takes today, and the
  one its I06 removes), and a sixth member on `IJsHostExotic`, which would break every implementer.

### 13.2 `NewPromiseCapability`, `ResolvePromise`, `RejectPromise`

**A genuine promise of the realm, settled by the realm's own machinery, with the resolving functions
kept on the host side.** `NewPromiseCapability` builds a `JsPromiseObject` on the realm's intrinsic
`Promise.prototype`, charged as `new Promise` charges, and never reads the `Promise` global - so a
guest that replaces or deletes `Promise` changes nothing about it. The resolving functions are never
guest values, so only the embedder can settle it.

- **First settlement wins.** `ResolvePromise` and `RejectPromise` share one `[[AlreadyResolved]]`
  flag on the capability. The first call answers `JsHostSettlement.Accepted`; every later call on
  either half answers `JsHostSettlement.AlreadyResolved` and does nothing - no value read, no `then`
  looked up, no reaction queued.
- **Thenables follow the engine's resolve procedure**, because it is the same procedure: resolving
  with the promise itself rejects with a `TypeError`; an object's `then` is read once, synchronously,
  and a throwing getter rejects with what it threw; a callable `then` is called from a queued job.
  Rejection never adopts.
- **Reactions run only from the job queue.** Settlement goes through the realm's single scheduling
  point, so no reaction runs inside `ResolvePromise` or `RejectPromise`; they run when the host drains
  or steps the queue.
- **The thread and turn rule.** Both settlement members open with `Enter`, so they are valid only
  inside a step of the owning instance and on the guest's thread, which is where a `then` getter on a
  resolution value may run guest code. An embedder whose work completes elsewhere asks for a turn
  (`JavaScriptProfile.TurnEntryPoint`) and settles inside `IJsHostSurface.OnTurn`.
- **Explicit outcomes.** Late: `AlreadyResolved`. Outside a step or from another thread:
  `JsHostSurfaceException` with `RealmNotCurrent`. After the instance is released: also
  `RealmNotCurrent`, because the profile is not told about release and no step can open for that
  realm again, so a capability that outlives its instance can never settle. A capability or a value
  another realm minted: `ForeignRealm`. After an abort was latched: `JsHostTerminatedException`, as
  for every member. `JsHostValue.Missing` as the value or reason: `ArgumentException`, and the
  capability stays unsettled - Missing unwraps to the engine's uninitialised-binding marker, which
  is not a language value, and a promise settled with it would hand each reaction a hole that reads
  as a TDZ `ReferenceError`. Resolving with `undefined` is spelled `JsHostValue.Undefined`. Every
  misuse (foreign, Missing) is refused before the already-resolved flag is read, so it is reported
  even on a settled capability.
- **Rejected.** A parallel promise implementation on the host side, and exposing the engine's
  native resolving pair as guest functions for the embedder to `Invoke`, which would work but would
  make a settlement indistinguishable from an ordinary call and leave "late" unreportable.

**Unhandled rejections are still reported nowhere**, as `JsRealm.Promise.cs` records; a host promise
rejected with no handler is as silent as a guest one.

## 14. Addendum 2026-09-21: `DetachArrayBuffer`, the one host operation the language names

**One public member is added to the seam: `JsHostRealm.DetachArrayBuffer(JsHostValue)`.** The
specification makes `DetachArrayBuffer` an abstract operation hosts call - a structured transfer, a
conformance suite's `$262.detachArrayBuffer` - and the language's own door to it is
`ArrayBuffer.prototype.transfer`. Before this member an embedder holding a buffer had no way to
perform it, so the conformance harness could not provide the host half of the suite's
`detachArrayBuffer.js`, and several hundred variants over `ArrayBuffer`, `DataView` and the typed
arrays met the profile's refusing `$262.detachArrayBuffer` stub instead of the engine.

**It is added under this record's rules and changes none of them.** It opens with `Enter`, so it is
charged and bracketed like every other crossing (section 7, rule N21); it reaches `JsArrayBuffer`
and nothing in the core (section 2); and a guest reaches it only through a function an embedder
installed in a composition that registered `HostSurfaceCapability` (section 5). Ordinary guests are
unchanged: the end-user host registers no surface, and its realms keep the refusing stub. A value
that is not an `ArrayBuffer` is a guest `TypeError`, not a `JsHostSurfaceException`, because asking
is not a wiring defect of the host. The only composition that installs a caller is the conformance
harness (`Test262Host`), which replaces that one member of the profile's own `$262` and leaves every
other member refusing. JSeal follow-up VM-FIX-A; the profile's `docs/api/public-api.txt` records the
member.

**It is executed rather than asserted, like the rest of the seam (section 7).** The `--host-surface`
lane has two checks for it: a buffer the guest handed the host is detached (its views read empty, a
second detach is a no-op, a typed array and a number are guest `TypeError`s), and a buffer kept from
the previous check's realm is refused with `ForeignRealm` rather than detached. The lane's turn check
also asks for a detach from outside a step and sees `RealmNotCurrent`.

## 15. Addendum 2026-09-21: host-linked module graphs and deferred imports (JSeal I11 upstream)

**Status of this section:** implemented locally as the VM host additions JSeal's I11 and I12 need
(the "VM host operation" and "VM deferred module answers" follow-ups of JSeal's I09 module design,
VM ledger JSW-8 and JSP-10). It is not accepted milestone evidence; the local validation record is
`docs/evidence/jseal-vm-module-host/README.md` at the repository root. JSeal has not adopted it.
Nothing in `Broiler.VM.Abstractions` or `Broiler.VM.Runtime` changed, so section 10 holds as
written; the additions are public members of the profile family's own baseline
(`docs/api/public-api.txt`, JSD-0012).

**What was missing.** The module goal existed - records, live bindings, namespaces, cycles and
top-level `await` - but only an artifact's own entry point or a guest `import()` could reach it. An
embedder holding a realm could not link a module graph into it, and a guest `import()` had to be
answered synchronously inside the provider's `Answer`, so a host whose module source arrives later
could not answer one at all.

### 15.1 `LoadModule` and `EvaluateModule`

- **`JsHostModule LoadModule(string specifier, string referrer = "")`** puts the pair to the
  composition's artifact provider as the same module request (`JsFormat.ModuleRequest`) a guest
  `import()` makes. The provider resolves and compiles under the module goal from whatever it holds
  - for an embedder with its own module map, an in-memory table of keys, texts and resolutions -
  and the core verifies its answer under the operation's allowance. The answer must be an artifact
  whose `module` entry is a module body; anything else is refused, so module text never takes the
  classic-script path. The graph is linked (environments created, declarations initialised, every
  resolution confirmed through `ResolveCapability`) and no body runs.
- **Identity** is the realm's existing module registry: a key the realm already holds is adopted,
  whichever route reached it first, and the realm answers the same `JsHostModule` and the same
  namespace for a key every time.
- **Failures** are the ones `import()` already had, thrown as a `JsHostThrowException`: `TypeError`
  for a module the provider does not have, no provider, or an unconfirmed resolution; `SyntaxError`
  for a source the front end refused or a graph that does not link. A refused graph leaves nothing
  registered: resolutions are now confirmed before any instance is recorded, which also stops a
  later import from adopting an instance whose declarations were never initialised.
- **`JsHostValue EvaluateModule(JsHostModule)`** answers the graph's evaluation promise, the same
  promise on every call. It never blocks and never drains; it fulfils with `undefined` when the
  last module has finished and rejects with what a module threw, only through the job queue.
- **The evaluation walk these operations share with `import()`** is VM-FIX-D's (errored modules
  keep `[[EvaluationError]]` for every route, a module another walk has under way is entered as a
  wait, an async module body fulfils with `undefined`), with two additions made here.
  - The ordering walk keeps the specification's depth-first indices, so every member of a strongly
    connected component learns its root (`[[CycleRoot]]`). A module whose own body ran is finished
    only when its root is (ES2026 `Evaluate` step 3): a later walk that meets it is sent to the
    root, and waits there while the root is awaiting or another walk holds it, runs the root if a
    walk that stopped to wait gave it back unrun, or fails with the root's error. Neither a host
    evaluation nor a dynamic import can settle while a module of its graph is suspended.
  - Resolutions are confirmed before any instance is recorded, so a refused graph leaves nothing
    registered and a later import cannot adopt an instance whose declarations were never
    initialised.
  - What the walk records is the same whichever route met it first: every later evaluation of an
    errored module, of another member of its cycle, or of a module that depends on one - host or
    `import()` - rejects with the identical value, and no body among them runs again. A
    top-level-`await` module that throws before its first `await` counts as failed at once. What
    a failed walk had claimed and does not depend on the failure returns to the linked state,
    unrun, and a later evaluation runs it.
- **One deviation remains, and it predates this section**: the walk evaluates an order one module
  at a time, so an async module and a sibling that does not depend on it do not run concurrently
  as the specification's `ExecuteAsyncModule` lets them, and a walk waiting on another walk's
  module resumes from a promise reaction rather than as one of that module's async parents. The
  outcome of every evaluation agrees; the interleaving of such siblings' side effects, and the
  order in which two waiting evaluations settle, can differ.

### 15.2 `IJsHostModuleLoader`, `CompleteModuleRequest`, `FailModuleRequest`

- **An optional second interface on the surface**, recognised once when the realm is handed over,
  like `IJsHostExoticDeletion` (section 13.1). A surface that does not implement it changes
  nothing. It exists only where `HostSurfaceCapability` was registered.
- **What is offered.** Every guest `import()` the realm cannot answer from the calling module's own
  static requests, after the specifier was converted and the options checked, inside the guest's
  step, as a charged crossing. The request carries the calling module's key (or the script's
  compiled referrer) and the specifier. `OnImport` answers `Now` - the provider is asked in this
  step, as without a loader - or `Deferred`.
- **Completing later.** `CompleteModuleRequest` performs the load the import would have made, in
  the step it is called from, and evaluates the graph; the import fulfils with the namespace or
  rejects with `TypeError`, `SyntaxError` or the thrown value, through the job queue.
  `FailModuleRequest` rejects it with an error of a named kind. Once only: a later call answers
  `JsHostSettlement.AlreadyResolved`. A request `OnImport` answered `Now`, or threw out of (which
  rejects the import in that step), is settled already, so an embedder that kept it is answered
  `AlreadyResolved` too (corrected 2026-09-21, JSeal VM-FIX-H: a throw left the request open). A request is refused with `ForeignRealm` in another realm,
  with `RealmNotCurrent` outside a step and after the instance was released (the realm settles
  nothing then), and with `InvalidOperationException` from inside the `OnImport` call that offered
  it.
- **It decides when, never what.** A completion still goes through the artifact provider and the
  core's verification; no module reaches a realm by another route.

### 15.3 Module policy is not evaluation policy

The provider sees a module request and an evaluation request as two payloads distinguished by
their first byte, and nothing here consults a guest-evaluation permission. A composition that
refuses every evaluation request - as the `--host-surface` lane's module provider does - still links
and imports modules. A composition that wants a content policy forbidding `eval` to refuse
`import()` too says so in its own provider or loader.

**Executed, not asserted.** The `--host-surface` lane has eight module checks: a two-module graph
(handle and namespace identity, the dead zone before evaluation, one evaluation, the promise
pending until a drain, live bindings, `import()` reaching the same namespace); a cycle with the
cross-module dead zone; failures of each kind, not cached, with an unconfirmed resolution leaving
nothing linked; top-level `await`, a host evaluation of a module a guest import is still awaiting,
and rejections; a deferred import completed once from a turn, late completions reported, nested
imports carrying the calling module's key, and a failed request; a module a guest import already
errored, and a module depending on it, rejecting a host evaluation with the identical value without
running again, and a host evaluation of a cycle member waiting for its awaiting root; completion from inside the offer
refused while `eval` is refused and modules still load; and a request refused in another realm,
outside a step and after release. The differential probe `the-module-identity.mjs` covers the
guest-visible half against the comparison engine.

**Not provided.** Import attributes (still declined), `import.meta` properties, and a
host-visible module status. An embedder that needs one reads it from the evaluation promise: it is
pending while the graph is under way, fulfilled when it finished and rejected with the evaluation
error when any module of the graph failed, whichever route evaluated it first. *(Amended
2026-09-22: section 20.1 adds `TryGetModuleState`, which answers the status itself.)*

### 15.4 Addendum 2026-09-21: the specification's async evaluation (JSeal I11-async)

Recorded by the implementing slice, not by the owner; it changes no operation's signature and
signs nothing. Evidence: VM `docs/evidence/jseal-vm-module-gaps/README.md`.

- **The evaluation walk is the pinned ES2026 algorithm.** `Evaluate`, `InnerModuleEvaluation`,
  `ExecuteAsyncModule`, `GatherAvailableAncestors` and `AsyncModuleExecutionFulfilled`/`Rejected`
  replace the ordered list, its wait entries and its give-back: each instance carries the
  specification's `[[DFSIndex]]`, `[[DFSAncestorIndex]]`, `[[CycleRoot]]`,
  `[[PendingAsyncDependencies]]`, `[[AsyncEvaluationOrder]]`, `[[AsyncParentModules]]` and
  `[[TopLevelCapability]]`, and an `~evaluating-async~` state. An async module no longer holds up
  its siblings, and the modules one completion releases run in `[[AsyncEvaluationOrder]]`.
  `[[EvaluationError]]` caching, cycle roots and the rule that a second evaluation of a module under
  way answers its root's promise are the algorithm's own.
- **One evaluation at a time.** `Evaluate` step 1 forbids overlap. A guest `import()` links and
  evaluates from a job, as `ContinueDynamicImport` does after its load promise, and settles from
  the reaction to the evaluation's promise, so an import settles two turns later than before. An
  `EvaluateModule` that arrives while an evaluation is under way (a body that calls back into the
  host) is deferred to a job the same way. Otherwise `EvaluateModule` still evaluates at once, and
  its promise is settled at once when the graph finished synchronously.
- **The entry graph's late failure is raised.** An artifact's entry module whose evaluation
  rejects after an `await` has nobody holding its promise. The rejection is raised from a job of
  its own, which the host's drain reports as a job that threw; before, it was lost inside a promise
  reaction. The conformance harness counts such a job as the runtime error a negative module test
  declares.
- **Fuel and allocation** are charged as before: one unit per module visited, per parent recorded,
  per ancestor gathered and per module released. Every wait is a promise reaction. No new public
  member, product file, opcode, diagnostic code or decision number was taken.

## 16. Amendment of 2026-09-21: `EvaluateScript`, a script-goal route into an existing realm

**Status of this section:** implemented locally for JSeal slice V15-host (the host route the VM's
V15 work left JSeal waiting for; [JSD-0026](0026-the-direct-eval-environment-boundary.md) section
13 names the gap). It is not accepted milestone evidence; the local validation record is
`docs/evidence/jseal-script-host/README.md` at the repository root. The owner and co-signer position
of this record is unchanged, and nothing in `Broiler.VM.Abstractions` or `Broiler.VM.Runtime`
changed, so section 10 holds as written.

**Why a member is needed at all.** An embedder that runs its own scripts in a realm it already
holds - JSeal runs every classic, host and guest script of a page that way - had one door, the
captured `eval` intrinsic, and since V15 that door is global EVAL code: a script's top-level `let`,
`const` and `class` no longer persist into the next script, its `var`s and functions become
configurable, and a forced strictness has nowhere to go in an indirect call. None of that is a
defect of `eval`; the embedder was using the wrong goal. `JsHostRealm.EvaluateScript(source,
sourceName = "", strict = false)` is the right one.

**What it does: `ScriptEvaluation`.**

- **Global declaration instantiation runs first.** Before the script's first instruction the
  executor checks, in the pinned ES2026 order, that no lexically declared name is already a global
  lexical declaration or a non-configurable own property of the global object
  (`HasRestrictedGlobalProperty`), that no `var` or function name is a global lexical declaration
  (each a `SyntaxError`), and that the global object can take every function
  (`CanDeclareGlobalFunction`) and then every `var` (`CanDeclareGlobalVar`) (each a `TypeError`).
  A script that fails a check creates nothing. The edition has no `[[VarNames]]` list, so a `let`
  over a configurable global - an eval-introduced `var` included - is admitted.
- **Bindings persist as a script's do.** `let`, `const` and `class` become bindings of the realm's
  global lexical environment that every later script and every guest function sees; `var`s and
  functions become non-configurable properties of the global object, a function's replacing an
  absent or configurable property (an accessor included) as `CreateGlobalFunctionBinding` does.
- **The answer is the completion value**, wrapped like any other value the seam hands out (a BigInt
  completion is the `TypeError` every BigInt crossing is, JSD-0033).
- **Strictness is the embedder's option**: `strict: true` compiles the script as strict code whatever
  its directive prologue says; `false` leaves it to the prologue.
- **A source that does not parse is a `SyntaxError` that names the source** ("the host script
  `name` is not a program this profile admits"). The position is the provider's to hold: the core's
  reason vocabulary cannot carry a diagnostic across the mediator, so the request carries the name
  and the provider attributes its diagnostic to it (`JsScriptUnit.SourceName`), which is the half an
  embedder reports.

**How the checks reach the executor: a new optional section.** The lowering already created every
binding with its own instructions, one at a time, so no instruction could run a check that must see
the whole script first. Each script body that declares anything now carries a row in a new optional
section, `ScriptDeclarations` (kind 14): its lexical names, its `var` names that are no function's,
its function names and its Annex B candidates. The verifier holds each row to a program-body unit
that is not eval code or a module's, to interned names, and to one row per unit
(`MalformedScriptDeclarations`, diagnostic 1631, registry revision 15); the executor runs the checks
from it in the one entry every script takes, so a script the end-user host runs from a file, a
harness include and an embedder's script are instantiated alike. **An artifact with no row keeps the
lenient instantiation it always had**, which is what every artifact written before the kind existed
says. One shape is refused by name rather than answered wrongly: a sloppy script whose block-level
function Annex B would NOT hoist - its name is an earlier script's global lexical declaration, or the
global object cannot take it - is an `EvalError` before anything is created, because this lowering
writes the alias unconditionally.

**Who may compile it, and why the embedder's permission is separate from the guest's.** The source
travels to the composition's artifact provider through the mediator, as every load does, under a
fourth request mark, `JsFormat.ScriptRequestMark` (`0x02`), followed by a flags byte (`Strict`) and
the source name and text. **The profile writes that mark from this member and nowhere else**: a
guest's `eval` sends the eval mark, a dynamic `import()` the module mark, the `Function` constructor
source that begins with `(`, and a guest String that begins with a control byte is answered as a
`SyntaxError` before anything is sent. So a provider may treat the mark as the embedder's
authorisation - answering host scripts under a policy that refuses guest evaluation - and a direct
or indirect `eval` the script itself performs is still an eval request that policy decides.
`JsCompiler.TryReadProgramRequest` reads the new mark into a script unit, so every provider built on
it speaks it; a provider that does not refuses it as a reserved mark, which the embedder sees as the
`SyntaxError` every refusal becomes. The source-provider capability stays at version 2: the mark is
additive, and bumping the version would have unregistered every provider for a request only an
embedder that asks for it sends.

**Charged, bracketed, latched.** The member opens with `Enter` (section 7, rule N21): one
`HostCalls` unit and 4 fuel, then the request's length in fuel as every evaluation is charged, then
the verified answer's own verification and execution under the same allowance. A guest throw reaches
the embedder as `JsHostThrowException`; an exhausted allowance or a cancellation as the latched
`JsHostTerminatedException`; a call outside a step as `RealmNotCurrent`. No provider registered, or an
answer that is not a script compiled under the strictness asked for, is a guest `EvalError`. A source
name containing U+0000 is an `ArgumentException` (the request separates the name from the source
with one).

**The conformance harness is its first caller**: `Test262Host` replaces the profile's refusing
`$262.evalScript` with a member that runs its argument through `EvaluateScript`, which is what the
suite's `global-code` directories measure. Every other composition keeps the refusing stub.

**It is executed rather than asserted (section 7).** The `--host-surface` lane has eight checks for
it: a `let` persisting into the next script with both completion values; conflicts refused before
anything is created (lexical over lexical, `var` and function over lexical, lexical over a
non-configurable global and over `undefined` and `NaN`, and a function and a `var` the non-extensible
global object cannot take); a guest `eval`, indirect `eval` and `Function` inside a host script
meeting a provider that refuses guest evaluation while the host scripts are answered; a guest that
cannot send the script mark; a syntax error that names its source while the provider holds
`broken.js:2:7`; the strictness option; an allowance spent inside a host script ending the run
uncatchably; and scripts an embedder runs from a turn, whose `let` a later program reads.

**What this does not provide.** No module goal (the module host is separate work), no referrer for a
dynamic `import()` written in a host script (the request carries none), and no host-defined data on
the script record. *(Amended 2026-09-22: a provider that places the script through
`JsScriptUnit.Referrer` now places its eval code and `Function` bodies too, section 20.3.)*

## 17. Addendum 2026-09-21: detached clone adoption across realms and threads (JSeal I17)

**Status of this section:** implemented locally for JSeal slice I17; not accepted milestone evidence.
The local validation record is `docs/evidence/jseal-i17/README.md` at the repository root. The
owner and co-signer position of this record is unchanged. The format, the supported-type matrix
and the cost model belong to [JSD-0032](0032-the-internal-structured-clone-carrier.md) (proposed),
which this section amends only where it says so; nothing in `Broiler.VM.Abstractions` or
`Broiler.VM.Runtime` changed, so section 10 holds as written.

**Two public members and one public type are added to the seam.**

- `JsHostRealm.DetachClone(JsHostValue value, ReadOnlySpan<JsHostValue> transfer = default)`
  answers a `JsHostCloneCarrier`: the sending half of a message to a realm on another thread,
  including the transfer of fixed-length `ArrayBuffer`s (JSD-0032 section 5a).
- `JsHostRealm.AdoptClone(object carrier)` answers a `JsHostValue` in this realm: the receiving
  half.
- `JsHostCloneCarrier` is opaque. It exposes `Profile`, `FormatVersion` (and the static
  `CurrentFormatVersion`), `IsSingleUse`, `IsConsumed` and `ChargedBytes`, and nothing of the graph.
- `JsHostRefusal` gains `ForeignCarrier = 4` and `CarrierConsumed = 5`.

**Both members are crossings under this record's rules.** Each opens with `Enter` (rule N21), so it
is valid only inside a step of the owning instance on the guest's thread, is charged one host call
(and, for `DetachClone`, fuel per transfer-list entry on top of JSD-0032's per-record charges), and
latches an abort. The carrier is the one value on this surface that is **not** realm-bound: it
holds no `JsHostValue`, no guest object and no realm or instance handle, which is what lets it leave
the realm that made it.

**The thread and turn rule.** Serialization runs on the sending realm's thread inside one of its
steps; adoption builds every object on the receiving realm's thread inside one of its steps. An
embedder whose sender and receiver run on different CLR threads therefore detaches in the sender's
turn (or in a host function the sender's guest called), hands the carrier over through any
synchronising handoff, and adopts in the receiver's turn (`JavaScriptProfile.TurnEntryPoint`). A
call from any other thread, or outside a step, is `RealmNotCurrent` and claims nothing. After the
handoff a carrier is never written except for the one-way claim below, which is an atomic
exchange, so several threads may read one carrier at once.

**Lifetime.** A carrier is sealed when `DetachClone` returns and stays valid, unchanged, after the
sending instance and runtime are disposed, for as long as the host references it. There is no byte
form, so a carrier does not outlive the process.

**Single use or repeatable (the JSD-0032 rule, now public).** A carrier made without a transfer list
is repeatable: it may be adopted any number of times, into one realm or several, on any threads;
each adoption builds new objects and copies the buffer bytes again, so two adoptions share nothing a
guest can write. A carrier holding transferred bytes is single-use: the first adoption claims it
atomically, before building or charging anything, and every later adoption, including one racing on
another thread, is `CarrierConsumed`. A claim is final even if the adoption that made it then fails.
`IsConsumed` is a snapshot for diagnostics; the refusal is the authoritative answer.

**Compatibility.** `AdoptClone` takes `object` because a host carries whatever its engines mint (a
JSeal provider holds a carrier as an untyped payload). It accepts only a `JsHostCloneCarrier` of this
profile build whose `Profile` is `JavaScriptProfile.Id` and whose `FormatVersion` is
`CurrentFormatVersion`; anything else - another engine's carrier, a carrier from a second copy of
this assembly (a different type), any other object - is `ForeignCarrier`. `null` is an
`ArgumentNullException`. In this build a carrier's profile and version always equal the current
ones, so those two comparisons can fail only in a later build that changes the layout; they are the
place such a build states what it still admits.

**Error shape (the choice JSD-0032 left to I17).** A value outside the matrix, a bad transfer list,
a graph past the bounds, and a destination that cannot represent a record are guest `TypeError`s
whose message begins `DataCloneError:` (raised to host code as `JsHostThrowException`), because they
are about the guest's value and the sending side is where HTML raises `DataCloneError`. A getter's
own throw propagates as itself. `ForeignCarrier` and `CarrierConsumed` are
`JsHostSurfaceException`s, because they are the host's mistakes; inside a host function a guest
called, those reach the guest as `TypeError`s as every surface refusal does. `JsHostValue.Missing`,
as the value or as a transfer-list entry, is an `ArgumentException` before anything is serialized
or detached, as it is for a promise settlement (section 13.2); it had been cloned as `undefined`
(corrected 2026-09-21, JSeal VM-FIX-H).

**Who pays for a carrier in flight (JSD-0032 section 4, amended).** The sender. `DetachClone`
reports `ChargedBytes` - 32 bytes per carrier entry plus every copied buffer and string byte, but not
transferred bytes, which the sender retained when it made the buffers and is not credited for - to
the sending instance's `LiveBytes` with `TryCharge` **before** any listed buffer is detached, so a
ceiling that refuses the carrier ends the operation with every buffer still attached. Like every
retention in this profile it is never credited while the instance lives and is released with the
instance's other live bytes when the instance is disposed; a carrier the host still holds after
that is host memory bounded per carrier (JSD-0032's two maxima) and by no budget, which is the
accepted limitation that remains. The receiver pays fuel for the rebuild and `LiveBytes` for every
buffer it rebuilds, as before.

**Not provided.** No `WorkerRealms` capability, no second realm on a second thread created by the
profile, no event loop or `postMessage`, and no browser `Worker` (card I17 exclusion; JSeal I18 owns
the capability). No sharing of live `JsHostValue`s across realms: every existing member still
refuses a value another realm minted with `ForeignRealm`.

**It is executed rather than asserted.** The `--host-surface` lane has four checks, each running
every runtime on a host thread of its own through the turn model: a graph with every supported
brand, a cycle, a shared object and holes is detached on thread A, whose runtime is then disposed
and collected, adopted on thread B and judged by B's guest against B's own intrinsics and `===`,
then adopted twice more on thread C, where the two copies share nothing; a transfer carrier detaches
its source at once, two receivers on two threads race for it and exactly one wins, and a later one
is refused; a foreign object, a string and `null` are refused, and an adoption outside a step or
from another thread during one is refused without claiming the carrier; and the sender's parent
budget moves by exactly `ChargedBytes`, a transfer carrier does not re-charge moved bytes, the
receiver pays exactly for the buffers it rebuilds, and a carrier its sender's ceiling refuses ends
the turn as an exhaustion.

## 18. Addendum 2026-09-21: `NewHtmlDdaObject`, an `[[IsHTMLDDA]]` object for a conformance host (JSeal VM-FIX-H)

**Status of this section:** implemented locally for JSeal follow-up VM-FIX-H; not accepted
milestone evidence. The local validation record is `docs/evidence/jseal-small-gaps/README.md` at the
repository root. The owner and co-signer position of this record is unchanged, and nothing in
`Broiler.VM.Abstractions` or `Broiler.VM.Runtime` changed, so section 10 holds as written.

**One public member is added.** `JsHostRealm.NewHtmlDdaObject()` answers a new object of the realm
with ECMA-262 Annex B.3.6's `[[IsHTMLDDA]]` internal slot. It is a non-constructable function whose
every call answers `null`, which is what the pinned suite's INTERPRETING.md asks of
`$262.IsHTMLDDA`. It is a crossing under this record's rules: it opens with `Enter`, is charged
like `NewMethod`, and latches an abort.

**The slot changes three operations and nothing else**, as the annex does: `typeof` answers
`"undefined"`, `ToBoolean` answers `false`, and `==` / `!=` treat the object as equal to `null` and
`undefined`. Strict equality, `??`, optional chaining, default initialisers, `GetMethod` and every
property operation treat it as the ordinary function object it is.

**One lowering changed with it.** `??`, `??=` and optional chaining had been lowered as
`LoadNull; LooseEquals`, which asks `== null`; with the slot that question differs from the one the
language asks (is the value `undefined` or `null`), and `$262.IsHTMLDDA ?? 1` answered `1`. They now
emit two strict comparisons and a strict comparison of the two answers - no opcode was added, no
guest code can run in the test, and the retained corpus regenerates byte-identical.

**Who uses it.** The conformance harness installs one as `$262.IsHTMLDDA` in the realms of its wide
runs, where the profile's own `$262` has no such member. No guest can make one, and the end-user
host and every other composition install none, so an ordinary program never meets the slot. A web
page host that models `document.all` is the only other embedder with a reason to call it.

**It is executed rather than asserted.** The `--host-surface` lane has a check over `typeof`,
truthiness, `&&`, `||`, `==`/`!=` against both nullish values, `===`, a call, `??`, optional
chaining and `Boolean`; the pinned suite's `IsHTMLDDA` tests are scored by the conformance harness.

## 19. Amendment of 2026-09-22: a BigInt crosses the surface (JSeal B06)

**Status of this section:** implemented locally for JSeal card B06 (its VM host-API half); not
accepted milestone evidence. The local validation record is `docs/evidence/jseal-b06/README.md` at
the repository root. The owner and co-signer position of this record is unchanged, and nothing in
`Broiler.VM.Abstractions` or `Broiler.VM.Runtime` changed, so section 10 holds as written.

Until this section the surface had no BigInt kind: a BigInt a guest handed across was refused with a
`TypeError` at the crossing, and one it threw reached the host as a `TypeError` standing in for it
(JSD-0033 sections 2 and 7). Both substitutions are gone.

**Five public members are added**, recorded in `docs/api/public-api.txt`:

| Member | What it is |
|---|---|
| `JsHostValueKind.BigInt` (10) | the eighth language kind, appended so no stored kind renumbers |
| `JsHostValue.BigInt(BigInteger)` | a BigInt of exactly that integer; realm-free, as `JsHostValue.String` is |
| `JsHostValue.AsBigInt()` | the integer, or `null` for any other kind (a Number is not a BigInt) |
| `JsHostRealm.ToBoolean(JsHostValue)` | the language's `ToBoolean`; runs no guest code |
| `JsHostRefusal.SurfaceDeclined` (6) | a value of a surface the realm's composition declined was presented |

**Conversion and identity.** A guest BigInt crosses as its exact integer, and a host
`BigInteger` crosses in as exactly that integer: nothing on the surface converts a BigInt through a
double, `AsNumber` of a BigInt is NaN and `AsBigInt` of a Number is `null`. A BigInt is a primitive
with no identity, so `JsHostValue.Equals` and `GetHashCode` compare a BigInt by its mathematical
value - two crossings of one `10n`, and `JsHostValue.BigInt(10)`, are equal and hash alike - which is
what `===` does; a BigInt is never equal to a Number, as `10n === 10` is false. A BigInt **object**
(`Object(10n)`) is an `Object` and compares by identity like every object. **The existing equality
is otherwise unchanged, and its one gap is recorded here rather than silently changed:** a String
compares by reference, so two equal texts that crossed separately may be unequal; embedders already
key tables on this struct, and a host that wants the language's relation over Strings compares
`AsString()`.

**Conversions.** `ToBoolean` answers `false` for `0n` and `true` for every other BigInt; it is a
realm member because an `[[IsHTMLDDA]]` object (section 18) is the one object that is false, and
`Missing` answers `false`. `ToJsString` of a BigInt is its decimal digits, charged as the language's
own formatting is. `ToNumber` of a BigInt - or of a BigInt object - is the language's `TypeError`,
raised as a `JsHostThrowException`.

**The crossing, in both directions.** Arguments to a host body, `this`, a body's return value,
`GetProperty`/`GetIndex` results, `SetProperty`/`DefineValue`/`DefineIndex` values, `Invoke` and
`Construct` arguments and results, promise settlement values, `Throw`, and clone roots all carry a
BigInt. **A host BigInt is admitted at the crossing, not at `JsHostValue.BigInt`**, because the
width and the surface are properties of a realm: a realm whose composition declined
`broiler.javascript.bigint` refuses one with `JsHostRefusal.SurfaceDeclined` (a guest `TypeError`
when a host body returned it), and a value wider than `JsBigInt.MaximumBits` (2^20 bits, the ceiling
every BigInt the language computes answers past) is the language's `RangeError` - a
`JsHostThrowException` inside a crossing, a guest `RangeError` from a body's return - measured from
the integer's length **before** the realm allocates anything. **Every crossing of a BigInt is
charged `LinearCost` of its 64-bit words**, into the realm and out of it; the integer is immutable
and is shared, not copied. The four crossings that resolve a value before their own `try`
(`DetachArrayBuffer`, `Invoke`'s and `Construct`'s callee, a promise settlement) translate a refusal
or an abort as the rest of the crossing would, so no internal exception reaches host code.

**A thrown BigInt reaches the host as itself**: a `JsHostThrowException` whose `Thrown` is the
BigInt, and a host that re-raises that exception throws the same value back into the guest. If the
per-word charge of carrying a thrown BigInt spends the allowance, the host receives the latched
`JsHostTerminatedException`, as for any other abort in a crossing.

**The clone.** `DetachClone`/`AdoptClone` carry a BigInt and a BigInt object (decision JSD-0032,
amended the same day: carrier layout 2). A realm that declined the BigInt surface refuses a carrier
holding one before it claims moved bytes.

**Exclusions, by name.** `BigInt64Array`/`BigUint64Array` (card B07) and the DataView accessors
(card B08) are not in the realm; the JSeal half of card B06 (its `VmMarshal`, `JsValue` kind and
equality contract) is JSeal's change and is not made here. The exotic-object hooks
(`IJsHostExotic`) call `Unwrap` without translating a refusal, as they did before this section for a
foreign reference; a handler that answers a BigInt past the ceiling there raises the guest
`RangeError`, and one that answers a BigInt to a declining realm raises the surface refusal
untranslated - a pre-existing gap of those hooks, not widened in kind.

**It is executed rather than asserted.** The `--host-surface` lane has eight BigInt rows (the
crossing and its exactness at the ceiling, host-built values and the `RangeError` past it, the three
conversions, equality, properties/calls/constructs, a thrown BigInt, the clone, and a declining
realm); the slice-compiler checks `bigint/b06/a-declining-realm-refuses-a-host-bigint` and
`bigint/b06/a-host-crossing-is-charged-per-word` (a fuel bisection), and the amended
`bigint/b01/a-thrown-bigint-reaches-the-host-as-a-host-throw`, which now expects the BigInt.

**Falsified if** a BigInt crosses inexactly or through a double, a BigInt wider than
`JsBigInt.MaximumBits` reaches a realm, a BigInt reaches a realm whose composition declined the
surface, a crossing of a BigInt is not charged in proportion to its words, two BigInts of one value
compare unequal or a BigInt compares equal to a Number, a thrown BigInt reaches host code as
anything but a `JsHostThrowException` carrying it or the latched termination, or `ToBoolean` of
`0n` answers `true`.

### 19.1 Note of 2026-09-22: exotic hooks and a host's thrown value are translated (JSeal VM-FIX-I)

**Status of this note:** implemented locally for JSeal VM-FIX-I; not accepted milestone evidence.
The local validation record is `docs/evidence/jseal-vm-fix-i/README.md` at the repository root. No
public member is added or changed.

The gap the exclusions above recorded is closed. **The hooks of `IJsHostExotic` are host bodies the
guest reaches through a property operation instead of a call**, so what they raise is translated
exactly as a host body's is: `TryGetNamed`, `TryGetIndex`, `TrySetNamed`, `SupportedNames` and
`IndexedLength` each run inside a translation that re-throws a `JsHostThrowException` as the guest
throw it carries, turns a `JsHostSurfaceException` into a guest `TypeError`, and re-raises the
latched abort for a `JsHostTerminatedException` - the rule `IJsHostExoticDeletion` already
followed (section 13.1). A value a hook **answers** is resolved inside the same translation, so a
reference another realm minted, or a BigInt answered to a realm whose composition declined the
surface, is the guest's `TypeError`; a BigInt past the ceiling remains the guest's `RangeError`.

**A value a host throws is resolved as a value it returns.** Every crossing that re-throws a host's
`JsHostThrowException` into the guest - a host body, a host job, a deletion hook, a module loader's
`OnImport`, and now the exotic hooks - resolves the thrown value in one place, where a refusal of
that value becomes the guest's `TypeError`: a host throwing `JsHostValue.BigInt(..)` into a realm
that declined the surface, or throwing a reference minted by another realm, no longer ends the
invocation with the surface refusal unwinding through interpreter frames.

**Falsified if** a hook's `JsHostThrowException`, `JsHostSurfaceException` or refusal of the value it
answered reaches the interpreter untranslated (the invocation ends in `ProfileFault` rather than a
catchable guest throw or the latched termination), or a host's thrown BigInt reaches a declining
realm as anything but a guest `TypeError`. The `--host-surface` lane executes each route: three
exotic rows (named and indexed reads that throw, are refused or answer a foreign value; writes that
throw or are refused; enumeration whose hooks throw or are refused; a hook whose guest callback
exhausts the allowance) and two BigInt rows (a thrown BigInt and a hook's BigInt reaching an
admitting realm as themselves; a returned, a thrown and a hook's BigInt in a declining realm).

### 19.2 Note of 2026-09-22: every exotic hook is a charged crossing (JSeal VM-FIX-J)

**Status of this note:** implemented locally for JSeal VM-FIX-J; not accepted milestone evidence.
The local validation record is `docs/evidence/jseal-vm-fix-j/README.md` at the repository root. No
public member is added or changed.

Section 19.1 translated what the `IJsHostExotic` hooks raise but left them uncharged and ungated,
while the deletion hook (section 13.1) and the module loader (section 15.2) each opened with
`Enter(1)`. **The accounting is now one rule for every host code unit the guest reaches without a
call:** before each `TryGetNamed`, `TryGetIndex`, `TrySetNamed`, `SupportedNames` and
`IndexedLength` the host object enters one crossing (`JsHostRealm.TryEnterHook`, which is
`Enter(1)` inside a step): one `HostCalls` unit and one fuel unit, the step must be on the guest's
thread, and a latched abort refuses the hook. The entry runs inside the hook's translation, so a
latched abort re-raises that abort and a step open on another thread is the guest's `TypeError`,
exactly as for the deletion hook; a spent allowance ends the operation as a `JsAbort` does anywhere
else. The enumeration of one exotic object is two crossings (its names and its indexed length). A
hook is entered only when the object's own storage did not answer, so an ordinary own property
still costs no crossing. `Wrap` of a value offered to `TrySetNamed` is unchanged and precedes the
entry.

**Outside every step the handler is not asked, and nothing is charged or raised.** Guest code runs
only inside a step, so the reads that reach a hook when no step is open are the engine's own: the
execution layer rendering an uncaught value (its `name` and `message`, or its `ToString`) or a
completion value after the step has closed. Those are not the guest's crossings and there is no step
to charge them to; refusing them turned an uncaught exotic object into a contract violation. The
host object therefore answers such a read as though the handler held nothing: a named or indexed
read finds only the object's own storage, an enumeration lists only its own keys, and a named write
is not offered. An uncaught exotic object ends the run as any uncaught value does
(`ProfileFault/ProfileFaultUnspecified`), rendered from its ordinary properties and prototype, and
a completion value that is one completes normally.

**Falsified if** an exotic hook runs without a `HostCalls` unit being charged, runs outside a step,
or runs after the realm latched an abort, or if the engine's own read of an exotic value after the
step closed changes the run's outcome; the `--host-surface` lane's exotic rows exercise the charged
path (a hook whose guest callback exhausts the allowance still answers the latched termination),
and two rows throw an exotic object uncaught and complete with one.

## 20. Amendment of 2026-09-22: module state, the referrer of eval and `Function` code, top-level await as a compile option, and `Missing` at every crossing (JSeal I11-upstream, I12-upstream, JSD-0024-missing)

**Status of this section:** implemented locally for the upstream half of JSeal cards I11 and I12
and the JSD-0024 `Missing` follow-up; not accepted milestone evidence. The local validation record
is `docs/evidence/jseal-vm-host-modules/README.md` at the repository root. The owner and co-signer
position of this record is unchanged, and nothing in `Broiler.VM.Abstractions` or
`Broiler.VM.Runtime` changed, so section 10 holds as written. It closes the four gaps JSeal's VM
module adoption records: three `Status` refusals, `import()` from eval and `Function` code, and a
host `Missing` reaching guest code as the uninitialised-binding marker.

**Public additions**, recorded in `docs/api/public-api.txt` (JSD-0012):

| Member | What it is |
|---|---|
| `JsHostRealm.TryGetModuleState(string moduleKey, out JsHostModuleState? state)` | a linked module's state, or `false` when the realm holds no module of that key |
| `JsHostModuleState` (sealed class) | `Key`, `Status`, `HasTopLevelAwait`, `EvaluationError`, `HasEvaluationError`, `CycleRoot` |
| `JsHostModuleStatus` (enum) | `Unlinked` 0, `Linking` 1, `Linked` 2, `Evaluating` 3, `EvaluatingAsync` 4, `Evaluated` 5 |
| `JsFormat.SectionKind.ScriptReferrers` (15), `JsArtifactWriter.ScriptReferrers(...)` | the optional section of 20.3 |
| `JavaScriptDiagnosticCode.MalformedScriptReferrers` (1632) | its structural refusal; diagnostic registry revision 16 |

### 20.1 `TryGetModuleState`: what the realm knows, read rather than inferred

Until this section an embedder read a module's status from evaluation promises (section 15.3,
"Not provided"), which cannot tell a module a failed walk never reached from one it errored, cannot
see a graph a host callback evaluated (deferred to a job, section 15.4), and cannot say which of two
modules that threw equal primitives did so. The realm holds the specification's answer on every
instance, so it now answers it.

- **By key, for every module the realm holds**, whichever route linked it - `LoadModule`, a static
  import, a guest `import()`, an artifact's entry graph. `false` means the realm holds no module of
  that key: never linked here, or its graph did not link (a refused graph registers nothing, section
  15.1), which is how the specification's `~new~` and `~unlinked~` are expressed.
- **What is answered**: `[[Status]]`; `[[EvaluationError]]` as the identical value every later
  evaluation rejects with (`Missing` while there is none, so a module that threw `undefined` is
  distinguishable); `[[HasTLA]]` of the module's own body (a top-level `for await` and
  `await using` included); and the key of `[[CycleRoot]]`, empty until the module completes a walk.
- **When each status is observable.** `Linked` from `LoadModule`'s return until an evaluation
  enters the module, and for good after a failed walk that never reached it. `Evaluating` only
  while a walk is running and has entered the module without completing its component - from host
  code a module body of that walk calls synchronously (a host function, an exotic hook, a loader's
  `OnImport`); the walk runs inside one step. `EvaluatingAsync` between the walk that completed the
  component and the async completion a drain delivers. `Evaluated` from then on, with the error set
  when it failed; a cycle member whose own body ran is `Evaluated` while its root may still be
  `EvaluatingAsync` (the specification's own state), which `CycleRoot` names. An evaluation asked
  for while another is under way - an `EvaluateModule` from a host callback, every guest
  `import()` - runs from a job, so its modules stay `Linked` until a drain. `Linking` and
  `Unlinked` are never answered: linking runs no guest or host code.
- **It runs nothing and charges as a crossing**: `Enter(1)` (one `HostCalls` unit, one unit of
  fuel), `RealmNotCurrent` outside a step or on another thread, a snapshot object the guest never
  sees, and a BigInt evaluation error charged per word (section 19).

### 20.2 `SliceParseOptions.AllowTopLevelAwait` is honoured

The switch existed in the public options and the wide compiler never read it. It is **honoured,
not removed**: removing it would break the public constructor every composition calls, and the
switch has an honest meaning. A module compiled with it `false` has an ordinary module top level,
where `await` is reserved and not an operator, so a top-level `await`, `for await` or
`await using` is refused with the diagnostic the same construct gets in a module's non-async
function (`ReservedWordAsBinding` 2209, `UnexpectedToken` 2101 for `for await`), and the module
carries no `[[HasTLA]]`. `SliceParseOptions.Module` and `new SliceParseOptions(SliceGoal.Module)`
keep it `true`. The four composition call sites that passed `false` together with an explicit
nesting depth (`--max-depth`; in three files: the CLI's two hosts and two in the polyglot lane) now
pass `module`, so their behaviour is unchanged. The slice front end refuses `await` in every position regardless.

### 20.3 The referrer of eval code and `Function` bodies: `GetActiveScriptOrModule`

`import()` in eval code or in a `Function` body was offered to the loader (and the provider) with an
empty referrer, because that code is compiled from a String and its `ImportCall` carries no
compiled referrer. The language resolves it against `GetActiveScriptOrModule()`.

- **The engine tracks the running frame's referrer.** Every frame sets it on entry and restores it
  on exit: a function's frame to the function's `[[ScriptOrModule]]`, a script body's to the
  referrer its artifact placed it at, a module's initialiser and body to the module's key, eval code
  and a `Function` body to the referrer of the code that evaluated them. A built-in - `eval`, the
  `Function` constructor, a host function - enters no frame, so it sees its caller's, which is the
  specification's "topmost context whose ScriptOrModule is not null". With no guest frame and no
  job on the stack the referrer is empty - the specification's null, for which it uses the realm;
  the loader is offered the empty referrer, as before, and resolving it is the embedder's policy.
- **A job runs with the referrer that queued it.** `HostEnqueuePromiseJob` requires the script or
  module active when the job was enqueued to be the active one when it runs, so every queued job -
  a promise reaction, a thenable job, an embedder's `EnqueueJob` callback - keeps the referrer of
  the moment it was queued and the drain restores it around the call. A job whose callable is
  itself `eval` or the `Function` constructor (`p.then(eval)`, `p.then(Function)`) therefore
  resolves against the module or script that queued it; a host job queued with no guest code on the
  stack has none.
- **Functions keep theirs.** A function's `[[ScriptOrModule]]` is fixed at creation
  (`OrdinaryFunctionCreate`), so a `Function` body module code made carries that module's key
  however, and by whom, it is later called; a function eval code made carries the eval's caller's.
- **An `import()` whose operand is empty uses the running frame's referrer**; one with a compiled
  referrer keeps it (it is the same value for code the lowering placed). Code that was compiled
  with no referrer and runs in no placed script still offers the empty referrer.
- **Scripts say where they are placed: a new optional section, `ScriptReferrers` (kind 15).** The
  compiler writes one row per script body compiled with a non-empty `JsScriptUnit.Referrer`: its
  unit and the interned name. The verifier holds each row to a program-body unit that is not eval
  code and not a module's, to a non-empty interned name, and to one row per unit
  (`MalformedScriptReferrers`, 1632, registry revision 16); an artifact without the section places
  nothing, which is what every older artifact says. The section grants nothing; it is a name a
  resolver reads. Seven retained `script-referrers-*` corpus entries (five refusals, two that run)
  were added to `src/tests/corpus/js-1`; no existing entry's bytes changed.
- **The request vocabulary is unchanged**: no request mark, no opcode (none was taken) and no
  source-provider version. The loader still decides only when; the provider still decides what.

### 20.4 `JsHostValue.Missing` never becomes the binding marker

`Unwrap(Missing)` resolved to the realm's empty marker, the value an uninitialised binding holds, so
a host that presented `Missing` where a value belonged injected a dead-zone read into guest code -
and at a host member's own crossing ended the invocation as a `ProfileContractViolation`. Every
crossing now chooses:

| Crossing | `Missing` is |
|---|---|
| `Invoke` / `Construct` arguments | `ArgumentException` (`arguments`); nothing is called |
| `SetProperty` value and target, `DefineValue` / `DefineIndex` value, `GetProperty` / `GetIndex` target | `ArgumentException` (`value` / `target`); nothing is written |
| `NewArray` element | `ArgumentException` (`elements`); this member makes no holes |
| `Throw` | `ArgumentException` (`value`) |
| `ResolvePromise` / `RejectPromise`, `DetachClone` (and its transfer list) | `ArgumentException`, as before this section |
| `Invoke` receiver | `undefined`, as a call with no receiver |
| a host body's return value, an exotic hook's `TryGetNamed` / `TryGetIndex` answer | `undefined` (a construct body's `Missing` keeps the object the realm made, as any non-object does) |
| `ToJsString` / `ToNumber` / `ToBoolean` | converted as `undefined` (`"undefined"`, NaN, `false`) |
| `Invoke` / `Construct` callee, `DetachArrayBuffer` | refused as before (`NotCallable`, `NotAnObject`, the guest `TypeError`) |
| `SetPrototype` prototype, object-only targets (`HasProperty`, `DeleteProperty`, ...) | unchanged: nullish gives a null prototype; a target that is not an object is `NotAnObject` |

The rule is **refuse where the embedder presents a value the realm must hold; `undefined` where the
host answers the guest or a value is only read**, because an `ArgumentException` raised inside a
guest operation would end the invocation rather than reach the embedder that made the mistake.
`Unwrap` itself maps `Missing` to `undefined`, so no route - including one added later - can hand
the guest the marker.

**Executed, not asserted.** The `--host-surface` lane has nine new rows and one extended: two
`Missing` rows (every refusing crossing with its parameter name and nothing written; the receiver,
a body's return, both exotic hooks and a construct body answering `undefined`), three referrer rows
(indirect and direct eval, a `Function` body, an arrow and a `Function` body eval code made, eval
reached through a host function, a `Function` body module code made and the host calls later, a
host-made `Function` with no referrer, eval after a top-level `await`; a host script's own
`import()`, indirect and direct eval, `Function` bodies and a declared function called later; and
`eval` and `Function` called directly by promise jobs module code queued, beside a host job queued
with no guest code on the stack), four
module-state rows (statuses across a walk with a host callback, an awaiting module and its importer,
a throw inside a cycle leaving an unreached dependency `Linked`, a graph a host callback evaluated,
equal primitive errors from modules that can and cannot await, and a cycle member before its
awaiting root), and the lifetime row now refuses `TryGetModuleState` outside a step. The
slice-compiler check `a module compiled without top-level await refuses it by name, and one compiled
with it does not` covers 20.2; the differential probe `the-eval-code-import-referrer.mjs` covers
20.3's guest-visible half against the comparison engine. Every new row the base could compile -
the `Missing` and referrer rows, the check and the probe - failed on it first; the module-state rows
name the new member, so they could not be run against the base.

**What the JSeal adoption changes.** `VmModuleMap` reads `TryGetModuleState(key, out var state)`
instead of inferring status from promise reactions and searching source text for `await`:
`Linked` and `Evaluating` map to its `Linked` and `Evaluating`, `EvaluatingAsync` to
`EvaluatingAsync`, `Evaluated` to `Evaluated` or, with `HasEvaluationError`, `Errored` with
`EvaluationError` as the error; its three `Status` refusals are removed. Its `OfferImport` receives a
module key or its script label for eval and `Function` code too, provided its source provider sets
`JsScriptUnit.Referrer` for host scripts (it does) - the empty-referrer refusal stays only for code
with no script or module on the stack (host code outside any guest frame, or a job such code
queued). It stops passing `Missing` where it means `undefined`.

**Falsified if** `TryGetModuleState` answers a status the instance does not hold, runs guest code,
answers outside a step, or reports an evaluation error that is not the identical value the
module's evaluation rejects with; a module compiled with `AllowTopLevelAwait` false compiles a
top-level `await`, or one compiled with it true refuses one; an `import()` in eval code or a
`Function` body - including one a promise job calls directly - is offered a referrer other than
`GetActiveScriptOrModule()`'s, or a non-empty one where no script or module is on the stack; a `ScriptReferrers` row naming anything but a placed
script body verifies; or `JsHostValue.Missing` reaches guest code as anything but `undefined`.
