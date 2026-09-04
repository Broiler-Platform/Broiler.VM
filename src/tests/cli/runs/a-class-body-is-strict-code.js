// This file declares no `"use strict"` anywhere, and the class body is strict all the
// same. Two things say so and neither could say it in sloppy code: an ordinary function
// called with no receiver sees `undefined` rather than the global object, and writing to
// a frozen object throws rather than being ignored.
class Probe {
  receiver() { return String((function () { return this; })()); }

  frozen() {
    var sealed = Object.freeze({ x: 1 });

    try {
      sealed.x = 2;
      return "silent";
    } catch (e) {
      return e.name;
    }
  }
}

var probe = new Probe();
probe.receiver() + " " + probe.frozen();
