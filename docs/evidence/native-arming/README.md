# Native arming restored to the conformance harness and the end-user host

Date: 2026-09-23. Owner: JavaScript profile. Reviewer: none.
This is **local implementation validation, not accepted milestone evidence**. Nothing here is a
conformance score, a support claim or a measurement of any kind, and nothing in it has been read
by a human reviewer. Approvals stay deferred under the MVP terms ([`docs/mvp.md`](../../mvp.md)).

Checkout: detached worktree `D:/wt/native-arming` at `f902cbb` of
`jseal-roadmap/javascript-profile`. Windows 11, .NET SDK 10.0.401, Python 3.11.
Pinned suite: `test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`, unpacked at
`D:/test262-pinned/`. Every figure below is from a run taken in this worktree.

## 1. What was wrong

A wide-manifest artifact in its native form refused to instantiate in
`Broiler.VM.Composition.JavaScript.Conformance` and in
`Broiler.VM.Composition.JavaScript.Cli`: `ProfileFault/UnsatisfiedHostAssumption`, raised where
`src/Broiler.VM.Profile.JavaScript/JsExecution.cs` maps the page at instantiation, because `JsEngine.NativePageOf` answered
null, because `JsNativePage.TryMap` answered null, because the static `JsNativePage.Mapper` hook
was never installed in those processes. The only assignment in the tree was
`src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/Program.cs`, and that
composition was also the only one naming `Broiler.VM.Profile.MachineCode`.

Commit `db57c2f` (2026-09-18) moved the arming path out of the profile into that assembly and
`7bfe561` restored the reference for the slice-compiler root alone. Before those two, every
composition holding `Broiler.VM.Profile.JavaScript` could arm a page. Retained evidence from
2026-09-15 -
`src/Broiler.VM.Profile.JavaScript/docs/evidence/jsb-11-001/test262-native-wide.report.gz`, bundle
JSB-11-001 - holds a native run over the wide manifest passing 70,834 of 94,545 variants of the
whole pinned suite. So this was a wiring regression that withdrew a capability, not a profile
defect.

## 2. What changed

- `Broiler.VM.Composition.JavaScript.Conformance` and `Broiler.VM.Composition.JavaScript.Cli` each
  gained a `ProjectReference` to `Broiler.VM.Profile.MachineCode.csproj` and
  `AllowUnsafeBlocks` set true.
- Each `Program.cs` gained `InitializeNativeMapping`, copied from the slice-compiler root, called
  from a static constructor as well as from `Main` so an entry point reached by some other route is
  covered. Both carry the assurance annotation pair, assessed as the arming path's own units are -
  `Origin=AI; IP=Low; Security=Medium; Resources=2` - matching `VmNativePage.TryMap` and
  `VmNativePage.Arm`, which is what these four lines delegate to. No `Broiler-Falsified-If:` line,
  because the pair is not `Security=High` or `Critical`.
- `docs/compositions.md`: the Conformance and Cli rows' sibling-assembly cells gained
  `Broiler.VM.Profile.MachineCode` and their native-execution cells moved from `none` to `x86-64`.
  The one-line paragraph that stood under the register table, dated 2026-09-18, is replaced by a
  dated correction that quotes it and says what happened; it invents no decision record, because
  there is none.
- `src/tests/Broiler.VM.Architecture.Tests/graph.manifest.json`: two edges added, and the
  `$comment` narration extended. Its VM-8 sentence said 27 projects and 87 edges while ADR 0001's
  own 2026-09-18 revision said 88 and the checkout held 88; that sentence is corrected to 88 in
  place, with the discrepancy named rather than silently fixed.
- `docs/adr/0001-component-topology-and-dependency-graph.md`: a dated revision for 2026-09-23.
  Rule A15 requires it - the record's LAST budget sentence must state the graph's size, and the
  graph goes from 27 projects and 88 edges to 27 and 90. The revision is a topology record entry,
  not a decision record, and it says so.
