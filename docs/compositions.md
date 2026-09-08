# The composition register

**Owner:** Broiler.VM architecture and developer-experience owner
**Core contract version:** 1
**Milestone:** VM-3
**Closes:** Exclusion EX-08 of ADR 0001

This is the register ADR 0001 names and defers to VM-3: the schema for a
composition, the advertised set, the demonstration set, and the runtime
identifiers each composition has actually been published and run for.

It exists because the composition-root allow-list rule A11 reads was an empty
constant inside the architecture-test project. An empty constant is maximally
strict and completely unreviewable: relaxing it is a code change nobody outside
the test project sees. From VM-3 the allow-list is a path - every project under
`src/compositions/` - and what each of those projects is allowed to contain is
this document, held to the checkout by rules K1 and K2 in both directions.

---

## 1. The advertised set is empty

**At core contract version 1, Broiler.VM advertises no composition and ships no
language profile.**

That sentence is the whole of the support position and it is not a placeholder.
An advertised composition is one a consumer may take a dependency on: it has a
package identity, a supported RID set, a compatibility promise across core
contract versions, and a named owner who answers for it. None of those exist,
and publishing a composition before they do would be the untruthful support
claim roadmap section 16 makes a stop condition.

Consequences that follow from the set being empty, rather than being separate
decisions:

- Every composition root in the checkout is **non-packable**. ADR 0001 requires
  it of any root this register does not list as advertised, and this register
  lists none.
- The packable set stays exactly the three product packages:
  `Broiler.VM.Abstractions`, `Broiler.VM.Binary`, `Broiler.VM.Runtime`.
- No RID in section 4 is a supported RID. It is a record of what was published
  and run, on one machine, once. Claiming a RID is a release act and this
  document performs none.

---

## 2. The schema

A row in the composition table declares, in this order:

| Field | Meaning |
|---|---|
| Composition | The assembly name of the composition root. Its project lives at `src/compositions/<name>/<name>.csproj`. |
| Kind | `advertised` or `demonstration`. An advertised composition is packable and carries a support promise; a demonstration composition is neither and exists to be published and run as evidence. |
| Profiles | The profile IDs the composition registers, in the order its catalog builder names them. Exactly these, and no others. |
| Profile assemblies | The assemblies those profiles come from. This is what a closure report must contain beyond the three core assemblies, and nothing else. |
| Host capabilities | The capability IDs the root registers, with whether the composed profiles import them. |
| Guest-initiated loads | Whether the root registers an artifact provider. A composition that registers none refuses every guest-initiated load deterministically, which is what a content policy is. |
| Sibling assemblies | Assemblies the root links that are neither core, nor the root, nor a profile - a profile's own siblings, of which a lowering is the first. `none` where there are none. |
| Evidence | The bundle directory holding this composition's retained catalog table and closure report. Rules K3 and K4 read it. |
| Native execution | Whether the composition may map artifact bytes executable: `none`, or the architectures it arms. A composition declaring `none` must be UNABLE to, not merely not doing it. |

**A sibling is in the image and need not be in the project file.** The format
assembly arrives transitively through the profile and appears in no composition
root's references, while being unmistakably in every published closure - so rule
K2 requires every REFERENCED assembly to be declared and every declared PROFILE
to be referenced, and does not require a declared sibling to be. The execution-only
root's sibling cell therefore names the format and not the lowering, which is the
distinction the whole label rests on.

**The sibling column was added on 2026-08-31 because the schema had no place for
a lowering.** A profile's lowering is in the closure of a compiler-bearing root
and comes from no profile at all, so before this column the only way to make such
a root pass rule K2 was to declare the lowering in the Profile-assemblies cell -
as though a profile came from it - which would then have made the catalog table
and the register disagree. The two claims are now separate: what a profile comes
from, and what else is in the image. **The execution-only root's cell reads
`none`, and that is the whole of the execution-only label.**

**The evidence column was added on 2026-08-31 and it is not decoration.** K3 and
K4 used to read one bundle - the core's current one - which was right while every
composition belonged to the core. This repository now holds two milestone series,
and the JavaScript profile's roots keep their evidence in the profile's own
bundle tree; a rule that read the core's bundle for them would either fail or,
worse, compare a JavaScript closure against a file nobody wrote for it. Naming
the bundle per row keeps the two ledgers apart while one rule still holds every
composition to its own evidence. **A core milestone bump has to move the two
`docs/evidence/vm-6` cells**, and that is deliberate: the register is a reviewed
document, and a cell that moved without anyone reading it is what a literal
inside a test would have been.

**The native-execution column was added on 2026-09-07, and it is the first column
in this table that declares a permission rather than an inventory.** Every other
field answers "what is in the image": which profiles, which assemblies, which
capabilities, which bundle. This one answers "what may this image do to a page of
memory", and the two are not the same question. A root that links an arming path
and never calls it still declares the architectures it may arm, because a closure
report cannot see a call, and a column that described calls would be a field
nothing could check - which section 3 already gives as the reason a root's
contents beyond its label are prose in that section rather than a column here.

**`none` is a claim of incapability and not of restraint.** A composition
declaring `none` must be unable to map artifact bytes executable, not merely not
doing it today, and "unable" is a property of the closure rather than of the code
path a reader followed. That is the strictest reading of the cell and it is the
intended one: the weaker reading - that the root has no current caller - would
make every `none` in this table a statement about somebody's reading of somebody
else's control flow.

**Why the column exists at all, in the sentence the roadmap uses for it.** The
core's closing stop condition was narrowed on 2026-09-07 from "a product closure
reaches dynamic code" to "reaches UNDECLARED dynamic code", and the old clause was
not wrong: it bought a property, and the narrowing spends it in exchange for a
declaration. What replaces an absolute is not a weaker rule but a rule with an
enforcement problem - **an absolute prohibition needs no allowlist and admits no
mistake in one, and a declaration needs both.** This register is where the
declaration lives, so **this register is now load-bearing in a way it was not**:
before 2026-09-07 a mistake in it could make a closure report disagree with a
project file, and from 2026-09-07 a mistake in it is the difference between a
permission somebody granted and a permission nobody noticed.

**And the honest half of that, which said until 2026-09-07 that no automated gate
in this checkout read this column.** It was true when it was written, and it is
what the paragraph below records as having cost this register its first false
cells. **It is no longer true.** Rule B5's scope widened from the core assemblies
to every assembly a published image can contain and its member list gained the
native-code-preparation half; a companion rule reads the platform-invoke
declarations that no member reference names, and permits exactly one assembly of
this repository to declare a mapping or protection entry point; a third pins that
arming path as the one place in the shipping tree that names such an API and
refuses any protection admitting a write and an execute at once; and a fourth
reads this column against the tree in both directions - a `none` over an image
that can arm, and an architecture over an image that cannot. Each carries a
witness that is watched failing. **So a cell here is now a claim a rule checks**,
and the sentence a reader most needs is the one that has not changed: **a rule
reading a column is not evidence that anything in it was reviewed**, no bundle
retains any of this, and the human review [`docs/mvp.md`](mvp.md) defers is
deferred still.

