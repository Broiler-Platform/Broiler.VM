#!/usr/bin/env python3
"""Side by side of one probe's lines in Broiler.VM (runs/<name>.vm.txt) and Broiler.JS (runs/<name>.bjs.txt).

Usage: sidebyside.py <rows-dir> <name> [--all] [--vmtag vm] [--jstag bjs]
Prints, per label, '  ' when both engines answer alike, '~ ' when both throw the same constructor
(each engine's own message), '! ' otherwise, then the label and both answers.
"""
import re
import sys


def lines_of(path, js):
    out = []
    with open(path, encoding='utf-8', errors='replace') as f:
        for line in f:
            line = line.rstrip('\n')
            if line.startswith('# ') or line.startswith('exit='):
                continue
            out.append(line)
    return out


def split(line):
    i = line.find(' => ')
    return (line[:i], line[i + 4:]) if i >= 0 else (line, '')


def main():
    rows, name = sys.argv[1], sys.argv[2]
    show_all = '--all' in sys.argv
    vmtag = sys.argv[sys.argv.index('--vmtag') + 1] if '--vmtag' in sys.argv else 'vm'
    jstag = sys.argv[sys.argv.index('--jstag') + 1] if '--jstag' in sys.argv else 'bjs'
    vm = lines_of('%s/runs/%s.%s.txt' % (rows, name, vmtag), False)
    js = lines_of('%s/runs/%s.%s.txt' % (rows, name, jstag), True)
    jmap = {}
    for l in js:
        k, v = split(l)
        jmap.setdefault(k, []).append(v)
    same = diff = 0
    for l in vm:
        k, v = split(l)
        jv = jmap.get(k, ['(no line)']).pop(0) if jmap.get(k) else '(no line)'
        ev = re.match(r'THROWS (\w+):', v)
        ej = re.match(r'THROWS (\w+):', jv)
        if v == jv:
            mark = '  '
        elif ev and ej and ev.group(1) == ej.group(1):
            mark = '~ '
        else:
            mark = '! '
        if mark != '! ':
            same += 1
        else:
            diff += 1
        if show_all or mark == '! ':
            if mark != '! ':
                print('%s%s => %s' % (mark, k, v))
            else:
                print('%s%s\n     VM: %s\n     JS: %s' % (mark, k, v, jv))
    for k, vs in jmap.items():
        for v in vs:
            print('? JS only: %s => %s' % (k, v))
    print('# %s: alike %d, differ %d' % (name, same, diff))


main()
