// A REJECTION NOBODY HANDLED IS SILENT HERE, AND THAT IS A STATED DEVIATION rather than an
// oversight. The specification tracks whether a rejected promise ever had a handler so that a HOST
// can report one; this profile's result envelope carries a completion or a fault and has no third
// thing to carry a rejection nobody asked about, so there is nothing for the flag to be read by.
// The comparison engine prints the rejection at exit and leaves a non-zero status; this host runs
// the rest of the queue and completes.
//
// WHAT THE ROW ACTUALLY PINS is the second half: the drain does not stop. A rejected promise with
// no reactions runs no job at all, so the jobs enqueued after it must still run - and a drain that
// treated the rejection as a fault would swallow them.
async function unhandled() {
  throw new Error("nobody-catches-this");
}

unhandled();

Promise.resolve()
  .then(function () { print("the queue ran after it"); })
  .then(function () { print("and kept running"); });
