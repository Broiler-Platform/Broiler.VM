#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT BUNDLE UBC-1-001: Broiler.VM.Ubc's rules, its codec's round trip and its malformed corpus.
#
# Run from the repository root at the commit the bundle names, after `dotnet build Broiler.VM.slnx -c Release`:
#
#   python docs/evidence/ubc-1-001/collect.py
#
# It writes, into this directory:
#   round-trip.log          the codec determinism tests (UBC-1.8): every control, every entry the reader
#                           admits and every sample of every section kind read and rewritten twice
#   corpus-tests.log        the malformed corpus's tests (UBC-1.7): the corpus on disk is what the
#                           generator writes, every row's length and hash, a mutated entry detected, every
#                           answer as recorded, every shape, effect form, target form, kind, ceiling and
#                           code reached; and the replayer's own tests
#   corpus-replay.log       the replayer's table over src/tests/corpus/ubc-1, one line per entry, printed by
#                           the fixture composition's --ubc1-corpus mode on the JIT lane, which compiles the
#                           replayer from the one copy the contract suite compiles
#   contract-tests.log      the whole contract suite, every test named
#   architecture-tests.log  rules U1, U2, U4, U8, U9, A7, A15 and M1 and the core contract version test in
#                           detail, then the whole architecture suite, every test named
# and then eng/ubc-bundle-manifest.py writes manifest.json. It judges nothing: the README says what the
# files show.

import os
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
CONTRACT = "src/tests/Broiler.VM.Contract.Tests"
ARCHITECTURE = "src/tests/Broiler.VM.Architecture.Tests"
ROOT_DLL = "src/compositions/Broiler.VM.Composition.Ubc.Fixture/bin/Release/net10.0/Broiler.VM.Composition.Ubc.Fixture.dll"

# The retained logs are read by people who did not run them: the command line's own messages in English.
ENV = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en", VSLANG="1033", DOTNET_NOLOGO="1")


def run(command):
    result = subprocess.run(command, cwd=ROOT, capture_output=True, text=True, env=ENV,
                            encoding="utf-8", errors="replace")
    return result.returncode, (result.stdout + result.stderr).replace("\r\n", "\n")


def write(name, text):
    with open(os.path.join(HERE, name), "w", encoding="utf-8", newline="\n") as handle:
        handle.write(text if text.endswith("\n") else text + "\n")


def test(project, filter_expression=None):
    command = ["dotnet", "test", project, "-c", "Release", "--no-build",
               "--logger", "console;verbosity=detailed"]
    if filter_expression:
        command += ["--filter", filter_expression]
    code, out = run(command)
    return "$ %s\n%s\nexit %d\n" % (" ".join(command), out, code)


def classes(*names):
    return "|".join("FullyQualifiedName~%s" % name for name in names)


def main():
    write("round-trip.log", test(CONTRACT, classes("UbcCodecDeterminismTests")))
    write("corpus-tests.log", test(CONTRACT, classes("UbcMalformedCorpusTests", "UbcCorpusReplayTests")))

    command = ["dotnet", ROOT_DLL, "--ubc1-corpus", "src/tests/corpus/ubc-1"]
    code, out = run(command)
    write("corpus-replay.log", "$ %s\n%s\nexit %d" % (" ".join(command), out, code))

    write("contract-tests.log", test(CONTRACT))

    rules = test(ARCHITECTURE, classes("UbcRuleTests", "TopologyBudgetTests", "ApiSurfaceTests",
                                       "CoreContractVersionTests", "ProjectFileRuleTests.A7_"))
    write("architecture-tests.log", rules + "\n--- THE WHOLE ARCHITECTURE SUITE ---\n\n" + test(ARCHITECTURE))
    print("collected; now write README.md, then run eng/ubc-bundle-manifest.py")
    return 0


if __name__ == "__main__":
    sys.exit(main())
