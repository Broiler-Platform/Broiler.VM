#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Collect Broiler.VM's evidence for milestone SJL-3 (work packages SJL-3.4 and SJL-3.6) as bundle sjl-3-001.

    python3 docs/evidence/sjl-3-001/collect.py --tree <a checkout at the cut-over commit> \
        [--base-tree <a checkout at bundle sjl-0-001's base commit>] \
        [--suite <an unpacked checkout of the pinned test262 revision>] \
        --out docs/evidence/sjl-3-001 --only STEP [--only STEP ...]

THE AFTER ARM IS A CHECKOUT, NOT THIS TREE. Every run is taken from `--tree`, a separate, clean checkout
of the commit that lands SJL-3.4's cut-over, as the rule asks ("the after runs are taken at the commit
that lands SJL-3's Broiler.VM cut-over"). The base is bundle sjl-0-001's, read from its retained files;
the measurement rebuilds it from `--base-tree`, as its rule asks.

IT REUSES BUNDLE SJL-0-001'S COLLECTOR for every step whose procedure the rule takes from the base -
test262, the differential cross-check - so the two arms run the same code; it adds the steps the base
did not have: the closures of every JavaScript root including PolyglotCli, the twins at the base's own
ceilings, and the comparison.

THE COLLECTOR JUDGES NOTHING. Each step writes what it ran and what it printed, failures included; the
README reads them under the rule.

Steps, each with `--only`:

  identity   the after commit, its cleanliness, the base it is compared with, the SDK and the machine;
  closures   every JavaScript composition root the machine can publish - ExecutionOnly, SliceCompiler,
             Conformance and Cli through the tree's own eng/collect-js-evidence.py compositions
             procedure, and PolyglotCli through the same publish and closure helpers with the runs bundle
             UBC-4-002 retained - under JIT, trimming and Native AOT, into closures/;
  test262    the whole pinned suite in both forms, through sjl-0-001's procedure, run at the after tree;
  twins      every twin of sjl-0-001's fuel-parity.log, at its base figure and one less, in both forms;
  compare    each form's after report against the base's retained one, through the tree's
             eng/compare-test262-forms.py --same-form, the native pair with --exempt-guest-loads, which
             part A's class C4 admits, into compare-<run>.log;
  measure    part C: the CLI root built at --base-tree (arm B) and at --tree (arms A and A-again), and
             for each workload of src/tests/forms/numbers and each form, every arm running the workload
             and its control inside every repetition, the arms' order rotating by repetition, into
             measurement/, a sub-bundle of its own;
  rerun      part A's class C4 (c'): every path of a (c') row of compare-<run>.log run again alone, one
             process, at --base-tree's conformance root and at --tree's, the two re-run reports compared
             with --same-form, and each row's four verdicts tabulated, into rerun-<run>.*;
  rules      the whole architecture suite at the after tree, rules I1 to I7, N37 and K4 among them, into
             rules.log;
  simplification
             SJL-3.7: every member rule I5 registers, searched as a whole word in the JavaScript family's
             product sources, and the search for a surviving duplicate of the area in the whole
             component's, into simplification/.
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
import statistics
import subprocess
import sys
import time

HERE = os.path.dirname(os.path.abspath(__file__))
BASE_BUNDLE = os.path.join(os.path.dirname(HERE), "sjl-0-001")

_spec = importlib.util.spec_from_file_location("sjl_0_001_collect", os.path.join(BASE_BUNDLE, "collect.py"))
base = importlib.util.module_from_spec(_spec)
_spec.loader.exec_module(base)

RETIRED = os.path.join("src", "tests", "Broiler.VM.Architecture.Tests", "retired-members.json")
# The surviving-duplicate search, the same constructs Broiler.JS's bundle sjl-3-001 searches for, and the
# .NET conversions this component's sources spell differently.
DUPLICATE_PATTERN = (r'double\.(Try)?Parse\(|decimal\.(Try)?Parse\(|\.ToString\("(R|G17|E[0-9]*|F[0-9]*|N[0-9]*)"'
                     r'|0123456789abcdefghijklmnopqrstuvwxyz|Math\.Pow\(10|char\.IsWhiteSpace|string\.IsNullOrWhiteSpace'
                     r'|\.Trim\(\)|BigInteger\.(Try)?Parse\(|NumberStyles\.(Float|AllowExponent|HexNumber)'
                     r'|Convert\.ToDouble|double\.TryFormat|"R"')
POLYGLOT = ("polyglotcli", "Broiler.VM.Composition.PolyglotCli",
            os.path.join("src", "compositions", "Broiler.VM.Composition.PolyglotCli",
                         "Broiler.VM.Composition.PolyglotCli.csproj"))
INPUTS = "docs/evidence/vm-7-cli-001/inputs"
POLYGLOT_RUNS = [
    ["version"], ["run", INPUTS + "/adder.wasm"],
    ["run", INPUTS + "/adder.wasm", "--invoke", "add", "--arg", "i32:7", "--arg", "i32:35"],
    ["run", INPUTS + "/hello.js", INPUTS + "/adder.wasm"], ["run", INPUTS + "/nottext.wasm"],
    ["run", INPUTS + "/hello.js"]]


class Collector(base.Collector):
    def __init__(self, arguments):
        super().__init__(arguments)
        self.base_tree = os.path.abspath(arguments.base_tree) if arguments.base_tree else None
        self.images = os.path.abspath(arguments.images) if arguments.images else None

    def hide(self, text):
        text = super().hide(text)
        if self.base_tree:
            text = text.replace(self.base_tree, "<base-tree>")
        return text

    def identity(self):
        commit = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        status = self.run(["git", "status", "--porcelain"])[1]
        base_commit = next(line.split(" ", 1)[1] for line in open(
            os.path.join(BASE_BUNDLE, "identity.txt"), encoding="utf-8") if line.startswith("base-commit ")).strip()
        lines = [
            "bundle sjl-3-001",
            "milestone SJL-3, work packages SJL-3.4 and SJL-3.6, Broiler.VM's half",
            "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "after-commit " + commit,
            "after-tree " + ("clean" if not status.strip() else "dirty"),
            "base-commit " + base_commit + " (bundle sjl-0-001)",
            "sdk " + self.run(["dotnet", "--version"])[1].strip(),
            "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
            "suite-pin " + base.PIN + " sha256=" + self.sha256_lf(os.path.join(self.tree, base.PIN)),
            "allowance fuel=%d wallClockMs=%d" % (base.FUEL, base.WALL),
            "jobs %d" % self.jobs,
            "native-backend " + base.NATIVE_BACKEND,
        ]
        if status.strip():
            lines.append("dirty:")
            lines.extend("  " + line for line in status.splitlines())
        path = os.path.join(self.out, "identity.txt")
        if os.path.exists(path):
            os.chmod(path, stat.S_IREAD | stat.S_IWRITE)
        self.write("identity.txt", "\n".join(lines) + "\n")
        os.chmod(path, stat.S_IREAD | stat.S_IRGRP | stat.S_IROTH)
        print("identity: after %s (%s tree)" % (commit, "clean" if not status.strip() else "DIRTY"))
        return 0

    def closures(self):
        path = os.path.join(self.tree, "eng", "collect-js-evidence.py")
        spec = importlib.util.spec_from_file_location("after_collect_js_evidence", path)
        module = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(module)

        class Arguments:
            rid = "win-x64" if platform.system() == "Windows" else "linux-x64"

        out = os.path.join(self.out, "closures")
        os.makedirs(out, exist_ok=True)
        corpus = os.path.join(self.tree, "src", "tests", "corpus", "js-1")
        ok = module.compositions(Arguments, out, corpus)
        # The procedure writes its transcript under one name; PolyglotCli's is kept beside it.
        ok = self.polyglot(module, Arguments.rid, out) and ok
        for entry in os.listdir(out):
            full = os.path.join(out, entry)
            if entry.endswith(".mstat"):
                continue
            with open(full, encoding="utf-8", errors="replace") as stream:
                text = stream.read()
            with open(full, "w", encoding="utf-8", newline="\n") as stream:
                stream.write(self.hide(text))
        print("closures: every root published and run in three modes: %s" % ok)
        return 0 if ok else 1

    def polyglot(self, module, rid, out):
        slug, assembly, project = POLYGLOT
        binary = ".exe" if platform.system() == "Windows" else ""
        closures = ["# closure " + assembly + " rid=" + rid, ""]
        catalogs = {}
        log = []
        ok = True
        for mode, extra in (
                ("jit", ["-r", rid, "--self-contained", "false", "-p:PublishTrimmed=false"]),
                ("trimmed", ["-r", rid, "--self-contained", "true"]),
                ("aot", ["-r", rid, "-p:PublishAot=true", "-p:IlcGenerateMstatFile=true"])):
            directory = os.path.join(module.ROOT, "artifacts", "js-publish", slug, mode)
            shutil.rmtree(directory, ignore_errors=True)
            code, text = module.publish(project, directory, extra, dict(os.environ))
            log.append("[" + slug + "/" + mode + "] publish exit " + str(code) + "\n" + text)
            if code != 0:
                ok = False
                closures += ["[" + mode + "] publish failed", ""]
                continue
            executable = os.path.join(directory, assembly + binary)
            catalog = subprocess.run([executable, "closure"], capture_output=True, text=True,
                                     encoding="utf-8", errors="replace", cwd=module.ROOT)
            catalogs[mode] = catalog.stdout or ""
            log.append("[" + slug + "/" + mode + "] closure exit " + str(catalog.returncode) + "\n" + catalogs[mode])
            for arguments in POLYGLOT_RUNS:
                result = subprocess.run([executable] + arguments, capture_output=True, text=True,
                                        encoding="utf-8", errors="replace", cwd=module.ROOT)
                log.append("[" + slug + "/" + mode + "] " + " ".join(arguments) + " exit " + str(result.returncode)
                           + "\n" + (result.stdout or "") + (result.stderr or ""))
                # nottext.wasm is a refusal by design: bundle UBC-4-002 retains it exiting non-zero.
                if result.returncode != 0 and not arguments[-1].endswith("nottext.wasm"):
                    ok = False
            names = module.closure_of(directory)
            closures.append("[" + mode + "] " + str(len(names)) + " non-framework assemblies")
            closures.extend(names)
            closures.append("")
            if mode == "aot":
                mstat = module.find_mstat(project, rid, assembly)
                if mstat is None:
                    closures.append("[aot] no mstat record of the image was found")
                else:
                    shutil.copyfile(mstat, os.path.join(out, "mstat-" + slug + ".mstat"))
                    closures.append("[aot] the image's record is mstat-" + slug + ".mstat")
                closures.append("")
        distinct = {text for text in catalogs.values()}
        log.append("[" + slug + "] catalog tables identical across modes: " + str(len(distinct) <= 1))
        ok = ok and len(distinct) <= 1
        module.write(os.path.join(out, "catalog-" + slug + ".txt"), next(iter(catalogs.values()), ""))
        module.write(os.path.join(out, "closure-" + slug + ".txt"), "\n".join(closures))
        module.write(os.path.join(out, "publish-and-run-" + slug + ".log"), "\n\n".join(log))
        return ok

    def twins(self):
        binaries = self.build(base.CLI, base.CLI_BINARY)
        if binaries is None:
            print("twins: the CLI root did not build; see build-cli.log")
            return 1
        binary = os.path.join(binaries, base.CLI_BINARY)
        folder = os.path.join(self.tree, base.TWINS)
        figures = {}
        for line in open(os.path.join(BASE_BUNDLE, "fuel-parity.log"), encoding="utf-8"):
            if line.startswith("#") or "|" not in line:
                continue
            cells = [cell.strip() for cell in line.split("|")]
            if cells[1] == "bytecode" and cells[2].isdigit():
                figures[cells[0]] = int(cells[2])
        lines = ["# fuel-parity twins of %s after the cut-over, at each twin's base figure (sjl-0-001/fuel-parity.log) and one less"
                 % base.TWINS.replace(os.sep, "/"),
                 "# twin | form | base figure | at the base figure | at one less"]
        verdict = 0

        def attempt(form, twin, fuel):
            command = [binary]
            if form == "native":
                command += ["--native", base.NATIVE_BACKEND]
            command += ["--fuel", str(fuel), "--wall", str(base.TWIN_WALL), os.path.join(folder, twin)]
            return self.run(command)

        for twin, figure in sorted(figures.items()):
            for form in ("bytecode", "native"):
                done, said_done = attempt(form, twin, figure)
                refused, said = attempt(form, twin, figure - 1)
                below = ("refused with %s" % base.EXHAUSTED_ON_FUEL if refused != 0 and base.EXHAUSTED_ON_FUEL in said
                         else "exit %d: %s" % (refused, (said.strip().splitlines() or ["(no output)"])[-1]))
                at = "completes" if done == 0 else "exit %d: %s" % (
                    done, (said_done.strip().splitlines() or ["(no output)"])[-1])
                lines.append("%s | %s | %d | %s | %s" % (twin, form, figure, at, below))
                if done != 0 or not below.startswith("refused"):
                    verdict = 1
                print("twins: %s %s %d" % (twin, form, figure), flush=True)
        self.write("fuel-parity.log", "\n".join(lines) + "\n")
        return verdict

    def compare(self):
        verdict = 0
        for name, extra, _ in base.RUNS:
            native = "native" in extra
            reports = []
            for side, folder in (("base", BASE_BUNDLE), ("after", self.out)):
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

    def measure(self):
        if not self.base_tree:
            print("measure: arm B needs --base-tree")
            return 1
        binaries = {}
        for arm, tree in (("B", self.base_tree), ("A", self.tree)):
            project = os.path.join(tree, base.CLI, os.path.basename(base.CLI) + ".csproj")
            code, output = self.run(["dotnet", "build", project, "-c", "Release"], cwd=tree)
            self.write("build-cli-%s.log" % arm.lower(), output.strip() + "\nexit code: %d\n" % code)
            if code != 0:
                print("measure: the CLI root did not build for arm %s" % arm)
                return 1
            copy = os.path.join(self.scratch, "cli-" + arm)
            shutil.copytree(os.path.join(tree, base.CLI, "bin", "Release", "net10.0"), copy, dirs_exist_ok=True)
            binaries[arm] = os.path.join(copy, base.CLI_BINARY)
        binaries["A-again"] = binaries["A"]
        folder = os.path.join(self.tree, base.WORKLOADS)
        out = os.path.join(self.out, "measurement")
        if os.path.isdir(out):
            for root, _, files in os.walk(out):
                for name in files:
                    os.chmod(os.path.join(root, name), stat.S_IREAD | stat.S_IWRITE)
            shutil.rmtree(out)
        os.makedirs(out)
        environment = dict(os.environ)
        environment["DOTNET_CLI_UI_LANGUAGE"] = "en"

        def child(arm, form, path):
            command = [binaries[arm], "--runtime"]
            if form == "native":
                command += ["--native", base.NATIVE_BACKEND]
            command += ["--fuel", str(base.MEASURE_FUEL), "--wall", str(base.MEASURE_WALL), path]
            started = time.perf_counter()
            done = subprocess.run(command, capture_output=True, encoding="utf-8", errors="replace", env=environment)
            elapsed = (time.perf_counter() - started) * 1000.0
            answer = (done.stdout or "").strip().splitlines()
            runtime = next((line.strip() for line in (done.stderr or "").splitlines()
                            if line.startswith(base.RUNTIME_PREFIX)), "")
            return done.returncode, answer[-1] if answer else "", runtime, elapsed

        base_commit = self.run(["git", "rev-parse", "HEAD"], cwd=self.base_tree)[1].strip()
        recorded = next(line.split(" ", 1)[1] for line in open(os.path.join(BASE_BUNDLE, "identity.txt"), encoding="utf-8")
                        if line.startswith("base-commit "))
        retained = {line.split()[1]: line.split()[2] for line in open(os.path.join(BASE_BUNDLE, "measurement", "manifest.txt"),
                                                                       encoding="utf-8") if line.startswith("workload ")}
        manifest = [
            "sub-bundle sjl-3-001/measurement",
            "evidence-class measurement",
            "decision docs/evidence/sjl-3-001/decision-rule.md part C, as written: MET if and only if every lane's "
            "condition passes before and after for every workload and form, every effective-configuration line is "
            "identical across arms, lanes and repetitions, and for every workload and form D <= N.",
            "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "arm-B-commit %s (bundle sjl-0-001's base commit: %s)" % (base_commit, recorded.split(" ")[0]),
            "arm-A-commit " + self.run(["git", "rev-parse", "HEAD"])[1].strip(),
            "arm-A-again the arm A binaries, measured a second time under another lane name",
        ]
        for arm in ("B", "A"):
            manifest.append("binary arm %s %s.dll sha256=%s" % (arm, base.CLI_BINARY, self.sha256(binaries[arm] + ".dll")))
        manifest += [
            "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
            "repetitions %d retained per lane, %d warm-up excluded from Q" % (base.REPETITIONS, base.WARMUP),
            "allowance fuel=%d wallClockMs=%d" % (base.MEASURE_FUEL, base.MEASURE_WALL),
            "native-backend " + base.NATIVE_BACKEND,
        ]
        for stem in base.WORKLOAD_STEMS:
            for name in (stem + ".js", stem + ".control.js"):
                digest = "sha256=" + self.sha256_lf(os.path.join(folder, name))
                manifest.append("workload %s %s (as at the base reading: %s)" % (name, digest, retained.get(name) == digest))
        path = os.path.join(out, "manifest.txt")
        with open(path, "w", encoding="utf-8", newline="\n") as stream:
            stream.write(self.hide("\n".join(manifest) + "\n"))
        os.chmod(path, stat.S_IREAD | stat.S_IRGRP | stat.S_IROTH)

        arms = ("B", "A", "A-again")
        runs = ["form\tworkload\trepetition\tretained\tarm\tfile\texit\tanswer-ok\twall-ms"]
        conditions = ["form\tworkload\tarm\tfile\twhen\texit\tanswer\tequals arm B's"]
        summary = ["# Q is the median of the retained repetitions, in ms; N = |Q(A) - Q(A-again)|; D = Q(A) - Q(B);",
                   "# E(arm) = Q(workload, arm) - Q(control, arm)",
                   "# form | workload | Q(B) | Q(A) | Q(A-again) | N | D | D <= N | E(B) | E(A)"]
        configurations = set()
        verdict = 0
        for form in ("bytecode", "native"):
            for stem in base.WORKLOAD_STEMS:
                files = (stem + ".js", stem + ".control.js")
                expected = {}
                for arm in arms:
                    for name in files:
                        code, answer, runtime, _ = child(arm, form, os.path.join(folder, name))
                        if arm == "B":
                            expected[name] = answer
                        equal = code == 0 and answer == expected.get(name)
                        conditions.append("%s\t%s\t%s\t%s\tbefore\t%d\t%s\t%s" % (form, stem, arm, name, code, answer, equal))
                        verdict |= 0 if equal else 1
                times = {(arm, name): [] for arm in arms for name in files}
                for repetition in range(base.WARMUP + base.REPETITIONS):
                    kept = repetition >= base.WARMUP
                    order = arms[repetition % 3:] + arms[:repetition % 3]
                    for arm in order:
                        for name in files:
                            code, answer, runtime, elapsed = child(arm, form, os.path.join(folder, name))
                            ok = code == 0 and answer == expected[name]
                            configurations.add(runtime)
                            runs.append("%s\t%s\t%d\t%s\t%s\t%s\t%d\t%s\t%.3f" % (
                                form, stem, repetition, "yes" if kept else "warm-up", arm, name, code,
                                "yes" if ok else "NO", elapsed))
                            verdict |= 0 if ok else 1
                            if kept:
                                times[(arm, name)].append(elapsed)
                for arm in arms:
                    for name in files:
                        code, answer, runtime, _ = child(arm, form, os.path.join(folder, name))
                        equal = code == 0 and answer == expected[name]
                        conditions.append("%s\t%s\t%s\t%s\tafter\t%d\t%s\t%s" % (form, stem, arm, name, code, answer, equal))
                        verdict |= 0 if equal else 1
                q = {key: statistics.median(values) for key, values in times.items()}
                workload, control = files
                floor = abs(q[("A", workload)] - q[("A-again", workload)])
                difference = q[("A", workload)] - q[("B", workload)]
                summary.append("%s | %s | %.3f | %.3f | %.3f | %.3f | %.3f | %s | %.3f | %.3f" % (
                    form, stem, q[("B", workload)], q[("A", workload)], q[("A-again", workload)], floor, difference,
                    "yes" if difference <= floor else "NO",
                    q[("B", workload)] - q[("B", control)], q[("A", workload)] - q[("A", control)]))
                print("measure: %s %s D=%.3f N=%.3f" % (form, stem, difference, floor), flush=True)
        summary.append("# effective-configuration lines seen: %d" % len(configurations))
        summary.extend("#   " + line for line in sorted(configurations))
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

    def rerun(self):
        if not self.base_tree or not self.suite:
            print("rerun: needs --base-tree and --suite")
            return 1
        sides = {}
        for side, tree in (("base", self.base_tree), ("after", self.tree)):
            directory = os.path.join(tree, base.CONFORMANCE, "bin", "Release", "net10.0")
            if not os.path.isfile(os.path.join(directory, base.CONFORMANCE_BINARY + ".dll")):
                code, output = self.run(["dotnet", "build", os.path.join(tree, base.CONFORMANCE,
                                         os.path.basename(base.CONFORMANCE) + ".csproj"), "-c", "Release"], cwd=tree)
                if code != 0:
                    print("rerun: the %s conformance root did not build" % side)
                    return 1
            sides[side] = (tree, os.path.join(directory, base.CONFORMANCE_BINARY))
        verdict = 0
        for name, extra, _ in base.RUNS:
            rows = []
            for line in open(os.path.join(self.out, "compare-%s.log" % name), encoding="utf-8"):
                found = re.match(r"^  (test/\S+) \[(\w+)\]: (\S+) -> (\S+)", line)
                if found:
                    rows.append(found.groups())
            paths = sorted({row[0] for row in rows})
            reports = {}
            log = ["# part A, class C4 (c'): the %d variants of %d paths compare-%s.log admits as (c'), run again alone"
                   % (len(rows), len(paths), name)]
            for side in ("base", "after"):
                tree, binary = sides[side]
                report = os.path.join(self.scratch, "rerun-%s-%s.report" % (name, side))
                command = [binary, "--test262", self.suite, "--manifest", "broiler.javascript.wide",
                           "--expect", os.path.join(tree, base.PIN), "--report", report,
                           "--fuel", str(base.FUEL), "--wall", str(base.WALL)] + (extra or ["--form", "bytecode"])
                for path in paths:
                    command += ["--test", path]
                started = datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")
                code, output = self.run(command, cwd=tree)
                log.append("## %s: %s.dll sha256=%s, started %s, exit %d\n%s" % (
                    side, base.CONFORMANCE_BINARY, self.sha256(binary + ".dll"), started, code,
                    self.hide(output).replace(self.base_tree, "<base-tree>").strip()[-4000:]))
                reports[side] = report
                if os.path.isfile(report):
                    with open(report, "rb") as source, gzip.GzipFile(
                            os.path.join(self.out, "rerun-%s-%s.report.gz" % (name, side)), "wb", compresslevel=9,
                            mtime=0) as target:
                        shutil.copyfileobj(source, target)
            code, output = self.run([sys.executable, os.path.join(self.tree, "eng", "compare-test262-forms.py"),
                                     reports["base"], reports["after"], "--same-form", "--suite", self.suite]
                                    + (["--exempt-guest-loads"] if "native" in extra else []))
            log.append("## the two re-runs compared, --same-form\n%s\nexit code: %d" % (output.strip(), code))
            self.write("rerun-%s.log" % name, "\n\n".join(log) + "\n")

            def verdicts(report):
                table = {}
                if os.path.isfile(report):
                    for line in open(report, encoding="utf-8"):
                        if line.startswith("result|"):
                            parts = line.rstrip("\n").split("|")
                            verdict_text = parts[3] + ("/" + parts[5] if parts[3] == "Exhausted" and len(parts) > 5 else "")
                            table[(parts[1], parts[2])] = verdict_text
                return table

            base_rerun, after_rerun = verdicts(reports["base"]), verdicts(reports["after"])
            table = ["path\tvariant\tbase run\tafter run\tbase re-run\tafter re-run\tre-runs agree"]
            disagreeing = 0
            for path, variant, before, after in rows:
                key = (path, variant)
                agree = key in base_rerun and base_rerun.get(key) == after_rerun.get(key)
                disagreeing += 0 if agree else 1
                table.append("\t".join((path, variant, before, after, base_rerun.get(key, "(absent)"),
                                        after_rerun.get(key, "(absent)"), "yes" if agree else "NO")))
            self.write("rerun-%s.tsv" % name, "\n".join(table) + "\n")
            print("rerun: %s %d rows, %d whose re-runs disagree" % (name, len(rows), disagreeing), flush=True)
            verdict |= 1 if disagreeing else 0
        return verdict

    def rules(self):
        # Rule I3 reads every mstat-<root>.mstat under src/ as that root's Native AOT image. No bundle
        # commits the records (they are megabytes each; closures/mstat.sha256 names them), so the
        # closures step's records are placed in the tree for this run alone and removed after it.
        images = os.path.join(self.tree, "src", "Broiler.VM.Profile.JavaScript", "docs", "evidence", "sjl-3-001-images")
        source = self.images or os.path.join(self.out, "closures")
        placed = sorted(name for name in os.listdir(source) if name.endswith(".mstat")) if os.path.isdir(source) else []
        shutil.rmtree(images, ignore_errors=True)
        if placed:
            os.makedirs(images)
            for name in placed:
                shutil.copyfile(os.path.join(source, name), os.path.join(images, name))
        try:
            code, output = self.run(["dotnet", "test", os.path.join("src", "tests", "Broiler.VM.Architecture.Tests"),
                                     "-c", "Release", "--nologo", "--logger", "console;verbosity=normal"])
        finally:
            shutil.rmtree(images, ignore_errors=True)
        header = ("# the after tree at %s; the image records placed for rule I3 for this run alone: %s\n"
                  % (self.run(["git", "rev-parse", "HEAD"])[1].strip(),
                     ", ".join("%s sha256=%s" % (name, self.sha256(os.path.join(source, name))) for name in placed) or "none"))
        self.write("rules.log", header + output.strip() + "\nexit code: %d\n" % code)
        print("rules: exit %d" % code)
        return code

    def grep(self, arguments, paths):
        return self.run(["git", "grep", "-n"] + arguments + ["--"] + paths)[1].splitlines()

    def simplification(self):
        out = os.path.join(self.out, "simplification")
        os.makedirs(out, exist_ok=True)
        after = self.run(["git", "rev-parse", "HEAD"])[1].strip()
        family = [p for p in sorted(os.listdir(os.path.join(self.tree, "src")))
                  if p.startswith("Broiler.VM.Profile.JavaScript")]
        family_paths = [os.path.join("src", p, "*.cs") for p in family]
        register = json.load(open(os.path.join(self.tree, RETIRED), encoding="utf-8"))["retired"]
        lines = ["# SJL-3.7 deletion check, Broiler.VM at %s: every member rule I5 registers (%s), its simple"
                 % (after, RETIRED.replace(os.sep, "/")),
                 "# name searched as a declaration - a modifier, then the name as a whole word - in the JavaScript",
                 "# family's C# sources (%s). A member listed with no line is declared nowhere there."
                 % ", ".join(p.replace(os.sep, "/") for p in family_paths),
                 "# Rule I5 itself decides by the fully qualified name, and its run is rules.log.", ""]
        absent = 0
        for entry in register:
            simple = entry["member"].rsplit(".", 1)[-1]
            hits = self.grep(["-E", r"\b(static|private|internal|public|protected|const|readonly)\b[^=;(]*\b%s\b\s*[(=;{<]"
                              % re.escape(simple)], family_paths)
            absent += not hits
            lines += ["## %s (%s)" % (entry["member"], entry["milestone"])] + hits + [""]
        lines.append("# registered members with no declaration of their simple name: %d" % absent)
        lines.append("# registered members: %d" % len(register))
        self.write(os.path.join("simplification", "deletion-check.log"), "\n".join(lines) + "\n")

        product = ["src/*.cs", ":(exclude)src/tests", ":(exclude,glob)src/**/tests/**"]
        hits = self.grep(["-E", DUPLICATE_PATTERN], product)
        search = ["# SJL-3.7 search for a surviving duplicate of the number area, Broiler.VM at %s: every construct" % after,
                  "# that converts between numbers and text, rounds by powers of ten, or classifies white space with a",
                  "# table other than the library's, in the component's C# product sources (src/, every tests/ excluded).",
                  "# pattern: " + DUPLICATE_PATTERN, "# hits: %d; each is reviewed in the bundle's README" % len(hits), ""]
        search += [hit[:220] for hit in hits]
        self.write(os.path.join("simplification", "duplicate-search.log"), "\n".join(search) + "\n")
        print("simplification: %d registered members searched, %d duplicate-search hits" % (len(register), len(hits)))
        return 0


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--tree", required=True, help="a checkout at the cut-over commit")
    parser.add_argument("--base-tree", help="a checkout at bundle sjl-0-001's base commit")
    parser.add_argument("--suite", help="an unpacked checkout of the pinned test262 revision")
    parser.add_argument("--out", required=True)
    parser.add_argument("--jobs", type=int, default=max(1, os.cpu_count() or 1))
    parser.add_argument("--against-broiler-js", help="the BroilerJS apphost built at the named commit")
    parser.add_argument("--against-root", help="the Broiler.JS checkout it was built from")
    parser.add_argument("--images", help="the closures step's mstat records, for rule I3 (rules)")
    parser.add_argument("--only", action="append", required=True,
                        choices=("identity", "closures", "test262", "compare", "rerun", "twins", "differential", "measure",
                                 "rules", "simplification"))
    arguments = parser.parse_args()

    collector = Collector(arguments)
    verdict = 0
    for step in arguments.only:
        verdict |= getattr(collector, step)()
    shutil.rmtree(collector.scratch, ignore_errors=True)
    return verdict


if __name__ == "__main__":
    sys.exit(main())
