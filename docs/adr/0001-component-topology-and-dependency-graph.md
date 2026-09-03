# ADR 0001 - Component Topology, Package Boundaries, And The Dependency Graph

**Status:** Proposed

**Date:** 2026-08-27

**Core contract:** not contract-bearing

## Context

Roadmap section 5 offers the package names as hypotheses rather than as
authorization to create assemblies, and milestone VM-0 (section 13) is the
milestone that turns them into a graph a build proves. Its gate asks for an
acyclic shell graph that builds and for architecture tests that, in the gate's
own words, "express every forbidden edge, including any edge to a legacy Broiler
component". That is the PLANNED gate; the result this milestone actually claims
is narrower and is stated verbatim in the register section below.

This record settles where Broiler.VM's code lives, which five projects VM-0
creates, what each may reference, which public surface exists, how every
forbidden edge is proven rather than asserted, which component-level files
exist, and that VM-0 changes nothing in the aggregate repository. It is not
contract-bearing: it fixes the shape of the component, not the profile-neutral
contract. Core contract version 1 and the frozen public-name table belong to ADR
0003 (`0003-core-contract-v1-and-amendments.md`).

Nothing here describes behaviour. At VM-0 the three product assemblies contain
no catalog, no descriptor, no verifier, no lifecycle and no budget code; the
paper freeze of those names lives in ADR 0002
(`0002-profile-identity-and-static-catalog.md`) and its siblings, and VM-1
implements them.

## Decision: the path, not the name, partitions product from test

Broiler.VM uses the `src/` + `src/tests/` layout, rooted at the component root.
A project lives at exactly one of two path shapes and nowhere else:

| Kind | Path shape | Packable |
|---|---|---|
| Product | `src/<AssemblyName>/<AssemblyName>.csproj` | yes, if it declares a PackageId |
| Test-only | `src/tests/<AssemblyName>/<AssemblyName>.csproj` | never |

For every project the directory name, the project file base name, the
`AssemblyName` and the `RootNamespace` are equal. A project file at any other
path is a failing violation, not a style preference.

The path is the authoritative partition, and a project's NAME is never the
authority for the product/test question. Section 1 names the test-only fixture
component `Broiler.VM.Fixtures`, which ends in no suffix the vendored packaging
conventions recognise; a name-based partition therefore fails on precisely the
project the containment rule exists to hold. A path-based partition is decidable
from the project file's location alone, survives renames, and is the same fact
VM-3's closure reports must confirm.

Rejected: the flat sibling-directory layout, which reduces the partition to a
name-suffix convention; a top-level `tests/` beside `src/`, which splits the
glob root for no gain; and deferring the layout to VM-1, which is impossible
because the rules are path expressions and the layout is therefore an input to
VM-0's gate rather than an output of it.

Two directories are reserved by this record and created later:
`src/compositions/<name>/` for advertised or sample composition roots (deferred
to VM-3) and further `src/tests/<host>/` directories for test-only hosts
(deferred to VM-1).

## Decision: five projects, eight edges, and a closed budget

VM-0 creates exactly five projects and no others. The table below is the content
of `src/tests/Broiler.VM.Architecture.Tests/graph.manifest.json` (exists at
VM-0: src/tests/Broiler.VM.Architecture.Tests/graph.manifest.json), which is the
machine-readable authority; this printing is a convenience, and the docs test
that asserts the two agree is deferred to VM-6.

| Project file | AssemblyName | RootNamespace | PackageId | IsPackable | IsTestProject | ProjectReference targets |
|---|---|---|---|---|---|---|
| `src/Broiler.VM.Abstractions/Broiler.VM.Abstractions.csproj` | Broiler.VM.Abstractions | Broiler.VM.Abstractions | Broiler.VM.Abstractions | true | false | none |
| `src/Broiler.VM.Binary/Broiler.VM.Binary.csproj` | Broiler.VM.Binary | Broiler.VM.Binary | Broiler.VM.Binary | true | false | none |
| `src/Broiler.VM.Runtime/Broiler.VM.Runtime.csproj` | Broiler.VM.Runtime | Broiler.VM.Runtime | Broiler.VM.Runtime | true | false | Abstractions, Binary |
| `src/tests/Broiler.VM.Fixtures/Broiler.VM.Fixtures.csproj` | Broiler.VM.Fixtures | Broiler.VM.Fixtures | none | false | false | Abstractions, Binary |
| `src/tests/Broiler.VM.Architecture.Tests/Broiler.VM.Architecture.Tests.csproj` | Broiler.VM.Architecture.Tests | Broiler.VM.Architecture.Tests | none | false | true | Abstractions, Binary, Runtime, Fixtures |

All five project files exist at VM-0 at the paths above. The complete legal edge
multiset is these eight edges and no others:

| From | To |
|---|---|
| Broiler.VM.Runtime | Broiler.VM.Abstractions |
| Broiler.VM.Runtime | Broiler.VM.Binary |
| Broiler.VM.Fixtures | Broiler.VM.Abstractions |
| Broiler.VM.Fixtures | Broiler.VM.Binary |
| Broiler.VM.Architecture.Tests | Broiler.VM.Abstractions |
| Broiler.VM.Architecture.Tests | Broiler.VM.Binary |
| Broiler.VM.Architecture.Tests | Broiler.VM.Runtime |
| Broiler.VM.Architecture.Tests | Broiler.VM.Fixtures |

Abstractions and Binary are sinks, so the graph is acyclic by construction
rather than by inspection. An extra edge and a missing edge both fail, which is
what makes the manifest a drift check rather than a description.

Each of the three product assemblies enforces a boundary that cannot be enforced
inside another. Abstractions is the dependency boundary a profile compiles
against, and the only assembly whose zero-reference property makes "the core
knows no concrete profile" mechanically checkable. Binary is awarded core
ownership from day one by section 8's table because it has two independent
consumers - the core envelope and every profile verifier - before any profile
exists, and it must be linkable by a verifier without dragging in the runtime.
Runtime is the deployment and ownership boundary a composition root links, and
the only assembly permitted to know the catalog and lifecycle implementation.
Binary deliberately does not reference Abstractions: bounded reading is
mechanism, and section 8's rule is that mechanism does not acquire contract
vocabulary.

Rejected: merging Binary into Abstractions, which makes section 5's opposite
content rules inexpressible and inflates every VM-3 closure report; merging
Abstractions into Runtime, after which no closure could contain a profile
without the runtime; a `Broiler.VM` facade package, which enforces no boundary
and which section 5 forbids as an assembly that only shortens a file; a public
`Broiler.VM.Testing` support library, which is the standard route by which test
tooling reaches a product closure; a product-located profile shell, which would
make the fixture-containment rule vacuous; and deferring the architecture-test
project to VM-1, without which VM-0's gate cannot close at all.

### Section 5's parenthetical, read strictly

Section 5's target-direction block writes a profile's edges as `Abstractions +
Binary (+ Runtime contracts)`. This record reads "Runtime contracts" as the
contract TYPES, all of which are declared in `Broiler.VM.Abstractions`. NO
PROFILE PACKAGE EVER REFERENCES THE `Broiler.VM.Runtime` ASSEMBLY. A profile's
reference set is exactly {`Broiler.VM.Abstractions`, `Broiler.VM.Binary`}, and
that binds the fixture profile, every future `Broiler.VM.Profile.*`, and every
application-local consumer profile.

**Revised 2026-08-31 to state the subject the sentence always had.** The set is of
**Broiler.VM-owned** assemblies. A profile component's own siblings - a format
assembly, a lowering, its composition roots - are not members of it and this
sentence does not bound them. The subject here is the `Broiler.VM.Runtime`
exclusion, which is what the paragraph above argues and all that rule A13 tests.
The qualifier is added because this is the sentence readers quote, and a profile
roadmap proposing its own format assembly had no way to tell from it whether the
graph it drew was legal. ADR 0011 P1 carries the same qualifier.

The consequence is binding on VM-1 and on ADR 0011
(`0011-source-level-profile-contract.md`): any contract a profile needs is
declared in Abstractions or it does not exist. The loose reading was rejected
because it puts the runtime on every profile package's dependency graph,
contradicts VM-3's requirement that a second profile change nothing in the core,
and leaves the profile-to-runtime rule unwritable.

### The assembly and package budget

- Packable package IDs are exactly three - `Broiler.VM.Abstractions`,
  `Broiler.VM.Binary`, `Broiler.VM.Runtime` - for VM-0, VM-1 and VM-2. VM-1 adds
  zero packable assemblies; VM-2 adds zero. A fourth packable assembly before
  VM-3 requires a dated revision of this record naming which of section 5's
  boundaries (dependency, AOT, deployment, ownership, test, package) it enforces
  and why that boundary is not enforceable inside an existing assembly.
- Test-only projects: exactly 2 at VM-0; VM-1 may add at most 2 (a trimmed or
  Native AOT fixture construction host and a behavioural contract suite); VM-2
  may add at most 1 (a fuzz target host). The ceiling at VM-2 is 5 test-only
  projects and 8 projects in total.
- The set may SHRINK at VM-6, which finalizes only the package boundaries VM-0
  evidence justifies. It may not grow silently at any point.

### Target framework, analyzers, and the absence of an AOT claim

All five projects target `net10.0`, set from the component's own
`Directory.Build.props` (exists at VM-0: Directory.Build.props) together with
`LangVersion latest`, `Nullable enable`, `ImplicitUsings disable`,
`EnableNETAnalyzers true` and `AnalysisLevel latest`.

No project sets `IsAotCompatible`, and that is a decision. Invariant 7
classifies analyzer success as an INPUT and requires that Native AOT be
demonstrated by publishing and running a declared composition; a shell with no
method body gives the trim and AOT analyzers nothing to analyse, so switching
them on at VM-0 would produce a green result that carries no claim and invites
one to be read from it. The property is decided again at VM-1, when real code
lands. Broiler.VM claims no RID at VM-0; the declared and reserved matrix
belongs to ADR 0012 (`0012-security-ownership-and-support-matrix.md`).

## Decision: the public surface is one type, and one namespace

The product graph exports exactly one public type at VM-0:
`Broiler.VM.VmCoreContract` (exists at VM-0:
src/Broiler.VM.Abstractions/VmCoreContract.cs), a static class whose only
members are `public const int Version = 1` and `public const int
MinimumSupportedVersion = 1`. `Broiler.VM.Binary` and `Broiler.VM.Runtime`
export zero public types.

Each of the four non-test projects holds one `internal sealed class
AssemblyMarker` (exists at VM-0: src/Broiler.VM.Abstractions/AssemblyMarker.cs,
src/Broiler.VM.Binary/AssemblyMarker.cs,
src/Broiler.VM.Runtime/AssemblyMarker.cs,
src/tests/Broiler.VM.Fixtures/AssemblyMarker.cs), which exists so the
architecture tests have a compile-time anchor into every project and which
declares no contract.

