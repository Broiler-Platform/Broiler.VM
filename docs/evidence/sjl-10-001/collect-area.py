#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Collect Broiler.VM's evidence for milestone SJL-10 (the Temporal core) into bundle sjl-10-001.

    python3 docs/evidence/sjl-10-001/collect-area.py --tree <a clean checkout> --out docs/evidence/sjl-10-001 \
        [--artifact-zip <a run's test262-evidence artifact> --run-url <its workflow run, with its attempt>
         --artifact-id <its id> --run-times <when it was dispatched and completed>] --only STEP [--only STEP ...]

THE RULE IS decision-rule-area.md BESIDE THIS FILE, committed with the area verdict JSD-0078 before any code
of SJL-10. Its part A takes the base and after runs with .github/workflows/test262-evidence.yml, on GitHub's
runners, at the commit that carries the rule; its part B runs the slice compiler's checks over the Temporal
programs this family retains under src/tests/temporal/, and its part C bisects the wide twins, both locally,
from a clean checkout of the same commit.

IT REUSES THE BUNDLES BEFORE IT, so that each arm runs the code an earlier arm ran: bundle sjl-8-001's
collect-area.py, and through it bundle sjl-7-001's, sjl-6-001's (the import of a hosted run and its
completeness) and sjl-5-001's collector (the twin bisection), over the wide twins only: the rule adds no probe,
because the command-line root carries no Intl data and so reaches no Temporal code. Nothing of theirs is
changed; this file names this bundle's rule, its folders, its checks and its baseline commit. Part B's step is
bundle sjl-8-001's, over the six checks this rule names and the one folder it holds byte for byte.

THE COLLECTOR JUDGES NOTHING. Each step writes what it ran and what it printed, failures included; the
README reads them under the rule.

