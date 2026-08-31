# The composition register

**Owner:** Broiler.VM architecture and developer-experience owner
**Core contract version:** 1
**Milestone:** VM-3
**Closes:** Exclusion EX-08 of ADR 0001

This is the register ADR 0001 names and defers to VM-3: the schema for a
composition, the advertised set, the demonstration set, and the runtime
identifiers each composition has actually been published and run for.

It exists because the composition-root allow-list rule A11 reads was an empty
constant inside the architecture-test project. An empty constant is maximally
strict and completely unreviewable: relaxing it is a code change nobody outside
the test project sees. From VM-3 the allow-list is a path - every project under
`src/compositions/` - and what each of those projects is allowed to contain is
this document, held to the checkout by rules K1 and K2 in both directions.

---

## 1. The advertised set is empty

**At core contract version 1, Broiler.VM advertises no composition and ships no
language profile.**

That sentence is the whole of the support position and it is not a placeholder.
An advertised composition is one a consumer may take a dependency on: it has a
package identity, a supported RID set, a compatibility promise across core
contract versions, and a named owner who answers for it. None of those exist,
and publishing a composition before they do would be the untruthful support
claim roadmap section 16 makes a stop condition.

Consequences that follow from the set being empty, rather than being separate
decisions:

- Every composition root in the checkout is **non-packable**. ADR 0001 requires
  it of any root this register does not list as advertised, and this register
  lists none.
- The packable set stays exactly the three product packages:
  `Broiler.VM.Abstractions`, `Broiler.VM.Binary`, `Broiler.VM.Runtime`.
- No RID in section 4 is a supported RID. It is a record of what was published
  and run, on one machine, once. Claiming a RID is a release act and this
  document performs none.

---

## 2. The schema

A row in the composition table declares, in this order:

| Field | Meaning |
|---|---|
| Composition | The assembly name of the composition root. Its project lives at `src/compositions/<name>/<name>.csproj`. |
| Kind | `advertised` or `demonstration`. An advertised composition is packable and carries a support promise; a demonstration composition is neither and exists to be published and run as evidence. |
| Profiles | The profile IDs the composition registers, in the order its catalog builder names them. Exactly these, and no others. |
| Profile assemblies | The assemblies those profiles come from. This is what a closure report must contain beyond the three core assemblies, and nothing else. |
| Host capabilities | The capability IDs the root registers, with whether the composed profiles import them. |
| Guest-initiated loads | Whether the root registers an artifact provider. A composition that registers none refuses every guest-initiated load deterministically, which is what a content policy is. |

Two things the schema deliberately does not have. There is no "profiles
available but not registered" column, because composition as a runtime option
that disables a linked profile is forbidden outright: a disabled-but-linked
profile appears in the closure anyway and makes every closure report untruthful.
And there is no aggregate profile-listing type anywhere - no `AllProfiles`, no
`KnownProfiles` - because one would reference every profile assembly and defeat
the exact closure this register exists to describe.

---

## 3. The compositions

| Composition | Kind | Profiles | Profile assemblies | Host capabilities | Guest-initiated loads |
|---|---|---|---|---|---|
| `Broiler.VM.Composition.Calculator` | demonstration | `com.example.calculator` | `Com.Example.Calculator` | `com.example.host.unreachable` (imported by no composed profile) | none registered |
| `Broiler.VM.Composition.Workbench` | demonstration | `com.example.calculator`, `com.example.ledger` | `Com.Example.Calculator`, `Com.Example.Ledger` | `com.example.ledger.stamp` (optional import of `com.example.ledger`; the calculator imports nothing) | none registered |

**Why the single-profile root registers a capability nothing imports.** It is
the demonstration that registering a capability never implies a provider. The
calculator declares no import, so its binding table has zero slots and the
registered handler is unreachable from the guest whatever the host does. A host
that had to curate its registrations per composed profile would be doing the
core's containment work by hand, and getting it wrong would be silent.

**Why the two-profile root states three ceilings explicitly.** Adopting the
profile default resolves to the *tightest* default in the catalog. That is a
catalog-wide fact rather than a per-profile one, so the calculator's numbers
would otherwise decide what the ledger may do: the calculator's defaults write
`1` for section count and `0` for host calls, the ledger frames two sections and
imports a stamping capability, and a runtime adopting throughout would refuse its
artifact for want of a second section. The root therefore states `HostCalls`,
`SectionCount` and `StructuralDepth` itself. This is not a way around the
calculator's limits - the effective ceiling for an operation is the intersection
of the host's with **that profile's own** hard maxima, so the calculator is still
held to one section and no host call - and the profile-authoring consequence is
recorded in section 5.

*Corrected 2026-08-31, and this paragraph contradicted section 5 until it was.*
It also said a runtime ceiling was "additionally clamped to the tightest profile
hard maximum in the catalog", and called both terms catalog-wide. Only the
default is. The maximum clamp was an implementation defect rather than a property
of the contract - ADR 0007 puts `ProfileMax` at P2, against the profile an
artifact names - and it has been removed. **The reason this root states three
ceilings is unchanged**, because it was always the default fold that produced the
refusal; what changed is that a reader of this section is no longer told the
maxima do it too.

---

## 4. Published and run

Neither row is a supported RID. This is what was actually built and executed
when the VM-3 evidence bundle was collected.

| Composition | RID | JIT | Trimmed, self-contained | Native AOT |
|---|---|---|---|---|
| `Broiler.VM.Composition.Calculator` | `linux-x64` | published and run | published and run | published and run |
| `Broiler.VM.Composition.Workbench` | `linux-x64` | published and run | published and run | published and run |

