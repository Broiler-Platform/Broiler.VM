# xUnit v3 migration

The architecture and contract suites use xunit.v3 4.0.2 and the existing
xunit.runner.visualstudio 4.0.1 / Microsoft.NET.Test.Sdk 18.11.0 selections.
Both test projects are executables, as v3 requires. Product projects are unchanged.
`IsTestingPlatformApplication=false` retains the repository's VSTest commands,
filters and TRX output on .NET 10, rather than requiring an SDK test-runner switch.

## Test changes

- Test output helpers use the v3 Xunit namespace.
- Predicate assertions use the direct Contains/DoesNotContain/Single overloads.
- Roslyn parsing and test waits receive the test cancellation token.
- The pinned-operation concurrency test uses a LongRunning task on the default
  scheduler to force a dedicated thread. Task.Run followed by GetResult can inline
  work on the waiting pool thread, so it was not a reliable affinity witness.
- The graph manifest names xunit.v3. The product-closure deny-list covers v3 core,
  common and in-process runner assemblies while retaining the v2 names. Three
  negative controls verify v3 assemblies are refused from product closures.
- No warning suppression was added, and existing product warning policy is retained.

## Existing package-state mismatch

The working tree already selected Unicode preview.6, while the admission list,
committed feed and current package evidence still described preview.5. Those are
now aligned with the selected version. Both signed-archive and restored-content
hashes remain checked, and the project/use restrictions are unchanged. The two
admission witnesses were refreshed so their sole defect remains the project/use
restriction being tested.

The new JS-10-013 evidence records successful candidate/rollback/roll-forward
consumer runs, the expected negative-control refusal and NativeAOT publish/run.
Older evidence was preserved. The collector's optional --aot-pack-version seeds
only the selected SDK runtime/compiler version for this local recertification.

## Verification

From a standalone checkout:

```powershell
dotnet restore Broiler.VM.slnx
dotnet build Broiler.VM.slnx -c Release --no-restore -warnaserror
dotnet test Broiler.VM.slnx -c Release --no-build --no-restore --logger trx
dotnet build Broiler.VM.slnx -c Debug --no-restore -warnaserror
dotnet list Broiler.VM.slnx package --vulnerable --include-transitive
```

A solution build before tests is necessary: architecture rules inspect assemblies
that are not test-project dependencies. A normal solution restore also supplies
restore metadata for all product profiles used by the admission tests.

Inside D:/Broiler the pre-existing D1 boundary check finds an incoming reference
from Broiler.JSeal.Vm. This migration does not relax that architectural policy or
remove that application's reference. The standalone GitHub checkout takes D1's
explicitly logged INCONCLUSIVE branch (EX-01), because there is no aggregate to
scan. A standalone verification copy is used to match CI; the aggregate run can
also execute the remaining tests with a filter excluding only that boundary case.

Include the restored ECMAScript preview.8 archives/pins and the Unicode preview.6
archives/pins, admission witnesses and new evidence when committing; a local
package cache must not hide missing committed-feed inputs on CI.

## Results (2026-10-31)

- Main VM: Debug rebuild and standalone Release build passed with warnings as errors;
  473 contract tests and 391 architecture tests passed (864 total).
- The separately versioned HyperV subtree was already on xUnit v3 3.2.2. Its test
  dependencies now match the main suite. The old CollectionBehavior parallelization
  property was replaced by Parallelization(Mode = None), keeping the suite serial.
  Debug/Release builds passed with warnings as errors; 35 tests passed with one
  expected platform-conditional skip.
- The main solution's transitive NuGet vulnerability audit reported no vulnerabilities.
- NativeAOT consumer recertification passed as recorded in JS-10-013.

Local verification logs and TRX files are under D:/Broiler/artifacts/vm-xunit3-*;
vm-xunit3-verification-summary.json identifies the latest test results. The aggregate
D1 boundary finding described above remains outside this migration's scope.
