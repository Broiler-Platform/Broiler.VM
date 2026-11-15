"""Part A of bundle sjl-13-002 (task 7.3): completeness of the four hosted runs as bundle sjl-13-001's part A states it,
and the table of every changed verdict of the two retained comparisons with its class and its re-runs, from files the
bundle retains. Reads files and git; runs nothing. usage: part-a-tables.py <bundle> <repo> <out-dir>"""
import json, re, subprocess, sys, pathlib
B, REPO, OUT = pathlib.Path(sys.argv[1]), sys.argv[2], pathlib.Path(sys.argv[3])
BASE, AFTER = '389b55c9ec9bb77409bb2487067fbebc0868849b', '467ad7c395234b335c816b8f49ae5b505740e32c'
def git(*a): return subprocess.run(['git', '-C', REPO] + list(a), capture_output=True, text=True, check=True).stdout.strip()
lines = ["# part A, task 7.3: completeness of each hosted run as bundle sjl-13-001's part A states it (bundle sjl-6-001's reading), read from the imported artifacts"]
for path in ('.github/workflows/test262-evidence.yml', 'eng/test262-evidence.py'):
    lines.append('%s: blob %s at the base commit, %s at the after commit (both dispatches ran from the default branch at the after commit)' % (path, git('rev-parse', BASE + ':' + path), git('rev-parse', AFTER + ':' + path)))
sdk = {}
for side in ('base', 'after'):
    h = B / side / 'hosted'
    imp = (h / 'imported.txt').read_text().splitlines()
    lines.append('## %s (%s)' % (side, [l for l in imp if l.startswith('workflow run:')][0]))
    lines.append('tested: %s' % [l for l in (h / 'run.txt').read_text().splitlines() if l.startswith('tested commit:')][0])
    for form in ('wide-bytecode', 'wide-native'):
        shards = sorted((h / 'shards' / form).glob('shard-*.log.gz'))
        merge = (h / (form + '.merge.log')).read_text()
        m = re.search(r'merged (\d+) test262 shard report', merge)
        run = (h / (form + '.run.log')).read_text()
        ec = re.findall(r'^exit code: (\d+)', run, re.M)
        j = json.load(open(h / (form + '.test262.json')))
        binl = (h / (form + '.binaries.log')).read_text()
        dl = re.search(r'distinct lists: (\d+)', binl)
        sdks = sorted(set(re.findall(r'^sdk (\S+)', (h / (form + '.runners.txt')).read_text(), re.M)))
        sdk[(side, form)] = sdks
        lines.append('%s: shard logs %d of %d; merge took %s shard reports; run log exit code %s; retention retainable=%s; coverage whole=%s; every variant accounted=%s; distinct assembly lists %s; runners\' SDK %s' % (
            form, len(shards), j['partition']['shardCount'], m.group(1) if m else '?', ec[-1] if ec else '?', j['retention']['retainable'], j['coverage']['whole'], j['totals']['accountsForEveryVariant'], dl.group(1) if dl else '?', ', '.join(sdks)))
    lines.append('%s' % [l for l in (h / 'run.txt').read_text().splitlines() if l.startswith('merging job:')][0])
cmp = {}
for form in ('wide-bytecode', 'wide-native'):
    t = (B / 'after/parta-comparisons' / ('compare-%s.log' % form)).read_text()
    cmp[form] = t
    m = re.search(r'(\d+) variant\(s\) present in only one report were ignored', t)
    ex = re.search(r'^exit=(\d+)', t, re.M)
    lines.append('the comparison %s (after/parta-comparisons/compare-%s.log): variants present in only one report: %s; exit %s; "%s"' % (
        form, form, m.group(1), ex.group(1) if ex else '?', 'every difference is in an admitted class' if '# every difference is in an admitted class' in t else 'NOT every difference admitted'))
(OUT / 'completeness.txt').write_text('\n'.join(lines) + '\n')
rows = []
for form in ('wide-bytecode', 'wide-native'):
    cls = None
    for l in cmp[form].splitlines():
        if l.startswith('## '): cls = l[3:].split(' ')[0]; continue
        m = re.match(r"  (test/\S+) \[(\w+)\]: (\S+) -> (\S+)", l)
        if m: rows.append((form, cls) + m.groups())
agree = {}
for form in ('wide-bytecode', 'wide-native'):
    for l in open(B / 'after/parta-rerun' / ('agreement-%s.tsv' % form)):
        c = l.rstrip('\n').split('\t')
        if c[0] == 'path': continue
        agree[(form, c[0], c[1])] = (c[2], c[3])
differing = {form: (B / 'after/parta-rerun' / ('differing-%s.tsv' % form)).read_text() for form in ('wide-bytecode', 'wide-native')}
with open(OUT / 'changed-verdicts.tsv', 'w', newline='\n') as o:
    o.write("# part A, task 7.3: every variant whose verdict or exhausted dimension differs between the base's and the after commit's hosted runs, by form, with its class; each (c') row with its six one-process re-runs, three at each commit (after/parta-rerun/agreement-*.tsv)\n")
    o.write("form\tpath\tvariant\tbase (hosted)\tafter (hosted)\tcomparison's class\tclass\tre-runs (side, run, verdict/dimension)\treading\n")
    for form, cls, path, var, b, a in rows:
        rr = agree.get((form, path, var))
        if cls == "(c')" and rr and rr[1] == 'yes' and len(set(x.split('=')[1] for x in rr[0].split())) == 1 and len(rr[0].split()) == 6:
            kind, reading = "C4 (c')", 're-runs agree (six, one verdict and dimension, none a wall-clock exhaustion)' if not rr[0].endswith('WallClock') else 'RE-RUNS EXHAUSTED ON THE WALL CLOCK'
        elif cls == "(c')":
            kind, reading = 'REGRESSION unless (e)', 're-runs missing or differ'
        else:
            kind, reading = 'REGRESSION', 'not a class part A admits'
        o.write('\t'.join([form, path, var, b, a, cls, kind, rr[0] if rr else '-', reading]) + '\n')
    o.write("# (e): differing-wide-bytecode.tsv is %s and differing-wide-native.tsv is %s, so no row is raced (after/parta-race/note.txt)\n" % (
        'empty' if not differing['wide-bytecode'].strip() else 'NOT EMPTY', 'empty' if not differing['wide-native'].strip() else 'NOT EMPTY'))
print(open(OUT / 'completeness.txt').read()); print(open(OUT / 'changed-verdicts.tsv').read())
