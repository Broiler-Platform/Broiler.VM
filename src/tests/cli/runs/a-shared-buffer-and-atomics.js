// SharedArrayBuffer and Atomics in one agent (JSD-0041, JSC-266). Every Atomics member answers the
// element's old value or the converted operand, narrowed to the element; this host's agent may not
// block, so Atomics.wait refuses, while Atomics.waitAsync's timeout is settled by the host's drain.
var sab = new SharedArrayBuffer(16);
var i32 = new Int32Array(sab);
var rows = [];

function ask(name, f) {
  try { rows.push(name + " " + f()); }
  catch (e) { rows.push(name + " threw " + e.name); }
}

ask("a shared buffer", function () {
  return [Object.prototype.toString.call(sab), sab.byteLength, sab.growable].join(" ");
});
ask("add then load", function () { return Atomics.add(i32, 0, 5) + " " + Atomics.load(i32, 0); });
ask("compareExchange", function () { return Atomics.compareExchange(i32, 0, 5, 9) + " " + i32[0]; });
ask("narrowed to the element", function () {
  var i8 = new Int8Array(sab);
  return [Atomics.store(i8, 8, 300), i8[8], Atomics.compareExchange(i8, 8, 300, 1), i8[8]].join(" ");
});
ask("BigInt64", function () {
  var b = new BigInt64Array(sab);
  return [Atomics.store(b, 1, -5n), Atomics.add(b, 1, 2n), b[1]].join(" ");
});
ask("notify with no waiter", function () { return Atomics.notify(i32, 0); });
ask("wait", function () { return Atomics.wait(i32, 0, 9, 0); });
ask("an ArrayBuffer member on it", function () { return ArrayBuffer.prototype.slice.call(sab); });
ask("grow", function () {
  var g = new SharedArrayBuffer(4, { maxByteLength: 8 });
  var view = new Uint8Array(g);
  g.grow(8);
  return [g.byteLength, view.length].join(" ");
});

var r = Atomics.waitAsync(i32, 0, 9, 10);
rows.push("waitAsync " + r.async);
r.value.then(function (v) { print("settled at the drain " + v); });
rows.join("\n");
