// R6, one case per probe: a Broiler.VM host exception ends the whole invocation, so each case that
// raises one runs alone.
t('new PlainDate(2**53 + 2, 1, 1)', function () { return new T.PlainDate(Math.pow(2, 53) + 2, 1, 1); });
