# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
"""Exercise the driver through real child processes, with no engine build required."""
import importlib.util
import json
from pathlib import Path
import subprocess
import sys
import tempfile
import time
import unittest

DRIVER = Path(__file__).resolve().parents[1] / "run-differential.py"
spec = importlib.util.spec_from_file_location("differential", DRIVER)
driver = importlib.util.module_from_spec(spec)
spec.loader.exec_module(driver)

FAKE = '''import json, os, pathlib, subprocess, sys, time
sys.stdout.reconfigure(encoding="utf-8")
path = pathlib.Path(sys.argv[2])
assert sys.argv[1] == ("--module" if path.suffix == ".mjs" else "--script")
assert os.environ["TZ"] == "UTC"
data = json.loads(path.read_text(encoding="utf-8"))
if data.get("dependency"):
    assert (path.parent / "modules" / "dependency.mjs").read_text(encoding="utf-8") == "Grüße"
mode = data.get("comparison", {}) if "--comparison" in sys.argv else data.get("host", {})
if mode.get("spawn"):
    subprocess.Popen([sys.executable, "-c", "import time; time.sleep(60)"])
if mode.get("sleep"):
    time.sleep(60)
if mode.get("invalidUtf8"):
    sys.stdout.buffer.write(b"1 \\xff\\n")
else:
    sys.stdout.write(mode.get("stdout", "1 42\\n2 Grüße\\n"))
sys.stderr.write(mode.get("stderr", ""))
sys.exit(mode.get("exit", 0))
'''


class DriverTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory(prefix="differential tests ü ")
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.probes = self.root / "probes"
        self.probes.mkdir()
        self.fake = self.root / "fake engine.py"
        self.fake.write_text(FAKE, encoding="utf-8")
        self.host = self.config("host", False)
        self.other = self.config("broiler-js", True)
        self.report = self.root / "report.json"

    def config(self, name, comparison):
        path = self.root / (name + ".json")
        tail = ["--comparison"] if comparison else []
        path.write_text(json.dumps({"name": name, "executable": sys.executable,
            "scriptArgs": [str(self.fake), "--script", "{probe}", *tail],
            "moduleArgs": [str(self.fake), "--module", "{probe}", *tail]}), encoding="utf-8")
        return path

    def fixture(self, name="probe.js", payload=None, declarations=""):
        probe = self.probes / name
        probe.write_text(json.dumps(payload or {}), encoding="utf-8")
        probe.with_suffix(".expected.txt").write_text(declarations + "1 42\n2 Grüße\n", encoding="utf-8")
        return probe

    def run_driver(self, *extra, comparison=True):
        command = [sys.executable, str(DRIVER), "--host-config", str(self.host),
                   "--probe-directory", str(self.probes), "--report", str(self.report)]
        if comparison:
            command += ["--against-config", str(self.other)]
        done = subprocess.run(command + list(extra), capture_output=True, encoding="utf-8", timeout=25)
        self.assertIn(done.returncode, (0, 1, 2), done.stderr)
        report = json.loads(self.report.read_text(encoding="utf-8")) if self.report.exists() else None
        return done, report

    def assert_failure(self, payload, text, declarations=""):
        self.fixture(payload=payload, declarations=declarations)
        done, report = self.run_driver()
        self.assertEqual(1, done.returncode, done.stdout + done.stderr)
        self.assertIn(text, done.stdout)
        self.assertFalse(report["ok"])
        return report

    def test_script_module_unicode_paths_and_engine_identity(self):
        script = self.fixture("script.js")
        module = self.fixture("module.mjs", {"dependency": True})
        (self.probes / "modules").mkdir()
        (self.probes / "modules/dependency.mjs").write_text("Grüße", encoding="utf-8")
        originals = [p.read_bytes() for p in (script, module)]
        done, report = self.run_driver()
        self.assertEqual(0, done.returncode, done.stdout + done.stderr)
        self.assertEqual(["module", "script"], [p["runs"][1]["goal"] for p in report["probes"]])
        self.assertEqual(["host", "broiler-js"], [e["name"] for e in report["engines"]])
        self.assertTrue(all(e["binaries"] for e in report["engines"]))
        for probe in report["probes"]:
            self.assertIn("Grüße", probe["runs"][1]["stdout"])
            self.assertEqual(probe["runs"][0]["argv"][3], probe["runs"][1]["argv"][3])
        self.assertEqual(originals, [p.read_bytes() for p in (script, module)])

    def test_hung_engine_and_descendant_are_bounded(self):
        self.fixture(payload={"comparison": {"sleep": True, "spawn": True}})
        started = time.monotonic()
        done, report = self.run_driver("--timeout", "1")
        self.assertEqual(1, done.returncode, done.stdout)
        self.assertIn("timed out", done.stdout)
        self.assertTrue(report["probes"][0]["runs"][1]["timedOut"])
        self.assertLess(time.monotonic() - started, 15)

    def test_missing_engine_is_visible_in_report(self):
        self.fixture()
        config = json.loads(self.other.read_text())
        config["executable"] = str(self.root / "missing-engine")
        self.other.write_text(json.dumps(config))
        done, report = self.run_driver()
        self.assertEqual(1, done.returncode)
        self.assertIn("engine executable not found", done.stdout)
        self.assertEqual(2, len(report["engines"]))

    def test_stale_declaration_fails(self):
        self.assert_failure({}, "stale divergence broiler-js/1", "#diverges broiler-js 1 old reason\n")

    def test_absent_declared_case_fails(self):
        self.assert_failure({}, "stale/unverifiable", "#diverges broiler-js 99 absent case\n")

    def test_node_declaration_does_not_exempt_broiler_js(self):
        self.assert_failure({"comparison": {"stdout": "1 7\n2 Grüße\n"}},
                            "undeclared divergence broiler-js/1", "#diverges node 1 Node-only reason\n")

    def test_named_declaration_accepts_difference_and_retains_both_values(self):
        self.fixture(payload={"comparison": {"stdout": "1 7\n2 Grüße\n"}},
                     declarations="#diverges broiler-js 1 known reason\n#diverges node 2 other reason\n")
        done, report = self.run_driver()
        self.assertEqual(0, done.returncode, done.stdout)
        self.assertEqual("7", report["probes"][0]["declaredDivergences"][0]["comparison"])

    def test_comparison_nonzero_exit_is_not_ignored(self):
        report = self.assert_failure({"comparison": {"exit": 7}}, "exited 7")
        self.assertEqual(7, report["probes"][0]["runs"][1]["exitCode"])

    def test_host_nonzero_exit_is_not_ignored(self):
        self.assert_failure({"host": {"exit": 4}}, "exited 4")

    def test_stderr_is_not_merged_into_cases(self):
        self.assert_failure({"comparison": {"stderr": "1 42\n"}}, "wrote stderr")

    def test_invalid_utf8_is_a_failure(self):
        self.assert_failure({"comparison": {"invalidUtf8": True}}, "invalid UTF-8")

    def test_missing_extra_duplicate_and_unframed_output_fail(self):
        for output, message in (("1 42\n", "case 2 missing"),
                                ("1 42\n2 Grüße\n3 extra\n", "case 3 missing"),
                                ("1 42\n1 duplicate\n", "duplicate case"),
                                ("1 42\nnoise\n", "non-case output"), ("", "no numbered cases")):
            with self.subTest(output=output):
                self.assert_failure({"comparison": {"stdout": output}}, message)

    def test_meaningful_whitespace_and_case_order_are_preserved(self):
        self.assert_failure({"comparison": {"stdout": "1 42 \n2 Grüße\n"}}, "undeclared divergence")
        self.assert_failure({"comparison": {"stdout": "2 Grüße\n1 42\n"}}, "case order differs")

    def test_retained_answers_remain_an_independent_check(self):
        self.assert_failure({"host": {"stdout": "1 7\n2 Grüße\n"},
                             "comparison": {"stdout": "1 7\n2 Grüße\n"}}, "differs from retained")

    def test_legacy_and_duplicate_declarations_fail(self):
        for declaration, error in (("#diverges 1 unnamed engine\n", "use #diverges"),
                ("#diverges node 1 reason\n#diverges node 1 again\n", "duplicate divergence")):
            with self.subTest(declaration=declaration):
                self.assert_failure({}, error, declaration)

    def test_write_preserves_all_authored_declarations(self):
        comments = "# provenance: manual\n#diverges node 1 a\n#diverges broiler-js 2 b\n"
        probe = self.fixture(declarations=comments)
        done, _ = self.run_driver("--write", comparison=False)
        self.assertEqual(0, done.returncode, done.stdout)
        self.assertTrue(probe.with_suffix(".expected.txt").read_text(encoding="utf-8").startswith(comments))
        done, _ = self.run_driver("--write")
        self.assertEqual(2, done.returncode)

    def test_typo_in_config_fails(self):
        self.fixture()
        config = json.loads(self.other.read_text())
        config["moduleArguments"] = config.pop("moduleArgs")
        self.other.write_text(json.dumps(config))
        done, _ = self.run_driver()
        self.assertEqual(1, done.returncode)
        self.assertIn("unknown engine configuration fields", done.stdout)

    def test_unicode_line_separator_is_not_a_transport_newline(self):
        self.assertEqual(["1 x\u2028y"], driver.lines("1 x\u2028y\r\n"))

    def test_existing_async_subcase_ids_are_compared(self):
        probe = self.fixture(payload={"host": {"stdout": "39a 1/2/\n39b 1/2/3/4\n"},
                                      "comparison": {"stdout": "39a 1/2/\n39b different\n"}})
        probe.with_suffix(".expected.txt").write_text("39a 1/2/\n39b 1/2/3/4\n", encoding="utf-8")
        done, _ = self.run_driver()
        self.assertEqual(1, done.returncode)
        self.assertIn("undeclared divergence broiler-js/39b", done.stdout)
        probe.with_suffix(".expected.txt").write_text(
            "#diverges broiler-js 39b subcase reason\n39a 1/2/\n39b 1/2/3/4\n", encoding="utf-8")
        done, _ = self.run_driver()
        self.assertEqual(0, done.returncode, done.stdout)


if __name__ == "__main__":
    unittest.main()
