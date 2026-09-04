// A REGRESSION FIXTURE. Two parameters sharing one name declare ONE binding, so the frame's copy
// loop was told to fill two slots that were not there - and the verifier refused an artifact this
// host had just produced. It is an ordinary sloppy-mode program that every engine runs, and the
// second parameter is the one that wins.
//
// Reverting the repair puts this file back to exit 4.
function pick(a, a) {
  return a;
}

pick(1, 2) + "/" + pick(3) + "/" + pick.length;
