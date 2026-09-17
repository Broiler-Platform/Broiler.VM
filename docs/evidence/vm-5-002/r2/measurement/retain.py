"""Copy the re-collection's transcripts from the throwaway directory into r2/, LF-normalised.

    python retain.py <throwaway dir> <r2 dir>

Every file is copied under the same relative name, with CRLF turned into LF for text, which is how the
repository stores it; a .gz report is copied byte for byte. Only the files the README names are copied:
the shard transcripts of the test262 runs, the decompressed base reports and the interrupted native run's
shards stay in the throwaway directory, as the first collection's did. Prints one line per directory.
"""
import pathlib
import shutil
import sys

src = pathlib.Path(sys.argv[1])
dst = pathlib.Path(sys.argv[2])

PLAN = [
    ("gates", "gates", None),
    ("e2", "e2", None),
    ("e3", "e3", None),
    ("control16", "control16", None),
    ("e4", "e4", None),
    ("e5", "e5", None),
    ("e6", "e6", None),
    ("e7", "e7", None),
    ("e7-runs-console.log", "e7/e7-runs-console.log", None),
    ("off-series/t262-credit-native-interrupted-driver.log", "e7/off-series/t262-credit-native-interrupted-driver.log", None),
    ("off-series/README-interrupted.txt", "e7/off-series/interrupted-run.txt", None),
    ("e8", "e8", None),
    ("e8-runs-console.log", "e8/e8-runs-console.log", None),
    ("e9", "e9", None),
    ("e10", "e10", None),
    ("e12", "e12", None),
    ("e13", "e13", None),
    ("machine-state-test.txt", "machine-state-test.txt", None),
    ("machine-state-before.txt", "machine-state-before.txt", None),
    ("machine-state-after.txt", "machine-state-after.txt", None),
    ("timing-driver.log", "timing-driver.log", None),
]


def copy(a, b):
    b.parent.mkdir(parents=True, exist_ok=True)
    data = a.read_bytes()
    if a.suffix != ".gz":
        data = data.replace(b"\r\n", b"\n")
    b.write_bytes(data)


for s, d, _ in PLAN:
    a, b = src / s, dst / d
    if not a.exists():
        print("absent, not copied: %s" % s)
        continue
    if a.is_file():
        copy(a, b)
        print("file %s -> %s" % (s, d))
        continue
    n = 0
    for f in sorted(a.rglob("*")):
        if f.is_file():
            copy(f, b / f.relative_to(a))
            n += 1
    print("dir %s -> %s: %d files" % (s, d, n))
