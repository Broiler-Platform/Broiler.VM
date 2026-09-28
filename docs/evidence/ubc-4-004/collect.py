#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT BUNDLE UBC-4-004: UBC-4's clause 6 on its revised gate - the live bytes an instance retains at
# instantiation, at a growth, at each kind of refused growth and at disposal, and the fuel a refused growth
# spends, asserted by the harness's execution checks over the store.
#
# Run from the repository root at the commit the bundle names, from a clean tree:
#
#   python3 docs/evidence/ubc-4-004/collect.py [--rid linux-x64]
#
# It writes, into this directory:
#   build.log                    the solution built with warnings as errors
#   run-harness-jit.log          the harness root's catalog, then a whole run - its execution checks, the
#                                retention checks among them, its differential checks and the retained
#                                corpus replayed - verbose, from the framework-dependent build
#   run-harness-trimmed.log      the same, from a trimmed self-contained publish
#   run-harness-aot.log          the same, from the Native AOT image
#   harness-modes.log            whether the three modes printed the same lines, line for line
#   publish.log                  the two publishes
#   control-retired-order.log    the harness with control-retired-order.patch applied to the store - the
#                                retired executor's growth: a refused charge answers the guest minus one and
#                                lets it go on, and the retention is reported after the allocation - then
#                                reverted
#   control-amounts.log          the harness with control-amounts.patch applied - a table entry retaining
#                                eight bytes rather than four - then reverted
#   tests.log                    the solution's test projects, the contract and architecture suites
# It judges nothing: the README says what the files show.

import argparse
import os
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
BUNDLE = "docs/evidence/ubc-4-004"
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", DOTNET_NOLOGO="1", DOTNET_CLI_TELEMETRY_OPTOUT="1")
HARNESS = "src/compositions/Broiler.VM.Composition.WebAssembly.Harness"
ASSEMBLY = "Broiler.VM.Composition.WebAssembly.Harness"
WHOLE_RUN = ["--corpus", "src/tests/wasm/corpus", "--verbose"]
CONTROLS = ["control-retired-order", "control-amounts"]


def run(command, cwd=ROOT):
    """Runs a command from the repository root; answers its exit code and its output with the root's path hidden."""
    completed = subprocess.run(command, cwd=cwd, env=ENV, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    text = completed.stdout.decode("utf-8", "replace").replace("\r\n", "\n").replace(ROOT, "<root>")
    return completed.returncode, text


def write(name, text):
    with open(os.path.join(HERE, name), "w", encoding="utf-8", newline="\n") as handle:
        handle.write(text if text.endswith("\n") else text + "\n")


def logged(command, code, text):
    return "$ %s\n%s# exit %d\n" % (" ".join(command), text, code)


def lines_of(text):
    """The lines a mode's run prints, without the command and exit lines this script adds."""
    return [line for line in text.splitlines() if not line.startswith("$ ") and not line.startswith("# exit")]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--rid", default="linux-x64")
    arguments = parser.parse_args()
    failures = 0

    code, status = run(["git", "status", "--porcelain", "--", ".", ":!" + BUNDLE])
    if status.strip():
        print("collect: the tree outside this bundle is not clean; refusing to collect")
        return 2

    _, head = run(["git", "rev-parse", "HEAD"])

    command = ["dotnet", "build", "Broiler.VM.slnx", "-c", "Release", "-warnaserror"]
    code, text = run(command)
    write("build.log", "# collected at %s" % head + logged(command, code, text))
    failures += code != 0

    runs = {}
    jit = ["dotnet", "%s/bin/Release/net10.0/%s.dll" % (HARNESS, ASSEMBLY)]
    publish_log = []
    images = {"jit": jit}

    for mode, extra in (("trimmed", ["--self-contained", "true", "-p:PublishTrimmed=true"]),
                        ("aot", ["-p:PublishAot=true"])):
        output = os.path.join(ROOT, "artifacts", "publish-ubc4-004-harness-" + mode)
        shutil.rmtree(output, ignore_errors=True)
        command = ["dotnet", "publish", HARNESS, "-c", "Release", "-r", arguments.rid, "-o", output] + extra
        code, text = run(command)
        publish_log.append("--- harness %s ---\n%s" % (mode.upper(), logged(command, code, text)))
        failures += code != 0
        images[mode] = [os.path.join(output, ASSEMBLY)]

    write("publish.log", "".join(publish_log))

    for mode in ("jit", "trimmed", "aot"):
        text = ""
        for tail in (["--closure"], WHOLE_RUN):
            command = images[mode] + tail
            code, output = run(command)
            shown = [part.replace(ROOT, "<root>") for part in command]
            text += logged(shown, code, output)
            failures += code != 0
        write("run-harness-%s.log" % mode, text)
        runs[mode] = text

    # The three modes must print the same catalog and the same run; only the command lines differ.
    same = lines_of(runs["jit"]) == lines_of(runs["trimmed"]) == lines_of(runs["aot"])
    write("harness-modes.log",
          "# the harness's catalog and a whole run, compared line for line across the modes\n"
          "# jit, trimmed and aot print the same lines: %s\n" % ("yes" if same else "NO"))
    failures += not same

    # The negative controls: the checks must fail against a store doctored in the two ways clause 6 is
    # about - the retired executor's order, and an amount other than the one it reported - and the tree is
    # restored after each.
    for control in CONTROLS:
        patch = os.path.join(HERE, control + ".patch")
        shown_patch = "%s/%s.patch" % (BUNDLE, control)
        code, text = run(["git", "apply", patch])
        log = logged(["git", "apply", shown_patch], code, text)
        if code == 0:
            command = ["dotnet", "build", HARNESS, "-c", "Release"]
            code, text = run(command)
            log += logged(command, code, text[-400:])
            command = jit + ["--verbose"]
            code, text = run(command)
            log += logged(command, code, text)
            failures += code == 0
            code, text = run(["git", "apply", "-R", patch])
            log += logged(["git", "apply", "-R", shown_patch], code, text)
            failures += code != 0
            command = ["dotnet", "build", HARNESS, "-c", "Release"]
            code, text = run(command)
            log += logged(command, code, text[-400:])
            failures += code != 0
        else:
            failures += 1
        write(control + ".log", log)

    command = ["dotnet", "test", "Broiler.VM.slnx", "-c", "Release", "--no-build"]
    code, text = run(command)
    write("tests.log", logged(command, code, text))
    failures += code != 0

    code, status = run(["git", "status", "--porcelain", "--", ".", ":!" + BUNDLE, ":!artifacts"])
    if status.strip():
        print("collect: the tree is not clean after the collection:\n" + status)
        failures += 1

    print("collect: %s" % ("every step answered as expected" if failures == 0 else "%d steps did NOT" % failures))
    return 0 if failures == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
