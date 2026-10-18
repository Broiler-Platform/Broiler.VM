# Compares a retained Temporal program's answers in both engines with the reference polyfill's (SJL-8.1's
# compare_retained.py, generalised): Broiler.VM against the polyfill with divergences.txt applied, then Broiler.JS
# against Broiler.VM, line by line, and lists each line Broiler.JS answers otherwise.
# Usage: compare-retained.py <run name> <polyfill answers> <divergences.txt>
import sys
name, poly_path, div_path = sys.argv[1], sys.argv[2], sys.argv[3]
vm = open(f'{name}.vm.txt', encoding='utf-8').read().rstrip('\n').split('\n')
js = [l for l in open(f'{name}.bjs.txt', encoding='utf-8').read().rstrip('\n').split('\n') if not l.startswith('exit=')]
poly = open(poly_path, encoding='utf-8').read().rstrip('\n').split('\n')
div = {}
for l in open(div_path, encoding='utf-8'):
    l = l.rstrip('\n')
    if l and not l.startswith('#') and ' => ' in l: div[l.split(' => ')[0]] = l
key = lambda l: l.split(' => ')[0]
print(f'lines: Broiler.VM {len(vm)}, Broiler.JS {len(js)}, polyfill {len(poly)}, divergences named {len(div)}')
vm_off = [(p, v) for p, v in zip(poly, vm) if div.get(key(p), p) != v]
print(f'Broiler.VM lines that are neither the polyfill\'s nor a named divergence: {len(vm_off)}')
for p, v in vm_off: print(f'  VM {v}\n  PF {p}')
js_off = [(i, v, j, p) for i, (v, j, p) in enumerate(zip(vm, js, poly)) if v != j]
print(f'Broiler.JS lines that differ from Broiler.VM: {len(js_off)}')
for i, v, j, p in js_off:
    print(f'  #{i + 1}\n    VM  {v}\n    BJS {j}\n    PF  {p}')
