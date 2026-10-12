// TWO NUMBER LITERALS ONE UNIT IN THE LAST PLACE APART: 2**-25 and the double below it. The wide
// compiler interned a Number constant under the platform's "R" text of its value, and both of these
// render 2.980232238769531E-08 there, so the two literals were one constant and compared equal. The
// pool keys a Number by its bits now, as the slice builder always did (milestone SJL-3.4).
var high = 2.9802322387695312e-8;
var low = 2.980232238769531e-8;
[high === low, String(high), String(low)].join(" ");
