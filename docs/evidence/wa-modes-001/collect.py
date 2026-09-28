#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT RECORD WA-MODES-001: the WebAssembly profile's two composition roots - the harness root and the execution
# root - published trimmed self-contained and Native AOT for one runtime identifier, with trim and AOT warnings treated
# as errors, and run in each of the three modes beside the framework-dependent JIT build. The harness root replays the
# retained corpus, its nesting entries included, and runs its execution and differential checks; the execution root
# runs its own checks. The three runs of each root are compared line by line, the closure is read off each published
# output, and the corpus's integrity check is run with the Native AOT image as its replay, so the mutations it makes
# must be noticed by that image.
#
# Run from the repository root at the commit being recorded, from a clean tree:
#
#   python3 docs/evidence/wa-modes-001/collect.py
#
# It writes, into this directory:
#   build.log                      the solution built with warnings as errors
#   publish.log                    the four publishes, each root trimmed and Native AOT
#   harness-<mode>.log             the harness root's run over the retained corpus, in jit, trimmed and aot
#   harness-closure-<mode>.log     the harness root's --closure report in each mode
#   execution-<mode>.log           the execution root's run in each mode
#   modes.log                      whether each trimmed and aot run printed the jit run's lines
#   closure.txt                    the files each publish holds, read off the output directory
#   corpus-integrity-aot.log       eng/wasm-corpus-integrity.py with the Native AOT image as the replay
#   tests.log                      the solution's test projects
# It judges nothing: the README says what the files show.

import os
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
RECORD = "docs/evidence/wa-modes-001"
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", DOTNET_NOLOGO="1", DOTNET_CLI_TELEMETRY_OPTOUT="1")
RID = "linux-x64"
CORPUS = "src/tests/wasm/corpus"
SCRATCH = os.path.join(ROOT, "artifacts", "wa-modes-001")

ROOTS = [
    ("harness", "src/compositions/Broiler.VM.Composition.WebAssembly.Harness", "Broiler.VM.Composition.WebAssembly.Harness",
     ["--verbose", "--corpus", CORPUS]),
    ("execution", "src/compositions/Broiler.VM.Composition.WebAssembly.Execution", "Broiler.VM.Composition.WebAssembly.Execution",
     []),
]

PUBLISHES = [
    ("trimmed", ["--self-contained", "true", "-p:PublishTrimmed=true"]),
    ("aot", ["-p:PublishAot=true"]),
]


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
    write("build.log", "# collected at %s" % head + logged(command, code, text))
    failures += code != 0

    # The publishes. Both projects set TreatWarningsAsErrors, and it is passed again here so that a trim or AOT warning
    # fails the publish whatever the project says.
    publish_log = []
    images = {}

    for name, project, assembly, _ in ROOTS:
        images[(name, "jit")] = ["dotnet", os.path.join(ROOT, project, "bin", "Release", "net10.0", assembly + ".dll")]

        for mode, extra in PUBLISHES:
            output = os.path.join(SCRATCH, "publish-%s-%s" % (name, mode))
            command = ["dotnet", "publish", project, "-c", "Release", "-r", RID, "-o", output,
                       "-p:TreatWarningsAsErrors=true"] + extra
            code, text = run(command)
            publish_log.append("--- %s %s ---\n%s" % (name, mode.upper(), logged(command, code, text)))
            failures += code != 0
            images[(name, mode)] = [os.path.join(output, assembly)]

    write("publish.log", "".join(publish_log))

    # The runs, in the three modes, and their comparison with the jit run.
    modes = []

    for name, _, _, arguments in ROOTS:
        outputs = {}

        for mode in ("jit", "trimmed", "aot"):
            command = images[(name, mode)] + arguments
            code, outputs[mode] = run(command)
            write("%s-%s.log" % (name, mode), logged(command, code, outputs[mode]))
            failures += code != 0

        for mode in ("trimmed", "aot"):
            same = outputs[mode] == outputs["jit"]
            modes.append("# %s %s: prints the jit run's lines: %s" % (name, mode, "yes" if same else "NO"))
            failures += not same

            if not same:
                jit_lines, mode_lines = outputs["jit"].splitlines(), outputs[mode].splitlines()
                for index, (was, now) in enumerate(zip(jit_lines, mode_lines)):
                    if was != now:
                        modes.append("  first difference at line %d:\n    jit:  %s\n    %s: %s" % (index + 1, was, mode, now))
                        break

    closures = {}
    for mode in ("jit", "trimmed", "aot"):
        command = images[("harness", mode)] + ["--closure"]
        code, closures[mode] = run(command)
        write("harness-closure-%s.log" % mode, logged(command, code, closures[mode]))
        failures += code != 0

    for mode in ("trimmed", "aot"):
        same = closures[mode] == closures["jit"]
        modes.append("# harness --closure %s: prints the jit run's lines: %s" % (mode, "yes" if same else "NO"))
        failures += not same

    write("modes.log", "\n".join(modes))

    # The closure, read off each published output rather than asserted: every file a publish holds, with the
    # Broiler.VM assemblies named, and for the trimmed publishes the count of the platform's own.
    closure = ["# closure rid=%s, read off each publish's output directory" % RID, ""]

    for name, _, assembly, _ in ROOTS:
        for mode, _ in PUBLISHES:
            output = os.path.join(SCRATCH, "publish-%s-%s" % (name, mode))
            files = sorted(os.listdir(output))
            broiler = [f for f in files if f.startswith("Broiler.") and f.endswith(".dll")]
            managed = [f for f in files if f.endswith(".dll")]
            closure.append("[%s %s] %d files, %d .dll files, %d of them Broiler assemblies" % (
                name, mode, len(files), len(managed), len(broiler)))
            closure.extend("  " + f for f in (broiler if mode == "trimmed" else files))
            closure.append("")

    write("closure.txt", "\n".join(closure))

    # The integrity check with the Native AOT image as the replay it mutates the corpus under.
    aot = images[("harness", "aot")]
    command = [sys.executable, "eng/wasm-corpus-integrity.py", "--corpus", CORPUS, "--"] + aot + ["--corpus", CORPUS]
    code, text = run(command)
    write("corpus-integrity-aot.log", logged(command, code, text))
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
