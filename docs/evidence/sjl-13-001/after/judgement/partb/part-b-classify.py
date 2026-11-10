"""Part B of bundle sjl-13-001 (task 7.3): every difference between the base, the registry commit and the after
commit's parse-corpus reports, each given a class of the area rule as revised (R, R2, S1 and S3 to S8, F, L, L2, N),
or none, which is a regression. It reads retained files and runs nothing.

Inputs (all retained in the bundle, paths relative to it):
  base/local/parse-corpus-base-run1.tsv.gz, registry/local/parse-corpus-registry-run1.tsv.gz,
  after/local/parse-corpus-after-run1.tsv.gz      part B's three run 1 reports (task 7.2)
  after/judgement/partb/stage-tags-registry.tsv.gz, stage-tags-after.tsv.gz
                                                  for every key whose report row differs between two of the three
                                                  runs, the row as the stage-tagged runs of task 7.3 reported it: each
                                                  diagnostic code:line:column@stage#message (base64); the stripped
                                                  rows equal run 1's byte for byte (stage-tags-compared.txt)
  after/judgement/partb/binder-sites.tsv          the registry commit's binder messages, each with its correspondence
                                                  row (B01 to B34) and the stage table's stage
  corpus/stages.tsv                               the stage table copy the mapping names
and, from git, the mapping at the registry commit (equal to the after commit's) and the parse corpus's manifest and
pinned archive at the base commit (to read an input's text where a class turns on it).

usage: part-b-classify.py <bundle> <repo> <out-dir>"""
import base64, collections, gzip, io, re, subprocess, sys, tarfile, pathlib

BUNDLE, REPO, OUT = pathlib.Path(sys.argv[1]), sys.argv[2], pathlib.Path(sys.argv[3])
BASE, REGISTRY, AFTER = 'df75f8cea79aa2452ca22892d47d8afa5a14c93f', 'cbee7c46ece7c9d7cf8a0c3f9a5a68418266672d', 'a8d8398bd59e7f5600cb0aa9e733ff8948859cf4'
OUT.mkdir(parents=True, exist_ok=True)

def git(*a, binary=False):
    r = subprocess.run(['git', '-C', REPO] + list(a), capture_output=True, check=True)
    return r.stdout if binary else r.stdout.decode('utf-8')

# ---- the mapping, at the registry commit (it does not change before the after commit; checked) ----
MAP = 'src/Broiler.VM.Profile.JavaScript/docs/diagnostics/frontend-mapping.tsv'
mapping_text = git('show', REGISTRY + ':' + MAP)
assert mapping_text == git('show', AFTER + ':' + MAP), 'the mapping moved between the registry and the after commit'
code_rows = {}            # shared code -> dict
R_PAIRS, R2_PAIRS = set(), set()
for line in mapping_text.splitlines():
    if not line.startswith('code\t'): continue
    c = line.split('\t')
    other = []
    for part in c[9].split(' | '):
        m = re.match(r'(\d{4}) (split|other-row)', part)
        if m:
            kind = 'other-row' if m.group(2) == 'other-row' else ('R2' if 'until the after commit' in part else 'R')
            other.append((m.group(1), kind))
            if kind == 'R': R_PAIRS.add((m.group(1), c[6]))
            if kind == 'R2': R2_PAIRS.add((m.group(1), c[6]))
    code_rows[c[1]] = {'row': c[2], 'registry_code': c[6], 'other': other}
stage_of = {}
for line in (BUNDLE / 'corpus/stages.tsv').read_text(encoding='utf-8').splitlines()[1:]:
    c = line.split('\t'); stage_of[c[0]] = c[2]

# ---- the binder's sites at the registry commit ----
BINDER = []
for line in (BUNDLE / 'after/judgement/partb/binder-sites.tsv').read_text(encoding='utf-8').splitlines():
    if line.startswith('#') or line.startswith('code\t'): continue
    code, pattern, brow, rows, stage = line.split('\t')[:5]
    BINDER.append((code, re.compile(pattern), brow, rows, stage))

def binder_site(code, message, text_at):
    hits = [b for b in BINDER if b[0] == code and b[1].fullmatch(message)]
    if len(hits) > 1:   # "the left-hand side of an assignment is not a reference": B18/B20 (syntax) or B21 (semantics)
        if text_at is not None and re.search(r'\bfor\s*(await\s*)?\(\s*((var|let|const)\s+)?$', text_at):
            hits = [b for b in hits if b[2] == 'B21']
        else:
            hits = [b for b in hits if b[2] != 'B21']
    if len(hits) != 1:
        raise SystemExit('binder message %r (code %s) matches %d sites' % (message, code, len(hits)))
    return hits[0]

