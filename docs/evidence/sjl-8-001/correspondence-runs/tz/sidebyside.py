# Prints two engines' probe outputs line by line, marking the lines that differ with "!!".
import sys
a = open(sys.argv[1], encoding='utf-8').read().strip('\n').split('\n')
b = [l for l in open(sys.argv[2], encoding='utf-8').read().strip('\n').split('\n') if not l.startswith('exit=')]
for x, y in zip(a, b):
    k = x.split(' => ')[0]
    print(('   ' if x == y else '!! ') + k + ' | VM: ' + x.split(' => ', 1)[-1] + ' | BJS: ' + y.split(' => ', 1)[-1])
if len(a) != len(b): print('LENGTH DIFF', len(a), len(b))
