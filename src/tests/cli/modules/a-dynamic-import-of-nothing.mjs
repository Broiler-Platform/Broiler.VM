// A specifier this composition can resolve and find no file for. It is a REJECTION and not a
// refusal: the artifact was sound, the module was simply not there, and a program that says what
// it will do about that is a program this host runs to completion.
import("./nowhere.mjs").then(
  function () {
    print("resolved");
  },
  function (reason) {
    print("rejected " + (reason instanceof TypeError));
  });

"asked";
