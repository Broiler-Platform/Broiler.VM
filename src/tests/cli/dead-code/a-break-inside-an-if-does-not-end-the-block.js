// An `if` with no `else` does not end the block: the test may be false and the branch not
// taken. A suppression that treated it as terminating would drop the counter below.
var n = 0;
for (var i = 0; i < 5; i = i + 1) {
  if (i > 1) { break; }
  n = n + 1;
}
n;