- Retained closure evidence corrected for the two roots (section 4).
- **Two checks, because a cell claiming a capability is worse than a cell withdrawing one.**
  Rule `K5` gained a clause: a row naming an architecture over an image that links the arming path
  and whose ROOT names that assembly's page type nowhere is the same overstatement as one that
  links nothing at all. Linking the arming path is not arming. The clause has its own rejecting
  control in the group K test, its own sentence in the rule register, and the clause's input is
  asserted non-empty in both directions before any row is read. Separately, the end-user host's
  `--runtime` line gained a `native-arming=` field reporting whether this process filled the
  profile's mapper hook, and `src/tests/cli/expected.txt` gained a row reading its VALUE. The
  host's `--help` entry for `--runtime` enumerates the fields of that line, so it gained the new
  one: a host whose own help understates what it prints is the defect this component treats the
  same way as one that overstates it. Section 3.5 records the mutations that show both checks are
  load-bearing.

## 3. Commands, before and after

Every command was run from `D:/wt/native-arming`. The suite root is
`D:/test262-pinned/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`.

### 3.1 The point of the change: the conformance host, both forms, compared per variant

The lane's own selection and the lane's own comparison, read off
`.github/workflows/broiler-vm-lane.yml` and `eng/compare-test262-forms.py`:

    python eng/run-test262.py --suite SUITE --jobs 12 --form bytecode --wall 60000 \
      --out OUT/t262-bytecode --dir test/language/statements/class --dir test/built-ins/Promise
    python eng/run-test262.py --suite SUITE --jobs 12 --form native --wall 60000 \
      --out OUT/t262-native --dir test/language/statements/class --dir test/built-ins/Promise
    python eng/compare-test262-forms.py OUT/t262-bytecode/test262.report \
      OUT/t262-native/test262.report --exempt-guest-loads --suite SUITE

| | files | variants | passed | failed | skipped |
|---|---|---|---|---|---|
| bytecode form (unchanged by this work) | 5,044 | 9,962 | 9,855 | 54 | 53 |
| native form, **before** | 5,044 | 9,962 | 1,280 | **8,629** | 53 |
| native form, **after** | 5,044 | 9,962 | **9,855** | 54 | 53 |

Both native reports carry `form|native|x86-64-win64` in their headers, so neither silently
compiled to bytecode. The "after" row was taken twice - once when the wiring landed and once
against the final tree, after the checks in section 3.5 were added - and answered the same three
figures and the same `0 differ` both times. Every one of the 8,629 failures before carried
`UnsatisfiedHostAssumption`. The comparison script:

- **before:** `8575 difference(s) in no admitted class`, exit 1. Every one reads
  `Passed -> Failed  the artifact would not instantiate`.
- **after:** `9962 variants in both reports, 0 differ`, `every difference is in an admitted class`,
  exit 0. A per-variant exact match with the bytecode form over 9,962 variants.

The 1,280 that passed before are the negative variants - a test whose declared verdict is a
compile-time or verify-time refusal reaches its verdict without an instance, so it scored the same
either way. The 54 failures the two forms now share are the bytecode form's own and are unrelated
to this change.

### 3.2 The end-user CLI

`tiny.js` is `var x = 1;` followed by `x = x + 41;`. **Before** was taken against a separate
worktree at the same commit (`D:/wt/native-arming-base`) so the figures are from a build, not from
recollection.

| command | before | after |
|---|---|---|
| `Cli tiny.js` | `42`, exit 0 | `42`, exit 0 |
| `Cli --native x86-64-win64 tiny.js` | `the artifact verified and would not instantiate: (ProfileFault/UnsatisfiedHostAssumption)`, exit 4 | `42`, exit 0 |
| `Cli --native x86-64-sysv tiny.js` | same refusal, exit 4 | same refusal, exit 4 |
| `Cli --native arm64-aapcs64 tiny.js` | `2104:ConstructOutsideManifest`, exit 3 | unchanged, exit 3 |
| `Cli --runtime --check tiny.js` (standard error) | the configuration line, with no `native-arming` field on it | the same line ending `native-arming=installed` |

The last two of the `--native` rows are why the "after" column is only one row wide: the convention
this process does not use, and the architecture nothing arms, must still refuse by name. They do.

