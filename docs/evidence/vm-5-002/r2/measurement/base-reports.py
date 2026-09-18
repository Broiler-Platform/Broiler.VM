"""Decompress the base test262 reports VM-5-002's first collection retained, and check each against the
digest and length the first collection's hashes.txt records for it, before the re-collection compares
anything with them.

python base-reports.py <outdir>     (writes <outdir>/<retained name less .gz>, and prints one row each)
"""
import gzip
import hashlib
import pathlib
import re
import sys

BUNDLE = pathlib.Path(__file__).resolve().parents[2]
out = pathlib.Path(sys.argv[1])
recorded = {}
section = None
for line in (BUNDLE / "hashes.txt").read_text(encoding="utf-8").splitlines():
    if re.match(r"^## \d+\. ", line):
        section = line[3:5]
    elif section == "5." and line.strip() and not line.startswith("##"):
        digest, name, size = line.split()
        recorded[name] = (digest, int(size))
names = [n for n in recorded if "/t262-base-" in n or ("/low-base-" in n and "off-series" not in n)]
bad = 0
for name in sorted(names):
    data = gzip.decompress((BUNDLE / (name + ".gz")).read_bytes())
    digest = hashlib.sha256(data).hexdigest()
    target = out / name
    target.parent.mkdir(parents=True, exist_ok=True)
    target.write_bytes(data)
    ok = (digest, len(data)) == recorded[name]
    bad += not ok
    print("%s  %s  %d  %s" % (digest, name, len(data), "matches hashes.txt" if ok else "DOES NOT MATCH hashes.txt"))
print("reports: %d, not matching: %d" % (len(names), bad))
sys.exit(1 if bad else 0)
