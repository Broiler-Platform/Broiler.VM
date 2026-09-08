# Evidence bundle VM-7-CLI-001

**Subject:** `Broiler.VM.Composition.PolyglotCli`, the end-user command-line host and the first
image in this repository that composes two product profiles
**Collected:** 2026-09-07
**Core contract version:** 1
**Status of the work after this collection:** unaccepted, unreviewed, and advertised as nothing.

This bundle records what was published, what was run, and what came back. **It accepts nothing and
it closes nothing.** No person has read a line of the composition it describes, `docs/mvp.md`
defers that review still, and section 1 of the composition register goes on advertising no
composition at all.

**What this bundle is FOR is narrow and worth stating before anything else.** Rules K3 and K4 read
two files out of it - the catalog table a published binary printed, and the closure report of the
modes that were published - and hold them against the composition register and against a baseline
in the architecture test project. Those two files are the reason this directory exists. Everything
else here is context for a reader, and none of it is a gate.

---

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle ID | VM-7-CLI-001 |
| Subject | `Broiler.VM.Composition.PolyglotCli` |
| Register row | `docs/compositions.md` section 3 |
| Authorising record | `docs/adr/0001-component-topology-and-dependency-graph.md`, revision of 2026-09-07 |
| Runtime identifier | `win-x64`, one machine, one collection |
| Collector | The author of the change. Nobody else has read it. |

---

## 2. What was published

Two modes, both on `win-x64`:

- a framework-dependent publish, and
- a trimmed self-contained publish.

Both produced the same eight non-framework assemblies, which is what
`closure-polyglotcli.txt` lists, read off the published directories rather than described. The
trimmer removed none of them.

**A third mode was attempted and is not retained.** A Native AOT publish was started; the managed
compilation ran and the native link step failed to locate its toolchain on this machine, so no
native image exists and none is claimed. The closure file says so in its own header rather than
leaving the absence to be inferred. **This composition has not been shown to publish under Native
AOT, and nothing in this bundle says it has.**

---

## 3. What was run

`publish-and-run.log` is the transcript of the published framework-dependent binary answering
thirteen command lines, with the absolute path of each input rewritten to its file name and nothing
else edited. The inputs it names are retained under `inputs/` so a reader can run the same thing -
all but one of them. `mislabelled.js` is not retained because it is a byte-for-byte copy of
`adder.wasm` under a JavaScript name, and a directory of documents is not the place to keep a binary
wearing a source file's extension; copy `inputs/adder.wasm` to `mislabelled.js` to reproduce that
row.

What the transcript shows, in the order a reader should weigh it:

- **Both profiles ran, out of one catalog, in one process.** `broiler run hello.js adder.wasm`
  compiled and ran a JavaScript file and verified, instantiated and invoked a WebAssembly module,
  and reported one distribution over the two.
- **The two output forms of the numeric surface agreed.** The same source answered `999000` through
  the interpreter and through machine code emitted by `x86-64-win64` and armed by this image.
- **The two forms this machine cannot arm refused by name.** An artifact emitted for
  `x86-64-sysv` on a Windows process, and one emitted for `arm64-aapcs64` anywhere, each verified
  and then declined to instantiate, reporting the reason rather than a defect.
- **A mislabelled file was named as one in both directions.** A module saved as `.js` and a text
  file saved as `.wasm` were each refused by the router, before any front end saw a byte.
- **`compile` refused a `.wasm` input by name**, because the WebAssembly profile carries no lowering
  in this image and a copy presented as a compilation would be a claim about one.

---

## 4. Environment

`environment.txt` records the machine, the SDK and the runtime identifier. One machine, one
operating system, one lane. No other runtime identifier was built and none is claimed.

---

## 5. What is NOT here, named rather than left as an absence

- **No Native AOT closure**, for the reason section 2 gives.
- **No conformance run.** Neither profile's suite was run against this composition. The JavaScript
  profile's own bundles hold its suite work and this bundle does not repeat or extend it.
- **No measurement of anything.** No timing, no throughput, no image size, no comparison between the
  two output forms beyond that they answered the same value. `docs/baselines.md` and rule L1 govern
  what may be retained as a figure, and nothing here was collected under a predeclared rule, in an
  A/A lane, against a control, with repetitions. So there is no number in this bundle a reader could
  mistake for a baseline, deliberately.
- **No acceptance suite of this host's own.** The JavaScript host it was copied from has one, driven
  over its built binary; this root has a transcript and not a driver, and a transcript is weaker
  because nothing fails when it changes.
- **No human review.** Nothing here has been read by anybody but the author.

---

## 6. What a reader should take from it

That a second product profile was added to a composition by writing two `Add` calls, and that the
core runtime, the execution loop and the two profiles were not changed to allow it. That is the
property `docs/roadmap.md` section 14 asked to see, and it is the whole of what this bundle
demonstrates. It demonstrates nothing about how well either profile runs anything, and it supports
no claim about either surface's completeness.

---

## 9. Exclusions

**This section defines no exclusion identifier, and that is deliberate rather than an omission.**
An identifier here is a thing other documents cite by name, and nothing in this repository cites
this bundle. What would have been an identified exclusion is written as prose in section 5 instead,
where a reader meets it beside the thing it excludes rather than in a table at the end.

The limits of this collection, restated in one place so that they are not spread across the
document: one runtime identifier, one machine, one collector, two publish modes of three, no
conformance run, no retained figure, and no review.
