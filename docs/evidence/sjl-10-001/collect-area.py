#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Collect Broiler.VM's evidence for milestone SJL-10 (the Temporal core) into bundle sjl-10-001.

    python3 docs/evidence/sjl-10-001/collect-area.py --tree <a clean checkout> --out docs/evidence/sjl-10-001 \
        [--base-tree <a clean checkout of the base commit>] [--suite <the pinned test262 revision, extracted>] \
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

THE AFTER STEPS WERE ADDED ON 2026-10-19, AT SJL-10.6, when there was an after commit to run them at, as bundle
sjl-8-001's were at SJL-8.7, and they run that bundle's code wherever it has the step. They write into after/ and
read the base from base/. `--tree` is then a clean checkout of a commit whose product sources are the cut-over's
(after-identity lists every file outside the documents that differs from it), and `--base-tree` one of the base
commit, which only rerun and wall-race read. Part A's after run is a dispatch of the workflow, imported as the
base's was; every other step runs locally:

  after-identity  the after commit, its cleanliness, the cut-over commit it carries, every file outside the
                  documents that differs from it and every commit since it, the base it is compared with, the
                  rule's digest, the SDK and the machine, into after/identity.txt (this bundle's names, bundle
                  sjl-8-001's step);
  after-import    the after dispatch's artifact test262-evidence, every file unchanged, into after/hosted/, as
                  base-import does (bundle sjl-6-001's step);
  after-complete  each hosted form's completeness, read off after/hosted/, into after/completeness.log (bundle
                  sjl-6-001's step);
  after-twins     every twin of base/fuel-parity.log, at its base figure and one less, in both forms, with the
                  answer it completes with beside the base's, into after/fuel-parity.log (bundle sjl-4-001's
                  step, which sjl-5-001 to sjl-8-001 ran);
  after-datasets  part B at --tree, as base-datasets takes it, into after/datasets/; then each named check's line
                  beside the base's, files.sha256 beside the base's, and `git diff --name-only` between the two
                  commits over src/tests/temporal/, into after/datasets/compare.txt (bundle sjl-8-001's step,
                  over this rule's six checks and its one folder);
  compare         each form's after report against the base's, both as imported (base/hosted/, after/hosted/),
                  through the after tree's eng/compare-test262-forms.py --same-form, the native pair with
                  --exempt-guest-loads over the extracted suite, into after/compare-<run>.log (bundle sjl-6-001's
                  step);
  rerun           part A's class C4 (c'): every path of a (c') row run again alone at --base-tree and at --tree,
                  into after/rerun-<run>.* (bundle sjl-4-001's step);
  wall-race       part A's class C4 (e): every (c') row whose re-runs differ, run alone once more at each commit
                  with ten times the wall-clock allowance, into after/wall-race-<run>.* (bundle sjl-5-001's step);
  closures        every JavaScript composition root under JIT, trimming and Native AOT, into after/closures/, and
                  the SHA-256 of each Native AOT image record, into after/closures/mstat.sha256, run from --tree as
                  the working directory (bundle sjl-8-001's step); the records themselves are not retained in the
                  bundle;
  rules           the whole architecture suite at --tree, rule I3 reading the image records closures left (or
                  --images), into after/rules.log (bundle sjl-3-001's step);
  simplification  every member rule I5 registers, searched as a declaration in the JavaScript family's sources
                  (bundle sjl-8-001's search); the search for a surviving duplicate of the Temporal core in the
                  whole component's product sources - the platform's dates and calendars, the area's algorithms
                  by the pinned text's names and by their constants, the names of the types SJL-10.4 deleted, and
                  calendar identifiers and era names spelled as literals; and the lines each C# file gained and
                  lost between the base and after commits, into after/simplification/;
  packages        eng/collect-js-packages.py at --tree, into after/packages/ (bundle sjl-6-001's step, with this
                  bundle's name).

Before that, this paragraph read: "The after steps join this file when there is an after commit to run them at,
SJL-10.4's cut-over, as bundle sjl-8-001's did at SJL-8.7."
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

# SJL-10.4's cut-over (JSD-0079): its code landed in 66b0877, a13a41c, f14abb7 and 9b7eead, its record in c0355f4,
# and 8b6cc96 pinned the committed feed to Broiler.ECMAScript 0.1.0-preview.6 as published (JSC-315); e42cd5d is
# their merge (Broiler-Platform/Broiler.VM#151). The after runs are taken at a commit whose product sources are
# this one's.
CUT_OVER = "e42cd5d931177658e5beb40e13b9aa000d4d0ee6"
CUT_OVER_NOTE = ("SJL-10.4's cut-over, JSD-0079: its code in 66b0877, a13a41c, f14abb7 and 9b7eead, its record in"
                 " c0355f4, and the committed feed pinned to Broiler.ECMAScript 0.1.0-preview.6 as published in"
                 " 8b6cc96, merged in e42cd5d")
CUT_OVER_MILESTONE = "SJL-10.4"

# The search for a surviving duplicate of the Temporal core, in four parts. HOST: the platform's dates and
# calendars a hand-written core would compute with - .NET's calendar classes, which SJL-D-21 keeps out of both
# engines, DateOnly, TimeOnly and ISOWeek, and DateTime's month lengths, leap years and month and year addition.
HOST_PATTERN = (r'\bDateOnly\b|\bTimeOnly\b|\bISOWeek\b|\bDateTime\.(DaysInMonth|IsLeapYear)\(|\.Add(Months|Years)\('
                r'|\b(ChineseLunisolar|KoreanLunisolar|JapaneseLunisolar|TaiwanLunisolar|EastAsianLunisolar|UmAlQura'
                r'|Persian|Hijri|Hebrew|Japanese|Taiwan|ThaiBuddhist|Korean|Gregorian|Julian)Calendar\b'
                r'|\bCultureInfo\b[^;]*\.(Calendar|OptionalCalendars)\b')
# ALGORITHM: the area's algorithms by the names the pinned text gives the operations SJL-10.4's deleted code
# implemented (JSD-0079 section 8, the correspondence's section 10), which a hand-written copy would cite or be
# named after; the limits' constants (the years -271821 and 275760, 8.64e21 nanoseconds, a day's nanoseconds);
# and the formula calendars' epochs and rules (the Rata Die epochs, the Hebrew calendar's parts and molad, the
# tabular Islamic cycle, the mean synodic month and year, and the Persian 33-year rule).
ALGORITHM_PATTERN = (r'\b(BalanceISODate(Time)?|BalanceTime|RegulateISODate|RegulateTime|IsValidISODate'
                     r'|ISODate(Time)?WithinLimits|ISOYearMonthWithinLimits|IsValidEpochNanoseconds|AddDaysToISODate'
                     r'|CompareISODate|ISODateSurpasses|DifferenceISODate(Time)?|DifferenceInstant'
                     r'|DifferencePlainDateTimeWith(Rounding|Total)|DifferenceZonedDateTime(With(Rounding|Total))?'
                     r'|GetDifferenceSettings|ComputeNudgeWindow|NudgeToCalendarUnit|NudgeToZonedTime|NudgeToDayOrTime'
                     r'|BubbleRelativeDuration|RoundRelativeDuration|TotalRelativeDuration|RoundTimeDuration(ToIncrement)?'
                     r'|TotalTimeDuration|DateDurationDays|Add24HourDaysToTimeDuration|TimeDurationFromComponents'
                     r'|CombineDateAndTimeDuration|ToInternalDurationRecord\w*|TemporalDurationFromInternal'
                     r'|DefaultTemporalLargestUnit|LargerOfTwoTemporalUnits|MaximumTemporalDurationRoundingIncrement'
                     r'|RoundISODateTime|RoundTemporalInstant|CalendarDateAdd|CalendarDateUntil'
                     r'|Calendar(Date|YearMonth|MonthDay)FromFields|CalendarResolveFields|CalendarMonthDayToISOReferenceDate'
                     r'|NonISO\w+|CalendarISOToDate|CanonicalizeEraInCalendar|ConstrainMonthCode|ParseMonthCode'
                     r'|CreateMonthCode|ParseISODateTime|ParseTemporal\w*String'
                     r'|Temporal(Date|DateTime|Time|Instant|ZonedDateTime|YearMonth|MonthDay|Duration)ToString'
                     r'|FormatFractionalSeconds|FormatTimeString|FormatCalendarAnnotation|FormatDateTimeUTCOffsetRounded'
                     r'|FormatUTCOffsetNanoseconds|PadISOYear)\b'
                     r'|\b27(5760|1821)\b|8\.64e21|8_?640_?000_?000_?000_?000_?000_?000|86_?400_?000_?000_?000\b'
                     r'|\b(719163|103605|2796|227015|226896|1373427|25920|765433|13753|98496|35975351|10631)\b'
                     r'|29\.53|365\.24[0-9]|\bRataDie\b|[Mm]olad|\bMetonic\b|33-year|[Ss]ynodic')
# IDENTIFIERS: a calendar identifier or an era name of the era proposal spelled as a string literal, which a
# hand-written list of calendars or table of eras outside the shared library would spell.
IDENTIFIER_PATTERN = (r'"(buddhist|chinese|coptic|dangi|ethioaa|ethiopic|gregory|hebrew|indian'
                      r'|islamic(-civil|-tbla|-umalqura|-rgsa)?|iso8601|japanese|persian|roc)"'
                      r'|"(meiji|taisho|showa|heisei|reiwa|bce|broc|shaka|saka)"')
DUPLICATE_SEARCHES = (
    ("host", HOST_PATTERN, "the platform's dates, times and calendar classes"),
    ("algorithm", ALGORITHM_PATTERN, "the area's algorithms by the pinned text's names and by their constants"),
    # NAMES is built from the register: every type rule I5 registers for SJL-10.4, its simple name as a word.
    ("names", None, "the names of the types rule I5 registers for SJL-10.4, as words anywhere"),
    ("identifiers", IDENTIFIER_PATTERN, "calendar identifiers and era names spelled as string literals"),
)
# The namespace whose types rule I5 registers: a registered member of it with no further dot is a type.
FAMILY_NAMESPACE = "Broiler.VM.Profile.JavaScript"

STEPS = ("base-identity", "base-import", "base-complete", "base-twins", "base-datasets",
         "after-identity", "after-import", "after-complete", "after-twins", "after-datasets", "compare", "rerun",
         "wall-race", "closures", "rules", "simplification", "packages", "vectors", "manifest")


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

    # ---- the after arm (SJL-10.6) -------------------------------------------------------------
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
            "bundle sjl-10-001, the Temporal core's after arm",
            "milestone SJL-10, work package SJL-10.6, Broiler.VM's half",
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
        # Bundle sjl-8-001's step, with that module's DATASET_CHECKS and DATASET_FOLDERS set to this rule's six
        # checks and its one folder, which its comparison with the base reads too.
        area8.DATASET_CHECKS = DATASET_CHECKS
        area8.DATASET_FOLDERS = DATASET_FOLDERS
        return area8.Collector.after_datasets(self)

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
        lines = ["# SJL-10.6 deletion check, Broiler.VM at %s: every member rule I5 registers (%s), its simple"
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

        # The types SJL-10.4 deleted, as rule I5 registers them: a member of the family's namespace with no
        # further dot. Their names are searched as words anywhere, a use or a comment included.
        types = sorted(entry["member"].rsplit(".", 1)[-1] for entry in register
                       if entry["milestone"] == CUT_OVER_MILESTONE
                       and entry["member"].rsplit(".", 1)[0] == FAMILY_NAMESPACE)
        names_pattern = r"\b(%s)\b" % "|".join(re.escape(name) for name in types)
        product = ["src/*.cs", ":(exclude)src/tests", ":(exclude,glob)src/**/tests/**"]
        search = ["# SJL-10.6 search for a surviving duplicate of the Temporal core, Broiler.VM at %s, in the" % after,
                  "# component's C# product sources (src/, every tests/ excluded), in four parts, each with its",
                  "# pattern and its hits; every hit is reviewed in the bundle's README. The names part is built",
                  "# from rule I5's register: the %d types it registers for %s." % (len(types), CUT_OVER_MILESTONE),
                  ""]
        total = 0
        for name, pattern, what in DUPLICATE_SEARCHES:
            pattern = pattern or names_pattern
            hits = self.grep(["-E", pattern], product)
            total += len(hits)
            search += ["## %s: %s" % (name, what), "# pattern: " + pattern, "# hits: %d" % len(hits)]
            search += [hit[:220] for hit in hits] + [""]
        search.append("# hits in all four parts: %d" % total)
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
        counted = ["# SJL-10.6 lines added and removed in every C# file between the base %s and the after commit %s,"
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
        Native AOT, by the after tree's own eng/collect-js-packages.py, as bundle sjl-8-001 took them."""
        self.into("after")
        out = os.path.join(self.out, "packages")
        if os.path.exists(out):
            print("packages: %s exists; the step writes it once" % out)
            return 1
        code, output = self.run([sys.executable, os.path.join("eng", "collect-js-packages.py"),
                                 "--bundle", "SJL-10-001", "--out", out])
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
    # The earlier collectors' constructors read these; no step uses them.
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
