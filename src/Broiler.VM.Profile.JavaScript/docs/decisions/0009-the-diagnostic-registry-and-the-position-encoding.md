# JSD-0009 - The diagnostic-code registry, its two halves, and the position encoding

**Status:** Accepted for JS-3a. **Scope: the registry half of JS-3a only.** The conformance
oracle - the pinned suite revision, the harness, the self-check, the sharding and the ratchet - is
the other half of that milestone and this record does not touch it. JS-3a remains `In progress`.

**Date:** 2026-08-31

**Owner:** conformance owner. **Co-signer:** the verification-boundary owner, for the registry
split. **Both roles are held by one person** and this record does not claim the co-signature is
independent.

**Milestone:** JS-3a.

## Why a registry rather than an enum

The codes already existed. `JavaScriptDiagnosticCode` has declared them since JS-1, grouped by the
stage that emits them, and a reader could have opened the file. What did not exist is anything a
retained corpus entry could be dated against, anything that said which core reason a code carries,
and anything that would notice a code the vocabulary declares and nothing can produce.

A retained corpus entry records a diagnostic code and nothing else about it. If a code's meaning
changes between two releases, every entry that recorded it silently becomes a claim about
something else - and the corpus, which is this profile's only pinned record of what the verifier
does, would go wrong quietly and stay green. The registry is what makes that impossible: it states
its own revision, each row states the revision its meaning dates from, and **a rejection whose
meaning changes takes a new number while the old one is retired.**

## Decision: the registry, and what a row says

The registry is published at
[`docs/diagnostics/registry.txt`](../diagnostics/registry.txt) at **revision 1**, one row per
code, with eight columns:

| Column | What it says |
|---|---|
| `code` | The number the core carries in its profile diagnostic-code field |
| `name` | The member of `JavaScriptDiagnosticCode` that declares it |
| `half` | `core-result` or `embedder-seam` - see the next section |
| `reason` | **The one** core `VmReason` every emission of this code carries |
| `stage` | The pass that refuses: header, manifest, framing, limits, constants, code, entries, positions, reserved, reader |
| `reachability` | `corpus` or `defensive` |
| `case` | The retained corpus entry that produces the code, or the reason no artifact reaches it |
| `since` | The registry revision the code's current meaning dates from |

**Nothing in the file is the authority on its own, and that is the design.** Five registered rules
bind it to four independently written artefacts, so it cannot be made to agree with everything by
being edited to match one thing:

| Rule | Binds the registry to | Failing direction |
|---|---|---|
| **N5** | The `JavaScriptDiagnosticCode` declaration | A declared code with no row; a row naming no member of its number; a row dated from a revision that does not exist |
| **N6** | Every emission site in the profile assembly | A code emitted with two reasons; a row naming a reason the core does not have; a declared code nothing emits |
| **N7** | The retained corpus manifest | A row naming a case the corpus does not have, or a case that records a different code |
| **N8** | The composition's restated constants | The producer and the registry disagreeing on a number |
| **N9** | The position factories | A position built anywhere but the file that decides the encoding |

## Decision: which half each code belongs to

