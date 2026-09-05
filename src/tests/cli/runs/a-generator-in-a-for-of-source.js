// THE POSITION THE REFUSAL WAS GUARDING, CHECKED FROM THE OTHER SIDE. The source of a `for … of`
// is an expression position that was opened by the parameter-default bundle, and a generator
// function written there was refused BY NAME for as long as the generator family was outside the
// manifest. It is inside it now, so the row that asserted the refusal asserted something false and
// this file replaces it: the same position, admitted, with the composition of the two families -
// `for … of` stepping a generator through the real `Symbol.iterator` - as the thing that answers.
function* counting(limit) {
  for (var at = 1; at <= limit; at++) {
    yield at;
  }
}

var total = 0;

for (var value of counting(4)) {
  total += value;
}

print(total);
