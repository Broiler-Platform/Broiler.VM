# Compares Broiler.VM's offset history of each identifier (probe z10, 1800 to 2037) with the host
# zic's -b fat compilation of the pinned 2026e source files: the offset at every transition instant
# of either side and the second before it.
import sys, os, struct, bisect, datetime
dump, zicdir = sys.argv[1], sys.argv[2]
def tzif(path):
    tz = open(path, 'rb').read()
    isut, isstd, leap, tc, ty, ch = struct.unpack_from(">6i", tz, 20)
    p = 44 + tc*5 + ty*6 + ch + leap*8 + isstd + isut
    isut, isstd, leap, tc, ty, ch = struct.unpack_from(">6i", tz, p+20); p += 44
    times = list(struct.unpack_from(">%dq" % tc, tz, p)); p += 8*tc; idx = tz[p:p+tc]; p += tc
    types = [struct.unpack_from(">ibB", tz, p+6*i)[0] for i in range(ty)]
    return types[0], times, [types[i] for i in idx]
def at(init, times, offs, t):
    i = bisect.bisect_right(times, t) - 1
    return init if i < 0 else offs[i]
lo, hi = -5364662400, 2145916800
diff = []; n = 0
for line in open(dump, encoding='utf-8'):
    parts = line.strip().split(' ')
    if len(parts) < 2: continue
    name = parts[0]; n += 1
    vinit = int(float(parts[1])); vt = [(int(float(a)), int(float(b))) for a, b in (x.split('/') for x in parts[2:])]
    vtimes = [a for a, _ in vt]; voffs = [b for _, b in vt]
    zinit, ztimes, zoffs = tzif(os.path.join(zicdir, name))
    pts = sorted(set([t for t in vtimes] + [t for t in ztimes if lo <= t < hi] + [lo]))
    bad = [t for t in pts for u in (t, t - 1) if lo <= u < hi and at(vinit, vtimes, voffs, u) != at(zinit, ztimes, zoffs, u)]
    if bad:
        t = bad[0]; iso = (datetime.datetime(1970,1,1) + datetime.timedelta(seconds=t)).isoformat()
        last = (datetime.datetime(1970,1,1) + datetime.timedelta(seconds=bad[-1])).isoformat()
        diff.append((name, iso, at(vinit, vtimes, voffs, t), at(zinit, ztimes, zoffs, t), len(bad), last))
print('identifiers compared:', n, '; differing from zic -b fat of the same nine files, 1800-2037:', len(diff))
for d in diff: print('  ', d[0], 'first at', d[1], 'Broiler.VM', d[2], 'zic', d[3], '(%d instants, the last at %s)' % (d[4], d[5]))
