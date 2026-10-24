
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LogController logController;
    [SerializeField] private SceneController sceneController;
    [SerializeField] private PhaseManager phaseManager;
    [SerializeField] private EndingController endingController;
    [SerializeField] private TMPro.TMP_Text objectiveText;
    [SerializeField] private UnityEngine.UI.Button nextPhaseButton;
    [SerializeField] private StoryPresentation presentation;
    public event System.Action<bool> ExecutionStateChanged;
    private bool executing;
    private PythonRunner pythonRunner;

    private PhaseEvaluator phaseEvaluator;

    private void Start()
    {
        pythonRunner = new PythonRunner();
        phaseEvaluator = new PhaseEvaluator();
        nextPhaseButton.gameObject.SetActive(false);
        nextPhaseButton.onClick.AddListener(AvancarParaProximaFase);
        if (presentation != null) presentation.ShowPhase(phaseManager.FaseAtual);
    }

    public async void ExecutarCodigo(string codigo)
    {
        if (executing) return;
        executing = true;
        if (phaseManager.FaseAtual == 1) sceneController.PrepararPrimeirasPalavras();
        else sceneController.OcultarBalao();
        ExecutionStateChanged?.Invoke(true);
        nextPhaseButton.interactable = false;
        try
        {
            logController.MostrarMensagem(
                "> Executando codigo Python..."
            );

            var resultado = await pythonRunner.ExecutarAsync(codigo, phaseManager.FaseAtual == 1);
            if (this == null) return;

            if (!string.IsNullOrEmpty(resultado.erro))
            {
                string erro = resultado.erro;

                if (erro.StartsWith("FASE1_APENAS_PRINT:"))
                {
                    string mensagem = "Lia ainda nao aprendeu a fazer isso! Nesta fase, use apenas print() para ajuda-la a falar.";
                    logController.MostrarMensagem("> " + mensagem);
                    sceneController.MostrarFala(mensagem);
                }
                else if (erro.Contains("SyntaxError"))
                {
                    logController.MostrarMensagem(
                        "> Erro de sintaxe!\n" +
                        "> Verifique parenteses, aspas e a escrita do codigo."
                    );
                }
                else if (erro.Contains("NameError"))
                {
                    logController.MostrarMensagem(
                        "> Nome nao encontrado!\n" +
                        "> Verifique se voce criou a variavel antes de usa-la."
                    );
                }
                else if (erro.Contains("ZeroDivisionError"))
                {
                    logController.MostrarMensagem(
                        "> Nao e possivel dividir um numero por zero."
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
                    "> Codigo executado com sucesso, sem saida."
                );

                return;
            }

            if (phaseManager.FaseAtual == 1)
            {
                nextPhaseButton.gameObject.SetActive(true);
            }
            else sceneController.MostrarSaida(resultado.saida, phaseManager.FaseAtual);


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
                            "> Voce precisa criar a variavel inteira quantidade."
                        );
                        return;
                    }

                    avaliacao = phaseEvaluator.AvaliarFase2(resultado.quantidade);
                    break;

                default:
                    logController.MostrarMensagem(
                        "> Esta fase ainda nao esta implementada."
                    );
                    return;
            }


            string mensagemLog =
                "> Codigo executado com sucesso.\n> " + resultado.saida;

            if (avaliacao.TemFinalAlternativo)
            {
                endingController.MostrarFinal(
                    avaliacao.TituloFinal,
                    avaliacao.MensagemFinal
                );
            }
            else
            {
                endingController.OcultarFinal();
            }

            logController.MostrarMensagem(mensagemLog);
            if (phaseManager.FaseAtual == 1)
                sceneController.MostrarPrimeirasPalavras(resultado.saida);
            else
            {
                sceneController.MostrarFala(avaliacao.Fala);
                sceneController.MostrarSaida(resultado.saida, phaseManager.FaseAtual);
            }
        }
        finally
        {
            executing = false;
            if (this != null)
            {
                nextPhaseButton.interactable = true;
                ExecutionStateChanged?.Invoke(false);
            }
        }
    }

    private void AvancarParaProximaFase()
    {
        if (!phaseManager.AvancarFase())
        {
            logController.MostrarMensagem(
                "> Voce chegou a ultima fase disponivel."
            );

            nextPhaseButton.gameObject.SetActive(false);
            return;
        }

        endingController.OcultarFinal();
        nextPhaseButton.gameObject.SetActive(false);

        sceneController.MostrarFala("Lia cresceu! Agora vamos ajuda-la a contar as macas do lanche.");
        if (presentation != null) presentation.ShowPhase(phaseManager.FaseAtual);

        objectiveText.text =
            "Fase 2 — Guardando numeros inteiros\n" +
            "Ajude Lia a separar as macas. " +
            "Crie a variavel inteira quantidade e use print() para exibi-la.";

        logController.MostrarMensagem(
            "> Fase 2 iniciada. Prepare seu codigo Python!"
        );
    }

}
