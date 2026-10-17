#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Collect Broiler.VM's evidence for milestone SJL-7 (the Unicode area) into bundle sjl-7-001.

    python3 docs/evidence/sjl-7-001/collect-area.py --tree <a clean checkout> --out docs/evidence/sjl-7-001 \
        [--base-tree <a clean checkout of the base commit>] [--suite <the pinned test262 revision, extracted>] \
        [--artifact-zip <a run's test262-evidence artifact> --run-url <its workflow run, with its attempt>
         --artifact-id <its id> --run-times <when it was dispatched and completed>] [--dry-run] \
        --only STEP [--only STEP ...]

THE RULE IS decision-rule-area.md BESIDE THIS FILE, committed with the area verdict JSD-0074 before any code
of SJL-7. Its part A takes the base and after runs with .github/workflows/test262-evidence.yml, on GitHub's
runners, at the commit that carries the rule; its part B takes the fuel-parity base locally, from a clean
checkout of the same commit; its part C is a measurement whose base arm is built from that commit when the
after arm exists, at SJL-7.6.

IT REUSES THE BUNDLES BEFORE IT, so that each arm runs the code an earlier arm ran: bundle sjl-6-001's
collect-area.py, and through it that bundle's collect.py (the import of a hosted run and its completeness)
and bundle sjl-5-001's collector (the twin bisection), over the wide twins and the Unicode probes the rule
adds (src/tests/forms/unicode). Nothing of theirs is changed; this file names this bundle's rule, its folders
and its baseline commit.

