#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Compare numbered probes with retained answers and, optionally, a named engine.

Retained answers detect regressions; a second engine exposes differences, not a
conformance score. Authored #diverges <engine> <case> <reason> records are scoped
to that comparison engine. See src/tests/differential/README.md for the contract.
"""

import argparse
import datetime
import hashlib
import json
import math
import os
import pathlib
import platform
import re
import shutil
import signal
import subprocess
import sys
import tempfile
import time

ROOT = pathlib.Path(__file__).resolve().parent.parent
PROBES = ROOT / "src/tests/differential"
DEFAULT_BINARY_DIRECTORY = (
    ROOT / "src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0"
)
SHIM = ('if (typeof globalThis.print !== "function") globalThis.print = (...a) => '
        'console.log(a.map(x => String(x)).join(" "));\n')
ENVIRONMENT = {"TZ": "UTC", "PYTHONIOENCODING": "utf-8", "DOTNET_CLI_UI_LANGUAGE": "en-US"}
CASE = r"[0-9]+[a-z]*"  # Existing async probes include stable subcase IDs such as 39a/39b.


def lines(text):
    """Normalize transport newlines only; keep whitespace and Unicode separators."""
    result = text.replace("\r\n", "\n").split("\n")
    return result[:-1] if result[-1] == "" else result


def numbered(output):
    cases = {}
    for line in output:
        match = re.fullmatch(rf"({CASE}) (.*)", line)
        if not match:
            raise ValueError(f"non-case output: {line!r}")
        case, answer = match.groups()
        if case in cases:
            raise ValueError(f"duplicate case {case}")
        cases[case] = answer
    if not cases:
        raise ValueError("no numbered cases")
    return cases


def answers(path):
    if not path.exists():
        return None, {}
    retained, declared = [], {}
    for line in lines(path.read_text(encoding="utf-8")):
        if line.startswith("#diverges "):
            parts = line.split(" ", 3)
            if (len(parts) != 4 or not re.fullmatch(r"[a-z][a-z0-9-]*", parts[1])
                    or not re.fullmatch(CASE, parts[2]) or not parts[3].strip()):
                raise ValueError(f"{path.name}: use #diverges <engine> <case> <reason>")
            _, engine, case, reason = parts
            key = (engine, case)
            if key in declared:
                raise ValueError(f"{path.name}: duplicate divergence {engine}/{case}")
            declared[key] = reason
        elif not line.startswith("#"):
            retained.append(line)
    return retained, declared


def engine_config(path):
    config = json.loads(path.read_text(encoding="utf-8"))
    if not isinstance(config, dict):
        raise ValueError("engine configuration must be a JSON object")
    unknown = config.keys() - {"name", "executable", "scriptArgs", "moduleArgs", "sourceRoot"}
    if unknown:
        raise ValueError(f"unknown engine configuration fields: {sorted(unknown)}")
    if not isinstance(config.get("name"), str) or not re.fullmatch(r"[a-z][a-z0-9-]*", config["name"]):
        raise ValueError("engine name must use lower-case letters, digits and hyphens")
    if not isinstance(config.get("executable"), str) or not config["executable"]:
        raise ValueError("engine executable is required")
    for goal in ("scriptArgs", "moduleArgs"):
        args = config.get(goal)
        if (not isinstance(args, list) or not all(isinstance(arg, str) for arg in args)
                or "{probe}" not in args):
            raise ValueError(f"{goal} must be an argv array containing the token {{probe}}")
    for key in ("executable", "sourceRoot"):
        value = config.get(key)
        if value is not None:
            if not isinstance(value, str):
                raise ValueError(f"{key} must be a string")
            if key == "sourceRoot" or "/" in value or "\\" in value:
                config[key] = str((path.parent / value).resolve())
    return config


def digest(path):
    return {"path": str(path), "sha256": hashlib.sha256(path.read_bytes()).hexdigest()}


def identify(config):
    """Hash the executable and local assemblies, including dotnet-hosted payloads."""
    identity = dict(config)
    resolved = shutil.which(config["executable"])
    if resolved is None:
        identity["error"] = f"engine executable not found: {config['executable']}"
        return identity
    identity["resolvedExecutable"] = str(pathlib.Path(resolved).resolve())
    binaries = {pathlib.Path(identity["resolvedExecutable"])}
    for arg in config["scriptArgs"] + config["moduleArgs"]:
        if arg != "{probe}" and pathlib.Path(arg).is_file():
            binaries.add(pathlib.Path(arg).resolve())
    for goal in ("scriptArgs", "moduleArgs"):
        identity[goal] = [str(pathlib.Path(arg).resolve()) if pathlib.Path(arg).is_file() else arg
                          for arg in config[goal]]
    # Apphosts alone do not identify a .NET engine: include its managed payload.
    for binary in list(binaries):
        binaries.update(binary.parent.glob("Broiler*.dll"))
    identity["binaries"] = [digest(p) for p in sorted(binaries)]
    if config.get("sourceRoot"):
        identity["source"] = {}
        for label, args in (("revision", ["rev-parse", "HEAD"]),
                            ("workingTree", ["status", "--porcelain=v1"])):
            done = subprocess.run(["git", "-C", config["sourceRoot"], *args],
                                  capture_output=True, timeout=10, encoding="utf-8")
            if done.returncode:
                raise ValueError(f"cannot identify source: {done.stderr.strip()}")
            identity["source"][label] = done.stdout.strip()
    return identity


def stop(process):
    """Terminate the timed-out engine's process tree without shell interpolation."""
    if os.name == "nt":
        try:
            subprocess.run(["taskkill", "/PID", str(process.pid), "/T", "/F"],
                           capture_output=True, timeout=10)
        finally:
            if process.poll() is None:
                process.kill()
    else:
        try:
            os.killpg(process.pid, signal.SIGKILL)
        except ProcessLookupError:
            pass
    if process.poll() is None:
        process.kill()


