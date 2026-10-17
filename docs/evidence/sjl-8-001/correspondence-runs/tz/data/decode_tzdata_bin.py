# Decodes Broiler.JS's tzdata.bin (raw DEFLATE of the TZDB pack) and reports facts about it.
import sys, struct, zlib, hashlib
path = sys.argv[1]
raw = open(path, 'rb').read()
data = zlib.decompress(raw, -15)
print("packed bytes", len(raw), "sha256", hashlib.sha256(raw).hexdigest())
print("inflated bytes", len(data))
pos = 0
assert data[:4] == b"TZDB"; pos = 4
def i32():
    global pos
    v = struct.unpack_from("<i", data, pos)[0]; pos += 4; return v
def s():
    n = i32(); global pos
    v = data[pos:pos+n].decode("utf-8"); pos += n; return v
fmt = i32(); ver = s()
print("format", fmt, "version", ver)
zc = i32(); zones = {}
for _ in range(zc):
    n = s(); l = i32(); zones[n] = data[pos:pos+l]; pos += l
lc = i32(); links = {}
for _ in range(lc):
    a = s(); t = s(); links[a] = t
print("zones", zc, "links", lc, "identifiers", zc + lc)
print("Factory:", "zone" if "Factory" in zones else ("link" if "Factory" in links else "absent"))

def parse(tz):
    ver = chr(tz[4])
    def counts(p):
        return struct.unpack_from(">6i", tz, p)
    c1 = counts(20)
    isut, isstd, leap, timecnt, typecnt, charcnt = c1
    p = 44 + timecnt*4 + timecnt + typecnt*6 + charcnt + leap*8 + isstd + isut
    if ver in "23":
        assert tz[p:p+4] == b"TZif"
        c2 = counts(p+20); p += 44
        isut, isstd, leap, timecnt, typecnt, charcnt = c2
        times = list(struct.unpack_from(">%dq" % timecnt, tz, p)); p += 8*timecnt
        idx = list(tz[p:p+timecnt]); p += timecnt
        types = []
        for i in range(typecnt):
            off, dst, ai = struct.unpack_from(">ibB", tz, p); p += 6; types.append((off, dst))
        p += charcnt + leap*12 + isstd + isut
        footer = tz[p:].decode("ascii").strip("\n")
    else:
        footer = None; times=[]; idx=[]; types=[]
    return ver, times, idx, types, footer

import datetime
def iso(t):
    return (datetime.datetime(1970,1,1) + datetime.timedelta(seconds=t)).isoformat() + "Z"
for name in sys.argv[2:]:
    key = links.get(name, name)
    kind = "link -> " + links[name] if name in links else ("zone" if name in zones else "absent")
    if key not in zones:
        print(name, kind); continue
    ver, times, idx, types, footer = parse(zones[key])
    last = iso(times[-1]) if times else None
    first = [(iso(times[i]), types[idx[i]][0]) for i in range(min(3, len(times)))]
    print(f"{name}: {kind}; TZif v{ver}; {len(times)} transitions; first {first}; last {last} -> {types[idx[-1]][0] if times else None}; footer {footer!r}; type0 {types[0] if types else None}")
