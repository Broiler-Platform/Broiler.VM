// THE SAME RECURSION WITH A WIDE FRAME, WHICH IS WHAT MAKES THE MEASUREMENT AN UPPER BOUND.
//
// A frame's native cost is not one number: the executor sizes its operand stack from the height the
// verifier computed for the unit, and holds a live environment beside it, so a function whose
// expressions are deep costs more per call than one whose are not. Measuring only the narrow shape
// would produce a per-frame figure that is true of the smallest frame and false of every other, and
// a depth bound derived from it would be a bound that holds for the program nobody writes.
//
// This one holds nineteen live values across a call and passes eight arguments, which is wide
// enough to move the figure and narrow enough that the arithmetic stays readable.

function wide(a, b, c, d, e, f, g, h) {
  var p = a + 1, q = b + 2, r = c + 3, s = d + 4, t = e + 5, u = f + 6, v = g + 7, w = h + 8;
  var x = p * q, y = r * s, z = t * u, aa = v * w, bb = x + y, cc = z + aa;
  return wide(p, q, r, s, t, u, v, w) + bb + cc + x + y + z + aa;
}

wide(1, 2, 3, 4, 5, 6, 7, 8);
