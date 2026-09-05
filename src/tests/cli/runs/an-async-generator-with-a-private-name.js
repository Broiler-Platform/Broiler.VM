// THE POSITION WHERE TWO FAMILIES MEET: a member whose KEY is a private name and whose KIND is an
// async generator. The modifiers are settled before the key is read, so `async *#m` is one member
// rather than a `#` the parser met while it was expecting something else - and the same holds one
// word further along for `static async *#m`.
class Held {
  async *#pump() {
    yield 1;
    yield 2;
  }

  static async *#served() {
    yield "static";
  }

  run() {
    return this.#pump();
  }

  static serve() {
    return Held.#served();
  }
}

async function drain(source) {
  var out = [];

  for await (const value of source) {
    out.push(value);
  }

  return out.join(",");
}

drain(new Held().run()).then(function (instance) {
  return drain(Held.serve()).then(function (served) {
    print(instance + ";" + served);
  });
});
