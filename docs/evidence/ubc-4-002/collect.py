#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT BUNDLE UBC-4-002: the run after, the comparison with the base, and the four roots published.
#
# Run from the repository root at the commit the bundle names, after `dotnet build Broiler.VM.slnx -c Release`:
#
#   python docs/evidence/ubc-4-002/collect.py [--rid win-x64] [--vcvars PATH-TO-vcvars64.bat]
#
# It writes, into this directory:
#   after-run.log, after-population.txt   population B run once after the change, as ubc-4-001's base run was,
#                                         and reduced to rows the same way
#   comparison.log                        the base run and the run after joined member by member within each
#                                         lane, every difference classified under the predeclared rule of
#                                         docs/evidence/ubc-4-001/decision-rule.md, and the rule's verdict
#   determinism.log                       the harness's --determinism lane: every module translated twice
#   corpus-rebase.log                     the retained malformed corpus rewritten from the harness into a scratch
#                                         directory and compared byte for byte with src/tests/wasm/corpus, and
#                                         the replay's own line on which stage answered each entry
#   publish.log                           every trimmed and Native AOT publish
#   run-<root>-<mode>.log                 each root's checks in each of the three modes
#   catalog-<root>.txt, closure-<root>.txt, <root>-modes.log
#                                         the catalog each root prints, the managed non-framework assemblies each
#                                         publish contains, and whether the three modes printed the same tables
#   architecture-tests.log                the rules of the register and the graph in detail, run after the
#                                         catalogs and closures are written, then the whole architecture suite
# The negative control's two halves, control-failing.log and control-passing.log, were each retained at the
# commit they name by collect-control.py and are not rewritten here. It judges nothing beyond the rule's own
# classification: the README says what the files show.

import argparse
import io
import os
import re
import shutil
import subprocess
import sys
import tempfile

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
BASE = os.path.join(ROOT, "docs", "evidence", "ubc-4-001")
WINDOWS = os.name == "nt"
DEFAULT_VCVARS = r"C:\Program Files\Microsoft Visual Studio\18\Professional\VC\Auxiliary\Build\vcvars64.bat"
BASE_ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", VSLANG="1033", DOTNET_NOLOGO="1")
INPUTS = "docs/evidence/vm-7-cli-001/inputs"

# Each root: its register slug, its project, its assembly, the command that prints its catalog, and the runs
# retained in each mode. Paths inside the runs are relative to the repository root, which every run uses.
ROOTS = [
    ("fixture", "src/compositions/Broiler.VM.Composition.Ubc.Fixture", "Broiler.VM.Composition.Ubc.Fixture",
     ["--closure"],
     [["--verbose"], ["--corpus", "src/tests/corpus/ubc-2"], ["--ubc1-corpus", "src/tests/corpus/ubc-1"]]),
    ("execution", "src/compositions/Broiler.VM.Composition.WebAssembly.Execution",
     "Broiler.VM.Composition.WebAssembly.Execution", ["--closure"], [["--verbose"]]),
    ("harness", "src/compositions/Broiler.VM.Composition.WebAssembly.Harness",
     "Broiler.VM.Composition.WebAssembly.Harness", ["--closure"],
     [["--verbose", "--corpus", "src/tests/wasm/corpus"], ["--determinism"],
      ["--primitives", "src/tests/corpus/ubc-2/primitives.txt"]]),
    ("polyglotcli", "src/compositions/Broiler.VM.Composition.PolyglotCli", "Broiler.VM.Composition.PolyglotCli",
     ["closure"],
     [["version"], ["run", INPUTS + "/adder.wasm"],
      ["run", INPUTS + "/adder.wasm", "--invoke", "add", "--arg", "i32:7", "--arg", "i32:35"],
      ["run", INPUTS + "/hello.js", INPUTS + "/adder.wasm"], ["run", INPUTS + "/nottext.wasm"],
      ["run", INPUTS + "/hello.js"]]),
]