Two things the schema deliberately does not have. There is no "profiles
available but not registered" column, because composition as a runtime option
that disables a linked profile is forbidden outright: a disabled-but-linked
profile appears in the closure anyway and makes every closure report untruthful.
And there is no aggregate profile-listing type anywhere - no `AllProfiles`, no
`KnownProfiles` - because one would reference every profile assembly and defeat
the exact closure this register exists to describe.

---

## 3. The compositions

| Composition | Kind | Profiles | Profile assemblies | Sibling assemblies | Host capabilities | Guest-initiated loads | Evidence | Native execution |
|---|---|---|---|---|---|---|---|---|
| `Broiler.VM.Composition.Calculator` | demonstration | `com.example.calculator` | `Com.Example.Calculator` | none | `com.example.host.unreachable` (imported by no composed profile) | none registered | `docs/evidence/vm-6` | none |
| `Broiler.VM.Composition.Workbench` | demonstration | `com.example.calculator`, `com.example.ledger` | `Com.Example.Calculator`, `Com.Example.Ledger` | none | `com.example.ledger.stamp` (optional import of `com.example.ledger`; the calculator imports nothing) | none registered | `docs/evidence/vm-6` | none |
| `Broiler.VM.Composition.JavaScript.ExecutionOnly` | demonstration | `broiler.javascript` | `Broiler.VM.Profile.JavaScript` | `Broiler.VM.Profile.JavaScript.Format` | `broiler.javascript.write` (optional import of `broiler.javascript`) | none registered | `src/Broiler.VM.Profile.JavaScript/docs/evidence/js-1` | `x86-64` |
| `Broiler.VM.Composition.JavaScript.SliceCompiler` | demonstration | `broiler.javascript` | `Broiler.VM.Profile.JavaScript` | `Broiler.VM.Profile.JavaScript.Format`, `Broiler.VM.Profile.JavaScript.Compiler` | `broiler.javascript.write` (optional import of `broiler.javascript`) | none registered | `src/Broiler.VM.Profile.JavaScript/docs/evidence/js-1` | `x86-64` |
| `Broiler.VM.Composition.JavaScript.Android` | demonstration | `broiler.javascript` | `Broiler.VM.Profile.JavaScript` | `Broiler.VM.Profile.JavaScript.Format` | `broiler.javascript.write` (optional import of `broiler.javascript`) | none registered | `src/Broiler.VM.Profile.JavaScript/docs/evidence/js-android-001` | `x86-64` |
| `Broiler.VM.Composition.JavaScript.Conformance` | demonstration | `broiler.javascript` | `Broiler.VM.Profile.JavaScript` | `Broiler.VM.Profile.JavaScript.Format`, `Broiler.VM.Profile.JavaScript.Compiler` | `broiler.javascript.write` (optional import of `broiler.javascript`) | `broiler.javascript.write` | `src/Broiler.VM.Profile.JavaScript/docs/evidence/js-3a-004` | `x86-64` |
| `Broiler.VM.Composition.JavaScript.Cli` | demonstration | `broiler.javascript` | `Broiler.VM.Profile.JavaScript` | `Broiler.VM.Profile.JavaScript.Format`, `Broiler.VM.Profile.JavaScript.Compiler` | `broiler.javascript.write`, `broiler.javascript.source-provider` (both optional imports of `broiler.javascript`) | `broiler.javascript.source-provider`, answered by this root's own compiler | `src/Broiler.VM.Profile.JavaScript/docs/evidence/js-3b-001` | `x86-64` |
| `Broiler.VM.Composition.WebAssembly.Execution` | demonstration | `broiler.webassembly` | `Broiler.VM.Profile.WebAssembly` | none | none registered | none registered | `src/Broiler.VM.Profile.WebAssembly/docs/evidence/wa-0-001` | none |
| `Broiler.VM.Composition.WebAssembly.Harness` | demonstration | `broiler.webassembly` | `Broiler.VM.Profile.WebAssembly` | none | none registered | none registered | `src/Broiler.VM.Profile.WebAssembly/docs/evidence/wa-0-001` | none |
| `Broiler.VM.Composition.PolyglotCli` | demonstration | `broiler.javascript`, `broiler.webassembly` | `Broiler.VM.Profile.JavaScript`, `Broiler.VM.Profile.WebAssembly` | `Broiler.VM.Profile.JavaScript.Format`, `Broiler.VM.Profile.JavaScript.Compiler` | `broiler.javascript.write`, `broiler.javascript.resolve`, `broiler.javascript.source-provider` (all optional imports of `broiler.javascript`; the WebAssembly profile imports nothing, and the three are registered only for a JavaScript run) | `broiler.javascript.source-provider`, answered by this root's own compiler | `docs/evidence/vm-7-cli-001` | `x86-64` |

