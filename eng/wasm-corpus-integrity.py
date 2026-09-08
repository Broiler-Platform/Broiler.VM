#!/usr/bin/env python3
"""Verify the WebAssembly retained corpus against its manifest, then move a byte and require the
replay to notice.

A CORPUS IS ONLY EVIDENCE WHILE THE THING THAT READS IT WOULD NOTICE IF THE BYTES MOVED. Every
other control around this profile injects into SOURCE - a doctored declaration, a mutated module
handed to a verifier; this one injects into the RETAINED BYTES, which is the other direction and
the one that would otherwise be taken on trust.

WHAT THIS SCRIPT CHECKS, IN THREE PASSES.

One, the manifest describes the directory and the directory describes the manifest. Every row's
file exists and hashes to the recorded digest, and every file in the directory has a row. Both
directions, because they fail differently: a row without a file is a replay that would skip an
entry, and a file without a row is an entry somebody added and nothing reads.

Two, the manifest's own shape. Ten columns on every row, a name that appears once, a provenance
that is one of two words, an invariant that is one of six, and a derived row that actually carries
an outcome. A manifest is parsed by two independent implementations - the harness reads it in C#
and this reads it in Python - and the shape rules are how the two are held to one format rather
than to whichever one was written first.

Three, the mutation. Four entries are each flipped by one byte, replayed, restored byte for byte
and replayed again. They are chosen because they fail differently: a control that must VERIFY, a
module refused by the DECODER, a module refused by the VALIDATOR, and one answered with a RESOURCE
EXHAUSTION, which carries no diagnostic code at all and is the row a replay comparing only the
triple would be least likely to notice moving.

AND THREE OF THE FOUR ARE CAUGHT BY THE HASH ALONE, WHICH IS THE POINT. The last byte of the
canonical module sits inside a custom section the decoder reads past without examining, and the
last byte of a wrong-magic module is behind a refusal that already happened at offset zero - so in
both cases the verification answer does not move and only the recorded digest does. A replay that
compared answers and did not re-hash would report those two mutations as passing. The run below
says, per entry, whether the answer moved or whether the digest was the only thing that noticed.

WHAT IS AND IS NOT THE PRODUCT PATH. The replay is - it is the composition root, named by the
caller, and this script never decides a verification answer itself. The hashing, the mutation and
the restore are harness, which is why they are here and not in a composition root: adding a
write-arbitrary-bytes mode to an image whose whole claim is what it cannot do would be widening the
claim in order to test it.

THE RESTORE IS THE DANGEROUS PART and it is checked rather than assumed. Each entry is restored
byte for byte and re-read; a restore that does not reproduce the original stops the run rather than
leaving the corpus modified. A caller that wants belt and braces asserts the tree is clean
afterwards with `git diff --exit-code`.
"""

import argparse
import hashlib
import io
import os
import subprocess
import sys

EXTENSION = ".wasm"
MANIFEST = "corpus.manifest"
COLUMNS = 10

PROVENANCES = ("derived", "recorded")

INVARIANTS = (
    "accepts",
    "refuses-decoding",
    "refuses-validation",
    "exhausts",
    "refuses",
    "sound-either-way",
)

# Four entries, chosen because they fail differently. Each name is a row of the manifest; a name
# that stopped existing is reported as a stale choice here rather than silently skipped, because a
# mutation lane that mutated nothing would pass.
MUTATED_ENTRIES = (
    "control-canonical-module-with-a-padded-section-length",
    "preamble-wrong-magic",
    "validation-an-operand-stack-underflow-in-reachable-code",
    "ceiling-a-vector-length-above-the-declared-count-ceiling",
)


def read_bytes(path):
    with io.open(path, "rb") as handle:
        return handle.read()


def overwrite_bytes(path, payload):
    with io.open(path, "wb") as handle:
        handle.write(payload)


