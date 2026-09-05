// A dependency of the module-goal probe, reached only by a dynamic import and never by a static
// one. It re-exports a name the module it names does not have, which is a failure of LINKING and
// not of parsing: the text is a perfectly good module and the graph it belongs to is not.
export { missingEntirely } from "./counter.mjs";
