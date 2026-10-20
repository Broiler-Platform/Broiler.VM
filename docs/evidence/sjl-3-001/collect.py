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

THE REVISED MEASUREMENT WAS ADDED ON 2026-10-20, after measure's run was seen, with
decision-rule-revised-2026-10-20.md, which moves part C's arm A and gives to-string.js a condition of its own,
held by a transcript. measure and every other step are unchanged.

    python3 docs/evidence/sjl-3-001/collect.py --tree <a clean checkout of arm A> --default-branch origin/main \
        --base-tree <a clean checkout of 0087e8d73ae3833200fa263b68f71db15b059a9c> \
        --numref <Broiler.JS scripts/conformance/numref.py at 966140ad> --out docs/evidence/sjl-3-001 \
        --only measure-revised [--check-only]

  Arm A is the commit on the default branch's first-parent history that adds the revised rule: the merge that
  lands it, whose first parent does not carry it. --default-branch names the default branch's ref.

  measure-revised
             part C under the revised rule, into measurement-revised/, a sub-bundle of its own, in this order:
             1. the refusals, before anything is built (exit 2, every one printed): each arm's tree is clean
                and holds no ignored file but bin/ and obj/ directories and Python's caches (arm A's may also
                hold this step's own sub-bundle, when --out is in it); arm A's HEAD is on --default-branch's
                first-parent history, adds the revised rule there (its first parent does not carry it) and
                descends from aaa39fb, and outside the documents it differs from aaa39fb only under the four
                paths the rule's header names, every path added, changed or removed counted (a moved file is
                both: the comparison takes no rename); no project of the command-line root's closure lies under
                those paths, no project file of it and no build file names them, and no differing path under a
                docs/ directory is one a build compiles;
                the six workloads have the blobs manifest.json names, and their loop and the transcript
                program's are the checker's; arm B's tree is clean at sjl-0-001's base commit; the reference has
                the rule's SHA-256; no .NET build server or build is running; the counts the rule cites read as
                the collector's; and measurement-revised/ holds no completed run (one without a summary, a
                stopped run, is moved aside to measurement-revised-stopped-<n>/ and retained);
             2. the builds: in each tree, every ignored bin/ and obj/ directory removed, then the command-line
                root built in Release, with node reuse, the MSBuild server and the shared compiler off, every
                file of each output listed by hash in binaries-<arm>.sha256;
             3. manifest.txt, written and made read-only: the rules, the collector, the checker, the program and
                the reference, each arm's commit and binaries, the root's closure, every commit since the first
                run's after commit 49ea00e with its files outside the documents, the workloads by digest and
                blob, the protocol, the machine and the load average at the start;
             4. every arm's first check of every file, in each form, before any lane; then
                transcript/to-string.transcript.js once per arm and form, kept as transcript-<arm>-<form>.txt.gz,
                and the transcript check of transcript/check.py, into transcript-check.tsv (every line the arms
                differ on, and every line of arm A the specification does not admit) and transcript-check.txt;
             5. the lanes, as measure takes them: per form and workload, three warm-up repetitions and seven
                retained, the arms' order rotating by repetition, each arm running the workload and then its
                control, every child run with --runtime; then every answer checked again. The expected answer is
                arm B's first for every file but to-string.js, and for to-string.js the lane's own arm's first,
                arm A's for A-again;
             6. runs.tsv, conditions.tsv, summary.txt (with the load average at the end) and hashes.txt.
             With --check-only it takes 1 and 2 only, builds into its scratch directory, writes nothing under
             --out, and runs no transcript, no answer and no lane.
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

# The revised rule of 2026-10-20 (measure-revised). Arm A is the commit that adds it to the default branch's
# history, whose product sources are aaa39fb's - the default branch after the move to 0.1.0-preview.7 - but for the
# four paths below: the ones bundle sjl-12-001's rule lets change before its corpus commit beyond the documents,
# none of which a build of the command-line root reads. The step checks that last claim at arm A before any build.
REVISED_RULE = "docs/evidence/sjl-3-001/decision-rule-revised-2026-10-20.md"
ORIGINAL_RULE = "docs/evidence/sjl-3-001/decision-rule.md"
PROGRAM = "docs/evidence/sjl-3-001/transcript/to-string.transcript.js"
CHECKER = "docs/evidence/sjl-3-001/transcript/check.py"
COLLECTOR = "docs/evidence/sjl-3-001/collect.py"
PREVIEW_7 = "aaa39fbc2030e1a2554317bea75bcbb56041c696"
FIRST_AFTER = "49ea00effa457ac8d5b05f34cbae718b862007b2"
NOT_BUILT = ("src/tests/corpus/parse-1/", "src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/",
             "eng/compare-parse-corpus.py", "eng/check-frontend-mapping.py")
# What a build reads under a docs/ directory if it is there: C# sources and resources (the SDK's default items) and
# MSBuild files.
BUILD_INPUT_SUFFIXES = (".cs", ".resx", ".csproj", ".props", ".targets")
MEASUREMENT_REVISED = "measurement-revised"
ARMS = ("B", "A", "A-again")
FORMS = ("bytecode", "native")


def load_module(name, path):
    spec = importlib.util.spec_from_file_location(name, path)
    module = importlib.util.module_from_spec(spec)
    sys.modules[name] = module
    spec.loader.exec_module(module)
    return module


transcript = load_module("sjl_3_001_transcript_check", os.path.join(HERE, "transcript", "check.py"))


