// AN OBJECT SPREAD AND AN OBJECT REST COPY ENUMERABLE SYMBOL-KEYED PROPERTIES (JSP-6, JSC-238), as
// they copy String-keyed ones; every Symbol was dropped. A non-enumerable one is not copied, and
// the keys come out Strings first, then Symbols, as `[[OwnPropertyKeys]]` gives them.
var tag = Symbol('tag');
var hidden = Symbol('hidden');
var source = { a: 1 };
source[tag] = 'spread';
Object.defineProperty(source, hidden, { value: 'hidden', enumerable: false });

var copy = { ...source };
var { a, ...rest } = source;

[
  copy[tag],
  hidden in copy,
  rest[tag],
  Reflect.ownKeys(copy).length,
  String(Reflect.ownKeys(copy)[1]),
].join(' / ');
