#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# JOIN TWO PARSE-CORPUS REPORTS AND NAME EVERY COMPILATION THAT DIFFERS (bundle sjl-12-001, parts B
# and C). Written 2026-10-20 at SJL-12.4, before any change the rule judges.
#
# The reports are what the slice compiler's `--parse-corpus` mode writes: one line per compilation,
# keyed on (input, sub-population, variant, form), carrying the outcome, the ordered diagnostics
# (code:line:column), and for an accepted compilation the artifact's SHA-256 and length. The rule is
# docs/evidence/sjl-12-001/decision-rule-area.md; this script is the comparison its part B names, and
# the four-hash comparison its part C names.
#
#   identity BASE AFTER --manifest M (--census BASE AFTER [--census BASE AFTER ...] | --no-census)
#            [--out F] [--differing-keys F]
#       Part B. Joined on the key, it compares the outcome; for a compilation accepted on both sides,
#       the artifact's hash and length; for one refused on both sides, the ordered diagnostic list;
#       for one not compiled on both sides, the note. It holds both reports to the manifest, which
#       is required: its digest, and every compilation the four sub-populations owe it, no more and
#       no fewer. Each --census pair is two `--census` outputs, compared byte for byte (a .gz file is
#       read decompressed, as a report is); one pair per `--census` call the run procedure makes.
#       The census is required too, because part B compares it between the corpus commit and the
#       after commit; --no-census says, explicitly, that this comparison is not that one (the base
#       against the ordering commit, where the rule takes no census, or part C's control K1).
#       --differing-keys writes the key of every compilation whose bytes differ (an artifact on
#       either side and the two hashes unequal), one per line, for `--parse-corpus --select` with
#       `--write-bytes` at each commit, because the rule retains those bytes.
#
#   determinism RUN1 RUN2 RUN3 --population AFTER --manifest M [--out F]
#       Part C. RUN1 and RUN2 must each say one worker in their header and RUN3 must be a
#       `--twice-concurrent` report of two. The population is every compilation of B1 and B3 that
#       AFTER (part B's after run, itself complete) accepted; for each, the four results - run 1,
#       run 2 and the two compilations of run 3 - must all be accepted, with one hash and one length.
#
#   control BASE OUT
#       Part B's control: a copy of BASE with one artifact hash changed and one outcome flipped. It
#       changes the first accepted B1 bytecode compilation's hash (its last hex digit) and turns the
#       second accepted B1 bytecode compilation into a refusal with no diagnostic, and says which in
#       a comment line. `identity BASE OUT --manifest M --no-census` must then report exactly those
#       two, both unclassified.
#
# THE OUTCOMES. The rule's are accepted and refused. The mode also writes `not-compiled`, in the two
# cases its reading 2 names (every report's header repeats the readings): B2's test262 `_FIXTURE`
# rows, note `fixture`, and B4's json rows, note `no-json-goal`. A `crashed` row (an exception left
# the compiler), or a `not-compiled` row outside those two cases, has no outcome the rule knows, so
# its run is not complete: `identity` lists each such row as `incomplete` and exits 2, and
# `determinism` refuses a population report that holds one and counts a crashed population row as
# one whose four results are not one.
#
# THE SCRIPT DECIDES NO CLASS. Every difference it reports is UNCLASSIFIED: the rule's classes O, S, T
# and P each need a reading this script cannot do - O and P the two byte files, T the source text,
# S whether a code is one the binder pass reports. What it adds is a SHAPE beside each difference,
# computed from the two rows alone, which says which class's mechanical form the difference has, if
# any: `S1` (the same entries, the after list in source order), `S2` (an entry the base repeated at one
# position appears once), `S3` (only entries the lowering keeps are gone: 2104, 2301, 2302, 2303, 2405,
# 2406, 2201, and 2103 at 0:0), `S4?` (entries added after; whether each is the binder's is a reading),
# `T?` (accepted at the base and refused after with only 2101 or 2103), `bytes` (an artifact
# difference, which the byte files decide), `B4` and `census` (where no class is admitted at all). A
# shape is a pointer for the reader, never a verdict.
#
# A REPORT IS REFUSED, NOT COMPARED, when it is not format 1, when a key repeats, when an outcome is
# not one of the four, when the two do not name the manifest's digest, when either was a selected
# run, or when either lacks a compilation the manifest owes or holds one it does not. A comparison
# over less than the whole corpus would read as a clean one.
#
# EXIT CODES: 0 when nothing differs, 1 when anything does, 2 when the input could not be read or was
# refused, or a run is not complete (argparse's own code for a bad command line).

