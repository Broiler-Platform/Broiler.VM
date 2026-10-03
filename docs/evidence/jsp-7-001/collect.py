#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT RECORD JSP-7-001: the parity roadmap's JSP-7, the surfaces that were absent without being declared, and
# Annex B admitted whole. The acceptance suite over the host, each fixture the change added beside the comparison
# engine's answer for the same file, the slice-compiler root's checks, the retained corpus replayed and re-derived,
# the differential probes alone and beside the comparison engine on the base and on the change, the pinned test262's
# language, Annex B and touched built-in subtrees on the base and on the change, and one control per change: its
# patch applied, the rows it must fail, the patch reverted.
#
# Run from the repository root at the commit being recorded, from a clean tree, naming the commit before the
# change and the comparison engine:
#
#   python3 docs/evidence/jsp-7-001/collect.py --base <commit> --node <path to node>
#
# It writes, into this directory:
#   build.log            the solution built with warnings as errors
#   acceptance.log       every row of src/tests/cli/expected.txt judged on the host
#   comparison.log       each fixture the change added on the host and on the comparison engine, and whether they
#                        agree as declared
#   checks.log           the slice-compiler root's --checks: the closing total
#   corpus.log           the retained corpus replayed, and re-derived and compared byte for byte
#   differential.log     the differential probes' closing lines
#   differential-node.log  the comparison engine's findings over the probes on the base and on the change
#   test262-base.log     the subtrees on the base commit's conformance root
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
RECORD = "docs/evidence/jsp-7-001"
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", DOTNET_NOLOGO="1", DOTNET_CLI_TELEMETRY_OPTOUT="1")
SCRATCH = os.path.join(ROOT, "artifacts", "jsp-7-001")
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
# THE SUBTREES THE CHANGES REACH: the whole of the language, because the tokenizer now reads `<!--` and a line-leading
# `-->` in every script and a call may stand as an assignment target anywhere; the whole of Annex B; and the built-ins
# whose members were added, removed or changed - the Uint8Array codecs, JSON's reviver, Error.isError, WeakMap's pair,
# RegExp's `unicodeSets` and `compile`, the String and Date members, the registry, and Function, whose dynamic sources
# may hold the comments.
SUBTREES = ["test/language", "test/annexB"] + ["test/built-ins/" + name for name in (
    "Uint8Array", "JSON", "Error", "WeakMap", "RegExp", "String", "Date", "FinalizationRegistry", "Function")]
EXPECTED = "src/tests/cli/expected.txt"
MARKER = "# JSP-7, THE SURFACES THAT WERE ABSENT"

# EACH FIXTURE, THE HOST'S ARGUMENTS FOR IT, AND WHAT THE COMPARISON ENGINE MUST ANSWER BESIDE THE HOST:
#   same       both run it and print the same value
#   differs    both run it and the engine prints another value: it gives an error an own `stack`, which the edition
#              does not define and this host declines
#   lacks      the engine has no such member and throws a TypeError, where the host runs it: the row's values come
#              from the specification's algorithms
#   refused    the host refuses it at exit 3 and the engine raises a SyntaxError
#   early      the host refuses it at exit 3 and the engine runs it to a ReferenceError: in strict code a call is
#              not an assignment target, which test262 requires as an early error and the engine raises late
FIXTURES = [
    ([], "runs/annex-b-string-date-and-regexp-members.js", "same"),
    ([], "runs/html-like-comments-in-a-script.js", "same"),
    (["--module"], "refused/an-html-like-comment-in-a-module.mjs", "refused"),
    ([], "runs/a-for-in-initialiser.js", "same"),
    ([], "runs/a-call-as-an-assignment-target.js", "same"),
    ([], "refused/a-call-as-an-assignment-target-in-strict-code.js", "early"),
    ([], "runs/the-edition-members-the-realm-lacked.js", "lacks"),
    ([], "runs/uint8array-base64-and-hex.js", "lacks"),
    ([], "runs/a-reviver-reads-the-source-text.js", "same"),
    ([], "runs/an-error-has-no-stack.js", "differs"),
]

