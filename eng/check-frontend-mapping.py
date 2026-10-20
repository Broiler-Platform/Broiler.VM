#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# HOLD THE FRONT END'S DIAGNOSTIC MAPPING TO THE EARLY-ERROR CATALOGUE AND TO THE REGISTRY.
#
# Part E of docs/evidence/sjl-12-001/decision-rule-area.md (validation section 7's control for
# SJL-12, "a diagnostic code mapped to two registry meanings"). It reads three files and nothing
# else:
#
#   the mapping    src/Broiler.VM.Profile.JavaScript/docs/diagnostics/frontend-mapping.tsv
#   the catalogue  the copy the mapping's header names (catalogue-copy), which bundle sjl-12-001
#                  retains: Broiler.JS's Broiler.ECMAScript/docs/frontend/early-errors.tsv at the
#                  commit the header names (catalogue-commit)
#   the registry   src/Broiler.VM.Profile.JavaScript/docs/diagnostics/registry.txt
#
# and refuses, naming each problem, when:
#
#   E1  the copy's SHA-256 is not the one the mapping's header names;
#   E2  a catalogue row with a shared code has no mapping row, or more than one, or a mapping row
#       names a shared code, a row or a condition the catalogue does not hold;
#   E3  a shared code maps to anything but exactly one registry code of the registry's
#       embedder-seam half, so a shared code mapped to two registry meanings is refused here;
#   E4  a row marked within-meaning does not quote its registry row's published meaning exactly;
#   E5  a row marked split does not name the registry row it leaves, or does not say that the new
#       number is minted at SJL-13 under JSD-0009, or splits a condition the row's meaning covers;
#   E6  a registry code is reached under a second meaning, one its published meaning does not
#       cover, by a row not marked split (a rule merged onto a registry row whose meaning does not
#       cover it); or one condition reaches two registry meanings, or two registry rows' published
#       meanings cover one condition, or a registry row claims a condition no row reaches within
#       its meaning. A registry row's `conditions` cell is what its published meaning covers;
#       a row's condition is its catalogue row's message identifier, read from the catalogue copy;
#   E7  a registry code from 2001 to 2210 or from 2401 to 2406, other than 2104, is reached by no
#       row within its meaning and names no engine-only (EX) row of the catalogue that explains it;
#   E8  a code the catalogue row's VM cells name (its vm_code, and 2104 where the slice refuses by
#       manifest) is not listed by the mapping row, or a code the row lists beside its registry
#       code is not accounted for in other_codes;
#   E9  the mapping names a registry code registry.txt lacks, or a registry row's name or the
#       header's registry revision differs from registry.txt's.
#
# With --broiler-js <checkout>, it also refuses a copy that differs from the catalogue at the
# commit the header names in that checkout (git show). Part E does not need it: the digest is the
# binding, and the header names the commit for a reader.
#
# Exit status: 0 when nothing is refused, 1 when anything is, 2 when an input cannot be read.

import argparse
import hashlib
import os
import re
import subprocess
import sys

TOOL = "check-frontend-mapping"
ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DEFAULT_MAPPING = "src/Broiler.VM.Profile.JavaScript/docs/diagnostics/frontend-mapping.tsv"

MAPPING_COLUMNS = [
    "kind", "shared_code", "row", "conditions", "wide_codes", "slice_codes", "registry_code",
    "relation", "meaning", "other_codes", "notes",
]
CATALOGUE_COLUMNS = ("id", "family", "shared_code", "message_id", "vm_slice", "vm_code")
HEADER_KEYS = (
    "catalogue-repository", "catalogue-path", "catalogue-commit", "catalogue-sha256",
    "catalogue-copy", "registry", "registry-revision",
)
SPLIT_MEANING = re.compile(
    r"^leaves (?P<code>\d{4}) (?P<name>[A-Za-z]+): (?P<fact>.+); "
    r"the new number is minted at SJL-13 under JSD-0009$")
PUBLISHED_MEANING = re.compile(r"^(?P<code>\d{4}) (?P<name>[A-Za-z]+): (?P<summary>.+)$")
OTHER_CODE = re.compile(r"^(?P<code>\d{4}) (?P<kind>split|other-row (?P<row>E[AEGPX]\d{4})): .+$")
CODE = re.compile(r"^\d{4}$")
REACHED_RANGES = ((2001, 2210), (2401, 2406))
NOT_REQUIRED = {"2104"}


class Unreadable(Exception):
    pass


