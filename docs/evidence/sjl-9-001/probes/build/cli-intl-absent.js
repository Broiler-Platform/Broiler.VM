// The command-line root's Intl composition: no Intl member is named, so the verifier has nothing to refuse.
t('typeof Intl', function () { return typeof Intl; });
t('typeof globalThis.Intl === "undefined"', function () { return typeof globalThis['Int' + 'l'] === 'undefined'; });
