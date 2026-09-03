// A DIFFERENT SHAPE, and the one the compiler own remark originally named: this loop has no
// exit, so everything after it is unreachable INCLUDING THE PROGRAM TAIL. Suppressing the
// tail would leave a function with no terminator, so unlike the two files beside it this may
// genuinely be the format answer rather than something the lowering can fix. It is pinned
// separately for that reason: the two defects should not be repaired by one change without
// somebody deciding this one.
for (;;) {
  var x = 1;
}
