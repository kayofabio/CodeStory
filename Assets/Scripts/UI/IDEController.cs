using TMPro;
using UnityEngine;

public class IDEController : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private GameManager gameManager;

    public void ExecutarCodigo()
    {
        string codigo = codeInput.text;

        gameManager.ExecutarCodigo(codigo);
    }
}