A const int folds away entirely under trimming and Native AOT, so the one
executable artefact VM-0 ships costs nothing at run time and carries no
behaviour. Everything else this milestone freezes is paper, and ADR 0002 records
why: shipping unexercised catalog code would produce code no VM-0 gate
evaluates, which the ledger's update rule 4 forbids promoting.

NAMESPACE. Every exported type in the product graph is declared `namespace
Broiler.VM;`, written explicitly in its own file, in all three product
assemblies. The assembly split is a dependency boundary, not a vocabulary
boundary, so a consumer that references two of the three assemblies does not
acquire two namespaces to import. `RootNamespace` stays equal to `AssemblyName`
so the layout equality rule above is unchanged; it only supplies the default for
new files, and the internal `AssemblyMarker` types take that default precisely
because they are not exported.

## Decision: four mechanisms contain the fixture profile, three run at VM-0

`Broiler.VM.Fixtures` exists as a shell at VM-0 because a containment rule with
no possible violator expresses nothing. Its sole content is the internal marker
named above; VM-1 replaces that marker with the fixture profile without touching
any containment mechanism.

| # | Mechanism | State at VM-0 |
|---|---|---|
| 1 | PATH: it lives under `src/tests/`, and moving it to `src/` is itself a failing violation | enforced (Rule A4 and the path-shape assertion) |
| 2 | LITERAL PACKABILITY: its own project file contains the element `<IsPackable>false</IsPackable>`, asserted as XML text rather than as the evaluated MSBuild property | enforced (Rule A5) |
| 3 | GRAPH: no product project references anything under `src/tests/`, at project-file and at assembly level | project-file half enforced (Rule A4); assembly half is Rule B6, Vacuous |
| 4 | ARTEFACT: a pack produces exactly three packages and none bearing the fixture name | Rule C1, Deferred to VM-6; see Exclusion EX-04 |

Mechanism 2 is mandatory rather than decorative. The vendored
`eng/Broiler.Packaging.props` (exists at VM-0: eng/Broiler.Packaging.props)
defaults `IsPackable` to true and switches it off only for project names ending
`.Tests`, `.Demo`, `.Diagnostic`, `.Benchmark`, `.Benchmarks`, `.DataTool`,
`.Generator` or `.PerfTests`. `Broiler.VM.Fixtures` matches none of them, so
without the explicit element it would pack and publish. Asserting the literal
element means a change to the vendored file cannot silently re-enable packing.

The project is NOT renamed to `Broiler.VM.Fixtures.Tests` to obtain the
convention: section 1 fixes the term, the project is a library consumed by tests
rather than a test project, and making containment depend on a convention in a
vendored file this component may not edit is a weaker guarantee than an explicit
element. Relying on the convention plus review was rejected for the same reason
section 12 gives about the legacy boundary - it must be an architecture-tested
rule, not a convention.

## Decision: composition roots are defined and named, and VM-0 creates none

A COMPOSITION ROOT is a project that (a) is the only kind of project permitted
to reference a profile assembly, (b) names each profile it includes by a direct
descriptor accessor from that profile's own package, (c) produces an executable
or an explicitly declared closure host rather than a library other Broiler.VM
code links, and (d) is non-packable unless the composition register lists it as
advertised. It is a deployment-composition boundary in the sense of section 1,
not a code-organisation device.

VM-0 CREATES NO COMPOSITION-ROOT PROJECT (VM-0 decision on paper; no file at
VM-0). It freezes the definition, the naming scheme and the enforcement rule,
and reserves the names:

| Kind | Name | Location | Created by |
|---|---|---|---|
| Advertised or sample | `Broiler.VM.Composition.<CompositionName>` | `src/compositions/<name>/` | VM-3 |
| Test-only | `Broiler.VM.Fixtures.Host` with optional `.Jit`, `.Trimmed`, `.Aot` suffix | `src/tests/<name>/` | VM-1 |

`<CompositionName>` names the DEPLOYMENT COMPOSITION, never its profile arity
and never a profile list. Single-profile and multi-profile roots are spelled
identically, and the profile count, the exact descriptor set, the declared RIDs
and the evidence-bundle ID belong in the composition register, not in the
assembly name. A root that gains or loses a profile keeps its name and gets a
new evidence bundle, which is what keeps accepted Native AOT evidence attached
to the thing it was collected for.

Creating a root now was rejected: it would either reference the test-only
fixture project - the edge this milestone exists to forbid - or be a copy of
`Broiler.VM.Runtime` with an entry point, which enforces no boundary and is the
shape-only stub invariant 8 rejects. Naming roots by arity was rejected because
it forces a rename when a composition gains a profile. An aggregate registration
helper was rejected because section 3 forbids exactly such a type by name and
reasoning: it would reference every profile assembly and defeat VM-3's
exact-closure gates. Composition as a runtime option that disables a linked
profile is forbidden outright by section 5, and a disabled-but-linked profile
would appear in the closure anyway, making every closure report untruthful.

Two consequences are asserted now rather than at VM-3. `Broiler.VM.Runtime` is a
library, has no entry point and may never contain a composition root (Rule A9).
No product assembly may export a type named `BuiltInProfiles`,
`DefaultProfiles`, `AllProfiles` or `KnownProfiles` (Rule B7).

At core contract version 1 the ADVERTISED COMPOSITION SET IS EMPTY: the core
ships no composition and no language profile. The composition register that will
carry the schema, the advertised set and the RID rows is `docs/compositions.md`
(deferred to VM-3); at VM-0 the composition-root allow-list is instead an empty
constant inside the architecture-test project, which makes the rule maximally
strict now and reviewable later. Exclusion EX-08 records that the allow-list is
not yet a reviewable register. Selecting a profile a composition does not
contain is the distinct `unsupported profile` outcome and never `invalid
artifact`; that outcome set is owned by ADR 0005
(`0005-operation-result-envelope.md`), and a composition that registers no
artifact-provider capability refuses every guest-initiated load
deterministically, which ADR 0008 (`0008-guest-initiated-loads.md`) owns.

## The architecture-rule register

Every rule Broiler.VM names has exactly one row in
`src/tests/Broiler.VM.Architecture.Tests/rules.register.json` (exists at VM-0:
src/tests/Broiler.VM.Architecture.Tests/rules.register.json), with its owning
ADR, its status, its activation milestone, the artefact that makes it
non-vacuous, and its witness. The register is the authority the evidence bundle
and the ledger row quote; a meta-test asserts the correspondence between the
register, the test methods and the witness inputs in both directions, so a rule
cannot be lost between ADR prose and test code.

STATUS VOCABULARY, exactly three values: Active (it runs, and a witness shows it
rejecting a violation), Vacuous (it runs, and nothing in the VM-0 graph can
violate it), Deferred (not asserted at VM-0; the activation milestone owns
promoting it).

ACTIVATION PREDICATE, stated once here and nowhere else: a Deferred or Vacuous
row may not name an activation milestone at or before the current milestone, and
a rule whose activation milestone IS the current milestone must have status
Active with a passing witness. Promotion from Deferred to Active is part of the
activating milestone's exit evidence. The two halves are the same rule read from
opposite ends, and the looser form - that no row names a milestone earlier than
the current one - is struck, because at VM-0 it would let a rule claim
activation here while asserting nothing.

WITNESSES. An Active rule ships a WITNESS: a deliberately violating input the
rule must reject. Eleven witness project files live at
`src/tests/Broiler.VM.Architecture.Tests/witnesses/` (exists at VM-0:
src/tests/Broiler.VM.Architecture.Tests/witnesses/) with the extension
`.csproj.witness` so MSBuild never globs them into the build; they are neither
one per rule nor all group A's. Ten of them serve the eleven group A rules,
because Rule A7 and Rule A8 share an input, and the eleventh is Rule D1's
inbound input. Two further witnesses are types compiled into the test assembly
itself (exists at VM-0:
src/tests/Broiler.VM.Architecture.Tests/DynamicLoadingWitness.cs,
src/tests/Broiler.VM.Architecture.Tests/PublicSurfaceLeakWitness.cs), because
the violations they carry are metadata facts rather than project-file facts. A
rule that has never rejected anything expresses nothing, which is what separates
this gate from a shape-only stub.

TWO ENFORCEMENT LEVELS ARE REQUIRED, not one. A project-file scan cannot see a
package-shaped reintroduction of a forbidden edge, and a metadata scan cannot
see an unused ProjectReference or a shared-source `Compile` item that has not
yet produced a type. Group A reads project files; group B reads compiled
metadata. The AssemblyRef, MemberRef, TypeDef and CustomAttribute rules - B1,
B2, B3, B5, B5b, B6 and B7 - read that metadata with
`System.Reflection.Metadata` over a `PEReader` and never by loading and
reflecting, because using runtime reflection to prove the absence of runtime
reflection would introduce the machinery into the test host and would miss every
unexecuted call site. The exported-member checks do use ordinary reflection, and
that is a scoped exception rather than an oversight: Rule B4 and the member half
of Rule E5 call `GetExportedTypes` and `GetMembers` on assemblies this test
project already references. They inspect a public signature that is already
loaded, so reflection reads exactly the thing it is asked about; neither is
trying to prove the absence of dynamic loading, which is Rule B5's job and stays
on the `PEReader` path, as does Rule E5's exported-type half.

THE COUNTS. 28 rows: 19 Active, 6 Vacuous, 3 Deferred. The Vacuous rules are B2,
B3, B4, B5b, B6 and B7; the Deferred rules are C1, C2 and C3.

THE CLAIM. VM-0 claims this sentence and no other: "every forbidden edge in the
VM-0 shell graph is expressed and witnessed; 9 rules await their subject and are
registered in rules.register.json". The unqualified sentence that architecture
tests express every forbidden edge is not written in this record, in the
evidence bundle or in the ledger row, and the gate wording quoted in Context is
the planned gate rather than the result.

### Group A - project files

Rule A1: No ProjectReference, resolved to a full path, lies outside the
component root. Status: Active; witness
src/tests/Broiler.VM.Architecture.Tests/witnesses/A1-outbound-project-reference.csproj.witness.

Rule A2: No PackageReference names a Broiler.* package. Status: Active; witness
src/tests/Broiler.VM.Architecture.Tests/witnesses/A2-broiler-package-reference.csproj.witness.

Rule A3: No Compile, None, Content, EmbeddedResource or Import item resolves
outside the component root. Status: Active; witness
src/tests/Broiler.VM.Architecture.Tests/witnesses/A3-shared-source-link.csproj.witness.

Rule A4: No product project has a ProjectReference resolving under src/tests/.
Status: Active; witness
src/tests/Broiler.VM.Architecture.Tests/witnesses/A4-product-references-test.csproj.witness.

