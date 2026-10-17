# For the identifiers whose histories differ only before 1970 (probe z10), says whether each is a
# Zone in Broiler.JS's data while it is a Link in tzdb 2026e's default build (backzone's mark).
import sys, struct, zlib, tarfile, io
cmp_path, bin_path, tar_path = sys.argv[1:4]
ids = []
f = False
for line in open(cmp_path):
    if line.startswith('differing only before 1970'): f = True; continue
    if f and line.strip(): ids.append(line.split()[0])
data = zlib.decompress(open(bin_path,'rb').read(), -15)
pos = 4
def i32():
    global pos; v = struct.unpack_from("<i", data, pos)[0]; pos += 4; return v
def s():
    global pos; n = i32(); v = data[pos:pos+n].decode(); pos += n; return v
i32(); s(); zc = i32(); zones = set(); 
for _ in range(zc):
    n = s(); l = i32(); pos += l; zones.add(n)
lc = i32(); links = {}
for _ in range(lc):
    a = s(); t = s(); links[a] = t
default_links = {}; default_zones = set(); backzone_zones = set()
with tarfile.open(tar_path) as tf:
    for name in ["africa","antarctica","asia","australasia","europe","northamerica","southamerica","etcetera","backward"]:
        for line in tf.extractfile(name).read().decode().splitlines():
            p = line.split('#')[0].split()
            if not p: continue
            if p[0] == 'Link': default_links[p[2]] = p[1]
            elif p[0] == 'Zone': default_zones.add(p[1])
    for line in tf.extractfile('backzone').read().decode().splitlines():
        p = line.split('#')[0].split()
        if p and p[0] == 'Zone': backzone_zones.add(p[1])
kinds = {}
for i in ids:
    if i in zones and i in default_links: k = 'Link in the default build, Zone in Broiler.JS (backzone Zone: %s)' % (i in backzone_zones)
    elif i in links and i in default_links and links[i] != default_links[i]: k = 'Link to another target'
    else: k = 'other'
    kinds.setdefault(k, []).append(i)
for k, v in kinds.items(): print(len(v), k, ':', ' '.join(v))
print('Broiler.JS zones that are default-build Links:', len([z for z in zones if z in default_links]), '; default-build Zones:', len(default_zones))
