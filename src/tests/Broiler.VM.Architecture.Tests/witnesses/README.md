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
witnesses.

`RuleRegisterTests` holds the register and this directory to each other: every Active rule must
name a witness that resolves to a file here or to a type in the test assembly, and no witness
file may sit here unnamed by a rule. It does not verify that the named witness is the one the
rule's own test actually exercises; that correspondence is maintained by review.

Two rules have no witness at VM-0 and say so in the register. B3 would need a foreign Broiler
assembly, and the component's graph is closed over its own five; B6 would need a product assembly
referencing a test assembly, which cannot be built without breaking the graph. Both have
witnessed project-file twins - A1 and A2 for B3, A4 for B6.
