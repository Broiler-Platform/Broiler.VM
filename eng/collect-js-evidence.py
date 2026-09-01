"""Collect the retained evidence for a Broiler.VM.Profile.JavaScript milestone bundle.

    python eng/collect-js-evidence.py --bundle JS-0-001 \
        --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-0

The JavaScript profile is a family of product projects inside this component (decision JSD-0001),
but its EVIDENCE is its own: a JS bundle is cited only by the profile's status ledger and a core
bundle only by the core's, because a result that can be read from either ledger is a result that
proves whichever claim a reader wanted. Decision JSD-0006 records why this is a script of its own
rather than a flag on eng/collect-evidence.py - that script publishes composition roots, replays
a corpus and runs three hosts, none of which this profile has at JS-0, and adding a mode would
have made a collection script's behaviour conditional on who was collecting.

Nothing here decides whether the result is good. It runs the procedure and retains what happened,
including failures. Reading the result is the bundle's job.

Every negative control is an injection into the real checkout, followed by a revert. A control
that fails to revert stops the run loudly rather than leaving the tree modified.
"""

import argparse
import datetime
import hashlib
import io
import os
import platform
import shutil
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SOLUTION = "Broiler.VM.slnx"

PROFILE_ROOT = os.path.join("src", "Broiler.VM.Profile.JavaScript")
PROFILE_PROJECT = os.path.join(PROFILE_ROOT, "Broiler.VM.Profile.JavaScript.csproj")
FORMAT_PROJECT = os.path.join(
    "src", "Broiler.VM.Profile.JavaScript.Format", "Broiler.VM.Profile.JavaScript.Format.csproj")
COMPILER_PROJECT = os.path.join(
    "src", "Broiler.VM.Profile.JavaScript.Compiler", "Broiler.VM.Profile.JavaScript.Compiler.csproj")
PROFILE_MARKER = os.path.join(PROFILE_ROOT, "AssemblyMarker.cs")
PROFILE_VALUE = os.path.join(PROFILE_ROOT, "JavaScriptValue.cs")
PROFILE_EXECUTOR = os.path.join(PROFILE_ROOT, "JavaScriptExecutor.cs")
PROFILE_VERIFIER = os.path.join(PROFILE_ROOT, "JavaScriptVerifier.cs")
PROFILE_POSITION = os.path.join(PROFILE_ROOT, "JavaScriptPosition.cs")
FORMAT_SOURCE = os.path.join(
    "src", "Broiler.VM.Profile.JavaScript.Format", "JavaScriptFormat.cs")
PROFILE_API_BASELINE = os.path.join(PROFILE_ROOT, "docs", "api", "public-api.txt")
REGISTRY = os.path.join(PROFILE_ROOT, "docs", "diagnostics", "registry.txt")
LEDGER = os.path.join(PROFILE_ROOT, "docs", "roadmap.status.md")
MIRROR = os.path.join(
    "src", "compositions", "Broiler.VM.Composition.JavaScript.SliceCompiler", "CorpusBuilder.cs")
FIXTURES_PROJECT = os.path.join(
    "src", "tests", "Broiler.VM.Fixtures", "Broiler.VM.Fixtures.csproj")

# The two composition roots the register lists for this profile, with the slug rules K3 and K4 use
# to find their retained artefacts: the last dot-separated segment, lowercased.
EXECUTION_ONLY = os.path.join(
    "src", "compositions", "Broiler.VM.Composition.JavaScript.ExecutionOnly",
    "Broiler.VM.Composition.JavaScript.ExecutionOnly.csproj")
SLICE_COMPILER = os.path.join(
    "src", "compositions", "Broiler.VM.Composition.JavaScript.SliceCompiler",
    "Broiler.VM.Composition.JavaScript.SliceCompiler.csproj")
COMPOSITIONS = (
    ("executiononly", "Broiler.VM.Composition.JavaScript.ExecutionOnly", EXECUTION_ONLY),
    ("slicecompiler", "Broiler.VM.Composition.JavaScript.SliceCompiler", SLICE_COMPILER),
)

# Native AOT on Windows needs vswhere.exe on PATH. The ILCompiler package's own findvcvarsall.bat
# calls it unqualified, and when it is missing the batch file ERROR TEXT is substituted into the
# property that becomes the linker path - so the publish fails with MSB3073 naming a command that
# looks like a sentence. The core exclusion EX-42 records this as needing a vcvars64 shell; it does
# not. It needs this directory on PATH, which is a narrower and more useful statement.
VS_INSTALLER = "C:\\Program Files (x86)\\Microsoft Visual Studio\\Installer"

# The candidate snapshot identity JSD-0005 records. The script re-derives the four revisions from
# an aggregate checkout and compares; it does not take a snapshot and does not judge the result.
SEED_CANDIDATE = {
    "Broiler.JS": "0341e5c98553b43569217aa7a30c8a01a1eada0c",
    "Broiler.JS/Broiler.DateTime": "d0c036783bdeeedaeb657a69bea6e2d5f5d438e9",
    "Broiler.JS/Broiler.Regex": "4df3fb8e005d9688921c235ccc44e2e89746180e",
    "Broiler.JS/Broiler.Unicode": "151799bb010bd8c882e07bace636ed12197c3410",
}


def run(command, cwd=None):
    """Run a command and return (exit code, combined output). Never raises on a non-zero exit."""
    completed = subprocess.run(
        command,
        cwd=cwd or ROOT,
        capture_output=True,
        text=True,
        encoding="utf-8",
        errors="replace",
        shell=False)

    return completed.returncode, (completed.stdout or "") + (completed.stderr or "")


def write(path, text):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with io.open(path, "w", encoding="utf-8", newline="\n") as handle:
        handle.write(text)


def read(path):
    with io.open(os.path.join(ROOT, path), encoding="utf-8", newline="") as handle:
        return handle.read()


def overwrite(path, text):
    with io.open(os.path.join(ROOT, path), "w", encoding="utf-8", newline="") as handle:
        handle.write(text)


def read_bytes(path):
    with io.open(os.path.join(ROOT, path), "rb") as handle:
        return handle.read()


def overwrite_bytes(path, payload):
    with io.open(os.path.join(ROOT, path), "wb") as handle:
        handle.write(payload)


def suite():
    """Build and run the whole solution. The gate is the suite, so a control is judged by it."""
    return run([
        "dotnet", "test", SOLUTION, "-c", "Release", "--nologo",
        "-p:TreatWarningsAsErrors=true"])


