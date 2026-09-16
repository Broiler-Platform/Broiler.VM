#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# MEASURE WHAT THE BASELINE FORM'S STEP GRANULARITY COSTS, UNDER A RULE FIXED BEFORE THE THING BEING
# MEASURED EXISTED.
#
# --------------------------------------------------------------------------------------------------
# WHAT THIS IS, AND WHAT MAY BE DONE WITH WHAT IT PRODUCES
# --------------------------------------------------------------------------------------------------
#
# Two builds of this host are handed to it: a CONTROL, which emits the baseline form at the
# granularity the tree already has, and a CANDIDATE, which differs from it by the change being
# weighed and by nothing else. It runs both over the same shapes, in both output forms, interleaved,
# with an A/A lane, and it computes the verdict of a decision rule THAT IT DOES NOT ITSELF CONTAIN:
# the constants come out of the rule file's own fenced block, which is committed before any candidate
# code exists, and a rule file whose formulas no longer say what this harness computes is REFUSED
# rather than reinterpreted.
#
# Every figure it produces belongs to one evidence bundle and to nothing else. `roadmap.gates.md`
# section 17 rule 8 admits exactly one evidence class and one predeclared decision per bundle, and
# `docs/baselines.md` rule L1 governs what a retained figure is. A figure from this harness that
# turns up in a roadmap, a ledger, a support table, a decision record or a code remark is a defect in
# the commit that put it there - not a finding this harness made. That is also why NO WALL-CLOCK
# FIGURE REACHES THIS SCRIPT'S CONSOLE OUTPUT: the console says what the harness did, the files say
# what it measured, and a transcript of a run can therefore be read by someone who is not allowed to
# see the figures yet.
#
# --------------------------------------------------------------------------------------------------
# THE RULES IT IMPLEMENTS, RESTATED VERBATIM FROM roadmap.gates.md SECTION 17 BEFORE ANY CODE
# --------------------------------------------------------------------------------------------------
#
#   1. A control that is the same workload minus the thing being measured. A difference between two
#      different programs is a comparison, not an attribution.
#   2. Interleaved lanes. Candidate and control alternate inside each repetition rather than running
#      as two blocks, so a machine that gets slower slows both.
#   3. An A/A lane. The candidate is measured a second time, identically. A candidate-versus-control
#      difference smaller than the A/A difference is reported below resolution, not as a result.
#   4. Every repetition retained, with no outlier policy and no statistical model. The spread between
#      repetitions is most of what a single figure hides.
#   5. A condition checked before and after every lane. The operation must still do what its name
#      says. A measurement whose operation quietly failed is the most dangerous output a harness can
#      produce: it is fast, it is stable, and it is a number for the refusal path.
#   6. An immutable manifest written before either arm runs, carrying both commits with recursive
#      submodule revisions, the clean-tree assertion or the retained patch, the resolved dependency
#      graph, and the SDK and runtime identity.
#   7. Effective, not requested, configuration. Each measured child reports its actual RID, process
#      architecture, GC mode, and tiering state, and the arm fails on a mismatch.
#   8. Exactly one evidence class per bundle, declared up front, with exactly one predeclared
#      decision. A bundle that proves the harness works accepts nothing, even when every number in it
#      moves the right way.
#
# Rule 1 is the arms' business and is asserted by the manifest rather than by this script: it pins
# both commits, and the hashes here are how a reader knows the binaries timed were the binaries the
# manifest describes. Rules 2, 3, 4 and 5 are the lane order, the A/A pair, the retained repetitions
# and the condition below. Rule 6 is the manifest check. Rule 7 is `--runtime`, carried by EVERY
# timed child and compared against the first line recorded. Rule 8 is the bundle's business, and this
# harness contributes to it by refusing to compute any verdict other than the committed rule's.
#
# --------------------------------------------------------------------------------------------------
# THE REFUSALS, ALL OF WHICH HAPPEN BEFORE ANY CHILD IS TIMED
# --------------------------------------------------------------------------------------------------
#
#   * the manifest or the rule file is missing, or does not parse;
#   * either file, or any shape, is not committed, or has uncommitted changes (`--rehearsal` below is
#     the one exception, and it may not write into this repository);
#   * the rule's fenced block still carries a threshold in angle brackets that nobody has fixed;
#   * the rule's own statement of the quantity, the noise floor or the difference is not the one this
#     harness computes - the constants are read from the rule, so the formulas must be the rule's too;
#   * the rule does not quantify its clauses as `if and only if all of`, does not say `REFUSE
#     otherwise`, or does not state clause 1's two conditions or clause 5's one configuration. Every
#     clause this harness applies is pinned, not only the ones it reads a constant out of: a clause
#     that is computed here and stated nowhere is the harness's rule and not the committed one;
#   * any binary's SHA-256 differs from the manifest, or an arm holds a binary the manifest does not
#     name, or the manifest names one the arm does not hold;
#   * the two arms are the same bytes and the manifest has not declared that on purpose;
#   * a shape's git blob id differs from the manifest's, or the folder holds a shape the manifest does
#     not name;
#   * this script's own blob id differs from the manifest's;
#   * R is not 7, W is not 3, or R, W, the allowances, the backend or the lane order differ from the
#     manifest;
#   * the set of DOTNET_* and COMPlus_* keys in this environment is not the set the manifest says was
#     scrubbed. They are scrubbed out of every child's environment and the list is recorded in
#     `harness.log`; what the manifest pins is WHICH ones were there, because that is part of the
#     configuration a later reader cannot recover.
#
# A refusal exits 2 and writes no measurement. A condition that fails once timing has started exits 1,
# names the lane in `harness.log`, and writes no `summary.txt`: a run whose operation stopped doing
# what its name says has no summary to write.
#
# --------------------------------------------------------------------------------------------------
# THE QUANTITY
# --------------------------------------------------------------------------------------------------
#
# For a shape s, an arm a and a form f, one repetition measures `run_ms(a, f, s) - check_ms(a, f, s)`.
# The run lane runs the program; the check lane is the same command with `--check`, which compiles,
# verifies and stops. The difference is what instantiating and executing cost, with process start,
# JIT warm-up, reading the file, parsing, lowering, emitting and verifying subtracted out because
# they are in both lanes. Both raw wall clocks are retained in `runs.jsonl`; the subtraction removes
# what is common and does not remove variance, which is why the A/A lane exists.
#
# --------------------------------------------------------------------------------------------------
# THE LANES OF ONE REPETITION, IN THIS FIXED ORDER, PER SHAPE
# --------------------------------------------------------------------------------------------------
#
#   C-bytecode-check, C-bytecode-run, C-native-check, C-native-run,
#   K-bytecode-check, K-bytecode-run, K-native-check, K-native-run,
#   K-native-check-again, K-native-run-again,          (the A/A pair, native)
#   K-bytecode-check-again, K-bytecode-run-again       (the A/A pair, bytecode)
#
# Twelve processes per shape per repetition. The A/A lanes are the candidate run a second time under
# a different name and nothing else, so the difference between their medians is ONE OBSERVATION of
# this machine's noise floor for this shape and form - not an estimate of it.
#
# The shapes themselves are visited in the MANIFEST's order, not the directory's. The manifest names
# the same files either way - the run is refused unless it does - so the two orders measure the same
# thing; the manifest's is the one a reader can check against a committed file.
#
# --------------------------------------------------------------------------------------------------
# THE CONDITION (RULE 5), CHECKED BEFORE ANY TIMING, ON EVERY LANE, AND AGAIN AFTER ALL OF IT
# --------------------------------------------------------------------------------------------------
#
#   1. Before any timing, per shape: each of the four arm-and-form combinations is run once, untimed,
#      and each of their check lanes as well. All four must exit 0 and print IDENTICAL standard
#      output. That output's SHA-256 becomes the shape's expected answer, and each check lane's
#      becomes its expected check output. Then FUEL PARITY: the smallest completing `--fuel` of the
#      shape's `.small.js` twin is bisected under the control's bytecode lane, all four combinations
#      must complete at that figure, and all four must refuse with `AllowanceExhausted on Fuel` at one
#      less. A fuel figure is exact and machine-independent, which is why it may be printed here. A
#      population shape with no twin stops the harness rather than being bisected over itself.
#   2. Every lane, as it runs: the process exits 0, its standard output hashes to the expected answer
#      (run lanes) or to the expected check output (check lanes), and its `--runtime` line equals the
#      first one recorded. A mismatch stops the harness at once.
#   3. After all timing, per shape: step 1's equivalence again, and its answers must equal step 1's.
#
# A shape whose lanes disagree stops the whole harness. It is not excluded and timed around, because
# an excluded shape is exactly the one a reader would want to know about.
#
# --------------------------------------------------------------------------------------------------
# WHAT THIS HARNESS DELIBERATELY DOES NOT DO
# --------------------------------------------------------------------------------------------------
#
#   * No benchmarking framework, whose warm-up, pilot and outlier policies would be part of every
#     figure and invisible in this repository.
#   * No pilot phase and no adaptive iteration count. The shapes' literals are constants, fixed in the
#     folder's README by a written argument in units of charged fuel rather than by a trial run.
#   * No mean, no outlier policy, no model: the median of the retained repetitions, beside all of them.
#   * No verdict of its own. The thresholds come out of the committed rule file, and a rule this
#     harness cannot parse is refused rather than approximated.
#
# --------------------------------------------------------------------------------------------------
# DEPARTURES FROM THE DESIGN THIS WAS WRITTEN FROM, STATED HERE RATHER THAN LEFT TO BE FOUND
# --------------------------------------------------------------------------------------------------
#
#   1. `--quiet` IS NOT PASSED TO A TIMED CHILD, though the design's command line carries it. This
#      host prints a completion value only when `--quiet` is ABSENT, so under it every lane's standard
#      output is empty and "the lane's output equals the shape's expected answer" would compare two
#      empty strings - the check that exists to catch a faster wrong answer, reduced to nothing. The
#      value is printed instead, in the run lanes of both arms and both forms, and the check lanes
#      print nothing because they run nothing; each is compared against its own recorded expectation.
#      The cost of printing one short line is in every run lane of every arm, so the subtraction and
#      the comparison between arms both carry it identically.
#   2. `--rehearsal` EXISTS, AND NO BUNDLE MAY USE IT. Committing a harness that has never been run is
#      not an option, and the manifest it will be run under does not exist until the arms do. The flag
#      relaxes exactly one thing - that the manifest, the rule and the shapes be committed and clean -
#      and in exchange it refuses to write anywhere inside this repository, stamps `"rehearsal": true`
#      into every line of `runs.jsonl`, and heads every output file with a line saying no figure in it
#      may be retained. Everything else is enforced exactly as in an evidence run.
#   3. MACHINE STATE IS SAMPLED BY WHICHEVER PROBE THIS HOST ANSWERS. `typeperf` needs membership of
#      the Performance Log Users group and refuses without it, and `Get-Counter` refuses where the
#      counter names are localised. The harness tries `typeperf`, then `Get-Counter`, then
#      `Win32_Processor`'s LoadPercentage, records WHICH ONE ANSWERED and what the others said, and
#      does not stop for a probe that refuses: machine state is context for a reader, not a gate.
#   4. THE POPULATION IS THE SHAPES FOLDER MINUS TWO KINDS OF FILE: `*.small.js`, which are the
#      fuel-parity twins rather than workloads, and `fresh-small.js`, which is the fresh-process shape
#      and is judged by no rule. `--fresh` measures that one and computes no verdict.
#   5. A RUNTIME KNOB IN THIS ENVIRONMENT IS SCRUBBED AND PINNED, NOT REFUSED. The design's fifth
#      refusal is "the child environment carries any `DOTNET_*` or `COMPlus_*` key", and it also asks
#      for those keys to be scrubbed out of every child - which cannot both hold, because a scrubbed
#      child environment carries none of them and there would be nothing left to refuse. What is
#      refused instead is a SET that differs from `manifest.environment.scrubbed`: the keys are
#      removed from every child, the list is recorded in `harness.log`, and the manifest pins WHICH
#      ones this machine had, because that is the part of the configuration a later reader cannot
#      recover. A knob set on the machine is then not a reason to stop, and a knob that appeared or
#      vanished since the manifest was written is.
#
#   python3 eng/measure-baseline-granularity.py --control <dir> --candidate <dir>
#                                               --manifest <bundle>/manifest.json
#                                               --rule <bundle>/decision-rule.md
#                                               --out <bundle>/runs.jsonl
#                                               [--shapes <dir>] [--repetitions 7] [--warmup 3]
#                                               [--fuel N] [--wall MS] [--backend NAME]
#                                               [--fresh] [--rehearsal]

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

