// A CALL AS AN ASSIGNMENT TARGET IN NON-STRICT CODE IS A RUN-TIME ReferenceError (JSP-7, JSC-239),
// which is Annex B's: the call runs, its result is not converted, the right-hand side never runs,
// and the write throws. `=`, a compound operator, both updates and a `for … in` head each do it; the
// program was refused before it ran.
var log = [];

function f() {
  log.push('f');
  return {
    valueOf: function () {
      log.push('valueOf');
      return 1;
    },
  };
}

function g() {
  log.push('g');
  return 1;
}

var forms = [
  function () { f() = g(); },
  function () { f() += g(); },
  function () { f()++; },
  function () { --f(); },
  function () { for (f() in { key: 1 }) {} },
];

forms.map(function (form) {
  log = [];

  try {
    form();
    return 'completed';
  } catch (error) {
    return error.constructor.name + ':' + log.join(',');
  }
}).join(' / ');
