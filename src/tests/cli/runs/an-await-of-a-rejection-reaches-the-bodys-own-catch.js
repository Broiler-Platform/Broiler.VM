// A REJECTED `await` RAISES AT THE `await`, inside the body's own exception regions - which is what
// makes the `catch` below the program's and not the promise machinery's. The loop is the part worth
// reading: the frame is resumed abruptly at the suspension point, the region search runs the same
// handlers it would run for a throw from the instruction itself, and the body then CARRIES ON round
// the loop. A resumption that unwound the frame instead would answer the first two elements and
// stop.
async function survey() {
  var seen = [];

  for (var index = 0; index < 4; index++) {
    try {
      seen.push("ok:" + (await (index === 2 ? Promise.reject(new Error("bad")) : index)));
    } catch (e) {
      seen.push("caught:" + e.message);
    } finally {
      seen.push("f" + index);
    }
  }

  return seen.join(",");
}

survey().then(function (v) { print(v); });
