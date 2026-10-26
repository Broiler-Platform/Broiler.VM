#!/usr/bin/env python3
"""Groups Broiler.JS's differing lines of the dates and timezones datasets (runs/d-<name>.bjs.compare.txt) by
kind, to reproduce or refute the JS survey's breakdown. Writes runs/d-<name>.bjs.kinds.txt.
Usage: classify_dates.py <rows-dir>"""
import re, sys, collections
from pathlib import Path
import gzip
def rd(f):
    return f.read_text(encoding='utf-8') if f.exists() else gzip.open(str(f) + '.gz', 'rt', encoding='utf-8').read()
rows = Path(sys.argv[1]).resolve()
def load(name):
    out = []
    for l in rd(rows / 'runs' / ('d-%s.bjs.compare.txt' % name)).splitlines()[2:]:
        n, src, exp, ans = l.split('\t', 3)
        out.append((int(n), src, exp[len('expected: '):], ans[len('answered: '):]))
    return out
def norm(s):
    return s.replace('\\u202f', ' ').replace('\\u2009\\u2013\\u2009', ' \\u2013 ')
res = []
kinds = collections.Counter()
for n, src, exp, ans in load('dates'):
    label = exp.split(' => ')[0]
    toks = label.split(' ')
    loc = toks[1] if len(toks) > 1 else ''
    if ' range ' in label: k = 'range'
    elif loc.startswith('de'): k = 'de (not a range)'
    elif norm(exp) == norm(ans): k = 'en: only U+202F (and thin-space range separator)'
    elif norm(exp).replace(' at ', ', ') == norm(ans).replace(' at ', ', '): k = 'en: U+202F and the " at " join'
    else: k = 'en: other'
    kinds[(k, src)] += 1
    res.append('%d\t%s\t%s' % (n, src, k))
u202f_any = sum(1 for n, src, exp, ans in load('dates') if norm(exp) == norm(ans))
out = ['# work/classify_dates.py over runs/d-dates.bjs.compare.txt (Broiler.JS at f918b040)']
for (k, src), c in sorted(kinds.items()):
    out.append('dates\t%s\t%s\t%d' % (k, src, c))
out.append('dates\tany line equal to its expectation once U+202F and the thin-spaced range dash are read as spaces\t-\t%d' % u202f_any)
de = sum(1 for n, src, exp, ans in load('dates') if (exp.split(' => ')[0].split(' ') + ['', ''])[1].startswith('de'))
out.append('dates\tany line of a de case (range or not)\t-\t%d' % de)
# timezones: the difference is only the resolved timeZone?
z = collections.Counter()
for n, src, exp, ans in load('timezones'):
    ez, er = exp.split(' => ', 1); az, ar = ans.split(' => ', 1)
    erz, erest = er.split(' | ', 1) if ' | ' in er else (er, ''); arz, arest = ar.split(' | ', 1) if ' | ' in ar else (ar, '')
    z['only the resolved name differs: the identifier as given (answered) against its primary (expected)' if ez == az and erest == arest and arz == az and erz != az else 'other'] += 1
for k, c in sorted(z.items()):
    out.append('timezones\t%s\t-\t%d' % (k, c))
(rows / 'runs' / 'd-dates-timezones.bjs.kinds.txt').write_text('\n'.join(out) + '\n', encoding='utf-8')
print('\n'.join(out))
# The survey's own measure: a line equal to its expectation once U+202F alone is read as a space.
only = 0
for n, src, exp, ans in load('dates'):
    if exp.replace('\\u202f', ' ') == ans:
        only += 1
line = 'dates\tany line equal to its expectation once U+202F alone is read as a space\t-\t%d' % only
with open(rows / 'runs' / 'd-dates-timezones.bjs.kinds.txt', 'a', encoding='utf-8') as f:
    f.write(line + '\n')
print(line)