# ---- the reports ----
def report(path, tagged=False):
    rows = {}
    for l in gzip.open(path, 'rt', encoding='utf-8'):
        if l.startswith('#') or l.startswith('id\t'): continue
        c = l.rstrip('\n').split('\t')
        rows[tuple(c[:4])] = c[4:]
    return rows
base = report(BUNDLE / 'base/local/parse-corpus-base-run1.tsv.gz')
registry = report(BUNDLE / 'registry/local/parse-corpus-registry-run1.tsv.gz')
after = report(BUNDLE / 'after/local/parse-corpus-after-run1.tsv.gz')
assert set(base) == set(registry) == set(after)
tagged = {'registry': report(BUNDLE / 'after/judgement/partb/stage-tags-registry.tsv.gz'),
          'after': report(BUNDLE / 'after/judgement/partb/stage-tags-after.tsv.gz')}
TAG = re.compile(r'@[^;#\t]*#[A-Za-z0-9+/=]*')
for side, plain in (('registry', registry), ('after', after)):
    for k, v in tagged[side].items():
        assert TAG.sub('', '\t'.join(v)) == '\t'.join(plain[k]), (side, k)

# ---- input texts, for the classes that turn on them ----
manifest = {}
for line in git('show', BASE + ':src/tests/corpus/parse-1/corpus.manifest').splitlines():
    if line.startswith('#') or line.startswith('id\t'): continue
    c = line.split('\t'); manifest[c[0]] = c
_archive = None
def text_of(id_):
    global _archive
    m = manifest[id_]
    if m[1] == 'test262':
        if _archive is None:
            data = git('show', BASE + ':src/tests/conformance/pins/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e.tar.gz', binary=True)
            _archive = tarfile.open(fileobj=io.BytesIO(data))
            _archive.names = {n.split('/', 1)[1] if '/' in n else n: n for n in _archive.getnames()}
        return _archive.extractfile(_archive.names[m[2]]).read().decode('utf-8')
    return git('show', BASE + ':src/tests/corpus/parse-1/' + m[2])

def before_on_line(id_, line, col):
    try:
        lines = text_of(id_).split('\n')
        return lines[line - 1][:col - 1] if 1 <= line <= len(lines) else None
    except Exception:
        return None

# ---- diagnostics ----
def parse_list(cell, tagged_cell=None):
    if cell == '-': return []
    out = []
    items = cell.split(';'); titems = tagged_cell.split(';') if tagged_cell else [None] * len(items)
    for i, (x, t) in enumerate(zip(items, titems)):
        code, line, col = x.split(':'); d = {'code': code, 'line': int(line), 'col': int(col), 'i': i, 'tag': None, 'msg': None}
        if t is not None:
            _, rest = t.split('@', 1); tag, msg = rest.split('#', 1)
            d['tag'] = tag; d['msg'] = base64.b64decode(msg).decode('utf-8')
        out.append(d)
    return out
def place(d): return (d['line'], d['col'])
def show(ds): return ';'.join('%s:%d:%d' % (d['code'], d['line'], d['col']) for d in ds) or '-'

POLICY = re.compile(r'BigInt|module specifier')
def stage_kind(id_, d):
    """lexical, parse, policy (a JsParser policy or limit refusal the admission walk makes from step 1), binder, lowering, numeric."""
    t = d['tag']
    if t == 'tokenizer': return 'lexical'
    if t == 'parser':
        return 'policy' if (d['code'] in ('2106', '2104') and POLICY.search(d['msg'] or '')) or (d['code'] == '2104' and 'not well-formed' in (d['msg'] or '')) else 'parse'
    if t in ('binder', 'lowering', 'numeric'): return t
    raise SystemExit('unknown stage tag %r' % t)

# ---- base -> registry: class R only ----
rows_out = []
tally = collections.Counter()
def emit(k, step, cls, kinds, glist, alist, extra):
    rows_out.append((k[0], k[1], k[2], k[3], step, cls, '+'.join(kinds) if kinds else '-', glist, alist, extra))
    tally[(step, cls, '+'.join(kinds) if kinds else '-', k[1])] += 1