Steps, each with `--only`:

  base-identity  the base commit, its cleanliness, the rule's digest, every file outside the documents that
                 differs from the default branch after SJL-8 (047dcec), the SDK and the machine, into
                 base/identity.txt;
  base-import    the base dispatch's artifact test262-evidence, every file unchanged, into base/hosted/, with
                 base/hosted/imported.txt naming the run, the artifact and the digest of the zip as downloaded
                 (bundle sjl-6-001's step);
  base-complete  each hosted form's completeness, as the rule states it, read off base/hosted/, into
                 base/completeness.log (bundle sjl-6-001's step);
  base-twins     every *.small.js twin of src/tests/forms/wide: its smallest completing fuel allowance
                 bisected under the bytecode form, then each form run at that figure and at one less, with the
                 answer it completes with, into base/fuel-parity.log (bundle sjl-5-001's step);
  base-datasets  part B: the slice compiler built in Release at --tree and run with --checks --verbose, its
                 whole output into base/datasets/checks.log, the line of each of the six checks the rule names
                 (or its absence) into base/datasets/named-checks.txt, and the SHA-256 of every file under
                 src/tests/temporal/ into base/datasets/files.sha256 (bundle sjl-8-001's step);
  manifest       the SHA-256 of every file this collector writes or reads as its own (base/, cutover/, after/,
                 vectors/, this file, the rule and the correspondence), with the base commit base/identity.txt
                 names, into area-manifest.json. correspondence-runs/ is the correspondence's own record and is not
                 listed. cutover/ holds the checks SJL-10.4 ran at its cut-over, retained by hand (README section 4),
                 and is listed from SJL-10.4.

THE VECTORS STEP WAS ADDED AT SJL-10.2, when this family's pin of the shared corpus moved to revision 25, which
holds the Temporal area. It is bundle sjl-6-001's step, unchanged, as bundle sjl-8-001's collector added it:

  vectors        the conformance root over a retained pin of the shared corpus (--pin, by default the tree's own
                 src/tests/vectors/pins/vectors.pin), and its negative control, into
                 vectors/<label>-revision-<n>.* and vectors/<label>-negative-control-revision-<n>.*.

The after steps join this file when there is an after commit to run them at, SJL-10.4's cut-over, as bundle
sjl-8-001's did at SJL-8.7.
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


# Bundle sjl-8-001's area collector, which loads sjl-7-001's and through it sjl-6-001's and sjl-5-001's.
area8 = load("sjl_8_001_collect_area", os.path.join(EVIDENCE, "sjl-8-001", "collect-area.py"))
area7 = area8.area7
area6 = area8.area6
base = area8.base

RULE = os.path.join("docs", "evidence", "sjl-10-001", "decision-rule-area.md")
# The default branch after SJL-8's merges, whose product sources the rule says the base commit carries.
SJL_8_MERGE = "047dcec1efe21faf516d8acb207fe2adf180dbf7"
TWIN_FOLDERS = [os.path.join("src", "tests", "forms", "wide")]
# Part B's checks, as the rule names them, and the folder whose files it holds byte for byte.
DATASET_CHECKS = (
    "temporal/t1/temporal-matches-the-reference-polyfill",
    "temporal/t2/intl-over-temporal-matches-the-reference-polyfill",
    "temporal/t3/calendars-match-the-reference-polyfill",
    "temporal/t3/calendar-conversions-match-icu4x",
    "temporal/t4/calendar-formats-match-the-reference-polyfill",
    "temporal/t5/zone-names-match-the-reference-polyfill",
)
DATASET_FOLDERS = (os.path.join("src", "tests", "temporal"),)

STEPS = ("base-identity", "base-import", "base-complete", "base-twins", "base-datasets", "vectors", "manifest")


class Collector(area8.Collector):
    def base_identity(self):
        self.into("base")
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        status = self.run(["git", "status", "--porcelain"])[1]
        rule = os.path.join(self.tree, RULE)
        code, names = self.run(["git", "diff", "--name-only", SJL_8_MERGE, commit])
        changed = area7.outside_documents(names)
        lines = [
            "bundle sjl-10-001, the Temporal core's base",
            "milestone SJL-10, the Temporal core; the base is this family after SJL-8",
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
            "since-sjl-8-merge %s, outside the documents: %s" % (
                SJL_8_MERGE, ("git diff failed with exit %d" % code) if code != 0
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
        # Bundle sjl-8-001's step reads its module's DATASET_CHECKS and DATASET_FOLDERS; this rule names six
        # checks, without that bundle's tzdb check, and holds src/tests/temporal/ alone.
        area8.DATASET_CHECKS = DATASET_CHECKS
        area8.DATASET_FOLDERS = DATASET_FOLDERS
        return area8.Collector.datasets(self, folder)

    def manifest(self):
        files = {}
        for folder in ("base", "cutover", "after", "vectors"):
            for root, _, names in os.walk(os.path.join(self.root_out, folder)):
                for name in names:
                    path = os.path.join(root, name)
                    if "__pycache__" in path:
                        continue
                    files[os.path.relpath(path, self.root_out).replace(os.sep, "/")] = self.sha256(path)
        for name in ("collect-area.py", "decision-rule-area.md", "correspondence.md"):
            if os.path.isfile(os.path.join(self.root_out, name)):
                files[name] = self.sha256(os.path.join(self.root_out, name))
        document = {"bundle": "sjl-10-001", "repository": "Broiler.VM",
                    "part": "the Temporal core (SJL-10.1 onwards)",
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
    parser.add_argument("--tree", required=True, help="a clean checkout of the base commit")
    parser.add_argument("--suite", help="an unpacked checkout of the pinned test262 revision")
    parser.add_argument("--out", required=True)
    parser.add_argument("--jobs", type=int, default=max(1, os.cpu_count() or 1))
    parser.add_argument("--artifact-zip")
    parser.add_argument("--run-url")
    parser.add_argument("--artifact-id")
    parser.add_argument("--run-times")
    parser.add_argument("--pin", help="vectors: a retained suite pin of the shared corpus, by default the tree's")
    parser.add_argument("--label", help="vectors: the run's name in its files, by default 'run'")
    parser.add_argument("--only", action="append", required=True, choices=STEPS)
    arguments = parser.parse_args()
    # The earlier collectors' constructors read these; no base step uses them.
    arguments.base_tree = None
    arguments.images = None
    arguments.against_broiler_js = None
    arguments.against_root = None
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
