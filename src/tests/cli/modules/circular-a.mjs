// A cycle in an export RESOLUTION names a binding that exists nowhere. It is refused with a
// named diagnostic at verification rather than by walking the cycle until an allowance runs out.
export { spin } from "./circular-b.mjs";