def identity(arguments, out):
    now = datetime.datetime.now(datetime.timezone.utc).isoformat(timespec="seconds")

    _, commit = run(["git", "rev-parse", "HEAD"])
    _, branch = run(["git", "rev-parse", "--abbrev-ref", "HEAD"])
    _, status = run(["git", "status", "--porcelain"])

    dirty = [line for line in status.splitlines() if line.strip()]

    lines = [
        f"bundle:                {arguments.bundle}",
        f"milestone:             {arguments.milestone}",
        f"collected (UTC):       {now}",
        f"component commit:      {commit.strip()}",
        f"branch:                {branch.strip()}",
        f"working tree:          {'DIRTY' if dirty else 'clean'}",
        f"dirty entries:         {len(dirty)}",
        f"owner:                 {arguments.owner}",
        f"reviewer:              {arguments.reviewer}",
        "",
        "Every path below is relative to the component root. A figure in this bundle is this",
        "component's own; no result from any other component is evidence here.",
        "",
    ]

    if dirty:
        lines.append("Dirty entries at collection time, listed rather than summarised:")
        lines.extend(f"  {entry}" for entry in dirty)
        lines.append("")

    write(os.path.join(out, "identity.txt"), "\n".join(lines))
    return dirty


def environment(out):
    code, info = run(["dotnet", "--info"])

    text = "\n".join([
        f"os:                    {platform.platform()}",
        f"machine:               {platform.machine()}",
        f"python:                {platform.python_version()}",
        f"dotnet --info exit:    {code}",
        "",
        info,
    ])

    write(os.path.join(out, "environment.txt"), text)


def snapshot_identity(out):
    """Re-derive JSD-0005's candidate seed identity from an aggregate checkout, if there is one."""
    aggregate = os.path.dirname(ROOT)
    lines = [
        "JSD-0005 records a CANDIDATE snapshot identity. This step re-derives it and compares.",
        "A match is not a taken snapshot: JS-2 takes one and records what it actually took.",
        "",
        f"aggregate checkout:    {aggregate}",
        "",
    ]

    if not os.path.exists(os.path.join(aggregate, ".gitmodules")):
        lines.append(
            "INCONCLUSIVE - no aggregate checkout above the component root, so the seed's "
            "revisions could not be read. This is not a match and not a mismatch.")
        write(os.path.join(out, "snapshot-identity.txt"), "\n".join(lines) + "\n")
        return

    mismatches = 0

    for path, expected in sorted(SEED_CANDIDATE.items()):
        parent = aggregate if "/" not in path else os.path.join(aggregate, os.path.dirname(path))
        name = os.path.basename(path)

        code, output = run(["git", "ls-files", "-s", name], cwd=parent)
        actual = output.split()[1] if code == 0 and len(output.split()) > 1 else "<unreadable>"

        verdict = "match" if actual == expected else "MISMATCH"
        mismatches += 0 if verdict == "match" else 1

        lines.append(f"{path}")
        lines.append(f"  recorded: {expected}")
        lines.append(f"  checkout: {actual}   [{verdict}]")

    lines.append("")
    lines.append(f"mismatches: {mismatches}")

    write(os.path.join(out, "snapshot-identity.txt"), "\n".join(lines) + "\n")


