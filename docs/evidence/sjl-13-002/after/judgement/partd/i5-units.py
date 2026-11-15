"""Rule I5's register at the after commit, read against the units both commits' generated assurance.manifest.json list
for the compiler assembly, each unit read by its name without its parameter list, as rule I5 reads declared names: every unit of the five deleted files at the base is covered by its type's row; every unit of a
type the after commit still declares that the base lists and the after commit does not is registered by member.
Reads git only. usage: i5-units.py <repo> <base> <after>"""
import json, subprocess, sys, collections, re
repo, base, after = sys.argv[1:4]
def show(c, p): return subprocess.run(['git', '-C', repo, 'show', f'{c}:{p}'], capture_output=True, text=True, check=True).stdout
P = 'src/Broiler.VM.Profile.JavaScript.Compiler/'
DEL = [P + f for f in ('JsSyntax.cs', 'JsFrontendAdapter.cs', 'JsFrontendAdapter.Expressions.cs', 'JsFrontendAdapter.Functions.cs', 'JsFrontendAdapter.Modules.cs')]
mb, ma = json.loads(show(base, 'assurance.manifest.json')), json.loads(show(after, 'assurance.manifest.json'))
reg = json.loads(show(after, 'src/tests/Broiler.VM.Architecture.Tests/retired-members.json'))['retired']
keys = {r['member']: r for r in reg}
step2 = [r for r in reg if r['milestone'] == 'SJL-13.3']
def plain(n): return re.sub(r'\(.*\)$', '', n)
ub = [dict(u, name=plain(u['name'])) for u in mb['units'] if u['file'].startswith(P)]
ua = [dict(u, name=plain(u['name'])) for u in ma['units'] if u['file'].startswith(P)]
after_names = {u['name'] for u in ua}
del_units = [u for u in ub if u['file'] in DEL]
# the types declared in the deleted files: units whose name has no unit of the same file as a proper prefix
names = sorted({u['name'] for u in del_units})
types = sorted(n for n in names if not any(n.startswith(m + '.') for m in names if m != n))
print('deleted files at the base: units', len(del_units), 'distinct names', len(names), 'outermost names (types declared directly in a namespace)', len(types))
missing_types = [t for t in types if t not in keys]
print('outermost names of the deleted files with no register row:', missing_types or 'none')
print('of them declared at the after commit (any unit of that name):', [t for t in types if t in after_names] or 'none')
uncovered = [n for n in names if not any(n == t or n.startswith(t + '.') for t in types if t in keys)]
print('units of the deleted files not under a registered type row:', uncovered or 'none')
# members of types that stay: units at the base, outside the deleted files, absent at the after commit
gone = sorted({u['name'] for u in ub if u['file'] not in DEL} - after_names)
print('units of files that stay, listed at the base and not after:', len(gone))
for n in gone: print('  ', n, 'REGISTERED' if n in keys else 'not registered')
type_rows = [r['member'] for r in step2 if r['member'] in types]
member_rows = [r['member'] for r in step2 if r['member'] not in types]
print('SJL-13.3 rows: %d, of which type rows of the deleted files %d, other rows %d' % (len(step2), len(type_rows), len(member_rows)))
step1 = 'not checked'
for m in member_rows:
    st = 'declared at the after commit' if m in after_names else 'not declared after'
    print('   row', m, '|', st, '| listed at base' if m in {u["name"] for u in ub} else '| not a base unit')
