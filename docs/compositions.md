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
| Sibling assemblies | Assemblies the root links that are neither core, nor the root, nor a profile - a profile's own siblings, of which a lowering is the first. `none` where there are none. |
| Evidence | The bundle directory holding this composition's retained catalog table and closure report. Rules K3 and K4 read it. |

**A sibling is in the image and need not be in the project file.** The format
assembly arrives transitively through the profile and appears in no composition
root's references, while being unmistakably in every published closure - so rule
K2 requires every REFERENCED assembly to be declared and every declared PROFILE
to be referenced, and does not require a declared sibling to be. The execution-only
root's sibling cell therefore names the format and not the lowering, which is the
distinction the whole label rests on.

**The sibling column was added on 2026-08-31 because the schema had no place for
a lowering.** A profile's lowering is in the closure of a compiler-bearing root
and comes from no profile at all, so before this column the only way to make such
a root pass rule K2 was to declare the lowering in the Profile-assemblies cell -
as though a profile came from it - which would then have made the catalog table
and the register disagree. The two claims are now separate: what a profile comes
from, and what else is in the image. **The execution-only root's cell reads
`none`, and that is the whole of the execution-only label.**

**The evidence column was added on 2026-08-31 and it is not decoration.** K3 and
K4 used to read one bundle - the core's current one - which was right while every
composition belonged to the core. This repository now holds two milestone series,
and the JavaScript profile's roots keep their evidence in the profile's own
bundle tree; a rule that read the core's bundle for them would either fail or,
worse, compare a JavaScript closure against a file nobody wrote for it. Naming
the bundle per row keeps the two ledgers apart while one rule still holds every
composition to its own evidence. **A core milestone bump has to move the two
`docs/evidence/vm-6` cells**, and that is deliberate: the register is a reviewed
document, and a cell that moved without anyone reading it is what a literal
inside a test would have been.

Two things the schema deliberately does not have. There is no "profiles
available but not registered" column, because composition as a runtime option
that disables a linked profile is forbidden outright: a disabled-but-linked
profile appears in the closure anyway and makes every closure report untruthful.
And there is no aggregate profile-listing type anywhere - no `AllProfiles`, no
`KnownProfiles` - because one would reference every profile assembly and defeat
the exact closure this register exists to describe.

---

## 3. The compositions

| Composition | Kind | Profiles | Profile assemblies | Sibling assemblies | Host capabilities | Guest-initiated loads | Evidence |
|---|---|---|---|---|---|---|---|
| `Broiler.VM.Composition.Calculator` | demonstration | `com.example.calculator` | `Com.Example.Calculator` | none | `com.example.host.unreachable` (imported by no composed profile) | none registered | `docs/evidence/vm-6` |
| `Broiler.VM.Composition.Workbench` | demonstration | `com.example.calculator`, `com.example.ledger` | `Com.Example.Calculator`, `Com.Example.Ledger` | none | `com.example.ledger.stamp` (optional import of `com.example.ledger`; the calculator imports nothing) | none registered | `docs/evidence/vm-6` |
| `Broiler.VM.Composition.JavaScript.ExecutionOnly` | demonstration | `broiler.javascript` | `Broiler.VM.Profile.JavaScript` | `Broiler.VM.Profile.JavaScript.Format` | `broiler.javascript.write` (optional import of `broiler.javascript`) | none registered | `src/Broiler.VM.Profile.JavaScript/docs/evidence/js-1` |
| `Broiler.VM.Composition.JavaScript.SliceCompiler` | demonstration | `broiler.javascript` | `Broiler.VM.Profile.JavaScript` | `Broiler.VM.Profile.JavaScript.Format`, `Broiler.VM.Profile.JavaScript.Compiler` | `broiler.javascript.write` (optional import of `broiler.javascript`) | none registered | `src/Broiler.VM.Profile.JavaScript/docs/evidence/js-1` |
| `Broiler.VM.Composition.JavaScript.Android` | demonstration | `broiler.javascript` | `Broiler.VM.Profile.JavaScript` | `Broiler.VM.Profile.JavaScript.Format` | `broiler.javascript.write` (optional import of `broiler.javascript`) | none registered | `src/Broiler.VM.Profile.JavaScript/docs/evidence/js-android-001` |
| `Broiler.VM.Composition.JavaScript.Conformance` | demonstration | `broiler.javascript` | `Broiler.VM.Profile.JavaScript` | `Broiler.VM.Profile.JavaScript.Format`, `Broiler.VM.Profile.JavaScript.Compiler` | `broiler.javascript.write` (optional import of `broiler.javascript`) | `broiler.javascript.write` | `src/Broiler.VM.Profile.JavaScript/docs/evidence/js-3a-004` |
| `Broiler.VM.Composition.JavaScript.Cli` | demonstration | `broiler.javascript` | `Broiler.VM.Profile.JavaScript` | `Broiler.VM.Profile.JavaScript.Format`, `Broiler.VM.Profile.JavaScript.Compiler` | `broiler.javascript.write`, `broiler.javascript.source-provider` (both optional imports of `broiler.javascript`) | `broiler.javascript.source-provider`, answered by this root's own compiler | `src/Broiler.VM.Profile.JavaScript/docs/evidence/js-3b-001` |

