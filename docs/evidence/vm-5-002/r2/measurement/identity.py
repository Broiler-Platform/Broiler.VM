"""The identity lines every transcript of VM-5-002's re-collection begins with, written by its driver.

    python identity.py --build <label> --tree <dir> [--bin <file> ...] [--note <text> ...]

Prints, one per line and each prefixed "# ": the branch and head of the main checkout D:/Broiler.VM,
the tree the run uses and that tree's head and working-copy changes, the build label, the start time
(ISO 8601 with offset), and for each binary the run loads its path, SHA-256 and the product version the
SDK writes from the informational version (whose suffix after '+' is the commit the build was stamped
with). Exits 97 without printing the rest when the main checkout is not on the branch this work runs
on, so a wrapper that checks the exit code stops before the run.
"""
import argparse
import ctypes
import datetime
import hashlib
import pathlib
import struct
import subprocess
import sys

MAIN = "D:/Broiler.VM"
BRANCH = "claude/fuel-credit-block-steps"


def git(tree, *args):
    p = subprocess.run(["git", "-C", tree] + list(args), capture_output=True, text=True)
    return p.stdout.strip() if p.returncode == 0 else "(git %s failed: %s)" % (" ".join(args), p.stderr.strip())


def product_version(path):
    try:
        version = ctypes.windll.version
    except AttributeError:
        return "(no version API)"
    size = version.GetFileVersionInfoSizeW(str(path), None)
    if not size:
        return "(no version resource)"
    buffer = ctypes.create_string_buffer(size)
    if not version.GetFileVersionInfoW(str(path), 0, size, buffer):
        return "(version resource unreadable)"
    pointer, length = ctypes.c_void_p(), ctypes.c_uint()
    if not version.VerQueryValueW(buffer, "\VarFileInfo\Translation", ctypes.byref(pointer), ctypes.byref(length)):
        return "(no translation)"
    language, codepage = struct.unpack("<HH", ctypes.string_at(pointer.value, 4))
    key = "\StringFileInfo\%04x%04x\ProductVersion" % (language, codepage)
    if not version.VerQueryValueW(buffer, key, ctypes.byref(pointer), ctypes.byref(length)) or length.value == 0:
        return "(no product version)"
    return ctypes.wstring_at(pointer.value, length.value).rstrip("\0")


def lines(build, tree, binaries, notes):
    branch = git(MAIN, "branch", "--show-current")
    out = ["branch=%s" % branch]
    if branch != BRANCH:
        return out + ["STOP: the main checkout is not on %s" % BRANCH], 97
    out += [
        "head=%s" % git(MAIN, "rev-parse", "HEAD"),
        "tree=%s" % tree,
        "tree-head=%s" % git(tree, "rev-parse", "HEAD"),
        "tree-changes=[%s]" % ";".join(
            l for l in git(tree, "status", "--porcelain", "--untracked-files=no").splitlines()),
        "build=%s" % build,
        "started=%s" % datetime.datetime.now().astimezone().isoformat(timespec="seconds"),
    ]
    for b in binaries:
        path = pathlib.Path(b)
        if path.is_file():
            out.append("binary=%s sha256=%s version=%s" % (
                path.as_posix(), hashlib.sha256(path.read_bytes()).hexdigest(), product_version(path)))
        else:
            out.append("binary=%s MISSING" % path.as_posix())
    out += ["note=%s" % n for n in notes]
    return out, 0


if __name__ == "__main__":
    ap = argparse.ArgumentParser()
    ap.add_argument("--build", required=True)
    ap.add_argument("--tree", required=True)
    ap.add_argument("--bin", action="append", default=[])
    ap.add_argument("--note", action="append", default=[])
    a = ap.parse_args()
    text, code = lines(a.build, a.tree, a.bin, a.note)
    sys.stdout.write("".join("# %s\n" % l for l in text))
    sys.exit(code)
