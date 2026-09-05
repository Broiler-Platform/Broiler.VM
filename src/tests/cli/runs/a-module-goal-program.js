// Read under the module goal, which is strict by its goal symbol rather than by a
// directive. Nothing here is a module GRAPH: the goal and the graph are two things, and this
// row is the goal on its own - a source with no import and no export, presented under
// `--module`, which is strict and carries no module records at all.
var x = 1;
x + 2;
