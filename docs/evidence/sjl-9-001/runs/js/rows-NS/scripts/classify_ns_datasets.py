#!/usr/bin/env python3
"""Classifies each differing line of Broiler.JS's dataset comparisons (runs/d-*.bjs.compare.txt) under the rows of
rows-NS/rows.md. Lines of d-numbers that sub-area b classified (rows-MP/runs/d-numbers.classes.tsv) keep sub-area b's
row and are not repeated here. Writes runs/d-numbers.classes-ns.tsv, runs/d-relativetimes.classes.tsv and
runs/d-displaynames-patched.classes.tsv, and prints the count of lines per row.

Usage: python3 -I classify_ns_datasets.py <rows-NS dir> <rows-MP dir>
"""
import collections
import gzip
import re
import sys
from pathlib import Path

rows = Path(sys.argv[1]).resolve()
mp = Path(sys.argv[2]).resolve()


def opened(path):
    if path.exists():
        return open(path, encoding='utf-8')
    return gzip.open(str(path) + '.gz', 'rt', encoding='utf-8')


def lines(name):
    with opened(rows / 'runs' / name) as f:
        for line in f:
            if line[:1].isdigit():
                n, src, exp, ans = line.rstrip('\n').split('\t')
                yield n, src, exp[len('expected: '):], ans[len('answered: '):]


LABEL = re.compile(r'(\d+) (\S+) (\{.*?\}) (.*?) => ("(?:[^"\\]|\\.)*"|.*)')


def number_row(e, a):
    me, ma = LABEL.match(e), LABEL.match(a)
    group, what, text = int(me.group(1)), me.group(4), me.group(5)
    answered = ma.group(5) if ma else a
    if what.startswith('range'):
        if '\\u2248' in text and '\\u2248' not in answered:
            return 'N9c'
        if group == 21:
            return 'M7 (sub-area b; also N9a or N9b)'
        if group in (26, 48):
            return 'N4 (also N9d)'
        if group in (31, 54):
            return 'N6c'
        if group == 25:
            return 'N3b (also N9a)'
        if group in (22, 46) and what == 'range 0.9999..1.0001':
            return 'M5 (sub-area b)'
        if group == 37:
            return 'N9d (also N9b)' if what == 'range -5..-1' else 'N9d'
        if group in (29, 30):
            return 'N9a'
        if what == 'range -5..-1' and text.count('-') > answered.count('-'):
            return 'N9b'
        return 'N9a'
    if group == 25:
        return 'N3b'
    if group in (26, 48):
        return 'N4'
    if group in (31, 54):
        return 'N6c'
    if '999.995' in what:
        return 'N6a'
    if '5e-324' in what:
        return 'N6b'
    if '123456789012345678901234567890.5' in what:
        return 'N6e'
    return 'unclassified'


def write(name, header, rows_out):
    with open(rows / 'runs' / name, 'w', encoding='utf-8') as f:
        f.write('\t'.join(header) + '\n')
        for r in rows_out:
            f.write('\t'.join(r) + '\n')


counts = collections.Counter()
taken = set()
with open(mp / 'runs/d-numbers.classes.tsv', encoding='utf-8') as f:
    for line in f:
        if line[:1].isdigit():
            taken.add(line.split('\t')[0])
out = []
for n, src, e, a in lines('d-numbers.bjs.compare.txt'):
    if n in taken:
        counts['sub-area b (rows-MP/runs/d-numbers.classes.tsv)'] += 1
        continue
    r = number_row(e, a)
    counts['d-numbers ' + r] += 1
    out.append((n, src, r, e[:240], a[:240]))
write('d-numbers.classes-ns.tsv', ['line', 'source of the expected answer', 'row', 'expected (Broiler.VM, = the run), to 240 characters', 'Broiler.JS, to 240 characters'], out)

out = []
for n, src, e, a in lines('d-relativetimes.bjs.compare.txt'):
    r = 'S5' if 'numberingSystem' in e else 'unclassified'
    counts['d-relativetimes ' + r] += 1
    out.append((n, src, r, e[:240], a[:240]))
write('d-relativetimes.classes.tsv', ['line', 'source of the expected answer', 'row', 'expected', 'Broiler.JS'], out)

out = []
for n, src, e, a in lines('d-displaynames-patched.bjs.compare.txt'):
    ev = e.split(' => ', 1)[1].split(' | ')
    av = a.split(' => ', 1)[1].split(' | ')
    if ev[0] != av[0] and ev[0].lower() == av[0].lower() and '"type":"dateTimeField"' not in e:
        r = 'S9'
    else:
        r = 'S8'
    counts['d-displaynames-patched ' + r] += 1
    out.append((n, src, r, e[:240], a[:240]))
write('d-displaynames-patched.classes.tsv', ['line', 'source of the expected answer', 'row', 'expected', 'Broiler.JS'], out)

for k, v in sorted(counts.items()):
    print(v, k, sep='\t')
