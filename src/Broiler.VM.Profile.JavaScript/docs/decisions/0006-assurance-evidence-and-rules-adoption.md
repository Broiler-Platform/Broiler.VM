# JSD-0006 - Adopting the component's assurance system, rule register, API baseline and evidence contract

**Status:** Accepted for JS-0, with three deviations from the roadmap recorded below and one
gate clause carried forward.

**Date:** 2026-08-31

**Owner:** profile architecture owner. **Milestone:** JS-0.

## Context

JS-0's exit gate asks this component to stand up **its own** assurance system - annotation
grammar, exemption predicate, generated review report, fingerprint binding, release-mode gate -
its own architecture rule register with witnesses and negative controls, its own public API
baseline mechanism, and its own evidence-bundle contract and collection script. The roadmap also
says, in the same breath, that the first five are **repository policy rather than this
component's inventions** and that this milestone implements them rather than redefining them,
recording a deviation if any of the five has to differ.

[JSD-0001](0001-placement-identity-and-assembly-topology.md) places this profile as a set of
product projects inside the `Broiler.VM` component, which already implements all four mechanisms
over the same tree.

## Decision: adopt, do not duplicate

This profile uses the component's existing mechanisms:

| Mechanism | Where it lives | What JS-0 changed |
|---|---|---|
| Assurance scanner, fingerprinter, generator, manifest, release gate | `src/tests/Broiler.VM.Architecture.Tests/` | The covered set grew from three product assemblies to six. Nothing else |
| Architecture rule register and witnesses | `src/tests/Broiler.VM.Architecture.Tests/rules.register.json` | Group **N** minted: four rules, nine witness inputs, each clause asserted on the content of the violation it expects |
| Public API baseline | `docs/api/public-api.txt`, rule M1 | Nothing. See the carried clause below |
| Evidence-bundle contract | `docs/evidence/`, `eng/collect-evidence.py` | A profile-owned bundle tree and a profile-owned collection script, for the reason below |

**The reason is the platform's own.** The repository's `CODE-ASSURANCE.md` records that
`Broiler.VM`'s assurance implementation is the reference one, that two profile roadmaps each
require an implementation with the same parts in the same words, that seven of the nine clauses
are identical between them, and that three implementations of one repository-level policy is the
outcome to avoid. It names the trigger for extracting a shared tool - a second implementation
existing in merged code - and asks that a component copying the mechanisms do so deliberately and
say so. **Adopting is strictly better than copying**: there is one implementation over one tree,
so there is nothing to drift.

**Rejected: a second assurance system inside the profile's own test project.** It would be a
second scanner over the same files, producing a second report of the same units, with two
answers available for "is this unit reviewed". It would also need a test project, which ADR
0001's budget does not currently permit and which would have to be justified by a boundary this
one does not enforce.

**Rejected: keeping the profile's assemblies out of the covered set until they have behaviour.**
The component's own assertion refuses it, and rightly: coverage follows the product/test
partition, and a product project that is not covered is a product project nobody would notice
going unannotated. The three assembly markers are covered from the commit that creates them.

## Deviation 1 - one generated report covers both

`CODE-ASSURANCE.md`, `HUMAN_REVIEW.md` and `assurance.manifest.json` now describe six product
assemblies rather than three, so the component's review debt and this profile's are reported in
one place.

**What this costs, stated rather than left implicit.** The roadmap's invariant 12 says no
evidence transfers and every claim starts at zero, and the status ledger's update rule 6 says not
to record core work in the profile's ledger or profile work in the core's. A single generated
report is in tension with the *presentation* of that rule, and the tension is resolved this way:
the report is a **measurement of the tree**, not an evidence bundle, and it attributes every unit
to its own assembly and every file to its own path. No JS row in the profile's status ledger may
cite it as evidence that a gate closed, and no core milestone may cite the profile's units as its
own coverage. **The rule that no result transfers is unchanged; what is shared is the instrument,
not the reading.**

## Deviation 2 - the rule register is the component's, and group N is the profile's

Group N's four rules sit in the component's register because there is one project graph and a
second register over the same graph would be two answers to one question. Every N rule names its
own witnesses, and the register's existing orphan check and witness-resolution checks hold them
exactly as they hold the core's.

**One core rule changed and it is recorded in the core's series**, not here: A11's sibling
exemption, in ADR 0001 revision 5. A13 was deliberately not widened, because widening it would
have weakened the rule that holds the two consumer profiles.

## Deviation 3 - the evidence bundle and its script are the profile's own

Evidence is where the ledgers must **not** merge. A JS bundle lives at
`src/Broiler.VM.Profile.JavaScript/docs/evidence/js-<n>/`, is produced by
`eng/collect-js-evidence.py`, and is cited only by the profile's status ledger. The core's
bundles under `docs/evidence/vm-<n>/` are untouched and are cited only by the core's.

**A milestone-specific script rather than a flag on the core's.** The core's collection script
publishes composition roots, replays a corpus, runs a fuzz target, a soak host and a bench host -
none of which exists in this profile at JS-0. Adding a mode to it would have made its own
behaviour conditional on which component was collecting, which is how a collection script starts
deciding things instead of running a procedure and retaining what happened.

## One cost of adoption, observed while collecting JS-0-001

**A run of the suite from profile work rewrites the core's most recent evidence bundle.** Rule D1
writes its outcome - scanned or inconclusive, and the aggregate path it saw - into the *current*
bundle, and the current bundle is read from the rule register's `milestone` field, which names a
**core** milestone. So collecting JS-0 evidence on this machine rewrote a line in the core's
retained VM-6 bundle with this machine's path. **It was reverted rather than committed**, because
a JS milestone editing a VM bundle is the ledgers merging by accident, which is the one thing
JSD-0006 does not adopt.

The core's own comment on that field records the same class of defect from an earlier version -
"history a later run edits is not history" - and the fix it took, naming the directory from the
register rather than from a literal, does not cover this case: the register has one milestone
field and there are now two milestone series in the repository. **This is recorded as an observed
wrinkle, not fixed here.** Fixing it means either a per-component current-bundle pointer or a D1
outcome that is not retained in a bundle at all, and both are the core's decision rather than
this profile's. Until then, a collector reverts what their run touched outside their own bundle,
and this paragraph is why.

## The carried clause: the API baseline does not yet cover the profile

Rule M1 compares `docs/api/public-api.txt` against the **packable** assemblies in both
directions. The profile's three assemblies are deliberately unpackable until JS-10, so they are
outside the baseline's declared subject, and at JS-0 they export no public type at all - the
three assembly markers are `internal`.

**That stops being harmless at JS-1**, which lands a descriptor accessor and payload projections
that a composition root consumes: a public surface with nothing freezing it is exactly the gap
rule M1 was minted to close. **JS-1 owns extending the baseline's subject to the profile's
assemblies**, and this clause is carried in the JS-0 evidence bundle's exclusions and in the
ledger row rather than being left for someone to notice when the surface has already moved twice.

## What this record does not decide

- **Nothing is reviewed.** Every relevant unit in this component, the profile's three included,
  is `HUMAN_PENDING`. The system records the absence of review precisely; that is its value and
  it is not a claim of safety.
- **The release-mode gate has never refused a publish of this profile**, because nothing here has
  been published. Its behaviour is asserted by the component's own group J rules over the
  component's tree, and JS-0 adds no new claim about it.
