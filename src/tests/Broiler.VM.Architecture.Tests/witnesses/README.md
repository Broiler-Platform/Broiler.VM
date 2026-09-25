# Architecture-rule witnesses

Each file here is a deliberately invalid input that at least one rule must reject. Every one of
them carries a `.witness` extension so that MSBuild never globs it into the build - a project
witness is `*.csproj.witness` and a source witness `*.cs.witness` - and they exist so that "the
rule is expressed" can be replaced by "the rule rejected this". *(Corrected 2026-09-07: this
sentence said every file at this level was a project file, which stopped being true when the
first source witness was stored here and was not noticed then.)*

They are neither one per rule nor all group A's. Ten serve the eleven group A rules, because
rules A7 and A8 share an input, and the eleventh is rule D1's inbound input. Two further
witnesses are types compiled into the test assembly rather than files - `DynamicLoadingWitness`
for B5, `PublicSurfaceLeakWitness` for B4, `ModuleInitializerWitness` for B5b,
`ProfileCatalogWitness.BuiltInProfiles` for B7 and `NativeMappingWitness` for B5c - because those
rules read compiled metadata and a project file cannot express what they look for. The fixture records under `adr/` are the group E
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

**Rule X1's two witnesses are the one place in this repository where the page protection that
admits a write and an execute at once is written down**, and they are stored rather than
constructed because a negative control nobody has watched failing is a control in name only.
`X1-a-page-armed-read-write-and-execute.cs.witness` stands in for the arming path's Windows half
at that half's own path and carries the defect twice - as a named constant of the arming path
whose value admits both permissions, and as a bare literal passed straight into the protection
argument, which is what an author writes when the named constants do not have the value they
wanted. `X1-a-second-place-that-maps-memory.cs.witness` sits at a path outside the arming path
and names both platforms' spellings, because an author who copies one platform's imports copies
the other's on the next machine. Neither file is compiled by anything.

**Rule X2's witness is the one source witness that is compiled**, and it is compiled in memory by
the rule's own test rather than by any project. X2 reads compiled metadata, which is why its
neighbours in group B use types compiled into the test assembly; X2 cannot, because its witness has
to be `JsBaselineFrame` at that type's full name, and a second type of that name in the test
assembly would be one every other reader of that assembly could mistake for the real frame.
`X2-a-baseline-frame-field-holding-a-reference.cs.witness` carries the defect as a field that is a
reference and as a value type that holds one, because "is, or contains" is two questions.
`X3-an-unmanaged-entry-outside-the-handler-file.cs.witness` sits at a path outside the handler file
and is a native callback that both carries `[UnmanagedCallersOnly]` and parks an activation in the
thread slot, because an author who adds the entry adds the slot access in the same edit.

**Five of rule X4's eight witnesses are edits rather than files**, and they are stored as the members
the edit writes. The rule's test puts each one's members into the real file they belong to - the handler
table or the activation - in place of the members of the same name, and adds the ones the real file
lacks, because the files they edit are a hundred times the witness's size and a stored copy of either
would go stale at its next edit, the reason rule X3's rejecting directions edit the real activation.
`X4-a-call-routed-through-the-block-step.cs.witness` routes `Call` through the block step alone and
gives `Nop` a step of its own, the uniform table a tidy-up produces. `X4-a-step-that-no-longer-checks-the-opcode.cs.witness`
is `Step` without the comparison the template scan made unreachable from a verified payload, and
`X4-a-step-whose-checks-are-joined-with-and.cs.witness` is `Step` with every comparison present and
joined with `&&`, which is what a rule that found the comparisons by name would clear.
`X4-a-wrapper-that-expects-another-opcode.cs.witness` has `Nop` and `Call` expect each other's opcode and
`StepCall` answer `Construct`, and the test exchanges the two slots in the real table before it puts
them in, so every check but the rule's own agrees with it. `X4-a-handler-table-that-shadows-the-activation.cs.witness`
is a class named `JsNativeActivation` nested in the table, whose `Step` relays every mode to the block
step: the edit adds that one member, and every wrapper's unchanged text then binds to the relay. The
sixth,
`X4-a-layout-read-outside-the-scan-and-the-lowering.cs.witness`, is a whole file, read at a path in the
profile assembly where it is reported and at a path in the lowering assembly where it is not. The
seventh, `X4-a-lay-call-outside-the-scan-and-the-layout.cs.witness`, is a whole file too: a public member
of the format assembly that copies a unit's layout out through `JsBaselineBlocks.Lay`, the walk `Layout`
is made by, without naming `Layout` or `Fixed`. It is read at a path in the format assembly and at one
in the lowering assembly, where the clause reports the walk although it allows the other two names,
and in place of the template scan, where it is not reported. The eighth,
`X4-a-layout-named-through-an-escape.cs.witness`, is a whole file in the profile assembly that names the
layout and the fixed bytes only through unicode escapes: the compiler reads the escapes before it binds
the names, and the file's text, its comment included, contains neither name, so a clause that chose the
files to parse by their text would never open it. The test asserts that the text holds neither name
before it reads the two reports.

**Group U's witnesses are one per clause, and one of them has no comment on purpose.**
`U2-identifiers-naming-a-language.txt.witness` is in the input format of
`eng/ubc-vocabulary-scan.py --identifiers` - one identifier per line - and that mode scans every line
as a name, a line opening with `#` included, so a comment explaining the file would be scanned as an
identifier and would change what the script reports. The explanation lives here instead: of its five
names, `WasmModule` and `JsValue` are reported by rule U2 and by the script alike, and `UbcOpcode`,
`JsonReader` and `UbcInstructionTable` are near misses neither reports. The thirteen `U1-*` project files
each break one clause of the universal bytecode's project shape, the packability clause in eight ways: the
element set to true, quoted only in a comment, overridden, held under a condition, present only as item
metadata, only inside `ProjectExtensions` or only inside a target, and set to true by a target after an
unconditional false. The item metadata, `ProjectExtensions` and target ones - the places a reader counting
every element of that name would find a definition the evaluated property does not come from - have `N4-*`
twins. `U2-a-static-family-table.txt.witness` is four lines in the API describer's format, two of them static family rows that must be reported and two
of them - an instance property and a static method - that must not.
`U4-an-appendix-that-disagrees-with-the-table.md.witness` is a copy of the concept's Appendix A with four
perturbations, one per direction clause (a) decides, and
`U4-a-second-width-table-with-one-row-perturbed.cs.witness` is the roadmap's perturbed row: a second
width table for the common family, not compiled by anything, read at a path in a project that
references the assembly, where every arm is reported, and at the one table's own path, where none is.
The two `api/U9-*` witnesses are the universal bytecode's baseline with one member removed and one
member that does not exist added, as W2's are. The three `diagnostics/U8-*` witnesses are one per clause
of the registry rule, beside group N's registry witnesses: a registry omitting a declared code, whose
other rows carry a name that is not its number's member and a revision the registry does not have; a
registry whose rows name a corpus entry the manifest does not have, entries that expect another code
or another reason, an entry of a sweep, and a defensive row the rule does not admit; and a source,
read as though it were a file of the assembly, that emits one code with two reasons, the second
through a forwarding helper of its own so the rule has to read the helper's body to see it.
