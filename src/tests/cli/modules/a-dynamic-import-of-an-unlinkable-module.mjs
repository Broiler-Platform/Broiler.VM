// A module whose text is a perfectly good module and whose GRAPH is not: it re-exports a name the
// module it names does not have, so it parses, compiles, and is refused where the whole graph is
// present. The rejection is a `SyntaxError` and not a `TypeError`, because the language calls a
// name nothing exports part of the module's own syntax - and a host that answered a link failure
// and a parse failure differently would be telling the guest which of its own passes refused.
import("./no-such-export.mjs").then(
  function () {
    print("resolved");
  },
  function (reason) {
    print("rejected " + (reason instanceof SyntaxError));
  });

"asked";
