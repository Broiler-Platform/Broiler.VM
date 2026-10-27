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
# With --broiler-js <checkout>, it also reads two files from that checkout at the commit the
# header names (git show), and refuses:
#
#   E1  a copy that differs from the catalogue there;
#   E8  a code the mapping row lists in wide_codes or slice_codes that neither the catalogue row's
#       VM cells nor a site the catalogue's VM site inventory (SITES_PATH below) assigns to the row
#       give, and a code such a site refuses with that the row does not list (`manifest` lists
#       2104). This is E8's other direction: without it, a stale code left in wide_codes with its
#       item in other_codes passes.
#
# Part E's population is the three files above, and it does not need this mode: the digest is the
# binding, and the header names the commit for a reader. The inventory is read only here.
#
# REVISION 19 AND AFTER (added 2026-10-27, at SJL-13's tools commit; part E of
# docs/evidence/sjl-13-001/decision-rule-area.md, JSD-0083 decisions D12 and D27). Checks E10 to E14
# are INERT while registry.txt states a revision below 19: below it the checker reads exactly what it
# read before and refuses exactly what it refused. From revision 19 it also reads the stage table's copy
# the mapping's header names, SliceSourceDiagnosticCode (SliceSourceDiagnostics.cs), the conformance
# root's LanguageErrors.cs and Test262Run.cs, and the retained source corpus's manifest, and refuses
# when:
#
#   E10 a mapping row is marked split: no split relation remains from revision 19 (decision D12);
#   E11 the header does not name the stage table (stage-table-path, stage-table-commit,
#       stage-table-sha256, stage-table-copy), or its copy's SHA-256 is not the one it names, or, with
#       --broiler-js, the copy differs from the stage table at the commit the header names;
#   E12 registry.txt names no owning decision (`# owning-decision: JSD-nnnn`), or its
#       published-at-milestone list does not name the revision it states;
#   E13 a code the registry adds at revision 19 or later (an embedder-seam row whose `since` is 19 or
#       more) has no member of that name and number in SliceSourceDiagnosticCode; or its stage is not one
#       of the half's four, or its reachability is not `source`, or its case is not a `refused` entry of
#       the retained source corpus answered with its name (rule N7 admits no new defensive row); or the
#       mapping has no `registry` row for it whose `notes` names the code it left, as
#       `left <code> <Name>` - a code of an earlier revision, under its registry name;
#   E14 such a code has no class: it is missing from LanguageErrors.Classified, or Classify gives it no
#       arm; or it is not graded as decision D12 grades it: a code whose registry row's `notes` says
#       `a language rule` left 2104, is classed EarlyError, and is not among the codes Test262Run's
#       Refused grades unsupported; every other new code has the class of the code it left and is graded
#       unsupported exactly when that code is.
#
# and, from revision 19, E7's scope takes in every code the registry adds at revision 19 or later.
#
# The C# files are read as text, by the anchors their declarations have today: the enumeration's
# `Name = number,` lines; LanguageErrors' `Classify(...) => code switch` arms and its `Classified` set;
# and, in Test262Run, every SliceSourceDiagnosticCode named between `static Test262Outcome Refused(`
# and the first `Test262Verdict.Unsupported` after it, which are the codes graded unsupported. An
# anchor that is gone is an input that cannot be read (exit 2), never a pass.
#
# --controls <directory> runs this checker's own controls: the directory's controls.json names a
# fixture (a small registry, mapping, catalogue and stage table copy, and the four files above, at
# revision 19), which must pass, and edits of it, each of which must be refused by the checks it names;
# and edits of this checkout's own mapping and registry. See that directory's README.md.
#
# Exit status: 0 when nothing is refused, 1 when anything is, 2 when an input cannot be read.

import argparse
import hashlib
import json
import os
import re
import shutil
import subprocess
import sys
import tempfile

TOOL = "check-frontend-mapping"
ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DEFAULT_MAPPING = "src/Broiler.VM.Profile.JavaScript/docs/diagnostics/frontend-mapping.tsv"
# The catalogue's VM site inventory in Broiler.JS, which its completeness report reads: one line
# per site, with the registry code it refuses with and the catalogue rows it is assigned to.
SITES_PATH = "docs/evidence/sjl-12-001/catalogue/vm-sites.tsv"
SITES_COLUMNS = ("site", "code", "rows")

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

