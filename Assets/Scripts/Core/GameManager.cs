using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LogController logController;
    [SerializeField] private SceneController sceneController;
    [SerializeField] private GameObject completionPanel;

    private Interpreter interpreter;

    private bool faseConcluida = false;

    private void Start()
    {
        interpreter = new Interpreter();
    }

    public void ExecutarCodigo(string codigo)
    {
        string resultado = interpreter.Executar(codigo);

        if (resultado != null)
        {
            logController.MostrarMensagem(
                "> Código executado com sucesso.\n" +
                "> " + resultado
            );

            sceneController.MostrarFala(resultado);

            ConcluirFase();
        }
        else
        {
            logController.MostrarMensagem(
                "> Erro: comando inválido."
            );
        }
    }

    private void ConcluirFase()
{
    if (faseConcluida)
        return;

    faseConcluida = true;

    logController.MostrarMensagem(
        "> Código executado com sucesso.\n" +
        "> Fase concluída!"
    );

    completionPanel.SetActive(true);

    Debug.Log("Fase concluída!");
}
}