# The conformance floor

**Owner:** verification-boundary owner. **Reviewer:** none.

[`floor.txt`](floor.txt) is the ratchet roadmap
[section 14](../roadmap.md#14-the-conformance-oracle) asks for: the first per-host-mode totals
admitted for a suite revision, which no later run of that revision may fall below.

**It is a measurement discipline and not a status.** *Admitted* is deliberately not the
[ledger](../roadmap.status.md)'s `Accepted`, which additionally needs a reviewer decision nothing
in this component has. A floor that could only be set by an accepted milestone would be a floor
nothing could ever set. Nothing here advances a milestone, and a floor holding is not evidence for
any gate.

**It records the revision it was set under, and is never compared across revisions.** A suite that
added tests would otherwise read as a regression, and one that removed them would silently lower
the bar. A revision change re-bases the floor from the first run admitted on the new revision, and
the old floor stays in the file on a `retired` line with the reason. This is the discipline the
[diagnostic registry](../diagnostics/registry.txt) and the retained corpus already apply to their
own pinned revisions.

**Re-basing is a decision somebody makes.** The component's CI lane compares against this file and
fails a run that falls below it; it never writes it. A run on a revision the floor was not set
under reports that and changes nothing, because a lane that re-based its own floor would be a
ratchet with no teeth.

**What may set one.** A merged run of every shard, carrying no configuration failure and having
executed something. A single shard's run may not, and neither may a run that covered less than it
claimed to cover: the whole value of a floor is that the number behind it was measured over the
selection it names.

## The current floor

Set on 2026-09-03 from the merged three-shard run retained in
[Bundle JS-3A-004](../evidence/js-3a-004/README.md), on `win-x64`. The suite is this component's
own — `broiler.javascript.slice.js-3a` — because roadmap section 14 builds the harness against the
smallest scoring target that exists rather than after the language it will eventually score. **No
third-party conformance suite is pinned**, so no floor over one exists and none is implied by this
file.
