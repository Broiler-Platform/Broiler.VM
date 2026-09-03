// The temporal dead zone. Reading a lexical binding before its initialiser has run is a
// RUNTIME ReferenceError, not `undefined` - and answering `undefined` was what this profile
// did until the format grew an instruction that could fail at all.
x; let x;
