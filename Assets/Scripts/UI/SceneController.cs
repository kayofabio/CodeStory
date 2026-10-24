using TMPro;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] private TMP_Text speechText;
    [SerializeField] private GameObject babySpeechBubble;
    [SerializeField] private TMP_Text babySpeechText;

    private void Start() { OcultarBalao(); }

    public void MostrarFala(string texto)
    {
        OcultarBalao();
        speechText.text = texto;
    }

    public void PrepararPrimeirasPalavras()
    {
        MostrarFala("Toda grande historia comeca com uma pequena palavra.");
    }

    public void MostrarPrimeirasPalavras(string saida)
    {
        speechText.text = "Lia disse suas primeiras palavras!";
        MostrarSaida(saida, 1);
    }

    public void MostrarSaida(string saida, int fase)
    {
        var rect = (RectTransform)babySpeechBubble.transform;
        rect.anchorMin = fase == 1 ? new Vector2(.12f, .67f) : new Vector2(.07f, .83f);
        rect.anchorMax = fase == 1 ? new Vector2(.88f, .67f) : new Vector2(.73f, .83f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = new Vector2(0, fase == 1 ? 78 : 60);
        babySpeechText.text = saida.TrimEnd('\r', '\n');
        babySpeechBubble.SetActive(true);
    }

    public void OcultarBalao()
    {
        if (babySpeechBubble != null) babySpeechBubble.SetActive(false);
    }
}
