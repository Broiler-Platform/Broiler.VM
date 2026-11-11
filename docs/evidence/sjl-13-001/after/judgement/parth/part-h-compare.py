"""Part H of bundle sjl-13-001 (task 7.3): every vector of the syntax area, its verdict at the base against its
verdict at the after commit, read from the vectors runs task 7.2 retained, with the correspondence row each changed
vector's divergence.broiler-vm field names. Reads files; runs nothing.
usage: part-h-compare.py <bundle> <extracted vectors pin> <out.tsv>"""
import gzip, sys, re, collections, pathlib
bundle, pin, out = pathlib.Path(sys.argv[1]), pathlib.Path(sys.argv[2]), sys.argv[3]
def load(p):
    d = {}
    for l in gzip.open(p, 'rt', encoding='utf-8'):
        if l.startswith('#') or l.startswith('id\t'): continue
        c = l.rstrip('\n').split('\t'); d[c[0]] = c
    return d
base = load(bundle / 'base/local/vectors-run.tsv.gz'); after = load(bundle / 'after/local/vectors-run.tsv.gz')
fields = {}
for f in sorted((pin / 'conformance/vectors/syntax').glob('*.vectors')):
    cur = None
    for line in f.read_text(encoding='utf-8').splitlines():
        if line.startswith('id: '): cur = line[4:].strip()
        elif line.startswith('divergence.broiler-vm:') and cur: fields[cur] = line.split(':', 1)[1].strip()
syntax = sorted(k for k in base if k.startswith('syntax/'))
assert set(base) == set(after)
rows, tally = [], collections.Counter()
MATCH = ('match', 'match (divergence no longer holds)')
for k in syntax:
    b, a = base[k][4], after[k][4]
    tally[('after', a)] += 1
    if b == a: continue
    field = fields.get(k, '')
    m = re.match(r'rows? ([A-Z]\d+[a-z]?)', field)
    row = m.group(1) if m else '-'
    cls = 'C1' if b in ('difference', 'classified-divergence') and a in MATCH and row != '-' else 'regression'
    tally[cls] += 1
    rows.append((k, b, a, row, cls, field[:160]))
with open(out, 'w', encoding='utf-8', newline='\n') as o:
    o.write('# part H, task 7.3: every syntax-area vector whose verdict differs between the base and the after commit\n')
    o.write('# vectors of the syntax area: %d; changed: %d; C1: %d; regressions: %d\n' % (len(syntax), len(rows), tally['C1'], tally['regression']))
    o.write('# after verdicts over the area: %s\n' % ', '.join('%s %d' % (v, n) for (t, v), n in sorted(tally.items(), key=str) if t == 'after'))
    o.write('id\tbase\tafter\trow\tclass\tdivergence.broiler-vm (first 160 characters)\n')
    for r in rows: o.write('\t'.join(r) + '\n')
other = sorted(k for k in base if not k.startswith('syntax/') and base[k][4] != after[k][4])
with open(out, 'a', encoding='utf-8', newline='\n') as o:
    o.write('# outside the population (not the syntax area), changed, recorded and not judged: %s\n' % ', '.join('%s %s -> %s' % (k, base[k][4], after[k][4]) for k in other))
print(open(out).read()[:600])
