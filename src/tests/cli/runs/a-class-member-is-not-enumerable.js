// A class's methods are not enumerable and its `constructor` back-link is not either, so
// `for…in` over an instance reaches the instance's own fields and nothing else. The
// prototype's `prototype` property is not writable, which is the other half of the same
// claim and the half an ordinary function does not share.
class Recorded {
  constructor() { this.own = 1; }
  method() { return 1; }
  get accessor() { return 2; }
}

var seen = [];

for (var key in new Recorded()) {
  seen.push(key);
}

var method = Object.getOwnPropertyDescriptor(Recorded.prototype, "method");
var back = Object.getOwnPropertyDescriptor(Recorded.prototype, "constructor");
var proto = Object.getOwnPropertyDescriptor(Recorded, "prototype");

seen.join(",") + " " +
  method.enumerable + method.writable + method.configurable + " " +
  back.enumerable + " " +
  proto.writable + proto.enumerable + proto.configurable;
