# Evidence bundle JS-3A-002

**Milestone:** JS-3a. **What this bundle adds to [JS-3A-001](../js-3a/README.md):** the public-API
baseline clause, open since JS-0 and closed here.

**JS-3A-001 is not superseded and is not edited.** A retained bundle is immutable — the rule this
component wrote for itself at [JSD-0010](../../decisions/0010-which-review-rules-govern-this-profiles-documents.md)
— so the registry half's evidence stays where it was collected and this is a second bundle beside
it. Read them together: 001 is the diagnostic registry and the position encoding, 002 is the
frozen public surface.

**Verdict this bundle supports:** JS-3a is **In progress**, not accepted. The oracle half of its
exit gate is still untouched and still waits on a human pinning a third-party suite, and **nothing
here has been reviewed**.

Produced by `eng/collect-js-evidence.py`. Every file beside this one is its output.

**No result from any other component is evidence here.**

## Identity

| Field | Value |
|---|---|
| Bundle | `JS-3A-002` |
| Milestone | JS-3a |
| Adds | The public-API baseline clause carried from JS-0 and JS-1 |
| Registry revision | 1, unchanged from JS-3A-001 |
| Core contract version | 1 (implemented; **not accepted**) |
| Format version | 1, accepted range 1–1 |
| Claimed RID | `win-x64`, and nothing else |
| Owner | profile contract owner |
| Reviewer | **none** |

## What this bundle claims

The JavaScript profile family's three assemblies export exactly what
[`docs/api/public-api.txt`](../../api/public-api.txt) declares — 24 exported types over 269 lines
— compared in both directions by rule **N10**, with the surface described from the build output by
`MetadataLoadContext`, **which reflects without running anything**.

That last clause is the whole reason this took until JS-3a. `ApiSurface` describes a surface by
*loading* an assembly; loading needs a project reference; **rule A11 forbids a test project to
reference a profile assembly**, and that prohibition is one of the properties this component
exists to demonstrate rather than an obstacle to route around. `Assembly.LoadFrom` over the built
file would have worked and is refused on principle: loading **runs module initializers**, which
invariant 2 forbids and rule B5b exists to detect, so a describer built on it would execute the
code it is describing.

**N10 asserts the non-execution rather than citing it.** A type obtained from the context refuses
to hand out a runtime handle, and the rule makes that a clause — so a future edit swapping in
`LoadFrom` fails there instead of passing quietly.

## Procedure and results

| Step | Log | Result |
|---|---|---|
| Release build of the whole solution | `build.log` | Succeeded, **0 warnings**, 0 errors |
| Whole test suite | `suite.log` | 207 contract tests and 138 architecture tests passed, 0 failed |
| Assurance gate mode | `assurance-gate.log` | Passed |
| Assurance release mode | `assurance-release.log` | **Refused, as it must.** Every relevant unit is `HUMAN_PENDING` |
| Publish and run, both roots × three modes | `publish-and-run.log` | **6 publishes, 6 runs, all exit 0** |
| Closure reports | `closure-*.txt` | 6 and 7 managed assemblies under JIT and trimmed; 0 under Native AOT |
| Negative controls, suite-judged | `negative-controls.log` | **16 injected, 16 failed while injected and passed after revert, 0 skipped** |
| Negative controls, corpus-judged | `corpus-controls.log` | **7 injected, 7 failed while injected and passed after revert, 0 skipped** |

The architecture suite is 138 rather than JS-3A-001's 132: six of them are N10's own, and the
count is over a wider corpus in another way too — the review-document rules read this profile's
documents now, which they did not when JS-0-001 and JS-1-001 were collected.

**The control that matters for this bundle is `N10-a-public-member-appears-without-a-baseline-entry`.**
A public constant added to the format assembly, the baseline left alone, the suite red; reverted,
green. That is the direction that matters for a profile: these assemblies are referenced by
composition roots, so a member added here is a member a composition can bind to without anyone
deciding it should be bindable — and until this milestone nothing in this component would have
noticed.

## What the clause cost to close, and what it says about the ledger

It was open from JS-0, could not be closed at JS-1 for a reason JS-1-001 recorded correctly, and
was carried to **JS-3b — which is blocked on JS-2, which is blocked on the core's acceptance
gate**. So the last open gate clause on two milestones sat behind two blockers it needed neither
of. [JSD-0012](../../decisions/0012-the-profile-api-baseline-and-where-its-clause-lives.md)
re-homes it here and records why, on the same grounds the JS-3a/JS-3b split was made in the first
place: a clause nobody can schedule and a clause nobody has scheduled read the same in a table.

## Two corrections landed with it

Both were invisible until one describer had to serve two loaders:

- **The describer wrote a base type into one baseline and not the other.** It skipped `object` by
  comparing against `typeof(object)`, and a type described through a `MetadataLoadContext` has its
  own `System.Object` — not the running runtime's — so the identity comparison was false for every
  profile type. Compared by name now.
- **The packable baseline was rewritten with the platform's newline on every regeneration.**
  `WriteAllLines` uses `Environment.NewLine` and `.gitattributes` stores the file as LF, so a
  regeneration on Windows produced a twelve-hundred-line diff on a run that changed nothing — in
  the one file whose diff a reviewer is supposed to be reading. Both writers emit LF explicitly.

## Exclusions — what this bundle does not show

1. **The oracle half of JS-3a is still untouched**, and it is the larger half. No suite revision is
   pinned, no harness exists, no totals are published, no ratchet is set. The suite-revision
   dependency is open and needs a human.
2. **This baseline is over a build output, not a package.** Nothing in the family packs — rule N4
   keeps it that way until JS-10 — so what is frozen is what a composition root in this repository
   can bind to. When JS-10 takes the packaging decision, whichever assembly becomes packable
   becomes rule M1's subject too, and JSD-0012's separation is re-examined then.
3. **It reads the last build in this configuration**, taken from the test run's own output path.
   A run that has not built the profile describes nothing, and the rule fails on the empty surface
   rather than passing over it — but it is a build output and not an artefact retained by this
   bundle.
4. **The second route JS-1-001 named is not taken and not needed.** A composition root printing its
   own surface would put a reflection host in a product assembly, which is what the closure reports
   exist to keep out.
5. **Roadmap section 7's third discipline is still not implemented.** The ordering assertions —
   ceilings materialised before the first byte is read, a refusal before the allocation it would
   have authorised, a declared count compared against its bound before it sizes anything, *asserted
   mechanically for every corpus entry* — are not here. One ordering is observed, by
   `unsupported-profile-examines-no-payload-byte`, and it is a different one. Named here because it
   is a JS-1 gate discipline that no bundle has yet shown.
6. **One RID, one machine.** `win-x64`, because it was published and run.
7. **Nothing is reviewed.** Every relevant unit in this component is `HUMAN_PENDING`, and a frozen
   surface nobody has read is a surface frozen at whatever it happened to be.
