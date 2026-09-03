/*---
esid: sec-declarative-environment-records-getbindingvalue-n-s
description: >
    The same proposed declaration form, in a shape the proposal accepts as syntax and rejects at
    run time. This profile refuses it while reading it, so the answer arrives from the wrong place
    entirely, and without the exclusion the case is scored as a FAILURE - a gap reported against
    a construct that is not in any edition this component targets. The two files together are why
    the exclusion is not a way of hiding failures: it removes one of each.
negative:
  phase: runtime
  type: ReferenceError
features: [explicit-resource-management]
---*/
{
  using a = a;
}
