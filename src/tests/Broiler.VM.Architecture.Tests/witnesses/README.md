# Architecture-rule witnesses

Each file here is a deliberately invalid project file that at least one rule must reject. They
are named `*.csproj.witness` so MSBuild never globs them into the build, and they exist so that
"the rule is expressed" can be replaced by "the rule rejected this".

They are neither one per rule nor all group A's. Ten serve the eleven group A rules, because
rules A7 and A8 share an input, and the eleventh is rule D1's inbound input. Two further
witnesses are types compiled into the test assembly rather than files - `DynamicLoadingWitness`
for B5, `PublicSurfaceLeakWitness` for B4, `ModuleInitializerWitness` for B5b and
`ProfileCatalogWitness.BuiltInProfiles` for B7 - because those rules read compiled metadata and a
project file cannot express what they look for. The fixture records under `adr/` are the group E
witnesses, the fixture review documents under `review/` are the group H ones, and the fixture
sources and report fragments under `assurance/` are the group J ones - the last two are one per
CLAUSE rather than one per rule, and `review/README.md` and `assurance/README.md` say why.

`RuleRegisterTests` holds the register and this directory to each other: every Active rule must
name a witness that resolves to a file here or to a type in the test assembly, and no witness
file may sit unnamed by a rule anywhere under this directory - the orphan check recurses over
`*.witness`, so `adr/`, `review/` and `assurance/` are covered as well as this level. That check compares
whole paths. Asking instead whether some row's witness field *contained* the file name made
every orphan whose name is a suffix of a named witness invisible, and the realistic orphan is
exactly that: a witness renamed to carry its rule prefix, the register updated, and the old file
left behind.

A witness file whose name begins with a rule identifier must also be named by **that** rule's
row. Without it, the three checks above were all satisfied by a permutation of the truth -
exchanging two rows' witness fields wholesale left every path resolving, every file named by
some rule, and every count unchanged. Another rule may name the same file as well, which is how
A7 and A8 share one input. What is still not verified is that the named witness is the one the
rule's own test actually exercises; that correspondence is maintained by review.

Two rules have no witness at VM-0 and say so in the register. B3 would need a foreign Broiler
assembly, and the component's graph is closed over its own five; B6 would need a product assembly
referencing a test assembly, which cannot be built without breaking the graph. Both have
witnessed project-file twins - A1 and A2 for B3, A4 for B6.