Rule A5: Every project under src/tests/ contains the literal element IsPackable
false. Status: Active; witness
src/tests/Broiler.VM.Architecture.Tests/witnesses/A5-test-project-omits-ispackable.csproj.witness.

Rule A6: Exactly three projects declare a PackageId, and for each PackageId
equals AssemblyName equals RootNamespace equals the project file base name.
Status: Active; witness
src/tests/Broiler.VM.Architecture.Tests/witnesses/A6-fourth-package-id.csproj.witness.

Rule A7: The resolved ProjectReference edge multiset equals graph.manifest.json
exactly; extra and missing edges both fail, and it subsumes acyclicity. Status:
Active; witness
src/tests/Broiler.VM.Architecture.Tests/witnesses/A8-profile-references-runtime.csproj.witness,
shared with Rule A8 because an edge absent from the manifest is exactly what
that input adds.

Rule A8: No profile project references Broiler.VM.Runtime. Status: Active;
witness
src/tests/Broiler.VM.Architecture.Tests/witnesses/A8-profile-references-runtime.csproj.witness.

Rule A9: Broiler.VM.Runtime declares OutputType Library or omits it, and
declares no composition root. Status: Active; witness
src/tests/Broiler.VM.Architecture.Tests/witnesses/A9-runtime-is-an-executable.csproj.witness.

Rule A10: No product project declares InternalsVisibleTo, because VM-3 must
prove a profile is writable through the public source contract alone and a test
reaching internals is how that proof quietly stops being possible. Status:
Active; witness
src/tests/Broiler.VM.Architecture.Tests/witnesses/A10-product-internals-visible-to.csproj.witness.

Rule A11: No project outside the composition-root allow-list references an
assembly matching Broiler.VM.Profile.*, and the allow-list is empty at VM-0.
Status: Active; witness
src/tests/Broiler.VM.Architecture.Tests/witnesses/A11-profile-reference-outside-composition-root.csproj.witness.

An exception to Rule A10 requires a dated revision of this record naming the
specific type it unblocks.

### Group B - compiled metadata

Rule B1: Broiler.VM.Abstractions and Broiler.VM.Binary reference nothing outside
the framework, which is the mechanical form of section 5's arrow to nothing.
Status: Active; witness the architecture-test assembly itself, which references
xunit and must be flagged.

Rule B2: Broiler.VM.Runtime references nothing outside Broiler.VM.Abstractions
and Broiler.VM.Binary. Status: Vacuous at VM-0 - it runs, and nothing in the
VM-0 graph can violate it; it becomes non-vacuous at VM-1 when the runtime uses
a type from either assembly and the rule tightens from subset to equality.

Rule B3: No assembly in the product graph names a Broiler.* assembly outside the
component's own set, which is the assembly-level twin of Rule A1 and Rule A2.
Status: Vacuous at VM-0 - it runs, and nothing in the VM-0 graph can violate it;
it becomes non-vacuous at VM-1 when a product assembly names any non-framework
assembly at all.

Rule B4: No exported member of a product assembly names a type outside System.*
and Broiler.VM. Status: Vacuous at VM-0 - it runs, and nothing in the VM-0 graph
can violate it; it becomes non-vacuous at VM-1 when a product assembly exports a
member with a parameter or return type. The same scanner is shown flagging a
leak in the test assembly, but that demonstrates the scanner rather than the
product graph, so the register records B4 Vacuous and names no witness.

Rule B5: No assembly in the product graph references a dynamic-loading,
reflection-invocation or IL-emit API, because invariant 2 requires registration
to be static and typed. Status: Active; witness the DynamicLoadingWitness type
compiled into the architecture-test assembly, which calls Type.GetType and
Activator.CreateInstance and must be flagged.

Rule B5b: No assembly in the product graph applies ModuleInitializerAttribute,
which invariant 2 forbids by name. Status: Vacuous at VM-0 - it runs, and
nothing in the VM-0 graph can violate it; it becomes non-vacuous at VM-1 when a
product assembly declares any method that could carry the attribute.

Rule B6: No product assembly references an assembly built from src/tests/.
Status: Vacuous at VM-0 - it runs, and nothing in the VM-0 graph can violate it;
it becomes non-vacuous at VM-1 when a product assembly names any non-framework
assembly. Its project-file twin, Rule A4, is Active now.

Rule B7: No product assembly exports a type named BuiltInProfiles,
DefaultProfiles, AllProfiles or KnownProfiles, which section 3 rejects by name.
Status: Vacuous at VM-0 - it runs, and nothing in the VM-0 graph can violate it;
it becomes non-vacuous at VM-1 when the product graph exports a type other than
VmCoreContract.

Rule B5b is not hypothetical. The aggregate repository's own build properties
suppress CA2255 repository-wide, with the stated reason that legacy assemblies
use module initializers to auto-register built-ins - precisely the pattern
invariant 2 outlaws. Not chaining this component's build properties to that file
(below) is what keeps the suppression out of Broiler.VM.

### Group C - packed artefacts

Rule C1: dotnet pack over Broiler.VM.slnx produces exactly three .nupkg and
three .snupkg, named for the three product assemblies. Status: Deferred;
activation milestone VM-6; non-vacuous when a component CI pack step exists; not
asserted at VM-0.

Rule C2: No produced .nuspec declares a dependency on a Broiler.* package
outside the declared three. Status: Deferred; activation milestone VM-6;
non-vacuous when a component CI pack step exists; not asserted at VM-0.

Rule C3: No produced .nuspec's id, title, description, tags or release notes
names a language, which implements section 14's packaging failure of a language
capability implied by package or API. Status: Deferred; activation milestone
VM-6; non-vacuous when a component CI pack step exists; not asserted at VM-0.

A pack was executed by hand at VM-0 and its log is retained in the evidence
bundle as observed repository state, explicitly not as a rule assertion; ADR
0012 owns the bundle. Exclusion EX-04 records that packaging is not gated here.

### Group D - the inbound half of the legacy boundary

Rule D1: No project file outside the component declares a ProjectReference or
PackageReference resolving into Broiler.VM. Status: Active; witness
src/tests/Broiler.VM.Architecture.Tests/witnesses/D1-inbound-project-reference.csproj.witness.

### Group E - the records themselves

Rule E4: The ADR index lists exactly the ADR files present, and every rule
identifier named in an ADR has exactly one row in the register. Status: Active;
witness: the assertion fails when an ADR is added without an index row, or names
an identifier the register does not carry.

Rule E5: The product graph exports exactly one public type,
`Broiler.VM.VmCoreContract`, whose only members are the two contract-version
constants. Status: Active; witness: the assertion fails when any product
assembly exports a second type or the type gains a member.

The register's three remaining rows, E1 to E3, bind the ADRs to `VmCoreContract`
and are owned by ADR 0003; they are not restated here.

## Decision: the legacy boundary, stated in the only form the evidence supports

Sections 5 and 14 make the legacy-boundary rule bidirectional, and section 12
requires it to be an architecture-tested rule rather than a convention. The two
halves are not equally decidable, and this record never writes "in either
direction" without the qualifier below.

OUTBOUND - Broiler.VM references no legacy Broiler component. Enforced from
inside the component at both levels by Rule A1, Rule A2, Rule A3 and Rule B3,
and at pack level by Rule C2 when VM-6 activates it. Rule A1 is a
containing-directory test rather than a deny-list of legacy names, so it cannot
go stale on a rename or on a new legacy component; a deny-list was rejected for
exactly that reason.

INBOUND - no component outside Broiler.VM references it. This is not decidable
from inside a standalone checkout of a submodule, because the potential violator
is a project file this component does not own. Rule D1 is therefore
ENVIRONMENT-CONDITIONAL: it locates an aggregate checkout above the component
root, scans every project file outside the component when one is present, and
records an explicit INCONCLUSIVE result - not a pass, and not a skip that reads
as a pass - when none is. Its witness proves the predicate rejects an inbound
edge, so the clean result means something in the branch that runs.

Exclusion EX-01: the inbound half of the legacy-boundary rule is decidable only
when an aggregate checkout is present above the component; in a standalone
checkout rule D1 records Inconclusive. Reason: a submodule cannot enumerate the
repositories that may reference it, and asserting a bidirectional rule on a
one-directional test is the untruthful support claim section 16 stops for.
Closed by: an aggregate-repository check, recommended to the
aggregate-repository owner, not owned by VM-0.

VM-0 makes no change to the aggregate repository, adds no aggregate CI check,
and states no acceptance condition binding on another repository.

## Decision: the aggregate repository is unchanged, and that is a ruling

VM-0 changes none of the four artefacts that could plausibly have to change, and
each is unaffected for a reason rather than by oversight:

| Aggregate artefact | Why VM-0 does not touch it |
|---|---|
| `eng/solutions.json` | Each generated solution's project set is the transitive ProjectReference closure of its declared browser roots. No such closure reaches Broiler.VM, and Broiler.VM references nothing, so every closure is unchanged. Adding a root would place a shell graph inside a browser solution and make browser CI a gate on a component the browser does not consume - the coupling sections 5 and 12 exist to prevent. |
| `scripts/update-solutions.ps1` | Its only repository-wide assertion enumerates `*.slnx` files directly in the repository root and throws on an undeclared one. `Broiler.VM/Broiler.VM.slnx` is not in the root, exactly like the other component solutions, so the guard does not see it and the verify job stays green with no manifest edit. |
| The four generated root `*.slnx` files | They are generated from the manifest above; a hand-edit to include a Broiler.VM project would not survive regeneration. |
| `.github/actions/setup-broiler` | Its submodule initialisation lines name exactly the nested checkouts the browser's closure reaches. Broiler.VM has no nested submodules and is in no closure, so a line would be dead and would erode that file's stated discipline. |

No aggregate CI build leg is added for Broiler.VM at VM-0. The component's own
tests prove the graph, and section 12 states that Broiler.VM is not part of the
browser's graph, gates or evidence.

Recorded for the profile roadmaps rather than acted on here: the generated
solutions carry a forbidden-project pattern that rejects any project file whose
name contains `WebAssembly`. It is name-based, so it would reject a core project
that merely contained the token as readily as a future profile, which is one
more reason the terminology freeze in ADR 0003 keeps language tokens out of core
names. Amending that pattern for a profile that does not exist is the mistake
section 8's extraction gate forbids, so VM-0 does not.

## Component-level files

