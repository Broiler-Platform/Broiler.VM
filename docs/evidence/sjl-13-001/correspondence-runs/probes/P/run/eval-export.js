var r;
try { eval("export {};"); r = "accepted"; } catch (e) { r = "threw " + e.name; }
print("PR03 " + r);
"PR03 " + r;