CONTROLS = [
    (
        "N1-profile-references-the-runtime",
        "The profile assembly is given an edge to Broiler.VM.Runtime. ADR 0011 P1 forbids it and "
        "rule N1 must report it; rule A7 must also report the edge the graph manifest does not have.",
        PROFILE_PROJECT,
        lambda text: text.replace(
            '    <ProjectReference Include="..\\Broiler.VM.Binary\\Broiler.VM.Binary.csproj" />',
            '    <ProjectReference Include="..\\Broiler.VM.Binary\\Broiler.VM.Binary.csproj" />\n'
            '    <ProjectReference Include="..\\Broiler.VM.Runtime\\Broiler.VM.Runtime.csproj" />'),
    ),
    (
        "N1-profile-references-the-lowering",
        "The profile assembly is given an edge to its own lowering. This is the violation the "
        "execution-only composition label exists to exclude, and N1 names it in its own words.",
        PROFILE_PROJECT,
        lambda text: text.replace(
            '    <ProjectReference Include="..\\Broiler.VM.Binary\\Broiler.VM.Binary.csproj" />',
            '    <ProjectReference Include="..\\Broiler.VM.Binary\\Broiler.VM.Binary.csproj" />\n'
            '    <ProjectReference Include="..\\Broiler.VM.Profile.JavaScript.Compiler'
            '\\Broiler.VM.Profile.JavaScript.Compiler.csproj" />'),
    ),
    (
        "N3-format-is-not-a-sink",
        "The format assembly is given one edge. It is the pivot two consumers depend on, so a "
        "single edge out of it puts one consumer on the other's graph.",
        FORMAT_PROJECT,
        lambda text: text.replace(
            "</Project>",
            '  <ItemGroup>\n'
            '    <ProjectReference Include="..\\Broiler.VM.Abstractions'
            '\\Broiler.VM.Abstractions.csproj" />\n'
            '  </ItemGroup>\n\n</Project>'),
    ),
    (
        "N4-family-project-declares-a-package-id",
        "The lowering declares a PackageId. Packaging is JS-10's decision and the ledger's "
        "standing claim is that nothing here is packable.",
        COMPILER_PROJECT,
        lambda text: text.replace(
            "    <IsPackable>false</IsPackable>",
            "    <PackageId>Broiler.VM.Profile.JavaScript.Compiler</PackageId>\n"
            "    <IsPackable>false</IsPackable>"),
    ),
    (
        "N4-family-project-omits-ispackable",
        "The lowering loses its literal IsPackable false. The vendored packaging props would "
        "then default it to packable, so an omission ships rather than defaulting to safe.",
        COMPILER_PROJECT,
        lambda text: text.replace("    <IsPackable>false</IsPackable>\n", ""),
    ),
    (
        "N2-a-non-family-project-references-the-profile",
        "The fixture profile, which is in no profile family, is given an edge into the JavaScript "
        "profile. This is the INBOUND half of the no-edge-to-another-profile rule, and it is the "
        "half that would otherwise be satisfied from the side that never changes.",
        FIXTURES_PROJECT,
        lambda text: text.replace(
            "</Project>",
            "  <ItemGroup>\n"
            "    <ProjectReference Include=\"..\\..\\Broiler.VM.Profile.JavaScript"
            "\\Broiler.VM.Profile.JavaScript.csproj\" />\n"
            "  </ItemGroup>\n\n</Project>"),
    ),
    (
        "N5-the-registry-omits-a-declared-code",
        "One published row is deleted while the code stays declared and emittable. This is the "
        "forward half of the registry binding, and the shape a real edit takes: a code retired "
        "from the registry and left in the vocabulary.",
        REGISTRY,
        lambda text: text.replace(
            "1411|UnreachableCode|core-result|InconsistentStructure|code|corpus|"
            "an-instruction-no-entry-point-reaches|1\n",
            ""),
    ),
    (
        "N6-the-registry-names-a-reason-the-sites-do-not-carry",
        "One row's core reason is changed to another real core reason. Nothing about the file "
        "looks wrong afterwards, which is why the rule reads the emission sites rather than the "
        "row's plausibility.",
        REGISTRY,
        lambda text: text.replace(
            "1401|UnknownOpcode|core-result|UnknownFeature|",
            "1401|UnknownOpcode|core-result|SemanticValidationFailed|"),
    ),
    (
        "N7-the-registry-names-a-case-the-corpus-does-not-have",
        "One row's case is renamed to an entry nobody wrote. The backward binding is the half a "
        "registry cannot satisfy by being internally consistent, so it is the half worth a "
        "control of its own.",
        REGISTRY,
        lambda text: text.replace(
            "|corpus|an-unknown-opcode|1", "|corpus|an-opcode-nobody-wrote-a-case-for|1"),
    ),
    (
        "N8-a-restated-code-drifts-from-the-registry",
        "The corpus producer's restated constant is renumbered. The duplication is deliberate - "
        "the producer must not read its codes from the profile it tests - and it only buys "
        "anything while the registry holds both halves.",
        MIRROR,
        lambda text: text.replace(
            "    internal const int UnknownOpcode = 1401;",
            "    internal const int UnknownOpcode = 1499;"),
    ),
    (
        "N9-a-position-is-built-outside-the-factory",
        "The verifier builds a position itself instead of going through the published encoding. "
        "This is the exact shape of the conflation JS-3a corrected, reintroduced: one call site "
        "answering with its own convention.",
        PROFILE_VERIFIER,
        lambda text: text.replace(
            "    private static VmSourcePosition At(ulong offset) => "
            "JavaScriptPosition.InArtifact(offset);",
            "    private static VmSourcePosition At(ulong offset) => new(-1, offset, 0, 0);"),
    ),
    (
        "H1-a-profile-ledger-uses-the-components-own-mark",
        "One milestone row's verdict is replaced by a mark from the component's nine-member "
        "legend. The two vocabularies are different claims about different subjects, and a reader "
        "meeting [MET] in a profile ledger would have to guess which one it came from.",
        LEDGER,
        lambda text: text.replace("| [PARTIAL] | **JS-0", "| [MET] | **JS-0"),
    ),
    (
        "H1-a-profile-legend-drops-a-mark",
        "The legend stops publishing [FULL] while the vocabulary still has three members. A legend "
        "that may drop a member leaves a mark in the table above it that nothing defines - and "
        "this control is also what proves the rule reads THIS ledger, which until JS-3a it did "
        "not.",
        LEDGER,
        lambda text: text.replace(
            "| `[FULL]` | The row's bundle demonstrates every exit-gate clause. It is still not "
            "`Accepted`: acceptance additionally needs an owner and a reviewer decision, which "
            "nothing here has. |\n",
            ""),
    ),
    (
        "N10-a-public-member-appears-without-a-baseline-entry",
        "A public constant is added to the format assembly and the frozen baseline is left alone. "
        "This is the direction that matters for a profile: these assemblies are referenced by "
        "composition roots, so a member added here is a member a composition can bind to without "
        "anyone deciding it should be bindable - and until JS-3a nothing in this component would "
        "have noticed.",
        FORMAT_SOURCE,
        lambda text: text.replace(
            "    public const uint MaximumEntryNameBytes = 256;",
            "    public const uint MaximumEntryNameBytes = 256;\n\n"
            "    /// <summary>An addition nobody recorded.</summary>\n"
            "    public const uint UnrecordedCeiling = 7;"),
    ),
    (
        "J3-a-profile-fingerprint-is-stale",
        "The profile's assembly marker keeps its recorded fingerprint while its declaration "
        "changes. This is the control that proves the assurance system REACHES the three new "
        "assemblies rather than merely listing them.",
        PROFILE_MARKER,
        lambda text: text.replace(
            "internal sealed class AssemblyMarker\n{\n}",
            "internal sealed class AssemblyMarker\n{\n    internal const int Injected = 1;\n}"),
    ),
    (
        "J4-a-profile-unit-claims-a-reviewer",
        "The profile's assembly marker claims a human review nobody performed. The value of this "
        "system is that it records the ABSENCE of review, so a mechanism that could turn PENDING "
        "into a name would convert an honest record into a false one.",
        PROFILE_MARKER,
        lambda text: text.replace(
            "// Broiler-Human:        PENDING",
            "// Broiler-Human:        APPROVED; Reviewer=NOBODY; Date=2026-08-31"),
    ),
]


