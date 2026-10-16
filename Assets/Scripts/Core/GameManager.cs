
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LogController logController;
    [SerializeField] private SceneController sceneController;
    [SerializeField] private PhaseManager phaseManager;
    private PythonRunner pythonRunner;

    private PhaseEvaluator phaseEvaluator;

    private void Start()
    {
        pythonRunner = new PythonRunner();
        phaseEvaluator = new PhaseEvaluator();
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


        if (string.IsNullOrEmpty(resultado.saida))
        {
            logController.MostrarMensagem(
                "> Código executado com sucesso, sem saída."
            );

            return;
        }


        ResultadoFase avaliacao;

        switch (phaseManager.FaseAtual)
        {
            case 1:
                avaliacao = phaseEvaluator.AvaliarFase1(resultado.saida);
                break;

            case 2:
                if (!resultado.temQuantidade)
                {
                    logController.MostrarMensagem(
                        "> Você precisa criar a variável inteira quantidade."
                    );
                    return;
                }

                avaliacao = phaseEvaluator.AvaliarFase2(resultado.quantidade);
                break;

            default:
                logController.MostrarMensagem(
                    "> Esta fase ainda não está implementada."
                );
                return;
        }


        string mensagemLog =
            "> Código executado com sucesso.\n> " + resultado.saida;

        if (avaliacao.TemFinalAlternativo)
        {
            mensagemLog +=
                "\n\n> " + avaliacao.TituloFinal +
                "\n> " + avaliacao.MensagemFinal;
        }

        logController.MostrarMensagem(mensagemLog);
        sceneController.MostrarFala(avaliacao.Fala);

    }
}
