// vm-fuel: 200000
// B7: compiling a large v-mode class under i, fifty property escapes, under a fuel allowance of
// 200,000 in Broiler.VM: whether compilation is charged by its expansion or by the source length.
var p = '[' + '\\p{L}'.repeat(50) + ']';
t('new RegExp([\\p{L} x50], "vi").test("\\u00e9")', function () { return new RegExp(p, 'vi').test('\u00e9'); });
var e = '[' + '\\p{RGI_Emoji}'.repeat(20) + ']';
t('new RegExp([\\p{RGI_Emoji} x20], "v").test("\\u231a")', function () { return new RegExp(e, 'v').test('\u231a'); });
