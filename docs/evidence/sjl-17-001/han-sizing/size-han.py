#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Size the radical-stroke order of every unified ideograph against rule N28's headroom (SJL-17.3, decision E-o3h).

Recomputes every figure the sizing gave, from three inputs, and writes them to standard output:

  size-han.py UNIHAN_ZIP PROPLIST VECTORS_DIR

UNIHAN_ZIP   Unicode 17.0.0's Unihan.zip, https://www.unicode.org/Public/17.0.0/ucd/Unihan.zip, whose SHA-256 must be
             f7a48b2b545acfaa77b2d607ae28747404ce02baefee16396c5d2d7a8ef34b5e; it is read in place, not extracted.
PROPLIST     Unicode 17.0.0's PropList.txt, as this repository pins it: src/tests/unicode/pins/ucd-17.0.0/PropList.txt.
VECTORS_DIR  conformance/vectors/collation of Broiler.JS's corpus revision 34 (60fc3a5b14098e53181a8422d62c159b1e6fb701),
             written by: git archive 60fc3a5b14098e53181a8422d62c159b1e6fb701 -- conformance/vectors/collation

The order: each unified ideograph's first kRSUnicode value (radical, simplified-radical marks, residual strokes),
then the core block (U+4E00..U+9FFF and U+F900..U+FAFF) before the others, then the code point. The forms measured:

  fixed     the order as runs of consecutive code points, each run its first code point in 3 bytes and its length
            in 2 bytes, little-endian;
  delta     the same runs, each the zigzag LEB128 varint of its first code point less the previous run's end, then the
            LEB128 varint of its length;
  xz        the fixed form compressed by Python's lzma (the xz container) at preset 9 extreme;
  rank      each unified ideograph's rank in the order, 17 bits a code point in code point order, packed;
  reach     a table of only the ideographs the collation vectors name, 3 bytes of code point and 3 of rank each.

