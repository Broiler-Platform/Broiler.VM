# The committed feed

A NuGet folder source for the shared JavaScript libraries' packages while an area is in development
(decision SJL-D-4 (c), taken 2026-10-11; milestone SJL-2.7). [`NuGet.config`](../../NuGet.config)'s
package source mapping sends the `Broiler.ECMAScript.*` prefix to this directory and to nuget.org,
and every other package to nuget.org only.

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
| `Broiler.ECMAScript.Algorithms` | `0.0.1-probe.1` | test-only | The admission probe of SJL-2.9: the contract types only, never published to nuget.org |
