#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Collect Broiler.VM's evidence for milestone SJL-7 (the Unicode area) into bundle sjl-7-001.

    python3 docs/evidence/sjl-7-001/collect-area.py --tree <a clean checkout> --out docs/evidence/sjl-7-001 \
        [--artifact-zip <a run's test262-evidence artifact> --run-url <its workflow run, with its attempt>
         --artifact-id <its id> --run-times <when it was dispatched and completed>] --only STEP [--only STEP ...]

THE RULE IS decision-rule-area.md BESIDE THIS FILE, committed with the area verdict JSD-0074 before any code
of SJL-7. Its part A takes the base and after runs with .github/workflows/test262-evidence.yml, on GitHub's
runners, at the commit that carries the rule; its part B takes the fuel-parity base locally, from a clean
checkout of the same commit; its part C is a measurement whose base arm is built from that commit when the
after arm exists, at SJL-7.6, and is not collected here.

IT REUSES THE BUNDLES BEFORE IT, so that each arm runs the code an earlier arm ran: bundle sjl-6-001's
collect-area.py, and through it that bundle's collect.py (the import of a hosted run and its completeness)
and bundle sjl-5-001's collector (the twin bisection), over the wide twins and the Unicode probes the rule
adds (src/tests/forms/unicode). Nothing of theirs is changed; this file names this bundle's rule, its folders
and its baseline commit.

THE COLLECTOR JUDGES NOTHING. Each step writes what it ran and what it printed, failures included; the README
reads them under the rule.

Steps, each with `--only`:

  base-identity  the base commit, its cleanliness, the rule's digest, every file outside the documents that
                 differs from the default branch after SJL-6 (24632b1), the SDK and the machine, into
                 base/identity.txt;
  base-import    the base dispatch's artifact test262-evidence, every file unchanged, into base/hosted/, with
                 base/hosted/imported.txt naming the run, the artifact and the digest of the zip as downloaded;
  base-complete  each hosted form's completeness, as the rule states it, read off base/hosted/, into
                 base/completeness.log;
  base-twins     every *.small.js twin of src/tests/forms/wide and probe of src/tests/forms/unicode: its
                 smallest completing fuel allowance bisected under the bytecode form, then each form run at
                 that figure and at one less, with the answer it completes with, into base/fuel-parity.log;
  manifest       the SHA-256 of every file this collector writes or reads as its own (base/, this file, the
                 rule and the correspondence), and of the engine-level vector runs retained beside them
                 (vectors/), with the base commit base/identity.txt names, into area-manifest.json.

The after steps join this file at SJL-7.6, when there is an after commit to run them at, as bundle sjl-6-001's
did at SJL-6.6.
"""

import argparse
import datetime
import importlib.util
import json
import os
import platform
import shutil
import stat
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
EVIDENCE = os.path.dirname(HERE)


def load(name, path):
    spec = importlib.util.spec_from_file_location(name, path)
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


# Bundle sjl-6-001's area collector, which loads that bundle's collect.py and bundle sjl-5-001's collector.
area6 = load("sjl_6_001_collect_area", os.path.join(EVIDENCE, "sjl-6-001", "collect-area.py"))
base = area6.base

RULE = os.path.join("docs", "evidence", "sjl-7-001", "decision-rule-area.md")
# The default branch after SJL-6's merges, whose product sources the rule says the base commit carries.
SJL_6_MERGE = "24632b16b570a85163599e6daf45bea1c63c81d3"
TWIN_FOLDERS = [os.path.join("src", "tests", "forms", "wide"), os.path.join("src", "tests", "forms", "unicode")]
STEPS = ("base-identity", "base-import", "base-complete", "base-twins", "manifest")


class Collector(area6.Collector):
    def base_identity(self):
        self.into("base")
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        status = self.run(["git", "status", "--porcelain"])[1]
        rule = os.path.join(self.tree, RULE)
        code, names = self.run(["git", "diff", "--name-only", SJL_6_MERGE, commit])
        changed = [name for name in names.splitlines()
                   if name and not name.startswith("docs/") and "/docs/" not in name]
        lines = [
            "bundle sjl-7-001, the Unicode area's base",
            "milestone SJL-7, the Unicode area; the base is this family after SJL-6",
            "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "base-commit " + commit,
            "base-tree " + ("clean" if not status.strip() else "dirty"),
            "rule %s %s" % (RULE.replace(os.sep, "/"),
                            ("sha256=" + self.sha256_lf(rule)) if os.path.isfile(rule) else "ABSENT"),
            "sdk " + self.run(["dotnet", "--version"])[1].strip(),
            "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
            "twin-allowance ceiling=%d wallClockMs=%d" % (base.TWIN_CEILING, base.TWIN_WALL),
            "native-backend " + base.NATIVE_BACKEND,
            "twin-folders " + ", ".join(folder.replace(os.sep, "/") for folder in TWIN_FOLDERS),
            "since-sjl-6-merge %s, outside the documents: %s" % (
                SJL_6_MERGE, ("git diff failed with exit %d" % code) if code != 0
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

    def base_twins(self):
        # Bundle sjl-5-001's bisection reads its module's TWIN_FOLDERS; this area's are the wide twins and
        # the Unicode probes.
        area6.text.TWIN_FOLDERS = TWIN_FOLDERS
        return area6.text.Collector.base_twins(self)

    def manifest(self):
        files = {}
        for folder in ("base", "vectors"):
            for root, _, names in os.walk(os.path.join(self.root_out, folder)):
                for name in names:
                    path = os.path.join(root, name)
                    files[os.path.relpath(path, self.root_out).replace(os.sep, "/")] = self.sha256(path)
        for name in ("collect-area.py", "decision-rule-area.md", "correspondence.md"):
            files[name] = self.sha256(os.path.join(self.root_out, name))
        identity = os.path.join(self.root_out, "base", "identity.txt")
        base_commit = next((line.split(" ", 1)[1].strip() for line in open(identity, encoding="utf-8")
                            if line.startswith("base-commit ")), "(absent)") if os.path.isfile(identity) else "(absent)"
        document = {"bundle": "sjl-7-001", "repository": "Broiler.VM", "part": "the Unicode area (SJL-7.2 onwards)",
                    "baseCommit": base_commit, "files": dict(sorted(files.items()))}
        with open(os.path.join(self.root_out, "area-manifest.json"), "w", encoding="utf-8", newline="\n") as stream:
            stream.write(json.dumps(document, indent=1) + "\n")
        print("manifest: %d files" % len(files), flush=True)
        return 0


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--tree", required=True, help="a clean checkout of the base commit")
    parser.add_argument("--suite", help="an unpacked checkout of the pinned test262 revision")
    parser.add_argument("--out", required=True)
    parser.add_argument("--jobs", type=int, default=max(1, os.cpu_count() or 1))
    parser.add_argument("--artifact-zip")
    parser.add_argument("--run-url")
    parser.add_argument("--artifact-id")
    parser.add_argument("--run-times")
    parser.add_argument("--only", action="append", required=True, choices=STEPS)
    arguments = parser.parse_args()
    arguments.against_broiler_js = None
    arguments.against_root = None
    arguments.base_tree = None
    arguments.images = None
    arguments.pin = None
    arguments.label = None
    collector = Collector(arguments)
    collector.arguments = arguments
    verdict = 0
    for step in arguments.only:
        print("== %s %s" % (step, datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")),
              flush=True)
        verdict |= getattr(collector, step.replace("-", "_"))() or 0
    shutil.rmtree(collector.scratch, ignore_errors=True)
    return verdict


if __name__ == "__main__":
    sys.exit(main())
