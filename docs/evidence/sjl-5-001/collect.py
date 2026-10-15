#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Collect Broiler.VM's evidence for milestone SJL-5 (the text area) as bundle sjl-5-001.

    python3 docs/evidence/sjl-5-001/collect.py --tree <a clean checkout> \
        [--base-tree <a clean checkout of the base commit>] \
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

THE AFTER STEPS WERE ADDED AT SJL-5.6, when there was an after commit to run them at. They write into
after/ and vectors/, read the base's runs from base/, and reuse bundle sjl-4-001's collector (and through
it sjl-3-001's) for the after commit's test262 runs, the twins at the base's figures, the comparisons, the
re-runs of class C4 (c'), the closures and the architecture suite, so that SJL-4's and SJL-5's after arms
run the same code. `--tree` is then a clean checkout of a commit whose product sources are the cut-over
commit's (after-identity lists any file outside the documents that differs from it), and `--base-tree`
one of the base commit, which only rerun and wall-race read.

  after-identity     the after commit, its cleanliness, the cut-over commit it carries and every file
                     outside the documents that differs from it, the base it is compared with, the rule's
                     and the harness's digests, the SDK and the machine, into after/identity.txt;
  after-test262      the whole pinned suite in both forms, through sjl-0-001's procedure, at --tree, into
                     after/;
  after-twins        every twin and probe of base/fuel-parity.log, at its base figure and one less, in both
                     forms, with the answer it completes with beside the base's, into after/fuel-parity.log;
  after-json-modules part C's harness, twice, at --tree, as base-json-modules runs it, into
                     after/json-modules-1.txt and -2.txt, and each document's line at the base and after set
                     side by side, with each commit's two runs compared, into after/json-modules-compare.txt;
  compare            each form's after report against the base's, through the tree's
                     eng/compare-test262-forms.py --same-form, the native pair with --exempt-guest-loads,
                     into after/compare-<run>.log;
  rerun              part A's class C4 (c'): every path of a (c') row run again alone at --base-tree and at
                     --tree, into after/rerun-<run>.*;
  wall-race          part A's class C4 (e): every (c') row whose re-runs differ (after/rerun-<run>.tsv) run
                     alone once more at each commit, with the part's fuel allowance and ten times its
                     wall-clock allowance, both reports retained, into after/wall-race-<run>.*;
  closures           every JavaScript composition root under JIT, trimming and Native AOT, into
                     after/closures/;
  rules              the whole architecture suite at --tree, into after/rules.log;
  simplification     every member rule I5 registers, searched as a declaration in the JavaScript family's
                     sources, and the search for a surviving duplicate of the text area in the whole
                     component's product sources, into after/simplification/;
  vectors            the conformance root over this family's pin of the shared corpus, and its negative
                     control, into vectors/after-revision-<n>.* and vectors/after-negative-control-revision-<n>.*.
"""

import argparse
import datetime
import gzip
import importlib.util
import json
import os
import platform
import re
import shutil
import stat
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
BASE_BUNDLE = os.path.join(os.path.dirname(HERE), "sjl-0-001")

_spec = importlib.util.spec_from_file_location("sjl_0_001_collect", os.path.join(BASE_BUNDLE, "collect.py"))
base = importlib.util.module_from_spec(_spec)
_spec.loader.exec_module(base)

# Bundle sjl-4-001's collector, whose after steps SJL-5's after arm reuses. It loads sjl-3-001's and
# sjl-0-001's itself.
_numerics_spec = importlib.util.spec_from_file_location(
    "sjl_4_001_collect", os.path.join(os.path.dirname(HERE), "sjl-4-001", "collect.py"))
numerics = importlib.util.module_from_spec(_numerics_spec)
_numerics_spec.loader.exec_module(numerics)

RULE = os.path.join("docs", "evidence", "sjl-5-001", "decision-rule.md")
HARNESS = os.path.join("docs", "evidence", "sjl-5-001", "json-modules.cs")
# The default branch after SJL-4's merge, whose product sources the rule says the base commit carries.
SJL_4_MERGE = "1f253ff7a6ba67051854c52b6e9a68a3e1f65f31"
TWIN_FOLDERS = [os.path.join("src", "tests", "forms", "wide"), os.path.join("src", "tests", "forms", "text")]
JSON_MODULES = os.path.join("src", "tests", "forms", "text", "json-modules")
# The commit that lands SJL-5.4's cut-over, whose product sources the after runs must be taken at.
CUT_OVER = "6614fa9eadcadae474518fd5a8ba8c46c1e54d7c"
VECTORS_PIN = os.path.join("src", "tests", "vectors", "pins", "vectors.pin")
# The control vector every run of this family's text vectors mutates, as SJL-5.2's did.
VECTORS_CONTROL = "text/trim/0001"
# Class C4 (e): the wall-clock allowance a (c') row whose re-runs differ is run with once more.
WALL_RACE_FACTOR = 10
# The search for a surviving duplicate of the text area: UTF-16 read or written by the platform or by
# the surrogate constants, padding by the platform, base64, hex and URI by the platform, JSON by
# System.Text.Json, a \uXXXX escape written by format string, and the line terminators and the byte
# order mark the whitespace set holds, spelled as code.
DUPLICATE_PATTERN = (r'char\.Is(High|Low)?Surrogate\(|char\.ConvertToUtf32\(|char\.ConvertFromUtf32\('
                     r'|0xD800|0xDC00|0xDBFF|0xDFFF|\.PadLeft\(|\.PadRight\('
                     r'|Convert\.(To|From|TryFrom)(Base64|Hex)|Uri\.(Escape|Unescape)DataString|WebUtility|HttpUtility'
                     r'|System\.Text\.Json|Utf8JsonReader|JsonDocument|JsonSerializer'
                     r'|"[xX]4"|0x2028|0x2029|0xFEFF|0x180E')


def json_lines(path):
    """Each document's line of a part C output, keyed by the document's name."""
    table = {}
    if os.path.isfile(path):
        for line in open(path, encoding="utf-8"):
            if line.strip() and not line.startswith("#"):
                table[line.split("|", 1)[0]] = line.rstrip("\n")
    return table


class Collector(numerics.Collector):
    def base_identity(self):
        self.into("base")
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
        self.into("base")
        return self.test262()

    def base_twins(self):
        self.into("base")
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
        self.into("base")
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

    # ---- the after arm (SJL-5.6) --------------------------------------------------------------
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
        harness = os.path.join(self.tree, HARNESS)
        lines = [
            "bundle sjl-5-001, its after arm",
            "milestone SJL-5, work package SJL-5.6, Broiler.VM's half",
            "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "after-commit " + commit,
            "after-tree " + ("clean" if not status.strip() else "dirty"),
            "cut-over-commit " + CUT_OVER,
            "base-commit " + base_commit + " (base/identity.txt)",
            "rule %s %s" % (RULE.replace(os.sep, "/"),
                            ("sha256=" + self.sha256_lf(rule)) if os.path.isfile(rule) else "ABSENT"),
            "harness %s %s" % (HARNESS.replace(os.sep, "/"),
                               ("sha256=" + self.sha256_lf(harness)) if os.path.isfile(harness) else "ABSENT"),
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

    def after_json_modules(self):
        self.into("after")
        if not self.suite:
            print("after-json-modules: needs --suite")
            return 1
        suite_tests = os.path.join(self.suite, "test")
        verdict = 0
        for run in (1, 2):
            code, output = self.run(["dotnet", "run", "-c", "Release", os.path.join(self.tree, HARNESS), "--",
                                     os.path.join(self.tree, JSON_MODULES), suite_tests])
            self.write("json-modules-%d.txt" % run, output)
            print("after-json-modules: run %d exit %d" % (run, code), flush=True)
            verdict |= 0 if code == 0 else 1
        runs = {(side, run): json_lines(os.path.join(self.root_out, side, "json-modules-%d.txt" % run))
                for side in ("base", "after") for run in (1, 2)}
        lines = ["# part C: each commit's two runs against each other, then each document's line at the base",
                 "# (base/json-modules-1.txt) and after (after/json-modules-1.txt); the README reads them under the rule"]
        for side in ("base", "after"):
            first, second = runs[(side, 1)], runs[(side, 2)]
            differing = sorted(name for name in set(first) | set(second) if first.get(name) != second.get(name))
            lines.append("%s: run 1 has %d documents, run 2 has %d, %d differ" % (
                side, len(first), len(second), len(differing)))
            lines.extend("  differs: " + name for name in differing)
        before, after = runs[("base", 1)], runs[("after", 1)]
        names = sorted(set(before) | set(after))
        changed = [name for name in names if before.get(name) != after.get(name)]
        lines.append("base against after: %d documents, %d the same, %d differ" % (
            len(names), len(names) - len(changed), len(changed)))
        for name in changed:
            lines += ["  " + name, "    base:  " + before.get(name, "(absent)"),
                      "    after: " + after.get(name, "(absent)")]
        self.write("json-modules-compare.txt", "\n".join(lines) + "\n")
        print("after-json-modules: %d documents, %d differ from the base" % (len(names), len(changed)), flush=True)
        return verdict

    def wall_race(self):
        """Part A's class C4 (e): each (c') row whose re-runs differ, run alone once more at each commit with
        the part's fuel allowance and WALL_RACE_FACTOR times its wall-clock allowance. It records the two
        verdicts; the README reads them under the rule."""
        self.into("after")
        if not self.base_tree or not self.suite:
            print("wall-race: needs --base-tree and --suite")
            return 1
        sides = {}
        for side, tree in (("base", self.base_tree), ("after", self.tree)):
            directory = os.path.join(tree, base.CONFORMANCE, "bin", "Release", "net10.0")
            if not os.path.isfile(os.path.join(directory, base.CONFORMANCE_BINARY + ".dll")):
                code, output = self.run(["dotnet", "build", os.path.join(tree, base.CONFORMANCE,
                                         os.path.basename(base.CONFORMANCE) + ".csproj"), "-c", "Release"], cwd=tree)
                if code != 0:
                    print("wall-race: the %s conformance root did not build" % side)
                    return 1
            sides[side] = (tree, os.path.join(directory, base.CONFORMANCE_BINARY))
        wall = base.WALL * WALL_RACE_FACTOR
        for name, extra, _ in base.RUNS:
            source = os.path.join(self.out, "rerun-%s.tsv" % name)
            rows = []
            if os.path.isfile(source):
                for line in open(source, encoding="utf-8").read().splitlines()[1:]:
                    cells = line.split("\t")
                    if len(cells) == 7 and cells[6] == "NO":
                        rows.append(cells)
            if not rows:
                print("wall-race: %s has no (c') row whose re-runs differ" % name, flush=True)
                continue
            keys = [(cells[0], cells[1]) for cells in rows]
            paths = sorted({key[0] for key in keys})
            log = ["# part A, class C4 (e): the %d (c') rows of rerun-%s.tsv whose re-runs differ, run alone once "
                   "more at each commit, fuel=%d wallClockMs=%d" % (len(keys), name, base.FUEL, wall)]
            verdicts = {}
            for side in ("base", "after"):
                tree, binary = sides[side]
                report = os.path.join(self.scratch, "wall-race-%s-%s.report" % (name, side))
                command = [binary, "--test262", self.suite, "--manifest", "broiler.javascript.wide",
                           "--expect", os.path.join(tree, base.PIN), "--report", report,
                           "--fuel", str(base.FUEL), "--wall", str(wall)] + (extra or ["--form", "bytecode"])
                for path in paths:
                    command += ["--test", path]
                started = datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")
                code, output = self.run(command, cwd=tree)
                log.append("## %s: %s.dll sha256=%s, started %s, exit %d\n%s" % (
                    side, base.CONFORMANCE_BINARY, self.sha256(binary + ".dll"), started, code,
                    self.hide(output).strip()[-4000:]))
                verdicts[side] = numerics.report_verdicts(report)
                if os.path.isfile(report):
                    with open(report, "rb") as stream, gzip.GzipFile(
                            os.path.join(self.out, "wall-race-%s-%s.report.gz" % (name, side)), "wb",
                            compresslevel=9, mtime=0) as target:
                        shutil.copyfileobj(stream, target)
            self.write("wall-race-%s.log" % name, "\n\n".join(log) + "\n")
            table = ["path\tvariant\tbase\tafter\tsame verdict\teither on the wall clock"]
            for key in keys:
                before, after = verdicts["base"].get(key, "(absent)"), verdicts["after"].get(key, "(absent)")
                table.append("\t".join((key[0], key[1], before, after, "yes" if before == after else "NO",
                                        "YES" if "WallClock" in before + after else "no")))
            self.write("wall-race-%s.tsv" % name, "\n".join(table) + "\n")
            print("wall-race: %s %d rows" % (name, len(keys)), flush=True)
        return 0

    def simplification(self):
        self.into("after")
        out = os.path.join(self.out, "simplification")
        os.makedirs(out, exist_ok=True)
        after = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        family = [p for p in sorted(os.listdir(os.path.join(self.tree, "src")))
                  if p.startswith("Broiler.VM.Profile.JavaScript")]
        family_paths = [os.path.join("src", p, "*.cs") for p in family]
        register = json.load(open(os.path.join(self.tree, numerics.prior.RETIRED), encoding="utf-8"))["retired"]
        lines = ["# SJL-5.6 deletion check, Broiler.VM at %s: every member rule I5 registers (%s), its simple"
                 % (after, numerics.prior.RETIRED.replace(os.sep, "/")),
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
        search = ["# SJL-5.6 search for a surviving duplicate of the text area, Broiler.VM at %s: every construct" % after,
                  "# that reads or writes UTF-16 by the platform or by the surrogate constants, pads by the platform,",
                  "# converts base64, hex or a URI by the platform, reads JSON by System.Text.Json, writes a \\uXXXX",
                  "# escape by format string, or spells a line terminator or the byte order mark, in the component's",
                  "# C# product sources (src/, every tests/ excluded).",
                  "# pattern: " + DUPLICATE_PATTERN, "# hits: %d; each is reviewed in the bundle's README" % len(hits), ""]
        search += [hit[:220] for hit in hits]
        self.write(os.path.join("simplification", "duplicate-search.log"), "\n".join(search) + "\n")
        print("simplification: %d registered members searched, %d duplicate-search hits" % (len(register), len(hits)),
              flush=True)
        return 0

    def vectors(self):
        self.into("vectors")
        pin = os.path.join(self.tree, VECTORS_PIN)
        text = open(pin, encoding="utf-8").read()
        revision = re.search(r"is at corpus revision (\d+)", text).group(1)
        archive = re.search(r"^archived-at (\S+)$", text, re.M).group(1)
        binaries = self.build(base.CONFORMANCE, base.CONFORMANCE_BINARY)
        if binaries is None:
            print("vectors: the conformance root did not build; see vectors/build-conformance.log")
            return 1
        binary = os.path.join(binaries, base.CONFORMANCE_BINARY)
        verdict = 0
        for name, extra, wanted in (("after-revision-" + revision, [], 0),
                                    ("after-negative-control-revision-" + revision, ["--mutate", VECTORS_CONTROL], 1)):
            report = os.path.join(self.scratch, name + ".tsv")
            code, output = self.run([binary, "--vectors", archive, "--expect", VECTORS_PIN.replace(os.sep, "/"),
                                     "--report", report] + extra)
            self.write(name + ".log", output.strip() + "\n")
            if os.path.isfile(report):
                with open(report, "rb") as source, gzip.GzipFile(
                        os.path.join(self.out, name + ".tsv.gz"), "wb", compresslevel=9, mtime=0) as target:
                    shutil.copyfileobj(source, target)
            print("vectors: %s exit %d" % (name, code), flush=True)
            verdict |= 0 if code == wanted else 1
        return verdict


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--tree", required=True, help="a clean checkout: the base commit, or the after commit")
    parser.add_argument("--base-tree", help="a clean checkout of the base commit, for rerun and wall-race")
    parser.add_argument("--suite", help="an unpacked checkout of the pinned test262 revision")
    parser.add_argument("--out", required=True)
    parser.add_argument("--jobs", type=int, default=max(1, os.cpu_count() or 1))
    parser.add_argument("--images", help="the closures step's mstat records, for rule I3 (rules)")
    parser.add_argument("--only", action="append", required=True,
                        choices=("base-identity", "base-test262", "base-twins", "base-json-modules",
                                 "after-identity", "after-test262", "after-twins", "after-json-modules", "compare",
                                 "rerun", "wall-race", "closures", "rules", "simplification", "vectors"))
    arguments = parser.parse_args()
    arguments.against_broiler_js = None
    arguments.against_root = None
    collector = Collector(arguments)
    verdict = 0
    for step in arguments.only:
        print("== %s %s" % (step, datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")),
              flush=True)
        verdict |= getattr(collector, step.replace("-", "_"))() or 0
    shutil.rmtree(collector.scratch, ignore_errors=True)
    return verdict


if __name__ == "__main__":
    sys.exit(main())
