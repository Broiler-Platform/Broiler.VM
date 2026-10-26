#!/usr/bin/env python3
"""Compares runs/l10-locales-dataset.<tag>.txt (one engine's lines of the locales dataset program) with the expected
answer (the retained ICU 77.1 answer with Broiler.VM's divergences applied), by Broiler.JS's dataset harness's own
compare_program (scripts/conformance/run_reference_datasets.py at f918b040, imported). Writes
runs/l10-locales-dataset.<tag>.compare.txt. Usage: compare_locales_dataset.py <rows-dir> <tag>"""
import importlib.util, sys, collections
from pathlib import Path
rows = Path(sys.argv[1]).resolve(); tag = sys.argv[2]
spec = importlib.util.spec_from_file_location('harness', '/home/user/s9-js/scripts/conformance/run_reference_datasets.py')
h = importlib.util.module_from_spec(spec); sys.modules['harness'] = h; spec.loader.exec_module(h)
ds = rows / 'work/datasets'
lines = [l for l in (rows / ('runs/l10-locales-dataset.%s.txt' % tag)).read_text(encoding='utf-8').split('\n')
         if l and not l.startswith('# ') and not l.startswith('exit=')]
expected, results = h.compare_program(h.read_all_text((ds / 'locales--locales.icu-77.1.txt').read_bytes()),
                                      h.read_all_text((ds / 'locales--divergences.txt').read_bytes()), lines)
eq = sum(1 for r in results if r.equal)
out = ['# compare: work/compare_locales_dataset.py <rows> %s (Broiler.JS harness compare_program over runs/l10-locales-dataset.%s.txt)' % (tag, tag),
       'summary: lines=%d compared=%d equal=%d differing=%d divergences-used=%d' % (len(lines), len(results), eq, len(results) - eq, expected.used)]
for r in results:
    if not r.equal:
        out.append('%d\t%s\texpected: %s\tanswered: %s' % (r.number, r.source, r.expected, r.actual))
(rows / ('runs/l10-locales-dataset.%s.compare.txt' % tag)).write_text('\n'.join(out) + '\n', encoding='utf-8')
print('\n'.join(out[:8]))
