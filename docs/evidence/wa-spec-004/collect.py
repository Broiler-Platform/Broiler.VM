#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT RECORD WA-SPEC-004: the run that sets WA-4's ratchet for the malformed and invalid families, and the
# spec lane's discipline around it. The specification's core scripts run through the --spec lane at the pinned
# revision, taken twice; the first run writes the ratchet, the second is held to it. Then each named configuration
# failure, each way a ratchet is refused, and the self-check's negative control with the injection ignored.
#
# Run from the repository root at the commit being recorded, from a clean tree:
#
#   python3 docs/evidence/wa-spec-004/collect.py
#
# It writes, into this directory:
#   build.log                    the solution built with warnings as errors
#   set-run.log                  the scripts through the lane at the pin, writing the ratchet; every command on one line
#   wasm-spec.ratchet            the ratchet that run wrote
#   hold-run.log                 the same scripts again, held to that ratchet
#   determinism.log              whether the two runs printed the same per-command lines
#   refusals.log                 each refusal the lane names: a raised floor, another revision, other limits, a foreign
#                                family, a lowering write, a missing revision, an empty selection, an all-skipped one
#   control-ignored-injection.log  the patch applied, the lane that must fail its self-check run, the patch reverted
#   harness.log                  the harness root's run over the retained corpus
#   tests.log                    the solution's test projects
# It judges nothing: the README says what the files show.

import os
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
RECORD = "docs/evidence/wa-spec-004"
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", DOTNET_NOLOGO="1", DOTNET_CLI_TELEMETRY_OPTOUT="1")
HARNESS = "src/compositions/Broiler.VM.Composition.WebAssembly.Harness"
ASSEMBLY = "Broiler.VM.Composition.WebAssembly.Harness"
PIN = "src/tests/wasm/spec/wasm-spec.pin"
ARCHIVE = "src/tests/wasm/spec/wasm-spec-977f97014c962f7bd1291fcc6d28b41a924882bf-test-core.tar.gz"
CORPUS = "src/tests/wasm/corpus"
SCRATCH = os.path.join(ROOT, "artifacts", "wa-spec-004")
RATCHET = os.path.join(HERE, "wasm-spec.ratchet")


