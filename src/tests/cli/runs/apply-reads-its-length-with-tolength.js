// `Function.prototype.apply` READS ITS LIST'S `length` WITH `ToLength` (JSP-4, JSC-236), which is
// what `CreateListFromArrayLike` says. A `length` of -1 is an empty list, where `ToUint32` made it
// four billion reads and the program spent its allowance; a fractional or a String length is
// truncated and converted; and 2**32 + 2 is longer than this profile calls with, the RangeError
// `Reflect.apply` gives, where `ToUint32` made it a list of two.
function count() {
  return arguments.length;
}

var tooLong;

try {
  count.apply(null, { length: 2 ** 32 + 2, 0: 'a', 1: 'b' });
} catch (error) {
  tooLong = error.constructor.name;
}

[
  Math.max.apply(null, { length: -1, 0: 5 }),
  count.apply(null, { length: 1.5, 0: 'a' }),
  count.apply(null, { length: '2' }),
  tooLong,
].join(' / ');
