# Retained suite pins

**Owner:** verification-boundary owner. **Reviewer:** none.

A pin this repository holds for a suite it does not hold. The harness takes a suite as a directory
on a command line — [`test262.pin`](test262.pin) is what says which directory that may be.

## Why a pin lives here rather than in the suite

`--pin` computes a digest over a directory and writes it into that directory. For a suite this
repository holds — `js-3a`, `ingest-shape` — that is exactly right: the pin travels with the files
it covers and an edit to either moves the revision.

**For a suite this repository does not hold it is worth nothing, and worse than nothing because it
looks like a pin.** A `suite.pin` generated inside a third-party checkout certifies that the
directory has not changed since the harness last looked at it. It certifies nothing about which
upstream revision the directory *is*, and whoever can edit the suite can regenerate the pin in the
same gesture. Every test262 figure this component has published was obtained under exactly that
arrangement, and the ledger described it as a pin over a transient checkout — true, and not the
part that mattered *(corrected: [JSC-68](../../../Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-68))*.

A retained pin fixes the authority in a file the suite cannot reach:

```bash
--suite <checkout> --expect src/tests/conformance/pins/test262.pin
```

A run given one refuses a checkout whose name, content digest or file count is not the one this
repository decided. A run not given one behaves as before, and a suite carrying no revision at all
still reports `MissingSuiteRevision`.

## What a pin holds, and what it does not

| | |
|---|---|
| `revision` | The **immutable upstream commit** roadmap [section 14](../../../Broiler.VM.Profile.JavaScript/docs/roadmap.md#14-the-conformance-oracle) asks for — "never a branch name" |
| `archive` / `archive-sha256` | Where the retrieval came from and what those bytes hashed to |
| `content-sha256` | The digest the harness computes over every path and content the checkout produces, its own pin excepted |
| `files` | The count, checked beside the digest: a digest says two things differ, a count says how |
| `archived` | Whether anybody holds the material. **`no` today** |

**No suite content is here and none may be.** test262 at the pinned revision is 232 MB over 56,560
files; what is retained is the pin, the licence, and the record of how the pin was taken. Archiving
the material is a further human action, and `archived` is the field that would say so.

## How this pin was taken

Retrieved **twice** on 2026-09-03, from the codeload archive of the named commit, into two
directories. The two downloads were byte-identical, and the second was extracted into a fresh
directory and hashed **independently of the first** — which is what makes `content-sha256` a second
reading rather than a copy of the first one. Anybody can repeat it:

```bash
curl -sSL -o t262.tar.gz https://codeload.github.com/tc39/test262/tar.gz/ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e && sha256sum t262.tar.gz
```

## Licence

test262 is © 2012 Ecma International under the **BSD 3-Clause** terms retained here as
[`test262-LICENSE.txt`](test262-LICENSE.txt). **This repository redistributes no part of the suite
except that licence file**, so its conditions are met trivially and in advance: the day a suite
file does land here, the notice it would need is already in the tree rather than being discovered
at a publish.

The attribution row roadmap section 14 defers to "the change that first ingests a suite file" is
**not** discharged by this change, because this change ingests none. What has expired is the reason
that row could not be *written*: [JSC-30](../../../Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-30)
deferred it because an attribution for material nobody had retrieved would be an attribution for
material nobody had read, and the material has now been retrieved, hashed and read.
