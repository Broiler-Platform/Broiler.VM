// A String OBJECT'S OWN KEYS ARE ITS EXOTIC ONES, IN THE LANGUAGE'S ORDER (JSP-5, JSC-237): the
// string's indices, every other index ascending, `length`, then the other names as they were made.
// Redefining an index with the descriptor it already has changes nothing, where it added a second
// own `0`; and an index past the end sorts before `length`, where it sorted after it.
var redefined = new String('ab');

Object.defineProperty(redefined, '0', {
  value: 'a',
  writable: false,
  enumerable: true,
  configurable: false,
});

var grown = new String('ab');
grown.x = 1;
grown[7] = 1;
grown[5] = 1;

var refused;

try {
  Object.defineProperty(new String('ab'), '0', { value: 'z' });
} catch (error) {
  refused = error.constructor.name;
}

[
  Object.getOwnPropertyNames(redefined).join(','),
  Object.getOwnPropertyNames(grown).join(','),
  Reflect.ownKeys(grown).length,
  refused,
].join(' / ');
