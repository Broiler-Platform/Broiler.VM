"""WP7.2 step 6's tables. Mode `rows LOG`: print each (c') row of a compare-<run>.log, read as bundle sjl-4-001's
collector's rerun step reads it (a row under the `## (c')` heading), one `path<TAB>variant<TAB>before<TAB>after` per
line. Mode `table LOG BASEDIR AFTERDIR PREFIX OUT`: for each (c') row, the verdict the row's path's re-run report
gives at each commit (BASEDIR/PREFIX-<n>.report and AFTERDIR/..., <n> the path's index in sorted order, as the
driver names them), read as that step reads a report (`result|` rows; an exhaustion with its dimension), written to
OUT with whether the two re-runs agree; it prints the rows whose re-runs differ. Mode `race ROWS BASEDIR AFTERDIR
PREFIX OUT`: the same for (e)'s runs, over the rows of ROWS (path<TAB>variant), with whether either is on the wall
clock. It reads and prints; it decides no class."""
import pathlib, re, sys


def rows(log):
    out, heading = [], ""
    for line in open(log, encoding="utf-8"):
        if line.startswith("## "):
            heading = line
        found = re.match(r"^  (test/\S+) \[(\w+)\]: (\S+) -> (\S+)", line)
        if found and heading.startswith("## (c')"):
            out.append(found.groups())
    return out


def verdicts(report):
    table = {}
    if pathlib.Path(report).is_file():
        for line in open(report, encoding="utf-8"):
            if line.startswith("result|"):
                parts = line.rstrip("\n").split("|")
                text = parts[3] + ("/" + parts[5] if parts[3] == "Exhausted" and len(parts) > 5 else "")
                table[(parts[1], parts[2])] = text
    return table


def by_path(keys, base_dir, after_dir, prefix):
    paths = sorted({k[0] for k in keys})
    index = {p: i for i, p in enumerate(paths)}
    got = {}
    for p in paths:
        b = verdicts(f"{base_dir}/{prefix}-{index[p]}.report")
        a = verdicts(f"{after_dir}/{prefix}-{index[p]}.report")
        for k in keys:
            if k[0] == p:
                got[k] = (b.get(k, "(absent)"), a.get(k, "(absent)"))
    return index, got


mode = sys.argv[1]
if mode == "rows":
    for r in rows(sys.argv[2]):
        print("\t".join(r))
elif mode == "table":
    log, base_dir, after_dir, prefix, out = sys.argv[2:7]
    rs = rows(log)
    index, got = by_path([(r[0], r[1]) for r in rs], base_dir, after_dir, prefix)
    lines = ["path\tvariant\tbase run\tafter run\tbase re-run\tafter re-run\tre-runs agree\tre-run report index"]
    for path, variant, before, after in rs:
        b, a = got[(path, variant)]
        agree = b != "(absent)" and b == a
        lines.append("\t".join((path, variant, before, after, b, a, "yes" if agree else "NO", str(index[path]))))
        if not agree:
            print(f"{path}\t{variant}")
    pathlib.Path(out).write_text("\n".join(lines) + "\n", encoding="utf-8")
elif mode == "race":
    rows_file, base_dir, after_dir, prefix, out = sys.argv[2:7]
    keys = [tuple(l.split("\t")[:2]) for l in open(rows_file, encoding="utf-8").read().splitlines() if l]
    index, got = by_path(keys, base_dir, after_dir, prefix)
    lines = ["path\tvariant\tbase\tafter\tsame verdict\teither on the wall clock\trace report index"]
    for k in keys:
        b, a = got[k]
        lines.append("\t".join((k[0], k[1], b, a, "yes" if b == a else "NO", "YES" if "WallClock" in b + a else "no",
                                str(index[k[0]]))))
    pathlib.Path(out).write_text("\n".join(lines) + "\n", encoding="utf-8")
