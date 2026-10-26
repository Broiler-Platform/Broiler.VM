#!/usr/bin/env python3
"""Compares one engine's lines of a dataset probe (runs/d-<area>.<tag>.txt, or .txt.gz) with the expected answer:
the retained ICU 77.1 answer with Broiler.VM's divergences applied, by Broiler.JS's dataset harness's own
compare_program (scripts/conformance/run_reference_datasets.py of the f918b040 worktree, imported, not modified).
Writes runs/d-<area>.<tag>.compare.txt: the summary and every differing line (number, source of the expectation,
expected, answered). <area> may be displaynames-patched (compared with the displaynames answer).

Usage: compare_dataset.py <rows-dir> <area> <tag>
"""
import gzip
import importlib.util
import sys
from pathlib import Path

rows = Path(sys.argv[1]).resolve()
area, tag = sys.argv[2], sys.argv[3]
spec = importlib.util.spec_from_file_location('harness', '/home/user/s9-js/scripts/conformance/run_reference_datasets.py')
h = importlib.util.module_from_spec(spec)
sys.modules['harness'] = h
spec.loader.exec_module(h)
data = 'displaynames' if area == 'displaynames-patched' else area
ds = rows / 'work/datasets'
p = rows / 'runs' / ('d-%s.%s.txt' % (area, tag))
text = gzip.open(str(p) + '.gz', 'rt', encoding='utf-8').read() if not p.exists() else p.read_text(encoding='utf-8')
lines = [l for l in text.split('\n') if l and not l.startswith('# ') and not l.startswith('exit=')]
expected, results = h.compare_program(h.read_all_text((ds / ('%s--%s.icu-77.1.txt' % (data, data))).read_bytes()),
                                      h.read_all_text((ds / ('%s--divergences.txt' % data)).read_bytes()), lines)
eq = sum(1 for r in results if r.equal)
out = ['# compare: work/compare_dataset.py <rows> %s %s (Broiler.JS harness compare_program at f918b040 over runs/d-%s.%s.txt;'
       ' expected = work/datasets/%s--%s.icu-77.1.txt with work/datasets/%s--divergences.txt applied)' % (area, tag, area, tag, data, data, data),
       'summary: lines=%d compared=%d equal=%d differing=%d divergences-used=%d' % (len(lines), len(results), eq, len(results) - eq, expected.used)]
for r in results:
    if not r.equal:
        out.append('%d\t%s\texpected: %s\tanswered: %s' % (r.number, r.source, r.expected, r.actual))
dest = rows / 'runs' / ('d-%s.%s.compare.txt' % (area, tag))
dest.write_text('\n'.join(out) + '\n', encoding='utf-8')
print('\n'.join(out[:3]))
