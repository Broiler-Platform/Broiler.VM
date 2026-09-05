// THE DYNAMIC SURFACE, AND THE ONE THING ABOUT IT THAT IS NOT THE LANGUAGE'S.
//
// `eval` here does not compile anything. It hands its source to the artifact provider this
// composition registered, the core verifies whatever comes back into its own immutable handle
// before a byte of it runs, and the result is executed in this realm. A sibling composition that
// registers no provider runs this same file and answers an `EvalError` instead - that refusal is
// the content policy, and it is deliberately a different event from a composition DECLINING the
// `broiler.javascript.dynamic` identity, which refuses the artifact at verification before any
// guest exists.
//
// THE DIRECT FORM IS ADMITTED ONLY WHERE IT MEANS WHAT THE INDIRECT FORM MEANS. A direct `eval`
// evaluates in the caller's scope, and this profile resolves every name at lowering: source
// compiled without any knowledge of the calling frame reaches the global object. At the top level
// of a script that is exactly right. Inside a function it is not, and rather than answer a program
// that reads a local with a global's value, it refuses by name - which is what the last two lines
// assert, and which is a published exclusion rather than a defect waiting to be found.

var indirect = eval;
print(indirect("1 + 2"));

indirect("var declaredByEval = 41;");
print(declaredByEval + 1);

print(eval("6 * 7"));

var add = new Function("a", "b", "return a + b;");
print(add(2, 3) + ":" + add.length + ":" + typeof add);

var joined = new Function("a,b", "return a * b;");
print(joined(6, 7));

print(eval(42) + ":" + typeof eval({}));

function localsAreNotVisible() {
  var hidden = 1;
  try {
    return eval("hidden");
  } catch (refused) {
    return refused.name + ":" + (refused.message.indexOf("direct eval") >= 0);
  }
}

print(localsAreNotVisible());

function indirectionEscapesIt() {
  var outer = eval;
  return outer("typeof hidden");
}

print(indirectionEscapesIt());

"indirect-eval ok";
