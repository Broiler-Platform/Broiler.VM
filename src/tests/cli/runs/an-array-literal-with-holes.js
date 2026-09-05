// A REGRESSION FIXTURE FOR THE LOWERING OF AN ARRAY LITERAL THAT IS NOT DENSE-AND-SMALL.
//
// The dense path under a thousand elements pushes every element and takes one `NewArray`. Anything
// else - a hole, or more elements than that path admits - is built element by element and then has
// its `length` set, and the length-setting used to consume the array itself: `SetProperty` pops a
// value and a base and pushes the value back, so the array under construction was replaced by the
// count and discarded by the `Pop` that followed. The literal then produced NO value where its
// caller expected one, and this host's own verifier refused the artifact with an operand-stack
// underflow at whatever instruction later popped one value too many
// *(corrected: JSC-81)*.
//
// The whole file was reachable only through the Octane `pdfjs` benchmark, which is a file this
// repository does not hold. This is the same construct in eleven lines.

var holes = [, 1];
print(holes.length + ":" + (0 in holes) + ":" + holes[1]);

var trailing = [1, , ];
print(trailing.length + ":" + trailing[0]);

var wide = [];
for (var i = 0; i < 1200; i++) { wide[i] = i; }
var big = [
  wide[0], wide[1], wide[2]
];
print(big.length + ":" + big[2]);

var nested = [[, 2], [3, , 4]];
print(nested.length + ":" + nested[0].length + ":" + nested[1].length + ":" + nested[1][2]);

"array-literal-holes ok";
