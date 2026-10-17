# Compares ICU4X 2.3.0's ummalqura_data.rs (Broiler.VM's generator input) with the CLDR package's
# CldrUmmAlQura as Broiler.JS reads it (netprobe output), year by year: 1 Muharram and month lengths.
import re, sys
C = sys.argv[1]
src = open(f'{C}/crates/icu_calendar-2.3.0/src/cal/hijri/ummalqura_data.rs').read()
icu = {}
for m in re.finditer(r'PackedHijriYearData::try_new\((\d+), \[([ls, ]+)\], gregorian\((\d+), (\d+), (\d+)\)\)', src):
    y = int(m.group(1)); icu[y] = ('%04d-%02d-%02d' % (int(m.group(3)), int(m.group(4)), int(m.group(5))), m.group(2).replace(', ', ''))
cldr = {}
for l in open(f'{C}/runs/netprobe-umalqura.txt'):
    if l.startswith('cldr '):
        _, y, d, s = l.split(); cldr[int(y)] = (d, s)
print('ICU4X years', min(icu), '..', max(icu), len(icu), '; CLDR years', min(cldr), '..', max(cldr), len(cldr))
diff = [y for y in sorted(set(icu) | set(cldr)) if icu.get(y) != cldr.get(y)]
print('years differing:', len(diff))
for y in diff[:20]: print(' ', y, 'ICU4X', icu.get(y), 'CLDR', cldr.get(y))
# ICU4C's encoded month lengths, as ICU4X's own test states them, against the CLDR hex
hexs = re.search(r'ICU4C_ENCODED_MONTH_LENGTHS: \[u16; 1601 - 1300\] = \[([^\]]+)\]', src).group(1)
icu4c = [int(h, 16) for h in re.findall(r'0x[0-9A-Fa-f]+', hexs)]
cl = open('<Broiler.Unicode>/src/UnicodeCldr.LocaleData/CldrUmmAlQura.cs').read()
cldrhex = [int(h, 16) for h in re.findall(r'0x[0-9A-Fa-f]{4}', cl.split('MonthLengthBits =')[1].split('];')[0])]
print('ICU4C hex entries (in ICU4X test)', len(icu4c), '; CLDR hex entries', len(cldrhex), '; identical:', icu4c == cldrhex)
