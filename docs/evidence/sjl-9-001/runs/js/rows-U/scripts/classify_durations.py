# Usage: python3 -I classify_durations.py <rows-dir> <Broiler.VM checkout at cd1928d>
# Compares each engine's lines of the d-durations probe (runs/d-durations.{vm,vm-native,bjs}.txt) with Broiler.VM's
# retained answer src/tests/cldr/durations/durations.icu-77.1.txt after its divergences.txt is applied (a divergence
# line replaces the retained line whose text before " => " is the same), and writes runs/d-durations.classes.tsv:
# every line where an engine differs from that expected answer, with its divergence group (by the line range of
# divergences.txt it comes from) and the row of rows.md it falls in. Prints the counts.
import os, sys
rows_dir, vm_root = sys.argv[1], sys.argv[2]
base = os.path.join(vm_root, 'src/tests/cldr/durations')
ret = open(os.path.join(base, 'durations.icu-77.1.txt'), encoding='utf-8').read().split('\n')
if ret and ret[-1] == '':
    ret.pop()
groups = [((5, 185), 'spec: the current draft (13 DurationFormat Objects)'), ((187, 203), 'spec: CLDR 48 data'),
          ((205, 219), 'coverage: a numbering system\'s own symbols (D21)'), ((221, 224), 'spec: Temporal\'s amendment')]
div = {}
for n, line in enumerate(open(os.path.join(base, 'divergences.txt'), encoding='utf-8').read().split('\n'), 1):
    if not line.strip() or line.startswith('#'):
        continue
    g = next(name for (a, b), name in groups if a <= n <= b)
    div[line.split(' => ')[0]] = (line, n, g)
exp = []
for l in ret:
    k = l.split(' => ')[0]
    exp.append((div[k][0], 'divergences.txt:%d' % div[k][1], div[k][2]) if k in div else (l, 'retained', ''))
def read(path, skip):
    t = open(path, encoding='utf-8').read().split('\n')[skip:]
    t = [x for x in t if not x.startswith('exit=')]
    while t and t[-1] == '':
        t.pop()
    return t
vm = read(os.path.join(rows_dir, 'runs/d-durations.vm.txt'), 0)
vmn = read(os.path.join(rows_dir, 'runs/d-durations.vm-native.txt'), 0)
js = read(os.path.join(rows_dir, 'runs/d-durations.bjs.txt'), 3)
def row(e, j):
    k = e.split(' => ')[0]
    if k.startswith('fractional always'):
        return 'U3'
    if '"minutes":"numeric"' in k:
        return 'U1'
    if 'resolvedOptions' in k:
        return 'U2'
    return '?'
out = open(os.path.join(rows_dir, 'runs/d-durations.classes.tsv'), 'w', encoding='utf-8', newline='\n')
out.write('line\tsource of the expected answer\tdivergence group\tengine\trow\texpected (Broiler.VM\'s, = its run)\tthe engine\'s line\n')
counts = {}
for name, lines in (('Broiler.VM bytecode', vm), ('Broiler.VM native', vmn), ('Broiler.JS', js)):
    n = 0
    for i in range(max(len(lines), len(exp))):
        e = exp[i] if i < len(exp) else ('(none)', '', '')
        l = lines[i] if i < len(lines) else '(none)'
        if l != e[0]:
            n += 1
            r = row(e[0], l)
            counts[(name, e[2] or 'retained', r)] = counts.get((name, e[2] or 'retained', r), 0) + 1
            out.write('%d\t%s\t%s\t%s\t%s\t%s\t%s\n' % (i + 1, e[1], e[2], name, r, e[0], l))
    print('%s: %d lines, %d differ from the expected answer' % (name, len(lines), n))
for k, v in sorted(counts.items()):
    print('  %s | %s | %s: %d' % (k[0], k[1], k[2], v))
# Lines on which Broiler.JS equals Broiler.VM, by source of the expected answer, for the survey's comparison
by = {}
for i, e in enumerate(exp):
    src = e[2] or 'retained'
    same = i < len(js) and js[i] == e[0]
    by.setdefault(src, [0, 0])[0 if same else 1] += 1
for k, v in sorted(by.items()):
    print('  Broiler.JS against the expected answer, %s: %d equal, %d differ' % (k, v[0], v[1]))
