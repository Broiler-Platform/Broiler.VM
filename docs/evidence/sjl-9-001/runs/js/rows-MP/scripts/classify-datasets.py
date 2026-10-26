# Usage: python3 -I classify-datasets.py <rows-dir> <Broiler.VM checkout at cd1928d>
# Compares each engine's lines of the d-plurals and d-numbers probes (runs/d-<name>.{vm,bjs}.txt[.gz]) with
# Broiler.VM's retained answer, src/tests/cldr/<name>/<name>.icu-77.1.txt, after its divergences.txt is applied
# (a divergence line replaces the retained line whose text before " => " is the same), prints the counts, and
# writes runs/d-<name>.classes.tsv: each Broiler.JS line that differs, with the row of sub-area b it falls in.
import gzip, os, re, sys
rows_dir, vm_root = sys.argv[1], sys.argv[2]

def read_lines(path, skip):
    if not os.path.exists(path):
        path += '.gz'
    opener = gzip.open if path.endswith('.gz') else open
    with opener(path, 'rt', encoding='utf-8') as f:
        t = f.read().split('\n')
    t = t[skip:]
    t = [x for x in t if not x.startswith('exit=')]
    while t and t[-1] == '':
        t.pop()
    return t

def expected(name):
    base = os.path.join(vm_root, 'src/tests/cldr', name)
    ret = open(os.path.join(base, name + '.icu-77.1.txt'), encoding='utf-8').read().split('\n')
    if ret and ret[-1] == '':
        ret.pop()
    div, hdr, prev_blank, header = {}, {}, True, ''
    for line in open(os.path.join(base, 'divergences.txt'), encoding='utf-8').read().split('\n'):
        if line.startswith('#'):
            header = line[1:].strip() if prev_blank else header + ' ' + line[1:].strip()
            prev_blank = False
            continue
        prev_blank = line.strip() == ''
        if prev_blank:
            continue
        key = line.split(' => ')[0]
        div[key], hdr[key] = line, header
    out = []
    for l in ret:
        k = l.split(' => ')[0]
        out.append((div[k], 'divergence', hdr[k]) if k in div else (l, 'retained', ''))
    return out

def plural_row(src, exp, header):
    k = exp.split(' => ')[0]
    if re.search(r' select(Range)? .*\dn\b', k) or k.endswith(' 1n'):
        return 'P2'
    if ' resolvedOptions' in k:
        return 'P6'
    if ' selectRange ' in k:
        return 'P5a' if header.startswith('ICU 77.1 resolves every range') else 'P5b'
    if ' select ' in k:
        return 'P1'
    return '?'

M1_VALUES = {'1.005', '12.345', '999.995', '1234.5678', '-1234.5678', '1.23e-7', '5e-324'}
C_CASES = {25, 26, 31, 34, 35, 48, 52, 53, 54}   # every value line differs for sub-area c's reasons
DIGIT_KEYS = {'minimumFractionDigits', 'maximumFractionDigits', 'minimumSignificantDigits', 'maximumSignificantDigits', 'roundingPriority'}

def number_row(exp, js):
    case = int(exp.split(' ', 1)[0])
    if ' range ' in exp.split(' => ')[0]:
        return 'c'
    if ' resolved ' in exp:
        a = dict(x.split('=', 1) for x in exp.split(' resolved ', 1)[1].split(' '))
        b = dict(x.split('=', 1) for x in js.split(' resolved ', 1)[1].split(' '))
        diff = {k for k in set(a) | set(b) if a.get(k) != b.get(k)}
        return 'M5' if diff and diff <= DIGIT_KEYS else 'c'
    if case in C_CASES:
        return 'c'
    value = re.match(r'(\d+) (\S+) (\{.*?\}) (.*?) => ', exp).group(4)
    if case == 21:
        return 'M7'
    if case in (22, 46):
        return 'M5'
    if value == '"1e-400"':
        return 'M2b'
    if value in M1_VALUES:
        return 'M1'
    return 'c'

for name in ('plurals', 'numbers'):
    exp = expected(name)
    vm = read_lines(os.path.join(rows_dir, 'runs', 'd-%s.vm.txt' % name), 0)
    js = read_lines(os.path.join(rows_dir, 'runs', 'd-%s.bjs.txt' % name), 3)
    vm_diff = sum(1 for i in range(max(len(vm), len(exp))) if i >= len(vm) or i >= len(exp) or vm[i] != exp[i][0])
    js_diff = [i for i in range(max(len(js), len(exp))) if i >= len(js) or i >= len(exp) or js[i] != exp[i][0]]
    counts = {}
    rows = []
    for i in js_diff:
        e = exp[i]
        row = plural_row(e[1], e[0], e[2]) if name == 'plurals' else number_row(e[0], js[i])
        counts[row] = counts.get(row, 0) + 1
        rows.append((i + 1, e[1], row, e[0], js[i]))
    print(name, 'expected lines', len(exp), 'Broiler.VM lines differing', vm_diff, 'Broiler.JS lines differing', len(js_diff), 'by row', dict(sorted(counts.items())))
    with open(os.path.join(rows_dir, 'runs', 'd-%s.classes.tsv' % name), 'w', encoding='utf-8', newline='\n') as o:
        if name == 'plurals':
            o.write('line\tsource of the expected answer\trow\texpected (Broiler.VM, = the run)\tBroiler.JS\n')
            for n, s, r, e, j in rows:
                o.write('%d\t%s\t%s\t%s\t%s\n' % (n, s, r, e, j.split(' => ', 1)[-1]))
        else:
            o.write('line\tsource of the expected answer\trow\texpected (Broiler.VM, = the run), to 240 characters\tBroiler.JS, to 240 characters\n')
            for n, s, r, e, j in rows:
                if r != 'c':
                    o.write('%d\t%s\t%s\t%s\t%s\n' % (n, s, r, e[:240], j[:240]))
            o.write('# every other differing line is sub-area c\'s (formatRange, currency names and code spacing, compact notation); the full lines are in the run files\n')
