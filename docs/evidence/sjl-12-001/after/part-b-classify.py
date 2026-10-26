#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# READ EVERY DIFFERENCE PART B'S COMPARISON PRINTED, AND SAY WHICH OF THE RULE'S CLASSES ITS WORDS ADMIT IT
# UNDER (bundle sjl-12-001, part B, base run 1 against after run 1). Written 2026-10-26 at the records step
# (WP7.3), after the after runs, by reading their output; it is the bundle's reading, not the rule's.
#
# It reads three things and runs nothing:
#   - after/identity-base-after.tsv.gz, the output of eng/compare-parse-corpus.py identity, which lists each
#     difference with both diagnostic lists and a mechanical shape (S4? or S2) and decides no class;
#   - src/tests/corpus/parse-1/corpus.manifest, for each input's path and SHA-256;
#   - the pinned test262 archive the manifest names, for each input's source text, checked against the
#     manifest's SHA-256.
#
# A row is put in a class only when every test below holds; otherwise it is written `unclassified`, which
# the rule makes a regression. The tests are those the rule's words set for the two shapes the comparison
# printed, read with the bundle's section 3 ("Readings named before the after runs"):
#
#   S4, "unmasked - an early error after that the base did not report, because the lowering left a construct
#   after its first refusal in it, with a code the binder pass reports":
#     - the input is refused at both commits, in B1, B2 or B3 (B4 admits no class);
#     - the after list is the base list with exactly one entry added, and nothing else changed;
#     - the added entry is a 2206 (UnresolvableIdentifier), which the binder pass reports for a private name
#       no class declares (JsBinder.ResolvePrivate, JsBinder.EarlyErrors.cs at the after commit);
#     - the base list's entry on that line is a 2205 at a `delete` whose operand is a private member
#       expression, the refusal after which the base's lowering emitted `true` and lowered nothing of the
#       operand (JsCompiler.Operators.cs's `delete o.#x` arm at the corpus commit);
#     - the added 2206 is at that operand, after the `delete` and any parentheses, and names a private name
#       that appears nowhere else in the input's code (so no class there declares it);
#     - the request is not E7, under which eval code may read its caller's class's private names, and the
#       input's code holds no template literal, so the added entry is not the parser's (section 3's edge 1).
#   S2, "collapse - a diagnostic the base repeated at one position, because a finally block or another
#   construct was lowered more than once, appears once after":
#     - the input is refused at both commits, in B3 under E6 (InClassFieldInitializer);
#     - the base list is the same 2206 at 0:0 twice, and the after list is that entry once;
#     - the input's code names `arguments` exactly once, as the target of an update or compound or logical
#       assignment: one reference the base's lowering emitted twice, a read and a write, each refused by
#       EmitEvalName at no place (JsCompiler.Eval.cs at the corpus commit), and the binder reports once per
#       reference (JsBinder.Referenced at the after commit);
#     - the input's code holds no template literal.
#
# Usage, from the repository root at a checkout that holds the bundle and the pinned archive:
#   python3 -I docs/evidence/sjl-12-001/after/part-b-classify.py > docs/evidence/sjl-12-001/after/part-b-classes.tsv
# It exits 0 when every row is in a class and 1 when any is unclassified.

import gzip
import hashlib
import os
import re
import sys
import tarfile

