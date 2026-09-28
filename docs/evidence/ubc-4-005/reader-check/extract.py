#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# THE READER CHECK'S FIRST HALF: every text module the harness's --spec reader encodes, cut out of the pinned
# scripts by an independent reading of their lists, numbered as the reader numbers commands, as base64 of the
# exact source bytes. compare.js encodes each with wabt and compares it with the reader's --encode-to output.
#
#   python3 extract.py <test/core directory> <modules.json>
import json, os, sys
root, out = sys.argv[1], sys.argv[2]
def forms(b):
    i=0; n=len(b); res=[]
    def skip(i):
        while i<n:
            c=b[i]
            if c in b' \t\r\n' or c<=0x20: i+=1
            elif b[i:i+2]==b';;':
                while i<n and b[i]!=0x0a: i+=1
            elif b[i:i+2]==b'(;':
                d=0
                while i<n:
                    if b[i:i+2]==b'(;': d+=1; i+=2
                    elif b[i:i+2]==b';)':
                        d-=1; i+=2
                        if d==0: break
                    else: i+=1
            else: return i
        return i
    def node(i):
        # returns (start,end,children) for a list; atoms as (start,end,None)
        if b[i]==0x28:
            s=i; i+=1; ch=[]
            while True:
                i=skip(i)
                if b[i]==0x29: return (s,i+1,ch), i+1
                c,i=node(i); ch.append(c)
        if b[i]==0x22:
            s=i; i+=1
            while b[i]!=0x22:
                if b[i]==0x5c: i+=1
                i+=1
            return (s,i+1,None), i+1
        s=i
        while i<n and b[i] not in b' \t\r\n();"' and b[i]>0x20: i+=1
        return (s,i,None), i
    while True:
        i=skip(i)
        if i>=n: return res
        f,i=node(i); res.append(f)
def text(b,f): return b[f[0]:f[1]]
def head(b,f): return text(b,f[2][0]).decode() if f[2] and f[2][0][2] is None else None
items=[]
for name in sorted(os.listdir(root)):
    if not name.endswith('.wast'): continue
    b=open(os.path.join(root,name),'rb').read()
    for k,f in enumerate(forms(b),1):
        h=head(b,f); m=None
        if h=='module': m=f
        elif h in ('assert_malformed','assert_invalid','assert_unlinkable','assert_trap') and len(f[2])>1 and f[2][1][2] is not None and head(b,f[2][1])=='module': m=f[2][1]
        if m is None: continue
        words=[text(b,c) for c in m[2] if c[2] is None]
        if b'binary' in words or b'quote' in words: continue
        import base64; items.append({'file':name,'ordinal':k,'b64':base64.b64encode(text(b,m)).decode()})
json.dump(items,open(out,'w'))
print(len(items),'modules')
