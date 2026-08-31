# JSD-0012 - The profile's public-API baseline, and re-homing the clause that asked for it

**Status:** Accepted for JS-3a.

**Date:** 2026-08-31

**Owner:** profile contract owner. **Co-signer:** the component's API-baseline owner. **Both roles
are held by one person** and this record does not claim the co-signature is independent.

**Milestone:** JS-3a.

## The clause, and how it came to be parked

**JS-0** recorded that the public API baseline's subject is the packable set, so it does not cover
this profile's assemblies. It carried the clause to JS-1 on a reasonable ground: the profile
exported nothing public yet, and JS-1 would land a surface.

**JS-1** landed one and could not close it, and bundle JS-1-001 recorded why. `ApiSurface`
describes a surface by loading an assembly; loading needs the assembly in the test project's
output; that needs a project reference; and **rule A11 forbids a test project to reference a
profile assembly**. The prohibition is not an obstacle to route around — it is one of the
properties this component exists to demonstrate. The bundle named two routes out and carried the
clause to **JS-3b**.

**JS-3b is blocked on JS-2, which is blocked on the core's acceptance gate.** So the last open gate
clause on two milestones sat behind two blockers, and needed neither of them. That is the same
defect the JS-3a/JS-3b split was made to fix: roadmap
[section 19](../roadmap.delivery.md#19-milestones) split them because leaving the conformance
harness fused to the copied front end "put this component's only external correctness signal behind
**both** of its external blockers when it needed to be behind neither".

## Decision: the clause is re-homed to JS-3a and closed there

Not to JS-3b, and not to a milestone of its own. JS-3a is where this profile's *published
artefacts* landed — the diagnostic registry at [JSD-0009](0009-the-diagnostic-registry-and-the-position-encoding.md)
— and a frozen public surface is the same kind of thing: a file that says what this component
promises, bound to the code by a rule.

**JS-3b keeps nothing of it.** The clause is discharged, not moved again.

## Decision: describe from metadata, without executing

Route one of the two JS-1-001 named. Rule **N10** compares the family's three assemblies against
[`docs/api/public-api.txt`](../api/public-api.txt) in both directions, with the surface described
by `MetadataLoadContext` over the projects' build output.

**Why not the reference.** A11 forbids it, and the rule exists to describe a boundary rather than
to cross it.

**Why not `Assembly.LoadFrom` over the built file, which would have worked.** Loading **runs module
initializers**. Invariant 2 forbids them and rule B5b exists to detect them, so a describer built
on loading would execute the code it is describing, and would pass or fail partly on what that
execution did. This is the same reason `AssemblyFacts` reads metadata tables directly for the group
B rules rather than loading and reflecting. `MetadataLoadContext` runs nothing, and **rule N10
asserts that rather than citing it**: a type obtained from the context refuses to hand out a
runtime handle, and the assertion is there so that a future edit swapping in `LoadFrom` — which
would work — fails here instead of passing quietly.

**One describer, two loaders.** Every line of both baselines is written by `ApiSurface`'s own type
and member describers; only the loading differs. A reader who can read one file can read the other,
and a change to how a member is spelled moves both.

**Two subjects, two files.** This is deliberately not a widening of rule M1. M1's own non-vacuity
clause is that it covers the packable assemblies **and nothing else**, and none of this family
packs — rule N4 keeps every one of them unpackable until JS-10 takes the packaging decision. A
single rule holding two disjoint subjects to one baseline would be freezing a surface twice, and
two records of one surface can disagree.

## What this baseline is, and what it is not

**It freezes what a composition root in this repository can bind to. It is not a package surface**,
because there is no package. When JS-10 takes the packaging decision, whichever of these assemblies
becomes packable becomes M1's subject as well, and this record's separation is re-examined then.

**It reads the last build in the configuration the test run was built in**, which the rule takes
from its own output path rather than assuming Release. A run that has not built the profile
describes nothing, and the rule fails on the empty surface rather than passing over it — it asserts
that every named assembly was found and that the surface is non-empty, because an empty surface
compared against an empty baseline would agree.

## Two corrections landed with it

**The two describers disagreed on one line, and the disagreement was invisible.** `ApiSurface`
skipped a base type by comparing it against `typeof(object)`. A type described through a
`MetadataLoadContext` has its own `System.Object`, which is not the running runtime's, so the
identity comparison was false for every profile type and wrote `System.Object` into one baseline
and not the other — two spellings from one describer, which is the thing having one describer was
for. The comparison is by name now.

**The packable baseline was rewritten with the platform's newline on every regeneration.**
`File.WriteAllLines` uses `Environment.NewLine`, and `.gitattributes` stores the file as LF, so
every regeneration on Windows produced a twelve-hundred-line diff on a run that changed nothing —
in the one file whose diff a reviewer is supposed to be reading. Both writers emit LF explicitly
now.

## Rejected: leaving the clause at JS-3b and recording that it is schedulable

It is the cheapest honest option and it was considered. It was rejected because the ledger would
then carry, for two milestones, an open clause with a note saying it could be closed at any time —
which is a worse record than either closing it or being blocked on it. A clause nobody can schedule
and a clause nobody has scheduled read the same in a table.

## What this record does not decide

- **Nothing about packaging.** JS-10 owns it, and rule N4 holds every family project unpackable
  until then.
- **Nothing about the second route.** JS-1-001 named a second — a composition root printing its own
  surface, compared the way rule K3 compares a catalog table. It is not taken and not needed; it
  would put a reflection host in a product assembly, which is what the closure reports exist to
  keep out.
- **Nothing is reviewed.** No human has read the rule, the baseline or this record, and a frozen
  surface that nobody has read is a surface frozen at whatever it happened to be.
