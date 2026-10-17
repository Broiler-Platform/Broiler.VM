#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Collect Broiler.VM's evidence for milestone SJL-8 (the time-zone and calendar area) into bundle sjl-8-001.

    python3 docs/evidence/sjl-8-001/collect-area.py --tree <a clean checkout> --out docs/evidence/sjl-8-001 \
        [--artifact-zip <a run's test262-evidence artifact> --run-url <its workflow run, with its attempt>
         --artifact-id <its id> --run-times <when it was dispatched and completed>] --only STEP [--only STEP ...]

THE RULE IS decision-rule-area.md BESIDE THIS FILE, committed with the area verdict JSD-0076 before any code
of SJL-8. Its part A takes the base and after runs with .github/workflows/test262-evidence.yml, on GitHub's
runners, at the commit that carries the rule; its part B runs the slice compiler's checks over the reference
datasets this family retains, and its part C bisects the wide twins, both locally, from a clean checkout of
the same commit.

IT REUSES THE BUNDLES BEFORE IT, so that each arm runs the code an earlier arm ran: bundle sjl-7-001's
collect-area.py, and through it bundle sjl-6-001's (the import of a hosted run and its completeness) and
bundle sjl-5-001's collector (the twin bisection), over the wide twins only: the rule adds no probe, because
the command-line root carries no Intl data. Nothing of theirs is changed; this file names this bundle's rule,
its folders and its baseline commit, and adds part B's step.

THE COLLECTOR JUDGES NOTHING. Each step writes what it ran and what it printed, failures included; the
README reads them under the rule.

Steps, each with `--only`:

  base-identity  the base commit, its cleanliness, the rule's digest, every file outside the documents that
                 differs from the default branch after SJL-7 (9508bd5), the SDK and the machine, into
                 base/identity.txt;
  base-import    the base dispatch's artifact test262-evidence, every file unchanged, into base/hosted/, with
                 base/hosted/imported.txt naming the run, the artifact and the digest of the zip as downloaded;
  base-complete  each hosted form's completeness, as the rule states it, read off base/hosted/, into
                 base/completeness.log;
  base-twins     every *.small.js twin of src/tests/forms/wide: its smallest completing fuel allowance
                 bisected under the bytecode form, then each form run at that figure and at one less, with the
                 answer it completes with, into base/fuel-parity.log;
  base-datasets  part B: the slice compiler built in Release at --tree and run with --checks --verbose, its
                 whole output into base/datasets/checks.log, the line of each check the rule names (or its
                 absence) into base/datasets/named-checks.txt, and the SHA-256 of every file under
                 src/tests/cldr/timezones/ and src/tests/temporal/ into base/datasets/files.sha256;
  manifest       the SHA-256 of every file this collector writes or reads as its own (base/, after/, this file,
                 the rule and the correspondence), with the base commit base/identity.txt names, into
                 area-manifest.json. correspondence-runs/ is the correspondence's own record and is not listed.
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


# Bundle sjl-7-001's area collector, which loads sjl-6-001's and through it sjl-5-001's.
area7 = load("sjl_7_001_collect_area", os.path.join(EVIDENCE, "sjl-7-001", "collect-area.py"))
area6 = area7.area6
base = area7.base

RULE = os.path.join("docs", "evidence", "sjl-8-001", "decision-rule-area.md")
# The default branch after SJL-7's merges, whose product sources the rule says the base commit carries.
SJL_7_MERGE = "9508bd5b9b6d0f6d009418dda351fb0e7d846745"
TWIN_FOLDERS = [os.path.join("src", "tests", "forms", "wide")]
SLICE_COMPILER = os.path.join("src", "compositions", "Broiler.VM.Composition.JavaScript.SliceCompiler")
SLICE_COMPILER_BINARY = "Broiler.VM.Composition.JavaScript.SliceCompiler"
# Part B's checks, as the rule names them, and the folders whose files it holds byte for byte.
DATASET_CHECKS = (
    "tzdb/time-zone-offsets-match-icu",
    "temporal/t1/temporal-matches-the-reference-polyfill",
    "temporal/t2/intl-over-temporal-matches-the-reference-polyfill",
    "temporal/t3/calendars-match-the-reference-polyfill",
    "temporal/t3/calendar-conversions-match-icu4x",
    "temporal/t4/calendar-formats-match-the-reference-polyfill",
    "temporal/t5/zone-names-match-the-reference-polyfill",
)
DATASET_FOLDERS = (os.path.join("src", "tests", "cldr", "timezones"), os.path.join("src", "tests", "temporal"))

STEPS = ("base-identity", "base-import", "base-complete", "base-twins", "base-datasets", "manifest")


