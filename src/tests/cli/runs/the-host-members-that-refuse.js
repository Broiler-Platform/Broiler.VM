// The host members this realm defines and cannot perform are present and refuse, by a decision
// roadmap section 13 states with its reasons: `read` because a shell probe reads the name without
// calling it, and `$262`'s members because the conformance suite's own contract requires them
// defined and `gc` to throw. Each refusal says what is true of this realm (JSP-10, JSC-240).
var rows = ["typeof read=" + typeof read + " typeof $262.gc=" + typeof $262.gc];

function ask(name, f) {
  try { f(); rows.push(name + ": answered"); }
  catch (e) { rows.push(name + ": " + e.name + ": " + e.message); }
}

ask("read", function () { read("x"); });
ask("gc", function () { $262.gc(); });
ask("createRealm", function () { $262.createRealm(); });
ask("detachArrayBuffer", function () { $262.detachArrayBuffer(new ArrayBuffer(1)); });
ask("evalScript", function () { $262.evalScript("1"); });
ask("agent.start", function () { $262.agent.start(""); });

rows.join("\n");
