// Dead code after a statement control cannot pass. The lowering used to emit the declaration
// anyway and the verifier refused the artifact as 1411:UnreachableCode.
//
// SUPPRESSING A `var` IS SAFE and this file is the case that shows it: `var` is hoisted, so
// `x` exists and holds `undefined` whether or not its initialiser runs - which is exactly
// what the language says, because the initialiser never runs here either.
for (;;) {
  break;
  var x = 1;
}
x;
