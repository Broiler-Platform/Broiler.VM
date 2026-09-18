"""E4 of VM-5-002's re-collection: the inputs of the two profile builds, and their source diff.

Lists the project-reference closure of the JavaScript and WebAssembly profile projects, read from the
project files at the tree's head, and writes git diff --stat from <from> to <to> over those project
directories and the build-wide property files. An empty diff is what section 5.11 reads the
profile-assembly clause of rule item 1 from.

    python profile-closure.py <from> <to>     (from the repository root)
"""
import pathlib
import re
import subprocess
import sys

ROOT = pathlib.Path(__file__).resolve().parents[5]
BS = chr(92)


def refs(commit, project):
    text = subprocess.run(["git", "show", "%s:%s" % (commit, project)], cwd=ROOT, capture_output=True,
                          check=True).stdout.decode("utf-8-sig")
    base = pathlib.PurePosixPath(project).parent
    out = []
    for m in re.finditer(r'<ProjectReference\s+Include="([^"]+)"', text):
        parts = []
        for part in (base / m.group(1).replace(BS, "/")).parts:
            if part == "..":
                parts.pop()
            else:
                parts.append(part)
        out.append("/".join(parts))
    return out


def closure(commit):
    seen, stack = set(), ["src/Broiler.VM.Profile.JavaScript/Broiler.VM.Profile.JavaScript.csproj",
                          "src/Broiler.VM.Profile.WebAssembly/Broiler.VM.Profile.WebAssembly.csproj"]
    while stack:
        p = stack.pop()
        if p in seen:
            continue
        seen.add(p)
        stack += refs(commit, p)
    return sorted(seen)


frm, to = sys.argv[1], sys.argv[2]
projects = sorted(set(closure(frm)) | set(closure(to)))
dirs = sorted({str(pathlib.PurePosixPath(p).parent) for p in projects})
extra = ["Directory.Build.props", "Directory.Build.targets", "Directory.Packages.props", "global.json",
         "NuGet.config", "nuget.config"]
extra += sorted(p.relative_to(ROOT).as_posix() for p in (ROOT / "eng").glob("*.props"))
extra += sorted(p.relative_to(ROOT).as_posix() for p in (ROOT / "eng").glob("*.targets"))
print("# project-reference closure of the two profile projects, at %s and at %s:" % (frm, to))
for p in projects:
    print("#   " + p)
print("# build-wide files included in the diff (a path absent at both commits contributes nothing):")
for e in extra:
    print("#   " + e)
cmd = ["git", "diff", "--stat", frm, to, "--"] + dirs + extra
print("# command=" + " ".join(cmd))
stat = subprocess.run(cmd, cwd=ROOT, capture_output=True, text=True, check=True).stdout
print(stat, end="")
quiet = subprocess.run(["git", "diff", "--quiet", frm, to, "--"] + dirs + extra, cwd=ROOT).returncode
print("# git diff --quiet exit=%d (%s)" % (quiet, "empty" if quiet == 0 else "NOT EMPTY"))
