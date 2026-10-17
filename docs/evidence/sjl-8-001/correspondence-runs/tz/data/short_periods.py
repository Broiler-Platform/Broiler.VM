# Lists zones in Broiler.JS's tzdata.bin with an offset in force for less than two days
# between two changes of offset (where two samples a day either side can miss it).
import sys, struct, zlib, datetime
data = zlib.decompress(open(sys.argv[1],'rb').read(), -15)
pos = 4
def i32():
    global pos; v = struct.unpack_from("<i", data, pos)[0]; pos += 4; return v
def s():
    global pos; n = i32(); v = data[pos:pos+n].decode(); pos += n; return v
i32(); s(); zc = i32(); zones = {}
for _ in range(zc):
    n = s(); l = i32(); zones[n] = data[pos:pos+l]; pos += l
def parse(tz):
    c1 = struct.unpack_from(">6i", tz, 20); isut, isstd, leap, tc, ty, ch = c1
    p = 44 + tc*5 + ty*6 + ch + leap*8 + isstd + isut
    c2 = struct.unpack_from(">6i", tz, p+20); p += 44
    isut, isstd, leap, tc, ty, ch = c2
    times = struct.unpack_from(">%dq" % tc, tz, p); p += 8*tc
    idx = tz[p:p+tc]; p += tc
    types = [struct.unpack_from(">ibB", tz, p+6*i)[0] for i in range(ty)]
    return times, [types[i] for i in idx], types[0]
iso = lambda t: (datetime.datetime(1970,1,1)+datetime.timedelta(seconds=t)).isoformat()
for name in sorted(zones):
    times, offs, init = parse(zones[name])
    # collapse no-op transitions
    ch = []
    prev = init
    for t, o in zip(times, offs):
        if o != prev: ch.append((t, prev, o)); prev = o
    for a, b in zip(ch, ch[1:]):
        if b[0] - a[0] < 2*86400:
            print(name, iso(a[0]), a[1], "->", a[2], "until", iso(b[0]), "->", b[2], "length_h", (b[0]-a[0])/3600)
