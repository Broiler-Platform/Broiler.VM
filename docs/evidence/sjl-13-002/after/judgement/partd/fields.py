"""Crude read of field and property names per file of the compiler assembly at two commits (files that stay only)."""
import re, subprocess, sys
repo, a, b = sys.argv[1:4]
P='src/Broiler.VM.Profile.JavaScript.Compiler/'
def files(c): return [f for f in subprocess.run(['git','-C',repo,'ls-tree','-r','--name-only',c,P],capture_output=True,text=True).stdout.split() if f.endswith('.cs') and 'JsSyntax.cs' not in f and 'JsFrontendAdapter' not in f]
pat = re.compile(r'^\s*(?:(?:private|internal|public|protected|static|readonly|const|required|volatile|new)\s+)+[\w<>,\.\?\[\] ]+?\s+(\w+)\s*(?:=(?!>)|;|\{\s*(?:get|init|set))')
def names(c):
    out=set()
    for f in files(c):
        t=subprocess.run(['git','-C',repo,'show',f'{c}:{f}'],capture_output=True,text=True).stdout
        for l in t.splitlines():
            m=pat.match(l)
            if m: out.add((f,m.group(1)))
    return out
na, nb = names(a), names(b)
print('field/property-like declarations, base', len(na), 'after', len(nb))
for f,n in sorted(na-nb):
    print('gone', f, n)
