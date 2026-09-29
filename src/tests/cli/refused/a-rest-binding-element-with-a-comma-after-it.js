// A REST ELEMENT IN A BINDING PATTERN IS LAST WITH NOTHING AFTER IT, the rule
// `a-rest-element-with-a-comma-after-it.js` pins for an assignment pattern. Until 2026-09-29 the
// comma here reached the closing bracket's check and was answered as "`]` was expected" - a
// token-level refusal for a construct the other side of the cover grammar already named.
var [...rest, last] = [1, 2];
