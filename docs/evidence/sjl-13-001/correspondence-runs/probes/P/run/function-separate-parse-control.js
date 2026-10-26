var r;
try { var f = new Function("a, b", "return a + b;"); r = "accepted " + f(1, 2); } catch (e) { r = "threw " + e.name; }
print("PR05 " + r);
"PR05 " + r;
