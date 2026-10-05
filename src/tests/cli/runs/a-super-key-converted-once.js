// A `super[k]` THAT IS READ AND THEN WRITTEN CONVERTS `k` ONCE (JSP-4, JSC-236). A compound, an
// update and a logical assignment each read the inherited value and write onto `this`, and the
// write uses the key the read produced, as the reference keeps it - so the key's `toString` runs
// once where it ran twice. The base is still taken before the key is converted: a `toString` that
// re-points the home object's prototype does not change where the read looks. The same four forms
// on an ordinary computed member held before this change, and are asked beside them.
var seen = 0;
var key = {
  toString: function () {
    seen++;
    return 'p';
  },
};

var base = { p: 1 };
var elsewhere = { p: -100 };

var object = {
  __proto__: base,
  compound() {
    super[key] += 10;
    return this.p;
  },
  update() {
    super[key]++;
    return this.p;
  },
  logical() {
    super[key] ||= 5;
    return this.p;
  },
  nullish() {
    super[key] ??= 5;
    return this.p;
  },
};

var answers = [];

for (var name of ['compound', 'update', 'logical', 'nullish']) {
  seen = 0;
  answers.push(name + ' ' + object[name]() + ':' + seen);
}

var plain = { p: 1 };
seen = 0;
plain[key] += 10;
plain[key]++;
plain[key] ||= 5;
plain[key] ??= 5;
answers.push('plain ' + plain.p + ':' + seen);

var turning = {
  toString: function () {
    Object.setPrototypeOf(late, elsewhere);
    return 'p';
  },
};

var late = {
  __proto__: base,
  compound() {
    return super[turning] += 1;
  },
};

answers.push('base ' + late.compound());
answers.join(' / ');
