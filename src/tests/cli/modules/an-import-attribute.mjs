// The clause parses and the ATTRIBUTE is declined, which is the whole of the distinction: the
// grammar is ordinary, this front end reads it, and what no composition of this profile has is a
// loader for a module of this type. For a static import the load happens where the graph is
// walked, which is here, so this is where the refusal is. (It named `type: "json"` until JSON
// modules arrived, JSC-255.)
import held from "./lib.mjs" with { type: "css" };

held;
