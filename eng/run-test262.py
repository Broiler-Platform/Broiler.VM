#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# TAKE A WHOLE RUN OF THE PINNED TEST262 SUITE, UNDER ONE NAMED MANIFEST, AND RETAIN EVERYTHING IT
# DID.
#
# The workload roadmap's JSW-10 asks for a whole-suite run per manifest, retained under the evidence
# ledger's section 4 contract, with the four verdicts reported and every `unsupported` family named -
# and the run reports a fifth, because a variant that spent an allowance is neither a pass, a
# failure, a construct outside the manifest nor a skip, and folding it into any of the four would
# hide the one outcome a reader can fix by raising a number. A whole run of the pinned checkout is
# tens of thousands of files and one process will not finish it in a useful time, so a whole run is
# `n` processes and a merge. This script is that command line.
#
#   * it CHECKS the checkout against `src/tests/conformance/pins/test262.pin` before it starts a
#     single process, the way `run-octane.py` checks an archive against its own pin, so a run is
#     against the revision the repository decided rather than against whatever is on the disk;
#   * it passes `--expect` to every shard as well, so that no shard's report can name a revision the
#     shard did not itself read - a report that certified its own input would be the thing the
#     retained pin exists to replace;
#   * it fans the shards out across processes and merges the shard reports into one whole-run report;
#   * and it retains every line of every shard, failures included, rather than the passing half.
#
# IT PUBLISHES NO FIGURE OF ITS OWN. Every number printed at the end is the merged report's, produced
# by the harness and read back off the merge; this script adds no total, computes no rate, and
# compares nothing against a baseline or an earlier run. Roadmap section 17 governs any figure that
# is ever retained, and the ratchet is `--floor`'s and not this script's.
#
# THE ALLOWANCES, AND WHY THEY ARE SMALL HERE. A variant gets a fuel ceiling and a wall-clock
# ceiling, and both are allowances this script states rather than measurements it took. Fuel is the
# one meant to decide: it is charged per instruction, so the same test decides the same way on a busy
# machine and on an idle one, which is the property a retained run needs. The wall clock is the
# backstop for a variant that stalls without spending fuel.
#
# Both are far smaller than the mode's own defaults, and the mode's are right for what they are for:
# somebody pointing the harness at ONE test wants the answer rather than the ceiling. Over fifty
# thousand files neither is affordable - a minute a variant, and a fuel ceiling whose exhaustion
# takes over a minute to reach, is what turns a subtree into half an hour - so the figures here are
# chosen together: the fuel ceiling is sized to be reached at about the same point as the five-second
# wall clock, so that the DETERMINISTIC ceiling is normally the one that bites and the wall clock
# only catches what fuel cannot see. A conformance test that has not answered in seconds is not a
# test this engine is about to pass by waiting.
#
# AND A VARIANT THAT MEETS EITHER CEILING IS AN EXHAUSTION, WHICH IS ITS OWN VERDICT IN ITS OWN
# COLUMN. It is never folded into the failures: an absence, a refusal, a failure and a ceiling are
# four different answers, and a failed column that silently carried "we did not wait long enough" is
# a column nobody can act on. The merged report names every exhausted variant and the dimension it
# spent, so raising a ceiling later is a decision a reader takes on the transcript rather than a
# number that vanished into "fail".
#
# THE EXIT CODE IS THE MERGE'S, so it says what the harness said: 0 where nothing failed, 1 where
# cases failed or the merged run was misconfigured, and this script stops before the merge where any
# shard reported the harness's own defect code, because a shard that could not measure has totals
# nobody may add. A whole run of this suite fails cases today, so 1 is the ordinary outcome and not a
# reason to discard the transcript.
#
#   python3 eng/run-test262.py --suite <root> [--binary-directory <dir>] [--manifest <id>]
#                              [--decline <surface>]... [--jobs <n>] [--shards <n>]
#                              [--fuel <n>] [--wall <ms>] [--out <dir>] [--dir <subtree>]...

import argparse
import concurrent.futures
import hashlib
import os
import pathlib
import subprocess
import sys

