// G47-G48: decimal digits other than ASCII (Arabic-Indic U+0660-U+0669, fullwidth U+FF10-U+FF19), and the sign
// U+2212 MINUS SIGN, in each parser; the error's type and message. The grammar's digits are ASCII
// (DecimalDigit), its signs ASCIISign (U+2212 left the grammar with the proposal's ae9c7855, 2024-07-04).
var AI = function (s) { return s.replace(/[0-9]/g, function (c) { return String.fromCharCode(0x0660 + +c); }); };
var FW = function (s) { return s.replace(/[0-9]/g, function (c) { return String.fromCharCode(0xFF10 + +c); }); };
var cases = [
  ['PlainDate', '2020-01-01'], ['PlainDateTime', '2020-01-01T12:00'], ['PlainTime', '12:30'], ['PlainYearMonth', '2020-01'],
  ['PlainMonthDay', '01-01'], ['ZonedDateTime', '2020-01-01T12:00[UTC]'], ['Instant', '2020-01-01T12:00Z'], ['Duration', 'P1D']];
cases.forEach(function (c) {
  t1(c[0] + '.from(Arabic-Indic ' + JSON.stringify(AI(c[1])) + ')', function () { return FROM[c[0]](AI(c[1])); });
  t1(c[0] + '.from(fullwidth ' + JSON.stringify(FW(c[1])) + ')', function () { return FROM[c[0]](FW(c[1])); });
});
// Digits other than ASCII in one part only.
t1('PlainDate.from("2020-01-0١")', function () { return Temporal.PlainDate.from('2020-01-0١').toString(); });
t1('PlainDateTime.from("2020-01-01T12:0٠")', function () { return Temporal.PlainDateTime.from('2020-01-01T12:0٠').toString(); });
t1('PlainDateTime.from("2020-01-01T12:00:00.١")', function () { return Temporal.PlainDateTime.from('2020-01-01T12:00:00.١').toString(); });
t1('Instant.from("2020-01-01T12:00+0١:00")', function () { return Temporal.Instant.from('2020-01-01T12:00+0١:00').toString(); });
t1('ZonedDateTime.from("2020-01-01T12:00+0١:00[UTC]")', function () { return Temporal.ZonedDateTime.from('2020-01-01T12:00+0١:00[UTC]').toString(); });
t1('PlainDate.from("2020-01-01[u-ca=١]")', function () { return Temporal.PlainDate.from('2020-01-01[u-ca=١]').toString(); });
t1('PlainDate.from("2020-01-01[١=a]")', function () { return Temporal.PlainDate.from('2020-01-01[١=a]').toString(); });
t1('Duration.from("PT1.٥S")', function () { return Temporal.Duration.from('PT1.٥S').toString(); });
// U+2212 in the year, the offset and the duration sign, in each parser.
var M = '−';
[['PlainDate', M + '002020-01-01'], ['PlainDateTime', M + '002020-01-01T00:00'], ['PlainYearMonth', M + '002020-01'],
 ['PlainMonthDay', M + '002020-01-01'], ['ZonedDateTime', M + '002020-01-01T00:00[UTC]'], ['Instant', M + '002020-01-01T00:00Z'],
 ['PlainDate', '2020-01-01T00:00' + M + '01:00'], ['PlainDateTime', '2020-01-01T00:00' + M + '01:00'], ['PlainTime', '00:00' + M + '01:00'],
 ['PlainYearMonth', '2020-01-01T00:00' + M + '01:00'], ['PlainMonthDay', '2020-01-01T00:00' + M + '01:00'],
 ['ZonedDateTime', '2020-01-01T00:00' + M + '01:00[UTC]'], ['Instant', '2020-01-01T00:00' + M + '01:00'],
 ['ZonedDateTime', '2020-01-01T00:00[' + M + '01:00]'], ['Duration', M + 'P1D']
].forEach(function (c) { t1(c[0] + '.from(' + JSON.stringify(c[1]) + ')', function () { return FROM[c[0]](c[1]); }); });
t1('new ZonedDateTime(0n, "−01:00")', function () { return new Temporal.ZonedDateTime(0n, M + '01:00').toString(); });
