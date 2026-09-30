#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT RECORD JSP-9-001: the parity roadmap's JSP-9, the catalogue of what must not be taken from the comparison
# engine, as declarations the differential driver reports. The driver's own tests, the probes against their retained
# answers, every probe against Broiler.JS at the commit the declarations were taken against, the changed probes
# against Node, and one control per claim: its patch applied, the run it must fail, the patch reverted.
#
# Run from the repository root at the commit being recorded, from a clean tree, naming the Broiler.JS apphost, its
# checkout, and Node:
#
#   python3 docs/evidence/jsp-9-001/collect.py --broiler-js <apphost> --broiler-js-root <checkout> --node <node>
#
# It writes, into this directory:
#   build.log              the solution built with warnings as errors
#   tooling.log            the driver's own tests
#   retained.log           every probe against its retained answers
#   broiler-js.log         every probe against Broiler.JS: the driver's output
#   catalogue.log          that run judged: every broiler-js declaration reported declared, none stale, and the
#                          findings outside the catalogue by probe
#   node.log               the probes this change added or changed, against Node
#   control-<name>.log     each control: the patch applied, the run it must fail, the patch reverted
#   after-controls.log     the controlled probes against Broiler.JS once every patch is reverted
#   tests.log              the solution's test projects
# It judges nothing but whether each step answered as it must: the README says what the files show.

import argparse
import json
import os
import re
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
RECORD = "docs/evidence/jsp-9-001"
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", DOTNET_NOLOGO="1", DOTNET_CLI_TELEMETRY_OPTOUT="1")
SCRATCH = os.path.join(ROOT, "artifacts", "jsp-9-001")
PROBES = os.path.join(ROOT, "src", "tests", "differential")
# The commit of Broiler.JS the declarations were taken against, which the probes' README names.
PINNED = "c249764"
# The probes this change added, and the one whose source it changed.
CHANGED = ["the-comparison-engine-catalogue", "the-recursive-built-ins", "the-async-family"]
# EACH CONTROL: the patch, the probe it runs, and what the driver must then report.
CONTROLS = [
    ("case-declaration-removed", "the-comparison-engine-catalogue", "undeclared divergence broiler-js/8"),
    ("run-declaration-removed", "the-with-statement", "duplicate case 54"),
    ("stale-case-declaration", "the-settling-of-promises", "stale divergence broiler-js/1"),
    ("stale-run-declaration", "the-settling-of-promises", "stale divergence broiler-js/run"),
    ("driver-without-run", "the-with-statement", "use #diverges <engine> <case> <reason>"),
]