def controls(out):
    """Each control is injected into the real checkout, judged by the suite, and reverted."""
    log = [
        "Every control below is an injection into the real checkout followed by a revert. A",
        "control PASSES when the suite fails while it is injected and passes after the revert.",
        "A control that does not restore its file byte for byte stops the run.",
        "",
    ]

    passed = 0
    skipped = 0

    for name, why, path, mutate in CONTROLS:
        original = read(path)
        mutated = mutate(original)

        if mutated == original:
            skipped += 1
            log.append(f"[{name}] SKIPPED - the injection changed nothing; the anchor has moved.")
            log.append(f"    file: {path}")
            log.append("")
            continue

        overwrite(path, mutated)
        injected_code, injected_output = suite()
        overwrite(path, original)

        if read(path) != original:
            raise SystemExit(f"control {name} did not restore {path}; stopping with the tree modified")

        reverted_code, _ = suite()

        verdict = "PASS" if injected_code != 0 and reverted_code == 0 else "FAIL"
        passed += 1 if verdict == "PASS" else 0

        log.append(f"[{name}] {verdict}")
        log.append(f"    why:       {why}")
        log.append(f"    file:      {path}")
        log.append(f"    injected:  exit {injected_code}")
        log.append(f"    reverted:  exit {reverted_code}")
        log.append("    failing tests while injected:")
        log.extend(
            f"      {line.strip()}"
            for line in injected_output.splitlines()
            if "[FAIL]" in line)
        log.append("")

    log.append(
        f"controls run: {len(CONTROLS)}; controls passed: {passed}; controls SKIPPED: {skipped}")

    if skipped:
        # A skipped control is not a smaller control set, it is a control that was never run while
        # the log still lists it - which is the shape of a bundle that reads stronger than it is.
        # The run is finished and retained either way; main() answers non-zero.
        log.append(
            "A SKIPPED control is a GAP, not a smaller total. Its anchor has moved, so the "
            "injection it names was never made and the row above is a name with nothing behind "
            "it. This collection is not a complete control matrix.")
    log.append("")
    log.append(
        "STATED LIMIT. Rule N2 has a control for its INBOUND half and none for its cross-family "
        "half, because a second profile family does not exist in this graph: an injected edge "
        "would name a project that is not there and the build would fail before any rule ran, so "
        "the suite would go red for the wrong reason. That half has a witness input instead. The "
        "control becomes constructible when the WebAssembly profile's own JS-0 equivalent lands, "
        "and it is named in this bundle's exclusions rather than left as a silent gap.")

    write(os.path.join(out, "negative-controls.log"), "\n".join(log) + "\n")
    return passed, skipped


def publish(project, out_directory, extra, environment=None):
    """Publish one project into a directory, returning the exit code and the log."""
    command = [
        "dotnet", "publish", project, "-c", "Release", "--nologo",
        "-p:TreatWarningsAsErrors=true", "-o", out_directory] + extra

    completed = subprocess.run(
        command, cwd=ROOT, capture_output=True, text=True,
        encoding="utf-8", errors="replace", env=environment)

    return completed.returncode, " ".join(command) + "\n" + (completed.stdout or "") + (completed.stderr or "")


def is_managed(path):
    """
    Whether a PE file carries a CLI header, which is what makes it a managed assembly.

    The core's collection script filters a closure by NAME - anything System.* or Microsoft.* -
    and that is enough on Linux, where the runtime's own native components are .so files a .dll
    glob never sees. On Windows they are .dll files called coreclr, clrjit, hostfxr and so on, and
    a name filter lets every one of them into the report. A closure report is a statement about
    what the composition contributes, so the question it has to ask is whether a file is a managed
    assembly at all - which the PE header answers exactly.
    """
    try:
        with io.open(path, "rb") as handle:
            data = handle.read(1024)
    except OSError:
        return False

    if len(data) < 0x40 or data[:2] != b"MZ":
        return False

    header = int.from_bytes(data[0x3C:0x40], "little")

    if len(data) < header + 24 or data[header:header + 4] != b"PE\0\0":
        return False

    magic = int.from_bytes(data[header + 24:header + 26], "little")
    directories = header + 24 + (96 if magic == 0x10B else 112)
    cli = directories + (14 * 8)

    if len(data) < cli + 8:
        return False

    return int.from_bytes(data[cli + 4:cli + 8], "little") != 0


def closure_of(directory):
    """
    The non-framework managed assemblies a published directory contains.

    Read off the published output rather than derived from the project file, because a reference
    set that looks right can still pull something in through a transitive package, and the linker
    is the only party that knows what actually shipped.
    """
    if not os.path.isdir(directory):
        return []

    return sorted(
        name[:-4] for name in os.listdir(directory)
        if name.endswith(".dll")
        and is_managed(os.path.join(directory, name))
        and not name.startswith("System.")
        and not name.startswith("Microsoft.")
        and name not in ("netstandard.dll", "mscorlib.dll", "WindowsBase.dll"))


def compositions(arguments, out, corpus):
    """
    Publish and run both roots in three modes, and retain what each composed and shipped.

    Three artefacts per composition. The transcript says whether the checks passed in every mode;
    the catalog table says which profiles were composed, compared across modes byte for byte so a
    difference between JIT and Native AOT is a failure rather than a footnote; and the closure
    report lists what the published image actually contains, which is the only form of Native AOT
    evidence the roadmap admits - a linker annotation without execution is insufficient.
    """
    rid = arguments.rid
    binary = ".exe" if platform.system() == "Windows" else ""
    environment = dict(os.environ)

    if platform.system() == "Windows" and os.path.isdir(VS_INSTALLER):
        environment["PATH"] = VS_INSTALLER + os.pathsep + environment["PATH"]

    log = []
    ok = True

    for slug, assembly, project in COMPOSITIONS:
        catalogs = {}
        closures = ["# closure " + assembly + " rid=" + rid, ""]

        for mode, extra in (
                ("jit", ["-r", rid, "--self-contained", "false", "-p:PublishTrimmed=false"]),
                ("trimmed", ["-r", rid, "--self-contained", "true"]),
                ("aot", ["-r", rid, "-p:PublishAot=true"])):

            directory = os.path.join(ROOT, "artifacts", "js-publish", slug, mode)
            shutil.rmtree(directory, ignore_errors=True)
            code, text = publish(project, directory, extra, environment)
            log.append("[" + slug + "/" + mode + "] publish exit " + str(code) + "\n" + text)

            if code != 0:
                ok = False
                closures.append("[" + mode + "] publish failed")
                closures.append("")
                continue

            executable = os.path.join(directory, assembly + binary)

            catalog = subprocess.run(
                [executable, "--closure"], capture_output=True, text=True,
                encoding="utf-8", errors="replace")

            catalogs[mode] = catalog.stdout or ""
            log.append("[" + slug + "/" + mode + "] --closure exit " + str(catalog.returncode)
                       + "\n" + catalogs[mode])

            # --soak is passed HERE and nowhere else. The plateau check is a reading of a heap on
            # a machine rather than a total function of this build, so the composition root makes
            # it opt-in and CI does not opt in - a lane on an ephemeral shared runner cannot
            # attribute a heap number to anything. An evidence collection can: it names its machine,
            # its RID and its publish mode in this bundle. Dropping the flag here would narrow what
            # a bundle covers, which is the opposite of what making it opt-in was for.
            run_arguments = (
                [executable, "--corpus", corpus, "--soak", "--verbose"] if slug == "executiononly"
                else [executable, "--checks", "--verbose"])

            result = subprocess.run(
                run_arguments, capture_output=True, text=True,
                encoding="utf-8", errors="replace")

            log.append("[" + slug + "/" + mode + "] run exit " + str(result.returncode) + "\n"
                       + (result.stdout or "") + (result.stderr or ""))

            if result.returncode != 0:
                ok = False

            names = closure_of(directory)
            closures.append("[" + mode + "] " + str(len(names)) + " non-framework assemblies")
            closures.extend(names)
            closures.append("")

        distinct = {text for text in catalogs.values()}
        log.append("[" + slug + "] catalog tables identical across modes: " + str(len(distinct) <= 1))

        if len(distinct) > 1:
            ok = False

        write(os.path.join(out, "catalog-" + slug + ".txt"), next(iter(catalogs.values()), ""))
        write(os.path.join(out, "closure-" + slug + ".txt"), "\n".join(closures))

    write(os.path.join(out, "publish-and-run.log"), "\n\n".join(log))
    return ok


