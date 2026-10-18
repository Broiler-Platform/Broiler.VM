// G10-G13: UTC offsets after a time, and the designator Z, in every goal. Z is refused where the goal's
// DateTimeUTCOffset is [~Z] (the plain types); an offset is optional except for Instant; hour 00-23, minute and
// second 00-59, at most nine fraction digits, consistent separators within the offset, ASCII sign and digits.
var offsets = ['Z', 'z', '+00:00', '-00:00', '-00', '-0000', '+01', '+0100', '+01:00', '+01:00:00', '+010000',
  '+01:00:00.5', '+01:00:00,5', '+010000.123456789', '+01:00:00.1234567890', '+01:0000', '+0100:00', '+24:00',
  '+23:60', '+23:59:60', '+23:59:59.999999999', '+1', '+001', '−01:00', '+٠١:٠٠', '+01:00Z',
  'Z+01:00', '+01:00:00.', '+01:00.5'];
offsets.forEach(function (o) {
  each(PLAIN, '2020-01-01T12:00' + o);
  each(['PlainTime'], '12:00' + o);
  k('ZonedDateTime.from(' + JSON.stringify('2020-01-01T12:00' + o + '[UTC]') + ', {offset: "ignore"})', function () {
    return Temporal.ZonedDateTime.from('2020-01-01T12:00' + o + '[UTC]', { offset: 'ignore' }).toString(); });
  each(['Instant'], '2020-01-01T12:00' + o);
});
// An offset with no time, and an offset on a date-only string.
each(ALL7, '2020-01-01+01:00');
each(ALL7, '2020-01-01Z');
each(ALL7, '2020-01-01+01:00[UTC]');
