#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""The revision check of bundle sjl-3-001's part C: every workload's answer and to-string.js's texts, at both arms,
before a revised rule is written. Not evidence, and nothing is timed.

    python3 docs/evidence/sjl-3-001/revision-check/revision-check.py --tree <a clean checkout of arm A's sources> \
        --base-tree <a clean checkout of 0087e8d73ae3833200fa263b68f71db15b059a9c> \
        --numref <Broiler.JS scripts/conformance/numref.py at 966140ad> --out docs/evidence/sjl-3-001/revision-check

It builds the command-line composition root in Release in each tree, as part C does; runs each of the six files of
src/tests/forms/numbers (read from --tree; the base commit predates them) once per arm and form with --runtime and
part C's allowances; runs transcript/to-string.transcript.js once per arm and form; and holds the transcripts to the
check of transcript/check.py. It writes:

  identity.txt          both arms' commits and binaries, the reference, the program, the SDK and the machine;
  answers.tsv           arm, form, file, exit and answer, beside bundle sjl-0-001's base reading;
  configuration.txt     each arm's effective-configuration lines;
  transcript-check.txt  the transcript check, per form and conversion;
  gate.txt              the gate the revised rule may be written under: at arm A, to-number.js, literals.js and every
                        control answer as at the base reading; one configuration line across both arms; and the
                        transcript check holds in both forms.