class Collector(area7.Collector):
    def base_identity(self):
        self.into("base")
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        status = self.run(["git", "status", "--porcelain"])[1]
        rule = os.path.join(self.tree, RULE)
        code, names = self.run(["git", "diff", "--name-only", SJL_7_MERGE, commit])
        changed = area7.outside_documents(names)
        lines = [
            "bundle sjl-8-001, the time-zone and calendar area's base",
            "milestone SJL-8, the time-zone and calendar area; the base is this family after SJL-7",
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
            "dataset-folders " + ", ".join(folder.replace(os.sep, "/") for folder in DATASET_FOLDERS),
            "since-sjl-7-merge %s, outside the documents: %s" % (
                SJL_7_MERGE, ("git diff failed with exit %d" % code) if code != 0
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
        return 0 if not status.strip() and os.path.isfile(rule) and code == 0 and not changed else 1

    def base_twins(self):
        # Bundle sjl-5-001's bisection reads its module's TWIN_FOLDERS; this area's are the wide twins alone.
        area6.text.TWIN_FOLDERS = TWIN_FOLDERS
        return area6.text.Collector.base_twins(self)

    def datasets(self, folder):
        """Part B at --tree, into <folder>/datasets/."""
        self.into(os.path.join(folder, "datasets"))
        binaries = self.build(SLICE_COMPILER, SLICE_COMPILER_BINARY)
        if binaries is None:
            print("%s-datasets: the slice compiler did not build; see %s/datasets/build-slicecompiler.log"
                  % (folder, folder), flush=True)
            return 1
        binary = os.path.join(binaries, SLICE_COMPILER_BINARY)
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        started = datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")
        code, output = self.run([binary, "--checks", "--verbose"])
        self.write("checks.log", "# %s.dll sha256=%s, at %s, started %s\n# --checks --verbose\n%s\nexit code: %d\n" % (
            SLICE_COMPILER_BINARY, self.sha256(binary + ".dll"), commit, started, output.rstrip(), code))
        lines = ["# part B's checks at %s, as checks.log beside this file prints them" % commit]
        verdict = 0
        for name in DATASET_CHECKS:
            found = [line for line in output.splitlines() if line[4:].startswith(" " + name + ":")]
            if len(found) != 1:
                lines.append("ABSENT %s: %d lines name it" % (name, len(found)))
                verdict = 1
                continue
            lines.append(found[0])
            verdict |= 0 if found[0].startswith("ok  ") else 1
        lines.append("slice-compiler exit code %d" % code)
        self.write("named-checks.txt", "\n".join(lines) + "\n")
        digests = []
        for dataset in DATASET_FOLDERS:
            for root, _, names in os.walk(os.path.join(self.tree, dataset)):
                for name in names:
                    path = os.path.join(root, name)
                    digests.append("%s  %s" % (self.sha256(path),
                                               os.path.relpath(path, self.tree).replace(os.sep, "/")))
        self.write("files.sha256", "\n".join(sorted(digests, key=lambda line: line[66:])) + "\n")
        print("%s-datasets: %d of %d named checks pass, %d dataset files, slice compiler exit %d"
              % (folder, sum(1 for line in lines if line.startswith("ok  ")), len(DATASET_CHECKS), len(digests),
                 code), flush=True)
        return verdict

    def base_datasets(self):
        return self.datasets("base")

    def manifest(self):
        files = {}
        for folder in ("base", "after"):
            for root, _, names in os.walk(os.path.join(self.root_out, folder)):
                for name in names:
                    path = os.path.join(root, name)
                    if "__pycache__" in path:
                        continue
                    files[os.path.relpath(path, self.root_out).replace(os.sep, "/")] = self.sha256(path)
        for name in ("collect-area.py", "decision-rule-area.md", "correspondence.md"):
            if os.path.isfile(os.path.join(self.root_out, name)):
                files[name] = self.sha256(os.path.join(self.root_out, name))
        document = {"bundle": "sjl-8-001", "repository": "Broiler.VM",
                    "part": "the time-zone and calendar area (SJL-8.1 onwards)",
                    "baseCommit": area7.recorded(os.path.join(self.root_out, "base", "identity.txt"), "base-commit")
                    or "(absent)",
                    "afterCommit": area7.recorded(os.path.join(self.root_out, "after", "identity.txt"), "after-commit")
                    or "(absent)",
                    "notListed": "correspondence-runs/, the correspondence's own record of its probes and both"
                                 " engines' answers",
                    "files": dict(sorted(files.items()))}
        with open(os.path.join(self.root_out, "area-manifest.json"), "w", encoding="utf-8", newline="\n") as stream:
            stream.write(json.dumps(document, indent=1) + "\n")
        print("manifest: %d files" % len(files), flush=True)
        return 0


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--tree", required=True, help="a clean checkout: the base commit, or the after commit")
    parser.add_argument("--base-tree", help="a clean checkout of the base commit")
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
    arguments.pin = None
    arguments.label = None
    arguments.images = None
    arguments.dry_run = False
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
