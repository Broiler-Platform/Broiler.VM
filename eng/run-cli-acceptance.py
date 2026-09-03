"""Judge the JavaScript host on its own command line, over files, with nothing injected.

WHY THIS IS A SCRIPT AND NOT A TEST METHOD. The subject is a PUBLISHED BINARY: its argument
parsing, its exit codes, which of its two output streams carries which message, and what it does
with a file that is not UTF-8. None of that is reachable from a test project - rule A11 forbids a
test project to reference a profile assembly, so a test could not compose the host at all, and a
test that called an internal type would be judging something a user cannot invoke.

WHAT IT DELIBERATELY DOES NOT DO. It patches no source, reaches for no internal, and embeds no
JavaScript. Every case is a row in `src/tests/cli/expected.txt` naming a command line and the exit
code it must answer with; the inputs are files in that tree. A control here is an INPUT, which is
the whole point: this is the host as a person meets it.
"""

import argparse
import io
import os
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SUITE = os.path.join(ROOT, "src", "tests", "cli")
EXPECTED = os.path.join(SUITE, "expected.txt")
PROJECT = os.path.join(
    "src", "compositions", "Broiler.VM.Composition.JavaScript.Cli",
    "Broiler.VM.Composition.JavaScript.Cli.csproj")
BINARY_NAME = "Broiler.VM.Composition.JavaScript.Cli"


def rows(path):
    """Every case: the argument words, the exit code, and the substring that must appear."""
    cases = []

    for number, line in enumerate(io.open(path, encoding="utf-8").read().splitlines(), start=1):
        text = line.strip()

        if len(text) == 0 or text.startswith("#"):
            continue

        parts = text.split("|")

        if len(parts) != 3:
            raise SystemExit(f"{path}:{number}: `{text}` is not `args|exit|substring`")

        cases.append((number, parts[0].split(), int(parts[1]), parts[2]))

    return cases


def shorten(path):
    """The path relative to the checkout where that is expressible, and the path otherwise.

    An expectation table given on the command line may sit on another drive, and on Windows
    `relpath` raises rather than returning an absolute path when the two have no common root.
    """
    try:
        return os.path.relpath(path, ROOT)
    except ValueError:
        return path


def binary(built):
    """The host to judge: a published directory when given one, else the build output."""
    for name in (BINARY_NAME + ".exe", BINARY_NAME):
        candidate = os.path.join(built, name)

        if os.path.isfile(candidate):
            return candidate

    raise SystemExit(f"no {BINARY_NAME} under {built}")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--binary-directory",
        default=os.path.join(
            ROOT, "src", "compositions", "Broiler.VM.Composition.JavaScript.Cli",
            "bin", "Release", "net10.0"),
        help="the directory holding the host to judge; a published output, or the build's")
    parser.add_argument("--build", action="store_true", help="build the host first")
    parser.add_argument("--verbose", action="store_true")
    parser.add_argument(
        "--expected", default=EXPECTED,
        help="the expectation table to judge against. It exists so this script can be shown to "
             "REPORT A MISMATCH: point it at a table with deliberately wrong rows and it must "
             "fail, because a driver whose every row passes may simply not be comparing anything.")
    arguments = parser.parse_args()

    if arguments.build:
        built = subprocess.run(
            ["dotnet", "build", PROJECT, "-c", "Release", "--nologo", "-v", "q"],
            cwd=ROOT, capture_output=True, text=True, encoding="utf-8", errors="replace")

        if built.returncode != 0:
            print(built.stdout or "")
            raise SystemExit("the host did not build")

    host = binary(arguments.binary_directory)
    cases = rows(arguments.expected)
    failed = []

    print(f"# judging {host}")
    print(f"# over {len(cases)} command lines from {shorten(arguments.expected)}")

    for number, words, expected_exit, substring in cases:
        # The suite directory is the working directory, so every path in a row is relative to it
        # and no row carries an absolute path that would differ between two machines.
        done = subprocess.run(
            [host] + words, cwd=SUITE, capture_output=True, text=True,
            encoding="utf-8", errors="replace")

        output = (done.stdout or "") + (done.stderr or "")
        wrong = []

        if done.returncode != expected_exit:
            wrong.append(f"exit {done.returncode}, expected {expected_exit}")

        if len(substring) != 0 and substring not in output:
            wrong.append(f"output does not contain `{substring}`")

        shown = " ".join(words)

        if len(wrong) == 0:
            if arguments.verbose:
                print(f"ok   {shown}")

            continue

        failed.append(shown)
        print(f"FAIL {shown}: " + "; ".join(wrong))

        for line in output.splitlines()[:4]:
            print(f"       {line}")

    print(
        f"# {len(cases) - len(failed)} of {len(cases)} command lines answered as declared"
        if len(failed) == 0
        else f"# {len(failed)} of {len(cases)} command lines FAILED")

    return 1 if len(failed) != 0 else 0


sys.exit(main())