| Path | State | What it fixes |
|---|---|---|
| `Broiler.VM.slnx` | exists at VM-0: Broiler.VM.slnx | The five projects in `/src/` and `/tests/` folders. It declares no configuration set, so the solution carries Debug and Release only. |
| `Directory.Build.props` | exists at VM-0: Directory.Build.props | Imports `eng/Broiler.Packaging.props` and nothing else; does not chain to a parent. Sets the framework, language, nullable, analyzer and package-metadata properties named above. |
| `eng/Broiler.Packaging.props` | exists at VM-0: eng/Broiler.Packaging.props | Vendored byte-identical from `D:/Broiler.Browser/Broiler.DOM/eng/Broiler.Packaging.props`; SHA-256 `82b186ff0d5c54ca6951eb519344970c53d7b4b880445591852885911261db03`. Never hand-edited. |
| `eng/icon.png` | exists at VM-0: eng/icon.png | The packaging props packs it unconditionally whenever a project is packable, so a pack fails without it. |
| `NuGet.config` | exists at VM-0: NuGet.config | `<clear />`, nuget.org as the single source, package source mapping `*` to nuget.org. |
| `LICENSE` | exists at VM-0: LICENSE | Apache-2.0, matching the packaging props' license expression, so the component packs standalone. |
| `.gitignore` | exists at VM-0: .gitignore | Excludes build output: `bin/`, `obj/`, `.vs/`, `*.user`. |
| `README.md` | exists at VM-0: README.md | Packed into each package by the props when present. |
| `docs/adr/0001` to `docs/adr/0012` | exists at VM-0: docs/adr/ | This record set. The index is `docs/adr/README.md`. |
| `docs/evidence/vm-0/` | exists at VM-0: docs/evidence/vm-0/ | The VM-0 evidence bundle. Its fields and its Decision text are owned by ADR 0012. |
| `CODE-ASSURANCE.md` | exists since VM-1: `CODE-ASSURANCE.md` | **Generated.** The component's code-assurance report: how many units are relevant, how many exempt, the risk distribution, the units at the top of the security vocabulary, and how much of that surface carries a falsification criterion. It is a measurement and not a decision; `HUMAN_REVIEW.md` is where a decision is recorded. |
| `assurance.manifest.json` | exists since VM-1: `assurance.manifest.json` | **Generated.** One entry per code unit in the three product assemblies, exempt and relevant alike, and one per covered file, each with a fingerprint. A change-detection record and not a review: an entry says what a declaration hashed to, never that anyone read it. Rule J7 holds it to the tree. |
| `.gitattributes` | not created (VM-0 decision on paper; no file at VM-0) | The component contains no shell script, so there is no line-ending rule to fix yet. |
| `global.json` | not created (VM-0 decision on paper; no file at VM-0) | No SDK pin; see Exclusion EX-03. |
| `.github/workflows/` | exists since VM-1: `.github/workflows/` | Four files and three lanes, and none publishes. **The component lane was split on 2026-09-03 and is two files plus a definition**: `broiler-vm-lane.yml` holds every job and is entered by nothing directly; `broiler-vm.yml` calls it on every push and pull request over two of the six declared RIDs with short fuzz sessions; `broiler-vm-full.yml` calls it over all six with the full sessions and the emulator, on a `v*` tag, a weekly schedule and a button. One definition and two callers, because two copies of it would have drifted apart, and a caller chooses a matrix and an iteration count and can disable no check. `review.yml` regenerates every assurance artefact on a pull request, commits what moved, and then asserts that what is on disk is what the generator would write. `release.yml` runs that same gate and then the release gate - Rule J11 - before it packs, and stops at `dotnet pack`: pushing to a feed needs a credential this repository does not hold. The component ran no CI of its own at VM-0, which Exclusion EX-06 records; these lanes discharge that early and only in part, because they fire on a pull request and on a tag and nothing else. `broiler-vm.yml` was added at VM-6 to run the graph, catalog, AOT and drift checks. **It has now run on hosted runners and passes**, on `ubuntu-latest` and `windows-latest`, publishing and running every composition root as Native AOT on `linux-x64` and `win-x64`; it did the same on `macos-latest` and `osx-arm64` until the second 2026-09-01 revision withdrew that entry as outside ADR 0012's declared matrix, and it gained jobs for `linux-arm64`, `win-arm64`, `osx-arm64` and `osx-x64` as the third, fourth and fifth revisions of that date widened the matrix to every cell of a three-toolchain by two-architecture grid - which readmits the `osx-arm64` the second revision had withdrawn, by an argument rather than by a job. No revision settles **any RID claim**, because a lane is not an evidence bundle. |
| `docs/compositions.md` | exists at VM-3: docs/compositions.md | The composition and RID register. It carries the schema, the advertised set - empty at core contract version 1 - the two demonstration compositions, and what each was published and run for. Rule A11's allow-list is a path rather than a constant now, and group K holds the register to the checkout. Exclusion EX-08 is closed; revision 1 records it. |
| `docs/platform-references.md` | not created (VM-0 decision on paper; no file at VM-0) | The section 17 pinned-revision table. ADR 0012 records why it is absent and what closes it. |
| `docs/support.md` | deferred to VM-6 | The public support table. ADR 0012 records that none is published at VM-0. |
| `THIRD_PARTY_NOTICES.md` | deferred to VM-6 | Broiler.VM has zero third-party runtime dependencies at VM-0 - SourceLink is `PrivateAssets=all` and xunit is test-only - and an empty notices file would assert a license pass that has not happened. Required at VM-6 or on the day a runtime dependency first lands, whichever comes first. |
| `HUMAN_REVIEW.md` | exists at VM-0, generated since VM-1: `HUMAN_REVIEW.md` | The component's review record. **Generated** from the `// Broiler-Human:` line on each code declaration and from the per-file assurance headers, and edited by nobody: a reviewer records a decision on the declaration they read, and this file is computed from those lines. It therefore carries a row for every alias the tree names rather than one signature block, which is what lets more than one person review. Originally deferred to VM-6 because there was nothing to review and a template with unfilled fields invites a false approval record; created at VM-0 once a maintainer was named, and generated from VM-1 once there was code to bind a decision to. Binding rule: no Broiler.VM package is published while any relevant code unit is unresolved, which Rule J11 asserts and the publish lane runs. The record names no commit - each decision names the fingerprint of the declaration it was made against, which says whether that unit changed rather than whether the tree did. |

Three of these are boundary decisions rather than hygiene.

NOT CHAINING `Directory.Build.props`. MSBuild stops at the first
`Directory.Build.props` it finds walking upward, so a standalone checkout and
one nested inside the aggregate repository evaluate identically. Chaining would
import the aggregate file, whose suppression list silences the nullable
diagnostics this component has no legacy reason to suppress - in a component
whose correctness is a security boundary - and silences CA2255, which was added
to accommodate the legacy component's module-initializer auto-registration, the
exact pattern invariant 2 outlaws and Rule B5b tests for. Broiler.VM also needs
none of the path properties chaining exists to deliver, because it references no
other Broiler component.

THE SINGLE-SOURCE `NuGet.config`. Adding a Broiler-serving package feed would
make a PackageReference to a legacy Broiler package RESOLVABLE. Omitting it
makes that forbidden edge unreachable rather than merely detected, so Rule A2
becomes a second line of defence instead of the only one. It also pins restore
determinism for every evidence bundle.

THE VENDORED PACKAGING PROPS. The file names a sync script as its vendoring
mechanism; that script does not exist anywhere in the aggregate checkout, and
the vendored copies have already drifted - only the Broiler.DOM copy carries the
`PackageReference Remove` line that keeps the import idempotent, which is why
this component vendors that copy. Drift is therefore undetectable except against
the recorded hash above, and asserting the hash is deferred; see Exclusion
EX-07.

## Exclusions

Exclusion EX-01 is stated in full in the legacy-boundary section above and is
not repeated here.

Exclusion EX-02: VM-0 does not claim section 15 gate 2. Reason: gate 2 is a
release gate over a generated dependency closure, a documented static catalog, a
proven public source-level profile contract and published packages, none of
which exists at VM-0; Rule A7 proves only that the declared project graph
matches `graph.manifest.json`. Closed by: VM-6.

Exclusion EX-03: no SDK pin exists; VM-0 evidence is reproducible only against
the recorded SDK version, not enforced by the repository. Reason: no sibling
component pins an SDK, no VM-0 gate clause requires one, and this component is a
submodule whose standalone checkout has no parent pin to inherit; all five
projects target `net10.0` and reproducibility rests on the evidence bundle's
Environment and Dependencies fields recording the SDK and runtime actually used,
which at this collection is SDK 10.0.400. Closed by: a global.json decision by
the component owner, recommended and not taken at VM-0.

Exclusion EX-04: packaging is not gated at VM-0; C1, C2 and C3 are Deferred to
VM-6, so the fourth fixture-containment mechanism and the language-token check
on package metadata assert nothing here. Reason: those rules run in a component
CI pack step, and VM-0 creates no CI workflow; the hand-executed pack log is
observed repository state, not a rule result. Closed by: VM-6. Revised
2026-08-29: a pack step exists in the publish lane from VM-1, so the reason
above no longer holds; C1, C2 and C3 remain Deferred on their own merits and
this record does not promote them.

Exclusion EX-05: 9 of 28 rules await their subject and assert nothing about
anything real at VM-0 - the Vacuous rules B2, B3, B4, B5b, B6 and B7, and the
Deferred rules C1, C2 and C3. Reason: a shell graph can express a rule long
before it contains anything able to violate one, and invariant 8 with ledger
update rule 4 forbids reporting either state as coverage. Closed by: VM-1 for
the six Vacuous rules and VM-6 for the three Deferred ones, each promotion being
part of that milestone's exit evidence.

Exclusion EX-06: no rule in the register runs automatically on any change,
because the component has no CI workflow at VM-0; every VM-0 rule result comes
from a manually executed build and test run, on whichever platforms the evidence
bundle records. Reason: a component CI workflow is VM-6's, and the group A rules
do path handling whose cross-platform behaviour therefore has no automated
second-platform check here. Closed by: VM-6. Revised 2026-08-29: two lanes
landed at VM-1 and run every rule in the register on `ubuntu-latest`, which
discharges the second-platform half and leaves the rest - a lane fires on a
pull request and on a tag, so a commit pushed to a branch nobody opens a pull
request for is examined by nothing until the next one.

Exclusion EX-07: the SHA-256 of `eng/Broiler.Packaging.props` is recorded in
this record and in the evidence bundle, but no automated check asserts it, so
drift in the vendored copy is not detected at VM-0. Reason: the assertion
belongs to a component CI step that does not exist, and the canonical sync
script the file names is absent from the aggregate checkout. Closed by: VM-6.

Exclusion EX-08: the composition-root allow-list Rule A11 reads is an empty
constant inside the architecture-test project rather than a reviewable register,
so relaxing it at VM-3 will be a code change rather than a documented row.
Reason: `docs/compositions.md` is VM-3's artefact and creating it now would
publish a schema for compositions that do not exist. Closed by: VM-3. **CLOSED at
VM-3**: the register exists, the allow-list is the path `src/compositions/`, and
rules K1 to K4 hold the register, the reference sets, the catalogs and the
published closures to each other. Revision 1 of this record states what changed.