import argparse
import collections
import gzip
import hashlib
import pathlib
import sys

REPORT_FORMAT = "broiler-vm-parse-corpus-report\t1"
MANIFEST_FORMAT = "broiler-parse-corpus\t1"
POPULATIONS = ("B1", "B2", "B3", "B4")
COLUMNS = ("id", "population", "variant", "form", "outcome", "diagnostics", "sha256", "length", "note")
SECOND = ("outcome-2", "diagnostics-2", "sha256-2", "length-2", "note-2")
OUTCOMES = ("accepted", "refused", "not-compiled", "crashed")

# The diagnostics JSD-0080 decision D12 keeps in the lowering, which class S3 lets the after list lose.
S3_CODES = {"2104", "2301", "2302", "2303", "2405", "2406", "2201"}

Result = collections.namedtuple("Result", "outcome diagnostics sha256 length note")


def refuse(message):
    """Stops with the input-could-not-be-read code, which is not the comparison's own 1."""
    print("compare-parse-corpus: refused: " + message, file=sys.stderr)
    raise SystemExit(2)


def read_bytes(path):
    data = pathlib.Path(path).read_bytes()
    return gzip.decompress(data) if data[:2] == b"\x1f\x8b" else data


class Report:
    """One `--parse-corpus` report, held to format 1."""

    def __init__(self, path):
        self.path = str(path)
        try:
            text = read_bytes(path).decode("utf-8")
        except (OSError, UnicodeDecodeError, EOFError, gzip.BadGzipFile) as failure:
            refuse(f"{path}: {failure}")
        if "\r" in text or not text.endswith("\n"):
            refuse(f"{path}: its lines do not all end in LF")
        lines = text[:-1].split("\n")
        self.header = {}
        at = 0
        while at < len(lines) and lines[at].startswith("#"):
            if lines[at].startswith("#% "):
                key, _, value = lines[at][3:].partition("\t")
                self.header[key] = value
            at += 1
        if self.header.get("format") != REPORT_FORMAT:
            refuse(f"{path}: not a parse-corpus report of format 1")
        if at >= len(lines):
            refuse(f"{path}: no column line")
        columns = tuple(lines[at].split("\t"))
        self.twice = self.header.get("twice-concurrent") == "yes"
        expected = COLUMNS + (SECOND if self.twice else ())
        if columns != expected:
            refuse(f"{path}: the column line is not format 1's")
        self.rows = {}
        self.second = {}
        self.order = []
        for number, line in enumerate(lines[at + 1:], start=at + 2):
            cells = line.split("\t")
            if len(cells) != len(expected):
                refuse(f"{path}: line {number} has {len(cells)} fields, not {len(expected)}")
            key = tuple(cells[:4])
            if key in self.rows:
                refuse(f"{path}: line {number} repeats the key {' '.join(key)}")
            self.rows[key] = Result(*cells[4:9])
            if self.twice:
                self.second[key] = Result(*cells[9:14])
            for result in (self.rows[key],) + ((self.second[key],) if self.twice else ()):
                if result.outcome not in OUTCOMES:
                    refuse(f"{path}: line {number} has the outcome {result.outcome!r}, not one of "
                           f"{', '.join(OUTCOMES)}")
            self.order.append(key)
        declared = self.header.get("compilations")
        if declared != str(len(self.rows)):
            refuse(f"{path}: declares {declared} compilations and holds {len(self.rows)}")

    @property
    def manifest_sha256(self):
        return self.header.get("manifest-sha256", "")

    @property
    def workers(self):
        return self.header.get("workers", "")

    def outcomes(self):
        """Per sub-population, how many compilations had each outcome (both answers of a twice run)."""
        counts = {population: collections.Counter() for population in POPULATIONS}
        for key, result in self.rows.items():
            counts[key[1]][result.outcome] += 1
            if self.twice:
                counts[key[1]][self.second[key].outcome] += 1
        return "; ".join(
            population + " " + (", ".join(f"{outcome} {counts[population][outcome]}" for outcome in OUTCOMES
                                          if counts[population][outcome]) or "none")
            for population in POPULATIONS)

    def whole(self):
        """Refuses a report that is not a complete, unselected run of all four sub-populations."""
        if self.header.get("selection") != "all":
            refuse(f"{self.path}: a selected run (--select) is not a whole run")
        if self.header.get("populations") != ",".join(POPULATIONS):
            refuse(f"{self.path}: covers {self.header.get('populations')}, not every sub-population")


