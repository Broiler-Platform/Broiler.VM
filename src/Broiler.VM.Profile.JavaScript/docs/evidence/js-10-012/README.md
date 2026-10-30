# Bundle JS-10-012 — ECMAScript preview.8 package recertification

Collected on 2026-10-30 on Windows with SDK 10.0.403 from the working tree recorded
in `manifest.txt`. The tree includes uncommitted repairs; this is local packaging
evidence, not a publication or an independent review.

The collector packs the candidate, restores and runs a pristine consumer with
upstream unavailable, rolls back to the published previous set, rolls forward,
checks an unavailable-version negative control, and publishes and runs NativeAOT.
`consumer.log` records the actual outcomes; `hashes.txt` authenticates the retained
files. No historical evidence bundle was edited.

Command:

```powershell
python eng/collect-js-packages.py --bundle JS-10-012 --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-10-012 --rid win-x64
```

The ECMAScript preview.8 archives were downloaded from NuGet.org and verified
against the admission list before collection. The collector now respects `--rid`
when selecting NativeAOT packs and the executable filename.

All three managed consumer runs passed and the unavailable-version control failed
as expected. The first NativeAOT attempt failed because the collector seeded no
WindowsDesktop runtime pack. The native step was retried with that pack and the
SDK 10.0.14 packs in a fresh isolated cache; publish and all six native consumer
checks passed. Both attempts remain in `consumer.log`. The retry uses the same
candidate feed; managed steps were not replayed. The collector now includes the
WindowsDesktop runtime pack for Windows RIDs so a future full run can seed it.
