// THE DEFAULT INSTRUCTION ALLOWANCE IS REACHABLE BY ORDINARY CODE, and this is the program the host's
// usage text names when it says so. Three million additions spend the profile's default allowance,
// so the run ends with the allowance spent (exit 5), where the comparison engine completes it in about
// a second. The allowance is a declared property of this host, not a defect; what was missing until
// 2026-09-29 was a document saying that an unremarkable loop reaches it. With --fuel raised the same
// file answers the sum.
var total = 0;

for (var i = 0; i < 3000000; i++) {
  total += i;
}

total;