Exit 0 when the gate holds, 1 when it does not, 2 when an input is refused.
"""

import argparse
import datetime
import gzip
import hashlib
import importlib.util
import os
import platform
import shutil
import subprocess
import sys
import tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
BUNDLE = os.path.dirname(HERE)
EVIDENCE = os.path.dirname(BUNDLE)
BASE_COMMIT = "0087e8d73ae3833200fa263b68f71db15b059a9c"
PREVIEW_7 = "aaa39fbc2030e1a2554317bea75bcbb56041c696"
FORMS = ("bytecode", "native")


def load(name, path):
    spec = importlib.util.spec_from_file_location(name, path)
    module = importlib.util.module_from_spec(spec)
    sys.modules[name] = module
    spec.loader.exec_module(module)
    return module


base = load("sjl_0_001_collect", os.path.join(EVIDENCE, "sjl-0-001", "collect.py"))
transcript = load("sjl_3_001_transcript_check", os.path.join(BUNDLE, "transcript", "check.py"))
PROGRAM = os.path.join(BUNDLE, "transcript", "to-string.transcript.js")


def git(tree, *arguments):
    done = subprocess.run(["git"] + list(arguments), cwd=tree, capture_output=True, encoding="utf-8", errors="replace")
    return done.returncode, (done.stdout or "").strip()


def sha256_lf(path):
    with open(path, "rb") as stream:
        return hashlib.sha256(stream.read().replace(b"\r\n", b"\n")).hexdigest()


def sha256(path):
    with open(path, "rb") as stream:
        return hashlib.sha256(stream.read()).hexdigest()


def base_reading():
    """Bundle sjl-0-001's base reading: each file's first answer per form."""
    answers = {}
    for line in open(os.path.join(EVIDENCE, "sjl-0-001", "measurement", "conditions.tsv"), encoding="utf-8"):
        cells = line.rstrip("\n").split("\t")
        if len(cells) == 6 and cells[3] == "before":
            answers.setdefault((cells[0], cells[2]), cells[5])
    return answers


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--tree", required=True)
    parser.add_argument("--base-tree", required=True)
    parser.add_argument("--numref", required=True)
    parser.add_argument("--out", required=True)
    arguments = parser.parse_args()
    trees = {"A": os.path.abspath(arguments.tree), "B": os.path.abspath(arguments.base_tree)}
    numref_path = os.path.abspath(arguments.numref)
    out = os.path.abspath(arguments.out)
    scratch = tempfile.mkdtemp(prefix="sjl-3-001-revision-check-")

    def hide(text):
        for path, name in ((scratch, "<scratch>"), (numref_path, "<numref>"), (trees["B"], "<base-tree>"),
                           (trees["A"], "<tree>")):
            text = text.replace(path, name)
        return text

    def write(name, lines):
        with open(os.path.join(out, name), "w", encoding="utf-8", newline="\n") as stream:
            stream.write(hide("\n".join(lines) + "\n"))

    problems = []
    try:
        numref = transcript.load_numref(numref_path)
    except transcript.Refused as refusal:
        problems.append(str(refusal))
        numref = None
    commits = {arm: git(trees[arm], "rev-parse", "HEAD")[1] for arm in ("A", "B")}
    for arm in ("A", "B"):
        if git(trees[arm], "status", "--porcelain")[1]:
            problems.append("arm %s: the tree is not clean" % arm)
    if commits["B"] != BASE_COMMIT:
        problems.append("arm B: the tree is at %s, not bundle sjl-0-001's base commit %s" % (commits["B"], BASE_COMMIT))
    folder = os.path.join(trees["A"], base.WORKLOADS)
    loops = {"the workload to-string.js": os.path.join(folder, "to-string.js"), "the transcript program": PROGRAM}
    for what, path in loops.items():
        if transcript.loop_of(open(path, encoding="utf-8").read()) != transcript.expected_loop():
            problems.append("%s's loop is not the checker's (values, bound, scale and conversions)" % what)
    for problem in problems:
        print("revision-check: REFUSED: %s" % hide(problem), flush=True)
    if problems:
        return 2
    os.makedirs(out, exist_ok=True)

    binaries = {}
    builds = []
    for arm in ("B", "A"):
        tree = trees[arm]
        project = os.path.join(tree, base.CLI, os.path.basename(base.CLI) + ".csproj")
        done = subprocess.run(["dotnet", "build", project, "-c", "Release", "-nodeReuse:false",
                               "-p:UseSharedCompilation=false"], cwd=tree, capture_output=True, encoding="utf-8",
                              errors="replace", env=dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en",
                                                         DOTNET_CLI_TELEMETRY_OPTOUT="1"))
        log = os.path.join(scratch, "build-cli-%s.log" % arm.lower())
        with open(log, "w", encoding="utf-8") as stream:
            stream.write((done.stdout or "") + (done.stderr or ""))
        warnings = [line for line in (done.stdout or "").splitlines() if ": warning " in line]
        builds.append("build arm %s: exit %d, %d warning lines" % (arm, done.returncode, len(warnings)))
        print("revision-check: build arm %s exit %d" % (arm, done.returncode), flush=True)
        if done.returncode != 0:
            print(hide((done.stdout or "")[-4000:]))
            return 1
        copy = os.path.join(scratch, "cli-" + arm)
        shutil.copytree(os.path.join(tree, base.CLI, "bin", "Release", "net10.0"), copy)
        binaries[arm] = os.path.join(copy, base.CLI_BINARY)

    environment = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en")

    def child(arm, form, path):
        command = [binaries[arm], "--runtime"]
        if form == "native":
            command += ["--native", base.NATIVE_BACKEND]
        command += ["--fuel", str(base.MEASURE_FUEL), "--wall", str(base.MEASURE_WALL), path]
        done = subprocess.run(command, capture_output=True, encoding="utf-8", errors="replace", env=environment)
        runtime = next((line.strip() for line in (done.stderr or "").splitlines()
                        if line.startswith(base.RUNTIME_PREFIX)), "(none)")
        return done.returncode, done.stdout or "", runtime

    reading = base_reading()
    answers = {}
    configurations = {"A": [], "B": []}
    rows = ["arm\tform\tfile\texit\tanswer\tbase reading (sjl-0-001)\tequals the base reading"]
    for arm in ("B", "A"):
        for form in FORMS:
            for stem in base.WORKLOAD_STEMS:
                for name in (stem + ".js", stem + ".control.js"):
                    code, output, runtime = child(arm, form, os.path.join(folder, name))
                    lines = output.strip().splitlines()
                    answer = lines[-1] if lines else ""
                    answers[(arm, form, name)] = (code, answer)
                    if runtime not in configurations[arm]:
                        configurations[arm].append(runtime)
                    expected = reading.get((form, name))
                    rows.append("\t".join([arm, form, name, str(code), answer, expected or "(none)",
                                           "yes" if code == 0 and answer == expected else "no"]))
            print("revision-check: answers arm %s %s" % (arm, form), flush=True)

    transcripts = {}
    texts = []
    for arm in ("B", "A"):
        for form in FORMS:
            code, output, runtime = child(arm, form, PROGRAM)
            if runtime not in configurations[arm]:
                configurations[arm].append(runtime)
            transcripts[(arm, form)] = output if code == 0 else None
            path = os.path.join(scratch, "transcript-%s-%s.txt.gz" % (arm.lower(), form))
            with gzip.GzipFile(path, "wb", compresslevel=9, mtime=0) as stream:
                stream.write(output.encode("utf-8"))
            texts.append("transcript arm %s %s: exit %d, %d bytes, sha256=%s" % (
                arm, form, code, len(output.encode("utf-8")), hashlib.sha256(output.encode("utf-8")).hexdigest()))
            print("revision-check: transcript arm %s %s exit %d" % (arm, form, code), flush=True)

    reference = transcript.Reference(numref)
    summary = ["# The transcript check of decision-rule-revised-2026-10-20.md, taken by the revision check before that"
               " rule was written:",
               "# transcript/to-string.transcript.js run once per arm and form; (i) and (ii) tie each transcript to the"
               " answers in answers.tsv,",
               "# (iii) holds arm A's texts to Broiler.JS's numref.py at %s." % transcript.NUMREF_COMMIT, ""]
    summary += texts + [""]
    differing = ["form\tline\ti\tj\tvalue-bits\tvalue\tconversion\tarm A's text\tarm B's text"]
    check_holds = {}
    for form in FORMS:
        holds, lines, tsv = transcript.check(
            reference, form, {"A": transcripts[("A", form)], "B": transcripts[("B", form)]},
            answers[("B", form, "to-string.control.js")][1],
            {"A": answers[("A", form, "to-string.js")][1], "B": answers[("B", form, "to-string.js")][1]})
        check_holds[form] = holds
        summary += lines + [""]
        with open(os.path.join(scratch, "transcript-check-%s.tsv" % form), "w", encoding="utf-8") as stream:
            stream.write("\n".join([transcript.TSV_HEADER] + tsv) + "\n")
        print("revision-check: transcript check %s %s" % (form, "holds" if holds else "DOES NOT HOLD"), flush=True)

    gate = ["# The gate of task 1.1: the revised rule is written only if every item holds.", ""]
    first = True
    for form in FORMS:
        for name in ("to-number.js", "literals.js", "to-string.control.js", "to-number.control.js",
                     "literals.control.js"):
            code, answer = answers[("A", form, name)]
            expected = reading.get((form, name))
            ok = code == 0 and answer == expected
            first = first and ok
            gate.append("arm A %s %s: exit %d, answer %s, base reading %s: %s" % (
                form, name, code, answer, expected, "as at the base reading" if ok else "DIFFERS"))
    gate.append("(a) at arm A, to-number.js, literals.js and every control answer as at the base reading: %s"
                % ("holds" if first else "FAILS"))
    lines_seen = configurations["A"] + [line for line in configurations["B"] if line not in configurations["A"]]
    second = len(lines_seen) == 1 and lines_seen[0] != "(none)"
    gate.append("(b) one effective-configuration line across both arms: %s (%d distinct)"
                % ("holds" if second else "FAILS", len(lines_seen)))
    third = all(check_holds.values())
    gate.append("(c) the transcript check holds in both forms: %s (%s)" % (
        "holds" if third else "FAILS", ", ".join("%s %s" % (form, "holds" if check_holds[form] else "fails")
                                                  for form in FORMS)))
    for form in FORMS:
        code, answer = answers[("A", form, "to-string.js")]
        gate.append("not gated: arm A %s to-string.js answers %s, against %s at the base reading; the revised"
                    " condition takes arm A's own answer" % (form, answer, reading.get((form, "to-string.js"))))
    holds = first and second and third
    gate += ["", "gate: %s" % ("HOLDS" if holds else "DOES NOT HOLD")]

    configuration = ["# Each arm's effective-configuration lines (--runtime), over its twelve answers and its two"
                     " transcripts"]
    for arm in ("B", "A"):
        configuration.append("arm %s: %d distinct" % (arm, len(configurations[arm])))
        configuration.extend("  " + line for line in configurations[arm])
    configuration.append("identical across both arms: %s" % ("yes" if second else "NO"))

    sdk = subprocess.run(["dotnet", "--version"], capture_output=True, encoding="utf-8").stdout.strip()
    identity = [
        "revision check of bundle sjl-3-001's part C, taken before decision-rule-revised-2026-10-20.md was written",
        "taken " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
        "arm-B-commit %s (bundle sjl-0-001's base commit; clean tree)" % commits["B"],
        "arm-A-commit %s (clean tree; the default branch, whose product sources are %s's but for: %s)" % (
            commits["A"], PREVIEW_7, ", ".join(
                name for name in git(trees["A"], "diff", "--name-only", PREVIEW_7, commits["A"])[1].splitlines()
                if name and not name.startswith("docs/") and "/docs/" not in name) or "none"),
    ]
    identity += builds
    for arm in ("B", "A"):
        identity.append("binary arm %s %s.dll sha256=%s" % (arm, base.CLI_BINARY, sha256(binaries[arm] + ".dll")))
    identity += [
        "build dotnet build <root> -c Release -nodeReuse:false -p:UseSharedCompilation=false, root %s"
        % base.CLI.replace(os.sep, "/"),
        "child <arm>/%s --runtime [--native %s] --fuel %d --wall %d <file>"
        % (base.CLI_BINARY, base.NATIVE_BACKEND, base.MEASURE_FUEL, base.MEASURE_WALL),
        "workloads %s, read from arm A's tree" % base.WORKLOADS.replace(os.sep, "/"),
    ]
    for stem in base.WORKLOAD_STEMS:
        for name in (stem + ".js", stem + ".control.js"):
            path = (base.WORKLOADS + "/" + name).replace(os.sep, "/")
            identity.append("workload %s sha256=%s blob=%s" % (
                name, sha256_lf(os.path.join(folder, name)), git(trees["A"], "rev-parse", "HEAD:" + path)[1]))
    identity += [
        "program transcript/to-string.transcript.js sha256=%s" % sha256_lf(PROGRAM),
        "checker transcript/check.py sha256=%s" % sha256_lf(os.path.join(BUNDLE, "transcript", "check.py")),
        "numref scripts/conformance/numref.py at Broiler.JS %s sha256=%s" % (transcript.NUMREF_COMMIT,
                                                                             sha256(numref_path)),
        "sdk " + sdk,
        "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
        "not retained: the build logs, the binaries, the four transcripts and the per-line listing of the check, which"
        " the command above writes again",
    ]
    write("identity.txt", identity)
    write("answers.tsv", rows)
    write("configuration.txt", configuration)
    write("transcript-check.txt", summary)
    write("gate.txt", gate)
    print("revision-check: gate %s; outputs in %s; scratch %s" % ("HOLDS" if holds else "DOES NOT HOLD", out, scratch),
          flush=True)
    return 0 if holds else 1


if __name__ == "__main__":
    sys.exit(main())
