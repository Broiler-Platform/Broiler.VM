#!/bin/bash
# run_p16b.sh - the probes of 2026-10-27 for rows P16b, T24b and T24f of SJL-13.1's correspondence (the owner's
# decisions of that day). It takes them as run_all.sh, beside this file and unchanged, took the probes of 2026-10-26:
# the same heads, the same composition roots and tools, the same environment, the same answer files and the same
# rewrite of local paths; it runs only the new probes, from probe-P16b.manifest, T/parse/t24-end.manifest,
# P/value/p-values.js and T/value/t24-values.js, whose expected answers are plan-p16b.tsv.
#
# Serially:
#   1. Broiler.VM: a scratch worktree $VMW at 4cf9ca4 (git worktree add --detach), one Release build of the two
#      composition roots these probes use (SliceCompiler, Conformance), every Broiler.VM probe; then the worktree's
#      bin/ and obj/ are deleted and the worktree removed.
#   2. Broiler.JS: the same with $JSW at d6795fc8 (the shell BroilerJS and the SJL-12 parse-corpus tool, one build
#      tree), every Broiler.JS probe; then bin/ and obj/ deleted and the worktree removed.
# Each answer is runs/<engine>/<probe-id>.txt (a header naming the command, then the probe's output); whole reports,
# transcripts and logs are under runs/<engine>/reports/ and runs/<engine>/logs/, the programs a value probe ran under
# runs/<engine>/built/. The names differ from run_all.sh's wherever a file of the same kind exists in the bundle
# (p16b-revision.txt, p16b-build.txt, logs/p16b-*.log), so the answers join correspondence-runs/vm/ and js/ beside the
# earlier ones, and each folder's hashes.txt is then written again over every file it holds. Every scratch or
# worktree path in runs/<engine>/ is rewritten to a placeholder (<corr>, <probes>, <runs>, <work>, <vm>, <js>,
# <scratch>) before hashing.
#
# Rules kept: builds are Release with --disable-build-servers; nothing in $VM_REPO or $JS_REPO is edited (worktrees
# are added detached and removed); no pkill or killall; every probe runs under TZ=UTC with LANG, LANGUAGE, every LC_*
# and every BROILER_* variable unset, and under timeout. Exit codes of probes are recorded, never fatal.
#
# Usage: bash run_p16b.sh [all|vm|js]   (default all; a phase refuses to start while runs/<engine>/ holds anything).
#        KEEP=1 keeps the worktrees and builds. SCRATCH names the folder holding sjl-rest/sjl13-corr-p16b/, whose
#        probes/ is a copy of correspondence-runs/probes/; VM_REPO, JS_REPO, VMW and JSW are as for run_all.sh.
set -u
set -o pipefail

SCRATCH=${SCRATCH:?set SCRATCH to the folder that holds sjl-rest/sjl13-corr-p16b}
CORR=$SCRATCH/sjl-rest/sjl13-corr-p16b
PROBES=$CORR/probes
RUNS=$CORR/runs
WORK=$CORR/runner-work
VM_REPO=${VM_REPO:?set VM_REPO to a Broiler.VM clone}
JS_REPO=${JS_REPO:?set JS_REPO to a Broiler.JS clone}
VM_REV=4cf9ca449fc96a1a5be810f6d0ca31c42b855c8f
JS_REV=d6795fc86ca607698983a06739ddd15b9aaa9ccd
VMW=${VMW:?set VMW to the path of the Broiler.VM worktree to add}
JSW=${JSW:?set JSW to the path of the Broiler.JS worktree to add}
T262_ARCHIVE=src/tests/conformance/pins/test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e.tar.gz
T262_SHA=f58ce79141529c9fa33592e22ff3ff0d83b83830ac8e372ecd32e1623db1c040
T262_TOP=test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e
FUEL=200000000000
WALL=600000

VM_SC=$VMW/src/compositions/Broiler.VM.Composition.JavaScript.SliceCompiler/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.SliceCompiler.dll
VM_CF=$VMW/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/bin/Release/net10.0/Broiler.VM.Composition.JavaScript.Conformance.dll
JS_SH=$JSW/Broiler.JS/Broiler.JavaScript/bin/Release/net10.0/BroilerJS.dll
JS_PC=$JSW/docs/evidence/sjl-12-001/tools/parse-corpus/bin/Release/net10.0/parse-corpus.dll

