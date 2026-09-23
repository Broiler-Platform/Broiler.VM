// The same promise on standard error: an uncaught error's message is guest text, and it reaches
// the diagnostic stream as UTF-8 rather than as whatever the console's code page can spell.
throw new RangeError("gr\u00f6\u00dfer als \u221e");
