#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT BUNDLE UBC-4-005: UBC-4's clauses 4 and 5 under the dated rule decision-rule.md in this directory -
# population A (the specification's core test scripts at the pinned commit) and population B (the harness root's
# corpus, execution checks and differential checks) at the base commit a56180b and at the after commit, each
# population A run taken twice; the run after in three publish modes; the negative control; the comparison.
#
# Run from the repository root at the after commit, from a clean tree:
#
#   python3 docs/evidence/ubc-4-005/collect.py [--rid linux-x64] [--wabt <directory holding node_modules/wabt>]
#
# It writes, into this directory:
#   build.log                 the solution built with warnings as errors, at the after commit
#   base-tree.log             the base working tree: its commit, and every file the reader changes in it
#   base-build.log            the harness root built in the base working tree
#   base-run.log              population A at the base: the harness's --spec lane, every command on one line
#   base-population-b.log     population B at the base: the harness root's whole run, verbose, with the corpus
#   base-verdicts.txt         the base run's verdict per command: the floor the rule sets from the base
#   after-run.log             population A at the after commit, framework-dependent
#   after-population-b.log    population B at the after commit
#   after-verdicts.txt        the run after's verdict per command
#   determinism.log           whether each population A run, taken twice, printed the same lines
#   publish.log               the trimmed and Native AOT publishes of the harness root at the after commit
#   after-modes.log           whether the trimmed and Native AOT runs of population A printed the JIT run's lines
#   floor-after.log           the run after checked against the base floor: every command the base passes
#   control-failing.log       obligation E2 over the float comparisons with the reference arms' routing restored
#                             to the retired interpreter's (control.patch), then the patch reverted
#   control-passing.log       the same lane after the revert
#   comparison.log            compare.py's join of the base and the run after, per command and per check
#   reader-check.log          with --wabt: every text module the reader encodes, compared with wabt's encoding
#   tests.log                 the solution's test projects
# It judges nothing: the README says what the files show.

import argparse
import os
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
BUNDLE = "docs/evidence/ubc-4-005"
BASE_COMMIT = "a56180b"
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", DOTNET_NOLOGO="1", DOTNET_CLI_TELEMETRY_OPTOUT="1")
HARNESS = "src/compositions/Broiler.VM.Composition.WebAssembly.Harness"
ASSEMBLY = "Broiler.VM.Composition.WebAssembly.Harness"
READER = ["ScriptText.cs", "TextModule.cs", "TextInstructions.cs", "ScriptRunner.cs", "SpecSuite.cs"]
PIN = "src/tests/wasm/spec/wasm-spec.pin"
ARCHIVE = "src/tests/wasm/spec/wasm-spec-977f97014c962f7bd1291fcc6d28b41a924882bf-test-core.tar.gz"
SCRATCH = os.path.join(ROOT, "artifacts", "ubc4-005")
HOOK_AFTER = '''            if (args.Contains("--closure", StringComparer.Ordinal))
            {
                return ReportClosure();
            }
'''
HOOK = '''
            // The specification's scripts are a lane of their own, in runtimes of their own.
            if (args.Contains("--spec", StringComparer.Ordinal))
            {
                return SpecSuite.Run(args);
            }
'''


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


