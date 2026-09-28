#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT RECORD WA-SPEC-001: the WebAssembly profile's corrections to custom-section names, import entries,
# function types with two results and call_indirect's type check, and the --spec lane's scoring by what a refusal
# says. The specification's core test scripts run through the lane before the change (the base commit, in a working
# tree of its own) and after it, each run taken twice. It also takes the harness root's own run with the retained
# corpus, the corpus's integrity check, and one negative control per correction, each failing and then reverted.
#
# Run from the repository root at the commit being recorded, from a clean tree:
#
#   python3 docs/evidence/wa-spec-001/collect.py
#
# It writes, into this directory:
#   build.log               the solution built with warnings as errors
#   base-tree.log           the base working tree: its commit and its state
#   base-build.log          the harness root built in the base working tree
#   before-run.log          the scripts through the base's --spec lane, every command on one line
#   after-run.log           the same through this commit's lane
#   determinism.log         whether each run, taken twice, printed the same lines
#   harness.log             this commit's harness root: execution and differential checks, the corpus replayed
#   corpus-integrity.log    eng/wasm-corpus-integrity.py over the retained corpus, with its mutations
#   control-<name>.log      each control-<name>.patch applied, the lane that must fail run, the patch reverted
#   controls-passing.log    the harness root's run and the lane's self-check after every revert
#   comparison.log          compare.py's join of the two runs, per command
#   tests.log               the solution's test projects
# It judges nothing: the README says what the files show.

import os
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
RECORD = "docs/evidence/wa-spec-001"
BASE_COMMIT = "d380b5e"
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", DOTNET_NOLOGO="1", DOTNET_CLI_TELEMETRY_OPTOUT="1")
HARNESS = "src/compositions/Broiler.VM.Composition.WebAssembly.Harness"
ASSEMBLY = "Broiler.VM.Composition.WebAssembly.Harness"
PIN = "src/tests/wasm/spec/wasm-spec.pin"
ARCHIVE = "src/tests/wasm/spec/wasm-spec-977f97014c962f7bd1291fcc6d28b41a924882bf-test-core.tar.gz"
CORPUS = "src/tests/wasm/corpus"
SCRATCH = os.path.join(ROOT, "artifacts", "wa-spec-001")

# Each control, and the lane that has to fail with it applied: the harness root's run over the corpus for the four
# profile corrections, the --spec lane (which stops at its self-check) for the scoring.
CONTROLS = [
    ("custom-name", "corpus"),
    ("import-early", "corpus"),
    ("result-arity", "corpus"),
    ("nominal", "corpus"),
    ("scoring", "spec"),
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


def spec(image, suite):
    # The pin by its absolute path: the base run's working directory is the base tree.
    return image + ["--spec", suite, "--expect", os.path.join(ROOT, PIN)]


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

    suite_root = os.path.join(SCRATCH, "suite")
    os.makedirs(suite_root)
    code, text = run(["tar", "xzf", ARCHIVE, "-C", suite_root])
    failures += code != 0
    suite = os.path.join(suite_root, "test", "core")

    # THE BASE: a working tree at the commit before the change, built as it is.
    base = os.path.join(SCRATCH, "base")
    run(["git", "worktree", "remove", "--force", base])
    code, text = run(["git", "worktree", "add", "--detach", base, BASE_COMMIT])
    tree = logged(["git", "worktree", "add", "--detach", "<scratch>/base", BASE_COMMIT], code, text)
    failures += code != 0

    for command in (["git", "rev-parse", "HEAD"], ["git", "status", "--porcelain"]):
        code, text = run(command, cwd=base)
        tree += logged(command, code, text)

    write("base-tree.log", tree)

    command = ["dotnet", "build", HARNESS, "-c", "Release", "-warnaserror"]
    code, text = run(command, cwd=base)
    write("base-build.log", logged(command, code, text))
    failures += code != 0

    base_image = ["dotnet", os.path.join(base, HARNESS, "bin", "Release", "net10.0", ASSEMBLY + ".dll")]
    jit = ["dotnet", os.path.join(ROOT, HARNESS, "bin", "Release", "net10.0", ASSEMBLY + ".dll")]
    determinism = []

    for label, image, cwd in (("before", base_image, base), ("after", jit, ROOT)):
        command = spec(image, suite)
        code, first = run(command, cwd=cwd)
        write(label + "-run.log", logged(command, code, first))
        failures += code != 0
        code, second = run(command, cwd=cwd)
        failures += code != 0
        same = first == second
        determinism.append("# %s: the scripts taken twice, the same lines: %s" % (label, "yes" if same else "NO"))
        failures += not same

    write("determinism.log", "\n".join(determinism))

    command = jit + ["--verbose", "--corpus", CORPUS]
    code, text = run(command)
    write("harness.log", logged(command, code, text))
    failures += code != 0

    command = [sys.executable, "eng/wasm-corpus-integrity.py", "--corpus", CORPUS, "--"] + jit + ["--corpus", CORPUS]
    code, text = run(command)
    write("corpus-integrity.log", logged(command, code, text))
    failures += code != 0

    # The controls: each patch applied, the lane that must fail run, the patch reverted and the harness rebuilt.
    build = ["dotnet", "build", HARNESS, "-c", "Release"]

    for name, lane in CONTROLS:
        patch = os.path.join(HERE, "control-%s.patch" % name)
        shown = RECORD + "/control-%s.patch" % name
        code, text = run(["git", "apply", patch])
        control = logged(["git", "apply", shown], code, text)

        if code != 0:
            failures += 1
            write("control-%s.log" % name, control)
            continue

        code, text = run(build)
        control += logged(build, code, text[-400:])
        command = jit + ["--corpus", CORPUS] if lane == "corpus" else spec(jit, suite)
        code, text = run(command)
        control += logged(command, code, text)
        failures += code == 0
        code, text = run(["git", "apply", "-R", patch])
        control += logged(["git", "apply", "-R", shown], code, text)
        failures += code != 0
        code, text = run(build)
        control += logged(build, code, text[-400:])
        failures += code != 0
        write("control-%s.log" % name, control)

    passing = ""
    command = jit + ["--corpus", CORPUS]
    code, text = run(command)
    passing += logged(command, code, text)
    failures += code != 0
    command = spec(jit, suite)
    code, text = run(command)
    passing += logged(command, code, "\n".join(line for line in text.splitlines() if line.startswith("# self-check")) + "\n")
    failures += code != 0
    write("controls-passing.log", passing)

    command = [sys.executable, os.path.join(HERE, "compare.py"), HERE]
    code, text = run(command)
    write("comparison.log", text)
    failures += code != 0

    # The base tree goes before the suites run: rule A14 reads every project file under the checkout.
    run(["git", "worktree", "remove", "--force", base])

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
