var r;
try { new Function("/*", "*/){"); r = "accepted"; } catch (e) { r = "threw " + e.name; }
print("PR01 " + r);
"PR01 " + r;
