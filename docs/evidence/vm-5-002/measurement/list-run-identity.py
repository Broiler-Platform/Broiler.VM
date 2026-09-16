"""Write what the throwaway directory still shows about when E4 to E10 ran and which binaries they ran.

python docs/evidence/vm-5-002/measurement/list-run-identity.py   (from the repository root)

Written after the bundle was committed, in answer to a review: no transcript of E4 to E9 carries a
time, E7's and E8's transcripts name no binary, and the stamps the README cited were read from
binaries this directory did not retain. Everything here is read from artifacts/fc as it stands when
the script runs - file-system times and the files themselves - and nothing is read from a transcript.
Where the throwaway directory no longer holds something, the file written says so.

Writes e5/transcript-times.txt, e7/run-times.txt, e8/run-times.txt, e9/binaries.txt and
e10/lanes.txt, LF.
"""
import ctypes
import datetime
import hashlib
import pathlib
import struct

ROOT = pathlib.Path(__file__).resolve().parents[4]
BUNDLE = ROOT / "docs" / "evidence" / "vm-5-002"
FC = ROOT / "artifacts" / "fc"
READ_AT = datetime.datetime.now().strftime("%Y-%m-%dT%H:%M:%S")


def clock(seconds):
    return datetime.datetime.fromtimestamp(seconds).strftime("%H:%M:%S")


def product_version(path):
    """The version resource's ProductVersion, which the SDK writes from the informational version."""
    version = ctypes.windll.version
    size = version.GetFileVersionInfoSizeW(str(path), None)
    if not size:
        return "(no version resource)"
    buffer = ctypes.create_string_buffer(size)
    if not version.GetFileVersionInfoW(str(path), 0, size, buffer):
        return "(version resource unreadable)"
    pointer, length = ctypes.c_void_p(), ctypes.c_uint()
    if not version.VerQueryValueW(buffer, "\\VarFileInfo\\Translation", ctypes.byref(pointer), ctypes.byref(length)):
        return "(no translation)"
    language, codepage = struct.unpack("<HH", ctypes.string_at(pointer.value, 4))
    key = "\\StringFileInfo\\%04x%04x\\ProductVersion" % (language, codepage)
    if not version.VerQueryValueW(buffer, key, ctypes.byref(pointer), ctypes.byref(length)) or length.value == 0:
        return "(no product version)"
    return ctypes.wstring_at(pointer.value, length.value).rstrip("\0")