# THE SHAPE THE FRESH-PROCESS COMPARISON USES, AND THE ONE FILE OF THE FOLDER NO RULE JUDGES.
FRESH_SHAPE = "fresh-small.js"

# The twins are not workloads: each is its shape with a smaller literal, and they exist so that fuel
# parity can be bisected in a moment rather than over the measured literal.
TWIN_SUFFIX = ".small.js"

# THE COUNTS ARE NOT THE CALLER'S TO CHOOSE. `docs/baselines.md` rule 4 fixes seven retained
# repetitions per lane and the core's register is the authority for it (JSC-18); three warm-up
# repetitions, retained and named as discarded, are `eng/compare-forms.py` rule 5's precedent in this
# family. Both are checked against the manifest AND against the rule file, so three statements of
# them have to agree before anything runs.
REQUIRED_REPETITIONS = 7
REQUIRED_WARMUP = 3

# THE LANE ORDER, WHICH IS PART OF THE MEASUREMENT AND NOT AN IMPLEMENTATION DETAIL. It is pinned in
# the manifest and compared here, because interleaving that changed between the manifest and the run
# would make the control and the candidate meet the machine at different moments.
LANES = [
    ("C-bytecode-check", "C", "control", "bytecode", "check"),
    ("C-bytecode-run", "C", "control", "bytecode", "run"),
    ("C-native-check", "C", "control", "native", "check"),
    ("C-native-run", "C", "control", "native", "run"),
    ("K-bytecode-check", "K", "candidate", "bytecode", "check"),
    ("K-bytecode-run", "K", "candidate", "bytecode", "run"),
    ("K-native-check", "K", "candidate", "native", "check"),
    ("K-native-run", "K", "candidate", "native", "run"),
    ("K-native-check-again", "K-again", "candidate", "native", "check"),
    ("K-native-run-again", "K-again", "candidate", "native", "run"),
    ("K-bytecode-check-again", "K-again", "candidate", "bytecode", "check"),
    ("K-bytecode-run-again", "K-again", "candidate", "bytecode", "run"),
]
LANE_NAMES = [lane[0] for lane in LANES]
ARMS = ("C", "K", "K-again")

