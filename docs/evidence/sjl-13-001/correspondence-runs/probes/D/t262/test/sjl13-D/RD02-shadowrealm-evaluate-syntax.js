/*---
description: sjl13 D reader probe RD02 (row D62). ShadowRealm.prototype.evaluate of a text that is not a Script throws a SyntaxError of the calling realm.
esid: sec-performshadowrealmeval
flags: [raw]
---*/
var r = new ShadowRealm();
var kind = "none";
try { r.evaluate("1 +"); } catch (e) { kind = e.constructor === SyntaxError ? "SyntaxError" : String(e); }
if (kind !== "SyntaxError") { throw new Error("RD02 " + kind); }
