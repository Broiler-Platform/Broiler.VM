#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT RECORD JSP-3-001: the parity roadmap's JSP-3, the static semantics the wide front end did not have. The
# acceptance suite over the host, each JSP-3 fixture beside the comparison engine's answer for the same file, the
# slice-compiler root's checks, the retained corpus replayed and re-derived, the differential probes, the pinned
# test262's test/language and test/annexB on the base and on the change, and one control per change: its patch
# applied, the rows it must fail, the patch reverted.
#
# Run from the repository root at the commit being recorded, from a clean tree, naming the commit before the
# change and the comparison engine:
#
#   python3 docs/evidence/jsp-3-001/collect.py --base <commit> --node <path to node>
#
# It writes, into this directory:
#   build.log            the solution built with warnings as errors
#   acceptance.log       every row of src/tests/cli/expected.txt judged on the host
#   comparison.log       each JSP-3 fixture on the host and on the comparison engine, and whether they agree
#   checks.log           the slice-compiler root's --checks: the closing total
#   corpus.log           the retained corpus replayed, and re-derived and compared byte for byte
#   differential.log     the differential probes' closing lines
#   test262-base.log     test/language and test/annexB on the base commit's conformance root
#   test262-change.log   the same on this commit's
#   test262-moves.log    every variant whose verdict differs between the two
#   control-<name>.log   each control: the patch applied, what it must fail, the patch reverted
#   rows-after-controls.log   the rows this change added, after every revert
#   tests.log            the solution's test projects
# It judges nothing but whether each step answered as it must: the README says what the files show.

import argparse
import filecmp
import os
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
RECORD = "docs/evidence/jsp-3-001"
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", DOTNET_NOLOGO="1", DOTNET_CLI_TELEMETRY_OPTOUT="1")
SCRATCH = os.path.join(ROOT, "artifacts", "jsp-3-001")
CLI_PROJECT = "src/compositions/Broiler.VM.Composition.JavaScript.Cli"
CLI = os.path.join(ROOT, CLI_PROJECT, "bin", "Release", "net10.0", "Broiler.VM.Composition.JavaScript.Cli")
SLICE_PROJECT = "src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler"
SLICE = ["dotnet", os.path.join(ROOT, SLICE_PROJECT, "bin", "Release", "net10.0",
                                "Broiler.VM.Composition.JavaScript.SliceCompiler.dll")]
EXECUTION = ["dotnet", os.path.join(ROOT, "src", "compositions", "Broiler.VM.Composition.JavaScript.ExecutionOnly",
                                    "bin", "Release", "net10.0",
                                    "Broiler.VM.Composition.JavaScript.ExecutionOnly.dll")]
CONFORMANCE_PROJECT = "src/compositions/Broiler.VM.Composition.JavaScript.Conformance"
ARCHIVE = "src/tests/conformance/pins/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e.tar.gz"
CORPUS = "src/tests/corpus/js-1"
# The families live in the language's grammar and in Annex B, and a label or a jump can stand in any statement, so the
# whole of both trees is run rather than the subtrees named after the families.
SUBTREES = ["test/language", "test/annexB"]
EXPECTED = "src/tests/cli/expected.txt"
MARKER = "# JSP-3, EARLY ERRORS THIS HOST ADMITTED"

# EACH FIXTURE AND WHAT THE COMPARISON ENGINE MUST ANSWER BESIDE THE HOST:
#   same       both run it and print the same value
#   refused    the host refuses it at exit 3 and the engine raises a SyntaxError
#   admitted   the host refuses it at exit 3 and the engine runs it: the one place this host follows the
#              specification where the engine does not
#   threw      both throw the named error
FIXTURES = [
    ("runs/a-legacy-octal-escape-in-sloppy-code.js", "same"),
    ("refused/a-legacy-octal-escape-in-strict-code.js", "refused"),
    ("refused/a-legacy-octal-escape-before-use-strict.js", "refused"),
    ("refused/a-non-octal-decimal-escape-in-a-class-body.js", "refused"),
    ("refused/a-legacy-octal-escape-in-a-class-element-name.js", "admitted"),
    ("runs/a-tagged-template-with-an-invalid-escape.js", "same"),
    ("refused/an-octal-escape-in-an-untagged-template.js", "refused"),
    ("refused/a-second-constructor.js", "refused"),
    ("runs/a-class-with-one-constructor-and-a-static-one.js", "same"),
    ("refused/a-label-nested-in-itself.js", "refused"),
    ("runs/a-label-reused-where-it-is-not-in-force.js", "same"),
    ("runs/yield-and-await-as-labels.js", "same"),
    ("refused/a-coalesce-beside-an-or.js", "refused"),
    ("runs/a-coalesce-mixed-with-parentheses.js", "same"),
    ("refused/a-line-break-before-an-arrow.js", "refused"),
    ("runs/an-arrow-body-on-the-next-line.js", "same"),
    ("refused/a-restricted-name-as-a-function-name-under-its-directive.js", "refused"),
    ("refused/a-future-reserved-word-as-a-class-name.js", "refused"),
    ("refused/eval-as-a-parameter-under-a-body-directive.js", "refused"),
    ("refused/a-unary-operand-of-exponentiation.js", "refused"),
    ("dead-zone/a-read-before-the-initialiser-in-a-function-body.js", "threw ReferenceError"),
]