# A member line as the harness prints it, and the lanes of population B, as ubc-4-001's collector reads them;
# here every other '#' header ends a lane, so a lane added after the base cannot leak into one of the three.
MEMBER = re.compile(r"^(ok  |FAIL) (.*)$")
LANES = {"# retained corpus": "corpus", "# execution": "execution", "# differential": "differential"}


def run(command, env=None, cwd=ROOT):
    result = subprocess.run(command, cwd=cwd, capture_output=True, text=True, env=env or BASE_ENV,
                            encoding="utf-8", errors="replace")
    return result.returncode, (result.stdout + result.stderr).replace("\r\n", "\n")


def write(name, text):
    with io.open(os.path.join(HERE, name), "w", encoding="utf-8", newline="\n") as handle:
        handle.write(text if text.endswith("\n") else text + "\n")


def vcvars_batch(vcvars, commands):
    script = '@echo off\r\ncall "%s" >nul\r\nset Platform=\r\n%s\r\n' % (vcvars, "\r\n".join(commands))
    handle, batch = tempfile.mkstemp(suffix=".bat")
    os.close(handle)
    io.open(batch, "w", encoding="ascii", newline="").write(script)
    try:
        return run(["cmd", "/c", batch])
    finally:
        os.remove(batch)


def is_managed(path):
    """True when the file is a PE image carrying a CLR header - an assembly, not a native library the
    self-contained runtime ships beside it under the same extension."""
    try:
        with open(path, "rb") as handle:
            data = handle.read(4096)
        pe = int.from_bytes(data[0x3C:0x40], "little")
        if data[pe:pe + 4] != b"PE\0\0":
            return False
        optional = pe + 24
        magic = int.from_bytes(data[optional:optional + 2], "little")
        directories = optional + (96 if magic == 0x10B else 112)
        clr = directories + 14 * 8
        return int.from_bytes(data[clr:clr + 4], "little") != 0
    except (OSError, ValueError):
        return False


def members(log):
    """Every member line of population B, in the order printed, keyed by lane: the lane is set by one of the
    three headers and ended by any other '#' header."""
    lanes = {"corpus": [], "execution": [], "differential": []}
    lane = None
    for line in log.splitlines():
        if line.startswith("#"):
            lane = next((name for prefix, name in LANES.items() if line.startswith(prefix)), None)
            continue
        if lane and MEMBER.match(line):
            lanes[lane].append(line)
    return lanes


