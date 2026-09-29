using TMPro;
using UnityEngine;

public class LogController : MonoBehaviour
{
    [SerializeField] private TMP_Text logText;

    public void MostrarMensagem(string mensagem)
    {
        logText.text = mensagem;
    }
}