# Revision 19's checks (E10 to E14), inert below it.
REVISION_19 = 19
STAGE_TABLE_KEYS = ("stage-table-path", "stage-table-commit", "stage-table-sha256", "stage-table-copy")
DEFAULT_ENUM = "src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceDiagnostics.cs"
DEFAULT_LANGUAGE_ERRORS = "src/compositions/Broiler.VM.Composition.JavaScript.Conformance/LanguageErrors.cs"
DEFAULT_TEST262_RUN = "src/compositions/Broiler.VM.Composition.JavaScript.Conformance/Test262Run.cs"
DEFAULT_SOURCE_MANIFEST = "src/tests/corpus/js-1/source/source.manifest"
SEAM_STAGES = ("tokenizer", "parser", "semantics", "lowering")
LEFT = re.compile(r"\bleft (?P<code>\d{4}) (?P<name>[A-Za-z]+)\b")
LANGUAGE_RULE = "a language rule"
CONSTRUCT_OUTSIDE_MANIFEST = "2104"
EARLY_ERROR = "EarlyError"


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
            if match and (match.group(1) in HEADER_KEYS or match.group(1) in STAGE_TABLE_KEYS):
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
    revision, codes, _ = read_registry_header(path)
    return revision, codes


def read_registry_header(path):
    # The registry's rows, and what its header states: its owning decision and the text of its
    # published-at-milestone list, which E12 reads from revision 19.
    _, text = read_text(path, "registry")
    revision, codes = None, {}
    header = {"owning-decision": None, "published-at-milestone": ""}
    in_published = False
    for line in text.split("\n"):
        match = re.match(r"^# registry-revision: (\d+)\s*$", line)
        if match:
            revision = match.group(1)
            in_published = False
            continue
        key = re.match(r"^# ([a-z-]+): (.*)$", line)
        if key:
            in_published = key.group(1) == "published-at-milestone"
            if in_published:
                header["published-at-milestone"] = key.group(2).strip()
            elif key.group(1) == "owning-decision":
                header["owning-decision"] = key.group(2).strip()
            continue
        if in_published and re.match(r"^#\s{2,}\S", line):
            header["published-at-milestone"] += " " + line[1:].strip()
            continue
        in_published = False
        if not line or line.startswith("#"):
            continue
        cells = line.split("|")
        if len(cells) != 8 or not CODE.match(cells[0]):
            raise Unreadable(f"{path}: a row that is not code|name|half|reason|stage|reachability|case|since: {line}")
        codes[cells[0]] = {"name": cells[1], "half": cells[2], "stage": cells[4], "reachability": cells[5],
                           "case": cells[6], "since": cells[7]}
    if revision is None:
        raise Unreadable(f"{path}: no registry-revision line")
    return revision, codes, header


def git_show(checkout, commit, path):
    try:
        return subprocess.run(["git", "-C", checkout, "show", f"{commit}:{path}"],
                              check=True, capture_output=True).stdout
    except (OSError, subprocess.CalledProcessError) as error:
        raise Unreadable(f"{path} at {commit} cannot be read from {checkout}: {error}")


def read_sites(data, where):
    # The inventory's sites, as {catalogue row: {registry code: [site, ...]}}.
    try:
        text = data.decode("utf-8")
    except UnicodeDecodeError:
        raise Unreadable(f"{where} is not UTF-8")
    lines = [line for line in text.split("\n") if line]
    if not lines:
        raise Unreadable(f"{where}: empty")
    columns = lines[0].split("\t")
    for column in SITES_COLUMNS:
        if column not in columns:
            raise Unreadable(f"{where}: the inventory has no {column} column")
    assigned = {}
    for number, line in enumerate(lines[1:], start=2):
        cells = line.split("\t")
        if len(cells) != len(columns):
            raise Unreadable(f"{where}:{number}: {len(cells)} cells where there are {len(columns)} columns")
        site = dict(zip(columns, cells))
        if not CODE.match(site["code"]):
            raise Unreadable(f"{where}:{number}: the code {site['code']!r} is no registry code")
        for row in split_codes(site["rows"]):
            assigned.setdefault(row, {}).setdefault(site["code"], []).append(site["site"])
    return hashlib.sha256(data).hexdigest(), len(lines) - 1, assigned


