#!/usr/bin/env python3
"""Groups Broiler.JS's differing lines of the three Temporal Intl datasets (runs/d-<name>.bjs.compare.txt) by a
coarse kind, each kind named with the row of rows.md it falls in. Writes runs/d-temporal.bjs.kinds.txt.
Usage: classify_temporal.py <rows-dir>"""
import json, re, sys, collections
from pathlib import Path
import gzip
def rd(f):
    return f.read_text(encoding='utf-8') if f.exists() else gzip.open(str(f) + '.gz', 'rt', encoding='utf-8').read()
rows = Path(sys.argv[1]).resolve()
def load(name):
    for l in rd(rows / 'runs' / ('d-%s.bjs.compare.txt' % name)).splitlines()[2:]:
        n, src, exp, ans = l.split('\t', 3)
        yield int(n), src, exp[len('expected: '):], ans[len('answered: '):]
def norm(s): return s.replace('\\u202f', ' ').replace(' ', ' ').replace(' – ', ' – ').replace('\\u2009\\u2013\\u2009', ' \\u2013 ')
out = ['# work/classify_temporal.py over runs/d-{temporal-intl,calendar-formats,zone-names}.bjs.compare.txt (Broiler.JS at f918b040)']
c = collections.Counter()
for n, src, exp, ans in load('temporal-intl'):
    lab, e = exp.split(' => ', 1); a = ans.split(' => ', 1)[1]
    loc = lab.split(' ')[0]
    if e.startswith('throws') or a.startswith('throws'): k = 'an error, or its kind (T39: the calendar check before the format)'
    elif ' range' in lab: k = 'a range (T29-T31)'
    elif loc.startswith('de'): k = 'de (T14, T15: names and layouts)'
    elif norm(e) == norm(a): k = 'en, U+202F alone (T12)'
    elif norm(e).replace(' at ', ', ') == norm(a): k = 'en, U+202F and the at-join (T12)'
    else: k = 'en, other (T13: pattern selection; T35, T36: calendars)'
    c[k] += 1
for k, v in sorted(c.items()): out.append('temporal-intl\t%s\t%d' % (k, v))
c = collections.Counter()
for n, src, exp, ans in load('calendar-formats'):
    lab = exp.split(' => ', 1)[0]
    m = re.search(r'"calendar":"([^"]+)"', lab)
    cal = m.group(1) if m else '?'
    c['%s %s' % (lab.split(' ')[0], cal)] += 1
for k, v in sorted(c.items()): out.append('calendar-formats\t%s (T33-T36)\t%d' % (k, v))
c = collections.Counter()
for n, src, exp, ans in load('zone-names'):
    lab, e = exp.split(' => ', 1); a = ans.split(' => ', 1)[1]
    loc = lab.split(' ')[0]
    es, as_ = e.split(' | '), a.split(' | ')
    cols = ['short', 'long', 'shortGeneric', 'longGeneric']
    diff = [cols[i] for i in range(min(len(es), len(as_), 4)) if es[i] != as_[i]]
    c['%s, differing in %s' % (loc, '+'.join(diff) or 'none')] += 1
for k, v in sorted(c.items()): out.append('zone-names\t%s (T22, T23)\t%d' % (k, v))
(rows / 'runs' / 'd-temporal.bjs.kinds.txt').write_text('\n'.join(out) + '\n', encoding='utf-8')
print('\n'.join(out))
