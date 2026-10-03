// `08` is a NonOctalDecimalIntegerLiteral, which strict code forbids as it forbids a legacy octal.
// Until 2026-10-03 it was admitted there (JSC-253).
"use strict";
08;
