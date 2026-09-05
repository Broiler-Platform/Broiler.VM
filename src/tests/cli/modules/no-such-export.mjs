// The dependency of the unlinkable-module row: a module that re-exports a name `lib.mjs` does not
// publish. It is reached only by a dynamic import, so nothing that runs statically depends on it.
export { missingEntirely } from "./lib.mjs";
