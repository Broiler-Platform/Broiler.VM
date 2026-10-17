# Checks, for every zone in tzdata.bin, whether TZif time type 0 (RFC 8536 3.2's type before the
# first transition) is the type TzifReader picks (the first non-DST type), and whether any
# 64-bit block lists leap seconds.
import sys, struct, zlib
data = zlib.decompress(open(sys.argv[1],'rb').read(), -15)
pos = 4
def i32():
    global pos; v = struct.unpack_from("<i", data, pos)[0]; pos += 4; return v
def s():
    global pos; n = i32(); v = data[pos:pos+n].decode(); pos += n; return v
i32(); s(); zc = i32(); diff = []; leaps = []
for _ in range(zc):
    n = s(); l = i32(); tz = data[pos:pos+l]; pos += l
    isut, isstd, leap, tc, ty, ch = struct.unpack_from(">6i", tz, 20)
    p = 44 + tc*5 + ty*6 + ch + leap*8 + isstd + isut
    isut, isstd, leap, tc, ty, ch = struct.unpack_from(">6i", tz, p+20); p += 44 + tc*9
    types = [struct.unpack_from(">ibB", tz, p+6*i) for i in range(ty)]
    first_std = next((i for i,(o,d,a) in enumerate(types) if not d), 0)
    if first_std != 0: diff.append((n, types[0], types[first_std]))
    if leap: leaps.append(n)
print("zones whose type 0 is not the first non-DST type:", diff)
print("zones with leap-second records:", leaps)
