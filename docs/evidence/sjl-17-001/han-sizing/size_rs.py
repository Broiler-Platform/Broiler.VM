# Size radical-stroke data for Han (item 3 of SJL-17.3). Inputs: Unihan_IRGSources.txt, PropList.txt, vectors dir.
import sys, re, os, glob, gzip
unihan, proplist, vecdir = sys.argv[1:4]
uni = set()
for line in open(proplist, encoding='utf-8'):
    line = line.split('#')[0].strip()
    if not line: continue
    rng, prop = [x.strip() for x in line.split(';')]
    if prop != 'Unified_Ideograph': continue
    if '..' in rng:
        a, b = rng.split('..'); uni.update(range(int(a,16), int(b,16)+1))
    else: uni.add(int(rng,16))
rs = {}
for line in open(unihan, encoding='utf-8'):
    if '\tkRSUnicode\t' not in line: continue
    cp, _, val = line.rstrip('\n').split('\t')
    first = val.split()[0]
    m = re.match(r"(\d+)('*)\.(-?\d+)$", first)
    rs[int(cp[2:],16)] = (int(m.group(1)), len(m.group(2)), int(m.group(3)))
missing = [c for c in uni if c not in rs]
print('unified ideographs', len(uni), 'with kRSUnicode', sum(1 for c in uni if c in rs), 'missing', len(missing))
def block(c):
    core = 0x4E00 <= c <= 0x9FFF or 0xF900 <= c <= 0xFAFF
    return 0 if core else 1
order = sorted(uni, key=lambda c: (rs[c], block(c), c))
# runs of consecutive code points in RS order
runs = []
for c in order:
    if runs and c == runs[-1][0] + runs[-1][1]: runs[-1][1] += 1
    else: runs.append([c, 1])
print('runs over all', len(runs), 'bytes at 5 per run (cp24,len16):', len(runs)*5)
print('per code point table, 3 bytes each:', len(uni)*3, '; rank 17 bits packed:', (len(uni)*17+7)//8)
# implicit-by-block order vs RS order: how many runs where implicit primary sequence differs
# vectors reach
reach = set()
for f in glob.glob(os.path.join(vecdir, '*.vectors')):
    for line in open(f, encoding='utf-8'):
        if not line.startswith(('in:', 'out:', 'script:')): continue
        s = line
        cps = []
        for m in re.finditer(r'\\u([0-9A-Fa-f]{4})', s):
            cps.append(int(m.group(1),16))
        # combine surrogate pairs
        i=0
        while i < len(cps):
            c=cps[i]
            if 0xD800<=c<=0xDBFF and i+1<len(cps) and 0xDC00<=cps[i+1]<=0xDFFF:
                c=0x10000+((c-0xD800)<<10)+(cps[i+1]-0xDC00); i+=1
            if c in uni: reach.add(c)
            i+=1
        for ch in s:
            if ord(ch) in uni: reach.add(ord(ch))
print('ideographs the vectors reach', len(reach), sorted('U+%04X' % c for c in reach))
print('reach table at 6 bytes each (cp24 + rank24):', len(reach)*6)
blocks = {}
for c in uni:
    for lo,hi,name in [(0x3400,0x4DBF,'A'),(0x4E00,0x9FFF,'URO'),(0xF900,0xFAFF,'CJKCompat'),(0x20000,0x2A6DF,'B'),(0x2A700,0x2B73F,'C'),(0x2B740,0x2B81F,'D'),(0x2B820,0x2CEAF,'E'),(0x2CEB0,0x2EBEF,'F'),(0x2EBF0,0x2EE5F,'I'),(0x30000,0x3134F,'G'),(0x31350,0x323AF,'H'),(0x323B0,0x3347F,'J')]:
        if lo<=c<=hi: blocks[name]=blocks.get(name,0)+1
print('by block', blocks)
