// A REGRESSION FIXTURE FOR A `for await` AT A MODULE'S TOP LEVEL.
//
// A top-level `for await` suspends the module body at every step, so the body is lowered as a
// unit that may await - which is what a top-level `await` and a top-level `await using` already
// made it. The parser recorded those two and not this one, so the body was lowered as a unit that
// may not await, and this host's own verifier refused the loop's first asynchronous instruction
// (1630, AsyncIterationOutsideAsync) at exit 4, on bytes this host's own lowering produced
// *(corrected: JSC-229)*. The same loop inside an async function always ran.
//
// Each row is an ANSWER rather than an exit code, so reverting the repair moves a value.

async function* counted() {
  yield 1;
  yield 2;
}

let sum = 0;

for await (const value of counted()) {
  sum += value;
}

print("generator " + sum);

// An Array has no Symbol.asyncIterator; the loop falls back to its iterator and awaits each value.
let seen = [];

for await (let value of [Promise.resolve("p"), "q", Promise.resolve("r")]) {
  seen.push(value);
}

print("fallback " + seen.join(","));

// A `var` head, and a `break` that closes the iterator before it runs out.
let closed = false;

const source = {
  [Symbol.asyncIterator]() {
    let step = 0;
    return {
      next() { step++; return Promise.resolve({ value: step, done: false }); },
      return() { closed = true; return Promise.resolve({ done: true }); },
    };
  },
};

for await (var head of source) {
  if (head === 3) {
    break;
  }
}

print("break " + head + " " + closed);

"top-level-for-await ok";
