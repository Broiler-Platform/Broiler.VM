// A REST ELEMENT IS LAST WITH NOTHING AFTER IT, AND A COMMA IS SOMETHING. This is the one rule
// that reads differently on the two sides of the cover grammar these brackets are: an array
// LITERAL may end with a comma, so `var a = [...x,];` is a value and is a program, and an
// `ArrayAssignmentPattern` puts the rest element last with not even that comma after it.
//
// The refusal is `2205` and not `2104`: the manifest admits a rest element in a pattern, so a
// program that writes a comma after one is wrong about the LANGUAGE. The accepting side of the
// same rule is `runs/the-patterns-a-rest-and-a-shorthand-still-admit.js` beside this one.
var x;

0, [...x,] = [];
