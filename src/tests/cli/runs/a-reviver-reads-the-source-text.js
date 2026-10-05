// `JSON.parse` HANDS A REVIVER A THIRD ARGUMENT, A CONTEXT OBJECT (JSP-7, JSC-239): its `source` is the
// text a primitive was read from, exactly as written, while the value is still the one the parse
// produced. An object or an Array has no `source`, and neither has a value a reviver replaced first.
var seen = [];

JSON.parse(' { "a": 1.50, "b": ["\\u0041", -0, 1e3], "c": 2 } ', function (key, value, context) {
  if (key === 'a') {
    this.c = 3;
  }

  seen.push(key + '=' + (context && 'source' in context ? context.source : '-'));
  return value;
});

seen.join(' ');
