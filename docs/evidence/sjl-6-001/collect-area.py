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
  manifest       the SHA-256 of every file this collector writes or reads as its own (base/, after/, vectors/,
                 this file, the rule and the correspondence), with the base commit base/identity.txt names, into
                 area-manifest.json.

THE AFTER STEPS WERE ADDED AT SJL-6.6, when there was an after commit to run them at. They write into after/
and read the base from base/. `--tree` is then a clean checkout of a commit whose product sources are the
cut-over commit's (after-identity lists every file outside the documents that differs from it), and
`--base-tree` one of the base commit, which only rerun and wall-race read. Part A's after runs are a dispatch of
the workflow, imported as the base's was; every other step runs locally, and reuses bundle sjl-5-001's
collector (and through it sjl-4-001's, sjl-3-001's and sjl-0-001's), so that SJL-5's and SJL-6's after arms run
the same code:

  after-identity  the after commit, its cleanliness, the cut-over commit it carries and every file outside the
                  documents that differs from it, the base it is compared with, the rule's digest, the SDK and
                  the machine, into after/identity.txt;
  after-import    the after dispatch's artifact test262-evidence, every file unchanged, into after/hosted/, as
                  base-import does;
  after-complete  each hosted form's completeness, read off after/hosted/, into after/completeness.log;
  after-twins     every twin and probe of base/fuel-parity.log, at its base figure and one less, in both forms,
                  with the answer it completes with beside the base's, into after/fuel-parity.log;
  compare         each form's after report against the base's, both as imported (base/hosted/, after/hosted/),
                  through the after tree's eng/compare-test262-forms.py --same-form, the native pair with
                  --exempt-guest-loads over the extracted suite, into after/compare-<run>.log;
  rerun           part A's class C4 (c'): every path of a (c') row run again alone at --base-tree and at --tree,
                  into after/rerun-<run>.*;
  wall-race       part A's class C4 (e): every (c') row whose re-runs differ, run alone once more at each commit
                  with ten times the wall-clock allowance, into after/wall-race-<run>.*;
  closures        every JavaScript composition root under JIT, trimming and Native AOT, into after/closures/;
  rules           the whole architecture suite at --tree, into after/rules.log;
  simplification  every member rule I5 registers, searched as a declaration in the JavaScript family's sources,
                  and the search for a surviving duplicate of the Date area in the whole component's product
                  sources, into after/simplification/;
  packages        eng/collect-js-packages.py at --tree, into after/packages/;
  vectors         as above, with --label after.
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
# The commit that lands SJL-6.4's cut-over (JSD-0073), whose product sources the after runs must be taken at.
CUT_OVER = "31f405bfdacae1f04318156445242e921da4541b"
# The search for a surviving duplicate of the Date area: the civil-day and time constants, the platform's own
# dates and zones, the civil-day helpers by name, and the month and weekday names a rendering spells. The
# platform's DateTime is named with its namespace, its constructor or one of its static members, because Temporal
# names its own date-time record and goals DateTime.
DUPLICATE_PATTERN = (r'146097|719468|\b86400000\b|8\.64e15|8640000000000000|\bSystem\.DateTime\b|\bDateTimeOffset\b'
                     r'|\bnew DateTime\(|\bDateTime\.(Now|UtcNow|Today|Parse|TryParse|ParseExact|TryParseExact|DaysInMonth'
                     r'|IsLeapYear|SpecifyKind)\b|\bTimeZoneInfo\b|\bDate(Only|TimeKind)\b|DaysFromCivil|CivilFromDays'
                     r'|IsLeapYear|DaysInMonth|"(Sun|Mon|Tue|Wed|Thu|Fri|Sat)"|"(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)"')
