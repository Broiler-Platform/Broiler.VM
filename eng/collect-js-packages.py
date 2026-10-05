"""Pack the JavaScript profile's package set, restore a pristine consumer from it, and roll back.

    python eng/collect-js-packages.py --bundle JS-10-002 \
        --out src/Broiler.VM.Profile.JavaScript/docs/evidence/js-10-002

Phase F9's slice R3 (decision JSD-0061) takes release gate 8's facts and retains them as one bundle:

  1. THE MANIFEST FIRST, made read-only before anything is packed: the commit and whether the tree
     is clean, the SDK, the candidate's version, and the previous package set's source.
  2. THE CANDIDATE PACK. The solution is packed at a version no release would carry, and every
     package's .nuspec and file list are retained - the produced metadata release gate 8 asks to
     declare no foreign dependency, and what rule N37 holds the profile's package baseline to.
  3. THE PREVIOUS PACKAGE SET is the one actually published: the consumer's packages at
     0.1.0-preview.5, downloaded from nuget.org and hashed. It carries no Intl package, because none
     was published.
  4. A PRISTINE CONSUMER RESTORES AND RUNS with upstream feeds unreachable: samples/NuGet.config
     lists one local source, the packages folder and the HTTP cache are fresh and empty, and every
     proxy variable points at a closed port, so a package found anywhere but the local feed is a
     failed restore rather than a quiet download.
  5. ROLLBACK: the same consumer, unchanged, restored and run against the previous set, then rolled
     forward to the candidate again.
  6. A NEGATIVE CONTROL: the consumer asked for exactly 0.1.0-preview.4, which nuget.org holds and the
     local feed does not, must FAIL to restore, naming the local feed as the only source searched. A
     restore that succeeded would mean upstream was reachable.
  7. The consumer published as Native AOT from the candidate and run. The platform packs a
     RID-specific Native AOT publish needs come from the SDK's own download, not from a feed this
     component controls, so they are the one thing seeded into that step's packages folder, by name.

Nothing here publishes anything. The feed is a directory under artifacts/, which is not committed.
"""

import argparse
import datetime
import hashlib
import os
import platform
import shutil
import stat
import subprocess
import sys
import tempfile
import urllib.request
import zipfile

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SOLUTION = "Broiler.VM.slnx"
SAMPLE = os.path.join(ROOT, "samples", "Broiler.VM.Sample.JavaScriptConsumer")
FEED = os.path.join(ROOT, "artifacts", "feed")

PREFIX = "0.1.0"
CANDIDATE_SUFFIX = "preview.6.f9-candidate"
CANDIDATE = PREFIX + "-" + CANDIDATE_SUFFIX
PREVIOUS = "0.1.0-preview.5"
UNPUBLISHED = "0.1.0-preview.4"

# The previous set: what the consumer references at 0.1.0-preview.5, as published. The Intl package
# did not exist then.
PREVIOUS_PACKAGES = [
    "Broiler.VM.Abstractions",
    "Broiler.VM.Binary",
    "Broiler.VM.Runtime",
    "Broiler.VM.Profile.JavaScript",
    "Broiler.VM.Profile.JavaScript.Format",
    "Broiler.VM.Profile.JavaScript.Compiler",
]
UPSTREAM = "https://api.nuget.org/v3-flatcontainer/{lower}/{version}/{lower}.{version}.nupkg"

# The SDK's own platform packs a RID-specific Native AOT publish downloads - the runtime packs, the
# trimmer's tasks and the compiler - seeded by name into the AOT step's packages folder. None is a
# Broiler.VM package and none is a dependency any Broiler.VM package declares.
AOT_PACKS = [
    "microsoft.aspnetcore.app.runtime.linux-x64",
    "microsoft.dotnet.ilcompiler",
    "microsoft.net.illink.tasks",
    "microsoft.netcore.app.runtime.linux-x64",
    "microsoft.netcore.app.runtime.nativeaot.linux-x64",
    "runtime.linux-x64.microsoft.dotnet.ilcompiler",
]

UNREACHABLE = "http://127.0.0.1:9"


def run(command, cwd=ROOT, env=None):
    result = subprocess.run(command, cwd=cwd, capture_output=True, text=True, env=env)
    return result.returncode, (result.stdout + result.stderr)


def sha256(path):
    digest = hashlib.sha256()
    with open(path, "rb") as stream:
        for block in iter(lambda: stream.read(1 << 20), b""):
            digest.update(block)
    return digest.hexdigest()


