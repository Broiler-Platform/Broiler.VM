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
| `Broiler.Unicode.Generation`, and `Broiler.Unicode.Algorithms` as its dependency | 0.1.0-preview.5, published to nuget.org on 2026-10-17 from `Broiler-Platform/broiler.unicode` commit `b80201436e8f8ff0402ed75c0b9657fe3616d730` (revised 2026-10-17: the cell read "0.1.0-preview.4, published to nuget.org on 2026-10-16 from `Broiler-Platform/broiler.unicode` commit `6e14f93ad753094c99f0013d22269b47550ae552` (it read "packed locally from" until then, JSC-310)") and held on the committed feed at [`eng/packages/`](eng/packages/README.md) | Apache-2.0, under this repository's own copyright holder | The owner's table generator and pin for the Unicode area (JSD-0074, decision SJL-D-6 (c)): rules N22 and N28 regenerate the embedded Unicode tables with it from this repository's pin, and rule I4 compares that pin with the owner's, which the package carries. Neither package carries a Unicode table; their own `THIRD_PARTY_NOTICES.md` names the Unicode data and ECMA-262's property tables the owner's repository archives, and the code ported from this repository *(added 2026-10-16, programme milestone SJL-7.4)* | Admitted test-only for `Broiler.VM.Architecture.Tests` alone by rule I1's allow-list, which refuses a test-only package in any shipping project; `eng/verify-feed.ps1` refuses a release that depends on one |
| `Broiler.ECMAScript.Generation` | 0.1.0-preview.5, published to nuget.org from `Broiler-Platform/broiler.js` commit `a04cd447af3424951eb767524812b5b310d71da3` and held on the committed feed at [`eng/packages/`](eng/packages/README.md), which holds the published package (revised 2026-10-17, when the package was published, JSC-313. It read "packed locally from `Broiler-Platform/broiler.js` commit `2d37a38a2560b076ebf2ca29989bfe20db64ed64` and held on the committed feed at [`eng/packages/`](eng/packages/README.md)") | Apache-2.0, under this repository's own copyright holder | The time data home's generator for the time-zone and calendar area (JSD-0076, decision SJL-D-6 (c)): rules N29 to N32 regenerate the embedded time zone and calendar tables with it from this repository's pins, and rule I4 holds the data versions it declares to those pins. It carries no table and no pinned input; its own `THIRD_PARTY_NOTICES.md` names it as code ported from this repository's architecture tests, and its compiler as written from that code and the tz source format, not from `zic`'s *(added 2026-10-17, programme milestone SJL-8.5)* | Admitted test-only for `Broiler.VM.Architecture.Tests` alone by rule I1's allow-list, which refuses a test-only package in any shipping project; `eng/verify-feed.ps1` refuses a release that depends on one |

