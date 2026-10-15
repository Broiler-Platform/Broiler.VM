#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Collect bundle sjl-6-001: the hosted test262 workflow's first run, against the local procedure's.

    python3 docs/evidence/sjl-6-001/collect.py --tree <a clean checkout> --out docs/evidence/sjl-6-001 \
        [--artifact-zip <the run's test262-evidence artifact> --run-url <its workflow run, with its attempt>
         --artifact-id <its id> --run-times <when it was dispatched and completed>] --only STEP [STEP ...]

THE RULE IS decision-rule.md BESIDE THIS FILE, committed before the run it judges. The hosted run is
.github/workflows/test262-evidence.yml's, dispatched once at bundle sjl-5-001's after commit with both
forms; the local runs are that bundle's own, taken at the same commit by bundle sjl-0-001's procedure
and retained in docs/evidence/sjl-5-001/after/.

THE COLLECTOR JUDGES NOTHING. Each step writes what it ran and what it printed; the README reads them
under the rule.

Steps, each with `--only`:

  identity  the checkout this collector ran from, its cleanliness, the rule's digest, and the SDK and
            machine, into identity.txt;
  import    the workflow's artifact, every file unchanged, into hosted/, with hosted/imported.txt naming
            the run, the artifact and the digest of the zip as downloaded;
  complete  each hosted form's completeness, as the rule states it, read off the files the artifact
            holds, into completeness.log;
  compare   each form's local report against the hosted one, through the tree's
            eng/compare-test262-forms.py --same-form, the native pair with --exempt-guest-loads and the
            extracted suite (--suite), into compare-<run>.log;
  manifest  the SHA-256 of every file the bundle retains, into manifest.json.