ROOT = pathlib.Path(__file__).resolve().parent.parent
PIN = ROOT / "src/tests/conformance/pins/test262.pin"
DEFAULT_BINARY_DIRECTORY = (
    ROOT / "src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0"
)
BINARY_NAME = "Broiler.VM.Composition.JavaScript.Conformance"

# The instruction allowance one variant gets. Deterministic, so a retained run says the same thing
# twice, and sized to run out at about the same point as the wall clock below rather than a minute
# after it - which is what made the mode's own two-billion default unusable over a whole suite. It is
# still two orders of magnitude above what a test262 file plus the suite's harness prelude spends, so
# a variant that reaches it is one that is not finishing.
DEFAULT_FUEL = 100_000_000

# The wall-clock backstop, in milliseconds. See the header: this is the figure that makes a whole run
# takeable, and it is stated rather than inherited.
DEFAULT_WALL = 5_000


def pinned():
    """The retained pin's fields, or a failure. The authority is here and not in the checkout."""
    if not PIN.exists():
        raise SystemExit(f"# no pin at {PIN}")

    fields = {}

    for line in PIN.read_text().splitlines():
        line = line.strip()

        if not line or line.startswith("#"):
            continue

        key, _, value = line.partition(" ")
        fields[key] = value.strip()

    for required in ("suite", "upstream", "revision", "content-sha256", "files"):
        if required not in fields:
            raise SystemExit(f"# {PIN} declares no `{required}`")

    return fields


def identify(suite, fields):
    """Refuses a checkout that is not the pinned revision. A disagreement is refused, never fixed."""
    rows = []

    for path in sorted(suite.rglob("*")):
        if not path.is_file():
            continue

        # The suite's own `suite.pin`, if somebody generated one inside the checkout, is not part of
        # what the retained pin is over: a digest that included it would be a function of itself.
        relative = path.relative_to(suite).as_posix()

        if relative == "suite.pin":
            continue

        rows.append((relative, hashlib.sha256(path.read_bytes()).hexdigest()))

    content = hashlib.sha256()

    for relative, digest in sorted(rows):
        content.update(f"{relative}\n{digest}\n".encode())

    observed = content.hexdigest()
    declared = fields["content-sha256"]

    if len(rows) != int(fields["files"]):
        raise SystemExit(
            f"# the checkout holds {len(rows)} files and the pin says {fields['files']}: "
            f"this is not {fields['upstream']} at {fields['revision']}"
        )

    if observed != declared:
        raise SystemExit(
            f"# the checkout's contents do not hash to what the pin says\n"
            f"#   pin      {declared}\n#   checkout {observed}"
        )

    return observed