THE COLLECTOR JUDGES NOTHING. Each step writes what it ran and what it printed, failures included; the
README reads them under the rule.

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
  manifest       the SHA-256 of every file this collector writes or reads as its own (base/, after/, cutover/,
                 vectors/, this file, the rule, its revision of 2026-10-17 and the correspondence), with the base
                 commit base/identity.txt names and the after commit after/identity.txt names, into
                 area-manifest.json. measurement/ and measurement-revised/ are each a sub-bundle of its own
                 evidence class, with its own manifest and hashes, and are not listed.
  cutover-closures
                 every JavaScript composition root under JIT, trimming and Native AOT, as bundle sjl-3-001's
                 closures step publishes and runs them, at the tree of SJL-7.4's change that adds the Unicode
                 area's package references, into cutover/closures/, with cutover/identity.txt naming the
                 commit and the digest of its product sources (added at SJL-7.4, for rules K3, K4 and I7, which
                 read the closures a composition register row's evidence cell names).

THE AFTER STEPS WERE ADDED AT SJL-7.6, when there was an after commit to run them at, as bundle sjl-6-001's
were at SJL-6.6, and they run that bundle's code wherever it has the step. They write into after/ and read
the base from base/. `--tree` is then a clean checkout of a commit whose product sources are the cut-over's
(after-identity lists every file outside the documents that differs from it), and `--base-tree` one of the
base commit, which only rerun, wall-race and measure read. Part A's after run is a dispatch of the workflow,
imported as the base's was; every other step runs locally:

  after-identity  the after commit, its cleanliness, the cut-over commit it carries, every file outside the
                  documents that differs from it and every commit since it, the base it is compared with, the
                  rule's digest, the SDK and the machine, into after/identity.txt;
  after-import    the after dispatch's artifact test262-evidence, every file unchanged, into after/hosted/, as
                  base-import does (bundle sjl-6-001's step);
  after-complete  each hosted form's completeness, read off after/hosted/, into after/completeness.log
                  (bundle sjl-6-001's step);
  after-twins     every twin and probe of base/fuel-parity.log, at its base figure and one less, in both forms,
                  with the answer it completes with beside the base's, into after/fuel-parity.log (bundle
                  sjl-4-001's step, which sjl-5-001 and sjl-6-001 ran);
  compare         each form's after report against the base's, both as imported (base/hosted/, after/hosted/),
                  through the after tree's eng/compare-test262-forms.py --same-form, the native pair with
                  --exempt-guest-loads over the extracted suite, into after/compare-<run>.log (bundle
                  sjl-6-001's step);
  rerun           part A's class C4 (c'): every path of a (c') row run again alone at --base-tree and at --tree,
                  into after/rerun-<run>.* (bundle sjl-4-001's step);
  wall-race       part A's class C4 (e): every (c') row whose re-runs differ, run alone once more at each commit
                  with ten times the wall-clock allowance, into after/wall-race-<run>.* (bundle sjl-5-001's
                  step);
  closures        every JavaScript composition root under JIT, trimming and Native AOT, into after/closures/
                  (bundle sjl-3-001's step), and the SHA-256 of each Native AOT image record it leaves there, into
                  after/closures/mstat.sha256; the records themselves are not retained in the bundle;
  rules           the whole architecture suite at --tree, rule I3 reading the image records closures left (or
                  --images), into after/rules.log (bundle sjl-3-001's step);
  simplification  every member rule I5 registers, searched as a declaration in the JavaScript family's sources,
                  and the search for a surviving duplicate of the Unicode area in the whole component's product
                  sources - the platform's Unicode classification, casing, normalization and text elements, the
                  area's algorithms by their names and constants, and tables of code-point ranges - into
                  after/simplification/;
  packages        eng/collect-js-packages.py at --tree, into after/packages/ (bundle sjl-6-001's step);
  measure         part C, the measurement: the command-line root built in Release at --base-tree (arm B, the base
                  commit base/identity.txt names) and at --tree (arm A, the after commit after/identity.txt
                  names; A-again is arm A's binaries measured a second time under another lane name); the
                  sub-bundle's manifest, naming every binary by hash, written and made read-only before any lane
                  runs; then, for each form and each of the three workloads of src/tests/forms/unicode, every
                  arm's answer for the workload and its control checked before the repetitions, three warm-up
                  repetitions (eng/compare-forms.py's default, retained and named as excluded from Q) and seven
                  retained (docs/baselines.md rule 4), each repetition running every arm's workload and control
                  with the arms' order rotating by repetition, every child run with --runtime and its
                  effective-configuration line kept, then every arm's answer checked again; into measurement/,
                  a sub-bundle of its own (its README is written by hand). A measurement/ holding a summary is
                  never run again; one without (a stopped run) is moved aside to measurement-stopped-<n>/ and
                  retained. With --dry-run it checks the arms and prints the plan, and builds and runs nothing.

THE REVISED MEASUREMENT WAS ADDED ON 2026-10-17, after measure's run was seen, with
decision-rule-area-revised-2026-10-17.md, which revises part C's arm A and nothing else:

  measure-revised part C under the revised rule, as measure takes it, with arm A at --tree: a clean checkout of a
                  commit that carries the revised rule, descends from 9508bd5 (this family on Broiler.Unicode
                  0.1.0-preview.5) and differs from it in no file outside the documents; arm B is the base commit
                  base/identity.txt names, as before; the sub-bundle's manifest also names every commit since the
                  after commit after/identity.txt names, with its files outside the documents; into
                  measurement-revised/, which measure's rules for a completed or stopped run govern as they govern
                  measurement/.
"""

import argparse
import datetime
import hashlib
import importlib.util
import json
import os
import platform
import re
import shutil
import stat
import statistics
import subprocess
import sys
import time

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
# SJL-7.4's cut-over (JSD-0075): its code landed in 093feb8, 0271008, b0ac6e2 and 8aa88db, its record in d4f7d54,
# and 8e9e411 re-pinned the committed feed to Broiler.Unicode 0.1.0-preview.4 as published. The after runs are
# taken at a commit whose product sources are this one's.
CUT_OVER = "8e9e411a7fc8d460f8ab12af797c1910e3ae0a4c"
CUT_OVER_NOTE = ("SJL-7.4's cut-over, JSD-0075: its code in 093feb8, 0271008, b0ac6e2 and 8aa88db, its record in"
                 " d4f7d54, and the committed feed re-pinned to Broiler.Unicode 0.1.0-preview.4 as published in"
                 " 8e9e411")

# The search for a surviving duplicate of the Unicode area, in three parts. HOST: the platform's Unicode
# classification, casing, normalization and text elements (JSD-0074's X6 names char.IsLetter, IsWhiteSpace,
# IsLetterOrDigit, GetUnicodeCategory and CharUnicodeInfo among the family's; Broiler.JS's are
# string.Normalize, IsNormalized, StringInfo and the culture's casing). The UTF-16 tests (IsSurrogate and its
# kin) are the text area's, and the ASCII tests (IsAscii*) read no Unicode data, so neither is searched.
HOST_PATTERN = (r'\b(char|Char|Rune)\.(IsLetter|IsLetterOrDigit|IsDigit|IsNumber|IsWhiteSpace|IsUpper|IsLower'
                r'|IsPunctuation|IsSymbol|IsSeparator|IsControl|GetUnicodeCategory|GetNumericValue|ToUpper|ToLower'
                r'|ToUpperInvariant|ToLowerInvariant)\(|\bCharUnicodeInfo\b|\bUnicodeCategory\b|\.Normalize\('
                r'|\.IsNormalized\(|\bNormalizationForm\b|\bStringInfo\b|\bTextInfo\b|\bTextElementEnumerator\b'
                r'|\.To(Upper|Lower)(Invariant)?\(')
# ALGORITHM: the area's algorithms by their names and constants - Hangul composition's constants, the casing
# contexts, the combining classes and decompositions, the break properties, the identifier and pattern
# properties, and the UCD files a hand-written copy would cite.
ALGORITHM_PATTERN = (r'\b0x(AC00|D7A3|1161|11A7)\b|\b(SBase|LBase|VBase|TBase|SCount|LCount|VCount|NCount|TCount)\b'
                     r'|\bHangul|\bFinal_?Sigma|\bSoft_?Dotted|\bMore_?Above|\bBefore_?Dot'
                     r'|\bAfter_?(Capital_?I|Soft_?Dotted)|\bCombiningClass|\bCanonical_?Combining|\b[Dd]ecompos'
                     r'|\b[Rr]ecompos|\bCompose(Canonically|Pair)|\bGrapheme_?Break|\bWord_?Break|\bSentence_?Break'
                     r'|\bExtended_?Pictographic|\bRegional_?Indicator|\b(Other_)?ID_(Start|Continue)\b'
                     r'|Pattern_?(Syntax|White_?Space)|Default_?Ignorable|\bSpecialCasing|\bCaseFolding\.txt'
                     r'|\bUnicodeData\.txt')
# RANGES: a table of code-point ranges, as pairs of hexadecimal code points, or a range test over code points
# above ASCII written inline, in hexadecimal or as a \u character literal.
_CODE_POINT = r"(0x[0-9A-Fa-f]{3,6}|'\\u[0-9A-Fa-f]{4}')"
RANGE_PATTERN = (r'[({]\s*0x[0-9A-Fa-f]{4,6}\s*,\s*0x[0-9A-Fa-f]{4,6}\s*[)}]'
                 r'|>=?\s*' + _CODE_POINT + r'\s*(&&|and)[^;]*<=?\s*' + _CODE_POINT)
DUPLICATE_SEARCHES = (
    ("host", HOST_PATTERN, "the platform's Unicode classification, casing, normalization and text elements"),
    ("algorithm", ALGORITHM_PATTERN, "the area's algorithms by their names and constants"),
    ("ranges", RANGE_PATTERN, "tables of code-point ranges and inline range tests above ASCII"),
)

# Part C (decision-rule-area.md): the workloads and their controls, the arms, and the counts the rule cites.
WORKLOADS = os.path.join("src", "tests", "forms", "unicode")
WORKLOAD_STEMS = ("normalize", "casing", "identifiers")
# Each workload's control is the file of its stem with this suffix beside it.
CONTROL = ".control.js"
ARMS = ("B", "A", "A-again")
FORMS = ("bytecode", "native")
MEASUREMENT = "measurement"
# The revised rule of 2026-10-17: part C again, arm A at a commit whose product sources are those of the default
# branch after #138, this family on Broiler.Unicode 0.1.0-preview.5, into a sub-bundle of its own.
REVISED_RULE = os.path.join("docs", "evidence", "sjl-7-001", "decision-rule-area-revised-2026-10-17.md")
PREVIEW_5 = "9508bd5b9b6d0f6d009418dda351fb0e7d846745"
MEASUREMENT_REVISED = "measurement-revised"

STEPS = ("base-identity", "base-import", "base-complete", "base-twins",
         "after-identity", "after-import", "after-complete", "after-twins", "compare", "rerun", "wall-race",
         "closures", "rules", "simplification", "packages", "measure", "measure-revised", "manifest",
         "cutover-closures")


def outside_documents(names):
    """The paths of a `git diff --name-only` listing outside docs/ and outside any docs/ directory."""
    return [name for name in names.splitlines() if name and not name.startswith("docs/") and "/docs/" not in name]


def recorded(path, key):
    """A `<key> <value>` line of an identity file, or None."""
    if not os.path.isfile(path):
        return None
    for line in open(path, encoding="utf-8"):
        if line.startswith(key + " "):
            return line.split(" ", 1)[1].strip().split(" ", 1)[0]
    return None


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

    def cutover_closures(self):
        """Every JavaScript root published and run in three modes, by bundle sjl-3-001's closures step, into
        cutover/closures/, beside cutover/identity.txt."""
        self.into("cutover")
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        status = self.run(["git", "status", "--porcelain"])[1]
        listing = self.run(["git", "ls-tree", "-r", "HEAD"])[1]
        product = [line for line in listing.splitlines()
                   if not line.split("\t", 1)[-1].startswith("docs/") and "/docs/" not in line.split("\t", 1)[-1]]
        digest = hashlib.sha256(("\n".join(product) + "\n").encode("utf-8")).hexdigest()
        lines = [
            "bundle sjl-7-001, the closures at SJL-7.4's cut-over",
            "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "commit " + commit,
            "tree " + ("clean" if not status.strip() else "dirty"),
            "product-sources sha256=%s over %d entries of `git ls-tree -r`, every path outside docs/ and outside any"
            " docs/ directory" % (digest, len(product)),
            "sdk " + self.run(["dotnet", "--version"])[1].strip(),
            "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
        ]
        self.write("identity.txt", "\n".join(lines) + "\n")
        print("cutover-closures: %s (%s tree)" % (commit, "clean" if not status.strip() else "DIRTY"), flush=True)
        return area6.text.numerics.prior.Collector.closures(self) | (0 if not status.strip() else 1)

    # ---- the after arm (SJL-7.6) --------------------------------------------------------------
    def after_identity(self):
        self.into("after")
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        status = self.run(["git", "status", "--porcelain"])[1]
        base_commit = recorded(os.path.join(self.root_out, "base", "identity.txt"), "base-commit") or "(absent)"

        def outside_documents(names):
            return [name for name in names.splitlines()
                    if name and not name.startswith("docs/") and "/docs/" not in name]

        code, names = self.run(["git", "diff", "--name-only", CUT_OVER, commit])
        changed = outside_documents(names)
        ancestry = self.run(["git", "merge-base", "--is-ancestor", CUT_OVER, commit])[0]
        commits = self.run(["git", "log", "--reverse", "--format=%H%x09%s", "%s..%s" % (CUT_OVER, commit)])[1]
        rule = os.path.join(self.tree, RULE)
        lines = [
            "bundle sjl-7-001, the Unicode area's after arm",
            "milestone SJL-7, work package SJL-7.6, Broiler.VM's half",
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
            touched = outside_documents(self.run(["git", "diff-tree", "--no-commit-id", "--name-only", "-r", sha])[1])
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

    def closures(self):
        """Bundle sjl-3-001's closures step into after/closures/, and the digest of each Native AOT image
        record it leaves there, which rules reads and the bundle does not retain."""
        verdict = super().closures()
        folder = os.path.join(self.out, "closures")
        records = sorted(name for name in os.listdir(folder) if name.endswith(".mstat")) if os.path.isdir(folder) else []
        with open(os.path.join(folder, "mstat.sha256"), "w", encoding="utf-8", newline="\n") as stream:
            stream.write("".join("%s  %s\n" % (self.sha256(os.path.join(folder, name)), name) for name in records))
        print("closures: %d image records hashed into after/closures/mstat.sha256" % len(records), flush=True)
        return verdict

    def simplification(self):
        self.into("after")
        os.makedirs(os.path.join(self.out, "simplification"), exist_ok=True)
        after = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        family = [p for p in sorted(os.listdir(os.path.join(self.tree, "src")))
                  if p.startswith("Broiler.VM.Profile.JavaScript")]
        family_paths = [os.path.join("src", p, "*.cs") for p in family]
        retired = area6.text.numerics.prior.RETIRED
        register = json.load(open(os.path.join(self.tree, retired), encoding="utf-8"))["retired"]
        lines = ["# SJL-7.6 deletion check, Broiler.VM at %s: every member rule I5 registers (%s), its simple"
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
        search = ["# SJL-7.6 search for a surviving duplicate of the Unicode area, Broiler.VM at %s, in the" % after,
                  "# component's C# product sources (src/, every tests/ excluded), in three parts, each with its",
                  "# pattern and its hits; every hit is reviewed in the bundle's README.", ""]
        total = 0
        for name, pattern, what in DUPLICATE_SEARCHES:
            hits = self.grep(["-E", pattern], product)
            total += len(hits)
            search += ["## %s: %s" % (name, what), "# pattern: " + pattern, "# hits: %d" % len(hits)]
            search += [hit[:220] for hit in hits] + [""]
        search.append("# hits in all three parts: %d" % total)
        self.write(os.path.join("simplification", "duplicate-search.log"), "\n".join(search) + "\n")
        print("simplification: %d registered members searched, %d duplicate-search hits" % (len(register), total),
              flush=True)
        return 0

    def packages(self):
        """The candidate pack, the pristine consumer, the rollback, the negative control and the consumer as
        Native AOT, by the after tree's own eng/collect-js-packages.py, as bundle sjl-6-001 took them."""
        self.into("after")
        out = os.path.join(self.out, "packages")
        if os.path.exists(out):
            print("packages: %s exists; the step writes it once" % out)
            return 1
        code, output = self.run([sys.executable, os.path.join("eng", "collect-js-packages.py"),
                                 "--bundle", "SJL-7-001", "--out", out])
        self.write("packages.log", output.strip() + "\nexit code: %d\n" % code)
        print("packages: exit %d" % code, flush=True)
        return code

    # ---- part C, the measurement (SJL-7.6), and its revision of 2026-10-17 ----------------------
    def measure(self):
        """Part C of decision-rule-area.md."""
        return self.part_c("measure", RULE, MEASUREMENT, revised=False)

    def measure_revised(self):
        """Part C of decision-rule-area-revised-2026-10-17.md: arm A at --tree, on 0.1.0-preview.5."""
        return self.part_c("measure-revised", REVISED_RULE, MEASUREMENT_REVISED, revised=True)

    def part_c(self, step, rule, subbundle, revised):
        """Every check that can refuse runs before anything is built; with --dry-run nothing is built, written
        or run."""
        dry = bool(getattr(self.arguments, "dry_run", False))
        problems = []
        if not self.base_tree:
            print("%s: arm B needs --base-tree, a clean checkout of the base commit" % step)
            return 1
        after = recorded(os.path.join(self.root_out, "after", "identity.txt"), "after-commit")
        expected = {"B": recorded(os.path.join(self.root_out, "base", "identity.txt"), "base-commit"),
                    "A": after}
        trees = {"B": self.base_tree, "A": self.tree}
        commits = {}
        for arm in ("B", "A"):
            commits[arm] = self.run(["git", "rev-parse", "HEAD"], cwd=trees[arm])[1].strip()
            dirty = self.run(["git", "status", "--porcelain"], cwd=trees[arm])[1].strip()
            if revised and arm == "A":
                pass
            elif not expected[arm]:
                problems.append("arm %s: the bundle names no commit for it (%s/identity.txt)"
                                % (arm, "base" if arm == "B" else "after"))
            elif commits[arm] != expected[arm]:
                problems.append("arm %s: the tree is at %s, and the bundle names %s" % (arm, commits[arm], expected[arm]))
            if dirty:
                problems.append("arm %s: the tree is not clean" % arm)
        # Under the revised rule, arm A is the commit that carries it, whose product sources are PREVIEW_5's.
        since = []
        if revised:
            if not os.path.isfile(os.path.join(self.tree, REVISED_RULE)):
                problems.append("arm A: the tree does not carry %s" % REVISED_RULE.replace(os.sep, "/"))
            if self.run(["git", "merge-base", "--is-ancestor", PREVIEW_5, commits["A"]], cwd=self.tree)[0] != 0:
                problems.append("arm A: %s does not descend from %s" % (commits["A"], PREVIEW_5))
            code, names = self.run(["git", "diff", "--name-only", PREVIEW_5, commits["A"]], cwd=self.tree)
            differ = outside_documents(names)
            if code != 0 or differ:
                problems.append("arm A: %s differs from %s outside the documents: %s"
                                % (commits["A"], PREVIEW_5, ", ".join(differ) or "git diff exit %d" % code))
            if not after:
                problems.append("the bundle names no after commit (after/identity.txt)")
            else:
                listing = self.run(["git", "log", "--reverse", "--format=%H%x09%s", "%s..%s" % (after, commits["A"])],
                                   cwd=self.tree)[1]
                for entry in listing.splitlines():
                    if entry.strip():
                        sha, subject = entry.split("\t", 1)
                        touched = outside_documents(self.run(
                            ["git", "diff-tree", "--no-commit-id", "--name-only", "-r", sha], cwd=self.tree)[1])
                        since.append("  %s %s: %s" % (sha, subject, (str(len(touched)) + " files") if touched else "none"))
                        since.extend("    " + name for name in touched)
        # The workloads are the rule's, as committed with it: read from the base tree, and equal in both.
        folder = os.path.join(self.base_tree, WORKLOADS)
        workloads = []
        for stem in WORKLOAD_STEMS:
            for name in (stem + ".js", stem + CONTROL):
                paths = [os.path.join(trees[arm], WORKLOADS, name) for arm in ("B", "A")]
                if not all(os.path.isfile(path) for path in paths):
                    problems.append("workload %s is missing from a tree" % name)
                    continue
                digests = [self.sha256_lf(path) for path in paths]
                if digests[0] != digests[1]:
                    problems.append("workload %s differs between the arms' trees" % name)
                workloads.append((name, digests[0]))
        # The counts the rule cites, read where it cites them.
        compare_forms = open(os.path.join(self.tree, "eng", "compare-forms.py"), encoding="utf-8").read()
        warmup = re.search(r'"--warmup", type=int, default=(\d+)', compare_forms)
        baselines = open(os.path.join(self.tree, "docs", "baselines.md"), encoding="utf-8").read()
        seven = re.search(r"\| 4 \| \*\*Every repetition retained\.\*\* Seven per lane", baselines)
        if not warmup or int(warmup.group(1)) != base.WARMUP:
            problems.append("eng/compare-forms.py's warm-up default is not %d" % base.WARMUP)
        if not seven or base.REPETITIONS != 7:
            problems.append("docs/baselines.md rule 4 does not read seven per lane")
        out = os.path.join(self.root_out, subbundle)
        stopped = None
        if os.path.isdir(out) and os.listdir(out):
            if os.path.isfile(os.path.join(out, "summary.txt")):
                problems.append("%s/ holds a completed measurement; a measurement is not run again" % subbundle)
            else:
                number = 1
                while os.path.exists(os.path.join(self.root_out, "%s-stopped-%d" % (subbundle, number))):
                    number += 1
                stopped = os.path.join(self.root_out, "%s-stopped-%d" % (subbundle, number))
        children = len(FORMS) * len(WORKLOAD_STEMS) * len(ARMS) * 2 * (2 + base.WARMUP + base.REPETITIONS)
        plan = [
            "%s: part C of %s" % (step, rule.replace(os.sep, "/")),
            "  arm B        %s at %s (the bundle names %s)" % (self.hide(trees["B"]), commits["B"], expected["B"]),
            "  arm A        %s at %s (%s)" % (self.hide(trees["A"]), commits["A"], ("the bundle names %s" % expected["A"])
                                         if not revised else "the commit carrying the revised rule, on %s's product sources"
                                         % PREVIEW_5),
            "  arm A-again  arm A's binaries, measured a second time under another lane name",
            "  root         %s, built in Release under the JIT in each arm's tree" % base.CLI.replace(os.sep, "/"),
            "  forms        %s; native under %s" % (", ".join(FORMS), base.NATIVE_BACKEND),
            "  workloads    %s" % ", ".join(name for name, _ in workloads),
            "  repetitions  %d warm-up (excluded from Q) and %d retained per lane; the arms' order rotates by"
            " repetition, and each arm runs the workload, then its control" % (base.WARMUP, base.REPETITIONS),
            "  checks       every arm's answer for the workload and its control, before and after the repetitions",
            "  allowance    fuel=%d wallClockMs=%d, the same for every lane" % (base.MEASURE_FUEL, base.MEASURE_WALL),
            "  child        <arm>/%s --runtime [--native %s] --fuel %d --wall %d <workload>"
            % (base.CLI_BINARY, base.NATIVE_BACKEND, base.MEASURE_FUEL, base.MEASURE_WALL),
            "  children     %d timed or checked, plus two builds" % children,
            "  into         %s/%s" % (self.hide(self.root_out), subbundle)
            + ("; the stopped run there moves to %s" % os.path.basename(stopped) if stopped else ""),
        ]
        print("\n".join(plan), flush=True)
        for problem in problems:
            print("%s: REFUSED: %s" % (step, problem), flush=True)
        if problems:
            return 1
        if dry:
            print("%s: dry run; nothing was built, written or run" % step, flush=True)
            return 0

        if stopped:
            os.rename(out, stopped)
            print("%s: the stopped run is retained as %s" % (step, os.path.basename(stopped)), flush=True)
        os.makedirs(out, exist_ok=True)
        binaries, listings = {}, {}
        for arm in ("B", "A"):
            tree = trees[arm]
            project = os.path.join(tree, base.CLI, os.path.basename(base.CLI) + ".csproj")
            code, output = self.run(["dotnet", "build", project, "-c", "Release"], cwd=tree)
            with open(os.path.join(out, "build-cli-%s.log" % arm.lower()), "w", encoding="utf-8", newline="\n") as stream:
                stream.write(self.hide(output.strip() + "\nexit code: %d\n" % code))
            if code != 0:
                print("%s: the command-line root did not build for arm %s" % (step, arm))
                return 1
            copy = os.path.join(self.scratch, "cli-" + arm)
            shutil.copytree(os.path.join(tree, base.CLI, "bin", "Release", "net10.0"), copy, dirs_exist_ok=True)
            binaries[arm] = os.path.join(copy, base.CLI_BINARY)
            entries = []
            for root, _, names in os.walk(copy):
                for name in names:
                    path = os.path.join(root, name)
                    entries.append("%s  %s" % (self.sha256(path), os.path.relpath(path, copy).replace(os.sep, "/")))
            listing = "\n".join(sorted(entries, key=lambda line: line.split("  ", 1)[1])) + "\n"
            path = os.path.join(out, "binaries-%s.sha256" % arm.lower())
            with open(path, "w", encoding="utf-8", newline="\n") as stream:
                stream.write(listing)
            os.chmod(path, stat.S_IREAD | stat.S_IRGRP | stat.S_IROTH)
            listings[arm] = hashlib.sha256(listing.encode("utf-8")).hexdigest()
        binaries["A-again"] = binaries["A"]

        manifest = [
            "sub-bundle sjl-7-001/%s" % subbundle,
            "evidence-class measurement",
            "decision %s part C, as %s: MET if and only if every lane's condition passes, before and after, for"
            " every workload and form; every effective-configuration line is identical across arms, lanes and"
            " repetitions; and for every workload and form D <= N." % (rule.replace(os.sep, "/"),
                                                                       "revised" if revised else "written"),
            "rule %s sha256=%s" % (rule.replace(os.sep, "/"), self.sha256_lf(os.path.join(self.tree, rule))),
            "collector docs/evidence/sjl-7-001/collect-area.py sha256=%s" % self.sha256_lf(os.path.abspath(__file__)),
            "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "arm-B-commit %s (the base commit base/identity.txt names; clean tree)" % commits["B"],
            ("arm-A-commit %s (the after commit after/identity.txt names; clean tree)" % commits["A"]) if not revised
            else ("arm-A-commit %s (it carries the revised rule, descends from %s and differs from it in no file"
                  " outside the documents; clean tree)" % (commits["A"], PREVIEW_5)),
            "arm-A-again the arm A binaries, measured a second time under another lane name",
        ]
        if revised:
            manifest[4:4] = ["rule-it-revises %s sha256=%s" % (RULE.replace(os.sep, "/"),
                                                              self.sha256_lf(os.path.join(self.tree, RULE)))]
            manifest.append("since-after-commit %s (after/identity.txt), each commit with its files outside the"
                            " documents:" % after)
            manifest.extend(since)
        for arm in ("B", "A"):
            manifest.append("binary arm %s %s.dll sha256=%s; every file of its output, binaries-%s.sha256 sha256=%s" % (
                arm, base.CLI_BINARY, self.sha256(binaries[arm] + ".dll"), arm.lower(), listings[arm]))
        manifest += [
            "root %s, Release, under the JIT" % base.CLI.replace(os.sep, "/"),
            "rid " + ("win-x64" if platform.system() == "Windows" else "linux-x64"),
            "sdk " + self.run(["dotnet", "--version"])[1].strip(),
            "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
            "repetitions %d retained per lane (docs/baselines.md rule 4), %d warm-up excluded from Q"
            " (eng/compare-forms.py's default)" % (base.REPETITIONS, base.WARMUP),
            "order in each repetition r (from 0): the arms rotated by r from B, A, A-again; each arm runs the"
            " workload, then its control",
            "allowance fuel=%d wallClockMs=%d" % (base.MEASURE_FUEL, base.MEASURE_WALL),
            "forms " + " ".join(FORMS),
            "native-backend " + base.NATIVE_BACKEND,
            "configuration-check every child is run with --runtime; its line starting %s is kept" % base.RUNTIME_PREFIX,
        ]
        for name, digest in workloads:
            blob = self.run(["git", "rev-parse", "%s:%s" % (commits["B"], (WORKLOADS + "/" + name).replace(os.sep, "/"))],
                            cwd=self.base_tree)[1].strip()
            manifest.append("workload %s/%s sha256=%s blob=%s (identical in both arms' trees)"
                            % (WORKLOADS.replace(os.sep, "/"), name, digest, blob))
        path = os.path.join(out, "manifest.txt")
        with open(path, "w", encoding="utf-8", newline="\n") as stream:
            stream.write(self.hide("\n".join(manifest) + "\n"))
        os.chmod(path, stat.S_IREAD | stat.S_IRGRP | stat.S_IROTH)
        print("%s: manifest written and made read-only; the lanes start" % step, flush=True)

        environment = dict(os.environ)
        environment["DOTNET_CLI_UI_LANGUAGE"] = "en"
        configurations = []

        def child(arm, form, name):
            command = [binaries[arm], "--runtime"]
            if form == "native":
                command += ["--native", base.NATIVE_BACKEND]
            command += ["--fuel", str(base.MEASURE_FUEL), "--wall", str(base.MEASURE_WALL), os.path.join(folder, name)]
            started = time.perf_counter()
            done = subprocess.run(command, capture_output=True, encoding="utf-8", errors="replace", env=environment)
            elapsed = (time.perf_counter() - started) * 1000.0
            answer = (done.stdout or "").strip().splitlines()
            runtime = next((line.strip() for line in (done.stderr or "").splitlines()
                            if line.startswith(base.RUNTIME_PREFIX)), "(none)")
            if runtime not in configurations:
                configurations.append(runtime)
            return done.returncode, answer[-1] if answer else "", configurations.index(runtime) + 1, elapsed

        runs = open(os.path.join(out, "runs.tsv"), "w", encoding="utf-8", newline="\n")
        conditions = open(os.path.join(out, "conditions.tsv"), "w", encoding="utf-8", newline="\n")
        runs.write("form\tworkload\trepetition\tretained\tposition\tarm\tfile\texit\tanswer-ok\tconfiguration\twall-ms\n")
        conditions.write("form\tworkload\tarm\tfile\twhen\texit\tanswer\tconfiguration\tequals arm B's\n")
        summary = ["# part C of %s, measured %s to %%s" % (rule.replace(os.sep, "/"),
                   datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")),
                   "# Q is the median of the retained repetitions, in ms; N = |Q(A) - Q(A-again)|; D = Q(A) - Q(B);",
                   "# E(arm) = Q(workload, arm) - Q(control, arm), retained beside D and deciding nothing",
                   "# form | workload | Q(B) | Q(A) | Q(A-again) | N | D | D <= N | Q(control, B) | Q(control, A) |"
                   " Q(control, A-again) | E(B) | E(A) | E(A-again)"]
        failed_conditions = failed_runs = 0
        within = 0
        try:
            for form in FORMS:
                for stem in WORKLOAD_STEMS:
                    files = (stem + ".js", stem + CONTROL)
                    answers = {}

                    def check(when):
                        failures = 0
                        for arm in ARMS:
                            for name in files:
                                code, answer, configuration, _ = child(arm, form, name)
                                if arm == "B" and when == "before":
                                    answers[name] = answer if code == 0 else None
                                equal = code == 0 and answers.get(name) is not None and answer == answers[name]
                                failures += 0 if equal else 1
                                conditions.write("%s\t%s\t%s\t%s\t%s\t%d\t%s\t%d\t%s\n" % (
                                    form, stem, arm, name, when, code, answer, configuration, "yes" if equal else "NO"))
                                conditions.flush()
                        return failures

                    failed_conditions += check("before")
                    times = {(arm, name): [] for arm in ARMS for name in files}
                    for repetition in range(base.WARMUP + base.REPETITIONS):
                        kept = repetition >= base.WARMUP
                        order = ARMS[repetition % len(ARMS):] + ARMS[:repetition % len(ARMS)]
                        position = 0
                        for arm in order:
                            for name in files:
                                position += 1
                                code, answer, configuration, elapsed = child(arm, form, name)
                                ok = code == 0 and answers.get(name) is not None and answer == answers[name]
                                failed_runs += 0 if ok else 1
                                runs.write("%s\t%s\t%d\t%s\t%d\t%s\t%s\t%d\t%s\t%d\t%.3f\n" % (
                                    form, stem, repetition, "yes" if kept else "warm-up", position, arm, name, code,
                                    "yes" if ok else "NO", configuration, elapsed))
                                runs.flush()
                                if kept:
                                    times[(arm, name)].append(elapsed)
                    failed_conditions += check("after")
                    q = {key: statistics.median(values) for key, values in times.items()}
                    workload, control = files
                    floor = abs(q[("A", workload)] - q[("A-again", workload)])
                    difference = q[("A", workload)] - q[("B", workload)]
                    within += difference <= floor
                    summary.append(" | ".join([form, stem] + ["%.3f" % value for value in (
                        q[("B", workload)], q[("A", workload)], q[("A-again", workload)], floor, difference)]
                        + ["yes" if difference <= floor else "NO"] + ["%.3f" % value for value in (
                            q[("B", control)], q[("A", control)], q[("A-again", control)],
                            q[("B", workload)] - q[("B", control)], q[("A", workload)] - q[("A", control)],
                            q[("A-again", workload)] - q[("A-again", control)])]))
                    print("%s: %s %s D=%.3f N=%.3f" % (step, form, stem, difference, floor), flush=True)
        finally:
            runs.close()
            conditions.close()
        summary[0] = summary[0] % datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")
        pairs = len(FORMS) * len(WORKLOAD_STEMS)
        summary += [
            "# answer checks (conditions.tsv) that differ from arm B's: %d; timed children that do: %d"
            % (failed_conditions, failed_runs),
            "# workloads and forms with D <= N: %d of %d" % (within, pairs),
            "# effective-configuration lines seen: %d (runs.tsv and conditions.tsv name each by its number)"
            % len(configurations),
        ]
        summary.extend("#   %d: %s" % (number, line) for number, line in enumerate(configurations, 1))
        with open(os.path.join(out, "summary.txt"), "w", encoding="utf-8", newline="\n") as stream:
            stream.write(self.hide("\n".join(summary) + "\n"))
        lines = ["%s  %s" % (self.sha256_lf(os.path.join(out, entry)), entry)
                 for entry in sorted(os.listdir(out)) if entry not in ("hashes.txt", "README.md")]
        with open(os.path.join(out, "hashes.txt"), "w", encoding="utf-8", newline="\n") as stream:
            stream.write("\n".join(lines) + "\n")
        print("%s: %d answer checks differ, %d timed children differ, %d configuration lines, D <= N in %d of %d"
              % (step, failed_conditions, failed_runs, len(configurations), within, pairs), flush=True)
        return 0 if failed_conditions == 0 and failed_runs == 0 and len(configurations) == 1 else 1

    def manifest(self):
        files = {}
        for folder in ("base", "after", "cutover", "vectors"):
            for root, _, names in os.walk(os.path.join(self.root_out, folder)):
                for name in names:
                    path = os.path.join(root, name)
                    if name.endswith(".mstat") or "__pycache__" in path:
                        continue
                    files[os.path.relpath(path, self.root_out).replace(os.sep, "/")] = self.sha256(path)
        for name in ("collect-area.py", "decision-rule-area.md", "decision-rule-area-revised-2026-10-17.md",
                     "correspondence.md"):
            if os.path.isfile(os.path.join(self.root_out, name)):
                files[name] = self.sha256(os.path.join(self.root_out, name))
        document = {"bundle": "sjl-7-001", "repository": "Broiler.VM", "part": "the Unicode area (SJL-7.2 onwards)",
                    "baseCommit": recorded(os.path.join(self.root_out, "base", "identity.txt"), "base-commit")
                    or "(absent)",
                    "afterCommit": recorded(os.path.join(self.root_out, "after", "identity.txt"), "after-commit")
                    or "(absent)",
                    "notListed": "measurement/ and measurement-revised/, each a sub-bundle of its own evidence class"
                                 " with its own manifest.txt and hashes.txt; any Native AOT image record (*.mstat), which"
                                 " after/closures/mstat.sha256 and cutover/closures/mstat.sha256 name",
                    "files": dict(sorted(files.items()))}
        with open(os.path.join(self.root_out, "area-manifest.json"), "w", encoding="utf-8", newline="\n") as stream:
            stream.write(json.dumps(document, indent=1) + "\n")
        print("manifest: %d files" % len(files), flush=True)
        return 0


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--tree", required=True, help="a clean checkout: the base commit, or the after commit")
    parser.add_argument("--base-tree", help="a clean checkout of the base commit, for rerun, wall-race and measure")
    parser.add_argument("--suite", help="an unpacked checkout of the pinned test262 revision")
    parser.add_argument("--images", help="the closures step's mstat records, for rule I3 (rules)")
    parser.add_argument("--out", required=True)
    parser.add_argument("--jobs", type=int, default=max(1, os.cpu_count() or 1))
    parser.add_argument("--artifact-zip")
    parser.add_argument("--run-url")
    parser.add_argument("--artifact-id")
    parser.add_argument("--run-times")
    parser.add_argument("--dry-run", action="store_true", help="measure, measure-revised: check the arms and print the plan only")
    parser.add_argument("--only", action="append", required=True, choices=STEPS)
    arguments = parser.parse_args()
    arguments.against_broiler_js = None
    arguments.against_root = None
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