def read_manifest(path):
    """Parse the manifest by hand, the way the harness's C# half does and for the same reason."""
    rows = []

    with io.open(path, "r", encoding="utf-8", newline="") as handle:
        text = handle.read()

    if "\r\n" in text:
        raise SystemExit("the manifest carries CR-LF line endings, and it is pinned as LF")

    for line in text.split("\n"):
        if line == "" or line.startswith("#"):
            continue

        parts = line.split("|")

        if len(parts) != COLUMNS:
            raise SystemExit(
                "manifest row has " + str(len(parts)) + " columns and " + str(COLUMNS) +
                " were expected: " + line)

        rows.append({
            "name": parts[0],
            "sha256": parts[1],
            "family": parts[2],
            "outcome": parts[3],
            "reason": parts[4],
            "diagnostic": parts[5],
            "dimension": parts[6],
            "scope": parts[7],
            "provenance": parts[8],
            "invariant": parts[9],
        })

    return rows


def check_shape(rows, lines):
    """The manifest's own rules, checked before a single byte is hashed."""
    complaints = []
    seen = set()

    for row in rows:
        if row["name"] in seen:
            complaints.append(row["name"] + ": appears more than once")

        seen.add(row["name"])

        if len(row["sha256"]) != 64 or any(c not in "0123456789abcdef" for c in row["sha256"]):
            complaints.append(row["name"] + ": the digest column is not 64 lowercase hex digits")

        if row["provenance"] not in PROVENANCES:
            complaints.append(
                row["name"] + ": provenance " + row["provenance"] + " is not one of " +
                ", ".join(PROVENANCES))

        if row["invariant"] not in INVARIANTS:
            complaints.append(
                row["name"] + ": invariant " + row["invariant"] + " is not one of " +
                ", ".join(INVARIANTS))

        if row["outcome"] == "":
            complaints.append(row["name"] + ": carries no outcome")

        # An exhaustion carries no diagnostic code, so the dimension and the scope are the only
        # things that tell one exhaustion from another. A row that named neither would be recording
        # ResourceExhaustion/CeilingReached/0 and nothing else, which is the same answer for every
        # ceiling this profile has.
        if row["outcome"] == "ResourceExhaustion" and row["dimension"] == "-":
            complaints.append(row["name"] + ": is an exhaustion and names no dimension")

        if row["outcome"] != "ResourceExhaustion" and row["dimension"] != "-":
            complaints.append(
                row["name"] + ": names the dimension " + row["dimension"] +
                " and is not an exhaustion")

    lines.append("[manifest shape] " + ("PASS" if not complaints else "FAIL"))
    lines.append("    rows:      " + str(len(rows)))
    lines.append("    columns:   " + str(COLUMNS))
    lines.append("    derived:   " + str(sum(1 for r in rows if r["provenance"] == "derived")))
    lines.append("    recorded:  " + str(sum(1 for r in rows if r["provenance"] == "recorded")))

    for complaint in complaints:
        lines.append("    " + complaint)

    lines.append("")
    return len(complaints)


def check_hashes(corpus, rows, lines):
    """Every row against its file, and every file against the rows. Both directions."""
    complaints = []
    verified = 0

    for row in rows:
        path = os.path.join(corpus, row["name"] + EXTENSION)

        if not os.path.exists(path):
            complaints.append(row["name"] + ": the manifest names a file that is not there")
            continue

        digest = hashlib.sha256(read_bytes(path)).hexdigest()

        if digest != row["sha256"]:
            complaints.append(
                row["name"] + ": recorded " + row["sha256"] + " and the file hashes to " + digest)
            continue

        verified += 1

    named = set(row["name"] for row in rows)

    for entry in sorted(os.listdir(corpus)):
        if not entry.endswith(EXTENSION):
            continue

        if entry[:-len(EXTENSION)] not in named:
            complaints.append(entry + ": is in the directory and in no manifest row")

    lines.append("[hashes] " + ("PASS" if not complaints else "FAIL"))
    lines.append("    files re-hashed and matched: " + str(verified) + " of " + str(len(rows)))

    for complaint in complaints:
        lines.append("    " + complaint)

    lines.append("")
    return len(complaints)


