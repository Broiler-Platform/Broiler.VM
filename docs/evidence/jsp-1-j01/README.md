# JSP-1 runner work: JSeal slice J01

Date: 2026-09-20. Owner: JavaScript profile tooling. Reviewer: none.
This is local implementation validation, **not accepted milestone evidence or a conformance score**.
It covers the differential-runner portion of JSP-1. The broader document-absence audit remains open.

The change extends [the existing runner](../../../eng/run-differential.py). Reproduction and the
configuration/report contracts are in the [probe README](../../../src/tests/differential/README.md).
No production engine behavior changed. Historical expected answer values and declaration reasons
were preserved; formerly unscoped declarations now explicitly name Node. Its original calibration
version is unknown, and those declarations are not asserted to hold against current Node.

## Executed on Windows

- [windows-tests.txt](windows-tests.txt): subprocess tests of explicit goals, paths containing
  spaces/non-ASCII characters, engine identity, timeout with a descendant, missing executable,
  host/comparison exit codes, stderr, invalid UTF-8, duplicate/missing/extra/reordered cases,
  significant whitespace, stale/engine-scoped declarations and independent retained answers.
- [windows-broiler-js.json](windows-broiler-js.json): real VM and current-source Broiler.JS run
  the script and module smoke fixtures. Reports identify binaries, source revisions, commands,
  fixture hashes, raw output and declared differences. The VM checkout is dirty with this work;
  its recorded HEAD alone is not the identity of the modified runner, whose SHA-256 is also recorded.
- [windows-node.json](windows-node.json): the same fixtures run against Node with its print
  helper preloaded separately, preserving the script's strict directive.
- [windows-vm-regression.json](windows-vm-regression.json): the full retained-answer lane exits
  1 for the existing library case 36: retained cube root `1.2599210498948734`, observed
  `1.259921049894873`. Other retained probes agree, including async subcases `39a`/`39b`.
  The floating-point spelling is preserved, not rounded into agreement.

Run from the VM root after building the VM and JS CLI projects in Release:

```powershell
python -m unittest discover -s eng/tests -p test_differential.py -v
python eng/run-differential.py --probe-directory src/tests/differential/smoke --against D:/Broiler.JS/Broiler.JS/Broiler.JavaScript/bin/Release/net10.0/BroilerJS.exe --against-kind broiler-js --against-root D:/Broiler.JS --report docs/evidence/jsp-1-j01/windows-broiler-js.json
python eng/run-differential.py --probe-directory src/tests/differential/smoke --against node --report docs/evidence/jsp-1-j01/windows-node.json
python eng/run-differential.py --timeout 10 --report docs/evidence/jsp-1-j01/windows-vm-regression.json
```

The module smoke expectations are authored from the module contract. Broiler.JS at `73f071d1`
reads an imported mutable primitive binding as 41 after the exporter increments it, where VM
reads 42; its module top-level `this` is non-undefined, where VM gives undefined. These differences
are declared only for `broiler-js`, with both observed values retained. A fix making either agree
will fail this lane as stale until its declaration is removed. Node has no exemptions here.

Additional exploratory calls to the larger corpus remained failures: `the-with-statement` under
Broiler.JS emits duplicate case 54, and `the-module-goal` exits with a parser error at the string-named
export in `modules/counter.mjs`. The driver reports both. They were not repaired or accepted as
exemptions, and the full comparison corpus is not claimed green.

## Pending platform validation

The `differential-tooling` job in
[broiler-vm-lane.yml](../../../.github/workflows/broiler-vm-lane.yml) runs the subprocess tests and
real VM/Node script/module smoke fixtures on Windows and Linux, retaining the JSON report. It has
been added locally, not dispatched or observed. Both registered WSL distributions on this machine
fail to start with `ERROR_PATH_NOT_FOUND` for their backing disks. No Linux result is claimed.
J01's cross-platform acceptance remains pending until that lane executes. Broiler.JS comparison
on Linux additionally requires an explicitly supplied source build; this CI job does not fetch
a separate repository or substitute pinned packages for current source.