def run(command, cwd=ROOT):
    """Runs a command; answers its exit code and its output with the checkout's paths hidden."""
    completed = subprocess.run(command, cwd=cwd, env=ENV, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    text = completed.stdout.decode("utf-8", "replace").replace("\r\n", "\n")
    return completed.returncode, text.replace(SCRATCH, "<scratch>").replace(ROOT, "<root>")


def write(name, text):
    with open(os.path.join(HERE, name), "w", encoding="utf-8", newline="\n") as handle:
        handle.write(text if text.endswith("\n") else text + "\n")


def logged(command, code, text):
    shown = " ".join(command).replace(SCRATCH, "<scratch>").replace(ROOT, "<root>")
    return "$ %s\n%s# exit %d\n" % (shown, text, code)


def summary(text):
    """The lane's own lines: the self-check, the totals, the ratchet and every named failure."""
    return "\n".join(line for line in text.splitlines() if line.startswith(("# ", "broiler-wasm-harness:"))) + "\n"


def main():
    failures = 0

    code, status = run(["git", "status", "--porcelain", "--", ".", ":!" + RECORD])
    if status.strip():
        print("collect: the tree outside this record is not clean; refusing to collect")
        return 2

    _, head = run(["git", "rev-parse", "HEAD"])
    shutil.rmtree(SCRATCH, ignore_errors=True)
    os.makedirs(SCRATCH)

    if os.path.exists(RATCHET):
        os.remove(RATCHET)

    command = ["dotnet", "build", "Broiler.VM.slnx", "-c", "Release", "-warnaserror"]
    code, text = run(command)
    write("build.log", "# collected at %s" % head + logged(command, code, text))
    failures += code != 0

    suite_root = os.path.join(SCRATCH, "suite")
    os.makedirs(suite_root)
    code, text = run(["tar", "xzf", ARCHIVE, "-C", suite_root])
    failures += code != 0
    suite = os.path.join(suite_root, "test", "core")
    lane = ["dotnet", os.path.join(ROOT, HARNESS, "bin", "Release", "net10.0", ASSEMBLY + ".dll")]
    pinned = lane + ["--spec", suite, "--expect", PIN]

    # THE RUN THAT SETS THE RATCHET, and the same scripts held to it.
    command = pinned + ["--write-ratchet", RATCHET]
    code, first = run(command)
    write("set-run.log", logged(command, code, first))
    failures += code != 0

    command = pinned + ["--ratchet", RATCHET]
    code, second = run(command)
    write("hold-run.log", logged(command, code, second))
    failures += code != 0

    lines = lambda text: [line for line in text.splitlines() if not line.startswith("# spec: ratchet")]
    same = lines(first) == lines(second)
    write("determinism.log", "# the scripts taken twice, the same lines but for the ratchet's own: %s" % ("yes" if same else "NO"))
    failures += not same

    # EVERY REFUSAL, each with the exit code it must answer.
    with open(RATCHET, encoding="utf-8") as handle:
        ratchet = handle.read()

    def variant(name, text):
        path = os.path.join(SCRATCH, name)
        with open(path, "w", encoding="utf-8", newline="\n") as handle:
            handle.write(text)
        return path

    raised = variant("raised.ratchet", "\n".join(
        line.replace(" passed ", " passed 1", 1) if line.startswith("family malformed ") else line for line in ratchet.splitlines()) + "\n")
    other = variant("other.ratchet", "\n".join(
        "revision 0000000000000000000000000000000000000000" if line.startswith("revision ") else line for line in ratchet.splitlines()) + "\n")
    tighter = variant("tighter.ratchet", "\n".join(
        line.replace("Fuel=50000000", "Fuel=40000000") if line.startswith("limits ") else line for line in ratchet.splitlines()) + "\n")
    foreign = variant("foreign.ratchet", ratchet + "family trap passed 438 of 460 executed, 460 selected\n")
    higher = variant("higher.ratchet", "\n".join(
        line.replace(" passed ", " passed 1", 1) if line.startswith("family invalid ") else line for line in ratchet.splitlines()) + "\n")
    empty = os.path.join(SCRATCH, "empty")
    os.makedirs(empty)
    skipped = os.path.join(SCRATCH, "skipped")
    os.makedirs(skipped)
    variant("skipped/only-text.wast", '(assert_malformed (module quote "(func") "unexpected end")\n')

    refusals = [
        ("a floor above the run", pinned + ["--ratchet", raised], 1),
        ("a floor of another revision", pinned + ["--ratchet", other], 1),
        ("a floor set under other limits", pinned + ["--ratchet", tighter], 1),
        ("a floor naming a family WA-4 does not ratchet", pinned + ["--ratchet", foreign], 4),
        ("a write lower than the floor it would replace", pinned + ["--write-ratchet", higher], 1),
        ("a ratchet with no pinned revision", lane + ["--spec", suite, "--ratchet", RATCHET], 4),
        ("an empty selection", lane + ["--spec", empty], 4),
        ("a selection whose every command is skipped", lane + ["--spec", skipped], 4),
    ]

    text = ""
    for name, command, expected in refusals:
        code, output = run(command)
        answered = code == expected
        failures += not answered
        text += "## %s: exit %d, expected %d, %s\n" % (name, code, expected, "as expected" if answered else "NOT as expected")
        text += logged(command, code, summary(output))

    write("refusals.log", text)

    # THE SELF-CHECK'S NEGATIVE CONTROL, WITH THE INJECTION IGNORED: the lane must refuse to run a script.
    build = ["dotnet", "build", HARNESS, "-c", "Release"]
    patch = os.path.join(HERE, "control-ignored-injection.patch")
    shown = RECORD + "/control-ignored-injection.patch"
    code, text = run(["git", "apply", patch])
    control = logged(["git", "apply", shown], code, text)
    failures += code != 0

    if code == 0:
        code, text = run(build)
        control += logged(build, code, text[-400:])
        command = pinned
        code, text = run(command)
        control += logged(command, code, summary(text))
        failures += code != 3
        code, text = run(["git", "apply", "-R", patch])
        control += logged(["git", "apply", "-R", shown], code, text)
        failures += code != 0
        code, text = run(build)
        control += logged(build, code, text[-400:])
        failures += code != 0

    write("control-ignored-injection.log", control)

    command = lane + ["--corpus", CORPUS]
    code, text = run(command)
    write("harness.log", logged(command, code, text))
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
