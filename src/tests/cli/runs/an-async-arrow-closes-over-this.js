// AN ASYNC ARROW IS AN ARROW: it has no `this`, no `new.target` and no `super` of its own, and it
// keeps all three across a suspension. The derived class is the case that needs the box rather than
// the value - a derived constructor's `this` does not exist until `super()` returns, so an arrow
// written there reads a box, and the box has to survive the `await` that follows.
class Base {
  who() {
    return "Base";
  }
}

class Derived extends Base {
  constructor() {
    super();
    this.mark = "derived";
    this.later = async () => {
      await 0;
      return super.who() + "/" + this.mark + "/" + (new.target === undefined);
    };
  }
}

var plain = {
  mark: "plain",
  run: function () {
    var read = async () => {
      await 0;
      return this.mark;
    };

    return read();
  },
};

Promise.all([new Derived().later(), plain.run()]).then(function (v) { print(v.join(" ")); });