r_keys = set()
for k in base:
    b, g = base[k], registry[k]
    if b == g: continue
    if k[1] == 'B4' and False: pass
    if b[0] == g[0] == 'refused':
        bl, gl = parse_list(b[1]), parse_list(g[1])
        ok = len(bl) == len(gl) and all(place(x) == place(y) and (x['code'] == y['code'] or (x['code'], y['code']) in R_PAIRS) for x, y in zip(bl, gl))
        pairs = sorted({(x['code'], y['code']) for x, y in zip(bl, gl) if x['code'] != y['code']}) if ok else []
        if ok:
            r_keys.add(k); emit(k, 'base>registry', 'R', [], b[1], g[1], ' '.join('%s>%s' % p for p in pairs)); continue
    emit(k, 'base>registry', 'REGRESSION', [], '|'.join(b[:4]), '|'.join(g[:4]), 'a difference between the base and the registry commit outside R')

# ---- registry -> after ----
P15B = re.compile(r'\{\s*\w+\s*:\s*\(\s*(\w+\s*=|\[)')
NEWNEW = re.compile(r'\bnew\s+new\b')
def b1_lists(id_, side):
    rep = registry if side == 'registry' else after
    return [rep[(id_, 'B1', v, f)] for v in ('module',) for f in ('bytecode', 'native') if (id_, 'B1', v, f) in rep]

