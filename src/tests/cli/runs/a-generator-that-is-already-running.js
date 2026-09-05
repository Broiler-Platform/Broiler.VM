// RE-ENTERING A GENERATOR THAT IS EXECUTING IS A TypeError, not a second walk of one operand stack.
// The generator is completed by the throw escaping its body, so the second half of this file
// asserts the state it lands in rather than only the error it gave.
var self;

function* reentrant() {
  yield self.next();
}

self = reentrant();
var answer;

try {
  self.next();
  answer = "no error";
} catch (e) {
  answer = e.constructor.name;
}

var after = self.next();
answer + " then done=" + after.done;
