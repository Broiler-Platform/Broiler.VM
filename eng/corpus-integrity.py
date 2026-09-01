#!/usr/bin/env python3
"""Flip one byte of a retained corpus entry and require the replay to notice.

JS-9's exit gate asks that a MUTATED CORPUS ENTRY prove the replay detects a changed observed
triple. Every other control in this component injects into SOURCE; this one injects into the
RETAINED BYTES, which is the other direction and the one that would otherwise be taken on trust -
a corpus is only evidence while the thing that reads it would notice if the bytes moved.

WHY THIS IS A FILE OF ITS OWN. It has two callers that must not diverge: the evidence collection
script, which replays through `dotnet run` and keeps the output in a bundle, and the CI lane, which
replays through the published Native AOT image. Writing it twice would be two implementations of
one discipline, and the second one would be the one nobody reads.

WHAT IS AND IS NOT THE PRODUCT PATH. The replay is - it is the composition root, named by the
caller, and this script never decides an answer itself. The mutation and the restore are harness,
which is why they are here and not in a composition root: adding a write-arbitrary-bytes mode to an
image whose whole claim is what it cannot do would be widening the claim to test it.

THE RESTORE IS THE DANGEROUS PART and it is checked rather than assumed. Each entry is restored
byte for byte and re-read; a restore that does not reproduce the original stops the run rather than
leaving the corpus modified. A caller that wants belt and braces asserts the tree is clean
afterwards, which is what the CI lane does with `git diff --exit-code`.
"""

import argparse
import io
import os
import subprocess
import sys

# Two entries, chosen because they fail differently: a control that must VERIFY and execute to a
# recorded value, and a malformed entry that must be REFUSED with a recorded code. Mutating only a
# control would leave "does the replay notice a rejection that stopped being one" untested.
MUTATED_ENTRIES = ("addition", "an-unknown-opcode")


def read_bytes(path):
    with io.open(path, "rb") as handle:
        return handle.read()


def overwrite_bytes(path, payload):
    with io.open(path, "wb") as handle:
        handle.write(payload)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--corpus", required=True)
    parser.add_argument(
        "replay", nargs=argparse.REMAINDER,
        help="the replay command, after --. It is run with no further arguments added.")
    arguments = parser.parse_args()

    replay = [argument for argument in arguments.replay if argument != "--"]

    if not replay:
        print("corpus-integrity: no replay command was given after --")
        return 2

    lines = [
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
        "replay command: " + " ".join(replay),
        "",
    ]

    detected = 0

    for name in MUTATED_ENTRIES:
        path = os.path.join(arguments.corpus, name + ".bjsb")
        original = read_bytes(path)

        # The last byte, which is inside the last section's body rather than in the header - so
        # what moves is the artifact's content and not the magic, and the replay has to reach a
        # real comparison rather than refusing at the first four bytes.
        mutated = bytearray(original)
        mutated[-1] ^= 0xFF

        overwrite_bytes(path, bytes(mutated))
        injected = subprocess.run(
            replay, capture_output=True, text=True, encoding="utf-8", errors="replace")
        overwrite_bytes(path, original)

        if read_bytes(path) != original:
            raise SystemExit("corpus integrity check did not restore " + path)

        reverted = subprocess.run(
            replay, capture_output=True, text=True, encoding="utf-8", errors="replace")

        verdict = "PASS" if injected.returncode != 0 and reverted.returncode == 0 else "FAIL"
        detected += 1 if verdict == "PASS" else 0

        lines.append("[" + name + "] " + verdict)
        lines.append("    file:      " + path)
        lines.append("    mutation:  the last byte, exclusive-or 0xFF")
        lines.append("    injected:  exit " + str(injected.returncode))
        lines.append("    reverted:  exit " + str(reverted.returncode))
        lines.extend(
            "      " + line.strip()
            for line in (injected.stdout or "").splitlines()
            if line.strip().startswith("FAIL"))
        lines.append("")

    lines.append(
        "entries mutated: " + str(len(MUTATED_ENTRIES)) + "; detected: " + str(detected))

    print("\n".join(lines))

    return 0 if detected == len(MUTATED_ENTRIES) else 1


if __name__ == "__main__":
    sys.exit(main())