Roadmap [section 9](../roadmap.md#9-the-semantic-front-end-and-lowering) states the problem this
answers. Source with an early error never becomes an artifact: its diagnostic never occupies the
core's profile diagnostic-code field, never carries a byte offset into an artifact, and never
crosses a core result envelope at all. So the registry has two halves with two transports, and a
reader who could not tell them apart would be told a code travels somewhere it never does.

- **`core-result`** - the code travels in a core verification result, beside a core reason and a
  position.
- **`embedder-seam`** - the code is carried by a rejection of source, on the embedder's own
  interface.

**Every row is `core-result` at revision 1, and that is a fact rather than a simplification.** The
lowering at this milestone is hand-written and takes no source text, so there is no source
rejection to carry a code. **JS-3b mints the first `embedder-seam` code**, and the column exists
now so that it lands in a registry that already distinguishes the two rather than in one that has
to be re-cut.

## Decision: three rows are reachable from no artifact, and they are named

The exit gate asks that every code in the registry be reachable from a named case. Thirty-seven of
the forty are: a named entry of the retained corpus produces each one, and JS-3a added nine corpus
entries to close the gap it inherited. **Three are not, and each is a fact about this build rather
than a gap in the corpus:**

| Code | Why no artifact reaches it |
|---|---|
| `1003 DescriptorFormatVersionMismatch` | The core screens the descriptor's format version against the profile's registered range **before calling the profile**, and this build registers exactly one version. The check becomes reachable when the profile accepts two |
| `1006 DescriptorManifestMismatch` | The same, for the manifest: the core screens the descriptor's manifest against the accepted set, and this build accepts exactly one |
| `1903 ReaderStopped` | Every bounded-read status this build was compiled against has an arm of its own, so this arm answers a status the reader does not currently have. It becomes reachable if the core adds a ninth status and this profile is not updated |

**The list of three lives in rule N7 and not in the registry**, which is the point of it. A row
claiming to be unreachable is a row excused from the backward binding; if the registry alone
decided which rows may claim it, the excuse would be available by editing the file the rule reads.
A fourth is an edit to a test, which is a review.

**This is a stated limit on what the gate clause proves and it is carried into the ledger as one.**
The clause reads "every code in it is reachable from a named case"; thirty-seven are, three are
named as unreachable with a reason and a condition that would change it, and no row is silent.

## Decision: the position encoding

The core's position record carries four fields, two of which are explicitly the profile's own and
which the core never parses, orders, formats or compares. Roadmap
[section 7](../roadmap.md#7-the-bytecode-format-and-the-verifier) says why that has to be written
down: two profiles designing two position encodings against one shared record, neither naming it,
is how two incompatible conventions get built against one struct. This is the one for
`broiler.javascript`.

| Field | This profile's use |
|---|---|
| `SectionIndex` | The **ordinal index of the framed section** the position is inside - the index of the frame in this artifact's own section sequence, not the section kind. `-1` means the position is an offset into the artifact's byte stream rather than into a section body |
| `ByteOffset` | Always populated. **What it is an offset into is what the section index says**: artifact-relative at `-1`, section-body-relative otherwise |
| `ProfileCoordinate0` | The one-based source line from the canonical position table, or `0` |
| `ProfileCoordinate1` | The one-based source column from the canonical position table, or `0` |

**Zero in both coordinates means the position is not known**, and it is reserved for exactly that:
the verifier now refuses a position-table row declaring line or column zero, so an artifact cannot
mint an unknown-looking position a consumer would then trust. A refusal at an offset the table has
no covering row for reports zero and says so, rather than reporting the nearest row it could find.

The two shapes are two factories in
[`JavaScriptPosition.cs`](../../JavaScriptPosition.cs), and rule N9 keeps every position in the
assembly going through them.

### What this corrected

**The verifier was reporting code-section offsets with the artifact-relative marker.** Every
diagnostic the link and walk stages produce carries an offset into the *code section* - a jump
target, an unreachable instruction, an operand-stack fault - and every one of them went through a
helper that set the section index to `-1`, which under the encoding above says the number is an
offset into the artifact. The number was right and the frame it named was wrong, so a consumer
resolving it would have landed on an unrelated byte. This is exactly the failure roadmap section 7
predicted, found inside one profile rather than between two.

Four retained corpus entries now pin the encoding, and each fails differently if it moves: one
read-stage position (`-1:0:0:0`), one code-stage position in an artifact with no table
(`2:0:0:0`), one whose refusal is the covering row itself (`2:1:1:1`), and one whose refusal is
covered by the **second** of two rows (`2:3:7:5`). The manifest gained a `position` column to
carry them.

**A row that pins no position says so.** Writing a position on all fifty-nine rows would mean
hand-computing offsets into bytes the producer assembles, which no reader could check and which
any change to the writer would invalidate - and having the producer ask the verifier for them
would be recording the answer under test. The four above pin the encoding; the rest pin what they
always pinned.

## Decision: `EntryStackNotEmpty` is emitted on the edge, not at the join

`1505` was declared at JS-1 and emitted by nothing: a path arriving at an entry point with
operands on the stack was reported as an inconsistent-stack-height join, because an entry point is
seeded at height zero and the second arrival disagrees with it.

That is a worse diagnostic and it was also **order-dependent**: a join mismatch is reported by
whichever of the two arrivals the worklist happens to pop second, so which code an artifact
provoked was a property of a traversal order no artifact can see. The check is now on the **edge**
- any successor edge reaching an entry offset with a non-zero height is refused - which makes the
answer a property of the program. A named corpus entry produces it.

## Rejected: generating the registry from the source

It would never disagree with the enum, and that is the whole objection. The forward binding would
become a tautology, the reason column would be a restatement of whatever the sites happen to say
today, and N8's purpose - holding a deliberately duplicated set of constants to a third artefact -
would disappear, because there would be no third artefact. The registry is authored and the rules
make drift fail.

## Rejected: retiring the three unreachable codes

`1903` in particular is a defensive arm for a core amendment that has not happened, and deleting
it would mean the arm answers with some other code - reporting a profile defect or an unhandled
core status as an artifact fault. Retiring the two descriptor codes would mean the check has to be
written again when the profile accepts a second format version or a second manifest, which is a
milestone away. Naming them costs three rows and a rule that lists them; deleting them costs a
wrong answer at the moment the answer matters.

## Rejected: putting the position encoding in the roadmap and not in code

Roadmap section 7 asks the profile to *state* which fields it populates, and a paragraph would
have satisfied that sentence. It would not have found the conflation above, which was in the
code and not in the prose, and nothing would stop the next call site from writing a third
convention. The encoding is a pair of factories and a rule.

## What this record does not decide

- **Nothing about the conformance oracle.** No suite revision is pinned, no harness exists, no
  self-check runs, no totals are published and no ratchet is set. Those are JS-3a's other half and
  the open external dependency on the suite revision is unchanged.
- **Nothing about the verification boundary.** Whether the verifier re-derives every early error
  from artifact bytes, and what a doubly-bad artifact answers, is **JS-3b's** and is untouched
  here. What this record settles is the narrower question JS-3a was asked: which half of the
  registry a code belongs to, and what the two halves mean.
- **Nothing is reviewed.** No human has read the registry, the encoding, the rules or the corpus
  entries, and every unit this milestone touched carries `PENDING`.