# The corpus controls: injections the SUITE cannot see.
#
# Every control in CONTROLS above is judged by the test suite, which is right for a rule about the
# graph or about an annotation. A language semantic is not in the suite at all - rule A11 forbids a
# test project to reference a profile assembly, so the behavioural evidence is the composition
# root's own run - and a corpus that could not detect a semantic regression would be a directory of
# bytes rather than a gate. These four are judged by running the execution-only root against the
# retained corpus, which is the thing that would have to notice.
CORPUS_CONTROLS = [
    (
        "the-language-guards-division-by-zero",
        "Division by zero is made a fault, which is what a calculator does and not what the "
        "language does. The corpus entry recording Infinity must stop agreeing.",
        PROFILE_EXECUTOR,
        lambda text: text.replace(
            "                    stack[top - 2] = JavaScriptValue.Number(\n"
            "                        stack[top - 2].ToNumber() / stack[top - 1].ToNumber());",
            "                    if (stack[top - 1].ToNumber() == 0)\n"
            "                    {\n"
            "                        return VmExecutionStep.Faulted(new JavaScriptFault(\n"
            "                            ProfileId, JavaScriptErrorKind.RangeError, \"division by zero\"));\n"
            "                    }\n\n"
            "                    stack[top - 2] = JavaScriptValue.Number(\n"
            "                        stack[top - 2].ToNumber() / stack[top - 1].ToNumber());"),
    ),
    (
        "strict-equality-stops-comparing-kinds",
        "Strict equality is made to compare numbers whatever the kinds are, so `1 === true` "
        "becomes true. The entry recording false must stop agreeing.",
        PROFILE_VALUE,
        lambda text: text.replace(
            "        if (Kind != other.Kind)\n        {\n            return false;\n        }",
            "        if (Kind != other.Kind)\n        {\n            return ToNumber() == other.ToNumber();\n        }"),
    ),
    (
        "to-uint32-becomes-a-cast",
        "The ToUint32 conversion is replaced by a C# cast, which saturates instead of reducing "
        "modulo 2^32. The bitwise-or entry recording -2147483648 must stop agreeing.",
        PROFILE_VALUE,
        lambda text: text.replace(
            "        var truncated = System.Math.Truncate(value) % 4294967296.0;",
            "        var truncated = (double)(uint)System.Math.Min(System.Math.Max(value, 0), 4294967295.0);"),
    ),
    (
        "the-position-encoding-loses-the-section-index",
        "A code-section position is reported with the artifact-relative marker again, which is "
        "the defect JS-3a corrected. The number stays right and the frame it names goes wrong, so "
        "no outcome, reason or diagnostic code moves - only the four rows that pin a position.",
        PROFILE_POSITION,
        lambda text: text.replace(
            "        return new(codeSectionIndex, codeOffset, line, column);",
            "        return new(OutsideAnySection, codeOffset, line, column);"),
    ),
    (
        "the-position-lookup-takes-the-first-row",
        "The covering-row scan stops at the first row rather than the last one at or before the "
        "offset. Three of the four pinned rows are unaffected; the entry whose refusal sits under "
        "the SECOND row of a two-row table is the one that must notice.",
        PROFILE_POSITION,
        lambda text: text.replace(
            "            line = row.Line;\n            column = row.Column;\n        }",
            "            line = row.Line;\n            column = row.Column;\n            break;\n        }"),
    ),
    (
        "an-entry-point-may-be-reached-with-operands",
        "The fall-through edge into an entry point stops being refused. A program is entered with "
        "an empty operand stack; without this check the artifact is answered by whichever fault "
        "the traversal reaches first, which is the order-dependence JS-3a removed.",
        PROFILE_VERIFIER,
        lambda text: text.replace(
            "            if (isEntry[next] == 1 && after != 0)",
            "            if (false && isEntry[next] == 1 && after != 0)"),
    ),
    (
        "the-constant-pool-is-sized-before-its-count-is-checked",
        "The pool array is allocated from the declared count BEFORE the count is compared against "
        "the limits section's maximum. This is the injection roadmap section 7's third discipline "
        "exists for, and it is the one that shows why the discipline is separate from the corpus: "
        "the outcome, the reason and the diagnostic code are all unchanged, so every replay row "
        "still agrees and only the ORDERING checks notice - a hostile entry declaring sixty "
        "thousand constants charges most of a megabyte from a fifty-seven-byte artifact before "
        "refusing it.",
        PROFILE_VERIFIER,
        lambda text: text.replace(
            "        if (count > sections.MaxConstants)\n"
            "        {\n"
            "            return Invalid(\n"
            "                VmReason.InconsistentStructure,\n"
            "                JavaScriptDiagnosticCode.ConstantCountExceedsDeclaredMaximum,\n"
            "                reader.Position);\n"
            "        }\n"
            "\n"
            "        if (!VmBoundedAllocator.TryAllocate<JavaScriptValue>("
            "in bounds, adapter, count, out var constants))",

            "        if (!VmBoundedAllocator.TryAllocate<JavaScriptValue>("
            "in bounds, adapter, count, out var constants))\n"
            "        {\n"
            "            return VmVerifierOutcome.ResourceExhaustion(\n"
            "                VmBudgetDimension.AllocatedBytes, VmBudgetScope.Artifact);\n"
            "        }\n"
            "\n"
            "        if (count > sections.MaxConstants)\n"
            "        {\n"
            "            return Invalid(\n"
            "                VmReason.InconsistentStructure,\n"
            "                JavaScriptDiagnosticCode.ConstantCountExceedsDeclaredMaximum,\n"
            "                reader.Position);\n"
            "        }\n"
            "\n"
            "        if (!VmBoundedAllocator.TryAllocate<JavaScriptValue>("
            "in bounds, adapter, count, out constants))"),
    ),
    (
        "the-executor-stops-charging-fuel-per-step",
        "The interpreter loop stops charging Fuel for the step it is about to take. Nothing about "
        "one program's ANSWER changes - a counting loop still returns 55 - so the corpus replay is "
        "unmoved. What breaks is every claim that rests on a budget being spent: two siblings under "
        "one parent stop exhausting it, and a host that declined stops declining. This is the "
        "control for the aggregate-budget exercise, and it is judged by the same run.",
        PROFILE_EXECUTOR,
        lambda text: text.replace(
            "            if (!environment.Meter.TryCharge(VmBudgetDimension.Fuel, 1))\n"
            "            {\n"
            "                return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);\n"
            "            }",
            "            if (false && !environment.Meter.TryCharge(VmBudgetDimension.Fuel, 1))\n"
            "            {\n"
            "                return VmExecutionStep.ContractViolation(VmReason.AllowanceExhausted);\n"
            "            }"),
    ),
    (
        "the-verifier-stops-refusing-unreachable-code",
        "The unreachable-code check is removed, so an artifact carrying bytes no entry point "
        "reaches would verify. The entry recording that rejection must stop agreeing.",
        PROFILE_VERIFIER,
        lambda text: text.replace(
            "            if (boundary[offset] == 1 && height[offset] < 0)",
            "            if (false && boundary[offset] == 1 && height[offset] < 0)"),
    ),
]