def isolated(packages, http_cache):
    """An environment whose packages folder and HTTP cache are fresh and whose upstream is unreachable."""
    env = dict(os.environ)
    env["NUGET_PACKAGES"] = packages
    env["NUGET_HTTP_CACHE_PATH"] = http_cache
    env["NUGET_PLUGINS_CACHE_PATH"] = os.path.join(http_cache, "plugins")
    for name in ("HTTPS_PROXY", "HTTP_PROXY", "https_proxy", "http_proxy", "ALL_PROXY", "all_proxy"):
        env[name] = UNREACHABLE
    env.pop("NO_PROXY", None)
    env.pop("no_proxy", None)
    env["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1"
    env["DOTNET_NOLOGO"] = "1"
    return env


def write_manifest(out, bundle):
    commit = run(["git", "rev-parse", "HEAD"])[1].strip()
    status = run(["git", "status", "--porcelain"])[1]
    sdk = run(["dotnet", "--version"])[1].strip()
    lines = [
        "bundle " + bundle,
        "evidence-class packaging",
        "decision The candidate set is recorded as this profile's packable baseline if every family "
        "package's metadata names only Broiler.VM packages, the pristine consumer restores and runs "
        "against it with upstream unreachable, rolls back to the published previous set and forward "
        "again, and the negative control fails to restore; otherwise the bundle records which step "
        "failed and no baseline is taken.",
        "collected " + datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%M:%SZ"),
        "commit " + commit,
        "tree " + ("clean" if status.strip() == "" else "dirty"),
        "sdk " + sdk,
        "machine " + platform.platform() + " " + platform.machine() + " cpus=" + str(os.cpu_count()),
        "candidate " + CANDIDATE,
        "previous " + PREVIOUS + " from " + UPSTREAM.format(lower="{id}", version=PREVIOUS),
        "previous-packages " + " ".join(PREVIOUS_PACKAGES),
        "negative-control " + UNPUBLISHED,
        "upstream-during-restore " + UNREACHABLE,
        "aot-seeded " + " ".join(AOT_PACKS),
    ]
    if status.strip():
        lines.append("dirty:")
        lines.extend("  " + line for line in status.splitlines())
    path = os.path.join(out, "manifest.txt")
    with open(path, "w", encoding="utf-8", newline="\n") as stream:
        stream.write("\n".join(lines) + "\n")
    os.chmod(path, stat.S_IREAD | stat.S_IRGRP | stat.S_IROTH)
    return path, status.strip() == ""


def nuspecs_and_contents(directory):
    """Every .nuspec, and every file in every .nupkg, the candidate pack produced."""
    nuspecs, contents = [], []
    for name in sorted(os.listdir(directory)):
        if not name.endswith(".nupkg"):
            continue
        with zipfile.ZipFile(os.path.join(directory, name)) as archive:
            for entry in sorted(archive.namelist()):
                if entry.endswith(".nuspec"):
                    nuspecs.append("=== %s :: %s ===" % (name, entry))
                    nuspecs.append(archive.read(entry).decode("utf-8").strip())
                    nuspecs.append("")
                if entry.startswith(("_rels/", "package/")) or entry == "[Content_Types].xml":
                    continue
                contents.append("%s %s %d" % (name, entry, archive.getinfo(entry).file_size))
    return "\n".join(nuspecs).strip() + "\n", "\n".join(contents) + "\n"


def step(lines, title, code, output):
    lines.append("")
    lines.append("--- %s ---" % title)
    lines.append(output.strip())
    lines.append("exit code: %d" % code)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--bundle", required=True)
    parser.add_argument("--out", required=True)
    parser.add_argument("--rid", default="linux-x64")
    arguments = parser.parse_args()

    out = os.path.join(ROOT, arguments.out)
    os.makedirs(out, exist_ok=True)
    manifest, clean = write_manifest(out, arguments.bundle)
    print("manifest " + manifest + (" (clean tree)" if clean else " (DIRTY tree)"))

    # 2. The candidate pack, into a directory of its own, then onto an empty feed.
    if os.path.isdir(FEED):
        shutil.rmtree(FEED)
    os.makedirs(FEED)
    packed = tempfile.mkdtemp(prefix="broiler-js-pack-")
    code, output = run(["dotnet", "pack", SOLUTION, "-c", "Release", "-o", packed,
                        "-p:VersionSuffix=" + CANDIDATE_SUFFIX])
    produced = sorted(os.listdir(packed))
    with open(os.path.join(out, "pack.log"), "w", encoding="utf-8", newline="\n") as stream:
        stream.write(output.strip() + "\nexit code: %d\n--- produced ---\n%s\n" % (code, "\n".join(produced)))
    if code != 0:
        print("the pack failed; see pack.log")
        return 1
    nuspecs, contents = nuspecs_and_contents(packed)
    with open(os.path.join(out, "nuspecs.txt"), "w", encoding="utf-8", newline="\n") as stream:
        stream.write(nuspecs)
    with open(os.path.join(out, "contents.txt"), "w", encoding="utf-8", newline="\n") as stream:
        stream.write(contents)
    for name in produced:
        if name.endswith(".nupkg"):
            shutil.copy(os.path.join(packed, name), FEED)

    # 3. The previous set, as published.
    lines = ["=== THE PREVIOUS PACKAGE SET, AS PUBLISHED ==="]
    for package in PREVIOUS_PACKAGES:
        lower = package.lower()
        url = UPSTREAM.format(lower=lower, version=PREVIOUS)
        target = os.path.join(FEED, "%s.%s.nupkg" % (package, PREVIOUS))
        with urllib.request.urlopen(url) as response, open(target, "wb") as stream:
            shutil.copyfileobj(response, stream)
        lines.append("%s %s sha256=%s bytes=%d" % (package, PREVIOUS, sha256(target), os.path.getsize(target)))
    lines.append("")
    lines.append("--- feed contents ---")
    lines.extend("%s sha256=%s" % (name, sha256(os.path.join(FEED, name))) for name in sorted(os.listdir(FEED)))

    # 4-6. Restore and run, roll back, roll forward, and the negative control, each from nothing.
    sequence = [
        ("restore and run the candidate " + CANDIDATE, CANDIDATE, []),
        ("ROLL BACK to the published " + PREVIOUS + " and run", PREVIOUS, ["-p:BroilerVmIntl=false"]),
        ("roll forward to the candidate " + CANDIDATE + " and run", CANDIDATE, []),
    ]
    results = []
    for title, version, extra in sequence:
        scratch = tempfile.mkdtemp(prefix="broiler-js-consumer-")
        env = isolated(os.path.join(scratch, "packages"), os.path.join(scratch, "http-cache"))
        for directory in ("bin", "obj"):
            shutil.rmtree(os.path.join(SAMPLE, directory), ignore_errors=True)
        code, output = run(["dotnet", "run", "-c", "Release", "-p:BroilerVmVersion=" + version] + extra,
                           cwd=SAMPLE, env=env)
        step(lines, title, code, output)
        results.append(code)
        shutil.rmtree(scratch, ignore_errors=True)

    scratch = tempfile.mkdtemp(prefix="broiler-js-consumer-")
    env = isolated(os.path.join(scratch, "packages"), os.path.join(scratch, "http-cache"))
    for directory in ("bin", "obj"):
        shutil.rmtree(os.path.join(SAMPLE, directory), ignore_errors=True)
    code, output = run(["dotnet", "restore", "-p:BroilerVmVersion=[" + UNPUBLISHED + "]", "-p:BroilerVmIntl=false"],
                       cwd=SAMPLE, env=env)
    step(lines, "NEGATIVE CONTROL: restore exactly " + UNPUBLISHED + ", which nuget.org holds and the feed does not; it must fail", code, output)
    # It must fail for the reason it exists: the version is not found, and only the local feed was searched.
    control = code if ("NU1102" in output and "local-feed" in output and "nuget.org" not in output) else 0
    shutil.rmtree(scratch, ignore_errors=True)

    # 7. Native AOT, from the candidate.
    scratch = tempfile.mkdtemp(prefix="broiler-js-consumer-aot-")
    packages = os.path.join(scratch, "packages")
    os.makedirs(packages)
    home = os.environ.get("NUGET_PACKAGES") or os.path.join(os.path.expanduser("~"), ".nuget", "packages")
    for pack in AOT_PACKS:
        shutil.copytree(os.path.join(home, pack), os.path.join(packages, pack))
    env = isolated(packages, os.path.join(scratch, "http-cache"))
    for directory in ("bin", "obj"):
        shutil.rmtree(os.path.join(SAMPLE, directory), ignore_errors=True)
    published = os.path.join(scratch, "publish")
    code, output = run(["dotnet", "publish", "-c", "Release", "-r", arguments.rid, "-p:PublishAot=true",
                        "-p:BroilerVmVersion=" + CANDIDATE, "-o", published], cwd=SAMPLE, env=env)
    step(lines, "publish the consumer as Native AOT from the candidate", code, output)
    aot = code
    binary = os.path.join(published, "Broiler.VM.Sample.JavaScriptConsumer")
    if code == 0 and os.path.isfile(binary):
        lines.append("native image size: %d bytes" % os.path.getsize(binary))
        code, output = run([binary], cwd=published)
        step(lines, "run the Native AOT consumer", code, output)
        aot = code
    shutil.rmtree(scratch, ignore_errors=True)
    for directory in ("bin", "obj"):
        shutil.rmtree(os.path.join(SAMPLE, directory), ignore_errors=True)

    with open(os.path.join(out, "consumer.log"), "w", encoding="utf-8", newline="\n") as stream:
        stream.write("\n".join(lines) + "\n")

    hashes = []
    for name in sorted(os.listdir(out)):
        if name != "hashes.txt" and os.path.isfile(os.path.join(out, name)):
            hashes.append("%s  %s" % (sha256(os.path.join(out, name)), name))
    with open(os.path.join(out, "hashes.txt"), "w", encoding="utf-8", newline="\n") as stream:
        stream.write("\n".join(hashes) + "\n")

    ok = all(code == 0 for code in results) and control != 0 and aot == 0
    print("consumer runs: %s; negative control exit %d; native AOT exit %d" % (results, control, aot))
    print("ALL STEPS AS EXPECTED" if ok else "A STEP DID NOT GO AS EXPECTED")
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())
