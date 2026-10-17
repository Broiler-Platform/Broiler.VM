# Three-way comparison of Chinese and Korean years: ICU4X 2.3.0's tables (Broiler.VM's source),
# .NET 10.0.12's lunisolar classes, and the CLDR package's computation (Broiler.JS's sources).
import re, sys
C = sys.argv[1]
R = f'{C}/crates/icu_calendar-2.3.0/src/cal/east_asian_traditional/'
def icu(file):
    d = {}
    for m in re.finditer(r'PackedEastAsianTraditionalYearData::new\((\d+), \[([ls, ]+)\], (None|Some\((\d+)\)), gregorian\((\d+), (\d+), (\d+)\)\)', open(R + file).read()):
        lengths = m.group(2).replace(', ', '')
        leap = int(m.group(4)) if m.group(4) else 0
        if leap == 0: lengths = lengths[:12]
        d[int(m.group(1))] = ('%04d-%02d-%02d' % (int(m.group(5)), int(m.group(6)), int(m.group(7))), leap, lengths)
    return d
qing = icu('qing_data.rs'); china = {**qing, **icu('china_data.rs')}; korea = {**qing, **icu('korea_data.rs')}
src = {'net-chinese': {}, 'net-korean': {}, 'cldr-chinese': {}}
for l in (x for x in open(f'{C}/runs/netprobe-eastasian.txt') if x.startswith(('net-', 'cldr-'))):
    n, y, d, leap, s = l.split(); src[n][int(y)] = (d, int(leap), s)
def ranges(ys):
    out = []
    for y in sorted(ys):
        if out and y == out[-1][1] + 1: out[-1][1] = y
        else: out.append([y, y])
    return ', '.join(f'{a}' if a == b else f'{a}-{b}' for a, b in out) or '(none)'
def cmp(label, a, b):
    common = sorted(set(a) & set(b))
    diff = [y for y in common if a[y] != b[y]]
    print(f'{label}: years compared {common[0]}-{common[-1]} ({len(common)}); differing: {ranges(diff)}')
    for y in diff[:6]: print(f'    {y}: {a[y]} vs {b[y]}')
cmp('ICU4X chinese vs .NET ChineseLunisolarCalendar', china, src['net-chinese'])
cmp('ICU4X dangi   vs .NET KoreanLunisolarCalendar ', korea, src['net-korean'])
cmp('ICU4X chinese vs CLDR CldrChineseCalendar     ', china, src['cldr-chinese'])
cmp('.NET chinese  vs CLDR CldrChineseCalendar     ', src['net-chinese'], src['cldr-chinese'])
cmp('ICU4X dangi   vs CLDR CldrChineseCalendar     ', korea, src['cldr-chinese'])