def owed(manifest_path):
    """
    The manifest's digest, and every compilation the four sub-populations owe it, each mapped to the
    note under which reading 2 admits it not compiled, or None where it must have an outcome.
    """
    data = pathlib.Path(manifest_path).read_bytes()
    lines = data.decode("utf-8").rstrip("\n").split("\n")
    header = collections.defaultdict(list)
    at = 0
    while at < len(lines) and lines[at].startswith("#"):
        if lines[at].startswith("#% "):
            key, _, value = lines[at][3:].partition("\t")
            header[key].append(value)
        at += 1
    if header.get("format") != [MANIFEST_FORMAT]:
        refuse(f"{manifest_path}: not a parse-corpus manifest of format 1")
    eval_sets = [value.split("\t")[0] for value in header.get("eval-flags", [])]
    every = {}
    for line in lines[at + 1:]:
        cells = line.split("\t")
        if len(cells) != 9:
            refuse(f"{manifest_path}: a row is not nine fields")
        row_id, origin, path, goal = cells[0], cells[1], cells[2], cells[4]
        variants, eval_family = cells[5].split(","), cells[7]
        fixture = origin == "test262" and "_FIXTURE" in path
        for variant in variants:
            every[(row_id, "B1", variant, "bytecode")] = None
            every[(row_id, "B1", variant, "native")] = None
            if origin == "test262":
                every[(row_id, "B2", variant, "bytecode")] = "fixture" if fixture else None
        if eval_family == "yes":
            for name in eval_sets:
                every[(row_id, "B3", name, "bytecode")] = None
        # B4 IS ONE COMPILATION PER INPUT, UNDER ITS GOAL: the rule's B4 names no variant.
        every[(row_id, "B4", goal, "slice")] = "no-json-goal" if goal == "json" else None
    return hashlib.sha256(data).hexdigest(), every


def held_to(report, manifest_path, digest, every):
    """Refuses a report that does not name the manifest or does not hold exactly what it owes."""
    if report.manifest_sha256 != digest:
        refuse(f"{report.path}: names the manifest {report.manifest_sha256}, and {manifest_path} is {digest}")
    missing = every.keys() - report.rows.keys()
    extra = report.rows.keys() - every.keys()
    if missing or extra:
        sample = sorted(missing or extra)[0]
        refuse(f"{report.path}: {len(missing)} compilation(s) the manifest owes are missing and "
               f"{len(extra)} it does not owe are present, the first {' '.join(sample)}")


def incomplete(result, admitted):
    """Whether a result has no outcome the rule knows: crashed, or not compiled outside reading 2."""
    return result.outcome == "crashed" or (result.outcome == "not-compiled" and result.note != admitted)


def entries(diagnostics):
    return [] if diagnostics == "-" else diagnostics.split(";")


def position(entry):
    _, line, column = entry.split(":")
    return int(line), int(column)


def diagnostic_shape(base, after):
    """The mechanical shape of a difference between two refusals' lists, or `-` when it has none."""
    b, a = entries(base), entries(after)
    shapes = []
    collapsed = list(dict.fromkeys(b))
    if len(collapsed) != len(b):
        shapes.append("S2")
    removed = collections.Counter(collapsed) - collections.Counter(a)
    added = collections.Counter(a) - collections.Counter(collapsed)
    if removed:
        if all(entry.split(":")[0] in S3_CODES or entry == "2103:0:0" for entry in removed):
            shapes.append("S3")
        else:
            return "-"
    common_before = [entry for entry in collapsed if entry not in removed]
    common_after = [entry for entry in a if entry not in added]
    if common_before != common_after:
        if [position(entry) for entry in a] == sorted(position(entry) for entry in a):
            shapes.append("S1")
        else:
            return "-"
    if added:
        shapes.append("S4?")
    return "+".join(sorted(set(shapes), key=shapes.index)) or "-"