export PATH=/root/.dotnet:$PATH DOTNET_ROOT=/root/.dotnet
export DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1 DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

PHASE=${1:-all}
case $PHASE in all|vm|js) ;; *) echo "usage: bash run_p16b.sh [all|vm|js]" >&2; exit 2 ;; esac

PY=$WORK/runner.py

log() { printf '[run_p16b %s] %s\n' "$(date -u +%H:%M:%S)" "$*"; }
die() { printf 'run_p16b.sh: %s\n' "$*" >&2; exit 2; }

# Runs "$@" with TZ=UTC and no LANG, LANGUAGE, LC_* or BROILER_* variable.
clean() {
  local unset=() v
  for v in $(compgen -e); do
    case $v in LANG|LANGUAGE|LC_*|BROILER_*) unset+=(-u "$v") ;; esac
  done
  env ${unset[@]+"${unset[@]}"} TZ=UTC "$@"
}

# run_logged <file> <seconds> <command...>: appends the command (and the directory it ran in), its standard output
# and error, and its exit code to <file>; runs in the current directory, under clean and timeout.
run_logged() {
  local out=$1 limit=$2
  shift 2
  { printf '# command (in %s): ' "$PWD"; printf '%q ' "$@"; printf '\n'; } >> "$out"
  clean timeout "$limit" "$@" >> "$out" 2>&1
  printf '# exit=%s\n' "$?" >> "$out"
}

# build <log> <project> [msbuild args...]: one Release build, its exit code returned.
build() {
  local logf=$1 proj=$2
  shift 2
  log "build $(basename "$proj")"
  { printf '# dotnet build -c Release --disable-build-servers -nologo %s' "$proj"; printf ' %q' "$@"; printf '\n'; } > "$logf"
  dotnet build -c Release --disable-build-servers -nologo "$proj" "$@" >> "$logf" 2>&1
  local rc=$?
  printf '# exit=%s\n' "$rc" >> "$logf"
  return $rc
}

