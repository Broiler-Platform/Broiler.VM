<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Evidence bundle UBC-4-002

**Milestone:** UBC-4 of [the universal bytecode programme roadmap](../../universal-bytecode.roadmap.md) -
the WebAssembly family. This bundle is the run after the change, its comparison with the base run
[`ubc-4-001`](../ubc-4-001/README.md) retained, the negative control, and the four roots published.
**Collected:** 2026-09-26, at commit `15d7a02`, from a clean tree, by `collect.py`, `opcode-coverage.py`
and `collect-integrity.py` in this directory. The negative control's two halves were retained earlier by
`collect-control.py`, each at the commit its own first line names.
**Core contract version:** 1, unchanged. **Universal bytecode contract version:** 2, moved by this
milestone. **Universal bytecode format version:** 1, unchanged.
**Status of the milestone after this collection:** `In progress`. Clauses 1, 2, 7, 8, 9 and 10 of the
exit gate are met on the evidence below. **Clauses 3, 4, 5 and 6 are not**: clause 3 for the
region-access primitive rows, which the E2 check does not reach; clauses 4 and 5 because the predeclared
rule judges them with one decision, which is NOT MET while population A cannot be judged; clause 6 for
the `LiveBytes` retention report, which this milestone changed. The milestone is not accepted either:
under [`docs/mvp.md`](../../mvp.md) no milestone is accepted while review is deferred, and nothing here
has been read by anyone but its author.

## 1. Identity

| Field | Value |
|---|---|
| Evidence bundle | UBC-4-002 |
| Milestone | UBC-4, exit-gate clauses 1 to 10 |
| Base | bundle `ubc-4-001`, the base run at `72e7491` |
| Rule | [`decision-rule.md`](../ubc-4-001/decision-rule.md), added in `6486add` and revised in `3042855`, both before the first code commit of UBC-4, `72e7491`, which added the harness's float comparisons before the base run |
| Commits of the milestone | `72e7491` and `0837208` the base (UBC-4.1); `9d160d6` the decision series (UBC-4.3, UBC-4.4); `1b55423` and `30f611e` the universal bytecode and the emitter (W1); `80a7e3e` and `5d171ce` the family (W2a); `1d78d76` and `75b1cc7` the translator (W2b); `c49347c` and `1883b96` the roots (W3); `77db8fb` the negative control failing; `e7efbc0` and `1b14548` the retirement; `4931f3e` the correction of the arms and `fe59638` the control passing (UBC-4.6); `9ccc9c5` the records (UBC-4.9), and `8cbbc0c` and `e906d78` their corrections after the checks of this bundle, the second also qualifying the family's and the emitter's remarks and assurance falsifiers; `15d7a02` the release of an allocation that throws, which the last check found unreleased |
| Compositions | `Broiler.VM.Composition.WebAssembly.Execution`, `Broiler.VM.Composition.WebAssembly.Harness`, `Broiler.VM.Composition.PolyglotCli` and `Broiler.VM.Composition.Ubc.Fixture`, the register rows of [`docs/compositions.md`](../../compositions.md) |
| RID | `win-x64`, which is not a supported RID |
| Owner | the WebAssembly profile owner, with the core architecture owner and the security owner; one person holds every role |
| Reviewer | none |
| Evidence class | conformance parity on population B, the negative control, publish-and-run logs, retained corpora replayed, a source reading and rule runs; no measurement of performance of any kind |

## 2. What this bundle demonstrates, clause by clause

