// THE SECOND SCRIPT, WHICH IS WHY THE FIRST ONE'S JOB IS EVIDENCE. It runs to its end with the
// first script's async call still suspended and its promise still pending, so the line the first
// script prints reports an order two scripts and one drain produced rather than an order two
// statements produced. It also enqueues a job of its own, so the drain runs more than one.
//
// THE TWO FILES ARE NAMED SO THEY SORT IN THE ORDER THEY RUN. This host expands its arguments into
// a stable ordinal order rather than the order they were typed, which is what makes two transcripts
// of one command comparable - so a pair of scripts whose point is their ORDER has to carry it in
// the names.
//
// THIS FILE IS NOT A PROGRAM BY ITSELF, which is why it sits under `one-realm/` and not under
// `runs/`. `trace` is declared by the first script; run alone - as a sweep over a directory runs
// every file it finds, each in a realm of its own - this one can only throw.
trace.push("second-script-ended");

Promise.resolve().then(function () {
  trace.push("a-job-the-second-script-owed");
});