# The four arm-and-form combinations the condition compares. The A/A lane is the candidate again, so
# it adds no combination: an answer it gave that the candidate's own lane did not would be a defect
# in the machine rather than in either form.
COMBINATIONS = [
    ("control", "bytecode"), ("control", "native"),
    ("candidate", "bytecode"), ("candidate", "native"),
]

EXHAUSTED_ON_FUEL = "AllowanceExhausted on Fuel"
RUNTIME_PREFIX = "runtime-identifier="

# THE MACHINE-STATE PROBES, IN ORDER, AND THE HARNESS USES THE FIRST THAT ANSWERS. See departure 3.
MACHINE_PROBES = [
    ("typeperf", ["typeperf", "\\Processor(_Total)\\% Processor Time", "-sc", "3"]),
    ("get-counter", [
        "powershell", "-NoProfile", "-NonInteractive", "-Command",
        "(Get-Counter '\\Processor(_Total)\\% Processor Time' -SampleInterval 1 -MaxSamples 3)"
        ".CounterSamples | ForEach-Object { $_.CookedValue }"]),
    ("win32-processor", [
        "powershell", "-NoProfile", "-NonInteractive", "-Command",
        "(Get-CimInstance Win32_Processor | Measure-Object -Property LoadPercentage -Average).Average"]),
]
PROCESS_COUNT = [
    "powershell", "-NoProfile", "-NonInteractive", "-Command", "(Get-Process).Count"]

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


def git(arguments, cwd):
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
    code, out, _ = git(["log", "-1", "--format=%H", "--", str(path)], ROOT)

    if code != 0 or not out:
        return False, False

    code, out, _ = git(["status", "--porcelain", "--", str(path)], ROOT)

    return True, code == 0 and not out


def binaries_of(directory):
    """Every runnable byte of an arm, by name and SHA-256."""
    directory = pathlib.Path(directory)

    return {
        path.name: sha256_of(path)
        for path in sorted(directory.iterdir())
        if path.is_file() and path.suffix.lower() in (".dll", ".exe")
    }


def cli_of(directory):
    """The host binary of one arm, with the suffix APPENDED (this assembly's name is dotted)."""
    binary = pathlib.Path(directory) / "Broiler.VM.Composition.JavaScript.Cli"

    if not binary.exists() and binary.with_name(binary.name + ".exe").exists():
        binary = binary.with_name(binary.name + ".exe")

    if not binary.exists():
        refuse(f"no host binary at {binary}")

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
    """A threshold out of the rule file, or a refusal naming which one could not be read."""
    try:
        return float(text)
    except ValueError:
        refuse(f"{path} states {what} as `{text}`, which is not a number")


