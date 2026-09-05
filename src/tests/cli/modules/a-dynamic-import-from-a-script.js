// A dynamic import written in a SCRIPT, which is the case the mediator answers. Nothing here
// carries a module record - a script's artifact has none - so the specifier is put to this
// composition's artifact provider, which resolves it against the script's own path, reads the
// file, compiles a module graph and answers with it. The guest sees a promise either way.
import("./lib.mjs").then(function (namespace) {
  namespace.bump();
  print("loaded " + namespace.default + " " + namespace.counter);
});

"asked";
