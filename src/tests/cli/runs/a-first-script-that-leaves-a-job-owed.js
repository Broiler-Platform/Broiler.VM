// THE FIRST OF TWO SCRIPTS IN ONE REALM. It starts an async function whose continuation cannot run
// while it is still running: the body suspends at its `await`, and the only thing that resumes it
// is a job. This host states its drain point as "after the last script", so the resumption below
// happens after the SECOND script has finished - which is what the printed order is evidence for,
// and which a profile that drained implicitly at the end of each script would get wrong.
globalThis.trace = [];

globalThis.owed = async function () {
  trace.push("suspended-in-the-first-script");
  await 0;
  trace.push("resumed-at-the-drain");
  print(trace.join(","));
};

owed();
trace.push("first-script-ended");