## Consequences

- The fixture-containment, packability and product/test rules are path
  expressions rather than name expressions, so they stay true when projects are
  renamed, and VM-3's closure reports can be checked against "no assembly whose
  project lived under `src/tests/`" without a name list.
- Every contract a VM-1 implementer wants to expose to a profile must be placed
  in `Broiler.VM.Abstractions`, which forces the semantics-neutral surface to be
  designed deliberately rather than accreted onto the runtime. VM-1 and VM-2
  cannot enlarge the shipped surface: a new capability fits an existing assembly
  or amends this record.
- This record supersedes the illustrative parenthetical `(+ Runtime contracts)`
  in section 5's target-direction block: a profile's reference set is exactly
  Abstractions and Binary. The roadmap is not edited; the amendment is carried
  as a Proposed, not applied row in the register in ADR 0003, which is also
  where the disagreement between the roadmap text and these records is recorded
  until an owner lands the patch.
- Section 5's candidate-package table is confirmed rather than superseded: the
  three product names it lists as hypotheses are now the frozen, build-proven
  set, and the fixture package it marks test-only is contained by four named
  mechanisms of which three run at VM-0.
- VM-0 requires no aggregate-repository change, and none was made. Two
  recommendations are surfaced to the aggregate-repository owner rather than
  acted on: add the inbound check of Rule D1 to the aggregate repository so
  Exclusion EX-01 can close, and either restore the packaging sync script or
  designate one component's copy of the packaging props as canonical in writing
  so Exclusion EX-07 has a mechanism to close against. A third recommendation
  goes to the canonical packaging file's owner: the name-suffix packability list
  has a documented gap for `.Fixtures`, which this component works around with
  an explicit element rather than by editing the vendored copy.
- A fourth recommendation goes to the component owner: pin the SDK with a
  `global.json` at the component root and assert the resolved version in CI, so
  that Exclusion EX-03 closes and every later evidence bundle is reproducible by
  construction rather than by recollection.
- The strongest assertions available - exactly one exported type, no reflection
  or IL-emit reference, an exact reference graph, no product-to-test edge - are
  available immediately and stay meaningful as code lands, which is the
  practical argument for keeping the shells empty.
- VM-1 inherits named obligations rather than a false green: six Vacuous rules
  to make non-vacuous, an API baseline whose expected content is the name table
  frozen in ADR 0003, and the first test-only composition root under the name
  reserved here.
- No VM-0 result may be read as a capability claim. The shells hold no method
  body, the component advertises no composition, claims no RID, and publishes no
  package; ADR 0012 owns the roles, the support position and the evidence bundle
  in which every exclusion identifier above is repeated verbatim.

## Revisions

This record is `not contract-bearing`, so it changes without the amendment
procedure ADR 0003 section 6 fixes for the contract-bearing ten - that procedure
mints a core contract version, which is not what a component-shape record needs.
What it is held to instead is ledger update rule 1: earlier decisions are
preserved as dated history rather than overwritten. Each revision below states
what the record said before, so a reader of an evidence bundle collected against
the earlier text can still see what it was quoting.

### 2026-08-29 - the review record is generated, and the component has CI

Driven by the owner's ruling that a reviewer fills in the source comment and
nothing else. Four rows of the component-level file table and two exclusions are
affected. No decision elsewhere in this record changes: the graph, the package
budget, the containment mechanisms and the legacy boundary are untouched.

| Row | What it said | What it says now |
|---|---|---|
| `HUMAN_REVIEW.md` | *A scoped attestation of a reviewed revision with build, test, dependency and security evidence* ... *It is unsigned and its decision is `PENDING`* ... *Binding rule: no Broiler.VM package is published without a completed review naming the reviewed commit.* | Generated from the annotations, carrying a row per alias and naming no commit. The binding rule is now that no package is published while any relevant unit is unresolved, which Rule J11 asserts. |
| `.github/workflows/` | *deferred to VM-6* - *The component runs no CI of its own at VM-0; see Exclusion EX-06.* | Two lanes exist from VM-1: one produces the review record on a pull request, one gates and packs. |
| `CODE-ASSURANCE.md` | absent from the table | Added. |
| `assurance.manifest.json` | absent from the table | Added. Both landed at VM-1 and are component-level generated files this table did not list, which made it an incomplete register of exactly the artefacts it exists to register. |

**Why this is a revision and not an erratum.** The three deviations the VM-1
bundle records are cases where the implementation could not honour a record, and
there the record was left standing. This is the opposite: the owner changed the
decision, so the record is what moves. Filing it as an erratum would have left
this table describing a document the component does not produce.

**What did not change.** No package boundary, no project edge, no rule statement
and no exclusion identifier. Exclusions EX-04 and EX-06 keep their VM-0
statements and gain a dated line each, because what they recorded was true at
VM-0 and a later lane does not make it retroactively false.

**Still open.** Nobody has approved this record; it is `Proposed`, as all twelve
are. The lanes discharge Exclusion EX-06 only in part, and nothing in them binds
a reviewer alias to a hosting identity - the record holds an alias to appearing
in the tree, and no rule can tell a real person from a plausible string.

### 2026-08-29 - the VM-3 project budget and the compositions directory

This record's budget section stops at VM-2 and says the project set "may not
grow silently at any point". VM-3's gate requires a separate consumer project
and named composition roots, so the growth has to be recorded here before it
happens. This is that record. It changes no decision above; it extends one
budget and settles three readings that the sections above left open because
nothing existed to read them against.

**The budget.** VM-3 adds four projects and no packable assembly:

| Project | Path | Kind | Why |
|---|---|---|---|
| `Com.Example.Calculator` | `src/tests/Com.Example.Calculator/` | test-only | The application-local consumer profile ADR 0011 asks for |
| `Com.Example.Ledger` | `src/tests/Com.Example.Ledger/` | test-only | A second one, so a two-profile composition exists whose closure is not the single-profile one |
| `Broiler.VM.Composition.Calculator` | `src/compositions/Broiler.VM.Composition.Calculator/` | composition root | The single-profile composition |
| `Broiler.VM.Composition.Workbench` | `src/compositions/Broiler.VM.Composition.Workbench/` | composition root | The two-profile composition |

The graph goes from 8 projects to 12, and test-only projects from 5 to 7. **The
packable set is unchanged at exactly three** - `Broiler.VM.Abstractions`,
`Broiler.VM.Binary` and `Broiler.VM.Runtime` - so the clause requiring a dated
revision for a fourth packable assembly is not exercised by this one. The
composition roots are non-packable, which the section above already requires of
any root the composition register does not list as advertised, and the register
`docs/compositions.md` lists neither as advertised: core contract version 1
ships no composition.

**Which boundary `src/compositions/` enforces.** Section 5's DEPLOYMENT
boundary, and only that one. A composition root is the one project kind
permitted to reference a profile assembly, so the directory is where the
reference graph is allowed to fan out and everywhere else it is not. It is not a
package boundary - nothing there packs - and not an ownership boundary, because
both roots are owned by the same person as everything else here.

**Why the consumer profiles live under `src/tests/`.** The path decision above
admits exactly two shapes plus this reserved third, and none of them is "an
application project". A consumer profile is not a Broiler product package - it
must not be, since its identity is deliberately outside the reserved namespace -
and it is not a composition root. `src/tests/` is therefore the only shape that
fits, and the profiles are non-packable for the same reason every project there
is. Adding a fourth path shape was rejected: it would exist to hold two projects
that already have a shape that fits, and every path expression in the group A
rules would have to learn about it.

**How "no fixture or test assembly" is read in a closure report.** VM-3's gate
asks that each closure report contain "exactly the declared profiles and no
fixture or test assembly", and the consumer profiles live at a test-only path,
so the sentence read literally forbids the closure from containing the profile
the composition declares. The reading this record fixes is the one that makes
the clause do its job: a closure report is accepted when it contains exactly the
profile assemblies its register row declares, **no** `Broiler.VM.Fixtures`, no
testing-framework assembly, and no reflection or dynamic-code assembly. The
consumer profile is one of the declared profiles and belongs there; what the
clause exists to catch is a composition that drags the fixture profile or a test
harness into a shipped image, and both are still caught.

**What this revision moves, and what it does not.** It moves no packable
boundary: the three product assemblies are unchanged across this milestone, the
API baseline they export is unchanged, and their published image sizes are
byte-identical to VM-2's, which is the claim VM-3 exists to make rather than a
side effect of it.

It does change the project partition the group A rules read, and that is a real
change rather than a clarification, so it is written down here rather than only
in the test project. Two of the three partitions were exhaustive - a project was
test-only or it was product - and a composition root is neither. Product it
cannot be: nothing there packs, and rule A4 would forbid the reference to a
consumer profile that this record exists to permit. Test-only it is not: it is
published and run rather than collected by a runner. So there are three
partitions now, and three rules moved with it:

- **A4** gains an exemption for a composition root, and the register row says so.
  The exemption is not a hole, because the rule that replaces it is stricter than
  A4 was.
- **A12** is new: a composition root's references are exactly the three core
  packages plus one or more profile assemblies, it composes at least one profile,
  and it declares no package reference. `Broiler.VM.Fixtures` and every test
  project are forbidden by name.
- **A13** is new: ADR 0011's obligation P1 as a rule. A consumer profile
  references exactly `Broiler.VM.Abstractions` and `Broiler.VM.Binary`, declares
  no package reference, and opens its internals to nobody.

**Exclusion EX-08 is closed.** The composition-root allow-list rule A11 reads is
no longer an empty constant inside the test project. It is a path - every project
under `src/compositions/` - and what each of those projects may contain is
`docs/compositions.md` (exists at VM-3: `docs/compositions.md`), the register
this record deferred. Group K holds the register and the checkout to each other
in both directions, holds each row to the composition's own reference set and to
the catalog its published binary prints, and holds each published closure to
exactly what its row declares. A11's scope widened with it: it now covers
consumer profiles as well as `Broiler.VM.Profile.*`, and its allow-list has real
members rather than being empty.

**One finding, recorded because it is a property of the contract rather than of
these two profiles.** ~~A runtime ceiling is clamped to the tightest profile hard
maximum in the CATALOG, across every profile in it, and adopting a profile default
resolves to the tightest default in the catalog. Both are catalog-wide facts.~~

**Corrected 2026-08-31: only one of the two was a property of the contract, and
the other was a defect.** Adopting a profile default does resolve to the tightest
default in the catalog, and ADR 0007 now records that as the marker's referent.
The hard-maximum clamp was not in the contract at all - ADR 0007 puts `ProfileMax`
at P2 against the profile an artifact names - and the implementation has been
corrected to match. A profile's declared maximum no longer caps profiles composed
beside it. The two consumer profiles were written that way
first and the two-profile composition could not verify a ledger artifact until
they were corrected. Nothing in the core changed; what changed is what a profile
author should declare, and `docs/compositions.md` section 5 records it where a
profile author will look.

