#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT RECORD JSP-1-N24-001: the parity roadmap's JSP-1 clause naming rule N17, taken by rule N24. The rule over the
# profile's documents, the realm probes the parity roadmap's section 4.3 note was written from, and four controls: the
# documents as they were before the change (N24's first run), an injected stale claim, the one span marked as written
# unmarked, and the rule narrowed to the ledger alone.
#
# Run from the repository root at the commit being recorded, from a clean tree:
#
#   python3 docs/evidence/jsp-1-n24-001/collect.py
#
# It writes, into this directory:
#   build.log                    the solution built with warnings as errors
#   n24.log                      rule N24's two tests, each by name
#   probes.log                   survey-4.3.js and unicode.js through the host, and through Node when there is one
#   control-<name>.log           each control: the patch applied, rule N24 run, the patch reverted
#   tests.log                    the solution's test projects, after every revert
# It judges nothing but whether each step answered as it must: the README says what the files show.

import os
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
RECORD = "docs/evidence/jsp-1-n24-001"
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", DOTNET_NOLOGO="1", DOTNET_CLI_TELEMETRY_OPTOUT="1", TZ="UTC")
TESTS = "src/tests/Broiler.VM.Architecture.Tests"
CLI = os.path.join(ROOT, "src", "compositions", "Broiler.VM.Composition.JavaScript.Cli", "bin", "Release", "net10.0",
                   "Broiler.VM.Composition.JavaScript.Cli")
N24 = ["dotnet", "test", TESTS, "-c", "Release", "--filter", "FullyQualifiedName~N24",
       "--logger", "console;verbosity=normal"]


def run(command, cwd=ROOT):
    """Runs a command; answers its exit code and its output with the checkout's paths hidden."""
    completed = subprocess.run(command, cwd=cwd, env=ENV, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    text = completed.stdout.decode("utf-8", "replace").replace("\r\n", "\n")
    return completed.returncode, text.replace(ROOT, "<root>")


def logged(command, code, text):
    shown = " ".join(command).replace(ROOT, "<root>")
    return "$ %s\n%s# exit %d\n" % (shown, text if text.endswith("\n") or not text else text + "\n", code)


def write(name, text):
    with open(os.path.join(HERE, name), "w", encoding="utf-8", newline="\n") as handle:
        handle.write(text if text.endswith("\n") else text + "\n")


def rule_lines(text):
    """What the rule said: each test's verdict, each finding, the assertion that failed and the total."""
    keep = ("Passed ", "Failed ", "src/Broiler.VM.Profile.JavaScript/", "Assert.", "Total tests", "Passed!", "Failed!")
    return "".join(line.strip() + "\n" for line in text.splitlines() if line.strip().startswith(keep))


def main():
    failures = 0

    code, status = run(["git", "status", "--porcelain", "--", ".", ":!" + RECORD])
    if status.strip():
        print("collect: the tree outside this record is not clean; refusing to collect")
        return 2

    _, head = run(["git", "rev-parse", "HEAD"])

    command = ["dotnet", "build", "Broiler.VM.slnx", "-c", "Release", "-warnaserror"]
    code, text = run(command)
    write("build.log", "# collected at %s" % head + logged(command, code, text[-600:]))
    failures += code != 0

    code, text = run(N24)
    write("n24.log", logged(N24, code, rule_lines(text)))
    failures += code != 0

    # THE REALM, ASKED THE WAY SECTION 4.3 ASKED IT, and the comparison engine beside it when one is installed.
    probes = ""
    node = shutil.which("node") or ("/opt/node22/bin/node" if os.path.exists("/opt/node22/bin/node") else None)
    for name in ("survey-4.3.js", "unicode.js"):
        path = os.path.join(HERE, name)
        code, text = run([CLI, path])
        probes += logged(["broiler-js", RECORD + "/" + name], code, text)
        failures += code != 0
        if node:
            with open(path, encoding="utf-8") as handle:
                source = handle.read()
            code, text = run([node, "-p", source])
            _, version = run([node, "--version"])
            probes += logged(["node", version.strip(), "-p", "<" + name + ">"], code, text)
    write("probes.log", probes)

    # EACH CONTROL MUST MAKE N24 FAIL, and the tree must answer as before once it is reverted.
    for name in ("first-run", "injected-claim", "unmarked", "ledger-only"):
        patch = os.path.join(HERE, "control-%s.patch" % name)
        shown = RECORD + "/control-%s.patch" % name
        code, text = run(["git", "apply", patch])
        control = logged(["git", "apply", shown], code, text)
        failures += code != 0

        if code == 0:
            code, text = run(N24)
            control += logged(N24, code, rule_lines(text))
            failures += code == 0
            code, text = run(["git", "apply", "-R", patch])
            control += logged(["git", "apply", "-R", shown], code, text)
            failures += code != 0

        write("control-%s.log" % name, control)

    command = ["dotnet", "build", "Broiler.VM.slnx", "-c", "Release", "-warnaserror"]
    code, _ = run(command)
    failures += code != 0
    command = ["dotnet", "test", "Broiler.VM.slnx", "-c", "Release", "--no-build"]
    code, text = run(command)
    write("tests.log", logged(command, code, text))
    failures += code != 0

    code, status = run(["git", "status", "--porcelain", "--", ".", ":!" + RECORD])
    if status.strip():
        print("collect: the tree is not clean after the collection:\n" + status)
        failures += 1

    print("collect: %s" % ("every step answered as expected" if failures == 0 else "%d steps did NOT" % failures))
    return 0 if failures == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
