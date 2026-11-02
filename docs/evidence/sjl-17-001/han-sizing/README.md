<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The Han sizing of SJL-17.3

*(Added 2026-11-02, at SJL-17.3.)*

SJL-17.3 sized the radical-stroke order of every unified ideograph against rule N28's headroom, the measurement the
owner's decision E-o3h in
[JSD-0084](../../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0084-collation-moves-to-the-shared-intl-library.md)
rests on. This folder retains it; the bundle's [README](../README.md), section 3, states its figures. Nothing here is a
run of the bundle's rule.

**Inputs.**
- Unicode 17.0.0's `Unihan.zip`, from <https://www.unicode.org/Public/17.0.0/ucd/Unihan.zip>, SHA-256
  `f7a48b2b545acfaa77b2d607ae28747404ce02baefee16396c5d2d7a8ef34b5e`. It was retrieved twice on 2026-11-02, and both
  copies have that hash. It is not committed; `size-han.py` refuses any other file.
- Unicode 17.0.0's `PropList.txt` as this repository pins it,
  [`src/tests/unicode/pins/ucd-17.0.0/PropList.txt`](../../../../src/tests/unicode/pins/ucd-17.0.0/PropList.txt), for
  the `Unified_Ideograph` property.
- The collation vectors of Broiler.JS's corpus revision 34, `conformance/vectors/collation/` at
  `60fc3a5b14098e53181a8422d62c159b1e6fb701`, written by
  `git archive 60fc3a5b14098e53181a8422d62c159b1e6fb701 -- conformance/vectors/collation` (the commit of the bundle's
  [`vectors/scratch-pin-revision-34.pin`](../vectors/scratch-pin-revision-34.pin)).

**Files.**
- [`size_rs.py`](size_rs.py), the sizing script from SJL-17.3's scratch folder, kept as it was written, and
  [`size_rs.txt`](size_rs.txt), its output. It reads `Unihan_IRGSources.txt` extracted from the zip, and gives the
  ideographs, the runs and the fixed run list, the rank table and the ideographs the vectors reach. It does not give
  the other two forms the decision weighed, the delta-coded run list and the compressed one, and no script that
  computed them was kept.
- [`size-han.py`](size-han.py), written when the figures moved here, recomputes every figure in one run from the three
  inputs, the zip read in place, and [`size-han.txt`](size-han.txt) is its output. Its docstring defines each form. The
  delta-coded and compressed forms are defined so that they give the figures the session recorded, and they do; the
  other figures are those of `size_rs.py`. It reports one form more than the session weighed: the same run list
  big-endian, which compresses smaller. Rule N28's budget and use are bundle
  [`sjl-8-001`](../../sjl-8-001/README.md)'s, its section 12, written into the script as constants.
- [`hashes.txt`](hashes.txt), which `sha256sum -c` verifies.

**Commands**, each run from a scratch folder outside the repository, with Python 3.13.16 and its `lzma` over liblzma
5.4.5 (`<unihan>`, `<vectors>` and `<repo>` name that machine's copies):

```
python3 -I <repo>/docs/evidence/sjl-17-001/han-sizing/size_rs.py <unihan>/Unihan_IRGSources.txt \
  <repo>/src/tests/unicode/pins/ucd-17.0.0/PropList.txt <vectors>/conformance/vectors/collation > size_rs.txt
python3 -I <repo>/docs/evidence/sjl-17-001/han-sizing/size-han.py <unihan>/Unihan.zip \
  <repo>/src/tests/unicode/pins/ucd-17.0.0/PropList.txt <vectors>/conformance/vectors/collation > size-han.txt
```

`size-han.py` was run once on each retrieved copy of the zip, and the two outputs are byte for byte the same.
