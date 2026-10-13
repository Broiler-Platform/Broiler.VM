#!/usr/bin/env bash
# Point one pristine consumer under samples/ at a local feed and nothing else, for a lane step.
#
#   eng/sample-local-feed.sh <sample directory> <feed directory>
#
# samples/NuGet.config lists nuget.org, for a developer building the samples against the published
# packages (since 2026-10-11). A lane step that packs the tree and then restores a sample from what it
# packed needs the opposite: ONE source, the feed it just wrote, with every package mapped to it, so
# that a restore that succeeds found everything in the packages just built and nowhere else. This
# writes that NuGet.config beside the sample, where NuGet reads it after samples/NuGet.config and its
# <clear /> drops nuget.org; the caller removes it when the step ends. eng/collect-js-packages.py
# writes the same configuration for its own runs.
set -euo pipefail

sample="$1"
feed="$(cd "$2" && pwd)"
# Under Git Bash on a Windows runner the path is /d/a/...; NuGet needs D:/a/....
if command -v cygpath >/dev/null 2>&1; then
  feed="$(cygpath -m "$feed")"
fi

cat > "$sample/NuGet.config" <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local-feed" value="$feed" />
  </packageSources>
  <packageSourceMapping>
    <clear />
    <packageSource key="local-feed">
      <package pattern="*" />
    </packageSource>
  </packageSourceMapping>
</configuration>
EOF
