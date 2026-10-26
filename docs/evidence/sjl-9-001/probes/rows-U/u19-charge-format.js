// vm-fuel: 200000000000
// Row U19: Broiler.VM's charges in Intl.DurationFormat (VM/JsRealm.DurationFormat.cs:124, one per part in
// formatToParts; :597, one per unit PartitionDurationFormatPattern visits), beside the number and list formats'
// own. Three programs differ only in the expression the last function evaluates: u19-charge-control.js (0),
// u19-charge-format.js (df.format(d)) and u19-charge-parts.js (df.formatToParts(d)); every line they print is the
// same. The smallest completing fuel of each is bisected (scripts/bisect-vm-fuel.sh).
var df = new Intl.DurationFormat('en', { style: 'short' });
var d = { years: 1, months: 2, weeks: 3, days: 4, hours: 5, minutes: 6, seconds: 7, milliseconds: 8, microseconds: 9, nanoseconds: 10 };
t('U19 parts of the call, counted outside it', function () { return df.formatToParts(d).length; });
t('U19 the call', function () { var r = df.format(d); return 0; });
