# The WebAssembly specification's core test scripts, pinned

**Owner:** the WebAssembly profile owner. **Reviewer:** none.

This directory holds a third-party suite this repository runs and does not ship: the 73 script files of
`test/core` in [github.com/WebAssembly/spec](https://github.com/WebAssembly/spec) at commit
`977f97014c962f7bd1291fcc6d28b41a924882bf`, which the tag `wg-1.0` names. It was pinned on 2026-09-28 for
the universal bytecode programme's UBC-4 decision rule,
[`docs/evidence/ubc-4-005/decision-rule.md`](../../../../docs/evidence/ubc-4-005/decision-rule.md), whose
population A is every command of these scripts.

| File | What it is |
|---|---|
| [`wasm-spec.pin`](wasm-spec.pin) | The revision, the archive's SHA-256, the content digest over the 76 files it extracts to, and how both were taken |
| `wasm-spec-977f97014c962f7bd1291fcc6d28b41a924882bf-test-core.tar.gz` | `test/core` at the commit, unmodified: the 73 `.wast` scripts, and the directory's `.gitignore`, `README.md` and `run.py` |
| [`wasm-spec-test-LICENSE.txt`](wasm-spec-test-LICENSE.txt) | The Apache License 2.0 text of the upstream repository's `test/LICENSE`, which covers everything under `test/` |
| [`wasm-spec.ratchet`](wasm-spec.ratchet) | This profile's own file, not suite material: WA-4's ratchet, the passing totals of the malformed and invalid families at this revision, which no later run under the same manifest and limits falls below. Set by record [WA-SPEC-004](../../../docs/evidence/wa-spec-004/README.md) on 2026-09-29; the lane holds a run to it with `--ratchet` |

**Why this revision.** The profile runs the surface of the original standardised WebAssembly: the
numeric instructions with floats, structured control, one memory, one table, globals, the start function
and segments, and no import. `wg-1.0` is that standard's test suite. A later revision's scripts use
instructions this profile refuses at validation, such as sign extension, in the same modules as the ones
it runs. Pinning one of those would turn most of the suite into refusals and measure nothing.

**How it is read.** Extract the archive and hand the harness root the `test/core` directory. The harness
checks the directory against the pin before it reads any script:

```bash
tar xzf src/tests/wasm/spec/wasm-spec-977f97014c962f7bd1291fcc6d28b41a924882bf-test-core.tar.gz -C <scratch>
<harness> --spec <scratch>/test/core --expect src/tests/wasm/spec/wasm-spec.pin
```

The extracted files are scratch and are never committed. No project file names this directory, and rule
**N13** asserts that none does, so no suite file is carried into a build output.

**Licence.** Everything under the upstream repository's `test/` is licensed under the Apache License 2.0,
retained here as that licence's section 4(a) requires. The files are unmodified, so none is marked as
changed. The attribution row is in [`THIRD_PARTY_NOTICES.md`](../../../../THIRD_PARTY_NOTICES.md).