The transcripts are in `docs/evidence/vm-3/`, and the closure report for each
mode is listed there from the published output rather than described. One RID,
one machine, one lane: exclusion EX-45 of the VM-2 bundle applies unchanged.

---

## 5. What a profile author should take from this

Three things surfaced while composing two unlike profiles that are properties of
the contract rather than of these two profiles, and all three are cheap to get
wrong.

**A hard maximum is a statement about you, and an adopted default is a statement
about your neighbours.** These were one paragraph until 2026-08-31, and they are
two rules with opposite reach.

Your hard maximum binds *your* artifacts and nobody else's. It is applied at
verification, against the profile the artifact names, so declaring a tight one
constrains only what you accept. It used to clamp every runtime ceiling in a
shared catalog - one section because you frame one, zero host calls because you
make none, and every profile beside you held to that - and that was a defect, now
corrected. If you read this register before that correction and loosened your
maxima on its advice, nothing you did is now wrong; it is simply no longer
required.

The *default* you declare is still catalog-wide. A host that adopts profile
defaults rather than stating numbers gets the tightest in the catalog, because at
runtime creation no profile has been selected and there is no other safe answer.
So a stingy default is what now reaches your neighbours, and a host that wants
more states an explicit ceiling - which is what the two-profile composition below
does for the three dimensions where it mattered. Declare a maximum for what you
would tolerate being granted, and a default for what you actually need.

**An optional import is a per-runtime binding, not a property of the profile.**
The same profile, over the same artifact, answers differently in a runtime whose
host registered the capability and one whose host did not - and both are correct
answers. Write the unbound branch first: it is the one a host's policy can force
on you at any time, and `IsBound` is the whole of what you may ask.

**Defaults clamp catalog-wide, and the dimension you never use is the one to
watch.** Since the maxima clamp was removed this is the whole of the catalog-wide
reach, and it is the half that gets missed: adopting a profile default resolves
to the tightest default in the catalog. The fold reads every dimension of every
descriptor with no exemption for the ones a profile declares inapplicable. So a
profile writing `0` into a guest-load **default** because it has no guest loads
hands a host that adopts defaults a ceiling of zero, and the failure surfaces in
somebody else's verifier as a refusal naming a dimension they never touched.

**And there is no costless spelling of "I do not constrain this".**
`VmDescriptorValidation` refuses any descriptor whose `LimitDefaults` carries an
unconstrained slot, with reason `LimitDefaultsInvalid`, because a default meaning
unbounded would make adopting it identical to declaring no ceiling at all -
invariant 9's rule that omission never means unbounded. Hard maxima may use
`Unconstrained`; defaults may not. **So declare a large finite default on a
dimension you do not use, say why in the record, and state plainly that it still
participates in the fold** - the exposure is bounded, not removed. An earlier
version of this paragraph advised declaring `Unconstrained` and was wrong: that
descriptor does not build.

**Prove it rather than asserting it.** The way to show you have not mis-declared
a catalog-wide default is a two-profile catalog test with a deliberately adverse
neighbour - a descriptor that tightens the dimensions you do not declare and
writes a stingy default into one you do not use - asserting that the neighbour's
maxima do not reach your artifacts at all, and that its adopted defaults do. Both
intended first profiles build exactly this at their first milestone. It is a test
shape, not a shared asset: nothing is extracted to run it.

---

## 5a. A composition this register does not yet have

Both intended first profiles are written against this core, and the product that
consumes them first is a browser, which needs **both at once**. No such
composition exists, and this register deliberately has no row for it: rule K1
fails on a row naming a root that does not exist, so an anticipated composition
is recorded here in prose or not at all.

What is worth fixing now is who owes what, because none of it belongs to either
profile:

- **The closure, the RID matrix and the Native AOT evidence are the composing
  component's**, not the union of two profiles' evidence. One profile's roadmap
  calls a browser its *largest* closure because it links a lowering; the other
  calls a browser its *smallest* because it compiles nothing. Both are true of
  themselves and neither describes the image, which is the union plus the core.
- **The two profiles reach each other through their declared *defaults***, per
  the paragraph above, on all fifteen dimensions - and through their maxima not
  at all, since that clamp was removed on 2026-08-31. Reconciling two
  independently owned sets of defaults is the composing component's job and
  nothing in either profile can do it. A host that states explicit ceilings never
  meets the fold; a host that adopts defaults always does.
- **A call chain that crosses runtimes is bounded only under one shared parent.**
  Cross-runtime reentry is legal and depth-bounded, and it is the route a
  browser's cross-profile seam takes - but the bound is an aggregate one, so a
  composition root that creates two runtimes without a shared aggregate budget
  has no bound on the chain at all. Create one.
- **No cross-profile value channel exists or is coming.** A guest-initiated load
  may not name another profile; the provider must answer with an artifact of the
  profile that asked. The seam is the embedder's, every call across it is two
  host-boundary transits, and a shared mutable region has no core representation.

Until that component exists, the honest position is the one section 1 already
takes: nothing here is advertised, and a browser is not a composition this
register describes.

---

## 6. Changing this register

Adding a composition root means adding a row here, adding the project to
`graph.manifest.json`, and amending ADR 0001's project budget if the count
changes. Rule K1 fails if a root exists with no row or a row names a root that
does not exist; rule K2 fails if a row's profile list and the composition's own
reference set and closure report disagree. Making a composition advertised is a
release decision that ADR 0012 owns, not an edit to the Kind column.
