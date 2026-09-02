# Bundle JS-ANDROID-002 — the Android head's negative controls

**Collected:** 2026-09-02. **Owner:** profile architecture owner. **Reviewer:** none.

**What this bundle is.** The gap [JS-ANDROID-001](../js-android-001/README.md) recorded, closed.
That bundle had no negative control and said so in those words: every control in this component
injects into a source file and is judged by a suite, a replay or a fuzz session, and none of those
can judge an Android head. **Two controls now can**, and this is their log.

**It advances no milestone and claims no RID.** What it adds is that the head's own result means
something: a run that passes is now a run that would have failed had the thing it checks been
broken.

---

## 1. What judges these

An Android head that ran. Nothing else in the component can: rule A11 forbids a test project to
reference a profile assembly, and the corpus replay and the fuzz session are both desktop
processes.

A control **passes** when the injected run does **not** print the sentinel and the reverted run
does. The direction is the same one the CI job uses and it is chosen for the same reason: an
application hands no exit code to a harness, so the sentinel's absence is what a broken run and a
failed check have in common. A control that looked for the word `FAIL` would pass on a run that
never started.

## 2. The two controls, and why each

| Control | What it injects | What it proves |
|---|---|---|
| `the-extraction-changes-one-byte` | one byte flipped as a corpus resource is written to the cache directory | **The Android-specific claim.** The corpus travels into this image as embedded resources, and JS-ANDROID-001's argument that the round trip cannot corrupt the evidence rests on the replay re-hashing every entry. Until this ran, that was asserted |
| `the-language-guards-division-by-zero-on-the-device` | division by zero made a fault in the **profile** — the same injection a desktop corpus control uses | **That the head detects a real engine regression** rather than only its own plumbing. Judged on the device, it names the four entries that stop agreeing: `division-by-zero`, `zero-over-zero`, `negative-zero-division`, `not-a-number-is-not-itself` |

## 3. Results

`android-controls.log`, unedited. **Two run, two passed, none skipped.**

## 4. What the harness had to learn, recorded because it cost two wrong results

**The first run of these controls reported both as FAIL, and the harness was the thing that was
wrong.** After a revert the build is up to date, MSBuild skips the install target in five seconds,
and the device keeps running the APK the *injected* build put there — so the reverted run
reproduces the injection and every control fails for the same reason, which is not the reason its
row names. The package is uninstalled before each install now.

**That was not sufficient either.** An Android build that changes a source file and rebuilds in
place can produce an APK whose Java stubs no longer match its managed side, and the launch dies
with `UnsatisfiedLinkError` on `n_onCreate` — which reads as a code fault and is a stale `obj/`
directory. Each run deletes the head's intermediates first. It costs about a minute a build and
buys a result that means what it says.

**Both failures looked exactly like a working control.** A control whose injected run does not
print the sentinel is what a passing control looks like from one side; only the reverted run's
sentinel tells the two apart, which is why the verdict requires it and why the log prints both.

## 5. Exclusions — what this bundle does not show

1. **An emulator is not a device**, and the RID that ran is `android-x64`. The same two exclusions
   JS-ANDROID-001 leads with.
2. **Two controls are not a control matrix.** The desktop tables carry seventeen, thirteen and two;
   these two cover the extraction path and one language semantic, and nothing else about this head
   is controlled — not the catalog table, not the closure report, not the ordering assertions.
3. **They run where an emulator is.** The collection script skips them loudly when no Android SDK
   is present, and a skipped control is a gap rather than a smaller total. The CI lane does not run
   them: it runs the head, which is a regression signal, and a control there would double the
   emulator time for a result no bundle retains.
4. **Nothing here is reviewed**, and no milestone moves.
