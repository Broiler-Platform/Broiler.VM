// `yield` AND `await` ARE LABELS WHERE THEY ARE NAMES - `yield` in sloppy code outside a generator,
// `await` in a script outside an async function and a static block - and a jump reaches a label by
// any name a label may have, the contextual keywords included. Until 2026-09-30 `yield:` and
// `await:` were refused as a missing semicolon, and so was `break of` under an `of:` this host
// admitted.
var trace = [];

yield: for (var index = 0; index < 3; index++) {
  if (index === 1) {
    continue yield;
  }
  trace.push('yield ' + index);
}

await: {
  trace.push('await');
  break await;
}

of: for (;;) {
  trace.push('of');
  break of;
}

trace.join(', ');
