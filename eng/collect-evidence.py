"""Collect the retained evidence for a Broiler.VM milestone bundle.

Both earlier bundles named a reproduction gap this file closes. VM-0-001 said its negative
control "needs the injection script; it is not retained in the repository", and VM-1-002 said
the controls "are reproduced by the script in the change that landed this bundle" - which was
not true, because no script was landed. The controls are the reason a green suite means
anything, so the thing that produces them belongs in the repository.

    python eng/collect-evidence.py --bundle VM-1-003 --out docs/evidence/vm-1

Every step writes its own log. Nothing here decides whether the result is good: it runs the
procedure and retains what happened, including failures. Reading the result is the bundle's job.

Step 6 needs a vcvars64 shell. The ILCompiler package's own findvcvarsall.bat cannot locate
vswhere.exe on this machine and emits its error text into the property that becomes the linker
path, so a plain -p:PublishAot=true fails with MSB3073. Exclusion EX-42 records it; pass
--vcvars to point at a different Visual Studio.
"""

import argparse
import hashlib
import io
import os
import platform
import re
import subprocess
import sys
import tempfile

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
HOST = r"src\tests\Broiler.VM.Fixtures.Host\Broiler.VM.Fixtures.Host.csproj"
SOLUTION = "Broiler.VM.slnx"
DEFAULT_VCVARS = r"C:\Program Files\Microsoft Visual Studio\18\Community\VC\Auxiliary\Build\vcvars64.bat"

# The four negative controls. Each is one edit that a named rule must reject: (file, old, new).
# A control that does not fail is a finding about the suite, not a step to retry - VM-1-002
# records the run where control 4 passed and what that exposed.
CONTROLS = [
    (
        "Broiler.VM.Runtime references the test-only Broiler.VM.Fixtures",
        r"src\Broiler.VM.Runtime\Broiler.VM.Runtime.csproj",
        "</Project>",
        '  <ItemGroup>\n'
        '    <ProjectReference Include="..\\tests\\Broiler.VM.Fixtures\\Broiler.VM.Fixtures.csproj" />\n'
        '  </ItemGroup>\n\n'
        "</Project>",
    ),
    (
        "an edge the checkout HAS is deleted from graph.manifest.json",
        r"src\tests\Broiler.VM.Architecture.Tests\graph.manifest.json",
        None,  # resolved at run time: drop the first Runtime edge the manifest declares
        None,
    ),
    (
        "a retired name is exported from a product assembly",
        r"src\Broiler.VM.Abstractions\VmCoreContract.cs",
        None,  # append a public type carrying a struck name
        None,
    ),
    (
        "the deterministic no-provider refusal is removed",
        r"src\Broiler.VM.Runtime\VmArtifactLoadMediator.cs",
        "        if (provider is null)\n        {",
        "        if (provider is null && false)\n        {",
    ),
]


def run(command, shell=False):
    """Run a command from the component root and return (exit code, combined output)."""
    environment = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en")
    completed = subprocess.run(
        command,
        cwd=ROOT,
        shell=shell,
        capture_output=True,
        text=True,
        errors="replace",
        env=environment,
    )
    return completed.returncode, (completed.stdout or "") + (completed.stderr or "")


def write(path, text):
    io.open(path, "w", encoding="utf-8", newline="").write(text)


def read_bytes(path):
    return io.open(path, "rb").read()


def restore(path, original):
    io.open(path, "wb").write(original)
    assert read_bytes(path) == original, "failed to restore " + path


def native_binary(publish_directory):
    exe = os.path.join(ROOT, publish_directory, "Broiler.VM.Fixtures.Host.exe")
    return exe if os.path.exists(exe) else None


