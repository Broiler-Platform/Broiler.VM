// THE BINARY SURFACE, EXERCISED WHERE IT IS EXOTIC RATHER THAN WHERE IT IS ORDINARY.
//
// A typed array is an integer-indexed exotic object, and every line below is one of the places
// that is observable: an out-of-range read is `undefined` and does NOT reach the prototype chain
// even when something was put there; an out-of-range write is discarded rather than creating an
// ordinary property; a write converts through `ToNumber`, which for an object means calling its
// `valueOf`; and a detached buffer takes every element and every own key with it.
//
// `SharedArrayBuffer` and `Atomics` are absent DELIBERATELY - they are the multi-agent surface and
// they need the agent model, and folding them into an ordinary byte buffer's identity would let a
// composition that wanted one admit cross-agent shared memory by accident. The line that asserts
// their absence is the one that would go red if that ever stopped being true.

var bytes = new Uint8Array(4);
bytes[0] = 255;
bytes[1] = 256;
bytes[2] = -1;
bytes[3] = 3.7;
print(bytes.length + ":" + bytes[0] + "," + bytes[1] + "," + bytes[2] + "," + bytes[3]);

Object.prototype[9] = "inherited";
print(String(bytes[9]) + ":" + ("9" in bytes));
bytes[9] = 1;
print(bytes.length + ":" + Object.keys(bytes).join(","));

var converted = new Int32Array(1);
converted[0] = { valueOf: function () { return 7.9; } };
print(converted[0]);

var clamped = new Uint8ClampedArray(4);
clamped[0] = 0.5;
clamped[1] = 1.5;
clamped[2] = 2.5;
clamped[3] = 300;
print(clamped[0] + "," + clamped[1] + "," + clamped[2] + "," + clamped[3]);

var buffer = new ArrayBuffer(8);
var view = new DataView(buffer);
view.setInt32(0, 0x01020304);
print(view.getInt32(0) + ":" + view.getInt32(0, true) + ":" + view.getUint8(0));

var words = new Uint32Array(buffer);
print(words.length + ":" + words[0]);

var live = new Uint8Array(buffer);
var moved = buffer.transfer();
print(live.length + ":" + String(live[0]) + ":" + buffer.byteLength + ":" + moved.byteLength);

try {
  new Uint8Array(buffer);
  print("a detached buffer was accepted, which it must not be");
} catch (detached) {
  print(detached.name);
}

print(typeof SharedArrayBuffer + ":" + typeof Atomics + ":" + typeof BigInt64Array);

"typed-array-over-a-buffer ok";
