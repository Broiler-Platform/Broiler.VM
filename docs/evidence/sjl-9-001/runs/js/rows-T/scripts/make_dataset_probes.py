#!/usr/bin/env python3
"""Writes rows-T/probes/d-<name>.js for Broiler.VM's reference datasets of sub-area (d): src/tests/cldr/dates,
src/tests/cldr/timezones (row Z8's lines), src/tests/temporal/intl, calendars-intl and zone-names at cd1928d. Each
probe embeds the program byte for byte as a string and evaluates it by indirect eval, as the dataset READMEs run
it; one output line per line of the completion value; a program that throws is one line, ABORT <constructor>:
<message>. Rows N and S's form (rows-NS/work/make_dataset_probes.py). Copies each program, its retained answer
and its divergences file into work/datasets/<name>--<file>, with their sha256 in work/datasets/SHA256SUMS.

Usage: make_dataset_probes.py <rows-dir>
"""
import hashlib, json, shutil, sys
from pathlib import Path

rows = Path(sys.argv[1]).resolve()
src = Path('/home/user/s9-vm/src/tests')
probes = rows / 'probes'
ds = rows / 'work/datasets'
ds.mkdir(parents=True, exist_ok=True)
sets = [
    ('dates', 'cldr/dates', 'dates.js', 'dates.icu-77.1.txt'),
    ('timezones', 'cldr/timezones', 'timezones.js', 'timezones.icu-77.1.txt'),
    ('temporal-intl', 'temporal/intl', 'temporal-intl.js', 'temporal-intl.polyfill-e8cc03fc.txt'),
    ('calendar-formats', 'temporal/calendars-intl', 'calendar-formats.js', 'calendar-formats.polyfill-e8cc03fc.txt'),
    ('zone-names', 'temporal/zone-names', 'zone-names.js', 'zone-names.polyfill-e8cc03fc.txt'),
]
sums = []
for name, rel, prog, answer in sets:
    d = src / rel
    text = (d / prog).read_text(encoding='utf-8')
    body = ('// Rows T: Broiler.VM\'s reference dataset program src/tests/%s/%s at cd1928d, byte for byte as a\n'
            '// string, evaluated by indirect eval as its README says (the completion value is its lines).\n'
            '// The prelude\'s helpers are kept under names of their own first, in case the program declares globals.\n'
            'var __out = out, __errName = errName, __plain = plain;\n'
            'var __text = %s;\n'
            'var __value;\n'
            'try { __value = (0, eval)(__text); }\n'
            'catch (e) { __value = "ABORT " + __errName(e) + ": " + __plain(String(e && e.message)); }\n'
            'String(__value).split("\\n").forEach(function (line) { __out(line); });\n') % (rel, prog, json.dumps(text, ensure_ascii=True))
    (probes / ('d-' + name + '.js')).write_text(body, encoding='utf-8')
    for f, tgt in [(prog, prog), (answer, answer), ('divergences.txt', 'divergences.txt')]:
        shutil.copyfile(d / f, ds / (name + '--' + tgt))
        sums.append('%s  %s' % (hashlib.sha256((ds / (name + '--' + tgt)).read_bytes()).hexdigest(), name + '--' + tgt))
(ds / 'SHA256SUMS').write_text('\n'.join(sums) + '\n', encoding='utf-8')
print('\n'.join(sums))
