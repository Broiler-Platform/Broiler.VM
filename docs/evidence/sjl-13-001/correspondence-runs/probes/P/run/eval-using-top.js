var r;
try { eval("using x = null;"); r = "accepted"; } catch (e) { r = "threw " + e.name; }
print("PR04 " + r);
"PR04 " + r;
