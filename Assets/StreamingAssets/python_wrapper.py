
import ast
import contextlib
import io
import json
import runpy
import sys

caminho_codigo = sys.argv[1]
captura = io.StringIO()

# Validate the entire program before executing any part of the first lesson.
# Only direct print calls with literal arguments are taught at this stage.
if "--apenas-print" in sys.argv[2:]:
    with open(caminho_codigo, encoding="utf-8-sig") as arquivo:
        arvore = ast.parse(arquivo.read(), filename=caminho_codigo)

    def print_permitido(instrucao):
        if not isinstance(instrucao, ast.Expr):
            return False
        chamada = instrucao.value
        return (
            isinstance(chamada, ast.Call)
            and isinstance(chamada.func, ast.Name)
            and chamada.func.id == "print"
            and all(isinstance(arg, ast.Constant) for arg in chamada.args)
            and all(
                item.arg in ("sep", "end") and isinstance(item.value, ast.Constant)
                for item in chamada.keywords
            )
        )

    if not all(print_permitido(item) for item in arvore.body):
        print("__UNITY_RESULT__" + json.dumps({
            "saida": "",
            "erro": "FASE1_APENAS_PRINT: comando ainda nao aprendido",
            "temQuantidade": False,
            "quantidade": 0
        }))
        sys.exit(0)

with contextlib.redirect_stdout(captura):
    variaveis = runpy.run_path(
        caminho_codigo,
        run_name="__main__"
    )

quantidade = variaveis.get("quantidade")
tem_quantidade = type(quantidade) is int

resposta = {
    "saida": captura.getvalue(),
    "temQuantidade": tem_quantidade,
    "quantidade": quantidade if tem_quantidade else 0
}

print(
    "__UNITY_RESULT__" +
    json.dumps(resposta, ensure_ascii=False)
)