def spec(image, suite, extra=()):
    # The pin by its absolute path: the base run's working directory is the base tree, which has no pin.
    return image + ["--spec", suite, "--expect", os.path.join(ROOT, PIN)] + list(extra)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--rid", default="linux-x64")
    parser.add_argument("--wabt", default=None)
    arguments = parser.parse_args()
    failures = 0

    code, status = run(["git", "status", "--porcelain", "--", ".", ":!" + BUNDLE])
    if status.strip():
        print("collect: the tree outside this bundle is not clean; refusing to collect")
        return 2

    _, head = run(["git", "rev-parse", "HEAD"])
    shutil.rmtree(SCRATCH, ignore_errors=True)
    os.makedirs(SCRATCH)

    command = ["dotnet", "build", "Broiler.VM.slnx", "-c", "Release", "-warnaserror"]
    code, text = run(command)
    write("build.log", "# collected at %s" % head + logged(command, code, text))
    failures += code != 0

    # The suite, extracted from the retained archive into scratch; the harness checks it against the pin.
    suite_root = os.path.join(SCRATCH, "suite")
    os.makedirs(suite_root)
    code, text = run(["tar", "xzf", ARCHIVE, "-C", suite_root])
    failures += code != 0
    suite = os.path.join(suite_root, "test", "core")

    # THE BASE: a working tree at a56180b with the reader's files from this commit, the base's adapter from this
    # bundle and the same Program.cs lines. No product file is touched.
    base = os.path.join(SCRATCH, "base")
    run(["git", "worktree", "remove", "--force", base])
    code, text = run(["git", "worktree", "add", "--detach", base, BASE_COMMIT])
    tree = logged(["git", "worktree", "add", "--detach", "<scratch>/base", BASE_COMMIT], code, text)
    failures += code != 0

    for name in READER:
        shutil.copyfile(os.path.join(ROOT, HARNESS, name), os.path.join(base, HARNESS, name))

    shutil.copyfile(os.path.join(HERE, "base", "ScriptVerification.cs"), os.path.join(base, HARNESS, "ScriptVerification.cs"))
    program = os.path.join(base, HARNESS, "Program.cs")

    with open(program, encoding="utf-8") as handle:
        source = handle.read()

    if source.count(HOOK_AFTER) != 1:
        print("collect: the base Program.cs does not have the one place the hook goes")
        return 1

    with open(program, "w", encoding="utf-8", newline="\n") as handle:
        handle.write(source.replace(HOOK_AFTER, HOOK_AFTER + HOOK))

    for command in (["git", "rev-parse", "HEAD"], ["git", "status", "--porcelain"], ["git", "diff", "--stat"]):
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

    for label, image, cwd in (("base", base_image, base), ("after", jit, ROOT)):
        verdicts = os.path.join(SCRATCH, label + "-verdicts.txt")
        command = spec(image, suite, ["--write-verdicts", verdicts])
        code, first = run(command, cwd=cwd)
        write(label + "-run.log", logged(command, code, first))
        failures += code != 0
        shutil.copyfile(verdicts, os.path.join(HERE, label + "-verdicts.txt"))

        code, second = run(spec(image, suite), cwd=cwd)
        failures += code != 0
        same = [line for line in first.splitlines() if not line.startswith("# spec: verdicts written")] == second.splitlines()
        determinism.append("# %s: population A taken twice, the same lines: %s" % (label, "yes" if same else "NO"))
        failures += not same

        command = image + ["--verbose", "--corpus", "src/tests/wasm/corpus"]
        code, text = run(command, cwd=cwd)
        write(label + "-population-b.log", logged(command, code, text))

    write("determinism.log", "\n".join(determinism))

    # The run after in the two publish modes, which must print the JIT run's lines.
    publish_log = []
    modes = []
    _, jit_lines = run(spec(jit, suite))

    for mode, extra in (("trimmed", ["--self-contained", "true", "-p:PublishTrimmed=true"]),
                        ("aot", ["-p:PublishAot=true"])):
        output = os.path.join(SCRATCH, "publish-" + mode)
        command = ["dotnet", "publish", HARNESS, "-c", "Release", "-r", arguments.rid, "-o", output] + extra
        code, text = run(command)
        publish_log.append("--- harness %s ---\n%s" % (mode.upper(), logged(command, code, text)))
        failures += code != 0
        code, lines = run(spec([os.path.join(output, ASSEMBLY)], suite))
        failures += code != 0
        same = lines == jit_lines
        modes.append("# %s: population A prints the framework-dependent run's lines: %s" % (mode, "yes" if same else "NO"))
        failures += not same

    write("publish.log", "".join(publish_log))
    write("after-modes.log", "\n".join(modes))

    # The ratchet's first half: the run after against the floor the base sets.
    command = spec(jit, suite, ["--floor", os.path.join(HERE, "base-verdicts.txt")])
    code, text = run(command)
    write("floor-after.log", logged(command, code, "\n".join(line for line in text.splitlines() if line.startswith("# spec:")) + "\n"))

    # The negative control: E2 over the float comparisons with the reference arms' routing restored to the
    # retired interpreter's, then reverted.
    patch = os.path.join(HERE, "control.patch")
    lane = jit + ["--primitives", "src/tests/corpus/ubc-2/primitives.txt"]
    code, text = run(["git", "apply", patch])
    control = logged(["git", "apply", BUNDLE + "/control.patch"], code, text)

    if code == 0:
        command = ["dotnet", "build", HARNESS, "-c", "Release"]
        code, text = run(command)
        control += logged(command, code, text[-400:])
        code, text = run(lane)
        control += logged(lane, code, text)
        failures += code == 0
        code, text = run(["git", "apply", "-R", patch])
        control += logged(["git", "apply", "-R", BUNDLE + "/control.patch"], code, text)
        failures += code != 0
        command = ["dotnet", "build", HARNESS, "-c", "Release"]
        code, text = run(command)
        control += logged(command, code, text[-400:])
        failures += code != 0
    else:
        failures += 1

    write("control-failing.log", control)
    code, text = run(lane)
    write("control-passing.log", logged(lane, code, text))
    failures += code != 0

    command = [sys.executable, os.path.join(HERE, "compare.py"), HERE]
    code, text = run(command)
    write("comparison.log", text)
    failures += code != 0

    if arguments.wabt:
        encoded = os.path.join(SCRATCH, "encoded")
        code, text = run(spec(jit, suite, ["--encode-to", encoded]))
        check = logged(spec(jit, suite, ["--encode-to", "<scratch>/encoded"]), code, text)
        modules = os.path.join(SCRATCH, "modules.json")
        code, text = run([sys.executable, os.path.join(HERE, "reader-check", "extract.py"), suite, modules])
        check += text
        code, text = run(["node", os.path.join(HERE, "reader-check", "compare.js"), modules, encoded], cwd=arguments.wabt)
        check += text
        write("reader-check.log", check)
        failures += code != 0

    # The base tree goes before the suites run: rule A14 reads every project file under the checkout, and the
    # base tree's would be read as this commit's.
    run(["git", "worktree", "remove", "--force", base])

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
