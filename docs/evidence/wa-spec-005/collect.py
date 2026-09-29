#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT RECORD WA-SPEC-005: WA-4's selection pipeline, sharding, merge, failure queue and tooling regression suite,
# over the pinned scripts. A whole run under the slice's scope manifest, held to the ratchet; the same selection in
# four shards, their answers compared with the whole run's line for line; the merge of the four, held to the ratchet
# and writing the failure queue; the merge again, held to that queue; every refusal the pipeline, the merge and the
# queue name, with the exit code each must answer; and the tooling suite catching a merge that sums a partial set.
#
# Run from the repository root at the commit being recorded, from a clean tree:
#
#   python3 docs/evidence/wa-spec-005/collect.py
#
# It writes, into this directory:
#   build.log                        the solution built with warnings as errors
#   whole-run.log                    the scripts through the lane at the pin, under the scope manifest, held to the
#                                    ratchet; every command on one line
#   shard-<i>.log                    each of the four shards' own lines: the tooling suite, the self-check, the
#                                    selection, the totals (its answer lines are compared, not retained)
#   shard-<i>.report                 each shard's report
#   comparison.log                   whether the four shards' answer lines are the whole run's, and their merged
#                                    family totals the whole run's
#   merge.log                        the merge of the four, held to the ratchet, writing the queue
#   wasm-spec.queue                  the failure queue that merge wrote
#   merge-queue.log                  the merge again, held to that queue
#   refusals.log                     each refusal, with the exit code it must answer
#   control-partial-merge.log        the patch applied, the merge that must fail its tooling suite run, the patch reverted
#   harness.log                      the harness root's run over the retained corpus
#   tests.log                        the solution's test projects
# It judges nothing: the README says what the files show.

import os
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
RECORD = "docs/evidence/wa-spec-005"
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", DOTNET_NOLOGO="1", DOTNET_CLI_TELEMETRY_OPTOUT="1")
HARNESS = "src/compositions/Broiler.VM.Composition.WebAssembly.Harness"
ASSEMBLY = "Broiler.VM.Composition.WebAssembly.Harness"
PIN = "src/tests/wasm/spec/wasm-spec.pin"
SCOPE = "src/tests/wasm/spec/wasm-spec.slice.scope"
RATCHET = "src/tests/wasm/spec/wasm-spec.ratchet"
ARCHIVE = "src/tests/wasm/spec/wasm-spec-977f97014c962f7bd1291fcc6d28b41a924882bf-test-core.tar.gz"
CORPUS = "src/tests/wasm/corpus"
SCRATCH = os.path.join(ROOT, "artifacts", "wa-spec-005")
QUEUE = os.path.join(HERE, "wasm-spec.queue")
SHARDS = 4


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


def own(text):
    """The lane's own lines: the tooling suite, the self-check, the selection, the totals and every named failure."""
    return "\n".join(line for line in text.splitlines() if line.startswith(("# ", "broiler-wasm-harness:"))) + "\n"


def answers(text):
    return [line for line in text.splitlines() if not line.startswith(("# ", "broiler-wasm-harness:", "$ "))]


def families(text, prefix):
    return [line[len(prefix):] for line in text.splitlines() if line.startswith(prefix)]


