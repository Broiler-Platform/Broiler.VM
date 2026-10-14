#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Collect Broiler.VM's evidence for milestone SJL-4 (the numerics area) as bundle sjl-4-001.

    python3 docs/evidence/sjl-4-001/collect.py --tree <a clean checkout> \
        [--base-tree <a clean checkout of the base commit>] \
        [--suite <an unpacked checkout of the pinned test262 revision>] \
        --out docs/evidence/sjl-4-001 --only STEP [--only STEP ...]

THE BASE IS THIS FAMILY AFTER SJL-3, NOT BUNDLE SJL-0-001'S COMMIT. The rule (decision-rule.md beside
this file) takes its base runs at the commit that carries it, before any code of SJL-4 lands, from a
clean checkout on an otherwise idle machine, and retains them in base/. The base steps below write
there and nowhere else.

IT REUSES BUNDLE SJL-0-001'S COLLECTOR for the test262 procedure, so the base and the after run the same
code, and generalizes its twin bisection to the numerics probes the rule adds (src/tests/forms/numerics).

THE COLLECTOR JUDGES NOTHING. Each step writes what it ran and what it printed, failures included; the
README reads them under the rule.

Steps, each with `--only`:

  base-identity  the base commit, its cleanliness, the rule's presence, the SDK and the machine, into
                 base/identity.txt;
  base-test262   the whole pinned suite in both forms, through sjl-0-001's procedure, at --tree, into base/;
  base-twins     every *.small.js twin of src/tests/forms/wide and probe of src/tests/forms/numerics: its
                 smallest completing fuel allowance bisected under the bytecode form, then each form run at
                 that figure and at one less, with the answer it completes with, into base/fuel-parity.log.

