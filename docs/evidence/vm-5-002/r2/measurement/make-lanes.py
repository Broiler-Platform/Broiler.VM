"""Snapshot the three lanes E10 of VM-5-002's re-collection runs, and write what each lane is.

    python make-lanes.py <lanes dir> <lanes.txt>

base is a byte copy of the command-line host's output directory in D:/Broiler.VM-base (f127d92 with the
E9 measurement patch), credit a byte copy of the same directory in D:/broiler-arms/credit-measure (the
remedy head 16e3d6d with the same patch), and aa a second byte copy of credit's source, so the A/A lane
is an identical build measured in the same interleaving. The lanes directory lies outside every
repository, so the identity lines measure-shapes.py writes for a lane name no tree head; lanes.txt
gives, for each lane, the identity lines of the tree it was copied from (identity.py, with every file
the lane holds and its SHA-256 and product version), and whether each copied file equals its source.
Nothing is built here.
"""
import filecmp
import pathlib
import shutil
import subprocess
import sys

HERE = pathlib.Path(__file__).resolve().parent
CLI = "src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0"
SOURCES = {
    "base": "D:/Broiler.VM-base",
    "credit": "D:/broiler-arms/credit-measure",
    "aa": "D:/broiler-arms/credit-measure",
}
lanes = pathlib.Path(sys.argv[1])
report = pathlib.Path(sys.argv[2])
out = []
for lane, tree in SOURCES.items():
    source = pathlib.Path(tree) / CLI
    target = lanes / lane
    if target.exists():
        shutil.rmtree(target)
    shutil.copytree(source, target)
    files = sorted(p for p in target.rglob("*") if p.is_file() and p.suffix in (".dll", ".exe"))
    ident = subprocess.run(
        [sys.executable, str(HERE / "identity.py"), "--build", "e10 lane %s, copied from %s" % (lane, source.as_posix()),
         "--tree", tree] + sum((["--bin", str(p)] for p in files), []),
        capture_output=True, text=True)
    out.append("lane %s: %s, copied from %s" % (lane, target.as_posix(), source.as_posix()))
    out += ident.stdout.splitlines()
    if ident.returncode != 0:
        report.write_text("\n".join(out) + "\n", encoding="utf-8", newline="\n")
        sys.exit(97)
    same = all(filecmp.cmp(p, source / p.relative_to(target), shallow=False)
               for p in target.rglob("*") if p.is_file())
    out.append("# every file of the lane equals its source byte for byte: %s" % same)
    out.append("")
report.parent.mkdir(parents=True, exist_ok=True)
report.write_text("\n".join(out) + "\n", encoding="utf-8", newline="\n")
print("\n".join(l for l in out if "Runtime.dll" in l or l.startswith("lane") or "byte for byte" in l))