def shown(path):
    full = os.path.abspath(path)
    relative = os.path.relpath(full, ROOT)
    return full if relative.startswith("..") else relative


def split_codes(cell):
    if cell in ("-", "none"):
        return []
    return cell.split(";")


def check(mapping_path, catalogue_override, registry_path, broiler_js, root=ROOT, sources=None):
    # `root` is the directory the header's paths are read under; `sources` names the four files the
    # revision-19 checks read ({"enum", "language-errors", "test262-run", "source-manifest"}), each
    # under `root` by default. Neither is read below revision 19.
    sources = dict(sources or {})
    header, mapping = read_mapping(mapping_path)
    copy_path = catalogue_override or os.path.join(root, header["catalogue-copy"])
    digest, catalogue = read_catalogue(copy_path)
    revision, registry, registry_header = read_registry_header(
        registry_path or os.path.join(root, header["registry"]))
    revision_19 = int(revision) >= REVISION_19

    problems = []

    def refuse(check_id, message):
        problems.append(f"{check_id} {message}")

    # E1 - the digest the header names is the copy's.
    if digest != header["catalogue-sha256"]:
        refuse("E1", f"the catalogue copy {shown(copy_path)} has SHA-256 {digest}, "
                     f"and the mapping's header names {header['catalogue-sha256']}")
    inventory, assigned = None, None
    if broiler_js:
        committed = git_show(broiler_js, header["catalogue-commit"], header["catalogue-path"])
        committed_digest = hashlib.sha256(committed).hexdigest()
        if committed_digest != digest:
            refuse("E1", f"{header['catalogue-path']} at {header['catalogue-commit']} has SHA-256 "
                         f"{committed_digest}, and the copy has {digest}")
        sites_digest, site_count, assigned = read_sites(
            git_show(broiler_js, header["catalogue-commit"], SITES_PATH),
            f"{SITES_PATH} at {header['catalogue-commit']}")
        inventory = (sites_digest, site_count)

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
        listed, by_manifest, columns_of = set(), False, {}
        for column in ("wide_codes", "slice_codes"):
            for code in split_codes(row[column]):
                if code == "manifest" and column == "slice_codes":
                    by_manifest = True
                elif CODE.match(code):
                    if known(code, f"{where} {column}"):
                        listed.add(code)
                        columns_of.setdefault(code, []).append(column)
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
        if assigned is not None:
            # With --broiler-js: the row lists what the catalogue row's cells and the inventory's
            # sites give it, both ways.
            sites = assigned.get(source["id"], {})
            for code in sorted(listed - named - set(sites)):
                verb = "list" if len(columns_of[code]) > 1 else "lists"
                refuse("E8", f"{where}: {' and '.join(columns_of[code])} {verb} {code}, which neither "
                             f"the catalogue row's vm_code nor a site {SITES_PATH} assigns to "
                             f"{source['id']} gives")
            for code, at in sorted(sites.items()):
                if code not in listed and not (code == "2104" and by_manifest):
                    refuse("E8", f"{where}: {SITES_PATH} assigns {', '.join(at)} to {source['id']}, "
                                 f"which refuses with {code}, and the mapping row does not list it")
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

    # The codes the registry adds at revision 19 or later; empty below revision 19.
    new_codes = sorted(code for code, row in registry.items()
                       if revision_19 and row["half"] == "embedder-seam" and row["since"].isdigit()
                       and int(row["since"]) >= REVISION_19)

    # E7 - every registry code in the ranges is reached within its meaning, or explained; from
    # revision 19, every code the registry adds too, wherever its number falls.
    for code in sorted(registry):
        number = int(code)
        if code in NOT_REQUIRED or not (any(low <= number <= high for low, high in REACHED_RANGES)
                                        or code in new_codes):
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

    if revision_19:
        check_revision_19(header, revision, registry, registry_header, registry_rows, code_rows, new_codes,
                          broiler_js, root, sources, refuse)

    return header, digest, inventory, len(code_rows), len(registry_rows), problems


