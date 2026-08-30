"""Collect the retained evidence for a Broiler.VM milestone bundle.

Both earlier bundles named a reproduction gap this file closes. VM-0-001 said its negative
control "needs the injection script; it is not retained in the repository", and VM-1-002 said
the controls "are reproduced by the script in the change that landed this bundle" - which was
not true, because no script was landed. The controls are the reason a green suite means
anything, so the thing that produces them belongs in the repository.

    python eng/collect-evidence.py --bundle VM-2-001 --out docs/evidence/vm-2

Every step writes its own log. Nothing here decides whether the result is good: it runs the
procedure and retains what happened, including failures. Reading the result is the bundle's job.

VM-2 adds four steps and makes the file portable. The corpus is replayed by the published host
in all three modes and the three tables are compared byte for byte, which is the whole of the
cross-mode stability claim; the fuzz target runs several seeded sessions; and four controls are
added that a corpus and a fuzz target must be shown to reject, because a corpus that has never
rejected anything is a file tree and not a gate.

Portability, because VM-1 collected on win-x64 and this runs anywhere. The RID, the binary's
extension and the Native AOT invocation are all derived from the platform. On Windows the AOT
step still needs a vcvars64 shell - the ILCompiler package's own findvcvarsall.bat cannot locate
vswhere.exe on the VM-1 machine and emits its error text into the property that becomes the
linker path, so a plain -p:PublishAot=true fails with MSB3073, which exclusion EX-42 records.
Elsewhere the ordinary publish is used and no shell is needed.
"""

import argparse
import hashlib
import io
import os
import platform
import re
import subprocess
import sys
import shutil
import tempfile
import zipfile

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))


def local(*parts):
    """A repository-relative path in this platform's own separator."""
    return os.path.join(*parts)


HOST = local("src", "tests", "Broiler.VM.Fixtures.Host", "Broiler.VM.Fixtures.Host.csproj")

# The two named composition roots VM-3 publishes and runs. The register docs/compositions.md
# lists the same two, and rule K1 holds the two lists to each other.
COMPOSITIONS = (
    ("Broiler.VM.Composition.Calculator",
     local("src", "compositions", "Broiler.VM.Composition.Calculator",
           "Broiler.VM.Composition.Calculator.csproj")),
    ("Broiler.VM.Composition.Workbench",
     local("src", "compositions", "Broiler.VM.Composition.Workbench",
           "Broiler.VM.Composition.Workbench.csproj")),
)
FUZZ_HOST = local("src", "tests", "Broiler.VM.Fuzz.Host", "Broiler.VM.Fuzz.Host.csproj")
SOAK_HOST = local("src", "tests", "Broiler.VM.Soak.Host", "Broiler.VM.Soak.Host.csproj")
BENCH_HOST = local("src", "tests", "Broiler.VM.Bench.Host", "Broiler.VM.Bench.Host.csproj")

# The soak run VM-4 retains. Long enough that a plateau is a measurement rather than a snapshot,
# and short enough that a person collecting the bundle will actually wait for it.
SOAK_CYCLES = 400_000
SOAK_WORKERS = 4
SOAK_SAMPLE_EVERY = 20_000
CORPUS = local("src", "tests", "corpus", "vm-2")
SOLUTION = "Broiler.VM.slnx"
DEFAULT_VCVARS = r"C:\Program Files\Microsoft Visual Studio\18\Community\VC\Auxiliary\Build\vcvars64.bat"
WINDOWS = os.name == "nt"
DEFAULT_RID = "win-x64" if WINDOWS else ("osx-x64" if sys.platform == "darwin" else "linux-x64")
HOST_BINARY = "Broiler.VM.Fixtures.Host" + (".exe" if WINDOWS else "")
SAMPLE_DIRECTORY = os.path.join(ROOT, "samples", "Broiler.VM.Sample.FeedConsumer")

# The two versions the rollback evidence needs. The prefix matches eng/Broiler.Packaging.props;
# the suffixes are this script's, because the point is to have two package sets on one feed and
# neither of them is what a release would be called.
VERSION_PREFIX = "0.1.0"
ROLLBACK_SUFFIX = "preview.2"
CURRENT_SUFFIX = "preview.1"

# Eight sessions rather than one long one. A session is a total function of its seed, so eight
# seeds are eight independent walks of the same space, and a defect only one walk reaches is
# still reproduced by naming its seed.
FUZZ_SEEDS = (1, 2, 3, 4, 5, 6, 7, 8)
FUZZ_ITERATIONS = 250_000