Paths in retained logs are written as <tree>, <suite> and <scratch>.
"""

import argparse
import datetime
import gzip
import hashlib
import json
import os
import platform
import shutil
import subprocess
import sys
import tempfile
import zipfile

HERE = os.path.dirname(os.path.abspath(__file__))
RULE = "docs/evidence/sjl-6-001/decision-rule.md"
LOCAL = os.path.join("docs", "evidence", "sjl-5-001", "after")
TESTED = "72ad941e1c3248780af8231e548c3656b0984f99"
RUNS = [("wide-bytecode", []), ("wide-native", ["--exempt-guest-loads"])]
STEPS = ("identity", "import", "complete", "compare", "manifest")


class Collector:
    def __init__(self, arguments):
        self.tree = os.path.abspath(arguments.tree)
        self.suite = os.path.abspath(arguments.suite) if arguments.suite else None
        self.out = os.path.abspath(arguments.out)
        self.arguments = arguments
        self.scratch = tempfile.mkdtemp(prefix="sjl-6-001-")

    def hide(self, text):
        for path, name in ((self.scratch, "<scratch>"), (self.suite, "<suite>"), (self.tree, "<tree>")):
            if path:
                text = text.replace(path, name)
        return text

    def run(self, command):
        result = subprocess.run(command, cwd=self.tree, capture_output=True, encoding="utf-8", errors="replace")
        return result.returncode, (result.stdout or "") + (result.stderr or "")

    def write(self, name, text):
        path = os.path.join(self.out, name)
        os.makedirs(os.path.dirname(path), exist_ok=True)
        with open(path, "w", encoding="utf-8", newline="\n") as stream:
            stream.write(self.hide(text))

    @staticmethod
    def sha256(path):
        digest = hashlib.sha256()
        with open(path, "rb") as stream:
            for block in iter(lambda: stream.read(1 << 20), b""):
                digest.update(block)
        return digest.hexdigest()

    def identity(self):
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        status = self.run(["git", "status", "--porcelain"])[1]
        rule = os.path.join(self.tree, RULE)
        with open(rule, "rb") as stream:
            rule_digest = hashlib.sha256(stream.read().replace(b"\r\n", b"\n")).hexdigest()
        lines = [
            "bundle sjl-6-001",
            "milestone SJL-6, work package SJL-6.0",
            "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "collector-commit " + commit,
            "collector-tree " + ("clean" if not status.strip() else "dirty"),
            "rule %s sha256=%s" % (RULE, rule_digest),
            "tested-commit " + TESTED + " (bundle sjl-5-001's after commit)",
            "local-runs docs/evidence/sjl-5-001/after/wide-bytecode.report.gz, wide-native.report.gz",
            "sdk " + self.run(["dotnet", "--version"])[1].strip(),
            "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
        ]
        self.write("identity.txt", "\n".join(lines) + "\n")
        print("identity: %s (%s tree)" % (commit, "clean" if not status.strip() else "DIRTY"))
        return 0 if not status.strip() else 1

    def import_(self):
        arguments = self.arguments
        if not (arguments.artifact_zip and arguments.run_url and arguments.artifact_id and arguments.run_times):
            print("import: pass --artifact-zip, --run-url, --artifact-id and --run-times")
            return 1
        hosted = os.path.join(self.out, "hosted")
        if os.path.exists(hosted):
            print("import: %s exists; the import writes it once" % hosted)
            return 1
        os.makedirs(hosted)
        with zipfile.ZipFile(arguments.artifact_zip) as archive:
            names = sorted(archive.namelist())
            archive.extractall(hosted)
        size = os.path.getsize(arguments.artifact_zip)
        lines = [
            "# imported: the hosted run was taken by .github/workflows/test262-evidence.yml",
            "workflow run: %s, %s" % (arguments.run_url, arguments.run_times),
            "artifact: test262-evidence, id %s, %s bytes, sha256 %s (the zip as downloaded)" % (
                arguments.artifact_id, format(size, ","), self.sha256(arguments.artifact_zip)),
            "retained: every file of the artifact, unchanged, in this directory (%d entries); this file is the "
            "only one added" % len([name for name in names if not name.endswith("/")]),
        ]
        with open(os.path.join(hosted, "imported.txt"), "w", encoding="utf-8", newline="\n") as stream:
            stream.write("\n".join(lines) + "\n")
        print("import: %d entries" % len(names))
        return 0

    def complete(self):
        hosted = os.path.join(self.out, "hosted")
        lines = ["# each hosted form's completeness, as decision-rule.md states it, read off the artifact",
                 "# run | run-test262 exit code | coverage whole | retention retainable | shard findings | "
                 "distinct assembly lists | complete"]
        verdict = 0
        for name, _ in RUNS:
            exit_code = "(absent)"
            path = os.path.join(hosted, "%s.run.log" % name)
            if os.path.isfile(path):
                last = open(path, encoding="utf-8").read().strip().splitlines()[-1]
                exit_code = last.split(":", 1)[1].strip() if last.startswith("exit code:") else "(none)"
            whole = retainable = "(absent)"
            findings = "(absent)"
            path = os.path.join(hosted, "%s.test262.json" % name)
            if os.path.isfile(path):
                report = json.load(open(path, encoding="utf-8"))
                whole = str(report["coverage"]["whole"]).lower()
                retainable = str(report["retention"]["retainable"]).lower()
                shard = [finding["failure"] for finding in report["findings"]
                         if finding["failure"] in ("IncompleteShardCoverage", "InconsistentShardConfiguration")]
                findings = ", ".join(sorted(set(shard))) or "none"
            distinct = "(absent)"
            path = os.path.join(hosted, "%s.binaries.log" % name)
            if os.path.isfile(path):
                last = open(path, encoding="utf-8").read().strip().splitlines()[-1]
                distinct = last.rsplit(":", 1)[1].strip()
            done = (exit_code in ("0", "1", "4") and whole == "true" and retainable == "true"
                    and findings == "none" and distinct == "1")
            verdict |= 0 if done else 1
            lines.append("%s | %s | %s | %s | %s | %s | %s" % (
                name, exit_code, whole, retainable, findings, distinct, "yes" if done else "NO"))
        self.write("completeness.log", "\n".join(lines) + "\n")
        print("complete: %s" % ("both forms" if verdict == 0 else "NOT both forms"))
        return verdict

    def compare(self):
        verdict = 0
        script = os.path.join(self.tree, "eng", "compare-test262-forms.py")
        for name, extra in RUNS:
            reports = []
            for side, folder in (("local", os.path.join(self.tree, LOCAL)), ("hosted", os.path.join(self.out, "hosted"))):
                source = os.path.join(folder, "%s.report.gz" % name)
                if not os.path.isfile(source):
                    print("compare: no %s report %s" % (side, source))
                    return 1
                target = os.path.join(self.scratch, "%s-%s.report" % (side, name))
                with gzip.open(source, "rb") as stream, open(target, "wb") as copy:
                    shutil.copyfileobj(stream, copy)
                reports.append(target)
            command = [sys.executable, script, reports[0], reports[1], "--same-form"] + extra
            if extra:
                if not self.suite:
                    print("compare: %s reads each test's source and needs --suite" % name)
                    return 1
                command += ["--suite", self.suite]
            code, output = self.run(command)
            self.write("compare-%s.log" % name, "$ %s\n%s\nexit code: %d\n" % (
                " ".join(command), output.strip(), code))
            print("compare: %s exit %d" % (name, code))
            verdict |= 0 if code == 0 else 1
        return verdict

    def manifest(self):
        files = {}
        for root, _, names in os.walk(self.out):
            for name in names:
                path = os.path.join(root, name)
                relative = os.path.relpath(path, self.out).replace(os.sep, "/")
                if relative != "manifest.json" and "__pycache__" not in relative:
                    files[relative] = self.sha256(path)
        document = {"bundle": "sjl-6-001", "repository": "Broiler.VM", "testedCommit": TESTED,
                    "collectorCommit": self.run(["git", "rev-parse", "HEAD"])[1].strip(),
                    "files": dict(sorted(files.items()))}
        with open(os.path.join(self.out, "manifest.json"), "w", encoding="utf-8", newline="\n") as stream:
            stream.write(json.dumps(document, indent=1) + "\n")
        print("manifest: %d files" % len(files))
        return 0


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--tree", required=True)
    parser.add_argument("--out", required=True)
    parser.add_argument("--suite", help="the pinned suite, extracted; the native comparison reads its sources")
    parser.add_argument("--artifact-zip")
    parser.add_argument("--run-url")
    parser.add_argument("--artifact-id")
    parser.add_argument("--run-times")
    parser.add_argument("--only", nargs="+", choices=STEPS, required=True)
    arguments = parser.parse_args()
    collector = Collector(arguments)
    verdict = 0
    for step in arguments.only:
        verdict |= getattr(collector, "import_" if step == "import" else step)()
    shutil.rmtree(collector.scratch, ignore_errors=True)
    return verdict


if __name__ == "__main__":
    sys.exit(main())