def corpus_controls(out, corpus, arguments):
    """Each control is injected, judged by the corpus replay, and reverted."""
    log = [
        "These controls are judged by running the execution-only composition against the retained",
        "corpus, NOT by the test suite. A language semantic is in no test project - rule A11",
        "forbids one to reference a profile assembly - so a corpus that could not detect a",
        "semantic regression would be a directory of bytes rather than a gate.",
        "",
    ]

    passed = 0
    skipped = 0

    for name, why, path, mutate in CORPUS_CONTROLS:
        original = read(path)
        mutated = mutate(original)

        if mutated == original:
            skipped += 1
            log.append("[" + name + "] SKIPPED - the injection changed nothing; the anchor moved.")
            log.append("    file: " + path)
            log.append("")
            continue

        overwrite(path, mutated)
        injected_code, injected_output = replay(corpus)
        overwrite(path, original)

        if read(path) != original:
            raise SystemExit("control " + name + " did not restore " + path)

        reverted_code, _ = replay(corpus)

        verdict = "PASS" if injected_code != 0 and reverted_code == 0 else "FAIL"
        passed += 1 if verdict == "PASS" else 0

        log.append("[" + name + "] " + verdict)
        log.append("    why:       " + why)
        log.append("    file:      " + path)
        log.append("    injected:  exit " + str(injected_code))
        log.append("    reverted:  exit " + str(reverted_code))
        log.extend(
            "      " + line.strip()
            for line in injected_output.splitlines()
            if line.strip().startswith("FAIL"))
        log.append("")

    log.append(
        "corpus controls run: " + str(len(CORPUS_CONTROLS)) + "; passed: " + str(passed) +
        "; SKIPPED: " + str(skipped))

    if skipped:
        log.append(
            "A SKIPPED control is a GAP, not a smaller total: its anchor has moved, so the "
            "injection it names was never made.")

    write(os.path.join(out, "corpus-controls.log"), "\n".join(log) + "\n")
    return passed, skipped


# The fuzz sessions a bundle retains. Seeds and iteration budgets are STATED, because a session
# is a total function of its seed and its seed corpus and a finding is reproduced by naming both.
# There is no wall-clock budget and no thread count: either would make the same session behave
# differently on two machines, which is the nondeterministic failure class this component's own
# gates forbid.
FUZZ_SESSIONS = ((1, 25_000), (2, 25_000), (3, 25_000), (4, 25_000))


def fuzz(out, corpus):
    """Run the retained fuzz sessions and keep everything they printed, findings included."""
    log = [
        "Coverage-guided fuzzing over the two of roadmap section 7's four surfaces that exist:",
        "the verifier, and the executor over verified-but-adversarial artifacts. The source",
        "tokenizer and parser and the regular-expression matcher are surfaces this profile has",
        "not written, and a session may not be read as covering them.",
        "",
        "Each session is a total function of its seed and the seed corpus. A session that answers",
        "the same way every time, or that never reaches the executor, exits NON-ZERO rather than",
        "reporting clean iterations it did not earn.",
        "",
    ]

    findings = 0

    for seed, iterations in FUZZ_SESSIONS:
        code, output = run([
            "dotnet", "run", "--project", EXECUTION_ONLY, "-c", "Release", "--no-build",
            "--", "--corpus", corpus, "--fuzz",
            "--seed", str(seed), "--iterations", str(iterations)])

        findings += 1 if code == 1 else 0

        log.append(f"[seed {seed}, {iterations} iterations] exit {code}")
        log.extend("    " + line for line in output.splitlines())
        log.append("")

    log.append(
        f"sessions: {len(FUZZ_SESSIONS)}; sessions reporting a finding: {findings}")

    if findings:
        log.append(
            "A FINDING IS NOT CLOSED BY THIS LOG. Roadmap section 7 requires a counterexample to "
            "be closed by a named regression and never by an allow-list entry: the minimized "
            "input becomes a corpus entry with a recorded answer, and the defect is fixed.")

    write(os.path.join(out, "fuzz.log"), "\n".join(log) + "\n")
    return findings


# The fuzz controls: injections judged by a FUZZ SESSION rather than by the suite or the replay.
# A session that finds nothing is worth exactly as much as the demonstration that it would have
# found something, and this is that demonstration. Each one is a defect a hand-written corpus entry
# also catches - that is what the corpus is for - and what these show is that a session reaches the
# same class from bytes nobody wrote.
FUZZ_CONTROLS = [
    (
        "the-constant-index-is-admitted-unchecked",
        "The verifier stops checking a LoadConstant operand against the pool size. The artifact "
        "then verifies and the executor indexes past the pool - the core catches the exception and "
        "reports a fault the profile did not author, which is the executor-surface invariant. "
        "TWENTY-FIVE THOUSAND UNDIRECTED ITERATIONS DID NOT FIND THIS, and the operand-targeting "
        "mutation was written because of it; the session finds it in under two hundred now.",
        PROFILE_VERIFIER,
        lambda text: text.replace(
            "            return index < constantCount\n                ? Ok",
            "            return true\n                ? Ok"),
    ),
]


