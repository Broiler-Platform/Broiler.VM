# Architecture-rule witnesses

Each file here is a deliberately invalid project file that exactly one group A rule must
reject. They are named `*.csproj.witness` so MSBuild never globs them into the build, and they
exist so that "the rule is expressed" can be replaced by "the rule rejected this".

A rule with no witness that it rejects is registered as `Vacuous` or `Deferred` in
`../rules.register.json`, never as `Active`. `RuleRegisterTests` enforces that correspondence in
both directions, so a rule cannot quietly lose its witness and keep its status.
