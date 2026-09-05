// WHAT A `with` BODY DECLARES BELONGS TO THE ENCLOSING FUNCTION, and what it deletes is a property
// of the object. Those are the two halves this file pins.
//
// A `var` inside a `with` body is the enclosing function's binding - the body is not a hoisting
// scope - and a `function` declaration inside one is hoisted to the enclosing function too, so it
// is callable after the statement ends.
var answers = [];

function hoistsOutward() {
  with ({}) {
    var declaredInside = "hoisted";

    function declaredFunction() {
      return "callable";
    }
  }

  return declaredInside + "/" + declaredFunction();
}

answers.push(hoistsOutward());

// A NESTED `with` HOISTS THE SAME WAY, and a `var` in the deeper one is still the function's.
function hoistsThroughTwo() {
  with ({}) {
    with ({}) {
      var deep = "still the function's";
    }
  }

  return deep;
}

answers.push(hoistsThroughTwo());

// `delete` OF A BARE NAME IS THE ONE SPELLING THAT REACHES AN ENVIRONMENT RECORD AT ALL, and inside
// a `with` body it deletes the object's property when the object has the name.
var removable = { gone: 1 };
var removed;

with (removable) {
  removed = delete gone;
}

answers.push(removed, "gone" in removable);

// DELETING AN INHERITED NAME SUCCEEDS AND REMOVES NOTHING, because the deletion is of an own
// property the object does not have - which is what `delete` answers for any absent own property.
var inheritedFrom = { kept: "on the prototype" };
var inheritor = Object.create(inheritedFrom);
var inheritedResult;

with (inheritor) {
  inheritedResult = delete kept;
}

answers.push(inheritedResult, inheritor.kept);

// AND DELETING A NAME NO OBJECT ON THE CHAIN HAS IS ABOUT A BINDING RATHER THAN A PROPERTY: a slot
// binding is not configurable, so the answer is `false` and the binding is untouched.
function deletesABinding() {
  var bound = "still here";
  var answer;

  with ({}) {
    answer = delete bound;
  }

  return answer + "/" + bound;
}

answers.push(deletesABinding());

print(answers.join(" "));