# ONE CONTROL PER CHANGE, each judged against the rows this change added.
CONTROLS = [
    "octal-decode",
    "strict-escape",
    "prologue",
    "tagged-template",
    "template-octal",
    "constructor",
    "label",
    "label-scope",
    "coalesce",
    "parenthesised",
    "arrow",
    "yield-await-label",
    "jump-label",
]


def run(command, cwd=ROOT, stdin=None):
    """Runs a command; answers its exit code and its output with the checkout's paths hidden."""
    completed = subprocess.run(command, cwd=cwd, env=ENV, input=stdin, stdout=subprocess.PIPE,
                               stderr=subprocess.STDOUT)
    text = completed.stdout.decode("utf-8", "replace").replace("\r\n", "\n")
    return completed.returncode, text.replace(SCRATCH, "<scratch>").replace(ROOT, "<root>")


def shown(command):
    return " ".join(command).replace(SCRATCH, "<scratch>").replace(ROOT, "<root>")


def logged(command, code, text):
    return "$ %s\n%s# exit %d\n" % (shown(command), text if text.endswith("\n") or not text else text + "\n", code)


def write(name, text):
    with open(os.path.join(HERE, name), "w", encoding="utf-8", newline="\n") as handle:
        handle.write(text if text.endswith("\n") else text + "\n")


def closing(text, prefix):
    return "".join(line + "\n" for line in text.splitlines() if line.startswith(prefix))


def verdicts(report):
    """Every variant's verdict in a merged test262 report, by its path and variant."""
    found = {}
    with open(report, encoding="utf-8") as handle:
        for line in handle:
            fields = line.rstrip("\n").split("|")
            if fields[0] == "result" and len(fields) >= 4:
                found[fields[1] + " [" + fields[2] + "]"] = fields[3]
    return found


