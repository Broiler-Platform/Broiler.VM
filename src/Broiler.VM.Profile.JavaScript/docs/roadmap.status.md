# Broiler.VM.Profile.JavaScript roadmap status

**Last updated:** 2026-09-04

**Authority:** This file is the authoritative current-evidence ledger for the milestones in the
[JavaScript profile roadmap](roadmap.md). The roadmap defines planned work and objective exit
gates; this ledger records whether those gates have accepted evidence. Where implementation or a
dated decision replaced something the roadmap used to say, the plan carries the new reading and
[the corrections and rejections](roadmap.corrections.md) carry what it replaced — **that file
records no status and advances nothing here**.

**At this snapshot, JS-0, JS-1, JS-3a, JS-3b, JS-4, JS-5, JS-6 and JS-9 are `In progress`, JS-2 is
blocked, and the remaining rows are `Not started`. JS-4, JS-5 and JS-6 moved on 2026-09-04, and
JS-3b moved on 2026-09-03**, and all four moved without JS-2 moving: the slice front end and then
the wide one were each written in this checkout rather than ingested. For the slice that is the
change of plan recorded at [JSC-43](roadmap.corrections.md#jsc-43); for the wide surface **the plan
is unmoved** — roadmap [section 9](roadmap.md#9-the-semantic-front-end-and-lowering) still gives the
general front end to JS-2's ingest and nothing that landed on 2026-09-04 touches it — so what is
here is a fact about this checkout and not a change of state on its own. What moves a row is that
the milestone now owns code and a retained record.

**What existed on 2026-09-03 was one feature manifest, one format version**, a verifier, an
executor, a descriptor admitted by a catalog, a hand-written lowering, three composition roots that
publish on one RID under JIT, trimming and Native AOT — and whose **one retained Native AOT run
exits 1**, which the withdrawn-claim paragraph below is about — a retained corpus, a published
diagnostic-code registry, a frozen public-API baseline, a decision series, this profile's own group
in the rule register, retained evidence bundles, a fuzz target with its negative controls, a
**source front end** for the slice — a tokenizer, a syntax tree, a parser, one static-semantic
stage and a source-to-bytecode lowering — a conformance harness whose scoring target was this
component's own fixture trees, and, from that same day, a pinned and archived language edition and
a pinned and archived conformance suite with a floor over part of it. Each of those is counted by
section 2 below and by the record that holds it, not here. **This paragraph said until 2026-09-04
that no third-party conformance suite was pinned, in this repository, or scored by anything here**,
and section 3's own row, closed on 2026-09-03, said the opposite of all three
([JSC-68](roadmap.corrections.md#jsc-68)). That is update rule 1 going unapplied — the change that
closed the dependency did not update this summary in the same change — and it is recorded here
rather than quietly repaired, because a summary contradicting the section below it is the shape of
defect this ledger exists to catch.

**What changed on 2026-09-04 is that a second of nearly everything exists.** A second feature
manifest, `broiler.javascript.wide`; a second bytecode format version and a verifier of its own for
it; a value and object model; an interpreter; a standard library; a second source front end, which
shares JS-3b's tokenizer and nothing else; one optional host capability import,
`broiler.javascript.write`, through which `print` reaches a host; and two host modes — an end-user
CLI that runs the wide surface by default and keeps the slice behind a flag, and a conformance
composition that reads a real test262 checkout from a path on the machine rather than from this
repository. Together they are the first configuration of this profile that runs a real JavaScript
workload end to end. **The plan did not say `wide`**: roadmap
[section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)'s allocation table
opened `broiler.javascript.core` at JS-5, and the row the table gained instead is recorded at
[JSC-70](roadmap.corrections.md#jsc-70), on decision
[JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md). **What the plan
still says is that regular expressions go through a from-scratch matcher at JS-6 or out with a
published failure**, and neither is what shipped; section 3's row carries that.

**There is still no suspension, no guest-initiated load and no snapshot**, and the three rows that
moved are `[PARTIAL]`: each owns a bundle that demonstrates some of its gate, with every unmet
clause named in that bundle's own exclusions. **A third-party suite is pinned and run now, and what
it was run over is subtrees somebody chose** — which measures those subtrees and not the suite, so
roadmap [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)'s rule,
that a manifest with no retained run of its own is not accepted, is unmet for
`broiler.javascript.wide` and stays unmet. **And a front end now exists that admits functions,
objects and string values**, so the sentence this paragraph carried until 2026-09-04 — that nothing
here is a JavaScript front end — is gone rather than softened. What replaces it is not its opposite
but a list, of what the wide manifest refuses by name and of the approximations it makes, published
as a subsection of section 2 rather than summarised away.
No milestone is complete because its design appears in the roadmap, **nothing here has been
reviewed by a human**, and nothing in this component may be described as validated, accepted or
supported.

**One retained claim was withdrawn on 2026-09-01, and it is a correction to a summary rather than a
change of state.** Both JS-9 bundles reported their publish-and-run as *six runs, all exit 0*, and
both bundles' own `publish-and-run.log` records the execution-only root's **Native AOT run exiting
1** on the soak's plateau check — five of six. Each bundle now carries a dated correction beside
that row, no retained log was edited, and the JS-9 row below carries what is open because of it.
**Nothing moved**: JS-9 was `In progress` before and is `In progress` now, with one more clause
named. It is recorded up here rather than only in a row because the failure mode is this
component's own — a summary that reports the passing half of a run it retained in full — and
update rule 2 is the rule it broke.

**The check that produced that exit has since been corrected, and the distinction between the two
sentences is the whole of what this ledger is for.** The working tree's soak passes in every publish
mode today; **no retained bundle shows it doing so**, because none has been collected since the
correction. A reader must not read the first fact as the second: what a row here may cite is a
bundle, and the newest one retains an exit 1. The JS-9 row carries both halves.

**The placement decision is taken.** This component is not a repository of its own and is not a
component of its own: it is a family of product projects inside `Broiler.VM`, at
`src/Broiler.VM.Profile.JavaScript*`, with its roadmap and decisions in the profile assembly's own
project directory. The profile's half is [JSD-0001](decisions/0001-placement-identity-and-assembly-topology.md);
the core's half is ADR 0001 revision 5, which authorises the three projects and revises rule A11
so that a profile may reference its own format sibling. **Three things the roadmap assumed would be
this component's own are the host component's** - the assurance system and the rule register in
which this profile holds group N, each recorded as a dated deviation in
[JSD-0006](decisions/0006-assurance-evidence-and-rules-adoption.md) rather than dropped, and the
licence and notice files, which
[JSD-0001](decisions/0001-placement-identity-and-assembly-topology.md) records as the host's and
which no deviation covers because adopting them costs this profile nothing. **A fourth,
the API baseline, was adopted at JS-0 and became this family's own at JS-3a**, because the host's
describer cannot reach a profile assembly without a project reference rule A11 forbids;
[JSD-0012](decisions/0012-the-profile-api-baseline-and-where-its-clause-lives.md) records it and
rule N10 holds it. **What is not shared is evidence:** a JS bundle is cited only by this ledger, a
core bundle only by the core's, and update rule 6 below is unchanged.

---

## 1. Reading this ledger

Four categories must remain distinct, and conflating any two of them is how an unfounded claim
gets recorded:

- **Plan** is proposed scope, sequencing, ownership, or an exit gate in `roadmap.md`. It is not
  implementation evidence and not validation evidence.
- **Observed repository state** is a reviewable fact about the current checkout — for instance
  that this component contains no project file. It can explain a status; it cannot satisfy a
  future implementation, contract, conformance, Native AOT, or release gate.
- **Accepted evidence** is an immutable, reviewable bundle that identifies the exact sources and
  gate, records the executed commands and environment, retains their outputs, and demonstrates
  every part of the objective exit gate. Only accepted evidence may advance a milestone to
  `Accepted`.
- **Inherited material** is anything copied from the seed. It carries **no status of its own**.
  A copied file is unvalidated and unreviewed in this component on the day it lands, however long
  it has existed elsewhere.

A fifth thing is deliberately **not** a category here. **A correction to the plan is not evidence
and does not appear in this ledger's tables.** When implementation invalidates, rejects or
re-scopes something the roadmap said, the roadmap is edited and
[the corrections and rejections](roadmap.corrections.md) record what it said before — a change of
*plan*, never a change of *state*. A milestone whose scope was corrected has moved no row here,
and a row here moves only on evidence.

**Work in other components is not this component's evidence.** In particular, no conformance
result, benchmark, measurement, review decision, or Native AOT sample produced by the legacy
JavaScript engine component or by the Broiler.VM core establishes anything here, and no gate in
this ledger may cite one. That rule is not a courtesy to the fork; it is what makes a number in
this file mean something.

**This document is under the host component's review-document rules, and was not until JS-3a.**
Those rules built their corpus from the host's own `docs/` and this ledger lives elsewhere, so the
clauses that exist because a reviewer reads these documents — no citation of a source line number,
a closed mark vocabulary, every cited exclusion defined — governed a ledger a profile reviewer
never opens and not this one.
[JSD-0010](decisions/0010-which-review-rules-govern-this-profiles-documents.md) closes that, and
records the two clauses that still do not reach this profile's bundles and what would close each.
**Section 2's mark table is this document family's legend**, read by the rule rather than restated
by it.

**One consequence for earlier evidence, stated rather than left to be inferred.** The architecture
suite now reads more documents than it did, so a suite total collected before JS-3a is a total over
a different test corpus. Bundles [JS-0-001](evidence/js-0/README.md) and
[JS-1-001](evidence/js-1/README.md) are unaffected in what they demonstrate — no rule either of
them cites changed — but each `suite.log` is a run of a suite that did not read this ledger.

### Status vocabulary

| State | Meaning |
|---|---|
| `Not started` | No milestone-owned implementation or accepted gate evidence has been recorded. Planning text does not change this state. |
| `In progress` | Milestone-owned work or evidence collection has begun, but the objective exit gate has not been accepted. The ledger must link its working evidence and list every open gate condition. |
| `Blocked` | Work has a named external dependency that prevents the next action. The blocker, its holder, and its unblock condition must be recorded. **Lack of scheduling is not a blocker; an unaccepted upstream contract is.** |
| `Accepted` | Every objective exit condition has an immutable evidence bundle and an owner and reviewer decision recorded here. Partial success cannot use this state. |
| `Superseded` | A dated decision replaced the milestone or gate. The replacement and the decision record must be linked; evidence history is retained. |

---

## 2. Current milestone status

The leading column is an **evidence verdict** — the author's mark about what a row's retained
evidence shows. It is not a reviewer's finding and not a change of state.

**This table is this document family's mark legend, and rule H1 reads it.** The vocabulary is
closed and has three members; a mark used anywhere in this profile's review documents that this
table does not publish is a rule violation, and so is a mark from the component's own nine-member
legend — the two vocabularies say different things and a reader must not have to guess which one
a mark came from. [JSD-0010](decisions/0010-which-review-rules-govern-this-profiles-documents.md)
records the split.

| Mark | Meaning |
|---|---|
| `[NONE]` | The row has retained evidence of no kind. |
| `[PARTIAL]` | The row has a retained bundle that demonstrates some of its exit gate, with every unmet clause named in the bundle's own exclusions. **A `[PARTIAL]` row is not a qualified pass.** It is a row whose gate is open, and the named clauses are what is open. |
| `[FULL]` | The row's bundle demonstrates every exit-gate clause. It is still not `Accepted`: acceptance additionally needs an owner and a reviewer decision, which nothing here has. |

Seven rows are `[PARTIAL]` and the remaining five are `[NONE]`.

**The milestone set changed on 2026-08-31 and this table now carries the new shape.** What was one
`JS-3` is now `JS-3a` and `JS-3b`, split by dependency rather than by size: the conformance harness
needs a scoring target and not a copied front end, so leaving it fused put this component's only
external correctness signal behind both of the blockers in section 3 when it needed to be behind
neither. Twelve rows, not eleven. Nothing is accepted under either shape, so the split changes no
evidence claim — it changes what a reader is told is schedulable today. The split is recorded as
[JSC-15](roadmap.corrections.md#jsc-15) and the delivery file carries the same shape.

| Verdict | Milestone | State | Current evidence | Immediate evidence-producing action |
|---|---|---|---|---|
| [PARTIAL] | **JS-0 — boundary, placement, identity, assurance floor** | **In progress** | [Bundle JS-0-001](evidence/js-0/README.md): Release build of the whole solution with 0 warnings; the whole suite green; the assurance gate green and the assurance **release** mode refusing while naming each blocking declaration individually; **8 negative controls, each failing the suite when injected and passing after revert**; the candidate seed identity re-derived and matching on all four revisions. Seven decision records, [JSD-0001](decisions/0001-placement-identity-and-assembly-topology.md) through [JSD-0007](decisions/0007-cross-profile-position-and-amendment-grading.md). Rules N1–N4 registered Active with nine witness inputs. | **One exit-gate clause is open; the other was discharged at JS-3a.** (1) **Open**: the two-profile catalog test's `eval`-refusal half needs guest loads and is carried to JS-8. Its descriptor half was discharged at JS-1 in both directions. (2) **Discharged 2026-08-31** by rule N10 and bundle JS-3A-002: the family's public surface is frozen in a baseline of its own, described from the build output without loading anything, and compared in both directions. [JSD-0012](decisions/0012-the-profile-api-baseline-and-where-its-clause-lives.md) records why the clause was re-homed to JS-3a rather than left at JS-3b. **Discharged is not accepted**: JS-0 still needs a reviewer decision that nobody has made. |
| [PARTIAL] | **JS-1 — the whole contract loop on a narrow slice** | **In progress** | [Bundle JS-1-001](evidence/js-1/README.md). `broiler.javascript.slice` is minted and format version 1 defined, carrying framed sections, a tagged constant pool, fixed instruction boundaries, **exception regions and suspension targets reserved and refused**, a canonical position table and declared maxima checked before use. All seven core-facing types are implemented. The descriptor is filled in one full-arity construction, admitted by a catalog, and **four named negative cases each provoke a refusal**. **All five verifier outcomes** are produced by named entries of a 51-entry retained corpus which replays twice with no residue, contains 16 passing controls, and on which the verifier throws nothing. **Four of the five execution-step kinds** are produced by named checks. **Two composition roots publish AND run on `win-x64` under JIT, trimmed self-contained and Native AOT**, warnings as errors, closures read off the published output: six managed assemblies for the execution-only image and seven for the compiler-bearing one, **differing by exactly the lowering**. JS-0's carried two-profile catalog clause is discharged in both directions. **Twelve negative controls**, four of them judged by the corpus rather than by the suite. **A third composition root landed on 2026-09-02 and it is an application rather than a console program**: an Android head composing exactly what the execution-only root composes, running that root's own check source on a booted Android system, retained as [Bundle JS-ANDROID-001](evidence/js-android-001/README.md) — 66 corpus entries replayed to their recorded answers, twice, plus the four ordering assertions, on **Mono rather than CoreCLR**. It is the first evidence in this profile from a runtime that is not CoreCLR and it claims nothing about Native AOT, about trimming or about a device: the collection is on an emulator and the bundle's exclusions say so first. That bundle recorded having **no negative control** and named it a gap; [Bundle JS-ANDROID-002](evidence/js-android-002/README.md) closes it with two, judged by a run on the device — one flipping a byte in the resource extraction the first bundle's own argument rests on, one making division by zero a fault in the profile, which the device replay catches by name on four entries. Its first run reported both as passing when the harness was the thing that was broken, which that bundle records rather than smooths. **What the head DECLARES is controlled too**, by [Bundle JS-ANDROID-003](evidence/js-android-003/README.md): rules K3 and K4 had a rejecting direction over desktop rows and nothing showed they read the Android row's baseline or its retained closure — a composition can be registered, given an evidence column and compared by nothing. **K1 has one too** ([Bundle JS-ANDROID-004](evidence/js-android-004/README.md)): deleting the head's register row fails K1 **alone**, which is at once the demonstration that the rule reads the real register and the demonstration that no other rule would notice a composition leaving it — K2, K3 and K4 iterate the rows, so a deleted row is one they no longer check. **K2 has one whose subject it is** ([Bundle JS-ANDROID-005](evidence/js-android-005/README.md)): the row claims a profile its own catalog table does not report, which fires K2 alone where the K3 injection had only ever fired it alongside K3. **Group K now has a control per rule over this row** — K1, K2 and K4 firing alone, K3 firing with K2 by design — where before JS-ANDROID-003 none of the four had been shown to read this row at all, every rejecting direction in their own tests being asserted over lists built in memory. Three more of K2's clauses are controlled by [Bundle JS-ANDROID-006](evidence/js-android-006/README.md), **none of them alone** — K4 builds its allowed set from the row's assembly columns and K3's subject is the catalog files, so a clause living in either place cannot fire by itself, and the profile-identity clause is the only one that can. **Writing them found the register parser accepting a row nobody wrote**: it drops empty cells before indexing, so clearing one shifted every column after it left — the sibling became the profile assembly and the evidence path moved one column over, with K1 green throughout because the two counts moved together. The parser counts a row's non-empty cells against the header's and stops now, and the first version of that fix was itself a no-op that compared a split length which does not move when a cell is emptied. K2's last three clauses are controlled by [Bundle JS-ANDROID-007](evidence/js-android-007/README.md), which reaches all of them by editing the catalog baseline's profile line — two of its injections differing by one prefix on one word, which is what isolates the reserved-label clause from the package-identity one. **Every clause K2 has now has a control whose subject it is.** **Both directions of K1 are controlled** by [Bundle JS-ANDROID-008](evidence/js-android-008/README.md), and the reason the second stood open through four bundles was a defect rather than an inconvenience: a row naming a composition the checkout does not have made K2 and K3 throw and K4 blame a missing closure report, so three tests crashed or misattributed around the one rule with something accurate to say. Those three leave a row with no subject to K1 now, which is not a weakening — the row is still reported, by the rule whose subject it is. **And the clause attribution is an observation now** ([Bundle JS-ANDROID-009](evidence/js-android-009/README.md)): a reporter writes what each group K rule said, over exactly the inputs those rules' tests compare, and the log carries the rules' own messages for all twelve controls — which showed several injections reaching more clauses than their rows claimed, something an exit code could never have said. It reports and does not judge, runs only when asked, and asserts that it wrote something when it is. It covers **76 of the register's 77 rules** since [Bundle JS-ANDROID-011](evidence/js-android-011/README.md), which found that the thirty JS-ANDROID-010 had excluded were three different problems rather than one: fifteen already had a named helper returning a collection and needed no extraction at all, seven had one built inline and were extracted as a **move** — the test calling the extracted function rather than keeping a copy, since two implementations of one rule is the drift a report exists to prevent — and **eight cannot be reported without changing what the rule claims**: six assert equalities or an absence rather than producing messages, and two assert their clean direction over a witness input rather than over the checkout. **Five of those six are reported since [Bundle JS-ANDROID-012](evidence/js-android-012/README.md)**, which was asked to do the rewrite loudly and found the list of six wrong in two ways: `M1` and `N10` had returned message lists all along and were mis-sorted by a survey that counted assertion forms without opening the bodies, `C3`'s absence already computed the list of languages it found and merely never phrased it as messages, and only `C1` and `E1` needed genuine restating. `C1`'s restatement carries a clause the obvious rewrite would have dropped - two ordered sequences disagree on a duplicate where a membership check does not - and a control exists whose only finding is that clause. The sixth, `E5`, is not reported and the reason recorded for it was wrong in kind: it is a **Deferred** row superseded at VM-1 whose activation milestone is `never`, and the register's own test forbids any test to assert it, so reporting it would mean writing a rule this suite deliberately does not have. `J10` and `J11` were said to remain, asserting their clean direction over witness inputs rather than over the checkout, and that was a misreading too: both tests assert over a witness in the MIDDLE and over the checkout LAST. **Both are reported since [Bundle JS-ANDROID-013](evidence/js-android-013/README.md)**, which was asked for controls on them and found three things while building them. **`E5` is the only rule the mechanism does not reach**, and only because it is a row no test may assert. **J11 is not silent on a clean checkout** - it reports 905 blockers, every relevant unit in this component being `HUMAN_PENDING`, which the owner's ruling permits - so silence is not the universal shape of a clean report and a reader who assumed it was would read the release gate as a broken harness. **J11 is also the first control here that an exit code cannot judge**: its ordinary-run assertion is `NotEmpty`, so an injected blocker turns 905 into 906 and its own assertion never moves, while the suite goes red through other rules; the reporter is the only instrument that can say the gate saw the defect, which is the case JS-ANDROID-009 built it for arriving in a stronger form than it anticipated. **And J10's register row was false**: it said the rule was red with 41 units named while the generated report in the same repository said `| Required and missing | 0 |`, because the test that holds the row to what the rules implement compares it against a hardcoded copy of itself rather than against the tree. The row is corrected and the stale text quoted rather than erased. No control fires J10 or J11 alone, and that is structural: the criteria count and every annotation are generated figures, so an injection that changes what these rules read also changes what the generator would write. **That bundle closed its own finding with an exclusion rather than a fix** - the corrected row was prose, and nothing compared it to the tree on any later day - and [Bundle JS-ANDROID-014](evidence/js-android-014/README.md) mints the rule it named. **Rule J12 does not compare the row's figure to the report's; it removes the row's copy of the figure.** A row that needs one CITES it, as `{criteria:required}`, and the rule resolves the citation against the generated report's own table, so there is nothing left to go stale - which is this component's existing policy on hand-maintained figures, applied to the register rather than to an artefact. Its decisive control puts the real stale sentence back into J10's row **in both places the row is stored**, because two agreeing copies is exactly what made the defect invisible: the register's row-equality test is green on that injection and J12 is not. A second control types in the CORRECT current figure and is reported anyway, which is the rule's whole claim - a right number and a stale number are the same object seen at different times. **The first version of J12 reported sixteen innocent figures**, because it tested for a criteria word and a number in one sentence, and rows legitimately count comment lines and publish clauses while discussing criteria; the clause is adjacency now and a test asserts the rows that discuss criteria without counting units stay silent. Two rows were reworded rather than the rule weakened, J12's own among them, which is the choice rule J9 recorded making under the same pressure. **That bundle's own EX-102 is narrowed by [Bundle JS-ANDROID-015](evidence/js-android-015/README.md)**, which was asked to close it and closed one class of it completely while saying plainly that the rest is not closeable: a claim that some number of things EXIST must now cite its figure, and the catalog is widened from three criteria metrics to sixteen - the report's assurance figures, the graph's counts computed the way the graph rules compute them, and the ADR counts. **Sweeping for that one shape found eighteen counted claims and eight rows already wrong**: A7 said eight edges where the manifest holds fifty-nine, A4 said five test-only projects where there are nine, four rows said two composition roots where there are five, and one sentence of J1's row carried three wrong figures at once. Every one was green, because the row-equality test compares a row against a hardcoded copy of its own prose - the defect that produced J12 in the first place, found at scale rather than one row at a time. Thirteen claims became citations and five were reworded to stop asserting a count nothing can check, which is the principle in one sentence: if the tree can compute it, cite it; if the tree cannot, do not state it as a number. **What is NOT closed is every figure worded outside that shape**, and the general problem - prose that stops being true - is not solvable by a rule; EX-102 now says exactly that rather than gesturing at it. Writing the clause also put a CONTROL CHARACTER in its own regex: a shell heredoc collapsed a doubled backslash and a non-raw Python string turned the result into chr(8), so the identifier guard held a backspace, could never match, and silently guarded nothing until the bytes were read. That bundle reworded five claims because nothing computed their subjects and recorded the loss; **[Bundle JS-ANDROID-016](evidence/js-android-016/README.md) adds the six figures they needed and all five state their numbers again**, the catalog reaching 22. A consumer profile is defined as what a composition root reaches for that is NOT the core, since the three packable assemblies are what every root references and the fixture profiles are deliberately not named `.Profile.` - the answer was two at VM-3 and is four now. **Two of the six are not counts of the tree and the rows citing them say so**: one is rule K2's own arity and one the size of a declared contract set, both fixed by a decision rather than by what the checkout grew into, and citing them beats retyping them without being the same claim. **A second stale sentence fell out of rule H1's row on the way** - it named the two evidence-bundle READMEs where there are far more - and the count clause never saw it, because the figure was attached to a noun rather than to the word `exist`, which is the adjacency limit JS-ANDROID-015 recorded doing its predicted damage one row later. **That row was not alone, and the sweep for its shape is [Bundle JS-ANDROID-017](evidence/js-android-017/README.md)**: 169 figures across 46 rows are bound to a noun, most of them a rule describing itself, and filtering to subjects the tree computes left the whole assurance family stale from ONE event. The JavaScript profile came under assurance coverage, the covered set grew from 45 files to 61, and every figure derived from it moved - eight rows went on saying 45 covered source files, 48 artefacts, 689 annotated units, 903 exempt and 1,592 in the tree, against 61, 64, 905, 1082 and 1987. Every one was green, because each row agreed with its own hardcoded copy. **A fifth clause reads a figure standing before a countable subject**, and every subject it names has a figure behind it - asserted, because a subject with no metric would make the rule report a sentence nobody could repair. Three sentences were reworded rather than cited, since their figures count a witness set or a chosen subset rather than the tree, and **J12's own row fired on itself for the second time** by quoting the figures it forbids. What the sweep did NOT cover is stated: the register only, a vocabulary of fifteen subjects, and rule H2's `Thirty-two exclusions` and `two bundles` left standing and named. **The ledgers and every evidence bundle are swept by [Bundle JS-ANDROID-018](evidence/js-android-018/README.md)**, which found three live claims stale and explains why almost nothing else it flagged is a defect: a bundle is dated and immutable, so a figure it states was true on its collection date, and the 107 bundle hits are false positives by construction - the log keeps the markers rather than filtering them, because what a naive comparison produces is itself the finding. The three were both of the profile ledger's limit claims, which UNDERSTATED the evidence - it said two composition roots where three JavaScript roots are registered and one recorded runtime identifier where `android-x64` is recorded beside `win-x64` - and the core ledger's line count for the API baseline. **The ledger cannot be ruled the way the register is**, and that is stated rather than attempted: seven of its ten hits are the ledger QUOTING the defects it reported, and no mechanical sweep tells a stale claim from a report of one. **The ADRs are swept by [Bundle JS-ANDROID-019](evidence/js-android-019/README.md)**, the third document that bundle named, and they are a third KIND of document: revised in place, so live, but most of their figures are the decision itself rather than a measurement - `exactly three packages` is the budget the record sets, not a count that can rot. It found one defect, made in this same session. **ADR 0001's budget section authorises the project set** - it says the set may not grow without a dated revision, and every project-adding revision states the growth - and the revision that added the Android composition root carries no budget paragraph at all, so the record's last stated size stayed one revision behind the graph. Rule A7 could not see it: it holds the manifest to the project files and both of those ARE the tree, so a document describing them is prose no rule read. **Rule A15 reads the record's LAST budget sentence and compares it to the graph**, treats a record stating no budget as a violation rather than as nothing to disagree with, and leaves every earlier sentence alone as history. The missing revision is APPENDED rather than folded into the one above it, because a record whose author edits yesterday's entry when today's sweep finds it short is a record with no history. **And A15's own row tripped rule J12 on first write**, which is the fourth time a rule's row has fired a rule in this chain: a rule that forbids a sentence shape cannot use that shape to explain why it exists, and the repair has been reported speech every time. That bundle said the thing to reconsider at a fifth occurrence was the recogniser rather than the prose, and **[Bundle JS-ANDROID-020](evidence/js-android-020/README.md) reconsiders it at four**. Rule J12 reads shapes, and an assertion about the tree and a QUOTATION of one have the same shape; a figure is exempt now when it is inside a code span AND attributed by a reporting verb within 72 characters, so a row can show the sentence that was wrong instead of paraphrasing it away. **The exemption needs both halves because it is the one place in the rule where a miss is unsafe** - everywhere else an unrecognised phrase costs a false report, here a verb wrongly matched lets a live figure through - and `read`, `carries` and `names` are deliberately absent from the verb list, being ordinary words in this register. It is an escape hatch and the bundle says so: a row could quote a live claim to evade the clause, and what the design buys is that the abuse must be deliberate where the paraphrase route was available by accident. Five sentences got their evidence back, and **one still had to be reworded because it used the bare verb `say`, which the fix was to replace rather than to widen a list that grants exemptions**. THE RESULT THAT MATTERS: the exemption can only weaken these rules, so all eighteen controls minted for J12 and A15 were re-run, and all eighteen still fire. **The exemption has controls of its own since [Bundle JS-ANDROID-021](evidence/js-android-021/README.md)**, over the real register rather than over a witness: a code span with no verb, a verb with no code span, and a live figure standing BETWEEN the verb and the span it attributes - the last one testing the implementation rather than the design, since blanking the whole match instead of the quoted group alone would swallow it. **A fourth injection is labelled a limit demonstration and not a control**, because it is deliberately not caught: it presents a live figure as a quotation and the suite stays green on a stale number, which is the escape hatch measured instead of asserted. That measurement changed the reporter. **What the exemption lets through is no longer silent**: the report lists it on its own line, eight quotations today, so J12's own count stays a count of things wrong and a reviewer auditing the hatch has a list rather than an absence. The hatch is not closed and cannot be - no rule tells a quotation of a defect from a quotation smuggling a live claim - and the trace is a report rather than a gate. It covered 46 at [Bundle JS-ANDROID-010](evidence/js-android-010/README.md) — groups A, B, K, N and V, every group whose rules are functions returning the messages they would report. **The other thirty assert inline**, the rule being the test body rather than a function a reporter can call, and giving them one means extracting a function per test with the risk that the extraction and the test drift apart — the defect the reporter exists to prevent, so it is named as separate work rather than attempted. What remains open is stated rather than closed: `E5`; a message is what a rule said rather than proof the rule is right; and every suite-control run retained here was scoped to the rows in question rather than to the twenty-nine. Decision [JSD-0008](decisions/0008-format-version-1-the-entry-point-and-what-js-1-corrected.md) records the entry-point answer and four corrections to earlier records. | **Two things, and the second was found rather than carried.** (1) **The exit-gate clause JS-1-001 carried is discharged**, at JS-3a rather than at JS-3b. The obstacle was real — `ApiSurface` describes a surface by loading an assembly, which needs a project reference, which rule A11 forbids a test project to have on a profile — and the first of the two routes the bundle named is now taken: rule N10 describes the family from its build output with `MetadataLoadContext`, which reflects **without running anything**, so it needs neither the reference A11 forbids nor the execution invariant 2 forbids. Bundle JS-3A-002 retains it. The clause moved because JS-3b is blocked on JS-2 and this needed neither; [JSD-0012](decisions/0012-the-profile-api-baseline-and-where-its-clause-lives.md) records that. (2) **Roadmap [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier)'s third discipline was never implemented, and no bundle had said so.** The section names three orderings and asks that they be asserted mechanically *for every corpus entry, including every failing one*; JS-1-001 observed one ordering, and it is a different one. [Bundle JS-1-002](evidence/js-1-002/README.md) lands them, and grew the corpus by the entry the third discipline needed — which is the step from JS-3a's count to the one JS-9 is seeded from, recorded here because a reader tracing the corpus across three rows would otherwise meet an unexplained increment. **JS-1 is still not accepted**, because no reviewer decision exists.  `Suspended` is declared unreachable and produced at JS-7; five descriptor rows are provisional pending JS-5's measurements; one RID, one machine. |
| [NONE] | **JS-2 — seeding snapshot and front-end ingest** | **Blocked** (recorded as `Not started` above the blocker, because no work has begun either) | None. No snapshot has been taken. The candidate identity in roadmap [section 4.1](roadmap.md#41-the-snapshot-identity) is a recorded candidate, not a taken snapshot. | **Blocked on two named external dependencies.** See section 3. |
| [PARTIAL] | **JS-3a — diagnostic registry, position encoding, pinned suite, the oracle** | **In progress** | [Bundle JS-3A-001](evidence/js-3a/README.md), which is **the registry half of this milestone and not the oracle half**. [`docs/diagnostics/registry.txt`](diagnostics/registry.txt) is published at revision 1, one row per code, each naming the member that declares it, **the one core reason every emission carries**, the stage that refuses, **which half of the registry it belongs to**, the case that reaches it, and the revision its meaning dates from. **Five rules, N5 through N9, bind it to four independently written artefacts** — the code vocabulary, every emission site in the profile assembly, the retained corpus, and the composition's deliberately restated constants — plus the position factories, so no one edit can make it agree with everything. The corpus grew from 51 to **59 entries** to close the backward binding, and **37 of the 40 rows are reached by a named entry**. Decision [JSD-0009](decisions/0009-the-diagnostic-registry-and-the-position-encoding.md) records the registry, its two halves and the position encoding; the encoding is pinned by four corpus rows through a new manifest column, and landing it **corrected a conflation in which every link- and walk-stage diagnostic reported a code-section offset under the artifact-relative marker**. `EntryStackNotEmpty` was declared at JS-1 and emitted by nothing; it is refused on the edge now, which also removes an order-dependence in which code an artifact provoked. **Twenty-two negative controls**, seven of them judged by the corpus rather than by the suite. **And one clause that was not this milestone's** — the public API baseline, open since JS-0 and parked at JS-3b behind two blockers it did not need — is discharged here by rule N10 and a baseline of the family's own, retained as [Bundle JS-3A-002](evidence/js-3a-002/README.md), per [JSD-0012](decisions/0012-the-profile-api-baseline-and-where-its-clause-lives.md). **And the oracle half exists from 2026-09-03**, retained as [Bundle JS-3A-004](evidence/js-3a-004/README.md). `Broiler.VM.Composition.JavaScript.Conformance` is a composition root that is NEVER ADVERTISED - roadmap [section 5](roadmap.md#5-package-boundaries-and-the-dependency-graph) says the harness can be nothing else, because scoring a test means driving this profile own lowering, verifier and executor and rule A11 forbids a test project to reference a profile assembly. It carries a suite reader whose pin is a digest it recomputes on every read, a selection pipeline recorded stage by stage, content-independent FNV-1a sharding, per-host-mode totals over **script, module and raw**, a merge that proves the shards covered the selection before it adds anything, the six named configuration failures, a completion-kind classifier on every result, a ratchet, and **its own regression suite of 25 checks that composes no profile at all**. **The self-check runs before every shard**: nine fixtures whose declared verdicts the harness must reach, three of them controls that must pass, and a mismatch stops the run on an exit code of its own before anything is scored. **Seven negative controls, each injected, caught and reverted** - a harness that treats any refusal as any other, one that scores everything as a pass, one that shares a runtime between cases, an edited fixture under an unmoved pin, a merge missing a shard, and the two ingestion-path injections. **Rule N13** asserts that the harness and any suite it reads appear in no package and in no advertised composition closure, and it is deliberately NOT phrased as `no published closure` because this root publishes one of its own ([JSC-40](roadmap.corrections.md#jsc-40)). [JSD-0015](decisions/0015-the-conformance-oracle-and-what-it-refuses-to-score.md) records the seven answers the method needed, and the graph goes from 20 projects and 59 edges to 21 and 64 with the packable set unchanged at three. [Bundle JS-3A-005](evidence/js-3a-005/README.md): **the ingestion path**, without which the harness could not have read a real suite at all - five suite-shaped files were refused five times and the run scored nothing ([JSC-53](roadmap.corrections.md#jsc-53)). A reader for the dialect as written; the two strictness readings of one file; and **the rule that a refusal answers a question about the language only when it was a language answer** ([JSC-54](roadmap.corrections.md#jsc-54)), without which this manifest's ordinary refusal would have scored a suite's negative tests and reported a near-perfect total. 42 harness checks, up from 25; a second pinned suite of 12 candidates in the ingested dialect with 8 self-check fixtures; **8 negative controls, each injected, caught and reverted, retained unedited**. [JSD-0016](decisions/0016-ingesting-a-third-party-suite-and-the-refusals-that-answer-nothing.md) records the four answers the path needed. No project is added and the graph does not move. | **The oracle half is built and the gate is still open, and the sentence this row carried until 2026-09-03 is now wrong in a way worth stating: it said the half was untouched.** What is untouched is the part that needs a suite nobody has retrieved. **The harness scores this component own fixture trees and nothing else - two of them since 2026-09-03, one in each dialect, and both written here**, which roadmap section 14 asks for in those words - build against the smallest scoring target that exists - and which means every total in Bundle JS-3A-004 is a statement about the instrument rather than about JavaScript. **The forty-four declarations and the code they judge have one author**, so their agreement is internal consistency and an external oracle is exactly the thing this checkout does not have. **The retained JS-1 corpus, which section 14 names as part of the scoring target, is scored by nothing here**: the fixture tree drives the same verifier and executor, and reading the corpus manifest as a raw suite is available work that is not done. **Two of the four completion kinds and the whole `fault` expectation kind are reached by no fixture**, because this manifest admits no promise, no generator, no asynchronous function, no `throw` and no error objects; the classifier is exercised by recorded marker sequences instead, which is a reason to test it rather than a reason to leave it unwritten. **One machine, one RID**: everything was measured on `win-x64`, and the CI lane runs the harness on the whole matrix from this change onward with no run of it existing when the bundle was collected. **The floor is set and holds and accepts nothing** - admitted is not accepted, and a ratchet is a measurement discipline rather than a status; the ingested-dialect suite has a floor of its own, which guards the one thing an exit code cannot, because a case that stops being scorable is reported skipped and a skip is not a failure. **The ingestion path is exercised against files written here, which proves the path runs and predicts nothing about what a real suite would score.** Its twelve candidates were chosen to reach each arm of the translation once; a real checkout is five thousand times larger and its distribution is unknown, so the eight declines and four scored cases are a shape and not a forecast. **No runtime-negative case is exercised by either suite**: the translation for one is written and checked, but this manifest reaches none of the three fault kinds from source, so that arm is declared and not run. The suite-revision dependency in section 3 is still open and a human has to retrieve, hash and archive the suite before it can close; **the ingested suite's attribution row and the core's standing-claim confirmation travel with that ingestion and are open with it** ([JSC-30](roadmap.corrections.md#jsc-30)). Within the registry half: **three rows are reachable from no artifact**, named and reasoned about in JSD-0009 with the admitting list held in rule N7 rather than in the registry; **no `embedder-seam` code exists**, because the front end that would mint one is JS-3b's, so that half of the split is declared and not exercised; **four corpus rows of sixty-six pin a position, and the three that name a section are DERIVED since 2026-09-02** ([Bundle JS-3A-003](evidence/js-3a-003/README.md)). Bundle JS-3A-001 gave two reasons for not pinning more - a hand-computed offset is a number no reader can check, and asking the verifier records the answer under test - and both are true and neither is exhaustive: the producer computes the position from ITS OWN construction, the section it put the defect in and the table it wrote, and the replay compares that against what the verifier computed from what it READ. The hand-computed strings stay as the answer the derivation must reproduce, and **the producer refuses to write a corpus when the two disagree**; three controls break one side each - the human's, the producer's, the artifact's - and it refuses every time. `wrong-magic` is not derived and the derivation REFUSES to express it, because its refusal names no section. **The corpus regenerated byte-identical**, which is the evidence: three human answers reproduced without moving a byte. **No new row was pinned, and that is the finding**: pinning one means knowing which byte its refusal reports, and JSD-0009 defines what the fields mean without defining, per stage or per diagnostic, which byte a refusal is about. Writing that convention down is a decision record and is the next piece of work; pinning rows before it would encode whatever the verifier happens to do today as though it had been decided. The count here was stale until this bundle - it read `fifty-five pin none`, from when the corpus held 59 entries rather than 66 - and the register sweeps did not catch it, `corpus rows` not being one of the fifteen countable subjects their vocabulary reads. One RID, one machine. |
| [NONE] | **JS-3b — static semantics as one verification stage, and the lowering** | **In progress** | **No retained bundle. What this row records is observed repository state under section 1's third category, and it satisfies no gate.** The checkout holds a source front end for `broiler.javascript.slice`: a tokenizer producing one token array in one pass, a syntax tree, a recursive-descent parser, **one** validation stage carrying every early error, and a lowering onto JS-1's instruction buffer. [JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md) records the five answers roadmap [section 9](roadmap.md#9-the-semantic-front-end-and-lowering) asks for, three of which the plan carried as **open** until there was a front end to settle them against: the verifier re-derives no early error because a refused source produces no bytes; a doubly-bad artifact gets the framing answer, and not as a tie-break, because static semantics is a property of a tree and a doubly-bad artifact has none; strict mode is **recognised** by the tokenizer and **ruled on** by the validator, which is what deletes both of the seed's source re-scans rather than reimplementing them; parse options are a value and rule **N12** asserts the assembly holds nothing that could outlive a call; and deep nesting is an explicit bound that **refuses**. The retained corpus grew from 66 entries to 91: **25 of the 41 controls are compiled from JavaScript text**, and their source is retained beside their bytes, so a claim like *`10 - 3 - 2` is 5* is now checkable by a reader rather than only by a builder. **31 sources are refused by name**, in a second retained manifest at `src/tests/corpus/js-1/source/source.manifest`. The registry is at revision 2 and its `embedder-seam` half — declared and empty since JS-3a — is 22 rows ([JSC-44](roadmap.corrections.md#jsc-44)). **Six front-end checks run in the producer composition**, including the concurrent two-goal case section 9 asks for and a nesting case at 100,000 levels that is refused rather than overflowing a stack. **The parser reads JavaScript rather than the manifest since 2026-09-03** ([JSC-49](roadmap.corrections.md#jsc-49)): it was refusing a `function` as an unparseable reserved word, which put the manifest's boundary in the pass that owns the grammar - contradicting this front end's own first decision - and left it unable to READ the language. It now produces a node for every construct it recognises and refuses only what is not a tree; `SliceManifest` holds what is admitted and the validation stage refuses the rest BY NAME AND WALKS INTO IT, so one source yields one diagnostic per occurrence. Every retained corpus entry replays to the same answer and the artifact bytes did not move, which is the evidence that a grammar this much larger changed no lowered behaviour. | **Nothing here is closed, because no bundle has been collected.** Under section 1, observed repository state can explain a status and satisfies no gate, and every clause below is open on that ground alone. Beyond it, two clauses need something this component does not have: **the parse-and-early-error slice is scored against no ratchet over a real suite**. **This cell said "scored on no harness, because JS-3a's oracle half is untouched" until 2026-09-03, and both halves of that were out of date**: the harness landed in [Bundle JS-3A-004](evidence/js-3a-004/README.md) and the row was not revisited, which is the ledger's own update rule going unapplied rather than a new fact. The position now is narrower and worth stating exactly. The harness exists and scores. The ingestion path exists, and the arm this clause depends on is the one that makes the slice scorable at all — a negative test whose declared phase is `parse` never executes, so the assertion library it would have needed is never reached, and the question it asks is one this front end genuinely answers ([JSD-0016](decisions/0016-ingesting-a-third-party-suite-and-the-refusals-that-answer-nothing.md)). **This clause is MET as of 2026-09-03.** It read *what is missing is the suite: a ratchet is over a pinned revision, nobody has retrieved one*. The suite is retrieved, pinned in this repository where the checkout cannot reach the pin, archived as the archive it came as, and **a floor is set over 1,063 Script and 21 Module cases and checked once per lane invocation** — see section 3's row and [JSD-0020](decisions/0020-the-retained-conformance-suite-pin-and-the-one-it-replaces.md). The slice is no longer scored only against fixtures written here. And **the narrow-runtime-compiler composition publishes and runs on ONE RID and the gate asks for every claimed one** — the label was claimed at JS-1 for a root that carried a lowering, and JS-3b is the milestone whose gate makes it a real claim over a real source surface. **This cell read "has no publish-and-run on any RID" until 2026-09-03, and the composition it named did not exist then** *(corrected: [JSC-56](roadmap.corrections.md#jsc-56))*: the register recorded that no root here held the label, and what the label waited on turned out to be narrower than a source surface — a composition **handed source from outside its own image**, which every root here lacked because each reads its input from inside one. `Broiler.VM.Composition.JavaScript.Cli` is that composition, the end-user host: a path on a command line, compiled, verified, run, with the completion value printed and seven exit codes two of which accuse this component. Its catalog table prints the label where every sibling prints `narrow-runtime-compiler-shaped`, and **its closure is the first here a reader can compare against roadmap section 15's row without a paragraph of exceptions** — it carries no corpus replay, no corpus writer, no fuzz mutator, no soak and no conformance harness. It is judged over **31** command lines by a driver that patches nothing and reaches for no internal: the inputs are files and the subject is the built binary. **This cell read `eighteen command lines` until 2026-09-03 and was three changes stale**, the suite having grown by thirteen as the repairs below were pinned in it; the figure is the driver's own, printed on every run. **What is left of this clause is the matrix**, which is a collection and not a composition — one machine, `win-x64`, three publish modes — and the component lane runs the host on every cell from this change onward with no run of it existing when the bundle was collected. Three further gaps are this milestone's own and are named rather than left to a reader: **the front end WAS fuzzed by nothing and is fuzzed from 2026-09-03** *(corrected: [JSC-69](roadmap.corrections.md#jsc-69))* — a session over the source surface in the slice-compiler root, where it has to be because the execution-only image carries no lowering. Its assertion is not that nothing crashed but that **a source this front end compiles produces an artifact this verifier accepts** — the seam all three of this milestone's unreachable-code defects lived in, each found by a third-party suite rather than by anything here. Four sessions of 25,000 iterations: no finding, about 26,000 mutants a session compiled and verified, 23 of the 24 seam codes primed by the corpus and 21 reached by mutation. **The guidance kept nothing across 100,000 mutants**, which is the honest reading: what the session adds is the seam invariant over tens of thousands of compiled programs, not exploration; **the negative controls are two, and two is not a control per claim** — both are source entries whose refusal code the front end must produce, each written after the defect it names was found and each watched failing on the injected tree and passing on the reverted one. A leading string literal was taken as a directive whatever followed it, so `"use strict" + 1` **enabled strict mode for a program that never asked** and then failed on the `+` with a syntax error; and a `for` head's scope collected the head's `var` names and not the body's, so `for (let i = 0; ;) { var i; }` was legal here and an error everywhere else. Every OTHER check in this row is a check nobody has watched fail, which this component's own discipline says is a shape rather than evidence; and **the accepted programs and their recorded answers were written by the party that wrote the code they judge**, with no oracle anywhere, so they are this component's claims about JavaScript and not conformance. The general front end — functions, objects, strings, `try`, modules, regular expressions — is JS-2's ingest and is blocked with it. |
| [PARTIAL] | **JS-4 — value representation and object model** | **In progress** | [Bundle JS-4-001](evidence/js-4-001/README.md), which demonstrates **some** of this gate and names every unmet clause in its own exclusions. From 2026-09-04 the milestone owns code — `JsValue.cs`, `JsObject.cs`, `JsArray.cs`, `JsFunction.cs` and `JsNumberFormat.cs` — and every line of it was written in this checkout rather than copied. `JsValue` is a tag, a double and a reference, and **what that costs against the eight bytes a production engine spends is stated in the type's own remarks rather than hidden**. Own-property order is the specification's rather than a hash table's. An Array's elements are a dense list with a sparse fallback past a growth threshold, and **a hole is a distinguished empty value rather than a stored `undefined`**. `JsNumberFormat` implements `Number::toString` and `StringToNumber` from the specification rather than delegating to .NET, whose `R` format disagrees with the language at the edges. **The entry gate is unchanged and the code takes its answer**: [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md) answered the representation with *replace*, and this profile keeps its own tagged struct rather than adopting the seed's boxed hierarchy. What exercises the model is the two workloads the subsection below records — a benchmark that prints a score, and runs of the pinned test262 checkout over subtrees somebody chose — whose figures are in the bundle and not here. **The wide front end this model is reached through was WRITTEN IN THIS CHECKOUT rather than ingested**, exactly as JS-3b's slice front end was, so **nothing here unblocks JS-2** and nothing here is inherited material under section 1's fourth category. **The plan is unmoved by that**: [the copy table](roadmap.md#43-the-copy-table) still gives this milestone the storage half with its tests and this milestone's own `Seed` row still says so, and none of the corrections this change mints speaks to the seed at all — what changed is what the checkout contains, and the next column is what that leaves open. The manifest this model is reached through is [JSC-70](roadmap.corrections.md#jsc-70)'s and decision [JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md)'s. | **Every clause of the gate is open, and these are the ones a reader would otherwise assume closed.** The copy this milestone's `Seed` row describes — shapes, the transition table, element arrays, the named-property store, each with its tests and its recorded defect history — **has not happened**, so the regression the gate asks for per recorded defect has nothing to be written against; what exists instead is a model written here, which is unreviewed code written here rather than unreviewed code copied here, and both count. **No retained figure per value kind exists** under [section 17](roadmap.gates.md#17-measurement-discipline)'s rules, because no measurement lane exists at all and JS-10 owns it. **Nothing retained shows the two-runtime clauses** — key identity realm-scoped, one runtime observing neither the other's storage nor its shapes, the structural scan over a shared handle — and nothing retained shows JS-1's hand-written encoder and its hand-written programs deleted, which this gate asks for and asserts by scan. One machine, one RID, and **Native AOT was not published on the machine this was written on**; the component's lane is the authority for what publishes and it retains nothing, so what is open there is a collection. No reviewer decision exists and nothing here is accepted. |
| [PARTIAL] | **JS-5 — executor, abrupt completion, budgets** | **In progress** | [Bundle JS-4-001](evidence/js-4-001/README.md) — the same bundle, demonstrating **some** of this gate and naming every unmet clause in its own exclusions. An interpreter exists from 2026-09-04 (`JsEngine.cs`, `JsRealm.cs`, `JsExecution.cs`): the abstract operations, the prototype chain, accessors, the ordinary internal methods, and a dispatch loop that **charges fuel per instruction and polls within the bound the descriptor declares**. **A JavaScript `throw` travels on the CLR's own exception mechanism**, and each frame catches it and looks for a region covering the instruction that was executing — abrupt completion is something this executor answers rather than something format version 1 reserved and refused. **A realm is per-instance and an instance outlives one invocation**, which is what makes several scripts one program in one realm and what running several named files in order rests on. **What it executes is a second format version**: version 1's framing, its magic and its one-opcode-plus-fixed-operand rule, plus a `Functions` section declaring one code unit per function with its parameter count, its environment-slot count, its operand-stack maximum, its code range and its flags; constant tags for `String` and `Null` beside version 1's, with the `InternedName` tag version 1 reserved now admitted; exception regions that carry a scope depth and an operand-stack height; and **absolute branch targets rather than version 1's displacements**, because all code units share one code section and an absolute target is checkable against the range of the unit that contains the branch. Bindings are addressed by a (depth, slot) pair into a chain of environment records, and nothing addresses a variable by name at run time except a global, which is a property of an object. **A version-2 verifier holds version 1's discipline and adds two rules**: code units must **tile the code section exactly**, disjoint and ascending with no gap, because two overlapping ranges make every per-unit branch check meaningless; and the abstract pass carries a **scope depth** beside the operand-stack height, so a `PopScope` past the frame and a join at two different depths are both refused, with a handler seeded as a second entry into its unit at the height and depth its region declares. **Two of the five descriptor rows JS-1 recorded as provisional pending this milestone's measurements are measured**, `cancellationPollBound` and `maxUnchargedWork`, and they are measured **by construction rather than by benchmark**: the bounded reader charges one work unit per byte consumed and polls after each charge, so a charge larger than the declared bound is reported as a poll-bound violation — which is what a bulk code-section read produced while the provisional figure stood — and the verifier now reads every bulk run in windows no larger than the bound, so the declaration and the behaviour are two statements of one fact. The figures are the descriptor's and the bundle's, not this file's. **The retained corpus is no longer of one format version**: it gained malformed version-2 artifacts, one per structural refusal version 2 adds, two in which the caller mislabels the bytes, and one whole version-2 program — a closure called through a property of an object — that verifies, instantiates and completes with a value. The diagnostics registry is at revision 3 and each new code is bound to one of those entries; **two rows that had been `defensive` since revision 1 are `corpus` now**, because while this profile registered one format version and admitted one manifest the core screened the descriptor against a set with one member in it, so a descriptor mismatch could not be observed — registering a second of each made both reachable. **The budget declaration matrix's `HostCalls` row moved from `NotApplicable` to `Charged`**, because one optional host capability import now exists. **This executor recurses on the CLR stack, which the plan's own reasoning assumed it would not**: section 8 says the depth bound is promisable *because a frame is a heap object rather than a CLR frame*, and one JavaScript call here is three C# calls. **A conformance case that recurses a hundred thousand deep terminated the process** before any ceiling reached it, which is the stop condition [gates section 21](roadmap.gates.md#21-format-and-verifier-safety) names in those words, and it was found by pointing the pinned suite at the host rather than by anything here. What repairs it is that **the profile runs a guest invocation on a thread whose stack it declares** rather than on whatever stack the caller had, so the bound means the same thing on every host; a recursing program is refused as a resource exhaustion naming `CallDepth`, and the engine's own ceiling sits above that as the answer for a host that granted more depth than the declared stack holds. **The per-frame cost is not measured and the declared stack is chosen against an estimate of it**, so the discipline [JSC-72](roadmap.corrections.md#jsc-72) satisfied for two rows is not satisfied here ([JSC-79](roadmap.corrections.md#jsc-79)). **The corrections carry what the plan said before, each on decision [JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md)**: the manifest this executor opens ([JSC-70](roadmap.corrections.md#jsc-70)), the second format version and what the version-1 reader's declared maximum means ([JSC-71](roadmap.corrections.md#jsc-71), [JSC-77](roadmap.corrections.md#jsc-77)), the two descriptor rows and how the plan said they would be chosen ([JSC-72](roadmap.corrections.md#jsc-72)), the `HostCalls` row ([JSC-73](roadmap.corrections.md#jsc-73)), the two registry rows ([JSC-74](roadmap.corrections.md#jsc-74)), and one corpus holding entries of both versions ([JSC-78](roadmap.corrections.md#jsc-78)). | **Open, and the gate's largest clauses are the ones nothing here touches.** **`CallDepth` is not measured**: the gate derives its default from a retained frame-cost measurement on each claimed RID under Native AOT, and this is one machine, one RID, with **Native AOT not published on it at all** — the lane is the authority for what publishes and it retains nothing, so what is open there is a collection. **Nothing retained shows a proportionality fixture for any operation family**, and the family that was to arrive with the library arrives approximated rather than implemented, which the subsection below names. **There is no suspension** — `Suspended` is still declared unreachable and JS-7 owns it — and **there is no job queue**, so an asynchronous test262 case cannot complete whatever the front end does with its syntax. Nothing retained shows the nested-handler and `finally` matrix in both directions across the host boundary, the binding-time refusal of a capability whose version, signature or kind does not match, or a deliberately non-charging variant being detected. **What the corpus gained is one entry per structural refusal the format adds, and the gate asks for entries per new opcode covering its structural, index and stack-consistency rejections** — the second list is not the first and nothing here shows it. The descriptor rows JS-1 left provisional that are not named above are still provisional. No reviewer decision exists and nothing here is accepted. |
| [PARTIAL] | **JS-6 — the standard library** | **In progress** | [Bundle JS-4-001](evidence/js-4-001/README.md) — the same bundle again, demonstrating **some** of this gate and naming every unmet clause in its own exclusions. A standard library exists from 2026-09-04, and it is **the rewrite this milestone was re-scoped to be rather than a copy re-typed**, which is [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md)'s answer arriving as [JSC-17](roadmap.corrections.md#jsc-17) said it would: `JsRealm.Object.cs` and its siblings carry Object with the full property-descriptor surface, Function with call/apply/bind, Array, String, Number with `toFixed`, `toExponential` and `toPrecision`, Boolean, Error and its native subtypes, Math, JSON, Date and RegExp, plus the global functions and the host-defined `print` and `$262`. **`print` reaches a host through one optional capability import**, `broiler.javascript.write`: a composition that registers nothing still creates a runtime and still runs programs, and what it does not have is a `print` that reaches anywhere. **Three of these surfaces are approximations, and the bundle does not let them pass as anything else.** RegExp is translated to `System.Text.RegularExpressions`, which the file that does it declares as an approximation, and **the Octane RegExp benchmark runs and fails its own checksum** — an approximation costing what an approximation costs, since it neither refuses nor agrees. **`Date` fixes the local time zone to UTC.** **`arguments` is unmapped.** **The exclusion list this milestone must publish on the day it lands is the subsection below**, which names every construct the wide manifest refuses and every approximation it makes; a rewritten library is smaller than a copied one and the difference is a support claim. **The plan's route for regular expressions is not replaced and is not what shipped**: it still reads a from-scratch matcher acquired as this checkout's own dependency, or the surface excluded by name under `broiler.javascript.regexp`, and what exists is a translation inside `broiler.javascript.wide` — which [JSC-70](roadmap.corrections.md#jsc-70) records only as the second of its two reasons this manifest could not take the `core` name, on decision [JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md). | **Open, and one clause is now worse than open.** Section 3's satellite-acquisition row is still unacquired, and what shipped is neither of the two routes that row and this gate anticipated: not the from-scratch matcher and not the published exclusion, but an approximation inside the wide manifest, and the one third-party benchmark in the bundle that exercises it **runs and fails its own checksum**. The Unicode and locale halves are unacquired too. **The gate's ported-test clause needs the seed**, and JS-2 is blocked: nothing was ported, so there is no covered list, no excluded list and no justification per exclusion of the kind the gate asks for, and the subsection below is an exclusion list rather than that record. Nothing retained shows the metadata tests the gate asks for — no IL-emission assembly and no compiled-mode regular-expression construction in the closure — which a translation onto `System.Text.RegularExpressions` makes load-bearing rather than moot. **`broiler.javascript.wide` has no retained run of its own over the pinned suite**: what exists is runs over subtrees somebody chose, so roadmap [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)'s rule is unmet. One machine, one RID, no Native AOT publish on it, no reviewer decision, and nothing here is accepted. |
| [NONE] | **JS-7 — suspension** | **Not started** | None. The continuation design needs no copied code and may be opened early. | After JS-5 and JS-6. |
| [NONE] | **JS-8 — guest-initiated loads and the three compositions** | **Not started** | None. No guest-load declaration, no mediator adapter, no composition registers a provider or declines to. | After JS-7. |
| [PARTIAL] | **JS-9 — adversarial input, agents, soak** | **In progress** | [Bundle JS-9-001](evidence/js-9/README.md): a **seeded mutation fuzz target over two of roadmap [section 7](roadmap.md#7-the-bytecode-format-and-the-verifier)'s four surfaces** — the verifier, and the executor over verified-but-adversarial artifacts — **which is not the guided target the section asks for**: it draws every mutant from the fixed retained corpus and takes no feedback from what a mutant reached, and this row and the bundle's own header called it coverage-guided until 2026-09-01, when both were corrected and no retained log was edited ([JSC-38](roadmap.corrections.md#jsc-38)). The target in the checkout takes feedback now and the retained sessions predate it, which the closing column states. Four retained sessions of 25,000 iterations each, seeded from the 60-entry manifest, with **no counterexample**: about four thousand mutants verified and were instantiated and invoked, and every fault the executor produced carried this profile's own typed payload. A session is a total function of its seed and its seed corpus — no wall clock, no thread count — and **a session that answers the same way every time, or that never reaches the executor, exits non-zero** rather than reporting clean iterations it did not earn. One fuzz control: the verifier's constant-index check removed, found at a named iteration, reverted. **[Bundle JS-9-002](evidence/js-9-002/README.md) adds the two host-level exercises**: two runtimes under one aggregate budget spend one total — 28 invocations completed and 100 refused, the parent spending exactly its allowance, and **which sibling was refused is deliberately not asserted** because the order is a race; disposing a parent with a live child is refused and accepted after; a sealed parent admits no further runtime; and a soak of **2,000 create-run-dispose cycles** reaches a heap plateau **under JIT and trimming and NOT under Native AOT**, where the same check on the same code grows by a factor of 2.30 against a band of 2.0 and the run exits 1. **Both bundles' READMEs summarised that run as “6 runs, all exit 0”, which their own `publish-and-run.log` contradicts**; each now carries a dated correction and the logs are unedited. **[Bundle JS-9-003](evidence/js-9-003/README.md)** adds the last clause of the gate that needs nothing unbuilt: a **mutated corpus entry** — one byte of a control entry and one of a malformed entry — is detected by the replay, which reports the changed triple *and* the hash mismatch, and is restored byte for byte. Every other control in this component injects into source; that one injects into the retained bytes, which is the direction that would otherwise be taken on trust. | **Two of the four surfaces are not fuzzed, and since 2026-09-03 only one of them is absent.** The regular-expression matcher does not exist, and waits on JS-6. **The source tokenizer and parser now EXIST and no session reaches them**, which is a different admission wearing the same sentence: this row said both were unfuzzed BECAUSE ABSENT, and for one of them that stopped being true the day JS-3b wrote the front end ([JSC-47](roadmap.corrections.md#jsc-47)). A session may not be read as covering either. **And the retained sessions are evidence over a different population again**: they were seeded from a 66-entry manifest and this checkout holds 91, so under update rule 5 they recertify nothing about the corpus as it stands and the next collection re-runs them. **The guidance clause is built in the checkout and is not closed.** No session was guided at all until 2026-09-01: the mutator drew from the retained corpus and nothing a mutant reached fed back into what it drew from next. It does now — a mutant whose published answer no seed artifact produces is kept as a further seed, the pool opens as the retained corpus and grows, and the declining host rotates through one vector per exhaustion dimension rather than tightening four at once, which is what lets a session reach the three arms it could not and attribute the four it could. **The signal is the answer this profile publishes and not an edge**, so two paths to one answer are one signal and a defect on a path that answers like its neighbour is invisible to the guidance; instrumenting for anything finer would put a coverage host in a published closure or change the assembly under test, and [JSD-0013](decisions/0013-the-fuzz-sessions-coverage-signal.md) records the refusal and what would falsify it ([JSC-42](roadmap.corrections.md#jsc-42)). **What a session judges about itself is its loop and not its growth**: how much a seed set grows is a fact about the corpus as much as about the mutator, so a session fails when it offered fewer mutants to the pool than it drew, and the composition asserts separately that the pool keeps a new answer and refuses a repeat. **Retained sessions are guided since 2026-09-02** ([Bundle JS-9-004](evidence/js-9-004/README.md)): four sessions of 25,000 iterations each, every one reporting what it kept. The four in [Bundle JS-9-001](evidence/js-9/README.md) were collected before any of this and their logs stay unedited. **Collecting it found the harness describing sessions that no longer existed** - the header written into every retained `fuzz.log` still said `NOT coverage-guided` and `no session in this log closes it`, four lines above sessions printing what they had kept, so a collection run before the fix would have retained a log denying its own output. That header has now been wrong in BOTH directions, having once claimed coverage-guidance the mutator did not have, and it says so. **And the yield is one or two answers per session**, which the bundle puts first rather than leaving a reader to assume: 25,000 iterations move the pool from 66 artifacts to 67 or 68, because the signal is the published answer and this profile's vocabulary of answers over a slice-scope corpus is small. Guidance being present is not guidance being effective, and only the former is claimed. **The corpus is still slice-scope, not full-format**; there is no retained-bytes report over an object model that does not exist; no agents; and no session or soak budget — the seeds, the iteration counts and the cycle count are stated so a run is reproducible, not because any of them is a number something justifies. The soak's plateau is a band and not a measurement. **And a second false signal in the same check was found on 2026-09-03 and removed** ([JSC-48](roadmap.corrections.md#jsc-48)): the plateau reading was coupled to what the process allocated BEFORE the soak, because two forced collections in a whole run leave the final number holding heap the collector has not returned. Growing this profile's retained corpus from 66 entries to 91 was enough to push it past the band on macOS under Native AOT, on both architectures, byte-identically across three runs, while `win-x64`, `linux-x64` and the Android head stayed flat. The check samples the heap sixteen times across the run now and prints the curve; **no band was widened and no threshold moved**, and the sampling makes the check stricter rather than kinder because a real leak grows live bytes whatever the collection frequency. **It closes nothing** — the plateau clause needs a retained bundle and none has been collected since. **The Native AOT failure this row carried is diagnosed and the check is corrected**, on 2026-09-01: it was **not** a per-cycle retention. Running 2,000, 8,000 and 16,000 cycles produced a final heap identical to the byte — eight times the work, same heap — and sampling out to 20,000 cycles showed one step and then a heap that did not move for 19,500 consecutive cycles. The growth is **one-time warm-up**, and the baseline was read before it finished: under Native AOT the heap settles at about cycle 1,000, where the check sampled at cycle 99. Under JIT the runtime's own allocation front-loads and the heap is already settled by cycle 99, which is why the same code read 0.95 there and 2.30 here. **The band was not widened** — the baseline moved to the midpoint of the run, so both readings are after warm-up in every publish mode, and the band was **tightened** from 2.0 to 1.20 because the midpoint form makes 2.0 unreachable by any linear leak. **A negative control now injects a per-cycle retention**, which nothing did before and whose absence is why the defect survived. Every figure above is an **observed repository-state fact** under section 1's third category: it explains the status and satisfies no gate. **The clause is not closed here** — closing it needs a retained bundle, which is JS-9's, and none has been collected since the correction. **Both composition roots are now published and run on every declared RID by the component's own CI lane, which also runs the four fuzz sessions, the corpus-integrity mutation and — since the correction — the soak itself, against the published image** — none of which it did when this failure reached two bundles unnoticed. The soak was briefly excluded from that lane on the ground that a heap reading on a shared runner attributes to nothing; that was true of the old check's absolute reading and is not true of the corrected check's ratio, which repeated dispatched runs across three operating systems settled rather than argued — the lane runs two of those three since 2026-09-01, when it was brought back to the component's declared RID matrix, and the third reading stands as a record of what ran rather than as a claim about a platform. **Those readings are cited nowhere as a figure and this ledger states none of them**, because they were taken in lane runs that retain nothing: under update rule 10 a number with no retained record behind it is not a number this file may carry, and what the checkout holds is the check, its band and the control that makes it fail. **The lane advances nothing here either**: it collects no bundle, so it is a regression signal between collections and never a row in this table, and **a green lane is not evidence that the plateau passes**. The corpus grows from JS-1 onward, which is why this could start before JS-8. **The seven-dimension clause this row opened on 2026-09-01 is now built in the checkout and is not closed.** Roadmap section 7 names **seven** budget dimensions a verification can answer `ResourceExhaustion` on — it named four until that date, and the verifier's allocator, work-charge and poll arms name three more ([JSC-39](roadmap.corrections.md#jsc-39)) — and asks for a corpus entry per dimension because an exhaustion answer carries no diagnostic code and the registry's both-directions binding therefore reaches none of them. Where the row previously reported one entry, an ordering assertion, two categories buried in a fuzz histogram and three dimensions reached by nothing, the checkout now holds: a manifest with a **dimension and scope column**, **seven entries**, one per dimension, each presenting the same well-formed program to a host that declined it on exactly one ceiling; and **rule N11**, which reads every resource-exhaustion answer out of the verifier's own source and holds it to an entry that pins it, in both directions and against the core's two enumerations — the clause rule N7 could not reach, for the answers that carry no code. Landing it **found that artifact bytes is not this profile's answer at all**: the core compares the payload length one call before the verifier is entered, so the verifier's two artifact-bytes arms are unreachable through any host ceiling and are defensive ([JSC-41](roadmap.corrections.md#jsc-41)). The scope column earns itself on the same evidence: the reader's ceilings answer at `Artifact` and the three allowances answer at `Runtime`, because the meter reports the level that refused, and a row recording the dimension alone would have hidden it. **The corpus grows from 60 entries to 66**, which is where a reader tracing the count across three rows meets this step. **None of that closes the clause**: closing it needs a retained bundle and none has been collected since, and under update rule 5 the four retained fuzz sessions are now evidence over a different population — they were seeded from the 60-entry manifest and this checkout holds 66, so they recertify nothing about the corpus as it stands and the next collection re-runs them. This milestone owns the clause, and the delivery file's exit gate carries it. |
| [NONE] | **JS-10 — baselines, packaging, support table, release gate** | **Not started** | None. No measurement lane, no baseline register, no package, no support table, no human review decision on anything. **The language-specification edition is pinned and archived since 2026-09-03; the conformance-suite revision is not pinned at all** *(this cell said neither was pinned; corrected: [JSC-67](roadmap.corrections.md#jsc-67))*, and roadmap [section 24](roadmap.gates.md#24-specification-and-platform-references) asks for both by immutable identifier, retrieved, hashed and archived — see section 3, where the edition's row is now closed and the suite's is not. **Gate 1's support table needs both**, so one of the two is answered and this row waits on the other, on a measurement lane, on packaging, and on a human review of every relevant unit. | After JS-9, and after a named human has read every relevant unit — which is the largest single-owner task in the programme and must be scheduled, not assumed. |

### What the construct census measured, and what it is not

**Everything in this subsection is observed repository state under section 1's third category. It
satisfies no gate, advances no row, and is not a conformance result.** It is recorded because the
roadmap's remaining scope — JS-4's object model, JS-5's calls, JS-6's library — has been argued
from what a JavaScript engine is generally assumed to need, and there is now a measurement instead.

**The instrument.** The producer composition grew a `--census` mode on 2026-09-03: it reads a
directory of JavaScript, parses each file, and ranks the constructs the declared manifest excludes
by **how many files need them** and by how often they occur. Since 2026-09-03 it also reports what
that ranking would *buy*: how many constructs each file needs, and how many whole files become
compilable as the ranked constructs are admitted in order. It keeps no copy of what it reads.
**It is not the conformance oracle** [section 14](roadmap.md#14-the-conformance-oracle) specifies
and has none of that instrument's parts — no pinned suite revision, no content-independent
sharding, no self-check proving a failing test comes back as a failure, no per-host-mode totals and
no ratchet — so no number below may be read as a score, and the suite-revision dependency in
section 3 is untouched by it.

**Two bodies of code were measured, and neither entered this repository.** Retrieving, hashing and
archiving third-party material is the human action section 3 records as open, and a census that
takes a path performs none of it.

| Measured | Files | Parsed | Contain nothing outside the manifest |
|---|---:|---:|---:|
| The Octane checkout, every `.js` file in it | 24 | 24 | **0** |
| — of which are benchmark sources | 17 | 17 | **0** |
| test262 | 53,771 | 52,330 | **141** |

**The twenty-four are not twenty-four benchmarks, and the first row said so until 2026-09-03**
*(corrected: [JSC-55](roadmap.corrections.md#jsc-55))*. Three are the demonstration page's own
assets — jQuery and two Bootstrap plugins, shipped so the checkout's `index.html` renders — one is
the harness that defines the benchmark type, one is the runner, and two are data blobs a benchmark
reads rather than code it runs. **Seventeen are benchmark sources**, and the second row is the one
a scope decision should be read from.

**What Octane needs, by files of twenty-four:** a string value (24), a call (23), a function (23),
a property access (21), then an array literal, a loose equality, `new`, `null` and `return` (20
each), a computed property access and `this` (19), `++` or `--` (18), an object literal (16).
**`==` appears in twenty of the twenty-four**, which is worth naming because
[JSD-0014](decisions/0014-the-source-front-end-and-the-verification-boundary.md) refuses to lower
it onto `===` — that would answer a conformance case wrongly rather than decline it — so those
files need a semantic this profile has deliberately declined to fake.

**And what that ranking buys is the opposite of what it suggests.** A list ordered by how many
files need a construct invites "buy the top of it first". The number a scope decision actually
wants is what buying the top *k* would admit, and the census reports that curve beside the ranking
so the two cannot be read apart:

| Admitting, in the ranked order over the seventeen | Benchmark sources this profile could then compile |
|---|---:|
| the first 13 — array literal, call, function, `==`, `new`, `null`, string value, member access, `return`, `this`, `++`/`--`, computed member access, `throw` | **0** |
| the first 14 (adding compound assignment) | 1 |
| the first 15 (adding the object literal) | 3 |
| the first 17 (adding `typeof` and `~`) | 6 |
| the first 21 (adding `switch`, `try`, `delete`, the comma operator) | 10 |
| all 28 | 17 |

**Thirteen constructs buy nothing.** There is no cheap partial win here: the nearest benchmark
source needs nine constructs and the median needs sixteen. Over the twenty-four-file corpus one
construct does admit one whole file, which reads as a first win and is not one — the file is
`typescript-input.js`, a data blob.

**The two corpora rank differently, which is the point of separating them.** Over the seventeen,
the array literal and `==` join the tier every file needs; over the twenty-four they do not, because
jQuery and the page assets dilute them.

**The curve is along one ranking and is not a smallest set.** The order is the census's own, by how
many files need a construct, which is a reasonable order to buy things in and is not the cheapest
set that would admit some file. Reading it as a minimum would overstate what it proves.

**And the census and the shipped host disagree about two of the twenty-four**
*(corrected: [JSC-57](roadmap.corrections.md#jsc-57))*. The census reads at the largest nesting
bound the parser supports, deliberately, because it wants to read the file rather than enforce this
build's conservative default — so it reports all 24 as parsed. **The end-user host at its default
bound of 64 refuses two of them before the manifest is consulted at all**, with
`2103:NestingTooDeep`: `earley-boyer.js` and `mandreel.js` nest deeper than 64 levels. Raise the
bound and both get past the parser and are refused by the manifest instead. Both numbers are right
and they answer different questions — the census measures **the language**, the host at its default
measures **the product** — and what was missing until now is that nothing said so, in the direction
that flatters. It also means those two files report a diagnostic that says nothing about the
manifest, which is what a reader of this table wants to know about. **Whether 64 is the right
default for a host is not decided by this row**; it now has a measurement behind it and still needs
an owner.

**The test262 row was re-derived on 2026-09-03 at a named ref, and this paragraph said it could
not be.** It could not, when it was written: the checkout the first census read was gone. One was
then retrieved to a temporary directory at ref `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e` and the
census re-run — **53,469 files, 52,038 parsed, and 141 containing nothing outside the manifest**.
The 141 matches the figure first recorded exactly; the file counts differ by 302 because the ref
differs, and the first census's ref was never written down. **Neither body of code is in this
repository and neither may be**: a census takes a path, the checkout was left where it was, and
retrieving, hashing and archiving as evidence is the human action section 3 still records as open —
a tarball fetched for one sweep performs the first two of those three and not the third.

**What test262 needs, by files of fifty-three thousand:** a call (51,570), a property access
(45,578), a string value (42,615), an object literal (26,278), a function (23,374), `new` (22,766),
an array literal (17,750). The two lists agree on their first five, which is the useful part: the
benchmark and the suite are not asking for different languages.

**The census found four defects in the front end and each was fixed with the measurement that
found it.** Unknown escape sequences were refused, though `\q` is `q` in the language, and that
alone rejected eight of Octane's twenty-four files. Numeric separators and BigInt suffixes were
refused, which was 2,362 test262 files. Private names and unicode escapes in identifiers were
refused, which was another 5,034. And **a lone surrogate threw out of the tokenizer**, which is a
fault escaping a pass whose contract is that it refuses rather than throws; it killed a whole
census run, and the census counts faults separately now rather than dying of one, because a tool
that stops at the first fault measures nothing. Parsing went from 84.3 per cent of test262 to
**97.3 per cent** across those fixes.

**What the remaining 1,441 unparsed files are is not claimed.** A large part of test262 is negative
tests whose whole purpose is to be syntactically invalid, so some of that number is this instrument
working; the rest is grammar this parser does not have. **Nothing here separates the two**, and a
reader must not read 97.3 per cent as a conformance figure or the remainder as a defect count.

### What running real material through the product path found, and what it did not settle

**Three instruments were pointed at the same retrieved test262 checkout on 2026-09-03** — the
end-user host, the construct census and the conformance harness — and they answer three different
questions. Retained in [Bundle JS-3B-001](evidence/js-3b-001/README.md). **The checkout was
retrieved to a temporary directory, read, and left there; no suite file is in this repository, no
floor is set over any figure below, and section 3's dependency is not closed by them** — it asks
for material retrieved, hashed **and archived**, and the third of those did not happen.

**The end-user host, over 53,469 files.** This is what a person pointing the shipped tool at the
suite sees:

| Outcome | Files | What it means |
|---|---:|---|
| Completed | 103 | bodies that happen to lie inside this manifest, run as programs |
| Refused at the source seam | 53,337 | of which **51,125** are `ConstructOutsideManifest` |
| Spent the allowance | 16 | verification is charged work and these files are large |
| **Artifact refused by this profile's own verifier** | **0**, and it was 13 | the defect is repaired *(corrected: [JSC-58](roadmap.corrections.md#jsc-58), [JSC-60](roadmap.corrections.md#jsc-60))* |

**The thirteen were the finding, and they are repaired.** Every one was `for (…) { break; }`, and
the compiler's own remark had named a different shape — a loop with no exit — as the one it cannot
emit. A body that always breaks leaves the update and the back-edge unreachable, which is ordinary
JavaScript; the lowering now emits a loop's continuation only where something reaches it, and
**completions over the same 53,469 files went from 103 to 116 — the thirteen, and nothing else**
*(corrected: [JSC-60](roadmap.corrections.md#jsc-60))*.

**The number that did not move is worth as much.** The harness's totals over the same suite are
identical before and after: the thirteen are positive tests needing the assertion prelude, so it
never selected them. **A repair visible to every instrument would have been a repair to something
else.** And the analysis is conservative in one direction — unsure means reachable, which is what
the lowering did unconditionally before — so the only bytes that can move belong to programs the
verifier was already refusing, and **the retained corpus regenerated byte-identical**. A loop with
no exit at all is still refused, is still the format's answer, and is pinned separately so one
change cannot claim both.

**The conformance harness, in the ingested dialect.** This is the first external correctness signal
this component has had, and the column to read is the one nobody would look at:

| | Selected | Executed | Passed | Failed | **Declined as unscorable** |
|---|---:|---:|---:|---:|---:|
| Script | 8,360 | 1,184 | 1,167 | 17 | **7,176** |
| Module | 212 | 22 | 21 | 1 | **190** |

57,421 candidates, 48,849 unselectable — almost all needing the assertion prelude this manifest
admits no call to load — and 8,572 selected, which is the parse-and-early-error slice.

**7,365 of the 7,366 declined cases were declined because the refusal was
`ConstructOutsideManifest`**, and every one is a negative test whose declared expectation is that a
refusal happens. **A harness comparing outcomes would have scored all 7,365 as passes and reported
8,535 of 8,553 — 99.8 per cent.** That is the number [JSC-54](roadmap.corrections.md#jsc-54)'s rule
exists to refuse, and this is the first time it has been measured rather than argued. What the rule
leaves is **1,170 of 1,188 scored**.

**The eighteen failures are three groups, and only two of them are gaps in this component** — all of which are now repaired, which the section below measures. Six
are hashbang comments — `#!` opening a script, which is in the language and which this tokenizer
does not know. Eight are the temporal dead zone: `let`/`const` used before initialisation must
throw a runtime `ReferenceError` and this profile answers `undefined`, so the dead zone is not
implemented. **Four are `using` declarations, and those four should not have been scored at all**:
the construct is a proposal, the harness can filter by the suite's own feature metadata, and this
run did not pass a filter. That is a mistake in how the run was made — and it is also the first
concrete cost of section 3's *unpinned language edition*, because which constructs are in the
edition is exactly what decides whether such a test applies. *(Both halves of that sentence were
wrong and are corrected: [JSC-66](roadmap.corrections.md#jsc-66). The harness had no way to exclude
by feature at all — `--features` is an inclusion filter and no value of it removes one feature —
and the cost was not four failures but **117 passes this run had not earned**, over the same
construct.)*

**Two files of the 53,469 were refused by the dialect reader and both refusals were its own
defects** *(corrected: [JSC-59](roadmap.corrections.md#jsc-59))*: a file written with carriage
returns alone, and a file whose whole metadata block is indented. Neither was reachable from any
check this component wrote, because every fixture and every check used LF and a block at the
margin. **A reader's fidelity is measured against material nobody here authored, or against its
author's habits.**

**What none of this is.** Not an accepted milestone, not a floor, not a supported claim, and not a
conformance total anybody may quote: the pin is over a transient checkout, the language edition is
unpinned, one run passed no feature filter, and the harness scored 1,188 cases of a suite of
53,469. What it is, is the first time this component has been told something about itself by
material it did not write.

### What the first external signal was worth, measured by what it changed

**Between them, the runs above found four defects and three of the four are repaired.** The table
is the whole of the case for pointing a real suite at this component, and it is small enough to
read:

| Found | Where it showed | Repaired |
|---|---|---|
| A loop whose body always breaks emits a continuation nothing reaches | the host: 13 artifact refusals | **yes** *(corrected: [JSC-58](roadmap.corrections.md#jsc-58), [JSC-60](roadmap.corrections.md#jsc-60))* |
| `#!` opening a source text is a comment and the tokenizer refused it | the harness: 6 failures | **yes** *(corrected: [JSC-61](roadmap.corrections.md#jsc-61))* |
| The temporal dead zone was **unexpressible**: no instruction in format version 1 could fail | the harness: 8 failures | **yes** *(corrected: [JSC-62](roadmap.corrections.md#jsc-62))* |
| Two files the dialect reader could not read | the harness refusing the suite | **yes** *(corrected: [JSC-59](roadmap.corrections.md#jsc-59))* |

**The harness's failures went from 18 to 4, and the four that remain are not a gap in this
component.** They are `using` declarations — a proposal — scored because the run passed no feature
filter. The suite carries the metadata to exclude them and the harness can read it; what decides
whether such a test applies at all is the **language edition**, which
[section 3](#3-open-external-dependencies) records as unpinned. That row now has a cost attached to
it rather than only a description. *(Corrected: [JSC-66](roadmap.corrections.md#jsc-66) — the
harness could read the metadata and could not act on it, and the four failures were the smaller
half of what that cost.)*

| | before | after |
|---|---:|---:|
| Executed | 1,188 | 1,205 |
| Passed | 1,170 | **1,201** |
| Failed | 18 | **4** |
| Declined as unscorable | 7,366 | 7,367 |

**And the host's own sweep of the same 53,469 files moved in four columns**, each for a reason
named above: 103 → **117** completed, 13 → **0** artifact refusals, 53,337 → **53,332** refused at
the source seam as five hashbang files began to parse, and **4 faults** where there were none —
programs that now throw the `ReferenceError` the dead zone requires.

**One of the six hashbang files is now DECLINED rather than passed**, and that is the honest
outcome rather than a shortfall: it parses past the hashbang and then needs a construct this
manifest does not admit, so the refusal is not a language answer and
[JSC-54](roadmap.corrections.md#jsc-54)'s rule reports it unscorable.

**Two safety properties held across all three repairs**, and both are checks rather than claims.
**The retained corpus regenerated byte-identical** after each — no artifact whose bytes are pinned
was touched. And the reachability analysis answers "reachable" wherever it is unsure, which is what
the lowering did unconditionally before, so the only bytes that could move belonged to programs the
verifier was already refusing.

**The write half is taken too, and it moved no number** *(corrected:
[JSC-63](roadmap.corrections.md#jsc-63))*. `x = 1; let x;` is a `ReferenceError` in the language and
now is one here. **Every suite figure above is identical before and after**: the cases that would
exercise a write in the dead zone are not in the selectable slice, because they need the assertion
prelude this manifest admits no call to load. **It is a repair the language required and the suite
did not ask for**, pinned in the host's acceptance suite because that is the only instrument here
that can hold it. Taking it also forced the opcode's contract to change — it declared a push and a
write replaces an instruction that pops, so it is a guard that moves nothing and stands before the
instruction it prevents.

**Dead code after a terminator is repaired too, and the reason it was declined was wrong**
*(corrected: [JSC-64](roadmap.corrections.md#jsc-64))*. It was declined because "a slot the executor
never wrote is a state this profile does not guarantee" — and the executor writes `undefined` into
**every** local when the instance is built, in a loop, with a comment saying that is what `var`
does in the language. **Two remarks in two assemblies, and the more pessimistic one was the one
consulted.** A block now stops being lowered after a statement control cannot pass; the program
body does not, because its tail is the `Return`.

**Neither of the last two repairs moved a number**, and both are pinned in the host's acceptance
suite because that is the only instrument here that can hold them: no file of test262's selectable
slice writes to a binding in its dead zone or carries dead code after a terminator, since those
cases live behind an assertion prelude this manifest admits no call to load.

**And the shape that was called the format's answer three times is repaired too** *(corrected:
[JSC-65](roadmap.corrections.md#jsc-65))*. A loop nothing can leave makes everything after it
unreachable including the program's tail, and suppressing a tail was said to leave a function with
no terminator. **The verifier's rule is that every REACHABLE PATH ends in a return, and such a loop
has no path that ends at all** — the code finishes on a backward jump rather than by falling off the
end. The sentence was true of every other way of reaching the end and not of this one, and the
difference went unchecked through two repairs of its neighbours. `for (;;) { var x = 1; }` now runs
until it spends its allowance, which is what an infinite loop is.

**It needed a second repair to work, and that one is the interesting half.** `while (true)` emitted
a `JumpIfFalse` past the loop that no execution takes; once the tail was suppressed its target sat
past the end of the code and the verifier refused the jump target instead — a different diagnostic
for the same mistake. **A test that can never be false is not a branch.**

**One retained artifact changed bytes, the first in this whole sequence.**
`source-break-leaves-the-loop` lost a test and a branch; **its recorded answer is unchanged at `3`**
and the replay confirms it. An artifact whose bytes move without its answer moving is exactly the
event the corpus manifest exists to make visible.

**All three unreachable-code shapes are repaired.** The directory that pinned them was called
`known-defects`; it holds none.

### The last four failures were not failures, and removing them cost 117 passes

*(Corrected: [JSC-66](roadmap.corrections.md#jsc-66).)* The four `using` cases were the visible end
of something larger. **The run scored 121 cases claiming that construct and 117 of them passed** —
every one a refusal this front end makes because it has **no production for `using` in any
spelling**, agreeing with a test that declared a `SyntaxError` for one particular malformed
spelling. The outcomes match and the reasons are unrelated, which is the bug
[JSC-54](roadmap.corrections.md#jsc-54) exists to refuse, arriving through the one door that rule
cannot see: its question is whether a refusal was a language answer, and `ExpectedToken` is one —
672 of this run's 1,201 passes rested on that code. **What no rule here could ask was whether the
test was about this language at all.**

**The suite answers that, and the harness now reads its answer.** An ingested suite ships a
`features.txt` that splits its own flags into proposals and published constructs, and says in its
own prose that the proposed section exists so consumers can omit it. Reading it is **required**: a
run whose suite carries no readable list stops, in the same voice as a run pointed at a suite with
no pin.
[JSD-0018](decisions/0018-which-tests-are-about-this-language-and-who-decides.md) records the four
alternatives this rejected, the one that was never available — reclassifying `ExpectedToken`, which
most of the honest passes rest on — and why the exclusion is its own counted stage rather than a
column added to the run's own filter.

| | before | after |
|---|---:|---:|
| Executed | 1,205 | **1,084** |
| Passed | 1,201 | **1,084** |
| Failed | 4 | **0** |
| Candidates excluded for claiming a proposal | 0 | **8,304** |

**The passes fell by 117 and nothing was repaired to make that happen**, which is the whole of why
this is worth reading: a change that removed four failures and left the passes where they were
would have been the same change with its cost hidden. **Every remaining scored case claims a
construct of a published edition or claims nothing at all**, which is a census over the run rather
than a reading of the code.

**The reader's one silent failure mode is pinned by a check and by a control.** The list writes
`##` for comments inside a section as well as for its headings — the pinned checkout does it twice
— so a reader keying on the prefix would end the proposed section early and lose **twelve of its
twenty-one proposals** while reporting the same shape of success. **And two of this change's own
new checks passed under their own negative controls** and were repaired before it landed; the six
controls now each fail exactly the check that names them.

**This narrows section 3's unpinned language edition and does not close it.** What is read is one
suite's opinion of its own flags at one revision. A construct that suite has not flagged is still
scored on whatever this front end happens to do with it.

### The language edition is pinned and archived, and it checked three claims and found one disagreement

*(Corrected: [JSC-67](roadmap.corrections.md#jsc-67); recorded in
[JSD-0019](decisions/0019-the-pinned-language-edition-and-what-two-of-three-actions-buy.md).)* The
edition row had stood open since JS-0 on a reason that was correct every time it was given — an
edition nobody has retrieved would be a pin in name only — and was written as though there were two
states. Roadmap [section 24](roadmap.gates.md#24-specification-and-platform-references) defines
three, and the pin passed through the middle one — **retrieved and hashed and not archived is a
provisional pin carrying a named exclusion** — for the few hours between being taken and being
completed. **All three actions are done: the document is archived in this repository**, under the
Ecma alternative copyright notice, which permits redistribution on three conditions this change
meets rather than argues about.

**The pin actually taken** is ECMA-262, 17th edition (ES2026), at `tc39/ecma262` commit
`0248456c758431e4bb8e5d26333ff1865123c9cd`, `spec.html` of 2,978,793 bytes hashing to
`ce7bc30174061fd8d212270b81cf6511661180c1e174f6911d10ced0581527b0`. A **commit** rather than the
`es2026` tag, because a tag can be moved; the published edition rather than `es2026-errata`, because
errata accumulate and pinning them means pinning a moving target under a name that sounds fixed.

**Three claims this component had made in prose are now checked against a fixed document**, and all
three hold: `#!` is a comment from ES2023 and not from ES2022 ([JSC-61](roadmap.corrections.md#jsc-61));
`using` declarations are in no published edition, which is the premise
[JSC-66](roadmap.corrections.md#jsc-66) removed 121 cases on; and a binding used before its
initialiser is a runtime `ReferenceError` ([JSC-62](roadmap.corrections.md#jsc-62)).

**And the second authority disagreed with the first once, which is what a second authority is
for.** Of the twenty-one flags the pinned checkout calls proposals, twenty carry no marker in the
pinned edition and one does: **`regexp-duplicate-named-groups` is in ES2025 and ES2026** while the
suite still calls it a proposal, so JSC-66's exclusion removes **19 files that are about this
language**. It moves no figure — none of the 19 was scored even by the run that had no exclusion at
all — and what changes is that the risk JSD-0018 recorded without a size now has one.

**The pin is declared in code so a run states it and one edit cannot move it quietly.**
`JavaScriptLanguageEdition` carries the revision, the digest and, as a field rather than a
paragraph, **whether the document has been archived**; the report format goes to 3 for an `edition`
line beside the suite revision, and refuses a report scored against a different one; the end-user
host prints it under `--version`. **Rule N14** holds the code, the decision record and section 3's
row to the same revision and digest, and holds the archived field to the ledger's account of the
pin in both directions — with four negative controls, each moving the pin in one of the four places
that must agree, each caught.

**Archiving is what makes the digest checkable rather than only readable.** A pin whose document
lives at a URL depends on somebody else's uptime and somebody else's history; rule N14 now hashes
the retained bytes and compares them against the published constant on every run of the suite,
which is a clause that could not exist while the file was elsewhere. **It still accepts nothing** —
acceptance needs a human review this component does not have — and it says nothing about the
conformance suite, which is separately licensed material of a different size and is still not
retrieved.

### What two workloads through the shipped path showed, and everything the wide manifest does not admit

**This subsection is the exclusion list JS-6's gate asks to be published on the day the library
lands, and it is also what the two workloads did not settle.** Everything in it is what one retained
bundle shows — [Bundle JS-4-001](evidence/js-4-001/README.md), which carries the two workloads
beside the suite, the corpus replay, the composition checks and the fuzz sessions — beside observed
repository state under section 1's third category, which explains a status and satisfies no gate.
**The figures are in the bundle and this file restates none of them**, under update rule 10.
Nothing below is a conformance result, a baseline, or a claim that anything is supported, and no
human has read any of it.

**A benchmark harness and a benchmark run through the ordinary command line and print a score.**
The end-user host is handed `base.js`, one benchmark, and `src/tests/octane/run-one.js` as named
files run in order as separate scripts in one realm — which is what a per-instance realm that
outlives one invocation is for — and the benchmark's own harness computes the score and prints it.
**What that demonstrates is that the whole path runs**: a third-party benchmark and its own harness,
written against a real engine's assumptions rather than against this one, through this front end,
this verifier, this executor and this library, with the runner the only file in the loop written
here. **What it is not is a measurement.** There is no measurement lane and no baseline register —
JS-10 owns both — so a score printed on one machine is a number about this configuration rather
than a comparison with anything, including with itself on another day. One benchmark per run is how
it was run and how it is retained. **And the benchmark checkout is not in this repository and is
not pinned**: the host takes a path and keeps no copy, so there is no digest behind these rows, and
pointing a tool at a directory performs none of the retrieve-hash-archive that section 3 records as
the human action. A benchmark that produced no score is retained saying what it did instead.

**A real conformance suite runs against the wide manifest, and it runs over subtrees somebody
chose.** The conformance composition gained `--test262 <root>`
([JSC-76](roadmap.corrections.md#jsc-76)), which reads a test262 checkout — the archive section 3's
closed row pins, extracted to a scratch directory outside this repository and run from there —
under `--test`, `--dir`, `--limit`, `--fuel` and `--wall`. **A run over named subtrees measures
those subtrees.** Which subtrees were named is a choice somebody made, so a total from such a run
is a fact about that choice as much as about the engine. **The runs contain failures and the log
retains them** rather than the passing half: first failures are named per subtree, and a subtree
whose cases failed exits non-zero. Roadmap
[section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)'s rule — a manifest
with no retained run of its own is not accepted — is unmet for `broiler.javascript.wide` and stays
unmet. The floor section 3 records was set over the slice through the ingested dialect, and **no
floor is set over any figure of the wide manifest's**.

**The verdict to read is `unsupported`, and it exists because pass and fail are not enough.** The
mode reports pass, fail, unsupported and skipped, because **a construct this manifest does not
admit is neither a pass nor a failure**: refusing such a program is this profile doing what it
declares, and scoring that refusal against a negative test that expected a `SyntaxError` for an
unrelated reason produces a near-perfect total that means nothing — the mistake
[JSC-54](roadmap.corrections.md#jsc-54)'s rule exists to refuse and
[JSC-66](roadmap.corrections.md#jsc-66) has already measured the price of once. A reader who reads
the pass column of one of these runs without the unsupported column beside it is reading a number
about which subtree was chosen.

**What the wide manifest does not admit is a list, and nothing on it is admitted partially.** It is
the same list the end-user host's own correction carries as the reason that host is still not
advertised ([JSC-75](roadmap.corrections.md#jsc-75)), and the manifest's minting records it too
([JSC-70](roadmap.corrections.md#jsc-70)). Refused **by name at the front end**: `class`, generator,
`async` function, module, destructuring, spread, template literal, tagged template, `for … of`,
optional chain, `with`, Proxy, Reflect,
Symbol, BigInt, typed array, `eval` and the `Function` constructor. A refusal by name is what the
harness's `unsupported` verdict counts, and a program using any of these is refused rather than
mis-run — which is the manifest boundary roadmap
[section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted) states, applied to
a manifest wide enough that a reader might now expect a fallback. There is none.

**Three approximations are shipped rather than excluded, and each is declared where it is made**
([JSC-75](roadmap.corrections.md#jsc-75) carries them beside the list above).
**RegExp is translated to `System.Text.RegularExpressions`**, and the file that does it declares the
approximation rather than leaving a reader to find it; **the Octane RegExp benchmark runs and fails
its own checksum**, which is the shape of what an approximation costs — it does not refuse, and it
does not agree. **`Date` fixes the local time zone to UTC.** **`arguments` is unmapped.**

**A fourth divergence is about scoping rather than about a library surface, and it is the one most
likely to be read as a defect in something else.** **Script-level `let` and `const` become
properties of the global object** rather than bindings of a separate global lexical environment,
and the observable difference is that **a read before the declaration answers `undefined` instead
of throwing**. A reader who saw the temporal dead zone repaired on 2026-09-03
([JSC-62](roadmap.corrections.md#jsc-62)) must not read that repair as reaching this: it made a
read before initialisation throw where the binding is a binding, and a script-level `let` here is a
property.

**The suite found one defect of the kind it exists to find, and it is recorded rather than
quietly repaired.** Three subtrees of the first run did not report a result at all: a
tail-call-optimisation case recursing a hundred thousand deep **terminated the process**, at a
depth neither the call-depth ceiling nor the runtime's own sufficient-stack probe reached
first. A stack overflow is the one failure the CLR cannot turn into an exception, so nothing
downstream could have reported it. It is repaired - the profile runs a guest invocation on a
thread whose stack it declares, and a recursing program is refused as a resource exhaustion
naming `CallDepth` - and what the repair does not do is measure the per-frame cost the declared
stack was chosen against, or show the refusal under Native AOT on any RID.
[JSC-79](roadmap.corrections.md#jsc-79) carries the reading of the plan this changed.

**And there is no job queue.** An asynchronous test262 case cannot complete, whatever the front end
does with its syntax. The two facts are separate and both are true: the `async` function is refused
by name at the manifest boundary, and the host has nowhere to run a job that would fall due after a
script returns.

**What none of this is.** **Native AOT was not published on the machine this was written on**,
and the component's lane is the authority for what publishes — under section 1 a lane retains
nothing and advances nothing here, so what stands open there is a collection rather than a fact
about the code. One machine, one RID. No row moved to `Accepted` and none could: acceptance needs
an owner and a reviewer decision, and **nothing here has been read by a human**. What this
component has that it did not have the day before is a configuration that runs a real workload end
to end, and a list — the one above — of what it does not do while running it.

### What this component is not claiming

Stated positively, because a table of empty rows invites a reader to fill them in:

- **No language is supported.** Two feature manifests exist and **neither is accepted**: acceptance
  needs a manifest's own retained run over the pinned suite, neither has one — the slice was scored
  over the ingested dialect and the wide surface over subtrees somebody chose — and a manifest name
  would not be a conformance claim even if it did. `broiler.javascript.slice` admits numbers,
  arithmetic, comparison, local variables and structured control flow, and admits no object, no
  string, no function and no property access - which is deliberately not JavaScript anyone would
  ship. **`broiler.javascript.wide` admits objects, strings, functions and property access**, and
  what it refuses by name and what it approximates are the subsection above rather than a summary
  here.
- **No composition is advertised** and none is packable. Three JavaScript composition roots
  exist - the slice compiler, the execution-only image and the Android head - and all three are
  registered as demonstrations. **Two runtime identifiers are recorded as published and run** -
  `win-x64`, and `android-x64` on an emulator - which is a record of what happened on two
  machines and not a supported-RID claim;
  claiming a RID is a release act and JS-10 owns it. **And no retained bundle shows that run
  clean in every mode**: the newest one records the execution-only root's Native AOT run exiting 1
  on the soak's plateau check. That check has since been corrected and the working tree passes in
  every mode, which is a fact about the checkout and not about any bundle - so the clause stays
  open until a collection shows it, which the JS-9 row states and which no row here reads past.
- **A composition root carries more than the image its label describes.** The execution-only root
  holds the corpus replay, the ordering assertions, the fuzz mutator, the soak and the
  aggregate-budget exercises, and all of them are in the closure it publishes - because each has to
  drive this profile's own verifier and executor, and the rules leave nowhere else for them to be.
  The label is a claim about a reference set and not a file inventory
  ([JSC-34](roadmap.corrections.md#jsc-34)).
- **No conformance result anybody may quote exists.** This bullet read *the suite is not pinned and
  the harness is not built* and both halves were overtaken on 2026-09-03, and again on 2026-09-04
  when a second manifest was run against the pinned revision over subtrees. What holds is narrower
  and is the part that matters: **a run over a subtree measures that subtree**, roadmap section 14
  forbids publishing an aggregate percentage, and no manifest has a retained run of its own. A
  published diagnostic registry is not a conformance claim and neither is a retained corpus: both
  are this component's own record of what its verifier does.
- **No measurement exists**, and no figure from any other component stands in for one. **A
  benchmark printing a score is not one**: from 2026-09-04 the end-user host runs a third-party
  benchmark through the ordinary command line, and what that lacks in order to be a measurement is
  everything JS-10 owns — a lane, a baseline register, and
  [section 17](roadmap.gates.md#17-measurement-discipline)'s rules applied to a retained figure.
- **Nothing is reviewed.** No human has read anything here, and nothing that will be copied
  arrives reviewed.
- **The seed has not been taken.** Section 4.1 of the roadmap records a candidate identity so the
  record has a shape; JS-2 records what was actually taken, and may differ. Bundle JS-0-001
  re-derives that candidate from the checkout and matches on all four revisions, which says the
  record is reproducible and says nothing about a snapshot having happened.
- **A taken decision is not implemented code.** Two records decide things no line in this checkout
  does yet: [JSD-0011](decisions/0011-the-value-frame-and-call-abi.md)
  fixes an eight-row ABI whose object model, frame object and string are all unwritten, and
  [JSD-0005](decisions/0005-the-seed-waited-on-set-and-snapshot-stop-condition.md) rules on a
  snapshot nobody has taken. Each says so in its own text; this bullet is here because a reader
  counting decision records would otherwise be counting the wrong thing.
- **The product code that exists is a slice and a wide surface beside it, and each says which it
  is.** For `broiler.javascript.slice`: a verifier, an executor, a hand-written lowering, and —
  since JS-3b — a **source front end**: a tokenizer, a syntax tree, a parser, one static-semantic
  stage and a source-to-bytecode lowering. For `broiler.javascript.wide`, since 2026-09-04: a
  second format version and a verifier for it, a value and object model, an interpreter, a standard
  library, and a second source front end sharing the first's tokenizer and nothing else. **There is
  still no suspension and no guest-initiated load**, and the representation JS-4's entry gate fixed
  is the one the code now takes. This bullet read that **a front end for one manifest admitting no
  function, no object and no string value is not a JavaScript front end**; that was true of the
  slice's front end and is not true of the wide one's, and what stands in its place is the list
  above of what the wide manifest refuses by name and what it approximates.
- **JS-1's hand-written encoder and its hand-written programs are scheduled for deletion at
  JS-4**, with a named owner and a gate clause, because a second handle-producing path and a second
  corpus of programs are non-goals. **The instruction buffer beside them is not**, because JS-3b's
  source lowering uses it rather than writing a second copy *(corrected: JSC-45)*.
- **No milestone is accepted.** JS-0, JS-1 and JS-3a each have an open exit-gate clause, and
  acceptance would in any case need a reviewer decision that nobody has made.

---

## 3. Open external dependencies

A milestone blocked by a named external dependency records the blocker, its holder, and its
unblock condition. **One is open today**, and it belongs to JS-2. The second, the seed's
un-itemised waited-on set, was closed by JS-0 and is recorded below as closed rather than
deleted.

| Blocker | Holder | Unblock condition | Note |
|---|---|---|---|
| **The core contract is not accepted.** Every core milestone is in progress and unaccepted, and the core's review record is unsigned. The core roadmap's own seeding conditions require the copy to be adapted to an accepted contract rather than a moving one. | The Broiler.VM core's architecture and release owners | A recorded human review decision on the core's contract surface, at a named contract version | This blocks JS-2 onward. It does **not** block JS-0 or JS-1, which build against the contract as implemented — a distinction the roadmap's delivery order states and this ledger holds it to. **Four milestones the delivery order places after JS-2 have moved without it moving** — JS-3b on 2026-09-03, and JS-4, JS-5 and JS-6 on 2026-09-04 — and that is not this row weakening: none of them performed the copy this row blocks, because each was written in this checkout rather than ingested. What this row blocks is the ingest, and JS-2 is the ingest. **Nothing that landed on 2026-09-04 unblocks it**, and a wide language surface existing here must not be read as the seed having been taken: none of the corrections this change mints speaks to the seed at all, and roadmap [section 9](roadmap.md#9-the-semantic-front-end-and-lowering) still gives the general front end to the ingest. |
| **The seed's waited-on set.** **Closed 2026-08-31** by [JSD-0005](decisions/0005-the-seed-waited-on-set-and-snapshot-stop-condition.md): a dated ruling on each of the five items — one `Wait`, four `Do not wait` — plus a stop condition, **2026-11-30 or 400 further commits on the seed's default branch, whichever comes first**, after which the snapshot is taken as-is and the remaining waited-on item is re-derived on this side of the fork. | This component's architecture owner | Met | The closure removes the open-ended postponement roadmap [section 23](roadmap.gates.md#23-risks-and-stop-conditions) names as a risk. **It does not unblock JS-2**, which still waits on the row above. |

Four further dependencies were **unopened rather than blocked** — an unopened dependency has no
holder and no unblock condition, which is a weaker position than a blocked one, not a stronger
one. **JS-0 opened two of them and left two unopened; a third was opened AND CLOSED on
2026-09-03**, when the language-specification edition was pinned and the document archived. Its row
is kept rather than deleted, because a dependency that stood open for eleven milestones and what it
cost is the part worth reading. The table says which is which:

| Unopened dependency | Opened at | If it has not landed |
|---|---|---|
| **OPENED 2026-08-31.** Acquisition of the regular-expression matcher and the Unicode and locale data as this checkout's own dependencies. **Owner: the profile built-ins owner**, named in [JSD-0005](decisions/0005-the-seed-waited-on-set-and-snapshot-stop-condition.md). Nothing is acquired yet; what changed is that the dependency now has a holder. **Still unacquired on 2026-09-04, and the milestone that consumes it landed anyway.** | Opened at JS-0, consumed at JS-6 | JS-6 excludes every surface needing it and publishes the exclusions, rather than waiting. `broiler.javascript.regexp` is already a separate manifest identity, so the exclusion is a manifest not yet minted rather than a hole in one that is. **Neither of those two routes is the one taken.** JS-6 landed on 2026-09-04 with regular expressions **translated onto `System.Text.RegularExpressions`** — inside `broiler.javascript.wide` rather than behind a manifest identity of their own, declared as an approximation in the file that makes it, and with the Octane RegExp benchmark running and failing its own checksum — and with `Date` fixed to UTC where the locale half of this dependency would have been consumed. **An approximation is a third state this row did not anticipate**: it neither waits nor excludes, and what it costs is a surface that answers wrongly instead of refusing. **The route itself is corrected nowhere** — JS-6's row still asks for the matcher and for the separate manifest identity, and [JSC-70](roadmap.corrections.md#jsc-70) records this manifest admitting a `RegExp` only as the second of its two reasons the manifest could not be called `core`, on decision [JSD-0021](decisions/0021-the-wide-bring-up-manifest-and-format-version-2.md). The dependency itself is **still open** and still has the holder JS-0 gave it. |
| **The language-specification edition is PINNED, and all three of section 24's actions are done. CLOSED 2026-09-03** *(the row read "not pinned, and JS-0 did not pin it" until that morning; corrected: [JSC-67](roadmap.corrections.md#jsc-67), and closed the same day when the document was archived)*. The pin is **ECMA-262, 17th edition (ES2026)**, at `tc39/ecma262` commit **`0248456c758431e4bb8e5d26333ff1865123c9cd`** — a commit rather than the `es2026` tag, because a tag can be moved — with `spec.html` of 2,978,793 bytes hashing to **`ce7bc30174061fd8d212270b81cf6511661180c1e174f6911d10ced0581527b0`**. **Retrieved, hashed and ARCHIVED**: the document is retained in this repository at [`docs/specification/`](specification/README.md) under the Ecma alternative copyright notice, whose full text is retained beside it as that licence requires, with the entry and the scoping confirmation in [`THIRD_PARTY_NOTICES.md`](../../../THIRD_PARTY_NOTICES.md). [JSD-0019](decisions/0019-the-pinned-language-edition-and-what-two-of-three-actions-buy.md) records the pin and the alternatives refused. | This component's architecture owner | **Met.** The digest is now checkable in a checkout with no network, and rule **N14** checks it on every run of the suite: it hashes the archived bytes and compares them against the declared constant, which is the clause archiving made possible. **What this does NOT do is accept a manifest** — acceptance needs a human review this component does not have — and it says nothing about the conformance-suite row below, which is separately licensed material of a different size and is still open. |
| **The conformance-suite revision is PINNED AND THE SUITE IS ARCHIVED. CLOSED 2026-09-03** *(the row read "is not pinned" that morning, and the state it replaced was not merely transient but SELF-CERTIFYING — corrected: [JSC-68](roadmap.corrections.md#jsc-68))*. The pin is **test262 at `tc39/test262` commit `ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`**, content digest **`46d54f57ae3a4803c6ebc5f4625dd4b417254ed65058836732f182801e1cfe93`** over 56,560 files, held at [`src/tests/conformance/pins/`](../../tests/conformance/pins/README.md) where the suite cannot reach it; `--expect` makes a run answerable to it. Taken **twice**, the second retrieval extracted into a fresh directory and hashed independently. **Archived as the archive it was retrieved as**: one file of 9,487,173 bytes hashing to `f58ce79141529c9fa33592e22ff3ff0d83b83830ac8e372ecd32e1623db1c040`, four per cent of the 232 MB the extracted form would have cost, carrying the same evidence — that a reader can check the digest with no network. **The licence and attribution obligations are discharged in this same change**, which is what section 14 asks for: the suite is BSD 3-Clause material and `THIRD_PARTY_NOTICES.md` now carries its row, with the scoping confirmation and the release owner's non-independent co-signature. [JSD-0020](decisions/0020-the-retained-conformance-suite-pin-and-the-one-it-replaces.md) records the pin and what was refused. | This component's architecture owner | **Met.** All three of section 24's actions are done and rule **N15** hashes the archived bytes against the pin on every run of the suite. **And the first floor over third-party material is set**, at [`floor-test262.txt`](conformance/floor-test262.txt): 1,063 Script cases and 21 Module cases at that revision, which the lane checks **once per invocation on one Linux runner** rather than on every publish cell — the figures are the engine's answers over a fixed corpus and no runtime identifier changes them, so the exclusion is that **no other RID has scored this suite**. It could not have been set the day before: a ratchet over material somebody else can change is a promise about a directory this repository does not hold. **The floor is not a conformance claim**: 1,084 cases of a suite of 56,560 files were scored and roadmap section 14 forbids publishing an aggregate percentage. What it guards is what an exit code cannot — a case that stops being scorable is reported SKIPPED and a run full of skips exits zero. **JS-3a stays `In progress` on its other clauses.** **A second manifest is run against a checkout at this revision from 2026-09-04**, through the conformance composition's `--test262` mode and over subtrees somebody named. It adds nothing to the floor above and sets none of its own: a run over part of a suite measures that part, so roadmap [section 6](roadmap.md#6-feature-manifests-how-the-language-surface-is-admitted)'s rule is unmet for `broiler.javascript.wide` and this row closes nothing for it. |
| **This profile's declared defaults are catalog-wide and unreconciled.** Roadmap section 3 records that a host adopting profile defaults gets the tightest in the catalog, so a neighbour's stingy default reaches this profile wherever ceilings are adopted rather than stated, and that reconciling two profiles' declarations belongs to whichever component composes them. That component does not exist and has no owner. **Narrowed 2026-08-31**: the maxima half of this row was retired when the core removed a catalog-wide maximum clamp its own record never authorised. A maximum now binds only the artifacts of the profile that declared it. **PARTLY OPENED 2026-08-31**: [JSD-0004](decisions/0004-limit-defaults-hard-maxima-and-the-budget-matrix.md) records the fifteen defaults and fifteen maxima with the split stated inside the decision, and chooses `NestedLoadDepth`'s default at 4 rather than the 1 this profile would need, precisely so a neighbour adopting defaults is not strangled. **The reconciliation itself is still unowned.** | JS-0 recorded the vectors; the composing component owns the reconciliation | A browser composition that adopts defaults discovers it as a resource exhaustion naming a dimension this profile did not breach, in a verifier that did nothing wrong. |

---

## 4. Required evidence bundle

Every status claim beyond `Not started` must point to a retained bundle carrying all applicable
fields below. **A command written in a plan is not evidence that the command ran.**

| Field | Required record |
|---|---|
| **Identity** | Milestone and item IDs, roadmap and gate revision, core contract version, format version, feature manifest set, evidence-bundle ID, collection timestamp, owner, and reviewer. |
| **Source** | Component commit, recursive submodule revisions, dirty-tree state and patch identity, and the exact paths and projects under test. |
| **Dependencies and corpus** | Lockfile and package identities, toolchain and SDK versions, corpus and fixture hashes, the pinned conformance suite revision, and applicable provenance or licence decisions. |
| **Environment** | OS, architecture, RID, hardware or lane identity, runtime mode, configuration, JIT/trimming/Native AOT mode, effective environment variables, and resource limits. Secrets redacted without hiding semantically relevant configuration. |
| **Procedure** | Exact commands, working directories, ordered setup, inputs, repetitions and seeds, timeouts, and clean or pristine-consumer conditions. |
| **Results** | Raw outputs retained, including failures. A bundle that retains only the passing half is not a bundle. |
| **Negative controls** | Each control, the injection that must make it fail, and the revert that must make it pass. The count is stated and grows across milestones. |
| **Closure** | For any Native AOT claim: the published output's dependency closure, read off the published image rather than asserted. |
| **Exclusions** | What the bundle does **not** show. Every open gate clause, every unexercised path, every single-machine or single-RID limitation, named. |

---

## 5. Update rules

1. Update this ledger in the same change that accepts, rejects, blocks, supersedes, or materially
   narrows a milestone claim. Preserve earlier evidence links and decisions as dated history.
2. Do not copy a planned exit gate into the evidence column. Link the immutable bundle and state
   what it demonstrated, **including its failures and its exclusions**.
3. Do not infer completion transitively. JS-1 acceptance does not accept JS-2; a slice-manifest
   result does not accept a later manifest; and JIT, trimmed, or one-RID success does not accept
   an untested Native AOT or RID claim.
4. Do not promote seed, shell, smoke, analyzer-only, or shape-only results beyond what they prove.
   A failing or partial bundle is retained but leaves the milestone `In progress` unless a named
   dependency meets the `Blocked` definition.
5. If a gate changes, record the gate revision and re-evaluate existing evidence. Evidence
   gathered against a different population is not silently carried forward. A core contract
   amendment is such a change: record the new version and state, per affected record, what
   recertifies unchanged, what must be re-collected, and what is superseded.
6. **Do not record core work here, and never record profile work in the core's ledger.** A core
   result never advances a row in this file, and no row here advances a row there.
7. A milestone moves to `Accepted` only after its owner and reviewer confirm that every objective
   exit condition for that record is covered. Record the decision date and the evidence-bundle ID
   in the affected row. Where owner and reviewer are the same person, record the
   non-independence in the row rather than resolving it by assertion.
8. **Human review gates a release, not a development step.** Development work — implementing a
   milestone, landing it, collecting its evidence — may proceed and merge without a review
   decision. A **release** may not: no package is published, no RID is claimed, no support table
   is issued, and no milestone moves to `Accepted` until a named human has read the work and
   recorded a decision on every relevant code unit, bound to that declaration's fingerprint so a
   unit that changes afterwards reports stale rather than being silently carried.

   Two consequences are worth stating plainly, because this component will feel them harder than
   a greenfield one would. **Unreviewed work accumulates**, and this component starts with a large
   copied body of it: everything the snapshot brings in is unreviewed here on the day it lands,
   and the review debt is real from the first commit rather than from the first release. And **a
   development step that lands unreviewed carries its risk forward rather than dissolving it** —
   a passing conformance run over an unreviewed parser is a statement about the parser's outputs,
   not about the parser.
9. **A copied unit records its origin.** Every unit taken from the seed is annotated as ported,
   and the origin distribution is published in the generated assurance report. A component whose
   report cannot say how much of it was written here is a component whose review status cannot be
   read.
10. **No count, total, graph, commit, or score is copied into prose.** This ledger names the
    command or the retained record that reads it. A number transcribed into a sentence goes stale
    silently, and a ledger that goes stale silently is worse than one with a gap in it.

---

Until such updates are recorded, section 2 remains the complete status of this component: **no
milestone is accepted, no snapshot has been taken, no language surface is supported, no
composition is advertised, no runtime identifier is claimed, no measurement exists, no manifest has
a conformance run of its own, and nothing has been reviewed.**

A closing summary that restates a table rather than pointing at it is a second copy of the status,
and the second copy is the one that goes stale. This one restates only what no milestone can
change without changing the table *(corrected: JSC-20)*.
