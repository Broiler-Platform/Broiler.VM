// Z5: the primary identifiers each engine lists.
t('Intl.supportedValuesOf("timeZone").length', function () { return Intl.supportedValuesOf('timeZone').length; });
Intl.supportedValuesOf('timeZone').forEach(function (z) { out('primary ' + z); });
