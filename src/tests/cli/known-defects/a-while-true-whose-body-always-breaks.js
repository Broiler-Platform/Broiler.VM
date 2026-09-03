// The same defect through `while` rather than `for`, so a fix that repaired one lowering and
// not the other is caught. Here it is the back-edge alone that nothing reaches.
while (true) {
  break;
}