### 2026-08-29 - the VM-4 soak host

VM-4's gate asks for a **declared memory plateau**. The behavioural suite already
asserts the metered plateau - the live-bytes counter returns to where it started
after a load-run-evict cycle - and that is a different claim from the one the
gate makes. A metered counter says the core's accounting balances; it says
nothing about whether the process grows. A plateau is a measurement of the
running image, and a measurement needs something that runs long enough to
measure.

**The budget.** VM-4 adds one project and no packable assembly:

| Project | Path | Kind | Why |
|---|---|---|---|
| `Broiler.VM.Soak.Host` | `src/tests/Broiler.VM.Soak.Host/` | test-only | The long-running lifecycle host whose managed heap and working set are sampled |

The graph goes from 12 projects to 13, and test-only projects from 7 to 8. **The
packable set is unchanged at exactly three**, so the clause requiring a dated
revision for a fourth packable assembly is not exercised by this one either.

**Why a host rather than a test.** Three reasons, and the third is the one that
decides it. A test process is shared with a runner whose own allocations are
indistinguishable from the component's in any working-set figure. A test that ran
long enough to plateau would dominate the suite's wall-clock, and a suite people
skip proves nothing. And a plateau is a property of a PUBLISHED image: the
figure a host actually cares about is what the trimmed or Native AOT binary does
over an hour, which a JIT-hosted test cannot report at all. The fuzz host exists
for the same reason and the same shape is reused deliberately.

**What it does not do.** It declares no threshold and passes no judgement. It
runs the cycles it was asked for, samples at intervals, and prints what it saw;
whether the numbers are a plateau is the bundle's reading, and whether that
plateau is acceptable is a release decision ADR 0012 owns. A host that decided
for itself would be a benchmark with an opinion, and VM-5 is the milestone that
owns baselines.

### 2026-08-29 - the VM-5 benchmark host

VM-5's gate asks for **uninstrumented decision-grade baselines** of what the core
costs a profile, each with a predeclared rule, a comparable control, an A/A lane
validity check and retained repetitions. None of those is expressible in the
behavioural suite: a test asserts a property and a measurement reports a number,
and a suite that failed when a number moved would be a performance regression
gate, which is a different instrument with a different failure mode.

**The budget.** VM-5 adds one project and no packable assembly:

| Project | Path | Kind | Why |
|---|---|---|---|
| `Broiler.VM.Bench.Host` | `src/tests/Broiler.VM.Bench.Host/` | test-only | The measurement harness: candidate against control, two lanes, retained repetitions |

The graph goes from 13 projects to 14, and test-only projects from 8 to 9. **The
packable set is unchanged at exactly three.**

**Uninstrumented means what it says.** The harness times a delegate and reads
`GC.GetAllocatedBytesForCurrentThread` and the collection counts around it. It
installs no profiler, no ETW session and no interception, and it references no
benchmarking package - so the thing measured is the product assemblies as they
ship, and the measurement apparatus is thirty lines a reader can check. A
benchmarking framework would be a better instrument and a worse artefact: its
own warmup, pilot and outlier policies would be part of every number, and none
of them would be visible in this repository.

**Why the harness judges nothing but its own validity.** A measurement host that
compared against a threshold would be asserting a performance claim, and section
16's stop condition is a claim without a predeclared rule. It reports the
candidate, the control, their difference, the A/A lane difference and every
repetition, and it exits non-zero only when its own A/A check fails - which is
the one thing it can decide without an opinion about what the numbers should be.

---

### 2026-08-30 - the VM-6 package boundary, samples and CI

VM-6's gate asks that the package boundaries be finalized, that pristine feed
consumers and samples use public APIs only, that the public API be frozen
against a baseline, and that graph, catalog, AOT and contract drift checks be
wired into CI. Three of those need something this record has so far forbidden,
so it is revised rather than worked around.

**The budget does not grow, and this is the milestone where that sentence
finally means something.** The graph stays at fourteen projects and the packable
set stays at exactly three. What VM-6 adds is a project that is **not in the
graph at all**:

| Project | Path | In `Broiler.VM.slnx` | Why not |
|---|---|---|---|
| `Broiler.VM.Sample.FeedConsumer` | `samples/` | **No** | It cannot be. Restoring it requires a `dotnet pack` to have already happened, so a solution containing it would not restore from a clean checkout |

That exclusion is the point rather than a workaround. A sample inside the
solution would acquire the component's `Directory.Build.props`, its vendored
packaging metadata, its analyzer level and its warning policy, and would resolve
Broiler.VM through the project graph like everything else - so it would prove
that our packages work inside our build, which is not the claim VM-6 makes.
`samples/Directory.Build.props` is deliberately empty and exists only to stop the
component's own properties reaching anything under it, and `samples/NuGet.config`
lists exactly one source: a directory of `.nupkg` files.

**Rule A7 therefore reads the solution and not the directory tree.** A project
outside the solution is outside the frozen graph by construction, and
`graph.manifest.json` continues to describe fourteen projects and forty-one
edges. A reader who counts `.csproj` files finds fifteen, and this paragraph is
why.

**One restore source, and it is an assertion.** The samples' `NuGet.config`
clears every configured source and adds back only the local feed. nuget.org is
not reachable from a sample. That is not caution about supply chain - it is how
the claim "these three packages depend on nothing" is made falsifiable, because
a package that depended on anything at all would fail the restore outright
rather than resolve it quietly from the internet. Rule C2 asserts the same
property from the other side, against the produced `.nuspec` files.

**The API baseline is a file and not a set of properties.** Group V has fixed
named properties of the public surface since VM-1 - that a frozen name is
exported, that no member returns a task, that no member is called `Grant` - and
every one of them is a claim about what must or must not be there. None is a
claim about what IS there, so a member added tomorrow that breaks no V rule is
an addition nothing in this repository notices, and a member deleted tomorrow is
a breaking change nothing notices either. VM-6 mints rule group **M**, whose
single rule compares `docs/api/public-api.txt` against the built assemblies in
both directions.

It is regenerated by `BROILER_API_WRITE=1`, the same shape the Code Assurance
generator uses and for the same reason: a baseline that regenerated itself on
every run would agree with every change, and one that could only be hand-edited
would be hand-edited wrongly. The switch makes the update an act, and the diff
is what a reviewer reads. A reviewer who regenerates without reading has
defeated it, which is exclusion EX-99.

**C1, C2 and C3 are promoted, and B3 is not.** The three pack rules were minted
at VM-0 with activation milestone VM-6 and were Deferred ever since for the
honest reason that no pack step existed. VM-6 runs one, so they are asserted -
against the pack log and the `.nuspec` files the collection now retains, which
carries rule K3's limit that the comparison is with the last collection rather
than the working tree. B3 stays **Vacuous**, and its row now says why that did
not change: rules A1 and A2 stop a Broiler assembly reference being constructed
at all, so nothing in the graph can violate it however many milestones pass.

**A third lane, and it has never run.** `.github/workflows/broiler-vm.yml`
joins the two lanes the 2026-08-29 revision above records. Those already
discharged exclusion EX-06 in part, so this one narrows what is left rather
than closing it. The workflow's
own header says it has not run on a hosted runner, its RID matrix is
aspirational for every entry except `linux-x64`, and `docs/support.md` claims no
platform on the strength of the file existing. A workflow that has not run is a
plan, and a support table that treated one as evidence would be the untruthful
claim roadmap section 16 stops a release for.

**What VM-6 does not finalize.** The package boundary is unchanged from VM-0's
hypothesis because nothing since has justified changing it: three packages, one
of which depends on the other two, and no evidence in five milestones has argued
for a fourth or for a merge. "Finalize only the boundaries justified by VM-0
evidence" is satisfied by leaving them alone, and saying so is more useful than
a revision that moves something to look decisive.

---

### 2026-08-31 - the JavaScript profile's project shells, and where a profile component lives

Milestone JS-0 of the `Broiler.VM.Profile.JavaScript` roadmap owns a placement
decision, and this record is the half of it that belongs to the core's topology
owner. The profile's own half - its identity, its manifests, its budget
declarations, its waited-on set - is recorded in
`src/Broiler.VM.Profile.JavaScript/docs/decisions/`, which is that component's
own series and not this one.

**Where the profile lives.** The two profile roadmaps were staged in the
aggregate repository beside the components they would sit next to, and the
JavaScript profile's status ledger recorded that the component had no repository
of its own and that JS-0 owned moving them. They now sit in THIS repository, at
`src/Broiler.VM.Profile.JavaScript/` and
`src/Broiler.VM.Profile.WebAssembly/`, which is the product-project path shape
this record already fixes. So a language profile is a set of product projects in
the Broiler.VM component rather than a component of its own, and its roadmap
documents live inside the project directory whose assembly they describe.

**This revises a rejection, and the rejection is worth reading before the
revision.** The "Decision: five projects" section rejects "a product-located
profile shell, which would make the fixture-containment rule vacuous". That
rejection was right and is unchanged for the subject it had: the FIXTURE profile
is test-only, and what keeps it out of a shipped image is its path under
`src/tests/`, which rules A4 and A5 hold. A product-located fixture would have
dissolved that. A product-located PRODUCT profile is a different subject - it is
meant to ship, it is what `src/compositions/` exists to link, and the
containment rule it would have to violate is one nobody wrote because no product
profile existed. Rules A4, A5 and A11 keep the fixture exactly where it was.

**The budget.** JS-0 adds three projects and no packable assembly:

| Project | Path | Kind | Why |
|---|---|---|---|
| `Broiler.VM.Profile.JavaScript.Format` | `src/Broiler.VM.Profile.JavaScript.Format/` | product | The bytecode format. The PIVOT: the executor and the lowering must agree on the bytecode and neither may depend on the other, so both reference this and it references nothing. Rule N3 keeps it a sink |
| `Broiler.VM.Profile.JavaScript` | `src/Broiler.VM.Profile.JavaScript/` | product | The profile: descriptor, verifier, executor, value and object model, standard library, host imports. Rule N1 fixes its reference set |
| `Broiler.VM.Profile.JavaScript.Compiler` | `src/Broiler.VM.Profile.JavaScript.Compiler/` | product | The lowering. A sibling of the profile, never a part of it, so that an execution-only composition contains no compiler |

The graph goes from 14 projects and 41 edges to 17 and 46, and product projects
from 3 to 6. **The packable set is unchanged at exactly three.** All three new
projects declare no `PackageId` and carry the literal
`<IsPackable>false</IsPackable>`; rule N4 asserts both halves, and the clause
above requiring a dated revision for a fourth packable assembly is therefore not
exercised by this one either. Packaging the profile is milestone JS-10's
decision and it will need its own revision here.

