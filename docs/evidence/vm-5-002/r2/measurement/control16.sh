#!/usr/bin/env bash
# Negative control 16 of eng/collect-evidence.py ("a runtime stores a capability depth of zero instead
# of releasing it"), run by hand at the remedy head in a worktree outside the repository, as the
# collector runs it: the collector's own build step first (the assembly-reading architecture rules read
# every shipping project's bin/Release), then the injection, the whole solution tested, the tree
# restored, a restore, and the whole solution tested again. Each transcript begins with identity lines.
#   control16.sh <worktree> <outdir>
set -u
HERE=$(cd "$(dirname "$0")" && pwd)
WT=$1; OUT=$2
TB=src/tests/Broiler.VM.Contract.Tests/bin/Release/net10.0
AB=src/tests/Broiler.VM.Architecture.Tests/bin/Release/net10.0
export DOTNET_CLI_UI_LANGUAGE=en
mkdir -p "$OUT"
cd "$WT" || exit 1
git checkout -q -- src/Broiler.VM.Runtime

"$HERE/run-logged.sh" "$OUT/control16-build.log" "wit2 clean, the collector's build step" "$WT" -- \
  dotnet build Broiler.VM.slnx -c Release --no-incremental -warnaserror || { echo "build failed"; exit 1; }

python - "$WT" > "$OUT/control16.apply.txt" <<'PY'
import io, sys
path = sys.argv[1] + "/src/Broiler.VM.Runtime/VmRuntime.cs"
text = io.open(path, encoding="utf-8", newline="").read()
old = "            inCapabilityDepth.Value = depth > 0 ? depth : null;"
new = "            inCapabilityDepth.Value = depth;"
assert old in text, "control 16: injection point not found"
mutated = text.replace(old, new, 1)
assert mutated != text, "control 16 changed nothing"
io.open(path, "w", encoding="utf-8", newline="").write(mutated)
print("control 16 injected, the collector's injection text")
PY
git diff -- src/Broiler.VM.Runtime >> "$OUT/control16.apply.txt"

# dotnet test builds the injected tree first; the identity lines are written before that build, so
# their digests are of the clean build, and the transcript's build lines show the recompile.
"$HERE/run-logged.sh" "$OUT/control16-injected.log" "wit2 with control 16 injected (digests read before the test's own build)" "$WT" \
  $TB/Broiler.VM.Runtime.dll $TB/Broiler.VM.Contract.Tests.dll $AB/Broiler.VM.Architecture.Tests.dll -- \
  dotnet test Broiler.VM.slnx -c Release
echo "injected exit=$?"

git checkout -q -- src/Broiler.VM.Runtime
echo "# after revert: git status --porcelain --untracked-files=no -> [$(git status --porcelain --untracked-files=no)]" >> "$OUT/control16-injected.log"
"$HERE/run-logged.sh" "$OUT/control16-restore.log" "wit2 after control 16 reverted" "$WT" -- dotnet restore Broiler.VM.slnx
"$HERE/run-logged.sh" "$OUT/control16-reverted.log" "wit2 after control 16 reverted (digests read before the test's own build)" "$WT" \
  $TB/Broiler.VM.Runtime.dll $TB/Broiler.VM.Contract.Tests.dll $AB/Broiler.VM.Architecture.Tests.dll -- \
  dotnet test Broiler.VM.slnx -c Release
echo "reverted exit=$?"

for f in control16-injected control16-reverted; do
  echo "$f: $(grep -E '^(Passed!|Failed!)' "$OUT/$f.log" | tr '\n' ' ')"
  grep -E "^\s+Failed Broiler" "$OUT/$f.log" | sed -E 's/ \[[^]]*\]$//'
done