def first_entry(result):
    if result.outcome == "accepted":
        return "accepted " + result.sha256 + " " + result.length
    if result.outcome == "refused":
        return "refused " + result.diagnostics
    return result.outcome + " " + result.note


def compare(key, base, after):
    """What differs between two results under one key, as (what, base, after, shape), or None."""
    population = key[1]
    if base.outcome != after.outcome:
        shape = "-"
        if base.outcome == "accepted" and after.outcome == "refused":
            codes = {entry.split(":")[0] for entry in entries(after.diagnostics)}
            if codes and codes <= {"2101", "2103"}:
                shape = "T?"
        found = ("outcome", first_entry(base), first_entry(after), shape)
    elif base.outcome == "accepted":
        if (base.sha256, base.length) == (after.sha256, after.length):
            return None
        found = ("artifact", base.sha256 + " " + base.length, after.sha256 + " " + after.length, "bytes")
    elif base.outcome == "refused":
        if base.diagnostics == after.diagnostics:
            return None
        found = ("diagnostics", base.diagnostics, after.diagnostics,
                 diagnostic_shape(base.diagnostics, after.diagnostics))
    else:
        if base.note == after.note:
            return None
        found = ("note", base.note, after.note, "-")
    if population == "B4":
        found = found[:3] + ("B4",)
    return found


def emit(out, lines):
    text = "".join(line + "\n" for line in lines)
    if out:
        pathlib.Path(out).write_text(text, encoding="utf-8", newline="\n")
    sys.stdout.write(text if not out else "".join(line + "\n" for line in lines if line.startswith("#")))


def read_census(path):
    try:
        return read_bytes(path)
    except (OSError, EOFError, gzip.BadGzipFile) as failure:
        refuse(f"{path}: {failure}")


def identity(args):
    base, after = Report(args.base), Report(args.after)
    base.whole()
    after.whole()
    if base.manifest_sha256 != after.manifest_sha256:
        refuse(f"the two reports name two manifests ({base.manifest_sha256} and {after.manifest_sha256})")
    digest, every = owed(args.manifest)
    for report in (base, after):
        held_to(report, args.manifest, digest, every)

    differences = []
    unfinished = []
    for key in base.order:
        found = compare(key, base.rows[key], after.rows[key])
        if found is not None:
            differences.append((key, found))
        if incomplete(base.rows[key], every[key]) or incomplete(after.rows[key], every[key]):
            unfinished.append(key)

    censuses = []
    census_differs = False
    for number, (left_path, right_path) in enumerate(args.census or [], start=1):
        left, right = read_census(left_path), read_census(right_path)
        if left == right:
            censuses.append(f"# census {number} identical, byte for byte ({left_path}, {right_path})")
        else:
            census_differs = True
            a, b = left.decode("utf-8", "replace").split("\n"), right.decode("utf-8", "replace").split("\n")
            line = next((n for n, (x, y) in enumerate(zip(a, b), start=1) if x != y), min(len(a), len(b)) + 1)
            censuses.append(f"# census {number} DIFFERS from line {line} ({left_path}, {right_path}); "
                            f"no class admits a census difference")
    if args.no_census:
        censuses.append("# census not compared (--no-census): part B compares it only between the corpus "
                        "commit and the after commit, and this comparison was named as another")

    by_population = collections.Counter(key[1] for key, _ in differences)
    by_what = collections.Counter(found[0] for _, found in differences)
    lines = [
        "# compare-parse-corpus identity (bundle sjl-12-001 part B); the script decides no class",
        f"# base {base.path}",
        f"# after {after.path}",
        f"# manifest-sha256 {base.manifest_sha256} (both reports checked against {args.manifest}: its "
        f"digest, and every owed compilation present and no other)",
        f"# compilations {len(base.rows)}",
        f"# base outcomes {base.outcomes()}",
        f"# after outcomes {after.outcomes()}",
        f"# incomplete {len(unfinished)} (crashed, or not compiled outside reading 2, on either side)"
        + ("; THE RUNS ARE NOT COMPLETE, and each such row is listed below as incomplete" if unfinished else ""),
        f"# differences {len(differences)}, every one unclassified until the bundle lists it with a class",
        "# by sub-population " + (", ".join(f"{p} {by_population[p]}" for p in POPULATIONS)),
        "# by what differs " + (", ".join(f"{w} {by_what[w]}" for w in
                                           ("outcome", "artifact", "diagnostics", "note"))),
        f"# B4 differences {by_population['B4']} (no class admits one)",
        *censuses,
        "id\tpopulation\tvariant\tform\twhat\tbase\tafter\tstatus\tshape",
    ]
    for key, (what, left, right, shape) in differences:
        lines.append("\t".join(key + (what, left, right, "unclassified", shape)))
    for key in unfinished:
        lines.append("\t".join(key + ("incomplete", first_entry(base.rows[key]), first_entry(after.rows[key]),
                                       "incomplete", "-")))
    emit(args.out, lines)

    if args.differing_keys:
        keys = [key for key, found in differences
                if found[0] == "artifact" or (found[0] == "outcome" and "accepted" in
                                              (base.rows[key].outcome, after.rows[key].outcome))]
        pathlib.Path(args.differing_keys).write_text(
            "".join("\t".join(key) + "\n" for key in keys), encoding="utf-8", newline="\n")

    if unfinished:
        print(f"compare-parse-corpus: {len(unfinished)} compilation(s) crashed or were not compiled "
              f"outside reading 2: the runs are not complete", file=sys.stderr)
        return 2
    return 1 if differences or census_differs else 0