def compare(base_log, after_log):
    """The rule's comparison: member by member within each lane, in the order printed, each difference
    classified as (f), (r) or a regression, and the verdict the rule gives."""
    base, after = members(base_log), members(after_log)
    lines = ["# population B, the base run (ubc-4-001/base-run.log) against the run after (after-run.log)", ""]
    classes = {"f": 0, "r": 0, "regression": 0, "same": 0}
    for lane in ("corpus", "execution", "differential"):
        b, a = base[lane], after[lane]
        lines.append("[%s] %d members at the base, %d after" % (lane, len(b), len(a)))
        if len(b) != len(a):
            lines.append("    REGRESSION: the lane's membership changed")
            classes["regression"] += 1
        for index, (x, y) in enumerate(zip(b, a)):
            if x == y:
                classes["same"] += 1
                continue
            if (lane == "execution" and x.startswith("FAIL float-comparison:") and y.startswith("ok   float-comparison:")
                    and "ProfileFault/ProfileContractViolation, no results payload" in x
                    and x.split(": ")[1] == y.split(": ")[1]):
                classes["f"] += 1
                lines.append("    (f) #%d %s" % (index + 1, y[5:]))
                lines.append("        base: %s" % x[5:])
                continue
            classes["regression"] += 1
            lines.append("    REGRESSION #%d" % (index + 1))
            lines.append("        base:  %s" % x)
            lines.append("        after: %s" % y)
    lines.append("")
    lines.append("# unchanged %d, class (f) %d, class (r) %d, outside both %d"
                 % (classes["same"], classes["f"], classes["r"], classes["regression"]))
    lines.append("# population A: not judged - the specification's suite is not pinned and has no reader (the"
                 " owner's decision of 2026-09-25); under the rule its precondition is unmet")
    # The rule is MET only if population A was judged, so with A's precondition unmet it is NOT MET whatever
    # population B shows; B's comparison is retained as the partial evidence the rule names.
    lines.append("# the rule's decision: NOT MET - population A cannot be judged; population B %s" % (
        "differs only in class (f), retained as partial evidence named as such"
        if classes["regression"] == 0 else "HAS A REGRESSION"))
    return "\n".join(lines), classes["regression"]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--rid", default="win-x64" if WINDOWS else "linux-x64")
    parser.add_argument("--vcvars", default=DEFAULT_VCVARS)
    arguments = parser.parse_args()
    exe = ".exe" if WINDOWS else ""

    # Population B after, the comparison, determinism and the corpus re-base, all on the JIT build.
    harness = ["dotnet", os.path.join("src/compositions/Broiler.VM.Composition.WebAssembly.Harness/bin/Release/net10.0",
                                      "Broiler.VM.Composition.WebAssembly.Harness.dll")]
    command = harness + ["--verbose", "--corpus", "src/tests/wasm/corpus"]
    code, after = run(command)
    write("after-run.log", "$ %s\n%s\nexit %d" % (" ".join(command), after, code))
    rows = []
    for lane, lines in members(after).items():
        for line in lines:
            verdict, rest = line[:4].strip(), line[5:]
            name, _, answer = rest.partition(": ")
            rows.append("%s|%s|%s|%s" % (lane, name, verdict, answer))
    write("after-population.txt",
          "# population B after: lane|name|verdict|answer, one row per member, sorted\n" + "\n".join(sorted(rows)))
    base = io.open(os.path.join(BASE, "base-run.log"), encoding="utf-8").read()
    table, regressions = compare(base, after)
    write("comparison.log", table)

    code, out = run(harness + ["--determinism"])
    write("determinism.log", "$ %s --determinism\n%s\nexit %d" % (" ".join(harness), out, code))

    scratch = tempfile.mkdtemp(prefix="ubc-4-002-corpus-")
    try:
        code, out = run(harness + ["--write-corpus", scratch])
        retained = os.path.join(ROOT, "src", "tests", "wasm", "corpus")
        names = sorted(set(os.listdir(retained)) | set(os.listdir(scratch)))
        differing = [n for n in names if not (os.path.exists(os.path.join(retained, n)) and os.path.exists(os.path.join(scratch, n))
                     and open(os.path.join(retained, n), "rb").read() == open(os.path.join(scratch, n), "rb").read())]
        stage = [l for l in after.splitlines() if l.startswith("# retained corpus")]
        write("corpus-rebase.log",
              "$ %s --write-corpus <scratch>\n%s\nexit %d\n\n# %d files compared with src/tests/wasm/corpus, %d differ%s\n\n%s"
              % (" ".join(harness), out, code, len(names), len(differing),
                 "".join("\n    " + n for n in differing), "\n".join(stage)))
    finally:
        shutil.rmtree(scratch, ignore_errors=True)

    # Every root published trimmed and Native AOT, and run in the three modes.
    publish_log = []
    launchers = {}
    for slug, project, name, _, _ in ROOTS:
        trimmed = os.path.join("artifacts", "publish-ubc4-%s-trimmed" % slug)
        aot = os.path.join("artifacts", "publish-ubc4-%s-aot" % slug)
        for directory in (trimmed, aot):
            shutil.rmtree(os.path.join(ROOT, directory), ignore_errors=True)
        code, out = run(["dotnet", "publish", project, "-c", "Release", "-r", arguments.rid, "--self-contained", "true",
                         "-p:PublishAot=false", "-p:PublishTrimmed=true", "-o", trimmed])
        publish_log.append("--- %s TRIMMED ---\n%s\nexit %d\n" % (slug, out, code))
        command = "dotnet publish %s -c Release -r %s -p:PublishAot=true -p:IlcUseEnvironmentalTools=true -o %s" % (
            project, arguments.rid, aot)
        code, out = vcvars_batch(arguments.vcvars, [command]) if WINDOWS else run(command.split())
        publish_log.append("--- %s NATIVE AOT: %s ---\n%s\nexit %d\n" % (slug, command, out, code))
        launchers[slug] = {
            "jit": ["dotnet", os.path.join(project, "bin", "Release", "net10.0", name + ".dll")],
            "trimmed": [os.path.join(ROOT, trimmed, name + exe)],
            "aot": [os.path.join(ROOT, aot, name + exe)],
        }
    write("publish.log", "\n".join(publish_log))

    for slug, project, name, closure_command, runs in ROOTS:
        catalogs, outputs = {}, {}
        for mode, launcher in launchers[slug].items():
            text = []
            for arguments_ in runs:
                code, out = run(launcher + arguments_)
                text.append("$ %s %s\n%s\nexit %d\n" % (" ".join(launcher), " ".join(arguments_), out, code))
            outputs[mode] = [t.split("\n", 1)[1] for t in text]
            write("run-%s-%s.log" % (slug, mode), "\n".join(text))
            code, catalogs[mode] = run(launcher + closure_command)
        write("catalog-%s.txt" % slug, catalogs["jit"])
        same_catalog = catalogs["jit"] == catalogs["trimmed"] == catalogs["aot"]
        same_output = outputs["jit"] == outputs["trimmed"] == outputs["aot"]
        write("%s-modes.log" % slug, "the three modes' catalogs are %s\nthe three modes' run outputs are %s\n" % (
            "BYTE-IDENTICAL" if same_catalog else "NOT IDENTICAL",
            "BYTE-IDENTICAL" if same_output else "NOT IDENTICAL - see the run logs"))
        closure = ["# closure %s rid=%s" % (name, arguments.rid), ""]
        for mode in ("trimmed", "aot"):
            full = os.path.join(ROOT, "artifacts", "publish-ubc4-%s-%s" % (slug, mode))
            names = sorted(n[:-4] for n in os.listdir(full) if n.endswith(".dll") and is_managed(os.path.join(full, n))
                           and not n.startswith(("System.", "Microsoft."))
                           and n not in ("netstandard.dll", "mscorlib.dll", "WindowsBase.dll"))
            closure.append("[%s] %d non-framework assemblies" % (mode, len(names)))
            closure.extend(names)
            closure.append("")
        write("closure-%s.txt" % slug, "\n".join(closure))

    def architecture(filter_expression=None):
        command = ["dotnet", "test", "src/tests/Broiler.VM.Architecture.Tests", "-c", "Release", "--no-build",
                   "--logger", "console;verbosity=detailed"]
        if filter_expression:
            command += ["--filter", filter_expression]
        code, out = run(command)
        return "$ %s\n%s\nexit %d\n" % (" ".join(command), out, code)

    rules = architecture("|".join("FullyQualifiedName~%s" % n for n in (
        "CompositionRegisterTests", "TopologyBudgetTests", "ProjectFileRuleTests.A7_", "ProjectFileRuleTests.A11_",
        "ProjectFileRuleTests.A12_", "ProjectFileRuleTests.A13_", "WebAssemblyFamilyRuleTests", "UbcRuleTests")))
    write("architecture-tests.log", rules + "\n--- THE WHOLE ARCHITECTURE SUITE ---\n\n" + architecture())
    print("collected; %d regressions in population B; now write README.md, then run eng/ubc-bundle-manifest.py"
          % regressions)
    return 0


if __name__ == "__main__":
    sys.exit(main())
