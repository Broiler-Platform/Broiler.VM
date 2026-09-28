# The WebAssembly profile's retained corpus

**This directory holds bytes and one text file, and nothing that runs.** `corpus/` is three hundred
and sixteen WebAssembly modules, each one pinned by SHA-256 in `corpus/corpus.manifest` beside the
answer it must produce. The encoder that wrote them and the replay that reads them both live in
`src/compositions/Broiler.VM.Composition.WebAssembly.Harness/` — rule A11 forbids a project outside
`src/compositions/` to reference a profile assembly, so an encoder that produces the bytes a
verifier is asked to accept or refuse cannot be a test project, and that is forced rather than
chosen.

## What a manifest row says

    name|sha256|family|outcome|reason|diagnostic|dimension|scope|provenance|invariant

The outcome, the reason and the diagnostic code are the triple an embedder reads back. The
dimension and the scope are the pair rule N11 fixed for the JavaScript profile's corpus and this
one adopts: **a resource exhaustion carries no diagnostic code**, so
`ResourceExhaustion/CeilingReached/0` is the same answer for a declared-count ceiling and a
structural-depth one, and a row recording the category alone could not tell a verifier that refuses
the right module for the wrong reason from one that does not.

## The two provenances, and what each is worth

**`derived`** — a person wrote the answer down from the format before the profile was asked. The
writer refuses to emit a manifest at all when the profile contradicts one: the declaration wins and
the run stops, so a regeneration cannot quietly record the profile's answer over the person's. Of
the three hundred and sixteen rows, a hundred and twenty-two are derived. *(Corrected 2026-09-28: the
corpus had two hundred and ninety rows, ninety-six derived, until eight derived rows were added for
custom-section names, import entries and a function type with two results. Nine more were added the
same day, when limits and constant expressions moved to validation: two for limits and seven for
constant expressions. Nine more again when the diagnostic registry was published: one each for the
tag section, the data count and a body whose locals overrun it, two for a segment's first field, one
for a body declared past the artifact's end, and the three phase-order rows below.)*

**`recorded`** — the answer came from the profile at the moment the corpus was written. It detects
a change between one regeneration and the next and it **proves no correctness**. Nobody hand-derives
a hundred and ninety-four diagnostic codes for two mutation sweeps without inventing most of them,
and a corpus that pretended otherwise would be worth less than one that says so. What holds a
recorded row is its invariant.

## The invariant column, which is the half that survives a regeneration

A hand-written claim about the family, never regenerated, checked on every replay and on every
write: `accepts`, `refuses-decoding`, `refuses-validation`, `exhausts`, `refuses`,
`sound-either-way`. The decode-and-validate split is checkable from outside because this profile
numbered it — diagnostic codes below 2700 are the decoder's and 2700 and above are the validator's
— so "this module was refused before validation began" is a claim a caller can check rather than a
claim about the profile's internal call order. The family hook's codes and the translation's own
limit codes, numbered in the upper half of the 2800s since the universal bytecode programme's
milestone UBC-4, fall on the validator's side of that line, and no retained row records one.
*(Noted 2026-09-28: the band tells the PASS, and the category a refusal names is its reason. A
malformation the validator meets inside a function body carries a 2800-band code with a malformation
reason, as draft decision WAD-0004 in the profile's decisions records.)*

**The `phase-order` family** holds modules that are invalid at a byte before the one at which they
are malformed - a function type, a memory, and a function body - and each must be refused as
malformed, with a decoding code. A build that validated any of them before it had decoded the rest of
the module would answer invalid, so these are the named cases WA-3's gate asks for that fail when the
two phases are fused at module granularity.

## The diagnostic registry, and the revision line

Every code a row records is published in
[the profile's diagnostic registry](../../Broiler.VM.Profile.WebAssembly/docs/diagnostics/registry.txt),
with its passes, its carrier, its one reason and the named case that reaches it. The manifest's
second line, `# registry-revision: N`, is the registry revision the writer dated it with - the writer
takes it from `CorpusStore.RegistryRevision` - and every code below it is read against that revision.
Rule W3 holds the two together: a corpus row of the registry names a derived entry here, every entry
recording a code records its row's reason, and the manifest is never dated past the registry.

`sound-either-way` is the inversion sweep's, and it is the strongest invariant that is **true** of
it: an inverted byte inside a custom section's contents after its name, or inside a data segment's
contents, produces a different module and not an invalid one, so "refuses" would be false. What is true of all
ninety-nine is that the answer is sound — an acceptance, or a refusal carrying a code the published
enumeration holds, and never the translator or the verifier reporting a defect of its own.

## Running it

    # write the corpus (a mode of its own, never a side effect of a run)
    dotnet run --project src/compositions/Broiler.VM.Composition.WebAssembly.Harness \
        -c Release -- --write-corpus src/tests/wasm/corpus

    # replay it, twice, against the recorded hashes and answers
    dotnet run --project src/compositions/Broiler.VM.Composition.WebAssembly.Harness \
        -c Release -- --corpus src/tests/wasm/corpus

    # check the bytes against the manifest, then move one and require the replay to notice
    python3 eng/wasm-corpus-integrity.py --corpus src/tests/wasm/corpus -- \
        <the replay command above>

## What is NOT here

There is no entry produced by a fuzzer and none minimised from a counterexample: every module was
written by hand by the same person who wrote its expected answer, so **this corpus cannot find a
refusal nobody thought of**. There is no reader for the specification's text format, so none of the
published conformance assertions is run. *(Corrected 2026-09-28: the harness root's `--spec` lane
reads the specification's pinned scripts, in `src/tests/wasm/spec/`, and runs them; see record
[WA-SPEC-001](../../../docs/evidence/wa-spec-001/README.md). This corpus holds none of them.)* The manifest records one answer per row and no publish
mode: a replay under the harness root's trimmed or Native AOT publish is retained, where it is
retained at all, in the evidence bundle that ran it, and never here. And no row records an execution
answer. Since the universal bytecode programme's milestone UBC-4 a module is source: every entry
stops at the profile's translator, which refuses it in the fields the core's verification answers,
or at the core's verification of the universal bytecode artifact the translator wrote. What runs a
module — the bytecode emitter executing that artifact over the profile's family — is scored by the
harness's differential lane instead, where the expected values come from a second implementation
rather than from what runs the module.
