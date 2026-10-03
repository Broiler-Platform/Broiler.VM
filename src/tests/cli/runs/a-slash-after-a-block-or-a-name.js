// A `/` after a `)` or a `}` opens a regular expression when the bracket ended a statement's head,
// a block or a declaration's body, and divides when it ended a value: an object literal, a
// function or class expression. `get`, `set`, `async`, `static` and `let` before a `/` are names,
// and `of` is the keyword only after a for head's binding. Until 2026-10-03 every `)` and `}` was
// read as ending a value and those five names as keywords, so `{}/1/` and `get / 2` were refused
// (JSC-245).
var r = [];
var get = 6, set = 6, async = 6, of = 6, g = 2;
r.push(get / g / 3, set / g / 1, async / g / 1, of / g / 1);
r.push(String({} / 2), String(function () {} / 2), String(class {} / 2));
var a = 1;
if (a) /b/.test("b") && r.push("if");
var w = 0;
while (w++ < 1) /x/.test("x") && r.push("while");
r.push(String(eval("{}/1/;")), String(eval("function fn() {}/2/;")), String(eval("class C {}/3/g;")));
r.push(eval("async function af() {}/4/.source"), eval("var q = 1\n{}\n/5/.source"));
for (var m of /a/g[Symbol.split]("bab")) r.push(m);
var o = { f: function () {} / 1, n: 3 };
r.push(String(o.f), o.n, String(true ? {} / 1 : 0));
r.join(" ")
