// A `var` is initialised to `undefined` when its scope is entered, so reading one before its
// declaration is `undefined` and not an error. This is the distinction the dead zone draws,
// and a change that threw here would have broken hoisting.
var y = x;
var x = 1;
y;
