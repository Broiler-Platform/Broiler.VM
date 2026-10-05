"use strict";
// A call in tail position of strict code takes no frame (the edition's PrepareForTailCall): each
// shape below recurses a hundred thousand times, past the call-depth bound, and returns. The last
// is NOT in tail position - a `try` would catch what the callee throws - so it still reaches the
// bound and throws a RangeError. The comparison engine implements no tail calls, so these values
// are the edition's.
function plain(n) { if (n === 0) { return "plain"; } return plain(n - 1); }
function conditional(n) { return n === 0 ? "conditional" : conditional(n - 1); }
function logical(n) { return n === 0 || logical(n - 1); }
function lastOfComma(n) { return n === 0 ? "comma" : (0, lastOfComma(n - 1)); }
function inFinally(n) { if (n === 0) { return "finally"; } try { } finally { return inFinally(n - 1); } }
function inBlock(n) { if (n === 0) { return "block"; } { let m = n - 1; return inBlock(m); } }
function spread(n) { if (n === 0) { return "spread"; } return spread(...[n - 1]); }
var method = { step(n) { return n === 0 ? "method" : this.step(n - 1); } };
function inTry(n) { if (n === 0) { return "try"; } try { return inTry(n - 1); } catch (e) { throw e; } }

var deep = 100000;
var answers = [plain(deep), conditional(deep), logical(deep), lastOfComma(deep), inFinally(deep),
  inBlock(deep), spread(deep), method.step(deep)];

try {
  answers.push(inTry(deep));
} catch (e) {
  answers.push(e.name);
}

print(answers.join(" / "));
