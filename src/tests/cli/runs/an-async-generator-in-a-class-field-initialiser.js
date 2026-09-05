// TWO POSITIONS THE CLASS BODY OPENED, one an expression and one a statement list. A field
// initialiser is its own function body, so an async generator EXPRESSION written in one goes down a
// path no top-level declaration reaches; a static block is a statement list, so an async generator
// DECLARATION written in one goes down another. Both were refused here while the family was, and
// both are exercised rather than assumed.
class Streamed {
  source = async function* () {
    yield "field";
  };

  static made = null;

  static {
    async function* pump() {
      yield "block";
    }

    Streamed.made = pump;
  }
}

async function drain(source) {
  var out = [];

  for await (const value of source) {
    out.push(value);
  }

  return out.join(",");
}

drain(new Streamed().source()).then(function (field) {
  return drain(Streamed.made()).then(function (block) {
    print(field + ";" + block);
  });
});
