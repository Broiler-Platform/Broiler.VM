# Compares the two engines' offset histories (probe z10) and classifies each identifier:
# identical; differing only before 1970; differing from 1970 to 2037.
import sys, bisect
def load(p):
    d = {}
    for line in open(p, encoding='utf-8'):
        line = line.strip()
        if not line or line.startswith('exit='): continue
        parts = line.split(' ')
        initial = int(float(parts[1])); tr = [(int(float(a)), int(float(b))) for a, b in (x.split('/') for x in parts[2:])]
        d[parts[0]] = (initial, tr)
    return d
vm, bj = load(sys.argv[1]), load(sys.argv[2])
Y1970 = 0; Y2038 = 2145916800
def offset_at(z, t):
    init, tr = z; i = bisect.bisect_right([a for a, _ in tr], t) - 1
    return init if i < 0 else tr[i][1]
def instants(z): return {a for a, _ in z[1]}
same = []; pre = []; post = []
details = []
for k in sorted(vm):
    a, b = vm[k], bj[k]
    if a == b: same.append(k); continue
    pts = sorted(instants(a) | instants(b) | {-5364662400})
    bad = [t for t in pts if offset_at(a, t) != offset_at(b, t) or offset_at(a, t - 1) != offset_at(b, t - 1)]
    if not bad: same.append(k); continue
    later = [t for t in bad if t >= Y1970]
    (post if later else pre).append(k)
    t0 = later[0] if later else bad[0]
    details.append((k, 'from 1970' if later else 'before 1970 only', t0, offset_at(a, t0), offset_at(b, t0)))
import datetime
iso = lambda t: (datetime.datetime(1970, 1, 1) + datetime.timedelta(seconds=t)).isoformat() + 'Z'
print('identifiers compared:', len(vm), '; identical histories 1800-2037:', len(same), '; differing only before 1970:', len(pre), '; differing in 1970-2037:', len(post))
print('differing in 1970-2037 (first differing instant, Broiler.VM offset, Broiler.JS offset):')
for k, w, t, o1, o2 in details:
    if w == 'from 1970': print('  ', k, iso(t), o1, o2)
print('differing only before 1970 (first differing instant, Broiler.VM offset, Broiler.JS offset):')
for k, w, t, o1, o2 in details:
    if w != 'from 1970': print('  ', k, iso(t), o1, o2)