# The four negative controls. Each is one edit that a named rule must reject: (file, old, new).
# A control that does not fail is a finding about the suite, not a step to retry - VM-1-002
# records the run where control 4 passed and what that exposed.
CONTROLS = [
    (
        "Broiler.VM.Runtime references the test-only Broiler.VM.Fixtures",
        local("src", "Broiler.VM.Runtime", "Broiler.VM.Runtime.csproj"),
        "</Project>",
        '  <ItemGroup>\n'
        '    <ProjectReference Include="..\\tests\\Broiler.VM.Fixtures\\Broiler.VM.Fixtures.csproj" />\n'
        '  </ItemGroup>\n\n'
        "</Project>",
    ),
    (
        "an edge the checkout HAS is deleted from graph.manifest.json",
        local("src", "tests", "Broiler.VM.Architecture.Tests", "graph.manifest.json"),
        None,  # resolved at run time: drop the first Runtime edge the manifest declares
        None,
    ),
    (
        "a retired name is exported from a product assembly",
        local("src", "Broiler.VM.Abstractions", "VmCoreContract.cs"),
        None,  # append a public type carrying a struck name
        None,
    ),
    (
        "the deterministic no-provider refusal is removed",
        local("src", "Broiler.VM.Runtime", "VmArtifactLoadMediator.cs"),
        "        if (provider is null)\n        {",
        "        if (provider is null && false)\n        {",
    ),
    (
        "the verifier reserves an allocation before reading a byte",
        local("src", "tests", "Broiler.VM.Fixtures", "FixtureVmVerifier.cs"),
        "        var reader = new VmBoundedReader(payload, in bounds, adapter);",
        "        VmBoundedAllocator.TryAllocate<long>(in bounds, adapter, 8, out _);\n"
        "        var reader = new VmBoundedReader(payload, in bounds, adapter);",
    ),
    (
        "the verifier escapes instead of answering",
        local("src", "tests", "Broiler.VM.Fixtures", "FixtureVmVerifier.cs"),
        "        if (!reader.TryReadDeclaredCount(out var sectionCount))\n"
        "        {\n"
        "            return Fail(ref reader, reader.Position);",
        "        if (!reader.TryReadDeclaredCount(out var sectionCount))\n"
        "        {\n"
        "            throw new System.InvalidOperationException(\"negative control\");",
    ),
    (
        "one declared corpus expectation is changed to a wrong reason",
        local("src", "tests", "Broiler.VM.Fixtures", "FixtureCorpus.cs"),
        "            VmReason.SemanticValidationFailed, 1006,\n"
        "            \"An opcode outside",
        "            VmReason.Truncated, 2001,\n"
        "            \"An opcode outside",
    ),
    (
        "one byte of one retained corpus artifact is changed",
        local("src", "tests", "corpus", "vm-2", "control-sum.bin"),
        None,  # resolved at run time: invert the artifact's last byte
        None,
    ),
    (
        "a composition root links the fixture profile",
        local("src", "compositions", "Broiler.VM.Composition.Calculator",
              "Broiler.VM.Composition.Calculator.csproj"),
        "  </ItemGroup>",
        '    <ProjectReference Include="..\\..\\tests\\Broiler.VM.Fixtures\\Broiler.VM.Fixtures.csproj" />\n'
        "  </ItemGroup>",
    ),
    (
        "a composition is deleted from the composition register",
        local("docs", "compositions.md"),
        "| `Broiler.VM.Composition.Workbench` | demonstration |",
        "| ~~Broiler.VM.Composition.Workbench~~ | demonstration |",
    ),
    (
        "a checked-in catalog baseline gains a profile the composition does not compose",
        local("src", "tests", "Broiler.VM.Architecture.Tests", "catalogs", "calculator.catalog.txt"),
        "profile com.example.calculator Com.Example.Calculator 1 0",
        "profile com.example.calculator Com.Example.Calculator 1 0\n"
        "profile com.example.stowaway Com.Example.Stowaway 1 0",
    ),
    (
        "disposal stops waiting for a step that is inside the profile",
        local("src", "Broiler.VM.Runtime", "VmInstanceImplementation.cs"),
        "            while (stepsInFlight > 0)",
        "            while (false && stepsInFlight > 0)",
    ),
    (
        "the declared thread affinity is no longer checked on resume",
        local("src", "Broiler.VM.Runtime", "VmRuntime.cs"),
        "            if (!operation.AffinityAdmitsCurrentThread)",
        "            if (false && !operation.AffinityAdmitsCurrentThread)",
    ),
    (
        "a disposing runtime accepts an instance registration again",
        local("src", "Broiler.VM.Runtime", "VmRuntime.cs"),
        "            if (state is not VmRuntimeState.Ready)\n            {\n                return false;",
        "            if (false)\n            {\n                return false;",
    ),
    (
        "the guest-load mediator stops resetting its per-operation counters",
        local("src", "Broiler.VM.Runtime", "VmInstanceImplementation.cs"),
        "mediator?.EnterScope(operation.Baseline, operation.ObjectId);",
        "mediator?.EnterScope(operation.Baseline, default);",
    ),
    (
        "a runtime stores a capability depth of zero instead of releasing it",
        local("src", "Broiler.VM.Runtime", "VmRuntime.cs"),
        "            inCapabilityDepth.Value = depth > 0 ? depth : null;",
        "            inCapabilityDepth.Value = depth;",
    ),
    (
        "the baseline register quotes a figure the retained log contradicts",
        local("docs", "baselines.md"),
        None,  # resolved at run time: change the first lane figure of the first measurement row
        None,
    ),
    (
        "a public member is added without the API baseline being regenerated",
        local("src", "Broiler.VM.Abstractions", "VmCoreContract.cs"),
        None,  # resolved at run time: append a public type the baseline does not declare
        None,
    ),
    (
        "the pristine feed consumer gains a project reference into the repository",
        local("samples", "Broiler.VM.Sample.FeedConsumer", "Broiler.VM.Sample.FeedConsumer.csproj"),
        "  <ItemGroup>",
        '  <ItemGroup>\n'
        '    <ProjectReference Include="..\\..\\src\\Broiler.VM.Runtime\\Broiler.VM.Runtime.csproj" />\n'
        '  </ItemGroup>\n\n'
        "  <ItemGroup>",
    ),
]


