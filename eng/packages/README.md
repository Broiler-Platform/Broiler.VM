# The committed feed

A NuGet folder source for the shared JavaScript libraries' packages while an area is in development
(decision SJL-D-4 (c), taken 2026-10-11; milestone SJL-2.7). [`NuGet.config`](../../NuGet.config)'s
package source mapping sends the `Broiler.ECMAScript.*` and `Broiler.Unicode.*` prefixes to this
directory and to nuget.org, and every other package to nuget.org only. *(Revised 2026-10-16, at SJL-7.4,
when the Unicode area's owner packages were admitted. It read "sends the `Broiler.ECMAScript.*` prefix to
this directory and to nuget.org".)*

- **Every package here has a `.pin` beside it**: its identifier, version, NuGet content hash, SHA-256,
  and the upstream commit it was packed from. A lock file pins the same content hash whichever source
  supplied the package, and rule `I1` holds the lock file, the pin and the allow-list
  (`src/tests/Broiler.VM.Architecture.Tests/admitted-packages.json`) to one another.
- **Nothing else is a source.** No other repository's local folder, `D:\local-packages` included, is
  ever added to a configuration in this repository.
- **A package a Broiler.VM release depends on comes from nuget.org**, and `eng/verify-feed.ps1`
  refuses a release whose admitted dependency is not there yet. This directory is for development and
  for test-only admissions.

| Package | Version | Admitted as | Why it is here |
|---|---|---|---|
| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.4` | product | The Date area of SJL-6 (JSD-0072) beside the text area of SJL-5 (JSD-0070), the numerics area of SJL-4 (JSD-0067) and the number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-6.4; the admission probe restores it too. Published to nuget.org on 2026-10-15 from Broiler.JS `d1e4b11`; this feed holds the package as published |
| `Broiler.Unicode.Algorithms` | `0.1.0-preview.4` | product | The Unicode area of SJL-7's code package (JSD-0074): property lookups, lexical classes, case folding, case mapping, normalization and properties of strings, over tables this repository embeds; referenced by Format, the runtime, the compiler and the Intl data since SJL-7.4, and `Broiler.Unicode.Generation`'s dependency. Packed locally from Broiler.Unicode `6e14f93`; not yet on nuget.org |
| `Broiler.Unicode.Segmentation` | `0.1.0-preview.4` | product | The Unicode area's optional segmentation (JSD-0074), referenced by the Intl data assembly alone since SJL-7.4. Packed locally from Broiler.Unicode `6e14f93`; not yet on nuget.org |
| `Broiler.Unicode.Generation` | `0.1.0-preview.4` | test-only | The owner's table generator and pin (JSD-0074, decision SJL-D-6 (c)), which the architecture tests run for rules N22, N28 and I4 since SJL-7.4; no shipping project may reference it. Packed locally from Broiler.Unicode `6e14f93`; not yet on nuget.org |

*(Revised 2026-10-12, at SJL-3.4. The table's row read: "| `Broiler.ECMAScript.Algorithms` | `0.0.1-probe.1` | test-only | The admission probe of SJL-2.9: the contract types only, never published to nuget.org |". The probe pre-release, its `.nupkg` and its pin left the feed in the same change: no project references it, and the allow-list admits one version of a package.)*

*(Revised 2026-10-13, at SJL-4.4. The row read: "| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.1` | product | The number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-3.4; the admission probe restores it too. Published to nuget.org on 2026-10-13; this feed holds the package as published (revised that day: it held a copy packed locally before publication) |". That package and its pin left the feed in the same change: the allow-list admits one version of a package.)*

*(Revised 2026-10-14, when the package was published (JSC-303). The row read: "| `Broiler.ECMAScript.Algorithms` | `0.2.0-preview.1` | product | The numerics area of SJL-4 (JSD-0067) beside the number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-4.4; the admission probe restores it too. Packed locally from Broiler.JS `01209c4`; not yet on nuget.org |". The same library was published under the shared stream's 0.1.0 prefix, as `0.1.0-preview.2`, with one change to its code, `BigIntPolicy.RepresentationLimit`; `0.2.0-preview.1` will not be published, and its `.nupkg` and pin left the feed in the same change.)*

*(Revised 2026-10-15, at SJL-5.4. The row read: "| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.2` | product | The numerics area of SJL-4 (JSD-0067) beside the number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-4.4; the admission probe restores it too. Published to nuget.org on 2026-10-14 from Broiler.JS `b3d54f7`; this feed holds the package as published |". That package and its pin left the feed in the same change: the allow-list admits one version of a package.)*

*(Revised 2026-10-15, when the package was published (JSC-305). The row read: "| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.3` | product | The text area of SJL-5 (JSD-0070) beside the numerics area of SJL-4 (JSD-0067) and the number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-5.4; the admission probe restores it too. Packed locally from Broiler.JS `0db9042`; not yet on nuget.org |". The library code is the same; the copy packed locally left the feed in the same change.)*

*(Revised 2026-10-15, at SJL-6.4. The row read: "| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.3` | product | The text area of SJL-5 (JSD-0070) beside the numerics area of SJL-4 (JSD-0067) and the number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-5.4; the admission probe restores it too. Published to nuget.org on 2026-10-15 from Broiler.JS `6e20998`; this feed holds the package as published |". That package and its pin left the feed in the same change: the allow-list admits one version of a package.)*

*(Revised 2026-10-15, when the package was published (JSC-307). The row read: "| `Broiler.ECMAScript.Algorithms` | `0.1.0-preview.4` | product | The Date area of SJL-6 (JSD-0072) beside the text area of SJL-5 (JSD-0070), the numerics area of SJL-4 (JSD-0067) and the number-conversion area of SJL-3 (JSD-0065), referenced by the runtime and the compiler since SJL-6.4; the admission probe restores it too. Packed locally from Broiler.JS `e04874d`; not yet on nuget.org |". The library code is the same; the copy packed locally left the feed in the same change.)*

*(Revised 2026-10-16, at SJL-7.4. The table gained the rows of `Broiler.Unicode.Generation` and `Broiler.Unicode.Algorithms`, each `0.1.0-preview.4`, packed locally from Broiler.Unicode `6e14f93ad753094c99f0013d22269b47550ae552`. `Broiler.Unicode.Tables` is not admitted: this repository embeds its own tables (decision SJL-D-6 (c)).)*

*(Revised 2026-10-16, later at SJL-7.4, when the family's projects took the Unicode packages. `Broiler.Unicode.Segmentation`'s row was added, and `Broiler.Unicode.Algorithms`' read: "| `Broiler.Unicode.Algorithms` | `0.1.0-preview.4` | (dependency) | The Unicode area of SJL-7's code package (JSD-0074), here as `Broiler.Unicode.Generation`'s dependency, which reaches the architecture tests transitively; no project references it, and the allow-list admits it when one does. Packed locally from Broiler.Unicode `6e14f93`; not yet on nuget.org |".)*
