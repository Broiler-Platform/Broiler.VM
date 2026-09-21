"use strict";
// Script goal and a preserved directive prologue; no comparison-engine exemptions.
print("1 " + (6 * 7));
print("2 Grüße");
print("3 " + ((function () { return this; })() === undefined));
