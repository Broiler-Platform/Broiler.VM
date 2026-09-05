// `return` OUTSIDE A FUNCTION IS AN EARLY ERROR AND NOT A SURFACE THIS PROFILE DECLINES. The
// refusal was `2104` until 2026-09-05, which said the manifest had not built the construct - and a
// conformance runner reading that code takes every case of a top-level `return` out of both the
// pass and the fail column instead of scoring the `SyntaxError` the language states.
//
// The block is here for the reason [JSC-111](../../../Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md)
// records: it declares a lexical name and therefore pushes a scope, which is the shape that once
// made the refusal depend on whether an unrelated declaration was present.
{
  let anything = 1;
  return anything;
}
