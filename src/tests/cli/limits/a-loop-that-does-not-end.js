// A program with no exit. It ends by spending its instruction allowance, in a bounded
// number of instructions rather than in a number of seconds, so this file decides the
// same way on a busy machine.
var i = 0;
while (true) {
  i = i + 1;
}