def compare(node):
    """Each fixture on the host and on the comparison engine; answers the log and how many disagree."""
    text, wrong = "", 0
    fixtures = os.path.join(ROOT, "src", "tests", "cli")

    for path, expected in FIXTURES:
        host_code, host = run([CLI, path], cwd=fixtures)
        with open(os.path.join(fixtures, path), encoding="utf-8") as handle:
            source = handle.read()

        if expected == "same":
            engine_code, engine = run([node, "-p", source], cwd=fixtures)
        else:
            engine_code, engine = run([node, path], cwd=fixtures)

        errors = [line for line in engine.splitlines() if "Error" in line]
        if expected == "same":
            agrees = host_code == 0 and engine_code == 0 and host.strip() == engine.strip()
        elif expected == "refused":
            agrees = host_code == 3 and any(line.startswith("SyntaxError") for line in errors)
        elif expected == "admitted":
            agrees = host_code == 3 and engine_code == 0 and not errors
        else:
            name = expected.split(" ", 1)[1]
            agrees = host_code == 1 and name in host and any(line.startswith(name) for line in errors)

        wrong += not agrees
        engine_shown = engine.strip() if expected == "same" else (errors[0] if errors else "(ran, printed nothing)")
        text += "%s  [%s] %s\n  host   exit %d: %s\n  engine exit %d: %s\n" % (
            path, expected, "agrees as declared" if agrees else "DOES NOT agree as declared",
            host_code, host.strip().splitlines()[0] if host.strip() else "", engine_code, engine_shown)

    return text, wrong


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--base", required=True, help="the commit before the change")
    parser.add_argument("--node", required=True, help="the comparison engine")
    arguments = parser.parse_args()
    failures = 0

    code, status = run(["git", "status", "--porcelain", "--", ".", ":!" + RECORD])
    if status.strip():
        print("collect: the tree outside this record is not clean; refusing to collect")
        return 2

    _, head = run(["git", "rev-parse", "HEAD"])
    _, base = run(["git", "rev-parse", arguments.base])
    _, engine = run([arguments.node, "--version"])
    shutil.rmtree(SCRATCH, ignore_errors=True)
    os.makedirs(SCRATCH)

    command = ["dotnet", "build", "Broiler.VM.slnx", "-c", "Release", "-warnaserror"]
    code, text = run(command)
    write("build.log", "# collected at %s# base %s# comparison engine %s" % (head, base, engine)
          + logged(command, code, text[-600:]))
    failures += code != 0

    # THE ACCEPTANCE SUITE, every row.
    command = ["python3", "eng/run-cli-acceptance.py"]
    code, text = run(command)
    write("acceptance.log", logged(command, code, text))
    failures += code != 0

    # THE ROWS THIS CHANGE ADDED, as a table of their own the controls are judged against.
    with open(os.path.join(ROOT, EXPECTED), encoding="utf-8") as handle:
        table = handle.read()
    rows = os.path.join(SCRATCH, "jsp-3.expected.txt")
    with open(rows, "w", encoding="utf-8", newline="\n") as handle:
        handle.write(table[table.index(MARKER):])
    judge = ["python3", "eng/run-cli-acceptance.py", "--verbose", "--expected", rows]

    # EACH FIXTURE BESIDE THE COMPARISON ENGINE.
    text, wrong = compare(arguments.node)
    write("comparison.log", "# comparison engine %s%s# %d of %d fixtures do not agree as declared\n"
          % (engine, text, wrong, len(FIXTURES)))
    failures += wrong

    # THE SLICE-COMPILER ROOT'S CHECKS, by their closing total.
    command = SLICE + ["--checks", "--verbose"]
    code, text = run(command)
    write("checks.log", logged(command, code, closing(text, "broiler-js-slice-compiler:")))
    failures += code != 0

    # THE RETAINED CORPUS, replayed, and re-derived and compared: a front-end change that moved a lowered byte of
    # an accepted program would show here.
    command = EXECUTION + ["--corpus", CORPUS]
    code, text = run(command)
    corpus = logged(command, code, closing(text, "broiler-js-execution-only:"))
    failures += code != 0
    written = os.path.join(SCRATCH, "corpus")
    command = SLICE + ["--write", written]
    code, text = run(command)
    corpus += logged(command, code, closing(text, "broiler-js-slice-compiler:"))
    failures += code != 0
    comparison = filecmp.dircmp(written, os.path.join(ROOT, CORPUS))
    differing = []

    def walk(node, prefix):
        differing.extend(prefix + name for name in node.diff_files)
        for name, child in node.subdirs.items():
            walk(child, prefix + name + "/")

    walk(comparison, "")
    corpus += "# re-derived entries whose bytes differ from the retained ones: %d\n" % len(differing)
    corpus += "".join("  %s\n" % name for name in differing)
    failures += len(differing) != 0
    write("corpus.log", corpus)

    # THE DIFFERENTIAL PROBES.
    command = ["python3", "eng/run-differential.py"]
    code, text = run(command)
    write("differential.log", logged(command, code, "".join(text.splitlines(keepends=True)[-4:])))
    failures += code != 0

    # THE PINNED SUITE'S TWO TREES, on the base commit's conformance root and on this one's.
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
                   "--jobs", "2", "--out", out]
        for subtree in SUBTREES:
            command += ["--dir", subtree]
        code, text = run(command)
        write("test262-%s.log" % name, logged(command, code, text[-3000:]))
        reports[name] = os.path.join(out, "test262.report")

    before, after = verdicts(reports["base"]), verdicts(reports["change"])
    moves = sorted((variant, before.get(variant), after.get(variant))
                   for variant in set(before) | set(after) if before.get(variant) != after.get(variant))
    backwards = [move for move in moves if move[2] != "Passed"]
    write("test262-moves.log",
          "# %d variants on the base, %d on the change; %d change verdict, %d of them to anything but Passed\n"
          % (len(before), len(after), len(moves), len(backwards))
          + "".join("%s %s -> %s\n" % move for move in moves))
    failures += len(backwards) != 0
    run(["git", "worktree", "remove", "--force", worktree])

    # ONE CONTROL PER CHANGE: applied, judged, reverted.
    for name in CONTROLS:
        patch = os.path.join(HERE, "control-%s.patch" % name)
        record = RECORD + "/control-%s.patch" % name
        code, text = run(["git", "apply", patch])
        control = logged(["git", "apply", record], code, text)
        failures += code != 0

        if code == 0:
            build = ["dotnet", "build", CLI_PROJECT, "-c", "Release"]
            code, text = run(build)
            control += logged(build, code, text[-400:])
            failures += code != 0
            code, text = run(judge)
            control += logged(judge, code, text)
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