def main():
    failures = 0

    code, status = run(["git", "status", "--porcelain", "--", ".", ":!" + RECORD])
    if status.strip():
        print("collect: the tree outside this record is not clean; refusing to collect")
        return 2

    _, head = run(["git", "rev-parse", "HEAD"])
    shutil.rmtree(SCRATCH, ignore_errors=True)
    os.makedirs(SCRATCH)

    for name in os.listdir(HERE):
        if name.startswith("shard-") or name == "wasm-spec.queue":
            os.remove(os.path.join(HERE, name))

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
    scoped = lane + ["--spec", suite, "--expect", PIN, "--scope", SCOPE]

    # THE WHOLE SELECTION, held to the ratchet.
    command = scoped + ["--ratchet", RATCHET]
    code, whole = run(command)
    write("whole-run.log", logged(command, code, whole))
    failures += code != 0

    # THE SAME SELECTION IN FOUR SHARDS, each writing its report.
    shards = os.path.join(SCRATCH, "shards")
    os.makedirs(shards)
    sharded = []

    for index in range(SHARDS):
        report = os.path.join(shards, "%d.report" % index)
        command = scoped + ["--shard", "%d/%d" % (index, SHARDS), "--report", report]
        code, text = run(command)
        write("shard-%d.log" % index, logged(command, code, own(text)))
        failures += code != 0
        sharded.extend(answers(text))
        shutil.copyfile(report, os.path.join(HERE, "shard-%d.report" % index))

    # THE MERGE, held to the ratchet and writing the queue; then held to that queue.
    command = lane + ["--merge", shards, "--ratchet", RATCHET, "--write-queue", QUEUE]
    code, merged = run(command)
    write("merge.log", logged(command, code, merged))
    failures += code != 0

    command = lane + ["--merge", shards, "--queue", QUEUE]
    code, text = run(command)
    write("merge-queue.log", logged(command, code, text))
    failures += code != 0

    same_answers = sorted(sharded) == sorted(answers(whole))
    same_totals = families(merged, "# merge: family ") == families(whole, "# spec: family ")
    write("comparison.log",
          "# the four shards' %d answer lines are the whole run's %d, line for line: %s\n"
          "# the merge's family totals are the whole run's: %s\n"
          % (len(sharded), len(answers(whole)), "yes" if same_answers else "NO", "yes" if same_totals else "NO"))
    failures += not same_answers
    failures += not same_totals

    # EVERY REFUSAL, each with the exit code it must answer.
    def variant_dir(name, change):
        path = os.path.join(SCRATCH, name)
        shutil.copytree(shards, path)
        change(path)
        return path

    def rewrite(path, old, new):
        with open(path, encoding="utf-8") as handle:
            text = handle.read()
        with open(path, "w", encoding="utf-8", newline="\n") as handle:
            handle.write(text.replace(old, new, 1))

    with open(os.path.join(shards, "1.report"), encoding="utf-8") as handle:
        report_one = handle.read()

    missing = variant_dir("missing", lambda path: os.remove(os.path.join(path, "3.report")))
    inconsistent = variant_dir("inconsistent", lambda path: rewrite(os.path.join(path, "2.report"), "Fuel=50000000", "Fuel=40000000"))
    unfinished = variant_dir("unfinished", lambda path: rewrite(
        os.path.join(path, "1.report"), report_one[report_one.index("family "):], ""))

    def queue_variant(name, change):
        with open(QUEUE, encoding="utf-8") as handle:
            text = handle.read()
        path = os.path.join(SCRATCH, name)
        with open(path, "w", encoding="utf-8", newline="\n") as handle:
            handle.write(change(text))
        return path

    written_by_hand = queue_variant("written-by-hand.queue", lambda text: text + "path written-by-hand.wast failing 1\n")
    passing = queue_variant("passing.queue", lambda text: text + "path address.wast failing 1\n")
    unlisted = queue_variant("unlisted.queue", lambda text: "\n".join(
        line for line in text.splitlines() if not line.startswith("path imports.wast ")) + "\n")

    absent_scope = os.path.join(SCRATCH, "absent.scope")
    with open(os.path.join(ROOT, SCOPE), encoding="utf-8") as handle:
        scope_text = handle.read()
    with open(absent_scope, "w", encoding="utf-8", newline="\n") as handle:
        handle.write(scope_text + "file absent.wast\n")

    refusals = [
        ("a merge missing one shard's report", lane + ["--merge", missing], 4),
        ("a merge of reports run under different limits", lane + ["--merge", inconsistent], 4),
        ("a merge of a report cut short, a shard that did not finish", lane + ["--merge", unfinished], 4),
        ("a scope manifest naming a script the suite does not contain", lane + ["--spec", suite, "--expect", PIN, "--scope", absent_scope], 4),
        ("a ratchet held by one shard", scoped + ["--shard", "1/%d" % SHARDS, "--ratchet", RATCHET], 4),
        ("a queue holding a hand-written entry for a script the suite does not have", lane + ["--merge", shards, "--queue", written_by_hand], 1),
        ("a queue listing a script that passes", lane + ["--merge", shards, "--queue", passing], 1),
        ("a queue missing a script that fails", lane + ["--merge", shards, "--queue", unlisted], 1),
    ]

    text = ""
    for name, command, expected in refusals:
        code, output = run(command)
        answered = code == expected
        failures += not answered
        text += "## %s: exit %d, expected %d, %s\n" % (name, code, expected, "as expected" if answered else "NOT as expected")
        text += logged(command, code, own(output))

    write("refusals.log", text)

    # THE TOOLING SUITE CATCHES A MERGE THAT SUMS A PARTIAL SET: the lane must refuse to merge.
    build = ["dotnet", "build", HARNESS, "-c", "Release"]
    patch = os.path.join(HERE, "control-partial-merge.patch")
    shown = RECORD + "/control-partial-merge.patch"
    code, text = run(["git", "apply", patch])
    control = logged(["git", "apply", shown], code, text)
    failures += code != 0

    if code == 0:
        code, text = run(build)
        control += logged(build, code, text[-400:])
        command = lane + ["--merge", missing]
        code, text = run(command)
        control += logged(command, code, own(text))
        failures += code != 3
        code, text = run(["git", "apply", "-R", patch])
        control += logged(["git", "apply", "-R", shown], code, text)
        failures += code != 0
        code, text = run(build)
        control += logged(build, code, text[-400:])
        failures += code != 0

    write("control-partial-merge.log", control)

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
