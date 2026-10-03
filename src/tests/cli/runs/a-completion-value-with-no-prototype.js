// A script whose completion value has no `toString` to call completed normally, and the host
// renders the value by its class tag. Until 2026-10-03 it was reported as an uncaught TypeError the
// script never threw (JSC-252). The comparison engine prints its own inspection of the object.
Object.create(null);
