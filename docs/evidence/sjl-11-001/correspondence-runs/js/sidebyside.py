#!/usr/bin/env python3
"""SJL-11.1's comparison of one probe's answers in the two engines (run with python3 -I).

Usage: sidebyside.py <name>   (from correspondence-runs/js/)

Reads ../vm/<name>.vm.txt and <name>.bjs.txt (and <name>.tr-TR.bjs.txt and <name>.lt-LT.bjs.txt
when present), joins the lines on their label (the text before " => "), and writes
compare/<name>.compare.txt. Each line starts with two marks:
  the first, Broiler.VM against Broiler.JS under the invariant culture: blank where the answers
  are equal, "~" where both throw the same constructor with different messages (a message is each
  engine's own), "!" for any other difference;
  the second, Broiler.JS across cultures: blank, or "T" (tr-TR differs), "L" (lt-LT differs),
  "TL" (both differ) from the invariant run.
A probe Broiler.VM ended before printing (an exhaustion or a stop) has one VM line, the verdict.
Lines starting with "#" and the shell's "exit=" line are not compared; the exit is printed last.
A value or line longer than 400 characters is cut there, with its full length noted; the run files
hold it whole (or gzip-compressed, for a file over 100 KB).
"""
import os, re, sys

def read(path):
    if not os.path.exists(path):
        return None
    lines = open(path, encoding='utf-8', errors='replace').read().splitlines()
    return [l for l in lines if not l.startswith('#')]

def split(lines):
    out, extra = {}, []
    order = []
    for l in lines:
        if ' => ' in l:
            k, v = l.split(' => ', 1)
            if k in out:
                k = k + ' #' + str(sum(1 for o in order if o.startswith(k)))
            out[k] = v
            order.append(k)
        else:
            extra.append(l)
    return out, order, extra

def cut(v):
    if v is None or len(v) <= 400:
        return v
    return v[:400] + ' ... (cut; %d characters in all)' % len(v)

def kind(v):
    m = re.match(r'THROWS (\w+)', v)
    return m.group(1) if m else None

name = sys.argv[1]
vm = read(os.path.join('..', 'vm', name + '.vm.txt')) or []
js = read(name + '.bjs.txt') or []
tr = read(name + '.tr-TR.bjs.txt')
lt = read(name + '.lt-LT.bjs.txt')
vmd, _, vmx = split(vm)
jsd, order, jsx = split(js)
trd = split(tr)[0] if tr is not None else None
ltd = split(lt)[0] if lt is not None else None
os.makedirs('compare', exist_ok=True)
with open(os.path.join('compare', name + '.compare.txt'), 'w', encoding='utf-8') as f:
    f.write('# %s: marks VM-vs-JS, JS culture; then the label, the VM answer, the JS answers\n' % name)
    if vmx:
        f.write('# VM ended before printing: %s\n' % cut(' '.join(vmx)))
    for k in order:
        a, b = vmd.get(k), jsd[k]
        if a is None:
            m1 = '?'
        elif a == b:
            m1 = ' '
        elif kind(a) and kind(a) == kind(b):
            m1 = '~'
        else:
            m1 = '!'
        m2 = ''
        if trd is not None and trd.get(k) != b:
            m2 += 'T'
        if ltd is not None and ltd.get(k) != b:
            m2 += 'L'
        line = '%s %-2s %s\n    VM: %s\n    JS: %s\n' % (m1, m2, cut(k), cut(a) if a is not None else '(no line)', cut(b))
        if trd is not None:
            line += '    JS tr-TR: %s\n    JS lt-LT: %s\n' % (cut(trd.get(k)), cut(ltd.get(k)))
        f.write(line)
    for l in jsx[:40]:
        f.write('# JS: %s\n' % cut(l))
    if len(jsx) > 40:
        f.write('# JS: ... (%d more lines in the run file)\n' % (len(jsx) - 40))