def fuzz_controls(out, corpus):
    """Each control is injected, judged by a fuzz session, and reverted."""
    log = [
        "These controls are judged by a FUZZ SESSION. A session that reports no counterexample is",
        "worth what the demonstration that it would have reported one is worth, and nothing more.",
        "",
        "A control PASSES when the session exits 1 - a finding - while injected, and 0 after the",
        "revert. Any other exit code is a session that failed for a reason unrelated to the",
        "injection, and is not a pass.",
        "",
    ]

    passed = 0
    skipped = 0

    for name, why, path, mutate in FUZZ_CONTROLS:
        original = read(path)
        mutated = mutate(original)

        if mutated == original:
            skipped += 1
            log.append("[" + name + "] SKIPPED - the injection changed nothing; the anchor moved.")
            log.append("    file: " + path)
            log.append("")
            continue

        overwrite(path, mutated)
        injected_code, injected_output = fuzz_session(corpus)
        overwrite(path, original)

        if read(path) != original:
            raise SystemExit("control " + name + " did not restore " + path)

        reverted_code, _ = fuzz_session(corpus)

        # The injected session RETAINED its finding, and that finding is an artefact of a defect
        # this control put there and took away again. Leaving it on disk would put an unresolved
        # counterexample in the tree for a defect that does not exist, which reads as the one thing
        # a fuzz finding must never read as. A control reverts everything it did, not only the
        # source.
        findings = os.path.join(os.path.dirname(corpus), "js-1-fuzz-findings")

        if os.path.isdir(findings):
            shutil.rmtree(findings)

        verdict = "PASS" if injected_code == 1 and reverted_code == 0 else "FAIL"
        passed += 1 if verdict == "PASS" else 0

        log.append("[" + name + "] " + verdict)
        log.append("    why:       " + why)
        log.append("    file:      " + path)
        log.append("    injected:  exit " + str(injected_code))
        log.append("    reverted:  exit " + str(reverted_code))
        log.extend(
            "      " + line.strip()
            for line in injected_output.splitlines()
            if "FINDING" in line or "minimized" in line or line.strip().startswith("a verified"))
        log.append("")

    log.append(
        "fuzz controls run: " + str(len(FUZZ_CONTROLS)) + "; passed: " + str(passed) +
        "; SKIPPED: " + str(skipped))

    if skipped:
        log.append(
            "A SKIPPED control is a GAP, not a smaller total: its anchor has moved, so the "
            "injection it names was never made.")

    write(os.path.join(out, "fuzz-controls.log"), "\n".join(log) + "\n")
    return passed, skipped


def fuzz_session(corpus, seed=1, iterations=25_000):
    """Rebuild and run one fuzz session, returning its exit code and output."""
    code, text = run(["dotnet", "build", SOLUTION, "-c", "Release", "--nologo"])

    if code != 0:
        return code, text

    return run([
        "dotnet", "run", "--project", EXECUTION_ONLY, "-c", "Release", "--no-build",
        "--", "--corpus", corpus, "--fuzz", "--seed", str(seed), "--iterations", str(iterations)])


# The corpus entries this check mutates. One control entry and one malformed entry, because the
# replay compares a different field for each: a control's completion VALUE and a malformed entry's
# diagnostic code. A check that only ever mutated one of the two would leave the other half of the
# comparison unexercised.
MUTATED_ENTRIES = ("addition", "an-unknown-opcode")


def corpus_integrity(out, corpus):
    """Flip one byte of a retained entry and require the replay to notice."""
    log = [
        "JS-9's exit gate asks that a MUTATED CORPUS ENTRY prove the replay detects a changed",
        "observed triple. Every other control in this bundle injects into SOURCE; this one injects",
        "into the retained bytes, which is the other direction and the one that would otherwise be",
        "taken on trust - a corpus is only evidence while the thing that reads it would notice if",
        "the bytes moved.",
        "",
        "Each entry is mutated by one byte, replayed, restored byte for byte, and replayed again.",
        "A restore that does not reproduce the original stops the run rather than leaving the",
        "corpus modified.",
        "",
    ]

    passed = 0

    for name in MUTATED_ENTRIES:
        path = os.path.join(corpus_relative(corpus), name + ".bjsb")
        original = read_bytes(path)

        # The last byte, which is inside the last section's body rather than in the header - so
        # what moves is the artifact's content and not the magic, and the replay has to reach a
        # real comparison rather than refusing at the first four bytes.
        mutated = bytearray(original)
        mutated[-1] ^= 0xFF

        overwrite_bytes(path, bytes(mutated))
        injected_code, injected_output = replay(corpus)
        overwrite_bytes(path, original)

        if read_bytes(path) != original:
            raise SystemExit("corpus integrity check did not restore " + path)

        reverted_code, _ = replay(corpus)

        verdict = "PASS" if injected_code != 0 and reverted_code == 0 else "FAIL"
        passed += 1 if verdict == "PASS" else 0

        log.append("[" + name + "] " + verdict)
        log.append("    file:      " + path)
        log.append("    mutation:  the last byte, exclusive-or 0xFF")
        log.append("    injected:  exit " + str(injected_code))
        log.append("    reverted:  exit " + str(reverted_code))
        log.extend(
            "      " + line.strip()
            for line in injected_output.splitlines()
            if line.strip().startswith("FAIL"))
        log.append("")

    log.append(
        "entries mutated: " + str(len(MUTATED_ENTRIES)) + "; detected: " + str(passed))

    write(os.path.join(out, "corpus-integrity.log"), "\n".join(log) + "\n")
    return passed


def corpus_relative(corpus):
    """The corpus path relative to the component root, which is what the byte helpers take."""
    return os.path.relpath(corpus, ROOT)


def replay(corpus):
    """Rebuild and run the execution-only root against the retained corpus."""
    code, text = run(["dotnet", "build", SOLUTION, "-c", "Release", "--nologo"])

    if code != 0:
        return code, text

    return run([
        "dotnet", "run", "--project", EXECUTION_ONLY, "-c", "Release", "--no-build",
        "--", "--corpus", corpus])