STEPS = ("base-identity", "base-import", "base-complete", "base-twins",
         "after-identity", "after-import", "after-complete", "after-twins", "compare", "rerun", "wall-race",
         "closures", "rules", "simplification", "packages", "vectors", "manifest")


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

    def hosted_collector(self, folder):
        """SJL-6.0's collector, pointed at base/ or after/: it reads and writes <out>/hosted/ and
        <out>/completeness.log."""
        arguments = argparse.Namespace(tree=self.tree, suite=self.suite, out=os.path.join(self.root_out, folder),
                                       artifact_zip=self.arguments.artifact_zip, run_url=self.arguments.run_url,
                                       artifact_id=self.arguments.artifact_id, run_times=self.arguments.run_times)
        return hosted.Collector(arguments)

    def base_import(self):
        self.into("base")
        return self.hosted_collector("base").import_()

    def base_complete(self):
        self.into("base")
        return self.hosted_collector("base").complete()

    def base_twins(self):
        # Bundle sjl-5-001's bisection reads its module's TWIN_FOLDERS; this area's are the wide twins and
        # the Date probes.
        text.TWIN_FOLDERS = TWIN_FOLDERS
        return super().base_twins()

    # ---- the after arm (SJL-6.6) --------------------------------------------------------------
    def after_identity(self):
        self.into("after")
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        status = self.run(["git", "status", "--porcelain"])[1]
        base_commit = next((line.split(" ", 1)[1].strip() for line in open(
            os.path.join(self.root_out, "base", "identity.txt"), encoding="utf-8")
            if line.startswith("base-commit ")), "(absent)")
        code, names = self.run(["git", "diff", "--name-only", CUT_OVER, commit])
        changed = [name for name in names.splitlines()
                   if name and not name.startswith("docs/") and "/docs/" not in name]
        rule = os.path.join(self.tree, RULE)
        lines = [
            "bundle sjl-6-001, the Date area's after arm",
            "milestone SJL-6, work package SJL-6.6, Broiler.VM's half",
            "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "after-commit " + commit,
            "after-tree " + ("clean" if not status.strip() else "dirty"),
            "cut-over-commit " + CUT_OVER,
            "base-commit " + base_commit + " (base/identity.txt)",
            "rule %s %s" % (RULE.replace(os.sep, "/"),
                            ("sha256=" + self.sha256_lf(rule)) if os.path.isfile(rule) else "ABSENT"),
            "sdk " + self.run(["dotnet", "--version"])[1].strip(),
            "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
            "suite-pin " + base.PIN + " sha256=" + self.sha256_lf(os.path.join(self.tree, base.PIN)),
            "allowance fuel=%d wallClockMs=%d" % (base.FUEL, base.WALL),
            "twin-allowance ceiling=%d wallClockMs=%d" % (base.TWIN_CEILING, base.TWIN_WALL),
            "native-backend " + base.NATIVE_BACKEND,
            "since-cut-over %s, outside the documents: %s" % (
                CUT_OVER, ("git diff failed with exit %d" % code) if code != 0
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
        print("after-identity: %s (%s tree)" % (commit, "clean" if not status.strip() else "DIRTY"), flush=True)
        return 0 if not status.strip() and os.path.isfile(rule) else 1

    def after_import(self):
        self.into("after")
        return self.hosted_collector("after").import_()

    def after_complete(self):
        self.into("after")
        return self.hosted_collector("after").complete()

    def compare(self):
        """Part A's comparison: each form's after dispatch against the base dispatch, both as imported, with
        the base's report as control and the after's as candidate. The log keeps sjl-4-001's form, which its
        rerun step reads."""
        self.into("after")
        if not self.suite:
            print("compare: the native pair reads each test's source and needs --suite")
            return 1
        verdict = 0
        for name, extra, _ in base.RUNS:
            reports = []
            for side in ("base", "after"):
                source = os.path.join(self.root_out, side, "hosted", "%s.report.gz" % name)
                if not os.path.isfile(source):
                    print("compare: no %s report %s" % (side, source))
                    return 1
                target = os.path.join(self.scratch, "%s-%s.report" % (side, name))
                with gzip.open(source, "rb") as stream, open(target, "wb") as copy:
                    shutil.copyfileobj(stream, copy)
                reports.append(target)
            command = [sys.executable, os.path.join(self.tree, "eng", "compare-test262-forms.py"), reports[0], reports[1],
                       "--same-form"] + (["--exempt-guest-loads"] if "native" in extra else []) + ["--suite", self.suite]
            code, output = self.run(command)
            self.write("compare-%s.log" % name, "$ compare-test262-forms.py <base %s> <after %s> %s\n%s\nexit code: %d\n"
                       % (name, name, " ".join(command[4:]).replace(self.suite, "<suite>"), output.strip(), code))
            print("compare: %s exit %d" % (name, code), flush=True)
            verdict |= code
        return verdict

    def simplification(self):
        self.into("after")
        os.makedirs(os.path.join(self.out, "simplification"), exist_ok=True)
        after = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        family = [p for p in sorted(os.listdir(os.path.join(self.tree, "src")))
                  if p.startswith("Broiler.VM.Profile.JavaScript")]
        family_paths = [os.path.join("src", p, "*.cs") for p in family]
        retired = text.numerics.prior.RETIRED
        register = json.load(open(os.path.join(self.tree, retired), encoding="utf-8"))["retired"]
        lines = ["# SJL-6.6 deletion check, Broiler.VM at %s: every member rule I5 registers (%s), its simple"
                 % (after, retired.replace(os.sep, "/")),
                 "# name searched as a declaration - a modifier, then the name as a whole word - in the JavaScript",
                 "# family's C# sources (%s). A member listed with no line is declared nowhere there."
                 % ", ".join(p.replace(os.sep, "/") for p in family_paths),
                 "# Rule I5 itself decides by the fully qualified name, and its run is rules.log.", ""]
        absent = 0
        for entry in register:
            simple = entry["member"].rsplit(".", 1)[-1]
            hits = self.grep(["-E", r"\b(static|private|internal|public|protected|const|readonly|class|struct|enum)\b"
                              r"[^=;(]*\b%s\b\s*[(=;{<:]" % re.escape(simple)], family_paths)
            absent += not hits
            lines += ["## %s (%s)" % (entry["member"], entry["milestone"])] + hits + [""]
        lines.append("# registered members with no declaration of their simple name: %d" % absent)
        lines.append("# registered members: %d" % len(register))
        self.write(os.path.join("simplification", "deletion-check.log"), "\n".join(lines) + "\n")

        product = ["src/*.cs", ":(exclude)src/tests", ":(exclude,glob)src/**/tests/**"]
        hits = self.grep(["-E", DUPLICATE_PATTERN], product)
        search = ["# SJL-6.6 search for a surviving duplicate of the Date area, Broiler.VM at %s: every construct" % after,
                  "# that spells a civil-day or time constant (the 400-year cycle's days, the civil epoch's shift, the",
                  "# day's milliseconds, the time value's limit), uses the platform's dates or zones, names a civil-day",
                  "# helper, or spells a month or weekday name, in the component's C# product sources (src/, every",
                  "# tests/ excluded).",
                  "# pattern: " + DUPLICATE_PATTERN, "# hits: %d; each is reviewed in the bundle's README" % len(hits), ""]
        search += [hit[:220] for hit in hits]
        self.write(os.path.join("simplification", "duplicate-search.log"), "\n".join(search) + "\n")
        print("simplification: %d registered members searched, %d duplicate-search hits" % (len(register), len(hits)),
              flush=True)
        return 0

    def packages(self):
        """The candidate pack, the pristine consumer, the rollback, the negative control and the consumer as
        Native AOT, by the after tree's own eng/collect-js-packages.py, as bundle sjl-5-001 took them."""
        self.into("after")
        out = os.path.join(self.out, "packages")
        if os.path.exists(out):
            print("packages: %s exists; the step writes it once" % out)
            return 1
        code, output = self.run([sys.executable, os.path.join("eng", "collect-js-packages.py"),
                                 "--bundle", "SJL-6-001", "--out", out])
        self.write("packages.log", output.strip() + "\nexit code: %d\n" % code)
        print("packages: exit %d" % code, flush=True)
        return code

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
        for folder in ("base", "after", "vectors"):
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
    parser.add_argument("--tree", required=True, help="a clean checkout: the base commit, or the after commit")
    parser.add_argument("--base-tree", help="a clean checkout of the base commit, for rerun and wall-race")
    parser.add_argument("--suite", help="an unpacked checkout of the pinned test262 revision")
    parser.add_argument("--images", help="the closures step's mstat records, for rule I3 (rules)")
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