# The helper is run_all.sh's, but for verify, which checks the new manifests and files.
write_python() {
  mkdir -p "$WORK"
  cat > "$PY" <<'PYEOF'
# run_p16b.sh's helper: verify, split reports into answers, scrub paths, hash. Run with python3 -I.
import hashlib, os, re, sys


def lines_of(path):
    with open(path, encoding="utf-8", errors="replace") as fh:
        return fh.read().split("\n")


def manifest_rows(path):
    rows, started = [], False
    for line in lines_of(path):
        if not line or line.startswith("#"):
            continue
        if not started:
            started = True
            continue
        c = line.split("\t")
        rows.append((c[0], c[2], c[3]))
    return rows


def log_meta(path):
    cmd, ext = "(no log)", "(no log)"
    if os.path.exists(path):
        for line in lines_of(path):
            if line.startswith("# command") and cmd == "(no log)":
                cmd = line
            if line.startswith("# exit="):
                ext = line
    return cmd, ext


def verify(probes):
    # Each manifest: LF only, no BOM, the declared row count, every input present with the manifest's SHA-256 and
    # strict UTF-8. And every value file is present.
    mans = ["P/probe-P16b.manifest", "T/parse/t24-end.manifest"]
    bad = []
    for m in mans:
        p = os.path.join(probes, m)
        b = open(p, "rb").read()
        if b.startswith(b"\xef\xbb\xbf") or b"\r" in b or not b.endswith(b"\n"):
            bad.append(m + ": not LF-only UTF-8 without a BOM")
        rows = manifest_rows(p)
        declared = [l for l in b.decode("utf-8").split("\n") if l.startswith("#% rows\t")]
        if not declared or int(declared[0].split("\t")[1]) != len(rows):
            bad.append(m + ": row count")
        for pid, rel, sha in rows:
            f = os.path.join(os.path.dirname(p), rel)
            if not os.path.exists(f):
                bad.append(f"{m}: {pid} missing {rel}")
                continue
            data = open(f, "rb").read()
            if hashlib.sha256(data).hexdigest() != sha:
                bad.append(f"{m}: {pid} SHA-256 differs")
            try:
                data.decode("utf-8", "strict")
            except UnicodeDecodeError:
                bad.append(f"{m}: {pid} not UTF-8")
    for rel in ["P/value/prelude.js", "P/value/p-values.js", "P/value/epilogue.js", "T/value/prelude.js",
                "T/value/t24-values.js", "T/value/epilogue.js", "plan-p16b.tsv"]:
        if not os.path.exists(os.path.join(probes, rel)):
            bad.append("missing " + rel)
    for b in bad:
        print("verify: " + b)
    return 1 if bad else 0


def split_parse(engine, manifest, report, outdir, logpath):
    # One answer per manifest id: the report's column line and every row whose first field is the id.
    cmd, ext = log_meta(logpath)
    have = os.path.exists(report)
    column, rows, meta = None, [], []
    if have:
        for line in lines_of(report):
            if not line:
                continue
            if line.startswith("#% ") and line.split("\t")[0] in ("#% manifest-sha256", "#% format", "#% corpus", "#% end"):
                meta.append(line)
            if line.startswith("#"):
                continue
            if column is None:
                column = line
                continue
            rows.append(line)
    for pid, rel, _ in manifest_rows(manifest):
        with open(os.path.join(outdir, pid + ".txt"), "w", encoding="utf-8", newline="\n") as fh:
            fh.write(f"# probe {pid} ({engine}): input {rel}; its rows of reports/{os.path.basename(report)}\n")
            fh.write(cmd + "\n" + ext + "\n")
            if not have:
                fh.write(f"NO REPORT: the run wrote none; see logs/{os.path.basename(logpath)}\n")
                continue
            for m in meta:
                fh.write(m + "\n")
            fh.write((column or "(no column line)") + "\n")
            mine = [r for r in rows if r.split("\t", 1)[0] == pid]
            for r in mine:
                fh.write(r + "\n")
            if not mine:
                fh.write("NO ROW for this id in the report\n")


def split_values(engine, values_js, transcript, outdir, prefix):
    # One answer per label of the value file: the output line that begins with the label's id.
    src = open(values_js, encoding="utf-8").read()
    ids = re.findall(r"\b(?:t|out)\('([A-Za-z0-9]+) ", src)
    cmd, ext = log_meta(transcript)
    text = open(transcript, encoding="utf-8", errors="replace").read() if os.path.exists(transcript) else ""
    note = ""
    if engine == "vm":
        a, b = text.find("OUT<<"), text.rfind(">>OUT")
        if a >= 0 and b > a:
            body = text[a + 5:b]
        else:
            body = ""
            verdict = [l for l in text.split("\n") if "test/sjl13" in l and not l.startswith("#")]
            note = "NO OUTPUT; the verdict line: " + (verdict[0] if verdict else "(none)")
    else:
        body = "\n".join(l for l in text.split("\n") if not l.startswith("# "))
    out = body.split("\n")
    for pid in ids:
        mine = [l for l in out if l.startswith(pid + " ")]
        with open(os.path.join(outdir, prefix + pid + ".txt"), "w", encoding="utf-8", newline="\n") as fh:
            fh.write(f"# probe {prefix}{pid} ({engine}): the line labelled {pid} of {os.path.basename(values_js)}'s run; "
                     f"whole transcript reports/{os.path.basename(transcript)}\n")
            fh.write(cmd + "\n" + ext + "\n")
            for l in mine:
                fh.write(l + "\n")
            if not mine:
                fh.write((note or "NO LINE for this label in the output") + "\n")


def scrub(directory, pairs):
    subs = [(old.encode(), new.encode()) for old, new in (p.split("=", 1) for p in pairs)]
    for root, _, files in os.walk(directory):
        for name in files:
            path = os.path.join(root, name)
            data = open(path, "rb").read()
            new = data
            for old, rep in subs:
                new = new.replace(old, rep)
            if new != data:
                open(path, "wb").write(new)


def hashes(directory):
    entries = []
    for root, _, files in os.walk(directory):
        for name in files:
            path = os.path.join(root, name)
            rel = os.path.relpath(path, directory).replace(os.sep, "/")
            if rel == "hashes.txt":
                continue
            entries.append((rel, hashlib.sha256(open(path, "rb").read()).hexdigest()))
    entries.sort()
    with open(os.path.join(directory, "hashes.txt"), "w", encoding="utf-8", newline="\n") as fh:
        for rel, h in entries:
            fh.write(f"{h}  {rel}\n")


if __name__ == "__main__":
    op, args = sys.argv[1], sys.argv[2:]
    if op == "verify":
        sys.exit(verify(*args))
    elif op == "split-parse":
        split_parse(*args)
    elif op == "split-values":
        split_values(*args)
    elif op == "scrub":
        scrub(args[0], args[1:])
    elif op == "hashes":
        hashes(*args)
    else:
        sys.exit("unknown operation " + op)
PYEOF
}