ROOT = os.path.abspath(os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..", "..", ".."))
BUNDLE = "docs/evidence/sjl-12-001"
IDENTITY = BUNDLE + "/after/identity-base-after.tsv.gz"
MANIFEST = "src/tests/corpus/parse-1/corpus.manifest"


def manifest():
    rows, archive, prefix = {}, None, None
    with open(os.path.join(ROOT, MANIFEST), encoding="utf-8") as handle:
        for line in handle:
            line = line.rstrip("\n")
            if line.startswith("#% pin-test262-archive"):
                _, path, digest = line.split("\t")
                archive = (path, digest)
            elif line.startswith("#% pin-test262\t"):
                prefix = "test262-" + line.split("\t")[1] + "/"
            elif line and not line.startswith("#") and not line.startswith("id\t"):
                field = line.split("\t")
                rows[field[0]] = {"origin": field[1], "path": field[2], "sha256": field[3], "goal": field[4]}
    return rows, archive, prefix


def differences():
    rows, header = [], []
    with gzip.open(os.path.join(ROOT, IDENTITY), "rt", encoding="utf-8") as handle:
        for line in handle:
            line = line.rstrip("\n")
            if line.startswith("#"):
                header.append(line)
            elif not line.startswith("id\t"):
                rows.append(line.split("\t"))
    return header, rows


def entries(text):
    if text in ("", "-"):
        return []
    return [tuple(int(part) for part in item.split(":")) for item in text.split(";")]


def code_of(text):
    # The source with its comments blanked out, keeping every line and column in place.
    out, i, n = list(text), 0, len(text)
    while i < n:
        if text.startswith("//", i):
            while i < n and text[i] != "\n":
                out[i] = " "
                i += 1
        elif text.startswith("/*", i):
            end = text.find("*/", i + 2)
            end = n if end < 0 else end + 2
            for j in range(i, end):
                if text[j] != "\n":
                    out[j] = " "
            i = end
        else:
            i += 1
    return "".join(out)


def at(lines, line, column):
    # Diagnostics are 1-based on both counts.
    return lines[line - 1][column - 1:] if 0 < line <= len(lines) else ""


DELETE = re.compile(r"delete[\s(]*")
PRIVATE_MEMBER = re.compile(r"(?:this|[A-Za-z_$][\w$]*\(\))\.#([A-Za-z_$][\w$]*)")
WRITTEN_ARGUMENTS = re.compile(
    r"(?:\+\+|--)\s*arguments\b|\barguments\s*(?:\+\+|--|(?:\*\*|>>>|<<|>>|&&|\|\||\?\?|[-+*/%&|^])=)")


def s4(row, base, after, lines, code):
    population, variant = row[1], row[2]
    if population == "B4" or variant == "E7" or "`" in code:
        return None
    if len(after) != len(base) + 1:
        return None
    for k, added in enumerate(after):
        if after[:k] + after[k + 1:] == base:
            break
    else:
        return None
    code_number, line, column = added
    if code_number != 2206:
        return None
    refusals = [e for e in base if e[0] == 2205 and e[1] == line]
    if len(refusals) != 1:
        return None
    _, _, delete_column = refusals[0]
    deleted = DELETE.match(at(lines, line, delete_column))
    if not deleted or delete_column + deleted.end() != column:
        return None
    member = PRIVATE_MEMBER.match(at(lines, line, column))
    if not member:
        return None
    name = member.group(1)
    if len(re.findall(r"#" + re.escape(name) + r"\b", code)) != 1:
        return None
    return ("S4", "2206 at %d:%d, the private member `%s` that the base's 2205 refusal of `delete` at %d:%d left "
            "unlowered; #%s is declared by no class around it" % (line, column, member.group(0), line,
                                                                 delete_column, name))


def s2(row, base, after, code):
    population, variant = row[1], row[2]
    if population != "B3" or variant != "E6" or "`" in code:
        return None
    if base != [(2206, 0, 0), (2206, 0, 0)] or after != [(2206, 0, 0)]:
        return None
    if len(re.findall(r"\barguments\b", code)) != 1 or len(WRITTEN_ARGUMENTS.findall(code)) != 1:
        return None
    written = WRITTEN_ARGUMENTS.search(code).group(0)
    return ("S2", "2206 at 0:0 twice becomes once: `%s` is one reference the base lowered as a read and a write, "
            "each refused at no place; the binder reports once per reference" % " ".join(written.split()))


def main():
    rows_of, (archive_path, archive_digest), prefix = manifest()
    header, rows = differences()
    wanted = {rows_of[row[0]]["path"]: row[0] for row in rows}
    with open(os.path.join(ROOT, archive_path), "rb") as handle:
        data = handle.read()
    if hashlib.sha256(data).hexdigest() != archive_digest:
        sys.exit("the archive's SHA-256 is not the manifest's")
    sources = {}
    with tarfile.open(os.path.join(ROOT, archive_path), "r:gz") as archive:
        for member in archive:
            if member.isfile() and member.name.startswith(prefix) and member.name[len(prefix):] in wanted:
                raw = archive.extractfile(member).read()
                identifier = wanted[member.name[len(prefix):]]
                if hashlib.sha256(raw).hexdigest() != rows_of[identifier]["sha256"]:
                    sys.exit("%s: the source's SHA-256 is not the manifest's" % identifier)
                sources[identifier] = raw.decode("utf-8")
    out = ["# part B's differences, base run 1 against after run 1, each with the class the rule's words admit it",
           "# under (docs/evidence/sjl-12-001/after/part-b-classify.py; its header says every test). Read from:",
           "#   " + IDENTITY,
           "#   " + MANIFEST + " and the archive it pins, " + archive_path + " sha256 " + archive_digest]
    out += ["# the comparison's header: " + line.lstrip("# ") for line in header if "differences" in line or
            "by what differs" in line or "census" in line or "incomplete" in line]
    body, tally, unclassified = [], {}, 0
    for row in rows:
        identifier, population, variant, form, what, base_text, after_text, status, shape = row
        base, after = entries(base_text), entries(after_text)
        text = sources[identifier]
        code = code_of(text)
        lines = code.split("\n")
        verdict = None
        if what == "diagnostics":
            verdict = s4(row, base, after, lines, code) if shape == "S4?" else s2(row, base, after, code)
        if verdict is None:
            verdict = ("unclassified", "no class's tests hold")
            unclassified += 1
        tally[verdict[0]] = tally.get(verdict[0], 0) + 1
        decision = {"S4": "JSD-0080 D12 (SJL-12.6 item 2): the binder walks the whole unit",
                    "S2": "JSD-0080 D11 and D12 (SJL-12.6 item 2): the binder reports once per reference"}.get(
                        verdict[0], "-")
        body.append("\t".join((identifier, population, variant, form, base_text, after_text, shape, verdict[0],
                               verdict[1], decision, rows_of[identifier]["path"])))
    out.append("# rows %d: %s" % (len(rows), ", ".join("%s %d" % item for item in sorted(tally.items()))))
    out.append("\t".join(("id", "population", "variant", "form", "base", "after", "script's shape", "class",
                          "how it was read", "the decision behind it", "path")))
    sys.stdout.write("\n".join(out + body) + "\n")
    return 1 if unclassified else 0


if __name__ == "__main__":
    sys.exit(main())