def outside_documents(names):
    """The paths of a list outside docs/ and outside any docs/ directory."""
    return [name for name in names if name and not name.startswith("docs/") and "/docs/" not in name]


# Ignored entries of `git status --porcelain --ignored=traditional --untracked-files=normal` a tree may hold: build
# outputs, which the step removes before each build, and Python's caches, which running this collector from arm A's
# tree writes. Any other ignored file - a <project>.csproj.user, say, which MSBuild imports - can change a build, and
# refuses the run.
BUILD_OUTPUTS = ("bin", "obj")
PYTHON_CACHES = ("__pycache__",)


def ignored_entries(status):
    """The ignored entries of that status, split into the build-output directories, the Python caches and every
    other one."""
    outputs, caches, others = [], [], []
    for line in status.splitlines():
        if not line.startswith("!! "):
            continue
        path = line[3:]
        last = path.rstrip("/").rsplit("/", 1)[-1]
        if path.endswith("/") and last in BUILD_OUTPUTS:
            outputs.append(path)
        elif (path.endswith("/") and last in PYTHON_CACHES) or path.endswith(".pyc"):
            caches.append(path)
        else:
            others.append(path)
    return outputs, caches, others


def not_built(path):
    return any(path == prefix or (prefix.endswith("/") and path.startswith(prefix)) for prefix in NOT_BUILT)


def project_closure(tree, project):
    """Every project the given one references, followed transitively, as paths relative to the tree."""
    seen, pending = [], [os.path.normpath(os.path.join(tree, project))]
    while pending:
        path = pending.pop()
        if path in seen:
            continue
        seen.append(path)
        with open(path, encoding="utf-8") as stream:
            text = stream.read()
        for reference in re.findall(r'<ProjectReference\s+Include="([^"]+)"', text):
            pending.append(os.path.normpath(os.path.join(os.path.dirname(path), reference.replace("\\", "/"))))
    return sorted(os.path.relpath(path, tree).replace(os.sep, "/") for path in seen)


def build_processes():
    """Every running .NET build server or build: the compiler server, an MSBuild node or MSBuild itself, or a
    dotnet command that builds. None when the platform cannot be read."""
    if not os.path.isdir("/proc"):
        return None
    found = []
    for entry in os.listdir("/proc"):
        if not entry.isdigit() or int(entry) == os.getpid():
            continue
        try:
            with open(os.path.join("/proc", entry, "cmdline"), "rb") as stream:
                words = [word.decode("utf-8", "replace") for word in stream.read().split(b"\0") if word]
        except OSError:
            continue
        if not words:
            continue
        names = [os.path.basename(word) for word in words]
        if (any(name.startswith("VBCSCompiler") or name == "MSBuild.dll" or name == "rzc.dll" for name in names)
                or (names[0] == "dotnet" and len(names) > 1
                    and names[1] in ("build", "test", "run", "publish", "pack", "restore", "msbuild", "build-server"))):
            found.append("pid %s: %s" % (entry, " ".join(words)[:160]))
    return found