# The pinned test262 archive, read out of Broiler.VM's history at 4cf9ca4 (git cat-file of the blob, nothing checked
# out), its SHA-256 checked against the probe manifests' pin; harness/ and features.txt extracted to $WORK/t262.
prepare_test262() {
  [ -f "$WORK/t262/features.txt" ] && [ -d "$WORK/t262/harness" ] && return 0
  mkdir -p "$WORK/t262"
  git -C "$VM_REPO" cat-file blob "$VM_REV:$T262_ARCHIVE" > "$WORK/test262.tar.gz" || die "cannot read $T262_ARCHIVE at $VM_REV"
  [ "$(sha256sum "$WORK/test262.tar.gz" | cut -d' ' -f1)" = "$T262_SHA" ] || die "the test262 archive does not hash to the pin"
  tar -xzf "$WORK/test262.tar.gz" -C "$WORK/t262" --strip-components=1 "$T262_TOP/harness" "$T262_TOP/features.txt" \
    || die "cannot extract harness/ and features.txt"
  rm -f "$WORK/test262.tar.gz"
}

# remove_worktree <repository> <worktree>: bin/ and obj/ first, then the worktree itself.
remove_worktree() {
  local repo=$1 wt=$2
  [ -d "$wt" ] || return 0
  if [ "${KEEP:-0}" = 1 ]; then log "KEEP=1: $wt kept"; return 0; fi
  log "delete bin/ and obj/ under $wt, then remove the worktree"
  find "$wt" -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} +
  git -C "$repo" worktree remove --force "$wt" || log "worktree remove failed for $wt (left in place)"
}

finish_engine() { # <engine dir>
  local d=$1
  python3 -I "$PY" scrub "$d" "$WORK=<work>" "$VMW=<vm>" "$JSW=<js>" "$PROBES=<probes>" "$RUNS=<runs>" \
    "$CORR=<corr>" "$SCRATCH=<scratch>"
  python3 -I "$PY" hashes "$d"
}

# vm262: a test262 root for the conformance root (harness/ and features.txt; the value probes' tests under test/).
prepare_vm262() {
  rm -rf "$WORK/vm262"
  mkdir -p "$WORK/vm262/test"
  cp -r "$WORK/t262/harness" "$WORK/vm262/harness"
  cp "$WORK/t262/features.txt" "$WORK/vm262/features.txt"
}

