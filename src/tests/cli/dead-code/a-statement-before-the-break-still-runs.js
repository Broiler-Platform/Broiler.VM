// The other direction, pinned because a suppression that started one statement too early
// would pass the file beside this one and fail here.
var x = 0;
for (;;) {
  x = 9;
  break;
}
x;
