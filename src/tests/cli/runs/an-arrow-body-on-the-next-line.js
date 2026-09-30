// AN ARROW's `=>` STANDS ON THE LINE ITS PARAMETERS END ON, and the body may start on the next:
// only the break BEFORE the arrow is refused.
var double = (value) =>
  value * 2;
var add = (left,
  right) => left + right;
var negate = value =>
  -value;

[double(4), add(2, 3), negate(6)].join(' / ');
