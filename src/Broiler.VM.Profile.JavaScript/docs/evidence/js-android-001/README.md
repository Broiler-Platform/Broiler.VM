# Bundle JS-ANDROID-001 — the JavaScript profile on an Android runtime

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** The first collection taken from this component on a runtime that is not
CoreCLR. An Android head composes the same profile the execution-only root composes, carries the
retained corpus into its image, and runs the corpus replay and the ordering assertions on a
booted Android system. It answers one question — *does this component's verifier and executor
work on an Android RID at all* — and nothing else.

**It advances no milestone and claims no RID.** A collection is what a claim would have to be
about; the claim itself is a release act and JS-10 owns it.

---

## 1. Identity

| Field | Value |
|---|---|
| Component commit | `062e5bc097075fd824f67f32e61c9a39838ec167`, plus the working tree that adds this head — the tree is dirty by construction, because the head is what is being collected |
| Core contract version | 1 (implemented; **not accepted**) |
| Format version | 1 |
| Feature manifest | `broiler.javascript.slice` |
| Composition | `Broiler.VM.Composition.JavaScript.Android`, registered as a demonstration |
| Corpus | the retained `js-1` corpus, 66 entries, carried as embedded resources |
| SDK | .NET 10.0.400; Android workload 36.1.69/10.0.100 |

## 2. Source

The head is `src/compositions/Broiler.VM.Composition.JavaScript.Android`. Its checks are **not its
own code**: `Hosts.cs`, `CorpusReplay.cs` and `OrderingChecks.cs` are compiled from the
execution-only root, so what ran on the device is the same source that runs on a desktop RID. The
driver — the activity, the resource extraction and the report — is the only code in the image that
exists for this head.

## 3. Environment

An Android emulator, API 36, `x86_64`, booted headless on a Windows x64 host: `pixel_9_pro_xl_-_api_36_0`
with the `android-36` system image. **Not a device**, and see the exclusions.

The image was published with `RuntimeIdentifier=android-x64`, and the report reads its RID off the
device rather than off the build — `rid=android-x64` in `closure-android.txt` is what actually ran.

## 4. Procedure

```
dotnet build src/compositions/Broiler.VM.Composition.JavaScript.Android -c Release \
  -f net10.0-android36.0 -p:RuntimeIdentifier=android-x64 -t:Install
adb shell monkey -p com.broiler.vm.composition.javascript.android \
  -c android.intent.category.LAUNCHER 1
adb logcat -d -s broiler-js-android:*
```

## 5. Results

`run.log` is the unedited logcat. **Six checks passed:**

- the retained corpus replayed to its recorded answers — **all 66 entries**, on Mono, on Android;
- it replayed twice with both passes agreeing row for row, so the run left no residue;
- the four ordering assertions — ceilings materialised before the first byte, a well-formed
  artifact charging for what it allocates, a declaration past its bound sizing nothing, and
  allocation proportional to the bytes present.

`catalog-android.txt` and `closure-android.txt` are the catalog table and the closure the image
printed, in the format the desktop roots print them. The closure holds **exactly six Broiler-owned
assemblies** and no test, reflection or dynamic-code assembly.

## 6. Negative controls

**None, and that is a gap rather than an omission with a reason.** Every control in this component
injects into a source file and is judged by a suite, a replay or a fuzz session that a script can
run; nothing here can inject into a source file and re-run an emulator without a harness that does
not exist yet. The CI lane's Android job is where a control would live, and it is owed.

> **Correction, 2026-09-02.** The gap is closed and this section's last sentence guessed wrong
> about where. The controls are in the collection script, not the lane, for the reason every other
> control in this component is: a control belongs to a bundle, and the lane retains nothing.
> [Bundle JS-ANDROID-002](../js-android-002/README.md) carries two of them - one for the resource
> extraction this bundle's own argument rests on, one for a language semantic - both passing.
> **Nothing in the log below is edited**, and the sentence above stands as what was true when this
> collection was taken.

## 7. Exclusions — what this bundle does not show

1. **An emulator is not a device.** Nothing here is evidence about phone hardware, a real GPU, a
   real thermal envelope or a real memory ceiling.
2. **The RID that ran is `android-x64`, not `android-arm64`.** The emulator is x86_64. The image
   was *published* for arm64 too and nothing here executed it, so `android-arm64` is published and
   not run, which is the state ADR 0012 declines to call declared.
3. **Neither trimming nor any AOT.** Android runs Mono and this head sets `PublishTrimmed=false`
   and `RunAOTCompilation=false`, matching the consuming repository's own Android head. **No
   sentence here is evidence about Native AOT**, which is what every other RID in this component's
   matrix is compiled with.
4. **The soak, the fuzz sessions and the aggregate-budget exercises did not run.** They are
   wall-clock and heap-shaped and an emulator is neither a machine nor a stable one; a plateau band
   read there would be a figure attributable to nothing.
5. **One machine, one collection, one emulator image.** No repetition, no second host.
6. **Nothing here is reviewed**, and no milestone moves.
