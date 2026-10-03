// `Error.prototype.stack` IS DECLINED BY NAME (JSP-7, JSC-239): it is not a member of the edition, and
// section 6 of the roadmap names it with the answer a program meets, which is this one - no own
// `stack` on an error and nothing it inherits, so reading it answers `undefined`.
var error = new Error('x');

[typeof error.stack, 'stack' in error, Object.getOwnPropertyNames(error).join()].join(' / ');
