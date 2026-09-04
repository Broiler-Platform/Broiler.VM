// AN ASYNC METHOD KEEPS ITS RECEIVER AND ITS HOME OBJECT ACROSS A SUSPENSION, which is what the
// second `super` call after the `await` is here to catch: the frame that resumes is resumed by a
// JOB, and a job knows nothing about the call site - so both have to have been recorded on the
// frame rather than re-supplied. A static async method is the same question one level up.
class Base {
  async name() {
    await 0;
    return "Base";
  }

  tag() {
    return "tag";
  }
}

class Derived extends Base {
  constructor(mark) {
    super();
    this.mark = mark;
  }

  async describe() {
    var inherited = await super.name();
    await 0;
    return inherited + "/" + super.tag() + "/" + this.mark;
  }

  static async build(mark) {
    await 0;
    return new Derived(mark);
  }
}

Derived.build("m")
  .then(function (d) { return d.describe(); })
  .then(function (v) { print(v); });
