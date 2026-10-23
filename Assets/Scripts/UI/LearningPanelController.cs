
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LearningPanelController : MonoBehaviour
{
    [Header("Painel")]
    [SerializeField] private GameObject learningPanel;
    [SerializeField] private TMP_Text learningTitle;
    [SerializeField] private TMP_Text learningContent;

    [Header("Botões")]
    [SerializeField] private Button learnButton;
    [SerializeField] private Button closeButton;

    [Header("Fases")]
    [SerializeField] private PhaseManager phaseManager;

    [SerializeField] private Button useExampleButton;
    [SerializeField] private IDEController ideController;

    private string exemploAtual;

    private void Start()
    {
        learnButton.onClick.AddListener(AbrirPainel);
        closeButton.onClick.AddListener(FecharPainel);
        useExampleButton.onClick.AddListener(UsarExemplo);

        learningPanel.SetActive(false);
    }

    public void AbrirPainel()
    {
        MostrarConteudo(phaseManager.FaseAtual);
        learningPanel.SetActive(true);
    }

    public void FecharPainel()
    {
        learningPanel.SetActive(false);
    }

    private void MostrarConteudo(int fase)
    {
        switch (fase)
        {
            case 1:
                exemploAtual = "print(\"Olá!\")";
                learningTitle.text = "Aprendendo o print()";

                learningContent.text =
                    "<b>O que é?</b>\n" +
                    "O print() é uma função do Python usada para " +
                    "mostrar informações na saída do programa.\n\n" +

                    "<b>Como usar?</b>\n" +
                    "Escreva print, abra parênteses, coloque o texto " +
                    "entre aspas e feche os parênteses.\n\n" +

                    "<b>Exemplo:</b>\n" +
                    "<color=#8ED6A5>print(\"Olá!\")</color>\n\n" +

                    "<b>Resultado:</b>\nOlá!\n\n" +

                    "<i>Dica: não esqueça as aspas e os parênteses.</i>";
                break;

            case 2:
                exemploAtual = "quantidade = 3\nprint(quantidade)";
                learningTitle.text = "Aprendendo números inteiros";

                learningContent.text =
                    "<b>O que é?</b>\n" +
                    "Uma variável guarda um valor que pode ser usado " +
                    "durante a execução do programa. O tipo int " +
                    "representa números inteiros.\n\n" +

                    "<b>Como usar?</b>\n" +
                    "Escolha um nome para a variável e atribua um número.\n\n" +

                    "<b>Exemplo:</b>\n" +
                    "<color=#8ED6A5>quantidade = 3\n" +
                    "print(quantidade)</color>\n\n" +

                    "<b>Resultado:</b>\n3\n\n" +

                    "<i>Dica: não coloque aspas ao redor do número " +
                    "se quiser armazenar um inteiro.</i>";
                break;

            default:
                learningTitle.text = "Ajuda da fase";
                learningContent.text =
                    "Ainda não há uma explicação cadastrada para esta fase.";
                break;
        }
    }

    private void OnDestroy()
    {
        learnButton.onClick.RemoveListener(AbrirPainel);
        closeButton.onClick.RemoveListener(FecharPainel);
    }

    private void UsarExemplo()
    {
        ideController.PreencherCodigo(exemploAtual);
        FecharPainel();
    }
}
