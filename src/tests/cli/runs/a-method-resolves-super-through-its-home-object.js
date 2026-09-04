// THE RECEIVER DOES NOT DECIDE WHERE `super` STARTS. The method is taken off the
// prototype and called against an object that is not an instance of anything in the
// chain; its `super.name()` still reaches the class it was DEFINED under, because a
// method's home object travels with the function rather than with the call. A lowering
// that walked the receiver's prototype would answer "Stranger" here, or recur forever.
class Upper { name() { return "Upper"; } }

class Lower extends Upper {
  name() { return "Lower"; }
  reach() { return super.name(); }
}

class Lowest extends Lower { name() { return "Lowest"; } }

var taken = Lower.prototype.reach;

taken.call(new Lowest()) + " " +
  taken.call({ name: function () { return "Stranger"; } });