**The Android head composes exactly what the execution-only root composes**, and
that is the point of it rather than an accident: it names the profile and not the
lowering, so the execution-only property travels onto a device unchanged. What
differs is the target framework, which is the only way an Android RID can be
published at all, and the checks it runs - the corpus replay and the ordering
assertions, compiled from the execution-only root's own source rather than
re-implemented. It runs neither the soak nor the fuzz sessions: those are
wall-clock and heap-shaped, and an emulator is neither a machine nor a stable
one. **Its evidence is a collection taken on an emulator**, which the bundle's
own exclusions say in those words: an emulator is not a device, and the RID that
ran is `android-x64` because that is the emulator's architecture.

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

**The conformance harness is the one row in this table whose non-advertisement is
a rule rather than a consequence.** Every other root here is a demonstration
because section 1's advertised set is empty; this one would have to stay out of
that set even if the set filled. It is the ingestion path for a conformance
suite — separately licensed third-party material that a human has still not
retrieved, hashed or archived — and roadmap section 14 asks the property to be
asserted rather than assumed. Rule N13 is that assertion, and it is deliberately
**not** phrased as "appears in no published closure": this root publishes a
closure of its own, for its own evidence, so that phrasing would be falsified by
the very bundle the row's Evidence column names. What N13 asserts is that the
harness appears in **no package and in no advertised composition's closure**,
that no other project references it, and that no project file names a suite
directory — the last being how suite *files* would reach a build output with the
dependency graph still looking clean. Its negative control adds the reference
from the execution-only root, which is the direction that would actually ship.

**It carries a lowering, and that is forced rather than chosen.** Scoring a
conformance test means lowering its source, verifying the artifact and running
it, so this root's reference set is the slice-compiler root's. It composes no
second profile and registers no capability: what it adds beyond the image is a
suite reader, a selection pipeline, a self-check, a merge and a ratchet, all of
which are in the closure it publishes for the same reason the corpus replay is in
the execution-only root's.

**Why the two JavaScript roots are two projects and not two modes of one.** They
differ by exactly one reference - the lowering - and that difference is the whole
of the `execution-only` composition label. The execution-only root names the
profile and not the compiler, so it cannot turn source into an artifact however
it is invoked, and every artifact it runs is precompiled and read as bytes from
the retained corpus. The slice-compiler root names both, lowers the slice
programs and writes that corpus. A flag on one binary would have made the
difference a run-time choice inside one closure, and a closure report cannot see
a flag.

**Neither of the two ORIGINAL JavaScript roots is `narrow-runtime-compiler`, and
the slice-compiler root is only shaped like one.** That label belongs to a
composition carrying a lowering for a named restricted SOURCE surface, and there
was no source surface until JS-3b wrote the tokenizer and the static semantics.
What that root lowers is a programmatic builder, so it is recorded here as a
demonstration and claims no label. **JS-3b claims the label with a publish-and-run
gate of its own, and the row that holds it is the CLI root added on 2026-09-03**;
the paragraphs at the end of this section are that row's.
**The `narrow-runtime-compiler` label is claimed, and this row is the one that
claims it.** The paragraph below said until 2026-09-03 that no root here held the
label and that JS-3b would claim it with a publish-and-run gate of its own. That
is what this row is. The label belongs to a composition carrying the tokenizer,
the static semantics and the lowering for a named restricted **source** surface,
and what was missing after JS-3b wrote those three was a composition handed
source from *outside* the image — a file a person names on a command line. The
slice-compiler root lowers a programmatic builder and the conformance root lowers
a fixture tree this repository also wrote; both print
`narrow-runtime-compiler-shaped` in their catalog table, and this root is the
first to print the label itself.

**It is the end-user host, and it is a demonstration anyway.** Point it at a
`.js` file and it compiles, verifies and runs it, printing the completion value;
name several files and it runs them as separate scripts sharing one realm, in
order; point it at a directory and it sweeps every `.js` file under it, a realm
each, and prints the distribution. **From 2026-09-04 it lowers
`broiler.javascript.wide` by default and keeps the slice behind `--slice`**, and
what that changes about this row is one sentence and not the Kind column. The
sentence that changed: pointed at the Octane benchmark it used to refuse every
file, and it now runs several of them and prints a score.

