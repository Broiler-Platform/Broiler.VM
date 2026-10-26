#!/usr/bin/env python3
"""Three columns: rows-T/work/sbs3.py <rows-dir> <probe>. Like sbs.py, with the witness (runs/<probe>.node.txt)
under each differing label and a mark of which engine it equals (VM, JS, both or neither)."""
import sys, re
rows, name = sys.argv[1], sys.argv[2]
def load(p, skip):
    d = {}; order = []
    for line in open(p, encoding='utf-8').read().splitlines()[skip:]:
        if ' => ' not in line: continue
        k, v = line.split(' => ', 1)
        if k in d: k = k + ' #2'
        d[k] = v; order.append(k)
    return d, order
v, order = load(f'{rows}/runs/{name}.vm.txt', 0)
j, _ = load(f'{rows}/runs/{name}.bjs.txt', 3)
n, _ = load(f'{rows}/runs/{name}.node.txt', 3)
def kind(a):
    m = re.match(r'THROWS (\w+):', a or ''); return m.group(1) if m else a
same = diff = 0; tally = {'VM': 0, 'JS': 0, 'neither': 0}
for k in order:
    a, b, c = v.get(k), j.get(k), n.get(k)
    if a == b or kind(a) == kind(b) and (a or '').startswith('THROWS'):
        same += 1; print('= ' + k + ' => ' + (a if a == b else 'THROWS ' + kind(a)) + ('' if kind(c) == kind(a) else '   [Node: ' + str(c) + ']'))
    else:
        diff += 1
        w = 'VM' if kind(c) == kind(a) else 'JS' if kind(c) == kind(b) else 'neither'
        tally[w] += 1
        print('# ' + k + '\n   VM: ' + str(a) + '\n   JS: ' + str(b) + '\n   Node (' + w + '): ' + str(c))
print(f'-- {same} same, {diff} differ; Node equals VM {tally["VM"]}, JS {tally["JS"]}, neither {tally["neither"]}')
