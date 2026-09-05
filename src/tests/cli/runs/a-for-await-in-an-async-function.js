// `for await` IN THE FOUR BODIES THAT MAY HOLD ONE, and the four are not one case tested four
// times: an async function, an async METHOD, an async ARROW and an async GENERATOR each reach the
// head through a different parse and lower it into a unit with a different flag pairing. The arrow
// is the one that was never obvious - it has no `this` and no `new.target` of its own, and the loop
// suspends inside it.
async function fromFunction(source) {
  var out = [];

  for await (const value of source) {
    out.push(value);
  }

  return out.join(",");
}

var holder = {
  async fromMethod(source) {
    var out = [];

    for await (const value of source) {
      out.push(value);
    }

    return out.join(",");
  },
};

var fromArrow = async (source) => {
  var out = [];

  for await (const value of source) {
    out.push(value);
  }

  return out.join(",");
};

async function* fromGenerator(source) {
  for await (const value of source) {
    yield value * 2;
  }
}

fromFunction([1, 2]).then(function (plain) {
  return holder.fromMethod([3, 4]).then(function (method) {
    return fromArrow([5, 6]).then(function (arrow) {
      return fromFunction(fromGenerator([7, 8])).then(function (nested) {
        print([plain, method, arrow, nested].join(";"));
      });
    });
  });
});
