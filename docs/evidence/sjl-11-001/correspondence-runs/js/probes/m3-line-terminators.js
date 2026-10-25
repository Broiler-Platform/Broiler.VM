// M3 (bundle sjl-7-001's W5, carried to SJL-11): the four line terminators LF, CR, LS and PS for
// ^ and $ under m, for $ without m, and for the dot; U+0085 is none of them.
var subjects = ['a\nb', 'a\rb', 'a\u2028b', 'a\u2029b', 'a\r\nb', 'a\u0085b'];
for (var i = 0; i < subjects.length; i++) {
  (function (s) {
    t('/^b/m on ' + q(s), function () { return m(/^b/m, s); });
    t('/^b/gm on ' + q(s), function () { return m(/^b/gm, s); });
    t('/a$/m on ' + q(s), function () { return m(/a$/m, s); });
    t('/a$/gm on ' + q(s), function () { return m(/a$/gm, s); });
    t('/^b/mu on ' + q(s), function () { return m(/^b/mu, s); });
    t('/a$/mu on ' + q(s), function () { return m(/a$/mu, s); });
    t('/a.b/ on ' + q(s), function () { return m(/a.b/, s); });
    t('/a.b/u on ' + q(s), function () { return m(/a.b/u, s); });
    t('/a.b/s on ' + q(s), function () { return m(/a.b/s, s); });
    t('/(?m:^b)/ on ' + q(s), function () { return m(/(?m:^b)/, s); });
  })(subjects[i]);
}
t('/a$/ on "a\\n"', function () { return m(/a$/, 'a\n'); });
t('/a$/ on "a\\r"', function () { return m(/a$/, 'a\r'); });
t('/a$/u on "a\\n"', function () { return m(/a$/u, 'a\n'); });
t('/a$/g on "a\\n"', function () { return m(/a$/g, 'a\n'); });
t('/^b/ on "a\\nb"', function () { return m(/^b/, 'a\nb'); });
t('/a$/m on "a\\n"', function () { return m(/a$/m, 'a\n'); });
t('/[^]$/ on "a\\n"', function () { return m(/[^]$/, 'a\n'); });
t('"a\\nb\\u2028c".split(/$/m)', function () { return 'a\nb\u2028c'.split(/$/m); });
t('"a\\nb\\u2028c".replace(/^/gm, ">")', function () { return 'a\nb\u2028c'.replace(/^/gm, '>'); });