def read_text(path, what):
    try:
        with open(path, "rb") as handle:
            data = handle.read()
    except OSError as error:
        raise Unreadable(f"the {what} cannot be read: {path}: {error.strerror}")
    try:
        return data, data.decode("utf-8")
    except UnicodeDecodeError:
        raise Unreadable(f"the {what} is not UTF-8: {path}")


def read_mapping(path):
    _, text = read_text(path, "mapping")
    header, rows, columns = {}, [], None
    for number, line in enumerate(text.split("\n"), start=1):
        if not line:
            continue
        if line.startswith("#"):
            match = re.match(r"^# ([a-z0-9-]+): (.+)$", line)
            if match and match.group(1) in HEADER_KEYS:
                if match.group(1) in header:
                    raise Unreadable(f"{path}:{number}: the header names {match.group(1)} twice")
                header[match.group(1)] = match.group(2).strip()
            continue
        cells = line.split("\t")
        if columns is None:
            if cells != MAPPING_COLUMNS:
                raise Unreadable(
                    f"{path}:{number}: the column line is not {' '.join(MAPPING_COLUMNS)}")
            columns = cells
            continue
        if len(cells) != len(columns):
            raise Unreadable(f"{path}:{number}: {len(cells)} cells where there are {len(columns)} columns")
        row = dict(zip(columns, cells))
        row["line"] = number
        rows.append(row)
    if columns is None:
        raise Unreadable(f"{path}: no column line")
    missing = [key for key in HEADER_KEYS if key not in header]
    if missing:
        raise Unreadable(f"{path}: the header does not name {', '.join(missing)}")
    return header, rows


def read_catalogue(path):
    data, text = read_text(path, "catalogue copy")
    lines = text.split("\n")
    if lines and lines[-1] == "":
        lines.pop()
    if not lines:
        raise Unreadable(f"{path}: empty")
    columns = lines[0].split("\t")
    for column in CATALOGUE_COLUMNS:
        if column not in columns:
            raise Unreadable(f"{path}: the catalogue has no {column} column")
    rows = []
    for number, line in enumerate(lines[1:], start=2):
        cells = line.split("\t")
        if len(cells) != len(columns):
            raise Unreadable(f"{path}:{number}: {len(cells)} cells where there are {len(columns)} columns")
        row = dict(zip(columns, cells))
        row["line"] = number
        rows.append(row)
    return hashlib.sha256(data).hexdigest(), rows


def read_registry(path):
    _, text = read_text(path, "registry")
    revision, codes = None, {}
    for line in text.split("\n"):
        match = re.match(r"^# registry-revision: (\d+)\s*$", line)
        if match:
            revision = match.group(1)
            continue
        if not line or line.startswith("#"):
            continue
        cells = line.split("|")
        if len(cells) != 8 or not CODE.match(cells[0]):
            raise Unreadable(f"{path}: a row that is not code|name|half|reason|stage|reachability|case|since: {line}")
        codes[cells[0]] = {"name": cells[1], "half": cells[2]}
    if revision is None:
        raise Unreadable(f"{path}: no registry-revision line")
    return revision, codes


def shown(path):
    full = os.path.abspath(path)
    relative = os.path.relpath(full, ROOT)
    return full if relative.startswith("..") else relative


def split_codes(cell):
    if cell in ("-", "none"):
        return []
    return cell.split(";")