def shard(binary, suite, arguments, index, count, reports, logs):
    """One shard, one process. Its whole transcript is retained whatever it exited with."""
    report = reports / f"shard-{index:04d}.report"
    command = [
        str(binary),
        "--test262", str(suite),
        "--manifest", arguments.manifest,
        "--shard", f"{index}/{count}",
        "--expect", str(PIN),
        "--report", str(report),
        "--fuel", str(arguments.fuel),
        "--wall", str(arguments.wall),
    ]

    for surface in arguments.decline:
        command += ["--decline", surface]

    for subtree in arguments.dir:
        command += ["--dir", subtree]

    done = subprocess.run(command, cwd=str(ROOT), capture_output=True, text=True)
    transcript = logs / f"shard-{index:04d}.log"
    transcript.write_text(done.stdout + done.stderr)
    return index, done.returncode, transcript


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--suite", required=True, help="an unpacked checkout of the pinned suite")
    parser.add_argument("--binary-directory", default=str(DEFAULT_BINARY_DIRECTORY))
    parser.add_argument("--manifest", default="broiler.javascript.wide")
    parser.add_argument("--decline", action="append", default=[])
    parser.add_argument("--dir", action="append", default=[], help="a subtree; a run naming one is partial")
    parser.add_argument("--fuel", type=int, default=DEFAULT_FUEL)
    parser.add_argument("--wall", type=int, default=DEFAULT_WALL)

    # THE MACHINE'S PROCESSORS LESS TWO, so a run of several hours leaves the machine usable and
    # leaves the harness's own processes somewhere to run. It is a default and not a policy.
    parser.add_argument("--jobs", type=int, default=max(1, (os.cpu_count() or 3) - 2))

    # MORE SHARDS THAN JOBS ON PURPOSE. The partition is a hash of the path, so the shards are of
    # similar SIZE and not of similar COST - one shard holding a handful of variants that sit against
    # the wall clock finishes long after its siblings. Cutting finer than the pool is wide lets the
    # pool refill instead of idling, and costs one extra suite verification per shard.
    parser.add_argument("--shards", type=int, default=0)
    parser.add_argument("--out", default=None, help="where the transcript is retained")
    arguments = parser.parse_args()

    binary = pathlib.Path(arguments.binary_directory) / BINARY_NAME

    # The published image carries a suffix on Windows and not on the other two claimed platforms,
    # and a driver that assumed either would leave one cell of the matrix unable to run at all.
    if not binary.exists() and binary.with_suffix(".exe").exists():
        binary = binary.with_suffix(".exe")

    if not binary.exists():
        raise SystemExit(f"# no binary at {binary}")

    suite = pathlib.Path(arguments.suite).resolve()

    if not (suite / "harness").is_dir():
        raise SystemExit(f"# {suite} has no harness/ directory, so it is not a test262 checkout")

    fields = pinned()
    digest = identify(suite, fields)

    jobs = max(1, arguments.jobs)
    shards = arguments.shards if arguments.shards > 0 else jobs * 4

    out = pathlib.Path(arguments.out) if arguments.out else ROOT / "artifacts/test262"
    reports = out / "shards"
    logs = out / "logs"
    reports.mkdir(parents=True, exist_ok=True)
    logs.mkdir(parents=True, exist_ok=True)

    # AN EARLIER RUN'S SHARDS ARE REMOVED BEFORE THIS ONE STARTS. The merge reads every report in
    # the directory, so a run at a smaller shard count left beside a run at a larger one would be
    # merged into it - and the totals would be a sum over two runs that the coverage check could not
    # see, because every field they must share they do share. Refusing to start would have been the
    # other answer and is worse: it leaves the caller deleting files by hand between attempts.
    stale = sorted(reports.glob("*.report")) + sorted(logs.glob("*.log"))

    for path in stale:
        path.unlink()

    print(f"# test262 {fields['upstream']} at {fields['revision']}")
    print(f"# {fields['files']} files, content {digest} - the checkout answers to {PIN.name}")
    print(f"# manifest {arguments.manifest}; declined {arguments.decline or '(none)'}")
    print(f"# {shards} shards across {jobs} processes, {arguments.fuel} fuel and "
          f"{arguments.wall} ms per variant")
    print(f"# retaining every shard's transcript under {out}"
          + (f", after removing {len(stale)} file(s) an earlier run left there" if stale else ""))

    failed = []

    with concurrent.futures.ThreadPoolExecutor(max_workers=jobs) as pool:
        futures = [
            pool.submit(shard, binary, suite, arguments, index, shards, reports, logs)
            for index in range(shards)
        ]

        for future in concurrent.futures.as_completed(futures):
            index, code, transcript = future.result()
            print(f"--- shard {index} of {shards} exited {code}; {transcript}")

            # EXIT CODE 3 IS THE HARNESS SAYING IT CANNOT MEASURE, and it is the one a merge must
            # not be handed a report from. Every other code is a measurement: 1 means cases failed,
            # which is a result and not a reason to stop.
            if code == 3:
                failed.append(index)

    if failed:
        raise SystemExit(
            f"# {len(failed)} shard(s) reported a harness defect and measured nothing: {failed}"
        )

    merged = out / "test262.report"
    done = subprocess.run(
        [str(binary), "--merge", str(reports), "--report", str(merged)],
        cwd=str(ROOT),
        capture_output=True,
        text=True,
    )

    (out / "merge.log").write_text(done.stdout + done.stderr)
    print(done.stdout.rstrip())

    if done.stderr.strip():
        print(done.stderr.rstrip())

    print(f"# the whole-run report is {merged}")
    return done.returncode


if __name__ == "__main__":
    sys.exit(main())
