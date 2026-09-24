#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# JUDGE THE VALUE FORM ONCE, UNDER A RULE COMMITTED BEFORE THE FORM'S FIRST SPEED-BEARING LINE EXISTED.
#
# --------------------------------------------------------------------------------------------------
# WHAT THIS IS, AND WHAT MAY BE DONE WITH WHAT IT PRODUCES
# --------------------------------------------------------------------------------------------------
#
# The collector of evidence bundle `jsv-4-001`, and of nothing else. JSD-0035 section 10 says the value
# form is judged once, by a predeclared rule, against a retained measurement, and the rule is
# `docs/evidence/jsv-4-001/decision-rule.md`, committed before any code of stage JSV-2. Two builds of
# the command-line host are handed to it: a CANDIDATE, built from the commit that completes stage
# JSV-4, and a CONTROL, built from the commit that added the rule. It times four arms of the candidate
# and one of the control over the wide shapes, interleaved, with an A/A lane, and computes the rule's
# verdict from the rule's own fenced block: the constants are read out of it, and a block whose
# formulas no longer say what this harness computes is REFUSED rather than reinterpreted.
#
# It is `eng/measure-baseline-granularity.py` (bundle `jsb-11-002`) re-aimed, and it keeps that
# harness's discipline whole: `roadmap.gates.md` section 17's eight rules, restated there and not
# repeated here, and `docs/baselines.md` rule L1. A figure from this harness that turns up in a
# roadmap, a ledger, a support table, a decision record or a code remark is a defect in the commit
# that put it there. NO WALL-CLOCK FIGURE REACHES THIS SCRIPT'S CONSOLE OUTPUT: the console says what
# the harness did, and the files beside `--out` say what it measured.
#
# --------------------------------------------------------------------------------------------------
# THE ARMS, AS THE RULE NAMES THEM
# --------------------------------------------------------------------------------------------------
#
#   B-control  the control build, bytecode form        exists only for clause 4 (the interpreter)
#   B          the candidate build, bytecode form      the control of the verdict
#   V          the candidate build, value form         the candidate
#   V-again    the candidate build, value form again   the A/A lane
#   L          the candidate build, baseline form      timed and reported, judged by no clause
#
# --------------------------------------------------------------------------------------------------
# THE LANES OF ONE REPETITION, IN THIS FIXED ORDER, PER SHAPE
# --------------------------------------------------------------------------------------------------
#
#   Bc-bytecode-check, Bc-bytecode-run,       (B-control)
#   B-bytecode-check,  B-bytecode-run,        (B)
#   V-value-check,     V-value-run,           (V)
#   V-again-check,     V-again-run,           (V-again, the A/A pair's second half)
#   L-native-check,    L-native-run           (L)
#
# Ten processes per shape per repetition. B sits between B-control and V because it is the arm each of
# them is differenced against; V-again follows V because the A/A pair is the candidate measured twice
# and nothing else, and what separates them should be the machine and not another arm's work. L is
# last because nothing is differenced against it.
#
# THE QUANTITY is the rule's: for a shape and an arm, one repetition measures `run_ms - check_ms`,
# where the check lane is the same command with `--check`, which compiles, verifies and stops. Q is
# the median over the retained repetitions. Every raw wall clock is retained in `runs.jsonl`.
#
# --------------------------------------------------------------------------------------------------
# THE CONDITION, CHECKED BEFORE ANY TIMING, ON EVERY LANE, AND AGAIN AFTER ALL OF IT
# --------------------------------------------------------------------------------------------------
#
#   1. Before any timing, per shape: each of the four arm-and-form combinations (control bytecode,
#      candidate bytecode, candidate value, candidate baseline) is run once, untimed, and each of their
#      check lanes as well. All must exit 0 and the run lanes must print IDENTICAL standard output,
#      whose SHA-256 becomes the shape's expected answer; each check lane's output becomes its own
#      expectation. Then FUEL PARITY on the shape's `.small.js` twin: the smallest completing `--fuel`
#      is bisected under B, and B, V and B-control must each complete at that figure and refuse with
#      `AllowanceExhausted on Fuel` at one less. L is tried at the same two figures and its result is
#      recorded without stopping anything (departure 3).
#   2. Every lane, as it runs: the process exits 0, its output hashes to its expectation, and its
#      `--runtime` line equals the first one recorded (clause 5). A mismatch stops the harness at once.
#   3. After all timing, per shape: step 1's answers again, and they must equal step 1's.
#
# --------------------------------------------------------------------------------------------------
# WHAT A FAILED CONDITION MEANS, WHICH THE RULE'S OWNER RULED ON
# --------------------------------------------------------------------------------------------------
#
# The rule's readings say "a failed answer condition or fuel-parity condition in the candidate is a
# refusal, not a re-run". So a condition that fails IN THE VALUE FORM - a V or V-again lane that exits
# nonzero or prints another answer, or the value form missing fuel parity - ends the run with a
# `summary.txt` whose verdict is REFUSE under clause 1 and which carries no figure. Any other failed
# condition (a bytecode or baseline lane, the control, a differing effective configuration) ends the
# run with no summary at all, as `measure-baseline-granularity.py` does: it is not the candidate's
# failure, and the rule's reading of an interrupted collection is a re-run from the start, the stopped
# run retained beside the new one and named as stopped.
#
# --------------------------------------------------------------------------------------------------
# THE REFUSALS, ALL OF WHICH HAPPEN BEFORE ANY CHILD IS TIMED
# --------------------------------------------------------------------------------------------------
#
#   * the manifest or the rule is missing, does not parse, is not committed, or has uncommitted
#     changes (`--rehearsal` is the one exception, and it may not write into this repository);
#   * the rule is not byte for byte the file the control commit added, or its fenced block does not
#     state the quantity, the noise floor, the difference, the drift, the quantifier over the clauses,
#     `REFUSE otherwise`, clause 1's conditions, clause 5's one configuration, or the readings this
#     harness applies; or a threshold is still in angle brackets;
#   * the manifest's control commit is not the commit that added the rule, or its candidate commit is
#     not in this repository's history;
#   * a shape differs from the one the control commit holds, or from the manifest's, or the folder and
#     the control commit do not hold the same set of shapes (the population is "as committed with this
#     file");
#   * any file of either arm differs from the manifest, or an arm holds a file the manifest does not
#     name, or the manifest names one the arm does not hold;
#   * the host that starts every child (departure 1) is not the one the manifest hashes;
#   * this script's own blob id differs from the manifest's;
#   * R is not 7, W is not 3, or R, W, the allowances, the backend or the lane order differ from the
#     manifest;
#   * the set of DOTNET_* and COMPlus_* keys in this environment is not the set the manifest says was
#     scrubbed;
#   * `--out` names a directory that already holds a `runs.jsonl`: a stopped run is retained, so it is
#     moved aside and named by whoever re-runs, never overwritten by the re-run.
#
# A refusal exits 2 and writes no measurement.
#
# --------------------------------------------------------------------------------------------------
# DEPARTURES FROM `measure-baseline-granularity.py`, STATED HERE RATHER THAN LEFT TO BE FOUND
# --------------------------------------------------------------------------------------------------
#
#   1. EVERY CHILD IS STARTED BY THE `dotnet` HOST NAMED WITH `--dotnet`, NOT BY THE APPLICATION HOST.
#      On this machine the runtime is not installed where the application host looks for it, so the
#      application host finds it only through `DOTNET_ROOT` - which is a `DOTNET_*` key and is
#      scrubbed out of every child. The `dotnet` host finds the runtime beside itself. It is named by
#      absolute path, and the manifest pins it by SHA-256.
#   2. `--quiet` IS NOT PASSED, for the reason `measure-baseline-granularity.py` departure 1 gives: the
#      host prints the completion value only without it, and a lane whose output is empty cannot be
#      told from a lane that answered wrongly.
#   3. L's FUEL PARITY IS RECORDED AND DOES NOT STOP THE RUN. Clause 1 names the bytecode and value
#      forms; stopping the value form's verdict over the baseline form's charging would be a rule this
#      harness made. B-control IS held to parity, because it is a bytecode form and clause 1 names the
#      bytecode form without naming a build.
#   4. MACHINE STATE IS `/proc/loadavg` and the count of `/proc`'s process directories where those
#      exist, `os.getloadavg()` where only that does, and nothing otherwise. It is context for a
#      reader, not a gate, and nothing about it can stop a run.
#   5. THE LANES' ROWS ARE WRITTEN AS THEY RUN, not at the end, so a run that is interrupted leaves
#      every row it timed in `runs.jsonl` for the stopped run the rule says is retained.
#
#   python3 eng/measure-value-form.py --control <dir> --candidate <dir> --dotnet <path>
#                                     --manifest <bundle>/manifest.json
#                                     --rule <bundle>/decision-rule.md
#                                     --out <bundle>/runs.jsonl
#                                     [--shapes <dir>] [--repetitions 7] [--warmup 3]
#                                     [--fuel N] [--wall MS] [--backend NAME] [--rehearsal]

