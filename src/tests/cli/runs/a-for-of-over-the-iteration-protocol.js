// A pattern head takes each yielded value apart, and a String iterates by code point rather than
// by index property, which is the difference between `for … of` and `for … in` over one.
var out = [];

for (const [key, value] of [["a", 1], ["b", 2]]) {
  out.push(key + value);
}

for (const character of "xy") {
  out.push(character);
}

out.join(",");
