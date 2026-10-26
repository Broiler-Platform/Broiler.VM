function inStrict(t) { "use strict"; try { return "value " + String(eval(t)); } catch (e) { return e.name + ": " + e.message; } }
function inSloppy(t) { try { return "value " + String(eval(t)); } catch (e) { return e.name + ": " + e.message; } }
function dynamic(b) { try { new Function(b); return "accepted"; } catch (e) { return e.name + ": " + e.message; } }
var lines = [
  "A1 " + inStrict("eval; )"),
  "A2 " + inSloppy("eval; )"),
  "B1 " + inStrict("eval;\n  )"),
  "B2 " + inSloppy("eval;\n  )"),
  "C1 " + inStrict("#!x\n1"),
  "C2 " + inSloppy("#!x\n1"),
  "D1 " + dynamic("a; )")
];
lines.join("\n");
