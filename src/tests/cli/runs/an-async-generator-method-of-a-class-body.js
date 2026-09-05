// THE SAME CONSTRUCT AT MEMBER POSITION, static and instance, with `super` reaching through it. A
// class body's member parse is its own branch and an object literal's is another, so a family
// admitted in one is not thereby admitted in the other - which is why this row stands beside the
// declaration's rather than being covered by it.
class Base {
  label() {
    return "base";
  }
}

class Feed extends Base {
  async *pages() {
    yield super.label();
    yield "instance";
  }

  static async *served() {
    yield "static";
  }
}

async function drain(source) {
  var out = [];

  for await (const value of source) {
    out.push(value);
  }

  return out.join(",");
}

drain(new Feed().pages()).then(function (instance) {
  return drain(Feed.served()).then(function (served) {
    print(instance + ";" + served);
  });
});
