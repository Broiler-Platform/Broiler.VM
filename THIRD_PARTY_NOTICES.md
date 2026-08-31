# Third-party notices

**The three Broiler.VM packages carry no third-party code and no third-party runtime dependency.**

That is a stronger statement than "we found nothing to list", so it is worth saying how it is
known and what it excludes.

Broiler.VM is licensed under Apache-2.0. See [LICENSE](LICENSE).

---

## What ships

| Package | Third-party code | Package dependencies |
|---|---|---|
| `Broiler.VM.Abstractions` | None | None |
| `Broiler.VM.Binary` | None | None |
| `Broiler.VM.Runtime` | None | `Broiler.VM.Abstractions`, `Broiler.VM.Binary` - both listed above, both Apache-2.0, both this component's |

Every type in all three is written for this component. Nothing is vendored, nothing is copied from
another project, and nothing is generated from a third-party schema.

Rule **C2** asserts the dependency half against the `.nuspec` of every produced package in the
current evidence bundle, and it asserts more than the absence of stray *Broiler* dependencies: it
asserts that **every declared dependency is one of the three**. The pristine feed consumer is the
same claim from the other side - its only restore source is a directory holding these three
packages, with nuget.org unreachable, so a package that depended on anything else would fail that
restore rather than resolve quietly.

The framework itself - .NET 10 - is not a third-party dependency to be listed here. It is the
platform, it is referenced through the SDK rather than through a package, and its own notices ship
with it.

---

## What is used to build and test, and does not ship

These are development-time only. None of them is in any package, in any published closure, or in
any Native AOT image.

| Component | Version | License | Why it is here | How it is kept out |
|---|---|---|---|---|
| `Microsoft.SourceLink.GitHub` | 8.0.0 | MIT | Stamps the repository and commit into the built assemblies so a package can be traced to its source | `PrivateAssets="all"` in `eng/Broiler.Packaging.props`, so it is not a dependency of anything produced |
| `xunit` | 2.5.3 | Apache-2.0 | The test framework | Referenced only by test projects. Rule A4 forbids a product project to reference a test project; rule B1 asserts no product assembly references a non-framework assembly |
| `xunit.runner.visualstudio` | 2.5.3 | Apache-2.0 | Runs the tests | As above |
| `Microsoft.NET.Test.Sdk` | 17.8.0 | MIT | Test host | As above |
| `Microsoft.CodeAnalysis.CSharp` | 5.3.0 | MIT | Used by the architecture suite to read source for the Code Assurance rules | Test-only. It is the reason rule B1 has a real witness: the test assembly genuinely violates the framework-only rule, which is what makes that rule falsifiable |

**The closure reports are the check that this table is true.** Every published composition's
closure is listed in each evidence bundle, and rule K4 asserts that a closure contains exactly the
composition, the three core assemblies and the profile assemblies its register row declares - no
fixture, no testing framework, no reflection-emit assembly. A build-time dependency that leaked
into a shipped closure would fail that rule rather than survive as a missing row here.

---

## What this notice does not cover

- **No license scan has been run by a tool.** The table above is the result of reading every
  `PackageReference` in the repository, and the repository has five. A component with a large
  dependency graph would need a scanner; this one has a graph small enough to read, and saying so
  is more honest than implying a process that has not happened.
- **No third-party security advisory feed is monitored.** `docs/support.md` section 6 records that
  the vulnerability-response gap is a gap.
- **Transitive dependencies of the development-time packages are not enumerated.** They are the
  test toolchain's own, they ship in nothing, and enumerating them would suggest they are part of
  the product.

If a runtime dependency is ever added to any of the three packages, this file must gain a row for
it **in the same change**. ADR 0001 records that requirement as the reason this file was deferred
rather than created empty: an empty notices file asserts a license pass that has not happened.

## The scope of the claim, and who may falsify it

The opening sentence is scoped to **the three Broiler.VM packages**, and that scoping is
load-bearing now that other components are being written against this one. Two profile roadmaps
plan trees that will contain third-party-derived material: one starts from a snapshot copy of an
Apache-2.0 engine that is itself derived from an upstream project, and the other ingests a
third-party conformance suite as test-only material.

**Neither falsifies the sentence above, and neither may be allowed to falsify it silently.** A
profile is a separate component with its own packages, its own licence file, and its own notices;
nothing it vendors ships in `Broiler.VM.Abstractions`, `Broiler.VM.Binary`, or
`Broiler.VM.Runtime`, and no gitlink or project reference runs from any of the three to any
profile. The claim stays true because it is a claim about *these three packages*, not about the
repository they sit in or the components that reference them.

What follows is an obligation on the other side rather than on this one. **A component that
ingests or copies third-party source confirms this scoping, or amends this file, in the change
that introduces the material** - with the release owner co-signing, because the confirmation is a
release-facing statement and not a housekeeping edit. Both profile roadmaps carry that as a
milestone item. This section exists so that the confirmation has something specific to confirm
against, rather than a reader inferring the scope from a sentence that does not state it.

**Ingesting components, as of 2026-08-31: none.** This list is empty and is expected to gain
entries; an entry names the component, what it ingests, and the date the scoping was confirmed.