# ONE CONTROL PER CHANGE, each judged against the rows this change added.
CONTROLS = [
    "uint8-codecs",
    "partial-write",
    "reviver-context",
    "reviver-replaced",
    "html-methods",
    "trim-alias",
    "date-year",
    "gmt-alias",
    "regexp-compile",
    "html-comments",
    "html-comments-module",
    "for-in-initialiser",
    "call-target",
    "call-target-update",
    "call-target-strict",
    "is-error",
    "weakmap-upsert",
    "unicode-sets",
    "cleanup-some",
    "error-stack",
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

    for flags, path, expected in FIXTURES:
        host_code, host = run([CLI] + flags + [path], cwd=fixtures)
        with open(os.path.join(fixtures, path), encoding="utf-8") as handle:
            source = handle.read()

        # A LEADING LINE TERMINATOR, because a file may open with `-->` and the engine reads an argument that starts
        # with a dash as one of its own options. It moves nothing else: the comment still follows a line terminator.
        if expected in ("refused", "early"):
            engine_code, engine = run([node, path], cwd=fixtures)
        else:
            engine_code, engine = run([node, "-p", "\n" + source], cwd=fixtures)

        errors = [line for line in engine.splitlines() if "Error" in line]
        same = host_code == 0 and engine_code == 0 and host.strip() == engine.strip()

        if expected == "same":
            agrees = same
        elif expected == "differs":
            agrees = host_code == 0 and engine_code == 0 and not same
        elif expected == "lacks":
            agrees = host_code == 0 and any(line.startswith("TypeError") and "is not a function" in line
                                            for line in errors)
        elif expected == "early":
            agrees = host_code == 3 and any(line.startswith("ReferenceError") for line in errors)
        else:
            agrees = host_code == 3 and any(line.startswith("SyntaxError") for line in errors)

        wrong += not agrees
        engine_shown = engine.strip().splitlines()[0] if expected in ("same", "differs") and engine.strip() else (
            errors[0] if errors else "(printed nothing)")
        text += "%s  [%s] %s\n  host   exit %d: %s\n  engine exit %d: %s\n" % (
            " ".join(flags + [path]), expected, "agrees as declared" if agrees else "DOES NOT agree as declared",
            host_code, host.strip().splitlines()[0] if host.strip() else "", engine_code, engine_shown)

    return text, wrong


def findings(text):
    """The comparison engine's findings in a differential run, by probe: what a declaration did not cover."""
    found, probe = [], ""
    for line in text.splitlines():
        if line.startswith("--- "):
            probe = line[4:].strip()
        elif "FAIL:" in line and "retained answers" not in line:
            found.append(probe + ": " + line.split("FAIL:", 1)[1].strip())
    return found


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
    rows = os.path.join(SCRATCH, "jsp-7.expected.txt")
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
    code, text = run(["dotnet", "build", CLI_PROJECT, "-c", "Release"], cwd=worktree)
    failures += code != 0

    # THE COMPARISON ENGINE OVER THE PROBES, on the base host and on this one. A run against Node has findings on
    # both - surfaces Node 22 lacks, answers declared for another engine - so what is retained is the difference:
    # a finding the change added, or one it removed.
    seen = {}
    for name, directory in (
            ("base", os.path.join(worktree, CLI_PROJECT, "bin", "Release", "net10.0")),
            ("change", os.path.join(ROOT, CLI_PROJECT, "bin", "Release", "net10.0"))):
        command = ["python3", "eng/run-differential.py", "--against", arguments.node, "--binary-directory", directory,
                   "--report", os.path.join(SCRATCH, "differential-%s.json" % name)]
        code, text = run(command)
        seen[name] = findings(text)
    added = [line for line in seen["change"] if line not in seen["base"]]
    removed = [line for line in seen["base"] if line not in seen["change"]]
    write("differential-node.log",
          "# comparison engine %s# %d findings on the base, %d on the change\n# added by the change: %d\n%s"
          "# removed by the change: %d\n%s"
          % (engine, len(seen["base"]), len(seen["change"]), len(added), "".join("  %s\n" % line for line in added),
             len(removed), "".join("  %s\n" % line for line in removed)))
    failures += len(added) != 0
    reports = {}

    for name, directory in (
            ("base", os.path.join(worktree, CONFORMANCE_PROJECT, "bin", "Release", "net10.0")),
            ("change", os.path.join(ROOT, CONFORMANCE_PROJECT, "bin", "Release", "net10.0"))):
        out = os.path.join(SCRATCH, "test262-" + name)
        command = ["python3", "eng/run-test262.py", "--suite", suite, "--binary-directory", directory,
                   "--jobs", "3", "--out", out]
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