def determinism(args):
    runs = [Report(path) for path in (args.run1, args.run2, args.run3)]
    population_report = Report(args.population)
    population_report.whole()
    digest, every = owed(args.manifest)
    held_to(population_report, args.manifest, digest, every)
    unfinished = [key for key in population_report.order if incomplete(population_report.rows[key], every[key])]
    if unfinished:
        refuse(f"{population_report.path}: part B's after run is not complete: {len(unfinished)} "
               f"compilation(s) crashed or were not compiled outside reading 2, the first "
               f"{' '.join(unfinished[0])}")
    for number, report in enumerate(runs[:2], start=1):
        if report.twice or report.workers != "1":
            refuse(f"{report.path}: run {number} is not a run with one worker (its header says workers "
                   f"{report.workers or 'nothing'}, twice-concurrent {'yes' if report.twice else 'no'})")
    if not runs[2].twice or runs[2].workers != "2":
        refuse(f"{runs[2].path}: run 3 is not a --twice-concurrent report of two workers")
    for report in runs:
        if report.manifest_sha256 != digest:
            refuse(f"{report.path}: names the manifest {report.manifest_sha256}, and {args.manifest} is {digest}")
        if report.header.get("selection") != "all":
            refuse(f"{report.path}: a selected run (--select) is not a whole run")

    population = [key for key in population_report.order
                  if key[1] in ("B1", "B3") and population_report.rows[key].outcome == "accepted"]
    missing = [key for key in population if any(key not in report.rows for report in runs)]
    if missing:
        refuse(f"{len(missing)} accepted compilation(s) of the population are missing from a run, the "
               f"first {' '.join(missing[0])}")

    # ONE HASH IN ALL FOUR COMPILATIONS, AND EACH OF THEM ACCEPTED: four equal refusals, crashes or
    # non-compilations give no hash at all, which part C's MET condition does not admit.
    differing = []
    for key in population:
        four = [runs[0].rows[key], runs[1].rows[key], runs[2].rows[key], runs[2].second[key]]
        values = [(result.outcome, result.sha256, result.length) for result in four]
        if len(set(values)) != 1 or any(value[0] != "accepted" for value in values):
            differing.append((key, values))

    lines = [
        "# compare-parse-corpus determinism (bundle sjl-12-001 part C); no class is admitted",
        f"# run 1 {runs[0].path} (one worker)",
        f"# run 2 {runs[1].path} (one worker)",
        f"# run 3 {runs[2].path} (two workers, every compilation twice concurrently)",
        f"# population {population_report.path}: the B1 and B3 compilations it accepted",
        f"# manifest-sha256 {digest} (every report checked against {args.manifest})",
        f"# run 1 outcomes {runs[0].outcomes()}",
        f"# run 2 outcomes {runs[1].outcomes()}",
        f"# run 3 outcomes, both compilations {runs[2].outcomes()}",
        f"# compilations in the population {len(population)}",
        f"# compilations whose four results are not one accepted hash {len(differing)}",
        "id\tpopulation\tvariant\tform\trun-1\trun-2\trun-3a\trun-3b",
    ]
    for key, values in differing:
        lines.append("\t".join(key + tuple(" ".join(value) for value in values)))
    emit(args.out, lines)
    return 1 if differing else 0


