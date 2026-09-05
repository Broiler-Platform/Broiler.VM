// THE SCOPE ACCOUNTING, EXERCISED ON EVERY WAY OUT OF A `with` BODY. The object environment record
// is a scope, so the depth has to be exactly right at each exit or the verifier refuses the
// artifact this lowering produced - and where it does NOT refuse, the executor reads a slot of the
// wrong environment, which compiles, verifies and answers the wrong value.
//
// The five exits are: falling out of the bottom, `break`, `continue`, a labelled `break` that
// leaves two of them at once, `return`, and an exception unwinding to a handler outside.
var answers = [];

var fell = "";
for (var i = 0; i < 3; i++) {
  with ({ turn: i }) {
    fell += turn;
  }
}

answers.push(fell);

var stopped = "";
for (var i = 0; i < 5; i++) {
  with ({ turn: i }) {
    if (turn === 3) {
      break;
    }

    stopped += turn;
  }
}

answers.push(stopped);

var skipped = "";
for (var i = 0; i < 5; i++) {
  with ({ turn: i }) {
    if (turn % 2 === 0) {
      continue;
    }

    skipped += turn;
  }
}

answers.push(skipped);

var labelled = "";
outer: for (var i = 0; i < 3; i++) {
  with ({ row: i }) {
    for (var j = 0; j < 3; j++) {
      with ({ column: j }) {
        if (row === 1 && column === 1) {
          break outer;
        }

        labelled += "" + row + column;
      }
    }
  }
}

answers.push(labelled);

// A `return` OUT OF TWO OF THEM, WITH A `finally` BETWEEN. The finaliser runs at the depth it was
// written at, which is what the unwinding has to restore before it emits the body.
function returnsThroughTwo() {
  var ran = [];

  try {
    with ({ a: 1 }) {
      with ({ b: 2 }) {
        return "returned:" + (a + b);
      }
    }
  } finally {
    ran.push("finally");
    answers.push(ran.join(""));
  }
}

answers.push(returnsThroughTwo());

// AN EXCEPTION UNWINDING THROUGH ONE. The handler is entered at the scope depth its region
// declared, which is the depth OUTSIDE the `with` - so the object record is discarded on the way.
var caught = "";

try {
  with ({ visible: "inside" }) {
    caught += visible;
    throw new RangeError("thrown through a with");
  }
} catch (failure) {
  caught += "/" + failure.name + "/" + typeof visible;
}

answers.push(caught);

// AND `continue` OUT OF TWO OF THEM, which discards two records and re-enters the loop.
var continued = "";
for (var i = 0; i < 3; i++) {
  with ({}) {
    with ({}) {
      if (i === 1) {
        continue;
      }
    }
  }

  continued += i;
}

answers.push(continued);

print(answers.join(" "));
