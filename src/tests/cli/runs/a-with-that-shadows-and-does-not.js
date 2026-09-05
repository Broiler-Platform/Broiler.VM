// THE WHOLE OF WHAT `with` DOES, IN THE ORDER IT DOES IT. A name inside the body is asked of the
// object FIRST - through `HasProperty`, so the prototype chain counts - and only then of the
// enclosing scopes. Every value below was taken from the comparison engine before it was written
// here.
//
// The last two lines are the pair that matters most: a name the object HAS is the object's, and a
// name it does not have reaches the binding the language's own scope rules give it. An
// implementation that resolved statically and ignored the object would answer `outer` twice; one
// that searched the scope chain by name would answer the first and then reach a slot it has no
// business reaching.
var answers = [];

var outerVar = "outer-var";
let outerLet = "outer-let";
const outerConst = "outer-const";

var holder = { outerVar: "held-var", outerLet: "held-let", outerConst: "held-const" };

with (holder) {
  answers.push(outerVar, outerLet, outerConst);
}

with ({}) {
  answers.push(outerVar, outerLet, outerConst);
}

// A PARAMETER IS SHADOWED EXACTLY AS A `var` IS, because both are bindings of a declarative record
// and the object record sits in front of both.
function shadowsAParameter(given) {
  with ({ given: "held" }) {
    return given;
  }
}

function doesNotShadowAParameter(given) {
  with ({}) {
    return given;
  }
}

answers.push(shadowsAParameter("passed"), doesNotShadowAParameter("passed"));

// A NAME THE OBJECT HAS AS `undefined` IS STILL THE OBJECT'S. The test is `HasProperty` and not
// truthiness, which is the difference between a binding and a value.
var present = "outer";
with ({ present: undefined }) {
  answers.push(String(present), typeof present);
}

// AND `typeof` ANSWERS FOR A NAME NOTHING HAS, without throwing, exactly as it does outside.
with ({}) {
  answers.push(typeof nothingHasThisName);
}

print(answers.join(" "));
