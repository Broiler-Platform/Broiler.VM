// R6, one case per probe: a Broiler.VM host exception ends the whole invocation, so each case that
// raises one runs alone.
t('new PlainMonthDay(1, 1, undefined, 1e20)', function () { return new T.PlainMonthDay(1, 1, undefined, 1e20); });
