#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# RUN THE RETAINED OCTANE WORKLOAD, ONE BENCHMARK PER PROCESS, AND RETAIN WHAT EVERY ONE OF THEM DID.
#
# The workload roadmap's JSW-10 asks for the Octane checkout to be pinned and archived so that a
# benchmark result has an identity, and section 1 states the target as behaviour: every benchmark in
# the checkout reports a score through the ordinary command line of the end-user host, driven one
# benchmark per process by `src/tests/octane/run-one.js`, with the process exit code agreeing with
# whether a score was produced.
#
# This script is that command line, run from the pin rather than from a directory somebody happens
# to have:
#
#   * it extracts `src/tests/octane/pins/octane-<revision>.tar.gz` into a scratch directory;
#   * it CHECKS the archive against `octane.pin` before extracting a byte of it, so a run is against
#     the revision the repository decided rather than against whatever is on the disk;
#   * it runs each benchmark with the files that benchmark needs, in the order the suite's own
#     `index.html` loads them;
#   * and it retains every line, failures included, rather than the passing half.
#
# IT PUBLISHES NO FIGURE OF ITS OWN AND COMPARES NOTHING. A score printed here is a number about
# this configuration; there is no measurement lane and no baseline register, both of which are
# `JS-10`'s, and roadmap section 17 governs any figure that is ever retained. What this answers is
# the question section 1 asks: does the benchmark REPORT a score, or does it meet something.
#
#   python3 eng/run-octane.py [--binary-directory <dir>] [--only <name>] [--fuel <n>] [--wall <ms>]
#                             [--live-bytes <n>]

import argparse
import hashlib
import pathlib
import shutil
import subprocess
import sys
import tarfile
import tempfile

ROOT = pathlib.Path(__file__).resolve().parent.parent
PINS = ROOT / "src/tests/octane/pins"
PIN = PINS / "octane.pin"
DRIVER = "src/tests/octane/run-one.js"
DEFAULT_BINARY_DIRECTORY = (
    ROOT / "src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0"
)

# The files each benchmark needs beside `base.js`, in the order the suite's own index.html loads
# them. Every other benchmark is one file named after itself.
COMPANIONS = {
    "gbemu": ["gbemu-part1.js", "gbemu-part2.js"],
    "zlib": ["zlib.js", "zlib-data.js"],
    "typescript": ["typescript.js", "typescript-input.js", "typescript-compiler.js"],
}

BENCHMARKS = [
    "richards", "deltablue", "crypto", "raytrace", "earley-boyer", "regexp", "splay",
    "navier-stokes", "pdfjs", "mandreel", "gbemu", "code-load", "box2d", "zlib", "typescript",
]


def pinned():
    """The pin's fields, or a failure."""
    if not PIN.exists():
        raise SystemExit(f"# no pin at {PIN}")

    fields = {}

    for line in PIN.read_text().splitlines():
        line = line.strip()

        if not line or line.startswith("#"):
            continue

        key, _, value = line.partition(" ")
        fields[key] = value.strip()

    return fields


def extract(fields, into):
    """Checks the archive against the pin and extracts it. A disagreement is refused, never fixed."""
    archive = ROOT / fields["archived-at"]

    if not archive.exists():
        raise SystemExit(f"# the pin names an archive that is not here: {archive}")

    digest = hashlib.sha256(archive.read_bytes()).hexdigest()

    if digest != fields["archive-sha256"]:
        raise SystemExit(
            f"# the archive does not hash to what the pin says\n"
            f"#   pin  {fields['archive-sha256']}\n#   file {digest}"
        )

    with tarfile.open(archive) as opened:
        members = sorted((m for m in opened.getmembers() if m.isfile()), key=lambda m: m.name)
        content = hashlib.sha256()

        for member in members:
            path = member.name.split("/", 1)[1]
            body = opened.extractfile(member).read()
            content.update((f"{path}\n{hashlib.sha256(body).hexdigest()}\n").encode())

        if content.hexdigest() != fields["content-sha256"]:
            raise SystemExit("# the archive's contents do not hash to what the pin says")

        if len(members) != int(fields["files"]):
            raise SystemExit(
                f"# the archive holds {len(members)} files and the pin says {fields['files']}"
            )

        opened.extractall(into, filter="data")

    return into / f"octane-{fields['revision']}"


def run(binary, checkout, name, fuel, wall, live_bytes):
    """One benchmark, one process, through the ordinary command line."""
    files = [str(checkout / "base.js")]
    files += [str(checkout / f) for f in COMPANIONS.get(name, [f"{name}.js"])]

    # THE DRIVER MUST BE LAST AND THE HOST SORTS ITS PATHS ORDINALLY, so it is named relatively
    # from the repository root, where it sorts after an absolute path. That is a property of the
    # host's argument handling rather than of this script, and naming it here is cheaper than
    # discovering it from a run in which the harness had not been defined yet.
    command = [str(binary)] + files + [
        DRIVER, "--fuel", str(fuel), "--wall", str(wall), "--live-bytes", str(live_bytes)]
    done = subprocess.run(command, cwd=str(ROOT), capture_output=True, text=True)
    return done.returncode, (done.stdout + done.stderr).rstrip()


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--binary-directory", default=str(DEFAULT_BINARY_DIRECTORY))
    parser.add_argument("--only", default=None)
    parser.add_argument("--fuel", type=int, default=1_000_000_000_000)
    parser.add_argument("--wall", type=int, default=3_600_000)

    # THE MEMORY ALLOWANCE IS NAMED HERE RATHER THAN LEFT TO THE PROFILE, because the profile's
    # default is sized for a program a person types and two of these benchmarks hold working sets
    # far larger than that. A run under an allowance nobody chose reports a named exhaustion after
    # a benchmark has already printed its score, which is the least useful of the outcomes: it is
    # neither a score nor an absence. The figure is an allowance a caller states, not a
    # measurement, and the profile's hard maximum still bounds it.
    parser.add_argument("--live-bytes", type=int, default=1_000_000_000)
    arguments = parser.parse_args()

    binary = pathlib.Path(arguments.binary_directory) / "Broiler.VM.Composition.JavaScript.Cli"

    if not binary.exists():
        raise SystemExit(f"# no binary at {binary}")

    fields = pinned()
    scratch = pathlib.Path(tempfile.mkdtemp(prefix="broiler-octane-"))

    try:
        checkout = extract(fields, scratch)
        print(f"# octane {fields['upstream']} at {fields['revision']}")
        print(f"# {fields['files']} files, content {fields['content-sha256']}")
        print(f"# judging {binary}")

        wanted = [arguments.only] if arguments.only else BENCHMARKS
        scored = 0

        for name in wanted:
            code, output = run(
                binary, checkout, name, arguments.fuel, arguments.wall, arguments.live_bytes)
            print(f"--- {name} (exit {code})")

            for line in output.splitlines():
                print(f"    {line}")

            if code == 0:
                scored += 1

        print(f"# {scored} of {len(wanted)} benchmarks reported a score and exited zero")
        return 0 if scored == len(wanted) else 1
    finally:
        shutil.rmtree(scratch, ignore_errors=True)


if __name__ == "__main__":
    sys.exit(main())
