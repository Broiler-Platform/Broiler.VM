#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT RECORD JSP-PARITY-001: the parity roadmap's JSP-8 (the places this component disagreed with itself) and
# JSP-10's severable Math.random clause. The acceptance suite over the host, the probes of each clause through the
# host's own command line, the slice-compiler root's Math.random checks, the pinned test262 subtrees the changes
# reach on the base and on the change, and one control per change: its patch applied, the rows or checks it must
# fail, the patch reverted.
#
# Run from the repository root at the commit being recorded, from a clean tree, naming the commit before the
# change:
#
#   python3 docs/evidence/jsp-parity-001/collect.py --base <commit>
#
# It writes, into this directory:
#   build.log            the solution built with warnings as errors
#   acceptance.log       every row of src/tests/cli/expected.txt judged on the host
#   probes.log           each clause through the host's command line, with its exit code
#   checks.log           the slice-compiler root's --checks: the Math.random lines and the closing total
#   test262-base.log     the pinned subtrees on the base commit's conformance root
#   test262-change.log   the same subtrees on this commit's
#   test262-moves.log    every variant whose verdict differs between the two
#   control-<name>.log   each control: the patch applied, what it must fail, the patch reverted
#   tests.log            the solution's test projects
# It judges nothing but whether each step answered as it must: the README says what the files show.

import argparse
import os
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
RECORD = "docs/evidence/jsp-parity-001"
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", DOTNET_NOLOGO="1", DOTNET_CLI_TELEMETRY_OPTOUT="1")
SCRATCH = os.path.join(ROOT, "artifacts", "jsp-parity-001")
CLI_PROJECT = "src/compositions/Broiler.VM.Composition.JavaScript.Cli"
CLI = os.path.join(ROOT, CLI_PROJECT, "bin", "Release", "net10.0", "Broiler.VM.Composition.JavaScript.Cli")
SLICE_PROJECT = "src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler"
SLICE = ["dotnet", os.path.join(ROOT, SLICE_PROJECT, "bin", "Release", "net10.0",
                                "Broiler.VM.Composition.JavaScript.SliceCompiler.dll")]
CONFORMANCE_PROJECT = "src/compositions/Broiler.VM.Composition.JavaScript.Conformance"
ARCHIVE = "src/tests/conformance/pins/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e.tar.gz"
SUBTREES = [
    "test/language/module-code/top-level-await",
    "test/built-ins/RegExp/match-indices",
    "test/built-ins/Math/random",
]
EXPECTED = "src/tests/cli/expected.txt"
MARKER = "# JSP-8, THE PLACES THIS COMPONENT DISAGREED WITH ITSELF"