def read_enum(path):
    # SliceSourceDiagnosticCode's members, {name: number}, from its `Name = number,` lines.
    _, text = read_text(path, "SliceSourceDiagnosticCode declaration")
    start = text.find("enum SliceSourceDiagnosticCode")
    if start < 0:
        raise Unreadable(f"{path}: no `enum SliceSourceDiagnosticCode`")
    open_brace = text.find("{", start)
    close_brace = text.find("\n}", open_brace)
    if open_brace < 0 or close_brace < 0:
        raise Unreadable(f"{path}: the enumeration's body cannot be found")
    members = {}
    for match in re.finditer(r"^\s*([A-Za-z]\w*)\s*=\s*(\d+)\s*,?\s*$", text[open_brace:close_brace], re.M):
        members[match.group(1)] = match.group(2)
    if not members:
        raise Unreadable(f"{path}: the enumeration declares no `Name = number,` member")
    return members


def region(text, path, opening, closing, what):
    start = text.find(opening)
    if start < 0:
        raise Unreadable(f"{path}: no `{opening}` ({what})")
    end = text.find(closing, start + len(opening))
    if end < 0:
        raise Unreadable(f"{path}: `{opening}` is not closed by `{closing}` ({what})")
    return text[start:end]


def read_language_errors(path):
    # LanguageErrors' classes, {name: class}, from Classify's arms, and the names Classified lists.
    _, text = read_text(path, "LanguageErrors")
    arms = region(text, path, "Classify(SliceSourceDiagnosticCode code) => code switch", "};", "Classify")
    classes = {match.group(1): match.group(2) for match in re.finditer(
        r"SliceSourceDiagnosticCode\.(\w+)\s*=>\s*RefusalClass\.(\w+)", arms)}
    listed = region(text, path, "Classified { get; } =", "};", "Classified")
    classified = set(re.findall(r"SliceSourceDiagnosticCode\.(\w+)", listed))
    if not classes or not classified:
        raise Unreadable(f"{path}: Classify gives no arm, or Classified lists no code")
    return classes, classified


def read_unsupported(path):
    # The codes Test262Run's Refused grades unsupported: every SliceSourceDiagnosticCode it names
    # before the first `Test262Verdict.Unsupported` of its body.
    _, text = read_text(path, "Test262Run")
    graded = region(text, path, "static Test262Outcome Refused(", "Test262Verdict.Unsupported", "Refused")
    names = set(re.findall(r"SliceSourceDiagnosticCode\.(\w+)", graded))
    if not names:
        raise Unreadable(f"{path}: Refused grades no code unsupported, so its anchor no longer reads")
    return names


def read_source_cases(path):
    # The retained source corpus's refused entries, {name: answer}.
    _, text = read_text(path, "source corpus manifest")
    cases = {}
    for line in text.split("\n"):
        cells = line.split("|")
        if len(cells) == 5 and cells[0] == "refused":
            cases[cells[1]] = cells[3]
    if not cases:
        raise Unreadable(f"{path}: no refused entry")
    return cases