| Clause | What it asks | Status | Where it is shown |
|---|---|---|---|
| 1 | Rule W1's list is Abstractions, Binary and `Broiler.VM.Ubc`; W2's baseline matches in both directions; U3 is not engaged and U2 passes over `Broiler.VM.Ubc` unchanged | met | `architecture-tests.log`: `W1_The_WebAssembly_Profile_References_Exactly_Abstractions_Binary_And_Ubc`, the five W2 tests (the surface equals its baseline, and a baseline omitting a member and one declaring a member that is gone are both rejected), and the six U2 tests. Rule U3 is minted by UBC-5 and does not exist at this commit, so nothing engages it |
| 2 | The translator is deterministic over the harness corpus, two translations compared byte for byte | met | `determinism.log`, and the `--determinism` run in each of `run-harness-jit.log`, `run-harness-trimmed.log` and `run-harness-aot.log`: every module the harness holds - the canonical module, every retained corpus entry and every module its execution and differential lanes build - translated twice and answered alike, and every artifact of the two compared byte for byte |
| 3 | Every `WasmOpcode` member has a row of Appendix C's table or a common mapping, and every `Primitive` row has a reference handler the E2 corpus reaches | **not met** | First half, met: `opcode-coverage.log`, written by `opcode-coverage.py`, finds every member either named by a case label of `WasmLowering.Step` or inside one of the two byte ranges `WasmFamilyTable` declares for its access and numeric rows, every byte of those two ranges naming a member, and no case label naming no member. It reads the range constants, not the table's rows, and so does not look at the eight global rows at `0xE0` to `0xE7`, which name no member by design; it was watched failing on a doctored reading of the lowering and the table, not retained. Second half, met for the numeric rows only: the `--primitives` run in each harness run log reaches every numeric `Primitive` row of the slice table from an input line of `src/tests/corpus/ubc-2/primitives.txt`, and fails a row with no input line. **The region-access `Primitive` rows - the loads, the stores and `memory.size` - are outside the lane and outside that corpus**, whose inputs are the numeric primitives'; their handler applies Appendix D's region evaluation itself, and the execution checks exercise them only through whole modules |
| 4 | The float-comparison check is retained failing on the unmodified arms and passing after the correction, both watched | **not met under the rule** | The two logs exist: `control-failing.log`, run at `1883b96` and committed in `77db8fb` - the rows outside the control agree and the twelve comparison rows `0x5B` to `0x66` fail, the reference arms giving no answer, exit 1; `control-passing.log`, run at `4931f3e` and committed in `fe59638` - the twelve agree, exit 0; the same lane passes in all three modes in the harness run logs. The arms are the ones W2a moved verbatim out of the retired interpreter, routing defect included. `4931f3e` changes the routing and, in the lane, only the control's header, which no longer says the arms are unmodified (line 33 of the two logs); between the two runs the retirement, `e7efbc0`, also replaced the retired `WasmValue` slot the arms used with a private slot carrying the accessors the arms use, and made their dispatcher private; no arm body changed otherwise. **The predeclared rule judges clauses 4 and 5 with one decision**, and its dated revision removed the reading that allowed a MET for clause 4 alone. That decision is NOT MET, so the two logs are partial evidence, named as such |
| 5 | The specification's test suite gives per assertion the verdict the retained base run gives, outside the named class; the ratchet is re-based by hand with the class named | **not met** | Population A was not judged: the suite is not pinned and has no reader in the harness root, and the WebAssembly profile owner decided on 2026-09-25 that this milestone is carried out on population B alone. Under the rule, population A's precondition - the pin and the reader existing at the base commit - is unmet, and the decision is NOT MET. `comparison.log` retains population B's comparison as the partial evidence the rule allows (section 3). No ratchet exists to re-base: the plan's ratchet is the conformance oracle's, which has no pinned suite, and the harness root sets none; the root's exit code moved from 1 at the base to 0, and the only members that moved are class (f)'s. **The holder** is the WebAssembly profile owner; section 7 says what meeting the clause would now take |
| 6 | `memory.grow`'s guest-observable refusal and the `LiveBytes` retention report are unchanged, with the existing tests passing over the new store | **not met** | Met for the guest-observable refusal: the `memory` check in `after-run.log` and in every harness run log - a growth past the profile's own page ceiling answers minus one and the memory's size is unchanged after it, as at the base. **Not met for the retention report, which changed in two ways.** (a) The store charges its `LiveBytes` retention before it allocates, with a charge that can be refused, where the base reported it after the allocation. (b) A growth whose charge a core budget refuses ends the step at the growth. The base answered minus one to the guest when fuel or allocated bytes were refused and let it run on until the core rewrote the completed step, and when the retention was refused it had already grown the memory and let the guest see the growth succeed. Both are recorded in the plan's `WAC-43` and in the concept's Appendix G, (a) also in [WAD-0001](../../../src/Broiler.VM.Profile.WebAssembly/docs/decisions/0001-the-memory-representation.md)'s addition and [WAD-0003](../../../src/Broiler.VM.Profile.WebAssembly/docs/decisions/0003-the-value-store-and-frame-routes-under-the-universal-bytecode.md)'s corrections of 2026-09-26, and (b) in the WebAssembly ledger's correction of its route paragraph. No test in the repository asserted this profile's `LiveBytes` amounts at the base, and none does now. **The holder** is the WebAssembly profile owner, who either accepts the change through a gate revision recorded in the programme roadmap with the superseded clause quoted, or has the store report after the allocation again |
| 7 | The memory representation decision exists in that profile's series and the store implements it; the WA-5 manifest is minted or its absence is recorded (UBC-D-3) | met | WAD-0001 (a pinned managed array reallocated on a successful growth with its base republished, decision UBC-D-2) and WAD-0002 (the table stays selected by `broiler.webassembly.slice`; `broiler.webassembly.numeric1` is not minted, and the table admits more than the identity's definition, decision UBC-D-3), both in `9d160d6`; the store is `WasmFamily`'s instance state, and the `memory` checks run over it |
| 8 | Both WebAssembly roots publish and run under the three modes with closures matching their register rows, the execution root's sibling cell naming `Broiler.VM.Ubc` and its native cell `none` | met | `publish.log`: eight publishes, the four roots trimmed and Native AOT, each exit 0. `run-<root>-<mode>.log` for each root and mode; `<root>-modes.log`, the three modes' catalogs byte-identical for every root and the run outputs byte-identical for all but the fixture root, whose one difference is named in section 7; `catalog-<root>.txt`; `closure-<root>.txt`, the trimmed publish's managed non-framework assemblies (a Native AOT image is one native file and carries none). Rules K1 to K5 in `architecture-tests.log`, run after the catalogs and closures were written, hold each closure to its row; the execution root's row names `Broiler.VM.Ubc` and `Broiler.VM.Emitter.Bytecode` as siblings and `none` as its native cell. The PolyglotCli and fixture roots are published and run too, because the first carries the WebAssembly profile and both link the universal bytecode |
| 9 | The `WAC-nn` entries exist for every plan sentence the milestone makes false, and the plan carries the bare pointers | met | [The profile's corrections file](../../../src/Broiler.VM.Profile.WebAssembly/docs/roadmap.corrections.md): `WAC-30` to `WAC-41` in `9ccc9c5` and `WAC-43` in `8cbbc0c` - among them the compiler non-goal (`WAC-30`), the one interpreter and its execution arm (`WAC-31`), section 5's package boundaries (`WAC-33`), section 7's unwrapped module (`WAC-34`), section 9's value, store and frame routes (`WAC-35`), section 10's execution mapping (`WAC-36`), section 17's restatement of the core's refusal (`WAC-40`) and the store's retention (`WAC-43`), which the first check of this bundle found uncorrected; each is pointed at from the sentences it corrects. `WAC-42`, filed with them, corrects a header stale since 2026-09-07 and is not one of them. `WAC-41` moves a counterweight position and waits on the owner's confirmation (section 7) |
| 10 | The ledger's UBC-4 row names each unmet clause individually while any is unmet | met | [The programme ledger](../../universal-bytecode.status.md), whose UBC-4 row is moved in the commit that retains this bundle and names clauses 3, 4, 5 and 6, each with its holder |

## 3. What the comparison shows

`comparison.log` joins the base run (`ubc-4-001/base-run.log`) and the run after (`after-run.log`) member
by member within each of population B's three lanes, in the order the root prints them, and classifies
every difference under the rule:

- **The retained corpus.** Every entry answers what its manifest records, before and after, in the
  fields the replay compares: outcome, reason, code, dimension and scope. The replay does not compare
  a source position, by design. It now records which stage answered each entry and prints the totals:
  most are refused by the translator - the profile's decoder and validator, run before the core - and
  the rest are translated and answered by the core, none of them refused. **Class (r) is empty**: no
  refusal moved to a universal code. `corpus-rebase.log` rewrites the corpus from the harness into a
  scratch directory and finds it byte-identical to `src/tests/wasm/corpus`, so the re-base changed no
  retained byte. `corpus-integrity.log` is the mutation pass `ubc-4-001` could not run, because the
  replay now exits 0: four entries each moved by one byte, every one detected.
- **The execution checks.** Every float comparison moved from `ProfileFault/ProfileContractViolation`
  with no results payload to the specification's value. **Those are class (f), and they are the only
  differences in the lane.** Every other execution check answers what it answered at the base.
- **The differential checks.** Every check agrees with its oracle, before and after.

No difference falls outside both classes. The rule's decision is nevertheless **NOT MET**, because it
is MET only if population A was judged; this comparison and the negative control are the partial
evidence the rule names, and neither is a verdict on clause 4 or clause 5.

## 4. What was run

```text
dotnet build Broiler.VM.slnx -c Release
python docs/evidence/ubc-4-002/collect.py --vcvars "C:\Program Files\Microsoft Visual Studio\18\Professional\VC\Auxiliary\Build\vcvars64.bat"
python docs/evidence/ubc-4-002/opcode-coverage.py
python docs/evidence/ubc-4-002/collect-integrity.py
python eng/ubc-bundle-manifest.py --bundle docs/evidence/ubc-4-002 --milestone UBC-4 --evidence-class conformance-parity-and-publish-and-run ...
```

`collect.py` runs population B once after the change and compares it with the base, runs the
determinism lane and the corpus re-base, publishes the four roots trimmed and Native AOT, runs each
root's checks in each of the three modes, and then runs the architecture suite; its header says what
each file is. `collect-integrity.py` runs `eng/wasm-corpus-integrity.py` over a copy of the retained
corpus made for the run, so the retained bytes are never moved, compares the copy with the retained
corpus byte for byte afterwards, and writes both into `corpus-integrity.log`. The tool's output is
kept with its line endings made LF and the copy's path replaced by `<scratch>`, and every line the
script adds - the command, the exit code and two notes - begins with `# collect-integrity:`. `collect-control.py`
retained the negative control's two halves at the commits they name and is not re-run here. The
manifest command's remaining arguments are recorded in `manifest.json`: `--source-revision` and
`--note` in its `source-revision` and `note` fields, and the `--source` and `--input` lists in
`sources` and `inputs`.

## 5. Environment

Windows 11 Enterprise, the .NET SDK `10.0.401`, Python `3.11.9`, the Professional installation's
`vcvars64.bat` for the Native AOT link, the checkout at the commit above with nothing outside this
directory changed. The JIT runs use the framework-dependent build output; the trimmed runs a
self-contained publish; the Native AOT runs the published image.

## 6. The departures from the concept this milestone is responsible for

[Appendix G of the concept](../../universal-bytecode.md) records where the code departs from the text,
and each bundle repeats the list it is responsible for. This one's rows are the nineteen marked
"(milestone UBC-4)" there, the row marked "(decision WAD-0001, UBC-D-2)" and the row marked "(bundle
`ubc-4-002`)":

