# JSD-0019 - The pinned language edition, and what two of the three actions buy

**Status:** Accepted. **Updated the same day: the pin is no longer provisional.** It was recorded
as provisional in the sense roadmap
[section 24](../roadmap.gates.md#24-specification-and-platform-references) defines — retrieved and
hashed and not archived — and the document was archived hours later, so all three of section 24's
actions are done and the ledger's row is closed. The body below is left as it was written, in the
state it described; where it says two of the three actions are done, the third is now done too, and
[`docs/specification/`](../specification/README.md) records it. **Nothing here accepts a manifest
or advances a milestone**, then or now.

**Date:** 2026-09-03

**Owner:** this component's architecture owner. **Co-signer:** the verification-boundary owner.
**Both roles are held by one person** and this record does not claim the co-signature is
independent.

**Milestone:** JS-3a, which [section 24](../roadmap.gates.md#24-specification-and-platform-references)
and [the ledger](../roadmap.status.md#3-open-external-dependencies) both name as the milestone that
records the pin actually taken.

## What was open

The ledger has carried this row since JS-0, and its last sentence is the one that mattered:

> **The language-specification edition is not pinned, and JS-0 did not pin it.** … JS-0 was asked to
> record the intended edition and no decision record does, because **recording an edition nobody
> has retrieved would be a pin in name only.**

That reasoning was right and it is why nothing was written down for eleven milestones. The
consequence was also recorded, in the ledger's own words: **no manifest may be accepted against an
unpinned edition, because a conformance total against a moving document is not a total.**

Two things made it worth closing now rather than later. The component had begun making
**edition-shaped claims in prose** — that `#!` is a comment "in the language since ES2023"
([JSC-61](../roadmap.corrections.md#jsc-61)), that `using` declarations are in no published edition
([JSC-66](../roadmap.corrections.md#jsc-66)) — and those claims were checked against nothing. And
[JSD-0018](0018-which-tests-are-about-this-language-and-who-decides.md) had just made a suite's own
feature list the authority on which constructs are the language, while recording that this was the
**nearest thing to an edition that anything here pins**. A second authority was overdue.

## 1. The pin is a commit, and the edition name is how it was found

| | |
|---|---|
| Standard | ECMA-262 |
| Edition | 17th (ES2026) |
| Source | `tc39/ecma262` |
| Tag | `es2026` |
| **Revision** | **`0248456c758431e4bb8e5d26333ff1865123c9cd`** |
| Document | `spec.html`, 2,978,793 bytes |
| **Digest** | **`ce7bc30174061fd8d212270b81cf6511661180c1e174f6911d10ced0581527b0`** |
| Archived | **no** |

Re-derived by anyone, in one line:

```
curl -sS https://raw.githubusercontent.com/tc39/ecma262/0248456c758431e4bb8e5d26333ff1865123c9cd/spec.html | sha256sum
```

**A tag was rejected as the pin.** `es2026` is how the commit was found and is recorded as that; a
tag can be moved and a commit cannot, and section 24 asks for an *immutable* revision identifier.
The tag also resolves to the same commit as `es2026-candidate-2026-03-31`, which is a useful check
on having found the right one rather than a second thing to pin.

**`es2026-errata` was rejected**, though it is later — `d89c03f2` of 2026-07-28 against
`0248456c` of 2026-03-31. Errata accumulate: pinning them means pinning a moving target under a
name that sounds fixed, which is the failure the whole row is about. The published edition is what
is pinned, and an erratum that matters becomes a reason to re-pin, recorded like any other.

**The specification's SOURCE is what is hashed, not the rendered document.** `spec.html` in the
repository is what the published HTML and PDF are generated from, so it is the more precise
artifact and the one a revision identifier actually addresses. The trade-off is stated rather than
hidden: a reader comparing this digest against a PDF downloaded from ECMA will not get a match, and
should not expect one.

**Why the latest published edition and not an older one.** The manifest
`broiler.javascript.slice` admits only constructs that have been in the language since ES5, so
almost any edition would carry them; what an edition actually decides here is the *boundary* —
which constructs exist at all — and that boundary is what the conformance harness needs. Pinning an
older edition would declare a language this component knows more about than it claims to.

## 2. What two of the three actions buy, and what the third would

Section 24 asks for a document **retrieved, hashed and archived**. Two are done. The third is a
human action and nobody has performed it, exactly as with the conformance suite
([JSC-30](../roadmap.corrections.md#jsc-30)), so the pin is provisional and the ledger carries the
exclusion with its holder and its unblock condition.

**What retrieval and hashing already buy is not nothing, and it is worth naming rather than
waiting.** Three prose claims this component had made about the language are now checked against a
fixed document rather than against memory:

| Claim, and where it was made | Checked against the retrieved editions |
|---|---|
| `#!` is a comment "in the language since ES2023" ([JSC-61](../roadmap.corrections.md#jsc-61)) | **Confirmed.** `HashbangComment` is absent from ES2022 and present from ES2023 onward. |
| `using` declarations are in no published edition, which is why 121 cases were excluded ([JSC-66](../roadmap.corrections.md#jsc-66)) | **Confirmed.** No `UsingDeclaration`, `DisposableStack` or `SuppressedError` appears in the pinned edition. |
| A binding used before its initialiser is a runtime `ReferenceError` ([JSC-62](../roadmap.corrections.md#jsc-62)) | **Confirmed.** The uninitialised-binding clauses are in the pinned edition and in every edition retrieved beside it. |

**And the pin immediately found one disagreement with the other authority, which is what a second
authority is for.** Of the twenty-one flags the pinned checkout of test262 lists under *Proposed
language features*, **twenty carry no marker in the pinned edition and one does**:
`regexp-duplicate-named-groups` is in ES2025 and ES2026 — the `MightBothParticipate` machinery
appears from ES2025 — while the suite still calls it a proposal. So
[JSD-0018](0018-which-tests-are-about-this-language-and-who-decides.md)'s exclusion removes 19
files that ARE about this language.

**It moves no figure, and that is measured rather than assumed:** none of those 19 was scored even
by the run that had no exclusion at all, because a regular expression is not in this manifest. What
changes is that the disagreement is **known and bounded** instead of being the unquantified risk
JSD-0018 recorded.

## 3. The pin is declared in code, so a run states it and an edit cannot move it quietly

`JavaScriptLanguageEdition` in the profile assembly carries the edition, the revision, the digest
and — as a field rather than as a paragraph — **whether the document has been archived**.

- **The conformance report carries an `edition` line** beside the suite revision it already
  carried, because those are the two pinned inputs and a total is about neither alone. The delivery
  map lists "a manifest scored against an unpinned edition" among the failures this milestone must
  not produce; a report that names its edition is what makes that checkable after the fact.
- **A report scored against a different edition is refused rather than merged.** Two shards built
  against two editions are two runs whatever their totals look like.
- **The end-user host prints it under `--version`**, which is where a person asks what this
  implements, with two acceptance rows over it — one for the manifest and one for the words *NOT
  archived*, because a version string that quietly started claiming a fully taken pin would pass a
  check that only looked for the edition's name.
- **Rule N14** holds the code, this record and the ledger to naming the same revision and the same
  digest, and holds the `Archived` field to the ledger's account of the pin in **both** directions.

## What this refuses to do

- **It archives nothing, and no third-party document is in this repository.** The specification was
  retrieved into a temporary directory, hashed and read. That is the same discipline the pinned
  conformance suite is under and for the same reason.
- **It accepts no manifest.** The ledger's rule is that no manifest may be accepted against an
  unpinned edition; a provisional pin does not turn that into a yes, and acceptance additionally
  needs a human review this component does not have.
- **It does not make the feature filter edition-aware.** Feature flags do not map onto clauses
  mechanically, so [JSD-0018](0018-which-tests-are-about-this-language-and-who-decides.md)'s reader
  still uses the suite's own split. The one known disagreement is recorded above with what it costs.
- **It claims no conformance.** An edition name is not a conformance claim, the same way a manifest
  name is not, and this component implements a fraction of the document it has just pinned.

## What it is for, stated plainly

For eleven milestones the honest answer to *which JavaScript is this* was that nobody had written
it down, and the reason given was a good one: an edition nobody had retrieved would be a pin in name
only. Somebody has now retrieved it. What that buys is smaller than acceptance and larger than
paperwork — three claims that were prose are now checked, one disagreement between this component's
two authorities is known and bounded, and every run says which document it was measured against and
that the pin is not yet fully taken.
