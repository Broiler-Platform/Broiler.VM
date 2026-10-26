var r;
try { var f = new Function("", "}); (function(){"); r = "accepted " + typeof f; } catch (e) { r = "threw " + e.name; }
print("PR02 " + r);
"PR02 " + r;
