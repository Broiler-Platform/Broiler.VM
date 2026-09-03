// The same repaired defect through `while` rather than `for`, so a fix that repaired one
// lowering and not the other is still caught. Here it was the back-edge alone that nothing
// reached.
while (true) {
  break;
}
