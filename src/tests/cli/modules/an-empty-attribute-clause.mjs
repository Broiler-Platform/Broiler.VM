// An EMPTY attribute clause asks nothing of a host, so there is nothing for a host to be unable to
// honour and all three forms load. The bindingless one is the row that matters: it returns from the
// import parser before the clause is reached unless the parser looks for one there too, and a
// reading that did not reported a missing semicolon against a program the grammar admits.
import held from "./lib.mjs" with {};
import "./lib.mjs" with {};
export * from "./lib.mjs" with {};

held;
