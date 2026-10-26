#!/usr/bin/env python3
"""Compares one engine's lines of a dataset probe (runs/d-<name>.<tag>.txt or .txt.gz) with the expected answer:
the retained answer with Broiler.VM's divergences applied, by Broiler.JS's dataset harness's own compare_program
(scripts/conformance/run_reference_datasets.py of the f918b040 worktree, imported, not modified). Writes
runs/d-<name>.<tag>.compare.txt: the summary and every differing line (number, source, expected, answered).
Rows N and S's form (rows-NS/work/compare_dataset.py).

Usage: compare_dataset.py <rows-dir> <name> <tag>
"""
import gzip, importlib.util, sys
from pathlib import Path

rows = Path(sys.argv[1]).resolve()
name, tag = sys.argv[2], sys.argv[3]
spec = importlib.util.spec_from_file_location('harness', '/home/user/s9-js/scripts/conformance/run_reference_datasets.py')
h = importlib.util.module_from_spec(spec); sys.modules['harness'] = h; spec.loader.exec_module(h)
answers = {'dates': 'dates.icu-77.1.txt', 'timezones': 'timezones.icu-77.1.txt', 'temporal-intl': 'temporal-intl.polyfill-e8cc03fc.txt',
           'calendar-formats': 'calendar-formats.polyfill-e8cc03fc.txt', 'zone-names': 'zone-names.polyfill-e8cc03fc.txt'}
ds = rows / 'work/datasets'
def rdb(f):
    return f.read_bytes() if f.exists() else gzip.open(str(f) + '.gz', 'rb').read()
p = rows / 'runs' / ('d-%s.%s.txt' % (name, tag))
text = p.read_text(encoding='utf-8') if p.exists() else gzip.open(str(p) + '.gz', 'rt', encoding='utf-8').read()
lines = [l for l in text.split('\n') if l and not l.startswith('# ') and not l.startswith('exit=')]
expected, results = h.compare_program(h.read_all_text(rdb(ds / ('%s--%s' % (name, answers[name])))),
                                      h.read_all_text(rdb(ds / ('%s--divergences.txt' % name))), lines)
eq = sum(1 for r in results if r.equal)
out = ['# compare: work/compare_dataset.py <rows> %s %s (Broiler.JS harness compare_program at f918b040 over runs/d-%s.%s.txt;'
       ' expected = work/datasets/%s--%s with work/datasets/%s--divergences.txt applied)' % (name, tag, name, tag, name, answers[name], name),
       'summary: lines=%d compared=%d equal=%d differing=%d divergences-used=%d' % (len(lines), len(results), eq, len(results) - eq, expected.used)]
for r in results:
    if not r.equal:
        out.append('%d\t%s\texpected: %s\tanswered: %s' % (r.number, r.source, r.expected, r.actual))
(rows / 'runs' / ('d-%s.%s.compare.txt' % (name, tag))).write_text('\n'.join(out) + '\n', encoding='utf-8')
print(name, tag, out[1])
