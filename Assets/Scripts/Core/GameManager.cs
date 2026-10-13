
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
            logController.MostrarMensagem(
                "> Erro na execução:\n" + resultado.erro
            );

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
