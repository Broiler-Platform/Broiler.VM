// A BigInt literal is a BigInt and never a Number. Until 2026-09-08 the wide front end read `1n`
// as the Number 1 - `typeof 1n` answered "number" and `9007199254740993n` a different integer -
// which is the refusal the parity roadmap's JSP-2 calls lost. The wide manifest admits the type
// now (JSeal B01-B08), and this is its gate's type half: `typeof`, mixing with a Number, the
// equalities, a value past the Number range, `JSON.stringify`, and every literal form (JSC-241).
var rows = [];

function attempt(f) {
  try { return String(f()); }
  catch (e) { return e.constructor.name; }
}

rows.push(typeof 1n, typeof Object(1n));
rows.push(attempt(function () { return 1n + 1; }), attempt(function () { return 1n * 1.5; }),
  attempt(function () { return +1n; }));
rows.push(1n === 1, 1n == 1, 1n < 2, 2n > 1.5, 0n == "", 1n == "1");

var big = 2n ** 64n + 1n;
rows.push(String(big), BigInt(String(big)) === big, 9007199254740993n, Number(9007199254740993n));
rows.push(attempt(function () { return JSON.stringify({ a: 1n }); }));
rows.push(0xFFn, 0o17n, 0b101n, 0x1fffffffffffffn + 2n, -(2n ** 63n));
rows.push(5n / 2n, -5n % 3n, 1n << 70n, BigInt.asIntN(8, 255n), BigInt.asUintN(8, -1n));

rows.join(" / ");
