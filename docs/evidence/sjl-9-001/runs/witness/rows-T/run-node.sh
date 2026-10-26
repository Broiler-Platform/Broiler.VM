#!/bin/bash
# Usage: run-node.sh <rows-dir> <probe name>...   The witness: Node (never the arbiter of a class) runs
# <rows-dir>/built/<name>.js (the program the engines ran, prelude and epilogue included) by indirect eval
# with print bound to stdout, TZ=UTC, no LANG; writes <rows-dir>/runs/<name>.node.txt.
set -u
ROWS=$(cd "$1" && pwd); shift
for name in "$@"; do
  out=$ROWS/runs/$name.node.txt
  {
    echo "# command (from <rows>/): env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 300 node -e '<print = stdout line; indirect eval of built/$name.js>'"
    echo "# revision: node $(node --version); ICU $(node -p process.versions.icu); CLDR $(node -p process.versions.cldr); tz $(node -p process.versions.tz)"
    echo "# probe: built/$name.js sha256 $(sha256sum "$ROWS/built/$name.js" | cut -c1-64)"
  } > "$out"
  ( cd "$ROWS" && env -u LANG -u LC_ALL -u LANGUAGE TZ=UTC timeout 300 node -e 'globalThis.print = function (s) { process.stdout.write(String(s) + "\n"); }; (0, eval)(require("fs").readFileSync(process.argv[1], "utf8"));' "built/$name.js" >> "$out" 2>&1; echo "exit=$?" >> "$out" )
done
