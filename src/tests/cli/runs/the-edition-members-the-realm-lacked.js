// MEMBERS OF THE EDITION THE REALM DID NOT HAVE, and one it had that the edition does not (JSP-7,
// JSC-239). `Error.isError` asks for the slot, so a re-pointed error is one and `Error.prototype`
// is not; `WeakMap` has `getOrInsert` and `getOrInsertComputed`; `RegExp.prototype.unicodeSets`
// exists, and answers `false` for a `u` pattern and `true` for a `v` one since phase F2 admitted the
// flag (JSC-262) - it answered `false` for every RegExp before; and `cleanupSome`, a proposal's
// member, is gone.
var weak = new WeakMap();
var key = {};

[
  [
    Error.isError(new TypeError()),
    Error.isError(Object.setPrototypeOf(new Error(), null)),
    Error.isError(Error.prototype),
    Error.isError({ __proto__: Error.prototype }),
  ].join(),
  [
    weak.getOrInsert(key, 1),
    weak.getOrInsert(key, 2),
    weak.getOrInsertComputed(Symbol('fresh'), function (made) { return typeof made; }),
  ].join(),
  typeof Object.getOwnPropertyDescriptor(RegExp.prototype, 'unicodeSets').get + ' ' + /a/u.unicodeSets + ' ' + /a/v.unicodeSets,
  typeof new FinalizationRegistry(function () {}).cleanupSome,
].join(' / ');