THE AFTER STEPS WERE ADDED AT SJL-4.6, when there was an after commit to run them at. They write into
after/, read the base's runs from base/, and reuse bundle sjl-3-001's collector for the closures, the
re-runs of class C4 (c') and the architecture suite, so that SJL-3's and SJL-4's after arms run the same
code. `--tree` is then a clean checkout of a commit whose product sources are the cut-over commit's
(after-identity lists any file outside the documents that differs from it), and `--base-tree` one of
the base commit, which only `rerun` reads.

  after-identity the after commit, its cleanliness, the cut-over commit it carries and every file outside
                 the documents that differs from it, the base it is compared with, the SDK and the machine,
                 into after/identity.txt;
  after-test262  the whole pinned suite in both forms, through sjl-0-001's procedure, at --tree, into after/;
  after-twins    every twin and probe of base/fuel-parity.log, at its base figure and one less, in both
                 forms, with the answer it completes with beside the base's, into after/fuel-parity.log;
  compare        each form's after report against the base's, through the tree's
                 eng/compare-test262-forms.py --same-form, the native pair with --exempt-guest-loads,
                 which part A's class C4 admits, into after/compare-<run>.log;
  rerun          part A's class C4 (c'): every path of a (c') row run again alone at --base-tree and at
                 --tree, as bundle sjl-3-001's collector does, into after/rerun-<run>.*;
  closures       every JavaScript composition root under JIT, trimming and Native AOT, as bundle
                 sjl-3-001's collector does, into after/closures/;
  rules          the whole architecture suite at --tree, as bundle sjl-3-001's collector runs it, into
                 after/rules.log;
  simplification every member rule I5 registers, searched as a declaration in the JavaScript family's
                 sources, and the search for a surviving duplicate of the numerics area in the whole
                 component's product sources, into after/simplification/;
  vectors        the conformance root over this family's pin of the shared corpus, and its negative
                 control, into vectors/run-revision-<n>.* and vectors/negative-control-revision-<n>.*.
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

# Bundle sjl-3-001's collector, whose after steps SJL-4's after arm reuses. It loads sjl-0-001's itself.
_prior_spec = importlib.util.spec_from_file_location(
    "sjl_3_001_collect", os.path.join(os.path.dirname(HERE), "sjl-3-001", "collect.py"))
prior = importlib.util.module_from_spec(_prior_spec)
_prior_spec.loader.exec_module(prior)

RULE = os.path.join("docs", "evidence", "sjl-4-001", "decision-rule.md")
# The default branch after SJL-3's merge, whose product sources the rule says the base commit carries.
SJL_3_MERGE = "a04b2e5bc35461d3ed99aab4354871846e9253d6"
TWIN_FOLDERS = [os.path.join("src", "tests", "forms", "wide"), os.path.join("src", "tests", "forms", "numerics")]
# The commit that lands SJL-4.4's cut-over, whose product sources the after runs must be taken at.
CUT_OVER = "6bd3009527ca293e54fd73ee7a4704fe79935800"
VECTORS_PIN = os.path.join("src", "tests", "vectors", "pins", "vectors.pin")
# The control vector every run of this family's vectors mutates, as SJL-4.2's did.
VECTORS_CONTROL = "numerics/toint32/0010"
# The search for a surviving duplicate of the numerics area: the platform's own approximations and
# conversions the shared library replaces, the rounding modes spelled as strings, binary16 by the
# platform, BigInt arithmetic by BigInteger's own parse and power, and the 32-bit wrap written out.
DUPLICATE_PATTERN = (r'Math\.(Pow|Cbrt|Round|Truncate)\(|double\.(ExpM1|LogP1)|\(Half\)|\bHalf\.'
                     r'|BitConverter\.(HalfToUInt16Bits|UInt16BitsToHalf)|MidpointRounding'
                     r'|"half(Expand|Even|Trunc|Ceil|Floor)"|BigInteger\.(Pow|Parse|TryParse)\('
                     r'|4294967296|unchecked\(\(int\)\(uint\)')


class Collector(prior.Collector):
    def __init__(self, arguments):
        super().__init__(arguments)
        self.root_out = self.out
        self.out = os.path.join(self.root_out, "base")
        os.makedirs(self.out, exist_ok=True)

    def into(self, folder):
        """Points the steps' output at base/, after/ or vectors/ of the bundle."""
        self.out = os.path.join(self.root_out, folder)
        os.makedirs(self.out, exist_ok=True)

    def base_identity(self):
        self.into("base")
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        status = self.run(["git", "status", "--porcelain"])[1]
        rule = os.path.join(self.tree, RULE)
        code, names = self.run(["git", "diff", "--name-only", SJL_3_MERGE, commit])
        changed = [name for name in names.splitlines()
                   if name and not name.startswith("docs/") and "/docs/" not in name]
        lines = [
            "bundle sjl-4-001, its base",
            "milestone SJL-4, the numerics area; the base is this family after SJL-3",
            "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "base-commit " + commit,
            "base-tree " + ("clean" if not status.strip() else "dirty"),
            "rule %s %s" % (RULE.replace(os.sep, "/"),
                            ("sha256=" + self.sha256_lf(rule)) if os.path.isfile(rule) else "ABSENT"),
            "sdk " + self.run(["dotnet", "--version"])[1].strip(),
            "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
            "suite-pin " + base.PIN + " sha256=" + self.sha256_lf(os.path.join(self.tree, base.PIN)),
            "allowance fuel=%d wallClockMs=%d" % (base.FUEL, base.WALL),
            "twin-allowance ceiling=%d wallClockMs=%d" % (base.TWIN_CEILING, base.TWIN_WALL),
            "jobs %d" % self.jobs,
            "native-backend " + base.NATIVE_BACKEND,
            "since-sjl-3-merge %s, outside the documents: %s" % (
                SJL_3_MERGE, ("git diff failed with exit %d" % code) if code != 0
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

    # ---- the after arm (SJL-4.6) --------------------------------------------------------------
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
            "bundle sjl-4-001, its after arm",
            "milestone SJL-4, work package SJL-4.6, Broiler.VM's half",
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

    def after_test262(self):
        self.into("after")
        return self.test262()

    def after_twins(self):
        self.into("after")
        binaries = self.build(base.CLI, base.CLI_BINARY)
        if binaries is None:
            print("after-twins: the CLI root did not build; see after/build-cli.log")
            return 1
        binary = os.path.join(binaries, base.CLI_BINARY)
        figures, answers = {}, {}
        for line in open(os.path.join(self.root_out, "base", "fuel-parity.log"), encoding="utf-8"):
            if line.startswith("#") or "|" not in line:
                continue
            cells = [cell.strip() for cell in line.split("|")]
            if len(cells) >= 6 and cells[2].isdigit():
                figures[cells[0]] = int(cells[2])
                answers[(cells[0], cells[1])] = cells[5]
        lines = ["# fuel-parity twins and probes after the cut-over, at each one's base figure "
                 "(base/fuel-parity.log) and one less",
                 "# twin | form | base figure | at the base figure | at one less | answer | the base's answer"]
        verdict = 0

        def attempt(form, path, fuel):
            command = [binary]
            if form == "native":
                command += ["--native", base.NATIVE_BACKEND]
            command += ["--fuel", str(fuel), "--wall", str(base.TWIN_WALL), path]
            return self.run(command)

        def last(output):
            return (output.strip().splitlines() or ["(no output)"])[-1]

        for label, figure in sorted(figures.items()):
            folder, twin = label.split("/", 1)
            path = os.path.join(self.tree, "src", "tests", "forms", folder, twin)
            for form in ("bytecode", "native"):
                done, said_done = attempt(form, path, figure)
                refused, said = attempt(form, path, figure - 1)
                below = ("refused with %s" % base.EXHAUSTED_ON_FUEL
                         if refused != 0 and base.EXHAUSTED_ON_FUEL in said
                         else "exit %d: %s" % (refused, last(said)))
                at = "completes" if done == 0 else "exit %d: %s" % (done, last(said_done))
                answer = last(said_done) if done == 0 else "-"
                was = answers.get((label, form), "(absent)")
                lines.append("%s | %s | %d | %s | %s | %s | %s" % (label, form, figure, at, below, answer, was))
                if done != 0 or not below.startswith("refused") or answer != was:
                    verdict = 1
                print("after-twins: %s %s %d" % (label, form, figure), flush=True)
        self.write("fuel-parity.log", "\n".join(lines) + "\n")
        return verdict

    def compare(self):
        self.into("after")
        verdict = 0
        for name, extra, _ in base.RUNS:
            native = "native" in extra
            reports = []
            for side, folder in (("base", os.path.join(self.root_out, "base")), ("after", self.out)):
                source = os.path.join(folder, "%s.report.gz" % name)
                if not os.path.isfile(source):
                    print("compare: no %s report %s" % (side, source))
                    return 1
                target = os.path.join(self.scratch, "%s-%s.report" % (side, name))
                with gzip.open(source, "rb") as stream, open(target, "wb") as copy:
                    shutil.copyfileobj(stream, copy)
                reports.append(target)
            command = [sys.executable, os.path.join(self.tree, "eng", "compare-test262-forms.py"), reports[0], reports[1],
                       "--same-form"] + (["--exempt-guest-loads"] if native else [])
            if self.suite:
                command += ["--suite", self.suite]
            code, output = self.run(command)
            self.write("compare-%s.log" % name, "$ compare-test262-forms.py <base %s> <after %s> %s\n%s\nexit code: %d\n"
                       % (name, name, " ".join(command[4:]).replace(self.suite or "\0", "<suite>"), output.strip(), code))
            print("compare: %s exit %d" % (name, code), flush=True)
            verdict |= code
        return verdict

    def rerun(self):
        self.into("after")
        return super().rerun()

    def closures(self):
        self.into("after")
        return super().closures()

    def rules(self):
        self.into("after")
        return super().rules()

    def simplification(self):
        self.into("after")
        out = os.path.join(self.out, "simplification")
        os.makedirs(out, exist_ok=True)
        after = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        family = [p for p in sorted(os.listdir(os.path.join(self.tree, "src")))
                  if p.startswith("Broiler.VM.Profile.JavaScript")]
        family_paths = [os.path.join("src", p, "*.cs") for p in family]
        register = json.load(open(os.path.join(self.tree, prior.RETIRED), encoding="utf-8"))["retired"]
        lines = ["# SJL-4.6 deletion check, Broiler.VM at %s: every member rule I5 registers (%s), its simple"
                 % (after, prior.RETIRED.replace(os.sep, "/")),
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
        search = ["# SJL-4.6 search for a surviving duplicate of the numerics area, Broiler.VM at %s: every" % after,
                  "# construct that approximates a Math function the shared library computes, converts binary16 or a",
                  "# BigInt by the platform, rounds by a mode spelled as a string or by the platform's midpoint rule, or",
                  "# writes the 32-bit wrap out, in the component's C# product sources (src/, every tests/ excluded).",
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
        for name, extra, wanted in (("run-revision-" + revision, [], 0),
                                    ("negative-control-revision-" + revision, ["--mutate", VECTORS_CONTROL], 1)):
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
    parser.add_argument("--base-tree", help="a clean checkout of the base commit, for rerun")
    parser.add_argument("--suite", help="an unpacked checkout of the pinned test262 revision")
    parser.add_argument("--out", required=True)
    parser.add_argument("--jobs", type=int, default=max(1, os.cpu_count() or 1))
    parser.add_argument("--images", help="the closures step's mstat records, for rule I3 (rules)")
    parser.add_argument("--only", action="append", required=True,
                        choices=("base-identity", "base-test262", "base-twins", "after-identity", "after-test262",
                                 "after-twins", "compare", "rerun", "closures", "rules", "simplification", "vectors"))
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
