// SJL-13.1's correspondence, sub-area P, row P16b (added 2026-10-27): value probes. Each line names its probe, and
// PP<nn> is the parse probe of probe-P16b.manifest that holds the same head. A source is a single-quoted string here,
// and \n puts a line feed into it.
t('PE01 PP11 var, a line break for the first semicolon', function () { return ev('for (var i = 0\ni < 3; i++) ;\ni'); });
t('PE02 PP12 let, a line break for the first semicolon', function () { return ev('var n = 0;\nfor (let i = 0\ni < 3; i++) n++;\nn'); });
t('PE03 PP13 const, a line break for the first semicolon', function () { return ev('var n = 0;\nfor (const i = 0\ni < 3; i++) n++;\nn'); });
t('PE04 PP14 an expression, a line break for the first semicolon', function () { return ev('var i;\nfor (i = 0\ni < 3; i++) ;\ni'); });
t('PE05 PP15 a line break for the second semicolon', function () { return ev('for (var i = 0; i < 3\ni++) ;\ni'); });
t('PE06 PP16 control, a line break in a for-in head', function () { return ev('var r = "";\nfor (var k\nin {a: 1}) r += k;\nr'); });
t('PE07 PP17 control, a line break in a for-of head', function () { return ev('var s = 0;\nfor (var v\nof [1, 2]) s += v;\ns'); });
t('PE08 PP18 control, PE01 with no line break', function () { return ev('for (var i = 0 i < 3; i++) ;\ni'); });
t('PE09 PP19 control, PE01 with its semicolon before the line break', function () { return ev('for (var i = 0;\ni < 3; i++) ;\ni'); });