def collect_controls(out):
    """Inject, run, revert, re-run. Both runs are retained for every control."""
    log = []

    for index, (name, relative, old, new) in enumerate(CONTROLS, start=1):
        path = os.path.join(ROOT, relative)
        original = read_bytes(path)
        text = original.decode("utf-8")

        # Injection points are written with \n and the working tree is CRLF. Matching without
        # normalising found nothing at all, which reads as "the control does not apply" rather
        # than as the bug it is.
        crlf = "\r\n" in text
        if crlf:
            old = old.replace("\n", "\r\n") if old else old
            new = new.replace("\n", "\r\n") if new else new

        if index == 2:
            # \r?\n on both sides: the manifest is CRLF in the working tree, and a bare \n
            # matched nothing at all the first time this ran.
            match = re.search(r'(\r?\n)[ \t]*"Broiler\.VM\.Abstractions",[ \t]*\r?\n', text)
            assert match, "no edge to delete from the graph manifest"
            mutated = text[: match.start()] + match.group(1) + text[match.end():]
        elif index == 3:
            struck = "\n\npublic sealed class VmHandle\n{\n}\n"
            mutated = text.rstrip() + (struck.replace("\n", "\r\n") if crlf else struck)
        else:
            assert old in text, "control %d: injection point not found in %s" % (index, relative)
            mutated = text.replace(old, new, 1)

        assert mutated != text, "control %d changed nothing" % index

        log.append("=== CONTROL %d: %s ===" % (index, name))
        log.append("file: " + relative)
        write(path, mutated)
        try:
            _, injected = run(["dotnet", "test", SOLUTION, "-c", "Release"])
        finally:
            restore(path, original)

        log.append(injected.strip())
        log.append("")
        log.append("--- after revert ---")
        # A control that edits a project file leaves the previous restore in obj/, so the
        # revert run has to restore again or it re-reads the injected graph and stays red.
        run(["dotnet", "restore", SOLUTION])
        _, reverted = run(["dotnet", "test", SOLUTION, "-c", "Release"])
        log.append(reverted.strip())
        log.append("")

        failed = "Failed!" in injected
        print("  control %d: %s when injected, %s after revert"
              % (index, "FAILED" if failed else "PASSED - a finding about the suite",
                 "green" if "Failed!" not in reverted else "STILL RED"))

    write(os.path.join(out, "negative-control.log"), "\n".join(log))


