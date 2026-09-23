// THE FIRST OF TWO SCRIPTS IN ONE REALM (JSeal V15-host). Its `let` is a binding of the realm's
// global lexical environment, which outlives this script: the second script reads it.
let shared = "from-the-first-script";
