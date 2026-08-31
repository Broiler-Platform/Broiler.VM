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

**Neither falsifies the sentence above, and neither may be allowed to falsify it silently.**

**Restated 2026-08-31, because the reason changed even though the claim did not.** This paragraph
used to give its reason as "a profile is a separate component with its own packages, its own
licence file, and its own notices". That is no longer true of the JavaScript profile: milestone
JS-0's placement decision - ADR 0001 revision 5, and the profile's own JSD-0001 - puts it in
**this repository**, as three product projects at `src/Broiler.VM.Profile.JavaScript*`, sharing
this licence file and this notice. A reason that has stopped being true is worse than no reason,
so here is the one that holds.

The claim is about **these three packages**, and it stays true for three reasons a reader can
check rather than take:

1. **The profile packs nothing.** None of its three projects declares a `PackageId` and every one
   carries the literal `<IsPackable>false</IsPackable>`; **rule N4** asserts both halves, and
   **rule A6** independently asserts that exactly three projects in the whole repository declare a
   `PackageId`. Packaging the profile is milestone JS-10's decision and needs its own revision of
   ADR 0001.
2. **The dependency runs one way.** A profile references the core; no core package references a
   profile assembly. **Rule A11** permits a reference to a profile assembly only from a
   composition root or a sibling in the same profile family, and the three packable projects are
   neither.
3. **The closure reports are the check.** Rule K4 asserts that a published composition's closure
   contains exactly the composition, the three core assemblies and the profile assemblies its
   register row declares.

So the sentence's scope is now *narrower than the repository* rather than *the same as the
component*, and it says which mechanism holds each half.

What follows is an obligation on the other side rather than on this one. **A component that
ingests or copies third-party source confirms this scoping, or amends this file, in the change
that introduces the material** - with the release owner co-signing, because the confirmation is a
release-facing statement and not a housekeeping edit. Both profile roadmaps carry that as a
milestone item. This section exists so that the confirmation has something specific to confirm
against, rather than a reader inferring the scope from a sentence that does not state it.

**Ingesting components, as of 2026-08-31: none.** This list is empty and is expected to gain
entries; an entry names the component, what it ingests, and the date the scoping was confirmed.

**One obligation moved inside this repository on 2026-08-31 and is recorded here rather than
left to be discovered.** The JavaScript profile's snapshot copy of an Apache-2.0 engine now lands
in **this tree**, at milestone JS-2, not in a repository of its own. When it does, three things
happen in the same change or the change is not complete: this file gains an entry naming what was
ingested and its upstream derivation; the copied files are marked as changed as Apache-2.0
section 4(b) requires; and the release owner co-signs the confirmation that the opening sentence
is still true. **JS-0 copies nothing** - the profile's three assemblies hold one assembly marker
each - so the list is empty today and the sentence is unamended.

**The co-signature is not independent.** Every owner and co-signer role this repository names is
currently held by one person, and this notice records that rather than resolving it by
assertion.