def execute(engine, probe, timeout, shim=None):
    goal = "module" if probe.suffix == ".mjs" else "script"
    tokens = {"{probe}": str(probe), "{shim}": str(shim)}
    argv = [engine["resolvedExecutable"], *[
        tokens.get(arg, arg) for arg in engine[goal + "Args"]]]
    result = {"engine": engine["name"], "goal": goal, "argv": argv,
              "cwd": str(probe.parent), "timeoutSeconds": timeout,
              "exitCode": None, "timedOut": False, "stdout": "", "stderr": ""}
    started = time.monotonic()
    try:
        with subprocess.Popen(argv, cwd=probe.parent, stdout=subprocess.PIPE,
                              stderr=subprocess.PIPE, env={**os.environ, **ENVIRONMENT},
                              start_new_session=os.name != "nt") as process:
            try:
                stdout, stderr = process.communicate(timeout=timeout)
            except subprocess.TimeoutExpired:
                result["timedOut"] = True
                stop(process)
                stdout, stderr = process.communicate(timeout=10)
            result["exitCode"] = process.returncode
            for key, data in (("stdout", stdout), ("stderr", stderr)):
                try:
                    result[key] = data.decode("utf-8")
                except UnicodeDecodeError:
                    result[key] = data.decode("utf-8", errors="backslashreplace")
                    result["error"] = "invalid UTF-8 output"
    except (OSError, subprocess.SubprocessError) as error:
        result["error"] = str(error)
    result["elapsedSeconds"] = round(time.monotonic() - started, 3)
    return result


def checked_output(result):
    if result.get("error"):
        raise ValueError(result["error"])
    if result["timedOut"]:
        raise ValueError(f"{result['engine']} timed out after {result['timeoutSeconds']}s")
    if result["exitCode"] != 0:
        raise ValueError(f"{result['engine']} exited {result['exitCode']}: {result['stderr']!r}")
    if result["stderr"]:
        raise ValueError(f"{result['engine']} wrote stderr: {result['stderr']!r}")
    output = lines(result["stdout"])
    numbered(output)
    return output