def run(command, shell=False, cwd=None):
    """
    Run a command and return (exit code, combined output).

    From the component root unless a directory is named. The samples need to run from their own
    directory and not from here: their NuGet.config and their Directory.Build.props are found by
    walking up from the working directory, and running them from the root would silently give them
    the component's sources and the component's build properties - which is the one thing a
    pristine consumer must not have.
    """
    environment = dict(os.environ, DOTNET_CLI_UI_LANGUAGE="en")
    completed = subprocess.run(
        command,
        cwd=cwd or ROOT,
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
    exe = os.path.join(ROOT, publish_directory, HOST_BINARY)
    return exe if os.path.exists(exe) else None


def published_binary(publish_directory, assembly_name):
    """The executable a publish of `assembly_name` produced, or None."""
    exe = os.path.join(ROOT, publish_directory, assembly_name + (".exe" if WINDOWS else ""))
    return exe if os.path.exists(exe) else None


def publish(project, out_directory, arguments, aot):
    """Publish one project, trimmed or Native AOT, and return the combined output."""
    if not aot:
        return run(["dotnet", "publish", project, "-c", "Release", "-r", arguments.rid,
                    "--self-contained", "true", "-p:PublishAot=false", "-p:PublishTrimmed=true",
                    "-o", out_directory])

    if not WINDOWS:
        return run(["dotnet", "publish", project, "-c", "Release", "-r", arguments.rid,
                    "-p:PublishAot=true", "-o", out_directory])

    if not os.path.exists(arguments.vcvars):
        return 1, "SKIPPED: no vcvars64.bat at " + arguments.vcvars

    # The same one-session batch file the fixtures host needs, for the same reason: chaining
    # through Python's own shell loses the environment vcvars64 sets, and the publish then fails
    # with the MSB3073 exclusion EX-42 describes.
    script = ('@echo off\r\n'
              'call "%s"\r\n'
              'set Platform=\r\n'
              'dotnet publish %s -c Release -r %s -p:PublishAot=true '
              '-p:IlcUseEnvironmentalTools=true -o %s\r\n'
              % (arguments.vcvars, project, arguments.rid, out_directory))
    handle, batch = tempfile.mkstemp(suffix=".bat")
    os.close(handle)
    io.open(batch, "w", encoding="ascii", newline="").write(script)

    try:
        return run(["cmd", "/c", batch])
    finally:
        os.remove(batch)


def closure_of(publish_directory):
    """
    The non-framework assemblies a published directory contains.

    This is the closure report VM-3's gate asks for, and it is read off the published output
    rather than derived from the project file. A reference set that looks right can still pull
    something in through a transitive package, and the linker is the only party that knows what
    actually shipped.

    Framework assemblies are excluded by prefix because a self-contained publish carries the
    whole runtime and the claim is about what the COMPOSITION contributes. The prefixes are
    deliberately coarse - anything System.* or Microsoft.* - which cannot hide a fixture, a test
    framework or a profile, since none of those is named that way.
    """
    directory = os.path.join(ROOT, publish_directory)

    if not os.path.isdir(directory):
        return []

    return sorted(
        name[:-4] for name in os.listdir(directory)
        if name.endswith(".dll")
        and not name.startswith("System.")
        and not name.startswith("Microsoft.")
        and name not in ("netstandard.dll", "mscorlib.dll", "WindowsBase.dll"))


def collect_compositions(arguments, out):
    """
    Publish and run each named composition in three modes, and retain what it composed.

    Three artefacts per composition. The transcript says the checks passed in every mode; the
    catalog table says which profiles were composed, compared across modes byte for byte so a
    difference between JIT and Native AOT is a failure rather than a footnote; and the closure
    report lists what the published image actually contains.
    """
    for name, project in COMPOSITIONS:
        slug = name.split(".")[-1].lower()
        lines = ["=== COMPOSITION %s ===" % name, "project: " + project, ""]
        catalogs = {}

        lines.append("--- JIT: dotnet run -- --verbose ---")
        code, output = run(["dotnet", "run", "--project", project, "-c", "Release",
                            "--", "--verbose"])
        lines.append(output.strip())
        lines.append("exit code: %d" % code)

        _, catalog = run(["dotnet", "run", "--project", project, "-c", "Release",
                          "--", "--closure"])
        catalogs["jit"] = catalog.strip()

        trimmed_out = os.path.join("artifacts", "publish-" + slug + "-trimmed")
        aot_out = os.path.join("artifacts", "publish-" + slug + "-aot")

        for mode, directory, is_aot in (("trimmed", trimmed_out, False), ("aot", aot_out, True)):
            lines.append("")
            lines.append("--- %s: dotnet publish -r %s ---" % (mode.upper(), arguments.rid))
            _, published = publish(project, directory, arguments, aot=is_aot)
            lines.append(published.strip())

            binary = published_binary(directory, name)

            if is_aot and binary is not None and os.path.getsize(binary) < 500_000:
                # A native image is over a megabyte. Anything this small is a trimmed binary or a
                # stale artefact, and reporting it as Native AOT would be a false claim.
                lines.append("REFUSED: the produced binary is %d bytes, too small to be a native image."
                             % os.path.getsize(binary))
                binary = None

            if binary is None:
                lines.append("no binary to run in this mode")
                continue

            # The same two spellings the fixtures-host steps use, because rule H5 reads the logs
            # by them: "native image size" for an AOT image and "image size" for a trimmed one.
            lines.append("%simage size: %d bytes"
                         % ("native " if is_aot else "", os.path.getsize(binary)))
            code, output = run([binary, "--verbose"])
            lines.append(output.strip())
            lines.append("exit code: %d" % code)

            _, catalog = run([binary, "--closure"])
            catalogs[mode] = catalog.strip()

        lines.append("")
        lines.append("--- catalog table, compared across modes ---")

        for mode in ("jit", "trimmed", "aot"):
            lines.append("[%s]" % mode)
            lines.append(catalogs.get(mode, "MISSING"))

        distinct = set(catalogs.values())
        lines.append("")
        lines.append("modes captured: %d; identical: %s"
                     % (len(catalogs), "yes" if len(distinct) == 1 else "NO"))

        write(os.path.join(out, "composition-%s.log" % slug), "\n".join(lines) + "\n")

        # The catalog table itself, retained on its own so a rule can compare it against the
        # checked-in baseline without parsing a transcript.
        write(os.path.join(out, "catalog-%s.txt" % slug),
              (catalogs.get("jit", "") + "\n") if catalogs else "\n")

        report = ["# closure %s rid=%s" % (name, arguments.rid), ""]

        for mode, directory in (("trimmed", trimmed_out), ("aot", aot_out)):
            assemblies = closure_of(directory)
            report.append("[%s] %d non-framework assemblies" % (mode, len(assemblies)))
            report += assemblies
            report.append("")

        write(os.path.join(out, "closure-%s.txt" % slug), "\n".join(report).strip() + "\n")


def soak_run(arguments):
    """
    Publish the soak host and run it, retaining every sample it printed.

    Published rather than run from source, for the reason the host exists at all: a plateau is a
    property of the image a host would actually ship, and a JIT run under `dotnet run` measures a
    process that also contains the SDK's own host. The trimmed self-contained publish is what is
    measured; the JIT run is retained beside it so the two can be compared.
    """
    lines = ["=== SOAK: %d cycles across %d workers ===" % (SOAK_CYCLES, SOAK_WORKERS)]

    arguments_for_run = ["--cycles", str(SOAK_CYCLES),
                         "--workers", str(SOAK_WORKERS),
                         "--sample-every", str(SOAK_SAMPLE_EVERY)]

    lines.append("")
    lines.append("--- JIT ---")
    code, output = run(["dotnet", "run", "--project", SOAK_HOST, "-c", "Release", "--"] + arguments_for_run)
    lines.append(output.strip())
    lines.append("exit code: %d" % code)

    out_directory = os.path.join("artifacts", "publish-soak-trimmed")

    lines.append("")
    lines.append("--- TRIMMED: dotnet publish -r %s ---" % arguments.rid)
    _, published = publish(SOAK_HOST, out_directory, arguments, aot=False)
    lines.append(published.strip())

    binary = published_binary(out_directory, "Broiler.VM.Soak.Host")

    if binary is None:
        lines.append("no binary to run")
        return "\n".join(lines) + "\n"

    lines.append("image size: %d bytes" % os.path.getsize(binary))
    code, output = run([binary] + arguments_for_run)
    lines.append(output.strip())
    lines.append("exit code: %d" % code)

    return "\n".join(lines) + "\n"


def bench_run(arguments):
    """
    Run the baselines on JIT and on Native AOT, retaining every repetition of both.

    Two lanes because the gate asks for two, and they answer different questions. The JIT lane is
    what a host running from a framework-dependent deployment pays; the Native AOT lane is what a
    published single-file image pays, and it is the one whose startup figure means anything - the
    JIT lane's includes the SDK host that launched it.

    Nothing here decides whether a figure is acceptable. The host exits non-zero if any A/A lane
    exceeded its effect, and that exit code is retained rather than acted on: a measurement the
    harness refused to publish is a fact about the run, and the bundle's job is to record it.
    """
    # Retained unless re-measuring was asked for, and that default is load-bearing. Rule L1 binds
    # docs/baselines.md to this log by value, and a benchmark - unlike a test count or an image
    # size - produces different numbers every run. Re-measuring on every collection would leave the
    # register permanently one collection behind its own log, with no state in which both are true.
    # So the figures move when someone decides to move them, and the register moves in the same
    # change. Pass --rebench to re-measure.
    existing = os.path.join(ROOT, arguments.out, "bench.log")

    if os.path.exists(existing) and not arguments.rebench:
        print("      retained: pass --rebench to re-measure")
        return io.open(existing, encoding="utf-8").read()

    lines = ["=== BENCH: baselines on JIT and Native AOT ==="]

    lines.append("")
    lines.append("--- JIT: dotnet run ---")
    code, output = run(["dotnet", "run", "--project", BENCH_HOST, "-c", "Release"])
    lines.append(output.strip())
    lines.append("exit code: %d" % code)

    out_directory = os.path.join("artifacts", "publish-bench-aot")

    lines.append("")
    lines.append("--- NATIVE AOT: dotnet publish -r %s -p:PublishAot=true ---" % arguments.rid)
    _, published = publish(BENCH_HOST, out_directory, arguments, aot=True)
    lines.append(published.strip())

    binary = published_binary(out_directory, "Broiler.VM.Bench.Host")

    if binary is None:
        lines.append("no native binary to run")
        return "\n".join(lines) + "\n"

    lines.append("")
    lines.append("--- running the native binary ---")
    lines.append("native image size: %d bytes" % os.path.getsize(binary))
    code, output = run([binary])
    lines.append(output.strip())
    lines.append("exit code: %d" % code)

    return "\n".join(lines) + "\n"


def nuspecs(directory):
    """Every .nuspec inside every .nupkg the pack produced, concatenated in a readable order."""
    lines = []

    for name in sorted(os.listdir(directory)):
        if not name.endswith(".nupkg"):
            continue

        with zipfile.ZipFile(os.path.join(directory, name)) as archive:
            for entry in sorted(archive.namelist()):
                if not entry.endswith(".nuspec"):
                    continue

                lines.append("=== %s :: %s ===" % (name, entry))
                lines.append(archive.read(entry).decode("utf-8").strip())
                lines.append("")

    return "\n".join(lines).strip() + "\n"


def feed_consumer(arguments):
    """
    Pack to a local feed, restore a consumer that has no project reference, run it, and roll back.

    The whole VM-6 packaging claim in one procedure. The consumer's NuGet.config lists ONE source
    and it is this feed - nuget.org is not reachable from it - so a restore that succeeds is also
    evidence that the three packages depend on nothing, and one that fails says so loudly rather
    than resolving something from the internet and looking fine.

    Two versions are packed because rollback has to be exercised rather than described. The
    consumer prints the informational version of each assembly it actually loaded, so the
    transcript shows which package set answered rather than which one was asked for.
    """
    feed = os.path.join(ROOT, "artifacts", "feed")
    lines = ["=== FEED CONSUMER: restore from a feed, run, and roll back ==="]

    if os.path.isdir(feed):
        shutil.rmtree(feed)

    os.makedirs(feed)

    for version in (CURRENT_SUFFIX, ROLLBACK_SUFFIX):
        lines.append("")
        lines.append("--- pack %s-%s to the feed ---" % (VERSION_PREFIX, version))
        code, output = run(["dotnet", "pack", SOLUTION, "-c", "Release", "-o", feed,
                            "-p:VersionSuffix=" + version])
        lines.append(output.strip())
        lines.append("exit code: %d" % code)

    lines.append("")
    lines.append("--- feed contents ---")
    lines.extend(sorted(os.listdir(feed)))

    # Newest first, then the rollback. The order is the point: a rollback that was never rolled
    # forward from proves nothing.
    for label, version in (("restore and run", ROLLBACK_SUFFIX), ("ROLL BACK and run", CURRENT_SUFFIX)):
        full = "%s-%s" % (VERSION_PREFIX, version)

        lines.append("")
        lines.append("--- %s against %s ---" % (label, full))
        code, output = run(["dotnet", "run", "-c", "Release", "-p:BroilerVmVersion=" + full],
                           cwd=SAMPLE_DIRECTORY)
        lines.append(output.strip())
        lines.append("exit code: %d" % code)

    lines.append("")
    lines.append("--- publish the consumer as Native AOT and run it ---")
    published = os.path.join("artifacts", "publish-sample-aot")
    full = "%s-%s" % (VERSION_PREFIX, ROLLBACK_SUFFIX)

    code, output = run(["dotnet", "publish", "-c", "Release", "-r", arguments.rid,
                        "-p:PublishAot=true", "-p:BroilerVmVersion=" + full,
                        "-o", os.path.join(ROOT, published)], cwd=SAMPLE_DIRECTORY)
    lines.append(output.strip())

    binary = published_binary(published, "Broiler.VM.Sample.FeedConsumer")

    if binary is None:
        lines.append("no native binary to run")
        return "\n".join(lines) + "\n"

    lines.append("native image size: %d bytes" % os.path.getsize(binary))
    code, output = run([binary])
    lines.append(output.strip())
    lines.append("exit code: %d" % code)

    return "\n".join(lines) + "\n"


def collect_controls(out):
    """Inject, run, revert, re-run. Both runs are retained for every control."""
    log = []

    for index, (name, relative, old, new) in enumerate(CONTROLS, start=1):
        path = os.path.join(ROOT, relative)
        original = read_bytes(path)

        if relative.endswith(".bin"):
            # A corpus artifact is bytes and has no injection point to match. Inverting one byte
            # is the whole control: the manifest records a hash, and a file that no longer hashes
            # to it is a case nothing can cite any more.
            mutated_bytes = bytearray(original)
            mutated_bytes[-1] ^= 0xFF

            log.append("=== CONTROL %d: %s ===" % (index, name))
            log.append("file: " + relative)
            io.open(path, "wb").write(bytes(mutated_bytes))
            try:
                _, injected = run(["dotnet", "test", SOLUTION, "-c", "Release"])
            finally:
                restore(path, original)

            log.append(injected.strip())
            log.append("")
            log.append("--- after revert ---")
            _, reverted = run(["dotnet", "test", SOLUTION, "-c", "Release"])
            log.append(reverted.strip())
            log.append("")

            print("  control %d: %s when injected, %s after revert"
                  % (index,
                     "FAILED" if "Failed!" in injected else "PASSED - a finding about the suite",
                     "green" if "Failed!" not in reverted else "STILL RED"))
            continue

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
        elif relative.endswith("VmCoreContract.cs") and "public sealed class VmHandle" not in text:
            # A public type the API baseline does not declare. Rule M1 must see an ADDITION, which
            # is the direction that matters for a frozen surface: a member reaches a package
            # without anyone deciding it should. Struck names are control 3's job, so this one uses
            # a name nothing else objects to.
            added = "\n\npublic sealed class VmAddedWithoutADecision\n{\n}\n"
            mutated = text.rstrip() + (added.replace("\n", "\r\n") if crlf else added)
        elif relative.endswith("baselines.md"):
            # The register's figures are rewritten every time the bench is re-run, so an injection
            # point written as a literal would go stale with the first collection that moved a
            # number. The first measurement row's JIT figure is found by shape instead, and the
            # digit before the decimal point is changed - a difference rule L1 must see and a
            # reader might not.
            match = re.search(r"(\r?\n\|\s*`[a-z][a-z0-9-]*`\s*\|[^\r\n]*\|\s*)(\d+)(\.\d+\s*\|)", text)
            assert match, "no measurement row to falsify in the baseline register"
            mutated = (text[: match.start()] + match.group(1) +
                       str(int(match.group(2)) + 1) + match.group(3) + text[match.end():])
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


def replay_corpus(arguments, trimmed_out, aot_out):
    """Replay the corpus from each published binary and compare the three tables byte for byte.

    This is the whole of the cross-mode stability claim, and it is a claim about published
    binaries rather than about a test run: an enumeration rendered by name, a switch the linker
    reshaped, a generic instantiation the AOT compiler could not see - each of those changes what
    a host is told an artifact was, and none of them is visible to a suite running under the JIT.
    """
    corpus = os.path.join(ROOT, CORPUS)
    lines = ["=== CORPUS REPLAY: one table per publish mode, compared byte for byte ==="]
    tables = {}

    # The JIT row runs from the built assemblies rather than a published directory, which is what
    # "JIT" means here: the same IL the suite runs, with nothing removed and nothing compiled ahead.
    code, jit = run(["dotnet", "run", "--project", HOST, "-c", "Release", "--", "--corpus", corpus])
    tables["jit"] = jit
    lines.append("--- jit (dotnet run) exit=%d ---" % code)

    for mode, directory in (("trimmed", trimmed_out), ("aot", aot_out)):
        binary = native_binary(directory)

        if binary is None:
            lines.append("--- %s: SKIPPED, no published binary ---" % mode)
            continue

        code, output = run([binary, "--corpus", corpus])
        tables[mode] = output
        lines.append("--- %s exit=%d ---" % (mode, code))

    for mode, table in sorted(tables.items()):
        digest = hashlib.sha256(table.encode("utf-8")).hexdigest()
        rows = len([line for line in table.splitlines() if line and not line.startswith("#")])
        lines.append("%-8s rows=%-4d sha256=%s" % (mode, rows, digest))

    digests = {hashlib.sha256(table.encode("utf-8")).hexdigest() for table in tables.values()}

    lines.append("")
    lines.append("modes compared: %d" % len(tables))
    lines.append("distinct tables: %d" % len(digests))
    lines.append("IDENTICAL" if len(digests) == 1 and len(tables) > 1
                 else "NOT IDENTICAL - the failure classes differ between publish modes")
    lines.append("")
    lines.append("--- the table, as every mode produced it ---")
    lines.append(sorted(tables.items())[0][1].strip() if tables else "(no table)")

    return "\n".join(lines) + "\n"


def fuzz_sessions():
    """Run the seeded fuzz sessions and retain every histogram, including a session that found one."""
    lines = ["=== FUZZ: %d iterations per seed, seeds %s ==="
             % (FUZZ_ITERATIONS, ", ".join(str(seed) for seed in FUZZ_SEEDS))]

    findings = 0

    for seed in FUZZ_SEEDS:
        code, output = run(["dotnet", "run", "--project", FUZZ_HOST, "-c", "Release", "--",
                            "--iterations", str(FUZZ_ITERATIONS), "--seed", str(seed)])
        lines.append("--- seed %d exit=%d ---" % (seed, code))
        lines.append(output.strip())
        lines.append("")

        if code != 0:
            findings += 1

    lines.append("sessions: %d" % len(FUZZ_SEEDS))
    lines.append("iterations: %d" % (len(FUZZ_SEEDS) * FUZZ_ITERATIONS))
    lines.append("sessions reporting a finding: %d" % findings)

    return "\n".join(lines) + "\n"


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

    # The composition register and the catalog baselines rules K1 to K4 read. A bundle that pinned
    # the closure reports but not the baselines they are compared against would pin one side of a
    # comparison, which is the shape the corpus note below already warns about.
    files.append("docs/compositions.md")

    catalogs = os.path.join(ROOT, "src", "tests", "Broiler.VM.Architecture.Tests", "catalogs")
    if os.path.isdir(catalogs):
        files += ["src/tests/Broiler.VM.Architecture.Tests/catalogs/" + name
                  for name in sorted(os.listdir(catalogs)) if name.endswith(".txt")]

    # The corpus, every artifact of it. A bundle that hashed the manifest and not the files it
    # names would pin a description of the corpus rather than the corpus, and a minimized fuzz
    # regression has no declaration anywhere else to be checked against.
    corpus = os.path.join(ROOT, CORPUS)
    if os.path.isdir(corpus):
        files.append("src/tests/corpus/vm-2/manifest.json")
        files += ["src/tests/corpus/vm-2/" + name
                  for name in sorted(os.listdir(corpus)) if name.endswith(".bin")]

    return [f.replace(os.sep, "/") for f in files]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--bundle", required=True)
    parser.add_argument("--out", required=True)
    parser.add_argument("--vcvars", default=DEFAULT_VCVARS)
    parser.add_argument("--rid", default=DEFAULT_RID)
    parser.add_argument("--skip-controls", action="store_true")
    parser.add_argument("--rebench", action="store_true")
    arguments = parser.parse_args()

    out = os.path.join(ROOT, arguments.out)
    os.makedirs(out, exist_ok=True)

    print("collecting %s into %s" % (arguments.bundle, arguments.out))

    # --no-incremental is not a preference. A warning is emitted by the COMPILER, so a project
    # MSBuild considers up to date is not recompiled and its warnings are not re-emitted: an
    # incremental build over a warm tree writes a log that says 0 warnings because nothing was
    # compiled, not because nothing was wrong. Bundle VM-6-001 retained exactly that log while a
    # cold build of the same commit produced nineteen. -warnaserror is what CI runs, so the
    # collection and the lane now agree on what a clean build means.
    print("  1 build")
    _, build = run(["dotnet", "build", SOLUTION, "-c", "Release", "--no-incremental", "-warnaserror"])
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

    # Every .nuspec, extracted and retained. Rules C2 and C3 read declared dependencies and
    # package text, and neither is in the pack transcript: a log says a package was created and
    # says nothing about what it promises. The manifests are small, they are the actual contract a
    # consumer's restore resolves against, and retaining them makes those two rules assertable
    # against evidence rather than against a build that has already been thrown away.
    write(os.path.join(out, "nuspecs.txt"), nuspecs(packed))

    print("  4-5 jit and trimmed")
    lines = ["=== JIT: dotnet run --project %s -c Release -- --verbose ===" % HOST]
    _, jit = run(["dotnet", "run", "--project", HOST, "-c", "Release", "--", "--verbose"])
    lines.append(jit.strip())

    lines.append("")
    lines.append("=== TRIMMED: dotnet publish -r %s --self-contained true -p:PublishAot=false ==="
                 % arguments.rid)
    trimmed_out = os.path.join("artifacts", "publish-trimmed")
    _, trimmed = run(["dotnet", "publish", HOST, "-c", "Release", "-r", arguments.rid,
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
    aot_out = os.path.join("artifacts", "publish-aot")
    aot = ["=== NATIVE AOT ==="]

    if not WINDOWS:
        # No shell to prepare. The toolchain discovery that EX-42 records is a Windows-only
        # problem, so this platform publishes with the ordinary command and the exclusion does
        # not apply to the result.
        command = ["dotnet", "publish", HOST, "-c", "Release", "-r", arguments.rid,
                   "-p:PublishAot=true", "-o", aot_out]
        aot.append("command: " + " ".join(command))
        _, published = run(command)
        aot.append(published.strip())

        binary = native_binary(aot_out)

        if binary is not None and os.path.getsize(binary) < 500_000:
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
        aot = None
    elif not os.path.exists(arguments.vcvars):
        aot.append("SKIPPED: no vcvars64.bat at " + arguments.vcvars)
        aot.append("Exclusion EX-42 records why this step needs one.")
    else:
        aot.append("vcvars64: " + arguments.vcvars)
        # One cmd session, three lines, written to a batch file. Chaining with && through
        # Python's own shell lost the environment vcvars64 sets, and the publish then failed
        # with exactly the MSB3073 that EX-42 describes - after which the step read the trimmed
        # binary still sitting in the shared publish directory and would have retained a Native
        # AOT result that never happened.
        script = ('@echo off\r\n'
                  'call "%s"\r\n'
                  'set Platform=\r\n'
                  'dotnet publish %s -c Release -r %s -p:PublishAot=true '
                  '-p:IlcUseEnvironmentalTools=true -o %s\r\n'
                  % (arguments.vcvars, HOST, arguments.rid, aot_out))
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

    if aot is not None:
        write(os.path.join(out, "publish-aot.log"), "\n".join(aot) + "\n")

    print("  7 corpus replay in three modes")
    write(os.path.join(out, "corpus-replay.log"), replay_corpus(arguments, trimmed_out, aot_out))

    print("  8 fuzz sessions")
    write(os.path.join(out, "fuzz.log"), fuzz_sessions())

    print("  8b compositions: publish, run and report the closure of each")
    collect_compositions(arguments, out)

    print("  8c soak: a long lifecycle run, sampled")
    write(os.path.join(out, "soak.log"), soak_run(arguments))

    print("  8d bench: the baselines, on JIT and on Native AOT")
    write(os.path.join(out, "bench.log"), bench_run(arguments))

    print("  8e feed consumer: pack, restore without a project reference, run, roll back")
    write(os.path.join(out, "feed-consumer.log"), feed_consumer(arguments))

    if not arguments.skip_controls:
        print("  9 negative controls")
        collect_controls(out)

    print("  10 environment, hashes")
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
    environment.append("Not a CI lane. One machine, one RID (%s)." % arguments.rid)
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