def hashes(out, paths):
    lines = []

    for path in sorted(paths):
        full = os.path.join(ROOT, path)

        if not os.path.exists(full):
            lines.append(f"{'<missing>':<64}  {path}")
            continue

        with io.open(full, "rb") as handle:
            digest = hashlib.sha256(handle.read()).hexdigest()

        lines.append(f"{digest}  {path}")

    write(os.path.join(out, "hashes.txt"), "\n".join(lines) + "\n")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--bundle", required=True)
    parser.add_argument("--out", required=True)
    parser.add_argument("--milestone", default="JS-0")
    parser.add_argument("--owner", default="profile architecture owner (unassigned identity)")
    parser.add_argument("--reviewer", default="NONE - nothing here has been reviewed")
    parser.add_argument("--skip-controls", action="store_true")
    parser.add_argument("--skip-fuzz", action="store_true")
    parser.add_argument("--skip-publish", action="store_true")
    parser.add_argument("--rid", default="win-x64" if platform.system() == "Windows" else "linux-x64")
    parser.add_argument("--corpus", default=os.path.join("src", "tests", "corpus", "js-1"))
    arguments = parser.parse_args()

    out = os.path.join(ROOT, arguments.out)
    os.makedirs(out, exist_ok=True)

    identity(arguments, out)
    environment(out)
    snapshot_identity(out)

    code, output = run(["dotnet", "build", SOLUTION, "-c", "Release", "--nologo"])
    write(os.path.join(out, "build.log"), f"exit {code}\n\n{output}")

    code, output = suite()
    write(os.path.join(out, "suite.log"), f"exit {code}\n\n{output}")

    gate_code, gate_output = run([
        "dotnet", "test", os.path.join("src", "tests", "Broiler.VM.Architecture.Tests"),
        "-c", "Release", "--nologo"])
    write(os.path.join(out, "assurance-gate.log"), f"exit {gate_code}\n\n{gate_output}")

    release_environment = dict(os.environ, BROILER_ASSURANCE_RELEASE="1")
    completed = subprocess.run(
        ["dotnet", "test", os.path.join("src", "tests", "Broiler.VM.Architecture.Tests"),
         "-c", "Release", "--nologo"],
        cwd=ROOT, capture_output=True, text=True, encoding="utf-8", errors="replace",
        env=release_environment)

    write(
        os.path.join(out, "assurance-release.log"),
        "The release mode is EXPECTED to refuse. Every relevant unit in this component is\n"
        "HUMAN_PENDING, the profile's three among them, so a release gate that passed here would\n"
        "be the defect. What this log is read for is that each blocking declaration is named\n"
        "individually rather than counted.\n\n"
        f"exit {completed.returncode}\n\n{(completed.stdout or '') + (completed.stderr or '')}")

    corpus = os.path.join(ROOT, arguments.corpus)

    if not arguments.skip_publish:
        compositions(arguments, out, corpus)

    skipped = 0

    if not arguments.skip_fuzz:
        fuzz(out, corpus)

    if not arguments.skip_controls:
        corpus_integrity(out, corpus)
        _, suite_skipped = controls(out)
        _, corpus_skipped = corpus_controls(out, corpus, arguments)
        _, fuzz_skipped = fuzz_controls(out, corpus)
        skipped = suite_skipped + corpus_skipped + fuzz_skipped

    hashes(out, [
        SOLUTION,
        PROFILE_PROJECT,
        FORMAT_PROJECT,
        COMPILER_PROJECT,
        PROFILE_MARKER,
        os.path.join("src", "Broiler.VM.Profile.JavaScript.Format", "AssemblyMarker.cs"),
        os.path.join("src", "Broiler.VM.Profile.JavaScript.Compiler", "AssemblyMarker.cs"),
        os.path.join("src", "tests", "Broiler.VM.Architecture.Tests", "rules.register.json"),
        os.path.join("src", "tests", "Broiler.VM.Architecture.Tests", "graph.manifest.json"),
        os.path.join(PROFILE_ROOT, "docs", "roadmap.md"),
        os.path.join(PROFILE_ROOT, "docs", "roadmap.delivery.md"),
        os.path.join(PROFILE_ROOT, "docs", "roadmap.gates.md"),
        os.path.join(PROFILE_ROOT, "docs", "roadmap.status.md"),
        os.path.join(PROFILE_ROOT, "docs", "decisions", "0001-placement-identity-and-assembly-topology.md"),
        os.path.join(PROFILE_ROOT, "docs", "decisions", "0002-feature-manifest-allocation.md"),
        os.path.join(PROFILE_ROOT, "docs", "decisions", "0003-deployment-composition-labels.md"),
        os.path.join(PROFILE_ROOT, "docs", "decisions", "0004-limit-defaults-hard-maxima-and-the-budget-matrix.md"),
        os.path.join(PROFILE_ROOT, "docs", "decisions", "0005-the-seed-waited-on-set-and-snapshot-stop-condition.md"),
        os.path.join(PROFILE_ROOT, "docs", "decisions", "0006-assurance-evidence-and-rules-adoption.md"),
        os.path.join(PROFILE_ROOT, "docs", "decisions", "0007-cross-profile-position-and-amendment-grading.md"),
        os.path.join(PROFILE_ROOT, "docs", "decisions", "0008-format-version-1-the-entry-point-and-what-js-1-corrected.md"),
        os.path.join(PROFILE_ROOT, "docs", "decisions", "0009-the-diagnostic-registry-and-the-position-encoding.md"),
        os.path.join(PROFILE_ROOT, "docs", "decisions", "0010-which-review-rules-govern-this-profiles-documents.md"),
        os.path.join(PROFILE_ROOT, "docs", "decisions", "0011-the-value-frame-and-call-abi.md"),
        os.path.join(PROFILE_ROOT, "docs", "decisions", "0012-the-profile-api-baseline-and-where-its-clause-lives.md"),
        PROFILE_API_BASELINE,
        REGISTRY,
        PROFILE_POSITION,
        os.path.join(arguments.corpus, "corpus.manifest"),
    ])

    print(f"collected {arguments.bundle} into {arguments.out}")

    if skipped:
        # Everything is written; the exit code is what stops a skipped control from being read as
        # a control that passed. The JS-3a collection found this the hard way: a refactor moved an
        # anchor, the log said SKIPPED, and nothing else did.
        print(
            f"broiler-js-evidence: {skipped} control(s) SKIPPED because their anchors have moved. "
            "The bundle is retained and is NOT a complete control matrix.")

        return 1

    return 0


if __name__ == "__main__":
    sys.exit(main())
