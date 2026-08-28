# Code-assurance rule witnesses

Fixture sources and report fragments that the group J rules must reject, and a handful they must
accept. They carry a `.cs.witness` or `.md.witness` extension so the SDK never globs them into the
build and the assurance scanner never mistakes one for a product file.

They are **one per clause, not one per rule**, for the reason `../review/README.md` gives: a
witness asserted with a bare non-empty check pins only whichever clause happens to fire first, and
several independent clauses can then be deleted in one patch with nothing red. Every assertion in
`AssuranceRuleTests` names the *content* of the violation it expects - the field, the fingerprint,
the reviewer, the header line - rather than checking that some list is non-empty.

Most of the C# witnesses declare the same method, `Fold`, so that one baked fingerprint
(`44EBF3`) serves every witness that needs a real one. That constant is asserted against the
fingerprinter itself in J3: a witness that computed the value it is supposed to record would agree
with itself whatever the fingerprinter did, and a change to normalization must fail loudly rather
than quietly making four witnesses vacuous.

## The reviewer identifier in here is not a person

Several witnesses carry `WITNESS-ONLY` on a `Broiler-Human:` line. A rule about false approvals
cannot be witnessed without an input that contains one. It appears nowhere outside this directory,
and J4 asserts in the other direction that **no** product unit and **no** generated artefact names
anyone at all: every human line in Broiler.VM reads `PENDING`, because nothing here has been
reviewed by a human.

## What each group covers

| Rule | Clauses witnessed |
|---|---|
| J1 | The three ways a block stops covering its unit - never written, written below the declaration, written as half a block - and the two accepting directions: the exemption predicate, and the per-unit escape hatch. Then the three places the predicate was too broad: a constructor that assigns its parameters to the wrong members, a delegation that supplies a literal or an enum member rather than forwarding its own parameters, and an initialized `const` or `static readonly` field, which was not a code unit at all. Each of those three carries its accepting direction in the same file, so the case is narrowed rather than deleted. |
| J2 | Each closed vocabulary, each required field, each malformed-exemption shape, a field with no `=`, a well-formed block attached to no declaration, and a `Spec=` citation naming an ADR that is not in `docs/adr/`. Then the four ways the falsification criterion stops being an instruction anybody can act on: a line that states no criterion, one that states a `Key=Value` field, one wrapped onto a second line, and one standing below the human line where the block parses without it. |
| J3 | A recorded value the code does not produce, the unresolved placeholder, a human approval the code has outrun, and the three that must be accepted: a `Previous=` history, a review of the current version, and an `EXEMPT=` line, which records no fingerprint and is not required to. |
| J4 | A live reviewer, the `VERIFIED` state, a preserved `Previous` reviewer, the accepted `PENDING`, and the four generator transitions - including the refusal to promote `PENDING`, and the refusal to clear `STALE`. One witness is driven through `DesiredSource` rather than by calling the guard, so what it pins is the CALL SITE: a human line outside the four defined shapes must make a real generation throw. |
| J5 | A header count that is not true, a header claiming a human review, a file with no header, a hand-written SPDX pair with no generated marker, a header that is already current, and four report fragments whose figures disagree with the units they describe. Then the clause that gates the checkout: an artefact written to a real file and read back, so the comparison runs against bytes on a disk rather than against a string the test is holding. And a file carrying a second, forged assurance block below the real one, which used to survive regeneration verbatim and become a fixed point. |
| J6 | A conditional region the build compiles and a default parse would have discarded as trivia, and a `#pragma` that hides no declaration at all - because the rule is about every directive and not only about `#if`. The first witness carries the other half too: under the scanner's parse the region is active, so the method inside it is a scanned, annotated, fingerprinted unit. |
| J9 | A falsification criterion carrying the review vocabulary, which is not a review claim and is outside the corpus by construction - the corpus of a source artefact is its generated header, and the same line read as a whole file IS reported, so the exclusion is the corpus rather than a blind spot. |
| J10 | A unit assessed `High` and a unit assessed `Critical` that carry no criterion, each named with the risk it was assessed at, and the `Medium` unit beside them that is not named because below High the line is permitted. Then the accepting direction over both risks. The fingerprint half needs no witness file: it is asserted on `src/Broiler.VM.Binary/VmBoundedAllocator.cs` itself, whose three criteria are reworded so that every unit fingerprint, the file fingerprint and the rendered manifest entries can be compared across the edit. |
