// A REST PROPERTY IN A BINDING PATTERN IS LAST WITH NOTHING AFTER IT. Until 2026-09-29 this was
// answered as "`}` was expected", where the same comma in an assignment pattern named the rest
// property.
var { ...rest, last } = {};