### 3.3 The numeric arm

The numeric arm reaches the same `JsNativePage.TryMap`, so it was refused for the same reason.
`src/tests/forms/loop-integer.js` at `--fuel 2000000000 --wall 60000`:

| command | before | after |
|---|---|---|
| `Cli --numeric` | `499999500000`, exit 0 | `499999500000`, exit 0 |
| `Cli --numeric --native x86-64-win64` | `ProfileFault/UnsatisfiedHostAssumption`, exit 4 | `499999500000`, exit 0 |

The two forms answer the same value.

### 3.4 The gate and the lanes

| command | before | after |
|---|---|---|
| `dotnet build Broiler.VM.slnx -c Release` | 0 warnings, 0 errors | 0 warnings, 0 errors |
| `dotnet test Broiler.VM.slnx -c Release --no-build` - Contract | 267/267 | 267/267 |
| the same run - Architecture | 266/266 | 266/266 |
| `SliceCompiler.exe --checks` | 372 passed, 2 not run | 372 passed, 2 not run |
| `python eng/run-cli-acceptance.py --binary-directory src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0` | 225/225 | **227/227** |
| `ExecutionOnly.exe --corpus src/tests/corpus/js-1` | 22 checks passed | 22 checks passed |
| `Cli.exe --host-surface` | every check passed | every check passed |

The acceptance table grew by the two rows section 3.5 is about. The two test counts did not move:
the `K5` clause is an assertion inside the rule's existing test rather than a new one.

Four architecture rules failed in between and are what the bookkeeping is for, in the order they
were met: `A15` (ADR 0001's last budget no longer described the graph), `A7` (the checkout had two
edges the manifest did not), `K2` (the Conformance row's sibling cell disagreed with its reference
set), and `J12` (the sentence added to `K5`'s register row counted composition roots, which is a
figure the catalog computes; it was reworded to stop counting them). A fifth, `H3`, went red on the
first draft of this record because it cited source files by line number; the citations were
reworded.

`K5` never failed on this checkout, and that is the part worth stating. It reads the register's own
closure cells, so it stayed green through the five days the capability was gone: the cell said
`none` and the image really could arm nothing. **It holds the register to the image and cannot hold
the image to anybody's intent, and nothing here changes that.** What the clause added on 2026-09-23
does change is the other direction, which is the one this change creates: a row that now CLAIMS an
architecture has to have a root that reaches the arming path, so half-done wiring fails instead of
passing. Before that clause, the project reference was held by `A7` and the graph manifest and the
four lines that fill the hook were held by nothing.

### 3.5 The two new checks, shown to fail

A check added beside the capability it guards is worth nothing until something shows it failing.
Each mutation below was applied to this worktree, built, run, and reverted; the tree at the end of
this record is the unmutated one.

| mutation | what it leaves in place | what fails |
|---|---|---|
| the hook assignment in the end-user root wrapped in a condition nothing satisfies | project reference, register cells, graph manifest, closure evidence, and the text of the install | `--runtime` answers `native-arming=none`; `run-cli-acceptance.py` reports `1 of 227 command lines FAILED` on the row that reads the value |
| the conformance root's `InitializeNativeMapping` emptied, so the root names the arming assembly nowhere | project reference, register cells, graph manifest, closure evidence | `K5_Each_Register_Row_Declares_What_Its_Image_Can_Arm` fails, naming `Broiler.VM.Composition.JavaScript.Conformance` |

The first mutation is the one the review performed, and it is the reason the end-user root is held
by a run rather than by a source reading: `K5`'s clause reads whether the root's source names the
arming assembly's page type, which that mutation leaves true. **The clause catches the shape that
actually happened - a root that names the assembly nowhere - and not a deliberate neutering.** The
rule register's row says so in those words rather than leaving a reader to find the limit.

**The conformance root has the source reading and not the run.** Nothing in the gate instantiates a
native artifact in that host; what does is the two-form comparison in section 3.1, which is a lane
step rather than a gate step. Closing that is in the follow-up list and was not done here.