def hashed_files():
    """The set the bundle pins: the vendored file, the manifests, the records, product source."""
    files = [SOLUTION, "Directory.Build.props", "NuGet.config",
             os.path.join("eng", "Broiler.Packaging.props"),
             "CODE-ASSURANCE.md", "assurance.manifest.json"]

    records = os.path.join(ROOT, "docs", "adr")
    files += [os.path.join("docs", "adr", name)
              for name in sorted(os.listdir(records)) if name.endswith(".md")]

    for assembly in ("Broiler.VM.Abstractions", "Broiler.VM.Binary", "Broiler.VM.Runtime"):
        directory = os.path.join(ROOT, "src", assembly)
        for current, directories, names in os.walk(directory):
            directories[:] = [d for d in directories if d not in ("bin", "obj")]
            for name in sorted(names):
                if name.endswith(".cs"):
                    full = os.path.join(current, name)
                    files.append(os.path.relpath(full, ROOT).replace(os.sep, "/"))

    for manifest in ("graph.manifest.json", "rules.register.json"):
        files.append("src/tests/Broiler.VM.Architecture.Tests/" + manifest)

    return [f.replace(os.sep, "/") for f in files]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--bundle", required=True)
    parser.add_argument("--out", required=True)
    parser.add_argument("--vcvars", default=DEFAULT_VCVARS)
    parser.add_argument("--skip-controls", action="store_true")
    arguments = parser.parse_args()

    out = os.path.join(ROOT, arguments.out)
    os.makedirs(out, exist_ok=True)

    print("collecting %s into %s" % (arguments.bundle, arguments.out))

    print("  1 build")
    _, build = run(["dotnet", "build", SOLUTION, "-c", "Release"])
    write(os.path.join(out, "build.log"), build.strip() + "\n")

    print("  2 test")
    _, test = run(["dotnet", "test", SOLUTION, "-c", "Release"])
    write(os.path.join(out, "test.log"), test.strip() + "\n")

    print("  3 pack")
    packed = tempfile.mkdtemp(prefix="broiler-vm-pack-")
    _, pack = run(["dotnet", "pack", SOLUTION, "-c", "Release", "-o", packed])
    produced = sorted(os.listdir(packed))
    pack += "\n--- produced ---\n" + "\n".join(produced)
    pack += "\nnupkg: %d\nsnupkg: %d\n" % (
        sum(1 for p in produced if p.endswith(".nupkg")),
        sum(1 for p in produced if p.endswith(".snupkg")))
    write(os.path.join(out, "pack.log"), pack.strip() + "\n")

    print("  4-5 jit and trimmed")
    lines = ["=== JIT: dotnet run --project %s -c Release -- --verbose ===" % HOST]
    _, jit = run(["dotnet", "run", "--project", HOST, "-c", "Release", "--", "--verbose"])
    lines.append(jit.strip())

    lines.append("")
    lines.append("=== TRIMMED: dotnet publish -r win-x64 --self-contained true -p:PublishAot=false ===")
    trimmed_out = os.path.join("artifacts", "publish-trimmed")
    _, trimmed = run(["dotnet", "publish", HOST, "-c", "Release", "-r", "win-x64",
                      "--self-contained", "true", "-p:PublishAot=false", "-p:PublishTrimmed=true",
                      "-o", trimmed_out])
    lines.append(trimmed.strip())

    binary = native_binary(trimmed_out)
    if binary:
        lines.append("")
        lines.append("--- running the trimmed binary ---")
        lines.append("image size: %d bytes" % os.path.getsize(binary))
        code, output = run([binary, "--verbose"])
        lines.append(output.strip())
        lines.append("exit code: %d" % code)
    write(os.path.join(out, "publish-jit-and-trimmed.log"), "\n".join(lines) + "\n")

    print("  6 native aot")
    aot = ["=== NATIVE AOT ==="]
    if not os.path.exists(arguments.vcvars):
        aot.append("SKIPPED: no vcvars64.bat at " + arguments.vcvars)
        aot.append("Exclusion EX-42 records why this step needs one.")
    else:
        aot.append("vcvars64: " + arguments.vcvars)
        # One cmd session, three lines, written to a batch file. Chaining with && through
        # Python's own shell lost the environment vcvars64 sets, and the publish then failed
        # with exactly the MSB3073 that EX-42 describes - after which the step read the trimmed
        # binary still sitting in the shared publish directory and would have retained a Native
        # AOT result that never happened.
        aot_out = os.path.join("artifacts", "publish-aot")
        script = ('@echo off\r\n'
                  'call "%s"\r\n'
                  'set Platform=\r\n'
                  'dotnet publish %s -c Release -r win-x64 -p:PublishAot=true '
                  '-p:IlcUseEnvironmentalTools=true -o %s\r\n' % (arguments.vcvars, HOST, aot_out))
        handle, batch = tempfile.mkstemp(suffix=".bat")
        os.close(handle)
        io.open(batch, "w", encoding="ascii", newline="").write(script)
        aot.append("command: " + script.replace("\r\n", " ; ").strip())

        try:
            _, published = run(["cmd", "/c", batch])
        finally:
            os.remove(batch)

        aot.append(published.strip())
        binary = native_binary(aot_out)

        if binary is not None and os.path.getsize(binary) < 500_000:
            # A native image is over a megabyte. Anything this small is the trimmed binary or a
            # stale artefact, and reporting it as Native AOT would be a false claim.
            aot.append("")
            aot.append("REFUSED: the produced binary is %d bytes, too small to be a native image."
                       % os.path.getsize(binary))
            binary = None
        if binary:
            aot.append("")
            aot.append("--- running the native binary ---")
            aot.append("native image size: %d bytes" % os.path.getsize(binary))
            code, output = run([binary, "--verbose"])
            aot.append(output.strip())
            aot.append("exit code: %d" % code)
    write(os.path.join(out, "publish-aot.log"), "\n".join(aot) + "\n")

    if not arguments.skip_controls:
        print("  7 negative controls")
        collect_controls(out)

    print("  8 environment, hashes")
    environment = [
        "Broiler.VM evidence bundle %s - environment" % arguments.bundle,
        "",
        "OS                : %s" % platform.platform(),
        "Architecture      : %s" % platform.machine(),
        "Processors        : %s" % (os.cpu_count() or "unknown"),
        "Python            : %s" % sys.version.split()[0],
        "",
    ]
    for label, command in (("dotnet --version", ["dotnet", "--version"]),
                           ("dotnet --list-sdks", ["dotnet", "--list-sdks"]),
                           ("dotnet --list-runtimes", ["dotnet", "--list-runtimes"])):
        _, output = run(command)
        environment.append("--- %s ---" % label)
        environment.append(output.strip())
        environment.append("")
    environment.append("Not a CI lane. One developer workstation, one RID (win-x64).")
    environment.append("Exclusion EX-45 records what that does not cover.")
    write(os.path.join(out, "environment.txt"), "\n".join(environment) + "\n")

    manifest = [
        "SHA-256 of the files evidence bundle %s depends on." % arguments.bundle,
        "",
        "The vendored packaging props is hashed because ADR 0001 records it as a drift risk: it",
        "is a copy of a file this component may not edit, and its packability defaults decide",
        "whether a test-only project would pack. The assurance manifest and the generated report",
        "are hashed because they are the record of what the code was when it was measured.",
        "",
    ]
    for relative in hashed_files():
        full = os.path.join(ROOT, relative.replace("/", os.sep))
        if os.path.exists(full):
            manifest.append("%s  %s" % (hashlib.sha256(read_bytes(full)).hexdigest(), relative))
    write(os.path.join(out, "hashes.txt"), "\n".join(manifest) + "\n")

    print("done. Read the logs before writing the bundle - this script does not judge them.")


if __name__ == "__main__":
    main()
