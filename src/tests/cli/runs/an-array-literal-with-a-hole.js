// A REGRESSION FIXTURE, not a feature one. An array literal with an elision was lowered by
// setting `length` with SetProperty, which pops the Array as well as the value and pushes only
// the value back - so the literal left NOTHING on the operand stack and the verifier refused the
// whole artifact this host had just produced. Reverting the repair puts this file back to exit 4.
//
// A hole is not a stored `undefined`, which is what the `in` test here is for: a repair that
// filled the holes would answer `3 true undefined 2` and pass a length-only check.
var sparse = [1, , 3];

sparse.length + " " + (1 in sparse) + " " + String(sparse[1]) + " " + [, ,].length;
