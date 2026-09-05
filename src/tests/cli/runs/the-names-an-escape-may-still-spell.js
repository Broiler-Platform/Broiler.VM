// THE NAMES AN ESCAPE MAY STILL SPELL. The rule that refuses a reserved word reached through a
// unicode escape is about an Identifier, and an IdentifierName is a different production: a
// property key and a member name both admit any spelling of any word at all.
//
// The second half is the other direction of the CONTEXT half of the rule: a word that is reserved
// somewhere and not here is a name here however it is spelled, so an escaped `await` binds in a
// sloppy script exactly as the plain one does.
var o = { \u0069f: 1, \u0063lass: 2 };
var \u0061wait = 3;
var \u0061sync = 4;

[o.\u0069f, o["class"], \u0061wait, \u0061sync].join("/");
