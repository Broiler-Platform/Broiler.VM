# Usage: python3 -I sidebyside.py <runs dir> <probe name> [all]
# Prints each probe line where Broiler.VM's bytecode answer (runs/<name>.vm.txt) and Broiler.JS's
# (runs/<name>.bjs.txt, after its three header lines, without exit=) differ; with "all", every line.
import sys, os
d, name = sys.argv[1], sys.argv[2]
show_all = len(sys.argv) > 3
vm = open(os.path.join(d, name + '.vm.txt'), encoding='utf-8').read().rstrip('\n').split('\n')
js = open(os.path.join(d, name + '.bjs.txt'), encoding='utf-8').read().rstrip('\n').split('\n')[3:]
js = [l for l in js if not l.startswith('exit=')]
n = max(len(vm), len(js))
same = 0
for i in range(n):
    a = vm[i] if i < len(vm) else '(none)'
    b = js[i] if i < len(js) else '(none)'
    if a == b:
        same += 1
        if show_all:
            print('=  ' + a)
    else:
        print('VM ' + a)
        print('JS ' + b)
        print()
print('# %s: %d lines, %d the same, %d differ' % (name, n, same, n - same))
