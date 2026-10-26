# Usage: curl -sS <tarball> | python3 -I cldr_digital.py <kind: units|numbers> <expected sha512 integrity>
# Streams an npm cldr-json tarball from stdin (never unpacked to disk), checks its sha512 against the
# registry's integrity value, and prints per locale: for units, the durationUnit-type-hms pattern; for
# numbers, defaultNumberingSystem and the timeSeparator of every symbols-numberSystem-* block.
import base64, hashlib, io, json, sys, tarfile
kind, integrity = sys.argv[1], sys.argv[2]
data = sys.stdin.buffer.read()
digest = 'sha512-' + base64.b64encode(hashlib.sha512(data).digest()).decode()
print('# tarball bytes %d, %s, registry integrity %s, %s' % (len(data), digest, integrity, 'EQUAL' if digest == integrity else 'DIFFERENT'))
rows = []
with tarfile.open(fileobj=io.BytesIO(data), mode='r:gz') as t:
    for m in t:
        if not m.isfile():
            continue
        parts = m.name.split('/')
        if kind == 'units' and parts[-1] == 'units.json' and 'main' in parts:
            loc = parts[parts.index('main') + 1]
            j = json.load(t.extractfile(m))['main'][loc]['units']
            hms = j.get('durationUnit-type-hms', {}).get('durationUnitPattern')
            if hms is None:
                hms = j.get('long', {}).get('durationUnit-type-hms', {}).get('durationUnitPattern')
            rows.append((loc, str(hms)))
        if kind == 'numbers' and parts[-1] == 'numbers.json' and 'main' in parts:
            loc = parts[parts.index('main') + 1]
            j = json.load(t.extractfile(m))['main'][loc]['numbers']
            seps = []
            for k in sorted(j):
                if k.startswith('symbols-numberSystem-'):
                    seps.append(k[len('symbols-numberSystem-'):] + '=' + j[k].get('timeSeparator', '(none)'))
            rows.append((loc, 'default=' + j.get('defaultNumberingSystem', '?') + ' ' + ' '.join(seps)))
for loc, v in sorted(rows):
    print(loc + '\t' + v)
