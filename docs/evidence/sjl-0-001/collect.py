#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Collect Broiler.VM's half of milestone SJL-0's baselines (work package SJL-0.8) as bundle sjl-0-001.

    python3 docs/evidence/sjl-0-001/collect.py --tree <a checkout at the base commit> \
        --suite <an unpacked checkout of the pinned test262 revision> \
        --out docs/evidence/sjl-0-001 [--only STEP ...]

THE BASE IS A CHECKOUT, NOT THIS TREE. The shared JavaScript libraries programme compares every later
cut-over against a base taken before any engine changes, at a named commit. This collector is written
after that commit, so it does not run from the commit it measures: `--tree` names a separate checkout
of the base, and every build, run and hash below is taken there. The manifest records that checkout's
commit and whether it was clean, and a collection from a dirty base says so rather than pretending.

THE COLLECTOR JUDGES NOTHING. Each step writes what it ran and what it printed, failures included. The
bundle's README interprets; no figure leaves the bundle.

Steps, each with `--only` to take it alone:

  identity   the base commit, its cleanliness, the SDK and the machine, written first and made
             read-only, as eng/collect-js-release-candidate.py writes its manifest;
  test262    the whole pinned suite under the wide manifest in the bytecode form and the native form,
             through the base's own eng/run-test262.py, with the release-candidate collector's
             allowances and floors; each run's effective limit vector, merged report (compressed),
             failure manifest, JSON summary, merge log and floor log;
  api        the committed API baselines of the core and of the JavaScript family, by SHA-256 of their
             LF bytes and by git blob id at the base commit;
  twins      the fuel-parity twins of src/tests/forms/wide: each twin's smallest completing allowance
             under the bytecode form, bisected, and in each form whether it completes at that figure and
             refuses with `AllowanceExhausted on Fuel` at one less;
  measure    the base arm of part C of docs/evidence/sjl-3-001/decision-rule.md, measured alone: each
             workload of src/tests/forms/numbers (taken from the tree this collector lives in, because
             the workloads were committed after the base) with its control and an A/A lane, interleaved
             inside every repetition, in both forms; every repetition retained, the answer checked
             before and after every lane, and each child's effective-configuration line recorded. It
             writes measurement/ as a sub-bundle of its own, because a measurement is its own evidence
             class;
  closures   every JavaScript composition root published and run under JIT, trimming and Native AOT,
             with its catalog and the closure read off the published output, by calling the base
             tree's own eng/collect-js-evidence.py compositions procedure (bundle JS-10-004's), into
             closures/;
  packages   the family's packed nuspecs and contents, a pristine consumer's restores and the rollback,
             by running the base tree's own eng/collect-js-packages.py (bundle JS-10-002's collector),
             into packages/;
  differential
             the base tree's own eng/run-differential.py over its retained answers and against the
             Broiler.JS engine built at a named commit (--against-broiler-js, --against-root), into
             differential/. A cross-check only: it meets no conformance clause.

