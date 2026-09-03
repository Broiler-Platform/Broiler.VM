// A loop nothing can leave. Everything after it is unreachable INCLUDING THE PROGRAM TAIL,
// and this was recorded three times as the shape that could not be repaired: suppressing a
// tail would leave a function with no terminator.
//
// IT WAS WRONG ABOUT THIS ONE CASE. The verifier requires every REACHABLE PATH to end in a
// return, and a loop nothing leaves has no path that ends at all - the code finishes on a
// backward jump rather than by falling off the end. So the tail is suppressed and the program
// runs, forever, until it spends its instruction allowance. Exit 5 is a program that did not
// settle, which is what an infinite loop IS.
for (;;) {
  var x = 1;
}
