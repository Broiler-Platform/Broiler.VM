// `for … in` ASKS FOR EACH NAME WHEN IT REACHES IT (JSP-5, JSC-237), which is what the
// specification's `%ForInIteratorPrototype%.next` does. A name deleted before the loop reaches it is
// not visited, own or inherited, and a name made non-enumerable before it is reached is not visited
// either; the loop collected every name at its start and visited all three. A name added during
// the loop is not visited, which the language permits.
function visit(object, change) {
  var seen = [];

  for (var key in object) {
    seen.push(key);
    change(key);
  }

  return seen.join(',');
}

var own = { a: 1, b: 2, c: 3 };
var parent = { x: 1 };
var child = Object.create(parent);
child.a = 1;
var demoted = { a: 1, b: 2, c: 3 };
var grown = { a: 1 };

[
  visit(own, function (key) {
    if (key === 'a') delete own.b;
  }),
  visit(child, function (key) {
    if (key === 'a') delete parent.x;
  }),
  visit(demoted, function (key) {
    if (key === 'a') Object.defineProperty(demoted, 'b', { enumerable: false });
  }),
  visit(grown, function (key) {
    if (key === 'a') grown.z = 1;
  }),
].join(' / ');
