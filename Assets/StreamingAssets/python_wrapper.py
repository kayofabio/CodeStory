
import contextlib
import io
import json
import runpy
import sys

caminho_codigo = sys.argv[1]
captura = io.StringIO()

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
