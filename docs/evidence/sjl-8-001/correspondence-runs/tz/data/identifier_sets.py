# Compares the identifier sets: Broiler.VM's TimeZoneIds table and Broiler.JS's tzdata.bin zones and links.
import sys, struct, zlib, re
vm_table, bin_path = sys.argv[1], sys.argv[2]
text = open(vm_table, encoding='utf-8').read()
block = text[text.index('TimeZoneIds =>'):text.index('TimeZoneRegions =>')]
vm = {}
for line in block.splitlines():
    line = line.strip()
    m = re.match(r'^([A-Za-z0-9_+\-/]+)\|(\d+)\|(.*)$', line)
    if m: vm[m.group(1)] = (int(m.group(2)), m.group(3) or m.group(1))
data = zlib.decompress(open(bin_path,'rb').read(), -15)
pos = 4
def i32():
    global pos; v = struct.unpack_from("<i", data, pos)[0]; pos += 4; return v
def s():
    global pos; n = i32(); v = data[pos:pos+n].decode(); pos += n; return v
i32(); ver = s(); zc = i32(); zones = []
for _ in range(zc):
    n = s(); l = i32(); pos += l; zones.append(n)
lc = i32(); links = {}
for _ in range(lc):
    a = s(); t = s(); links[a] = t
bjs = set(zones) | set(links)
print("Broiler.VM identifiers:", len(vm), " primaries:", sum(1 for k,(z,p) in vm.items() if p == k))
print("Broiler.JS identifiers:", len(bjs), "(zones", len(zones), "links", len(links), ") release", ver)
print("only in Broiler.VM:", sorted(set(vm) - bjs))
print("only in Broiler.JS:", sorted(bjs - set(vm)))
# Broiler.JS primary: link target (one hop, as GetPrimary does), legacy aliases, UTC family
legacy = {"CET":"Europe/Brussels","CST6CDT":"America/Chicago","EET":"Europe/Athens","EST":"America/Panama","EST5EDT":"America/New_York","HST":"Pacific/Honolulu","MET":"Europe/Brussels","MST":"America/Phoenix","MST7MDT":"America/Denver","PST8PDT":"America/Los_Angeles","WET":"Europe/Lisbon"}
for a,t in legacy.items():
    if a not in links and t in zones: links[a] = t
def bjs_primary(n):
    p = links.get(n, n)
    return "UTC" if p in ("Etc/UTC","Etc/GMT","GMT") else p
link_to_link = sorted(a for a,t in links.items() if t in links)
print("Broiler.JS links whose target is itself a link:", link_to_link)
diff = []
for n in sorted(set(vm) & bjs):
    vp = vm[n][1]; bp = bjs_primary(n)
    if vp != bp: diff.append((n, vp, bp))
print("identifiers whose primary differs (identifier, Broiler.VM primary, Broiler.JS primary):", len(diff))
for d in diff: print("  ", *d)
bjs_prims = sorted(n for n in bjs if bjs_primary(n) == n)
vm_prims = sorted(k for k,(z,p) in vm.items() if p == k)
print("Broiler.JS primaries:", len(bjs_prims))
print("primary in Broiler.VM only:", sorted(set(vm_prims) - set(bjs_prims)))
print("primary in Broiler.JS only:", sorted(set(bjs_prims) - set(vm_prims)))
