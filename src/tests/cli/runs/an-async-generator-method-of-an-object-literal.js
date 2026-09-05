// THE POSITION ONE TOKEN AWAY FROM TWO OTHERS. An object literal's `async m()` and its `*m()` are
// each admitted and each reaches a different arm; `async *m()` is neither of them, and the arm that
// decides has to read the `*` before it reads the key. Reading the key first would have made the
// `*` a surprise token, and reading `async` alone would have taken every ordinary async method with
// it. A computed key is here for the same reason: it is the second thing the arm reads.
var key = "computed";

var pump = {
  async *from(start) {
    yield start;
    yield start + 1;
  },

  async *[key]() {
    yield "keyed";
  },
};

async function drain(source) {
  var out = [];

  for await (const value of source) {
    out.push(value);
  }

  return out.join(",");
}

drain(pump.from(7)).then(function (numbers) {
  return drain(pump[key]()).then(function (keyed) {
    print(numbers + ";" + keyed);
  });
});
