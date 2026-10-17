import sys
C = sys.argv[1]; name = sys.argv[2] if len(sys.argv) > 2 else 'east-asian-years'
def load(p):
    d = {}
    for l in open(p):
        k, v = l.rstrip('\n').split(' => ', 1); d[k] = v
    return d
vm = load(f'{C}/runs/{name}.vm.txt'); js = load(f'{C}/runs/{name}.js.txt')
def ranges(ys):
    out = []; ys = sorted(ys)
    for y in ys:
        if out and y == out[-1][1] + 1: out[-1][1] = y
        else: out.append([y, y])
    return ', '.join(f'{a}' if a == b else f'{a}-{b}' for a, b in out)
for cal in ['chinese', 'dangi']:
    keys = [k for k in vm if k.startswith(cal + ' ')]
    same = [int(k.split()[1]) for k in keys if vm[k] == js[k]]
    diff = [int(k.split()[1]) for k in keys if vm[k] != js[k]]
    ny = [int(k.split()[1]) for k in keys if vm[k] != js[k] and vm[k].split()[0] != js[k].split()[0]]
    lp = [int(k.split()[1]) for k in keys if vm[k] != js[k] and vm[k].split()[3:4] != js[k].split()[3:4]]
    print(f'{cal}: {len(keys)} years probed; equal in {len(same)}: {ranges(same)}')
    print(f'  differ in {len(diff)}: {ranges(diff)}')
    print(f'  of which new-year date differs: {ranges(ny)}')
    print(f'  of which leap month (code@ordinal) differs: {ranges(lp)}')
