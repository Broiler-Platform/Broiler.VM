// A PROGRAM THAT AWAITS FOR EVER ENDS AS A NAMED RESOURCE EXHAUSTION, never as a hang and never as
// a terminated process. Every turn round this loop makes a promise, enqueues a reaction and resumes
// the frame, and all three are charged - so what bounds it is the allowance rather than the queue's
// length, exactly as it bounds a loop with no exit. The native stack does not grow with it: each
// resumption starts from the drain and returns to it.
async function forever() {
  while (true) {
    await 0;
  }
}

forever();
