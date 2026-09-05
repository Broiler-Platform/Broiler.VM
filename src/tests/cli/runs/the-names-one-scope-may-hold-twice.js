// THE OTHER DIRECTION OF EVERY RULE THE REDECLARATION BUNDLE ADDED, in one file. An early error
// that fires where the language has none turns a working program into a refusal, which is a worse
// failure than the one the bundle was written to fix - so each shape below is one a conforming
// engine RUNS, and this file is the row that goes red the day one of them stops running.
//
// Two `var` declarations of one name are one binding; two sibling blocks are two scopes; a
// function declared twice in one block is Annex B's web-compatibility case and sloppy code keeps
// it; a `var` may reach through a simple `catch` parameter; a loop body is a scope of its own; and
// a parameter may be shadowed one block inside the body it belongs to.
var parts = [];

{ var twice; var twice = 1; parts.push(twice); }
{ let sibling = "a"; parts.push(sibling); }
{ let sibling = "b"; parts.push(sibling); }
{ function annexB() { return "first"; } function annexB() { return "second"; } parts.push(annexB()); }

try { throw 1; } catch (e) { var e = "reached"; parts.push(e); }

for (let i = 0; i < 1; i++) { let i = "inner"; parts.push(i); }

parts.push((function (a) { { let a = "shadow"; return a; } })("outer"));
parts.push((function (a, a) { return a; })(1, "last-wins"));

switch (1) { case 1: let clause = "one-scope"; parts.push(clause); break; }

parts.join("/");
