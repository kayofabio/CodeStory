"""Run with: python -m unittest discover -s Tests -p test_first_words.py"""
import json
from pathlib import Path
import subprocess
import sys
import tempfile
import unittest


WRAPPER = Path(__file__).resolve().parents[1] / "Assets/StreamingAssets/python_wrapper.py"


class FirstWordsTests(unittest.TestCase):
    def run_code(self, code, restricted=True):
        with tempfile.TemporaryDirectory() as directory:
            script = Path(directory) / "lesson.py"
            script.write_text(code, encoding="utf-8")
            result = subprocess.run(
                [sys.executable, "-X", "utf8", str(WRAPPER), str(script)]
                + (["--apenas-print"] if restricted else []),
                capture_output=True, text=True, encoding="utf-8", timeout=5,
            )
        if result.returncode:
            return result.stderr
        return json.loads(result.stdout.split("__UNITY_RESULT__")[-1])

    def test_print_and_comments(self):
        result = self.run_code('# comentario\nprint("Ola, Lia!")\nprint("print(input())", 3, sep=" / ")')
        self.assertEqual(result["saida"], 'Ola, Lia!\nprint(input()) / 3\n')

    def test_comments_only(self):
        self.assertEqual(self.run_code("# aprendendo")["saida"], "")

    def test_other_constructs_are_rejected_before_any_output(self):
        for code in [
            'nome = "Lia"', 'import math', 'input()', 'print(input())',
            'print(1 + 2)', 'for i in range(2): print(i)',
            'if True: print("Ola")', 'print(str(3))',
            'print(f"{1 + 2}")', 'print("oi".upper())',
            'print((nome := "Lia"))', 'print = 3', 'pass',
        ]:
            with self.subTest(code=code):
                result = self.run_code('print("nao deve executar")\n' + code)
                self.assertTrue(result["erro"].startswith("FASE1_APENAS_PRINT:"))
                self.assertEqual(result["saida"], "")

    def test_syntax_errors_keep_original_feedback(self):
        self.assertIn("SyntaxError", self.run_code("print("))

    def test_phase_two_still_accepts_variables_and_arithmetic(self):
        result = self.run_code("quantidade = 1 + 2\nprint(quantidade)", restricted=False)
        self.assertEqual(result["saida"], "3\n")
        self.assertTrue(result["temQuantidade"])
        self.assertEqual(result["quantidade"], 3)


if __name__ == "__main__":
    unittest.main()