import argparse
import hashlib
import json
import math
import os
import pathlib
import platform
import re
import statistics
import subprocess
import sys
import time
from datetime import datetime, timezone

ROOT = pathlib.Path(__file__).resolve().parent.parent
DEFAULT_SHAPES = ROOT / "src/tests/forms/wide"
SHAPES_IN_TREE = "src/tests/forms/wide"
FRESH_SHAPE = "fresh-small.js"
TWIN_SUFFIX = ".small.js"
HOST_DLL = "Broiler.VM.Composition.JavaScript.Cli.dll"

# THE COUNTS ARE NOT THE CALLER'S TO CHOOSE, and the rule says so as well: seven retained repetitions
# and three warm-up ones, bundle `jsb-11-002`'s inheritance from the core's baseline register and
# `eng/compare-forms.py`. The rule, the manifest and this file must all agree.
REQUIRED_REPETITIONS = 7
REQUIRED_WARMUP = 3

# (lane name, arm, build, form, kind). The order is part of the measurement; the manifest pins it.
LANES = [
    ("Bc-bytecode-check", "B-control", "control", "bytecode", "check"),
    ("Bc-bytecode-run", "B-control", "control", "bytecode", "run"),
    ("B-bytecode-check", "B", "candidate", "bytecode", "check"),
    ("B-bytecode-run", "B", "candidate", "bytecode", "run"),
    ("V-value-check", "V", "candidate", "value", "check"),
    ("V-value-run", "V", "candidate", "value", "run"),
    ("V-again-check", "V-again", "candidate", "value", "check"),
    ("V-again-run", "V-again", "candidate", "value", "run"),
    ("L-native-check", "L", "candidate", "native", "check"),
    ("L-native-run", "L", "candidate", "native", "run"),
]
LANE_NAMES = [lane[0] for lane in LANES]
ARMS = ("B-control", "B", "V", "V-again", "L")

# The four build-and-form combinations the condition compares. V-again is V run again, so it adds no
# combination: an answer it gave that V's own lane did not would be a defect in the machine.
COMBINATIONS = [
    ("control", "bytecode"), ("candidate", "bytecode"),
    ("candidate", "value"), ("candidate", "native"),
]
PARITY_HELD = [("candidate", "bytecode"), ("candidate", "value"), ("control", "bytecode")]
PARITY_RECORDED = [("candidate", "native")]

EXHAUSTED_ON_FUEL = "AllowanceExhausted on Fuel"
RUNTIME_PREFIX = "runtime-identifier="

REHEARSAL_BANNER = (
    "# A REHEARSAL. The manifest and rule behind this run were not committed, so no figure in this "
    "file may be retained by anything.")


