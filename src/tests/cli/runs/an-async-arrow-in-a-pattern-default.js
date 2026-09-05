// THE POSITION THE OLD REFUSAL WAS GUARDING, CHECKED FROM THE OTHER SIDE. This file asserted that
// an async arrow written in a pattern element's default was refused BY NAME; the family is inside
// the manifest now, so that assertion is false and the file checks the composition instead - the
// default runs only when the element is absent, the arrow it builds is an async one, and its
// promise settles with what the arrow returned.
var built = [];

var [first = async () => "from-the-default"] = [];
var [second = async () => "unused"] = [async () => "from-the-array"];

Promise.all([first(), second()]).then(function (v) {
  built.push(v.join(","));
  print(built.join(",") + " " + Object.prototype.toString.call(first));
});
