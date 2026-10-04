// AN ERROR HAS A STACK (phase F3, JSD-0038, JSC-261): an own accessor, listed before `message`, not
// enumerable, rendered when first read as V8 renders it - the header `Error.prototype.toString`
// gives, then `    at name (place:line:column)` per frame, innermost first and at most ten. Places
// are reduced to the file's own name here so the answer does not depend on where the file is.
function base(line) { return line.replace(/\((?:[^()]*\/)?([^/()]+)\)$/, '($1)').replace(/at (?:[^ ]*\/)([^/ ]+)$/, 'at $1'); }
function frames(error, count) { return error.stack.split('\n').slice(0, count + 1).map(base).join(' > '); }

function inner() { return new Error('boom'); }
function outer() { return inner(); }
var e = outer();
var d = Object.getOwnPropertyDescriptor(e, 'stack');

class Failure extends Error { constructor(m) { super(m); } }
function make() { return new Failure('sub'); }

function read(o) { return o.missing.value; }
function caught() { try { read({}); } catch (x) { return x; } }

function deep(n) { return n === 0 ? new RangeError('deep') : deep(n - 1); }

var later = new Error('before'); later.message = 'after';
var assigned = new Error('a'); assigned.stack = 'replaced';

[typeof e.stack, 'stack' in e, Object.getOwnPropertyNames(e).join(),
 typeof d.get + '/' + typeof d.set + '/' + d.enumerable + '/' + d.configurable,
 frames(e, 2), frames(make(), 1), frames(caught(), 2), deep(30).stack.split('\n').length,
 later.stack.split('\n')[0], assigned.stack, Object.keys(e).length, JSON.stringify(e),
 'stack' in Error.prototype].join(' / ');
