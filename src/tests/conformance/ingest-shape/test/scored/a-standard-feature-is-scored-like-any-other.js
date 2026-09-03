#!/usr/bin/env broiler-js
/*---
esid: sec-hashbang
description: >
    A test claiming a flag the feature list declares standard rather than proposed. The control
    for the two files under `excluded`: the exclusion has to be about which constructs are in the
    language, so a test claiming a published edition's construct is scored on the same terms as a
    test claiming nothing at all. It is raw for the two reasons that flag carries here: a positive
    test otherwise needs the implicit harness prelude, which this manifest cannot load, and a
    strict reading would prepend a prologue - and a hashbang is only a comment where it opens the
    source text.
flags: [raw]
features: [hashbang]
---*/
var a = 2;
a * 3;
