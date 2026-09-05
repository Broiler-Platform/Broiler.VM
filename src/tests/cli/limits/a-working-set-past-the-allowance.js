// A PROGRAM WHOSE RETAINED WORKING SET IS LARGER THAN THE PROFILE'S DEFAULT ALLOWANCE, which is the
// shape `--live-bytes` sizes.
//
// It allocates BUFFERS rather than arrays or strings, and that is not incidental. `LiveBytes` is a
// ceiling that is never released, so this profile charges it only for allocations retained for as
// long as the object holding them is - a buffer's bytes, a keyed collection's entries, a promise's
// reactions. Transient allocation is bounded by the instruction allowance instead, which is the
// correction the RayTrace benchmark forced: reporting an argument list as retained reached a
// ceiling for memory the program had already given back. So an ordinary Array or String growing
// without bound is bounded here by fuel and by the wall clock, and not by this dimension.
//
// The Octane `zlib` benchmark is the workload that made the option necessary. It printed its score
// and then met the profile's default, so the process exited non-zero on a run that had produced
// exactly what was asked of it.

var held = [];

for (var index = 0; index < 40; index++) {
  held.push(new Uint8Array(1024 * 1024));
}

print("held " + held.length);
