# JSD-0001 - Placement, identity, and the assembly topology

**Status:** Accepted for JS-0, with one clause provisional and named below.

**Date:** 2026-08-31

**Owner:** profile architecture owner. **Co-signer:** the core's topology owner, for the
placement half. **Both roles are held by one person**, so the co-signature is not independent
and this record does not claim that it is.

**Milestone:** JS-0.

## Context

The roadmap's [section 5](../roadmap.md#5-package-boundaries-and-the-dependency-graph) offers
package names as hypotheses and requires JS-0 to prove the graph with project shells and an
explicit assembly budget. The status ledger recorded that this component had no repository of
its own, that its two documents were staged in the aggregate repository, and that **JS-0 owns
the placement decision and moving them is part of it rather than a side effect of it**.

Section 5 also left one question open and marked it as the core's to answer: the frozen
profile-facing contract states a profile's reference set as exactly two core assemblies, and the
graph section 5 draws adds a third `Broiler.VM`-named assembly - this profile's own format. The
section says the placement decision cannot be taken correctly until the core has ruled.

## Decision: the profile is a set of product projects inside the Broiler.VM component

`Broiler.VM.Profile.JavaScript` is **not a component of its own**. It is a family of product
projects in the `Broiler.VM` repository, at the product path shape ADR 0001 fixes -
`src/<AssemblyName>/<AssemblyName>.csproj` - and its roadmap and decision records live inside
the project directory of the assembly they describe, at
`src/Broiler.VM.Profile.JavaScript/docs/`.

The core's half of this decision is **ADR 0001 revision 5** (2026-08-31), which authorises the
three projects, records the budget growth from 14 projects to 17 with the packable set unchanged
at three, and revises its own earlier rejection of a product-located profile shell by
distinguishing the fixture profile - whose containment is its test-only path - from a product
profile, which is meant to ship and which `src/compositions/` exists to link.

**What this changes about the roadmap, stated rather than left for a reader to notice.** The
roadmap is written for a component with its own repository, its own licence and notice files, its
own assurance system, its own rule register, its own evidence tree and its own packages. Four of
those are now the component's rather than this profile's, and each is recorded as a deviation
with its reason in [JSD-0006](0006-assurance-evidence-and-rules-adoption.md) rather than being
quietly dropped. What does **not** change is the discipline the roadmap exists to enforce: no
evidence from any other component is this profile's evidence, no core gate closes a JS gate, no
JS gate closes a core gate, and the two ledgers stay apart.

**Rejected: a separate repository added as a submodule.** It is the shape the roadmap assumed
and it remains available later. It is not taken now because it buys a boundary this profile does
not need against the core - the two are developed together, the profile compiles against the
core's source contract, and a submodule boundary would mean every contract change crossed a
repository and a package version before the profile could compile against it - and because the
one boundary that genuinely matters, the **fork from the legacy JavaScript engine**, is a
boundary against a component that is not in this repository at all and is held by rules A1, A2,
A3, B3 and D1 whatever this component's repository is.

**Rejected: a nested component tree** - `src/Broiler.VM.Profile.JavaScript/src/...` with its own
`Directory.Build.props`, `NuGet.config` and solution. It would put a second component inside the
first's product partition, break ADR 0001's path shape, and give the checkout two answers to
"which properties does this project evaluate".

## Decision: the identity

| Row | Value | Authority |
|---|---|---|
| Profile ID | `broiler.javascript` | Two to eight dot-separated ASCII labels; first label `broiler` is reserved and obliges a `Broiler.*` package identity |
| Package identity | `Broiler.VM.Profile.JavaScript` | The reserved first label's obligation. **Declared in the descriptor at JS-1; not a NuGet PackageId yet** - see below |
| Feature manifest shape | `broiler.javascript.<surface>` | A manifest ID must begin with its profile's ID followed by a dot and at least one further label |
| Payload kind-ID range | Reserved at JS-1 with the descriptor | Every payload kind this profile mints must lie inside its declared range |
| Diagnostics identity | `broiler.javascript.diagnostics` | Minted at JS-3a with the registry |
| Conformance manifest identity | `broiler.javascript.conformance` | Minted at JS-3a with the harness |

