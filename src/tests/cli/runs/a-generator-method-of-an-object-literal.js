// AND THE SAME `*` ONE POSITION AWAY, ADMITTED. An object literal's generator method is parsed,
// lowered and run - the key, the parameter list and the body all go through the paths a
// `function*` goes through - so this row is the other half of the refusal beside it: what the
// manifest declines is the CLASS MEMBER and not the modifier.
var counter = {
  base: 10,
  *from(start, step = 1) {
    for (var at = 0; at < 3; at++) {
      yield this.base + start + (at * step);
    }
  },
};

print([...counter.from(1, 2)].join(","));
