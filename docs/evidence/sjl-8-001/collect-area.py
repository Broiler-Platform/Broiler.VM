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
  manifest       the SHA-256 of every file this collector writes or reads as its own (base/, cutover/, after/,
                 vectors/, this file, the rule and the correspondence), with the base commit base/identity.txt
                 names, into area-manifest.json. correspondence-runs/ is the correspondence's own record and is not
                 listed. cutover/ holds the checks SJL-8.5 ran at its cut-over, retained by hand (README section 4),
                 and is listed from SJL-8.5.

THE VECTORS STEP WAS ADDED AT SJL-8.4, when the shared corpus gained the time-zone area. It is bundle sjl-6-001's
step, unchanged:

  vectors        the conformance root over a retained pin of the shared corpus (--pin, by default the tree's own
                 src/tests/vectors/pins/vectors.pin), and its negative control, into
                 vectors/<label>-revision-<n>.* and vectors/<label>-negative-control-revision-<n>.*.

THE AFTER STEPS WERE ADDED ON 2026-10-17, AT SJL-8.7, when there was an after commit to run them at, as bundle
sjl-7-001's were at SJL-7.6, and they run that bundle's code wherever it has the step. They write into after/ and
read the base from base/. `--tree` is then a clean checkout of a commit whose product sources are the cut-over's
(after-identity lists every file outside the documents that differs from it), and `--base-tree` one of the base
commit, which only rerun and wall-race read. Part A's after run is a dispatch of the workflow, imported as the
base's was; every other step runs locally:

  after-identity  the after commit, its cleanliness, the cut-over commit it carries, every file outside the
                  documents that differs from it and every commit since it, the base it is compared with, the
                  rule's digest, the SDK and the machine, into after/identity.txt (this bundle's names, bundle
                  sjl-7-001's step);
  after-import    the after dispatch's artifact test262-evidence, every file unchanged, into after/hosted/, as
                  base-import does (bundle sjl-6-001's step);
  after-complete  each hosted form's completeness, read off after/hosted/, into after/completeness.log (bundle
                  sjl-6-001's step);
  after-twins     every twin of base/fuel-parity.log, at its base figure and one less, in both forms, with the
                  answer it completes with beside the base's, into after/fuel-parity.log (bundle sjl-4-001's
                  step, which sjl-5-001 to sjl-7-001 ran);
  after-datasets  part B at --tree, as base-datasets takes it, into after/datasets/; then each named check's line
                  beside the base's, files.sha256 beside the base's, and `git diff --name-only` between the two
                  commits over the dataset folders, into after/datasets/compare.txt;
  compare         each form's after report against the base's, both as imported (base/hosted/, after/hosted/),
                  through the after tree's eng/compare-test262-forms.py --same-form, the native pair with
                  --exempt-guest-loads over the extracted suite, into after/compare-<run>.log (bundle sjl-6-001's
                  step);
  rerun           part A's class C4 (c'): every path of a (c') row run again alone at --base-tree and at --tree,
                  into after/rerun-<run>.* (bundle sjl-4-001's step);
  wall-race       part A's class C4 (e): every (c') row whose re-runs differ, run alone once more at each commit
                  with ten times the wall-clock allowance, into after/wall-race-<run>.* (bundle sjl-5-001's step);
  closures        every JavaScript composition root under JIT, trimming and Native AOT, into after/closures/, and
                  the SHA-256 of each Native AOT image record, into after/closures/mstat.sha256 (bundle sjl-7-001's
                  step), run from --tree as the working directory, under which each published slice compiler finds
                  the retained datasets its checks read (since 2026-10-18: the step's first run, from the
                  collector's own working directory, left those checks not run); the records themselves are not
                  retained in the bundle;
  rules           the whole architecture suite at --tree, rule I3 reading the image records closures left (or
                  --images), into after/rules.log (bundle sjl-3-001's step);
  simplification  every member rule I5 registers, searched as a declaration in the JavaScript family's sources
                  (bundle sjl-7-001's search); the search for a surviving duplicate of the time-zone and calendar
                  area in the whole component's product sources - the platform's zones and calendars, the area's
                  algorithms by their names and constants, and IANA zone identifiers spelled as literals; and the
                  lines each C# file gained and lost between the base and after commits, into
                  after/simplification/;
  packages        eng/collect-js-packages.py at --tree, into after/packages/ (bundle sjl-6-001's step, with this
                  bundle's name).
"""

import argparse
import datetime
import importlib.util
import json
import os
import platform
import re
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

# SJL-8.5's cut-over (JSD-0077): its code landed in 4df595b, f09a0dd, c5943b1, 186a1d3, 8613309 and eed568a, its
# record in 05f99db, and 88f1081 pinned the committed feed to Broiler.ECMAScript 0.1.0-preview.5 as published
# (JSC-313). The after runs are taken at a commit whose product sources are this one's.
CUT_OVER = "88f10812f8f1e89fa135c7bc37d8a35d88556212"
CUT_OVER_NOTE = ("SJL-8.5's cut-over, JSD-0077: its code in 4df595b, f09a0dd, c5943b1, 186a1d3, 8613309 and"
                 " eed568a, its record in 05f99db, and the committed feed pinned to Broiler.ECMAScript"
                 " 0.1.0-preview.5 as published in 88f1081")

# The search for a surviving duplicate of the time-zone and calendar area, in three parts. HOST: the platform's
# zones and calendars - TimeZoneInfo, the local-time conversions, DateTimeOffset, and .NET's calendar classes,
# which SJL-D-21 keeps out of both engines.
HOST_PATTERN = (r'\bTimeZoneInfo\b|\bTimeZone\.CurrentTimeZone\b|\bDateTimeOffset\b|\.ToLocalTime\(|\.ToUniversalTime\('
                r'|\b(ChineseLunisolar|KoreanLunisolar|JapaneseLunisolar|TaiwanLunisolar|EastAsianLunisolar|UmAlQura'
                r'|Persian|Hijri|Hebrew|Japanese|Taiwan|ThaiBuddhist|Korean|Gregorian|Julian)Calendar\b'
                r'|\bCultureInfo\b[^;]*\.(Calendar|OptionalCalendars)\b')
# ALGORITHM: the area's algorithms by their names and constants - the tz database's files and compiled form, the
# table format's varints, the lunisolar and Persian calendars' astronomy and rules, and the proposal's names of
# the zone operations a hand-written copy would cite.
ALGORITHM_PATTERN = (r'\bTZif\b|\bzic\b|zone1970\.tab|\bzone\.tab\b|\bbackzone\b|\btzdata\b|\bvarint|\bVarint'
                     r'|29\.530|365\.24[0-9]|[Ss]ynodic|[Ss]olar ?[Tt]erm|[Nn]ew ?[Mm]oons?\b|[Ss]olstice|[Ee]quinox'
                     r'|\bRataDie\b|33-year|\bMetonic\b|GetPossibleEpochNanoseconds|DisambiguatePossibleEpochNanoseconds'
                     r'|GetStartOfDay|GetNamedTimeZone(Next|Previous)Transition|GetNamedTimeZoneOffsetNanoseconds'
                     r'|GetNamedTimeZoneEpochNanoseconds')
# IDENTIFIERS: an IANA zone identifier spelled as a string literal - an area's name and a zone, or Etc/ - which a
# hand-written list of zones or a zone-name rule outside the tables would spell.
IDENTIFIER_PATTERN = r'"(Africa|America|Antarctica|Arctic|Asia|Atlantic|Australia|Europe|Indian|Pacific)/[A-Za-z_+-]|"Etc/'
DUPLICATE_SEARCHES = (
    ("host", HOST_PATTERN, "the platform's zones, local-time conversions and calendar classes"),
    ("algorithm", ALGORITHM_PATTERN, "the area's algorithms by their names and constants"),
    ("identifiers", IDENTIFIER_PATTERN, "IANA zone identifiers spelled as string literals"),
)

STEPS = ("base-identity", "base-import", "base-complete", "base-twins", "base-datasets",
         "after-identity", "after-import", "after-complete", "after-twins", "after-datasets", "compare", "rerun",
         "wall-race", "closures", "rules", "simplification", "packages", "vectors", "manifest")


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

    # ---- the after arm (SJL-8.7) --------------------------------------------------------------
    def after_identity(self):
        self.into("after")
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        status = self.run(["git", "status", "--porcelain"])[1]
        base_commit = area7.recorded(os.path.join(self.root_out, "base", "identity.txt"), "base-commit") or "(absent)"
        code, names = self.run(["git", "diff", "--name-only", CUT_OVER, commit])
        changed = area7.outside_documents(names)
        ancestry = self.run(["git", "merge-base", "--is-ancestor", CUT_OVER, commit])[0]
        commits = self.run(["git", "log", "--reverse", "--format=%H%x09%s", "%s..%s" % (CUT_OVER, commit)])[1]
        rule = os.path.join(self.tree, RULE)
        lines = [
            "bundle sjl-8-001, the time-zone and calendar area's after arm",
            "milestone SJL-8, work package SJL-8.7, Broiler.VM's half",
            "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "after-commit " + commit,
            "after-tree " + ("clean" if not status.strip() else "dirty"),
            "cut-over-commit %s (%s)" % (CUT_OVER, CUT_OVER_NOTE),
            "cut-over-is-ancestor " + ("yes" if ancestry == 0 else "NO"),
            "base-commit " + base_commit + " (base/identity.txt)",
            "rule %s %s" % (RULE.replace(os.sep, "/"),
                            ("sha256=" + self.sha256_lf(rule)) if os.path.isfile(rule) else "ABSENT"),
            "sdk " + self.run(["dotnet", "--version"])[1].strip(),
            "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
            "suite-pin " + base.PIN + " sha256=" + self.sha256_lf(os.path.join(self.tree, base.PIN)),
            "allowance fuel=%d wallClockMs=%d" % (base.FUEL, base.WALL),
            "twin-allowance ceiling=%d wallClockMs=%d" % (base.TWIN_CEILING, base.TWIN_WALL),
            "jobs %d" % self.jobs,
            "native-backend " + base.NATIVE_BACKEND,
            "twin-folders " + ", ".join(folder.replace(os.sep, "/") for folder in TWIN_FOLDERS),
            "dataset-folders " + ", ".join(folder.replace(os.sep, "/") for folder in DATASET_FOLDERS),
            "since-cut-over %s, outside the documents: %s" % (
                CUT_OVER, ("git diff failed with exit %d" % code) if code != 0
                else (str(len(changed)) + " files") if changed else "none"),
        ]
        lines.extend("  " + name for name in changed)
        lines.append("commits since the cut-over, each with its files outside the documents:")
        for entry in commits.splitlines():
            if not entry.strip():
                continue
            sha, subject = entry.split("\t", 1)
            touched = area7.outside_documents(
                self.run(["git", "diff-tree", "--no-commit-id", "--name-only", "-r", sha])[1])
            lines.append("  %s %s: %s" % (sha, subject, (str(len(touched)) + " files") if touched else "none"))
            lines.extend("    " + name for name in touched)
        if status.strip():
            lines.append("dirty:")
            lines.extend("  " + line for line in status.splitlines())
        path = os.path.join(self.out, "identity.txt")
        if os.path.exists(path):
            os.chmod(path, stat.S_IREAD | stat.S_IWRITE)
        self.write("identity.txt", "\n".join(lines) + "\n")
        os.chmod(path, stat.S_IREAD | stat.S_IRGRP | stat.S_IROTH)
        print("after-identity: %s (%s tree)" % (commit, "clean" if not status.strip() else "DIRTY"), flush=True)
        return 0 if not status.strip() and os.path.isfile(rule) and ancestry == 0 else 1

    def after_datasets(self):
        """Part B at --tree, then the rule's comparison with the base, read off base/datasets/."""
        verdict = self.datasets("after")
        base_folder = os.path.join(self.root_out, "base", "datasets")
        after_folder = os.path.join(self.root_out, "after", "datasets")
        base_commit = area7.recorded(os.path.join(self.root_out, "base", "identity.txt"), "base-commit")
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()

        def body(path):
            # The first line names the commit the checks ran at; every other line is compared.
            return open(path, encoding="utf-8").read().splitlines()[1:] if os.path.isfile(path) else None

        lines = ["# part B's comparison: base %s (base/datasets/), after %s (after/datasets/)" % (base_commit, commit)]
        before, after = body(os.path.join(base_folder, "named-checks.txt")), body(os.path.join(after_folder,
                                                                                               "named-checks.txt"))
        differing = 0
        if before is None or after is None:
            lines.append("named-checks.txt is missing on one side")
            differing += 1
        else:
            for index in range(max(len(before), len(after))):
                left = before[index] if index < len(before) else "(absent)"
                right = after[index] if index < len(after) else "(absent)"
                same = left == right
                differing += 0 if same else 1
                lines.append("%s %s" % ("same   " if same else "DIFFERS", left))
                if not same:
                    lines.append("        after: " + right)
        digests = [os.path.join(folder, "files.sha256") for folder in (base_folder, after_folder)]
        identical = all(os.path.isfile(path) for path in digests) and \
            open(digests[0], "rb").read() == open(digests[1], "rb").read()
        lines.append("files.sha256: %s" % ("identical, %d files" % len(open(digests[1], encoding="utf-8").read()
                                                                     .splitlines()) if identical else "DIFFERS"))
        code, names = self.run(["git", "diff", "--name-only", base_commit or "(absent)", commit, "--"]
                               + [folder.replace(os.sep, "/") for folder in DATASET_FOLDERS])
        changed = [name for name in names.splitlines() if name.strip()]
        lines.append("git diff --name-only %s %s -- %s: %s" % (
            base_commit, commit, " ".join(folder.replace(os.sep, "/") for folder in DATASET_FOLDERS),
            ("exit %d" % code) if code != 0 else ("%d files" % len(changed)) if changed else "no file"))
        lines.extend("  " + name for name in changed)
        with open(os.path.join(after_folder, "compare.txt"), "w", encoding="utf-8", newline="\n") as stream:
            stream.write("\n".join(lines) + "\n")
        print("after-datasets: %d named-check lines differ from the base's, files.sha256 %s, %d dataset files differ"
              % (differing, "identical" if identical else "DIFFERS", len(changed)), flush=True)
        return verdict | (1 if differing or not identical or changed or code != 0 else 0)

    def closures(self):
        """Bundle sjl-7-001's step, from --tree. Bundle sjl-3-001's procedure runs each published root in the
        collector's own working directory, and a published slice compiler runs the checks over retained datasets
        only when it finds them under that directory; from anywhere else it leaves them not run. Bundle
        sjl-7-001's collector was run from a checkout; this one runs the roots from the tree it publishes."""
        here = os.getcwd()
        os.chdir(self.tree)
        try:
            return area7.Collector.closures(self)
        finally:
            os.chdir(here)

    def simplification(self):
        self.into("after")
        os.makedirs(os.path.join(self.out, "simplification"), exist_ok=True)
        after = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        base_commit = area7.recorded(os.path.join(self.root_out, "base", "identity.txt"), "base-commit")
        family = [p for p in sorted(os.listdir(os.path.join(self.tree, "src")))
                  if p.startswith("Broiler.VM.Profile.JavaScript")]
        family_paths = [os.path.join("src", p, "*.cs") for p in family]
        retired = area6.text.numerics.prior.RETIRED
        register = json.load(open(os.path.join(self.tree, retired), encoding="utf-8"))["retired"]
        lines = ["# SJL-8.7 deletion check, Broiler.VM at %s: every member rule I5 registers (%s), its simple"
                 % (after, retired.replace(os.sep, "/")),
                 "# name searched as a declaration - a modifier, then the name as a whole word, then an opening",
                 "# parenthesis, brace, angle bracket, colon, assignment or semicolon, or the end of the line, as a",
                 "# type's declaration ends - in the JavaScript family's C# sources (%s)."
                 % ", ".join(p.replace(os.sep, "/") for p in family_paths),
                 "# A member listed with no line is declared nowhere there. Rule I5 itself decides by the fully",
                 "# qualified name, and its run is rules.log.", ""]
        absent = {}
        for entry in register:
            simple = entry["member"].rsplit(".", 1)[-1]
            hits = self.grep(["-E", r"\b(static|private|internal|public|protected|const|readonly|class|struct|enum)\b"
                              r"[^=;(]*\b%s\b\s*([(=;{<:]|$)" % re.escape(simple)], family_paths)
            absent.setdefault(entry["milestone"], [0, 0])
            absent[entry["milestone"]][0] += 1
            absent[entry["milestone"]][1] += not hits
            lines += ["## %s (%s)" % (entry["member"], entry["milestone"])] + hits + [""]
        for milestone, (count, missing) in sorted(absent.items()):
            lines.append("# %s: %d registered, %d with no declaration of their simple name" % (milestone, count, missing))
        lines.append("# registered members with no declaration of their simple name: %d"
                     % sum(missing for _, missing in absent.values()))
        lines.append("# registered members: %d" % len(register))
        self.write(os.path.join("simplification", "deletion-check.log"), "\n".join(lines) + "\n")

        product = ["src/*.cs", ":(exclude)src/tests", ":(exclude,glob)src/**/tests/**"]
        search = ["# SJL-8.7 search for a surviving duplicate of the time-zone and calendar area, Broiler.VM at %s," % after,
                  "# in the component's C# product sources (src/, every tests/ excluded), in three parts, each with",
                  "# its pattern and its hits; every hit is reviewed in the bundle's README.", ""]
        total = 0
        for name, pattern, what in DUPLICATE_SEARCHES:
            hits = self.grep(["-E", pattern], product)
            total += len(hits)
            search += ["## %s: %s" % (name, what), "# pattern: " + pattern, "# hits: %d" % len(hits)]
            search += [hit[:220] for hit in hits] + [""]
        search.append("# hits in all three parts: %d" % total)
        self.write(os.path.join("simplification", "duplicate-search.log"), "\n".join(search) + "\n")

        # The lines each C# file gained and lost between the base and after commits, by `git diff --numstat`,
        # in three groups: the JavaScript family's product sources, the other product sources, and the tests.
        code, numstat = self.run(["git", "diff", "--numstat", "--no-renames", base_commit or "(absent)", after, "--",
                                  "*.cs"])
        groups = {"family": [0, 0, 0], "other product": [0, 0, 0], "tests": [0, 0, 0]}
        rows = []
        for line in numstat.splitlines():
            cells = line.split("\t")
            if len(cells) != 3 or not cells[0].isdigit():
                continue
            added, removed, path = int(cells[0]), int(cells[1]), cells[2]
            group = ("tests" if path.startswith("src/tests/") or "/tests/" in path
                     else "family" if path.startswith("src/Broiler.VM.Profile.JavaScript") else "other product")
            groups[group][0] += added
            groups[group][1] += removed
            groups[group][2] += 1
            rows.append("%s\t%d\t%d\t%s" % (group, added, removed, path))
        counted = ["# SJL-8.7 lines added and removed in every C# file between the base %s and the after commit %s,"
                   % (base_commit, after),
                   "# by `git diff --numstat --no-renames <base> <after> -- '*.cs'` (exit %d), each file in one group: the"
                   % code,
                   "# JavaScript family's product sources (src/Broiler.VM.Profile.JavaScript*), the other product sources,",
                   "# and the tests (src/tests/ and every tests/ folder). Generated files count as written.", ""]
        counted += ["group\tadded\tremoved\tfile"] + sorted(rows, key=lambda row: (row.split("\t")[0], row.split("\t")[3]))
        counted.append("")
        for group, (added, removed, files) in groups.items():
            counted.append("# %s: %d files, %d lines added, %d removed, net %+d" % (group, files, added, removed,
                                                                                   added - removed))
        self.write(os.path.join("simplification", "lines.log"), "\n".join(counted) + "\n")
        print("simplification: %d registered members searched, %d duplicate-search hits, %d C# files changed"
              % (len(register), total, len(rows)), flush=True)
        return 0 if code == 0 else 1

    def packages(self):
        """The candidate pack, the pristine consumer, the rollback, the negative control and the consumer as
        Native AOT, by the after tree's own eng/collect-js-packages.py, as bundle sjl-7-001 took them."""
        self.into("after")
        out = os.path.join(self.out, "packages")
        if os.path.exists(out):
            print("packages: %s exists; the step writes it once" % out)
            return 1
        code, output = self.run([sys.executable, os.path.join("eng", "collect-js-packages.py"),
                                 "--bundle", "SJL-8-001", "--out", out])
        self.write("packages.log", output.strip() + "\nexit code: %d\n" % code)
        print("packages: exit %d" % code, flush=True)
        return code

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