# ---------------------------------------------------------------------------------------------------------------
phase_vm() {
  local RV=$RUNS/vm
  [ -d "$RV" ] && [ -n "$(ls -A "$RV")" ] && die "$RV is not empty; move it aside first"
  [ -e "$VMW" ] && die "$VMW exists; remove it (git -C $VM_REPO worktree remove) first"
  mkdir -p "$RV/reports" "$RV/logs" "$RV/built"
  git -C "$VM_REPO" cat-file -e "$VM_REV^{commit}" || die "Broiler.VM has no $VM_REV"
  log "worktree $VMW at $VM_REV"
  git -C "$VM_REPO" worktree add --detach "$VMW" "$VM_REV" > "$RV/logs/p16b-worktree.log" 2>&1 || die "worktree add failed"
  printf 'Broiler.VM %s\n' "$(git -C "$VMW" rev-parse HEAD)" > "$RV/p16b-revision.txt"

  local ok=1 p
  for p in SliceCompiler Conformance; do
    build "$WORK/p16b-build-vm-$p.log" "$VMW/src/compositions/Broiler.VM.Composition.JavaScript.$p/Broiler.VM.Composition.JavaScript.$p.csproj" || ok=0
    { printf '%s: ' "$p"; tail -n 1 "$WORK/p16b-build-vm-$p.log"; } >> "$RV/p16b-build.txt"
  done
  for p in "$VM_SC" "$VM_CF"; do [ -f "$p" ] || ok=0; done
  if [ $ok != 1 ]; then
    log "the Broiler.VM build failed; logs in $WORK/p16b-build-vm-*.log"
    cp "$WORK"/p16b-build-vm-*.log "$RV/logs/" 2>/dev/null
    finish_engine "$RV"
    remove_worktree "$VM_REPO" "$VMW"
    return 1
  fi

  # Parse probes: one --parse-corpus run per manifest (--parse-corpus <manifest> <report> [--workers n]
  # [--populations list]), as run_all.sh ran P's (every population) and T's (B1 and B4).
  vm_parse() { # <name> <manifest> <seconds> [options...]
    local name=$1 man=$2 limit=$3
    shift 3
    log "VM-parse $name"
    run_logged "$RV/logs/$name.log" "$limit" dotnet "$VM_SC" --parse-corpus "$man" "$RV/reports/$name.tsv" "$@" --workers 1
    python3 -I "$PY" split-parse vm "$man" "$RV/reports/$name.tsv" "$RV" "$RV/logs/$name.log"
  }
  vm_parse P16b-parse "$PROBES/P/probe-P16b.manifest" 1800
  vm_parse T24-end-parse "$PROBES/T/parse/t24-end.manifest" 1800 --populations B1,B4

  # The conformance root's --test262 mode, as run_all.sh ran the value probes.
  prepare_vm262

  # Value probes: one raw test262 program each (SJL-11.1's form), the answer between OUT<< and >>OUT.
  vm_value() { # <sub-area letter> <values file> <test dir> <transcript name> <prefix>
    local S=$1 vf=$2 dir=$3 name=$4 prefix=$5 base
    base=$(basename "$vf" .js)
    log "VM-value $name"
    mkdir -p "$WORK/vm262/test/$dir"
    { printf '/*---\ndescription: SJL-13.1 %s value probes\nflags: [raw]\n---*/\nvar __vmTest = true;\n' "$name"
      cat "$PROBES/$S/value/prelude.js" "$PROBES/$S/value/$vf" "$PROBES/$S/value/epilogue.js"; } > "$WORK/vm262/test/$dir/$vf"
    cp "$WORK/vm262/test/$dir/$vf" "$RV/built/$base.test262.js"
    ( cd "$WORK" && run_logged "$RV/reports/$name-values.transcript.txt" 900 dotnet "$VM_CF" --test262 vm262 \
        --test "test/$dir/$vf" --fuel "$FUEL" --wall "$WALL" )
    python3 -I "$PY" split-values vm "$PROBES/$S/value/$vf" "$RV/reports/$name-values.transcript.txt" "$RV" "$prefix"
  }
  vm_value P p-values.js sjl13p P16b ""
  vm_value T t24-values.js sjl13t24 T24-end TV-

  cp "$WORK"/p16b-build-vm-*.log "$RV/logs/"
  finish_engine "$RV"
  remove_worktree "$VM_REPO" "$VMW"
  rm -rf "$WORK/vm262"
  log "Broiler.VM phase done: $RV"
}