**Why three assemblies rather than one.** Each enforces a boundary that cannot
be enforced inside another, which is the test this record applies to every
assembly it authorises. The format is a DEPENDENCY boundary: two consumers must
agree on it and neither may see the other, and one assembly holding format and
profile together would put the format's second consumer - the lowering - on the
profile's graph. The lowering is a DEPLOYMENT boundary: the difference between
an execution-only composition and a runtime-compiler one is whether this
assembly is in the closure, and a build switch inside one assembly is not a
closure report. The profile itself is the OWNERSHIP boundary a composition root
names. Whether the profile later splits further - a value and object model apart
from the standard library - is left open and needs its own revision.

**What the profile's reference set is, and which record admits each part.** The
profile references `Broiler.VM.Abstractions`, `Broiler.VM.Binary` and its own
format sibling. The first two are ADR 0011's obligation P1. The third is P1's
2026-08-31 revision, which states that the set P1 bounds is of
**Broiler.VM-owned** assemblies and that a profile component's own siblings are
not members of it. Rule N1 asserts the whole set in one place, and refuses the
runtime and the lowering by their own witnesses.

**One core rule changed.** A11 forbade every reference to a
`Broiler.VM.Profile.*` assembly from outside a composition root. It was written
when no product profile existed, and it makes the format pivot illegal. A11 now
exempts a sibling **in the same profile family**, keyed on the language segment -
so `Broiler.VM.Profile.JavaScript.Compiler` may reference
`Broiler.VM.Profile.JavaScript.Format`, and a JavaScript project referencing a
WebAssembly one is still a violation. Rule N2 asserts that second half in both
directions with its own witnesses, because an exemption keyed on the
`Broiler.VM.Profile.` prefix would have dissolved the cross-profile boundary
that the extraction gate's fourth condition exists to keep.

**A13 was not widened, and that is a decision rather than an oversight.** Its
subject is a test-only consumer profile and it states a reference set of exactly
two assemblies. A product profile with a format sibling has three, so widening
A13 to cover it would have loosened the rule that holds
`Com.Example.Calculator` and `Com.Example.Ledger`. N1 states the stronger claim
over its own subject instead, and the two rules now cover the two kinds of
profile this component contains without either weakening the other.

**What JS-0 does not do.** It lands no product code: the three assemblies
contain one `AssemblyMarker` each and nothing else, for the same reason VM-0's
did. There is no descriptor, no format, no verifier, no executor, no feature
manifest and no composition root, and the profile's status ledger says so. The
assurance system's covered set grows to six product assemblies, which is the
product/test partition applied unchanged rather than a new policy - decision
JSD-0006 records why the profile adopts this component's assurance system rather
than standing up a second one over the same tree.

---

### 2026-08-31 - the JavaScript profile's two composition roots

Milestone JS-1 of the `Broiler.VM.Profile.JavaScript` roadmap closes the whole
contract loop on one feature manifest, and it needs somewhere to run. This
record authorises the projects; the profile's own JSD-0008 records what they are
and why there are two.

**The budget.** JS-1 adds two projects and no packable assembly:

| Project | Path | Kind | Why |
|---|---|---|---|
| `Broiler.VM.Composition.JavaScript.ExecutionOnly` | `src/compositions/` | composition root | The execution-only image: format, verifier, executor and **no compiler**. It runs precompiled artifacts read as bytes |
| `Broiler.VM.Composition.JavaScript.SliceCompiler` | `src/compositions/` | composition root | The producer: it carries the lowering, writes the retained corpus, and holds the checks that need a second profile in the catalog |

The graph goes from 17 projects and 46 edges to 19 and 55. **The packable set is
unchanged at exactly three**, and both roots carry the literal
`<IsPackable>false</IsPackable>`.

**Why two projects and not two modes of one binary.** They differ by exactly one
reference - the lowering - and that difference *is* the `execution-only`
composition label. A flag on one binary would have made the difference a run-time
choice inside one closure, and a closure report cannot see a flag. The published
closures are what settles it: six managed assemblies for the execution-only image
and seven for the compiler-bearing one, differing by
`Broiler.VM.Profile.JavaScript.Compiler` and nothing else.

**A naming collision, recorded because it is a property of the rules rather than
a preference.** The JavaScript roadmap proposes the name
`Broiler.VM.Profile.JavaScript.Composition.*` for these roots. It cannot be used:
every rule that identifies a profile assembly does so by the
`Broiler.VM.Profile.` prefix, so a composition root under that prefix *is* a
profile assembly to rules A8, A11 and A13 - and A8 fired on the first build,
correctly, because a composition root must reference the runtime. The roots take
this record's own `Broiler.VM.Composition.*` shape instead, which puts them where
A12 and the composition register can hold them.

**Three rules in group K changed, and each change is a widening the register
needed rather than a relaxation.**

- **K2 no longer forbids the reserved first label; it checks the pairing.** The
  reserved `broiler` label is reserved FOR Broiler, and what the core refuses at
  catalog construction is a profile claiming it without a `Broiler.*` package
  identity. Forbidding the namespace outright was indistinguishable from the
  pairing rule while every composed profile was a consumer one under a
  documentation domain, and became wrong the moment a genuine Broiler-owned
  profile was composed.
- **K2's reference-set comparison is now two directions rather than one
  equality.** Every non-core assembly a root references must be declared; every
  declared PROFILE must be referenced. A declared SIBLING need not be, because a
  profile's format assembly arrives transitively and appears in no composition's
  project file while being unmistakably in every published closure.
- **The register gains a sibling column and an evidence column.** The first
  because the schema had no place for a lowering - an assembly in the closure
  that no profile comes from - and the second because this repository now has two
  milestone series, so rules K3 and K4 read the bundle each row names instead of
  the core's current one. Without the second, a JavaScript closure would have
  been compared against a file nobody wrote for it.

**One measurement correction that is not this record's but is recorded where a
reader will look.** Exclusion EX-42 says the Native AOT publish step needs a
`vcvars64` shell on Windows. It does not. It needs `vswhere.exe` on `PATH`: the
ILCompiler package's own `findvcvarsall.bat` calls it unqualified, and when it is
missing the batch file's error text is substituted into the property that becomes
the linker path, so the publish fails with MSB3073 naming a command that reads as
a sentence. Adding the Visual Studio Installer directory to `PATH` publishes and
runs; a `vcvars64` shell without it still fails. The JavaScript collection script
does that and says so at the constant.

---

### 2026-09-01 - the CI lane has run, and what that does not settle

**What the record said.** That `broiler-vm.yml` "has never run on a hosted
runner", and the 2026-08-30 revision above that "a workflow that has not run is
a plan" whose RID matrix "is aspirational for every entry except `linux-x64`".
The workflow's own header carried the same note, and `docs/support.md` carried
it a third time as part of exclusion EX-45.

**What is now true.** The lane has run on hosted runners and passes. Across two
pull requests it executed on `ubuntu-latest`, `windows-latest` and
`macos-latest`, and each of the three `publish and run` jobs published every
composition root as Native AOT with trim and AOT warnings as errors and ran it -
`linux-x64`, `win-x64` and `osx-arm64`. The `win-x64` job publishes a Native AOT
image on a stock runner with no `vcvars64` shell, which is the second
independent confirmation of the EX-42 correction recorded above.

**What it does not settle, and this is the load-bearing half.** **No RID claim
moves**, and the three unclaimed rows in `docs/support.md` stay unclaimed. The
workflow's own header states the reason and it has not changed: **this lane does
not collect an evidence bundle.** A bundle is collected deliberately, by a
person, into `docs/evidence/`, and it records the machine, the SDK, the
effective configuration and the raw outputs that make a figure readable. A green
job on an ephemeral runner nobody names records none of that. The distinction
the 2026-08-30 revision drew - a workflow that has not run is a plan - was right,
and its converse is not: **a workflow that has run is not a bundle.**

So what changed is narrower than "CI works" and worth stating exactly. Before,
a reader was told nothing had ever published on `win-x64` or `osx-arm64`. Now
something has, repeatedly, and the reason those rows are unclaimed is no longer
that the platform is unattempted - it is that no retained collection exists on
them. That is a better reason and a smaller gap, and pretending it is no gap
would be the untruthful claim roadmap section 16 stops a release for.

**What is not edited.** The 2026-08-30 revision above stands unchanged. It
described what was true when it was written, and the append-only rule at the top
of this section is what makes it worth reading; this entry is where a reader
learns the sentence has been overtaken.

---

### 2026-09-01 - the lane's third RID is withdrawn

**What the record said.** The revision above, written earlier the same day: that
the lane had run on `ubuntu-latest`, `windows-latest` and `macos-latest`, and
published and ran every composition root as Native AOT on `linux-x64`, `win-x64`
and `osx-arm64`.

**What is now true.** It runs the first two. `osx-arm64` is a RID ADR 0012 marks
*Reserved - no evidence*, its declared matrix is exactly { win-x64, linux-x64 },
and the ground that record gives for choosing two is that the consuming
repository publishes those two and no other. It still does, and it has no macOS
head - so the lane was publishing a RID nothing consumes and the support table
was carrying a row for a RID the ADR says may never have one. ADR 0012's own
2026-09-01 revision records the withdrawal, what the consuming repository's
Android head does to the Android exclusion's reasoning, and the arm64 gap the
withdrawal leaves named rather than covered.

**What it does not settle.** Exactly what the revision above did not settle, and
for the same reason: no RID claim moves, because this lane collects no bundle.
The sentence that entry turned on is unaffected - a workflow that has run is
still not a bundle - and withdrawing a lane entry removes a publish nobody
consumed rather than any evidence, because none was retained.

**What is not edited.** The revision above stands as written. It described what
was true when it was written, on the day it was written, and this entry is where
a reader learns the RID list in it has been narrowed by a decision recorded
elsewhere.

---

### 2026-09-01 - and a fourth job, because the matrix widened

**What the record said.** The revision above: that the lane runs `linux-x64` and
`win-x64`, after `osx-arm64` was withdrawn as outside ADR 0012's declared matrix.

**What is now true.** It runs three. ADR 0012's third revision of the same date
declares `linux-arm64` - for the architecture rather than for a consumer, since
nothing publishes it - and the lane follows the matrix, which is the order the
withdrawal above exists to restore. That record carries the grounding and the
limit; this one records only that the lane now has the job, on an
`ubuntu-24.04-arm` runner.

**What it does not settle.** Nothing this lane ever settles: it collects no
bundle, so no RID claim moves, and the new job is attempted-and-passing at best.

**What is not edited.** The three revisions above stand as written.

---

### 2026-09-01 - a fifth job, for the same reason as the fourth

**What the record said.** The revision above: that the lane runs three RIDs
after ADR 0012 declared `linux-arm64`.