**The package identity and the NuGet package ID are two different things and JS-0 separates
them.** The descriptor's `packageIdentity` row is a claim about who owns the profile ID, and the
reserved `broiler` label obliges it. A NuGet `PackageId` is a shipping decision, and it is
JS-10's. None of the three projects declares one, all three carry
`<IsPackable>false</IsPackable>`, and **rule N4 asserts both halves** so the ledger's standing
claim that nothing here is packable cannot decay into prose nobody checks.

## Decision: three assemblies

| Assembly | Boundary it enforces | References |
|---|---|---|
| `Broiler.VM.Profile.JavaScript.Format` | Dependency. The **pivot**: the executor and the lowering must agree on the bytecode and neither may depend on the other | nothing at all |
| `Broiler.VM.Profile.JavaScript` | Ownership and package. The descriptor a composition root names | `Broiler.VM.Abstractions`, `Broiler.VM.Binary`, and the format |
| `Broiler.VM.Profile.JavaScript.Compiler` | Deployment. Whether this assembly is in the closure **is** the difference between an execution-only composition and a runtime-compiler one | `Broiler.VM.Abstractions` and the format |

Each is held by a registered rule with a passing witness and a violating input: **N1** fixes the
profile's reference set and refuses the runtime, the lowering, a package reference and an
internals grant; **N2** refuses an edge between two profile families in either direction; **N3**
keeps the format a sink; **N4** keeps every family project unpackable.

**Rejected: one assembly.** The single-assembly default needs no justification and a split does,
so here is the justification. Folding the format into the profile puts the format's second
consumer - the lowering - on the profile's dependency graph, which is the one thing the pivot
exists to prevent. Folding the lowering into the profile makes the execution-only composition a
build switch inside one assembly rather than a closure report, and the roadmap's whole Native AOT
discipline is that a closure is read off a published output rather than inferred from a property.

**Deferred, and not decided here:** whether the profile assembly later splits further - a value
and object model apart from the standard library. The single-assembly default holds until
evidence argues otherwise, and a split needs its own dated record and its own ADR 0001 revision.

## The section 5 open question is settled, and it was settled by the core before this record

Section 5 asked whether a profile's own sibling assemblies sit outside the "exactly two core
assemblies" reference set. **They do.** ADR 0011's obligation P1 carries an editorial revision
dated 2026-08-31 stating that the set is of **Broiler.VM-owned** assemblies and that a profile
component's own siblings - its format assembly, its lowering, its composition roots - are not
members of it and P1 does not bound them. ADR 0001's own section-5 reading carries the same
qualifier and gives the same reason: the roadmap's format pivot is incoherent unless a profile
may reference its own format assembly.

So the graph section 5 draws is legal, and the risk-table row that named this hazard is closed
rather than carried. **One thing did have to change on the core's side**, and it is recorded in
ADR 0001 revision 5 rather than here: rule A11 forbade any reference to a
`Broiler.VM.Profile.*` assembly from outside a composition root, which made the pivot
unreachable. A11 now exempts a sibling in the same profile family, keyed on the language segment
so that a JavaScript project referencing a WebAssembly one is still a violation.

## What this record does not decide

- **No product code exists.** Each of the three assemblies contains one `AssemblyMarker` and
  nothing else. There is no descriptor, no format, no verifier, no executor, no manifest and no
  composition root, and nothing here may be described as implemented, supported or published.
- **No runtime identifier is claimed** and no composition is advertised.
- **The two-profile catalog test JS-0's exit gate names is not closed by this milestone.** It
  composes this profile's descriptor beside a hostile neighbour, and there is no descriptor to
  compose until JS-1 builds one - the same milestone the delivery order says lands the first
  product code. The clause is carried to JS-1 as an open gate condition, named in the JS-0
  evidence bundle's exclusions and in the ledger row, rather than satisfied with a fabricated
  descriptor. Its `eval`-refusal half needs guest-initiated loads and is carried to JS-8.