def load_average():
    try:
        return "%.2f %.2f %.2f" % os.getloadavg()
    except OSError:
        return "(unavailable)"


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

    # ---- part C under the revised rule of 2026-10-20 ----------------------------------------------
    def git(self, tree, *arguments):
        return self.run(["git"] + list(arguments), cwd=tree)

    def changed_paths(self, tree, older, newer):
        """Every path added, changed or removed between two commits, read with no rename detection, so a moved file
        is listed at the path it left as well as the path it took. Its standard output only, NUL-separated, so no
        warning and no quoting is read as a path."""
        done = subprocess.run(["git", "diff", "--no-renames", "--no-ext-diff", "--name-only", "-z", older, newer],
                              cwd=tree, capture_output=True, env=dict(os.environ, LC_ALL="C"))
        names = [name.decode("utf-8", "surrogateescape") for name in done.stdout.split(b"\0") if name]
        return done.returncode, names, done.stderr.decode("utf-8", "replace")

    def build_files(self, closure):
        """The repository's build files a build of the closure reads: each Directory.Build.props or .targets from a
        project's directory up to the root, the root's Directory.Packages.props, NuGet.config and global.json, and
        every file one of them imports."""
        found = []
        for project in closure:
            folder = os.path.dirname(project)
            while True:
                for name in ("Directory.Build.props", "Directory.Build.targets"):
                    path = os.path.join(folder, name).replace(os.sep, "/")
                    if os.path.isfile(os.path.join(self.tree, path)) and path not in found:
                        found.append(path)
                if not folder:
                    break
                folder = os.path.dirname(folder)
        for name in ("Directory.Packages.props", "NuGet.config", "global.json"):
            if os.path.isfile(os.path.join(self.tree, name)) and name not in found:
                found.append(name)
        index = 0
        while index < len(found):
            path = found[index]
            index += 1
            with open(os.path.join(self.tree, path), encoding="utf-8", errors="replace") as stream:
                text = stream.read()
            for imported in re.findall(r'<Import\s+Project="([^"]+)"', text):
                imported = imported.replace("$(MSBuildThisFileDirectory)", "").replace("\\", "/")
                if "$(" in imported:
                    continue
                relative = os.path.normpath(os.path.join(os.path.dirname(path), imported)).replace(os.sep, "/")
                if os.path.isfile(os.path.join(self.tree, relative)) and relative not in found:
                    found.append(relative)
        return found

    def measure_revised(self):
        """Part C of decision-rule-revised-2026-10-20.md. Every check that can refuse runs before anything is built;
        with --check-only the step builds both arms into its scratch directory and stops."""
        step = "measure-revised"
        check_only = bool(getattr(self.arguments, "check_only", False))
        numref_path = os.path.abspath(self.arguments.numref) if getattr(self.arguments, "numref", None) else None
        started_load = load_average()

        def hide(text):
            text = self.hide(text)
            return text.replace(numref_path, "<numref>") if numref_path else text

        problems = []
        base_commit = next(line.split(" ", 1)[1] for line in open(os.path.join(BASE_BUNDLE, "identity.txt"),
                                                                     encoding="utf-8")
                           if line.startswith("base-commit ")).strip()
        trees = {"A": self.tree, "B": self.base_tree}
        commits = {}
        outputs = {}
        if not self.base_tree:
            problems.append("arm B needs --base-tree, a clean checkout of %s" % base_commit)
        if not numref_path:
            problems.append("the transcript check needs --numref, Broiler.JS's scripts/conformance/numref.py at %s"
                            % transcript.NUMREF_COMMIT)
        for arm in ("A", "B"):
            if not trees[arm]:
                continue
            code, head = self.git(trees[arm], "rev-parse", "HEAD")
            commits[arm] = head.strip() if code == 0 else "(not a checkout)"
            if code != 0:
                problems.append("arm %s: %s is not a checkout" % (arm, trees[arm]))
                continue
            status = self.git(trees[arm], "status", "--porcelain", "--ignored=traditional", "--untracked-files=normal")[1]
            # The step's own sub-bundle, or a stopped run of it, may be in arm A's tree when --out is there.
            dirty = [line for line in status.splitlines() if line.strip() and not line.startswith("!! ")
                     and not (arm == "A" and line.startswith("?? docs/evidence/sjl-3-001/" + MEASUREMENT_REVISED))]
            outputs[arm], _, ignored = ignored_entries(status)
            if dirty:
                problems.append("arm %s: the tree is not clean: %s" % (arm, "; ".join(dirty[:10])))
            if ignored:
                problems.append("arm %s: the tree holds ignored files a build may read (only bin/ and obj/ directories,"
                                " which are removed before its build, and Python's caches may be there): %s"
                                % (arm, "; ".join(ignored[:10]) + (" ..." if len(ignored) > 10 else "")))
        if commits.get("B") and commits["B"] != base_commit:
            problems.append("arm B: the tree is at %s, not bundle sjl-0-001's base commit %s" % (commits["B"], base_commit))

        # Arm A: the commit on the default branch's first-parent history that adds the revised rule, on aaa39fb's
        # product sources but for the paths the root does not build.
        if self.git(self.tree, "cat-file", "-e", "HEAD:" + REVISED_RULE)[0] != 0:
            problems.append("arm A: HEAD does not carry %s" % REVISED_RULE)
        elif self.git(self.tree, "cat-file", "-e", "HEAD^1:" + REVISED_RULE)[0] == 0:
            problems.append("arm A: HEAD's first parent already carries %s; arm A is the commit that adds it to the"
                            " default branch's history, not a later one" % REVISED_RULE)
        default_ref = getattr(self.arguments, "default_branch", None)
        default_commit = None
        if not default_ref:
            problems.append("arm A needs --default-branch, the default branch's ref (origin/main, say): arm A is the"
                            " commit on its first-parent history that adds %s" % REVISED_RULE)
        else:
            code, resolved = self.git(self.tree, "rev-parse", "--verify", "--quiet", default_ref + "^{commit}")
            if code != 0:
                problems.append("arm A: --default-branch %s does not name a commit in arm A's repository" % default_ref)
            else:
                default_commit = resolved.strip()
                chain = self.git(self.tree, "rev-list", "--first-parent", default_commit)[1].split()
                if commits.get("A") not in chain:
                    problems.append("arm A: HEAD is not on the first-parent history of --default-branch %s (%s); arm A"
                                    " is the commit there that adds %s, the merge that lands it"
                                    % (default_ref, default_commit, REVISED_RULE))
        if self.git(self.tree, "merge-base", "--is-ancestor", PREVIEW_7, "HEAD")[0] != 0:
            problems.append("arm A: HEAD does not descend from %s" % PREVIEW_7)
        code, names, error = self.changed_paths(self.tree, PREVIEW_7, "HEAD")
        if code != 0:
            problems.append("arm A: git diff --no-renames --name-only %s HEAD failed: %s" % (PREVIEW_7, error.strip()[-300:]))
            names = []
        product = outside_documents(names)
        differing = [name for name in product if not not_built(name)]
        exempted = [name for name in product if not_built(name)]
        if differing:
            problems.append("arm A: outside the documents it differs from %s in %d path(s) the rule does not exempt: %s"
                            % (PREVIEW_7, len(differing), ", ".join(differing[:25]) + (" ..." if len(differing) > 25 else "")))
        compiled = [name for name in names if (name.startswith("docs/") or "/docs/" in name)
                    and name.endswith(BUILD_INPUT_SUFFIXES)]
        if compiled:
            problems.append("arm A: a path under a docs/ directory that a build compiles or reads differs from %s: %s"
                            % (PREVIEW_7, ", ".join(compiled[:25])))
        cli_project = (base.CLI + "/" + os.path.basename(base.CLI) + ".csproj").replace(os.sep, "/")
        try:
            closure = project_closure(self.tree, cli_project)
            build_files = self.build_files(closure)
        except OSError as error:
            closure, build_files = [], []
            problems.append("arm A: the command-line root's closure could not be read: %s" % error)
        under = [project for project in closure if not_built(project)]
        if under:
            problems.append("arm A: the command-line root's closure holds a project under a path the rule exempts: %s"
                            % ", ".join(under))
        # Each path's last component, so a relative reference is caught as well as a rooted one.
        markers = [prefix.rstrip("/").rsplit("/", 1)[-1] for prefix in NOT_BUILT]
        naming = []
        for path in closure + build_files:
            with open(os.path.join(self.tree, path), encoding="utf-8", errors="replace") as stream:
                text = stream.read().replace("\\", "/")
            naming += ["%s names %s" % (path, marker) for marker in markers if marker in text]
        if naming:
            problems.append("arm A: a project or build file of the command-line root names a path the rule exempts: %s"
                            % "; ".join(naming))

        # The workloads, byte for byte the rule's, and the transcript program's loop the workload's.
        sources = json.load(open(os.path.join(HERE, "manifest.json"), encoding="utf-8"))["sources"]
        workloads = []
        for stem in base.WORKLOAD_STEMS:
            for name in (stem + ".js", stem + ".control.js"):
                relative = (base.WORKLOADS + "/" + name).replace(os.sep, "/")
                path = os.path.join(self.tree, relative)
                if not os.path.isfile(path):
                    problems.append("workload %s is missing from arm A's tree" % relative)
                    continue
                blob = self.git(self.tree, "hash-object", path)[1].strip()
                if blob != sources.get(relative):
                    problems.append("workload %s has blob %s; the bundle's manifest.json names %s"
                                    % (relative, blob, sources.get(relative)))
                workloads.append((relative, self.sha256_lf(path), blob))
        for what, relative in (("the workload to-string.js", base.WORKLOADS + "/to-string.js"),
                               ("the transcript program", PROGRAM)):
            path = os.path.join(self.tree, relative)
            if not os.path.isfile(path) or transcript.loop_of(open(path, encoding="utf-8").read()) != transcript.expected_loop():
                problems.append("%s's loop is not the transcript checker's (values, bound, scale and conversions)" % what)

        # The reference, the counts the rule cites, the machine and the sub-bundle.
        numref = None
        if numref_path:
            try:
                numref = transcript.load_numref(numref_path)
            except (transcript.Refused, OSError) as refusal:
                problems.append(str(refusal))
        compare_forms = open(os.path.join(self.tree, "eng", "compare-forms.py"), encoding="utf-8").read()
        warmup = re.search(r'"--warmup", type=int, default=(\d+)', compare_forms)
        baselines = open(os.path.join(self.tree, "docs", "baselines.md"), encoding="utf-8").read()
        if not warmup or int(warmup.group(1)) != base.WARMUP:
            problems.append("eng/compare-forms.py's warm-up default is not %d" % base.WARMUP)
        if not re.search(r"\| 4 \| \*\*Every repetition retained\.\*\* Seven per lane", baselines) or base.REPETITIONS != 7:
            problems.append("docs/baselines.md rule 4 does not read seven per lane")
        servers = build_processes()
        if servers is None:
            problems.append("this platform's processes cannot be listed, so no .NET build server can be ruled out")
        elif servers:
            problems.append("a .NET build server or build is running: %s" % "; ".join(servers))
        out = os.path.join(self.out, MEASUREMENT_REVISED)
        stopped = None
        if os.path.isdir(out) and os.listdir(out):
            if os.path.isfile(os.path.join(out, "summary.txt")):
                problems.append("%s/ holds a completed measurement; a measurement is not run again" % MEASUREMENT_REVISED)
            else:
                number = 1
                while os.path.exists(os.path.join(self.out, "%s-stopped-%d" % (MEASUREMENT_REVISED, number))):
                    number += 1
                stopped = os.path.join(self.out, "%s-stopped-%d" % (MEASUREMENT_REVISED, number))

        children = len(FORMS) * len(base.WORKLOAD_STEMS) * len(ARMS) * 2 * (2 + base.WARMUP + base.REPETITIONS)
        plan = [
            "%s: part C of %s%s" % (step, REVISED_RULE, " (check only: refusals and builds)" if check_only else ""),
            "  arm B        %s at %s (bundle sjl-0-001's base commit %s)" % (self.base_tree, commits.get("B"), base_commit),
            "  arm A        %s at %s (the commit on the default branch's first-parent history that adds the revised"
            " rule; product sources %s's but for the paths the root does not build)"
            % (self.tree, commits.get("A"), PREVIEW_7),
            "  default      --default-branch %s at %s" % (default_ref, default_commit),
            "  arm A-again  arm A's binaries, measured a second time under another lane name",
            "  exempted     differing from %s outside the documents, under the rule's four paths: %d file(s)%s"
            % (PREVIEW_7, len(exempted), "" if not exempted else " (" + ", ".join(sorted({
                next(prefix for prefix in NOT_BUILT if not_built(name) and (name == prefix or name.startswith(prefix)))
                for name in exempted})) + ")"),
            "  root         %s, built in Release under the JIT in each arm's tree; closure at arm A: %s"
            % (base.CLI.replace(os.sep, "/"), ", ".join(closure)),
            "  build files  %s" % ", ".join(build_files),
            "  forms        %s; native under %s" % (", ".join(FORMS), base.NATIVE_BACKEND),
            "  workloads    %s" % ", ".join(relative.rsplit("/", 1)[1] for relative, _, _ in workloads),
            "  transcript   %s, once per arm and form, held to %s with numref.py at %s"
            % (PROGRAM, CHECKER, transcript.NUMREF_COMMIT),
            "  files        as arm A holds them: %s" % ", ".join("%s blob=%s" % (
                relative.rsplit("/", 1)[1], self.git(self.tree, "rev-parse", "HEAD:" + relative)[1].strip() or "(absent)")
                for relative in (REVISED_RULE, COLLECTOR, CHECKER, PROGRAM)),
            "  repetitions  %d warm-up (excluded from Q) and %d retained per lane; the arms' order rotates by"
            " repetition, and each arm runs the workload, then its control" % (base.WARMUP, base.REPETITIONS),
            "  checks       every arm's first check of every file and form before any lane, and again after its lanes",
            "  allowance    fuel=%d wallClockMs=%d, the same for every lane" % (base.MEASURE_FUEL, base.MEASURE_WALL),
            "  children     %d timed or checked, four transcripts and two builds" % children,
            "  load average %s at the step's start" % started_load,
            "  into         %s" % ("nothing under --out: the builds go to the step's scratch directory" if check_only
                                   else os.path.join(self.out, MEASUREMENT_REVISED)
                                   + ("; the stopped run there moves to %s" % os.path.basename(stopped) if stopped else "")),
        ]
        print(hide("\n".join(plan)), flush=True)
        for problem in problems:
            print("%s: REFUSED: %s" % (step, hide(problem)), flush=True)
        if problems:
            return 2

        # 2. The builds.
        if check_only:
            target = os.path.join(self.scratch, MEASUREMENT_REVISED)
        else:
            target = out
            if stopped:
                os.rename(out, stopped)
                print("%s: the stopped run is retained as %s" % (step, os.path.basename(stopped)), flush=True)
        os.makedirs(target, exist_ok=True)
        environment = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", DOTNET_CLI_TELEMETRY_OPTOUT="1",
                           DOTNET_CLI_USE_MSBUILD_SERVER="0", MSBUILDDISABLENODEREUSE="1", UseSharedCompilation="false")
        binaries, listings = {}, {}
        for arm in ("B", "A"):
            tree = trees[arm]
            # No output or restore file but this build's own: every ignored bin/ and obj/ directory goes first.
            for relative in outputs[arm]:
                shutil.rmtree(os.path.join(tree, relative.rstrip("/")))
            print("%s: arm %s: %d ignored bin/ and obj/ director%s removed before its build"
                  % (step, arm, len(outputs[arm]), "y" if len(outputs[arm]) == 1 else "ies"), flush=True)
            project = os.path.join(tree, base.CLI, os.path.basename(base.CLI) + ".csproj")
            done = subprocess.run(["dotnet", "build", project, "-c", "Release", "--no-incremental", "-nodeReuse:false",
                                   "-p:UseSharedCompilation=false"], cwd=tree, capture_output=True, encoding="utf-8",
                                  errors="replace", env=environment)
            output = ((done.stdout or "") + (done.stderr or "")).strip()
            with open(os.path.join(target, "build-cli-%s.log" % arm.lower()), "w", encoding="utf-8", newline="\n") as stream:
                stream.write(hide(output + "\nexit code: %d\n" % done.returncode))
            print("%s: arm %s build exit %d" % (step, arm, done.returncode), flush=True)
            if done.returncode != 0:
                print(hide(output[-3000:]), flush=True)
                print("%s: the command-line root did not build for arm %s; a build failure before any lane is not a"
                      " run, and its log is retained in %s" % (step, arm, hide(target)), flush=True)
                return 1
            copy = os.path.join(self.scratch, "cli-revised-" + arm)
            shutil.rmtree(copy, ignore_errors=True)
            shutil.copytree(os.path.join(tree, base.CLI, "bin", "Release", "net10.0"), copy)
            binaries[arm] = os.path.join(copy, base.CLI_BINARY)
            entries = []
            for root, _, files in os.walk(copy):
                for name in files:
                    path = os.path.join(root, name)
                    entries.append("%s  %s" % (self.sha256(path), os.path.relpath(path, copy).replace(os.sep, "/")))
            listing = "\n".join(sorted(entries, key=lambda line: line.split("  ", 1)[1])) + "\n"
            path = os.path.join(target, "binaries-%s.sha256" % arm.lower())
            with open(path, "w", encoding="utf-8", newline="\n") as stream:
                stream.write(listing)
            os.chmod(path, stat.S_IREAD | stat.S_IRGRP | stat.S_IROTH)
            listings[arm] = hashlib.sha256(listing.encode("utf-8")).hexdigest()
            print("%s: arm %s %s.dll sha256=%s; binaries-%s.sha256 sha256=%s" % (
                step, arm, base.CLI_BINARY, self.sha256(binaries[arm] + ".dll"), arm.lower(), listings[arm]), flush=True)
        binaries["A-again"] = binaries["A"]
        if check_only:
            print("%s: check only: every refusal check passed and both arms built; no transcript, answer or lane was"
                  " run, and nothing was written under --out" % step, flush=True)
            return 0
        return self.revised_runs(step, out, binaries, listings, commits, exempted, closure, build_files, workloads,
                                 numref, numref_path, started_load, hide, (default_ref, default_commit))

    def revised_runs(self, step, out, binaries, listings, commits, exempted, closure, build_files, workloads, numref,
                     numref_path, started_load, hide, default_branch):
        """measure-revised after its refusals and builds: the manifest, the first checks, the transcripts and their
        check, the lanes and the results, into out."""
        # 3. The manifest, read-only before anything runs.
        def blob(path):
            return self.git(self.tree, "hash-object", path)[1].strip()

        def held(relative):
            return self.git(self.tree, "rev-parse", "HEAD:" + relative)[1].strip()

        since = []
        listing = self.git(self.tree, "log", "--reverse", "--format=%H%x09%P%x09%s", "%s..HEAD" % FIRST_AFTER)[1]
        for entry in listing.splitlines():
            if not entry.strip():
                continue
            sha, parents, subject = entry.split("\t", 2)
            touched = outside_documents(self.changed_paths(self.tree, sha + "^1", sha)[1])
            if " " in parents:
                # A merge's files are those of the commits it brings in, each listed here with its own.
                since.append("  %s %s: a merge, %s against its first parent" % (
                    sha, subject, (str(len(touched)) + " files") if touched else "no files"))
                continue
            since.append("  %s %s: %s" % (sha, subject, (str(len(touched)) + " files") if touched else "none"))
            since.extend("    " + name for name in touched)
        manifest = [
            "sub-bundle sjl-3-001/%s" % MEASUREMENT_REVISED,
            "evidence-class measurement",
            "decision %s part C, as revised: MET if and only if every lane's condition passes, before and after, for"
            " every workload and form (for to-string.js, each arm's own first answer, A-again held to arm A's, and the"
            " transcript check holds); every effective-configuration line is identical across arms, lanes and"
            " repetitions; and for every workload and form D <= N. Weaker evidence: written after measurement/ was"
            " seen." % REVISED_RULE,
            "rule %s blob=%s sha256=%s" % (REVISED_RULE, held(REVISED_RULE), self.sha256_lf(os.path.join(self.tree, REVISED_RULE))),
            "rule-it-revises %s blob=%s sha256=%s" % (ORIGINAL_RULE, held(ORIGINAL_RULE),
                                                      self.sha256_lf(os.path.join(self.tree, ORIGINAL_RULE))),
            "collector %s blob=%s as run, blob=%s as arm A holds it" % (COLLECTOR, blob(os.path.abspath(__file__)),
                                                                        held(COLLECTOR)),
            "checker %s blob=%s as run, blob=%s as arm A holds it" % (CHECKER, blob(transcript.__file__), held(CHECKER)),
            "program %s blob=%s sha256=%s" % (PROGRAM, held(PROGRAM), self.sha256_lf(os.path.join(self.tree, PROGRAM))),
            "reference scripts/conformance/numref.py at Broiler.JS %s sha256=%s" % (transcript.NUMREF_COMMIT,
                                                                                    self.sha256(numref_path)),
            "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
            "load-average %s at the step's start, %s when this manifest was written after both builds"
            % (started_load, load_average()),
            "arm-B-commit %s (bundle sjl-0-001's base commit; clean tree, no ignored file but bin/ and obj/ directories,"
            " removed before its build, and Python's caches)" % commits["B"],
            "arm-A-commit %s (on the first-parent history of --default-branch %s at %s, the commit there that adds the"
            " revised rule: its first parent does not carry it; descends from %s; clean tree, no ignored file but bin/"
            " and obj/ directories, removed before its build, and Python's caches)"
            % (commits["A"], default_branch[0], default_branch[1], PREVIEW_7),
            "arm-A-differs-from %s outside the documents only under the rule's four paths, none of which the root's"
            " closure or build files name, every path added, changed or removed counted, with no rename detection: %s"
            % (PREVIEW_7, ("%d files" % len(exempted)) if exempted else "none"),
        ]
        manifest.extend("  " + name for name in exempted)
        manifest += [
            "arm-A-again the arm A binaries, measured a second time under another lane name",
        ]
        for arm in ("B", "A"):
            manifest.append("binary arm %s %s.dll sha256=%s; every file of its output, binaries-%s.sha256 sha256=%s" % (
                arm, base.CLI_BINARY, self.sha256(binaries[arm] + ".dll"), arm.lower(), listings[arm]))
        manifest += [
            "root %s, Release, under the JIT, built with --no-incremental, node reuse, the MSBuild server and the"
            " shared compiler off" % base.CLI.replace(os.sep, "/"),
            "root-closure %s" % ", ".join(closure),
            "build-files %s" % ", ".join(build_files),
            "rid " + ("win-x64" if platform.system() == "Windows" else "linux-x64"),
            "sdk " + self.run(["dotnet", "--version"])[1].strip(),
            "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
            "protocol first, every arm's first check of every file in each form, before any lane; then the transcript"
            " program once per arm and form, and its check; then, per form and workload, the repetitions; then every"
            " arm's answer for the workload and its control checked again",
            "repetitions %d retained per lane (docs/baselines.md rule 4), %d warm-up excluded from Q"
            " (eng/compare-forms.py's default)" % (base.REPETITIONS, base.WARMUP),
            "order in each repetition r (from 0): the arms rotated by r from B, A, A-again; each arm runs the"
            " workload, then its control",
            "condition to-number.js, literals.js and every control: arm B's first answer; to-string.js: the lane's own"
            " arm's first answer, arm A's for A-again",
            "allowance fuel=%d wallClockMs=%d" % (base.MEASURE_FUEL, base.MEASURE_WALL),
            "forms " + " ".join(FORMS),
            "native-backend " + base.NATIVE_BACKEND,
            "configuration-check every child is run with --runtime; its line starting %s is kept" % base.RUNTIME_PREFIX,
        ]
        for relative, digest, workload_blob in workloads:
            manifest.append("workload %s sha256=%s blob=%s (the blob the bundle's manifest.json names)"
                            % (relative, digest, workload_blob))
        manifest.append("since-first-after-commit %s (measurement/'s arm A), every commit to arm A with its files"
                        " outside the documents:" % FIRST_AFTER)
        manifest.extend(since)
        path = os.path.join(out, "manifest.txt")
        with open(path, "w", encoding="utf-8", newline="\n") as stream:
            stream.write(hide("\n".join(manifest) + "\n"))
        os.chmod(path, stat.S_IREAD | stat.S_IRGRP | stat.S_IROTH)
        print("%s: manifest written and made read-only" % step, flush=True)

        folder = os.path.join(self.tree, base.WORKLOADS)
        child_environment = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en")
        configurations = []

        def child(arm, form, path):
            command = [binaries[arm], "--runtime"]
            if form == "native":
                command += ["--native", base.NATIVE_BACKEND]
            command += ["--fuel", str(base.MEASURE_FUEL), "--wall", str(base.MEASURE_WALL), path]
            started = time.perf_counter()
            done = subprocess.run(command, capture_output=True, encoding="utf-8", errors="replace",
                                  env=child_environment)
            elapsed = (time.perf_counter() - started) * 1000.0
            runtime = next((line.strip() for line in (done.stderr or "").splitlines()
                            if line.startswith(base.RUNTIME_PREFIX)), "(none)")
            lines = (done.stdout or "").strip().splitlines()
            return done.returncode, lines[-1] if lines else "", runtime, elapsed, done.stdout or ""

        def numbered(runtime):
            if runtime not in configurations:
                configurations.append(runtime)
            return configurations.index(runtime) + 1

        first = {}

        def expected_for(form, arm, name):
            source = "A" if name == "to-string.js" and arm != "B" else "B"
            code, answer = first.get((form, source, name), (None, None))
            return (answer if code == 0 else None), "arm %s's first check" % source

        conditions = open(os.path.join(out, "conditions.tsv"), "w", encoding="utf-8", newline="\n")
        runs = open(os.path.join(out, "runs.tsv"), "w", encoding="utf-8", newline="\n")
        conditions.write("form\tworkload\tarm\tfile\twhen\texit\tanswer\tconfiguration\theld to\tequal\n")
        runs.write("form\tworkload\trepetition\tretained\tposition\tarm\tfile\texit\tanswer-ok\tconfiguration\twall-ms\n")
        failed_conditions = failed_runs = within = 0
        summary = ["# part C of %s, measured %s to %%s" % (REVISED_RULE, datetime.datetime.now(
                       datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")),
                   "# Q is the median of the retained repetitions, in ms; N = |Q(A) - Q(A-again)|; D = Q(A) - Q(B);",
                   "# E(arm) = Q(workload, arm) - Q(control, arm), retained beside D and deciding nothing",
                   "# form | workload | Q(B) | Q(A) | Q(A-again) | N | D | D <= N | Q(control, B) | Q(control, A) |"
                   " Q(control, A-again) | E(B) | E(A) | E(A-again)"]
        transcript_holds = False
        try:
            def check(form, stem, when):
                failures = 0
                for arm in ARMS:
                    for name in (stem + ".js", stem + ".control.js"):
                        code, answer, runtime, _, _ = child(arm, form, os.path.join(folder, name))
                        if when == "before":
                            first[(form, arm, name)] = (code, answer)
                        expected, source = expected_for(form, arm, name)
                        equal = code == 0 and expected is not None and answer == expected
                        failures += 0 if equal else 1
                        conditions.write("%s\t%s\t%s\t%s\t%s\t%d\t%s\t%d\t%s\t%s\n" % (
                            form, stem, arm, name, when, code, answer, numbered(runtime), source,
                            "yes" if equal else "NO"))
                        conditions.flush()
                return failures

            # 4. The first checks, then the transcripts and their check, all before any lane.
            for form in FORMS:
                for stem in base.WORKLOAD_STEMS:
                    failed_conditions += check(form, stem, "before")
            print("%s: first checks taken" % step, flush=True)
            transcripts, taken = {}, []
            for arm in ("B", "A"):
                for form in FORMS:
                    code, _, runtime, elapsed, output = child(arm, form, os.path.join(self.tree, PROGRAM))
                    data = output.encode("utf-8")
                    with gzip.GzipFile(os.path.join(out, "transcript-%s-%s.txt.gz" % (arm.lower(), form)), "wb",
                                       compresslevel=9, mtime=0) as stream:
                        stream.write(data)
                    transcripts[(arm, form)] = output if code == 0 else None
                    taken.append("transcript arm %s %s: exit %d, %d bytes, sha256=%s, %.0f ms, configuration: %s" % (
                        arm, form, code, len(data), hashlib.sha256(data).hexdigest(), elapsed, runtime))
                    print("%s: transcript arm %s %s exit %d" % (step, arm, form, code), flush=True)
            reference = transcript.Reference(numref)
            lines = ["# The transcript check of %s, taken before any lane: %s run once per arm and form;" % (
                         REVISED_RULE, PROGRAM),
                     "# (i) and (ii) tie each transcript to the first checks in conditions.tsv; (iii) holds arm A's texts"
                     " to Broiler.JS's numref.py at %s." % transcript.NUMREF_COMMIT, ""] + taken + [""]
            rows = [transcript.TSV_HEADER]
            transcript_holds = True
            for form in FORMS:
                holds, summary_lines, form_rows = transcript.check(
                    reference, form, {"A": transcripts[("A", form)], "B": transcripts[("B", form)]},
                    expected_for(form, "B", "to-string.control.js")[0],
                    {"A": expected_for(form, "A", "to-string.js")[0], "B": expected_for(form, "B", "to-string.js")[0]})
                transcript_holds = transcript_holds and holds
                lines += summary_lines + [""]
                rows += form_rows
            lines.append("transcript check: %s" % ("HOLDS in both forms" if transcript_holds else "DOES NOT HOLD"))
            with open(os.path.join(out, "transcript-check.txt"), "w", encoding="utf-8", newline="\n") as stream:
                stream.write(hide("\n".join(lines) + "\n"))
            with open(os.path.join(out, "transcript-check.tsv"), "w", encoding="utf-8", newline="\n") as stream:
                stream.write("\n".join(rows) + "\n")
            print("%s: transcript check %s; the lanes start" % (step, "holds" if transcript_holds else "DOES NOT HOLD"),
                  flush=True)

            # 5. The lanes, as measure takes them, each workload's answers checked again after its lanes.
            for form in FORMS:
                for stem in base.WORKLOAD_STEMS:
                    files = (stem + ".js", stem + ".control.js")
                    times = {(arm, name): [] for arm in ARMS for name in files}
                    for repetition in range(base.WARMUP + base.REPETITIONS):
                        kept = repetition >= base.WARMUP
                        order = ARMS[repetition % len(ARMS):] + ARMS[:repetition % len(ARMS)]
                        position = 0
                        for arm in order:
                            for name in files:
                                position += 1
                                code, answer, runtime, elapsed, _ = child(arm, form, os.path.join(folder, name))
                                expected, _ = expected_for(form, arm, name)
                                ok = code == 0 and expected is not None and answer == expected
                                failed_runs += 0 if ok else 1
                                runs.write("%s\t%s\t%d\t%s\t%d\t%s\t%s\t%d\t%s\t%d\t%.3f\n" % (
                                    form, stem, repetition, "yes" if kept else "warm-up", position, arm, name, code,
                                    "yes" if ok else "NO", numbered(runtime), elapsed))
                                runs.flush()
                                if kept:
                                    times[(arm, name)].append(elapsed)
                    failed_conditions += check(form, stem, "after")
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

        # 6. The results.
        summary[0] = summary[0] % datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")
        pairs = len(FORMS) * len(base.WORKLOAD_STEMS)
        summary += [
            "# answer checks (conditions.tsv) that differ from the answer they are held to: %d; timed children that"
            " do: %d" % (failed_conditions, failed_runs),
            "# transcript check (transcript-check.txt): %s" % ("holds in both forms" if transcript_holds else "DOES NOT HOLD"),
            "# workloads and forms with D <= N: %d of %d" % (within, pairs),
            "# effective-configuration lines seen: %d (runs.tsv and conditions.tsv name each by its number)"
            % len(configurations),
        ]
        summary.extend("#   %d: %s" % (number, line) for number, line in enumerate(configurations, 1))
        summary.append("# load average %s at the step's start, %s at its end" % (started_load, load_average()))
        with open(os.path.join(out, "summary.txt"), "w", encoding="utf-8", newline="\n") as stream:
            stream.write(hide("\n".join(summary) + "\n"))
        lines = ["%s  %s" % (self.sha256(os.path.join(out, entry)) if entry.endswith(".gz")
                             else self.sha256_lf(os.path.join(out, entry)), entry)
                 for entry in sorted(os.listdir(out)) if entry not in ("hashes.txt", "README.md")]
        with open(os.path.join(out, "hashes.txt"), "w", encoding="utf-8", newline="\n") as stream:
            stream.write("\n".join(lines) + "\n")
        print("%s: %d answer checks differ, %d timed children differ, transcript check %s, %d configuration lines,"
              " D <= N in %d of %d" % (step, failed_conditions, failed_runs, "holds" if transcript_holds else "FAILS",
                                       len(configurations), within, pairs), flush=True)
        return 0 if (failed_conditions == 0 and failed_runs == 0 and transcript_holds
                     and len(configurations) == 1) else 1

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
            # The solution first, as the lane builds it: rules B3, B5, N10, U9 and W2 read every
            # shipping assembly's built image, which the test project's own build does not produce.
            code, output = self.run(["dotnet", "build", "Broiler.VM.slnx",
                                     "-c", "Release", "--nologo"])
            if code == 0:
                code, output = self.run(["dotnet", "test", os.path.join("src", "tests", "Broiler.VM.Architecture.Tests"),
                                         "-c", "Release", "--no-build", "--nologo", "--logger", "console;verbosity=normal"])
            else:
                output = "the solution did not build\n" + output
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
    parser.add_argument("--numref", help="measure-revised: Broiler.JS's scripts/conformance/numref.py at 966140ad")
    parser.add_argument("--default-branch",
                        help="measure-revised: the default branch's ref (origin/main, say); arm A is the commit on its"
                             " first-parent history that adds the revised rule")
    parser.add_argument("--check-only", action="store_true",
                        help="measure-revised: the refusals and the builds only; no transcript, answer or lane")
    parser.add_argument("--only", action="append", required=True,
                        choices=("identity", "closures", "test262", "compare", "rerun", "twins", "differential", "measure",
                                 "measure-revised", "rules", "simplification"))
    arguments = parser.parse_args()

    collector = Collector(arguments)
    collector.arguments = arguments
    verdict = 0
    for step in arguments.only:
        verdict |= getattr(collector, step.replace("-", "_"))()
    shutil.rmtree(collector.scratch, ignore_errors=True)
    return verdict


if __name__ == "__main__":
    sys.exit(main())
