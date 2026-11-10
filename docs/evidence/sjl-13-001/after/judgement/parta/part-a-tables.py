"""Part A of bundle sjl-13-001 (task 7.3): completeness of the four hosted runs, and the tables of every changed
verdict of the two comparisons, from files the bundle retains. Reads files and git; runs nothing.
usage: part-a-tables.py <bundle> <repo> <out-dir>"""
import gzip, json, re, subprocess, sys, pathlib, collections
B, REPO, OUT = pathlib.Path(sys.argv[1]), sys.argv[2], pathlib.Path(sys.argv[3])
BASE, AFTER = 'df75f8cea79aa2452ca22892d47d8afa5a14c93f', 'a8d8398bd59e7f5600cb0aa9e733ff8948859cf4'
def git(*a): return subprocess.run(['git', '-C', REPO] + list(a), capture_output=True, text=True, check=True).stdout.strip()
lines = ['# part A, task 7.3: completeness of each hosted run as the rule states it (bundle sjl-6-001\'s reading), read from the imported artifacts']
for path in ('.github/workflows/test262-evidence.yml', 'eng/test262-evidence.py'):
    lines.append('%s: blob %s at the base commit, %s at the after commit (both dispatches ran from the default branch at the after commit)' % (path, git('rev-parse', BASE + ':' + path), git('rev-parse', AFTER + ':' + path)))
for side in ('base', 'after'):
    h = B / side / 'hosted'
    lines.append('## %s (%s)' % (side, [l for l in (h / 'imported.txt').read_text().splitlines() if l.startswith('workflow run:')][0]))
    for form in ('wide-bytecode', 'wide-native'):
        shards = sorted((h / 'shards' / form).glob('shard-*.log.gz'))
        merge = (h / (form + '.merge.log')).read_text()
        m = re.search(r'merged (\d+) test262 shard report', merge)
        run = (h / (form + '.run.log')).read_text()
        ec = re.findall(r'^exit code: (\d+)', run, re.M)
        j = json.load(open(h / (form + '.test262.json')))
        binl = (h / (form + '.binaries.log')).read_text()
        dl = re.search(r'distinct lists: (\d+)', binl)
        lines.append('%s: shard logs %d of %d; merge took %s shard reports; run log exit code %s; retention retainable=%s; coverage whole=%s; every variant accounted=%s; distinct assembly lists %s' % (
            form, len(shards), j['partition']['shardCount'], m.group(1) if m else '?', ec[-1] if ec else '?', j['retention']['retainable'], j['coverage']['whole'], j['totals']['accountsForEveryVariant'], dl.group(1) if dl else '?'))
cmp = {}
for form in ('wide-bytecode', 'wide-native'):
    t = (OUT / ('compare-%s.log' % form)).read_text()
    cmp[form] = t
    m = re.search(r'(\d+) variant\(s\) present in only one report were ignored', t)
    lines.append('the comparison %s: variants present in only one report: %s' % (form, m.group(1)))
(OUT / 'completeness.txt').write_text('\n'.join(lines) + '\n')
# the changed verdicts, by class
rows = []
for form in ('wide-bytecode', 'wide-native'):
    cls = None
    for l in cmp[form].splitlines():
        h = re.match(r"## \((.+?)\)", l)
        if h: cls = h.group(1); continue
        m = re.match(r"  (test/\S+) \[(\w+)\]: (\S+) -> (\S+)", l)
        if m: rows.append((form, cls) + m.groups())
KINDF = {'test/staging/sm/RegExp/7.8.5-01.js': 'T33', 'test/staging/sm/statements/arrow-function-in-for-statement-head.js': 'P38',
         'test/staging/sm/class/compPropNames.js': 'P39', 'test/staging/sm/async-functions/await-in-arrow-parameters.js': 'T24f',
         'test/staging/sm/expressions/destructuring-pattern-parenthesized.js': 'P15b'}
# kind (F)'s first diagnostics, from the gated one-process re-runs (kindF-gated/)
first = {}
for f in sorted((OUT / 'kindF-gated' / 'reports').glob('*.report*')):
    op = gzip.open if f.suffix == '.gz' else open
    lab = re.match(r'kindF-(wide-\w+)-(base|after)-', f.name)
    for l in op(f, 'rt'):
        if l.startswith('result|'):
            c = l.rstrip('\n').split('|')
            first[(lab.group(1), lab.group(2), c[1], c[2])] = (c[3], c[8] if len(c) > 8 and c[8] else '-')
# (c') re-runs and the race, task 7.2's pinned runs
rer = collections.defaultdict(list)
for form in ('wide-bytecode', 'wide-native'):
    for l in open(B / 'after/parta-rerun-pinned' / ('rerun-%s.tsv' % form)):
        side, f2, rep, res = l.rstrip('\n').split('\t')
        c = res.split('|'); rer[(form, c[1], c[2], side)].append('%s/%s' % (c[3], c[5]) if c[5] else c[3])
race = {}
for l in open(B / 'after/parta-race-pinned/race-wide-native.tsv'):
    side, f2, res = l.rstrip('\n').split('\t'); c = res.split('|'); race[(c[1], c[2], side)] = '%s/%s' % (c[3], c[5]) if c[5] else c[3]
with open(OUT / 'changed-verdicts.tsv', 'w', newline='\n') as o:
    o.write('# part A, task 7.3: every variant whose verdict or exhausted dimension differs, by form, with its class; kind (F) rows with the first diagnostic each gated one-process re-run reports (kindF-gated/), (c\') rows with task 7.2\'s pinned re-runs (after/parta-rerun-pinned/) and race (after/parta-race-pinned/)\n')
    o.write('form\tpath\tvariant\tbase (hosted)\tafter (hosted)\tcomparison\'s class\tclass and kind\trow\tre-run at the base\tre-run at the after commit\tfirst diagnostic at the base\tfirst diagnostic after\treading\n')
    for form, cls, path, var, b, a in rows:
        if cls.startswith('unclassified'):
            row = KINDF.get(path, '-')
            fb = first.get((form, 'base', path, var)); fa = first.get((form, 'after', path, var))
            ok = row != '-' and b in ('Failed',) and a == 'Passed' and fb and fa and fb[0] == 'Failed' and fa[0] == 'Passed'
            o.write('\t'.join([form, path, var, b, a, 'unclassified', 'C1 (F)' if ok else 'REGRESSION', row, fb[0] if fb else '?', fa[0] if fa else '?', fb[1] if fb else '?', fa[1] if fa else '?', 'fails at the base and passes after, by the row\'s correction' if ok else 'no kind']) + '\n')
        else:
            rb = rer.get((form, path, var, 'base'), []); ra = rer.get((form, path, var, 'after'), [])
            agree = len(set(rb + ra)) == 1
            if agree: kind, reading = "C4 (c')", 're-runs agree'
            else:
                rc = (race.get((path, var, 'base')), race.get((path, var, 'after')))
                okr = rc[0] is not None and rc[0] == rc[1] and not rc[0].endswith('WallClock') and 'Defect' not in rc[0]
                kind = "C4 (e)" if okr else 'REGRESSION'
                reading = 're-runs differ; race at ten times the wall clock: base %s, after %s' % rc
            o.write('\t'.join([form, path, var, b, a, "(c')", kind, '-', ', '.join(rb), ', '.join(ra), '-', '-', reading]) + '\n')
print(open(OUT / 'completeness.txt').read()); print(open(OUT / 'changed-verdicts.tsv').read())
