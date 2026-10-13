
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LogController logController;
    [SerializeField] private SceneController sceneController;

    private PythonRunner pythonRunner;

    private void Start()
    {
        pythonRunner = new PythonRunner();
    }

    public async void ExecutarCodigo(string codigo)
    {
        logController.MostrarMensagem(
            "> Executando código Python..."
        );

        var resultado = await pythonRunner.ExecutarAsync(codigo);

        if (!string.IsNullOrEmpty(resultado.erro))
        {
            string erro = resultado.erro;

            if (erro.Contains("SyntaxError"))
            {
                logController.MostrarMensagem(
                    "> Erro de sintaxe!\n" +
                    "> Verifique parênteses, aspas e a escrita do código."
                );
            }
            else if (erro.Contains("NameError"))
            {
                logController.MostrarMensagem(
                    "> Nome não encontrado!\n" +
                    "> Verifique se você criou a variável antes de usá-la."
                );
            }
            else if (erro.Contains("ZeroDivisionError"))
            {
                logController.MostrarMensagem(
                    "> Não é possível dividir um número por zero."
                );
            }
            else
            {
                logController.MostrarMensagem(
                    "> O Python encontrou um erro:\n" + erro
                );
            }

            return;
        }

        logController.MostrarMensagem(
            string.IsNullOrEmpty(resultado.saida)
                ? "> Código executado com sucesso, sem saída."
                : "> Código executado com sucesso.\n> " + resultado.saida
        );

        if (!string.IsNullOrEmpty(resultado.saida))
        {
            sceneController.MostrarFala(resultado.saida);
        }
    }
}
