"""The admitted shared packages, for the evidence collectors (milestone SJL-2.7; rule I7).

Rule I1's allow-list is src/tests/Broiler.VM.Architecture.Tests/admitted-packages.json. A collector
that publishes a composition reads it here for two things:

  - IDENTITY (rule I7). When a closure the collector retains carries an admitted package, the bundle
    records the package's identifier, version and NuGet content hash, and the source commit its
    nuspec names - the upstream revision a release's review gate reads (decision SJL-D-5 (a)). The
    commit is read from a copy of the package on this machine, never from a network: the committed
    feed's copy, eng/packages/, where one is there, and otherwise the copy restore placed in the NuGet
    global packages folder, the folder rule I1 holds to the entry's archiveHash. Either copy is read
    only after its bytes hash to the entry's archiveHash.
    (Revised 2026-11-01. The last sentence read: "The commit is read from the committed feed's copy of
    the package, eng/packages/, never from a network." The committed feed holds no package since the
    owner's change of 2026-10-30 and 2026-10-31, and every admitted package restores from nuget.org,
    so the identity this file wrote from then on recorded no commit, which rule I7 refuses.)
  - DECLARATION. A closure listing treats an admitted assembly as declared rather than filtering it by
    name: the closure filters drop System.* and Microsoft.* (EX-87), and an admitted assembly is
    neither, so it is listed, and the identity record says what it is.

Nothing here restores, downloads or publishes anything.
"""

import hashlib
import base64
import json
import os
import zipfile
import xml.etree.ElementTree as ET

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ALLOW_LIST = os.path.join(ROOT, "src", "tests", "Broiler.VM.Architecture.Tests", "admitted-packages.json")
IDENTITY_FILE = "admitted-packages.json"


def load(root=ROOT):
    """The allow-list's entries."""
    path = os.path.join(root, "src", "tests", "Broiler.VM.Architecture.Tests", "admitted-packages.json")
    with open(path, encoding="utf-8") as handle:
        return json.load(handle)["packages"]


def feed_path(entry, root=ROOT):
    """The committed feed's copy of an admitted package, beside its pin."""
    return os.path.join(root, os.path.dirname(entry["pin"]), entry["id"] + "." + entry["version"] + ".nupkg")


def content_hash(path):
    """
    The base64 SHA-512 of the package's bytes: its archive hash, which the allow-list's archiveHash
    pins. For an unsigned package it is also NuGet's content hash; for a signed one NuGet's content
    hash leaves the signature out, and the allow-list's contentHash holds that. (Revised 2026-11-01.
    It read: "NuGet's content hash: the base64 SHA-512 of the package's bytes, which a lock file
    records." The lock files were removed on 2026-10-30.)
    """
    with open(path, "rb") as handle:
        return base64.b64encode(hashlib.sha512(handle.read()).digest()).decode("ascii")


def packages_folder(root=ROOT):
    """
    The NuGet global packages folder restore placed the admitted packages in, as rule I1 finds it: the
    first of the architecture tests' obj/project.assets.json `packageFolders`; else NUGET_PACKAGES; else
    the user's ~/.nuget/packages. None when none of them is a directory.
    """
    assets = os.path.join(root, "src", "tests", "Broiler.VM.Architecture.Tests", "obj", "project.assets.json")
    candidates = []
    if os.path.isfile(assets):
        with open(assets, encoding="utf-8") as handle:
            candidates.extend(json.load(handle).get("packageFolders", {}).keys())
    if os.environ.get("NUGET_PACKAGES"):
        candidates.append(os.environ["NUGET_PACKAGES"])
    candidates.append(os.path.join(os.path.expanduser("~"), ".nuget", "packages"))
    return next((folder for folder in candidates if os.path.isdir(folder)), None)


def restored_path(entry, root=ROOT):
    """The restored copy of an admitted package in the global packages folder, or None."""
    folder = packages_folder(root)
    if folder is None:
        return None
    name, version = entry["id"].lower(), entry["version"].lower()
    return os.path.join(folder, name, version, name + "." + version + ".nupkg")


def nuspec_commit(path):
    """The source commit the package's nuspec records in its repository element, or None."""
    with zipfile.ZipFile(path) as archive:
        name = next(entry for entry in archive.namelist() if entry.endswith(".nuspec"))
        document = ET.fromstring(archive.read(name))
    for element in document.iter():
        if element.tag.rsplit("}", 1)[-1] == "repository":
            return element.get("commit")
    return None


def identities(assemblies, root=ROOT):
    """The upstream identity of every admitted package among `assemblies` (assembly names)."""
    held = set(assemblies)
    records = []
    for entry in load(root):
        if entry["id"] not in held:
            continue
        path = feed_path(entry, root)
        record = {"id": entry["id"], "version": entry["version"], "contentHash": entry["contentHash"], "use": entry["use"]}
        if os.path.isfile(path):
            archive_hash = content_hash(path)
            if archive_hash != entry.get("archiveHash", entry["contentHash"]):
                raise ValueError("Committed package archive hash mismatch: " + path)
            record["feedContentHash"] = entry["contentHash"]
            record["commit"] = nuspec_commit(path)
        else:
            # THE RESTORED COPY (2026-11-01), held to the archive pin before its nuspec is read, as
            # rule I1 holds it. An entry with no archiveHash is one rule I1 refuses, so its commit is
            # not read; nor is a package restore has not placed, and the record then names no commit,
            # which rule I7 refuses for a bundle whose closure carries it.
            restored = restored_path(entry, root)
            if restored is not None and os.path.isfile(restored) and entry.get("archiveHash"):
                if content_hash(restored) != entry["archiveHash"]:
                    raise ValueError("Restored package archive hash mismatch: " + restored)
                record["archiveHash"] = entry["archiveHash"]
                record["commit"] = nuspec_commit(restored)
        records.append(record)
    return records


def write_identities(out, assemblies, root=ROOT):
    """Writes `out`/admitted-packages.json when a retained closure carries an admitted package; returns the records."""
    records = identities(assemblies, root)
    if records:
        with open(os.path.join(out, IDENTITY_FILE), "w", encoding="utf-8", newline="\n") as handle:
            json.dump({"admittedPackages": records}, handle, indent=2)
            handle.write("\n")
    return records


def place_on_feed(feed, root=ROOT):
    """Copies every admitted package on the committed feed onto `feed`; returns the file names placed."""
    placed = []
    os.makedirs(feed, exist_ok=True)
    for entry in load(root):
        path = feed_path(entry, root)
        if os.path.isfile(path):
            target = os.path.join(feed, os.path.basename(path))
            with open(path, "rb") as source, open(target, "wb") as destination:
                destination.write(source.read())
            placed.append(os.path.basename(path))
    return placed


if __name__ == "__main__":
    for record in identities([entry["id"] for entry in load()]):
        print(json.dumps(record))