def check(mapping_path, catalogue_override, registry_path, broiler_js):
    header, mapping = read_mapping(mapping_path)
    copy_path = catalogue_override or os.path.join(ROOT, header["catalogue-copy"])
    digest, catalogue = read_catalogue(copy_path)
    revision, registry = read_registry(registry_path or os.path.join(ROOT, header["registry"]))

    problems = []

    def refuse(check_id, message):
        problems.append(f"{check_id} {message}")

    # E1 - the digest the header names is the copy's.
    if digest != header["catalogue-sha256"]:
        refuse("E1", f"the catalogue copy {shown(copy_path)} has SHA-256 {digest}, "
                     f"and the mapping's header names {header['catalogue-sha256']}")
    if broiler_js:
        try:
            committed = subprocess.run(
                ["git", "-C", broiler_js, "show",
                 f"{header['catalogue-commit']}:{header['catalogue-path']}"],
                check=True, capture_output=True).stdout
        except (OSError, subprocess.CalledProcessError) as error:
            raise Unreadable(f"{header['catalogue-path']} at {header['catalogue-commit']} cannot be "
                             f"read from {broiler_js}: {error}")
        committed_digest = hashlib.sha256(committed).hexdigest()
        if committed_digest != digest:
            refuse("E1", f"{header['catalogue-path']} at {header['catalogue-commit']} has SHA-256 "
                         f"{committed_digest}, and the copy has {digest}")

    # E9 - the registry the header names is the one read.
    if header["registry-revision"] != revision:
        refuse("E9", f"the header names registry revision {header['registry-revision']}, and "
                     f"registry.txt is at revision {revision}")

    registry_rows = [row for row in mapping if row["kind"] == "registry"]
    code_rows = [row for row in mapping if row["kind"] == "code"]
    for row in mapping:
        if row["kind"] not in ("registry", "code"):
            refuse("E2", f"line {row['line']}: kind {row['kind']!r} is neither registry nor code")

    def known(code, where):
        if code not in registry:
            refuse("E9", f"{where} names {code}, which registry.txt does not hold")
            return False
        return True

    # The registry rows: one per code, each quoting registry.txt's name.
    published, covers, explains = {}, {}, {}
    for row in registry_rows:
        code = row["registry_code"]
        where = f"line {row['line']} (registry {code})"
        if not CODE.match(code) or not known(code, where):
            continue
        if code in published:
            refuse("E9", f"{where}: a second registry row for {code}")
            continue
        quote = PUBLISHED_MEANING.match(row["meaning"])
        if not quote or quote.group("code") != code:
            refuse("E4", f"{where}: the meaning does not begin '{code} <name>: '")
        elif quote.group("name") != registry[code]["name"]:
            refuse("E9", f"{where}: names {quote.group('name')}, and registry.txt names "
                         f"{code} {registry[code]['name']}")
        if row["relation"] != "published":
            refuse("E4", f"{where}: a registry row's relation is 'published', not {row['relation']!r}")
        published[code] = row["meaning"]
        covers[code] = set(split_codes(row["conditions"]))
        explains[code] = row["row"]

    owner_of_condition = {}
    for code, conditions in sorted(covers.items()):
        for condition in sorted(conditions):
            if condition in owner_of_condition:
                refuse("E6", f"the condition {condition} is covered by the published meanings of both "
                             f"{owner_of_condition[condition]} and {code}")
            else:
                owner_of_condition[condition] = code

    # E2 - one mapping row per catalogue row with a shared code.
    catalogue_by_code = {}
    catalogue_by_id = {}
    for row in catalogue:
        catalogue_by_id[row["id"]] = row
        if row["shared_code"] not in ("", "-"):
            catalogue_by_code.setdefault(row["shared_code"], []).append(row)
    for shared, rows in sorted(catalogue_by_code.items()):
        if len(rows) > 1:
            refuse("E2", f"the catalogue gives {shared} to {len(rows)} rows "
                         f"({', '.join(row['id'] for row in rows)})")
    mapped = {}
    for row in code_rows:
        mapped.setdefault(row["shared_code"], []).append(row)
    for shared in sorted(catalogue_by_code):
        count = len(mapped.get(shared, []))
        if count == 0:
            refuse("E2", f"{shared} ({catalogue_by_code[shared][0]['id']}) has no mapping row")
        elif count > 1:
            codes = ", ".join(row["registry_code"] for row in mapped[shared])
            refuse("E2", f"{shared} has {count} mapping rows (lines "
                         f"{', '.join(str(row['line']) for row in mapped[shared])}), mapping it to "
                         f"{codes}: a shared code is mapped to two registry meanings")
    for shared in sorted(mapped):
        if shared not in catalogue_by_code:
            refuse("E2", f"line {mapped[shared][0]['line']}: {shared} is not a shared code of the catalogue")

    def check_codes(row, where, source, own):
        # E8 - the catalogue's codes are listed, and every other listed code is accounted for.
        # `manifest` in slice_codes is 2104 by the manifest's own row (EX0017), reached before the
        # rule applies; it is listed, and it is no code of this rule's to account for.
        listed, by_manifest = set(), False
        for column in ("wide_codes", "slice_codes"):
            for code in split_codes(row[column]):
                if code == "manifest" and column == "slice_codes":
                    by_manifest = True
                elif CODE.match(code):
                    if known(code, f"{where} {column}"):
                        listed.add(code)
                else:
                    refuse("E8", f"{where}: {column} holds {code!r}, which is no registry code")
        named = set(split_codes(source["vm_code"]))
        if source["vm_slice"].startswith("manifest"):
            named.add("2104")
        for code in sorted(named):
            if not CODE.match(code):
                refuse("E8", f"{where}: the catalogue's vm_code holds {code!r}")
            elif code not in listed and not (code == "2104" and by_manifest):
                refuse("E8", f"{where}: the catalogue row's VM sites refuse it with {code}, which the "
                             f"mapping row does not list")
        others = {}
        if row["other_codes"] != "-":
            for item in row["other_codes"].split(" | "):
                match = OTHER_CODE.match(item)
                if not match:
                    refuse("E8", f"{where}: other_codes item {item[:60]!r} is not "
                                 f"'<code> split: ...' or '<code> other-row <row>: ...'")
                    continue
                code = match.group("code")
                if not known(code, f"{where} other_codes"):
                    continue
                if code in own:
                    refuse("E8", f"{where}: other_codes names its own registry code {code}")
                if code in others:
                    refuse("E8", f"{where}: other_codes names {code} twice")
                if match.group("row"):
                    if match.group("row") not in catalogue_by_id:
                        refuse("E8", f"{where}: other_codes names {match.group('row')}, which the "
                                     f"catalogue does not hold")
                    elif match.group("row") == source["id"]:
                        refuse("E8", f"{where}: other_codes names the row itself as another row")
                others[code] = match.group("kind")
        for code in sorted(listed - own - set(others)):
            refuse("E8", f"{where}: lists {code} beside its registry code, and other_codes "
                         f"does not say whether it is a split or another row's refusal")
        for code in sorted(set(others) - listed):
            refuse("E8", f"{where}: other_codes names {code}, which the row's wide_codes and "
                         f"slice_codes do not list")

    condition_targets = {}
    reached_within = {}
    meanings_at = {}
    for row in code_rows:
        shared = row["shared_code"]
        where = f"line {row['line']} ({shared}, {row['row']})"
        source = catalogue_by_code.get(shared, [None])[0]
        if source is None:
            continue
        if row["row"] != source["id"]:
            refuse("E2", f"{where}: the catalogue gives {shared} to {source['id']}")
        if row["conditions"] != source["message_id"]:
            refuse("E2", f"{where}: the condition is {row['conditions']}, and the catalogue's is "
                         f"{source['message_id']}")
        condition = source["message_id"]

        # E3 - exactly one registry code, of the embedder-seam half.
        targets = row["registry_code"].split(";")
        own = {code for code in targets if CODE.match(code) and known(code, where)}
        if len(targets) != 1 or not CODE.match(targets[0]):
            refuse("E3", f"{where}: registry_code is {row['registry_code']!r}: a shared code maps to "
                         f"exactly one registry code, and this one is mapped to two registry meanings"
                         if len(targets) > 1 else
                         f"{where}: registry_code is {row['registry_code']!r}, not one registry code")
        target = targets[0] if len(targets) == 1 and targets[0] in own else None
        if target is not None and registry[target]["half"] != "embedder-seam":
            refuse("E3", f"{where}: {target} is a {registry[target]['half']} code; a front end's "
                         f"refusal is an embedder-seam code")
        check_codes(row, where, source, own)
        if target is None:
            continue

        # E4 and E5 - the relation and its wording. Whether the published meaning covers the rule
        # is E6's question, asked of every row that reaches the code.
        if row["relation"] == "within-meaning":
            if target not in published:
                refuse("E4", f"{where}: {target} has no registry row to quote")
            elif row["meaning"] != published[target]:
                refuse("E4", f"{where}: marked within-meaning, and the meaning is not {target}'s "
                             f"published meaning as its registry row quotes it")
            if condition in covers.get(target, set()):
                reached_within.setdefault(target, []).append(row)
        elif row["relation"] == "split":
            leaves = SPLIT_MEANING.match(row["meaning"])
            if not leaves:
                refuse("E5", f"{where}: marked split, and the meaning does not read 'leaves <code> "
                             f"<name>: <meaning>; the new number is minted at SJL-13 under JSD-0009'")
            else:
                if leaves.group("code") != target:
                    refuse("E5", f"{where}: marked split from {target}, and the meaning names "
                                 f"{leaves.group('code')}")
                elif leaves.group("name") != registry[target]["name"]:
                    refuse("E5", f"{where}: the meaning names {target} {leaves.group('name')}, and "
                                 f"registry.txt names it {registry[target]['name']}")
            if condition in covers.get(target, set()):
                refuse("E5", f"{where}: marked split from {target}, whose published meaning its "
                             f"registry row says covers {condition}")
        else:
            refuse("E4", f"{where}: relation {row['relation']!r} is neither within-meaning nor split")
            continue
        # The meaning under which the row reaches its code: the published one when it covers the
        # row's condition, the condition's own otherwise.
        meaning = "published" if condition in covers.get(target, set()) else condition
        meanings_at.setdefault(target, {}).setdefault(meaning, []).append(row)
        condition_targets.setdefault(condition, set()).add((target, row["relation"]))

    # E6 - a registry code reached under a meaning other than its published one is reached there
    # by rows marked split, and only by them; one condition reaches one registry meaning.
    for code, meanings in sorted(meanings_at.items()):
        for meaning, rows in sorted(meanings.items()):
            if meaning == "published":
                continue
            for row in rows:
                if row["relation"] != "split":
                    refuse("E6", f"line {row['line']} ({row['shared_code']}, {row['row']}): reaches "
                                 f"{code} {registry[code]['name']} under a second meaning, {meaning}, "
                                 f"which {code}'s published meaning does not cover, and is marked "
                                 f"{row['relation']}, not split: a rule merged onto a registry row "
                                 f"whose meaning does not cover it")
    for condition, targets in sorted(condition_targets.items()):
        if len(targets) > 1:
            pairs = ", ".join(f"{code} {relation}" for code, relation in sorted(targets))
            refuse("E6", f"the condition {condition} is mapped to two registry meanings ({pairs})")
    for code in sorted(covers):
        stale = sorted(condition for condition in covers[code]
                       if not any(row["conditions"] == condition for row in reached_within.get(code, [])))
        if stale:
            refuse("E6", f"registry {code}'s row says its meaning covers {', '.join(stale)}, which no "
                         f"row reaches within its meaning")

    # E7 - every registry code in the ranges is reached within its meaning, or explained.
    for code in sorted(registry):
        number = int(code)
        if code in NOT_REQUIRED or not any(low <= number <= high for low, high in REACHED_RANGES):
            continue
        if reached_within.get(code):
            continue
        explanation = explains.get(code, "-")
        source = catalogue_by_id.get(explanation)
        if explanation == "-" or source is None or source["family"] != "EX":
            refuse("E7", f"{code} {registry[code]['name']} is reached by no row within its meaning and "
                         f"names no engine-only row of the catalogue that explains it")
            continue
        explaining = mapped.get(source["shared_code"], [])
        if not any(row["registry_code"] == code for row in explaining):
            refuse("E7", f"{code} names {explanation} as its explanation, and {explanation}'s shared "
                         f"code does not map to {code}")
    for code, explanation in sorted(explains.items()):
        if explanation == "-":
            continue
        source = catalogue_by_id.get(explanation)
        if source is None or source["family"] != "EX":
            refuse("E7", f"registry {code} names {explanation}, which is not an engine-only row of the "
                         f"catalogue")

    return header, digest, len(code_rows), len(registry_rows), problems


