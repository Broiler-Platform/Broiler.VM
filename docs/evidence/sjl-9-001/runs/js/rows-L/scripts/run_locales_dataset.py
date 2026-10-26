#!/usr/bin/env python3
"""Runs Broiler.VM's locales dataset (src/tests/cldr/locales/ at cd1928d, copied into work/datasets/) through the
Broiler.JS shell with Broiler.JS's own dataset harness (scripts/conformance/run_reference_datasets.py of the
f918b040 worktree, imported, not modified), and writes one run file: the command, revisions, the summary and every
line that differs from the expected answer (the retained ICU 77.1 answer with Broiler.VM's divergences applied),
each with its kind (the word before ' => ') and the source of its expectation (retained or divergence).

Usage: run_locales_dataset.py <rows-dir>
"""
import collections
import hashlib
import importlib.util
import os
import subprocess
import sys
from pathlib import Path

rows = Path(sys.argv[1]).resolve()
tree = Path('/home/user/s9-js')
dll = tree / 'Broiler.JS/Broiler.JavaScript/bin/Release/net10.0/BroilerJS.dll'
datasets = rows / 'work/datasets'
out_dir = rows / 'work/locales-run'
out_dir.mkdir(parents=True, exist_ok=True)
spec = importlib.util.spec_from_file_location('harness', tree / 'scripts/conformance/run_reference_datasets.py')
h = importlib.util.module_from_spec(spec)
sys.modules['harness'] = h
spec.loader.exec_module(h)
os.environ['PATH'] = '/root/.dotnet:' + os.environ.get('PATH', '')
os.environ['DOTNET_ROOT'] = '/root/.dotnet'
prog = h.Program('locales', 'locales--locales.js', 'locales--locales.icu-77.1.txt', 'locales--divergences.txt')
run = h.run_program(dll, datasets, prog, out_dir, 900.0)


def kind(source):
    i = source.find(' => ')
    head = source[:i] if i >= 0 else source
    word = head.split(' ')[-1]
    return word if not word.startswith('{') and not word[0].isdigit() else '(identifier)'


def sha(p):
    return hashlib.sha256(p.read_bytes()).hexdigest()


lines = []
lines.append('# command: python3 -I work/run_locales_dataset.py <rows>  (imports %s/scripts/conformance/run_reference_datasets.py; '
             'its run_program runs `dotnet %s --script-host driver-locales.js` from work/datasets/ with no LANG and TZ=UTC)' % (tree, dll))
rev = subprocess.run(['git', '-C', str(tree), 'rev-parse', 'HEAD'], capture_output=True, text=True).stdout.strip()
lines.append('# revision: Broiler.JS %s; the dataset copied from Broiler.VM cd1928d67a735c467ee0a93e4640834eb4bd2e40 src/tests/cldr/locales/' % rev)
for f in sorted(datasets.iterdir()):
    lines.append('# input: %s sha256 %s' % (f.name, sha(f)))
equal = sum(1 for r in run.results if r.equal)
lines.append('summary: completed=%s exit=%d retained=%d compared=%d equal=%d differing=%d divergence-entries=%d divergences-used=%d'
             % (run.outcome.completed, run.exit_code, run.retained_lines, len(run.results), equal, len(run.results) - equal,
                run.divergence_entries, run.expected.used))
by = collections.Counter()
for r in run.results:
    if not r.equal:
        by[(kind(r.key if hasattr(r, 'key') and r.key else r.expected), r.source)] += 1
for (k, s), n in sorted(by.items()):
    lines.append('differing by kind: %s from %s: %d' % (k, s, n))
for r in run.results:
    if not r.equal:
        lines.append('%d\t%s\texpected: %s\tBroiler.JS: %s' % (r.number, r.source, r.expected, r.actual))
lines.append('exit=%d' % run.exit_code)
(rows / 'runs/locales-dataset.bjs.txt').write_text('\n'.join(lines) + '\n', encoding='utf-8')
print('\n'.join(lines[:30]))
