/*---
esid: sec-let-and-const-declarations
description: >
    A declaration form the feature list calls a proposal, in a shape that is an error under the
    proposal too. Both this front end and the proposal refuse it, for reasons that have nothing
    to do with each other: the proposal because the form requires an initialiser, this profile
    because it has no production for the form at all and refuses every spelling of it. Without
    the exclusion the two refusals agree on the observable outcome and the case is scored as a
    PASS - a credit for a construct nothing here implements.
negative:
  phase: parse
  type: SyntaxError
features: [explicit-resource-management]
---*/
using a;