**What is now true.** It runs four. ADR 0012's fourth revision of the same date
declares `win-arm64` - for the Windows Native AOT toolchain targeting arm64,
which neither `win-x64` nor `linux-arm64` compiles - and states the rule all four
rows follow, so that a fifth is added by argument rather than by resemblance. The
lane follows the matrix, as it now does in both directions.

**What it does not settle.** Nothing this lane ever settles: it collects no
bundle, so no RID claim moves.

**What is not edited.** The four revisions above stand as written.

---

### 2026-09-01 - six jobs, and one of them is a RID this lane dropped in the morning

**What the record said.** The revision above: that the lane runs four RIDs after
ADR 0012 declared `win-arm64`.

**What is now true.** It runs six. ADR 0012's fifth revision of the same date
declares both macOS RIDs - `osx-arm64` for the clang-into-Mach-O toolchain that
no other declared RID reaches, `osx-x64` for the remaining cell of the grid - and
records that the entry which called macOS reserved had read only the
*architecture* limb of its own rule. `osx-arm64` is therefore the same RID the
second revision withdrew from this lane that morning, back by a stated ground
rather than by the drift that had put it there.

**Two things worth carrying here, because they are about the lane rather than the
matrix.** A job naming the retired `macos-13` label does not fail - it queues for
a runner that never arrives, so the run hangs; the successor Intel image took the
same job in seconds. And one `win-arm64` attempt of three failed inside
`actions/setup-dotnet` with an access violation before any build, passing either
side of it.

**What it does not settle.** Nothing this lane ever settles: it collects no
bundle, so no RID claim moves.

**What is not edited.** The five revisions above stand as written, the withdrawal
included.

---

### 2026-09-02 - a second solution, and a composition root that is an application

**What the record said.** That `Broiler.VM.slnx` is the frozen project graph and
the group A rules read it, with rule A14 reporting any project file it does not
list that is not a sample.

**What is now true.** The graph is the union of the solutions
`ComponentGraph.SolutionFiles` names, and there are two:
`Broiler.VM.slnx` and `Broiler.VM.Mobile.slnx`. Every group A rule governs both
sets identically - A7's edge multiset, A11's profile-reference rule, A12's
composition-root shape, A14's own complement. **The named list is the safeguard**:
a glob for `*.slnx` would let a project escape every one of those rules by
arriving with a solution of its own, so adding a solution is an edit to the test
assembly, which is a review.

**Why the split exists at all.** `Broiler.VM.slnx` must build on a machine with
the .NET SDK and nothing else. The mobile heads target frameworks that need a
workload, and an iOS one would need Xcode; folding them in would make the
ordinary build - the one a contributor runs before pushing - fail without an
installation this component has never required. The consuming repository reached
the same shape for the same reason and keeps its Android head in a solution of
its own.

**And the first composition root that is an application rather than a console
program.** `Broiler.VM.Composition.JavaScript.Android` composes what the
execution-only root composes and differs by its target framework, which is the
only way an Android RID can be published at all. Its checks are the
execution-only root's own source files, compiled into it rather than
re-implemented: the question it exists to answer is whether *this component's
code* runs there, and a re-implementation would answer a question about a second
piece of code. Rule A3 permits it - those items resolve inside the component
root, which is what A3 is about.

**What it does not settle.** No RID claim moves, and the head's collection -
bundle JS-ANDROID-001 - is a collection on an emulator, which the bundle's own
exclusions put first.

**What is not edited.** Every revision above stands as written.

### 2026-09-02 - the budget entry the revision above did not carry

**What the record said.** The revision above adds
`Broiler.VM.Composition.JavaScript.Android`, states what it composes, why it is
an application rather than a console program, and what it does not settle. **It
carries no budget paragraph.** Every other project-adding revision in this record
carries one, and this record's budget section is what authorises a project set at
all: it says the set "may not grow without a dated revision".

**What is now true.** The graph goes from 19 projects and 55 edges to 20 and 59.
**The packable set is unchanged at exactly three**, and the head carries the
literal `<IsPackable>false</IsPackable>`.

**How the omission was found, and why it could stand.** A sweep of this record's
prose for figures the tree contradicts, run first over the status ledger and
extended here. **Nothing compared this record's stated graph size to the graph.**
Rule A7 holds `graph.manifest.json` to the project files and both are the tree,
so a budget sentence that stopped being true was a sentence no rule read - the
same shape as rule J10's register row, one document over. Rule **A15** reads the
last budget sentence here and compares it to `ComponentGraph`, so the next
project-adding revision that omits its budget fails the suite rather than waiting
for a sweep.

**What is not edited.** Every revision above stands as written, including the one
this completes.


### 2026-09-03 - the conformance harness, a root that is never advertised

**What changes.** `Broiler.VM.Composition.JavaScript.Conformance` is added under
`src/compositions/`. It is the conformance oracle roadmap section 14 specifies:
it reads a suite of tests, lowers each one, verifies the artifact, runs it, and
compares what happened against what the test declared.

**Why it is a composition root and not a test project.** Scoring a test means
driving this profile's own lowering, verifier and executor. Rule A11 forbids a
test project to reference a profile assembly and rule A12 forbids a composition
root to reference the fixture assembly, which between them leave the harness
nowhere else. That is the same argument the retained corpus's producer and the
execution-only root's replay already stand on, and the JavaScript profile's
roadmap section 5 states it in advance rather than leaving it to be discovered
here.

**What it composes.** One profile, `broiler.javascript`, through its own static
accessor, with the lowering in its reference set because a conformance test is
source. Its reference set is therefore the slice-compiler root's, and it composes
no second profile and registers no capability.

**What is now true.** The graph goes from 20 projects and 59 edges to 21 and 64.
**The packable set is unchanged at exactly three**, and the root carries the
literal `<IsPackable>false</IsPackable>`.

**One rule is minted with it, and it is not a formality.** This root is the
ingestion path for a conformance suite, which is separately licensed third-party
material. Rule **N13** asserts that the harness appears in no package and in no
advertised composition's closure, that no other project references it, and that
no project file names a suite directory - the last being how suite files would
reach a build output without any reference changing. It is deliberately not
phrased as "no published closure", because this root publishes one of its own
for its own evidence. Its negative controls add the reference from the
execution-only root and add a content item carrying the suite, which are the two
directions that would actually ship.

**What it does not settle.** No RID claim moves, no composition becomes
advertised, and no conformance suite is pinned: the suite this root reads today
is the component's own fixture tree, and retrieving, hashing and archiving a
third-party one is the human action the profile's ledger still records as open.

**What is not edited.** Every revision above stands as written.

### 2026-09-03 - the component lane is split, and it gains a concurrency group

**What the record said.** The row above described three lanes, one of which -
`broiler-vm.yml` - ran the whole declared RID matrix, the fuzz sessions and the
Android emulator on every push and every pull request.

**What was actually true.** It carried no concurrency group, so three pushes in a
row left three complete matrices running at once, and the matrix itself took
**26m56s**. The breakdown says where that went: the six publish jobs took 759,
882, 917, 1041, 1051 and 1405 seconds, and the fuzz-and-mutate step was **88 to
91 per cent of every one of them**. Everything else in a publish job - six Native
AOT publishes of four composition roots, running each, and the package consumer -
is 78 to 208 seconds. A fuzz session is a total function of its seed and its seed
corpus, so the matrix was answering one deterministic question six times.

**The step's own comment said those sessions "cost well under a second on the
published image".** That was never measured and is why the cost was invisible for
as long as it was. It is corrected in place.

**What replaced it.** One lane definition with two callers.
`broiler-vm-lane.yml` holds `checks`, `publish`, `mobile` and `android` and is
entered only through `workflow_call`; it takes a publish matrix, a fuzz iteration
count and whether the emulator runs, **and nothing else** - so a caller cannot
disable a check, which would have made two gates wearing one name.
`broiler-vm.yml` runs on every push and pull request over `linux-x64` and
`linux-arm64` with 2,000-iteration sessions and a build of the Android head, under
a concurrency group that cancels the run a previous push started.
`broiler-vm-full.yml` runs all six cells with 25,000-iteration sessions and the
emulator, on a `v*` tag - the trigger `release.yml` already publishes on - on a
weekly schedule, and on a button, and it deliberately does **not** cancel in
progress.

**What is now true, and what is not.** The push path is two cells rather than six,
so it is **blind to a Windows-only and a macOS-only regression until the full lane
runs** - and this component has had one, the soak's heap plateau failing on macOS
under Native AOT on both architectures while every other RID stayed flat. The
quick lane is a fast signal; the full lane is the gate, and a release goes through
it. **No RID claim moves**, because a lane is not an evidence bundle and this
revision changes nothing about what a collection is.

**What is not edited.** Every revision above stands as written.

### 2026-09-03 - the ingestion path, and a rule about which refusals mean anything

**What changes.** No project is added and no edge moves. The conformance root
gains the path that reads a third-party suite's own metadata dialect and
translates it into cases this harness can score, and a second pinned fixture
suite is added under `src/tests/conformance/`, written in that dialect.

**Why it was needed, which is not the reason it was planned.** The harness's
metadata reader carried a remark that its shape was the suite's own, "so that the
day a pinned suite is retrieved, this reader is pointed at it rather than
replaced". Five files written in the real dialect were put through the harness to
check that. **All five were refused**, and a suite is read whole, so the run
scored nothing at all - not a wrong total, no total. The reader was shaped like a
simplification of the dialect: flat `key: value` lines, no nested mapping, no
block scalars, and one required key that dialect does not have.

**The part worth recording in a topology record.** One rule in that path is not
about metadata at all. `broiler.javascript.slice` admits no function, no object,
no string value and no property access, so it refuses valid JavaScript on almost
every line - with one diagnostic code. A third-party suite's negative tests almost
all declare that a refusal must happen. Compared on the observable outcome the two
agree nearly every time, for reasons that have nothing to do with each other, and
**a manifest admitting almost nothing would have reported a near-perfect
conformance total.** Every source-refusal code therefore carries a declared
language class, and only a genuine early error may answer such a test; a refusal
this profile did not earn makes the case unscorable rather than passed. The rule
runs ahead of the comparison, so no declaration can be written that gets past it.

**What this does to the graph and the closures.** Nothing. The new code is
`internal` to a root that is never advertised, rule N13's clauses are unchanged
and still hold, no package gains a dependency, and no advertised composition's
closure moves. The component lane runs both suites on every publish and compares
each against its own floor.

**What is now true, and what is not.** The harness can read a real suite's
dialect; it has read none. No third-party suite is in this repository, none is
pinned, and nothing here fetches one. Section 3's suite-revision blocker narrows
from "a person and a reader" to "a person", and does not close. **No RID claim
moves and nothing is accepted.**

**What is not edited.** Every revision above stands as written.