def read_rule(path, population):
    """The decision rule's constants, read out of its own fenced block and out of nothing else.

    THE HARNESS OWNS NO THRESHOLD. What it owns is the shape of the computation, so the block has to
    state the quantity, the noise floor and the difference in the terms this file computes them in -
    a rule whose formulas have been edited away from those is refused rather than reinterpreted, and
    a threshold still in angle brackets is a rule the owner has not finished writing.
    """
    try:
        text = pathlib.Path(path).read_text(encoding="utf-8")
    except (OSError, UnicodeDecodeError) as failure:
        refuse(
            f"the rule at {path} cannot be read as text",
            f"{type(failure).__name__}: {failure}")

    block = re.search(r"```rule\n(.*?)```", text, re.S)

    if not block:
        refuse(f"{path} carries no fenced `rule` block, so it states no rule this harness can read")

    body = block.group(1)

    if re.search(r"<(?!=)", body):
        unfixed = re.findall(r"<(?!=)[^<>\n]*>?", body)
        refuse(
            "the rule still carries a threshold nobody has fixed: " + ", ".join(unfixed),
            "the owner fixes every angle-bracketed figure before the rule is committed")

    # EVERY PART OF THE RULE THIS HARNESS APPLIES IS PINNED, NOT JUST THE PARTS IT READS CONSTANTS
    # OUT OF. A clause whose text is not checked is a clause the harness computes from its own
    # formula while the file beside the summary says something else - which is precisely the
    # substitution the predeclared decision exists to prevent. The three quantities come first, then
    # the quantifier over the clauses, then the two clauses whose wording is all there is to check
    # because their enforcement is a refusal rather than a computation.
    for statement, what in (
            ("run_ms - check_ms", "the quantity"),
            ("Q(shape, K, form) - Q(shape, K-again, form)", "the noise floor"),
            ("Q(shape, C, form) - Q(shape, K, form)", "the difference"),
            ("ADOPT per-block steps if and only if all of", "that every clause must hold"),
            ("REFUSE otherwise", "what it does when one does not"),
            ("condition-before", "clause 1's before-condition"),
            ("condition-after", "clause 1's after-condition"),
            ("identical across arms, lanes and repetitions", "clause 5's one effective configuration")):
        if statement not in body:
            refuse(
                f"the rule does not state {what} as `{statement}`",
                "this harness computes that formula, so a rule stating another one is not this "
                "harness's rule and the two may not be mixed")

    repetitions = re.search(r"R=(\d+)", body)
    warmup = re.search(r"W=(\d+)", body)

    # THE COUNT OF SHAPES MAY BE AN EXPRESSION WITH SPACES IN IT - `ceil(2/3 * shapes)` is the form
    # the design proposes - so it is not read as one non-blank word. The thresholds beside it are
    # read as numbers rather than as whatever punctuation the clause ends with: a rule that ends its
    # clauses in semicolons is still the rule, and a harness that refused it would be refusing over
    # its own parser rather than over anything the owner decided.
    majority = re.search(
        r"at least\s+(.+?)\s+shapes:\s*D > N and Q\(K\)/Q\(C\) <=\s*([0-9.]+)", body)
    interpreter = re.search(
        r"\|D\| <= N or \|D\| <=\s*([0-9.]+)\s*\* Q\(C, bytecode\)", body)

    # CLAUSE 2 IS READ, NOT ASSUMED. The harness used to compute `D >= -N` from a constant of its
    # own while accepting a block that said `D >= -2N` or said nothing at all, so the multiplier
    # comes out of the rule exactly as clauses 3 and 4's thresholds do. An absent multiplier is the
    # design's own `D >= -N` and means one.
    never_slower = re.search(r"D >= -\s*([0-9.]*)\s*\*?\s*N", body)

    if not (repetitions and warmup and majority and interpreter and never_slower):
        refuse(
            f"{path}'s rule block does not state R, W, the never-slower clause, the majority clause "
            "and the interpreter clause in the form this harness reads")

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
        "never-slower-multiplier": (
            number(never_slower.group(1), "the multiplier of clause 2", path)
            if never_slower.group(1) else 1.0),
        "ratio": number(majority.group(2), "the ratio of clause 3", path),
        "interpreter-fraction": number(interpreter.group(1), "the fraction of clause 4", path),
        "population-line": next(
            (line for line in body.splitlines() if line.startswith("population:")), ""),
    }


def check_manifest(manifest, path, arguments, rule, shapes, folder, removed):
    """Rule 6: the run is refused unless it is the run the manifest describes."""
    for field in ("bundle", "arms", "repetitions", "warmup", "lanes", "shapes", "harness",
                  "allowances", "environment", "fuel-credit"):
        if field not in manifest:
            refuse(f"{path} states no `{field}`, so it does not pin the run it is supposed to pin")

    if manifest["repetitions"] != REQUIRED_REPETITIONS or manifest["warmup"] != REQUIRED_WARMUP:
        refuse(
            f"the manifest asks for R={manifest['repetitions']} and W={manifest['warmup']}; this "
            f"family's counts are R={REQUIRED_REPETITIONS} and W={REQUIRED_WARMUP}, and they are not "
            "the caller's to choose")

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
            f"manifest: {manifest['harness']}",
            f"running:  {mine}")

    on_disk = {path.name: blob_id(path) for path in sorted(folder.glob("*.js"))}

    if manifest["shapes"] != on_disk:
        missing = sorted(set(manifest["shapes"]) - set(on_disk))
        extra = sorted(set(on_disk) - set(manifest["shapes"]))
        moved = sorted(
            name for name in set(on_disk) & set(manifest["shapes"])
            if on_disk[name] != manifest["shapes"][name])
        refuse(
            "the shapes are not the shapes the manifest pins",
            f"named and absent: {missing or 'none'}",
            f"present and unnamed: {extra or 'none'}",
            f"changed since the manifest: {moved or 'none'}")

    declared = manifest["environment"].get("scrubbed", None)

    if declared is None or sorted(declared) != removed:
        refuse(
            "the runtime knobs in this environment are not the ones the manifest was written under",
            f"manifest: {sorted(declared) if declared is not None else 'not stated'}",
            f"here:     {removed or 'none'}")

    arms = manifest["arms"]
    held = {}

    for arm, directory in (("control", arguments.control), ("candidate", arguments.candidate)):
        if arm not in arms or "binaries" not in arms[arm]:
            refuse(f"the manifest names no binaries for the {arm} arm")

        held[arm] = binaries_of(directory)

        if held[arm] != arms[arm]["binaries"]:
            named = set(arms[arm]["binaries"])
            there = set(held[arm])
            refuse(
                f"the {arm} arm is not the arm the manifest hashes",
                f"named and absent: {sorted(named - there) or 'none'}",
                f"present and unnamed: {sorted(there - named) or 'none'}",
                "changed: " + (", ".join(
                    sorted(name for name in named & there
                           if held[arm][name] != arms[arm]['binaries'][name])) or "none"))

    if held["control"] == held["candidate"] and not arms.get("identical-by-design", False):
        refuse(
            "the two arms are the same bytes, so there is nothing between them to attribute",
            "a run of one build against itself is an A/A run and the manifest has to say so, with "
            "`\"identical-by-design\": true` under `arms`")

    return held


