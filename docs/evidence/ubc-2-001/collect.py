#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# COLLECT BUNDLE UBC-2-001: the fixture composition published and run in three modes, and what it retains.
#
# Run from the repository root at the commit the bundle names, after `dotnet build Broiler.VM.slnx -c Release`:
#
#   python docs/evidence/ubc-2-001/collect.py [--rid win-x64] [--vcvars PATH-TO-vcvars64.bat]
#
# It writes, into this directory:
#   run-jit.log, run-trimmed.log, run-aot.log      the root's contract checks, verbose, in each mode
#   catalog-fixture.txt                            the catalog table the published root prints (--closure),
#                                                  and catalog-modes.log saying whether the three modes agree
#   closure-fixture.txt                            the non-framework assemblies each publish contains
#   corpus-jit.log, corpus-trimmed.log, corpus-aot.log
#                                                  the fixture family's retained corpus (src/tests/corpus/ubc-2)
#                                                  replayed in each mode, and corpus-modes.log comparing them
#   ubc1-corpus-jit.log, ...-trimmed.log, ...-aot.log
#                                                  the universal bytecode's malformed corpus (src/tests/corpus/ubc-1)
#                                                  replayed in each mode, and ubc1-corpus-modes.log comparing them
#   disasm-jit.txt, disasm-aot.txt, fold-check.log the loop's disassembly for the fixture family on the JIT
#                                                  lane and on the Native AOT image, and the assertion over both
#   publish.log                                    the three publishes' output
# It judges nothing beyond the fold assertion it states: the README says what the files show.
#
# THE NATIVE AOT PUBLISH on Windows needs a vcvars64 shell, as the core's collector records (EX-42). On the
# machine this bundle was collected on the Community installation's vcvars64 could not find vswhere and
# left link.exe off the path, and the Professional installation's worked; --vcvars names the one to use.

import argparse
import io
import os
import re
import subprocess
import sys
import tempfile

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__)))))
HERE = os.path.dirname(os.path.abspath(__file__))
PROJECT = "src/compositions/Broiler.VM.Composition.Ubc.Fixture"
NAME = "Broiler.VM.Composition.Ubc.Fixture"
WINDOWS = os.name == "nt"
DEFAULT_VCVARS = r"C:\Program Files\Microsoft Visual Studio\18\Professional\VC\Auxiliary\Build\vcvars64.bat"

# What a type test on the family struct, or an interface dispatch through the family contract, looks like
# in each disassembly. A listing containing any of these has not folded the family away.
UNFOLDED = re.compile(
    r"CORINFO_HELP_ISINSTANCEOF|CORINFO_HELP_CHKCAST|CORINFO_HELP_BOX|CORINFO_HELP_UNBOX|"
    r"RhTypeCast|IsInstanceOf|CheckCast|IUbcFamily|RhpNewFast.*TallyFamily")


def run(command, env=None, cwd=ROOT):
    result = subprocess.run(command, cwd=cwd, capture_output=True, text=True, env=env,
                            encoding="utf-8", errors="replace")
    return result.returncode, (result.stdout + result.stderr).replace("\r\n", "\n")


def write(name, text):
    with io.open(os.path.join(HERE, name), "w", encoding="utf-8", newline="\n") as handle:
        handle.write(text if text.endswith("\n") else text + "\n")


