// A WEAK-MAP CHAIN OF A HUNDRED THOUSAND LINKS ENDS (JSC-260): each key's value is the next key, the
// shape of test262's `staging/sm/regress/regress-1507322-deep-weakmap.js` without its `$262.gc`.
// Over the runtime's dependent-handle table a collection of this chain ran for minutes, and no
// allowance can interrupt a collection; the values now live on their keys.
var m = new WeakMap(), head = {};
for (var key = head, i = 0; i < 99999; i++, key = m.get(key)) { m.set(key, {}); }
var links = 0;
for (key = head; key !== undefined; key = m.get(key)) { links++; }
links;
