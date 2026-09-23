<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0032 - The internal structured-clone carrier: its format, its lifetime, what it costs, and the types it supports

**Status:** Proposed, 2026-09-21. **Owner decision pending.** Unlike the other proposed records
in this series, the code it describes is in the tree: `JsClone.cs`, `JsRealm.Clone.cs` and two
internal members of `JsHostRealm`. They are **internal and advertised nowhere**. Taking the record
would accept the format, the cost model and the matrix below as the basis for cards I17-I18; not
taking it leaves an internal serializer that nothing public reaches. **Amended 2026-09-21 for card
I16:** ArrayBuffer transfer (section 5a), still internal, is in the tree too. **Amended 2026-09-21
for card I17:** the carrier now has one public door, `JsHostRealm.DetachClone` and `AdoptClone`
with the opaque `JsHostCloneCarrier` (section 5b and [JSD-0024](0024-the-in-realm-host-surface.md)
section 17), and the sender pays `LiveBytes` for it (section 4). Still no `WorkerRealms` claim.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It answers cards I14, I15 and I16 of the Broiler.JSeal
integration plan (`docs/roadmap.integration.md` in that repository). Card I14 asks for "an explicit
clone-format/lifetime decision and cost model" before the implementation, card I15 for "a
documented supported-type matrix", and card I16 for transfer "with validation before detachment". The part of this profile's plan that owns the subject is
[section 13](../roadmap.md#13-realms-agents-and-the-host-boundary), which no milestone has
scheduled.

**Context.** The JSeal `WorkerRealms` capability, and `VmRealm.Clone`, `Detach` and `Adopt` behind
it, throw `Lacking(WorkerRealms)` on the VM provider, because this profile has no way to copy a
value graph out of one realm and rebuild it in another. [JSD-0028](0028-shared-memory-and-atomics.md)
keeps `SharedArrayBuffer` excluded and names I14-I18 as the route to a second realm on a second
thread. [JSD-0024](0024-the-in-realm-host-surface.md) is the seam an embedder uses to reach a
realm, and it is the seam this record's two internal members sit on. The realm itself has no
`structuredClone` global, and this record does not add one.

---

## 1. What was true before, verified

| Question | Answer | Where |
|---|---|---|
| Does the realm have `structuredClone`, `postMessage` or `Worker`? | No, all three are `undefined` | JSD-0028 section 1 probe; unchanged |
| Could anything copy a guest object graph out of a realm? | No. The host surface reads one property at a time; JSON is the only whole-graph route and it is not a clone | `JsHostRealm.cs`, `JsRealm.Json.cs` |
| What does the roadmap's proportionality rule say about cloning? | "Structured cloning is the seventh family and it does not ship", so it had no fixture | `ProportionalityChecks.cs` in the slice-compiler root |
| Where are a Date's time value and a RegExp's original source held? | In `DateObject` and `RegExpObject`, types private to `JsRealm` | `JsRealm.Date.cs`, `JsRealm.RegExp.cs` |
| How is an Error branded? | As a plain `JsObject` tagged `Error`; the intrinsic Error prototypes carry the same tag | `JsRealm.Error.cs`, `JsRealm.cs` |
| Are there BigInt values or resizable buffers? | No. `JsType` has no BigInt and `JsArrayBuffer` holds a fixed `byte[]` | `JsValue.cs`, `JsBinary.cs` |

## 2. The decision

1. **The carrier is data.** A `JsCloneCarrier` holds an array of `JsCloneRecord`s and a root
   `JsCloneSlot`. A slot is `undefined`, `null`, a Boolean, a Number (bit for bit, so `-0` and NaN
   survive), a CLR string, or the integer index of a record. A record holds numbers, strings,
   arrays of slots and keys, and, for an `ArrayBuffer`, a **copy** of its bytes. It holds no
   `JsObject`, no `JsValue`, no `JsHostRef` and nothing else that belongs to a realm. The carrier
   carries `FormatVersion = 1`.
2. **Its lifetime is independent of both realms.** Nothing in a carrier refers back to the realm
   that produced it, so disposing that runtime leaves the carrier valid; the checks in section 7
   dispose it and force a collection before adopting. A carrier is not changed after it is sealed
   and **may be adopted any number of times**: every adoption builds new objects and copies the
   buffer bytes again, so two adoptions share nothing a guest can write. **A carrier holding moved
   bytes (section 5a) is the exception: its first adoption claims them, atomically, and every
   later adoption is refused** (`Consumed`). Card I17 made this the public rule (section 5b).
3. **It is internal, and one composition root reaches it through two unsafe accessors.**
   `JsHostRealm.CloneSerialize(JsHostValue, long, long)`, `JsHostRealm.CloneSerializeWithTransfer(
   JsHostValue, JsHostValue[], long, long)` (card I16) and `JsHostRealm.CloneDeserialize` are
   `internal`. Rule A10 forbids `InternalsVisibleTo` in a product project, and rule A11 forbids a
   test project to reference a profile. Reflection was tried first, and rule B5 refused it because
   `MethodBase.Invoke` is on its list of dynamic entry points. So the slice-compiler root's
   `CloneChecks` bind the three members at compile time with `UnsafeAccessorAttribute` and
   `UnsafeAccessorType`, which invoke nothing dynamically and which the trimmer preserves. **That is
   still a door into the profile's internals from outside it, and this record names it as one.**
   B5's list does not cover unsafe accessors, and this record does not claim that B5 approves of
   them. `CloneChecks.cs` is the only file that declares such an accessor. The owner may prefer a
   rule that pins that. The realm's global inventory (rule N17) and the JSeal capability table are
   unchanged. **No `WorkerRealms` claim follows from this record.** *(Card I17:* the public API
   baseline now records the public door of section 5b, which wraps these internals and exposes
   none of the carrier's records; the accessors stay for the narrowed-bound checks.)
4. **Both members run inside a step, like every other member of the host surface.** Each charges
   one host crossing through the JSD-0024 gate. Guest throws reach host code as
   `JsHostThrowException` and aborts are latched, exactly as for `GetProperty`.
5. **A refusal is a CLR-typed outcome inside the profile, and a `TypeError` at the bridge.** The
   serializer raises `JsCloneRefusedException` with a `JsCloneRefusal` reason (`Symbol`, `Function`,
   `Proxy`, `UnsupportedBrand`, `DetachedBuffer`, `TooLarge`, `Unrepresentable`, and for transfer
   `NotTransferable`, `DuplicateTransfer` and `Consumed`). This realm has
   no `DOMException`, so there is no guest value that *is* a `DataCloneError`. The internal bridge
   raises a guest `TypeError` whose message begins `DataCloneError:`. Which guest error a public API
   throws is card I17/I18's decision, and the reason code is kept so that it can decide. *(Card
   I17 decided it: the public door keeps the `DataCloneError:` `TypeError` for everything about the
   guest's value, and turns `Consumed` into the host-side refusal `CarrierConsumed`; section 5b.)*
6. **Destination objects are built from the destination realm's intrinsics.** These are its
   `Object.prototype`, `Array.prototype`, wrapper, Date, RegExp, Map, Set and Error prototypes and
   its typed-array prototype for each element kind. Deserialization runs no guest code: objects
   are fresh, properties are defined rather than assigned, and Map and Set entries are appended to
   the table directly, as HTML's `StructuredDeserialize` does.

## 3. Getter and error behaviour

The serializer is HTML's `StructuredSerializeInternal` over the matrix in section 5, walked with an
explicit stack of frames rather than CLR recursion. It visits in the same depth-first order, so a
program that watches its getters sees the same sequence.

- **The memory map comes first.** An object met before answers with its existing record, so cycles
  terminate and shared references stay shared. An object enters the map when its record is created,
  before anything inside it is serialized.
- **Ordinary objects and Arrays: own enumerable string keys, in `[[OwnPropertyKeys]]` order,
  snapshotted when the record is created.** Each key is re-checked with `HasOwnProperty` before
  `[[Get]]`, so a getter that deletes a later key makes that key disappear from the clone. It is
  not cloned as `undefined`. Getters run, and a throw from one propagates **as the same guest
  value**. Symbol keys, non-enumerable properties and prototypes are not carried. A class instance
  clones as a plain object. An Array's `length` is read when its record is made, so holes and a
  length past the last element survive, and a sparse Array costs what it holds, not its length.
- **Map and Set entries are snapshotted when the record is created** (HTML's `copiedList`), then
  serialized key then value, in order.
- **Errors.** `name` is read with `[[Get]]` (which may run a getter). A name outside `Error`,
  `EvalError`, `RangeError`, `ReferenceError`, `SyntaxError`, `TypeError` and `URIError` becomes
  `Error`, and so do `AggregateError` and any name that is not a string. `message` is carried only as an own
  **data** property, converted with ToString (which may run guest code). An accessor or an
  inherited message clones as no own message. An own data `cause` is serialized like any other
  value. HTML does not name `cause`, but it lets a user agent attach "interesting accompanying data"
  and the engines this profile is compared against attach it. Own enumerable properties, `stack`
  and `errors` are not carried. The clone's `message` and `cause` are non-enumerable, writable and
  configurable.
- **The intrinsic Error prototypes are not Errors.** In the specification they hold no
  `[[ErrorData]]`, so `Error.prototype` clones as an ordinary object.

## 4. The cost model

| Work | Charged to | Amount |
|---|---|---|
| Each crossing of the bridge | source or destination instance | one `HostCalls` unit and one fuel unit (JSD-0024) |
| Each object record serialized | source | 4 fuel |
| Enumerating an object's keys | source | one fuel per own key, plus one |
| Each key read, and each Map, Set or cause value | source | one fuel, plus whatever a getter or ToString costs as guest code |
| Walking a Map or Set table | source | one fuel per slot, tombstones included |
| An ArrayBuffer's bytes | source | one fuel per byte, charged before the copy |
| Rebuilding | destination | one fuel per record plus one, then 4 per record and 4 per view, one per property or entry |
| A destination buffer | destination | one fuel per byte before allocation, and the bytes reported to `LiveBytes` after it |
| Each transfer-list entry (I16) | source | 4 fuel and one entry, before the walk |
| A transferred buffer's bytes (I16) | source: none (they move, not copy); destination: as any destination buffer | counted against `MaxBytes` at validation, before any detachment |
| The carrier itself, in flight (I17) | source `LiveBytes` | 32 bytes per entry plus every copied buffer and string byte (not moved bytes), with `TryCharge` after validation and before any detachment |

**The carrier is bounded while it is built.** `MaxEntries = 2^22` bounds the carrier's entries,
which are its records plus its value slots (one per property value, Array element, Map key or value,
Set value and Error cause); a property key is not a separate entry, because it travels with its
slot. `MaxBytes = 2^28` bounds buffer bytes plus two bytes per character of each string carried,
where a string is counted once per distinct CLR string instance (`ReferenceEqualityComparer`): one
instance named many times is counted once, and equal text held in two instances is counted twice,
which only makes the bound stricter. Passing either bound is a `TooLarge` refusal raised at the
entry or string that passes it, before that entry is added to the carrier and before any buffer
copy that would pass it. The bound covers the carrier, not the walk's own scratch: an object's
own-key list and a Map's or Set's snapshot list are built in full when its record is made, before
their entries are counted. Those lists are proportional to an object the guest already holds in
its own budget, and each of their entries is charged fuel before it is counted. The internal serializer
accepts narrower bounds and clamps wider ones, which lets a check reach the refusal with a graph of
a hundred entries. **Who pays for the carrier (card I17): the sender.** It was host memory reported
to no instance's `LiveBytes`; now the serializer reports `JsCloneCarrier.ChargeFor(entries, copied
bytes)` to the sending instance with `RetainOrAbort` after the list and graph checks and before the
commit loop, so a ceiling that refuses it ends the operation with every listed buffer attached.
Moved bytes are excluded because the sender retained them when it made the buffers and is not
credited when they leave. The charge is never credited while the sending instance lives (no
retention in this profile is) and is released with the instance's other live bytes when it is
disposed; from then on a carrier the host still holds is host memory bounded by the two maxima and
by no budget. That residue is the accepted limitation. Charging the receiver instead was rejected:
a carrier may never be adopted, or adopted many times, and the realm that decided to make it is the
one whose budget should stop it. The slice-compiler check `clone/i14/the-clone-is-charged-in-proportion-to-the-graph`
measures the charge by bisecting the fuel allowance, the method `ProportionalityChecks` uses.
Removing the walk's charges makes it fail.

## 5. The supported-type matrix

| Value | Status | Carried | Rebuilt as |
|---|---|---|---|
| `undefined`, `null`, Boolean, Number (`-0`, NaN, infinities), String | supported (I14) | the value | the value |
| Symbol | **refused** (`Symbol`) | - | - |
| BigInt | supported (B06, 2026-09-22) *(was: not in the realm; then, from card B05, in the realm and **refused**, `Unrepresentable`)* | the exact integer, in the `BigInt` slot kind; it counts eight bytes per 64-bit word against `MaxBytes` and is charged per word on both ends, each occurrence separately | the same integer. A realm whose composition declined `broiler.javascript.bigint` refuses the whole carrier (`Unrepresentable`, a `DataCloneError` at the public door) **before** it claims moved bytes, so a single-use carrier it refused is still whole |
| BigInt wrapper object (`Object(1n)`) | supported (B06, 2026-09-22), HTML's `[[BigIntData]]` row | the wrapped integer only, as the one BigInt slot of a `BigInt` record | a BigInt object on the destination `BigInt.prototype` (class `Object`, tag from the prototype); objects the source shared are shared again |
| Ordinary object (any plain `JsObject` not listed below, including class instances and ordinary prototypes such as `Map.prototype` and `Error.prototype`) | supported (I14) | own enumerable string-keyed properties via `[[Get]]` | a plain object on the destination `Object.prototype` |
| Array (dense, holey, sparse; `Array.prototype` too) | supported (I14) | `length` and own enumerable string-keyed properties | an Array of that length on the destination `Array.prototype` |
| Boolean, Number, String wrapper objects | supported (I15) | the wrapped primitive only | a wrapper of the same kind |
| Symbol wrapper object | **refused** (`Symbol`) | - | - |
| Date | supported (I15) | the time value | a Date |
| RegExp | supported (I15) | original source and normalized flags; `lastIndex` and own properties are **not** carried | a RegExp recompiled from them, `lastIndex` 0 |
| Map, Set | supported (I15) | live entries in order, snapshotted | a Map or Set with those entries appended in order |
| Error and the six native subtypes, and their subclasses | supported (I15) | the name (section 3), the own data message, the own data cause | an Error on the destination prototype for that name |
| AggregateError | supported as `Error` | as above; `errors` is not carried | an `Error` |
| ArrayBuffer (fixed length, attached) | supported (I15) | a copy of the bytes | a new buffer holding a new copy |
| ArrayBuffer, detached | **refused** (`DetachedBuffer`) | - | - |
| Typed array or DataView over a detached buffer | **refused** (`DetachedBuffer`) | - | - (HTML: `IsArrayBufferViewOutOfBounds`). The view is asked itself whenever it is met, so a getter that detaches a buffer the memory map already recorded cannot leave a later view cloned over the bytes copied before |
| ArrayBuffer (fixed length, attached) **in the transfer list** | supported (I16), section 5a | the source's own bytes, moved; the source is detached | a new buffer holding those bytes, once: the carrier is single-use |
| Transfer-list entry that is not an ArrayBuffer (a view, an ordinary object, a primitive) | **refused** (`NotTransferable`), before any getter runs | - | - |
| The same ArrayBuffer listed twice | **refused** (`DuplicateTransfer`), before any getter runs | - | - |
| Listed ArrayBuffer already detached, or detached by a getter during the walk | **refused** (`DetachedBuffer`), after the walk, before any detachment | - | - |
| Typed array or DataView over a transferred buffer | supported (I16) | as any view, naming the transferred buffer's record | a view over the one destination buffer; every source view observes the detached state |
| Resizable ArrayBuffer, and a typed array or DataView over one | **refused** (`UnsupportedBrand`, "a resizable ArrayBuffer could not be cloned"; since 2026-09-21, JSeal F06) | - | resizable buffers are in the realm now (F04-F06) and the carrier still cannot express them. To be supported, the record must carry the maximum byte length, and a length-tracking view must carry "tracks length" instead of a fixed count, as HTML's serializer does. Until both are implemented, a resizable buffer is refused and not cloned as a fixed one; a view over one is refused before its offset or length is read |
| SharedArrayBuffer | **not in the realm** (JSD-0028) | - | if it is ever admitted it shares bytes rather than copying them, which this carrier cannot express and must refuse |
| Typed arrays of every element kind the realm has | supported (I15) | the element kind by constructor name, byte offset, length and the buffer's record | a view over the one rebuilt buffer. Views that shared a buffer share one again. A kind the destination lacks is refused (`Unrepresentable`). This is how `Float16Array` and the BigInt kinds enter: by being in `JsElements.All` on both ends. *(Amended 2026-09-22, JSeal B07: `BigInt64Array` and `BigUint64Array` now enter exactly this way, row `clone/b07/bigint-views-clone-by-kind-and-share-one-buffer`; a destination realm whose composition declines BigInt has neither kind and refuses the name.)* |
| DataView | supported (I15) | byte offset, byte length and the buffer's record | a DataView over the one rebuilt buffer |
| Functions of every kind, including classes, bound functions and built-ins | **refused** (`Function`) | - | - |
| Proxy, callable or not | **refused** (`Proxy`), whatever its target | - | - |
| WeakMap, WeakSet, WeakRef, FinalizationRegistry, Promise, generator objects, async generators, module namespaces, host objects | **refused** (`UnsupportedBrand`) | - | - |
| Arguments objects, built-in iterators (array, string, Map, Set and RegExp-string iterators, iterator helpers) | **refused** (`UnsupportedBrand`). They are plain objects in this implementation, so the refusal is by tag: they hold slots in the specification, so HTML refuses them | - | - |
| DOM objects, platform objects of an embedder | **out of scope** (card I15 exclusion). Host objects are refused, as above | - | - |

**Amended 2026-09-22 (JSeal B06).** The carrier layout is version 2 (`JsCloneCarrier.FormatVersion`,
`JsHostCloneCarrier.CurrentFormatVersion`): a slot kind `BigInt` (6) and a record kind `BigInt` (13)
were added, and the carrier records whether it holds a BigInt so that a declining destination can
refuse it before anything is claimed or built. The slot keeps its size: the String and the BigInt
share one reference field. A `JsBigInt` is immutable and names no realm, so a carrier shares it with
every destination exactly as it shares a CLR string, and the rule of section 2 - no reference into
the producing realm - still holds. Checks: `clone/b06/a-bigint-and-a-bigint-object-clone-by-value`,
`clone/b06/a-shared-bigint-object-stays-shared-and-keys-survive`,
`clone/b06/a-bigint-counts-its-words-against-the-byte-bound`, and the host-surface lane's
"a realm that declined BigInt refuses a BigInt from the host and a carrier holding one, and claims
nothing". The BigInt typed-array kinds entered with card B07 on 2026-09-22 through the typed-array
row: a view names its kind by constructor, and a realm without the kind refuses the name.

## 5a. Transfer (card I16)

`CloneSerializeWithTransfer(value, transfer, ...)` is HTML's `StructuredSerializeWithTransfer` for
the one transferable this realm has, the fixed-length `ArrayBuffer`, with one deliberate change of
order.

1. **The list is checked before the graph is walked**, as HTML does: each entry must be an
   `ArrayBuffer` (`NotTransferable` otherwise; this realm has no `SharedArrayBuffer` and no
   transferable platform object) and none may appear twice (`DuplicateTransfer`). Each is entered
   in the memory map with its own record, marked transferred, so no getter has run yet when either
   refusal is raised.
2. **The graph is walked** exactly as in section 3. A listed buffer met in the graph answers with
   its record, so every reference to it and every view over it names the one destination buffer.
   A listed buffer not reachable from the value is still transferred, as in HTML.
3. **Every listed buffer is checked again**: still attached (a getter may have detached it:
   `DetachedBuffer`), and its bytes counted against `MaxBytes` (`TooLarge`). Moved bytes count
   because the carrier holds them.
4. **Only then are the buffers detached**, in one loop that charges nothing, counts nothing and
   cannot throw. Each record takes its source's array (no copy); every source view observes a
   detached buffer from that instant.

**The deviation from HTML.** HTML detaches each buffer as it passes step 3, so a later entry that
fails leaves the earlier ones detached. Card I16 requires that a failure "does not prematurely
detach source storage", so here every check precedes every detachment: a refusal, a guest throw or
an abort (fuel, cancellation) anywhere in steps 1-3 leaves every listed buffer attached and
usable. On success the answer is the same as HTML's.

**Adoption.** A carrier with transferred records is single-use: `CloneDeserialize` claims it with
an atomic exchange before building or charging anything, and every later adoption is refused
(`Consumed`); a claim is final even if the adoption that made it then fails. The destination
buffer is built from the moved bytes by one copy, charged per byte and reported to `LiveBytes` like
any rebuilt buffer, and the record then drops its reference to them. The copy is there because
`JsArrayBuffer` has no constructor that adopts an existing array and cards F04-F06 are reshaping
that type; handing the array over without a copy is a later optimisation with no observable
difference. The source realm's `LiveBytes` is not credited when its buffer is detached, which is
how `ArrayBuffer.prototype.transfer` already behaves.

**Not covered:** `SharedArrayBuffer` (excluded, JSD-0028), transferring any host object (card I16
exclusion), resizable buffers (row above), and the guest-facing conversion of an iterable into a
transfer list, which is the caller's step as WebIDL's sequence conversion is in HTML.

## 5b. The public door (card I17)

`JsHostRealm.DetachClone(value, transfer)` is `CloneSerializeWithTransfer` at the carrier's own
bounds, answering an opaque `JsHostCloneCarrier`; `JsHostRealm.AdoptClone(object)` is
`CloneDeserialize` behind a compatibility check. Both are JSD-0024 crossings: inside a step, on the
realm's own thread, charged, latching aborts. The carrier crosses threads through any synchronising
handoff; it is not written after it is sealed except for the atomic claim. **Lifetime:** valid
after its source is disposed, for as long as the host holds it; no byte form. **Repeatable** when it
holds no moved bytes, from any number of realms and threads; **single-use** otherwise, the claim
made atomically before anything is built, a racing or later adoption refused as `CarrierConsumed`
(a `JsHostSurfaceException`). **Compatibility:** a `JsHostCloneCarrier` of this build whose `Profile`
is `broiler.javascript` and whose `FormatVersion` is 1; any other object, including another
engine's payload and a carrier from a second copy of this assembly, is `ForeignCarrier`. JSD-0024
section 17 gives the rest, and the `--host-surface` lane executes it on distinct host threads.

**No brand is flattened.** A value either has a row with its own record kind or is refused. The
check `clone/i15/no-brand-is-flattened-into-an-ordinary-object` compares
`Object.prototype.toString` of every supported brand before and after.

## 6. What was considered and rejected

- **A JSON round-trip.** It loses `undefined`, `-0`, NaN, holes, cycles, shared references and
  every brand. Card I14 excludes it by name.
- **A carrier of live handles** (`JsValue`s or `JsHostRef`s resolved on adoption). It could not
  outlive its source, could not cross a thread, and would let a destination reach source objects.
  Card I14 asks for the opposite.
- **Sharing the source's prototypes or reusing its intrinsics.** Card I14 excludes guest prototype
  sharing, and a realm has no business holding another's `Object.prototype`.
- **A recursive CLR walk.** A 200,000-node chain would overflow the host stack in a place no budget
  governs. The explicit frame stack keeps HTML's order at heap cost, and a check clones such a chain.
- **Exposing it now**, as a public `JsHostRealm` API or as a guest-visible hook such as a `$262`
  member. `$262` is installed by the realm's own global setup (`JsRealm.Global.cs`), so a JSeal realm
  carries it too, and a hook there would be a guest-visible clone with a partial matrix. A public API would be a contract
  before I16-I17 decide transfer, threads and single use. `InternalsVisibleTo` is forbidden by
  rule A10, and reflective invocation from a composition by rule B5.
- **Throwing a guest `TypeError` from inside the serializer.** That would decide the public error
  shape inside the profile and lose the reason, which a later public surface needs.

## 7. Evidence in the tree

`CloneChecks` in `Broiler.VM.Composition.JavaScript.SliceCompiler` (run by `--checks`, which the
lane workflow runs). There are twenty-six checks, each judged by the guest program's own answer.
Primitives; key order and filtering; getter order and the deleted-key rule; a getter's throw
arriving as the same value; cycles and shared references; holes, length and sparse arrays; mutation
in both directions; the refusal list; a 200,000-node chain; each brand of section 5 with its value,
identity and refusal cases; every brand referenced twice keeping one identity, and each container
brand refusing an unsupported member; the no-flattening check; narrowed bounds; a carrier adopted twice by a
second runtime after the first was disposed, with prototypes checked against the destination's
own; and the proportionality measurement. Disabling the memory map fails five of them, and
removing the walk's fuel charges fails the proportionality check. Card I16 added five: bytes move
and every source view observes detachment while aliases and views share one destination buffer;
a refused list or graph (duplicate, view, object, primitive, detached entry, unsupported member,
getter-detached entry, getter throw) detaches nothing and runs no getter when the list itself is
wrong; moved bytes count against the byte bound before anything is detached; a carrier holding
moved bytes outlives its disposed source and is adopted once; and the resizable-buffer tripwire.
Detaching each buffer as it passes validation (HTML's order) fails two of them, and removing both
the claim and the record guard fails the single-use check.

## 8. Follow-ups this record leaves open

- **I16:** done internally (section 5a), except resizable buffers, which do not exist at this base
  and must be refused when F04-F06 add them.
- **I17:** done locally (section 5b, JSD-0024 section 17): the public carrier, its version and
  foreign-carrier rule, the thread contract exercised on distinct host threads, single-use and
  repeated adoption, and the sender paying `LiveBytes`. Left: a carrier held after its sender is
  disposed is outside every budget.
- **I18:** JSeal's `VmRealm.Clone`/`Detach`/`Adopt` and the `WorkerRealms` capability.
- The Error-brand test is a tag check on plain objects, as is `Object.prototype.toString` here. A
  dedicated `[[ErrorData]]` representation would make both exact.

## 9. What would falsify this

- A carrier that holds any `JsObject`, `JsValue` or `JsHostRef`, or an adoption that fails or
  answers differently after the source runtime is disposed.
- A clone whose prototype, or any object reachable from it, belongs to the source realm.
- A value outside section 5's supported rows that serializes rather than being refused, or a
  supported brand whose clone reports a different `Object.prototype.toString` tag.
- A cycle that does not terminate, or one source object that yields two destination objects.
- A serialization or deserialization whose fuel charge does not grow with the graph, or a carrier
  larger than its two bounds.
- A refused or aborted transfer that leaves any listed buffer detached, a completed one that leaves
  any listed buffer attached, two destination buffers for one transferred source, or a carrier with
  moved bytes adopted twice.
- (I17) A public carrier that exposes its records or a realm handle, an adoption that succeeds for
  an object that is not this build's carrier or outside the receiving realm's step and thread, two
  racing adoptions of a single-use carrier that both succeed, or a sending instance whose
  `LiveBytes` does not grow by `ChargedBytes` when a carrier is made.
