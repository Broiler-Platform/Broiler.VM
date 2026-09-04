// WHAT GOES ON THE CHAIN IS `ToObject` OF THE OPERAND, so a String puts a String wrapper there and
// `length` resolves inside the body; `null` and `undefined` are the `TypeError` the coercion
// already throws, which is what makes `with (maybe)` a guarded construct rather than a silent one.
var answers = [];

with ("abc") {
  answers.push(length, charAt(1), toUpperCase());
}

with (42) {
  answers.push(toFixed(2));
}

with (new Number(5)) {
  answers.push(valueOf() + 1);
}

with (true) {
  answers.push(String(valueOf()));
}

with ([10, 20, 30]) {
  answers.push(length, join("-"), indexOf(20));
}

// A METHOD FOUND ON THE CHAIN IS CALLED AGAINST THE OBJECT THE `with` NAMED, so `push` grows the
// Array this statement was given rather than being called against nothing.
var grown = [1];
with (grown) {
  push(2, 3);
}

answers.push(grown.join(","));

with (Math) {
  answers.push(floor(PI), max(1, 2));
}

// AND THE TWO THAT THROW. Each is caught here so that the file goes on to print, which is what
// makes this a row about the coercion rather than a row about the exit code.
try {
  with (null) {
    answers.push("unreachable");
  }
} catch (refused) {
  answers.push(refused.name);
}

try {
  with (undefined) {
    answers.push("unreachable");
  }
} catch (refused) {
  answers.push(refused.name);
}

// A FUNCTION IS AN OBJECT AND BINDS WHAT AN OBJECT BINDS.
with (function named() { return 1; }) {
  answers.push(name, length, typeof call);
}

print(answers.join(" "));
