// ONE DECLARATIVE SCOPE HOLDS ONE LEXICAL BINDING PER NAME, and a second declaration of the same
// name has nothing to do rather than something to overwrite. The refusal is `2201` and not `2104`:
// the manifest admits `let`, so a program that declares one twice is wrong about the LANGUAGE.
//
// The block matters. The same two declarations in two sibling blocks are a program, which is what
// `runs/the-names-one-scope-may-hold-twice.js` beside this one asserts from the other side.
{
  let f = 1;
  let f = 2;
}
