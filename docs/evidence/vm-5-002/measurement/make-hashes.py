"""Write docs/evidence/vm-5-002/hashes.txt.

Every digest is SHA-256 of the bytes the repository stores: a tracked file is read from its blob at
the retained commit (and must equal the working copy, or the script refuses, so a hashed file changed
since that commit is refused rather than hashed), and a file of this bundle is read from the working
copy, where it is already LF.

docs/baselines.md was in the third set as first committed, as the register this bundle does not
edit. It was withdrawn: no run, figure or verdict here reads it, and the dated note the change's own
records commit adds to it hit this bundle's first recertification trigger (README section 8).

Changed with the re-collection after the owner-directed remedy: tracked files are read at the remedy
head 16e3d6d instead of at 34dbd7a, and the second set also names the files the remedy changed or rests
on that it did not name before (README sections 5.10 and 8). The fourth set already took every file under
this directory, r2/ included; the fifth takes the digests of the uncompressed reports behind every
.report.gz here, which the argument, a JSON file, lists.

python docs/evidence/vm-5-002/measurement/make-hashes.py   (from the repository root)
"""
import hashlib
import importlib.util
import json
import pathlib
import subprocess
import sys

ROOT = pathlib.Path(__file__).resolve().parents[4]
BUNDLE = ROOT / "docs" / "evidence" / "vm-5-002"
RETAINED = "16e3d6d"


crlf_in_working_copy = set()


def blob(relative):
    data = subprocess.run(["git", "show", RETAINED + ":" + relative], cwd=ROOT, capture_output=True,
                          check=True).stdout
    working = (ROOT / relative).read_bytes()
    if working != data:
        if working.replace(b"\r\n", b"\n") != data:
            sys.exit("REFUSED: %s differs between %s and the working copy" % (relative, RETAINED))
        crlf_in_working_copy.add(relative)
    return data


def row(data, relative):
    return "%s  %s" % (hashlib.sha256(data).hexdigest(), relative)


spec = importlib.util.spec_from_file_location("collector", ROOT / "eng" / "collect-evidence.py")
collector = importlib.util.module_from_spec(spec)
spec.loader.exec_module(collector)
collector_set = collector.hashed_files()

design_set = [
    "src/Broiler.VM.Runtime/VmMeter.cs",
    "src/Broiler.VM.Runtime/VmFuelPreAdmissions.cs",
    "src/Broiler.VM.Runtime/VmBudgetLevel.cs",
    "src/Broiler.VM.Runtime/VmRuntime.cs",
    "src/Broiler.VM.Runtime/VmInstanceImplementation.cs",
    "src/Broiler.VM.Runtime/VmInstantiation.cs",
    "src/Broiler.VM.Runtime/VmExecutionScope.cs",
    "src/Broiler.VM.Runtime/VmVerification.cs",
    "src/tests/Broiler.VM.Contract.Tests/FuelChargeExactnessTests.cs",
    "src/tests/Broiler.VM.Fixtures/FixtureArtifactWriter.cs",
    "src/tests/Broiler.VM.Fixtures/FixtureHostCapabilities.cs",
    "src/tests/Broiler.VM.Fixtures/FixtureVmExecutor.cs",
    "src/tests/Broiler.VM.Fixtures/FixtureVmProfile.cs",
    "docs/adr/0003-core-contract-v1-and-amendments.md",
    "docs/adr/0007-resource-authority-and-budgets.md",
    "src/Broiler.VM.Runtime/VmCapabilityBinding.cs",
    "src/Broiler.VM.Runtime/VmArtifactLoadMediator.cs",
    "src/tests/Broiler.VM.Contract.Tests/ReviewRegressionTests.cs",
    "docs/adr/0011-source-level-profile-contract.md",
]

drivers = [
    "eng/collect-evidence.py",
    "eng/run-test262.py",
    "eng/compare-test262-forms.py",
    "eng/run-octane.py",
    "src/tests/conformance/pins/test262.pin",
    "src/tests/octane/pins/octane.pin",
    "docs/api/public-api.txt",
]

retained = sorted(
    p.relative_to(BUNDLE).as_posix() for p in BUNDLE.rglob("*")
    if p.is_file() and p.name not in ("README.md", "hashes.txt") or (p.is_file() and p.parent != BUNDLE and p.name in ("README.md",)))

raw = json.loads(pathlib.Path(sys.argv[1]).read_text(encoding="utf-8"))["raw_report_digests"] if len(sys.argv) > 1 else []

lines = [
    "SHA-256 of the files evidence bundle VM-5-002 depends on, and of every file it retains.",
    "",
    "Every digest is of the bytes the repository stores (LF text). Paths are repository-relative in",
    "the first three sections and relative to this directory in the fourth. This README and this file",
    "are not hashed here.",
    "",
    "## 1. The set eng/collect-evidence.py pins for a core bundle, recomputed at the commit retained",
    "",
]
lines += [row(blob(f), f) for f in collector_set if (ROOT / f).exists()]
lines += ["", "## 2. The files design section 8.4 names: the metering path, the fuel-exactness tests, the fixture", "##    files the fuel work changed, and the two ADRs the change rests on without editing; and, since the", "##    re-collection, the capability boundary's files, the review-regression tests and ADR 0011, which the", "##    owner-directed remedy changed or rests on", ""]
lines += [row(blob(f), f) for f in design_set]
lines += ["", "## 3. The drivers and pins the runs used, and the public API file E1c compares", ""]
lines += [row(blob(f), f) for f in drivers]
lines += ["", "## 4. Every file retained in this directory", ""]
lines += [row((BUNDLE / f).read_bytes(), f) for f in retained]
lines += ["", "## 5. The uncompressed test262 reports behind each .report.gz above: digest, retained name less", "##    its .gz suffix, and length in bytes. These bytes are not in the repository.", ""]
lines += ["%s  %s  %d" % (digest, name, size) for digest, name, size in raw]
lines += ["", "## 6. Tracked files whose working copy at collection carried CRLF line endings, although the", "##    repository stores them LF; the digests above are of the stored bytes", ""]
lines += sorted(crlf_in_working_copy) or ["(none)"]
(BUNDLE / "hashes.txt").write_bytes(("\n".join(lines) + "\n").encode("utf-8"))
print("hashes.txt: %d tracked, %d retained, %d raw" % (len(collector_set) + len(design_set) + len(drivers), len(retained), len(raw)))
