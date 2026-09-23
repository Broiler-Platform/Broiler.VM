# The archived language specification

**Owner:** this component's architecture owner. **Reviewer:** none.

[`ecma-262-es2026-spec.html`](ecma-262-es2026-spec.html) is the language-specification edition this
profile's manifests are defined against, retained here because roadmap
[section 24](../roadmap.gates.md#24-specification-and-platform-references) asks for the edition to
be **retrieved, hashed and archived** and this is the third of those three.

| | |
|---|---|
| Standard | ECMA-262, 17th edition (ES2026) |
| Source | `tc39/ecma262` |
| Tag it was found by | `es2026` |
| **Revision** | `0248456c758431e4bb8e5d26333ff1865123c9cd` |
| Bytes | 2,978,793 |
| **SHA-256** | `ce7bc30174061fd8d212270b81cf6511661180c1e174f6911d10ced0581527b0` |
| Retrieved and hashed | 2026-09-03 |
| **Archived** | **2026-09-03** |

## Verifying it, which is the whole point of archiving rather than citing

The digest is a **public constant** of this profile — `JavaScriptLanguageEdition.DocumentDigest` —
and it is in the profile's API baseline and on every conformance report. Two commands check that
the file here is the file that names:

```bash
sha256sum src/Broiler.VM.Profile.JavaScript/docs/specification/ecma-262-es2026-spec.html
```

```bash
curl -sS https://raw.githubusercontent.com/tc39/ecma262/0248456c758431e4bb8e5d26333ff1865123c9cd/spec.html | sha256sum
```

The first is what archiving buys and the second is what it replaces. **A pin whose document lives
only at a URL is a pin that depends on somebody else's uptime and somebody else's history**; the
first command works in a checkout with no network and would keep working if the upstream
repository were rewritten or removed. Rule **N14** holds the constant, the decision record and the
ledger to naming the same digest, so the three cannot drift apart in silence.

**The file is declared `binary` in `.gitattributes`**, and that is not a claim about its content —
it is HTML and reads as text. It is a claim about what may happen to it: one byte of end-of-line
conversion on one platform would make a published constant describe a file nobody has.

## Licence, and what it required

The specification's natural-language text is licensed under the **Alternative copyright notice of
the Ecma text copyright policy**, which the `tc39/ecma262` `LICENSE.md` names at the archived
revision. It permits copying and distribution "for any purpose and without fee or royalty",
subject to three conditions, all of which are met here:

1. **The full notice text is retained** in
   [`ECMA-alternative-copyright-notice.txt`](ECMA-alternative-copyright-notice.txt), as text in
   this repository rather than as a link.
2. **Pre-existing notices are kept**: the document is unmodified and carries its own.
3. **Notice of changes** — there are none, and that is stated rather than omitted.

[`THIRD_PARTY_NOTICES.md`](../../../../THIRD_PARTY_NOTICES.md) carries the entry this ingestion
required, with the confirmation that the notice's opening claim about the three packable assemblies
is unaffected.

## The three Unicode property tables, archived 2026-09-22

The edition does not contain the tables that say which Unicode property names a regular
expression may use: `spec.html` pulls them in with three `<emu-import>` elements, so the archived
document has none of the 53 binary property names in it. Decision
[JSD-0031](../decisions/0031-unicode-data-source-and-build-boundary.md) section 5 therefore adds
the three imported files to the Unicode retrieval, and they are archived here, beside the edition
they belong to, from the **same revision** `0248456c758431e4bb8e5d26333ff1865123c9cd` of
`tc39/ecma262`:

| File | Bytes | SHA-256 |
|---|---|---|
| [`table-nonbinary-unicode-properties.html`](table-nonbinary-unicode-properties.html) | 1,033 | `08da59f7725ab8c8c932c965d22de931bbf38b1ea9430f67a2cd0163984074f2` |
| [`table-binary-unicode-properties.html`](table-binary-unicode-properties.html) | 10,528 | `bb09794cb0cf887f011f5d1a133b2a29734e432ba0cf87db9281afbd151b8ea0` |
| [`table-binary-unicode-properties-of-strings.html`](table-binary-unicode-properties-of-strings.html) | 702 | `9c4e35660c28217fb8dd7786de72010c30f901e534a57e3c16eef4831d6bf0e1` |

They were retrieved twice on 2026-09-22 from
`https://raw.githubusercontent.com/tc39/ecma262/0248456c758431e4bb8e5d26333ff1865123c9cd/<name>`,
by Claude with the repository owner's permission given in conversation, and the two retrievals
were byte-identical. They are unmodified, so their own notices are intact and there are no changes
to give notice of; **the Ecma notice above covers them on the same three conditions as the
edition**, because they are part of the same specification text at the same revision.

**They do not change `JavaScriptLanguageEdition.DocumentDigest`**, which names `spec.html` alone,
and rule N14 is unaffected. They are held instead by rule **N22**: `src/tests/unicode/pins/unicode.pin`
records the same lengths and hashes, the Unicode table generator reads the admitted names from
these files and refuses them if a byte has moved, and N22 checks that this table and the pin agree.
They are declared `binary` by the existing `docs/specification/*.html` line in `.gitattributes`.

## What is here and what deliberately is not

**One edition is archived: the one that is pinned**, with three of its own imported tables (the
section above). Four more editions were retrieved and hashed on
2026-09-03 to check three claims this component had made about the language in prose — that `#!` is a
comment from ES2023, that `using` declarations are in no published edition, and that a binding used
before its initialiser is a runtime `ReferenceError`. Those four are **not** archived, because they
are not the pin; they are cited by revision so the measurement can be repeated:

| Edition | `tc39/ecma262` revision |
|---|---|
| ES2022 | `d711ba960cd12b7658d6bb26d7556e690290190c` |
| ES2023 | `d048f32e861c2ed4a26f59a50d392918f26da3ba` |
| ES2024 | `0b24a049c11fe0604b1c929772e7cbd671b78492` |
| ES2025 | `84b38ad852ff426795fa29cebc06949027336c64` |

**No conformance suite is here and none may be.** The suite is separately licensed third-party
material of a different size and a different obligation, and its row in
[section 3 of the ledger](../roadmap.status.md#3-open-external-dependencies) is still open. That
this document could be archived says nothing about that one.

**Archiving accepts nothing.** [JSD-0019](../decisions/0019-the-pinned-language-edition-and-what-two-of-three-actions-buy.md)
records the pin and what it does not buy: this component implements a fraction of the document it
has pinned, an edition name is not a conformance claim, and no manifest is accepted by any of this.
