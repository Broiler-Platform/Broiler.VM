#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Collect Broiler.VM's evidence for milestone SJL-5 (the text area) as bundle sjl-5-001.

    python3 docs/evidence/sjl-5-001/collect.py --tree <a clean checkout> \
        [--suite <an unpacked checkout of the pinned test262 revision>] \
        --out docs/evidence/sjl-5-001 --only STEP [--only STEP ...]

THE BASE IS THIS FAMILY AFTER SJL-4. The rule (decision-rule.md beside this file) takes its base runs at
the commit that carries it, before any code of SJL-5 lands, from a clean checkout on an otherwise idle
machine, and retains them in base/. The base steps below write there and nowhere else.

IT REUSES BUNDLE SJL-0-001'S COLLECTOR for the test262 procedure, as SJL-3's and SJL-4's did, so the base
and the after run the same code; its twin bisection is SJL-4's, over the wide twins and the text probes
the rule adds (src/tests/forms/text); and its JSON module step runs part C's harness, json-modules.cs
beside this file, against the tree's own compiler.

THE COLLECTOR JUDGES NOTHING. Each step writes what it ran and what it printed, failures included; the
README reads them under the rule.

Steps, each with `--only`:

  base-identity      the base commit, its cleanliness, the rule's presence, the SDK and the machine, into
                     base/identity.txt;
  base-test262       the whole pinned suite in both forms, through sjl-0-001's procedure, at --tree, into
                     base/;
  base-twins         every *.small.js twin of src/tests/forms/wide and probe of src/tests/forms/text: its
                     smallest completing fuel allowance bisected under the bytecode form, then each form
                     run at that figure and at one less, with the answer it completes with, into
                     base/fuel-parity.log;
  base-json-modules  part C's harness, twice, over src/tests/forms/text/json-modules and the pinned
                     suite's test/ directory (--suite), into base/json-modules-1.txt and -2.txt.