**The closure reports are the check that this table is true, for every dependency whose name is
not a framework name.** Every published composition's closure is listed in each evidence bundle,
and rule K4 asserts that a closure contains exactly the composition, the three core assemblies and
the profile assemblies its register row declares - no fixture and no testing framework. A
build-time dependency that leaked into a shipped closure would fail that rule rather than survive
as a missing row here. A closure report lists non-framework assemblies only: the collectors drop
every name beginning with `System.` or `Microsoft.`, so a leak of `Microsoft.CodeAnalysis.CSharp`,
or of the assemblies `Microsoft.NET.Test.Sdk` brings, would not show in one. Those are held where
they would enter instead: rule A4 refuses a test-only project in a product project, and rules such
as A12, A13 and N1 refuse a package reference in a composition root, a consumer profile and the
JavaScript profile. Reflection emit is rule B5's. *(Corrected 2026-10-11: the paragraph listed "no
reflection-emit assembly" among what rule K4 asserts, and said without condition that a leaked
build-time dependency "would fail that rule". A closure report cannot carry a `System.*` or
`Microsoft.*` name, so neither held for a dependency named that way.)*

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
   **CORRECTED 2026-09-22: this reason is no longer true of the checkout, and it is corrected here
   rather than left for a reader to discover.** Since commit `d16a6aa` (2026-09-19, "Packable =
   true for nuget") all three JavaScript profile projects carry `<IsPackable>true</IsPackable>`,
   and the N4 sweep in `ProjectFileRuleTests.cs` is commented out, so rule N4 no longer asserts the
   half this sentence leans on. The profile's packages are therefore outside the opening sentence
   for a different reason: the sentence names the **three core packages**, and the profile's
   packages are separate packages. They are not free of third-party material. From 2026-09-22 two of
   them carry tables derived from the Unicode Character Database, which the section
   [Unicode data, and the tables derived from it](#unicode-data-and-the-tables-derived-from-it)
   below records. Reasons 2 and 3 are unaffected.
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

**Ingesting components.** An entry names the component, what it ingests, and the date the scoping
was confirmed.

| Component | What it ingests | Confirmed |
|---|---|---|
| `Broiler.VM.Profile.JavaScript` | **The ECMAScript Language Specification, ECMA-262, 17th edition (ES2026)**, four files - the edition and, since 2026-09-22, the three property tables it imports - retained at [`src/Broiler.VM.Profile.JavaScript/docs/specification/`](src/Broiler.VM.Profile.JavaScript/docs/specification/README.md) | 2026-09-03; the three tables not yet confirmed |
| `Broiler.VM.Composition.JavaScript.Conformance` | **test262**, the ECMAScript conformance suite, at `tc39/test262` commit `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e` — archived as the retrieved archive at [`src/tests/conformance/pins/`](src/tests/conformance/pins/README.md), with its licence beside it | 2026-09-03 |
| `Broiler.VM.Profile.JavaScript`, `Broiler.VM.Profile.JavaScript.Format`, `Broiler.VM.Profile.JavaScript.Intl` | **The Unicode Character Database 17.0.0** - thirteen UCD files and the Unicode licence text, archived at [`src/tests/unicode/pins/`](src/tests/unicode/pins/README.md) - and **tables derived from it that are compiled into the three assemblies**; see [below](#unicode-data-and-the-tables-derived-from-it) *(revised 2026-10-16, programme milestone SJL-7.4, when the Unicode area's owner's generator began writing the Intl assembly's break table from the same pin; the row named the first two assemblies alone and read "compiled into both assemblies")* | **not yet confirmed** |
| `Broiler.VM.Composition.JavaScript.Cli` | **Octane**, the retired JavaScript benchmark suite, at `chromium/octane` commit `570ad1ccfe86e3eecba0636c8f932ac08edec517` — archived as the retrieved archive at [`src/tests/octane/pins/`](src/tests/octane/pins/README.md), BSD 3-Clause, with its licence beside it as that licence requires | **not yet confirmed** |
| `Broiler.VM.Composition.WebAssembly.Harness` | **The WebAssembly specification's core test scripts**, the 73 `.wast` files of `test/core` at `WebAssembly/spec` commit `977f97014c962f7bd1291fcc6d28b41a924882bf` (tag `wg-1.0`) — archived unmodified at [`src/tests/wasm/spec/`](src/tests/wasm/spec/README.md), Apache-2.0, with its licence beside it | **not yet confirmed** |
| `Broiler.VM.Composition.JavaScript.Conformance` | **The shared conformance vectors** of the shared JavaScript libraries programme, at `Broiler-Platform/broiler.js` commit `2d496a5544937e1fcf39b59d45e7eee95ecccd2b` (corpus revision 23, pinned on 2026-10-17; revision 22, commit `4ce20ac`, earlier on 2026-10-17; revision 20, commit `a34b1f7`, from 2026-10-16; revision 19, commit `c976cbe`, earlier on 2026-10-16; revision 17, commit `520ecf9`, earlier on 2026-10-16; revision 16, commit `6f29deb`, on 2026-10-15; revision 15, commit `a3452d9`, earlier on 2026-10-15; revision 13, commit `bb1fc66`, earlier on 2026-10-15; revision 12, commit `9ae67ee`, from 2026-10-14; revision 9, commit `f2c3597`, earlier on 2026-10-14; revision 7, commit `b725bb0`, from 2026-10-13; and revision 3, commit `593f2c5`, until then) — archived as `git archive` wrote it at [`src/tests/vectors/pins/`](src/tests/vectors/pins/README.md), Apache-2.0, under this repository's own copyright holder | **not yet confirmed** |
| `Broiler.VM.Profile.JavaScript`, `Broiler.VM.Profile.JavaScript.Compiler` | **`Broiler.ECMAScript.Algorithms` `0.1.0-preview.5`**, the shared JavaScript libraries' algorithms package, published to nuget.org from `Broiler-Platform/broiler.js` commit `a04cd447af3424951eb767524812b5b310d71da3` and admitted from the committed feed at [`eng/packages/`](eng/packages/README.md), which holds the published package, under rule I1 (added 2026-10-12, programme milestone SJL-3.4; revised 2026-10-13, programme milestone SJL-4.4, when the numerics area moved to it. It read "**`Broiler.ECMAScript.Algorithms` `0.1.0-preview.1`**", "published to nuget.org from `Broiler-Platform/broiler.js` commit `2cb52757c78f793af2a275b6ea7e8730552f0a5a` and admitted from the committed feed at [`eng/packages/`](eng/packages/README.md), which holds the published package" and "It carries one third-party component, a C# port of **Ryu**'s double-to-shortest-decimal routine (Copyright 2018 Ulf Adams, Apache-2.0), named in the package's own `THIRD_PARTY_NOTICES.md`"; revised 2026-10-14, when the package was published under the shared stream's 0.1.0 prefix, JSC-303. It read "**`Broiler.ECMAScript.Algorithms` `0.2.0-preview.1`**" and "packed from `Broiler-Platform/broiler.js` commit `01209c43a3a244b9e502908e68fcded49e9bc198` and admitted from the committed feed at [`eng/packages/`](eng/packages/README.md)"; revised 2026-10-15, programme milestone SJL-5.4, when the text area moved to it. It read "**`Broiler.ECMAScript.Algorithms` `0.1.0-preview.2`**" and "published to nuget.org from `Broiler-Platform/broiler.js` commit `b3d54f7c9c01f84abbd56a823482200dd695f222` and admitted from the committed feed at [`eng/packages/`](eng/packages/README.md), which holds the published package"; revised 2026-10-15, later, when the package was published, JSC-305. It read "packed from `Broiler-Platform/broiler.js` commit `0db90422c8aca642926acff6b715d788cb17f9c6` and admitted from the committed feed at [`eng/packages/`](eng/packages/README.md)"; revised 2026-10-15, programme milestone SJL-6.4, when the Date area moved to it. It read "**`Broiler.ECMAScript.Algorithms` `0.1.0-preview.3`**", "published to nuget.org from `Broiler-Platform/broiler.js` commit `6e20998fae9b4124ab3051a773c5348eca6feca3` and admitted from the committed feed at [`eng/packages/`](eng/packages/README.md), which holds the published package" and, of what it carries, "and one table generated from the Unicode Character Database 17.0.0, the `Space_Separator` members of the white-space set"; revised 2026-10-15, later, when the package was published, JSC-307. It read "packed from `Broiler-Platform/broiler.js` commit `e04874d069f10a3f71bbcdd66e497a5b37069368` and admitted from the committed feed at [`eng/packages/`](eng/packages/README.md)"; revised 2026-10-17, programme milestone SJL-8.5, when the time-zone and calendar area moved to it. It read "**`Broiler.ECMAScript.Algorithms` `0.1.0-preview.4`**", "published to nuget.org from `Broiler-Platform/broiler.js` commit `d1e4b1163b65708ca94257d3654db133f69a833a` and admitted from the committed feed at [`eng/packages/`](eng/packages/README.md), which holds the published package" and, of what it carries, "It carries two third-party components" and "a C# port of **fdlibm** 5.3's `expm1` and `log1p` as FreeBSD's `msun` keeps them (Copyright (C) 1993 by Sun Microsystems, Inc., under the notice it carries, SPDX `SunPro`); one table"; revised 2026-10-17, later, when the package was published, JSC-313. It read "packed from `Broiler-Platform/broiler.js` commit `2d37a38a2560b076ebf2ca29989bfe20db64ed64` and admitted from the committed feed at [`eng/packages/`](eng/packages/README.md), not yet on nuget.org"). Apache-2.0, under this repository's own copyright holder. It carries three third-party components, named in the package's own `THIRD_PARTY_NOTICES.md`: a C# port of **Ryu**'s double-to-shortest-decimal routine (Copyright 2018 Ulf Adams, Apache-2.0), a C# port of **fdlibm** 5.3's `expm1` and `log1p` as FreeBSD's `msun` keeps them (Copyright (C) 1993 by Sun Microsystems, Inc., under the notice it carries, SPDX `SunPro`), and a C# port of **ICU4X's lunisolar approximation**, the mean-solar-term and mean-new-moon reckoning of a `chinese` or `dangi` year outside the published tables, from the `icu_calendar` crate 2.3.0 (`src/cal/east_asian_traditional/simple.rs`, Copyright The ICU4X Project Developers, under the Unicode License v3, SPDX `Unicode-3.0`, whose text the package's notices and the CLDR entry below carry), the port this repository's `JsCalendars` carried until the cut-over (see the ICU4X entry below); one table generated from the Unicode Character Database 17.0.0, the `Space_Separator` members of the white-space set; and it implements **Howard Hinnant**'s civil-day formulas, `days_from_civil` and `civil_from_days`, which their author donated to the public domain, credited there | **not yet confirmed** |
| `Broiler.VM.Profile.JavaScript`, `Broiler.VM.Profile.JavaScript.Format`, `Broiler.VM.Profile.JavaScript.Compiler`, `Broiler.VM.Profile.JavaScript.Intl` | **`Broiler.Unicode.Algorithms` `0.1.0-preview.5`**, the Unicode area's code package, referenced by all four, and **`Broiler.Unicode.Segmentation` `0.1.0-preview.5`**, its optional segmentation package, referenced by the Intl data assembly alone; both packed from `Broiler-Platform/broiler.unicode` commit `b80201436e8f8ff0402ed75c0b9657fe3616d730` and admitted from the committed feed at [`eng/packages/`](eng/packages/README.md) under rule I1, as published to nuget.org on 2026-10-17 (added 2026-10-16, programme milestone SJL-7.4; JSD-0074, JSD-0075; revised the same day when they were published, JSC-310: the cell read "not yet on nuget.org" where it now reads "as published to nuget.org"; revised 2026-10-17, when both moved to `0.1.0-preview.5`: it read `0.1.0-preview.4`, commit `6e14f93ad753094c99f0013d22269b47550ae552` and "published to nuget.org on 2026-10-16"). Apache-2.0, under this repository's own copyright holder. Neither carries a Unicode table or any third-party component: their algorithms read the tables this family embeds, which [below](#unicode-data-and-the-tables-derived-from-it) describes, and each package's own `THIRD_PARTY_NOTICES.md` names the Unicode data and ECMA-262's property tables the owner's repository archives, and the code it ported from this repository | **not yet confirmed** |

**What that entry is and what it is not.** It is a **normative reference document**, archived
because roadmap section 24 asks for the pinned edition to be retrieved, hashed **and archived**,
and because a digest is only checkable by a reader who has the bytes. It is **not code**, nothing
is derived from it, no line of it is copied into any assembly, and it compiles into nothing.

**CORRECTED 2026-09-22, and the correction is the whole of what changed.** The sentence above holds
of `spec.html` and no longer of the whole row. The three property tables archived beside it that day -
`table-nonbinary-unicode-properties.html`, `table-binary-unicode-properties.html` and
`table-binary-unicode-properties-of-strings.html`, the files the edition imports rather than spells out -
are read by the Unicode table generator for **the set of property names ECMAScript admits**, and those
names are encoded into `JsUnicodeProperties.g.cs`, which compiles into
`Broiler.VM.Profile.JavaScript.Format` and ships in that assembly's package and in every composition
image carrying it. So material from this entry **is** derived from and does compile into an assembly,
and the row's confirmation covers the edition alone until the release owner confirms the three tables.
What is derived is a list of names the specification fixes, not its prose.

**The opening sentence of this file is unaffected, and here is the mechanism rather than the
assertion.** The claim is scoped to the three packable assemblies. The archived document is under
`src/Broiler.VM.Profile.JavaScript/docs/`, which is documentation: no project includes it, no
assembly embeds it, and rule **K4** asserts that a published composition's closure contains exactly
the composition, the three core assemblies and the profile assemblies its register row declares —
so a document that reached a shipped image would fail that rule rather than survive as a missing
row here. **Confirmed by the release owner on 2026-09-03**, and, as everywhere else in this file,
**the co-signature is not independent**: the owner and the co-signer are one person.

**Licence, stated because an attribution obligation discovered at a publish is a stop.** The
`tc39/ecma262` repository licenses its natural-language text under the **Alternative copyright
notice of the Ecma text copyright policy**, which grants permission "to copy, modify, prepare
derivative works of, and distribute this work… for any purpose and without fee or royalty" on three
conditions. All three are met in the change that archived the document: the **full notice text** is
retained beside it at
[`ECMA-alternative-copyright-notice.txt`](src/Broiler.VM.Profile.JavaScript/docs/specification/ECMA-alternative-copyright-notice.txt)
rather than linked — a licence condition met by somebody else's uptime is not met; the document's
**own pre-existing notices are intact**, because it is byte-for-byte unmodified, which its SHA-256
attests; and the **notice of changes** required by condition (iii) records that there are none,
which is a different statement from omitting it.

Broiler.VM includes material copied from the ECMAScript Language Specification, ECMA-262,
17th edition (ES2026). Copyright © Ecma International.

**The second row is the one roadmap section 14 has been waiting for since JS-0, and it lands here
because this is the change that first ingests suite material.**
`Broiler.VM.Composition.JavaScript.Conformance` scores this profile against **test262**, © 2012
Ecma International under the **BSD 3-Clause** licence, whose full text is retained at
[`src/tests/conformance/pins/test262-LICENSE.txt`](src/tests/conformance/pins/test262-LICENSE.txt)
as condition 1 of that licence requires. **The suite is unmodified**: it is retained as the archive
it was retrieved as, one file whose SHA-256 the pin beside it names and rule N15 checks, so there
is no modified file to mark.

**The opening sentence of this file is unaffected, and here is the mechanism rather than the
assertion.** The claim is scoped to the three packable assemblies. The suite is a `.tar.gz` under
`src/tests/`, no project includes it, the harness is handed a suite as a directory on a command
line and packs nothing, and rule **N13** asserts that neither the harness nor any suite directory
reaches a package or an advertised composition's closure and that no project file names one — so a
suite file that reached a shipped image would fail that rule rather than survive as a missing row
here. **Confirmed by the release owner on 2026-09-03**, and, as everywhere else in this file, **the
co-signature is not independent**: the owner and the co-signer are one person.

**Why the archive and not the 56,560 files.** Extracted, the suite is 232 MB; this repository packs
to a few megabytes, and the archive is 9.5 MB carrying exactly the same evidence — that the pinned
digest can be checked in a checkout with no network. The choice is recorded rather than left to be
inferred from a tree that does not hold what a reader might expect.

**The third row is Octane, and its confirmation column says `not yet confirmed` because that is
true.** The archive was retrieved, hashed and committed on 2026-09-04 under the workload roadmap's
last stage, and the obligation two paragraphs above — that a component ingesting third-party
material amends this file **in the change that introduces the material** — was not discharged then.
This entry is the amendment; **the confirmation itself is a release-facing statement a person makes
and is not made here.** It is recorded as owed rather than supplied, which is the same discipline
the ledger applies to every other unmet clause.

**What that row is, and how it differs from the two above it.** Octane is third-party **code**: 37
JavaScript files, retained as the archive `git archive` produced at the pinned commit, run by the
end-user host as a guest program. Nothing is derived from it, no line of it is copied into any
assembly, and the suite is retired upstream — the pinned commit is its retirement commit — so a
score from it is a number about this configuration and a comparison with nothing.

**The opening sentence of this file is unaffected here too, and the mechanism is the same one.** The
archive is a `.tar.gz` under `src/tests/`, no project includes it, the host is handed the extracted
files as paths on a command line and packs nothing, and rule **N13** now names `tests/octane`
alongside `tests/conformance` as a directory no project file may mention — so a benchmark file that
reached a shipped image would fail that rule rather than survive as a missing row here.

**The last row is the WebAssembly specification's core test scripts, and it lands in the change that
first ingests them**, as the WebAssembly profile's roadmap section 4.4 asks. The harness root
`Broiler.VM.Composition.WebAssembly.Harness` reads them for the universal bytecode programme's UBC-4
decision rule. They are © the WebAssembly Community Group and its contributors under the **Apache
License 2.0**, whose text the upstream repository's `test/LICENSE` carries and which is retained at
[`src/tests/wasm/spec/wasm-spec-test-LICENSE.txt`](src/tests/wasm/spec/wasm-spec-test-LICENSE.txt), as
section 4(a) of that licence requires. **The suite is unmodified**: it is retained as an archive of
`test/core` at the pinned commit, whose digest and whose extracted files' digest the pin beside it names.
The harness checks the extracted files against that digest before reading any, so no file is marked as
changed because none is. The upstream repository carries no NOTICE file, so section 4(d) carries nothing
forward.

**The opening sentence of this file is unaffected, and the mechanism is the one the rows above use.** The
archive is a `.tar.gz` under `src/tests/`. No project includes it. The harness root is handed the
extracted directory on a command line and packs nothing. Rule **N13** names `tests/wasm/spec` beside
`tests/conformance` and `tests/octane` as a directory no project file may mention. **The release owner's
confirmation is not made here**, for the reason the Octane row gives: it is a release-facing statement a
person makes. It is recorded as owed.

**The shared conformance vectors' row is the one whose material is not third-party at all**, and it
is listed because it is ingested the same way and kept out by the same mechanism. The corpus is the
Broiler.JS repository's, under Apache-2.0 and this repository's copyright holder; it quotes no
specification text and copies no Unicode data file, naming ECMA-262 clauses by their identifiers and
stating which code points a set holds. The archive is a `.tar.gz` under `src/tests/`, no project
includes it, the conformance root reads it from a path on its command line and packs nothing, and
rule **N13** names `tests/vectors` beside the other three directories, with a witness that names it
from the profile's own project file. **The release owner's confirmation is not made here**, for the
reason the Octane row gives. It is recorded as owed.

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

## Unicode data, and the tables derived from it

**Added 2026-09-22 (JSeal F07, slices U1 and U2; decision
[JSD-0031](src/Broiler.VM.Profile.JavaScript/docs/decisions/0031-unicode-data-source-and-build-boundary.md)).
This is the first entry in this file whose material is compiled into a shipped assembly.**

This repository contains data files published by the Unicode Consortium and tables generated from
them. **Those data files and the tables derived from them remain subject to the
[Unicode Terms of Use](https://www.unicode.org/terms_of_use.html), which apply the Unicode License
v3 (SPDX `Unicode-3.0`) to data files. The Apache License 2.0 in [`LICENSE`](LICENSE) applies to
Broiler's source code, the generator and the lookup code included, and does not replace the Unicode
terms for Unicode-provided data.** The pattern is the one `Broiler.Unicode`'s own
`THIRD_PARTY_NOTICES.md` uses; the repository owner chose it in conversation on 2026-09-22.

**What was ingested.** From `https://www.unicode.org/Public/17.0.0/ucd/`, Unicode 17.0.0:
`UnicodeData.txt`, `DerivedNormalizationProps.txt`, `NormalizationTest.txt` (test input only),
`PropertyAliases.txt`, `PropertyValueAliases.txt`, `extracted/DerivedGeneralCategory.txt`,
`extracted/DerivedBinaryProperties.txt`, `Scripts.txt`, `ScriptExtensions.txt`, `PropList.txt`,
`DerivedCoreProperties.txt`, `emoji/emoji-data.txt` and `CaseFolding.txt`, and since 2026-10-03
`SpecialCasing.txt`; and the licence text from `https://www.unicode.org/license.txt`. They are archived unmodified at
[`src/tests/unicode/pins/`](src/tests/unicode/pins/README.md), where `unicode.pin` records each
file's length and SHA-256 and rule **N22** hashes them on every run of the architecture suite.
Retrieved twice on 2026-09-22 by Claude with the owner's permission given in conversation; the two
retrievals were byte-identical. `SpecialCasing.txt` was retrieved the same way on 2026-10-03, while
continuing the roadmap at the owner's request, with no permission specific to it (`unicode.pin` says
so). `auxiliary/GraphemeBreakProperty.txt`, `auxiliary/WordBreakProperty.txt` and
`auxiliary/SentenceBreakProperty.txt`, with the three matching `*BreakTest.txt` files (test input
only), were retrieved the same way on 2026-10-04 for `Intl.Segmenter` (decision
[JSD-0050](src/Broiler.VM.Profile.JavaScript/docs/decisions/0050-intl-segmenter.md)). Their table is
generated into the Intl data assembly, by the owner's generator since 2026-10-16 (below) *(revised
2026-10-16, programme milestone SJL-7.4; it read "Their tables are generated not here but by
`CldrTableGenerator` into the Intl data assembly, described in the CLDR entry below")*.

**What is derived, and where it ships.** `UnicodeTableGenerator` (architecture test project, not
shipped) writes four source files of tables from those files:
`src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeProperties.g.cs` and
`src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeCaseFolding.g.cs`, compiled into
**`Broiler.VM.Profile.JavaScript.Format.dll`**, and
`src/Broiler.VM.Profile.JavaScript/JsUnicodeNormalization.g.cs` and, since 2026-10-03,
`src/Broiler.VM.Profile.JavaScript/JsUnicodeCasing.g.cs`, compiled into
**`Broiler.VM.Profile.JavaScript.dll`**. The derived tables therefore ship in:

- **the packages** `Broiler.VM.Profile.JavaScript.Format` and `Broiler.VM.Profile.JavaScript`
  (both packable since `d16a6aa`; `Broiler.VM.Profile.JavaScript.Compiler` carries no table and
  depends on the Format package);
- **every composition image that carries the realm**: the published outputs of
  `Broiler.VM.Composition.JavaScript.Cli`, `.Conformance`, `.ExecutionOnly`, `.SliceCompiler` and
  `Broiler.VM.Composition.PolyglotCli`, and the **Android** application
  `Broiler.VM.Composition.JavaScript.Android`, which is not packable but carries both assemblies in
  its image with trimming off.

**Since 2026-10-16 the owner's generator writes Format's tables** (programme milestone SJL-7.4; decisions
JSD-0074 and JSD-0075). `Broiler.Unicode.Generation`, admitted test-only for the architecture tests, is
run over the same archived files, and `UnicodeTableEmbedding` writes what it generates into
`src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeTables.g.cs`, which replaces
`JsUnicodeProperties.g.cs`, `JsUnicodeCaseFolding.g.cs` and `JsUnicodeStringProperties.g.cs` byte for byte
in their tables, and into `src/Broiler.VM.Profile.JavaScript.Intl/JsUnicodeTables.g.cs`, compiled into
**`Broiler.VM.Profile.JavaScript.Intl.dll`** and so into the package of that name and the images of
`.Conformance` and `.SliceCompiler`: the break table `Intl.Segmenter` reads, which `CldrTableGenerator`
wrote into `JsCldrTables.g.cs` until then. **The same day it began writing the profile's tables**, into
`src/Broiler.VM.Profile.JavaScript/JsUnicodeTables.g.cs`, compiled into
**`Broiler.VM.Profile.JavaScript.dll`**, which replaces `JsUnicodeNormalization.g.cs` and
`JsUnicodeCasing.g.cs` byte for byte in their tables and holds one table more: the Soft_Dotted ranges
the language-sensitive case mappings read, which `CldrTableGenerator` wrote into `JsCldrTables.g.cs` until
then. Those ranges therefore ship in the profile's package and in every composition image that carries
the realm, where until then only the images that admit `Intl` carried them. The three files are Unicode
data under the same terms; rule N22 holds Format's and the profile's to that generation byte for byte,
and rule N28 the Intl assembly's. `UnicodeTableGenerator` writes no table since then: it is the gate
that compares them, and it writes the probes below.

**Since 2026-10-17 the owner's algorithms read constant-time tables** (`Broiler.Unicode.Algorithms`
`0.1.0-preview.5`). Format's file holds one table more, the lexical classes' trie. The profile's holds
the two layouts, the case mapping pool and the eight tables of the constant-time normalization and case
mapping, and no longer the range and index tables, the Soft_Dotted ranges among them, which no type of
the owner reads any more; Soft_Dotted ships in the casing records instead. They are Unicode data under
the same terms, generated from the same pin.

The same generator also writes the conformance probes `src/tests/differential/the-unicode-normalization-*.js`,
which carry the vectors of `NormalizationTest.txt` (Unicode data) as JavaScript string literals.
They are test files: no package or composition image contains them. Each names the Unicode License
v3 in its header and points at the text below.

The three ECMAScript property tables the generator also reads are Ecma material, archived beside
the edition under the Ecma notice in the first row above; nothing Unicode-licensed is in them.

**How the notice travels.** The licence permits copying and redistribution "provided that either
(a) this copyright and permission notice appear with all copies of the Data Files or Software, or
(b) this copyright and permission notice appear in associated Documentation" - which JSD-0031
section 8 asked to be checked against the retrieved text, and which the text below states. The
mechanism chosen is (a) for the packages: **this file is packed into every package this repository
produces** (`eng/Broiler.Packaging.props` packs `THIRD_PARTY_NOTICES.md` at the package root), and
**the full licence text is reproduced verbatim below**, so each package that carries derived tables
carries the notice with them. Rule N22 asserts that the text below is the archived licence text,
character for character. Each generated file also names the licence in a comment below its
generated header. **For the composition images the notice is not yet carried**: a published host
directory does not contain this file today, and whether it should carry a notices file beside the
host or a constant the host can print is a release-owner decision JSD-0031 leaves open. That is
recorded as owed rather than treated as met.

**Owner decisions, as given in conversation on 2026-09-22, and what is not given.** The licence
entry follows the pattern above, names the files, the version and the assemblies, packages and
composition images that carry derived tables, and ships the licence text; the size budget is about
200 KB with a hard cap of 300 KB of table data, checked by rule N22 (measured on 2026-09-22 in
[`docs/evidence/jseal-f07-u2/`](docs/evidence/jseal-f07-u2/README.md)). **Nobody has signed
either decision, and the release owner's co-signature this file requires for ingested material is
not given**: the row above says `not yet confirmed` because that is true.

The Unicode licence text, as archived at `src/tests/unicode/pins/unicode-LICENSE.txt`:

```text
UNICODE LICENSE V3

COPYRIGHT AND PERMISSION NOTICE

Copyright © 1991-2026 Unicode, Inc.

NOTICE TO USER: Carefully read the following legal agreement. BY
DOWNLOADING, INSTALLING, COPYING OR OTHERWISE USING DATA FILES, AND/OR
SOFTWARE, YOU UNEQUIVOCALLY ACCEPT, AND AGREE TO BE BOUND BY, ALL OF THE
TERMS AND CONDITIONS OF THIS AGREEMENT. IF YOU DO NOT AGREE, DO NOT
DOWNLOAD, INSTALL, COPY, DISTRIBUTE OR USE THE DATA FILES OR SOFTWARE.

Permission is hereby granted, free of charge, to any person obtaining a
copy of data files and any associated documentation (the "Data Files") or
software and any associated documentation (the "Software") to deal in the
Data Files or Software without restriction, including without limitation
the rights to use, copy, modify, merge, publish, distribute, and/or sell
copies of the Data Files or Software, and to permit persons to whom the
Data Files or Software are furnished to do so, provided that either (a)
this copyright and permission notice appear with all copies of the Data
Files or Software, or (b) this copyright and permission notice appear in
associated Documentation.

THE DATA FILES AND SOFTWARE ARE PROVIDED "AS IS", WITHOUT WARRANTY OF ANY
KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT OF
THIRD PARTY RIGHTS.

IN NO EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS NOTICE
BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL DAMAGES,
OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS,
WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION,
ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THE DATA
FILES OR SOFTWARE.

Except as contained in this notice, the name of a copyright holder shall
not be used in advertising or otherwise to promote the sale, use or other
dealings in these Data Files or Software without prior written
authorization of the copyright holder.
```

## CLDR data, and the tables derived from it

**Added 2026-10-04 (phase F7, slices I0 and I1; decision
[JSD-0043](src/Broiler.VM.Profile.JavaScript/docs/decisions/0043-intl-data-boundary-and-collation.md)).
The second entry in this file whose material is compiled into a shipped assembly, and the first
whose assembly holds nothing else.**

This repository contains data files published by the Unicode Consortium's Common Locale Data
Repository (CLDR) and tables generated from them. **Those data files and the tables derived from
them remain subject to the [Unicode Terms of Use](https://www.unicode.org/terms_of_use.html), which
apply the Unicode License v3 (SPDX `Unicode-3.0`) to data files. The Apache License 2.0 in
[`LICENSE`](LICENSE) applies to Broiler's source code, the generator and the code that reads the
tables included, and does not replace the Unicode terms for Unicode-provided data.** The pattern is
the one the entry above uses for the UCD.

**What was ingested.** CLDR 48.2.0: from the npm packages `cldr-core` and `cldr-bcp47`, the
supplemental files `likelySubtags.json`, `aliases.json` and `parentLocales.json`, the files
`availableLocales.json` and `defaultContent.json`, every `bcp47/*.json` file and both
`package.json` files; and from the CLDR release's `common/` tree, `uca/allkeys_CLDR.txt`, the two
short CollationTest files (test input only) and `collation/root.xml`, `de.xml` and `en.xml`, and (added 2026-10-05, phase F8's slice T2, JSD-0055)
`main/root.xml`, whose ISO 8601 calendar patterns the date tables carry; and (added 2026-10-05,
phase F8's slice T4, JSD-0057) from the npm packages `cldr-cal-buddhist-full`, `-chinese-`,
`-coptic-`, `-dangi-`, `-ethiopic-`, `-hebrew-`, `-indian-`, `-islamic-`, `-japanese-`, `-persian-`
and `-roc-full`, each one's `package.json` and its `ca-*.json` files for `de` and `en`, and from
`cldr-dates-full` `ca-generic.json` for `de` and `en`; and (added 2026-10-05, phase F8's slice T5,
JSD-0058) from `cldr-core` `supplemental/metaZones.json` and `supplemental/primaryZones.json`; and
the licence text. They are archived unmodified at [`src/tests/cldr/pins/`](src/tests/cldr/pins/README.md),
where `cldr.pin` records each file's length and SHA-256 and rule **N27** hashes them on every run of
the architecture suite. The pin and its README record how and when they were retrieved. Since the
same day, for `Intl.NumberFormat` (decision
[JSD-0044](src/Broiler.VM.Profile.JavaScript/docs/decisions/0044-intl-numberformat.md)), the archive
also holds from `cldr-numbers-full` `numbers.json` for `de`, `en` and `und` and `currencies.json` for
`de` and `en`, from `cldr-units-full` `units.json` for `de` and `en`, and from `cldr-core`
`currencyData.json`, `numberingSystems.json`, `plurals.json` and `pluralRanges.json`. Since the same
day, for `Intl.DateTimeFormat` (decision
[JSD-0045](src/Broiler.VM.Profile.JavaScript/docs/decisions/0045-intl-datetimeformat.md)), it also
holds from `cldr-dates-full` `ca-gregorian.json`, `dateFields.json` and `timeZoneNames.json` for `de`
and `en`, and from `cldr-core` `timeData.json` and `dayPeriods.json`. Since the same day, for
`Intl.Locale` (decision
[JSD-0046](src/Broiler.VM.Profile.JavaScript/docs/decisions/0046-intl-locale.md)), it also holds
from `cldr-core` `scriptMetadata.json` and `weekData.json`, and for `Intl.PluralRules` (decision
[JSD-0047](src/Broiler.VM.Profile.JavaScript/docs/decisions/0047-intl-pluralrules.md))
`ordinals.json`. Since the same day, for `Intl.ListFormat` (decision
[JSD-0048](src/Broiler.VM.Profile.JavaScript/docs/decisions/0048-intl-listformat.md)), it also
holds from `cldr-misc-full` `listPatterns.json` for `de` and `en`. Since 2026-10-05, for
`Intl.DisplayNames` (decision
[JSD-0052](src/Broiler.VM.Profile.JavaScript/docs/decisions/0052-intl-displaynames.md)), it also
holds from `cldr-localenames-full` `languages.json`, `territories.json`, `scripts.json`,
`variants.json` and `localeDisplayNames.json` for `de` and `en`. Since the same day, for
`Intl.DateTimeFormat` in the CLDR calendars (decision
[JSD-0057](src/Broiler.VM.Profile.JavaScript/docs/decisions/0057-intl-datetimeformat-in-the-cldr-calendars.md)),
it also holds the eleven `cldr-cal-*-full` packages' calendar files and `ca-generic.json` for `de`
and `en`. Since the same day, for the time zones' names (decision
[JSD-0058](src/Broiler.VM.Profile.JavaScript/docs/decisions/0058-time-zone-names.md)), it also holds
from `cldr-core` `metaZones.json` and `primaryZones.json`.

**What is derived, and where it ships.** `CldrTableGenerator` (architecture test project, not
shipped) writes one source file of tables from those files and the pinned UCD:
`src/Broiler.VM.Profile.JavaScript.Intl/JsCldrTables.g.cs`, compiled into
**`Broiler.VM.Profile.JavaScript.Intl.dll`**, which rule **N28** holds to the generator byte for
byte. It carries likely subtags, the language, script, region, variant and subdivision aliases, the
BCP 47 keys and types, the supported locales, the root collation and three tailorings, and since slice
I2 each language's number patterns and symbols, currency names,
unit patterns and plural rules, the currency fraction digits and the numbering systems' digits, and
since slice I3 each language's Gregorian calendar names and patterns, date field names and UTC and GMT
zone names, the hour cycles of the supported regions and the day period rules, and since slice I4
the hour cycles and weeks of every region, the scripts' line directions, and each language's ordinal
plural rules, list patterns, relative time patterns and duration patterns, and each language's names
of languages, regions, scripts, variants, calendars and date fields, and since slice T4 each language's names and patterns in the
calendars of the Intl era and month code proposal, as layers over its Gregorian data, and since
slice T5 each language's metazone and zone names and exemplar cities, and the metazone periods,
golden zones and primary zones those names are chosen by. *(Revised 2026-10-16, programme milestone
SJL-7.4: the list named "the UCD's Soft_Dotted ranges" after the tailorings, and "from the UCD the break
properties `Intl.Segmenter` reads" after the names of date fields. Since then the Unicode area's owner's
generator writes the break table into the same assembly's `JsUnicodeTables.g.cs`, and the Soft_Dotted
ranges into the profile's; the UCD entry above describes both.)* The derived tables therefore ship in:

- **the package** `Broiler.VM.Profile.JavaScript.Intl` (packable from 2026-10-04, rule N4);
- **the composition images that admit `Intl`**: the published outputs of
  `Broiler.VM.Composition.JavaScript.SliceCompiler` and `Broiler.VM.Composition.JavaScript.Conformance`.
  No other composition references the assembly, so no other image carries the data.

**How the notice travels.** As for the UCD entry above: this file is packed into every package this
repository produces, so the Intl package carries it, and the full CLDR licence text is reproduced
verbatim below. Rule N28 asserts that it is the archived text, character for character. The
generated file names the licence in a comment below its generated header. **For the composition
images the notice is not yet carried**, for the reason the entry above records; that is owed rather
than met.

**Owner decisions, and what is not given.** JSD-0027's owner decision (c) asks for a size budget from
a measured prototype. The tables measured 325,646 bytes of data on 2026-10-04, in an assembly of
333,312 bytes, and 438,831 bytes in an assembly of 448,000 once slice I2's data joined them the same
day. Rule N28 held them under a provisional bound of 512 KiB that JSD-0043 records, until the
repository owner set the budget at 768 KiB on 2026-10-05, when `Intl.DisplayNames`'s names needed
more. They measured 572,024 bytes that day, in an assembly of 583,680 bytes. **The release owner's
co-signature this file requires for ingested material is not given.**

The CLDR licence text, as archived at `src/tests/cldr/pins/cldr-LICENSE.txt`:

```text
UNICODE LICENSE V3

COPYRIGHT AND PERMISSION NOTICE

Copyright © 2004-2026 Unicode, Inc.

NOTICE TO USER: Carefully read the following legal agreement. BY
DOWNLOADING, INSTALLING, COPYING OR OTHERWISE USING DATA FILES, AND/OR
SOFTWARE, YOU UNEQUIVOCALLY ACCEPT, AND AGREE TO BE BOUND BY, ALL OF THE
TERMS AND CONDITIONS OF THIS AGREEMENT. IF YOU DO NOT AGREE, DO NOT
DOWNLOAD, INSTALL, COPY, DISTRIBUTE OR USE THE DATA FILES OR SOFTWARE.

Permission is hereby granted, free of charge, to any person obtaining a
copy of data files and any associated documentation (the "Data Files") or
software and any associated documentation (the "Software") to deal in the
Data Files or Software without restriction, including without limitation
the rights to use, copy, modify, merge, publish, distribute, and/or sell
copies of the Data Files or Software, and to permit persons to whom the
Data Files or Software are furnished to do so, provided that either (a)
this copyright and permission notice appear with all copies of the Data
Files or Software, or (b) this copyright and permission notice appear in
associated Documentation.

THE DATA FILES AND SOFTWARE ARE PROVIDED "AS IS", WITHOUT WARRANTY OF ANY
KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT OF
THIRD PARTY RIGHTS.

IN NO EVENT SHALL THE COPYRIGHT HOLDER OR HOLDERS INCLUDED IN THIS NOTICE
BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL INDIRECT OR CONSEQUENTIAL DAMAGES,
OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS,
WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION,
ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THE DATA
FILES OR SOFTWARE.

Except as contained in this notice, the name of a copyright holder shall
not be used in advertising or otherwise to promote the sale, use or other
dealings in these Data Files or Software without prior written
authorization of the copyright holder.

SPDX-License-Identifier: Unicode-3.0
```

## IANA Time Zone Database data, and the tables derived from it

**Added 2026-10-05 (phase F8; decision
[JSD-0053](src/Broiler.VM.Profile.JavaScript/docs/decisions/0053-time-zone-data-and-temporal-admission.md)).
The third entry in this file whose material is compiled into a shipped assembly; it shares the CLDR
entry's assembly.**

This repository contains a release of the IANA Time Zone Database (tzdb) and tables generated from
it. **The release's `LICENSE` states: "Unless specified below, all files in the tz code and data
(including this LICENSE file) are in the public domain." The files it then names - `date.c`,
`newstrftime.3` and `strftime.c` - are code, are not in the data tarball, and are not here.** No
licence text therefore has to travel with the derived tables; this entry records where they come
from all the same, so a reader finds every ingested input in one file.

**What was ingested.** tzdb 2026e's data tarball, `tzdata2026e.tar.gz`, archived unmodified at
[`src/tests/tzdb/pins/`](src/tests/tzdb/pins/README.md), where `tzdb.pin` records the tarball's
length, SHA-256 and SHA-512 and the length and SHA-256 of the twelve members a generator reads - the
source files `africa`, `antarctica`, `asia`, `australasia`, `europe`, `northamerica`,
`southamerica`, `etcetera` and `backward`, and `zone.tab`, `version` and `LICENSE` - and rule **N29**
checks them on every run of the architecture suite. The pin and its README record how and when the
tarball was retrieved.

**What is derived, and where it ships.** `TzdbTableGenerator`, the time data home's generator since
programme milestone SJL-8.5, run by the architecture test project from the test-only package
`Broiler.ECMAScript.Generation` (not shipped), compiles the source files' zones into UTC offsets and writes
`src/Broiler.VM.Profile.JavaScript.Intl/JsTzdbTables.g.cs`, compiled into
**`Broiler.VM.Profile.JavaScript.Intl.dll`**, which rule **N30** holds to the generator byte for byte.
It carries each zone's offsets and transitions, every Zone and Link name with its primary identifier,
which is taken from CLDR's `bcp47/timezone.json` under the CLDR entry above, and `zone.tab`'s zones of
each country. The tables therefore ship where the CLDR tables ship: in the package
`Broiler.VM.Profile.JavaScript.Intl` and the composition images that admit `Intl`. *(Revised 2026-10-17, programme
milestone SJL-8.5 (JSD-0076), when the compiler and the generator moved from this repository's
architecture tests to the Broiler.JS repository, the time data home, under decision SJL-D-6 (c); the
pin, the tarball and the generated file stay here, unchanged. The paragraph began "`TzdbTableGenerator`
(architecture test project, not shipped) compiles".)*

**Owner decisions, and what is not given.** The repository owner chose on 2026-10-05 to start phase
F8 by archiving and pinning tzdb, which answers JSD-0027's owner decision (d). The tables count
against the Intl data budget of 768 KiB that the owner set the same day: with them the data measured
670,742 bytes. **The release owner's co-signature this file requires for ingested material is not
given.**

## ICU4X's calendar crates, and the table derived from them

**Added 2026-10-05 (phase F8, slice T3; decision
[JSD-0056](src/Broiler.VM.Profile.JavaScript/docs/decisions/0056-temporal-in-the-cldr-calendars.md)).
The fourth entry in this file whose material is compiled into a shipped assembly; it shares the CLDR
entry's assembly.**

This repository contains two crates of the ICU4X project and a table generated from them; until
programme milestone SJL-8.5 it also contained code ported from one of them, which the shared algorithms
package carries now:

- **`icu_calendar` 2.3.0**, under the Unicode License v3 (SPDX `Unicode-3.0`), whose text the CLDR
  entry above carries; its own copy is the crate's `LICENSE`, `Copyright © 2020-2024 Unicode, Inc.`
- **`calendrical_calculations` 0.2.4**, under the Apache License 2.0, the licence of this
  repository's own code. Its files state that they implement algorithms from *Calendrical
  Calculations* by Reingold and Dershowitz (4th edition, 2018), which their authors released as Lisp
  code under the Apache License 2.0. The crate carries no `NOTICE` file.

**What was ingested.** Both crates as crates.io serves them, archived unmodified at
[`src/tests/calendars/pins/`](src/tests/calendars/pins/README.md), where `calendars.pin` records each
crate's length, SHA-256 and SHA-512 and the length and SHA-256 of the nine members a generator reads -
`china_data.rs`, `korea_data.rs`, `qing_data.rs` and `ummalqura_data.rs` of `icu_calendar`, `persian.rs`
of `calendrical_calculations`, and each crate's `Cargo.toml` and `LICENSE` - and rule **N31** checks
them on every run of the architecture suite.

**What is derived, and where it ships.** `CalendarTableGenerator`, the time data home's generator
since programme milestone SJL-8.5, run by the architecture test project from the test-only package
`Broiler.ECMAScript.Generation` (not shipped), reads the Chinese, Korean, Qing-era and Umm al-Qura years those files state and the Persian
calendar's 78 corrections, and writes `src/Broiler.VM.Profile.JavaScript.Intl/JsCalendarTables.g.cs`,
compiled into **`Broiler.VM.Profile.JavaScript.Intl.dll`**, which rule **N32** holds to the generator
byte for byte. **The years remain subject to the Unicode License v3**, and the corrections to the
Apache License 2.0; the generated file says so. **One function of `icu_calendar` is ported, in the
shared algorithms package**: the approximation of a Chinese or Korean year by mean new moons and mean
solar terms (`src/cal/east_asian_traditional/simple.rs`), which `CalendarYears` in
**`Broiler.ECMAScript.Algorithms`** answers beyond the table and which that package's own
`THIRD_PARTY_NOTICES.md` and its row among the ingesting components above name; this repository
no longer compiles a copy. The table is read by that package's `CalendarTables`, and the calendars'
arithmetic that stays in `src/Broiler.VM.Profile.JavaScript/JsCalendars.cs` is written from
*Calendrical Calculations*' published formulas and the calendars' own rules. *(Revised 2026-10-17,
programme milestone SJL-8.5 (JSD-0076), when the generator moved from this repository's architecture
tests to the Broiler.JS repository, the time data home, under decision SJL-D-6 (c); the pin, the
crates and the generated file stay here, unchanged. The paragraph began "`CalendarTableGenerator`
(architecture test project, not shipped) reads". Revised again the same day, at the same milestone's
cut-over (JSD-0077), when the port moved to the package and this repository's copy was deleted. The
sentence on the port read "**One function of `icu_calendar` is ported**: the approximation of a
Chinese or Korean year by mean new moons and mean solar terms
(`src/cal/east_asian_traditional/simple.rs`), as `JsEastAsianCalendar.Approximate` in
`src/Broiler.VM.Profile.JavaScript/JsCalendars.cs`, compiled into
**`Broiler.VM.Profile.JavaScript.dll`**; that port is a derivative of Unicode-licensed software, and
this entry is the notice the licence's condition (b) asks associated documentation to carry. The other
calendars' arithmetic in that file is written from", and the entry's opening sentence read "a table
generated from them, and code ported from one of them:".)*

**Used to test, not shipped.** `src/tests/temporal/calendars/icu4x-reference.rs` is a program this
repository wrote; built against the archived crates and the `icu_calendar_data` 2.3.0 crate, which is
not archived, it wrote `temporal-calendars.icu4x-2.3.0.txt`, ICU4X's answers, which the slice
compiler's check compares. `temporal-calendars.polyfill-e8cc03fc.txt` beside it is the reference
polyfill's answer, under the next entry.

**Owner decisions, and what is not given.** The repository owner asked on 2026-10-05 for phase F8's
slices to be continued; JSD-0056 names the source. The table counts against the Intl data budget of
768 KiB: with it the data measures 679,058 bytes. **The release owner's co-signature this file
requires for ingested material is not given.**

## The Temporal proposal's reference polyfill, whose answers are retained

**Added 2026-10-05 (phase F8, slice T1; decision
[JSD-0054](src/Broiler.VM.Profile.JavaScript/docs/decisions/0054-temporal-in-the-iso-and-gregorian-calendars.md)).
Nothing of it ships, and none of its code is in this repository.**

`src/tests/temporal/temporal.polyfill-e8cc03fc.txt` is the output of a program this repository wrote,
`src/tests/temporal/temporal.js`, run under the reference polyfill in `polyfill/lib` of
[tc39/proposal-temporal](https://github.com/tc39/proposal-temporal) at
`e8cc03fc970a65a3359e8870e3b35e687ac94e55`, which is licensed under the ISC licence. The polyfill and
its three runtime dependencies were installed outside the repository for the run and are not
archived. The file holds the answers the polyfill gave, one per line, as the slice compiler's check
compares them; it reproduces none of the polyfill's code or text, so no licence text travels with it.
This entry records the source so a reader finds every reference the tree's datasets were checked
against in one file.

The same holds for the answers of the slices that followed, each from a program of this repository's
run under the same polyfill: `src/tests/temporal/intl/temporal-intl.polyfill-e8cc03fc.txt` (slice T2,
JSD-0055), `src/tests/temporal/calendars/temporal-calendars.polyfill-e8cc03fc.txt` (slice T3,
JSD-0056), `src/tests/temporal/calendars-intl/calendar-formats.polyfill-e8cc03fc.txt` (slice T4,
JSD-0057) and `src/tests/temporal/zone-names/zone-names.polyfill-e8cc03fc.txt` (slice T5, JSD-0058).
