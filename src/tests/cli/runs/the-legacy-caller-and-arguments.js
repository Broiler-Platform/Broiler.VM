// A sloppy plain function has the legacy `caller` and `arguments`: `caller` answers the sloppy
// function that called its latest running call - looking through direct eval code - and `null`
// when a built-in, a strict function, a generator or global code made the call; `arguments` answers
// that call's arguments object, mapped onto simple parameters, and `null` when it is not running.
// A strict function still has neither, and reading either still throws.
var out = [];
function innermost() { return arguments.callee.caller; }
function nest() { return eval("innermost();"); }
function nest2() { return nest(); }
out.push(nest2() === nest);

function top() { return top.caller; }
out.push([top() === null, [0].map(top)[0] === null, Reflect.apply(top, undefined, []) === null].join(","));
(function strictCaller() { "use strict"; out.push(top() === null); })();
function* gen() { yield top(); }
out.push(gen().next().value === null);

function foo(a, b) { return [foo.arguments.length, foo.arguments[0], foo.caller === wrapper].join(","); }
function wrapper() { return foo(5, undefined); }
out.push(wrapper(), foo.arguments === null && foo.caller === null);

function mapped(x) { x = 9; return mapped.arguments[0]; }
out.push(mapped(1));

var descriptor = Object.getOwnPropertyDescriptor(function plain() {}, "caller");
out.push([typeof descriptor.get, descriptor.enumerable, descriptor.configurable].join(","));

try { (function strict() { "use strict"; }).caller; out.push("read"); } catch (e) { out.push(e.name); }
print(out.join(" / "));
