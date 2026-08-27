# Documentation-rule witnesses

Fixture records that the group E rules must reject. They are stored here, outside `docs/adr/`
and with a `.md.witness` extension, so the ADR loader never globs them into the real set.

They exist because an Active rule has to have rejected something. Before they were added, E1, E2
and E3 were registered Active on the strength of clean-direction assertions alone - the rule
agreed with a correct document and had never been shown disagreeing with a wrong one.
