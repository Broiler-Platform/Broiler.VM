// A cycle in the module GRAPH is ordinary and runs. `b` is evaluated first because `a`
// requests it, and the function of `a` it calls already exists - which is what the separate
// initialisation pass is for.
import { fromB } from "./cycle-b.mjs";

export function fromA() {
  return 2;
}

fromB() + 1;
