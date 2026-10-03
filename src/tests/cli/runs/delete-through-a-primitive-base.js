// `delete` CONVERTS ITS BASE WITH `ToObject` (JSP-4, JSC-236). A primitive is asked through its
// wrapper, so a String's own `length` and its indices are refused - `false` in sloppy code and a
// TypeError in strict code - where every delete through a primitive answered `true`. A nullish
// base is a TypeError before the key is converted, where `delete undefined.x` answered `true`.
var answers = [delete 'abc'.length, delete 'abc'[0], delete 'abc'[5], delete 'abc'.foo, delete (1).x];

function strictly() {
  'use strict';

  try {
    delete 'abc'.length;
    return 'deleted';
  } catch (error) {
    return error.constructor.name;
  }
}

answers.push(strictly());

var log = '';

try {
  delete undefined[{
    toString: function () {
      log += 'key ';
      return 'x';
    },
  }];
} catch (error) {
  log += error.constructor.name;
}

answers.push(log);

try {
  var nothing = null;
  delete nothing.x;
} catch (error) {
  answers.push(error.constructor.name);
}

answers.join(' / ');