def check_revision_19(header, revision, registry, registry_header, registry_rows, code_rows, new_codes,
                      broiler_js, root, sources, refuse):
    # E10 - no split relation remains.
    for row in code_rows:
        if row["relation"] == "split":
            refuse("E10", f"line {row['line']} ({row['shared_code']}, {row['row']}): marked split at registry "
                          f"revision {revision}; no split relation remains from revision {REVISION_19} "
                          f"(JSD-0083 decision D12)")

    # E11 - the stage table the header names, by its copy's digest.
    missing = [key for key in STAGE_TABLE_KEYS if key not in header]
    if missing:
        refuse("E11", f"at registry revision {revision} the header does not name {', '.join(missing)}")
    else:
        copy = os.path.join(root, header["stage-table-copy"])
        data, _ = read_text(copy, "stage table copy")
        digest = hashlib.sha256(data).hexdigest()
        if digest != header["stage-table-sha256"]:
            refuse("E11", f"the stage table copy {shown(copy)} has SHA-256 {digest}, and the mapping's header "
                          f"names {header['stage-table-sha256']}")
        if broiler_js:
            committed = hashlib.sha256(
                git_show(broiler_js, header["stage-table-commit"], header["stage-table-path"])).hexdigest()
            if committed != digest:
                refuse("E11", f"{header['stage-table-path']} at {header['stage-table-commit']} has SHA-256 "
                              f"{committed}, and the copy has {digest}")

    # E12 - the registry states its owning decision and names its revision.
    owner = registry_header["owning-decision"]
    if not owner or not re.match(r"^JSD-\d{4}\b", owner):
        refuse("E12", "registry.txt names no owning decision (`# owning-decision: JSD-nnnn`)")
    published = registry_header["published-at-milestone"]
    if not re.search(rf"\brevisions?\b[^;()]*?\b{revision}\b", published):
        refuse("E12", f"registry.txt states revision {revision} and its published-at-milestone list does not "
                      f"name it")

    if not new_codes:
        return

    def source(key, default):
        return sources.get(key) or os.path.join(root, default)

    members = read_enum(source("enum", DEFAULT_ENUM))
    classes, classified = read_language_errors(source("language-errors", DEFAULT_LANGUAGE_ERRORS))
    unsupported = read_unsupported(source("test262-run", DEFAULT_TEST262_RUN))
    cases = read_source_cases(source("source-manifest", DEFAULT_SOURCE_MANIFEST))
    registry_row_of = {row["registry_code"]: row for row in registry_rows}

    for code in new_codes:
        entry = registry[code]
        name = entry["name"]
        where = f"registry {code} {name} (since {entry['since']})"

        # E13 - the code's member, its row's stage and reachability, its case, and the code it left.
        if members.get(name) != code:
            refuse("E13", f"{where}: SliceSourceDiagnosticCode has no member {name} = {code}")
        if entry["stage"] not in SEAM_STAGES:
            refuse("E13", f"{where}: the stage {entry['stage']!r} is not one of {', '.join(SEAM_STAGES)}")
        if entry["reachability"] != "source":
            refuse("E13", f"{where}: the reachability is {entry['reachability']!r}, and a new code is reached "
                          f"from a case of the retained source corpus (rule N7 lists every defensive row)")
        elif cases.get(entry["case"]) != name:
            refuse("E13", f"{where}: the case {entry['case']!r} is not a refused entry of the retained source "
                          f"corpus answered with {name}")
        mapping_row = registry_row_of.get(code)
        left = LEFT.search(mapping_row["notes"]) if mapping_row else None
        if mapping_row is None:
            refuse("E13", f"{where}: the mapping has no registry row for it")
        elif not left:
            refuse("E13", f"{where}: the mapping's registry row (line {mapping_row['line']}) does not name the "
                          f"code it left, as `left <code> <Name>`")
        elif (left.group("code") not in registry or registry[left.group("code")]["name"] != left.group("name")
              or not registry[left.group("code")]["since"].isdigit()
              or int(registry[left.group("code")]["since"]) >= int(entry["since"])
              or registry[left.group("code")]["half"] != "embedder-seam"):
            refuse("E13", f"{where}: it names `left {left.group('code')} {left.group('name')}`, which is no "
                          f"embedder-seam code of an earlier revision under that name")
            left = None

        # E14 - its class, and its grading.
        if name not in classified or name not in classes:
            refuse("E14", f"{where}: LanguageErrors gives it no class (Classified lists it: "
                          f"{'yes' if name in classified else 'no'}; Classify has an arm: "
                          f"{'yes' if name in classes else 'no'})")
            continue
        if left is None:
            continue
        left_code, left_name = left.group("code"), left.group("name")
        graded = name in unsupported
        if LANGUAGE_RULE in mapping_row["notes"]:
            if left_code != CONSTRUCT_OUTSIDE_MANIFEST:
                refuse("E14", f"{where}: named a language rule, and it left {left_code}, where decision D12's "
                              f"three language rules left {CONSTRUCT_OUTSIDE_MANIFEST}")
            if classes[name] != EARLY_ERROR or graded:
                refuse("E14", f"{where}: a language rule is classed {EARLY_ERROR} and not graded unsupported "
                              f"(decision D12); it is classed {classes[name]} and "
                              f"{'graded' if graded else 'not graded'} unsupported")
        else:
            if classes[name] != classes.get(left_name):
                refuse("E14", f"{where}: classed {classes[name]}, and the code it left, {left_code} {left_name}, "
                              f"is classed {classes.get(left_name, 'nothing')}")
            if graded != (left_name in unsupported):
                refuse("E14", f"{where}: Test262Run {'grades' if graded else 'does not grade'} it unsupported, "
                              f"and {'grades' if left_name in unsupported else 'does not grade'} the code it "
                              f"left, {left_code} {left_name}, so (decision D12: graded as the code it left)")


