#!/usr/bin/env bash
# Run one command of VM-5-002's re-collection with its identity lines written first, by this driver.
#   run-logged.sh <log> <build> <tree> [bin ...] -- <command ...>
# The command runs with <tree> as its working directory. The log gets the identity lines of
# identity.py, the command, its whole output, its exit code and the finish time. Stops, writing the
# refusal to the log, when the main checkout is not on the branch this work runs on.
set -u
HERE=$(cd "$(dirname "$0")" && pwd)
LOG=$1; BUILD=$2; TREE=$3; shift 3
BINS=()
# A relative binary path is relative to <tree>, the directory the command runs in. (The first E2 credit
# run of the re-collection resolved it against the caller's directory instead; see r2/e2.)
while [ $# -gt 0 ] && [ "$1" != "--" ]; do
  case "$1" in /*|[A-Za-z]:*) BINS+=(--bin "$1") ;; *) BINS+=(--bin "$TREE/$1") ;; esac
  shift
done
shift
export DOTNET_CLI_UI_LANGUAGE=en
mkdir -p "$(dirname "$LOG")"
python "$HERE/identity.py" --build "$BUILD" --tree "$TREE" "${BINS[@]}" > "$LOG"
if [ $? -ne 0 ]; then cat "$LOG"; echo "STOPPED: branch check failed" >&2; exit 97; fi
printf '# command=%s\n' "$*" >> "$LOG"
( cd "$TREE" && "$@" ) >> "$LOG" 2>&1
code=$?
printf '# exit=%s\n# finished=%s\n' "$code" "$(date -Iseconds)" >> "$LOG"
exit $code