def check_mutation(corpus, rows, replay, lines):
    """Flip one byte of four entries and require the replay to notice each time."""
    named = set(row["name"] for row in rows)
    detected = 0
    attempted = 0

    for name in MUTATED_ENTRIES:
        if name not in named:
            lines.append("[" + name + "] FAIL")
            lines.append("    this script names an entry the manifest does not hold")
            lines.append("")
            attempted += 1
            continue

        attempted += 1
        path = os.path.join(corpus, name + EXTENSION)
        original = read_bytes(path)

        # The LAST byte, which is inside a section body rather than in the preamble - so what moves
        # is the module's content and not its magic, and the replay has to reach a real comparison
        # rather than refusing at the first four bytes. A mutation the replay caught only because
        # the magic changed would prove nothing about the rest of the file.
        mutated = bytearray(original)
        mutated[-1] ^= 0xFF

        overwrite_bytes(path, bytes(mutated))
        injected = subprocess.run(
            replay, capture_output=True, text=True, encoding="utf-8", errors="replace")
        overwrite_bytes(path, original)

        if read_bytes(path) != original:
            raise SystemExit("the corpus integrity check did not restore " + path)

        reverted = subprocess.run(
            replay, capture_output=True, text=True, encoding="utf-8", errors="replace")

        passed = injected.returncode != 0 and reverted.returncode == 0
        detected += 1 if passed else 0

        reported = [
            line.strip()
            for line in (injected.stdout or "").splitlines()
            if line.strip().startswith("FAIL") and name in line]

        # Which half of the replay noticed. The replay prints one line per failing entry listing
        # every complaint it had, separated by semicolons; a line whose ONLY complaint is the digest
        # is a row the verification answer did not move on, and that is the case that makes
        # recording a hash worth the column it takes.
        complaints = [
            complaint.strip()
            for line in reported
            for complaint in line.split(":", 1)[1].split(";")]

        moved = any(not complaint.startswith("hash ") for complaint in complaints)

        lines.append("[" + name + "] " + ("PASS" if passed else "FAIL"))
        lines.append("    file:      " + path)
        lines.append("    mutation:  the last byte, exclusive-or 0xFF")
        lines.append("    injected:  exit " + str(injected.returncode))
        lines.append("    reverted:  exit " + str(reverted.returncode))
        lines.append(
            "    noticed:   " +
            ("the verification answer moved, and the digest as well" if moved
             else "THE DIGEST ALONE - the verification answer did not move"))
        lines.extend("      " + line for line in reported)
        lines.append("")

    lines.append("entries mutated: " + str(attempted) + "; detected: " + str(detected))
    return attempted - detected


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--corpus", required=True)
    parser.add_argument(
        "--hashes-only", action="store_true",
        help="run the manifest and hash passes and skip the mutation pass, which needs a replay")
    parser.add_argument(
        "replay", nargs=argparse.REMAINDER,
        help="the replay command, after --. It is run with no further arguments added.")
    arguments = parser.parse_args()

    replay = [argument for argument in arguments.replay if argument != "--"]

    if not replay and not arguments.hashes_only:
        print("wasm-corpus-integrity: no replay command was given after --")
        return 2

    manifest_path = os.path.join(arguments.corpus, MANIFEST)

    if not os.path.exists(manifest_path):
        print("wasm-corpus-integrity: no " + MANIFEST + " in " + arguments.corpus)
        return 2

    rows = read_manifest(manifest_path)

    lines = [
        "THE WEBASSEMBLY RETAINED CORPUS, CHECKED AGAINST ITS MANIFEST AND THEN MOVED.",
        "",
        "A corpus is only evidence while the thing that reads it would notice if the bytes moved.",
        "Three passes: the manifest's own shape, every file against its recorded digest in both",
        "directions, and four entries flipped by one byte and required to fail the replay.",
        "",
        "corpus:  " + arguments.corpus,
        "replay:  " + (" ".join(replay) if replay else "not run (--hashes-only)"),
        "",
    ]

    failures = check_shape(rows, lines)
    failures += check_hashes(arguments.corpus, rows, lines)

    if not arguments.hashes_only:
        failures += check_mutation(arguments.corpus, rows, replay, lines)

    print("\n".join(lines))

    return 0 if failures == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