# ---------------------------------------------------------------------------------------------------------------
phase_js() {
  local RJ=$RUNS/js
  [ -d "$RJ" ] && [ -n "$(ls -A "$RJ")" ] && die "$RJ is not empty; move it aside first"
  [ -e "$JSW" ] && die "$JSW exists; remove it (git -C $JS_REPO worktree remove) first"
  mkdir -p "$RJ/reports" "$RJ/logs" "$RJ/built"
  if ! git -C "$JS_REPO" cat-file -e "$JS_REV^{commit}" 2>/dev/null; then
    log "fetch $JS_REV"
    git -C "$JS_REPO" fetch origin "$JS_REV" > "$RJ/logs/p16b-fetch.log" 2>&1 || die "Broiler.JS has no $JS_REV"
  fi
  log "worktree $JSW at $JS_REV"
  git -C "$JS_REPO" worktree add --detach "$JSW" "$JS_REV" > "$RJ/logs/p16b-worktree.log" 2>&1 || die "worktree add failed"
  printf 'Broiler.JS %s\n' "$(git -C "$JSW" rev-parse HEAD)" > "$RJ/p16b-revision.txt"

  # One build tree: the shell, then the parse-corpus tool, whose product projects are the shell's.
  local ok=1
  build "$WORK/p16b-build-js-shell.log" "$JSW/Broiler.JS/Broiler.JavaScript/Broiler.JavaScript.csproj" || ok=0
  { printf 'shell: '; tail -n 1 "$WORK/p16b-build-js-shell.log"; } >> "$RJ/p16b-build.txt"
  build "$WORK/p16b-build-js-parse-corpus.log" "$JSW/docs/evidence/sjl-12-001/tools/parse-corpus/parse-corpus.csproj" || ok=0
  { printf 'parse-corpus: '; tail -n 1 "$WORK/p16b-build-js-parse-corpus.log"; } >> "$RJ/p16b-build.txt"
  [ -f "$JS_SH" ] && [ -f "$JS_PC" ] || ok=0
  if [ $ok != 1 ]; then
    log "the Broiler.JS build failed; logs in $WORK/p16b-build-js-*.log"
    cp "$WORK"/p16b-build-js-*.log "$RJ/logs/" 2>/dev/null
    finish_engine "$RJ"
    remove_worktree "$JS_REPO" "$JSW"
    return 1
  fi

  # Parse probes: the SJL-12 tool's part C (--manifest, --test262 <suite root>, --out).
  js_parse() { # <name> <manifest> <seconds>
    local name=$1 man=$2 limit=$3
    log "JS-parse $name"
    run_logged "$RJ/logs/$name.log" "$limit" dotnet "$JS_PC" --manifest "$man" --test262 "$WORK/t262" --out "$RJ/reports/$name.tsv"
    python3 -I "$PY" split-parse js "$man" "$RJ/reports/$name.tsv" "$RJ" "$RJ/logs/$name.log"
  }
  js_parse P16b-parse "$PROBES/P/probe-P16b.manifest" 1800
  js_parse T24-end-parse "$PROBES/T/parse/t24-end.manifest" 1800

  # Value probes: one program each through the shell's --script-host.
  js_value() { # <sub-area letter> <values file> <transcript name> <prefix>
    local S=$1 vf=$2 name=$3 prefix=$4
    log "JS-value $name"
    cat "$PROBES/$S/value/prelude.js" "$PROBES/$S/value/$vf" "$PROBES/$S/value/epilogue.js" > "$RJ/built/$vf"
    ( cd "$RJ/built" && run_logged "$RJ/reports/$name-values.transcript.txt" 900 dotnet "$JS_SH" --script-host "$vf" )
    python3 -I "$PY" split-values js "$PROBES/$S/value/$vf" "$RJ/reports/$name-values.transcript.txt" "$RJ" "$prefix"
  }
  js_value P p-values.js P16b ""
  js_value T t24-values.js T24-end TV-

  cp "$WORK"/p16b-build-js-*.log "$RJ/logs/"
  finish_engine "$RJ"
  remove_worktree "$JS_REPO" "$JSW"
  log "Broiler.JS phase done: $RJ"
}

# ---------------------------------------------------------------------------------------------------------------
command -v python3 > /dev/null || die "python3 is required"
[ -x /root/.dotnet/dotnet ] || die "no /root/.dotnet/dotnet"
mkdir -p "$RUNS" "$WORK"
cd "$CORR" || die "cannot enter $CORR"
write_python
python3 -I "$PY" verify "$PROBES" || die "the probe inputs do not match their manifests"
prepare_test262
log "free disk: $(df -h --output=avail "$WORK" | tail -n 1 | tr -d ' ')"

trap 'log "interrupted"; [ "${KEEP:-0}" = 1 ] || { remove_worktree "$VM_REPO" "$VMW"; remove_worktree "$JS_REPO" "$JSW"; }; exit 130' INT TERM

status=0
if [ "$PHASE" = all ] || [ "$PHASE" = vm ]; then phase_vm || status=1; fi
if [ "$PHASE" = all ] || [ "$PHASE" = js ]; then phase_js || status=1; fi
rm -rf "$WORK/t262"
log "done (status $status). Answers: $RUNS/vm, $RUNS/js; build logs: $WORK"
exit $status