The after steps (the after commit's runs, the twins at the base's figures, the comparisons, the re-runs,
the closures, the rules) are added at SJL-5.6, when there is an after commit to run them at.
"""

import argparse
import datetime
import importlib.util
import os
import platform
import stat
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
BASE_BUNDLE = os.path.join(os.path.dirname(HERE), "sjl-0-001")

_spec = importlib.util.spec_from_file_location("sjl_0_001_collect", os.path.join(BASE_BUNDLE, "collect.py"))
base = importlib.util.module_from_spec(_spec)
_spec.loader.exec_module(base)

RULE = os.path.join("docs", "evidence", "sjl-5-001", "decision-rule.md")
HARNESS = os.path.join("docs", "evidence", "sjl-5-001", "json-modules.cs")
# The default branch after SJL-4's merge, whose product sources the rule says the base commit carries.
SJL_4_MERGE = "1f253ff7a6ba67051854c52b6e9a68a3e1f65f31"
TWIN_FOLDERS = [os.path.join("src", "tests", "forms", "wide"), os.path.join("src", "tests", "forms", "text")]
JSON_MODULES = os.path.join("src", "tests", "forms", "text", "json-modules")


class Collector(base.Collector):
    def __init__(self, arguments):
        super().__init__(arguments)
        self.root_out = self.out
        self.out = os.path.join(self.root_out, "base")
        os.makedirs(self.out, exist_ok=True)

    def base_identity(self):
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        status = self.run(["git", "status", "--porcelain"])[1]
        rule = os.path.join(self.tree, RULE)
        code, names = self.run(["git", "diff", "--name-only", SJL_4_MERGE, commit])
        changed = [name for name in names.splitlines()
                   if name and not name.startswith("docs/") and "/docs/" not in name]
        lines = [
            "bundle sjl-5-001, its base",
            "milestone SJL-5, the text area; the base is this family after SJL-4",
            "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "base-commit " + commit,
            "base-tree " + ("clean" if not status.strip() else "dirty"),
            "rule %s %s" % (RULE.replace(os.sep, "/"),
                            ("sha256=" + self.sha256_lf(rule)) if os.path.isfile(rule) else "ABSENT"),
            "harness %s %s" % (HARNESS.replace(os.sep, "/"),
                               ("sha256=" + self.sha256_lf(os.path.join(self.tree, HARNESS)))
                               if os.path.isfile(os.path.join(self.tree, HARNESS)) else "ABSENT"),
            "sdk " + self.run(["dotnet", "--version"])[1].strip(),
            "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
            "suite-pin " + base.PIN + " sha256=" + self.sha256_lf(os.path.join(self.tree, base.PIN)),
            "allowance fuel=%d wallClockMs=%d" % (base.FUEL, base.WALL),
            "twin-allowance ceiling=%d wallClockMs=%d" % (base.TWIN_CEILING, base.TWIN_WALL),
            "jobs %d" % self.jobs,
            "native-backend " + base.NATIVE_BACKEND,
            "since-sjl-4-merge %s, outside the documents: %s" % (
                SJL_4_MERGE, ("git diff failed with exit %d" % code) if code != 0
                else (str(len(changed)) + " files") if changed else "none"),
        ]
        lines.extend("  " + name for name in changed)
        if status.strip():
            lines.append("dirty:")
            lines.extend("  " + line for line in status.splitlines())
        path = os.path.join(self.out, "identity.txt")
        if os.path.exists(path):
            os.chmod(path, stat.S_IREAD | stat.S_IWRITE)
        self.write("identity.txt", "\n".join(lines) + "\n")
        os.chmod(path, stat.S_IREAD | stat.S_IRGRP | stat.S_IROTH)
        print("base-identity: %s (%s tree)" % (commit, "clean" if not status.strip() else "DIRTY"), flush=True)
        return 0 if not status.strip() and os.path.isfile(rule) else 1

    def base_test262(self):
        return self.test262()

    def base_twins(self):
        binaries = self.build(base.CLI, base.CLI_BINARY)
        if binaries is None:
            print("base-twins: the CLI root did not build; see base/build-cli.log")
            return 1
        binary = os.path.join(binaries, base.CLI_BINARY)
        lines = ["# fuel-parity twins and probes at the base commit, " + ", ".join(
                     folder.replace(os.sep, "/") for folder in TWIN_FOLDERS),
                 "# twin | form | smallest completing allowance (bytecode, bisected) | at that figure | at one less | answer"]
        verdict = 0

        def attempt(form, path, fuel):
            command = [binary]
            if form == "native":
                command += ["--native", base.NATIVE_BACKEND]
            command += ["--fuel", str(fuel), "--wall", str(base.TWIN_WALL), path]
            return self.run(command)

        def last(output):
            return (output.strip().splitlines() or ["(no output)"])[-1]

        for folder in TWIN_FOLDERS:
            directory = os.path.join(self.tree, folder)
            for twin in sorted(name for name in os.listdir(directory) if name.endswith(".small.js")):
                path = os.path.join(directory, twin)
                label = "%s/%s" % (os.path.basename(folder), twin)
                code, output = attempt("bytecode", path, base.TWIN_CEILING)
                if code != 0:
                    lines.append("%s | bytecode | does not complete under %d | exit %d | - | %s" % (
                        label, base.TWIN_CEILING, code, last(output)))
                    verdict = 1
                    continue
                low, high = 1, base.TWIN_CEILING
                while low < high:
                    middle = (low + high) // 2
                    if attempt("bytecode", path, middle)[0] == 0:
                        high = middle
                    else:
                        low = middle + 1
                for form in ("bytecode", "native"):
                    done, answer = attempt(form, path, low)
                    refused, said = attempt(form, path, low - 1)
                    below = ("refused with %s" % base.EXHAUSTED_ON_FUEL
                             if refused != 0 and base.EXHAUSTED_ON_FUEL in said
                             else "exit %d: %s" % (refused, last(said)))
                    lines.append("%s | %s | %d | %s | %s | %s" % (
                        label, form, low, "completes" if done == 0 else "exit %d" % done, below,
                        last(answer) if done == 0 else "-"))
                    print("base-twins: %s %s %d" % (label, form, low), flush=True)
        self.write("fuel-parity.log", "\n".join(lines) + "\n")
        return verdict

    def base_json_modules(self):
        if not self.suite:
            print("base-json-modules: needs --suite")
            return 1
        suite_tests = os.path.join(self.suite, "test")
        verdict = 0
        for run in (1, 2):
            code, output = self.run(["dotnet", "run", "-c", "Release", os.path.join(self.tree, HARNESS), "--",
                                     os.path.join(self.tree, JSON_MODULES), suite_tests])
            self.write("json-modules-%d.txt" % run, output)
            print("base-json-modules: run %d exit %d" % (run, code), flush=True)
            verdict |= 0 if code == 0 else 1
        return verdict


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--tree", required=True, help="a clean checkout of the commit that carries the rule")
    parser.add_argument("--suite", help="an unpacked checkout of the pinned test262 revision")
    parser.add_argument("--out", required=True)
    parser.add_argument("--jobs", type=int, default=max(1, os.cpu_count() or 1))
    parser.add_argument("--only", action="append", required=True,
                        choices=("base-identity", "base-test262", "base-twins", "base-json-modules"))
    arguments = parser.parse_args()
    arguments.against_broiler_js = None
    arguments.against_root = None
    collector = Collector(arguments)
    verdict = 0
    for step in arguments.only:
        print("== %s %s" % (step, datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")),
              flush=True)
        verdict |= getattr(collector, step.replace("-", "_"))() or 0
    return verdict


if __name__ == "__main__":
    sys.exit(main())