def refuse(*lines):
    """Stop before anything is timed, saying which rule was not met."""
    for line in lines:
        print("# REFUSED: " + line, file=sys.stderr)

    raise SystemExit(2)


def sha256_of(path):
    return hashlib.sha256(pathlib.Path(path).read_bytes()).hexdigest()


def sha256_text(text):
    return hashlib.sha256(text.encode("utf-8")).hexdigest()


def git(arguments, cwd=ROOT):
    done = subprocess.run(
        ["git", *arguments], cwd=str(cwd), capture_output=True,
        encoding="utf-8", errors="replace")

    return done.returncode, (done.stdout or "").strip(), (done.stderr or "").strip()


def blob_id(path):
    """The git blob id of a file, which needs no repository and so works for either arm."""
    path = pathlib.Path(path)
    code, out, _ = git(["hash-object", "-t", "blob", "--", path.name], path.parent)

    return out if code == 0 else None


def committed_and_clean(path):
    """(is committed, is clean) for a file of THIS repository; (False, False) for one outside it."""
    path = pathlib.Path(path).resolve()
    code, out, _ = git(["log", "-1", "--format=%H", "--", str(path)])

    if code != 0 or not out:
        return False, False

    code, out, _ = git(["status", "--porcelain", "--", str(path)])

    return True, code == 0 and not out


def files_of(directory):
    """Every file of an arm, by name and SHA-256: what the host loads, and what it is configured by."""
    directory = pathlib.Path(directory)

    return {
        path.name: sha256_of(path)
        for path in sorted(directory.iterdir())
        if path.is_file()
    }


def dll_of(directory):
    binary = pathlib.Path(directory) / HOST_DLL

    if not binary.exists():
        refuse(f"no host assembly at {binary}")

    return binary


def scrubbed_environment():
    """The child environment, with every runtime knob removed, and the keys that were removed."""
    child = dict(os.environ)
    removed = sorted(
        key for key in child
        if key.startswith("DOTNET_") or key.startswith("COMPlus_"))

    for key in removed:
        del child[key]

    return child, removed


def number(text, what, path):
    try:
        return float(text)
    except ValueError:
        refuse(f"{path} states {what} as `{text}`, which is not a number")


def adding_commit(path):
    """The commit that added a file of this repository, which for the rule is B-control's commit."""
    relative = pathlib.Path(path).resolve().relative_to(ROOT).as_posix()
    code, out, _ = git(["log", "--diff-filter=A", "--format=%H", "--", relative])

    if code != 0 or not out:
        refuse(f"no commit of this repository added {relative}")

    return out.splitlines()[-1], relative


def read_rule(path, population):
    """The decision rule's constants, read out of its own fenced block and out of nothing else."""
    try:
        text = pathlib.Path(path).read_text(encoding="utf-8")
    except (OSError, UnicodeDecodeError) as failure:
        refuse(f"the rule at {path} cannot be read as text", f"{type(failure).__name__}: {failure}")

    block = re.search(r"```rule\n(.*?)```", text, re.S)

    if not block:
        refuse(f"{path} carries no fenced `rule` block, so it states no rule this harness can read")

    body = block.group(1)

    if re.search(r"<(?!=)", body):
        unfixed = re.findall(r"<(?!=)[^<>\n]*>?", body)
        refuse(
            "the rule still carries a threshold nobody has fixed: " + ", ".join(unfixed),
            "the owner fixes every angle-bracketed figure before the rule is committed")

    # EVERY PART OF THE RULE THIS HARNESS APPLIES IS PINNED, NOT ONLY THE PARTS IT READS A CONSTANT
    # OUT OF: the four quantities, the quantifier over the clauses, the clauses enforced by stopping,
    # and the two readings the computation of clause 2 depends on.
    for statement, what in (
            ("median over retained repetitions of (run_ms - check_ms)", "the quantity"),
            ("N(shape) = |Q(shape, V) - Q(shape, V-again)|", "the noise floor"),
            ("D(shape) = Q(shape, B) - Q(shape, V)", "the difference"),
            ("E(shape) = Q(shape, B) - Q(shape, B-control)", "the bytecode drift"),
            ("ADOPT the value form if and only if all of", "that every clause must hold"),
            ("REFUSE otherwise", "what it does when one does not"),
            ("condition-before", "clause 1's before-condition"),
            ("condition-after", "clause 1's after-condition"),
            ("refuses with AllowanceExhausted on Fuel at one less in both", "clause 1's fuel parity"),
            ("identical across arms, lanes and repetitions", "clause 5's one effective configuration"),
            ("a tie does not count toward clause 2", "that a tie is not a win"),
            ("a shape whose Q(B) is not positive has no ratio", "how a non-positive Q(B) is read"),
            ("excluded from Q", "that the warm-up repetitions are not in Q")):
        if statement not in body:
            refuse(
                f"the rule does not state {what} as `{statement}`",
                "this harness computes that formula, so a rule stating another one is not this "
                "harness's rule and the two may not be mixed")

    repetitions = re.search(r"R=(\d+)", body)
    warmup = re.search(r"W=(\d+)", body)
    majority = re.search(
        r"at least\s+(.+?)\s+shapes:\s*D > N and Q\(V\)/Q\(B\) <=\s*([0-9.]+)", body)
    ceiling = re.search(r"every shape:\s*Q\(V\) <=\s*([0-9.]+)\s*\*\s*Q\(B\) \+ N", body)
    drift = re.search(r"\|E\| <=\s*([0-9.]+)\s*\*\s*Q\(B-control\)", body)

    if not (repetitions and warmup and majority and ceiling and drift):
        refuse(
            f"{path}'s rule block does not state R, W, the majority clause, the ceiling clause and "
            "the drift clause in the form this harness reads")

    count = majority.group(1)

    if re.fullmatch(r"\d+", count):
        wanted = int(count)
    else:
        share = re.fullmatch(r"ceil\((\d+)/(\d+)\s*\*\s*shapes\)", count)

        if not share:
            refuse(f"`{count}` is neither a number of shapes nor `ceil(p/q * shapes)`")

        wanted = math.ceil(int(share.group(1)) / int(share.group(2)) * len(population))

    return {
        "path": str(path),
        "sha256": sha256_text(text),
        "repetitions": int(repetitions.group(1)),
        "warmup": int(warmup.group(1)),
        "majority-shapes": wanted,
        "majority-source": count,
        "ratio": number(majority.group(2), "the ratio of clause 2", path),
        "ceiling": number(ceiling.group(1), "the multiplier of clause 3", path),
        "drift": number(drift.group(1), "the fraction of clause 4", path),
        "population-line": next(
            (line for line in body.splitlines() if line.startswith("population:")), ""),
    }