Paths in retained logs are written as <tree>, <suite> and <scratch>.
"""

import argparse
import datetime
import gzip
import hashlib
import os
import platform
import shutil
import stat
import statistics
import subprocess
import sys
import tempfile
import time

CONFORMANCE = os.path.join("src", "compositions", "Broiler.VM.Composition.JavaScript.Conformance")
CONFORMANCE_BINARY = "Broiler.VM.Composition.JavaScript.Conformance"
CLI = os.path.join("src", "compositions", "Broiler.VM.Composition.JavaScript.Cli")
CLI_BINARY = "Broiler.VM.Composition.JavaScript.Cli"
PIN = os.path.join("src", "tests", "conformance", "pins", "test262.pin")
FLOORS = os.path.join("src", "tests", "conformance", "floors")
TWINS = os.path.join("src", "tests", "forms", "wide")

# The release-candidate collector's allowances, so the base is taken under the procedure that
# milestone SJL-0.8 names as its model.
FUEL = 100_000_000
WALL = 5_000

RUNS = [
    ("wide-bytecode", [], "test262-wide.floor"),
    ("wide-native", ["--form", "native"], "test262-wide-native.floor"),
]

KEPT = ("Failed", "Exhausted")

# The core's baseline and the JavaScript family's, which freezes every assembly of the family in one
# file. The universal bytecode's and the WebAssembly profile's baselines are not this programme's.
API_BASELINES = [
    os.path.join("docs", "api", "public-api.txt"),
    os.path.join("src", "Broiler.VM.Profile.JavaScript", "docs", "api", "public-api.txt"),
]

# The twins' allowance search starts here; a twin that does not complete under it is reported.
# The measurement's counts are the files' own, cited by the rule rather than restated there: seven
# retained repetitions per lane is docs/baselines.md rule 4, and three warm-ups is eng/compare-forms.py's
# default. The allowances are ample and the same for every lane, so no lane can stop on one.
REPETITIONS = 7
WARMUP = 3
MEASURE_FUEL = 100_000_000_000
MEASURE_WALL = 600_000
WORKLOADS = os.path.join("src", "tests", "forms", "numbers")
WORKLOAD_STEMS = ("to-string", "to-number", "literals")
RUNTIME_PREFIX = "runtime-identifier="
HERE = os.path.dirname(os.path.abspath(__file__))
COLLECTOR_ROOT = os.path.dirname(os.path.dirname(os.path.dirname(HERE)))

TWIN_CEILING = 1_000_000_000
TWIN_WALL = 600_000
EXHAUSTED_ON_FUEL = "AllowanceExhausted on Fuel"
NATIVE_BACKEND = {"Windows": "x86-64-win64"}.get(platform.system(), "x86-64-sysv")


class Collector:
    def __init__(self, arguments):
        self.tree = os.path.abspath(arguments.tree)
        self.suite = os.path.abspath(arguments.suite) if arguments.suite else None
        self.out = os.path.abspath(arguments.out)
        self.jobs = arguments.jobs
        self.against = os.path.abspath(arguments.against_broiler_js) if arguments.against_broiler_js else None
        self.against_root = os.path.abspath(arguments.against_root) if arguments.against_root else None
        self.scratch = tempfile.mkdtemp(prefix="sjl-0-001-")
        os.makedirs(self.out, exist_ok=True)

    # ---- helpers ------------------------------------------------------------------------------
    def hide(self, text):
        for path, name in ((self.scratch, "<scratch>"), (self.suite, "<suite>"), (self.tree, "<tree>")):
            if path:
                text = text.replace(path, name)
        return text

    def run(self, command, cwd=None):
        environment = dict(os.environ)
        environment.setdefault("DOTNET_CLI_TELEMETRY_OPTOUT", "1")
        environment["DOTNET_CLI_UI_LANGUAGE"] = "en"
        result = subprocess.run(command, cwd=cwd or self.tree, capture_output=True, encoding="utf-8",
                                errors="replace", env=environment)
        return result.returncode, (result.stdout or "") + (result.stderr or "")

    def write(self, name, text):
        with open(os.path.join(self.out, name), "w", encoding="utf-8", newline="\n") as stream:
            stream.write(self.hide(text))

    @staticmethod
    def sha256(path):
        digest = hashlib.sha256()
        with open(path, "rb") as stream:
            for block in iter(lambda: stream.read(1 << 20), b""):
                digest.update(block)
        return digest.hexdigest()

    @staticmethod
    def sha256_lf(path):
        with open(path, "rb") as stream:
            return hashlib.sha256(stream.read().replace(b"\r\n", b"\n")).hexdigest()

    def build(self, project, binary):
        code, output = self.run(["dotnet", "build", os.path.join(project, os.path.basename(project) + ".csproj"),
                                 "-c", "Release"])
        self.write("build-%s.log" % binary.rsplit(".", 1)[-1].lower(), output.strip() + "\nexit code: %d\n" % code)
        if code != 0:
            return None
        built = os.path.join(self.tree, project, "bin", "Release", "net10.0")
        copy = os.path.join(self.scratch, binary)
        shutil.copytree(built, copy, dirs_exist_ok=True)
        return copy

    # ---- steps --------------------------------------------------------------------------------
    def identity(self):
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        status = self.run(["git", "status", "--porcelain"])[1]
        lines = [
            "bundle sjl-0-001",
            "milestone SJL-0, work package SJL-0.8, Broiler.VM's half",
            "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "base-commit " + commit,
            "base-tree " + ("clean" if not status.strip() else "dirty"),
            "sdk " + self.run(["dotnet", "--version"])[1].strip(),
            "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
            "suite-pin " + PIN + " sha256=" + self.sha256_lf(os.path.join(self.tree, PIN)),
            "allowance fuel=%d wallClockMs=%d" % (FUEL, WALL),
            "jobs %d" % self.jobs,
            "native-backend " + NATIVE_BACKEND,
        ]
        if status.strip():
            lines.append("dirty:")
            lines.extend("  " + line for line in status.splitlines())
        path = os.path.join(self.out, "identity.txt")
        if os.path.exists(path):
            os.chmod(path, stat.S_IREAD | stat.S_IWRITE)
        self.write("identity.txt", "\n".join(lines) + "\n")
        os.chmod(path, stat.S_IREAD | stat.S_IRGRP | stat.S_IROTH)
        print("identity: base %s (%s tree)" % (commit, "clean" if not status.strip() else "DIRTY"))
        return 0

    def test262(self):
        if not self.suite:
            print("test262: no --suite given")
            return 1
        binaries = self.build(CONFORMANCE, CONFORMANCE_BINARY)
        if binaries is None:
            print("test262: the conformance root did not build; see build-conformance.log")
            return 1
        binary = os.path.join(binaries, CONFORMANCE_BINARY)
        self.write("test262-binary.txt", "%s.dll sha256=%s\n" % (
            CONFORMANCE_BINARY, self.sha256(binary + ".dll")))
        verdict = 0
        for name, extra, floor in RUNS:
            code, output = self.run([binary, "--effective-limits", "--fuel", str(FUEL), "--wall", str(WALL)]
                                    + extra)
            self.write("limits-%s.log" % name, output.strip() + "\nexit code: %d\n" % code)
            transcript = os.path.join(self.scratch, "transcript-" + name)
            command = [sys.executable, os.path.join(self.tree, "eng", "run-test262.py"),
                       "--suite", self.suite, "--binary-directory", binaries, "--jobs", str(self.jobs),
                       "--fuel", str(FUEL), "--wall", str(WALL), "--out", transcript] + extra
            if floor:
                command += ["--floor", os.path.join(self.tree, FLOORS, floor)]
            print("test262: run %s ..." % name, flush=True)
            code, output = self.run(command)
            self.write("%s.run.log" % name, output.strip() + "\nexit code: %d\n" % code)
            print("test262: %s runner exit %d" % (name, code), flush=True)
            if code not in (0, 1, 4):
                verdict = 1
            report = os.path.join(transcript, "test262.report")
            if os.path.isfile(report):
                with open(report, "rb") as source, gzip.GzipFile(
                        os.path.join(self.out, "%s.report.gz" % name), "wb", compresslevel=9,
                        mtime=0) as target:
                    shutil.copyfileobj(source, target)
                kept = []
                with open(report, encoding="utf-8") as stream:
                    for line in stream:
                        if line.startswith("total|"):
                            kept.insert(0, "# " + line.strip())
                        if line.startswith("result|"):
                            parts = line.rstrip("\n").split("|")
                            if len(parts) > 3 and parts[3] in KEPT:
                                kept.append(line.rstrip("\n"))
                self.write("%s.failures.txt" % name,
                           "# failure manifest of base run %s, generated from its merged report\n"
                           "# result|path|variant|verdict|family|dimension|kind|features|detail\n" % name
                           + "\n".join(kept) + "\n")
            for retained in ("test262.json", "merge.log", "floor.log"):
                path = os.path.join(transcript, retained)
                if os.path.isfile(path):
                    with open(path, encoding="utf-8", errors="replace") as stream:
                        self.write("%s.%s" % (name, retained), stream.read())
            shutil.rmtree(transcript, ignore_errors=True)
        return verdict

    def api(self):
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        lines = ["# the committed API baselines at the base commit " + commit,
                 "# path | sha256 of the LF bytes | git blob id at the base commit"]
        for path in API_BASELINES:
            full = os.path.join(self.tree, path)
            if not os.path.isfile(full):
                lines.append("%s | absent | absent" % path.replace(os.sep, "/"))
                continue
            blob = self.run(["git", "rev-parse", "%s:%s" % (commit, path.replace(os.sep, "/"))])[1].strip()
            lines.append("%s | %s | %s" % (path.replace(os.sep, "/"), self.sha256_lf(full), blob))
        self.write("api-baselines.txt", "\n".join(lines) + "\n")
        print("api: %d baselines hashed" % len(API_BASELINES))
        return 0

    def twins(self):
        binaries = self.build(CLI, CLI_BINARY)
        if binaries is None:
            print("twins: the CLI root did not build; see build-cli.log")
            return 1
        binary = os.path.join(binaries, CLI_BINARY)
        folder = os.path.join(self.tree, TWINS)
        names = sorted(name for name in os.listdir(folder) if name.endswith(".small.js"))
        lines = ["# fuel-parity twins of %s at the base commit" % TWINS.replace(os.sep, "/"),
                 "# twin | form | smallest completing allowance (bytecode, bisected) | at that figure | at one less"]
        verdict = 0

        def attempt(form, twin, fuel):
            command = [binary]
            if form == "native":
                command += ["--native", NATIVE_BACKEND]
            command += ["--fuel", str(fuel), "--wall", str(TWIN_WALL), os.path.join(folder, twin)]
            return self.run(command)

        for twin in names:
            code, output = attempt("bytecode", twin, TWIN_CEILING)
            if code != 0:
                lines.append("%s | bytecode | does not complete under %d | exit %d | - | %s" % (
                    twin, TWIN_CEILING, code, (output.strip().splitlines() or ["(no output)"])[-1]))
                verdict = 1
                continue
            low, high = 1, TWIN_CEILING
            while low < high:
                middle = (low + high) // 2
                if attempt("bytecode", twin, middle)[0] == 0:
                    high = middle
                else:
                    low = middle + 1
            for form in ("bytecode", "native"):
                done, _ = attempt(form, twin, low)
                refused, said = attempt(form, twin, low - 1)
                below = ("refused with %s" % EXHAUSTED_ON_FUEL if refused != 0 and EXHAUSTED_ON_FUEL in said
                         else "exit %d: %s" % (refused, (said.strip().splitlines() or ["(no output)"])[-1]))
                lines.append("%s | %s | %d | %s | %s" % (
                    twin, form, low, "completes" if done == 0 else "exit %d" % done, below))
                print("twins: %s %s %d" % (twin, form, low), flush=True)
        self.write("fuel-parity.log", "\n".join(lines) + "\n")
        return verdict

    def measure(self):
        binaries = self.build(CLI, CLI_BINARY)
        if binaries is None:
            print("measure: the CLI root did not build; see build-cli.log")
            return 1
        binary = os.path.join(binaries, CLI_BINARY)
        folder = os.path.join(COLLECTOR_ROOT, WORKLOADS)
        out = os.path.join(self.out, "measurement")
        os.makedirs(out, exist_ok=True)
        environment = dict(os.environ)
        environment["DOTNET_CLI_UI_LANGUAGE"] = "en"

        def child(form, path):
            command = [binary, "--runtime"]
            if form == "native":
                command += ["--native", NATIVE_BACKEND]
            command += ["--fuel", str(MEASURE_FUEL), "--wall", str(MEASURE_WALL), path]
            started = time.perf_counter()
            done = subprocess.run(command, capture_output=True, encoding="utf-8", errors="replace",
                                  env=environment)
            elapsed = (time.perf_counter() - started) * 1000.0
            answer = (done.stdout or "").strip().splitlines()
            runtime = next((line.strip() for line in (done.stderr or "").splitlines()
                            if line.startswith(RUNTIME_PREFIX)), "")
            return done.returncode, answer[-1] if answer else "", runtime, elapsed

        manifest = [
            "sub-bundle sjl-0-001/measurement",
            "evidence-class measurement",
            "decision For each workload and form, the attribution E(B) = Q(B) - Q(control) is recorded as "
            "the base's figure when |E(B)| exceeds the A/A difference |Q(B) - Q(B-again)|, and as below "
            "resolution otherwise. Nothing else is decided, and nothing about milestone SJL-3.",
            "rule docs/evidence/sjl-3-001/decision-rule.md part C, the base reading at SJL-0",
            "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "base-commit " + self.run(["git", "rev-parse", "HEAD"])[1].strip(),
            "workloads-commit " + self.run(["git", "rev-parse", "HEAD"], cwd=COLLECTOR_ROOT)[1].strip(),
            "binary %s.dll sha256=%s" % (CLI_BINARY, self.sha256(binary + ".dll")),
            "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
            "repetitions %d retained, %d warm-up excluded from Q" % (REPETITIONS, WARMUP),
            "allowance fuel=%d wallClockMs=%d" % (MEASURE_FUEL, MEASURE_WALL),
            "native-backend " + NATIVE_BACKEND,
        ]
        for stem in WORKLOAD_STEMS:
            for name in (stem + ".js", stem + ".control.js"):
                manifest.append("workload %s sha256=%s" % (name, self.sha256_lf(os.path.join(folder, name))))
        path = os.path.join(out, "manifest.txt")
        with open(path, "w", encoding="utf-8", newline="\n") as stream:
            stream.write(self.hide("\n".join(manifest) + "\n"))
        os.chmod(path, stat.S_IREAD | stat.S_IRGRP | stat.S_IROTH)

        runs = ["form\tworkload\trepetition\tretained\tlane\texit\tanswer-ok\twall-ms"]
        conditions = ["form\tworkload\tfile\twhen\texit\tanswer"]
        summary = ["# the base arm measured alone; Q is the median of the retained repetitions, in ms",
                   "# form | workload | Q(B) | Q(control) | Q(B-again) | E = Q(B) - Q(control) | "
                   "N = |Q(B) - Q(B-again)| | reading"]
        configurations = set()
        verdict = 0
        for form in ("bytecode", "native"):
            for stem in WORKLOAD_STEMS:
                files = {"B": stem + ".js", "control": stem + ".control.js", "B-again": stem + ".js"}
                expected = {}
                for lane_file in (stem + ".js", stem + ".control.js"):
                    code, answer, runtime, _ = child(form, os.path.join(folder, lane_file))
                    expected[lane_file] = answer
                    conditions.append("%s\t%s\t%s\tbefore\t%d\t%s" % (form, stem, lane_file, code, answer))
                    if code != 0:
                        verdict = 1
                times = {lane: [] for lane in files}
                for repetition in range(WARMUP + REPETITIONS):
                    retained = repetition >= WARMUP
                    for lane in ("B", "control", "B-again"):
                        code, answer, runtime, elapsed = child(form, os.path.join(folder, files[lane]))
                        ok = code == 0 and answer == expected[files[lane]]
                        configurations.add(runtime)
                        runs.append("%s\t%s\t%d\t%s\t%s\t%d\t%s\t%.3f" % (
                            form, stem, repetition, "yes" if retained else "warm-up", lane, code,
                            "yes" if ok else "NO", elapsed))
                        if not ok:
                            verdict = 1
                        if retained:
                            times[lane].append(elapsed)
                for lane_file in (stem + ".js", stem + ".control.js"):
                    code, answer, runtime, _ = child(form, os.path.join(folder, lane_file))
                    conditions.append("%s\t%s\t%s\tafter\t%d\t%s" % (form, stem, lane_file, code, answer))
                    if code != 0 or answer != expected[lane_file]:
                        verdict = 1
                q = {lane: statistics.median(values) for lane, values in times.items()}
                attribution = q["B"] - q["control"]
                floor = abs(q["B"] - q["B-again"])
                reading = "recorded" if abs(attribution) > floor else "below resolution"
                summary.append("%s | %s | %.3f | %.3f | %.3f | %.3f | %.3f | %s" % (
                    form, stem, q["B"], q["control"], q["B-again"], attribution, floor, reading))
                print("measure: %s %s %s" % (form, stem, reading), flush=True)
        summary.append("# effective-configuration lines seen: %d" % len(configurations))
        summary.extend("#   " + line for line in sorted(configurations))
        if len(configurations) != 1:
            verdict = 1
        for name, lines in (("runs.tsv", runs), ("conditions.tsv", conditions), ("summary.txt", summary)):
            with open(os.path.join(out, name), "w", encoding="utf-8", newline="\n") as stream:
                stream.write(self.hide("\n".join(lines) + "\n"))
        lines = []
        for entry in sorted(os.listdir(out)):
            if entry != "hashes.txt":
                lines.append("%s  %s" % (self.sha256_lf(os.path.join(out, entry)), entry))
        with open(os.path.join(out, "hashes.txt"), "w", encoding="utf-8", newline="\n") as stream:
            stream.write("\n".join(lines) + "\n")
        return verdict

    def closures(self):
        import importlib.util
        path = os.path.join(self.tree, "eng", "collect-js-evidence.py")
        spec = importlib.util.spec_from_file_location("base_collect_js_evidence", path)
        module = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(module)

        class Arguments:
            rid = "win-x64" if platform.system() == "Windows" else "linux-x64"

        out = os.path.join(self.out, "closures")
        corpus = os.path.join(self.tree, "src", "tests", "corpus", "js-1")
        ok = module.compositions(Arguments, out, corpus)
        for entry in os.listdir(out):
            full = os.path.join(out, entry)
            with open(full, encoding="utf-8", errors="replace") as stream:
                text = stream.read()
            with open(full, "w", encoding="utf-8", newline="\n") as stream:
                stream.write(self.hide(text))
        print("closures: every root published and run in three modes: %s" % ok)
        return 0 if ok else 1

    def packages(self):
        out = os.path.join(self.scratch, "packages")
        code, output = self.run([sys.executable, os.path.join(self.tree, "eng", "collect-js-packages.py"),
                                 "--bundle", "sjl-0-001", "--out", out])
        target = os.path.join(self.out, "packages")
        os.makedirs(target, exist_ok=True)
        if os.path.isdir(out):
            for entry in sorted(os.listdir(out)):
                full = os.path.join(out, entry)
                if os.path.isfile(full):
                    with open(full, "rb") as stream:
                        data = stream.read()
                    try:
                        text = self.hide(data.decode("utf-8"))
                        with open(os.path.join(target, entry), "w", encoding="utf-8", newline="\n") as stream:
                            stream.write(text)
                    except UnicodeDecodeError:
                        with open(os.path.join(target, entry), "wb") as stream:
                            stream.write(data)
        with open(os.path.join(target, "collector.log"), "w", encoding="utf-8", newline="\n") as stream:
            stream.write(self.hide(output.strip() + "\nexit code: %d\n" % code))
        print("packages: collector exit %d" % code)
        return 0 if code == 0 else 1

    def differential(self):
        if not self.against or not self.against_root:
            print("differential: no --against-broiler-js and --against-root given")
            return 1
        binaries = os.path.join(self.tree, CLI, "bin", "Release", "net10.0")
        if not os.path.isfile(os.path.join(binaries, CLI_BINARY + ".dll")):
            binaries = self.build(CLI, CLI_BINARY)
            if binaries is None:
                return 1
        out = os.path.join(self.out, "differential")
        os.makedirs(out, exist_ok=True)
        report = os.path.join(self.scratch, "differential.json")
        commit = self.run(["git", "rev-parse", "HEAD"], cwd=self.against_root)[1].strip()
        status = self.run(["git", "status", "--porcelain"], cwd=self.against_root)[1]
        code, output = self.run([sys.executable, os.path.join(self.tree, "eng", "run-differential.py"),
                                 "--binary-directory", binaries, "--against", self.against,
                                 "--against-kind", "broiler-js", "--against-root", self.against_root,
                                 "--timeout", "30", "--report", report])
        text = self.hide(output).replace(self.against_root, "<broiler-js>")
        with open(os.path.join(out, "run.log"), "w", encoding="utf-8", newline="\n") as stream:
            stream.write("against Broiler.JS at %s (%s tree)\n" % (commit, "clean" if not status.strip() else "dirty"))
            stream.write(text.strip() + "\nexit code: %d\n" % code)
        if os.path.isfile(report):
            with open(report, encoding="utf-8", errors="replace") as stream:
                body = self.hide(stream.read()).replace(self.against_root, "<broiler-js>")
            with open(os.path.join(out, "report.json"), "w", encoding="utf-8", newline="\n") as stream:
                stream.write(body)
        print("differential: exit %d" % code)
        return 0

    def hashes(self):
        lines = []
        for entry in sorted(os.listdir(self.out)):
            path = os.path.join(self.out, entry)
            if entry not in ("hashes.txt", "manifest.json") and os.path.isfile(path):
                lines.append("%s  %s" % (self.sha256_lf(path), entry))
        with open(os.path.join(self.out, "hashes.txt"), "w", encoding="utf-8", newline="\n") as stream:
            stream.write("\n".join(lines) + "\n")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--tree", required=True, help="a checkout at the base commit")
    parser.add_argument("--suite", help="an unpacked checkout of the pinned test262 revision")
    parser.add_argument("--out", required=True)
    parser.add_argument("--jobs", type=int, default=max(1, os.cpu_count() or 1))
    parser.add_argument("--against-broiler-js", help="the BroilerJS apphost built at the named commit")
    parser.add_argument("--against-root", help="the Broiler.JS checkout it was built from")
    parser.add_argument("--only", action="append", default=[],
                        choices=("identity", "test262", "api", "twins", "measure", "closures", "packages",
                                 "differential"))
    arguments = parser.parse_args()

    collector = Collector(arguments)
    steps = arguments.only or ["identity", "test262", "api", "twins", "measure", "closures", "packages",
                               "differential"]
    verdict = 0
    for step in steps:
        verdict |= getattr(collector, step)()
    collector.hashes()
    shutil.rmtree(collector.scratch, ignore_errors=True)
    return verdict


if __name__ == "__main__":
    sys.exit(main())