def main():
    parser = argparse.ArgumentParser(
        description="Hold the front end's diagnostic mapping to the early-error catalogue and the "
                    "registry (bundle sjl-12-001, part E).")
    parser.add_argument("--mapping", default=os.path.join(ROOT, DEFAULT_MAPPING),
                        help="the mapping (default: %(default)s)")
    parser.add_argument("--catalogue", default=None,
                        help="the catalogue copy (default: the copy the mapping's header names)")
    parser.add_argument("--registry", default=None,
                        help="the registry (default: the one the mapping's header names)")
    parser.add_argument("--broiler-js", default=None,
                        help="a Broiler.JS checkout: also compare the copy with the catalogue at the "
                             "commit the header names")
    arguments = parser.parse_args()

    try:
        header, digest, codes, registry_rows, problems = check(
            arguments.mapping, arguments.catalogue, arguments.registry, arguments.broiler_js)
    except Unreadable as error:
        print(f"{TOOL}: cannot check: {error}")
        return 2

    print(f"{TOOL}: mapping {shown(arguments.mapping)}")
    print(f"{TOOL}: catalogue {header['catalogue-repository']} {header['catalogue-path']} at "
          f"{header['catalogue-commit']}, copy SHA-256 {digest}")
    print(f"{TOOL}: registry revision {header['registry-revision']}")
    for problem in problems:
        print(f"{TOOL}: refused: {problem}")
    if problems:
        print(f"{TOOL}: REFUSED: {len(problems)} problem(s)")
        return 1
    print(f"{TOOL}: passed: {codes} shared codes, each mapped to one registry code; "
          f"{registry_rows} registry rows; checks E1 to E9 hold")
    return 0


if __name__ == "__main__":
    sys.exit(main())