def check_manifest(manifest, path, arguments, rule, shapes, folder, removed):
    """The run is refused unless it is the run the manifest describes, over the rule's population."""
    for field in ("bundle", "arms", "host", "repetitions", "warmup", "lanes", "shapes", "harness",
                  "rule", "allowances", "environment"):
        if field not in manifest:
            refuse(f"{path} states no `{field}`, so it does not pin the run it is supposed to pin")

    if manifest["repetitions"] != REQUIRED_REPETITIONS or manifest["warmup"] != REQUIRED_WARMUP:
        refuse(
            f"the manifest asks for R={manifest['repetitions']} and W={manifest['warmup']}; the "
            f"counts are R={REQUIRED_REPETITIONS} and W={REQUIRED_WARMUP}")

    if (rule["repetitions"], rule["warmup"]) != (manifest["repetitions"], manifest["warmup"]):
        refuse("the rule file and the manifest state different repetition counts")

    if (arguments.repetitions, arguments.warmup) != (manifest["repetitions"], manifest["warmup"]):
        refuse("this invocation asks for repetition counts the manifest and the rule do not state")

    if manifest["lanes"] != LANE_NAMES:
        refuse(
            "the manifest's lane order is not this harness's",
            "manifest: " + ", ".join(manifest["lanes"]),
            "harness:  " + ", ".join(LANE_NAMES))

    allowances = manifest["allowances"]

    if (allowances.get("fuel"), allowances.get("wall"), allowances.get("backend")) != (
            arguments.fuel, arguments.wall, arguments.backend):
        refuse("this invocation's allowances or backend are not the manifest's")

    mine = blob_id(pathlib.Path(__file__))

    if manifest["harness"] != mine:
        refuse(
            "the manifest pins a different harness than the one running",
            f"manifest: {manifest['harness']}", f"running:  {mine}")

    # THE RULE IS THE FILE THE CONTROL COMMIT ADDED, UNCHANGED. A rule edited after the candidate was
    # seen would be exactly the substitution the predeclared decision exists to prevent.
    control_commit, relative = adding_commit(arguments.rule)
    code, at_control, _ = git(["rev-parse", f"{control_commit}:{relative}"])
    here = blob_id(arguments.rule)

    if code != 0 or at_control != here or manifest["rule"] != here:
        refuse(
            "the rule is not the file the control commit added",
            f"at {control_commit[:12]}: {at_control}", f"here: {here}",
            f"manifest: {manifest['rule']}")

    arms = manifest["arms"]

    for arm in ("control", "candidate"):
        if arm not in arms or "commit" not in arms[arm] or "files" not in arms[arm]:
            refuse(f"the manifest names no commit or no files for the {arm} arm")

    if arms["control"]["commit"] != control_commit:
        refuse(
            "the manifest's control commit is not the commit that added the rule",
            f"manifest: {arms['control']['commit']}", f"rule added at: {control_commit}")

    candidate = arms["candidate"]["commit"]

    if git(["merge-base", "--is-ancestor", candidate, "HEAD"])[0] != 0:
        refuse(f"the candidate commit {candidate} is not in this checkout's history")

    # THE POPULATION IS "AS COMMITTED WITH THIS FILE", so the shapes on disk, the manifest's and the
    # control commit's must be one set of blobs.
    on_disk = {path.name: blob_id(path) for path in sorted(folder.glob("*.js"))}
    code, listing, _ = git(["ls-tree", "--format=%(objectname) %(path)", control_commit,
                            SHAPES_IN_TREE + "/"])
    at_rule = {}

    for line in listing.splitlines():
        objectname, name = line.split(" ", 1)

        if name.endswith(".js"):
            at_rule[pathlib.PurePosixPath(name).name] = objectname

    for label, other in (("the manifest", manifest["shapes"]), ("the control commit", at_rule)):
        if other != on_disk:
            refuse(
                f"the shapes on disk are not the shapes {label} holds",
                f"absent here: {sorted(set(other) - set(on_disk)) or 'none'}",
                f"only here: {sorted(set(on_disk) - set(other)) or 'none'}",
                "changed: " + (", ".join(sorted(
                    name for name in set(on_disk) & set(other)
                    if on_disk[name] != other[name])) or "none"))

    declared = manifest["environment"].get("scrubbed", None)

    if declared is None or sorted(declared) != removed:
        refuse(
            "the runtime knobs in this environment are not the ones the manifest was written under",
            f"manifest: {sorted(declared) if declared is not None else 'not stated'}",
            f"here:     {removed or 'none'}")

    host = pathlib.Path(arguments.dotnet).resolve()

    if manifest["host"].get("path") != str(host) or manifest["host"].get("sha256") != sha256_of(host):
        refuse(
            "the host that starts every child is not the one the manifest pins",
            f"manifest: {manifest['host']}", f"here: {host}")

    for arm, directory in (("control", arguments.control), ("candidate", arguments.candidate)):
        held = files_of(directory)
        named = arms[arm]["files"]

        if held != named:
            refuse(
                f"the {arm} arm is not the arm the manifest hashes",
                f"named and absent: {sorted(set(named) - set(held)) or 'none'}",
                f"present and unnamed: {sorted(set(held) - set(named)) or 'none'}",
                "changed: " + (", ".join(sorted(
                    name for name in set(named) & set(held) if held[name] != named[name])) or "none"))


