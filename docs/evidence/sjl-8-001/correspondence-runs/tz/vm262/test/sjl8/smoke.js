/*---
description: smoke
flags: [raw]
---*/
var out = [];
out.push(typeof Temporal, typeof Intl);
out.push(new Temporal.ZonedDateTime(0n, "Europe/Berlin").toString());
throw new Error("OUT<<" + out.join("\n") + ">>OUT");