**The Kind column did not change, and that is the part worth reading rather than
skimming.** **A tool advertised as a JavaScript host has to be able to run
JavaScript**, and running some of it is not the claim advertising would make.
`broiler.javascript.wide` admits no class, generator, `async` function, module,
destructuring, spread, template literal, `for … of`, Proxy, Symbol, BigInt,
typed array, `eval` or `Function` constructor; its regular expressions are
translated to the platform's engine and are declared an approximation where that
is done; nothing in it has been read by a human; and it has no conformance run of
its own over the pinned suite. Advertising it would be the untruthful support
claim the core roadmap makes a stop condition, and section 1's advertised set
stays empty.

**Its closure is the one with nothing to explain away, and that is the point of
it.** The paragraph further down records what the other JavaScript roots carry
beyond the image each demonstrates — a corpus replay, ordering assertions, a fuzz
mutator, a soak, a corpus writer, a conformance harness — all forced there by
rules A11 and A12 leaving such code nowhere else, and all of it in the closure
each publishes. **This root carries none of it.** It reads a file, compiles it,
verifies it, runs it, and reports; a reader comparing its closure against section
15's row for the label finds the assemblies the label names and no others. That
property is why the label could not simply have been asserted of a sibling.

**What it registers is one capability, and what it does NOT register is its content
policy.** From 2026-09-04 it registers `broiler.javascript.write`, which is how a
program's `print` reaches standard output; the import is optional, so a sibling that
registers nothing composes the same profile, runs the same programs, and has a
`print` that reaches nowhere. What it still registers is no artifact provider, and
that is the policy: no capability
and no artifact provider, so every guest-initiated load is refused
deterministically — the only policy a manifest with no `eval`, no `Function`
constructor and no dynamic `import()` could have. It states no ceiling either: the
instruction allowance is the profile's own declared default unless a caller passes
`--fuel`, because a host with an opinion about how long a program may run is a
host imposing a policy the profile did not declare.

**Its acceptance suite is input files and not injected code.** `src/tests/cli/`
holds the programs and `eng/run-cli-acceptance.py` drives the built binary over
eighteen command lines, judging exit codes and output. No source of this
component is patched to make a case fail and no internal type is reached for:
what is under test is the binary a person would run, including its argument
parsing, which of its two streams carries which message, and what it does with a
file that is not UTF-8. The driver takes `--expected` so it can be pointed at a
table of deliberately wrong rows and shown to report the mismatch, because a
driver whose every row passes may not be comparing anything.


**What the two JavaScript roots contain beyond the image each demonstrates,
because a label describes a reference set and not a file inventory.** The
execution-only root carries the retained corpus's replay, the ordering
assertions over every entry of it, the fuzz mutator, the soak over recycled
runtimes and the shared-aggregate-budget exercises. The slice-compiler root
carries the corpus writer and the cross-profile catalog checks with the
neighbour descriptor those need. **All of it is in the closure each root
publishes**, and that is forced rather than chosen: every one of those drives a
profile's own verifier and executor, rule A11 forbids a test project to
reference a profile assembly, and rule A12 forbids a composition root to
reference the fixture assembly. There is nowhere else for them to be, and a
corpus a test project produced would in any case be a corpus the product path
never exercised.

Two consequences a reader should meet here rather than in a closure report.
**Rule K4's "no test assembly" clause is satisfied by the assembly boundary
while the property is weaker than it reads** - no test *assembly* is present,
and a mutator and a soak driver are. And **an advertised composition may not
carry any of this**: section 1's set is empty, both roots are demonstrations,
and a root proposed for advertisement has to answer for its closure separately.
That is one more reason none is advertised today.

**This is prose rather than a ninth column, deliberately.** The columns are the
claims a rule reads back - profiles, assemblies, siblings, capabilities, loads,
evidence - and each has a mechanical counterpart in the checkout that K1 to K4
compare it against. What a root contains beyond its label has no such
counterpart: a scan cannot tell a soak driver from an interpreter, so a column
would be a field nothing could check, sitting in a table where every other field
is checked. Naming it here keeps the distinction visible.

**The neighbour profile the slice-compiler root composes for its cross-profile
checks is defined inside that root**, not referenced as an assembly, which is why
its Profiles column names one profile. It exists so that a neighbour's maxima and
a neighbour's adopted defaults can be shown to reach different things; putting it
in the execution-only closure would have contradicted the single-profile claim
that closure exists to make.

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