def invoke(binary, form, shape, arguments, check, environment, fuel=None):
    """One child process. Every timed child carries `--runtime` (rule 7)."""
    command = [str(binary), "--runtime"]

    if form == "native":
        command += ["--native", arguments.backend]

    command += ["--fuel", str(arguments.fuel if fuel is None else fuel), "--wall", str(arguments.wall)]

    if check:
        command.append("--check")

    command.append(str(shape))
    started = time.perf_counter()

    # THE DECODING IS NAMED RATHER THAN INHERITED. `text=True` decodes a child's output with the
    # HOST's locale encoding, which on this machine is cp1252 and has undefined byte values in it; a
    # child that emits one kills the reader thread, and what comes back is None rather than a string.
    # The first rehearsal of this harness died that way, in a machine-state probe, before a single
    # lane had run. This host writes UTF-8, so UTF-8 is what is asked for, and an undecodable byte
    # becomes a replacement character instead of ending the run.
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


def smallest_completing_fuel(binary, form, shape, arguments, environment):
    """The least allowance this program finishes under, by bisection: exact, and machine-independent."""
    high = arguments.fuel

    if invoke(binary, form, shape, arguments, False, environment, fuel=high)["exit"] != 0:
        refuse(f"{shape.name} does not complete under the run allowance, so parity cannot be bisected")

    low = 1

    while low < high:
        middle = (low + high) // 2

        if invoke(binary, form, shape, arguments, False, environment, fuel=middle)["exit"] == 0:
            high = middle
        else:
            low = middle + 1

    return low


