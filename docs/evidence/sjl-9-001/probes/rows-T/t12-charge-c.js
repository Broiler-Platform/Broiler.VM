// vm-fuel: 200000000000
// SJL-9.1 sub-area (d), row T46: Broiler.VM's charge for a date format. JsDateTimeFormatter.Parts charges the
// output's length and DatePartsArray one per part. t12-charge-a, -b and -c build the same format and print the
// same line; they differ only in the instant formatted (a: "Friday, May 3, 2024 at 1:00:00 AM Coordinated
// Universal Time", 60 code units; b: "Wednesday, September 25, 2024 at 11:00:00 AM Coordinated Universal Time",
// 71) and, for c, in going through formatToParts. The smallest completing fuel of each is bisected
// (work/bisect-vm-fuel.sh); Broiler.JS has no fuel, so its run only prints the line.
var f = new Intl.DateTimeFormat('en', { dateStyle: 'full', timeStyle: 'full', timeZone: 'UTC' });
var r = f.formatToParts(Date.UTC(2024, 8, 25, 11, 0, 0));
out('formatted');
