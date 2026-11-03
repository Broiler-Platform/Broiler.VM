# The committed feed

A NuGet folder source for the shared JavaScript libraries' packages while an area is in development
(decision SJL-D-4 (c), taken 2026-10-11; milestone SJL-2.7). [`NuGet.config`](../../NuGet.config)'s
package source mapping sends the `Broiler.ECMAScript.*` and `Broiler.Unicode.*` prefixes to this
directory and to nuget.org, and every other package to nuget.org only. *(Revised 2026-10-16, at SJL-7.4,
when the Unicode area's owner packages were admitted. It read "sends the `Broiler.ECMAScript.*` prefix to
this directory and to nuget.org".)*

## How a shared package is admitted, since 2026-10-30

**This directory holds no package since 2026-10-30.** The owner's change of 2026-10-30 and 2026-10-31
took every `.nupkg` and `.pin` out of it and removed the lock files; every admitted package now
restores from nuget.org, and its pins are in the allow-list. Rule `I1` and the rules that read a
package (`I2`, `I4`, `I6`) were moved onto that model on 2026-11-01.

- **The allow-list is the pin.** Each entry of
  [`admitted-packages.json`](../../src/tests/Broiler.VM.Architecture.Tests/admitted-packages.json)
  names the package, its exact version (the version `Directory.Packages.props` declares), its use
  (product or test-only), the projects that may reference it, and two hashes: `contentHash`, NuGet's
  content hash, which restore records in a project's `obj/project.assets.json` and in the
  `.nupkg.metadata` beside the restored package, and which leaves a signed package's signature out; and
  `archiveHash`, the base64 SHA-512 of the complete archive as nuget.org serves it, signature included.
  The two differ for a signed package, and both are held.
- **Rule `I1` holds what restored to the list.** Every project that references an admitted package is
  held, through its `obj/project.assets.json`, to the entry's version and content hash. Every entry is
  held to the package as restored into the NuGet global packages folder (the folder the architecture
  tests' own `obj/project.assets.json` names under `packageFolders`): its `.nupkg` must hash to
  `archiveHash` and its `.nupkg.metadata` must record `contentHash`, whichever source supplied it. An
  entry with no `archiveHash` is refused.
- **Rules `I2`, `I4` and `I6` read that restored archive**, the one `I1` holds to its archive pin: the
  assemblies' metadata for `I2`, the Unicode and time data versions they declare for `I4`, and their
  public types for `I6`.
- **This directory stays a mapped source.** [`NuGet.config`](../../NuGet.config) still sends the two
  prefixes here and to nuget.org, so a package not yet published can be placed here for development with
  a `.pin` beside it; where a pin file or a copy here is present, `I1` holds it to the entry too. The
  entries' `pin` field still names the pin file each package last had here, and their `sources` still
  list `committed-feed`, because `eng/admitted_packages.py` and `eng/verify-sources.py` read those
  fields; `eng/admitted_packages.py` reads a package's source commit, for rule `I7`, from a copy here,
  and records none while this directory holds no package.
- **Nothing else is a source.** No other repository's local folder, `D:\local-packages` included, is
  ever added to a configuration in this repository.
- **A package a Broiler.VM release depends on comes from nuget.org**, and `eng/verify-feed.ps1`
  refuses a release whose admitted dependency is not there yet.

*(Revised 2026-11-01, completing the owner's change of 2026-10-30 and 2026-10-31. The section's
heading was not there, and its first bullet read: "**Every package here has a `.pin` beside it**: its
identifier, version, NuGet content hash, SHA-256, and the upstream commit it was packed from. A lock file
pins the same content hash whichever source supplied the package, and rule `I1` holds the lock file, the
pin and the allow-list (`src/tests/Broiler.VM.Architecture.Tests/admitted-packages.json`) to one
another." The last bullet ended "This directory is for development and for test-only admissions.", and
the table's last column was headed "Why it is here".)*

| Package | Version | Admitted as | Why it is admitted |
|---|---|---|---|
| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.9` | product | Published package downloaded from nuget.org on 2026-11-03; source commit `6f7284daa2d2fbec3104a9498d5f7c989f956946` (Broiler.JS's publish-shared workflow, run 41679821770). Its code is `0.1.0-preview.8`'s. Existing admission scope is retained. |
| `Broiler.ECMAScript.Generation` | `0.1.0-preview.9` | test-only | Published package downloaded from nuget.org on 2026-11-03; source commit `6f7284daa2d2fbec3104a9498d5f7c989f956946` (Broiler.JS's publish-shared workflow, run 41679821770). Its Intl collation generator gains Broiler.JS's tailorings; its default tailorings stay this repository's. Existing admission scope is retained. |
| `Broiler.ECMAScript.Intl` | `0.1.0-preview.9` | product | Published package downloaded from nuget.org on 2026-11-03; source commit `6f7284daa2d2fbec3104a9498d5f7c989f956946` (Broiler.JS's publish-shared workflow, run 41679821770). The shared Intl library (JSD-0082, decision D9), referenced by the JavaScript profile alone since SJL-9.4; it carries no locale data, which this family embeds (decision SJL-D-6 (c)). |
| `Broiler.Unicode.Algorithms` | `0.1.0-preview.6` | product | The Unicode area of SJL-7's code package (JSD-0074): property lookups, lexical classes, case folding, case mapping, normalization and properties of strings, over tables this repository embeds; referenced by Format, the runtime, the compiler and the Intl data since SJL-7.4, and `Broiler.Unicode.Generation`'s dependency. Downloaded from nuget.org on 2026-10-31; its archive hash is in the allow-list, and its nuspec names Broiler.Unicode `ccef653c8aa903f7eecf25faafb8bc5d458e6cc4`. |
| `Broiler.Unicode.Segmentation` | `0.1.0-preview.6` | product | The Unicode area's optional segmentation (JSD-0074), referenced by the Intl data assembly alone since SJL-7.4. Downloaded from nuget.org on 2026-10-31; its archive hash is in the allow-list, and its nuspec names Broiler.Unicode `ccef653c8aa903f7eecf25faafb8bc5d458e6cc4`. |
| `Broiler.Unicode.Generation` | `0.1.0-preview.6` | test-only | The owner's table generator and pin (JSD-0074, decision SJL-D-6 (c)), which the architecture tests run for rules N22, N28 and I4 since SJL-7.4; no shipping project may reference it. Downloaded from nuget.org on 2026-10-31; its archive hash is in the allow-list, and its nuspec names Broiler.Unicode `ccef653c8aa903f7eecf25faafb8bc5d458e6cc4`. |

*(Revised 2026-10-12, at SJL-3.4. The table's row read: "| `Broiler.ECMAScript.Algorithms` | `0.0.1-probe.1` | test-only | The admission probe of SJL-2.9: the contract types only, never published to nuget.org |". The probe pre-release, its `.nupkg` and its pin left the feed in the same change: no project references it, and the allow-list admits one version of a package.)*

*(Revised 2026-10-13, at SJL-4.4. The row read: "| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.1` | product | The number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-3.4; the admission probe restores it too. Published to nuget.org on 2026-10-13; this feed holds the package as published (revised that day: it held a copy packed locally before publication) |". That package and its pin left the feed in the same change: the allow-list admits one version of a package.)*

*(Revised 2026-10-14, when the package was published (JSC-303). The row read: "| `Broiler.ECMAScript.Algorithms` | `0.2.0-preview.1` | product | The numerics area of SJL-4 (JSD-0067) beside the number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-4.4; the admission probe restores it too. Packed locally from Broiler.JS `01209c4`; not yet on nuget.org |". The same library was published under the shared stream's 0.1.0 prefix, as `0.1.0-preview.2`, with one change to its code, `BigIntPolicy.RepresentationLimit`; `0.2.0-preview.1` will not be published, and its `.nupkg` and pin left the feed in the same change.)*

*(Revised 2026-10-15, at SJL-5.4. The row read: "| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.2` | product | The numerics area of SJL-4 (JSD-0067) beside the number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-4.4; the admission probe restores it too. Published to nuget.org on 2026-10-14 from Broiler.JS `b3d54f7`; this feed holds the package as published |". That package and its pin left the feed in the same change: the allow-list admits one version of a package.)*

*(Revised 2026-10-15, when the package was published (JSC-305). The row read: "| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.3` | product | The text area of SJL-5 (JSD-0070) beside the numerics area of SJL-4 (JSD-0067) and the number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-5.4; the admission probe restores it too. Packed locally from Broiler.JS `0db9042`; not yet on nuget.org |". The library code is the same; the copy packed locally left the feed in the same change.)*

*(Revised 2026-10-15, at SJL-6.4. The row read: "| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.3` | product | The text area of SJL-5 (JSD-0070) beside the numerics area of SJL-4 (JSD-0067) and the number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-5.4; the admission probe restores it too. Published to nuget.org on 2026-10-15 from Broiler.JS `6e20998`; this feed holds the package as published |". That package and its pin left the feed in the same change: the allow-list admits one version of a package.)*

*(Revised 2026-10-15, when the package was published (JSC-307). The row read: "| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.4` | product | The Date area of SJL-6 (JSD-0072) beside the text area of SJL-5 (JSD-0070), the numerics area of SJL-4 (JSD-0067) and the number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-6.4; the admission probe restores it too. Packed locally from Broiler.JS `e04874d`; not yet on nuget.org |". The library code is the same; the copy packed locally left the feed in the same change.)*

*(Revised 2026-10-16, at SJL-7.4. The table gained the rows of `Broiler.Unicode.Generation` and `Broiler.Unicode.Algorithms`, each `0.1.0-preview.4`, packed locally from Broiler.Unicode `6e14f93ad753094c99f0013d22269b47550ae552`. `Broiler.Unicode.Tables` is not admitted: this repository embeds its own tables (decision SJL-D-6 (c)).)*

*(Revised 2026-10-16, later at SJL-7.4, when the family's projects took the Unicode packages. `Broiler.Unicode.Segmentation`'s row was added, and `Broiler.Unicode.Algorithms`' read: "| `Broiler.Unicode.Algorithms` | `0.1.0-preview.4` | (dependency) | The Unicode area of SJL-7's code package (JSD-0074), here as `Broiler.Unicode.Generation`'s dependency, which reaches the architecture tests transitively; no project references it, and the allow-list admits it when one does. Packed locally from Broiler.Unicode `6e14f93`; not yet on nuget.org |".)*

*(Revised 2026-10-16, when the packages were published (JSC-310). The three rows ended "Packed locally from Broiler.Unicode `6e14f93`; not yet on nuget.org". The library code is the same; the copies packed locally left the feed in the same change.)*

*(Revised 2026-10-17, when the Unicode packages moved to `0.1.0-preview.5`: the faster normalization and case mapping, and the constant-time tables they read, which this repository embeds. The three rows ended "`0.1.0-preview.4` |" in their version column and "Published to nuget.org on 2026-10-16 from Broiler.Unicode `6e14f93` (its Publish workflow, run 38952273946); this feed holds the package as published". The preview.4 packages and their pins left the feed in the same change: the allow-list admits one version of a package.)*

*(Revised 2026-10-17, at SJL-8.5. The row read: "| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.4` | product | The Date area of SJL-6 (JSD-0072) beside the text area of SJL-5 (JSD-0070), the numerics area of SJL-4 (JSD-0067) and the number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-6.4; the admission probe restores it too. Published to nuget.org on 2026-10-15 from Broiler.JS `d1e4b11`; this feed holds the package as published |". That package and its pin left the feed in the same change: the allow-list admits one version of a package.)*

*(Revised 2026-10-17, later at SJL-8.5, when the time data home's generator reached the architecture tests. The table gained the row of `Broiler.ECMAScript.Generation` `0.1.0-preview.5`, packed locally from Broiler.JS `2d37a38a2560b076ebf2ca29989bfe20db64ed64`. No package carries the time zone or calendar tables: this repository embeds its own (decision SJL-D-6 (c)).)*

*(Revised 2026-10-17, when the packages were published (JSC-313). The rows of `Broiler.ECMAScript.Algorithms` and `Broiler.ECMAScript.Generation` ended "Packed locally from Broiler.JS `2d37a38`; not yet on nuget.org". The library code is the same; the copies packed locally left the feed in the same change.)*

*(Revised 2026-10-18, at SJL-10.4. The rows of `Broiler.ECMAScript.Algorithms` and `Broiler.ECMAScript.Generation` read: "| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.5` | product | The time-zone and calendar area of SJL-8 (JSD-0076) beside the Date area of SJL-6 (JSD-0072), the text area of SJL-5 (JSD-0070), the numerics area of SJL-4 (JSD-0067) and the number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-8.5; the admission probe restores it too. Published to nuget.org on 2026-10-17 from Broiler.JS `a04cd44` (its publish-shared workflow, run 39064131506); this feed holds the package as published |" and "| `Broiler.ECMAScript.Generation` | `0.1.0-preview.5` | test-only | The time data home's generator (JSD-0076, decision SJL-D-6 (c)): the tz, CLDR and calendar pin readers, the tz compiler and the table generators, which the architecture tests run for rules N29 to N32 and I4 since SJL-8.5; no shipping project may reference it. Published to nuget.org on 2026-10-17 from Broiler.JS `a04cd44` (its publish-shared workflow, run 39064131506); this feed holds the package as published |". The shared stream's next preview, `0.1.0-preview.6`, carries the Temporal core of SJL-10.3 beside the areas `0.1.0-preview.5` carried; `Broiler.ECMAScript.Generation` moves with the stream's version, its code unchanged. Both preview.5 packages and their pins left the feed in the same change: the allow-list admits one version of a package.)*

*(Revised 2026-10-18, when the packages were published (JSC-315). The rows of `Broiler.ECMAScript.Algorithms` and `Broiler.ECMAScript.Generation` ended "Packed locally from Broiler.JS `bf81d1b`; not yet on nuget.org". The library code is the same; the copies packed locally left the feed in the same change.)*

*(Revised 2026-10-19, when the shared stream moved to `0.1.0-preview.7` for JSD-0078's decision D12 (JSC-317). The rows of `Broiler.ECMAScript.Algorithms` and `Broiler.ECMAScript.Generation` read: "| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.6` | product | The Temporal core of SJL-10 (JSD-0078) beside the time-zone and calendar area of SJL-8 (JSD-0076), the Date area of SJL-6 (JSD-0072), the text area of SJL-5 (JSD-0070), the numerics area of SJL-4 (JSD-0067) and the number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-10.4; the admission probe restores it too. Published to nuget.org on 2026-10-18 from Broiler.JS `fc829f8` (its publish-shared workflow, run 39161419185); this feed holds the package as published |" and "| `Broiler.ECMAScript.Generation` | `0.1.0-preview.6` | test-only | The time data home's generator (JSD-0076, decision SJL-D-6 (c)): the tz, CLDR and calendar pin readers, the tz compiler and the table generators, which the architecture tests run for rules N29 to N32 and I4 since SJL-8.5; no shipping project may reference it. Its code is `0.1.0-preview.5`'s; it moved with the shared stream's version at SJL-10.4. Published to nuget.org on 2026-10-18 from Broiler.JS `fc829f8` (its publish-shared workflow, run 39161419185); this feed holds the package as published |". `0.1.0-preview.7`'s one change is decision D12, in the algorithms package: where a zone's skipped day breaks the day span's sign NudgeToZonedTime asserts, its rounding answers `OutOfRange`, a `RangeError` in this family, where `0.1.0-preview.6` answered a duration. `Broiler.ECMAScript.Generation` moves with the stream's version, its code unchanged. Both preview.6 packages and their pins left the feed in the same change: the allow-list admits one version of a package.)*

For signed preview.8 packages, `content-hash` is NuGet's restored content identity;
`archive-hash` separately pins SHA-512 of the complete signed archive. Both are
recorded in the admission list and checked by I1; they need not be equal.

*(Revised 2026-11-01. The three `Broiler.Unicode.*` rows ended "Downloaded from nuget.org on 2026-10-31; see its pin for archive hash and source commit.": no pin file was committed for them, so their archive hashes are the allow-list's `archiveHash`, and the source commit is the one each restored package's nuspec records.)*

*(Added 2026-11-01, at SJL-9.2c (JSC-321).)* `Broiler.ECMAScript.Generation` `0.1.0-preview.8`, admitted above on
2026-10-29, carries the Intl data home's generator (JSD-0082, decisions D10 to D12) beside the time data home's, and
the architecture tests run it for rules N27, N28 and I4 since SJL-9.2c, as restored from nuget.org into the NuGet global
packages folder, the archive rule `I1` holds to the allow-list's `archiveHash`. Its admission, version, hashes and
test-only scope are unchanged by that: its allow-list entry keeps its version, `contentHash`, `archiveHash`, use and
projects, its `admittedBy` gaining a sentence that says so; no entry joined or left the allow-list; and no `.nupkg` or
`.pin` was placed in this directory.

*(Revised 2026-11-03, when the shared stream's `0.1.0-preview.9` was published. The rows of `Broiler.ECMAScript.Algorithms` and `Broiler.ECMAScript.Generation` read: "| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.8` | product | Published package downloaded from nuget.org on 2026-10-29; source commit `3b4b6860d66a82a5dd6e6538f60b2bc4b1632fd6`. Existing admission scope is retained. |" and "| `Broiler.ECMAScript.Generation` | `0.1.0-preview.8` | test-only | Published package downloaded from nuget.org on 2026-10-29; source commit `3b4b6860d66a82a5dd6e6538f60b2bc4b1632fd6`. Existing admission scope is retained. |". Both packages were downloaded from nuget.org and their archives hash to the SHA-512 nuget.org's catalog publishes for them, which the allow-list's `archiveHash` now holds; their `contentHash` is the one a restore into an empty package cache recorded. `Broiler.ECMAScript.Intl` and `Broiler.ECMAScript.Frontend`, published at the same version, are not admitted: they enter with their cut-overs. No `.nupkg` or `.pin` was placed in this directory.)*

*(Revised 2026-11-03, at SJL-9.4, when `Broiler.ECMAScript.Intl` `0.1.0-preview.9` was admitted, product, for
`Broiler.VM.Profile.JavaScript` alone, by the same procedure: downloaded from nuget.org, its archive hashing to the
SHA-512 nuget.org's catalog publishes for it, which the allow-list's `archiveHash` holds, and its `contentHash` the
one a restore into an empty package cache recorded. The table gained its row; the revision above says "`Broiler.ECMAScript.Intl` and `Broiler.ECMAScript.Frontend`, published at the same version, are not admitted:
they enter with their cut-overs", which now holds of `Broiler.ECMAScript.Frontend` alone. No `.nupkg` or `.pin` was
placed in this directory.)*