def invoke(host, binary, form, shape, arguments, check, environment, fuel=None):
    """One child process. Every child carries `--runtime` (clause 5)."""
    command = [str(host), str(binary), "--runtime"]

    if form == "native":
        command += ["--native", arguments.backend]
    elif form == "value":
        command += ["--value", arguments.backend]

    command += ["--fuel", str(arguments.fuel if fuel is None else fuel), "--wall", str(arguments.wall)]

    if check:
        command.append("--check")

    command.append(str(shape))
    started = time.perf_counter()
    done = subprocess.run(
        command, cwd=str(ROOT), capture_output=True, encoding="utf-8", errors="replace",
        env=environment)
    elapsed = (time.perf_counter() - started) * 1000.0

    return {
        "exit": done.returncode,
        "stdout": done.stdout or "",
        "stderr": done.stderr or "",
        "wall_ms": elapsed,
        "runtime": next(
            (line.strip() for line in (done.stderr or "").splitlines()
             if line.startswith(RUNTIME_PREFIX)), ""),
    }


class Refused(Exception):
    """The value form failed a condition: the rule's owner reads that as a refusal, not a re-run."""


class Harness:
    def __init__(self, arguments, manifest, rule, population, environment, removed):
        self.arguments = arguments
        self.manifest = manifest
        self.rule = rule
        self.population = population
        self.environment = environment
        self.removed = removed
        self.out = pathlib.Path(arguments.out)
        self.directory = self.out.parent
        self.directory.mkdir(parents=True, exist_ok=True)
        self.host = pathlib.Path(arguments.dotnet).resolve()
        self.binaries = {
            "control": dll_of(arguments.control),
            "candidate": dll_of(arguments.candidate),
        }
        self.expected = {}
        self.runtime_line = None
        self.log_lines = []
        self.runtime_reports = []
        self.rows = 0

    # ---- the files it writes -----------------------------------------------------------------
    def head(self):
        return [REHEARSAL_BANNER] if self.arguments.rehearsal else []

    def write(self, name, lines):
        (self.directory / name).write_text(
            "\n".join(self.head() + lines) + "\n", encoding="utf-8", newline="\n")

    def log(self, line):
        self.log_lines.append(line)

    def say(self, line):
        """The console, which carries no wall-clock figure."""
        print(line, flush=True)

    def append_row(self, row):
        with self.out.open("a", encoding="utf-8", newline="\n") as handle:
            handle.write(json.dumps(row, sort_keys=True) + "\n")

        self.rows += 1

    def stop(self, *lines):
        """A condition failed that is not the value form's: no summary is written."""
        for line in lines:
            self.log("STOPPED: " + line)
            print("# STOPPED: " + line, file=sys.stderr)

        self.write("harness.log", self.log_lines)
        self.write("runtime-reports.log", self.runtime_reports)
        raise SystemExit(1)

    def refused(self, *lines):
        """A condition failed in the value form: the verdict is REFUSE under clause 1, with no figure."""
        for line in lines:
            self.log("REFUSED UNDER CLAUSE 1: " + line)
            print("# REFUSED UNDER CLAUSE 1: " + line, file=sys.stderr)

        self.write("summary.txt", [
            "# What bundle jsv-4-001's measurement found, and nothing else.",
            f"# The verdict is computed ONLY from the fenced block of {self.rule['path']} "
            f"(sha256 {self.rule['sha256']}).",
            "",
            "NOT MET 1 the value form failed a condition:",
            *("        " + line for line in lines),
            "",
            "The rule's readings: a failed answer condition or fuel-parity condition in the candidate "
            "is a refusal, not a re-run. No figure is written: the run did not reach the clauses "
            "that read one.",
            "",
            "VERDICT: REFUSE the value form, by the rule above and by nothing else.",
        ])
        self.write("harness.log", self.log_lines)
        self.write("runtime-reports.log", self.runtime_reports)
        raise SystemExit(1)

    def fail(self, form, *lines):
        if form == "value":
            self.refused(*lines)

        self.stop(*lines)

    # ---- the condition -----------------------------------------------------------------------
    def check_runtime(self, answer, where):
        if not answer["runtime"]:
            self.stop(f"{where} reported no effective configuration, which clause 5 requires")

        if self.runtime_line is None:
            self.runtime_line = answer["runtime"]
            self.log("effective configuration, from the first child: " + self.runtime_line)

        self.runtime_reports.append(f"{where}: {answer['runtime']}")

        if answer["runtime"] != self.runtime_line:
            self.stop(
                f"{where} ran under a different effective configuration (clause 5)",
                "first: " + self.runtime_line, "here:  " + answer["runtime"])

    def observe(self, build, form, shape, check, label, lines):
        answer = invoke(
            self.host, self.binaries[build], form, shape, self.arguments, check, self.environment)
        digest = sha256_text(answer["stdout"])
        lines.append(
            f"{label:<18} {build:<9} {form:<8} {'check' if check else 'run':<5} "
            f"exit {answer['exit']} stdout {digest}")

        if answer["exit"] != 0:
            self.fail(
                form,
                f"{label} for {shape.name}: {build} {form} {'check' if check else 'run'} "
                f"exited {answer['exit']}",
                (answer["stderr"].strip().splitlines() or ["(no complaint)"])[-1])

        self.check_runtime(answer, f"{label} {shape.name} {build} {form}")

        return digest

    def condition(self, label):
        lines = [f"# {label}: every build and form over every shape, untimed, with --runtime",
                 f"# {datetime.now(timezone.utc).strftime('%Y-%m-%dT%H:%M:%SZ')}"]
        answers = {}

        for shape in self.population:
            self.say(f"# {label}: {shape.name}")
            seen = {
                (build, form): self.observe(build, form, shape, False, label, lines)
                for build, form in COMBINATIONS
            }
            reference = seen[("candidate", "bytecode")]
            differing = [key for key, digest in seen.items() if digest != reference]

            if differing:
                self.fail(
                    "value" if ("candidate", "value") in differing else "other",
                    f"{label}: the combinations do not agree on {shape.name}",
                    "; ".join(f"{build}-{form} {digest[:16]}" for (build, form), digest in seen.items()),
                    "NOTHING IS TIMED AROUND A SHAPE WHOSE FORMS DISAGREE")

            checks = {
                f"{build}-{form}": self.observe(build, form, shape, True, label, lines)
                for build, form in COMBINATIONS
            }
            answers[shape.name] = {"answer": reference, "checks": checks}
            lines.append(f"{shape.name}: every combination answers {reference}")

        return answers, lines

    def smallest_completing_fuel(self, twin):
        """The least allowance the twin finishes under, by bisection under B: exact, machine-independent."""
        def completes(fuel):
            return invoke(
                self.host, self.binaries["candidate"], "bytecode", twin, self.arguments, False,
                self.environment, fuel=fuel)["exit"] == 0

        high = self.arguments.fuel

        if not completes(high):
            self.stop(f"{twin.name} does not complete under the run allowance in B")

        low = 1

        while low < high:
            middle = (low + high) // 2

            if completes(middle):
                high = middle
            else:
                low = middle + 1

        return low

    def fuel_parity(self, lines):
        """B, V and B-control complete at one figure and refuse at one less; L is recorded."""
        for shape in self.population:
            twin = shape.with_name(shape.stem + TWIN_SUFFIX)

            if not twin.exists():
                self.stop(f"{shape.name} has no `{twin.name}`, so fuel parity has no twin to bisect")

            figure = self.smallest_completing_fuel(twin)
            self.say(f"# fuel parity: {twin.name} completes at {figure} in B")
            lines.append(f"{twin.name}: smallest completing allowance {figure}, under B")

            for (build, form), held in (
                    [(combination, True) for combination in PARITY_HELD]
                    + [(combination, False) for combination in PARITY_RECORDED]):
                completed = invoke(
                    self.host, self.binaries[build], form, twin, self.arguments, False,
                    self.environment, fuel=figure)
                short = invoke(
                    self.host, self.binaries[build], form, twin, self.arguments, False,
                    self.environment, fuel=figure - 1)
                completes = completed["exit"] == 0
                refuses = short["exit"] != 0 and EXHAUSTED_ON_FUEL in short["stderr"]
                lines.append(
                    f"    {build:<9} {form:<8} {'completes' if completes else 'DOES NOT COMPLETE'} "
                    f"at {figure}, "
                    + (f"{EXHAUSTED_ON_FUEL} at {figure - 1}" if refuses
                       else f"DOES NOT REFUSE at {figure - 1}")
                    + ("" if held else "   (recorded, judged by no clause)"))

                if held and not (completes and refuses):
                    self.write("condition-before.log", lines)
                    self.fail(
                        form,
                        f"fuel parity: {build} {form} on {twin.name} "
                        + ("does not complete" if not completes else "completes")
                        + f" at {figure} and "
                        + ("refuses" if refuses else "does not refuse with "
                           f"`{EXHAUSTED_ON_FUEL}`") + f" at {figure - 1}")

    # ---- machine state -----------------------------------------------------------------------
    def machine_state(self, phase, repetition, lines):
        stamp = datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")

        try:
            load = pathlib.Path("/proc/loadavg").read_text().strip()
            processes = sum(1 for entry in pathlib.Path("/proc").iterdir() if entry.name.isdigit())
            state = f"loadavg {load} | processes {processes}"
        except OSError:
            try:
                state = "getloadavg " + " ".join(f"{value:.2f}" for value in os.getloadavg())
            except (OSError, AttributeError):
                state = "no probe answers on this host"

        lines.append(f"{stamp} repetition {repetition} {phase}: {state}")

    # ---- the timed lanes ---------------------------------------------------------------------
    def measure(self):
        machine = ["# machine state before and after every repetition"]
        total = self.arguments.warmup + self.arguments.repetitions

        for repetition in range(total):
            warm = repetition < self.arguments.warmup
            self.say(
                f"# repetition {repetition + 1} of {total} "
                f"({'warm-up, retained and excluded from Q' if warm else 'retained'})")
            self.machine_state("before", repetition + 1, machine)

            for shape in self.population:
                expectation = self.expected[shape.name]

                for order, (name, arm, build, form, kind) in enumerate(LANES):
                    answer = invoke(
                        self.host, self.binaries[build], form, shape, self.arguments,
                        kind == "check", self.environment)
                    digest = sha256_text(answer["stdout"])
                    wanted = (
                        expectation["checks"][f"{build}-{form}"] if kind == "check"
                        else expectation["answer"])
                    condition = "ok"

                    if answer["exit"] != 0:
                        condition = f"exit {answer['exit']}"
                    elif digest != wanted:
                        condition = "output is not the expected one"

                    row = {
                        "rep": repetition + 1,
                        "warmup": warm,
                        "shape": shape.name,
                        "arm": arm,
                        "build": build,
                        "form": form,
                        "lane": kind,
                        "name": name,
                        "order": order,
                        "exit": answer["exit"],
                        "wall_ms": answer["wall_ms"],
                        "stdout_sha256": digest,
                        "condition": condition,
                        "runtime": answer["runtime"],
                    }

                    if self.arguments.rehearsal:
                        row["rehearsal"] = True

                    self.append_row(row)

                    if condition != "ok":
                        self.write("machine-state.log", machine)
                        self.fail(
                            form,
                            f"lane {name} on {shape.name}, repetition {repetition + 1}: {condition}",
                            (answer["stderr"].strip().splitlines() or ["(no complaint)"])[-1])

                    self.check_runtime(answer, f"repetition {repetition + 1} {shape.name} {name}")

            self.machine_state("after", repetition + 1, machine)
            self.write("machine-state.log", machine)

    # ---- what the rule makes of it -----------------------------------------------------------
    def quantities(self):
        rows = [json.loads(line) for line in self.out.read_text(encoding="utf-8").splitlines()]
        quantity = {}

        for shape in self.population:
            quantity[shape.name] = {}

            for arm in ARMS:
                attributed = []

                for repetition in range(
                        self.arguments.warmup + 1,
                        self.arguments.warmup + self.arguments.repetitions + 1):
                    lanes = {
                        row["lane"]: row["wall_ms"] for row in rows
                        if row["rep"] == repetition and row["shape"] == shape.name
                        and row["arm"] == arm
                    }
                    attributed.append(lanes["run"] - lanes["check"])

                quantity[shape.name][arm] = {
                    "repetitions": attributed,
                    "median": statistics.median(attributed),
                }

        return quantity

    def compare_conditions(self, before, after):
        for name, expectation in before.items():
            if after[name]["answer"] != expectation["answer"]:
                self.fail(
                    "other",
                    f"condition-after: {name} answers something else than it did before the timing")

            for combination, digest in expectation["checks"].items():
                if after[name]["checks"][combination] != digest:
                    self.fail(
                        "value" if combination == "candidate-value" else "other",
                        f"condition-after: {name}'s {combination} check lane prints something else "
                        "than it did before the timing")

    def summarise(self, quantity):
        rule = self.rule
        lines = [
            "# What bundle jsv-4-001's measurement found, and nothing else.",
            "#",
            "# Q is the median over the retained repetitions of (run_ms - check_ms), in milliseconds.",
            "# N = |Q(V) - Q(V-again)|, one observation of the noise floor rather than an estimate.",
            "# D = Q(B) - Q(V); positive means the value form is faster. E = Q(B) - Q(B-control).",
            f"# The verdict is computed ONLY from the fenced block of {rule['path']} "
            f"(sha256 {rule['sha256']}).",
            "#",
            f"# repetitions retained {self.arguments.repetitions}, warm-up {self.arguments.warmup}",
            f"# clause 2: {rule['majority-source']} = {rule['majority-shapes']} of "
            f"{len(self.population)} shapes, ratio {rule['ratio']}; clause 3: {rule['ceiling']} * Q(B) + N;"
            f" clause 4: {rule['drift']} * Q(B-control)",
            "",
            f"{'shape':<16} {'Q(B-control)':>12} {'Q(B)':>10} {'Q(V)':>10} {'Q(V-again)':>10} "
            f"{'Q(L)':>10} {'N':>9} {'D':>10} {'V/B':>6} {'E':>9}  resolution",
        ]
        judged = {}

        for shape in self.population:
            q = {arm: quantity[shape.name][arm]["median"] for arm in ARMS}
            floor = abs(q["V"] - q["V-again"])
            difference = q["B"] - q["V"]
            drift = q["B"] - q["B-control"]
            ratio = q["V"] / q["B"] if q["B"] > 0.0 else None
            judged[shape.name] = (q, floor, difference, drift, ratio)
            lines.append(
                f"{shape.name:<16} {q['B-control']:12.1f} {q['B']:10.1f} {q['V']:10.1f} "
                f"{q['V-again']:10.1f} {q['L']:10.1f} {floor:9.1f} {difference:10.1f} "
                + (f"{ratio:6.3f}" if ratio is not None else "     -")
                + f" {drift:9.1f}  "
                + ("faster, above the floor" if difference > floor
                   else "slower, beyond the floor" if difference < -floor
                   else "a tie, within the floor"))

        lines.append("")
        clauses = []
        unreadable = [name for name, (q, *_rest) in judged.items() if q["B"] <= 0.0]
        faster = [
            name for name, (q, floor, difference, drift, ratio) in judged.items()
            if ratio is not None and difference > floor and ratio <= rule["ratio"]]
        clauses.append((
            f"2 value, at least {rule['majority-shapes']} shapes clear the floor and the ratio",
            len(faster) >= rule["majority-shapes"],
            f"{len(faster)} of {len(self.population)}: " + (", ".join(faster) or "none")
            + (f"; no ratio is read on {', '.join(unreadable)}, whose Q(B) is not positive"
               if unreadable else "")))
        slower = [
            name for name, (q, floor, *_rest) in judged.items()
            if not q["V"] <= rule["ceiling"] * q["B"] + floor]
        clauses.append((
            f"3 value, every shape within {rule['ceiling']} * Q(B) + N", not slower,
            "past it on: " + (", ".join(slower) or "no shape")))
        moved = [
            name for name, (q, floor, difference, drift, ratio) in judged.items()
            if not abs(drift) <= rule["drift"] * q["B-control"]]
        clauses.append((
            f"4 bytecode, every shape within {rule['drift']} * Q(B-control)", not moved,
            "moved on: " + (", ".join(moved) or "no shape")))

        lines.append("## the rule, clause by clause")
        lines.append("        clauses 1 and 5 are not computed here. Each is enforced by stopping - a")
        lines.append("        condition that fails, or a child whose effective-configuration line differs")
        lines.append("        from the first recorded, ends the run before this file is written - so a")
        lines.append("        summary that carries figures is what those two clauses assert.")
        lines.append("        every timed child reported " + (self.runtime_line or "nothing"))

        for name, held, detail in clauses:
            lines.append(f"{'MET    ' if held else 'NOT MET'} {name}: {detail}")

        lines.append("")
        lines.append(
            "VERDICT: " + ("ADOPT" if all(held for _, held, _ in clauses) else "REFUSE")
            + " the value form, by the rule above and by nothing else.")
        self.write("summary.txt", lines)

        return lines[-1]

    # ---- the run -----------------------------------------------------------------------------
    def run(self):
        self.log(f"# {datetime.now(timezone.utc).strftime('%Y-%m-%dT%H:%M:%SZ')}")
        self.log("# " + " ".join(sys.argv))
        self.log(f"# {platform.platform()}, {platform.machine()}, {os.cpu_count()} processors")
        self.log(f"manifest: {self.arguments.manifest} sha256 {sha256_of(self.arguments.manifest)}")
        self.log(f"rule: {self.rule['path']} sha256 {self.rule['sha256']}")
        self.log(f"rule population: {self.rule['population-line']}")
        self.log(f"host: {self.host}")
        self.log(f"control: {self.binaries['control']} ({self.manifest['arms']['control']['commit']})")
        self.log(
            f"candidate: {self.binaries['candidate']} "
            f"({self.manifest['arms']['candidate']['commit']})")
        self.log("shapes: " + ", ".join(shape.name for shape in self.population))
        self.log(
            "scrubbed from every child's environment: " + (", ".join(self.removed) or "nothing"))

        if self.arguments.rehearsal:
            self.log(
                "REHEARSAL: the manifest and the rule were not required to be committed, and no "
                "figure of this run may be retained by anything")

        self.say("# condition before any timing, and nothing is timed until it holds")
        before, lines = self.condition("condition-before")
        self.expected = before
        self.fuel_parity(lines)
        self.write("condition-before.log", lines)
        self.say("# the condition holds; timing starts")
        self.measure()
        self.say("# every lane ran and every lane's answer was the expected one")
        after, lines = self.condition("condition-after")
        self.write("condition-after.log", lines)
        self.compare_conditions(before, after)
        self.write("runtime-reports.log", self.runtime_reports)
        verdict = self.summarise(self.quantities())
        self.log(verdict)
        self.write("harness.log", self.log_lines)
        self.say(f"# written to {self.directory}")
        self.say(verdict)
        self.say(
            "# NO FIGURE OF THIS RUN MAY LEAVE ITS BUNDLE. summary.txt and runs.jsonl are the files "
            "carrying one, and the bundle's README is the one place a figure from them is written.")

        return 0


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--control", required=True, metavar="DIR")
    parser.add_argument("--candidate", required=True, metavar="DIR")
    parser.add_argument("--dotnet", required=True, metavar="PATH")
    parser.add_argument("--shapes", default=str(DEFAULT_SHAPES), metavar="DIR")
    parser.add_argument("--manifest", required=True, metavar="FILE")
    parser.add_argument("--rule", required=True, metavar="FILE")
    parser.add_argument("--out", required=True, metavar="FILE")
    parser.add_argument("--repetitions", type=int, default=REQUIRED_REPETITIONS)
    parser.add_argument("--warmup", type=int, default=REQUIRED_WARMUP)
    parser.add_argument("--fuel", type=int, default=1_000_000_000_000)
    parser.add_argument("--wall", type=int, default=600_000)
    parser.add_argument("--backend", default="x86-64-sysv", metavar="NAME")
    parser.add_argument("--rehearsal", action="store_true")
    arguments = parser.parse_args()

    folder = pathlib.Path(arguments.shapes).resolve()

    if not folder.is_dir():
        refuse(f"no shapes at {folder}")

    shapes = sorted(folder.glob("*.js"))
    population = [
        path for path in shapes if not path.name.endswith(TWIN_SUFFIX) and path.name != FRESH_SHAPE]

    if not population:
        refuse(f"no shapes to measure under {folder}")

    for name, path in (("manifest", arguments.manifest), ("rule", arguments.rule)):
        if not pathlib.Path(path).is_file():
            refuse(f"no {name} at {path}")

    out = pathlib.Path(arguments.out).resolve()

    if out.exists():
        refuse(
            f"{out} already exists",
            "a stopped run is retained beside the new one and named as stopped: move it aside and "
            "name it, and this harness will not overwrite it for you")

    if arguments.rehearsal:
        if ROOT in out.parents:
            refuse(f"a rehearsal may not write into {ROOT}")
    else:
        for name, path in (("manifest", arguments.manifest), ("rule", arguments.rule),
                           *((f"shape {shape.name}", shape) for shape in shapes)):
            is_committed, is_clean = committed_and_clean(path)

            if not is_committed:
                refuse(f"the {name} at {path} is not committed")

            if not is_clean:
                refuse(f"the {name} at {path} has uncommitted changes")

    try:
        manifest = json.loads(pathlib.Path(arguments.manifest).read_text(encoding="utf-8"))
    except (OSError, UnicodeDecodeError, json.JSONDecodeError) as failure:
        refuse(
            f"the manifest at {arguments.manifest} is not JSON this harness can read",
            f"{type(failure).__name__}: {failure}")

    if not isinstance(manifest, dict):
        refuse(f"the manifest at {arguments.manifest} is JSON but not an object")

    rule = read_rule(arguments.rule, population)
    environment, removed = scrubbed_environment()
    check_manifest(manifest, arguments.manifest, arguments, rule, shapes, folder, removed)

    order = list(manifest["shapes"])
    population.sort(key=lambda path: order.index(path.name))

    print("# broiler-js value-form measurement, bundle " + manifest["bundle"])
    print(f"# {datetime.now(timezone.utc).strftime('%Y-%m-%dT%H:%M:%SZ')}")
    print(f"# {platform.platform()}, {platform.machine()}, {os.cpu_count()} logical processors")
    print(f"# {len(population)} shapes, {arguments.warmup} warm-up + {arguments.repetitions} "
          "retained repetitions, interleaved")
    print("#")
    print("# NO WALL-CLOCK FIGURE IS PRINTED HERE. The console says what the harness did; the files")
    print("# beside --out say what it measured, and rule L1 governs what may be done with those.")
    print("#")

    if arguments.rehearsal:
        print("# " + REHEARSAL_BANNER.lstrip("# "))
        print("#")

    return Harness(arguments, manifest, rule, population, environment, removed).run()


if __name__ == "__main__":
    sys.exit(main())
