// `Uint8Array`'s BASE64 AND HEX MEMBERS (JSP-7, JSC-239), which the edition defines and the realm
// lacked. Encoding with and without padding and in the URL alphabet; decoding with whitespace, each
// last-chunk rule and a decode into a short array that stops before a chunk it cannot finish; and
// a decoding error, which writes what it decoded before it throws.
var bytes = new Uint8Array([72, 105, 251, 255]);

function failure(work) {
  try {
    work();
    return 'decoded';
  } catch (error) {
    return error.constructor.name;
  }
}

var into = new Uint8Array(4);
var partial = into.setFromBase64('SGk= ', {});
var short = new Uint8Array(2);
var stopped = short.setFromBase64('SGkh');
var written = new Uint8Array(3);
var spoiled = failure(function () { written.setFromHex('4869zz'); });

[
  bytes.toBase64(),
  bytes.toBase64({ alphabet: 'base64url', omitPadding: true }),
  bytes.toHex(),
  Array.from(Uint8Array.fromBase64(' SGk h ')).join(),
  Array.from(Uint8Array.fromBase64('SGk', { lastChunkHandling: 'stop-before-partial' })).join(),
  failure(function () { Uint8Array.fromBase64('SGk', { lastChunkHandling: 'strict' }); }),
  Array.from(Uint8Array.fromHex('4869')).join(),
  partial.read + ':' + partial.written + ':' + into.join(),
  stopped.read + ':' + stopped.written + ':' + short.join(),
  spoiled + ':' + written.join(),
].join(' / ');
