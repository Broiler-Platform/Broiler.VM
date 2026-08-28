# Review-record witnesses

Fixture review documents that the group H rules must reject. They are stored here, outside the
real review-document set, and with a `.md.witness` extension, so the corpus loader never globs
them into it.

They are one per CLAUSE, not one per rule, and each breaks exactly one clause and nothing else.
That is the point of them. The reverted group R asserted every witness with a bare
`Assert.NotEmpty` over an input violating several clauses at once, so each witness pinned only
whichever clause happened to fire first - and four independent clauses were then deleted in one
patch with the suite still green. Every assertion here names the CONTENT of the violation it
expects, so deleting the clause makes that assertion fail rather than quietly shifting onto
another clause's violation.

Several are fragments rather than whole documents, because the rules read fragments: a legend, a
route table, an attestation, a sentence quoting two image sizes. A fragment is read as though it
were a review document, which is what lets one clause be exercised on its own.

A SECOND ADVERSARIAL PASS ran against group H itself, and the inputs it found are all here. What
it showed is that "one witness per clause" has to be read strictly, because a clause is smaller
than it looks. Each of the four figure-phrasing lists in H5 is a set of clauses, not one clause,
so each entry now has a sentence no other entry matches; the four attestation field names and
the eight placeholder words are clauses each, so each has an input naming it; and a branch that
reports a MISSING thing is a different clause from the branch that reports a WRONG one, so the
absent-row, absent-total and absent-Area-row directions have inputs of their own. Anything that
could not be told apart from its own absence was removed rather than left as decoration - a
clause no witness can distinguish is a clause someone deletes in another patch with the suite
green.

`RuleRegisterTests` holds this directory and the register to each other in both directions:
every Active rule must name witnesses that resolve to files here, no `.witness` file anywhere
under `witnesses/` may sit unnamed by a rule, and a file named for a rule must be named by that
rule's own row.
