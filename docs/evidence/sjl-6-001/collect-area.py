#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Collect Broiler.VM's evidence for milestone SJL-6 (the Date area) into bundle sjl-6-001.

    python3 docs/evidence/sjl-6-001/collect-area.py --tree <a clean checkout> --out docs/evidence/sjl-6-001 \
        [--artifact-zip <a run's test262-evidence artifact> --run-url <its workflow run, with its attempt>
         --artifact-id <its id> --run-times <when it was dispatched and completed>] \
        [--pin <a retained suite pin of the vectors> --label <the run's name>] --only STEP [--only STEP ...]

THE RULE IS decision-rule-area.md BESIDE THIS FILE, committed with the area verdict JSD-0072 before any code
of SJL-6. Its part A takes the base and after runs with .github/workflows/test262-evidence.yml, on GitHub's
runners, at the commit that carries the rule; its part B takes the fuel-parity base locally, from a clean
checkout of the same commit. This collector writes the base into base/ and the engine-level vector runs into
vectors/. It does not touch hosted/, completeness.log, compare-*.log, identity.txt or manifest.json, which are
SJL-6.0's and were written by collect.py beside it.

IT REUSES THE BUNDLES BEFORE IT, so that each arm runs the code an earlier arm ran: collect.py beside this file
(SJL-6.0's) for the import of a hosted run and its completeness, and bundle sjl-5-001's collector, and through
it sjl-0-001's, for the twin bisection, over the wide twins and the Date probes the rule adds
(src/tests/forms/date).

THE COLLECTOR JUDGES NOTHING. Each step writes what it ran and what it printed, failures included; the README
reads them under the rule.

Steps, each with `--only`:

  base-identity  the base commit, its cleanliness, the rule's digest, every file outside the documents that
                 differs from the default branch after SJL-5 (72ad941), the SDK and the machine, into
                 base/identity.txt;
  base-import    the base dispatch's artifact test262-evidence, every file unchanged, into base/hosted/, with
                 base/hosted/imported.txt naming the run, the artifact and the digest of the zip as downloaded;
  base-complete  each hosted form's completeness, as the rule states it, read off base/hosted/, into
                 base/completeness.log;
  base-twins     every *.small.js twin of src/tests/forms/wide and probe of src/tests/forms/date: its smallest
                 completing fuel allowance bisected under the bytecode form, then each form run at that figure
                 and at one less, with the answer it completes with, into base/fuel-parity.log;
  vectors        the conformance root over a retained pin of the shared corpus (--pin, by default the tree's
                 own src/tests/vectors/pins/vectors.pin), and its negative control, into
                 vectors/<label>-revision-<n>.* and vectors/<label>-negative-control-revision-<n>.*;
  manifest       the SHA-256 of every file this collector writes or reads as its own (base/, vectors/, this
                 file, the rule and the correspondence), with the base commit base/identity.txt names, into
                 area-manifest.json.
"""

import argparse
import datetime
import gzip
import hashlib
import importlib.util
import json
import os
import platform
import re
import shutil
import stat
import sys
import tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
EVIDENCE = os.path.dirname(HERE)


def load(name, path):
    spec = importlib.util.spec_from_file_location(name, path)
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


# SJL-6.0's collector beside this file: the import of a hosted run and its completeness.
hosted = load("sjl_6_001_collect", os.path.join(HERE, "collect.py"))
# Bundle sjl-5-001's collector, whose twin bisection SJL-6's part B reuses; it loads sjl-4-001's, sjl-3-001's
# and sjl-0-001's itself.
text = load("sjl_5_001_collect", os.path.join(EVIDENCE, "sjl-5-001", "collect.py"))
base = text.base

RULE = os.path.join("docs", "evidence", "sjl-6-001", "decision-rule-area.md")
# The default branch after SJL-5's merge, whose product sources the rule says the base commit carries.
SJL_5_MERGE = "72ad941e1c3248780af8231e548c3656b0984f99"
TWIN_FOLDERS = [os.path.join("src", "tests", "forms", "wide"), os.path.join("src", "tests", "forms", "date")]
VECTORS_PIN = os.path.join("src", "tests", "vectors", "pins", "vectors.pin")
# The control vector every run of this family's vectors mutates, as SJL-5's runs did.
VECTORS_CONTROL = "text/trim/0001"
STEPS = ("base-identity", "base-import", "base-complete", "base-twins", "vectors", "manifest")


class Collector(text.Collector):
    def base_identity(self):
        self.into("base")
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        status = self.run(["git", "status", "--porcelain"])[1]
        rule = os.path.join(self.tree, RULE)
        code, names = self.run(["git", "diff", "--name-only", SJL_5_MERGE, commit])
        changed = [name for name in names.splitlines()
                   if name and not name.startswith("docs/") and "/docs/" not in name]
        lines = [
            "bundle sjl-6-001, the Date area's base",
            "milestone SJL-6, the Date area; the base is this family after SJL-5",
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
            "since-sjl-5-merge %s, outside the documents: %s" % (
                SJL_5_MERGE, ("git diff failed with exit %d" % code) if code != 0
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

    def hosted_collector(self):
        """SJL-6.0's collector, pointed at base/: it reads and writes <out>/hosted/ and <out>/completeness.log."""
        arguments = argparse.Namespace(tree=self.tree, suite=self.suite, out=os.path.join(self.root_out, "base"),
                                       artifact_zip=self.arguments.artifact_zip, run_url=self.arguments.run_url,
                                       artifact_id=self.arguments.artifact_id, run_times=self.arguments.run_times)
        return hosted.Collector(arguments)

    def base_import(self):
        self.into("base")
        return self.hosted_collector().import_()

    def base_complete(self):
        self.into("base")
        return self.hosted_collector().complete()

    def base_twins(self):
        # Bundle sjl-5-001's bisection reads its module's TWIN_FOLDERS; this area's are the wide twins and
        # the Date probes.
        text.TWIN_FOLDERS = TWIN_FOLDERS
        return super().base_twins()

    def vectors(self):
        self.into("vectors")
        pin = os.path.abspath(self.arguments.pin) if self.arguments.pin else os.path.join(self.tree, VECTORS_PIN)
        content = open(pin, encoding="utf-8").read()
        revision = re.search(r"is at corpus revision (\d+)", content).group(1)
        archive = re.search(r"^archived-at (\S+)$", content, re.M).group(1)
        binaries = self.build(base.CONFORMANCE, base.CONFORMANCE_BINARY)
        if binaries is None:
            print("vectors: the conformance root did not build; see vectors/build-conformance.log")
            return 1
        binary = os.path.join(binaries, base.CONFORMANCE_BINARY)
        label = self.arguments.label or "run"
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        status = self.run(["git", "status", "--porcelain"])[1]
        header = "== %s at %s (%s tree), pin %s sha256=%s, %s\n" % (
            label, commit, "clean" if not status.strip() else "dirty: " + " ".join(
                line[3:] for line in status.splitlines()),
            os.path.relpath(pin, self.tree).replace(os.sep, "/") if pin.startswith(self.tree) else os.path.basename(pin),
            self.sha256_lf(pin), datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"))
        verdict = 0
        for name, extra, wanted in (("%s-revision-%s" % (label, revision), [], 0),
                                    ("%s-negative-control-revision-%s" % (label, revision),
                                     ["--mutate", VECTORS_CONTROL], 1)):
            report = os.path.join(self.scratch, name + ".tsv")
            code, output = self.run([binary, "--vectors", archive, "--expect", pin, "--report", report] + extra)
            self.write(name + ".log", header + output.strip() + "\nexit code: %d\n" % code)
            if os.path.isfile(report):
                with open(report, "rb") as source, gzip.GzipFile(
                        os.path.join(self.out, name + ".tsv.gz"), "wb", compresslevel=9, mtime=0) as target:
                    shutil.copyfileobj(source, target)
            print("vectors: %s exit %d" % (name, code), flush=True)
            verdict |= 0 if code == wanted else 1
        return verdict

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
        document = {"bundle": "sjl-6-001", "repository": "Broiler.VM", "part": "the Date area (SJL-6.2 onwards)",
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
    parser.add_argument("--pin", help="vectors: a retained suite pin of the shared corpus, by default the tree's")
    parser.add_argument("--label", help="vectors: the run's name in its files, by default 'run'")
    parser.add_argument("--only", action="append", required=True, choices=STEPS)
    arguments = parser.parse_args()
    arguments.base_tree = None
    arguments.images = None
    arguments.against_broiler_js = None
    arguments.against_root = None
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