def vcvars_batch(vcvars, commands):
    script = '@echo off\r\ncall "%s" >nul\r\nset Platform=\r\n%s\r\n' % (vcvars, "\r\n".join(commands))
    handle, batch = tempfile.mkstemp(suffix=".bat")
    os.close(handle)
    io.open(batch, "w", encoding="ascii", newline="").write(script)
    try:
        return run(["cmd", "/c", batch])
    finally:
        os.remove(batch)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--rid", default="win-x64" if WINDOWS else "linux-x64")
    parser.add_argument("--vcvars", default=DEFAULT_VCVARS)
    arguments = parser.parse_args()

    exe = ".exe" if WINDOWS else ""
    trimmed = os.path.join("artifacts", "publish-ubc-fixture-trimmed")
    aot = os.path.join("artifacts", "publish-ubc-fixture-aot")
    publish_log = []

    code, out = run(["dotnet", "publish", PROJECT, "-c", "Release", "-r", arguments.rid, "--self-contained", "true",
                     "-p:PublishAot=false", "-p:PublishTrimmed=true", "-o", trimmed])
    publish_log.append("--- TRIMMED: dotnet publish -r %s --self-contained true -p:PublishTrimmed=true ---\n%s\nexit %d\n" % (arguments.rid, out, code))

    command = "dotnet publish %s -c Release -r %s -p:PublishAot=true -p:IlcUseEnvironmentalTools=true -o %s" % (PROJECT, arguments.rid, aot)
    code, out = vcvars_batch(arguments.vcvars, [command]) if WINDOWS else run(command.split())
    publish_log.append("--- NATIVE AOT: %s ---\n%s\nexit %d\n" % (command, out, code))
    write("publish.log", "\n".join(publish_log))

    modes = {
        "jit": ["dotnet", os.path.join(PROJECT, "bin", "Release", "net10.0", NAME + ".dll")],
        "trimmed": [os.path.join(ROOT, trimmed, NAME + exe)],
        "aot": [os.path.join(ROOT, aot, NAME + exe)],
    }

    catalogs, corpora, malformed = {}, {}, {}

    for mode, launcher in modes.items():
        code, out = run(launcher + ["--verbose"])
        write("run-%s.log" % mode, "$ %s --verbose\n%s\nexit %d" % (" ".join(launcher), out, code))
        code, catalogs[mode] = run(launcher + ["--closure"])
        code, corpora[mode] = run(launcher + ["--corpus", "src/tests/corpus/ubc-2"])
        write("corpus-%s.log" % mode, corpora[mode] + "exit %d" % code)
        code, malformed[mode] = run(launcher + ["--ubc1-corpus", "src/tests/corpus/ubc-1"])
        write("ubc1-corpus-%s.log" % mode, malformed[mode] + "exit %d" % code)

    write("catalog-fixture.txt", catalogs["jit"])

    def compare(name, tables):
        same = tables["jit"] == tables["trimmed"] == tables["aot"]
        write(name, "the three modes' tables are %s\n" % ("BYTE-IDENTICAL" if same
              else "NOT IDENTICAL - the answers differ between publish modes"))

    compare("catalog-modes.log", catalogs)
    compare("corpus-modes.log", corpora)
    compare("ubc1-corpus-modes.log", malformed)

    closure = ["# closure %s rid=%s" % (NAME, arguments.rid), ""]
    for mode, directory in (("trimmed", trimmed), ("aot", aot)):
        full = os.path.join(ROOT, directory)
        names = sorted(n[:-4] for n in os.listdir(full) if n.endswith(".dll")
                       and not n.startswith(("System.", "Microsoft."))
                       and n not in ("netstandard.dll", "mscorlib.dll", "WindowsBase.dll"))
        closure.append("[%s] %d non-framework assemblies" % (mode, len(names)))
        closure.extend(names)
        closure.append("")
    write("closure-fixture.txt", "\n".join(closure))

    # The fold check, JIT half: the loop compiled with full optimisation, its listing extracted.
    listing_file = os.path.join(tempfile.gettempdir(), "ubc-2-001-disasm.txt")
    if os.path.exists(listing_file):
        os.remove(listing_file)
    env = dict(os.environ, DOTNET_TieredCompilation="0", DOTNET_ReadyToRun="0",
               DOTNET_JitDisasm="Run", DOTNET_JitStdOutFile=listing_file)
    run(modes["jit"], env)
    text = io.open(listing_file, encoding="utf-8", errors="replace").read()
    marker = "; Assembly listing for method Broiler.VM.Emitter.Bytecode.UbcInterpreter`1[Com.Example.Tally.TallyFamily]:Run"
    start = text.index(marker)
    end = text.find("; Assembly listing for method", start + len(marker))
    jit = text[start:end if end > 0 else len(text)]
    write("disasm-jit.txt", jit)

    # The fold check, Native AOT half: the image disassembled with dumpbin, the same instantiation's loop.
    aot_listing = ""
    if WINDOWS and os.path.exists(arguments.vcvars):
        dump = os.path.join(tempfile.gettempdir(), "ubc-2-001-aot.txt")
        vcvars_batch(arguments.vcvars, ['dumpbin /disasm:nobytes "%s" > "%s"' % (modes["aot"][0], dump)])
        image = io.open(dump, encoding="latin-1").read()
        label = "UbcInterpreter_1<Com_Example_Tally_Com_Example_Tally_TallyFamily>__Run:"
        at = image.find(label)
        if at >= 0:
            at = image.rfind("\n", 0, at) + 1
            following = re.compile(r"^\S[^\n]*:\s*$", re.M).search(image, at + len(label) + 1)
            aot_listing = image[at:following.start() if following else len(image)]
    write("disasm-aot.txt", aot_listing or "NOT COLLECTED: no disassembler over the published image in this lane")

    lines = []
    for mode, listing in (("jit", jit), ("aot", aot_listing)):
        if not listing:
            lines.append("[%s] not asserted: no listing was collected" % mode)
            continue
        unfolded = [l.strip() for l in listing.splitlines() if UNFOLDED.search(l)]
        direct = [l.strip() for l in listing.splitlines() if "TallyFamily" in l and "Handle" in l and "call" in l]
        lines.append("[%s] %s: %s type test or family-contract dispatch; %s direct call to the family's Handle"
                     % (mode, "FOLDED" if not unfolded and direct else "NOT FOLDED",
                        "no" if not unfolded else "%d lines with a" % len(unfolded),
                        "a" if direct else "no"))
        lines.extend("    " + l for l in unfolded)
    write("fold-check.log", "\n".join(lines))
    print("collected; now write README.md, then run eng/ubc-bundle-manifest.py")
    return 0


if __name__ == "__main__":
    sys.exit(main())
