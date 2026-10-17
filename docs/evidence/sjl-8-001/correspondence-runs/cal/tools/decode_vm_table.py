# Decodes Broiler.VM's generated JsCalendarTables.g.cs in the layout JsCalendarData reads
# (VM/JsCalendars.cs:1048-1140) and compares every year with the pinned crate members.
import re, sys, datetime
C = sys.argv[1]
g = open('<Broiler.VM>/src/Broiler.VM.Profile.JavaScript.Intl/JsCalendarTables.g.cs').read()
data = bytes(int(x) for x in re.findall(r'\b\d+\b', g.split('new byte[]')[1].split('};')[0]))
at = 0
def u16():
    global at; v = data[at] | data[at + 1] << 8; at += 2; return v
def section(size):
    global at; start, count = u16(), u16(); recs = data[at:at + count * size]; at += count * size; return start, count, recs
E = datetime.date(1970, 1, 1)
def iso(days): return (E + datetime.timedelta(days=days)).isoformat()
def days(y, m, d): return (datetime.date(y, m, d) - E).days
def east(start, count, recs):
    out = {}
    for i in range(count):
        p = recs[3*i] | recs[3*i+1] << 8 | recs[3*i+2] << 16
        leap = (p >> 13) & 0xF; n = 13 if leap else 12
        out[start + i] = (iso(days(start + i, 1, 19) + (p >> 17)), leap, ''.join('l' if (p >> m) & 1 else 's' for m in range(n)))
    return out
china = east(*section(3)); korea = east(*section(3)); qing = east(*section(3))
us, uc, ur = section(2)
civil_epoch = 227015 - 719163
uq = {}
for i in range(uc):
    y = us + i; p = ur[2*i] | ur[2*i+1] << 8
    tab = civil_epoch + (y - 1) * 354 + (3 + 11 * y) // 30
    uq[y] = (iso(tab + (p >> 12) - 8), ''.join('l' if (p >> m) & 1 else 's' for m in range(12)))
pc = u16(); persian = [u16() for _ in range(pc)]
print('bytes', len(data), 'consumed', at)
print('china', min(china), max(china), 'korea', min(korea), max(korea), 'qing', min(qing), max(qing), 'umalqura', min(uq), max(uq), 'persian corrections', len(persian), persian[0], persian[-1])
R = f'{C}/crates/icu_calendar-2.3.0/src/cal/'
def icu_east(f):
    d = {}
    for m in re.finditer(r'PackedEastAsianTraditionalYearData::new\((\d+), \[([ls, ]+)\], (None|Some\((\d+)\)), gregorian\((\d+), (\d+), (\d+)\)\)', open(R + f).read()):
        leap = int(m.group(4)) if m.group(4) else 0; s = m.group(2).replace(', ', '')
        d[int(m.group(1))] = ('%04d-%02d-%02d' % tuple(int(m.group(k)) for k in (5, 6, 7)), leap, s if leap else s[:12])
    return d
icu_uq = {}
for m in re.finditer(r'PackedHijriYearData::try_new\((\d+), \[([ls, ]+)\], gregorian\((\d+), (\d+), (\d+)\)\)', open(R + 'hijri/ummalqura_data.rs').read()):
    icu_uq[int(m.group(1))] = ('%04d-%02d-%02d' % tuple(int(m.group(k)) for k in (3, 4, 5)), m.group(2).replace(', ', ''))
cc = [int(x) for x in re.search(r'NON_LEAP_CORRECTION: \[i32; 78\] = \[([0-9,\s]+)\]', open(f'{C}/crates/calendrical_calculations-0.2.4/src/persian.rs').read()).group(1).replace(',', ' ').split()]
print('china == china_data.rs:', china == icu_east('east_asian_traditional/china_data.rs'))
print('korea == korea_data.rs:', korea == icu_east('east_asian_traditional/korea_data.rs'))
print('qing  == qing_data.rs: ', qing == icu_east('east_asian_traditional/qing_data.rs'))
print('umalqura == ummalqura_data.rs:', uq == icu_uq)
print('persian == NON_LEAP_CORRECTION:', persian == cc)