The same fixed form big-endian, compressed alike, is reported as well. Rule N28's budget and use are bundle
sjl-8-001's (README section 12, the table "All four"), restated here as constants.
"""
import hashlib
import io
import lzma
import os
import re
import sys
import zipfile

UNIHAN_SHA256 = "f7a48b2b545acfaa77b2d607ae28747404ce02baefee16396c5d2d7a8ef34b5e"
N28_BUDGET = 832 * 1024     # bundle sjl-8-001's README: CldrTablesRuleTests.Budget
N28_USED = 840365           # bundle sjl-8-001's README: "All four", the Intl assembly's tables


def sha256(data):
    return hashlib.sha256(data).hexdigest()


def unified_ideographs(text):
    out = set()
    for line in text.splitlines():
        line = line.split("#")[0].strip()
        if not line:
            continue
        rng, prop = [x.strip() for x in line.split(";")]
        if prop != "Unified_Ideograph":
            continue
        if ".." in rng:
            a, b = rng.split("..")
            out.update(range(int(a, 16), int(b, 16) + 1))
        else:
            out.add(int(rng, 16))
    return out


def radical_stroke(text):
    rs = {}
    for line in text.splitlines():
        if "\tkRSUnicode\t" not in line:
            continue
        cp, _, val = line.split("\t")
        m = re.match(r"(\d+)('*)\.(-?\d+)$", val.split()[0])
        rs[int(cp[2:], 16)] = (int(m.group(1)), len(m.group(2)), int(m.group(3)))
    return rs


def uleb(n):
    out = bytearray()
    while True:
        b = n & 0x7F
        n >>= 7
        if n:
            out.append(b | 0x80)
        else:
            out.append(b)
            return bytes(out)


def zigzag(n):
    return n << 1 if n >= 0 else ((-n) << 1) - 1


def reached(vecdir, uni):
    found = set()
    for name in sorted(os.listdir(vecdir)):
        if not name.endswith(".vectors"):
            continue
        for line in open(os.path.join(vecdir, name), encoding="utf-8"):
            if not line.startswith(("in:", "out:", "script:")):
                continue
            units = [int(m.group(1), 16) for m in re.finditer(r"\\u([0-9A-Fa-f]{4})", line)]
            i = 0
            while i < len(units):
                c = units[i]
                if 0xD800 <= c <= 0xDBFF and i + 1 < len(units) and 0xDC00 <= units[i + 1] <= 0xDFFF:
                    c = 0x10000 + ((c - 0xD800) << 10) + (units[i + 1] - 0xDC00)
                    i += 1
                if c in uni:
                    found.add(c)
                i += 1
            found.update(ord(ch) for ch in line if ord(ch) in uni)
    return found


def main():
    zip_path, proplist_path, vecdir = sys.argv[1:4]
    zip_bytes = open(zip_path, "rb").read()
    if sha256(zip_bytes) != UNIHAN_SHA256:
        sys.exit("Unihan.zip's SHA-256 is not the pinned one")
    irg = zipfile.ZipFile(io.BytesIO(zip_bytes)).read("Unihan_IRGSources.txt")
    proplist = open(proplist_path, "rb").read()

    print("inputs")
    print("  Unihan.zip sha256", sha256(zip_bytes))
    print("  Unihan_IRGSources.txt sha256", sha256(irg))
    print("  PropList.txt sha256", sha256(proplist))
    for name in sorted(os.listdir(vecdir)):
        if name.endswith(".vectors"):
            print("  vectors", name, "sha256", sha256(open(os.path.join(vecdir, name), "rb").read()))

    uni = unified_ideographs(proplist.decode("utf-8"))
    rs = radical_stroke(irg.decode("utf-8"))
    with_rs = sum(1 for c in uni if c in rs)
    print("unified ideographs", len(uni), "with kRSUnicode", with_rs, "without", len(uni) - with_rs)

    def core(c):
        return 0 if (0x4E00 <= c <= 0x9FFF or 0xF900 <= c <= 0xFAFF) else 1

    order = sorted(uni, key=lambda c: (rs[c], core(c), c))
    runs = []
    for c in order:
        if runs and c == runs[-1][0] + runs[-1][1]:
            runs[-1][1] += 1
        else:
            runs.append([c, 1])
    print("runs of consecutive code points in radical-stroke order", len(runs),
          "longest", max(r[1] for r in runs))

    fixed_le = b"".join(s.to_bytes(3, "little") + n.to_bytes(2, "little") for s, n in runs)
    fixed_be = b"".join(s.to_bytes(3, "big") + n.to_bytes(2, "big") for s, n in runs)
    delta = bytearray()
    end = 0
    for s, n in runs:
        delta += uleb(zigzag(s - end)) + uleb(n)
        end = s + n
    preset = 9 | lzma.PRESET_EXTREME
    xz_le = len(lzma.compress(fixed_le, preset=preset))
    xz_be = len(lzma.compress(fixed_be, preset=preset))

    rank = {c: i for i, c in enumerate(order)}
    rank_packed = (len(uni) * 17 + 7) // 8
    assert max(rank.values()) < 1 << 17

    reach = reached(vecdir, uni)
    headroom = N28_BUDGET - N28_USED
    print("rule N28: budget", N28_BUDGET, "used", N28_USED, "headroom", headroom)
    print("form, bytes, times the headroom")
    forms = [
        ("fixed (5 bytes a run, little-endian)", len(fixed_le)),
        ("delta (zigzag and LEB128 varints)", len(delta)),
        ("xz of fixed, little-endian (preset 9 extreme)", xz_le),
        ("rank (17 bits a code point, packed)", rank_packed),
        ("also: xz of fixed, big-endian (preset 9 extreme)", xz_be),
    ]
    for name, size in forms:
        print("  %s, %d, %.2f" % (name, size, size / headroom))
    print("ideographs the collation vectors reach", len(reach),
          " ".join("U+%04X" % c for c in sorted(reach)))
    print("  a table of those alone, 6 bytes each", len(reach) * 6)


if __name__ == "__main__":
    main()
