# JS-10-013 — Unicode preview.6 dependency recertification

Collected 2026-10-31 on Windows using SDK 10.0.403 and its 10.0.14 NativeAOT
packs. The working tree, including uncommitted xUnit v3 changes, is recorded in
`manifest.txt`. This is local packaging evidence, not a publication or independent
review. Earlier retained bundles are unchanged.

The central package versions already selected Unicode preview.6. Its admission
list and committed feed now identify the published packages at that version,
with both NuGet's restored content identity and the full signed-archive hash.

```powershell
python eng/collect-js-packages.py --bundle JS-10-013 --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-10-013 --rid win-x64 --aot-pack-version 10.0.14
```

All three pristine managed consumer runs passed (candidate, rollback, roll-forward).
The unavailable-version negative control failed as expected. NativeAOT publish
and execution passed. `consumer.log` retains the actual output, `nuspecs.txt` and
`contents.txt` retain the candidate metadata, and `hashes.txt` records the retained
files' digests. The optional seed version avoids copying unrelated cached SDK
versions while preserving the existing isolated, offline consumer check.
