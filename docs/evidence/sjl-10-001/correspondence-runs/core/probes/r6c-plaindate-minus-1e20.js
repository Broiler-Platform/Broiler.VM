// R6, one case per probe: a Broiler.VM host exception ends the whole invocation, so each case that
// raises one runs alone.
t('new PlainDate(-1e20, 1, 1)', function () { return new T.PlainDate(-1e20, 1, 1); });
