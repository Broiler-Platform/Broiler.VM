"""Read one hosted test262-evidence folder's part A completeness facts, as the rule states them, for
one run (wide-bytecode or wide-native): every shard of the partition reported, the run log's exit code,
the merged report's coverage and retention objects, and the number of distinct assembly lists in the
binaries log. It prints what it reads; it decides nothing. (WP7.2, written for these records.)"""
import json, pathlib, re, sys
folder, run = pathlib.Path(sys.argv[1]), sys.argv[2]
report = json.loads((folder / f"{run}.test262.json").read_text(encoding="utf-8"))
count = report["partition"]["shardCount"]
shards = sorted(p.name for p in (folder / "shards" / run).glob("shard-*.log.gz"))
expected = [f"shard-{i:04d}.log.gz" for i in range(count)]
exits = re.findall(r"^exit code: (-?\d+)$", (folder / f"{run}.run.log").read_text(encoding="utf-8"), re.M)
merged = re.findall(r"^broiler-js-conformance: merged (\d+) test262 shard report", (folder / f"{run}.merge.log").read_text(encoding="utf-8"), re.M)
blog = (folder / f"{run}.binaries.log").read_text(encoding="utf-8")
lists = [l.split()[0] for l in blog.splitlines() if l and not l.startswith("#") and not l.startswith("runners:")]
tail = [l for l in blog.splitlines() if l.startswith("runners:")]
print(f"{run}: partition shardCount {count}; shard logs {len(shards)}, every one 0..{count - 1} present: {shards == expected}; "
      f"merge.log merged {merged}")
print(f"{run}: run.log exit code {exits}")
print(f"{run}: coverage {json.dumps(report['coverage'])}; retention {json.dumps(report['retention'])}; findings {len(report['findings'])}")
print(f"{run}: binaries.log lists {len(lists)}, distinct {len(set(lists))} {sorted(set(lists))}; {tail}")
