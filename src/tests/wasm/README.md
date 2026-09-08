# The WebAssembly profile's retained corpus

**This directory holds bytes and one text file, and nothing that runs.** `corpus/` is two hundred
and ninety WebAssembly modules, each one pinned by SHA-256 in `corpus/corpus.manifest` beside the
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
the two hundred and ninety rows, ninety-six are derived.

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
claim about the profile's internal call order.

`sound-either-way` is the inversion sweep's, and it is the strongest invariant that is **true** of
it: an inverted byte inside a custom section's payload or inside a data segment's contents produces
a different module and not an invalid one, so "refuses" would be false. What is true of all
ninety-nine is that the answer is sound — an acceptance, or a refusal carrying a code the published
enumeration holds, and never the verifier reporting its own defect.

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
published conformance assertions is run. No row is replayed under a second publish mode, because
the harness root publishes one. And no row records an execution answer — every entry stops at
verification, and the interpreter is scored by the harness's differential lane instead, where the
expected values come from a second implementation rather than from the interpreter.