def run(command, cwd=ROOT):
    """Runs a command; answers its exit code and its output with the checkout's paths hidden."""
    completed = subprocess.run(command, cwd=cwd, env=ENV, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    text = completed.stdout.decode("utf-8", "replace").replace("\r\n", "\n")
    return completed.returncode, text.replace(SCRATCH, "<scratch>").replace(ROOT, "<root>")


def shown(command):
    return " ".join(command).replace(SCRATCH, "<scratch>").replace(ROOT, "<root>")


def logged(command, code, text):
    return "$ %s\n%s# exit %d\n" % (shown(command), text if text.endswith("\n") or not text else text + "\n", code)


def write(name, text):
    with open(os.path.join(HERE, name), "w", encoding="utf-8", newline="\n") as handle:
        handle.write(text if text.endswith("\n") else text + "\n")


def math_lines(text):
    return "".join(line + "\n" for line in text.splitlines()
                   if "Math.random" in line or line.startswith("broiler-js-slice-compiler:"))


def verdicts(report):
    """Every variant's verdict in a merged test262 report, by its path and variant."""
    found = {}
    with open(report, encoding="utf-8") as handle:
        for line in handle:
            fields = line.rstrip("\n").split("|")
            if fields[0] == "result" and len(fields) >= 4:
                found[fields[1] + " [" + fields[2] + "]"] = fields[3]
    return found


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--base", required=True, help="the commit before the change")
    arguments = parser.parse_args()
    failures = 0

    code, status = run(["git", "status", "--porcelain", "--", ".", ":!" + RECORD])
    if status.strip():
        print("collect: the tree outside this record is not clean; refusing to collect")
        return 2

    _, head = run(["git", "rev-parse", "HEAD"])
    _, base = run(["git", "rev-parse", arguments.base])
    shutil.rmtree(SCRATCH, ignore_errors=True)
    os.makedirs(SCRATCH)

    command = ["dotnet", "build", "Broiler.VM.slnx", "-c", "Release", "-warnaserror"]
    code, text = run(command)
    write("build.log", "# collected at %s# base %s" % (head, base) + logged(command, code, text[-600:]))
    failures += code != 0

    # THE ACCEPTANCE SUITE, every row.
    command = ["python3", "eng/run-cli-acceptance.py"]
    code, text = run(command)
    write("acceptance.log", logged(command, code, text))
    failures += code != 0

    # THE ROWS THIS CHANGE ADDED, as a table of their own the controls are judged against.
    with open(os.path.join(ROOT, EXPECTED), encoding="utf-8") as handle:
        table = handle.read()
    rows = os.path.join(SCRATCH, "jsp-8.expected.txt")
    with open(rows, "w", encoding="utf-8", newline="\n") as handle:
        handle.write(table[table.index(MARKER):])
    judge = ["python3", "eng/run-cli-acceptance.py", "--verbose", "--expected", rows]

    # EACH CLAUSE THROUGH THE HOST'S OWN COMMAND LINE.
    probes = [
        ["modules/a-top-level-for-await.mjs"],
        ["runs/a-match-with-indices.js"],
        ["refused/a-rest-binding-element-with-a-comma-after-it.js"],
        ["refused/a-rest-binding-property-with-a-comma-after-it.js"],
        ["refused/a-rest-parameter-with-a-comma-after-it.js"],
        ["refused/an-arrow-rest-parameter-with-a-trailing-comma.js"],
        ["refused/a-rest-element-with-a-comma-after-it.js"],
        ["--slice", "runs/a-class-static-block.js"],
        ["--version"],
    ]
    text = ""
    for words in probes:
        code, output = run([CLI] + words, cwd=os.path.join(ROOT, "src", "tests", "cli"))
        text += logged(["broiler-js"] + words, code, output)
    code, output = run([CLI, "--help"])
    text += logged(["broiler-js", "--help", "(the lines naming a manifest)"], code,
                   "".join(line + "\n" for line in output.splitlines() if "broiler.javascript." in line))
    write("probes.log", text)

    # THE MATH.RANDOM CHECKS, with the root's closing total.
    command = SLICE + ["--checks", "--verbose"]
    code, text = run(command)
    write("checks.log", logged(command, code, math_lines(text)))
    failures += code != 0

    # THE PINNED SUBTREES, on the base commit's conformance root and on this one's.
    suite_root = os.path.join(SCRATCH, "suite")
    os.makedirs(suite_root)
    code, _ = run(["tar", "xzf", ARCHIVE, "-C", suite_root])
    failures += code != 0
    suite = os.path.join(suite_root, os.listdir(suite_root)[0])
    worktree = os.path.join(SCRATCH, "base")
    code, _ = run(["git", "worktree", "add", "--detach", worktree, base.strip()])
    failures += code != 0
    code, text = run(["dotnet", "build", CONFORMANCE_PROJECT, "-c", "Release"], cwd=worktree)
    failures += code != 0
    reports = {}

    for name, directory in (
            ("base", os.path.join(worktree, CONFORMANCE_PROJECT, "bin", "Release", "net10.0")),
            ("change", os.path.join(ROOT, CONFORMANCE_PROJECT, "bin", "Release", "net10.0"))):
        out = os.path.join(SCRATCH, "test262-" + name)
        command = ["python3", "eng/run-test262.py", "--suite", suite, "--binary-directory", directory,
                   "--shards", "1", "--jobs", "1", "--out", out]
        for subtree in SUBTREES:
            command += ["--dir", subtree]
        code, text = run(command)
        write("test262-%s.log" % name, logged(command, code, text[-3000:]))
        reports[name] = os.path.join(out, "test262.report")

    before, after = verdicts(reports["base"]), verdicts(reports["change"])
    moves = sorted((variant, before.get(variant), after.get(variant))
                   for variant in set(before) | set(after) if before.get(variant) != after.get(variant))
    write("test262-moves.log",
          "# %d variants on the base, %d on the change; %d change verdict\n" % (len(before), len(after), len(moves))
          + "".join("%s %s -> %s\n" % move for move in moves))
    run(["git", "worktree", "remove", "--force", worktree])

    # ONE CONTROL PER CHANGE: applied, judged, reverted.
    controls = [
        ("for-await", CLI_PROJECT, judge),
        ("indices", CLI_PROJECT, judge),
        ("rest", CLI_PROJECT, judge),
        ("static-block", CLI_PROJECT, judge),
        ("usage", CLI_PROJECT, judge),
        ("constant-seed", SLICE_PROJECT, SLICE + ["--checks", "--verbose"]),
        ("counter-seed", SLICE_PROJECT, SLICE + ["--checks", "--verbose"]),
    ]

    for name, project, command in controls:
        patch = os.path.join(HERE, "control-%s.patch" % name)
        record = RECORD + "/control-%s.patch" % name
        code, text = run(["git", "apply", patch])
        control = logged(["git", "apply", record], code, text)
        failures += code != 0

        if code == 0:
            build = ["dotnet", "build", project, "-c", "Release"]
            code, text = run(build)
            control += logged(build, code, text[-400:])
            code, text = run(command)
            control += logged(command, code, math_lines(text) if project == SLICE_PROJECT else text)
            failures += code == 0
            code, text = run(["git", "apply", "-R", patch])
            control += logged(["git", "apply", "-R", record], code, text)
            failures += code != 0
            code, text = run(build)
            control += logged(build, code, text[-400:])
            failures += code != 0

        write("control-%s.log" % name, control)

    # AFTER EVERY REVERT, the added rows answer as declared again.
    code, text = run(judge)
    write("rows-after-controls.log", logged(judge, code, text))
    failures += code != 0

    command = ["dotnet", "test", "Broiler.VM.slnx", "-c", "Release", "--no-build"]
    code, text = run(command)
    write("tests.log", logged(command, code, text))
    failures += code != 0

    code, status = run(["git", "status", "--porcelain", "--", ".", ":!" + RECORD, ":!artifacts"])
    if status.strip():
        print("collect: the tree is not clean after the collection:\n" + status)
        failures += 1

    print("collect: %s" % ("every step answered as expected" if failures == 0 else "%d steps did NOT" % failures))
    return 0 if failures == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