for k in sorted(registry):
    g, a = registry[k], after[k]
    if g == a: continue
    id_, pop = k[0], k[1]
    if pop == 'B4':
        emit(k, 'registry>after', 'REGRESSION', [], '|'.join(g[:4]), '|'.join(a[:4]), 'B4 admits no class but R'); continue
    # outcome moves
    if g[0] != a[0]:
        if g[0] == 'accepted' and a[0] == 'refused':
            ad = parse_list(a[1], tagged['after'][k][1])
            if pop == 'B5':
                b1 = [registry[(id_, 'B1', 'module', f)] for f in ('bytecode', 'native')]
                b1a = [after[(id_, 'B1', 'module', f)] for f in ('bytecode', 'native')]
                if all(x[0] == 'refused' for x in b1) and all(x[1] == a[1] for x in b1a) and ad[0]['tag'].startswith('library/'):
                    emit(k, 'registry>after', 'F', ['D64'], 'accepted ' + g[4], a[1],
                         'B1 under the module goal refused at the registry commit (%s) and after with this one diagnostic; library %s' % (b1[0][1], ad[0]['tag'][8:])); continue
            elif P15B.search(text_of(id_)) and ad[0]['tag'].startswith('library/'):
                emit(k, 'registry>after', 'F', ['P15b'], 'accepted', a[1], 'a parenthesised nested target in an object pattern; library %s' % ad[0]['tag'][8:]); continue
            if ad and ad[0]['code'] == '2103':
                emit(k, 'registry>after', 'L?', [], 'accepted', a[1], 'needs the measure'); continue
        if g[0] == 'refused' and a[0] == 'accepted':
            emit(k, 'registry>after', 'L2?/F?', [], g[1], 'accepted', 'needs reading'); continue
        emit(k, 'registry>after', 'REGRESSION', [], '|'.join(g[:2]), '|'.join(a[:2]), 'an outcome that moves outside F, L and L2'); continue
    if g[0] == 'accepted':
        if pop == 'B5':
            emit(k, 'registry>after', 'REGRESSION', [], g[4], a[4], 'B5 specifiers differ'); continue
        places = ['%d:%d' % (t[:m.start()].count('\n') + 1, m.start() - t.rfind('\n', 0, m.start())) for t in [text_of(id_)] for m in NEWNEW.finditer(t)]
        if places:
            emit(k, 'registry>after', 'F', ['P37'], '%s %s' % (g[2], g[3]), '%s %s' % (a[2], a[3]), '`new new` at ' + ', '.join(places)); continue
        emit(k, 'registry>after', 'REGRESSION', [], g[2], a[2], 'a byte difference of an input accepted at both commits'); continue
    # refused at both
    gl = parse_list(g[1], tagged['registry'][k][1]); al = parse_list(a[1], tagged['after'][k][1])
    if len(al) != 1:
        emit(k, 'registry>after', 'REGRESSION', [], g[1], a[1], 'the after list holds %d diagnostics' % len(al)); continue
    d = al[0]
    if not d['tag'].startswith('library/'):
        emit(k, 'registry>after', 'REGRESSION', [], g[1], a[1], 'the one diagnostic after is the %s\'s, not the library\'s' % d['tag']); continue
    _, shared, msgid, phase = d['tag'].split('/')
    drow = code_rows[shared]
    if d['code'] != drow['registry_code']:
        emit(k, 'registry>after', 'REGRESSION', [], g[1], a[1], 'code %s is not the registry_code %s of %s' % (d['code'], drow['registry_code'], shared)); continue
    # Between the registry and the after commit only R2's pairs are read: every renumbering class R admits is made at
    # the registry commit (a single-rule site's code is changed there), so a pair the mapping marks R is never one the
    # library's diagnostic and the registry commit's can still differ by. An R2 pair is the one the library's row (the
    # row of its shared code) names as a multi-rule site's code "until the after commit".
    paired = lambda c: c == d['code'] or any(o == c and kind == 'R2' for o, kind in drow['other'])
    pairkind = lambda c: '' if c == d['code'] else next(kind for o, kind in drow['other'] if o == c and kind == 'R2')
    rpair = lambda c: any(o == c and kind == 'R' for o, kind in drow['other'])
    info = []
    for x in gl:
        x['kind'] = stage_kind(id_, x)
        if x['kind'] == 'binder':
            site = binder_site(x['code'], x['msg'], before_on_line(id_, x['line'], x['col']))
            x['brow'], x['stage'], x['rows'] = site[2], site[4], site[3]
    def s6(x): return x['kind'] == 'lexical' and place(x) > place(d)
    def s3able(x): return x['kind'] in ('binder', 'lowering', 'numeric', 'policy')
    cands = [x for x in gl if not s6(x) and (not s3able(x) or (x['kind'] == 'binder' and x['stage'] == 'syntax'))]
    r = min(cands, key=lambda x: (x['line'], x['col'], x['i'])) if cands else None
    widened = False
    if r is None:
        at = [x for x in gl if x['kind'] == 'binder' and place(x) == place(d)]
        if at: r = at[0]; widened = True
    lib = '%s %s (%s) row %s' % (shared, msgid, phase, drow['row'])
    def removal(exclude):
        kinds, bad = set(), []
        for x in gl:
            if x is exclude: continue
            if x['kind'] == 'lexical':
                if s6(x): kinds.add('S6')
                else: bad.append(x)
            elif s3able(x): kinds.add('S3')
            elif x['kind'] == 'parse':
                if exclude is not None and place(x) >= place(exclude): kinds.add('S7')
                elif exclude is None and place(x) > place(d): kinds.add('S7')
                else: bad.append(x)
        return kinds, bad
    if r is not None and place(r) == place(d) and paired(r['code']):
        kinds, bad = removal(r)
        if r['kind'] == 'binder' and r['brow'] == 'B25': kinds.add('S5')
        if pairkind(r['code']) == 'R2': kinds.add('R2')
        if bad: emit(k, 'registry>after', 'REGRESSION', sorted(kinds), g[1], a[1], 'unexplained removal of ' + show(bad)); continue
        emit(k, 'registry>after', 'S', sorted(kinds), g[1], a[1], 'r %s:%d:%d %s%s; %s' % (r['code'], r['line'], r['col'], r['kind'], (' ' + r['brow'] + ' ' + r['stage']) if r['kind'] == 'binder' else '', lib)); continue
    # S8: two declared-name conflicts of one scope
    DECL = {'2201', '2202'}
    if gl[0]['code'] in DECL and d['code'] in DECL and any(place(x) == place(d) and paired(x['code']) for x in gl):
        x = next(x for x in gl if place(x) == place(d) and paired(x['code']))
        kinds, bad = removal(x); kinds |= {'S8', 'S7'}
        if pairkind(x['code']) == 'R2': kinds.add('R2')
        kinds.discard('S7') if not any(y['kind'] == 'parse' and y is not x for y in gl) else None
        if not bad:
            emit(k, 'registry>after', 'S', sorted(kinds), g[1], a[1], 'registry first %s:%d:%d; after the other conflict, at %d:%d; %s' % (gl[0]['code'], gl[0]['line'], gl[0]['col'], d['line'], d['col'], lib)); continue
    if r is not None:
        same_place = place(r) == place(d)
        kind = 'code' if same_place else ('place' if paired(r['code']) else 'place and code')
        if widened and kind != 'code':
            emit(k, 'registry>after', 'REGRESSION', [], g[1], a[1], 'widened r with a place'); continue
        kinds, bad = removal(r)
        if bad:
            emit(k, 'registry>after', 'REGRESSION', sorted(kinds), g[1], a[1], 'N: unexplained removal of ' + show(bad)); continue
        kinds = {kk for kk in kinds if kk != 'S7'}
        rdesc = '%s:%d:%d %s' % (r['code'], r['line'], r['col'], 'parse' if r['kind'] == 'parse' else r['kind'])
        if r['kind'] == 'binder': rdesc += ' %s (%s), stage table %s' % (r['brow'], r['rows'], r['stage'])
        if base[k][0] != 'refused':
            emit(k, 'registry>after', 'REGRESSION', [], g[1], a[1], 'N needs an input refused at the base'); continue
        note = '; (%s,%s) is an R pair of row %s, for another site, not read here' % (r['code'], d['code'], drow['row']) if kind == 'code' and rpair(r['code']) else ''
        emit(k, 'registry>after', 'N', ['N(%s)%s' % (kind, ' widened' if widened else '')] + sorted(kinds), g[1], a[1], 'r %s; %s; mapping code row %s -> %s%s' % (rdesc, lib, shared, drow['registry_code'], note)); continue
    # no r: S4 (unmasked), with S6 or S3 removing every diagnostic the registry commit reported
    kinds, bad = removal(None)
    if not bad and stage_of.get(drow['row']) in ('syntax', 'request') and kinds <= {'S6', 'S3'} and 'S6' in kinds:
        emit(k, 'registry>after', 'S', sorted(kinds | {'S4'}), g[1], a[1], 'the registry commit\'s tokenizer stopped the unit; %s, stage %s' % (lib, stage_of.get(drow['row']))); continue
    emit(k, 'registry>after', 'REGRESSION', sorted(kinds), g[1], a[1], 'no class: no r, %s' % lib)

