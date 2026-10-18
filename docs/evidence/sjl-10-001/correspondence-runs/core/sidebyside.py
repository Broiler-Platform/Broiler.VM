# Prints the engines' probe outputs line by line, with the reference polyfill's when it was run (SJL-8's
# form, with a third column). Each line starts with two marks, Broiler.VM's then Broiler.JS's: " " when
# the engine answers as the reference does (as the other engine does, when there is no reference), "~"
# when both throw the same constructor with different messages, "!" for any other difference.
import re, sys
def read(path):
    return [l for l in open(path, encoding='utf-8').read().strip('\n').split('\n')
            if not l.startswith('exit=') and (' => ' in l or l.startswith('NO OUTPUT'))]
def kind(v):
    m = re.match(r'THROWS (\S+?):', v)
    return m.group(1) if m else None
def mark(x, y):
    if x == y: return ' '
    vx, vy = x.split(' => ', 1)[-1], y.split(' => ', 1)[-1]
    if x.split(' => ')[0] == y.split(' => ')[0] and kind(vx) and kind(vx) == kind(vy): return '~'
    return '!'
vm, bjs = read(sys.argv[1]), read(sys.argv[2])
ref = read(sys.argv[3]) if len(sys.argv) > 3 else None
for i, (x, y) in enumerate(zip(vm, bjs)):
    k = x.split(' => ')[0]
    val = lambda l: l.split(' => ', 1)[-1]
    if ref is None:
        m = mark(x, y); line = m + m + ' ' + k + ' | VM: ' + val(x) + ' | BJS: ' + val(y)
    else:
        z = ref[i] if i < len(ref) else 'MISSING'
        line = mark(x, z) + mark(y, z) + ' ' + k + ' | VM: ' + val(x) + ' | BJS: ' + val(y) + ' | REF: ' + val(z)
    print(line)
if len(vm) != len(bjs) or (ref is not None and len(ref) != len(vm)):
    print('LENGTH DIFF', len(vm), len(bjs), len(ref) if ref is not None else '-')
