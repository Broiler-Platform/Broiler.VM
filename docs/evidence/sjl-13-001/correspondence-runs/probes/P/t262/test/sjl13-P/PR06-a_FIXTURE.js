// sjl13 P GAP probe PR06: no top-level await here; the await is the arrow's own.
globalThis.sjl13log = ["a"];
Promise.resolve().then(function () { globalThis.sjl13log.push("tick"); });
export var f = async () => { await 0; };