def run_controls(directory):
    # The checker's own controls (see the directory's README.md): a fixture at revision 19 that must
    # pass, edits of it each of which must be refused by the checks it names, and edits of this
    # checkout's own mapping and registry. Every edit replaces a text that occurs exactly once.
    try:
        with open(os.path.join(directory, "controls.json"), encoding="utf-8") as handle:
            plan = json.load(handle)
    except (OSError, ValueError) as error:
        print(f"{TOOL}: cannot run the controls: {error}")
        return 2

    def edited(source_directory, names, edits, target):
        for name in names:
            shutil.copyfile(os.path.join(source_directory, name), os.path.join(target, name))
        for edit in edits:
            path = os.path.join(target, edit["file"])
            with open(path, encoding="utf-8", newline="") as handle:
                text = handle.read()
            if text.count(edit["old"]) != 1:
                raise Unreadable(f"the edit of {edit['file']} names a text that occurs "
                                 f"{text.count(edit['old'])} times, not once: {edit['old'][:60]!r}")
            with open(path, "w", encoding="utf-8", newline="") as handle:
                handle.write(text.replace(edit["old"], edit["new"]))

    def outcome(run):
        try:
            return run()[-1], None
        except Unreadable as error:
            return None, str(error)

    fixture = os.path.join(directory, plan["fixture"])
    files = plan["files"]
    names = sorted(set(files.values()) | set(plan["fixture-files"]))
    failures = 0

    def fixture_run(target):
        return lambda: check(
            os.path.join(target, files["mapping"]), None, None, None, root=target,
            sources={key: os.path.join(target, files[key])
                     for key in ("enum", "language-errors", "test262-run", "source-manifest")})

    def real_run(target, sources):
        return lambda: check(
            os.path.join(target, "frontend-mapping.tsv"), None, os.path.join(target, "registry.txt"), None,
            root=ROOT, sources=sources)

    cases = [("fixture", {"id": "F0", "what": "the fixture, unedited", "edits": [], "refused": []})]
    cases += [("fixture", control) for control in plan["controls"]]
    cases += [("real", control) for control in plan["real"]]

    for kind, control in cases:
        with tempfile.TemporaryDirectory() as target:
            try:
                if kind == "fixture":
                    edited(fixture, names, control["edits"], target)
                    run = fixture_run(target)
                else:
                    shutil.copyfile(os.path.join(ROOT, DEFAULT_MAPPING), os.path.join(target, "frontend-mapping.tsv"))
                    shutil.copyfile(os.path.join(ROOT, "src/Broiler.VM.Profile.JavaScript/docs/diagnostics/registry.txt"),
                                    os.path.join(target, "registry.txt"))
                    edited(target, [], control["edits"], target)
                    sources = {key: os.path.join(target, value) for key, value in control.get("sources", {}).items()}
                    run = real_run(target, sources)
            except (Unreadable, OSError) as error:
                print(f"{TOOL}: control {control['id']} cannot be made: {error}")
                return 2
            problems, unreadable = outcome(run)
        expected = control["refused"]
        if unreadable is not None:
            seen = "could not be read: " + unreadable
            met = False
        else:
            ids = sorted({problem.split(" ", 1)[0] for problem in problems})
            seen = "passed" if not problems else "refused at " + ", ".join(ids)
            met = (not problems) if not expected else all(check_id in ids for check_id in expected)
        failures += 0 if met else 1
        wanted = "to pass" if not expected else "to be refused at " + ", ".join(expected)
        print(f"{TOOL}: control {control['id']} ({control['what']}): {seen}; "
              f"{'as stated' if met else 'NOT AS STATED'} ({wanted})")
        if problems and not met or (problems and os.environ.get("CHECK_FRONTEND_MAPPING_VERBOSE")):
            for problem in problems:
                print(f"{TOOL}:     {problem}")
    if failures:
        print(f"{TOOL}: CONTROLS NOT AS STATED: {failures} of {len(cases)}")
        return 1
    print(f"{TOOL}: controls: all {len(cases)} behave as stated")
    return 0