## 4. Retained closure evidence: **corrected, not regenerated**

`src/Broiler.VM.Profile.JavaScript/docs/evidence/js-3a-004/closure-conformance.txt` and
`src/Broiler.VM.Profile.JavaScript/docs/evidence/js-3b-001/closure-cli.txt` each moved from
`7 non-framework assemblies` to `8` in both the `[jit]` and the `[trimmed]` block, with
`Broiler.VM.Profile.MachineCode` inserted in sort order. The `[aot]` block reads
`0 non-framework assemblies` in both and is untouched.

**Corrected by hand, which is what this repository does for this case.** `7bfe561` did exactly this
to `js-1/closure-slicecompiler.txt` when it gave the slice-compiler root the same edge. Regenerating
would mean re-running `eng/collect-js-evidence.py`, which rewrites the whole bundle - `identity.txt`,
`hashes.txt`, `build.log`, `suite.log`, the assurance logs and every other artefact - and that is a
bundle recollection, an act belonging to a milestone rather than to a wiring change.

**The corrected lines are a reading and not a guess.** Both roots were published in both modes the
file records, into `D:/wt/native-arming-logs/publish/SLUG/MODE`, with the exact arguments
`compositions()` uses:

    dotnet publish PROJECT -c Release -o OUT -r win-x64 --self-contained false -p:PublishTrimmed=false
    dotnet publish PROJECT -c Release -o OUT -r win-x64 --self-contained true

and the published directories were then read with `closure_of` from `eng/collect-js-evidence.py`.
All four answered the eight assemblies now written in the files; in particular the trimmed publish
keeps `Broiler.VM.Profile.MachineCode`, because the static constructor roots it.

**No `catalog-*.txt` moved.** A catalog table names composed profiles and the lowering, not sibling
assemblies; `--closure` on both published binaries confirms `Broiler.VM.Profile.MachineCode` appears
in neither, and `7bfe561` left `catalog-slicecompiler.txt` alone for the same reason.

## 5. Scope: the roots that were left alone

`Broiler.VM.Composition.JavaScript.ExecutionOnly` and
`Broiler.VM.Composition.JavaScript.Android` read `none` today and keep it, and so does
`Broiler.VM.Composition.PolyglotCli`.

**They are left out for one reason and it is not that they are a different case.** An earlier draft
of this record, of the register correction and of the ADR revision said the first two had never
composed an arming path, that their reference sets named the profile and not the lowering, and that
the register had read `none` for them since they were written. That is false, and it was checked
against the commit rather than argued: `git show db57c2f^:docs/compositions.md` reads `x86-64` in
the native-execution cell of all six rows naming `Broiler.VM.Profile.JavaScript` -
`ExecutionOnly`, `SliceCompiler`, `Android`, `Conformance`, `Cli` and `PolyglotCli`. `git show
--stat db57c2f` deletes `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` and moves its two
platform halves into `Broiler.VM.Profile.MachineCode`, so before that commit the type that mapped
the page lived in the profile and linking the profile WAS arming. **All five roots that lost the
cell lost the same thing on the same day, and all five are restoration cases.**

The three still reading `none` are left because the owner named two roots and nobody asked for the
other three. A register row moved on nobody's request is the quiet edit that column exists to
prevent. Whoever picks them up should read it as restoring what they had rather than as granting
something new - which is what the false version of this paragraph would have told them.

**Does the retained corpus want it?** No. `src/tests/corpus/js-1` holds five entries naming the
native surface. Four are verifier refusals - `wide-a-native-payload-no-surface-declares`,
`wide-a-native-code-section-that-disagrees-with-itself`,
`wide-the-native-surface-a-composition-declined` and `wide-a-native-payload-of-four-zero-bytes` -
which never reach instantiation and so never reach a page. The fifth,
`wide-a-native-payload-for-an-architecture-no-host-arms`, declares
`ProfileFault/UnsatisfiedHostAssumption` at instantiation and carries an `arm64` payload -
`0xD65F03C0`, an A64 `ret` - chosen precisely because `JsNativeExecution.HostArchitecture` never
answers `Arm64`, so it is refused before a page is mapped on every machine this component runs on
(`WideCorpus.cs`, the comment above the row, which says so in those words). Its verdict is therefore
the same in a root that can arm and in one that cannot. **No entry of `js-1` would behave
differently in an ExecutionOnly that linked the arming path**, and the 22 checks pass unchanged
either way.