**Four cells read `none` and six read an architecture, and the five that changed
are recorded rather than edited quietly** *(corrected 2026-09-07, and this is the
first correction this column has needed; the count moved from five to six later the
same day, when a tenth row was added, and the paragraph below is that row's)*.

**What the cells said.** All nine read `none`, and the paragraph that stood here
said that none of them was arrived at by inspection: no project in either
solution named a native-memory API, so the cells were `none` because there was
nothing in the graph this register governs that could be anything else. That
argument was sound and its conclusion has expired. What is unchanged is the one
component in this repository that has always named such an API -
`src/Broiler.VM.HyperV/`, which `ComponentGraph.VendoredComponents` places
outside every rule in group A - which composes nothing here and maps its host
pages `PAGE_READWRITE` only; its execute bit is a guest permission inside a
hardware partition and not a host page protection.

**When the cells went false, which is not the day anybody noticed.** They went
false the moment `JsNativePage` landed in `Broiler.VM.Profile.JavaScript` - the
one type in this repository that reserves a mapping, writes it, and then arms it
readable-and-executable - and not on the day this paragraph was rewritten. There
is no interval in which the old cells were still true and merely stale: a cell
declaring a permission is false as soon as the closure it describes can exercise
one, and the register carried five false declarations from the change that landed
that type until the change carrying this paragraph.

**What they say now, read off the reference graph rather than assumed.** Each of
the five JavaScript roots names
`..\..\Broiler.VM.Profile.JavaScript\Broiler.VM.Profile.JavaScript.csproj` in
its own project file, so each has that assembly - and therefore that type - in
its closure, and each declares `x86-64`.

**Why `x86-64` and nothing beside it, when the profile carries encoders for
two architectures.** The architecture a root may arm is the one its arming path
will accept, and that path admits exactly two calling conventions of one
architecture - Windows x64 and System V x64 - answering `None` for every other
process architecture. An artifact emitted for arm64 is therefore refused where a
handle would be instantiated, deterministically and by name, rather than run. So
the emitting side of this family names three backends and the arming side names
one architecture, and this column is the arming side.

**The Android head was checked separately and its cell is the same for a
different reason.** It targets `net10.0-android36.0` and publishes
`android-arm64` and `android-x64`. On `android-x64` the process architecture is
x64 and the Unix half of the arming type carries Linux's mapping constants, which
is the kernel Android has, so that image can arm exactly what its siblings can.
On `android-arm64` nothing can be armed at all - not because the root differs but
because the profile's own host-architecture test answers `None` off x64. A cell
is a property of a composition and not of one of its runtime identifiers, so the
cell reads `x86-64` and this sentence is where the per-identifier half of it
lives.

**No caller is claimed by any of the five, and the column does not ask for one.**
The execution-only root names nothing native in its own source; it declares
`x86-64` because its closure can arm one, which is the strict reading section 2
fixes and the only reading a closure report can check. A cell here is a
permission and never a report of a call.

**A sixth row declares `x86-64` from 2026-09-07, and it is the first row to do so
after a rule read this column rather than before.** The five above carried a false
cell for a whole change and were corrected by an editor; this one was written against
a rule that fails a build, which is a different act even though the cell says the same
word. `Broiler.VM.Composition.PolyglotCli` names the JavaScript profile in its own
project file, so the arming type is in its closure, and the sentence the five above
needed - that a cell is a permission and never a report of a call - applies to it
unchanged. What is new about the row is on the other side of the table: it is the
first row here whose Profiles cell holds two PRODUCT profiles, so it is the first row
where the arming permission belongs to ONE of two composed profiles. The WebAssembly
profile in that image can arm nothing, and the cell still reads `x86-64`, because a
cell is a property of a composition rather than of a profile inside one. A column that
answered per profile would be a different column and it would not be checkable against
a closure report.

**The two WebAssembly rows keep `none`, and it is now a checked fact rather than
an inherited one.** That profile's Broiler-owned reference set is
`Broiler.VM.Abstractions` and `Broiler.VM.Binary` and nothing else, and a search
of those two assemblies and of the profile's own sources for the platform's
reserve, protect and map entry points returns nothing at all. So `none` there is
the claim of incapability section 2 requires and not a claim of restraint, and
the two demonstration rows of the core's own fixtures keep theirs on the same
ground.

**This column was added to prevent an untruthful declaration and carried one
before any rule read it, which is recorded here as its own first finding.** For
the interval between the arming path landing and this correction, five cells
stated a permission nobody granted, and nothing failed while they did - because
in that interval the column was, in section 2's own words, a claim a reader
checks and a rule does not. **A rule reads it now.** The VM-7 action that widens
rule B5 to the mapping and arming surface has been taken, and with it a rule that
holds every cell of this column against the tree in both directions: a `none`
over an image containing an assembly that can map memory executable, and an
architecture over an image containing none. **So this correction is the last one
this column can receive from an editor alone** - from here a wrong cell fails a
build rather than waiting to be noticed - and the episode is recorded rather than
quietly overtaken, because what a reader should weigh is that the gap was real,
was open for a whole change, and closed by somebody writing the rule rather than
by anybody noticing the cells.

**The two WebAssembly rows composed a verifier when they were written and now
compose an interpreter as well** *(corrected 2026-09-07)*. This paragraph said
that the profile carried a decoder and a validator and carried no interpreter,
and that its executor refused every step including a step on a handle that
profile really did mint. That was true of the rows on the day they were added and
it is not true now: the profile instantiates a module, runs it, and answers a
completed step carrying results, and the harness root drives that loop from
catalog to invocation. **What has not changed is that neither row is a support
claim of any kind** - no bundle covers the interpreter, nothing has been published
or run on a claimed runtime identifier since it landed, and that profile's own
ledger is the authority for both. And the `execution-only` label on the first of
them is narrower than it is for a compiling profile for the reason it always was:
that image cannot turn source into an artifact because no image of that profile
can - there is no lowering in the component at all - and not because this root
declined to link a compiler.

**The harness row is the placement decision, and now the code that needed it.** A
binary corpus encoder writes the bytes a verifier is then asked to refuse, so it
has to name the profile assembly; rule A11 forbids a project outside
`src/compositions/` to name one; so the encoder is a composition root and never a
test project. It holds an encoder written by hand and a corpus of entries pinning
the triple each one produces; what it does not hold is the retained corpus the
gates ask for, with a hash beside every entry and a replay under three publish
modes. It is a `demonstration` and it
is never advertised, and it has the execution root's reference set exactly - the
two roots publish identical closures and differ only in this register.

**What the two bundles retain, and what they do not.** Each row's bundle holds
the catalog table its published binary printed and a closure report over a
framework-dependent publish. It holds no trimmed and no Native AOT closure, and
the closure files say why in their own headers rather than leaving it to be read
off an absence: a trimmed publish of a build with no decoder drops
`Broiler.VM.Binary`, because nothing in it calls into that assembly, and
retaining that as this composition's closure would be recording a true fact about
a shell as a claim about the composition. Those two modes belong to the milestone
that lands a decoder. **A decoder has since landed and the bundles have not been
collected again**, so what those two rows retain still describes the shell they
were collected from, and the trimmed and Native AOT closures of a build that does
call into `Broiler.VM.Binary` are owed rather than held.

**The Android head composes exactly what the execution-only root composes**, and
that is the point of it rather than an accident: it names the profile and not the
lowering, so the execution-only property travels onto a device unchanged. What
differs is the target framework, which is the only way an Android RID can be
published at all, and the checks it runs - the corpus replay and the ordering
assertions, compiled from the execution-only root's own source rather than
re-implemented. It runs neither the soak nor the fuzz sessions: those are
wall-clock and heap-shaped, and an emulator is neither a machine nor a stable
one. **Its evidence is a collection taken on an emulator**, which the bundle's
own exclusions say in those words: an emulator is not a device, and the RID that
ran is `android-x64` because that is the emulator's architecture.

**Why the single-profile root registers a capability nothing imports.** It is
the demonstration that registering a capability never implies a provider. The
calculator declares no import, so its binding table has zero slots and the
registered handler is unreachable from the guest whatever the host does. A host
that had to curate its registrations per composed profile would be doing the
core's containment work by hand, and getting it wrong would be silent.

**Why the two-profile root states three ceilings explicitly.** Adopting the
profile default resolves to the *tightest* default in the catalog. That is a
catalog-wide fact rather than a per-profile one, so the calculator's numbers
would otherwise decide what the ledger may do: the calculator's defaults write
`1` for section count and `0` for host calls, the ledger frames two sections and
imports a stamping capability, and a runtime adopting throughout would refuse its
artifact for want of a second section. The root therefore states `HostCalls`,
`SectionCount` and `StructuralDepth` itself. This is not a way around the
calculator's limits - the effective ceiling for an operation is the intersection
of the host's with **that profile's own** hard maxima, so the calculator is still
held to one section and no host call - and the profile-authoring consequence is
recorded in section 5.

**The conformance harness is the one row in this table whose non-advertisement is
a rule rather than a consequence.** Every other root here is a demonstration
because section 1's advertised set is empty; this one would have to stay out of
that set even if the set filled. It is the ingestion path for a conformance
suite — separately licensed third-party material that a human has still not
retrieved, hashed or archived — and roadmap section 14 asks the property to be
asserted rather than assumed. Rule N13 is that assertion, and it is deliberately
**not** phrased as "appears in no published closure": this root publishes a
closure of its own, for its own evidence, so that phrasing would be falsified by
the very bundle the row's Evidence column names. What N13 asserts is that the
harness appears in **no package and in no advertised composition's closure**,
that no other project references it, and that no project file names a suite
directory — the last being how suite *files* would reach a build output with the
dependency graph still looking clean. Its negative control adds the reference
from the execution-only root, which is the direction that would actually ship.

**It carries a lowering, and that is forced rather than chosen.** Scoring a
conformance test means lowering its source, verifying the artifact and running
it, so this root's reference set is the slice-compiler root's. It composes no
second profile and registers no capability: what it adds beyond the image is a
suite reader, a selection pipeline, a self-check, a merge and a ratchet, all of
which are in the closure it publishes for the same reason the corpus replay is in
the execution-only root's.

**Why the two JavaScript roots are two projects and not two modes of one.** They
differ by exactly one reference - the lowering - and that difference is the whole
of the `execution-only` composition label. The execution-only root names the
profile and not the compiler, so it cannot turn source into an artifact however
it is invoked, and every artifact it runs is precompiled and read as bytes from
the retained corpus. The slice-compiler root names both, lowers the slice
programs and writes that corpus. A flag on one binary would have made the
difference a run-time choice inside one closure, and a closure report cannot see
a flag.

**Neither of the two ORIGINAL JavaScript roots is `narrow-runtime-compiler`, and
the slice-compiler root is only shaped like one.** That label belongs to a
composition carrying a lowering for a named restricted SOURCE surface, and there
was no source surface until JS-3b wrote the tokenizer and the static semantics.
What that root lowers is a programmatic builder, so it is recorded here as a
demonstration and claims no label. **JS-3b claims the label with a publish-and-run
gate of its own, and the row that holds it is the CLI root added on 2026-09-03**;
the paragraphs at the end of this section are that row's.
**The `narrow-runtime-compiler` label is claimed, and this row is the one that
claims it.** The paragraph below said until 2026-09-03 that no root here held the
label and that JS-3b would claim it with a publish-and-run gate of its own. That
is what this row is. The label belongs to a composition carrying the tokenizer,
the static semantics and the lowering for a named restricted **source** surface,
and what was missing after JS-3b wrote those three was a composition handed
source from *outside* the image — a file a person names on a command line. The
slice-compiler root lowers a programmatic builder and the conformance root lowers
a fixture tree this repository also wrote; both print
`narrow-runtime-compiler-shaped` in their catalog table, and this root is the
first to print the label itself.

**It is the end-user host, and it is a demonstration anyway.** Point it at a
`.js` file and it compiles, verifies and runs it, printing the completion value;
name several files and it runs them as separate scripts sharing one realm, in
order; point it at a directory and it sweeps every `.js` file under it, a realm
each, and prints the distribution. **From 2026-09-04 it lowers
`broiler.javascript.wide` by default and keeps the slice behind `--slice`**, and
what that changes about this row is one sentence and not the Kind column. The
sentence that changed: pointed at the Octane benchmark it used to refuse every
file, and it now runs several of them and prints a score.

**The Kind column did not change, and that is the part worth reading rather than
skimming.** **A tool advertised as a JavaScript host has to be able to run
JavaScript**, and this one runs a great deal of it. **BigInt is the only
construct this paragraph denies of the default manifest
`broiler.javascript.wide`**, and the qualifier is meant: a BigInt literal is
refused at compile time, by name, as
`2104:ConstructOutsideManifest at 1:14: a BigInt literal is not admitted by the
declared feature manifest`, and the value kind is implemented nowhere. Class,
generator, `async` function, module, destructuring, spread, template literal,
`for … of`, Proxy, Symbol, typed array, `eval` and the `Function` constructor
**all run to completion on this root's built binary**, and a reader should point
it at each of them rather than take the list from here. **What this paragraph
does not do is enumerate the manifest**: library surfaces the wide realm lacks —
`Intl`, `Temporal`, `SharedArrayBuffer` and `Atomics` among them — are absences
rather than denials this register has checked, and
`src/Broiler.VM.Profile.JavaScript/docs/roadmap.parity.md` is where they are
enumerated against a comparison engine. Nothing in it has been
read by a human, and it has no conformance run of its own over the pinned suite.
Advertising it would be the untruthful support claim the core roadmap makes a
stop condition, and section 1's advertised set stays empty.

**Corrected 2026-09-08, and the correction is of an UNDERSTATEMENT, which this
register treats as exactly the same defect as an overstatement.** The paragraph
above read, until this date:

> `broiler.javascript.wide` admits no class, generator, `async` function, module,
> destructuring, spread, template literal, `for … of`, Proxy, Symbol, BigInt,
> typed array, `eval` or `Function` constructor; its regular expressions are
> translated to the platform's engine and are declared an approximation where
> that is done

**Thirteen of those fourteen constructs run**, which was checked by running each
one through this root's published command line on the day of this correction and
not by reading any source; the shipped `--help` text of both hosts was corrected
the same day and this register did not follow it until now. **And the clause
about regular expressions was stale in the same direction**: the translation onto
the platform's engine was replaced by a matcher this profile owns, and rule N18
asserts over the profile's product assemblies that none of them constructs a
platform matcher at all — so the clause described an approximation the checkout
had already stopped making.

**BigInt is the survivor of that list, and what changed about it today ran the
other way.** Until 2026-09-08 the wide front end **admitted** a BigInt literal
and evaluated it as a Number: `typeof 1n` answered `"number"`, `1n === 1` was
`true`, and `9007199254740993n` answered `9007199254740992` — a silently wrong
integer, which the `--numeric --native x86-64-win64` path then emitted machine
code to return. That is the finding
`src/Broiler.VM.Profile.JavaScript/docs/roadmap.parity.md` section 4.2 records
under the heading **"The refusal that was lost"**, calling it "the one finding in
this document that breaks a property the profile has rather than missing one it
never had". **The refusal is restored and the value kind is not implemented**:
the wide parser now notices the `n` suffix the tokenizer had already left on the
token for a parser to notice, and answers with the diagnostic quoted above. **So
the one word of the old reading that was not an understatement was still not
right**: `BigInt` in that list read as an absence a program would meet, and what
a program met until today was a wrong number. A denial list is a promise about
what happens to a program, not a list of words, and that is why this register
quotes the diagnostic rather than naming the construct.

**None of this is an argument for advertising, and the Kind column is untouched.**
This row reads `demonstration` because nothing in it has been reviewed by a
person and section 1's advertised set is empty; an advertised composition needs a
package identity, a supported RID set, a compatibility promise and a named owner
who answers for it, and correcting a denial list supplies none of the four.
**Discovering that a host does more than its own register said is a reason to fix
the register, and never a reason to move the column beside it.**

**And one clause of the old paragraph survives the correction intact, which is
the conformance one — checked rather than carried forward.** This root has no
conformance run of its own over the pinned suite, and the whole-suite run bundle
`jsw-10-001` retains is not a counterexample to that: both of its runs are
`python3 eng/run-test262.py`, which drives
`Broiler.VM.Composition.JavaScript.Conformance` — a different root, with a row of
its own in this section. What that bundle retains *of this root* is the other
workload: its Octane runs go through this host's ordinary command line, which is
why the Octane sentence earlier in this row may say this root prints a score
while this paragraph says it has no conformance run of its own. **Two workloads,
two roots, one bundle**, and a reader who takes the Octane sentence for a
conformance claim has crossed a boundary the bundle itself keeps.

**The figures that bundle states are figures about the run it retains, and one of
its columns moved on 2026-09-08.** The BigInt refusal turns variants that passed
or failed over `test262`'s BigInt subtree into variants that meet a refusal
naming the construct, so the whole-suite `passed` total falls and the wide run's
`unsupported` column stops being empty. **This register states no figure of
either run**: what it states is the bundle's name, the direction of the move, and
that `jsw-10-001`'s own figures describe a run taken before the fix.

*(Corrected 2026-09-08, later the same day, and what is corrected is a
promissory note rather than a wrong fact. The paragraph above ended "**A fresh
whole-suite run is being taken and its totals are not known here**, so this
register states none". The run has since been taken — the whole `tc39/test262`
suite driven under `broiler.javascript.wide` by `python3 eng/run-test262.py`
against the pinned suite revision
`46d54f57ae3a4803c6ebc5f4625dd4b417254ed65058836732f182801e1cfe93`, by the
orchestrator, on `win-x64`, on this date — so its totals are known and the note
is withdrawn. **The clause that survives is the one that mattered**: this
register still states none of them, because a conformance total is the profile
ledger's to state and sharing a repository is not sharing a ledger.

**The direction this paragraph predicted is the direction the run took, and the
column it predicted about is settled rather than merely non-empty.** The wide
manifest's whole-suite `unsupported` column now holds exactly one construct
rather than a family list — the BigInt literal, meeting the refusal that names it
— and the `passed` total falls while the failure total falls further, because a
variant that meets a named refusal is thereafter neither a pass nor a failure.
**The run is retainable and not retained**: it was taken in a working tree rather
than into an evidence tree, no bundle has collected it, nobody has read it, and
no row of this register or of any ledger reaches `Accepted` on it.

**The whole-suite ratchet was re-based by hand for that run, and the fact that
`--admit` could not do it belongs in this register rather than only in the file.**
`src/tests/conformance/floors/test262-wide.floor` re-bases automatically only when
the suite revision or the manifest moved; neither moved here, and what moved was
the **engine**, deliberately — so the ratchet answered `Regressed` and refused,
which is the design working rather than the design failing. The floor's own header
now states in writing that a **lower `passed` row is not a regression in this one
case and why**, the retired rows are kept in the file with their reason, and the
gate is green beside all of it. **None of which advertises anything**: the Kind
column of every row in this section is untouched, and a ratchet re-based by hand
is a smaller claim than a ratchet that never had to be.

**And the clause about this root in particular survives the re-take, checked
rather than carried forward**: the 2026-09-08 run drives
`Broiler.VM.Composition.JavaScript.Conformance`, which is the row above and not
this one, so this root still has no conformance run of its own over the pinned
suite and the re-take gives it none.)*

**Its closure is the one with nothing to explain away, and that is the point of
it.** The paragraph further down records what the other JavaScript roots carry
beyond the image each demonstrates — a corpus replay, ordering assertions, a fuzz
mutator, a soak, a corpus writer, a conformance harness — all forced there by
rules A11 and A12 leaving such code nowhere else, and all of it in the closure
each publishes. **This root carries none of it.** It reads a file, compiles it,
verifies it, runs it, and reports; a reader comparing its closure against section
15's row for the label finds the assemblies the label names and no others. That
property is why the label could not simply have been asserted of a sibling.

**What it registers is two things, and both are content policy rather than
plumbing.** From 2026-09-04 it registers `broiler.javascript.write`, which is how a
program's `print` reaches standard output; the import is optional, so a sibling that
registers nothing composes the same profile, runs the same programs, and has a
`print` that reaches nowhere. From the same date it also registers an **artifact
provider** for `broiler.javascript.source`, which is what lets `eval` and the
`Function` constructor answer at all. That is a decision and not a default: a
person pointing a JavaScript host at a file expects it to evaluate source, and a
sibling root that registers nothing gets the deterministic refusal instead. Both are
correct compositions of the same profile, which is the whole argument for putting the
dynamic surface behind an identity a composition can decline.

**It states its ceilings only when a caller does.** The instruction allowance, the
wall clock, the call depth and the live-memory allowance are each the profile's own
declared default unless a caller passes `--fuel`, `--wall`, `--call-depth` or
`--live-bytes`, because a host with an opinion about how long a program may run — or
how much it may hold — is a host imposing a policy the profile did not declare. The
last two exist because a ceiling nobody can move from outside is a ceiling that
decides what may be measured: `--call-depth` so the per-frame cost can be measured
against the real binary rather than estimated, and `--live-bytes` so a workload whose
working set is larger than a typed program's is a run a caller can ask for rather
than a benchmark that scores and then exhausts.

**Its acceptance suite is input files and not injected code.** `src/tests/cli/`
holds the programs and `eng/run-cli-acceptance.py` drives the built binary over
the command lines the table declares, judging exit codes and output. No source of this
component is patched to make a case fail and no internal type is reached for:
what is under test is the binary a person would run, including its argument
parsing, which of its two streams carries which message, and what it does with a
file that is not UTF-8. The driver takes `--expected` so it can be pointed at a
table of deliberately wrong rows and shown to report the mismatch, because a
driver whose every row passes may not be comparing anything.


**What the two JavaScript roots contain beyond the image each demonstrates,
because a label describes a reference set and not a file inventory.** The
execution-only root carries the retained corpus's replay, the ordering
assertions over every entry of it, the fuzz mutator, the soak over recycled
runtimes and the shared-aggregate-budget exercises. The slice-compiler root
carries the corpus writer and the cross-profile catalog checks with the
neighbour descriptor those need. **All of it is in the closure each root
publishes**, and that is forced rather than chosen: every one of those drives a
profile's own verifier and executor, rule A11 forbids a test project to
reference a profile assembly, and rule A12 forbids a composition root to
reference the fixture assembly. There is nowhere else for them to be, and a
corpus a test project produced would in any case be a corpus the product path
never exercised.

Two consequences a reader should meet here rather than in a closure report.
**Rule K4's "no test assembly" clause is satisfied by the assembly boundary
while the property is weaker than it reads** - no test *assembly* is present,
and a mutator and a soak driver are. And **an advertised composition may not
carry any of this**: section 1's set is empty, both roots are demonstrations,
and a root proposed for advertisement has to answer for its closure separately.
That is one more reason none is advertised today.

**This is prose rather than a ninth column, deliberately.** The columns are the
claims a rule reads back - profiles, assemblies, siblings, capabilities, loads,
evidence - and each has a mechanical counterpart in the checkout that K1 to K4
compare it against. What a root contains beyond its label has no such
counterpart: a scan cannot tell a soak driver from an interpreter, so a column
would be a field nothing could check, sitting in a table where every other field
is checked. Naming it here keeps the distinction visible.

*(The count in that sentence moved on 2026-09-07, when the native-execution column
was added and "a ninth column" became a tenth. The sentence is left as it was
written rather than renumbered, because what it argues does not depend on which
number the column would have had: a claim with no mechanical counterpart does not
belong in this table at whatever position it would occupy. The native-execution
column is not a counter-example to it. That column does have a mechanical
counterpart - whether a published closure can reach an arming path at all - and
section 2 recorded, in the same change that added it, that no rule read that
counterpart yet. One does from 2026-09-07, in both directions, which changes
nothing about the argument: the column belongs here because it has a counterpart,
and it belonged here on the day the counterpart was unread.)*

**The neighbour profile the slice-compiler root composes for its cross-profile
checks is defined inside that root**, not referenced as an assembly, which is why
its Profiles column names one profile. It exists so that a neighbour's maxima and
a neighbour's adopted defaults can be shown to reach different things; putting it
in the execution-only closure would have contradicted the single-profile claim
that closure exists to make.

**The tenth row is the first composition in this repository to compose two PRODUCT
profiles, and roadmap section 14 has been asking for it since VM-3.** That section
asks for a catalog test over two profiles, and section 16 records that a composition
hosting two of them closes no gate until such a test exists. The two-profile
composition that existed until 2026-09-07 is the workbench root, and its two profiles
are `com.example.calculator` and `com.example.ledger` - written by this repository, for
this repository, in order to be composed, and designed beside each other. This row's
two are `broiler.javascript` and `broiler.webassembly`: different payload formats, one
with a lowering and one with none, one with a format sibling and one without, two
diagnostic vocabularies and two execution models, neither written with the other in
mind. **The row does not close that gate**, because a register row is not a test and
this document accepts nothing; what it records is that the composition now exists and
what is in it.

**What composing two of them cost, which is the number worth reading.** Two `Add`
calls on one catalog builder. No change to the core runtime, no change to the
execution loop, no change to either profile, and no aggregate profile listing anywhere
- section 2 forbids one outright, because a type naming every profile would reference
every profile assembly and defeat the closure this register describes.

**What it did cost is a ceiling decision, and it is the one the workbench root already
met with two fixtures.** A runtime that adopts the profile default for a dimension
resolves to the TIGHTEST default in the catalog, which is a catalog-wide fold rather
than a per-profile one. In a two-product-profile image that means one profile's
declared default would silently decide what the other's artifacts may do, and a
program refused for want of allowance would have been refused because of a profile it
never touched. This root therefore states, per dimension, the larger of the two
declared defaults wherever the two differ, and computes it from the two descriptors
rather than writing a number down. **That widens nothing**: an effective ceiling is
still intersected with the hard maxima of the profile an artifact names, so stating a
value buys the tighter profile nothing at all. What it buys is that neither profile's
default decides the other's run.

**It is the end-user tool a person would actually type, and it is a demonstration
anyway.** Point it at a `.js` or `.mjs` file and it compiles, verifies and runs it;
point it at a `.wasm` file and it verifies, instantiates and invokes it; name several
and it runs each on the profile it belongs to, adjacent JavaScript files sharing one
realm and every module its own instance. It has a subcommand grammar - `run`,
`compile`, `closure`, `version`, `help` - because four jobs with four argument shapes
cannot be told apart by a flat option list, and bare `broiler <file>` still runs the
file. **The Kind column is `demonstration` for the reason the JavaScript host's row
gives and for one more.** A tool advertised as a polyglot host claims both surfaces,
and neither claim is available: **BigInt is the only construct this sentence denies
of that JavaScript surface** — a BigInt literal is refused at compile time by name,
with the same `2104:ConstructOutsideManifest` diagnostic the JavaScript host's row
quotes, because both hosts compose the same profile at the same default manifest,
and that row is also where the library surfaces the wide realm merely lacks are
pointed at — and this
WebAssembly surface admits no import, no text format, no vector instruction, no
garbage collection, no exception handling, no thread and no memory64. A module that
declares an import is refused rather than linked. Advertising either would be the
untruthful support claim roadmap section 16 makes a stop condition, and section 1's
advertised set stays empty.

**Corrected 2026-09-08, in the same direction and for the same reason as the
JavaScript host's row.** The JavaScript clause above read, until this date:

> that JavaScript surface admits no async function, class field, private name,
> Proxy or BigInt

**Four of those five run here**, checked on the day of this correction by running
each through this root's own built binary rather than through the host it was copied
from: an `async` function awaits and settles, a class body carries fields and private
names, and a Proxy's traps fire. **The list went stale by being copied**, which is
the hazard the closure paragraph below already names — it was carried forward from
`Broiler.VM.Composition.JavaScript.Cli` and inherited that row's understatement
along with its code, and the shipped `--help` text of both hosts was corrected
earlier the same day while this register was not. **BigInt is the one that survives,
and it survives as a refusal rather than as an absence**: until 2026-09-08 the wide
front end admitted a BigInt literal and evaluated it as a Number, so a program doing
big-integer arithmetic here got a silently wrong number instead of meeting a
denial. The JavaScript host's row records that finding, its source and its repair.

**Correcting an understatement is not an argument for advertising, and neither
column of this row moves.** This root reads `demonstration` because nothing here has
been reviewed by a person and section 1's advertised set is empty — an advertised
composition needs a package identity, a supported RID set, a compatibility promise
and a named owner — and none of those four is supplied by finding out that a surface
is wider than the register said. **The WebAssembly half of the sentence is
independently a bar to advertising and is unchanged**: nothing above was checked
against it and nothing above weakens it.

**Routing is decided twice and refuses rather than guessing.** The extension says
which profile a file claims to be for, and the first four bytes say whether the file
agrees: a `.wasm` file that does not open with the module preamble is refused rather
than decoded, and a `.js` file that does is refused rather than tokenized. Both
directions are checked, because a check in one direction would leave the likelier
mistake - a module saved under the wrong name - reported as a syntax error on line 1.

**Its closure has one thing to explain and this is where it is explained.** Several of
its files are copies. The file reader, the module resolution, the artifact provider
and the JavaScript run loop are copied from `Broiler.VM.Composition.JavaScript.Cli`,
and every copied file says at its top that it is a copy and why. Rule A11 forbids a
project outside `src/compositions/` to reference a profile assembly, and rule A12 does
not admit a root referencing another root, so shared code between two composition
roots has nowhere to live and duplication is the shape those two rules leave. **A
reader should treat the two hosts as one implementation and not as two opinions**:
where they diverge, one of them is wrong.

**What this row's bundle retains and what it does not.** A catalog table and a closure
report from a published binary, in two modes - framework-dependent and trimmed
self-contained - both on `win-x64`, both run, both containing the same eight
assemblies. It retains no Native AOT closure: that publish was attempted, its native
link step failed to find a toolchain on the collecting machine, and the closure file
says so in its own header rather than leaving an absence to be read. It retains no
measurement of anything, no conformance run over either profile's suite, and no
acceptance driver of this host's own - the JavaScript host it was copied from has one
over its built binary, and this root has a transcript, which is weaker because nothing
fails when it changes.

**Re-checked 2026-09-08 against the whole-suite run taken that day, and nothing in this
row moved.** That run drives `Broiler.VM.Composition.JavaScript.Conformance` and no other
root, so "no conformance run over either profile's suite" is true of this row's bundle
after the run exactly as it was before, and it is recorded as checked rather than left
looking uninspected. **The BigInt clause of this row's denial sentence is likewise
unmoved**: the refusal that sentence quotes is the same refusal that accounts for every
variant in the wide run's `unsupported` column, and how many variants that is is the
profile ledger's figure to state and not this register's — so this row names the
construct and states no count, which is the same rule the paragraphs above obey.

*Corrected 2026-08-31, and this paragraph contradicted section 5 until it was.*
It also said a runtime ceiling was "additionally clamped to the tightest profile
hard maximum in the catalog", and called both terms catalog-wide. Only the
default is. The maximum clamp was an implementation defect rather than a property
of the contract - ADR 0007 puts `ProfileMax` at P2, against the profile an
artifact names - and it has been removed. **The reason this root states three
ceilings is unchanged**, because it was always the default fold that produced the
refusal; what changed is that a reader of this section is no longer told the
maxima do it too.

---

## 4. Published and run

Neither row is a supported RID. This is what was actually built and executed
when the VM-3 evidence bundle was collected.

| Composition | RID | JIT | Trimmed, self-contained | Native AOT |
|---|---|---|---|---|
| `Broiler.VM.Composition.Calculator` | `linux-x64` | published and run | published and run | published and run |
| `Broiler.VM.Composition.Workbench` | `linux-x64` | published and run | published and run | published and run |

The transcripts are in `docs/evidence/vm-3/`, and the closure report for each
mode is listed there from the published output rather than described. One RID,
one machine, one lane: exclusion EX-45 of the VM-2 bundle applies unchanged.

---

## 5. What a profile author should take from this

Three things surfaced while composing two unlike profiles that are properties of
the contract rather than of these two profiles, and all three are cheap to get
wrong.

**A hard maximum is a statement about you, and an adopted default is a statement
about your neighbours.** These were one paragraph until 2026-08-31, and they are
two rules with opposite reach.

Your hard maximum binds *your* artifacts and nobody else's. It is applied at
verification, against the profile the artifact names, so declaring a tight one
constrains only what you accept. It used to clamp every runtime ceiling in a
shared catalog - one section because you frame one, zero host calls because you
make none, and every profile beside you held to that - and that was a defect, now
corrected. If you read this register before that correction and loosened your
maxima on its advice, nothing you did is now wrong; it is simply no longer
required.

The *default* you declare is still catalog-wide. A host that adopts profile
defaults rather than stating numbers gets the tightest in the catalog, because at
runtime creation no profile has been selected and there is no other safe answer.
So a stingy default is what now reaches your neighbours, and a host that wants
more states an explicit ceiling - which is what the two-profile composition below
does for the three dimensions where it mattered. Declare a maximum for what you
would tolerate being granted, and a default for what you actually need.

**An optional import is a per-runtime binding, not a property of the profile.**
The same profile, over the same artifact, answers differently in a runtime whose
host registered the capability and one whose host did not - and both are correct
answers. Write the unbound branch first: it is the one a host's policy can force
on you at any time, and `IsBound` is the whole of what you may ask.

**Defaults clamp catalog-wide, and the dimension you never use is the one to
watch.** Since the maxima clamp was removed this is the whole of the catalog-wide
reach, and it is the half that gets missed: adopting a profile default resolves
to the tightest default in the catalog. The fold reads every dimension of every
descriptor with no exemption for the ones a profile declares inapplicable. So a
profile writing `0` into a guest-load **default** because it has no guest loads
hands a host that adopts defaults a ceiling of zero, and the failure surfaces in
somebody else's verifier as a refusal naming a dimension they never touched.

**And there is no costless spelling of "I do not constrain this".**
`VmDescriptorValidation` refuses any descriptor whose `LimitDefaults` carries an
unconstrained slot, with reason `LimitDefaultsInvalid`, because a default meaning
unbounded would make adopting it identical to declaring no ceiling at all -
invariant 9's rule that omission never means unbounded. Hard maxima may use
`Unconstrained`; defaults may not. **So declare a large finite default on a
dimension you do not use, say why in the record, and state plainly that it still
participates in the fold** - the exposure is bounded, not removed. An earlier
version of this paragraph advised declaring `Unconstrained` and was wrong: that
descriptor does not build.

**Prove it rather than asserting it.** The way to show you have not mis-declared
a catalog-wide default is a two-profile catalog test with a deliberately adverse
neighbour - a descriptor that tightens the dimensions you do not declare and
writes a stingy default into one you do not use - asserting that the neighbour's
maxima do not reach your artifacts at all, and that its adopted defaults do. Both
intended first profiles build exactly this at their first milestone. It is a test
shape, not a shared asset: nothing is extracted to run it.

---

## 5a. A composition this register does not yet have

Both intended first profiles are written against this core, and the product that
consumes them first is a browser, which needs **both at once**. No such
composition exists, and this register deliberately has no row for it: rule K1
fails on a row naming a root that does not exist, so an anticipated composition
is recorded here in prose or not at all.

What is worth fixing now is who owes what, because none of it belongs to either
profile:

- **The closure, the RID matrix and the Native AOT evidence are the composing
  component's**, not the union of two profiles' evidence. One profile's roadmap
  calls a browser its *largest* closure because it links a lowering; the other
  calls a browser its *smallest* because it compiles nothing. Both are true of
  themselves and neither describes the image, which is the union plus the core.
- **The two profiles reach each other through their declared *defaults***, per
  the paragraph above, on all fifteen dimensions - and through their maxima not
  at all, since that clamp was removed on 2026-08-31. Reconciling two
  independently owned sets of defaults is the composing component's job and
  nothing in either profile can do it. A host that states explicit ceilings never
  meets the fold; a host that adopts defaults always does.
- **A call chain that crosses runtimes is bounded only under one shared parent.**
  Cross-runtime reentry is legal and depth-bounded, and it is the route a
  browser's cross-profile seam takes - but the bound is an aggregate one, so a
  composition root that creates two runtimes without a shared aggregate budget
  has no bound on the chain at all. Create one.
- **No cross-profile value channel exists or is coming.** A guest-initiated load
  may not name another profile; the provider must answer with an artifact of the
  profile that asked. The seam is the embedder's, every call across it is two
  host-boundary transits, and a shared mutable region has no core representation.

Until that component exists, the honest position is the one section 1 already
takes: nothing here is advertised, and a browser is not a composition this
register describes.

---

## 5b. Compositions this programme plans, which this register does not have

The MVP programme recorded in [`docs/mvp.md`](mvp.md) plans composition roots for
a second input profile and for a native output form. **Two of the four rows this
table carried have since been built and have moved into section 3, and the two
that remain describe nothing that exists**, so the table below is still not part
of this register.

**Three rows left this table on 2026-09-07, and they are recorded rather than
deleted.** Two of them —
`Broiler.VM.Composition.WebAssembly.Execution` and
`Broiler.VM.Composition.WebAssembly.Harness` — read **PLANNED. Does not exist**,
with *no project, no source, no catalog baseline, no closure report, no bundle*
and, for the first of them, *the profile it would compose is a documentation
directory and nothing else*. Every clause of that was true when it was written
and none of it is true now: both roots are in `src/compositions/`, both are in
section 3 with a row apiece, both have a catalog baseline and a closure report in
`src/Broiler.VM.Profile.WebAssembly/docs/evidence/wa-0-001`, and the profile they
compose is a project with a decoder, a validator and an interpreter in it.
**A reader meeting both sections would have been told in one that the roots do
not exist and shown in the other that they do**, which is the failure a planning
table sitting beside a register is most exposed to and the reason this
reconciliation is a change in its own right rather than a tidy-up.

**The third row was withdrawn rather than fulfilled, which is a different thing.**
It anticipated *a JavaScript root that arms a page for one architecture*, at VM-7,
declaring `x86-64` and holding *a verifier and an executor and no code generator*.
No such root was minted, and none is now owed: what the row described arrived
**inside a row section 3 already had**, because the execution-only JavaScript root
acquired the ability to arm a page when the profile it composes did, and its cell
now says so. A planned row whose shape is satisfied by an existing composition is
withdrawn and not moved, because moving it would put a second row in section 3 for
one project and rule K1 fails on that.

**What none of that changes is the state of the work.** No row moved in any ledger
because a row moved here, no bundle was collected, and the roots that now exist
are demonstrations that have been published and run on no claimed runtime
identifier. Section 3's rows say what those roots are; their profiles' own ledgers
say what has been demonstrated about them; and this section says only which
planned rows stopped being planned.

**A planned row is not a register entry, and the distinction is mechanical rather
than editorial.** Rules K1 to K4 read section 3 and nothing else: K1 fails on a row
naming a root that does not exist and on a root existing with no row, K2 binds a
row's profile list to the composition's own reference set and printed catalog
table, K3 compares a retained catalog baseline, and K4 compares a published
closure. Every one of those needs a project, a build output and a bundle. A row
here has none, so **putting a planned composition into section 3 would fail K1
rather than reserve a place**, which is the same rule section 5a obeys when it
refuses to carry a row for the browser composition. The table below uses different
column names for the same reason: the register's table is located by its own
header, and a second table wearing that header would be a second register.

| Planned composition | Programme milestone | Expected profiles | Expected native execution | State |
|---|---|---|---|---|
| A conformance-oracle harness root | **WA-4**, which is **not on the MVP path and is unschedulable** — the profile's own ledger says so in those terms | The same unchosen profile ID | `none` | **NOT PLANNED, and named here only so its absence from the two rows above is not read as an oversight.** The programme walks WA-0, WA-1, WA-2, WA-3 and WA-5 and stops; WA-4 keeps every clause of its exit gate and goes unattempted, so nothing here is work anybody is tracking |
| A JavaScript root that arms a page for one architecture | VM-7 | `broiler.javascript` | `x86-64`, and nothing else | **WITHDRAWN 2026-09-07, and never created.** What this row described — an execution-only image holding a verifier and an executor and no code generator, permitted to arm one architecture — is what the execution-only JavaScript row of section 3 now declares, so no new root is owed for it. The row is kept and marked rather than deleted, because a reader who planned against it is owed the reason it stopped being planned |

**The conformance-oracle row was in this table for the opposite reason to the
three beside it, and it is now the only row here that is neither built nor
withdrawn.** The others were here because a milestone somebody is walking creates
them; this one is here because a reader who remembers the conformance oracle
would otherwise read its absence as an omission. Its milestone is unschedulable
and off the programme's path rather than merely later than the rest, and the two
harness roots must not be confused: the one the MVP creates is WA-1's, stood up
beside the execution-only root, carrying the binary corpus encoder and the corpus
store - and **that root now exists and is in section 3**, which is what makes
keeping this row worth the space rather than less.

**What the VM-7 row did not say was the part worth reading, and it holds after the
withdrawal.** It did not name the root, because a name in this document is a name
a later reader will treat as chosen; VM-7's own next action is to define the
declaration rather than to adopt one, and no name was ever minted here. It did not
say the architecture is settled beyond `x86-64`: the MVP record's route register
carries arm64 as emitting-only and x86-32 as dropped, and both are routes taken
without a decision rather than decisions - which is unchanged by an arm64 encoder
existing, because emitting is not arming and this column is about arming. And it
did not say that an image arming a page holds a backend: VM-7's own exclusion is
that an execution-only image holds no code generator at all, so the arming and the
generating are separate permissions that this schema deliberately keeps in separate
columns, and section 3 now has rows on both sides of that split - the execution-only
JavaScript root arms and generates nothing, the slice-compiler and CLI roots do
both.

**Nothing above is scheduled by being written here.** These rows move no ledger row
in any of the three ledgers this repository keeps, and a milestone that would
deliver one of them stays `Not started` until its own ledger says otherwise -
planning text does not change that state, and a table is planning text.

---

## 6. Changing this register

Adding a composition root means adding a row here, adding the project to
`graph.manifest.json`, and amending ADR 0001's project budget if the count
changes. Rule K1 fails if a root exists with no row or a row names a root that
does not exist; rule K2 fails if a row's profile list and the composition's own
reference set and closure report disagree. Making a composition advertised is a
release decision that ADR 0012 owns, not an edit to the Kind column.

**Changing a native-execution cell is not the same kind of edit as changing any
other cell, and this paragraph exists so that nobody discovers that afterwards.**
Every other column records something the checkout already is, so an edit that
disagrees with the checkout is caught by a rule. This column records what a
composition is permitted to do — and **from 2026-09-07 an edit that disagrees with
the checkout is caught by a rule as well**, in both directions, reading which
assemblies of the tree can map memory executable rather than reading this
document. *(Until that day it was caught by nothing at all, and this paragraph
said so and said to write the enforcement first where you can. The advice stands
and it was not taken here: section 3 records that five cells were false for a whole
change before the rule that would have failed them existed.)*
**What a rule still cannot catch is the architecture list being too wide**: an
image that links an arming path satisfies the rule whichever architectures the
cell names, so naming one the profile cannot arm is an overstatement no gate
reads. Name only what the arming path will accept, and say in the same change
where a reader can check it.

**A row for a composition that does not exist belongs in section 5b or nowhere.**
K1 fails on it here, which is a mechanism rather than a style guide: an anticipated
composition is recorded in prose, or in the plainly-marked table of section 5b, and
never in section 3.