class Harness:
    def __init__(self, arguments, manifest, rule, population, folder, environment, removed):
        self.arguments = arguments
        self.manifest = manifest
        self.rule = rule
        self.population = population
        self.folder = folder
        self.environment = environment
        self.removed = removed
        self.out = pathlib.Path(arguments.out)
        self.directory = self.out.parent
        self.directory.mkdir(parents=True, exist_ok=True)
        self.binaries = {
            "control": cli_of(arguments.control),
            "candidate": cli_of(arguments.candidate),
        }
        self.expected = {}
        self.runtime_line = None
        self.machine_probe = None
        self.machine_refusals = []
        self.runs = []
        self.log_lines = []
        self.runtime_reports = []

    # ---- the files it writes -----------------------------------------------------------------
    def head(self):
        return [REHEARSAL_BANNER] if self.arguments.rehearsal else []

    def write(self, name, lines):
        path = self.directory / name
        path.write_text(
            "\n".join(self.head() + lines) + "\n", encoding="utf-8", newline="\n")

    def log(self, line):
        self.log_lines.append(line)

    def say(self, line):
        """The console, which carries no wall-clock figure."""
        print(line, flush=True)

    def stop(self, *lines):
        """A condition failed once timing had started: no summary is written."""
        for line in lines:
            self.log("STOPPED: " + line)
            print("# STOPPED: " + line, file=sys.stderr)

        self.write("harness.log", self.log_lines)
        self.write_runs()
        raise SystemExit(1)

    def write_runs(self):
        with self.out.open("w", encoding="utf-8", newline="\n") as handle:
            for row in self.runs:
                handle.write(json.dumps(row, sort_keys=True) + "\n")

    # ---- the condition (rule 5) --------------------------------------------------------------
    def observe(self, arm, form, shape, check, label, lines):
        answer = invoke(self.binaries[arm], form, shape, self.arguments, check, self.environment)
        digest = sha256_text(answer["stdout"])
        lines.append(
            f"{label:<28} {arm:<9} {form:<8} {'check' if check else 'run':<5} "
            f"exit {answer['exit']} stdout {digest}")

        if answer["exit"] != 0:
            self.stop(
                f"{label} for {shape.name}: {arm} {form} "
                f"{'check' if check else 'run'} exited {answer['exit']}",
                (answer["stderr"].strip().splitlines() or ["(no complaint)"])[-1])

        self.check_runtime(answer, f"{label} {shape.name} {arm} {form}")

        return digest

    def check_runtime(self, answer, where):
        if not answer["runtime"]:
            self.stop(f"{where} reported no effective configuration, which rule 7 requires")

        if self.runtime_line is None:
            self.runtime_line = answer["runtime"]
            self.log("effective configuration, from the first child: " + self.runtime_line)

        self.runtime_reports.append(f"{where}: {answer['runtime']}")

        if answer["runtime"] != self.runtime_line:
            self.stop(
                f"{where} ran under a different effective configuration (rule 7)",
                "first: " + self.runtime_line,
                "here:  " + answer["runtime"])

    def condition(self, label):
        lines = [f"# {label}: every arm and form over every shape, untimed, with --runtime",
                 f"# {datetime.now(timezone.utc).strftime('%Y-%m-%dT%H:%M:%SZ')}"]
        answers = {}

        for shape in self.population:
            self.say(f"# {label}: {shape.name}")
            seen = {}

            for arm, form in COMBINATIONS:
                seen[(arm, form)] = self.observe(arm, form, shape, False, label, lines)

            distinct = set(seen.values())

            if len(distinct) != 1:
                self.stop(
                    f"{label}: the four arm-and-form combinations do not agree on {shape.name}",
                    "; ".join(f"{arm}-{form} {digest[:16]}" for (arm, form), digest in seen.items()),
                    "NOTHING IS TIMED AROUND A SHAPE WHOSE FORMS DISAGREE: a faster wrong answer is "
                    "the failure this harness exists to refuse")

            answer = distinct.pop()
            checks = {
                f"{arm}-{form}": self.observe(arm, form, shape, True, label, lines)
                for arm, form in COMBINATIONS
            }
            answers[shape.name] = {"answer": answer, "checks": checks}
            lines.append(f"{shape.name}: every combination answers {answer}")

        return answers, lines

    def fuel_parity(self, lines):
        """All four combinations complete at one figure and refuse at one less."""
        for shape in self.population:
            twin = shape.with_name(shape.stem + TWIN_SUFFIX)

            if not twin.exists():
                # A MISSING TWIN IS A REFUSAL AND NOT A FALLBACK. Bisecting over the measured shape
                # instead would run the allowance search across a hundred million units of work
                # rather than a twenty-thousand-unit twin, on every arm and form, before any timing
                # - so the quiet substitution costs an hour and says nothing about it. The fresh
                # shape is the one exception: it IS the small one, and no rule judges it.
                if shape.name != FRESH_SHAPE:
                    self.stop(
                        f"{shape.name} has no `{shape.stem + TWIN_SUFFIX}`, so fuel parity has no "
                        "twin to bisect",
                        "every population shape carries one, and a folder missing one is not the "
                        "population this rule was written over")

                twin = shape
                lines.append(
                    f"{shape.name}: the fresh-process shape is its own twin, so parity is bisected "
                    "over the shape itself")

            figure = smallest_completing_fuel(
                self.binaries["control"], "bytecode", twin, self.arguments, self.environment)
            self.say(f"# fuel parity: {twin.name} completes at {figure}")
            lines.append(f"{twin.name}: smallest completing allowance {figure}, under C-bytecode")

            for arm, form in COMBINATIONS:
                completed = invoke(
                    self.binaries[arm], form, twin, self.arguments, False, self.environment,
                    fuel=figure)
                refused = invoke(
                    self.binaries[arm], form, twin, self.arguments, False, self.environment,
                    fuel=figure - 1)

                if completed["exit"] != 0:
                    self.stop(
                        f"fuel parity: {arm} {form} does not complete {twin.name} at {figure}")

                if refused["exit"] == 0 or EXHAUSTED_ON_FUEL not in refused["stderr"]:
                    self.stop(
                        f"fuel parity: {arm} {form} does not refuse {twin.name} at {figure - 1} "
                        f"with `{EXHAUSTED_ON_FUEL}`",
                        (refused["stderr"].strip().splitlines() or ["(no complaint)"])[-1])

                lines.append(
                    f"    {arm:<9} {form:<8} completes at {figure}, "
                    f"{EXHAUSTED_ON_FUEL} at {figure - 1}")

    # ---- machine state -----------------------------------------------------------------------
    def choose_machine_probe(self):
        # NOTHING HERE MAY STOP A RUN. Machine state is context for a reader and not a gate, so every
        # failure a probe can have - missing, refused, timed out, or answering bytes that do not
        # decode - is recorded and stepped over. The first rehearsal of this harness died in this
        # loop, which is the whole argument for the `except Exception` below being the right width.
        for name, command in MACHINE_PROBES:
            try:
                done = subprocess.run(
                    command, capture_output=True, encoding="utf-8", errors="replace", timeout=60)
            except Exception as failure:  # noqa: BLE001 - see the paragraph above
                self.machine_refusals.append(f"{name}: {type(failure).__name__}: {failure}")
                continue

            if done.returncode == 0 and (done.stdout or "").strip():
                self.machine_probe = (name, command)
                self.log(f"machine-state probe: {name} answers here")

                for refusal in self.machine_refusals:
                    self.log("machine-state probe refused: " + refusal)

                return

            self.machine_refusals.append(
                f"{name}: exit {done.returncode}: "
                + " / ".join(((done.stderr or "") + (done.stdout or "")).strip().splitlines()[:2]))

        self.log("machine-state: no probe on this host answered, and the refusals are recorded")

        for refusal in self.machine_refusals:
            self.log("machine-state probe refused: " + refusal)

    def machine_state(self, phase, repetition, lines):
        stamp = datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ")

        if self.machine_probe is None:
            lines.append(f"{stamp} repetition {repetition} {phase}: no probe answers on this host")
            return

        name, command = self.machine_probe

        try:
            load = subprocess.run(
                command, capture_output=True, encoding="utf-8", errors="replace", timeout=120)
            count = subprocess.run(
                PROCESS_COUNT, capture_output=True, encoding="utf-8", errors="replace", timeout=120)
        except Exception as failure:  # noqa: BLE001 - a sample is never worth a run
            lines.append(
                f"{stamp} repetition {repetition} {phase}: {name} did not answer: "
                f"{type(failure).__name__}: {failure}")
            return

        lines.append(
            f"{stamp} repetition {repetition} {phase}: {name} "
            + " ".join((load.stdout or "").split())
            + " | processes " + (count.stdout or "").strip())

    # ---- the timed lanes ---------------------------------------------------------------------
    def measure(self):
        machine = [f"# machine state before and after every repetition",
                   f"# probe: {self.machine_probe[0] if self.machine_probe else 'none answered'}"]
        total = self.arguments.warmup + self.arguments.repetitions

        for repetition in range(total):
            warm = repetition < self.arguments.warmup
            self.say(
                f"# repetition {repetition + 1} of {total} "
                f"({'warm-up, retained and discarded' if warm else 'retained'})")
            self.machine_state("before", repetition + 1, machine)

            for shape in self.population:
                expectation = self.expected[shape.name]

                for order, (name, arm, which, form, kind) in enumerate(LANES):
                    answer = invoke(
                        self.binaries[which], form, shape, self.arguments, kind == "check",
                        self.environment)
                    digest = sha256_text(answer["stdout"])
                    wanted = (
                        expectation["checks"][f"{which}-{form}"] if kind == "check"
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

                    self.runs.append(row)

                    if condition != "ok":
                        self.stop(
                            f"lane {name} on {shape.name}, repetition {repetition + 1}: {condition}",
                            (answer["stderr"].strip().splitlines() or ["(no complaint)"])[-1],
                            "no summary is written: a lane that stopped doing what it is named for "
                            "has no figure worth keeping")

                    self.check_runtime(
                        answer, f"repetition {repetition + 1} {shape.name} {name}")

            self.machine_state("after", repetition + 1, machine)

        self.write("machine-state.log", machine)

    # ---- what the rule makes of it -----------------------------------------------------------
    def quantities(self):
        quantity = {}

        for shape in self.population:
            quantity[shape.name] = {}

            for form in ("native", "bytecode"):
                quantity[shape.name][form] = {}

                for arm in ARMS:
                    attributed = []

                    for repetition in range(
                            self.arguments.warmup + 1,
                            self.arguments.warmup + self.arguments.repetitions + 1):
                        lanes = {
                            row["lane"]: row["wall_ms"] for row in self.runs
                            if row["rep"] == repetition and row["shape"] == shape.name
                            and row["arm"] == arm and row["form"] == form
                        }
                        attributed.append(lanes["run"] - lanes["check"])

                    quantity[shape.name][form][arm] = {
                        "repetitions": attributed,
                        "median": statistics.median(attributed),
                    }

        return quantity

    def compare_conditions(self, before, after):
        """Rule 5's bookend: BOTH halves of the before-condition, not only the run lanes' answer.

        Each check lane's output was recorded before the timing, and every check lane of every
        repetition was compared against it - the subtraction that is the whole quantity rests on
        those lanes doing what they are named for. A bookend that compared only the run answer would
        pass a run whose check lanes had started printing something else partway through, which is
        the one direction the per-lane comparison cannot cover: it compares each lane against the
        SAME recorded digest, so a check lane that changed once and stayed changed would be caught,
        and one that changed only between the last timed lane and here would not.
        """
        for name, expectation in before.items():
            if after[name]["answer"] != expectation["answer"]:
                self.stop(
                    f"condition-after: {name} answers something else than it did before the timing",
                    "every figure of this run is void")

            for combination, digest in expectation["checks"].items():
                if after[name]["checks"][combination] != digest:
                    self.stop(
                        f"condition-after: {name}'s {combination} check lane prints something else "
                        "than it did before the timing",
                        "every figure of this run is void")

    def summarise(self, quantity):
        lines = [
            "# What the granularity measurement of the baseline form found, and nothing else.",
            "#",
            "# Q is the median over the retained repetitions of (run_ms - check_ms). N is the A/A",
            "# noise floor, |Q(K) - Q(K-again)|, one observation of it rather than an estimate. D is",
            f"# Q(C) - Q(K); positive means the candidate is faster. The verdict below is computed",
            f"# ONLY from the fenced block of {self.rule['path']} (sha256 {self.rule['sha256']}).",
            "#",
            f"# repetitions retained {self.arguments.repetitions}, warm-up {self.arguments.warmup}",
            f"# rule majority clause: {self.rule['majority-source']} "
            f"= {self.rule['majority-shapes']} of {len(self.population)} shapes, "
            f"ratio {self.rule['ratio']}, interpreter fraction {self.rule['interpreter-fraction']}",
            "",
        ]
        judged = {}

        for form in ("native", "bytecode"):
            lines.append(f"## {form}")
            lines.append(
                f"{'shape':<18} {'Q(C)':>12} {'Q(K)':>12} {'Q(K-again)':>12} {'N':>10} {'D':>10}  "
                "resolution")

            for shape in self.population:
                control = quantity[shape.name][form]["C"]["median"]
                candidate = quantity[shape.name][form]["K"]["median"]
                again = quantity[shape.name][form]["K-again"]["median"]
                floor = abs(candidate - again)
                difference = control - candidate
                judged[(shape.name, form)] = (control, candidate, again, floor, difference)
                lines.append(
                    f"{shape.name:<18} {control:12.1f} {candidate:12.1f} {again:12.1f} "
                    f"{floor:10.1f} {difference:10.1f}  "
                    + ("below resolution" if abs(difference) <= floor else "above the floor"))

            lines.append("")

        if self.arguments.fresh:
            lines.append(
                "NO VERDICT. This is the fresh-process comparison, and the decision rule's "
                "population does not include it.")
            self.write("summary.txt", lines)
            return

        clauses = []
        multiplier = self.rule["never-slower-multiplier"]
        never_slower = [
            name for name in (shape.name for shape in self.population)
            if judged[(name, "native")][4] < -multiplier * judged[(name, "native")][3]]
        clauses.append((
            "2 native, never slower beyond the floor", not never_slower,
            "slower beyond the floor on: " + (", ".join(never_slower) or "no shape")))
        # A RATIO IS ONLY A RATIO WHERE THE CONTROL'S QUANTITY IS POSITIVE. Q is a difference between
        # two wall clocks, so a shape whose run lane did not clearly exceed its check lane can leave
        # Q(C) at or below zero - and `Q(K)/Q(C) <= r`, read through a multiplication, would then
        # admit that shape for the arithmetic's reasons rather than the measurement's. Such a shape
        # does not clear this clause, and the summary names it instead of quietly counting it.
        # `eng/compare-forms.py` refuses to print a ratio through a non-positive median for exactly
        # this reason.
        unreadable = [
            shape.name for shape in self.population
            if judged[(shape.name, "native")][0] <= 0.0]
        faster = [
            name for name in (shape.name for shape in self.population)
            if name not in unreadable
            and judged[(name, "native")][4] > judged[(name, "native")][3]
            and judged[(name, "native")][1] <= self.rule["ratio"] * judged[(name, "native")][0]]
        clauses.append((
            f"3 native, {self.rule['majority-shapes']} shapes clear the floor and the ratio",
            len(faster) >= self.rule["majority-shapes"],
            f"{len(faster)} of {len(self.population)}: " + (", ".join(faster) or "none")
            + (f"; no ratio is read on {', '.join(unreadable)}, whose Q(C) is not positive"
               if unreadable else "")))
        moved = [
            name for name in (shape.name for shape in self.population)
            if not (abs(judged[(name, "bytecode")][4]) <= judged[(name, "bytecode")][3]
                    or abs(judged[(name, "bytecode")][4])
                    <= self.rule["interpreter-fraction"] * judged[(name, "bytecode")][0])]
        clauses.append((
            "4 bytecode, the interpreter is not moved", not moved,
            "moved on: " + (", ".join(moved) or "no shape")))

        lines.append("## the rule, clause by clause")

        # CLAUSES 1 AND 5 ARE NOT EVALUATED HERE, AND THE TABLE MAY NOT PRETEND THEY WERE. Both are
        # enforced by stopping: a condition that fails, before, during or after the timing, and a
        # child whose effective-configuration line differs from the first one recorded, each end the
        # run without writing this file. Printing them as two rows that always read MET would put
        # two constants among three measured clauses and invite a reader to count five.
        lines.append(
            "        clauses 1 and 5 are not computed here. Each is enforced by a refusal - a")
        lines.append(
            "        condition that fails, or a child whose effective-configuration line differs")
        lines.append(
            "        from the first recorded, ends the run and writes no summary - so a summary")
        lines.append(
            "        that exists is what those two clauses assert.")
        lines.append(
            "        every timed child reported " + (self.runtime_line or "nothing"))

        for name, held, detail in clauses:
            lines.append(f"{'MET    ' if held else 'NOT MET'} {name}: {detail}")

        lines.append("")
        lines.append(
            "VERDICT: " + ("ADOPT" if all(held for _, held, _ in clauses) else "REFUSE")
            + " per-block steps, by the rule above and by nothing else.")
        self.write("summary.txt", lines)

    # ---- the run -----------------------------------------------------------------------------
    def run(self):
        self.log(f"# {datetime.now(timezone.utc).strftime('%Y-%m-%dT%H:%M:%SZ')}")
        self.log("# " + " ".join(sys.argv))
        self.log(f"# {platform.platform()}, {platform.machine()}, {os.cpu_count()} processors")
        self.log(f"manifest: {self.arguments.manifest} sha256 {sha256_of(self.arguments.manifest)}")
        self.log(f"rule: {self.rule['path']} sha256 {self.rule['sha256']}")
        self.log(f"rule population: {self.rule['population-line']}")
        self.log(f"control: {self.binaries['control']}")
        self.log(f"candidate: {self.binaries['candidate']}")
        self.log(f"shapes: {len(self.population)} in {self.folder}")
        self.log(
            "scrubbed from every child's environment: " + (", ".join(self.removed) or "nothing"))

        if self.arguments.rehearsal:
            self.log(
                "REHEARSAL: the manifest and the rule were not required to be committed, and no "
                "figure of this run may be retained by anything")

        self.choose_machine_probe()
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
        self.write_runs()
        self.write("runtime-reports.log", self.runtime_reports)
        self.summarise(self.quantities())
        self.write("harness.log", self.log_lines)
        self.say(f"# written to {self.directory}")
        self.say(
            "# NO FIGURE OF THIS RUN MAY LEAVE ITS BUNDLE. summary.txt and runs.jsonl are the only "
            "files carrying one.")

        return 0


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--control", required=True, metavar="DIR")
    parser.add_argument("--candidate", required=True, metavar="DIR")
    parser.add_argument("--shapes", default=str(DEFAULT_SHAPES), metavar="DIR")
    parser.add_argument("--manifest", required=True, metavar="FILE")
    parser.add_argument("--rule", required=True, metavar="FILE")
    parser.add_argument("--out", required=True, metavar="FILE")
    parser.add_argument("--repetitions", type=int, default=REQUIRED_REPETITIONS)
    parser.add_argument("--warmup", type=int, default=REQUIRED_WARMUP)

    # THE ALLOWANCES ARE STATED AND GENEROUS ON EVERY LANE. A default that bounds one form and not
    # the other turns a comparison into a report about the meter.
    parser.add_argument("--fuel", type=int, default=1_000_000_000_000)
    parser.add_argument("--wall", type=int, default=600_000)
    parser.add_argument("--backend", default="x86-64-win64", metavar="NAME")
    parser.add_argument("--fresh", action="store_true")
    parser.add_argument("--rehearsal", action="store_true")
    arguments = parser.parse_args()

    folder = pathlib.Path(arguments.shapes).resolve()

    if not folder.is_dir():
        refuse(f"no shapes at {folder}")

    shapes = sorted(folder.glob("*.js"))
    population = (
        [path for path in shapes if path.name == FRESH_SHAPE] if arguments.fresh
        else [path for path in shapes
              if not path.name.endswith(TWIN_SUFFIX) and path.name != FRESH_SHAPE])

    if not population:
        refuse(f"no shapes to measure under {folder}")

    for name, path in (("manifest", arguments.manifest), ("rule", arguments.rule)):
        if not pathlib.Path(path).is_file():
            refuse(f"no {name} at {path}")

    out = pathlib.Path(arguments.out).resolve()

    if arguments.rehearsal:
        if ROOT in out.parents:
            refuse(
                f"a rehearsal may not write into {ROOT}",
                "its outputs are not evidence and an evidence directory is the one place they must "
                "never be able to sit")
    else:
        for name, path in (("manifest", arguments.manifest), ("rule", arguments.rule),
                           *((f"shape {shape.name}", shape) for shape in shapes)):
            is_committed, is_clean = committed_and_clean(path)

            if not is_committed:
                refuse(
                    f"the {name} at {path} is not committed",
                    "rule 6 asks for a manifest written BEFORE either arm runs, and a file that is "
                    "not in a commit is not written before anything")

            if not is_clean:
                refuse(
                    f"the {name} at {path} has uncommitted changes",
                    "the run would be judged by a rule the repository does not hold")

    # A MANIFEST THAT DOES NOT PARSE PINS NOTHING, AND RULE 6 IS A PIN. It is refused in the same
    # voice as everything else here rather than raised: a harness whose answer to a truncated or
    # mistyped manifest is a stack trace is one whose refusals a reader cannot tell from its defects.
    try:
        manifest = json.loads(pathlib.Path(arguments.manifest).read_text(encoding="utf-8"))
    except (OSError, UnicodeDecodeError, json.JSONDecodeError) as failure:
        refuse(
            f"the manifest at {arguments.manifest} is not JSON this harness can read",
            f"{type(failure).__name__}: {failure}")

    if not isinstance(manifest, dict):
        refuse(
            f"the manifest at {arguments.manifest} is JSON but not an object, so it names no field")

    rule = read_rule(arguments.rule, population)
    environment, removed = scrubbed_environment()
    check_manifest(manifest, arguments.manifest, arguments, rule, shapes, folder, removed)

    # THE LANES RUN THROUGH THE SHAPES IN MANIFEST ORDER, which is what the manifest is for and what
    # a sorted directory listing only happens to be. `check_manifest` has just proved the two name
    # exactly the same files, so this reorders and can drop nothing; a shape the manifest does not
    # name never reaches here. The order matters no more than any other fixed order does - the
    # interleaving that carries the comparison is inside a shape - but it is now the committed
    # file's and not the file system's.
    order = list(manifest["shapes"])
    population.sort(key=lambda path: order.index(path.name))

    print("# broiler-js baseline-form granularity measurement")
    print(f"# {datetime.now(timezone.utc).strftime('%Y-%m-%dT%H:%M:%SZ')}")
    print(f"# {platform.platform()}, {platform.machine()}, {os.cpu_count()} logical processors")
    print(f"# bundle {manifest['bundle']}, {len(population)} shapes, "
          f"{arguments.warmup} warm-up + {arguments.repetitions} retained repetitions, interleaved")
    print(f"# fuel-credit position: {manifest['fuel-credit']}")
    print("#")
    print("# NO WALL-CLOCK FIGURE IS PRINTED HERE. The console says what the harness did; the files")
    print("# beside --out say what it measured, and rule L1 governs what may be done with those.")
    print("#")

    if arguments.rehearsal:
        print("# " + REHEARSAL_BANNER.lstrip("# "))
        print("#")

    return Harness(
        arguments, manifest, rule, population, folder, environment, removed).run()


if __name__ == "__main__":
    sys.exit(main())
