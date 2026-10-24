using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IDEController : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button executeButton;
    [SerializeField] private Button clearButton;
    [SerializeField] private Button hintButton;
    [SerializeField] private PhaseManager phaseManager;
    [SerializeField] private LogController logController;

    private void OnEnable() { gameManager.ExecutionStateChanged += SetRunning; }
    private void OnDisable() { gameManager.ExecutionStateChanged -= SetRunning; }
    private void SetRunning(bool running)
    {
        if (executeButton == null) return;
        executeButton.interactable = !running;
        if (clearButton != null) clearButton.interactable = !running;
        if (hintButton != null) hintButton.interactable = !running;
        executeButton.GetComponentInChildren<TMP_Text>().text = running ? "Executando..." : "Executar";
    }
    public void ExecutarCodigo() { gameManager.ExecutarCodigo(codeInput.text); }
    public void LimparCodigo() { codeInput.text = ""; codeInput.ActivateInputField(); }
    public void MostrarDica()
    {
        logController.MostrarMensagem(phaseManager.FaseAtual == 1
            ? "> DICA\nUse print() e coloque sua mensagem entre aspas.\nExemplo: print(\"Ola, mundo!\")"
            : "> DICA\nCrie uma variavel inteira, sem aspas, e mostre seu valor.\nquantidade = 3\nprint(quantidade)");
    }
}
