// A name in parentheses is assigned and infers nothing: `(f) = function () {}` leaves the function
// anonymous, because the target is not an identifier reference once it is parenthesised. And a
// rejection reason that is a Symbol is caught as the Symbol: rethrowing it at an `await` once
// rendered it as a string first, which threw a TypeError in its place.
var fn, g, h;
(fn) = function () {};
g = function () {};
(h) ??= class {};
var k = (function () {});

var names = JSON.stringify([fn.name, g.name, h.name, k.name]);

var reasons = [];
var symbol = Symbol("why");

async function reject() {
  try {
    await Promise.reject(symbol);
  } catch (e) {
    reasons.push(e === symbol);
  }
}

reject().then(function () {
  print(names + " " + reasons.join());
});
