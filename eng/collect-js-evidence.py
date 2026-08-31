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
FIXTURES_PROJECT = os.path.join(
    "src", "tests", "Broiler.VM.Fixtures", "Broiler.VM.Fixtures.csproj")

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

    for name, why, path, mutate in CONTROLS:
        original = read(path)
        mutated = mutate(original)

        if mutated == original:
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

    log.append(f"controls run: {len(CONTROLS)}; controls passed: {passed}")
    log.append("")
    log.append(
        "STATED LIMIT. Rule N2 has a control for its INBOUND half and none for its cross-family "
        "half, because a second profile family does not exist in this graph: an injected edge "
        "would name a project that is not there and the build would fail before any rule ran, so "
        "the suite would go red for the wrong reason. That half has a witness input instead. The "
        "control becomes constructible when the WebAssembly profile's own JS-0 equivalent lands, "
        "and it is named in this bundle's exclusions rather than left as a silent gap.")

    write(os.path.join(out, "negative-controls.log"), "\n".join(log) + "\n")
    return passed


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

    if not arguments.skip_controls:
        controls(out)

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
    ])

    print(f"collected {arguments.bundle} into {arguments.out}")


if __name__ == "__main__":
    sys.exit(main())
