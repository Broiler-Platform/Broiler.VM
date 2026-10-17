import sys, collections
C, D = sys.argv[1], sys.argv[2]
vm = open(f'{C}/runs/retained-calendars.vm.txt').read().rstrip('\n').split('\n')
js = open(f'{C}/runs/retained-calendars.js.txt').read().rstrip('\n').split('\n')
poly = open(f'{D}/temporal-calendars.polyfill-e8cc03fc.txt').read().rstrip('\n').split('\n')
icu4x = open(f'{D}/temporal-calendars.icu4x-2.3.0.txt').read().rstrip('\n').split('\n')
div = {}
for l in open(f'{D}/divergences.txt'):
    l = l.rstrip('\n')
    if l and not l.startswith('#'): div[l.split(' => ')[0]] = l
key = lambda l: l.split(' => ')[0]
# Broiler.VM against the retained answers
vm_vs_poly = [ (p, v) for p, v in zip(poly, vm) if (div.get(key(p), p) != v) ]
print('VM lines', len(vm), 'polyfill lines', len(poly), 'VM differing from polyfill-or-divergence:', len(vm_vs_poly))
vmset = set(vm)
print('ICU4X conversion lines', len(icu4x), 'missing from VM:', sum(1 for l in icu4x if l not in vmset))
# Broiler.JS against Broiler.VM, conversion lines only (first 2055) and the rest
conv = 2055
bycal = collections.Counter(); rest = collections.Counter(); total = collections.Counter()
dif_conv = []
for i, (v, j) in enumerate(zip(vm, js)):
    cal = v.split(' ')[0]
    if i < conv:
        total[cal] += 1
        if v != j: bycal[cal] += 1; dif_conv.append((v, j, poly[i], icu4x[i] if i < len(icu4x) else ''))
    elif v != j: rest[cal] += 1
print('\nconversion lines (fields of 137 ISO dates) differing, Broiler.JS vs Broiler.VM, by calendar:')
for cal in sorted(total): print(f'  {cal:18} {bycal[cal]:3} of {total[cal]}')
print('\nother lines (arithmetic, from-fields, month-days; SJL-10) differing, by calendar:')
for cal in sorted(rest): print(f'  {cal:18} {rest[cal]:3}')
with open(f'{C}/runs/retained-calendars.conversions-differing.txt', 'w') as f:
    f.write('# Conversion lines of temporal-calendars.js where Broiler.JS answers otherwise than Broiler.VM.\n')
    f.write('# Each block: VM (= ICU4X 2.3.0, retained), JS (this run), and the polyfill (ICU4C 77.1, retained).\n')
    for v, j, p, x in dif_conv:
        f.write(f'VM     {v}\nJS     {j.split(" => ")[1]}\nICU4C  {p.split(" => ")[1]}\n\n')
# JS vs polyfill on conversion lines
jsp = sum(1 for i in range(conv) if js[i] != poly[i])
print('\nconversion lines where Broiler.JS differs from the polyfill (ICU4C 77.1):', jsp)