def control(args):
    data = read_bytes(args.base).decode("utf-8")
    report = Report(args.base)
    report.whole()
    accepted = [key for key in report.order
                if key[1] == "B1" and key[3] == "bytecode" and report.rows[key].outcome == "accepted"]
    if len(accepted) < 2:
        refuse(f"{args.base}: fewer than two accepted B1 bytecode compilations to change")
    changed, flipped = accepted[0], accepted[1]
    lines = data[:-1].split("\n")
    out = []
    for line in lines:
        cells = line.split("\t")
        key = tuple(cells[:4])
        if not line.startswith("#") and key == changed:
            digest = cells[6]
            cells[6] = digest[:-1] + format(int(digest[-1], 16) ^ 1, "x")
        elif not line.startswith("#") and key == flipped:
            cells[4], cells[5], cells[6], cells[7], cells[8] = "refused", "-", "-", "-", "-"
        out.append("\t".join(cells))
    note = (f"# control (bundle sjl-12-001 part B): {' '.join(changed)} has its hash changed and "
            f"{' '.join(flipped)} is flipped from accepted to refused; nothing else differs from "
            f"{pathlib.Path(args.base).name}")
    column = next(index for index, line in enumerate(out) if not line.startswith("#"))
    out.insert(column, note)
    text = ("\n".join(out) + "\n").encode("utf-8")
    pathlib.Path(args.out).write_bytes(gzip.compress(text, mtime=0) if args.out.endswith(".gz") else text)
    print(note)
    return 0


def main():
    parser = argparse.ArgumentParser(description="Compare parse-corpus reports (bundle sjl-12-001).")
    commands = parser.add_subparsers(dest="command", required=True)

    one = commands.add_parser("identity", help="part B: join a base report and an after report")
    one.add_argument("base")
    one.add_argument("after")
    one.add_argument("--manifest", required=True,
                     help="hold both reports to this manifest's digest and population")
    census = one.add_mutually_exclusive_group(required=True)
    census.add_argument("--census", nargs=2, action="append", metavar=("BASE", "AFTER"),
                        help="two --census outputs (.gz read decompressed); once per --census call made")
    census.add_argument("--no-census", action="store_true",
                        help="say that this is not a corpus-commit-to-after-commit comparison")
    one.add_argument("--out", help="write the comparison here; the summary still goes to stdout")
    one.add_argument("--differing-keys", help="write the keys whose bytes differ here")
    one.set_defaults(run=identity)

    two = commands.add_parser("determinism", help="part C: the four hashes of every accepted B1 and B3 input")
    two.add_argument("run1")
    two.add_argument("run2")
    two.add_argument("run3")
    two.add_argument("--population", required=True, help="part B's after report")
    two.add_argument("--manifest", required=True, help="the corpus commit's manifest")
    two.add_argument("--out")
    two.set_defaults(run=determinism)

    three = commands.add_parser("control", help="part B's control: one hash changed, one outcome flipped")
    three.add_argument("base")
    three.add_argument("out")
    three.set_defaults(run=control)

    args = parser.parse_args()
    return args.run(args)


if __name__ == "__main__":
    sys.exit(main())
