"""Restore through each source NuGet.config configures, and show an unmapped identifier refused.

    python eng/verify-sources.py [--log <file>]

Milestone SJL-2.7's done-when clause, as a procedure anyone can repeat. Each restore runs in a scratch
directory with a fresh package folder and no HTTP cache, against a copy of this repository's
NuGet.config whose relative paths are made absolute and which is otherwise unchanged:

  1. THE COMMITTED FEED. A scratch project references the admitted shared package at its pinned
     version; the source mapping sends the Broiler.ECMAScript prefix to eng/packages/ (and nuget.org),
     and the restore succeeds from the committed feed. Its lock file's content hash is printed beside
     the allow-list's pin.
  2. NUGET.ORG. A scratch project references a package only nuget.org holds, the one this repository's
     architecture tests already take (System.Reflection.MetadataLoadContext); the restore succeeds.
  3. AN UNMAPPED IDENTIFIER, REFUSED. A package whose identifier is outside the shared prefix is packed
     into a scratch copy of the committed feed, and the configuration points at that copy. The mapping
     sends the identifier to nuget.org only, which does not hold it, so the restore FAILS although the
     package sits in a configured source: a source serves only the identifiers mapped to it.

Exit 0 when the first two succeed and the third fails for that reason (NU1101).
"""

import argparse
import base64
import hashlib
import json
import os
import shutil
import subprocess
import sys
import tempfile
import xml.etree.ElementTree as ET

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import admitted_packages  # noqa: E402

NUGET_ORG_ONLY = ("System.Reflection.MetadataLoadContext", "9.0.0")
UNMAPPED = ("Unmapped.Source.Probe", "1.0.0")


def run(arguments, cwd, env, transcript):
    result = subprocess.run(arguments, cwd=cwd, env=env, capture_output=True, text=True, encoding="utf-8", errors="replace")
    transcript.append("$ " + " ".join(arguments).replace(cwd, "<scratch>"))
    transcript.append((result.stdout + result.stderr).replace(cwd, "<scratch>").rstrip())
    transcript.append("exit %d" % result.returncode)
    transcript.append("")
    return result.returncode, result.stdout + result.stderr


def config_copy(target, feed=None):
    """This repository's NuGet.config with relative source paths made absolute, or pointed at `feed`."""
    tree = ET.parse(os.path.join(ROOT, "NuGet.config"))
    for source in tree.getroot().iter("add"):
        value = source.get("value", "")
        if "://" not in value:
            source.set("value", feed or os.path.abspath(os.path.join(ROOT, value)))
    tree.write(target, encoding="utf-8", xml_declaration=True)


def project(directory, package, version, lock=False):
    os.makedirs(directory, exist_ok=True)
    with open(os.path.join(directory, "Scratch.csproj"), "w", encoding="utf-8", newline="\n") as handle:
        handle.write("""<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
    <RestorePackagesWithLockFile>%s</RestorePackagesWithLockFile>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="%s" Version="[%s]" />
  </ItemGroup>
</Project>
""" % ("true" if lock else "false", package, version))


def environment(scratch):
    env = dict(os.environ)
    env["NUGET_PACKAGES"] = os.path.join(scratch, "packages")
    env["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1"
    env["DOTNET_NOLOGO"] = "1"
    return env


def restore(scratch, transcript):
    return run(["dotnet", "restore", os.path.join(scratch, "project", "Scratch.csproj"),
                "--configfile", os.path.join(scratch, "NuGet.config"), "--no-http-cache"],
               scratch, environment(scratch), transcript)


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--log")
    arguments = parser.parse_args()
    transcript = ["# eng/verify-sources.py: a restore through each configured source, and an unmapped identifier", ""]
    results = []

    # 1. The committed feed.
    entry = next(e for e in admitted_packages.load() if "committed-feed" in e["sources"])
    scratch = tempfile.mkdtemp(prefix="broiler-sources-feed-")
    config_copy(os.path.join(scratch, "NuGet.config"))
    project(os.path.join(scratch, "project"), entry["id"], entry["version"], lock=True)
    transcript.append("## 1. the committed feed: %s %s" % (entry["id"], entry["version"]))
    code, _ = restore(scratch, transcript)
    with open(os.path.join(scratch, "project", "packages.lock.json"), encoding="utf-8") as handle:
        locked = json.load(handle)["dependencies"]["net10.0"][entry["id"]]["contentHash"]
    transcript.append("lock file content hash: " + locked)
    transcript.append("allow-list pin:         " + entry["contentHash"])
    transcript.append("")
    results.append(code == 0 and locked == entry["contentHash"])
    shutil.rmtree(scratch, ignore_errors=True)

    # 2. nuget.org.
    scratch = tempfile.mkdtemp(prefix="broiler-sources-nugetorg-")
    config_copy(os.path.join(scratch, "NuGet.config"))
    project(os.path.join(scratch, "project"), *NUGET_ORG_ONLY)
    transcript.append("## 2. nuget.org: %s %s" % NUGET_ORG_ONLY)
    code, _ = restore(scratch, transcript)
    results.append(code == 0)
    shutil.rmtree(scratch, ignore_errors=True)

    # 3. An identifier outside the shared prefix, sitting in a copy of the committed feed.
    scratch = tempfile.mkdtemp(prefix="broiler-sources-unmapped-")
    feed = os.path.join(scratch, "feed")
    shutil.copytree(os.path.join(ROOT, "eng", "packages"), feed)
    library = os.path.join(scratch, "library")
    os.makedirs(library)
    with open(os.path.join(library, "Unmapped.csproj"), "w", encoding="utf-8", newline="\n") as handle:
        handle.write("""<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <PackageId>%s</PackageId>
    <Version>%s</Version>
    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
  </PropertyGroup>
</Project>
""" % UNMAPPED)
    transcript.append("## 3. an unmapped identifier: %s %s, packed into a copy of the committed feed" % UNMAPPED)
    run(["dotnet", "pack", os.path.join(library, "Unmapped.csproj"), "-o", feed, "--nologo"], scratch, environment(scratch), transcript)
    transcript.append("feed copy: " + ", ".join(sorted(os.listdir(feed))))
    transcript.append("")
    config_copy(os.path.join(scratch, "NuGet.config"), feed)
    project(os.path.join(scratch, "project"), *UNMAPPED)
    code, output = restore(scratch, transcript)
    refused = code != 0 and "NU1101" in output
    results.append(refused)
    shutil.rmtree(scratch, ignore_errors=True)

    transcript.append("# Result: committed feed %s, nuget.org %s, unmapped identifier refused %s" % tuple(
        "yes" if ok else "NO" for ok in results))
    text = "\n".join(transcript) + "\n"
    if arguments.log:
        with open(arguments.log, "w", encoding="utf-8", newline="\n") as handle:
            handle.write(text)
    print(text)
    return 0 if all(results) else 1


if __name__ == "__main__":
    sys.exit(main())
