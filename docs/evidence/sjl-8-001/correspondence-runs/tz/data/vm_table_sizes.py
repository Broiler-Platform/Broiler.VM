# Sizes of Broiler.VM's three generated time-zone tables, read from JsTzdbTables.g.cs.
import sys, re
t = open(sys.argv[1], encoding='utf-8').read()
a = t.index('TimeZones => new byte[]'); b = t.index('};', a)
nums = re.findall(r'\d+', t[a + len('TimeZones => new byte[]'):b])
print('TimeZones (binary) bytes:', len(nums))
for name, nxt in (('TimeZoneIds', 'TimeZoneRegions =>'), ('TimeZoneRegions', None)):
    s = t.index(name + ' =>'); s = t.index('"""', s) + 3; e = t.index('"""u8', s)
    body = [l for l in t[s:e].split('\n') if l.strip()]
    text = '\n'.join(l.strip() for l in body) + '\n'
    print(name, 'lines:', len(body), 'bytes (UTF-8, as the raw literal holds them):', len(text.encode()) - 1)
print('file bytes:', len(t.encode()))
