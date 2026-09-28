#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT BUNDLE UBC-4-003: the second half of UBC-4's clause 3 for the region rows - every load, every store
# and memory.size compared with the primitive table's region primitive over the retained region inputs.
#
# Run from the repository root at the commit the bundle names, from a clean tree:
#
#   python3 docs/evidence/ubc-4-003/collect.py [--rid linux-x64]
#
# It writes, into this directory:
#   build.log                 the solution built with warnings as errors
#   run-harness-jit.log       the harness root's catalog, then its --regions and --primitives lanes, verbose, from
#                             the framework-dependent build
#   run-harness-trimmed.log   the same, from a trimmed self-contained publish
#   run-harness-aot.log       the same, from the Native AOT image
#   harness-modes.log         whether the three modes printed the same lanes, line for line
#   publish.log               the two publishes
#   fixture-corpus.log        the universal bytecode fixture root replaying src/tests/corpus/ubc-2, whose manifest
#                             now names regions.txt beside primitives.txt
#   control-failing.log       the --regions lane with control.patch applied to the reference arms - i32.load8_u
#                             sign-extending, and every store writing one byte past its window - then reverted
#   architecture-tests.log    the architecture suite
# It judges nothing: the README says what the files show.

import argparse
import os
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", DOTNET_NOLOGO="1", DOTNET_CLI_TELEMETRY_OPTOUT="1")
HARNESS = "src/compositions/Broiler.VM.Composition.WebAssembly.Harness"
ASSEMBLY = "Broiler.VM.Composition.WebAssembly.Harness"
FIXTURE = "src/compositions/Broiler.VM.Composition.Ubc.Fixture/bin/Release/net10.0/Broiler.VM.Composition.Ubc.Fixture.dll"
LANES = ["--regions", "src/tests/corpus/ubc-2/regions.txt", "--primitives", "src/tests/corpus/ubc-2/primitives.txt"]


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


def lanes_of(text):
    """The lines a mode's run prints for the two lanes, without the command and exit lines this script adds."""
    return [line for line in text.splitlines() if not line.startswith("$ ") and not line.startswith("# exit")]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--rid", default="linux-x64")
    arguments = parser.parse_args()
    failures = 0

    code, status = run(["git", "status", "--porcelain", "--", ".", ":!docs/evidence/ubc-4-003"])
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
        output = os.path.join(ROOT, "artifacts", "publish-ubc4-003-harness-" + mode)
        shutil.rmtree(output, ignore_errors=True)
        command = ["dotnet", "publish", HARNESS, "-c", "Release", "-r", arguments.rid, "-o", output] + extra
        code, text = run(command)
        publish_log.append("--- harness %s ---\n%s" % (mode.upper(), logged(command, code, text)))
        failures += code != 0
        images[mode] = [os.path.join(output, ASSEMBLY)]

    write("publish.log", "".join(publish_log))

    for mode in ("jit", "trimmed", "aot"):
        text = ""
        for tail in (["--closure"], LANES + ["--verbose"]):
            command = images[mode] + tail
            code, output = run(command)
            shown = [part.replace(ROOT, "<root>") for part in command]
            text += logged(shown, code, output)
            failures += code != 0
        write("run-harness-%s.log" % mode, text)
        runs[mode] = text

    # The three modes must print the same catalog and the same lanes; only the command lines differ.
    same = lanes_of(runs["jit"]) == lanes_of(runs["trimmed"]) == lanes_of(runs["aot"])
    write("harness-modes.log",
          "# the harness's catalog, --regions and --primitives lanes, compared line for line across the modes\n"
          "# jit, trimmed and aot print the same lines: %s\n" % ("yes" if same else "NO"))
    failures += not same

    command = ["dotnet", FIXTURE, "--corpus", "src/tests/corpus/ubc-2"]
    code, text = run(command)
    write("fixture-corpus.log", logged(command, code, text))
    failures += code != 0

    # The negative control: the lane must fail against a reference doctored in the two ways a region arm can
    # be wrong - the value it answers, and the bytes it writes - and the tree is restored after.
    patch = os.path.join(HERE, "control.patch")
    code, text = run(["git", "apply", patch])
    control = logged(["git", "apply", "docs/evidence/ubc-4-003/control.patch"], code, text)
    if code == 0:
        command = ["dotnet", "build", HARNESS, "-c", "Release"]
        code, text = run(command)
        control += logged(command, code, text[-400:])
        command = jit + ["--regions", "src/tests/corpus/ubc-2/regions.txt", "--verbose"]
        code, text = run(command)
        control += logged(command, code, text)
        failures += code == 0
        code, text = run(["git", "apply", "-R", patch])
        control += logged(["git", "apply", "-R", "docs/evidence/ubc-4-003/control.patch"], code, text)
        command = ["dotnet", "build", HARNESS, "-c", "Release"]
        code, text = run(command)
        control += logged(command, code, text[-400:])
    else:
        failures += 1
    write("control-failing.log", control)

    command = ["dotnet", "test", "src/tests/Broiler.VM.Architecture.Tests", "-c", "Release", "--no-build"]
    code, text = run(command)
    write("architecture-tests.log", logged(command, code, text))
    failures += code != 0

    code, status = run(["git", "status", "--porcelain", "--", ".", ":!docs/evidence/ubc-4-003", ":!artifacts"])
    if status.strip():
        print("collect: the tree is not clean after the collection:\n" + status)
        failures += 1

    print("collect: %s" % ("every step answered as expected" if failures == 0 else "%d steps did NOT" % failures))
    return 0 if failures == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