**What section 1 of this record says about `db57c2f` was right and stays.** The earlier draft's
error was not in the account of the regression; it was in the justification invented for leaving
three roots out of the repair, which turned "nobody asked" into "they never had it".

## 6. The numeric branch in `PolyglotCli/JavaScriptLane.cs`, checked and not changed

That branch treats `UnsatisfiedHostAssumption` on the numeric manifest as expected and reports
`Unroutable` with a sentence blaming the calling convention. **It still describes something that
happens**, because `PolyglotCli` is not wired here: run after this change,

    PolyglotCli.exe run --numeric --native x86-64-win64 --fuel 2000000000 --wall 60000 \
      src/tests/forms/loop-integer.js

still answers `the artifact verified and this image will not arm it:
(ProfileFault/UnsatisfiedHostAssumption)`, exactly as before.

**What is wrong with it is the sentence, and it was wrong before this change rather than because of
it.** The text tells the caller that the arming path "answers with the one x86-64 calling convention
this process's own platform uses ... and refuses an artifact emitted for the other one". In this
image there is no arming path to answer anything: the refusal is the unfilled
`JsNativePage.Mapper`, and `x86-64-win64` on a Windows x64 process - the convention this text says
WOULD be armed - is refused with the same words. What it should say, while that row reads `none`, is
that this image links no assembly that can map a page and so arms nothing at all, and name the
convention rule only as what would apply in an image that did.

**Not changed here.** No test covers that string - `RunStatus.Unroutable` has no assertion over its
detail text anywhere in the tree - and the honest repair is either that rewording or wiring
`PolyglotCli`, which is a register row nobody asked to move. An uncovered reword is how the wrong
sentence survived five days in the first place, so it is recorded rather than edited, **and it
leaves this change as a named follow-up on the pull request rather than only as a paragraph in an
evidence record nobody is obliged to read.**

## 7. What was not run

- **Native AOT.** No `-p:PublishAot=true` publish of either root. The retained closure files' `[aot]`
  blocks read `0 non-framework assemblies` and are unchanged, so nothing in this change claims
  anything about the Native AOT image. **The register correction and the ADR revision no longer
  assert the refusal as an observation across platforms and publish modes.** Both now say what it
  is: the refusal was reproduced on `win-x64` under the JIT, and it follows everywhere else because
  a static hook nobody fills is unfilled in every image - an inference from the wiring, marked as
  one. Their earlier wording stated it flatly, which this section contradicted.
- **Any platform but this one.** Windows 11, `win-x64`, `x86-64-win64` only. No Linux, no macOS, no
  `arm64`, no Android head. The System V half of the arming path was not exercised.
- **The whole pinned suite.** The native comparison covers the lane's two subtrees -
  `test/language/statements/class` and `test/built-ins/Promise` - and is `coverage partial` in its
  own report header. It is not a whole-suite figure and must not be retained as one.
- **The sharded lane run** and every other lane step, including the ones that publish, and the
  `--host-surface` matrix beyond the single local run in section 3.4.
- **Assurance artefacts.** `CODE-ASSURANCE.md`, `HUMAN_REVIEW.md` and `assurance.manifest.json` were
  not regenerated and are excluded from the patch; rule J5's write run belongs to the merge. Both
  new units carry `Fingerprint=TBF` and `Broiler-Human: PENDING` and neither has been read by a
  human. Note that `AssuranceSources.CoveredAssemblies` does not list composition roots, so the
  scanner does not see either new unit; the annotations are there because the neighbouring units in
  `Cli/Program.cs` carry them and because the next widening of that list should not find a gap.
- **Any measurement.** No timing of any kind was taken, and nothing here says anything about what
  the native form costs or saves.
