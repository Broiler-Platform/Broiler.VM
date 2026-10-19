#!/usr/bin/env python3
"""Decide whether a push to main may skip the jobs its pull request's runs already ran.

The gate job of broiler-vm.yml and of ci.yml runs this first. It writes `skip=true` or
`skip=false` to $GITHUB_OUTPUT, says why on standard output and in the step summary, and exits 0
either way: a gate that cannot decide says so and lets the run go ahead in full.

THE PUSH IS SKIPPED ONLY WHEN ALL OF THESE HOLD, and each is checked here rather than assumed:

1. The event is a push and the ref is refs/heads/main. Inside ci.yml this is not decoration: a
   workflow entered through workflow_call sees its CALLER's event, so a publish started by a tag
   push arrives here as a push, and only the ref tells it apart.
2. HEAD has two parents. A commit with one - a direct push, a squash, a rebase - was never the
   head of a pull request's run, so nothing tested its tree, and it is never skipped.
3. HEAD's tree is HEAD^2's tree: the merge brought nothing in that the pull request's head did not
   already hold.
4. HEAD^1 is an ancestor of HEAD^2, read from the repository's compare API. A pull request's run
   tests the merge of its head into its base AS IT STOOD THEN, and that merge has the head's tree
   whenever the head descends from that base. Both workflows run on a pull request only when it is
   into main (their pull_request triggers say `branches: [main]`), so the base of every pull
   request run on HEAD^2 was an earlier main, which is an ancestor of HEAD^1 as long as main is
   only ever added to; requiring HEAD^1 to be an ancestor of HEAD^2 then makes the tree each such
   run tested the tree that landed. A run on a button tested HEAD^2's own tree. THE SCRIPT CANNOT
   CHECK THE BASE ITSELF: a check run's pull_requests list is empty once its pull request has
   merged, so it cannot tell a run of a pull request into main from one into another branch, whose
   merge would have had that branch's commits in it. The triggers' branch filter is what holds it.
5. Every check run on HEAD^2 is complete and green: each job named in REQUIRED below has a run
   that succeeded, and every other check run there succeeded or was skipped. A job added to
   either workflow and not yet named below is still held by the second half; a job renamed or
   removed and still named below stops every skip, which costs minutes and nothing else.

The token needs `contents: read` and `checks: read`. It reads; it writes nothing anywhere.
"""

import json
import os
import re
import subprocess
import sys
import urllib.error
import urllib.request

# Every check run a pull request's runs of broiler-vm.yml and ci.yml produce, by the name GitHub
# gives it. A job of the lane is named `quick / <job name>` because broiler-vm.yml calls the lane
# from its job `quick`; the gate jobs themselves are skipped on a pull request and are not named.
REQUIRED = (
    # broiler-vm.yml, through broiler-vm-lane.yml
    "quick / differential runner (ubuntu-latest)",
    "quick / differential runner (windows-latest)",
    "quick / build, suites, graph and drift",
    "quick / publish and run (linux-x64)",
    "quick / publish and run (linux-arm64)",
    "quick / slice checks and source fuzz (linux-x64)",
    "quick / slice checks and source fuzz (linux-arm64)",
    "quick / android head (build only)",
    # ci.yml
    "windows-latest / Release",
)


class Undecided(Exception):
    """The gate could not read what it needs, so the run goes ahead in full."""


def git(*arguments):
    result = subprocess.run(["git", *arguments], capture_output=True, text=True)
    if result.returncode != 0:
        raise Undecided("git %s failed: %s" % (" ".join(arguments), result.stderr.strip()))
    return result.stdout.strip()


def api(path):
    base = os.environ.get("GITHUB_API_URL", "https://api.github.com")
    token = os.environ.get("GH_TOKEN") or os.environ.get("GITHUB_TOKEN")
    if not token:
        raise Undecided("no token in GH_TOKEN or GITHUB_TOKEN")
    request = urllib.request.Request(base + path, headers={
        "Accept": "application/vnd.github+json",
        "Authorization": "Bearer " + token,
        "X-GitHub-Api-Version": "2022-11-28",
    })
    try:
        with urllib.request.urlopen(request, timeout=30) as response:
            return json.load(response)
    except (urllib.error.URLError, OSError, ValueError) as failure:
        raise Undecided("GET %s failed: %s" % (path, failure))