- **The universal bytecode.** The contract version moves to 2 while the format version stays 1 (5.2);
  a third effect form, the signature (5.4 and 5.6); a signature row's parameters are the bottom slots of
  its inputs (5.9); a truncation traps `InvalidConversion` on NaN and `IntegerOverflow` out of range
  (Appendix D); a family may resolve an entry point, admit or refuse an instance, name a start unit and
  abandon a refused instance (7.2); a form carries the contract version it was compiled against (6.3
  and 7.1).
- **The WebAssembly family.** Globals are eight typed dynamic rows rather than region accesses
  (Appendix C); `call_indirect` is a signature call row with a u32 type-index operand and a nominal type
  check (Appendix C and 6.2); the emitter executes region accesses through the family's handler (7.2);
  the memory's base is stable between growths rather than for the instance's life (5.8, 8.2 and
  Appendix D); one FamilyData section of kind 9 holds the module's definitions, and the function types
  are the Types section, never deduplicated (5.2); the family declares no value plane; a growth a core
  budget refuses ends the operation rather than answering minus one, and retention is charged before
  its allocation (route MVP-1); an instantiation paces its segment charges within the uncharged-work
  bound.
- **The translator.** It runs in the composition root, before the core, under a meter of its own with
  the core's release classes (8.2 and 12, route UBC-R4); its work reaches no runtime-level account and
  no verification wall clock, and the lowering is charged to no meter; every edge into a label arrives
  with the frame's entry stack, a long drop runs through a shared chain, and a branch to the function's
  label is a return (Appendix C's lowering); an artifact's Entries section is empty and the family
  resolves exports from FamilyData (Appendix E); what the universal bytecode cannot hold of a valid
  module is refused with WebAssembly codes 2871 to 2874.
- **The roots and E2.** Every root that runs a module translates it under the runtime's effective
  ceilings and verifies the artifact under the family's descriptor, and the corpus replay records the
  stage that answered (9); obligation E2's check runs in the harness's `--primitives` lane over the
  numeric primitive rows, the float comparisons reported apart as the negative control (7.5), and the
  row now says the region-access rows are outside it.

## 7. What is NOT here, named rather than left as an absence

- **No population A, and no way to judge it under this rule.** The specification's test suite is not
  pinned and has no reader. The rule's precondition is the pin and the reader existing at the base
  commit, `72e7491`, which had neither, and the path the base run ran on - the profile's own
  interpreter - is deleted from `e7efbc0` on. So a pin and a reader written now do not meet clause 5 by
  themselves: it would also take a new dated rule quoting this one and a base run of population A taken
  again under it, on a commit that still has the retired path. The programme ledger names the
  WebAssembly profile owner as the holder.
- **The region-access rows are not reached by E2.** Clause 3's second half is met for the numeric
  `Primitive` rows only. Meeting it for the loads, the stores and `memory.size` needs a region input
  corpus and a lane that runs the family's handler for them; the programme's primitive corpus, from
  UBC-2, excludes region accesses by design.
- **The retention change is not accepted.** Clause 6 is unmet until the WebAssembly profile owner
  accepts the change through a gate revision or the store reports after the allocation again, and no
  test asserts this profile's `LiveBytes` amounts, before this milestone or after it.
- **One RID, one machine, one lane.** `win-x64` is not a supported RID, and nothing here was run
  anywhere else.
- **Clause 3's first half is a reading of the source by pattern.** `opcode-coverage.py` reads three
  files; it is not a check over the compiled assembly, and a lowering that reached a member through a
  method other than `Step` would not be seen. It is retained as a script's reading rather than written
  as a rule of the architecture suite, which could read the same files without referencing the
  profile; making it a rule is not done here.
- **Translation work is outside the core's accounts.** Route MVP-15's cost: a module's decoding,
  validation and lowering reach no runtime-level account and no verification wall clock. The translator
  meters decoding and validation itself with the core's release classes; the lowering is bounded by the
  token, the artifact-bytes ceiling and its linear growth, not by a meter.
- **The decoder's bulk reads are not paced.** A function body, a data segment's contents, a name -
  every export's among them - and a skipped custom section are each charged as one read, so any such
  read that takes the work since the last poll past the profile's uncharged-work bound - a body,
  segment or name longer than the bound, a shorter one read after enough unpolled work, or a large
  custom section - latches a poll-bound breach, and the module is
  refused with `ResourceExhaustion` on `VerifierWork` although nothing is wrong with it. The base's
  verification did the same with the same decoder, and no module of population B is large enough to
  meet it. It is not fixed here, and is written out as
  [a task](../../tasks/pace-webassembly-decoder-bulk-reads.md).
- **The core keeps a dropped instantiation's retention.** When a refusal or a cancellation latched
  during an instantiation the emitter answered as complete, the core answers an exhaustion or a
  cancellation, publishes no instance and releases nothing, so the retention stays charged for the
  runtime's life and, under an aggregate budget, at the parent beyond it. A refusal can latch without
  the family meeting it - an aggregate wall clock the meter latches at a poll that still answers - so no
  family can close this path. The family releases what it retained on every failure the emitter answers,
  an allocation that throws after its charges included since `15d7a02` - a path no check here exercises,
  because an allocation below the profile's own page ceiling does not throw on demand; the core's path is
  a defect of the core, not fixed here, and written out as
  [a task](../../tasks/release-dropped-instantiation-retention.md).
- **The fixture root's three run outputs differ in one figure.** `cancellation-within-bound` prints the
  fuel charged after the operation's token was cancelled, which depends on when the cancelling thread
  lands; each mode's figure is within the declared bound, and no other line of the three differs.
- **`WAC-41` moves a counterweight position.** The translator is an in-process producer, so the plan no
  longer says this profile would not co-sign an in-process producer input form; it says the form is not
  asked for while no measurement shows the byte round trip on a critical path. The entry records the
  move, and the owner's confirmation of it is still to be recorded.
- **Metering is the universal bytecode's.** Fuel is charged per universal bytecode row and per frame's
  declared locals, and a structural instruction the translator lowers away charges nothing (WAD-0003
  row 8, `WAC-35`); no line of population B moved because of it.
- **No review.** Every record and line of code here is unreviewed, and the author and the owner are
  one person.

## 9. Exclusions

**This section defines no exclusion identifier, and that is deliberate rather than an omission.**
