# Broiler.VM.Profile.JavaScript decision records

The JavaScript profile's own dated decisions, numbered `JSD-nnnn`. They are a **separate series
from the core's ADRs** and neither series numbers into the other: a `JSD` record decides
something about this profile, an `ADR` decides something about the Broiler.VM core, and where a
decision needs both halves each record carries its own and names the other.

The [roadmap](../roadmap.md) states planned work and objective exit gates. The
[status ledger](../roadmap.status.md) is the authority for what has been accepted. **A decision
recorded here is not evidence that it was implemented**, and a record whose subject the checkout
does not contain says so in its own text.

| Record | Decides | Milestone |
|---|---|---|
| [JSD-0001](0001-placement-identity-and-assembly-topology.md) | Where this component lives, its profile ID and package identity, and the three assemblies it is built from | JS-0 |
| [JSD-0002](0002-feature-manifest-allocation.md) | The feature-manifest identities, what each admits, and the admission criterion for the next increment | JS-0 |
| [JSD-0003](0003-deployment-composition-labels.md) | The three composition labels, what each contains, and which are advertised | JS-0 |
| [JSD-0004](0004-limit-defaults-hard-maxima-and-the-budget-matrix.md) | The fifteen limit defaults, the fifteen hard maxima, the budget declaration matrix, and which numbers are measured rather than chosen | JS-0 |
| [JSD-0005](0005-the-seed-waited-on-set-and-snapshot-stop-condition.md) | Per-item dispositions on the seed's open work, the snapshot stop condition, the satellite dependency, and the nullable and unsafe positions | JS-0 |
| [JSD-0006](0006-assurance-evidence-and-rules-adoption.md) | That this profile adopts the component's assurance system, rule register, API baseline and evidence contract rather than standing up its own, and what that costs | JS-0 |
| [JSD-0007](0007-cross-profile-position-and-amendment-grading.md) | The `WebAssembly` host-object surface as a named exclusion, the refusal of a cross-profile value channel, and the re-grading of the amendment candidates | JS-0 |
| [JSD-0008](0008-format-version-1-the-entry-point-and-what-js-1-corrected.md) | What format version 1 carries, the entry-point answer, and four things JS-1 corrected in earlier records | JS-1 |
| [JSD-0009](0009-the-diagnostic-registry-and-the-position-encoding.md) | The published diagnostic-code registry, its two halves, the three codes no artifact reaches, and this profile's use of the core's position record | JS-3a |
| [JSD-0010](0010-which-review-rules-govern-this-profiles-documents.md) | That the component's review-document rules read this profile's ledger and bundles, that the mark vocabulary is per document family, and the two clauses that do not govern them | JS-3a |
| [JSD-0011](0011-the-value-frame-and-call-abi.md) | The eight-row value, frame and call ABI, and the re-scoping of JS-6 from a copy to a rewrite that follows from it | JS-4, entry gate |
| [JSD-0012](0012-the-profile-api-baseline-and-where-its-clause-lives.md) | The family's own frozen public surface, described without loading anything, and the re-homing of the gate clause that asked for it | JS-3a |
| [JSD-0013](0013-the-fuzz-sessions-coverage-signal.md) | What a fuzz session observes as its coverage signal, why instrumenting for edge coverage is refused, and what a session judges about itself instead of its growth | JS-9 |
| [JSD-0014](0014-the-source-front-end-and-the-verification-boundary.md) | Where the verification boundary falls, that static semantics is one stage, where strict mode is ruled on, that parse options are a value, and what a nesting case gets | JS-3b |
| [JSD-0015](0015-the-conformance-oracle-and-what-it-refuses-to-score.md) | Where the conformance harness lives and why its non-advertisement is a rule, how a suite is pinned, that every case runs in a runtime of its own, and the six named ways a run is misconfigured | JS-3a |

## What a record must carry

Each record states its **status**, its **date**, its **owner** and any **co-signer**, the
**decision**, what it **rejects and why**, and — where the decision is provisional — the named
condition that would settle it and the milestone that owns settling it. A decision with no
recorded rejection is a decision nobody chose between.

**Where owner and co-signer are the same person, the record says so in those words.** The
roadmap requires the non-independence to be recorded as a limit on what the decision proves,
not resolved by assertion, and every record in this series is currently in that position.
