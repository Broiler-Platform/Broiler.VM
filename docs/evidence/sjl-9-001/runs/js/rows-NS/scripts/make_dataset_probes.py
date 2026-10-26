#!/usr/bin/env python3
"""Writes rows-NS/probes/d-<area>.js for Broiler.VM's reference datasets of sub-area c (numbers, lists,
relativetimes, displaynames), each the program src/tests/cldr/<area>/<area>.js at cd1928d embedded byte for byte
as a string and evaluated by indirect eval (as the dataset READMEs run it), one output line per program line.
A program that throws is reported as one line, ABORT <constructor>: <message>.
Also writes d-displaynames-patched.js: the same program with its last call,
Intl.DisplayNames.supportedLocalesOf, replaced by Intl.NumberFormat.supportedLocalesOf (the replacement the
JS survey made, survey-sjl9-js/datasets2/), so the lines before it can be compared in an engine that lacks it.
Copies each program, retained answer and divergences file into work/datasets/ as <area>--<file> (the names
Broiler.JS's harness reads), with their sha256 in work/datasets/SHA256SUMS.

Usage: make_dataset_probes.py <rows-dir>
"""
import hashlib
import json
import shutil
import sys
from pathlib import Path

rows = Path(sys.argv[1]).resolve()
src = Path('/home/user/s9-vm/src/tests/cldr')
probes = rows / 'probes'
ds = rows / 'work/datasets'
ds.mkdir(parents=True, exist_ok=True)
sums = []

def write_probe(name, text, note):
    body = ('// Rows N and S: %s\n' % note +
            '// The prelude\'s helpers are kept under names of their own first: the program declares globals of its\n'
            '// own (displaynames.js a variable `plain`, numbers.js functions `parts` and `show`) that replace them.\n'
            'var __out = out, __errName = errName, __plain = plain;\n'
            'var __text = ' + json.dumps(text, ensure_ascii=True) + ';\n'
            'var __value;\n'
            'try { __value = (0, eval)(__text); }\n'
            'catch (e) { __value = "ABORT " + __errName(e) + ": " + __plain(String(e && e.message)); }\n'
            'String(__value).split("\\n").forEach(function (line) { __out(line); });\n')
    (probes / (name + '.js')).write_text(body, encoding='utf-8')

for area in ['numbers', 'lists', 'relativetimes', 'displaynames']:
    d = src / area
    text = (d / (area + '.js')).read_text(encoding='utf-8')
    write_probe('d-' + area, text,
                "Broiler.VM's reference dataset program src/tests/cldr/%s/%s.js at cd1928d, byte for byte as a\n"
                "// string, evaluated by indirect eval as its README says (the completion value is its lines)." % (area, area))
    for f in [area + '.js', area + '.icu-77.1.txt', 'divergences.txt']:
        shutil.copyfile(d / f, ds / (area + '--' + f))
        sums.append('%s  %s' % (hashlib.sha256((ds / (area + '--' + f)).read_bytes()).hexdigest(), area + '--' + f))
    if area == 'displaynames':
        old = "Intl.DisplayNames.supportedLocalesOf(['de-AT', 'en', 'de-DE', 'zz'])"
        assert text.count(old) == 1
        patched = text.replace(old, "Intl.NumberFormat.supportedLocalesOf(['de-AT', 'en', 'de-DE', 'zz'])")
        write_probe('d-displaynames-patched', patched,
                    "src/tests/cldr/displaynames/displaynames.js at cd1928d with one change, the JS survey's\n"
                    "// (survey-sjl9-js/datasets2/): its last line calls Intl.NumberFormat.supportedLocalesOf where the program calls\n"
                    "// Intl.DisplayNames.supportedLocalesOf, so an engine without the latter still completes the other lines.")

(ds / 'SHA256SUMS').write_text('\n'.join(sums) + '\n', encoding='utf-8')
print('\n'.join(sums))
