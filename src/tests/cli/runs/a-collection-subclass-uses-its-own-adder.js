// A COLLECTION'S CONSTRUCTOR CALLS THE ADDER OF THE OBJECT IT BUILDS (JSP-5, JSC-237). The object is
// made from `new.target` first, so a subclass's own `set` or `add` is the one read; it was built
// from the intrinsic prototype and re-pointed afterwards, so the intrinsic's adder ran and the
// subclass's never did.
var log = [];

class Logged extends Map {
  set(key, value) {
    log.push('map ' + key);
    return super.set(key, value);
  }
}

class LoggedSet extends Set {
  add(value) {
    log.push('set ' + value);
    return super.add(value);
  }
}

class LoggedWeakMap extends WeakMap {
  set(key, value) {
    log.push('weakmap ' + value);
    return super.set(key, value);
  }
}

class LoggedWeakSet extends WeakSet {
  add(value) {
    log.push('weakset ' + typeof value);
    return super.add(value);
  }
}

var map = new Logged([[1, 'a'], [2, 'b']]);
var set = new LoggedSet([3]);
var weakMap = new LoggedWeakMap([[{}, 4]]);
var weakSet = new LoggedWeakSet([{}]);

log.push(map instanceof Logged && set instanceof LoggedSet && weakMap instanceof LoggedWeakMap &&
  weakSet instanceof LoggedWeakSet);
log.join(' / ');
