// GeneratorFunction, AsyncFunction and AsyncGeneratorFunction build from source where Function
// does: this host admits broiler.javascript.dynamic, so all four take the same door. Until
// 2026-10-03 the three refused in every realm with the dynamic surface's reason, which was not
// true of a realm that had admitted it (JSP-10, JSC-240). An async body runs up to its first
// `await` when it is called, and an async generator's up to its first `yield` at `next`, so what
// each body did is visible before any job runs.
var GeneratorFunction = Object.getPrototypeOf(function* () {}).constructor;
var AsyncFunction = Object.getPrototypeOf(async function () {}).constructor;
var AsyncGeneratorFunction = Object.getPrototypeOf(async function* () {}).constructor;
var seen = [];
var g = GeneratorFunction("a", "b", "yield a; yield b;");
var a = new AsyncFunction("x", "seen.push('async ' + x); return await x + 1;");
var ag = AsyncGeneratorFunction("seen.push('async generator'); yield 1;");
var promise = a(41);
ag().next();

[
  [...g(1, 2)].join("+"),
  seen.join(", "),
  Object.prototype.toString.call(promise),
  Object.getPrototypeOf(g) === GeneratorFunction.prototype &&
    Object.getPrototypeOf(a) === AsyncFunction.prototype &&
    Object.getPrototypeOf(ag) === AsyncGeneratorFunction.prototype,
  g.name + "," + a.name + "," + ag.name,
].join(" / ");