def main():
    parser = argparse.ArgumentParser(
        description="Hold the front end's diagnostic mapping to the early-error catalogue and the "
                    "registry (bundle sjl-12-001, part E; from registry revision 19, bundle sjl-13-001, "
                    "part E).")
    parser.add_argument("--mapping", default=os.path.join(ROOT, DEFAULT_MAPPING),
                        help="the mapping (default: %(default)s)")
    parser.add_argument("--catalogue", default=None,
                        help="the catalogue copy (default: the copy the mapping's header names)")
    parser.add_argument("--registry", default=None,
                        help="the registry (default: the one the mapping's header names)")
    parser.add_argument("--broiler-js", default=None,
                        help="a Broiler.JS checkout: also compare the copy with the catalogue at the "
                             "commit the header names, and hold each row's codes to the catalogue's "
                             "VM site inventory there")
    parser.add_argument("--root", default=ROOT,
                        help="the directory the header's paths and the files below are read under "
                             "(default: this checkout)")
    parser.add_argument("--enum-source", default=None,
                        help=f"from revision 19, SliceSourceDiagnosticCode's declaration (default: {DEFAULT_ENUM})")
    parser.add_argument("--language-errors", default=None,
                        help=f"from revision 19, LanguageErrors.cs (default: {DEFAULT_LANGUAGE_ERRORS})")
    parser.add_argument("--test262-run", default=None,
                        help=f"from revision 19, Test262Run.cs (default: {DEFAULT_TEST262_RUN})")
    parser.add_argument("--source-manifest", default=None,
                        help=f"from revision 19, the retained source corpus's manifest (default: "
                             f"{DEFAULT_SOURCE_MANIFEST})")
    parser.add_argument("--controls", default=None,
                        help="run this checker's controls from the directory named, and nothing else")
    arguments = parser.parse_args()

    if arguments.controls:
        return run_controls(arguments.controls)

    sources = {"enum": arguments.enum_source, "language-errors": arguments.language_errors,
               "test262-run": arguments.test262_run, "source-manifest": arguments.source_manifest}
    try:
        header, digest, inventory, codes, registry_rows, problems = check(
            arguments.mapping, arguments.catalogue, arguments.registry, arguments.broiler_js,
            root=arguments.root, sources=sources)
    except Unreadable as error:
        print(f"{TOOL}: cannot check: {error}")
        return 2

    revision_19 = int(header["registry-revision"]) >= REVISION_19
    print(f"{TOOL}: mapping {shown(arguments.mapping)}")
    print(f"{TOOL}: catalogue {header['catalogue-repository']} {header['catalogue-path']} at "
          f"{header['catalogue-commit']}, copy SHA-256 {digest}")
    if inventory is not None:
        print(f"{TOOL}: inventory {header['catalogue-repository']} {SITES_PATH} at "
              f"{header['catalogue-commit']}, SHA-256 {inventory[0]}, {inventory[1]} sites")
    if revision_19 and all(key in header for key in STAGE_TABLE_KEYS):
        print(f"{TOOL}: stage table {header['catalogue-repository']} {header['stage-table-path']} at "
              f"{header['stage-table-commit']}, copy SHA-256 {header['stage-table-sha256']}")
    print(f"{TOOL}: registry revision {header['registry-revision']}")
    for problem in problems:
        print(f"{TOOL}: refused: {problem}")
    if problems:
        print(f"{TOOL}: REFUSED: {len(problems)} problem(s)")
        return 1
    print(f"{TOOL}: passed: {codes} shared codes, each mapped to one registry code; "
          f"{registry_rows} registry rows; "
          + ("checks E1 to E14 hold" if revision_19 else
             f"checks E1 to E9 hold, and E10 to E14 are inert below registry revision {REVISION_19}"))
    return 0


if __name__ == "__main__":
    sys.exit(main())
