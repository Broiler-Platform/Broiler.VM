#!/bin/bash
# Usage: run-witness.sh <node binary> <rows-dir> <probe name>...
# The built program (built/<name>.js, as run-js.sh wrote it) under a Node.js witness, never the arbiter of a class:
# Node v26.12.0 (ICU 78.3, CLDR 48.0), the reference bundle sjl-9-001's reference.txt names (its tarball's SHA-256
# checked against that file), with print defined as console.log. No LANG, LC_ALL or LANGUAGE; TZ=UTC.
# Writes <rows-dir>/runs/<name>.node26.txt.
set -u
NODE=$1; shift
ROWS=$(cd "$1" && pwd); shift
for name in "$@"; do
  out=$ROWS/runs/$name.node26.txt
  { echo "# command (from <rows>/): env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC node -e 'globalThis.print = console.log; (0, eval)(require(\"fs\").readFileSync(\"built/$name.js\", \"utf8\"))'"
    echo "# node: $($NODE -p 'process.versions.node + " icu " + process.versions.icu + " cldr " + process.versions.cldr')"
    echo "# probe: built/$name.js sha256 $(sha256sum "$ROWS/built/$name.js" | cut -c1-64)"; } > "$out"
  ( cd "$ROWS" && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC $NODE -e "globalThis.print = console.log; (0, eval)(require('fs').readFileSync('built/$name.js', 'utf8'))" >> "$out" 2>&1; echo "exit=$?" >> "$out" )
done
