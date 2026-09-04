// A REGRESSION FIXTURE FOR THE TWO WAYS THE `arguments` BINDING WAS WRONG.
//
// FIRST: a formal parameter named `arguments` IS the binding, and no arguments object is created.
// The lowering used to declare the name after the parameters, get the parameter's own slot back,
// and initialise it with a fresh arguments object - so the actual the caller passed was gone before
// the first statement ran. The TypeScript compiler in the Octane suite has
// `function FuncDecl(name, bod, isConstructor, arguments, ...)` and then reads `this.arguments`,
// which is how a real workload found it *(corrected: JSC-82)*.
//
// SECOND: an arrow function has no `arguments` of its own, so a mention inside one is a mention of
// the enclosing function's. The walk that decides whether to materialise the object stopped at
// every function-like node, arrows included, so the enclosing function declared no slot and the
// inner reference fell through to a global read that threw *(corrected: JSC-83)*.
//
// Both are answers rather than refusals: reverting either repair leaves this file running and
// printing a different value, which is what a control has to do to have judged anything.

function shadowed(a, b, arguments) { return arguments; }
print(shadowed(1, 2, "third"));

function alsoShadowed(arguments) { return typeof arguments; }
print(alsoShadowed(7));

function ordinary(a) { return arguments.length + ":" + arguments[1]; }
print(ordinary(1, 2, 3));

function throughAnArrow() { var read = () => arguments[0]; return read(); }
print(throughAnArrow("captured"));

function throughANestedArrow(a) { var outer = () => (() => arguments.length)(); return outer(); }
print(throughANestedArrow(1, 2, 3, 4));

function notShadowedByAVar() { var arguments2 = 1; return arguments.length; }
print(notShadowedByAVar(5, 6));

"parameter-named-arguments ok";
