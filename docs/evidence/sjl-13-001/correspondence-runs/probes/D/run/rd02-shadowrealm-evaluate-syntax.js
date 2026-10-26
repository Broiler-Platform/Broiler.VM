var r = new ShadowRealm();
var kind = "none";
try { r.evaluate("1 +"); } catch (e) { kind = e.constructor === SyntaxError ? "SyntaxError" : String(e); }
print("RD02 " + kind);
"RD02 " + kind;