def sha256(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def write(relative, lines):
    (BUNDLE / relative).write_bytes(("\n".join(lines) + "\n").encode("utf-8"))
    print("wrote", relative)


def run_rows(directory):
    logs = sorted((directory / "logs").glob("*.log"), key=lambda p: p.stat().st_mtime)
    merge = directory / "merge.log"
    return "%-44s  created %s  shard logs %2d, first written %s, last written %s  merge.log written %s" % (
        directory.name,
        clock(directory.stat().st_ctime),
        len(logs),
        clock(logs[0].stat().st_mtime) if logs else "-",
        clock(logs[-1].stat().st_mtime) if logs else "-",
        clock(merge.stat().st_mtime) if merge.exists() else "-",
    )


RUN_HEADER = [
    "Read %s from the throwaway directory artifacts/fc, after this bundle was committed. These are" % READ_AT,
    "file-system times, not transcript times: no transcript of these runs carries a time.",
    "",
    "How eng/run-test262.py writes them: it verifies the checkout, then creates the run's output",
    "directory (\"created\"); each shard's transcript is written once, when that shard's process exits,",
    "so \"first written\" is when the first shard FINISHED, not when the run began; merge.log is written",
    "by the merge that ends the run. No retained driver transcript of these runs reports removing files",
    "an earlier run left in its directory, so each creation time is when that run's driver reached that",
    "point, and no shard process of the run started before it.",
    "",
]

# E7
e7 = RUN_HEADER + [
    "E7, the four whole-suite runs, in the order they ran.",
    "",
]
e7 += [run_rows(FC / name) for name in (
    "t262-base-bytecode", "t262-base-native", "t262-credit-bytecode", "t262-credit-native")]
e7 += [
    "",
    "The binary directory each E7 run was given is recorded in no file that survives: the driver prints",
    "none, and the command lines were not kept. The base build's directory was in the worktree",
    "D:/Broiler.VM-base, since removed, and the credit build's in the main checkout, rebuilt after the",
    "fallback, so no digest or commit stamp of either binary can be taken now.",
]
write("e7/run-times.txt", e7)

# E8
e8 = RUN_HEADER + [
    "E8, the low-fuel series, in the order measurement/e8-run.sh ran it; that script names the binary",
    "directory of each build. The six off-series runs (README section 5.8, item 7) follow.",
    "",
]
series = sorted(
    (d for d in FC.iterdir() if d.is_dir() and d.name.startswith("low-")),
    key=lambda d: d.stat().st_ctime)
e8 += [run_rows(d) for d in series]
e8 += [""]
e8 += [run_rows(d) for d in sorted((FC / "off-series").iterdir(), key=lambda d: d.stat().st_ctime) if d.is_dir()]
e8 += [
    "",
    "No digest or commit stamp of the binaries E8 ran can be taken now: the base build's directory was",
    "in the removed worktree D:/Broiler.VM-base, and the credit build's was rebuilt after the fallback.",
]
write("e8/run-times.txt", e8)


def binaries(directory):
    rows = []
    for path in sorted(directory.iterdir()):
        if path.is_file() and path.suffix in (".dll", ".exe"):
            rows.append("%s  %-58s  %s" % (sha256(path), path.name, product_version(path)))
    return rows


def nested(directory):
    rows = []
    for sub in sorted(p for p in directory.iterdir() if p.is_dir()):
        runtime = sub / "Broiler.VM.Runtime.dll"
        if runtime.exists():
            rows.append("  %s/Broiler.VM.Runtime.dll  %s  %s" % (sub.name, sha256(runtime), product_version(runtime)))
    return rows


# E9
e9 = [
    "Read %s from artifacts/fc/e9/bin, after this bundle was committed." % READ_AT,
    "",
    "measurement/p_bisect.py runs <its own directory>/bin/<variant>/Broiler.VM.Composition.JavaScript.Cli.exe,",
    "so these two directories - the builds of both trees with measurement/patch_measure2.diff applied,",
    "copied out - are the binaries E9 ran. Each row: SHA-256, file, and the product version the SDK",
    "writes from the informational version, whose suffix after '+' is the commit the build was stamped",
    "with.",
    "",
]
for variant in ("base", "credit"):
    directory = FC / "e9" / "bin" / variant
    e9 += ["%s: bin/%s, created %s" % (variant, variant, clock(directory.stat().st_ctime))]
    e9 += binaries(directory)
    extra = nested(directory)
    if extra:
        e9 += ["  A subdirectory copied along with the output directory, from an earlier RID-specific build;",
               "  it is a separate output and not the host above:"] + extra
    e9 += [""]
write("e9/binaries.txt", e9)

# E5 (E4, E5, E6 and E9 transcripts)
names = sorted(
    p for p in FC.iterdir()
    if p.is_file() and p.name.split("-")[0] in ("e4", "e5", "e6", "e9"))
times = sorted(p.stat().st_mtime for p in names)
e5 = [
    "Read %s from artifacts/fc, after this bundle was committed." % READ_AT,
    "",
    "No transcript of E4, E5, E6 or E9 carries a time. The copies in the throwaway directory from which",
    "the bundle's transcripts of those runs were taken are %d files, all" % len(names),
    "written between %s and %s, together, so they bound when those runs had finished and not" % (clock(times[0]), clock(times[-1])),
    "when each ran. e9/binaries.txt gives the creation time of E9's copied-out binaries.",
    "",
]
e5 += ["%s  %s" % (clock(p.stat().st_mtime), p.name) for p in names]
write("e5/transcript-times.txt", e5)

# E10
e10 = [
    "Read %s from artifacts/fc/e10/lanes, after this bundle was committed." % READ_AT,
    "",
    "measurement/measure-shapes.py runs each lane's Broiler.VM.Composition.JavaScript.Cli.exe. The lanes",
    "left here are those of E10's second run, after the fallback; the first run's credit and A/A lanes",
    "were overwritten by them, and only the digest of their Broiler.VM.Runtime.dll survives, in",
    "collection-notes.txt, with no stamp. The base lane was created before the first run began and is",
    "the one both runs used. Each row: SHA-256, file, product version.",
    "",
]
for lane in ("base", "credit", "aa"):
    directory = FC / "e10" / "lanes" / lane
    e10 += ["%s: lanes/%s, created %s" % (lane, lane, clock(directory.stat().st_ctime))]
    e10 += binaries(directory)
    extra = nested(directory)
    if extra:
        e10 += ["  A subdirectory copied along with the output directory, from an earlier RID-specific build;",
                "  it is a separate output and not the host above:"] + extra
    e10 += [""]
write("e10/lanes.txt", e10)