def check_runs(repository, sha):
    """Every check run on `sha`, the latest of each name (a re-run supersedes the run it repeats)."""
    runs, page = [], 1
    while True:
        body = api("/repos/%s/commits/%s/check-runs?filter=latest&per_page=100&page=%d"
                   % (repository, sha, page))
        batch = body.get("check_runs", [])
        runs.extend(batch)
        if len(batch) < 100:
            break
        page += 1
    latest = {}
    for run in runs:
        name = run.get("name", "")
        if name not in latest or run.get("id", 0) > latest[name].get("id", 0):
            latest[name] = run
    return latest


def verdict(run):
    return run.get("conclusion") or run.get("status") or "unknown"


def decide():
    event = os.environ.get("GITHUB_EVENT_NAME", "")
    ref = os.environ.get("GITHUB_REF", "")
    repository = os.environ.get("GITHUB_REPOSITORY", "")
    server = os.environ.get("GITHUB_SERVER_URL", "https://github.com")

    if event != "push" or ref != "refs/heads/main":
        return False, ["the event is %r on %r, and only a push to refs/heads/main is ever skipped"
                       % (event, ref)]

    head = git("rev-parse", "HEAD")
    parents = git("rev-list", "--parents", "-n", "1", "HEAD").split()[1:]
    if len(parents) != 2:
        return False, ["%s has %d parent(s); a commit that is not a merge is never skipped, because "
                       "no pull request's run tested its tree" % (head, len(parents))]
    first, second = parents

    head_tree = git("rev-parse", "HEAD^{tree}")
    second_tree = git("rev-parse", "HEAD^2^{tree}")
    if head_tree != second_tree:
        return False, ["the merge's tree %s is not the tree %s of its second parent %s, so the merge "
                       "brought in something the pull request's runs did not see"
                       % (head_tree, second_tree, second)]

    compare = api("/repos/%s/compare/%s...%s" % (repository, first, second))
    if compare.get("status") not in ("ahead", "identical"):
        return False, ["the first parent %s is not an ancestor of the second %s (compare says %r), so "
                       "a pull request's run may have tested a merge with another base"
                       % (first, second, compare.get("status"))]

    latest = check_runs(repository, second)
    missing = [name for name in REQUIRED if name not in latest]
    red = ["%s: %s" % (name, verdict(latest[name])) for name in REQUIRED
           if name in latest and not (latest[name].get("status") == "completed"
                                      and latest[name].get("conclusion") == "success")]
    others = ["%s: %s" % (name, verdict(run)) for name, run in sorted(latest.items())
              if name not in REQUIRED and not (run.get("status") == "completed"
                                               and run.get("conclusion") in ("success", "skipped"))]
    if missing or red or others:
        reasons = ["%s, the second parent, is not green in every job of both workflows" % second]
        reasons += ["  no check run: " + name for name in missing]
        reasons += ["  not a success: " + line for line in red]
        reasons += ["  neither a success nor skipped: " + line for line in others]
        return False, reasons

    relied = sorted({match.group(1) for name in REQUIRED
                     for match in [re.search(r"/actions/runs/(\d+)",
                                             latest[name].get("details_url") or
                                             latest[name].get("html_url") or "")]
                     if match})
    reasons = ["%s merges %s into %s, and its tree %s is that pull request head's" % (
        head, second, first, head_tree)]
    reasons.append("every job of both workflows succeeded on %s, in these runs:" % second)
    reasons += ["  %s/%s/actions/runs/%s" % (server, repository, run) for run in relied]
    reasons += ["  %s: %s" % (name, latest[name].get("html_url") or latest[name].get("details_url") or "")
                for name in REQUIRED]
    return True, reasons


def main():
    try:
        skip, reasons = decide()
    except Undecided as failure:
        skip, reasons = False, ["the gate could not decide, so nothing is skipped: %s" % failure]
        print("::warning::" + reasons[0])

    heading = ("SKIPPED: this push's tree was tested in full by its pull request's runs" if skip
               else "NOT SKIPPED: this run goes ahead in full")
    print(heading)
    for line in reasons:
        print(line)

    output = os.environ.get("GITHUB_OUTPUT")
    if output:
        with open(output, "a", encoding="utf-8") as stream:
            stream.write("skip=%s\n" % ("true" if skip else "false"))
    summary = os.environ.get("GITHUB_STEP_SUMMARY")
    if summary:
        with open(summary, "a", encoding="utf-8") as stream:
            stream.write("### %s\n\n```\n%s\n```\n" % (heading, "\n".join(reasons)))
    return 0


if __name__ == "__main__":
    sys.exit(main())
