#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# WRITE THE manifest.json OF A UNIVERSAL BYTECODE PROGRAMME EVIDENCE BUNDLE.
#
# The programme ledger (docs/universal-bytecode.status.md section 3) asks every bundle for a
# manifest.json "naming every input, script and binary by hash and the commit each was built from".
# The core's own collector writes a hashes.txt and no manifest, so this script writes the manifest the
# programme's bundles use. It judges nothing: it records what a bundle names, by hash, and a bundle's
# README says what those files demonstrate.
#
# WHAT IT HASHES, AND HOW:
#
#   --source-revision REV --source PATH...
#       files read AT a named revision, identified by their git blob id at that revision
#       (`git rev-parse REV:PATH`), so a reader can check the exact bytes the bundle's record was
#       written against without trusting the working copy;
#   --input PATH...
#       files of the working tree the bundle depends on (records, scripts, corpora), identified by the
#       SHA-256 of their bytes with CRLF normalised to LF, which is the form .gitattributes has git
#       store every text file in (a binary file holds no CRLF pair it could lose);
#   every file already in the bundle directory except manifest.json itself, identified the same way.
#
# The commit the bundle was collected at is `git rev-parse HEAD`, recorded as `collected-at`, with a
# `worktree-clean` flag. A bundle collected from an unclean tree says so in its manifest rather than
# pretending otherwise.

import argparse
import hashlib
import json
import os
import subprocess
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))


def git(*arguments):
    return subprocess.run(["git", *arguments], cwd=ROOT, check=True, capture_output=True,
                          text=True).stdout.strip()


def stored_bytes(path):
    full = os.path.join(ROOT, path)
    with open(full, "rb") as handle:
        data = handle.read()
    # The bytes git stores for a text file are LF; a Windows working copy may hold CRLF. Hash the
    # stored form, so a manifest written on either platform names the same digest.
    normalised = data.replace(b"\r\n", b"\n")
    return normalised


def sha256(data):
    return hashlib.sha256(data).hexdigest()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--bundle", required=True, help="the bundle directory, e.g. docs/evidence/ubc-0-001")
    parser.add_argument("--milestone", required=True)
    parser.add_argument("--evidence-class", required=True)
    parser.add_argument("--source-revision")
    parser.add_argument("--source", nargs="*", default=[])
    parser.add_argument("--input", nargs="*", default=[])
    parser.add_argument("--note", default="")
    arguments = parser.parse_args()

    bundle = arguments.bundle.replace("\\", "/").rstrip("/")
    head = git("rev-parse", "HEAD")
    # The bundle's own files are written before its manifest and committed with it, so the tree is
    # called clean when nothing outside the bundle directory differs from HEAD.
    dirty = [line for line in git("status", "--porcelain", "--untracked-files=all").splitlines()
             if not line[3:].replace("\\", "/").startswith(bundle + "/")]
    clean = not dirty

    manifest = {
        "bundle": os.path.basename(bundle),
        "milestone": arguments.milestone,
        "evidence-class": arguments.evidence_class,
        "collected-at": head,
        "worktree-clean": clean,
        "hash-method": "sources: git blob id at source-revision; inputs and retained files: sha256 of the LF bytes git stores",
    }
    if arguments.note:
        manifest["note"] = arguments.note
    if arguments.source:
        if not arguments.source_revision:
            parser.error("--source needs --source-revision")
        revision = git("rev-parse", arguments.source_revision)
        manifest["source-revision"] = revision
        manifest["sources"] = {path: git("rev-parse", f"{revision}:{path}") for path in sorted(arguments.source)}
    manifest["inputs"] = {path: sha256(stored_bytes(path)) for path in sorted(arguments.input)}

    retained = {}
    directory = os.path.join(ROOT, bundle)
    for name in sorted(os.listdir(directory)):
        if name == "manifest.json" or os.path.isdir(os.path.join(directory, name)):
            continue
        retained[name] = sha256(stored_bytes(f"{bundle}/{name}"))
    manifest["retained"] = retained

    with open(os.path.join(directory, "manifest.json"), "w", encoding="utf-8", newline="\n") as handle:
        json.dump(manifest, handle, indent=2)
        handle.write("\n")
    print(f"wrote {bundle}/manifest.json: {len(manifest.get('sources', {}))} source(s), "
          f"{len(manifest['inputs'])} input(s), {len(retained)} retained file(s); worktree-clean={clean}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