def compare(mine, theirs, declared, engine):
    failures, accepted = [], []
    if mine.keys() == theirs.keys() and list(mine) != list(theirs):
        failures.append("comparison case order differs from host output")
    selected = {case: reason for (name, case), reason in declared.items() if name == engine}
    def case_order(case):
        number, suffix = re.fullmatch(r"([0-9]+)([a-z]*)", case).groups()
        return int(number), suffix

    for case in sorted(mine.keys() | theirs.keys() | selected.keys(), key=case_order):
        if case not in mine or case not in theirs:
            failures.append(f"case {case} missing: host={mine.get(case)!r}, {engine}={theirs.get(case)!r}"
                            + ("; stale/unverifiable declaration" if case in selected else ""))
        elif mine[case] == theirs[case]:
            if case in selected:
                failures.append(f"stale divergence {engine}/{case}: both answered {mine[case]!r}")
        elif case in selected:
            accepted.append({"case": case, "reason": selected[case],
                             "host": mine[case], "comparison": theirs[case]})
        else:
            failures.append(f"undeclared divergence {engine}/{case}: host={mine[case]!r}, "
                            f"comparison={theirs[case]!r}")
    return failures, accepted


def retain(path, produced):
    # Preserve authored comments/declarations as text; never infer exemptions.
    comments = ([line for line in lines(path.read_text(encoding="utf-8")) if line.startswith("#")]
                if path.exists() else ["# Retained host answers; not a conformance oracle."])
    path.write_text("\n".join(comments + produced) + "\n", encoding="utf-8")


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--binary-directory", type=pathlib.Path, default=DEFAULT_BINARY_DIRECTORY)
    parser.add_argument("--host-config", type=pathlib.Path)
    other = parser.add_mutually_exclusive_group()
    other.add_argument("--against", help="comparison executable; Node unless --against-kind is set")
    other.add_argument("--against-config", type=pathlib.Path)
    parser.add_argument("--against-kind", choices=("node", "broiler-js"), default="node")
    parser.add_argument("--against-root", type=pathlib.Path, help="source checkout to identify")
    parser.add_argument("--probe-directory", type=pathlib.Path, default=PROBES)
    parser.add_argument("--timeout", type=float, default=30, help="seconds per engine/probe")
    parser.add_argument("--report", type=pathlib.Path, default=ROOT / "artifacts/differential/report.json")
    parser.add_argument("--write", action="store_true", help="deliberately update retained host answers")
    parser.add_argument("--only", help="one probe stem")
    args = parser.parse_args(argv)
    if not math.isfinite(args.timeout) or args.timeout <= 0:
        parser.error("--timeout must be finite and positive")
    if args.write and (args.against or args.against_config):
        parser.error("--write cannot be combined with a comparison run")
    report = {"schemaVersion": 1, "capturedUtc": datetime.datetime.now(datetime.timezone.utc).isoformat(),
              "platform": platform.platform(), "python": platform.python_version(),
              "driver": digest(pathlib.Path(__file__).resolve()), "environment": ENVIRONMENT,
              "engines": [], "probes": [], "failures": [], "ok": False}
    try:
        suffix = ".exe" if os.name == "nt" else ""
        host = (engine_config(args.host_config.resolve()) if args.host_config else {
            "name": "broiler-vm", "executable": str((args.binary_directory /
                ("Broiler.VM.Composition.JavaScript.Cli" + suffix)).resolve()),
            "scriptArgs": ["--quiet", "{probe}"],
            "moduleArgs": ["--quiet", "--module", "{probe}"], "sourceRoot": str(ROOT)})
        configs = [host]
        if args.against_config:
            configs.append(engine_config(args.against_config.resolve()))
        elif args.against:
            configs.append({"name": args.against_kind, "executable": args.against,
                            "scriptArgs": (["--script-host", "{probe}"] if args.against_kind == "broiler-js"
                                           else ["--require", "{shim}", "{probe}"]),
                            "moduleArgs": (["--module-host", "{probe}"] if args.against_kind == "broiler-js"
                                           else ["--require", "{shim}", "{probe}"])})
        if args.against_root:
            if len(configs) != 2:
                raise ValueError("--against-root needs a comparison engine")
            configs[1]["sourceRoot"] = str(args.against_root.resolve())
        report["engines"] = [identify(config) for config in configs]
        for engine in report["engines"]:
            if engine.get("error"):
                report["failures"].append(engine["error"])
        if report["failures"]:
            raise ValueError("engine setup failed")
        probes = sorted(p for p in args.probe_directory.resolve().iterdir()
                        if p.suffix in (".js", ".mjs") and p.is_file()
                        and (not args.only or p.stem == args.only))
        if not probes:
            raise ValueError(f"no probes under {args.probe_directory} matching {args.only!r}")
        for probe in probes:
            entry = {"name": probe.name, "input": digest(probe), "runs": [],
                     "failures": [], "declaredDivergences": []}
            report["probes"].append(entry)
            print(f"--- {probe.name}")
            try:
                expected = probe.with_suffix(".expected.txt")
                retained, declared = answers(expected)
                if expected.exists():
                    entry["retainedInput"] = digest(expected)
                dependencies = probe.parent / "modules"
                entry["dependencies"] = ([digest(p) for p in sorted(dependencies.rglob("*")) if p.is_file()]
                                         if dependencies.is_dir() else [])
                # Engines read the original file, preserving directive prologues and module
                # resolution. Hosts without print may explicitly preload the separate shim.
                with tempfile.TemporaryDirectory(prefix="broiler-differential-") as scratch:
                    shim = pathlib.Path(scratch) / "print.cjs"
                    shim.write_text(SHIM, encoding="utf-8")
                    outputs = []
                    for engine in report["engines"]:
                        run = execute(engine, probe, args.timeout, shim)
                        entry["runs"].append(run)
                        outputs.append(checked_output(run))
                produced = outputs[0]
                if args.write:
                    retain(expected, produced)
                    entry["writtenAnswers"] = digest(expected)
                    continue
                if retained is None:
                    entry["failures"].append(f"no retained answers: {expected.name}")
                elif retained != produced:
                    entry["failures"].append("host output differs from retained answers")
                    entry["retainedAnswers"] = retained
                if len(report["engines"]) == 2:
                    failures, accepted = compare(numbered(produced), numbered(outputs[1]), declared,
                                                 report["engines"][1]["name"])
                    entry["failures"].extend(failures)
                    entry["declaredDivergences"] = accepted
                    for divergence in accepted:
                        print(f"    declared {divergence['case']}: {divergence['reason']}")
            except (OSError, ValueError, subprocess.SubprocessError) as error:
                entry["failures"].append(str(error))
            for failure in entry["failures"]:
                print(f"    FAIL: {failure}")
        report["ok"] = not any(p["failures"] for p in report["probes"])
    except (OSError, ValueError, subprocess.SubprocessError) as error:
        report["failures"].append(str(error))
    for failure in report["failures"]:
        print(f"FAIL: {failure}")
    args.report.parent.mkdir(parents=True, exist_ok=True)
    args.report.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"Report: {args.report}")
    return 0 if report["ok"] else 1


if __name__ == "__main__":
    # Diagnostics must also survive redirected Windows consoles with an ANSI code page.
    sys.stdout.reconfigure(encoding="utf-8", errors="backslashreplace")
    sys.stderr.reconfigure(encoding="utf-8", errors="backslashreplace")
    sys.exit(main())
