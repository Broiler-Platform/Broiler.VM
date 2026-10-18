// D1: DurationSign, blank, negation and abs, and the record's zeros.
t('new Duration(0,0,0,0,0,0,0,0,0,-1).sign', function () { return new T.Duration(0, 0, 0, 0, 0, 0, 0, 0, 0, -1).sign; });
t('new Duration().sign', function () { return new T.Duration().sign; });
t('new Duration().blank', function () { return new T.Duration().blank; });
t('Duration("P1D").negated()', function () { return D('P1D').negated(); });
t('Object.is(Duration("P1D").negated().hours, 0)', function () { return Object.is(D('P1D').negated().hours, 0); });
t('Object.is(new Duration().negated().years, 0)', function () { return Object.is(new T.Duration().negated().years, 0); });
t('Duration("-P1DT1H").abs()', function () { return D('-P1DT1H').abs(); });
t('Duration("P1D").with({hours: -1})', function () { return D('P1D').with({ hours: -1 }); });
t('Duration("P1D").with({days: -1}).sign', function () { return D('P1D').with({ days: -1 }).sign; });
t('Duration("P1D").with({})', function () { return D('P1D').with({}); });
