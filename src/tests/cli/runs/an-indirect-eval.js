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
// THE DIRECT FORM EVALUATES IN THE CALLER'S SCOPE, and since JSeal V14 it does inside a function
// too: the caller's artifact carries a scope map for the call site, and the evaluated source reads
// and writes the caller's own bindings through it (JSD-0026). Since JSeal V15 a sloppy evaluation
// that DECLARES a `var` introduces it into the caller's function, as a binding `delete` can remove,
// which is what the declaration line asserts. The indirect form never sees a local, which is what
// the last line asserts.

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

function localsAreVisible() {
  var hidden = 1;
  eval("hidden = hidden + 1");
  return "local:" + hidden;
}

print(localsAreVisible());

function declarationsAreIntroduced() {
  eval("var introduced = 1");
  var before = introduced;
  var removed = delete introduced;
  return "introduced:" + before + ":" + removed + ":" + typeof introduced;
}

print(declarationsAreIntroduced());

function indirectionEscapesIt() {
  var outer = eval;
  return outer("typeof hidden");
}

print(indirectionEscapesIt());

"indirect-eval ok";
