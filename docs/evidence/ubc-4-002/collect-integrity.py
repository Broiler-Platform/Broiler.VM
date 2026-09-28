#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# RETAIN THE CORPUS MUTATION PASS BUNDLE UBC-4-001 COULD NOT RUN, OVER A COPY OF THE CORPUS.
#
# Run from the repository root at the commit the bundle names, after `dotnet build Broiler.VM.slnx -c Release`:
#
#   python docs/evidence/ubc-4-002/collect-integrity.py
#
# eng/wasm-corpus-integrity.py moves four entries by one byte each, replays, and restores them, and it
# checks its own restore. It is run here over a copy of src/tests/wasm/corpus made for the run, with the
# harness replaying the same copy, so the retained bytes are never moved at all; afterwards the copy is
# compared with the retained corpus byte for byte and the answer is written into the log. The tool's
# output is kept with its line endings made LF and the copy's path replaced by <scratch>, and every line
# this script adds - the command, the exit code and the two notes - begins with '# collect-integrity:'.

import filecmp
import io
import os
import shutil
import subprocess
import sys
import tempfile

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
CORPUS = os.path.join(ROOT, "src", "tests", "wasm", "corpus")
HARNESS = "src/compositions/Broiler.VM.Composition.WebAssembly.Harness/bin/Release/net10.0/" \
          "Broiler.VM.Composition.WebAssembly.Harness.dll"
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", VSLANG="1033", DOTNET_NOLOGO="1")


def identical(left, right):
    names = sorted(set(os.listdir(left)) | set(os.listdir(right)))
    return [n for n in names if not (os.path.isfile(os.path.join(left, n)) and os.path.isfile(os.path.join(right, n))
                                     and filecmp.cmp(os.path.join(left, n), os.path.join(right, n), shallow=False))]


def main():
    scratch = tempfile.mkdtemp(prefix="ubc-4-002-integrity-")
    try:
        copy = os.path.join(scratch, "corpus")
        shutil.copytree(CORPUS, copy)
        command = [sys.executable, "eng/wasm-corpus-integrity.py", "--corpus", copy, "--",
                   "dotnet", HARNESS, "--corpus", copy]
        result = subprocess.run(command, cwd=ROOT, capture_output=True, text=True, env=ENV,
                                encoding="utf-8", errors="replace")
        output = (result.stdout + result.stderr).replace("\r\n", "\n").replace(copy, "<scratch>")
        differing = identical(copy, CORPUS)
        text = "\n".join([
            "# collect-integrity: $ python eng/wasm-corpus-integrity.py --corpus <scratch> -- dotnet %s --corpus <scratch>"
            % HARNESS,
            "# collect-integrity: <scratch> is a copy of src/tests/wasm/corpus made for this run",
            output.rstrip("\n"),
            "",
            "# collect-integrity: exit %d" % result.returncode,
            "",
            "# collect-integrity: after the run the copy %s src/tests/wasm/corpus byte for byte%s" % (
                "is identical to" if not differing else "DIFFERS from",
                "".join("\n#   " + n for n in differing)),
        ])
        with io.open(os.path.join(HERE, "corpus-integrity.log"), "w", encoding="utf-8", newline="\n") as handle:
            handle.write(text + "\n")
        print("corpus-integrity.log: exit %d, %d files differ after the run" % (result.returncode, len(differing)))
        return 1 if result.returncode or differing else 0
    finally:
        shutil.rmtree(scratch, ignore_errors=True)


if __name__ == "__main__":
    sys.exit(main())