# ---- the census, base against after, byte for byte ----
census = []
for name in ('census-inputs.txt.gz', 'census-test.txt.gz'):
    same = gzip.open(BUNDLE / 'base/local' / name).read() == gzip.open(BUNDLE / 'after/local' / name).read()
    census.append('%s: %s' % (name, 'identical' if same else 'DIFFERS'))
    if not same: tally[('census', 'REGRESSION', '-', '-')] += 1

# ---- the control: the base report with three planted changes, read with the same R test ----
control = report(BUNDLE / 'controls/partb/part-b-control.tsv.gz')
control_rows = []
for k in base:
    b, c = base[k], control[k]
    if b == c: continue
    admitted = b[0] == c[0] == 'refused' and len(parse_list(b[1])) == len(parse_list(c[1])) and all(
        place(x) == place(y) and (x['code'] == y['code'] or (x['code'], y['code']) in R_PAIRS) for x, y in zip(parse_list(b[1]), parse_list(c[1])))
    control_rows.append('%s\t%s\t%s\t%s\t%s' % ('\t'.join(k), '|'.join(b[:3]), '|'.join(c[:3]), 'R' if admitted else 'unclassified', ''))

# ---- reading 3: R's renumbering is not undone at the after commit ----
undone = []
for k in r_keys:
    bl, gl, al = parse_list(base[k][1]), parse_list(registry[k][1]), parse_list(after[k][1]) if after[k][0] == 'refused' else []
    for x, y in zip(bl, gl):
        if x['code'] != y['code'] and any(place(z) == place(x) and z['code'] == x['code'] for z in al):
            undone.append(k)
with open(OUT / 'part-b-classes.tsv', 'w', encoding='utf-8', newline='\n') as o:
    o.write('# part B of bundle sjl-13-001, task 7.3: every difference, by part-b-classify.py; class REGRESSION is a difference no class admits\n')
    o.write('id\tpopulation\tvariant\tform\tstep\tclass\tkinds\tbefore\tafter\treading\n')
    for r in rows_out: o.write('\t'.join(r) + '\n')
with open(OUT / 'part-b-summary.txt', 'w', encoding='utf-8', newline='\n') as o:
    o.write('# part B of bundle sjl-13-001, task 7.3: counts by step, class, kinds and sub-population (part-b-classes.tsv)\n')
    for key in sorted(tally): o.write('%s\t%s\t%s\t%s\t%d\n' % (key + (tally[key],)))
    o.write('# keys renumbered between the base and the registry commit (class R): %d; of them, whose renumbered code the after commit reports again at the base\'s place (reading 3: undone): %d\n' % (len(r_keys), len(set(undone))))
    o.write('# census, base against after: %s\n' % '; '.join(census))
    o.write('# the control (controls/partb/part-b-control.tsv.gz against the base report): %d differences, %d unclassified\n' % (len(control_rows), sum(1 for r in control_rows if '\tunclassified\t' in r)))
    for r in control_rows: o.write('#   ' + r + '\n')
    o.write('# regressions: %d\n' % sum(v for kk, v in tally.items() if kk[1] == 'REGRESSION'))
print(open(OUT / 'part-b-summary.txt').read())
