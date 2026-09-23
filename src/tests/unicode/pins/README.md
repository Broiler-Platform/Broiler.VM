# Unicode data pin

**Owner:** the profile built-ins owner named in JSD-0005, as the holder of the ledger's Unicode and
locale data row. **Reviewer:** none.

The Unicode Character Database files the JavaScript profile's Unicode tables are generated from,
and the Unicode licence text, retrieved once, hashed, and archived here unmodified.
[`unicode.pin`](unicode.pin) records each file's length and SHA-256; decision
[JSD-0031](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0031-unicode-data-source-and-build-boundary.md)
section 5 is the design this follows.

## What is here

| | |
|---|---|
| Version | **Unicode 17.0.0**, the version the pinned test262 (`ccaac100`) measures and Node v24.17.0 reports |
| Source | `https://www.unicode.org/Public/17.0.0/ucd/<path>`, and `https://www.unicode.org/license.txt` for the licence |
| Files | the thirteen UCD files JSD-0031 section 5 lists, under [`ucd-17.0.0/`](ucd-17.0.0) at their UCD paths (`emoji/`, `extracted/` kept), and [`unicode-LICENSE.txt`](unicode-LICENSE.txt) |
| Test-only | `NormalizationTest.txt`: no table is generated from it; it is the conformance input slice U3 (F08) runs `normalize` against |
| Retrieved | 2026-09-22, **twice**, into two directories; `diff -r` found them byte-identical, and the first copy is the one archived |
| Not here | the three ECMAScript property tables the generator also reads - they are Ecma material and are archived beside the edition in [`docs/specification/`](../../../Broiler.VM.Profile.JavaScript/docs/specification/README.md); the pin records them by repository path |

The retrieval was performed by Claude with the repository owner's explicit permission given in
conversation on 2026-09-22. Roadmap section 24 treats retrieval as a human action; the permission
is recorded here as given, and nobody has signed anything.

## Why here

**Nothing but a test reads these files.** The generator that turns them into tables lives in the
architecture test project (JSD-0031 section 5, following rule J5's one-function pattern), and the
build compiles the generated `.g.cs` files, which are committed - so a build needs neither these
files nor a network. That makes them test inputs in the same sense as the conformance suite's
archive in [`../../conformance/pins/`](../../conformance/pins/README.md) and the Octane archive in
[`../../octane/pins/`](../../octane/pins/README.md), and they sit in the same shape: a `pins`
directory under `src/tests/<subject>/`, a pin file, a licence beside it. They are **not** under a
product project directory, so no product project can include them by a glob, and no project file
names this directory.

**What keeps them out of a shipped image is that shape, not a rule, and that is a difference from
the other two archives.** Rule N13's `SuiteDirectories` names `tests/conformance` and
`tests/octane`, so a file of those two that reached a composition's closure fails a test; this
directory is not on that list, and the containment above is checkable by reading rather than by a
rule. Adding `tests/unicode` to N13 is the obvious next step and is deliberately not taken in the
change that first archives the files.

**Every file is declared `binary` in `.gitattributes`.** The generic `*.txt text eol=lf` line would
otherwise claim them as text and let a checkout filter rewrite a line ending, which would move a
recorded hash on one platform. The pin and this README stay text: nothing hashes them.

## How the pin is enforced

Rule **N22** (`UnicodePinRuleTests` in the architecture suite) reads the pin the way the generator
does and, on every run:

- hashes every file the pin names and compares length and SHA-256;
- refuses a UCD file whose first line does not name `17.0.0` (`emoji-data.txt` is read for its
  `# Version: 17.0` line; `UnicodeData.txt` has no header and is held by its hash alone);
- cross-checks the names the ECMAScript property tables admit against `PropertyAliases.txt`;
- regenerates the tables in memory and asserts the checked-in `.g.cs` files are byte-identical.

The generator refuses to write anything from an archive that fails any of these. To regenerate
after a change to the generator: `BROILER_UNICODE_WRITE=1 dotnet test Broiler.VM.slnx -c Release
--no-build`, then regenerate the code assurance record through J5 as usual.

**Re-pinning** to a later Unicode version is a recorded decision, not an update, and moves together
with the test262 pin (JSD-0031 section 3).

## Licence

The files are © Unicode, Inc., distributed under the Unicode Terms of Use, which apply the Unicode
License v3 (SPDX `Unicode-3.0`) to data files; the text is retained here as
[`unicode-LICENSE.txt`](unicode-LICENSE.txt) and reproduced in
[`THIRD_PARTY_NOTICES.md`](../../../../THIRD_PARTY_NOTICES.md), which is packed into every package
and carries the entry for the tables derived from these files. **The files are unmodified**, which
their hashes attest.