def run(command, cwd=ROOT):
    """Runs a command; answers its exit code and its output with the checkout's paths hidden."""
    completed = subprocess.run(command, cwd=cwd, env=ENV, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    text = completed.stdout.decode("utf-8", "replace").replace("\r\n", "\n")
    return completed.returncode, text.replace(SCRATCH, "<scratch>").replace(ROOT, "<root>")


def shown(command):
    return " ".join(command).replace(SCRATCH, "<scratch>").replace(ROOT, "<root>")


def logged(command, code, text):
    return "$ %s\n%s# exit %d\n" % (shown(command), text if text.endswith("\n") or not text else text + "\n", code)


def write(name, text):
    with open(os.path.join(HERE, name), "w", encoding="utf-8", newline="\n") as handle:
        handle.write(text if text.endswith("\n") else text + "\n")


def declarations(engine):
    """Every declaration for the engine in the answer files, as (probe, case)."""
    found = set()
    for name in sorted(os.listdir(PROBES)):
        if name.endswith(".expected.txt"):
            with open(os.path.join(PROBES, name), encoding="utf-8") as handle:
                for line in handle:
                    parts = line.split(" ", 3)
                    if parts[0] == "#diverges" and len(parts) == 4 and parts[1] == engine:
                        found.add((name[:-len(".expected.txt")], parts[2]))
    return found


def judge(report_path):
    """The Broiler.JS run judged: answers the log and how many of its claims failed."""
    with open(report_path, encoding="utf-8") as handle:
        report = json.load(handle)
    declared = declarations("broiler-js")
    reported, stale, findings = set(), [], {}
    for probe in report["probes"]:
        stem = re.sub(r"\.m?js$", "", probe["name"])
        for divergence in probe["declaredDivergences"]:
            reported.add((stem, divergence["case"]))
        for case in probe.get("uncheckedDeclarations", []):
            reported.add((stem, case))
        for failure in probe["failures"]:
            if failure.startswith("stale divergence broiler-js/"):
                stale.append("%s: %s" % (probe["name"], failure))
            else:
                findings[probe["name"]] = findings.get(probe["name"], 0) + 1
    missing = sorted(declared - reported)
    text = "# broiler-js declarations in the answer files: %d\n" % len(declared)
    text += "# reported declared (or not checked under a declared run): %d\n" % len(declared & reported)
    text += "# declarations the run did not report: %d\n" % len(missing)
    text += "".join("  %s/%s\n" % item for item in missing)
    text += "# stale declarations: %d\n" % len(stale)
    text += "".join("  %s\n" % item for item in stale)
    text += "# findings outside the catalogue, by probe (not adjudicated): %d in %d probes\n" % (
        sum(findings.values()), len(findings))
    text += "".join("  %4d  %s\n" % (count, name) for name, count in sorted(findings.items()))
    return text, len(missing) + len(stale)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--broiler-js", required=True, help="the Broiler.JS apphost")
    parser.add_argument("--broiler-js-root", required=True, help="the Broiler.JS checkout it was built from")
    parser.add_argument("--node", required=True, help="Node, for the probes this change added or changed")
    arguments = parser.parse_args()
    failures = 0

    code, status = run(["git", "status", "--porcelain", "--", ".", ":!" + RECORD])
    if status.strip():
        print("collect: the tree outside this record is not clean; refusing to collect")
        return 2

    _, head = run(["git", "rev-parse", "HEAD"])
    _, pinned = run(["git", "-C", arguments.broiler_js_root, "rev-parse", "HEAD"])
    _, engine = run([arguments.node, "--version"])
    if not pinned.strip().startswith(PINNED):
        print("collect: the Broiler.JS checkout is at %s, not %s; refusing to collect" % (pinned.strip(), PINNED))
        return 2
    shutil.rmtree(SCRATCH, ignore_errors=True)
    os.makedirs(SCRATCH)

    command = ["dotnet", "build", "Broiler.VM.slnx", "-c", "Release", "-warnaserror"]
    code, text = run(command)
    write("build.log", "# collected at %s# Broiler.JS at %s# Node %s" % (head, pinned, engine)
          + logged(command, code, text[-600:]))
    failures += code != 0

    command = ["python3", "-m", "unittest", "discover", "-s", "eng/tests", "-p", "test_differential.py", "-v"]
    code, text = run(command)
    write("tooling.log", logged(command, code, text))
    failures += code != 0

    command = ["python3", "eng/run-differential.py", "--report", os.path.join(SCRATCH, "retained.json")]
    code, text = run(command)
    write("retained.log", logged(command, code, text))
    failures += code != 0

    # EVERY PROBE AGAINST BROILER.JS. The run itself fails, on findings outside the catalogue; what is judged is that
    # every declaration is reported and none is stale.
    against = ["--against", arguments.broiler_js, "--against-kind", "broiler-js",
               "--against-root", arguments.broiler_js_root, "--timeout", "30"]
    report = os.path.join(SCRATCH, "broiler-js.json")
    command = ["python3", "eng/run-differential.py"] + against + ["--report", report]
    code, text = run(command)
    write("broiler-js.log", logged(command, code, text))
    verdict, wrong = judge(report)
    write("catalogue.log", verdict)
    failures += wrong

    text = ""
    for stem in CHANGED:
        command = ["python3", "eng/run-differential.py", "--only", stem, "--against", arguments.node,
                   "--report", os.path.join(SCRATCH, "node-%s.json" % stem)]
        code, output = run(command)
        text += logged(command, code, output)
        failures += code != 0
    write("node.log", text)

    # ONE CONTROL PER CLAIM: applied, run, reverted.
    for name, stem, expected in CONTROLS:
        patch = os.path.join(HERE, "control-%s.patch" % name)
        recorded = RECORD + "/control-%s.patch" % name
        code, text = run(["git", "apply", patch])
        control = logged(["git", "apply", recorded], code, text)
        failures += code != 0

        if code == 0:
            command = ["python3", "eng/run-differential.py", "--only", stem] + against + [
                "--report", os.path.join(SCRATCH, "control-%s.json" % name)]
            code, text = run(command)
            control += logged(command, code, text)
            failed_as_expected = code != 0 and expected in text
            control += "# the run %s `%s`\n" % ("reported" if failed_as_expected else "DID NOT report", expected)
            failures += not failed_as_expected
            code, text = run(["git", "apply", "-R", patch])
            control += logged(["git", "apply", "-R", recorded], code, text)
            failures += code != 0

        write("control-%s.log" % name, control)

    # AFTER EVERY REVERT, the controlled probes are reported as declared again.
    text = ""
    for stem in sorted({stem for _, stem, _ in CONTROLS}):
        command = ["python3", "eng/run-differential.py", "--only", stem] + against + [
            "--report", os.path.join(SCRATCH, "after-%s.json" % stem)]
        code, output = run(command)
        text += logged(command, code, output)
        failures += code != 0
    write("after-controls.log", text)

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
