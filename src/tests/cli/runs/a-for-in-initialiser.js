// ANNEX B'S `for (var x = 1 in o)` EVALUATES ITS INITIALISER ONCE, BEFORE THE OBJECT (JSP-7, JSC-239).
// The form was admitted and the value dropped, so `x` was `undefined` after a loop that ran no
// iteration; an anonymous function takes the name.
var effects = 0;
var stored;

for (var a = (++effects, -1) in stored = a, { p: 0, q: 1 }) {
}

for (var b = 0 in {}) {
}

for (var c = function () {} in {}) {
}

[effects, stored, a, b, c.name].join(' / ');
