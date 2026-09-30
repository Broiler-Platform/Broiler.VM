#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT RECORD JSP-10-HOST-001: the reporting half of the parity roadmap's JSP-10. The default allowance stated and
# reached, the constant pool's ceiling named where it is met, a top-level `await` in a template substitution run, and
# the source-encoding set stated with a file outside it refused by name; each through the host's own command line or
# the slice-compiler root's checks, and each with a control that reverts it.
#
# Run from the repository root at the commit being recorded, from a clean tree:
#
#   python3 docs/evidence/jsp-10-host-001/collect.py
#
# It writes, into this directory:
#   build.log            the solution built with warnings as errors
#   acceptance.log       every row of src/tests/cli/expected.txt judged on the host
#   probes.log           each clause through the host's command line, with its exit code
#   checks.log           the slice-compiler root's --checks: the constant-pool line and the closing total
#   control-<name>.log   each control: the patch applied, what it must fail, the patch reverted
#   rows-after-controls.log   the rows this change added, after every revert
#   tests.log            the solution's test projects
# It judges nothing but whether each step answered as it must: the README says what the files show.

import os
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
RECORD = "docs/evidence/jsp-10-host-001"
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", DOTNET_NOLOGO="1", DOTNET_CLI_TELEMETRY_OPTOUT="1")
SCRATCH = os.path.join(ROOT, "artifacts", "jsp-10-host-001")
CLI_PROJECT = "src/compositions/Broiler.VM.Composition.JavaScript.Cli"
CLI = os.path.join(ROOT, CLI_PROJECT, "bin", "Release", "net10.0", "Broiler.VM.Composition.JavaScript.Cli")
SLICE_PROJECT = "src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler"
SLICE = ["dotnet", os.path.join(ROOT, SLICE_PROJECT, "bin", "Release", "net10.0",
                                "Broiler.VM.Composition.JavaScript.SliceCompiler.dll")]
EXPECTED = "src/tests/cli/expected.txt"
MARKER = "# JSP-10, THE HOST SURFACE AN EMBEDDER MEETS FIRST"


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


def pool_lines(text):
    return "".join(line + "\n" for line in text.splitlines()
                   if "constant pool" in line or line.startswith("broiler-js-slice-compiler:"))


def main():
    failures = 0

    code, status = run(["git", "status", "--porcelain", "--", ".", ":!" + RECORD])
    if status.strip():
        print("collect: the tree outside this record is not clean; refusing to collect")
        return 2

    _, head = run(["git", "rev-parse", "HEAD"])
    shutil.rmtree(SCRATCH, ignore_errors=True)
    os.makedirs(SCRATCH)

    command = ["dotnet", "build", "Broiler.VM.slnx", "-c", "Release", "-warnaserror"]
    code, text = run(command)
    write("build.log", "# collected at %s" % head + logged(command, code, text[-600:]))
    failures += code != 0

    command = ["python3", "eng/run-cli-acceptance.py"]
    code, text = run(command)
    write("acceptance.log", logged(command, code, text))
    failures += code != 0

    # THE ROWS THIS CHANGE ADDED, as a table of their own the host controls are judged against.
    with open(os.path.join(ROOT, EXPECTED), encoding="utf-8") as handle:
        table = handle.read()
    rows = os.path.join(SCRATCH, "jsp-10.expected.txt")
    with open(rows, "w", encoding="utf-8", newline="\n") as handle:
        handle.write(table[table.index(MARKER):])
    judge = ["python3", "eng/run-cli-acceptance.py", "--verbose", "--expected", rows]

    # EACH CLAUSE THROUGH THE HOST'S OWN COMMAND LINE.
    probes = [
        ["modules/a-top-level-await-in-a-template.mjs"],
        ["limits/an-ordinary-loop-past-the-default-allowance.js"],
        ["--fuel", "1000000000", "limits/an-ordinary-loop-past-the-default-allowance.js"],
        ["encoding/utf-16le-with-a-byte-order-mark.js"],
        ["encoding/utf-16be-with-a-byte-order-mark.js"],
        ["encoding/not-utf8.js"],
        ["encoding/with-a-byte-order-mark.js"],
    ]
    text = ""
    for words in probes:
        code, output = run([CLI] + words, cwd=os.path.join(ROOT, "src", "tests", "cli"))
        text += logged(["broiler-js"] + words, code, output)
    code, output = run([CLI, "--help"])
    keep = [line for line in output.splitlines() if "UTF-8" in line or "--fuel" in line or "ORDINARY" in line
            or "million numbers" in line or "byte-order mark says" in line or "replacements" in line]
    text += logged(["broiler-js", "--help", "(the <path> and --fuel lines)"], code, "\n".join(keep))
    write("probes.log", text)

    command = SLICE + ["--checks", "--verbose"]
    code, text = run(command)
    write("checks.log", logged(command, code, pool_lines(text)))
    failures += code != 0

    # ONE CONTROL PER CHANGE: applied, judged, reverted.
    controls = [
        ("template-await", CLI_PROJECT, judge),
        ("encoding", CLI_PROJECT, judge),
        ("usage-fuel", CLI_PROJECT, judge),
        ("constant-pool", SLICE_PROJECT, SLICE + ["--checks", "--verbose"]),
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
            control += logged(command, code, pool_lines(text) if project == SLICE_PROJECT else text)
            failures += code == 0
            code, text = run(["git", "apply", "-R", patch])
            control += logged(["git", "apply", "-R", record], code, text)
            failures += code != 0
            code, text = run(build)
            control += logged(build, code, text[-400:])
            failures += code != 0

        write("control-%s.log" % name, control)

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
